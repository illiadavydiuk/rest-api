using Domain.Brands;

namespace Application.Brands.Exceptions;

public abstract class BrandException(BrandId brandId, string message, Exception? innerException = null)
    : Exception(message, innerException)
{
    public BrandId BrandId { get; } = brandId;
}

public class BrandAlreadyExistsException(BrandId brandId)
    : BrandException(brandId, $"Brand with ID '{brandId}' already exists");

public class BrandNotFoundException(BrandId brandId)
    : BrandException(brandId, $"Brand with ID '{brandId}' not found");

public class BrandUnhandledException(BrandId brandId, Exception? innerException = null)
    : BrandException(brandId, $"Unhandled exception under id {brandId}", innerException);
    