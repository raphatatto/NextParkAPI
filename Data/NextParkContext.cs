using Microsoft.EntityFrameworkCore;
using NextParkAPI.Models;

namespace NextParkAPI.Data
{
    public class NextParkContext : DbContext
    {
        public NextParkContext(DbContextOptions<NextParkContext> options) : base(options) { }

        public DbSet<Moto> Motos { get; set; }
        public DbSet<Vaga> Vagas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Moto>().ToTable("TB_NEXTPARK_MOTO");

            modelBuilder.Entity<Moto>().HasKey(m => m.IdMoto);

            modelBuilder.Entity<Moto>().Property(m => m.IdMoto).HasColumnName("ID_MOTO");
            modelBuilder.Entity<Moto>().Property(m => m.NrPlaca).HasColumnName("NR_PLACA").HasMaxLength(50);
            modelBuilder.Entity<Moto>().Property(m => m.NmModelo).HasColumnName("NM_MODELO").HasMaxLength(50);
            modelBuilder.Entity<Moto>().Property(m => m.StMoto).HasColumnName("ST_MOTO");
            modelBuilder.Entity<Moto>().Property(m => m.IdVaga).HasColumnName("ID_VAGA");

            modelBuilder.Entity<Vaga>().ToTable("TB_NEXTPARK_VAGA");
            
            modelBuilder.Entity<Vaga>().HasKey(v => v.IdVaga);

            modelBuilder.Entity<Vaga>().Property(v => v.IdVaga).HasColumnName("ID_VAGA");
            modelBuilder.Entity<Vaga>().Property(v => v.AreaVaga).HasColumnName("AREA_VAGA");
            modelBuilder.Entity<Vaga>().Property(v => v.StVaga).HasColumnName("ST_VAGA");
            modelBuilder.Entity<Vaga>().Property(v => v.IdPatio).HasColumnName("ID_PATIO");

        }

    }
}
