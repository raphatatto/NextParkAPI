using Microsoft.EntityFrameworkCore;
using NextParkAPI.Models;

namespace NextParkAPI.Data
{
    public class NextParkContext : DbContext
    {
        public NextParkContext(DbContextOptions<NextParkContext> options) : base(options) { }

        public DbSet<Moto> Motos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Moto>().ToTable("TB_NEXTPARK_MOTO");

            modelBuilder.Entity<Moto>().HasKey(m => m.IdMoto);

            modelBuilder.Entity<Moto>().Property(m => m.IdMoto).HasColumnName("ID_MOTO");
            modelBuilder.Entity<Moto>().Property(m => m.NrPlaca).HasColumnName("NR_PLACA").HasMaxLength(50);
            modelBuilder.Entity<Moto>().Property(m => m.NmModelo).HasColumnName("NM_MODELO").HasMaxLength(50);
            modelBuilder.Entity<Moto>().Property(m => m.StMoto).HasColumnName("ST_MOTO");
            modelBuilder.Entity<Moto>().Property(m => m.IdVaga).HasColumnName("ID_VAGA");
        }
    }
}
