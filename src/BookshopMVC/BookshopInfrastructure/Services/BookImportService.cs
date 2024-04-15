using BookshopDomain.Model;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Vml.Office;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace BookshopInfrastructure.Services
{
    public class BookImportService: IImportService<Book>
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

        public async Task ImportFromStreamAsync(Stream stream, CancellationToken cancellationToken)
        {
            if (!stream.CanRead)
            {
                throw new ArgumentException("Дані не можуть бути прочитані", nameof(stream));
            }

            using (XLWorkbook workBook = new XLWorkbook(stream))
            {
                foreach (IXLWorksheet worksheet in workBook.Worksheets)
                {
                    foreach (var row in worksheet.RowsUsed().Skip(1))
                    {
                        await AddBookAsync(row, cancellationToken);
                    }
                }
            }
            await _context.SaveChangesAsync(cancellationToken);
        }

        private async Task AddBookAsync(IXLRow row, CancellationToken cancellationToken)
        {
            Book? book = _context.Books.FirstOrDefault(b => b.Title == GetBookTitle(row));

            if (book != null)
            {
                book.Description = GetBookDescription(row);
                book.Price = GetBookPrice(row);
                book.PublisherId = GetPublisherId(row);
                _context.Books.Add(book);
                await GetAuthorsAsync(row, book, cancellationToken);
                await GetCategoriesAsync(row, book, cancellationToken);
            }
            else if (_isValid)
            {
                Book _book = new Book
                {
                    Title = GetBookTitle(row),
                    Description = GetBookDescription(row),
                    PublisherId = GetPublisherId(row),
                    Price = GetBookPrice(row)
                };

                _context.Books.Add(_book);
                await GetAuthorsAsync(row, _book, cancellationToken);
                await GetCategoriesAsync(row, _book, cancellationToken);
            }

            if (!_isValid)
            {
                throw new ArgumentException(_validationErrorStr);
            }
        }

        private static string GetBookTitle(IXLRow row)
        {
            int rowNumber = row.RowNumber();
            string value = row.Cell(1).Value.ToString();

            if (string.IsNullOrEmpty(value))
            {
                _isValid = false;
                _validationErrorStr += $"Назва книги обов'язкова! Помилка в рядку {rowNumber}.\n";
                return "";
            }

            return value;
        }

        private static string GetBookDescription(IXLRow row)
        {
            int rowNumber = row.RowNumber();
            string value = row.Cell(3).Value.ToString();

            if (string.IsNullOrEmpty(value))
            {
                _isValid = false;
                _validationErrorStr += $"Опис книги обов'язковий! Помилку в рядку {rowNumber}.\n";
                return "";
            }

            return value;
        }

        private static double GetBookPrice(IXLRow row)
        {
            int rowNumber = row.RowNumber();
            string valueStr = row.Cell(5).Value.ToString();

            if (string.IsNullOrEmpty(valueStr))
            {
                _isValid = false;
                _validationErrorStr += $"Ціна книги обов'язкова! Помилка в рядку {rowNumber}.\n";
                return 0;
            }

            double value;

            if (double.TryParse(valueStr, out value))
            {
                if (value < 0.01 || value > 20000.00)
                {
                    _isValid = false;
                    _validationErrorStr += $"Ціна має бути більше 0.01 і менше 20000.00. Помилка в рядку {rowNumber}.\n";
                    return 0;
                }

                Regex regex = new Regex(@"^\d+(\.\d{1,2})?$");
                if (!regex.IsMatch(valueStr))
                {
                    _isValid = false;
                    _validationErrorStr += $"Неправильний формат ціни. Помилка в рядку {rowNumber}.\n";
                    return 0;
                }

                return value;
            }
            else
            {
                _isValid = false;
                _validationErrorStr += $"Помилка в ціні в рядку {rowNumber}.\n";
                return 0;
            }
        }

        private int GetPublisherId(IXLRow row)
        {
            int rowNumber = row.RowNumber();
            var publisherName = row.Cell(4).Value.ToString().Trim();

            if (!string.IsNullOrWhiteSpace(publisherName))
            {
                var publisher = _context.Publishers.FirstOrDefault(p => p.Name.Equals(publisherName));

                if (publisher != null)
                {
                    return publisher.Id;
                }
                else
                {
                    publisher = new Publisher { Name = publisherName };
                    _context.Publishers.Add(publisher);
                    _context.SaveChanges(); 

                    return publisher.Id;
                }
                /*else
                {
                    _isValid = false;
                    _validationErrorStr += $"Видавництва '{publisherName}' не знайдено. Помилка в рядку {rowNumber}.\n";
                    return -1;
                }*/
            }
            else
            {
                _isValid = false;
                _validationErrorStr += $"Назва видавництва обов'язкова! Помилка в рядку {rowNumber}.\n";
                return -1;
            }
        }

        private async Task GetCategoriesAsync(IXLRow row, Book book, CancellationToken cancellationToken)
        {
            string value = row.Cell(6).Value.ToString();

            if (!string.IsNullOrEmpty(value))
            {
                var categoriesArr = value.Split(",").Select(part => part.Trim()).ToArray();

                for (int i = 0; i < categoriesArr.Length; i++)
                {
                    var categoryName = categoriesArr[i];
                    var category = await _context.Categories.FirstOrDefaultAsync(cat => cat.Name == categoryName, cancellationToken);

                    if (category is null)
                    {
                        category = new Category();
                        category.Name = categoryName;
                        _context.Categories.Add(category);
                    }

                    book.Categories.Add(category);
                }
            }
            else
            {
                int rowNumber = row.RowNumber();
                _isValid = false;
                _validationErrorStr += $"Категорії обов'язкові! Помилка в рядку {rowNumber}.\n";
            }
        }

        private async Task GetAuthorsAsync(IXLRow row, Book book, CancellationToken cancellationToken)
        {
            string value = row.Cell(2).Value.ToString();

            if (!string.IsNullOrEmpty(value))
            {
                var authorsArr = value.Split(",").Select(part => part.Trim()).ToArray();

                for (int i = 0; i < authorsArr.Length; i++)
                {
                    var authorName = authorsArr[i];
                    var author = await _context.Authors.FirstOrDefaultAsync(a => a.FullName == authorName, cancellationToken);

                    if (author is null)
                    {
                        string[] nameParts = authorName.Split(' ');
                        string firstName = nameParts[0];
                        string lastName = nameParts.Length > 1 ? nameParts[1] : "";

                        author = new Author();
                        author.FullName = authorName;
                        author.FirstName = firstName;
                        author.LastName = lastName;
                        _context.Authors.Add(author);
                    }
                    book.Authors.Add(author);
                }
            }
            else
            {
                int rowNumber = row.RowNumber();
                _isValid = false;
                _validationErrorStr += $"Автори обов'язкові! Помилка в рядку {rowNumber}.\n";
            }
        }
    }
}
