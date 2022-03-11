Imports System.Data
Imports System.Data.SqlClient
Imports System.IO

Partial Class IPMacRecording
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As New SqlConnection(conStr)
            Dim da As New SqlDataAdapter("", con)
            Dim ds As New DataSet

            Try


                da.SelectCommand.CommandText = "select username ,uid from mmm_mst_user where eid=" & Session("EID") & " and uid in (select distinct uid from MMM_MST_UserLoginLog where eid=" & Session("EID") & ") order by uid"
                da.Fill(ds, "data")
                If ds.Tables("data").Rows.Count > 0 Then
                    ddlField.DataSource = ds.Tables("data")
                    ddlField.DataTextField = "username"
                    ddlField.DataValueField = "Uid"
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


            txtto.Attributes.Add("readonly", "readonly")
            txtValue.Attributes.Add("readonly", "readonly")
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
        Dim ds As New DataSet()
        Dim str As String = String.Empty
        Try
            If ddlField.SelectedItem.Text = "Select All" Then
                str = String.Empty
            Else
                str = " and l.uid= '" & ddlField.SelectedItem.Value & "'"
            End If

            'da.SelectCommand.CommandText = "Select * from mmm_mst_wslog where eid=" & Session("EID") & " and logtime>='" & txtValue.Text.Trim() & "' and logtime<='" & txtto.Text.Trim() & "'  " & str.ToString() & ""
            da.SelectCommand.CommandText = "select (select username from mmm_mst_user where uid=l.uid)[UserName],convert(varchar, l.LoginTime, 120)[LoginTime],convert(varchar, l.LogoutTime, 120)[LogoutTime] from MMM_MST_UserLoginLog l  where l.eid=" & Session("EID") & " and l.LoginTime>='" & txtValue.Text.Trim() & " 00:00:00:000" & "' and l.LogoutTime<='" & txtto.Text.Trim() & " 23:59:59:999" & "'   " & str
            da.SelectCommand.CommandType = CommandType.Text
            da.Fill(ds, "ex")
            If ds.Tables("ex").Rows.Count > 0 Then
                ''code to export in excel
                
                Dim grdtripdata As New GridView()

                grdtripdata.AllowPaging = False
                grdtripdata.DataSource = ds.Tables("ex")
                grdtripdata.DataBind()
                Response.Clear()
                Response.Buffer = True
                Response.AddHeader("content-disposition", "attachment;filename=Recording.xls")
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
