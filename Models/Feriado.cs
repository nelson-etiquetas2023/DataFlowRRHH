using System.ComponentModel.DataAnnotations;

namespace DataFlowRRHH.Models
{
    public class Feriado
    {
        public int IdException { get; set; }
        [Required(ErrorMessage = "la fecha es requeridos")]
        public DateTime BeginingDate { get; set; }
        [Required(ErrorMessage = "la fecha es requeridos")]
        public DateTime EndingDate { get; set; }

        [Required(ErrorMessage = "la descripcion del dia feriado es requerida...")]
        [StringLength(60, MinimumLength = 3)]
        public string? Description { get; set; }
        public Boolean Recurring { get; set; }
        [Range(1, 100)]
        [Required]
        public int PaymentFactor { get; set; }
        public string? Comment { get; set; }
    }
}
