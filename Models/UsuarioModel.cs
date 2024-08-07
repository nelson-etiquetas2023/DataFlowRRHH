using System.ComponentModel.DataAnnotations;

namespace DataFlowRRHH.Models
{
    public class UsuarioModel
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = null!;
        [Required]
        public string Email { get; set; } = null!;
        [Required]
        public string Password { get; set; } = null!;
        [Required]
        public string TypeUser { get; set; } = null!;
        public string Phone { get; set; } = null!;
        [Required]
        public string Departament { get; set; } = null!;
        public bool Active { get; set; }
    }
}
