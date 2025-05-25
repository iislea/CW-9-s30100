using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models;

[Table(name:"PrescriptionMedicament")]
public class PrescriptionMedicament
{
    
    [Key]
    public int IdMedicament { get; set; }
    public Medicament Medicament { get; set; }
 
    public int IdPrescription { get; set; }
    public Prescription Prescription { get; set; }

    public int Dose { get; set; }
    public string Description { get; set; }
}
