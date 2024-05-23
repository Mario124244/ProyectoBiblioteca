namespace BibliotecaP.ViewModels
{
    public class ReservaViewModels
    {
        public string Nombre { get; set; }
        public DateTime HoraEntrada { get; set; }
        public DateTime HoraSalida { get; set; }
        public string AccesoQrBase64 { get; set; }
        public string SalidaQrBase64 { get; set; }
    }
}
