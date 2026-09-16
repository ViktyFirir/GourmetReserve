using System.ComponentModel.DataAnnotations;

namespace GourmetReserve.Models
{
    public class BookingViewModel
    {
        [Required(ErrorMessage = "Выберите стол")]
        [Display(Name = "Стол")]
        public int TableId { get; set; }

        [Required(ErrorMessage = "Укажите дату")]
        [DataType(DataType.Date)]
        [Display(Name = "Дата")]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "Укажите время")]
        [DataType(DataType.Time)]
        [Display(Name = "Время")]
        public TimeSpan Time { get; set; }

        [Required(ErrorMessage = "Укажите имя")]
        [StringLength(150, MinimumLength = 2)]
        [Display(Name = "Имя гостя")]
        public string GuestName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Укажите телефон")]
        [Phone]
        [Display(Name = "Телефон")]
        public string Phone { get; set; } = string.Empty;

        [EmailAddress]
        [Display(Name = "Email")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Укажите количество гостей")]
        [Range(1, 20)]
        [Display(Name = "Количество гостей")]
        public int GuestsCount { get; set; }

        [StringLength(500)]
        [Display(Name = "Особые пожелания")]
        public string? SpecialRequests { get; set; }

        // Удобное вычисляемое свойство для объединения даты и времени
        public DateTime GetReservationDateTime() => Date.Date + Time;
    }
}
