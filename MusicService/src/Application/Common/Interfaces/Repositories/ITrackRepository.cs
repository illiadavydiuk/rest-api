using Domain.Tracks;
using LanguageExt;

namespace Application.Common.Interfaces.Repositories;

public interface ITrackRepository
{
    Task<Option<Track>> GetById(TrackId id, CancellationToken cancellationToken);
    Task<Option<Track>> GetByIsrc(string isrc, CancellationToken cancellationToken);
    Task<Track> Add(Track track, CancellationToken cancellationToken);
    Task<Track> Update(Track track, CancellationToken cancellationToken);
    Task Delete(Track track, CancellationToken cancellationToken);
}