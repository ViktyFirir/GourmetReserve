using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GourmetReserve.Models
{
    public enum TableLocation
    {
        [Display(Name = "У окна")]
        Window,
        [Display(Name = "Терраса")]
        Terrace,
        [Display(Name = "VIP-зона")]
        Vip,
        [Display(Name = "Общий зал")]
        MainHall
    }

    public enum TableStatus
    {
        [Display(Name = "Доступен")]
        Available,
        [Display(Name = "На обслуживании")]
        Maintenance,
        [Display(Name = "Недоступен")]
        Disabled
    }

    public class Table
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Укажите номер стола")]
        [Display(Name = "Номер стола")]
        [Range(1, 999, ErrorMessage = "Номер стола должен быть от 1 до 999")]
        public int TableNumber { get; set; }

        [Required(ErrorMessage = "Укажите вместимость")]
        [Display(Name = "Вместимость (мест)")]
        [Range(1, 20, ErrorMessage = "Вместимость должна быть от 1 до 20 мест")]
        public int Capacity { get; set; }

        [Required]
        [Display(Name = "Расположение")]
        public TableLocation Location { get; set; }

        [Required]
        [Display(Name = "Статус доступности")]
        public TableStatus Status { get; set; } = TableStatus.Available;

        // Позиция в сетке зала (для CSS Grid)
        [Display(Name = "Позиция: ряд")]
        public int GridRow { get; set; }

        [Display(Name = "Позиция: колонка")]
        public int GridColumn { get; set; }

        // Навигационное свойство: один стол — много броней
        public ICollection<Reservation>? Reservations { get; set; }
    }
}
