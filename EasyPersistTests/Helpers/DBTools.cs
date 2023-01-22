using System;
using System.Data;

namespace EasyPersist.Tests.Helpers
{
    using Microsoft.Data.SqlClient;

    public static class DBTools
	{
		/// <summary>
		/// Creates database
		/// </summary>
		/// <param name="DBParam"></param>
		public static void CreateDatabase(DatabaseParam DBParam)
		{
			using (SqlConnection sqlConnection = new SqlConnection(DBParam.GetMasterConnection()))
			{
				string sqlCreateDBQuery = " CREATE DATABASE " + DBParam.DatabaseName;
				                       
				using (SqlCommand myCommand = new SqlCommand(sqlCreateDBQuery, sqlConnection))
				{
					try
					{
						sqlConnection.Open();
						Console.WriteLine(sqlCreateDBQuery);
						myCommand.ExecuteNonQuery();
						Console.WriteLine("Database has been created successfully!");
					}
					catch (Exception ex)
					{
						Console.WriteLine(ex.ToString());
						throw;
					}
					finally
					{
						sqlConnection.Close();
					}
				}
			}
		}
		/// <summary>
		/// Deletes database by name
		/// </summary>
		/// <param name="DBParam"></param>
		public static void DeleteDatabase(DatabaseParam DBParam)
		{
			string deleteSql = @"

USE master;
GO
IF DB_ID (N'" + DBParam.DatabaseName + @"') IS NOT NULL
ALTER DATABASE " + DBParam.DatabaseName + @" SET SINGLE_USER WITH ROLLBACK IMMEDIATE  
GO
IF DB_ID (N'" + DBParam.DatabaseName + @"') IS NOT NULL
DROP DATABASE " +DBParam.DatabaseName+ @";
GO


";
			using (SqlConnection sqlConnection = new SqlConnection(DBParam.GetMasterConnection()))
			{
				try
				{
					sqlConnection.Open();
					SqlScriptHelper.ExecuteScript(sqlConnection, deleteSql);
					Console.WriteLine("Database has been deleted successfully!");
						
					
				}
				finally
				{
					if (sqlConnection.State != ConnectionState.Closed)
					{
						sqlConnection.Close();
					}
				}
			}
		}
	}
	/// <summary>
	/// A helper structure contains params to create/delete db
	/// </summary>
	public struct DatabaseParam
	{
		public string ServerName;
		public string MasterLogin;
		public string MasterPassword;
		
		//
		public string DatabaseName;

        public string GetConnection()
        {
            return "TrustServerCertificate=True;Encrypt=False;" +
                   "SERVER = " + ServerName + "; DATABASE = " + DatabaseName + "; User ID = " + MasterLogin + "; Pwd = " + MasterPassword;
        }
        public string GetMasterConnection()
		{
			return "TrustServerCertificate=True;Encrypt=False;" +
                   "SERVER = " + ServerName + "; DATABASE = master;User ID = " + MasterLogin + "; Pwd = " + MasterPassword;
		}
	}

}
