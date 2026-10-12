using FluentValidation;

namespace Application.Cars.Commands;

public class UpdateCarCommandValidator : AbstractValidator<UpdateCarCommand>
{
    public UpdateCarCommandValidator()
    {
        RuleFor(x => x.CarId).NotEmpty();
        RuleFor(x => x.Vin).NotEmpty().Length(17);
        RuleFor(x => x.Brand).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Model).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Price).InclusiveBetween(0.01m, 10_000_000m);
        RuleFor(x => x.FuelType).IsInEnum();
    }
}