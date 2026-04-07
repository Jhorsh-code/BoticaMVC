using BoticaMVC.Models;
using Microsoft.EntityFrameworkCore;

namespace BoticaMVC.Data
{
    public class BoticaDbContext : DbContext
    {
        public BoticaDbContext(DbContextOptions<BoticaDbContext> options) : base(options) { }

        public DbSet<Medicamento> Medicamentos => Set<Medicamento>();
        public DbSet<Cliente> Clientes => Set<Cliente>();
        public DbSet<Venta> Ventas => Set<Venta>();
        public DbSet<DetalleVenta> DetalleVentas => Set<DetalleVenta>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Medicamento>()
                .HasIndex(m => m.Codigo)
                .IsUnique();

            modelBuilder.Entity<Venta>()
                .HasOne(v => v.Cliente)
                .WithMany()
                .HasForeignKey(v => v.ClienteId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<DetalleVenta>()
                .HasOne(d => d.Venta)
                .WithMany(v => v.Detalles)
                .HasForeignKey(d => d.VentaId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<DetalleVenta>()
                .HasOne(d => d.Medicamento)
                .WithMany()
                .HasForeignKey(d => d.MedicamentoId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Venta>()
                .Property(v => v.SubTotal)
                .HasColumnType("decimal(10,2)");

            modelBuilder.Entity<Venta>()
                .Property(v => v.Igv)
                .HasColumnType("decimal(10,2)");

            modelBuilder.Entity<Venta>()
                .Property(v => v.Total)
                .HasColumnType("decimal(10,2)");

            modelBuilder.Entity<DetalleVenta>()
                .Property(d => d.PrecioUnitario)
                .HasColumnType("decimal(10,2)");

            modelBuilder.Entity<DetalleVenta>()
                .Property(d => d.Importe)
                .HasColumnType("decimal(10,2)");
        }
    }
}