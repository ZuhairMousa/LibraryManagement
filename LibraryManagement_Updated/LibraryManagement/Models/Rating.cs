using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Models
{
    public class Rating
    {
        public int RatingId { get; set; }

        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public int BookId { get; set; }
        public Book Book { get; set; } = null!;

        [Range(1, 5)]
        public int Stars { get; set; }

        public string Comment { get; set; } = string.Empty;

        public DateTime RatedAt { get; set; } = DateTime.Now;
    }
}
