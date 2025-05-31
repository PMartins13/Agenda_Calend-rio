using Microsoft.EntityFrameworkCore;
using AgendaCalendario.Data;

var builder = WebApplication.CreateBuilder(args);

// ⚙️ Liga o DbContext à connection string definida no appsettings.json
builder.Services.AddDbContext<AgendaDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Adiciona os controladores com views (MVC).
builder.Services.AddControllersWithViews();
builder.Services.AddSession();

var app = builder.Build();

// Configuração do pipeline HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();