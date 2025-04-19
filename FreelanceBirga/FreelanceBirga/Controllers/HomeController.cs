// Controllers/HomeController.cs
using FreelanceBirga.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

public class HomeController : Controller
{
    private readonly AppDbContext _context;

    public HomeController(AppDbContext context)
    {
        _context = context;
    }
    [HttpGet]
    public IActionResult Index()
    {
        if (User.Identity.IsAuthenticated)
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            HttpContext.Session.SetInt32("UserId", userId);
            return RedirectToAction("MainPage", "MainPages");
        }
        
        return View();
    }
}