using System.ComponentModel.DataAnnotations;

namespace AgendaCalendario.Models;

/// <summary>
/// Modelo que representa uma categoria de tarefas
/// </summary>
public class Categoria
{
    /// <summary>
    /// Identificador único da categoria
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Nome da categoria (obrigatório, máximo 50 caracteres)
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string Nome { get; set; } = string.Empty;

    /// <summary>
    /// Cor para representação visual da categoria (máximo 100 caracteres)
    /// </summary>
    [MaxLength(100)]
    public string Cor { get; set; } = string.Empty;

    /// <summary>
    /// ID do utilizador proprietário da categoria
    /// </summary>
    public int UtilizadorId { get; set; }

    /// <summary>
    /// Navegação para o utilizador proprietário
    /// </summary>
    public Utilizador Utilizador { get; set; } = null!;

    /// <summary>
    /// Coleção de tarefas associadas a esta categoria
    /// </summary>
    public ICollection<Tarefa> Tarefas { get; set; } = new List<Tarefa>();
}