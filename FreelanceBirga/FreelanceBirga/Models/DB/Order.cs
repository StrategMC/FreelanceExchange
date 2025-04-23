using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace FreelanceBirga.Models.DB
{
    public class Order
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int CustomerID { get; set; }
        public int? ExecutorID { get; set; }
        public string Description { get; set; }
        public int Price { get; set; }
        public string Title { get; set; }
        public bool InWork { get; set; }
      
    }
}
