namespace WebApplication1.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;
using WebApplication1.DTOs;

[ApiController]
[Route("api/[controller]")]
public class PrescriptionController : ControllerBase
{
    private readonly AppDbContext _context;

    public PrescriptionController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> AddPrescription(AddPrescriptionRequest request)
    {
        if (request.Medicaments.Count > 10)
            return BadRequest("Prescription cannot contain more than 10 medicaments.");

        if (request.DueDate < request.Date)
            return BadRequest("DueDate must be later or equal to Date.");

        var doctor = await _context.Doctors.FindAsync(request.IdDoctor);
        if (doctor == null)
            return NotFound("Doctor not found.");

        var patient = await _context.Patients
            .FirstOrDefaultAsync(p => p.FirstName == request.FirstName &&
                                      p.LastName == request.LastName &&
                                      p.Birthdate == request.Birthdate);
        if (patient == null)
        {
            patient = new Patient
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Birthdate = request.Birthdate
            };
            _context.Patients.Add(patient);
            await _context.SaveChangesAsync();
        }

        var invalidMedicaments = request.Medicaments
            .Where(m => !_context.Medicaments.Any(dbm => dbm.IdMedicament == m.IdMedicament))
            .ToList();

        if (invalidMedicaments.Any())
            return NotFound($"Invalid medicament IDs: {string.Join(",", invalidMedicaments.Select(x => x.IdMedicament))}");

        var prescription = new Prescription
        {
            Date = request.Date,
            DueDate = request.DueDate,
            IdDoctor = request.IdDoctor,
            IdPatient = patient.IdPatient,
            PrescriptionMedicaments = request.Medicaments.Select(m => new PrescriptionMedicament
            {
                IdMedicament = m.IdMedicament,
                Dose = m.Dose,
                Description = m.Description
            }).ToList()
        };

        _context.Prescriptions.Add(prescription);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetPatient), new { id = patient.IdPatient }, null);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetPatient(int id)
    {
        var patient = await _context.Patients
            .Include(p => p.Prescriptions)
                .ThenInclude(pr => pr.Doctor)
            .Include(p => p.Prescriptions)
                .ThenInclude(pr => pr.PrescriptionMedicaments)
                    .ThenInclude(pm => pm.Medicament)
            .FirstOrDefaultAsync(p => p.IdPatient == id);

        if (patient == null) return NotFound("Patient not found.");

        var result = new PatientDetailsDto
        {
            IdPatient = patient.IdPatient,
            FirstName = patient.FirstName,
            LastName = patient.LastName,
            Birthdate = patient.Birthdate,
            Prescriptions = patient.Prescriptions
                .OrderBy(p => p.DueDate)
                .Select(p => new PrescriptionDto
                {
                    IdPrescription = p.IdPrescription,
                    Date = p.Date,
                    DueDate = p.DueDate,
                    Doctor = new DoctorDto
                    {
                        IdDoctor = p.Doctor.IdDoctor,
                        FirstName = p.Doctor.FirstName,
                        LastName = p.Doctor.LastName
                    },
                    Medicaments = p.PrescriptionMedicaments.Select(pm => new MedicamentDtoExtended
                    {
                        IdMedicament = pm.IdMedicament,
                        Name = pm.Medicament.Name,
                        Dose = pm.Dose,
                        Description = pm.Description
                    }).ToList()
                }).ToList()
        };

        return Ok(result);
    }
}
