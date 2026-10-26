using Domain.Tracks;

namespace Application.Tracks.Exceptions;

public abstract class TrackException(TrackId trackId, string message, Exception? innerException = null)
    : Exception(message, innerException)
{
    public TrackId TrackId { get; } = trackId;
}

public class TrackAlreadyExistsException(TrackId trackId, string isrc)
    : TrackException(trackId, $"Track with ISRC '{isrc}' already exists.")
{
    public string Isrc { get; } = isrc;
}

public class TrackNotFoundException(TrackId trackId)
    : TrackException(trackId, $"Track not found under id '{trackId}'.");

public class TrackUnhandledException(TrackId trackId, Exception? innerException = null)
    : TrackException(trackId, $"Unhandled exception under id '{trackId}'.", innerException);