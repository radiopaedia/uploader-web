using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class _Default : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            string refresh = Request["refresh"];                                  
            string code = Request["code"];

            if (!string.IsNullOrEmpty(code))
            {
                hid_auth.Value = "INITIAL2";
                hid_token.Value = code;
                return;
            }

            if (!string.IsNullOrEmpty(refresh))
            {
                APetaPoco.SetConnectionString("cn1");
                var bm = APetaPoco.PpRetrieveOne<User>("Users", "[refresh_token] = '" + refresh + "'");
                if (!bm.Success || bm.Data == null || bm.Data == default(User))
                {
                    //unable to get user details based on refresh token
                    ScriptManager.RegisterStartupScript(this, GetType(), "AnyValue", "notify('warning','Unable to get user details from refresh code\nTry authenticating again');", true);
                    hid_auth.Value = "FALSE";
                    return;
                }
                else
                {
                    var user = (User)bm.Data;
                    hid_username.Value = user.username;
                    hid_refresh.Value = user.refresh_token;
                    hid_auth.Value = "TRUE";
                    return;
                }
            }

            hid_auth.Value = "INITIAL";
            return;
        }        
    }

    protected void btn_auth_Click(object sender, EventArgs e)
    {
        var api = ACFG.GetSiteApiDetails();
        if(api != null)
        {
            Response.Redirect(api.oauth_url + "authorize?client_id="+api.site_id+"&redirect_uri="+api.redirect_url+"&response_type=code", false);
        }    
    }


    protected void btn_checkUser_Click(object sender, EventArgs e)
    {
        APetaPoco.SetConnectionString("cn1");
        string username = txt_username.Text;
        if (string.IsNullOrEmpty(username))
        {            
            ScriptManager.RegisterStartupScript(this, GetType(), "AnyValue", "notify('warning','No username provided');", true);
            return;
        }
        var bm = APetaPoco.PpRetrieveOne<User>("Users", "[username] = '" + txt_username.Text + "'");
        if (bm.Success)
        {
            var user = (User)bm.Data;
            if(user == null || user == default(User))
            {
                hid_auth.Value = "FALSE";
                return;
            }
            else
            {
                hid_auth.Value = "TRUE";
                hid_refresh.Value = user.refresh_token;
                hid_username.Value = user.username;
            }
        }
        else
        {
            hid_auth.Value = "FALSE";
        }
    }
    protected void btn_bindUser_Click(object sender, EventArgs e)
    {
        string username = txt_username2.Text;
        if (string.IsNullOrEmpty(username))
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "AnyValue", "notify('warning','No username provided');", true);
            return;
        }
        if (string.IsNullOrEmpty(hid_token.Value))
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "AnyValue", "notify('warning','No auth token received from Radiopaedia');", true);
            return;
        }
        var tr = AOA.TradeAuthCodeForToken(hid_token.Value);
        if(tr == null || tr == default(TokenResponse))
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "AnyValue", "notify('warning','No auth token received from Radiopaedia');", true);
            return;
        }

        var bm = APetaPoco.PpRetrieveOne<User>("Users", "[username] = '" + username + "'");
        if (bm.Success)
        {
            if(bm.Data != null && bm.Data != default(User))
            {
                var dbUser = (User)bm.Data;
                if(dbUser.username == username)
                {
                    dbUser.access_token = tr.access_token;
                    dbUser.refresh_token = tr.refresh_token;
                    dbUser.username = username;
                    dbUser.expiry_date = DateTime.Now.AddSeconds(tr.expires_in - (30 * 60));
                    bm = APetaPoco.PpUpdate(dbUser);
                    if (!bm.Success)
                    {
                        ScriptManager.RegisterStartupScript(this, GetType(), "AnyValue", "notify('error','Unable to update user in database');", true);
                        return;
                    }
                    hid_username.Value = username;
                    hid_refresh.Value = tr.refresh_token;
                    hid_auth.Value = "TRUE";
                    return;
                }
            }
        }

        var user = new User();
        user.username = username;
        user.access_token = tr.access_token;
        user.expiry_date = DateTime.Now.AddSeconds(tr.expires_in - (30 * 60));
        user.refresh_token = tr.refresh_token;

        APetaPoco.SetConnectionString("cn1");
        bm = APetaPoco.PpInsert(user);
        if (!bm.Success)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "AnyValue", "notify('error','Unable to insert user into database');", true);
            return;
        }
        hid_username.Value = username;
        hid_refresh.Value = tr.refresh_token;
        hid_auth.Value = "TRUE";
    }
}