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
        public async Task<IActionResult> MyOrders()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return RedirectToAction("Index", "Home");
            }
            var customer = await _context.Customers.Where(cu => cu.UserID == userId).FirstAsync();
            if (customer == null)
            {
                return RedirectToAction("Index", "Home");
            }
            var Orders = await _context.Orders.Where(or => or.CustomerID == customer.Id).ToListAsync();
            var TempOrders = await _context.TempOrders.Where(or => or.CustomerID == customer.Id).ToListAsync();
            MyOrderViewModel myOrderViewModel = new MyOrderViewModel { orders = Orders, tempOrders = TempOrders };
            return View(myOrderViewModel);
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
        public async Task<IActionResult> AddOrders(OrderViewModel orderViewModel)
        {
            if (orderViewModel != null)
            {
                return View(orderViewModel);
            }
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
            var customer = await _context.Customers.Where(c => c.UserID == userId.Value).FirstAsync();
            var temporders = new TempOrder
            {
                CustomerID = customer.Id,
                Description = orderViewModel.Description,
                Title = orderViewModel.Title,
                Price = (int)orderViewModel.Price
            };
            if(customer.Money < temporders.Price)
            {
                ModelState.AddModelError("", "Недостаточно средств на балансе");
                return View("AddOrders", orderViewModel);
            }
            customer.Money -= temporders.Price;
            customer.OnHoldMoney += temporders.Price;
            _context.TempOrders.Add(temporders);
            await _context.SaveChangesAsync();
            return RedirectToAction("MainPage", "MainPages");

        }
        [HttpGet]
        public async Task<IActionResult> CancelTempOrder(int id)
        {          
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return RedirectToAction("Index", "Home");
            }
            var customer = await _context.Customers.Where(cu => cu.UserID == userId).FirstAsync();
            if (customer == null)
            {
                return RedirectToAction("Index", "Home");
            }
            var TempOrder = await _context.TempOrders.FindAsync(id);
            if (TempOrder == null)
            {
                return RedirectToAction("MyOrders", "Orders");
            }
            _context.TempOrders.Remove(TempOrder);
            customer.Money += TempOrder.Price;
            customer.OnHoldMoney -= TempOrder.Price;
            await _context.SaveChangesAsync();
            return RedirectToAction("MyOrders", "Orders"); 
        }
        [HttpGet]
        public async Task<IActionResult> CancelOrder(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return RedirectToAction("Index", "Home");
            }
            var customer = await _context.Customers.Where(cu => cu.UserID == userId).FirstAsync();
            if (customer == null)
            {
                return RedirectToAction("Index", "Home");
            }
            var Order = await _context.Orders.FindAsync(id);
            if (Order == null || (Order.InWork || Order.Redy))
            {
                return RedirectToAction("MyOrders", "Orders");
            }
            _context.Orders.Remove(Order);
            customer.Money += Order.Price;
            customer.OnHoldMoney -= Order.Price;
            await _context.SaveChangesAsync();
            return RedirectToAction("MyOrders", "Orders");
        }
    }
}
