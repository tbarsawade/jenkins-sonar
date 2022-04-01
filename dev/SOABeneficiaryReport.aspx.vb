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

Partial Class SOABeneficiaryReport
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
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As New SqlConnection(conStr)
            Dim da As New SqlDataAdapter("", con)
            If Session("USERROLE") = "SU" Or Session("USERROLE") = "CADMIN" Then
                da.SelectCommand.CommandText = "Select distinct fld2 from mmm_mst_master where eid=" & Session("EID") & " and documenttype='Hub Master' "
            Else
                da.SelectCommand.CommandText = "Select distinct fld2 from mmm_mst_master where eid=" & Session("EID") & " and documenttype='Hub Master' and tid in (select * from DMS.InputString1((select fld3 from  mmm_ref_role_user where uid=" & Session("UID") & "))) "
            End If
            '        Dim da As New SqlDataAdapter("Select * from mmm_mst_Reallocation where eid=" & Session("EID") & " and role='" & Session("USERROLE") & "'", con)
            Dim ds As New DataSet
            da.Fill(ds, "data")
            ddlbName.DataSource = ds.Tables("data")
            ddlbName.DataTextField = "fld2"
            ddlbName.DataValueField = "fld2"
            ddlbName.DataBind()
            ddlbName.Items.Insert(0, New ListItem("Select"))
            con.Close()
            txtd1.Text = Now.AddDays(-1).ToString("yyyy-MM-dd")
            txtd2.Text = Now.ToString("yyyy-MM-dd")
        End If
    End Sub
    <WebMethod()>
    Public Shared Function GetData(sdate As String, tdate As String, BName As String) As DGrid
        Dim jsonData As String = ""
        Dim grid As New DGrid()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim da As New SqlDataAdapter("", con)
        Try
            Dim tbl As New DataTable
            tbl.Columns.Add("Beneficiary Name", GetType(String))
            tbl.Columns.Add("Hub Code", GetType(String))
            tbl.Columns.Add("Hub Name", GetType(String))
            tbl.Columns.Add("Voucher No", GetType(String))
            tbl.Columns.Add("Voucher Date", GetType(String))
            tbl.Columns.Add("Copmpany Name", GetType(String))
            tbl.Columns.Add("Country", GetType(String))
            tbl.Columns.Add("Currency", GetType(String))
            tbl.Columns.Add("Cash Feed", GetType(String))
            tbl.Columns.Add("JV Setteled", GetType(String))
            tbl.Columns.Add("Cash Deposit", GetType(String))
            tbl.Columns.Add("Physical Balance", GetType(String))

            Dim ds As New DataSet
            da.SelectCommand.CommandType = CommandType.Text
            Dim ff = 0
            da.SelectCommand.CommandText = "select isnull((select isnull(convert(float,fld17),0) from mmm_mst_history where eid=165 and documenttype='hub master' and tid in ((select max(tid) from mmm_mst_history where fld2='" & BName & "' and convert(date,adate)<convert(date,'" & sdate & "')))),0)"
            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            ff = da.SelectCommand.ExecuteScalar()
            If ff = 0 Then
                da.SelectCommand.CommandText = "select isnull((select isnull(convert(float,fld17),0) from mmm_mst_history where eid=165 and documenttype='hub master' and tid in ((select min(tid) from mmm_mst_history where fld2='" & BName & "' ))),0)"
            End If
            ff = da.SelectCommand.ExecuteScalar()
            da.SelectCommand.CommandText = "select qryfield from mmm_mst_report where reportid=1713"
            da.Fill(ds, "data")
            Dim qry As String = ds.Tables("data").Rows(0).Item(0).ToString   '' get qry (child items of selected doc id)
            da.SelectCommand.CommandType = CommandType.Text
            qry = Replace(qry, "@BName", "'" & BName & "'")
            qry = Replace(qry, "@Date1", "'" & sdate & "'")
            qry = Replace(qry, "@Date2", "'" & tdate & "'")
            da.SelectCommand.CommandText = qry
            Dim SOA As New DataTable
            da.Fill(SOA)
            Dim phbal = 0
            ' FOR CHILD ITEM AND DOC JOIN
            Dim DocCnt As Integer = 0
            If SOA.Rows.Count > 0 Then
                For i = 0 To SOA.Rows.Count - 1   '' main doc table loop 
                    DocCnt = i + 1
                    Dim rw As DataRow
                    rw = tbl.NewRow
                    If i = 0 Then
                        rw(0) = SOA.Rows(i).Item(0).ToString
                        rw(1) = SOA.Rows(i).Item(1).ToString
                        rw(2) = SOA.Rows(i).Item(2).ToString
                        rw(3) = SOA.Rows(i).Item(3).ToString
                        rw(4) = SOA.Rows(i).Item(4).ToString
                        rw(5) = SOA.Rows(i).Item(5).ToString
                        rw(6) = SOA.Rows(i).Item(6).ToString
                        rw(7) = SOA.Rows(i).Item(7).ToString
                        rw(8) = SOA.Rows(i).Item(8).ToString
                        rw(9) = SOA.Rows(i).Item(9).ToString
                        rw(10) = SOA.Rows(i).Item(10).ToString
                        phbal = ff + SOA.Rows(i).Item(8) - SOA.Rows(i).Item(9) - SOA.Rows(i).Item(10)
                        rw(11) = phbal.ToString
                        tbl.Rows.Add(rw)
                        'SOA.Clear()
                    Else
                        rw(0) = SOA.Rows(i).Item(0).ToString
                        rw(1) = SOA.Rows(i).Item(1).ToString
                        rw(2) = SOA.Rows(i).Item(2).ToString
                        rw(3) = SOA.Rows(i).Item(3).ToString
                        rw(4) = SOA.Rows(i).Item(4).ToString
                        rw(5) = SOA.Rows(i).Item(5).ToString
                        rw(6) = SOA.Rows(i).Item(6).ToString
                        rw(7) = SOA.Rows(i).Item(7).ToString
                        rw(8) = SOA.Rows(i).Item(8).ToString
                        rw(9) = SOA.Rows(i).Item(9).ToString
                        rw(10) = SOA.Rows(i).Item(10).ToString
                        phbal = phbal + SOA.Rows(i).Item(8) - SOA.Rows(i).Item(9) - SOA.Rows(i).Item(10)
                        rw(11) = phbal.ToString
                        tbl.Rows.Add(rw)
                    End If
                Next
            End If
            Dim strError = ""
            grid = DynamicGrid.GridData(tbl, strError)
            If tbl.Rows.Count = 0 Then
                grid.Message = "No data found...!"
                grid.Success = False
            End If
        Catch Ex As Exception
            grid.Success = False
            grid.Message = "No data found...!"
            grid.Count = 0
        Finally
            con.Close()
            da.Dispose()
            con.Dispose()
        End Try
        Return grid
    End Function
End Class
