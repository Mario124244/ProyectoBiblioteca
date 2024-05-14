using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaP.Models.dbModels;

public partial class Mesa
{
    [Key]
    [Column("MesaID")]
    public int MesaId { get; set; }

    [StringLength(50)]
    public string Nombre { get; set; } = null!;

    [Column("EstadoID")]
    public int EstadoId { get; set; }

    [ForeignKey("EstadoId")]
    [InverseProperty("Mesas")]
    public virtual EstadoMesa Estado { get; set; } = null!;

    [InverseProperty("Mesa")]
    public virtual ICollection<ReservacionMesa> ReservacionMesas { get; set; } = new List<ReservacionMesa>();
}
