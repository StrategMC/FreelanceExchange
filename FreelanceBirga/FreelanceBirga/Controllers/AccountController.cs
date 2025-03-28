using FreelanceBirga.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

public class AccountController : Controller
{
    private readonly AppDbContext _context;

    public AccountController(AppDbContext context)
    {
        _context = context;
    }
    [HttpGet]
    public IActionResult Autorization()
    {
        return View();
    }
    [HttpGet]
    public IActionResult Registration()
    {
        return View();
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Registration(RegistrationViewModel model)
    {
        if (ModelState.IsValid)
        {
            if (await _context.Users.AnyAsync(u => u.Email == model.Email))
            {
                ModelState.AddModelError("Email", "Пользователь с таким email уже существует");
                return View(model);
            }

            if (await _context.Users.AnyAsync(u => u.Login == model.Login))
            {
                ModelState.AddModelError("Login", "Этот логин уже занят");
                return View(model);
            }

            var user = new User
            {
                Email = model.Email,
                Login = model.Login,
                Password = HashPassword(model.Password)
            };
            HttpContext.Session.SetInt32("UserId", user.Id);
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return RedirectToAction("MainPage", "MainPages");
        }
        return View(model);
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Autorization(AutorizationViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Login == model.Login && u.Password == HashPassword(model.Password));

            if (user != null)
            {
                HttpContext.Session.SetString("UserId", user.Id.ToString());
                HttpContext.Session.SetString("UserLogin", user.Login);
                HttpContext.Session.SetInt32("UserId", user.Id);
                return RedirectToAction("MainPage", "MainPages");
            }
            else
            {
                ModelState.AddModelError("Password", "Логин или пароль не верны");
            }
        }
        return View(model);
    }

    private string HashPassword(string input)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
            string hashString = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            return hashString.Substring(0, 50);
        }
    }
}