namespace Domain.Tracks;

public class Track
{
    public TrackId Id { get; }
    public string Isrc { get; private set; }
    public string Title { get; private set; }
    public string Artist { get; private set; }
    public int DurationSeconds { get; private set; }
    public Genre Genre { get; private set; }
    public bool IsExplicit { get; private set; }
    public DateOnly ReleaseDate { get; private set;}
    public DateTime CreatedAt { get; }

    private Track(
        TrackId id,
        string isrc,
        string title,
        string artist,
        int durationSeconds,
        Genre genre,
        bool isExplicit,
        DateOnly releaseDate,
        DateTime createdAt)
    {
        Id = id;
        Isrc = isrc;
        Title = title;
        Artist = artist;
        DurationSeconds = durationSeconds;
        Genre = genre;
        IsExplicit = isExplicit;
        ReleaseDate = releaseDate;
        CreatedAt = createdAt;
    }

    public static Track New(
        TrackId id,
        string isrc,
        string title,
        string artist,
        int durationSeconds,
        Genre genre,
        bool isExplicit,
        DateOnly releaseDate)
    {
        return new Track(id, isrc, title, artist, durationSeconds, genre, isExplicit, releaseDate, DateTime.UtcNow);
    }

    public void UpdateDetails(
        string isrc,
        string title,
        string artist,
        int durationSeconds,
        Genre genre,
        bool isExplicit,
        DateOnly releaseDate)
    {
        Isrc = isrc;
        Title = title;
        Artist = artist;
        DurationSeconds = durationSeconds;
        Genre = genre;
        IsExplicit = isExplicit;
        ReleaseDate = releaseDate;
    }
}