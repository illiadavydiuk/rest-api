using Domain.Tracks;
using LanguageExt;

namespace Application.Common.Interfaces.Queries;

public interface ITrackQueries
{
    Task<IReadOnlyList<Track>> GetAll(CancellationToken cancellationToken);
    Task<Option<Track>> GetById(TrackId id, CancellationToken cancellationToken);
}