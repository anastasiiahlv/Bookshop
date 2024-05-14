using BookshopDomain.Model;

namespace BookshopInfrastructure.Services
{
    public interface IImportService<TEntity>
         where TEntity : Entity
    {
        Task<string> ImportFromStreamAsync(Stream stream, CancellationToken cancellationToken);
    }

}
