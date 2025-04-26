using FreelanceBirga.Models.DB;
using Microsoft.AspNetCore.Mvc;

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
        public IActionResult ChatsWindow()
        {
            userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return RedirectToAction("Index", "Home");
            }
            List<OrdersChat> model = new List<OrdersChat>();
            model = _context.OrdersChat.Select(ut=>ut).ToList();
            return View(model);
        }
    }
}
