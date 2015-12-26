using System.Data.Common;
using System.Data.SqlServerCe;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Data.EntityClient;
using System;

namespace CloudMoveyWorkerService.LocalDatabase
{
    public class InCodeEFConfig
	{
		private const string DbFileName = "CloudMoveyWorker.sqlite3";


        public static DbConnection Connection { get; } = new SqlCeConnection(GetConnectionString());

		private static string GetConnectionString()
		{
            string providerName = "System.Data.SqlClient";
            string serverName = ".";
            string databaseName = "CloudMoveyWorker";

            // Initialize the connection string builder for the
            // underlying provider.
            SqlConnectionStringBuilder sqlBuilder = new SqlConnectionStringBuilder();

            // Set the properties for the data source.
            sqlBuilder.DataSource = serverName;
            sqlBuilder.InitialCatalog = databaseName;
            sqlBuilder.IntegratedSecurity = true;

            // Build the SqlConnection connection string.
            string providerString = sqlBuilder.ToString();

            // Initialize the EntityConnectionStringBuilder.
            EntityConnectionStringBuilder entityBuilder =
            new EntityConnectionStringBuilder();

            //Set the provider name.
            entityBuilder.Provider = providerName;

            // Set the provider-specific connection string.
            entityBuilder.ProviderConnectionString = providerString;

            // Set the Metadata location.
            entityBuilder.Metadata = @"res://*/AdventureWorksModel.csdl|
                        res://*/AdventureWorksModel.ssdl|
                        res://*/AdventureWorksModel.msl";
            Console.WriteLine(entityBuilder.ToString());
            return entityBuilder.ToString();
        }

		private static string GetFilePath()
		{
			return "|DataDirectory|" + DbFileName;
		}
    }
}