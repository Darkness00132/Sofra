using FluentValidation;
using Sofra.Application.Common.Storage;
using Sofra.Application.Common.Validation;

namespace Sofra.Application.Features.Restaurants.UploadRestaurantImage;

/// <summary>Enforces the shared picture business rules (<see cref="ImageUploadRules"/>) for restaurant pictures.</summary>
public sealed class UploadRestaurantImageCommandValidator : AbstractValidator<UploadRestaurantImageCommand>
{
    public UploadRestaurantImageCommandValidator()
    {
        RuleFor(x => x.Image)
            .NotNull().WithMessage("Image file is required.");

        RuleFor(x => x.Image)
            .Must(image => image is not null && image.Length > 0)
            .WithMessage("Image file is empty.")
            .When(x => x.Image is not null);

        RuleFor(x => x.Image)
            .Must(image => image is not null && image.Length <= ImageUploadRules.MaxSizeBytes)
            .WithMessage("Image file must be at most 5 MB.")
            .When(x => x.Image is not null);

        RuleFor(x => x.Image)
            .Must(image => image is not null && ImageUploadRules.IsExtensionAllowed(image.FileName))
            .WithMessage("Only .jpg, .jpeg, .png and .webp images are allowed.")
            .When(x => x.Image is not null);

        RuleFor(x => x.Image)
            .Must(image => image is not null && ImageUploadRules.AllowedContentTypes.Contains(image.ContentType))
            .WithMessage("The file is not an accepted image type.")
            .When(x => x.Image is not null);
    }
}
