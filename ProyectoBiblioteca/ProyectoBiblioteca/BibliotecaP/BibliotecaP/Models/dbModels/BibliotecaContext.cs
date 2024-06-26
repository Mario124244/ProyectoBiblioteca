﻿using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaP.Models.dbModels;

public partial class BibliotecaContext : IdentityDbContext<AplicationUser, IdentityRole<int>, int>
{
   

    public BibliotecaContext(DbContextOptions<BibliotecaContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Aviso> Avisos { get; set; }
    public virtual DbSet<Cubiculo> Cubiculos { get; set; }
    public virtual DbSet<EstadoCubiculo> EstadoCubiculos { get; set; }
    public virtual DbSet<EstadoMesa> EstadoMesas { get; set; }
    public virtual DbSet<Mesa> Mesas { get; set; }
    public virtual DbSet<ReservacionCubiculo> ReservacionCubiculos { get; set; }
    public virtual DbSet<ReservacionMesa> ReservacionMesas { get; set; }
    public virtual DbSet<Reseña> Reseñas { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);


        modelBuilder.Entity<Aviso>(entity =>
        {
            entity.HasOne(d => d.Usuario)
                .WithMany(p => p.Avisos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Avisos_Usuarios");
        });

        modelBuilder.Entity<Cubiculo>(entity =>
        {
            entity.HasKey(e => e.CubiculoId).HasName("PK__Cubiculo__7607D6432A33615A");

            entity.HasOne(d => d.Estado)
                .WithMany(p => p.Cubiculos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Cubiculos__Estad__5441852A");
        });

        modelBuilder.Entity<EstadoCubiculo>(entity =>
        {
            entity.HasKey(e => e.EstadoId).HasName("PK__EstadoCu__FEF86B604EEAE8D8");
        });

        modelBuilder.Entity<EstadoMesa>(entity =>
        {
            entity.HasKey(e => e.EstadoId).HasName("PK__EstadoMe__FEF86B600687573A");
        });

        modelBuilder.Entity<Mesa>(entity =>
        {
            entity.HasKey(e => e.MesaId).HasName("PK__Mesas__6A4196C85BF6333E");

            entity.HasOne(d => d.Estado)
                .WithMany(p => p.Mesas)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Mesas__EstadoID__571DF1D5");
        });

        modelBuilder.Entity<ReservacionCubiculo>(entity =>
        {
            entity.HasKey(e => e.ReservacionId).HasName("PK__Reservac__145B3EB530AA4DBE");

            entity.HasOne(d => d.Cubiculo)
                .WithMany(p => p.ReservacionCubiculos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ReservacionCubiculos_Cubiculos");

            entity.HasOne(d => d.Usuario)
                .WithMany(p => p.ReservacionCubiculos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Reservaci__Usuar__5CD6CB2B");
        });

        modelBuilder.Entity<ReservacionMesa>(entity =>
        {
            entity.HasKey(e => e.ReservacionId).HasName("PK__Reservac__145B3EB57A155FBF");

            entity.HasOne(d => d.Mesa)
                .WithMany(p => p.ReservacionMesas)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ReservacionMesas_Mesas");

            entity.HasOne(d => d.Usuario)
                .WithMany(p => p.ReservacionMesas)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Reservaci__Usuar__619B8048");
        });

        modelBuilder.Entity<Reseña>(entity =>
        {
            entity.HasOne(d => d.Usuario)
                .WithMany(p => p.Reseñas)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Reseñas_Usuarios");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

