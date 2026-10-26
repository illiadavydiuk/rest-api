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
}