using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace AgendaCalendario.Models;

/// <summary>
/// Modelo que representa um utilizador no sistema
/// </summary>
public class Utilizador
{
    /// <summary>
    /// Identificador único do utilizador
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Nome do utilizador
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string Nome { get; set; } = string.Empty;

    /// <summary>
    /// Email do utilizador (usado para login)
    /// </summary>
    [Required]
    [EmailAddress]
    [MaxLength(150)]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Hash da senha do utilizador
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string PasswordHash { get; set; } = string.Empty;

    /// <summary>
    /// Perfil/Role do utilizador no sistema
    /// </summary>
    [Required]
    [MaxLength(20)]
    public string PerfilUtilizador { get; set; } = "Utilizador";

    /// <summary>
    /// Coleção de tarefas do utilizador
    /// </summary>
    public ICollection<Tarefa> Tarefas { get; set; } = new List<Tarefa>();

    /// <summary>
    /// Coleção de categorias do utilizador
    /// </summary>
    public ICollection<Categoria> Categorias { get; set; } = new List<Categoria>();

    /// <summary>
    /// Código para confirmação de email
    /// </summary>
    public string? CodigoConfirmacao { get; set; }

    /// <summary>
    /// Indica se o email foi confirmado
    /// </summary>
    public bool EmailConfirmado { get; set; } = false;
}