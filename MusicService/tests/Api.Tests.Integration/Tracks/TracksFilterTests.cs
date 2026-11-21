using System.Net;
using System.Net.Http.Json;
using Api.Dtos;
using Domain.Tracks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Tests.Common;
using Tests.Data.Tracks;
using Xunit;

namespace Api.Tests.Integration.Tracks;

public class TracksFilterTests : BaseIntegrationTest, IAsyncLifetime
{
    private readonly Track _firstTrack;  // Eminem - Mockingbird
    private readonly Track _secondTrack; // Queen - Bohemian Rhapsody
    private readonly Track _thirdTrack;  // The Weeknd - Blinding Lights
    private readonly Track _fourthTrack; // Travis Scott - SICKO MODE
    private readonly string _filterRoute;

    public TracksFilterTests(IntegrationTestWebFactory factory) : base(factory)
    {
        _firstTrack = TracksTestData.FirstTestTrack();
        _secondTrack = TracksTestData.SecondTestTrack();
        _thirdTrack = TracksTestData.ThirdTestTrack();
        _fourthTrack = TracksTestData.FourthTestTrack();
        _filterRoute = "tracks/filter";
    }

    private static readonly DateTime BaseCreatedAt = new(2026, 1, 1, 12, 0, 0, DateTimeKind.Utc);

    public async Task InitializeAsync()
    {
        var tracks = new[] { _firstTrack, _secondTrack, _thirdTrack, _fourthTrack };
        
        for (var i = 0; i < tracks.Length; i++)
        {
            Context.Entry(tracks[i]).Property("CreatedAt").CurrentValue = BaseCreatedAt.AddMinutes(i);
        }

        await Context.Tracks.AddRangeAsync(tracks);
        await Context.SaveChangesAsync();
        Context.ChangeTracker.Clear();
    }

    public async Task DisposeAsync()
    {
        Context.Tracks.RemoveRange(Context.Tracks);
        await Context.SaveChangesAsync();
        Context.ChangeTracker.Clear();
    }

    private static SearchTrackDto EmptyFilter() => new(
        null, null, null, null, null, null, null, null, null, null, null);

    private async Task<List<Track>> GetTracksOrderedByCreatedAtAsync()
    {
        var tracks = await Context.Tracks
            .AsNoTracking()
            .OrderBy(x => x.CreatedAt)
            .ThenBy(x => x.Id)
            .ToListAsync();

        tracks.Should().HaveCount(4);
        tracks.Select(x => x.CreatedAt).Distinct().Should().HaveCount(4,
            "CreatedAt має відрізнятися для кожного треку, інакше межі діапазону не визначені");

        return tracks;
    }

    [Fact]
    public async Task ShouldFilterByIsrcPartialMatch()
    {
        // Arrange
        var filter = EmptyFilter() with { Isrc = "US" };

        // Act
        var response = await Client.PostAsJsonAsync(_filterRoute, filter);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await ReadFromJsonAsync<List<TrackDto>>(response);
        result!.Select(x => x.Id).Should().BeEquivalentTo([
            _firstTrack.Id.Value,
            _thirdTrack.Id.Value,
            _fourthTrack.Id.Value
        ]);
    }

    [Fact]
    public async Task ShouldFilterByTitlePartialMatch()
    {
        // Arrange
        var filter = EmptyFilter() with { Title = "Lights" };

        // Act
        var response = await Client.PostAsJsonAsync(_filterRoute, filter);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await ReadFromJsonAsync<List<TrackDto>>(response);
        result!.Select(x => x.Id).Should().BeEquivalentTo([_thirdTrack.Id.Value]);
    }

    [Fact]
    public async Task ShouldFilterByTitleCaseInsensitive()
    {
        // Arrange
        var filter = EmptyFilter() with { Title = "mocking" };

        // Act
        var response = await Client.PostAsJsonAsync(_filterRoute, filter);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await ReadFromJsonAsync<List<TrackDto>>(response);
        result!.Select(x => x.Id).Should().BeEquivalentTo([_firstTrack.Id.Value]);
    }

    [Fact]
    public async Task ShouldFilterByArtistPartialMatch()
    {
        // Arrange
        var filter = EmptyFilter() with { Artist = "Queen" };

        // Act
        var response = await Client.PostAsJsonAsync(_filterRoute, filter);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await ReadFromJsonAsync<List<TrackDto>>(response);
        result!.Select(x => x.Id).Should().BeEquivalentTo([_secondTrack.Id.Value]);
    }

    [Fact]
    public async Task ShouldFilterByDurationSecondsFromInclusive()
    {
        // Arrange
        var filter = EmptyFilter() with { DurationSecondsFrom = 312 };

        // Act
        var response = await Client.PostAsJsonAsync(_filterRoute, filter);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await ReadFromJsonAsync<List<TrackDto>>(response);
        result!.Select(x => x.Id).Should().BeEquivalentTo([
            _secondTrack.Id.Value,
            _fourthTrack.Id.Value
        ]);
    }

    [Fact]
    public async Task ShouldFilterByDurationSecondsToInclusive()
    {
        // Arrange
        var filter = EmptyFilter() with { DurationSecondsTo = 250 };

        // Act
        var response = await Client.PostAsJsonAsync(_filterRoute, filter);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await ReadFromJsonAsync<List<TrackDto>>(response);
        result!.Select(x => x.Id).Should().BeEquivalentTo([
            _firstTrack.Id.Value,
            _thirdTrack.Id.Value
        ]);
    }

    [Fact]
    public async Task ShouldFilterByDurationSecondsRange()
    {
        // Arrange
        var filter = EmptyFilter() with { DurationSecondsFrom = 250, DurationSecondsTo = 312 };

        // Act
        var response = await Client.PostAsJsonAsync(_filterRoute, filter);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await ReadFromJsonAsync<List<TrackDto>>(response);
        result!.Select(x => x.Id).Should().BeEquivalentTo([
            _firstTrack.Id.Value,
            _fourthTrack.Id.Value
        ]);
    }

