' NOTE: You can use the "Rename" command on the context menu to change the class name "BPMMobile" in code, svc and config file together.
' NOTE: In order to launch WCF Test Client for testing this service, please select BPMMobile.svc or BPMMobile.svc.vb at the Solution Explorer and start debugging.
Imports System.Data
Imports System.Data.SqlClient
Imports System.Net
Imports System.IO
Imports System.Xml.Serialization
Imports System.Runtime.Serialization
Imports System.ServiceModel
Imports System.ServiceModel.Web
Imports System.Xml
Imports System.Web.Hosting

Public Class BPMMobile
    Implements IBPMMobile

    Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString

    Dim BPMKEY As String = ConfigurationManager.AppSettings("BPMKEY").ToString()


    Function logout(UID As String) As String Implements IBPMMobile.logout
        Dim ret As String = "Fail"
        Try
            Dim objDC As New DataClass()
            objDC.ExecuteNonQuery(" update mmm_mst_user set islogin=0 where uid=" & UID)
            ret = "PASS"
        Catch ex As Exception

        End Try
        Return ret
    End Function

    Function GetActiveAudiCycle(ByVal FieldID As String, ByVal ECode As String) As String Implements IBPMMobile.GetActiveAudiCycle
        Dim result As String = ""
        Dim objDC As New DataClass
        result = objDC.ExecuteQryScaller("Select top 1 convert(nvarchar(20),m.tid) + '||' + m.fld1 From mmm_mst_master M with (nolock) inner join mmm_mst_entity E on m.eid=e.eid where e.code='" & ECode & "' and M.documenttype='audit cycle' and m.isauth=1 order by m.tid desc ")
        'Select top 1 convert(nvarchar(20),m.tid) + '||' + m.fld1 From mmm_mst_master M with (nolock) inner join mmm_mst_entity E on m.eid=e.eid where e.code='HRPL' and M.documenttype='audit cycle' and m.isauth=1 order by m.tid desc
        'ret = "1943125||Test-1"
        Return result
    End Function

    Function EmpNomCheck(ByVal userid As String, ByVal cid As String) As String Implements IBPMMobile.EmpNomCheck
        Dim result As String = ""
        Dim objDC As New DataClass
        'result = objDC.ExecuteQryScaller("Select 'DOCID::'+ convert(varchar(20),tid) + '||' + 'Mobile_No::' + fld1 + '||' + 'Name_of_Nominee::'+fld2  + '||' + 'Permanent_Address1::'+fld4 + '||' + 'Permanent_Address2::'+fld5 + '||' + 'Permanent_City::'+fld6 + '||' + 'Permanent_State::'+fld7 + '||' + 'Permanent_Pin::'+fld8 + '||' +  'Present_Address1::'+fld9 + '||' + 'Present_Address2::'+fld10 + '||' +  'Present_City::'+fld11 + '||' + 'Relationship_with_Nominee::'+fld12 + '||' +  'Present_State::'+fld13 + '||' + 'Present_Pin::'+fld14 + '||' +  'Current_UserID::'+fld15 + '||' +  'CID::'+fld16 From mmm_mst_master with (nolock) where eid=120 and documenttype='Nomination' and fld16='" & cid.Trim & "' and fld15='" & userid.Trim & "' and isauth=1")
        'Select count(*) From mmm_mst_master with (nolock)  where eid=120 and documenttype='Nomination' and fld16='g4s' and fld15='999' and isauth=1
        Dim ht As New Hashtable()
        ht.Add("@cid", cid)
        ht.Add("@userid", userid)
        result = objDC.ExecuteProScaller("EmpNomCheck_PHR_120", ht)
        If Not String.IsNullOrEmpty(result) Then
            Return result '"Exist"
        Else
            Return "Not Exist"
        End If
    End Function

    Function AuthanticateUser(UserID As String, Password As String, ECode As String) As Userdetails Implements IBPMMobile.AuthanticateUser
        Dim ret As String = "Fail"
        Dim uObj As New User()
        Dim ds As New DataSet
        Dim objU As New Userdetails()
        Dim enitity As Integer = 0
        Try
            objU = CommanUtil.AuthanticateUser(UserID, Password, ECode)
        Catch ex As Exception
        Finally
        End Try
        Return objU
    End Function

    Function GetUserMenu(Key As String, EID As String, UserRole As String) As List(Of UserMenu) Implements IBPMMobile.GetUserMenu
        Dim lstMenu As New List(Of UserMenu)
        Dim dsMenu As New DataSet()
        Dim objM As UserMenu

        Try
            Dim BPMKEY As String = ConfigurationManager.AppSettings("BPMKEY").ToString()
            If BPMKEY = Key Then
                UserRole = "{" & UserRole
                dsMenu = GetMenu(EID:=EID, userrole:=UserRole)
                Dim FormName As String = ""
                If dsMenu.Tables(0).Rows.Count > 0 Then
                    For i As Integer = 0 To dsMenu.Tables(0).Rows.Count - 1
                        objM = New UserMenu()
                        objM.MenuName = dsMenu.Tables(0).Rows(i).Item("MenuName")
                        objM.MID = dsMenu.Tables(0).Rows(i).Item("MID")
                        FormName = dsMenu.Tables(0).Rows(i).Item("PageLink")

                        'Dim arStr As New String()=dsMenu.Tables(0).Rows(i).Item("PageLink").ToString().Split('=')
                        If dsMenu.Tables(0).Rows(i).Item("PageLink").ToString().Contains("=") Then
                            Dim arStr As String() = dsMenu.Tables(0).Rows(i).Item("PageLink").ToString().Split("=")
                            FormName = arStr(1)
                        End If
                        objM.FormName = FormName
                        objM.ImageName = dsMenu.Tables(0).Rows(i).Item("Image")
                        objM.PMenu = dsMenu.Tables(0).Rows(i).Item("Pmenu")
                        lstMenu.Add(objM)
                    Next
                End If
            End If
        Catch ex As Exception

        End Try
        Return lstMenu
    End Function


    Function GetDataOFAllForm(Key As String, EID As String, URole As String) As List(Of FormData) Implements IBPMMobile.GetDataOFAllForm
        Dim lstFlds As New List(Of FormData)
        Dim dsfld As New DataSet()
        Dim objFD As FormData
        Dim con As SqlConnection = Nothing
        Dim da As SqlDataAdapter = Nothing
        Try
            'Dim BPMKEY As String = ConfigurationManager.AppSettings("BPMKEY").ToString()

            Dim dsMenu As New DataSet()
            URole = "{" & URole
            dsMenu = GetMenu(EID:=EID, userrole:=URole)
            Dim FormName As New StringBuilder()
            For m As Integer = 0 To dsMenu.Tables(0).Rows.Count - 1
                If dsMenu.Tables(0).Rows(m).Item("PageLink").ToString().Contains("=") Then
                    Dim arStr As String() = dsMenu.Tables(0).Rows(m).Item("PageLink").ToString().Split("=")
                    If arStr.Length = 2 Then
                        If FormName.ToString.Trim() = "" Then
                            FormName.Append("'" & arStr(1) & "'")
                        Else
            FormName.Append("," & "'" & arStr(1) & "'")
                        End If
                    End If
                End If
            Next
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            con = New SqlConnection(conStr)
            Dim dsChild As New DataSet()
            Dim Query = "SELECT F.*  FROM MMM_MST_FORMS F   where F.FormSource ='DETAIL FORM ' or F.FormSource = 'ACTION DRIVEN' and   F.EID=" & EID
            da = New SqlDataAdapter(Query, con)
            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            da.Fill(dsChild)
            If dsChild.Tables(0).Rows.Count Then
                For M As Integer = 0 To dsChild.Tables(0).Rows.Count - 1
                    If FormName.ToString.Trim() = "" Then
                        FormName.Append("'" & dsChild.Tables(0).Rows(M).Item("FormName") & "'")
                    Else
                        FormName.Append("," & "'" & dsChild.Tables(0).Rows(M).Item("FormName") & "'")
                    End If
                Next
            End If
            If BPMKEY = Key Then
                Query = "SELECT FF.*,F.*  FROM MMM_MST_FIELDS FF left outer JOIN MMM_MST_FORMS F on F.FormName = FF.DocumentType and F.EID = FF.EID   where FF.isactive=1 and F.EID=" & EID & "AND FF.DocumentType in (" & FormName.ToString().Trim() & ")"
                da = New SqlDataAdapter(Query, con)
                If con.State <> ConnectionState.Open Then
                    con.Open()
                End If
                da.Fill(dsfld)
                If dsfld.Tables(0).Rows.Count > 0 Then
                    For i As Integer = 0 To dsfld.Tables(0).Rows.Count - 1
                        objFD = New FormData()
                        objFD.FieldID = dsfld.Tables(0).Rows(i).Item("FieldID").ToString()
                        objFD.EID = dsfld.Tables(0).Rows(i).Item("EID").ToString()
                        objFD.displayName = dsfld.Tables(0).Rows(i).Item("displayName").ToString()
                        objFD.FieldType = dsfld.Tables(0).Rows(i).Item("FieldType").ToString()
                        objFD.displayOrder = dsfld.Tables(0).Rows(i).Item("displayOrder").ToString()
                        objFD.FieldMapping = dsfld.Tables(0).Rows(i).Item("FieldMapping").ToString()
                        objFD.isRequired = dsfld.Tables(0).Rows(i).Item("isRequired").ToString()
                        objFD.isActive = dsfld.Tables(0).Rows(i).Item("isActive").ToString()
                        objFD.dropdown = dsfld.Tables(0).Rows(i).Item("dropdown").ToString()
                        objFD.DocumentType = dsfld.Tables(0).Rows(i).Item("DocumentType").ToString()
                        objFD.DBTableName = dsfld.Tables(0).Rows(i).Item("DBTableName").ToString()
                        objFD.FieldNature = dsfld.Tables(0).Rows(i).Item("FieldNature").ToString()
                        objFD.DropDownType = dsfld.Tables(0).Rows(i).Item("DropDownType").ToString()
                        objFD.isEditable = dsfld.Tables(0).Rows(i).Item("isEditable").ToString()
                        objFD.datatype = dsfld.Tables(0).Rows(i).Item("datatype").ToString()
                        objFD.MinLen = dsfld.Tables(0).Rows(i).Item("MinLen").ToString()
                        objFD.MaxLen = dsfld.Tables(0).Rows(i).Item("MaxLen").ToString()
                        objFD.isWorkFlow = dsfld.Tables(0).Rows(i).Item("isWorkFlow").ToString()
                        objFD.lookupvalue = dsfld.Tables(0).Rows(i).Item("lookupvalue").ToString()
                        objFD.Cal_Fields = dsfld.Tables(0).Rows(i).Item("Cal_Fields").ToString()
                        objFD.AutoFilter = dsfld.Tables(0).Rows(i).Item("AutoFilter").ToString()
                        objFD.KC_VALUE = dsfld.Tables(0).Rows(i).Item("KC_VALUE").ToString()
                        objFD.KC_LOGIC = dsfld.Tables(0).Rows(i).Item("KC_LOGIC").ToString()
                        objFD.isunique = dsfld.Tables(0).Rows(i).Item("isunique").ToString()
                        objFD.ShowOnDocDetail = dsfld.Tables(0).Rows(i).Item("ShowOnDocDetail").ToString()
                        objFD.IsSession = dsfld.Tables(0).Rows(i).Item("IsSession").ToString()
                        objFD.isMail = dsfld.Tables(0).Rows(i).Item("isMail").ToString()
                        objFD.KC_STATUS = dsfld.Tables(0).Rows(i).Item("KC_STATUS").ToString()
                        objFD.ShowOnAmendment = dsfld.Tables(0).Rows(i).Item("ShowOnAmendment").ToString()
                        objFD.isSearch = dsfld.Tables(0).Rows(i).Item("isSearch").ToString()
                        objFD.isImieno = dsfld.Tables(0).Rows(i).Item("isImieno").ToString()
                        objFD.isPhoneNo = dsfld.Tables(0).Rows(i).Item("isPhoneNo").ToString()
                        objFD.iseditonAmend = dsfld.Tables(0).Rows(i).Item("iseditonAmend").ToString()
                        objFD.InLineEditing = dsfld.Tables(0).Rows(i).Item("InLineEditing").ToString()
                        objFD.dependentON = dsfld.Tables(0).Rows(i).Item("dependentON").ToString()
                        objFD.Invisible = dsfld.Tables(0).Rows(i).Item("Invisible").ToString()
                        objFD.Cal_Text = dsfld.Tables(0).Rows(i).Item("cal_text").ToString()
                        objFD.DefaultValue = dsfld.Tables(0).Rows(i).Item("defaultfieldval").ToString()
                        'DefaultValue
                        'Code for wrapping all the drop down list value into the single container
                        If (objFD.FieldType.ToString() = "Drop Down") And (Not objFD.dropdown.ToString() Is Nothing) And (objFD.DropDownType.ToString()) = "MASTER VALUED" And (objFD.AutoFilter.Trim() = "" Or objFD.AutoFilter Is Nothing) Then
                            'GetDropDownData(EID As Integer, Doctype As String, FieldName As String, TableName As String)
                            Dim tableName = "", FieldName = "", DocType = ""
                            Dim arrFile As String() = objFD.dropdown.ToString().Split("-")
                            Try
                                If arrFile.Count = 3 Then
                                    If arrFile(0).ToUpper() = "MASTER" Then
                                        tableName = "MMM_MST_MASTER"
                                    ElseIf arrFile(0).ToUpper() = "DOCUMENT" Then
                                        tableName = "MMM_MST_DOC"
                                    ElseIf arrFile(0).ToString.ToUpper = "USER" Then
                                        tableName = "MMM_MST_USER"
                                    Else
                                        tableName = "MMM_MST_DOC_ITEM"
                                    End If
                                    DocType = arrFile(1).ToString()
                                    FieldName = arrFile(2).ToString()
                                    If tableName <> "" And DocType <> "" And FieldName <> "" Then
                                        Dim dsData As New DataSet()
                                        dsData = CommanUtil.GetDropDownData(EID, DocType, FieldName, tableName)
                                        If dsData.Tables(0).Rows.Count > 0 Then
                                            Dim lstData As New List(Of Data)
                                            Dim StrData = ""
                                            For k As Integer = 0 To dsData.Tables(0).Rows.Count - 1
                                                'objData = New Data()
                                                'objData.ValueField = dsData.Tables(0).Rows(k).Item(0)
                                                'objData.TextField = dsData.Tables(0).Rows(k).Item(1)
                                                'lstData.Add(objData)
                                                If StrData = "" Then
                                                    StrData = dsData.Tables(0).Rows(k).Item(0).ToString() & " : " & dsData.Tables(0).Rows(k).Item(1).ToString()
                                                Else
                                                    StrData = StrData & " $ " & dsData.Tables(0).Rows(k).Item(0) & " : " & dsData.Tables(0).Rows(k).Item(1)
                                                End If
                                            Next
                                            StrData = "$" & StrData
                                            objFD.dropdown = StrData
                                            'objFD.DropDownData = lstData
                                        Else
                                            objFD.dropdown = ""
                                        End If
                                    End If
                                End If
                            Catch ex As Exception

                            End Try

                        End If
                        lstFlds.Add(objFD)
                    Next
                End If
            End If
        Catch ex As Exception
        Finally
            con.Close()
            con.Dispose()
            da.Dispose()
        End Try
        'Dim str = WebOperationContext.Current.OutgoingResponse.ToString()
        Return lstFlds
    End Function

    Function GetDataOFForm(Key As String, EID As String, FormName As String) As List(Of FormData) Implements IBPMMobile.GetDataOFForm
        Dim lstFlds As New List(Of FormData)
        Dim dsfld As New DataSet()
        Dim objFD As FormData
        Dim con As SqlConnection = Nothing
        Dim da As SqlDataAdapter = Nothing
        Try
            'Dim BPMKEY As String = ConfigurationManager.AppSettings("BPMKEY").ToString()
            If BPMKEY = Key Then
                Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
                con = New SqlConnection(conStr)
                da = New SqlDataAdapter("getDataOfForm", con)
                da.SelectCommand.CommandType = CommandType.StoredProcedure
                da.SelectCommand.Parameters.AddWithValue("@EID", EID)
                da.SelectCommand.Parameters.AddWithValue("@FormName", FormName)
                If con.State <> ConnectionState.Open Then
                    con.Open()
                End If
                da.Fill(dsfld)
                If dsfld.Tables(0).Rows.Count > 0 Then
                    For i As Integer = 0 To dsfld.Tables(0).Rows.Count - 1
                        objFD = New FormData()
                        objFD.FieldID = dsfld.Tables(0).Rows(i).Item("FieldID").ToString()
                        objFD.EID = dsfld.Tables(0).Rows(i).Item("EID").ToString()
                        objFD.displayName = dsfld.Tables(0).Rows(i).Item("displayName").ToString()
                        objFD.FieldType = dsfld.Tables(0).Rows(i).Item("FieldType").ToString()
                        objFD.displayOrder = dsfld.Tables(0).Rows(i).Item("displayOrder").ToString()
                        objFD.FieldMapping = dsfld.Tables(0).Rows(i).Item("FieldMapping").ToString()
                        objFD.isRequired = dsfld.Tables(0).Rows(i).Item("isRequired").ToString()
                        objFD.isActive = dsfld.Tables(0).Rows(i).Item("isActive").ToString()
                        objFD.dropdown = dsfld.Tables(0).Rows(i).Item("dropdown").ToString()
                        objFD.DocumentType = dsfld.Tables(0).Rows(i).Item("DocumentType").ToString()
                        objFD.DBTableName = dsfld.Tables(0).Rows(i).Item("DBTableName").ToString()
                        objFD.FieldNature = dsfld.Tables(0).Rows(i).Item("FieldNature").ToString()
                        objFD.DropDownType = dsfld.Tables(0).Rows(i).Item("DropDownType").ToString()
                        objFD.isEditable = dsfld.Tables(0).Rows(i).Item("isEditable").ToString()
                        objFD.datatype = dsfld.Tables(0).Rows(i).Item("datatype").ToString()
                        objFD.MinLen = dsfld.Tables(0).Rows(i).Item("MinLen").ToString()
                        objFD.MaxLen = dsfld.Tables(0).Rows(i).Item("MaxLen").ToString()
                        objFD.isWorkFlow = dsfld.Tables(0).Rows(i).Item("isWorkFlow").ToString()
                        objFD.lookupvalue = dsfld.Tables(0).Rows(i).Item("lookupvalue").ToString()
                        objFD.Cal_Fields = dsfld.Tables(0).Rows(i).Item("Cal_Fields").ToString()
                        objFD.AutoFilter = dsfld.Tables(0).Rows(i).Item("AutoFilter").ToString()
                        objFD.KC_VALUE = dsfld.Tables(0).Rows(i).Item("KC_VALUE").ToString()
                        objFD.KC_LOGIC = dsfld.Tables(0).Rows(i).Item("KC_LOGIC").ToString()
                        objFD.isunique = dsfld.Tables(0).Rows(i).Item("isunique").ToString()
                        objFD.ShowOnDocDetail = dsfld.Tables(0).Rows(i).Item("ShowOnDocDetail").ToString()
                        objFD.IsSession = dsfld.Tables(0).Rows(i).Item("IsSession").ToString()
                        objFD.isMail = dsfld.Tables(0).Rows(i).Item("isMail").ToString()
                        objFD.KC_STATUS = dsfld.Tables(0).Rows(i).Item("KC_STATUS").ToString()
                        objFD.ShowOnAmendment = dsfld.Tables(0).Rows(i).Item("ShowOnAmendment").ToString()
                        objFD.isSearch = dsfld.Tables(0).Rows(i).Item("isSearch").ToString()
                        objFD.isImieno = dsfld.Tables(0).Rows(i).Item("isImieno").ToString()
                        objFD.isPhoneNo = dsfld.Tables(0).Rows(i).Item("isPhoneNo").ToString()
                        objFD.iseditonAmend = dsfld.Tables(0).Rows(i).Item("iseditonAmend").ToString()
                        objFD.InLineEditing = dsfld.Tables(0).Rows(i).Item("InLineEditing").ToString()
                        objFD.dependentON = dsfld.Tables(0).Rows(i).Item("dependentON").ToString()
                        objFD.Invisible = dsfld.Tables(0).Rows(i).Item("Invisible").ToString()
                        objFD.Cal_Text = dsfld.Tables(0).Rows(i).Item("cal_text").ToString()
                        'Code for wrapping all the drop down list value into the single container
                        If (objFD.FieldType.ToString() = "Drop Down") And (Not objFD.dropdown.ToString() Is Nothing) And (objFD.DropDownType.ToString()) = "MASTER VALUED" And (objFD.AutoFilter.Trim() = "" Or objFD.AutoFilter Is Nothing) Then
                            'GetDropDownData(EID As Integer, Doctype As String, FieldName As String, TableName As String)
                            Dim tableName = "", FieldName = "", DocType = ""
                            Dim arrFile As String() = objFD.dropdown.ToString().Split("-")
                            Try
                                If arrFile.Count = 3 Then
                                    If arrFile(0).ToUpper() = "MASTER" Then
                                        tableName = "MMM_MST_MASTER"
                                    ElseIf arrFile(0).ToUpper() = "DOCUMENT" Then
                                        tableName = "MMM_MST_DOC"
                                    ElseIf arrFile(0).ToString.ToUpper = "USER" Then
                                        tableName = "MMM_MST_USER"
                                    Else
                                        tableName = "MMM_MST_DOC_ITEM"
                                    End If
                                    DocType = arrFile(1).ToString()
                                    FieldName = arrFile(2).ToString()
                                    If tableName <> "" And DocType <> "" And FieldName <> "" Then
                                        Dim dsData As New DataSet()
                                        dsData = CommanUtil.GetDropDownData(EID, DocType, FieldName, tableName)
                                        If dsData.Tables(0).Rows.Count > 0 Then
                                            Dim lstData As New List(Of Data)
                                            Dim StrData = ""
                                            For k As Integer = 0 To dsData.Tables(0).Rows.Count - 1
                                                'objData = New Data()
                                                'objData.ValueField = dsData.Tables(0).Rows(k).Item(0)
                                                'objData.TextField = dsData.Tables(0).Rows(k).Item(1)
                                                'lstData.Add(objData)
                                                If StrData = "" Then
                                                    StrData = dsData.Tables(0).Rows(k).Item(0).ToString() & " : " & dsData.Tables(0).Rows(k).Item(1).ToString()
                                                Else
                                                    StrData = StrData & " $ " & dsData.Tables(0).Rows(k).Item(0) & " : " & dsData.Tables(0).Rows(k).Item(1)
                                                End If
                                            Next
                                            StrData = "$" & StrData
                                            objFD.dropdown = StrData
                                            'objFD.DropDownData = lstData
                                        End If
                                    End If
                                End If
                            Catch ex As Exception

                            End Try

                        End If
                        lstFlds.Add(objFD)
                    Next
                End If
            End If
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
        'Dim str = WebOperationContext.Current.OutgoingResponse.ToString()
        Return lstFlds
    End Function

    Public Function GetMenu(ByVal EID As Integer, userrole As String) As DataSet
        Dim dsMenu As New DataSet
        Dim con As SqlConnection = Nothing
        Dim da As SqlDataAdapter = Nothing
        Try
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            con = New SqlConnection(conStr)
            da = New SqlDataAdapter("getMenuMob", con)
            da.SelectCommand.CommandType = CommandType.StoredProcedure
            da.SelectCommand.Parameters.AddWithValue("@EID", EID)
            da.SelectCommand.Parameters.AddWithValue("@userRole", userrole)
            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            da.Fill(dsMenu)
            Return dsMenu
        Catch ex As Exception
            Throw
        Finally
            con.Close()
            con.Dispose()
            da.Dispose()
        End Try

    End Function

    Function PostTest(Data As Stream) As String Implements IBPMMobile.PostTest
        Dim reader As New StreamReader(Data)
        Dim returnValue As String = reader.ReadToEnd()
        Return returnValue
    End Function


    Function ImportMaster(UserID As String, Password As String, ECode As String, DocType As String, Data As String) As String Implements IBPMMobile.ImportMaster
        Dim Result As String = "fail"
        Dim objU As New Userdetails()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim UID As Integer = 0
        Try
            'Validating user
            objU = CommanUtil.AuthanticateUser(UserID, Password, ECode)
            UID = objU.UID
            If UID > 0 Then
                'input parameter check
                If (Not (DocType Is Nothing)) And DocType.Trim() <> "" And (Not (Data Is Nothing)) And Data.Trim() <> "" Then
                    DocType = DocType.Trim().Replace("+", "")
                    Dim ds As New DataSet()
                    ds = CommanUtil.GetFormFields(objU.EID, DocType)
                    If ds.Tables("fields").Rows.Count > 0 Then
                        Dim onlyFiltered As DataView = ds.Tables("fields").DefaultView
                        onlyFiltered.RowFilter = "FieldType<>'" & "Formula field" & "' AND FieldType <> '" & "Calculative Field" & "'"
                        Dim theFlds As DataTable = onlyFiltered.Table.DefaultView.ToTable()
                        Dim arrData As String() = Data.Split("|")
                        'checking all tha mandatery fields
                        If arrData.Count = theFlds.Rows.Count Then
                            'For Each column As DataColumn In theFlds.Columns
                            'Next
                            Dim DisplayName As String = ""
                            Dim DisplayNameS As String = ""
                            Dim IsRequired As String = ""
                            Dim IsValidData As Boolean = True
                            Dim IsParameterSupplyed As Boolean = False
                            For i As Integer = 0 To theFlds.Rows.Count - 1
                                DisplayName = theFlds.Rows(i).Item("displayName")
                                IsRequired = theFlds.Rows(i).Item("IsRequired")
                                'Checking the concern field in array
                                For j As Integer = 0 To arrData.Count - 1
                                    Dim arr As String() = arrData(j).ToString().Split(":")
                                    'checking Real data that will be supplyed using : seperaotr
                                    If arr.Length > 2 Then
                                        If arr(0).ToUpper() = DisplayName.ToUpper() Then
                                            IsParameterSupplyed = True
                                            If IsRequired = 1 And (arr(1).Trim() = "" Or arr(1) = Nothing) Then
                                                IsValidData = False
                                            End If
                                            'checking wether the concern field is mandatory or not
                                        End If
                                    Else
                                        IsValidData = False
                                        Result = "Insufficient parameter"
                                    End If
                                    If IsValidData = False Then
                                        Exit For
                                    End If
                                Next
                                If IsValidData = False Then
                                    Exit For
                                End If
                            Next
                        Else
                            Result = "Insufficient parameter"
                        End If
                    End If
                Else
                    'Din't get all the required parameter 
                    Result = "Insufficient parameter"
                End If
            Else
                Result = "Invalid validation credantial."
            End If
        Catch ex As Exception
            Return "Sorry!!! Your request can not be processed at the moment.Please Try again later ."
        End Try
        Return Result
    End Function

    Function SaveData(Data As Stream) As String Implements IBPMMobile.SaveData
        Dim Result = ""
        Dim Key As String = "Ajeet.kumar@myndsol.com", EID As String = "42", DocType As String = "", UID As Integer = 0, ImiNumber As String = ""
        Try
            ' Dim BPMKEY As String = ConfigurationManager.AppSettings("BPMKEY").ToString()
            Dim reader As New StreamReader(Data)
            Dim strData As String = reader.ReadToEnd()
            'strData = HttpContext.Current.Server.UrlDecode(strData)
            strData = HttpUtility.UrlDecode(strData)
            Dim Data1 As New StringBuilder(strData)
            Dim IsProcessed = False
            Dim arr = strData.Split("~")
            UID = arr(0)
            DocType = arr(1)
            Dim le = arr(2).ToString().Length
            strData = arr(2).Substring(1, arr(2).ToString().Length - 1)
            If BPMKEY = Key Then
                Result = CommanUtil.ValidateParameterByDocumentType(EID, DocType, UID, strData)
            Else
                Result = "Sorry!!! Authentication failed."
            End If
            IsProcessed = True
            CommanUtil.SaveServicerequest(Data1, "BPMMobile", "SaveData", Result)
        Catch ex As Exception
            Return "Sorry!!! Your request can not be completed at the moment. Try again later."
        End Try
        Return Result
    End Function

    Function SaveDataA(Data As Stream) As String Implements IBPMMobile.SaveDataA
        Dim Result = ""
        Dim Key As String = "", EID As String = "", DocType As String = "", UID As Integer = 0, ImINumber As String = ""
        Try
            ' Dim BPMKEY As String = ConfigurationManager.AppSettings("BPMKEY").ToString()
            Dim reader As New StreamReader(Data)
            Dim strData As String = reader.ReadToEnd()
            'strData = HttpContext.Current.Server.UrlDecode(strData)
            strData = HttpUtility.UrlDecode(strData)
            Dim Data1 As New StringBuilder(strData)
            Dim IsProcessed = False
            Dim arr = strData.Split("~")
            UID = arr(0)
            DocType = arr(1)
            EID = arr(2)
            Key = arr(3)
            ImINumber = arr(4)
            Dim le = arr(5).ToString().Length
            strData = arr(5).Substring(1, arr(5).ToString().Length - 1)
            Dim objDev As New DeviceInfo()
            objDev.IMINumber = ImINumber
            Dim IsValidrequest = False
            Dim dsD As New DataSet()
            dsD = objDev.ValidateDevice(EID, UID)
            If dsD.Tables(0).Rows.Count > 0 Then
                IsValidrequest = True
            End If

            If BPMKEY = Key And IsValidrequest = True Then
                Result = CommanUtil.ValidateParameterByDocumentType(EID, DocType, UID, strData)
            Else
                Result = "Sorry!!! Authentication failed."
            End If
            IsProcessed = True
            CommanUtil.SaveServicerequest(Data1, "BPMMobile", "SaveData", Result)
        Catch ex As Exception
            Return "Sorry!!! Your request can not be completed at the moment. Try again later."
        End Try
        Return Result
    End Function


    Function SaveDraft(Data As Stream) As String Implements IBPMMobile.SaveDraft
        Dim Result = ""
        Dim Key As String = "Ajeet.kumar@myndsol.com", EID As String = "42", DocType As String = "", UID As Integer = 0, DocID = 0
        Try
            ' Dim BPMKEY As String = ConfigurationManager.AppSettings("BPMKEY").ToString()
            Dim reader As New StreamReader(Data)
            Dim strData As String = reader.ReadToEnd()
            'strData = HttpContext.Current.Server.UrlDecode(strData)
            strData = HttpUtility.UrlDecode(strData)
            Dim Data1 As New StringBuilder(strData)

            Dim arr = strData.Split("~")
            UID = arr(0)
            DocType = arr(1)
            DocID = arr(2)
            strData = arr(3).Substring(1, arr(3).ToString().Length - 1)
            If BPMKEY = Key Then
                Result = CommanUtil.SaveDraft(EID, DocType, UID, strData, DocID)
            Else
                Result = "Sorry!!! Authentication failed."
            End If
            CommanUtil.SaveServicerequest(Data1, "BPMMobile", "SaveDraft", Result)
        Catch ex As Exception
            Return "Sorry!!! Your request can not be completed at the moment. Try again later."
        End Try
        Return Result
    End Function

    Function SaveDraftA(Data As Stream) As String Implements IBPMMobile.SaveDraftA
        Dim Result = ""
        Dim Key As String = "Ajeet.kumar@myndsol.com", EID As String = "42", DocType As String = "", UID As Integer = 0, DocID = 0, ImINumber As String = ""
        Try
            ' Dim BPMKEY As String = ConfigurationManager.AppSettings("BPMKEY").ToString()
            Dim reader As New StreamReader(Data)
            Dim strData As String = reader.ReadToEnd()
            'strData = HttpContext.Current.Server.UrlDecode(strData)
            strData = HttpUtility.UrlDecode(strData)
            Dim Data1 As New StringBuilder(strData)

            Dim arr = strData.Split("~")
            UID = arr(0)
            DocType = arr(1)
            DocID = arr(2)
            EID = arr(3)
            strData = arr(4).Substring(1, arr(4).ToString().Length - 1)
            Dim objDev As New DeviceInfo()
            objDev.IMINumber = ImINumber
            Dim IsValidrequest = False
            Dim dsD As New DataSet()
            dsD = objDev.ValidateDevice(EID, UID)
            If dsD.Tables(0).Rows.Count > 0 Then
                IsValidrequest = True
            End If

            If BPMKEY = Key And IsValidrequest = True Then
                Result = CommanUtil.SaveDraft(EID, DocType, UID, strData, DocID)
            Else
                Result = "Sorry!!! Authentication failed."
            End If
            CommanUtil.SaveServicerequest(Data1, "BPMMobile", "SaveDraft", Result)
        Catch ex As Exception
            Return "Sorry!!! Your request can not be completed at the moment. Try again later."
        End Try
        Return Result
    End Function

    Function UpdateData(Data As Stream) As String Implements IBPMMobile.UpdateData
        Dim Result = ""
        Dim Key As String = "", EID As String = "", DocType As String = "", UID As String = 0
        Try
            Dim BPMKEY As String = ConfigurationManager.AppSettings("BPMKEY").ToString()
            Dim reader As New StreamReader(Data)
            Dim strData As String = reader.ReadToEnd()
            'strData = HttpContext.Current.Server.UrlDecode(strData)
            strData = HttpUtility.UrlDecode(strData)
            Dim Data1 As New StringBuilder(strData)
            Dim IsProcessed = False
            Dim arr = strData.Split("~")
            UID = arr(0)
            DocType = arr(1)
            EID = arr(2)
            Key = arr(3)

            Dim le = arr(4).ToString().Length
            strData = arr(4).Substring(1, arr(4).ToString().Length - 1)
            'Dim arr = strData.Split("~")
            'For i As Integer = 0 To arr.Length - 1
            '    Dim ar = Split(arr(i), "::")
            '    If ar(0).ToUpper().Trim() = "EID" Then
            '        EID = ar(1)
            '    ElseIf ar(0).ToUpper().Trim() = "KEY" Then
            '        Key = ar(1)
            '    ElseIf ar(0).ToUpper().Trim() = "DOCTYPE" Then
            '        DocType = ar(1)
            '    ElseIf ar(0).ToUpper().Trim() = "UID" Then
            '        UID = ar(1)
            '    End If
            'Next
            Dim DS As New DataSet()
            'DS = AuthenticateWSRequest(Key)
            If BPMKEY = Key.Trim Then
                'EID = DS.Tables(0).Rows(0).Item("EID")
                'UID = DS.Tables(1).Rows(0).Item("uid")
                Dim objUp As New UpdateData()
                Result = objUp.UpdateData(EID, DocType, UID, strData, "IsEditOnamend")
            Else
                Result = "Sorry!!! Authentication failed."
            End If
            CommanUtil.SaveServicerequest(Data1, "BPMMobile", "UpdateData", Result)
        Catch ex As Exception
            ErrorLog.sendMail("BPMMOBILE.UpdateData", ex.Message)
            Return "RTO"
        End Try
        Return Result
    End Function

    Function UpdateDataA(Data As Stream) As String Implements IBPMMobile.UpdateDataA
        Dim Result = ""
        Dim Key As String = "", EID As String = "", DocType As String = "", UID As String = 0, ImINumber As String = ""
        Try
            Dim BPMKEY As String = ConfigurationManager.AppSettings("BPMKEY").ToString()
            Dim reader As New StreamReader(Data)
            Dim strData As String = reader.ReadToEnd()
            'strData = HttpContext.Current.Server.UrlDecode(strData)
            strData = HttpUtility.UrlDecode(strData)
            Dim Data1 As New StringBuilder(strData)
            Dim IsProcessed = False
            Dim arr = strData.Split("~")
            UID = arr(0)
            DocType = arr(1)
            EID = arr(2)
            Key = arr(3)
            ImINumber = arr(4)
            Dim le = arr(5).ToString().Length
            strData = arr(5).Substring(1, arr(5).ToString().Length - 1)
            Dim objDev As New DeviceInfo()
            objDev.IMINumber = ImINumber
            Dim IsValidrequest = False
            Dim dsD As New DataSet()
            dsD = objDev.ValidateDevice(EID, UID)
            If dsD.Tables(0).Rows.Count > 0 Then
                IsValidrequest = True
            End If
            Dim DS As New DataSet()
            If BPMKEY = Key.Trim And IsValidrequest = True Then
                Dim objUp As New UpdateData()
                Result = objUp.UpdateData(EID, DocType, UID, strData, "IsEditOnamend")
            Else
                Result = "Sorry!!! Authentication failed."
            End If
            CommanUtil.SaveServicerequest(Data1, "BPMMobile", "UpdateData", Result)
        Catch ex As Exception
            ErrorLog.sendMail("BPMMOBILE.UpdateDataA", ex.Message)
            Return "RTO"
        End Try
        Return Result
    End Function


    Function DropDownFilter(Key As String, EID As String, FieldID As Integer, DOCID As Integer) As List(Of DropDownData) Implements IBPMMobile.DropDownFilter
        Dim Result As New List(Of DropDownData)
        Dim objDD As DropDownData
        Dim lstData As List(Of Data)
        Dim ObjData As Data
        Dim con As SqlConnection = Nothing
        Dim da As SqlDataAdapter = Nothing
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Try
            If Key = BPMKEY Then
                Dim dsFields As New DataSet()
                Dim AutoFilter = "", DropDown = "", LookupValue = "", tableName = ""
                dsFields = CommanUtil.GetFormFields(EID:=EID, FieldID:=FieldID)
                If dsFields.Tables("fields").Rows.Count > 0 Then
                    DropDown = Convert.ToString(dsFields.Tables("fields").Rows(0).Item("DropDown"))
                    LookupValue = Convert.ToString(dsFields.Tables("fields").Rows(0).Item("LookupValue"))
                    LookupValue = LookupValue.Trim(",".ToString())
                    If (DropDown <> String.Empty And DropDown <> "") And (LookupValue <> String.Empty And LookupValue <> "") Then
                        Dim arrlookUp As String() = LookupValue.Split(",")
                        If arrlookUp.Count > 0 Then
                            'Dim arrFldPair=
                            Dim dsTrgFld As DataSet
                            Dim dsData As DataSet
                            Dim Flds = "", Filter = "", Doctype = ""
                            'Now getting the target field
                            Dim strData = ""
                            For i As Integer = 0 To arrlookUp.Count - 1
                                Dim arrFldPair = arrlookUp(i).ToString().Split("-")
                                dsTrgFld = New DataSet()
                                dsTrgFld = CommanUtil.GetFormFields(EID:=EID, FieldID:=arrFldPair(0))
                                objDD = New DropDownData()
                                'Setting two comman properties
                                If dsTrgFld.Tables(0).Rows.Count > 0 Then
                                    Try
                                        objDD.ControlID = dsTrgFld.Tables("fields").Rows(0).Item("FieldId").ToString()
                                        objDD.Controltype = dsTrgFld.Tables("fields").Rows(0).Item("FieldType").ToString()
                                    Catch ex As Exception

                                    End Try

                                    lstData = New List(Of Data)
                                    'IF the target field is drop down list
                                    If arrFldPair(1) = "S" Or arrFldPair(1) = "R" Then
                                    Else
                                        'IF target field is look up in this case all the informmation is stored in the source field property
                                        Dim arrFile1 As String() = Convert.ToString(dsFields.Tables("fields").Rows(0).Item("DropDown")).Split("-")
                                        'Getting table name of targeted field
                                        If arrFile1(0).ToUpper() = "MASTER" Then
                                            tableName = "MMM_MST_MASTER"
                                        ElseIf arrFile1(0).ToUpper() = "DOCUMENT" Then
                                            tableName = "MMM_MST_DOC"
                                        ElseIf arrFile1(0).ToString.ToUpper = "SESSION" Then
                                            tableName = "MMM_MST_USER"
                                        ElseIf arrFile1(0).ToString.ToUpper = "USER" Then
                                            tableName = "MMM_MST_USER"
                                        Else
                                            tableName = "MMM_MST_DOC_ITEM"
                                        End If
                                        Flds = arrFldPair(1).ToString()
                                        Doctype = arrFile1(1)
                                        dsData = New DataSet()
                                        ObjData = New Data()
                                        If tableName = "MMM_MST_USER" Then
                                            Filter = "AND UID= '" & DOCID & "'"
                                        Else

                                            Filter = "AND tid= '" & DOCID & "'"
                                        End If
                                        'Get the field property of drop down

                                        Dim StrQuery = "select * FROM MMM_MST_FIELDS where Documenttype='" & Doctype & "' AND FieldMapping='" & Flds & "' AND EID=" & EID
                                        Dim dsFlddef As New DataSet()

                                        con = New SqlConnection(conStr)
                                        da = New SqlDataAdapter(StrQuery, con)
                                        If con.State <> ConnectionState.Open Then
                                            con.Open()
                                        End If
                                        da.Fill(dsFlddef)
                                        Dim tFldtype = ""
                                        Dim DDLtype = ""
                                        Dim TblName = ""
                                        Dim arr2 As String()
                                        If dsFlddef.Tables(0).Rows.Count > 0 Then
                                            tFldtype = dsFlddef.Tables(0).Rows(0).Item("FieldType")
                                            DDLtype = dsFlddef.Tables(0).Rows(0).Item("DropDownType")
                                        End If

                                        If DDLtype.ToUpper() = "MASTER VALUED" Or (DDLtype.ToUpper() = "SESSION VALUED") And tFldtype.ToUpper() = "DROP DOWN" Then

                                            Dim dsTID = CommanUtil.GetDropDownData(EID:=EID, Doctype:=Doctype, FieldName:=Flds, Filter:=Filter, TableName:=tableName)
                                            If dsTID.Tables(0).Rows.Count > 0 And Not String.IsNullOrEmpty(dsTID.Tables(0).Rows(0).Item(Flds).ToString()) Then
                                                Filter = "AND tid= '" & dsTID.Tables(0).Rows(0).Item(Flds).ToString() & "'"
                                                arr2 = dsFlddef.Tables(0).Rows(0).Item("DropDown").ToString().Split("-")
                                                TblName = arr2(1)
                                                Flds = arr2(2)
                                                If arr2(0).ToUpper() = "MASTER" Then
                                                    tableName = "MMM_MST_MASTER"
                                                ElseIf arr2(0).ToUpper() = "DOCUMENT" Then
                                                    tableName = "MMM_MST_DOC"
                                                ElseIf arr2(0).ToString.ToUpper = "SESSION" Then
                                                    tableName = "MMM_MST_USER"
                                                ElseIf arr2(0).ToString.ToUpper = "USER" Then
                                                    tableName = "MMM_MST_USER"
                                                Else
                                                    tableName = "MMM_MST_DOC_ITEM"
                                                End If
                                                Doctype = arr2(1)
                                                Try
                                                    dsData = CommanUtil.GetDropDownData(EID:=EID, Doctype:=Doctype, FieldName:=Flds, Filter:=Filter, TableName:=tableName)
                                                Catch ex As Exception
                                                    dsData.Tables.Add()
                                                End Try
                                            Else
                                                dsData.Tables.Add()
                                            End If
                                        Else
                                            Try
                                                dsData = CommanUtil.GetDropDownData(EID:=EID, Doctype:=Doctype, FieldName:=Flds, Filter:=Filter, TableName:=tableName)
                                            Catch ex As Exception
                                                dsData.Tables.Add()
                                            End Try
                                        End If
                                        strData = ""
                                        If (Not dsData.Tables(0) Is Nothing) And dsData.Tables(0).Rows.Count > 0 Then
                                            If strData = "" Then
                                                strData = dsData.Tables(0).Rows(0).Item(Flds).ToString() & ":" & dsData.Tables(0).Rows(0).Item(Flds).ToString()
                                            Else
                                                strData = strData & "$" & dsData.Tables(0).Rows(0).Item(Flds).ToString() & ":" & dsData.Tables(0).Rows(0).Item(Flds).ToString()
                                            End If
                                        Else
                                            If strData = "" Then
                                                strData = "" & ":" & ""
                                            Else
                                                strData = strData & "$" & "" & ":" & ""
                                            End If
                                        End If
                                    End If
                                    objDD.Item = strData
                                    Result.Add(objDD)
                                End If
                            Next
                        End If
                    End If
                End If
            End If
        Catch ex As Exception
            Throw
        Finally
            con.Dispose()
            da.Dispose()
        End Try
        Return Result
    End Function

    Function GetExternalLookup(Key As String, EID As String, FieldID As Integer, Value As String, UID As Integer) As List(Of DropDownData) Implements IBPMMobile.GetExternalLookup
        Dim Result As New List(Of DropDownData)
        Dim objDD As DropDownData
        Dim lstData As List(Of Data)
        Dim ObjData As Data
        Dim con As SqlConnection = Nothing
        Dim da As SqlDataAdapter = Nothing
        Dim DOCID As Integer
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Try
            If Key = BPMKEY Then
                Dim dsFields As New DataSet()
                Dim AutoFilter = "", DropDownDef = "", LookupValue = "", tableName = "", DropDownType = "", DDLLookupValue = ""
                dsFields = CommanUtil.GetFormFields(EID:=EID, FieldID:=FieldID)
                If dsFields.Tables("fields").Rows.Count > 0 Then
                    DropDownDef = Convert.ToString(dsFields.Tables("fields").Rows(0).Item("DropDown"))
                    LookupValue = Convert.ToString(dsFields.Tables("fields").Rows(0).Item("LookupValue"))
                    DropDownType = Convert.ToString(dsFields.Tables("fields").Rows(0).Item("DropDownType"))
                    LookupValue = LookupValue.Trim(",".ToString())
                    'Add condition for ddl lookup value
                    DDLLookupValue = Convert.ToString(dsFields.Tables("fields").Rows(0).Item("DDllookupvalue"))
                    DDLLookupValue = DDLLookupValue.Trim(",".ToString())

                    Dim DR As DataRow = dsFields.Tables(0).Rows(0)
                    DOCID = DropDown.GetDropDownID(DR, EID, Value)
                    If (DropDownDef <> String.Empty And DropDownDef <> "") And (LookupValue <> String.Empty And LookupValue <> "") Then
                        Dim arrlookUp As String() = LookupValue.Split(",")
                        If arrlookUp.Count > 0 Then
                            'Dim arrFldPair=
                            Dim dsTrgFld As DataSet
                            Dim dsData As DataSet
                            Dim Flds = "", Filter = "", Doctype = ""
                            'Now getting the target field
                            Dim strData = ""
                            For i As Integer = 0 To arrlookUp.Count - 1
                                Dim arrFldPair = arrlookUp(i).ToString().Split("-")
                                dsTrgFld = New DataSet()
                                dsTrgFld = CommanUtil.GetFormFields(EID:=EID, FieldID:=arrFldPair(0))
                                objDD = New DropDownData()
                                'Setting two comman properties
                                If dsTrgFld.Tables(0).Rows.Count > 0 Then
                                    Try
                                        objDD.ControlID = dsTrgFld.Tables("fields").Rows(0).Item("FieldId").ToString()
                                        objDD.Controltype = dsTrgFld.Tables("fields").Rows(0).Item("FieldType").ToString()
                                    Catch ex As Exception

                                    End Try

                                    lstData = New List(Of Data)
                                    'IF the target field is drop down list
                                    If arrFldPair(1) = "S" Or arrFldPair(1) = "R" Then
                                        If DropDownType.Trim.ToUpper = "SESSION VALUED" Then
                                            Dim arrFile1 As String() = Convert.ToString(dsFields.Tables("fields").Rows(0).Item("DropDown")).Split("-")
                                            If arrFile1(0).ToUpper() = "MASTER" Then
                                                tableName = "MMM_MST_MASTER"
                                            ElseIf arrFile1(0).ToUpper() = "DOCUMENT" Then
                                                tableName = "MMM_MST_DOC"
                                            ElseIf arrFile1(0).ToString.ToUpper = "SESSION" Then
                                                tableName = "MMM_MST_USER"
                                            ElseIf arrFile1(0).ToString.ToUpper = "USER" Then
                                                tableName = "MMM_MST_USER"
                                            Else
                                                tableName = "MMM_MST_DOC_ITEM"
                                            End If
                                            Flds = Convert.ToString(dsTrgFld.Tables("fields").Rows(0).Item("AutoFilter"))
                                            Dim tarDropDown() = Convert.ToString(dsTrgFld.Tables("fields").Rows(0).Item("DropDown")).Split("-")
                                            Doctype = arrFile1(1)
                                            dsData = New DataSet()
                                            ObjData = New Data()
                                            If tableName = "MMM_MST_USER" Then
                                                Filter = " UID= '" & DOCID & "'"
                                            Else

                                                Filter = " tid= '" & DOCID & "'"
                                            End If
                                            'Get the field property of drop down
                                            Try
                                                Dim targetTableName As String = ""
                                                If tarDropDown(0).ToUpper() = "MASTER" Then
                                                    targetTableName = "MMM_MST_MASTER"
                                                ElseIf tarDropDown(0).ToUpper() = "DOCUMENT" Then
                                                    targetTableName = "MMM_MST_DOC"
                                                ElseIf tarDropDown(0).ToString.ToUpper = "SESSION" Then
                                                    targetTableName = "MMM_MST_USER"
                                                ElseIf tarDropDown(0).ToString.ToUpper = "USER" Then
                                                    targetTableName = "MMM_MST_USER"
                                                Else
                                                    targetTableName = "MMM_MST_DOC_ITEM"
                                                End If

                                                Dim StrQuery = " select " & tarDropDown(2) & " from " & targetTableName & " where tid in (select " & Flds & " from " & tableName & " where " & Filter & ")"
                                                Dim dsFlddef As New DataSet()

                                                con = New SqlConnection(conStr)
                                                da = New SqlDataAdapter(StrQuery, con)
                                                If con.State <> ConnectionState.Open Then
                                                    con.Open()
                                                End If
                                                da.Fill(dsFlddef)
                                                If (Not dsFlddef.Tables(0) Is Nothing) And dsFlddef.Tables(0).Rows.Count > 0 Then
                                                    strData = dsFlddef.Tables(0).Rows(0).Item(0).ToString()
                                                End If
                                            Catch ex As Exception

                                            End Try


                                        End If
                                    Else
                                        'IF target field is look up in this case all the informmation is stored in the source field property
                                        Dim arrFile1 As String() = Convert.ToString(dsFields.Tables("fields").Rows(0).Item("DropDown")).Split("-")
                                        'Getting table name of targeted field
                                        If arrFile1(0).ToUpper() = "MASTER" Then
                                            tableName = "MMM_MST_MASTER"
                                        ElseIf arrFile1(0).ToUpper() = "DOCUMENT" Then
                                            tableName = "MMM_MST_DOC"
                                        ElseIf arrFile1(0).ToString.ToUpper = "SESSION" Then
                                            tableName = "MMM_MST_USER"
                                        ElseIf arrFile1(0).ToString.ToUpper = "USER" Then
                                            tableName = "MMM_MST_USER"
                                        Else
                                            tableName = "MMM_MST_DOC_ITEM"
                                        End If
                                        Flds = arrFldPair(1).ToString()
                                        Doctype = arrFile1(1)
                                        dsData = New DataSet()
                                        ObjData = New Data()
                                        If tableName = "MMM_MST_USER" Then
                                            Filter = "And UID= '" & DOCID & "'"
                                        Else

                                            Filter = "AND tid= '" & DOCID & "'"
                                        End If
                                        'Get the field property of drop down

                                        Dim StrQuery = "select * FROM MMM_MST_FIELDS where Documenttype='" & Doctype & "' AND FieldMapping='" & Flds & "' AND EID=" & EID
                                        Dim dsFlddef As New DataSet()

                                        con = New SqlConnection(conStr)
                                        da = New SqlDataAdapter(StrQuery, con)
                                        If con.State <> ConnectionState.Open Then
                                            con.Open()
                                        End If
                                        da.Fill(dsFlddef)
                                        Dim tFldtype = ""
                                        Dim DDLtype = ""
                                        Dim TblName = ""
                                        Dim arr2 As String()
                                        If dsFlddef.Tables(0).Rows.Count > 0 Then
                                            tFldtype = dsFlddef.Tables(0).Rows(0).Item("FieldType")
                                            DDLtype = dsFlddef.Tables(0).Rows(0).Item("DropDownType")
                                        End If

                                        If DDLtype.ToUpper() = "MASTER VALUED" Or (DDLtype.ToUpper() = "SESSION VALUED") And tFldtype.ToUpper() = "DROP DOWN" Then

                                            Dim dsTID = CommanUtil.GetDropDownData(EID:=EID, Doctype:=Doctype, FieldName:=Flds, Filter:=Filter, TableName:=tableName)
                                            If dsTID.Tables(0).Rows.Count > 0 And Not String.IsNullOrEmpty(dsTID.Tables(0).Rows(0).Item(Flds).ToString()) Then
                                                Filter = "AND tid= '" & dsTID.Tables(0).Rows(0).Item(Flds).ToString() & "'"
                                                arr2 = dsFlddef.Tables(0).Rows(0).Item("DropDown").ToString().Split("-")
                                                TblName = arr2(1)
                                                Flds = arr2(2)
                                                If arr2(0).ToUpper() = "MASTER" Then
                                                    tableName = "MMM_MST_MASTER"
                                                ElseIf arr2(0).ToUpper() = "DOCUMENT" Then
                                                    tableName = "MMM_MST_DOC"
                                                ElseIf arr2(0).ToString.ToUpper = "SESSION" Then
                                                    tableName = "MMM_MST_USER"
                                                ElseIf arr2(0).ToString.ToUpper = "USER" Then
                                                    tableName = "MMM_MST_USER"
                                                Else
                                                    tableName = "MMM_MST_DOC_ITEM"
                                                End If
                                                Doctype = arr2(1)
                                                Try
                                                    dsData = CommanUtil.GetDropDownData(EID:=EID, Doctype:=Doctype, FieldName:=Flds, Filter:=Filter, TableName:=tableName)
                                                Catch ex As Exception
                                                    dsData.Tables.Add()
                                                End Try
                                            Else
                                                dsData.Tables.Add()
                                            End If
                                        Else
                                            Try
                                                dsData = CommanUtil.GetDropDownData(EID:=EID, Doctype:=Doctype, FieldName:=Flds, Filter:=Filter, TableName:=tableName)
                                            Catch ex As Exception
                                                dsData.Tables.Add()
                                            End Try
                                        End If
                                        strData = ""
                                        If (Not dsData.Tables(0) Is Nothing) And dsData.Tables(0).Rows.Count > 0 Then
                                            strData = dsData.Tables(0).Rows(0).Item(Flds).ToString()
                                        Else
                                            If strData = "" Then
                                                strData = "" & ":" & ""
                                            Else
                                                strData = strData & "$" & "" & ":" & ""
                                            End If
                                        End If
                                    End If
                                    objDD.Item = strData
                                    Result.Add(objDD)
                                End If
                            Next
                        End If
                    End If
                    'Add condition for ddllookup value in fields table
                    If (DropDownDef <> String.Empty And DropDownDef <> "") And (DDLLookupValue <> String.Empty And DDLLookupValue <> "") Then
                        Dim arrddllookUp As String() = DDLLookupValue.Split(",")
                        If arrddllookUp.Count > 0 Then
                            Dim dsTrgFld As DataSet
                            Dim Flds = "", Filter = "", Doctype = ""
                            For i As Integer = 0 To arrddllookUp.Count - 1
                                Dim arrFldPair = arrddllookUp(i).ToString().Split("-")
                                dsTrgFld = New DataSet()
                                dsTrgFld = CommanUtil.GetFormFields(EID:=EID, FieldID:=arrFldPair(0))
                                objDD = New DropDownData()
                                Dim MainDocumenttype As String() = DropDownDef.Split("-")
                                If dsTrgFld.Tables(0).Rows.Count > 0 Then
                                    Dim DDLlookupValueSource As String = ""
                                    Try
                                        objDD.ControlID = dsTrgFld.Tables("fields").Rows(0).Item("FieldId").ToString()
                                        objDD.Controltype = dsTrgFld.Tables("fields").Rows(0).Item("FieldType").ToString()
                                        DDLlookupValueSource = dsTrgFld.Tables(0).Rows(0)("DDLlookupValueSource")
                                    Catch ex As Exception
                                    End Try

                                    Dim objDC As New DataClass
                                    Dim objdt As New DataTable
                                    objdt = objDC.ExecuteQryDT("select fieldmapping,dropdown from mmm_mst_fields where documenttype='" & MainDocumenttype(1) & "' and eid=" & EID & " and SUBSTRING(dropdown,CHARINDEX('-',dropdown) + 1,CHARINDEX('-',dropdown,CHARINDEX('-',dropdown) + 1) - (CHARINDEX('-',dropdown) + 1))='" & DDLlookupValueSource & "' and dropdowntype='Master Valued'")
                                    If objdt.Rows.Count > 0 Then
                                        objDD.Item = GetDropDownLookupValue(objdt.Rows(0)("dropdown"), DOCID, eid:=EID, fieldMapping:=objdt.Rows(0)("fieldMapping"))
                                        Result.Add(objDD)
                                    End If
                                End If
                            Next
                        End If
                    End If
                End If
            End If
        Catch ex As Exception
            Throw
        Finally
            If Not con Is Nothing Then
                con.Dispose()
                da.Dispose()
            End If
        End Try
        Return Result
    End Function

    Function GetDropDownLookupValue(ByVal dropdown As String, ByVal tid As Int32, ByVal eid As Int32, ByVal fieldMapping As String) As String
        Dim result As String = ""
        Dim objDC As New DataClass
        Dim splitDropDown As String() = dropdown.ToString().Split("-")
        result = objDC.ExecuteQryScaller("select " & splitDropDown(2) & " from  mmm_mst_master  where tid in (select " & fieldMapping & " from mmm_mst_master where tid= " & tid & ")")
        Return result
    End Function



    Function GetChildItemDefaultValue(Key As String, EID As String, ChildItemName As String) As List(Of ChildItemDefaultValue) Implements IBPMMobile.GetChildItemDefaultValue
        Dim lstFlds As New List(Of ChildItemDefaultValue)
        Try
            Dim ds As New DataSet()
            Dim obj As ChildItemDefaultValue
            ds = CommanUtil.GetDropDownDefaultValue(EID, ChildItemName)
            If ds.Tables(0).Rows.Count > 0 Then
                For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                    obj = New ChildItemDefaultValue()
                    obj.fld1 = ds.Tables(0).Rows(i).Item("fld1").ToString()
                    obj.fld2 = ds.Tables(0).Rows(i).Item("fld2").ToString()
                    obj.fld3 = ds.Tables(0).Rows(i).Item("fld3").ToString()
                    obj.fld4 = ds.Tables(0).Rows(i).Item("fld4").ToString()
                    obj.fld5 = ds.Tables(0).Rows(i).Item("fld5").ToString()
                    obj.fld6 = ds.Tables(0).Rows(i).Item("fld6").ToString()
                    obj.fld7 = ds.Tables(0).Rows(i).Item("fld7").ToString()
                    obj.fld8 = ds.Tables(0).Rows(i).Item("fld8").ToString()
                    obj.fld9 = ds.Tables(0).Rows(i).Item("fld9").ToString()
                    obj.fld10 = ds.Tables(0).Rows(i).Item("fld10").ToString()
                    obj.fld11 = ds.Tables(0).Rows(i).Item("fld11").ToString()
                    obj.fld12 = ds.Tables(0).Rows(i).Item("fld12").ToString()
                    obj.fld13 = ds.Tables(0).Rows(i).Item("fld13").ToString()
                    obj.fld14 = ds.Tables(0).Rows(i).Item("fld14").ToString()
                    obj.fld15 = ds.Tables(0).Rows(i).Item("fld15").ToString()
                    obj.fld16 = ds.Tables(0).Rows(i).Item("fld16").ToString()
                    obj.fld17 = ds.Tables(0).Rows(i).Item("fld17").ToString()
                    obj.fld18 = ds.Tables(0).Rows(i).Item("fld18").ToString()
                    obj.fld19 = ds.Tables(0).Rows(i).Item("fld19").ToString()
                    obj.fld20 = ds.Tables(0).Rows(i).Item("fld20").ToString()

                    obj.fld21 = ds.Tables(0).Rows(i).Item("fld21").ToString()
                    obj.fld22 = ds.Tables(0).Rows(i).Item("fld22").ToString()
                    obj.fld23 = ds.Tables(0).Rows(i).Item("fld23").ToString()
                    obj.fld24 = ds.Tables(0).Rows(i).Item("fld24").ToString()
                    obj.fld25 = ds.Tables(0).Rows(i).Item("fld25").ToString()
                    obj.fld26 = ds.Tables(0).Rows(i).Item("fld26").ToString()
                    obj.fld27 = ds.Tables(0).Rows(i).Item("fld27").ToString()
                    obj.fld28 = ds.Tables(0).Rows(i).Item("fld28").ToString()
                    obj.fld29 = ds.Tables(0).Rows(i).Item("fld29").ToString()
                    obj.fld30 = ds.Tables(0).Rows(i).Item("fld30").ToString()

                    obj.fld30 = ds.Tables(0).Rows(i).Item("fld30").ToString()
                    obj.fld31 = ds.Tables(0).Rows(i).Item("fld31").ToString()
                    obj.fld32 = ds.Tables(0).Rows(i).Item("fld32").ToString()
                    obj.fld33 = ds.Tables(0).Rows(i).Item("fld33").ToString()
                    obj.fld34 = ds.Tables(0).Rows(i).Item("fld34").ToString()
                    obj.fld35 = ds.Tables(0).Rows(i).Item("fld35").ToString()
                    obj.fld36 = ds.Tables(0).Rows(i).Item("fld36").ToString()
                    obj.fld37 = ds.Tables(0).Rows(i).Item("fld37").ToString()
                    obj.fld38 = ds.Tables(0).Rows(i).Item("fld38").ToString()
                    obj.fld39 = ds.Tables(0).Rows(i).Item("fld39").ToString()
                    obj.fld40 = ds.Tables(0).Rows(i).Item("fld40").ToString()

                    obj.fld41 = ds.Tables(0).Rows(i).Item("fld41").ToString()
                    obj.fld42 = ds.Tables(0).Rows(i).Item("fld42").ToString()
                    obj.fld43 = ds.Tables(0).Rows(i).Item("fld43").ToString()
                    obj.fld44 = ds.Tables(0).Rows(i).Item("fld44").ToString()
                    obj.fld45 = ds.Tables(0).Rows(i).Item("fld45").ToString()
                    obj.fld46 = ds.Tables(0).Rows(i).Item("fld46").ToString()
                    obj.fld47 = ds.Tables(0).Rows(i).Item("fld47").ToString()
                    obj.fld48 = ds.Tables(0).Rows(i).Item("fld48").ToString()
                    obj.fld49 = ds.Tables(0).Rows(i).Item("fld49").ToString()
                    obj.fld50 = ds.Tables(0).Rows(i).Item("fld50").ToString()
                    lstFlds.Add(obj)
                Next
            End If
        Catch ex As Exception
        End Try
        Return lstFlds
    End Function

    Function GetChildItemDefaultValueALL(Key As String, EID As String) As List(Of ChildItemDefaultValue) Implements IBPMMobile.GetChildItemDefaultValueALL
        Dim lstFlds As New List(Of ChildItemDefaultValue)
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim da As SqlDataAdapter = Nothing
        Dim con As SqlConnection = Nothing
        Try
            Dim ds As New DataSet()
            Dim strQuery = "SELECT Substring(DocumentType,1,len(DocumentType)-7)'DocType', * FROM MMM_MST_MASTER WHERE EID=" & EID & " AND  isauth =1 AND DocumentType in(select FormName+'_Master' FROM MMM_MST_FORMS where EID=" & EID & " and hasdefaultvalue=1 and FormSource='DETAIL FORM') order by DocumentType"
            con = New SqlConnection(conStr)
            con.Open()
            da = New SqlDataAdapter(strQuery, con)
            da.Fill(ds)
            Dim obj As ChildItemDefaultValue
            'ds = CommanUtil.GetDropDownDefaultValue(EID, ChildItemName)
            If ds.Tables(0).Rows.Count > 0 Then
                For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                    obj = New ChildItemDefaultValue()
                    obj.DocumentType = ds.Tables(0).Rows(i).Item("DocType").ToString()
                    obj.fld1 = ds.Tables(0).Rows(i).Item("fld1").ToString()
                    obj.fld2 = ds.Tables(0).Rows(i).Item("fld2").ToString()
                    obj.fld3 = ds.Tables(0).Rows(i).Item("fld3").ToString()
                    obj.fld4 = ds.Tables(0).Rows(i).Item("fld4").ToString()
                    obj.fld5 = ds.Tables(0).Rows(i).Item("fld5").ToString()
                    obj.fld6 = ds.Tables(0).Rows(i).Item("fld6").ToString()
                    obj.fld7 = ds.Tables(0).Rows(i).Item("fld7").ToString()
                    obj.fld8 = ds.Tables(0).Rows(i).Item("fld8").ToString()
                    obj.fld9 = ds.Tables(0).Rows(i).Item("fld9").ToString()
                    obj.fld10 = ds.Tables(0).Rows(i).Item("fld10").ToString()
                    obj.fld11 = ds.Tables(0).Rows(i).Item("fld11").ToString()
                    obj.fld12 = ds.Tables(0).Rows(i).Item("fld12").ToString()
                    obj.fld13 = ds.Tables(0).Rows(i).Item("fld13").ToString()
                    obj.fld14 = ds.Tables(0).Rows(i).Item("fld14").ToString()
                    obj.fld15 = ds.Tables(0).Rows(i).Item("fld15").ToString()
                    obj.fld16 = ds.Tables(0).Rows(i).Item("fld16").ToString()
                    obj.fld17 = ds.Tables(0).Rows(i).Item("fld17").ToString()
                    obj.fld18 = ds.Tables(0).Rows(i).Item("fld18").ToString()
                    obj.fld19 = ds.Tables(0).Rows(i).Item("fld19").ToString()
                    obj.fld20 = ds.Tables(0).Rows(i).Item("fld20").ToString()

                    obj.fld21 = ds.Tables(0).Rows(i).Item("fld21").ToString()
                    obj.fld22 = ds.Tables(0).Rows(i).Item("fld22").ToString()
                    obj.fld23 = ds.Tables(0).Rows(i).Item("fld23").ToString()
                    obj.fld24 = ds.Tables(0).Rows(i).Item("fld24").ToString()
                    obj.fld25 = ds.Tables(0).Rows(i).Item("fld25").ToString()
                    obj.fld26 = ds.Tables(0).Rows(i).Item("fld26").ToString()
                    obj.fld27 = ds.Tables(0).Rows(i).Item("fld27").ToString()
                    obj.fld28 = ds.Tables(0).Rows(i).Item("fld28").ToString()
                    obj.fld29 = ds.Tables(0).Rows(i).Item("fld29").ToString()
                    obj.fld30 = ds.Tables(0).Rows(i).Item("fld30").ToString()

                    obj.fld30 = ds.Tables(0).Rows(i).Item("fld30").ToString()
                    obj.fld31 = ds.Tables(0).Rows(i).Item("fld31").ToString()
                    obj.fld32 = ds.Tables(0).Rows(i).Item("fld32").ToString()
                    obj.fld33 = ds.Tables(0).Rows(i).Item("fld33").ToString()
                    obj.fld34 = ds.Tables(0).Rows(i).Item("fld34").ToString()
                    obj.fld35 = ds.Tables(0).Rows(i).Item("fld35").ToString()
                    obj.fld36 = ds.Tables(0).Rows(i).Item("fld36").ToString()
                    obj.fld37 = ds.Tables(0).Rows(i).Item("fld37").ToString()
                    obj.fld38 = ds.Tables(0).Rows(i).Item("fld38").ToString()
                    obj.fld39 = ds.Tables(0).Rows(i).Item("fld39").ToString()
                    obj.fld40 = ds.Tables(0).Rows(i).Item("fld40").ToString()

                    obj.fld41 = ds.Tables(0).Rows(i).Item("fld41").ToString()
                    obj.fld42 = ds.Tables(0).Rows(i).Item("fld42").ToString()
                    obj.fld43 = ds.Tables(0).Rows(i).Item("fld43").ToString()
                    obj.fld44 = ds.Tables(0).Rows(i).Item("fld44").ToString()
                    obj.fld45 = ds.Tables(0).Rows(i).Item("fld45").ToString()
                    obj.fld46 = ds.Tables(0).Rows(i).Item("fld46").ToString()
                    obj.fld47 = ds.Tables(0).Rows(i).Item("fld47").ToString()
                    obj.fld48 = ds.Tables(0).Rows(i).Item("fld48").ToString()
                    obj.fld49 = ds.Tables(0).Rows(i).Item("fld49").ToString()
                    obj.fld50 = ds.Tables(0).Rows(i).Item("fld50").ToString()
                    lstFlds.Add(obj)
                Next
            End If
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
        Return lstFlds
    End Function

    Function SyncData(Key As String, UserID As String, EID As String, URole As String, IMINum As String) As List(Of DocData) Implements IBPMMobile.SyncData
        Try


            Dim result As String = ""
            Dim ret = ""
            Dim con As SqlConnection = Nothing
            Dim da As SqlDataAdapter = Nothing
            Dim DocQuery As New StringBuilder()
            Dim MstQuery As New StringBuilder()
            Dim ChildQuery As New StringBuilder()
            Dim lstData As New List(Of DocData)
            Dim obj As DocData
            Try
                If (Key = BPMKEY) Then
                    Dim objDev As New DeviceInfo()
                    objDev.IMINumber = IMINum
                    'ret = objDev.CheckDevice(EID)
                    Dim Query = "select DropDown FROM MMM_MST_FIELDS where EID=" & EID & " AND FieldType='Drop Down' and DropDownType='Master valued'"
                    con = New SqlConnection(conStr)
                    da = New SqlDataAdapter(Query, con)
                    If con.State <> ConnectionState.Open Then
                        con.Open()
                    End If
                    Dim ds As New DataSet()
                    da.Fill(ds)
                    Dim Mstlist As New StringBuilder()
                    Dim Doclist As New StringBuilder()
                    Dim Childlist As New StringBuilder()
                    If ds.Tables(0).Rows.Count > 0 Then
                        Dim temp = ""
                        For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                            temp = ds.Tables(0).Rows(i).Item("DropDown")
                            'spliting it for getting its table name
                            Dim arr As String() = temp.Split("-")
                            If arr.Length = 3 Then
                                'if table name is master
                                If arr(0).ToUpper() = "MASTER" Then
                                    If Mstlist.ToString() <> "" Then
                                        If Not Mstlist.ToString().Contains("{" & arr(1) & "}") Then
                                            Mstlist.Append("," & "{'" & arr(1) & "'}")
                                        End If
                                    Else
                                        Mstlist.Append("{'" & arr(1) & "'}")
                                    End If
                                ElseIf arr(0).ToUpper() = "DOCUMENT" Then
                                    If Doclist.ToString() <> "" Then
                                        If Not Doclist.ToString().Contains("{" & arr(1) & "}") Then
                                            Doclist.Append("," & "{'" & arr(1) & "'}")
                                        End If
                                    Else
                                        Doclist.Append("{'" & arr(1) & "'}")
                                    End If
                                End If
                            Else
                                If Childlist.ToString() <> "" Then
                                    If Not Childlist.ToString().Contains("{" & arr(1) & "}") Then
                                        Childlist.Append("," & "{'" & arr(1) & "'}")
                                    End If
                                Else
                                    Childlist.Append("{'" & arr(1) & "'}")
                                End If
                            End If
                            'Ds loop end here
                        Next
                        MstQuery.Append("select top 100 *,0 As DOCID FROM MMM_MST_MASTER WHERE EID=" & EID & " and isauth=1")

                        DocQuery.Append("select top 100 *,0 As DOCID FROM MMM_MST_DOC WHERE EID=" & EID & " and isauth=1")

                        ChildQuery.Append("select top 100 *,1 As IsAuth FROM MMM_MST_DOC_Item ")

                        If Not String.IsNullOrEmpty(Mstlist.ToString()) Then

                            MstQuery.Append(" AND Documenttype in (" & Mstlist.ToString().Replace("{", "").Replace("}", "") & ")")

                        Else
                            MstQuery.Append(" AND Documenttype in (" & "'-11111'" & ")")
                        End If
                        If Not String.IsNullOrEmpty(Doclist.ToString()) Then
                            DocQuery.Append(" AND Documenttype in (" & Doclist.ToString().Replace("{", "").Replace("}", "") & ")")
                        Else
                            DocQuery.Append(" AND Documenttype in (" & "'-11111'" & ")")
                        End If
                        If Not String.IsNullOrEmpty(Childlist.ToString()) Then
                            ChildQuery.Append(" where Documenttype in (" & Childlist.ToString().Replace("{", "").Replace("}", "") & ")")
                        Else
                            ChildQuery.Append(" where Documenttype in (" & "'-11111'" & ")")
                        End If
                        Dim FinalQuery = MstQuery.ToString() & ";" & DocQuery.ToString() & ";" & ChildQuery.ToString()
                        If con.State <> ConnectionState.Open Then
                            con.Open()
                        End If
                        da = New SqlDataAdapter(FinalQuery, con)
                        Dim dsData As New DataSet()
                        da.Fill(dsData)
                        If dsData.Tables.Count = 3 Then
                            For i As Integer = 0 To dsData.Tables.Count - 1
                                If dsData.Tables(i).Rows.Count > 0 Then
                                    For j As Integer = 0 To dsData.Tables(i).Rows.Count - 1
                                        obj = New DocData()
                                        obj.fld1 = dsData.Tables(i).Rows(j).Item("fld1").ToString()
                                        obj.fld2 = dsData.Tables(i).Rows(j).Item("fld2").ToString()
                                        obj.fld3 = dsData.Tables(i).Rows(j).Item("fld3").ToString()
                                        obj.fld4 = dsData.Tables(i).Rows(j).Item("fld4").ToString()
                                        obj.fld5 = dsData.Tables(i).Rows(j).Item("fld5").ToString()
                                        obj.fld6 = dsData.Tables(i).Rows(j).Item("fld6").ToString()
                                        obj.fld7 = dsData.Tables(i).Rows(j).Item("fld7").ToString()
                                        obj.fld8 = dsData.Tables(i).Rows(j).Item("fld8").ToString()
                                        obj.fld9 = dsData.Tables(i).Rows(j).Item("fld9").ToString()
                                        obj.fld10 = dsData.Tables(i).Rows(j).Item("fld10").ToString()
                                        obj.fld11 = dsData.Tables(i).Rows(j).Item("fld11").ToString()
                                        obj.fld12 = dsData.Tables(i).Rows(j).Item("fld12").ToString()
                                        obj.fld13 = dsData.Tables(i).Rows(j).Item("fld13").ToString()
                                        obj.fld14 = dsData.Tables(i).Rows(j).Item("fld14").ToString()
                                        obj.fld15 = dsData.Tables(i).Rows(j).Item("fld15").ToString()
                                        obj.fld16 = dsData.Tables(i).Rows(j).Item("fld16").ToString()
                                        obj.fld17 = dsData.Tables(i).Rows(j).Item("fld17").ToString()
                                        obj.fld18 = dsData.Tables(i).Rows(j).Item("fld18").ToString()
                                        obj.fld19 = dsData.Tables(i).Rows(j).Item("fld19").ToString()
                                        obj.fld20 = dsData.Tables(i).Rows(j).Item("fld20").ToString()

                                        obj.fld21 = dsData.Tables(i).Rows(j).Item("fld21").ToString()
                                        obj.fld22 = dsData.Tables(i).Rows(j).Item("fld22").ToString()
                                        obj.fld23 = dsData.Tables(i).Rows(j).Item("fld23").ToString()
                                        obj.fld24 = dsData.Tables(i).Rows(j).Item("fld24").ToString()
                                        obj.fld25 = dsData.Tables(i).Rows(j).Item("fld25").ToString()
                                        obj.fld26 = dsData.Tables(i).Rows(j).Item("fld26").ToString()
                                        obj.fld27 = dsData.Tables(i).Rows(j).Item("fld27").ToString()
                                        obj.fld28 = dsData.Tables(i).Rows(j).Item("fld28").ToString()
                                        obj.fld29 = dsData.Tables(i).Rows(j).Item("fld29").ToString()
                                        obj.fld30 = dsData.Tables(i).Rows(j).Item("fld30").ToString()

                                        obj.fld30 = dsData.Tables(i).Rows(j).Item("fld30").ToString()
                                        obj.fld31 = dsData.Tables(i).Rows(j).Item("fld31").ToString()
                                        obj.fld32 = dsData.Tables(i).Rows(j).Item("fld32").ToString()
                                        obj.fld33 = dsData.Tables(i).Rows(j).Item("fld33").ToString()
                                        obj.fld34 = dsData.Tables(i).Rows(j).Item("fld34").ToString()
                                        obj.fld35 = dsData.Tables(i).Rows(j).Item("fld35").ToString()
                                        obj.fld36 = dsData.Tables(i).Rows(j).Item("fld36").ToString()
                                        obj.fld37 = dsData.Tables(i).Rows(j).Item("fld37").ToString()
                                        obj.fld38 = dsData.Tables(i).Rows(j).Item("fld38").ToString()
                                        obj.fld39 = dsData.Tables(i).Rows(j).Item("fld39").ToString()
                                        obj.fld40 = dsData.Tables(i).Rows(j).Item("fld40").ToString()

                                        obj.fld41 = dsData.Tables(i).Rows(j).Item("fld41").ToString()
                                        obj.fld42 = dsData.Tables(i).Rows(j).Item("fld42").ToString()
                                        obj.fld43 = dsData.Tables(i).Rows(j).Item("fld43").ToString()
                                        obj.fld44 = dsData.Tables(i).Rows(j).Item("fld44").ToString()
                                        obj.fld45 = dsData.Tables(i).Rows(j).Item("fld45").ToString()
                                        obj.fld46 = dsData.Tables(i).Rows(j).Item("fld46").ToString()
                                        obj.fld47 = dsData.Tables(i).Rows(j).Item("fld47").ToString()
                                        obj.fld48 = dsData.Tables(i).Rows(j).Item("fld48").ToString()
                                        obj.fld49 = dsData.Tables(i).Rows(j).Item("fld49").ToString()
                                        obj.fld50 = dsData.Tables(i).Rows(j).Item("fld50").ToString()
                                        obj.TID = dsData.Tables(i).Rows(j).Item("tid").ToString()
                                        obj.DocID = dsData.Tables(i).Rows(j).Item("DOCID").ToString()
                                        obj.DocumentType = dsData.Tables(i).Rows(j).Item("DocumentType").ToString()
                                        obj.IsAuth = dsData.Tables(i).Rows(j).Item("IsAuth").ToString()

                                        lstData.Add(obj)

                                    Next
                                End If
                            Next
                        End If
                    End If
                Else
                    result = "Sorry!!! Authantication failed."
                End If
            Catch ex As Exception
                Throw New FaultException(ex.Message & "Send From inner  block")
            Finally
                If Not con Is Nothing Then
                    con.Close()
                    con.Dispose()
                End If
                If Not da Is Nothing Then
                    da.Dispose()
                End If
            End Try
            Try
                Return lstData
            Catch ex As Exception
                Throw New FaultException(ex.Message & "Send From return block")
            End Try
        Catch ex As Exception
            Throw New FaultException(ex.Message & "Send From First block")
        End Try
    End Function


    Function Checkduplicate(BarCode As String) As String Implements IBPMMobile.Checkduplicate
        Try
            Dim ret As String = "FALSE"
            Dim ds As New DataSet()
            Using con = New SqlConnection(conStr)
                Using da = New SqlDataAdapter("CheckDuplicateBarCode", con)
                    da.SelectCommand.CommandType = CommandType.StoredProcedure
                    da.SelectCommand.Parameters.AddWithValue("@BarCode", BarCode)
                    da.Fill(ds)
                End Using
            End Using
            If (ds.Tables(0).Rows.Count > 0) Then
                ret = ds.Tables(0).Rows(0).Item("Count")
            End If
            Return ret
        Catch ex As Exception
            Return "FALSE"
        End Try
    End Function

    Public Function Base64DecodedImageName(ByVal base64EncodedString As String, ByVal filePath As String) As String Implements IBPMMobile.Base64DecodedImageName
        Dim fileName As String = ""
        Dim imgName As String = ""
        Try
            imgName = System.DateTime.Now.Day & System.DateTime.Now.Year & System.DateTime.Now.Ticks & ".jpeg"
            imgName.Replace("/", "_")
            imgName.Replace(":", "_")
            Dim completeFilePath As String = HostingEnvironment.MapPath("~/" & filePath & "/" & imgName)
            File.WriteAllBytes(completeFilePath, Convert.FromBase64String(base64EncodedString))
            Return imgName
        Catch ex As Exception
            Return "Error Please Try Again"
        End Try

    End Function

