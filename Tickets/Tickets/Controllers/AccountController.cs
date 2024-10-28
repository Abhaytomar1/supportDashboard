using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YourNamespace.Data;
using YourNamespace.Models;

namespace Tickets.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Account/Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email && u.Password == password);

            if (user != null)
            {
                // Set session or authentication here
                HttpContext.Session.SetString("UserEmail", user.Email);
                HttpContext.Session.SetString("UserRole", user.Role);
                HttpContext.Session.SetInt32("UserId", user.Id); // Store UserId in session
                if (user.Role == "Admin")
                {
                    return RedirectToAction("Dashboard", "Home");
                }
                else if (user.Role == "TeamLead")
                {
                    return RedirectToAction("TeamLeadDashboard", "Home", new { Id = user.Id });
                }
                else if (user.Role == "User")
                {
                    
                    return RedirectToAction("UserDashboard", "Home", new { Id = user.Id });
                }
            }

            ViewBag.ErrorMessage = "Invalid email or password.";
            return View("Login");
        }

        // GET: /Account/Signup
        [HttpGet]
        public IActionResult Signup()
        {
            return View();
        }

        // POST: /Account/Signup
        [HttpPost]
        public async Task<IActionResult> Signup(string email, string password, string confirmPassword, string role)
        {
            if (password != confirmPassword)
            {
                ViewBag.ErrorMessage = "Passwords do not match.";
                return View();
            }

            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email);

            if (existingUser != null)
            {
                ViewBag.ErrorMessage = "User already exists.";
                return View();
            }

            var user = new User
            {
                Email = email,
                Password = password,
                Role = role
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return RedirectToAction("Login");
        }

        // GET: /Account/ForgotPassword
        //[HttpGet]
        //public IActionResult ForgotPassword()
        //{
        //    return View();
        //}

        //// POST: /Account/ForgotPassword
        //[HttpPost]
        //public async Task<IActionResult> ForgotPassword(string email)
        //{
        //    var user = await _context.Users
        //        .FirstOrDefaultAsync(u => u.Email == email);

        //    if (user != null)
        //    {
        //        // In a real application, send an email here
        //        ViewBag.Message = "Password reset link has been sent to your email.";
        //    }
        //    else
        //    {
        //        ViewBag.Message = "Email not found.";
        //    }

        //    return View();
        //}

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
    

            return RedirectToAction("Login");
        }
    }
}
