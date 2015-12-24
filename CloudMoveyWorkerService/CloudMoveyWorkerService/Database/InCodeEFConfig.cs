using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Core.Common;
using System.Data.SQLite;
using System.Data.SQLite.EF6;

namespace CloudMoveyWorkerService.Database
{
	public class InCodeEFConfig : DbConfiguration
	{
		private const string DbFileName = "CloudMoveyWorker.sqlite3";

		public InCodeEFConfig()
		{
			SetProviderFactory(nameof(System.Data.SQLite), SQLiteFactory.Instance);
			SetProviderFactory(nameof(System.Data.SQLite.EF6), SQLiteProviderFactory.Instance);
			SetProviderServices(nameof(System.Data.SQLite), (DbProviderServices)SQLiteProviderFactory.Instance.GetService(typeof(DbProviderServices)));
		}

		/// <summary>
		///     A <see cref="DbConnection" /> to the SQLite Database.
		/// </summary>
		/// <value>
		///     The connection.
		/// </value>
		public static DbConnection Connection { get; } = new SQLiteConnection(GetConnectionString(), true);

		private static string GetConnectionString()
		{
			// "foreign key=true" activates the FK-Constraints
			return "Data Source=" + GetFilePath() + ";foreign keys=true;synchronous=Full";
		}

		private static string GetFilePath()
		{
			return "|DataDirectory|" + DbFileName;
		}
	}
}