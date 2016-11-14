using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for ACASE
/// </summary>
public static class ACASE
{
    public static void InsertNewCase(HttpContext context)
    {
        string caseId = "";
        var bm = new BoolMessage();
        bm.Data = null;        
        try
        {
            var upCasePackage = AF.GetObjectFromJSON<UploadPackage>(context);
            if (upCasePackage == null || upCasePackage == default(UploadPackage))
            {
                throw new Exception("Unable to parse incoming package");
            }
            APetaPoco.SetConnectionString("cn1");            
            caseId = DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + upCasePackage.username;
            var dCase = new DbCase();
            dCase.age = upCasePackage.case_details.age;
            dCase.body = upCasePackage.case_details.body;
            dCase.case_id = caseId;
            dCase.date = DateTime.Now;
            dCase.diagnostic_certainty_id = upCasePackage.case_details.diagnostic_certainty_id;
            dCase.presentation = upCasePackage.case_details.presentation;
            dCase.status = "PENDING";
            dCase.status_message = "Inserted via Uploader UI";
            dCase.suitable_for_quiz = upCasePackage.case_details.suitable_for_quiz;
            dCase.system_id = upCasePackage.case_details.system_id;
            dCase.title = upCasePackage.case_details.title;
            dCase.username = upCasePackage.username;
            bm = APetaPoco.PpInsert(dCase);
            if (!bm.Success) { throw new Exception("Unable to insert case"); }
            foreach (var st in upCasePackage.studies)
            {
                var dcmSt = ADCM.GetStudyFromStudyUid(st.study_uid);
                var dSt = new DbStudy();                
                dSt.caption = st.caption;
                dSt.case_id = caseId;
                DateTime dt;
                bool dtp = DateTime.TryParseExact(st.date, "dd/MM/yyyy HH:mm", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out dt);
                if (dtp) { dSt.date = dt; }
                if(dcmSt != null && dcmSt != default(Study))
                {
                    dSt.patient_dob = dcmSt.PatientDob;
                    dSt.patient_id = dcmSt.PatientId;
                    dSt.patient_name = dcmSt.PatientSurname.ToUpper() + ", " + dcmSt.PatientFirstname;
                }
                dSt.description = st.description;
                dSt.findings = st.findings;
                dSt.images = st.images;
                dSt.modality = st.modality;
                dSt.position = st.position;
                dSt.study_uid = st.study_uid;                
                bm = APetaPoco.PpInsert(dSt);
                if (!bm.Success) { throw new Exception("Unable to insert study"); }
                foreach (var se in st.series)
                {
                    var dSe = new DbSeries();
                    dSe.case_id = caseId;
                    dSe.description = se.description;
                    dSe.images = se.images;
                    dSe.series_uid = se.series_uid;
                    dSe.study_uid = st.study_uid;
                    dSe.crop_h = se.crop_h;
                    dSe.crop_w = se.crop_w;
                    dSe.crop_x = se.crop_x;
                    dSe.crop_y = se.crop_y;
                    dSe.window_wc = se.window_wc;
                    dSe.window_ww = se.window_ww;
                    dSe.end_image = se.end_image;
                    dSe.every_image = se.every_image;
                    dSe.start_image = se.start_image;
                    bm = APetaPoco.PpInsert(dSe);
                    if (!bm.Success) { throw new Exception("Unable to insert series"); }
                }
            }
            InsertEvent("Successfully inserted case into database:\n" + caseId, "WEB", Newtonsoft.Json.JsonConvert.SerializeObject(dCase), dCase.case_id);
            bm.Success = true;
            bm.Data = null;
            bm.Message = "Successfully inserted case into database";
            AF.BoolMessageRespond(context, bm);
        }
        catch (System.Threading.ThreadAbortException) { return; }
        catch (Exception ex)
        {
            if (!string.IsNullOrEmpty(caseId)) { ClearAllInDbWithCaseId(caseId); }            
            AF.ExceptionRespond(context, ex);
        }                        
    }
    public static void GetCaseList(HttpContext context)
    {
        try
        {
            APetaPoco.SetConnectionString("cn1");
            string username = context.Request["username"];
            if (string.IsNullOrEmpty(username))
                throw new Exception("No refresh provided");
            string status = context.Request["status"];
            if (string.IsNullOrEmpty(status))
                throw new Exception("No status provided");
            string conditionString = string.Format("[username] = '{0}' and [status] = '{1}'", username, status);
            var bm = APetaPoco.PpRetrieveList<DbCase>("Cases", conditionString);
            var resp = new TableResponse();
            resp.Name = status + " Cases";
            if (bm.Success) {
                resp.Cases = (List<DbCase>)bm.Data;
                foreach(var c in resp.Cases)
                {
                    c.study_list = GetStudiesForCase(c.case_id);
                }
            }
            else { AF.BoolMessageRespond(context, bm); }            
            bm = APetaPoco.PpGetScalar<int>("select count([Id]) from Cases where [username] = '" + username + "' and [status] = 'PENDING'");
            if (bm.Success) { resp.PendingCount = (int)bm.Data; }
            else { AF.BoolMessageRespond(context, bm); }
            bm = APetaPoco.PpGetScalar<int>("select count([Id]) from Cases where [username] = '" + username + "' and [status] = 'COMPLETED'");
            if (bm.Success) { resp.CompletedCount = (int)bm.Data; }
            else { AF.BoolMessageRespond(context, bm); }
            bm = APetaPoco.PpGetScalar<int>("select count([Id]) from Cases where [username] = '" + username + "' and [status] = 'ERROR'");
            if (bm.Success) { resp.ErrorCount = (int)bm.Data; }
            else { AF.BoolMessageRespond(context, bm); }
            AF.StandardJSON(true, "Successfully retrieved cases for username: " + username, context, resp);
        }
        catch (System.Threading.ThreadAbortException) { return; }
        catch (Exception ex)
        {            
            AF.ExceptionRespond(context, ex);
        }
    }

