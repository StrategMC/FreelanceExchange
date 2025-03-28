using System.ComponentModel.DataAnnotations;

namespace FreelanceBirga.Models
{
    public class AutorizationViewModel
    {

        [Required(ErrorMessage = "Введите логин")]
        [StringLength(50, MinimumLength = 3)]
        public string Login { get; set; }

        [Required(ErrorMessage = "Введите пароль")]
        [DataType(DataType.Password)]
        [MinLength(6)]
        public string Password { get; set; }
    }
}
