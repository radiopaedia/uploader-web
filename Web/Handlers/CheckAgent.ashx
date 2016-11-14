<%@ WebHandler Language="C#" Class="CheckAgent" %>

using System;
using System.Web;
using System.Net;

public class CheckAgent : IHttpHandler {

    public void ProcessRequest (HttpContext context) {
        try
        {

            WebRequest request = WebRequest.Create("http://localhost:1234?check=true");
            request.Method = "GET";
            WebResponse response = request.GetResponse();
            var dataStream = response.GetResponseStream();
            System.IO.StreamReader reader = new System.IO.StreamReader(dataStream);
            string responseFromServer = reader.ReadToEnd();
            //var respObj = JsonConvert.DeserializeObject<UserResponse>(responseFromServer);
            reader.Close();
            dataStream.Close();
            response.Close();
            context.Response.ContentType = "application/json";
            context.Response.ContentEncoding = System.Text.Encoding.UTF8;
            context.Response.Write(responseFromServer);
            context.Response.End();
        }
        catch (System.Threading.ThreadAbortException) { return; }
        catch (Exception ex)
        {
            //using (var stream = ex.Response.GetResponseStream())
            //using (var reader = new System.IO.StreamReader(stream))
            //{
            //    string errorResponse = reader.ReadToEnd();
            //    System.IO.File.WriteAllText(@"c:\temp\templog.txt", errorResponse);
            //}
            AF.ExceptionRespond(context, ex);
        }
    }

    public bool IsReusable {
        get {
            return false;
        }
    }

}