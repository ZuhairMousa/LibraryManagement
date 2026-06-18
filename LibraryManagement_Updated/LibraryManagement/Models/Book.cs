using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Models
{
    public class Book
    {
        public int BookId { get; set; }

        [Required(ErrorMessage = "Please enter a title.")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter an author.")]
        public string Author { get; set; } = string.Empty;

        public string Genre { get; set; } = string.Empty;

        public string ISBN { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter a year.")]
        [Range(1000, 2100, ErrorMessage = "Please enter a valid year.")]
        public int Year { get; set; }

        public bool IsAvailable { get; set; } = true;

        public string Description { get; set; } = string.Empty;
    }
}
