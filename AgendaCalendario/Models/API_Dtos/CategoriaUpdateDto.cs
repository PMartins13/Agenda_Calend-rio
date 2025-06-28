using System.ComponentModel.DataAnnotations;

namespace AgendaCalendario.Models.API_Dtos
{
    public class CategoriaUpdateDto
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Nome { get; set; }

        [MaxLength(100)]
        public string Cor { get; set; }

        [Required]
        public int UtilizadorId { get; set; } // Campo necessário para cumprir a FK
    }
}