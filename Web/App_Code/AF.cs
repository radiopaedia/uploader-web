using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Text;
using System.Text.RegularExpressions;
using System.Security.Cryptography;

using Newtonsoft.Json;

using System.Collections.Generic;
using System.Reflection;
/// <summary>
/// Custom Helper Functions
/// </summary>
public static class AF
{
    public static T GetObjectFromJSON<T>(HttpContext context)
    {

        var jsonString = String.Empty;
        context.Request.InputStream.Position = 0;
        using (var inputStream = new System.IO.StreamReader(context.Request.InputStream))
        {
            jsonString = inputStream.ReadToEnd();
        }
        JsonSerializerSettings settings = new JsonSerializerSettings();
        settings.DateTimeZoneHandling = DateTimeZoneHandling.RoundtripKind;
        if (String.IsNullOrEmpty(jsonString))
            return default(T);
        else
            return JsonConvert.DeserializeObject<T>(jsonString, settings);

    }

    public static string GetStringFromIncomingRequest(HttpContext context)
    {
        string rStr = null;
        context.Request.InputStream.Position = 0;
        using (var inputStream = new System.IO.StreamReader(context.Request.InputStream))
        {
            rStr = inputStream.ReadToEnd();
        }
        return rStr;
    }

    public static string GetUniqueKey(int maxSize)
    {
        char[] chars = new char[62];
        chars =
        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();
        byte[] data = new byte[1];
        using (System.Security.Cryptography.RNGCryptoServiceProvider crypto = new System.Security.Cryptography.RNGCryptoServiceProvider())
        {
            crypto.GetNonZeroBytes(data);
            data = new byte[maxSize];
            crypto.GetNonZeroBytes(data);
        }
        StringBuilder result = new StringBuilder(maxSize);
        foreach (byte b in data)
        {
            result.Append(chars[b % (chars.Length)]);
        }
        return result.ToString();
    }

    public static List<Select2<T>> CreateSelect2List<T>(List<T> objList, string idProperty, string textProperty)
    {
        List<Select2<T>> _rtnList = new List<Select2<T>>();
        foreach (T obj in objList)
        {
            Select2<T> _newSelect2 = new Select2<T>();
            _newSelect2.id = Convert.ToInt32(obj.GetType().GetProperty(idProperty).GetValue(obj, null));
            _newSelect2.text = obj.GetType().GetProperty(textProperty).GetValue(obj, null).ToString();
            _newSelect2.Object = obj;
            _rtnList.Add(_newSelect2);
        }
        return _rtnList;
    }
    public static List<Select2<string>> CreateSelect2StringList(List<string> strList)
    {
        List<Select2<string>> _rtnList = new List<Select2<string>>();
        if (strList != null && strList.Count > 0)
        {
            for (int i = 0; i < strList.Count; i++)
            {
                Select2<string> sel2 = new Select2<string>();
                sel2.id = i;
                sel2.Object = null;
                sel2.text = strList[i];
                _rtnList.Add(sel2);
            }
        }
        return _rtnList;
    }

    public static void StandardJSON(bool Success, string Message, HttpContext Context, object Object = null)
    {
        ResponseObject response = new ResponseObject();
        response.Data = Object;
        response.Message = Message;
        response.Success = Success;
        string jsonString = JsonConvert.SerializeObject(response, new JsonSerializerSettings { DateTimeZoneHandling = DateTimeZoneHandling.Local });
        Context.Response.ContentType = "application/json";
        Context.Response.ContentEncoding = System.Text.Encoding.UTF8;
        Context.Response.Write(jsonString);
        Context.Response.End();
    }
    public static void ExceptionRespond(HttpContext Context, Exception ex)
    {
        BoolMessage bm = new BoolMessage();
        bm.Success = false;
        bm.Data = ex;
        bm.Message = ex.Message;
        BoolMessageRespond(Context, bm);
    }
    public static void BoolMessageRespond(HttpContext Context, BoolMessage BoolMsg)
    {
        ResponseObject response = new ResponseObject();
        response.Data = BoolMsg.Data;
        response.Success = BoolMsg.Success;
        if (string.IsNullOrEmpty(BoolMsg.Message) && BoolMsg.Messages.Count > 0)
            response.Message = string.Join(Environment.NewLine, BoolMsg.Messages);
        else
            response.Message = BoolMsg.Message;
        string jsonString = JsonConvert.SerializeObject(response, new JsonSerializerSettings { DateTimeZoneHandling = DateTimeZoneHandling.Local });
        Context.Response.ContentType = "application/json";
        Context.Response.ContentEncoding = System.Text.Encoding.UTF8;
        Context.Response.Write(jsonString);
        Context.Response.End();
    }

