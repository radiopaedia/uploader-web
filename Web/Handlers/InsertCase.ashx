<%@ WebHandler Language="C#" Class="InsertCase" %>

using System;
using System.Web;

public class InsertCase : IHttpHandler {

    public void ProcessRequest (HttpContext context) {
        ACASE.InsertNewCase(context);
    }

    public bool IsReusable {
        get {
            return false;
        }
    }

}