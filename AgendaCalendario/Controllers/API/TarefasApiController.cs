using AgendaCalendario.Data;
using AgendaCalendario.Models.API_Dtos;
using AgendaCalendario.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AgendaCalendario.Controllers.API
{
    [ApiController]
    [Route("api/[controller]")]
    public class TarefasApiController : ControllerBase
    {
        private readonly AgendaDbContext _context;

        public TarefasApiController(AgendaDbContext context)
        {
            _context = context;
        }

        // GET: api/tarefas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Tarefa>>> GetTarefas()
        {
            return await _context.Tarefas
                .Include(t => t.Categorias)
                .ToListAsync();
        }
        
        
        // GET: api/tarefas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Tarefa>> GetTarefa(int id)
        {
            var tarefa = await _context.Tarefas.FindAsync(id);

            if (tarefa == null)
                return NotFound();

            return tarefa;
        }

        // POST: api/tarefas
        [HttpPost]
        public async Task<ActionResult<Tarefa>> PostTarefa([FromBody] TarefaCreateDto dto)
        {
            var categorias = await _context.Categorias
                .Where(c => dto.CategoriasIds.Contains(c.Id) && c.UtilizadorId == dto.UtilizadorId)
                .ToListAsync();
            
            var tarefa = new Tarefa
            {
                Titulo = dto.Titulo,
                Descricao = dto.Descricao,
                Data = dto.Data,
                UtilizadorId = dto.UtilizadorId,
                Recorrencia = dto.Recorrencia,
                DataFimRecorrencia = dto.DataFimRecorrencia,
                Categorias = categorias
            };

            _context.Tarefas.Add(tarefa);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTarefa", new { id = tarefa.Id }, tarefa);
        }

        // PUT: api/tarefas/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTarefa(int id, [FromBody] TarefaUpdateDto dto)
        {
            if (id != dto.Id) return BadRequest();

            var tarefa = await _context.Tarefas
                .Include(t => t.Categorias)
                .FirstOrDefaultAsync(t => t.Id == id);
            if (tarefa == null) return NotFound();

            tarefa.Titulo = dto.Titulo;
            tarefa.Descricao = dto.Descricao;
            tarefa.Data = dto.Data;
            tarefa.UtilizadorId = dto.UtilizadorId;

            tarefa.Categorias.Clear();
            var novasCategorias = await _context.Categorias
                .Where(c => dto.CategoriasIds.Contains(c.Id) && c.UtilizadorId == dto.UtilizadorId)
                .ToListAsync();
            foreach (var cat in novasCategorias)
                tarefa.Categorias.Add(cat);

            await _context.SaveChangesAsync();
            return NoContent();
        }


        // DELETE: api/tarefas/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTarefa(int id)
        {
            var tarefa = await _context.Tarefas.FindAsync(id);
            if (tarefa == null)
                return NotFound();

            _context.Tarefas.Remove(tarefa);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
