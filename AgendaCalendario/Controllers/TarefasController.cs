using AgendaCalendario.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AgendaCalendario.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http; 

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
    
    [HttpGet]
    public async Task<IActionResult> PorIntervalo(string inicio, string fim)
    {
        var utilizadorId = HttpContext.Session.GetInt32("UtilizadorId");
        if (utilizadorId == null)
            return Unauthorized();

        if (!DateTime.TryParse(inicio, out var dataInicio) || !DateTime.TryParse(fim, out var dataFim))
            return BadRequest("Datas inválidas");

        var tarefas = await _context.Tarefas
            .Where(t => t.UtilizadorId == utilizadorId && t.Data.Date >= dataInicio.Date && t.Data.Date <= dataFim.Date)
            .Include(t => t.Categoria)
            .ToListAsync();

        var eventos = tarefas.Select(t => new {
            id = t.Id,
            title = t.Titulo,
            start = t.Data.ToString("yyyy-MM-dd"),
            backgroundColor = t.Categoria?.Cor ?? "#999",
            borderColor = t.Categoria?.Cor ?? "#999"
        }).ToList();

        // Background highlights nos dias com tarefas
        var destaques = tarefas
            .Select(t => t.Data.Date)
            .Distinct()
            .Select(d => new {
                start = d.ToString("yyyy-MM-dd"),
                display = "background",
                backgroundColor = "#fdf3d6"
            });

        var todos = new List<object>();
        todos.AddRange(eventos);
        todos.AddRange(destaques);
        return Json(todos);
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
            DateTime limite = tarefa.Data.AddMonths(6);
            if (tarefa.DataFimRecorrencia.HasValue && tarefa.DataFimRecorrencia.Value < limite)
                limite = tarefa.DataFimRecorrencia.Value;

            DateTime dataAtual = tarefa.Data;
            while (true)
            {
                switch (tarefa.Recorrencia)
                {
                    case TipoRecorrencia.Diaria: dataAtual = dataAtual.AddDays(1); break;
                    case TipoRecorrencia.Semanal: dataAtual = dataAtual.AddDays(7); break;
                    case TipoRecorrencia.Mensal: dataAtual = dataAtual.AddMonths(1); break;
                }

                if (dataAtual > limite) break;

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
            categoriaId = tarefa.CategoriaId,
            recorrencia = tarefa.Recorrencia,
            dataFimRecorrencia = tarefa.DataFimRecorrencia?.ToString("yyyy-MM-dd")
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
        original.Recorrencia = tarefa.Recorrencia;
        original.DataFimRecorrencia = tarefa.DataFimRecorrencia;

        // Adiciona novas instâncias se houver recorrência
        if (tarefa.Recorrencia != TipoRecorrencia.Nenhuma)
        {
            DateTime limite = tarefa.Data.AddMonths(6);
            if (tarefa.DataFimRecorrencia.HasValue && tarefa.DataFimRecorrencia.Value < limite)
                limite = tarefa.DataFimRecorrencia.Value;

            DateTime dataAtual = tarefa.Data;
            while (true)
            {
                switch (tarefa.Recorrencia)
                {
                    case TipoRecorrencia.Diaria: dataAtual = dataAtual.AddDays(1); break;
                    case TipoRecorrencia.Semanal: dataAtual = dataAtual.AddDays(7); break;
                    case TipoRecorrencia.Mensal: dataAtual = dataAtual.AddMonths(1); break;
                }

                if (dataAtual > limite) break;

                var novaTarefa = new Tarefa
                {
                    Titulo = tarefa.Titulo,
                    Descricao = tarefa.Descricao,
                    Data = dataAtual,
                    UtilizadorId = utilizadorId.Value, // CORRIGIDO: vem da sessão
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

    [HttpPost]
    public async Task<IActionResult> EditarTodasComTitulo([FromBody] EditarTodasViewModel tarefa)
    {
        var utilizadorId = HttpContext.Session.GetInt32("UtilizadorId");
        if (utilizadorId == null) return Unauthorized();

        if (string.IsNullOrEmpty(tarefa.TituloOriginal) || string.IsNullOrEmpty(tarefa.Titulo))
            return BadRequest("Título obrigatório.");

        var tarefas = await _context.Tarefas
            .Where(t => t.UtilizadorId == utilizadorId && t.Titulo == tarefa.TituloOriginal)
            .ToListAsync();

        foreach (var t in tarefas)
        {
            t.Titulo = tarefa.Titulo;
            t.Descricao = tarefa.Descricao;
            t.CategoriaId = tarefa.CategoriaId;
            t.Recorrencia = tarefa.Recorrencia;
            t.DataFimRecorrencia = tarefa.DataFimRecorrencia;
            // t.Data = tarefa.Data; // Só se quiseres alterar a data de todas!
        }

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

public class EditarTodasViewModel
{
    public string TituloOriginal { get; set; }
    public string Titulo { get; set; }
    public string Descricao { get; set; }
    public DateTime Data { get; set; }
    public int? CategoriaId { get; set; }
    public TipoRecorrencia Recorrencia { get; set; }
    public DateTime? DataFimRecorrencia { get; set; }
}