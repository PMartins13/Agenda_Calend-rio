using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AgendaCalendario.Models
{
    /// <summary>
    /// Modelo para entrada de dados de uma tarefa
    /// </summary>
    public class TarefaInputModel
    {
        /// <summary>
        /// Identificador da tarefa (opcional para novas tarefas)
        /// </summary>
        public int? Id { get; set; }

        /// <summary>
        /// Título da tarefa (obrigatório)
        /// </summary>
        [Required]
        public string Titulo { get; set; }

        /// <summary>
        /// Descrição detalhada da tarefa
        /// </summary>
        public string Descricao { get; set; }

        /// <summary>
        /// Data de execução da tarefa (obrigatória)
        /// </summary>
        [Required]
        public DateTime Data { get; set; }

        /// <summary>
        /// Tipo de recorrência da tarefa
        /// </summary>
        public TipoRecorrencia Recorrencia { get; set; }

        /// <summary>
        /// Data final para tarefas recorrentes (opcional)
        /// </summary>
        public DateTime? DataFimRecorrencia { get; set; }

        /// <summary>
        /// Lista de IDs das categorias associadas
        /// </summary>
        public List<int> CategoriasIds { get; set; } = new();

        /// <summary>
        /// ID do utilizador proprietário da tarefa
        /// </summary>
        public int UtilizadorId { get; set; }
    }
}