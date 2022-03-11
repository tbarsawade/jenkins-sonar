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
Public Class ControlUtils


#Region "Dropdown"
    Public Shared Function GetDropDownData(dr As DataRow, dtFields As DataTable, IsEdit As Integer, uBo As Userdetails) As DataTable
        Try
            Dim ddlType = ""
            Dim ds As New DataSet()
            ddlType = Convert.ToString(dr.Item("Dropdowntype"))
            Select Case ddlType.ToUpper.Trim
                Case "FIX VALUED"
                    ds = FixedValued(dr)
                Case "MASTER VALUED"
                    ds = MasterValuedDropdown(dr, dtFields, IsEdit)
                Case "SESSION VALUED"
                    ds = SessionValued(uBo)
                Case "CHILD VALUED"
                    ds = SessionValued(uBo)
            End Select
            Return ds.Tables(0)
        Catch ex As Exception
            Throw
        End Try
    End Function

    Public Shared Function MasterValuedDropdown(dr As DataRow, dtFields As DataTable, IsEdit As Integer) As DataSet
        Dim ds As New DataSet()
        Try
            'Variable Declaration 
            Dim EID As Integer = Convert.ToInt32(dr.Item("EID"))
            Dim DocumentType As String = Convert.ToString(dr.Item("DocumentType"))
            Dim FieldMapping = Convert.ToString(dr.Item("initialFilter"))

            Dim InitialFilter As String = Convert.ToString(dr.Item("initialFilter"))
            Dim defaultfieldval = Convert.ToString(dr.Item("defaultfieldval"))
            Dim defaultVal = ""
            Dim TID As String = "TID"
            Dim TABLENAME As String = "MMM_MST_MASTER"
            Dim arrDocType = dr.Item("DropDown").ToString().Split("-")
            'Field that will be displayed in dropdown
            Dim fldToBeBind As String = ""
            'Document that will be binded in dropdown
            Dim DocToBeBind As String = ""
            fldToBeBind = arrDocType(2).Trim()
            DocToBeBind = arrDocType(1).Trim()
            Dim strQuery As New StringBuilder()
            Dim where As New StringBuilder(" where 1=1")
            Dim objDyn As New DynamicForm()
            'finding table name of dropdown and their respecctive pkey
            Select Case arrDocType(0).ToString().Trim
                Case "MASTER"
                    TABLENAME = "MMM_MST_MASTER"
                    where.Append(" AND ").Append("M.DocumentType='" & DocToBeBind & "' AND M.EID=" & EID)
                Case "DOCUMENT"
                    TABLENAME = "MMM_MST_DOC"
                    where.Append(" AND ").Append("M.DocumentType='" & DocToBeBind & "' AND M.EID=" & EID)
                Case "CHILD"
                    TABLENAME = "MMM_MST_DOC_ITEM"
                    where.Append(" AND ").Append("M.DocumentType='" & DocToBeBind & "'")
                Case "SESSION"
                    TABLENAME = "MMM_MST_USER"
                    TID = "UID"
                    where.Append(" AND M.EID=" & EID)
                Case "STATIC"
                    If arrDocType(1).ToString.ToUpper = "USER" Then
                        TABLENAME = "MMM_MST_USER"
                        TID = "UID"
                        where.Append(" AND M.EID=" & EID)
                    ElseIf arrDocType(1).ToString().ToUpper = "LOCATION" Then
                        TABLENAME = "MMM_MST_LOCATION"
                        TID = "LOCID"
                        where.Append(" AND M.EID=" & EID)
                    End If
            End Select
            'Generiting Query for binding dropdown
            strQuery.Append("select " & fldToBeBind & " Text," & TID & "[tid]  from " & TABLENAME & " M ").Append("{where}").Append("{orderby}")
            ''FILTER THE DATA ACCORDING TO USER 
            Dim tids As String = objDyn.UserDataFilter(DocumentType, arrDocType(1))
            If tids.Length >= 2 Then
                where.Append(" AND CONVERT(NVARCHAR(10),M.TID) IN (" & tids & ")")
            ElseIf tids = "0" Then
                'not clear needs to be cleared
                where.Append(" AND 1=2")
            End If
            'for pre role data filter
            Dim Rtids As String = objDyn.UserDataFilter_PreRole(arrDocType(1).ToString(), TABLENAME)
            If Rtids <> "" Then
                where.Append(" AND CONVERT(NVARCHAR(10),M.TID) IN (" & Rtids & ")")
            End If
            '' for getting def. value from field master
            If Not String.IsNullOrEmpty(InitialFilter.Trim) Then
                Dim arrInlFltr = InitialFilter.Trim.Split(":")
                Dim row() As DataRow = dtFields.Select("fieldid=" & arrInlFltr(0).ToString())
                If row.Length > 0 Then
                    where.Append(" AND " & arrInlFltr(1).ToString() & "='" & defaultfieldval & "'")
                End If
                If (arrDocType(0).ToUpper() = "STATIC") Then
                    If row.Length > 0 Then
                        ' to be used for apm user bind from role assignment also 12_sep_14

                        Dim Qry = strQuery.ToString.ToString.Replace("{orderby}", "")
                        strQuery = New StringBuilder()
                        strQuery.Append(Qry)
                        strQuery.Append(" union Select  " & fldToBeBind & " Text,convert(nvarchar(10)," & TID & ")  [tid]" & "FROM " & TABLENAME & " where convert(nvarchar(10)," & TID & ") in (select uid from MMM_Ref_Role_User where eid=" & EID & " and rolename='" & row(0).Item("defaultFieldVal").ToString & "') order by " & fldToBeBind)
                    End If
                End If
            End If
            If (IsEdit = False) Then
                where.Append(" and M.IsAuth=1")
            End If
            Dim AutoFilter As String = Convert.ToString(dr.Item("AutoFilter"))
            If AutoFilter.Length > 0 Then
                If arrDocType(0).ToUpper() = "CHILD" Then
                    strQuery = New StringBuilder()
                    If AutoFilter.ToUpper = "DOCID" Then
                        strQuery.Append(GetQuery1(arrDocType(1).ToString, arrDocType(2).ToString(), EID))
                    Else
                        strQuery.Append(GetQuery(arrDocType(1).ToString, arrDocType(2).ToString(), EID))
                    End If
                End If
            End If
            Dim InitFilterArr As String() = dr.Item("InitialFilter").ToString().Split(":")
            Dim SessionFieldvalue As String = Convert.ToString(dr.Item("SessionFieldVal"))
            If SessionFieldvalue.Length > 0 Then
                Dim val As String() = SessionFieldvalue.ToString().Split("-")
                If arrDocType(0).ToUpper() <> "STATIC" Then
                    Dim QR = ""
                    QR = "select isnull(" & val(0) & ",0) from mmm_mst_user where eid=" & EID & " and uid=" & HttpContext.Current.Session("UID")
                    Dim Conval As String = Replace(Convert.ToString(DataLibMaster.ExecuteScalar(CommandType.Text, QR, Nothing)), ",", "','")
                    If Conval.Length > 1 Then
                        If SessionFieldvalue.Contains("-") Then
                            where.Append(" and " & val(2) & " in('" & Conval & "')")
                        Else
                            where.Append("   and tid in ('" & Conval & "') ")
                        End If
                    End If
                End If
            End If
            Dim orderby = " order by " & fldToBeBind
            Dim Query As String = strQuery.ToString.Replace("{where}", where.ToString).Replace("{orderby}", orderby)
            ds = DataLibMaster.ExecuteDataSet(CommandType.Text, Query)
            Return ds
        Catch ex As Exception
            Throw New Exception("Dropdown.BindDropDown")
        End Try
    End Function

    Public Shared Function SessionValued(uBo As Userdetails) As DataSet
        Dim ds As New DataSet()
        Try
            Dim dt As New DataTable()
            dt.Columns.Add("tid")
            dt.Columns.Add("Text")
            Dim drA As DataRow
            drA = dt.NewRow()
            drA.Item("tid") = uBo.UID
            drA.Item("Text") = uBo.UserName
            dt.Rows.Add(drA)
            ds.Tables.Add(dt)
            Return ds
        Catch ex As Exception
            Throw (New Exception("Dropdown.SessionValued" & ex.Message))
        End Try
    End Function
    Public Shared Function FixedValued(dr As DataRow) As DataSet
        Dim arrValue = dr.Item("DropDown").ToString().Split(",")
        Dim ds As New DataSet()
        Try
            Dim dt As New DataTable()
            dt.Columns.Add("tid")
            dt.Columns.Add("Text")
            Dim drA As DataRow
            For i As Integer = 0 To arrValue.Length - 1
                If (arrValue(i).Trim <> "") Then
                    drA = dt.NewRow()
                    drA.Item("tid") = arrValue(i).ToUpper.Trim.ToUpper
                    drA.Item("Text") = arrValue(i).ToUpper.Trim.ToUpper
                    dt.Rows.Add(drA)
                End If
            Next
            ds.Tables.Add(dt)
            Return ds
        Catch ex As Exception
            Throw (New Exception("Dropdown.FixedValued" & ex.Message))
        End Try
    End Function
    Public Shared Function GetQuery(ByVal doctype As String, ByVal fld As String, EID As Integer) As String
        Try
            Dim parameters() As SqlParameter = New SqlParameter() _
       {
         New SqlParameter("@doctype", doctype),
         New SqlParameter("@eid", EID),
         New SqlParameter("@fldmapping", fld)
       }
            Return DataLibMaster.ExecuteScalar(CommandType.StoredProcedure, "usp_GetMasterValued_MVC", parameters)
        Catch ex As Exception
            Throw
        End Try
    End Function

    Public Shared Function GetQuery1(ByVal doctype As String, ByVal fld As String, EID As String) As String
        Try
            Dim parameters() As SqlParameter = New SqlParameter() _
      {
        New SqlParameter("@doctype", doctype),
        New SqlParameter("@eid", EID),
        New SqlParameter("@fldmapping", fld)
      }
            Return DataLibMaster.ExecuteScalar(CommandType.StoredProcedure, "usp_GetMasterValued1_MVC", parameters)
        Catch ex As Exception
            Throw
        End Try
    End Function
