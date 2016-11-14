<%@ WebHandler Language="C#" Class="TestConnection" %>

using System;
using System.Web;
using System.Data.SqlClient;
using System.Configuration;
public class TestConnection : IHttpHandler {

    public void ProcessRequest (HttpContext context) {
        try
        {
            string connectionstring = ConfigurationManager.ConnectionStrings["cn1"].ToString();
            if (string.IsNullOrEmpty(connectionstring))
                throw new Exception("No 'cn1' Connection String provided in web.config file");
            using(SqlConnection conn = new SqlConnection(connectionstring)) {
                conn.Open();
                conn.Close();
            }
            AF.StandardJSON(true, "Connection string is valid", context);
        }
        catch (System.Threading.ThreadAbortException) { return; }
        catch (Exception ex)
        {
            AF.ExceptionRespond(context, ex);
        }
    }

    public bool IsReusable {
        get {
            return false;
        }
    }

}