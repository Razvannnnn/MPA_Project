using Intalnire1.Models;
using Microsoft.EntityFrameworkCore;

public class LibraryContext(DbContextOptions<LibraryContext> options) : DbContext(options)
{
    public DbSet<Intalnire1.Models.Book> Book { get; set; } = default!;
    public DbSet<Author> Author { get; set; } = default!;
    public DbSet<Genre> Genre { get; set; } = default!;
    public DbSet<Publisher> Publisher { get; set; } = default!;
    public DbSet<Customer> Customer { get; set; } = default!;
    public DbSet<Order> Order { get; set; } = default!;

}
