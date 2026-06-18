namespace Directfn.Custody.ApiFramework.Auditing;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class AuditActionAttribute : Attribute
{
    public AuditActionAttribute(string actionName)
    {
        ActionName = actionName;
    }

    public string ActionName { get; }
}