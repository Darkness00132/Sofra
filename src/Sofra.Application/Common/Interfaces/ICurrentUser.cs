using Sofra.Application.Common.Results;

namespace Sofra.Application.Common.Interfaces;

/// <summary>
/// Current HTTP user context. Defined here so Application handlers can use it; implemented in
/// the API layer (HttpContext-backed) and registered in DI there. Used for ownership checks
/// (SRS BR-03/BR-10) and role checks.
/// </summary>
public interface ICurrentUser
{
    /// <summary>
    /// The authenticated user's id. When there is no authenticated user (or the id claim is
    /// missing), returns an <see cref="ErrorType.Unauthorized"/> failure — never null.
    /// </summary>
    Result<Guid> GetUserId();

    /// <summary>True when the current user belongs to the given role (Customer / Restaurant / Administrator).</summary>
    bool IsInRole(string role);
}
