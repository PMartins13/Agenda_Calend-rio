using System.ComponentModel.DataAnnotations;

namespace AgendaCalendario.Models.API_Dtos
{
    /// <summary>
    /// DTO (Data Transfer Object) para atualização de uma categoria existente
    /// </summary>
    public class CategoriaUpdateDto
    {
        /// <summary>
        /// ID da categoria a ser atualizada
        /// </summary>
        [Required]
        public int Id { get; set; }

        /// <summary>
        /// Novo nome da categoria (limitado a 50 caracteres)
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string Nome { get; set; }

        /// <summary>
        /// Nova cor da categoria em formato hexadecimal (limitado a 100 caracteres)
        /// </summary>
        [MaxLength(100)]
        public string Cor { get; set; }

        /// <summary>
        /// ID do utilizador dono da categoria (necessário para validação de FK)
        /// </summary>
        [Required]
        public int UtilizadorId { get; set; }
    }
}