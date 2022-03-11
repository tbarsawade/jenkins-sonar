Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports System.Net
Imports Newtonsoft.Json
Imports Renci.SshNet
Partial Class IHCLONdemand
    Inherits System.Web.UI.Page
    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            Binduser()
            Binduser1()


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

    Public Sub Binduser()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim da As SqlDataAdapter = New SqlDataAdapter("", con)
        Try
            Dim ds As New DataSet()
            Dim qry As String = ""
            da.SelectCommand.CommandText = "Select TID,OUT_INTGR_FLD3 [ReportSubject] from mmm_mst_reportscheduler where  ftpflag=4 and eid=" & HttpContext.Current.Session("EID") & ""
            da.Fill(ds, "qry")
            ddlReportName.DataSource = ds
            ddlReportName.DataTextField = "ReportSubject"
            ddlReportName.DataValueField = "TID"
            ddlReportName.DataBind()
            ddlReportName.Items.Insert("0", New ListItem("SELECT"))
        Catch
        Finally
            da.Dispose()
            con.Close()
            con.Dispose()
        End Try
    End Sub


    Public Sub Binduser1()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim da As SqlDataAdapter = New SqlDataAdapter("", con)
        Try
            Dim ds As New DataSet()
            Dim qry As String = ""

            ddlSchType.DataBind()
            ddlSchType.Items.Insert("0", New ListItem("-Select-"))
            ddlSchType.Items.Insert("1", New ListItem("On Demand"))
            ddlSchType.Items.Insert("2", New ListItem("Re Run"))
        Catch
        Finally
            da.Dispose()
            con.Close()
            con.Dispose()
        End Try
    End Sub

    Protected Sub ddlReportName_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlReportName.SelectedIndexChanged
        If (ddlReportName.SelectedItem.Text = "SELECT") Then

            btnsearch.Visible = False
            btnsave.Visible = False
            dvkgd.Style("display") = "none"
            dvcal.Style("display") = "none"
            ScriptManager.RegisterClientScriptBlock(Me, Me.GetType(), "alertMessage", "alert('please select Report Name')", True)

        End If

        If (ddlSchType.SelectedItem.Text = "-Select-") Then

            btnsearch.Visible = False
            btnsave.Visible = False
            dvkgd.Style("display") = "none"
            dvcal.Style("display") = "none"
            ScriptManager.RegisterClientScriptBlock(Me, Me.GetType(), "alertMessage", "alert('please select integration Name')", True)

        End If

    End Sub

    Protected Sub ddlSchType_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlSchType.SelectedIndexChanged


        Dim aaa As String = ddlReportName.SelectedItem.Text


        If (ddlReportName.SelectedItem.Text = "SELECT") Then
            btnsearch.Visible = False
            btnsave.Visible = False
            dvkgd.Style("display") = "none"
            dvcal.Style("display") = "none"
            ScriptManager.RegisterClientScriptBlock(Me, Me.GetType(), "alertMessage", "alert('please select report Type')", True)
        End If


        If (ddlSchType.SelectedItem.Text = "On Demand") Then
            btnsearch.Visible = True
            btnsave.Visible = False
            dvkgd.Style("display") = "none"
            dvcal.Style("display") = "none"
            'ScriptManager.RegisterClientScriptBlock(Me, Me.GetType(), "alertMessage", "alert('please select report Type')", True)
        End If


        If (ddlSchType.SelectedItem.Text = "Re Run") Then

            btnsave.Visible = True
            btnsearch.Visible = False
            dvkgd.Style("display") = "block"
            dvcal.Style("display") = "block"

        End If
        If (ddlSchType.SelectedItem.Text = "-Select-") Then

            btnsearch.Visible = False
            btnsave.Visible = False
            dvkgd.Style("display") = "none"
            dvcal.Style("display") = "none"
            ScriptManager.RegisterClientScriptBlock(Me, Me.GetType(), "alertMessage", "alert('please select Integration Type')", True)

        End If

    End Sub

    Protected Sub onReRun(ByVal sender As Object, ByVal e As System.EventArgs)

        If Len(TextBox2.Text.Trim) < 2 Then
            lblError.Text = "Enter Valid Pearl IDs First!"
            Exit Sub
        End If
        lblError.Text = ""
        GridView1.DataSource = Nothing
        GridView1.DataBind()
        IHCLWebServiceRunOnButtonClick.PearlIds = TextBox2.Text.Trim()
        Dim instance As New IHCLWebServiceRunOnButtonClick()
        Dim vvv As String = ddlReportName.SelectedItem.Text
        Dim response As String = instance.EntryPoint(vvv)
        GridView1.DataSource = IHCLWebServiceRunOnButtonClick.ReportQueryResult
        GridView1.DataBind()
        TextBox1.Text = response
        TextBox2.Text = ""



    End Sub

    Protected Sub ondemandexecute(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
        Dim ds As New DataSet()
        Dim eid As String = ""
        Dim Rptname As String = ""
        Dim FTPType As String = ""
        Dim instance As New IHCLWebService()
        Dim vvv As String = ddlReportName.SelectedItem.Text
        Dim response As String = instance.EntryPoint1(vvv)
        GridView1.DataSource = IHCLWebService.ReportQueryResult
        GridView1.DataBind()
        TextBox1.Text = response
    End Sub


    Protected Sub ondemandexecute1(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
        Dim ds As New DataSet()
        Dim eid As String = ""
        Dim Rptname As String = ""
        Dim FTPType As String = ""
        Dim iHCLWebService As New IHCLWebService()
        iHCLWebService.EntryPoint()
        'Dim vvv As String = ddlReportName.SelectedItem.Text
        'Dim response As String = instance.EntryPoint("")
        GridView1.DataSource = IHCLWebService.ReportQueryResult
        GridView1.DataBind()
        'TextBox1.Text = response
    End Sub


End Class

