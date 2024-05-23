namespace BibliotecaP.Controllers
{
    internal class ReservaViewModel
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string HoraEntrada { get; set; }
        public string HoraSalida { get; set; }
        public string AccesoQrBase64 { get; set; }
        public string SalidaQrBase64 { get; set; }
    }
}