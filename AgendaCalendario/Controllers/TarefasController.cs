using AgendaCalendario.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AgendaCalendario.Models;

namespace AgendaCalendario.Controllers;

public class TarefasController : Controller
{
    private readonly AgendaDbContext _context;

    public TarefasController(AgendaDbContext context)
    {
        _context = context;
    }

    // GET: /Tarefas/PorData?data=2025-06-12
    [HttpGet]
    public async Task<IActionResult> PorData(string data)
    {
        var utilizadorId = HttpContext.Session.GetInt32("UtilizadorId");
        if (utilizadorId == null)
            return Unauthorized();

        if (!DateTime.TryParse(data, out var dataSelecionada))
            return BadRequest("Data inválida");

        var tarefas = await _context.Tarefas
            .Where(t => t.UtilizadorId == utilizadorId && t.Data.Date == dataSelecionada.Date)
            .Include(t => t.Categoria)
            .Select(t => new
            {
                id = t.Id,
                titulo = t.Titulo,
                descricao = t.Descricao,
                categoriaNome = t.Categoria != null ? t.Categoria.Nome : null,
                cor = t.Categoria != null ? t.Categoria.Cor : null
            })
            .ToListAsync();

        return Json(tarefas);
    }
    
    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] Tarefa tarefa)
    {
        var utilizadorId = HttpContext.Session.GetInt32("UtilizadorId");
        if (utilizadorId == null) return Unauthorized();

        tarefa.UtilizadorId = utilizadorId.Value;

        if (string.IsNullOrEmpty(tarefa.Titulo) || tarefa.Data == default)
            return BadRequest("Campos obrigatórios em falta.");

        _context.Tarefas.Add(tarefa);
        await _context.SaveChangesAsync();
        return Ok();
    }

}