using FreelanceBirga.Models.DB;
using FreelanceBirga.Models.VM;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FreelanceBirga.Controllers
{
    public class ReviewController : Controller
    {
        private readonly AppDbContext _context;

        int? userId;
        public ReviewController(AppDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<IActionResult> Review(bool IsCustomer,int ChatId)
        {
            userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return RedirectToAction("Index", "Home");
            }
            var chat = await _context.OrdersChat.FindAsync(ChatId);
            int? customerId;
            int? executorId;
            string RecepiantName;
            bool check;

            executorId = chat.ExecutorId;
            customerId = chat.CustomerId;

            var chat_for_review = await _context.OrdersChatForRewiew.Where(ut => ut.OrderChatId == chat.Id).FirstOrDefaultAsync();
            if (IsCustomer)
            {
                executorId = chat.ExecutorId;
                check = chat_for_review.CustomerReview;
                var executor = await _context.Executors.FindAsync(executorId);
                RecepiantName = executor.Username;
            }
            else
            {
                customerId = chat.CustomerId;
                check = chat_for_review.ExecutorReview;
                var customer = await _context.Customers.FindAsync(customerId);
                RecepiantName = customer.Username;
            }
            if (check)
            {
                return RedirectToAction("MainPage", "MainPages");
            }
            var order = await _context.Orders.FindAsync(chat.OrderId);
            ReviewViewModel model = new ReviewViewModel
            {
                RecepiantName = RecepiantName,
                IsCustomer = IsCustomer,
                ChatId = ChatId,
                OrderName = order.Title
            };
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Review(ReviewViewModel model)
        {

            userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return RedirectToAction("Index", "Home");
            }

            if (model.Mark == 0)
            {
                ModelState.AddModelError("Mark", "Поставьте оценку");
                return View(model);
            }
            //if (!ModelState.IsValid)
            //{
            //    return View(model);
            //}
            var chat = await _context.OrdersChat.FindAsync(model.ChatId);
            var customer = await _context.Customers.FindAsync(chat.CustomerId);
            var executor = await _context.Executors.FindAsync(chat.ExecutorId);

            var chat_for_review = await _context.OrdersChatForRewiew.Where(ut => ut.OrderChatId == chat.Id).FirstOrDefaultAsync();
            List<int> ratings;
            if (model.IsCustomer)
            {
                ReviewExecutor review = new ReviewExecutor
                {
                    SenderId = customer.Id,
                    RecipientId = executor.Id,
                    OrderId = chat.OrderId,
                    Mark = model.Mark,
                    Content = model.Description
                };

                await _context.ReviewsExecutor.AddAsync(review);
                chat_for_review.CustomerReview = true;
                // await _context.SaveChangesAsync();

                executor.ColRating = await _context.ReviewsExecutor.Where(ut => ut.RecipientId == executor.Id).CountAsync() + 1;
                ratings = await _context.ReviewsExecutor.Where(ut => ut.RecipientId == executor.Id).Select(ut => ut.Mark).ToListAsync();
                ratings.Add(model.Mark);
                executor.Rating = Convert.ToInt32(ratings.Average());
                // Console.WriteLine($"Rating ;{executor.Rating};");
                //// _context.Executors.Update(executor);
                //_context.Entry(executor).Property(s => s.Rating).IsModified = true;
                //_context.Entry(executor).Property(s => s.ColRating).IsModified = true;
                Console.WriteLine($"ID {executor.Id}");
                Console.WriteLine($"Name {executor.Username}");
                Console.WriteLine($"Rating {executor.Rating}");
                Console.WriteLine($"Col {executor.ColRating}");
            }
            else
            {
                ReviewCustomer review = new ReviewCustomer
                {
                    SenderId = executor.Id,
                    RecipientId = customer.Id,
                    OrderId = chat.OrderId,
                    Mark = model.Mark,
                    Content = model.Description
                };

                await _context.ReviewsCustomer.AddAsync(review);
                chat_for_review.ExecutorReview = true;
                //await _context.SaveChangesAsync();

                customer.ColRating = await _context.ReviewsCustomer.Where(ut => ut.RecipientId == customer.Id).CountAsync() + 1;
                ratings = await _context.ReviewsCustomer.Where(ut => ut.RecipientId == customer.Id).Select(ut => ut.Mark).ToListAsync();
                ratings.Add(model.Mark);
                customer.Rating = Convert.ToInt32(ratings.Average());
                //_context.Update(customer);
            }
            //proverka
            //var entry = _context.Entry(executor);
            
            //for (int i = 0; i < 10; i++)
            //{
            //    Console.WriteLine($"State: {entry.State}");
            //}

            _context.SaveChanges();
            return RedirectToAction("MainPage", "MainPages");
        }
    }
}
