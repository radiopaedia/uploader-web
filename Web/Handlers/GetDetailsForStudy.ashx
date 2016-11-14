<%@ WebHandler Language="C#" Class="GetDetailsForStudy" %>

using System;
using System.Web;

public class GetDetailsForStudy : IHttpHandler {

    public void ProcessRequest (HttpContext context) {
        ACASE.GetSpecificCaseDetails(context);
    }

    public bool IsReusable {
        get {
            return false;
        }
    }

}