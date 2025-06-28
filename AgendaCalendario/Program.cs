using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using AgendaCalendario.Data;

var builder = WebApplication.CreateBuilder(args);

// Liga o DbContext à connection string definida no appsettings.json
builder.Services.AddDbContext<AgendaDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Adiciona os controladores com views (MVC).
builder.Services.AddControllersWithViews();
builder.Services.AddSession();

// Adiciona suporte ao Swagger e API Explorer
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configuração do pipeline HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Middleware do Swagger (visível mesmo em produção — opcional)
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "AgendaCalendario API V1");
    c.RoutePrefix = "swagger"; // Acede a partir de /swagger
});

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();

app.UseAuthorization();

// Garante que os controladores de API são mapeados corretamente
app.MapControllers();

// Controlador MVC padrão
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();