End Class

<DataContractAttribute(Namespace:="", Name:="UserDetails")>
Public Class Userdetails
    Public Sub New()   'constructor
        'Console.WriteLine("Object is being created")
        EmailID = "Fail"
        UID = 0
        EmailID = "Fail"
        UserName = "Fail"
        UserRoles = "Fail"
        EID = "Fail"
        Roles = "Fail"
        BPMKEY = "Fail"
    End Sub

    Dim u_UID As Integer
    Dim u_UserName As String
    Dim u_UserRoles As String
    Dim u_EmailID As String
    Dim u_EID As String
    Dim u_Roles As String
    Dim u_BPMKEY As String
    'u_EmailID BPMKEY
    <DataMember(Name:="BPMKEY")>
    Public Property BPMKEY() As String
        Get
            Return Me.u_BPMKEY
        End Get
        Set(ByVal Value As String)
            Me.u_BPMKEY = Value
        End Set
    End Property
    <DataMember(Name:="UID")>
    Public Property UID() As Integer
        Get
            Return Me.u_UID
        End Get
        Set(ByVal Value As Integer)
            Me.u_UID = Value
        End Set
    End Property
    <DataMember(Name:="UserName")>
    Public Property UserName() As String
        Get
            Return Me.u_UserName
        End Get
        Set(ByVal Value As String)
            Me.u_UserName = Value
        End Set
    End Property
    <DataMember(Name:="UserRole")>
    Public Property UserRoles() As String
        Get
            Return Me.u_UserRoles
        End Get
        Set(ByVal Value As String)
            Me.u_UserRoles = Value
        End Set
    End Property
    <DataMember(Name:="EmailID")>
    Public Property EmailID() As String
        Get
            Return Me.u_EmailID
        End Get
        Set(ByVal Value As String)
            Me.u_EmailID = Value
        End Set
    End Property
    <DataMember(Name:="EID")>
    Public Property EID() As String
        Get
            Return Me.u_EID
        End Get
        Set(ByVal Value As String)
            Me.u_EID = Value
        End Set
    End Property
    <DataMember(Name:="Roles")>
    Public Property Roles() As String
        Get
            Return Me.u_Roles
        End Get
        Set(ByVal Value As String)
            Me.u_Roles = Value
        End Set
    End Property
