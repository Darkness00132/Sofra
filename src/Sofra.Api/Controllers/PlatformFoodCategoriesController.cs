using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sofra.Api.Helper;
using Sofra.Application.Common.Results;
using Sofra.Application.Constants;
using Sofra.Application.Features.FoodCategories.AddCategory;
using Sofra.Application.Features.FoodCategories.Dtos;
using Sofra.Application.Features.FoodCategories.RemoveCategory;
using Sofra.Application.Features.FoodCategories.UpdateCategory;

namespace Sofra.Api.Controllers;

/// <summary>Administrator management of the platform-level food category catalog.</summary>
[Route("api/v{version:apiVersion}/admin/food-categories")]
[ApiController]
[Authorize(Roles = Roles.Administrator)]
public class PlatformFoodCategoriesController(IMediator mediator) : ControllerBase
{
    /// <summary>Adds a platform food category.</summary>
    /// <param name="command">The category name and optional description.</param>
    /// <returns>The created category.</returns>
    /// <response code="200">The category was created.</response>
    /// <response code="400">The request failed validation (field errors are returned).</response>
    /// <response code="401">The request is not authenticated.</response>
    /// <response code="403">The caller is not an administrator.</response>
    /// <response code="409">A category with this name already exists.</response>
    [HttpPost]
    [ProducesResponseType(typeof(FoodCategoryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> AddCategory(AddCategoryCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        if (result.IsFailure)
            return result.Error.ToActionResult(HttpContext);

        return Ok(result.Response);
    }

    /// <summary>Updates a platform food category.</summary>
    /// <param name="categoryId">The category id.</param>
    /// <param name="command">The new name and description.</param>
    /// <returns>The updated category.</returns>
    /// <response code="200">The category was updated.</response>
    /// <response code="400">The request failed validation (field errors are returned).</response>
    /// <response code="401">The request is not authenticated.</response>
    /// <response code="403">The caller is not an administrator.</response>
    /// <response code="404">The category was not found.</response>
    /// <response code="422">The update violated a business rule.</response>
    [HttpPut("{categoryId:guid}")]
    [ProducesResponseType(typeof(FoodCategoryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> UpdateCategory(Guid categoryId, UpdateCategoryCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command with { CategoryId = categoryId }, cancellationToken);
        if (result.IsFailure)
            return result.Error.ToActionResult(HttpContext);

        return Ok(result.Response);
    }

    /// <summary>Removes a platform food category. Blocked while menu types still reference it.</summary>
    /// <param name="categoryId">The category id.</param>
    /// <response code="200">The category was removed.</response>
    /// <response code="401">The request is not authenticated.</response>
    /// <response code="403">The caller is not an administrator.</response>
    /// <response code="404">The category was not found.</response>
    /// <response code="409">Menu types still use the category.</response>
    [HttpDelete("{categoryId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> RemoveCategory(Guid categoryId, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new RemoveCategoryCommand(categoryId), cancellationToken);
        if (result.IsFailure)
            return result.Error.ToActionResult(HttpContext);

        return Ok();
    }
}
