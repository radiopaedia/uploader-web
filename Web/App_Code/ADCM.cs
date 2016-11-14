using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.Dicom.Iod.Iods;
using ClearCanvas.Dicom.Network;
using ClearCanvas.Dicom.Network.Scu;

/// <summary>
/// Code Behind for DICOM related Handlers
/// </summary>
public static class ADCM
{
    public static Study GetStudyFromAccession(string accession)
    {
        var node = GetSelectedNode();
        if (node == null)
            throw new Exception("Unable to get selected DICOM node");
        StudyRootFindScu findScu = new StudyRootFindScu();
        StudyQueryIod queryMessage = new StudyQueryIod();
        queryMessage.SetCommonTags();
        queryMessage.AccessionNumber = accession;
        IList<StudyQueryIod> results = findScu.Find(node.LocalAe, node.AET, node.IP, node.Port, queryMessage);
        if (results.Count == 1)
        {
            Study st = new Study();
            st.Accession = results[0].AccessionNumber;
            st.Images = (int)results[0].NumberOfStudyRelatedInstances;
            st.PatientDob = results[0].PatientsBirthDate;
            st.PatientFirstname = results[0].PatientsName.FirstName;
            st.PatientId = results[0].PatientId;
            st.PatientSurname = results[0].PatientsName.LastName;
            st.StudyDate = results[0].StudyDate;
            st.StudyDescription = results[0].StudyDescription;
            st.StudyModality = results[0].ModalitiesInStudy;
            st.StudyUid = results[0].StudyInstanceUid;
            if(st.StudyDate != null && st.PatientDob != null)
            {
                int age = st.StudyDate.Value.Year - st.PatientDob.Value.Year;
                if (st.PatientDob.Value > st.StudyDate.Value.AddYears(-age)) age--;
                st.PatientAge = age;
            }
            StudyRootFindScu seriesFindScu = new StudyRootFindScu();
            SeriesQueryIod seriesQuery = new SeriesQueryIod();
            seriesQuery.SetCommonTags();
            seriesQuery.StudyInstanceUid = results[0].StudyInstanceUid;
            IList<SeriesQueryIod> seriesResults = seriesFindScu.Find(node.LocalAe, node.AET, node.IP, node.Port, seriesQuery);
            if (seriesResults.Count > 0)
            {
                st.Series = new List<Series>();
                foreach (var se in seriesResults)
                {
                    Series s = new Series();
                    s.StudyUid = results[0].StudyInstanceUid;
                    s.Images = (int)se.NumberOfSeriesRelatedInstances;
                    s.SeriesDescription = se.SeriesDescription;
                    s.SeriesModality = se.Modality;
                    s.SeriesUid = se.SeriesInstanceUid;
                    st.Series.Add(s);
                }
            }
            return st;
        }
        else
        {
            throw new Exception("No study found");
        }
    }
    public static Study GetStudyFromStudyUid(string uid)
    {
        var node = GetSelectedNode();
        if (node == null)
            throw new Exception("Unable to get selected DICOM node");
        StudyRootFindScu findScu = new StudyRootFindScu();
        StudyQueryIod queryMessage = new StudyQueryIod();
        queryMessage.SetCommonTags();
        queryMessage.StudyInstanceUid = uid;
        IList<StudyQueryIod> results = findScu.Find(node.LocalAe, node.AET, node.IP, node.Port, queryMessage);
        if (results.Count == 1)
        {
            Study st = new Study();
            st.Accession = results[0].AccessionNumber;
            st.Images = (int)results[0].NumberOfStudyRelatedInstances;
            st.PatientDob = results[0].PatientsBirthDate;
            st.PatientFirstname = results[0].PatientsName.FirstName;
            st.PatientId = results[0].PatientId;
            st.PatientSurname = results[0].PatientsName.LastName;
            st.StudyDate = results[0].StudyDate;
            st.StudyDescription = results[0].StudyDescription;
            st.StudyModality = results[0].ModalitiesInStudy;
            st.StudyUid = results[0].StudyInstanceUid;
            if (st.StudyDate != null && st.PatientDob != null)
            {
                int age = st.StudyDate.Value.Year - st.PatientDob.Value.Year;
                if (st.PatientDob.Value > st.StudyDate.Value.AddYears(-age)) age--;
                st.PatientAge = age;
            }            
            return st;
        }
        else
        {
            throw new Exception("No study found");
        }
    }
    public static void DicomTestQueryStudy(string accession)
    {
        StudyRootFindScu findScu = new StudyRootFindScu();
        StudyQueryIod queryMessage = new StudyQueryIod();
        queryMessage.SetCommonTags();
        queryMessage.AccessionNumber = accession;
        IList<StudyQueryIod> results = findScu.Find("ANDYPACS3", "RMHSYNSCP", "172.28.40.151", 104, queryMessage);
        if (results.Count == 1)
        {
            Study st = new Study();
            st.Accession = results[0].AccessionNumber;
            st.Images = (int)results[0].NumberOfStudyRelatedInstances;
            st.PatientDob = results[0].PatientsBirthDate;
            st.PatientFirstname = results[0].PatientsName.FirstName;
            st.PatientId = results[0].PatientId;
            st.PatientSurname = results[0].PatientsName.LastName;
            st.StudyDate = results[0].StudyDate;
            st.StudyDescription = results[0].StudyDescription;
            st.StudyModality = results[0].ModalitiesInStudy;
            st.StudyUid = results[0].StudyInstanceUid;
            if (st.StudyDate != null && st.PatientDob != null)
            {
                int age = st.StudyDate.Value.Year - st.PatientDob.Value.Year;
                if (st.PatientDob.Value > st.StudyDate.Value.AddYears(-age)) age--;
                st.PatientAge = age;
            }            
        }
    }
    public static List<Study> GetPatientStudiesFromId(string patientId)
    {
        var node = GetSelectedNode();
        if (node == null)
            throw new Exception("Unable to get selected DICOM node");
        StudyRootFindScu findScu = new StudyRootFindScu();
        StudyQueryIod queryMessage = new StudyQueryIod();
        queryMessage.SetCommonTags();
        queryMessage.PatientId = patientId;
        IList<StudyQueryIod> results = findScu.Find(node.LocalAe, node.AET, node.IP, node.Port, queryMessage);
        if (results.Count > 0)
        {
            List<Study> stList = new List<Study>();

            foreach(var st in results)
            {
                var nst = new Study();
                nst.Accession = st.AccessionNumber;
                nst.Images = (int)st.NumberOfStudyRelatedInstances;
                nst.PatientDob = st.PatientsBirthDate;
                nst.PatientFirstname = st.PatientsName.FirstName;
                nst.PatientId = st.PatientId;
                nst.PatientSurname = st.PatientsName.LastName;
                nst.StudyDate = st.StudyDate;
                nst.StudyDescription = st.StudyDescription;
                nst.StudyModality = st.ModalitiesInStudy;               
                nst.StudyUid = st.StudyInstanceUid;
                if (nst.StudyDate != null && nst.PatientDob != null)
                {
                    int age = nst.StudyDate.Value.Year - nst.PatientDob.Value.Year;
                    if (nst.PatientDob.Value > nst.StudyDate.Value.AddYears(-age)) age--;
                    nst.PatientAge = age;
                }
                StudyRootFindScu seriesFindScu = new StudyRootFindScu();
                SeriesQueryIod seriesQuery = new SeriesQueryIod();
                seriesQuery.SetCommonTags();
                seriesQuery.StudyInstanceUid = st.StudyInstanceUid;
                IList<SeriesQueryIod> seriesResults = seriesFindScu.Find(node.LocalAe, node.AET, node.IP, node.Port, seriesQuery);
                if (seriesResults.Count > 0)
                {
                    nst.Series = new List<Series>();
                    foreach (var se in seriesResults)
                    {
                        if((int)se.NumberOfSeriesRelatedInstances < 1) { continue; }
                        if (nst.StudyModality == "SR" && se.Modality != "SR") { nst.StudyModality = se.Modality; }
                        Series s = new Series();
                        s.StudyUid = results[0].StudyInstanceUid;
                        s.Images = (int)se.NumberOfSeriesRelatedInstances;
                        s.SeriesDescription = se.SeriesDescription;
                        s.SeriesModality = se.Modality;
                        s.SeriesUid = se.SeriesInstanceUid;
                        nst.Series.Add(s);
                    }
                }
                if (nst.StudyModality == "SR") { nst.StudyModality = ""; }
                stList.Add(nst);
            }
            return stList;
        }
        else
        {
            throw new Exception("Unable to find studies for patient on PACS");
        }
    }

