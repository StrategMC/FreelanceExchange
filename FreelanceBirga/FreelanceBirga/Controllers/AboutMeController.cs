using FreelanceBirga.Models.DB;
using FreelanceBirga.Models.VM;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.HttpSys;
using Microsoft.EntityFrameworkCore;

namespace FreelanceBirga.Controllers
{
    public class AboutMeController : Controller
    {
        private readonly AppDbContext _context;

        public AboutMeController(AppDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<IActionResult> AboutMeCustomer()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return RedirectToAction("Autorization", "Account");
            }
            
            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.UserID == userId.Value);

            if (customer == null)
            {
                return NotFound();
            }
            
            return View(customer);
        }

        [HttpGet]
        public async Task<IActionResult> AboutMeExecutor()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return RedirectToAction("Autorization", "Account");
            }
            
            var executor = await _context.Executors
                .FirstOrDefaultAsync(c => c.UserID == userId.Value);

            if (executor == null)
            {
                return NotFound();
            }
            var model = new ExecutorViewModel();
           
            List<int> tagsId = await _context.ExecutorsTag.Where(ut => ut.UserID == userId).Select(ut=>ut.TagID).ToListAsync();
            List<string> tagsValue = await _context.Tags.Where(t => tagsId.Contains(t.Id)).Select(t => t.Value).ToListAsync();
            model.Username = executor.Username;
            model.Description = executor.Description;
            model.Tags= tagsValue;
            return View(model);
        }
    }
}
