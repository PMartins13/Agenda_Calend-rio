using AgendaCalendario.Models;

public class SuaClasse
{
    // ...código existente...

    public TipoRecorrencia Recorrencia { get; set; } = TipoRecorrencia.Nenhuma;
    public DateTime? DataFimRecorrencia { get; set; }

    // ...código existente...
}