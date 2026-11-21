using Domain.Tracks;

namespace Tests.Data.Tracks;

public static class TracksTestData
{
    public static Track FirstTestTrack() => Track.New(
        TrackId.New(),
        isrc: "USIR20400138",
        title: "Mockingbird",
        artist: "Eminem",
        durationSeconds: 250,
        genre: Genre.HipHop,
        isExplicit: true,
        releaseDate: new DateOnly(2004, 11, 12));

    public static Track SecondTestTrack() => Track.New(
        TrackId.New(),
        isrc: "GBAYE6700035",
        title: "Bohemian Rhapsody",
        artist: "Queen",
        durationSeconds: 354,
        genre: Genre.Rock,
        isExplicit: false,
        releaseDate: new DateOnly(1975, 10, 31));

    public static Track ThirdTestTrack() => Track.New(
        TrackId.New(),
        isrc: "USUM71703847",
        title: "Blinding Lights",
        artist: "The Weeknd",
        durationSeconds: 200,
        genre: Genre.Pop,
        isExplicit: false,
        releaseDate: new DateOnly(2020, 3, 20));

    public static Track FourthTestTrack() => Track.New(
        TrackId.New(),
        isrc: "USUG11801293",
        title: "SICKO MODE",
        artist: "Travis Scott",
        durationSeconds: 312,
        genre: Genre.HipHop,
        isExplicit: true,
        releaseDate: new DateOnly(2018, 8, 3));
}