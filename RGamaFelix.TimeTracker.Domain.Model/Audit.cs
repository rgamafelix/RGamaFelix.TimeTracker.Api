using Microsoft.AspNetCore.Identity;

namespace RGamaFelix.TimeTracker.Domain.Model;

public class Audit
{
    public AuditAction Action { get; private set; }
    public DateTime Date { get; private set; }
    public Guid Entity { get; private set; }
    public string EntityType { get; set; }
    public string? Memo { get; private set; }
    public IdentityUser<Guid> User { get; private set; }

    public static Audit Create(IdentityUser<Guid> user, AuditAction action, Guid entity, string entityType, string? memo)
    {
        return new Audit
        {
            User = user,
            Action = action,
            Entity = entity,
            EntityType = entityType,
            Date = DateTime.Now,
            Memo = memo
        };
    }
}