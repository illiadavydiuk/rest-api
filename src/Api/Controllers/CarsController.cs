using Api.Dtos;
using Application.Cars.Commands;
using Application.Common.Interfaces.Queries;
using Domain.Cars;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;


[Route("cars")]
[ApiController]
public class CarsController(ISender sender, ICarQueries carQueries) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<CarDto>>> GetCars(
        [FromQuery] string? brand,
        [FromQuery] bool? isAvailable,
        [FromQuery] decimal? minPrice,
        [FromQuery] decimal? maxPrice,
        CancellationToken cancellationToken)
    {
        var cars = await carQueries.GetAll(brand, isAvailable, minPrice, maxPrice, cancellationToken);
        return Ok(cars.Select(CarDto.FromDomainModel).ToList());
    }
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<CarDto>> GetCar(Guid id, CancellationToken cancellationToken)
    {
        var car = await carQueries.GetById(id, cancellationToken);
        if (car is null) return NotFound();
        return Ok(CarDto.FromDomainModel(car));
    }

    [HttpPost]
    public async Task<ActionResult<CarDto>> CreateCar([FromBody] CreateCarDto request, CancellationToken cancellationToken)
    {
        var command = new CreateCarCommand
        {
            Vin = request.Vin,
            Brand = request.Brand,
            Model = request.Model,
            Price = request.Price,
            FuelType = request.FuelType,
            IsAvailable = request.IsAvailable
        };
        
        try
        {
            var car = await sender.Send(command, cancellationToken);
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
        var command = new UpdateCarCommand
        {
            CarId = id,
            Vin = request.Vin,
            Brand = request.Brand,
            Model = request.Model,
            Price = request.Price,
            FuelType = request.FuelType,
            IsAvailable = request.IsAvailable
        };
        
        try
        {
            var updated = await sender.Send(command, cancellationToken);

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
        var command = new DeleteCarCommand { CarId = id };
        var deleted = await sender.Send(command, cancellationToken);
        
        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}