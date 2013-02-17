using System;
using System.Collections.Generic;
using System.Web.UI;
using EasyPersist.Core;
using EasyPersist.Core.IFaces;
using EasyPersist.Demo.Core;


namespace EasyPersistDemoSite {
    public partial class _Default : Page {
        //create the DAO using SQL Connection String
        private DataBaseObjectsFactorySQLServer _dao = new DataBaseObjectsFactorySQLServer(AppConfig.ConnectionString());
        
        /// <summary>
        /// Retrieve data from DB
        /// And bind controls with data
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e) {
            //SQL querry to get all cities
            string rescanSql = "SELECT * FROM City ORDER BY Name";
            
            //load the list of Cities
            IList<IPersistent> rescans = _dao.getListFromDb(rescanSql, typeof(City));
            
            //set the Gridview dataSource and bind it
            CitiesRepeater.DataSource = rescans;
            CitiesRepeater.DataBind();
        }
    }
    
}
