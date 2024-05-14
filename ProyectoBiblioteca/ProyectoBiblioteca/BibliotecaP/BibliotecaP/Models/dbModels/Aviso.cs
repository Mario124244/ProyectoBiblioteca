using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaP.Models.dbModels;

public partial class Aviso
{
    [Key]
    [Column("AvisosID")]
    public int AvisosId { get; set; }

    [StringLength(50)]
    public string Titulo { get; set; } = null!;

    [StringLength(400)]
    public string Descripcin { get; set; } = null!;

    [StringLength(500)]
    public string? Imagen { get; set; }

    [Column("UsuarioID")]
    public int UsuarioId { get; set; }

    [ForeignKey("UsuarioId")]
    [InverseProperty("Avisos")]
    public virtual Usuario Usuario { get; set; } = null!;
}
