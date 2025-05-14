namespace FreelanceBirga.Models.VM
{
    public class ChatViewModel
    {
        public int ChatId { get; set; }
        public int OrderId { get; set; }
        public string OrderName {  get; set; }
        public int CurrentUserId { get; set; }
        public bool IsCustomer { get; set; }
        public int Status { get; set; }
        public List<MessageViewModel> Messages { get; set; }
        public bool ShowReviewButton { get; set; }
        public List<ReviewForProfileViewModel> Review {  get; set; }
    }
}
