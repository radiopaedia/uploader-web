<%@ WebHandler Language="C#" Class="EditNode" %>

using System;
using System.Web;

public class EditNode : IHttpHandler {

    public void ProcessRequest (HttpContext context) {
        try
        {
            var node = AF.GetObjectFromJSON<Node>(context);
            if (node == null || node == default(Node))
                throw new Exception("Unable to parse input");
            var bm = ACFG.ValidateNode(node);
            if (!bm.Success) { AF.BoolMessageRespond(context, bm); }
            bm = ACFG.CheckIfOnlySelected(node);
            if (!bm.Success) { AF.BoolMessageRespond(context, bm); }
            APetaPoco.SetConnectionString("cn1");
            bm = APetaPoco.PpUpdate(node);
            AF.BoolMessageRespond(context, bm);
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