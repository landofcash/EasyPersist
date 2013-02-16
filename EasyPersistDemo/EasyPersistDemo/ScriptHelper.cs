using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;

namespace EasyPersist.Demo
{
	/// <summary>
	/// ����� ��� ���������� ��������.</summary>
	public sealed class SqlScriptHelper
	{
		public static Regex _NonWhiteRegex = new Regex(@"\S+", RegexOptions.ExplicitCapture|RegexOptions.Compiled|RegexOptions.Multiline);

		public static Regex _GoRegex = new Regex(@"^\s*(g|G)(o|O)\s*$", RegexOptions.ExplicitCapture|RegexOptions.Compiled|RegexOptions.Multiline);
		
		/// <summary>
		/// ��������� SQL-������.</summary>
		/// <param name="command"><see cref="SqlCommand"/>, � ������� ������ ���� ������ �������� <see cref="SqlCommand.Connection"/>.</param>
		/// <param name="script">SQL-������.</param>
		/// <remarks>���� ����� ������� �������� �������� ������ "GO", �� �� ����������� ����������������� ���������.</remarks>
		/// <exception cref="ArgumentNullException">���� �� ������ ������� ��� <see cref="SqlCommand.Connection"/>.</exception>
		/// <exception cref="SqlException">���� ��������� ������ ��� ���������� ������� �� �������.</exception>
		private static void InternalExecuteScript(SqlCommand command, string script)
		{
			if( command == null || command.Connection == null ) 
			{
				throw new ArgumentNullException();
			}
			string [] commands = _GoRegex.Split(script);
			// �������������� Regex ��� ������ "���������"
			// ���� �����, ��������� ����������
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
				// ���������� ��� �������
				foreach( string commandText in commands )
				{
					if( !_NonWhiteRegex.IsMatch(commandText) )
					{
						// ��������� ������
						continue;
					}
					// ��������� �������
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
		/// ��������� SQL-������.</summary>
		/// <param name="transaction">����������, � ������ ������� ������ ���������� ������.</param>
		/// <param name="script">SQL-������.</param>
		/// <remarks>���� ����� ������� �������� �������� ������ "GO", �� �� ����������� ����������������� ���������.</remarks>
		/// <exception cref="SqlException">���� ��������� ������ ��� ���������� ������� �� �������.</exception>
		public static void ExecuteScript(SqlTransaction transaction, string script)
		{
			CheckParameters(script, transaction);

			SqlCommand command = new SqlCommand();
			command.Connection = transaction.Connection;
			command.Transaction = transaction;
			InternalExecuteScript(command, script);
		}

		/// <summary>
		/// ��������� SQL-������.</summary>
		/// <param name="connection">���������� � ��.</param>
		/// <param name="script">SQL-������.</param>
		/// <remarks>���� ����� ������� �������� �������� ������ "GO", �� �� ����������� ����������������� ���������.</remarks>
		/// <exception cref="SqlException">���� ��������� ������ ��� ���������� ������� �� �������.</exception>
		public static void ExecuteScript(SqlConnection connection, string script)
		{
			CheckParameters(script, connection);
			// �������������� command
			SqlCommand command = new SqlCommand();
			command.Connection = connection;
			InternalExecuteScript(command, script);
		}
		
		/// <summary>
		/// ��������� SQL-������, ���������� � ��������� �����.</summary>
		/// <param name="transaction">����������, � ������ ������� ������ ���������� ������.</param>
		/// <param name="resourceName">�������� �������, ����������� ������.</param>
		/// <remarks>���� ����� ������� �������� �������� ������ "GO", �� �� ����������� ����������������� ���������.</remarks>
		/// <exception cref="SqlException">���� ��������� ������ ��� ���������� ������� �� �������.</exception>
		public static void ExecuteResourceScript(SqlTransaction transaction, string resourceName)
		{
			if (resourceName == null)
				throw new ArgumentNullException("resourceName");
			ExecuteScript(transaction, GetScript(resourceName));
		}
		
		/// <summary>
		/// ��������� SQL-������, ���������� � ��������� �����.</summary>
		/// <param name="connection">���������� � ��.</param>
		/// <param name="resourceName">�������� �������, ����������� ������.</param>
		/// <remarks>���� ����� ������� �������� �������� ������ "GO", �� �� ����������� ����������������� ���������.</remarks>
		/// <exception cref="SqlException">���� ��������� ������ ��� ���������� ������� �� �������.</exception>
		public static void ExecuteResourceScript(SqlConnection connection, string resourceName)
		{
			if (resourceName == null)
				throw new ArgumentNullException("resourceName");
			ExecuteScript(connection, GetScript(resourceName));
		}
		
		/// <summary>
		/// ���������� ������, ���������� � ��������� �����.</summary>
		/// <param name="resourceName">�������� �������, ����������� ������.</param>
		public static string GetScript(string resourceName)
		{
			if (resourceName == null)
				throw new ArgumentNullException("resourceName");
			// �������� ����� �� ��������� ������
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
				throw new ArgumentNullException("transaction");
			if (script == null)
				throw new ArgumentNullException("script");
		}

		private static void CheckParameters(string script, SqlConnection connection)
		{
			if (connection == null)
				throw new ArgumentNullException("transaction");
			if (script == null)
				throw new ArgumentNullException("script");
		}

		/// <summary>
		/// ������������� �������� ���������� ������.</summary>
		private SqlScriptHelper()
		{
		}
	}
}