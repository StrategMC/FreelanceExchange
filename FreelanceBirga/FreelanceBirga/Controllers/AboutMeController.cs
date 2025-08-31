using FreelanceBirga.Models.DB;
using FreelanceBirga.Models.VM;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.HttpSys;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace FreelanceBirga.Controllers
{
    public class AboutMeController : Controller
    {
        private readonly AppDbContext _context;
        int? userId;
        public AboutMeController(AppDbContext context)
        {
            _context = context;
        }
        [HttpPost]
        public async Task<IActionResult> UpdateExecutorTags([FromBody] List<string> tags)
        {
            userId = HttpContext.Session.GetInt32("UserId");
            List<string> deleteTagsString = new List<string>();
            List<string> addTagsString = new List<string>();
            try
            {
                List<Tag> tagsCurrently = await GetTag();
                foreach (var tag in tags)
                {
                    if (!tagsCurrently.Any(t => t.Value == tag))
                    {
                        addTagsString.Add(tag);
                    }
                }
                foreach (var tag in tagsCurrently)
                {
                    if (!tags.Contains(tag.Value))
                    {
                        deleteTagsString.Add(tag.Value);
                    }
                }

                List<int> deleteTagsInt = await _context.Tags.Where(t => deleteTagsString.Contains(t.Value)).Select(t => t.Id).ToListAsync();
                List<ExecutorTag> deleteTags = await _context.ExecutorsTag.Where(tags => deleteTagsInt.Contains(tags.TagID)).Select(tags => tags).ToListAsync();
                if (deleteTags.Any())
                {
                    _context.ExecutorsTag.RemoveRange(deleteTags);
                    await _context.SaveChangesAsync();
                }

                if (addTagsString.Any())
                {
                    var existingTags = await _context.Tags.Where(t => addTagsString.Contains(t.Value)).ToListAsync();

                    foreach (var tagValue in addTagsString)
                    {
                        var existingTag = existingTags.FirstOrDefault(t => t.Value == tagValue);
                        if (existingTag == null)
                        {
                            var newTag = new Tag { Value = tagValue };
                            _context.Tags.Add(newTag);
                            await _context.SaveChangesAsync();
                            existingTag = newTag;
                        }

                        _context.ExecutorsTag.Add(new ExecutorTag
                        {
                            UserID = userId.Value,
                            TagID = existingTag.Id
                        });
                    }

                    await _context.SaveChangesAsync();
                }
                //foreach (var tag in tags)
                //{
                //    Console.WriteLine(tag);
                //}
                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при обработке тегов: {ex.Message}");
                return StatusCode(500, "Произошла ошибка при обработке тегов");
            }
        }
        public async Task<List<ExecutorTag>> GetExecutorTag(Tag tag)
        {
            List<ExecutorTag> tags = await _context.ExecutorsTag.Where(ut => ut.TagID == tag.Id).Select(ut => ut).ToListAsync();
            return tags;
        }
        public async Task<List<Tag>> GetTag()
        {
            List<int> tagsId = await _context.ExecutorsTag.Where(ut => ut.UserID == userId).Select(ut => ut.TagID).ToListAsync();
            List<Tag> tags = await _context.Tags.Where(t => tagsId.Contains(t.Id)).Select(t => t).ToListAsync();
            return tags;
        }
        [HttpGet]
        public async Task<IActionResult> AboutMeCustomer()
        {
            userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return RedirectToAction("Index", "Home");
            }
            
            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.UserID == userId.Value);

            if (customer == null)
            {
                return NotFound();
            }

            List<ReviewCustomer> reviews = await _context.ReviewsCustomer.Where(rw => rw.RecipientId == customer.Id).ToListAsync();
            List<ReviewForProfileViewModel> reviewViewModels = new List<ReviewForProfileViewModel>();
            foreach(var review in reviews)
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
            var model = new CustomerViewModel
            {
                Username = customer.Username,
                Description = customer.Description,
                Rating = customer.Rating,
                ColRating = customer.ColRating,
                Reviews = reviewViewModels,
                Money = customer.Money,
                OnHoldMoney = customer.OnHoldMoney
            };
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> AboutMeExecutor()
        {
            userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return RedirectToAction("Index", "Home");
            }

            var executor = await _context.Executors
                .FirstOrDefaultAsync(c => c.UserID == userId.Value);

            if (executor == null)
            {
                return NotFound();
            }
           

            List<Tag> tags = await GetTag();
            List<string> tagsValue = new List<string>();
            for (int i=0; i<tags.Count; i++)
            {
                tagsValue.Add(tags[i].Value);
            }

            List<ReviewExecutor> reviews  = await _context.ReviewsExecutor.Where(rw => rw.RecipientId == executor.Id).ToListAsync();
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
                Username = executor.Username,
                Description = executor.Description,
                Rating = executor.Rating,
                ColRating = executor.ColRating,
                Tags = tagsValue,
                Reviews = reviewViewModels,
                Money = executor.Money
            };
            return View(model);
        }
    }
}
