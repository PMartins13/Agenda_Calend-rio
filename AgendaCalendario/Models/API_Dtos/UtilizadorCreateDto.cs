using System.ComponentModel.DataAnnotations;

namespace AgendaCalendario.Models.API_Dtos
{
    public class UtilizadorCreateDto
    {
        [Required]
        [MaxLength(100)]
        public string Nome { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }  // será encriptada no controlador
    }
}