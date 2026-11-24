using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sofra.Api.Helper;
using Sofra.Application.Features.Carts.AddCartItem;
using Sofra.Application.Features.Carts.ClearCart;
using Sofra.Application.Features.Carts.Dtos;
using Sofra.Application.Features.Carts.GetCart;
using Sofra.Application.Features.Carts.RemoveCartItem;
using Sofra.Application.Features.Carts.UpdateCartItem;

namespace Sofra.Api.Controllers;

/// <summary>The signed-in customer's cart (FR-19…FR-24). One restaurant per cart.</summary>
[Route("api/v{version:apiVersion}/cart")]
[ApiController]
[Authorize]
public class CartController(IMediator mediator) : ControllerBase
{
    /// <summary>Returns the cart with its items, prices and subtotal (FR-23).</summary>
    /// <returns>The cart; an empty cart when none exists yet.</returns>
    /// <response code="200">The cart.</response>
    /// <response code="401">The request is not authenticated.</response>
    [HttpGet]
    [ProducesResponseType(typeof(CartResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetCart(CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetCartQuery(), cancellationToken);
        if (result.IsFailure)
            return result.Error.ToActionResult(HttpContext);

        return Ok(result.Response);
    }

    /// <summary>Adds a food item to the cart. Adding from a different restaurant is rejected (FR-19, FR-21, FR-22).</summary>
    /// <param name="command">The food item id and quantity.</param>
    /// <returns>The updated cart.</returns>
    /// <response code="200">The item was added.</response>
    /// <response code="400">The request failed validation (field errors are returned).</response>
    /// <response code="401">The request is not authenticated.</response>
    /// <response code="404">The food item was not found.</response>
    /// <response code="409">The cart already holds items from another restaurant, or the item is unavailable (FR-22, BR-05).</response>
    [HttpPost("items")]
    [ProducesResponseType(typeof(CartResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> AddCartItem(AddCartItemCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        if (result.IsFailure)
            return result.Error.ToActionResult(HttpContext);

        return Ok(result.Response);
    }

    /// <summary>Changes the quantity of an item in the cart (FR-20).</summary>
    /// <param name="foodItemId">The food item id.</param>
    /// <param name="command">The new quantity.</param>
    /// <returns>The updated cart.</returns>
    /// <response code="200">The quantity was updated.</response>
    /// <response code="400">The request failed validation (field errors are returned).</response>
    /// <response code="401">The request is not authenticated.</response>
    /// <response code="404">The cart or the item was not found.</response>
    [HttpPut("items/{foodItemId:guid}")]
    [ProducesResponseType(typeof(CartResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateCartItem(Guid foodItemId, UpdateCartItemCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command with { FoodItemId = foodItemId }, cancellationToken);
        if (result.IsFailure)
            return result.Error.ToActionResult(HttpContext);

        return Ok(result.Response);
    }

    /// <summary>Removes one item from the cart (FR-20).</summary>
    /// <param name="foodItemId">The food item id.</param>
    /// <returns>The updated cart.</returns>
    /// <response code="200">The item was removed.</response>
    /// <response code="401">The request is not authenticated.</response>
    /// <response code="404">The cart or the item was not found.</response>
    [HttpDelete("items/{foodItemId:guid}")]
    [ProducesResponseType(typeof(CartResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveCartItem(Guid foodItemId, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new RemoveCartItemCommand(foodItemId), cancellationToken);
        if (result.IsFailure)
            return result.Error.ToActionResult(HttpContext);

        return Ok(result.Response);
    }

    /// <summary>Empties the cart completely — used when switching restaurants (FR-22).</summary>
    /// <returns>The emptied cart.</returns>
    /// <response code="200">The cart is empty.</response>
    /// <response code="401">The request is not authenticated.</response>
    [HttpDelete]
    [ProducesResponseType(typeof(CartResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ClearCart(CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new ClearCartCommand(), cancellationToken);
        if (result.IsFailure)
            return result.Error.ToActionResult(HttpContext);

        return Ok(result.Response);
    }
}
