using FluentValidation;

namespace Application.Tracks.Commands;

public class DeleteTrackCommandValidator : AbstractValidator<DeleteTrackCommand>
{
    public DeleteTrackCommandValidator()
    {
        RuleFor(x => x.TrackId)
            .NotEmpty()
            .WithMessage("TrackId must not be empty.");
    }
}