    [Fact]
    public async Task ShouldFilterByGenre()
    {
        // Arrange
        var filter = EmptyFilter() with { Genre = Genre.HipHop };

        // Act
        var response = await Client.PostAsJsonAsync(_filterRoute, filter);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await ReadFromJsonAsync<List<TrackDto>>(response);
        result!.Select(x => x.Id).Should().BeEquivalentTo([
            _firstTrack.Id.Value,
            _fourthTrack.Id.Value
        ]);
    }

    [Fact]
    public async Task ShouldFilterByIsExplicit()
    {
        // Arrange
        var filter = EmptyFilter() with { IsExplicit = false };

        // Act
        var response = await Client.PostAsJsonAsync(_filterRoute, filter);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await ReadFromJsonAsync<List<TrackDto>>(response);
        result!.Select(x => x.Id).Should().BeEquivalentTo([
            _secondTrack.Id.Value,
            _thirdTrack.Id.Value
        ]);
    }

    [Fact]
    public async Task ShouldFilterByReleaseDateFromInclusive()
    {
        // Arrange
        var filter = EmptyFilter() with { ReleaseDateFrom = new DateOnly(2018, 8, 3) };

        // Act
        var response = await Client.PostAsJsonAsync(_filterRoute, filter);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await ReadFromJsonAsync<List<TrackDto>>(response);
        result!.Select(x => x.Id).Should().BeEquivalentTo([
            _thirdTrack.Id.Value,
            _fourthTrack.Id.Value
        ]);
    }

    [Fact]
    public async Task ShouldFilterByReleaseDateToInclusive()
    {
        // Arrange
        var filter = EmptyFilter() with { ReleaseDateTo = new DateOnly(2004, 11, 12) };

        // Act
        var response = await Client.PostAsJsonAsync(_filterRoute, filter);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await ReadFromJsonAsync<List<TrackDto>>(response);
        result!.Select(x => x.Id).Should().BeEquivalentTo([
            _firstTrack.Id.Value,
            _secondTrack.Id.Value
        ]);
    }

    [Fact]
    public async Task ShouldFilterByCreatedAtFromOnly()
    {
        // Arrange:
        var dbTracks = await GetTracksOrderedByCreatedAtAsync();

        var filter = EmptyFilter() with { CreatedAtFrom = dbTracks[2].CreatedAt };

        // Act
        var response = await Client.PostAsJsonAsync(_filterRoute, filter);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await ReadFromJsonAsync<List<TrackDto>>(response);
        result!.Select(x => x.Id).Should().BeEquivalentTo([
            dbTracks[2].Id.Value,
            dbTracks[3].Id.Value
        ]);
    }

    [Fact]
    public async Task ShouldFilterByCreatedAtToOnly()
    {
        // Arrange
        var dbTracks = await GetTracksOrderedByCreatedAtAsync();

        var filter = EmptyFilter() with { CreatedAtTo = dbTracks[1].CreatedAt };

        // Act
        var response = await Client.PostAsJsonAsync(_filterRoute, filter);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await ReadFromJsonAsync<List<TrackDto>>(response);
        result!.Select(x => x.Id).Should().BeEquivalentTo([
            dbTracks[0].Id.Value,
            dbTracks[1].Id.Value
        ]);
    }

    [Fact]
    public async Task ShouldFilterByCreatedAtFromAndTo()
    {
        // Arrange
        var dbTracks = await GetTracksOrderedByCreatedAtAsync();

        var filter = EmptyFilter() with
        {
            CreatedAtFrom = dbTracks[1].CreatedAt,
            CreatedAtTo = dbTracks[2].CreatedAt
        };

        // Act
        var response = await Client.PostAsJsonAsync(_filterRoute, filter);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await ReadFromJsonAsync<List<TrackDto>>(response);
        result!.Select(x => x.Id).Should().BeEquivalentTo([
            dbTracks[1].Id.Value,
            dbTracks[2].Id.Value
        ]);
    }

    [Fact]
    public async Task ShouldFilterByCombinedConditions()
    {
        // Arrange: очікується лише SICKO MODE
        var filter = EmptyFilter() with
        {
            Genre = Genre.HipHop,
            IsExplicit = true,
            ReleaseDateFrom = new DateOnly(2010, 1, 1)
        };

        // Act
        var response = await Client.PostAsJsonAsync(_filterRoute, filter);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await ReadFromJsonAsync<List<TrackDto>>(response);
        result!.Select(x => x.Id).Should().BeEquivalentTo([_fourthTrack.Id.Value]);
    }

    [Fact]
    public async Task ShouldReturnAllTracksWhenFilterIsEmpty()
    {
        // Arrange
        var filter = EmptyFilter();

        // Act
        var response = await Client.PostAsJsonAsync(_filterRoute, filter);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await ReadFromJsonAsync<List<TrackDto>>(response);
        result!.Select(x => x.Id).Should().BeEquivalentTo([
            _firstTrack.Id.Value,
            _secondTrack.Id.Value,
            _thirdTrack.Id.Value,
            _fourthTrack.Id.Value
        ]);
    }

    [Fact]
    public async Task ShouldReturnEmptyListWhenNoMatchesFound()
    {
        // Arrange
        var filter = EmptyFilter() with { Title = "NonExistentSongTitle12345" };

        // Act
        var response = await Client.PostAsJsonAsync(_filterRoute, filter);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await ReadFromJsonAsync<List<TrackDto>>(response);
        result.Should().NotBeNull();
        result!.Should().BeEmpty();
    }
}