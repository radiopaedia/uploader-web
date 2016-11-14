<%@ WebHandler Language="C#" Class="GetUsername" %>

using System;
using System.Web;

public class GetUsername : IHttpHandler {

    public void ProcessRequest (HttpContext context) {
        try
        {
            string refresh = context.Request["refresh"];
            if (string.IsNullOrEmpty(refresh)) { throw new Exception("No refresh token provided"); }
            string username = AOA.GetUsernameFromDb(refresh);
            if (string.IsNullOrEmpty(username)) { throw new Exception("Unable to get username for refresh token"); }
            else { AF.StandardJSON(true, "Successfully retrieved username from refresh token", context, username); }
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