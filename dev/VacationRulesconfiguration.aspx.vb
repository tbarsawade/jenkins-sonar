
Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports System.Security
Imports System.Security.Cryptography
Imports System.Random
Imports System.Globalization
Partial Class VacationRulesconfiguration
    Inherits System.Web.UI.Page
    Protected Sub Page_PreInit1(ByVal sender As Object, ByVal e As EventArgs) Handles Me.PreInit
        Try
            If Not Session("CTheme") Is Nothing And Not Session("CTheme") = String.Empty Then
                Page.Theme = Convert.ToString(Session("CTheme"))
                'Select Case* From mmm_mst_delegation Where Creatorid = 21879 And CAST(CreatedOn As Date) between CAST('2021-10-22' AS date) and  CAST('2021-10-23' AS date) 
            Else
                Page.Theme = "Default"
            End If
        Catch ex As Exception

        End Try

    End Sub

    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            Binduser()
            Binduser1()
        End If
        BinddataGrid()
    End Sub

    Public Sub Binduser1()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim da As SqlDataAdapter = New SqlDataAdapter("", con)
        Try
            Dim ds As New DataSet()
            Dim qry As String = ""

            If (HttpContext.Current.Session("USERROLE") = "CADMIN" Or HttpContext.Current.Session("USERROLE") = "SU") Then
                da.SelectCommand.CommandText = "Select distinct uid,userid=CONCAT(userid,'(' + emailid +')'),username,userrole from MMM_MST_USER where eid=" & HttpContext.Current.Session("EID") & ""
                da.Fill(ds, "qry")
            Else
                da.SelectCommand.CommandText = "Select distinct uid,userid=CONCAT(userid,'(' + emailid +')'),username,userrole from MMM_MST_USER where eid=" & HttpContext.Current.Session("EID") & " and uid=  " & HttpContext.Current.Session("UID") & " "
                '                Select Case r.roleid,r.rolename,r.eid,r.roledescription,r.roletype,r.AllowedRoles,u.userid,u.UserName [CreatedBy] from mmm_mst_role As r  
                'inner Join  MMM_MST_USER  as u ON u.userrole=r.rolename And r.AllowedRoles=1 And r.eid=209 And u.uid=74034
                da.Fill(ds, "qry")
            End If
            Sourceuserid.DataSource = ds
            Sourceuserid.DataTextField = "userid"
            Sourceuserid.DataValueField = "uid"
            Sourceuserid.DataBind()
            Sourceuserid.Items.Insert("0", New ListItem("SELECT"))
        Catch
        Finally
            da.Dispose()
            con.Close()
            con.Dispose()
        End Try
    End Sub
    Public Sub Binduser()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim da As SqlDataAdapter = New SqlDataAdapter("", con)
        Try
            Dim ds As New DataSet()
            Dim qry As String = ""
            'da.SelectCommand.CommandText = "Select distinct uid,userid=CONCAT(userid,'(' + emailid +')'),username,userrole from MMM_MST_USER where eid=" & HttpContext.Current.Session("EID") & " and  username  <> 'Super Admin' "

            da.SelectCommand.CommandText = "Select distinct  u.uid,userid=CONCAT(userid,'(' + u.emailid +')'),r.rolename,r.eid,r.roledescription,r.roletype,r.alloweddelegate,u.userrole,u.UserName from mmm_mst_role As r  inner Join MMM_MST_USER  as u ON u.userrole=r.rolename And r.alloweddelegate=1 And r.eid=" & HttpContext.Current.Session("EID") & " and  u.username  <> 'Super Admin' "
            da.Fill(ds, "qry")
            Delegate_to_Id.DataSource = ds
            Delegate_to_Id.DataTextField = "userid"
            Delegate_to_Id.DataValueField = "uid"
            Delegate_to_Id.DataBind()
            Delegate_to_Id.Items.Insert("0", New ListItem("SELECT"))
        Catch
        Finally
            da.Dispose()
            con.Close()
            con.Dispose()
        End Try
    End Sub
    Public Sub BinddataGrid()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim da As SqlDataAdapter = New SqlDataAdapter("", con)

        If (HttpContext.Current.Session("USERROLE") = "CADMIN" Or HttpContext.Current.Session("USERROLE") = "SU") Then
            da.SelectCommand.CommandText = "SELECT TID,CreatorID,CreatorName,(Select MMM_MST_USER.userid from MMM_MST_USER where MMM_MST_USER.uid=mmm_mst_delegation.Delegate_to_Id) as Delegate_to_Id,Delegate_to_Name,CONVERT(char(10), Start_Date,126) as Start_Date,CONVERT(char(10), End_Date,126) as End_Date,Delegation_Reason FROM mmm_mst_delegation WHERE  EID ='" & Session("EID").ToString & "' order by tid desc"
        Else
            da.SelectCommand.CommandText = "SELECT TID,CreatorID,CreatorName,(Select MMM_MST_USER.userid from MMM_MST_USER where MMM_MST_USER.uid=mmm_mst_delegation.Delegate_to_Id) as Delegate_to_Id,Delegate_to_Name,CONVERT(char(10), Start_Date,126) as Start_Date,CONVERT(char(10), End_Date,126) as End_Date,Delegation_Reason FROM mmm_mst_delegation WHERE CreatorID = '" & HttpContext.Current.Session("UID") & "' and  EID ='" & Session("EID").ToString & "'  order by tid desc "

        End If
        'Dim da As New SqlDataAdapter("SELECT TID,CreatorID,CreatorName,(Select MMM_MST_USER.userid from MMM_MST_USER where MMM_MST_USER.uid=mmm_mst_delegation.Delegate_to_Id) as Delegate_to_Id,Delegate_to_Name,CONVERT(char(10), Start_Date,126) as Start_Date,CONVERT(char(10), End_Date,126) as End_Date,Delegation_Reason FROM mmm_mst_delegation WHERE CreatorID = '" & HttpContext.Current.Session("UID") & "'  order by tid ", con)

        Dim ds As New DataSet
        da.Fill(ds, "data")

        gvData.DataSource = ds.Tables("data")
        gvData.DataBind()

        da.Dispose()
        con.Dispose()
    End Sub

    Protected Sub AddRecord(ByVal sender As Object, ByVal e As System.EventArgs)

        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim da As SqlDataAdapter = New SqlDataAdapter("", con)


        Try
            Dim strData As String = sourceuserID.SelectedItem.Text.ToString()
            Dim arrData As String() = Split(strData, "(")
            Dim Creatername As String = arrData(0)
            da.SelectCommand.CommandText = "insert into mmm_mst_delegation(CreatorID,CreatorName,Delegate_to_Id,Delegate_to_Name,Start_Date,End_Date,Delegation_Reason,CreatedOn,EID) VALUES('" & sourceuserID.SelectedItem.Value & "' ,'" & Creatername.ToString() & "','" & Delegate_to_Id.SelectedItem.Value & "','" & Delegate_to_Name.Text & "','" & txtSdate.Text & "', '" & txtEdate.Text & "','" & Delegation_Reason.Value & "','" & DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") & "','" & Session("EID").ToString & "')"
            Dim ds As New DataSet
            da.Fill(ds, "data")
            da.Dispose()
            con.Close()
            con.Dispose()
            BinddataGrid()
            Delegate_to_Id.SelectedIndex = 0
            sourceuserID.SelectedIndex = 0
            Delegate_to_Name.Text = ""
            Delegation_Reason.Value = ""
            txtSdate.Text = ""
            txtEdate.Text = ""
            ScriptManager.RegisterClientScriptBlock(Me, Me.GetType(), "alertMessage", "alert('Record Inserted Successfully')", True)

        Catch ex As Exception
        End Try
    End Sub
    Protected Sub Delegate_to_Id_SelectedIndexChanged(sender As Object, e As EventArgs) Handles Delegate_to_Id.SelectedIndexChanged
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim da As SqlDataAdapter = New SqlDataAdapter("", con)
        Try
            Dim ds As New DataSet()
            Dim qry As String = ""
            da.SelectCommand.CommandText = "Select username from MMM_MST_USER where eid=" & HttpContext.Current.Session("EID") & " and uid =  " & Delegate_to_Id.SelectedItem.Value & " "
            da.Fill(ds, "qry")

            Delegate_to_Name.Text = ds.Tables(0).Rows(0).Item("username").ToString()
        Catch
        Finally
            da.Dispose()
            con.Close()
            con.Dispose()
        End Try

    End Sub

    Protected Sub EditHit(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim btnDetails As ImageButton = TryCast(sender, ImageButton)
        Dim row As GridViewRow = DirectCast(btnDetails.NamingContainer, GridViewRow)
        Dim tid As Integer = Convert.ToString(Me.gvData.DataKeys(row.RowIndex).Value)
        'btnActEdit.Text = "Update"
        ViewState("tid") = tid

        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
        Dim dt As New DataTable()
        oda.SelectCommand.CommandText = "Select distinct uid,userid=CONCAT(userid,'(' + emailid +')'),username,userrole from MMM_MST_USER where eid=" & HttpContext.Current.Session("EID") & " and  username  <> 'Super Admin' "
        oda.Fill(dt)
        If Delegate_to_Id1.Items.Count > 0 Then
            Delegate_to_Id1.Items.Clear()
        End If
        If dt.Rows.Count > 0 Then
            Delegate_to_Id1.DataSource = dt
            Delegate_to_Id1.DataTextField = "userid"
            Delegate_to_Id1.DataValueField = "uid"
            Delegate_to_Id1.DataBind()
            Delegate_to_Id1.Items.Insert("0", New ListItem("SELECT"))
        End If
        'Binduser()
        Dim odaNew As SqlDataAdapter = New SqlDataAdapter("", con)
        Dim dtNew As New DataTable()
        odaNew.SelectCommand.CommandText = "select Delegate_to_Id,Delegate_to_Name,CONVERT(char(10), Start_Date,126) as Start_Date,CONVERT(char(10), End_Date,126) as End_Date,Delegation_Reason from mmm_mst_delegation where   tid= '" & ViewState("tid") & "' and eid=" & HttpContext.Current.Session("EID") & " "
        odaNew.Fill(dtNew)
        If dtNew.Rows.Count > 0 Then
            Delegate_to_Id1.SelectedValue = dtNew.Rows(0).Item("Delegate_to_Id").ToString()
            Delegate_to_Name1.Text = dtNew.Rows(0).Item("Delegate_to_Name").ToString()
            Delegation_Reason1.Value = dtNew.Rows(0).Item("Delegation_Reason").ToString()
            txtS1date.Text = dtNew.Rows(0).Item("Start_Date").ToString()
            txtE1date.Text = dtNew.Rows(0).Item("End_Date").ToString()


        End If
        Me.updatePanelEdit.Update()
        Me.btnEdit_ModalPopupExtender.Show()
    End Sub

    Protected Sub EditRecord(ByVal sender As Object, ByVal e As System.EventArgs)

        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim da As SqlDataAdapter = New SqlDataAdapter("", con)

        Try

            da.SelectCommand.CommandText = "update mmm_mst_delegation set Delegate_to_Id='" & Delegate_to_Id1.SelectedItem.Value.ToString() & "', Delegate_to_Name='" & Delegate_to_Name1.Text & "', Delegation_Reason='" & Delegation_Reason1.Value & "',Start_Date='" & txtS1date.Text & "' ,End_Date='" & txtE1date.Text & "', modifiedON='" & DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") & "'  where tid= '" & ViewState("tid") & "' and EID='" & Session("EID").ToString & "' "
            Dim ds As New DataSet
            da.Fill(ds, "data")
            da.Dispose()
            con.Close()
            con.Dispose()
            BinddataGrid()
            Me.updatePanelEdit.Update()
            Me.btnEdit_ModalPopupExtender.Hide()
            ScriptManager.RegisterClientScriptBlock(Me, Me.GetType(), "alertMessage", "alert('Record Updated  Successfully')", True)
        Catch ex As Exception
        End Try
    End Sub

    Protected Sub Delegate_to_Id1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles Delegate_to_Id1.SelectedIndexChanged
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim da As SqlDataAdapter = New SqlDataAdapter("", con)
        Try
            Dim ds As New DataSet()
            Dim qry As String = ""
            'da.SelectCommand.CommandText = "select fld2 + ' (' + fld1 + ')'[Vendor],tid from mmm_mst_master with(nolock) where eid=180 and isauth=1  and  documenttype='Vendor Master' order by fld2"
            da.SelectCommand.CommandText = "Select username from MMM_MST_USER where eid=" & HttpContext.Current.Session("EID") & " and uid =  " & Delegate_to_Id1.SelectedItem.Value & " "
            da.Fill(ds, "qry")

            Delegate_to_Name1.Text = ds.Tables(0).Rows(0).Item("username").ToString()
        Catch
        Finally
            da.Dispose()
            con.Close()
            con.Dispose()
        End Try

    End Sub



    Protected Sub btnexport_Click(sender As Object, e As ImageClickEventArgs) Handles btnexport.Click

        Try
            Response.ClearContent()
            gvData.AllowPaging = False
            gvData.FooterRow.Visible = False
            gvData.Columns(5).Visible = False
            Response.ContentType = "application/vnd.ms-excel"
            Response.AddHeader("content-disposition", "attachment;filename=vacationrule"".xls")
            'Prepare to export the DataGrid
            Dim oStringWriter As New System.IO.StringWriter
            Dim oHtmlTextWriter As New HtmlTextWriter(oStringWriter)

            'Use the DataGrid control to add the details
            gvData.RenderControl(oHtmlTextWriter)
            'Finish the Excel spreadsheet and send the response
            Response.Write(oStringWriter.ToString())
            ' gridDocs0.Dispose()
            Response.End()

        Catch ex As Exception
            'lblMsg.ForeColor = Drawing.Color.Red
            'lblMsg.Text = "An error occured when exporting data. Please try again"
        End Try

    End Sub

    Public Overloads Overrides Sub VerifyRenderingInServerForm(ByVal control As Control)
        '    '    ' Verifies that the control is rendered
        ' BinddataGrid()
    End Sub


End Class
