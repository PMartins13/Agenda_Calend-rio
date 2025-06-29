using AgendaCalendario.Data;
using AgendaCalendario.Models.API_Dtos;
using AgendaCalendario.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AgendaCalendario.Controllers.API
{
    /// <summary>
    /// Controlador API para gestão de Categorias
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriasApiController : ControllerBase
    {
        private readonly AgendaDbContext _context;

        /// <summary>
        /// Construtor que injeta o contexto da base de dados
        /// </summary>
        /// <param name="context">Contexto da base de dados</param>
        public CategoriasApiController(AgendaDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Obtém todas as categorias
        /// </summary>
        /// <returns>Lista de todas as categorias</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Categoria>>> GetCategorias()
        {
            return await _context.Categorias.ToListAsync();
        }

        /// <summary>
        /// Obtém uma categoria específica pelo seu ID
        /// </summary>
        /// <param name="id">ID da categoria</param>
        /// <returns>A categoria encontrada ou NotFound se não existir</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<Categoria>> GetCategoria(int id)
        {
            var categoria = await _context.Categorias.FindAsync(id);
            if (categoria == null) return NotFound();
            return categoria;
        }

        /// <summary>
        /// Cria uma nova categoria
        /// </summary>
        /// <param name="dto">DTO com os dados da nova categoria</param>
        /// <returns>A categoria criada e o URL para aceder à mesma</returns>
        [HttpPost]
        public async Task<ActionResult<Categoria>> PostCategoria(CategoriaCreateDto dto)
        {
            try
            {
                // Cria nova instância de Categoria com os dados do DTO
                var novaCategoria = new Categoria
                {
                    Nome = dto.Nome,
                    Cor = dto.Cor,
                    UtilizadorId = dto.UtilizadorId // Associa ao utilizador fornecido
                };

                // Adiciona à base de dados e guarda alterações
                _context.Categorias.Add(novaCategoria);
                await _context.SaveChangesAsync();

                // Retorna resposta 201 (Created) com URL para a nova categoria
                return CreatedAtAction(
                    nameof(GetCategoria), 
                    new { id = novaCategoria.Id }, 
                    novaCategoria
                );
            }
            catch (Exception ex)
            {
                return BadRequest("Erro ao criar categoria: " + ex.Message);
            }
        }

        /// <summary>
        /// Atualiza uma categoria existente
        /// </summary>
        /// <param name="id">ID da categoria a atualizar</param>
        /// <param name="dto">DTO com os novos dados da categoria</param>
        /// <returns>NoContent se sucesso, BadRequest se IDs não coincidirem, NotFound se não existir</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCategoria(int id, CategoriaUpdateDto dto)
        {
            // Verifica se o ID do URL corresponde ao ID do DTO
            if (id != dto.Id) return BadRequest();

            // Procura a categoria na base de dados
            var categoria = await _context.Categorias.FindAsync(id);
            if (categoria == null) return NotFound();

            // Atualiza os dados da categoria
            categoria.Nome = dto.Nome;
            categoria.Cor = dto.Cor;

            // Guarda alterações
            await _context.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>
        /// Elimina uma categoria específica
        /// </summary>
        /// <param name="id">ID da categoria a eliminar</param>
        /// <returns>NoContent se sucesso, NotFound se não existir</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategoria(int id)
        {
            // Procura a categoria na base de dados
            var categoria = await _context.Categorias.FindAsync(id);
            if (categoria == null)
            {
                return NotFound();
            }

            // Obtém todas as tarefas que usam esta categoria
            var tarefas = await _context.Tarefas
                .Include(t => t.Categorias)
                .Where(t => t.Categorias.Any(c => c.Id == id))
                .ToListAsync();

            // Remove a categoria de todas as tarefas associadas
            foreach (var tarefa in tarefas)
            {
                var categoriaParaRemover = tarefa.Categorias.FirstOrDefault(c => c.Id == id);
                if (categoriaParaRemover != null)
                {
                    tarefa.Categorias.Remove(categoriaParaRemover);
                }
            }

            // Remove a categoria e guarda alterações
            _context.Categorias.Remove(categoria);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}