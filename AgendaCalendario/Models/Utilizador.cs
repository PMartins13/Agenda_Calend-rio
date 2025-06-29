using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace AgendaCalendario.Models;

public class Utilizador
{
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Nome { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [MaxLength(150)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string PasswordHash { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    public string PerfilUtilizador { get; set; } = "Utilizador";

    // Relação: 1 Utilizador tem muitas Tarefas
    public ICollection<Tarefa> Tarefas { get; set; } = new List<Tarefa>();

    // Relação: 1 Utilizador tem muitas Categorias
    public ICollection<Categoria> Categorias { get; set; } = new List<Categoria>();

    public string? CodigoConfirmacao { get; set; }
    public bool EmailConfirmado { get; set; } = false;

}