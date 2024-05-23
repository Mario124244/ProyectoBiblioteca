using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaP.Models.dbModels;
public partial class ReservacionCubiculo
{
    [Key]
    [Column("ReservacionID")]
    public int ReservacionId { get; set; }

    [Column("CubiculoID")]
    public int CubiculoId { get; set; }

    [Column("UsuarioID")]
    public int UsuarioId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime FechaHoraInicio { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime FechaHoraFin { get; set; }

    [ForeignKey("CubiculoId")]
    [InverseProperty("ReservacionCubiculos")]
    public virtual Cubiculo Cubiculo { get; set; } = null!;

    [ForeignKey("UsuarioId")]
    [InverseProperty("ReservacionCubiculos")]
    public virtual AplicationUser Usuario { get; set; } = null!;

    // Propiedades para los códigos QR
    public string? AccesoQrBase64 { get; set; }
    public string? SalidaQrBase64 { get; set; }
}