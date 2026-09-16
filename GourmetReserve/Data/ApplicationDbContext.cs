using GourmetReserve.Models;
using Microsoft.EntityFrameworkCore;

namespace GourmetReserve.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Table> Tables { get; set; } = null!;
        public DbSet<Reservation> Reservations { get; set; } = null!;
        public DbSet<MenuCategory> MenuCategories { get; set; } = null!;
        public DbSet<MenuItem> MenuItems { get; set; } = null!;
        public DbSet<NewsItem> NewsItems { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ---------- Table -> Reservation (один-ко-многим) ----------
            // Ограничиваем каскадное удаление: если стол удаляют, брони не должны
            // удаляться автоматически (иначе теряем историю). Используем Restrict.
            modelBuilder.Entity<Reservation>()
                .HasOne(r => r.Table)
                .WithMany(t => t.Reservations)
                .HasForeignKey(r => r.TableId)
                .OnDelete(DeleteBehavior.Restrict);

            // ---------- MenuCategory -> MenuItem (один-ко-многим) ----------
            // Здесь каскадное удаление уместно: если категория удаляется,
            // удаляем и её блюда.
            modelBuilder.Entity<MenuItem>()
                .HasOne(mi => mi.MenuCategory)
                .WithMany(mc => mc.MenuItems)
                .HasForeignKey(mi => mi.MenuCategoryId)
                .OnDelete(DeleteBehavior.Cascade);

            // ---------- Индексы для ускорения поиска пересечений по времени ----------
            modelBuilder.Entity<Reservation>()
                .HasIndex(r => new { r.TableId, r.ReservationDateTime });

            modelBuilder.Entity<Table>()
                .HasIndex(t => t.TableNumber)
                .IsUnique();

            // ---------- Enum -> string маппинг (читаемо в БД PostgreSQL) ----------
            modelBuilder.Entity<Table>()
                .Property(t => t.Location)
                .HasConversion<string>()
                .HasMaxLength(30);

            modelBuilder.Entity<Table>()
                .Property(t => t.Status)
                .HasConversion<string>()
                .HasMaxLength(30);

            // ---------- Даты без таймзоны ----------
            // Нам не нужна таймзона (ресторан работает по одному местному времени),
            // а Npgsql по умолчанию мапит DateTime на "timestamp with time zone"
            // и требует Kind=Utc, иначе падает с ArgumentException. Явно указываем
            // "timestamp without time zone", чтобы принимать обычный DateTime
            // (Kind=Unspecified) из форм без лишних конвертаций в UTC.
            modelBuilder.Entity<Reservation>()
                .Property(r => r.ReservationDateTime)
                .HasColumnType("timestamp without time zone");

            modelBuilder.Entity<Reservation>()
                .Property(r => r.CreatedAt)
                .HasColumnType("timestamp without time zone");

            modelBuilder.Entity<NewsItem>()
                .Property(n => n.PublishedAt)
                .HasColumnType("timestamp without time zone");

            // ---------- Значения по умолчанию ----------
            modelBuilder.Entity<Reservation>()
                .Property(r => r.CreatedAt)
                .HasDefaultValueSql("now()");

            modelBuilder.Entity<NewsItem>()
                .Property(n => n.PublishedAt)
                .HasDefaultValueSql("now()");
        }
    }
}
