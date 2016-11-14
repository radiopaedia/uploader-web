<%@ WebHandler Language="C#" Class="GetImage" %>

using System;
using System.Web;
using System.Net;

public class GetImage : IHttpHandler {

    public void ProcessRequest (HttpContext context) {
        try
        {
            string studyuid = context.Request["studyuid"];
            string seriesuid = context.Request["seriesuid"];
            if(string.IsNullOrEmpty(studyuid) || string.IsNullOrEmpty(seriesuid)) { throw new Exception("studyuid or seriesuid not provided"); }
            var api = ACFG.GetSiteApiDetails();
            WebRequest request = WebRequest.Create(string.Format("http://localhost:1234?studyuid={0}&seriesuid={1}", studyuid, seriesuid));
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
        catch (WebException ex)
        {
            using (var stream = ex.Response.GetResponseStream())
            using (var reader = new System.IO.StreamReader(stream))
            {
                string errorResponse = reader.ReadToEnd();
                System.IO.File.WriteAllText(@"c:\temp\templog.txt", errorResponse);
            }
            AF.ExceptionRespond(context, ex);
        }
    }

    public bool IsReusable {
        get {
            return false;
        }
    }

}