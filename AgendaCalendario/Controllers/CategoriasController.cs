using AgendaCalendario.Data;
using AgendaCalendario.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AgendaCalendario.Controllers;

public class CategoriasController : Controller
{
    private readonly AgendaDbContext _context;

    public CategoriasController(AgendaDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] Categoria categoria)
    {
        var utilizadorId = HttpContext.Session.GetInt32("UtilizadorId");
        if (utilizadorId == null) return Unauthorized();

        if (string.IsNullOrEmpty(categoria.Nome)) return BadRequest("Nome obrigatório.");

        categoria.UtilizadorId = utilizadorId.Value;
        _context.Categorias.Add(categoria);
        await _context.SaveChangesAsync();

        return Ok();
    }
    
    [HttpGet]
    public async Task<IActionResult> Minhas()
    {
        var utilizadorId = HttpContext.Session.GetInt32("UtilizadorId");
        if (utilizadorId == null) return Unauthorized();

        var categorias = await _context.Categorias
            .Where(c => c.UtilizadorId == utilizadorId)
            .Select(c => new {
                id = c.Id,
                nome = c.Nome
            })
            .ToListAsync();

        return Json(categorias);
    }
}