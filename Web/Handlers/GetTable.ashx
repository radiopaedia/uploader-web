<%@ WebHandler Language="C#" Class="GetTable" %>

using System;
using System.Web;

public class GetTable : IHttpHandler {

    public void ProcessRequest (HttpContext context) {
        ACASE.GetCaseList(context);
    }

    public bool IsReusable {
        get {
            return false;
        }
    }

}