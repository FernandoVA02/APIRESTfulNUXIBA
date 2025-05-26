namespace APIRESTfulNUXIBA.Entities
{
    public class Login
    {
        public int logId { get; set; }
        public int User_id { get; set; }
        public int Extension { get; set; }
        public int TipoMov { get; set; }
        public DateTime fecha { get; set; }

        // Relación
        public virtual User? User { get; set; }
    }
}
