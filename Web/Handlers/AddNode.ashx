<%@ WebHandler Language="C#" Class="AddNode" %>

using System;
using System.Web;

public class AddNode : IHttpHandler {

    public void ProcessRequest (HttpContext context) {
        try
        {
            var node = AF.GetObjectFromJSON<Node>(context);

            //string testNodeString = "{\"IP\":\"172.28.40.151\",\"LocalAe\":\"RAD-RAPI\",\"Port\":104,\"AET\":\"RMHSYNSCP\",\"Description\":\"RAD-RAPI to RMHSYNSCP\",\"Selected\":false,\"Notes\":\"Production Server\",\"LocalStorage\":\"\"}";
            //node = Newtonsoft.Json.JsonConvert.DeserializeObject<Node>(testNodeString);
            
            if (node == null || node == default(Node))
                throw new Exception("Unable to parse input");
            var bm = ACFG.ValidateNode(node);
            if (!bm.Success) { AF.BoolMessageRespond(context, bm); }
            bm = ACFG.CheckIfOnlySelected(node);
            if (!bm.Success) { AF.BoolMessageRespond(context, bm); }
            APetaPoco.SetConnectionString("cn1");
            bm = APetaPoco.PpInsert(node);
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