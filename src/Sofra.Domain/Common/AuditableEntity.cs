namespace Sofra.Domain.Common
{
    public class AuditableEntity : Entity
    {
        public DateTimeOffset CreatedAt { get; set; }

        public DateTimeOffset? LastModifiedAt { get; set; }
        public Guid? ModifiedBy { get; set; }
    }
}
