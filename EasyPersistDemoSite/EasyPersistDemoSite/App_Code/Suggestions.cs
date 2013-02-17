using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web.Script.Services;
using System.Web.Services;
using ConvincingMail.AdvancedAutoSuggest;
using EasyPersist.Core;
using EasyPersist.Core.Attributes;


/// <summary>
/// Summary description for Suggestions
/// </summary>
[WebService(Namespace = "http://convincingmail.com/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
public class Suggestions : WebService {
    private DataBaseObjectsFactorySQLServer _dao = new DataBaseObjectsFactorySQLServer(AppConfig.ConnectionString());
    [WebMethod()]
    [ScriptMethod()]
    public string CountySuggest(string tryValue, string[] additionalParams) {
        //SQL Query to load cities
        string loadCitySQL = @"
SELECT distinct top 7 County.CountyId as CountyId, County.Name as CountyName, State.Name as StateName 
FROM County 
INNER JOIN State ON State.StateId=County.StateId 
WHERE (County.Name like @tryValue OR County.Name like @tryValue1) ";
        SqlCommand sqlCommand = new SqlCommand(loadCitySQL);
        //bind query parameters
        sqlCommand.Parameters.AddWithValue("tryValue", "% " + tryValue + "%");
        sqlCommand.Parameters.AddWithValue("tryValue1", tryValue + "%");
        
        //load data from DataBase
        //note that we use read only helper object here CountySuggestionHelper
        ArrayList items = _dao.GetReadOnlyListFromDb(sqlCommand, typeof(CountySuggestionHelper));
        //now create the suggestions list
        List<SuggestionItem> suggestionItems = new List<SuggestionItem>(items.Count);
        foreach (CountySuggestionHelper item in items)
        {
            //create SuggestionItem
            SuggestionItem suggestionItem = new SuggestionItem();
            suggestionItem.Title = item.Name;
            suggestionItem.Description = item.StateName;
            suggestionItem.Id = item.Id.ToString();
            //add item to the list
            suggestionItems.Add(suggestionItem);
        }
        return SuggestionItem.SuggestionArrayToJSON(suggestionItems.ToArray(), tryValue);
    }
    
    /// <summary>
    /// a helper class to load SuggestionItem data
    /// </summary>
    internal class CountySuggestionHelper {
        private int _id;
        private string _name;
        private string _stateName;
        [PersistentProperty("CountyId", DbType.Int32)]
        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }
        [PersistentProperty("CountyName", DbType.String)]
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        [PersistentProperty("StateName", DbType.String)]
        public string StateName
        {
            get { return _stateName; }
            set { _stateName = value; }
        }
    }
}