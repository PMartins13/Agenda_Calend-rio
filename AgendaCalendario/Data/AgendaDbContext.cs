using Microsoft.EntityFrameworkCore;
using AgendaCalendario.Models;

namespace AgendaCalendario.Data
{
    public class AgendaDbContext : DbContext
    {
        public AgendaDbContext(DbContextOptions<AgendaDbContext> options)
            : base(options) { }

        public DbSet<Utilizador> Utilizadores { get; set; }
        public DbSet<Tarefa> Tarefas { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
    }
}