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

Partial Class MonthWiseActivity
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

    <WebMethod()> _
          <Script.Services.ScriptMethod()> _
    Public Shared Function GetCompany() As String

        Dim ret As String = ""
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim Qry As String = "select Distinct fld1[Company] FROM mmm_mst_master with(nolock) where eid=98 and documenttype='Company Master'"
        Dim ds As New DataSet()
        Using con As New SqlConnection(conStr)
            Using da As New SqlDataAdapter(Qry, con)
                da.Fill(ds)
            End Using
        End Using
        Dim serializerSettings As New JsonSerializerSettings()
        Dim json_serializer As New JavaScriptSerializer()
        serializerSettings.Converters.Add(New DataTableConverter())
        ret = JsonConvert.SerializeObject(ds.Tables(0), Newtonsoft.Json.Formatting.None, serializerSettings)
        Return ret
    End Function
    <WebMethod()>
    Public Shared Function GetReport(company As String, Y1 As String) As DGrid
        Dim grid As New DGrid()
        Dim strError = ""

        Try
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As SqlConnection = New SqlConnection(conStr)
            Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
            Dim ds As New DataSet
            Dim qry As String = ""
            oda.SelectCommand.CommandType = CommandType.Text
            oda.SelectCommand.CommandText = "select qryfield from mmm_mst_report where reportid=424"
            oda.Fill(ds, "qry")
            qry = ds.Tables("qry").Rows(0).Item(0).ToString()
            Dim yr As String = ""

            Dim com() As String
            Dim str As String = ""
            com = company.Split(",")
            For i = 0 To com.Length - 1
                str = str & "'" & com(i).ToString & "',"
            Next
            str = Left(str, str.Length - 1)

            qry = Replace(qry, "@year", "'" & Y1 & "'")
            qry = Replace(qry, "@Comp", str)

            oda.SelectCommand.CommandText = qry
            
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




