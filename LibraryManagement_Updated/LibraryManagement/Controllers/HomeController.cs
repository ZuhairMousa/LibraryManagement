using LibraryManagement.Models;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.Controllers
{
    public class HomeController : Controller
    {
        private LibraryContext context { get; set; }

        public HomeController(LibraryContext ctx) => context = ctx;

        public IActionResult Index()
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
                return RedirectToAction("Login", "Account");

            ViewBag.TotalBooks = context.Books.Count();
            ViewBag.AvailableBooks = context.Books.Count(b => b.IsAvailable);
            ViewBag.TotalUsers = context.Users.Count();
            ViewBag.TotalBorrowings = context.Borrowings.Count(b => !b.IsReturned);
            return View();
        }
    }
}