End Class

<DataContractAttribute(Namespace:="", Name:="Menu")>
Public Class UserMenu

    Dim m_ID As Integer
    Dim m_MenuName As String
    Dim m_FormName As String
    Dim m_PID As String
    Dim m_ImageName As String
    'u_EmailID BPMKEY
    <DataMember(Name:="MID")>
    Public Property MID() As String
        Get
            Return Me.m_ID
        End Get
        Set(ByVal Value As String)
            Me.m_ID = Value
        End Set
    End Property
    <DataMember(Name:="MenuName")>
    Public Property MenuName() As String
        Get
            Return Me.m_MenuName
        End Get
        Set(ByVal Value As String)
            Me.m_MenuName = Value
        End Set
    End Property
    <DataMember(Name:="FormName")>
    Public Property FormName() As String
        Get
            Return Me.m_FormName
        End Get
        Set(ByVal Value As String)
            Me.m_FormName = Value
        End Set
    End Property
    <DataMember(Name:="ImageName")>
    Public Property ImageName() As String
        Get
            Return Me.m_ImageName
        End Get
        Set(ByVal Value As String)
            Me.m_ImageName = Value
        End Set
    End Property
    <DataMember(Name:="PMenu")>
    Public Property PMenu() As String
        Get
            Return Me.m_PID
        End Get
        Set(ByVal Value As String)
            Me.m_PID = Value
        End Set
    End Property
