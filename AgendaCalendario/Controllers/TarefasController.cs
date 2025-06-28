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
            .Select(t => new {
                id = t.Id,
                titulo = t.Titulo,
                descricao = t.Descricao,
                data = t.Data,
                categoriaId = t.CategoriaId,
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

        // Adiciona a tarefa original
        _context.Tarefas.Add(tarefa);

        // Se for recorrente, gera as próximas instâncias
        if (tarefa.Recorrencia != TipoRecorrencia.Nenhuma)
        {
            var dataFim = tarefa.DataFimRecorrencia;
            DateTime limite = tarefa.Data.AddMonths(6); // Limite de 6 meses
            if (dataFim.HasValue && dataFim.Value < limite)
                limite = dataFim.Value;

            DateTime dataAtual = tarefa.Data;
            while (true)
            {
                switch (tarefa.Recorrencia)
                {
                    case TipoRecorrencia.Diaria:
                        dataAtual = dataAtual.AddDays(1);
                        break;
                    case TipoRecorrencia.Semanal:
                        dataAtual = dataAtual.AddDays(7);
                        break;
                    case TipoRecorrencia.Mensal:
                        dataAtual = dataAtual.AddMonths(1);
                        break;
                }

                // Condição de paragem: nunca passar do limite de 6 meses (ou data de fim, se for antes)
                if (dataAtual > limite)
                    break;

                var novaTarefa = new Tarefa
                {
                    Titulo = tarefa.Titulo,
                    Descricao = tarefa.Descricao,
                    Data = dataAtual,
                    UtilizadorId = tarefa.UtilizadorId,
                    CategoriaId = tarefa.CategoriaId,
                    Recorrencia = tarefa.Recorrencia,
                    DataFimRecorrencia = tarefa.DataFimRecorrencia
                };
                _context.Tarefas.Add(novaTarefa);
            }
        }

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

    [HttpPost]
    public async Task<IActionResult> ApagarTodasComTitulo([FromBody] string titulo)
    {
        var utilizadorId = HttpContext.Session.GetInt32("UtilizadorId");
        if (utilizadorId == null) return Unauthorized();

        var tarefas = await _context.Tarefas
            .Where(t => t.UtilizadorId == utilizadorId && t.Titulo == titulo)
            .ToListAsync();

        if (!tarefas.Any()) return NotFound();

        _context.Tarefas.RemoveRange(tarefas);
        await _context.SaveChangesAsync();
        return Ok();
    }


}