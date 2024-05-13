using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace ProyectoBiblioteca.Models.dbModels
{
    public class AplicationUser : IdentityUser<int>
    {
        

        

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
