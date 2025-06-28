using System.ComponentModel.DataAnnotations;

namespace AgendaCalendario.Models;

public enum TipoRecorrencia
{
    Nenhuma = 0,
    Diaria = 1,
    Semanal = 2,
    Mensal = 3
}

public class Tarefa
{
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string Titulo { get; set; } = string.Empty;

    [MaxLength(500)]
    public string Descricao { get; set; } = string.Empty;

    [Required]
    public DateTime Data { get; set; }

    // FK para o utilizador
    public int UtilizadorId { get; set; }
    public Utilizador Utilizador { get; set; } = null!;

    // FK para a categoria (opcional)
    public int? CategoriaId { get; set; }
    public Categoria? Categoria { get; set; }

    // Recorrência
    public TipoRecorrencia Recorrencia { get; set; } = TipoRecorrencia.Nenhuma;
    public DateTime? DataFimRecorrencia { get; set; }
}