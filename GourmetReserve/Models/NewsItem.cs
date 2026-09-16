using System.ComponentModel.DataAnnotations;

namespace GourmetReserve.Models
{
    public class NewsItem
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Укажите заголовок")]
        [Display(Name = "Заголовок")]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Display(Name = "Краткое описание")]
        [StringLength(300)]
        public string? ShortDescription { get; set; }

        [Display(Name = "Текст новости")]
        public string? FullText { get; set; }

        [Required]
        [Display(Name = "Дата публикации")]
        [DataType(DataType.Date)]
        public DateTime PublishedAt { get; set; } = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);

        [Display(Name = "Изображение")]
        [StringLength(255)]
        public string? ImageFileName { get; set; }

        [Display(Name = "Актуально")]
        public bool IsActive { get; set; } = true;
    }
}
