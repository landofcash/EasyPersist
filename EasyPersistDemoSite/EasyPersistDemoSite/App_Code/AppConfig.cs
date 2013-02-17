using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

/// <summary>
/// Summary description for AppConfig
/// </summary>
public static class AppConfig {
    public static string ConnectionString() {
        return ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
    }
}
