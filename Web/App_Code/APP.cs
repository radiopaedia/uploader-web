using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Web.Configuration;
using System.Configuration;

using System.Data.SqlClient;

using PetaPoco;

/// <summary>
/// Andy's PetaPoco Custom Helper Classes
/// (c) 2016 Andy Le - The Royal Melbourne Hospital
/// </summary>
public static class APetaPoco
{
    public static BoolMessage _checkVal = new BoolMessage();
    public static string _cstring = null;
    public static string _cskey = null;
    public static void SetConnectionString(string cskey)
    {
        _cstring = null;
        _cskey = null;
        if (!string.IsNullOrEmpty(cskey))
        {
            _cskey = cskey;
        }

        if (ConfigurationManager.ConnectionStrings[_cskey] == null)
        {
            DecryptConnectionStrings();
            _cstring = ConfigurationManager.ConnectionStrings[_cskey].ToString();
            EncryptConnectionStrings();
        }
        else
        {
            _cstring = ConfigurationManager.ConnectionStrings[_cskey].ToString();
        }
    }
    public static void EncryptConnectionStrings()
    {
        Configuration config = null;
        if (HttpRuntime.AppDomainAppId != null)
        {
            config = WebConfigurationManager.OpenWebConfiguration("~");
        }
        else
        {
            config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
        }
        ConnectionStringsSection connSection = (ConnectionStringsSection)config.GetSection("connectionStrings");
        if (!connSection.SectionInformation.IsProtected)
        {
            connSection.SectionInformation.ProtectSection("DataProtectionConfigurationProvider");
            //config.Save();
        }
    }
    public static void DecryptConnectionStrings()
    {
        Configuration config = null;
        if (HttpRuntime.AppDomainAppId != null)
        {
            config = WebConfigurationManager.OpenWebConfiguration("~");
        }
        else
        {
            config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
        }
        ConnectionStringsSection connSection = (ConnectionStringsSection)config.GetSection("connectionStrings");
        if (connSection.SectionInformation.IsProtected)
        {
            connSection.SectionInformation.UnprotectSection();
            //config.Save();
        }
    }

    public static void DeleteSql(string _tableName, string _condition)
    {
        try
        {
            if (string.IsNullOrEmpty(_cstring))
                throw new Exception("Connection String not set");
            if (string.IsNullOrEmpty(_condition))
                throw new Exception("Condition String not set");
            string query = string.Format("DELETE FROM [{0}] WHERE {1}", _tableName.ToUpper(), _condition);
            using (SqlConnection conn = new SqlConnection(_cstring))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }
        }
        catch (Exception ex)
        {
            return;
        }
    }
    public static BoolMessage PpDelete(object o)
    {
        _checkVal.Data = null;
        try
        {
            if (string.IsNullOrEmpty(_cskey))
                throw new Exception("No connection string provided");
            using (var pp = new Database(_cskey))
            {
                int rows = pp.Delete(o);
                _checkVal.Success = true;
                _checkVal.Message = string.Format("Successfully deleted {0} rows", rows);
            }
        }
        catch (Exception ex)
        {
            _checkVal.Success = false;
            _checkVal.Message = ex.Message;
        }
        return _checkVal;
    }

    public static BoolMessage PpUpdate(object o)
    {
        _checkVal.Data = null;
        try
        {
            if (string.IsNullOrEmpty(_cskey))
                throw new Exception("No connection string provided");
            using (var pp = new Database(_cskey))
            {
                int rows = pp.Update(o);
                _checkVal.Success = true;
                _checkVal.Message = string.Format("Successfully updated {0} rows", rows);
            }
        }
        catch (Exception ex)
        {
            _checkVal.Success = false;
            _checkVal.Message = ex.Message;
        }
        return _checkVal;
    }
    public static BoolMessage PpRetrieveList<T>(string tableName, string condition = null)
    {
        _checkVal.Data = null;
        try
        {
            if (string.IsNullOrEmpty(_cskey))
                throw new Exception("No connection string provided");
            List<T> rtnList = null;

            using (var pp = new Database(_cskey))
            {
                if (string.IsNullOrEmpty(condition))
                {
                    rtnList = pp.Query<T>(string.Format("SELECT * FROM [{0}]", tableName)).ToList();
                }
                else
                {
                    rtnList = pp.Query<T>(string.Format("SELECT * FROM [{0}] WHERE {1}", tableName, condition)).ToList();
                }
            }
            _checkVal.Data = rtnList;
            _checkVal.Success = true;
            _checkVal.Message = string.Format("Successfully returned list from table {0}", tableName);
        }
        catch (Exception ex)
        {
            _checkVal.Data = null;
            _checkVal.Success = false;
            _checkVal.Message = ex.Message;
        }
        return _checkVal;
    }
    public static BoolMessage PpRetrieveOne<T>(string tableName, string condition)
    {
        _checkVal.Data = null;
        try
        {
            if (string.IsNullOrEmpty(_cskey))
                throw new Exception("No connection string provided");

            using (var pp = new Database(_cskey))
            {
                _checkVal.Data = pp.SingleOrDefault<T>(string.Format("SELECT * FROM [{0}] WHERE {1}", tableName, condition));
            }
            _checkVal.Success = true;
            _checkVal.Message = string.Format("Successfully returned one item from table {0}", tableName);
        }
        catch (Exception ex)
        {
            _checkVal.Data = null;
            _checkVal.Success = false;
            _checkVal.Message = ex.Message;
        }
        return _checkVal;
    }
    public static BoolMessage PpRetrieveCustomQuery<T>(string query)
    {
        _checkVal.Data = null;
        try
        {
            if (string.IsNullOrEmpty(_cskey))
                throw new Exception("Connection String not set");
            if (string.IsNullOrEmpty(query))
                throw new Exception("No query string provided");

            using (var pp = new Database(_cskey))
            {
                _checkVal.Data = pp.Query<T>(query).ToList();
            }
            _checkVal.Success = true;
            _checkVal.Message = "Successfully retrieved list based on custom query";
        }
        catch (Exception ex)
        {
            _checkVal.Data = null;
            _checkVal.Success = false;
            _checkVal.Message = ex.Message;
        }
        return _checkVal;
    }
    public static BoolMessage PpInsert(object obj)
    {
        _checkVal.Data = null;
        try
        {
            if (string.IsNullOrEmpty(_cskey))
                throw new Exception("No connection string provided");
            using (var pp = new Database(_cskey))
            {
                pp.Insert(obj);
            }
            _checkVal.Success = true;
            _checkVal.Message = string.Format("Successfully inserted {0}", obj.ToString());
        }
        catch (Exception ex)
        {
            _checkVal.Success = false;
            _checkVal.Message = ex.Message;
        }
        return _checkVal;
    }

    public static BoolMessage PpGetScalar<T>(string query)
    {
        _checkVal.Data = null;
        try
        {
            if (string.IsNullOrEmpty(_cskey))
                throw new Exception("No connection string provided");
            using (var pp = new Database(_cskey))
            {
                _checkVal.Data = pp.ExecuteScalar<T>(query);
            }
            _checkVal.Success = true;
            _checkVal.Message = string.Format("Successfully retrieved scalar");
        }
        catch (Exception ex)
        {
            _checkVal.Data = null;
            _checkVal.Success = false;
            _checkVal.Message = ex.Message;
        }
        return _checkVal;
    }
}