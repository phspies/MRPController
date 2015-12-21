using System.Data.Entity;
using SQLite.CodeFirst;
using System.Linq;

namespace CloudMoveyWorkerService.Database
{
	public partial class LocalDB : DbContext
	{
		public LocalDB() : base(InCodeEFConfig.Connection, false)
		{

		}

        public virtual DbSet<Credential> Credentials { get; set; }
        public virtual DbSet<Platform> Platforms { get; set; }
        public virtual DbSet<Event> Events { get; set; }
        public virtual DbSet<Workload> Workloads { get; set; }
        public virtual DbSet<NetworkFlow> NetworkFlows { get; set; }
        public virtual DbSet<Performance> Performance { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{

			ConfigureCredentialEntity(modelBuilder);
			ConfigurePlatformEntity(modelBuilder);
            ConfigureEventEntity(modelBuilder);
            ConfigureNetworkFlowEntity(modelBuilder);
            ConfigurePerformanceEntity(modelBuilder);
            ConfigureWorkloadEntity(modelBuilder);

            SqliteCreateDatabaseIfNotExists<LocalDB> sqliteConnectionInitializer = new SqliteCreateDatabaseIfNotExists<LocalDB>(modelBuilder);

			System.Data.Entity.Database.SetInitializer(sqliteConnectionInitializer);
		}

        private static void ConfigureCredentialEntity(DbModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Credential>().HasKey(p => p.id);
		}

		private static void ConfigurePlatformEntity(DbModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Platform>().HasKey(p => p.id);
		}
        private static void ConfigureEventEntity(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Event>().HasKey(p => p.id);
        }
        private static void ConfigureNetworkFlowEntity(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<NetworkFlow>().HasKey(p => p.id);
        }
        private static void ConfigurePerformanceEntity(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Performance>().HasKey(p => p.id);
        }
        private static void ConfigureWorkloadEntity(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Workload>().HasKey(p => p.id);
        }

    }

}
