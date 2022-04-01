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

Public Class CommanUtil

    Public Shared Function AuthanticateUser(UserID As String, Password As String, ECode As String) As Userdetails
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim ret As String = "0"
        Dim Result As String = "Fail"
        Dim con As SqlConnection = Nothing
        Dim oda As SqlDataAdapter = Nothing
        Dim uObj As New User()
        Dim ds As New DataSet
        Dim objU As New Userdetails()
        Dim enitity As Integer = 0
        Try
            con = New SqlConnection(conStr)
            Dim sqlq As String
            sqlq = "SELECT u.pwd [pwd],u.isauth [uisauth],u.sKey,minPassAttempt,passtry,passExpDays,passExpMsgDays,autoUnlockHour,datediff(hour,ModifyDate,getdate()) [hourElapsed],locationID,E.EID,U.islogin FROM MMM_MST_USER U left outer join MMM_MST_ENTITY E on u.EID=E.EID where U.userid='" & UserID & "' and E.CODE='" & ECode & "'"
            oda = New SqlDataAdapter(sqlq, con)
            oda.Fill(ds, "user")
            If ds.Tables("user").Rows.Count Then
                If Convert.ToInt32(ds.Tables("user").Rows(0).Item("islogin")) = 1 Then
                    objU.UID = 1
                Else
                    enitity = Val(ds.Tables("user").Rows(0).Item("EID").ToString())
                    Dim sKey As String = ds.Tables("user").Rows(0).Item("sKey").ToString()
                    Dim sPwd As String = uObj.DecryptTripleDES(ds.Tables("user").Rows(0).Item("pwd"), sKey)
                    If sPwd = Password Then
                        If Val(ds.Tables("user").Rows(0).Item("minPassAttempt").ToString()) <= Val(ds.Tables("user").Rows(0).Item("passtry").ToString()) Then
                            'password retry reached to limit set isauth to lock mode and exit sub
                            ret = 0
                        End If
                        Select Case Val(ds.Tables("user").Rows(0).Item("uisauth").ToString())
                            Case 1
                                If Val(ds.Tables("user").Rows(0).Item("passExpMsgDays").ToString()) * 24 <= Val(ds.Tables("user").Rows(0).Item("hourElapsed").ToString()) Then
                                    'Send him a message to change the password
                                    ret = 1
                                End If
                                If Val(ds.Tables("user").Rows(0).Item("passExpDays").ToString()) * 24 <= Val(ds.Tables("user").Rows(0).Item("hourElapsed").ToString()) Then
                                    ret = 0
                                    'Lock the password and send message password expired
                                Else
                                    ret = 1
                                End If
                            Case 3
                                If Val(ds.Tables("user").Rows(0).Item("hourElapsed").ToString()) >= Val(ds.Tables("user").Rows(0).Item("autoUnlockHour").ToString()) Then
                                    'Unlock and let him login - change the isauth to 1
                                    ret = 1
                                Else
                                    ret = 0
                                End If
                            Case 2
                                ret = 0
                            Case Else
                                ret = 0
                        End Select
                    Else
                        ret = 0
                    End If
                End If

            End If
            If ret = 1 Then
                Call getValueByEmail(UserID, enitity, objU)
                objU.BPMKEY = ConfigurationManager.AppSettings("BPMKEY").ToString()
            End If
        Catch ex As Exception
            Return objU
        Finally
            If Not con Is Nothing Then
                con.Close()
                con.Dispose()
            End If
            If Not oda Is Nothing Then
                oda.Dispose()
            End If
        End Try
        Return objU
    End Function

    Public Shared Sub getValueByEmail(ByVal UN As String, ByVal entity As Integer, ByRef Udetails As Userdetails)
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("SELECT uid,username,emailid,U.pwd [pwd],U.eid [EID],userrole,U.isauth [EmpStatus],E.isauth [entityStatus],E.Code,E.Headerstrip ,Imagefile,isnull(logo,'mynd.gif') [logo],footer,domainname,localfilesystem,locationID,T.indVal FROM MMM_MST_USER U LEFT OUTER JOIN MMM_MST_ENTITY E on E.eid=U.EID left outer join MMM_MST_LOCATION L on U.locationID=L.locID left outer join MMM_MST_TIMEZONE T on L.ZoneID=T.ZoneID where U.userid='" & UN & "' and U.EID=" & entity & "", con)
        Try
            Dim ds As New DataSet
            oda.Fill(ds, "user")
            If ds.Tables("user").Rows.Count = 1 Then
                'User Is Found now Initilized all values
                Udetails.EmailID = UN
                Udetails.UID = ds.Tables("user").Rows(0).Item("UID")
                Udetails.EmailID = ds.Tables("user").Rows(0).Item("emailid").ToString()
                Udetails.UserName = ds.Tables("user").Rows(0).Item("Username")
                Udetails.UserRoles = ds.Tables("user").Rows(0).Item("userrole").ToString()
                Udetails.EID = ds.Tables("user").Rows(0).Item("EID").ToString()
                'strCode = ds.Tables("user").Rows(0).Item("Code").ToString()
                ''fetch role from role definition
                Dim str As String = ""
                str &= "select distinct rolename [userrole] from MMM_REF_ROLE_USER where eid=" & Udetails.EID & " AND UID=" & Udetails.UID & " "
                oda.SelectCommand.CommandText = str
                oda.Fill(ds, "Roles")
                Dim Roles As String = ""
                For Each dr As DataRow In ds.Tables("Roles").Rows
                    If dr.Item("userrole").ToString.ToUpper <> Udetails.UserRoles.ToUpper() Then
                        Roles &= dr.Item("userrole").ToString() & ","
                    End If
                Next
                Roles &= Udetails.UserRoles.ToUpper()
                Udetails.Roles = Roles
                Dim objDC As New DataClass()
                objDC.ExecuteNonQuery(" update mmm_mst_user set islogin=1 where UID=" & Udetails.UID)
            End If
        Catch ex As Exception
        Finally
            con.Close()
            oda.Dispose()
            con.Dispose()
        End Try
    End Sub

    Public Shared Function GetDropDownData(EID As Integer, Doctype As String, FieldName As String, TableName As String, Optional Filter As String = "") As DataSet
        Dim ds As New DataSet()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim strQuery As String = ""
        Dim con As SqlConnection = Nothing
        Dim sda As SqlDataAdapter = Nothing
        Try
            If (TableName.ToUpper() = "MMM_MST_USER") Then
                strQuery = "SELECT " & FieldName & " FROM " & TableName & " WHERE EID=" & EID
            Else
                strQuery = "SELECT TID," & FieldName & " FROM " & TableName & " WHERE EID=" & EID & " AND DocumentType='" & Doctype & "'"

            End If

            If Filter.Trim() <> "" Then
                strQuery = strQuery & Filter
            End If
            strQuery = strQuery & "and isAuth=1"
            con = New SqlConnection(conStr)
            con.Open()
            sda = New SqlDataAdapter(strQuery, con)
            sda.Fill(ds)
        Catch ex As Exception
            Throw
        Finally
            If Not con Is Nothing Then
                con.Close()
                con.Dispose()
            End If
            If Not con Is Nothing Then
                sda.Dispose()
            End If
        End Try
        Return ds
    End Function

    Public Shared Function GetFormFields(EID As Integer, Optional Doctype As String = "", Optional FieldID As Integer = 0) As DataSet
        Dim dsFields As New DataSet()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim strQuery = ""
        Dim con As SqlConnection = Nothing
        Dim sda As SqlDataAdapter = Nothing
        Try
            strQuery = "SELECT FF.*,F.*  FROM MMM_MST_FIELDS FF left outer JOIN MMM_MST_FORMS F on F.FormName = FF.DocumentType and F.EID = FF.EID   where  F.EID=" & EID & " "
            If FieldID <> 0 Then
                strQuery = strQuery & " AND FF.FieldID= " & FieldID
            Else
                strQuery = strQuery & " AND FF.FieldID= " & FieldID & "AND FF.isactive=1 "
            End If
            If Doctype <> "" Then
                strQuery = strQuery & "and FormName = '" & Doctype & "'"
            End If
            strQuery = strQuery & "  order by displayOrder"
            con = New SqlConnection(conStr)
            con.Open()
            sda = New SqlDataAdapter(strQuery, con)
            sda.Fill(dsFields, "fields")
        Catch ex As Exception
            Throw
        Finally
            If Not con Is Nothing Then
                con.Close()
                con.Dispose()
            End If
            If Not con Is Nothing Then
                sda.Dispose()
            End If
        End Try
        Return dsFields
    End Function

    Public Shared Function ValidateParameterByDocumentType(EID As Integer, DocType As String, UID As Integer, Data As String, Optional ByRef lineitem1 As LineitemWrap = Nothing, Optional ByRef ChldFlag As Boolean = False, Optional isChild As Boolean = False, Optional isActionForm As Boolean = False, Optional ByRef fileUploadPath As ArrayList = Nothing, Optional ByRef prefixFilePath As String = "") As String
        Dim result As String = ""
        Dim con As SqlConnection = Nothing
        Dim da As SqlDataAdapter = Nothing
        Try

            Dim ds As New DataSet()
            Dim LStDataFinal As New List(Of DataWraper1)
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            con = New SqlConnection(conStr)
            da = New SqlDataAdapter("getDataOfForm100", con)
            da.SelectCommand.CommandType = CommandType.StoredProcedure
            da.SelectCommand.Parameters.AddWithValue("@EID", EID)
            da.SelectCommand.Parameters.AddWithValue("@FormName", DocType)
            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            da.Fill(ds)
            Dim onlyFiltered As DataView = ds.Tables(0).DefaultView
            'Note: in the case of Action form "IsActive" do not play any role. 
            If isActionForm = True Then
                onlyFiltered.RowFilter = "DocumentType='" & DocType & "' AND FieldType <> 'Formula Field'  AND invisible=0"
            Else
                onlyFiltered.RowFilter = "DocumentType='" & DocType & "' AND FieldType <> 'Formula Field' AND Isactive=1"
            End If
            Dim theFlds As DataTable = onlyFiltered.Table.DefaultView.ToTable()

            'Since user is a special type of document that's why adding some addition fields

            If DocType.ToUpper() = "USER" Then
                'Adding rows for static 
                AddUserStaticField(theFlds)
                'MASTER VALUED
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

                        End If
                        'If IsParameterSupp = False Then
                        '    Exit For
                        'End If
                    Next
                    'If IsParameterSupp = False Then
                    '    result = "Insufficient Parameter"
                    '    Exit For
                    'End If
                Next
                'Setting Form Data into object
                objDW.DocumentType = DocType
                objDW.Data = StrFormData
                Dim UDataC As UserData
                Dim objF As DataWraper1
                Dim objC As DataWraper1
                Dim ObjItem As LineitemWrap
                Dim LStDataItem As List(Of UserData)
                Dim LstlineItem As List(Of LineitemWrap)
                For Each objData In LstData
                    If DocType = objData.DocumentType Then
                        objF = New DataWraper1()
                        LStDataItem = New List(Of UserData)

                        If isActionForm = True Then
                            onlyFiltered.RowFilter = "DocumentType='" & objData.DocumentType & "' AND FieldType <> 'Formula Field' AND FieldType <> 'Child Item' AND invisible=0"
                        Else
                            onlyFiltered.RowFilter = "DocumentType='" & objData.DocumentType & "' AND FieldType <> 'Formula Field'  AND IsActive=1 AND FieldType <> 'Child Item'"
                        End If
                        theFlds = onlyFiltered.Table.DefaultView.ToTable()
                        'getting one row data
                        ObjItem = New LineitemWrap()
                        LstlineItem = New List(Of LineitemWrap)
                        ObjItem = CreateListCollection(objData.Data, theFlds, EID, DocType)
                        LstlineItem.Add(ObjItem)
                        'LstlineItem.Add(LStDataItem)
                        objF.LineItem = LstlineItem
                        objF.DocumentType = DocType
                        objF.FormType = objData.FormType
                        'objF.LineItem = LStDataItem
                        LStDataFinal.Add(objF)
                    Else
                        Dim arr As String() = Split(objData.Data, "{}")
                        UDataC = New UserData()

                        If isActionForm = True Then
                            onlyFiltered.RowFilter = "DocumentType='" & objData.DocumentType & "' AND FieldType <> 'Formula Field' AND FieldType <> 'Child Item'  AND invisible=0"
                        Else
                            onlyFiltered.RowFilter = "DocumentType='" & objData.DocumentType & "' AND FieldType <> 'Formula Field' AND IsActive=1 AND FieldType <> 'Child Item'"
                        End If

                        theFlds = onlyFiltered.Table.DefaultView.ToTable()
                        ObjItem = New LineitemWrap()
                        LstlineItem = New List(Of LineitemWrap)
                        objC = New DataWraper1()
                        For d As Integer = 0 To arr.Count - 1
                            objC.DocumentType = objData.DocumentType
                            objC.FormType = objData.FormType
                            If arr(d).Trim <> "" Then
                                UDataC = New UserData()
                                ObjItem = CreateListCollection(arr(d), theFlds, EID, objData.DocumentType)
                                LstlineItem.Add(ObjItem)
                            End If
                        Next
                        objC.LineItem = LstlineItem
                        LStDataFinal.Add(objC)
                    End If
                Next

                'Fill Drop Down And lookup values
                FillData(LStDataFinal, ds, EID, DocType, UID, fileUploader:=fileUploadPath, prefixFilePath:=prefixFilePath)
                'Validate Form
                Dim FormData As New DataWraper1()
                Dim lineitem As LineitemWrap
                'Navigating all the form
                Dim ErrMsg = "", AllError = ""
                Dim IsAllFormValid As Boolean = True
                Dim Flag As Boolean = True
                Dim lstData1 As New List(Of UserData)
                For Each Form In LStDataFinal
                    FormData = Form
                    Dim DocumentType = FormData.DocumentType
                    Dim LstLine = FormData.LineItem
                    Dim ErrorLine As Integer = 1
                    For Each Row In LstLine
                        ErrMsg = "Error(s) in document " & DocumentType & ".| " & ErrorLine & "|"
                        ErrorLine = ErrorLine + 1
                        lineitem = New LineitemWrap()
                        lineitem = Row
                        If Not lineitem1 Is Nothing Then
                            lineitem1 = Row
                        End If
                        Flag = ValidateForm(EID, lineitem, Form.FormType, DocumentType, ErrMsg)
                        Dim Keys As String = ""
                        'Code For composit key validation
                        If Flag = False Then
                            AllError = ErrMsg
                            Exit For
                        End If
                        'Code For Rule Engine

                        Dim ObjRet As New RuleResponse()
                        Dim objRule As New RuleEngin(EID, Form.DocumentType, "CREATED", "SUBMIT")
                        Dim dsrule As DataSet = objRule.GetRules()
                        Dim dtrule As New DataTable
                        dtrule = dsrule.Tables(1)
                        For Each dr As DataRow In dsrule.Tables(0).Rows
                            If Form.DocumentType.ToUpper.Trim = DocType.ToUpper.Trim Then
                                lstData1 = lineitem.DataItem
                                ObjRet = objRule.ExecuteRule(lineitem.DataItem, Nothing, False, "", dr, dtrule)
                            Else
                                ObjRet = objRule.ExecuteRule(lstData1, lineitem.DataItem, True, "", dr, dtrule)
                            End If
                            If ObjRet.Success = False Then
                                Flag = ObjRet.Success
                                AllError &= ObjRet.ErrorMessage
                            End If
                        Next
                        If Flag = False Then
                            Exit For
                        End If



                        'Dim ObjRet As New RuleResponse()
                        ''Initialising rule Object
                        'Dim ObjRule As New RuleEngin(EID, Form.DocumentType, "CREATED", "SUBMIT")
                        ''Uncomment
                        ''For parent Doc
                        'If Form.DocumentType.ToUpper.Trim = DocType.ToUpper.Trim Then
                        '    lstData1 = lineitem.DataItem
                        '    ObjRet = ObjRule.ExecuteRule(lineitem.DataItem, Nothing, False)
                        'Else
                        '    ObjRet = ObjRule.ExecuteRule(lstData1, lineitem.DataItem, True)
                        'End If

                        'Flag = ObjRet.Success
                        'If Flag = False Then
                        '    AllError = ObjRet.Message
                        '    Exit For
                        'End If
                        Flag = ValidateKeys(EID, DocumentType, lineitem, Form.FormType, Keys)
                        If Flag = False Then
                            ErrMsg = ErrMsg & Keys
                            AllError = ErrMsg
                            Exit For
                        End If
                        'Code for form validation
                        'Fire this code only when field pass all field level validation
                        If Flag = True Then
                            Dim onlyFormValiDate As DataView = ds.Tables(0).DefaultView
                            onlyFiltered.RowFilter = "DocumentType='" & DocumentType & "' AND FieldType <> 'Child Item' "
                            Dim theFlds1 As DataTable = onlyFormValiDate.Table.DefaultView.ToTable()
                            theFlds1.Columns.Add("Value")
                            theFlds1 = pushValueIntoTable(theFlds1, lineitem)
                            Dim Str = FormValidation(DocumentType, EID, theFlds1, "ADD")
                            'IF str returns "true" means it passed all validations
                            If Str.ToString().ToLower() = "true" Then
                                Flag = True
                            Else
                                ErrMsg = ErrMsg & " Please " & Str
                                Flag = False
                            End If
                        End If
                        ChldFlag = Flag
                        IsAllFormValid = IsAllFormValid And Flag
                        If Flag = False Then
                            If AllError = "" Then
                                AllError = ErrMsg
                            Else
                                AllError = AllError & "." & ErrMsg
                            End If
                        End If
                    Next
                    If Flag = False Then
                        IsAllFormValid = False
                        Exit For
                    End If
                Next
                ' IF form is validated go and save it 
                'Dim MainForm = From Form In LStDataFinal Where Form.DocumentType = DocType Select Form
                If IsAllFormValid = True And isChild = False Then
                    result = SaveData(EID, DocType, LStDataFinal, UID)
                Else
                    result = AllError
                End If
            Else
                result = "Invalid DocType."
            End If
        Catch ex As Exception
            ErrorLog.sendMail("ValidateParameterByDocumentType", ex.Message)
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
        Return result
    End Function

    Public Shared Function ValidateParameterByDocumentType1(EID As Integer, DocType As String, UID As Integer, Data As String, Optional ByRef lineitem1 As LineitemWrap = Nothing, Optional ByRef ChldFlag As Boolean = False, Optional isChild As Boolean = False, Optional isActionForm As Boolean = False, Optional ByRef fileUploadPath As ArrayList = Nothing, Optional ByRef prefixFilePath As String = "") As String
        Dim result As String = ""
        Dim con As SqlConnection = Nothing
        Dim da As SqlDataAdapter = Nothing
        Try

            Dim ds As New DataSet()
            Dim LStDataFinal As New List(Of DataWraper1)
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            con = New SqlConnection(conStr)
            da = New SqlDataAdapter("getDataOfForm100", con)
            da.SelectCommand.CommandType = CommandType.StoredProcedure
            da.SelectCommand.Parameters.AddWithValue("@EID", EID)
            da.SelectCommand.Parameters.AddWithValue("@FormName", DocType)
            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            da.Fill(ds)
            Dim onlyFiltered As DataView = ds.Tables(0).DefaultView
            'Note: in the case of Action form "IsActive" do not play any role. 
            If isActionForm = True Then
                onlyFiltered.RowFilter = "DocumentType='" & DocType & "' AND FieldType <> 'Formula Field'  AND invisible=0"
            Else
                onlyFiltered.RowFilter = "DocumentType='" & DocType & "' AND FieldType <> 'Formula Field' AND Isactive=1"
            End If
            Dim theFlds As DataTable = onlyFiltered.Table.DefaultView.ToTable()

            'Since user is a special type of document that's why adding some addition fields

            If DocType.ToUpper() = "USER" Then
                'Adding rows for static 
                AddUserStaticField(theFlds)
                'MASTER VALUED
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

                        End If
                        'If IsParameterSupp = False Then
                        '    Exit For
                        'End If
                    Next
                    'If IsParameterSupp = False Then
                    '    result = "Insufficient Parameter"
                    '    Exit For
                    'End If
                Next
                'Setting Form Data into object
                objDW.DocumentType = DocType
                objDW.Data = StrFormData
                Dim UDataC As UserData
                Dim objF As DataWraper1
                Dim objC As DataWraper1
                Dim ObjItem As LineitemWrap
                Dim LStDataItem As List(Of UserData)
                Dim LstlineItem As List(Of LineitemWrap)
                For Each objData In LstData
                    If DocType = objData.DocumentType Then
                        objF = New DataWraper1()
                        LStDataItem = New List(Of UserData)

                        If isActionForm = True Then
                            onlyFiltered.RowFilter = "DocumentType='" & objData.DocumentType & "' AND FieldType <> 'Formula Field' AND FieldType <> 'Child Item' AND invisible=0"
                        Else
                            onlyFiltered.RowFilter = "DocumentType='" & objData.DocumentType & "' AND FieldType <> 'Formula Field'  AND IsActive=1 AND FieldType <> 'Child Item'"
                        End If
                        theFlds = onlyFiltered.Table.DefaultView.ToTable()
                        'getting one row data
                        ObjItem = New LineitemWrap()
                        LstlineItem = New List(Of LineitemWrap)
                        ObjItem = CreateListCollection(objData.Data, theFlds, EID, DocType)
                        LstlineItem.Add(ObjItem)
                        'LstlineItem.Add(LStDataItem)
                        objF.LineItem = LstlineItem
                        objF.DocumentType = DocType
                        objF.FormType = objData.FormType
                        'objF.LineItem = LStDataItem
                        LStDataFinal.Add(objF)
                    Else
                        Dim arr As String() = Split(objData.Data, "{}")
                        UDataC = New UserData()

                        If isActionForm = True Then
                            onlyFiltered.RowFilter = "DocumentType='" & objData.DocumentType & "' AND FieldType <> 'Formula Field' AND FieldType <> 'Child Item'  AND invisible=0"
                        Else
                            onlyFiltered.RowFilter = "DocumentType='" & objData.DocumentType & "' AND FieldType <> 'Formula Field' AND IsActive=1 AND FieldType <> 'Child Item'"
                        End If

                        theFlds = onlyFiltered.Table.DefaultView.ToTable()
                        ObjItem = New LineitemWrap()
                        LstlineItem = New List(Of LineitemWrap)
                        objC = New DataWraper1()
                        For d As Integer = 0 To arr.Count - 1
                            objC.DocumentType = objData.DocumentType
                            objC.FormType = objData.FormType
                            If arr(d).Trim <> "" Then
                                UDataC = New UserData()
                                ObjItem = CreateListCollection(arr(d), theFlds, EID, objData.DocumentType)
                                LstlineItem.Add(ObjItem)
                            End If
                        Next
                        objC.LineItem = LstlineItem
                        LStDataFinal.Add(objC)
                    End If
                Next

                'Fill Drop Down And lookup values

                FillData(LStDataFinal, ds, EID, DocType, UID, fileUploader:=fileUploadPath, prefixFilePath:=prefixFilePath)
                'Validate Form
                Dim FormData As New DataWraper1()
                Dim lineitem As LineitemWrap
                'Navigating all the form
                Dim ErrMsg = "", AllError = ""
                Dim IsAllFormValid As Boolean = True
                Dim Flag As Boolean = True
                Dim lstData1 As New List(Of UserData)
                For Each Form In LStDataFinal
                    FormData = Form
                    Dim DocumentType = FormData.DocumentType
                    Dim LstLine = FormData.LineItem
                    Dim ErrorLine As Integer = 1
                    For Each Row In LstLine
                        ErrMsg = "Error(s) in document " & DocumentType & ".| " & ErrorLine & "|"
                        ErrorLine = ErrorLine + 1
                        lineitem = New LineitemWrap()
                        lineitem = Row
                        If Not lineitem1 Is Nothing Then
                            lineitem1 = Row
                        End If
                        'Commented By Manvendra Singh 21-01-2018'
                        'Flag = ValidateForm(EID, lineitem, Form.FormType, DocumentType, ErrMsg)
                        Dim Keys As String = ""
                        'Code For composit key validation
                        If Flag = False Then
                            AllError = ErrMsg
                            Exit For
                        End If
                        'Code For Rule Engine 

                        'Dim ObjRet As New RuleResponse()
                        'Dim objRule As New RuleEngin(EID, Form.DocumentType, "CREATED", "SUBMIT")
                        'Dim dsrule As DataSet = objRule.GetRules()
                        'Dim dtrule As New DataTable
                        'dtrule = dsrule.Tables(1)
                        'For Each dr As DataRow In dsrule.Tables(0).Rows
                        '    If Form.DocumentType.ToUpper.Trim = DocType.ToUpper.Trim Then
                        '        lstData1 = lineitem.DataItem
                        '        ObjRet = objRule.ExecuteRule(lineitem.DataItem, Nothing, False, "", dr, dtrule)
                        '    Else
                        '        ObjRet = objRule.ExecuteRule(lstData1, lineitem.DataItem, True, "", dr, dtrule)
                        '    End If
                        '    If ObjRet.Success = False Then
                        '        Flag = ObjRet.Success
                        '        AllError &= ObjRet.ErrorMessage
                        '    End If
                        'Next
                        'If Flag = False Then
                        '    Exit For
                        'End If



                        'Dim ObjRet As New RuleResponse()
                        ''Initialising rule Object
                        'Dim ObjRule As New RuleEngin(EID, Form.DocumentType, "CREATED", "SUBMIT")
                        ''Uncomment
                        ''For parent Doc
                        'If Form.DocumentType.ToUpper.Trim = DocType.ToUpper.Trim Then
                        '    lstData1 = lineitem.DataItem
                        '    ObjRet = ObjRule.ExecuteRule(lineitem.DataItem, Nothing, False)
                        'Else
                        '    ObjRet = ObjRule.ExecuteRule(lstData1, lineitem.DataItem, True)
                        'End If

                        'Flag = ObjRet.Success
                        'If Flag = False Then
                        '    AllError = ObjRet.Message
                        '    Exit For
                        'End If
                        Flag = ValidateKeys(EID, DocumentType, lineitem, Form.FormType, Keys)
                        If Flag = False Then
                            ErrMsg = ErrMsg & Keys
                            AllError = ErrMsg
                            Exit For
                        End If
                        'Code for form validation
                        'Fire this code only when field pass all field level validation
                        If Flag = True Then
                            Dim onlyFormValiDate As DataView = ds.Tables(0).DefaultView
                            onlyFiltered.RowFilter = "DocumentType='" & DocumentType & "' AND FieldType <> 'Child Item' "
                            Dim theFlds1 As DataTable = onlyFormValiDate.Table.DefaultView.ToTable()
                            theFlds1.Columns.Add("Value")
                            theFlds1 = pushValueIntoTable(theFlds1, lineitem)
                            Dim Str = FormValidation(DocumentType, EID, theFlds1, "ADD")
                            'IF str returns "true" means it passed all validations
                            If Str.ToString().ToLower() = "true" Then
                                Flag = True
                            Else
                                ErrMsg = ErrMsg & " Please " & Str
                                Flag = False
                            End If
                        End If
                        ChldFlag = Flag
                        IsAllFormValid = IsAllFormValid And Flag
                        If Flag = False Then
                            If AllError = "" Then
                                AllError = ErrMsg
                            Else
                                AllError = AllError & "." & ErrMsg
                            End If
                        End If
                    Next
                    If Flag = False Then
                        IsAllFormValid = False
                        Exit For
                    End If
                Next
                ' IF form is validated go and save it 
                'Dim MainForm = From Form In LStDataFinal Where Form.DocumentType = DocType Select Form
                If IsAllFormValid = True And isChild = False Then
                    result = DraftData(EID, DocType, LStDataFinal, UID)
                Else
                    result = AllError
                End If
            Else
                result = "Invalid DocType."
            End If
        Catch ex As Exception
            ErrorLog.sendMail("ValidateParameterByDocumentType", ex.Message)
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
        Return result
    End Function

    Public Shared Function ValidateKeys(EID As Integer, DocumentType As String, Data As LineitemWrap, Formtype As String, ByRef Msg As String, Optional tid As String = "0") As Boolean

        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim ret As Boolean = False
        Dim ds As New DataSet()
        Dim IskeySupplyed As Boolean = False
        Dim ExChars As String = ""
        Dim StrUSerunqQuery = ""
        Dim objUp As New UpdateData()
        Dim objD As New DynamicForm()
        Dim Keys = ""
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
                ds = objUp.GetKeys(EID, DocumentType)
            End If
            If ds.Tables(0).Rows.Count > 0 Then
                ExChars = Convert.ToString(ds.Tables(0).Rows(0).Item("ExChars"))
                'loop through 
                For j As Integer = 0 To ds.Tables(0).Rows.Count - 1
                    IskeySupplyed = False
                    If Msg <> "" Then
                        Msg = Msg & ", " & ds.Tables(0).Rows(j).Item("DisplayName")
                    Else
                        Msg = ds.Tables(0).Rows(j).Item("DisplayName")
                    End If
                    For Each Row In Data.DataItem
                        If ds.Tables(0).Rows(j).Item("DisplayName").Trim.ToUpper = Row.DisplayName.ToString.ToUpper.Trim Then
                            Dim Values = Row.Values.Trim
                            'Code For Removing Extra Charactor 
                            If Not String.IsNullOrEmpty(ExChars.Trim) Then
                                Values = objUp.Removekeys(Values, ExChars).Trim()
                            End If
                            Keys = Keys & " AND " & objD.GenerateReplaceStatement(ds.Tables(0).Rows(j).Item("FieldMapping"), ExChars) & "='" & Values.Trim() & "'"
                            IskeySupplyed = True
                            Exit For
                        End If
                    Next
                    If IskeySupplyed = False Then
                        Exit For
                    End If
                Next
                Dim TIDCO = " AND  tid<> "
                If IskeySupplyed = False Then
                    Keys = "Insufficient keys supplyed."
                    Msg = Keys
                    ret = False
                Else
                    'Query From data Base For check purpous 
                    Dim tableName = "", CurStatus = "", whereCond = "", Query = "SELECT COUNT(*) FROM "
                    If Formtype.Trim.ToUpper = "MASTER" Then
                        tableName = "MMM_MST_MASTER"
                    ElseIf Formtype.Trim.ToUpper = "DOCUMENT" Then
                        tableName = "MMM_MST_DOC"
                        CurStatus = " AND CurStatus<>'REJECTED'"
                    ElseIf Formtype.Trim.ToUpper = "USER" Then
                        tableName = "MMM_MST_USER"
                        TIDCO = " AND  uid= "
                    End If
                    If tableName = "MMM_MST_USER" Then
                        whereCond = " WHERE EID= " & EID & Keys & CurStatus
                    Else
                        whereCond = " WHERE EID= " & EID & " AND DocumentType= '" & DocumentType & "' " & Keys & CurStatus
                    End If
                    Query = Query & tableName & whereCond
                    If tid.Trim <> "0" Then
                        Query = Query & TIDCO & tid
                    End If
                    Dim Count = 0
                    Using con = New SqlConnection(conStr)
                        Using da = New SqlDataAdapter(Query, con)
                            con.Open()
                            Count = da.SelectCommand.ExecuteScalar()
                        End Using
                    End Using
                    If Count > 0 Then
                        ret = False
                        Msg = "Please check combination of  " & Msg & ".It must be unique."
                    Else
                        ret = True
                    End If
                End If
            Else
                ret = True
                'This will be returned in case of document without key configuration
            End If
        Catch ex As Exception
            Throw
        End Try
        Return ret.ToString.Trim()
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
        dt.Rows(dt.Rows.Count - 1).Item("DocumentType") = "USER"
        dt.Rows(dt.Rows.Count - 1).Item("DisplayOrder") = "10000"
        dt.Rows(dt.Rows.Count - 1).Item("AllowCreateRecord_onfly") = "False"
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
        dt.Rows(dt.Rows.Count - 1).Item("DocumentType") = "USER"
        dt.Rows(dt.Rows.Count - 1).Item("DisplayOrder") = "10000"
        dt.Rows(dt.Rows.Count - 1).Item("AllowCreateRecord_onfly") = "False"
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
        dt.Rows(dt.Rows.Count - 1).Item("DocumentType") = "USER"
        dt.Rows(dt.Rows.Count - 1).Item("DisplayOrder") = "10000"
        dt.Rows(dt.Rows.Count - 1).Item("AllowCreateRecord_onfly") = "False"
        dt.Rows.Add()
        dt.Rows(dt.Rows.Count - 1).Item("DisplayName") = "isAuth"
        dt.Rows(dt.Rows.Count - 1).Item("FieldType") = "Text Box"
        dt.Rows(dt.Rows.Count - 1).Item("IsRequired") = "0"
        dt.Rows(dt.Rows.Count - 1).Item("Datatype") = "Numeric"
        dt.Rows(dt.Rows.Count - 1).Item("FieldMapping") = "IsAuth"
        dt.Rows(dt.Rows.Count - 1).Item("FieldID") = "-300000"
        dt.Rows(dt.Rows.Count - 1).Item("MaxLen") = "1"
        dt.Rows(dt.Rows.Count - 1).Item("DropDown") = ""
        dt.Rows(dt.Rows.Count - 1).Item("DropDownType") = ""
        dt.Rows(dt.Rows.Count - 1).Item("MinLen") = "1"
        dt.Rows(dt.Rows.Count - 1).Item("EnableEdit") = "1"
        dt.Rows(dt.Rows.Count - 1).Item("DocumentType") = "USER"
        dt.Rows(dt.Rows.Count - 1).Item("DisplayOrder") = "10000"
        dt.Rows(dt.Rows.Count - 1).Item("AllowCreateRecord_onfly") = "False"
        Return dt
    End Function

    Public Shared Function GetQuery(ByVal doctype As String, ByVal fld As String, ByVal EID As Integer, DDlText As String) As String

        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim da As New SqlDataAdapter("", con)
        Dim ds As New DataSet
        Dim str As String = ""
        Try
            da.SelectCommand.CommandText = "usp_GetMasterValued1Uploader"
            da.SelectCommand.CommandType = CommandType.StoredProcedure
            da.SelectCommand.Parameters.Clear()
            da.SelectCommand.Parameters.AddWithValue("@doctype", doctype)
            da.SelectCommand.Parameters.AddWithValue("@eid", EID)
            da.SelectCommand.Parameters.AddWithValue("@fldmapping", fld)
            da.SelectCommand.Parameters.AddWithValue("@DDL_TEXT", DDlText)

            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            str = da.SelectCommand.ExecuteScalar()
            Return str
        Catch ex As Exception
            Throw
        Finally
            con.Close()
            con.Dispose()
        End Try

    End Function
    Public Shared Function GetQuery1(ByVal doctype As String, ByVal fld As String, ByVal EID As Integer, DDlText As String) As String
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim da As New SqlDataAdapter("", con)
        Try
            Dim ds As New DataSet
            Dim str As String = ""
            da.SelectCommand.CommandText = "usp_GetMasterValued1Uploader1"
            da.SelectCommand.CommandType = CommandType.StoredProcedure
            da.SelectCommand.Parameters.Clear()
            da.SelectCommand.Parameters.AddWithValue("@doctype", doctype)
            da.SelectCommand.Parameters.AddWithValue("@eid", EID)
            da.SelectCommand.Parameters.AddWithValue("@fldmapping", fld)
            da.SelectCommand.Parameters.AddWithValue("@DDL_TEXT", DDlText)
            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            str = da.SelectCommand.ExecuteScalar()
            Return str
        Catch ex As Exception
            Throw
        Finally
            con.Close()
            con.Dispose()
            da.Dispose()
        End Try

    End Function

    Public Function UserDataFilter(ByVal cdocumenttype As String, ByVal ddocumenttype As String, EID As Integer) As String
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim da As New SqlDataAdapter("", con)
        Try
            Dim ds As New DataSet
            Dim fldmapping As String = ""
            Dim fldid As String = ""
            da.SelectCommand.CommandText = "select docmapping from mmm_mst_forms where eid=" & EID & " and Formname='" & ddocumenttype & "'"
            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            fldmapping = da.SelectCommand.ExecuteScalar.ToString()
            If fldmapping.Length > 2 Then
                da.SelectCommand.CommandText = "select " & fldmapping & ",documenttype,iscreate,isedit from mmm_ref_role_user where eid=" & EID & " and Uid=" & HttpContext.Current.Session("uid") & " and roleNAME='" & HttpContext.Current.Session("USERROLE") & "' and '" & cdocumenttype & "' in (select * from InputString1(documenttype))"
                da.Fill(ds, "FILTER")
                If ds.Tables("FILTER").Rows.Count = 0 Then
                    fldid = ""
                ElseIf ds.Tables("FILTER").Rows.Count = 1 And ds.Tables("FILTER").Rows(0).Item("iscreate").ToString() <> "0" Then
                    fldid = ds.Tables("FILTER").Rows(0).Item(0).ToString()
                Else
                    Dim RW() As DataRow = ds.Tables("FILTER").Select("ISCREATE=1")
                    If RW.Length > 0 Then
                        fldid = RW(0).Item(0).ToString()
                    Else
                        fldid = ""
                    End If
                End If
            End If
            Return fldid
        Catch ex As Exception
            Throw
        Finally
            da.Dispose()
            con.Close()
            con.Dispose()
        End Try
    End Function

    Public Shared Function pushValueIntoTable(dtFiels As DataTable, lineItem As LineitemWrap) As DataTable
        Dim dt As New DataTable()
        Try
            dt = dtFiels.Copy()
            For i As Integer = 0 To dt.Rows.Count - 1
                Dim Value = ""
                Dim DisplayName = dt.Rows(i).Item("DisplayName")
                For Each line In lineItem.DataItem
                    If line.DisplayName.Trim() = DisplayName.ToString().Trim() Then
                        dt.Rows(i).Item("Value") = line.Values.ToString().Trim()
                    End If
                Next
            Next
        Catch ex As Exception

        End Try
        Return dt

    End Function

    Public Shared Function SaveData(EID As Integer, DocType As String, LStDataFinal As List(Of DataWraper1), UID As Integer) As String
        Dim strResult = ""
        Dim strColumn As String = "", strValue As String = ""
        Dim StrQuery = ""
        Dim FileID = 0
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = Nothing
        Dim da As SqlDataAdapter = Nothing
        Dim tran As SqlTransaction = Nothing
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
                        'Source
                        strColumn = "INSERT INTO MMM_MST_Master(EID,Documenttype,CreatedBy,UpdatedDate,Source"
                        strValue = " VALUES" & "(" & EID & ",'" & line.DocumentType & "'," & UID & ",getdate(),'WS' "
                        FormType1 = "Master"

                    ElseIf (line.FormType.ToUpper = "USER") Then
                        strColumn = "INSERT INTO MMM_MST_USER(EID,ModiFyDate,LocationID"
                        strValue = "VALUES (" & EID & " ,getdate(),'2072'"
                        DocDBTableName = "MMM_MST_USER"
                        FormType1 = "USER"
                    Else
                        strColumn = "INSERT INTO MMM_MST_DOC(EID,Documenttype,oUID,adate,Source"
                        strValue = "VALUES (" & EID & ",'" & line.DocumentType & "'," & UID & ",getdate(),'WS'"
                        DocDBTableName = "MMM_MST_DOC"
                        FormType1 = "Document"
                    End If
                    'Generate query 
                    For Each obj In line.LineItem
                        StrQuery = GenerateQuery(strColumn, strValue, obj)
                    Next
                    StrQuery = StrQuery & ";Select @@identity"
                    con = New SqlConnection(conStr)
                    If con.State <> ConnectionState.Open Then
                        con.Open()
                    End If
                    tran = con.BeginTransaction()
                    da = New SqlDataAdapter(StrQuery, con)
                    da.SelectCommand.Transaction = tran
                    'Transactional Query
                    FileID = da.SelectCommand.ExecuteScalar()
                    If FormType1.ToUpper() <> "USER" Then
                        'Code For saving Auto Number
                        Dim AutoNumQ = "select * FROM MMM_MST_FIELDS WHERE EID=" & EID & " AND DocumentType='" & line.DocumentType & "' AND (FieldType='Auto Number' or Fieldtype='New Auto Number')"
                            Dim DSAuto As New DataSet()
                            da.SelectCommand.CommandText = AutoNumQ
                            da.Fill(DSAuto)
                            For AutoNumC As Integer = 0 To DSAuto.Tables(0).Rows.Count - 1
                                Select Case DSAuto.Tables(0).Rows(AutoNumC).Item("fieldtype").ToString
                                    Case "Auto Number"
                                        da.SelectCommand.Parameters.Clear()
                                        If da.SelectCommand.Transaction Is Nothing Then
                                            da.SelectCommand.Transaction = tran
                                        End If
                                        da.SelectCommand.CommandText = "usp_GetAutoNoNew_WS"
                                        da.SelectCommand.CommandType = CommandType.StoredProcedure
                                        da.SelectCommand.Parameters.AddWithValue("Fldid", DSAuto.Tables(0).Rows(AutoNumC).Item("FieldID"))
                                        da.SelectCommand.Parameters.AddWithValue("docid", FileID)
                                        da.SelectCommand.Parameters.AddWithValue("fldmapping", DSAuto.Tables(0).Rows(AutoNumC).Item("FieldMapping"))
                                        da.SelectCommand.Parameters.AddWithValue("FormType", FormType1)
                                        'Transactional Query
                                        Dim an As String = Convert.ToString(da.SelectCommand.ExecuteScalar())
                                        da.SelectCommand.Parameters.Clear()
                                    Case "New Auto Number"
                                        da.SelectCommand.Parameters.Clear()
                                        da.SelectCommand.CommandText = "usp_GetAutoNoNew_New"
                                        da.SelectCommand.CommandType = CommandType.StoredProcedure
                                        da.SelectCommand.Parameters.AddWithValue("Fldid", DSAuto.Tables(0).Rows(AutoNumC).Item("FieldID"))
                                        da.SelectCommand.Parameters.AddWithValue("SearchFldid", DSAuto.Tables(0).Rows(AutoNumC).Item("Dropdown"))
                                        da.SelectCommand.Parameters.AddWithValue("docid", FileID)
                                        da.SelectCommand.Parameters.AddWithValue("fldmapping", DSAuto.Tables(0).Rows(AutoNumC).Item("FieldMapping"))
                                        da.SelectCommand.Parameters.AddWithValue("FormType", FormType1)
                                        Dim an As String = da.SelectCommand.ExecuteScalar()
                                        da.SelectCommand.Parameters.Clear()
                                End Select

                            Next
                            Dim FormulaQ = "select * FROM MMM_MST_FIELDS WHERE EID=" & EID & " AND DocumentType='" & line.DocumentType & "' AND FieldType='Formula Field' order by displayorder"
                            Dim DSF As New DataSet()
                            da.SelectCommand.Parameters.Clear()
                            da.SelectCommand.CommandText = FormulaQ
                            da.SelectCommand.CommandType = CommandType.Text
                            da.Fill(DSF)
                            'Ececuting Formula Fields
                            Try
                                Dim viewdoc As String = line.DocumentType.Trim()
                                viewdoc = viewdoc.Replace(" ", "_")
                                If DSF.Tables(0).Rows.Count > 0 Then
                                    For f As Integer = 0 To DSF.Tables(0).Rows.Count - 1
                                        Dim formulaeditorr As New formulaEditor
                                        Dim forvalue As String = String.Empty
                                        forvalue = formulaeditorr.ExecuteFormulaT(DSF.Tables(0).Rows(f).Item("KC_LOGIC"), FileID, "v" + EID.ToString + viewdoc, EID, 0, con, tran)
                                        'Transactional Query
                                        Dim upquery As String = "update " & DSF.Tables(0).Rows(f).Item("DBTableName").ToString & "  set  " & DSF.Tables(0).Rows(f).Item("FieldMapping").ToString & "='" & forvalue.ToString & "'  where tid =" & FileID & ""
                                        da.SelectCommand.CommandText = upquery
                                        da.SelectCommand.CommandType = CommandType.Text
                                        da.SelectCommand.ExecuteNonQuery()
                                    Next
                                End If
                            Catch ex As Exception
                                Throw
                            End Try
                        End If
                Next
                'Getting Child Item Generating Query
                Dim ChildForm = From Form In LStDataFinal Where Form.DocumentType <> DocType Select Form
                StrQuery = ""
                Dim StrQry1 = ""
                Dim ObjForm As DataWraper1
                If FileID > 0 Then
                    For Each line In ChildForm
                        strColumn = "INSERT INTO MMM_MST_DOC_ITEM(DOCID,documenttype,isauth"
                        strValue = "VALUES (" & FileID & ",'" & line.DocumentType & "'," & "1"
                        StrQuery = ""
                        For Each obj In line.LineItem
                            StrQry1 = GenerateQuery(strColumn, strValue, obj)

                            If StrQuery = "" Then
                                StrQuery = StrQry1
                            Else
                                StrQuery = StrQuery & ";" & StrQry1
                            End If
                        Next
                        'da = New SqlDataAdapter(StrQuery, con)
                        da.SelectCommand.CommandText = StrQuery
                        da.SelectCommand.CommandType = CommandType.Text
                        'If con.State <> ConnectionState.Open Then
                        '    con.Open()
                        'End If
                        Dim FileID1 = da.SelectCommand.ExecuteNonQuery()
                        ObjForm = New DataWraper1()
                        ObjForm = line
                        'Code For executing formula for child item
                        If (FileID > 0) And (ObjForm.DocumentType <> "") Then
                            Dim FormulaQ = "select * FROM MMM_MST_FIELDS WHERE EID=" & EID & " AND DocumentType='" & line.DocumentType & "' AND FieldType='Formula Field' order by displayorder"
                            Dim DSF As New DataSet()
                            'da = New SqlDataAdapter(FormulaQ, con)
                            da.SelectCommand.CommandText = FormulaQ
                            da.SelectCommand.CommandType = CommandType.Text
                            da.Fill(DSF)
                            'Ececuting Formula Fields
                            Try
                                Dim viewdoc As String = line.DocumentType
                                viewdoc = viewdoc.Replace(" ", "_")
                                If DSF.Tables(0).Rows.Count > 0 Then
                                    For f As Integer = 0 To DSF.Tables(0).Rows.Count - 1
                                        Dim formulaeditorr As New formulaEditor
                                        Dim forvalue As String = String.Empty
                                        forvalue = formulaeditorr.ExecuteFormulaT(DSF.Tables(0).Rows(f).Item("KC_LOGIC"), FileID, DSF.Tables(0).Rows(f).Item("DBTableName").ToString, EID, 0, con, tran)
                                        'Transactional Query
                                        Dim upquery As String = "update " & DSF.Tables(0).Rows(f).Item("DBTableName").ToString & "  set  " & DSF.Tables(0).Rows(f).Item("FieldMapping").ToString & "='" & forvalue.ToString & "'  where tid =" & FileID & ""
                                        da.SelectCommand.CommandText = upquery
                                        da.SelectCommand.CommandType = CommandType.Text
                                        da.SelectCommand.ExecuteNonQuery()
                                    Next
                                End If
                            Catch ex As Exception
                                Throw
                            End Try

                            'Added functionality for New Auto Number in Child Item  02/08/2018
                            Dim objDC As New DataClass()
                            Dim objDT As New DataTable()
                            If line.DocumentType <> "" Then
                                objDT = objDC.TranExecuteQryDT("select * from mmm_mst_fields where documenttype='" & line.DocumentType & "' and eid=" & EID & " and fieldtype='Auto Number'", con:=con, tran:=tran)
                                'Dim rowAutoNumber As DataRow() = dtField.Select("Fieldtype='Auto Number' or Fieldtype='New Auto Number'")
                                Dim ht As New Hashtable()
                                For i As Integer = 0 To objDT.Rows.Count - 1
                                    Dim chldDT As New DataTable()
                                    chldDT = objDC.TranExecuteQryDT("select tid from mmm_mst_doc_item where docid=" & FileID, con:=con, tran:=tran)
                                    For Each drChildDT As DataRow In chldDT.Rows
                                        ht.Clear()
                                        ht.Add("Fldid", objDT.Rows(i).Item("fieldid"))
                                        ht.Add("docid", drChildDT(0))
                                        ht.Add("fldmapping", objDT.Rows(i).Item("fieldmapping"))
                                        ht.Add("FormType", "DOCDETAIL")
                                        objDC.TranExecuteProDT("usp_GetAutoNoNew", ht, con:=con, tran:=tran).Rows(0)(0).ToString()
                                    Next
                                Next
                            End If
                            ''''Ended

                            'Last parameter is for child item
                            'Add Trigger Code File into Project
                            'Transactional Query
                            Trigger.ExecuteTriggerT(ObjForm.DocumentType, EID, FileID, con, tran, 1, TriggerNature:="Create")
                            'Trigger.ExecuteTriggerT(ObjForm.DocumentType, EID, FileID, con, tran, 1, TriggerNature:="Create")
                        End If
                    Next
                    If FormType.ToUpper() = "DOCUMENT" Then
                        Dim ob1 As New DMSUtil()
                        da.SelectCommand.CommandText = "InsertDefaultMovement"
                        da.SelectCommand.CommandType = CommandType.StoredProcedure
                        da.SelectCommand.Parameters.Clear()
                        da.SelectCommand.Parameters.AddWithValue("tid", FileID)
                        da.SelectCommand.Parameters.AddWithValue("CUID", UID.ToString())
                        da.SelectCommand.Parameters.AddWithValue("what", "UPLOADED")
                        'If con.State <> ConnectionState.Open Then
                        '    con.Open()
                        'End If
                        'Transactional Query
                        da.SelectCommand.ExecuteNonQuery()
                        Dim res As String
                        'Transactional Query
                        res = ob1.GetNextUserFromRolematrixT(FileID, EID, UID, "", UID, con, tran)
                        Dim sretMsgArr() As String = res.Split(":")

                        '''' check if no skip setting and if not allowed then don't move doc and show msg to user by sunil on 07-Oct
                        If sretMsgArr(0).ToUpper() = "NO SKIP" Then
                            Dim Noskipmsg As String = "Next Approvar/User not found, please contact Admin"
                            'lblMsg.Text = "System Docid is " & fileid & " " & msgAN & "" & "<br/> " & Noskipmsg
                            'this code block is added by ajeet kumar for transaction to be rolled back
                            tran.Rollback()
                            Return strResult = Noskipmsg
                        End If

                        'Code for Mapping
                        If sretMsgArr(0) = "ARCHIVE" Then
                            Dim Op As New Exportdata()
                            'Transactional Query
                            Op.PushdataT(FileID, sretMsgArr(1), EID, con, tran)
                        End If
                        'Code Added By Ajeet For Period Wise Balance Date:04-Feb-2015
                        Try
                            Dim objRel As New Relation()
                            objRel.ExecutePeriodWiseBalance(EID, FileID, MainDocType, con, tran)
                        Catch ex As Exception

                        End Try
                        'Chandni code starts from here for Lease master setting check
                        Try
                            Dim ComUtillObj As New CommanUtil()
                            Dim LMres As String = ""
                            If sretMsgArr(0) = "ARCHIVE" And EID = 181 Then
                                ComUtillObj.CheckLMSettings(EID, DocType, UID, FileID, con, tran)
                            End If
                        Catch ex As Exception

                        End Try
                        'Chandni code ends here for Lease master setting check

                        'Commiting transaction 

                    End If

                    If FormType1 <> "USER" Then
                        'Code For Saving it into histroy
                        Dim ob As New DynamicForm
                        'Transactional Query
                        'ob.History(EID, FileID, UID, MainDocType, DocDBTableName, "ADD")
                        ob.HistoryT(EID, FileID, UID, MainDocType, DocDBTableName, "ADD", con, tran)
                        'Trigger OF Form Will Execute here 
                        Trigger.ExecuteTriggerT(MainDocType, EID, FileID, con, tran, 1, TriggerNature:="Create", FormType:=FormType1)
                        'Trigger.ExecuteTrigger(MainDocType, EID, FileID, TriggerNature:="Create")
                        If FormType1.ToUpper() = "MASTER" Then
                            da.SelectCommand.CommandText = "usp_AutoEntryInRoleAssignment"
                            da.SelectCommand.CommandType = CommandType.StoredProcedure
                            da.SelectCommand.Parameters.Clear()
                            da.SelectCommand.Parameters.AddWithValue("@EID", EID)
                            da.SelectCommand.Parameters.AddWithValue("@ScreenName", MainDocType)
                            da.SelectCommand.Parameters.AddWithValue("@DocMappedId", FileID)

                            da.SelectCommand.ExecuteNonQuery()

                        End If

                        strResult = "Your DocID is " + FileID.ToString()
                        tran.Commit()
                        'Code For extending relation 
                        Try
                            Dim objRel As New Relation()
                            Dim objRes = objRel.ExtendRelation(EID, MainDocType, FileID, UID, "", True)
                        Catch ex As Exception
                        End Try
                        Try
                            GisMethods.ExecuteReverseGeoCoding(EID, FileID, MainDocType)
                            'trans.Commit()
                        Catch ex As Exception

                        End Try
                        'This Code block is added by Ajeet On:23/07/2015 For AutoDocument creation
                        Try
                            Dim objSh As New ScheduleDocument(EID, MainDocType, "Save")
                            Dim res = objSh.Execute(FileID)
                        Catch ex As Exception

                        End Try
                        'Code for calling outward webservice
                        Try
                            If FileID > 0 Then
                                Dim WSOUT As New WSOutward()
                                Dim WSOret = WSOUT.WBS(DocType, EID, FileID)
                                If Not (String.IsNullOrEmpty(WSOret)) Then
                                    strResult = strResult & "~" & WSOret
                                End If
                            End If
                        Catch ex As Exception
                        End Try

                        ' below code for webservice report - 20_june_14 ravi
                        If FileID > 0 Then
                            Dim da1 As SqlDataAdapter = New SqlDataAdapter("select * from MMM_Print_Template where documenttype='" & DocType & "' and draft='Service'", con)
                            Dim ds1 As New DataSet
                            da1.Fill(ds1, "dataset")
                            Try
                                For k As Integer = 0 To ds1.Tables("dataset").Rows.Count - 1
                                    If ds1.Tables("dataset").Rows(k).Item("SendType").ToString() = "WS" Then
                                        Dim ws As New WSOutward()
                                        Dim URLIST As String = ws.WBSREPORT(DocType, EID, FileID)
                                    End If
                                Next
                            Catch ex As Exception
                            Finally
                                da1.Dispose()
                                ds1.Dispose()
                            End Try
                        End If
                        'Code For templet calling
                        Dim ob1 As New DMSUtil()
                        Try
                            'Change Required
                            ob1.TemplateCalling(FileID, EID, MainDocType, "CREATED")
                        Catch ex As Exception
                        End Try
                        Try
                            da.SelectCommand.CommandType = CommandType.Text
                            da.SelectCommand.Parameters.Clear()
                            da.SelectCommand.CommandText = "select isworkflow from mmm_mst_doc where TID=" & FileID & " and eid='" & EID & "'"
                            Dim dt As New DataTable
                            da.Fill(dt)
                            If dt.Rows.Count = 1 Then
                                If dt.Rows(0).Item(0).ToString = "1" Then
                                    'Change Required
                                    ob1.TemplateCalling(FileID, EID, MainDocType, "APPROVE")
                                End If
                            End If
                        Catch ex As Exception
                        End Try
                    Else
                        'IF User 
                        Dim UserResult As String = ""
                        Dim dsres As New DataSet()
                        da = New SqlDataAdapter("UserCheckWS", con)
                        da.SelectCommand.CommandType = CommandType.StoredProcedure
                        da.SelectCommand.Parameters.Clear()
                        da.SelectCommand.Parameters.AddWithValue("@UID", FileID)
                        da.SelectCommand.Parameters.AddWithValue("@EID", EID)
                        da.SelectCommand.Transaction = tran
                        da.Fill(dsres)
                        'Transactional Query
                        Dim arr = dsres.Tables(1).Rows(0).Item(0).ToString().Split(":")
                        Trigger.ExecuteTriggerT(MainDocType, EID, FileID, con, tran, 1, TriggerNature:="Create", FormType:="Document")
                        'Trigger.ExecuteTrigger(MainDocType, EID, FileID, TriggerNature:="Create")
                        If arr(0) <> "0" Then

                            strResult = "Your DocID is " + FileID.ToString()
                            'Commiting transaction here
                            tran.Commit()
                            Try
                                GisMethods.ExecuteReverseGeoCoding(EID, FileID, MainDocType)
                                'trans.Commit()
                            Catch ex As Exception

                            End Try
                            Dim DMS As New DMSUtil
                            Try
                                ''Change Required
                                DMS.notificationMail(FileID, EID, "USER", "USER CREATED")
                            Catch ex As Exception
                            End Try
                        Else
                            strResult = arr(1)
                            tran.Rollback()
                        End If
                    End If
                End If
            End If
        Catch ex As Exception
            tran.Rollback()
            ErrorLog.sendMail("Commanutil.SaveData", ex.Message)
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
    Public Function CheckLMSettingsEdit(ByVal eid As Integer, ByVal DOCType As String, ByVal UID As String, ByVal DocID As Integer, ByVal con As SqlConnection, ByVal tran As SqlTransaction) As String
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        'Dim con1 As SqlConnection = New SqlConnection(conStr)
        Dim ob As New DynamicForm
        Dim da As SqlDataAdapter = New SqlDataAdapter("", con)
        da.SelectCommand.Transaction = tran

        Try

            Dim con1 As SqlConnection = New SqlConnection(conStr)
            Dim da1 As SqlDataAdapter = New SqlDataAdapter("select lm.Tid,TDocType,SDocType,IsActiveStatus,wfstatus,lm.ChildDocType  as MChildDocType,TFld,SFld,Ordering,lmfm.ChildDocType,DocType,LMTid from  MMM_MST_LMsetting lm  inner join  MMM_MST_LMFieldMappingsetting lmfm on lm.Tid=lmfm.LMTid   where  lm.eid=" & eid & "   and IsActiveStatus=1  and lmfm.eid=" & eid & " and lm.SDocType='" & DOCType & "'", con1)

            Dim ds As New DataSet
            Dim dt As New DataTable
            da1.Fill(ds, "LMSettingData")

            Dim Fldstr As String = ""
            If ds.Tables("LMSettingData").Rows.Count <> 0 Then


                Dim Uptstr As String = String.Empty
                Dim res As String = ""
                Dim strTFlds As String = String.Empty
                Dim strSFlds As String = String.Empty
                Dim strHTFlds As String = String.Empty
                Dim strHSFlds As String = String.Empty
                Dim ChildDocT As String = String.Empty
                Dim TDocType As String = String.Empty
                Dim TFldmapping As String = String.Empty
                Dim Sfldmapping As String = String.Empty


                Dim dtdata As New DataTable
                Dim dtdataHeader As New DataTable
                If ds.Tables("LMSettingData").Rows.Count > 0 Then
                    dtdata = ds.Tables("LMSettingData").[Select]("DocType = 'Line'").CopyToDataTable()
                    dtdataHeader = ds.Tables("LMSettingData").[Select]("DocType = 'Header'").CopyToDataTable()
                    If dtdataHeader.Rows.Count > 0 Then

                        strHTFlds += String.Join(",", (From row In dtdataHeader.Rows Select CType(row.Item("TFld"), String)).ToArray)
                        strHSFlds += String.Join(",d.", (From row In dtdataHeader.Rows Select CType(row.Item("SFld"), String)).ToArray)
                        Dim strHSFldsArr As String()
                        strHSFldsArr = strHSFlds.Split(",")
                        Dim value As String = String.Join(",',',", strHSFldsArr)
                        strHSFlds = "d." + value
                    End If
                    If dtdata.Rows.Count > 0 Then
                        TDocType = dtdata.Rows(0).Item("TDocType")
                        ChildDocT = dtdata.Rows(0).Item("MChildDocType")
                        strTFlds += String.Join(",", (From row In dtdata.Rows Select CType(row.Item("TFld"), String)).ToArray)
                        strSFlds += String.Join(",i.", (From row In dtdata.Rows Select CType(row.Item("SFld"), String)).ToArray)
                        Dim strSFldsArr As String()
                        strSFldsArr = strSFlds.Split(",")
                        Dim valueL As String = String.Join(",',',", strSFldsArr)
                        strSFlds = "i." + valueL
                        'For i As Integer = 0 To dtdata.Rows.Count - 1
                        da.SelectCommand.CommandText = "select df.FieldMapping as DFld,mf.FieldMapping as MFld from MMM_MST_FIELDS mf inner join MMM_MST_FIELDS df on mf.displayName=df.displayName where mf.eid=" & eid & " and mf.documenttype='" & TDocType & "' and  df.eid=" & eid & " and df.documenttype='" & DOCType & "'  and  df.FieldType='Auto Number'; "
                        da.Fill(ds, "LMDocDataFlds")
                        ' Next
                        If ds.Tables("LMDocDataFlds").Rows.Count > 0 Then
                            TFldmapping = ds.Tables("LMDocDataFlds").Rows(0).Item("MFld")
                            Sfldmapping = "d." + ds.Tables("LMDocDataFlds").Rows(0).Item("DFld")
                        End If


                        da.SelectCommand.CommandText = " select i.tid, concat(" & strSFlds & ") as SrcLineFldVAl,concat(" & strHSFlds & ") as SrcHeaderFldVAl," & Sfldmapping & " as Sfldmapping from mmm_mst_doc d inner join mmm_mst_doc_item i on d.tid=i.DOCID where d.documenttype='" & DOCType & "' and  i.documenttype='" & ChildDocT & "' and d.eid=" & eid & "  and i.docid=" & DocID & " order by i.tid asc "
                        da.Fill(ds, "LMDocData")

                        'da.SelectCommand.CommandText = "Select TID from mmm_mst_master  where documenttype='" & TDocType & "' and eid=" & eid & "  and " & TFldmapping & "='" & ds.Tables("LMDocData").Rows(0).Item("Sfldmapping") & "' order by tid asc"
                        'da.Fill(ds, "LMMasterData")

                    End If

                    If ds.Tables("LMDocData").Rows.Count > 0 Then

                        For j As Integer = 0 To ds.Tables("LMDocData").Rows.Count - 1


                            Dim SrcLineFldsArr As String() = strTFlds.Split(",")

                            Dim strLineFldvalarr As String() = ds.Tables("LMDocData").Rows(j).Item("SrcLineFldVAl").ToString().Split(",")

                            Dim strRes As String = ""
                            For str1 As Integer = 0 To SrcLineFldsArr.Length - 1
                                For str As Integer = str1 To strLineFldvalarr.Length - 1
                                    strRes += "," + SrcLineFldsArr(str1) + "='" + strLineFldvalarr(str) + "'"

                                    Exit For
                                Next
                            Next

                            Dim SrcHeaderFldVAl As String = String.Empty
                            SrcHeaderFldVAl = ds.Tables("LMDocData").Rows(j).Item("SrcHeaderFldVAl")


                            Dim SfldmappingVal As String = String.Empty
                            SfldmappingVal = ds.Tables("LMDocData").Rows(j).Item("Sfldmapping")

                            Uptstr = "Update mmm_mst_master set UPDATEDDATE=getdate()" & strRes & " where eid=" & eid & " and " & TFldmapping & "='" & SfldmappingVal & "' and  documenttype='" & TDocType & "' and reftid=" & ds.Tables("LMDocData").Rows(j).Item("tid") & ";"
                            da.SelectCommand.CommandText = Uptstr.ToString()
                            res = da.SelectCommand.ExecuteScalar()

                            ob.HistoryT(eid, res, UID, TDocType, "MMM_MST_MASTER", "EDIT", con, tran)



                        Next


                    End If

                End If

            Else

                da.Dispose()
                da.Dispose()
                Return "fail"
                Return "NA"
            End If

        Catch ex As Exception
            Throw
        Finally
            If Not da Is Nothing Then
                da.Dispose()
            End If

            'If Not con Is Nothing Then
            '    con.Close()
            '    con.Dispose()
            'End If
        End Try
        Return "Success"
    End Function

    Public Function CheckLMSettings(ByVal eid As Integer, ByVal DOCType As String, ByVal UID As String, ByVal DocID As Integer, ByVal con As SqlConnection, ByVal tran As SqlTransaction) As String
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        'Dim con1 As SqlConnection = New SqlConnection(conStr)
        Dim ob As New DynamicForm
        Dim da As SqlDataAdapter = New SqlDataAdapter("", con)
        da.SelectCommand.Transaction = tran
        'Chandni
        Try

            Dim con1 As SqlConnection = New SqlConnection(conStr)
            Dim da1 As SqlDataAdapter = New SqlDataAdapter("select lm.Tid,TDocType,SDocType,IsActiveStatus,wfstatus,lm.ChildDocType  as MChildDocType,TFld,SFld,Ordering,lmfm.ChildDocType,lmfm.BaseDocName,DocType,LMTid from  MMM_MST_LMsetting lm  inner join  MMM_MST_LMFieldMappingsetting lmfm on lm.Tid=lmfm.LMTid   where  lm.eid=" & eid & "   and IsActiveStatus=1  and lmfm.eid=" & eid & " and lm.SDocType='" & DOCType & "'", con1)

            Dim ds As New DataSet
            Dim dt As New DataTable
            da1.Fill(ds, "LMSettingData")

            Dim Fldstr As String = ""
            If ds.Tables("LMSettingData").Rows.Count <> 0 Then


                Dim insStr As String = String.Empty
                Dim UpdateStr As String = String.Empty
                Dim res As String = ""
                Dim Upres As String = ""
                Dim MasterRes As String = ""
                Dim ChildDocT As String = String.Empty
                Dim TDocType As String = String.Empty

                Dim dtdata As New DataTable
                Dim dtdataHeader As New DataTable
                Dim dtdataSource As New DataTable
                Dim InsertQryStr As String = String.Empty
                Dim UpdateQryStr As String = String.Empty
                If ds.Tables("LMSettingData").Rows.Count > 0 Then

                    Dim SDocnames As List(Of String) = (From row In ds.Tables("LMSettingData").AsEnumerable()
                                                        Select row.Field(Of String)("BaseDocName") Distinct).ToList()
                    Dim strTFlds As String = String.Empty
                    Dim strHTFlds As String = String.Empty
                    For f As Integer = 0 To SDocnames.Count - 1
                        If dtdataSource.Rows.Count > 0 Then
                            dtdataSource = Nothing
                        End If
                        Dim dvs As DataView = ds.Tables("LMSettingData").DefaultView
                        dvs.RowFilter = "BaseDocName='" & SDocnames(f) & "'"
                        dtdataSource = dvs.ToTable()
                        If dtdataSource.Rows.Count > 0 Then
                            ChildDocT = ""
                            TDocType = ""
                            If SDocnames(f) = DOCType Then

                                Dim strSFlds As String = String.Empty
                                If dtdataHeader.Rows.Count > 0 Then
                                    dtdataHeader = Nothing
                                End If
                                If dtdata.Rows.Count > 0 Then
                                    dtdata = Nothing
                                End If
                                Dim strHSFlds As String = String.Empty
                                dtdata = dtdataSource.[Select]("DocType ='Line'").CopyToDataTable()
                                dtdataHeader = dtdataSource.[Select]("DocType ='Header'").CopyToDataTable()
                                strHTFlds = ""
                                If dtdataHeader.Rows.Count > 0 Then
                                    strHTFlds += String.Join(",", (From row In dtdataHeader.Rows Select CType(row.Item("TFld"), String)).ToArray)
                                    strHSFlds += String.Join(",d.", (From row In dtdataHeader.Rows Select CType(row.Item("SFld"), String)).ToArray)
                                    Dim strHSFldsArr As String()
                                    strHSFldsArr = strHSFlds.Split(",")
                                    Dim value As String = String.Join(",',',", strHSFldsArr)
                                    strHSFlds = "d." + value
                                End If

                                If dtdata.Rows.Count > 0 Then

                                    strTFlds += String.Join(",", (From row In dtdata.Rows Select CType(row.Item("TFld"), String)).ToArray)
                                    strSFlds += String.Join(",i.", (From row In dtdata.Rows Select CType(row.Item("SFld"), String)).ToArray)
                                    Dim strSFldsArr As String()
                                    strSFldsArr = strSFlds.Split(",")
                                    Dim valueL As String = String.Join(",',',", strSFldsArr)
                                    strSFlds = "i." + valueL + ",',',"

                                End If
                                TDocType = dtdata.Rows(0).Item("TDocType")
                                ChildDocT = dtdata.Rows(0).Item("ChildDocType")
                                Dim BaseDocName As String = dtdata.Rows(0).Item("BaseDocName")

                                InsertQryStr = " select i.tid, concat(" & strSFlds + strHSFlds & ") as TFldVAl  from mmm_mst_doc d inner join mmm_mst_doc_item i on d.tid=i.DOCID where d.documenttype='" & BaseDocName & "' and  i.documenttype='" & ChildDocT & "' and d.eid=" & eid & "  and i.docid=" & DocID & "; "
                                da.SelectCommand.CommandText = InsertQryStr
                                da.Fill(ds, "LMDocData")
                                If ds.Tables("LMDocData").Rows.Count > 0 Then

                                    For j As Integer = 0 To ds.Tables("LMDocData").Rows.Count - 1

                                        Dim LMFldsVal As String = String.Empty
                                        LMFldsVal = ds.Tables("LMDocData").Rows(j).Item("TFldVAl")
                                        Dim LMFldsValArr As String()
                                        LMFldsValArr = LMFldsVal.Split(",")
                                        Dim value As String = String.Join("','", LMFldsValArr)
                                        value = "'" + value + "'"


                                        insStr = "insert into mmm_mst_master (documenttype,EID,ISAUTH,ADATE,CREATEDBY,UPDATEDDATE,LASTUPDATE," & strTFlds & "," & strHTFlds & ",refTid) values ('" & TDocType & "'," & eid & ",1,getdate(),'" & UID & "',getdate(),getdate()," & value & "," & ds.Tables("LMDocData").Rows(j).Item("tid") & " );"

                                        da.SelectCommand.CommandText = insStr.ToString() + ";Select @@identity"
                                        da.SelectCommand.Transaction = tran
                                        If con.State = ConnectionState.Closed Then
                                            con.Open()
                                        End If
                                        res = da.SelectCommand.ExecuteScalar()
                                        MasterRes = MasterRes + res + ","

                                        ob.HistoryT(eid, res, UID, TDocType, "MMM_MST_MASTER", "ADD", con, tran)
                                        'Execute Trigger
                                        Try
                                            Dim FormName As String = TDocType
                                            '     Dim EID As Integer = 0
                                            If (res > 0) And (FormName <> "") Then
                                                '  Trigger.ExecuteTriggerT(FormName, eid, res, con, tran)
                                            End If
                                        Catch ex As Exception
                                            Throw
                                        End Try

                                    Next
                                    '    End If
                                End If
                            ElseIf SDocnames(f) <> DOCType Then
                                If dtdataHeader.Rows.Count > 0 Then
                                    dtdataHeader = Nothing
                                End If
                                If dtdata.Rows.Count > 0 Then
                                    dtdata = Nothing
                                End If

                                Dim strSFlds As String = String.Empty
                                Dim strHSFlds As String = String.Empty
                                strHTFlds = ""
                                strTFlds = ""
                                dtdata = dtdataSource.[Select]("DocType ='Line'").CopyToDataTable()
                                dtdataHeader = dtdataSource.[Select]("DocType ='Header'").CopyToDataTable()

                                If dtdataHeader.Rows.Count > 0 Then

                                    strHTFlds += String.Join(",", (From row In dtdataHeader.Rows Select CType(row.Item("TFld"), String)).ToArray)
                                    strHSFlds += String.Join(",d.", (From row In dtdataHeader.Rows Select CType(row.Item("SFld"), String)).ToArray)
                                    Dim strHSFldsArr As String()
                                    strHSFldsArr = strHSFlds.Split(",")
                                    Dim value As String = String.Join(",',',", strHSFldsArr)
                                    strHSFlds = "d." + value
                                End If

                                If dtdata.Rows.Count > 0 Then

                                    strTFlds += String.Join(",", (From row In dtdata.Rows Select CType(row.Item("TFld"), String)).ToArray)
                                    strSFlds += String.Join(",i.", (From row In dtdata.Rows Select CType(row.Item("SFld"), String)).ToArray)
                                    Dim strSFldsArr As String()
                                    strSFldsArr = strSFlds.Split(",")
                                    Dim valueL As String = String.Join(",',',", strSFldsArr)
                                    strSFlds = "i." + valueL + ",',',"

                                End If
                                TDocType = dtdata.Rows(0).Item("TDocType")
                                ChildDocT = dtdata.Rows(0).Item("ChildDocType")
                                Dim BaseDocName As String = dtdata.Rows(0).Item("BaseDocName")

                                UpdateQryStr = ""
                                Dim ValueinsStr As String = ""
                                ValueinsStr = "select p.FieldMapping as pFieldMapping, p.FieldType as pFieldType, p.Dropdown as pDropdown,c.FieldMapping as cFieldMapping from mmm_mst_fields p inner join mmm_mst_fields c on p.displayName=c.displayName where p.eid=" & eid & " and p.Documenttype='" & DOCType & "' and c.Documenttype='" & ChildDocT & "' ;"
                                da.SelectCommand.CommandText = ValueinsStr
                                da.Fill(ds, "ValueinsStr")

                                Dim MasterDAta As String = ""
                                MasterDAta = "select * from mmm_mst_master  where  eid=" & eid & " and  Documenttype='" & TDocType & "' and tid in (" & MasterRes.TrimEnd(",") & ") ;"
                                da.SelectCommand.CommandText = MasterDAta
                                da.Fill(ds, "MasterDAta")



                                If ds.Tables("MasterDAta").Rows.Count > 0 Then

                                    For j As Integer = 0 To ds.Tables("MasterDAta").Rows.Count - 1
                                        Dim FldinsStr As String = ""
                                        Dim fldtype As String = ""
                                        If ds.Tables("ValueinsStr").Rows(0).Item("pFieldType") = "Drop Down" Then
                                            fldtype = "dms.udf_split('" & ds.Tables("ValueinsStr").Rows(0).Item("pDropdown").ToString() & "'," + ds.Tables("ValueinsStr").Rows(0).Item("pFieldMapping").ToString() + ")"
                                        Else
                                            fldtype = ds.Tables("ValueinsStr").Rows(0).Item("pFieldMapping").ToString()
                                        End If
                                        FldinsStr = "select " & fldtype & " as fldval from mmm_mst_doc   where  eid=" & eid & " and  Documenttype='" & DOCType & "' and tid=" & DocID & " ;"
                                        da.SelectCommand.CommandText = FldinsStr
                                        da.Fill(ds, "FldinsStr")
                                        If ds.Tables("FldinsStr").Rows.Count > 0 Then
                                            UpdateQryStr = " select i.tid, concat(" & strSFlds + strHSFlds & ") as TFldVAl  from mmm_mst_doc d inner join mmm_mst_doc_item i on d.tid=i.DOCID where d.documenttype='" & BaseDocName & "' and  i.documenttype='" & ChildDocT & "' and d.eid=" & eid & "  and i." & ds.Tables("ValueinsStr").Rows(0).Item("cFieldMapping").ToString() & "='" & ds.Tables("FldinsStr").Rows(0).Item("fldval").ToString() & "'; "
                                            da.SelectCommand.CommandText = UpdateQryStr
                                            da.Fill(ds, "UpdateLMDocData")

                                            If ds.Tables("UpdateLMDocData").Rows.Count > 0 Then
                                                'Fields
                                                Dim FieldStr As String = strTFlds + "," + strHTFlds
                                                Dim FieldArr As String() = FieldStr.Split(",")
                                                Dim Field As String = String.Join("=,", FieldArr)
                                                Field = Field + "="
                                                'Value
                                                Dim LMFldsVal As String = String.Empty
                                                LMFldsVal = ds.Tables("UpdateLMDocData").Rows(j).Item("TFldVAl")
                                                Dim LMFldsValArr As String()
                                                LMFldsValArr = LMFldsVal.Split(",")
                                                Dim valuestr As String = String.Join("','", LMFldsValArr)
                                                valuestr = "'" + valuestr + "'"
                                                Dim ValueArr As String() = valuestr.Split(",")

                                                Dim joinarr = Field.Split(",")
                                                Dim Value As String = String.Join(",", ValueArr)
                                                Dim strValue As String = ""
                                                For k As Integer = 0 To joinarr.Length - 1
                                                    For l As Integer = k To ValueArr.Length - 1
                                                        strValue = strValue + joinarr(k) + ValueArr(l) + ","
                                                        Exit For
                                                    Next
                                                    Dim strValu1e As String = ""
                                                Next

                                                UpdateStr = "Update mmm_mst_master set UPDATEDDATE=getdate(),LASTUPDATE=getdate()," & strValue.TrimEnd(",") & " where tid=" & ds.Tables("MasterDAta").Rows(j).Item("Tid") & " and eid=" & eid & " and documenttype='" & TDocType & "' ;"

                                                da.SelectCommand.CommandText = UpdateStr.ToString()
                                                da.SelectCommand.Transaction = tran
                                                If con.State = ConnectionState.Closed Then
                                                    con.Open()
                                                End If
                                                Upres = da.SelectCommand.ExecuteScalar()



                                            End If

                                        End If

                                        'calculating backdated data for Lease Amendment Document
                                        If DOCType = "Lease Amendment Document" Then

                                            Dim FieldData As New DataTable

                                            If FieldData.Rows.Count > 0 Then
                                                FieldData.Clear()
                                            End If
                                            'Collecting the field data
                                            da = New SqlDataAdapter("SELECT FF.*,F.*  FROM MMM_MST_FIELDS FF left outer JOIN MMM_MST_FORMS F on F.FormName = FF.DocumentType and F.EID = FF.EID where (FF.isactive=1 or Fieldtype='auto number' or fieldtype='New Auto Number') and F.EID=" & eid.ToString() & " and FormName = '" & Convert.ToString(DOCType) & "' order by displayOrder", con)
                                            da.SelectCommand.CommandType = CommandType.Text
                                            da.SelectCommand.Transaction = tran
                                            da.Fill(ds, "fields")
                                            FieldData = ds.Tables("fields")

                                            Dim AmendmentSdt As Date
                                            Dim LeaseSdt As Date
                                            Dim AmendmentSdate As String = String.Empty
                                            Dim LeaseSdate As String = String.Empty
                                            Dim LeaseSDocNo As String = String.Empty
                                            Dim rowln As DataRow() = ds.Tables("fields").Select("DisplayName='Lease Doc no' and Datatype='text' and Documenttype='" & DOCType & "'")
                                            If rowln.Length > 0 Then
                                                For Each CField As DataRow In rowln
                                                    LeaseSDocNo = CField.Item("FieldMapping").ToString
                                                Next
                                            End If

                                            Dim rowASdt As DataRow() = ds.Tables("fields").Select("DisplayName='Amendment Start Date' and Datatype='Datetime' and Documenttype='" & DOCType & "'")
                                            If rowASdt.Length > 0 Then
                                                For Each CField As DataRow In rowASdt
                                                    AmendmentSdate = CField.Item("FieldMapping").ToString
                                                Next
                                            End If

                                            Dim rowLSdt As DataRow() = ds.Tables("fields").Select("DisplayName='Lease Start Date' and Datatype='Datetime' and Documenttype='" & DOCType & "'")
                                            If rowLSdt.Length > 0 Then
                                                For Each CField As DataRow In rowLSdt
                                                    LeaseSdate = CField.Item("FieldMapping").ToString
                                                Next
                                            End If


                                            Dim ChildItemdocType As String = String.Empty
                                            Dim rowLSCI As DataRow() = ds.Tables("fields").Select("FieldType='Child Item' and Documenttype='" & DOCType & "'")
                                            If rowLSCI.Length > 0 Then
                                                For Each CField As DataRow In rowLSCI
                                                    ChildItemdocType = CField.Item("DropDown").ToString
                                                Next
                                            End If

                                            'Collecting the field data
                                            da = New SqlDataAdapter("SELECT FF.*,F.*  FROM MMM_MST_FIELDS FF left outer JOIN MMM_MST_FORMS F on F.FormName = FF.DocumentType and F.EID = FF.EID where (FF.isactive=1 or Fieldtype='auto number' or fieldtype='New Auto Number') and F.EID=" & eid.ToString() & " and FormName = '" & Convert.ToString(ChildItemdocType) & "' order by displayOrder", con)
                                            da.SelectCommand.Transaction = tran
                                            da.SelectCommand.CommandType = CommandType.Text
                                            da.Fill(ds, "Childfields")

                                            Dim LALessorsMgAmt As Double = 0
                                            Dim LLessorsMgAmt As Double = 0
                                            Dim LALessorsMgAmtstr As String = ""
                                            Dim LLessorsMgAmtstr As String = ""
                                            Dim rowLASChildItem As DataRow() = ds.Tables("Childfields").Select("DisplayName='Lessors MG Amount Share' and Datatype='Numeric' and Documenttype='" & ChildItemdocType & "'")
                                            If rowLASChildItem.Length > 0 Then
                                                For Each CField As DataRow In rowLASChildItem
                                                    LLessorsMgAmtstr = CField.Item("FieldMapping").ToString
                                                Next
                                            End If

                                            Dim rowLSChildItem As DataRow() = ds.Tables("Childfields").Select("DisplayName='Lessors MG Amount Share.' and Datatype='Numeric' and Documenttype='" & ChildItemdocType & "'")
                                            If rowLSChildItem.Length > 0 Then
                                                For Each CField As DataRow In rowLSChildItem
                                                    LALessorsMgAmtstr = CField.Item("FieldMapping").ToString
                                                Next
                                            End If

                                            da = New SqlDataAdapter("Select dms.udf_split('DOCUMENT-Mou Lease Document-fld50',d." & LeaseSDocNo & ") as LeaseSDocNo, d." & AmendmentSdate & " as AmendmentSdate,d." & LeaseSdate & " as LeaseSdate,i." & LALessorsMgAmtstr & " as LALessorsMgAmt,i." & LLessorsMgAmtstr & " as LLessorsMgAmt from  mmm_mst_doc d inner join mmm_mst_doc_item i on d.tid=i.docid where d.documenttype='" & DOCType & "' and d.eid=" & eid & " and d.tid=" & DocID & "", con)
                                            da.SelectCommand.CommandType = CommandType.Text
                                            da.SelectCommand.Transaction = tran
                                            da.Fill(ds, "LeaseAmendmentData")


                                            If ds.Tables("LeaseAmendmentData").Rows.Count > 0 Then
                                                'Lease No
                                                Dim LDocNo As String = ds.Tables("LeaseAmendmentData").Rows(0).Item("LeaseSDocNo").ToString()
                                                'Calculeting the Amendment lease start date
                                                Dim LADtarr = ds.Tables("LeaseAmendmentData").Rows(0).Item("AmendmentSdate").ToString().Split("/")
                                                Dim LAdate1 As New Date(LADtarr(2), LADtarr(1), LADtarr(0))
                                                LAdate1 = CDate(LAdate1)
                                                AmendmentSdt = Convert.ToDateTime(LAdate1.ToString("MM/dd/yy"))
                                                'Calculeting the Actual lease start date
                                                Dim LDtarr = ds.Tables("LeaseAmendmentData").Rows(0).Item("LeaseSdate").ToString().Split("/")
                                                Dim Ldate1 As New Date(LDtarr(2), LDtarr(1), LDtarr(0))
                                                Ldate1 = CDate(Ldate1)
                                                LeaseSdt = Convert.ToDateTime(Ldate1.ToString("MM/dd/yy"))

                                                Dim TotalMGAmt As Double = 0
                                                'checking backdated data of Lease amendment
                                                If AmendmentSdt > LeaseSdt And AmendmentSdt < Date.Now Then
                                                    Dim AlreadyEistData As New DataTable
                                                    Dim MGAmtDT As Double = 0
                                                    Dim Fieldmap As String = String.Empty
                                                    Dim EscFieldmap As String = String.Empty
                                                    'Collecting the field data
                                                    da = New SqlDataAdapter("SELECT FF.*,F.*  FROM MMM_MST_FIELDS FF left outer JOIN MMM_MST_FORMS F on F.FormName = FF.DocumentType and F.EID = FF.EID where (FF.isactive=1 or Fieldtype='auto number' or fieldtype='New Auto Number') and F.EID=" & eid.ToString() & " and FormName = 'Rental Invoice' order by displayOrder", con)
                                                    da.SelectCommand.Transaction = tran
                                                    da.SelectCommand.CommandType = CommandType.Text
                                                    da.Fill(ds, "Rentalfields")

                                                    Dim rowRCD As DataRow() = ds.Tables("Rentalfields").Select("DisplayName='Next Invoice Creation Date' and Datatype='Datetime'")
                                                    Dim rowECD As DataRow() = ds.Tables("Rentalfields").Select("DisplayName='Escalation Date' and Datatype='Datetime'")
                                                    If rowRCD.Length > 0 Then
                                                        For Each CField As DataRow In rowRCD
                                                            Fieldmap = CField.Item("FieldMapping").ToString()
                                                        Next
                                                    End If
                                                    If rowECD.Length > 0 Then
                                                        For Each CField As DataRow In rowECD
                                                            EscFieldmap = CField.Item("FieldMapping").ToString()
                                                        Next
                                                    End If
                                                    da.SelectCommand.CommandText = "Select top 1 " & Fieldmap & "  as InvCreationDt, " & EscFieldmap & " as EscDate from mmm_mst_doc with (nolock) where fld2='" & LDocNo & "' and eid=" & eid & "  and Documenttype='rental invoice' order by tid desc "
                                                    da.Fill(AlreadyEistData)


                                                    Dim InvCreationDtarr = AlreadyEistData.Rows(0).Item("InvCreationDt").ToString().Split("/")
                                                    Dim InvCreationLRIGDTdate1 As New Date(InvCreationDtarr(2), InvCreationDtarr(1), InvCreationDtarr(0))
                                                    InvCreationLRIGDTdate1 = CDate(InvCreationLRIGDTdate1)
                                                    Dim LastInvGenDt As DateTime = Convert.ToDateTime(InvCreationLRIGDTdate1.ToString("MM/dd/yy"))

                                                    Dim LAdiff = DateDiff(DateInterval.Month, AmendmentSdt, LastInvGenDt) + 1 'DateDiff(DateInterval.Month, startDt, endDt)

                                                    If (LAdiff > 0) Then
                                                        LALessorsMgAmt = LAdiff * Convert.ToDouble(ds.Tables("LeaseAmendmentData").Rows(0).Item("LALessorsMgAmt"))
                                                        LLessorsMgAmt = LAdiff * Convert.ToDouble(ds.Tables("LeaseAmendmentData").Rows(0).Item("LLessorsMgAmt"))
                                                    End If


                                                    TotalMGAmt = LALessorsMgAmt - LLessorsMgAmt
                                                    Dim decimalVar As Decimal
                                                    decimalVar = Decimal.Round(TotalMGAmt, 2, MidpointRounding.AwayFromZero)
                                                    decimalVar = Math.Round(decimalVar, 2)
                                                    TotalMGAmt = 0
                                                    TotalMGAmt = decimalVar



                                                    Call AutoInvoice(MasterTid:=ds.Tables("MasterDAta").Rows(j).Item("Tid"), Doctype:="Lease Master", LessorsMGAmt:=TotalMGAmt, trans:=tran, con1:=con)

                                                End If

                                            End If


                                        End If


                                    Next
                                End If
                            End If

                        End If
                    Next

                Else

                    da.Dispose()
                    da.Dispose()
                    Return "fail"
                    Return "NA"
                End If



            End If
        Catch ex As Exception
            Throw
        Finally
            If Not da Is Nothing Then
                da.Dispose()
            End If

            'If Not con Is Nothing Then
            '    con.Close()
            '    con.Dispose()
            'End If
        End Try
        Return "Success"
    End Function

    Protected Sub AutoInvoice(ByVal MasterTid As String, ByVal Doctype As String, ByVal LessorsMGAmt As Double, con1 As SqlConnection, trans As SqlTransaction)


        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim insStr As String = String.Empty
        Dim UpdStr As String = String.Empty
        Dim res As Integer = 0
        Dim ob1 As New DMSUtil()
        Dim ob As New DynamicForm
        Dim con As New SqlConnection(conStr)
        Dim tran As SqlTransaction = Nothing
        Dim da As SqlDataAdapter = New SqlDataAdapter("", con)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("", con1)
        ' da.SelectCommand.Transaction = tran
        Try

            da = New SqlDataAdapter("select convert(varchar(100),LastInvoceGenrationDate,103) as LastInvoceGenrationDate,Tid,TDocType,SDocType,IsActiveStatus,LeaseType,EID,StartDateFld,EndDateFld,FrequencyFld,PeriodFld=0,RentalFld,SDFld,CAMFld,RegistrationDateFld,IsDoc_Master,SourceIsDoc_Master,RentFreePeriodFld,RentEsc,CAMEsc,SDmonths,RentEscptage	,CamEscptage from   MMM_MST_AutoInvoiceSetting with(nolock) where  IsActiveStatus=1 and Sdoctype='" & Doctype & "'", con)
            '  da.SelectCommand.Transaction = tran
            Dim ds As New DataSet
            Dim dt As New DataTable
            da.Fill(ds, "AutoInvSettingData")
            Dim StartDateFld As String = String.Empty
            Dim EndDateFld As String = String.Empty
            Dim FrequencyFld As String = String.Empty
            Dim PeriodFld As Int16 = 0
            Dim RentalFld As String = String.Empty
            Dim RegistrationDateFld As String = String.Empty
            Dim SchedulerTidID As String = String.Empty
            Dim RentEsc As String = String.Empty
            Dim CAMEsc As String = String.Empty
            Dim SDmonths As String = String.Empty
            Dim RentEscPtage As String = String.Empty
            Dim CAMEscPtage As String = String.Empty
            Dim Fldstr As String = ""
            Dim strTFlds As String = String.Empty
            Dim strSFlds As String = String.Empty
            Dim strHTFlds As String = String.Empty
            Dim strHSFlds As String = String.Empty
            Dim strHSFldsArr As String()
            Dim RentFreePeriodFlds As String = String.Empty
            Dim EID As String = String.Empty
            Dim LeaseType As String = String.Empty
            Dim MGAmount As String = String.Empty
            If ds.Tables("AutoInvSettingData").Rows.Count <> 0 Then
                For i As Integer = 0 To ds.Tables("AutoInvSettingData").Rows.Count - 1


                    StartDateFld = Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("StartDateFld"))
                    EndDateFld = Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("EndDateFld"))
                    FrequencyFld = Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("FrequencyFld"))
                    RentalFld = Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("RentalFld"))
                    RegistrationDateFld = Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("RegistrationDateFld"))
                    RentFreePeriodFlds = Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("RentFreePeriodFld"))
                    EID = Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("EID"))
                    SchedulerTidID = Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("Tid"))
                    LeaseType = Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("LeaseType"))
                    MGAmount = Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("RentalFld"))
                    RentEsc = Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("RentEsc"))
                    CAMEsc = Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("CAMEsc"))
                    RentEscPtage = Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("RentEscPtage"))
                    CAMEscPtage = Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("CAMEscPtage"))
                    SDmonths = Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("SDMonths"))

                    Dim FldsVal As String = String.Empty
                    Dim SchedulerCheck As Boolean
                    '  SchedulerCheck = Scheduler(SchedulerTidID)
                    SchedulerCheck = True

                    Dim FldsValArr As String()
                    Dim value As String = ""

                    da = New SqlDataAdapter("select  F.Tid,F.TFld,F.SFld,F.SDocType,F.AutoInvTid,F.EID,F.TFldName,F.sFldName,F.Leasetype,F.TDocType  from    MMM_MST_AutoInvoiceSetting C inner join MMM_MST_AutoInvFieldSetting F on c.Tid=F.AutoInvTid where C.eid=" & EID & " and C.IsActiveStatus=1 and F.AutoInvTid=" & Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("Tid")) & "", con)
                    'da.SelectCommand.Transaction = tran
                    da.Fill(ds, "AutoInvFieldData")
                    Dim valueL As String

                    Dim AutoInvDocData As New DataTable

                    Dim fieldmapping As String = ""
                    Dim fieldmappingINVdt As String = ""
                    Dim fieldmappingLeaseDocdt As String = ""
                    If AutoInvDocData.Rows.Count <> 0 Then
                        ds.Tables("AutoInvDocData").Clear()
                    End If


                    da.SelectCommand.CommandType = CommandType.Text
                    da.SelectCommand.Parameters.Clear()
                    da = New SqlDataAdapter("select fieldmapping from MMM_MST_FIELDS with(nolock) where  Eid=" & EID & " and DocumentType='" & Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("SDocType")) & "' and Displayname='Rent Invoice Generation Date' and datatype='Datetime'", con)
                    Dim dtDtF As New DataTable
                    'da.SelectCommand.Transaction = tran
                    da.Fill(dtDtF)
                    If dtDtF.Rows.Count > 0 Then
                        fieldmappingINVdt = dtDtF.Rows(0).Item("fieldmapping")
                    End If
                    da.SelectCommand.CommandType = CommandType.Text
                    da.SelectCommand.Parameters.Clear()
                    da = New SqlDataAdapter("select fieldmapping from MMM_MST_FIELDS with(nolock) where  Eid=" & EID & " and DocumentType='" & Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("SDocType")) & "' and Displayname='Lease Doc No' and datatype='Text'", con)
                    Dim LDocdtDtF As New DataTable
                    '  da.SelectCommand.Transaction = tran
                    da.Fill(LDocdtDtF)
                    If LDocdtDtF.Rows.Count > 0 Then
                        fieldmappingLeaseDocdt = LDocdtDtF.Rows(0).Item("fieldmapping")
                    End If


                    'Rental Inv Gen data
                    da = New SqlDataAdapter("select   " & LeaseType & "  as leasetype, " & fieldmappingLeaseDocdt & " as [LeaseDocNo], " & fieldmappingINVdt & " as LRentInvGenDate,  tid as tid,convert(varchar, convert(datetime," & RegistrationDateFld & ", 3), 103)  as RegistrationDate ," & RentFreePeriodFlds & " as RentFreePeriod," & FrequencyFld & " as [RentPaymentCycle],fld49 as [RentFreeFitOutStartDate],fld50 as [RentFreeFitOutEndDate]," & RentFreePeriodFlds & " as [RentFreeDays]," & StartDateFld & " as LStartDate , " & EndDateFld & " as LEndDate," & MGAmount & " as LMGAmount," & RentEsc & " As RentEsc," & CAMEsc & " as CAMEsc," & SDmonths & " as SDMonths," & RentEscPtage & " as RentEscPtage," & CAMEscPtage & " as CAMEscPtage,fld57 as [LessorsPropertyShare],fld41 as [RentType],fld76 as [CamPaymentcycle],fld10 as SDAmt,fld48 as CAMCommDate,fld63 as CAMAmt,'YES' as AmendmentFlag from MMM_MST_MASTER  with(nolock)   where eid=" & EID & " and Documenttype='" & Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("SDocType")) & "' and  convert(date, getdate(), 3) between   convert(date," & StartDateFld & ", 3) and  convert(date, " & EndDateFld & ", 3)    and reftid<>''   and tid in (" & MasterTid & ")", con)
                    'da.SelectCommand.Transaction = tran

                    da.Fill(ds, "AutoInvDocData")
                    AutoInvDocData = ds.Tables("AutoInvDocData")
                    Dim SourceDocData As New DataTable
                    Dim RDOCIDData As New DataTable
                    Dim FieldData As New DataTable

                    Dim LDocNo As String = String.Empty
                    Dim LStartDate As String = String.Empty
                    Dim LENDDate As String = String.Empty
                    Dim LFitOutStartDate As String = String.Empty
                    Dim LFitOutEndDate As String = String.Empty
                    Dim LeaseComDate As String = String.Empty
                    Dim CAMComDate As String = String.Empty
                    Dim LRentPayment As String = String.Empty
                    Dim LRentFreedays As String = String.Empty
                    Dim LRentInvGenDate As String = String.Empty
                    Dim LRentESC As String = String.Empty
                    Dim LCAMEsc As String = String.Empty
                    Dim LRentESCPtage As String = String.Empty
                    Dim LCAMEscPtage As String = String.Empty
                    Dim LSDMonths As String = String.Empty
                    Dim LPropershare As String = String.Empty
                    Dim LRenttype As String = String.Empty
                    Dim LCAMRentCycletype As String = String.Empty
                    Dim LAmendmentFlag As String = String.Empty
                    Dim LMGAmt As Double = 0
                    Dim LSDAmt As Double = 0
                    Dim LCAMAmt As Double = 0


                    If ds.Tables("AutoInvDocData").Rows.Count <> 0 Then

                        For j As Integer = 0 To ds.Tables("AutoInvDocData").Rows.Count - 1

                            LDocNo = ds.Tables("AutoInvDocData").Rows(j).Item("LeaseDocNo")
                            LStartDate = ds.Tables("AutoInvDocData").Rows(j).Item("LStartDate")
                            LENDDate = ds.Tables("AutoInvDocData").Rows(j).Item("LEndDate")
                            LFitOutStartDate = ds.Tables("AutoInvDocData").Rows(j).Item("RentFreeFitOutStartDate")
                            LFitOutEndDate = ds.Tables("AutoInvDocData").Rows(j).Item("RentFreeFitOutEndDate")
                            LRentPayment = ds.Tables("AutoInvDocData").Rows(j).Item("RentPaymentCycle")
                            LRentInvGenDate = ds.Tables("AutoInvDocData").Rows(j).Item("LRentInvGenDate")
                            LMGAmt = ds.Tables("AutoInvDocData").Rows(j).Item("LMGAmount")
                            LSDAmt = ds.Tables("AutoInvDocData").Rows(j).Item("SDAmt")
                            LRentESC = ds.Tables("AutoInvDocData").Rows(j).Item("RentESC")
                            LCAMEsc = ds.Tables("AutoInvDocData").Rows(j).Item("CAMESC")
                            LRentESCPtage = ds.Tables("AutoInvDocData").Rows(j).Item("RentESCPtage")
                            LCAMEscPtage = ds.Tables("AutoInvDocData").Rows(j).Item("CAMESCPtage")
                            LSDMonths = ds.Tables("AutoInvDocData").Rows(j).Item("SdMonths")
                            LPropershare = ds.Tables("AutoInvDocData").Rows(j).Item("LessorsPropertyShare")
                            LRenttype = ds.Tables("AutoInvDocData").Rows(j).Item("Renttype")
                            LCAMRentCycletype = ds.Tables("AutoInvDocData").Rows(j).Item("CamPaymentcycle")
                            CAMComDate = ds.Tables("AutoInvDocData").Rows(j).Item("CAMCommDate")
                            LCAMAmt = ds.Tables("AutoInvDocData").Rows(j).Item("CAMAmt")
                            LAmendmentFlag = ds.Tables("AutoInvDocData").Rows(j).Item("AmendmentFlag")



                            If ds.Tables("AutoInvDocData").Rows(j).Item("leasetype") = "1491591" Or ds.Tables("AutoInvDocData").Rows(j).Item("leasetype") = "1570943" Or ds.Tables("AutoInvDocData").Rows(j).Item("leasetype") = "1554309" Or ds.Tables("AutoInvDocData").Rows(j).Item("leasetype") = "1570941" Then 'Lease Type Master
                                Dim names As List(Of String) = (From row In ds.Tables("AutoInvFieldData").AsEnumerable()
                                                                Select row.Field(Of String)("TDocType") Distinct).ToList()

                                For f As Integer = 0 To names.Count - 1
                                    If names(f) = "Rental Invoice" Then

                                        Dim dvs As DataView = ds.Tables("AutoInvFieldData").DefaultView
                                        dvs.RowFilter = "TDocType='" & names(f) & "'"

                                        Dim filteredTable As New DataTable()
                                        filteredTable = dvs.ToTable()
                                        If filteredTable.Rows.Count <> 0 Then
                                            strTFlds = ""
                                            strSFlds = ""
                                            strTFlds += String.Join(",", (From row In filteredTable.Rows Select CType(row.Item("TFld"), String)).ToArray)
                                            strSFlds += String.Join(",", (From row In filteredTable.Rows Select CType(row.Item("SFld"), String)).ToArray)
                                            strHSFldsArr = Nothing
                                            strHSFldsArr = strSFlds.Split(",")
                                            valueL = String.Join(",',',", strHSFldsArr)
                                            strHSFlds = valueL
                                        End If
                                        If SourceDocData.Rows.Count > 0 Then
                                            SourceDocData.Clear()
                                        End If
                                        If FieldData.Rows.Count > 0 Then
                                            FieldData.Clear()
                                        End If
                                        If con.State = ConnectionState.Closed Then
                                            con.Open()
                                        End If
                                        Dim Finaldate As String = String.Empty

                                        oda = New SqlDataAdapter(" select fld2 as [LeaseDocNo],fld11 as [Name of Lessor],fld7 as [Rental Amount],tid as RDOCID,concat(" & strHSFlds & ") as TFldVAl from " & Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("SourceIsDoc_Master")) & "   where documenttype='" & Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("SDocType")) & "'  and eid=" & EID & " and tid=" & ds.Tables("AutoInvDocData").Rows(j).Item("Tid") & " ", con1)
                                        oda.SelectCommand.CommandType = CommandType.Text
                                        oda.SelectCommand.Transaction = trans
                                        oda.Fill(ds, "SourceDocData")

                                        SourceDocData = ds.Tables("SourceDocData")
                                        Dim RDOCID As String = ""
                                        Dim LeaseDocNo As String = ""
                                        If ds.Tables("SourceDocData").Rows.Count > 0 Then
                                            FldsVal = ""
                                            FldsVal = ds.Tables("SourceDocData").Rows(0).Item("TFldVAl")
                                            FldsValArr = FldsVal.Split(",")
                                            value = ""
                                            value = String.Join("','", FldsValArr)
                                            value = "'" + value + "'"
                                            RDOCID = Convert.ToString(ds.Tables("SourceDocData").Rows(0).Item("RDOCID"))
                                            LeaseDocNo = Convert.ToString(ds.Tables("SourceDocData").Rows(0).Item("LeaseDocNo"))
                                        End If
                                        If ds.Tables("SourceDocData").Rows.Count > 0 Then
                                            If RDOCIDData.Rows.Count > 0 Then
                                                RDOCIDData.Clear()
                                            End If
                                            'Name of Lessor
                                            If Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("IsDoc_Master")) = "MMM_MST_MASTER" Then
                                                da = New SqlDataAdapter("DECLARE @DATE DATETIME ;SET @DATE= getdate();  select * from MMM_MST_MASTER where RefTid=" & RDOCID & " and documenttype='" & Convert.ToString(names(f)) & "'  --and adate between @DATE-DAY(@DATE)+1 and EOMONTH(@DATE) ", con)
                                            Else
                                                da = New SqlDataAdapter("DECLARE @DATE DATETIME ;SET @DATE= getdate(); select * from MMM_MST_DOC where fld2='" & LeaseDocNo & "' and documenttype='" & Convert.ToString(names(f)) & "' --and fld11='" & Convert.ToString(ds.Tables("SourceDocData").Rows(0).Item("Name of Lessor")) & "' --and  adate between @DATE-DAY(@DATE)+1 and EOMONTH(@DATE);", con)
                                            End If
                                            da.SelectCommand.CommandType = CommandType.Text
                                            '  da.SelectCommand.Transaction = tran
                                            da.Fill(ds, "RDOCIDData")
                                            RDOCIDData = ds.Tables("RDOCIDData")
                                            If FieldData.Rows.Count > 0 Then
                                                FieldData.Clear()
                                            End If
                                            ' Collecting the field data
                                            da = New SqlDataAdapter("SELECT FF.*,F.*  FROM MMM_MST_FIELDS FF left outer JOIN MMM_MST_FORMS F on F.FormName = FF.DocumentType and F.EID = FF.EID where (FF.isactive=1 or Fieldtype='auto number' or fieldtype='New Auto Number') and F.EID=" & EID.ToString() & " and FormName = '" & Convert.ToString(names(f)) & "' order by displayOrder", con)
                                            da.SelectCommand.CommandType = CommandType.Text
                                            '  da.SelectCommand.Transaction = tran
                                            da.Fill(ds, "fields")
                                            FieldData = ds.Tables("fields")

                                            If ds.Tables("RDOCIDData").Rows.Count > 0 Then

                                                If LRenttype = 1554651 Then 'Fixed
                                                    If con.State = ConnectionState.Closed Then
                                                        con.Open()
                                                    End If
                                                    tran = con.BeginTransaction()
                                                    Dim FinalInvDate As String = String.Empty
                                                    Dim AlreadyEistData As New DataTable
                                                    Dim MGAmtDT As Double = 0
                                                    Dim Fieldmap As String = String.Empty
                                                    Dim EscFieldmap As String = String.Empty
                                                    If Convert.ToString(names(f)) = "Rental Invoice" Then
                                                        Dim rowRCD As DataRow() = ds.Tables("fields").Select("DisplayName='Next Invoice Creation Date' and Datatype='Datetime'")
                                                        Dim rowECD As DataRow() = ds.Tables("fields").Select("DisplayName='Escalation Date' and Datatype='Datetime'")
                                                        If rowRCD.Length > 0 Then
                                                            For Each CField As DataRow In rowRCD
                                                                Fieldmap = CField.Item("FieldMapping").ToString()
                                                            Next
                                                        End If
                                                        If rowECD.Length > 0 Then
                                                            For Each CField As DataRow In rowECD
                                                                EscFieldmap = CField.Item("FieldMapping").ToString()
                                                            Next
                                                        End If
                                                        oda = New SqlDataAdapter("Select top 1 " & Fieldmap & "  as InvCreationDt, " & EscFieldmap & " as EscDate from mmm_mst_doc with (nolock) where fld2='" & LDocNo & "' and eid=" & EID & "  and Documenttype='" & Convert.ToString(names(f)) & "' order by tid desc ", con1)


                                                    End If
                                                    oda.SelectCommand.Transaction = trans
                                                    If AlreadyEistData.Rows.Count > 0 Then
                                                        AlreadyEistData.Clear()
                                                    End If
                                                    oda.Fill(AlreadyEistData)


                                                    insStr = ""
                                                    If Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("IsDoc_Master")) = "MMM_MST_MASTER" Then

                                                        insStr = "insert into " & Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("IsDoc_Master")) & " (documenttype,EID,ISAUTH,ADATE,CREATEDBY,UPDATEDDATE,LASTUPDATE," & strTFlds & ",reftid) values ('" & Convert.ToString(names(f)) & "'," & EID & ",1,getdate(),'30200',getdate(),getdate()," & value & ",'" & ds.Tables("SourceDocData").Rows(0).Item("RDOCID") & "' );"
                                                    ElseIf Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("IsDoc_Master")) = "MMM_MST_DOC" Then

                                                        insStr = "insert into " & Convert.ToString(ds.Tables("AutoInvSettingData").Rows(i).Item("IsDoc_Master")) & " (documenttype,EID,ISAUTH,ADATE,CREATEDON,LASTUPDATE,OUID," & strTFlds & ",RDOCID) values ('" & Convert.ToString(names(f)) & "'," & EID & ",1,getdate(),getdate(),getdate(),'30200'," & value & ",'" & ds.Tables("SourceDocData").Rows(0).Item("RDOCID") & "' );"
                                                    End If
                                                    da.SelectCommand.CommandText = insStr.ToString() & ";Select @@identity"
                                                    da.SelectCommand.CommandType = CommandType.Text
                                                    da.SelectCommand.Transaction = tran
                                                    If con.State = ConnectionState.Closed Then
                                                        con.Open()
                                                    End If
                                                    res = da.SelectCommand.ExecuteScalar()


                                                    ' End If

                                                    If res <> 0 Then

                                                        Dim rowMGAmt As DataRow() = ds.Tables("fields").Select("DisplayName='MG Amount'  and Datatype='Numeric'")
                                                        If rowMGAmt.Length > 0 Then
                                                            For Each CField As DataRow In rowMGAmt

                                                                Dim upquery As String = "update " & CField.Item("DBTableName").ToString & "  set  " & CField.Item("FieldMapping").ToString & "='" & LessorsMGAmt & "'  where eid=" & EID & " and Documenttype='" & names(f) & "' and tid =" & res & ""
                                                                da.SelectCommand.CommandText = upquery
                                                                da.SelectCommand.Transaction = tran
                                                                da.SelectCommand.CommandType = CommandType.Text
                                                                da.SelectCommand.ExecuteNonQuery()
                                                            Next
                                                        End If

                                                        Dim rowRCD As DataRow() = ds.Tables("fields").Select("DisplayName='Next Invoice Creation Date' and Datatype='Datetime'")
                                                        If rowRCD.Length > 0 Then
                                                            For Each CField As DataRow In rowRCD

                                                                Dim upquery As String = "update " & CField.Item("DBTableName").ToString & "  set  " & CField.Item("FieldMapping").ToString & "='" & AlreadyEistData.Rows(0).Item("InvCreationDt").ToString() & "'  where tid =" & res & ""
                                                                da.SelectCommand.CommandText = upquery
                                                                da.SelectCommand.Transaction = tran
                                                                da.SelectCommand.CommandType = CommandType.Text
                                                                da.SelectCommand.ExecuteNonQuery()

                                                            Next
                                                        End If
                                                        Dim rowECD As DataRow() = ds.Tables("fields").Select("DisplayName='Escalation Date' and Datatype='Datetime' and documenttype='Rental Invoice'")
                                                        If rowECD.Length > 0 Then
                                                            For Each CField As DataRow In rowECD

                                                                Dim upquery As String = "update " & CField.Item("DBTableName").ToString & "  set  " & CField.Item("FieldMapping").ToString & "='" & AlreadyEistData.Rows(0).Item("EscDate").ToString() & "'  where tid =" & res & ""
                                                                da.SelectCommand.CommandText = upquery
                                                                da.SelectCommand.Transaction = tran
                                                                da.SelectCommand.CommandType = CommandType.Text
                                                                da.SelectCommand.ExecuteNonQuery()


                                                            Next
                                                        End If
                                                        ' Call CommonFunctionality(Documenttype:=names(f), EID:=EID, Res:=res, fields:=FieldData, con, tran)
                                                        Call CommonFunctionality(Documenttype:=names(f), EID:=EID, Res:=res, fields:=FieldData, con:=con, tran:=tran)

                                                        ''INSERT INTO HISTORY 
                                                        ob.HistoryT(EID, res, "30200", Convert.ToString(names(f)), "MMM_MST_DOC", "ADD", con, tran)

                                                        Try
                                                            UpdStr = "Update MMM_MST_AutoInvoiceSetting Set LastInvoceGenrationDate=getdate() where eid=" & EID & " and tid=" & ds.Tables("AutoInvSettingData").Rows(i).Item("Tid") & ""
                                                            da.SelectCommand.CommandText = UpdStr.ToString()
                                                            da.SelectCommand.CommandType = CommandType.Text
                                                            da.SelectCommand.ExecuteNonQuery()
                                                            Dim srerd As String = String.Empty
                                                        Catch ex As Exception

                                                        End Try
                                                        tran.Commit()
                                                        'Dim ob1 As New DMSUtil()
                                                        ' Dim res1 As String = String.Empty
                                                        ' Dim ob As New DynamicForm
                                                        'Non transactional Query
                                                        'Check Work Flow
                                                        ob1.TemplateCalling(res, EID, Convert.ToString(names(f)), "CREATED")
                                                        da.SelectCommand.CommandType = CommandType.Text
                                                        da.SelectCommand.Parameters.Clear()
                                                        da.SelectCommand.CommandText = "select isworkflow from mmm_mst_doc where TID=" & res & " and eid='" & EID & "'"
                                                        Dim dt1 As New DataTable
                                                        da.Fill(dt1)
                                                        If dt1.Rows.Count = 1 Then
                                                            If dt1.Rows(0).Item(0).ToString = "1" Then
                                                                ob1.TemplateCalling(res, EID, Convert.ToString(names(f)), "APPROVE")
                                                            End If
                                                        End If

                                                    End If


                                                ElseIf LRenttype = 1554653 Then 'Fixed And Revenue Sharing


                                                End If

                                            End If
                                        End If
                                    End If
                                Next

                            End If

                        Next
                    End If

                Next
            End If
        Catch ex As Exception
            If Not tran Is Nothing Then
                tran.Rollback()
            End If
        Finally
            If Not da Is Nothing Then
                da.Dispose()
            End If
            If Not con Is Nothing Then
                con.Close()
            End If
        End Try

    End Sub

    Public Function CheckLMSettingsChildInsertion(ByVal eid As Integer, ByVal DOCType As String, ByVal UID As String, ByVal DocID As Integer, ByVal con As SqlConnection, ByVal tran As SqlTransaction) As String
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        'Dim con1 As SqlConnection = New SqlConnection(conStr)
        Dim ob As New DynamicForm
        Dim da As SqlDataAdapter = New SqlDataAdapter("", con)
        da.SelectCommand.Transaction = tran

        Try

            Dim con1 As SqlConnection = New SqlConnection(conStr)
            Dim da1 As SqlDataAdapter = New SqlDataAdapter("select lm.Tid,TDocType,SDocType,IsActiveStatus,wfstatus,lm.ChildDocType  as MChildDocType,TFld,SFld,Ordering,lmfm.ChildDocType,DocType,LMTid from  MMM_MST_LMsetting lm  inner join  MMM_MST_LMFieldMappingsetting lmfm on lm.Tid=lmfm.LMTid   where  lm.eid=" & eid & "   and IsActiveStatus=1  and lmfm.eid=" & eid & " and lm.SDocType='" & DOCType & "'", con1)

            Dim ds As New DataSet
            Dim dt As New DataTable
            da1.Fill(ds, "LMSettingData")

            Dim Fldstr As String = ""
            If ds.Tables("LMSettingData").Rows.Count <> 0 Then


                Dim insStr As String = String.Empty
                Dim res As String = ""
                Dim strTFlds As String = String.Empty
                Dim strSFlds As String = String.Empty
                Dim strHTFlds As String = String.Empty
                Dim strHSFlds As String = String.Empty
                Dim ChildDocT As String = String.Empty
                Dim TDocType As String = String.Empty

                Dim dtdata As New DataTable
                Dim dtdataHeader As New DataTable
                If ds.Tables("LMSettingData").Rows.Count > 0 Then
                    dtdata = ds.Tables("LMSettingData").[Select]("DocType = 'Line'").CopyToDataTable()
                    dtdataHeader = ds.Tables("LMSettingData").[Select]("DocType = 'Header'").CopyToDataTable()

                    If dtdataHeader.Rows.Count > 0 Then

                        strHTFlds += String.Join(",", (From row In dtdataHeader.Rows Select CType(row.Item("TFld"), String)).ToArray)
                        strHSFlds += String.Join(",d.", (From row In dtdataHeader.Rows Select CType(row.Item("SFld"), String)).ToArray)
                        Dim strHSFldsArr As String()
                        strHSFldsArr = strHSFlds.Split(",")
                        Dim value As String = String.Join(",',',", strHSFldsArr)
                        strHSFlds = "d." + value
                    End If
                    If dtdata.Rows.Count > 0 Then

                        strTFlds += String.Join(",", (From row In dtdata.Rows Select CType(row.Item("TFld"), String)).ToArray)
                        strSFlds += String.Join(",i.", (From row In dtdata.Rows Select CType(row.Item("SFld"), String)).ToArray)
                        Dim strSFldsArr As String()
                        strSFldsArr = strSFlds.Split(",")
                        Dim valueL As String = String.Join(",',',", strSFldsArr)
                        strSFlds = "i." + valueL + ",',',"
                        'For i As Integer = 0 To dtdata.Rows.Count - 1

                        TDocType = dtdata.Rows(0).Item("TDocType")
                        ChildDocT = dtdata.Rows(0).Item("MChildDocType")
                        da.SelectCommand.CommandText = " select i.tid, concat(" & strSFlds + strHSFlds & ") as TFldVAl from mmm_mst_doc d inner join mmm_mst_doc_item i on d.tid=i.DOCID where d.documenttype='" & DOCType & "' and  i.documenttype='" & ChildDocT & "' and d.eid=" & eid & "  and i.Tid=" & DocID & " "
                        da.Fill(ds, "LMDocData")

                        ' Next

                    End If

                    If ds.Tables("LMDocData").Rows.Count > 0 Then

                        For j As Integer = 0 To ds.Tables("LMDocData").Rows.Count - 1
                            Dim LMFldsVal As String = String.Empty
                            LMFldsVal = ds.Tables("LMDocData").Rows(j).Item("TFldVAl")
                            Dim LMFldsValArr As String()
                            LMFldsValArr = LMFldsVal.Split(",")
                            Dim value As String = String.Join("','", LMFldsValArr)
                            value = "'" + value + "'"


                            insStr = "insert into mmm_mst_master (documenttype,EID,ISAUTH,ADATE,CREATEDBY,UPDATEDDATE,LASTUPDATE," & strTFlds & "," & strHTFlds & ",refTid) values ('" & TDocType & "'," & eid & ",1,getdate(),'" & UID & "',getdate(),getdate()," & value & "," & ds.Tables("LMDocData").Rows(j).Item("tid") & " );"
                            da.SelectCommand.CommandText = insStr.ToString()
                            res = da.SelectCommand.ExecuteScalar()

                            ob.HistoryT(eid, res, UID, TDocType, "MMM_MST_MASTER", "ADD", con, tran)
                            'Execute Trigger
                            Try
                                Dim FormName As String = TDocType
                                '     Dim EID As Integer = 0
                                If (res > 0) And (FormName <> "") Then
                                    Trigger.ExecuteTriggerT(FormName, eid, res, con, tran)
                                End If
                            Catch ex As Exception
                                Throw
                            End Try
                        Next

                    End If

                End If
            Else

                da.Dispose()
                da.Dispose()
                Return "fail"
                Return "NA"
            End If

        Catch ex As Exception
            Throw
        Finally
            If Not da Is Nothing Then
                da.Dispose()
            End If

            'If Not con Is Nothing Then
            '    con.Close()
            '    con.Dispose()
            'End If
        End Try
        Return "Success"
    End Function

    Public Sub CommonFunctionality(ByVal Documenttype As String, ByVal EID As Integer, ByVal Res As Integer, ByVal fields As DataTable, con As SqlConnection, tran As SqlTransaction)

        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con1 As New SqlConnection(conStr)
        'Dim da As SqlDataAdapter = New SqlDataAdapter("", con1)
        Dim da As SqlDataAdapter = New SqlDataAdapter("", con)
        da.SelectCommand.Transaction = tran

        If Res <> 0 Then


            'Fieldtype='Auto Number'
            Dim row As DataRow() = fields.Select("Fieldtype='Auto Number' or Fieldtype='New Auto Number'")
            If row.Length > 0 Then
                For l As Integer = 0 To row.Length - 1
                    Select Case row(l).Item("fieldtype").ToString
                        Case "Auto Number"
                            da.SelectCommand.Parameters.Clear()
                            da.SelectCommand.Transaction = tran
                            da.SelectCommand.CommandText = "usp_GetAutoNoNew"
                            da.SelectCommand.CommandType = CommandType.StoredProcedure
                            da.SelectCommand.Parameters.AddWithValue("Fldid", row(l).Item("fieldid"))
                            da.SelectCommand.Parameters.AddWithValue("docid", Res)
                            da.SelectCommand.Parameters.AddWithValue("fldmapping", row(l).Item("fieldmapping"))
                            da.SelectCommand.Parameters.AddWithValue("FormType", "Document")
                            da.SelectCommand.ExecuteScalar()
                            da.SelectCommand.Parameters.Clear()
                        Case "New Auto Number"
                            da.SelectCommand.Parameters.Clear()
                            da.SelectCommand.Transaction = tran
                            da.SelectCommand.CommandText = "usp_GetAutoNoNew_New"
                            da.SelectCommand.CommandType = CommandType.StoredProcedure
                            da.SelectCommand.Parameters.AddWithValue("Fldid", row(l).Item("fieldid"))
                            da.SelectCommand.Parameters.AddWithValue("SearchFldid", row(l).Item("dropdown").ToString)
                            da.SelectCommand.Parameters.AddWithValue("docid", Res)
                            da.SelectCommand.Parameters.AddWithValue("fldmapping", row(l).Item("fieldmapping"))
                            da.SelectCommand.Parameters.AddWithValue("FormType", "Document")
                            da.SelectCommand.ExecuteScalar()
                            da.SelectCommand.Parameters.Clear()
                    End Select
                Next
            End If


            ' here is recalculate for main form   28 april 2020
            Call Recalculate_CalfieldsonSave(EID, Res, con, tran) 'fOR cALCULATIV fIELD   

            'calculative fields
            Dim CalculativeField() As DataRow = fields.Select("Fieldtype='Formula Field'")
            Dim viewdoc As String = Convert.ToString(Documenttype)
            viewdoc = viewdoc.Replace(" ", "_")
            If CalculativeField.Length > 0 Then
                For Each CField As DataRow In CalculativeField
                    Dim formulaeditorr As New formulaEditor
                    Dim forvalue As String = String.Empty
                    'Coomented By Komal on 28March2014
                    forvalue = formulaeditorr.ExecuteFormulaT(CField.Item("KC_LOGIC"), Res, "v" + EID.ToString + viewdoc, EID.ToString, 0, con, tran)
                    ' forvalue = formulaeditorr.ExecuteFormula(CField.Item("KC_LOGIC"), res, "v" + EID.ToString + viewdoc, EID.ToString, 0)
                    Dim upquery As String = "update " & CField.Item("DBTableName").ToString & "  set  " & CField.Item("FieldMapping").ToString & "='" & forvalue.ToString & "'  where tid =" & Res & ""
                    da.SelectCommand.CommandText = upquery
                    da.SelectCommand.CommandType = CommandType.Text
                    da.SelectCommand.ExecuteNonQuery()
                Next


            End If

            Dim ob1 As New DMSUtil()
            ' Dim res1 As String = String.Empty
            Dim ob As New DynamicForm

            '' insert default first movement of document - by sunil
            da.SelectCommand.CommandText = "InsertDefaultMovement"
            da.SelectCommand.CommandType = CommandType.StoredProcedure
            da.SelectCommand.Parameters.Clear()
            da.SelectCommand.Transaction = tran
            da.SelectCommand.Parameters.AddWithValue("tid", Res)
            da.SelectCommand.Parameters.AddWithValue("CUID", "30200")
            da.SelectCommand.Parameters.AddWithValue("what", "UPLOADED")
            da.SelectCommand.ExecuteNonQuery()

            Dim res12 As String = String.Empty

            res12 = ob1.GetNextUserFromRolematrixT(Res, EID, "30200", "", "30200", con, tran)
            Dim sretMsgArr() As String = res12.Split(":")
            ob.HistoryT(EID, Res, "30200", Convert.ToString(Documenttype), "MMM_MST_DOC", "ADD", con, tran)
            If sretMsgArr(0) = "ARCHIVE" Then
                'Dim Op As New Exportdata()
                'Op.PushdataT(res, sretMsgArr(1), EID, con, tran)
            End If

            '''' check if no skip setting and if not allowed then don't move doc and show msg to user by sunil on 07-Oct
            If sretMsgArr(0).ToUpper() = "NO SKIP" Then
                Dim Noskipmsg As String = "Next Approvar/User not found, please contact Admin"
                'lblMsg.Text = "System Docid is " & fileid & " " & msgAN & "" & "<br/> " & Noskipmsg
                'this code block is added by ajeet kumar for transaction to be rolled back
                ''''tran.Rollback()
                ''''Exit Sub
            Else

            End If
            'Execute Trigger
            Try
                Dim FormName As String = Convert.ToString(Documenttype)
                '     Dim EID As Integer = 0
                If (Res > 0) And (FormName <> "") Then
                    Trigger.ExecuteTriggerT(FormName, EID, Res, con, tran)
                End If
            Catch ex As Exception
                Throw
            End Try


        End If
    End Sub
    Public Sub Recalculate_CalfieldsonSave(ByVal EID As Integer, ByVal docid As Integer, con As SqlConnection, tran As SqlTransaction)
        '''''''''For recalculation of calculative fields, if any ''''''''''''''''
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con1 As New SqlConnection(conStr)
        'Dim da As SqlDataAdapter = New SqlDataAdapter("", con1)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
        oda.SelectCommand.Transaction = tran
        Dim dsscal As New DataSet
        Dim dt5 As New DataTable
        Dim dt6 As New DataTable

        Dim cal_mpng As String = ""
        Dim cal_text As String = ""
        Dim fldmpng As String = ""
        Dim stringf As String = ""
        oda.SelectCommand.CommandText = "Select documenttype from MMM_MST_DOC where tid = " & docid & ""
        oda.Fill(dsscal, "caldoc")
        Dim Dtype As String = ""
        Dtype = Convert.ToString(oda.SelectCommand.ExecuteScalar())

        oda.SelectCommand.CommandText = "Select cal_text,fieldmapping,isrecalculative from MMM_MST_FIELDS with (nolock) where documenttype ='" & Dtype & "' and fieldtype='Calculative Field' and eid='" & EID & "' and isactive=1"
        oda.Fill(dt5)
        If dt5.Rows.Count > 0 Then
            For n As Integer = 0 To dt5.Rows.Count - 1
                'Add Code if not required again calculation
                If dt5.Rows(n).Item("isrecalculative") = True Then
                    Dim orignlfinalstr As String = ""


                    cal_text = dt5.Rows(n).Item("cal_text")
                    cal_mpng = dt5.Rows(n).Item("fieldmapping")
                    dt6.Rows.Clear()
                    oda.SelectCommand.CommandText = "Select displayname,fieldmapping,isactive from MMM_MST_FIELDS with(nolock) where documenttype = '" & dsscal.Tables("caldoc").Rows(0).Item("documenttype").ToString() & "' and eid = " & EID & " "
                    oda.Fill(dt6)
                    stringf = cal_text
                    For m As Integer = 0 To dt6.Rows.Count - 1
                        '  If cal_text.Contains("{" & dt6.Rows(m).Item("displayname").ToString() & "}") Then
                        If cal_text.Trim().Contains("{" & dt6.Rows(m).Item("displayname").ToString().Trim() & "}") Then
                            fldmpng = dt6.Rows(m).Item("fieldmapping").ToString().Trim()
                            stringf = stringf.Replace("{" & dt6.Rows(m).Item("displayname").ToString().Trim() & "}", "{" & fldmpng.Trim() & "}")
                        End If
                    Next
                    stringf = stringf.Replace("{", "")
                    stringf = stringf.Replace("}", "")
                    Dim st As String() = Split(stringf, ",")
                    For k As Int16 = 0 To st.Length - 1
                        Dim dt7 As New DataTable
                        Dim finalstrr As String = ""
                        Dim s As String() = Split(st(k), "=")
                        If s(0).ToString.Length > 2 Then
                            Dim resultfldd As String = s(0)
                            orignlfinalstr = s(1)
                            If Right(orignlfinalstr, 1) = "," Then
                                orignlfinalstr = Left(orignlfinalstr, orignlfinalstr.Length - 1)
                            End If
                            Dim intval As String = ""
                            Dim spltstr() As String = orignlfinalstr.Split("(", ")", "+", "-", "*", "/")
                            For i As Integer = 0 To spltstr.Length - 1
                                If spltstr(i).Contains("fld") Then
                                    finalstrr = finalstrr & spltstr(i) & ","
                                Else
                                    intval = intval & spltstr(i) & ","
                                End If
                            Next
                            If Right(finalstrr, 1) = "," Then
                                finalstrr = Left(finalstrr, finalstrr.Length - 1)
                            End If
                            Dim value As String = ""
                            Dim Numericvalue As String = ""
                            Dim opr As String = ""
                            oda.SelectCommand.CommandText = "Select " & finalstrr & " from MMM_MST_DOC  where tid = " & docid & ""
                            oda.Fill(dt7)
                            Dim str As String = ""
                            Dim Temporignlfinalstr As String = orignlfinalstr
                            For h As Integer = 0 To dt7.Columns.Count - 1
                                For Each c As Char In orignlfinalstr
                                    str &= c
                                    If c = "(" Or c = ")" Then
                                        If orignlfinalstr.ToString.Contains("(") Or orignlfinalstr.ToString.Contains(")") Then
                                            value = value & c
                                            str = ""
                                        End If
                                    ElseIf str.Trim = dt7.Columns(h).ColumnName.ToString Then
                                        ' previous 
                                        'value &= dt7.Rows(0).Item(dt7.Columns(h).ColumnName.ToString)
                                        '' wtire here to apend .0 in every value if not  BY SUNIL 02-12-20
                                        Dim Val As String = IIf(String.IsNullOrEmpty(dt7.Rows(0).Item(dt7.Columns(h).ColumnName.ToString)), "0", dt7.Rows(0).Item(dt7.Columns(h).ColumnName.ToString))
                                        If Not Val.Contains(".") Then
                                            Val = String.Format("{0}.0", Val)
                                        End If
                                        value &= Val
                                        'Exit For
                                        'orignlfinalstr = orignlfinalstr.Replace(dt7.Columns(h).ColumnName.ToString, IIf(IsDBNull(dt7.Rows(0).Item(dt7.Columns(h).ColumnName)), "0", dt7.Rows(0).Item(dt7.Columns(h).ColumnName).ToString()))
                                    ElseIf c = "+" Or c = "-" Or c = "*" Or c = "/" Or c = "%" Then
                                        If String.IsNullOrEmpty(value) Then
                                            value = value & Numericvalue & c
                                            Numericvalue = ""
                                        Else
                                            value = value & c
                                        End If

                                        'orignlfinalstr = Left(orignlfinalstr, orignlfinalstr.Length - 1)
                                        Dim fld As String = str & c
                                        ' orignlfinalstr = Replace(orignlfinalstr, "(" & str.Trim, "")
                                        If orignlfinalstr.ToString.Contains("(") Or orignlfinalstr.ToString.Contains(")") Then
                                            orignlfinalstr = Replace(orignlfinalstr, "(" & str.Trim, "")
                                        Else
                                            orignlfinalstr = Replace(orignlfinalstr, str.Trim, "")
                                        End If
                                        str = ""
                                        'orignlfinalstr = Right(orignlfinalstr, orignlfinalstr.Length - 1)
                                        If h < dt7.Columns.Count - 1 Then
                                            Exit For
                                        End If
                                    Else
                                        If Temporignlfinalstr.Length <> orignlfinalstr.Length Then
                                            If IsNumeric(str) Then
                                                value = value & c
                                            End If
                                            'ElseIf c = "(" Or c = ")" Then
                                            '    value = value & c
                                        Else
                                            If IsNumeric(str) Then
                                                Numericvalue = Numericvalue & c
                                            End If
                                        End If
                                    End If
                                Next
                                'orignlfinalstr = orignlfinalstr.Replace(dt7.Columns(h).ColumnName.ToString, "")
                                'orignlfinalstr = Right(orignlfinalstr, orignlfinalstr.Length - 1)
                            Next
                            If Val(orignlfinalstr.Trim) <> 0 Then
                                value = value & Val(orignlfinalstr.Trim)  '' removed on 01_jan_15
                            End If
                            Dim res = New DataTable().Compute(value, 0).ToString()
                            Dim decimalVar As Decimal
                            decimalVar = Decimal.Round(res, 2, MidpointRounding.AwayFromZero)
                            decimalVar = Math.Round(decimalVar, 2)
                            'Dim res = New DataTable().Compute(orignlfinalstr, 0).ToString()


                            oda.SelectCommand.CommandText = "Update MMM_MST_DOC set " & resultfldd & "='" & decimalVar.ToString() & "' where tid = " & docid & ""
                            oda.SelectCommand.ExecuteNonQuery()
                            dt7.Rows.Clear()
                            dt7.Dispose()
                        End If
                    Next
                End If
            Next

        End If
    End Sub
    Public Function ParseDateFn(ByVal LRentPayment As String, ByVal date2 As Date, ByVal MGAmount As Double) As String


        Dim Result As String = String.Empty
        Dim FinalInvDate As String = ""
        Dim MGAmtDT As Double = 0
        Dim dat As Date
        Dim dat1 As Date

        If Date.TryParse(date2, dat) Then

            If LRentPayment = "1554654" Then '"Monthly"
                Dim startDt As New Date(dat.Year, dat.Month, dat.Day)
                Dim startDate As New Date(dat.Year, dat.Month, 1)
                Dim dss As String = ""
                Dim dtss As String = ""
                Dim DTS As Date
                dtss = DateAdd("M", 1, startDt)
                If Date.TryParse(dtss, DTS) Then
                    If DTS.Year = dat.Year Then
                        Dim endDt As New Date(dat.Year, dat.Month + 1, 1)
                        Dim lastDay As Date = New Date(dat.Year, dat.Month, 1)
                        Dim diff2 As Int64 = (endDt - startDt).TotalDays.ToString()
                        Dim difft2 As String = lastDay.AddMonths(1).AddDays(-1)
                        If Date.TryParse(difft2, dat1) Then
                            Dim endDate As New Date(dat1.Year, dat1.Month, dat1.Day)
                            Dim tss As TimeSpan = endDate.Subtract(startDate)

                            If Convert.ToInt32(tss.Days) >= 0 Then
                                dss = tss.Days
                            End If

                        End If
                        Dim FinalInvDt As New Date(dat.Year, dat.Month + 1, 1)
                        FinalInvDate = FinalInvDt
                        ' calculating rent Amount
                        If (diff2 - 1) < dss Then
                            MGAmtDT = MGAmount
                            MGAmtDT = MGAmtDT / dss
                            MGAmtDT = MGAmtDT * (diff2 - 1)
                        ElseIf (diff2 - 1) = dss Then
                            MGAmtDT = MGAmount
                        End If
                    Else
                        Dim endDt As New Date(DTS.Year, DTS.Month, 1)
                        Dim lastDay As Date
                        If DTS.Month = 1 Then
                            lastDay = DateAndTime.DateSerial(Year(endDt) - 1, 13, 0)
                        Else

                            lastDay = New DateTime(DTS.Year, DTS.Month - 1, DTS.Day)
                        End If
                        Dim difft2 As DateTime = lastDay.AddMonths(0).AddDays(-1)
                        Dim diff2 As Int64 = (difft2 - startDt).TotalDays.ToString()
                        If Date.TryParse(difft2, DTS) Then
                            Dim endDate As New Date(DTS.Year, DTS.Month, DTS.Day)
                            Dim tss As TimeSpan = endDate.Subtract(startDate)

                            If Convert.ToInt32(tss.Days) >= 0 Then
                                dss = tss.Days
                            End If

                        End If
                        Dim FinalInvDt As New Date(DTS.Year, DTS.Month, 1)
                        FinalInvDate = FinalInvDt
                        ' calculating rent Amount
                        If diff2 < dss Then
                            MGAmtDT = MGAmount
                            MGAmtDT = MGAmtDT / dss
                            MGAmtDT = MGAmtDT * diff2
                        ElseIf diff2 = dss Then
                            MGAmtDT = MGAmount
                        End If
                    End If
                End If


            ElseIf LRentPayment = "1554655" Then 'Quaterly
                Dim startDt As New Date(dat.Year, dat.Month, dat.Day)
                Dim startDate As New Date(dat.Year, dat.Month, 1)
                Dim dss As String = ""
                Dim dtss As String = ""
                Dim DTS As Date
                dtss = DateAdd("M", 3, startDt)
                If Date.TryParse(dtss, DTS) Then
                    If DTS.Year = dat.Year Then
                        Dim endDt As New Date(dat.Year, dat.Month + 3, 1)
                        Dim diff2 As Int64
                        Dim lastDay As Date
                        If DTS.Month = 1 Then
                            lastDay = DateAndTime.DateSerial(Year(endDt) - 1, 13, 0)
                        Else

                            lastDay = New DateTime(DTS.Year, DTS.Month - 1, DTS.Day)
                        End If
                        diff2 = (endDt - startDt).TotalDays.ToString()
                        Dim difft2 As String = lastDay.AddMonths(1).AddDays(-1)
                        If Date.TryParse(difft2, dat1) Then
                            Dim endDate As New Date(dat1.Year, dat1.Month, dat1.Day)
                            Dim tss As TimeSpan = endDate.Subtract(startDate)

                            If Convert.ToInt32(tss.Days) >= 0 Then
                                dss = tss.Days
                            End If

                        End If


                        Dim FinalInvDt As New Date(dat.Year, dat.Month + 3, 1)
                        FinalInvDate = FinalInvDt
                        ' calculating rent Amount
                        If (diff2 - 1) < dss Then
                            MGAmtDT = MGAmount * 3
                            MGAmtDT = MGAmtDT / dss
                            MGAmtDT = MGAmtDT * (diff2 - 1)
                        ElseIf (diff2 - 1) = dss Then
                            MGAmtDT = MGAmount * 3
                        End If
                    Else
                        Dim endDt As New Date(DTS.Year, DTS.Month, 1)
                        Dim lastDay As DateTime
                        If DTS.Month = 1 Then
                            lastDay = DateAndTime.DateSerial(Year(endDt) - 1, 13, 0)


                        Else

                            lastDay = New DateTime(DTS.Year, DTS.Month - 1, DTS.Day)
                        End If

                        Dim diff2 As Int64 = (endDt - startDt).TotalDays.ToString()
                        '  Dim diff2 As Int64 = (endDt - startDt).TotalDays.ToString()
                        If Date.TryParse(endDt, DTS) Then
                            Dim endDate As New Date(DTS.Year, DTS.Month, DTS.Day)
                            Dim tss As TimeSpan = lastDay.Subtract(startDate)

                            If Convert.ToInt32(tss.Days) >= 0 Then
                                dss = tss.Days
                            End If

                        End If
                        Dim FinalInvDt As New Date(DTS.Year, DTS.Month, 1)
                        FinalInvDate = FinalInvDt
                        ' calculating rent Amount
                        If diff2 - 1 < dss Then
                            MGAmtDT = MGAmount * 3
                            MGAmtDT = MGAmtDT / dss
                            MGAmtDT = MGAmtDT * diff2 - 1
                        ElseIf diff2 - 1 = dss Then
                            MGAmtDT = MGAmount * 3
                        End If
                    End If
                End If

            ElseIf LRentPayment = "1554656" Then 'Half Yearly
                Dim startDt As New Date(dat.Year, dat.Month, dat.Day)
                Dim startDate As New Date(dat.Year, dat.Month, 1)
                Dim dss As String = ""
                Dim dtss As String = ""
                Dim DTS As Date
                dtss = DateAdd("M", 6, startDt)
                If Date.TryParse(dtss, DTS) Then
                    If DTS.Year = dat.Year Then
                        Dim endDt As New Date(dat.Year, dat.Month + 6, 1)
                        Dim lastDay As DateTime = New DateTime(dat.Year, dat.Month + 5, 1)
                        Dim diff2 As Int64 = (endDt - startDt).TotalDays.ToString()
                        Dim difft2 As String = lastDay.AddMonths(1).AddDays(-1)
                        If Date.TryParse(difft2, dat1) Then
                            Dim endDate As New Date(dat1.Year, dat1.Month, dat1.Day)
                            Dim tss As TimeSpan = endDate.Subtract(startDate)

                            If Convert.ToInt32(tss.Days) >= 0 Then
                                dss = tss.Days
                            End If

                        End If
                        Dim FinalInvDt As New Date(dat.Year, dat.Month + 6, 1)
                        FinalInvDate = FinalInvDt
                        ' calculating rent Amount
                        If (diff2 - 1) < dss Then
                            MGAmtDT = MGAmount * 6
                            MGAmtDT = MGAmtDT / dss
                            MGAmtDT = MGAmtDT * (diff2 - 1)
                        ElseIf (diff2 - 1) = dss Then
                            MGAmtDT = MGAmount * 6
                        End If
                    Else
                        Dim endDt As New Date(DTS.Year, DTS.Month, 1)
                        Dim lastDay As DateTime
                        If DTS.Month = 1 Then
                            lastDay = DateAndTime.DateSerial(Year(endDt) - 1, 13, 0)
                        Else

                            lastDay = New DateTime(DTS.Year, DTS.Month - 1, DTS.Day)
                        End If
                        Dim difft2 As DateTime = lastDay.AddMonths(1).AddDays(-1)
                        Dim diff2 As Int64 = (difft2 - startDt).TotalDays.ToString()
                        If Date.TryParse(difft2, DTS) Then
                            Dim endDate As New Date(DTS.Year, DTS.Month, DTS.Day)
                            Dim tss As TimeSpan = endDate.Subtract(startDate)

                            If Convert.ToInt32(tss.Days) >= 0 Then
                                dss = tss.Days
                            End If

                        End If
                        Dim FinalInvDt As New Date(DTS.Year, DTS.Month, 1)
                        FinalInvDate = FinalInvDt
                        ' calculating rent Amount
                        If diff2 < dss Then
                            MGAmtDT = MGAmount * 6
                            MGAmtDT = MGAmtDT / dss
                            MGAmtDT = MGAmtDT * diff2
                        ElseIf diff2 = dss Then
                            MGAmtDT = MGAmount * 6
                        End If
                    End If
                End If
            End If


        End If

        Dim decimalVar As Decimal
        decimalVar = Decimal.Round(MGAmtDT, 2, MidpointRounding.AwayFromZero)
        decimalVar = Math.Round(decimalVar, 2)
        Result = Convert.ToString(FinalInvDate) + "=" + Convert.ToString(decimalVar)

        Return Result

    End Function

    'Commented on 8 july2020

    'Public Function CheckLMSettings(ByVal eid As Integer, ByVal DOCType As String, ByVal UID As String, ByVal DocID As Integer, ByVal con As SqlConnection, ByVal tran As SqlTransaction) As String
    '    Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
    '    'Dim con1 As SqlConnection = New SqlConnection(conStr)
    '    Dim ob As New DynamicForm
    '    Dim da As SqlDataAdapter = New SqlDataAdapter("", con)
    '    da.SelectCommand.Transaction = tran

    '    Try

    '        Dim con1 As SqlConnection = New SqlConnection(conStr)
    '        Dim da1 As SqlDataAdapter = New SqlDataAdapter("select lm.Tid,TDocType,SDocType,IsActiveStatus,wfstatus,lm.ChildDocType  as MChildDocType,TFld,SFld,Ordering,lmfm.ChildDocType,DocType,LMTid from  MMM_MST_LMsetting lm  inner join  MMM_MST_LMFieldMappingsetting lmfm on lm.Tid=lmfm.LMTid   where  lm.eid=" & eid & "   and IsActiveStatus=1  and lmfm.eid=" & eid & " and lm.SDocType='" & DOCType & "'", con1)

    '        Dim ds As New DataSet
    '        Dim dt As New DataTable
    '        da1.Fill(ds, "LMSettingData")

    '        Dim Fldstr As String = ""
    '        If ds.Tables("LMSettingData").Rows.Count <> 0 Then


    '            Dim insStr As String = String.Empty
    '            Dim res As String = ""
    '            Dim strTFlds As String = String.Empty
    '            Dim strSFlds As String = String.Empty
    '            Dim strHTFlds As String = String.Empty
    '            Dim strHSFlds As String = String.Empty
    '            Dim ChildDocT As String = String.Empty
    '            Dim TDocType As String = String.Empty

    '            Dim dtdata As New DataTable
    '            Dim dtdataHeader As New DataTable
    '            If ds.Tables("LMSettingData").Rows.Count > 0 Then
    '                dtdata = ds.Tables("LMSettingData").[Select]("DocType = 'Line'").CopyToDataTable()
    '                dtdataHeader = ds.Tables("LMSettingData").[Select]("DocType = 'Header'").CopyToDataTable()

    '                If dtdataHeader.Rows.Count > 0 Then

    '                    strHTFlds += String.Join(",", (From row In dtdataHeader.Rows Select CType(row.Item("TFld"), String)).ToArray)
    '                    strHSFlds += String.Join(",d.", (From row In dtdataHeader.Rows Select CType(row.Item("SFld"), String)).ToArray)
    '                    Dim strHSFldsArr As String()
    '                    strHSFldsArr = strHSFlds.Split(",")
    '                    Dim value As String = String.Join(",',',", strHSFldsArr)
    '                    strHSFlds = "d." + value
    '                End If
    '                If dtdata.Rows.Count > 0 Then

    '                    strTFlds += String.Join(",", (From row In dtdata.Rows Select CType(row.Item("TFld"), String)).ToArray)
    '                    strSFlds += String.Join(",i.", (From row In dtdata.Rows Select CType(row.Item("SFld"), String)).ToArray)
    '                    Dim strSFldsArr As String()
    '                    strSFldsArr = strSFlds.Split(",")
    '                    Dim valueL As String = String.Join(",',',", strSFldsArr)
    '                    strSFlds = "i." + valueL + ",',',"
    '                    'For i As Integer = 0 To dtdata.Rows.Count - 1

    '                    TDocType = dtdata.Rows(0).Item("TDocType")
    '                    ChildDocT = dtdata.Rows(0).Item("MChildDocType")
    '                    da.SelectCommand.CommandText = " select i.tid, concat(" & strSFlds + strHSFlds & ") as TFldVAl from mmm_mst_doc d inner join mmm_mst_doc_item i on d.tid=i.DOCID where d.documenttype='" & DOCType & "' and  i.documenttype='" & ChildDocT & "' and d.eid=" & eid & "  and i.docid=" & DocID & " "
    '                    da.Fill(ds, "LMDocData")

    '                    ' Next

    '                End If

    '                If ds.Tables("LMDocData").Rows.Count > 0 Then

    '                    For j As Integer = 0 To ds.Tables("LMDocData").Rows.Count - 1
    '                            Dim LMFldsVal As String = String.Empty
    '                            LMFldsVal = ds.Tables("LMDocData").Rows(j).Item("TFldVAl")
    '                            Dim LMFldsValArr As String()
    '                            LMFldsValArr = LMFldsVal.Split(",")
    '                            Dim value As String = String.Join("','", LMFldsValArr)
    '                            value = "'" + value + "'"


    '                            insStr = "insert into mmm_mst_master (documenttype,EID,ISAUTH,ADATE,CREATEDBY,UPDATEDDATE,LASTUPDATE," & strTFlds & "," & strHTFlds & ",refTid) values ('" & TDocType & "'," & eid & ",1,getdate(),'" & UID & "',getdate(),getdate()," & value & "," & ds.Tables("LMDocData").Rows(j).Item("tid") & " );"
    '                            da.SelectCommand.CommandText = insStr.ToString()
    '                            res = da.SelectCommand.ExecuteScalar()

    '                            ob.HistoryT(eid, res, UID, TDocType, "MMM_MST_MASTER", "ADD", con, tran)

    '                        Next
    '                    End If

    '                End If
    '        Else

    '            da.Dispose()
    '            da.Dispose()
    '            Return "fail"
    '            Return "NA"
    '        End If

    '    Catch ex As Exception
    '        Throw
    '    Finally
    '        If Not da Is Nothing Then
    '            da.Dispose()
    '        End If

    '        'If Not con Is Nothing Then
    '        '    con.Close()
    '        '    con.Dispose()
    '        'End If
    '    End Try
    '    Return "Success"
    'End Function
    Public Shared Function GenerateQuery(strColumn As String, strValue As String, objData As LineitemWrap) As String
        Dim StrQuery = ""
        Try
            Dim value = ""
            For Each Data In objData.DataItem
                strColumn = strColumn & "," & Data.FieldMapping
                value = Data.Values.Replace("'", "''")
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

    Public Shared Function GetFDate(strDate As String) As Date
        Dim ret As Date
        Try
            'Here we are assuming seperator may be /,\,-,.
            Dim arr = strDate.Split("/", "\", "-", ".")
            Dim Month As String = "0"
            Try
                'Checking if Month Name is string
                If Not IsNumeric(arr(1)) Then
                    'dim Months thisMonth
                    If arr(1).ToUpper = "JAN" Or arr(1).ToUpper = "JANUARY" Then
                        Month = "01"
                    ElseIf arr(1).ToUpper = "FEB" Or arr(1).ToUpper = "FEBRUARY" Then
                        Month = "02"
                    ElseIf arr(1).ToUpper = "MAR" Or arr(1).ToUpper = "JANUARY" Then
                        Month = "03"
                    ElseIf arr(1).ToUpper = "APR" Or arr(1).ToUpper = "APRIL" Then
                        Month = "04"
                    ElseIf arr(1).ToUpper = "MAY" Then
                        Month = "05"
                    ElseIf arr(1).ToUpper = "JUNE" Or arr(1).ToUpper = "JUN" Then
                        Month = "06"
                    ElseIf arr(1).ToUpper = "JULY" Or arr(1).ToUpper = "JUL" Then
                        Month = "07"
                    ElseIf arr(1).ToUpper = "AUG" Or arr(1).ToUpper = "AUGUST" Then
                        Month = "08"
                    ElseIf arr(1).ToUpper = "SEPT" Or arr(1).ToUpper = "SEPTEMBER" Then
                        Month = "09"
                    ElseIf arr(1).ToUpper = "OCT" Or arr(1).ToUpper = "OCTOBER" Then
                        Month = "10"
                    ElseIf arr(1).ToUpper = "NOV" Or arr(1).ToUpper = "NOVEMBER" Then
                        Month = "11"
                    ElseIf arr(1).ToUpper = "DEC" Or arr(1).ToUpper = "DECEMBER" Then
                        Month = "12"
                    End If
                Else
                    Month = arr(1)
                End If
            Catch ex As Exception
                Throw
            End Try
            ret = New Date(arr(2), Month, arr(0))
            If Not IsDate(ret) Then
                Throw New Exception()
            End If
        Catch ex As Exception
            Throw
        End Try
        Return ret
    End Function

    Public Shared Function ValidateForm(EID As String, Data As LineitemWrap, ByVal FormType As String, DocType As String, ByRef ErrorMag As String) As Boolean

        Dim errorMsg = ""
        Dim Result = ""
        Dim Value = ""
        Dim DisplayName = ""
        Dim IsAllFormValid As Boolean = True
        For Each Row In Data.DataItem
            Value = Row.Values
            DisplayName = Row.DisplayName
            Select Case Row.FieldType.ToUpper
                Case "TEXT BOX"
                    If Row.IsRequired.ToString() = "1" And Value.Trim() = "" Then
                        errorMsg = errorMsg & " " & DisplayName & " required,"
                        IsAllFormValid = False
                        Exit For
                    End If
                    Select Case Row.DataType
                        Case "Datetime"
                            If Value.Trim() <> "" Then
                                Try
                                    Dim dt As New Date()
                                    dt = GetFDate(Value)
                                    Result = dt.ToString("dd/MM/yy", CultureInfo.InvariantCulture)
                                    Value = Result
                                    'IsAllFormValid = True
                                Catch ex As Exception
                                    errorMsg = errorMsg & " Invalid Date Formate of " & DisplayName & ","
                                    IsAllFormValid = False
                                End Try
                                Row.Values = Value
                            End If

                        Case "Numeric"
                            If Value.Trim() <> "" Then
                                Dim numberAsString As String = Value.ToString()

                                Dim indexOfDecimalPoint As Integer = numberAsString.IndexOf(".")
                                Dim numberOfDecimals As Integer = 0
                                If indexOfDecimalPoint <> -1 Then
                                    numberOfDecimals = numberAsString.Substring(indexOfDecimalPoint + 1).Length
                                End If
                                If IsNumeric(Value) = False Then
                                    errorMsg &= " NUMERIC data at " & DisplayName & ","
                                    IsAllFormValid = False
                                End If
                                If numberOfDecimals > Convert.ToInt32(Row.AllowDecimalDigit) And Convert.ToInt32(Row.AllowDecimalDigit) <> 0 Then
                                    errorMsg &= " Decimal digit in """ & DisplayName & """ cannot be greater than""" & Row.AllowDecimalDigit & """ ,"
                                    IsAllFormValid = False
                                End If
                                Value = Value.Replace(",", String.Empty)
                                Row.Values = Value
                            End If
                        Case "Text"
                            If Not String.IsNullOrEmpty(Value.Trim()) Then
                                If Value.Length < CInt(Row.MinVal.ToString) Then
                                    errorMsg &= " Minimum  " & Row.MinVal.ToString() & " character in " & DisplayName & ","
                                    IsAllFormValid = False
                                End If
                                If Value.Length > CInt(Row.MaxVal.ToString) Then
                                    errorMsg &= " Maximum  " & Row.MaxVal.ToString() & " character in " & DisplayName & ","
                                    IsAllFormValid = False
                                End If
                            End If
                            'IS unique Code Will Go here 
                    End Select
                Case "DROP DOWN"
                    If Row.IsRequired.ToString() = "1" And (Value.ToLower() = "0" Or Value.ToLower() = "select" Or Value.Trim = "") And Row.InVisible = 0 Then
                        errorMsg &= " " & DisplayName & " required,"
                        IsAllFormValid = False
                    End If
                    If Value.Trim = "-1" Then
                        'Please note that either this VRF number is wrong or an expense claim is already booked against this VRF number. Please check the value entered in '" & displayname & "' field. This value does not exist!
                        errorMsg &= " Please check the value entered in '" & DisplayName & "' field. This value does not exist!"
                        IsAllFormValid = False
                    End If

                Case "LOOKUPDDL"
                    If Row.IsRequired.ToString() = "1" And (Value.ToLower() = "0" Or Value.ToLower() = "select" Or Value.Trim = "") Then
                        errorMsg &= " " & DisplayName & " required,"
                        IsAllFormValid = False
                    End If
                    If Value.Trim = "-1" Then
                        'Please note that either this VRF number is wrong or an expense claim is already booked against this VRF number. Please check the value entered in '" & displayname & "' field. This value does not exist!
                        errorMsg &= " Please check the value entered in '" & DisplayName & "' field. This value does not exist!"
                        IsAllFormValid = False
                    End If


                Case "AUTOCOMPLETE"

                    If Row.IsRequired.ToString() = "1" And (Value.ToLower() = "0" Or Value.ToLower() = "select") Then
                        errorMsg &= " " & DisplayName & " required,"
                        IsAllFormValid = False
                    End If
                    If Value = "-1" Then
                        'Please note that either this VRF number is wrong or an expense claim is already booked against this VRF number. Please check the value entered in '" & displayname & "' field. This value does not exist!
                        errorMsg &= " Please check the value entered in '" & DisplayName & "' field. This value does not exist!"
                        IsAllFormValid = False
                    End If

                Case "TEXT AREA"
                    If Row.IsRequired.ToString() = "1" And Value.Trim() = "" Then
                        errorMsg &= " " & DisplayName & " required,"
                        IsAllFormValid = False
                    End If
                    If Not String.IsNullOrEmpty(Value.Trim()) Then
                        If Value.Length < CInt(Row.MinVal.ToString) Then
                            errorMsg &= " Minimum  " & Row.MinVal.ToString() & " character in " & DisplayName & ","
                            IsAllFormValid = False
                        End If
                        If Value.Length > CInt(Row.MaxVal.ToString) Then
                            errorMsg &= " Maximum  " & Row.MaxVal.ToString() & " character in " & DisplayName & ","
                            IsAllFormValid = False
                        End If
                    End If
                Case "FILE UPLOADER"
                    If Row.IsRequired.ToString() = "1" And Value = "" Then
                        errorMsg &= " " & DisplayName & " required,"
                        IsAllFormValid = False
                    End If
                Case "LOOKUP"
                    If Row.IsRequired.ToString() = "1" And Value = "" Then
                        errorMsg &= " " & DisplayName & " required,"
                        IsAllFormValid = False
                    End If
                Case "CALCULATIVE FIELD"
                    If Row.IsRequired.ToString() = "1" And Value = "" Then
                        errorMsg &= " " & DisplayName & " required,"
                        IsAllFormValid = False
                    End If
                Case "AUTO NUMBER"
                    If Row.IsRequired.ToString() = "1" And Value = "" Then
                        errorMsg &= " " & DisplayName & " required,"
                        IsAllFormValid = False
                    End If
                Case "CHECKBOX LIST"
                    If Row.IsRequired.ToString() = "1" Then
                        If Value.Trim = "" Or Value.ToLower = "0" Then
                            errorMsg &= " " & DisplayName & " required,"
                            IsAllFormValid = False
                        Else
                            'There can be n number of tid(tid of master or tid of document)
                            '-1 Means Invalid TID
                            Dim val = Value.Split(",")
                            For Each strValue In val
                                If strValue = "-1" Then
                                    errorMsg &= " Please check the value entered in '" & DisplayName & "' field. This value does not exist!"
                                    IsAllFormValid = False
                                End If
                            Next
                        End If
                    End If
            End Select
            'Unique Check validation
            If IsAllFormValid = True Then
                If Row.IsUnique.ToString() = "1" Then
                    Dim tableName = ""
                    If FormType.ToUpper() = "MASTER" Then
                        tableName = "MMM_MST_MASTER"
                    ElseIf FormType.ToUpper() = "DOCUMENT" Then
                        tableName = "MMM_MST_DOC"
                    ElseIf FormType.ToUpper() = "USER" Then
                        tableName = "MMM_MST_USER"
                    End If
                    Try
                        Dim IsExists As Boolean = True
                        IsExists = checkduplicate(EID, "ADD", 0, tableName, Row.FieldMapping, Row.Values, DocType)
                        If IsExists = True Then
                            errorMsg &= " " & DisplayName & " must be unique,"
                            IsAllFormValid = False
                        End If
                    Catch ex As Exception
                    End Try
                End If
            End If
        Next
        ErrorMag = ErrorMag & errorMsg

        Return IsAllFormValid

    End Function

    Public Shared Function checkduplicate(EId As Integer, ByVal qrytype As String, ByVal tid As Integer, ByVal tablename As String, ByVal fldmapping As String, ByVal value As String, ByVal doctype As String) As Boolean
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        If con.State <> ConnectionState.Open Then
            con.Open()
        End If
        Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
        Try
            Dim qry As String
            If Trim(tablename) = "MMM_MST_DOC" Then
                qry = "select count(*) from " & tablename & " where eid=" & EId & " AND DOCUMENTTYPE='" & doctype & "' AND " & fldmapping & "='" & value & "' and curstatus <> 'REJECTED' "
            ElseIf Trim(tablename) = "MMM_MST_USER" Then
                qry = "select count(*) from " & tablename & " where eid=" & EId & "  AND " & fldmapping & "='" & value & "'"
            Else
                qry = "select count(*) from " & tablename & " where eid=" & EId & " AND DOCUMENTTYPE='" & doctype & "' AND " & fldmapping & "='" & value & "'"
            End If
            Dim XWHR As String = ""
            If qrytype.ToUpper() = "UPDATE" Then
                XWHR = " AND TID<>" & tid & ""
            End If
            qry &= XWHR
            oda.SelectCommand.CommandText = qry
            Dim CNT As Integer = oda.SelectCommand.ExecuteScalar()
            If CNT > 0 Then
                Return True
            Else
                Return False
            End If
        Catch ex As Exception
            Return True
        Finally
            con.Close()
            con.Dispose()
            oda.Dispose()
        End Try

    End Function

    Public Shared Function FillData(ByRef LstData As List(Of DataWraper1), ds As DataSet, EID As Integer, DocType As String, Optional UID As Integer = 0, Optional ByRef fileUploader As ArrayList = Nothing, Optional prefixFilePath As String = "") As List(Of DataWraper1)

        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString

        Dim con As SqlConnection = Nothing
        Try
            If DocType.ToUpper() = "USER" Then
                'Adding rows 
                AddUserStaticField(ds.Tables(0))
            End If
            Dim Documenttype = ""
            'Processing For All Doc
            For Each objData In LstData
                Documenttype = objData.DocumentType
                'loop For line item
                For Each objItem In objData.LineItem
                    con = New SqlConnection(conStr)
                    'loop for data field item

                    'Dim Filter = From cust In objItem.LineItem Where cust.FieldType <> "Formula field" And cust.FieldType <> "Calculative Field" And cust.FieldType <> "Lookup" And cust.FieldType <> "Child Item" Select cust
                    'Filtering only master valued dropdown 
                    Dim DropDownLine = From cust In objItem.DataItem Where (cust.FieldType = "Drop Down" Or cust.FieldType = "AutoComplete") And cust.DropDownType = "MASTER VALUED" Or cust.DropDownType = "SESSION VALUED" Order By cust.DisplayOrder Select cust
                    'Added Functionality for FileUploader to upload the file if arraylist has the file on server control 09-03-2017 start here'
                    Dim objfileUpload = From fp In objItem.DataItem Where (fp.FieldType = "File Uploader") Order By fp.DisplayOrder Select fp
                    If Not IsNothing(fileUploader) Then
                        For Each objLineUpload In objfileUpload
                            For k As Integer = 0 To fileUploader.Count - 1
                                If fileUploader(k).ToString().ToUpper.Contains("/" & objLineUpload.Values.ToString().ToUpper) And objLineUpload.Values <> "" Then
                                    Try
                                        Dim sourceFilePath As String = prefixFilePath & "/" & fileUploader(k).ToString()
                                        Dim fileName = DateTime.Now.Day & DateTime.Now.Year & DateTime.Now.Ticks & "_" & objLineUpload.Values.ToString()
                                        fileName.Replace("/", "_")
                                        fileName.Replace(":", "_")
                                        fileName.Replace(" ", "_")
                                        Dim destinationFilePath As String = HttpContext.Current.Server.MapPath("~/DOCS/" & EID & "/" & fileName)
                                        System.IO.File.Copy(sourceFilePath, destinationFilePath)
                                        objLineUpload.Values = EID & "/" & fileName
                                    Catch ex As Exception
                                        objLineUpload.Values = ""
                                    End Try

                                End If
                            Next
                        Next
                    End If
                    'Added Functionality for FileUploader to upload the file if arraylist has the file on server control 09-03-2017 End here'

                    For Each Line1 In DropDownLine
                        Dim StrDDlID As String = "0"
                        '-1 For invalid drop down TID if it is master valued
                        'Get Tid of supplyed Drop Down
                        Dim DR As DataRow() = ds.Tables(0).Select("FieldID=" & Line1.FieldID)

                        If Not String.IsNullOrEmpty(Line1.Values.Trim()) And Line1.Values.Trim().ToUpper() <> "SELECT" And Line1.Values.Trim() <> "0" Then
                            StrDDlID = DropDown.GetDropDownID(DR(0), EID, Line1.Values.Trim(), objItem)
                            If Line1.LookUp.Contains("-R") = True Then
                                bind(Line1.FieldID, StrDDlID, LstData)
                            End If
                        End If

                        If Line1.DropDownType.Trim().ToUpper() = "SESSION VALUED" Then
                            If Line1.LookUp.Contains("-R") = True Then
                                bind(Line1.FieldID, StrDDlID, LstData)
                            End If
                        End If

                        ''Fill default val if invisible is true 

                        If Line1.ddlval IsNot Nothing Then
                            If Line1.ddlval.ToString() <> "" And Line1.InVisible.ToString() = "1" Then
                                StrDDlID = Line1.ddlval
                            End If
                        End If

                        If String.IsNullOrEmpty(StrDDlID) Then
                            StrDDlID = "0"
                        End If
                        Try
                            If Convert.ToInt32(StrDDlID) < 0 Then
                                'Pending Mayank Testing
                                If Line1.AllowCreatedRecord = True Then
                                    Try
                                        Dim dynamicInsertQuery As New StringBuilder("")
                                        Dim dynamicColumn As New ArrayList()
                                        Dim dynamicColumnValue As New ArrayList()
                                        Dim dynamicTableName As String = ""
                                        Dim arr = DR(0).Item("DropDown").ToString().Split("-")
                                        Dim isvalidUser As Boolean = True
                                        If UCase(arr(0).ToString) = "STATIC" Then
                                            If arr(1).ToString.ToUpper = "USER" Then
                                                dynamicTableName = "MMM_MST_USER"
                                                dynamicInsertQuery.Append(" insert into " & dynamicTableName.ToString() & " (EID,ModifyDate,lastupdate,isAuth,locationID,CreatedBy," & arr(2).ToString() & "")
                                                Dim objDC As New DataClass()
                                                Dim CreatedBy As String = "0"
                                                Dim objDT = objDC.ExecuteQryDT("select uid from MMM_MST_USER where eid='" & EID & "' and UserName IN ('System_User','Superuser') order by CreatedOn desc")
                                                If objDT IsNot Nothing And objDT.Rows.Count > 0 Then
                                                    CreatedBy = objDT.Rows(0).Item(0).ToString()
                                                End If
                                                If Not String.IsNullOrEmpty(Line1.OnflyFieldmapping) Then
                                                    Dim totalSpelitedVal() As String = Line1.OnflyFieldmapping.ToString().Split(",")
                                                    For commaseparr As Integer = 0 To totalSpelitedVal.Length - 1
                                                        Dim arrval As String() = totalSpelitedVal(commaseparr).ToString().Split("-")
                                                        'Dim dynval As String = arrval(0).ToString()
                                                        dynamicColumn.Add(arrval(0).ToString())
                                                        Dim Dynamicval = From cust In objItem.DataItem Where (cust.FieldMapping = "" & arrval(1).ToString() & "") Order By cust.DisplayOrder Select cust
                                                        If Dynamicval.Count > 0 Then
                                                            For Each singeLineData In Dynamicval
                                                                dynamicColumnValue.Add("'" & singeLineData.Values & "'")
                                                            Next
                                                        Else
                                                            dynamicColumnValue.Add("'" & arrval(1).ToString() & "'")
                                                        End If

                                                    Next
                                                    If dynamicColumn.Count > 0 Then
                                                        'changes by vinod when user credential not valid
                                                        For uservalid As Integer = 0 To dynamicColumn.Count - 1
                                                            If dynamicColumn(uservalid).ToString().ToLower() <> "emailid" Then
                                                                If dynamicColumnValue(uservalid) = "''" Then
                                                                    isvalidUser = False
                                                                    Exit For
                                                                End If
                                                            End If

                                                        Next
                                                        '------------------------------------------------------------------

                                                        dynamicInsertQuery.Append("," & String.Join(",", dynamicColumn.ToArray()) & ") values ( " & EID & ",getdate(),getdate(),100,2072,'" & CreatedBy & "','" & Line1.Values.Trim() & "',")
                                                        dynamicInsertQuery.Append("" & String.Join(",", dynamicColumnValue.ToArray()) & ")")
                                                    Else
                                                        dynamicInsertQuery.Append(") values ( " & EID & ",getdate(),getdate(),100,2072,'" & CreatedBy & "','" & Line1.Values.Trim() & "')")
                                                    End If
                                                Else
                                                    dynamicInsertQuery.Append(") values ( " & EID & ",getdate(),getdate(),100,2072,'" & CreatedBy & "','" & Line1.Values.Trim() & "')")
                                                End If
                                            End If
                                        ElseIf UCase(arr(0).ToString()) = "MASTER" Then
                                            dynamicTableName = "MMM_MST_MASTER"
                                            dynamicInsertQuery.Append(" insert into " & dynamicTableName.ToString() & " (EID,DocumentType,createdBy,UpdatedDate,lastupdate,adate,isauth,Source,NaNcounter,TallyIsActive," & arr(2).ToString() & "")
                                            If Not String.IsNullOrEmpty(Line1.OnflyFieldmapping) Then
                                                Dim totalSpelitedVal() As String = Line1.OnflyFieldmapping.ToString().Split(",")
                                                For commaseparr As Integer = 0 To totalSpelitedVal.Length - 1
                                                    Dim arrval As String() = totalSpelitedVal(commaseparr).ToString().Split("-")
                                                    'Dim dynval As String = arrval(0).ToString()
                                                    dynamicColumn.Add(arrval(0).ToString())
                                                    Dim Dynamicval = From cust In objItem.DataItem Where (cust.FieldMapping = "" & arrval(1).ToString() & "") Order By cust.DisplayOrder Select cust
                                                    For Each singeLineData In Dynamicval
                                                        dynamicColumnValue.Add("'" & singeLineData.Values & "'")
                                                    Next
                                                Next
                                                If dynamicColumn.Count > 0 Then
                                                    dynamicInsertQuery.Append("," & String.Join(",", dynamicColumn.ToArray()) & ") values ( " & EID & ",'" & DocType & "'," & UID & " getdate(),getdate(),getdate(),1,'WS',0,0,'" & Line1.Values.Trim() & "',")
                                                    dynamicInsertQuery.Append("" & String.Join(",", dynamicColumnValue.ToArray()) & ")")
                                                Else
                                                    dynamicInsertQuery.Append(") values ( " & EID & ",'" & DocType & "'," & UID & " getdate(),getdate(),getdate(),1,'WS',0,0,'" & Line1.Values.Trim() & "')")
                                                End If
                                            Else
                                                dynamicInsertQuery.Append(") values ( " & EID & ",'" & DocType & "'," & UID & " getdate(),getdate(),getdate(),1,'WS',0,0,'" & Line1.Values.Trim() & "')")
                                            End If
                                            'ElseIf UCase(arr(0).ToString()) = "DOCUMENT" Then
                                            '    dynamicTableName = "MMM_MST_DOC"
                                            '    dynamicInsertQuery.Append(" insert into " & dynamicTableName.ToString() & " (EID,cnt,adate,OUID,isworkflow,createdBy,UpdatedDate,lastupdate,,isauth,Source,NaNcounter,TallyIsActive," & arr(2).ToString() & "")
                                            '    If Not String.IsNullOrEmpty(Line1. OnflyFieldmapping) Then
                                            '        Dim totalSpelitedVal() As String = Line1.OnflyFieldmapping.ToString().Split(",")
                                            '        For commaseparr As Integer = 0 To totalSpelitedVal.Length - 1
                                            '            Dim arrval As String() = totalSpelitedVal(commaseparr).ToString().Split("-")
                                            '            'Dim dynval As String = arrval(0).ToString()
                                            '            dynamicColumn.Add(arrval(0).ToString())
                                            '            Dim Dynamicval = From cust In objItem.DataItem Where (cust.FieldMapping = "" & arrval(1).ToString() & "") Order By cust.DisplayOrder Select cust
                                            '            For Each singeLineData In Dynamicval
                                            '                dynamicColumnValue.Add("'" & singeLineData.Values & "'")
                                            '            Next
                                            '        Next
                                            '        If dynamicColumn.Count > 0 Then
                                            '            dynamicInsertQuery.Append("," & String.Join(",", dynamicColumn.ToArray()) & ") values ( " & EID & ",'" & DocType & "'," & UID & " getdate(),getdate(),getdate(),1,'WS',0,0,'" & Line1.Values.Trim() & "',")
                                            '            dynamicInsertQuery.Append("" & String.Join(",", dynamicColumnValue.ToArray()) & ")")
                                            '        Else
                                            '            dynamicInsertQuery.Append(") values ( " & EID & ",'" & DocType & "'," & UID & " getdate(),getdate(),getdate(),1,'WS',0,0,'" & Line1.Values.Trim() & "')")
                                            '        End If
                                            '    Else
                                            '        dynamicInsertQuery.Append(") values ( " & EID & ",'" & DocType & "'," & UID & " getdate(),getdate(),getdate(),1,'WS',0,0,'" & Line1.Values.Trim() & "')")
                                            '    End If
                                        End If
                                        'changes by vinod check if isvalid user---------
                                        If isvalidUser = True Then
                                            If con.State = ConnectionState.Closed Then
                                                con.Open()
                                            End If
                                            dynamicInsertQuery.Append(";SELECT IDENT_CURRENT('" & dynamicTableName.ToString() & "')")
                                            Dim da As New SqlDataAdapter(dynamicInsertQuery.ToString(), con)
                                            da.SelectCommand.Parameters.Clear()
                                            Dim dt As New DataTable
                                            da.Fill(dt)
                                            StrDDlID = dt.Rows(0)(0).ToString()
                                            If arr(1).ToString.ToUpper = "USER" Then

                                                '''' *** code to add user entry in role assingment if roletype is Post Role by sunil 
                                                Dim RoleType As String
                                                da.SelectCommand.CommandText = "CreateRoleAssignment_NewUser_Creation"
                                                da.SelectCommand.CommandType = CommandType.StoredProcedure
                                                da.SelectCommand.Parameters.Clear()
                                                da.SelectCommand.Parameters.AddWithValue("@eid", EID)
                                                da.SelectCommand.Parameters.AddWithValue("@uid", StrDDlID)
                                                Dim res As String = da.SelectCommand.ExecuteNonQuery()
                                                '''' *** code to add user entry in role assingment if roletype is Post Role by sunil 

                                                Dim DMS As New DMSUtil
                                                Try
                                                    ''Change Required
                                                    DMS.notificationMail(StrDDlID, EID, "USER", "USER CREATED")
                                                Catch ex As Exception
                                                    StrDDlID = dt.Rows(0)(0).ToString()
                                                End Try
                                            End If
                                        Else
                                            StrDDlID = "0"
                                        End If
                                    Catch ex As Exception
                                        StrDDlID = "-1"
                                    End Try
                                Else
                                    StrDDlID = "-1"
                                End If
                            End If
                        Catch ex As Exception
                        End Try

                        'Set TID into Drop Down
                        Line1.Values = StrDDlID
                    Next
                    'Filtering only look up Row 

                    ''Non Editable Current Date
                    Dim CurrDate = From cust In objItem.DataItem Where (cust.FieldType = "Text Box") And cust.DataType = "Datetime" And cust.isEditable = 0 Order By cust.DisplayOrder Select cust
                    For Each Line1 In CurrDate
                        Dim Strval As String = "0"
                        Dim DR As DataRow() = ds.Tables(0).Select("FieldID=" & Line1.FieldID)
                        Strval = Now.ToString("dd/MM/yy")
                        If Line1.Values = "" Then
                            Line1.Values = Strval.ToString()
                        End If
                    Next
                    ''End Non Editable Current Date

                    Dim DropDownLineChild = From cust In objItem.DataItem Where (cust.FieldType = "Drop Down" Or cust.FieldType = "AutoComplete") And cust.DropDownType = "CHILD VALUED" Order By cust.DisplayOrder Select cust
                    For Each Line1 In DropDownLineChild
                        Dim StrDDlID As String = "0"
                        '-1 For invalid drop down TID if it is master valued
                        'Get Tid of supplyed Drop Down
                        Dim DR As DataRow() = ds.Tables(0).Select("FieldID=" & Line1.FieldID)
                        If Not String.IsNullOrEmpty(Line1.Values.Trim()) And Line1.Values.Trim().ToUpper() <> "SELECT" And Line1.Values.Trim() <> "0" Then
                            StrDDlID = DropDown.GetDropDownChildID(DR(0), EID, Line1.Values.Trim(), objItem)
                        End If
                        If String.IsNullOrEmpty(StrDDlID) Then
                            StrDDlID = "0"
                        End If
                        Try
                            If Convert.ToInt32(StrDDlID) < 0 Then
                                StrDDlID = "-1"
                            End If
                        Catch ex As Exception
                        End Try

                        'Set TID into Drop Down
                        Line1.Values = StrDDlID
                    Next

                    Dim LookUPLine = From LookUPRow In objItem.DataItem Where LookUPRow.FieldType = "Lookup" Select LookUPRow
                    'Getting corresponding look up value from data base
                    For Each Line In LookUPLine
                        'Line.
                        Dim strLookUpValue = ""
                        If Line.Values.Trim() = "" Then
                            strLookUpValue = GetlookUpValue(Line, objItem, ds.Tables(0), EID, con)
                            Line.Values = strLookUpValue
                        End If
                    Next
                    'Filtering only Fixed valued Drop Down 
                    'Fixed valued drop down validation
                    Dim FixedDDL = From DDLFXD In objItem.DataItem Where DDLFXD.FieldType = "Drop Down" And DDLFXD.DropDownType.ToUpper = "FIX VALUED" Select DDLFXD
                    For Each Line In FixedDDL
                        'Line.Getting defined value of droop down and valiating it with the supplyed values
                        Dim StrAllowedValue = Line.DropDown.ToUpper
                        Dim arr = StrAllowedValue.Split(",")
                        If Line.Values.ToUpper.Trim = "SELECT" Or Line.Values.Trim = "" Then
                            Line.Values = ""
                        Else
                            Dim FixedDDLValues = From DDLFXD In arr Where DDLFXD.Trim <> "" Select DDLFXD
                            Dim ISFound As Boolean = False
                            For Each ITEM In FixedDDLValues
                                If ITEM.Trim.ToUpper = Line.Values.ToUpper Then
                                    ISFound = True
                                    Exit For
                                End If
                            Next
                            'if defined value is not supplyed set -1,So that it will not pass validation while validatiting it.
                            If ISFound = True Then
                                Line.Values = Line.Values.ToUpper
                            Else
                                Line.Values = "-1"
                            End If
                        End If
                    Next
                    'Now Pupulating DDLLookup

                    'Fixed valued text box and ddlval not empty
                    Dim FixedValue = From fixedVal In objItem.DataItem Where fixedVal.DropDownType = "FIX VALUED" And fixedVal.ddlval <> "" And fixedVal.InVisible = 1 Select fixedVal
                    For Each Line In FixedValue
                        Line.Values = Line.ddlval.ToUpper
                    Next
                    'Now Pupulating DDLLookup

                    Dim DDLLookuup1 = From cust In objItem.DataItem Where cust.FieldType = "LookupDDL" Order By cust.DisplayOrder Select cust

                    For Each Line12 In DDLLookuup1
                        Dim StrDDlID As String = "0"
                        '-1 For invalid drop down TID if it is master valued
                        'Get Tid of supplyed Drop Down
                        Dim DR As DataRow() = ds.Tables(0).Select("FieldID=" & Line12.FieldID)
                        'If Not String.IsNullOrEmpty(Line12.Values.Trim()) And Line12.Values.Trim().ToUpper() <> "SELECT" And Line12.Values.Trim() <> "0" Then
                        StrDDlID = DDLookUp.GetTID(DR(0), EID, Line12.Values.Trim(), objItem)
                        'End If
                        If String.IsNullOrEmpty(StrDDlID) Then
                            StrDDlID = "0"
                        End If
                        Try
                            If Convert.ToInt32(StrDDlID) < 0 Then
                                StrDDlID = "-1"
                            End If
                        Catch ex As Exception
                        End Try
                        'Set TID into Drop Down
                        Line12.Values = StrDDlID
                        Line12.DDLValue = Line12.Values.Trim()
                    Next
                    Dim CheckBoxList = From cust In objItem.DataItem Where cust.FieldType = "CheckBox List" Order By cust.DisplayOrder Select cust

                    For Each Item In CheckBoxList
                        Dim StrCHKID As String = "0"
                        Dim DR As DataRow() = ds.Tables(0).Select("FieldID=" & Item.FieldID)
                        If Not String.IsNullOrEmpty(Item.Values.Trim()) And Item.Values.Trim().ToUpper() <> "SELECT" And Item.Values.Trim() <> "0" Then
                            StrCHKID = ChkBoxList.GetCHKID(DR(0), EID, Item.Values.Trim(), objItem)
                        End If
                        If String.IsNullOrEmpty(StrCHKID) Then
                            StrCHKID = "0"
                        End If

                        'Set TID into Drop Down
                        Item.Values = StrCHKID
                    Next

                    'Getting all CalCulative field for evaluating it from expression.
                    Dim CalCulativeField = From CalRow In objItem.DataItem Where CalRow.FieldType = "Calculative Field" Select CalRow
                    Dim Cal_Text = ""
                    For Each line In CalCulativeField
                        Cal_Text = line.CalText.ToString()
                        'Calculating its equivalent values
                        line.Values = EvaluateFormula(Cal_Text, line.DisplayName, objItem)
                    Next
                Next
            Next
            '
            Dim MainForm = From Form In LstData Where Form.DocumentType = DocType Select Form
            'Getting all the childitem total for its evalution 
            For Each Form In MainForm
                For Each objItem In Form.LineItem
                    Dim ChidItemTotalRows = From ChldTotal In objItem.DataItem Where ChldTotal.FieldType = "Child Item Total" Select ChldTotal
                    For Each line In ChidItemTotalRows
                        'Getting details of concern Field
                        Dim DR1 As DataRow() = ds.Tables(0).Select("FieldID=" & line.DropDown)
                        Dim ConcernDocType = DR1(0).Item("DocumentType").ToString()
                        Dim DisplayName = DR1(0).Item("DisplayName").ToString()
                        Dim ChildForm = From LstChild In LstData Where LstChild.DocumentType = ConcernDocType Select LstChild
                        Dim result As Double = 0.0
                        For Each var In ChildForm
                            result = 0
                            For Each var1 In var.LineItem
                                For Each var3 In var1.DataItem
                                    Dim Val As Double = 0.0
                                    If IsNumeric(var3.Values) = False Then
                                        Val = 0
                                    Else
                                        Val = var3.Values
                                    End If
                                    If var3.DisplayName = DisplayName Then
                                        result = result + Val
                                    End If
                                Next
                            Next
                        Next
                        line.Values = result

                    Next
                Next
            Next
        Catch ex As Exception
            ex.ToString()
        Finally
            If Not con Is Nothing Then
                con.Close()
                con.Dispose()
            End If
        End Try
        Return LstData
    End Function

    'Public Shared Function FillData(ByRef LstData As List(Of DataWraper1), ds As DataSet, EID As Integer, DocType As String, Optional UID As Integer = 0) As List(Of DataWraper1)

    '    Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString

    '    Dim con As SqlConnection = Nothing
    '    Try
    '        If DocType.ToUpper() = "USER" Then
    '            'Adding rows 
    '            AddUserStaticField(ds.Tables(0))
    '        End If
    '        Dim Documenttype = ""
    '        'Processing For All Doc
    '        For Each objData In LstData
    '            Documenttype = objData.DocumentType
    '            'loop For line item
    '            For Each objItem In objData.LineItem
    '                con = New SqlConnection(conStr)
    '                'loop for data field item
    '                'Dim Filter = From cust In objItem.LineItem Where cust.FieldType <> "Formula field" And cust.FieldType <> "Calculative Field" And cust.FieldType <> "Lookup" And cust.FieldType <> "Child Item" Select cust
    '                'Filtering only master valued dropdown 
    '                Dim DropDownLine = From cust In objItem.DataItem Where (cust.FieldType = "Drop Down" Or cust.FieldType = "AutoComplete") And cust.DropDownType = "MASTER VALUED" Or cust.DropDownType = "SESSION VALUED" Order By cust.DisplayOrder Select cust
    '                For Each Line1 In DropDownLine
    '                    Dim StrDDlID As String = "0"
    '                    '-1 For invalid drop down TID if it is master valued
    '                    'Get Tid of supplyed Drop Down
    '                    Dim DR As DataRow() = ds.Tables(0).Select("FieldID=" & Line1.FieldID)
    '                    If Not String.IsNullOrEmpty(Line1.Values.Trim()) And Line1.Values.Trim().ToUpper() <> "SELECT" And Line1.Values.Trim() <> "0" Then
    '                        StrDDlID = DropDown.GetDropDownID(DR(0), EID, Line1.Values.Trim(), objItem)
    '                    End If
    '                    If String.IsNullOrEmpty(StrDDlID) Then
    '                        StrDDlID = "0"
    '                    End If
    '                    Try
    '                        If Convert.ToInt32(StrDDlID) < 0 Then
    '                            'Pending Mayank Testing
    '                            If Line1.AllowCreatedRecord = True Then
    '                                Try
    '                                    Dim dynamicInsertQuery As New StringBuilder("")
    '                                    Dim dynamicColumn As New ArrayList()
    '                                    Dim dynamicColumnValue As New ArrayList()
    '                                    Dim dynamicTableName As String = ""
    '                                    Dim arr = DR(0).Item("DropDown").ToString().Split("-")
    '                                    If UCase(arr(0).ToString) = "STATIC" Then
    '                                        If arr(1).ToString.ToUpper = "USER" Then
    '                                            dynamicTableName = "MMM_MST_USER"
    '                                            dynamicInsertQuery.Append(" insert into " & dynamicTableName.ToString() & " (EID,ModifyDate,lastupdate," & arr(2).ToString() & "")
    '                                            If Not String.IsNullOrEmpty(Line1.OnflyFieldmapping) Then
    '                                                Dim totalSpelitedVal() As String = Line1.OnflyFieldmapping.ToString().Split(",")
    '                                                For commaseparr As Integer = 0 To totalSpelitedVal.Length - 1
    '                                                    Dim arrval As String() = totalSpelitedVal(commaseparr).ToString().Split("-")
    '                                                    'Dim dynval As String = arrval(0).ToString()
    '                                                    dynamicColumn.Add(arrval(0).ToString())
    '                                                    Dim Dynamicval = From cust In objItem.DataItem Where (cust.FieldMapping = "" & arrval(1).ToString() & "") Order By cust.DisplayOrder Select cust
    '                                                    If Dynamicval.Count > 0 Then
    '                                                        For Each singeLineData In Dynamicval
    '                                                            dynamicColumnValue.Add("'" & singeLineData.Values & "'")
    '                                                        Next
    '                                                    Else
    '                                                        dynamicColumnValue.Add("'" & arrval(1).ToString() & "'")
    '                                                    End If

    '                                                Next
    '                                                If dynamicColumn.Count > 0 Then
    '                                                    dynamicInsertQuery.Append("," & String.Join(",", dynamicColumn.ToArray()) & ") values ( " & EID & ",getdate(),getdate(),'" & Line1.Values.Trim() & "',")
    '                                                    dynamicInsertQuery.Append("" & String.Join(",", dynamicColumnValue.ToArray()) & ")")
    '                                                Else
    '                                                    dynamicInsertQuery.Append(") values ( " & EID & ",getdate(),getdate(),'" & Line1.Values.Trim() & "')")
    '                                                End If
    '                                            Else
    '                                                dynamicInsertQuery.Append(") values ( " & EID & ",getdate(),getdate(),'" & Line1.Values.Trim() & "')")
    '                                            End If
    '                                        End If
    '                                    ElseIf UCase(arr(0).ToString()) = "MASTER" Then
    '                                        dynamicTableName = "MMM_MST_MASTER"
    '                                        dynamicInsertQuery.Append(" insert into " & dynamicTableName.ToString() & " (EID,DocumentType,createdBy,UpdatedDate,lastupdate,adate,isauth,Source,NaNcounter,TallyIsActive," & arr(2).ToString() & "")
    '                                        If Not String.IsNullOrEmpty(Line1.OnflyFieldmapping) Then
    '                                            Dim totalSpelitedVal() As String = Line1.OnflyFieldmapping.ToString().Split(",")
    '                                            For commaseparr As Integer = 0 To totalSpelitedVal.Length - 1
    '                                                Dim arrval As String() = totalSpelitedVal(commaseparr).ToString().Split("-")
    '                                                'Dim dynval As String = arrval(0).ToString()
    '                                                dynamicColumn.Add(arrval(0).ToString())
    '                                                Dim Dynamicval = From cust In objItem.DataItem Where (cust.FieldMapping = "" & arrval(1).ToString() & "") Order By cust.DisplayOrder Select cust
    '                                                For Each singeLineData In Dynamicval
    '                                                    dynamicColumnValue.Add("'" & singeLineData.Values & "'")
    '                                                Next
    '                                            Next
    '                                            If dynamicColumn.Count > 0 Then
    '                                                dynamicInsertQuery.Append("," & String.Join(",", dynamicColumn.ToArray()) & ") values ( " & EID & ",'" & DocType & "'," & UID & " getdate(),getdate(),getdate(),1,'WS',0,0,'" & Line1.Values.Trim() & "',")
    '                                                dynamicInsertQuery.Append("" & String.Join(",", dynamicColumnValue.ToArray()) & ")")
    '                                            Else
    '                                                dynamicInsertQuery.Append(") values ( " & EID & ",'" & DocType & "'," & UID & " getdate(),getdate(),getdate(),1,'WS',0,0,'" & Line1.Values.Trim() & "')")
    '                                            End If
    '                                        Else
    '                                            dynamicInsertQuery.Append(") values ( " & EID & ",'" & DocType & "'," & UID & " getdate(),getdate(),getdate(),1,'WS',0,0,'" & Line1.Values.Trim() & "')")
    '                                        End If
    '                                        'ElseIf UCase(arr(0).ToString()) = "DOCUMENT" Then
    '                                        '    dynamicTableName = "MMM_MST_DOC"
    '                                        '    dynamicInsertQuery.Append(" insert into " & dynamicTableName.ToString() & " (EID,cnt,adate,OUID,isworkflow,createdBy,UpdatedDate,lastupdate,,isauth,Source,NaNcounter,TallyIsActive," & arr(2).ToString() & "")
    '                                        '    If Not String.IsNullOrEmpty(Line1. OnflyFieldmapping) Then
    '                                        '        Dim totalSpelitedVal() As String = Line1.OnflyFieldmapping.ToString().Split(",")
    '                                        '        For commaseparr As Integer = 0 To totalSpelitedVal.Length - 1
    '                                        '            Dim arrval As String() = totalSpelitedVal(commaseparr).ToString().Split("-")
    '                                        '            'Dim dynval As String = arrval(0).ToString()
    '                                        '            dynamicColumn.Add(arrval(0).ToString())
    '                                        '            Dim Dynamicval = From cust In objItem.DataItem Where (cust.FieldMapping = "" & arrval(1).ToString() & "") Order By cust.DisplayOrder Select cust
    '                                        '            For Each singeLineData In Dynamicval
    '                                        '                dynamicColumnValue.Add("'" & singeLineData.Values & "'")
    '                                        '            Next
    '                                        '        Next
    '                                        '        If dynamicColumn.Count > 0 Then
    '                                        '            dynamicInsertQuery.Append("," & String.Join(",", dynamicColumn.ToArray()) & ") values ( " & EID & ",'" & DocType & "'," & UID & " getdate(),getdate(),getdate(),1,'WS',0,0,'" & Line1.Values.Trim() & "',")
    '                                        '            dynamicInsertQuery.Append("" & String.Join(",", dynamicColumnValue.ToArray()) & ")")
    '                                        '        Else
    '                                        '            dynamicInsertQuery.Append(") values ( " & EID & ",'" & DocType & "'," & UID & " getdate(),getdate(),getdate(),1,'WS',0,0,'" & Line1.Values.Trim() & "')")
    '                                        '        End If
    '                                        '    Else
    '                                        '        dynamicInsertQuery.Append(") values ( " & EID & ",'" & DocType & "'," & UID & " getdate(),getdate(),getdate(),1,'WS',0,0,'" & Line1.Values.Trim() & "')")
    '                                        '    End If
    '                                    End If
    '                                    If con.State = ConnectionState.Closed Then
    '                                        con.Open()
    '                                    End If
    '                                    dynamicInsertQuery.Append(";SELECT IDENT_CURRENT('" & dynamicTableName.ToString() & "')")
    '                                    Dim da As New SqlDataAdapter(dynamicInsertQuery.ToString(), con)
    '                                    da.SelectCommand.Parameters.Clear()
    '                                    Dim dt As New DataTable
    '                                    da.Fill(dt)
    '                                    StrDDlID = dt.Rows(0)(0).ToString()
    '                                    If arr(1).ToString.ToUpper = "USER" Then
    '                                        Dim DMS As New DMSUtil
    '                                        Try
    '                                            ''Change Required
    '                                            DMS.notificationMail(StrDDlID, EID, "USER", "USER CREATED")
    '                                        Catch ex As Exception
    '                                            StrDDlID = dt.Rows(0)(0).ToString()
    '                                        End Try
    '                                    End If
    '                                Catch ex As Exception
    '                                    StrDDlID = "-1"
    '                                End Try
    '                            Else
    '                                StrDDlID = "-1"
    '                            End If
    '                        End If
    '                    Catch ex As Exception
    '                    End Try

    '                    'Set TID into Drop Down
    '                    Line1.Values = StrDDlID
    '                Next
    '                'Filtering only look up Row 
    '                Dim DropDownLineChild = From cust In objItem.DataItem Where (cust.FieldType = "Drop Down" Or cust.FieldType = "AutoComplete") And cust.DropDownType = "CHILD VALUED" Order By cust.DisplayOrder Select cust
    '                For Each Line1 In DropDownLineChild
    '                    Dim StrDDlID As String = "0"
    '                    '-1 For invalid drop down TID if it is master valued
    '                    'Get Tid of supplyed Drop Down
    '                    Dim DR As DataRow() = ds.Tables(0).Select("FieldID=" & Line1.FieldID)
    '                    If Not String.IsNullOrEmpty(Line1.Values.Trim()) And Line1.Values.Trim().ToUpper() <> "SELECT" And Line1.Values.Trim() <> "0" Then
    '                        StrDDlID = DropDown.GetDropDownChildID(DR(0), EID, Line1.Values.Trim(), objItem)
    '                    End If
    '                    If String.IsNullOrEmpty(StrDDlID) Then
    '                        StrDDlID = "0"
    '                    End If
    '                    Try
    '                        If Convert.ToInt32(StrDDlID) < 0 Then
    '                            StrDDlID = "-1"
    '                        End If
    '                    Catch ex As Exception
    '                    End Try

    '                    'Set TID into Drop Down
    '                    Line1.Values = StrDDlID
    '                Next

    '                Dim LookUPLine = From LookUPRow In objItem.DataItem Where LookUPRow.FieldType = "Lookup" Select LookUPRow
    '                'Getting corresponding look up value from data base
    '                For Each Line In LookUPLine
    '                    'Line.
    '                    Dim strLookUpValue = ""
    '                    If Line.Values.Trim() = "" Then
    '                        strLookUpValue = GetlookUpValue(Line, objItem, ds.Tables(0), EID, con)
    '                        Line.Values = strLookUpValue
    '                    End If
    '                Next
    '                'Filtering only Fixed valued Drop Down 
    '                'Fixed valued drop down validation
    '                Dim FixedDDL = From DDLFXD In objItem.DataItem Where DDLFXD.FieldType = "Drop Down" And DDLFXD.DropDownType.ToUpper = "FIX VALUED" Select DDLFXD
    '                For Each Line In FixedDDL
    '                    'Line.Getting defined value of droop down and valiating it with the supplyed values
    '                    Dim StrAllowedValue = Line.DropDown.ToUpper
    '                    Dim arr = StrAllowedValue.Split(",")
    '                    If Line.Values.ToUpper.Trim = "SELECT" Or Line.Values.Trim = "" Then
    '                        Line.Values = ""
    '                    Else
    '                        Dim FixedDDLValues = From DDLFXD In arr Where DDLFXD.Trim <> "" Select DDLFXD
    '                        Dim ISFound As Boolean = False
    '                        For Each ITEM In FixedDDLValues
    '                            If ITEM.Trim.ToUpper = Line.Values.ToUpper Then
    '                                ISFound = True
    '                                Exit For
    '                            End If
    '                        Next
    '                        'if defined value is not supplyed set -1,So that it will not pass validation while validatiting it.
    '                        If ISFound = True Then
    '                            Line.Values = Line.Values.ToUpper
    '                        Else
    '                            Line.Values = "-1"
    '                        End If
    '                    End If
    '                Next
    '                'Now Pupulating DDLLookup
    '                Dim DDLLookuup1 = From cust In objItem.DataItem Where cust.FieldType = "LookupDDL" Order By cust.DisplayOrder Select cust

    '                For Each Line12 In DDLLookuup1
    '                    Dim StrDDlID As String = "0"
    '                    '-1 For invalid drop down TID if it is master valued
    '                    'Get Tid of supplyed Drop Down
    '                    Dim DR As DataRow() = ds.Tables(0).Select("FieldID=" & Line12.FieldID)
    '                    'If Not String.IsNullOrEmpty(Line12.Values.Trim()) And Line12.Values.Trim().ToUpper() <> "SELECT" And Line12.Values.Trim() <> "0" Then
    '                    StrDDlID = DDLookUp.GetTID(DR(0), EID, Line12.Values.Trim(), objItem)
    '                    'End If
    '                    If String.IsNullOrEmpty(StrDDlID) Then
    '                        StrDDlID = "0"
    '                    End If
    '                    Try
    '                        If Convert.ToInt32(StrDDlID) < 0 Then
    '                            StrDDlID = "-1"
    '                        End If
    '                    Catch ex As Exception
    '                    End Try
    '                    'Set TID into Drop Down
    '                    Line12.Values = StrDDlID
    '                    Line12.DDLValue = Line12.Values.Trim()
    '                Next
    '                Dim CheckBoxList = From cust In objItem.DataItem Where cust.FieldType = "CheckBox List" Order By cust.DisplayOrder Select cust

    '                For Each Item In CheckBoxList
    '                    Dim StrCHKID As String = "0"
    '                    Dim DR As DataRow() = ds.Tables(0).Select("FieldID=" & Item.FieldID)
    '                    If Not String.IsNullOrEmpty(Item.Values.Trim()) And Item.Values.Trim().ToUpper() <> "SELECT" And Item.Values.Trim() <> "0" Then
    '                        StrCHKID = ChkBoxList.GetCHKID(DR(0), EID, Item.Values.Trim(), objItem)
    '                    End If
    '                    If String.IsNullOrEmpty(StrCHKID) Then
    '                        StrCHKID = "0"
    '                    End If

    '                    'Set TID into Drop Down
    '                    Item.Values = StrCHKID
    '                Next

    '                'Getting all CalCulative field for evaluating it from expression.
    '                Dim CalCulativeField = From CalRow In objItem.DataItem Where CalRow.FieldType = "Calculative Field" Select CalRow
    '                Dim Cal_Text = ""
    '                For Each line In CalCulativeField
    '                    Cal_Text = line.CalText.ToString()
    '                    'Calculating its equivalent values
    '                    line.Values = EvaluateFormula(Cal_Text, line.DisplayName, objItem)
    '                Next
    '            Next
    '        Next
    '        '
    '        Dim MainForm = From Form In LstData Where Form.DocumentType = DocType Select Form
    '        'Getting all the childitem total for its evalution 
    '        For Each Form In MainForm
    '            For Each objItem In Form.LineItem
    '                Dim ChidItemTotalRows = From ChldTotal In objItem.DataItem Where ChldTotal.FieldType = "Child Item Total" Select ChldTotal
    '                For Each line In ChidItemTotalRows
    '                    'Getting details of concern Field
    '                    Dim DR1 As DataRow() = ds.Tables(0).Select("FieldID=" & line.DropDown)
    '                    Dim ConcernDocType = DR1(0).Item("DocumentType").ToString()
    '                    Dim DisplayName = DR1(0).Item("DisplayName").ToString()
    '                    Dim ChildForm = From LstChild In LstData Where LstChild.DocumentType = ConcernDocType Select LstChild
    '                    Dim result = 0.0F
    '                    For Each var In ChildForm
    '                        result = 0
    '                        For Each var1 In var.LineItem
    '                            For Each var3 In var1.DataItem
    '                                Dim Val = 0.0F
    '                                If IsNumeric(var3.Values) = False Then
    '                                    Val = 0
    '                                Else
    '                                    Val = var3.Values
    '                                End If
    '                                If var3.DisplayName = DisplayName Then
    '                                    result = result + Val
    '                                End If
    '                            Next
    '                        Next
    '                    Next
    '                    line.Values = result
    '                Next
    '            Next
    '        Next
    '    Catch ex As Exception
    '    Finally
    '        If Not con Is Nothing Then
    '            con.Close()
    '            con.Dispose()
    '        End If
    '    End Try
    '    Return LstData
    'End Function

    Public Shared Function EvaluateChildItemTotal(ObjData As Object, FieldtoBeCalculated As String) As String
        Dim result = 0
        For Each Form In ObjData
            For Each obj In Form
                Dim Val = 0
                If obj.DisplayName = FieldtoBeCalculated Then
                    Integer.TryParse(obj.Values, Val)
                    result = result + Val
                End If
            Next
        Next
        Return result.ToString()
    End Function

    Public Shared Function EvaluateFormula(Cal_Text As String, DisplayName As String, objLine As LineitemWrap) As String
        Dim result = "0"
        Dim Expression = ""
        Try
            If Cal_Text.Trim() <> "" Then
                'Split with , becouse more then one formula may exists
                Dim arrCal As String() = (Cal_Text & ",").Trim().Split(",")
                For i As Integer = 0 To arrCal.Count - 1
                    If arrCal(i).Trim() <> "" Then
                        'split with = for getting its display Name
                        Dim arr As String() = arrCal(i).Trim().Split("=")
                        'Replace {,} becouse it is reserved 
                        If arr(0).Trim().Replace("{", "").Replace("}", "") = DisplayName Then
                            Expression = arr(1).Trim()
                            'Loop through all the field for creating expression
                            For Each line In objLine.DataItem
                                'Replacing it with a relavent value
                                If Expression.Contains("{" & line.DisplayName & "}") Then
                                    line.Values = line.Values.Replace(",", String.Empty)
                                    Dim Val As Double = 0.0
                                    'If IsNumeric(line.Values) = False Then
                                    '    Val = 0.0
                                    'Else
                                    '    Val = line.Values
                                    'End If
                                    Expression = Expression.Replace("{" & line.DisplayName & "}", If(IsNumeric(line.Values), line.Values, "0.0"))
                                End If
                            Next
                            'Removing {,} From the expression
                            Expression = Expression.Trim().Replace("{", "").Replace("}", "")
                            'Finally evaluating its value with JUGAD
                            Dim res As Double = 0.0
                            res = New DataTable().Compute(Expression, "0").ToString()
                            result = res.ToString()
                        End If
                    End If
                Next
            End If
        Catch ex As Exception
            Return "0"
        End Try
        Return result
    End Function

    Public Shared Function GetlookUpValue(UData As UserData, lstData As LineitemWrap, DtField As DataTable, EID As Integer, ByRef con As SqlConnection) As String
        Dim StrResult = ""
        Dim strlookUp As String = ""
        Dim lookupTableName = ""
        Dim lookupDocType = ""
        Dim da As SqlDataAdapter = Nothing
        Try
            'Getting ID dropdown field  on which lookup has been set
            Dim l_Field_DDLID = Convert.ToString(UData.DropDown)

            Dim lookupFieldTID = Convert.ToString(UData.FieldID)
            'Selecting data row of concern lookup
            Dim Row As DataRow() = DtField.Select("FieldID='" & l_Field_DDLID & "'")
            'Gtting display name of the drop dwon list
            Dim ddlDisplayName = Row(0).Item("DisplayName").ToString()
            'getting look up value
            strlookUp = Row(0).Item("lookupvalue").ToString()

            'Getting Table Name of dropdown list
            Dim arrtble As String() = Row(0).Item("DropDown").ToString().Split("-")
            'MASTER-Bank -fld1
            If arrtble(0).ToUpper() = "MASTER" Then
                lookupTableName = "MMM_MST_MASTER"
            ElseIf arrtble(0).ToUpper() = "DOCUMENT" Then
                lookupTableName = "MMM_MST_DOC"
            ElseIf arrtble(1).ToString.ToUpper = "USER" Then
                lookupTableName = "MMM_MST_USER"
            Else
                lookupTableName = "MMM_MST_DOC_ITEM"
            End If
            Dim DocumentType = arrtble(1)
            If arrtble(1).Contains(":") Then
                Dim arrt22 = arrtble(1).Split(":")
                DocumentType = arrt22(1)
            End If

            'Now get lookup field mapping 
            Dim strlookupMapping = ""
            If Not (strlookUp Is Nothing) And (strlookUp <> "") Then
                Dim arrlookUp As String() = strlookUp.Split(",")
                Dim liveLookUp = From fldMapping In arrlookUp Where fldMapping <> "" And Not fldMapping Is Nothing Select fldMapping
                For Each lookUp In liveLookUp
                    Dim arr As String() = lookUp.ToString().Split("-")
                    If arr(0).Trim() = lookupFieldTID.Trim() Then
                        strlookupMapping = arr(1)
                        Exit For
                    End If
                Next
            End If
            'Getting Document Name  name of DropDown
            lookupDocType = arrtble(1)
            'Getting only that lookup that has valid field mapping
            'loop for getting DOCID of thd drop down list
            'Appending concern value field into select  statement string
            'Dim tid = From LookUPRow In lstData.LineItem Where LookUPRow.FieldType = "Lookup" Select LookUPRow
            Dim strTID = "0"
            For Each ObjData In lstData.DataItem
                If ObjData.DisplayName = ddlDisplayName Then
                    strTID = ObjData.Values
                    Exit For
                End If
            Next
            'Getting field type of concern field 
            Dim StrQuery = "select * FROM MMM_MST_FIelds where EID= " & EID & " AND Documenttype='" & DocumentType & "' AND  FIeldMapping='" & strlookupMapping & "'"
            da = New SqlDataAdapter(StrQuery, con)
            da.SelectCommand.Parameters.Clear()
            Dim dslookup As New DataSet()
            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            da.Fill(dslookup)
            Dim lookupSelectString = ""
            If dslookup.Tables(0).Rows.Count > 0 Then
                If dslookup.Tables(0).Rows(0).Item("FieldType").ToString().ToUpper() = "DROP DOWN" And (dslookup.Tables(0).Rows(0).Item("DropDowntype").ToString().ToUpper() = "MASTER VALUED" Or dslookup.Tables(0).Rows(0).Item("DropDowntype").ToString().ToUpper() = "SESSION VALUED") Then
                    'if the field is of dropdowntype and master valued
                    lookupSelectString = "select " & strlookupMapping & " FROM " & lookupTableName & " WHERE tid= " & strTID
                    da = New SqlDataAdapter(lookupSelectString, con)
                    da.SelectCommand.Parameters.Clear()
                    Dim dslookup1 As New DataSet()
                    da.Fill(dslookup1)
                    Dim arrtble1 As String() = dslookup.Tables(0).Rows(0).Item("DropDown").ToString().Split("-")

                If arrtble1(0).ToUpper() = "MASTER" Then
                    lookupTableName = "MMM_MST_MASTER"
                ElseIf arrtble1(0).ToUpper() = "DOCUMENT" Then
                    lookupTableName = "MMM_MST_DOC"
                ElseIf arrtble1(1).ToString.ToUpper = "USER" Then
                    lookupTableName = "MMM_MST_USER"
                Else
                    lookupTableName = "MMM_MST_DOC_ITEM"
                End If
                If lookupTableName = "MMM_MST_DOC_ITEM" Then
                    lookupTableName = "MMM_MST_MASTER"
                End If

                Dim ddlTID = dslookup1.Tables(0).Rows(0).Item(strlookupMapping).ToString()
                lookupSelectString = "select " & arrtble1(2) & " FROM " & lookupTableName & " WHERE tid= " & ddlTID
                strlookupMapping = arrtble1(2)

            Else
                lookupSelectString = "select " & strlookupMapping & " FROM " & lookupTableName & " WHERE tid= " & strTID
 End If
            Else
                If lookupTableName = "MMM_MST_USER" Then
                    lookupSelectString = "select " & strlookupMapping & " FROM " & lookupTableName & " WHERE uid= " & strTID
                End If
            End If
            'getting lookup value from the database
            'con = New SqlConnection(conStr)
            da = New SqlDataAdapter(lookupSelectString, con)
            da.SelectCommand.Parameters.Clear()
            dslookup = New DataSet()
            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            da.Fill(dslookup)
            'Appending that lookup field into insert statement
            If dslookup.Tables(0).Rows.Count > 0 Then
                StrResult = dslookup.Tables(0).Rows(0).Item(strlookupMapping).ToString()
            End If
        Catch ex As Exception
            Return ""
        Finally
            If Not da Is Nothing Then
                da.Dispose()
            End If
        End Try
        Return StrResult
    End Function

    Public Shared Function CreateListCollection(UData As String, DtField As DataTable, EID As Integer, DocType As String) As LineitemWrap
        Dim lstData1 As New List(Of UserData)
        Dim LineItem As New LineitemWrap()
        Dim Obj As UserData
        Dim arrData As String() = UData.Split("|")

        Try
            If DocType.ToUpper() = "USER" Then
                'Adding rows 
                AddUserStaticField(DtField)
            End If
        Catch ex As Exception

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
                Obj.DocumentType = DtField.Rows(i).Item("Documenttype").ToString()
                Obj.InVisible = DtField.Rows(i).Item("InVisible").ToString()
                Obj.ddlval = DtField.Rows(i).Item("ddlval").ToString()
                Obj.isEditable = DtField.Rows(i).Item("isEditable").ToString()
                Obj.AllowDecimalDigit = DtField.Rows(i).Item("AllowDecimalDigit").ToString()
                'Changes by Mayank
                Obj.AllowCreatedRecord = DtField.Rows(i).Item("AllowCreateRecord_onfly")
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
                    IsParameterFound = True
                End If
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

    Public Shared Function GenerateQuery(EID As Integer, DocType As String, UID As String, TableName As String, Data As String, ds As DataSet) As String
        Dim Result As String = ""
        Dim DOCID As Integer = 0
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim da As SqlDataAdapter = Nothing
        Dim con As SqlConnection = Nothing

        Dim strQuery As New StringBuilder("")
        'INSERT INTO MMM_MST_MASTER(EID,Documenttype,createdby,updateddate,", "VALUES (
        Dim strFields As New StringBuilder("INSERT INTO MMM_MST_Master(EID,Documenttype,CreatedBy,UpdatedDate ")
        Dim strValue As New StringBuilder("VALUES") '= "VALUES (" & EID & ",'" & DocType & "'," & UID & ",getdate(),"
        strValue.Append("(" & EID & ",'" & DocType & "'," & UID & ",getdate() ")
        Dim DisplayName As String = ""
        Dim DropDownType As String = ""
        Dim FieldType As String = ""
        Dim arrData As String() = Data.Split("|")
        Dim lstData As New List(Of UserData)
        Dim objUdata As UserData
        Try
            '######################'Code For saving document#######################
            'con = New SqlConnection(conStr)
            'da = New SqlDataAdapter()

            Dim onlyFiltered As DataView = ds.Tables("fields").DefaultView
            onlyFiltered.RowFilter = "FieldType<>'Formula field' AND FieldType <> 'Calculative Field' AND FieldType <>  'Lookup' AND FieldType <> 'Child Item'"
            Dim theFlds As DataTable = onlyFiltered.Table.DefaultView.ToTable()
            theFlds.Columns.Add("Values")
            Dim IsvalidDDlIDExists = True
            Dim StrDDlID = "0"
            Dim StrKey = "", strValue1 = ""
            For i As Integer = 0 To theFlds.Rows.Count - 1
                'strFields =strFields & "," & 
                objUdata = New UserData()
                DisplayName = theFlds.Rows(i).Item("DisplayName")

                'Appending fieldmapping into insert statement string
                For j As Integer = 0 To arrData.Count - 1
                    'Dim arr As String() = Split(arrData(j).ToString(), "::")
                    Dim arr As String() = Split(arrData(j).ToString(), "::")
                    StrKey = theFlds.Rows(i).Item("FieldMapping")
                    strValue1 = arr(1)
                    If arr(0) = DisplayName Then
                        'Get ID of concern Document
                        'IF drop down is required 
                        If FieldType = "Drop Down" And DropDownType = "MASTER VALUED" Then
                            'StrDDlID = DropDown.GetDropDownID(theFlds.Rows(i), EID, arr(1))
                            If theFlds.Rows(i).Item("Isrequired").ToString() = "1" Then
                                If Convert.ToInt32(StrDDlID) <= 0 Then
                                    IsvalidDDlIDExists = False
                                    Exit For
                                End If
                            Else
                                If Convert.ToInt32(StrDDlID) <= 0 Then
                                    '-3 if user supplyed "0" or empty
                                    If StrDDlID = "-3" Then
                                        StrDDlID = "0"
                                    Else
                                        IsvalidDDlIDExists = False
                                        Exit For
                                    End If
                                End If
                            End If
                        End If
                        'Appending concern value field into insert statement string
                        strFields.Append(", " & StrKey)
                        strValue.Append(", '" & strValue1 & "'")
                        'Only one data can be found
                        objUdata.DisplayName = DisplayName
                        objUdata.FieldID = theFlds.Rows(i).Item("FieldID")
                        objUdata.FieldMapping = StrKey
                        objUdata.FieldType = theFlds.Rows(i).Item("FieldType")
                        objUdata.Values = strValue1
                        lstData.Add(objUdata)
                        Exit For
                    End If
                Next
                If IsvalidDDlIDExists = False Then
                    Result = "Sorry!!! Transaction ID of master valued data does not exists in our database. "
                    Exit For
                End If
            Next
            strQuery.Append(strFields.ToString() & ")")
            strQuery.Append(strValue.ToString() & ")")
            strQuery.Append(" ; Select @@identity")
            con = New SqlConnection(conStr)
            da = New SqlDataAdapter(strQuery.ToString(), con)
            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            DOCID = da.SelectCommand.ExecuteScalar()
            If DOCID > 0 Then
                Result = "Your DocID is " & DOCID
            End If
        Catch ex As Exception
            Return "Sorry!!! Your request can not be completed at the moment. Try again later."
        Finally
            If Not con Is Nothing Then
                con.Close()
                con.Dispose()
            End If
            If Not da Is Nothing Then
                da.Dispose()
            End If
        End Try
        Return Result
    End Function

    Public Shared Function GetDropDownID(DRDDL As DataRow, EID As Integer, DDlText As String) As String
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
        Try
            If DDlText.Trim().ToLower() = "select" Or DDlText = "0" Then
                'values supplyed 
                StrResult = "0"
            Else

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
                    End If
                End If
                Dim lookUpqry As String = ""

                If arr(0).ToUpper() = "CHILD" Then
                    strQuery = "select " & arr(2).ToString() & "," & TID & "[tid]  from " & TABLENAME & " M WHERE   DOCUMENTTYPE='" & arr(1).ToString()
                    strQuery = strQuery & " and M." & arr(arr.Length - 1) & " = '" & DDlText & "'"
                    TID = "tid"
                ElseIf arr(0).ToUpper() <> "STATIC" Then
                    strQuery = "select " & arr(2).ToString() & "," & TID & "[tid]" & lookUpqry & " from " & TABLENAME & " M WHERE EID=" & EID & " AND  DOCUMENTTYPE='" & arr(1).ToString() & "'"
                    strQuery = strQuery & " and M." & arr(arr.Length - 1) & " = '" & DDlText & "'"
                    strQuery = strQuery & "  AND M.isauth=1 "
                    TID = "tid"

                Else
                    If arr(2).ToString.ToUpper = "LOCATIONSTATE" Then
                        strQuery = "select DISTINCT " & arr(2).ToString() & ",SID [tid]" & lookUpqry & " from " & TABLENAME & " M " & "' and " & arr(arr.Length - 1) & "'" & DDlText & "'"
                        strQuery = strQuery & " and M." & arr(arr.Length - 1) & " = '" & DDlText & "'"
                        TID = "tid"

                    Else
                        strQuery = "select " & arr(2).ToString() & "," & TID & "[tid]" & lookUpqry & " from " & TABLENAME & " M WHERE EID=" & EID & " " & "' "
                        strQuery = strQuery & " and M." & arr(arr.Length - 1) & " = '" & DDlText & "'"
                        TID = "tid"
                    End If
                End If

                If arr(0).ToString.ToUpper = "SESSION" Then
                    strQuery = "select " & arr(2).ToString() & " from " & TABLENAME & " M WHERE EID=" & EID & " " & ""
                    strQuery = strQuery & " and UserName = '" & DDlText & "'"
                    TID = "UID"
                End If

                Dim xwhr As String = ""
                Dim tids As String = ""
                'Dim tidarr() As String

                Dim AutoFilter As String = DRDDL.Item("AutoFilter").ToString()
                If AutoFilter.Length > 0 Then
                    If arr(0).ToUpper() = "CHILD" Then
                        If AutoFilter.ToUpper = "DOCID" Then
                            strQuery = GetQuery1(arr(1).ToString, arr(2).ToString(), EID, DDlText)
                        Else
                            strQuery = GetQuery(arr(1).ToString, arr(2).ToString, EID, DDlText)
                        End If
                    ElseIf arr(0).ToUpper() <> "STATIC" Then
                        strQuery = "select " & arr(2).ToString() & ",convert(nvarchar(10),tid)  [tid]" & lookUpqry & " from " & TABLENAME & " M WHERE EID=" & EID & " AND  DOCUMENTTYPE='" & arr(1).ToString() & "'"
                        strQuery = strQuery & "  AND M.isauth=1 " & xwhr
                        strQuery = strQuery & " and M." & arr(arr.Length - 1) & " = '" & DDlText & "'"
                        TID = "tid"
                    Else
                        strQuery = "select " & arr(2).ToString() & ",convert(nvarchar(10)," & TID & ")  [tid]" & lookUpqry & " from " & TABLENAME & " M WHERE EID=" & EID & " "
                        strQuery = strQuery & "  AND M.isauth=1 "
                        strQuery = strQuery & " and M." & arr(arr.Length - 1) & " = '" & DDlText & "'"
                        TID = "tid"
                    End If
                End If
            End If

            If strQuery <> "" Then
                con = New SqlConnection(conStr)
                con.Open()
                sda = New SqlDataAdapter(strQuery, con)
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
            If Not con Is Nothing Then
                sda.Dispose()
            End If
        End Try
        Return StrResult
    End Function

    Public Shared Sub bind(ByVal id1 As Integer, ByVal MstValue As Integer, ByRef LstData As List(Of DataWraper1))
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        If con.State <> ConnectionState.Open Then
            con.Open()
        End If

        Dim oda As SqlDataAdapter = New SqlDataAdapter("select LOOKUPVALUE,dropdown,documenttype,dropdowntype from MMM_MST_FIELDS WHERE FIELDID=" & id1 & "", con)
        'Dim ddlo As DropDownList
        Try
            Dim DS As New DataSet
            Dim xwhr As String = ""
            oda.Fill(DS, "data")
            Dim LOOKUPVALUE As String = DS.Tables("data").Rows(0).Item("lookupvalue").ToString()
            Dim documenttype() As String = DS.Tables("data").Rows(0).Item("dropdown").ToString.Split("-")
            If LOOKUPVALUE.Length > 0 Then
                Dim lookfld() As String = LOOKUPVALUE.ToString().Split(",")  '' get all controls to fill in lookup
                If lookfld.Length > 0 Then
                    For iLookFld As Integer = 0 To lookfld.Length - 1            '' loop for lookup vals 
                        Dim fldPair() As String = lookfld(iLookFld).Split("-")   '' get fieldid and mapping
                        If fldPair.Length > 1 Then
                            ' If GetControl(pnlFields, "fld" & Val(fldPair(0)).ToString()) Then   '' check if control to be filled exists
                            oda = New SqlDataAdapter("SELECT * FROM MMM_MST_FIELDS WHERE FIELDID=" & fldPair(0) & "", con)  ' get fld dtl from fld master
                            Dim dt As New DataTable
                            oda.Fill(dt)
                            Dim STR As String = ""

                            If fldPair(1).ToString.ToUpper = "R" Then
                                Dim TAB1 As String = ""
                                Dim TAB2 As String = ""
                                Dim STID As String = ""
                                Dim TID As String = ""
                                If documenttype(0).ToString.ToUpper = "MASTER" Then
                                    TAB2 = "MMM_MST_MASTER"
                                    TID = "TID"
                                ElseIf documenttype(0).ToString.ToUpper = "DOCUMENT" Then
                                    TAB2 = "MMM_MST_DOC"
                                    TID = "TID"
                                ElseIf documenttype(1).ToString.ToUpper = "USER" Then
                                    TAB2 = "MMM_MST_USER"
                                    TID = "UID"
                                End If
                                Dim DOCTYPE() As String = dt.Rows(0).Item("DROPDOWN").ToString.Split("-")
                                If DOCTYPE(0).ToString.ToUpper = "MASTER" Then
                                    TAB1 = "MMM_MST_MASTER"
                                    STID = "TID"
                                ElseIf DOCTYPE(0).ToString.ToUpper = "DOCUMENT" Then
                                    TAB1 = "MMM_MST_DOC"
                                    STID = "TID"
                                ElseIf DOCTYPE(1).ToString.ToUpper = "USER" Then
                                    TAB1 = "MMM_MST_USER"
                                    STID = "UID"
                                End If
                                Dim DROPDOWN1 As String = dt.Rows(0).Item("AUTOFILTER").ToString()
                                ''Dim DDL0 As DropDownList = TryCast(pnlFields.FindControl("fld" & DROPDOWN1), DropDownList)
                                oda.SelectCommand.CommandType = CommandType.StoredProcedure
                                oda.SelectCommand.Parameters.Clear()
                                oda.SelectCommand.CommandText = "USP_GETMANNUALFILTER"
                                oda.SelectCommand.Parameters.AddWithValue("@EID", HttpContext.Current.Session("EID"))
                                oda.SelectCommand.Parameters.AddWithValue("@TAB1", TAB1)
                                oda.SelectCommand.Parameters.AddWithValue("@TAB2", TAB2)
                                oda.SelectCommand.Parameters.AddWithValue("@DOCUMENTTYPE", DOCTYPE(1).ToString)
                                oda.SelectCommand.Parameters.AddWithValue("@FLDMAPPING", DOCTYPE(2).ToString)
                                oda.SelectCommand.Parameters.AddWithValue("@AUTOFILTER", dt.Rows(0).Item("AUTOFILTER").ToString())
                                oda.SelectCommand.Parameters.AddWithValue("@TID", TID)
                                oda.SelectCommand.Parameters.AddWithValue("@STID", STID)
                                If documenttype(1).ToString.ToUpper = "USER" Then
                                    oda.SelectCommand.Parameters.AddWithValue("@VAL", HttpContext.Current.Session("uid"))
                                Else
                                    oda.SelectCommand.Parameters.AddWithValue("@VAL", MstValue)
                                End If

                                Dim dss As New DataTable
                                oda.Fill(dss)
                                For Each objData In LstData
                                    'loop For line item
                                    For Each objItem In objData.LineItem
                                        con = New SqlConnection(conStr)

                                        Dim DropDownLine = From cust In objItem.DataItem Where (cust.FieldType = "Drop Down" Or cust.FieldType = "AutoComplete") And cust.DropDownType = "MASTER VALUED" Or cust.DropDownType = "SESSION VALUED" Order By cust.DisplayOrder Select cust

                                        For Each Line1 In DropDownLine
                                            Dim StrDDlID As String = "0"

                                            ' Dim DR As DataRow() = DS.Tables(0).Select("FieldID=" & Line1.FieldID)
                                            If Line1.FieldID.ToString = fldPair(0).ToString Then
                                                If dss.Rows.Count > 0 Then
                                                    If documenttype(1).ToString.ToUpper = "USER" Then
                                                        Line1.Values = dss.Rows(0).Item(1).ToString
                                                    Else
                                                        Line1.Values = dss.Rows(0).Item(0).ToString
                                                    End If

                                                End If
                                            End If
                                        Next
                                    Next
                                Next

                                'Dim ddl1 As DropDownList = TryCast(pnlFields.FindControl("fld" & fldPair(0).ToString()), DropDownList)

                            End If
                            ' End If
                        End If
                    Next
                End If
            End If

        Catch ex As Exception
        Finally
            If Not con Is Nothing Then
                con.Dispose()
            End If
            If Not oda Is Nothing Then
                oda.Dispose()
            End If
        End Try
    End Sub

    Public Shared Function GenerateQueryForCalulativeField(CalFieldID As Integer, ds As DataSet) As String
        Dim Result = ""
        Try

        Catch ex As Exception
            Return ""
        End Try
        Return Result
    End Function

    Public Shared Function GetDropDownDefaultValue(EID As Integer, DocumentType As String) As DataSet
        Dim ds As New DataSet()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim strQuery As String = ""
        Dim con As SqlConnection = Nothing
        Dim sda As SqlDataAdapter = Nothing
        Try
            'SELECT TID,isnull(FLD1,'') 'TextField' FROM MMM_MST_DOC WHERE EID=36 AND DocumentType='Invoice'
            strQuery = "select * FROM MMM_MST_Master WHERE eid=" & EID & " AND Documenttype='" & DocumentType & "_master' AND isAuth=1"
            con = New SqlConnection(conStr)
            con.Open()
            sda = New SqlDataAdapter(strQuery, con)
            sda.Fill(ds)
        Catch ex As Exception
            Throw
        Finally
            If Not con Is Nothing Then
                con.Close()
                con.Dispose()
            End If
            If Not sda Is Nothing Then
                sda.Dispose()
            End If
        End Try
        Return ds
    End Function

    Public Shared Function SaveServicerequest(Data As StringBuilder, ServiceName As String, EventName As String, Result As String) As String
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = Nothing
        Dim da As SqlDataAdapter = Nothing
        Dim StrResult = ""
        'Dim StrQuery = "insert into MMM_MST_ServiceRequestLog(Data)  values ('" & Data.ToString() & "')"
        Try
            con = New SqlConnection(conStr)
            da = New SqlDataAdapter("InsertServiceRequest", con)
            da.SelectCommand.CommandType = CommandType.StoredProcedure
            da.SelectCommand.Parameters.AddWithValue("@Data", Data.ToString())
            da.SelectCommand.Parameters.AddWithValue("@SNAME", ServiceName)
            da.SelectCommand.Parameters.AddWithValue("@ENAME", EventName)
            da.SelectCommand.Parameters.AddWithValue("@Response", Result)
            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            da.SelectCommand.ExecuteNonQuery()
        Catch ex As Exception
        Finally
            If Not con Is Nothing Then
                con.Close()
                con.Dispose()
            End If
            If Not da Is Nothing Then
                da.Dispose()
            End If
        End Try
        Return StrResult
    End Function

    Public Shared Function FormValidation(ByVal doctype As String, ByVal eid As Integer, ByVal Dtab As DataTable, ByVal Action As String, Optional ByVal tid As Integer = 0, Optional ByVal DocNat As String = "CREATE") As String
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim da As New SqlDataAdapter("", con)
        Try
            Dim datatype As String
            Dim datatype1 As String
            Dim errmsg As String = ""
            Dim txtobj1 As String
            Dim txtobj As String
            Dim ddltype As String
            Dim ddltype1 As String
            Dim arr1() As String
            Dim rtn As Integer
            da.SelectCommand.CommandText = "select * from MMM_MST_FORMVALIDATION where eid=" & eid & " and doctype='" & doctype & "' and docNature='" & DocNat & "'"
            Dim ds As New DataSet
            da.Fill(ds, "data")
            For Each rw As DataRow In ds.Tables("data").Rows

                If rw.Item("Valtype").ToString.ToUpper <> "DYNAMIC" Then
                    da.SelectCommand.CommandText = "select datatype from mmm_mst_fields where eid=" & eid & " and documenttype='" & doctype & "' and fieldid=" & Right(rw.Item("fldID"), rw.Item("fldID").ToString.Length - 3) & ""
                    If con.State <> ConnectionState.Open Then
                        con.Open()
                    End If
                    datatype = da.SelectCommand.ExecuteScalar()
                    If rw.Item("Valtype").ToString.ToUpper = "FIELD" Then
                        da.SelectCommand.CommandText = "select datatype from mmm_mst_fields where eid=" & eid & " and documenttype='" & doctype & "' and fieldid=" & Right(rw.Item("Value"), rw.Item("Value").ToString.Length - 3) & ""
                        datatype1 = da.SelectCommand.ExecuteScalar
                    End If

                    If rw.Item("Valtype").ToString.ToUpper = "STATIC" Then
                        For Each rw1 As DataRow In Dtab.Rows
                            If Right(rw.Item("fldID"), rw.Item("fldID").ToString.Length - 3) = rw1.Item("fieldid").ToString Then
                                txtobj = rw1.Item("Value").ToString
                            End If
                        Next
                    ElseIf rw.Item("Valtype").ToString.ToUpper = "FIELD" Then
                        For Each rw1 As DataRow In Dtab.Rows
                            If Right(rw.Item("fldID"), rw.Item("fldID").ToString.Length - 3) = rw1.Item("fieldid").ToString Then
                                txtobj = rw1.Item("Value").ToString
                            End If
                            If Right(rw.Item("Value"), rw.Item("Value").ToString.Length - 3) = rw1.Item("fieldid").ToString Then
                                txtobj1 = rw1.Item("Value").ToString
                            End If
                        Next
                    ElseIf rw.Item("Valtype").ToString.ToUpper = "MANDATORY" Then
                        For Each rw1 As DataRow In Dtab.Rows
                            If Right(rw.Item("fldID"), rw.Item("fldID").ToString.Length - 3) = rw1.Item("fieldid").ToString Then
                                txtobj = rw1.Item("Value").ToString
                            End If
                        Next
                    End If
                End If

                Dim tbname As String = ""
                Dim sb As String = ""
                Dim sts As String = ""

                If rw.Item("Valtype").ToString.ToUpper = "DYNAMIC" Then
                    Dim ddlobj As String = ""
                    For Each rw1 As DataRow In Dtab.Rows
                        If Right(rw.Item("fldID"), rw.Item("fldID").ToString.Length - 3) = rw1.Item("fieldid").ToString Then
                            ddlobj = rw1.Item("Value").ToString
                        End If
                    Next
                    arr1 = rw.Item("value").ToString.Split("-")
                    Dim arr2 As String()
                    Dim CNT As Integer = arr1.Length
                    For i As Integer = 1 To arr1.Length - 1
                        arr2 = Split(arr1(i), ":")
                        If i = 1 Then
                            tbname = arr2(0).ToString
                        End If
                        If arr2.Length > 1 Then
                            sb = sb & arr2(1).ToString & ","
                        End If
                        If i > 1 Then
                            Dim opr As String = Left(arr2(0), 1)
                            Dim opr1 As String = Left(arr2(0), 2)
                            opr1 = opr1.Replace(opr, "")

                            If opr1 = "=" Then
                                opr = opr & opr1
                            End If

                            Dim txt As String = Right(arr2(0), arr2(0).Length - opr.Length).ToString
                            Dim txtbx1 As String
                            For Each rw1 As DataRow In Dtab.Rows
                                If Right(txt, txt.ToString.Length - 3) = rw1.Item("fieldid").ToString Then
                                    txtbx1 = rw1.Item("Value").ToString
                                End If
                            Next
                            Dim ddllst As DropDownList
                            da.SelectCommand.CommandText = "select datatype from mmm_mst_fields where eid=" & eid & " and fieldid=" & Right(txt, txt.ToString.Length - 3) & ""
                            If con.State <> ConnectionState.Open Then
                                con.Open()
                            End If
                            datatype = da.SelectCommand.ExecuteScalar()
                            If CNT - 1 <> i Then
                                If IsNothing(ddllst) Then
                                    If datatype.ToUpper = "NUMERIC" Then
                                        sts = sts & opr & "convert(float," & txtbx1.ToString & ")" & "|"
                                    ElseIf datatype.ToUpper = "DATETIME" Then
                                        sts = sts & opr & " convert(datetime," & "'" & getdate(txtbx1.ToString) & "')" & "|"
                                    Else
                                        sts = sts & opr & "'" & txtbx1.ToString & "'" & "|"
                                    End If
                                Else
                                    sts = sts & opr & "'" & ddllst.SelectedValue & "'" & "|"
                                End If
                            Else
                                If IsNothing(ddllst) Then
                                    If datatype.ToUpper = "NUMERIC" Then
                                        sts = sts & opr & "convert(float," & txtbx1.ToString & ")" & "|"
                                    ElseIf datatype.ToUpper = "DATETIME" Then
                                        sts = sts & opr & " convert(datetime," & "'" & getdate(txtbx1.ToString) & "')" & "|"
                                    Else
                                        sts = sts & opr & "'" & txtbx1.ToString & "'" & "|"
                                    End If
                                Else
                                    sts = sts & opr & "'" & ddllst.SelectedValue & "'" & "|"
                                End If
                            End If
                        End If
                    Next
                    sb = Left(sb, sb.Length - 1)

                    da.SelectCommand.CommandText = "select " & sb & " from " & tbname & " where tid=" & ddlobj.ToString & ""
                    Dim DS1 As New DataSet
                    da.Fill(DS1, "DATA")
                    Dim V1 As String = ""
                    Dim sarr() As String
                    Dim QR As String = " "
                    sarr = sts.ToString.Split("|")
                    If DS1.Tables("DATA").Rows.Count > 0 Then
                        For X As Integer = 0 To DS1.Tables("DATA").Columns.Count - 1
                            V1 = DS1.Tables("DATA").Rows(0).Item(X).ToString
                            If V1.ToString.Length > 6 And (V1.ToString.Contains("/") Or V1.ToString.Contains("-")) And Left(V1, 1).Contains("-") = False Then
                                QR = QR & " Convert(datetime," & "'" & getdate(V1) & "')" & sarr(X) & " and "
                            Else
                                QR = QR & "'" & V1 & "'" & sarr(X) & " and "
                            End If
                        Next
                    End If
                    QR = Trim(QR)
                    QR = Left(QR, QR.Length - 3)

                    da.SelectCommand.CommandText = "select count(tid) from " & tbname & " where eid=" & eid & " and documenttype='" & arr1(0).ToString() & "' and TID=" & ddlobj & " and " & QR & " "
                End If

                ''Validation for other Type

                If rw.Item("Valtype").ToString.ToUpper = "OTHER" Then
                    Dim ddlobj As String
                    For Each rw1 As DataRow In Dtab.Rows
                        If Right(rw.Item("fldID"), rw.Item("fldID").ToString.Length - 3) = rw1.Item("fieldid").ToString Then
                            ddlobj = rw1.Item("Value").ToString
                        End If
                    Next
                    Dim txtbx1 As String
                    arr1 = rw.Item("value").ToString.Split("-")
                    Dim arr2 As String()
                    Dim CNT As Integer = arr1.Length
                    For i As Integer = 1 To arr1.Length - 1
                        arr2 = Split(arr1(i), ":")
                        If i = 1 Then
                            tbname = arr2(0).ToString
                        End If
                        If arr2.Length > 1 Then
                            sb = sb & arr2(1).ToString & ","
                        End If
                        If i > 1 Then
                            Dim opr As String = Left(arr2(0), 1)
                            Dim opr1 As String = Left(arr2(0), 2)
                            opr1 = opr1.Replace(opr, "")
                            If opr1 = "=" Then
                                opr = opr & opr1
                            End If

                            Dim txt As String = Right(arr2(0), arr2(0).Length - opr.Length).ToString
                            For Each rw1 As DataRow In Dtab.Rows
                                If Right(txt, txt.ToString.Length - 3) = rw1.Item("fieldid").ToString Then
                                    txtbx1 = rw1.Item("Value").ToString
                                End If
                            Next
                            Dim ddllst As DropDownList
                            da.SelectCommand.CommandText = "select datatype from mmm_mst_fields where eid=" & eid & " and fieldid=" & Right(txt, txt.ToString.Length - 3) & ""
                            If con.State <> ConnectionState.Open Then
                                con.Open()
                            End If
                            datatype = da.SelectCommand.ExecuteScalar()
                        End If
                    Next
                    sb = Left(sb, sb.Length - 1)

                    da.SelectCommand.CommandText = "select " & sb & " from " & tbname & " where tid=" & ddlobj.ToString & ""
                    Dim DS1 As New DataSet
                    da.Fill(DS1, "DATA")
                    Dim V1 As String = ""
                    Dim sarr() As String
                    Dim QR As String = " "
                    sarr = sts.ToString.Split("|")
                    If DS1.Tables("DATA").Rows.Count > 0 Then
                        For X As Integer = 0 To DS1.Tables("DATA").Columns.Count - 1
                            V1 = DS1.Tables("DATA").Rows(0).Item(X).ToString
                        Next
                    End If
                    da.SelectCommand.CommandText = "select  case when " & txtbx1.ToString & "  + " & V1 & ">=0 then 1 else 0 end "
                End If

                ''Validation For Duplicacy Check
                If rw.Item("Valtype").ToString.ToUpper = "DUPLICACYCHECK" Then
                    Dim ddlobj As String
                    For Each rw1 As DataRow In Dtab.Rows
                        If Right(rw.Item("fldID"), rw.Item("fldID").ToString.Length - 3) = rw1.Item("fieldid").ToString Then
                            ddlobj = rw1.Item("Value").ToString
                        End If
                    Next
                    arr1 = rw.Item("value").ToString.Split(":")
                    Dim arr2 As String()
                    Dim CNT As Integer = arr1.Length
                    Dim tbl As String = ""
                    Dim documenttype As String = ""
                    For ii As Integer = 0 To arr1.Length - 1
                        If ii = 0 Then
                            Dim arrr() As String = arr1(0).ToString.Split("-")
                            tbl = arrr(1).ToString
                            documenttype = arrr(0).ToString()
                        End If
                    Next
                    Dim xwhr As String = ""
                    For i As Integer = 1 To arr1.Length - 1
                        arr2 = Split(arr1(i), "-")
                        If arr2.Length > 1 Then
                            Dim opr As String = Left(arr2(1), 1)
                            Dim opr1 As String = Left(arr2(1), 2)
                            opr1 = opr1.Replace(opr, "")

                            If opr1 = "=" Then
                                opr = opr & opr1
                            End If

                            Dim txt As String = Right(arr2(1), arr2(1).Length - opr.Length).ToString
                            Dim fldid As String = Right(txt, txt.Length - 3)
                            'Dim row() As DataRow = dT.Select("fieldid=" & fldid & "")
                            Dim TXTBOX As String
                            For Each rw1 As DataRow In Dtab.Rows
                                If Right(rw.Item("fldID"), rw.Item("fldID").ToString.Length - 3) = rw1.Item("fieldid").ToString Then
                                    TXTBOX = rw1.Item("Value").ToString
                                End If
                            Next
                            'xwhr &= " AND " & arr3(0).ToString & arr3(1).ToString & TXTBOX.SelectedValue.ToString
                            xwhr &= " AND " & arr2(0).ToString & "" & opr & "'" & TXTBOX.ToString & "'"
                        End If
                    Next
                    If rw.Item("WF_STATUS").ToString.Length > 2 Then
                        Dim xwhrstatus As String = ""
                        Dim WFSTATUS() As String = rw.Item("WF_STATUS").ToString.Split(",")
                        If WFSTATUS.Length > 0 Then
                            For i As Integer = 0 To WFSTATUS.Length - 1
                                If i = 0 Then
                                    xwhrstatus = " and (curstatus='" & WFSTATUS(i).ToString & "'"

                                Else
                                    xwhrstatus &= " or curstatus='" & WFSTATUS(i).ToString & "' "
                                End If

                            Next
                            If xwhrstatus.Length > 5 Then
                                xwhrstatus &= ")"
                            End If
                        End If
                        xwhr &= xwhrstatus
                    End If
                    If Action = "UPDATE" Then
                        xwhr &= " AND TID<>" & tid & ""
                    End If

                    da.SelectCommand.CommandText = "SELECT count(*) FROM MMM_MST_DOC WHERE EID=" & eid & " and documenttype='" & documenttype & "' " & xwhr & ""
                End If


                If UCase(rw.Item("valtype").ToString) = "STATIC" Then
                    If datatype.ToString.ToUpper = "TEXT" Then
                        da.SelectCommand.CommandText = " select case when " & "'" & txtobj.ToString & "'" & rw.Item("operator") & "'" & rw.Item("value") & "'" & "  then 1 else 0 end"
                    Else
                        da.SelectCommand.CommandText = " select case when " & "'" & txtobj.ToString & "'" & rw.Item("operator") & rw.Item("value") & "  then 1 else 0 end"
                    End If
                ElseIf UCase(rw.Item("valtype").ToString) = "FIELD" Then
                    If datatype.ToUpper = "DATETIME" And datatype.ToUpper = "DATETIME" Then
                        da.SelectCommand.CommandText = " select case when " & " Convert(DateTime," & "'" & getdate(txtobj.ToString) & "'" & ")" & rw.Item("operator") & "Convert(DateTime," & "'" & getdate(txtobj1.ToString) & "'" & ")" & "  then 1 else 0 end"
                    ElseIf datatype.ToUpper = "NUMERIC" Then
                        da.SelectCommand.CommandText = " select case when " & "" & txtobj.ToString & "" & rw.Item("operator") & "" & txtobj1.ToString & "" & "  then 1 else 0 end"
                    Else
                        da.SelectCommand.CommandText = " select case when " & "'" & txtobj.ToString & "'" & rw.Item("operator") & "'" & txtobj1.ToString & "'" & "  then 1 else 0 end"
                    End If
                ElseIf UCase(rw.Item("valtype").ToString) = "MANDATORY" Then
                    da.SelectCommand.CommandText = " select case when " & "'" & txtobj.ToString & "'" & "=" & "'" & rw.Item("value").ToString & "'" & "  then 1 else 0 end"
                End If

                If con.State <> ConnectionState.Open Then
                    con.Open()
                End If

                Try
                    rtn = da.SelectCommand.ExecuteScalar()
                Catch ex As Exception
                    Return "check the entered data and ensure that all Mandatory fields have been filled with Valid data."
                End Try

                If rtn = 0 And rw.Item("valtype").ToString.ToUpper <> "DUPLICACYCHECK" Then
                    If UCase(rw.Item("valtype").ToString) <> "MANDATORY" Then
                        errmsg = Trim(errmsg) & " " & rw.Item("err_msg").ToString()
                    ElseIf UCase(rw.Item("valtype").ToString) = "OTHER" Then
                        errmsg = Trim(errmsg) & " " & rw.Item("err_msg").ToString()
                    End If
                ElseIf rtn = 1 And (rw.Item("Valtype").ToString.ToUpper = "STATIC" Or rw.Item("Valtype").ToString.ToUpper = "FIELD" Or rw.Item("Valtype").ToString.ToUpper = "MANDATORY") Then
                    If txtobj.ToString = "" Then
                        errmsg = Trim(errmsg) & " " & rw.Item("err_msg").ToString()
                    End If
                ElseIf rtn >= 1 And rw.Item("valtype").ToString.ToUpper = "DUPLICACYCHECK" Then
                    errmsg = Trim(errmsg) & " " & rw.Item("ERR_MSG").ToString
                End If
            Next
            con.Dispose()
            If errmsg.Length > 5 Then
                Return errmsg
            Else
                Return "True"
            End If
        Catch ex As Exception
            Return "check the entered data and ensure that all Mandatory fields have been filled with Valid data"
        Finally
            con.Close()
            con.Dispose()
            da.Dispose()
        End Try

    End Function

    Public Shared Function getdate(ByVal dbt As String) As DateTime
        Dim dtArr() As String
        dtArr = Split(dbt, "/")
        If dtArr.GetUpperBound(0) = 2 Then
            Dim dd, mm, yy As String
            dd = dtArr(0)
            mm = dtArr(1)
            yy = dtArr(2)
            Dim dt As Date
            Try
                dt = mm & "/" & dd & "/" & yy
                Return dt
            Catch ex As Exception
                Return Now.Date
            End Try
        Else
            Return Now.Date
        End If
    End Function

    Public Shared Function SaveDraft(EID As Integer, DocType As String, UID As Integer, Data As String, DOCID As Integer) As String
        Dim result As String = ""
        Dim con As SqlConnection = Nothing
        Dim da As SqlDataAdapter = Nothing
        Try

            Dim ds As New DataSet()
            Dim LStDataFinal As New List(Of DataWraper1)
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            con = New SqlConnection(conStr)
            da = New SqlDataAdapter("getDataOfForm100", con)
            da.SelectCommand.CommandType = CommandType.StoredProcedure
            da.SelectCommand.Parameters.AddWithValue("@EID", EID)
            da.SelectCommand.Parameters.AddWithValue("@FormName", DocType)
            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            da.Fill(ds)
            Dim onlyFiltered As DataView = ds.Tables(0).DefaultView
            onlyFiltered.RowFilter = "DocumentType='" & DocType & "' AND FieldType <> 'Formula Field' AND Isactive=1"
            onlyFiltered.Sort = "DisplayOrder"

            Dim theFlds As DataTable = onlyFiltered.Table.DefaultView.ToTable()
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
                            If arr(0).Trim().ToUpper() = DisplayName.ToUpper() Then
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
                                    objDW.FormType = theFlds.Rows(i).Item("FormType").ToString()
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
                'Setting Form Data into object
                objDW.DocumentType = DocType
                objDW.Data = StrFormData
                Dim UDataC As UserData
                Dim objF As DataWraper1
                Dim objC As DataWraper1
                Dim ObjItem As LineitemWrap
                Dim LStDataItem As List(Of UserData)
                Dim LstlineItem As List(Of LineitemWrap)
                For Each objData In LstData
                    If DocType = objData.DocumentType Then
                        objF = New DataWraper1()
                        LStDataItem = New List(Of UserData)
                        onlyFiltered.RowFilter = "DocumentType='" & DocType & "' AND FieldType <> 'Formula Field'  and IsActive=1"
                        onlyFiltered.Sort = "DisplayOrder"
                        theFlds = onlyFiltered.Table.DefaultView.ToTable()
                        'getting one row data
                        ObjItem = New LineitemWrap()
                        LstlineItem = New List(Of LineitemWrap)
                        ObjItem = CreateListCollection(objData.Data, theFlds, EID, DocType)
                        LstlineItem.Add(ObjItem)
                        'LstlineItem.Add(LStDataItem)
                        objF.LineItem = LstlineItem
                        objF.DocumentType = DocType
                        objF.FormType = objData.FormType
                        'objF.LineItem = LStDataItem
                        LStDataFinal.Add(objF)
                    Else
                        Dim arr As String() = Split(objData.Data, "{}")
                        UDataC = New UserData()
                        onlyFiltered.RowFilter = "DocumentType='" & DocType & "' AND FieldType <> 'Formula Field' AND  IsActive=1"
                        onlyFiltered.Sort = "DisplayOrder"
                        theFlds = onlyFiltered.Table.DefaultView.ToTable()
                        ObjItem = New LineitemWrap()
                        LstlineItem = New List(Of LineitemWrap)
                        objC = New DataWraper1()
                        For d As Integer = 0 To arr.Count - 1
                            objC.DocumentType = objData.DocumentType
                            objC.FormType = objData.FormType
                            If arr(d) <> "" Then
                                UDataC = New UserData()
                                ObjItem = CreateListCollection(arr(d), theFlds, EID, objData.DocumentType)
                                LstlineItem.Add(ObjItem)
                            End If
                        Next
                        objC.LineItem = LstlineItem
                        LStDataFinal.Add(objC)
                    End If
                Next

                'Fill Drop Down And lookup values
                FillData(LStDataFinal, ds, EID, DocType)
                'Validate Form
                Dim FormData As New DataWraper1()
                Dim lineitem As LineitemWrap
                'Navigating all the form
                Dim ErrMsg = "", AllError = ""
                Dim IsAllFormValid As Boolean = True
                Dim Flag As Boolean = True
                For Each Form In LStDataFinal
                    FormData = Form
                    Dim DocumentType = FormData.DocumentType
                    Dim LstLine = FormData.LineItem
                    For Each Row In LstLine
                        ErrMsg = "Error(s) in document " & DocumentType & ". "
                        lineitem = New LineitemWrap()
                        lineitem = Row
                        '' COMMented below on 22-feb for ocr (used draft for OCR file reading) by mg
                        ' Flag = ValidateForm(EID, lineitem, Form.FormType, DocumentType, ErrMsg)
                        'Code for form validation
                        'Fire this code only when field pass all field level validation
                        IsAllFormValid = IsAllFormValid And Flag
                        If Flag = False Then
                            If AllError = "" Then
                                AllError = ErrMsg
                            Else
                                AllError = AllError & "." & ErrMsg
                            End If
                        End If
                    Next
                Next
                ' IF form is validated go and save it 
                'Dim MainForm = From Form In LStDataFinal Where Form.DocumentType = DocType Select Form
                If IsAllFormValid = True Then
                    result = DraftData(EID, DocType, LStDataFinal, UID)
                Else
                    result = AllError
                End If
            Else
                result = "Invalid DocType."
            End If
        Catch ex As Exception
            ErrorLog.sendMail("CommanUtil.SaveDraft", ex.Message)
            Return "RTO" & Regex.Replace(ex.InnerException.Message.ToString, "[""']", String.Empty)
        Finally
            If Not con Is Nothing Then
                con.Close()
                con.Dispose()
            End If
            If Not da Is Nothing Then
                da.Dispose()
            End If
        End Try
        Return result
    End Function

    Public Shared Function DraftData(EID As Integer, DocType As String, LStDataFinal As List(Of DataWraper1), UID As Integer) As String
        Dim strResult = ""
        Dim strColumn As String = "", strValue As String = ""
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

                    strColumn = "INSERT INTO MMM_MST_DOC_Draft(EID,Documenttype,oUID,adate,Source"
                    strValue = "VALUES (" & EID & ",'" & line.DocumentType & "'," & UID & ",getdate(),'WS'"
                    'Generate query 
                    For Each obj In line.LineItem
                        StrQuery = GenerateQuery(strColumn, strValue, obj)
                    Next
                    StrQuery = StrQuery & ";Select @@identity"
                    con = New SqlConnection(conStr)
                    da = New SqlDataAdapter(StrQuery, con)
                    If con.State <> ConnectionState.Open Then
                        con.Open()
                    End If
                    FileID = da.SelectCommand.ExecuteScalar()
                    'Code For saving Auto Number
                    Dim FormulaQ = "select * FROM MMM_MST_FIELDS WHERE EID=" & EID & " AND DocumentType='" & line.DocumentType & "' AND FieldType='Formula Field' order by displayOrder"
                    Dim DSF As New DataSet()
                    da = New SqlDataAdapter(FormulaQ, con)
                    da.Fill(DSF)
                    'Ececuting Formula Fields
                    Try
                        Dim viewdoc As String = line.DocumentType
                        viewdoc = viewdoc.Replace(" ", "_")
                        If DSF.Tables(0).Rows.Count > 0 Then
                            For f As Integer = 0 To DSF.Tables(0).Rows.Count - 1
                                Dim formulaeditorr As New formulaEditor
                                Dim forvalue As String = String.Empty
                                forvalue = formulaeditorr.ExecuteFormula(DSF.Tables(0).Rows(f).Item("KC_LOGIC"), FileID, "dv" + EID.ToString + viewdoc, EID, 1)
                                Dim upquery As String = "update " & "MMM_MST_DOC_DRAFT" & "  set  " & DSF.Tables(0).Rows(f).Item("FieldMapping").ToString & "='" & forvalue.ToString & "'  where tid =" & FileID & ""
                                da.SelectCommand.CommandText = upquery
                                da.SelectCommand.CommandType = CommandType.Text
                                da.SelectCommand.ExecuteNonQuery()
                            Next
                        End If
                    Catch ex As Exception
                    End Try
                Next
            End If
            'Getting Child Item Generating Query
            Dim ChildForm = From Form In LStDataFinal Where Form.DocumentType <> DocType Select Form
            StrQuery = ""
            Dim StrQry1 = ""
            If FileID > 0 Then
                For Each line In ChildForm
                    strColumn = "INSERT INTO MMM_MST_DOC_Item_draft(DOCID,documenttype,isauth"
                    strValue = "VALUES (" & FileID & ",'" & line.DocumentType & "'," & "1"
                    StrQuery = ""
                    For Each obj In line.LineItem
                        StrQry1 = GenerateQuery(strColumn, strValue, obj)
                        da = New SqlDataAdapter(StrQuery, con)
                        If con.State <> ConnectionState.Open Then
                            con.Open()
                        End If
                        Try
                            Dim FileID1 = da.SelectCommand.ExecuteNonQuery()
                            Dim FormulaQ = "select * FROM MMM_MST_FIELDS WHERE EID=" & EID & " AND DocumentType='" & line.DocumentType & "' AND FieldType='Formula Field' order by displayOrder"
                            Dim DSF As New DataSet()
                            da = New SqlDataAdapter(FormulaQ, con)
                            da.Fill(DSF)
                            Dim viewdoc As String = line.DocumentType
                            viewdoc = viewdoc.Replace(" ", "_")
                            If DSF.Tables(0).Rows.Count > 0 Then
                                For f As Integer = 0 To DSF.Tables(0).Rows.Count - 1
                                    Dim formulaeditorr As New formulaEditor
                                    Dim forvalue As String = String.Empty
                                    forvalue = formulaeditorr.ExecuteFormula(DSF.Tables(0).Rows(f).Item("KC_LOGIC"), FileID, "dv" + EID.ToString + viewdoc, EID, 1)
                                    Dim upquery As String = "update " & "MMM_MST_DOC_Item_draft" & "  set  " & DSF.Tables(0).Rows(f).Item("FieldMapping").ToString & "='" & forvalue.ToString & "'  where tid =" & FileID & ""
                                    da.SelectCommand.CommandText = upquery
                                    da.SelectCommand.CommandType = CommandType.Text
                                    da.SelectCommand.ExecuteNonQuery()
                                Next
                            End If
                        Catch ex As Exception
                        End Try
                    Next
                Next
                strResult = "Your DocID is " + FileID.ToString()
                Dim pdfName = ""
                Dim OBJDMS As New DMSUtil()
                pdfName = OBJDMS.GetDraftPDF(EID, FileID)
                strResult = strResult & "~www.myndsaas.com" & pdfName
            End If
        Catch ex As Exception
            ErrorLog.sendMail("CommanUtill.DraftData", ex.Message)
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

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub

    Public Shared Function SetHistory(EID As Integer, FileID As Integer, DocDBTableName As String, UID As Integer, UAction As String, Doctype As String) As String
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim ret As String = ""
        Dim Var1 = 0
        Try
            Using con As New SqlConnection(conStr)
                Using da As New SqlDataAdapter("InsertHistory", con)
                    da.SelectCommand.CommandType = CommandType.StoredProcedure
                    da.SelectCommand.Parameters.AddWithValue("@EID", EID)
                    da.SelectCommand.Parameters.AddWithValue("@DOCID", FileID)
                    da.SelectCommand.Parameters.AddWithValue("@TableName", DocDBTableName)
                    da.SelectCommand.Parameters.AddWithValue("@UID", UID)
                    da.SelectCommand.Parameters.AddWithValue("@UAction", UAction)
                    da.SelectCommand.Parameters.AddWithValue("@DocumentType", Doctype)
                    con.Open()
                    Var1 = da.SelectCommand.ExecuteNonQuery()
                End Using
            End Using
        Catch ex As Exception
            Return "fail"
        End Try
        If Var1 > 0 Then
            ret = "success"
        Else
            ret = "fail"
        End If
        Return ret
    End Function

End Class

Public Class DataWraper
    Public Property FormType As String
    Public Property DocumentType As String
    Public Property Data As String
End Class

Public Class DataWraper1
    Public Property DocumentType As String
    Public Property FormType As String
    Public Property LineItem As List(Of LineitemWrap)
End Class

Public Class UserData
    Public Property FieldID As String
    Public Property FieldType As String
    Public Property MinVal As String
    Public Property MaxVal As String
    Public Property IsRequired As String
    Public Property DisplayName As String
    Public Property FieldMapping As String
    Public Property Values As String
    Public Property DataType As String
    Public Property DropDownType As String
    Public Property CalText As String
    Public Property LookUp As String
    Public Property DropDown As String
    Public Property DDLValue As String
    Public Property IsUnique As String
    Public Property AutoFilter As String
    Public Property DisplayOrder As Integer
    Public Property EnableEdit As String
    Public Property IsEditOnamend As String
    Public Property DocumentType As String
    Public Property HasSpecificRule As Boolean
    Public Property AllowCreatedRecord As Boolean
    Public Property OnflyFieldmapping As String
    Public Property AllowDecimalDigit As Integer

    Public Property ddlval As String
    Public Property InVisible As Integer
    Public Property isEditable As Integer

    Public Property isBlankDate As Boolean
    'DisplayOrder EnableEdit
End Class

Public Class LineitemWrap
    Public Property DataItem As List(Of UserData)
End Class

Public Class ErrorLog

    Public Shared Function sendMail(Source As String, Message As String) As String
        Dim ret As String = ""
        Dim mBody As String = "Hi Support Team (Live Server), <br/><br/> <b>Error Source : </b>" & Source & " <br/> <b>Error Message is </b><br/>" & Message & " <br/>"
        mBody = mBody & "<br/><br/>Error in web service "
        Dim Email As New System.Net.Mail.MailMessage("no-reply@myndsol.com", "swhelpdesk@myndsol.com", "Error In Web Service", mBody)
        Dim mailClient As New System.Net.Mail.SmtpClient()
        ' Email.Bcc.Add("ajeet.kumar@myndsol.com")
        Email.IsBodyHtml = True
        Dim basicAuthenticationInfo As New System.Net.NetworkCredential("no-reply@myndsol.com", "$up90rt#534")
        mailClient.Host = "mail.myndsol.com"
        mailClient.UseDefaultCredentials = False
        mailClient.Credentials = basicAuthenticationInfo
        Try
            mailClient.Send(Email)
        Catch ex As Exception
        End Try

        Return ret
    End Function
End Class

