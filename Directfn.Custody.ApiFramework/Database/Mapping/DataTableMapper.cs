using System.Data;
using System.Reflection;

namespace Directfn.Custody.ApiFramework.Database.Mapping
{
    public static class DataTableMapper
    {
        public static List<T> ToList<T>(DataTable dataTable) where T : class, new()
        {
            List<T> result = new();

            if (dataTable.Rows.Count == 0)
            {
                return result;
            }

            HashSet<string> columns = dataTable.Columns.Cast<DataColumn>().Select(x => Normalize(x.ColumnName)).ToHashSet(StringComparer.OrdinalIgnoreCase);

            foreach (DataRow row in dataTable.Rows)
            {
                T item = new();

                foreach (PropertyInfo property in typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    if (!property.CanWrite)
                    {
                        continue;
                    }

                    string normalizedPropertyName = Normalize(property.Name);

                    if (!columns.Contains(normalizedPropertyName))
                    {
                        continue;
                    }

                    string columnName = dataTable.Columns.Cast<DataColumn>().First(x => Normalize(x.ColumnName).Equals(normalizedPropertyName, StringComparison.OrdinalIgnoreCase)).ColumnName;

                    object? value = row[columnName];

                    if (value == DBNull.Value || value is null)
                    {
                        continue;
                    }

                    Type targetType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;

                    try
                    {
                        object convertedValue = ConvertValue(value, targetType);
                        property.SetValue(item, convertedValue);
                    }
                    catch
                    {
                        // Keep mapper tolerant for legacy DB fields.
                        // Later we can add logging if needed.
                    }
                }

                result.Add(item);
            }

            return result;
        }

        private static string Normalize(string value)
        {
            return value.Replace("_", string.Empty).ToUpperInvariant();
        }

        private static object ConvertValue(object value, Type targetType)
        {
            if (targetType == typeof(string))
            {
                return Convert.ToString(value) ?? string.Empty;
            }

            if (targetType == typeof(long))
            {
                return Convert.ToInt64(value);
            }

            if (targetType == typeof(int))
            {
                return Convert.ToInt32(value);
            }

            if (targetType == typeof(decimal))
            {
                return Convert.ToDecimal(value);
            }

            if (targetType == typeof(bool))
            {
                return Convert.ToInt32(value) == 1;
            }

            if (targetType == typeof(DateTime))
            {
                return Convert.ToDateTime(value);
            }

            if (targetType.IsEnum)
            {
                return Enum.Parse(targetType, Convert.ToString(value)!, true);
            }

            return Convert.ChangeType(value, targetType);
        }
    }
}