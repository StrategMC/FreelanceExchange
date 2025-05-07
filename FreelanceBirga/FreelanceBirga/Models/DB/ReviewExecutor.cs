using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace FreelanceBirga.Models.DB
{
    public class ReviewExecutor
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int SenderId { get; set; }   
        public int RecipientId { get; set; }
        public int OrderId { get; set; }
        public int Mark {  get; set; }  
        public string Content {  get; set; }
    }
}
