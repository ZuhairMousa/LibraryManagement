using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Models
{
    public class LibraryContext : DbContext
    {
        public LibraryContext(DbContextOptions<LibraryContext> options)
            : base(options) { }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Book> Books { get; set; } = null!;
        public DbSet<Borrowing> Borrowings { get; set; } = null!;
        public DbSet<Rating> Ratings { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Seed Admin user (password = "admin123")
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    UserId = 1,
                    FullName = "Admin",
                    Email = "admin@library.com",
                    Password = "admin123",
                    Role = "Admin",
                    CreatedAt = new DateTime(2024, 1, 1)
                }
            );

            // Seed Books
            modelBuilder.Entity<Book>().HasData(
                new Book { BookId = 1, Title = "The Great Gatsby", Author = "F. Scott Fitzgerald", Genre = "Fiction", ISBN = "9780743273565", Year = 1925, IsAvailable = true, Description = "A story of wealth and obsession in the 1920s." },
                new Book { BookId = 2, Title = "To Kill a Mockingbird", Author = "Harper Lee", Genre = "Fiction", ISBN = "9780061935466", Year = 1960, IsAvailable = true, Description = "A tale of racial injustice and moral growth." },
                new Book { BookId = 3, Title = "1984", Author = "George Orwell", Genre = "Dystopian", ISBN = "9780451524935", Year = 1949, IsAvailable = false, Description = "A dystopian novel about totalitarianism." },
                new Book { BookId = 4, Title = "Clean Code", Author = "Robert C. Martin", Genre = "Technology", ISBN = "9780132350884", Year = 2008, IsAvailable = true, Description = "A handbook of agile software craftsmanship." },
                new Book { BookId = 5, Title = "The Alchemist", Author = "Paulo Coelho", Genre = "Fiction", ISBN = "9780062315007", Year = 1988, IsAvailable = true, Description = "A philosophical novel about following your dreams." },
                new Book { BookId = 6, Title = "Dune", Author = "Frank Herbert", Genre = "Science Fiction", ISBN = "9780441013593", Year = 1965, IsAvailable = true, Description = "An epic science fiction saga." }
            );
        }
    }
}
