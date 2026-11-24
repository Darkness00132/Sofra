using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sofra.Api.Helper;
using Sofra.Application.Common.Results;
using Sofra.Application.Features.Menus.AddMenuItem;
using Sofra.Application.Features.Menus.AddMenuType;
using Sofra.Application.Features.Menus.Dtos;
using Sofra.Application.Features.Menus.GetMyMenu;
using Sofra.Application.Features.Menus.RemoveMenuItem;
using Sofra.Application.Features.Menus.RemoveMenuItemImage;
using Sofra.Application.Features.Menus.RemoveMenuType;
using Sofra.Application.Features.Menus.SetMenuItemAvailability;
using Sofra.Application.Features.Menus.UpdateMenuItem;
using Sofra.Application.Features.Menus.UpdateMenuType;
using Sofra.Application.Features.Menus.UploadMenuItemImage;
using Sofra.Api.Services;

namespace Sofra.Api.Controllers;

/// <summary>Management of the signed-in owner's menu: menu types, food items, availability and pictures (FR-13…FR-16).</summary>
[Route("api/v{version:apiVersion}/restaurants/me/menu")]
[ApiController]
[Authorize]
public class MyRestaurantMenuController(IMediator mediator) : ControllerBase
{
    /// <summary>Returns the owner's full menu — includes unavailable items (management view).</summary>
    /// <returns>The menu grouped by menu type.</returns>
    /// <response code="200">The owner's menu.</response>
    /// <response code="401">The request is not authenticated.</response>
    /// <response code="404">The user has no approved restaurant yet.</response>
    [HttpGet]
    [ProducesResponseType(typeof(MyMenuResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMyMenu(CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetMyMenuQuery(), cancellationToken);
        if (result.IsFailure)
            return result.Error.ToActionResult(HttpContext);

        return Ok(result.Response);
    }

    /// <summary>Adds a menu type under a platform category (FR-13).</summary>
    /// <param name="command">The platform category, type name and optional description.</param>
    /// <returns>The created menu type.</returns>
    /// <response code="200">The menu type was created.</response>
    /// <response code="400">The request failed validation (field errors are returned).</response>
    /// <response code="401">The request is not authenticated.</response>
    /// <response code="404">The restaurant or food category was not found.</response>
    [HttpPost("types")]
    [ProducesResponseType(typeof(MenuTypeResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddMenuType(AddMenuTypeCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        if (result.IsFailure)
            return result.Error.ToActionResult(HttpContext);

        return Ok(result.Response);
    }

    /// <summary>Updates a menu type (FR-13).</summary>
    /// <param name="menuTypeId">The menu type id.</param>
    /// <param name="command">The new name and description.</param>
    /// <returns>The updated menu type.</returns>
    /// <response code="200">The menu type was updated.</response>
    /// <response code="400">The request failed validation (field errors are returned).</response>
    /// <response code="401">The request is not authenticated.</response>
    /// <response code="404">The menu type was not found on your menu.</response>
    /// <response code="422">The update violated a business rule.</response>
    [HttpPut("types/{menuTypeId:guid}")]
    [ProducesResponseType(typeof(MenuTypeResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> UpdateMenuType(Guid menuTypeId, UpdateMenuTypeCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command with { MenuTypeId = menuTypeId }, cancellationToken);
        if (result.IsFailure)
            return result.Error.ToActionResult(HttpContext);

        return Ok(result.Response);
    }

    /// <summary>Removes a menu type. Blocked while it still has food items (FR-13).</summary>
    /// <param name="menuTypeId">The menu type id.</param>
    /// <response code="200">The menu type was removed.</response>
    /// <response code="401">The request is not authenticated.</response>
    /// <response code="404">The menu type was not found on your menu.</response>
    /// <response code="409">The menu type still has food items.</response>
    [HttpDelete("types/{menuTypeId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> RemoveMenuType(Guid menuTypeId, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new RemoveMenuTypeCommand(menuTypeId), cancellationToken);
        if (result.IsFailure)
            return result.Error.ToActionResult(HttpContext);

        return Ok();
    }

    /// <summary>Adds a food item to a menu type (FR-14, FR-15).</summary>
    /// <param name="command">The menu type, item name, description, price and initial availability.</param>
    /// <returns>The created food item.</returns>
    /// <response code="200">The food item was created.</response>
    /// <response code="400">The request failed validation (field errors are returned).</response>
    /// <response code="401">The request is not authenticated.</response>
    /// <response code="404">The restaurant or menu type was not found.</response>
    [HttpPost("items")]
    [ProducesResponseType(typeof(MenuItemResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddMenuItem(AddMenuItemCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        if (result.IsFailure)
            return result.Error.ToActionResult(HttpContext);

        return Ok(result.Response);
    }

    /// <summary>Updates a food item. Price changes never affect past orders (FR-14, BR-04).</summary>
    /// <param name="foodItemId">The food item id.</param>
    /// <param name="command">The new name, description and price.</param>
    /// <returns>The updated food item.</returns>
    /// <response code="200">The food item was updated.</response>
    /// <response code="400">The request failed validation (field errors are returned).</response>
    /// <response code="401">The request is not authenticated.</response>
    /// <response code="404">The food item was not found on your menu.</response>
    /// <response code="422">The update violated a business rule.</response>
    [HttpPut("items/{foodItemId:guid}")]
    [ProducesResponseType(typeof(MenuItemResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> UpdateMenuItem(Guid foodItemId, UpdateMenuItemCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command with { FoodItemId = foodItemId }, cancellationToken);
        if (result.IsFailure)
            return result.Error.ToActionResult(HttpContext);

        return Ok(result.Response);
    }

    /// <summary>Removes a food item from the menu. Blocked when past orders reference it (BR-04, FR-46).</summary>
    /// <param name="foodItemId">The food item id.</param>
    /// <response code="200">The food item was removed.</response>
    /// <response code="401">The request is not authenticated.</response>
    /// <response code="404">The food item was not found on your menu.</response>
    /// <response code="409">Past orders reference the item.</response>
    [HttpDelete("items/{foodItemId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> RemoveMenuItem(Guid foodItemId, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new RemoveMenuItemCommand(foodItemId), cancellationToken);
        if (result.IsFailure)
            return result.Error.ToActionResult(HttpContext);

        return Ok();
    }

    /// <summary>Marks a food item available or unavailable (FR-16, BR-05).</summary>
    /// <param name="foodItemId">The food item id.</param>
    /// <param name="command">The new availability flag.</param>
    /// <returns>The updated food item.</returns>
    /// <response code="200">The availability was updated.</response>
    /// <response code="401">The request is not authenticated.</response>
    /// <response code="404">The food item was not found on your menu.</response>
    [HttpPatch("items/{foodItemId:guid}/availability")]
    [ProducesResponseType(typeof(MenuItemResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SetMenuItemAvailability(Guid foodItemId, SetMenuItemAvailabilityCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command with { FoodItemId = foodItemId }, cancellationToken);
        if (result.IsFailure)
            return result.Error.ToActionResult(HttpContext);

        return Ok(result.Response);
    }

    /// <summary>Uploads a picture for a food item (.jpg, .jpeg, .png, .webp; max 5 MB). Replaces any existing picture.</summary>
    /// <param name="foodItemId">The food item id.</param>
    /// <param name="form">The multipart form carrying the image file.</param>
    /// <returns>The updated food item with its image URL.</returns>
    /// <response code="200">The picture was uploaded.</response>
    /// <response code="400">The file violated the picture rules (type, size) or failed validation.</response>
    /// <response code="401">The request is not authenticated.</response>
    /// <response code="404">The food item was not found on your menu.</response>
    [HttpPost("items/{foodItemId:guid}/image")]
    [ProducesResponseType(typeof(MenuItemResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UploadMenuItemImage(Guid foodItemId, [FromForm] UploadMenuItemImageForm form, CancellationToken cancellationToken)
    {
        var command = new UploadMenuItemImageCommand(foodItemId, form.Image.ToFileDto());
        var result = await mediator.Send(command, cancellationToken);
        if (result.IsFailure)
            return result.Error.ToActionResult(HttpContext);

        return Ok(result.Response);
    }

    /// <summary>Removes a food item's picture from the menu and from file storage.</summary>
    /// <param name="foodItemId">The food item id.</param>
    /// <returns>The updated food item.</returns>
    /// <response code="200">The picture was removed.</response>
    /// <response code="401">The request is not authenticated.</response>
    /// <response code="404">The food item or its picture was not found.</response>
    [HttpDelete("items/{foodItemId:guid}/image")]
    [ProducesResponseType(typeof(MenuItemResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveMenuItemImage(Guid foodItemId, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new RemoveMenuItemImageCommand(foodItemId), cancellationToken);
        if (result.IsFailure)
            return result.Error.ToActionResult(HttpContext);

        return Ok(result.Response);
    }

    /// <summary>The multipart form for picture uploads.</summary>
    public record UploadMenuItemImageForm(IFormFile Image);
}
