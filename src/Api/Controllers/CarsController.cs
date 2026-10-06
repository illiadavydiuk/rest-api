using Api.Dtos;
using Application.Cars.Services.Abstract;
using Domain.Cars;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;


[Route("cars")]
[ApiController]
public class CarsController(ICarService carService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<CarDto>>> GetCars(
        [FromQuery] string? brand,
        [FromQuery] bool? isAvailable,
        [FromQuery] decimal? minPrice,
        [FromQuery] decimal? maxPrice,
        CancellationToken cancellationToken)
    {
        var cars = await carService.GetCars(brand, isAvailable, minPrice, maxPrice, cancellationToken);
        return Ok(cars.Select(CarDto.FromDomainModel).ToList());
    }
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<CarDto>> GetCar(Guid id, CancellationToken cancellationToken)
    {
        var car = await carService.GetCar(id, cancellationToken);
        if (car is null) return NotFound();
        return Ok(CarDto.FromDomainModel(car));
    }

    [HttpPost]
    public async Task<ActionResult<CarDto>> CreateCar([FromBody] CreateCarDto request, CancellationToken cancellationToken)
    {
        try
        {
            var car = await carService.Add(
                request.Vin,
                request.Brand,
                request.Model,
                request.Price,
                request.FuelType,
                request.IsAvailable,
                cancellationToken);

            var dto = CarDto.FromDomainModel(car);
            return CreatedAtAction(nameof(GetCar), new { id = dto.Id }, dto);
        }
        catch (ArgumentException ex)
        {
            return Conflict(ex.Message);
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<CarDto>> UpdateCar(Guid id, [FromBody] UpdateCarDto request, CancellationToken cancellationToken)
    {
        try
        {
            var updated = await carService.Update(
                id,
                request.Vin,
                request.Brand,
                request.Model,
                request.Price,
                request.FuelType,
                request.IsAvailable,
                cancellationToken);

            if (updated is null)
            {
                return NotFound();
            }

            return Ok(CarDto.FromDomainModel(updated));
        }
        catch (ArgumentException ex)
        {
            return Conflict(ex.Message);
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteCar(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await carService.Delete(id, cancellationToken);
        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}