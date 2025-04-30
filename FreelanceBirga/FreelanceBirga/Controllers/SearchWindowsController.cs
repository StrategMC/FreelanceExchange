using FreelanceBirga.Models.DB;
using FreelanceBirga.Models.VM;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

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
        [HttpGet]
        public async Task<IActionResult> OpenChat(int orderId)
        {
            int userId = (int)HttpContext.Session.GetInt32("UserId");
           
            var orderQ = await _context.Orders.FindAsync(orderId);
            Order order = new Order
            {
                Id = orderQ.Id,
                CustomerID = orderQ.CustomerID,
                //ExecutorID = orderQ.ExecutorID,
                Description = orderQ.Description,
                Price = orderQ.Price,
                Title = orderQ.Title
            };
            //Console.WriteLine($"Id: {order.Id}\n" +
            //    $"CustomerID: {order.CustomerID}\n" +
            //    $"ExecutorID: {order.ExecutorID}\n" +
            //    $"Description {order.Description}\n" +
            //    $"Price: {order.Price}\n" +
            //    $"Title: {order.Title}\n");
            int executorId = await _context.Executors.Where(ex => ex.UserID == userId).Select(ex => ex.Id).FirstAsync();
            bool orderChatQ = await _context.OrdersChat.Where(oc => oc.OrderId == order.Id && oc.ExecutorId == executorId).AnyAsync();
            
            if (!orderChatQ)
            {
                Console.WriteLine("Не найдено");
                OrdersChat ordersChat = new OrdersChat
                {
                    OrderId = order.Id,
                    CustomerId = order.CustomerID,
                    ExecutorId = executorId,
                    Status = 0
                };
                _context.OrdersChat.Add(ordersChat);
                _context.SaveChanges();
            }
            else
            {
                Console.WriteLine("Найдено");
            }
            return RedirectToAction("ChatsWindow", "Chats");
        }
    }
}
