using LibraryManagement.Models;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.Controllers
{
    public class RatingController : Controller
    {
        private LibraryContext context { get; set; }

        public RatingController(LibraryContext ctx) => context = ctx;

        private bool IsLoggedIn() => HttpContext.Session.GetInt32("UserId") != null;

        [HttpPost]
        public IActionResult Rate(int bookId, int stars, string comment)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Account");

            int userId = (int)HttpContext.Session.GetInt32("UserId")!;

            // Check if already rated
            var existing = context.Ratings.FirstOrDefault(
                r => r.UserId == userId && r.BookId == bookId);

            if (existing != null)
            {
                existing.Stars = stars;
                existing.Comment = comment;
                existing.RatedAt = DateTime.Now;
                context.Ratings.Update(existing);
            }
            else
            {
                var rating = new Rating
                {
                    UserId = userId,
                    BookId = bookId,
                    Stars = stars,
                    Comment = comment,
                    RatedAt = DateTime.Now
                };
                context.Ratings.Add(rating);
            }

            context.SaveChanges();
            TempData["message"] = "Rating submitted successfully!";
            return RedirectToAction("Details", "Book", new { id = bookId });
        }
    }
}
