using Domain.Brands;

namespace Application.Brands.Exceptions;

public abstract class BrandException(BrandId brandId, string message, Exception? innerException = null)
    : Exception(message, innerException)
{
    public BrandId BrandId { get; } = brandId;
}

public class BrandAlreadyExistsException(BrandId brandId, string name)
    : BrandException(brandId, $"Brand with name '{name}' already exists (ID: '{brandId}')");

public class BrandNotFoundException(BrandId brandId)
    : BrandException(brandId, $"Brand with ID '{brandId}' not found");

public class BrandUnhandledException(BrandId brandId, Exception? innerException = null)
    : BrandException(brandId, $"Unhandled exception under id {brandId}", innerException);
    