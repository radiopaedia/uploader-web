<%@ WebHandler Language="C#" Class="EditApi" %>

using System;
using System.Web;

public class EditApi : IHttpHandler {

    public void ProcessRequest (HttpContext context) {
        try
        {
            var api = AF.GetObjectFromJSON<ApiClient>(context);
            if (api == null || api == default(ApiClient))
                throw new Exception("Unable to parse incoming api client");
            APetaPoco.SetConnectionString("cn1");
            string query = "SELECT COUNT ([id]) FROM [ApiClient]";
            var bm = APetaPoco.PpGetScalar<int>(query);
            if (!bm.Success)
                throw new Exception("Unable to get existing count of api clients");
            int count = (int)bm.Data;
            if(count == 0)
            {
                bm = APetaPoco.PpInsert(api);
                if (!bm.Success)
                    throw new Exception("Unable to insert new api client");
            }
            else
            {
                bm = APetaPoco.PpUpdate(api);
                if (!bm.Success)
                    throw new Exception("Unable to update api client");
            }
            AF.StandardJSON(true, "Successfully updated API Client", context);
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