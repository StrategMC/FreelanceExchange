using Microsoft.AspNetCore.Mvc;
using System.Net.NetworkInformation;
using System.Security.Claims;
using FreelanceBirga.Models.VM;
using FreelanceBirga.Models.DB;

namespace FreelanceBirga.Controllers
{
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;
        int? userId;
        public AdminController(AppDbContext context)
        {
            _context = context;
        }
        [HttpPost]
        public IActionResult RejectOrder(int id)
        {
            userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return RedirectToAction("Index", "Home");
            }
            if (!User.HasClaim(ClaimTypes.Role, "Admin"))
            {
                return RedirectToAction("MainPage", "MainPages");
            }
            var currentlyOrder = _context.TempOrders.FirstOrDefault(o => o.Id == id);
            if (currentlyOrder == null)
            {
                Console.WriteLine("Ошибка. Нет такого заказа");
                return RedirectToAction("MainPage", "MainPages");
            }
            _context.TempOrders.Remove(currentlyOrder);
            _context.SaveChanges();
            return RedirectToAction("OrderModeration", "Admin");

        }
        [HttpPost]
        public IActionResult ApproveOrder(int id)
        {
            userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return RedirectToAction("Index", "Home");
            }
            if (!User.HasClaim(ClaimTypes.Role, "Admin"))
            {
                return RedirectToAction("MainPage", "MainPages");
            }
            var currentlyOrder = _context.TempOrders.FirstOrDefault(o => o.Id == id);
            if (currentlyOrder == null)
            {
                Console.WriteLine("Ошибка. Нет такого заказа");
                return RedirectToAction("MainPage", "MainPages");
            }
            Order order = new Order
            {
                CustomerID = currentlyOrder.CustomerID,
                ExecutorID = null,
                Description = currentlyOrder.Description,
                Price = currentlyOrder.Price,
                Title = currentlyOrder.Title,
                InWork = false

            };
            _context.Orders.Add(order);
            _context.TempOrders.Remove(currentlyOrder);
            _context.SaveChanges();
            return RedirectToAction("OrderModeration", "Admin");
        }
        [HttpGet]
        public IActionResult OrderModeration()
        {
            userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return RedirectToAction("Index", "Home");
            }
            if (!User.HasClaim(ClaimTypes.Role, "Admin"))
            {
                return RedirectToAction("MainPage", "MainPages");
            }
            var pendingOrders = _context.TempOrders.Select(ut => ut).ToList();

          
            var model = new OrdersModerationViewModel
            {
                Orders = pendingOrders
            };
            return View(model);
        }
    }
}
