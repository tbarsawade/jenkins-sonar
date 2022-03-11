<%@ WebHandler Language="C#" Class="JsonResponse" %>

using System;
using System.Web;
using System.Collections;
using System.Collections.Generic;
using System.Web.SessionState;

public class JsonResponse : IHttpHandler, IRequiresSessionState 
{


    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "application/json";
       

        DateTime start = new DateTime(1970, 1, 1);
        DateTime end = new DateTime(1970, 1, 1);
        string DocType = null;

        if (context.Request.QueryString["start"] != null)
        {
            start = start.AddSeconds(double.Parse(context.Request.QueryString["start"]));
        }
        if (context.Request.QueryString["end"] != null)
        {
            end = end.AddSeconds(double.Parse(context.Request.QueryString["end"]));
        }
        
        if (context.Request.QueryString["Doc"] != null)
        {
             DocType = context.Request.QueryString["Doc"].ToString();
        }
       
        String result = String.Empty;


        int option =Convert.ToInt32(context.Request.QueryString["Option"]);

        if (option == 0)
        {
            result += "[";

            List<int> idList = new List<int>();
            foreach (CalendarEvent cevent in EventDAO.getEvents(DocType, Convert.ToInt32(HttpContext.Current.Session["eid"]), Convert.ToInt32(HttpContext.Current.Session["uid"]),HttpContext.Current.Session["roles"].ToString(), 0))
            //foreach (CalendarEvent cevent in EventDAO.getEvents("VRF FIXED_POOL", 32, 1136, 0))
            {
                result += convertCalendarEventIntoString(cevent);
                idList.Add(cevent.id);
            }

            if (result.EndsWith(","))
            {
                result = result.Substring(0, result.Length - 1);
            }

            result += "]";
            //store list of event ids in Session, so that it can be accessed in web methods
            context.Session["idList"] = idList;

            context.Response.Write(result);
        }
        else if (option == 1)
        {
           
            var data = EventDAO.GetFormNames(Convert.ToInt32(HttpContext.Current.Session["eid"]));
            var oSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            //string sJSON = oSerializer.Serialize(data);
            //context.Response.Write(data);

            System.Web.Script.Serialization.JavaScriptSerializer jSearializer =
                   new System.Web.Script.Serialization.JavaScriptSerializer();
            context.Response.ContentType = "text/html";
            context.Response.Write(jSearializer.Serialize(data));
        }
        
        
    }

    private String convertCalendarEventIntoString(CalendarEvent cevent)
    {
        String allDay = "true";
        if (ConvertToTimestamp(cevent.start).ToString().Equals(ConvertToTimestamp(cevent.end).ToString()))
        {

            if (cevent.start.Hour == 0 && cevent.start.Minute == 0 && cevent.start.Second == 0)
            {
                allDay = "true";
            }
            else
            {
                allDay = "false";
            }
        }
        else
        {
            if (cevent.start.Hour == 0 && cevent.start.Minute == 0 && cevent.start.Second == 0
                && cevent.end.Hour == 0 && cevent.end.Minute == 0 && cevent.end.Second == 0)
            {
                allDay = "true";
            }
            else
            {
                allDay = "false";
            }
        }
        return    "{" +
                  "id: '" + cevent.id + "'," +
                  "title: '" + HttpContext.Current.Server.HtmlEncode(cevent.title) + "'," +
                  "start:  " + ConvertToTimestamp(cevent.start).ToString() + "," +
                  "end: " + ConvertToTimestamp(cevent.end).ToString() + "," +
                  "allDay:" + allDay + "," +
                  "description: '" + HttpContext.Current.Server.HtmlEncode(cevent.description) + "'" +
                  "},";
    }

    
    public bool IsReusable
    {
        get
        {
            return false;
        }
    }

    private long ConvertToTimestamp(DateTime value)
    {


        long epoch = (value.ToUniversalTime().Ticks - 621355968000000000) / 10000000;
        return epoch;

    }

}