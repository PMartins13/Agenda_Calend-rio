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
    
    [HttpGet]
    public async Task<IActionResult> Detalhes(int id)
    {
        var utilizadorId = HttpContext.Session.GetInt32("UtilizadorId");
        if (utilizadorId == null) return Unauthorized();

        var tarefa = await _context.Tarefas
            .FirstOrDefaultAsync(t => t.Id == id && t.UtilizadorId == utilizadorId);

        if (tarefa == null) return NotFound();

        return Json(new
        {
            id = tarefa.Id,
            titulo = tarefa.Titulo,
            descricao = tarefa.Descricao,
            data = tarefa.Data.ToString("yyyy-MM-dd"),
            categoriaId = tarefa.CategoriaId
        });
    }

    [HttpPost]
    public async Task<IActionResult> Editar([FromBody] Tarefa tarefa)
    {
        var utilizadorId = HttpContext.Session.GetInt32("UtilizadorId");
        if (utilizadorId == null) return Unauthorized();

        var original = await _context.Tarefas.FindAsync(tarefa.Id);
        if (original == null || original.UtilizadorId != utilizadorId) return NotFound();

        if (string.IsNullOrEmpty(tarefa.Titulo) || tarefa.Data == default)
            return BadRequest("Campos obrigatórios em falta.");

        original.Titulo = tarefa.Titulo;
        original.Descricao = tarefa.Descricao;
        original.Data = tarefa.Data;
        original.CategoriaId = tarefa.CategoriaId;

        await _context.SaveChangesAsync();
        return Ok();
    }

    [HttpPost]
    public async Task<IActionResult> Apagar([FromBody] int id)
    {
        var utilizadorId = HttpContext.Session.GetInt32("UtilizadorId");
        if (utilizadorId == null) return Unauthorized();

        var tarefa = await _context.Tarefas.FindAsync(id);
        if (tarefa == null || tarefa.UtilizadorId != utilizadorId) return NotFound();

        _context.Tarefas.Remove(tarefa);
        await _context.SaveChangesAsync();
        return Ok();
    }


}