using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace AgendaCalendario.Middleware
{
    /// <summary>
    /// Middleware que controla o acesso à interface do Swagger
    /// Apenas utilizadores com perfil de Admin podem aceder
    /// </summary>
    public class SwaggerAuthorizationMiddleware
    {
        private readonly RequestDelegate _next;

        /// <summary>
        /// Construtor do middleware
        /// </summary>
        /// <param name="next">Próximo middleware na pipeline</param>
        public SwaggerAuthorizationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        /// <summary>
        /// Método executado para cada pedido HTTP
        /// </summary>
        /// <param name="context">Contexto HTTP atual</param>
        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path.Value;

            // Verifica se o pedido é para o Swagger e se o utilizador tem permissões
            if (path.StartsWith("/swagger") &&
                (!context.Session.TryGetValue("PerfilUtilizador", out var perfilBytes) ||
                 System.Text.Encoding.UTF8.GetString(perfilBytes) != "Admin"))
            {
                // Se não tiver permissões, retorna erro 403 Forbidden
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsync(
                    "Acesso ao Swagger apenas permitido a administradores.");
                return;
            }

            // Se não for pedido Swagger ou tiver permissões, continua para o próximo middleware
            await _next(context);
        }
    }
}