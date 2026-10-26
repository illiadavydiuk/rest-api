using Application.Common.Interfaces.Repositories;
using Application.Tracks.Exceptions;
using Domain.Tracks;
using LanguageExt;
using MediatR;

namespace Application.Tracks.Commands;

public record DeleteTrackCommand : IRequest<Either<TrackException, Track>>
{
    public required Guid TrackId { get; init; }
}

public class DeleteTrackCommandHandler(ITrackRepository trackRepository)
    : IRequestHandler<DeleteTrackCommand, Either<TrackException, Track>>
{
    public async Task<Either<TrackException, Track>> Handle(
        DeleteTrackCommand request,
        CancellationToken cancellationToken)
    {
        var trackId = new TrackId(request.TrackId);
        var trackOption = await trackRepository.GetById(trackId, cancellationToken);

        return await trackOption.MatchAsync<Track, Either<TrackException, Track>>(
            async c =>
            {
                await trackRepository.Delete(c, cancellationToken);
                return c;
            },
            () => new TrackNotFoundException(trackId));
    }
}