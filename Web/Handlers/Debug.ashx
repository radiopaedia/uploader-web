<%@ WebHandler Language="C#" Class="Debug" %>

using System;
using System.Web;
using System.IO;

public class Debug : IHttpHandler
{

    public void ProcessRequest(HttpContext context)
    {
        //ADCM.GetPatientStudiesFromId("565022");
        //var token = AOA.TradeAuthCodeForToken("5cea7fd2c2a3cda81819764e605e786d9cb4dcc593ad007638303bb0a7b830bb");
        //AF.StandardJSON(true, "", context, token);
        //ApiClient api = new ApiClient();
        //api.site_id = "ba325141e3db766579feb0ba3b22d7087142053633a02d8d2c19f82bbcfd3d04";
        //api.site_secret = "1f3a57940372fc9fbfb5f58efaee27c5d23d45c03ccf856bbc4c30af17bcb556";
        //api.redirect_url = "http://localhost:51339/Default.aspx";
        //APetaPoco.SetConnectionString("cn1");
        //var bm = APetaPoco.PpInsert(api);
        //AF.BoolMessageRespond(context, bm);

        //ACASE.ClearAllInDbWithCaseId("20160330075029_andyle");
        //ACFG.GetConfigData();
        //ADCM.DicomTestQueryStudy("2016R0076852-1");
        string path = @"d:\XA_Multiframe.dcm";
        using (FileStream fs = File.OpenRead(path))
        {
            int length = (int)fs.Length;
            byte[] buffer;

            using (BinaryReader br = new BinaryReader(fs))
            {
                buffer = br.ReadBytes(length);
            }

            context.Response.Clear();
            context.Response.Buffer = true;
            context.Response.AddHeader("content-disposition", String.Format("attachment;filename={0}", Path.GetFileName(path)));
            context.Response.ContentType = "application/" + Path.GetExtension(path).Substring(1);
            context.Response.BinaryWrite(buffer);
            context.Response.End();
        }
    }

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }

}