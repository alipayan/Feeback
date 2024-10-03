namespace Feedback.APIs;

public interface IUserPrincipals
{
    public int TenantId { get; }
}

public class UserPrincipal : IUserPrincipals

{
    public const string TenantHeaderName = "tenant_id";
    private int _tenantId;

    public UserPrincipal(int tenantId)
    {
        _tenantId = tenantId;
    }

    public int TenantId => _tenantId;
}
