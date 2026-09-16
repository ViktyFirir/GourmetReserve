using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GourmetReserve.Models
{
    public class Reservation
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Укажите имя гостя")]
        [Display(Name = "Имя гостя")]
        [StringLength(150, MinimumLength = 2)]
        public string GuestName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Укажите телефон")]
        [Display(Name = "Телефон")]
        [Phone(ErrorMessage = "Некорректный формат телефона")]
        [StringLength(30)]
        public string Phone { get; set; } = string.Empty;

        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "Некорректный формат Email")]
        [StringLength(150)]
        public string? Email { get; set; }

        [Required]
        [Display(Name = "Стол")]
        [ForeignKey(nameof(Table))]
        public int TableId { get; set; }
        public Table? Table { get; set; }

        [Required(ErrorMessage = "Укажите дату и время брони")]
        [Display(Name = "Дата и время бронирования")]
        [DataType(DataType.DateTime)]
        public DateTime ReservationDateTime { get; set; }

        [Required(ErrorMessage = "Укажите количество гостей")]
        [Display(Name = "Количество гостей")]
        [Range(1, 20, ErrorMessage = "Количество гостей должно быть от 1 до 20")]
        public int GuestsCount { get; set; }

        [Display(Name = "Особые пожелания")]
        [StringLength(500)]
        public string? SpecialRequests { get; set; }

        [Display(Name = "Дата создания брони")]
        public DateTime CreatedAt { get; set; } = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
    }
}
