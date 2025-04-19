using Microsoft.AspNetCore.Mvc;
using System.Net.NetworkInformation;
using System.Security.Claims;
using FreelanceBirga.Models.VM;

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
