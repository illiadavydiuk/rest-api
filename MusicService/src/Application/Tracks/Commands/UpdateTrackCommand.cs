using Application.Common.Interfaces.Repositories;
using Application.Tracks.Exceptions;
using Domain.Tracks;
using LanguageExt;
using MediatR;
using Unit = LanguageExt.Unit;

namespace Application.Tracks.Commands;

public record UpdateTrackCommand : IRequest<Either<TrackException, Track>>
{
    public required Guid TrackId { get; init; }
    public required string Isrc { get; init; }
    public required string Title { get; init; }
    public required string Artist { get; init; }
    public required int DurationSeconds { get; init; }
    public required Genre Genre { get; init; }
    public required bool IsExplicit { get; init; }
    public required DateOnly ReleaseDate { get; init; }
}

public class UpdateTrackCommandHandler(ITrackRepository trackRepository)
    : IRequestHandler<UpdateTrackCommand, Either<TrackException, Track>>
{
    public async Task<Either<TrackException, Track>> Handle(
        UpdateTrackCommand request,
        CancellationToken cancellationToken)
    {
        var trackId = new TrackId(request.TrackId);
        var track = await trackRepository.GetById(trackId, cancellationToken);

        return await track.MatchAsync(
            p => CheckDuplicates(p.Id, request.Isrc, cancellationToken)
                .BindAsync(_ => UpdateEntity(p, request, cancellationToken)),
            () => new TrackNotFoundException(trackId));
    }

    private async Task<Either<TrackException, Unit>> CheckDuplicates(
        TrackId currentTrackId,
        string isrc,
        CancellationToken cancellationToken)
    {
        var track = await trackRepository.GetByIsrc(isrc, cancellationToken);

        return track.Match<Either<TrackException, Unit>>(
            t => t.Id == currentTrackId ? Unit.Default : new TrackAlreadyExistsException(t.Id, t.Isrc),
            () => Unit.Default);
    }

    private async Task<Either<TrackException, Track>> UpdateEntity(
        Track track,
        UpdateTrackCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            track.UpdateDetails(
                request.Isrc,
                request.Title,
                request.Artist,
                request.DurationSeconds,
                request.Genre,
                request.IsExplicit,
                request.ReleaseDate);

            return await trackRepository.Update(track, cancellationToken);
        }
        catch (Exception exception)
        {
            return new TrackUnhandledException(track.Id, exception);
        }
    }
}