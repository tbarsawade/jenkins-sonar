Imports System.Data.SqlClient
Imports System.Data
Imports System.Web.Services
Imports Newtonsoft.Json
Imports System.Web.Script.Serialization
Imports Newtonsoft.Json.Converters
Partial Class EcompMontlhyScheduledDocumentRpt
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
    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load

        If Not IsPostBack Then

            FillDDL()

        End If

    End Sub
    Sub FillDDL()

        Try

            For i As Integer = 2015 To DateTime.Now.Year
                ddlYear.Items.Add(New ListItem(i.ToString(), i.ToString()))
            Next
            ddlYear.SelectedValue = DateTime.Now.Year.ToString()
            ddlMonth.SelectedValue = DateTime.Now.Month.ToString()
        Catch ex As Exception

        End Try

    End Sub

    <WebMethod> _
    Public Shared Function GetJSON(Month As String, year As String) As eReport
        Dim jsonData As String = ""

        Dim res As New eReport()

        Try
            Dim ds As New DataSet()

            Dim Query As String = "uspGetMonthlyScheduledDocumentRpt " & Month & "," & year & ", 'PE'"
            Dim constr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As New SqlConnection(constr)
            Dim dt As New DataTable
            Dim oda As SqlDataAdapter = New SqlDataAdapter(Query, con)
            oda.Fill(dt)
            Dim lstColumns As New List(Of grdcolumns)
            Dim objColumn As grdcolumns

            For i As Integer = 0 To dt.Columns.Count - 1

                objColumn = New grdcolumns()
                objColumn.field = Replace(dt.Columns(i).ColumnName, " ", "_")
                objColumn.title = dt.Columns(i).ColumnName
                If (dt.Columns(i).ColumnName.ToString() = "Created" Or dt.Columns(i).ColumnName.ToString() = "To be created") Then
                    objColumn.type = "number"
                End If

                lstColumns.Add(objColumn)
                dt.Columns(i).ColumnName = Replace(dt.Columns(i).ColumnName, " ", "_")

            Next


            Dim serializerSettings As New JsonSerializerSettings()
            Dim json_serializer As New JavaScriptSerializer()

            serializerSettings.Converters.Add(New DataTableConverter())


            jsonData = JsonConvert.SerializeObject(dt, Newtonsoft.Json.Formatting.None, serializerSettings)
            res.data = jsonData
            res.columns = lstColumns



        Catch Ex As Exception
            Throw
        End Try
        Return res

    End Function

    <WebMethod> _
    Public Shared Function GetJSONcontractor(Month As String, year As String) As eReport
        Dim jsonData As String = ""

        Dim res As New eReport()

        Try
            Dim ds As New DataSet()

            Dim Query As String = "uspGetMonthlyScheduledDocumentRpt " & Month & "," & year & ", 'Contractor'"
            Dim constr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As New SqlConnection(constr)
            Dim dt As New DataTable
            Dim oda As SqlDataAdapter = New SqlDataAdapter(Query, con)
            oda.Fill(dt)
            Dim lstColumns As New List(Of grdcolumns)
            Dim objColumn As grdcolumns

            For i As Integer = 0 To dt.Columns.Count - 1

                objColumn = New grdcolumns()
                objColumn.field = Replace(dt.Columns(i).ColumnName, " ", "_")
                objColumn.title = dt.Columns(i).ColumnName

                If (dt.Columns(i).ColumnName.ToString() = "Created" Or dt.Columns(i).ColumnName.ToString() = "To be created") Then
                    objColumn.type = "number"
                End If

                lstColumns.Add(objColumn)
                dt.Columns(i).ColumnName = Replace(dt.Columns(i).ColumnName, " ", "_")

            Next


            Dim serializerSettings As New JsonSerializerSettings()
            Dim json_serializer As New JavaScriptSerializer()
            serializerSettings.Converters.Add(New DataTableConverter())
            jsonData = JsonConvert.SerializeObject(dt, Newtonsoft.Json.Formatting.None, serializerSettings)
            res.data = jsonData
            res.columns = lstColumns



        Catch Ex As Exception
            Throw
        End Try
        Return res

    End Function

   

    

    Public Class eReport

        Public Property data As String
        Public Property columns As List(Of grdcolumns)

    End Class

    Public Class grdcolumns

        Public Property field As String
        Public Property title As String
        Public Property groupFooterTemplate As String = ""
        Public Property groupHeaderTemplate As String = ""
        Public Property aggregates As String = ""
        Public Property type As String = "string"
        '  Public Property template As String = ""

    End Class
End Class
