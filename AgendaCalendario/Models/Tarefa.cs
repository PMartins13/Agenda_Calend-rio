using System.ComponentModel.DataAnnotations;

namespace AgendaCalendario.Models;

public class Tarefa
{
    public int Id { get; set; }

    [Required]
    public string Titulo { get; set; }

    public string Descricao { get; set; }

    [Required]
    public DateTime Data { get; set; }

    // FK para o utilizador
    public int UtilizadorId { get; set; }
    public Utilizador Utilizador { get; set; }

    // FK para a categoria (opcional)
    public int? CategoriaId { get; set; }
    public Categoria Categoria { get; set; }
}