namespace FreelanceBirga.Models.VM
{
    public class ReviewViewModel
    {
       public int ChatId { get; set; }
       public bool IsCustomer { get; set; }
       public string SendlerName { get; set; }
       public string OrderName { get; set; }
       public int Mark { get; set; }
       public string Description { get; set; }
    }
}
