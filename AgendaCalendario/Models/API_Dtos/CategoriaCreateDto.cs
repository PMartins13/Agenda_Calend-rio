using System.ComponentModel.DataAnnotations;

namespace AgendaCalendario.Models.API_Dtos
{
    public class CategoriaCreateDto
    {
        [Required]
        public string Nome { get; set; }

        public string Cor { get; set; }

        [Required]
        public int UtilizadorId { get; set; }
    }
}