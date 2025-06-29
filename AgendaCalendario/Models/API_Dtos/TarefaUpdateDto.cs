namespace AgendaCalendario.Models.API_Dtos
{
    public class TarefaUpdateDto
    {
        public int Id { get; set; } // necessário para PUT
        public string Titulo { get; set; }
        public string Descricao { get; set; }
        public DateTime Data { get; set; }
        public int UtilizadorId { get; set; }
        public TipoRecorrencia Recorrencia { get; set; }
        public DateTime? DataFimRecorrencia { get; set; }
        public List<int> CategoriasIds { get; set; } = new();
    }

}