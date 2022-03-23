Imports System.Data.SqlClient
Imports System.Data

Partial Class AutoDocConfig
    Inherits System.Web.UI.Page

    Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
    Dim con As New SqlConnection(conStr)

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            BindGrid()

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


    'Add Auto Config
    Public Sub btnAutoConfig_click(ByVal sender As Object, e As System.EventArgs)
        btnSave.Text = "SAVE"
        ClearFields()
        txtConfigName.ReadOnly = False
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim da As SqlDataAdapter = New SqlDataAdapter("", con)
        Dim dt As New DataSet()
        'Dim btnDetails As ImageButton = TryCast(sender, ImageButton)
        'Dim row As GridViewRow = DirectCast(btnDetails.NamingContainer, GridViewRow)
        'If ddlSourceForm.Items.Count > 0 Then
        '    ddlSourceForm.Items.Clear()
        'End If
        'If ddlFormSourceType.Items.Count > 0 Then
        '    ddlFormSourceType.Items.Clear()
        'End If
        'ddlSourceForm.Items.Add(row.Cells(1).Text.Trim)
        'ddlFormSourceType.Items.Add(row.Cells(3).Text.Trim)
        Try

            da.SelectCommand.CommandText = "select formname,formname+'-'+formtype as [formtype] from mmm_mst_forms where eid=" & Session("EID") & " and formsource='Menu Driven' and isactive=1 ;"

            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            da.Fill(dt, "AllForms")
            If dt.Tables("AllForms").Rows.Count > 0 Then
                ddlSourceForm.DataSource = dt.Tables("AllForms")
                ddlSourceForm.DataTextField = "formname"
                ddlSourceForm.DataValueField = "formtype"
                ddlSourceForm.DataBind()
                'ddlSourceForm.Items.Insert("1", New ListItem("USER", "USER"))
                ddlSourceForm.Items.Insert("0", New ListItem("SELECT"))
                ddlSourceForm.SelectedIndex = 0
            End If
        Catch ex As Exception

        End Try

        Up_pnlAutoConfig.Update()
        MP_AutoConfig.Show()
    End Sub
    'Add For Auto Config

    Protected Sub CloseAutoConfig()
        Me.MP_AutoConfig.Hide()
    End Sub
    'Add For Creator 

    Protected Sub CloseAutoConfigdtl()
        Me.MP_AutoConfigdtl.Hide()
        Me.Up_pnlAutoConfigdtlMapping.Update()
        Me.MP_AutoConfigdtlMapping.Show()
    End Sub

    Protected Sub CloseAutoConfigdtlMapping()
        Me.MP_AutoConfigdtlMapping.Hide()
    End Sub
    Protected Sub CloseAutoConfigdtlMappingChild()
        Me.MP_AutoConfigChilddtlMapping.Hide()
    End Sub
    Protected Sub ddlCreator_SelectedIndexChanged(sender As Object, e As EventArgs)

        Dim da As New SqlDataAdapter("", con)

        Dim dt As New DataTable()

        If ddlCreator.SelectedIndex = 0 Then
            If ddlCreatorValue.Items.Count > 0 Then
                ddlCreatorValue.Items.Clear()
            End If
        ElseIf ddlCreator.SelectedIndex = 1 Then

            If dt.Rows.Count > 0 Then
                If ddlCreatorValue.Items.Count > 0 Then
                    ddlCreatorValue.Items.Clear()
                End If
                ddlCreatorValue.Items.Insert("0", New ListItem("SELECT"))
                ddlCreatorValue.Items.Insert("1", New ListItem(ddlCreator.SelectedItem.Text.Trim()))
            Else
                If ddlCreatorValue.Items.Count > 0 Then
                    ddlCreatorValue.Items.Clear()
                End If
                ddlCreatorValue.Items.Insert("0", New ListItem("SELECT"))
                ddlCreatorValue.Items.Insert("1", New ListItem(ddlCreator.SelectedItem.Text.Trim()))
            End If
        ElseIf ddlCreator.SelectedIndex = 2 Then
            da.SelectCommand.CommandText = "select RoleName from mmm_mst_role where eid=" & Session("EID")
            da.SelectCommand.CommandType = CommandType.Text
            da.Fill(dt)
            If dt.Rows.Count > 0 Then
                ddlCreatorValue.DataSource = dt
                ddlCreatorValue.DataTextField = "RoleName"
                ddlCreatorValue.DataValueField = "RoleName"
                ddlCreatorValue.DataBind()
                ddlCreatorValue.Items.Insert("0", New ListItem("SELECT"))
            Else
                ddlCreatorValue.DataSource = Nothing
                ddlCreatorValue.DataBind()
            End If
        ElseIf ddlCreator.SelectedIndex = 3 Then
            da.SelectCommand.CommandText = "select displayname,fieldmapping from mmm_mst_fields where documenttype='" & ddlSourceForm.SelectedItem.Text & "' and eid=" & Session("EID") & " and isactive=1 and invisible=0 and (fieldtype ='Drop down' or fieldtype ='LookupDDL' or fieldtype ='Multi LookupDDL')"
            da.SelectCommand.CommandType = CommandType.Text
            da.Fill(dt)
            If dt.Rows.Count > 0 Then
                ddlCreatorValue.DataSource = dt
                ddlCreatorValue.DataTextField = "displayname"
                ddlCreatorValue.DataValueField = "fieldmapping"
                ddlCreatorValue.DataBind()
                ddlCreatorValue.Items.Insert("0", New ListItem("SELECT"))
            Else
                ddlCreatorValue.DataSource = Nothing
                ddlCreatorValue.DataBind()
            End If
        ElseIf ddlCreator.SelectedIndex = 4 Then

            If dt.Rows.Count > 0 Then
                If ddlCreatorValue.Items.Count > 0 Then
                    ddlCreatorValue.Items.Clear()
                End If
                ddlCreatorValue.Items.Insert("0", New ListItem("SELECT"))
                ddlCreatorValue.Items.Insert("1", New ListItem("UID"))
            Else
                If ddlCreatorValue.Items.Count > 0 Then
                    ddlCreatorValue.Items.Clear()
                End If
                ddlCreatorValue.Items.Insert("0", New ListItem("SELECT"))
                ddlCreatorValue.Items.Insert("1", New ListItem("UID"))
            End If
        End If
    End Sub

    Protected Sub ddlSourceForm_SelectedIndexChanged(sender As Object, e As EventArgs)
        Dim da As New SqlDataAdapter("", con)
        ddlCreator.SelectedIndex = 0
        ddlCreator_SelectedIndexChanged(ddlCreator, New EventArgs())
        If ddlFormSourceType.Items.Count > 0 Then
            ddlFormSourceType.Items.Clear()
        End If
        Dim dt As New DataSet
        If ddlSourceForm.SelectedIndex <> 0 Then

            If ddlSourceForm.SelectedItem.Text.Trim().ToUpper = "USER" Then
                If ddlCreator.Items(1).Text.ToString() = "OUID" Then
                    ddlCreator.Items.RemoveAt(1)
                    ddlCreator.Items.Insert("1", New ListItem("UID", "UID"))
                Else
                    ddlCreator.Items.Insert("1", New ListItem("UID", "UID"))
                End If
                ddlFormSourceType.Items.Insert("0", "USER")
                ddlFormSourceType.SelectedIndex = 0
                da.SelectCommand.CommandText = "select formname,formname+'-'+formType as formType  from mmm_mst_forms where eid=" & Session("EID") & " and formtype='DOCUMENT' and formsource='Menu Driven' and isactive=1 and FormName not in ('" & ddlSourceForm.SelectedItem.Text.Trim & "') ;select statusname from mmm_mst_workflow_status where  eid=" & Session("EID") & " and documenttype='" & ddlSourceForm.SelectedItem.Text.Trim() & "';select formname,formname+'-'+formType as formType  from mmm_mst_forms where eid=" & Session("EID") & "  and formsource not in ('Detail Form','ACTION DRIVEN')  and isactive=1 and FormName not in ('" & ddlSourceForm.SelectedItem.Text.Trim & "');select name as displayname, 'MMM_MST_USER-'+name as name from sys.columns where object_id=object_id('mmm_mst_user') and name not like'fld%'  union all select displayname, 'MMM_MST_USER-'+FIELDMAPPING as name from mmm_mst_fields where eid=" & Session("EID") & " and documenttype='user';"
                Secondarydoc.Visible = True
                If con.State <> ConnectionState.Open Then
                    con.Open()
                End If
                da.Fill(dt, "FS")
                If dt.Tables("FS2").Rows.Count > 0 Then
                    ddlSecondarySourceForm.DataSource = dt.Tables("FS2")
                    ddlSecondarySourceForm.DataTextField = "formname"
                    ddlSecondarySourceForm.DataValueField = "formType"
                    ddlSecondarySourceForm.DataBind()
                    ddlSecondarySourceForm.Items.Insert("0", New ListItem("SELECT"))
                Else
                    If ddlSecondarySourceForm.Items.Count > 0 Then
                        ddlSecondarySourceForm.Items.Clear()
                        ddlSecondarySourceForm.Items.Insert("0", New ListItem("SELECT"))
                    End If
                End If
                If dt.Tables("FS3").Rows.Count > 0 Then
                    ddlConditionDataFields.DataSource = dt.Tables("FS3")
                    ddlConditionDataFields.DataTextField = "displayname"
                    ddlConditionDataFields.DataValueField = "name"
                    ddlConditionDataFields.DataBind()
                    ddlConditionDataFields.Items.Insert("0", New ListItem("SELECT"))
                Else
                    If ddlConditionDataFields.Items.Count > 0 Then
                        ddlConditionDataFields.Items.Clear()
                        ddlConditionDataFields.Items.Insert("0", New ListItem("SELECT"))
                    End If
                End If


            Else

                If ddlCreator.Items(1).Text.ToString() = "UID" Then
                    ddlCreator.Items.RemoveAt(1)
                    ddlCreator.Items.Insert("1", New ListItem("OUID", "OUID"))
                Else
                    If ddlCreator.Items(1).Text <> "OUID" Then
                        ddlCreator.Items.Insert("1", New ListItem("OUID", "OUID"))
                    End If
                End If
                'ddlCreator.Items(1).Attributes.Add("enabled", "enabled")
                'ddlCreator.Items(1).Attributes.Add("disabled", "enabled")
                Dim var As String() = ddlSourceForm.SelectedValue.Trim().Split("-")
                ddlFormSourceType.Items.Insert("0", New ListItem(var(1).ToString(), var(1).ToString()))
                ddlFormSourceType.SelectedIndex = 0
                'da.SelectCommand.CommandText = "select formname from mmm_mst_forms where eid=" & Session("EID") & " and formtype='DOCUMENT' and formsource='Menu Driven' and isactive=1 and FormName not in ('" & ddlSourceForm.SelectedItem.Text.Trim & "') ;select statusname from mmm_mst_workflow_status where  eid=" & Session("EID") & " and documenttype='" & ddlSourceForm.SelectedItem.Text.Trim() & "'; select formname,formname+'-'+formType as formType  from mmm_mst_forms where eid=" & Session("EID") & "  and formsource not in ('Detail Form','ACTION DRIVEN')  and isactive=1 and FormName not in ('" & ddlSourceForm.SelectedItem.Text.Trim & "'); select displayname, case '" & var(1).ToString() & "' when 'MASTER' then 'MMM_MST_MASTER-' else 'MMM_MST_DOCUMENT-' end +FIELDMAPPING as name from mmm_mst_fields where eid=" & Session("EID") & " and documenttype='" & ddlSourceForm.SelectedItem.Text & "';"
                da.SelectCommand.CommandText = "select formname,formname+'-'+formType as formType  from mmm_mst_forms where eid=" & Session("EID") & "  and formsource not in ('Detail Form','ACTION DRIVEN')  and isactive=1 and FormName not in ('" & ddlSourceForm.SelectedItem.Text.Trim & "');select statusname from mmm_mst_workflow_status where  eid=" & Session("EID") & " and documenttype='" & ddlSourceForm.SelectedItem.Text.Trim() & "'; select formname,formname+'-'+formType as formType  from mmm_mst_forms where eid=" & Session("EID") & "  and formsource not in ('Detail Form','ACTION DRIVEN')  and isactive=1 and FormName not in ('" & ddlSourceForm.SelectedItem.Text.Trim & "'); select displayname, case '" & var(1).ToString() & "' when 'MASTER' then 'MMM_MST_MASTER-' else 'MMM_MST_DOCUMENT-' end +FIELDMAPPING as name from mmm_mst_fields where eid=" & Session("EID") & " and documenttype='" & ddlSourceForm.SelectedItem.Text & "';"
                Secondarydoc.Visible = True
                If con.State <> ConnectionState.Open Then
                    con.Open()
                End If
                da.Fill(dt, "FS")
                If ddlSecondarySourceForm.Items.Count > 0 Then
                    ddlSecondarySourceForm.Items.Clear()
                    ddlSecondarySourceForm.Items.Insert("0", New ListItem("SELECT"))
                End If
                If dt.Tables("FS2").Rows.Count > 0 Then
                    ddlSecondarySourceForm.DataSource = dt.Tables("FS2")
                    ddlSecondarySourceForm.DataTextField = "formname"
                    ddlSecondarySourceForm.DataValueField = "formType"
                    ddlSecondarySourceForm.DataBind()
                    ddlSecondarySourceForm.Items.Insert("0", New ListItem("SELECT"))
                Else
                    If ddlSecondarySourceForm.Items.Count > 0 Then
                        ddlSecondarySourceForm.Items.Clear()
                        ddlSecondarySourceForm.Items.Insert("0", New ListItem("SELECT"))
                    End If
                End If
                If dt.Tables("FS3").Rows.Count > 0 Then
                    ddlConditionDataFields.DataSource = dt.Tables("FS3")
                    ddlConditionDataFields.DataTextField = "displayname"
                    ddlConditionDataFields.DataValueField = "name"
                    ddlConditionDataFields.DataBind()
                    ddlConditionDataFields.Items.Insert("0", New ListItem("SELECT"))
                Else
                    If ddlConditionDataFields.Items.Count > 0 Then
                        ddlConditionDataFields.Items.Clear()
                        ddlConditionDataFields.Items.Insert("0", New ListItem("SELECT"))
                    End If
                End If
            End If



            If dt.Tables("FS").Rows.Count > 0 Then
                ddlTargetDoc.DataSource = dt.Tables("FS")
                ddlTargetDoc.DataTextField = "formname"
                ddlTargetDoc.DataValueField = "formType"
                ddlTargetDoc.DataBind()
                ddlTargetDoc.Items.Insert("0", New ListItem("SELECT"))
            End If
            If dt.Tables("FS1").Rows.Count > 0 Then
                ddlWFStatus.DataSource = dt.Tables("FS1")
                ddlWFStatus.DataTextField = "statusname"
                ddlWFStatus.DataValueField = "statusname"
                ddlWFStatus.DataBind()
                ddlWFStatus.Items.Insert("0", New ListItem("SELECT"))
            End If

        Else
            If ddlTargetDoc.Items.Count > 0 Then
                ddlTargetDoc.Items.Clear()
                ddlTargetDoc.Items.Insert("0", New ListItem("SELECT"))
            End If
            If ddlWFStatus.Items.Count > 0 Then
                ddlWFStatus.Items.Clear()
                ddlWFStatus.Items.Insert("0", New ListItem("SELECT"))
            End If
        End If
        isSecCreator_CheckedChanged(isSecCreator, New EventArgs())
    End Sub


    <System.Web.Services.WebMethod()> _
    Public Shared Function CheckUserName(ByVal userName As String) As String

        Dim returnValue As String = String.Empty

        Try

            Dim consString As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString()

            Dim conn As New SqlConnection(consString)

            Dim cmd As New SqlCommand("sp_CheckUserAvailability", conn)

            cmd.CommandType = CommandType.StoredProcedure

            cmd.Parameters.AddWithValue("@UserName", userName.Trim())
            cmd.Parameters.AddWithValue("@EID", HttpContext.Current.Session("EID"))
            conn.Open()

            returnValue = cmd.ExecuteScalar().ToString()

            conn.Close()

        Catch

            returnValue = "error"

        End Try

        Return returnValue

    End Function

    Protected Sub BindGrid()
        Dim consString As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString()
        Dim conn As New SqlConnection(consString)
        Dim da As New SqlDataAdapter("", conn)
        da.SelectCommand.CommandType = CommandType.Text
        da.SelectCommand.CommandText = "select TID,configname,Sourcedoc,targetdoc,noofrecords,WFstatus from mmm_schdoc_main where eid=" & Session("EID")
        Dim dt As New DataTable
        da.Fill(dt)
        If dt.Rows.Count > 0 Then
            gvData.DataSource = dt
            gvData.DataBind()
        Else
            gvData.EmptyDataText = "There were no records against this entity"
        End If
    End Sub
    Protected Sub btnSave_Click(sender As Object, e As EventArgs)
        Dim consString As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString()
        Dim conn As New SqlConnection(consString)
        Dim returnValue As String = ""
        Dim filtervalue As New ArrayList

        For Each li As ListItem In chkCommanFields.Items
            If li.Selected = True Then
                filtervalue.Add(li.Value.Trim())
            End If
        Next
        Dim CreatorSource As String = ""
        Dim Creator As String = ""
        Dim CreatorValue As String = ""
        If isSecCreator.Checked = True Then
            CreatorSource = "SECONDARY"
            Creator = ddlSecSourceCreator.SelectedValue
            CreatorValue = ddlSecSourceCreatorValue.SelectedValue
        Else
            CreatorSource = "PRIMARY"
            Creator = ddlCreator.SelectedValue
            CreatorValue = ddlCreatorValue.SelectedValue
        End If
        Try
            If btnSave.Text = "SAVE" Then
                Dim pair As KeyValuePair(Of Boolean, String) = chkvalidation()
                If pair.Key = False Then
                    lblmsgred.Text = pair.Value
                    Exit Sub
                End If
                Dim cmd As New SqlCommand("sp_CheckUserAvailability", conn)
                cmd.CommandType = CommandType.StoredProcedure

                cmd.Parameters.AddWithValue("@UserName", txtConfigName.Text)
                conn.Open()
                returnValue = cmd.ExecuteScalar().ToString()
                conn.Close()
                If returnValue.ToUpper = "TRUE" Then
                    If ddlSecondarySourceForm.SelectedIndex = -1 Then
                        ddlSecondarySourceForm.Items.Insert("0", New ListItem("SELECT"))
                    End If
                    If ddlSecondarySourceType.SelectedIndex = -1 Then
                        ddlSecondarySourceType.Items.Insert("0", New ListItem("SELECT"))
                    End If

                    Dim cmd1 As New SqlCommand("", conn)
                    cmd1.CommandType = CommandType.Text
                    'Dim checkval As String = "IF EXISTS (select * from mmm_schdoc_main where SourceDoc='" & ddlSourceForm.SelectedItem.Text.Trim() & "' and Targetdoc='" & ddlTargetDoc.SelectedItem.Text.Trim() & "' and SqType='" & ddlFormSourceType.SelectedItem.Text & "' and TQType='" & ddlTargetType.SelectedItem.Text.Trim() & "' and eid=" & Session("EID") & ") select 'True' else select 'False'"
                    Dim strval As String = "insert into mmm_schdoc_main (eid,SourceDoc,SQType,TargetDoc,TQType,NoofRecords,CreateEvent,ScheduleTime,WFStatus,Creator,RoleName,lastdate,ConfigName,isactive,filterFields,ConditionFields,SecondarySourceDoc,SecondarySQType,ConditionFieldsText,SecConditionFields,SecConditionFieldsText,CreatorSource) values(" & Session("EID") & ",'" & ddlSourceForm.SelectedItem.Text.ToString() & "','" & ddlFormSourceType.SelectedValue.ToString() & "','" & ddlTargetDoc.SelectedItem.Text & "','" & ddlTargetType.SelectedValue.ToString() & "'," & txtNoRows.Text & ",'" & ddlCreateEvent.SelectedValue.ToString() & "','" & IIf(String.IsNullOrEmpty(txtMM.Text), "*", txtMM.Text) & "|" & IIf(String.IsNullOrEmpty(txtDD.Text), "*", txtDD.Text) & "|" & IIf(String.IsNullOrEmpty(txtWW.Text), "*", txtWW.Text) & "|" & IIf(String.IsNullOrEmpty(txtHH.Text), "*", txtHH.Text) & "|" & IIf(String.IsNullOrEmpty(txtMN.Text), "*", txtMN.Text) & "','" & ddlWFStatus.SelectedValue.ToString() & "','" & Creator.ToString() & "','" & CreatorValue.ToString() & "',getdate(),'" & txtConfigName.Text & "','" & chkisactive.Checked & "','" & IIf(filtervalue.Count = 0, "", String.Join(",", filtervalue.ToArray)) & "','" & IIf(txtAddConditionArea.Text.ToString() = String.Empty, "", txtAddConditionArea.Text.ToString()) & "','" & IIf(ddlSecondarySourceForm.SelectedIndex = 0, "", IIf(String.IsNullOrEmpty(ddlSecondarySourceForm.SelectedItem.Text), "", Convert.ToString(ddlSecondarySourceForm.SelectedItem.Text))) & "','" & IIf(ddlSecondarySourceForm.SelectedIndex = 0, "", Convert.ToString(ddlSecondarySourceType.SelectedItem.Text)) & "','" & IIf(txtShowAddConditionArea.Text.ToString() = String.Empty, "", txtShowAddConditionArea.Text.ToString()) & "','" & IIf(txtSAddConditionArea.Text.ToString() = String.Empty, "", txtSAddConditionArea.Text.ToString()) & "','" & IIf(txtSShowAddConditionArea.Text.ToString() = String.Empty, "", txtSShowAddConditionArea.Text.ToString()) & "','" & CreatorSource.ToString() & "')"
                    'cmd1.CommandText = checkval
                    'If conn.State = ConnectionState.Closed Then
                    '    conn.Open()
                    'End If
                    'Dim strresult As String = cmd1.ExecuteScalar()
                    'If strresult.ToUpper = "FALSE" Then
                    cmd1.CommandText = strval
                    If conn.State = ConnectionState.Closed Then
                        conn.Open()
                    End If
                    cmd1.ExecuteNonQuery()
                    'Else
                    '    lblmsgred.Text = "This Configuration already exist against this source document " & ddlSourceForm.SelectedValue.Trim() & " and Target document " & ddlTargetDoc.SelectedValue.Trim()
                    '    Exit Sub
                    'End If
                    BindGrid()
                    Up_pnlAutoConfig.Update()
                    MP_AutoConfig.Hide()

                ElseIf returnValue.ToUpper = "FALSE" Then
                    lblmsgred.Text = "This Config Name already Exist"
                    Exit Sub
                Else
                    lblmsgred.Text = "There were some error Please try again"
                    Exit Sub
                End If
            Else
                Dim pair As KeyValuePair(Of Boolean, String) = chkvalidation()
                If pair.Key = False Then
                    lblmsgred.Text = pair.Value
                    Exit Sub
                End If
                Dim cmd1 As New SqlCommand("", conn)
                cmd1.CommandType = CommandType.Text
                Dim strval As String = "update mmm_schdoc_main set SourceDoc='" & ddlSourceForm.SelectedItem.Text.ToString() & "',SQType='" & ddlFormSourceType.SelectedValue.ToString() & "',TargetDoc='" & ddlTargetDoc.SelectedItem.Text.Trim() & "',TQType='" & ddlTargetType.SelectedValue.ToString() & "',NoofRecords=" & txtNoRows.Text & ",CreateEvent='" & ddlCreateEvent.SelectedValue & "',ScheduleTime='" & IIf(String.IsNullOrEmpty(txtMM.Text), "*", txtMM.Text) & "|" & IIf(String.IsNullOrEmpty(txtDD.Text), "*", txtDD.Text) & "|" & IIf(String.IsNullOrEmpty(txtWW.Text), "*", txtWW.Text) & "|" & IIf(String.IsNullOrEmpty(txtHH.Text), "*", txtHH.Text) & "|" & IIf(String.IsNullOrEmpty(txtMN.Text), "*", txtMN.Text) & "',WFStatus='" & ddlWFStatus.SelectedValue & "',Creator='" & Creator.ToString() & "',RoleName='" & CreatorValue.ToString() & "',lastdate=getdate(),isactive='" & chkisactive.Checked & "',filterFields='" & IIf(filtervalue.Count = 0, "", String.Join(",", filtervalue.ToArray)) & "',ConditionFields='" & IIf(txtAddConditionArea.Text.ToString() = String.Empty, "", txtAddConditionArea.Text.ToString()) & "',SecondarySourceDoc='" & IIf(ddlSecondarySourceForm.SelectedIndex = 0, "", IIf(String.IsNullOrEmpty(ddlSecondarySourceForm.SelectedItem.Text), "", Convert.ToString(ddlSecondarySourceForm.SelectedItem.Text))) & "',SecondarySQType='" & IIf(ddlSecondarySourceForm.SelectedIndex = 0, "", ddlSecondarySourceType.SelectedItem.Text.Trim()) & "',ConditionFieldsText='" & IIf(txtShowAddConditionArea.Text.ToString() = String.Empty, "", txtShowAddConditionArea.Text.ToString()) & "',SecConditionFields='" & IIf(txtSAddConditionArea.Text.ToString() = String.Empty, "", txtSAddConditionArea.Text.ToString()) & "',SecConditionFieldsText='" & IIf(txtSShowAddConditionArea.Text.ToString() = String.Empty, "", txtSShowAddConditionArea.Text.ToString()) & "', CreatorSource='" & CreatorSource.ToString() & "'   where tid=" & ViewState("TID") & " and eid=" & Session("EID")
                cmd1.CommandText = strval
                If conn.State = ConnectionState.Closed Then
                    conn.Open()
                End If
                cmd1.ExecuteNonQuery()
                BindGrid()
                Up_pnlAutoConfig.Update()
                MP_AutoConfig.Hide()
            End If
        Catch ex As Exception

        End Try
    End Sub

    Protected Sub EditHit(ByVal sender As Object, ByVal e As System.EventArgs)
        btnSave.Text = "UPDATE"
        txtConfigName.ReadOnly = True
        Dim btnDetails As ImageButton = TryCast(sender, ImageButton)
        Dim row As GridViewRow = DirectCast(btnDetails.NamingContainer, GridViewRow)
        Dim Formid As Integer = Convert.ToString(Me.gvData.DataKeys(row.RowIndex).Value)
        Dim consString As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString()
        Dim conn As New SqlConnection(consString)
        Dim cmd As New SqlCommand("", conn)
        Dim dts As New DataTable
        Dim dt As New DataSet
        cmd.CommandType = CommandType.Text
        Dim strval As String = "select * from mmm_schdoc_main where eid=" & Session("EID") & " and TID=" & Formid & ";"
        ViewState("TID") = Formid.ToString()
        cmd.CommandText = strval
        If conn.State = ConnectionState.Closed Then
            conn.Open()
        End If
        cmd.ExecuteNonQuery()
        Dim da As SqlDataAdapter = New SqlDataAdapter(cmd)
        da.Fill(dts)
        If dts.Rows.Count > 0 Then
            txtConfigName.Text = dts.Rows(0)("configname").ToString()
            txtNoRows.Text = dts.Rows(0)("noofrecords").ToString()
            da.SelectCommand.CommandText = "select formname,formname+'-'+formtype as [formtype] from mmm_mst_forms where eid=" & Session("EID") & " and  formsource='Menu Driven' and isactive=1 ;"

            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            da.Fill(dt, "AllForms")
            If dt.Tables("AllForms").Rows.Count > 0 Then
                ddlSourceForm.DataSource = dt.Tables("AllForms")
                ddlSourceForm.DataTextField = "formname"
                ddlSourceForm.DataValueField = "formtype"
                ddlSourceForm.DataBind()
                ddlSourceForm.Items.Insert("0", New ListItem("SELECT"))
                ddlSourceForm.SelectedIndex = ddlSourceForm.Items.IndexOf(ddlSourceForm.Items.FindByText(dts.Rows(0)("SourceDoc").ToString()))
            End If
            If dts.Rows(0)("CreatorSource").ToString().Trim() = "PRIMARY" Then
                isSecCreator.Checked = False
            Else
                isSecCreator.Checked = True
            End If

            ddlSourceForm_SelectedIndexChanged(ddlSourceForm, New EventArgs())
            ddlTargetDoc.SelectedIndex = ddlTargetDoc.Items.IndexOf(ddlTargetDoc.Items.FindByText(dts.Rows(0)("TargetDoc").ToString()))
            ddlTargetDoc_SelectedIndexChanged(ddlTargetDoc, New EventArgs())
            ddlCreateEvent.SelectedValue = dts.Rows(0)("CreateEvent").ToString()
            ddlWFStatus.SelectedValue = dts.Rows(0)("WFStatus").ToString()
            ddlSourceForm.Attributes.Add("disabled", "disabled")
            ddlTargetDoc.Attributes.Add("disabled", "disabled")
            chkisactive.Checked = dts.Rows(0)("isactive")
            ddlSecondarySourceForm.SelectedIndex = ddlSecondarySourceForm.Items.IndexOf(ddlSecondarySourceForm.Items.FindByText(Convert.ToString(dts.Rows(0)("SecondarySourceDoc"))))
            If ddlSecondarySourceForm.SelectedIndex = 0 Then
            Else
                ddlSecondarySourceForm_SelectedIndexChanged(ddlSecondarySourceForm, New EventArgs())

                txtSShowAddConditionArea.Text = Convert.ToString(dts.Rows(0)("SecConditionFieldsText"))
                txtSShowAddConditionArea.Text = txtSShowAddConditionArea.Text.Replace("'", "''")

                txtSAddConditionArea.Text = Convert.ToString(dts.Rows(0)("SecConditionFields"))
                txtSAddConditionArea.Text = txtSAddConditionArea.Text.Replace("'", "''")
            End If
            txtShowAddConditionArea.Text = Convert.ToString(dts.Rows(0)("ConditionFieldsText"))
            'txtShowAddConditionArea.Attributes.Add("readonly", "readonly")
            txtShowAddConditionArea.Text = txtShowAddConditionArea.Text.Replace("'", "''")
            'txtAddConditionArea.Attributes.Add("readonly", "readonly")
            txtAddConditionArea.Text = Convert.ToString(dts.Rows(0)("ConditionFields"))
            txtAddConditionArea.Text = txtAddConditionArea.Text.Replace("'", "''")
            isSecCreator_CheckedChanged(isSecCreator, New EventArgs())
            If isSecCreator.Checked = True Then
                ddlSecSourceCreator.SelectedValue = dts.Rows(0)("Creator").ToString()
                ddlSecSourceCreator_SelectedIndexChanged(ddlSecSourceCreator, New EventArgs())
                ddlSecSourceCreatorValue.SelectedValue = dts.Rows(0)("RoleName").ToString()
            Else
                ddlCreator.SelectedValue = dts.Rows(0)("Creator").ToString()
                ddlCreator_SelectedIndexChanged(ddlCreator, New EventArgs())
                ddlCreatorValue.SelectedValue = dts.Rows(0)("RoleName").ToString()

            End If
            ddlCreateEvent_SelectedIndexChanged(ddlCreateEvent, New EventArgs())
            'txtschtime.Text = dts.Rows(0)("ScheduleTime").ToString()
            Dim time As String() = dts.Rows(0)("ScheduleTime").ToString().Split("|")
            If time.Length > 0 Then
                txtMM.Text = time(0).ToString()
                If time.Length > 1 Then
                    txtDD.Text = time(1).ToString()
                Else
                    txtDD.Text = ""
                End If
                If time.Length > 2 Then
                    txtWW.Text = time(2).ToString()
                Else
                    txtWW.Text = ""
                End If
                If time.Length > 3 Then
                    txtHH.Text = time(3).ToString()
                Else
                    txtHH.Text = ""
                End If
                If time.Length > 3 Then
                    txtMN.Text = time(4).ToString()
                Else
                    txtMN.Text = ""
                End If

            Else
                txtMM.Text = ""
                txtDD.Text = ""
                txtWW.Text = ""
                txtHH.Text = ""
                txtMN.Text = ""
            End If


            Dim str As String = Convert.ToString(dts.Rows(0)("filterFields"))
            If str.Length > 0 Then
                For Each li As ListItem In chkCommanFields.Items
                    If str.ToString().Contains(li.Value) Then
                        li.Selected = True
                    Else
                        li.Selected = False
                    End If
                Next
            End If

        End If
        Up_pnlAutoConfig.Update()
        MP_AutoConfig.Show()
    End Sub
    Protected Sub btnAddChildFields_Click(sender As Object, e As ImageClickEventArgs)
        'hdnAction.Value = "CHILD"
        ViewState("ACTION") = "CHILD"
        Dim btnDetails As ImageButton = TryCast(sender, ImageButton)
        Dim row As GridViewRow = DirectCast(btnDetails.NamingContainer, GridViewRow)
        Dim Formid As Integer = Convert.ToString(Me.gvData.DataKeys(row.RowIndex).Value)
        ViewState("TID") = Formid
        ViewState("FORMNAME") = row.Cells(3).Text
        Dim da As New SqlDataAdapter("", con)
        da.SelectCommand.CommandType = CommandType.Text
        da.SelectCommand.CommandText = "select dropdown from mmm_mst_fields where documenttype in (select targetdoc from mmm_schdoc_main where tid=" & Formid & " and eid=" & Session("EID") & ") and eid=" & Session("EID") & " and FieldType='Child Item' and isActive=1;select dropdown from mmm_mst_fields where documenttype in (select sourcedoc from mmm_schdoc_main where tid=" & Formid & " and eid=" & Session("EID") & ") and eid=" & Session("EID") & " and FieldType='Child Item' and isActive=1"

        Dim dts As New DataSet
        da.Fill(dts, "ChildMAP")
        If dts.Tables("ChildMAP").Rows.Count > 0 Then
            ddlTargetChildDoc.DataSource = dts.Tables("ChildMAP")
            ddlTargetChildDoc.DataTextField = "dropdown"
            ddlTargetChildDoc.DataValueField = "dropdown"
            ddlTargetChildDoc.DataBind()
            ddlTargetChildDoc.Items.Insert("0", New ListItem("SELECT"))
        Else
            If ddlTargetChildDoc.Items.Count > 0 Then
                ddlTargetChildDoc.Items.Clear()
                ddlTargetChildDoc.Items.Insert("0", New ListItem("SELECT"))
            End If
        End If
        If dts.Tables("ChildMAP1").Rows.Count > 0 Then
            ddlSourceChildDoc.DataSource = dts.Tables("ChildMAP1")
            ddlSourceChildDoc.DataTextField = "dropdown"
            ddlSourceChildDoc.DataValueField = "dropdown"
            ddlSourceChildDoc.DataBind()
            ddlSourceChildDoc.Items.Insert("0", New ListItem("SELECT"))
        Else
            If ddlSourceChildDoc.Items.Count > 0 Then
                ddlSourceChildDoc.Items.Clear()
                ddlSourceChildDoc.Items.Insert("0", New ListItem("SELECT"))
            End If
        End If
        ddlChildMappingType.SelectedIndex = 0
        ddlChildMappingType_SelectedIndexChanged(ddlChildMappingType, New EventArgs())
        BindConfigChildDetails()
        UP_AutoConfigChilddtlMappingChild.Update()
        Me.MP_AutoConfigChilddtlMapping.Show()
    End Sub
    Protected Sub ddlSourceChildDoc_SelectedIndexChanged(sender As Object, e As EventArgs)
        If ddlSourceChildDoc.SelectedIndex <> 0 Then
            Dim da As SqlDataAdapter = New SqlDataAdapter("", con)
            da.SelectCommand.CommandType = CommandType.Text
            da.SelectCommand.CommandText = "select displayname,fieldmapping from mmm_mst_fields where DocumentType='" & ddlSourceChildDoc.SelectedItem.Text.Trim() & "' and eid=" & Session("EID")
            Dim dtval As New DataTable
            da.Fill(dtval)
            If dtval.Rows.Count > 0 Then
                ddlChildSourceMapping.DataSource = dtval
                ddlChildSourceMapping.DataTextField = "displayname"
                ddlChildSourceMapping.DataValueField = "fieldmapping"
                ddlChildSourceMapping.DataBind()
                ddlChildSourceMapping.Items.Insert("0", New ListItem("SELECT"))
            Else
                If ddlChildSourceMapping.Items.Count > 0 Then
                    ddlChildSourceMapping.Items.Clear()
                    ddlChildSourceMapping.Items.Insert("0", New ListItem("SELECT"))
                End If
            End If
        Else
            If ddlChildSourceMapping.Items.Count > 0 Then
                ddlChildSourceMapping.Items.Clear()
                ddlChildSourceMapping.Items.Insert("0", New ListItem("SELECT"))
            End If
        End If
    End Sub
    Protected Sub ddlTargetChildDoc_SelectedIndexChanged(sender As Object, e As EventArgs)
        If ddlTargetChildDoc.SelectedIndex <> 0 Then
            Dim da As SqlDataAdapter = New SqlDataAdapter("", con)
            da.SelectCommand.CommandType = CommandType.Text
            da.SelectCommand.CommandText = "select displayname,fieldmapping from mmm_mst_fields where DocumentType='" & ddlTargetChildDoc.SelectedItem.Text.Trim() & "' and eid=" & Session("EID")
            Dim dtval As New DataTable
            da.Fill(dtval)
            If dtval.Rows.Count > 0 Then
                ddlChildTargetMapping.DataSource = dtval
                ddlChildTargetMapping.DataTextField = "displayname"
                ddlChildTargetMapping.DataValueField = "fieldmapping"
                ddlChildTargetMapping.DataBind()
                ddlChildTargetMapping.Items.Insert("0", New ListItem("SELECT"))
            Else
                If ddlChildTargetMapping.Items.Count > 0 Then
                    ddlChildTargetMapping.Items.Clear()
                    ddlChildTargetMapping.Items.Insert("0", New ListItem("SELECT"))
                End If
            End If
        Else
            If ddlChildTargetMapping.Items.Count > 0 Then
                ddlChildTargetMapping.Items.Clear()
                ddlChildTargetMapping.Items.Insert("0", New ListItem("SELECT"))
            End If
        End If

    End Sub
    Protected Sub btnAddFields_Click(sender As Object, e As ImageClickEventArgs)
        gvmap.DataSource = Nothing
        gvmap.DataBind()
        'hdnAction.Value = "MAIN"
        ViewState("ACTION") = "MAIN"
        btnSaveDtl.Text = "SAVE"

        txtConfigName.ReadOnly = True
        Dim btnDetails As ImageButton = TryCast(sender, ImageButton)
        Dim row As GridViewRow = DirectCast(btnDetails.NamingContainer, GridViewRow)
        Dim Formid As Integer = Convert.ToString(Me.gvData.DataKeys(row.RowIndex).Value)
        ViewState("TID") = Formid
        ViewState("FORMNAME") = row.Cells(3).Text
        ViewState("SFORMNAME") = row.Cells(2).Text
        Dim da As New SqlDataAdapter("", con)
        da.SelectCommand.CommandType = CommandType.Text
        If row.Cells(2).Text.ToUpper = "USER" Then
            da.SelectCommand.CommandText = "select displayname,fieldmapping from mmm_mst_fields where documenttype in (select sourcedoc from mmm_schdoc_main where tid=" & ViewState("TID") & " and eid=" & Session("EID") & ") and eid=" & Session("EID") & " and isactive=1 union all select name as displayname,name as fieldMapping from sys.columns where object_id=object_id('mmm_mst_user') and name not like'fld%';select displayname,fieldmapping from mmm_mst_fields where documenttype in (select Targetdoc from mmm_schdoc_main where tid=" & ViewState("TID") & " and eid=" & Session("EID") & ") and eid=" & Session("EID") & " and isactive=1;select sourcedoc,'Primary' as val from mmm_schdoc_main where tid=" & ViewState("TID") & " and eid=" & Session("EID") & " and isactive=1 union all select  SecondarySourceDoc,'Secondary' as val from mmm_schdoc_main  where tid=" & ViewState("TID") & " and eid=" & Session("EID") & " and SecondarySourceDoc<>'' and isactive=1;select Targetdoc from mmm_schdoc_main where tid=" & ViewState("TID") & " and eid=" & Session("EID") & ";"
        ElseIf row.Cells(3).Text.ToUpper = "USER" Then
            da.SelectCommand.CommandText = "select displayname,fieldmapping from mmm_mst_fields where documenttype in (select sourcedoc from mmm_schdoc_main where tid=" & ViewState("TID") & " and eid=" & Session("EID") & ") and eid=" & Session("EID") & " and isactive=1;select displayname,fieldmapping from mmm_mst_fields where documenttype in (select Targetdoc from mmm_schdoc_main where tid=" & ViewState("TID") & " and eid=" & Session("EID") & ") and eid=" & Session("EID") & " and isactive=1 union all select name as displayname,name as fieldMapping from sys.columns where object_id=object_id('mmm_mst_user') and name not like'fld%' ;select sourcedoc,'Primary' as val from mmm_schdoc_main where tid=" & ViewState("TID") & " and eid=" & Session("EID") & " and isactive=1 union all select  SecondarySourceDoc,'Secondary' as val from mmm_schdoc_main  where tid=" & ViewState("TID") & " and eid=" & Session("EID") & " and SecondarySourceDoc<>'' and isactive=1;select Targetdoc from mmm_schdoc_main where tid=" & ViewState("TID") & " and eid=" & Session("EID") & ";"
        Else
            da.SelectCommand.CommandText = "select displayname,fieldmapping from mmm_mst_fields where documenttype in (select sourcedoc from mmm_schdoc_main where tid=" & ViewState("TID") & " and eid=" & Session("EID") & ") and eid=" & Session("EID") & " and isactive=1;select displayname,fieldmapping from mmm_mst_fields where documenttype in (select Targetdoc from mmm_schdoc_main where tid=" & ViewState("TID") & " and eid=" & Session("EID") & ") and eid=" & Session("EID") & " and isactive=1;select sourcedoc,'Primary' as val from mmm_schdoc_main where tid=" & ViewState("TID") & " and eid=" & Session("EID") & " and isactive=1 union all select  SecondarySourceDoc,'Secondary' as val from mmm_schdoc_main  where tid=" & ViewState("TID") & " and eid=" & Session("EID") & " and SecondarySourceDoc<>'' and isactive=1;select Targetdoc from mmm_schdoc_main where tid=" & ViewState("TID") & " and eid=" & Session("EID") & ";"
        End If

        Dim dts As New DataSet
        da.Fill(dts, "MAP")
        If dts.Tables("MAP").Rows.Count > 0 Then
            ddlSourceFldMapping.DataSource = dts.Tables("MAP")
            ddlSourceFldMapping.DataTextField = "displayname"
            ddlSourceFldMapping.DataValueField = "fieldmapping"
            ddlSourceFldMapping.DataBind()
            ddlSourceFldMapping.Items.Insert("0", New ListItem("SELECT"))
            ddlSourceFldMapping.Items.Insert("1", New ListItem("TID"))
        Else
            If ddlSourceFldMapping.Items.Count > 0 Then
                ddlSourceFldMapping.Items.Clear()
                ddlSourceFldMapping.Items.Insert("0", New ListItem("SELECT"))
            End If
        End If
        If dts.Tables("MAP1").Rows.Count > 0 Then
            ddltargetfldMapping.DataSource = dts.Tables("MAP1")
            ddltargetfldMapping.DataTextField = "displayname"
            ddltargetfldMapping.DataValueField = "fieldmapping"
            ddltargetfldMapping.DataBind()
            ddltargetfldMapping.Items.Insert("0", New ListItem("SELECT"))
        Else
            If ddltargetfldMapping.Items.Count > 0 Then
                ddltargetfldMapping.Items.Clear()
                ddltargetfldMapping.Items.Insert("0", New ListItem("SELECT"))
            End If
        End If
        If dts.Tables("MAP2").Rows.Count > 0 Then
            ddlMainSource.DataSource = dts.Tables("MAP2")
            ddlMainSource.DataTextField = "val"
            ddlMainSource.DataValueField = "sourcedoc"
            ddlMainSource.DataBind()
        Else
            If ddlMainSource.Items.Count > 0 Then
                ddlMainSource.Items.Clear()
                ddlMainSource.Items.Insert("0", New ListItem("SELECT"))
            End If
        End If
        If dts.Tables("MAP3").Rows.Count > 0 Then
            ddlMainTarget.DataSource = dts.Tables("MAP3")
            ddlMainTarget.DataTextField = "Targetdoc"
            ddlMainTarget.DataValueField = "Targetdoc"
            ddlMainTarget.DataBind()
        Else
            If ddlMainTarget.Items.Count > 0 Then
                ddlMainTarget.Items.Clear()
                ddlMainTarget.Items.Insert("0", New ListItem("SELECT"))
            End If
        End If

        ddlMappingType.SelectedIndex = 0
        ddlMappingType_SelectedIndexChanged(ddlMappingType, New EventArgs())
        BindConfigDetails()
        Up_pnlAutoConfigdtlMapping.Update()
        MP_AutoConfigdtlMapping.Show()

    End Sub

    Protected Sub ClearRecords()



    End Sub

    Function chkvalidation() As KeyValuePair(Of Boolean, String)
        Dim type As Boolean = True
        Dim sb As New StringBuilder
        If txtConfigName.Text = String.Empty Then
            sb.Append("Please Fill Configuration Name ,")
        End If
        If txtNoRows.Text = String.Empty Then
            sb.Append("Please Fill No of Rows ,")
        End If

        If ddlSourceForm.SelectedIndex = 0 Then
            sb.Append("Please Fill Source Document ,")
        End If
        Try
            If ddlFormSourceType.SelectedItem.Text = "SELECT" Then
                sb.Append("Please Fill Source Document Type ,")
            End If
        Catch ex As Exception
            sb.Append("Please Fill Source Document Type ,")
        End Try

        If ddlTargetDoc.SelectedIndex = 0 Then
            sb.Append("Please Fill Target Document ,")
        End If
        Try
            If ddlTargetType.SelectedItem.Text = "SELECT" Then
                sb.Append("Please Fill Target Document Type ,")
            End If
        Catch ex As Exception
            sb.Append("Please Fill Target Document Type ,")
        End Try


        If ddlCreateEvent.SelectedIndex = 0 Then
            sb.Append("Please Fill Create Event ,")
        ElseIf ddlCreateEvent.SelectedIndex = 1 Then
            If txtMM.Text = String.Empty Then
                sb.Append("Please Fill month in Schedule Time if you want to left blank please fill * ,")
            End If
            If txtDD.Text = String.Empty Then
                sb.Append("Please Fill date in Schedule Time if you want to left blank please fill * ,")
            End If
            If txtWW.Text = String.Empty Then
                sb.Append("Please Fill week in Schedule Time if you want to left blank please fill * ,")
            End If
            If txtDD.Text = String.Empty Then
                sb.Append("Please Fill month in Schedule Time if you want to left blank please fill * ,")
            End If
            If txtHH.Text = String.Empty Then
                sb.Append("Please Fill hours in Schedule Time if you want to left blank please fill * ,")
            End If
            If txtMN.Text = String.Empty Then
                sb.Append("Please Fill minutes in Schedule Time if you want to left blank please fill * ,")
            End If
        ElseIf ddlCreateEvent.SelectedIndex = 3 Then
            If ddlWFStatus.SelectedIndex = 0 Then
                sb.Append("Please Select Work Flow Status, ")
            End If
        End If
        'If ddlWFStatus.SelectedIndex = 0 Then
        '    sb.Append("Please Select work Flow Status ,")
        'End If
        If isSecCreator.Checked = True Then
            If ddlSecSourceCreator.SelectedIndex = 0 Then
                sb.Append("Please Select Secondary Creator ,")
            End If
            If ddlSecSourceCreatorValue.SelectedIndex = 0 Or ddlSecSourceCreatorValue.SelectedIndex = -1 Then
                sb.Append("Please Select Secondary Creator value ,")
            End If
        Else
            If ddlCreator.SelectedIndex = 0 Then
                sb.Append("Please Select Creator ,")
            End If
            If ddlCreatorValue.SelectedIndex = 0 Then
                sb.Append("Please Select Creator value ,")
            End If
        End If

        If sb.Length > 1 Then
            type = False
        End If
        Return New KeyValuePair(Of Boolean, String)(type, sb.ToString())
    End Function

    Function chkdtlvalidation() As KeyValuePair(Of Boolean, String)
        Dim type As Boolean = True
        Dim sb As New StringBuilder
        ViewState("SFLDMAPPING") = Nothing
        If ddlMappingType.SelectedIndex = 0 Then
            sb.Append("Please Select Mapping Type ,")
        ElseIf ddlMappingType.SelectedIndex = 1 Then
            If txtFIX.Text = String.Empty Then
                sb.Append("Please Fill Fixed Value ,")
            Else
                ViewState("SFLDMAPPING") = txtFIX.Text
            End If
        ElseIf ddlMappingType.SelectedIndex = 2 Then
            If ddlSourceFldMapping.SelectedIndex = 0 Then
                sb.Append("Please Select Copy Value ,")
            Else
                ViewState("SFLDMAPPING") = ddlSourceFldMapping.SelectedValue
            End If
        ElseIf ddlMappingType.SelectedIndex = 3 Then
            If txtMappingFormula.Text = String.Empty Then
                sb.Append("Please Fill Mapping Formula Value ,")
            Else
                ViewState("SFLDMAPPING") = txtMappingFormula.Text
            End If
        End If
        If sb.Length > 1 Then
            type = False
        End If
        Return New KeyValuePair(Of Boolean, String)(type, sb.ToString())
    End Function

    Protected Sub EditRecord(sender As Object, e As EventArgs)
        If ViewState("ACTION").ToString().Trim().ToUpper = "MAIN" Then
            txtMappingFormula.Text = txtcontionadvform.Text.Trim()
            Up_pnlAutoConfigdtlMapping.Update()
            modalpopupadvformula.Hide()
        Else
            txtChildFormulaMapping.Text = txtcontionadvform.Text.Trim()
            UP_AutoConfigChilddtlMappingChild.Update()
            modalpopupadvformula.Hide()
        End If

    End Sub
    Protected Sub btnSaveDtl_Click(sender As Object, e As EventArgs)

        Dim pair As KeyValuePair(Of Boolean, String) = chkdtlvalidation()
        If pair.Key = False Then
            lblmsgred.Text = pair.Value
            Exit Sub
        End If
        If btnSaveDtl.Text.ToUpper = "SAVE" Then
            Dim da As New SqlDataAdapter("", con)
            da.SelectCommand.CommandType = CommandType.Text
            da.SelectCommand.CommandText = " IF EXISTS (SELECT * FROM mmm_schdoc_dtl  WHERE TfldMapping = '" & ddltargetfldMapping.SelectedValue & "' and eid=" & Session("EID") & " and mtid=" & ViewState("TID") & " and MappingSource='" & ddlMainSource.SelectedItem.Text & "')	  update mmm_schdoc_dtl set SfldMapping='" & ViewState("SFLDMAPPING") & "', TfldMapping='" & ddltargetfldMapping.SelectedValue & "',Mtype='" & ddlMappingType.SelectedValue & "',Remarks='" & txtRemarks.Text & "',TargetFieldName='" & ddltargetfldMapping.SelectedItem.Text & "',lastupdate=getdate() where tid in (SELECT tid FROM mmm_schdoc_dtl  WHERE TfldMapping = '" & ddltargetfldMapping.SelectedValue & "' and eid=" & Session("EID") & " and mtid=" & ViewState("TID") & " and MappingSource='" & ddlMainSource.SelectedItem.Text & "') and eid=" & Session("EID") & " and mtid=" & ViewState("TID") & " 	  else insert into mmm_schdoc_dtl (Mtid,EID,SfldMapping,TfldMapping,Mtype,Remarks,lastupdate,TargetFieldName,MappingSource) values (" & ViewState("TID") & "," & Session("EID") & ",'" & ViewState("SFLDMAPPING").ToString() & "','" & ddltargetfldMapping.SelectedValue & "','" & ddlMappingType.SelectedValue & "','" & txtRemarks.Text & "',getdate(),'" & ddltargetfldMapping.SelectedItem.Text & "','" & ddlMainSource.SelectedItem.Text & "') select case(SCOPE_IDENTITY()) when null then SCOPE_IDENTITY() else (SELECT tid FROM mmm_schdoc_dtl  WHERE TfldMapping = '" & ddltargetfldMapping.SelectedValue & "' and eid=" & Session("EID") & " and mtid=" & ViewState("TID") & " and MappingSource='" & ddlMainSource.SelectedItem.Text & "') end"
            If con.State = ConnectionState.Closed Then
                con.Open()
            End If
            Dim id As Integer = da.SelectCommand.ExecuteScalar()
            If ddlMappingType.SelectedIndex = 3 Then
                da.SelectCommand.CommandText = "Delete  from [MMM_MST_AdvFormulaRelation] where eid=" & Session("EID") & " and FormulaID=" & id & " and Type='MAIN'"
                If con.State <> ConnectionState.Open Then
                    con.Open()
                End If

                da.SelectCommand.ExecuteNonQuery()

                For Each gvrow As GridViewRow In gvmap.Rows

                    da.SelectCommand.Parameters.Clear()
                    da.SelectCommand.CommandText = "USP_insertAdvFormulaRelation"
                    da.SelectCommand.CommandType = CommandType.StoredProcedure
                    da.SelectCommand.Parameters.AddWithValue("@eid", Session("EID"))
                    da.SelectCommand.Parameters.AddWithValue("@formulaid", id)
                    da.SelectCommand.Parameters.AddWithValue("@sourcetype", gvrow.Cells(1).Text.ToString())
                    da.SelectCommand.Parameters.AddWithValue("@sourcename", gvrow.Cells(2).Text.ToString())
                    da.SelectCommand.Parameters.AddWithValue("@s_relationidentifierfiled", gvrow.Cells(5).Text.ToString())
                    da.SelectCommand.Parameters.AddWithValue("@targettype", gvrow.Cells(3).Text.ToString())
                    da.SelectCommand.Parameters.AddWithValue("@targetname", gvrow.Cells(4).Text.ToString())
                    da.SelectCommand.Parameters.AddWithValue("@t_relationidentifierfield", gvrow.Cells(6).Text.ToString())
                    da.SelectCommand.Parameters.AddWithValue("@sortingfield", gvrow.Cells(7).Text.ToString())
                    da.SelectCommand.Parameters.AddWithValue("@uid", Session("UID"))
                    da.SelectCommand.Parameters.AddWithValue("@Type", "MAIN")
                    If con.State <> ConnectionState.Open Then
                        con.Open()
                    End If
                    da.SelectCommand.ExecuteNonQuery()
                Next
            End If
        Else
            Dim da As New SqlDataAdapter("", con)
            da.SelectCommand.CommandType = CommandType.Text
            da.SelectCommand.CommandText = "update mmm_schdoc_dtl set SfldMapping='" & ViewState("SFLDMAPPING") & "', TfldMapping='" & ddltargetfldMapping.SelectedValue & "',Mtype='" & ddlMappingType.SelectedValue & "',Remarks='" & txtRemarks.Text & "',TargetFieldName='" & ddltargetfldMapping.SelectedItem.Text & "',lastupdate=getdate() where tid ="
            If con.State = ConnectionState.Closed Then
                con.Open()
            End If
            da.SelectCommand.ExecuteNonQuery()
        End If
        BindConfigDetails()
    End Sub

    Protected Sub btnAddFieldsMapping_Click(sender As Object, e As ImageClickEventArgs)
        gvmap.DataSource = Nothing
        gvmap.DataBind()
        tvadvform.Nodes.Clear()
        Dim button As ImageButton = CType(sender, ImageButton)
        Dim buttonId As String = button.ID
        If button.ID.ToString() = "btnAddFieldsMapping" Then
            'hdnAction.Value = "MAIN"
            ViewState("ACTION") = "MAIN"
        End If
        If ViewState("ACTION").ToString().Trim().ToUpper = "CHILD" Then
            If ddlTargetChildDoc.SelectedIndex = 0 Then
                lblChildRedMsg.Text = "Please Select Target Document"
                ddlTargetChildDoc.Focus()
                Exit Sub
            Else
                ViewState("SFORMNAME") = ddlSourceChildDoc.SelectedItem.Text.Trim()
            End If
        Else
            ViewState("SFORMNAME") = ddlMainSource.SelectedValue
        End If

        Dim da As New SqlDataAdapter("", con)
        da.SelectCommand.CommandText = "Select  upper(formname)[Formname],formid,formtype from mmm_mst_forms where eid=" & Session("EID") & "  order by formname"
        da.SelectCommand.CommandType = CommandType.Text
        Dim dt As New DataTable
        da.Fill(dt)
        ddladvfodoctype.DataSource = dt
        ddladvfodoctype.DataTextField = "formname"
        ddladvfodoctype.DataValueField = "formid"
        ddladvfodoctype.DataBind()
        'ddladvfodoctype.SelectedItem.Text = ViewState("FORMNAME")
        ddladvfodoctype.SelectedIndex = ddladvfodoctype.Items.IndexOf(ddladvfodoctype.Items.FindByText(Convert.ToString(ViewState("SFORMNAME")).ToUpper.Trim()))
        Dim drr() As DataRow

        drr = dt.Select("formname='" & ViewState("SFORMNAME") & "'")
        'ddladvfoftype.SelectedItem.Text = Convert.ToString(drr(0).Item("FormType"))
        ddladvfoftype.SelectedIndex = ddladvfoftype.Items.IndexOf(ddladvfoftype.Items.FindByText(Convert.ToString(drr(0).Item("FormType"))))
        If Not ViewState("SFORMNAME") Is Nothing Then
            Call LoadWorkGroupTree(ViewState("SFORMNAME"))
        End If
        updpaneladvanceformula.Update()
        modalpopupadvformula.Show()
    End Sub

    'Code here starts for advance formula
    Private Sub LoadWorkGroupTree(ByVal doctype As String)

        ViewState("cnt") = 0
        Dim conStr As String = System.Configuration.ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = Nothing
        Dim cmd As SqlCommand = Nothing
        Dim da As SqlDataAdapter = Nothing
        Try
            Dim table As New DataTable()
            con = New SqlConnection(conStr)
            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            Dim sql As String = "select fieldid,displayname,fieldmapping,dropdowntype,dropdown from mmm_mst_fields where EID=" & Session("EID") & " and documenttype='" & doctype.ToString().Trim() & "' order by displayname"
            cmd = New SqlCommand(sql, con)
            da = New SqlDataAdapter(cmd)
            Dim ds As New DataSet
            da.Fill(ds, "table")

            Dim bool As Boolean = False
            For Each node As TreeNode In tvadvform.Nodes
                If node.Text.Trim() = doctype.Trim.ToString().Trim() Then
                    bool = True
                    Exit For
                End If
            Next
            If bool = False Then
                Dim masterNode As New TreeNode(doctype.Trim.ToString())

                masterNode.Value = doctype.Trim.ToString()
                masterNode.ImageUrl = "images/Redp.png"
                masterNode.SelectAction = TreeNodeSelectAction.Expand
                'masterNode.NavigateUrl = "javascript:setCaret('" & masterNode.Text.ToString() & "')"
                masterNode.NavigateUrl = "javascript:setCaret('" & "Form." & masterNode.Text.ToString() & "')"
                tvadvform.Nodes.Add(masterNode)


                'tv.Attributes.Add("onclick", "return setCaret(" & masterNode.Text.ToString() & ");")
                Dim view As New DataView(ds.Tables("table"))
                For Each row As DataRowView In view
                    ViewState("cnt") = 0
                    If UCase(row.Item("dropdowntype").ToString) = "MASTER VALUED" Then
                        Dim n As New TreeNode()
                        n.Text = row.Item("displayname").ToString()
                        n.Value = row.Item("fieldmapping")
                        n.ImageUrl = "+"
                        n.ImageUrl = "images/redp.png"
                        n.NavigateUrl = "javascript:setCaret('" & "Form." & masterNode.Text.ToString() & "." & n.Text.ToString() & "')"
                        masterNode.ChildNodes.Add(n)
                        'tv.Attributes.Add("onclick", "return setCaret(" & n.Text.ToString() & ");")
                        LoadDocTree(row.Item("dropdown").ToString, n)
                    Else
                        Dim n As New TreeNode()
                        n.Text = row.Item("displayname").ToString()
                        n.Value = row.Item("fieldmapping")
                        n.ImageUrl = "images/bluep.png"
                        n.NavigateUrl = "javascript:setCaret('" & "Form." & masterNode.Text.ToString() & "." & n.Text.ToString() & "')"
                        masterNode.ChildNodes.Add(n)
                        ' tv.Attributes.Add("onclick", "return setCaret(" & n.Text.ToString() & ");")
                    End If

                Next
            End If


        Catch ex As Exception
            lblMsg.Text = "SERVER ERROR: PLEASE TRY AFTER SOME TIME"
        Finally
            If Not con Is Nothing Then
                con.Close()
                con.Dispose()
            End If
            If Not da Is Nothing Then
                da.Dispose()
            End If
            If Not cmd Is Nothing Then
                cmd.Dispose()
            End If

        End Try
    End Sub

    Protected Sub ddlChildMappingType_SelectedIndexChanged(sender As Object, e As EventArgs)
        If ddlChildMappingType.SelectedIndex = 0 Then
            lblChildMappingType.InnerText = "Please Select Mapping Type"
            txtChildFixMapping.Visible = False
            txtChildFormulaMapping.Visible = False
            ddlChildSourceMapping.Visible = False
            btnChildAddFieldsMapping.Visible = False
        ElseIf ddlChildMappingType.SelectedIndex = 1 Then
            lblChildMappingType.InnerText = "Fix Valued"
            txtChildFixMapping.Visible = True
            txtChildFormulaMapping.Visible = False
            ddlChildSourceMapping.Visible = False
            btnChildAddFieldsMapping.Visible = False
        ElseIf ddlChildMappingType.SelectedIndex = 2 Then
            lblChildMappingType.InnerText = "Copy Valued"
            txtChildFixMapping.Visible = False
            txtChildFormulaMapping.Visible = False
            ddlChildSourceMapping.Visible = True
            btnChildAddFieldsMapping.Visible = False
        ElseIf ddlChildMappingType.SelectedIndex = 3 Then
            lblChildMappingType.InnerText = "Formula Valued"
            txtChildFixMapping.Visible = False
            txtChildFormulaMapping.Visible = True
            ddlChildSourceMapping.Visible = False
            btnChildAddFieldsMapping.Visible = True
        End If
    End Sub
    Protected Sub ddlMappingType_SelectedIndexChanged(sender As Object, e As EventArgs)
        If ddlMappingType.SelectedIndex = 0 Then
            lblMapping.InnerText = "Please Select Mapping Type"
            txtFIX.Visible = False
            txtMappingFormula.Visible = False
            ddlSourceFldMapping.Visible = False
            btnAddFieldsMapping.Visible = False
        ElseIf ddlMappingType.SelectedIndex = 1 Then
            lblMapping.InnerText = "Fix Valued"
            txtFIX.Visible = True
            txtMappingFormula.Visible = False
            ddlSourceFldMapping.Visible = False
            btnAddFieldsMapping.Visible = False
        ElseIf ddlMappingType.SelectedIndex = 2 Then
            lblMapping.InnerText = "Copy Valued"
            txtFIX.Visible = False
            txtMappingFormula.Visible = False
            ddlSourceFldMapping.Visible = True
            btnAddFieldsMapping.Visible = False
        ElseIf ddlMappingType.SelectedIndex = 3 Then
            lblMapping.InnerText = "Formula Valued"
            txtFIX.Visible = False
            txtMappingFormula.Visible = True
            ddlSourceFldMapping.Visible = False
            btnAddFieldsMapping.Visible = True
        End If

    End Sub

    Protected Sub DeleteHitUser(ByVal sender As Object, ByVal e As System.EventArgs)
        Try

            Dim btnDetails As ImageButton = TryCast(sender, ImageButton)
            Dim row As GridViewRow = DirectCast(btnDetails.NamingContainer, GridViewRow)
            Dim Formid As Integer = Convert.ToString(Me.gvUsers.DataKeys(row.RowIndex).Value)
            Dim da As New SqlDataAdapter("", con)
            da.SelectCommand.CommandType = CommandType.Text
            da.SelectCommand.CommandText = "delete from mmm_schdoc_dtl where tid=" & Formid & " and eid=" & Session("EID")
            If con.State = ConnectionState.Closed Then
                con.Open()
            End If
            da.SelectCommand.ExecuteNonQuery()
            BindConfigDetails()
        Catch ex As Exception

        End Try
    End Sub

    Protected Sub DeleteHitUserChild(ByVal sender As Object, ByVal e As System.EventArgs)
        Try

            Dim btnDetails As ImageButton = TryCast(sender, ImageButton)
            Dim row As GridViewRow = DirectCast(btnDetails.NamingContainer, GridViewRow)
            Dim Formid As Integer = Convert.ToString(Me.GridView1.DataKeys(row.RowIndex).Value)
            Dim da As New SqlDataAdapter("", con)
            da.SelectCommand.CommandType = CommandType.Text
            da.SelectCommand.CommandText = "delete from mmm_schdoc_child where tid=" & Formid & " and eid=" & Session("EID")
            If con.State = ConnectionState.Closed Then
                con.Open()
            End If
            da.SelectCommand.ExecuteNonQuery()
            BindConfigChildDetails()
        Catch ex As Exception

        End Try
    End Sub

    Protected Sub BindConfigDetails()
        Dim da As New SqlDataAdapter("", con)
        Dim dt As New DataTable
        da.SelectCommand.CommandType = CommandType.Text
        da.SelectCommand.CommandText = "select tid,TargetFieldName,SfldMapping=case when len(SfldMapping)>10 then substring(SfldMapping,0,10)+'...' else SfldMapping end,SfldMapping as [description],Remarks,Mtype from mmm_schdoc_dtl where eid=" & Session("EID") & " and mtid=" & ViewState("TID")
        If con.State = ConnectionState.Closed Then
            con.Open()
        End If
        da.Fill(dt)
        If dt.Rows.Count > 0 Then
            gvUsers.DataSource = dt
            gvUsers.DataBind()
        Else
            gvUsers.DataSource = Nothing
            gvUsers.DataBind()
        End If
    End Sub
    Protected Sub BindConfigChildDetails()
        Dim da As New SqlDataAdapter("", con)
        Dim dt As New DataTable
        da.SelectCommand.CommandType = CommandType.Text
        da.SelectCommand.CommandText = "select tid,ChildSourcedoc,ChildTargetdoc,Remarks,MappingType from mmm_schdoc_child where eid=" & Session("EID") & " and ctid=" & ViewState("TID")
        If con.State = ConnectionState.Closed Then
            con.Open()
        End If
        da.Fill(dt)
        If dt.Rows.Count > 0 Then
            GridView1.DataSource = dt
            GridView1.DataBind()
        Else
            GridView1.DataSource = Nothing
            GridView1.DataBind()
        End If
    End Sub
    Protected Sub advanceformulaclose()
        Me.modalpopupadvformula.Hide()
    End Sub
    Protected Sub AddRelation(ByVal sender As Object, ByVal e As System.EventArgs)
        lblmsgFrelation.Text = ""
        btnsavefrelation.Text = "Save"
        lblsd.Text = ddladvfodoctype.SelectedItem.Text.ToString().Trim
        UpdatePanelFormulaRelation.Update()
        Me.modalpopupformularelation.Show()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim da As New SqlDataAdapter("", con)
        Dim ds As New DataSet
        Try

            da.SelectCommand.CommandText = "select upper(displayname)[formname],upper(fieldmapping)[fieldmapping] from mmm_mst_fields where eid=" & Session("Eid") & " and documenttype='" & Trim(ddladvfodoctype.SelectedItem.Text) & "' order by displayname"
            da.Fill(ds, "fields")
            If ds.Tables("fields").Rows.Count > 0 Then
                ddlsdf.DataSource = ds.Tables("fields")
                ddlsdf.DataTextField = "formname"
                ddlsdf.DataValueField = "fieldmapping"
                ddlsdf.DataBind()
                ddlsdf.Items.Insert(0, "SELECT")
            End If

        Catch ex As Exception
            lblMsg.Text = "SERVER ERROR: PLEASE TRY AFTER SOME TIME"
        Finally
            If Not con Is Nothing Then
                con.Close()
            End If
            If Not da Is Nothing Then
                da.Dispose()
            End If

        End Try
    End Sub
    Protected Sub gvmapOnRowDataBound(sender As Object, e As GridViewRowEventArgs)
        If e.Row.RowType = DataControlRowType.DataRow Then
            Dim item As String = e.Row.Cells(3).Text
            For Each button As ImageButton In e.Row.Cells(0).Controls.OfType(Of ImageButton)()
                If button.CommandName = "Delete" Then
                    button.Attributes("onclick") = "if(!confirm('Do you want to delete " + item + "?')){ return false; };"
                End If
            Next
        End If
    End Sub
    Protected Sub gvmapOnRowDeleting(sender As Object, e As GridViewDeleteEventArgs)

        Dim index As Integer = Convert.ToInt32(e.RowIndex)
        ' Dim dt As DataTable = TryCast(Session("datat"), DataTable)
        Dim dt As DataTable = TryCast(ViewState("datat"), DataTable)

        'dt.Rows(index).Delete()
        dt.Rows.RemoveAt(index)

        ViewState("datat") = String.Empty

        Session("datat") = dt
        ViewState("datat") = dt
        bindgridrelation()
    End Sub
    Protected Sub bindgridrelation()
        Dim ds As New DataTable
        If Not ViewState("datat") Is Nothing Then
            ds = DirectCast(ViewState("datat"), DataTable)
        Else
            Dim dt As New DataTable()
            Dim dss As New DataSet()
            Dim dc As New DataColumn("Source Type")
            Dim dc1 As New DataColumn("Source Name")
            Dim dc4 As New DataColumn("Target Name")
            Dim dc2 As New DataColumn("Target Type")
            Dim dc3 As New DataColumn("Source Field")

            Dim dc5 As New DataColumn("Target Field")
            Dim dc6 As New DataColumn("Sorting Field")

            dt.Columns.Add(dc)
            dt.Columns.Add(dc1)
            dt.Columns.Add(dc2)
            dt.Columns.Add(dc4)
            dt.Columns.Add(dc3)

            dt.Columns.Add(dc5)
            dt.Columns.Add(dc6)
            dss.Tables.Add(dt)
            ViewState("datat") = dt
            ds = DirectCast(ViewState("datat"), DataTable)
        End If


        Dim dr As DataRow = ds.NewRow()

        dr(0) = ddladvfoftype.SelectedItem.Text.Trim()
        dr(1) = ddladvfodoctype.SelectedItem.Text.Trim()
        dr(2) = ddlsourcetype.SelectedItem.Text.Trim()
        dr(3) = ddlsourcedoc.SelectedItem.Text.Trim()
        dr(4) = ddlsdf.SelectedItem.Text.Trim()
        dr(5) = ddltf.SelectedItem.Text.Trim()
        dr(6) = ddlsortingfields.SelectedItem.Text.Trim()
        ds.Rows.Add(dr)
        gvmap.DataSource = Nothing
        gvmap.DataBind()
        Session("dsszz") = ds
        ViewState("datat") = ds
        gvmap.DataSource = ds
        gvmap.DataBind()
        updpaneladvanceformula.Update()
    End Sub


    Protected Sub EditRecordFormulaRelation(sender As Object, e As EventArgs)
        Me.modalpopupformularelation.Hide()
        bindgridrelation()

        'CreateTreeView(ddlsourcedoc.SelectedItem.Text.ToString())
        LoadWorkGroupTreeSource(ddlsourcedoc.SelectedItem.Text.ToString())
        updpaneladvanceformula.Update()
    End Sub
    Private Sub LoadWorkGroupTreeSource(ByVal doctype As String)
        ViewState("cnt") = 0
        Dim conStr As String = System.Configuration.ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = Nothing
        Dim cmd As SqlCommand = Nothing
        Dim da As SqlDataAdapter = Nothing
        Try
            Dim table As New DataTable()
            con = New SqlConnection(conStr)
            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            Dim sql As String = "select * from mmm_mst_fields where EID=" & Session("EID") & " and documenttype='" & doctype.ToString().Trim() & "' order by displayname"
            cmd = New SqlCommand(sql, con)
            da = New SqlDataAdapter(cmd)
            Dim ds As New DataSet
            da.Fill(ds, "table")

            Dim masterNode As New TreeNode(doctype.Trim.ToString())

            masterNode.Value = doctype.Trim.ToString()
            masterNode.ImageUrl = "images/Redp.png"
            masterNode.SelectAction = TreeNodeSelectAction.Expand
            tvsource.Nodes.Add(masterNode)

            Dim view As New DataView(ds.Tables("table"))
            For Each row As DataRowView In view
                ViewState("cnt") = 0
                If UCase(row.Item("dropdowntype").ToString) = "MASTER VALUED" Then
                    Dim n As New TreeNode()
                    n.Text = row.Item("displayname").ToString()
                    n.Value = row.Item("fieldmapping")
                    n.ImageUrl = "+"
                    n.ImageUrl = "images/redp.png"
                    n.NavigateUrl = "javascript:setCaret('" & "Ds." & masterNode.Text.ToString() & "." & n.Text.ToString() & "')"
                    masterNode.ChildNodes.Add(n)
                    LoadDocTree(row.Item("dropdown").ToString, n)
                Else
                    Dim n As New TreeNode()
                    n.Text = row.Item("displayname").ToString()
                    n.Value = row.Item("fieldmapping")
                    n.ImageUrl = "images/bluep.png"
                    n.NavigateUrl = "javascript:setCaret('" & "DS." & masterNode.Text.ToString() & "." & n.Text.ToString() & "')"
                    masterNode.ChildNodes.Add(n)

                End If

            Next

        Catch ex As Exception
            lblMsg.Text = "SERVER ERROR: PLEASE TRY AFTER SOME TIME"
        Finally
            If Not con Is Nothing Then
                con.Close()
                con.Dispose()
            End If
            If Not da Is Nothing Then
                da.Dispose()
            End If
            If Not cmd Is Nothing Then
                cmd.Dispose()
            End If

        End Try
    End Sub

    Private Sub LoadDocTree(ByVal dropdown As String, ByRef node As TreeNode)
        If dropdown.Contains("-") Then
            Dim str As String() = dropdown.ToString.Split("-")
            If str.Length > 1 Then
                If ViewState("cnt") < 2 Then


                    Dim ddtype As String = str(1).ToString
                    Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
                    Dim con As New SqlConnection(conStr)
                    Dim da As New SqlDataAdapter("", con)
                    Dim ds As New DataSet
                    Try
                        da.SelectCommand.CommandText = "select fieldid,displayname,fieldmapping,dropdown,dropdowntype from mmm_mst_fields where EID=" & Session("EID") & " and documenttype='" & ddtype.ToString().Trim() & "' order by displayname"
                        da.Fill(ds, "child")
                        If ds.Tables("child").Rows.Count > 0 Then

                            Dim view As New DataView(ds.Tables("child"))
                            For Each row As DataRowView In view
                                Dim n As New TreeNode()
                                If UCase(row("dropdowntype")).ToString = "MASTER VALUED" Then
                                    n.Text = row("displayname").ToString()
                                    n.Value = row("fieldmapping").ToString()
                                    n.ImageUrl = "images/redp.png"

                                    n.NavigateUrl = "javascript:setCaret('" & node.Text.ToString() & "." & n.Text.ToString() & "')"

                                    node.ChildNodes.Add(n)
                                    If UCase(str(1)).ToString = "USER" Then
                                        ViewState("cnt") = ViewState("cnt") + 1
                                    End If
                                    '   LoadDocTree(row.Item("dropdown").ToString, n)
                                Else
                                    n.Text = row("displayname").ToString()
                                    n.Value = row("fieldmapping").ToString()
                                    n.ImageUrl = "images/bluep.png"
                                    n.NavigateUrl = "javascript:setCaret('" & node.Text.ToString() & "." & n.Text.ToString() & "')"
                                    node.ChildNodes.Add(n)
                                End If
                            Next
                        End If
                    Catch ex As Exception
                        lblMsg.Text = "SERVER ERROR: PLEASE TRY AFTER SOME TIME"
                    Finally
                        If Not con Is Nothing Then
                            con.Close()
                        End If
                        If Not da Is Nothing Then
                            da.Dispose()
                        End If

                    End Try

                End If
            End If
        End If

    End Sub
    Protected Sub ddlsourcetype_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlsourcetype.SelectedIndexChanged
        lblmsgFrelation.Text = String.Empty
        If UCase(ddlsourcetype.SelectedItem.Text) = "MASTER" Then
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As New SqlConnection(conStr)

            Dim da As New SqlDataAdapter("", con)
            Dim ds As New DataSet

            Try
                da.SelectCommand.CommandText = "Select  upper(formname)[Formname],formid from mmm_mst_forms where eid=" & Session("EID") & " and formtype='master' order by formname"
                da.Fill(ds, "master")

                If ds.Tables("master").Rows.Count > 0 Then
                    ddlsourcedoc.DataSource = ds.Tables("master")
                    ddlsourcedoc.DataTextField = "formname"
                    ddlsourcedoc.DataValueField = "formid"
                    ddlsourcedoc.DataBind()
                    ddlsourcedoc.Items.Insert(0, "SELECT")
                End If
            Catch ex As Exception
                lblmsgFrelation.Text = "SERVER ERROR: PLEASE TRY AFTER SOME TIME"
            Finally
                If Not con Is Nothing Then
                    con.Close()
                End If
                If Not da Is Nothing Then
                    da.Dispose()
                End If

            End Try

        ElseIf UCase(ddlsourcetype.SelectedItem.Text) = "DOCUMENT" Then
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As New SqlConnection(conStr)

            Dim da As New SqlDataAdapter("", con)
            Dim ds As New DataSet

            Try
                da.SelectCommand.CommandText = "Select upper(formname)[Formname],formid from mmm_mst_forms where eid=" & Session("EID") & " and formtype='DOCUMENT' and formsource='menu driven' order by formname"
                da.Fill(ds, "document")

                If ds.Tables("document").Rows.Count > 0 Then
                    ddlsourcedoc.DataSource = ds.Tables("document")
                    ddlsourcedoc.DataTextField = "formname"
                    ddlsourcedoc.DataValueField = "formid"
                    ddlsourcedoc.DataBind()
                    ddlsourcedoc.Items.Insert(0, "SELECT")
                End If
            Catch ex As Exception
                lblmsgFrelation.Text = "SERVER ERROR: PLEASE TRY AFTER SOME TIME"
            Finally
                If Not con Is Nothing Then
                    con.Close()
                End If
                If Not da Is Nothing Then
                    da.Dispose()
                End If

            End Try
        ElseIf UCase(ddlsourcetype.SelectedItem.Text) = "ACTION DRIVEN" Then
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As New SqlConnection(conStr)

            Dim da As New SqlDataAdapter("", con)
            Dim ds As New DataSet

            Try
                da.SelectCommand.CommandText = "Select  upper(formname)[Formname],formid from mmm_mst_forms where eid=" & Session("EID") & " and formsource='ACTION DRIVEN' order by formname"
                da.Fill(ds, "actiondriven")

                If ds.Tables("actiondriven").Rows.Count > 0 Then
                    ddlsourcedoc.DataSource = ds.Tables("actiondriven")
                    ddlsourcedoc.DataTextField = "formname"
                    ddlsourcedoc.DataValueField = "formid"
                    ddlsourcedoc.DataBind()
                    ddlsourcedoc.Items.Insert(0, "SELECT")
                End If
            Catch ex As Exception
                lblmsgFrelation.Text = "SERVER ERROR: PLEASE TRY AFTER SOME TIME"
            Finally
                If Not con Is Nothing Then
                    con.Close()
                End If
                If Not da Is Nothing Then
                    da.Dispose()
                End If

            End Try
        ElseIf UCase(ddlsourcetype.SelectedItem.Text) = "DETAIL FORM" Then
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As New SqlConnection(conStr)

            Dim da As New SqlDataAdapter("", con)
            Dim ds As New DataSet

            Try
                da.SelectCommand.CommandText = "Select upper(formname)[Formname],formid from mmm_mst_forms where eid=" & Session("EID") & " and formsource='DETAIL FORM ' order by formname"
                da.Fill(ds, "detailform")

                If ds.Tables("detailform").Rows.Count > 0 Then
                    ddlsourcedoc.DataSource = ds.Tables("detailform")
                    ddlsourcedoc.DataTextField = "formname"
                    ddlsourcedoc.DataValueField = "formid"
                    ddlsourcedoc.DataBind()
                    ddlsourcedoc.Items.Insert(0, "SELECT")
                End If
            Catch ex As Exception
                lblmsgFrelation.Text = "SERVER ERROR: PLEASE TRY AFTER SOME TIME"
            Finally
                If Not con Is Nothing Then
                    con.Close()
                End If
                If Not da Is Nothing Then
                    da.Dispose()
                End If

            End Try
        Else

            lblmsgFrelation.Text = "Please Select Source Type!!"
            ddlsourcedoc.Items.Clear()
            Exit Sub
        End If
    End Sub

    Protected Sub ddlsourcedoc_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlsourcedoc.SelectedIndexChanged
        bindfieldsRelation()
    End Sub

    Private Sub bindfieldsRelation()
        Dim conStr As String = System.Configuration.ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = Nothing
        Dim cmd As SqlCommand = Nothing
        Dim da As SqlDataAdapter = Nothing
        Try
            Dim table As New DataTable()
            con = New SqlConnection(conStr)
            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            Dim sql As String = "select displayname,fieldid,fieldmapping from mmm_mst_fields where EID=" & Session("EID") & " and documenttype='" & ddlsourcedoc.SelectedItem.Text.ToString().Trim() & "' order by displayname"
            cmd = New SqlCommand(sql, con)
            da = New SqlDataAdapter(cmd)
            Dim ds As New DataSet
            da.Fill(ds, "table")

            If ds.Tables("table").Rows.Count > 0 Then
                ddltf.DataSource = ds.Tables("table")
                ddltf.DataTextField = "displayname"
                ddltf.DataValueField = "fieldid"
                ddltf.DataBind()
                ddltf.Items.Insert(0, "Select")

                ddlsortingfields.DataSource = ds.Tables("table")
                ddlsortingfields.DataTextField = "displayname"
                ddlsortingfields.DataValueField = "fieldid"
                ddlsortingfields.DataBind()
                ddlsortingfields.Items.Insert(0, "Select")
            End If


        Catch ex As Exception
            lblMsg.Text = "SERVER ERROR: PLEASE TRY AFTER SOME TIME"
        Finally
            If Not con Is Nothing Then
                con.Close()
                con.Dispose()
            End If
            If Not da Is Nothing Then
                da.Dispose()
            End If
            If Not cmd Is Nothing Then
                cmd.Dispose()
            End If

        End Try
    End Sub

    Protected Sub btnSearch_Click(sender As Object, e As ImageClickEventArgs)
        Dim da As SqlDataAdapter = New SqlDataAdapter("", con)

        Dim dt As New DataTable
        If ddlfldname.SelectedIndex = 0 Then
            da.SelectCommand.CommandText = "declare @eid int=" & Session("EID") & " declare @value nvarchar(100)='" & txtValue.Text & "' declare  @PageIndex INT = 1 declare @PageSize INT = 20 declare @RecordCount INT SELECT ROW_NUMBER() OVER       (             ORDER BY [TID] ASC       )AS RowNumber      ,Tid 	,Eid 	,SourceDoc 	,SQType 	,TargetDoc 	,TQType 	,NoofRecords 	,CreateEvent 	,ScheduleTime 	,WFStatus 	,Creator 	,RoleName 	,lastdate 	,ConfigName       INTO #Results        FROM [mmm_schdoc_main]  WHERE ([ConfigName] LIKE @value + '%' OR @value = '') and eid=@eid  	  SELECT @RecordCount = COUNT(*)       FROM #Results 	   SELECT * FROM #Results       WHERE RowNumber BETWEEN(@PageIndex -1) * @PageSize + 1 AND(((@PageIndex -1) * @PageSize + 1) + @PageSize) - 1        DROP TABLE #Results	 "
            If con.State = ConnectionState.Closed Then
                con.Open()
            End If
            da.Fill(dt)
        ElseIf ddlfldname.SelectedIndex = 1 Then
            da.SelectCommand.CommandText = "declare @eid int=" & Session("EID") & " declare @value nvarchar(100)='" & txtValue.Text & "' declare  @PageIndex INT = 1 declare @PageSize INT = 20 declare @RecordCount INT SELECT ROW_NUMBER() OVER       (             ORDER BY [TID] ASC       )AS RowNumber      ,Tid 	,Eid 	,SourceDoc 	,SQType 	,TargetDoc 	,TQType 	,NoofRecords 	,CreateEvent 	,ScheduleTime 	,WFStatus 	,Creator 	,RoleName 	,lastdate 	,ConfigName       INTO #Results        FROM [mmm_schdoc_main]  WHERE ([SourceDoc] LIKE @value + '%' OR @value = '') and eid=@eid  	  SELECT @RecordCount = COUNT(*)       FROM #Results 	   SELECT * FROM #Results       WHERE RowNumber BETWEEN(@PageIndex -1) * @PageSize + 1 AND(((@PageIndex -1) * @PageSize + 1) + @PageSize) - 1        DROP TABLE #Results	 "
            If con.State = ConnectionState.Closed Then
                con.Open()
            End If
            da.Fill(dt)
        ElseIf ddlfldname.SelectedIndex = 2 Then
            da.SelectCommand.CommandText = "declare @eid int=" & Session("EID") & " declare @value nvarchar(100)='" & txtValue.Text & "' declare  @PageIndex INT = 1 declare @PageSize INT = 20 declare @RecordCount INT SELECT ROW_NUMBER() OVER       (             ORDER BY [TID] ASC       )AS RowNumber      ,Tid 	,Eid 	,SourceDoc 	,SQType 	,TargetDoc 	,TQType 	,NoofRecords 	,CreateEvent 	,ScheduleTime 	,WFStatus 	,Creator 	,RoleName 	,lastdate 	,ConfigName       INTO #Results        FROM [mmm_schdoc_main]  WHERE ([TargetDoc] LIKE @value + '%' OR @value = '') and eid=@eid  	  SELECT @RecordCount = COUNT(*)       FROM #Results 	   SELECT * FROM #Results       WHERE RowNumber BETWEEN(@PageIndex -1) * @PageSize + 1 AND(((@PageIndex -1) * @PageSize + 1) + @PageSize) - 1        DROP TABLE #Results	 "
            If con.State = ConnectionState.Closed Then
                con.Open()
            End If
            da.Fill(dt)
        End If
        If dt.Rows.Count > 0 Then
            gvData.DataSource = dt
            gvData.DataBind()
        Else
            gvData.DataSource = Nothing
            gvData.DataBind()
        End If
    End Sub

    Protected Sub ddladvfodoctype_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddladvfodoctype.SelectedIndexChanged

        Call LoadWorkGroupTree(ddladvfodoctype.SelectedItem.Text.Trim().ToString())
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim da As New SqlDataAdapter("", con)
        Dim ds As New DataSet
        Try

            da.SelectCommand.CommandText = "select upper(displayname)[formname],upper(fieldmapping)[fieldmapping] from mmm_mst_fields where eid=" & Session("Eid") & " and documenttype='" & Trim(ddladvfodoctype.SelectedItem.Text) & "' order by displayname"
            da.Fill(ds, "fields")
            If ds.Tables("fields").Rows.Count > 0 Then
                ddlsdf.DataSource = ds.Tables("fields")
                ddlsdf.DataTextField = "formname"
                ddlsdf.DataValueField = "fieldmapping"
                ddlsdf.DataBind()
                ddlsdf.Items.Insert(0, "SELECT")
            End If

        Catch ex As Exception
            lblMsg.Text = "SERVER ERROR: PLEASE TRY AFTER SOME TIME"
        Finally
            If Not con Is Nothing Then
                con.Close()
            End If
            If Not da Is Nothing Then
                da.Dispose()
            End If

        End Try
    End Sub
    Protected Sub btnSaveDtlChild_Click(sender As Object, e As EventArgs)
        Dim pair As KeyValuePair(Of Boolean, String) = chkChilddtlvalidation()
        If pair.Key = False Then
            lblChildRedMsg.Text = pair.Value
            Exit Sub
        End If
        Dim da As New SqlDataAdapter("", con)
        da.SelectCommand.CommandType = CommandType.Text
        da.SelectCommand.CommandText = "IF EXISTS (select * from mmm_schdoc_child where ChildSourcedoc='" & ddlSourceChildDoc.SelectedValue.Trim() & "' and ChildTargetdoc='" & ddlTargetChildDoc.SelectedValue.Trim() & "' and ChildTargetfldMapping='" & ddlChildTargetMapping.SelectedValue.Trim() & "' and eid=" & Session("EID") & " and ctid=" & ViewState("TID") & ")	  update mmm_schdoc_child set ChildSourcefldMapping='" & ViewState("CHILDSFLDMAPPING").ToString() & "', MappingType='" & ddlChildMappingType.SelectedValue & "',Remarks='" & txtChildRemarks.Text & "',lastupdate=getdate() where tid in (SELECT tid FROM mmm_schdoc_child  WHERE ChildTargetfldMapping = '" & ddlChildTargetMapping.SelectedValue & "' and eid=" & Session("EID") & " and ChildSourcedoc='" & ddlSourceChildDoc.SelectedValue.Trim() & "' and ChildTargetdoc='" & ddlTargetChildDoc.SelectedValue.Trim() & "' and ctid=" & ViewState("TID") & ") and eid=" & Session("EID") & " and ctid=" & ViewState("TID") & " 	  else insert into mmm_schdoc_child (Ctid,ChildSourcedoc,ChildTargetdoc,MappingType,ChildSourcefldMapping,ChildTargetfldMapping,Remarks,LastUpdate,eid) values (" & ViewState("TID") & ",'" & ddlSourceChildDoc.SelectedValue.Trim() & "','" & ddlTargetChildDoc.SelectedValue.Trim() & "','" & ddlChildMappingType.SelectedValue.Trim() & "','" & ViewState("CHILDSFLDMAPPING").ToString() & "','" & ddlChildTargetMapping.SelectedValue.Trim() & "','" & txtChildRemarks.Text.Trim() & "',getdate()," & Session("EID") & ") select case(SCOPE_IDENTITY()) when null then SCOPE_IDENTITY() else (select tid from mmm_schdoc_child where ChildSourcedoc='" & ddlSourceChildDoc.SelectedValue.Trim() & "' and ChildTargetdoc='" & ddlTargetChildDoc.SelectedValue.Trim() & "' and ChildTargetfldMapping='" & ddlChildTargetMapping.SelectedValue.Trim() & "' and eid=" & Session("EID") & " and ctid=" & ViewState("TID") & ") end"
        If con.State = ConnectionState.Closed Then
            con.Open()
        End If
        'da.SelectCommand.ExecuteNonQuery()
        Dim id As Integer = da.SelectCommand.ExecuteScalar()
        If ddlChildMappingType.SelectedIndex = 3 Then
            da.SelectCommand.CommandText = "Delete  from [MMM_MST_AdvFormulaRelation] where eid=" & Session("EID") & " and FormulaID=" & id & " and Type='CHILD'"
            If con.State <> ConnectionState.Open Then
                con.Open()
            End If

            da.SelectCommand.ExecuteNonQuery()

            For Each gvrow As GridViewRow In gvmap.Rows

                da.SelectCommand.Parameters.Clear()
                da.SelectCommand.CommandText = "USP_insertAdvFormulaRelation"
                da.SelectCommand.CommandType = CommandType.StoredProcedure
                da.SelectCommand.Parameters.AddWithValue("@eid", Session("EID"))
                da.SelectCommand.Parameters.AddWithValue("@formulaid", id)
                da.SelectCommand.Parameters.AddWithValue("@sourcetype", gvrow.Cells(1).Text.ToString())
                da.SelectCommand.Parameters.AddWithValue("@sourcename", gvrow.Cells(2).Text.ToString())
                da.SelectCommand.Parameters.AddWithValue("@s_relationidentifierfiled", gvrow.Cells(5).Text.ToString())
                da.SelectCommand.Parameters.AddWithValue("@targettype", gvrow.Cells(3).Text.ToString())
                da.SelectCommand.Parameters.AddWithValue("@targetname", gvrow.Cells(4).Text.ToString())
                da.SelectCommand.Parameters.AddWithValue("@t_relationidentifierfield", gvrow.Cells(6).Text.ToString())
                da.SelectCommand.Parameters.AddWithValue("@sortingfield", gvrow.Cells(7).Text.ToString())
                da.SelectCommand.Parameters.AddWithValue("@uid", Session("UID"))
                da.SelectCommand.Parameters.AddWithValue("@Type", "CHILD")
                If con.State <> ConnectionState.Open Then
                    con.Open()
                End If
                da.SelectCommand.ExecuteNonQuery()
            Next
        End If
        BindConfigChildDetails()
    End Sub
    Function chkChilddtlvalidation() As KeyValuePair(Of Boolean, String)
        Dim type As Boolean = True
        Dim sb As New StringBuilder
        ViewState("CHILDSFLDMAPPING") = Nothing
        If ddlTargetChildDoc.SelectedIndex = 0 Then
            sb.Append("Please Select Target Child Document ,")
        ElseIf ddlSourceChildDoc.SelectedIndex = 0 Then
            sb.Append("Please Select Source Child Document ,")
        ElseIf ddlChildMappingType.SelectedIndex = 1 Then
            If txtChildFixMapping.Text = String.Empty Then
                sb.Append("Please Fill Fixed Value ,")
            Else
                ViewState("CHILDSFLDMAPPING") = txtChildFixMapping.Text
            End If
        ElseIf ddlChildMappingType.SelectedIndex = 2 Then
            If ddlChildSourceMapping.SelectedIndex = 0 Then
                sb.Append("Please Select Copy Value ,")
            Else
                ViewState("CHILDSFLDMAPPING") = ddlChildSourceMapping.SelectedValue
            End If
        ElseIf ddlChildMappingType.SelectedIndex = 3 Then
            If txtChildFormulaMapping.Text = String.Empty Then
                sb.Append("Please Fill Mapping Formula Value ,")
            Else
                ViewState("CHILDSFLDMAPPING") = txtChildFormulaMapping.Text
            End If
        End If
        If sb.Length > 1 Then
            type = False
        End If
        Return New KeyValuePair(Of Boolean, String)(type, sb.ToString())
    End Function

    Protected Sub ddlSecondarySourceForm_SelectedIndexChanged(sender As Object, e As EventArgs)
        Dim value As String = IIf(ddlSecondarySourceForm.SelectedValue.Trim().Contains("-MASTER"), "MASTER", "DOCUMENT")
        If ddlSecondarySourceType.Items.Count > 0 Then
            ddlSecondarySourceType.Items.Clear()
            If ddlSecondarySourceForm.SelectedItem.Text.ToUpper = "USER" Then
                ddlSecondarySourceType.Items.Insert("0", "USER")
            Else
                ddlSecondarySourceType.Items.Insert("0", New ListItem(value))
            End If

        Else
            If ddlSecondarySourceForm.SelectedItem.Text.ToUpper = "USER" Then
                ddlSecondarySourceType.Items.Insert("0", "USER")
            Else
                ddlSecondarySourceType.Items.Insert("0", New ListItem(value))
            End If
        End If
        commanfilter.Visible = True
        Dim da As New SqlDataAdapter("", con)
        Dim dt As New DataSet
        If ddlSecondarySourceForm.SelectedItem.Text.ToUpper = "USER" Then
            da.SelectCommand.CommandText = "select (a.documenttype+'-'+a.displayname +'|'+ b.documenttype+'-'+b.displayname) as [Text],convert (varchar(20),a.fieldid)+'-'+convert(varchar(20),b.fieldid) as val   from mmm_mst_fields as a inner join mmm_mst_fields as b on a.eid=b.eid where a.eid=" & Session("EID") & " and b.eid=" & Session("EID") & "  and a.FieldType='Drop down' and b.FieldType='Drop down' and substring(right(a.dropdown,len(a.dropdown)-charindex('-',a.dropdown)),0,CHARINDEX('-',right(a.dropdown,len(a.dropdown)-charindex('-',a.dropdown)))) =substring(right(a.dropdown,len(b.dropdown)-charindex('-',b.dropdown)),0,CHARINDEX('-',right(b.dropdown,len(b.dropdown)-charindex('-',b.dropdown)))) and a.documenttype='" & ddlSourceForm.SelectedItem.Text.Trim() & "' and b.documenttype='" & ddlSecondarySourceForm.SelectedItem.Text & "'; select name as displayname, 'MMM_MST_USER-'+name as name from sys.columns where object_id=object_id('mmm_mst_user') and name not like'fld%'  union all select displayname, case 'DOCUMENT' when 'MASTER' then 'MMM_MST_MASTER-' else 'MMM_MST_DOCUMENT-' end +FIELDMAPPING as name from mmm_mst_fields where eid=" & Session("EID") & " and documenttype='" & ddlSecondarySourceForm.SelectedItem.Text & "'"
        Else
            da.SelectCommand.CommandText = "select (a.documenttype+'-'+a.displayname +'|'+ b.documenttype+'-'+b.displayname) as [Text],convert (varchar(20),a.fieldid)+'-'+convert(varchar(20),b.fieldid) as val   from mmm_mst_fields as a inner join mmm_mst_fields as b on a.eid=b.eid where a.eid=" & Session("EID") & " and b.eid=" & Session("EID") & "  and a.FieldType='Drop down' and b.FieldType='Drop down' and substring(right(a.dropdown,len(a.dropdown)-charindex('-',a.dropdown)),0,CHARINDEX('-',right(a.dropdown,len(a.dropdown)-charindex('-',a.dropdown)))) =substring(right(a.dropdown,len(b.dropdown)-charindex('-',b.dropdown)),0,CHARINDEX('-',right(b.dropdown,len(b.dropdown)-charindex('-',b.dropdown)))) and a.documenttype='" & ddlSourceForm.SelectedItem.Text.Trim() & "' and b.documenttype='" & ddlSecondarySourceForm.SelectedItem.Text & "';select displayname, case 'DOCUMENT' when 'MASTER' then 'MMM_MST_MASTER-' else 'MMM_MST_DOCUMENT-' end +FIELDMAPPING as name from mmm_mst_fields where eid=" & Session("EID") & " and documenttype='" & ddlSecondarySourceForm.SelectedItem.Text & "'"
        End If

        da.Fill(dt, "ds")
        If dt.Tables("ds").Rows.Count > 0 Then
            chkCommanFields.DataSource = dt.Tables("ds")
            chkCommanFields.DataTextField = "Text"
            chkCommanFields.DataValueField = "val"
            chkCommanFields.DataBind()
        Else
            If chkCommanFields.Items.Count > 0 Then
                chkCommanFields.Items.Clear()
            End If
            chkCommanFields.DataSource = Nothing
            chkCommanFields.DataBind()
            commanfilter.Visible = False
        End If
        If dt.Tables("ds1").Rows.Count > 0 Then
            ddlSConditionDataFields.DataSource = dt.Tables("ds1")
            ddlSConditionDataFields.DataTextField = "displayname"
            ddlSConditionDataFields.DataValueField = "name"
            ddlSConditionDataFields.DataBind()
            ddlSConditionDataFields.Items.Insert("0", New ListItem("SELECT"))
        Else
            If ddlSConditionDataFields.Items.Count > 0 Then
                ddlSConditionDataFields.Items.Clear()
            End If
            ddlSConditionDataFields.DataSource = Nothing
            ddlSConditionDataFields.DataBind()
        End If
        'da.SelectCommand.CommandText = "select displayName,dbtablename+'-'+FieldMapping as val from mmm_mst_fields where DocumentType='" & ddlSecondarySourceForm.SelectedItem.Text & "' and eid=" & Session("EID")
        'Dim dts As New DataTable
        'da.Fill(dts)
        'If dts.Rows.Count > 0 Then
        '    ddlConditionDataFields.DataSource = dts
        '    ddlConditionDataFields.DataTextField = "displayName"
        '    ddlConditionDataFields.DataValueField = "val"
        '    ddlConditionDataFields.DataBind()
        '    ddlConditionDataFields.Items.Insert("0", New ListItem("SELECT"))
        'End If
        isSecCreator_CheckedChanged(isSecCreator, New EventArgs())
    End Sub
    Protected Sub ddlConditionDataFields_SelectedIndexChanged(sender As Object, e As EventArgs)

        Dim vals As String() = Nothing
        Dim sb As New StringBuilder()
        If ddlConditionDataFields.SelectedIndex <> 0 Then
            vals = ddlConditionDataFields.SelectedValue.Trim().Split("-")
            Dim StrViewName = "[" & "V" & Session("EID") & ddlSourceForm.SelectedItem.Text.Replace(" ", "_") & "] D"
            Dim Query = ""
            If vals(1).ToString().Contains("fld") Then
                Query = "select * from MMM_MST_FIELDS WHERE EID= " & Session("EID") & " AND DocumentType='" & ddlSourceForm.SelectedItem.Text.Trim() & "' AND fieldMapping='" & vals(1).ToString() & "'"
            Else
                Query = "Select distinct " & vals(1).ToString() & " as val from " & vals(0).ToString() & " where  eid=" & Session("EID") & " and " & vals(1).ToString() & " is not null"
            End If

            Dim ds As New DataSet()
            Using con As New SqlConnection(conStr)
                Using da As New SqlDataAdapter(Query, con)
                    If con.State = ConnectionState.Closed Then
                        con.Open()
                    End If
                    da.Fill(ds)
                End Using
            End Using
            If ds.Tables(0).Rows.Count > 0 Then
                If ds.Tables(0).Columns.Count = 1 Then
                    ddlConditionValues.DataSource = ds
                    ddlConditionValues.DataTextField = "val"
                    ddlConditionValues.DataValueField = "val"
                    ddlConditionValues.DataBind()
                    ddlConditionValues.Items.Insert("0", New ListItem("SELECT"))
                    Exit Sub
                End If
                Dim FieldType = "", DropDownType = ""
                FieldType = Convert.ToString(ds.Tables(0).Rows(0).Item("FieldType")).Trim()
                DropDownType = Convert.ToString(ds.Tables(0).Rows(0).Item("DropDownType")).Trim()
                Dim StrQuery1 = ""
                StrQuery1 = "SELECT Distinct [" & ds.Tables(0).Rows(0).Item("DisplayName") & "] 'Text',D.[" & ds.Tables(0).Rows(0).Item("DisplayName") & "] 'value' FROM " & StrViewName
                If FieldType = "Drop Down" And (DropDownType = "MASTER VALUED" Or DropDownType = "SESSION VALUED") Then
                    Dim TID As String = "TID"
                    Dim arr = ds.Tables(0).Rows(0).Item("DropDown").ToString().Split("-")
                    Dim FieldID = ds.Tables(0).Rows(0).Item("Fieldid").ToString().Split("-")
                    Dim EID As Integer = Convert.ToInt32(Session("EID"))
                    'Getting table name
                    Dim TABLENAME As String = ""
                    If UCase(arr(0).ToString()) = "MASTER" Then
                        TABLENAME = "MMM_MST_MASTER"
                    ElseIf UCase(arr(0).ToString()) = "DOCUMENT" Then
                        TABLENAME = "MMM_MST_DOC"
                    ElseIf UCase(arr(0).ToString()) = "CHILD" Then
                        TABLENAME = "MMM_MST_DOC_ITEM"
                    ElseIf UCase(arr(0).ToString) = "SESSION" Then
                        TABLENAME = "MMM_MST_USER"
                        TID = "UID"
                    ElseIf UCase(arr(0).ToString) = "STATIC" Then
                        If arr(1).ToString.ToUpper = "USER" Then
                            TABLENAME = "MMM_MST_USER"
                            TID = "UID"
                        ElseIf arr(1).ToString().ToUpper = "LOCATION" Then
                            TABLENAME = "MMM_MST_LOCATION"
                            TID = "LOCID"
                        ElseIf arr(1).ToString() = "MMM_MST_Role" Then
                            TABLENAME = "MMM_MST_Role"
                            TID = "RoleName"
                        End If
                    End If
                    Dim DSIntl = New DataSet()
                    StrQuery1 = ""
                    If TABLENAME = "MMM_MST_MASTER" Or TABLENAME = "MMM_MST_DOC" Then
                        StrQuery1 = "select " & arr(2).ToString() & " as Text," & TID & "[value]  from " & TABLENAME & " M WHERE   DOCUMENTTYPE='" & arr(1).ToString() & "'"
                        StrQuery1 = StrQuery1 & "   AND EID= " & EID
                        'For initial Filter
                    ElseIf TABLENAME = "MMM_MST_USER" Then
                        If ds.Tables(0).Rows(0).Item("dropdowntype").ToString.ToUpper = "SESSION VALUED" Then
                            'For mobile they will supply userID
                            StrQuery1 = "select " & arr(2).ToString() & " as Text," & TID & " as value from " & TABLENAME & " M WHERE EID=" & EID & " " & ""
                        Else
                            StrQuery1 = "select " & arr(2).ToString() & " as Text," & TID & "  as value from " & TABLENAME & " M WHERE EID=" & EID & " " & ""
                            'For initial Filter
                        End If
                    ElseIf TABLENAME = "MMM_MST_LOCATION" Then
                        StrQuery1 = "select DISTINCT " & arr(2).ToString() & " as Text,SID [value]  from " & TABLENAME & " M "
                    End If

                Else
                    If ddlSourceForm.SelectedItem.Text.ToUpper = "USER" Then
                        StrQuery1 = StrQuery1 & " WHERE D.EID=" & Session("EID")
                    Else
                        StrQuery1 = StrQuery1 & " WHERE D.EID=" & Session("EID") & " and D.DocumentType='" & ddlSourceForm.SelectedItem.Text() & "'"
                    End If
                End If


                'Filling data to the concern dropdown
                ds = New DataSet()
                Using con As New SqlConnection(conStr)
                    Using da As New SqlDataAdapter(StrQuery1, con)
                        If con.State = ConnectionState.Closed Then
                            con.Open()
                        End If
                        da.Fill(ds)
                    End Using
                End Using
                If ds.Tables(0).Rows.Count > 0 Then
                    ddlConditionValues.DataSource = ds
                    ddlConditionValues.DataTextField = "Text"
                    ddlConditionValues.DataValueField = "value"
                    ddlConditionValues.DataBind()
                    ddlConditionValues.Items.Insert("0", New ListItem("SELECT"))
                Else
                    If ddlConditionValues.Items.Count > 0 Then
                        ddlConditionValues.Items.Clear()
                        ddlConditionValues.Items.Insert("0", New ListItem("SELECT"))
                    End If
                End If
            End If
        End If


        'Old working code


        'Dim vals As String() = Nothing
        'Dim sb As New StringBuilder()
        'If ddlConditionDataFields.SelectedIndex <> 0 Then
        '    vals = ddlConditionDataFields.SelectedValue.Trim().Split("-")
        '    Dim StrViewName = "[" & "V" & Session("EID") & ddlSourceForm.SelectedItem.Text.Replace(" ", "_") & "] D"
        '    Dim Query = ""
        '    If vals(1).ToString().Contains("fld") Then
        '        Query = "select * from MMM_MST_FIELDS WHERE EID= " & Session("EID") & " AND DocumentType='" & ddlSourceForm.SelectedItem.Text.Trim() & "' AND fieldMapping='" & vals(1).ToString() & "'"
        '    Else
        '        Query = "Select distinct " & vals(1).ToString() & " as val from " & vals(0).ToString() & " where  eid=" & Session("EID") & " and " & vals(1).ToString() & " is not null"
        '    End If

        '    Dim ds As New DataSet()
        '    Using con As New SqlConnection(conStr)
        '        Using da As New SqlDataAdapter(Query, con)
        '            If con.State = ConnectionState.Closed Then
        '                con.Open()
        '            End If
        '            da.Fill(ds)
        '        End Using
        '    End Using
        '    If ds.Tables(0).Rows.Count > 0 Then
        '        If ds.Tables(0).Columns.Count = 1 Then
        '            ddlConditionValues.DataSource = ds
        '            ddlConditionValues.DataTextField = "val"
        '            ddlConditionValues.DataValueField = "val"
        '            ddlConditionValues.DataBind()
        '            ddlConditionValues.Items.Insert("0", New ListItem("SELECT"))
        '            Exit Sub
        '        End If
        '        Dim FieldType = "", DropDownType = ""
        '        FieldType = Convert.ToString(ds.Tables(0).Rows(0).Item("FieldType")).Trim()
        '        DropDownType = Convert.ToString(ds.Tables(0).Rows(0).Item("DropDownType")).Trim()
        '        Dim StrQuery1 = ""
        '        StrQuery1 = "SELECT Distinct [" & ds.Tables(0).Rows(0).Item("DisplayName") & "] 'Text',D.[" & ds.Tables(0).Rows(0).Item("DisplayName") & "] 'value' FROM " & StrViewName
        '        If FieldType = "Drop Down" And (DropDownType = "MASTER VALUED" Or DropDownType = "SESSION VALUED") Then
        '            Dim arr = ds.Tables(0).Rows(0).Item("DropDown").ToString().Split("-")
        '            If arr(1).ToUpper = "USER" Then
        '                StrQuery1 = " isnull([userName],'') as 'Text',D.[" & ds.Tables(0).Rows(0).Item("DisplayName") & "] Value  from MMM_MST_USER s inner join " & StrViewName & " on D.eid=s.eid and s.[UID]=case when D.[Requester Name]='SELECT' then '0' else D.[" & ds.Tables(0).Rows(0).Item("DisplayName") & "] end "
        '            Else
        '                Dim Str = "SELECT DisplayName FROM MMM_MST_FIELDS WHERE EID=" & Session("EID") & "AND Documenttype='" & arr(1) & "' AND FieldMapping='" & arr(2) & "'"
        '                Dim ds1 As New DataSet()
        '                Using con As New SqlConnection(conStr)
        '                    Using da As New SqlDataAdapter(Str, con)
        '                        If con.State = ConnectionState.Closed Then
        '                            con.Open()
        '                        End If
        '                        da.Fill(ds1)
        '                    End Using
        '                End Using
        '                StrQuery1 = " s.[" & ds1.Tables(0).Rows(0).Item("DisplayName") & "]'Text',D.[" & ds.Tables(0).Rows(0).Item("DisplayName") & "] 'value'  from [V" & Session("EID") & arr(1).Replace(" ", "_") & "] s inner join " & StrViewName.ToString() & " on  s.tid=D.[" & ds.Tables(0).Rows(0).Item("DisplayName") & "] "
        '            End If
        '            StrQuery1 = "SELECT distinct  " & StrQuery1
        '        End If
        '        If ddlSourceForm.SelectedItem.Text.ToUpper = "USER" Then
        '            StrQuery1 = StrQuery1 & " WHERE D.EID=" & Session("EID")
        '        Else
        '            StrQuery1 = StrQuery1 & " WHERE D.EID=" & Session("EID") & " and D.DocumentType='" & ddlSourceForm.SelectedItem.Text() & "'"
        '        End If

        '        'Filling data to the concern dropdown
        '        ds = New DataSet()
        '        Using con As New SqlConnection(conStr)
        '            Using da As New SqlDataAdapter(StrQuery1, con)
        '                If con.State = ConnectionState.Closed Then
        '                    con.Open()
        '                End If
        '                da.Fill(ds)
        '            End Using
        '        End Using
        '        If ds.Tables(0).Rows.Count > 0 Then
        '            ddlConditionValues.DataSource = ds
        '            ddlConditionValues.DataTextField = "Text"
        '            ddlConditionValues.DataValueField = "value"
        '            ddlConditionValues.DataBind()
        '            ddlConditionValues.Items.Insert("0", New ListItem("SELECT"))
        '        Else
        '            If ddlConditionValues.Items.Count > 0 Then
        '                ddlConditionValues.Items.Clear()
        '                ddlConditionValues.Items.Insert("0", New ListItem("SELECT"))
        '            End If
        '        End If
        '    End If
        'End If
    End Sub

    Protected Sub ddlSConditionDataFields_SelectedIndexChanged(sender As Object, e As EventArgs)
        Dim vals As String() = Nothing
        Dim sb As New StringBuilder()
        If ddlSConditionDataFields.SelectedIndex <> 0 Then
            'If ddlSourceForm.SelectedItem.Text.Trim.ToUpper <> "USER" Then
            vals = ddlSConditionDataFields.SelectedValue.Trim().Split("-")
            Dim StrViewName = "[" & "V" & Session("EID") & ddlSecondarySourceForm.SelectedItem.Text.Replace(" ", "_") & "] D"
            Dim Query = ""
            If vals(1).ToString().Contains("fld") Then
                Query = "select * from MMM_MST_FIELDS WHERE EID= " & Session("EID") & " AND DocumentType='" & ddlSecondarySourceForm.SelectedItem.Text.Trim() & "' AND fieldMapping='" & vals(1).ToString() & "'"
            Else
                Query = "Select distinct " & vals(1).ToString() & " as val from " & vals(0).ToString() & " where  eid=" & Session("EID") & " and " & vals(1).ToString() & " is not null"
            End If

            Dim ds As New DataSet()
            Using con As New SqlConnection(conStr)
                Using da As New SqlDataAdapter(Query, con)
                    If con.State = ConnectionState.Closed Then
                        con.Open()
                    End If
                    da.Fill(ds)
                End Using
            End Using
            If ds.Tables(0).Rows.Count > 0 Then
                If ds.Tables(0).Columns.Count = 1 Then
                    ddlSConditionValues.DataSource = ds
                    ddlSConditionValues.DataTextField = "val"
                    ddlSConditionValues.DataValueField = "val"
                    ddlSConditionValues.DataBind()
                    ddlSConditionValues.Items.Insert("0", New ListItem("SELECT"))
                    Exit Sub
                End If
                Dim FieldType = "", DropDownType = ""
                FieldType = Convert.ToString(ds.Tables(0).Rows(0).Item("FieldType")).Trim()
                DropDownType = Convert.ToString(ds.Tables(0).Rows(0).Item("DropDownType")).Trim()
                Dim StrQuery1 = ""
                StrQuery1 = "SELECT Distinct [" & ds.Tables(0).Rows(0).Item("DisplayName") & "] 'Text',D.[" & ds.Tables(0).Rows(0).Item("DisplayName") & "] 'value' FROM " & StrViewName
                If FieldType = "Drop Down" And (DropDownType = "MASTER VALUED" Or DropDownType = "SESSION VALUED") Then
                    Dim TID As String = "TID"
                    Dim arr = ds.Tables(0).Rows(0).Item("DropDown").ToString().Split("-")
                    Dim FieldID = ds.Tables(0).Rows(0).Item("Fieldid").ToString().Split("-")
                    Dim EID As Integer = Convert.ToInt32(Session("EID"))
                    'Getting table name
                    Dim TABLENAME As String = ""
                    If UCase(arr(0).ToString()) = "MASTER" Then
                        TABLENAME = "MMM_MST_MASTER"
                    ElseIf UCase(arr(0).ToString()) = "DOCUMENT" Then
                        TABLENAME = "MMM_MST_DOC"
                    ElseIf UCase(arr(0).ToString()) = "CHILD" Then
                        TABLENAME = "MMM_MST_DOC_ITEM"
                    ElseIf UCase(arr(0).ToString) = "SESSION" Then
                        TABLENAME = "MMM_MST_USER"
                        TID = "UID"
                    ElseIf UCase(arr(0).ToString) = "STATIC" Then
                        If arr(1).ToString.ToUpper = "USER" Then
                            TABLENAME = "MMM_MST_USER"
                            TID = "UID"
                        ElseIf arr(1).ToString().ToUpper = "LOCATION" Then
                            TABLENAME = "MMM_MST_LOCATION"
                            TID = "LOCID"
                        ElseIf arr(1).ToString() = "MMM_MST_Role" Then
                            TABLENAME = "MMM_MST_Role"
                            TID = "RoleName"
                        End If
                    End If
                    Dim DSIntl = New DataSet()
                    StrQuery1 = ""
                    If TABLENAME = "MMM_MST_MASTER" Or TABLENAME = "MMM_MST_DOC" Then
                        StrQuery1 = "select " & arr(2).ToString() & " as Text," & TID & "[value]  from " & TABLENAME & " M WHERE   DOCUMENTTYPE='" & arr(1).ToString() & "'"
                        StrQuery1 = StrQuery1 & "   AND EID= " & EID
                        'For initial Filter
                    ElseIf TABLENAME = "MMM_MST_USER" Then
                        If ds.Tables(0).Rows(0).Item("dropdowntype").ToString.ToUpper = "SESSION VALUED" Then
                            'For mobile they will supply userID
                            StrQuery1 = "select " & arr(2).ToString() & " as Text," & TID & " as value from " & TABLENAME & " M WHERE EID=" & EID & " " & ""
                        Else
                            StrQuery1 = "select " & arr(2).ToString() & " as Text," & TID & "  as value from " & TABLENAME & " M WHERE EID=" & EID & " " & ""
                            'For initial Filter
                        End If
                    ElseIf TABLENAME = "MMM_MST_LOCATION" Then
                        StrQuery1 = "select DISTINCT " & arr(2).ToString() & " as Text,SID [value]  from " & TABLENAME & " M "
                    End If

                Else
                    If ddlSecondarySourceForm.SelectedItem.Text.ToUpper = "USER" Then
                        StrQuery1 = StrQuery1 & " WHERE D.EID=" & Session("EID")
                    Else
                        StrQuery1 = StrQuery1 & " WHERE D.EID=" & Session("EID") & " and D.DocumentType='" & ddlSecondarySourceForm.SelectedItem.Text() & "'"
                    End If
                End If

                'Old code
                'If FieldType = "Drop Down" And (DropDownType = "MASTER VALUED" Or DropDownType = "SESSION VALUED") Then
                '    Dim arr = ds.Tables(0).Rows(0).Item("DropDown").ToString().Split("-")
                '    If arr(1).ToUpper = "USER" Then
                '        StrQuery1 = " isnull([userName],'') as 'Text',D.[" & ds.Tables(0).Rows(0).Item("DisplayName") & "] Value  from MMM_MST_USER s inner join " & StrViewName & " on D.eid=s.eid and s.[UID]=case when D.[Requester Name]='SELECT' then '0' else D.[" & ds.Tables(0).Rows(0).Item("DisplayName") & "] end "
                '    Else
                '        Dim Str = "SELECT DisplayName FROM MMM_MST_FIELDS WHERE EID=" & Session("EID") & "AND Documenttype='" & arr(1) & "' AND FieldMapping='" & arr(2) & "'"
                '        Dim ds1 As New DataSet()
                '        Using con As New SqlConnection(conStr)
                '            Using da As New SqlDataAdapter(Str, con)
                '                If con.State = ConnectionState.Closed Then
                '                    con.Open()
                '                End If
                '                da.Fill(ds1)
                '            End Using
                '        End Using
                '        StrQuery1 = " s.[" & ds1.Tables(0).Rows(0).Item("DisplayName") & "]'Text',D.[" & ds.Tables(0).Rows(0).Item("DisplayName") & "] 'value'  from [V" & Session("EID") & arr(1).Replace(" ", "_") & "] s inner join " & StrViewName.ToString() & " on  s.tid=D.[" & ds.Tables(0).Rows(0).Item("DisplayName") & "] "
                '    End If
                '    StrQuery1 = "SELECT distinct  " & StrQuery1
                'End If
                'If ddlSourceForm.SelectedItem.Text.ToUpper = "USER" Then
                '    StrQuery1 = StrQuery1 & " WHERE D.EID=" & Session("EID")
                'Else
                '    StrQuery1 = StrQuery1 & " WHERE D.EID=" & Session("EID") & " and D.DocumentType='" & ddlSecondarySourceForm.SelectedItem.Text() & "'"
                'End If

                'Filling data to the concern dropdown
                ds = New DataSet()
                Using con As New SqlConnection(conStr)
                    Using da As New SqlDataAdapter(StrQuery1, con)
                        If con.State = ConnectionState.Closed Then
                            con.Open()
                        End If
                        da.Fill(ds)
                    End Using
                End Using
                If ds.Tables(0).Rows.Count > 0 Then
                    ddlSConditionValues.DataSource = ds
                    ddlSConditionValues.DataTextField = "Text"
                    ddlSConditionValues.DataValueField = "value"
                    ddlSConditionValues.DataBind()
                    ddlSConditionValues.Items.Insert("0", New ListItem("SELECT"))
                Else
                    If ddlSConditionValues.Items.Count > 0 Then
                        ddlSConditionValues.Items.Clear()
                        ddlSConditionValues.Items.Insert("0", New ListItem("SELECT"))
                    End If
                End If
            End If

            'If vals(0).ToString().ToUpper() = "MMM_MST_USER" Then
            '    sb.Append("Select distinct " & vals(1).ToString() & " as val from " & vals(0).ToString() & " where  eid=" & Session("EID") & " and " & vals(1).ToString() & " is not null")
            'Else
            '    sb.Append("Select distinct " & vals(1).ToString() & " as val from " & vals(0).ToString() & " where  DocumentType='" & ddlSourceForm.SelectedItem.Text.Trim() & "' and eid=" & Session("EID") & " and " & vals(1).ToString() & " is not null")
            'End If


            'Dim da1 As New SqlDataAdapter("", con)
            'da1.SelectCommand.CommandText = sb.ToString()
            'Dim dt As New DataTable
            'da1.Fill(dt)
            'If dt.Rows.Count > 0 Then
            '    ddlConditionValues.DataSource = dt
            '    ddlConditionValues.DataTextField = "val"
            '    ddlConditionValues.DataValueField = "val"
            '    ddlConditionValues.DataBind()
            '    ddlConditionValues.Items.Insert("0", New ListItem("SELECT"))
            'End If
            'ElseIf ddlSourceForm.SelectedItem.Text.Trim.ToUpper = "USER" Then
            '    sb.Append("select  name from sys.columns where object_id=object_id('mmm_mst_user') and name not like'fld%' union all select displayname from mmm_mst_fields where eid=" & Session("EID") & " and documenttype='user'")
            '    Dim da As New SqlDataAdapter("", con)
            '    da.SelectCommand.CommandText = sb.ToString()
            '    Dim dt As New DataTable
            '    da.Fill(dt)
            '    If dt.Rows.Count > 0 Then
            '        ddlConditionValues.DataSource = dt
            '        ddlConditionValues.DataTextField = "val"
            '        ddlConditionValues.DataValueField = "val"
            '        ddlConditionValues.DataBind()
            '    End If
            'End If
        Else

        End If
    End Sub

    Protected Sub imgAddCondition_Click(sender As Object, e As ImageClickEventArgs)
        'txtAddConditionArea.Attributes.Add("readonly", "readonly")
        'txtShowAddConditionArea.Attributes.Add("readonly", "readonly")
        Dim var As String() = ddlConditionDataFields.SelectedValue.Split("-")
        If ddlConditionDataFields.SelectedIndex <> 0 And ddlConditionValues.SelectedIndex <> 0 Then
            If txtAddConditionArea.Text.Contains("=") Then
                txtAddConditionArea.Text = txtAddConditionArea.Text & " " & ddlANDOR.SelectedValue.Trim() & " " & IIf(var.Length > 0, var(1).ToString(), ddlConditionValues.SelectedValue) & "=''" & ddlConditionValues.SelectedValue & "''"
                txtShowAddConditionArea.Text = txtShowAddConditionArea.Text & " " & ddlANDOR.SelectedValue.Trim() & " " & ddlConditionDataFields.SelectedItem.Text.Trim() & "=''" & ddlConditionValues.SelectedItem.Text & "''"
            Else
                txtAddConditionArea.Text = txtAddConditionArea.Text & " " & IIf(var.Length > 0, var(1).ToString(), ddlConditionValues.SelectedValue) & "=''" & ddlConditionValues.SelectedValue & "''"
                txtShowAddConditionArea.Text = txtShowAddConditionArea.Text & " " & ddlConditionDataFields.SelectedItem.Text.Trim() & "=''" & ddlConditionValues.SelectedItem.Text & "''"
            End If

        End If
    End Sub
    Protected Sub imgSAddCondition_Click(sender As Object, e As ImageClickEventArgs)
        'txtAddConditionArea.Attributes.Add("readonly", "readonly")
        'txtShowAddConditionArea.Attributes.Add("readonly", "readonly")
        Dim var As String() = ddlSConditionDataFields.SelectedValue.Split("-")
        If ddlSConditionDataFields.SelectedIndex <> 0 And ddlSConditionValues.SelectedIndex <> 0 Then
            If txtSAddConditionArea.Text.Contains("=") Then
                txtSAddConditionArea.Text = txtSAddConditionArea.Text & " " & ddlSANDOR.SelectedValue.Trim() & " " & IIf(var.Length > 0, var(1).ToString(), ddlSConditionValues.SelectedValue) & "=''" & ddlSConditionValues.SelectedValue & "''"
                txtSShowAddConditionArea.Text = txtSShowAddConditionArea.Text & " " & ddlSANDOR.SelectedValue.Trim() & " " & ddlSConditionDataFields.SelectedItem.Text.Trim() & "=''" & ddlSConditionValues.SelectedItem.Text & "''"
            Else
                txtSAddConditionArea.Text = txtSAddConditionArea.Text & " " & IIf(var.Length > 0, var(1).ToString(), ddlSConditionValues.SelectedValue) & "=''" & ddlSConditionValues.SelectedValue & "''"
                txtSShowAddConditionArea.Text = txtSShowAddConditionArea.Text & " " & ddlSConditionDataFields.SelectedItem.Text.Trim() & "=''" & ddlSConditionValues.SelectedItem.Text & "''"
            End If

        End If
    End Sub

    Protected Sub ddlMainSource_SelectedIndexChanged(sender As Object, e As EventArgs)
        Dim da As New SqlDataAdapter("", con)
        Dim dt As New DataTable
        If ddlMainSource.SelectedItem.Text.ToUpper = "PRIMARY" Then
            If ddlMainSource.SelectedValue.ToUpper = "USER" Then
                da.SelectCommand.CommandText = "select displayname,fieldmapping from mmm_mst_fields where documenttype in (select sourcedoc from mmm_schdoc_main where tid=" & ViewState("TID") & " and eid=" & Session("EID") & ") and eid=" & Session("EID") & " and isactive=1 union all select name as displayname,name as fieldMapping from sys.columns where object_id=object_id('mmm_mst_user') and name not like'fld%'"
            Else
                da.SelectCommand.CommandText = "select displayname,fieldmapping from mmm_mst_fields where documenttype in (select sourcedoc from mmm_schdoc_main where tid=" & ViewState("TID") & " and eid=" & Session("EID") & ") and eid=" & Session("EID") & " and isactive=1;"
            End If

            da.Fill(dt)
        Else
            If ddlMainSource.SelectedValue.ToUpper = "USER" Then
                da.SelectCommand.CommandText = "select displayname,fieldmapping from mmm_mst_fields where documenttype in (select SecondarySourceDoc from mmm_schdoc_main where tid=" & ViewState("TID") & " and eid=" & Session("EID") & ") and eid=" & Session("EID") & " and isactive=1 union all select name as displayname,name as fieldMapping from sys.columns where object_id=object_id('mmm_mst_user') and name not like'fld%'"
            Else
                da.SelectCommand.CommandText = "select displayname,fieldmapping from mmm_mst_fields where documenttype in (select SecondarySourceDoc from mmm_schdoc_main where tid=" & ViewState("TID") & " and eid=" & Session("EID") & ") and eid=" & Session("EID") & " and isactive=1;"
            End If

            da.Fill(dt)
        End If
        If dt.Rows.Count > 0 Then
            ddlSourceFldMapping.DataSource = dt
            ddlSourceFldMapping.DataTextField = "displayname"
            ddlSourceFldMapping.DataValueField = "fieldmapping"
            ddlSourceFldMapping.DataBind()
            ddlSourceFldMapping.Items.Insert("0", New ListItem("SELECT"))
            ddlSourceFldMapping.Items.Insert("1", New ListItem("TID"))
        End If
    End Sub

    Protected Sub ImgReferesh_Click(sender As Object, e As ImageClickEventArgs)
        txtAddConditionArea.Text = ""
        txtShowAddConditionArea.Text = ""
    End Sub
    Protected Sub ImgSReferesh_Click(sender As Object, e As ImageClickEventArgs)
        txtSAddConditionArea.Text = ""
        txtSShowAddConditionArea.Text = ""
    End Sub
    Protected Sub ClearFields()
        txtConfigName.Text = ""
        txtNoRows.Text = ""

        If ddlSourceForm.Items.Count > 0 Then
            ddlSourceForm.SelectedIndex = 0
        End If
        If ddlFormSourceType.Items.Count > 0 Then
            ddlFormSourceType.Items.Clear()
        End If
        If ddlSecondarySourceForm.Items.Count > 0 Then
            ddlSecondarySourceForm.Items.Clear()
        End If
        If ddlSecondarySourceType.Items.Count > 0 Then
            ddlSecondarySourceType.Items.Clear()
        End If
        If ddlConditionDataFields.Items.Count > 0 Then
            ddlConditionDataFields.Items.Clear()
        End If
        If ddlConditionValues.Items.Count > 0 Then
            ddlConditionValues.Items.Clear()
        End If
        txtShowAddConditionArea.Text = ""
        txtAddConditionArea.Text = ""
        If ddlSConditionDataFields.Items.Count > 0 Then
            ddlSConditionDataFields.Items.Clear()
        End If
        If ddlSConditionValues.Items.Count > 0 Then
            ddlSConditionValues.Items.Clear()
        End If
        txtSShowAddConditionArea.Text = ""
        txtSAddConditionArea.Text = ""
        ddlSourceForm.Attributes.Remove("disabled")
        If ddlTargetDoc.Items.Count > 0 Then
            ddlTargetDoc.SelectedIndex = 0
            ddlTargetDoc.Attributes.Remove("disabled")
        End If
        ddlCreator.SelectedIndex = 0
        ddlCreateEvent.SelectedIndex = 0
        If ddlWFStatus.Items.Count > 0 Then
            ddlWFStatus.Items.Clear()
        End If
        If ddlCreatorValue.Items.Count > 0 Then
            ddlCreatorValue.Items.Clear()
        End If
        txtMM.Text = ""
        txtDD.Text = ""
        txtWW.Text = ""
        txtHH.Text = ""
        txtMN.Text = ""
        chkisactive.Checked = False
        If chkCommanFields.Items.Count > 0 Then
            chkCommanFields.Items.Clear()
        End If
        commanfilter.Visible = False
    End Sub

    Protected Sub ddlTargetDoc_SelectedIndexChanged(sender As Object, e As EventArgs)
        If ddlTargetType.Items.Count > 0 Then
            ddlTargetType.Items.Clear()
        End If
        Dim strvalue As String() = ddlTargetDoc.SelectedValue.Split("-")
        If strvalue(0).ToString().ToUpper = "USER" Then
            ddlTargetType.Items.Insert(0, New ListItem("USER"))
        Else
            ddlTargetType.Items.Insert(0, New ListItem(Convert.ToString(strvalue(1))))
        End If

    End Sub

    Protected Sub ddlCreateEvent_SelectedIndexChanged(sender As Object, e As EventArgs)
        If ddlCreateEvent.SelectedIndex = 1 Then
            schtime.Visible = True
        Else
            schtime.Visible = False
        End If
    End Sub
    Public Shared Function BindDropDown(DRDDL As DataRow, EID As Integer) As String
        Dim StrResult = "0"
        Dim ds As New DataSet()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim strQuery As String = ""
        Dim con As SqlConnection = Nothing
        Dim sda As SqlDataAdapter = Nothing
        ' Dim tableName = ""
        Dim DocumentType = ""
        Dim FieldMapping = ""
        Dim TID As String = "TID"
        Dim arr = DRDDL.Item("DropDown").ToString().Split("-")
        Dim FieldID = DRDDL.Item("FieldID").ToString()
        Try
            'Getting table name
            Dim TABLENAME As String = ""
            If UCase(arr(0).ToString()) = "MASTER" Then
                TABLENAME = "MMM_MST_MASTER"
            ElseIf UCase(arr(0).ToString()) = "DOCUMENT" Then
                TABLENAME = "MMM_MST_DOC"
            ElseIf UCase(arr(0).ToString()) = "CHILD" Then
                TABLENAME = "MMM_MST_DOC_ITEM"
            ElseIf UCase(arr(0).ToString) = "SESSION" Then
                TABLENAME = "MMM_MST_USER"
                TID = "UID"
            ElseIf UCase(arr(0).ToString) = "STATIC" Then
                If arr(1).ToString.ToUpper = "USER" Then
                    TABLENAME = "MMM_MST_USER"
                    TID = "UID"
                ElseIf arr(1).ToString().ToUpper = "LOCATION" Then
                    TABLENAME = "MMM_MST_LOCATION"
                    TID = "LOCID"
                ElseIf arr(1).ToString() = "MMM_MST_Role" Then
                    TABLENAME = "MMM_MST_Role"
                    TID = "RoleName"
                End If
            End If
            Dim DSIntl = New DataSet()
            If TABLENAME = "MMM_MST_MASTER" Or TABLENAME = "MMM_MST_DOC" Then
                strQuery = "select " & arr(2).ToString() & "," & TID & "[tid]  from " & TABLENAME & " M WHERE   DOCUMENTTYPE='" & arr(1).ToString() & "'"
                strQuery = strQuery & "   AND EID= " & EID
                'For initial Filter
            ElseIf TABLENAME = "MMM_MST_USER" Then
                If DRDDL.Item("dropdowntype").ToString.ToUpper = "SESSION VALUED" Then
                    'For mobile they will supply userID
                    strQuery = "select " & arr(2).ToString() & "," & TID & " from " & TABLENAME & " M WHERE EID=" & EID & " " & ""
                Else
                    strQuery = "select " & arr(2).ToString() & "," & TID & " from " & TABLENAME & " M WHERE EID=" & EID & " " & ""
                    'For initial Filter
                End If
            ElseIf TABLENAME = "MMM_MST_LOCATION" Then
                strQuery = "select DISTINCT " & arr(2).ToString() & ",SID [tid]  from " & TABLENAME & " M "
            End If
            'Query For drop Down Filter
            con = New SqlConnection(conStr)
            con.Open()
            If strQuery <> "" Then
                sda = New SqlDataAdapter(strQuery, con)
                ds = New DataSet()
                sda.Fill(ds)
                If ds.Tables(0).Rows.Count > 0 Then
                    StrResult = ds.Tables(0).Rows(0).Item(TID).ToString()
                Else
                    StrResult = "-1"
                End If
            End If
        Catch ex As Exception
            StrResult = "-1"
        Finally
            If Not con Is Nothing Then
                con.Close()
                con.Dispose()
            End If
            If Not sda Is Nothing Then
                sda.Dispose()
            End If
        End Try
        Return StrResult
    End Function

    Protected Sub isSecCreator_CheckedChanged(sender As Object, e As EventArgs)
        If isSecCreator.Checked = True Then
            Primary.Visible = False
            Secondary.Visible = True
            If ddlSecondarySourceForm.SelectedIndex <> 0 And ddlSecondarySourceForm.SelectedIndex <> -1 Then
                If ddlSecSourceCreator.Items(1).Text = "UID" Or ddlSecSourceCreator.Items(1).Text = "OUID" Then
                    ddlSecSourceCreator.Items.RemoveAt(1)
                End If
                If ddlSecSourceCreatorValue.Items.Count > 0 Then
                    ddlSecSourceCreatorValue.Items.Clear()
                End If
                If ddlSecondarySourceForm.SelectedItem.Text = "USER" Then
                    ddlSecSourceCreator.Items.Insert("1", New ListItem("UID"))
                Else
                    ddlSecSourceCreator.Items.Insert("1", New ListItem("OUID"))
                End If
            Else
                If ddlSecSourceCreator.Items(1).Text = "UID" Or ddlSecSourceCreator.Items(1).Text = "OUID" Then
                    ddlSecSourceCreator.Items.RemoveAt(1)
                End If
                If ddlSecSourceCreatorValue.Items.Count > 0 Then
                    ddlSecSourceCreatorValue.Items.Clear()
                End If
            End If
        Else
            Primary.Visible = True
            Secondary.Visible = False
        End If
    End Sub

    Protected Sub ddlSecSourceCreator_SelectedIndexChanged(sender As Object, e As EventArgs)
        Dim da As New SqlDataAdapter("", con)

        Dim dt As New DataTable()

        If ddlSecSourceCreator.SelectedIndex = 0 Then
            If ddlSecSourceCreatorValue.Items.Count > 0 Then
                ddlSecSourceCreatorValue.Items.Clear()
            End If
        ElseIf ddlSecSourceCreator.SelectedIndex = 1 Then

            If dt.Rows.Count > 0 Then
                If ddlSecSourceCreatorValue.Items.Count > 0 Then
                    ddlSecSourceCreatorValue.Items.Clear()
                End If
                ddlSecSourceCreatorValue.Items.Insert("0", New ListItem("SELECT"))
                ddlSecSourceCreatorValue.Items.Insert("1", New ListItem(ddlSecSourceCreator.SelectedItem.Text.Trim()))
            Else
                If ddlSecSourceCreatorValue.Items.Count > 0 Then
                    ddlSecSourceCreatorValue.Items.Clear()
                End If
                ddlSecSourceCreatorValue.Items.Insert("0", New ListItem("SELECT"))
                ddlSecSourceCreatorValue.Items.Insert("1", New ListItem(ddlSecSourceCreator.SelectedItem.Text.Trim()))
            End If
        ElseIf ddlSecSourceCreator.SelectedIndex = 2 Then
            da.SelectCommand.CommandText = "select RoleName from mmm_mst_role where eid=" & Session("EID")
            da.SelectCommand.CommandType = CommandType.Text
            da.Fill(dt)
            If dt.Rows.Count > 0 Then
                ddlSecSourceCreatorValue.DataSource = dt
                ddlSecSourceCreatorValue.DataTextField = "RoleName"
                ddlSecSourceCreatorValue.DataValueField = "RoleName"
                ddlSecSourceCreatorValue.DataBind()
                ddlSecSourceCreatorValue.Items.Insert("0", New ListItem("SELECT"))
            Else
                ddlSecSourceCreatorValue.DataSource = Nothing
                ddlSecSourceCreatorValue.DataBind()
            End If
        ElseIf ddlSecSourceCreator.SelectedIndex = 3 Then
            da.SelectCommand.CommandText = "select displayname,fieldmapping from mmm_mst_fields where documenttype='" & ddlSourceForm.SelectedItem.Text & "' and eid=" & Session("EID") & " and isactive=1 and invisible=0 and (fieldtype ='Drop down' or fieldtype ='LookupDDL' or fieldtype ='Multi LookupDDL')"
            da.SelectCommand.CommandType = CommandType.Text
            da.Fill(dt)
            If dt.Rows.Count > 0 Then
                ddlSecSourceCreatorValue.DataSource = dt
                ddlSecSourceCreatorValue.DataTextField = "displayname"
                ddlSecSourceCreatorValue.DataValueField = "fieldmapping"
                ddlSecSourceCreatorValue.DataBind()
                ddlSecSourceCreatorValue.Items.Insert("0", New ListItem("SELECT"))
            Else
                ddlSecSourceCreatorValue.DataSource = Nothing
                ddlSecSourceCreatorValue.DataBind()
            End If
        ElseIf ddlSecSourceCreator.SelectedIndex = 4 Then

            If dt.Rows.Count > 0 Then
                If ddlSecSourceCreatorValue.Items.Count > 0 Then
                    ddlSecSourceCreatorValue.Items.Clear()
                End If
                ddlSecSourceCreatorValue.Items.Insert("0", New ListItem("SELECT"))
                ddlSecSourceCreatorValue.Items.Insert("1", New ListItem("UID"))
            Else
                If ddlSecSourceCreatorValue.Items.Count > 0 Then
                    ddlSecSourceCreatorValue.Items.Clear()
                End If
                ddlSecSourceCreatorValue.Items.Insert("0", New ListItem("SELECT"))
                ddlSecSourceCreatorValue.Items.Insert("1", New ListItem("UID"))
            End If
        End If

    End Sub
End Class
