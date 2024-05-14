using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaP.Models.dbModels;

public partial class Cubiculo
{
    [Key]
    [Column("CubiculoID")]
    public int CubiculoId { get; set; }

    [StringLength(50)]
    public string Nombre { get; set; } = null!;

    [Column("EstadoID")]
    public int EstadoId { get; set; }

    [ForeignKey("EstadoId")]
    [InverseProperty("Cubiculos")]
    public virtual EstadoCubiculo Estado { get; set; } = null!;

    [InverseProperty("Cubiculo")]
    public virtual ICollection<ReservacionCubiculo> ReservacionCubiculos { get; set; } = new List<ReservacionCubiculo>();
}
