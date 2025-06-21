using Microsoft.AspNetCore.Mvc;

namespace AgendaCalendario.Controllers
{
    public class CalendarioController : Controller
    {
        public IActionResult Index()
        {
            // Verificar se o utilizador está autenticado
            if (HttpContext.Session.GetInt32("UtilizadorId") == null)
            {
                return RedirectToAction("Login", "Conta");
            }

            // Em breve: carregar tarefas específicas deste utilizador

            return View();
        }
    }
}