﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AgendaCalendario.Data;
using AgendaCalendario.Models;
using AgendaCalendario.Models.API_Dtos;
using System.Security.Cryptography;
using System.Text;

namespace AgendaCalendario.Controllers.API
{
    /// <summary>
    /// Controlador API para gestão de Utilizadores
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class UtilizadoresApiController : ControllerBase
    {
        private readonly AgendaDbContext _context;

        /// <summary>
        /// Construtor que injeta o contexto da base de dados
        /// </summary>
        /// <param name="context">Contexto da base de dados</param>
        public UtilizadoresApiController(AgendaDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Obtém todos os utilizadores
        /// </summary>
        /// <returns>Lista de todos os utilizadores</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Utilizador>>> GetUtilizadores()
        {
            return await _context.Utilizadores.ToListAsync();
        }

        /// <summary>
        /// Obtém um utilizador específico pelo seu ID
        /// </summary>
        /// <param name="id">ID do utilizador</param>
        /// <returns>O utilizador encontrado ou NotFound se não existir</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<Utilizador>> GetUtilizador(int id)
        {
            var utilizador = await _context.Utilizadores.FindAsync(id);
            return utilizador == null ? NotFound() : utilizador;
        }

        /// <summary>
        /// Cria um novo utilizador
        /// </summary>
        /// <param name="dto">DTO com os dados do novo utilizador</param>
        /// <returns>O utilizador criado e o URL para aceder ao mesmo</returns>
        [HttpPost]
        public async Task<ActionResult<Utilizador>> PostUtilizador(UtilizadorCreateDto dto)
        {
            // Gera o hash da password
            var hash = ObterHash(dto.Password);

            // Cria nova instância de Utilizador com os dados do DTO
            var novo = new Utilizador
            {
                Nome = dto.Nome,
                Email = dto.Email,
                PasswordHash = hash
            };

            // Adiciona à base de dados e guarda alterações
            _context.Utilizadores.Add(novo);
            await _context.SaveChangesAsync();

            // Retorna resposta 201 (Created) com URL para o novo utilizador
            return CreatedAtAction(nameof(GetUtilizador), new { id = novo.Id }, novo);
        }

        /// <summary>
        /// Atualiza um utilizador existente
        /// </summary>
        /// <param name="id">ID do utilizador a atualizar</param>
        /// <param name="dto">DTO com os novos dados do utilizador</param>
        /// <returns>NoContent se sucesso, BadRequest se IDs não coincidirem, NotFound se não existir</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUtilizador(int id, UtilizadorUpdateDto dto)
        {
            // Verifica se o ID do URL corresponde ao ID do DTO
            if (id != dto.Id) return BadRequest();

            // Procura o utilizador na base de dados
            var utilizador = await _context.Utilizadores.FindAsync(id);
            if (utilizador == null) return NotFound();

            // Atualiza os dados básicos do utilizador
            utilizador.Nome = dto.Nome;
            utilizador.Email = dto.Email;

            // Atualiza a password apenas se uma nova for fornecida
            if (!string.IsNullOrWhiteSpace(dto.Password))
                utilizador.PasswordHash = ObterHash(dto.Password);

            // Guarda alterações
            await _context.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>
        /// Elimina um utilizador específico
        /// </summary>
        /// <param name="id">ID do utilizador a eliminar</param>
        /// <returns>NoContent se sucesso, NotFound se não existir</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUtilizador(int id)
        {
            var utilizador = await _context.Utilizadores.FindAsync(id);
            if (utilizador == null) return NotFound();

            // Remove o utilizador e guarda alterações
            _context.Utilizadores.Remove(utilizador);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>
        /// Método auxiliar para gerar hash SHA256 da password
        /// </summary>
        /// <param name="input">Password em texto plano</param>
        /// <returns>Hash SHA256 da password em formato hexadecimal</returns>
        private string ObterHash(string input)
        {
            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(input);
            var hash = sha.ComputeHash(bytes);
            return BitConverter.ToString(hash).Replace("-", "").ToLower();
        }
    }
}