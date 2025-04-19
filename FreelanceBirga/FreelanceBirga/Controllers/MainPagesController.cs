using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FreelanceBirga.Controllers
{
    public class MainPagesController : Controller
    {
        private readonly AppDbContext _context;

        public MainPagesController(AppDbContext context)
        {
            _context = context;
        }
        [HttpPost]
        public  IActionResult OpenExexutorRegestration(string role)
        {
                if (role == "Executor")
                {
                    return RedirectToAction("Executor", "AddRole");
                }
                else if (role == "Customer")
                {
                    return RedirectToAction("Customer", "AddRole");
                }

                return RedirectToAction("MainPage");

        }
        [HttpGet]
        public async Task<IActionResult> MainPage()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return RedirectToAction("Index", "Home");
            }
            User user = GetCurrentUser();
            ViewBag.CustomerRole = false;
            ViewBag.ExecutorRole = false;
           
                if (await _context.Executors.AnyAsync(e => e.UserID == user.Id))
                {
                    ViewBag.ExecutorRole = true;
                }
                if (await _context.Customers.AnyAsync(e => e.UserID == user.Id))
                {
                    ViewBag.CustomerRole = true;
                }


            return View();
        }

        
        private User GetCurrentUser()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId.HasValue)
            {
                return _context.Users.FirstOrDefault(u => u.Id == userId.Value);
            }
            return null;
        }


    }

}