#End Region
#Region "AutoComplete"
    Public Shared Function GetAutoCompleteData(dr As DataRow, dtFields As DataTable, IsEdit As Integer, uBo As Userdetails, str As String) As DataTable
        Try
            Dim ddlType = ""
            Dim ds As New DataSet()
            ddlType = Convert.ToString(dr.Item("Dropdowntype"))
            Select Case ddlType.ToUpper.Trim
                Case "FIX VALUED"
                    ds = FixedValued(dr)
                Case "MASTER VALUED"
                    ds = MasterValuedAutoComplete(dr, dtFields, IsEdit, str)
                Case "SESSION VALUED"
                    ds = SessionValued(uBo)
                Case "CHILD VALUED"
                    ds = SessionValued(uBo)
            End Select
            Return ds.Tables(0)
        Catch ex As Exception
            Throw
        End Try
    End Function

    Public Shared Function MasterValuedAutoComplete(dr As DataRow, dtFields As DataTable, IsEdit As Integer, str As String) As DataSet
        Dim ds As New DataSet()
        Try
            'Variable Declaration 
            Dim EID As Integer = Convert.ToInt32(dr.Item("EID"))
            Dim DocumentType As String = Convert.ToString(dr.Item("DocumentType"))
            Dim FieldMapping = Convert.ToString(dr.Item("initialFilter"))

            Dim InitialFilter As String = Convert.ToString(dr.Item("initialFilter"))
            Dim defaultfieldval = Convert.ToString(dr.Item("defaultfieldval"))
            Dim defaultVal = ""
            Dim TID As String = "TID"
            Dim TABLENAME As String = "MMM_MST_MASTER"
            Dim arrDocType = dr.Item("DropDown").ToString().Split("-")
            'Field that will be displayed in dropdown
            Dim fldToBeBind As String = ""
            'Document that will be binded in dropdown
            Dim DocToBeBind As String = ""
            fldToBeBind = arrDocType(2).Trim()
            DocToBeBind = arrDocType(1).Trim()
            Dim strQuery As New StringBuilder()
            Dim where As New StringBuilder(" where 1=1")
            Dim objDyn As New DynamicForm()
            'finding table name of dropdown and their respecctive pkey
            Select Case arrDocType(0).ToString().Trim
                Case "MASTER"
                    TABLENAME = "MMM_MST_MASTER"
                    where.Append(" AND ").Append("M.DocumentType='" & DocToBeBind & "' AND M.EID=" & EID)
                Case "DOCUMENT"
                    TABLENAME = "MMM_MST_DOC"
                    where.Append(" AND ").Append("M.DocumentType='" & DocToBeBind & "' AND M.EID=" & EID)
                Case "CHILD"
                    TABLENAME = "MMM_MST_DOC_ITEM"
                    where.Append(" AND ").Append("M.DocumentType='" & DocToBeBind & "'")
                Case "SESSION"
                    TABLENAME = "MMM_MST_USER"
                    TID = "UID"
                    where.Append(" AND M.EID=" & EID)
                Case "STATIC"
                    If arrDocType(1).ToString.ToUpper = "USER" Then
                        TABLENAME = "MMM_MST_USER"
                        TID = "UID"
                        where.Append(" AND M.EID=" & EID)
                    ElseIf arrDocType(1).ToString().ToUpper = "LOCATION" Then
                        TABLENAME = "MMM_MST_LOCATION"
                        TID = "LOCID"
                        where.Append(" AND M.EID=" & EID)
                    End If
            End Select
            where.Append(" AND ").Append(fldToBeBind & " like '%" & str & "%'")
            'Generiting Query for binding dropdown
            strQuery.Append("select " & fldToBeBind & " Text," & TID & "[tid]  from " & TABLENAME & " M ").Append("{where}").Append("{orderby}")
            ''FILTER THE DATA ACCORDING TO USER 
            Dim tids As String = objDyn.UserDataFilter(DocumentType, arrDocType(1))
            If tids.Length >= 2 Then
                where.Append(" AND CONVERT(NVARCHAR(10),M.TID) IN (" & tids & ")")
            ElseIf tids = "0" Then
                'not clear needs to be cleared
                where.Append(" AND 1=2")
            End If
            'for pre role data filter
            Dim Rtids As String = objDyn.UserDataFilter_PreRole(arrDocType(1).ToString(), TABLENAME)
            If Rtids <> "" Then
                where.Append(" AND CONVERT(NVARCHAR(10),M.TID) IN (" & Rtids & ")")
            End If
            '' for getting def. value from field master
            If Not String.IsNullOrEmpty(InitialFilter.Trim) Then
                Dim arrInlFltr = InitialFilter.Trim.Split(":")
                Dim row() As DataRow = dtFields.Select("fieldid=" & arrInlFltr(0).ToString())
                If row.Length > 0 Then
                    where.Append(" AND " & arrInlFltr(1).ToString() & "='" & defaultfieldval & "'")
                End If
                If (arrDocType(0).ToUpper() = "STATIC") Then
                    If row.Length > 0 Then
                        ' to be used for apm user bind from role assignment also 12_sep_14

                        Dim Qry = strQuery.ToString.ToString.Replace("{orderby}", "")
                        strQuery = New StringBuilder()
                        strQuery.Append(Qry)
                        strQuery.Append(" union Select  " & fldToBeBind & " Text,convert(nvarchar(10)," & TID & ")  [tid]" & "FROM " & TABLENAME & " where convert(nvarchar(10)," & TID & ") in (select uid from MMM_Ref_Role_User where eid=" & EID & " and rolename='" & row(0).Item("defaultFieldVal").ToString & "') order by " & fldToBeBind)
                    End If
                End If
            End If
            If (IsEdit = False) Then
                where.Append(" and M.IsAuth=1")
            End If
            Dim AutoFilter As String = Convert.ToString(dr.Item("AutoFilter"))
            If AutoFilter.Length > 0 Then
                If arrDocType(0).ToUpper() = "CHILD" Then
                    strQuery = New StringBuilder()
                    If AutoFilter.ToUpper = "DOCID" Then
                        strQuery.Append(GetQuery1(arrDocType(1).ToString, arrDocType(2).ToString(), EID))
                    Else
                        strQuery.Append(GetQuery(arrDocType(1).ToString, arrDocType(2).ToString(), EID))
                    End If
                End If
            End If
            Dim InitFilterArr As String() = dr.Item("InitialFilter").ToString().Split(":")
            Dim SessionFieldvalue As String = Convert.ToString(dr.Item("SessionFieldVal"))
            If SessionFieldvalue.Length > 0 Then
                Dim val As String() = SessionFieldvalue.ToString().Split("-")
                If arrDocType(0).ToUpper() <> "STATIC" Then
                    Dim QR = ""
                    QR = "select isnull(" & val(0) & ",0) from mmm_mst_user where eid=" & EID & " and uid=" & HttpContext.Current.Session("UID")
                    Dim Conval As String = Replace(Convert.ToString(DataLibMaster.ExecuteScalar(CommandType.Text, QR, Nothing)), ",", "','")
                    If Conval.Length > 1 Then
                        If SessionFieldvalue.Contains("-") Then
                            where.Append(" and " & val(2) & " in('" & Conval & "')")
                        Else
                            where.Append("   and tid in ('" & Conval & "') ")
                        End If
                    End If
                End If
            End If
            Dim orderby = " order by " & fldToBeBind
            Dim Query As String = strQuery.ToString.Replace("{where}", where.ToString).Replace("{orderby}", orderby)
            ds = DataLibMaster.ExecuteDataSet(CommandType.Text, Query)
            Return ds
        Catch ex As Exception
            Throw New Exception("Dropdown.BindDropDown")
        End Try
    End Function
