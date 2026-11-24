using MediatR;
using Sofra.Application.Common.Results;
using Sofra.Application.Features.Menus.Dtos;

namespace Sofra.Application.Features.Menus.UpdateMenuType;

/// <summary>Updates one of the owner's menu types (FR-13).</summary>
public record UpdateMenuTypeCommand(Guid MenuTypeId, string Name, string? Description) : IRequest<Result<MenuTypeResponse>>;
