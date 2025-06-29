using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace AgendaCalendario.Models;

/// <summary>
/// Tipos possíveis de recorrência para uma tarefa
/// </summary>
public enum TipoRecorrencia
{
    Nenhuma = 0,
    Diaria = 1,
    Semanal = 2,
    Mensal = 3
}

/// <summary>
/// Modelo que representa uma tarefa na agenda
/// </summary>
public class Tarefa
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
    /// Descrição detalhada da tarefa
    /// </summary>
    public string Descricao { get; set; }

    /// <summary>
    /// Data de execução da tarefa
    /// </summary>
    public DateTime Data { get; set; }

    /// <summary>
    /// ID do utilizador proprietário da tarefa
    /// </summary>
    public int UtilizadorId { get; set; }

    /// <summary>
    /// Navegação para o utilizador proprietário
    /// </summary>
    public Utilizador Utilizador { get; set; }

    /// <summary>
    /// Tipo de recorrência da tarefa
    /// </summary>
    public TipoRecorrencia Recorrencia { get; set; }

    /// <summary>
    /// Data final para tarefas recorrentes (opcional)
    /// </summary>
    public DateTime? DataFimRecorrencia { get; set; }

    /// <summary>
    /// Coleção de categorias associadas à tarefa
    /// </summary>
    public ICollection<Categoria> Categorias { get; set; } = new List<Categoria>();
}