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
        public async Task<IActionResult> ExecutorProfile(int id)
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

            List<ReviewExecutor> reviews = await _context.ReviewsExecutor.Where(rw => rw.RecipientId == executorWithTags.Executor.Id).ToListAsync();
            List<ReviewForProfileViewModel> reviewViewModels = new List<ReviewForProfileViewModel>();
            foreach (var review in reviews)
            {
                var customer = await _context.Customers.FindAsync(review.SenderId);
                ReviewForProfileViewModel reviewForProfile = new ReviewForProfileViewModel
                {
                    ProfileName = customer.Username,
                    Description = review.Content,
                    Mark = review.Mark,
                };
                reviewViewModels.Add(reviewForProfile);
            }

            var model = new ExecutorViewModel
            {
                Id = executorWithTags.Executor.Id,
                UserId = executorWithTags.Executor.UserID,
                Username = executorWithTags.Executor.Username,
                Description = executorWithTags.Executor.Description,
                Rating = executorWithTags.Executor.Rating,
                ColRating = executorWithTags.Executor.ColRating,
                Tags = executorWithTags.Tags,
                Reviews = reviewViewModels
            };

            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> CustomerProfile(int id)
        {
            userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return RedirectToAction("Index", "Home");
            }
            var customer = _context.Customers.Where(c => c.Id == id).FirstOrDefault();
            //List<ReviewCustomer> reviews = await _context.ReviewsCustomer.Where(ut => ut.SenderId == customer.Id).ToListAsync();
            //List<ReviewViewModel> reviewModels = new List<ReviewViewModel>();
            //foreach (var item in reviews)
            //{

            //}
            List<ReviewCustomer> reviews = await _context.ReviewsCustomer.Where(rw => rw.RecipientId == customer.Id).ToListAsync();
            List<ReviewForProfileViewModel> reviewViewModels = new List<ReviewForProfileViewModel>();
            foreach (var review in reviews)
            {
                var executor = await _context.Executors.FindAsync(review.SenderId);
                ReviewForProfileViewModel reviewForProfile = new ReviewForProfileViewModel
                {
                    ProfileName = executor.Username,
                    Description = review.Content,
                    Mark = review.Mark,
                };
                reviewViewModels.Add(reviewForProfile);
            }
            CustomerViewModel model = new CustomerViewModel
            {
                Username = customer.Username,
                Description = customer.Description,
                ColRating = customer.ColRating,
                Rating = customer.Rating,
                Reviews = reviewViewModels
            };
            return View(model);
        }
    }
}
