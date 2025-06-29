namespace AgendaCalendario.Models.API_Dtos
{
    /// <summary>
    /// DTO (Data Transfer Object) para criação de uma nova tarefa
    /// </summary>
    public class TarefaCreateDto
    {
        /// <summary>
        /// Título da tarefa
        /// </summary>
        public string Titulo { get; set; }

        /// <summary>
        /// Descrição detalhada da tarefa
        /// </summary>
        public string Descricao { get; set; }

        /// <summary>
        /// Data e hora da tarefa
        /// </summary>
        public DateTime Data { get; set; }

        /// <summary>
        /// ID do utilizador que está a criar a tarefa
        /// </summary>
        public int UtilizadorId { get; set; }

        /// <summary>
        /// Tipo de recorrência da tarefa (única, diária, semanal, etc.)
        /// </summary>
        public TipoRecorrencia Recorrencia { get; set; }

        /// <summary>
        /// Data limite para tarefas recorrentes (opcional)
        /// </summary>
        public DateTime? DataFimRecorrencia { get; set; }

        /// <summary>
        /// Lista de IDs das categorias associadas à tarefa
        /// </summary>
        public List<int> CategoriasIds { get; set; } = new();
    }
}