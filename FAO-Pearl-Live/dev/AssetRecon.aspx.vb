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

Partial Class AssetRecon
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
        txtd1.Text = Now.Date
        txtd2.Text = Now.Date
    End Sub
    <WebMethod()>
    Public Shared Function GetData(sdate As String, tdate As String) As DGrid
        Dim grid As New DGrid()
        Dim strError = ""

        Try
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As SqlConnection = New SqlConnection(conStr)
            Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
            Dim dt As New DataTable
            Dim qry As String = ""

            qry = "select tid[Doc ID],dms.udf_split('MASTER-SITE Master-fld1',fld2)[SiteID],fld3[Category Code],fld4[Asset Name],fld5[Make],fld6[Model],"
            qry &= "fld7[Quantity],fld9[Type],case when fld1 in (select convert(varchar,tid) from mmm_mst_master with(nolock) where eid=" & HttpContext.Current.Session("EID").ToString() & " and documenttype='Asset Master') then 'Verified' "
            qry &= "when fld1='' then  'New Entry'when fld1 not in (select convert(varchar,tid) from mmm_mst_master with(nolock) where eid=" & HttpContext.Current.Session("EID").ToString() & " and documenttype='Asset Master') then 'Unverified' "
            qry &= "end[Status] from mmm_mst_master with(nolock) where eid=" & HttpContext.Current.Session("EID").ToString() & " and documenttype='Physical verification' and convert(date,fld10,3)>='" & sdate & "' and  convert(date,fld10,3)<='" & tdate & "' order by tid desc"

            oda.SelectCommand.CommandText = qry
            Dim ds As New DataSet()

            Try

                Try
                    oda.SelectCommand.CommandTimeout = 300
                    oda.Fill(ds, "data")
                    If ds.Tables("data").Rows.Count = 0 Then
                        grid.Success = False
                        grid.Message = "No data found."
                    Else
                        grid = DynamicGrid.GridData(ds.Tables("data"), strError)
                    End If
                Catch exption As Exception
                    grid.Success = False
                    grid.Message = "Dear User please enter short date range."
                End Try
            Catch ex As Exception
                grid.Success = False
                grid.Message = "No data found."
            Finally
                con.Close()
                oda.Dispose()
                con.Dispose()
            End Try
        Catch ex As Exception

        End Try
        Return grid
    End Function

End Class
