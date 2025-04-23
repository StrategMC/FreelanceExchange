using FreelanceBirga.Models.DB;
using FreelanceBirga.Models.VM;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace FreelanceBirga.Controllers
{
    public class OrdersController : Controller
    {
        private readonly AppDbContext _context;
        int? userId;
        public OrdersController(AppDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<IActionResult> SearchOrders(SearchOrdersViewModel searchOrdersViewModel)
        {
            //polsovatel proverka
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return RedirectToAction("Index", "Home");
            }
            var executor = await _context.Executors.FirstOrDefaultAsync(c => c.UserID == userId.Value);

            if (executor == null)
            {
                return RedirectToAction("MainPage", "MainPages");
            }
            //filtr zakazy
           // var query = _context.Orders.Select(od => od);
            var query = _context.Orders.Where(od => od.InWork == false);

            if (!string.IsNullOrEmpty(searchOrdersViewModel.SearchWord))
            {
                if (searchOrdersViewModel.InDescription)
                {
                    query = query.Where(ord => EF.Functions.Like(ord.Title, $"%{searchOrdersViewModel.SearchWord}%") ||  EF.Functions.Like(ord.Description, $"%{searchOrdersViewModel.SearchWord}%"));
                }
                else
                {
                    query = query.Where(ord => EF.Functions.Like(ord.Title, $"%{searchOrdersViewModel.SearchWord}%"));
                }
            }

            if (searchOrdersViewModel.MinPrice > 0)
            {
                query = query.Where(ord => ord.Price >= searchOrdersViewModel.MinPrice);
            }

            searchOrdersViewModel.FilteredOrders = await query.ToListAsync();
            return View(searchOrdersViewModel);
        }
        [HttpGet]
        public async Task<IActionResult> AddOrders()
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
                return RedirectToAction("MainPage", "MainPages");
            }

            return View();
        }
        [HttpPost]

        public async Task<IActionResult> AddOrdersToTempOrders(OrderViewModel orderViewModel)
        {
            userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return RedirectToAction("Index", "Home");
            }
            if (!ModelState.IsValid)
            {
                return RedirectToAction("AddOrders", "Orders");
            }
            userId = HttpContext.Session.GetInt32("UserId");
            var temporders = new TempOrder
            {
                CustomerID = _context.Customers.Where(c => c.UserID == userId.Value).Select(c => c.Id).FirstOrDefault(),
                Description = orderViewModel.Description,
                Title = orderViewModel.Title,
                Price = (int)orderViewModel.Price
            };
            _context.TempOrders.Add(temporders);
            await _context.SaveChangesAsync();
            return RedirectToAction("MainPage", "MainPages");

        }
    }
}
