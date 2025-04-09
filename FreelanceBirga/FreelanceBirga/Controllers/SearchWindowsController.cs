using FreelanceBirga.Models.VM;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FreelanceBirga.Controllers
{
    public class SearchWindowsController : Controller
    {
        private readonly AppDbContext _context;

        public SearchWindowsController(AppDbContext context)
        {
            _context = context;
        }
       
        [HttpGet]
        public IActionResult SearchExecutorWindow()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return RedirectToAction("Autorization", "Account");
            }
            var tags = _context.Tags.Select(t => new TagViewModel
            {
                Id = t.Id,
                Name = t.Value
            }).ToList();

            return View(tags);
        }

        [HttpPost]
        public IActionResult ProcessSelectedTags([FromForm] List<int> selectedTags)
        {
            Console.WriteLine("Выбранные теги:");
            foreach (var tag in selectedTags)
            {
                Console.WriteLine(tag);
            }

            return SearchExecutorWindow();
        }
    }
}
