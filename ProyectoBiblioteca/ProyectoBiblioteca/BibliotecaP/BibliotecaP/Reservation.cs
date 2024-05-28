namespace BibliotecaP.Services
{
    public class Reservation
    {
        public interface IReservationService
        {
            Task<IEnumerable<Reservation>> GetUserReservations(int userId);
            // Otros métodos de la interfaz
        }
    }
}