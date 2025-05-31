using AgendaCalendario.Data;
using AgendaCalendario.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace AgendaCalendario.Controllers
{
    public class ContaController : Controller
    {
        private readonly AgendaDbContext _context;

        public ContaController(AgendaDbContext context)
        {
            _context = context;
        }

        // GET: Conta/Registar
        public IActionResult Registar()
        {
            return View();
        }

        // POST: Conta/Registar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Registar(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ModelState.AddModelError("", "Email e password são obrigatórios.");
                return View();
            }

            if (await _context.Utilizadores.AnyAsync(u => u.Email == email))
            {
                ModelState.AddModelError("", "Este email já está registado.");
                return View();
            }

            var hash = ObterHash(password);

            var novo = new Utilizador
            {
                Email = email,
                PasswordHash = hash
            };

            _context.Utilizadores.Add(novo);
            await _context.SaveChangesAsync();

            HttpContext.Session.SetInt32("UtilizadorId", novo.Id);
            return RedirectToAction("Index", "Home"); // podes mudar para "Calendario" quando existir
        }

        // GET: Conta/Login
        public IActionResult Login()
        {
            return View();
        }

        // POST: Conta/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ModelState.AddModelError("", "Email e password são obrigatórios.");
                return View();
            }

            var utilizador = await _context.Utilizadores
                .FirstOrDefaultAsync(u => u.Email == email);

            if (utilizador == null)
            {
                ModelState.AddModelError("", "Email ou password inválidos.");
                return View();
            }

            var hash = ObterHash(password);

            if (utilizador.PasswordHash != hash)
            {
                ModelState.AddModelError("", "Email ou password inválidos.");
                return View();
            }

            // Login com sucesso
            HttpContext.Session.SetInt32("UtilizadorId", utilizador.Id);
            return RedirectToAction("Index", "Home");
        }
        
        public IActionResult Logout()
        {
            HttpContext.Session.Clear(); // limpa a sessão atual
            return RedirectToAction("Index", "Home");
        }

        // Função auxiliar para encriptar password
        private string ObterHash(string input)
        {
            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(input);
            var hash = sha.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }
}
