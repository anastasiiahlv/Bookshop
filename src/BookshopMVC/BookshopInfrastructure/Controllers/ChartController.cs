using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookshopInfrastructure.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChartController : ControllerBase
    {
        private readonly DbbookshopContext _context;

        public ChartController(DbbookshopContext context)
        {
            _context = context;
        }

        [HttpGet("JsonData")]
        public JsonResult JsonData()
        {
            var categories = _context.Categories.Include(c => c.Books).ToList();
            List<object> catBook = new List<object>();
            catBook.Add(new[] { "Категорія", "Кількість книжок" });

            foreach (var category in categories)
            {
                catBook.Add(new object[] { category.Name, category.Books.Count });
            }
            return new JsonResult(catBook);
        }

        [HttpGet("AuthorBookCountsJson")]
        public JsonResult AuthorBookCountsJson()
        {
            var authors = _context.Authors.Include(a => a.Books).ToList();

            List<object> authorBookCounts = new List<object>();
            authorBookCounts.Add(new[] { "Ім'я автора", "Кількість книг" });

            foreach (var author in authors)
            {
                authorBookCounts.Add(new object[] { author.FullName, author.Books.Count });
            }

            return new JsonResult(authorBookCounts);
        }
    }
}
