using System.ComponentModel.DataAnnotations;

namespace AgendaCalendario.Models;

public class Categoria
{
    public int Id { get; set; }

    [Required]
    public string Nome { get; set; }

    // Cor visual associada à categoria
    public string Cor { get; set; }

    // FK para o utilizador
    public int UtilizadorId { get; set; }
    public Utilizador Utilizador { get; set; }

    // Relação com tarefas
    public ICollection<Tarefa> Tarefas { get; set; }
}