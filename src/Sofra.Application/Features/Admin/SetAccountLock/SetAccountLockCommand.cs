using MediatR;
using Sofra.Application.Common.Results;

namespace Sofra.Application.Features.Admin.SetAccountLock;

/// <summary>Locks or unlocks a user account (FR-50). Locked accounts cannot sign in.</summary>
public record SetAccountLockCommand(Guid AccountId, bool Locked) : IRequest<Result>;
