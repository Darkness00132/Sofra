using MediatR;
using Sofra.Application.Common.Results;
using Sofra.Application.Features.Menus.Dtos;

namespace Sofra.Application.Features.Menus.GetMyMenu;

/// <summary>The owner's full menu — includes unavailable items (management view).</summary>
public record GetMyMenuQuery : IRequest<Result<MyMenuResponse>>;
