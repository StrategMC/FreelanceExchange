using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace FreelanceBirga.Models.DB
{
    public class TempOrder
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int CustomerID { get; set; }
        public string Description { get; set; }
        public int Price { get; set; }
        public string Title { get; set; }
    }
}
