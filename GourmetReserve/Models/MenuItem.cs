using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GourmetReserve.Models
{
    public class MenuItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Категория")]
        [ForeignKey(nameof(MenuCategory))]
        public int MenuCategoryId { get; set; }
        public MenuCategory? MenuCategory { get; set; }

        [Required(ErrorMessage = "Укажите название блюда")]
        [Display(Name = "Название")]
        [StringLength(150)]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Описание (состав)")]
        [StringLength(1000)]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Укажите цену")]
        [Display(Name = "Цена")]
        [Column(TypeName = "decimal(10,2)")]
        [Range(0.01, 100000, ErrorMessage = "Цена должна быть больше 0")]
        public decimal Price { get; set; }

        [Display(Name = "Вес / Объем")]
        [StringLength(50)]
        public string? WeightOrVolume { get; set; } // например "250 г" или "0.3 л"

        [Display(Name = "Изображение")]
        [StringLength(255)]
        public string? ImageFileName { get; set; }

        [Display(Name = "Острое")]
        public bool IsSpicy { get; set; }

        [Display(Name = "Вегетарианское")]
        public bool IsVegetarian { get; set; }

        [Display(Name = "Выбор шефа")]
        public bool IsChefChoice { get; set; }
    }
}
