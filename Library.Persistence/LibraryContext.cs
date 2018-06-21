using Microsoft.EntityFrameworkCore;

namespace Library.Persistence
{
    public class LibraryContext : DbContext
    {
        public LibraryContext(DbContextOptions<LibraryContext> options) : base(options) { }

        public DbSet<Book> Books { get; set; }
        public DbSet<Volume> Volumes { get; set; }
        public DbSet<Rent> Rents { get; set; }
        public DbSet<Visitor> Visitors { get; set; }
        public DbSet<Librarian> Librarians { get; set; }
    }
}
