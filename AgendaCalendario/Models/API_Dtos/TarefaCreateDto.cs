namespace AgendaCalendario.Models.API_Dtos
{
    public class TarefaCreateDto
    {
        public string Titulo { get; set; } = null!;
        public string? Descricao { get; set; }
        public DateTime Data { get; set; }

        public int UtilizadorId { get; set; }

        // Categoria é opcional
        public int? CategoriaId { get; set; }
    }
}