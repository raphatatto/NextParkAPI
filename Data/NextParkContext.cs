using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NextParkAPI.Models;

namespace NextParkAPI.Data
{
    public class NextParkContext : DbContext
    {
        public NextParkContext(DbContextOptions<NextParkContext> options) : base(options) { }

        public DbSet<Moto> Motos { get; set; }
        public DbSet<Vaga> Vagas { get; set; }
        public DbSet<Manutencao> Manutencoes { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Login> Logins { get; set; }

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

            modelBuilder.Entity<Usuario>().ToTable("TB_NEXTPARK_USUARIO");

            modelBuilder.Entity<Usuario>().HasKey(u => u.IdUsuario);

            var usuarioIdProperty = modelBuilder.Entity<Usuario>().Property(u => u.IdUsuario).HasColumnName("ID_USUARIO");
            modelBuilder.Entity<Usuario>().Property(u => u.NrEmail).HasColumnName("NR_EMAIL").HasMaxLength(100);

            modelBuilder.Entity<Usuario>()
                .HasIndex(u => u.NrEmail)
                .IsUnique();

            modelBuilder.Entity<Login>().ToTable("TB_NEXTPARK_LOGIN");

            modelBuilder.Entity<Login>().HasKey(l => l.IdLogin);

            var loginIdProperty = modelBuilder.Entity<Login>().Property(l => l.IdLogin).HasColumnName("ID_LOGIN");
            modelBuilder.Entity<Login>().Property(l => l.IdUsuario).HasColumnName("ID_USUARIO");
            modelBuilder.Entity<Login>().Property(l => l.NrEmail).HasColumnName("NR_EMAIL").HasMaxLength(100);
            modelBuilder.Entity<Login>().Property(l => l.DsSenha).HasColumnName("DS_SENHA").HasMaxLength(255);

            modelBuilder.Entity<Login>()
                .HasIndex(l => l.NrEmail)
                .IsUnique();

            modelBuilder.Entity<Login>()
                .HasOne(l => l.Usuario)
                .WithMany(u => u.Logins)
                .HasForeignKey(l => l.IdUsuario)
                .OnDelete(DeleteBehavior.Cascade);

            ConfigureIdentityGeneration(usuarioIdProperty, loginIdProperty);
        }

        private void ConfigureIdentityGeneration(
            PropertyBuilder<int> usuarioIdProperty,
            PropertyBuilder<int> loginIdProperty)
        {
            var providerName = Database.ProviderName ?? string.Empty;
            var isSqlServer = providerName.IndexOf("SqlServer", StringComparison.OrdinalIgnoreCase) >= 0;

            if (isSqlServer)
            {
                usuarioIdProperty.ValueGeneratedOnAdd();
                loginIdProperty.ValueGeneratedOnAdd();
            }
            else
            {
                usuarioIdProperty.ValueGeneratedNever();
                loginIdProperty.ValueGeneratedNever();
            }
        }
    }
}
