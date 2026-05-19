using System.Data;

namespace Directfn.Custody.ApiFramework.Database;

public interface IDbConnectionFactory
{
    IDbConnection CreateConnection();
}