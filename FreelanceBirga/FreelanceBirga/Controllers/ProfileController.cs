using FreelanceBirga.Models.DB;
using FreelanceBirga.Models.VM;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.HttpSys;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace FreelanceBirga.Controllers
{
    public class ProfileController : Controller
    {
        private readonly AppDbContext _context;
        int? userId;
        public ProfileController(AppDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public IActionResult ExecutorProfile(int id)
        {
            userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return RedirectToAction("Index", "Home");
            }
            var executorWithTags = _context.Executors
                .Where(e => e.UserID == id)
                .Select(e => new
                {
                    Executor = e,
                    Tags = _context.ExecutorsTag
                        .Where(et => et.UserID == e.UserID)
                        .Join(_context.Tags,
                            et => et.TagID,
                            t => t.Id,
                            (et, t) => t.Value)
                        .ToList()
                })
                .FirstOrDefault();

            if (executorWithTags == null)
            {
                return NotFound();
            }

            var model = new ExecutorViewModel
            {
                Id = executorWithTags.Executor.Id,
                UserId = executorWithTags.Executor.UserID,
                Username = executorWithTags.Executor.Username,
                Description = executorWithTags.Executor.Description,
                Rating = executorWithTags.Executor.Rating,
                ColRating = executorWithTags.Executor.ColRating,
                Tags = executorWithTags.Tags
            };

            return View(model);
        }
    }
}
