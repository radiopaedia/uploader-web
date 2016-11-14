using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Net;
using Newtonsoft.Json;

/// <summary>
/// Class for OAuth handlers
/// /// </summary>
public static class AOA
{
    public static TokenResponse TradeAuthCodeForToken(string authCode)
    {
        string responseFromServer = null;
        try
        {
            var api = ACFG.GetSiteApiDetails();
            WebRequest request = WebRequest.Create(api.oauth_url + "token?client_id=" + api.site_id + "&client_secret=" + api.site_secret + "&code=" + authCode + "&grant_type=authorization_code&redirect_uri=" + api.redirect_url);
            request.Method = "POST";
            WebResponse response = request.GetResponse();
            var dataStream = response.GetResponseStream();
            System.IO.StreamReader reader = new System.IO.StreamReader(dataStream);
            responseFromServer = reader.ReadToEnd();
            reader.Close();
            dataStream.Close();
            response.Close();
        }
        catch (WebException ex)
        {
            using (var stream = ex.Response.GetResponseStream())
            using (var reader = new System.IO.StreamReader(stream))
            {
                string errorResponse = reader.ReadToEnd();
                System.IO.File.WriteAllText(@"c:\temp\templog.txt", errorResponse);
            }
            return null;
        }
        catch(Exception ex)
        {
            System.IO.File.WriteAllText(@"c:\temp\templog.txt", ex.Message);
            return null;
        }
        return JsonConvert.DeserializeObject<TokenResponse>(responseFromServer);   
    } 
    
    public static UserResponse GetUserName(string token)
    {
        try
        {            
            var api = ACFG.GetSiteApiDetails();
            WebRequest request = WebRequest.Create(api.users_url);
            request.Method = "GET";
            //request.ContentType = "application/json";
            request.Headers.Add("Authorization", "Bearer " + token);

            WebResponse response = request.GetResponse();
            var dataStream = response.GetResponseStream();
            System.IO.StreamReader reader = new System.IO.StreamReader(dataStream);
            string responseFromServer = reader.ReadToEnd();
            if (string.IsNullOrEmpty(responseFromServer))
                throw new Exception("Unable to get response from server when marking case complete");
            
            var respObj = JsonConvert.DeserializeObject<UserResponse>(responseFromServer);

            reader.Close();
            dataStream.Close();
            response.Close();
            return respObj;
        }
        catch (WebException ex)
        {
            using (var stream = ex.Response.GetResponseStream())
            using (var reader = new System.IO.StreamReader(stream))
            {
                string errorResponse = reader.ReadToEnd();
                System.IO.File.WriteAllText(@"c:\temp\templog.txt", errorResponse);
            }
            return null;
        }
        catch (Exception ex)
        {
            return null;
        }

    }
    
    

    public static string GetUsernameFromDb(string refresh)
    {
        try
        {
            APetaPoco.SetConnectionString("cn1");
            var bm = APetaPoco.PpGetScalar<string>("SELECT TOP 1 [username] FROM [Users] WHERE [refresh_token] = '" + refresh + "'");
            string username = null;
            if (bm.Success) { username = (string)bm.Data; }
            if (string.IsNullOrEmpty(username)) { throw new Exception("Unable to get user details"); }
            return username;
        }
        catch { return null; }
        
    }
}



public class TokenResponse
{
    public string access_token { set; get; }
    public string token_type { set; get; }
    public int expires_in { set; get; }
    public string refresh_token { set; get; }
    public string scope { set; get; }
    public long created_at { set; get; }
}

public class UserResponse
{
    public string login { set; get; }
    public Quotas quotas { set; get; }
}
public class Quotas
{
    public int? allowed_draft_cases { set; get; }
    public int? allowed_unlisted_cases { set; get; }
    public int? allowed_unlisted_playlists { set; get; }
    public int draft_case_count { set; get; }
    public int unlisted_case_count { set; get; }
    public int unlisted_playlist_count { set; get; }
}
//{"access_token":"38246d410cd801e09086c892112353b04f22416f432811ddee0d3e7d1d2212c8","token_type":"bearer","expires_in":86400,"refresh_token":"03be68594d360a8a8f728131a05de1f96669dade68c28b2e414326b848335ce0","scope":"cases","created_at":1458791436}