using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sofra.Api.Helper;
using Sofra.Application.Common.Results;
using Sofra.Application.Features.Restaurants.Dtos;
using Sofra.Application.Features.Restaurants.GetMyRestaurant;
using Sofra.Application.Features.Restaurants.RemoveRestaurantImage;
using Sofra.Application.Features.Restaurants.SetAcceptingOrders;
using Sofra.Application.Features.Restaurants.UpdateMyRestaurant;
using Sofra.Application.Features.Restaurants.UploadRestaurantImage;

namespace Sofra.Api.Controllers;

/// <summary>The signed-in owner's own restaurant storefront, including its picture (FR-12, FR-17).</summary>
[Route("api/v{version:apiVersion}/restaurants/me")]
[ApiController]
[Authorize]
public class MyRestaurantController(IMediator mediator) : ControllerBase
{
    /// <summary>Returns the signed-in owner's restaurant.</summary>
    /// <returns>The restaurant's information and order-acceptance flag.</returns>
    /// <response code="200">The owner's restaurant.</response>
    /// <response code="401">The request is not authenticated.</response>
    /// <response code="404">The user has no approved restaurant yet.</response>
    [HttpGet]
    [ProducesResponseType(typeof(MyRestaurantResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMyRestaurant(CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetMyRestaurantQuery(), cancellationToken);
        if (result.IsFailure)
            return result.Error.ToActionResult(HttpContext);

        return Ok(result.Response);
    }

    /// <summary>Updates the signed-in owner's restaurant information (FR-12).</summary>
    /// <param name="command">The restaurant's name, address, phone and description.</param>
    /// <returns>The updated restaurant.</returns>
    /// <response code="200">The restaurant was updated.</response>
    /// <response code="400">The request failed validation (field errors are returned).</response>
    /// <response code="401">The request is not authenticated.</response>
    /// <response code="404">The user has no approved restaurant yet.</response>
    /// <response code="422">The update violated a business rule.</response>
    [HttpPut]
    [ProducesResponseType(typeof(MyRestaurantResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> UpdateMyRestaurant(UpdateMyRestaurantCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        if (result.IsFailure)
            return result.Error.ToActionResult(HttpContext);

        return Ok(result.Response);
    }

    /// <summary>Toggles whether the signed-in owner's restaurant accepts new orders (FR-17).</summary>
    /// <param name="command">The new order-acceptance flag.</param>
    /// <returns>The updated restaurant.</returns>
    /// <response code="200">The flag was updated.</response>
    /// <response code="401">The request is not authenticated.</response>
    /// <response code="404">The user has no approved restaurant yet.</response>
    [HttpPatch]
    [ProducesResponseType(typeof(MyRestaurantResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SetAcceptingOrders(SetAcceptingOrdersCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        if (result.IsFailure)
            return result.Error.ToActionResult(HttpContext);

        return Ok(result.Response);
    }

    /// <summary>Uploads a picture for the owner's restaurant (.jpg, .jpeg, .png, .webp; max 5 MB). Replaces any existing picture.</summary>
    /// <param name="form">The multipart form carrying the image file.</param>
    /// <returns>The updated restaurant with its image URL.</returns>
    /// <response code="200">The picture was uploaded.</response>
    /// <response code="400">The file violated the picture rules (type, size) or failed validation.</response>
    /// <response code="401">The request is not authenticated.</response>
    /// <response code="404">The user has no approved restaurant yet.</response>
    [HttpPost("image")]
    [ProducesResponseType(typeof(MyRestaurantResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UploadRestaurantImage([FromForm] UploadRestaurantImageForm form, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new UploadRestaurantImageCommand(form.Image.ToFileDto()), cancellationToken);
        if (result.IsFailure)
            return result.Error.ToActionResult(HttpContext);

        return Ok(result.Response);
    }

    /// <summary>Removes the owner's restaurant picture from the storefront and from file storage.</summary>
    /// <returns>The updated restaurant.</returns>
    /// <response code="200">The picture was removed.</response>
    /// <response code="401">The request is not authenticated.</response>
    /// <response code="404">The user has no approved restaurant, or the restaurant has no picture.</response>
    [HttpDelete("image")]
    [ProducesResponseType(typeof(MyRestaurantResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveRestaurantImage(CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new RemoveRestaurantImageCommand(), cancellationToken);
        if (result.IsFailure)
            return result.Error.ToActionResult(HttpContext);

        return Ok(result.Response);
    }

    /// <summary>The multipart form for restaurant picture uploads.</summary>
    public record UploadRestaurantImageForm(IFormFile Image);
}
