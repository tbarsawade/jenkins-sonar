Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports System.Web.UI.Adapters.ControlAdapter
Imports iTextSharp.text
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
Partial Class WebServiceLogOutward
    Inherits System.Web.UI.Page


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
    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As New SqlConnection(conStr)
            Dim da As New SqlDataAdapter("", con)
            Dim ds As New DataSet

            Try


                da.SelectCommand.CommandText = "Select * from mmm_mst_forms where eid=" & Session("EID") & " and enablews=1 order by formname"
                da.Fill(ds, "data")
                If ds.Tables("data").Rows.Count > 0 Then
                    ddlField.DataSource = ds.Tables("data")
                    ddlField.DataTextField = "formname"
                    ddlField.DataValueField = "formid"
                    ddlField.DataBind()
                    ddlField.Items.Insert(0, "Select All")
                End If

            Catch ex As Exception
                lblMsg.Text = "Error Occured: Please try after some time!"
            Finally
                If Not con Is Nothing Then
                    con.Close()
                End If
                If Not da Is Nothing Then
                    da.Dispose()
                End If
            End Try

        End If
    End Sub


    Protected Sub btnexport_Click(sender As Object, e As ImageClickEventArgs) Handles btnexport.Click
        If txtValue.Text = "" Or txtValue.Text.Length < 5 Then
            lblMsg.Text = "Please enter valid From date!!"
            Exit Sub
        End If
        If txtto.Text = "" Or txtto.Text.Length < 5 Then
            lblMsg.Text = "Please enter valid to date!!!!"
            Exit Sub
        End If
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim da As New SqlDataAdapter("", con)
        Dim ds As New DataSet
        Dim str As String = String.Empty
        Try

            If ddlField.SelectedItem.Text = "Select All" Then
                str = String.Empty
            Else
                str = "and doctype= '" & ddlField.SelectedItem.Text & "'"
            End If
            If ddlsta.SelectedItem.Text = "Select All" Then
                str = str & String.Empty
            Else
                str = str & " and result='" & ddlsta.SelectedItem.Text & "'"
            End If

            'da.SelectCommand.CommandText = "Select * from mmm_mst_wslog where eid=" & Session("EID") & " and logtime>='" & txtValue.Text.Trim() & "' and logtime<='" & txtto.Text.Trim() & "'  " & str.ToString() & ""
            da.SelectCommand.CommandText = "select docid[Docid],logtime[LogTime],case when errortype ='' then trycatcherror else errortype end [Description],urlstring[URLString],result[Result],doctype[Doc Type],case when source='WS' then 'Tab' else source end[Source],wstype[WebService Type],LastRetry,errortry[Retrial Count] from mmm_mst_wslog w inner join mmm_mst_doc d on w.docid=d.tid where w.eid=" & Session("EID") & " and logtime>='" & txtValue.Text.Trim() & " 00:00:00:000" & "' and logtime<='" & txtto.Text.Trim() & " 23:59:59:999" & "'   " & str.ToString() & ""
            da.Fill(ds, "ex")
            If ds.Tables("ex").Rows.Count > 0 Then
                ''code to export in excel
                Dim grdtripdata As New GridView()
          
                grdtripdata.AllowPaging = False
                grdtripdata.DataSource = ds.Tables("ex")
                grdtripdata.DataBind()
                Response.Clear()
                Response.Buffer = True
                Response.Write("<br/><div align=""center"" style=""border:1px solid red"" ><h3>Web Service Log Outward</h3></div> <br/>")
                Response.AddHeader("content-disposition", "attachment;filename=WebServiceLogOutward.xls")
                Response.Charset = ""
                Response.ContentType = "application/vnd.ms-excel"
                Dim sw As New StringWriter()
                Dim hw As New HtmlTextWriter(sw)
                For i As Integer = 0 To grdtripdata.Rows.Count - 1
                    grdtripdata.Rows(i).Attributes.Add("class", "textmode")
                Next
                grdtripdata.RenderControl(hw)
                'style to format numbers to string 
                '  Dim style As String = "<style> .textmode{mso-number-format:\@;}</style>"

                Response.Output.Write(sw.ToString())
                Response.Flush()
                Response.End()
            End If

        Catch ex As Exception
            lblMsg.Text = "Error Occured: Please try after some time!!"
        Finally
            If Not con Is Nothing Then
                con.Close()
            End If
            If Not da Is Nothing Then
                da.Dispose()
            End If

        End Try


      


    End Sub
End Class
