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
Imports System.Web.Script.Services
Imports iTextSharp.text.pdf

Partial Class TicketStatus
    Inherits System.Web.UI.Page

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


    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load

    End Sub
    <WebMethod()> _
    Public Shared Function GetData(sdate As String, tdate As String) As DGrid
        Dim jsonData As String = ""
        Dim grid As New DGrid()
        Try

            Dim ds As New DataSet()
           
            Dim Query As String = ""
            If HttpContext.Current.Session("USERROLE") = "AGENT" Then
                Query = "select d.tid[TID],d.fld2[Requestor Email],d.fld3[Subject],d.tid[Ticket No.],(select username from mmm_mst_user with(nolock) where uid=d.fld7)[Assignee],dms.udf_split('MASTER-Organizations-fld1',d.fld16)[Organization],dms.udf_split('MASTER-Department-fld1',d.fld8)[Category],d.fld9[Ticket Status],"
                Query &= "convert(varchar(17),d.adate,113)[Creation Date],convert(varchar(17),d.lastupdate,113)[Last Updated On], "
                Query &= "case when isnull(d.fld19,48)=48 then 48 when isnull(d.fld19,0)=' ' then 48 else d.fld19 end as [TAT (H)],"
                Query &= "(DATEDIFF(day, dtl.fdate, getdate()))*8 as [Actual TAT (H)],"
                Query &= "cast((cast(((DATEDIFF(day, dtl.fdate, GETDATE()))*8) as numeric(18,2))-cast((case when isnull(d.fld19,48)=48 then 48 when isnull(d.fld19,0)=' ' then 48 else d.fld19 end) as numeric(18,2)))/cast((case when isnull(d.fld19,48)=48 then 48 when isnull(d.fld19,0)=' ' then 48 else d.fld19 end) as numeric(18,2))as numeric(18,2))*100 as [Adherence]  "
                Query &= " from mmm_mst_doc d inner join mmm_doc_dtl as dtl on d.tid=dtl.docid where d.eid='" & HttpContext.Current.Session("EID") & "' and convert(date,d.adate)>='" & sdate & "' and  convert(date,d.adate)<='" & tdate & "' and documenttype='Ticket' and aprstatus is null and dtl.userid=" & HttpContext.Current.Session("UID") & "  order by d.adate "
            Else
                Query = "select d.tid[TID],d.fld2[Requestor Email],d.fld3[Subject],d.tid[Ticket No.],(select username from mmm_mst_user with(nolock) where uid=d.fld7)[Assignee],dms.udf_split('MASTER-Organizations-fld1',d.fld16)[Organization],dms.udf_split('MASTER-Department-fld1',d.fld8)[Category],d.fld9[Ticket Status],"
                Query &= "convert(varchar(17),d.adate,113)[Creation Date],convert(varchar(17),d.lastupdate,113)[Last Updated On],   "
                Query &= "case when isnull(d.fld19,48)=48 then 48 when isnull(d.fld19,0)=' ' then 48 else d.fld19 end as [TAT (H)], "
                Query &= "(DATEDIFF(day, dtl.fdate, getdate()))*8 as [Actual TAT (H)],"
                Query &= "cast((cast(((DATEDIFF(day, dtl.fdate, GETDATE()))*8) as numeric(18,2))-cast((case when isnull(d.fld19,48)=48 then 48 when isnull(d.fld19,0)=' ' then 48 else d.fld19 end) as numeric(18,2)))/cast((case when isnull(d.fld19,48)=48 then 48 when isnull(d.fld19,0)=' ' then 48 else d.fld19 end) as numeric(18,2))as numeric(18,2))*100 as [Adherence]  "
                Query &= " from mmm_mst_doc d inner join mmm_doc_dtl as dtl on d.tid=dtl.docid  where   d.eid='" & HttpContext.Current.Session("EID") & "' and convert(date,d.adate)>='" & sdate & "' and  convert(date,d.adate)<='" & tdate & "' and documenttype='Ticket' and aprstatus is null    order by d.adate "
            End If
          

            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Using con As New SqlConnection(conStr)
                Using da As New SqlDataAdapter(Query, con)
                    da.Fill(ds)
                End Using
            End Using

            Dim strError = ""
            grid = DynamicGrid.GridData(ds.Tables(0), strError)
            If ds.Tables(0).Rows.Count = 0 Then
                grid.Message = "No data found...!"
                grid.Success = False
            End If
        Catch Ex As Exception
            grid.Success = False
            grid.Message = "No data found...!"
            grid.Count = 0
        End Try
        Return grid

    End Function

End Class
