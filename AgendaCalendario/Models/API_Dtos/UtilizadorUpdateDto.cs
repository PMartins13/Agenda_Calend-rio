using System.ComponentModel.DataAnnotations;

namespace AgendaCalendario.Models.API_Dtos
{
    /// <summary>
    /// DTO (Data Transfer Object) para atualização de um utilizador existente
    /// </summary>
    public class UtilizadorUpdateDto
    {
        /// <summary>
        /// ID do utilizador a ser atualizado
        /// </summary>
        [Required]
        public int Id { get; set; }

        /// <summary>
        /// Novo nome do utilizador (obrigatório, máximo 100 caracteres)
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string Nome { get; set; }

        /// <summary>
        /// Novo email do utilizador (obrigatório, deve ser um email válido)
        /// </summary>
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        /// <summary>
        /// Nova password (opcional - só será atualizada se fornecida)
        /// </summary>
        public string? Password { get; set; }
    }
}