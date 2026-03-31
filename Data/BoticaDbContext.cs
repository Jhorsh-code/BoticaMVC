using BoticaMVC.Models;
using Microsoft.EntityFrameworkCore;

namespace BoticaMVC.Data
{
    public class BoticaDbContext : DbContext
    {
        public BoticaDbContext(DbContextOptions<BoticaDbContext> options) : base(options) { }

        public DbSet<Medicamento> Medicamentos => Set<Medicamento>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Medicamento>()
                .HasIndex(m => m.Codigo)
                .IsUnique();

            base.OnModelCreating(modelBuilder);
        }
    }
}
