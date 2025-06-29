using AgendaCalendario.Data;
using AgendaCalendario.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AgendaCalendario.Controllers;

/// <summary>
/// Controlador para gestão de Categorias através de interface web
/// </summary>
public class CategoriasController : Controller
{
    private readonly AgendaDbContext _context;

    /// <summary>
    /// Construtor que injeta o contexto da base de dados
    /// </summary>
    /// <param name="context">Contexto da base de dados</param>
    public CategoriasController(AgendaDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Cria uma nova categoria para o utilizador autenticado
    /// </summary>
    /// <param name="categoria">Dados da categoria a criar</param>
    /// <returns>Ok se criada, BadRequest se dados inválidos, Unauthorized se não autenticado</returns>
    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] Categoria categoria)
    {
        // Verifica autenticação
        var utilizadorId = HttpContext.Session.GetInt32("UtilizadorId");
        if (utilizadorId == null) return Unauthorized();

        // Validação básica
        if (string.IsNullOrEmpty(categoria.Nome)) 
            return BadRequest("Nome obrigatório.");

        // Associa ao utilizador atual
        categoria.UtilizadorId = utilizadorId.Value;
        _context.Categorias.Add(categoria);
        await _context.SaveChangesAsync();

        return Ok();
    }
    
    /// <summary>
    /// Obtém todas as categorias do utilizador autenticado
    /// </summary>
    /// <returns>JSON com lista de categorias ou Unauthorized se não autenticado</returns>
    [HttpGet]
    public async Task<IActionResult> Minhas()
    {
        var utilizadorId = HttpContext.Session.GetInt32("UtilizadorId");
        if (utilizadorId == null) return Unauthorized();

        // Obtém categorias do utilizador com projeção personalizada
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
    /// Edita uma categoria existente do utilizador autenticado
    /// </summary>
    /// <param name="categoriaDto">Dados atualizados da categoria</param>
    /// <returns>Ok se atualizada, NotFound se não encontrada, Unauthorized se não autenticado</returns>
    [HttpPost]
    public async Task<IActionResult> Editar([FromBody] CategoriaDto categoriaDto)
    {
        var utilizadorId = HttpContext.Session.GetInt32("UtilizadorId");
        if (utilizadorId == null) return Unauthorized();

        // Procura categoria do utilizador
        var categoria = await _context.Categorias
            .FirstOrDefaultAsync(c => c.Id == categoriaDto.Id && 
                                    c.UtilizadorId == utilizadorId);

        if (categoria == null) return NotFound();

        // Atualiza dados
        categoria.Nome = categoriaDto.Nome;
        categoria.Cor = categoriaDto.Cor;

        await _context.SaveChangesAsync();
        return Ok();
    }
    
    /// <summary>
    /// Elimina uma categoria e remove suas associações com tarefas
    /// </summary>
    /// <param name="id">ID da categoria a eliminar</param>
    /// <returns>Ok se eliminada, NotFound se não encontrada, Unauthorized se não autenticado</returns>
    [HttpPost]
    public async Task<IActionResult> Eliminar([FromBody] int id)
    {
        var utilizadorId = HttpContext.Session.GetInt32("UtilizadorId");
        if (utilizadorId == null) return Unauthorized();

        // Procura categoria do utilizador
        var categoria = await _context.Categorias
            .FirstOrDefaultAsync(c => c.Id == id && 
                                    c.UtilizadorId == utilizadorId);

        if (categoria == null) return NotFound();

        // Remove associações com tarefas
        var tarefas = await _context.Tarefas
            .Include(t => t.Categorias)
            .Where(t => t.Categorias.Any(c => c.Id == id))
            .ToListAsync();

        foreach (var tarefa in tarefas)
        {
            var categoriaParaRemover = tarefa.Categorias
                .FirstOrDefault(c => c.Id == id);
            if (categoriaParaRemover != null)
                tarefa.Categorias.Remove(categoriaParaRemover);
        }

        // Remove a categoria
        _context.Categorias.Remove(categoria);
        await _context.SaveChangesAsync();

        return Ok();
    }
}