﻿using AgendaCalendario.Data;
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
    /// <summary>
    /// Controlador responsável pela gestão de contas de utilizador
    /// </summary>
    public class ContaController : Controller
    {
        private readonly AgendaDbContext _context;

        public ContaController(AgendaDbContext context)
        {
            _context = context;
        }

        #region Autenticação e Registo

        /// <summary>
        /// Exibe formulário de registo
        /// </summary>
        public IActionResult Registar() => View();

        /// <summary>
        /// Processa o registo de novo utilizador
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Registar(string nome, string email, string password)
        {
            // Validações
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

            // Validação complexa da password
            if (!Regex.IsMatch(password, @"^(?=(?:.*[A-Z]){1,})(?=(?:.*\d){4,})(?=(?:.*[!@#$%^&*()_\-=\[\]{};':""\\|,.<>\/?]){1,}).{8,}$"))
            {
                ModelState.AddModelError("", "Password inválida: requisitos mínimos não cumpridos.");
                return View();
            }

            // Criação do utilizador
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

            // Envio do email de confirmação
            await EnviarEmailConfirmacao(email, codigo);

            TempData["EmailParaConfirmar"] = email;
            return RedirectToAction("ConfirmarEmail");
        }

        /// <summary>
        /// Exibe formulário de login
        /// </summary>
        public IActionResult Login() => View();

        /// <summary>
        /// Processa tentativa de login
        /// </summary>
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
                TempData["EmailParaConfirmar"] = email;
                return RedirectToAction("ConfirmarEmail");
            }

            // Define dados da sessão
            HttpContext.Session.SetString("UtilizadorNome", utilizador.Nome);
            HttpContext.Session.SetInt32("UtilizadorId", utilizador.Id);
            HttpContext.Session.SetString("UtilizadorEmail", utilizador.Email);
            HttpContext.Session.SetString("PerfilUtilizador", utilizador.PerfilUtilizador);

            return RedirectToAction("Index", "Calendario");
        }

        #endregion

        #region Gestão de Perfil

        /// <summary>
        /// Exibe perfil do utilizador
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Perfil()
        {
            var utilizadorId = HttpContext.Session.GetInt32("UtilizadorId");
            if (utilizadorId == null) return RedirectToAction("Login");

            var utilizador = await _context.Utilizadores.FindAsync(utilizadorId);
            if (utilizador == null) return NotFound();

            return View(new UtilizadorPerfilViewModel
            {
                Nome = utilizador.Nome,
                Email = utilizador.Email
            });
        }

        /// <summary>
        /// Atualiza dados do perfil
        /// </summary>
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
            HttpContext.Session.SetString("UtilizadorNome", utilizador.Nome);

            ViewBag.Mensagem = "Dados atualizados com sucesso!";
            return View(vm);
        }

        /// <summary>
        /// Processa logout do utilizador
        /// </summary>
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// Elimina conta do utilizador e todos os seus dados
        /// </summary>
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

        #endregion

        #region Confirmação de Email

        /// <summary>
        /// Envia email de confirmação
        /// </summary>
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

        /// <summary>
        /// Reenvia código de confirmação
        /// </summary>
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

        /// <summary>
        /// Exibe página de confirmação de email
        /// </summary>
        [HttpGet]
        public IActionResult ConfirmarEmail()
        {
            ViewBag.Email = TempData["EmailParaConfirmar"];
            return View();
        }

        /// <summary>
        /// Processa confirmação de email
        /// </summary>
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

        #endregion

        /// <summary>
        /// Gera hash SHA256 para password
        /// </summary>
        private string ObterHash(string input)
        {
            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(input);
            var hash = sha.ComputeHash(bytes);
            return BitConverter.ToString(hash).Replace("-", "").ToLower();
        }
    }
}