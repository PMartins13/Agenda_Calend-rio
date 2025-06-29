namespace AgendaCalendario.Models.API_Dtos
{
    /// <summary>
    /// DTO (Data Transfer Object) para atualização de uma tarefa existente
    /// </summary>
    public class TarefaUpdateDto
    {
        /// <summary>
        /// ID da tarefa a ser atualizada (requerido para operações PUT)
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Novo título da tarefa
        /// </summary>
        public string Titulo { get; set; }

        /// <summary>
        /// Nova descrição da tarefa
        /// </summary>
        public string Descricao { get; set; }

        /// <summary>
        /// Nova data e hora da tarefa
        /// </summary>
        public DateTime Data { get; set; }

        /// <summary>
        /// ID do utilizador dono da tarefa
        /// </summary>
        public int UtilizadorId { get; set; }

        /// <summary>
        /// Novo tipo de recorrência da tarefa
        /// </summary>
        public TipoRecorrencia Recorrencia { get; set; }

        /// <summary>
        /// Nova data limite para tarefas recorrentes (opcional)
        /// </summary>
        public DateTime? DataFimRecorrencia { get; set; }

        /// <summary>
        /// Nova lista de IDs das categorias associadas
        /// </summary>
        public List<int> CategoriasIds { get; set; } = new();
    }
}