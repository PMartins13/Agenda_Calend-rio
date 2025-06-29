using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using AgendaCalendario.Models;

namespace AgendaCalendario.Controllers;

/// <summary>
/// Controlador responsável pelas páginas principais da aplicação
/// </summary>
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    /// <summary>
    /// Construtor que injeta o serviço de logging
    /// </summary>
    /// <param name="logger">Serviço de logging</param>
    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Página inicial da aplicação
    /// </summary>
    /// <returns>Vista da página inicial</returns>
    public IActionResult Index()
    {
        return View();
    }

    /// <summary>
    /// Página de política de privacidade
    /// </summary>
    /// <returns>Vista da política de privacidade</returns>
    public IActionResult Privacy()
    {
        return View();
    }

    /// <summary>
    /// Página de erro genérica
    /// </summary>
    /// <returns>Vista de erro com ID da requisição</returns>
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { 
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier 
        });
    }
}