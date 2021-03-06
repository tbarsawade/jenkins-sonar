<%@ WebHandler Language="C#" Class="FileUploadHandler" %>

using System;
using System.Web;
using System.IO;

public class FileUploadHandler : IHttpHandler {

    public void ProcessRequest(HttpContext context)
    {
        string fileName = null;
        if (context.Request.Files.Count > 0)
        {
          
            HttpFileCollection files = context.Request.Files;
            try
            {
                for (int i = 0; i < files.Count; i++)
                {
                    HttpPostedFile file = files[i];
                    string fExtension = Path.GetExtension(file.FileName);
                    fileName = DateTime.Now.Day + DateTime.Now.Year + DateTime.Now.Ticks + "_" + fExtension;
                    fileName.Replace('/', '_');
                    fileName.Replace(':', '_');
                    string fname = context.Server.MapPath("~/DOCS/temp/" + fileName);
                    file.SaveAs(fname);
                }
                context.Response.ContentType = "text/plain";
                context.Response.Write(fileName);
            }
            catch (Exception e)
            {
                context.Response.ContentType = "text/plain";
                string str=e.Message.Replace('.','_');
                context.Response.Write("<span style='color:red;'>Upload Error: '" + str + "' <span/>");
            }
            
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