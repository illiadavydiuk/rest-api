using FluentValidation;

namespace Application.Tracks.Commands;

public class CreateTrackCommandValidator : AbstractValidator<CreateTrackCommand>
{
    public CreateTrackCommandValidator()
    {
        RuleFor(x => x.Isrc).NotEmpty().MaximumLength(12);
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Artist).NotEmpty().MaximumLength(200);
        RuleFor(x => x.DurationSeconds).InclusiveBetween(1, 86400);
        RuleFor(x => x.Genre).IsInEnum();
        RuleFor(x => x.ReleaseDate).NotEmpty().LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.UtcNow));
    }
}