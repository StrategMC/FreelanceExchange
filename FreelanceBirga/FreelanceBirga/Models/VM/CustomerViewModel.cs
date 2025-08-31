using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace FreelanceBirga.Models.VM
{
    public class CustomerViewModel
    {
        [Required(ErrorMessage = "Имя пользовател")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Имя пользователя должно быть от 3 до 50 символов")]
        public string Username { get; set; }
        public string Description { get; set; }
        public int Rating { get; set; } = 0;
        public int ColRating { get; set; } = 0;
        public List<ReviewForProfileViewModel> Reviews { get; set; }
        public int Money { get; set; } = 0;
        public int OnHoldMoney { get; set; } = 0;
    }
}
