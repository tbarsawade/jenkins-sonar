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

Public Class DocumentApproval

    Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString

    'Public Function ApproveDocument(EID As Integer, DocType As String, UID As Integer, Data As String, DOCID As Integer, CurStatus As String, BaseDocType As String) As String
    '    Dim ret As String = ""
    '    Dim tran As SqlTransaction = Nothing
    '    Dim con As New SqlConnection(conStr)
    '    Try
    '        Dim ds As New DataSet()
    '        Using con1 As New SqlConnection(conStr)
    '            Using da As New SqlDataAdapter("getDataOfForm100", con1)
    '                da.SelectCommand.CommandType = CommandType.StoredProcedure
    '                da.SelectCommand.Parameters.AddWithValue("@EID", EID)
    '                da.SelectCommand.Parameters.AddWithValue("@FormName", DocType)

    '                da.Fill(ds)
    '            End Using
    '        End Using
    '        If ds.Tables(0).Rows.Count > 0 Then
    '            Dim ObjItem = New LineitemWrap()
    '            'Collection has been created 
    '            ObjItem = UpdateData.CreateListCollection(Data, ds.Tables(0), EID, DocType)
    '            'Now validation goes from here 
    '            Dim Flag = False
    '            Dim ErrMsg = "Error(s) in submission"
    '            'validating Form
    '            Flag = CommanUtil.ValidateForm(EID, ObjItem, "DOCUMENT", DocType, ErrMsg)
    '            'unique key validation
    '            If Flag = True Then
    '                Dim keys = ""
    '                Flag = CommanUtil.ValidateKeys(EID, BaseDocType, ObjItem, "DOCUMENT", keys, DOCID)
    '                If Flag = False Then
    '                    ErrMsg = keys
    '                End If
    '                If Flag = True Then
    '                    Dim onlyFormValiDate As DataView = ds.Tables(0).DefaultView
    '                    onlyFormValiDate.RowFilter = "DocumentType='" & DocType & "' AND FieldType <> 'Child Item' "
    '                    Dim theFlds1 As DataTable = onlyFormValiDate.Table.DefaultView.ToTable()
    '                    theFlds1.Columns.Add("Value")
    '                    theFlds1 = CommanUtil.pushValueIntoTable(theFlds1, ObjItem)
    '                    Dim Str = CommanUtil.FormValidation(DocType, EID, theFlds1, "ADD")
    '                    'IF str returns "true" means it passed all validations
    '                    If Str.ToString().ToLower() = "true" Then
    '                        Flag = True
    '                    Else
    '                        ErrMsg = ErrMsg & " Please " & Str
    '                        Flag = False
    '                    End If
    '                End If
    '            End If
    '            If Flag = True Then
    '                Dim HisReport = CommanUtil.SetHistory(EID, DOCID, "MMM_MST_DOC", UID, "Update WS", DocType)
    '                Dim StrQuery = "Update MMM_MST_DOC SET lastupdate=GETDATE() "
    '                For Each item In ObjItem.DataItem
    '                    If item.DisplayName.Trim.ToUpper <> "ISAUTH" Then
    '                        StrQuery = StrQuery & "," & item.FieldMapping & " = '" & item.Values & "'"
    '                    End If
    '                Next
    '                StrQuery = StrQuery & " WHERE EID=" & EID & "AND DOCUMENTTYPE= '" & BaseDocType & "' AND " & " tid=" & DOCID


    '                con.Open()
    '                tran = con.BeginTransaction()
    '                Dim da = New SqlDataAdapter(StrQuery, con)
    '                da.SelectCommand.Transaction = tran
    '                Dim var = da.SelectCommand.ExecuteNonQuery()
    '                Dim ob1 As New DMSUtil()
    '                Dim res As String
    '                'res = ob1.GetNextUserFromRolematrixT(DOCID, Session("EID"), Val(Session("UID").ToString()), UpQuery, Val(Session("UID").ToString()), con, tran)
    '                res = ob1.GetNextUserFromRolematrixT(DOCID, EID, UID, StrQuery, UID, con, tran)
    '                Dim sretMsgArr() As String = res.Split(":")
    '                Dim IsValidMovement = True
    '                If sretMsgArr(0).ToUpper() = "NO SKIP" Then
    '                    ret = "Next Approvar/User not found, please contact Admin"
    '                    IsValidMovement = False
    '                    tran.Rollback()
    '                End If
    '                If res = "User Not Authorised" Then
    '                    ret = "You are not authorised to Approve this document"
    '                    IsValidMovement = False
    '                    tran.Rollback()
    '                End If
    '                If IsValidMovement = True Then
    '                    If sretMsgArr(0) = "ARCHIVE" Then
    '                        Dim Op As New Exportdata()
    '                        Op.PushdataT(DOCID, sretMsgArr(1), EID, con, tran)
    '                    End If
    '                    Try
    '                        Trigger.ExecuteTriggerT(DocType, EID, DOCID, con, tran, TriggerNature:="Create")
    '                    Catch ex As Exception
    '                        Throw
    '                    End Try
    '                    tran.Commit()
    '                    ret = "Document has been approved successfully."
    '                    Try
    '                        ob1.TemplateCalling(DOCID, EID, "", "APPROVE")
    '                    Catch ex As Exception
    '                    End Try

    '                End If
    '            End If

    '        End If
    '    Catch ex As Exception
    '        If Not tran Is Nothing Then
    '            tran.Rollback()
    '        End If
    '        ErrorLog.sendMail("DocumentApproval.ApproveDocument", ex.Message)
    '        Throw
    '    Finally
    '        If Not con Is Nothing Then
    '            con.Dispose()
    '        End If
    '    End Try
    '    Return ret
    'End Function

    ' bkup on 31st aug 16 for issue resolution for Text coming instead of tid in drop down fields 

    'Public Function ApproveDocument(EID As Integer, DocType As String, UID As Integer, Data As String, DOCID As Integer, CurStatus As String, BaseDocType As String, Optional Action As String = "APPROVAL") As String
    '    Dim ret As String = ""
    '    Try
    '        Dim ds As New DataSet()
    '        Using con As New SqlConnection(conStr)
    '            Using da As New SqlDataAdapter("getDataOfForm100", con)
    '                da.SelectCommand.CommandType = CommandType.StoredProcedure
    '                da.SelectCommand.Parameters.AddWithValue("@EID", EID)
    '                da.SelectCommand.Parameters.AddWithValue("@FormName", DocType)
    '                If con.State <> ConnectionState.Open Then
    '                    con.Open()
    '                End If
    '                da.Fill(ds)
    '            End Using
    '        End Using
    '        '  If ds.Tables(0).Rows.Count > 0 Then
    '        Dim ObjItem = New LineitemWrap()
    '        'Collection has been created 
    '        ObjItem = UpdateData.CreateListCollection(Data, ds.Tables(0), EID, DocType)
    '        'Now validation goes from here 
    '        Dim Flag = False
    '        Dim ErrMsg = "Error(s) in submission"
    '        'validating Form
    '        Flag = CommanUtil.ValidateForm(EID, ObjItem, "DOCUMENT", DocType, ErrMsg)
    '        'unique key validation
    '        If Flag = True Then
    '            Dim keys = ""
    '            Flag = ValidateKeys(EID, DocType, ObjItem, "DOCUMENT", keys, DOCID, DocType)
    '            If Flag = False Then
    '                ErrMsg = keys
    '                ret = keys
    '            End If
    '        Else
    '            ret = ErrMsg
    '        End If
    '        If Flag = True Then
    '            Dim HisReport = CommanUtil.SetHistory(EID, DOCID, "MMM_MST_DOC", UID, "Update WS", DocType)
    '            Dim StrQuery = "Update MMM_MST_DOC SET lastupdate=GETDATE() "
    '            For Each item In ObjItem.DataItem
    '                If item.DisplayName.Trim.ToUpper <> "ISAUTH" Then
    '                    StrQuery = StrQuery & "," & item.FieldMapping & " = '" & item.Values & "'"
    '                End If
    '            Next
    '            StrQuery = StrQuery & " WHERE EID=" & EID & "AND DOCUMENTTYPE= '" & BaseDocType & "' AND " & " tid=" & DOCID
    '            'Dim tran As SqlTransaction = Nothing
    '            'Dim con As New SqlConnection(conStr)
    '            ' tran = con.BeginTransaction()
    '            Using con = New SqlConnection(conStr)
    '                Using da = New SqlDataAdapter(StrQuery, con)
    '                    con.Open()
    '                    Dim var = da.SelectCommand.ExecuteNonQuery()
    '                End Using
    '            End Using
    '            Dim ob1 As New DMSUtil()
    '            Dim res As String
    '            Dim IsValidMovement = True
    '            If UCase(Action) = "APPROVAL" Then
    '                res = "Not Uploaded:Fail" 'ob1.GetNextUserFromRolematrix(DocID, Session("EID"), Val(Session("UID").ToString()), UpQuery, Val(Session("UID").ToString()))
    '                res = ob1.GetNextUserFromRolematrix(DOCID, EID, UID, StrQuery, UID)
    '                Dim sretMsgArr() As String = res.Split(":")

    '                If sretMsgArr(0).ToUpper() = "NO SKIP" Then
    '                    ret = "Next Approvar/User not found, please contact Admin"
    '                    IsValidMovement = False
    '                End If
    '                If res = "User Not Authorised" Then
    '                    ret = "You are not authorised to Approve this document"
    '                    IsValidMovement = False
    '                End If
    '                If sretMsgArr(0) = "ARCHIVE" Then
    '                    Dim Op As New Exportdata()
    '                    Op.Pushdata(DOCID, sretMsgArr(1), EID)
    '                End If
    '                If IsValidMovement = True Then
    '                    ret = "Document has been approved successfully."
    '                    Try
    '                        ob1.TemplateCalling(DOCID, EID, "", "APPROVE")
    '                    Catch ex As Exception
    '                    End Try
    '                    Try
    '                        Trigger.ExecuteTrigger(DocType, EID, DOCID, TriggerNature:="Create")
    '                    Catch ex As Exception
    '                    End Try
    '                End If
    '            ElseIf UCase(Action) = "RECONSIDER" Then
    '                Using con As New SqlConnection(conStr)
    '                    Using da As New SqlDataAdapter("ReconsiderWorkFlow_RM", con)
    '                        da.SelectCommand.CommandType = CommandType.StoredProcedure
    '                        da.SelectCommand.Parameters.AddWithValue("TID", DOCID)
    '                        da.SelectCommand.Parameters.AddWithValue("remarks", "RECONSIDERED")
    '                        da.SelectCommand.Parameters.AddWithValue("qry", StrQuery)
    '                        da.SelectCommand.Parameters.AddWithValue("auid", UID)
    '                        If con.State <> ConnectionState.Open Then
    '                            con.Open()
    '                        End If
    '                        Dim iSt As Integer = da.SelectCommand.ExecuteScalar()
    '                    End Using
    '                End Using
    '                ret = "Document has been reconsidered successfully."
    '                Try
    '                    ob1.TemplateCalling(DOCID, EID, "", "REJECT")
    '                Catch ex As Exception
    '                End Try
    '                Try
    '                    Trigger.ExecuteTrigger(DocType, EID, DOCID, TriggerNature:="Create")
    '                Catch ex As Exception
    '                End Try
    '            ElseIf UCase(Action) = "REJECT" Then
    '                Using con As New SqlConnection(conStr)
    '                    Using da As New SqlDataAdapter("PermanentRejectDoc_RM", con)
    '                        da.SelectCommand.CommandType = CommandType.StoredProcedure
    '                        da.SelectCommand.Parameters.AddWithValue("TID", DOCID)
    '                        da.SelectCommand.Parameters.AddWithValue("remarks", "REJECTED")
    '                        da.SelectCommand.Parameters.AddWithValue("qry", StrQuery)
    '                        da.SelectCommand.Parameters.AddWithValue("auid", UID)
    '                        If con.State <> ConnectionState.Open Then
    '                            con.Open()
    '                        End If
    '                        Dim iSt As Integer = da.SelectCommand.ExecuteScalar()
    '                    End Using
    '                End Using
    '                ret = "Document has been rejected successfully."
    '                Try
    '                    ob1.TemplateCalling(DOCID, EID, "", "REJECT")
    '                Catch ex As Exception
    '                End Try
    '                Try
    '                    Trigger.ExecuteTrigger(DocType, EID, DOCID, TriggerNature:="Create")
    '                Catch ex As Exception
    '                End Try
    '            ElseIf UCase(Action) = "CRM" Then
    '                Dim CRMCols As New StringBuilder()
    '                Dim CRMVals As New StringBuilder()
    '                CRMCols.Append("INSERT INTO MMM_MST_CRM(EID,DOCID,USERID")
    '                CRMVals.Append("VALUES (" & EID & "," & DOCID & "," & UID)
    '                For Each item In ObjItem.DataItem
    '                    If item.DisplayName.Trim.ToUpper <> "ISAUTH" Then
    '                        CRMCols.Append("," & item.FieldMapping)
    '                        CRMVals.Append(",'" & item.Values & "'")
    '                    End If
    '                Next
    '                Dim UpdateQ = "DECLARE @TID AS INT;SET @TID=SCOPE_IDENTITY();UPDATE MMM_MST_CRM SET CurStatus=(SELECT curstatus FROM MMM_MST_DOC WHERE EID= " & EID & " AND tid= " & DOCID & " ),Ordering=(select Top 1 Ordering FROM MMM_DOC_DTL WHERE tid=(SELECT lasttid FROM MMM_MST_DOC WHERE EID=" & EID & " AND tid= " & DOCID & " )) where EID= " & EID & " AND tid=@TID;"
    '                Dim strCRM = CRMCols.Append(")").ToString & CRMVals.Append(")").ToString & UpdateQ
    '                Using con As New SqlConnection(conStr)
    '                    Using da As New SqlDataAdapter(strCRM, con)
    '                        da.SelectCommand.CommandType = CommandType.Text
    '                        If con.State <> ConnectionState.Open Then
    '                            con.Open()
    '                        End If
    '                        Dim CRMret As Integer = da.SelectCommand.ExecuteScalar()
    '                    End Using
    '                End Using
    '                ret = "CRM inserted successfully"
    '            End If

    '        End If

    '        '  End If
    '    Catch ex As Exception
    '        ErrorLog.sendMail("DocumentApproval.ApproveDocument", ex.Message)
    '        Throw
    '    End Try
    '    Return ret
    'End Function


    Public Function ApproveDocument(EID As Integer, DocType As String, UID As Integer, Data As String, DOCID As Integer, CurStatus As String, BaseDocType As String, Optional Action As String = "APPROVAL") As String
        Dim ret As String = ""
        Try
            Dim ds As New DataSet()
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
            '  If ds.Tables(0).Rows.Count > 0 Then
            Dim ObjItem = New LineitemWrap()
            Dim ObjItem1 = New LineitemWrap()
            'Collection has been created 
            ObjItem = UpdateData.CreateListCollection(Data, ds.Tables(0), EID, DocType)
            'Now validation goes from here 
            Dim LStDataFinal As New List(Of DataWraper1)
            Dim LstlineItem As New List(Of LineitemWrap)
            LstlineItem.Add(ObjItem)
            Dim objData As New DataWraper1()
            objData.DocumentType = DocType
            objData.FormType = "Action Driven"
            objData.LineItem = LstlineItem
            LStDataFinal.Add(objData)
            CommanUtil.FillData(LStDataFinal, ds, EID, DocType, UID)
            Dim Flag = False
            Dim ErrMsg = "Error(s) in submission"
            ObjItem1 = LStDataFinal.Item(0).LineItem.Item(0)
            'validating Form
            'Flag = CommanUtil.ValidateForm(EID, ObjItem, "DOCUMENT", DocType, ErrMsg)
            Flag = CommanUtil.ValidateForm(EID, LStDataFinal.Item(0).LineItem.Item(0), "DOCUMENT", DocType, ErrMsg)
            'unique key validation
            If Flag = True Then
                Dim keys = ""
                Flag = ValidateKeys(EID, DocType, LStDataFinal.Item(0).LineItem.Item(0), "DOCUMENT", keys, DOCID, DocType)
                If Flag = False Then
                    ErrMsg = keys
                    ret = keys
                End If
            Else
                ret = ErrMsg
            End If
            If Flag = True Then
                Dim HisReport = CommanUtil.SetHistory(EID, DOCID, "MMM_MST_DOC", UID, "Update WS", DocType)
                Dim StrQuery = "Update MMM_MST_DOC SET lastupdate=GETDATE() "
                For Each item In LStDataFinal.Item(0).LineItem.Item(0).DataItem
                    If item.DisplayName.Trim.ToUpper <> "ISAUTH" Then
                        StrQuery = StrQuery & "," & item.FieldMapping & " = '" & item.Values & "'"
                    End If
                Next
                StrQuery = StrQuery & " WHERE EID=" & EID & "AND DOCUMENTTYPE= '" & BaseDocType & "' AND " & " tid=" & DOCID
                'Dim tran As SqlTransaction = Nothing
                'Dim con As New SqlConnection(conStr)
                ' tran = con.BeginTransaction()
                If UCase(Action) <> "APPROVAL" Then
                    Using con = New SqlConnection(conStr)
                        Using da = New SqlDataAdapter(StrQuery, con)
                            con.Open()
                            Dim var = da.SelectCommand.ExecuteNonQuery()
                        End Using
                    End Using
                End If
                Dim ob1 As New DMSUtil()
                Dim res As String
                Dim IsValidMovement = True
                If UCase(Action) = "APPROVAL" Then
                    res = "Not Uploaded:Fail" 'ob1.GetNextUserFromRolematrix(DocID, Session("EID"), Val(Session("UID").ToString()), UpQuery, Val(Session("UID").ToString()))
                    res = ob1.GetNextUserFromRolematrix(DOCID, EID, UID, StrQuery, UID)
                    Dim sretMsgArr() As String = res.Split(":")

                    If sretMsgArr(0).ToUpper() = "NO SKIP" Then
                        ret = "Next Approvar/User not found, please contact Admin"
                        IsValidMovement = False
                    End If
                    If res = "User Not Authorised" Then
                        ret = "You are not authorised to Approve this document"
                        IsValidMovement = False
                    End If
                    If sretMsgArr(0) = "ARCHIVE" Then
                        Dim Op As New Exportdata()
                        Op.Pushdata(DOCID, sretMsgArr(1), EID)
                    End If
                    If IsValidMovement = True Then
                        ret = "Document has been approved successfully."
                        'Try
                        '    ob1.TemplateCalling(DOCID, EID, "", "APPROVE")
                        'Catch ex As Exception
                        'End Try
                        Try
                            Using con = New SqlConnection(conStr)
                                Using da = New SqlDataAdapter(StrQuery, con)
                                    con.Open()
                                    Dim var = da.SelectCommand.ExecuteNonQuery()
                                End Using
                            End Using
                            Trigger.ExecuteTrigger(DocType, EID, DOCID, TriggerNature:="Create")
                        Catch ex As Exception
                            ret = "Error in trigger execution (in Approval WS)"
                        End Try
                        Try
                            ob1.TemplateCalling(DOCID, EID, "", "APPROVE")
                        Catch ex As Exception
                        End Try
                    End If
                ElseIf UCase(Action) = "RECONSIDER" Then
                    Using con As New SqlConnection(conStr)
                        Using da As New SqlDataAdapter("ReconsiderWorkFlow_RM", con)
                            da.SelectCommand.CommandType = CommandType.StoredProcedure
                            da.SelectCommand.Parameters.AddWithValue("TID", DOCID)
                            da.SelectCommand.Parameters.AddWithValue("remarks", "RECONSIDERED")
                            da.SelectCommand.Parameters.AddWithValue("qry", StrQuery)
                            da.SelectCommand.Parameters.AddWithValue("auid", UID)
                            If con.State <> ConnectionState.Open Then
                                con.Open()
                            End If
                            Dim iSt As Integer = da.SelectCommand.ExecuteScalar()
                        End Using
                    End Using
                    ret = "Document has been reconsidered successfully."
                    Try
                        ob1.TemplateCalling(DOCID, EID, "", "REJECT")
                    Catch ex As Exception
                    End Try
                    Try
                        Trigger.ExecuteTrigger(DocType, EID, DOCID, TriggerNature:="Create")
                    Catch ex As Exception
                    End Try
                ElseIf UCase(Action) = "REJECT" Then
                    Using con As New SqlConnection(conStr)
                        Using da As New SqlDataAdapter("PermanentRejectDoc_RM", con)
                            da.SelectCommand.CommandType = CommandType.StoredProcedure
                            da.SelectCommand.Parameters.AddWithValue("TID", DOCID)
                            da.SelectCommand.Parameters.AddWithValue("remarks", "REJECTED")
                            da.SelectCommand.Parameters.AddWithValue("qry", StrQuery)
                            da.SelectCommand.Parameters.AddWithValue("auid", UID)
                            If con.State <> ConnectionState.Open Then
                                con.Open()
                            End If
                            Dim iSt As Integer = da.SelectCommand.ExecuteScalar()
                        End Using
                    End Using
                    ret = "Document has been rejected successfully."
                    Try
                        ob1.TemplateCalling(DOCID, EID, "", "REJECT")
                    Catch ex As Exception
                    End Try
                    Try
                        Trigger.ExecuteTrigger(DocType, EID, DOCID, TriggerNature:="Create")
                    Catch ex As Exception
                    End Try
                ElseIf UCase(Action) = "CRM" Then
                    Dim CRMCols As New StringBuilder()
                    Dim CRMVals As New StringBuilder()
                    CRMCols.Append("INSERT INTO MMM_MST_CRM(EID,DOCID,USERID")
                    CRMVals.Append("VALUES (" & EID & "," & DOCID & "," & UID)
                    For Each item In LStDataFinal.Item(0).LineItem.Item(0).DataItem
                        If item.DisplayName.Trim.ToUpper <> "ISAUTH" Then
                            CRMCols.Append("," & item.FieldMapping)
                            CRMVals.Append(",'" & item.Values & "'")
                        End If
                    Next
                    Dim UpdateQ = "DECLARE @TID AS INT;SET @TID=SCOPE_IDENTITY();UPDATE MMM_MST_CRM SET CurStatus=(SELECT curstatus FROM MMM_MST_DOC WHERE EID= " & EID & " AND tid= " & DOCID & " ),Ordering=(select Top 1 Ordering FROM MMM_DOC_DTL WHERE tid=(SELECT lasttid FROM MMM_MST_DOC WHERE EID=" & EID & " AND tid= " & DOCID & " )) where EID= " & EID & " AND tid=@TID;"
                    Dim strCRM = CRMCols.Append(")").ToString & CRMVals.Append(")").ToString & UpdateQ
                    Using con As New SqlConnection(conStr)
                        Using da As New SqlDataAdapter(strCRM, con)
                            da.SelectCommand.CommandType = CommandType.Text
                            If con.State <> ConnectionState.Open Then
                                con.Open()
                            End If
                            Dim CRMret As Integer = da.SelectCommand.ExecuteScalar()
                        End Using
                    End Using
                    ret = "CRM inserted successfully"
                End If
            End If
            '  End If
        Catch ex As Exception
            ErrorLog.sendMail("DocumentApproval.ApproveDocument", ex.Message)
            Throw
        End Try
        Return ret
    End Function



    Public Shared Function ValidateKeys(EID As Integer, DocumentType As String, Data As LineitemWrap, Formtype As String, ByRef Msg As String, Optional tid As String = "0", Optional actionName As String = "") As Boolean

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
                ds = objUp.GetKeys(EID, actionName)
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

    Public Function ValidateDocStatus(EID As Integer, DOCID As Integer, DocumentType As String, curstatus As String) As DataSet
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim ds As New DataSet()
        Try
            Using con = New SqlConnection(conStr)
                Using da = New SqlDataAdapter("checkCurrStatus", con)
                    da.SelectCommand.CommandType = CommandType.StoredProcedure
                    da.SelectCommand.Parameters.AddWithValue("@EID", EID)
                    da.SelectCommand.Parameters.AddWithValue("@DocumentType", DocumentType)
                    da.SelectCommand.Parameters.AddWithValue("@curstatus", curstatus)
                    da.Fill(ds)
                End Using
            End Using
        Catch ex As Exception
            Throw
        End Try
        Return ds
    End Function

    Public Function ValidateKeys(EID As Integer, DocumentType As String, Data As String, ByRef Keys As String) As Boolean
        Dim ret As Boolean = False
        Dim ds As New DataSet()
        Dim IskeySupplyed As Boolean = False
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
                                    Values = DropDown.GetDropDownID(DDLR, EID, Values, Nothing)
                                End If
                                'GetDropDownID(DRDDL As DataRow, EID As Integer, DDlText As String, obj As LineitemWrap)
                                'Code For Removing Extra Charactor 
                                Keys = Keys & " AND " & ds.Tables(0).Rows(j).Item("FieldMapping") & "='" & Values.Trim() & "'"
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

    Public Function GetKeys(EID As Integer, DocumentType As String) As DataSet
        Dim ret As String = ""
        Dim ds As New DataSet()
        Try
            'TO avoid Sql Injection "--" Is used to comment script 
            DocumentType = DocumentType.Replace("--", String.Empty)
            Using con As New SqlConnection(conStr)
                Using da As New SqlDataAdapter("GetKeys0000000", con)
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

End Class
