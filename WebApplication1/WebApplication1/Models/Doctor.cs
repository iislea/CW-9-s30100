using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models;

[Table(name:"Doctor")]
public class Doctor
{
        [Key]
        public int IdDoctor { get; set; }

        [MaxLength(100)] public string FirstName { get; set; } = null!;
        [MaxLength(100)] public string LastName { get; set; } = null!;
        [MaxLength(100)]
        public string Email { get; set; }

        public virtual ICollection<Prescription> Prescriptions { get; set; } = new List<Prescription>();
    }