End Class

<DataContractAttribute(Namespace:="", Name:="FormData")>
Public Class FormData

    Public Sub New()   'constructor
        'Console.WriteLine("Object is being created")
        'DropDownData = Nothing
    End Sub

    <DataMember(Name:="FieldID")>
    Public Property FieldID As String
    <DataMember(Name:="EID")>
    Public Property EID As String
    <DataMember(Name:="displayName")>
    Public Property displayName As String
    <DataMember(Name:="FieldType")>
    Public Property FieldType As String
    <DataMember(Name:="displayOrder")>
    Public Property displayOrder As String
    <DataMember(Name:="FieldMapping")>
    Public Property FieldMapping As String
    <DataMember(Name:="isRequired")>
    Public Property isRequired As String
    <DataMember(Name:="isActive")>
    Public Property isActive As String
    <DataMember(Name:="dropdown")>
    Public Property dropdown As String
    <DataMember(Name:="DocumentType")>
    Public Property DocumentType As String
    <DataMember(Name:="DBTableName")>
    Public Property DBTableName As String
    <DataMember(Name:="FieldNature")>
    Public Property FieldNature As String
    <DataMember(Name:="DropDownType")>
    Public Property DropDownType As String
    <DataMember(Name:="isEditable")>
    Public Property isEditable As String
    <DataMember(Name:="datatype")>
    Public Property datatype As String
    <DataMember(Name:="MinLen")>
    Public Property MinLen As String
    <DataMember(Name:="MaxLen")>
    Public Property MaxLen As String
    <DataMember(Name:="isWorkFlow")>
    Public Property isWorkFlow As String
    <DataMember(Name:="lookupvalue")>
    Public Property lookupvalue As String
    <DataMember(Name:="Cal_Fields")>
    Public Property Cal_Fields As String
    <DataMember(Name:="AutoFilter")>
    Public Property AutoFilter As String
    <DataMember(Name:="KC_VALUE")>
    Public Property KC_VALUE As String
    <DataMember(Name:="KC_LOGIC")>
    Public Property KC_LOGIC As String
    <DataMember(Name:="isunique")>
    Public Property isunique As String
    <DataMember(Name:="ShowOnDocDetail")>
    Public Property ShowOnDocDetail As String
    <DataMember(Name:="IsSession")>
    Public Property IsSession As String
    <DataMember(Name:="isMail")>
    Public Property isMail As String
    <DataMember(Name:="KC_STATUS")>
    Public Property KC_STATUS As String
    <DataMember(Name:="ShowOnAmendment")>
    Public Property ShowOnAmendment As String
    <DataMember(Name:="isSearch")>
    Public Property isSearch As String
    <DataMember(Name:="isImieno")>
    Public Property isImieno As String
    <DataMember(Name:="isPhoneNo")>
    Public Property isPhoneNo As String
    <DataMember(Name:="iseditonAmend")>
    Public Property iseditonAmend As String
    <DataMember(Name:="InLineEditing")>
    Public Property InLineEditing As String
    <DataMember(Name:="dependentON")>
    Public Property dependentON As String
    <DataMember(Name:="Invisible")>
    Public Property Invisible As String
    <DataMember(Name:="Cal_Text")>
    Public Property Cal_Text As String
    <DataMember(Name:="DefaultValue")>
    Public Property DefaultValue As String
    <DataMember(Name:="FixedValuedFilter")>
    Public Property FixedValuedFilter As String
    <DataMember(Name:="ShowOnDashBoard")>
    Public Property ShowOnDashBoard As String
    <DataMember(Name:="DDllookupvalue")>
    Public Property DDllookupvalue As String
    <DataMember(Name:="externallookupformobile")>
    Public Property externallookupformobile As String
    <DataMember(Name:="IsTotal")>
    Public Property IsTotal As String

    <DataMember(Name:="iscatchment")>
    Public Property iscatchment As String

    <DataMember(Name:="alloweditonedit")>
    Public Property alloweditonedit As String

    <DataMember(Name:="ChildTemp_column")>
    Public Property ChildTemp_column As String

    <DataMember(Name:="ddlval")>
    Public Property ddlval As String

    <DataMember(Name:="DDLlookupValueSource")>
    Public Property DDLlookupValueSource As String

    <DataMember(Name:="geofencetype")>
    Public Property geofencetype As String

    <DataMember(Name:="MultiLookUpVal")>
    Public Property MultiLookUpVal As String

    <DataMember(Name:="Child_Specific_text")>
    Public Property Child_Specific_text As String

    <DataMember(Name:="IsCardNo")>
    Public Property IsCardNo As String

    <DataMember(Name:="ddlMultilookupval")>
    Public Property ddlMultilookupval As String

    <DataMember(Name:="isgeofence_filter")>
    Public Property isgeofence_filter As String

    <DataMember(Name:="geofensefiltersourcefield")>
    Public Property geofensefiltersourcefield As String

    <DataMember(Name:="Show_on_CRM")>
    Public Property Show_on_CRM As String

    <DataMember(Name:="Edit_on_CRM")>
    Public Property Edit_on_CRM As String

    'isgeofence_filter geofensefiltersourcefield

