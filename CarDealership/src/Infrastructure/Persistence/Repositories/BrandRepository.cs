using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Domain.Brands;
using LanguageExt;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class BrandRepository(ApplicationDbContext context) : IBrandRepository, IBrandQueries
{
    public async Task<IReadOnlyList<Brand>> GetAll(
        string? name,
        string? country,
        CancellationToken cancellationToken)
    {
        var query = context.Brands
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(name))
        {
            query = query.Where(b => b.Name.ToLower().Contains(name.ToLower()));
        }

        if (!string.IsNullOrWhiteSpace(country))
        {
            query = query.Where(b => b.Country.ToLower().Contains(country.ToLower()));
        }

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<Option<Brand>> GetById(BrandId id, CancellationToken cancellationToken)
    {
        var entity = await context.Brands
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        return entity ?? Option<Brand>.None;
    }

    public async Task<Option<Brand>> GetByName(string name, CancellationToken cancellationToken)
    {
        var entity = await context.Brands
            .FirstOrDefaultAsync(x => x.Name.ToLower() == name.ToLower(), cancellationToken);

        return entity ?? Option<Brand>.None;
    }

    public async Task<Brand> Add(Brand brand, CancellationToken cancellationToken)
    {
        await context.Brands.AddAsync(brand, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        return brand;
    }

    public async Task<Brand> Update(Brand brand, CancellationToken cancellationToken)
    {
        context.Brands.Update(brand);
        await context.SaveChangesAsync(cancellationToken);
        return brand;
    }

    public async Task Delete(Brand brand, CancellationToken cancellationToken)
    {
        context.Brands.Remove(brand);
        await context.SaveChangesAsync(cancellationToken);
    }
}