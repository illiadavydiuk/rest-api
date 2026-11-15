using Api.Dtos;
using Api.Modules.Errors;
using Application.Brands.Commands;
using Application.Common.Interfaces.Queries;
using Domain.Brands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("brands")]
[ApiController]
public class BrandsController(ISender sender, IBrandQueries brandQueries) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<BrandDto>>> GetBrands(
        [FromQuery] string? name,
        [FromQuery] string? country,
        CancellationToken cancellationToken)
    {
        var brands = await brandQueries.GetAll(name, country, cancellationToken);
        return Ok(brands.Select(BrandDto.FromDomainModel).ToList());
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<BrandDto>> GetBrand(Guid id, CancellationToken cancellationToken)
    {
        var brand = await brandQueries.GetById(new BrandId(id), cancellationToken);
        return brand.Match<ActionResult<BrandDto>>(
            b => BrandDto.FromDomainModel(b),
            () => NotFound());
    }

    [HttpPost]
    public async Task<ActionResult<BrandDto>> CreateBrand([FromBody] CreateBrandDto request, CancellationToken cancellationToken)
    {
        var command = new CreateBrandCommand
        {
            Name = request.Name,
            Country = request.Country
        };

        var result = await sender.Send(command, cancellationToken);
        return result.Match<ActionResult<BrandDto>>(
            b => CreatedAtAction(nameof(GetBrand), new { id = b.Id.Value }, BrandDto.FromDomainModel(b)),
            e => e.ToObjectResult());
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<BrandDto>> UpdateBrand(Guid id, [FromBody] UpdateBrandDto request, CancellationToken cancellationToken)
    {
        var command = new UpdateBrandCommand
        {
            BrandId = id,
            Name = request.Name,
            Country = request.Country
        };

        var result = await sender.Send(command, cancellationToken);
        return result.Match<ActionResult<BrandDto>>(
            b => Ok(BrandDto.FromDomainModel(b)),
            e => e.ToObjectResult());
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteBrand(Guid id, CancellationToken cancellationToken)
    {
        var command = new DeleteBrandCommand { BrandId = id };
        var result = await sender.Send(command, cancellationToken);

        return result.Match<IActionResult>(
            _ => NoContent(),
            e => e.ToObjectResult());
    }
}