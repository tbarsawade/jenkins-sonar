Imports System.Collections.Generic
Imports System.Linq
Imports System.Web
Imports System.Web.Services
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Data.SqlClient
Imports System.Data
Imports System.Configuration
Imports System.Web.UI.Adapters.ControlAdapter
Imports System.Drawing
Imports System.Threading
Imports System
Imports System.Collections.Specialized
Imports System.Text
Imports System.Net.Security
Imports System.IO
Imports Newtonsoft.Json.Converters
Imports Newtonsoft.Json
Imports System.Web.Script.Serialization

Partial Class logbookreport
    Inherits System.Web.UI.Page
    Shared stdate As String = ""
    Shared eddate As String = ""
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        hdnUID.Value = Session("UID")
        hdnurole.Value = Session("ROLES")
        hdnEid.Value = Session("Eid")
    End Sub
    Public Function bindGrid() As DataSet
        Dim ds As New DataSet()
        Dim ConStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim d1 = Session("Sdate")
        Dim d2 = Session("edate")
        Using con As New SqlConnection(ConStr)
            Using da As New SqlDataAdapter("getIndusOverSpeed", con)
                da.SelectCommand.CommandType = CommandType.StoredProcedure
                da.SelectCommand.Parameters.AddWithValue("@uid", Session("UID"))
                da.SelectCommand.Parameters.AddWithValue("@rolename", Session("USERROLE"))
                da.SelectCommand.Parameters.AddWithValue("@d1", d1)
                da.SelectCommand.Parameters.AddWithValue("@d2", d2)
                da.SelectCommand.Parameters.AddWithValue("@eid", Session("EID"))
                da.SelectCommand.CommandTimeout = 120
                da.Fill(ds)
            End Using
        End Using
        Return ds
    End Function
    'Add Theme Code
    Protected Sub Page_PreInit1(ByVal sender As Object, ByVal e As EventArgs) Handles Me.PreInit
        Try
            If Not Session("CTheme") Is Nothing And Not Session("CTheme") = String.Empty Then
                Page.Theme = Convert.ToString(Session("CTheme"))
            Else
                Page.Theme = "Default"
            End If
        Catch ex As Exception
        End Try

    End Sub
    Protected Sub btnSearch_Click(sender As Object, e As EventArgs) Handles btnSearch.Click
        bindGrid()
    End Sub
    <WebMethod> _
    Public Shared Function GetJSON(d1 As String, d2 As String) As pichartCol
        Dim ret As New pichartCol()
        Dim ds As New DataSet()
        Dim Query As String = ""
        HttpContext.Current.Session("Sdate") = d1
        HttpContext.Current.Session("edate") = d2
        'If HttpContext.Current.Session("USERROLE").ToString.ToUpper = "SU" Or HttpContext.Current.Session("USERROLE").ToString.ToUpper = "CORPORATEUSER" Then
        '    If d1 = "" And d2 = "" Then
        '        Query = "select  dms.udf_split('MASTER-Vehicle Type-fld1',m.fld14)[Vehicle Type],(select count(m1.fld1) from mmm_mst_master m1 with (nolock) inner join mmm_mst_master m2 with (nolock) on m1.fld11=convert(nvarchar,m2.tid)   where m1.eid=54 and m1.fld14=m.fld14 and m1.fld11<>'')[Total Vehicle Count],(select count(m3.fld14) from mmm_mst_elogbook e with (nolock) inner join mmm_mst_master m3 with (nolock) on e.vehicle_no=m3.fld1 where sitevisiteid<>0 and m3.fld14=m.fld14 and Convert(date,trip_start_datetime)>=convert(date,getdate()-1) and Convert(date,trip_End_datetime)<=convert(date,getdate()-1) )[Total Site Visit Count],sum(convert(int,round(convert(numeric(10,4),isnull(e.total_distance,0)),0)))[Total Km.] from mmm_mst_elogbook e with (nolock) join mmm_mst_master m with (nolock) on m.fld1=e.vehicle_no  where m.eid=54 and e.eid=54 and Convert(date,trip_start_datetime)>=convert(date,getdate()-1) and Convert(date,trip_End_datetime)<=convert(date,getdate()-1)  group by m.fld14"
        '    Else
        '        Query = "select  dms.udf_split('MASTER-Vehicle Type-fld1',m.fld14)[Vehicle Type],(select count(m1.fld1) from mmm_mst_master m1 with (nolock) inner join mmm_mst_master m2 with (nolock) on m1.fld11=convert(nvarchar,m2.tid)   where m1.eid=54 and m1.fld14=m.fld14 and m1.fld11<>'')[Total Vehicle Count],(select count(m3.fld14) from mmm_mst_elogbook e with (nolock) inner join mmm_mst_master m3 with (nolock) on e.vehicle_no=m3.fld1 where sitevisiteid<>0 and m3.fld14=m.fld14 and Convert(date,trip_start_datetime)>='" & d1 & "' and Convert(date,trip_End_datetime)<='" & d2 & "' )[Total Site Visit Count],sum(convert(int,round(convert(numeric(10,4),isnull(e.total_distance,0)),0)))[Total Km.] from mmm_mst_elogbook e with (nolock) join mmm_mst_master m with (nolock) on m.fld1=e.vehicle_no  where m.eid=54 and e.eid=54 and Convert(date,trip_start_datetime)>='" & d1 & "' and Convert(date,trip_End_datetime)<='" & d2 & "'  group by m.fld14"
        '    End If
        'Else
        '    If d1 = "" And d2 = "" Then
        '        Query = "select  dms.udf_split('MASTER-Vehicle Type-fld1',m.fld14)[Vehicle Type],(select count(m1.fld1) from mmm_mst_master m1 with (nolock) inner join mmm_mst_master m2 with (nolock) on m1.fld11=convert(nvarchar,m2.tid)   where m1.eid=54 and m1.fld14=m.fld14 and m1.fld11<>'')[Total Vehicle Count],(select count(m3.fld14) from mmm_mst_elogbook e with (nolock) inner join mmm_mst_master m3 with (nolock) on e.vehicle_no=m3.fld1 where sitevisiteid<>0 and m3.fld14=m.fld14 and Convert(date,trip_start_datetime)>=convert(date,getdate()-1) and Convert(date,trip_End_datetime)<=convert(date,getdate()-1) )[Total Site Visit Count],sum(convert(int,round(convert(numeric(10,4),isnull(e.total_distance,0)),0)))[Total Km.] from mmm_mst_elogbook e with (nolock) join mmm_mst_master m with (nolock) on m.fld1=e.vehicle_no inner join  dbo.split((select fld1 from mmm_ref_role_user where uid=" & HttpContext.Current.Session("UID") & " and rolename='" & HttpContext.Current.Session("USERROLE") & "' and eid=" & HttpContext.Current.Session("EID") & "), ',') s on s.items in (select items from dbo.split(m.fld16, ','))  where m.eid=54 and e.eid=54 and Convert(date,trip_start_datetime)>=convert(date,getdate()-1) and Convert(date,trip_End_datetime)<=convert(date,getdate()-1)  group by m.fld14"
        '    Else
        '        Query = "select  dms.udf_split('MASTER-Vehicle Type-fld1',m.fld14)[Vehicle Type],(select count(m1.fld1) from mmm_mst_master m1 with (nolock) inner join mmm_mst_master m2 with (nolock) on m1.fld11=convert(nvarchar,m2.tid)   where m1.eid=54 and m1.fld14=m.fld14 and m1.fld11<>'')[Total Vehicle Count],(select count(m3.fld14) from mmm_mst_elogbook e with (nolock) inner join mmm_mst_master m3 with (nolock) on e.vehicle_no=m3.fld1 where sitevisiteid<>0 and m3.fld14=m.fld14 and Convert(date,trip_start_datetime)>='" & d1 & "' and Convert(date,trip_End_datetime)<='" & d2 & "' )[Total Site Visit Count],sum(convert(int,round(convert(numeric(10,4),isnull(e.total_distance,0)),0)))[Total Km.] from mmm_mst_elogbook e with (nolock) join mmm_mst_master m with (nolock) on m.fld1=e.vehicle_no inner join  dbo.split((select fld1 from mmm_ref_role_user where uid=" & HttpContext.Current.Session("UID") & " and rolename='" & HttpContext.Current.Session("USERROLE") & "' and eid=" & HttpContext.Current.Session("EID") & "), ',') s on s.items in (select items from dbo.split(m.fld16, ','))  where m.eid=54 and e.eid=54 and Convert(date,trip_start_datetime)>='" & d1 & "' and Convert(date,trip_End_datetime)<='" & d2 & "'  group by m.fld14"
        '    End If
        'End If
        'Dim ConStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        'Using con As New SqlConnection(ConStr)
        '    Using da As New SqlDataAdapter(Query, con)
        '        da.Fill(ds)
        '    End Using
        'End Using
        Try
            'If ds.Tables(0).Rows.Count > 0 Then
            '    'For Each column As DataColumn In ds.Tables(0).Columns
            '    'Next
            '    Dim obj As pichart
            '    Dim o1 As New List(Of pichart)()
            '    Dim o2 As New List(Of pichart)()
            '    Dim o3 As New List(Of pichart)()
            '    For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
            '        obj = New pichart()
            '        obj.category = ds.Tables(0).Rows(i).Item("Vehicle Type")
            '        obj.value = ds.Tables(0).Rows(i).Item("Total Vehicle Count")
            '        o1.Add(obj)
            '        obj = New pichart()
            '        obj.category = ds.Tables(0).Rows(i).Item("Vehicle Type")
            '        obj.value = ds.Tables(0).Rows(i).Item("Total Site Visit Count")
            '        o2.Add(obj)
            '        obj = New pichart()
            '        obj.category = ds.Tables(0).Rows(i).Item("Vehicle Type")
            '        obj.value = ds.Tables(0).Rows(i).Item("Total Km.")
            '        o3.Add(obj)
            '    Next
            'ret.data1 = o1
            'ret.data2 = o2
            'ret.data3 = o3
            ' End If
        Catch Ex As Exception
            Throw
        End Try
        Return ret
    End Function
    Public Shared Function GetOverSpeed(d1 As String, d2 As String, uid As Integer, urole As String) As DataSet
        Dim ds As New DataSet()
        Dim ConStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Using con As New SqlConnection(ConStr)
            Using da As New SqlDataAdapter("getIndusOverSpeed", con)
                da.SelectCommand.CommandType = CommandType.StoredProcedure
                da.SelectCommand.Parameters.AddWithValue("@uid", uid)
                da.SelectCommand.Parameters.AddWithValue("@rolename", urole)
                da.SelectCommand.Parameters.AddWithValue("@d1", d1)
                da.SelectCommand.Parameters.AddWithValue("@d2", d2)
                da.SelectCommand.Parameters.AddWithValue("@eid", HttpContext.Current.Session("EID"))
                da.SelectCommand.CommandTimeout = 120
                da.Fill(ds)
            End Using
        End Using
        Return ds
    End Function
    Public Overloads Overrides Sub VerifyRenderingInServerForm(ByVal control As Control)
    End Sub
    Protected Sub btnExcelExport_Click(sender As Object, e As System.Web.UI.ImageClickEventArgs) Handles btnExcelExport.Click
        Dim ds1 As New DataSet()
        'Dim d1 = txtd1.Text
        ds1 = bindGrid()
        Dim grdTripData As New GridView
        grdTripData.DataSource = ds1
        grdTripData.DataBind()
        grdTripData.ShowHeader = True
        grdTripData.RowHeaderColumn = True
        grdTripData.AllowPaging = False
        Response.Clear()
        Response.Buffer = True
        Response.Write("<br/><div align=""center"" style=""border:1px solid red"" ><h3>VEHICLE LOG BOOK (Electronic)</h3></div> <br/>")
        Response.AddHeader("content-disposition", "attachment;filename=Trip Report.xls")
        Response.Charset = ""
        Response.ContentType = "application/vnd.ms-excel"
        Dim sw As New StringWriter()
        Dim hw As New HtmlTextWriter(sw)
        For i As Integer = 0 To grdTripData.Rows.Count - 1
            'Apply text style to each Row 
            grdTripData.Rows(i).Attributes.Add("class", "textmode")
        Next
        grdTripData.RenderControl(hw)
        'style to format numbers to string 
        Dim style As String = "<style> .textmode{mso-number-format:\@;}</style>"
        Response.Write(style)
        Response.Output.Write(sw.ToString())
        Response.Flush()
        Response.End()
    End Sub

End Class
Public Class pichart
    Public category As String
    Public value As String
    'Public color As String
End Class
Public Class pichartCol
    Public str As String
End Class