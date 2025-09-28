using Microsoft.EntityFrameworkCore;
using NextParkAPI.Models;

namespace NextParkAPI.Data
{
    public class NextParkContext : DbContext
    {
        public NextParkContext(DbContextOptions<NextParkContext> options) : base(options) { }

        public DbSet<Moto> Motos { get; set; }
        public DbSet<Vaga> Vagas { get; set; }
        public DbSet<Manutencao> Manutencoes { get; set; }

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

            modelBuilder.Entity<Manutencao>().ToTable("TB_NEXTPARK_MANUTENCAO");

            modelBuilder.Entity<Manutencao>().HasKey(m => m.IdManutencao);

            modelBuilder.Entity<Manutencao>().Property(m => m.IdManutencao).HasColumnName("ID_MANUTENCAO");
            modelBuilder.Entity<Manutencao>().Property(m => m.DsManutencao).HasColumnName("DS_MANUTENCAO").HasMaxLength(255);
            modelBuilder.Entity<Manutencao>().Property(m => m.DtInicio).HasColumnName("DT_INICIO");
            modelBuilder.Entity<Manutencao>().Property(m => m.DtFim).HasColumnName("DT_FIM");
            modelBuilder.Entity<Manutencao>().Property(m => m.IdMoto).HasColumnName("ID_MOTO");

            modelBuilder.Entity<Manutencao>()
                .HasOne<Moto>()
                .WithMany()
                .HasForeignKey(m => m.IdMoto)
                .OnDelete(DeleteBehavior.Restrict);

        }

    }
}
