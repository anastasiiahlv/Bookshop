using BookshopDomain.Model;

namespace BookshopInfrastructure.Services
{
    public interface IImportService<TEntity>
         where TEntity : Entity
    {
        Task ImportFromStreamAsync(Stream stream, CancellationToken cancellationToken);
    }

}
