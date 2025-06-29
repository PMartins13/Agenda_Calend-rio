using AgendaCalendario.Data;
using AgendaCalendario.Models.API_Dtos;
using AgendaCalendario.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AgendaCalendario.Controllers.API
{
    /// <summary>
    /// Controlador API para gestão de Tarefas
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class TarefasApiController : ControllerBase
    {
        private readonly AgendaDbContext _context;

        /// <summary>
        /// Construtor que injeta o contexto da base de dados
        /// </summary>
        /// <param name="context">Contexto da base de dados</param>
        public TarefasApiController(AgendaDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Obtém todas as tarefas incluindo suas categorias associadas
        /// </summary>
        /// <returns>Lista de todas as tarefas com suas categorias</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Tarefa>>> GetTarefas()
        {
            return await _context.Tarefas
                .Include(t => t.Categorias)
                .ToListAsync();
        }

        /// <summary>
        /// Obtém uma tarefa específica pelo seu ID
        /// </summary>
        /// <param name="id">ID da tarefa</param>
        /// <returns>A tarefa encontrada ou NotFound se não existir</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<Tarefa>> GetTarefa(int id)
        {
            var tarefa = await _context.Tarefas.FindAsync(id);
            if (tarefa == null)
                return NotFound();
            return tarefa;
        }

        /// <summary>
        /// Cria uma nova tarefa
        /// </summary>
        /// <param name="dto">DTO com os dados da nova tarefa</param>
        /// <returns>A tarefa criada e o URL para aceder à mesma</returns>
        [HttpPost]
        public async Task<ActionResult<Tarefa>> PostTarefa([FromBody] TarefaCreateDto dto)
        {
            // Obtém as categorias válidas para o utilizador
            var categorias = await _context.Categorias
                .Where(c => dto.CategoriasIds.Contains(c.Id) && c.UtilizadorId == dto.UtilizadorId)
                .ToListAsync();
            
            // Cria nova instância de Tarefa com os dados do DTO
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

            // Adiciona à base de dados e guarda alterações
            _context.Tarefas.Add(tarefa);
            await _context.SaveChangesAsync();

            // Retorna resposta 201 (Created) com URL para a nova tarefa
            return CreatedAtAction(nameof(GetTarefa), new { id = tarefa.Id }, tarefa);
        }

        /// <summary>
        /// Atualiza uma tarefa existente
        /// </summary>
        /// <param name="id">ID da tarefa a atualizar</param>
        /// <param name="dto">DTO com os novos dados da tarefa</param>
        /// <returns>NoContent se sucesso, BadRequest se IDs não coincidirem, NotFound se não existir</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTarefa(int id, [FromBody] TarefaUpdateDto dto)
        {
            // Verifica se o ID do URL corresponde ao ID do DTO
            if (id != dto.Id) return BadRequest();

            // Procura a tarefa na base de dados incluindo suas categorias
            var tarefa = await _context.Tarefas
                .Include(t => t.Categorias)
                .FirstOrDefaultAsync(t => t.Id == id);
            if (tarefa == null) return NotFound();

            // Atualiza os dados básicos da tarefa
            tarefa.Titulo = dto.Titulo;
            tarefa.Descricao = dto.Descricao;
            tarefa.Data = dto.Data;
            tarefa.UtilizadorId = dto.UtilizadorId;

            // Atualiza as categorias associadas
            tarefa.Categorias.Clear();
            var novasCategorias = await _context.Categorias
                .Where(c => dto.CategoriasIds.Contains(c.Id) && c.UtilizadorId == dto.UtilizadorId)
                .ToListAsync();
            foreach (var cat in novasCategorias)
                tarefa.Categorias.Add(cat);

            // Guarda alterações
            await _context.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>
        /// Elimina uma tarefa específica
        /// </summary>
        /// <param name="id">ID da tarefa a eliminar</param>
        /// <returns>NoContent se sucesso, NotFound se não existir</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTarefa(int id)
        {
            var tarefa = await _context.Tarefas.FindAsync(id);
            if (tarefa == null)
                return NotFound();

            // Remove a tarefa e guarda alterações
            _context.Tarefas.Remove(tarefa);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}