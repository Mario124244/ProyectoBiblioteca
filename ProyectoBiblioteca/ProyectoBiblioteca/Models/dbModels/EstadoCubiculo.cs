using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ProyectoBiblioteca.Models.dbModels;

public partial class EstadoCubiculo
{
    [Key]
    [Column("EstadoID")]
    public int EstadoId { get; set; }

    [StringLength(50)]
    public string Nombre { get; set; } = null!;

    [InverseProperty("Estado")]
    public virtual ICollection<Cubiculo> Cubiculos { get; set; } = new List<Cubiculo>();
}
