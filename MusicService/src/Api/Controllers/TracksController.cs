using Api.Dtos;
using Api.Modules.Errors;
using Application.Common.Interfaces.Queries;
using Application.Tracks.Commands;
using Domain.Tracks;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("tracks")]
[ApiController]
public class TracksController(ISender sender, ITrackQueries trackQueries) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<TrackDto>>> GetTracks(CancellationToken cancellationToken)
    {
        var tracks = await trackQueries.GetAll(cancellationToken);
        return tracks.Select(TrackDto.FromDomainModel).ToList();
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TrackDto>> GetTrack(Guid id, CancellationToken cancellationToken)
    {
        var track = await trackQueries.GetById(new TrackId(id), cancellationToken);

        return track.Match<ActionResult<TrackDto>>(
            t => TrackDto.FromDomainModel(t),
            () => NotFound());
    }

    [HttpPost]
    public async Task<ActionResult<TrackDto>> CreateTrack(
        [FromBody] CreateTrackDto request,
        CancellationToken cancellationToken)
    {
        var input = new CreateTrackCommand
        {
            Isrc = request.Isrc,
            Title = request.Title,
            Artist = request.Artist,
            DurationSeconds = request.DurationSeconds,
            Genre = request.Genre,
            IsExplicit = request.IsExplicit,
            ReleaseDate = request.ReleaseDate
        };

        var result = await sender.Send(input, cancellationToken);

        return result.Match<ActionResult<TrackDto>>(
            t => CreatedAtAction(nameof(GetTrack), new { id = t.Id.Value }, 
                TrackDto.FromDomainModel(t)),
            e => e.ToObjectResult());
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<TrackDto>> UpdateTrack(
        Guid id,
        [FromBody] UpdateTrackDto request,
        CancellationToken cancellationToken)
    {
        var input = new UpdateTrackCommand
        {
            TrackId = id,
            Isrc = request.Isrc,
            Title = request.Title,
            Artist = request.Artist,
            DurationSeconds = request.DurationSeconds,
            Genre = request.Genre,
            IsExplicit = request.IsExplicit,
            ReleaseDate = request.ReleaseDate
        };

        var result = await sender.Send(input, cancellationToken);

        return result.Match<ActionResult<TrackDto>>(
            t => Ok(TrackDto.FromDomainModel(t)),
            e => e.ToObjectResult());
    }
    
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteTrack(Guid id, CancellationToken cancellationToken)
    {
        var input = new DeleteTrackCommand
        {
            TrackId = id
        };

        var result = await sender.Send(input, cancellationToken);

        return result.Match<IActionResult>(
            _ => NoContent(),
            e => e.ToObjectResult());
    }
}