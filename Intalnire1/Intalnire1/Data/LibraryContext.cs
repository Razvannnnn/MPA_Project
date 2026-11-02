using Microsoft.EntityFrameworkCore;

public class LibraryContext(DbContextOptions<LibraryContext> options) : DbContext(options)
{
    public DbSet<Intalnire1.Models.Book> Book { get; set; } = default!;
    public DbSet<Intalnire1.Models.Genre> Genre { get; set; } = default!;
}
