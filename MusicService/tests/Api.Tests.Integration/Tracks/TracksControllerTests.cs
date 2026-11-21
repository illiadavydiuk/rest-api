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

public class TracksControllerTests : BaseIntegrationTest, IAsyncLifetime
{
    private readonly Track _firstTestTrack;
    private readonly Track _secondTestTrack;
    private readonly Track _thirdTestTrack;
    private readonly string _baseRoute;
    private readonly string _secondTrackRoute;

    public TracksControllerTests(IntegrationTestWebFactory factory) : base(factory)
    {
        _firstTestTrack = TracksTestData.FirstTestTrack();
        _secondTestTrack = TracksTestData.SecondTestTrack();
        _thirdTestTrack = TracksTestData.ThirdTestTrack();
        _baseRoute = "tracks";
        _secondTrackRoute = $"tracks/{_secondTestTrack.Id}";
    }

    public async Task InitializeAsync()
    {
        await Context.Tracks.AddAsync(_secondTestTrack);
        await Context.SaveChangesAsync();
        Context.ChangeTracker.Clear();
    }

    public async Task DisposeAsync()
    {
        Context.Tracks.RemoveRange(Context.Tracks);
        await Context.SaveChangesAsync();
        Context.ChangeTracker.Clear();
    }

    [Fact]
    public async Task ShouldCreateTrack()
    {
        // Arrange
        var request = new CreateTrackDto(
            _firstTestTrack.Isrc,
            _firstTestTrack.Title,
            _firstTestTrack.Artist,
            _firstTestTrack.DurationSeconds,
            _firstTestTrack.Genre,
            _firstTestTrack.IsExplicit,
            _firstTestTrack.ReleaseDate);

        // Act
        var response = await Client.PostAsJsonAsync(_baseRoute, request);

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        var responseDto = await ReadFromJsonAsync<TrackDto>(response);
        responseDto.Should().NotBeNull();
        responseDto!.Id.Should().NotBeEmpty();

        var dbTrack = await Context.Tracks
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == new TrackId(responseDto.Id));

