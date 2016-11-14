using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Class for Objects storage
/// </summary>
public class Series
{
    public string StudyUid { set; get; }
    public string SeriesUid { set; get; }
    public int Images { set; get; }
    public string SeriesDescription { set; get; }
    public string SeriesModality { set; get; }

}
public class Study
{
    public string StudyUid { set; get; }
    public string Accession { set; get; }
    public string StudyDescription { set; get; }
    public string PatientSurname { set; get; }
    public string PatientFirstname { set; get; }
    public DateTime? PatientDob { set; get; }
    public string PatientId { set; get; }
    public int Images { set; get; }
    public string StudyModality { set; get; }
    public DateTime? StudyDate { set; get; }
    public List<Series> Series { set; get; }
    public int PatientAge { set; get; }
}
public class UploadPackage
{
    public UploadCase case_details { set; get; }
    public List<UploadStudy> studies { set; get; }
    public string username { set; get; }
}
public class UploadCase
{
    public string title { set; get; }
    public int system_id { set; get; }
    public int diagnostic_certainty_id { set; get; }
    public bool suitable_for_quiz { set; get; }
    public string presentation { set; get; }
    public int age { set; get; }
    public string body { set; get; }
}
public class UploadStudy
{
    public List<UploadSeries> series { set; get; }
    public string study_uid { set; get; }
    public string description { set; get; }
    public string date { set; get; }
    public string modality { set; get; }
    public string findings { set; get; }
    public string caption { set; get; }
    public int images { set; get; }
    public int position { set; get; }
}
public class UploadSeries
{
    public string series_uid { set; get; }
    public int images { set; get; }
    public string description { set; get; }
    public int? crop_x { set; get; }
    public int? crop_y { set; get; }
    public int? crop_h { set; get; }
    public int? crop_w { set; get; }
    public int? window_wc { set; get; }
    public int? window_ww { set; get; }
    public int? every_image { set; get; }
    public int? start_image { set; get; }
    public int? end_image { set; get; }
}

[PetaPoco.TableName("Cases")]
public class DbCase
{
    public int Id { set; get; }
    public string case_id { set; get; }
    public string username { set; get; }
    public DateTime date { set; get; }
    public string status { set; get; }
    public string status_message { set; get; }
    public string title { set; get; }
    public int system_id { set; get; }
    public int diagnostic_certainty_id { set; get; }
    public bool suitable_for_quiz { set; get; }
    public string presentation { set; get; }
    public int age { set; get; }
    public string body { set; get; }
    public string r_case_id { set; get; }
    [PetaPoco.Ignore]
    public List<DbStudy> study_list { set; get; }
}
[PetaPoco.TableName("Series")]
public class DbSeries
{
    public int Id { set; get; }
    public string case_id { set; get; }
    public string study_uid { set; get; }
    public string description { set; get; }
    public int images { set; get; }
    public string series_uid { set; get; }
    public int? crop_x { set; get; }
    public int? crop_y { set; get; }
    public int? crop_h { set; get; }
    public int? crop_w { set; get; }
    public int? window_wc { set; get; }
    public int? window_ww { set; get; }
    public int? every_image { set; get; }
    public int? start_image { set; get; }
    public int? end_image { set; get; }
}
[PetaPoco.TableName("Studies")]
public class DbStudy
{
    public int Id { set; get; }
    public string case_id { set; get; }
    public string study_uid { set; get; }
    public string description { set; get; }
    public DateTime? date { set; get; }
    public string modality { set; get; }
    public string findings { set; get; }
    public string caption { set; get; }
    public int images { set; get; }
    public int position { set; get; }
    public string r_study_id { set; get; }
    public string patient_id { set; get; }
    public string patient_name { set; get; }
    public DateTime? patient_dob { set; get; }
    [PetaPoco.Ignore]
    public List<DbSeries> series { set; get; }
}
[PetaPoco.TableName("Users")]
public class User
{
    public int Id { set; get; }
    public string username { set; get; }    
    public string access_token { set; get; }
    public string refresh_token { set; get; }
    public DateTime? expiry_date { set; get; }
}