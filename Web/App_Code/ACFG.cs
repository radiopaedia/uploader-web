using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Configuration;

/// <summary>
/// Config Code Behind
/// </summary>
public static class ACFG
{
    public static CFG GetConfigData()
    {
        CFG cfg = new CFG();
        cfg.ConnectionString = ConfigurationManager.ConnectionStrings["cn1"].ToString();
        if (!string.IsNullOrEmpty(cfg.ConnectionString))
        {
            APetaPoco.SetConnectionString("cn1");
            cfg.SiteApi = GetSiteApiDetails();
            var bm = APetaPoco.PpRetrieveList<Node>("PACS");
            if (bm.Success)
            {
                cfg.Nodes = (List<Node>)bm.Data;
            }            
        }
        return cfg;
    }
    public static ApiClient GetSiteApiDetails()
    {
        APetaPoco.SetConnectionString("cn1");
        var bm = APetaPoco.PpRetrieveCustomQuery<ApiClient>("SELECT TOP 1 * FROM [ApiClient]");
        if (bm.Success)
        {
            var list = (List<ApiClient>)bm.Data;
            if(list != null && list.Count > 0) { return list[0]; }
            else { return null; }
        }
        else
        {
            return null;
        }
    }
    public static BoolMessage CheckIfOnlySelected(Node node)
    {
        var rbm = new BoolMessage();
        rbm.Data = null;
        try
        {            
            APetaPoco.SetConnectionString("cn1");
            var bm = APetaPoco.PpRetrieveOne<Node>("PACS", "[Selected] = 1");
            Node currentSelectedNode = null;
            if (bm.Success)
                currentSelectedNode = (Node)bm.Data;
            else
                return bm;            
            if(currentSelectedNode == null || currentSelectedNode == default(Node))
            {
                if (node.Selected)
                {
                    rbm.Success = true;
                }
                else
                {
                    throw new Exception("There's no selected node in the database");
                }
            }
            else
            {
                if (node.Selected)
                {
                    if (node.Id == currentSelectedNode.Id)
                    {
                        rbm.Success = true;
                    }
                    else
                    {
                        currentSelectedNode.Selected = false;
                        bm = APetaPoco.PpUpdate(currentSelectedNode);
                        if (bm.Success)
                        {
                            rbm.Success = true;
                        }
                        else
                        {
                            return bm;
                        }
                    }
                }
                else
                {
                    if (currentSelectedNode.Id != node.Id)
                    {
                        rbm.Success = true;
                    }
                    else
                    {
                        throw new Exception("Can't unselect the only selected node in database");
                    }
                }
            }                        
        }
        catch(Exception ex)
        {
            rbm.Success = false;
            rbm.Message = ex.Message;
        }
        return rbm;
    }
    public static BoolMessage ValidateNode(Node node)
    {
        var bm = new BoolMessage();
        bm.Data = null;
        try
        {
            System.Net.IPAddress address;
            if (!System.Net.IPAddress.TryParse(node.IP, out address))
            {
                throw new Exception("Invalid IP Address");
            }

            if (string.IsNullOrEmpty(node.AET)) { throw new Exception("Invalid AET"); }
            if (string.IsNullOrEmpty(node.Description)) { throw new Exception("Invalid Description"); }
            if(node.Port > 65536 || node.Port < 1) { throw new Exception("Invalid Port"); }

            if (!ADCM.EchoNode(node))
            {
                throw new Exception("Unable to echo node");
            }

            bm.Success = true;
        }
        catch (Exception ex)
        {
            bm.Message = ex.Message;
            bm.Success = false;
        }        
        return bm;
    }
}
public class CFG
{
    public string ConnectionString { set; get; }
    public List<Node> Nodes { set; get; }
    public ApiClient SiteApi { set; get; }
}

[PetaPoco.TableName("PACS")]
public class Node
{
    public int Id { set; get; }
    public string LocalAe { set; get; }
    public string IP { set; get; }
    public int Port { set; get; }
    public string AET { set; get; }
    public string Description { set; get; }
    public bool Selected { set; get; }
    public string Notes { set; get; }
    public string LocalStorage { set; get; }
}

public class ApiClient
{
    public int Id { set; get; }
    public string site_id { set; get; }
    public string site_secret { set; get; }
    public string redirect_url { set; get; }
    public string oauth_url { set; get; }
    public string cases_url { set; get; }
    public string users_url { set; get; }
}