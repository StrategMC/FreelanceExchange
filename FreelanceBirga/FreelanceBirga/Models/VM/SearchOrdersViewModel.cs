using FreelanceBirga.Models.DB;

namespace FreelanceBirga.Models.VM
{
    public class SearchOrdersViewModel
    {
        public string SearchWord {  get; set; }
        public bool InDescription {  get; set; }
        public List<Order> FilteredOrders { get; set; }

        public int MinPrice { get; set; } = 0;
    }
}