#End Region
#Region "CheckBoxList"
    Public Shared Function GetCheckBoxListData(dr As DataRow, IsEdit As Integer) As DataTable
        Try
            Dim ddlType = ""
            Dim ds As New DataSet()
            ddlType = Convert.ToString(dr.Item("Dropdowntype"))
            Select Case ddlType.ToUpper.Trim
                Case "FIX VALUED"
                    ds = FixedValued(dr)
                Case "MASTER VALUED"
                    ds = MasterValuedCheckBoxList(dr, IsEdit)
            End Select
            Return ds.Tables(0)
        Catch ex As Exception
            Throw
        End Try
    End Function
    Public Shared Function MasterValuedCheckBoxList(dr As DataRow, IsEdit As Integer) As DataSet
        Dim ds As New DataSet()
        Try
            'Variable Declaration 
            Dim EID As Integer = Convert.ToInt32(dr.Item("EID"))
            Dim DocumentType As String = Convert.ToString(dr.Item("DocumentType"))
            Dim TID As String = "TID"
            Dim TABLENAME As String = "MMM_MST_MASTER"
            Dim arrDocType = dr.Item("DropDown").ToString().Split("-")
            'Field that will be displayed in dropdown
            Dim fldToBeBind As String = ""
            'Document that will be binded in dropdown
            Dim DocToBeBind As String = ""
            fldToBeBind = arrDocType(2).Trim()
            DocToBeBind = arrDocType(1).Trim()
            Dim strQuery As New StringBuilder()
            Dim where As New StringBuilder(" where 1=1")
            Dim objDyn As New DynamicForm()
            'finding table name of dropdown and their respecctive pkey
            Select Case arrDocType(0).ToString().Trim
                Case "MASTER"
                    TABLENAME = "MMM_MST_MASTER"
                    where.Append(" AND ").Append("M.DocumentType='" & DocToBeBind & "' AND M.EID=" & EID)
                Case "DOCUMENT"
                    TABLENAME = "MMM_MST_DOC"
                    where.Append(" AND ").Append("M.DocumentType='" & DocToBeBind & "' AND M.EID=" & EID)
                Case "CHILD"
                    TABLENAME = "MMM_MST_DOC_ITEM"
                    where.Append(" AND ").Append("M.DocumentType='" & DocToBeBind & "'")
                Case "SESSION"
                    TABLENAME = "MMM_MST_USER"
                    TID = "UID"
                    where.Append(" AND M.EID=" & EID)
                Case "STATIC"
                    If arrDocType(1).ToString.ToUpper = "USER" Then
                        TABLENAME = "MMM_MST_USER"
                        TID = "UID"
                        where.Append(" AND M.EID=" & EID)
                    ElseIf arrDocType(1).ToString().ToUpper = "LOCATION" Then
                        TABLENAME = "MMM_MST_LOCATION"
                        TID = "LOCID"
                        where.Append(" AND M.EID=" & EID)
                    End If
            End Select
            'Generiting Query for binding dropdown
            strQuery.Append("select " & fldToBeBind & " Text," & TID & "[tid]  from " & TABLENAME & " M ").Append("{where}").Append("{orderby}")
            ''FILTER THE DATA ACCORDING TO USER 
            Dim tids As String = objDyn.UserDataFilter(DocumentType, arrDocType(1))
            '' for getting def. value from field master

            If (IsEdit = False) Then
                where.Append(" and M.IsAuth=1")
            End If
            Dim orderby = " order by " & fldToBeBind
            Dim Query As String = strQuery.ToString.Replace("{where}", where.ToString).Replace("{orderby}", orderby)
            ds = DataLibMaster.ExecuteDataSet(CommandType.Text, Query)
            Return ds
        Catch ex As Exception
            Throw New Exception("Dropdown.BindCheckBoxList()")
        End Try
    End Function
