using Microsoft.AspNetCore.Identity;
using Sofra.Domain.Common;

namespace Sofra.Domain.Entities
{
    public class AppRole : IdentityRole<Guid>, IEntity
    {
    }
}
