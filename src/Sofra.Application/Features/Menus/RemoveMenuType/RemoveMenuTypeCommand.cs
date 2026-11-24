using MediatR;
using Sofra.Application.Common.Results;

namespace Sofra.Application.Features.Menus.RemoveMenuType;

/// <summary>Removes one of the owner's menu types. Blocked while it still has food items (FR-13).</summary>
public record RemoveMenuTypeCommand(Guid MenuTypeId) : IRequest<Result>;
