using System.ComponentModel.DataAnnotations;

namespace FreelanceBirga.Models.VM
{
    public class OrderViewModel
    {
        [Required(ErrorMessage = "Поле 'Заголовок' обязательно для заполнения")]
        [StringLength(20, ErrorMessage = "Заголовок не может быть длиннее 20 символов")]
        [Display(Name = "Заголовок")]
        public string Title { get; set; }

        [Display(Name = "Описание")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Поле 'Цена' обязательно для заполнения")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Цена должна быть больше 0")]
        [DataType(DataType.Currency)]
        [Display(Name = "Цена")]
        public decimal Price { get; set; }
    }
}
