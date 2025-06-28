using AgendaCalendario.Data;
using AgendaCalendario.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Mail;
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

            var codigo = new Random().Next(100000, 999999).ToString();

            var novo = new Utilizador
            {
                Nome = nome,
                Email = email,
                PasswordHash = ObterHash(password),
                CodigoConfirmacao = codigo,
                EmailConfirmado = false
            };

            _context.Utilizadores.Add(novo);
            await _context.SaveChangesAsync();

            await EnviarEmailConfirmacao(email, codigo);

            TempData["EmailParaConfirmar"] = email;
            return RedirectToAction("ConfirmarEmail");
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

            var utilizador = await _context.Utilizadores.FirstOrDefaultAsync(u => u.Email == email);

            if (utilizador == null || utilizador.PasswordHash != ObterHash(password))
            {
                ModelState.AddModelError("", "Email ou password inválidos.");
                return View();
            }

            if (!utilizador.EmailConfirmado)
            {
                ModelState.AddModelError("", "Confirme o seu email antes de entrar.");
                TempData["EmailParaConfirmar"] = email;
                return RedirectToAction("ConfirmarEmail");
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

            // Atualize a sessão com o novo nome
            HttpContext.Session.SetString("UtilizadorNome", utilizador.Nome);

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

    private async Task EnviarEmailConfirmacao(string destino, string codigo)
    {
        var smtp = new SmtpClient("smtp.gmail.com")
        {
            Port = 587,
            Credentials = new NetworkCredential("agendacalendario.suporte@gmail.com", "bovf fhxb qvdy nkww"),
            EnableSsl = true,
        };

        var mail = new MailMessage
        {
            From = new MailAddress("agendacalendario.suporte@gmail.com", "Agenda Calendário"),
            Subject = "Confirmação de Email - Agenda Calendário",
            IsBodyHtml = true,
            Body = $@"
            <div style='font-family:Segoe UI,Arial,sans-serif;max-width:600px;margin:auto;border:1px solid #e3f2fd;border-radius:8px;padding:32px;background:#f9fbfd;'>
                <h2 style='color:#1861ac;margin-bottom:16px;'>Bem-vindo à Agenda Calendário!</h2>
                <p>Olá,</p>
                <p>Obrigado por te registares na <strong>Agenda Calendário</strong>.<br>
                Para concluíres o teu registo e começares a organizar as tuas tarefas, confirma o teu email inserindo o código abaixo:</p>
                <div style='text-align:center;margin:32px 0;'>
                    <span style='display:inline-block;font-size:2rem;letter-spacing:8px;background:#e3f2fd;color:#1861ac;padding:16px 32px;border-radius:8px;font-weight:bold;border:1px dashed #1861ac;'>{codigo}</span>
                </div>
                <p>Se não foste tu a criar esta conta, podes ignorar este email.</p>
                <hr style='margin:32px 0;'>
                <p style='font-size:0.95rem;color:#888;'>Obrigado por confiares na nossa aplicação.<br>
                Qualquer dúvida ou sugestão, responde a este email.<br><br>
                Cumprimentos,<br>
                <strong>Equipa Agenda Calendário</strong></p>
            </div>"
        };

        mail.To.Add(destino);

        await smtp.SendMailAsync(mail);
    }

    [HttpGet]
    public async Task<IActionResult> ReenviarCodigo(string email)
    {
        if (string.IsNullOrEmpty(email))
            return RedirectToAction("Login");
    
        var utilizador = await _context.Utilizadores.FirstOrDefaultAsync(u => u.Email == email);
        if (utilizador == null)
            return RedirectToAction("Login");
    
        // Gera novo código
        var codigo = new Random().Next(100000, 999999).ToString();
        utilizador.CodigoConfirmacao = codigo;
        await _context.SaveChangesAsync();
    
        // Envia email
        await EnviarEmailConfirmacao(email, codigo);
    
        TempData["EmailParaConfirmar"] = email;
        TempData["CodigoReenviado"] = "Um novo código foi enviado para o seu email.";
        return RedirectToAction("ConfirmarEmail");
    }

        [HttpGet]
        public IActionResult ConfirmarEmail()
        {
            ViewBag.Email = TempData["EmailParaConfirmar"];
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmarEmail(string email, string codigo)
        {
            var utilizador = await _context.Utilizadores.FirstOrDefaultAsync(u => u.Email == email);
            if (utilizador == null || utilizador.CodigoConfirmacao != codigo)
            {
                ViewBag.Erro = "Código inválido.";
                ViewBag.Email = email;
                return View();
            }

            utilizador.EmailConfirmado = true;
            utilizador.CodigoConfirmacao = null;
            await _context.SaveChangesAsync();

            return RedirectToAction("Login");
        }
    }
}
