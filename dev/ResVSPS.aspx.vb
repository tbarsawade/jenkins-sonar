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
Imports System.Web.Script.Serialization
Imports Newtonsoft.Json.Converters

Partial Class ResVSPS
    Inherits System.Web.UI.Page
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
        End If
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

    <System.Web.Services.WebMethod()>
    Public Shared Function getData(FromDate As String, ToDate As String, uid As String, urole As String) As eReportrsps
        Dim jsonData As String = ""
        Dim res As New eReportrsps()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("GetResVSPS", con)
        oda.SelectCommand.CommandType = CommandType.StoredProcedure
        Try
            oda.SelectCommand.Parameters.AddWithValue("@FromDate", FromDate)
            oda.SelectCommand.Parameters.AddWithValue("@ToDate", ToDate)
            oda.SelectCommand.Parameters.AddWithValue("@UID", Convert.ToInt16(uid))
            oda.SelectCommand.Parameters.AddWithValue("@URole", urole)

            Dim ds As New DataSet()
            oda.Fill(ds)
            Dim serializerSettings As New JsonSerializerSettings()
            Dim json_serializer As New JavaScriptSerializer()

            serializerSettings.Converters.Add(New DataTableConverter())
            Dim lstColumns As New List(Of grdcolumns1rsps)
            Dim lstaggregate As New List(Of aggragatersps)
            Dim objColumn As grdcolumns1rsps
            Dim objaggregate = New aggragatersps()
            '  res.aggregate = "["
            For Each dc As DataColumn In ds.Tables(0).Columns
                objaggregate = New aggragatersps()
                objColumn = New grdcolumns1rsps()
                objColumn.field = Replace(dc.ColumnName.Replace("-", ""), " ", "")
                objColumn.title = dc.ColumnName
                If (objColumn.title.Length > 5) Then
                    If (objColumn.title.Length < 11) Then
                        objColumn.width = objColumn.title.Length * 18
                    ElseIf (objColumn.title.Length < 16) Then
                        objColumn.width = objColumn.title.Length * 19
                    Else
                        objColumn.width = objColumn.title.Length * 11
                    End If

                End If

                lstColumns.Add(objColumn)
                dc.ColumnName = dc.ColumnName.Replace("-", "").Replace(" ", "")
                If dc.ColumnName = "UserName" Or dc.ColumnName = "Domain" Then
                    objColumn.type = "string"
                    If (dc.ColumnName = "UserName") Then
                        objColumn.footerTemplate = " Grand Total :"
                    End If

                Else
                    objColumn.type = "number"
                    objaggregate.field = dc.ColumnName
                    objaggregate.aggregate = "sum"
                    lstaggregate.Add(objaggregate)
                    ' res.aggregate &= "{field:""" & dc.ColumnName & """,aggregate: ""sum"" },"
                    objColumn.aggregates = "[""sum""]"
                    objColumn.footerTemplate = "#= sum#"
                End If


            Next
            '  res.aggregate &= "]"
            jsonData = JsonConvert.SerializeObject(ds.Tables(0), Newtonsoft.Json.Formatting.None, serializerSettings)
            res.data = jsonData
            res.columns = lstColumns
            res.aggregate = lstaggregate
            jsonData = JsonConvert.SerializeObject(ds.Tables(0), Newtonsoft.Json.Formatting.None)
            Return res
        Catch ex As Exception
            Throw
        End Try
    End Function

End Class

Public Class eReportrsps
    Public Property data As String
    Public Property columns As List(Of grdcolumns1rsps)
    Public Property aggregate As List(Of aggragatersps)
End Class

Public Class grdcolumns1rsps
    Public Property field As String
    Public Property title As String
    Public Property aggregates As String = ""
    Public Property type As String = ""
    Public Property footerTemplate As String = ""
    Public Property width As Integer = 100
End Class


Public Class aggragatersps
    Public Property field As String
    Public aggregate As String
End Class

