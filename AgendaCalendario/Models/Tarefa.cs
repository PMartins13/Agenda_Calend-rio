using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using AgendaCalendario.Models;

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
    public string Titulo { get; set; }
    public string Descricao { get; set; }
    public DateTime Data { get; set; }

    public int UtilizadorId { get; set; }
    public Utilizador Utilizador { get; set; }

    // Remover este:
    // public int? CategoriaId { get; set; }
    // public Categoria Categoria { get; set; }

    public TipoRecorrencia Recorrencia { get; set; }
    public DateTime? DataFimRecorrencia { get; set; }

    // 👇 ADICIONAR ISTO:
    public ICollection<Categoria> Categorias { get; set; } = new List<Categoria>();
}
