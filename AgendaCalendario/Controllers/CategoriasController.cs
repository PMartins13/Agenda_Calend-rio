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
                nome = c.Nome,
                cor = c.Cor
            })
            .ToListAsync();

        return Json(categorias);
    }
    
    /// <summary>
    /// Edita uma categoria existente do utilizador autenticado.
    /// </summary>
    /// <param name="categoriaDto">Objeto com os dados da categoria a editar (Id, Nome, Cor).</param>
    /// <returns>Resposta HTTP 200 OK se for bem-sucedido, 404 se não for encontrada ou 401 se não autenticado.</returns>
    [HttpPost]
    public async Task<IActionResult> Editar([FromBody] CategoriaDto categoriaDto)
    {
        var utilizadorId = HttpContext.Session.GetInt32("UtilizadorId");
        if (utilizadorId == null) return Unauthorized();

        var categoria = await _context.Categorias
            .FirstOrDefaultAsync(c => c.Id == categoriaDto.Id && c.UtilizadorId == utilizadorId);

        if (categoria == null) return NotFound();

        categoria.Nome = categoriaDto.Nome;
        categoria.Cor = categoriaDto.Cor;

        await _context.SaveChangesAsync();
        return Ok();
    }
    
    /// <summary>
    /// Elimina uma categoria e remove a associação das tarefas relacionadas (CategoriaId = null).
    /// </summary>
    /// <param name="id">ID da categoria a eliminar.</param>
    /// <returns>HTTP 200 se for eliminada, 404 se não for encontrada, 401 se não autenticado.</returns>
    [HttpPost]
    public async Task<IActionResult> Eliminar([FromBody] int id)
    {
        var utilizadorId = HttpContext.Session.GetInt32("UtilizadorId");
        if (utilizadorId == null) return Unauthorized();

        var categoria = await _context.Categorias
            .FirstOrDefaultAsync(c => c.Id == id && c.UtilizadorId == utilizadorId);

        if (categoria == null) return NotFound();

        // Dissociar tarefas antes de apagar a categoria
        var tarefas = await _context.Tarefas
            .Include(t => t.Categorias)
            .Where(t => t.Categorias.Any(c => c.Id == id))
            .ToListAsync();

        foreach (var tarefa in tarefas)
        {
            var categoriaParaRemover = tarefa.Categorias.FirstOrDefault(c => c.Id == id);
            if (categoriaParaRemover != null)
                tarefa.Categorias.Remove(categoriaParaRemover);
        }


        _context.Categorias.Remove(categoria);
        await _context.SaveChangesAsync();

        return Ok();
    }
}