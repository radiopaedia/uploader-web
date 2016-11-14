<%@ WebHandler Language="C#" Class="GetUserDetailsFromApi" %>

using System;
using System.Web;

public class GetUserDetailsFromApi : IHttpHandler {

    public void ProcessRequest (HttpContext context) {
        try
        {
            string token = context.Request["token"];
            if (string.IsNullOrEmpty(token)) { throw new Exception("No token provided"); }
            var userDetails = AOA.GetUserName(token);
            AF.StandardJSON(true, "Successfully retrieved user details via API", context, userDetails);
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