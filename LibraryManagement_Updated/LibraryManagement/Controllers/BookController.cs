using LibraryManagement.Models;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.Controllers
{
    public class BookController : Controller
    {
        private LibraryContext context { get; set; }

        public BookController(LibraryContext ctx) => context = ctx;

        private bool IsLoggedIn() =>
            HttpContext.Session.GetInt32("UserId") != null;

        private bool IsAdmin() =>
            HttpContext.Session.GetString("UserRole") == "Admin";

        // ---- LIST ALL BOOKS ----
        public IActionResult Index()
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Account");

            var books = context.Books.OrderBy(b => b.Title).ToList();
            return View(books);
        }

        // ---- SEARCH ----
        public IActionResult Search(string searchBy, string query)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Account");

            ViewBag.SearchBy = searchBy;
            ViewBag.Query = query;

            if (string.IsNullOrWhiteSpace(query))
                return View(new List<Book>());

            IQueryable<Book> books = context.Books;

            if (searchBy == "author")
                books = books.Where(b => b.Author.Contains(query));
            else if (searchBy == "genre")
                books = books.Where(b => b.Genre.Contains(query));
            else
                books = books.Where(b => b.Title.Contains(query));

            return View(books.OrderBy(b => b.Title).ToList());
        }

        // ---- DETAILS ----
        public IActionResult Details(int id)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Account");

            var book = context.Books.Find(id);
            if (book == null) return NotFound();
            return View(book);
        }

        // ---- ADD (GET) - Admin only ----
        [HttpGet]
        public IActionResult Add()
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Account");
            if (!IsAdmin()) return RedirectToAction("Index");

            ViewBag.Action = "Add";
            return View("Edit", new Book());
        }

        // ---- EDIT (GET) - Admin only ----
        [HttpGet]
        public IActionResult Edit(int id)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Account");
            if (!IsAdmin()) return RedirectToAction("Index");

            ViewBag.Action = "Edit";
            var book = context.Books.Find(id);
            return View(book);
        }

        // ---- ADD/EDIT (POST) - Admin only ----
        [HttpPost]
        public IActionResult Edit(Book book)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Account");
            if (!IsAdmin()) return RedirectToAction("Index");

            if (ModelState.IsValid)
            {
                if (book.BookId == 0)
                    context.Books.Add(book);
                else
                    context.Books.Update(book);

                context.SaveChanges();
                TempData["message"] = "Book saved successfully!";
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.Action = (book.BookId == 0) ? "Add" : "Edit";
                return View(book);
            }
        }

        // ---- DELETE (GET) - Admin only ----
        [HttpGet]
        public IActionResult Delete(int id)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Account");
            if (!IsAdmin()) return RedirectToAction("Index");

            var book = context.Books.Find(id);
            if (book == null) return NotFound();
            return View(book);
        }

        // ---- DELETE (POST) - Admin only ----
        [HttpPost]
        public IActionResult Delete(Book book)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Account");
            if (!IsAdmin()) return RedirectToAction("Index");

            context.Books.Remove(book);
            context.SaveChanges();
            TempData["message"] = "Book deleted successfully!";
            return RedirectToAction("Index");
        }
    }
}
