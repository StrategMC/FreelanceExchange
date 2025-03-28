using FreelanceBirga.Models.DB;
using FreelanceBirga.Models.VM;
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
            return View();
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
                await _context.SaveChangesAsync();

                return RedirectToAction("MainPage", "MainPages");
         
        }
    }
}