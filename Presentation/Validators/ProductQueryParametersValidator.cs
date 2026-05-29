using CleanCQRSPOC.Presentation.Models;
using FluentValidation;

namespace CleanCQRSPOC.Presentation.Validators;

public class ProductQueryParametersValidator : AbstractValidator<ProductQueryParameters>
{
    private static readonly string[] AllowedSortFields = ["name", "price"];
    private static readonly string[] AllowedSortDirections = ["asc", "desc"];

    public ProductQueryParametersValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1).WithMessage("page must be greater than or equal to 1.");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100).WithMessage("pageSize must be between 1 and 100.");

        RuleFor(x => x.Sort)
            .Must(s => AllowedSortFields.Contains(s!.ToLowerInvariant()))
            .When(x => !string.IsNullOrWhiteSpace(x.Sort))
            .WithMessage("sort must be one of: name, price.");

        RuleFor(x => x.SortDir)
            .Must(d => AllowedSortDirections.Contains(d!.ToLowerInvariant()))
            .When(x => !string.IsNullOrWhiteSpace(x.SortDir))
            .WithMessage("sortDir must be one of: asc, desc.");

        RuleFor(x => x.MinPrice)
            .GreaterThanOrEqualTo(0).When(x => x.MinPrice.HasValue)
            .WithMessage("minPrice must be non-negative.");

        RuleFor(x => x.MaxPrice)
            .GreaterThanOrEqualTo(0).When(x => x.MaxPrice.HasValue)
            .WithMessage("maxPrice must be non-negative.");

        RuleFor(x => x)
            .Must(x => x.MinPrice <= x.MaxPrice)
            .When(x => x.MinPrice.HasValue && x.MaxPrice.HasValue)
            .WithMessage("minPrice must be less than or equal to maxPrice.");
    }
}
