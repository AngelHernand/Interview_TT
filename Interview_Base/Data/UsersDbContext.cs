using System;
using Microsoft.EntityFrameworkCore;
using Interview_Base.Models;

namespace Interview_Base.Data;

/// <summary>
/// DbContext generado por scaffold — configurado para Database First
/// </summary>
public partial class UsersDbContext : DbContext
{
    public UsersDbContext() { }

    public UsersDbContext(DbContextOptions<UsersDbContext> options)
        : base(options) { }

    public virtual DbSet<Rol> Roles { get; set; }
    public virtual DbSet<Usuario> Usuarios { get; set; }
    public virtual DbSet<RefreshToken> RefreshTokens { get; set; }
    public virtual DbSet<AuditLog> AuditLogs { get; set; }
    public virtual DbSet<LoginAttempt> LoginAttempts { get; set; }
    public virtual DbSet<VwUsuariosActivo> VwUsuariosActivos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Rol 
        modelBuilder.Entity<Rol>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.ToTable("Roles");

            entity.Property(e => e.Nombre).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Descripcion).HasMaxLength(200);

            entity.HasIndex(e => e.Nombre).IsUnique();
        });

        // Usuario 
        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.ToTable("Usuarios");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.Nombre).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Email).HasMaxLength(255).IsRequired();
            entity.Property(e => e.PasswordHash).HasMaxLength(500).IsRequired();
            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.Bloqueado).HasDefaultValue(false);
            entity.Property(e => e.IntentosLogin).HasDefaultValue(0);
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("(getdate())");

            entity.HasIndex(e => e.Email).IsUnique();

            entity.HasOne(d => d.Rol)
                  .WithMany(p => p.Usuarios)
                  .HasForeignKey(d => d.RolId)
                  .OnDelete(DeleteBehavior.ClientSetNull);
        });

        // RefreshToken 
        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.ToTable("RefreshTokens");

            entity.Property(e => e.Token).HasMaxLength(500).IsRequired();
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Revocado).HasDefaultValue(false);
            entity.Property(e => e.ReemplazadoPor).HasMaxLength(500);
            entity.Property(e => e.DireccionIp).HasColumnName("DireccionIP").HasMaxLength(50);
            entity.Property(e => e.UserAgent).HasMaxLength(500);

            entity.HasOne(d => d.Usuario)
                  .WithMany(p => p.RefreshTokens)
                  .HasForeignKey(d => d.UsuarioId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        //  AuditLog 
        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.ToTable("AuditLogs");

            entity.Property(e => e.Accion).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Entidad).HasMaxLength(100);
            entity.Property(e => e.EntidadId).HasMaxLength(100);
            entity.Property(e => e.DireccionIp).HasColumnName("DireccionIP").HasMaxLength(50);
            entity.Property(e => e.UserAgent).HasMaxLength(500);
            entity.Property(e => e.Fecha).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Usuario)
                  .WithMany(p => p.AuditLogs)
                  .HasForeignKey(d => d.UsuarioId)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        // LoginAttempt 
        modelBuilder.Entity<LoginAttempt>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.ToTable("LoginAttempts");

            entity.Property(e => e.Email).HasMaxLength(255).IsRequired();
            entity.Property(e => e.DireccionIp).HasColumnName("DireccionIP").HasMaxLength(50);
            entity.Property(e => e.UserAgent).HasMaxLength(500);
            entity.Property(e => e.MensajeError).HasMaxLength(500);
            entity.Property(e => e.Fecha).HasDefaultValueSql("(getdate())");
        });

        // Vista: vw_UsuariosActivos 
        modelBuilder.Entity<VwUsuariosActivo>(entity =>
        {
            entity.HasNoKey();
            entity.ToView("vw_UsuariosActivos");

            entity.Property(e => e.Nombre).HasMaxLength(100);
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.Rol).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
