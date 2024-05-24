using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BibliotecaP.Models.DTO.Cubiculo
{
    public class CubiculoDTO
    {
        [Required]
        public int CubiculoId { get; set; }

        [Required]
        [StringLength(50)]
        public string Nombre { get; set; } = null!;
        
        [Required]
        public int EstadoId { get; set; }

        [Required]
        public string EstadoNombre { get; set; } = null!;
    }
}

//Solucionado por el inge lepe
