using FreelanceBirga.Models.DB;
using FreelanceBirga.Models.VM;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FreelanceBirga.Controllers
{
    public class MoneyController : Controller
    {
        private readonly AppDbContext _context;

        public MoneyController(AppDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public IActionResult PutMoney()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return RedirectToAction("Index", "Home");
            }
            MoneyViewModel model = new MoneyViewModel();
            return View(model);
        }
        [HttpGet]
        public IActionResult WithdrawMoney()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return RedirectToAction("Index", "Home");
            }
            MoneyViewModel model = new MoneyViewModel();
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> PutMoney(MoneyViewModel model)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return RedirectToAction("Index", "Home");
            }
            var customer = await _context.Customers.Where(cm => cm.UserID == userId).FirstAsync();
            Console.WriteLine($"Before update: {customer.Money}");
            customer.Money += model.Money;
            Console.WriteLine($"After update: {customer.Money}");
           // _context.Customers.Update(customer);
            await _context.SaveChangesAsync();
            return RedirectToAction("AboutMeCustomer", "AboutMe");
        }
        [HttpPost]
        public async Task<IActionResult> WithdrawMoney(MoneyViewModel model)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return RedirectToAction("Index", "Home");
            }
            var executor = await _context.Executors.Where(ex => ex.UserID == userId).FirstAsync();
            if(executor.Money < model.Money)
            {
                ModelState.AddModelError("", "Недостаточно средств на балансе");
                return View(model);
            }
            executor.Money -= model.Money;
            await _context.SaveChangesAsync();
            return RedirectToAction("AboutMeExecutor", "AboutMe");
        }
    }
}
