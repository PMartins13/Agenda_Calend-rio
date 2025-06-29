namespace AgendaCalendario.Models;

/// <summary>
/// DTO para visualização resumida de uma tarefa com suas categorias
/// </summary>
public class TarefaDtoViewModel
{
    /// <summary>
    /// Identificador único da tarefa
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Título da tarefa
    /// </summary>
    public string Titulo { get; set; }

    /// <summary>
    /// Descrição da tarefa
    /// </summary>
    public string Descricao { get; set; }

    /// <summary>
    /// Data da tarefa
    /// </summary>
    public DateTime Data { get; set; }

    /// <summary>
    /// Lista de categorias associadas à tarefa
    /// </summary>
    public List<CategoriaResumoDto> Categorias { get; set; } = new();
}

/// <summary>
/// DTO para exibição resumida de uma categoria
/// </summary>
public class CategoriaResumoDto
{
    /// <summary>
    /// Identificador único da categoria
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Nome da categoria
    /// </summary>
    public string Nome { get; set; }

    /// <summary>
    /// Cor associada à categoria
    /// </summary>
    public string Cor { get; set; }
}