using Microsoft.EntityFrameworkCore;
using BookshopDomain.Model;
using System.IO;

namespace BookshopInfrastructure.Models
{
    public class CategoryViewModel
    {
        private DbbookshopContext _context;
        public Category Category { get; set; } = null!;

        public List<Book> CategoryBooks { get; set; }

        public CategoryViewModel(DbbookshopContext context, Category category)
        {
            _context = context;
            Category = category;

            CategoryBooks = context.Books
                .Where(b => b.Categories.Any(c => c.Id == category.Id))
                .ToList()!;
        }

        public void DeleteCategory()
        {
            var booksInCategory = _context.Books
                .Where(b => b.Categories.Any(c => c.Id == Category.Id))
                .ToList();

            foreach (var book in booksInCategory)
            {
                if (book != null)
                {
                    _context.Books.Remove(book);
                    _context.SaveChanges();
                }
            }

            _context.Categories.Remove(Category);
            _context.SaveChanges();
        }
    }
}

