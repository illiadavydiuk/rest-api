using Domain.Brands;
using LanguageExt;

namespace Application.Common.Interfaces.Repositories;

public interface IBrandRepository
{
    Task<Option<Brand>> GetById(BrandId id, CancellationToken cancellationToken);
    Task<Option<Brand>> GetByName(string name, CancellationToken cancellationToken);
    Task<Brand> Add(Brand brand, CancellationToken cancellationToken);
    Task<Brand> Update(Brand brand, CancellationToken cancellationToken);
    Task Delete(Brand brand, CancellationToken cancellationToken);
}