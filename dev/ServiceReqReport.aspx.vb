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

Partial Class ServiceReqReport
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
    <WebMethod()>
    Public Shared Function GetDSlip(d1 As String) As DGrid
        Dim grid As New DGrid()
        Dim strError = ""

        Try
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As SqlConnection = New SqlConnection(conStr)
            Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
            Dim dt As New DataTable
            Dim qry As String = ""


            If d1 = "Open" Then

                qry = "Select dms.udf_split('MASTER-Project Master-fld1',d.fld11)[Project Name],(select username from mmm_mst_user with(nolock) where uid=d.ouid)[Created By],"
                qry &= "convert(varchar(10),convert(date,d.adate))[Creation Date],d.curstatus[Current Status],d.fld2[Request Priority]"
                qry &= "from mmm_mst_doc d with(nolock) where d.eid=100 and d.documenttype='service request' and d.curstatus<>'ARCHIVE' ORDER BY d.TID DESC "

            Else

                qry = "Select dms.udf_split('MASTER-Project Master-fld1',d.fld11)[Project Name],(select username from mmm_mst_user with(nolock) where uid=d.ouid)[Created By],"
                qry &= "convert(varchar(10),convert(date,d.adate))[Creation Date],d.curstatus[Current Status],d.fld2[Request Priority]"
                qry &= "from mmm_mst_doc d with(nolock) where d.eid=100 and d.documenttype='service request' and d.curstatus='ARCHIVE' ORDER BY d.TID DESC "

            End If


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
