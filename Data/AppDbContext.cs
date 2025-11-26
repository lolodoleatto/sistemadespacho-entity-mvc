using TP_MVC_CRUD.Models;
using Microsoft.EntityFrameworkCore;

namespace TP_MVC_CRUD.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Direccion> Direcciones { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<Despacho> Despachos { get; set; }
        public DbSet<DespachoDetalle> DespachoDetalles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DespachoDetalle>()
                .HasOne(dd => dd.Producto)
                .WithMany(p => p.Detalles)
                .HasForeignKey(dd => dd.ProductoId);

            modelBuilder.Entity<DespachoDetalle>()
                .HasOne(dd => dd.Despacho)
                .WithMany(d => d.Detalles)
                .HasForeignKey(dd => dd.DespachoId);
        }
    }
}
