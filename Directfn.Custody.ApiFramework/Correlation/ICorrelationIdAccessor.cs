namespace Directfn.Custody.ApiFramework.Correlation
{
    public interface ICorrelationIdAccessor
    {
        string? CorrelationId { get; }
    }
}