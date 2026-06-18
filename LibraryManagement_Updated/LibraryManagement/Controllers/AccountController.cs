using LibraryManagement.Models;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.Controllers
{
    public class AccountController : Controller
    {
        private LibraryContext context { get; set; }

        public AccountController(LibraryContext ctx) => context = ctx;

        // ---- REGISTER (GET) ----
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // ---- REGISTER (POST) ----
        [HttpPost]
        public IActionResult Register(User user)
        {
            // Check if email already exists
            bool emailExists = context.Users.Any(u => u.Email == user.Email);
            if (emailExists)
            {
                ModelState.AddModelError("Email", "This email is already registered.");
            }

            if (ModelState.IsValid)
            {
                user.Role = "User";  // new users are always regular users
                user.CreatedAt = DateTime.Now;
                context.Users.Add(user);
                context.SaveChanges();
                TempData["message"] = "Account created! Please log in.";
                return RedirectToAction("Login");
            }
            return View(user);
        }

        // ---- LOGIN (GET) ----
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // ---- LOGIN (POST) ----
        [HttpPost]
        public IActionResult Login(User user)
        {
            var dbUser = context.Users.FirstOrDefault(
                u => u.Email == user.Email && u.Password == user.Password);

            if (dbUser == null)
            {
                ModelState.AddModelError("", "Invalid email or password.");
                return View(user);
            }

            // Save session
            HttpContext.Session.SetInt32("UserId", dbUser.UserId);
            HttpContext.Session.SetString("UserName", dbUser.FullName);
            HttpContext.Session.SetString("UserRole", dbUser.Role);

            return RedirectToAction("Index", "Home");
        }

        // ---- LOGOUT ----
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        // ---- PROFILE ----
        public IActionResult Profile()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login");

            var user = context.Users.Find(userId);
            if (user == null)
                return RedirectToAction("Login");

            return View(user);
        }

        // ---- CHANGE PASSWORD (GET) ----
        [HttpGet]
        public IActionResult ChangePassword()
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
                return RedirectToAction("Login");
            return View();
        }

        // ---- CHANGE PASSWORD (POST) ----
        [HttpPost]
        public IActionResult ChangePassword(string currentPassword,
            string newPassword, string confirmPassword)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login");

            var user = context.Users.Find(userId);
            if (user == null)
                return RedirectToAction("Login");

            if (user.Password != currentPassword)
            {
                ViewBag.Error = "Current password is incorrect.";
                return View();
            }
            if (newPassword != confirmPassword)
            {
                ViewBag.Error = "New passwords do not match.";
                return View();
            }

            user.Password = newPassword;
            context.Users.Update(user);
            context.SaveChanges();
            TempData["message"] = "Password changed successfully!";
            return RedirectToAction("Profile");
        }
    }
}
