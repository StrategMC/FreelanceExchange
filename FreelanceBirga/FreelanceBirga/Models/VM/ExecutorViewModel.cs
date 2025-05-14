
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    namespace FreelanceBirga.Models.VM
    {
        public class ExecutorViewModel
        {
            public int Id { get; set; } 

            [Required(ErrorMessage = "Имя пользователя обязательно")]
            [StringLength(50, MinimumLength = 3, ErrorMessage = "Имя пользователя должно быть от 3 до 50 символов")]
            public string Username { get; set; }

            public string Description { get; set; }
            public List<string> Tags { get; set; } = new List<string>();
            public int Rating { get; set; } = 0;  
            public int ColRating { get; set; } = 0;  
            public int UserId { get; set; }
            public List<ReviewForProfileViewModel> Review { get; set; }
        }
    }

