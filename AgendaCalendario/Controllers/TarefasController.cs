using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AgendaCalendario.Data;
using AgendaCalendario.Models;

namespace AgendaCalendario.Controllers;

/// <summary>
/// Controlador responsável pela gestão de tarefas no calendário
/// </summary>
public class TarefasController : Controller
{
    private readonly AgendaDbContext _context;

    public TarefasController(AgendaDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Obtém tarefas de uma data específica
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> PorData(string data)
    {
        var utilizadorId = HttpContext.Session.GetInt32("UtilizadorId");
        if (utilizadorId == null) return Unauthorized();

        if (!DateTime.TryParse(data, out var dataSelecionada))
            return BadRequest("Data inválida");

        var tarefas = await _context.Tarefas
            .Where(t => t.UtilizadorId == utilizadorId && 
                       t.Data.Date == dataSelecionada.Date)
            .Include(t => t.Categorias)
            .Select(t => new TarefaDtoViewModel
            {
                Id = t.Id,
                Titulo = t.Titulo,
                Descricao = t.Descricao,
                Data = t.Data,
                Categorias = t.Categorias.Select(c => new CategoriaResumoDto
                {
                    Id = c.Id,
                    Nome = c.Nome,
                    Cor = c.Cor
                }).ToList()
            })
            .ToListAsync();

        return Json(tarefas);
    }
    
    /// <summary>
    /// Obtém tarefas dentro de um intervalo de datas
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> PorIntervalo(string inicio, string fim)
    {
        var utilizadorId = HttpContext.Session.GetInt32("UtilizadorId");
        if (utilizadorId == null) return Unauthorized();

        if (!DateTime.TryParse(inicio, out var dataInicio) || 
            !DateTime.TryParse(fim, out var dataFim))
            return BadRequest("Datas inválidas");

        var tarefas = await _context.Tarefas
            .Where(t => t.UtilizadorId == utilizadorId && 
                       t.Data.Date >= dataInicio.Date && 
                       t.Data.Date <= dataFim.Date)
            .Include(t => t.Categorias)
            .ToListAsync();

        // Formata eventos para o calendário
        var eventos = tarefas.Select(t => new {
            id = t.Id,
            title = t.Titulo,
            start = t.Data.ToString("yyyy-MM-dd"),
            backgroundColor = t.Categorias.FirstOrDefault()?.Cor ?? "#999",
            borderColor = t.Categorias.FirstOrDefault()?.Cor ?? "#999"
        }).ToList();

        // Adiciona destaques visuais nos dias com tarefas
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
    
    /// <summary>
    /// Cria uma nova tarefa (com suporte a recorrência)
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] TarefaInputModel tarefa)
    {
        var utilizadorId = HttpContext.Session.GetInt32("UtilizadorId");
        if (utilizadorId == null) return Unauthorized();

        tarefa.UtilizadorId = utilizadorId.Value;

        if (string.IsNullOrEmpty(tarefa.Titulo) || tarefa.Data == default)
            return BadRequest("Campos obrigatórios em falta.");

        // Obtém categorias selecionadas
        var categorias = new List<Categoria>();
        if (tarefa.CategoriasIds?.Any() == true)
        {
            categorias = await _context.Categorias
                .Where(c => tarefa.CategoriasIds.Contains(c.Id) && 
                           c.UtilizadorId == utilizadorId)
                .ToListAsync();
        }

        // Cria tarefa principal
        var tarefaPrincipal = new Tarefa
        {
            Titulo = tarefa.Titulo,
            Descricao = tarefa.Descricao,
            Data = tarefa.Data,
            UtilizadorId = tarefa.UtilizadorId,
            Recorrencia = tarefa.Recorrencia,
            DataFimRecorrencia = tarefa.DataFimRecorrencia,
            Categorias = categorias
        };

        _context.Tarefas.Add(tarefaPrincipal);

        // Cria tarefas recorrentes se necessário
        if (tarefa.Recorrencia != TipoRecorrencia.Nenhuma)
        {
            DateTime limite = tarefa.Data.AddMonths(6);
            if (tarefa.DataFimRecorrencia.HasValue && 
                tarefa.DataFimRecorrencia.Value < limite)
                limite = tarefa.DataFimRecorrencia.Value;

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

                if (dataAtual > limite) break;

                var novaTarefa = new Tarefa
                {
                    Titulo = tarefa.Titulo,
                    Descricao = tarefa.Descricao,
                    Data = dataAtual,
                    UtilizadorId = utilizadorId.Value,
                    Recorrencia = tarefa.Recorrencia,
                    DataFimRecorrencia = tarefa.DataFimRecorrencia,
                    Categorias = await _context.Categorias
                        .Where(c => tarefa.CategoriasIds.Contains(c.Id) && 
                                  c.UtilizadorId == utilizadorId)
                        .ToListAsync()
                };

                _context.Tarefas.Add(novaTarefa);
            }
        }

