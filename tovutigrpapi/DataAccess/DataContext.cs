
using System.Data;
using System.Data.SqlClient;

namespace tovutigrpapi.DataAccess
{
    public class DataContext
    {
        private readonly IConfiguration configuration;
        public DataContext(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public IDbConnection CreateConnection()
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            return new SqlConnection(connectionString);
        
        }
    }
}
