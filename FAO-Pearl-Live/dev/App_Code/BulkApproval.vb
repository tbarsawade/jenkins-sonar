Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Data.SqlClient
Imports System.Net
Imports System.IO
Imports System.Xml.Serialization
Imports System.Runtime.Serialization
Imports System.ServiceModel
Imports System.ServiceModel.Web
Imports System.Xml

Public Class BulkApprovalBAL

    Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString


    Public Function GetBulkApprovalDocType(EID As Integer, UID As Integer) As DataSet

        Dim ds As New DataSet()
        Dim con As SqlConnection = Nothing
        Dim da As SqlDataAdapter = Nothing

        Try
            con = New SqlConnection(conStr)
            da = New SqlDataAdapter("GetBulkApprovalDocType", con)
            da.SelectCommand.CommandType = CommandType.StoredProcedure
            da.SelectCommand.Parameters.AddWithValue("@EID", EID)
            da.SelectCommand.Parameters.AddWithValue("@UID", UID)
            da.Fill(ds)
            Return ds
        Catch ex As Exception
            Throw New Exception()
        Finally
            If Not con Is Nothing Then
                con.Close()
                con.Dispose()
            End If
            If Not da Is Nothing Then
                da.Dispose()
            End If
        End Try
        Return ds
    End Function

    Public Function GetBulkApprovalWF(EID As Integer, DocumentType As String, UID As Integer) As DataSet

        Dim ds As New DataSet()
        Dim con As SqlConnection = Nothing
        Dim da As SqlDataAdapter = Nothing

        Try
            con = New SqlConnection(conStr)
            da = New SqlDataAdapter("GetBulkApprovalWF1", con)
            da.SelectCommand.CommandType = CommandType.StoredProcedure
            da.SelectCommand.Parameters.AddWithValue("@EID", EID)
            da.SelectCommand.Parameters.AddWithValue("@DocumentType", DocumentType)
            da.SelectCommand.Parameters.AddWithValue("@UID", UID)
            da.Fill(ds)
            Return ds
        Catch ex As Exception
            Throw New Exception()
        Finally
            If Not con Is Nothing Then
                con.Close()
                con.Dispose()
            End If
            If Not da Is Nothing Then
                da.Dispose()
            End If
        End Try
        Return ds
    End Function
    Public Function GetFormField(EID As Integer, DocumentType As String, ActionFormName As String) As DataSet

        Dim ds As New DataSet()
        Dim con As SqlConnection = Nothing
        Dim da As SqlDataAdapter = Nothing

        Try
            con = New SqlConnection(conStr)
            Dim Query = "select DisplayName,FieldMapping,ISActive from MMM_MST_FIELDS WHERE EID=" & EID & " AND DocumentType='" & DocumentType & "' AND FieldType <>'Child Item' AND invisible=0"
            'If Not String.IsNullOrEmpty(ActionFormName) Then
            '    Query = Query & " union select DisplayName,FieldMapping from MMM_MST_FIELDS WHERE EID=" & EID & " AND DocumentType='" & ActionFormName & "'"
            'End If
            Query = Query & " order by displayname"
            da = New SqlDataAdapter(Query, con)
            da.Fill(ds)
            Return ds
        Catch ex As Exception
            Throw New Exception()
        Finally
            If Not con Is Nothing Then
                con.Close()
                con.Dispose()
            End If
            If Not da Is Nothing Then
                da.Dispose()
            End If
        End Try
        Return ds
    End Function
    Public Function GetActionFormNameBA(EID As Integer, DocumentType As String, WFStatus As String) As DataSet

        Dim ds As New DataSet()
        Dim con As SqlConnection = New SqlConnection(conStr)
        con.Open()
        Dim da As SqlDataAdapter = Nothing

        Try
            con = New SqlConnection(conStr)
            da = New SqlDataAdapter("GetActionFormNameBA", con)
            da.SelectCommand.CommandType = CommandType.StoredProcedure
            da.SelectCommand.Parameters.AddWithValue("@EID", EID)
            da.SelectCommand.Parameters.AddWithValue("@DocumentType", DocumentType)
            da.SelectCommand.Parameters.AddWithValue("@WFStatus", WFStatus)
            da.Fill(ds)
            Return ds

        Catch ex As Exception
            Throw New Exception()

        Finally
            If Not con Is Nothing Then
                con.Close()
                con.Dispose()
            End If
            If Not da Is Nothing Then
                da.Dispose()
            End If
        End Try

        Return ds
    End Function
    Public Sub CreateControlsOnPanel(ByVal dsFields As DataTable, ByRef pnlFields As Panel, ByRef UpdatePanel1 As UpdatePanel, ByRef btnActEdit As Button)  ' Optional ByRef ddown As DropDownList = Nothing
        pnlFields.Controls.Clear()
        Dim oda As SqlDataAdapter = Nothing
        Dim oda1 As SqlDataAdapter = Nothing
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = Nothing
        Dim ob As New DynamicForm()
        Dim lblWidth As Integer = 130
        Dim controlWdth As Integer = 240
        Try
            Dim onlyFiltered As DataView = dsFields.DefaultView
            onlyFiltered.RowFilter = "Invisible=0 and FieldType<>'Check Box'"
            Dim ds As DataTable = onlyFiltered.Table.DefaultView.ToTable()
            pnlFields.Controls.Add(New LiteralControl("<div class=""form""><table width=""100%"" cellspacing=""5px"" border=""0px"" cellpadding=""0px"">"))
            Dim datatype As String = ""
            For i As Integer = 0 To ds.Rows.Count - 1
                Dim dispName As String = ds.Rows(i).Item("displayname").ToString()
                Dim lbl As New Label
                lbl.ID = "lbl" & ds.Rows(i).Item("FieldID").ToString()
                If ds.Rows(i).Item("isrequired").ToString() = "1" Then
                    dispName &= "*"
                End If
                lbl.Text = dispName
                lbl.Font.Bold = True
                If i Mod 2 = 0 Then
                    pnlFields.Controls.Add(New LiteralControl("<tr>"))
                End If
                pnlFields.Controls.Add(New LiteralControl("<td style=""width:" & lblWidth & "px;text-align:left"">"))
                'don't add label for child grid
                pnlFields.Controls.Add(lbl)
                pnlFields.Controls.Add(New LiteralControl("</td><td style=""width:" & controlWdth & "px;text-align:left"">"))
                Select Case ds.Rows(i).Item("FieldType").ToString().ToUpper()
                    Case "LOOKUP"
                        datatype = ds.Rows(i).Item("datatype").ToString().ToUpper()
                        Dim txtBox As New TextBox
                        txtBox.ID = "fld" & ds.Rows(i).Item("FieldID").ToString()
                        txtBox.Width = controlWdth - 10
                        txtBox.CssClass = "txtBox"
                        If ds.Rows(i).Item("isEditable").ToString() = "1" Then
                            txtBox.Enabled = True
                        Else
                            txtBox.Enabled = False
                        End If
                        pnlFields.Controls.Add(txtBox)
                    Case "CHILD ITEM TOTAL"
                        datatype = ds.Rows(i).Item("datatype").ToString().ToUpper()
                        Dim txtBox As New TextBox
                        txtBox.ID = "fld" & ds.Rows(i).Item("FieldID").ToString()
                        txtBox.Width = controlWdth - 10
                        txtBox.CssClass = "txtBox"
                        txtBox.Enabled = False
                        pnlFields.Controls.Add(txtBox)
                    Case "CHILD ITEM MAX"  ' by sunil 04_jul_14
                        datatype = ds.Rows(i).Item("datatype").ToString().ToUpper()
                        Dim txtBox As New TextBox
                        txtBox.ID = "fld" & ds.Rows(i).Item("FieldID").ToString()
                        txtBox.Width = controlWdth - 10
                        txtBox.CssClass = "txtBox"
                        txtBox.Enabled = False
                        pnlFields.Controls.Add(txtBox)
                    Case "CALCULATIVE FIELD"
                        datatype = ds.Rows(i).Item("datatype").ToString().ToUpper()
                        Dim txtBox As New TextBox
                        txtBox.ID = "fld" & ds.Rows(i).Item("FieldID").ToString()
                        txtBox.Width = controlWdth - 10
                        txtBox.CssClass = "txtBox"
                        If ds.Rows(i).Item("defaultfieldval").ToString.Length > 0 Then
                            txtBox.Text = ds.Rows(i).Item("defaultfieldval")
                        Else
                            If datatype = "NUMERIC" Then
                                txtBox.Text = "0"
                            End If
                        End If
                        pnlFields.Controls.Add(txtBox)
                    Case "TEXT BOX"
                        datatype = ds.Rows(i).Item("datatype").ToString().ToUpper()
                        Dim txtBox As New TextBox
                        txtBox.ID = "fld" & ds.Rows(i).Item("FieldID").ToString()
                        txtBox.Width = controlWdth - 10
                        txtBox.CssClass = "txtBox"
                        If ds.Rows(i).Item("defaultfieldval").ToString.Length > 0 Then
                            txtBox.Text = ds.Rows(i).Item("defaultfieldval").ToString
                        Else
                            If datatype = "NUMERIC" Then
                                txtBox.Text = "0"
                            End If
                        End If
                        If Val(ds.Rows(i).Item("isDescription").ToString()) = 1 Then
                            txtBox.ToolTip = ds.Rows(i).Item("Description").ToString()
                        End If
                        pnlFields.Controls.Add(txtBox)
                        If datatype = "DATETIME" Then
                            Dim CLNDR As New AjaxControlToolkit.CalendarExtender
                            CLNDR.Controls.Clear()
                            CLNDR.ID = "CLNDR" & ds.Rows(i).Item("FieldID").ToString()
                            CLNDR.Format = "dd/MM/yy"
                            CLNDR.TargetControlID = txtBox.ID
                            txtBox.Enabled = True
                            txtBox.Text = String.Format("{0:dd/MM/yy}", Date.Now())
                            If HttpContext.Current.Session("EDITonEDIT") Is Nothing Then ' this session is inittialized on doc detail page by balli  in order to check this is coming from edit option  or not 
                                If ds.Rows(i).Item("iseditable") = 1 Then
                                    Dim img As New Image
                                    img.ID = "img" & ds.Rows(i).Item("FieldID").ToString()
                                    img.ImageUrl = "~\images\Cal.png"
                                    txtBox.Width = controlWdth - 30
                                    pnlFields.Controls.Add(img)
                                    CLNDR.PopupButtonID = "img" & ds.Rows(i).Item("FieldID").ToString()
                                    pnlFields.Controls.Add(CLNDR)
                                Else
                                    txtBox.Enabled = False
                                End If
                            Else
                                If HttpContext.Current.Session("EDITonEDIT") = "EDITonEDIT" Then  ' this session is inittialized on doc detail page by balli 
                                    If ds.Rows(i).Item("alloweditonedit") = 1 And ds.Rows(i).Item("iseditable") = 1 Then
                                        Dim img As New Image
                                        img.ID = "img" & ds.Rows(i).Item("FieldID").ToString()
                                        img.ImageUrl = "~\images\Cal.png"
                                        txtBox.Width = controlWdth - 30
                                        pnlFields.Controls.Add(img)
                                        CLNDR.PopupButtonID = "img" & ds.Rows(i).Item("FieldID").ToString()
                                        pnlFields.Controls.Add(CLNDR)
                                    Else
                                        txtBox.Enabled = False
                                    End If
                                End If
                            End If
                        ElseIf datatype = "NEW DATETIME" Then
                            'Data -field = "datetime"
                            txtBox.Enabled = True
                            'txtBox.ReadOnly = True
                            txtBox.ID = "fld" & ds.Rows(i).Item("FieldID").ToString()
                            txtBox.Attributes.Add("data-field", "datetime")
                            txtBox.Attributes.Add("readonly", "readonly")
                            'readonly
                            txtBox.Width = controlWdth - 10
                            pnlFields.Controls.Add(txtBox)
                        ElseIf datatype = "TIME" Then
                            'Data -field = "datetime"
                            txtBox.Enabled = True
                            'txtBox.ReadOnly = True
                            txtBox.ID = "fld" & ds.Rows(i).Item("FieldID").ToString()
                            txtBox.Attributes.Add("data-field", "time")
                            txtBox.Attributes.Add("readonly", "readonly")
                            'readonly
                            txtBox.Width = controlWdth - 10
                            pnlFields.Controls.Add(txtBox)
                        End If
                    Case "AUTO NUMBER"
                        datatype = ds.Rows(i).Item("datatype").ToString().ToUpper()
                        Dim txtBox As New TextBox
                        txtBox.ID = "fld" & ds.Rows(i).Item("FieldID").ToString()
                        txtBox.Width = controlWdth - 10
                        txtBox.CssClass = "txtBox"
                        txtBox.Text = ds.Rows(i).Item("dropdown").ToString() & ds.Rows(i).Item("MaxLen").ToString()
                        txtBox.Enabled = False
                        pnlFields.Controls.Add(txtBox)
                    Case "FORMULA FIELD"
                        datatype = ds.Rows(i).Item("datatype").ToString().ToUpper()
                        Dim txtBox As New TextBox
                        txtBox.ID = "fld" & ds.Rows(i).Item("FieldID").ToString()
                        txtBox.Width = controlWdth - 10
                        txtBox.CssClass = "txtBox"
                        pnlFields.Controls.Add(txtBox)
                        txtBox.Enabled = False
                    Case "GEO POINT"
                        datatype = ds.Rows(i).Item("datatype").ToString().ToUpper()
                        Dim txtBox As New TextBox
                        txtBox.ID = "fld" & ds.Rows(i).Item("FieldID").ToString()
                        txtBox.Width = controlWdth - 10
                        txtBox.CssClass = "txtBox"
                        txtBox.Enabled = True
                        pnlFields.Controls.Add(txtBox)
                    Case "GEO FENCE"
                        datatype = ds.Rows(i).Item("datatype").ToString().ToUpper()
                        Dim txtBox As New TextBox
                        txtBox.ID = "fld" & ds.Rows(i).Item("FieldID").ToString()
                        txtBox.Width = controlWdth - 10
                        txtBox.CssClass = "txtBox"
                        txtBox.Enabled = True
                        pnlFields.Controls.Add(txtBox)
                    Case "AUTOCOMPLETE" 'AutoComplete
                        '' by balli for autoCompletefilter
                        datatype = ds.Rows(i).Item("datatype").ToString().ToUpper()
                        Dim txtbox As New TextBox
                        txtbox.ID = "fld" & ds.Rows(i).Item("FieldID").ToString()
                        txtbox.Width = controlWdth - 10
                        txtbox.CssClass = "txtBox"
                        txtbox.Attributes.Add("placeholder", "Please begin typing a " & ds.Rows(i).Item("displayname"))

                        txtbox.AutoPostBack = True
                        'AddHandler txtbox.TextChanged, AddressOf ob.txtbox_TextChanged
                        ' adding handler for binding lookup or other dropdown or filter

                        Dim autofilExtender As New AjaxControlToolkit.AutoCompleteExtender
                        autofilExtender.ID = "extenderID" & ds.Rows(i).Item("FieldID").ToString()

                        autofilExtender.TargetControlID = "fld" & ds.Rows(i).Item("FieldID").ToString()
                        autofilExtender.ServiceMethod = "GetCompletionList" ' THIS SERVICE METHOD IS WRITTEN ON DOCUMENT PAGE 
                        autofilExtender.DelimiterCharacters = ""
                        autofilExtender.ContextKey = ds.Rows(i).Item("dropdown").ToString() & "-" & ds.Rows(i).Item("FieldID").ToString() & "-" & ds.Rows(i).Item("dropdowntype").ToString() & "-" & ds.Rows(i).Item("autofilter").ToString() & "-" & ds.Rows(i).Item("InitialFilter").ToString()
                        'context key contain  : dropdown-fieldid-dropdowntype-autofilter-InitialFilter' adding the fieldid in context key because later on we need id of field in web method 
                        autofilExtender.Enabled = True
                        autofilExtender.CompletionSetCount = 5
                        autofilExtender.OnClientShown = "onDataShown"  ' by sunil
                        '   autofilExtender.CompletionListElementID = "ACsugestlist"
                        autofilExtender.MinimumPrefixLength = 1  ' start searching when entered one characters
                        'autofilExtender.OnClientItemSelected = "ace_itemSelected('" & ds.Rows(i).Item("FieldID").ToString() & "')"
                        autofilExtender.OnClientItemSelected = "ace_itemSelected"
                        pnlFields.Controls.Add(txtbox)

                        Dim hdnfld As New HiddenField
                        hdnfld.ID = "HDN" & ds.Rows(i).Item("FieldID").ToString()
                        pnlFields.Controls.Add(hdnfld)
                        'pnlFields.Controls.Add(img)
                        'pnlFields.Controls.Add(prgBar)
                        pnlFields.Controls.Add(autofilExtender)
                    Case "LOOKUPDDL"
                        datatype = ds.Rows(i).Item("datatype").ToString().ToUpper()
                        Dim fieldtypee As String = ds.Rows(i).Item("FieldType").ToString().ToUpper()
                        Dim ddl As New DropDownList
                        ddl.ID = "fld" & ds.Rows(i).Item("FieldID").ToString()
                        ddl.Width = controlWdth - 10
                        ddl.CssClass = "txtBox"
                        Dim ddlText As String = ds.Rows(i).Item("dropdown").ToString()
                        Dim dropdowntype As String = ds.Rows(i).Item("dropdowntype").ToString()
                        Dim od As SqlDataAdapter = New SqlDataAdapter("select dropdown,DDllookupvalue from MMM_MST_FIELDS where FieldID='" & ddlText & "'", con)
                        ' Dim dsMonth As New DataSet
                        Dim dt As New DataTable
                        od.Fill(dt)
                        Dim arr() As String
                        Dim darr() As String
                        Dim type As String = dt.Rows(0).Item("dropdown").ToString()
                        arr = type.Split("-")
                        Dim documenttype As String = arr(1).ToString()
                        Dim field As String = dt.Rows(0).Item("DDllookupvalue").ToString()
                        field = field.Substring(0, field.Length - 1)
                        darr = field.Split(",")
                        For k As Integer = 0 To darr.Count - 1
                            Dim fieldmping As String
                            Dim str As String = ""
                            Dim feild As String
                            Dim s = darr(k).Split("-")
                            feild = s(0).ToString()
                            fieldmping = s(1).ToString()

                            Dim TID As String = "TID"
                            Dim TABLENAME As String = ""
                            Dim str1 As String = ""
                            Dim lookUpqry As String = ""

                            Dim xwhr As String = ""
                            Dim tids As String = ""
                            Dim Rtids As String = ""   ' for prerole data filter
                            If feild = ds.Rows(i).Item("FieldID").ToString() Then
                                od.SelectCommand.CommandText = "select dropdown from MMM_MST_FIELDS where EID=" & HttpContext.Current.Session("EID") & " and documenttype='" & documenttype & "' and FieldMapping in('" & fieldmping & "')"
                                Dim dt1 As New DataTable
                                od.Fill(dt1)
                                Dim dropdown As String = dt1.Rows(0).Item("dropdown").ToString()
                                arr = dropdown.Split("-")

                                If UCase(arr(0).ToString()) = "MASTER" Then
                                    TABLENAME = "MMM_MST_MASTER"
                                ElseIf UCase(arr(0).ToString()) = "DOCUMENT" Then
                                    TABLENAME = "MMM_MST_DOC"
                                ElseIf UCase(arr(0).ToString()) = "CHILD" Then
                                    TABLENAME = "MMM_MST_DOC_ITEM"
                                ElseIf UCase(arr(0).ToString) = "STATIC" Then
                                    If arr(1).ToString.ToUpper = "USER" Then
                                        TABLENAME = "MMM_MST_USER"
                                        TID = "UID"
                                    ElseIf arr(1).ToString().ToUpper = "LOCATION" Then
                                        TABLENAME = "MMM_MST_LOCATION"
                                        TID = "LOCID"
                                    End If
                                End If

                                If arr(0).ToUpper() = "CHILD" Then
                                    str1 = "select " & arr(2).ToString() & "," & TID & "[tid]  from " & TABLENAME & " M WHERE   DOCUMENTTYPE='" & arr(1).ToString() & "'"
                                ElseIf arr(0).ToUpper() <> "STATIC" Then
                                    str1 = "select " & arr(2).ToString() & "," & TID & "[tid]" & lookUpqry & " from " & TABLENAME & " M WHERE EID=" & HttpContext.Current.Session("EID") & " AND  DOCUMENTTYPE='" & arr(1).ToString() & "'"
                                Else
                                    If arr(2).ToString.ToUpper = "LOCATIONSTATE" Then
                                        str1 = "select DISTINCT " & arr(2).ToString() & ",SID [tid]" & lookUpqry & " from " & TABLENAME & " M "
                                    Else
                                        str1 = "select " & arr(2).ToString() & "," & TID & "[tid]" & lookUpqry & " from " & TABLENAME & " M WHERE EID=" & HttpContext.Current.Session("EID") & " "
                                    End If
                                End If
                                '  End If

                                'Dim tidarr() As String

                                ''FILTER THE DATA ACCORDING TO USER 
                                tids = ob.UserDataFilter(ds.Rows(i).Item("documenttype").ToString(), arr(1).ToString())
                                Rtids = ob.UserDataFilter_PreRole(arr(1).ToString(), TABLENAME)  '' new by sunil for pre role data filter 22-feb

                                If tids.Length >= 2 Then
                                    xwhr = " AND CONVERT(NVARCHAR(10),TID) IN (" & tids & ")"
                                ElseIf tids = "0" Then
                                    pnlFields.Visible = False
                                    btnActEdit.Visible = False
                                    UpdatePanel1.Update()
                                    xwhr = ""
                                End If
                                '' new by sunil for pre role data filter 22-feb
                                If Rtids <> "" Then
                                    If xwhr.ToString = "" Then
                                        xwhr = " AND CONVERT(NVARCHAR(10),TID) IN (" & Rtids & ")"
                                    Else
                                        xwhr = " AND CONVERT(NVARCHAR(10),TID) IN (" & tids & "," & Rtids & ")"
                                    End If
                                End If
                                str1 = str1 & "  AND M.isauth=1 " & xwhr & " order by " & arr(2).ToString()
                                Dim dt2 As New DataTable
                                If str1.Length > 0 Then
                                    pnlFields.Controls.Add(ddl)
                                    od.SelectCommand.CommandText = str1
                                    od.Fill(dt2)
                                    Dim isAddJquery As Integer = 0
                                    ddl.Items.Add("Select")
                                    ddl.Items(0).Value = "0"
                                    For J As Integer = 0 To dt2.Rows.Count - 1
                                        ddl.Items.Add(dt2.Rows(J).Item(0).ToString())
                                        Dim lookddlVal As String = dt2.Rows(J).Item(1).ToString()
                                        ddl.Items(J + 1).Value = lookddlVal
                                    Next
                                    dt2.Dispose()
                                End If
                                If ds.Rows(i).Item("isEditable").ToString() = "1" Then
                                    ddl.Enabled = True
                                Else
                                    ddl.Enabled = False
                                End If
                            End If
                        Next
                    Case "DROP DOWN"
                        Dim ddl As New DropDownList
                        ddl.ID = "fld" & ds.Rows(i).Item("FieldID").ToString()
                        ddl.Width = controlWdth - 2
                        ddl.CssClass = "txtBox"
                        Dim ddlText As String = ds.Rows(i).Item("dropdown").ToString()
                        Dim dropdowntype As String = ds.Rows(i).Item("dropdowntype").ToString()
                        Dim arr() As String
                        Dim arrMid() As String
                        If UCase(dropdowntype) = "FIX VALUED" Then
                            arr = ddlText.Split(",")
                            ddl.Items.Add("SELECT")
                            For ii As Integer = 0 To arr.Count - 1
                                ddl.Items.Add(arr(ii).ToUpper().Trim())
                            Next
                        ElseIf UCase(dropdowntype) = "MASTER VALUED" Then
                            arr = ddlText.Split("-")
                            Dim TID As String = "TID"
                            Dim TABLENAME As String = ""
                            If UCase(arr(0).ToString()) = "MASTER" Then
                                TABLENAME = "MMM_MST_MASTER"
                            ElseIf UCase(arr(0).ToString()) = "DOCUMENT" Then
                                TABLENAME = "MMM_MST_DOC"
                            ElseIf UCase(arr(0).ToString()) = "CHILD" Then
                                TABLENAME = "MMM_MST_DOC_ITEM"
                            ElseIf UCase(arr(0).ToString) = "STATIC" Then
                                If arr(1).ToString.ToUpper = "USER" Then
                                    TABLENAME = "MMM_MST_USER"
                                    TID = "UID"
                                ElseIf arr(1).ToString().ToUpper = "LOCATION" Then
                                    TABLENAME = "MMM_MST_LOCATION"
                                    TID = "LOCID"
                                End If
                            End If
                            Dim lookUpqry As String = ""
                            Dim str As String = ""
                            If arr(0).ToUpper() = "CHILD" Then
                                str = "select " & arr(2).ToString() & "," & TID & "[tid]  from " & TABLENAME & " M WHERE   DOCUMENTTYPE='" & arr(1).ToString() & "'"
                            ElseIf arr(0).ToUpper() <> "STATIC" Then
                                str = "select " & arr(2).ToString() & "," & TID & "[tid]" & lookUpqry & " from " & TABLENAME & " M WHERE EID=" & HttpContext.Current.Session("EID") & " AND  DOCUMENTTYPE='" & arr(1).ToString() & "'"
                            Else
                                If arr(2).ToString.ToUpper = "LOCATIONSTATE" Then
                                    str = "select DISTINCT " & arr(2).ToString() & ",SID [tid]" & lookUpqry & " from " & TABLENAME & " M "
                                Else
                                    str = "select " & arr(2).ToString() & "," & TID & "[tid]" & lookUpqry & " from " & TABLENAME & " M WHERE EID=" & HttpContext.Current.Session("EID") & " "
                                End If
                            End If

                            Dim xwhr As String = ""
                            Dim tids As String = ""
                            Dim Rtids As String = ""   ' for prerole data filter
                            'Dim tidarr() As String

                            ''FILTER THE DATA ACCORDING TO USER 
                            tids = ob.UserDataFilter(ds.Rows(i).Item("documenttype").ToString(), arr(1).ToString())
                            Rtids = ob.UserDataFilter_PreRole(arr(1).ToString(), TABLENAME)  '' new by sunil for pre role data filter 22-feb

                            If tids.Length >= 2 Then
                                xwhr = " AND CONVERT(NVARCHAR(10),TID) IN (" & tids & ")"
                            ElseIf tids = "0" Then
                                pnlFields.Visible = False
                                btnActEdit.Visible = False
                                UpdatePanel1.Update()
                                xwhr = ""
                            End If
                            '' new by sunil for pre role data filter 22-feb
                            If Rtids <> "" Then
                                If xwhr.ToString = "" Then
                                    xwhr = " AND CONVERT(NVARCHAR(10),TID) IN (" & Rtids & ")"
                                Else
                                    xwhr = " AND CONVERT(NVARCHAR(10),TID) IN (" & tids & "," & Rtids & ")"
                                End If
                            End If


                            str = str & "  AND M.isauth=1 " & xwhr & " order by " & arr(2).ToString()

                            Dim AutoFilter As String = ds.Rows(i).Item("AutoFilter").ToString()
                            Dim InitFilterArr As String() = ds.Rows(i).Item("InitialFilter").ToString().Split(":")
                            If AutoFilter.Length > 0 Then
                                If arr(0).ToUpper() = "CHILD" Then
                                    If AutoFilter.ToUpper = "DOCID" Then
                                        str = ob.GetQuery1(arr(1).ToString, arr(2).ToString())
                                    Else
                                        str = ob.GetQuery(arr(1).ToString, arr(2).ToString)
                                    End If
                                ElseIf arr(0).ToUpper() <> "STATIC" Then
                                    str = "select " & arr(2).ToString() & ",convert(nvarchar(10),tid)  [tid]" & lookUpqry & " from " & TABLENAME & " M WHERE EID=" & HttpContext.Current.Session("EID") & " AND  DOCUMENTTYPE='" & arr(1).ToString() & "'"
                                    str = str & "  AND M.isauth=1 " & xwhr & " order by " & arr(2).ToString()
                                Else
                                    str = "select " & arr(2).ToString() & ",convert(nvarchar(10)," & TID & ")  [tid]" & lookUpqry & " from " & TABLENAME & " M WHERE EID=" & HttpContext.Current.Session("EID") & " "
                                    str = str & "  AND M.isauth=1 " & xwhr & " order by " & arr(2).ToString()
                                End If
                            ElseIf InitFilterArr.Length > 1 Then
                                '' for getting def. value from field master
                                Dim row() As DataRow = dsFields.Select("fieldid=" & InitFilterArr(0).ToString())
                                If arr(0).ToUpper() = "DOCUMENT" Or arr(0).ToUpper() = "MASTER" Then
                                    If row.Length > 0 Then
                                        str = " Select " & arr(2).ToString() & ", convert(nvarchar(10),tid) [TID] FROM " & TABLENAME & " M where EID=" & HttpContext.Current.Session("EID") & " AND  DOCUMENTTYPE='" & arr(1).ToString() & "'"
                                        str = str & " AND " & InitFilterArr(1).ToString() & "='" & row(0).Item("defaultFieldVal").ToString & "'"

                                        str = str & "  AND M.isauth=1 " & xwhr & " order by " & arr(2).ToString()

                                    End If
                                ElseIf arr(0).ToUpper() = "STATIC" Then
                                    If row.Length > 0 Then
                                        str = " Select " & arr(2).ToString() & ",convert(nvarchar(10)," & TID & ")  [tid]" & "FROM " & TABLENAME & " M where EID=" & HttpContext.Current.Session("EID") & " "
                                        str = str & " AND " & InitFilterArr(1).ToString() & "='" & row(0).Item("defaultFieldVal").ToString & "'"
                                        str = str & " AND M.isauth=1 " & xwhr
                                        ' to be used for apm user bind from role assignment also 12_sep_14
                                        str = str & " union Select  " & arr(2).ToString() & ",convert(nvarchar(10)," & TID & ")  [tid]" & "FROM " & TABLENAME & " where convert(nvarchar(10)," & TID & ") in (select uid from MMM_Ref_Role_User where eid=" & HttpContext.Current.Session("EID") & " and rolename='" & row(0).Item("defaultFieldVal").ToString & "') order by " & arr(2).ToString() & ""
                                    End If
                                End If
                            End If
                            con = New SqlConnection(conStr)
                            oda = New SqlDataAdapter("", con)
                            Dim dss As New DataSet

                            If str.Length > 0 Then
                                oda.SelectCommand.CommandText = str
                                oda.Fill(dss, "FV")
                                Dim isAddJquery As Integer = 0
                                ddl.Items.Add("Select")
                                ddl.Items(0).Value = "0"
                                For J As Integer = 0 To dss.Tables("FV").Rows.Count - 1
                                    ddl.Items.Add(dss.Tables("FV").Rows(J).Item(0).ToString())
                                    Dim lookddlVal As String = dss.Tables("FV").Rows(J).Item(1).ToString()
                                    ddl.Items(J + 1).Value = lookddlVal
                                Next
                                dss.Dispose()
                                If isAddJquery = 1 Then
                                    Dim JQuertStr As String = "var r1 = $('#ContentPlaceHolder1_" & ddl.ClientID & "').val(); var l = 0; var mycars = new Array(); for (var i = 0; i < r1.length; i++) { if (r1[i] == '|') { l++; mycars[l] = i; } } for (var i1 = 1; i1 < l; i1++) { var outpu = r1.substring(mycars[i1] + 1, mycars[i1 + 1]); var outpu1 = outpu.substring(0, outpu.indexOf(':')); var outpu2 = outpu.substring(outpu.indexOf(':') + 1); if (outpu2 == 'S') { var out = r1.substring(0, mycars[1]); var x = $('#ContentPlaceHolder1_tmp' + outpu1 + ' option').length; var options = ''; txt = ''; for (i = 0; i < x; i++) { var strUser = $('#ContentPlaceHolder1_tmp' + outpu1 + ' option:eq(' + i + ')').val(); var sel = strUser.substring(strUser.indexOf('-') + 1);  if (out == sel) { var finalshow = $('#ContentPlaceHolder1_tmp' + outpu1 + ' option:eq(' + i + ')').text();  options = options + '<option value=' + finalshow + '>' + finalshow + '</option>\n'; } } $('#ContentPlaceHolder1_' + outpu1 + '').html(options); } else { $('#ContentPlaceHolder1_' + outpu1 + '').val(outpu2); } $('#ContentPlaceHolder1_' + outpu1 + '').val(outpu2); }"
                                End If
                            End If

                            ' NEW ADDED BY SUNIL ON 07-12-13 FOR CHILD-CHILD FILTERING
                        ElseIf UCase(dropdowntype) = "CHILD VALUED" Then
                            ' you are here on 09-dec-13
                            arr = ddlText.Split("-")
                            Dim Midstr As String = arr(1).ToString()
                            Dim TID As String = "TID"
                            Dim TABLENAME As String = ""
                            If UCase(arr(0).ToString()) = "MASTER" Then
                                TABLENAME = "MMM_MST_MASTER"
                            ElseIf UCase(arr(0).ToString()) = "DOCUMENT" Then
                                TABLENAME = "MMM_MST_DOC"
                            ElseIf UCase(arr(0).ToString()) = "CHILD" Then
                                TABLENAME = "MMM_MST_DOC_ITEM"
                                arrMid = arr(1).Split(":")

                            End If
                            Dim lookUpqry As String = ""
                            Dim str As String = ""
                            If arr(0).ToUpper() = "CHILD" Then
                                str = "select " & arr(2).ToString() & "," & TID & "[tid]  from " & TABLENAME & " M WHERE   DOCUMENTTYPE='" & arrMid(1).ToString() & "'"
                            End If

                            Dim xwhr As String = ""
                            Dim tids As String = ""

                            ''FILTER THE DATA ACCORDING TO USER 
                            tids = ob.UserDataFilter(ds.Rows(i).Item("documenttype").ToString(), arrMid(1).ToString())

                            If tids.Length >= 2 Then
                                xwhr = " AND CONVERT(NVARCHAR(10),TID) IN (" & tids & ")"
                            ElseIf tids = "0" Then
                                pnlFields.Visible = False
                                btnActEdit.Visible = False
                                UpdatePanel1.Update()
                                xwhr = ""
                            End If
                            str = str & "  AND M.isauth=1 " & xwhr & " order by " & arr(2).ToString()
                            'Dim AutoFilter As String = ds.Rows(i).Item("AutoFilter").ToString()
                            Dim AutoFilter As String = arrMid(0).ToString()
                            If AutoFilter.Length > 0 Then
                                If arr(0).ToUpper() = "CHILD" Then
                                ElseIf arr(0).ToUpper() <> "STATIC" Then
                                    str = "select " & arr(2).ToString() & ",convert(nvarchar(10),tid)  [tid]" & lookUpqry & " from " & TABLENAME & " M WHERE EID=" & HttpContext.Current.Session("EID") & " AND  DOCUMENTTYPE='" & arr(1).ToString() & "'"
                                    str = str & "  AND M.isauth=1 " & xwhr & " order by " & arr(2).ToString()

                                Else
                                    str = "select " & arr(2).ToString() & ",convert(nvarchar(10)," & TID & ")  [tid]" & lookUpqry & " from " & TABLENAME & " M WHERE EID=" & HttpContext.Current.Session("EID") & " "
                                    str = str & "  AND M.isauth=1 " & xwhr & " order by " & arr(2).ToString()

                                End If
                            End If
                            oda = New SqlDataAdapter("", con)
                            Dim dss As New DataSet
                            If str.Length > 0 Then
                                oda.SelectCommand.CommandText = str
                                oda.Fill(dss, "FV")
                                Dim isAddJquery As Integer = 0
                                ddl.Items.Add("Select")
                                ddl.Items(0).Value = "0"
                                For J As Integer = 0 To dss.Tables("FV").Rows.Count - 1
                                    ddl.Items.Add(dss.Tables("FV").Rows(J).Item(0).ToString())
                                    Dim lookddlVal As String = dss.Tables("FV").Rows(J).Item(1).ToString()
                                    ddl.Items(J + 1).Value = lookddlVal
                                Next
                                oda.Dispose()
                                dss.Dispose()
                                If isAddJquery = 1 Then
                                    Dim JQuertStr As String = "var r1 = $('#ContentPlaceHolder1_" & ddl.ClientID & "').val(); var l = 0; var mycars = new Array(); for (var i = 0; i < r1.length; i++) { if (r1[i] == '|') { l++; mycars[l] = i; } } for (var i1 = 1; i1 < l; i1++) { var outpu = r1.substring(mycars[i1] + 1, mycars[i1 + 1]); var outpu1 = outpu.substring(0, outpu.indexOf(':')); var outpu2 = outpu.substring(outpu.indexOf(':') + 1); if (outpu2 == 'S') { var out = r1.substring(0, mycars[1]); var x = $('#ContentPlaceHolder1_tmp' + outpu1 + ' option').length; var options = ''; txt = ''; for (i = 0; i < x; i++) { var strUser = $('#ContentPlaceHolder1_tmp' + outpu1 + ' option:eq(' + i + ')').val(); var sel = strUser.substring(strUser.indexOf('-') + 1);  if (out == sel) { var finalshow = $('#ContentPlaceHolder1_tmp' + outpu1 + ' option:eq(' + i + ')').text();  options = options + '<option value=' + finalshow + '>' + finalshow + '</option>\n'; } } $('#ContentPlaceHolder1_' + outpu1 + '').html(options); } else { $('#ContentPlaceHolder1_' + outpu1 + '').val(outpu2); } $('#ContentPlaceHolder1_' + outpu1 + '').val(outpu2); }"
                                    'ddl.Attributes.Add("onchange", JQuertStr)
                                End If
                            End If
                        ElseIf UCase(dropdowntype) = "SESSION VALUED" Then
                            oda1 = New SqlDataAdapter("", con)
                            Dim ds1 As New DataSet
                            Dim QRY As String = ""
                            Dim DROPDOWN As String() = ds.Rows(i).Item("DROPDOWN").ToString().Split("-")
                            If DROPDOWN(1).ToString.ToUpper = "USER" Then
                                QRY = "SELECT USERNAME ,UID FROM MMM_MST_USER WHERE EID=" & HttpContext.Current.Session("EID") & " AND " & DROPDOWN(2) & "='" & HttpContext.Current.Session(DROPDOWN(2)) & "'"
                            ElseIf DROPDOWN(1).ToString.ToUpper = "LOCATION" Then
                                QRY = "SELECT LOCATIONNAME ,LOCID FROM MMM_MST_LOCATION WHERE EID=" & HttpContext.Current.Session("EID") & " AND " & DROPDOWN(2) & "='" & HttpContext.Current.Session(DROPDOWN(2)) & "'"
                            End If
                            oda1.SelectCommand.CommandText = QRY
                            oda1.Fill(ds1, "SESSION")
                            ddl.Items.Clear()
                            For iI As Integer = 0 To ds1.Tables("SESSION").Rows.Count - 1
                                ddl.Items.Add(ds1.Tables("SESSION").Rows(iI).Item(0))

                                ddl.Items(iI).Value = ds1.Tables("SESSION").Rows(iI).Item(1)

                            Next
                            ddl.Items.Insert(0, "SELECT")

                        End If

                        pnlFields.Controls.Add(ddl)

                    Case "CHECKBOX LIST"
                        'newScreenHeight = newScreenHeight + 60
                        Dim dynmdiv As System.Web.UI.HtmlControls.HtmlGenericControl = New System.Web.UI.HtmlControls.HtmlGenericControl("DIV")
                        dynmdiv.ID = "div" & ds.Rows(i).Item("FieldID").ToString()
                        Dim chklist As New CheckBoxList
                        chklist.ID = "fld" & ds.Rows(i).Item("FieldID").ToString()
                        chklist.CssClass = "txtbox"
                        Dim ddlText As String = ds.Rows(i).Item("dropdown").ToString()
                        Dim dropdowntype As String = ds.Rows(i).Item("dropdowntype").ToString()
                        Dim arr() As String
                        If UCase(dropdowntype) = "FIX VALUED" Then
                            arr = ddlText.Split(",")
                            For ii As Integer = 0 To arr.Count - 1
                                chklist.Items.Add(arr(ii).ToUpper())
                            Next
                        ElseIf UCase(dropdowntype) = "MASTER VALUED" Then
                            arr = ddlText.Split("-")
                            Dim TID As String = "TID"
                            Dim TABLENAME As String = ""
                            If UCase(arr(0).ToString()) = "MASTER" Then
                                TABLENAME = "MMM_MST_MASTER"
                            ElseIf UCase(arr(0).ToString()) = "DOCUMENT" Then
                                TABLENAME = "MMM_MST_DOC"
                            ElseIf UCase(arr(0).ToString()) = "CHILD" Then
                                TABLENAME = "MMM_MST_DOC_ITEM"
                            ElseIf UCase(arr(0).ToString) = "STATIC" Then
                                If arr(1).ToString.ToUpper = "USER" Then
                                    TABLENAME = "MMM_MST_USER"
                                    TID = "UID"
                                ElseIf arr(1).ToString().ToUpper = "LOCATION" Then
                                    TABLENAME = "MMM_MST_LOCATION"
                                    TID = "LOCID"
                                End If
                            End If
                            Dim lookUpqry As String = ""
                            Dim str As String = ""
                            If arr(0).ToUpper() = "CHILD" Then
                                str = "select " & arr(2).ToString() & "," & TID & "[tid]  from " & TABLENAME & " M WHERE   DOCUMENTTYPE='" & arr(1).ToString() & "'"
                            ElseIf arr(0).ToUpper() <> "STATIC" Then
                                str = "select " & arr(2).ToString() & "," & TID & "[tid]" & lookUpqry & " from " & TABLENAME & " M WHERE EID=" & HttpContext.Current.Session("EID") & " AND  DOCUMENTTYPE='" & arr(1).ToString() & "'"
                            Else
                                If arr(2).ToString.ToUpper = "LOCATIONSTATE" Then
                                    str = "select DISTINCT " & arr(2).ToString() & ",SID [tid]" & lookUpqry & " from " & TABLENAME & " M "
                                Else
                                    str = "select " & arr(2).ToString() & "," & TID & "[tid]" & lookUpqry & " from " & TABLENAME & " M WHERE EID=" & HttpContext.Current.Session("EID") & " "
                                End If
                            End If
                            'Dim str As String = "select " & arr(2).ToString() & ",tid from " & TABLENAME & " WHERE EID=" & HttpContext.Current.Session("EID") & " AND  DOCUMENTTYPE='" & arr(1).ToString() & "'"
                            oda = New SqlDataAdapter("", con)
                            Dim dss As New DataSet
                            oda.SelectCommand.CommandText = str
                            oda.Fill(dss, "FV")
                            For J As Integer = 0 To dss.Tables("FV").Rows.Count - 1
                                chklist.Items.Add(dss.Tables("FV").Rows(J).Item(0))
                                chklist.Items(J).Value = dss.Tables("FV").Rows(J).Item(1)
                            Next
                            oda.Dispose()
                            dss.Dispose()
                        End If
                        'chklist.Style.Add("width", "100px")
                        dynmdiv.Style.Add(HtmlTextWriterStyle.Width, "400px")
                        dynmdiv.Style.Add(HtmlTextWriterStyle.Height, "100px")
                        dynmdiv.Style.Add(HtmlTextWriterStyle.Overflow, "Scroll")
                        dynmdiv.Controls.Add(chklist)
                        pnlFields.Controls.Add(dynmdiv)

                    Case "LIST BOX"
                        'newScreenHeight = newScreenHeight + 60
                        Dim ddl As New ListBox
                        ddl.ID = "fld" & ds.Rows(i).Item("FieldID").ToString()
                        ddl.Width = controlWdth - 2
                        ddl.CssClass = "txtBox"
                        Dim ddlText As String = ds.Rows(i).Item("dropdown").ToString()
                        Dim dropdowntype As String = ds.Rows(i).Item("dropdowntype").ToString()
                        Dim arr() As String
                        If UCase(dropdowntype) = "FIX VALUED" Then
                            arr = ddlText.Split(",")
                            ddl.Items.Add("")
                            For ii As Integer = 0 To arr.Count - 1
                                ddl.Items.Add(arr(ii).ToUpper())
                            Next
                        ElseIf UCase(dropdowntype) = "MASTER VALUED" Then
                            arr = ddlText.Split("-")
                            Dim TABLENAME As String = ""
                            If UCase(arr(0).ToString()) = "MASTER" Then
                                TABLENAME = "MMM_MST_MASTER"
                            Else
                                TABLENAME = "MMM_MST_DOC"
                            End If
                            Dim str As String = "select " & arr(2).ToString() & ",tid from " & TABLENAME & " WHERE EID=" & HttpContext.Current.Session("EID") & " AND  DOCUMENTTYPE='" & arr(1).ToString() & "'"
                            oda = New SqlDataAdapter("", con)
                            Dim dss As New DataSet
                            oda.SelectCommand.CommandText = str
                            oda.Fill(dss, "FV")
                            For J As Integer = 0 To dss.Tables("FV").Rows.Count - 1
                                ddl.Items.Add(dss.Tables("FV").Rows(J).Item(0))
                                ddl.Items(J).Value = dss.Tables("FV").Rows(J).Item(1)
                            Next
                            dss.Dispose()
                        End If
                        ddl.SelectionMode = ListSelectionMode.Multiple
                        pnlFields.Controls.Add(ddl)
                    Case "TEXT AREA"
                        datatype = ds.Rows(i).Item("datatype").ToString().ToUpper()
                        'newScreenHeight = newScreenHeight + 100
                        Dim txtBox As New TextBox
                        txtBox.ID = "fld" & ds.Rows(i).Item("FieldID").ToString()
                        txtBox.Width = controlWdth - 10
                        txtBox.CssClass = "txtBox"
                        txtBox.TextMode = TextBoxMode.MultiLine
                        If ds.Rows(i).Item("defaultfieldval").ToString.Length > 0 Then
                            txtBox.Text = ds.Rows(i).Item("defaultfieldval")
                        Else
                            If datatype = "NUMERIC" Then
                                txtBox.Text = "0"
                            End If
                        End If
                        pnlFields.Controls.Add(txtBox)
                End Select
                pnlFields.Controls.Add(New LiteralControl("</td>"))
                If i Mod 2 = 1 Then
                    pnlFields.Controls.Add(New LiteralControl("</tr>"))
                End If
            Next
            For i As Integer = 0 To ds.Rows.Count - 1
                Select Case ds.Rows(i).Item("FieldType").ToString().ToUpper()
                    Case "DROP DOWN"
                        Dim dropdowntype As String = ds.Rows(i).Item("dropdowntype").ToString()
                        Dim ddlText As String = ds.Rows(i).Item("dropdown").ToString()
                        If UCase(dropdowntype) = "FIX VALUED" Then
                            Dim ddl As New DropDownList
                            If ds.Rows(i).Item("ddlval").ToString().Length > 1 Then
                                ddl = CType(pnlFields.FindControl("fld" & ds.Rows(i).Item("FieldID").ToString()), DropDownList)
                                Dim dropdowntypeFill As String = ds.Rows(i).Item("ddlval").ToString().ToUpper()
                                ddl.SelectedIndex = ddl.Items.IndexOf(ddl.Items.FindByText(dropdowntypeFill))
                            End If

                        ElseIf UCase(dropdowntype) = "MASTER VALUED" Then
                            If ds.Rows(i).Item("ddlval").ToString().Length > 1 Then
                                Dim ddl As New DropDownList
                                ddl = CType(pnlFields.FindControl("fld" & ds.Rows(i).Item("FieldID").ToString()), DropDownList)
                                Dim dropdowntypeFill As String = ds.Rows(i).Item("ddlval").ToString().ToUpper().Trim()
                                ddl.SelectedIndex = ddl.Items.IndexOf(ddl.Items.FindByValue(dropdowntypeFill))
                                '''''For filling lookup and lookupddl according drop down'''''''
                                Dim id As String = Right(ddl.ID, ddl.ID.Length - 3)
                                Dim id1 As Integer = CInt(id)
                                '' code for filling lookup/filter on contol cration by sunil on 04th oct 14
                                ob.bind(id, pnlFields, ddl)
                                ob.bindlookupddl(id, pnlFields, ddl)
                            Else   '' new by sunil on 21_Oct_14
                                Dim ddl As New DropDownList
                                ddl = CType(pnlFields.FindControl("fld" & ds.Rows(i).Item("FieldID").ToString()), DropDownList)
                                If ddl.Items.Count = 2 Then  '' perform here action of selecting only one item and firing filters
                                    ddl.SelectedIndex = ddl.Items.IndexOf(ddl.Items.Item(1))
                                    Dim id As String = Right(ddl.ID, ddl.ID.Length - 3)
                                    Dim id1 As Integer = CInt(id)
                                    ob.bind(id, pnlFields, ddl)
                                    ob.bindlookupddl(id, pnlFields, ddl)

                                End If
                            End If
                        ElseIf UCase(dropdowntype) = "SESSION VALUED" Then
                            ''code for filling lookup/filter etc. if session valued..  by sunil
                            Dim ddl As New DropDownList
                            ddl = CType(pnlFields.FindControl("fld" & ds.Rows(i).Item("FieldID").ToString()), DropDownList)
                            Dim dropdowntypeFill As String = ds.Rows(i).Item("dropdowntype").ToString()
                            ddl.SelectedIndex = ddl.Items.IndexOf(ddl.Items.Item(1))

                            Dim id As String = Right(ddl.ID, ddl.ID.Length - 3)
                            Dim id1 As Integer = CInt(id)
                            ob.bind(id, pnlFields, ddl)
                            ob.bindlookupddl(id, pnlFields, ddl)
                        End If
                End Select
                If i Mod 2 = 1 Then
                    pnlFields.Controls.Add(New LiteralControl("</tr>"))
                End If
            Next
            pnlFields.Controls.Add(New LiteralControl("</table></div>"))
            ds.Dispose()
        Catch ex As Exception
        Finally
            If Not con Is Nothing Then
                con.Close()
                con.Dispose()
            End If
            If Not oda Is Nothing Then
                oda.Dispose()
            End If
            If Not oda1 Is Nothing Then
                oda1.Dispose()
            End If
        End Try
    End Sub

    Public Function IsValidMovementRequest(EID As Integer, DocID As Integer, OUID As Integer, WFStatus As String) As Boolean
        Dim ret As Boolean = False
        Dim CurUID As Integer = -9999
        Dim CurStatus As String = ""
        Try
            'Now Finding CurStatus And current userID OF Document
            Dim DSDOC As New DataSet()
            Using con = New SqlConnection(conStr)
                Using da = New SqlDataAdapter("GETCURRENTUSER", con)
                    da.SelectCommand.CommandType = CommandType.StoredProcedure
                    da.SelectCommand.Parameters.AddWithValue("@DOCID", DocID)
                    da.SelectCommand.Parameters.AddWithValue("@EID", EID)
                    da.Fill(DSDOC)
                End Using
            End Using
            If (DSDOC.Tables(0).Rows.Count > 0) Then
                CurUID = Convert.ToInt32(DSDOC.Tables(0).Rows(0).Item("userid"))
                CurStatus = Convert.ToString(DSDOC.Tables(0).Rows(0).Item("curstatus"))
                If (OUID = CurUID And CurStatus = WFStatus) Then
                    ret = True
                End If
            End If
            Return ret
        Catch ex As Exception
            Throw
        End Try

    End Function

End Class
