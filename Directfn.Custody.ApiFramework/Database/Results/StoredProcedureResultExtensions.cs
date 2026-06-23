namespace Directfn.Custody.ApiFramework.Database.Results;

public static class StoredProcedureResultExtensions
{
    public static string? GetString(this StoredProcedureResult result, string parameterName)
    {
        return result.OutputParameters.TryGetValue(parameterName, out object? value) ? Convert.ToString(value) : null;
    }

    public static int? GetInt32(this StoredProcedureResult result, string parameterName)
    {
        if (!result.OutputParameters.TryGetValue(parameterName, out object? value) || value is null)
        {
            return null;
        }

        return Convert.ToInt32(value.ToString());
    }

    public static long? GetInt64(this StoredProcedureResult result, string parameterName)
    {
        if (!result.OutputParameters.TryGetValue(parameterName, out object? value) || value is null)
        {
            return null;
        }

        return Convert.ToInt64(value.ToString());
    }

    public static decimal? GetDecimal(this StoredProcedureResult result, string parameterName)
    {
        if (!result.OutputParameters.TryGetValue(parameterName, out object? value) || value is null)
        {
            return null;
        }

        return Convert.ToDecimal(value.ToString());
    }
}