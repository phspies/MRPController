using System.Data.Entity;
using System.IO;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.SqlServerCompact;
using System.Data.Common;
using System.Data.SqlServerCe;

namespace MRMPService.LocalDatabase
{
    [DbConfigurationType(typeof(MyDbConfiguration))]
    public partial class MRPDatabase : DbContext
	{
        public MRPDatabase() : base(GetConnection(), true)
        {
            Database.SetInitializer<MRPDatabase>(new MRPDBInitializer());
        }

        public DbSet<NetworkFlow> NetworkFlows { get; set; }
        public DbSet<PerfCounterSample> PerfCounterSample { get; set; }
        public DbSet<ManagerEvent> ManagerEvents { get; set; }
        public static DbConnection GetConnection()
        {
            string dblocation = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            string dbfilename = Path.Combine(dblocation,"Database","MRMPManager.sdf");
            if (!Directory.Exists(Path.GetDirectoryName(dbfilename)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(dbfilename));
            }
            var factory = DbProviderFactories.GetFactory("System.Data.SqlServerCe.4.0");
            var connection = factory.CreateConnection();
            string dbfullpath = Path.Combine(dblocation, dbfilename);
            System.Data.SqlServerCe.SqlCeConnectionStringBuilder builder = new System.Data.SqlServerCe.SqlCeConnectionStringBuilder();
            builder.DataSource = dbfullpath;
            builder.MaxBufferSize = 2048;
            builder.PersistSecurityInfo = false;
            builder.MaxDatabaseSize = 4000;
            builder.DefaultLockTimeout = 30000;
            connection.ConnectionString = builder.ConnectionString;
            return connection;
        }

        public class MRPDBInitializer : CreateDatabaseIfNotExists<MRPDatabase>
        {
            protected override void Seed(MRPDatabase context)
            {
                base.Seed(context);
            }
        }

        public class MyDbConfiguration : DbConfiguration
        {
            public MyDbConfiguration()
            {
                this.SetDefaultConnectionFactory(new System.Data.Entity.Infrastructure.SqlCeConnectionFactory("System.Data.SqlServerCe.4.0"));

                this.SetProviderServices("System.Data.SqlServerCe.4.0", SqlCeProviderServices.Instance);

                this.SetMigrationSqlGenerator("System.Data.SqlServerCe.4.0", () => new SqlCeMigrationSqlGenerator());
            }
        }

    }

}
