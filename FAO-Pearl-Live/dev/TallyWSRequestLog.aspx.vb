Imports System.IO
Imports System.Data.SqlClient
Imports System.Data

'Pallavi :

Partial Class TallyWSRequestLog
    Inherits System.Web.UI.Page

    Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
    Dim con As New SqlConnection(conStr)

    Protected Sub Page_PreInit(ByVal sender As Object, ByVal e As EventArgs) Handles Me.PreInit

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
    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            If Session("EID") = 63 Then
                bindPcomany()
                ddlPcompany.Visible = True
                lblPcompany.Visible = True
                FillDDLDocType()
                FillDDLServiceName()
                FillDDLSuperDistributorSSV()
            Else
                ddlPcompany.Visible = False
                lblPcompany.Visible = False
                FillDDLDocType()
                FillDDLServiceName()
                FillDDLSuperDistributor()
            End If

        End If
    End Sub

    Public Overrides Sub VerifyRenderingInServerForm(control As Control)
        ' Verifies that the control is rendered 

    End Sub
    Protected Sub btnexport_Click(sender As Object, e As ImageClickEventArgs) Handles btnexport.Click

        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim da As New SqlDataAdapter("", con)
        Dim ds As New DataSet
        Dim str As String = String.Empty

        Try
           
            Response.Clear()
            Response.Buffer = True
            Response.Write("<br/><div align=""center"" style=""border:1px solid red"" ><h3>Tally WS Request Log</h3></div> <br/>")
            Response.AddHeader("content-disposition", "attachment;filename=TallyWSRequestLog.xls")
            Response.Charset = ""
            Response.ContentType = "application/vnd.ms-excel"
            Dim sw As New StringWriter()
            Dim hw As New HtmlTextWriter(sw)
           
            gvSearch.RenderControl(hw)
            Response.Output.Write(sw.ToString())
            Response.Flush()
            Response.End()

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


    Sub FillDDLDocType()

        Dim da As New SqlDataAdapter("", con)
        Dim ds As New DataSet
        Try
            da.SelectCommand.CommandText = "Select distinct BPMDoctype from mmm_mst_TallyWSRequestLog where eid=" & Session("EID") & " order by BPMDoctype "
            da.Fill(ds, "data")

            If ds.Tables("data").Rows.Count > 0 Then
                ddlDocType.DataSource = ds.Tables("data")
                ddlDocType.DataTextField = "BPMDoctype"
                ddlDocType.DataValueField = "BPMDoctype"
                ddlDocType.DataBind()

            End If

            ddlDocType.Items.Insert(0, "Select All")
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

    End Sub

    Sub FillDDLServiceName()

        Dim da As New SqlDataAdapter("", con)
        Dim ds As New DataSet
        Try
            da.SelectCommand.CommandText = "Select distinct ServiceName from mmm_mst_TallyWSRequestLog where eid=" & Session("EID") & " order by ServiceName "
            da.Fill(ds, "data")

            If ds.Tables("data").Rows.Count > 0 Then

                ddlServiceNm.DataSource = ds.Tables("data")
                ddlServiceNm.DataTextField = "ServiceName"
                ddlServiceNm.DataValueField = "ServiceName"
                ddlServiceNm.DataBind()

            End If
            ddlServiceNm.Items.Insert(0, "Select All")
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

    End Sub

    Sub FillDDLSuperDistributor()

        Dim da As New SqlDataAdapter("", con)
        Dim ds As New DataSet

        Try
            da.SelectCommand.CommandText = "select fld1,BPMTID from mmm_mst_master m inner join mmm_mst_TallyRegInfo TI on m.tid =TI.BPMTID  where ti.eid =" & Session("EID") & " order by fld1"
            da.Fill(ds, "data")

            If ds.Tables("data").Rows.Count > 0 Then

                ddlSuperDistrbtr.DataSource = ds.Tables("data")
                ddlSuperDistrbtr.DataTextField = "fld1"
                ddlSuperDistrbtr.DataValueField = "BPMTID"
                ddlSuperDistrbtr.DataBind()
            End If

            ddlSuperDistrbtr.Items.Insert(0, "Select All")
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


    End Sub


    Protected Sub ddlDocType_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlDocType.SelectedIndexChanged

    End Sub


    Protected Sub ImgBtnSearch_Click(sender As Object, e As ImageClickEventArgs) Handles ImgBtnSearch.Click
        If (txtFrmDt.Text = "" And txtToDt.Text <> "") Or (txtFrmDt.Text <> "" And txtToDt.Text = "") Then
            lblMsg.Text = "Please enter both the dates !!"
        End If

        If txtFrmDt.Text <> "" And txtFrmDt.Text.Length < 5 Then
            lblMsg.Text = "Please enter valid 'From date'!!"
            Exit Sub
        End If
        If txtToDt.Text <> "" And txtToDt.Text.Length < 5 Then
            lblMsg.Text = "Please enter valid 'To date'!!!!"
            Exit Sub
        End If
        Dim strQueryCond As String = ""
        Dim da As New SqlDataAdapter("", con)
        Dim ds As New DataSet
        Try
            If (ddlDocType.SelectedIndex > 0) Then
                strQueryCond &= " and BPMRegDoctype = '" & ddlDocType.SelectedValue & "'"
            End If

            If (ddlSuperDistrbtr.SelectedIndex > 0) Then
                strQueryCond &= " and [BPMRegID] ='" & ddlSuperDistrbtr.SelectedValue & "'"
            End If

            If (ddlServiceNm.SelectedIndex > 0) Then
                strQueryCond &= " and ServiceName='" & ddlServiceNm.SelectedValue & "'"
            End If

            If (txtFrmDt.Text <> "" And txtToDt.Text <> "") Then
                strQueryCond &= " and convert(date, CallDate) between convert(datetime,'" & txtFrmDt.Text & "') and convert(datetime,'" & txtToDt.Text & "')"
            End If

            da.SelectCommand.CommandText = "SELECT m.fld1 [Distributor Name]  , tl.BPMDocType [BPM DocType],tl.ServiceName[Service Name]," &
 " tl.RecordsProcessed [Record Processed] ,convert(varchar(50), tl.CallDate,100) [Synchronize Date], tl.Result FROM mmm_mst_TallyWSRequestLog tl inner join MMM_MST_MASTER m on  tl.BPMRegID = m.tid " &
                     " where tl.eid =" & Session("EID") & strQueryCond
            If Session("EID") = 63 And ddlSuperDistrbtr.SelectedItem.Text = "Select All" Then
                da.SelectCommand.CommandText = "SELECT m.fld1 [Distributor Name]  , tl.BPMDocType [BPM DocType],tl.ServiceName[Service Name]," &
  " tl.RecordsProcessed [Record Processed] ,convert(varchar(50), tl.CallDate,100) [Synchronize Date], tl.Result FROM mmm_mst_TallyWSRequestLog tl inner join MMM_MST_MASTER m on  tl.BPMRegID = m.tid " &
                      " where  dms.udf_split('MASTER-PCompany-fld1',m.fld10)='" & ddlPcompany.SelectedItem.Text & "' and tl.eid =" & Session("EID") & strQueryCond

            End If
             
            da.Fill(ds, "data")

            If ds.Tables("data").Rows.Count > 0 Then
                gvSearch.DataSource = ds.Tables("data")
                gvSearch.DataBind()
                btnexport.Visible = True
                lblMsgupdate.Text = ""

            Else : gvSearch.DataSource = Nothing
                gvSearch.DataBind()
                btnexport.Visible = False
                lblMsgupdate.Text = "No record found"

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


    End Sub

    Protected Sub ddlPcompany_SelectedIndexChanged(sender As Object, e As System.EventArgs) Handles ddlPcompany.SelectedIndexChanged
        FillDDLSuperDistributorSSV()
    End Sub

    Sub bindPcomany()

        Dim da As New SqlDataAdapter("", con)
        Dim ds As New DataSet

        Try
            da.SelectCommand.CommandText = "select fld1 from mmm_mst_master where documenttype='PCompany' and eid =" & Session("EID") & "  order by fld1"
            da.Fill(ds, "data")

            If ds.Tables("data").Rows.Count > 0 Then
                ddlPcompany.DataSource = ds.Tables("data")
                ddlPcompany.DataTextField = "fld1"
                ddlPcompany.DataValueField = "fld1"
                ddlPcompany.DataBind()
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


    End Sub


    Sub FillDDLSuperDistributorSSV()

        Dim da As New SqlDataAdapter("", con)
        Dim ds As New DataSet

        Try
            da.SelectCommand.CommandText = "select fld1,tid from mmm_mst_master where documenttype='Super Distributor' and dms.udf_split('MASTER-PCompany-fld1',fld10)='" & ddlPcompany.SelectedItem.Text & "' and eid =" & Session("EID") & ""
            da.Fill(ds, "data")

            If ds.Tables("data").Rows.Count > 0 Then
                ddlSuperDistrbtr.DataSource = ds.Tables("data")
                ddlSuperDistrbtr.DataTextField = "fld1"
                ddlSuperDistrbtr.DataValueField = "tid"
                ddlSuperDistrbtr.DataBind()
            End If

            ddlSuperDistrbtr.Items.Insert(0, "Select All")
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


    End Sub
End Class
