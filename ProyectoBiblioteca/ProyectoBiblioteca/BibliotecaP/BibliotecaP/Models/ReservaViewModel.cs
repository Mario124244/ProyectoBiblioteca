namespace BibliotecaP.Models
{
    public class ReservacionViewModel
    {
        public int ReservacionId { get; set; }
        public int CubiculoId { get; set; }
        public string UsuarioEmail { get; set; }  // Añadir esta propiedad
        public DateTime FechaHoraInicio { get; set; }
        public DateTime FechaHoraFin { get; set; }
    }

}
