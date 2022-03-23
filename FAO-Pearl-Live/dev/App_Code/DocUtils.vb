Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Data.SqlClient

Public Class DocUtils
    Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString

    Public Function ExecuteCommanModule(EID As Integer, DocumentType As String, FormType As String, DOCID As Integer, UID As Integer, con As SqlConnection, trans As SqlTransaction) As String
        Dim ret As String = "Error From :DocUtils.ExecuteCommanModule"
        Dim ob As New DynamicForm()
        Try
            'Generating Autonumber fields
            Dim DocDBTableName = "MMM_MST_DOC"
            If (FormType.Trim.ToUpper = "MASTER") Then
                DocDBTableName = "MMM_MST_MASTER"
            ElseIf (FormType.Trim.ToUpper = "USER") Then
                DocDBTableName = "MMM_MST_USER"
            Else
                DocDBTableName = "MMM_MST_DOC_ITEM"
            End If
            Dim AutoNumQ = "select * FROM MMM_MST_FIELDS WHERE EID=" & EID & " AND DocumentType='" & DocumentType & "' AND (FieldType='Auto Number' or Fieldtype='New Auto Number')"
            Dim DSAuto As New DataSet()
            Dim da As New SqlDataAdapter(AutoNumQ, con)
            da.SelectCommand.CommandText = AutoNumQ
            da.SelectCommand.Transaction = trans
            da.Fill(DSAuto)
            For AutoNumC As Integer = 0 To DSAuto.Tables(0).Rows.Count - 1
                Select Case DSAuto.Tables(0).Rows(AutoNumC).Item("fieldtype").ToString
                    Case "Auto Number"
                        da.SelectCommand.Parameters.Clear()
                        If da.SelectCommand.Transaction Is Nothing Then
                            da.SelectCommand.Transaction = trans
                        End If
                        da.SelectCommand.CommandText = "usp_GetAutoNoNew_WS"
                        da.SelectCommand.CommandType = CommandType.StoredProcedure
                        da.SelectCommand.Parameters.AddWithValue("Fldid", DSAuto.Tables(0).Rows(AutoNumC).Item("FieldID"))
                        da.SelectCommand.Parameters.AddWithValue("docid", DOCID)
                        da.SelectCommand.Parameters.AddWithValue("fldmapping", DSAuto.Tables(0).Rows(AutoNumC).Item("FieldMapping"))
                        da.SelectCommand.Parameters.AddWithValue("FormType", FormType)
                        'Transactional Query
                        Dim an As String = Convert.ToString(da.SelectCommand.ExecuteScalar())
                        da.SelectCommand.Parameters.Clear()
                    Case "New Auto Number"
                        da.SelectCommand.Parameters.Clear()
                        da.SelectCommand.CommandText = "usp_GetAutoNoNew_New"
                        da.SelectCommand.CommandType = CommandType.StoredProcedure
                        da.SelectCommand.Parameters.AddWithValue("Fldid", DSAuto.Tables(0).Rows(AutoNumC).Item("FieldID"))
                        da.SelectCommand.Parameters.AddWithValue("SearchFldid", DSAuto.Tables(0).Rows(AutoNumC).Item("Dropdown"))
                        da.SelectCommand.Parameters.AddWithValue("docid", DOCID)
                        da.SelectCommand.Parameters.AddWithValue("fldmapping", DSAuto.Tables(0).Rows(AutoNumC).Item("FieldMapping"))
                        da.SelectCommand.Parameters.AddWithValue("FormType", FormType)
                        Dim an As String = da.SelectCommand.ExecuteScalar()
                        da.SelectCommand.Parameters.Clear()
                End Select
            Next
            'Executing formula field 
            Dim FormulaQ = "select * FROM MMM_MST_FIELDS WHERE EID=" & EID & " AND DocumentType='" & DocumentType & "' AND FieldType='Formula Field' order by displayorder"
            Dim DSF As New DataSet()
            da.SelectCommand.Parameters.Clear()
            da.SelectCommand.CommandText = FormulaQ
            da.SelectCommand.CommandType = CommandType.Text
            da.Fill(DSF)
            'Ececuting Formula Fields
            Dim viewdoc As String = DocumentType
            viewdoc = viewdoc.Replace(" ", "_")
            If DSF.Tables(0).Rows.Count > 0 Then
                For f As Integer = 0 To DSF.Tables(0).Rows.Count - 1
                    Dim formulaeditorr As New formulaEditor
                    Dim forvalue As String = String.Empty
                    forvalue = formulaeditorr.ExecuteFormulaT(DSF.Tables(0).Rows(f).Item("KC_LOGIC"), DOCID, "v" + EID.ToString + viewdoc, EID, 0, con, trans)
                    'Transactional Query
                    Dim upquery As String = "update " & DSF.Tables(0).Rows(f).Item("DBTableName").ToString & "  set  " & DSF.Tables(0).Rows(f).Item("FieldMapping").ToString & "='" & forvalue.ToString & "'  where tid =" & DOCID & ""
                    da.SelectCommand.CommandText = upquery
                    da.SelectCommand.CommandType = CommandType.Text
                    da.SelectCommand.ExecuteNonQuery()
                Next
            End If
            Trigger.ExecuteTriggerT(DocumentType, EID, DOCID, con, trans, 1, TriggerNature:="Create", FormType:=FormType)
            'Code For extending relation 
            Try
                Dim objRel As New Relation()
                Dim objRes = objRel.ExtendRelation(EID, DocumentType, DOCID, UID, "", True)
            Catch ex As Exception
                Throw
            End Try
            Try
                GisMethods.ExecuteReverseGeoCoding(EID, DOCID, DocumentType)
                'trans.Commit()
            Catch ex As Exception
                Throw
            End Try
            'Set History 
            If DocDBTableName <> "MMM_MST_DOC_ITEM" Then
                ob.HistoryT(EID, DOCID, UID, DocumentType, DocDBTableName, "ADD", con, trans)
            End If
            ret = "Comman module executed successfully"
            Return ret
        Catch ex As Exception
            Throw New Exception("Error From :DocUtils.ExecuteCommanModule")
        End Try
    End Function

    Public Function ExecuteDocumentModuleModule(EID As Integer, DocumentType As String, FormType As String, DOCID As Integer, UID As Integer, con As SqlConnection, trans As SqlTransaction) As String
        Try
            Dim ret As String = "ExecuteDocumentModuleModule excuted successfullt"
            Dim ob1 As New DMSUtil()
            Dim ob As New DynamicForm()
            Dim da As New SqlDataAdapter("InsertDefaultMovement", con)
            'da.SelectCommand.CommandText = "InsertDefaultMovement"
            da.SelectCommand.CommandType = CommandType.StoredProcedure
            da.SelectCommand.Parameters.Clear()
            da.SelectCommand.Parameters.AddWithValue("tid", DOCID)
            da.SelectCommand.Parameters.AddWithValue("CUID", UID.ToString())
            da.SelectCommand.Parameters.AddWithValue("what", "UPLOADED")
            da.SelectCommand.Transaction = trans
            da.SelectCommand.ExecuteNonQuery()
            Dim res As String
            'Transactional Query
            res = ob1.GetNextUserFromRolematrixT(DOCID, EID, UID, "", UID, con, trans)
            Dim sretMsgArr() As String = res.Split(":")
            If sretMsgArr(0).ToUpper() = "NO SKIP" Then
                Dim Noskipmsg As String = "Next Approvar/User not found, please contact Admin"
                Throw New Exception("Nexe Approvar not found")
            End If
            'Code for Mapping
            If sretMsgArr(0) = "ARCHIVE" Then
                Dim Op As New Exportdata()
                'Transactional Query
                Op.PushdataT(DOCID, sretMsgArr(1), EID, con, trans)
            End If
            Try
                Dim objRel As New Relation()
                objRel.ExecutePeriodWiseBalance(EID, DOCID, DocumentType, con, trans)
            Catch ex As Exception
                Throw
            End Try
            Return ret
        Catch ex As Exception
            Throw New Exception("Error From :DocUtils.ExecuteDocumentModuleModule")
        End Try

    End Function

    Public Function ExecuteOtherUtils(EID As Integer, DocumentType As String, FormType As String, DOCID As Integer, UID As Integer) As String
        Dim ret As String = "Error occured in DocUtils.ExecuteOtherUtils"
        Dim con As New SqlConnection(conStr)
        Dim da As New SqlDataAdapter("", con)
        Dim ob1 As New DMSUtil()
        Try
            'Code for calling outward webservice
            Try
                If DOCID > 0 Then
                    Dim WSOUT As New WSOutward()
                    Dim WSOret = WSOUT.WBS(DocumentType, EID, DOCID)
                End If
            Catch ex As Exception
            End Try
            'Code for calling outward web service using report
            Dim da1 As SqlDataAdapter = New SqlDataAdapter("select * from MMM_Print_Template where documenttype='" & DocumentType & "' and draft='Service'", con)
            Dim ds1 As New DataSet
            da1.Fill(ds1, "dataset")
            Try
                For kk As Integer = 0 To ds1.Tables("dataset").Rows.Count - 1
                    If ds1.Tables("dataset").Rows(kk).Item("SendType").ToString() = "WS" Then
                        Dim ws As New WSOutward()
                        Dim URLIST As String = ws.WBSREPORT(DocumentType, EID, DOCID)
                    End If
                Next
            Catch ex As Exception
            Finally
                da1.Dispose()
                ds1.Dispose()
            End Try
            'Code For templet calling
            Dim ObjD As New DMSUtil()
            Try
                'Change Required
                ObjD.TemplateCalling(DOCID, EID, DocumentType, "CREATED")
            Catch ex As Exception
            End Try
            Try
                da.SelectCommand.CommandType = CommandType.Text
                da.SelectCommand.Parameters.Clear()
                da.SelectCommand.CommandText = "select isworkflow from mmm_mst_doc where TID=" & DOCID & " and eid='" & EID & "'"
                Dim dt As New DataTable
                da.Fill(dt)
                If dt.Rows.Count = 1 Then
                    If dt.Rows(0).Item(0).ToString = "1" Then
                        'Change Required
                        ob1.TemplateCalling(DOCID, EID, DocumentType, "APPROVE")
                    End If
                End If
            Catch ex As Exception
            End Try
            ret = "ExecuteOtherUtils Executed successfully"
            Return ret
        Catch ex As Exception
            Return ret
        Finally
            con.Close()
            con.Dispose()
        End Try
    End Function

    Public Function ExecuteUserModule(EID As Integer, DocumentType As String, FormType As String, DOCID As Integer, UID As Integer, con As SqlConnection, trans As SqlTransaction) As String
        Dim ret As String = "Error From :DocUtils.ExecuteUserModule"
        Dim ob As New DynamicForm()
        Dim da As New SqlDataAdapter()
        Try
            Dim UserResult As String = ""
            Dim dsres As New DataSet()
            da = New SqlDataAdapter("UserCheckWS", con)
            da.SelectCommand.CommandType = CommandType.StoredProcedure
            da.SelectCommand.Parameters.Clear()
            da.SelectCommand.Parameters.AddWithValue("@UID", DOCID)
            da.SelectCommand.Parameters.AddWithValue("@EID", EID)
            da.SelectCommand.Transaction = trans
            da.Fill(dsres)
            Dim arr = dsres.Tables(1).Rows(0).Item(0).ToString().Split(":")
            If arr(0) <> "0" Then
            Else
                Throw New Exception("Duplicate User found")
            End If
            Return "Module executed successfully"
        Catch ex As Exception
            Throw New Exception("Error From :DocUtils.ExecuteCommanModule")
        End Try
    End Function
End Class
