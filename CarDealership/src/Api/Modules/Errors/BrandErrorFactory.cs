using Application.Brands.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Api.Modules.Errors;

public static class BrandErrorFactory
{
    public static ObjectResult ToObjectResult(this BrandException error)
    {
        return new ObjectResult(error.Message)
        {
            StatusCode = error switch
            {
                BrandAlreadyExistsException => StatusCodes.Status409Conflict,
                BrandNotFoundException => StatusCodes.Status404NotFound,
                BrandUnhandledException => StatusCodes.Status500InternalServerError,
                _ => throw new NotImplementedException("Brand error handler is not implemented.")
            }
        };
    }
}