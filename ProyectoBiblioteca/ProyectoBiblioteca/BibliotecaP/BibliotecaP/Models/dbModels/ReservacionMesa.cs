﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaP.Models.dbModels;

public partial class ReservacionMesa
{
    [Key]
    [Column("ReservacionID")]
    public int ReservacionId { get; set; }

    [Column("MesaID")]
    public int MesaId { get; set; }

    [Column("UsuarioID")]
    public int UsuarioId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime FechaHoraInicio { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime FechaHoraFin { get; set; }

    [ForeignKey("MesaId")]
    [InverseProperty("ReservacionMesas")]
    public virtual Mesa Mesa { get; set; } = null!;

    [ForeignKey("UsuarioId")]
    [InverseProperty("ReservacionMesas")]
    public virtual AplicationUser Usuario { get; set; } = null!;
}
