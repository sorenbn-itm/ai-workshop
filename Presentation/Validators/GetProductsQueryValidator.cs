using CleanCQRSPOC.Application.Queries;
using FluentValidation;

namespace CleanCQRSPOC.Presentation.Validators;

public class GetProductsQueryValidator : AbstractValidator<GetProductsQuery>
{
    private static readonly string[] AllowedSortFields = ["name", "price"];
    private static readonly string[] AllowedSortDirections = ["asc", "desc"];

    public GetProductsQueryValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Page must be greater than or equal to 1.");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100)
            .WithMessage("PageSize must be between 1 and 100.");

        RuleFor(x => x.Sort)
            .Must(sort => string.IsNullOrWhiteSpace(sort) || AllowedSortFields.Contains(sort, StringComparer.OrdinalIgnoreCase))
            .WithMessage("Sort must be one of: name, price.");

        RuleFor(x => x.SortDir)
            .Must(sortDir => AllowedSortDirections.Contains(sortDir, StringComparer.OrdinalIgnoreCase))
            .WithMessage("SortDir must be one of: asc, desc.");

        RuleFor(x => x)
            .Must(x => !x.MinPrice.HasValue || !x.MaxPrice.HasValue || x.MinPrice.Value <= x.MaxPrice.Value)
            .WithMessage("MinPrice must be less than or equal to MaxPrice.");
    }
}
