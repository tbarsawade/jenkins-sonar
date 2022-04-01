Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports System.Web.UI.Adapters.ControlAdapter
Imports System.Drawing
Imports System.Threading
Imports System.Net.Mail
Imports System.Net
Imports System.Net.HttpWebRequest
Imports System.Net.HttpWebResponse
Imports System
Imports System.Collections.Specialized
Imports System.Text
Imports System.Net.Security
Imports System.Web.Hosting


Partial Class checkwindowServices
    Inherits System.Web.UI.Page
    Private Fadd As String = "ftp://103.25.172.28/FORMYND"
    Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        FTPREPORT()
    End Sub

    Sub FTPREPORT()

        Dim con As New SqlConnection(constr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
        Dim ds As New DataSet()
        Dim FPath As String = HostingEnvironment.MapPath("~/MailAttach/")
        Try
            oda.SelectCommand.CommandText = "select * from  MMM_MST_ReportScheduler where FtpFlag='1'"
            oda.SelectCommand.CommandType = CommandType.Text
            oda.Fill(ds, "rpt")
            For d As Integer = 0 To ds.Tables("rpt").Rows.Count - 1
                ' If ReportScheduler(ds.Tables("rpt").Rows(d).Item("tid")) = True Then
                'If ds.Tables("rpt").Rows(d).Item("sendto").ToString.ToUpper = "USER" Then
                Dim str As String = ds.Tables("rpt").Rows(d).Item("reportquery").ToString()
                Dim lastschedule As String = ds.Tables("rpt").Rows(d).Item("LastScheduledDate").ToString
                lastschedule = Format(Convert.ToDateTime(lastschedule.ToString), "yyyy-MM-dd HH:mm:ss:fff")
                str = Replace(str, "@lastsch", lastschedule)
                oda.SelectCommand.CommandText = str
                oda.SelectCommand.CommandType = CommandType.Text
                Dim FTPR As New DataTable
                oda.Fill(FTPR)
                If FTPR.Rows.Count > 0 Then
                    Dim CNT As Integer = 0
                    Dim msg As String = ds.Tables("rpt").Rows(d).Item("msgbody").ToString()
                    oda.SelectCommand.CommandText = "INSERT_MAILLOGNEW"
                    oda.SelectCommand.CommandType = CommandType.StoredProcedure
                    oda.SelectCommand.Parameters.Clear()
                    oda.SelectCommand.Parameters.AddWithValue("@MAILTO", "FTP")
                    oda.SelectCommand.Parameters.AddWithValue("@CC", "")
                    oda.SelectCommand.Parameters.AddWithValue("@MSG", msg)
                    oda.SelectCommand.Parameters.AddWithValue("@ALERTTYPE", "FTPREPORT")
                    oda.SelectCommand.Parameters.AddWithValue("@MAILEVENT", ds.Tables("rpt").Rows(d).Item("reportname").ToString())
                    oda.SelectCommand.Parameters.AddWithValue("@EID", ds.Tables("rpt").Rows(d).Item("eid").ToString())
                    oda.SelectCommand.Parameters.AddWithValue("@RSID", ds.Tables("rpt").Rows(d).Item("tid").ToString)
                    If con.State <> ConnectionState.Open Then
                        con.Open()
                    End If
                    oda.SelectCommand.ExecuteNonQuery()
                    lbln.Text = "update succssfully"
                    Dim fname As String = ""
                    fname = CreateCSV(FTPR)
                    lbln.Text = "csv created"
                    CopyfiletoInbox(Fadd, "cushman", "qwe123#", FPath, fname, "FORMYND")

                    'Dim clsRequest As System.Net.FtpWebRequest = DirectCast(System.Net.WebRequest.Create("ftp://103.25.172.28/FORHCL/" & fname), System.Net.FtpWebRequest)
                    'clsRequest.Credentials = New System.Net.NetworkCredential("hcl", "sh1nodrm")
                    'clsRequest.Method = System.Net.WebRequestMethods.Ftp.UploadFile
                    '' read in file..
                    'Dim bFile() As Byte = System.IO.File.ReadAllBytes(FPath1 & fname)
                    '' upload file...
                    'Dim clsStream As System.IO.Stream = clsRequest.GetRequestStream()

                    'clsStream.Write(bFile, 0, bFile.Length)

                    'clsStream.Close()
                    'clsStream.Dispose()


                    lbln.Text = lbln.Text & "File Sent Successfully"
                End If
                ' End If
                'End If
            Next
        Catch ex As Exception
            oda.SelectCommand.CommandText = "INSERT_ERRORLOG"
            oda.SelectCommand.CommandType = CommandType.StoredProcedure
            oda.SelectCommand.Parameters.Clear()
            oda.SelectCommand.Parameters.AddWithValue("@ERRORMSG", ex.ToString())
            oda.SelectCommand.Parameters.AddWithValue("@ALERTNAME", "FTPREPORT")
            oda.SelectCommand.Parameters.AddWithValue("@EID", 46)
            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            oda.SelectCommand.ExecuteNonQuery()

        Finally
            oda.Dispose()
            con.Close()
            con.Dispose()
        End Try
    End Sub

    Protected Sub CopyfiletoInbox(ByVal Fadd As String, ByVal login As String, ByVal pwd As String, ByVal readPath As String, ByVal filenm As String, ByVal cfoldernm As String)
        Dim con As New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
        Try
            Dim URI As String = Fadd
            Dim clsRequest As System.Net.FtpWebRequest = DirectCast(System.Net.WebRequest.Create(URI), System.Net.FtpWebRequest)
            clsRequest.Credentials = New System.Net.NetworkCredential(login, pwd)
            clsRequest.Method = System.Net.WebRequestMethods.Ftp.UploadFile
            ' read in file...
            'Dim file() As Byte = System.IO.File.ReadAllBytes()
            Dim bFile() As Byte = System.IO.File.ReadAllBytes(readPath & filenm)


            ' upload file...
            Dim clsStream As System.IO.Stream = clsRequest.GetRequestStream()

            clsStream.Write(bFile, 0, bFile.Length)
            clsStream.Close()
            clsStream.Dispose()

        Catch ex As Exception
            oda.SelectCommand.CommandText = "INSERT_ERRORLOG"
            oda.SelectCommand.CommandType = CommandType.StoredProcedure
            oda.SelectCommand.Parameters.Clear()
            oda.SelectCommand.Parameters.AddWithValue("@ERRORMSG", ex.ToString())
            oda.SelectCommand.Parameters.AddWithValue("@ALERTNAME", "FTPREPORT")
            oda.SelectCommand.Parameters.AddWithValue("@EID", 46)
            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            oda.SelectCommand.ExecuteNonQuery()

        Finally
            con.Close()
            con.Dispose()
        End Try

    End Sub



    Private Function CreateCSV(ByVal dt As DataTable) As String
        'Dim fname As String = Now.Year & Now.Month & Now.Day & Now.Hour & Now.Minute & Now.Millisecond & ".CSV"
        Dim fname As String = "abc" & ".CSV"
        Dim FPath As String = HostingEnvironment.MapPath("~/MailAttach/")
        If File.Exists(FPath & fname) Then
            File.Delete(fname)
        End If

        Dim sw As StreamWriter = New StreamWriter(FPath & fname, False)

        sw.Flush()
        'First we will write the headers.
        Dim iColCount As Integer = dt.Columns.Count
        For i As Integer = 0 To iColCount - 1
            sw.Write(dt.Columns(i))
            If (i < iColCount - 1) Then
                sw.Write(",")
            End If
        Next
        sw.Write(sw.NewLine)

        ' Now write all the rows.
        Dim dr As DataRow
        For Each dr In dt.Rows
            For i As Integer = 0 To iColCount - 1
                If Not Convert.IsDBNull(dr(i)) Then
                    sw.Write(dr(i).ToString)
                End If
                If (i < iColCount - 1) Then
                    sw.Write(",")
                End If
            Next
            sw.Write(sw.NewLine)
        Next
        sw.Close()

        Return fname

    End Function

End Class
