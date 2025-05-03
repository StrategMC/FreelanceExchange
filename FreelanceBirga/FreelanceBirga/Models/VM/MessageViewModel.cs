namespace FreelanceBirga.Models.VM
{
    public class MessageViewModel
    {
        public string Content { get; set; }
        public bool Sender { get; set; } 
        public DateTime SendTime { get; set; }
        public bool IsCurrentUser { get; set; }
    }
}
