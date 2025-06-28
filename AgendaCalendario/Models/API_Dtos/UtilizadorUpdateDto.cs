using System.ComponentModel.DataAnnotations;

namespace AgendaCalendario.Models.API_Dtos
{
    public class UtilizadorUpdateDto
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Nome { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public string? Password { get; set; } // só atualiza se for fornecida
    }
}