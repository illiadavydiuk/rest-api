using Application.Cars.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Api.Modules.Errors;

public static class CarErrorFactory
{
    public static ObjectResult ToObjectResult(this CarException error)
    {
        return new ObjectResult(error.Message)
        {
            StatusCode = error switch
            {
                CarAlreadyExistsException => StatusCodes.Status409Conflict,
                CarNotFoundException => StatusCodes.Status404NotFound,
                CarUnhandledException => StatusCodes.Status500InternalServerError,
                _ => throw new NotImplementedException("Car error handler is not implemented.")
            }
        };
    }
}