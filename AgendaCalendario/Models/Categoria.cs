using System.ComponentModel.DataAnnotations;

namespace AgendaCalendario.Models;

public class Categoria
{
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string Nome { get; set; } = string.Empty;

    // Cor visual associada à categoria
    [MaxLength(100)]
    public string Cor { get; set; } = string.Empty;

    // FK para o utilizador
    public int UtilizadorId { get; set; }
    public Utilizador Utilizador { get; set; } = null!;

    // Relação com tarefas
    public ICollection<Tarefa> Tarefas { get; set; } = new List<Tarefa>();
}