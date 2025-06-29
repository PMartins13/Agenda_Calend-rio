namespace AgendaCalendario.Models;

/// <summary>
/// Modelo para exibição de informações de erro na aplicação
/// </summary>
public class ErrorViewModel
{
    /// <summary>
    /// Identificador único da requisição que gerou o erro
    /// </summary>
    public string? RequestId { get; set; }

    /// <summary>
    /// Indica se o RequestId deve ser mostrado na view
    /// Retorna true apenas se RequestId não for nulo ou vazio
    /// </summary>
    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}