End Class

<DataContractAttribute(Namespace:="", Name:="Data")>
Public Class Data
    <DataMember(Name:="TextField", Order:=1)>
    Public Property TextField As String
    <DataMember(Name:="ValueField", Order:=2)>
    Public Property ValueField As String
End Class

<DataContractAttribute(Namespace:="", Name:="DropDownData")>
Public Class DropDownData
    <DataMember(Name:="ControlID", Order:=1)>
    Public Property ControlID As String
    <DataMember(Name:="Controltype", Order:=2)>
    Public Property Controltype As String
    <DataMember(Name:="Item", Order:=3)>
    Public Property Item As String
End Class

<DataContractAttribute(Namespace:="", Name:="ChilddefaultValue")>
Public Class ChildItemDefaultValue
    <DataMember(Name:="DocumentType", Order:=1)>
    Public Property DocumentType As String

    <DataMember(Name:="fld1", Order:=2)>
    Public Property fld1 As String

    <DataMember(Name:="fld2", Order:=3)>
    Public Property fld2 As String

    <DataMember(Name:="fld3", Order:=4)>
    Public Property fld3 As String

    <DataMember(Name:="fld4", Order:=5)>
    Public Property fld4 As String

    <DataMember(Name:="fld5", Order:=6)>
    Public Property fld5 As String

    <DataMember(Name:="fld6", Order:=7)>
    Public Property fld6 As String

    <DataMember(Name:="fld7", Order:=8)>
    Public Property fld7 As String

    <DataMember(Name:="fld8", Order:=10)>
    Public Property fld8 As String

    <DataMember(Name:="fld9", Order:=11)>
    Public Property fld9 As String

    <DataMember(Name:="fld10", Order:=12)>
    Public Property fld10 As String

    <DataMember(Name:="fld11", Order:=13)>
    Public Property fld11 As String

    <DataMember(Name:="fld12", Order:=14)>
    Public Property fld12 As String

    <DataMember(Name:="fld13", Order:=15)>
    Public Property fld13 As String

    <DataMember(Name:="fld14", Order:=16)>
    Public Property fld14 As String

    <DataMember(Name:="fld15", Order:=17)>
    Public Property fld15 As String

    <DataMember(Name:="fld16", Order:=18)>
    Public Property fld16 As String

    <DataMember(Name:="fld17", Order:=19)>
    Public Property fld17 As String

    <DataMember(Name:="fld18", Order:=20)>
    Public Property fld18 As String

    <DataMember(Name:="fld19", Order:=21)>
    Public Property fld19 As String

    <DataMember(Name:="fld20", Order:=22)>
    Public Property fld20 As String

    <DataMember(Name:="fld21", Order:=22)>
    Public Property fld21 As String

    <DataMember(Name:="fld22", Order:=23)>
    Public Property fld22 As String

    <DataMember(Name:="fld23", Order:=24)>
    Public Property fld23 As String

    <DataMember(Name:="fld24", Order:=25)>
    Public Property fld24 As String

    <DataMember(Name:="fld25", Order:=26)>
    Public Property fld25 As String

    <DataMember(Name:="fld26", Order:=27)>
    Public Property fld26 As String

    <DataMember(Name:="fld27", Order:=28)>
    Public Property fld27 As String

    <DataMember(Name:="fld28", Order:=29)>
    Public Property fld28 As String

    <DataMember(Name:="fld29", Order:=30)>
    Public Property fld29 As String

    <DataMember(Name:="fld30", Order:=31)>
    Public Property fld30 As String

    <DataMember(Name:="fld31", Order:=32)>
    Public Property fld31 As String

    <DataMember(Name:="fld32", Order:=33)>
    Public Property fld32 As String

    <DataMember(Name:="fld33", Order:=34)>
    Public Property fld33 As String

    <DataMember(Name:="fld34", Order:=35)>
    Public Property fld34 As String

    <DataMember(Name:="fld35", Order:=36)>
    Public Property fld35 As String

    <DataMember(Name:="fld36", Order:=37)>
    Public Property fld36 As String

    <DataMember(Name:="fld37", Order:=38)>
    Public Property fld37 As String

    <DataMember(Name:="fld38", Order:=39)>
    Public Property fld38 As String

    <DataMember(Name:="fld39", Order:=40)>
    Public Property fld39 As String

    <DataMember(Name:="fld40", Order:=41)>
    Public Property fld40 As String

    <DataMember(Name:="fld41", Order:=42)>
    Public Property fld41 As String

    <DataMember(Name:="fld42", Order:=43)>
    Public Property fld42 As String

    <DataMember(Name:="fld43", Order:=44)>
    Public Property fld43 As String

    <DataMember(Name:="fld44", Order:=45)>
    Public Property fld44 As String

    <DataMember(Name:="fld45", Order:=46)>
    Public Property fld45 As String

    <DataMember(Name:="fld46", Order:=47)>
    Public Property fld46 As String

    <DataMember(Name:="fld47", Order:=48)>
    Public Property fld47 As String

    <DataMember(Name:="fld48", Order:=49)>
    Public Property fld48 As String

    <DataMember(Name:="fld49", Order:=50)>
    Public Property fld49 As String

    <DataMember(Name:="fld50", Order:=51)>
    Public Property fld50 As String

End Class