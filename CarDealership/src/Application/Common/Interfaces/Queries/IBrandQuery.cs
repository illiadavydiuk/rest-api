using Domain.Brands;
using LanguageExt;

namespace Application.Common.Interfaces.Queries;

public interface IBrandQueries
{
    Task<IReadOnlyList<Brand>> GetAll(string? name, string? country, CancellationToken cancellationToken);
    Task<Option<Brand>> GetById(BrandId id, CancellationToken cancellationToken);
}