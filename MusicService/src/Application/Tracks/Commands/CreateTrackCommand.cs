using Application.Common.Interfaces.Repositories;
using Application.Tracks.Exceptions;
using Domain.Tracks;
using LanguageExt;
using MediatR;

namespace Application.Tracks.Commands;

public record CreateTrackCommand : IRequest<Either<TrackException, Track>>
{
    public required string Isrc { get; init; }
    public required string Title { get; init; }
    public required string Artist { get; init; }
    public required int DurationSeconds { get; init; }
    public required Genre Genre { get; init; }
    public required bool IsExplicit { get; init; }
    public required DateOnly ReleaseDate { get; init; }
}

public class CreateTrackCommandHandler(ITrackRepository trackRepository)
    : IRequestHandler<CreateTrackCommand, Either<TrackException, Track>>
{
    public async Task<Either<TrackException, Track>> Handle(
        CreateTrackCommand request,
        CancellationToken cancellationToken)
    {
        var existingTrack = await trackRepository.GetByIsrc(request.Isrc, cancellationToken);

        return await existingTrack.MatchAsync(
            t => new TrackAlreadyExistsException(t.Id, t.Isrc),
            () => CreateEntity(request, cancellationToken));
    }

    private async Task<Either<TrackException, Track>> CreateEntity(
        CreateTrackCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var track = Track.New(
                TrackId.New(),
                request.Isrc,
                request.Title,
                request.Artist,
                request.DurationSeconds,
                request.Genre,
                request.IsExplicit,
                request.ReleaseDate);

            return await trackRepository.Add(track, cancellationToken);
        }
        catch (Exception ex)
        {
            return new TrackUnhandledException(TrackId.Empty(), ex);
        }
    }
}