    public static List<DbCase> FindAlreadyUploaded(string patientid)
    {
        APetaPoco.SetConnectionString("cn1");
        var bm = APetaPoco.PpRetrieveList<DbStudy>("Studies", "[patient_id] = '" + patientid + "'");
        if (!bm.Success) { return null; }
        var studies = (List<DbStudy>)bm.Data;
        if(studies == null || studies.Count < 1) { return null; }
        var cases = new List<DbCase>();
        string currentCaseId = string.Empty;
        foreach(var st in studies)
        {
            if(currentCaseId != st.case_id)
            {
                currentCaseId = st.case_id;
                bm = APetaPoco.PpRetrieveOne<DbCase>("Cases", "[case_id] = '" + st.case_id + "'");
                if (bm.Success)
                {
                    var ncase = (DbCase)bm.Data;
                    ncase.study_list = new List<DbStudy>();
                    ncase.study_list.Add(st);
                    cases.Add(ncase);
                }
            }
            else
            {
                foreach(var c in cases)
                {
                    if(c.case_id == st.case_id)
                    {
                        c.study_list.Add(st);
                        break;
                    }
                }
            }
        }
        foreach(var c in cases)
        {
            foreach(var st in c.study_list)
            {
                st.series = GetDbSeriesForDbStudy(st);
            }
        }
        return cases;
    }

    private static List<DbSeries> GetDbSeriesForDbStudy(DbStudy study)
    {
        APetaPoco.SetConnectionString("cn1");
        var bm = APetaPoco.PpRetrieveList<DbSeries>("Series", string.Format("[study_uid] = '{0}' AND [case_id] = '{1}'", study.study_uid, study.case_id));
        if (!bm.Success) { return null; }
        return (List<DbSeries>)bm.Data;
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

    public static bool EchoNode(Node node)
    {
        try
        {

            VerificationScu scu = new VerificationScu();
            var result = scu.Verify(node.LocalAe, node.AET, node.IP, node.Port);
            if (result == VerificationResult.Success)            
                return true;            
            else
                return false;
        }
        catch { return false; }
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
        if(results.Count > 0)
        {
            foreach(var r in results)
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
    
}

public class PatientSearch
{
    public string last_name;
    public string first_name;
    public DateTime? dob;
    public string patientid;
}