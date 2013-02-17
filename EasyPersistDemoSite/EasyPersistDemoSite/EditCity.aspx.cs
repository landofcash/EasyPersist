using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using EasyPersist.Core;
using EasyPersist.Demo.Core;


public partial class EditCity : Page {
    //create the DAO using SQL Connection String
    private DataBaseObjectsFactorySQLServer _dao = new DataBaseObjectsFactorySQLServer(AppConfig.ConnectionString());
    
    private City _city;

    public City City
    {
        get { return _city; }
    }
    protected void Page_Init(object sender, EventArgs e)
    {
        //we create SettlementTypeDropDownList
        SettlementTypeDropDownList.Items.Add(SettlementType.Hamlet.ToString());
        SettlementTypeDropDownList.Items.Add(SettlementType.Village.ToString());
        SettlementTypeDropDownList.Items.Add(SettlementType.Town.ToString());
        SettlementTypeDropDownList.Items.Add(SettlementType.City.ToString());
        SettlementTypeDropDownList.Items.Add(SettlementType.Megalopolis.ToString());
    }
    protected void Page_Load(object sender, EventArgs e) {
        //we edit City if we have cityid in the querystring else we create a new city 
        if (!string.IsNullOrEmpty(Request.QueryString["cityid"]))
        {
            //we have a cityId value need to load the city data from db and bind controls
            int cityId;
            if(int.TryParse(Request.QueryString["cityid"],out cityId) && cityId!=0)
            {
                _city = _dao.getFromDb(cityId, typeof(City)) as City;
                if(_city==null)
                {
                   //city doesn't exists in db throw exception
                    throw new ApplicationException("City doesn't exists in db.  Id:" + cityId);
                }
            }else
            {
                //bad city id throw exception
                throw new ApplicationException("Bad city Id.  Id:" + Request.QueryString["cityid"]);
            }
        }
    }
    protected void Page_PreRender(object sender, EventArgs e)
    {
        if (City != null)
        {
            //now bind the page controls
            DataBindChildren();
            SettlementTypeDropDownList.SelectedValue = City.Type.ToString();
        }
    }
    protected void SaveButton_OnClick(object sender, EventArgs e)
    {
        if (IsValid)
        {
            if (_city == null) {
                //create new city
                _city = new City();
            }
            //load county by county Id in hidden field
            //we check the value of hidden field using validator so no need to recheck it here again
            County county = (County)_dao.getFromDb(int.Parse(CountyIdHidden.Text), typeof(County));
            if (county == null) {
                throw new ApplicationException("Can't find county with id:" + CountyIdHidden.Text);
            }
            //set city properties
            _city.IsActive = IsActiveCheckBox.Checked;
            _city.Name = Server.HtmlEncode(NameTextBox.Text);
            _city.ChangeDate = DateTime.Now;
            _city.Type = (SettlementType)Enum.Parse(typeof(SettlementType), SettlementTypeDropDownList.SelectedValue);
            _city.County = county;
            //save city in db
            _dao.SaveOrUpdate(_city);
            //redirect to edit page
            Response.Redirect("Default.aspx");
        }
    }
    /// <summary>
    /// Validates in county Id is valid
    /// Ve also can validate if county exists in db here but 
    /// I think we don't need to
    /// </summary>
    /// <param name="source"></param>
    /// <param name="args"></param>
    protected void CountyIdCustomValidator_OnServerValidate(object source, ServerValidateEventArgs args)
    {
        if(string.IsNullOrEmpty(CountyIdHidden.Text))
        {
            args.IsValid = false;
        }else
        {
            int id;
            if (!int.TryParse(CountyIdHidden.Text, out id) || id == 0)
            {
                args.IsValid = false;
            }
        }
    }
}