using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using EasyPersist.Tests.Helpers;
using NUnit.Framework;

namespace EasyPersist.Tests
{
    [SetUpFixture]
    public class SetupDB : BaseDBDrivenText
    {
        [SetUp]
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
