using FreelanceBirga.Models.DB;

namespace FreelanceBirga.Models.VM
{
    public class MyOrderViewModel
    {
        public List<Order> orders = new List<Order>();
        public List<TempOrder> tempOrders = new List<TempOrder>();
    }
}
