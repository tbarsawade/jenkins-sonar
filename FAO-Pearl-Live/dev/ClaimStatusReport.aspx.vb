Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports iTextSharp.text.pdf
Imports System.Security.Policy
Imports System.Net.Security
Imports System.Net.HttpWebRequest
Imports System.Net.HttpWebResponse
Imports System.Security.Cryptography.X509Certificates
Imports System.Web.Services
Imports Newtonsoft
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Converters
Imports System.Web.Script.Serialization
Partial Class ClaimStatusReport
    Inherits System.Web.UI.Page
    Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString

    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load

    End Sub
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

    <WebMethod()> _
    Public Shared Function GetJSON(sdate As String, tdate As String) As String
        Dim jsonData As String = ""
        Try
            Dim ds As New DataSet()
            Dim UID As Integer = 0
            Dim URole As String = ""
            Try
                UID = Convert.ToInt32(HttpContext.Current.Session("UID").ToString())
                URole = HttpContext.Current.Session("USERROLE").ToString()
                If (UID = 0 Or URole = "") Then
                    Return "NoSession"
                End If
            Catch ex As Exception
                Return "NoSession"
            End Try
            Dim qry As String = ""
            qry &= "select 'Approved'[Status],count(d.tid)[Count] from mmm_mst_doc d with(nolock) inner join mmm_doc_dtl dd with(nolock) on d.tid=dd.docid where d.curstatus='ARCHIVE' "
            qry &= "and d.documenttype='TRAVEL and ENTERTAINMENT REIMBURSEMENT' and d.eid='" & HttpContext.Current.Session("EID") & "' and dd.aprstatus='Finance Approval' and convert(date,d.adate)>='" & sdate & "' and  convert(date,d.adate)<='" & tdate & "' and  convert(nvarchar,d.fld35) in (select * from  InputString((select top 1 fld1 from mmm_ref_role_user WITH (nolock) where eid='" & HttpContext.Current.Session("EID") & "' and uid='" & HttpContext.Current.Session("UID") & "'))) union all "
            qry &= "select 'Rejected'[Status],count(d.tid)[Count] from mmm_mst_doc d with(nolock) inner join mmm_doc_dtl dd with(nolock) on d.tid=dd.docid where d.curstatus='REJECTED' "
            qry &= "and d.documenttype='TRAVEL and ENTERTAINMENT REIMBURSEMENT' and d.eid='" & HttpContext.Current.Session("EID") & "' and dd.remarks='PERMANENT : REJECTED'  and  convert(date,d.adate)>='" & sdate & "' and  convert(date,d.adate)<='" & tdate & "' and  convert(nvarchar,d.fld35) in (select * from  InputString((select top 1 fld1 from mmm_ref_role_user WITH (nolock) where eid='" & HttpContext.Current.Session("EID") & "' and uid='" & HttpContext.Current.Session("UID") & "'))) union all "

            qry &= "select 'Pending for Finance Approval'[Status],count(d.tid)[Count] from mmm_mst_doc d with(nolock) where d.curstatus='Finance Approval' "
            qry &= "and d.documenttype='TRAVEL and ENTERTAINMENT REIMBURSEMENT' and d.eid='" & HttpContext.Current.Session("EID") & "' and  convert(date,d.adate)>='" & sdate & "' and  convert(date,d.adate)<='" & tdate & "' and  convert(nvarchar,d.fld35) in (select * from  InputString((select top 1 fld1 from mmm_ref_role_user WITH (nolock) where eid='" & HttpContext.Current.Session("EID") & "' and uid='" & HttpContext.Current.Session("UID") & "'))) union all "


            qry &= "select 'Reconsidered'[Status],count(d.tid)[Count] from mmm_mst_doc d with(nolock) inner join mmm_doc_dtl dd with(nolock) on d.tid=dd.docid where "
            qry &= "d.documenttype='TRAVEL and ENTERTAINMENT REIMBURSEMENT' and d.eid='" & HttpContext.Current.Session("EID") & "' and dd.aprstatus='REJECTED' and dd.remarks='RECONSIDERED' and convert(date,d.adate)>='" & sdate & "' and  convert(date,d.adate)<='" & tdate & "' and  convert(nvarchar,d.fld35) in (select * from  InputString((select top 1 fld1 from mmm_ref_role_user WITH (nolock) where eid='" & HttpContext.Current.Session("EID") & "' and uid='" & HttpContext.Current.Session("UID") & "')))"

            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As New SqlConnection(conStr)
            Dim dt As New DataTable
            Dim oda As SqlDataAdapter = New SqlDataAdapter(qry, con)
            oda.Fill(dt)
            con.Close()
            Dim lstColumns As New List(Of String)
            Dim serializerSettings As New JsonSerializerSettings()
            Dim json_serializer As New JavaScriptSerializer()
            serializerSettings.Converters.Add(New DataTableConverter())
            jsonData = JsonConvert.SerializeObject(dt, Newtonsoft.Json.Formatting.None, serializerSettings)

        Catch Ex As Exception
            Throw
        End Try
        Return jsonData

    End Function
    <WebMethod()> _
    Public Shared Function getDtl(sdate As String, tdate As String, status As String) As DGrid
        Dim jsonData As String = ""
        Dim grid As New DGrid()
        Try

            Dim ds As New DataSet()
            Dim Query As String = ""
            If status = "Approved" Then
                Query = " select dms.udf_split('MASTER-Employee Master-fld1',d.fld1)[Employee Code],d.fld11[Expense From],d.fld12[Expense To],d.fld13[Advance Taken],"
                Query &= "d.fld14[Travel expense claim],d.fld16[Lodging Expense Claim],d.fld17[Conveyance expense claim],d.fld18[Other expense claim],d.fld19[Total Travel Expense],"
                Query &= "d.fld20[Total Lodging Expense],d.fld21[Total Conveyance Expense],d.fld22[Total Other Expense],d.fld23[Total Amount to be claimed],d.fld24[Claim ID],"
                Query &= "d.fld31[Employee Grade],d.fld32[Booked By Company],d.fld33[Employee Name] from mmm_mst_doc d with(nolock) inner join "
                Query &= "mmm_doc_dtl dd with(nolock) on d.tid=dd.docid where d.curstatus='ARCHIVE' and d.documenttype='TRAVEL and ENTERTAINMENT REIMBURSEMENT' and d.eid='" & HttpContext.Current.Session("EID") & "' and convert(date,d.adate)>='" & sdate & "' and  convert(date,d.adate)<='" & tdate & "' and   convert(nvarchar,d.fld35) in (select * from  InputString((select top 1 fld1 from mmm_ref_role_user WITH (nolock) where eid='" & HttpContext.Current.Session("EID") & "' and uid='" & HttpContext.Current.Session("UID") & "'))) "
                Query &= "and dd.aprstatus='Finance Approval'"
            ElseIf status = "Rejected" Then
                Query = " select dms.udf_split('MASTER-Employee Master-fld1',d.fld1)[Employee Code],d.fld11[Expense From],d.fld12[Expense To],d.fld13[Advance Taken],"
                Query &= "d.fld14[Travel expense claim],d.fld16[Lodging Expense Claim],d.fld17[Conveyance expense claim],d.fld18[Other expense claim],d.fld19[Total Travel Expense],"
                Query &= "d.fld20[Total Lodging Expense],d.fld21[Total Conveyance Expense],d.fld22[Total Other Expense],d.fld23[Total Amount to be claimed],d.fld24[Claim ID],"
                Query &= "d.fld31[Employee Grade],d.fld32[Booked By Company],d.fld33[Employee Name] from mmm_mst_doc d with(nolock) inner join "
                Query &= "mmm_doc_dtl dd with(nolock) on d.tid=dd.docid where d.curstatus='REJECTED' and d.documenttype='TRAVEL and ENTERTAINMENT REIMBURSEMENT' and d.eid='" & HttpContext.Current.Session("EID") & "' and convert(date,d.adate)>='" & sdate & "' and  convert(date,d.adate)<='" & tdate & "' and  convert(nvarchar,d.fld35) in (select * from  InputString((select top 1 fld1 from mmm_ref_role_user WITH (nolock) where eid='" & HttpContext.Current.Session("EID") & "' and uid='" & HttpContext.Current.Session("UID") & "'))) "
                Query &= "and dd.remarks='PERMANENT : REJECTED'"
            ElseIf status = "Reconsidered" Then
                Query = " select dms.udf_split('MASTER-Employee Master-fld1',d.fld1)[Employee Code],d.fld11[Expense From],d.fld12[Expense To],d.fld13[Advance Taken],"
                Query &= "d.fld14[Travel expense claim],d.fld16[Lodging Expense Claim],d.fld17[Conveyance expense claim],d.fld18[Other expense claim],d.fld19[Total Travel Expense],"
                Query &= "d.fld20[Total Lodging Expense],d.fld21[Total Conveyance Expense],d.fld22[Total Other Expense],d.fld23[Total Amount to be claimed],d.fld24[Claim ID],"
                Query &= "d.fld31[Employee Grade],d.fld32[Booked By Company],d.fld33[Employee Name] from mmm_mst_doc d with(nolock) inner join "
                Query &= "mmm_doc_dtl dd with(nolock) on d.tid=dd.docid where d.documenttype='TRAVEL and ENTERTAINMENT REIMBURSEMENT' and d.eid='" & HttpContext.Current.Session("EID") & "' and convert(date,d.adate)>='" & sdate & "' and  convert(date,d.adate)<='" & tdate & "' and  convert(nvarchar,d.fld35) in (select * from  InputString((select top 1 fld1 from mmm_ref_role_user WITH (nolock) where eid='" & HttpContext.Current.Session("EID") & "' and uid='" & HttpContext.Current.Session("UID") & "'))) "
                Query &= " and dd.aprstatus='REJECTED' and dd.remarks='RECONSIDERED'"
            ElseIf status = "Pending for Finance Approval" Then
                Query = " select dms.udf_split('MASTER-Employee Master-fld1',d.fld1)[Employee Code],d.fld11[Expense From],d.fld12[Expense To],d.fld13[Advance Taken],"
                Query &= "d.fld14[Travel expense claim],d.fld16[Lodging Expense Claim],d.fld17[Conveyance expense claim],d.fld18[Other expense claim],d.fld19[Total Travel Expense],"
                Query &= "d.fld20[Total Lodging Expense],d.fld21[Total Conveyance Expense],d.fld22[Total Other Expense],d.fld23[Total Amount to be claimed],d.fld24[Claim ID],"
                Query &= "d.fld31[Employee Grade],d.fld32[Booked By Company],d.fld33[Employee Name] from mmm_mst_doc d with(nolock) "
                Query &= "where d.documenttype='TRAVEL and ENTERTAINMENT REIMBURSEMENT' and d.curstatus='Finance Approval' and d.eid='" & HttpContext.Current.Session("EID") & "' and convert(date,d.adate)>='" & sdate & "' and  convert(date,d.adate)<='" & tdate & "' and  convert(nvarchar,d.fld35) in (select * from  InputString((select top 1 fld1 from mmm_ref_role_user WITH (nolock) where eid='" & HttpContext.Current.Session("EID") & "' and uid='" & HttpContext.Current.Session("UID") & "'))) "


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


    Public Class vdashboard
        Public Property series As List(Of series)
        Public Property categoryAxis As List(Of String)
        Public Property HasSession As String
    End Class
    Public Class series
        Public Property name As String
        Public Property stack As String
        Public Property data As List(Of Integer)
        Public Property color As String
    End Class
End Class

