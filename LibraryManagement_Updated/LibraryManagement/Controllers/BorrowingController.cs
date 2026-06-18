using LibraryManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Controllers
{
    public class BorrowingController : Controller
    {
        private LibraryContext context { get; set; }

        public BorrowingController(LibraryContext ctx) => context = ctx;

        private bool IsLoggedIn() => HttpContext.Session.GetInt32("UserId") != null;
        private bool IsAdmin() => HttpContext.Session.GetString("UserRole") == "Admin";

        // ---- MY BORROWINGS ----
        public IActionResult MyBorrowings()
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Account");

            int userId = (int)HttpContext.Session.GetInt32("UserId")!;
            var borrowings = context.Borrowings
                .Include(b => b.Book)
                .Where(b => b.UserId == userId)
                .OrderByDescending(b => b.BorrowDate)
                .ToList();

            return View(borrowings);
        }

        // ---- BORROW BOOK ----
        [HttpPost]
        public IActionResult Borrow(int bookId)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Account");

            int userId = (int)HttpContext.Session.GetInt32("UserId")!;

            var book = context.Books.Find(bookId);
            if (book == null) return NotFound();

            if (!book.IsAvailable)
            {
                TempData["error"] = "This book is not available for borrowing.";
                return RedirectToAction("Details", "Book", new { id = bookId });
            }

            // Check if user already borrowed this book
            bool alreadyBorrowed = context.Borrowings.Any(
                b => b.UserId == userId && b.BookId == bookId && !b.IsReturned);

            if (alreadyBorrowed)
            {
                TempData["error"] = "You already borrowed this book.";
                return RedirectToAction("Details", "Book", new { id = bookId });
            }

            var borrowing = new Borrowing
            {
                UserId = userId,
                BookId = bookId,
                BorrowDate = DateTime.Now,
                IsReturned = false
            };

            book.IsAvailable = false;
            context.Borrowings.Add(borrowing);
            context.Books.Update(book);
            context.SaveChanges();

            TempData["message"] = "Book borrowed successfully!";
            return RedirectToAction("MyBorrowings");
        }

        // ---- RETURN BOOK ----
        [HttpPost]
        public IActionResult Return(int borrowingId)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Account");

            var borrowing = context.Borrowings.Include(b => b.Book).FirstOrDefault(b => b.BorrowingId == borrowingId);
            if (borrowing == null) return NotFound();

            borrowing.IsReturned = true;
            borrowing.ReturnDate = DateTime.Now;
            borrowing.Book.IsAvailable = true;

            context.Borrowings.Update(borrowing);
            context.Books.Update(borrowing.Book);
            context.SaveChanges();

            TempData["message"] = "Book returned successfully!";
            return RedirectToAction("MyBorrowings");
        }

        // ---- ALL BORROWINGS (Admin) ----
        public IActionResult AllBorrowings()
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Account");
            if (!IsAdmin()) return RedirectToAction("Index", "Home");

            var borrowings = context.Borrowings
                .Include(b => b.Book)
                .Include(b => b.User)
                .OrderByDescending(b => b.BorrowDate)
                .ToList();

            return View(borrowings);
        }
    }
}
