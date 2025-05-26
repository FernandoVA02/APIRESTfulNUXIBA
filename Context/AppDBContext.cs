using APIRESTfulNUXIBA.Entities;
using Microsoft.EntityFrameworkCore;

namespace APIRESTfulNUXIBA.Context
{
    public class AppDBContext : DbContext
    {
        public AppDBContext(DbContextOptions<AppDBContext> options) : base(options) { }

        public DbSet<User> ccUsers { get; set; }
        public DbSet<Area> ccRIACat_Areas { get; set; }
        public DbSet<Login> ccloglogin { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // User
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("ccUsers");

                entity.HasKey(u => u.User_id);

                entity.Property(u => u.User_id)
                    .ValueGeneratedOnAdd();

                entity.Property(u => u.Login)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(u => u.Nombre)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(u => u.ApellidoPaterno)
                    .HasMaxLength(50);

                entity.Property(u => u.ApellidoMaterno)
                    .HasMaxLength(50);

                entity.Property(u => u.Password)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(u => u.TipoUser_id)
                    .IsRequired();

                entity.Property(u => u.Status)
                    .IsRequired();

                entity.Property(u => u.fCreate)
                    .IsRequired();

                entity.Property(u => u.IDArea)
                    .IsRequired();

                entity.Property(u => u.LastLoginAttempt)
                    .IsRequired();

                entity.HasOne(u => u.Area)
                    .WithMany(a => a.Users)
                    .HasForeignKey(u => u.IDArea)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Area
            modelBuilder.Entity<Area>(entity =>
            {
                entity.ToTable("ccRIACat_Areas");

                entity.HasKey(a => a.IDArea);

                entity.Property(a => a.IDArea)
                    .IsRequired();

                entity.Property(a => a.AreaName)
                    .IsRequired()
                    .HasMaxLength(15);

                entity.Property(a => a.StatusArea)
                    .IsRequired();

                entity.Property(a => a.CreateDate)
                    .IsRequired();
            });

            // Login
            modelBuilder.Entity<Login>(entity =>
            {
                entity.ToTable("ccloglogin");

                entity.HasKey(l => l.logId);

                // Autogenerar el LogId
                entity.Property(l => l.logId)
                    .IsRequired()
                    .ValueGeneratedOnAdd();

                // Campos requeridos
                entity.Property(l => l.User_id).IsRequired();
                entity.Property(l => l.Extension).IsRequired();
                entity.Property(l => l.TipoMov).IsRequired();
                entity.Property(l => l.fecha).IsRequired();

                // Relación con ccUsers (User)
                entity.HasOne(l => l.User)
                    .WithMany(u => u.Logins)
                    .HasForeignKey(l => l.User_id)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
