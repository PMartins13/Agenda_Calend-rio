using System.ComponentModel.DataAnnotations;

namespace AgendaCalendario.Models.API_Dtos
{
    /// <summary>
    /// DTO (Data Transfer Object) para criação de um novo utilizador
    /// </summary>
    public class UtilizadorCreateDto
    {
        /// <summary>
        /// Nome do utilizador (obrigatório, máximo 100 caracteres)
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string Nome { get; set; }

        /// <summary>
        /// Email do utilizador (obrigatório, deve ser um email válido)
        /// </summary>
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        /// <summary>
        /// Password do utilizador (obrigatória, será encriptada antes do armazenamento)
        /// </summary>
        [Required]
        public string Password { get; set; }
    }
}