    public static void ErrorRespond(HttpContext context, string description, int code)
    {
        context.Response.TrySkipIisCustomErrors = true;
        context.Response.StatusDescription = description;
        context.Response.StatusCode = code;
        context.Response.End();
    }

    public static string SyntaxHighlightJson(string original)
    {
        return Regex.Replace(
          original,
          @"(¤(\\u[a-zA-Z0-9]{4}|\\[^u]|[^\\¤])*¤(\s*:)?|\b(true|false|null)\b|-?\d+(?:\.\d*)?(?:[eE][+\-]?\d+)?)".Replace('¤', '"'),
          match => {
              var cls = "color:green";
              if (Regex.IsMatch(match.Value, @"^¤".Replace('¤', '"')))
              {
                  if (Regex.IsMatch(match.Value, ":$"))
                  {
                      // cls = "key";
                      cls = "color:blue";
                  }
                  else
                  {
                      //cls = "string";
                      cls = "color:brown";
                  }
              }
              else if (Regex.IsMatch(match.Value, "true|false"))
              {
                  //cls = "boolean";
                  cls = "color:red";
              }
              else if (Regex.IsMatch(match.Value, "null"))
              {
                  //cls = "null";
                  cls = "color:dimgray";
              }
              return "<span style=\"" + cls + "\">" + match + "</span>";
          });
    }
    public static T XmlDeserializeFromHttpContext<T>(HttpContext context)
    {
        try
        {
            context.Request.InputStream.Position = 0;
            string xmlString = null;
            using (var inputStream = new System.IO.StreamReader(context.Request.InputStream))
            {
                xmlString = inputStream.ReadToEnd();
            }
            if (string.IsNullOrEmpty(xmlString))
                throw new Exception("No input xml string");
            return xmlString.XmlDeserializeFromString<T>();
        }
        catch (Exception ex)
        {
            return default(T);
        }
    }

    public static string XmlSerializeToString(this object objectInstance)
    {
        var serializer = new System.Xml.Serialization.XmlSerializer(objectInstance.GetType());
        var sb = new System.Text.StringBuilder();

        using (System.IO.TextWriter writer = new System.IO.StringWriter(sb))
        {
            serializer.Serialize(writer, objectInstance);
        }

        return sb.ToString();
    }

    public static T XmlDeserializeFromString<T>(this string objectData)
    {
        return (T)XmlDeserializeFromString(objectData, typeof(T));
    }

    public static object XmlDeserializeFromString(this string objectData, Type type)
    {
        var serializer = new System.Xml.Serialization.XmlSerializer(type);
        object result;

        using (System.IO.TextReader reader = new System.IO.StringReader(objectData))
        {
            result = serializer.Deserialize(reader);
        }

        return result;
    }

    private class ResponseObject
    {
        public bool Success { set; get; }
        public string Message { set; get; }
        public object Data { set; get; }
    }
}

public class Select2<T>
{
    public int id { set; get; }
    public string text { set; get; }
    public T Object { set; get; }
}

public class BoolMessage
{
    public bool Success { set; get; }
    public string Message { set; get; }
    public List<string> Messages { set; get; }
    public object Data { set; get; }
    public BoolMessage()
    {
        Success = false;
        Message = string.Empty;
        Messages = new List<string>();
        Data = null;
    }
}

