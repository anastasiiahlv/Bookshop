using Microsoft.EntityFrameworkCore;
using BookshopDomain.Model;
using System.IO;
using System.Security.Policy;

namespace BookshopInfrastructure.Models
{
    public class PublisherViewModel
    {
        private DbbookshopContext _context;
        public BookshopDomain.Model.Publisher Publisher { get; set; } = null!;

        public List<Book> PublisherBooks { get; set; }

        public PublisherViewModel(DbbookshopContext context, BookshopDomain.Model.Publisher publisher)
        {
            _context = context;
            Publisher = publisher;

            PublisherBooks = context.Books
                .Where(b => b.PublisherId == publisher.Id)
                .ToList()!;
        }

        public void DeletePublisher()
        {
            var booksByPublisher = _context.Books
                .Where(b => b.PublisherId == Publisher.Id)
                .ToList();

            foreach (var book in booksByPublisher)
            {
                if (book != null)
                {
                    _context.Books.Remove(book);
                    _context.SaveChanges();
                }
            }

            _context.Publishers.Remove(Publisher);
            _context.SaveChanges();
        }
    }
}
