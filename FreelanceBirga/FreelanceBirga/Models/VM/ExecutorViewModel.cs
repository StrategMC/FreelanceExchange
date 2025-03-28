using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace FreelanceBirga.Models.VM
{
    public class ExecutorViewModel
    {
        [Required(ErrorMessage = "Имя пользовател")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Имя пользователя должно быть от 3 до 50 символов")]
        public string Username { get; set; }
        public string Description { get; set; }
        public List<string> Tags { get; set; } = new List<string>();

    }
}
