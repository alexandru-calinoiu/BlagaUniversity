using System.Data.Entity;
using System.Data.Entity.Infrastructure.Interception;
using System.Data.Entity.SqlServer;

namespace BlagaUniversity.DAL
{
    public class UniversityConfiguration : DbConfiguration
    {
        public UniversityConfiguration()
        {
            SetExecutionStrategy("System.Data.SqlClient", () => new SqlAzureExecutionStrategy());
            DbInterception.Add(new UniversityLoggingInterceptor());
            DbInterception.Add(new UniversityInterceptorTransientError());
        }
    }
}