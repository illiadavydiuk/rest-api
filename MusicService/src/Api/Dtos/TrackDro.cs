using Domain.Tracks;

namespace Api.Dtos;

public record TrackDto(
    Guid Id,
    string Isrc,
    string Title,
    string Artist,
    int DurationSeconds,
    Genre Genre,
    bool IsExplicit,
    DateOnly ReleaseDate,
    DateTime CreatedAt)
{
    public static TrackDto FromDomainModel(Track track)
        => new(
            track.Id.Value,
            track.Isrc,
            track.Title,
            track.Artist,
            track.DurationSeconds,
            track.Genre,
            track.IsExplicit,
            track.ReleaseDate,
            track.CreatedAt);
}

public record CreateTrackDto(
    string Isrc,
    string Title,
    string Artist,
    int DurationSeconds,
    Genre Genre,
    bool IsExplicit,
    DateOnly ReleaseDate);

public record UpdateTrackDto(
    string Isrc,
    string Title,
    string Artist,
    int DurationSeconds,
    Genre Genre,
    bool IsExplicit,
    DateOnly ReleaseDate);