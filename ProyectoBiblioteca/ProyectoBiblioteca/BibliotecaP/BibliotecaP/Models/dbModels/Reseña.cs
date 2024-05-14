using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaP.Models.dbModels;

public partial class Reseña
{
    [Key]
    public int IdReseña { get; set; }

    [Column("UsuarioID")]
    public int UsuarioId { get; set; }

    [StringLength(200)]
    public string Descripcion { get; set; } = null!;

    [Column("calificacion")]
    public int? Calificacion { get; set; }

    [ForeignKey("UsuarioId")]
    [InverseProperty("Reseñas")]
    public virtual Usuario Usuario { get; set; } = null!;
}
