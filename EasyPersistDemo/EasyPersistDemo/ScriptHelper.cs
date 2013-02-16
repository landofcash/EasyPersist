using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;

namespace EasyPersist.Demo
{
	/// <summary>
	/// Класс для выполнения скриптов.</summary>
	public sealed class SqlScriptHelper
	{
		public static Regex _NonWhiteRegex = new Regex(@"\S+", RegexOptions.ExplicitCapture|RegexOptions.Compiled|RegexOptions.Multiline);

		public static Regex _GoRegex = new Regex(@"^\s*(g|G)(o|O)\s*$", RegexOptions.ExplicitCapture|RegexOptions.Compiled|RegexOptions.Multiline);
		
		/// <summary>
		/// Выполняет SQL-скрипт.</summary>
		/// <param name="command"><see cref="SqlCommand"/>, в котором должно быть задано свойство <see cref="SqlCommand.Connection"/>.</param>
		/// <param name="script">SQL-скрипт.</param>
		/// <remarks>Если текст скрипта разделен ключевым словом "GO", то он выполняется последовательными командами.</remarks>
		/// <exception cref="ArgumentNullException">Если не задана команда или <see cref="SqlCommand.Connection"/>.</exception>
		/// <exception cref="SqlException">Если произошла ошибка при выполнении скрипта на сервере.</exception>
		private static void InternalExecuteScript(SqlCommand command, string script)
		{
			if( command == null || command.Connection == null ) 
			{
				throw new ArgumentNullException();
			}
			string [] commands = _GoRegex.Split(script);
			// инициализируем Regex для поиска "пропусков"
			// если нужно, открываем соединение
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
				// перебираем все команды
				foreach( string commandText in commands )
				{
					if( !_NonWhiteRegex.IsMatch(commandText) )
					{
						// выполнять нечего
						continue;
					}
					// выполняем команду
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
		/// Выполняет SQL-скрипт.</summary>
		/// <param name="transaction">Транзакция, в рамках которой должен выполнится скрипт.</param>
		/// <param name="script">SQL-скрипт.</param>
		/// <remarks>Если текст скрипта разделен ключевым словом "GO", то он выполняется последовательными командами.</remarks>
		/// <exception cref="SqlException">Если произошла ошибка при выполнении скрипта на сервере.</exception>
		public static void ExecuteScript(SqlTransaction transaction, string script)
		{
			CheckParameters(script, transaction);

			SqlCommand command = new SqlCommand();
			command.Connection = transaction.Connection;
			command.Transaction = transaction;
			InternalExecuteScript(command, script);
		}

		/// <summary>
		/// Выполняет SQL-скрипт.</summary>
		/// <param name="connection">Соединение с БД.</param>
		/// <param name="script">SQL-скрипт.</param>
		/// <remarks>Если текст скрипта разделен ключевым словом "GO", то он выполняется последовательными командами.</remarks>
		/// <exception cref="SqlException">Если произошла ошибка при выполнении скрипта на сервере.</exception>
		public static void ExecuteScript(SqlConnection connection, string script)
		{
			CheckParameters(script, connection);
			// инициализируем command
			SqlCommand command = new SqlCommand();
			command.Connection = connection;
			InternalExecuteScript(command, script);
		}
		
		/// <summary>
		/// Выполняет SQL-скрипт, записанный в ресурсном файле.</summary>
		/// <param name="transaction">Транзакция, в рамках которой должен выполнится скрипт.</param>
		/// <param name="resourceName">Название ресурса, содержащего скрипт.</param>
		/// <remarks>Если текст скрипта разделен ключевым словом "GO", то он выполняется последовательными командами.</remarks>
		/// <exception cref="SqlException">Если произошла ошибка при выполнении скрипта на сервере.</exception>
		public static void ExecuteResourceScript(SqlTransaction transaction, string resourceName)
		{
			if (resourceName == null)
				throw new ArgumentNullException("resourceName");
			ExecuteScript(transaction, GetScript(resourceName));
		}
		
		/// <summary>
		/// Выполняет SQL-скрипт, записанный в ресурсном файле.</summary>
		/// <param name="connection">Соединение с БД.</param>
		/// <param name="resourceName">Название ресурса, содержащего скрипт.</param>
		/// <remarks>Если текст скрипта разделен ключевым словом "GO", то он выполняется последовательными командами.</remarks>
		/// <exception cref="SqlException">Если произошла ошибка при выполнении скрипта на сервере.</exception>
		public static void ExecuteResourceScript(SqlConnection connection, string resourceName)
		{
			if (resourceName == null)
				throw new ArgumentNullException("resourceName");
			ExecuteScript(connection, GetScript(resourceName));
		}
		
		/// <summary>
		/// Возвращает скрипт, записанный в ресурсном файле.</summary>
		/// <param name="resourceName">Название ресурса, содержащего скрипт.</param>
		public static string GetScript(string resourceName)
		{
			if (resourceName == null)
				throw new ArgumentNullException("resourceName");
			// получаем поток из вызвавшей сборки
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
		/// Предотвращает создание экземпляра класса.</summary>
		private SqlScriptHelper()
		{
		}
	}
}