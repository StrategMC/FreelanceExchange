using FreelanceBirga.Models.DB;
using FreelanceBirga.Models.VM;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FreelanceBirga.Controllers
{
    public class ChatsController : Controller
    {
        private readonly AppDbContext _context;
        int? userId;
        public ChatsController(AppDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<IActionResult> ChatsWindow()
        {
            userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return RedirectToAction("Index", "Home");
            }
            bool executor = await _context.Executors.AnyAsync(ut => ut.UserID == userId);
            bool customer = await _context.Customers.AnyAsync(ut => ut.UserID == userId);
            if (customer == false && executor == false)
            {
                return RedirectToAction("MainPage", "MainPages");
            }
            List<ChatWindowViewModelUnit> model = new List<ChatWindowViewModelUnit>();
            List<OrdersChat> ordersChats;
            if (executor == true)
            {
                int executorId = await _context.Executors.Where(ut => ut.UserID == userId).Select(ut => ut.Id).FirstAsync();
                ordersChats = await _context.OrdersChat.Where(od => od.ExecutorId == executorId).ToListAsync();
            }
            else
            {
                int customerId = await _context.Customers.Where(ut => ut.UserID == userId).Select(ut => ut.Id).FirstAsync();
                ordersChats = await _context.OrdersChat.Where(od => od.CustomerId == customerId).ToListAsync();
            }
            for (int i = 0; i < ordersChats.Count(); i++)
            {
                var orderTemp = await _context.Orders.FindAsync(ordersChats[i].OrderId);
                var executorTemp = await _context.Executors.FindAsync(ordersChats[i].ExecutorId);
                var customerTemp = await _context.Customers.FindAsync(ordersChats[i].CustomerId);
                string status;
                if (executor == true)
                {
                    switch (ordersChats[i].Status)
                    {

                        case 0:
                            status = "не отдан мне";
                            break;
                        case 1:
                            status = "отдан мне";
                            break;
                        case 2:
                            status = "завершён";
                            break;
                        case 3:
                            status = "отказано";
                            break;
                        default:
                            status = "Defolt :(";
                            break;
                    }
                }
                else
                {
                    switch (ordersChats[i].Status)
                    {

                        case 0:
                            status = "не выполняет";
                            break;
                        case 1:
                            status = "выполняет";
                            break;
                        case 2:
                            status = "завершён";
                            break;
                        case 3:
                            status = "отказано";
                            break;
                        default:
                            status = "Defolt :(";
                            break;
                    }
                }
                ChatWindowViewModelUnit chatWindowViewModelUnit = new ChatWindowViewModelUnit
                {
                    OrderName = orderTemp.Title,
                    ExecutorName = executorTemp.Username,
                    CustomerName = customerTemp.Username,
                    Status = status
                };
                model.Add(chatWindowViewModelUnit);
            }
            return View(model);
        }
    }
}
