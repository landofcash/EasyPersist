using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using EasyPersist.Core;
using EasyPersist.Core.IFaces;
using EasyPersist.Demo.Core;


namespace EasyPersist.Demo {
    
    class Program {
        //SQL server connection string
        protected static DatabaseParam DBParam = new DatabaseParam()
            {
                ServerName = "localhost\\SQLEXPRESS",
                MasterPassword = "z",
                MasterLogin = "sa",
                DatabaseName = "EasyPersistDemo"
            };

        private DataBaseObjectsFactorySQLServer _dao = new DataBaseObjectsFactorySQLServer(DBParam.GetConnection());

        public DataBaseObjectsFactorySQLServer Dao
        {
            get { return _dao; }
        }

        static void Main(string[] args)
        {
            Program program = new Program();
            program.InitDb();
            //create states
            Console.WriteLine(""); Console.WriteLine("------------- Create states");
            program.CreateStates();
            // create some counties for states
            Console.WriteLine(""); Console.WriteLine("------------- Create some counties for states");
            program.CreateCounties();
            // create cities
            Console.WriteLine("");Console.WriteLine("------------- Create cities");
            program.CreateCities();
            Console.WriteLine("");Console.WriteLine("------------- Load custom data and bind helper object");
            //lets load custom data and bid helper object
            string locationSql = @"
SELECT City.CityId as LocCityId, City.Name as CityName, County.Name as CountyName, State.Name as StateName
FROM City 
INNER JOIN County ON City.CountyId=County.CountyId
INNER JOIN State ON County.StateId=State.StateId
ORDER BY City.Name, County.Name, State.Name
";
            ArrayList locations = program.Dao.GetReadOnlyListFromDb(new SqlCommand(locationSql), typeof(TestLocation));
            foreach (TestLocation location in locations)
            {
                Console.WriteLine(location.ToString());
            }
            Console.WriteLine(""); 
            Console.WriteLine("Done! Press Enter to quit.");
            Console.ReadLine();
        }
        public void CreateStates()
        {
            //create 2 states
            State florida = new State(); //create new instance of state class
            florida.Name = "Florida"; //set name property
            _dao.SaveOrUpdate(florida); //save in db 
            //note that we don't set an ID property as it is Identity auto increment column in db 
            //the object ID is updated when saved
            Console.WriteLine("Saved State " + florida.Name + " ID:" + florida.Id);
            
            //Now create other state
            State california = new State(); //create new instance of state class
            california.Name = "California!!!"; //set "incorrect" name property
            _dao.SaveOrUpdate(california); //SAVE (INSERT) in db 
            Console.WriteLine("Saved State " + california.Name + " ID:" + california.Id);
            //now let's update the california name
            california.Name = "California";
            _dao.SaveOrUpdate(california); //UPDATE
            Console.WriteLine("Updated State " + california.Name + " ID:" + california.Id);
        }
        
        public void CreateCounties() {
            //first let's load the state we want to add counties to (Florida)
            //a usual SQL query & Command
            string stateSql = @"SELECT top 1 * FROM State WHERE State.Name = @name";
            SqlCommand sqlCommand = new SqlCommand(stateSql);//create SQL command using SQL query
            sqlCommand.Parameters.AddWithValue("name", "Florida"); // add query parameters
            //load state from DB (sql query should return ONE row)
            State state = (State)_dao.getFromDb(sqlCommand, typeof (State));
            //now we create counties for this state
            if(state!=null)
            {
                Console.WriteLine("Loaded State " + state.Name + " ID:" + state.Id);
                County county = new County();
                county.Name = "Duval";
                county.State = state;
                _dao.SaveOrUpdate(county);
                Console.WriteLine("Saved County " + county.Name + " ID:" + county.Id + " State:" + state.Name);
                
                //other county
                county = new County();
                county.Name = "Baker";
                county.State = state;
                _dao.SaveOrUpdate(county);
                Console.WriteLine("Saved County " + county.Name + " ID:" + county.Id + " State:" + state.Name);
            }
        }
        
        public void CreateCities() {
            //let's create cities for all counties we have
            //first load the list of counties (using usual sql)
            string countiesSql = "SELECT * FROM County";
            IList<IPersistent> counties = _dao.getListFromDb(countiesSql, typeof (County));
            
            //itereate thru loaded counties and create cities
            foreach (County county in counties)
            {
                if(county.Name=="Duval")
                {
                    City city = new City();
                    city.Name = "Jacksonville";
                    city.ChangeDate = DateTime.Now;
                    city.IsActive = true;
                    city.Type = SettlementType.City;
                    city.County = county;
                    _dao.SaveOrUpdate(city);
                    Console.WriteLine("Saved City " + city.Name + " ID:" + city.Id);
                    
                    //one more
                    city = new City();
                    city.Name = "Baldwin";
                    //city.ChangeDate = DateTime.Now; //we don't set the date it should be null in db
                    city.IsActive = false;
                    city.Type = SettlementType.Town;
                    city.County = county;
                    _dao.SaveOrUpdate(city);
                    Console.WriteLine("Saved City " + city.Name + " ID:" + city.Id);
                }
                if (county.Name == "Baker")
                {
                    //one more
                    City city = new City();
                    city.Name = "Baxter";
                    city.ChangeDate = DateTime.Now;
                    city.IsActive = false;
                    city.Type = SettlementType.Town;
                    city.County = county;
                    _dao.SaveOrUpdate(city);
                    Console.WriteLine("Saved City " + city.Name + " ID:" + city.Id);
                }
            }
        }



        public void InitDb()
        {

            DBTools.DeleteDatabase(DBParam);
            DBTools.CreateDatabase(DBParam);
            using (SqlConnection sqlConnection = new SqlConnection(DBParam.GetConnection()))
            {
                string createDbSqlFilePath = AppDomain.CurrentDomain.BaseDirectory +
                                             "\\CreateDatabaseStructureScript.sql";
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

