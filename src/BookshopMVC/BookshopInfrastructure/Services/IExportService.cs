﻿using BookshopDomain.Model;

namespace BookshopInfrastructure.Services
{
    public interface IExportService<TEntity>
    where TEntity : Entity
    {
        Task WriteToAsync(Stream stream, CancellationToken cancellationToken);
    }

}
