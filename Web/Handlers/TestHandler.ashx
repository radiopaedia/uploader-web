<%@ WebHandler Language="C#" Class="TestHandler" %>

using System;
using System.Web;
using System.Configuration;
using System.Collections.Generic;

using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.Dicom.Iod.Iods;
using ClearCanvas.Dicom.Network;
using ClearCanvas.Dicom.Network.Scu;
public class TestHandler : IHttpHandler
{

    public void ProcessRequest(HttpContext context)
    {
        try
        {
            var list = GetPatientListFromId("1517450");
            AF.StandardJSON(true, "", context, list);
        }
        catch (System.Threading.ThreadAbortException) { return; }
        catch (Exception ex)
        {
            AF.ExceptionRespond(context, ex);
        }
    }

    public static List<PatientSearch> GetPatientListFromId(string patientId)
    {
        var node = GetSelectedNode();
        if (node == null)
            throw new Exception("Unable to get selected DICOM node");
        List<PatientSearch> list = new List<PatientSearch>();
        var findScu = new PatientRootFindScu();
        PatientQueryIod queryMessage = new PatientQueryIod();
        queryMessage.SetCommonTags();
        queryMessage.PatientId = patientId;
        IList<PatientQueryIod> results = findScu.Find(node.LocalAe, node.AET, node.IP, node.Port, queryMessage);
        if (results.Count > 0)
        {
            foreach (var r in results)
            {
                var p = new PatientSearch();
                p.last_name = r.PatientsName.LastName;
                p.first_name = r.PatientsName.FirstName;
                p.dob = r.PatientsBirthDate;
                p.patientid = r.PatientId;
                list.Add(p);
            }
        }
        return list;
    }

    public static Node GetSelectedNode()
    {
        APetaPoco.SetConnectionString("cn1");
        var bm = APetaPoco.PpRetrieveOne<Node>("PACS", "[Selected] = 1");
        if (bm.Success)
            return (Node)bm.Data;
        else
            return null;
    }
    public static CFG GetConfigData()
    {
        CFG cfg = new CFG();
        cfg.ConnectionString = ConfigurationManager.ConnectionStrings["cn1"].ToString();
        //if (!string.IsNullOrEmpty(cfg.ConnectionString))
        //{
        //    APetaPoco.SetConnectionString("cn1");
        //    cfg.SiteApi = GetSiteApiDetails();
        //    var bm = APetaPoco.PpRetrieveList<Node>("PACS");
        //    if (bm.Success)
        //    {
        //        cfg.Nodes = (List<Node>)bm.Data;
        //    }
        //}
        return cfg;
    }
    public static ApiClient GetSiteApiDetails()
    {
        APetaPoco.SetConnectionString("cn1");
        var bm = APetaPoco.PpRetrieveCustomQuery<ApiClient>("SELECT TOP 1 * FROM [ApiClient]");
        if (bm.Success)
        {
            var list = (List<ApiClient>)bm.Data;
            if (list != null && list.Count > 0) { return list[0]; }
            else { return null; }
        }
        else
        {
            return null;
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