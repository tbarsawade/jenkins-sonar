Imports System.Data
Imports System.Data.SqlClient
Imports Microsoft.VisualBasic.FileIO
Imports System.Web.Services

Partial Class XmlSettings
    Inherits System.Web.UI.Page
    
    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        ''for SU 
        'Session("UID") = 2042 '6231
        'Session("USERNAME") = "Prashant Singh Sengar"
        'Session("USERROLE") = "SU"
        'Session("CODE") = "PAL"
        'Session("USERIMAGE") = "2.jpg"
        'Session("CLOGO") = "hfcl.png"
        'Session("EID") = 43
        'Session("ISLOCAL") = "TRUE"
        'Session("IPADDRESS") = "Vinay"
        'Session("MACADDRESS") = "Vinay"
        'Session("INTIME") = Now
        'Session("EMAIL") = "vinay.kumar@myndsol.com"
        'Session("LID") = "25"
        'Session("HEADERSTRIP") = "hfclstrip.jpg"
        'Session("ROLES") = "SU"
        If Not IsPostBack Then
            Bindddl()
        End If
    End Sub
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
    Public Sub Bindddl()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim da As New SqlDataAdapter("Select FormID [Value], FormName[Text] from  mmm_mst_Forms where Eid=" & Session("Eid") & " and IsActive=1 order by FormName", con)
        Dim dt As New DataTable()
        da.Fill(dt)
        ddlDocType.DataValueField = "Value"
        ddlDocType.DataTextField = "Text"
        ddlDocType.DataSource = dt
        ddlDocType.DataBind()
        ddlDocType.Items.Insert(0, "Select")
        ddlDocType.Items(0).Value = "0"
    End Sub

    <WebMethod()> _
<Script.Services.ScriptMethod()> _
    Public Shared Function GetFields(DocType As String) As XmlSettingsIO
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim da As New SqlDataAdapter("", con)
        Dim obj As New XmlSettingsIO()
        Try
            da.SelectCommand.CommandText = "Select FieldID, DisplayName, IsNull(OUTWARDXMLTAGNAME,'') OUTWARDXMLTAGNAME from mmm_mst_Fields with(nolock) where IsActive=1 and Eid=" & HttpContext.Current.Session("Eid") & " and Documenttype='" & DocType & "'"
            Dim dt As New DataTable()
            da.Fill(dt)
            Dim strData As String = ""
            strData = ToCsv(dt, "^", "|")
            obj.Fields = strData
            da.SelectCommand.CommandText = "Select FTPTime from mmm_mst_Forms with(nolock) where Eid=" & HttpContext.Current.Session("Eid") & " and FormName='" & DocType & "'"
            con.Open()
            Dim ftpTime As String = Convert.ToString(da.SelectCommand.ExecuteScalar())
            con.Close()

            If Not ftpTime = "0" And Not ftpTime = "" Then
                obj.FtpHr = ftpTime.Split(":")(0)
                obj.FtpMin = ftpTime.Split(":")(1)
            Else
                obj.FtpHr = ""
                obj.FtpMin = ""
            End If
            obj.Success = True
        Catch ex As Exception
            obj.Success = False
        Finally
            da.Dispose()
            con.Dispose()
        End Try
        Return obj
    End Function


    <WebMethod()> _
<Script.Services.ScriptMethod()> _
    Public Shared Function SaveSettings(Form As Dictionary(Of String, String), Fields As Dictionary(Of String, String)) As XmlSettingsReturn
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim da As New SqlDataAdapter("", con)
        Dim obj As New XmlSettingsReturn()
        Dim tran As SqlTransaction = Nothing
        Try
            If IsNothing(HttpContext.Current.Session("Eid")) Then
                obj.Message = "Session Expired.<br/>"
                obj.Success = False
            ElseIf Form("Hr").ToString.Trim = "" Or Form("Min").ToString.Trim = "" Then
                obj.Message = "Please enter Ftp Time.<br/>"
                obj.Success = False
            Else
                da.SelectCommand.CommandText = "Update mmm_mst_Forms set FTPTime=@ftpTime where Eid=@Eid and FormID=@FormId"
                da.SelectCommand.Parameters.AddWithValue("@ftpTime", Form("Hr").ToString.Trim & ":" & Form("Min").ToString.Trim)
                da.SelectCommand.Parameters.AddWithValue("@Eid", HttpContext.Current.Session("Eid"))
                da.SelectCommand.Parameters.AddWithValue("@FormId", Form("FormId").ToString)

                con.Open()
                con.BeginTransaction()
                da.SelectCommand.ExecuteNonQuery()
                For Each key In Fields.Keys
                    da.SelectCommand.CommandText = "Update mmm_mst_Fields set OUTWARDXMLTAGNAME=@xml where Eid=@Eid and FieldId=@FieldId"
                    da.SelectCommand.Parameters.AddWithValue("@xml", Fields(key).ToString())
                    da.SelectCommand.Parameters.AddWithValue("@Eid", HttpContext.Current.Session("Eid"))
                    da.SelectCommand.Parameters.AddWithValue("@FieldId", key.ToString)
                    da.SelectCommand.ExecuteNonQuery()
                Next
                tran.Commit()
                con.Close()
                obj.Message = "Settings Saved.<br/>"
                obj.Success = True
            End If
        Catch ex As Exception
            tran.Rollback()
            obj.Success = False
            obj.Message = "Server Error"
        Finally
            con.Dispose()
            da.Dispose()
        End Try
        Return obj
    End Function

    Public Shared Function ToCsv(dt As DataTable, ColSeperator As String, RowSeperator As String) As String
        Try
            Dim arr = dt.AsEnumerable().[Select](Function(row) String.Join(ColSeperator, row.ItemArray)).ToArray()
            Dim s = String.Join(RowSeperator, arr)
            Return s
        Catch ex As Exception
        End Try
        Return ""
    End Function
End Class

Public Class XmlSettingsReturn
    Public Property Success As Boolean
    Public Property Message As String
End Class

Public Class XmlSettingsIO
    Public Property Success As Boolean
    Public Property FormID As String
    Public Property Doctype As String
    Public Property FtpHr As String
    Public Property FtpMin As String
    Public Property Fields As String
End Class