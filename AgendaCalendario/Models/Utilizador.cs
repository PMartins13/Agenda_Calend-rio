using System.ComponentModel.DataAnnotations;

namespace AgendaCalendario.Models;

public class Utilizador
{
    public int Id { get; set; }

    [Required]
    [EmailAddress]
    [MaxLength(150)]
    public string Email { get; set; }

    [Required]
    [MaxLength(200)]
    public string PasswordHash { get; set; }

    // Relação: 1 Utilizador tem muitas Tarefas
    public ICollection<Tarefa> Tarefas { get; set; }

    // Relação: 1 Utilizador tem muitas Categorias
    public ICollection<Categoria> Categorias { get; set; }
}