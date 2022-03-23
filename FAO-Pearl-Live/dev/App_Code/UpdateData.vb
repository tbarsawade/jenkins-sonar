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
Imports System.Globalization

Public Class UpdateData

    Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
    Dim objdc As New DataClass()
    Public Function UpdateData(EID As Integer, DocType As String, UID As Integer, Data As String, Optional EditType As String = "EnableEdit", Optional tid As String = "0", Optional DOCID As Integer = 0) As String
        Dim ret As String = ""
        Try
            Dim ds As New DataSet()
            Dim LStDataFinal As New List(Of DataWraper1)
            Using con As New SqlConnection(conStr)
                Using da As New SqlDataAdapter("getDataOfForm100", con)
                    da.SelectCommand.CommandType = CommandType.StoredProcedure
                    da.SelectCommand.Parameters.AddWithValue("@EID", EID)
                    da.SelectCommand.Parameters.AddWithValue("@FormName", DocType)
                    If con.State <> ConnectionState.Open Then
                        con.Open()
                    End If
                    da.Fill(ds)
                End Using
            End Using
            Dim onlyFiltered As DataView = ds.Tables(0).DefaultView
            ''onlyFiltered.RowFilter = "DocumentType='" & DocType & "' AND FieldType <> 'Formula Field' AND IsActive=1 and EnableEdit=1"
            onlyFiltered.RowFilter = "DocumentType='" & DocType & "' AND FieldType <> 'Formula Field' AND IsActive=1"
            Dim theFlds As DataTable = onlyFiltered.Table.DefaultView.ToTable()
            'Since user is a special type of document that's why adding some addition fields
            If DocType.ToUpper() = "USER" Then
                'Adding rows for static 
                AddUserStaticField(theFlds)
                'MASTER VALUED
            Else
                AddStaticField(theFlds)
            End If
            Dim StrFormData = ""
            Dim arrData As String() = Split(Data, "|")
            If theFlds.Rows.Count > 0 Then
                Dim FieldType = "", DisplayName = ""
                Dim LstData As New List(Of DataWraper)
                Dim objDW As New DataWraper()
                Dim objDWC As DataWraper
                'Adding Base Document to List
                objDW.DocumentType = DocType
                LstData.Add(objDW)
                'Loop through all the DataSet
                Dim IsParameterSupp = False
                For i As Integer = 0 To theFlds.Rows.Count - 1
                    FieldType = theFlds.Rows(i).Item("FieldType")
                    DisplayName = theFlds.Rows(i).Item("DisplayName")
                    IsParameterSupp = False
                    For j As Integer = 0 To arrData.Length - 1
                        'Split for getting Key value
                        Dim arr = Split(arrData(j), "::")
                        If arr.Length = 2 Then
                            If arr(0).Trim().ToUpper() = DisplayName.ToUpper().Trim() Then
                                IsParameterSupp = True
                                If FieldType.ToUpper() = "CHILD ITEM" Then
                                    objDWC = New DataWraper()
                                    objDWC.DocumentType = theFlds.Rows(i).Item("DropDown").ToString()
                                    objDWC.FormType = theFlds.Rows(i).Item("FormType").ToString()
                                    Dim Line = arr(1) & "{}"
                                    'Getting all the line item of child item
                                    Line = Line.Replace("()", "|").Replace("<>", "::").ToString()
                                    objDWC.Data = Line
                                    LstData.Add(objDWC)
                                Else
                                    If objDW.DocumentType.ToUpper() <> "USER" Then
                                        objDW.FormType = theFlds.Rows(i).Item("FormType").ToString()
                                    Else
                                        objDW.FormType = "USER"
                                    End If
                                    If StrFormData = "" Then
                                        StrFormData = DisplayName & "::" & arr(1)
                                    Else
                                        StrFormData = StrFormData & "|" & DisplayName & "::" & arr(1)
                                    End If
                                End If
                                Exit For
                            End If
                        Else
                            IsParameterSupp = False
                        End If
                    Next
                Next
                objDW.DocumentType = DocType
                objDW.Data = StrFormData
                Dim UDataC As UserData
                Dim objF As DataWraper1
                Dim objC As DataWraper1
                Dim ObjItem As LineitemWrap
                Dim LStDataItem As List(Of UserData)
                Dim LstlineItem As List(Of LineitemWrap)
                Dim IsKeysSupplyed As Boolean = True
                Dim keys As String = ""
                For Each objData In LstData
                    If DocType = objData.DocumentType Then
                        'IsKeysSupplyed = ValidateKeys(EID, DocType, Data, keys)
                        If (tid = "0") Then
                            IsKeysSupplyed = ValidateKeys(EID, DocType, Data, keys)
                        Else
                            IsKeysSupplyed = True
                            keys = ""
                        End If
                        If IsKeysSupplyed = True Then
                            objF = New DataWraper1()
                            LStDataItem = New List(Of UserData)
                            onlyFiltered.RowFilter = "DocumentType='" & DocType & "' AND FieldType <> 'Formula Field'  AND IsActive=1 AND FieldType <> 'Child Item' and " & EditType & " =1"
                            theFlds = onlyFiltered.Table.DefaultView.ToTable()
                            'getting one row data
                            ObjItem = New LineitemWrap()
                            LstlineItem = New List(Of LineitemWrap)
                            ObjItem = CreateListCollection(objData.Data, theFlds, EID, DocType, DOCID:=DOCID)
                            LstlineItem.Add(ObjItem)
                            'LstlineItem.Add(LStDataItem)
                            objF.LineItem = LstlineItem
                            objF.DocumentType = DocType
                            objF.FormType = objData.FormType
                            'objF.LineItem = LStDataItem
                            LStDataFinal.Add(objF)
                        Else
                            ret = keys
                            Exit For
                        End If
                    Else
                        'We will do it later for child to update child item
                        Dim arr As String() = Split(objData.Data, "{}")
                        UDataC = New UserData()
                        onlyFiltered.RowFilter = "DocumentType='" & objData.DocumentType & "' AND FieldType <> 'Formula Field'  AND IsActive=1 AND FieldType <> 'Child Item'"
                        theFlds = onlyFiltered.Table.DefaultView.ToTable()
                        ObjItem = New LineitemWrap()
                        LstlineItem = New List(Of LineitemWrap)
                        objC = New DataWraper1()
                        Dim Count = 0
                        For d As Integer = 0 To arr.Count - 1
                            'we will do it later once new property will be added 
                            'IsKeysSupplyed = IsKeysSupplyed & ValidateKeys(EID, DocType, arr(d), keys)
                            objC.DocumentType = objData.DocumentType
                            objC.FormType = objData.FormType
                            If arr(d).Trim <> "" Then
                                UDataC = New UserData()
                                Count = 1
                                ObjItem = CreateListCollection(arr(d), theFlds, EID, objData.DocumentType, Count)
                                LstlineItem.Add(ObjItem)
                            End If
                        Next

                        'Add condition for unique keys and values Mayank'
                        Dim fldMapping As String = Convert.ToString(objdc.ExecuteQryScaller("select  FieldMapping   from mmm_mst_fields with(nolock) where documenttype='" & objData.DocumentType & "' and eid=" & EID & " and isunique=1"))
                        If (Not String.IsNullOrEmpty(fldMapping)) Then
                            Dim objTempData As New DataTable()
                            objTempData = objdc.ExecuteQryDT("select " & fldMapping & ",TID from mmm_mst_doc_item with(nolock) where docid=" & DOCID & " and documenttype='" & objData.DocumentType & "'")
                            Dim objuniqueCollectionList As New UniqueCollectionList()
                            Dim uniqueCollectionData As New List(Of UniqueCollection)
                            For Each dr As DataRow In objTempData.Rows
                                Dim uniqueData As New UniqueCollection()
                                uniqueData.fieldValue = Convert.ToString(dr(0))
                                uniqueData.fieldiID = dr(1)
                                uniqueData.IsFound = False
                                uniqueCollectionData.Add(uniqueData)
                            Next
                            objuniqueCollectionList.uniqueCollectionData = uniqueCollectionData

                            Dim onlytheFlds As DataView = theFlds.DefaultView
                            onlytheFlds.RowFilter = "dbRecordonEdit=1"
                            Dim theFilterFlds As DataTable = onlytheFlds.Table.DefaultView.ToTable()
                            For Each _objItem In LstlineItem
                                For Each objDR As DataRow In theFilterFlds.Rows
                                    Dim fieldID As Integer = 0
                                    For Each _eachItem In _objItem.DataItem
                                        If (_eachItem.FieldMapping = fldMapping) Then
                                            For Each lstItem As UniqueCollection In objuniqueCollectionList.uniqueCollectionData
                                                If (lstItem.fieldValue = _eachItem.Values) Then
                                                    fieldID = lstItem.fieldiID
                                                    Exit For
                                                End If
                                            Next
                                            '_eachItem.Values = objdc.ExecuteQryScaller("select " & _eachItem.FieldMapping & " from mmm_mst_doc_item with(nolock) where docid=" & DOCID & "")
                                        End If
                                    Next
                                    For Each _eachItemValue In _objItem.DataItem
                                        If (_eachItemValue.FieldMapping = objDR("fieldMapping")) Then
                                            _eachItemValue.Values = objdc.ExecuteQryScaller("if  exists(select isnull(" & _eachItemValue.FieldMapping & ",0) from mmm_mst_doc_item with(nolock) where docid=" & DOCID & " and tid=" & fieldID & ") begin select isnull(" & _eachItemValue.FieldMapping & ",0) from mmm_mst_doc_item with(nolock) where docid=" & DOCID & " and tid=" & fieldID & " end else begin select 0 end ")
                                        End If

                                    Next
                                Next
                            Next
                        End If




                        objC.LineItem = LstlineItem
                        LStDataFinal.Add(objC)
                    End If
                    'Else
                    '    ret = keys
                    'End If
                Next
                If IsKeysSupplyed = True Then
                    'Fill Drop Down And lookup values
                    CommanUtil.FillData(LStDataFinal, ds, EID, DocType, UID)
                    'Validate Form
                    Dim FormData As New DataWraper1()
                    Dim lineitem As LineitemWrap
                    'Navigating all the form
                    Dim ErrMsg = "", AllError = ""
                    Dim IsAllFormValid As Boolean = True
                    Dim Flag As Boolean = True
                    Dim lstData1 As New List(Of UserData)
                    Dim count As Integer = 0
                    For Each Form In LStDataFinal
                        FormData = Form
                        Dim DocumentType = FormData.DocumentType
                        Dim LstLine = FormData.LineItem
                        For Each Row In LstLine
                            count = count + 1
                            ErrMsg = "Error(s) in document " & DocumentType & ". "
                            lineitem = New LineitemWrap()
                            lineitem = Row
                            Flag = CommanUtil.ValidateForm(EID, lineitem, Form.FormType, DocumentType, ErrMsg)
                            IsAllFormValid = IsAllFormValid And Flag
                            If Flag = False Then
                                If AllError = "" Then
                                    AllError = ErrMsg
                                Else
                                    AllError = AllError & "." & ErrMsg & " , error in line number at " & count
                                End If
                            End If


                            ''Execute rule for main form'
                            ''Code For Rule Engine
                            Dim ObjRet As New RuleResponse()
                            Dim objRule As New RuleEngin(EID, Form.DocumentType, "MODIFY", "SUBMIT")
                            Dim dsrule As DataSet = objRule.GetRules()
                            Dim dtrule As New DataTable
                            dtrule = dsrule.Tables(1)
                            If Form.DocumentType.ToUpper.Trim = DocType.ToUpper.Trim Then
                                lstData1 = lineitem.DataItem
                            End If

                            For Each dr As DataRow In dsrule.Tables(0).Rows
                                If Form.DocumentType.ToUpper.Trim = DocType.ToUpper.Trim Then
                                    ObjRet = objRule.ExecuteRule(lineitem.DataItem, Nothing, False, "", dr, dtrule)
                                Else
                                    ObjRet = objRule.ExecuteRule(lstData1, lineitem.DataItem, True, "", dr, dtrule)
                                End If
                                If ObjRet.Success = False Then
                                    Flag = ObjRet.Success
                                    AllError &= " , Error In line number at " & count & " just because Of " & ObjRet.ErrorMessage
                                End If
                            Next
                            If Flag = False Then
                                'AllError &= ObjRet.ErrorMessage
                                IsAllFormValid = False
                                'Exit For
                            End If

                        Next
                    Next




                    'Execute rule for child Item'

                    If IsAllFormValid = True Then
                        'Now check condition for rule execution Mayank'
                        ret = Update(EID, DocType, LStDataFinal, UID, keys, EditType, tid)
                    Else
                        ret = AllError
                    End If
                End If
            Else
                ret = "Your request can Not be completed.Please contact admin To Get it resolved."
            End If
        Catch ex As Exception
            ErrorLog.sendMail("UpdateData.UpdateData", ex.Message)
            Throw
        End Try
        Return ret
    End Function

    Public Shared Function CreateListCollection(UData As String, DtField As DataTable, EID As Integer, DocType As String, Optional IsChild As Integer = 0, Optional DOCID As Integer = 0, Optional serviceUniqueColumn As String = "", Optional lstCollection As List(Of UniqueCollection) = Nothing) As LineitemWrap
        Dim lstData1 As New List(Of UserData)
        Dim LineItem As New LineitemWrap()
        Dim Obj As UserData
        Dim arrData As String() = UData.Split("|")

        Try
            If DocType.ToUpper() = "USER" Then
                'Adding rows 
                AddUserStaticField(DtField)
            Else
                If IsChild = 0 Then
                    AddStaticField(DtField)
                End If
            End If
        Catch ex As Exception
            Throw
        End Try
        For i As Integer = 0 To DtField.Rows.Count - 1
            Obj = New UserData()
            Dim IsParameterFound = False
            For j As Integer = 0 To arrData.Count - 1
                Dim arr As String() = Split(arrData(j).ToString(), "::")
                Obj.DisplayName = DtField.Rows(i).Item("DisplayName").ToString()
                Obj.FieldType = DtField.Rows(i).Item("FieldType").ToString()
                Obj.DataType = DtField.Rows(i).Item("DataType").ToString()
                Obj.MinVal = DtField.Rows(i).Item("MinLen").ToString()
                Obj.MaxVal = DtField.Rows(i).Item("MaxLen").ToString()
                Obj.IsRequired = DtField.Rows(i).Item("IsRequired").ToString()
                Obj.FieldID = DtField.Rows(i).Item("FieldID").ToString()
                Obj.FieldMapping = DtField.Rows(i).Item("FieldMapping").ToString()
                Obj.DropDownType = DtField.Rows(i).Item("DropDownType").ToString()
                Obj.CalText = DtField.Rows(i).Item("cal_text").ToString()
                Obj.LookUp = DtField.Rows(i).Item("lookupvalue").ToString()
                Obj.DropDown = DtField.Rows(i).Item("DropDown").ToString()
                Obj.IsUnique = DtField.Rows(i).Item("Isunique").ToString()
                Obj.AutoFilter = DtField.Rows(i).Item("AutoFilter").ToString()
                Obj.DisplayOrder = DtField.Rows(i).Item("Displayorder").ToString()
                Obj.EnableEdit = DtField.Rows(i).Item("EnableEdit").ToString()
                Obj.IsEditOnamend = DtField.Rows(i).Item("IsEditOnamend").ToString()
                'IsEditOnamend
                'Changes by Mayank
                Obj.DocumentType = Convert.ToString(DtField.Rows(i).Item("documenttype"))

                If Not IsDBNull(DtField.Rows(i).Item("AllowCreateRecord_onfly")) Then
                    Obj.AllowCreatedRecord = DtField.Rows(i).Item("AllowCreateRecord_onfly")
                Else
                    Obj.AllowCreatedRecord = False
                End If

                If Obj.AllowCreatedRecord = True Then
                    Obj.OnflyFieldmapping = Convert.ToString(DtField.Rows(i).Item("OnflyFieldmapping"))
                Else
                    Obj.OnflyFieldmapping = String.Empty
                End If
                'Changes by Mayank
                'Public Property IsUnique As String
                If DtField.Rows(i).Item("DisplayName").ToString().Trim().ToUpper = arr(0).Trim().ToUpper Then
                    Obj.Values = arr(1).Trim()
                    Obj.DDLValue = arr(1).Trim()
                    Obj.DocumentType = DtField.Rows(i).Item("Documenttype").ToString().Trim().ToUpper
                    IsParameterFound = True
                End If
                'If (Not IsDBNull(DtField.Rows(i).Item("dbRecordOnEdit"))) Then
                '    If (Convert.ToInt32(DtField.Rows(i).Item("dbRecordOnEdit")) > 0) Then
                '        Dim objDC As New DataClass()
                '        If (IsChild = 1) Then
                '            Obj.Values = objDC.ExecuteQryScaller("select " & serviceUniqueColumn & " mmm_mst_doc_item where docid=" & DOCID & " AND " & serviceUniqueCondition)
                '        End If

                '    End If
                'End If



            Next

            If IsParameterFound = False Then
                Obj.Values = ""
                Obj.DDLValue = ""
            End If
            lstData1.Add(Obj)
        Next
        LineItem.DataItem = lstData1
        Return LineItem
    End Function

    Public Shared Function AddUserStaticField(ByRef dt As DataTable) As DataTable

        dt.Rows.Add()
        dt.Rows(dt.Rows.Count - 1).Item("DisplayName") = "USERNAME"
        dt.Rows(dt.Rows.Count - 1).Item("FieldType") = "Text Box"
        dt.Rows(dt.Rows.Count - 1).Item("IsRequired") = "1"
        dt.Rows(dt.Rows.Count - 1).Item("IsRequired") = "1"
        dt.Rows(dt.Rows.Count - 1).Item("Datatype") = "Text"
        dt.Rows(dt.Rows.Count - 1).Item("FieldMapping") = "UserName"
        dt.Rows(dt.Rows.Count - 1).Item("FieldID") = "-19999"
        dt.Rows(dt.Rows.Count - 1).Item("MinLen") = "4"
        dt.Rows(dt.Rows.Count - 1).Item("MaxLen") = "50"
        dt.Rows(dt.Rows.Count - 1).Item("EnableEdit") = "1"
        dt.Rows(dt.Rows.Count - 1).Item("IsEditOnamend") = "1"
        dt.Rows(dt.Rows.Count - 1).Item("displayorder") = "0"
        'Text
        dt.Rows.Add()
        dt.Rows(dt.Rows.Count - 1).Item("DisplayName") = "EMAILID"
        dt.Rows(dt.Rows.Count - 1).Item("FieldType") = "Text Box"
        dt.Rows(dt.Rows.Count - 1).Item("IsRequired") = "1"
        dt.Rows(dt.Rows.Count - 1).Item("Datatype") = "Text"
        dt.Rows(dt.Rows.Count - 1).Item("FieldMapping") = "EmailID"
        dt.Rows(dt.Rows.Count - 1).Item("FieldID") = "-20000"
        dt.Rows(dt.Rows.Count - 1).Item("MaxLen") = "50"
        dt.Rows(dt.Rows.Count - 1).Item("MinLen") = "4"
        dt.Rows(dt.Rows.Count - 1).Item("EnableEdit") = "1"
        dt.Rows(dt.Rows.Count - 1).Item("IsEditOnamend") = "1"
        dt.Rows(dt.Rows.Count - 1).Item("displayorder") = "0"
        'EnableEdit
        dt.Rows.Add()
        dt.Rows(dt.Rows.Count - 1).Item("DisplayName") = "USERROLE"
        dt.Rows(dt.Rows.Count - 1).Item("FieldType") = "Drop Down"
        dt.Rows(dt.Rows.Count - 1).Item("IsRequired") = "1"
        dt.Rows(dt.Rows.Count - 1).Item("Datatype") = "Text"
        dt.Rows(dt.Rows.Count - 1).Item("FieldMapping") = "userrole"
        dt.Rows(dt.Rows.Count - 1).Item("FieldID") = "-30000"
        dt.Rows(dt.Rows.Count - 1).Item("MaxLen") = "50"
        dt.Rows(dt.Rows.Count - 1).Item("DropDown") = "Static-MMM_MST_Role-RoleName"
        dt.Rows(dt.Rows.Count - 1).Item("DropDownType") = "MASTER VALUED"
        dt.Rows(dt.Rows.Count - 1).Item("MinLen") = "4"
        dt.Rows(dt.Rows.Count - 1).Item("EnableEdit") = "1"
        dt.Rows(dt.Rows.Count - 1).Item("IsEditOnamend") = "1"
        dt.Rows(dt.Rows.Count - 1).Item("displayorder") = "0"
        dt.Rows.Add()
        dt.Rows(dt.Rows.Count - 1).Item("DisplayName") = "isAuth"
        dt.Rows(dt.Rows.Count - 1).Item("FieldType") = "Text Box"
        dt.Rows(dt.Rows.Count - 1).Item("IsRequired") = "1"
        dt.Rows(dt.Rows.Count - 1).Item("Datatype") = "Numeric"
        dt.Rows(dt.Rows.Count - 1).Item("FieldMapping") = "IsAuth"
        dt.Rows(dt.Rows.Count - 1).Item("FieldID") = "-300000"
        dt.Rows(dt.Rows.Count - 1).Item("MaxLen") = "1"
        dt.Rows(dt.Rows.Count - 1).Item("DropDown") = ""
        dt.Rows(dt.Rows.Count - 1).Item("DropDownType") = ""
        dt.Rows(dt.Rows.Count - 1).Item("MinLen") = "1"
        dt.Rows(dt.Rows.Count - 1).Item("EnableEdit") = "1"
        dt.Rows(dt.Rows.Count - 1).Item("IsEditonAmend") = "1"
        dt.Rows(dt.Rows.Count - 1).Item("displayorder") = "0"
        Return dt
    End Function

    Public Shared Function AddStaticField(ByRef dt As DataTable) As DataTable

        dt.Rows.Add()
        dt.Rows(dt.Rows.Count - 1).Item("DisplayName") = "IsAuth"
        dt.Rows(dt.Rows.Count - 1).Item("FieldType") = "Text Box"
        dt.Rows(dt.Rows.Count - 1).Item("IsRequired") = "0"
        dt.Rows(dt.Rows.Count - 1).Item("Datatype") = "Numeric"
        dt.Rows(dt.Rows.Count - 1).Item("FieldMapping") = "IsAuth"
        dt.Rows(dt.Rows.Count - 1).Item("FieldID") = "-199999"
        dt.Rows(dt.Rows.Count - 1).Item("MinLen") = "1"
        dt.Rows(dt.Rows.Count - 1).Item("MaxLen") = "1"
        dt.Rows(dt.Rows.Count - 1).Item("EnableEdit") = "1"
        dt.Rows(dt.Rows.Count - 1).Item("FormType") = dt.Rows(0).Item("FormType")
        dt.Rows(dt.Rows.Count - 1).Item("IsEditonAmend") = "1"
        dt.Rows(dt.Rows.Count - 1).Item("displayorder") = "999"
        'Text
        Return dt
    End Function

    Public Shared Function Update(EID As Integer, DocType As String, LStDataFinal As List(Of DataWraper1), UID As Integer, Keys As String, Optional EditType As String = "EnableEdit", Optional tid As String = "0", Optional DOCID As Integer = 0) As String
        Dim strResult = ""
        Dim StrQuery = ""
        Dim FileID = 0
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = Nothing
        Dim da As SqlDataAdapter = Nothing
        Dim FormType = ""
        Dim DocDBTableName = ""
        Dim MainDocType = ""
        Try
            'Getting Base Form For Generating Query
            Dim FormType1 = ""
            Dim MainForm = From Form In LStDataFinal Where Form.DocumentType = DocType Select Form
            If Not MainForm Is Nothing Then
                For Each line In MainForm
                    FormType = line.FormType
                    MainDocType = line.DocumentType
                    If line.FormType.ToUpper = "MASTER" Then
                        DocDBTableName = "MMM_MST_MASTER"
                        StrQuery = "Update MMM_MST_MASTER SET UpdatedDate=GETDATE() "
                        FormType1 = "Master"
                    ElseIf line.FormType.ToUpper = "USER" Then
                        DocDBTableName = "MMM_MST_USER"
                        StrQuery = "Update MMM_MST_USER   SET ModifyDate=GETDATE() "
                        FormType1 = "USER"
                    Else
                        DocDBTableName = "MMM_MST_DOC"
                        StrQuery = "Update MMM_MST_DOC SET lastupdate=GETDATE() "
                        FormType1 = "Document"
                    End If
                    For Each obj In line.LineItem
                        StrQuery = GenerateQuery(StrQuery, obj, EditType)
                    Next
                    Dim chkQuery = ""
                    'Query for getting transactiob ID from the database.
                    If FormType1 = "USER" Then
                        chkQuery = "Select UID from " & DocDBTableName & " WHERE EID=" & EID & Keys.Trim()
                    Else
                        chkQuery = "Select tid from " & DocDBTableName & " WHERE EID=" & EID & " AND DocumentType ='" & line.DocumentType & "'" & Keys.Trim()
                    End If
                    con = New SqlConnection(conStr)
                    da = New SqlDataAdapter(chkQuery, con)
                    If con.State <> ConnectionState.Open Then
                        con.Open()
                    End If


                    'FileID = da.SelectCommand.ExecuteScalar()
                    If (tid = "0") Then
                        FileID = da.SelectCommand.ExecuteScalar()
                    Else
                        FileID = tid
                    End If
                    'Code for maintaining history
                    If Convert.ToInt32(FileID) > 0 Then
                        Dim trans As SqlTransaction = Nothing
                        Try
                            Dim HisReport = CommanUtil.SetHistory(EID, FileID, DocDBTableName, UID, "Update WS", DocType)
                            If HisReport.ToString.ToLower <> "fail" Then
                                'Updating Record of data baes
                                If FormType1.ToUpper = "USER" Then
                                    'if Form name is of userType
                                    'Logic for unique goes from here
                                    StrQuery = StrQuery & " WHERE EID=" & EID & Keys & " AND UID= " & FileID
                                Else
                                    StrQuery = StrQuery & " WHERE EID=" & EID & " AND DocumentType ='" & line.DocumentType & "' " & Keys & " AND tid= " & FileID
                                End If

                                con = New SqlConnection(conStr)
                                con.Open()
                                trans = con.BeginTransaction()
                                'Logic for Canceling Period wise balance
                                Dim objRel As New Relation()
                                objRel.CanclePeriodWiseBalance(EID, FileID, line.DocumentType, con, trans)
                                da = New SqlDataAdapter(StrQuery, con)
                                da.SelectCommand.Transaction = trans
                                Dim ret = da.SelectCommand.ExecuteNonQuery()
                                If ret > 0 Then
                                    'Execute Formula From here 
                                    Try
                                        Dim FormulaQ = "select * FROM MMM_MST_FIELDS WHERE EID=" & EID & " AND DocumentType='" & line.DocumentType & "' AND FieldType='Formula Field' order by displayorder"
                                        Dim DSF As New DataSet()
                                        'da = New SqlDataAdapter(FormulaQ, con)
                                        da.SelectCommand.CommandText = FormulaQ
                                        da.Fill(DSF)
                                        Dim viewdoc As String = line.DocumentType
                                        viewdoc = viewdoc.Replace(" ", "_")
                                        If DSF.Tables(0).Rows.Count > 0 Then
                                            For f As Integer = 0 To DSF.Tables(0).Rows.Count - 1
                                                Dim formulaeditorr As New formulaEditor
                                                Dim forvalue As String = String.Empty
                                                forvalue = formulaeditorr.ExecuteFormulaT(DSF.Tables(0).Rows(f).Item("KC_LOGIC"), FileID, "v" + EID.ToString + viewdoc, EID, 1, con, trans)
                                                Dim upquery As String = "update " & DSF.Tables(0).Rows(f).Item("DBTableName").ToString & "  set  " & DSF.Tables(0).Rows(f).Item("FieldMapping").ToString & "='" & forvalue.ToString & "'  where tid =" & FileID & ""
                                                da.SelectCommand.CommandText = upquery
                                                da.SelectCommand.CommandType = CommandType.Text
                                                da.SelectCommand.ExecuteNonQuery()
                                            Next
                                        End If
                                    Catch ex As Exception
                                        Throw
                                    End Try
                                    'Excuting trigger from here
                                    Try
                                        Trigger.ExecuteTriggerT(MainDocType, EID, FileID, con, trans, TriggerNature:="UPDATE", FormType:=line.FormType)
                                    Catch ex As Exception
                                        Throw
                                    End Try
                                    'Code For Executing Child Item Data
                                    'First delete all child item of concern Document
                                    If line.FormType.ToUpper.Trim = "DOCUMENT" Then
                                        'Check unique condition for child item Mayank'
                                        Dim objDC As New DataClass()
                                        Dim CHldForm = From CForm In LStDataFinal Where CForm.DocumentType <> DocType Select CForm
                                        For Each Cline In CHldForm
                                            Dim StrQry1 = ""
                                            StrQuery = ""
                                            Dim fldMapping As String = Convert.ToString(objDC.TranExecuteQryScaller("select  FieldMapping   from mmm_mst_fields with(nolock) where documenttype='" & Cline.DocumentType & "' and eid=" & EID & " and isunique=1", con:=con, tran:=trans))
                                            If (Not String.IsNullOrEmpty(fldMapping)) Then
                                                Dim objTempData As New DataTable()
                                                objTempData = objDC.TranExecuteQryDT("select " & fldMapping & " from mmm_mst_doc_item with(nolock) where docid=" & FileID & " and documenttype='" & Cline.DocumentType & "'", con:=con, tran:=trans)
                                                Dim objuniqueCollectionList As New UniqueCollectionList()
                                                Dim uniqueCollectionData As New List(Of UniqueCollection)
                                                For Each dr As DataRow In objTempData.Rows
                                                    Dim uniqueData As New UniqueCollection()
                                                    uniqueData.fieldValue = Convert.ToString(dr(0))
                                                    uniqueData.IsFound = False
                                                    uniqueData.fieldMapping = fldMapping
                                                    uniqueCollectionData.Add(uniqueData)
                                                Next
                                                objuniqueCollectionList.uniqueCollectionData = uniqueCollectionData
                                                '''' new added by sunil for maintaining history of child item to be updaed on 13_jun_19 
                                                Dim HisRep = CommanUtil.SetHistory(EID, FileID, "MMM_MST_DOC_ITEM", UID, "Update WS", Cline.DocumentType)
                                                Dim conditionvalue As String = ""
                                                conditionvalue = " where docid=" & FileID & " "
                                                Dim UniqueCondition As String = ""
                                                Dim count As Integer = 0
                                                For Each obj In Cline.LineItem
                                                    StrQuery = ""  ' ' new added by sunil for making blank the variable 
                                                    count = count + 1
                                                    Dim isUniqueDataFound As Boolean = False
                                                    Dim isvalueFoundInDB As Boolean = False
                                                    Dim updateQuery As String = " Update MMM_MST_DOC_ITEM set lastupdate=getdate() "
                                                    Dim UpdateArrayList As New ArrayList()


                                                    For Each objItem In obj.DataItem
                                                        If (objItem.EnableEdit = "1") Then
                                                            UpdateArrayList.Add("" & objItem.FieldMapping & " = '" & objItem.Values & "'")
                                                        End If
                                                        If (objItem.FieldMapping = fldMapping) Then
                                                            isUniqueDataFound = True
                                                            UniqueCondition = objItem.FieldMapping & " = '" & objItem.Values & "'"
                                                            For Each objlist In objuniqueCollectionList.uniqueCollectionData
                                                                If (objlist.fieldValue = objItem.Values) Then
                                                                    objlist.IsFound = True
                                                                    isvalueFoundInDB = True
                                                                    Exit For
                                                                End If
                                                            Next

                                                        End If
                                                    Next
                                                    If (isUniqueDataFound = True And isvalueFoundInDB = True) Then
                                                        If UpdateArrayList.Count > 0 Then
                                                            updateQuery = updateQuery & "," & String.Join(",", UpdateArrayList.ToArray) & " " & conditionvalue & " AND " & UniqueCondition & " "
                                                            objDC.TranExecuteQryDT(updateQuery, con:=con, tran:=trans)
                                                        End If
                                                    ElseIf (isUniqueDataFound = False) Then
                                                        strResult = "Unique field must be supply in string"
                                                        trans.Rollback()
                                                        Return strResult
                                                    Else
                                                        'Dim StrChlddelQ = "delete from MMM_MST_DOC_ITEM where DOCID=" & FileID
                                                        'da.SelectCommand.CommandText = StrChlddelQ
                                                        'Dim TotaldelChild = da.SelectCommand.ExecuteNonQuery()
                                                        'For Each obj In Cline.LineItem
                                                        Dim strColumn = "INSERT INTO MMM_MST_DOC_ITEM(isauth,DOCID,documenttype"
                                                        Dim strValue = "VALUES (1," & FileID & ",'" & Cline.DocumentType & "'"
                                                        StrQry1 = GenerateQueryForChild(strColumn, strValue, obj)

                                                        StrQry1 = StrQry1 & ";Select @@identity"

                                                        'If StrQuery = "" Then
                                                        '    StrQuery = StrQry1
                                                        'Else
                                                        '    StrQuery = StrQuery & ";" & StrQry1
                                                        'End If

                                                        'Next
                                                        'da = New SqlDataAdapter(StrQuery, con)
                                                        'da.SelectCommand.CommandText = StrQuery  '' commmented by sumil on 13_jun_19
                                                        da.SelectCommand.CommandText = StrQry1
                                                        da.SelectCommand.CommandType = CommandType.Text
                                                        'If con.State <> ConnectionState.Open Then
                                                        '    con.Open()
                                                        'End If
                                                        Dim ChildTID As Integer = da.SelectCommand.ExecuteScalar()

                                                        'Chandni code starts from here for Lease master setting check
                                                        Try
                                                            Dim RentaltoolObj As New Rentaltool()
                                                            Dim LMres As String = ""
                                                            If EID = 181 Then
                                                                RentaltoolObj.CheckLMSettingsChildInsertion(EID, DocType, UID, ChildTID, con, trans)

                                                            End If

                                                        Catch ex As Exception

                                                        End Try
                                                        'Chandni code ends here for Lease master setting check



                                                        '''' new added by Sunil Pareek - for generting auto number in child item - 06-Apr-19
                                                        Dim objDT As New DataTable()
                                                        ' If dtField.Rows.Count > 0 Then
                                                        objDT = objDC.TranExecuteQryDT("select * from mmm_mst_fields with(nolock) where documenttype='" & Cline.DocumentType & "' and eid=" & EID & " and fieldtype='Auto Number'", con:=con, tran:=trans)
                                                        'Dim rowAutoNumber As DataRow() = dtField.Select("Fieldtype='Auto Number' or Fieldtype='New Auto Number'")
                                                        Dim ht As New Hashtable()
                                                        For i As Integer = 0 To objDT.Rows.Count - 1
                                                            'Dim chldDT As New DataTable()
                                                            'chldDT = objDC.TranExecuteQryDT("select tid from mmm_mst_doc_item with(nolock) where docid=" & docid, con:=con, tran:=trans)
                                                            'For Each drChildDT As DataRow In chldDT.Rows
                                                            ht.Clear()
                                                            ht.Add("Fldid", objDT.Rows(i).Item("fieldid"))
                                                            ht.Add("docid", ChildTID)
                                                            ht.Add("fldmapping", objDT.Rows(i).Item("fieldmapping"))
                                                            ht.Add("FormType", "DOCDETAIL")
                                                            objDC.TranExecuteProDT(sp:="usp_GetAutoNoNew", ht:=ht, con:=con, tran:=trans)
                                                            'Next
                                                        Next
                                                        '''' new added by Sunil Pareek - for generting auto number in child item - 06-Apr-19


                                                    End If

                                                Next
                                                'Check condition if unique id does not matched with current data
                                                Dim objDTChild As New DataTable()
                                                objDTChild = objDC.TranExecuteQryDT("select UpdateStatus,ActionField,targetfield from mmm_mst_forms with(nolock) where formname='" & Cline.DocumentType & "' and eid=" & EID & "", con:=con, tran:=trans)
                                                For Each dr As DataRow In objDTChild.Rows
                                                    If (Convert.ToString(dr("UpdateStatus")).Trim().ToUpper() = "ACTION") Then
                                                        For Each objlist In objuniqueCollectionList.uniqueCollectionData
                                                            If (objlist.IsFound = False) Then
                                                                Dim objCounter As Integer = 0
                                                                If (Convert.ToString(dr("ActionField")).Length > 0) Then
                                                                    objCounter = Convert.ToInt32(objDC.TranExecuteQryScaller("select count(*) from mmm_mst_doc_item with(nolock) where documenttype='" & Cline.DocumentType & "' and docid=" & FileID & " AND " & Convert.ToString(dr("ActionField")) & " AND " & objlist.fieldMapping & "='" & objlist.fieldValue & "' ", con:=con, tran:=trans))
                                                                    If (objCounter = 1) Then
                                                                        If (Convert.ToString(dr("targetfield")).Length > 0) Then
                                                                            objDC.TranExecuteQryScaller("Update mmm_mst_doc_item set " & Convert.ToString(dr("targetfield")) & " where documenttype='" & Cline.DocumentType & "' and docid=" & FileID & " AND " & Convert.ToString(dr("ActionField")) & " AND " & objlist.fieldMapping & "='" & objlist.fieldValue & "' ", con:=con, tran:=trans)
                                                                        Else
                                                                            trans.Rollback()
                                                                            strResult = "Please configure Target field Data"
                                                                            Return strResult

                                                                        End If

                                                                    Else
                                                                        trans.Rollback()
                                                                        strResult = "Updated field data " & objlist.fieldValue & " does not match with database "
                                                                        Return strResult
                                                                    End If
                                                                Else
                                                                    trans.Rollback()
                                                                    strResult = "Please configure actionfield data"
                                                                    Return strResult
                                                                End If
                                                            End If
                                                        Next
                                                    End If
                                                Next

                                            Else
                                                trans.Rollback()
                                                strResult = "There were no configuration setting for update, Kindly make unique key for update"
                                                Return strResult

                                                'Commented just because change the logic'

                                            End If


                                            Dim ObjForm = New DataWraper1()
                                            ObjForm = line
                                            'Code For executing formula for child item
                                            If (FileID > 0) And (ObjForm.DocumentType <> "") Then
                                                Dim FormulaQ = "select * FROM MMM_MST_FIELDS WHERE EID=" & EID & " AND DocumentType='" & line.DocumentType & "' AND FieldType='Formula Field' order by displayorder"
                                                Dim DSF As New DataSet()
                                                'da = New SqlDataAdapter(FormulaQ, con)
                                                da.SelectCommand.CommandText = FormulaQ
                                                da.SelectCommand.CommandType = CommandType.Text
                                                da.Fill(DSF)
                                                'Transactional Query
                                                '' change in below trigger call - instead of create use Update by sunil 
                                                Trigger.ExecuteTriggerT(Cline.DocumentType, EID, FileID, con, trans, 1, TriggerNature:="UPDATE")
                                                'Trigger.ExecuteTriggerT(ObjForm.DocumentType, EID, FileID, con, tran, 1, TriggerNature:="Create")
                                            End If
                                        Next
                                        objRel.ExecutePeriodWiseBalance(EID, FileID, MainDocType, con, trans)
                                    End If
                                    trans.Commit()

                                    If FileID > 0 Then
                                        Try
                                            Dim ws As New WSOutward()
                                            Dim URLIST As String = ws.WBS(line.DocumentType, EID, FileID, "Edit")
                                            If UCase(URLIST).Contains("IS NOT AVAILABLE") Or UCase(URLIST).Contains("NOT VALID") Then
                                                URLIST = ws.WBS(line.DocumentType, EID, FileID)
                                            End If
                                        Catch ex As Exception
                                        End Try
                                        ' below code for webservice report - 20_june_14 ravi
                                        Dim da1 As SqlDataAdapter = New SqlDataAdapter("select * from MMM_Print_Template where documenttype='" & DocType & "' and draft='Service'", con)
                                        Dim ds1 As New DataSet
                                        da1.Fill(ds1, "dataset")
                                        Try
                                            For k As Integer = 0 To ds1.Tables("dataset").Rows.Count - 1
                                                If ds1.Tables("dataset").Rows(k).Item("SendType").ToString() = "WS" Then
                                                    Dim ws As New WSOutward()
                                                    Dim URLIST As String = ws.WBSREPORT(line.DocumentType, EID, FileID, "Edit")
                                                End If
                                            Next
                                        Catch ex As Exception
                                        Finally
                                            da1.Dispose()
                                            ds1.Dispose()
                                        End Try
                                        'Calling for sending mails 
                                        Try
                                            Dim ob1 As New DMSUtil
                                            ob1.TemplateCalling(FileID, EID, line.DocumentType, "CREATED")
                                            If EID = 42 And line.DocumentType.Trim.ToUpper = "QUOTATION DOMESTIC MOVEMENT" Then
                                                ob1.TemplateCalling(FileID, EID, line.DocumentType, "APMQDM")
                                            End If
                                        Catch ex As Exception
                                        End Try

                                    End If

                                    strResult = "Record updated successfully."
                                Else
                                    strResult = "RTO" & " Error while updating record!"
                                End If
                            Else
                                strResult = "RTO" & " Error in SetHistory, History update failed!"
                            End If
                        Catch ex As Exception
                            If Not trans Is Nothing Then
                                trans.Rollback()
                                strResult = "RTO " & Regex.Replace(ex.InnerException.Message.ToString, "[""']", String.Empty)
                            End If
                        End Try

                    Else
                        strResult = "No matching record found  based on supplyed key."
                    End If
                Next
            End If
        Catch ex As Exception
            ErrorLog.sendMail("UpdateData.Update", ex.Message)
            Return "RTO " & Regex.Replace(ex.InnerException.Message.ToString, "[""']", String.Empty)
        Finally
            If Not con Is Nothing Then
                con.Close()
                con.Dispose()
            End If
            If Not da Is Nothing Then
                da.Dispose()
            End If
        End Try
        Return strResult
    End Function

    Public Class UniqueCollectionList
        Public uniqueCollectionData As List(Of UniqueCollection)
    End Class

    Public Class UniqueCollection
        Public fieldValue As String
        Public IsFound As Boolean
        Public fieldiID As Integer
        Public fieldMapping As String

    End Class

    Public Shared Function GenerateQueryForChild(strColumn As String, strValue As String, objData As LineitemWrap) As String
        Dim StrQuery = ""
        Try
            Dim value = ""
            For Each Data In objData.DataItem
                strColumn = strColumn & "," & Data.FieldMapping
                value = Data.Values.Replace("'", "''")
                If Data.DisplayName.ToUpper = "ISAUTH" Then
                    If value <> "0" Then
                        value = 1
                    End If
                End If
                strValue = strValue & ",'" & value & "'"
            Next
            strColumn = strColumn & ")"
            strValue = strValue & ")"
            StrQuery = strColumn & strValue
        Catch ex As Exception
            ErrorLog.sendMail("GenerateQuery", ex.Message)
            Return "RTO " & Regex.Replace(ex.InnerException.Message.ToString, "[""']", String.Empty)
        End Try

        Return StrQuery
    End Function


    Public Shared Function GenerateQuery(StrQuery As String, objData As LineitemWrap, Optional EditType As String = "EnableEdit") As String
        Try
            Dim value = ""
            For Each Data In objData.DataItem
                If EditType = "EnableEdit" Then
                    If Data.EnableEdit.ToString = "1" Then
                        value = Data.Values.Replace("'", "''")
                        'IF Any Value Other than 0 it should be 1
                        If Data.DisplayName.ToUpper = "ISAUTH" Then
                            If value <> "0" Then
                                value = 1
                            End If
                        End If
                        'IF ISAuth is not Supplyed it should not be updated
                        If Data.DisplayName.ToUpper = "ISAUTH" And Data.Values.Trim = "" Then
                            Continue For
                        End If
                        StrQuery = StrQuery & "," & Data.FieldMapping & " = '" & value & "'"
                    End If
                Else
                    If Data.IsEditOnamend.ToString = "1" Then
                        value = Data.Values.Replace("'", "''")
                        'IF Any Value Other than 0 it should be 1
                        If Data.DisplayName.ToUpper = "ISAUTH" Then
                            If value <> "0" Then
                                value = 1
                            End If
                        End If
                        'IF ISAuth is not Supplyed it should not be updated
                        If Data.DisplayName.ToUpper = "ISAUTH" And Data.Values.Trim = "" Then
                            Continue For
                        End If
                        StrQuery = StrQuery & "," & Data.FieldMapping & " = '" & value & "'"
                    End If
                End If
            Next
        Catch ex As Exception
            Throw
        End Try
        Return StrQuery
    End Function

    Public Function ValidateKeys(EID As Integer, DocumentType As String, Data As String, ByRef Keys As String) As Boolean
        Dim ret As Boolean = False
        Dim ds As New DataSet()
        Dim IskeySupplyed As Boolean = False
        Dim ExChars As String = ""
        Dim StrUSerunqQuery = ""
        Dim ObjD = New DynamicForm()
        Try
            'Code for unique key check if DocumentType is of UserType
            If DocumentType.ToUpper = "USER" Then
                StrUSerunqQuery = "SELECT *,''''',--' AS 'ExChars' FROM MMM_MST_FIELDS WHERE EID=" & EID & " AND DocumentType='USER' AND FieldMapping=(SELECT LoginField FROM MMM_MST_ENTITY WHERE EID=" & EID & ")"
                Using con = New SqlConnection(conStr)
                    Using da = New SqlDataAdapter(StrUSerunqQuery, con)
                        da.Fill(ds)
                    End Using
                End Using
            Else
                ds = GetKeys(EID, DocumentType)
            End If
            If ds.Tables(0).Rows.Count > 0 Then
                ExChars = Convert.ToString(ds.Tables(0).Rows(0).Item("ExChars"))
                'loop through 
                Dim arrData = Data.Split("|")
                If arrData.Length > 0 Then
                    For j As Integer = 0 To ds.Tables(0).Rows.Count - 1
                        IskeySupplyed = False
                        For i As Integer = 0 To arrData.Length - 1
                            Dim arr1 = Split(arrData(i), "::")
                            If ds.Tables(0).Rows(j).Item("DisplayName").Trim.ToUpper = arr1(0).ToString.ToUpper.Trim Then
                                Dim Values = arr1(1)
                                Dim DDLR = ds.Tables(0).Rows(j)
                                Dim Fieldtype = ds.Tables(0).Rows(j).Item("FieldType")
                                Dim ddlType = ds.Tables(0).Rows(j).Item("DropDownType")
                                If (Fieldtype.ToString.Trim = "AutoComplete" Or Fieldtype.ToString.Trim = "Drop Down") And (ddlType = "MASTER VALUED" Or ddlType = "SESSION VALUED") Then
                                    Values = DropDown.GetDropDownID(DDLR, EID, Values)
                                End If
                                Dim dataType = ds.Tables(0).Rows(j).Item("datatype")
                                If dataType.ToString.Trim = "Datetime" Then
                                    Try
                                        Dim dt As New Date()
                                        dt = CommanUtil.GetFDate(Values)
                                        Values = dt.ToString("dd/MM/yy", CultureInfo.InvariantCulture)
                                    Catch ex As Exception
                                        Keys = "Invalid date formate supplyed at " & ds.Tables(0).Rows(j).Item("DisplayName").Trim
                                        Return False
                                    End Try
                                End If
                                'GetDropDownID(DRDDL As DataRow, EID As Integer, DDlText As String, obj As LineitemWrap)
                                'Code For Removing Extra Charactor 
                                If Not String.IsNullOrEmpty(ExChars.Trim) Then
                                    Values = Removekeys(Values, ExChars).Trim()
                                End If
                                Keys = Keys & " AND " & ObjD.GenerateReplaceStatement(ds.Tables(0).Rows(j).Item("FieldMapping"), ExChars) & "='" & Values.Trim() & "'"
                                IskeySupplyed = True
                                Exit For
                            End If
                        Next
                    Next
                    If IskeySupplyed = False Then
                        Keys = "Insufficient keys supplyed."
                    End If
                Else
                    Keys = "Insufficient parameter."
                End If
            Else
                ret = False
                'This will be returned in case of document without key configuration
                Keys = "Your request can not be completed.Please contact admin to get it resolved."
            End If
            If IskeySupplyed = True Then
                ret = True
            Else
                ret = False
            End If
        Catch ex As Exception
            Throw
        End Try
        Return ret.ToString.Trim()
    End Function

    Public Function Removekeys(StrKey As String, rmvStr As String) As String
        Dim ret = ""
        Dim arr = rmvStr.Split(",")
        Dim arr1 = From Spliter In arr Where Spliter <> "" Select Spliter
        For Each item In arr1
            If StrKey.Contains(item) Then
                StrKey = StrKey.Replace(item, String.Empty)
            End If
        Next
        Return StrKey
    End Function

    Public Function GetKeys(EID As Integer, DocumentType As String) As DataSet
        Dim ret As String = ""
        Dim ds As New DataSet()
        Try
            'TO avoid Sql Injection "--" Is used to comment script 
            DocumentType = DocumentType.Replace("--", String.Empty)
            Using con As New SqlConnection(conStr)
                Using da As New SqlDataAdapter("GetKeys00000", con)
                    da.SelectCommand.CommandType = CommandType.StoredProcedure
                    da.SelectCommand.Parameters.AddWithValue("@EID", EID)
                    da.SelectCommand.Parameters.AddWithValue("@DocumentType", DocumentType)
                    If con.State <> ConnectionState.Open Then
                        con.Open()
                    End If
                    da.Fill(ds)
                End Using
            End Using
            'If ds.Tables(0).Rows.Count > 0 Then
            '    ret = Convert.ToString(ds.Tables(0).Rows(0).Item("keys"))
            'End If
        Catch ex As Exception
            Throw
        End Try
        Return ds
    End Function


    Public Function GetDocDetails(DocumentType As String, EID As String, tid As Integer) As DataSet
        Dim ds As New DataSet()
        Dim dsD As New DataSet()
        Dim IskeySupplyed As Boolean = False
        Try
            'ValidateKeys(EID As Integer, DocumentType As String, Data As String, ByRef Keys As String)
            Dim objRel As New Relation()
            ds = objRel.GetAllFields(EID)
            Dim StrQuery = objRel.GenearateQuery1(EID, DocumentType, ds, 1)
            'StrQuery = StrQuery.Replace(DocumentType & ".", "")
            Dim ViewName As String = ""
            ViewName = "V" & EID & DocumentType.Trim.Replace(" ", "_")
            StrQuery = "SELECT  " & StrQuery & " AND " & ViewName & ".tid = " & tid
            Using con = New SqlConnection(conStr)
                Using da = New SqlDataAdapter(StrQuery, con)
                    da.Fill(dsD, "tbldata")
                End Using
            End Using
        Catch ex As Exception
            Throw
        End Try
        Return dsD
    End Function

    Public Function CancelDocument(DocumentType As String, EID As Integer, DOCID As Integer, UID As Integer) As String
        Dim con As SqlConnection = Nothing
        Dim cmd As SqlCommand = Nothing
        Dim trans As SqlTransaction = Nothing
        Dim ret As String = "Cancellation Failed."
        Try
            'Setting History
            Dim HisReport = CommanUtil.SetHistory(EID, DOCID, "MMM_MST_DOC", UID, "Cancle WS", DocumentType)
            con = New SqlConnection(conStr)
            con.Open()
            trans = con.BeginTransaction()
            'Logic for Canceling Period wise balance
            Dim objRel As New Relation()
            objRel.CanclePeriodWiseBalance(EID, DOCID, DocumentType, con, trans)
            Dim Query As String = "UPDATE MMM_MST_DOC SET IsAuth='0' WHERE EID= @EID  AND DocumentType=@DocType AND IsAuth=1 AND tid=@DOCID"
            cmd = New SqlCommand(Query)
            cmd.Transaction = trans
            cmd.Parameters.AddWithValue("@EID", EID)
            cmd.Parameters.AddWithValue("@DocType", DocumentType)
            cmd.Parameters.AddWithValue("@DOCID", DOCID)
            cmd.Connection = con
            Dim RowEffected As Integer = 0
            RowEffected = cmd.ExecuteNonQuery()
            If (RowEffected > 0) Then
                ret = "Document canceled successfully."
                trans.Commit()
            Else
                trans.Rollback()
                ret = "Cancellation Failed!!!You can not cancel canceled document."
            End If
        Catch ex As Exception
            If Not con Is Nothing Then
                trans.Rollback()
            End If
            Return ret
        Finally
            con.Close()
            con.Dispose()
        End Try

        Return ret
    End Function

    Public Function GetDOCID(EID As Integer, ByRef Keys As String, DocumentType As String, Data As String) As Boolean

        Dim ret As Boolean = False
        Dim ds As New DataSet()
        Dim IskeySupplyed As Boolean = False
        Dim ExChars As String = ""
        Dim StrUSerunqQuery = ""
        Dim ObjD = New DynamicForm()
        Try
            'Code for unique key check if DocumentType is of UserType
            If DocumentType.ToUpper = "USER" Then
                StrUSerunqQuery = "SELECT *,''''',--' AS 'ExChars' FROM MMM_MST_FIELDS WHERE EID=" & EID & " AND DocumentType='USER' AND FieldMapping=(SELECT LoginField FROM MMM_MST_ENTITY WHERE EID=" & EID & ")"
                Using con = New SqlConnection(conStr)
                    Using da = New SqlDataAdapter(StrUSerunqQuery, con)
                        da.Fill(ds)
                    End Using
                End Using
            Else
                ds = GetKeys(EID, DocumentType)
            End If
            If ds.Tables(0).Rows.Count > 0 Then
                ExChars = Convert.ToString(ds.Tables(0).Rows(0).Item("ExChars"))
                'loop through 
                Dim arrData = Data.Split("|")
                If arrData.Length > 0 Then
                    For j As Integer = 0 To ds.Tables(0).Rows.Count - 1
                        IskeySupplyed = False
                        For i As Integer = 0 To arrData.Length - 1
                            Dim arr1 = Split(arrData(i), "::")
                            If ds.Tables(0).Rows(j).Item("DisplayName").Trim.ToUpper = arr1(0).ToString.ToUpper.Trim Then
                                Dim Values = arr1(1)
                                Dim DDLR = ds.Tables(0).Rows(j)
                                Dim Fieldtype = ds.Tables(0).Rows(j).Item("FieldType")
                                Dim ddlType = ds.Tables(0).Rows(j).Item("DropDownType")
                                Dim dataType = ds.Tables(0).Rows(j).Item("datatype")
                                If (Fieldtype.ToString.Trim = "AutoComplete" Or Fieldtype.ToString.Trim = "Drop Down") And (ddlType = "MASTER VALUED" Or ddlType = "SESSION VALUED") Then
                                    Values = DropDown.GetDropDownID(DDLR, EID, Values)
                                End If
                                'Code For Removing Extra Charactor 
                                If Not String.IsNullOrEmpty(ExChars.Trim) Then
                                    Values = Removekeys(Values, ExChars).Trim()
                                End If

                                If dataType.ToString.Trim = "Datetime" Then
                                    Try
                                        Dim dt As New Date()
                                        dt = CommanUtil.GetFDate(Values)
                                        Values = dt.ToString("dd/MM/yy", CultureInfo.InvariantCulture)
                                    Catch ex As Exception
                                        Keys = "Invalid date formate supplyed at " & ds.Tables(0).Rows(j).Item("DisplayName").Trim
                                        Return False
                                    End Try
                                End If

                                Keys = Keys & " AND [" & ObjD.GenerateReplaceStatement(ds.Tables(0).Rows(j).Item("DisplayName"), ExChars) & "]='" & Values.Trim() & "'"
                                IskeySupplyed = True
                                Exit For
                            End If
                        Next
                    Next
                    If IskeySupplyed = False Then
                        Keys = "Insufficient keys supplyed."
                    Else
                        Dim Query = "SELECT tid FROM " & "[v" & EID & DocumentType.Trim.Replace(" ", "_") & "]" & "WHERE EID=" & EID & Keys
                        Dim dsDOC As New DataSet()
                        Using con = New SqlConnection(conStr)
                            Using da = New SqlDataAdapter(Query, con)
                                da.Fill(dsDOC)
                            End Using
                        End Using
                        If dsDOC.Tables(0).Rows.Count > 0 Then
                            Keys = dsDOC.Tables(0).Rows(0).Item("tid")
                        Else
                            ret = False
                            IskeySupplyed = False
                            Keys = "No matching record found based on supplyed key."
                        End If
                    End If
                Else
                    Keys = "Insufficient parameter."
                End If
            Else
                ret = False
                'This will be returned in case of document without key configuration
                Keys = "Your request can not be completed.Please contact admin to get it resolved."
            End If
            If IskeySupplyed = True Then
                ret = True
            Else
                ret = False
            End If
        Catch ex As Exception
            Throw
        End Try
        Return ret.ToString.Trim()
    End Function



End Class







