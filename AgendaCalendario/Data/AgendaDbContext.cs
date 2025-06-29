using Microsoft.EntityFrameworkCore;
using AgendaCalendario.Models;

namespace AgendaCalendario.Data
{
    /// <summary>
    /// Contexto do Entity Framework para acesso à base de dados da aplicação
    /// </summary>
    public class AgendaDbContext : DbContext
    {
        /// <summary>
        /// Construtor que recebe as opções de configuração do contexto
        /// </summary>
        /// <param name="options">Opções de configuração do DbContext</param>
        public AgendaDbContext(DbContextOptions<AgendaDbContext> options)
            : base(options) { }

        /// <summary>
        /// DbSet para acesso à tabela de Utilizadores
        /// </summary>
        public DbSet<Utilizador> Utilizadores { get; set; }

        /// <summary>
        /// DbSet para acesso à tabela de Tarefas
        /// </summary>
        public DbSet<Tarefa> Tarefas { get; set; }

        /// <summary>
        /// DbSet para acesso à tabela de Categorias
        /// </summary>
        public DbSet<Categoria> Categorias { get; set; }
    }
}