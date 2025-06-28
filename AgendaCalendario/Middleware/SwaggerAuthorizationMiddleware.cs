using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace AgendaCalendario.Middleware
{
    public class SwaggerAuthorizationMiddleware
    {
        private readonly RequestDelegate _next;

        public SwaggerAuthorizationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path.Value;

            if (path.StartsWith("/swagger") &&
                (!context.Session.TryGetValue("PerfilUtilizador", out var perfilBytes) ||
                 System.Text.Encoding.UTF8.GetString(perfilBytes) != "Admin"))
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsync("Acesso ao Swagger apenas permitido a administradores.");
                return;
            }

            await _next(context);
        }
    }
}