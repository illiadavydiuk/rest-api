using Application.Tracks.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Api.Modules.Errors;

public static class TrackErrorFactory
{
    public static ObjectResult ToObjectResult(this TrackException error)
    {
        return new ObjectResult(error.Message)
        {
            StatusCode = error switch
            {
                TrackAlreadyExistsException => StatusCodes.Status409Conflict,
                TrackNotFoundException => StatusCodes.Status404NotFound,
                TrackUnhandledException => StatusCodes.Status500InternalServerError,
                _ => throw new NotImplementedException("Track error handler is not implemented.")
            }
        };
    }
}