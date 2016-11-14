<%@ WebHandler Language="C#" Class="GetPatientHistory" %>

using System;
using System.Web;

public class GetPatientHistory : IHttpHandler {

    public void ProcessRequest (HttpContext context) {
        try
        {
            string patientId = context.Request["patientid"];
            if (string.IsNullOrEmpty(patientId))
                throw new Exception("No patient ID provided");
            var list = ADCM.GetPatientStudiesFromId(patientId);
            AF.StandardJSON(true, "", context, list);
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