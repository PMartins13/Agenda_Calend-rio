using AgendaCalendario.Data;
using AgendaCalendario.Models.API_Dtos;
using AgendaCalendario.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AgendaCalendario.Controllers.API
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriasApiController : ControllerBase
    {
        private readonly AgendaDbContext _context;

        public CategoriasApiController(AgendaDbContext context)
        {
            _context = context;
        }

        // GET: api/CategoriasApi
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Categoria>>> GetCategorias()
        {
            return await _context.Categorias.ToListAsync();
        }

        // GET: api/CategoriasApi/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Categoria>> GetCategoria(int id)
        {
            var categoria = await _context.Categorias.FindAsync(id);
            if (categoria == null) return NotFound();
            return categoria;
        }

        // POST: api/CategoriasApi
        [HttpPost]
        public async Task<ActionResult<Categoria>> PostCategoria(CategoriaCreateDto dto)
        {
            try
            {
                var novaCategoria = new Categoria
                {
                    Nome = dto.Nome,
                    Cor = dto.Cor,
                    UtilizadorId = dto.UtilizadorId // ← associa ao utilizador fornecido
                };

                _context.Categorias.Add(novaCategoria);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetCategoria), new { id = novaCategoria.Id }, novaCategoria);
            }
            catch (Exception ex)
            {
                return BadRequest("Erro ao criar categoria: " + ex.Message);
            }
        }


        // PUT: api/CategoriasApi/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCategoria(int id, CategoriaUpdateDto dto)
        {
            if (id != dto.Id) return BadRequest();

            var categoria = await _context.Categorias.FindAsync(id);
            if (categoria == null) return NotFound();

            categoria.Nome = dto.Nome;
            categoria.Cor = dto.Cor;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/CategoriasApi/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategoria(int id)
        {
            var categoria = await _context.Categorias.FindAsync(id);
            if (categoria == null)
            {
                return NotFound();
            }

            // Desassociar todas as tarefas que usam esta categoria
            var tarefas = await _context.Tarefas
                .Include(t => t.Categorias)
                .Where(t => t.Categorias.Any(c => c.Id == id))
                .ToListAsync();

            foreach (var tarefa in tarefas)
            {
                var categoriaParaRemover = tarefa.Categorias.FirstOrDefault(c => c.Id == id);
                if (categoriaParaRemover != null)
                {
                    tarefa.Categorias.Remove(categoriaParaRemover);
                }
            }


            _context.Categorias.Remove(categoria);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
