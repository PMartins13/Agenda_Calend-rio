using AgendaCalendario.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace AgendaCalendario.Data
{
    /// <summary>
    /// Classe estática responsável pela inicialização de dados na base de dados
    /// </summary>
    public static class SeedData
    {
        /// <summary>
        /// Método que inicializa a base de dados com dados predefinidos
        /// </summary>
        /// <param name="app">Interface do construtor da aplicação</param>
        public static void Inicializar(IApplicationBuilder app)
        {
            // Cria um âmbito de serviços para gerir os recursos
            using var scope = app.ApplicationServices.CreateScope();
            // Obtém o contexto da base de dados através da injeção de dependências
            var context = scope.ServiceProvider.GetRequiredService<AgendaDbContext>();

            // Executa as migrações pendentes para garantir que a base de dados está atualizada
            context.Database.Migrate();

            // Verifica se existem utilizadores na base de dados
            if (!context.Utilizadores.Any())
            {
                // Adiciona utilizadores predefinidos se a tabela estiver vazia
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

                // Guarda as alterações na base de dados
                context.SaveChanges();
            }
        }

        /// <summary>
        /// Método privado para criar um hash seguro da palavra-passe
        /// </summary>
        /// <param name="password">Palavra-passe em texto simples</param>
        /// <returns>Hash da palavra-passe em formato hexadecimal</returns>
        private static string HashPassword(string password)
        {
            // Cria uma instância do algoritmo SHA256
            using var sha256 = SHA256.Create();
            // Converte a palavra-passe para bytes usando codificação UTF8
            var bytes = Encoding.UTF8.GetBytes(password);
            // Calcula o hash da palavra-passe
            var hash = sha256.ComputeHash(bytes);
            // Converte o hash para uma string hexadecimal sem hífens
            return BitConverter.ToString(hash).Replace("-", "").ToLower();
        }
    }
}