        await _context.SaveChangesAsync();
        return Ok();
    }

    /// <summary>
    /// Obtém detalhes de uma tarefa específica
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Detalhes(int id)
    {
        var utilizadorId = HttpContext.Session.GetInt32("UtilizadorId");
        if (utilizadorId == null) return Unauthorized();

        var tarefa = await _context.Tarefas
            .Include(t => t.Categorias)
            .FirstOrDefaultAsync(t => t.Id == id && 
                                    t.UtilizadorId == utilizadorId);

        if (tarefa == null) return NotFound();

        return Json(new
        {
            id = tarefa.Id,
            titulo = tarefa.Titulo,
            descricao = tarefa.Descricao,
            data = tarefa.Data.ToString("yyyy-MM-dd"),
            recorrencia = tarefa.Recorrencia,
            dataFimRecorrencia = tarefa.DataFimRecorrencia?
                .ToString("yyyy-MM-dd"),
            categoriasIds = tarefa.Categorias.Select(c => c.Id).ToList()
        });
    }

    /// <summary>
    /// Atualiza uma tarefa existente
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Editar([FromBody] TarefaInputModel tarefa)
    {
        var utilizadorId = HttpContext.Session.GetInt32("UtilizadorId");
        if (utilizadorId == null) return Unauthorized();

        var original = await _context.Tarefas
            .Include(t => t.Categorias)
            .FirstOrDefaultAsync(t => t.Id == tarefa.Id && 
                                    t.UtilizadorId == utilizadorId);

        if (original == null) return NotFound();

        if (string.IsNullOrEmpty(tarefa.Titulo) || tarefa.Data == default)
            return BadRequest("Campos obrigatórios em falta.");

        // Atualiza dados básicos
        original.Titulo = tarefa.Titulo;
        original.Descricao = tarefa.Descricao;
        original.Data = tarefa.Data;
        original.Recorrencia = tarefa.Recorrencia;
        original.DataFimRecorrencia = tarefa.DataFimRecorrencia;

        // Atualiza categorias
        original.Categorias.Clear();
        if (tarefa.CategoriasIds?.Any() == true)
        {
            var categorias = await _context.Categorias
                .Where(c => tarefa.CategoriasIds.Contains(c.Id) && 
                           c.UtilizadorId == utilizadorId)
                .ToListAsync();
            foreach (var cat in categorias)
                original.Categorias.Add(cat);
        }

        await _context.SaveChangesAsync();
        return Ok();
    }

    /// <summary>
    /// Atualiza todas as tarefas com um determinado título
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> EditarTodasComTitulo(
        [FromBody] EditarTodasViewModel tarefa)
    {
        var utilizadorId = HttpContext.Session.GetInt32("UtilizadorId");
        if (utilizadorId == null) return Unauthorized();

        if (string.IsNullOrEmpty(tarefa.TituloOriginal) || 
            string.IsNullOrEmpty(tarefa.Titulo))
            return BadRequest("Título obrigatório.");

        var tarefas = await _context.Tarefas
            .Where(t => t.UtilizadorId == utilizadorId && 
                       t.Titulo == tarefa.TituloOriginal)
            .Include(t => t.Categorias)
            .ToListAsync();

        if (!tarefas.Any())
            return NotFound("Nenhuma tarefa encontrada com esse título.");

        var categorias = new List<Categoria>();
        if (tarefa.CategoriasIds?.Any() == true)
        {
            categorias = await _context.Categorias
                .Where(c => tarefa.CategoriasIds.Contains(c.Id) && 
                           c.UtilizadorId == utilizadorId)
                .ToListAsync();
        }

        // Atualiza todas as tarefas encontradas
        foreach (var t in tarefas)
        {
            t.Titulo = tarefa.Titulo;
            t.Descricao = tarefa.Descricao;
            t.Recorrencia = tarefa.Recorrencia;
            t.DataFimRecorrencia = tarefa.DataFimRecorrencia;

            t.Categorias.Clear();
            foreach (var cat in categorias)
            {
                t.Categorias.Add(cat);
            }
        }

        await _context.SaveChangesAsync();
        return Ok();
    }

    /// <summary>
    /// Remove uma tarefa específica
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Apagar([FromBody] int id)
    {
        var utilizadorId = HttpContext.Session.GetInt32("UtilizadorId");
        if (utilizadorId == null) return Unauthorized();

        var tarefa = await _context.Tarefas.FindAsync(id);
        if (tarefa == null || tarefa.UtilizadorId != utilizadorId) 
            return NotFound();

        _context.Tarefas.Remove(tarefa);
        await _context.SaveChangesAsync();
        return Ok();
    }

    /// <summary>
    /// Remove todas as tarefas com um determinado título
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> ApagarTodasComTitulo([FromBody] string titulo)
    {
        var utilizadorId = HttpContext.Session.GetInt32("UtilizadorId");
        if (utilizadorId == null) return Unauthorized();

        var tarefas = await _context.Tarefas
            .Where(t => t.UtilizadorId == utilizadorId && 
                       t.Titulo == titulo)
            .ToListAsync();

        if (!tarefas.Any()) return NotFound();

        _context.Tarefas.RemoveRange(tarefas);
        await _context.SaveChangesAsync();
        return Ok();
    }
}

/// <summary>
/// ViewModel para edição em massa de tarefas
/// </summary>
public class EditarTodasViewModel
{
    public string TituloOriginal { get; set; }
    public string Titulo { get; set; }
    public string Descricao { get; set; }
    public DateTime Data { get; set; }
    public TipoRecorrencia Recorrencia { get; set; }
    public DateTime? DataFimRecorrencia { get; set; }
    public List<int> CategoriasIds { get; set; } = new();
}