using System.ComponentModel.DataAnnotations;

namespace GourmetReserve.Models
{
    public class MenuCategory
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Укажите название категории")]
        [Display(Name = "Название категории")]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty; // Закуски, Горячее, Десерты, Напитки

        [Display(Name = "Иконка / CSS-класс")]
        [StringLength(100)]
        public string? IconCssClass { get; set; } // например "bi bi-cup-hot"

        [Display(Name = "Порядок сортировки")]
        public int SortOrder { get; set; }

        // Навигационное свойство: одна категория — много блюд
        public ICollection<MenuItem>? MenuItems { get; set; }
    }
}
