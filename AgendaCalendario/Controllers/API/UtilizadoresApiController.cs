﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AgendaCalendario.Data;
using AgendaCalendario.Models;
using AgendaCalendario.Models.API_Dtos;
using System.Security.Cryptography;
using System.Text;

namespace AgendaCalendario.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class UtilizadoresApiController : ControllerBase
    {
        private readonly AgendaDbContext _context;

        public UtilizadoresApiController(AgendaDbContext context)
        {
            _context = context;
        }

        // GET: api/UtilizadoresApi
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Utilizador>>> GetUtilizadores()
        {
            return await _context.Utilizadores.ToListAsync();
        }

        // GET: api/UtilizadoresApi/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Utilizador>> GetUtilizador(int id)
        {
            var utilizador = await _context.Utilizadores.FindAsync(id);
            return utilizador == null ? NotFound() : utilizador;
        }

        // POST: api/UtilizadoresApi
        [HttpPost]
        public async Task<ActionResult<Utilizador>> PostUtilizador(UtilizadorCreateDto dto)
        {
            var hash = ObterHash(dto.Password);

            var novo = new Utilizador
            {
                Nome = dto.Nome,
                Email = dto.Email,
                PasswordHash = hash
            };

            _context.Utilizadores.Add(novo);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUtilizador), new { id = novo.Id }, novo);
        }

        // PUT: api/UtilizadoresApi/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUtilizador(int id, UtilizadorUpdateDto dto)
        {
            if (id != dto.Id) return BadRequest();

            var utilizador = await _context.Utilizadores.FindAsync(id);
            if (utilizador == null) return NotFound();

            utilizador.Nome = dto.Nome;
            utilizador.Email = dto.Email;

            if (!string.IsNullOrWhiteSpace(dto.Password))
                utilizador.PasswordHash = ObterHash(dto.Password);

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/UtilizadoresApi/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUtilizador(int id)
        {
            var utilizador = await _context.Utilizadores.FindAsync(id);
            if (utilizador == null) return NotFound();

            _context.Utilizadores.Remove(utilizador);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // Helper para encriptar password
        private string ObterHash(string input)
        {
            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(input);
            var hash = sha.ComputeHash(bytes);
            return BitConverter.ToString(hash).Replace("-", "").ToLower();
        }
    }
}