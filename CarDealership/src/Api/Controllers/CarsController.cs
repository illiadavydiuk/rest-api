using Api.Dtos;
using Api.Modules.Errors;
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
        var car = await carQueries.GetById(new CarId(id), cancellationToken);
        return car.Match<ActionResult<CarDto>>(
            c => CarDto.FromDomainModel(c),
            () => NotFound());
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
        
        var result = await sender.Send(command, cancellationToken);
        return result.Match<ActionResult<CarDto>>(
            c => CreatedAtAction(nameof(GetCar), new { id = c.Id.Value }, CarDto.FromDomainModel(c)),
            e => e.ToObjectResult());
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
        
        var result = await sender.Send(command, cancellationToken);
        return result.Match<ActionResult<CarDto>>(
            c => Ok(CarDto.FromDomainModel(c)),
            e => e.ToObjectResult());
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteCar(Guid id, CancellationToken cancellationToken)
    {
        var command = new DeleteCarCommand { CarId = id };
        var result = await sender.Send(command, cancellationToken);
        
        return result.Match<IActionResult>(
            _ => NoContent(),
            e => e.ToObjectResult());
    }
}