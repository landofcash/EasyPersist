namespace EasyPersist.Tests
{
    using System;
    using System.IO;
    using EasyPersist.Tests.Helpers;
    using NUnit.Framework;
    using Microsoft.Data.SqlClient;

    [SetUpFixture]
    public class SetupDB : BaseDBDrivenText
    {
        [OneTimeSetUp]
        public void InitDb()
        {
            DBTools.DeleteDatabase(DBParam);
            DBTools.CreateDatabase(DBParam);
            using (SqlConnection sqlConnection = new SqlConnection(DBParam.GetConnection()))
            {
                string createDbSqlFilePath = AppDomain.CurrentDomain.BaseDirectory +
                                             "\\DBScripts\\CreateDatabaseStructureScript.sql";
                if (!File.Exists(createDbSqlFilePath))
                {
                    throw new FileNotFoundException(createDbSqlFilePath);
                }
                else
                {
                    string createDBScript = File.ReadAllText(createDbSqlFilePath);
                    SqlScriptHelper.ExecuteScript(sqlConnection, createDBScript);
                }
            }
        }
    }
}
