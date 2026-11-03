
using Intalnire1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

public class BooksController : Controller
{
    private readonly LibraryContext _context;
    private readonly ILogger<BooksController> _logger;

    public BooksController(LibraryContext context, ILogger<BooksController> logger)
    {
        _context = context;
        _logger = logger;
    }

    // GET: BOOKS
    public async Task<IActionResult> Index()    
    {
        var books = await _context.Book.ToListAsync();
        foreach (var b in books)
        {
            var e = _context.Entry(b);
            await e.Reference(x => x.Author).LoadAsync();
            await e.Collection(x => x.Orders).LoadAsync();
        }

        var rows = books.Select(b => new BookRow(b.Title,
                b.Author?.LastName ?? "(gol)", b.Orders.Count)).ToList();

        var sw = Stopwatch.StartNew();
        _logger.LogInformation("Index: {Ms} ms", sw.ElapsedMilliseconds);
        return View(rows);
    }

    // GET: BOOKS/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var book = await _context.Book
            .FirstOrDefaultAsync(m => m.ID == id);
        if (book == null)
        {
            return NotFound();
        }

        return View(book);
    }

    // GET: BOOKS/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: BOOKS/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("ID,Title,Author,Price,GenreID,Genre")] Book book)
    {
        if (ModelState.IsValid)
        {
            _context.Add(book);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(book);
    }

    // GET: BOOKS/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var book = await _context.Book.FindAsync(id);
        if (book == null)
        {
            return NotFound();
        }
        return View(book);
    }

    // POST: BOOKS/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    //[HttpPost]
    //[ValidateAntiForgeryToken]
    //public async Task<IActionResult> Edit(int? id, [Bind("ID,Title,Author,Price,GenreID,Genre")] Book book)
    //{
    //    if (id != book.ID)
    //    {
    //        return NotFound();
    //    }

    //    if (ModelState.IsValid)
    //    {
    //        try
    //        {
    //            _context.Update(book);
    //            await _context.SaveChangesAsync();
    //        }
    //        catch (DbUpdateConcurrencyException)
    //        {
    //            if (!BookExists(book.ID))
    //            {
    //                return NotFound();
    //            }
    //            else
    //            {
    //                throw;
    //            }
    //        }
    //        return RedirectToAction(nameof(Index));
    //    }
    //    return View(book);
    //}

    // VARIANTA 1
    //[HttpPost, ValidateAntiForgeryToken]
    //public async Task<IActionResult> Edit(int id, Book book)
    //{
    //    if (id != book.ID) return NotFound();
    //    _context.Update(book);          // scrie TOATE coloanele
    //    await _context.SaveChangesAsync();
    //    return RedirectToAction(nameof(Index));
    //}

    // VARIANTA 2
    //[HttpPost, ActionName("Edit"), ValidateAntiForgeryToken]
    //public async Task<IActionResult> EditPost(int? id)
    //{
    //    if (id == null) return NotFound();
    //    var bookToUpdate = await _context.Book.FirstOrDefaultAsync(b => b.ID == id);
    //    if (bookToUpdate == null) return NotFound();

    //    if (await TryUpdateModelAsync(bookToUpdate, "",
    //            b => b.Title, b => b.Price, b => b.AuthorID, b => b.GenreID))
    //    {
    //        await _context.SaveChangesAsync();
    //        return RedirectToAction(nameof(Index));
    //    }
    //    return View(bookToUpdate);
    //}

    //VARIANTA 3
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
    int id,
    [Bind("ID,Title,Price,AuthorID,GenreID")] Book book)
    {
        if (id != book.ID) return NotFound();

        _context.Update(book);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }



    // GET: BOOKS/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var book = await _context.Book
            .FirstOrDefaultAsync(m => m.ID == id);
        if (book == null)
        {
            return NotFound();
        }

        return View(book);
    }

    // POST: BOOKS/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? id)
    {
        var book = await _context.Book.FindAsync(id);
        if (book != null)
        {
            _context.Book.Remove(book);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool BookExists(int? id)
    {
        return _context.Book.Any(e => e.ID == id);
    }
}
