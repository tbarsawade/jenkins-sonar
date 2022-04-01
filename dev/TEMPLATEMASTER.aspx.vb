Imports System.Data
Imports System.Data.SqlClient

Partial Class TEMPLATEMASTER
    Inherits System.Web.UI.Page
    Protected Sub Add(ByVal sender As Object, ByVal e As System.EventArgs)
        lblMsgEdit.Text = ""
        btnActEdit.Text = "Save"
        clearcontrol()
        updatePanelEdit.Update()
        Me.btnEdit_ModalPopupExtender.Show()
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
    Protected Sub btnActEdit_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnActEdit.Click
        Dim ccField As String = ""
        Dim Cntselect As Int32 = 0
        Dim PID As Integer = 0
        Dim TYPE As Integer = 0
        Dim COUNT As Integer = 0
        Dim qry As String = ""
        Dim qrychild1 As String = ""
        Dim qrychild2 As String = ""
        Dim CC As String = ""
        '' check template Name 

        If txtName.Text = "" Then
            lblMsgEdit.Text = "Please Enter Template Name"
            txtName.Focus()
            Exit Sub
        End If
        ''Check for Subject Line
        If txtSubject.Text = "" Then
            lblMsgEdit.Text = "Please Enter a Valid Subject Line"
            txtSubject.Focus()
            Exit Sub
        End If

        ''Check for Msg Body
        If txtBody.Text = "" Then
            lblMsgEdit.Text = "Message Body can't be blank."
            txtBody.Focus()
            Exit Sub
        End If

        If ddlAction.SelectedItem.Text.ToUpper = "SMS ALERT" Then

        Else
            ''Check for Sub Event 
            If UCase(ddlSBE.SelectedItem.Text) = "SELECT ONE" Then
                lblMsgEdit.Text = "Please Select SUB Event"
                ddlSBE.Focus()
                Exit Sub
            End If

            '' check for wordflow status
            '' check child item
            'Dim childDataTable As DataTable = GetChildItemByDocumentType(ddlEvent.SelectedValue)

            If UCase(ddlAction.SelectedItem.Text) = "GPS ALERT" Then
                If UCase(ddlWS.SelectedItem.Text) = "SELECT ROLE" Then
                    lblMsgEdit.Text = "Please Select Role."
                    ddlWS.Focus()
                    Exit Sub
                End If
            Else
                '' check Event Name is selected or not
                If UCase(ddlEvent.SelectedItem.Text) = "SELECT EVENT" Then
                    lblMsgEdit.Text = "Please Select a Event"
                    ddlEvent.Focus()
                    Exit Sub
                End If
                If UCase(ddlWS.SelectedItem.Text) = "SELECT STATUS" Then
                    lblMsgEdit.Text = "Please Select Workflow status."
                    ddlWS.Focus()
                    Exit Sub
                End If



                Dim ccList As String = ""
                Dim ToList As String = ""
                Dim Cnt As Integer = 0

                ''Check for mailto
                For i As Integer = 0 To ddlMailto.Items.Count - 1
                    If ddlMailto.Items(i).Selected = True Then
                        ToList = ToList & ddlMailto.Items(i).Text & ","
                        Cnt = Cnt + 1
                    End If
                Next
                If Cnt = 0 Then
                    lblMsgEdit.Text = "Please select atleast one user in To mail list"
                    ddlMailto.Focus()
                    Exit Sub
                End If

                ToList = ToList.Substring(0, ToList.Length - 1)
                Cnt = 0
                ' If UCase(ddlMailto.SelectedItem.Text) = "SELECT ONE" Then
                ' lblMsgEdit.Text = "Please Enter Receiver of Mail."
                ' ddlMailto.Focus()
                ' Exit Sub
                ' End If

                'If UCase(DDLCC.SelectedItem.Text) <> "SELECT" Then
                '    CC = DDLCC.SelectedValue
                'End If

                For i As Integer = 0 To DDLCC.Items.Count - 1
                    If DDLCC.Items(i).Selected = True Then
                        ccList = ccList & DDLCC.Items(i).Text & ","
                        Cnt = Cnt + 1
                    End If
                Next


                'If Cnt = 0 Then
                '    lblMsgEdit.Text = "Please select atleast one user in CC mail list"
                '    DDLCC.Focus()
                '    Exit Sub
                'End If
                If ccList.Length > 0 Then
                    ccList = ccList.Substring(0, ccList.Length - 1)
                End If
            End If

        End If

        If ddlAction.SelectedValue = "2" Then
            For i As Integer = 0 To methodckblist.Items.Count - 1
                If methodckblist.Items(i).Selected = True Then
                    ccField = ccField & methodckblist.Items(i).Value & ","
                    Cntselect = Cntselect + 1
                End If
            Next
            If ccField.Length > 0 Then
                ccField = ccField.Remove(ccField.Length - 1)
            End If

        End If
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim da As New SqlDataAdapter("", con)
        If con.State = ConnectionState.Closed Then
            con.Open()
        End If

        da.SelectCommand.CommandText = "select * from mmm_mst_fields where   eid=" & Session("eid") & " and  documentType='" & ddlEvent.SelectedValue & "' "
        Dim DtF_ChildItem As New DataTable
        da.Fill(DtF_ChildItem)
        Dim childItemValue As ArrayList = New ArrayList()
        Dim childitem() As DataRow = DtF_ChildItem.Select("Fieldtype='CHILD ITEM'")
        If Not IsNothing(childitem) Then
            For Each DR_item As DataRow In childitem
                childItemValue.Add(DR_item.Item("dropdown"))
            Next

        End If

        '    ddlWS.Items.Clear()
        If UCase(ddlAction.SelectedItem.Text) <> "GPS ALERT" Then
            da.SelectCommand.CommandText = String.Empty
            da.SelectCommand.CommandText = "select * from MMM_MST_FORMS where eid='" & Session("eid") & "' and formname='" & ddlEvent.SelectedValue & "'"
            Dim DtF As New DataTable
            da.Fill(DtF)
            Dim isWF As Integer
            If DtF.Rows.Count <> 0 Then
                isWF = DtF.Rows(0).Item("isworkflow")
            Else
                isWF = 0
            End If

            DtF.Clear()
            If isWF = 1 Then
                da.SelectCommand.CommandText = "select DISPLAYNAME,FieldMapping,DBTABLENAME FROM MMM_MST_FIELDS where DOCUMENTTYPE='WORKFLOW EVENT'  AND EID=0 order by displayorder"
                da.Fill(DtF)
            End If
            qrychild1 &= "SELECT "
            qrychild2 &= "SELECT "
            qry &= "SELECT "

            If DtF.Rows.Count > 0 Then
                For Each DR As DataRow In DtF.Rows
                    qry &= " " & DR.Item("FieldMapping") & " [" & DR.Item("DISPLAYNAME") & "],"
                Next
                ' qry = qry.Substring(0, qry.Length - 1)
            End If

            Dim dtN As New DataTable
            Dim DT As New DataTable

            da.SelectCommand.CommandType = CommandType.StoredProcedure
            da.SelectCommand.CommandText = "uspGetSelQueryforEmailTemplates"
            da.SelectCommand.Parameters.Clear()
            da.SelectCommand.Parameters.AddWithValue("EID", Session("eid").ToString)
            da.SelectCommand.Parameters.AddWithValue("FN", ddlEvent.SelectedValue)

            Dim result As String
            result = da.SelectCommand.ExecuteScalar().ToString()

            qry = Replace(result, "SELECT distinct", qry, 1, 1)




            For i As Integer = 0 To childItemValue.Count - 1
                da.SelectCommand.CommandType = CommandType.StoredProcedure
                da.SelectCommand.CommandText = "uspGetSelQueryforEmailTemplates"
                da.SelectCommand.Parameters.Clear()
                da.SelectCommand.Parameters.AddWithValue("EID", Session("eid").ToString)
                da.SelectCommand.Parameters.AddWithValue("FN", childItemValue(i).ToString())
                Dim resultchild1 As String
                resultchild1 = da.SelectCommand.ExecuteScalar().ToString()
                qrychild1 = Replace(resultchild1, "SELECT distinct", qrychild1, 1, 1)
            Next

            '-- da.Fill(DT)

            '''' prev code 
            'qry &= "SELECT "
            'DT = GetVariable(ddlEvent.SelectedValue)

            'For Each DR As DataRow In DT.Rows
            '    qry &= " " & DR.Item("FieldMapping") & " [" & DR.Item("DISPLAYNAME") & "],"
            'Next

            'qry = qry.Substring(0, qry.Length - 1
            'qry &= " FROM  " & DT.Rows(0).Item("DBTABLENAME") & " "
            '''' prev code ends here 
        End If

        Dim htmlcode As String = HEE_body.Decode(txtBody.Text)
        'Dim htmlcode As String = HttpUtility.HtmlEncode(txtBody.Text)

        If UCase(btnActEdit.Text) = "SAVE" Then
            PID = 0
            TYPE = 0
            da.SelectCommand.Parameters.Clear()
            da.SelectCommand.CommandType = CommandType.Text
            da.SelectCommand.CommandText = "SELECT COUNT(*) FROM MMM_MST_TEMPLATE WHERE TEMPLATE_NAME='" & txtName.Text & "'AND EID=" & Session("eid") & ""
            COUNT = da.SelectCommand.ExecuteScalar()
        ElseIf UCase(btnActEdit.Text) = "UPDATE" Then
            PID = ViewState("pid")
            TYPE = 1
            da.SelectCommand.CommandType = CommandType.Text
            da.SelectCommand.CommandText = "SELECT COUNT(*) FROM MMM_MST_TEMPLATE WHERE TEMPLATE_NAME='" & txtName.Text & "'AND TID<> " & PID & " AND EID=" & Session("eid") & ""
            COUNT = da.SelectCommand.ExecuteScalar()
        End If
        If COUNT > 0 Then
            lblMsgEdit.Text = "TEMPLATE NAME ALREADY EXISTS."

            Exit Sub
        End If

        Dim ddlmail As String = ""

        If UCase(ddlAction.SelectedItem.Text) <> "GPS ALERT" Then

            For i As Integer = 0 To ddlMailto.Items.Count - 1
                If ddlMailto.Items(i).Selected Then
                    ddlmail += ddlMailto.Items(i).Text & ","
                End If
            Next

            For i As Integer = 0 To DDLCC.Items.Count - 1
                If DDLCC.Items(i).Selected Then
                    CC += DDLCC.Items(i).Text & ","
                End If
            Next

            If ddlmail.Length > 0 Then ddlmail = ddlmail.Substring(0, ddlmail.Length - 1)
            If CC.Length > 0 Then CC = CC.Substring(0, CC.Length - 1)


            'For i As Integer = 0 To 
            '    If ddlMailto.Items(i).Selected Then
            '        ddlmail += ddlMailto.Items(i).Text & ";"
            '    End If
            'Next


            da.SelectCommand.CommandType = CommandType.StoredProcedure
            da.SelectCommand.CommandText = "usp_INSERT_TEMPLATE_NEW2"
            da.SelectCommand.Parameters.Clear()




            If ddlAction.SelectedItem.Text.ToUpper = "SMS ALERT" Then
                da.SelectCommand.Parameters.AddWithValue("TN", txtName.Text)
                da.SelectCommand.Parameters.AddWithValue("ACTION", ddlAction.SelectedValue)
                da.SelectCommand.Parameters.AddWithValue("MSG", htmlcode)
                da.SelectCommand.Parameters.AddWithValue("EVENT", "SMS")
                da.SelectCommand.Parameters.AddWithValue("SUBEVENT", "SMS")
                da.SelectCommand.Parameters.AddWithValue("EID", Session("eid"))
                da.SelectCommand.Parameters.AddWithValue("SUBJECT", txtSubject.Text)
                da.SelectCommand.Parameters.AddWithValue("mailto", ddlmail)
                da.SelectCommand.Parameters.AddWithValue("CCLIST", CC)
                da.SelectCommand.Parameters.AddWithValue("wfStatus", 0)
                da.SelectCommand.Parameters.AddWithValue("BCC", txtBcc.Text)


                'da.SelectCommand.Parameters.AddWithValue("@BCC", "")

                da.SelectCommand.Parameters.AddWithValue("TID", PID)
                da.SelectCommand.Parameters.AddWithValue("TYPE", TYPE)
                da.SelectCommand.Parameters.AddWithValue("qry", "SMS")


                da.SelectCommand.Parameters.AddWithValue("Asladay", 0)
                da.SelectCommand.Parameters.AddWithValue("Bsladay", 0)
                'add column in database@Bsladay
                da.SelectCommand.Parameters.AddWithValue("sHH", txtHH.Text)
                da.SelectCommand.Parameters.AddWithValue("sMM", txtMM.Text)
                da.SelectCommand.Parameters.AddWithValue("aprtype", ddlatype.SelectedItem.Text)
                da.SelectCommand.Parameters.AddWithValue("DOCNATURE", ddldocnature.SelectedItem.Text)
                da.SelectCommand.Parameters.AddWithValue("FieldName", ccField)
                da.SelectCommand.Parameters.AddWithValue("Condition", Conditiontxt.Text)
                da.SelectCommand.Parameters.AddWithValue("Userfield", IIf(ddlufield.SelectedValue.ToString.ToUpper = "SELECT", "", ddlufield.SelectedValue.ToString))
                ' public view of document 1-jan-14
                'If chkPublicView.Checked = True Then
                '    da.SelectCommand.Parameters.AddWithValue("PublicView", 1)
                '    da.SelectCommand.Parameters.AddWithValue("pvDocType", ddlPvdoctype.SelectedItem.Text)
                '    da.SelectCommand.Parameters.AddWithValue("pvCaption", txtPvCaption.Text)
                '    da.SelectCommand.Parameters.AddWithValue("pvMode", ddlPvMode.SelectedItem.Text)
                '    da.SelectCommand.Parameters.AddWithValue("pvRelationship", ddlPvRelationsip.SelectedItem.Text)
                'Else
                '    da.SelectCommand.Parameters.AddWithValue("PublicView", 0)
                'End If

            Else
                da.SelectCommand.Parameters.AddWithValue("TN", txtName.Text)
                da.SelectCommand.Parameters.AddWithValue("ACTION", ddlAction.SelectedValue)
                da.SelectCommand.Parameters.AddWithValue("MSG", htmlcode)
                da.SelectCommand.Parameters.AddWithValue("EVENT", ddlEvent.SelectedItem.Text)
                da.SelectCommand.Parameters.AddWithValue("SUBEVENT", ddlSBE.SelectedItem.Text)
                da.SelectCommand.Parameters.AddWithValue("EID", Session("eid"))
                da.SelectCommand.Parameters.AddWithValue("SUBJECT", txtSubject.Text)
                da.SelectCommand.Parameters.AddWithValue("mailto", ddlmail)
                da.SelectCommand.Parameters.AddWithValue("CCLIST", CC)
                da.SelectCommand.Parameters.AddWithValue("wfStatus", ddlWS.SelectedItem.Value())
                da.SelectCommand.Parameters.AddWithValue("BCC", txtBcc.Text)

                'da.SelectCommand.Parameters.AddWithValue("@BCC", "")

                da.SelectCommand.Parameters.AddWithValue("TID", PID)
                da.SelectCommand.Parameters.AddWithValue("TYPE", TYPE)
                da.SelectCommand.Parameters.AddWithValue("qry", qry)
                da.SelectCommand.Parameters.AddWithValue("qry_child1", qrychild1)


                da.SelectCommand.Parameters.AddWithValue("Asladay", txtAdays.Text)
                da.SelectCommand.Parameters.AddWithValue("Bsladay", txtBDays.Text)
                'add column in database@Bsladay
                da.SelectCommand.Parameters.AddWithValue("sHH", txtHH.Text)
                da.SelectCommand.Parameters.AddWithValue("sMM", txtMM.Text)
                da.SelectCommand.Parameters.AddWithValue("aprtype", ddlatype.SelectedItem.Text)
                da.SelectCommand.Parameters.AddWithValue("DOCNATURE", ddldocnature.SelectedItem.Text)
                da.SelectCommand.Parameters.AddWithValue("FieldName", ccField)
                da.SelectCommand.Parameters.AddWithValue("Condition", Conditiontxt.Text)
                da.SelectCommand.Parameters.AddWithValue("Userfield", IIf(ddlufield.SelectedValue.ToString.ToUpper = "SELECT", "", ddlufield.SelectedValue.ToString))
                'If chkPublicView.Checked = True Then
                '    da.SelectCommand.Parameters.AddWithValue("PublicView", 1)
                '    da.SelectCommand.Parameters.AddWithValue("pvDocType", ddlPvdoctype.SelectedItem.Text)
                '    da.SelectCommand.Parameters.AddWithValue("pvCaption", txtPvCaption.Text)
                '    da.SelectCommand.Parameters.AddWithValue("pvMode", ddlPvMode.SelectedItem.Text)
                '    da.SelectCommand.Parameters.AddWithValue("pvRelationship", ddlPvRelationsip.SelectedItem.Text)
                'Else
                '    da.SelectCommand.Parameters.AddWithValue("PublicView", 0)
                'End If


            End If

        Else
            da.SelectCommand.CommandType = CommandType.StoredProcedure
            da.SelectCommand.CommandText = "usp_INSERT_TEMPLATE_NEW2"
            da.SelectCommand.Parameters.Clear()


            If ddlAction.SelectedItem.Text.ToUpper = "SMS ALERT" Then
                da.SelectCommand.Parameters.AddWithValue("mailto", ddlmail)
                da.SelectCommand.Parameters.AddWithValue("CCLIST", CC)
                da.SelectCommand.Parameters.AddWithValue("TN", txtName.Text)
                da.SelectCommand.Parameters.AddWithValue("ACTION", ddlAction.SelectedValue)
                da.SelectCommand.Parameters.AddWithValue("MSG", htmlcode)
                da.SelectCommand.Parameters.AddWithValue("EVENT", "SMS")
                da.SelectCommand.Parameters.AddWithValue("SUBEVENT", "SMS")
                da.SelectCommand.Parameters.AddWithValue("EID", Session("eid"))
                da.SelectCommand.Parameters.AddWithValue("SUBJECT", txtSubject.Text)
                da.SelectCommand.Parameters.AddWithValue("wfStatus", 0)
                da.SelectCommand.Parameters.AddWithValue("BCC", txtBcc.Text)
                da.SelectCommand.Parameters.AddWithValue("TID", PID)
                da.SelectCommand.Parameters.AddWithValue("TYPE", TYPE)
                da.SelectCommand.Parameters.AddWithValue("qry", "SMS")
                da.SelectCommand.Parameters.AddWithValue("Asladay", 0)
                da.SelectCommand.Parameters.AddWithValue("Bsladay", 0)
                'add column in database@Bsladay
                da.SelectCommand.Parameters.AddWithValue("sHH", txtHH.Text)
                da.SelectCommand.Parameters.AddWithValue("sMM", txtMM.Text)
                da.SelectCommand.Parameters.AddWithValue("aprtype", ddlatype.SelectedItem.Text)
                da.SelectCommand.Parameters.AddWithValue("DOCNATURE", ddldocnature.SelectedItem.Text)
                da.SelectCommand.Parameters.AddWithValue("FieldName", ccField)
                da.SelectCommand.Parameters.AddWithValue("Condition", Conditiontxt.Text)
                da.SelectCommand.Parameters.AddWithValue("Userfield", IIf(ddlufield.SelectedValue.ToString.ToUpper = "SELECT", "", ddlufield.SelectedValue.ToString))
            Else
                da.SelectCommand.Parameters.AddWithValue("mailto", ddlmail)
                da.SelectCommand.Parameters.AddWithValue("CCLIST", CC)
                da.SelectCommand.Parameters.AddWithValue("TN", txtName.Text)
                da.SelectCommand.Parameters.AddWithValue("ACTION", ddlAction.SelectedValue)
                da.SelectCommand.Parameters.AddWithValue("MSG", htmlcode)
                da.SelectCommand.Parameters.AddWithValue("EVENT", DBNull.Value)
                da.SelectCommand.Parameters.AddWithValue("SUBEVENT", ddlSBE.SelectedItem.Text)
                da.SelectCommand.Parameters.AddWithValue("EID", Session("eid"))
                da.SelectCommand.Parameters.AddWithValue("SUBJECT", txtSubject.Text)
                da.SelectCommand.Parameters.AddWithValue("wfStatus", ddlWS.SelectedItem.Value())
                da.SelectCommand.Parameters.AddWithValue("BCC", txtBcc.Text)
                da.SelectCommand.Parameters.AddWithValue("TID", PID)
                da.SelectCommand.Parameters.AddWithValue("TYPE", TYPE)
                da.SelectCommand.Parameters.AddWithValue("qry", DBNull.Value)
                da.SelectCommand.Parameters.AddWithValue("Asladay", txtAdays.Text)
                da.SelectCommand.Parameters.AddWithValue("Bsladay", txtBDays.Text)
                'add column in database@Bsladay
                da.SelectCommand.Parameters.AddWithValue("sHH", txtHH.Text)
                da.SelectCommand.Parameters.AddWithValue("sMM", txtMM.Text)

                da.SelectCommand.Parameters.AddWithValue("aprtype", ddlatype.SelectedItem.Text)
                da.SelectCommand.Parameters.AddWithValue("DOCNATURE", ddldocnature.SelectedItem.Text)
                da.SelectCommand.Parameters.AddWithValue("FieldName", ccField)
                da.SelectCommand.Parameters.AddWithValue("Condition", Conditiontxt.Text)
                da.SelectCommand.Parameters.AddWithValue("Userfield", IIf(ddlufield.SelectedValue.ToString.ToUpper = "SELECT", "", ddlufield.SelectedValue.ToString))
            End If
        End If


        Dim INS As String = da.SelectCommand.ExecuteScalar()
        lblMsg1.Text = INS
        Me.btnEdit_ModalPopupExtender.Hide()
        getsearchresult()
        updPnlGrid.Update()
        da.Dispose()
        con.Close()
    End Sub
    Protected Sub RefreshPanel(ByVal sender As Object, ByVal e As EventArgs)

    End Sub
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load


        If Not IsPostBack Then
            methodselectid.Visible = False
            ' session("eid") = "0"
            txtvar_chil1.Visible = False
            txtvar_chil2.Visible = False
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As New SqlConnection(conStr)
            'fill Product  
            Dim da As New SqlDataAdapter("select disname,colname FROM MMM_MST_SEARCH where tableName='TEMPLATE'", con)
            Dim ds As New DataSet

            da.Fill(ds, "data")
            For i As Integer = 0 To ds.Tables("data").Rows.Count - 1
                ddlField.Items.Add(ds.Tables("data").Rows(i).Item(0))
                ddlField.Items(i).Value = ds.Tables("data").Rows(i).Item(1)
            Next

            ' da.SelectCommand.CommandText = "SELECT DISTINCT DOCUMENTTYPE, DOCUMENTTYPE FROM MMM_MST_FIELDS WHERE EID='" & session("eid") & "' OR EID=0"
            da.SelectCommand.CommandText = "select distinct fl.DOCUMENTTYPE from MMM_MST_FORMS F inner join MMM_MST_FIELDS FL on FL.DocumentType=f.FormName where (fl.EID='" & Session("eid") & "' or FL.EID=0) and f.FormSource in ('MENU DRIVEN') "
            da.Fill(ds, "type")
            For i As Integer = 0 To ds.Tables("type").Rows.Count - 1
                ddlEvent.Items.Add(ds.Tables("type").Rows(i).Item(0))
                'ddlEvent.Items(i).Value = ds.Tables("type").Rows(i).Item(1)
            Next

            ddlEvent.Items.Insert(0, "SELECT EVENT")
            ddlSBE.Items.Insert(0, "SELECT ONE")
            '' new added ends here

            da.SelectCommand.CommandText = "SELECT MSGBODY FROM MMM_MST_TEMPLATE where eid=" & Session("EID")


            da.SelectCommand.CommandTimeout = 5000
            da.Fill(ds, "MSG")
            If ds.Tables("MSG").Rows.Count > 0 Then
                lblMsgEdit.Text = ds.Tables("MSG").Rows(0).Item(0).ToString()
            Else
                lblMsgEdit.Text = ""
            End If




            da.Dispose()
            con.Dispose()
            ds.Dispose()
            getsearchresult()
        End If
    End Sub
    'Private Function GetChildItemByDocumentType(ByVal DocType As String) As DataTable
    '    Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
    '    Dim con As New SqlConnection(conStr)
    '    Dim DA As SqlDataAdapter = New SqlDataAdapter("select * from mmm_mst_fields where   eid=" & Session("eid") & " and  documentType=" & DocType.ToString & " ", con)
    '    Dim DtF As New DataTable
    '    ddlWS.Items.Clear()
    '    DA.Fill(DtF)
    '    Return DtF
    'End Function


    Protected Sub ddlEvent_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlEvent.SelectedIndexChanged
        'fill Product  
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim DA As SqlDataAdapter = New SqlDataAdapter("select * from MMM_MST_FORMS form inner join MMM_MST_FIELDS field on form.eid=field.eid and form.formname=field.documentType where form.eid='" & Session("eid") & "' and form.formname='" & ddlEvent.SelectedValue & "'", con)
        Dim DtF As New DataTable
        ddlWS.Items.Clear()
        DA.Fill(DtF)
        Dim childitem() As DataRow = DtF.Select("Fieldtype='CHILD ITEM'")
        If Not IsNothing(DtF.Select("FieldType='CHILD ITEM'")) Then
            '' add here child item variable
            '' get the child item value and put in select query
            txtvar_chil1.Visible = True
            If childitem.Length > 0 Then
                For Each DR As DataRow In childitem
                    Dim tran As SqlTransaction = Nothing
                    '' new added for saving differently if def. value feature is on
                    Dim strDF As String = "select * from mmm_mst_fields where documentType='" & DR.Item("DROPDOWN") & "' and EID=" & Session("EID") & ""
                    Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
                    Dim dtIsdv As New DataTable
                    oda.SelectCommand.CommandText = strDF
                    oda.SelectCommand.Transaction = tran
                    oda.Fill(dtIsdv)
                    If dtIsdv.Rows.Count > 0 Then
                        DA = New SqlDataAdapter("select * from MMM_MST_FIELDS  where eid='" & Session("eid") & "' documentType='" & ddlEvent.SelectedValue & "'", con)
                        Dim dtSChild As DataTable
                        dtSChild = GetVariable(DR.Item("DROPDOWN"))
                        Dim Childvariables As String = ""
                        For i As Integer = 0 To dtSChild.Rows.Count - 1
                            Childvariables &= "{" & dtSChild.Rows(i).Item(0).ToString() & "}"
                            Childvariables &= Environment.NewLine
                        Next
                        Childvariables &= "{DOCUMENT PUBLIC VIEW LINK}" ' adding  default public view for link 
                        txtvar_chil1.Text = Childvariables
                    End If
                Next
            End If
        End If
        Dim isWF As Integer
        If DtF.Rows.Count <> 0 Then
            isWF = DtF.Rows(0).Item("isworkflow")
        Else
            isWF = 0
        End If

        ddlSBE.Items.Clear()
        ddlMailto.Items.Clear()
        DDLCC.Items.Clear()
        Dim dtS As DataTable
        dtS = GetVariable(ddlEvent.SelectedValue)
        Dim variables As String = ""
        For i As Integer = 0 To dtS.Rows.Count - 1
            variables &= "{" & dtS.Rows(i).Item(0).ToString() & "}"
            variables &= Environment.NewLine
        Next
        variables &= "{DOCUMENT PUBLIC VIEW LINK}" ' adding  default public view for link 
        txtvar.Text = variables

        ''BIND MAIL VARIABLES
        ''BIND SUB EVENT
        If isWF = 1 Then
            BindMailTo("WORKFLOW EVENT")
            BindSubEvent("WORKFLOW EVENT")
            If ddlAction.SelectedItem.Value = 2 Then
                BindSubEvent("WORKFLOW EVENT")
            Else
                ddlSBE.Items.Clear()
                ddlSBE.Items.Add("SLA EXPIRED")
                ddlSBE.Items.Add("DOCUMENT EXPIRY")

                ddlSBE.Items.Insert(0, "SELECT ONE")
                '  ddlSBE.Items.Add("GPS ALERT")
            End If
        Else
            BindMailTo(ddlEvent.SelectedValue)
            If ddlAction.SelectedItem.Value = 2 Then
                BindSubEvent("WORKFLOW EVENT")
            Else
                ddlSBE.Items.Add("SLA EXPIRED")
                ddlSBE.Items.Add("DOCUMENT EXPIRY")

                ddlSBE.Items.Insert(0, "SELECT ONE")
                ' ddlSBE.Items.Add("GPS ALERT")
            End If
        End If
        '' new added for filling workflow status ddl - by sunil 
        If isWF = 1 Then
            'If ddlSBE.SelectedItem.Text = "APPROVE" Or ddlSBE.SelectedItem.Text = "CREATED" Then
            DA.SelectCommand.CommandText = "SELECT * FROM MMM_MST_WORKFLOW_STATUS WHERE EID='" & Session("eid") & "' AND DocumentType='" & ddlEvent.SelectedItem.Text & "'"
            Dim dtW As New DataTable
            DA.Fill(dtW)
            ddlWS.Items.Clear()
            ddlWS.Items.Add("ALL")
            If ddlAction.SelectedItem.Value = 1 Or ddlAction.SelectedItem.Value = 2 Then
                ddlWS.Items.Add("UPLOADED")
            End If
            For i As Integer = 0 To dtW.Rows.Count - 1
                ddlWS.Items.Add(dtW.Rows(i).Item("statusName"))
                'ddlEvent.Items(i).Value = ds.Tables("type").Rows(i).Item(1)
            Next
            ddlWS.Items.Add("ARCHIVE")
            ddlWS.Enabled = True
            dtW.Dispose()
        Else
            '' new added for filling workflow status ddl - by sunil 
            ddlWS.Items.Clear()
            ddlWS.Items.Insert(0, "NOT REQUIRED")
            ddlWS.Enabled = False
        End If

        Dim dacheckfields As New SqlDataAdapter("", con)
        If con.State = ConnectionState.Closed Then
            con.Open()
        End If

        dacheckfields.SelectCommand.CommandText = "select distinct(FieldMapping)+'|'+displayName as FieldMapping, displayName FROM [MMM_MST_FIELDS]  where  FieldType='File Uploader' and documenttype='" + ddlEvent.SelectedValue + "' Group By FieldMapping,displayName"
        Dim DtFF = New DataSet
        dacheckfields.Fill(DtFF)
        If (DtFF.Tables(0).Rows.Count > 0) Then

            If ddlAction.SelectedItem.Value = "2" Then
                methodselectid.Visible = True
            Else
                methodselectid.Visible = False
            End If
            methodckblist.DataSource = DtFF
            methodckblist.DataTextField = "displayName"
            methodckblist.DataValueField = "FieldMapping"
            methodckblist.DataBind()
        Else
            methodselectid.Visible = False


        End If
        con.Dispose()
        DA.Dispose()
        DtF.Dispose()
        DtFF.Dispose()

    End Sub
    Protected Sub EditHit(ByVal sender As Object, ByVal e As System.EventArgs)
        Try

            lblMsgEdit.Text = ""
            Dim EN As String = ""
            clearcontrol()
            Dim strfield As String = ""
            Dim btnDetails As ImageButton = TryCast(sender, ImageButton)
            Dim row As GridViewRow = DirectCast(btnDetails.NamingContainer, GridViewRow)
            Dim pid As Integer = Convert.ToString(Me.gvData.DataKeys(row.RowIndex).Value)
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As New SqlConnection(conStr)
            'fill Product  
            Dim da As New SqlDataAdapter("select * FROM MMM_MST_template WHERE TID=" & pid & "", con)
            Dim ds As New DataSet
            da.Fill(ds, "data")

            txtName.Text = ds.Tables("data").Rows(0).Item("TEMPLATE_NAME").ToString()
            ddlAction.SelectedIndex = ddlAction.Items.IndexOf(ddlAction.Items.FindByValue(ds.Tables("data").Rows(0).Item("ACTION")))
            ddlatype.SelectedIndex = ddlatype.Items.IndexOf(ddlatype.Items.FindByText(ds.Tables("data").Rows(0).Item("approvaltype")))
            ddldocnature.SelectedIndex = ddldocnature.Items.IndexOf(ddldocnature.Items.FindByText(ds.Tables("data").Rows(0).Item("docnature")))
            Conditiontxt.Text = Convert.ToString(ds.Tables("data").Rows(0).Item("Condition"))

            If UCase(ddlAction.SelectedItem.Text) = "GPS ALERT" Then
                ddlSBE.Items.Clear()
                ddlSBE.Items.Add("TRIP ALERT")
                ddlSBE.Items.Add("NO SIGNAL ALERT")
                ddlSBE.Items.Insert(0, "SELECT ONE")
                ddlWS.Items.Add("USER")
                ddlWS.Items.Add("SUPERVISOR")
                ddlWS.Items.Insert(0, "SELECT ONE")
                methodselectid.Visible = False
            End If


            If ddlAction.SelectedItem.Text = "SMS ALERT" Then
                ddlEvent.Enabled = False
                ddlSBE.Enabled = False
                ddlWS.Enabled = False
                txtBDays.Enabled = False
                txtAdays.Enabled = False
                methodselectid.Visible = False
            End If

            ddlWS.SelectedIndex = ddlWS.Items.IndexOf(ddlWS.Items.FindByText(ds.Tables("data").Rows(0).Item("StatusName")))
            ddlSBE.SelectedIndex = ddlSBE.Items.IndexOf(ddlSBE.Items.FindByText(ds.Tables("data").Rows(0).Item("SUBEVENT")))
            txtSubject.Text = ds.Tables("data").Rows(0).Item("SUBJECT").ToString()
            txtBcc.Text = ds.Tables("data").Rows(0).Item("BCC").ToString()
            txtBody.Text = HttpUtility.HtmlDecode(ds.Tables("data").Rows(0).Item("MSGBODY").ToString())
            Dim abc As String = ""
            abc &= "{USERNAME}"
            abc &= Environment.NewLine
            abc &= "{IMIENO}"
            abc &= Environment.NewLine
            abc &= "{VEHICLE NO}"
            abc &= Environment.NewLine
            abc &= "{DRIVER NAME}"
            abc &= Environment.NewLine
            abc &= "{GAPPING HOURS}"
            txtvar.Text = abc
            txtAdays.Text = ds.Tables("data").Rows(0).Item("Asladay").ToString()
            txtBDays.Text = ds.Tables("data").Rows(0).Item("Bsladay").ToString()
            txtHH.Text = ds.Tables("data").Rows(0).Item("sHH").ToString()
            txtMM.Text = ds.Tables("data").Rows(0).Item("sMM").ToString()

            If UCase(ddlAction.SelectedItem.Text) <> "GPS ALERT" Then
                EN = ds.Tables("data").Rows(0).Item("Eventname").ToString()
                ddlEvent.SelectedIndex = ddlEvent.Items.IndexOf(ddlEvent.Items.FindByText(EN))

                If con.State <> ConnectionState.Open Then
                    con.Open()
                End If

                da.SelectCommand.CommandText = "select * from MMM_MST_FORMS where eid='" & Session("eid") & "' and formname='" & EN & "'"
                Dim DtF As New DataTable
                da.Fill(DtF)
                Dim isWF As Integer
                If DtF.Rows.Count <> 0 Then
                    isWF = DtF.Rows(0).Item("isworkflow")
                Else
                    isWF = 0
                End If
                If ddlAction.SelectedItem.Text.ToUpper <> "SMS ALERT" Then
                    If isWF = 1 Then
                        BindSubEvent("WORKFLOW EVENT")  ''Bind Sub Event
                        BindMailTo("WORKFLOW EVENT")   ''BIND MAIL FIELDS
                        '' new added for filling workflow status ddl - by sunil 
                        da.SelectCommand.CommandText = "SELECT * FROM MMM_MST_WORKFLOW_STATUS WHERE EID='" & Session("eid") & "' AND DocumentType='" & ddlAction.SelectedItem.Text & "'"
                        Dim dtW As New DataTable
                        da.Fill(dtW)
                        ddlWS.Items.Clear()
                        ddlWS.Items.Add("SELECT STATUS")
                        ddlWS.Items.Add("UPLOADED")

                        For i As Integer = 0 To dtW.Rows.Count - 1
                            ddlWS.Items.Add(dtW.Rows(i).Item("statusName").ToString.ToUpper())
                            ddlWS.Items(i + 2).Value = dtW.Rows(i).Item("statusName").ToString.ToUpper()
                        Next
                        ddlWS.Items.Add("ARCHIVE")
                        If Not IsDBNull(ds.Tables("data").Rows(0).Item("statusName").ToString()) Then
                            If Len(ds.Tables("data").Rows(0).Item("statusName").ToString()) > 0 Then
                                If ds.Tables("data").Rows(0).Item("statusName").ToString() = "REJECTED" Then
                                    ddlWS.Items.Insert(0, "REJECTED")
                                Else
                                    ddlWS.SelectedIndex = ddlWS.Items.IndexOf(ddlWS.Items.FindByText(ds.Tables("data").Rows(0).Item("statusName").ToString()))
                                End If
                            End If
                        End If
                    Else
                        BindSubEvent(EN)    ''Bind Sub Event
                        BindMailTo(EN)    ''BIND MAIL FIELDS

                        '' new added for filling workflow status ddl - by sunil 
                        ddlWS.Items.Clear()
                        ddlWS.Items.Insert(0, "NOT REQUIRED")
                    End If
                Else
                    BindMailTo("SMS ALERT")
                End If

                ''Bind Sub Event

                If ddlAction.SelectedItem.Text = "ALERT" Then
                    ddlSBE.Items.Clear()
                    ddlSBE.Items.Add("SLA EXPIRED")
                    ddlSBE.Items.Add("DOCUMENT EXPIRY")
                    ddlSBE.Items.Insert(0, "SELECT ONE")
                    BSD.Visible = True
                    txtBDays.Visible = True
                    ASD.Visible = True
                    txtAdays.Visible = True
                    Stime.Visible = True
                    txtHH.Visible = True
                    txtMM.Visible = True
                    lblhh.Visible = True
                    lblmm.Visible = True
                    methodselectid.Visible = False
                ElseIf ddlAction.SelectedItem.Text = "MAIL" Then
                    BSD.Visible = False
                    txtBDays.Visible = False
                    ASD.Visible = False
                    txtAdays.Visible = False
                    Stime.Visible = False
                    txtHH.Visible = False
                    txtMM.Visible = False
                    lblhh.Visible = False
                    lblmm.Visible = False
                    ddlatype.Visible = True
                    lblatype.Visible = True

                    '' write here to show sla hh, mm fiels visible code.
                Else
                     methodselectid.Visible = False
                End If
                ddlSBE.SelectedIndex = ddlSBE.Items.IndexOf(ddlSBE.Items.FindByText(ds.Tables("data").Rows(0).Item("SUBEVENT").ToString()))
                If ds.Tables("data").Rows(0).Item("SUBEVENT").ToString() = "DOCUMENT EXPIRY" Then
                    If ddlSBE.SelectedItem.Text = "DOCUMENT EXPIRY" Then
                        da.SelectCommand.CommandType = CommandType.Text
                        da.SelectCommand.CommandText = "select FieldID,displayName,FieldMapping,datatype from MMM_MST_FIELDS where EID=" & Session("EID") & " and DocumentType='" & ddlEvent.SelectedItem.Text() & "' and datatype='datetime'"
                        da.Fill(ds, "fld")
                        ddlWS.Items.Clear()
                        ddlWS.Items.Insert(0, "SELECT FIELD")
                        For i As Integer = 0 To ds.Tables("fld").Rows.Count - 1
                            ddlWS.Items.Add(ds.Tables("fld").Rows(i).Item("displayName"))
                            ddlWS.Items(i + 1).Value = ds.Tables("fld").Rows(i).Item("FieldMapping").ToString.ToUpper
                        Next
                    Else
                        da.SelectCommand.CommandType = CommandType.Text
                        da.SelectCommand.CommandText = "select FieldID,displayName,FieldMapping,datatype from MMM_MST_FIELDS where EID=" & Session("EID") & " and DocumentType='USER'"
                        da.Fill(ds, "fld")
                        ddlufield.Items.Clear()
                        ddlufield.Items.Insert(0, "Select")
                        For i As Integer = 0 To ds.Tables("fld").Rows.Count - 1
                            ddlufield.Items.Add(ds.Tables("fld").Rows(i).Item("displayName"))
                            ddlufield.Items(i + 1).Value = ds.Tables("fld").Rows(i).Item("FieldMapping").ToString.ToUpper
                        Next
                        ddlufield.SelectedIndex = ddlufield.Items.IndexOf(ddlufield.Items.FindByValue(ds.Tables("data").Rows(0).Item("displayName").ToString().ToUpper))
                    End If
                End If

                ddlWS.SelectedIndex = ddlWS.Items.IndexOf(ddlWS.Items.FindByValue(ds.Tables("data").Rows(0).Item("statusname").ToString().ToUpper))
                ''Find System Variable According to Event 
                If isWF = 1 Then
                    da.SelectCommand.CommandText = "select DISPLAYNAME from MMM_MST_FIELDS where (DOCUMENTTYPE='" & EN & "' or documenttype='WORKFLOW EVENT') AND (EID=" & Session("eid") & " OR EID=0) order by displayorder"
                Else
                    da.SelectCommand.CommandText = "select DISPLAYNAME from MMM_MST_FIELDS where DOCUMENTTYPE='" & EN & "' AND (EID=" & Session("eid") & " OR EID=0)"
                End If
                da.Fill(ds, "Field")
                For Each dr As DataRow In ds.Tables("Field").Rows
                    strfield &= "{" & dr.Item(0).ToString() & "}"
                    strfield &= Environment.NewLine
                Next
                Dim fieldmethod As String
                txtvar.Text = strfield
                Dim mailTo() As String
                mailTo = Split(ds.Tables("data").Rows(0).Item("MAILTO").ToString(), ",")
                'DDLCC.SelectedIndex = DDLCC.Items.IndexOf(DDLCC.Items.FindByText(ds.Tables("data").Rows(0).Item("CC").ToString()))
                For i As Integer = 0 To mailTo.Length - 1
                    If ddlMailto.Items.Count > 0 Then
                        If mailTo(i).ToString <> "" Then
                            ddlMailto.Items(ddlMailto.Items.IndexOf(ddlMailto.Items.FindByValue(mailTo(i).ToString))).Selected = True
                        End If
                    End If
                Next
                ddlMailto.SelectedIndex = ddlMailto.Items.IndexOf(ddlMailto.Items.FindByText(ds.Tables("data").Rows(0).Item("MAILTO").ToString()))
                If Not ds Is Nothing Then
                    If ddlAction.Items.FindByValue(ds.Tables("data").Rows(0).Item("ACTION")).Value = "2" Then
                        Dim dacheckfields As New SqlDataAdapter("", con)
                        If con.State = ConnectionState.Closed Then
                            con.Open()
                        End If
                        dacheckfields.SelectCommand.CommandText = "select distinct(FieldMapping)+'|'+displayName as FieldMapping, displayName FROM [MMM_MST_FIELDS]  where  FieldType='File Uploader' and documenttype='" + ds.Tables("data").Rows(0).Item("EVENTNAME").ToString() + "' Group By FieldMapping,displayName"
                        Dim DtFF = New DataSet
                        dacheckfields.Fill(DtFF)
                        If DtFF.Tables(0).Rows.Count > 0 Then
                            If ddlAction.SelectedItem.Value = "2" Then
                                methodselectid.Visible = True
                                methodckblist.DataSource = DtFF
                                methodckblist.DataTextField = "displayName"
                                methodckblist.DataValueField = "FieldMapping"
                                methodckblist.DataBind()
                                Dim FieldName() As String
                                FieldName = Split(ds.Tables("data").Rows(0).Item("AttachmentFields").ToString(), ",")
                                For i As Integer = 0 To FieldName.Length - 1
                                    If Not FieldName(i).ToString = "" Then
                                        methodckblist.Items(methodckblist.Items.IndexOf(methodckblist.Items.FindByValue(FieldName(i).ToString))).Selected = True
                                    End If
                                Next
                            Else
                                methodselectid.Visible = False
                            End If
                        Else
                            methodselectid.Visible = False
                        End If
                    End If
                End If

                If Len(ds.Tables("data").Rows(0).Item("CC").ToString()) > 0 Then
                    Dim mailCC() As String
                    mailCC = Split(ds.Tables("data").Rows(0).Item("CC").ToString(), ",")
                    For i As Integer = 0 To mailCC.Length - 1
                        DDLCC.Items(DDLCC.Items.IndexOf(DDLCC.Items.FindByText(mailCC(i).ToString))).Selected = True
                    Next
                End If
            Else
                ddlEvent.Items.Clear()
                ddlEvent.Enabled = False
                BSD.Visible = True
                txtBDays.Visible = True
                ASD.Visible = True
                txtAdays.Visible = True
                Stime.Visible = True
                txtHH.Visible = True
                txtMM.Visible = True
                lblhh.Visible = True
                lblmm.Visible = True



            End If

            da.Dispose()
            ds.Dispose()
            con.Dispose()
            ' No Value in Session just fill the Edit Form and Show two button
            btnActEdit.Text = "Update"

            'two methods.. either show data from Grid or Show data from Database.
            ViewState("pid") = pid
            Me.updatePanelEdit.Update()
            Me.btnEdit_ModalPopupExtender.Show()
            con.Dispose()
            da.Dispose()
            ds.Dispose()
            If ddlEvent.SelectedItem IsNot Nothing Then

                If ddlEvent.SelectedItem.Text <> "SELECT EVENT" Then

                    Dim conStrs As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
                    Dim cons As New SqlConnection(conStrs)
                    Dim daS As New SqlDataAdapter("", cons)
                    Dim DSS As New DataSet
                    If cons.State = ConnectionState.Closed Then
                        cons.Open()
                    End If
                    daS.SelectCommand.CommandText = "SELECT * FROM MMM_MST_WORKFLOW_STATUS WHERE EID='" & Session("eid") & "' AND DocumentType='" & ddlEvent.SelectedItem.Text & "'"
                    Dim dtWS As New DataTable
                    daS.Fill(dtWS)
                    ddlWS.Items.Clear()
                    ddlWS.Items.Add("ALL")
                    If ddlAction.SelectedItem.Value = 1 Then
                        ddlWS.Items.Add("UPLOADED")
                    End If
                    For i As Integer = 0 To dtWS.Rows.Count - 1
                        ddlWS.Items.Add(dtWS.Rows(i).Item("statusName"))
                        'ddlEvent.Items(i).Value = ds.Tables("type").Rows(i).Item(1)
                    Next
                    ddlWS.Items.Add("ARCHIVE")
                    ddlWS.SelectedIndex = ddlWS.Items.IndexOf(ddlWS.Items.FindByText(ds.Tables("data").Rows(0).Item("StatusName")))
                End If
            End If
        Catch ex As Exception
            Throw
        End Try
    End Sub
    Protected Sub btnSearch_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles btnSearch.Click
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As SqlDataAdapter

        If ddlField.SelectedValue = "TemplateName" Then
            oda = New SqlDataAdapter("select tid,template_name,subject,EventName,subevent,statusName, case action WHEN 1 then 'ALERT' WHEN 2 THEN 'MAIL' END [ACTION] From MMM_MST_TEMPLATE  where template_name like '" & txtValue.Text & "%' and EID='" & Session("eid") & "'", con)
        Else : ddlField.SelectedValue = "EventName"
            oda = New SqlDataAdapter("select tid,template_name,subject,EventName,subevent, statusName, case action WHEN 1 then 'ALERT' WHEN 2 THEN 'MAIL' END [ACTION] From MMM_MST_TEMPLATE  where EventName like '" & txtValue.Text & "%' and EID='" & Session("eid") & "'", con)
        End If

        Dim ds As New DataSet()
        oda.Fill(ds, "data")

        gvData.DataSource = ds.Tables("data")
        gvData.DataBind()
        If gvData.Rows.Count = 0 Then
            lblMsg1.Visible = True
            lblMsg1.Text = "No Record Found"
        Else
            lblMsg1.Visible = False
        End If
        ds.Dispose()
        oda.Dispose()
        con.Close()
    End Sub
    Protected Sub deleteHit(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim btnDetails As ImageButton = TryCast(sender, ImageButton)
        Dim row As GridViewRow = DirectCast(btnDetails.NamingContainer, GridViewRow)
        Dim pid As Integer = Convert.ToString(Me.gvData.DataKeys(row.RowIndex).Value)
        ViewState("PID") = pid

        lblMsgDelete.Text = "Do you want to delete Template : '" & row.Cells(1).Text & "'"

        Me.updatePanelDelete.Update()
        Me.btnDelete_ModalPopupExtender.Show()
    End Sub
    Protected Sub DeleteRecord(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("uspDeleteTemplate ", con)
        oda.SelectCommand.CommandType = CommandType.StoredProcedure
        oda.SelectCommand.Parameters.AddWithValue("pid", ViewState("PID").ToString)

        If con.State <> ConnectionState.Open Then
            con.Open()
        End If

        Dim tempid As Integer = oda.SelectCommand.ExecuteScalar()

        If tempid = 1 Then
            getsearchresult()
            lblMsg1.Text = "Template deleted "
            Me.updPnlGrid.Update()
            Me.btnDelete_ModalPopupExtender.Hide()
        Else
            lblMsg1.Text = "Template not deleted "
            Me.btnDelete_ModalPopupExtender.Hide()
        End If

        con.Close()
        oda.Dispose()
    End Sub
    Public Sub clearcontrol()
        txtName.Text = ""
        txtAdays.Text = ""
        txtBDays.Text = ""
        txtHH.Text = ""
        txtMM.Text = ""
        ddlEvent.SelectedIndex = -1
        ddlSBE.SelectedIndex = -1
        ddlAction.SelectedIndex = -1
        ddlufield.SelectedIndex = -1
        txtSubject.Text = ""
        'DDLCC.SelectedIndex = 0
        txtBody.Text = ""
        txtvar.Text = ""
    End Sub
    Public Sub getsearchresult()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        'fill Product  
        Dim da As SqlDataAdapter = New SqlDataAdapter("", con)
        da.SelectCommand.CommandType = CommandType.StoredProcedure
        da.SelectCommand.CommandText = "usp_GetTemplate"
        da.SelectCommand.Parameters.Clear()
        da.SelectCommand.Parameters.AddWithValue("@eid", Session("eid"))
        Dim ds As New DataSet
        da.Fill(ds, "data")
        gvData.DataSource = ds.Tables("data")
        gvData.DataBind()
    End Sub

    Public Function GetSession() As String
        Dim str As String = ""
        str &= "{" & "Session(UID)" & "}"
        str &= Environment.NewLine
        str &= "{" & "Session(USERNAME)" & "}"
        str &= Environment.NewLine
        str &= "{" & "Session(USERNAME)" & "}"
        str &= Environment.NewLine
        Return str
    End Function
    Protected Sub BindSubEvent(ByVal En As String)
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim da As New SqlDataAdapter("", con)
        Dim DS As New DataSet
        If con.State = ConnectionState.Closed Then
            con.Open()
        End If
        ddlSBE.Items.Clear()

        'added by Pallavi on 13 july
        ddlSBE.Items.Add("CREATED")
        ddlSBE.Items.Add("APPROVE")
        ddlSBE.Items.Add("RECONSIDER")
        ddlSBE.Items.Add("REJECT")
        ddlSBE.Items.Insert(0, "SELECT ONE")
        ' added by pallavi

        'Dim DAs As SqlDataAdapter = New SqlDataAdapter("select FieldID,displayName,FieldMapping,datatype from MMM_MST_FIELDS where EID=" & Session("EID") & " and DocumentType='" & ddlEvent.SelectedItem.Text() & "' and datatype='datetime'", con)
        'Dim dss As New DataSet()
        'DAs.Fill(dss, "DocdateType")
        'If dss.Tables("DocdateType").Rows.Count > 0 Then
        '    ddlWS.DataSource = dss
        '    ddlWS.DataTextField = "displayName"
        '    ddlWS.DataValueField = "fieldMapping"
        '    ddlWS.DataBind()
        '    ddlWS.Items.Insert(0, "Select Date")
        'End If
        'commented by pallavi on 13 July 15 for hardcoding the subevent to (created ,approve ,recosider,reject)
        'da.SelectCommand.CommandText = "Select SubEvent,EVENTID from MMM_NOTIFICATION_EVENT WHERE EVENTNAME='" & En & "'"
        'da.Fill(DS, "SUB")
        'If DS.Tables("SUB").Rows.Count > 0 Then
        '    For J As Integer = 0 To DS.Tables("SUB").Rows.Count - 1
        '        ddlSBE.Items.Add(DS.Tables("SUB").Rows(J).Item(0))
        '        ddlSBE.Items(J).Value = DS.Tables("SUB").Rows(J).Item(1)
        '    Next
        'Else
        '    ddlSBE.Items.Insert(1, "CREATED")
        '    ddlSBE.Items.Insert(2, "UPDATED")
        '    ddlSBE.Items.Insert(3, "DELETED")
        'End If
        con.Dispose()
        da.Dispose()
        DS.Dispose()
    End Sub
    Protected Sub BindMailTo(ByVal en As String)
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim da As New SqlDataAdapter("", con)
        Dim DS As New DataSet
        If con.State = ConnectionState.Closed Then
            con.Open()
        End If
        ddlMailto.Items.Clear()
        DDLCC.Items.Clear()
        da.SelectCommand.CommandText = "SELECT DISPLAYNAME,DISPLAYNAME FROM MMM_MST_FIELDS WHERE DOCUMENTTYPE='" & en & "' AND (FIELDNATURE='MAIL' OR FIELDNATURE='BOTH') AND (EID=" & Session("eid") & " OR EID=0)"
        da.Fill(DS, "MAIL")

        da.SelectCommand.CommandText = "Select distinct rolename  from mmm_ref_role_user where eid=" & Session("EID") & " order by rolename"
        da.Fill(DS, "cc")

        If ddlAction.SelectedItem.Text.ToUpper <> "SMS ALERT" Then
            For i As Integer = 0 To DS.Tables("MAIL").Rows.Count - 1
                ddlMailto.Items.Add("{" & DS.Tables("MAIL").Rows(i).Item(0).ToString() & "}")
                ddlMailto.Items(i).Value = "{" & DS.Tables("MAIL").Rows(i).Item(1).ToString() & "}"
                DDLCC.Items.Add("{" & DS.Tables("MAIL").Rows(i).Item(0) & "}")
                DDLCC.Items(i).Value = "{" & DS.Tables("MAIL").Rows(i).Item(1) & "}"
            Next

            For j As Integer = 0 To DS.Tables("cc").Rows.Count - 1
                DDLCC.Items.Add("{" & DS.Tables("cc").Rows(j).Item(0) & "}")
                'DDLCC.Items(i).Value = "{" & DS.Tables("MAIL").Rows(j).Item(1) & "}"
            Next

        Else
            ddlMailto.Items.Add("User")
            DDLCC.Items.Add("User")
            ddlMailto.Items.Add("Driver")
            DDLCC.Items.Add("Driver")

        End If


        'ddlMailto.Items.Insert(0, "SELECT ONE")
        'DDLCC.Items.Insert(0, "SELECT")
        'ddlMailto.Items.Add("{" & "LOGIN USER" & "}")
        'ddlMailto.Items(ddlMailto.Items.Count - 1).Value = "{" & "LOGIN USER" & "}"
        'DDLCC.Items.Add("{" & "LOGIN USER" & "}")
        'DDLCC.Items(ddlMailto.Items.Count - 1).Value = "{" & "LOGIN USER" & "}"

        con.Dispose()
        da.Dispose()
        DS.Dispose()
    End Sub
    Public Function GetVariable(ByVal en As String) As DataTable
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim da As New SqlDataAdapter("select * from MMM_MST_FORMS where eid='" & Session("eid") & "' and formname='" & en & "'", con)
        Dim DS As New DataSet

        Dim DtF As New DataTable
        da.Fill(DtF)
        Dim isWF As Integer
        If DtF.Rows.Count <> 0 Then
            isWF = DtF.Rows(0).Item("isworkflow")
        Else
            isWF = 0
        End If

        If isWF = 1 Then
            da.SelectCommand.CommandText = "select DISPLAYNAME,FieldMapping,DBTABLENAME FROM MMM_MST_FIELDS where (DOCUMENTTYPE='" & en & "' or documenttype='WORKFLOW EVENT')  AND (EID=" & Session("eid") & " OR EID=0) order by displayorder"
        Else
            da.SelectCommand.CommandText = "select DISPLAYNAME,FieldMapping,DBTABLENAME FROM MMM_MST_FIELDS where DOCUMENTTYPE='" & en & "' AND (EID=" & Session("eid") & " OR EID=0) order by displayorder"
        End If

        da.Fill(DS, "VAR")
        Return DS.Tables("VAR")
        da.Dispose()
        DtF.Dispose()
        DS.Dispose()
        con.Dispose()
    End Function
    Protected Sub ddlSBE_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlSBE.SelectedIndexChanged
        'fill Product  
        Dim var As String = ""
        If ddlSBE.SelectedItem.Text = "DOCUMENT EXPIRY" Then
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As New SqlConnection(conStr)
            Dim DA As SqlDataAdapter = New SqlDataAdapter("select FieldID,displayName,FieldMapping,datatype from MMM_MST_FIELDS where EID=" & Session("EID") & " and DocumentType='" & ddlEvent.SelectedItem.Text() & "' and datatype='datetime'", con)
            Dim ds As New DataSet()
            DA.Fill(ds, "DocdateType")
            If ds.Tables("DocdateType").Rows.Count > 0 Then
                ddlWS.DataSource = ds
                ddlWS.DataTextField = "displayName"
                ddlWS.DataValueField = "fieldMapping"
                ddlWS.DataBind()
                ddlWS.Items.Insert(0, "Select Date")
            End If
            con.Dispose()
            DA.Dispose()
            ds.Dispose()
        ElseIf ddlSBE.SelectedItem.Text = "NO SIGNAL ALERT" Or ddlSBE.SelectedItem.Text = "TRIP ALERT" Then
            var &= "{USERNAME}"
            var &= Environment.NewLine
            var &= "{IMIENO}"
            var &= Environment.NewLine
            var &= "{VEHICLE NO}"
            var &= Environment.NewLine
            var &= "{DRIVER NAME}"
            var &= Environment.NewLine
            var &= "{GAPPING HOURS}"
            var &= Environment.NewLine
            txtvar.Text = var

        ElseIf ddlSBE.SelectedItem.Text = "SLA EXPIRED" Then
            ddlufield.Visible = True
            lblufield.Visible = True
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As New SqlConnection(conStr)
            Dim DA As SqlDataAdapter = New SqlDataAdapter("select FieldID,displayName,FieldMapping,datatype from MMM_MST_FIELDS where EID=" & Session("EID") & " and DocumentType='User' ", con)
            Dim ds As New DataSet()
            DA.Fill(ds, "DocdateType")
            If ds.Tables("DocdateType").Rows.Count > 0 Then
                ddlufield.DataSource = ds
                ddlufield.DataTextField = "displayName"
                ddlufield.DataValueField = "fieldMapping"
                ddlufield.DataBind()
                ddlufield.Items.Insert(0, "Select")
            End If
            con.Dispose()
            DA.Dispose()
            ds.Dispose()
        ElseIf ddlSBE.SelectedItem.Text = "REJECT" Then
            ddlWS.Items.Clear()

            ddlWS.Items.Insert(0, "SELECT")
            ddlWS.Items.Insert(1, "UPLOADED")
            ddlWS.Items.Insert(2, "REJECTED")

        End If


    End Sub
    Protected Sub ddlAction_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlAction.SelectedIndexChanged
        If ddlAction.SelectedItem.Value = 2 Then
            BSD.Visible = False
            txtBDays.Visible = False
            ASD.Visible = False
            txtAdays.Visible = False
            Stime.Visible = False
            txtHH.Visible = False
            txtMM.Visible = False
            lblhh.Visible = False
            lblmm.Visible = False
            ddlEvent.Enabled = True
            ddlMailto.Enabled = True
            ddlWS.Enabled = True
            lblatype.Visible = True
            ddlatype.Visible = True
            ddlMailto.Items.Clear()
            DDLCC.Items.Clear()
            lbldocnature.Visible = True
            ddldocnature.Visible = True
            'If methodckblist.Visible Then
            methodselectid.Visible = True
            'End If

        ElseIf ddlAction.SelectedItem.Value = 4 Then
            ddlEvent.Enabled = False
            ddlSBE.Enabled = False
            ddlWS.Enabled = False
            txtBDays.Enabled = False
            txtAdays.Enabled = False
            ddlMailto.Enabled = True
            DDLCC.Enabled = True
            ddlMailto.Items.Clear()
            DDLCC.Items.Clear()
            ddlMailto.Items.Add("User")
            DDLCC.Items.Add("User")
            ddlMailto.Items.Add("Driver")
            DDLCC.Items.Add("Driver")
            methodselectid.Visible = False

        Else
            BSD.Visible = True
            txtBDays.Visible = True
            ASD.Visible = True
            txtAdays.Visible = True
            Stime.Visible = True
            txtHH.Visible = True
            txtMM.Visible = True
            lblhh.Visible = True
            lblmm.Visible = True
            ddlEvent.Enabled = True
            ddlSBE.Enabled = True
            ddlWS.Enabled = True
            ddlMailto.Items.Clear()
            DDLCC.Items.Clear()
            ddlMailto.Enabled = True
            ddlWS.Enabled = True
            lblatype.Visible = False
            ddlatype.Visible = False
            methodselectid.Visible = False
        End If
        ddlEvent.SelectedIndex = ddlEvent.Items.IndexOf(ddlEvent.Items(0))
        ddlEvent_SelectedIndexChanged(ddlEvent, EventArgs.Empty)

    End Sub

    'Protected Sub chkPublicView_CheckedChanged(sender As Object, e As EventArgs) Handles chkPublicView.CheckedChanged
    '    'If chkPublicView.Checked = True Then
    '    '    lblPvDoc.Visible = True
    '    '    lblCaptiontext.Visible = True
    '    '    lblMode.Visible = True
    '    '    lblRelatioship.Visible = True
    '    '    ddlPvdoctype.Visible = True
    '    '    txtPvCaption.Visible = True
    '    '    ddlPvMode.Visible = True
    '    '    ddlPvRelationsip.Visible = True
    '    '    lblLnkExpiryDate.Visible = True
    '    '    txtLnkexpdate.Visible = True
    '    '    'updatePanelEdit.Update()
    '    '    Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
    '    '    Dim con As New SqlConnection(conStr)
    '    '    Dim DA As SqlDataAdapter = New SqlDataAdapter("select Formid,formname from MMM_MST_Forms where EID=" & Session("EID") & "", con)
    '    '    Dim ds As New DataSet()
    '    '    DA.Fill(ds, "formname")
    '    '    ddlPvdoctype.DataSource = ds.Tables("formname")
    '    '    ddlPvdoctype.DataTextField = "formname"
    '    '    ddlPvdoctype.DataValueField = "formid"
    '    '    ddlPvdoctype.DataBind()

    '    '    updatePanelEdit.Update()
    '    '    con.Dispose()
    '    '    DA.Dispose()
    '    '    con.Close()
    '    'Else
    '    '    lblPvDoc.Visible = False
    '    '    lblCaptiontext.Visible = False
    '    '    lblMode.Visible = False
    '    '    lblRelatioship.Visible = False
    '    '    ddlPvdoctype.Visible = False
    '    '    txtPvCaption.Visible = False
    '    '    ddlPvMode.Visible = False
    '    '    ddlPvRelationsip.Visible = False
    '    '    lblLnkExpiryDate.Visible = False
    '    '    txtLnkexpdate.Visible = False
    '    '    updatePanelEdit.Update()
    '    'End If
    'End Sub
    Protected Sub addExrtafld(ByVal sender As Object, ByVal e As EventArgs)
        Dim btnDetails As ImageButton = TryCast(sender, ImageButton)
        Dim row As GridViewRow = DirectCast(btnDetails.NamingContainer, GridViewRow)
        Dim pid As Integer = Convert.ToString(Me.gvData.DataKeys(row.RowIndex).Value)
        ViewState("TempID") = pid
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim DA As SqlDataAdapter = New SqlDataAdapter("select Formid,formname from MMM_MST_Forms where EID=" & Session("EID") & "", con)
        Dim ds As New DataSet()
        DA.Fill(ds, "formname")
        ddlPvdoctype.DataSource = ds.Tables("formname")
        ddlPvdoctype.DataTextField = "formname"
        ddlPvdoctype.DataValueField = "formid"
        ddlPvdoctype.DataBind()
        ddlPvdoctype.Items.Insert(0, "Please Select")
        ddlPvdoctype.SelectedIndex = ddlPvdoctype.Items.IndexOf(ddlPvdoctype.Items.FindByText(row.Cells(2).Text))
        lblTemheadr.Text = row.Cells(2).Text
        updExtraField.Update()
        MP_ExtraField.Show()
        con.Close()
        ds.Dispose()

    End Sub
    Protected Sub AddExtTemplate(ByVal sender As Object, ByVal e As EventArgs)
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim ODA As SqlDataAdapter = New SqlDataAdapter("", con)
        Try
            ODA.SelectCommand.CommandText = "USPinsertExtrafield"
            ODA.SelectCommand.CommandType = CommandType.StoredProcedure
            ODA.SelectCommand.Parameters.AddWithValue("pid", ViewState("TempID"))
            ODA.SelectCommand.Parameters.AddWithValue("pvDoctype", ddlPvdoctype.SelectedItem.Text)
            ODA.SelectCommand.Parameters.AddWithValue("pvLinkCaption", txtPvCaption.Text)
            ODA.SelectCommand.Parameters.AddWithValue("pvMode", ddlPvMode.SelectedItem.Text)
            ODA.SelectCommand.Parameters.AddWithValue("pvRelationship", ddlPvrealtioship.SelectedItem.Text)
            ODA.SelectCommand.Parameters.AddWithValue("linkExpirytype", ddlLnkExpDate.SelectedItem.Text)
            ODA.SelectCommand.Parameters.AddWithValue("linkexpiryDate", ddlExPvDocField.SelectedItem.Value)
            ODA.SelectCommand.Parameters.AddWithValue("controlname", ddlControlName.SelectedItem.Text)
            ODA.SelectCommand.Parameters.AddWithValue("eid", Session("EID"))
            'ODA.SelectCommand.Parameters.AddWithValue("")  for ID value
            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            ODA.SelectCommand.ExecuteNonQuery()
            lblMsg1.Text = "Extra field of " & ddlPvdoctype.SelectedItem.Text & " has been added succsessfully"
            updPnlGrid.Update()
            MP_ExtraField.Hide()
        Catch ex As Exception
            lblmessExtraField.Text = "Extra field of " & ddlPvdoctype.SelectedItem.Text & " has not  been Added/Updated"
            updExtraField.Update()
            MP_ExtraField.Show()
        Finally
            con.Close()
            ODA.Dispose()
        End Try
    End Sub
    Protected Sub ddlLnkExpDate_SelectedIndexChanged(sender As Object, e As System.EventArgs) Handles ddlLnkExpDate.SelectedIndexChanged
        If ddlLnkExpDate.SelectedItem.Value = "1" Then ' THIS IS FOR STATIC  LINK EXPIRY 
            lblHr.Visible = True
            txtExpLinkHH.Visible = True
            ddlExPvDocField.Visible = False
            lblEXDay.Visible = False
            txtExpDays.Visible = False
            'lblExAftrDate.Visible = True
            'lblExAftrDate.Visible = True

        ElseIf ddlLnkExpDate.SelectedItem.Value = "2" Then '' THIS IS FOR dynamic  LINK EXPIRY
            lblfieldtype.Visible = True
            txtExpLinkHH.Visible = False
            ddlExPvDocField.Visible = True
            lblEXDay.Visible = True
            txtExpDays.Visible = True
            'lblExAftrDate.Visible = True
            'lblExAftrDate.Visible = True

            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As New SqlConnection(conStr)
            Dim ODA As SqlDataAdapter = New SqlDataAdapter("", con)
            ODA.SelectCommand.CommandText = "select * from MMM_MST_Fields where eid=" & Session("EID") & " and documenttype='" & ddlPvdoctype.SelectedItem.Text & "'  AND dataTYPE='DATETIME' "
            ODA.SelectCommand.CommandType = CommandType.Text
            Dim DS As New DataSet
            ODA.Fill(DS, "dateField")
            ddlExPvDocField.DataSource = DS.Tables("dateField")
            ddlExPvDocField.DataTextField = "displayname"
            ddlExPvDocField.DataValueField = "fieldid"
            ddlExPvDocField.DataBind()
            ddlExPvDocField.Items.Insert(0, "PLEASE SELECT")
            con.Close()
            con.Dispose()
            ODA.Dispose()

        End If

    End Sub

    'Protected Sub ddlPvdoctype_SelectedIndexChanged(sender As Object, e As System.EventArgs) Handles ddlPvdoctype.SelectedIndexChanged
    '    Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
    '    Dim con As New SqlConnection(conStr)
    '    Dim ODA As SqlDataAdapter = New SqlDataAdapter("", con)
    '    ODA.SelectCommand.CommandText = "select * from MMM_MST_Template_extrafields where documenttype='" & ddlPvdoctype.SelectedItem.Text & "' and eid=" & Session("EID") & " "
    '    ODA.SelectCommand.CommandType = CommandType.Text
    '    Dim ds As New DataSet
    '    ODA.Fill(ds, "extFld")
    '    txtPvCaption.Text = ds.Tables("extFld").Rows
    'End Sub
    Protected Sub gvData_RowDataBound(sender As Object, e As GridViewRowEventArgs) Handles gvData.RowDataBound
        For i As Integer = 0 To e.Row.Cells.Count - 1
            ViewState("OrigData") = e.Row.Cells(i).Text
            If e.Row.Cells(i).Text.Length >= 31 Then
                e.Row.Cells(i).Text = e.Row.Cells(i).Text.Substring(0, 31) + "..."
                e.Row.Cells(i).ToolTip = ViewState("OrigData").ToString()
                e.Row.Cells(i).Wrap = True
            End If
        Next
    End Sub
End Class
