using BookshopDomain.Model;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Vml.Office;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace BookshopInfrastructure.Services
{
    public class BookImportService : IImportService<Book>
    {
        private readonly DbbookshopContext _context;
        private static bool _isValid;
        private static string? _validationErrorStr;

        public BookImportService(DbbookshopContext context)
        {
            _validationErrorStr = "";
            _isValid = true;
            _context = context;
        }

        public async Task<string> ImportFromStreamAsync(Stream stream, CancellationToken cancellationToken)
        {
            if (!stream.CanRead)
            {
                return "Дані не можуть бути прочитані";
            }

            using (XLWorkbook workBook = new XLWorkbook(stream))
            {
                foreach (IXLWorksheet worksheet in workBook.Worksheets)
                {
                    foreach (var row in worksheet.RowsUsed().Skip(1))
                    {
                        var (book, errorMessage) = await AddBookAsync(row, cancellationToken);
                        if (errorMessage != null)
                        {
                            return errorMessage;
                        }
                    }
                }
            }
            await _context.SaveChangesAsync(cancellationToken);
            return "Імпорт успішно завершено";
        }

        private bool IsBookInDB(Book? book)
        {
            if (book == null)
            {
                return false;
            }

            return _context.Books.Any(b =>
                b.Title == book.Title &&
                b.Description == book.Description &&
                b.Price == book.Price);
        }

        private async Task<(Book?, string?)> AddBookAsync(IXLRow row, CancellationToken cancellationToken)
        {
            var (title, titleError) = GetBookTitle(row);
            var (description, descriptionError) = GetBookDescription(row);
            var (price, priceError) = GetBookPrice(row);
            var (publisherId, publisherIdError) = GetPublisherId(row);

            var errorMessages = new List<string>();

            if (titleError != null) errorMessages.Add(titleError);
            if (descriptionError != null) errorMessages.Add(descriptionError);
            if (priceError != null) errorMessages.Add(priceError);
            if (publisherIdError != null) errorMessages.Add(publisherIdError);

            if (errorMessages.Any())
            {
                return (null, string.Join("\n", errorMessages));
            }

            var book = _context.Books
                .Include(b => b.Authors)
                .Include(b => b.Categories)
                .FirstOrDefault(b => b.Title == title);

            if (book != null)
            {
                book.Description = description;
                book.Price = price.Value;
                book.PublisherId = publisherId.Value;

                book.Authors.Clear();
                book.Categories.Clear();

                var authors = await GetAuthorsAsync(row, cancellationToken);
                var categories = await GetCategoriesAsync(row, cancellationToken);

                foreach (var author in authors)
                {
                    book.Authors.Add(author);
                }

                foreach (var category in categories)
                {
                    book.Categories.Add(category);
                }

                _context.Books.Update(book);
            }
            else
            {
                book = new Book
                {
                    Title = title,
                    Description = description,
                    PublisherId = publisherId.Value,
                    Price = price.Value
                };

                var authors = await GetAuthorsAsync(row, cancellationToken);
                var categories = await GetCategoriesAsync(row, cancellationToken);

                foreach (var author in authors)
                {
                    book.Authors.Add(author);
                }

                foreach (var category in categories)
                {
                    book.Categories.Add(category);
                }

                _context.Books.Add(book);
            }

            return (book, null);
        }

        private static (string, string?) GetBookTitle(IXLRow row)
        {
            int rowNumber = row.RowNumber();
            string value = row.Cell(1).Value.ToString();

            if (string.IsNullOrEmpty(value))
            {
                return ("", $"Назва книги обов'язкова! Помилка в рядку {rowNumber}.\n");
            }

            return (value, null);
        }

        private static (string, string?) GetBookDescription(IXLRow row)
        {
            int rowNumber = row.RowNumber();
            string value = row.Cell(3).Value.ToString();

            if (string.IsNullOrEmpty(value))
            {
                return ("", $"Опис книги обов'язковий! Помилка в рядку {rowNumber}.\n");
            }

            return (value, null);
        }

        private static (double?, string?) GetBookPrice(IXLRow row)
        {
            int rowNumber = row.RowNumber();
            string valueStr = row.Cell(5).Value.ToString();

            if (string.IsNullOrEmpty(valueStr))
            {
                return (null, $"Ціна книги обов'язкова! Помилка в рядку {rowNumber}.\n");
            }

            if (double.TryParse(valueStr, out double value))
            {
                if (value < 0.01 || value > 20000.00)
                {
                    return (null, $"Ціна має бути більше 0.01 і менше 20000.00. Помилка в рядку {rowNumber}.\n");
                }

                Regex regex = new Regex(@"^\d+(\.\d{1,2})?$");
                if (!regex.IsMatch(valueStr))
                {
                    return (null, $"Неправильний формат ціни. Помилка в рядку {rowNumber}.\n");
                }

                return (value, null);
            }
            else
            {
                return (null, $"Помилка в ціні в рядку {rowNumber}.\n");
            }
        }

        private (int?, string?) GetPublisherId(IXLRow row)
        {
            int rowNumber = row.RowNumber();
            var publisherName = row.Cell(4).Value.ToString().Trim();

            if (!string.IsNullOrWhiteSpace(publisherName))
            {
                var publisher = _context.Publishers.FirstOrDefault(p => p.Name.Equals(publisherName));

                if (publisher != null)
                {
                    return (publisher.Id, null);
                }
                else
                {
                    publisher = new Publisher { Name = publisherName };
                    _context.Publishers.Add(publisher);
                    _context.SaveChanges();

                    return (publisher.Id, null);
                }
            }
            else
            {
                return (null, $"Назва видавництва обов'язкова! Помилка в рядку {rowNumber}.\n");
            }
        }

        private async Task<List<Category>> GetCategoriesAsync(IXLRow row, CancellationToken cancellationToken)
        {
            string value = row.Cell(6).Value.ToString();
            var categories = new List<Category>();

            if (!string.IsNullOrEmpty(value))
            {
                var categoriesArr = value.Split(",").Select(part => part.Trim()).ToArray();

                foreach (var categoryName in categoriesArr)
                {
                    var category = await _context.Categories.FirstOrDefaultAsync(cat => cat.Name == categoryName, cancellationToken);

                    if (category == null)
                    {
                        category = new Category { Name = categoryName };
                        _context.Categories.Add(category);
                    }
                    categories.Add(category);
                }
            }
            else
            {
                int rowNumber = row.RowNumber();
                _isValid = false;
                _validationErrorStr += $"Категорії обов'язкові! Помилка в рядку {rowNumber}.\n";
            }

            return categories;
        }

        private async Task<List<Author>> GetAuthorsAsync(IXLRow row, CancellationToken cancellationToken)
        {
            string value = row.Cell(2).Value.ToString();
            var authors = new List<Author>();

            if (!string.IsNullOrEmpty(value))
            {
                var authorsArr = value.Split(",").Select(part => part.Trim()).ToArray();

                foreach (var authorName in authorsArr)
                {
                    var author = await _context.Authors.FirstOrDefaultAsync(a => a.FullName == authorName, cancellationToken);

                    if (author == null)
                    {
                        string[] nameParts = authorName.Split(' ');
                        string firstName = nameParts[0];
                        string lastName = nameParts.Length > 1 ? nameParts[1] : "";

                        author = new Author
                        {
                            FullName = authorName,
                            FirstName = firstName,
                            LastName = lastName
                        };
                        _context.Authors.Add(author);
                    }
                    authors.Add(author);
                }
            }
            else
            {
                int rowNumber = row.RowNumber();
                _isValid = false;
                _validationErrorStr += $"Автори обов'язкові! Помилка в рядку {rowNumber}.\n";
            }

            return authors;
        }
    }

}
