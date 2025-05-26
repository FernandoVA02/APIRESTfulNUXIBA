namespace APIRESTfulNUXIBA.Entities
{
    public class User
    {
        public int User_id { get; set; }
        public string Login { get; set; } = null!;
        public string Nombre { get; set; } = null!;
        public string? ApellidoPaterno { get; set; }
        public string? ApellidoMaterno { get; set; }
        public string Password { get; set; } = null!;
        public int TipoUser_id { get; set; }
        public int Status { get; set; }
        public DateTime fCreate { get; set; }
        public int IDArea { get; set; }
        public DateTime LastLoginAttempt { get; set; }

        // Relaciones
        public virtual Area? Area { get; set; }
        public virtual ICollection<Login>? Logins { get; set; }
    }
}
