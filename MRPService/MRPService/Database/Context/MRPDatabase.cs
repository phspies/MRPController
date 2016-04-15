using System.Data.Entity;
using System.IO;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.SqlServerCompact;
using System.Data.Common;
using System.Data.SqlServerCe;

namespace MRPService.LocalDatabase
{
    [DbConfigurationType(typeof(MyDbConfiguration))]
    public partial class MRPDatabase : DbContext
	{

        public MRPDatabase() : base(GetConnection(), true)
        {
            Database.SetInitializer<MRPDatabase>(new MRPDBInitializer());
        }

        public DbSet<Credential> Credentials { get; set; }
        public DbSet<Platform> Platforms { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Workload> Workloads { get; set; }
        public DbSet<NetworkFlow> NetworkFlows { get; set; }
        public DbSet<Netstat> Netstat { get; set; }
        public DbSet<Performance> Performance { get; set; }

        public static DbConnection GetConnection()
        {
            string dblocation = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            string dbfilename = Path.Combine(dblocation,"Database","MRPController.sdf");
            if (!Directory.Exists(Path.GetDirectoryName(dbfilename)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(dbfilename));
            }
            var factory = DbProviderFactories.GetFactory("System.Data.SqlServerCe.4.0");
            var connection = factory.CreateConnection();
            string dbfullpath = Path.Combine(dblocation, dbfilename);
            connection.ConnectionString = "Data Source=" + dbfullpath + "; Persist Security Info=False;";
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
