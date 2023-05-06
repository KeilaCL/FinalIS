using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace FinalIS.Models;

public partial class FinalisContext : DbContext
{
    public FinalisContext()
    {
    }

    public FinalisContext(DbContextOptions<FinalisContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Candidato> Candidatos { get; set; }

    public virtual DbSet<Votante> Votantes { get; set; }

    public virtual DbSet<Voto> Votos { get; set; }

    public virtual DbSet<User> User { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseMySQL("server=127.0.0.1;userid=root;password=root;database=finalis;TreatTinyAsBoolean=False");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Candidato>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("candidato");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(200)
                .HasColumnName("descripcion");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .HasColumnName("nombre");
            entity.Property(e => e.Partido)
                .HasMaxLength(100)
                .HasColumnName("partido");
        });

        modelBuilder.Entity<Votante>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("votante");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Dpi)
                .HasMaxLength(13)
                .HasColumnName("dpi");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<Voto>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("voto");

            entity.HasIndex(e => e.IdCandidato, "id_candidato");
            entity.HasIndex(e => e.ip, "ip");
            entity.HasIndex(e => e.IdVotante, "id_votante");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.EsNulo)
                .HasColumnType("tinyint(1)")
                .HasColumnName("es_nulo");
            entity.Property(e => e.Fecha)
                .HasColumnType("date")
                .HasColumnName("fecha");
            entity.Property(e => e.IdCandidato).HasColumnName("id_candidato");
            entity.Property(e => e.IdVotante).HasColumnName("id_votante");

            //entity.HasOne(d => d.IdCandidatoNavigation).WithMany(p => p.Votos)
            //    .HasForeignKey(d => d.IdCandidato)
            //    .HasConstraintName("voto_ibfk_2");

            //entity.HasOne(d => d.IdVotanteNavigation).WithMany(p => p.Votos)
            //    .HasForeignKey(d => d.IdVotante)
            //    .OnDelete(DeleteBehavior.ClientSetNull)
            //    .HasConstraintName("voto_ibfk_1");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
