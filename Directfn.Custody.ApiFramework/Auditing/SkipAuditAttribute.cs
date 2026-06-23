namespace Directfn.Custody.ApiFramework.Auditing;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class SkipAuditAttribute : Attribute
{
}