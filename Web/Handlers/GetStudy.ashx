<%@ WebHandler Language="C#" Class="GetStudy" %>

using System;
using System.Web;
using System.Collections.Generic;

public class GetStudy : IHttpHandler {

    public void ProcessRequest (HttpContext context) {
        try
        {
            string patientid = context.Request["patientid"];
            if (string.IsNullOrEmpty(patientid))
                throw new Exception("No patient id provided");
            var list = ADCM.GetPatientStudiesFromId(patientid);
            var respobj = new InitialPatientSearch();
            respobj.PatientStudies = list;
            respobj.UploadedCases = ADCM.FindAlreadyUploaded(patientid);
            AF.StandardJSON(true, "", context, respobj);
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
public class InitialPatientSearch
{
    public List<DbCase> UploadedCases { set; get; }
    public List<Study> PatientStudies { set; get; }
}