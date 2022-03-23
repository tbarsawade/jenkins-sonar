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
Partial Class PHRODetails
    Inherits System.Web.UI.Page
    Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
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
    Public Shared Function GetPHRODetails() As kGridPhRo
        Dim dataobj As New DataLib()
        Dim jsonData As String = ""
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim dsData As New DataSet()
        Dim strQuery As String = ""
        Dim ret As New kGridPhRo()
        Try
            strQuery = "SELECT Tid,IMINumber,Mobile_Number,Emp_Code,Comp_Code,ISNULL(AdminAppr_Status,'') AdminAppr_Status,Email_ID,ISNULL(Comment,'') Comment FROM PHRO_Employee"
            dsData = DataLib.ExecuteDataSet(conStr, CommandType.Text, strQuery)

            Dim listcol As New List(Of KColumnPhRo)()
            listcol.Add(New KColumnPhRo("IMINumber", "IMINumber", "string"))
            listcol.Add(New KColumnPhRo("Mobile_Number", "Mobile Number", "string"))
            listcol.Add(New KColumnPhRo("Emp_Code", "Emp Code", "string"))
            listcol.Add(New KColumnPhRo("AdminAppr_Status", "AdminAppr Status", "string"))
            'hh:mm:ss tt    KColumnPhRo
            listcol.Add(New KColumnPhRo("Email_ID", "Email ID", "string"))
            jsonData = JsonConvert.SerializeObject(dsData.Tables(0))
            ret.Data = jsonData
            ret.Column = listcol

        Catch ex As Exception
            Throw
        Finally
            'oda.Dispose()

        End Try
        Return ret
    End Function
    <System.Web.Services.WebMethod()>
    Public Shared Function SetApprStatus(Tid As String, Status As String, Comment As String) As String
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim result As String = ""
        Dim Con As SqlConnection = New SqlConnection(conStr)
        Dim Cmd As SqlCommand = New SqlCommand("SetPHROApprStatus", Con)
        Cmd.CommandType = CommandType.StoredProcedure
        Cmd.Parameters.AddWithValue("@TID", Tid)
        Cmd.Parameters.AddWithValue("@ApprStatus", Status)
        Cmd.Parameters.AddWithValue("@Comment", Comment)
        Try
            Con.Open()
            result = Cmd.ExecuteScalar().ToString()
        Catch ex As Exception
            result = "fail"
        Finally
            Con.Close()
            Con.Dispose()
            Cmd.Dispose()
        End Try
        Return JsonConvert.SerializeObject(result)
    End Function
End Class
Public Class kGridPhRo
    Public Data As String = ""
    Public Column As New List(Of KColumnPhRo)
End Class
Public Class KColumnPhRo
    Public Sub New(staticfield As [String], statictitle As [String], statictype As String)
        field = staticfield
        title = statictitle
        type = statictype
    End Sub

    Public field As String = ""
    Public title As String = ""
    Public width As Integer = 200
    Public type As String = ""
End Class

