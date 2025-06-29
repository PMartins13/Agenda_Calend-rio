using AgendaCalendario.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace AgendaCalendario.Data
{
    public static class SeedData
    {
        public static void Inicializar(IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AgendaDbContext>();

            context.Database.Migrate(); // Garante que a BD está atualizada

            if (!context.Utilizadores.Any())
            {
                context.Utilizadores.AddRange(
                    new Utilizador
                    {
                        Nome = "Administrador",
                        Email = "admin@teste.com",
                        PasswordHash = HashPassword("admin123"),
                        PerfilUtilizador = "Admin",
                        EmailConfirmado = true
                    },
                    new Utilizador
                    {
                        Nome = "Utilizador Teste",
                        Email = "utilizador@teste.com",
                        PasswordHash = HashPassword("User1234!"),
                        PerfilUtilizador = "Utilizador",
                        EmailConfirmado = true
                    }
                );

                context.SaveChanges();
            }
        }

        private static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);
            return BitConverter.ToString(hash).Replace("-", "").ToLower();
        }
    }
}