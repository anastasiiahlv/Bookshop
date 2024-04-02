using Microsoft.EntityFrameworkCore;
using BookshopDomain.Model;
using System.IO;

namespace BookshopInfrastructure.Models
{
    public class AuthorViewModel
    {
        private DbbookshopContext _context;
        public Author Author { get; set; } = null!;

        public List<Book> AuthorBooks { get; set; }


        public AuthorViewModel(DbbookshopContext context, Author author)
        {
            _context = context;
            Author = author;

            AuthorBooks = context.Books
                .Where(b => b.Authors.Any(a => a.Id == author.Id))
                .ToList()!;
        }

        public void DeleteAuthor()
        {
            var booksByAuthor = _context.Books
                .Where(b => b.Authors.Any(a => a.Id == Author.Id))
                .ToList();

            foreach (var book in booksByAuthor)
            {
                if (book != null)
                {
                    _context.Books.Remove(book);
                    _context.SaveChanges();
                }
            }

            _context.Authors.Remove(Author);
            _context.SaveChanges();
        }
    }
}
