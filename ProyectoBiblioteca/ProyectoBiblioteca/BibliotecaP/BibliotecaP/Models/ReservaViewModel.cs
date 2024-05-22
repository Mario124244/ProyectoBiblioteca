namespace BibliotecaP.Models
{
    public class ReservaViewModel
    {
        public string UsuarioNombre { get; set; }
        public string CubiculoNombre { get; set; }
        public DateTime HoraEntrada { get; set; }
        public DateTime HoraSalida { get; set; }
        public string QRDataAcceso { get; set; }
        public string QRDataSalida { get; set; }
    }
}
