namespace APIRESTfulNUXIBA.Entities
{
    public class Area
    {
        public int IDArea { get; set; }
        public string AreaName { get; set; } = null!;
        public int StatusArea { get; set; }
        public DateTime CreateDate { get; set; }

        // Relacion inversa
        public virtual ICollection<User>? Users { get; set; }
    }
}
