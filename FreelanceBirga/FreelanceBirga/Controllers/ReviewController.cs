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
            string SendlerName;
            bool check;
            if (IsCustomer)
            {
                executorId = chat.ExecutorId;
                check = await _context.ReviewsCustomer.AnyAsync(ut => ut.OrderId == chat.OrderId && ut.RecipientId == executorId);
                var executor = await _context.Executors.FindAsync(executorId);
                SendlerName = executor.Username;
            }
            else
            {
                customerId = chat.CustomerId;
                check = await _context.ReviewsExecutor.AnyAsync(ut => ut.OrderId == chat.OrderId && ut.RecipientId == customerId);
                var customer = await _context.Customers.FindAsync(customerId);
                SendlerName = customer.Username;
            }
            if (check)
            {
                return RedirectToAction("MainPage", "MainPages");
            }
            var order = await _context.Orders.FindAsync(chat.OrderId);
            ReviewViewModel model = new ReviewViewModel
            {
                SendlerName = SendlerName,
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
            for(int i = 0; i < 100; i++)
            {
                Console.WriteLine($"Description {model.Description}");
            }
            
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
            }
            await _context.SaveChangesAsync();
            return RedirectToAction("MainPage", "MainPages");
        }
    }
}
