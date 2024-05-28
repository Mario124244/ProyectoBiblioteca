namespace BibliotecaP.Models.DTOs
{
    public class ReseñaDTO
    {
        public int UsuarioID { get; set; }
        public int IdReseña { get; set; }
        public string Descripcion { get; set; }
        public int? Calificacion { get; set; }
    }
}
