using Domain.Cars;

namespace Application.Cars.Exceptions;

public abstract class CarException(CarId carId, string message, Exception? innerException = null)
    : Exception(message, innerException)
{
    public CarId CarId { get; } = carId;
}

public class CarAlreadyExistsException(CarId carId)
    : CarException(carId, $"Car with ID '{carId}' already exists");

public class CarNotFoundException(CarId carId)
    : CarException(carId, $"Car with ID '{carId}' not found");

public class CarUnhandledException(CarId carId, Exception? innerException = null)
    : CarException(carId, $"Unhandled exception under id {carId}", innerException);   