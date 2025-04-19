using FreelanceBirga.Models.VM;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FreelanceBirga.Controllers
{
    public class SearchWindowsController : Controller
    {
        private readonly AppDbContext _context;
        int? userId;
        public SearchWindowsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult SearchExecutorWindow()
        {
            userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return RedirectToAction("Index", "Home");
            }
            var model = new SearchViewModel
            {
                AllTags = _context.Tags
                    .Select(t => new TagViewModel { Id = t.Id, Name = t.Value })
                    .ToList(),
                FilteredExecutors = GetExecutorsWithTags()
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult ProcessSelectedTags([FromForm] List<int> selectedTags)
        {
            var model = new SearchViewModel
            {
                AllTags = _context.Tags
                    .Select(t => new TagViewModel { Id = t.Id, Name = t.Value })
                    .ToList(),
                FilteredExecutors = GetExecutorsWithTags(selectedTags),
                SelectedTagIds = selectedTags
            };

            return View("SearchExecutorWindow", model);
        }

        private List<ExecutorViewModel> GetExecutorsWithTags(List<int>? selectedTagIds = null)
        {
            var executorsQuery = _context.Executors.AsQueryable();

            if (selectedTagIds != null && selectedTagIds.Any())
            {
                var executorIdsWithTags = _context.ExecutorsTag
                    .Where(et => selectedTagIds.Contains(et.TagID))
                    .GroupBy(et => et.UserID)
                    .Where(g => g.Count() == selectedTagIds.Count)
                    .Select(g => g.Key)
                    .ToList();

                executorsQuery = executorsQuery.Where(e => executorIdsWithTags.Contains(e.UserID));
            }

            var executors = executorsQuery
                .Select(e => new ExecutorViewModel
                {
                    Id = e.Id,
                    UserId = e.UserID,
                    Username = e.Username,
                    Description = e.Description,
                    Rating = e.Rating,
                    ColRating = e.ColRating
                })
                .ToList();

            var executorTags = _context.ExecutorsTag
                .Where(et => executors.Select(e => e.UserId).Contains(et.UserID))
                .Join(_context.Tags,
                    et => et.TagID,
                    t => t.Id,
                    (et, t) => new { et.UserID, Tag = t.Value })
                .ToList();

            foreach (var executor in executors)
            {
                executor.Tags = executorTags
                    .Where(et => et.UserID == executor.UserId)
                    .Select(et => et.Tag)
                    .ToList();
            }

            return executors;
        }
    }
}
