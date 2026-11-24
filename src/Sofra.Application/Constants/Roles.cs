using Sofra.Domain.Entities;
namespace Sofra.Application.Constants;

/// <summary>Role names used by Identity authorization across the platform.</summary>
public static class Roles
{
    public const string Customer = "Customer";
    public const string Restaurant = "Restaurant";
    public const string Administrator = "Administrator";

    public static readonly string[] All = [Customer, Restaurant, Administrator];
}
