namespace AgendaCalendario.Models;

public class TarefaDtoViewModel
{
    public int Id { get; set; }
    public string Titulo { get; set; }
    public string Descricao { get; set; }
    public DateTime Data { get; set; }
    public List<CategoriaResumoDto> Categorias { get; set; } = new();
}

public class CategoriaResumoDto
{
    public int Id { get; set; }
    public string Nome { get; set; }
    public string Cor { get; set; }
}