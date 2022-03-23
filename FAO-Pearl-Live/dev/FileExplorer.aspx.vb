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



Partial Class FileExplorer
    Inherits System.Web.UI.Page
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
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
    Public Shared Function GetData() As treeList

        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim sStatement As String = ""
        Dim qrycolumn As String = ""
        Dim jsonString As String = ""
        Dim ds As New DataSet()
        Dim dtComp As New DataTable()
        Dim dtSite As New DataTable()
        Dim objret As New treeList()
        Dim objChild As treeClass
        Dim lstData As New List(Of treeClass)
        Dim item As List(Of items)
        Dim objitem As items
        'sStatement = "Select   distinct Top 50 M.tid[SYSTEM ID], M.DocumentType[SUBJECT],M.curstatus[STATUS],U.Username[CREATED BY],(replace(convert(char(11),d.fdate,113),' ','-') + ' ' + left(convert(varchar(13),d.fdate,108),8))[RECEIVED ON],datediff(day,fdate,getdate())[PENDING DAYS],M.PRIORITY  ,"
        qrycolumn = "Select M.tid, M.fld29 As [COMPANY] ,M.fld30 As [Site] from MMM_MST_DOC M with (nolock)  LEFT OUTER JOIN MMM_DOC_DTL D with (nolock) on D.tid=M.lasttid  LEFT OUTER JOIN MMM_MST_USER U with (nolock) ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP with (nolock) ON AP.uid=D.UserID where D.userid = " & Val(HttpContext.Current.Session("UID").ToString()) & ""
        Dim oda As SqlDataAdapter = New SqlDataAdapter(qrycolumn, con)
        Try
            oda.SelectCommand.CommandType = CommandType.Text
            oda.Fill(ds, "data")
            Dim view As DataView = New DataView(ds.Tables(0))
            dtComp = view.ToTable(True, "COMPANY")
            For i As Integer = 0 To dtComp.Rows.Count - 1
                objChild = New treeClass()
                objChild.text = dtComp.Rows(i)("COMPANY")
                item = New List(Of items)
                For j As Integer = 0 To ds.Tables(0).Rows.Count - 1
                    If ds.Tables(0).Rows(j)("COMPANY") = dtComp.Rows(i)("COMPANY") Then
                        objitem = New items()
                        objitem.text = ds.Tables(0).Rows(j)("Site").ToString()
                        objitem.Tid = ds.Tables(0).Rows(j)("tid").ToString()
                        item.Add(objitem)
                    End If
                Next

                objChild.items = item
                lstData.Add(objChild)
            Next
            objret.items = lstData
            'jsonString = JsonConvert.SerializeObject(objChild)

        Catch ex As Exception
        Finally
            con.Close()
            oda.Dispose()
        End Try
        Return objret
    End Function
    <System.Web.Services.WebMethod()>
    Public Shared Function GetDocument(TID As String) As String
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim qrycolumn As String = ""
        Dim jsonString As String = ""
        Dim ds As New DataSet()
        qrycolumn = "Select M.Eid, M.TID as [SYSTEM ID],M.TID As [DocID] ,M.fld29 As [COMPANY] ,M.fld30 As [Site] ,M.fld31 As [Act] ,M.fld47 As [PE Activity ] ,M.fld46 As [Contractor Activity ] ,M.fld44 As [Contractor Name] ,M.curstatus As [Status] ,d.fdate As [Received On] ,M.fld43 As [Expiry Date] ,datediff(day,fdate,getdate()) As [Pending Days],ISNULL(M.fld10,'') AS [FileUpload1],ISNULL(M.fld11,'') AS [FileUpload2],ISNULL(M.fld12,'') AS [FileUpload3],ISNULL(M.fld13,'') AS [FileUpload4],ISNULL(M.fld14,'') AS [FileUpload5] from MMM_MST_DOC M with (nolock)  LEFT OUTER JOIN MMM_DOC_DTL D with (nolock) on D.tid=M.lasttid  LEFT OUTER JOIN MMM_MST_USER U with (nolock) ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP with (nolock) ON AP.uid=D.UserID where D.userid = " & Val(HttpContext.Current.Session("UID").ToString()) & " and M.tid='" & TID & "' and m.curstatus<>'ARCHIVE'"
        Dim oda As SqlDataAdapter = New SqlDataAdapter(qrycolumn, con)
        Try
            oda.SelectCommand.CommandType = CommandType.Text
            oda.Fill(ds, "data")
            jsonString = JsonConvert.SerializeObject(ds)
        Catch ex As Exception

        Finally
            con.Close()
            oda.Dispose()
        End Try
        Return jsonString
    End Function
End Class

'Create User define class.
Public Class treeClass
    Public text As String = ""
    Public items As New List(Of items)
End Class
Public Class items
    Public Tid As Integer
    Public text As String = ""
End Class
Public Class treeList
    Public items As New List(Of treeClass)

End Class


