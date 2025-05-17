using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
namespace FreelanceBirga.Models.DB
{
    public class OrderChatForRewiew
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int OrderChatId { get; set; }
        public bool ExecutorReview { get; set; } = false;
        public bool CustomerReview { get; set; } = false;
    }
}
