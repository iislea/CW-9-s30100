namespace WebApplication1.DTOs;

public class AddPrescriptionRequest
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Birthdate { get; set; }

    public int IdDoctor { get; set; }

    public DateTime Date { get; set; }
    public DateTime DueDate { get; set; }

    public List<MedicamentDto> Medicaments { get; set; }
}

public class MedicamentDto
{
    public int IdMedicament { get; set; }
    public int Dose { get; set; }
    public string Description { get; set; }
}