#End Region
#Region "dropdownFilter"
    Function DropDownFilter(EID As String, FieldID As Integer, DOCID As Integer) As ClsDropDownFilterdata
        Dim Result As New ClsDropDownFilterdata()
        Try
            Dim dsFields As New DataSet()
            Dim DropDown = "", LookupValue = ""
            Dim lookupStr = ""
            'Getting field defination of current dropdown
            dsFields = CommanUtil.GetFormFields(EID:=EID, FieldID:=FieldID)
            If dsFields.Tables("fields").Rows.Count > 0 Then
                DropDown = Convert.ToString(dsFields.Tables("fields").Rows(0).Item("DropDown"))
                LookupValue = Convert.ToString(dsFields.Tables("fields").Rows(0).Item("LookupValue"))
                'removing last comma from the string
                LookupValue = LookupValue.TrimEnd(",")
                Dim LstDDL As New List(Of ClsDdlDataLst)
                'lookup values will not be null or emplty for the case of dropdownfilter and lookup values
                If (Not String.IsNullOrEmpty(DropDown.Trim)) And (Not String.IsNullOrEmpty(LookupValue.Trim)) Then
                    Dim arrlookUp As String() = LookupValue.Split(",")
                    'arrlookUp.Count > 0 Indicats that either lookup or dropdown filter has been configured
                    If arrlookUp.Count > 0 Then
                        'Handling lookup values Note:For lookup:lookupvalue string will defnetly contains string pattern like %-fld%
                        Dim sbColumns As New StringBuilder()
                        Dim DDLList As New List(Of ClsDdlDataLst)
                        For i As Integer = 0 To arrlookUp.Length - 1
                            Dim arrP = arrlookUp(i).Split("-")
                            'For avoiding situation when both lookup and dropdown filter has been configured on same dropdown
                            If (arrP(1).Trim.ToUpper <> "S" And arrP(1).Trim.ToUpper <> "R" And arrP(1).Trim.ToUpper <> "C" And Not String.IsNullOrEmpty(arrlookUp(i).ToString.Trim)) Then
                                'Appending fieldmapping to a string for generating query
                                sbColumns.Append("M." & arrP(1)).Append(" AS '").Append(arrP(0)).Append("' , ")
                            Else
                                Dim obj As New ClsDdlDataLst()
                                obj.FieldID = arrP(0).Trim()
                                obj.Data = GetDropDownData(dsFields.Tables("fields").Rows(0), DOCID, arrlookUp(i), obj)
                                DDLList.Add(obj)
                            End If
                        Next
                        'Now finding table name tid etc of current dropdown
                        If sbColumns.ToString <> "" Then
                            Dim TABLENAME As String = "MMM_MST_MASTER"
                            Dim TID As String = "tid"
                            Dim arrDocType = dsFields.Tables(0).Rows(0).Item("DropDown").ToString().Split("-")
                            'Field that will be displayed in dropdown
                            Dim fldToBeBind As String = ""
                            'Document that will be binded in dropdown
                            Dim DocToBeBind As String = ""
                            fldToBeBind = arrDocType(2).Trim()
                            DocToBeBind = arrDocType(1).Trim()
                            Dim strQuery As New StringBuilder()
                            Dim where As New StringBuilder(" where 1=1")
                            'finding table name of dropdown and their respecctive pkey
                            Select Case arrDocType(0).ToString().Trim
                                Case "MASTER"
                                    TABLENAME = "MMM_MST_MASTER"
                                    where.Append(" AND ").Append("M.DocumentType='" & DocToBeBind & "' AND M.EID=" & EID)
                                Case "DOCUMENT"
                                    TABLENAME = "MMM_MST_DOC"
                                    where.Append(" AND ").Append("M.DocumentType='" & DocToBeBind & "' AND M.EID=" & EID)
                                Case "CHILD"
                                    TABLENAME = "MMM_MST_DOC_ITEM"
                                    where.Append(" AND ").Append("M.DocumentType='" & DocToBeBind & "'")
                                Case "SESSION"
                                    TABLENAME = "MMM_MST_USER"
                                    TID = "UID"
                                    where.Append(" AND M.EID=" & EID)
                                Case "STATIC"
                                    If arrDocType(1).ToString.ToUpper = "USER" Then
                                        TABLENAME = "MMM_MST_USER"
                                        TID = "UID"
                                        where.Append(" AND M.EID=" & EID)
                                    ElseIf arrDocType(1).ToString().ToUpper = "LOCATION" Then
                                        TABLENAME = "MMM_MST_LOCATION"
                                        TID = "LOCID"
                                        where.Append(" AND M.EID=" & EID)
                                    End If
                            End Select
                            where.Append(" AND M." & TID & " = " & DOCID)
                            'Generiting Query for binding dropdown
                            strQuery.Append("select " & sbColumns.ToString.TrimEnd(",", " ") & "  from " & TABLENAME & " M ").Append(where)
                            'Getting it's value from database
                            Dim dsL As New DataSet()
                            dsL = DataLibMaster.ExecuteDataSet(CommandType.Text, strQuery.ToString.Trim)
                            lookupStr = DataLibMaster.JsonTableSerializer(dsL.Tables(0))
                        End If

                        Result.lookupvalue = lookupStr
                        Result.ddldata = DDLList
                    End If
                End If
            End If

        Catch


        End Try

        Return Result
    End Function

    Public Function GetDropDownData(DR As DataRow, DocID As String, Type As String, ByRef objDD As ClsDdlDataLst) As List(Of CLSDDLData)
        Dim fldPair = Type.Split("-")

        Dim ds As New DataSet()
        Try
            'oda = New SqlDataAdapter("SELECT * FROM MMM_MST_FIELDS WHERE FIELDID=" & fldPair(0) & "", con) 
            Dim dtFields As New DataTable()
            Dim dsF = DataLibMaster.ExecuteDataSet(CommandType.Text, "SELECT * FROM MMM_MST_FIELDS WHERE FIELDID=" & fldPair(0) & "")
            DR = dsF.Tables(0).Rows(0)
            objDD.FieldType = DR.Item("FieldType").ToString.ToUpper()
            Dim documenttype() As String = DR.Item("dropdown").ToString.Split("-")
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
                Dim DOCTYPE() As String = DR.Item("DROPDOWN").ToString.Split("-")
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
                Dim DROPDOWN1 As String = DR.Item("AUTOFILTER").ToString()
                Dim P As SqlParameter() = New SqlParameter() {
                    New SqlParameter("@EID", DR.Item("EID").ToString()),
                    New SqlParameter("@TAB1", TAB1),
                    New SqlParameter("@TAB2", TAB2),
                    New SqlParameter("@DOCUMENTTYPE", DOCTYPE(1).ToString),
                    New SqlParameter("@FLDMAPPING", DOCTYPE(2).ToString),
                    New SqlParameter("@AUTOFILTER", DR.Item("AUTOFILTER").ToString()),
                    New SqlParameter("@TID", TID),
                    New SqlParameter("@STID", STID),
                    New SqlParameter("@VAL", DocID)
                }
                ds = DataLibMaster.ExecuteDataSet(CommandType.StoredProcedure, "USP_GETMANNUALFILTER", P)
            Else
                Dim DROPDOWN As String() = DR.Item("DROPDOWN").ToString.Split("-")  ' this contains to be filled values 
                Dim TABLENAME As String = ""
                Dim TID As String = "TID"
                If UCase(DROPDOWN(0).ToString()) = "MASTER" Then
                    TABLENAME = "MMM_MST_MASTER"
                ElseIf UCase(DROPDOWN(0).ToString()) = "DOCUMENT" Then
                    TABLENAME = "MMM_MST_DOC"
                ElseIf UCase(DROPDOWN(0).ToString()) = "CHILD" Then
                    TABLENAME = "MMM_MST_DOC_ITEM"
                ElseIf UCase(DROPDOWN(0).ToString()) = "STATIC" Then
                    If UCase(DROPDOWN(1).ToString()) = "USER" Then
                        TABLENAME = "MMM_MST_USER"
                        TID = "UID"
                    ElseIf UCase(DROPDOWN(1).ToString()) = "LOCATION" Then
                        TABLENAME = "MMM_MST_LOCATION"
                        If DROPDOWN(2).ToString.ToUpper = "LOCATIONSTATE" Then
                            TID = "SID"
                        Else
                            TID = "LOCID"
                        End If
                    Else
                        TABLENAME = DR.Item("DBTABLENAME").ToString
                    End If
                ElseIf UCase(DROPDOWN(0).ToString()) = "SESSION" Then
                    TABLENAME = "MMM_MST_USER"
                    TID = "UID"
                End If
                Dim AUTOFILTER As String = DR.Item("AUTOFILTER").ToString()
                Dim tids As String = ""

                ''Filter Data according to Userid
                Dim objD As New DynamicForm()
                tids = objD.UserDataFilter(DR.Item("documenttype").ToString(), DROPDOWN(1).ToString())
                Dim xwhr = ""
                If tids.Length > 2 Then
                    xwhr = " AND CONVERT(NVARCHAR(10),TID) IN (" & tids & ")"
                Else
                    xwhr = ""
                End If
                Dim ChildDoctype As String = ""

                'If ds.Tables("data").Rows(0).Item("dropdowntype") = "CHILD VALUED" Then
                '    Dim ChildMid() As String = documenttype(1).ToString.Split(":")
                '    ChildDoctype = ChildMid(1).ToString
                'End If
                Dim P11 As SqlParameter() = New SqlParameter() {
                    New SqlParameter("@EID", DR.Item("EID").ToString),
                    New SqlParameter("@TableName", TABLENAME),
                    New SqlParameter("@Val", DocID),
                    New SqlParameter("@xwhr", xwhr),
                    New SqlParameter("@TID", TID),
                    New SqlParameter("@documenttype", DROPDOWN(1)),
                     New SqlParameter("@fldmapping", DROPDOWN(2)),
                    New SqlParameter("@autofilter", AUTOFILTER)
                }
                ds = DataLibMaster.ExecuteDataSet(CommandType.StoredProcedure, "USP_BINDDDL", P11)
            End If
            'AJeet
            Dim Lst As New List(Of CLSDDLData)
            Dim obj As CLSDDLData
            If (ds.Tables(0).Rows.Count > 0) Then
                For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                    obj = New CLSDDLData()
                    obj.Text = ds.Tables(0).Rows(i).Item(0)
                    obj.tid = ds.Tables(0).Rows(i).Item("tID")
                    Lst.Add(obj)
                Next
            End If
            Return Lst
        Catch ex As Exception

        End Try

    End Function
#End Region
End Class


<DataContractAttribute(Name:="ClsDropDownFilterdata")>
Public Class ClsDropDownFilterdata
    Public Property lookupvalue As String
    Public Property ddldata As List(Of ClsDdlDataLst)
End Class

Public Class ClsDdlDataLst
    Public Property FieldID As Integer
    Public Property FieldType As String
    Public Property Data As List(Of CLSDDLData)
End Class
Public Class CLSDDLData
    Public Property Text As String
    Public Property tid As String
End Class

