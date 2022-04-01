Imports System.Data
Imports System.Data.SqlClient
Imports System.Net
Imports System.IO
Imports System.Xml
Imports System.Collections.Generic
Imports System.Text
Imports System.Configuration

Partial Class DevSMS
    Inherits System.Web.UI.Page
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
       
    End Sub
    Public Function SMSCMD() As String
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim da As SqlDataAdapter = New SqlDataAdapter("", con)
        Dim ds As New DataSet()
        Dim qry As String = ""
        da.SelectCommand.CommandText = "select qryfield[cmd] from mmm_mst_report where reportid=425"
        ' da.SelectCommand.CommandText = "select qryfieldrole[cmd] from mmm_mst_report where reportid=769"
        da.Fill(ds, "qry")
        qry = ds.Tables("qry").Rows(0).Item(0).ToString()
        da.SelectCommand.CommandText = qry
        da.Fill(ds, "cmd")
        Dim mcnt As String = ""
        Dim ccnt As String = ""
        Dim STR As String = Trim(txtMSG.Text.ToString)
        Dim str1 As String() = STR.Split(",")
        STR = ""

        Try

            For i As Integer = 0 To str1.Length - 1
                Dim MobileNo As String = str1(i)
                For j = 0 To ds.Tables("cmd").Rows.Count - 1
                    Dim Message As String = ds.Tables("cmd").Rows(j).Item("cmd").ToString
                    Dim msgString As String = "http://121.241.247.144:7501/failsafe/HttpLink?aid=633208&pin=intech@1&mnumber=91" & MobileNo & "&message=" & Message
                    Dim result As String = apicall(msgString)
                    ccnt = j
                Next
                mcnt = i
            Next

        Catch ex As Exception
            Dim finalm As String = mcnt
            Dim finalc As String = ccnt
        Finally
            con.Dispose()
        End Try

    End Function


    Public Function apicall(url As String) As String
        Dim httpreq As HttpWebRequest = DirectCast(WebRequest.Create(url), HttpWebRequest)
        Try
            Dim httpres As HttpWebResponse = DirectCast(httpreq.GetResponse(), HttpWebResponse)
            Dim sr As New StreamReader(httpres.GetResponseStream())
            Dim results As String = sr.ReadToEnd()
            sr.Close()
            Return results
        Catch
            Return "0"
        End Try
    End Function

    Protected Sub btnSubmit_Click(sender As Object, e As System.EventArgs) Handles btnSubmit.Click
        SMSCMD()
        'If Not IsPostBack Then
        '    Dim script As String = "$(document).ready(function () { $('[id*=btnSubmit]').click(); });"
        '    ClientScript.RegisterStartupScript(Me.GetType, "load", script, True)
        'End If

    End Sub
End Class
