namespace Sofra.Application.Common.Interfaces;

/// <summary>
/// Current HTTP user context. Defined here so Application handlers can use it; implemented in
/// the API layer (HttpContext-backed) and registered in DI there. Used for ownership checks
/// (SRS BR-03/BR-10) and role checks.
/// </summary>
public interface ICurrentUser
{
    /// <summary>
    /// Returns the authenticated user's id. Throws when the id claim is missing or unparseable.
    /// </summary>
    Guid GetUserId();

    /// <summary>True when the current user belongs to the given role (Customer / Restaurant / Administrator).</summary>
    bool IsInRole(string role);
}
