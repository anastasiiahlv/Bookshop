using BookshopDomain.Model;
using DocumentFormat.OpenXml.Vml.Office;

namespace BookshopInfrastructure.Services
{
    public class BookDataPortServiceFactory: IDataPortServiceFactory<Book>
    {
        private readonly DbbookshopContext _context;
        public BookDataPortServiceFactory(DbbookshopContext context)
        {
            _context = context;
        }
        public IImportService<Book> GetImportService(string contentType)
        {
            if (contentType is "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            {
                return new BookImportService(_context);
            }
            throw new NotImplementedException($"Не розроблений сервіс імпорту книг з типом контнету {contentType}");
        }
        public IExportService<Book> GetExportService(string contentType)
        {
            if (contentType is "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            {
                return new BookExportService(_context);
            }
            throw new NotImplementedException($"Не розроблений сервіс імпорту екскурсій з типом контнету {contentType}");
        }

    }
}
