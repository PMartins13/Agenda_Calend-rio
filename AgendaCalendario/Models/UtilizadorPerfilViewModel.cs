using System.ComponentModel.DataAnnotations;

namespace AgendaCalendario.Models
{
    /// <summary>
    /// ViewModel para edição de perfil do utilizador
    /// </summary>
    public class UtilizadorPerfilViewModel
    {
        /// <summary>
        /// Nome do utilizador
        /// </summary>
        [Required]
        [StringLength(100)]
        public string Nome { get; set; } = null!;

        /// <summary>
        /// Email do utilizador
        /// </summary>
        [Required]
        [EmailAddress]
        [StringLength(150)]
        public string Email { get; set; } = null!;

        /// <summary>
        /// Nova password (opcional para alteração)
        /// </summary>
        [DataType(DataType.Password)]
        public string? NovaPassword { get; set; }

        /// <summary>
        /// Confirmação da nova password
        /// </summary>
        [DataType(DataType.Password)]
        [Compare("NovaPassword", ErrorMessage = "As passwords não coincidem.")]
        public string? ConfirmarPassword { get; set; }
    }
}