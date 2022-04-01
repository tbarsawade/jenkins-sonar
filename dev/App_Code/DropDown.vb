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

Public Class DropDown

    Public Shared Function GetDropDownID(DRDDL As DataRow, EID As Integer, DDlText As String, obj As LineitemWrap) As String
        Dim StrResult = "0"
        DDlText = DDlText.Replace("'", "''")
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
        Dim InitialFilter = Convert.ToString(DRDDL.Item("initialFilter"))
        Dim defaultfieldval = Convert.ToString(DRDDL.Item("defaultfieldval"))
        Dim defaultVal = ""

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
                strQuery = strQuery & " and M." & arr(arr.Length - 1) & " = '" & DDlText & "'"
                strQuery = strQuery & "  AND M.isauth=1 AND EID= " & EID
                'For initial Filter
                If Not String.IsNullOrEmpty(InitialFilter.Trim) Then
                    Dim arr1 = InitialFilter.Trim.Split(":")
                    Dim qry = "select defaultfieldval FROM MMM_MST_FIELDS WHERE FieldID=" & arr1(0) & ""
                    Using con1 = New SqlConnection(conStr)
                        Using da As New SqlDataAdapter(qry, con1)
                            da.Fill(DSIntl)
                        End Using
                    End Using
                    strQuery = strQuery & " AND M." & arr1(1) & " ='" & DSIntl.Tables(0).Rows(0).Item("defaultfieldval") & "'"
                End If

            ElseIf TABLENAME = "MMM_MST_DOC_ITEM" Then
                Dim AutoFilter As String = DRDDL.Item("AutoFilter").ToString()
                If AutoFilter.ToUpper = "DOCID" Then
                    strQuery = CommanUtil.GetQuery1(arr(1).ToString, arr(2).ToString(), EID, DDlText)
                Else
                    strQuery = CommanUtil.GetQuery(arr(1).ToString, arr(2).ToString, EID, DDlText)
                End If

            ElseIf TABLENAME = "MMM_MST_USER" Then
                If DRDDL.Item("dropdowntype").ToString.ToUpper = "SESSION VALUED" Then
                    'For mobile they will supply userID
                    If DDlText.Contains("[]") Then
                        Dim UserID = ""
                        Dim arrU = Split(DDlText, "[]")
                        UserID = arrU(1).Trim()
                        strQuery = "select " & "UserID" & "," & TID & " from " & TABLENAME & " M WHERE EID=" & EID & " " & ""
                        strQuery = strQuery & " and " & "UserID" & " = '" & UserID & "'"
                    Else
                        strQuery = "select " & arr(2).ToString() & "," & TID & " from " & TABLENAME & " M WHERE EID=" & EID & " " & ""
                        strQuery = strQuery & " and " & "USERNAME" & " = '" & DDlText & "'"
                    End If

                Else
                    strQuery = "select " & arr(2).ToString() & "," & TID & " from " & TABLENAME & " M WHERE EID=" & EID & " " & ""
                    strQuery = strQuery & " and " & arr(2).ToString() & " = '" & DDlText & "'"
                    'For initial Filter
                End If
                If Not String.IsNullOrEmpty(InitialFilter.Trim) Then
                    Dim arr1 = InitialFilter.Trim.Split(":")
                    Dim qry = "select defaultfieldval FROM MMM_MST_FIELDS WHERE FieldID=" & arr1(0) & ""
                    Using con1 = New SqlConnection(conStr)
                        Using da As New SqlDataAdapter(qry, con1)
                            da.Fill(DSIntl)
                        End Using
                    End Using
                    strQuery = strQuery & " AND M." & arr1(1) & " =" & DSIntl.Tables(0).Rows(0).Item("defaultfieldval")
                End If
            ElseIf TABLENAME = "MMM_MST_LOCATION" Then
                strQuery = "select DISTINCT " & arr(2).ToString() & ",SID [tid]  from " & TABLENAME & " M "
                strQuery = strQuery & " and M." & arr(arr.Length - 1) & " = '" & DDlText & "'"
            End If
            'Query For drop Down Filter
            Dim AutoFilter1 As String = DRDDL.Item("AutoFilter").ToString()
            con = New SqlConnection(conStr)
            con.Open()
            Dim FilterOnFld = "", FilterOnValue = ""
            Dim FilterType = "S"

            ''now enabled again
            '' below line till line no. 180 is disabled temporarily for issue troubleshoot paytm pr upload - 
            If AutoFilter1.Length > 0 And TABLENAME <> "MMM_MST_DOC_ITEM" Then
                FilterOnFld = AutoFilter1
                Dim str = "select * FROM MMM_MST_FIelds where EID=" & EID & " AND ISActive=1 AND Documenttype='" & DRDDL.Item("DocumentType") & "' AND  lookUpValue like '%" & DRDDL.Item("FieldID") & "%' "
                sda = New SqlDataAdapter(str, con)
                sda.Fill(ds)
                If ds.Tables(0).Rows.Count > 0 Then
                    'Code For Stright and reverse filtering 
                    Dim arra = ds.Tables(0).Rows(0).Item("lookupvalue").ToString().Split(",")
                    For am As Integer = 0 To arra.Length - 1
                        If arra(am).Trim <> "" Then
                            Dim var1 = arra(am).Split("-")
                            If var1(0) = FieldID Then
                                If var1(1).Trim.ToUpper = "R" Then
                                    FilterType = "R"
                                End If
                            End If
                        End If
                    Next
                    FilterOnValue = GetFilterOn(ds.Tables(0).Rows(0).Item("FieldID"), obj)
                    If FilterType = "S" Then
                        strQuery = strQuery & "and " & FilterOnFld & "='" & FilterOnValue & "'"
                        'For initial Filter
                        If Not String.IsNullOrEmpty(InitialFilter.Trim) Then
                            Dim arr1 = InitialFilter.Trim.Split(":")
                            Dim qry = "select defaultfieldval FROM MMM_MST_FIELDS WHERE FieldID=" & arr1(0) & ""
                            Using con1 = New SqlConnection(conStr)
                                Using da As New SqlDataAdapter(qry, con1)
                                    da.Fill(DSIntl)
                                End Using
                            End Using
                            strQuery = strQuery & " AND M." & arr1(1) & " =" & DSIntl.Tables(0).Rows(0).Item("defaultfieldval")
                        End If
                    Else
                        Dim t2 As String = "", d2 = ""
                        Dim arrt = GetTableName(ds.Tables(0).Rows(0).Item("DropDown").ToString()).Split("#")
                        Dim arrd2 = ds.Tables(0).Rows(0).Item("DropDown").ToString().Split("-")
                        d2 = arrd2(1)
                        t2 = arrt(0)
                        Dim Q = ""
                        If t2 = "MMM_MST_USER" Then
                            Q = "SELECT DISTINCT t1." & arr(2).ToString() & ",t1.tid FROM " & TABLENAME & " t1 INNER JOIN " & t2 & " t2 ON t1." & TID & "=t2." & FilterOnFld & " WHERE t1.DocumentType='" & arr(1) & "'  AND t1." & arr(2).ToString() & "='" & DDlText & "' AND t1.EID= " & EID & " AND t2.EID= " & EID & " AND  t1.IsAuth=1 AND t2.uid ='" & FilterOnValue & "'"
                        Else
                            Q = "SELECT DISTINCT t1." & arr(2).ToString() & ",t1.tid FROM " & TABLENAME & " t1 INNER JOIN " & t2 & " t2 ON t1." & TID & "=t2." & FilterOnFld & " WHERE t1.DocumentType='" & arr(1) & "' AND t2.DocumentType='" & d2 & "' AND t1." & arr(2).ToString() & "='" & DDlText & "' AND t1.EID= " & EID & " AND t2.EID= " & EID & " AND  t1.IsAuth=1 AND t2.tid ='" & FilterOnValue & "'"
                        End If
                        strQuery = Q
                        'For initial Filter
                        If Not String.IsNullOrEmpty(InitialFilter.Trim) Then
                            Dim arr1 = InitialFilter.Trim.Split(":")
                            Dim qry = "select defaultfieldval FROM MMM_MST_FIELDS WHERE FieldID=" & arr1(0) & ""
                            Using con1 = New SqlConnection(conStr)
                                Using da As New SqlDataAdapter(qry, con1)
                                    da.Fill(DSIntl)
                                End Using
                            End Using
                            strQuery = strQuery & " AND t1." & arr1(1) & " =" & DSIntl.Tables(0).Rows(0).Item("defaultfieldval")
                        End If
                    End If
                End If
            End If
            '' above line till line no. 180 is disabled temporarily for issue troubleshoot paytm pr upload
            ''now enabled again

            If TABLENAME = "MMM_MST_DOC_ITEM" Then

                Dim str = "select * FROM MMM_MST_FIelds where EID=" & EID & " AND Documenttype='" & DRDDL.Item("DocumentType") & "' AND  lookUpValue like '%" & DRDDL.Item("FieldID") & "%' "

                sda = New SqlDataAdapter(str, con)

                ds = New DataSet()

                sda.Fill(ds)
                Dim trgID = ds.Tables(0).Rows(0).Item("FieldID")
                Dim proc As String = DRDDL.Item("CAL_FIELDS").ToString()
                proc = proc + "_UPLOAD"
                Dim FilterOnDDL As String = DRDDL.Item("AUTOFILTER").ToString()
                If FilterOnDDL.Trim.ToUpper <> "DOCID" Then
                    sda = New SqlDataAdapter(proc, con)
                    sda.SelectCommand.CommandType = CommandType.StoredProcedure
                    'DocID OF master document
                    Dim DOCID = GetFilterOn(FilterOnDDL, obj)
                    sda.SelectCommand.Parameters.AddWithValue("@docid", DOCID)
                    'Now The FieldID OF targated field
                    sda.SelectCommand.Parameters.AddWithValue("@FIELDID", CInt(FilterOnDDL))
                    ''ds.Tables(0).Rows(0).Item("FieldID")
                    sda.SelectCommand.Parameters.AddWithValue("@VALUE", GetFilterOn(ds.Tables(0).Rows(0).Item("FieldID"), obj))
                    sda.SelectCommand.Parameters.AddWithValue("@DDLVAL", DDlText)
                    Dim dsProcQ As New DataSet()
                    sda.Fill(dsProcQ)
                    strQuery = dsProcQ.Tables(0).Rows(0).Item("Query").ToString()
                End If
            End If



            If TABLENAME = "MMM_MST_Role" Then
                strQuery = "select " & arr(arr.Length - 1) & "  FROM MMM_MST_Role where EID= " & EID & " AND RoleName<>'SU' AND " & arr(arr.Length - 1) & "='" & DDlText & "'"
            End If

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
            Return "-1"
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


    Public Shared Function GetDropDownChildID(DRDDL As DataRow, EID As Integer, DDlText As String, obj As LineitemWrap) As String
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
        Dim arr1 = arr(1).Split(":")
        'CHILD-8780:Value Contract Item-fld15

        Try
            'Getting table name
            Dim TABLENAME As String = "MMM_MST_DOC_ITEM"
            Dim DSIntl = New DataSet()
            'Query For drop Down Filter
            Dim AutoFilter1 As String = Convert.ToString(DRDDL.Item("ChildTemp_column").ToString())

            strQuery = "select tid," & arr(2) & " FROM " & TABLENAME & " WHERE  DocumentType= '" & arr1(1) & "' AND " & arr(2) & "= '" & DDlText & "'"
            If AutoFilter1.Trim <> "" Then
                Dim val = GetFilterOn(AutoFilter1, obj)
                strQuery = strQuery & " AND DOCID=" & val
            End If
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




    Public Shared Function GetTableName(Str As String) As String
        Dim TABLENAME = ""
        Dim arr = Str.Split("-")
        If UCase(arr(0).ToString()) = "MASTER" Then
            TABLENAME = "MMM_MST_MASTER" & "#tid"
        ElseIf UCase(arr(0).ToString()) = "DOCUMENT" Then
            TABLENAME = "MMM_MST_DOC" & "#tid"
        ElseIf UCase(arr(0).ToString()) = "CHILD" Then
            TABLENAME = "MMM_MST_DOC_ITEM" & "#tid"
        ElseIf UCase(arr(0).ToString) = "SESSION" Then
            TABLENAME = "MMM_MST_USER" & "#UID"
        ElseIf UCase(arr(0).ToString) = "STATIC" Then
            If arr(1).ToString.ToUpper = "USER" Then
                TABLENAME = "MMM_MST_USER" & "#UID"
            ElseIf arr(1).ToString().ToUpper = "LOCATION" Then
                TABLENAME = "MMM_MST_LOCATION" & "#LOCID"
            ElseIf arr(1).ToString() = "MMM_MST_Role" Then
                TABLENAME = "MMM_MST_Role#RoleName"""
            End If
        End If
        Return TABLENAME
    End Function
    Public Shared Function GetFilterOn(FieldID As String, obj As LineitemWrap) As String
        Dim Result As String = ""
        Try
            For Each item In obj.DataItem
                If item.FieldID = FieldID Then
                    Result = item.Values
                    Exit For
                End If
            Next
        Catch ex As Exception
            Return Result
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
                strQuery = "select " & arr(2).ToString() & "," & TID & "[tid]  from " & TABLENAME & " M WHERE isauth=1 and DOCUMENTTYPE='" & arr(1).ToString() & "'"
                strQuery = strQuery & " and M." & arr(arr.Length - 1) & " = '" & DDlText & "'"
                strQuery = strQuery & "   AND EID= " & EID
                'For initial Filter
            ElseIf TABLENAME = "MMM_MST_USER" Then
                If DRDDL.Item("dropdowntype").ToString.ToUpper = "SESSION VALUED" Then
                    'For mobile they will supply userID
                    If DDlText.Contains("[]") Then
                        Dim UserID = ""
                        Dim arrU = Split(DDlText, "[]")
                        UserID = arrU(1).Trim()
                        strQuery = "select " & "UserID" & "," & TID & " from " & TABLENAME & " M WHERE EID=" & EID & " " & ""
                        strQuery = strQuery & " and " & "UserID" & " = '" & UserID & "'"
                    Else
                        strQuery = "select " & arr(2).ToString() & "," & TID & " from " & TABLENAME & " M WHERE EID=" & EID & " " & ""
                        strQuery = strQuery & " and " & "USERNAME" & " = '" & DDlText & "'"
                    End If

                Else
                    strQuery = "select " & arr(2).ToString() & "," & TID & " from " & TABLENAME & " M WHERE EID=" & EID & " " & ""
                    strQuery = strQuery & " and " & arr(2).ToString() & " = '" & DDlText & "'"
                    'For initial Filter
                End If
            ElseIf TABLENAME = "MMM_MST_LOCATION" Then
                strQuery = "select DISTINCT " & arr(2).ToString() & ",SID [tid]  from " & TABLENAME & " M "
                strQuery = strQuery & " and M." & arr(arr.Length - 1) & " = '" & DDlText & "'"
            End If
            'Query For drop Down Filter
            con = New SqlConnection(conStr)
            con.Open()
            If TABLENAME = "MMM_MST_Role" Then
                strQuery = "select " & arr(arr.Length - 1) & "  FROM MMM_MST_Role where EID= " & EID & " AND RoleName<>'SU' AND " & arr(arr.Length - 1) & "='" & DDlText & "'"
            End If
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
End Class