/* 
 * Password Hashing With PBKDF2 (http://crackstation.net/hashing-security.htm).
 * Copyright (c) 2013, Taylor Hornby
 * All rights reserved.
 *
 * Redistribution and use in source and binary forms, with or without 
 * modification, are permitted provided that the following conditions are met:
 *
 * 1. Redistributions of source code must retain the above copyright notice, 
 * this list of conditions and the following disclaimer.
 *
 * 2. Redistributions in binary form must reproduce the above copyright notice,
 * this list of conditions and the following disclaimer in the documentation 
 * and/or other materials provided with the distribution.
 *
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
 * AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE 
 * IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE 
 * ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE 
 * LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR 
 * CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF 
 * SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS 
 * INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN 
 * CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) 
 * ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE 
 * POSSIBILITY OF SUCH DAMAGE.
 */

/// <summary>
/// Salted password hashing with PBKDF2-SHA1.
/// Author: havoc AT defuse.ca
/// www: http://crackstation.net/hashing-security.htm
/// Compatibility: .NET 3.0 and later.
/// </summary>
public class PasswordHash
{
    // The following constants may be changed without breaking existing hashes.
    public const int SALT_BYTE_SIZE = 24;
    public const int HASH_BYTE_SIZE = 24;
    public const int PBKDF2_ITERATIONS = 1000;

    public const int ITERATION_INDEX = 0;
    public const int SALT_INDEX = 1;
    public const int PBKDF2_INDEX = 2;

    /// <summary>
    /// Creates a salted PBKDF2 hash of the password.
    /// </summary>
    /// <param name="password">The password to hash.</param>
    /// <returns>The hash of the password.</returns>
    public static string CreateHash(string password)
    {
        // Generate a random salt
        RNGCryptoServiceProvider csprng = new RNGCryptoServiceProvider();
        byte[] salt = new byte[SALT_BYTE_SIZE];
        csprng.GetBytes(salt);

        // Hash the password and encode the parameters
        byte[] hash = PBKDF2(password, salt, PBKDF2_ITERATIONS, HASH_BYTE_SIZE);
        return PBKDF2_ITERATIONS + ":" +
            Convert.ToBase64String(salt) + ":" +
            Convert.ToBase64String(hash);
    }

    /// <summary>
    /// Validates a password given a hash of the correct one.
    /// </summary>
    /// <param name="password">The password to check.</param>
    /// <param name="correctHash">A hash of the correct password.</param>
    /// <returns>True if the password is correct. False otherwise.</returns>
    public static bool ValidatePassword(string password, string correctHash)
    {
        // Extract the parameters from the hash
        try
        {
            char[] delimiter = { ':' };
            string[] split = correctHash.Split(delimiter);
            int iterations = Int32.Parse(split[ITERATION_INDEX]);
            byte[] salt = Convert.FromBase64String(split[SALT_INDEX]);
            byte[] hash = Convert.FromBase64String(split[PBKDF2_INDEX]);

            byte[] testHash = PBKDF2(password, salt, iterations, hash.Length);
            return SlowEquals(hash, testHash);
        }
        catch
        {
            return false;
        }

    }

    /// <summary>
    /// Compares two byte arrays in length-constant time. This comparison
    /// method is used so that password hashes cannot be extracted from
    /// on-line systems using a timing attack and then attacked off-line.
    /// </summary>
    /// <param name="a">The first byte array.</param>
    /// <param name="b">The second byte array.</param>
    /// <returns>True if both byte arrays are equal. False otherwise.</returns>
    private static bool SlowEquals(byte[] a, byte[] b)
    {
        uint diff = (uint)a.Length ^ (uint)b.Length;
        for (int i = 0; i < a.Length && i < b.Length; i++)
            diff |= (uint)(a[i] ^ b[i]);
        return diff == 0;
    }

    /// <summary>
    /// Computes the PBKDF2-SHA1 hash of a password.
    /// </summary>
    /// <param name="password">The password to hash.</param>
    /// <param name="salt">The salt.</param>
    /// <param name="iterations">The PBKDF2 iteration count.</param>
    /// <param name="outputBytes">The length of the hash to generate, in bytes.</param>
    /// <returns>A hash of the password.</returns>
    private static byte[] PBKDF2(string password, byte[] salt, int iterations, int outputBytes)
    {
        Rfc2898DeriveBytes pbkdf2 = new Rfc2898DeriveBytes(password, salt);
        pbkdf2.IterationCount = iterations;
        return pbkdf2.GetBytes(outputBytes);
    }
}
