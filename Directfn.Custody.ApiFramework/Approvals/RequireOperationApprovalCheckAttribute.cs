namespace Directfn.Custody.ApiFramework.Approvals;

[AttributeUsage(AttributeTargets.Method)]
public sealed class RequireOperationApprovalCheckAttribute : Attribute
{
    public RequireOperationApprovalCheckAttribute(string screenName, string recordIdPropertyName)
    {
        ScreenName = screenName;
        RecordIdPropertyName = recordIdPropertyName;
    }

    public string ScreenName { get; }

    public string RecordIdPropertyName { get; }
}