    public static void GetSpecificCaseDetails(HttpContext context)
    {
        try
        {
            string caseid = context.Request["caseid"];
            if (string.IsNullOrEmpty(caseid)) { throw new Exception("No case id provided"); }
            APetaPoco.SetConnectionString("cn1");
            var bm = APetaPoco.PpRetrieveList<Event>("Events", "[InternalId] = '" + caseid + "'");
            AF.BoolMessageRespond(context, bm);
        }
        catch (System.Threading.ThreadAbortException) { return; }
        catch (Exception ex)
        {
            AF.ExceptionRespond(context, ex);
        }        
    }

    private static List<DbStudy> GetStudiesForCase(string caseid)
    {
        try
        {
            APetaPoco.SetConnectionString("cn1");
            var bm = APetaPoco.PpRetrieveList<DbStudy>("Studies", "[case_id] = '" + caseid + "'");
            if (!bm.Success || bm.Data == null || bm.Data == default(List<DbStudy>))
                throw new Exception("Unable to retrieve study list for case id: " + caseid);
            var studies = (List<DbStudy>)bm.Data;
            foreach(var st in studies)
            {
                bm = APetaPoco.PpRetrieveList<DbSeries>("Series", string.Format("[case_id] = '{0}' AND [study_uid] = '{1}'", caseid, st.study_uid));
                if (!bm.Success || bm.Data == null || bm.Data == default(List<DbSeries>))
                    throw new Exception("Unable to retrieve series list for case id: " + caseid);
                st.series = (List<DbSeries>)bm.Data;
            }
            return studies;
        }
        catch { return null; }
    }

    public static void ClearAllInDbWithCaseId(string caseid)
    {
        APetaPoco.SetConnectionString("cn1");
        string condition = string.Format("[case_id] = '{0}'", caseid);
        APetaPoco.DeleteSql("Cases", condition);
        APetaPoco.DeleteSql("Studies", condition);
        APetaPoco.DeleteSql("Series", condition);        
    }

    public static void InsertEvent(string msg, string type, string data = null, string dcase = null, string dstudy = null, string dseries = null)
    {
        Event e = new Event();
        e.Type = type;
        e.Message = msg;
        e.TimeStamp = DateTime.Now;
        e.Data = data;
        if (dcase != null)
        {
            e.InternalId = dcase;
        }
        if (dstudy != null)
        {
            e.StudyUid = dstudy;
        }
        if (dseries != null)
        {
            e.SeriesUid = dseries;
        }
        APetaPoco.SetConnectionString("cn1");
        var bm = APetaPoco.PpInsert(e);
    }
}

[PetaPoco.TableName("Events")]
public class Event
{
    public int Id { set; get; }
    public DateTime TimeStamp { set; get; }
    public string Type { set; get; }
    public string InternalId { set; get; }
    public string StudyUid { set; get; }
    public string SeriesUid { set; get; }
    public string Message { set; get; }
    public string Data { set; get; }
}

public class TableResponse
{
    public string Name;
    public List<DbCase> Cases;
    public int CompletedCount;
    public int PendingCount;
    public int ErrorCount;
}

public class DetailsResponse
{
    public DbCase Case;
    public List<Event> Events;
}