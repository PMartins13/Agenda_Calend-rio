using System.ComponentModel.DataAnnotations;

namespace AgendaCalendario.Models.API_Dtos
{
    /// <summary>
    /// DTO (Data Transfer Object) para criação de uma nova categoria
    /// </summary>
    public class CategoriaCreateDto
    {
        /// <summary>
        /// Nome da categoria (obrigatório)
        /// </summary>
        [Required]
        public string Nome { get; set; }

        /// <summary>
        /// Cor da categoria em formato hexadecimal (opcional)
        /// </summary>
        public string Cor { get; set; }

        /// <summary>
        /// ID do utilizador que está a criar a categoria (obrigatório)
        /// </summary>
        [Required]
        public int UtilizadorId { get; set; }
    }
}