using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sofra.Api.Helper;
using Sofra.Application.Common.Pagination;
using Sofra.Application.Common.Results;
using Sofra.Application.Features.FoodCategories.Dtos;
using Sofra.Application.Features.FoodCategories.ListCategories;
using Sofra.Application.Features.FoodCategories;

namespace Sofra.Api.Controllers;

/// <summary>Public catalog of platform-level food categories (used for discovery filters and menu setup).</summary>
[Route("api/v{version:apiVersion}/food-categories")]
[ApiController]
public class FoodCategoriesController(IMediator mediator) : ControllerBase
{
    /// <summary>Lists platform food categories with search and paging.</summary>
    /// <param name="search">Case-insensitive match on the category name.</param>
    /// <param name="pageNumber">Page number, starting at 1.</param>
    /// <param name="pageSize">Page size, 1-100.</param>
    /// <returns>A page of categories, ordered by name.</returns>
    /// <response code="200">The category page.</response>
    /// <response code="400">The filters failed validation.</response>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<FoodCategoryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ListFoodCategories([FromQuery] string? search,
        [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 50, CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(new ListCategoriesQuery(new FoodCategoryFilter(search, pageNumber, pageSize)), cancellationToken);
        if (result.IsFailure)
            return result.Error.ToActionResult(HttpContext);

        return Ok(result.Response);
    }
}
