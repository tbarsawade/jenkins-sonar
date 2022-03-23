Imports System.Data
Imports System.Data.SqlClient
Imports System.Configuration
Imports System.Globalization
Imports System.IO
Imports System.Collections

Partial Class RoutPlanSettings
    Inherits System.Web.UI.Page
    Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
    Dim con As SqlConnection = New SqlConnection(conStr)

    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            Bindddl()
            FillSetting()
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
        Dim da As New SqlDataAdapter("Select FormName[Text], FormName[Value], IsRoleDef from mmm_mst_forms where Eid=" & Session("Eid"), con)
        Dim dt As New DataTable()
        da.Fill(dt)
        ddlSiteDoc.DataValueField = "Value"
        ddlSiteDoc.DataTextField = "Text"
        ddlSiteDoc.DataSource = dt
        ddlSiteDoc.DataBind()
        ddlSiteDoc.Items.Insert(0, "Select")
        ddlSiteDoc.Items(0).Value = "0"
        


        ddlVehicleDoc.DataValueField = "Value"
        ddlVehicleDoc.DataTextField = "Text"
        ddlVehicleDoc.DataSource = dt
        ddlVehicleDoc.DataBind()
        ddlVehicleDoc.Items.Insert(0, "Select")
        ddlVehicleDoc.Items(0).Value = "0"
        
        ddlRoutPlan.DataValueField = "Value"
        ddlRoutPlan.DataTextField = "Text"
        ddlRoutPlan.DataSource = dt
        ddlRoutPlan.DataBind()
        ddlRoutPlan.Items.Insert(0, "Select")
        ddlRoutPlan.Items(0).Value = "0"

        ddlPlanMatrixDoc.DataValueField = "Value"
        ddlPlanMatrixDoc.DataTextField = "Text"
        ddlPlanMatrixDoc.DataSource = dt
        ddlPlanMatrixDoc.DataBind()
        ddlPlanMatrixDoc.Items.Insert(0, "Select")
        ddlPlanMatrixDoc.Items(0).Value = "0"

        Dim drs() As DataRow = dt.Select("IsRoleDef='1'")
        Dim dtRoleDef As New DataTable()

        dtRoleDef.Columns.Add("Text")
        dtRoleDef.Columns.Add("Value")
        dtRoleDef.Columns.Add("IsRoleDef")

        For Each dr As DataRow In drs
            dtRoleDef.ImportRow(dr)
        Next

        ddlRoleDef.DataValueField = "Value"
        ddlRoleDef.DataTextField = "Text"
        ddlRoleDef.DataSource = dtRoleDef
        ddlRoleDef.DataBind()
        ddlRoleDef.Items.Insert(0, "Select")
        ddlRoleDef.Items(0).Value = "0"

    End Sub

    Protected Sub ddl_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlSiteDoc.SelectedIndexChanged
        Dim ddl As DropDownList = DirectCast(sender, DropDownList)
        Dim da As New SqlDataAdapter("Select FieldMapping[Value], DisplayName[Text] from mmm_mst_fields where Eid=" & Session("Eid") & " and documenttype='" & ddl.SelectedItem.Text & "' and IsActive=1", con)
        Dim dt As New DataTable()
        da.Fill(dt)

        ddlSiteID.DataValueField = "Value"
        ddlSiteID.DataTextField = "Text"
        ddlSiteID.DataSource = dt
        ddlSiteID.DataBind()
        ddlSiteID.Items.Insert(0, "Select")
        ddlSiteID.Items(0).Value = "0"

        ddlSiteFencefld.DataValueField = "Value"
        ddlSiteFencefld.DataTextField = "Text"
        ddlSiteFencefld.DataSource = dt
        ddlSiteFencefld.DataBind()
        ddlSiteFencefld.Items.Insert(0, "Select")
        ddlSiteFencefld.Items(0).Value = "0"

        ddlSiteNamefld.DataValueField = "Value"
        ddlSiteNamefld.DataTextField = "Text"
        ddlSiteNamefld.DataSource = dt
        ddlSiteNamefld.DataBind()
        ddlSiteNamefld.Items.Insert(0, "Select")
        ddlSiteNamefld.Items(0).Value = "0"

    End Sub

    Protected Sub ddlVehicleDoc_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlVehicleDoc.SelectedIndexChanged
        Dim ddl As DropDownList = DirectCast(sender, DropDownList)
        Dim da As New SqlDataAdapter("Select FieldMapping[Value], DisplayName[Text] from mmm_mst_fields where Eid=" & Session("Eid") & " and documenttype='" & ddl.SelectedItem.Text & "' and IsActive=1", con)
        Dim dt As New DataTable()
        da.Fill(dt)

        ddlVehicleNo.DataValueField = "Value"
        ddlVehicleNo.DataTextField = "Text"
        ddlVehicleNo.DataSource = dt
        ddlVehicleNo.DataBind()
        ddlVehicleNo.Items.Insert(0, "Select")
        ddlVehicleNo.Items(0).Value = "0"

        ddlVehicleType.DataValueField = "Value"
        ddlVehicleType.DataTextField = "Text"
        ddlVehicleType.DataSource = dt
        ddlVehicleType.DataBind()
        ddlVehicleType.Items.Insert(0, "Select")
        ddlVehicleType.Items(0).Value = "0"

        ddlIMEIfld.DataValueField = "Value"
        ddlIMEIfld.DataTextField = "Text"
        ddlIMEIfld.DataSource = dt
        ddlIMEIfld.DataBind()
        ddlIMEIfld.Items.Insert(0, "Select")
        ddlIMEIfld.Items(0).Value = "0"

        ddlVehicleMapping.DataValueField = "Value"
        ddlVehicleMapping.DataTextField = "Text"
        ddlVehicleMapping.DataSource = dt
        ddlVehicleMapping.DataBind()
        ddlVehicleMapping.Items.Insert(0, "Select")
        ddlVehicleMapping.Items(0).Value = "0"

    End Sub

    Protected Sub ddlRoutPlan_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlRoutPlan.SelectedIndexChanged
        Dim ddl As DropDownList = DirectCast(sender, DropDownList)
        Dim da As New SqlDataAdapter("Select FieldMapping[Value], DisplayName[Text] from mmm_mst_fields where Eid=" & Session("Eid") & " and documenttype='" & ddl.SelectedItem.Text & "' and IsActive=1", con)
        Dim dt As New DataTable()
        da.Fill(dt)

        ddlPlanVehicleNo.DataValueField = "Value"
        ddlPlanVehicleNo.DataTextField = "Text"
        ddlPlanVehicleNo.DataSource = dt
        ddlPlanVehicleNo.DataBind()
        ddlPlanVehicleNo.Items.Insert(0, "Select")
        ddlPlanVehicleNo.Items(0).Value = "0"

        ddlPlanVehicleType.DataValueField = "Value"
        ddlPlanVehicleType.DataTextField = "Text"
        ddlPlanVehicleType.DataSource = dt
        ddlPlanVehicleType.DataBind()
        ddlPlanVehicleType.Items.Insert(0, "Select")
        ddlPlanVehicleType.Items(0).Value = "0"

    End Sub

    Protected Sub ddlPlanMatrixDoc_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlPlanMatrixDoc.SelectedIndexChanged

        Dim ddl As DropDownList = DirectCast(sender, DropDownList)
        Dim da As New SqlDataAdapter("Select FieldMapping[Value], DisplayName[Text] from mmm_mst_fields where Eid=" & Session("Eid") & " and documenttype='" & ddl.SelectedItem.Text & "' and IsActive=1", con)
        Dim dt As New DataTable()
        da.Fill(dt)

        ddlDestinationfld.DataValueField = "Value"
        ddlDestinationfld.DataTextField = "Text"
        ddlDestinationfld.DataSource = dt
        ddlDestinationfld.DataBind()
        ddlDestinationfld.Items.Insert(0, "Select")
        ddlDestinationfld.Items(0).Value = "0"

        ddlDatefld.DataValueField = "Value"
        ddlDatefld.DataTextField = "Text"
        ddlDatefld.DataSource = dt
        ddlDatefld.DataBind()
        ddlDatefld.Items.Insert(0, "Select")
        ddlDatefld.Items(0).Value = "0"

        ddlTimefld.DataValueField = "Value"
        ddlTimefld.DataTextField = "Text"
        ddlTimefld.DataSource = dt
        ddlTimefld.DataBind()
        ddlTimefld.Items.Insert(0, "Select")
        ddlTimefld.Items(0).Value = "0"

        ddlHault.DataValueField = "Value"
        ddlHault.DataTextField = "Text"
        ddlHault.DataSource = dt
        ddlHault.DataBind()
        ddlHault.Items.Insert(0, "Select")
        ddlHault.Items(0).Value = "0"

    End Sub

    Private Sub FillSetting()
        Try
            Dim da As New SqlDataAdapter("Select * from mmm_mst_RoutePlanSettings where Eid=" & Session("Eid"), con)
            Dim dt As New DataTable()
            da.Fill(dt)
            If dt.Rows.Count > 0 Then
                btnSvae.Visible = False
                btnUpdate.Visible = True
                ViewState("Tid") = dt.Rows(0).Item("Tid")
            Else
                btnSvae.Visible = True
                btnUpdate.Visible = False
            End If

            If Not IsNothing(ddlSiteDoc.Items.FindByText(dt.Rows(0).Item("SiteDoc").ToString)) Then
                ddlSiteDoc.ClearSelection()
                ddlSiteDoc.Items.FindByText(dt.Rows(0).Item("SiteDoc").ToString).Selected = True
                ddl_SelectedIndexChanged(ddlSiteDoc, Nothing)
            End If

            If Not IsNothing(ddlVehicleDoc.Items.FindByText(dt.Rows(0).Item("VehicleDoc").ToString)) Then
                ddlVehicleDoc.ClearSelection()
                ddlVehicleDoc.Items.FindByText(dt.Rows(0).Item("VehicleDoc").ToString).Selected = True
                ddlVehicleDoc_SelectedIndexChanged(ddlVehicleDoc, Nothing)
            End If

            If Not IsNothing(ddlRoutPlan.Items.FindByText(dt.Rows(0).Item("RoutePlanDoc").ToString)) Then
                ddlRoutPlan.ClearSelection()
                ddlRoutPlan.Items.FindByText(dt.Rows(0).Item("RoutePlanDoc").ToString).Selected = True
                ddlRoutPlan_SelectedIndexChanged(ddlRoutPlan, Nothing)
            End If

            If Not IsNothing(ddlPlanMatrixDoc.Items.FindByText(dt.Rows(0).Item("PlanMatrixDoc").ToString)) Then
                ddlPlanMatrixDoc.ClearSelection()
                ddlPlanMatrixDoc.Items.FindByText(dt.Rows(0).Item("PlanMatrixDoc").ToString).Selected = True
                ddlPlanMatrixDoc_SelectedIndexChanged(ddlPlanMatrixDoc, Nothing)
            End If

            If Not IsNothing(ddlSiteID.Items.FindByValue(dt.Rows(0).Item("SiteIDfld").ToString)) Then
                ddlSiteID.ClearSelection()
                ddlSiteID.Items.FindByValue(dt.Rows(0).Item("SiteIDfld").ToString).Selected = True
            End If

            If Not IsNothing(ddlSiteNamefld.Items.FindByValue(dt.Rows(0).Item("SiteNamefld").ToString)) Then
                ddlSiteNamefld.ClearSelection()
                ddlSiteNamefld.Items.FindByValue(dt.Rows(0).Item("SiteNamefld").ToString).Selected = True
            End If

            If Not IsNothing(ddlSiteFencefld.Items.FindByValue(dt.Rows(0).Item("SiteLatLongfld").ToString)) Then
                ddlSiteFencefld.ClearSelection()
                ddlSiteFencefld.Items.FindByValue(dt.Rows(0).Item("SiteLatLongfld").ToString).Selected = True
            End If

            If Not IsNothing(ddlVehicleNo.Items.FindByValue(dt.Rows(0).Item("VehicleNofld").ToString)) Then
                ddlVehicleNo.ClearSelection()
                ddlVehicleNo.Items.FindByValue(dt.Rows(0).Item("VehicleNofld").ToString).Selected = True
            End If

            If Not IsNothing(ddlIMEIfld.Items.FindByValue(dt.Rows(0).Item("VehicleIMEIfld").ToString)) Then
                ddlIMEIfld.ClearSelection()
                ddlIMEIfld.Items.FindByValue(dt.Rows(0).Item("VehicleIMEIfld").ToString).Selected = True
            End If

            If Not IsNothing(ddlVehicleType.Items.FindByValue(dt.Rows(0).Item("VehicleTypefld").ToString)) Then
                ddlVehicleType.ClearSelection()
                ddlVehicleType.Items.FindByValue(dt.Rows(0).Item("VehicleTypefld").ToString).Selected = True
            End If

            If Not IsNothing(ddlPlanVehicleNo.Items.FindByValue(dt.Rows(0).Item("PlanVehicleNofld").ToString)) Then
                ddlPlanVehicleNo.ClearSelection()
                ddlPlanVehicleNo.Items.FindByValue(dt.Rows(0).Item("PlanVehicleNofld").ToString).Selected = True
            End If

            If Not IsNothing(ddlPlanVehicleType.Items.FindByValue(dt.Rows(0).Item("PlanVehicleTypefld").ToString)) Then
                ddlPlanVehicleType.ClearSelection()
                ddlPlanVehicleType.Items.FindByValue(dt.Rows(0).Item("PlanVehicleTypefld").ToString).Selected = True
            End If

            If Not IsNothing(ddlDestinationfld.Items.FindByValue(dt.Rows(0).Item("Destinationfld").ToString)) Then
                ddlDestinationfld.ClearSelection()
                ddlDestinationfld.Items.FindByValue(dt.Rows(0).Item("Destinationfld").ToString).Selected = True
            End If

            If Not IsNothing(ddlDatefld.Items.FindByValue(dt.Rows(0).Item("Datefld").ToString)) Then
                ddlDatefld.ClearSelection()
                ddlDatefld.Items.FindByValue(dt.Rows(0).Item("Datefld").ToString).Selected = True
            End If

            If Not IsNothing(ddlTimefld.Items.FindByValue(dt.Rows(0).Item("Timefld").ToString)) Then
                ddlTimefld.ClearSelection()
                ddlTimefld.Items.FindByValue(dt.Rows(0).Item("Timefld").ToString).Selected = True
            End If

            If Not IsNothing(ddlRoleDef.Items.FindByText(dt.Rows(0).Item("RoleDefDoc").ToString)) Then
                ddlRoleDef.ClearSelection()
                ddlRoleDef.Items.FindByText(dt.Rows(0).Item("RoleDefDoc").ToString).Selected = True
            End If
            If Not IsNothing(ddlVehicleMapping.Items.FindByValue(dt.Rows(0).Item("VehicleRoleDefDocMap").ToString)) Then
                ddlVehicleMapping.ClearSelection()
                ddlVehicleMapping.Items.FindByValue(dt.Rows(0).Item("VehicleRoleDefDocMap").ToString).Selected = True
            End If

            If Not IsNothing(ddlHault.Items.FindByValue(dt.Rows(0).Item("HaultDurationFld").ToString)) Then
                ddlHault.ClearSelection()
                ddlHault.Items.FindByValue(dt.Rows(0).Item("HaultDurationFld").ToString).Selected = True
            End If

            If Convert.ToBoolean(dt.Rows(0).Item("IsActive")) Then
                chkActive.Checked = True
            Else
                chkActive.Checked = False
            End If

        Catch ex As Exception

        End Try
    End Sub

    Protected Sub btnSvae_Click(sender As Object, e As EventArgs) Handles btnSvae.Click
        Dim msg = ""
        Dim str = "Please fill out all required fields in order to continue."
        Dim values = ""

        Try

            msg = IIf(ddlSiteDoc.SelectedValue = "0", str, "")
            If Not msg = "" Then
                lblMsg.Text = msg
                lblMsg.ForeColor = Drawing.Color.Red
                Exit Sub
            Else
                values &= "'" & ddlSiteDoc.SelectedValue & "',"
            End If

            msg = IIf(ddlSiteID.SelectedValue = "0", str, "")
            If Not msg = "" Then
                lblMsg.Text = msg
                lblMsg.ForeColor = Drawing.Color.Red
                Exit Sub
            Else
                values &= "'" & ddlSiteID.SelectedValue & "',"
            End If

            msg = IIf(ddlSiteNamefld.SelectedValue = "0", str, "")
            If Not msg = "" Then
                lblMsg.Text = msg
                lblMsg.ForeColor = Drawing.Color.Red
                Exit Sub
            Else
                values &= "'" & ddlSiteNamefld.SelectedValue & "',"
            End If
            msg = IIf(ddlSiteFencefld.SelectedValue = "0", str, "")
            If Not msg = "" Then
                lblMsg.Text = msg
                lblMsg.ForeColor = Drawing.Color.Red
                Exit Sub
            Else
                values &= "'" & ddlSiteFencefld.SelectedValue & "',"
            End If
            
            msg = IIf(ddlVehicleDoc.SelectedValue = "0", str, "")
            If Not msg = "" Then
                lblMsg.Text = msg
                lblMsg.ForeColor = Drawing.Color.Red
                Exit Sub
            Else
                values &= "'" & ddlVehicleDoc.SelectedValue & "',"
            End If
            msg = IIf(ddlVehicleNo.SelectedValue = "0", str, "")
            If Not msg = "" Then
                lblMsg.Text = msg
                lblMsg.ForeColor = Drawing.Color.Red
                Exit Sub
            Else
                values &= "'" & ddlVehicleNo.SelectedValue & "',"
            End If

            msg = IIf(ddlIMEIfld.SelectedValue = "0", str, "")
            If Not msg = "" Then
                lblMsg.Text = msg
                lblMsg.ForeColor = Drawing.Color.Red
                Exit Sub
            Else
                values &= "'" & ddlIMEIfld.SelectedValue & "',"
            End If
            msg = IIf(ddlVehicleType.SelectedValue = "0", str, "")
            If Not msg = "" Then
                lblMsg.Text = msg
                lblMsg.ForeColor = Drawing.Color.Red
                Exit Sub
            Else
                values &= "'" & ddlVehicleType.SelectedValue & "',"
            End If
            msg = IIf(ddlRoutPlan.SelectedValue = "0", str, "")
            If Not msg = "" Then
                lblMsg.Text = msg
                lblMsg.ForeColor = Drawing.Color.Red
                Exit Sub
            Else
                values &= "'" & ddlRoutPlan.SelectedValue & "',"
            End If
            msg = IIf(ddlPlanVehicleNo.SelectedValue = "0", str, "")
            If Not msg = "" Then
                lblMsg.Text = msg
                lblMsg.ForeColor = Drawing.Color.Red
                Exit Sub
            Else
                values &= "'" & ddlPlanVehicleNo.SelectedValue & "',"
            End If
            msg = IIf(ddlPlanVehicleType.SelectedValue = "0", str, "")
            If Not msg = "" Then
                lblMsg.Text = msg
                lblMsg.ForeColor = Drawing.Color.Red
                Exit Sub
            Else
                values &= "'" & ddlPlanVehicleType.SelectedValue & "',"
            End If
            msg = IIf(ddlPlanMatrixDoc.SelectedValue = "0", str, "")
            If Not msg = "" Then
                lblMsg.Text = msg
                lblMsg.ForeColor = Drawing.Color.Red
                Exit Sub
            Else
                values &= "'" & ddlPlanMatrixDoc.SelectedValue & "',"
            End If

            msg = IIf(ddlDestinationfld.SelectedValue = "0", str, "")
            If Not msg = "" Then
                lblMsg.Text = msg
                lblMsg.ForeColor = Drawing.Color.Red
                Exit Sub
            Else
                values &= "'" & ddlDestinationfld.SelectedValue & "', "
            End If

            msg = IIf(ddlDatefld.SelectedValue = "0", str, "")
            If Not msg = "" Then
                lblMsg.Text = msg
                lblMsg.ForeColor = Drawing.Color.Red
                Exit Sub
            Else
                values &= "'" & ddlDatefld.SelectedValue & "',"
            End If
            msg = IIf(ddlTimefld.SelectedValue = "0", str, "")
            If Not msg = "" Then
                lblMsg.Text = msg
                lblMsg.ForeColor = Drawing.Color.Red
                Exit Sub
            Else
                values &= "'" & ddlTimefld.SelectedValue & "',"
            End If

            msg = IIf(ddlRoleDef.SelectedValue = "0", str, "")
            If Not msg = "" Then
                lblMsg.Text = msg
                lblMsg.ForeColor = Drawing.Color.Red
                Exit Sub
            Else
                values &= "'" & ddlRoleDef.SelectedItem.Text & "',"
            End If

            msg = IIf(ddlVehicleMapping.SelectedValue = "0", str, "")
            If Not msg = "" Then
                lblMsg.Text = msg
                lblMsg.ForeColor = Drawing.Color.Red
                Exit Sub
            Else
                values &= "'" & ddlVehicleMapping.SelectedValue & "',"
            End If
            
            values &= IIf(chkActive.Checked, "1", "0")

            msg = IIf(ddlHault.SelectedValue = "0", str, "")
            If Not msg = "" Then
                lblMsg.Text = msg
                lblMsg.ForeColor = Drawing.Color.Red
                Exit Sub
            Else
                values &= "'" & ddlHault.SelectedValue & "',"
            End If

            Dim strQuery = "Insert into mmm_mst_RoutePlanSettings(Eid,SiteDoc,SiteIDfld,SiteNamefld,SiteLatLongfld,VehicleDoc,VehicleNofld,VehicleIMEIfld,VehicleTypefld,RoutePlanDoc,PlanVehicleNofld,PlanVehicleTypefld,PlanMatrixDoc,Destinationfld,Datefld,Timefld,RoleDefDoc,VehicleRoleDefDocMap,IsActive,HaultDurationFld) "
            strQuery &= " Values(" & Session("Eid") & ", " & values & ")"
            Dim cmd As New SqlCommand(strQuery, con)
            con.Open()
            Dim rowAff As Integer = cmd.ExecuteNonQuery()
            con.Close()
            If rowAff > 0 Then
                lblMsg.Text = "Data saved successfully."
                lblMsg.ForeColor = Drawing.Color.Green
                FillSetting()
            End If
        Catch ex As Exception
            lblMsg.Text = "Error occured at server. Please contact your system administrator."
            lblMsg.ForeColor = Drawing.Color.Red
        End Try
    End Sub

    Protected Sub btnUpdate_Click(sender As Object, e As EventArgs) Handles btnUpdate.Click
        Dim msg = ""
        Dim str = "Please fill out all required fields in order to continue."
        Dim values = ""

        Try
            msg = IIf(ddlSiteDoc.SelectedValue = "0", str, "")
            If Not msg = "" Then
                lblMsg.Text = msg
                lblMsg.ForeColor = Drawing.Color.Red
                Exit Sub
            Else
                values &= " SiteDoc='" & ddlSiteDoc.SelectedValue & "',"
            End If
            msg = IIf(ddlSiteNamefld.SelectedValue = "0", str, "")
            If Not msg = "" Then
                lblMsg.Text = msg
                lblMsg.ForeColor = Drawing.Color.Red
                Exit Sub
            Else
                values &= "SiteNamefld='" & ddlSiteNamefld.SelectedValue & "',"
            End If
            msg = IIf(ddlSiteFencefld.SelectedValue = "0", str, "")
            If Not msg = "" Then
                lblMsg.Text = msg
                lblMsg.ForeColor = Drawing.Color.Red
                Exit Sub
            Else
                values &= "SiteLatLongfld='" & ddlSiteFencefld.SelectedValue & "',"
            End If
            msg = IIf(ddlSiteID.SelectedValue = "0", str, "")
            If Not msg = "" Then
                lblMsg.Text = msg
                lblMsg.ForeColor = Drawing.Color.Red
                Exit Sub
            Else
                values &= "SiteIDfld='" & ddlSiteID.SelectedValue & "',"
            End If
            msg = IIf(ddlVehicleDoc.SelectedValue = "0", str, "")
            If Not msg = "" Then
                lblMsg.Text = msg
                lblMsg.ForeColor = Drawing.Color.Red
                Exit Sub
            Else
                values &= "VehicleDoc='" & ddlVehicleDoc.SelectedValue & "',"
            End If
            msg = IIf(ddlVehicleNo.SelectedValue = "0", str, "")
            If Not msg = "" Then
                lblMsg.Text = msg
                lblMsg.ForeColor = Drawing.Color.Red
                Exit Sub
            Else
                values &= "VehicleNofld='" & ddlVehicleNo.SelectedValue & "',"
            End If

            msg = IIf(ddlIMEIfld.SelectedValue = "0", str, "")
            If Not msg = "" Then
                lblMsg.Text = msg
                lblMsg.ForeColor = Drawing.Color.Red
                Exit Sub
            Else
                values &= "VehicleIMEIfld='" & ddlIMEIfld.SelectedValue & "',"
            End If
            msg = IIf(ddlVehicleType.SelectedValue = "0", str, "")
            If Not msg = "" Then
                lblMsg.Text = msg
                lblMsg.ForeColor = Drawing.Color.Red
                Exit Sub
            Else
                values &= "VehicleTypefld='" & ddlVehicleType.SelectedValue & "',"
            End If
            msg = IIf(ddlRoutPlan.SelectedValue = "0", str, "")
            If Not msg = "" Then
                lblMsg.Text = msg
                lblMsg.ForeColor = Drawing.Color.Red
                Exit Sub
            Else
                values &= "RoutePlanDoc='" & ddlRoutPlan.SelectedValue & "',"
            End If
            msg = IIf(ddlPlanVehicleNo.SelectedValue = "0", str, "")
            If Not msg = "" Then
                lblMsg.Text = msg
                lblMsg.ForeColor = Drawing.Color.Red
                Exit Sub
            Else
                values &= "PlanVehicleNofld='" & ddlPlanVehicleNo.SelectedValue & "',"
            End If
            msg = IIf(ddlPlanVehicleType.SelectedValue = "0", str, "")
            If Not msg = "" Then
                lblMsg.Text = msg
                lblMsg.ForeColor = Drawing.Color.Red
                Exit Sub
            Else
                values &= "PlanVehicleTypefld='" & ddlPlanVehicleType.SelectedValue & "',"
            End If
            msg = IIf(ddlPlanMatrixDoc.SelectedValue = "0", str, "")
            If Not msg = "" Then
                lblMsg.Text = msg
                lblMsg.ForeColor = Drawing.Color.Red
                Exit Sub
            Else
                values &= "PlanMatrixDoc='" & ddlPlanMatrixDoc.SelectedValue & "',"
            End If
            msg = IIf(ddlDatefld.SelectedValue = "0", str, "")
            If Not msg = "" Then
                lblMsg.Text = msg
                lblMsg.ForeColor = Drawing.Color.Red
                Exit Sub
            Else
                values &= "Datefld='" & ddlDatefld.SelectedValue & "',"
            End If
            msg = IIf(ddlTimefld.SelectedValue = "0", str, "")
            If Not msg = "" Then
                lblMsg.Text = msg
                lblMsg.ForeColor = Drawing.Color.Red
                Exit Sub
            Else
                values &= "Timefld='" & ddlTimefld.SelectedValue & "',"
            End If
            msg = IIf(ddlDestinationfld.SelectedValue = "0", str, "")
            If Not msg = "" Then
                lblMsg.Text = msg
                lblMsg.ForeColor = Drawing.Color.Red
                Exit Sub
            Else
                values &= "Destinationfld='" & ddlDestinationfld.SelectedValue & "', "
            End If


            msg = IIf(ddlRoleDef.SelectedValue = "0", str, "")
            If Not msg = "" Then
                lblMsg.Text = msg
                lblMsg.ForeColor = Drawing.Color.Red
                Exit Sub
            Else
                values &= "RoleDefDoc='" & ddlRoleDef.SelectedItem.Text & "', "
            End If

            msg = IIf(ddlVehicleMapping.SelectedValue = "0", str, "")
            If Not msg = "" Then
                lblMsg.Text = msg
                lblMsg.ForeColor = Drawing.Color.Red
                Exit Sub
            Else
                values &= "VehicleRoleDefDocMap='" & ddlVehicleMapping.SelectedValue & "', "
            End If

            msg = IIf(ddlHault.SelectedValue = "0", str, "")
            If Not msg = "" Then
                lblMsg.Text = msg
                lblMsg.ForeColor = Drawing.Color.Red
                Exit Sub
            Else
                values &= "HaultDurationFld='" & ddlHault.SelectedValue & "', "
            End If

            values &= " IsActive=" & IIf(chkActive.Checked, "1", "0")

            Dim strQuery = "update mmm_mst_RoutePlanSettings set " & values & " where Eid=" & Session("Eid") & " and Tid=" & ViewState("Tid").ToString()

            Dim cmd As New SqlCommand(strQuery, con)
            con.Open()
            Dim rowAff As Integer = cmd.ExecuteNonQuery()
            con.Close()
            If rowAff > 0 Then
                lblMsg.Text = "Data updated successfully."
                lblMsg.ForeColor = Drawing.Color.Green
            End If
        Catch ex As Exception
            lblMsg.Text = "Error occured at server. Please contact your system administrator."
            lblMsg.ForeColor = Drawing.Color.Red
        End Try

    End Sub

End Class
