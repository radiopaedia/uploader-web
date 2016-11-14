<%@ WebHandler Language="C#" Class="GetConfig" %>

using System;
using System.Web;

public class GetConfig : IHttpHandler {

    public void ProcessRequest (HttpContext context) {
        try
        {
            var cfg = ACFG.GetConfigData();
            AF.StandardJSON(true, "", context, cfg);
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