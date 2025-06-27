using AgendaCalendario.Data;
using AgendaCalendario.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

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
        public async Task<IActionResult> Registar(string nome, string email, string password)
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
            
            if (!Regex.IsMatch(password, @"^(?=(?:.*[A-Z]){1,})(?=(?:.*\d){4,})(?=(?:.*[!@#$%^&*()_\-=\[\]{};':""\\|,.<>\/?]){1,}).{8,}$"))
            {
                ModelState.AddModelError("", "Password inválida (back-end): requisitos mínimos não cumpridos.");
                return View();
            }


            var hash = ObterHash(password);

            var novo = new Utilizador
            {
                Nome = nome,
                Email = email,
                PasswordHash = hash
            };

            _context.Utilizadores.Add(novo);
            await _context.SaveChangesAsync();
            
            HttpContext.Session.SetString("UtilizadorNome", novo.Nome);
            HttpContext.Session.SetInt32("UtilizadorId", novo.Id);
            HttpContext.Session.SetString("UtilizadorEmail", novo.Email);
            return RedirectToAction("Index", "Home");
            
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
            HttpContext.Session.SetString("UtilizadorNome", utilizador.Nome);
            HttpContext.Session.SetInt32("UtilizadorId", utilizador.Id);
            HttpContext.Session.SetString("UtilizadorEmail", utilizador.Email);
            return RedirectToAction("Index", "Calendario");
            
        }
        
        [HttpGet]
        public async Task<IActionResult> Perfil()
        {
            var utilizadorId = HttpContext.Session.GetInt32("UtilizadorId");
            if (utilizadorId == null) return RedirectToAction("Login");

            var utilizador = await _context.Utilizadores.FindAsync(utilizadorId);
            if (utilizador == null) return NotFound();

            var vm = new UtilizadorPerfilViewModel
            {
                Nome = utilizador.Nome,
                Email = utilizador.Email
            };

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Perfil(UtilizadorPerfilViewModel vm)
        {
            var utilizadorId = HttpContext.Session.GetInt32("UtilizadorId");
            if (utilizadorId == null) return RedirectToAction("Login");

            if (!ModelState.IsValid) return View(vm);

            var utilizador = await _context.Utilizadores.FindAsync(utilizadorId);
            if (utilizador == null) return NotFound();

            utilizador.Nome = vm.Nome;
            utilizador.Email = vm.Email;

            if (!string.IsNullOrWhiteSpace(vm.NovaPassword))
                utilizador.PasswordHash = ObterHash(vm.NovaPassword); 

            await _context.SaveChangesAsync();

            ViewBag.Mensagem = "Dados atualizados com sucesso!";
            return View(vm);
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApagarConta()
        {
            var utilizadorId = HttpContext.Session.GetInt32("UtilizadorId");
            if (utilizadorId == null) return RedirectToAction("Login");

            var utilizador = await _context.Utilizadores
                .Include(u => u.Tarefas)
                .Include(u => u.Categorias)
                .FirstOrDefaultAsync(u => u.Id == utilizadorId);

            if (utilizador == null) return NotFound();

            _context.Tarefas.RemoveRange(utilizador.Tarefas);
            _context.Categorias.RemoveRange(utilizador.Categorias);
            _context.Utilizadores.Remove(utilizador);

            await _context.SaveChangesAsync();

            HttpContext.Session.Clear();
            TempData["ContaApagada"] = "A sua conta foi eliminada com sucesso.";
            return RedirectToAction("Index", "Home");
        }
    }
}
