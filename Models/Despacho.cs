namespace TP_MVC_CRUD.Models
{
    public class Despacho
    {
        public int DespachoId { get; set; }
        public DateTime Fecha { get; set; }
        public int ClienteId { get; set; }
        public int UsuarioId { get; set; }
        public bool Confirmado { get; set; }

        public Cliente Cliente { get; set; }
        public Usuario Usuario { get; set; }
        public ICollection<DespachoDetalle> Detalles { get; set; }
    }
}
