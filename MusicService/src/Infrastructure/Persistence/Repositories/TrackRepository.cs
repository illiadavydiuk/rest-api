using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Domain.Tracks;
using LanguageExt;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class TrackRepository(ApplicationDbContext context) : ITrackRepository, ITrackQueries
{
    public async Task<IReadOnlyList<Track>> GetAll(CancellationToken cancellationToken)
    {
        return await context.Tracks.AsNoTracking().ToListAsync(cancellationToken);
    }

    public async Task<Option<Track>> GetById(TrackId id, CancellationToken cancellationToken)
    {
        var entity = await context.Tracks.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        return entity ?? Option<Track>.None;
    }

    public async Task<Option<Track>> GetByIsrc(string isrc, CancellationToken cancellationToken)
    {
        var entity = await context.Tracks.FirstOrDefaultAsync(x => x.Isrc == isrc, cancellationToken);
        return entity ?? Option<Track>.None;
    }

    public async Task<Track> Add(Track track, CancellationToken cancellationToken)
    {
        await context.Tracks.AddAsync(track, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        return track;
    }

    public async Task<Track> Update(Track track, CancellationToken cancellationToken)
    {
        context.Tracks.Update(track);
        await context.SaveChangesAsync(cancellationToken);
        return track;
    }
    

    public async Task Delete(Track track, CancellationToken cancellationToken)
    {
        context.Tracks.Remove(track);
        await context.SaveChangesAsync(cancellationToken);
    }
    
    public async Task<IReadOnlyList<Track>> Filter(
        string? isrc,
        string? title,
        string? artist,
        int? durationSecondsFrom,
        int? durationSecondsTo,
        Genre? genre,
        bool? isExplicit,
        DateOnly? releaseDateFrom,
        DateOnly? releaseDateTo,
        DateTime? createdAtFrom,
        DateTime? createdAtTo,
        CancellationToken cancellationToken)
    {
        var query = context.Tracks.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(isrc))
        {
            query = query.Where(x => EF.Functions.ILike(x.Isrc, $"%{isrc}%"));
        }

        if (!string.IsNullOrWhiteSpace(title))
        {
            query = query.Where(x => EF.Functions.ILike(x.Title, $"%{title}%"));
        }

        if (!string.IsNullOrWhiteSpace(artist))
        {
            query = query.Where(x => EF.Functions.ILike(x.Artist, $"%{artist}%"));
        }

        if (durationSecondsFrom.HasValue)
        {
            query = query.Where(x => x.DurationSeconds >= durationSecondsFrom.Value);
        }

        if (durationSecondsTo.HasValue)
        {
            query = query.Where(x => x.DurationSeconds <= durationSecondsTo.Value);
        }

        if (genre.HasValue)
        {
            query = query.Where(x => x.Genre == genre.Value);
        }

        if (isExplicit.HasValue)
        {
            query = query.Where(x => x.IsExplicit == isExplicit.Value);
        }

        if (releaseDateFrom.HasValue)
        {
            query = query.Where(x => x.ReleaseDate >= releaseDateFrom.Value);
        }

        if (releaseDateTo.HasValue)
        {
            query = query.Where(x => x.ReleaseDate <= releaseDateTo.Value);
        }

        if (createdAtFrom.HasValue)
        {
            var fromUtc = DateTime.SpecifyKind(createdAtFrom.Value, DateTimeKind.Utc);
            query = query.Where(x => x.CreatedAt >= fromUtc);
        }

        if (createdAtTo.HasValue)
        {
            var toUtc = DateTime.SpecifyKind(createdAtTo.Value, DateTimeKind.Utc);
            query = query.Where(x => x.CreatedAt <= toUtc);
        }

        return await query.ToListAsync(cancellationToken);
    }
}