using Azure;
using FreelanceBirga.Models.DB;
using FreelanceBirga.Models.VM;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FreelanceBirga.Controllers
{
    public class AddRoleController : Controller
    {
        private readonly AppDbContext _context;

        public AddRoleController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Executor()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }
       [HttpGet]
        public IActionResult Customer()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        } 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Customer(CustomerViewModel model)
        {
            if (await _context.Customers.AnyAsync(u => u.Username == model.Username))
            {
                ModelState.AddModelError("Username", "Исполнитель с таким именем уже существует");
                return View(model);
            }
            var customer = new Customer
            {
                UserID = (int)HttpContext.Session.GetInt32("UserId"),
                Username = model.Username,
                Description = model.Description
            };

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();
            return RedirectToAction("MainPage", "MainPages");

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Executor(ExecutorViewModel model)
        {
                if (await _context.Executors.AnyAsync(u => u.Username == model.Username))
                {
                    ModelState.AddModelError("Username", "Исполнитель с таким именем уже существует");
                    return View(model);
                }

                var executor = new Executor
                {
                    UserID = (int)HttpContext.Session.GetInt32("UserId"),
                    Username = model.Username,
                    Description = model.Description
                };
                   
                _context.Executors.Add(executor);


                ExecutorTag executortag;
                for (int i = 0; i < model.Tags.Count; i++)
                {
                    if (await SuchCategory(model.Tags[i]))  
                    {
                        executortag = new ExecutorTag
                        {
                            UserID = executor.UserID,
                            TagID = Convert.ToInt32(SuchCategoryId(model.Tags[i]))
                        };
                    }
                    else
                    {
                        var tag = new Tag
                        {
                            Value = model.Tags[i].ToLower()
                        };
                        _context.Tags.Add(tag);
                        await _context.SaveChangesAsync();  

                        executortag = new ExecutorTag
                        {
                            UserID = executor.UserID,
                            TagID = tag.Id 
                        };
                    }
                    _context.ExecutorsTag.Add(executortag);
                }
                await _context.SaveChangesAsync();
                return RedirectToAction("MainPage", "MainPages");
         
        }
        public async Task<int> SuchCategoryId(string tagValue)
        {
            var tag = await _context.Tags
                .FirstOrDefaultAsync(t => t.Value.ToLower() == tagValue.ToLower());

            return tag?.Id ?? 0;
        }
        public async Task<bool> SuchCategory(string tagValue)
        {
            if(await _context.Tags.AnyAsync(u=>u.Value==tagValue))
            {
                return true;
            }
            return false;
        }
    }
}