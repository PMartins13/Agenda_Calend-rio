using System.ComponentModel.DataAnnotations;

namespace AgendaCalendario.Models
{
    public class UtilizadorPerfilViewModel
    {
        [Required]
        [StringLength(100)]
        public string Nome { get; set; } = null!;

        [Required]
        [EmailAddress]
        [StringLength(150)]
        public string Email { get; set; } = null!;

        [DataType(DataType.Password)]
        public string? NovaPassword { get; set; }

        [DataType(DataType.Password)]
        [Compare("NovaPassword", ErrorMessage = "As passwords não coincidem.")]
        public string? ConfirmarPassword { get; set; }
    }
}