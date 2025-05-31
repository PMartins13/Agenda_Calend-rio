using System.ComponentModel.DataAnnotations;

namespace AgendaCalendario.Models;

public class Utilizador
{
    public int Id { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    public string PasswordHash { get; set; }

    // Relação: 1 Utilizador tem muitas Tarefas
    public ICollection<Tarefa> Tarefas { get; set; }

    // Relação: 1 Utilizador tem muitas Categorias
    public ICollection<Categoria> Categorias { get; set; }
}