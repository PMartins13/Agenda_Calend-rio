using Microsoft.AspNetCore.Mvc;

namespace AgendaCalendario.Controllers
{
    /// <summary>
    /// Controlador responsável pela gestão da vista do calendário
    /// </summary>
    public class CalendarioController : Controller
    {
        /// <summary>
        /// Ação principal que exibe a vista do calendário
        /// </summary>
        /// <returns>Vista do calendário ou redireciona para login se não autenticado</returns>
        public IActionResult Index()
        {
            // Verifica se existe um ID de utilizador na sessão
            // Se não existir, significa que o utilizador não está autenticado
            if (HttpContext.Session.GetInt32("UtilizadorId") == null)
            {
                // Redireciona para a página de login
                return RedirectToAction("Login", "Conta");
            }

            // TODO: Implementar carregamento de tarefas do utilizador
            // Futura implementação para mostrar apenas as tarefas
            // específicas do utilizador autenticado

            // Retorna a vista padrão do calendário
            return View();
        }
    }
}