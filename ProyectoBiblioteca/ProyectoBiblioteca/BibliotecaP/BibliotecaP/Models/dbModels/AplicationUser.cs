using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace BibliotecaP.Models.dbModels
{
    public class AplicationUser :   IdentityUser<int>
    {
        [Key]
        [Column("UsuarioID")]
        public int UsuarioId { get; set; }

        [StringLength(50)]
        

        [Column("RolID")]
        public int RolId { get; set; }

        [InverseProperty("Usuario")]
        public virtual ICollection<Aviso> Avisos { get; set; } = new List<Aviso>();

        [InverseProperty("Usuario")]
        public virtual ICollection<ReservacionCubiculo> ReservacionCubiculos { get; set; } = new List<ReservacionCubiculo>();

        [InverseProperty("Usuario")]
        public virtual ICollection<ReservacionMesa> ReservacionMesas { get; set; } = new List<ReservacionMesa>();

        [InverseProperty("Usuario")]
        public virtual ICollection<Reseña> Reseñas { get; set; } = new List<Reseña>();

       
        
    }
}