        dbTrack.Should().NotBeNull();
        dbTrack!.Isrc.Should().Be(request.Isrc);
        dbTrack.Title.Should().Be(request.Title);
        dbTrack.Artist.Should().Be(request.Artist);
        dbTrack.DurationSeconds.Should().Be(request.DurationSeconds);
        dbTrack.Genre.Should().Be(request.Genre);
        dbTrack.IsExplicit.Should().Be(request.IsExplicit);
        dbTrack.ReleaseDate.Should().Be(request.ReleaseDate);
    }

    [Fact]
    public async Task ShouldNotCreateTrackBecauseIsrcAlreadyTaken()
    {
        // Arrange
        var request = new CreateTrackDto(
            _secondTestTrack.Isrc,
            _firstTestTrack.Title,
            _firstTestTrack.Artist,
            _firstTestTrack.DurationSeconds,
            _firstTestTrack.Genre,
            _firstTestTrack.IsExplicit,
            _firstTestTrack.ReleaseDate);

        // Act
        var response = await Client.PostAsJsonAsync(_baseRoute, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);

        var dbTrack = await Context.Tracks.AsNoTracking().SingleAsync();
        dbTrack.Id.Should().Be(_secondTestTrack.Id);
    }

    [Fact]
    public async Task ShouldNotCreateTrackBecauseRequestIsInvalid()
    {
        // Arrange
        var request = new CreateTrackDto(
            "",
            "",
            "Artist",
            0,
            Genre.Pop,
            false,
            new DateOnly(2020, 1, 1));

        // Act
        var response = await Client.PostAsJsonAsync(_baseRoute, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var dbTrack = await Context.Tracks.AsNoTracking().SingleAsync();
        dbTrack.Id.Should().Be(_secondTestTrack.Id);
    }

    [Fact]
    public async Task ShouldUpdateTrack()
    {
        // Arrange
        var request = new UpdateTrackDto(
            _thirdTestTrack.Isrc,
            _thirdTestTrack.Title,
            _thirdTestTrack.Artist,
            _thirdTestTrack.DurationSeconds,
            _thirdTestTrack.Genre,
            _thirdTestTrack.IsExplicit,
            _thirdTestTrack.ReleaseDate);

        // Act
        var response = await Client.PutAsJsonAsync(_secondTrackRoute, request);

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        var responseDto = await ReadFromJsonAsync<TrackDto>(response);
        responseDto.Should().NotBeNull();
        responseDto!.Id.Should().Be(_secondTestTrack.Id.Value);

        var dbTrack = await Context.Tracks
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == _secondTestTrack.Id);

        dbTrack.Should().NotBeNull();
        dbTrack!.Isrc.Should().Be(_thirdTestTrack.Isrc);
        dbTrack.Title.Should().Be(_thirdTestTrack.Title);
        dbTrack.Artist.Should().Be(_thirdTestTrack.Artist);
        dbTrack.DurationSeconds.Should().Be(_thirdTestTrack.DurationSeconds);
        dbTrack.Genre.Should().Be(_thirdTestTrack.Genre);
        dbTrack.IsExplicit.Should().Be(_thirdTestTrack.IsExplicit);
        dbTrack.ReleaseDate.Should().Be(_thirdTestTrack.ReleaseDate);
    }

    [Fact]
    public async Task ShouldUpdateTrackWithSameIsrc()
    {
        // Arrange
        var request = new UpdateTrackDto(
            _secondTestTrack.Isrc,
            "Updated Title",
            _secondTestTrack.Artist,
            300,
            _secondTestTrack.Genre,
            _secondTestTrack.IsExplicit,
            _secondTestTrack.ReleaseDate);

        // Act
        var response = await Client.PutAsJsonAsync(_secondTrackRoute, request);

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();

        var dbTrack = await Context.Tracks
            .AsNoTracking()
            .FirstAsync(x => x.Id == _secondTestTrack.Id);

        dbTrack.Isrc.Should().Be(_secondTestTrack.Isrc);
        dbTrack.Title.Should().Be("Updated Title");
        dbTrack.DurationSeconds.Should().Be(300);
    }

    [Fact]
    public async Task ShouldNotUpdateTrackBecauseIsrcAlreadyTaken()
    {
        // Arrange: для конфлікту ISRC потрібен ще один трек у БД.
        // Додаємо його тут, щоб InitializeAsync не отримав ISRC, який зламає ShouldCreateTrack.
        await Context.Tracks.AddAsync(_firstTestTrack);
        await Context.SaveChangesAsync();
        Context.ChangeTracker.Clear();

        var request = new UpdateTrackDto(
            _firstTestTrack.Isrc,
            "New Title",
            "New Artist",
            220,
            Genre.Pop,
            false,
            new DateOnly(2021, 1, 1));

        // Act
        var response = await Client.PutAsJsonAsync(_secondTrackRoute, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);

        var dbSecond = await Context.Tracks
            .AsNoTracking()
            .FirstAsync(x => x.Id == _secondTestTrack.Id);

        dbSecond.Isrc.Should().Be(_secondTestTrack.Isrc);
        dbSecond.Title.Should().Be(_secondTestTrack.Title);
        dbSecond.Artist.Should().Be(_secondTestTrack.Artist);
        dbSecond.DurationSeconds.Should().Be(_secondTestTrack.DurationSeconds);
        dbSecond.Genre.Should().Be(_secondTestTrack.Genre);
        dbSecond.IsExplicit.Should().Be(_secondTestTrack.IsExplicit);
        dbSecond.ReleaseDate.Should().Be(_secondTestTrack.ReleaseDate);
    }

    [Fact]
    public async Task ShouldNotUpdateTrackBecauseTrackNotFound()
    {
        // Arrange
        var nonExistentRoute = $"tracks/{Guid.NewGuid()}";
        var request = new UpdateTrackDto(
            _thirdTestTrack.Isrc,
            _thirdTestTrack.Title,
            _thirdTestTrack.Artist,
            _thirdTestTrack.DurationSeconds,
            _thirdTestTrack.Genre,
            _thirdTestTrack.IsExplicit,
            _thirdTestTrack.ReleaseDate);

        // Act
        var response = await Client.PutAsJsonAsync(nonExistentRoute, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ShouldNotUpdateTrackBecauseRequestIsInvalid()
    {
        // Arrange
        var request = new UpdateTrackDto(
            "",
            "",
            "",
            -5,
            Genre.Rock,
            false,
            new DateOnly(2020, 1, 1));

        // Act
        var response = await Client.PutAsJsonAsync(_secondTrackRoute, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var dbTrack = await Context.Tracks
            .AsNoTracking()
            .FirstAsync(x => x.Id == _secondTestTrack.Id);

        dbTrack.Isrc.Should().Be(_secondTestTrack.Isrc);
        dbTrack.Title.Should().Be(_secondTestTrack.Title);
        dbTrack.Artist.Should().Be(_secondTestTrack.Artist);
        dbTrack.DurationSeconds.Should().Be(_secondTestTrack.DurationSeconds);
        dbTrack.Genre.Should().Be(_secondTestTrack.Genre);
        dbTrack.IsExplicit.Should().Be(_secondTestTrack.IsExplicit);
        dbTrack.ReleaseDate.Should().Be(_secondTestTrack.ReleaseDate);
    }
    

    [Fact]
    public async Task ShouldGetAllTracks()
    {
        // Arrange
        var dbTracks = await Context.Tracks.AsNoTracking().ToListAsync();

        // Act
        var response = await Client.GetAsync(_baseRoute);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var dtos = await ReadFromJsonAsync<List<TrackDto>>(response);
        dtos.Should().NotBeNull();
        dtos!.Should().HaveCount(dbTracks.Count);
        dtos.Select(x => x.Id).Should().BeEquivalentTo(dbTracks.Select(x => x.Id.Value));
    }

    [Fact]
    public async Task ShouldGetTrackById()
    {
        // Arrange
        var dbTrack = await Context.Tracks
            .AsNoTracking()
            .FirstAsync(x => x.Id == _secondTestTrack.Id);

        // Act
        var response = await Client.GetAsync(_secondTrackRoute);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var dto = await ReadFromJsonAsync<TrackDto>(response);
        dto.Should().NotBeNull();
        dto!.Id.Should().Be(dbTrack.Id.Value);
        dto.Isrc.Should().Be(dbTrack.Isrc);
        dto.Title.Should().Be(dbTrack.Title);
        dto.Artist.Should().Be(dbTrack.Artist);
        dto.DurationSeconds.Should().Be(dbTrack.DurationSeconds);
        dto.Genre.Should().Be(dbTrack.Genre);
        dto.IsExplicit.Should().Be(dbTrack.IsExplicit);
        dto.ReleaseDate.Should().Be(dbTrack.ReleaseDate);
    }

    [Fact]
    public async Task ShouldNotGetTrackByIdBecauseTrackNotFound()
    {
        // Arrange
        var nonExistentRoute = $"tracks/{Guid.NewGuid()}";

        // Act
        var response = await Client.GetAsync(nonExistentRoute);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}