using System.Data.Entity;

namespace TesteS4E.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext() : base("TesteS4E")
        {
        }
        public DbSet<Associado> Associados { get; set; }
        public DbSet<Empresa> Empresas { get; set; }
        public DbSet<AssociadoEmpresa> AssociadoEmpresa { get; set; }
    }
}