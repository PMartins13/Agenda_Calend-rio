using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using AgendaCalendario.Data;

// Cria o builder da aplicação
var builder = WebApplication.CreateBuilder(args);

// Configura o Entity Framework com SQLite
builder.Services.AddDbContext<AgendaDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Adiciona serviços essenciais
builder.Services.AddControllersWithViews(); // Suporte MVC
builder.Services.AddSession();              // Gerenciamento de sessão
builder.Services.AddEndpointsApiExplorer(); // Documentação API
builder.Services.AddSwaggerGen();           // Gerador Swagger

// Constrói a aplicação
var app = builder.Build();

// Configura o ambiente de produção
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error"); // Página de erro
    app.UseHsts();                         // Segurança HTTPS
}

// Pipeline de middleware HTTP
app.UseHttpsRedirection();    // Força HTTPS
app.UseStaticFiles();         // Serve arquivos estáticos
app.UseRouting();            // Configura rotas
app.UseSession();            // Habilita sessões

// Middleware de autorização do Swagger
app.UseMiddleware<AgendaCalendario.Middleware.SwaggerAuthorizationMiddleware>();

// Configuração do Swagger
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "AgendaCalendario API V1");
    c.RoutePrefix = "swagger"; // URL de acesso
});

app.UseAuthorization();      // Middleware de autorização

// Mapeia endpoints
app.MapControllers();        // Para controllers API

// Define rota MVC padrão
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Inicializa dados na base de dados
SeedData.Inicializar(app);

// Inicia a aplicação
app.Run();