using APIRESTfulNUXIBA.Entities;

namespace APIRESTfulNUXIBA.DTOs
{
    public class LoginDTO
    {
        public int logId {  get; set; }
        public int User_id { get; set; }
        public int Extension { get; set; }
        public int TipoMov { get; set; }
        public DateTime fecha { get; set; }

        public static implicit operator LoginDTO?(Login? v)
        {
            throw new NotImplementedException();
        }
    }
}
