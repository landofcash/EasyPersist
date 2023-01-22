namespace EasyPersist.Tests.Helpers
{
    using System;
    using System.Data;
    using System.IO;
    using System.Reflection;
    using System.Text.RegularExpressions;
    using Microsoft.Data.SqlClient;

    /// <summary>
    /// Execute scripts
	/// </summary>
	public static class SqlScriptHelper
	{
		public static Regex _NonWhiteRegex = new Regex(@"\S+", RegexOptions.ExplicitCapture|RegexOptions.Compiled|RegexOptions.Multiline);

		public static Regex _GoRegex = new Regex(@"^\s*(g|G)(o|O)\s*$", RegexOptions.ExplicitCapture|RegexOptions.Compiled|RegexOptions.Multiline);
		
		/// <summary>
		/// Executes SQL-script.</summary>
		/// <param name="command"><see cref="SqlCommand"/>, must specify the property <see cref="SqlCommand.Connection"/>.</param>
		/// <param name="script">SQL-script.</param>
		/// <remarks>If the script is split with "GO" commands, then it will be executed with multiple sequential commands.</remarks>
		/// <exception cref="ArgumentNullException">If the command is not  specified or <see cref="SqlCommand.Connection"/>.</exception>
		/// <exception cref="SqlException">If the sql execution error happened on the SQL Server.</exception>
		private static void InternalExecuteScript(SqlCommand command, string script)
		{
			if( command == null || command.Connection == null ) 
			{
				throw new ArgumentNullException();
			}
			string [] commands = _GoRegex.Split(script);
			bool isNeedCloseConnection;
			if( command.Connection.State == ConnectionState.Closed )
			{
				command.Connection.Open();
				isNeedCloseConnection = true;
			}
			else
			{
				isNeedCloseConnection = false;
			}
			try
			{
				foreach( string commandText in commands )
				{
					if( !_NonWhiteRegex.IsMatch(commandText) )
					{
						continue;
					}
					command.CommandText = commandText;
					command.ExecuteNonQuery();
				}
			}
			finally
			{
				if( isNeedCloseConnection )
				{
					command.Connection.Close();
				}
			}
		}

		/// <summary>
		/// Executes SQL-script.
		/// </summary>
		/// <param name="transaction">Transaction to execute the script.</param>
		/// <param name="script">SQL-script.</param>
        /// <exception cref="ArgumentNullException">If the command is not  specified or <see cref="SqlCommand.Connection"/>.</exception>
        /// <exception cref="SqlException">If the sql execution error happened on the SQL Server.</exception>
		public static void ExecuteScript(SqlTransaction transaction, string script)
		{
			CheckParameters(script, transaction);

			SqlCommand command = new SqlCommand();
			command.Connection = transaction.Connection;
			command.Transaction = transaction;
			InternalExecuteScript(command, script);
		}

		/// <summary>
		/// Executes SQL-script.
		/// </summary>
		/// <param name="connection">DB connection.</param>
		/// <param name="script">SQL-script.</param>
        /// <exception cref="ArgumentNullException">If the command is not  specified or <see cref="SqlCommand.Connection"/>.</exception>
        /// <exception cref="SqlException">If the sql execution error happened on the SQL Server.</exception>
		public static void ExecuteScript(SqlConnection connection, string script)
		{
			CheckParameters(script, connection);
			SqlCommand command = new SqlCommand();
			command.Connection = connection;
			InternalExecuteScript(command, script);
		}
		
		/// <summary>
		/// Executes SQL-script from the resource file.</summary>
		/// <param name="transaction">Transaction to execute the script.</param>
		/// <param name="resourceName">The name of the resource with the script.</param>
        /// <exception cref="ArgumentNullException">If the command is not  specified or <see cref="SqlCommand.Connection"/>.</exception>
        /// <exception cref="SqlException">If the sql execution error happened on the SQL Server.</exception>
		public static void ExecuteResourceScript(SqlTransaction transaction, string resourceName)
		{
			if (resourceName == null)
				throw new ArgumentNullException(nameof(resourceName));
			ExecuteScript(transaction, GetScript(resourceName));
		}
		
		/// <summary>
		/// Executes SQL-script from the resource file.</summary>
		/// <param name="connection">DB connection</param>
		/// <param name="resourceName">The name of the resource with the script.</param>
        /// <remarks>If the script is split with "GO" commands, then it will be executed with multiple sequential commands.</remarks>
        /// <exception cref="SqlException">If the sql execution error happened on the SQL Server.</exception>
		public static void ExecuteResourceScript(SqlConnection connection, string resourceName)
		{
			if (resourceName == null)
				throw new ArgumentNullException(nameof(resourceName));
			ExecuteScript(connection, GetScript(resourceName));
		}
		
		/// <summary>
		/// Returns script from the resource file
		/// </summary>
		/// <param name="resourceName">Resource name with the script.</param>
		public static string GetScript(string resourceName)
		{
            if (resourceName == null)
            {
                throw new ArgumentNullException(nameof(resourceName));
            }

            Assembly assembly = Assembly.GetExecutingAssembly();
			Stream stream = assembly.GetManifestResourceStream(assembly.GetName().Name+"."+resourceName);
			using( StreamReader streamReader = new StreamReader(stream) )
			{
				return streamReader.ReadToEnd();
			}
		}

		private static void CheckParameters(string script, SqlTransaction transaction)
		{
			if (transaction == null)
				throw new ArgumentNullException(nameof(transaction));
			if (script == null)
				throw new ArgumentNullException(nameof(script));
		}

		private static void CheckParameters(string script, SqlConnection connection)
		{
			if (connection == null)
				throw new ArgumentNullException(nameof(connection));
			if (script == null)
				throw new ArgumentNullException(nameof(script));
		}
    }
}