using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BookshopDomain.Model;
using BookshopInfrastructure;

namespace BookshopInfrastructure.Controllers
{
    public class BooksController : Controller
    {
        private readonly DbbookshopContext _context;

        public BooksController(DbbookshopContext context)
        {
            _context = context;
        }

        // GET: Books
        public async Task<IActionResult> Index()
        {
            var dbbookshopContext = _context.Books.Include(b => b.Publisher)
                                                  .Include(b => b.Authors)
                                                  .Include(b => b.Categories);

            return View(await dbbookshopContext.ToListAsync());
        }

        public async Task<IActionResult> Filter(string title, string author, string publisher)
        {
            var booksQuery = _context.Books.Include(b => b.Publisher)
                                           .Include(b => b.Authors)
                                           .Include(b => b.Categories);

            if (!string.IsNullOrEmpty(title))
            {
                booksQuery = booksQuery.Where(b => b.Title.Contains(title)).Include(b => b.Categories);
            }

            if (!string.IsNullOrEmpty(author))
            {
                booksQuery = booksQuery.Where(b => b.Authors.Any(a => a.FullName.Contains(author))).Include(b => b.Categories);
            }

            if (!string.IsNullOrEmpty(publisher))
            {
                booksQuery = booksQuery.Where(b => b.Publisher.Name.Contains(publisher)).Include(b => b.Categories);
            }

            var filteredBooks = await booksQuery.ToListAsync();

            return PartialView("_FilteredBooks", filteredBooks);
        }

        // GET: Books/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books
                .Include(b => b.Publisher)
                .Include(a => a.Authors)
                .Include(c => c.Categories)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // GET: Books/Create
        public IActionResult Create()
        {
            ViewData["PublisherId"] = new SelectList(_context.Publishers, "Id", "Name");
            ViewData["Categories"] = new MultiSelectList(_context.Categories, "Id", "Name");
            ViewData["Authors"] = new MultiSelectList(_context.Authors, "Id", "FullName");
            return View();
        }

        // POST: Books/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PublisherId, Categories, Authors, Title,Description,Price,Id")] Book book, int[] Categories, int[] Authors)
        {
            if (Categories != null)
            {
                foreach (var categoryId in Categories)
                {
                    var category = await _context.Categories.FindAsync(categoryId);
                    if (category != null)
                    {
                        book.Categories.Add(category);
                    }
                }
            }

            if (Authors != null)
            {
                foreach (var authorId in Authors)
                {
                    var author = await _context.Authors.FindAsync(authorId);
                    if (author != null)
                    {
                        book.Authors.Add(author);
                    }
                }
            }
            _context.Add(book);
                await _context.SaveChangesAsync();

            ViewData["PublisherId"] = new SelectList(_context.Publishers, "Id", "Name", book.PublisherId);
            ViewData["Categories"] = new MultiSelectList(_context.Categories, "Id", "Name", book.Categories.Select(c => c.Id));
            ViewData["Authors"] = new MultiSelectList(_context.Authors, "Id", "FullName", book.Authors.Select(a => a.Id));
            return RedirectToAction(nameof(Index));
        }


        // GET: Books/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books
                    .Include(b => b.Categories)
                    .Include(b => b.Authors)
                    .FirstOrDefaultAsync(m => m.Id == id);
            if (book == null)
            {
                return NotFound();
            }
            ViewData["PublisherId"] = new SelectList(_context.Publishers, "Id", "Name", book.PublisherId);
            ViewData["Categories"] = new MultiSelectList(_context.Categories, "Id", "Name", book.Categories.Select(c => c.Id));
            ViewData["Authors"] = new MultiSelectList(_context.Authors, "Id","FullName", book.Authors.Select(a => a.Id));
            return View(book);
        }

        // POST: Books/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PublisherId,Title,Description,Price,Id")] Book book, int[] Categories, int[] Authors)
        {
            if (id != book.Id)
            {
                return NotFound();
            }

            try
            {
                _context.Update(book);
                await _context.SaveChangesAsync();

                var bookInDb = await _context.Books
                .Include(b => b.Categories)
                .Include(b => b.Authors)
                .FirstOrDefaultAsync(m => m.Id == id);

                bookInDb.Categories.Clear();
                foreach (var categoryId in Categories)
                {
                    var category = await _context.Categories.FindAsync(categoryId);
                    if (category != null)
                    {
                        bookInDb.Categories.Add(category);
                    }
                }

                bookInDb.Authors.Clear();
                foreach (var authorId in Authors)
                {
                    var author = await _context.Authors.FindAsync(authorId);
                    if (author != null)
                    {
                        bookInDb.Authors.Add(author);
                    }
                }

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookExists(book.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            /*ViewData["PublisherId"] = new SelectList(_context.Publishers, "Id", "Name", book.PublisherId);
            ViewData["Categories"] = new MultiSelectList(_context.Categories, "Id", "Name", book.Categories.Select(c => c.Id));
            ViewData["Authors"] = new MultiSelectList(_context.Authors, "Id", "FullName", book.Authors.Select(a => a.Id));
            return View(book);*/
        }

       

        // GET: Books/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books
                .Include(b => b.Publisher)
                .Include(a => a.Authors)
                .Include(c => c.Categories)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // POST: Books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book != null)
            {
                _context.Books.Remove(book);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookExists(int id)
        {
            return _context.Books.Any(e => e.Id == id);
        }

        public async Task<IActionResult> BooksInCategory(int? id, string? name)
        {
            if (id == 0) return RedirectToAction("Categories", "Index");

            var category = await _context.Categories.FindAsync(id);
            if (category == null) return RedirectToAction("Categories", "Index");
            
            ViewBag.CategoryId = id;
            ViewBag.CategoryName = name;
            var booksInCategory = _context.Books.Where(b => b.Categories.Any(c => c.Id == id)).Include(b => b.Categories)
                                                                                              .Include(b => b.Authors)
                                                                                              .Include(b => b.Publisher);

            return View(await booksInCategory.ToListAsync());
        }

        public async Task<IActionResult> BooksByPublisher(int? id, string? name)
        {
            if (id == 0) return RedirectToAction("Publishers", "Index");

            var publisher = await _context.Publishers.FindAsync(id);
            if (publisher == null) return RedirectToAction("Publishers", "Index");

            ViewBag.PublisherId = id;
            ViewBag.PublisherName = name;
            var booksByPublisher = _context.Books.Where(b => b.PublisherId == id)
                                                 .Include(b => b.Authors);

            return View(await booksByPublisher.ToListAsync());
        }

        public async Task<IActionResult> BooksByAuthor(int? id, string? firstName, string? lastName)
        {
            if (id == 0) return RedirectToAction("Authors", "Index");

            var author = await _context.Authors.FindAsync(id);
            if (author == null) return RedirectToAction("Authors", "Index");

            ViewBag.AuthorId = id;
            ViewBag.AuthorFirstName = firstName;
            var booksByAuthor = _context.Books.Where(b => b.Authors.Any(c => c.Id == id)).Include(b => b.Categories)
                                                                                         .Include(b => b.Publisher);

            return View(await booksByAuthor.ToListAsync());
        }
    }
}
