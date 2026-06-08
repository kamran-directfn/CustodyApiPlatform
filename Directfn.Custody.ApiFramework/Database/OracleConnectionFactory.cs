using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace Directfn.Custody.ApiFramework.Database
{
    public sealed class OracleConnectionFactory : IDbConnectionFactory
    {
        private readonly IConfiguration _configuration;

        public OracleConnectionFactory(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IDbConnection CreateConnection()
        {
            string? connectionString = _configuration.GetConnectionString("CustodyDb");

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException("Connection string 'CustodyDb' is missing.");
            }

            return new OracleConnection(connectionString);
        }
    }
}