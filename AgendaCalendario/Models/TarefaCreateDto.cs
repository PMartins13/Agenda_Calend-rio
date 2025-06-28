namespace AgendaCalendario.Models
{
    public class TarefaCreateDto
    {
        public string Titulo { get; set; }
        public string? Descricao { get; set; }
        public DateTime Data { get; set; }
        public int UtilizadorId { get; set; }
        public int CategoriaId { get; set; }
    }
}