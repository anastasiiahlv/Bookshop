using BookshopDomain.Model;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Vml.Office;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace BookshopInfrastructure.Services
{
    public class BookExportService : IExportService<Book>
    {
        private readonly DbbookshopContext _context;

        public BookExportService(DbbookshopContext context)
        {
            _context = context;
        }

        private static readonly IReadOnlyList<string> HeaderNames = new string[]
        {
            "Назва",
            "Автори",
            "Опис",
            "Видавництво",
            "Ціна",
            "Категорії"
        };

        private static void WriteHeader(IXLWorksheet worksheet)
        {
            for (int columnIndex = 0; columnIndex < HeaderNames.Count; columnIndex++)
            {
                worksheet.Cell(1, columnIndex + 1).Value = HeaderNames[columnIndex];
                worksheet.Column(columnIndex + 1).Width = 25;
            }
            worksheet.Row(1).Style.Font.Bold = true;
        }

        private void WriteBook(IXLWorksheet worksheet, Book book, int rowIndex)
        {
            var columnIndex = 1;
            worksheet.Cell(rowIndex, columnIndex++).Value = book.Title;
            worksheet.Cell(rowIndex, columnIndex++).Value = string.Join(", ", book.Authors.Select(a => a.FullName));
            worksheet.Cell(rowIndex, columnIndex++).Value = book.Description;
            worksheet.Cell(rowIndex, columnIndex++).Value = book.Publisher.Name;
            worksheet.Cell(rowIndex, columnIndex++).Value = book.Price;
            worksheet.Cell(rowIndex, columnIndex++).Value = string.Join(", ", book.Categories.Select(c => c.Name));
        }

        private void WriteBooks(IXLWorksheet worksheet, ICollection<Book> books)
        {
            WriteHeader(worksheet);
            int rowIndex = 2;
            foreach (var book in books)
            {
                WriteBook(worksheet, book, rowIndex++);
            }
        }

        public async Task WriteToAsync(Stream stream, CancellationToken cancellationToken)
        {
            if (!stream.CanWrite)
            {
                throw new ArgumentException("Cannot write to the file.");
            }

            var books = await _context.Books
                .Include(b => b.Categories)
                .Include(b => b.Publisher)
                .Include(b => b.Authors)
                .ToListAsync(cancellationToken);

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Books");
            WriteBooks(worksheet, books);

            workbook.SaveAs(stream);
        }
    }
}
