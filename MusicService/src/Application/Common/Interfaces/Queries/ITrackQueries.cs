using Domain.Tracks;
using LanguageExt;

namespace Application.Common.Interfaces.Queries;

public interface ITrackQueries
{
    Task<IReadOnlyList<Track>> GetAll(CancellationToken cancellationToken);
    Task<Option<Track>> GetById(TrackId id, CancellationToken cancellationToken);
    Task<IReadOnlyList<Track>> Filter(
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
        CancellationToken cancellationToken);
}