Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Data.SqlClient
Imports System.Data.OracleClient

Public Class ScheduleDocument
    Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString


    Public EID As Integer
    Public SourceDoc As String = ""
    Public CreateEvent As String

    'start constructor....
    Public Sub New(EID As Integer, SourceDoc As String, CreateEvent As String)
        Me.EID = EID
        Me.SourceDoc = SourceDoc
        Me.CreateEvent = CreateEvent
    End Sub

    ' TODO: Complete member initialization
    Public Sub DynamicDocument()
    End Sub
    ' end constr........

    Function GetDynamicDocument() As DataSet
        Dim DS As New DataSet()
        Try
            Dim Qry As String = "select * from  mmm_schdoc_main where  CreateEvent='" + CreateEvent + "' AND IsActive='1'"
            If Not (String.IsNullOrEmpty(SourceDoc)) Then
                Qry = Qry & " and SourceDoc='" + SourceDoc + "' "
            End If
            If EID <> 0 Then
                Qry = Qry & " and EID=" & EID
            End If
            Using con As SqlConnection = New SqlConnection(conStr)
                Using da As New SqlDataAdapter(Qry, con)
                    da.Fill(DS)
                End Using
            End Using
        Catch ex As Exception
        End Try
        Return DS
    End Function

    Public Function CreateDynamicDocument(DOCID As Integer) As String
        Try
            Dim ret As String = "No setting exists."
            '    Dim DS As New DataSet()

            '    'If any setting exists
            '    If (DS.Tables(0).Rows.Count > 0) Then
            '        Dim Qry As String = ""
            ret = Execute(DOCID)
            '    End If
            Return ret
        Catch ex As Exception
            Throw New Exception("Exception caused")
        End Try
    End Function
    'Public Function CreateDynamicDocument(DOCID As Integer) As String
    '    Try
    '        Dim ret As String = "No setting exists."
    '        Dim DS As New DataSet()
    '        Dim newval As Integer
    '        Dim NoOfRecord As Integer
    '        DS = GetDynamicDocument()
    '        Dim dsflds As New DataSet()
    '        Dim DSdoc As New DataSet()
    '        Dim doctype As String = ""
    '        Dim CDocID As Integer = 0
    '        Dim DocDBTableName = "MMM_MST_DOC"
    '        Dim FormType = "MASTER"
    '        Dim UID As Integer = 1024
    '        Dim ob1 As New DMSUtil()
    '        Dim ob As New DynamicForm()
    '        Dim targetDoc As String = ""
    '        'If any setting exists
    '        If (DS.Tables(0).Rows.Count > 0) Then
    '            Dim Qry As String = ""
    '            ret = "Document created successfully."
    '            doctype = DS.Tables(0).Rows(0)("SQType")
    '            If doctype = "DOCUMENT" Then
    '                Qry = "SELECT * FROM MMM_MST_DOC where DocumentType='" + SourceDoc + "' and EID=" & EID & " and tid=" & DOCID & ""
    '            Else
    '                Qry = "SELECT * FROM MMM_MST_MASTER where DocumentType='" + SourceDoc + "' and EID=" & EID & " and tid=" & DOCID & ""
    '            End If
    '            Using con As SqlConnection = New SqlConnection(conStr)
    '                Using da As New SqlDataAdapter(Qry, con)
    '                    da.Fill(DSdoc)
    '                End Using
    '            End Using
    '            If doctype = "DOCUMENT" Then
    '                UID = Convert.ToString(DSdoc.Tables(0).Rows(0).Item(DS.Tables(0).Rows(0).Item("RoleName")))
    '            End If
    '            FormType = doctype
    '            Dim StrQuery = ""
    '            targetDoc = Convert.ToString(DS.Tables(0).Rows(0)("TargetDoc"))
    '            'based on the condition get all the values of created master or doc
    '            newval = DS.Tables(0).Rows(0)("Tid")
    '            NoOfRecord = DS.Tables(0).Rows(0)("NoofRecords")
    '            'Creating list for fromula execution
    '            Dim lstParentField As New List(Of UserData)
    '            lstParentField = CreateList(DSdoc.Tables(0).Rows(0), SourceDoc)
    '            dsflds = GetFieldMappingSetting(newval)
    '            For k As Integer = 1 To NoOfRecord
    '                StrQuery = ""
    '                StrQuery = GenerateQuery(EID, doctype, targetDoc, 0, dsflds.Tables(0), DSdoc.Tables(0).Rows(0), Nothing, lstParentField, Nothing, UID)
    '                Dim con As New SqlConnection(conStr)
    '                Dim da As New SqlDataAdapter(StrQuery, con)
    '                Dim trans As SqlTransaction
    '                con.Open()
    '                trans = con.BeginTransaction()
    '                da.SelectCommand.Transaction = trans
    '                da.SelectCommand.CommandText = StrQuery
    '                da.SelectCommand.Connection = con
    '                CDocID = da.SelectCommand.ExecuteScalar()
    '                'Code for creating child document
    '                CreateChildDoc(newval, DOCID, CDocID, lstParentField, con, trans)
    '                'Generating Autonumber fields
    '                Dim AutoNumQ = "select * FROM MMM_MST_FIELDS WHERE EID=" & EID & " AND DocumentType='" & targetDoc & "' AND (FieldType='Auto Number' or Fieldtype='New Auto Number')"
    '                Dim DSAuto As New DataSet()
    '                da.SelectCommand.CommandText = AutoNumQ
    '                da.Fill(DSAuto)
    '                For AutoNumC As Integer = 0 To DSAuto.Tables(0).Rows.Count - 1
    '                    Select Case DSAuto.Tables(0).Rows(AutoNumC).Item("fieldtype").ToString
    '                        Case "Auto Number"
    '                            da.SelectCommand.Parameters.Clear()
    '                            If da.SelectCommand.Transaction Is Nothing Then
    '                                da.SelectCommand.Transaction = trans
    '                            End If
    '                            da.SelectCommand.CommandText = "usp_GetAutoNoNew_WS"
    '                            da.SelectCommand.CommandType = CommandType.StoredProcedure
    '                            da.SelectCommand.Parameters.AddWithValue("Fldid", DSAuto.Tables(0).Rows(AutoNumC).Item("FieldID"))
    '                            da.SelectCommand.Parameters.AddWithValue("docid", CDocID)
    '                            da.SelectCommand.Parameters.AddWithValue("fldmapping", DSAuto.Tables(0).Rows(AutoNumC).Item("FieldMapping"))
    '                            da.SelectCommand.Parameters.AddWithValue("FormType", FormType)
    '                            'Transactional Query
    '                            Dim an As String = Convert.ToString(da.SelectCommand.ExecuteScalar())
    '                            da.SelectCommand.Parameters.Clear()
    '                        Case "New Auto Number"
    '                            da.SelectCommand.Parameters.Clear()
    '                            da.SelectCommand.CommandText = "usp_GetAutoNoNew_New"
    '                            da.SelectCommand.CommandType = CommandType.StoredProcedure
    '                            da.SelectCommand.Parameters.AddWithValue("Fldid", DSAuto.Tables(0).Rows(AutoNumC).Item("FieldID"))
    '                            da.SelectCommand.Parameters.AddWithValue("SearchFldid", DSAuto.Tables(0).Rows(AutoNumC).Item("Dropdown"))
    '                            da.SelectCommand.Parameters.AddWithValue("docid", CDocID)
    '                            da.SelectCommand.Parameters.AddWithValue("fldmapping", DSAuto.Tables(0).Rows(AutoNumC).Item("FieldMapping"))
    '                            da.SelectCommand.Parameters.AddWithValue("FormType", FormType)
    '                            Dim an As String = da.SelectCommand.ExecuteScalar()
    '                            da.SelectCommand.Parameters.Clear()
    '                    End Select
    '                Next
    '                'Executing formula field of main form
    '                Dim FormulaQ = "select * FROM MMM_MST_FIELDS WHERE EID=" & EID & " AND DocumentType='" & targetDoc & "' AND FieldType='Formula Field' order by displayorder"
    '                Dim DSF As New DataSet()
    '                da.SelectCommand.Parameters.Clear()
    '                da.SelectCommand.CommandText = FormulaQ
    '                da.SelectCommand.CommandType = CommandType.Text
    '                da.Fill(DSF)
    '                'Ececuting Formula Fields
    '                Try
    '                    Dim viewdoc As String = targetDoc
    '                    viewdoc = viewdoc.Replace(" ", "_")
    '                    If DSF.Tables(0).Rows.Count > 0 Then
    '                        For f As Integer = 0 To DSF.Tables(0).Rows.Count - 1
    '                            Dim formulaeditorr As New formulaEditor
    '                            Dim forvalue As String = String.Empty
    '                            forvalue = formulaeditorr.ExecuteFormulaT(DSF.Tables(0).Rows(f).Item("KC_LOGIC"), CDocID, "v" + EID.ToString + viewdoc, EID, 0, con, trans)
    '                            'Transactional Query
    '                            Dim upquery As String = "update " & DSF.Tables(0).Rows(f).Item("DBTableName").ToString & "  set  " & DSF.Tables(0).Rows(f).Item("FieldMapping").ToString & "='" & forvalue.ToString & "'  where tid =" & CDocID & ""
    '                            da.SelectCommand.CommandText = upquery
    '                            da.SelectCommand.CommandType = CommandType.Text
    '                            da.SelectCommand.ExecuteNonQuery()
    '                        Next
    '                    End If
    '                Catch ex As Exception
    '                    Throw
    '                End Try
    '                'Set History 
    '                ob.HistoryT(EID, CDocID, UID, targetDoc, DocDBTableName, "ADD", con, trans)
    '                'Trigger OF Form Will Execute here 
    '                Trigger.ExecuteTriggerT(targetDoc, EID, CDocID, con, trans, 1, TriggerNature:="Create", FormType:=FormType)
    '                'Code For extending relation 
    '                Try
    '                    Dim objRel As New Relation()
    '                    Dim objRes = objRel.ExtendRelation(EID, targetDoc, CDocID, UID, "", True)
    '                Catch ex As Exception
    '                    Throw
    '                End Try
    '                Try
    '                    GisMethods.ExecuteReverseGeoCoding(EID, CDocID, targetDoc)
    '                    'trans.Commit()
    '                Catch ex As Exception
    '                    Throw
    '                End Try
    '                'Code for calling outward webservice
    '                Try
    '                    If CDocID > 0 Then
    '                        Dim WSOUT As New WSOutward()
    '                        Dim WSOret = WSOUT.WBS(doctype, EID, CDocID)
    '                    End If
    '                Catch ex As Exception
    '                End Try
    '                'Code for Document Approval
    '                If FormType.ToUpper() = "DOCUMENT" Then
    '                    da.SelectCommand.CommandText = "InsertDefaultMovement"
    '                    da.SelectCommand.CommandType = CommandType.StoredProcedure
    '                    da.SelectCommand.Parameters.Clear()
    '                    da.SelectCommand.Parameters.AddWithValue("tid", CDocID)
    '                    da.SelectCommand.Parameters.AddWithValue("CUID", UID.ToString())
    '                    da.SelectCommand.Parameters.AddWithValue("what", "UPLOADED")
    '                    da.SelectCommand.ExecuteNonQuery()
    '                    Dim res As String
    '                    'Transactional Query
    '                    res = ob1.GetNextUserFromRolematrixT(CDocID, EID, UID, "", UID, con, trans)
    '                    Dim sretMsgArr() As String = res.Split(":")
    '                    'Code for Mapping
    '                    If sretMsgArr(0) = "ARCHIVE" Then
    '                        Dim Op As New Exportdata()
    '                        'Transactional Query
    '                        Op.PushdataT(CDocID, sretMsgArr(1), EID, con, trans)
    '                    End If
    '                    Try
    '                        Dim objRel As New Relation()
    '                        objRel.ExecutePeriodWiseBalance(EID, CDocID, targetDoc, con, trans)
    '                    Catch ex As Exception

    '                    End Try
    '                    If FormType <> "USER" Then
    '                        trans.Commit()

    '                        ' below code for webservice report - 20_june_14 ravi
    '                        If CDocID > 0 Then
    '                            Dim da1 As SqlDataAdapter = New SqlDataAdapter("select * from MMM_Print_Template where documenttype='" & doctype & "' and draft='Service'", con)
    '                            Dim ds1 As New DataSet
    '                            da1.Fill(ds1, "dataset")
    '                            Try
    '                                For kk As Integer = 0 To ds1.Tables("dataset").Rows.Count - 1
    '                                    If ds1.Tables("dataset").Rows(kk).Item("SendType").ToString() = "WS" Then
    '                                        Dim ws As New WSOutward()
    '                                        Dim URLIST As String = ws.WBSREPORT(doctype, EID, CDocID)
    '                                    End If
    '                                Next
    '                            Catch ex As Exception
    '                            Finally
    '                                da1.Dispose()
    '                                ds1.Dispose()
    '                            End Try
    '                        End If
    '                        'Code For templet calling
    '                        Dim ObjD As New DMSUtil()
    '                        Try
    '                            'Change Required
    '                            ObjD.TemplateCalling(CDocID, EID, targetDoc, "CREATED")
    '                        Catch ex As Exception
    '                        End Try
    '                        Try
    '                            da.SelectCommand.CommandType = CommandType.Text
    '                            da.SelectCommand.Parameters.Clear()
    '                            da.SelectCommand.CommandText = "select isworkflow from mmm_mst_doc where TID=" & CDocID & " and eid='" & EID & "'"
    '                            Dim dt As New DataTable
    '                            da.Fill(dt)
    '                            If dt.Rows.Count = 1 Then
    '                                If dt.Rows(0).Item(0).ToString = "1" Then
    '                                    'Change Required
    '                                    ob1.TemplateCalling(CDocID, EID, targetDoc, "APPROVE")
    '                                End If
    '                            End If
    '                        Catch ex As Exception
    '                        End Try
    '                    Else
    '                        'IF User 
    '                        Dim UserResult As String = ""
    '                        Dim dsres As New DataSet()
    '                        da = New SqlDataAdapter("UserCheckWS", con)
    '                        da.SelectCommand.CommandType = CommandType.StoredProcedure
    '                        da.SelectCommand.Parameters.Clear()
    '                        da.SelectCommand.Parameters.AddWithValue("@UID", CDocID)
    '                        da.SelectCommand.Parameters.AddWithValue("@EID", EID)
    '                        da.SelectCommand.Transaction = trans
    '                        da.Fill(dsres)
    '                        'Transactional Query
    '                        Dim arr = dsres.Tables(1).Rows(0).Item(0).ToString().Split(":")
    '                        If arr(0) <> "0" Then
    '                            'Commiting transaction here
    '                            trans.Commit()
    '                            Dim DMS As New DMSUtil
    '                            Try
    '                                ''Change Required
    '                                DMS.notificationMail(CDocID, EID, "USER", "USER CREATED")
    '                            Catch ex As Exception
    '                            End Try
    '                        Else
    '                            trans.Rollback()
    '                        End If
    '                    End If
    '                    'Commiting transaction 
    '                Else
    '                    trans.Commit()
    '                End If
    '            Next

    '        End If
    '        Return ret
    '    Catch ex As Exception
    '        Throw New Exception("Exception caused")
    '    End Try
    'End Function

    Private Function CreateChildDoc(ConfigID As Integer, SDocID As Integer, TDocID As Integer, lstParent As List(Of UserData), con As SqlConnection, trans As SqlTransaction) As String
        Dim ret As String = ""
        Try
            Dim dsChild As New DataSet()
            Dim ChildSourcedoc As String = ""
            Dim ChildTargetdoc As String = ""
            Dim StrQry As String = ""
            'Checking is there any confuguration exist or not
            dsChild = GetFieldMappingSettingofChild(ConfigID)
            If (dsChild.Tables(0).Rows.Count > 0) Then
                'there might be more than one child item on the same document
                For i As Integer = 0 To dsChild.Tables(0).Rows.Count - 1
                    'Getting child of source document
                    ChildSourcedoc = Convert.ToString(dsChild.Tables(0).Rows(0).Item("ChildSourcedoc"))
                    ChildTargetdoc = Convert.ToString(dsChild.Tables(0).Rows(0).Item("ChildTargetdoc"))
                    Dim lstChild As New List(Of UserData)
                    Dim qry As String = "SELECT * FROM  MMM_MST_DOC_Item WHERE DOCID=" & SDocID & " and DocumentType='" & ChildSourcedoc & "'"
                    Dim dscData As New DataSet()
                    Dim da As New SqlDataAdapter(qry, con)
                    da.SelectCommand.Transaction = trans
                    da.Fill(dscData)
                    For j As Integer = 0 To dscData.Tables(0).Rows.Count - 1
                        lstChild = CreateList(dscData.Tables(0).Rows(j), ChildSourcedoc)
                        StrQry = GenerateQuery(0, "CHILD", ChildTargetdoc, TDocID, dsChild.Tables(1), dscData.Tables(0).Rows(j), Nothing, lstParent, lstChild)
                        da = New SqlDataAdapter(StrQry, con)
                        da.SelectCommand.Transaction = trans
                        da.SelectCommand.Connection = con
                        Dim CDocID = da.SelectCommand.ExecuteScalar()
                        'Executing formula field of child form
                        Dim FormulaQ = "select * FROM MMM_MST_FIELDS WHERE EID=" & EID & " AND DocumentType='" & ChildTargetdoc & "' AND FieldType='Formula Field' order by displayorder"
                        Dim DSF As New DataSet()
                        da.SelectCommand.Parameters.Clear()
                        da.SelectCommand.CommandText = FormulaQ
                        da.SelectCommand.CommandType = CommandType.Text
                        da.Fill(DSF)
                        'Ececuting Formula Fields
                        Try
                            Dim viewdoc As String = ChildTargetdoc
                            viewdoc = viewdoc.Replace(" ", "_")
                            If DSF.Tables(0).Rows.Count > 0 Then
                                For f As Integer = 0 To DSF.Tables(0).Rows.Count - 1
                                    Dim formulaeditorr As New formulaEditor
                                    Dim forvalue As String = String.Empty
                                    forvalue = formulaeditorr.ExecuteFormulaT(DSF.Tables(0).Rows(f).Item("KC_LOGIC"), CDocID, "v" + EID.ToString + viewdoc, EID, 0, con, trans)
                                    'Transactional Query
                                    Dim upquery As String = "update " & DSF.Tables(0).Rows(f).Item("DBTableName").ToString & "  set  " & DSF.Tables(0).Rows(f).Item("FieldMapping").ToString & "='" & forvalue.ToString & "'  where tid =" & CDocID & ""
                                    da.SelectCommand.CommandText = upquery
                                    da.SelectCommand.CommandType = CommandType.Text
                                    da.SelectCommand.ExecuteNonQuery()
                                Next
                            End If
                        Catch ex As Exception
                            Throw
                        End Try
                    Next
                Next
            End If
        Catch ex As Exception
            Throw
        End Try
        Return ret
    End Function

    Private Function GenerateQuery(EID As Integer, DocDBTableName As String, DocumentType As String, DOCID As Integer, dtFields As DataTable, DR As DataRow, DRS As DataRow, lstParent As List(Of UserData), Optional lstChild As List(Of UserData) = Nothing, Optional uid As Integer = 0) As String
        Dim ret As String = ""
        Try
            Dim strColumn As New StringBuilder()
            Dim strValue As New StringBuilder()
            Dim StrQuery As String = ""
            Dim TfldMapping As String = "TfldMapping"
            Dim SfldMapping As String = "SfldMapping"
            Dim MappingTypeCol As String = "MType"
            Dim MappingType As String = "COPY"
            Dim ColValue As String = ""
            Dim MappingSource As String = "Primary"
            Dim IsChild As Boolean = False
            Dim ObjF As New FormulaEngine()
            If (DocDBTableName = "DOCUMENT") Then
                strColumn.Append("INSERT INTO MMM_MST_DOC(EID,Documenttype,adate,Source,ouid")
                strValue.Append("VALUES" & "(" & EID & ",'" & DocumentType & "',getdate(),'Schedul'," & uid)
            ElseIf (DocDBTableName = "MASTER") Then
                strColumn.Append("INSERT INTO MMM_MST_MASTER(EID,Documenttype,adate,Source")
                strValue.Append("VALUES (" & EID & ",'" & DocumentType & "',getdate(),'Schedul'")
            ElseIf (DocDBTableName = "USER") Then
                strColumn.Append("INSERT INTO MMM_MST_USER(EID,ModiFyDate,LocationID")
                strValue.Append("VALUES (" & EID & " ,getdate(),'2072'")
            ElseIf (DocDBTableName = "CHILD") Then
                strColumn.Append("INSERT INTO MMM_MST_DOC_Item(Documenttype,DOCID")
                strValue.Append("VALUES ('" & DocumentType & "'," & DOCID)
                TfldMapping = "ChildTargetfldMapping"
                SfldMapping = "ChildSourcefldMapping"
                MappingTypeCol = "MappingType"
                IsChild = True
            End If
            For j As Integer = 0 To dtFields.Rows.Count - 1
                strColumn.Append("," & Convert.ToString(dtFields.Rows(j).Item(TfldMapping)))
                MappingType = Convert.ToString(dtFields.Rows(j).Item(MappingTypeCol))
                MappingSource = Convert.ToString(dtFields.Rows(j).Item("MappingSource"))

                If (MappingType.ToUpper = "COPY") Then
                    If (MappingSource.Trim.ToUpper = "SECONDARY") Then
                        ColValue = Convert.ToString(DRS.Item(dtFields.Rows(j).Item(SfldMapping)))
                    Else
                        ColValue = Convert.ToString(DR.Item(dtFields.Rows(j).Item(SfldMapping)))
                    End If
                ElseIf (MappingType = "FIX") Then
                    ColValue = Convert.ToString(dtFields.Rows(j).Item(SfldMapping))
                ElseIf (MappingType = "FORMULA") Then
                    'ExecuteFormula(Formula As String, EID As Integer, lstParentField As List(Of UserData), lstChildField As List(Of UserData), IsChildForm As Boolean, Optional BaseDocType As String = "")
                    ColValue = ObjF.ExecuteFormula(dtFields.Rows(j).Item(SfldMapping), EID, lstParent, lstChild, IsChild)
                End If
                strValue.Append(",'" & ColValue & "'")
            Next
            StrQuery = "SET NOCOUNT ON;" & strColumn.Append(")").ToString + strValue.Append(")").ToString & ";select Scope_identity()"
            ret = StrQuery.Trim
        Catch ex As Exception
            Throw
        End Try
        Return ret
    End Function

    Private Function GetFieldMappingSetting(newval As Integer) As DataSet
        Try
            Dim ds As New DataSet()
            Dim Qry As String = "SELECT * from mmm_schdoc_dtl where MTid=" & newval & ""
            Using con As SqlConnection = New SqlConnection(conStr)
                Using da As New SqlDataAdapter(Qry, con)
                    da.Fill(ds)
                End Using
            End Using
            Return ds
        Catch ex As Exception
            Throw
        End Try
    End Function

    Private Function GetFieldMappingSettingofChild(ConfigID As Integer) As DataSet
        Try
            Dim ds As New DataSet()
            Dim qry As String = ""
            'getting defination of a child needs to be created
            qry = "SELECT Distinct ChildSourcedoc,childTargetdoc FROM mmm_schdoc_child where EID=" & EID & " AND Ctid=" & ConfigID & ";SELECT * FROM mmm_schdoc_child where EID=" & EID & " AND Ctid=" & ConfigID
            Using con As SqlConnection = New SqlConnection(conStr)
                Using da As New SqlDataAdapter(qry, con)
                    da.Fill(ds)
                End Using
            End Using
            Return ds
        Catch ex As Exception
            Throw
        End Try
    End Function

    Private Function CreateList(DR As DataRow, DocType As String) As List(Of UserData)
        Dim dt As New DataTable()
        Dim lstParentField As New List(Of UserData)
        Try
            Dim dsfields As New DataSet()
            dsfields = GetFormFields(EID, DocType)
            For f As Integer = 0 To dsfields.Tables(0).Rows.Count - 1
                Dim objuser As New UserData
                objuser.FieldID = Convert.ToString(dsfields.Tables(0).Rows(f).Item("FieldID"))
                objuser.FieldType = Convert.ToString(dsfields.Tables(0).Rows(f).Item("FieldType"))
                objuser.MinVal = Convert.ToString(dsfields.Tables(0).Rows(f).Item("MinLen"))
                objuser.MaxVal = Convert.ToString(dsfields.Tables(0).Rows(f).Item("MaxLen"))
                objuser.IsRequired = Convert.ToString(dsfields.Tables(0).Rows(f).Item("isRequired"))
                objuser.DisplayName = Convert.ToString(dsfields.Tables(0).Rows(f).Item("displayName"))
                objuser.FieldMapping = Convert.ToString(dsfields.Tables(0).Rows(f).Item("FieldMapping"))

                objuser.Values = Convert.ToString(DR.Item(Convert.ToString(dsfields.Tables(0).Rows(f).Item("FieldMapping"))))

                objuser.DataType = Convert.ToString(dsfields.Tables(0).Rows(f).Item("datatype"))
                objuser.DropDownType = Convert.ToString(dsfields.Tables(0).Rows(f).Item("DropDownType"))

                objuser.CalText = Convert.ToString(dsfields.Tables(0).Rows(f).Item("cal_text"))
                objuser.LookUp = Convert.ToString(dsfields.Tables(0).Rows(f).Item("lookupvalue"))
                objuser.DropDown = Convert.ToString(dsfields.Tables(0).Rows(f).Item("dropdown"))
                'objuser.DDLValue = dt.Rows(f).Item("dropdown")
                objuser.IsUnique = Convert.ToString(dsfields.Tables(0).Rows(f).Item("isunique"))
                objuser.AutoFilter = Convert.ToString(dsfields.Tables(0).Rows(f).Item("AutoFilter"))
                objuser.DisplayOrder = Convert.ToString(dsfields.Tables(0).Rows(f).Item("displayOrder"))
                objuser.EnableEdit = Convert.ToString(dsfields.Tables(0).Rows(f).Item("EnableEdit"))

                objuser.IsEditOnamend = Convert.ToString(dsfields.Tables(0).Rows(f).Item("iseditonAmend"))
                objuser.EnableEdit = Convert.ToString(dsfields.Tables(0).Rows(f).Item("EnableEdit"))
                objuser.DocumentType = Convert.ToString(dsfields.Tables(0).Rows(f).Item("DocumentType"))
                lstParentField.Add(objuser)
            Next

        Catch ex As Exception

        End Try
        Return lstParentField
    End Function

    Public Shared Function GetFormFields(EID As Integer, Optional Doctype As String = "", Optional FieldID As Integer = 0) As DataSet
        Dim dsFields As New DataSet()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim strQuery = ""
        Dim con As SqlConnection = Nothing
        Dim sda As SqlDataAdapter = Nothing
        Try
            strQuery = "SELECT FF.*,F.*  FROM MMM_MST_FIELDS FF left outer JOIN MMM_MST_FORMS F on F.FormName = FF.DocumentType and F.EID = FF.EID where  F.EID=" & EID & " and DocumentType='" & Doctype & "' "
            'If FieldID <> 0 Then
            'strQuery = strQuery & " AND FF.FieldID= " & FieldID
            'Else
            'strQuery = strQuery & " AND FF.FieldID= " & FieldID & "AND FF.isactive=1 "
            'End If
            '  If Doctype <> "" Then
            'strQuery = strQuery & "and FormName = '" & Doctype & "'"
            'End If
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

    Public Function Execute(Optional DOCID As String = "0") As String
        Dim con As New SqlConnection(conStr)
        Dim trans As SqlTransaction = Nothing
        Try
            Dim ret As String = ""
            'Get setting of scheduled document
            Dim dsSet As New DataSet()
            'This code will be used from sceduler
            'Please Note.
            '1.In case of scheduler EID,DOCID,DocumentType will not be supplyed
            '2.In case of other then scheduler EID,DOCID,DocumentType will  be supplyed from the respective Master,Document,Docdetail page
            '3.EID,Source,CreateEvent will be set using constructor
            dsSet = GetDynamicDocument()
            Try
                If (dsSet.Tables(0).Rows.Count > 0) Then
                    'There Could be more then one setting for single document or schedule
                    For i As Integer = 0 To dsSet.Tables(0).Rows.Count - 1
                        Dim EID = Convert.ToString(dsSet.Tables(0).Rows(i).Item("EID"))
                        Dim IsValidSetting = True
                        Dim CEvent As String = ""
                        CEvent = Convert.ToString(dsSet.Tables(0).Rows(i).Item("CreateEvent").ToString.Trim)
                        Dim NumberOfRows = 0
                        'In the case of schedule document time should be checked, for other case time can be ignored
                        If (CEvent.Trim.ToUpper = "SCHEDULE") Then
                            Dim Time As String = ""
                            Time = Convert.ToString(dsSet.Tables(0).Rows(i).Item("ScheduleTime").ToString.Trim())
                            If (IsValidScheduleTime(Time)) Then
                                IsValidSetting = True
                            Else
                                IsValidSetting = False
                            End If
                        End If
                        If (IsValidSetting = True) Then
                            Dim SourceDoc As String = ""
                            Dim SDocType As String = ""
                            Dim SecDoc As String = ""
                            Dim SecDocType As String = ""
                            Dim TrgDoc As String = ""
                            Dim TrgDocType As String = ""
                            Dim RowFilterCondition As String = ""
                            Dim SecDocFilterCondion As String = ""
                            Dim SecWhereCond As String = ""
                            Dim CreatorSource As String = ""
                            SourceDoc = Convert.ToString(dsSet.Tables(0).Rows(i).Item("SourceDoc").ToString.Trim())
                            SDocType = Convert.ToString(dsSet.Tables(0).Rows(i).Item("SQType").ToString.Trim())
                            SecDoc = Convert.ToString(dsSet.Tables(0).Rows(i).Item("SecondarySourceDoc").ToString.Trim())
                            SecDocType = Convert.ToString(dsSet.Tables(0).Rows(i).Item("SecondarySQType").ToString.Trim())
                            TrgDoc = Convert.ToString(dsSet.Tables(0).Rows(i).Item("TargetDoc").ToString.Trim())
                            TrgDocType = Convert.ToString(dsSet.Tables(0).Rows(i).Item("TQType").ToString.Trim())
                            CreatorSource = Convert.ToString(dsSet.Tables(0).Rows(i).Item("CreatorSource").ToString.Trim())
                            SecDocFilterCondion = Convert.ToString(dsSet.Tables(0).Rows(i).Item("filterFields").ToString.Trim())
                            SecWhereCond = Convert.ToString(dsSet.Tables(0).Rows(i).Item("SecConditionFields").ToString.Trim())
                            RowFilterCondition = Convert.ToString(dsSet.Tables(0).Rows(i).Item("ConditionFields").ToString.Trim())
                            'Generate Query for primary document
                            Dim PDocQuery As New StringBuilder()
                            Dim SDocQuery As New StringBuilder()
                            'Get TableName OF 
                            'Dim SourceDoctable As String = GetTableName(SDocType)
                            'Dim SecDoctable As String = GetTableName(SDocType)
                            'Generate query for primary doc table 
                            PDocQuery.Append(GenerateSelectQuery(SDocType, SourceDoc, "PRIMARY", RowFilterCondition, "", DOCID, EID))
                            'Generating Query for secondry relation
                            Dim dsFields As New DataSet()
                            If (Not String.IsNullOrEmpty(SecDoc)) Then
                                'Create Filter For Secondory Document
                                'Get Secondory doc filter setting
                                Dim FldIds As String = SecDocFilterCondion.Trim.TrimEnd(" ", "").TrimEnd(",", "").Replace("-", ",")
                                Dim FldQry As String = "SELECT FieldMapping,FieldID,DocumentType,DisplayName,FieldType,dropdown FROM MMM_MST_FIELDS  WHERE EID=" & EID & " AND FieldID in(" & FldIds & ")"
                                dsFields = ExecuteDataset(FldQry)
                                'PDocQuery.Append(GenerateSelectQuery(SecDocType, SecDoc, "secondary", SecDocFilterCondion, "", DOCID, EID))
                            End If
                            'Getting source document
                            Dim dsSource As New DataSet()
                            dsSource = ExecuteDataset(PDocQuery.ToString.Trim)
                            If (dsSource.Tables(0).Rows.Count > 0) Then
                                'Get all the source document. loop through all rows 
                                con = New SqlConnection(conStr)
                                For j As Integer = 0 To dsSource.Tables(0).Rows.Count - 1
                                    'Finding UID of creater
                                    'If doctype = "DOCUMENT" Then
                                    '    UID = Convert.ToString(DSdoc.Tables(0).Rows(0).Item(DS.Tables(0).Rows(0).Item("RoleName")))
                                    'End If
                                    'If any setting exists for secondory relation
                                    Dim dsSecource As New DataSet()
                                    If (Not String.IsNullOrEmpty(SecDoc)) Then
                                        'Create Filter For Secondory Document
                                        'Get Secondory doc filter setting
                                        SecDocFilterCondion = SecDocFilterCondion.Trim.TrimEnd(" ", "").TrimEnd(",", "")
                                        Dim arrMap = SecDocFilterCondion.Split(",")
                                        Dim SecWhere As New StringBuilder()
                                        Dim SecDocQry As New StringBuilder()
                                        'Creating where Condition for secondory relation doc
                                        For k As Integer = 0 To arrMap.Length - 1
                                            Dim arrFldIds = arrMap(k).Split("-")
                                            Dim pflds As String = ""
                                            Dim sflds As String = ""
                                            Dim pRows() As DataRow = dsFields.Tables(0).Select("FieldID= " & arrFldIds(0).Trim())
                                            Dim sRows() As DataRow = dsFields.Tables(0).Select("FieldID= " & arrFldIds(1).Trim())
                                            pflds = pRows(0).Item("FieldMapping")
                                            sflds = sRows(0).Item("FieldMapping")
                                            SecWhere.Append(" AND ").Append(sflds).Append(" ='" & dsSource.Tables(0).Rows(j).Item(pflds) & "'")
                                        Next
                                        SecDocQry.Append(GenerateSelectQuery(SecDocType, SecDoc, "secondary", "", SecWhere.ToString, DOCID, EID))
                                        'if where condition on secondry doc exist append it
                                        If Not (String.IsNullOrEmpty(SecWhereCond)) Then
                                            SecDocQry.Append(" AND ").Append(SecWhereCond)
                                        End If
                                        'fill secondory doc in the dataset
                                        dsSecource = ExecuteDataset(SecDocQry.ToString.Trim)
                                    End If
                                    'Creating list for formula execution 
                                    Dim CDocID As Integer = 0
                                    Dim UID As Integer = 0
                                    'Get UID Means creater of document
                                    'Creater doc may resisde on primary or secondory doc
                                    If (CreatorSource.Trim.ToUpper = "PRIMARY") Then
                                        UID = dsSource.Tables(0).Rows(j).Item(dsSet.Tables(0).Rows(i).Item("RoleName"))
                                    Else
                                        UID = dsSecource.Tables(0).Rows(0).Item(dsSet.Tables(0).Rows(i).Item("RoleName"))
                                    End If

                                    Dim lstParentField As New List(Of UserData)
                                    lstParentField = CreateList(dsSource.Tables(0).Rows(0), SourceDoc)
                                    'Getting field mapping setting of scheduled document from database
                                    Dim dsflds = GetFieldMappingSetting(dsSet.Tables(0).Rows(i).Item("TID"))
                                    Dim StrQuery As String = ""
                                    Dim drs As DataRow
                                    'Please Note:Secondory document might not available for all setting
                                    If (dsSecource.Tables.Count > 0) Then
                                        drs = dsSecource.Tables(0).Rows(0)
                                    End If
                                    'Generating Insert statement based on field mapping setting
                                    StrQuery = GenerateQuery(EID, TrgDocType, TrgDoc, 0, dsflds.Tables(0), dsSource.Tables(0).Rows(j), drs, lstParentField, Nothing, UID)
                                    Dim da As New SqlDataAdapter(StrQuery, con)
                                    con.Open()
                                    trans = con.BeginTransaction()
                                    da.SelectCommand.Transaction = trans
                                    da.SelectCommand.CommandText = StrQuery
                                    da.SelectCommand.Connection = con
                                    CDocID = da.SelectCommand.ExecuteScalar()
                                    'Code for creating child document
                                    CreateChildDoc(dsSet.Tables(0).Rows(i).Item("TID"), DOCID, CDocID, lstParentField, con, trans)
                                    'Execute Different Patches based on documentType and setting
                                    Dim objDocUtils As New DocUtils()
                                    'Executing Comman patch.
                                    'That is comman for Document,Master,User
                                    '1.Formula Execution
                                    '2.Auto Number Execution
                                    '3.Trigger Execution
                                    objDocUtils.ExecuteCommanModule(EID, TrgDoc, TrgDocType, CDocID, UID, con, trans)
                                    'Executing Document patch.
                                    'It Includes 
                                    '1.Document movement code
                                    '2.Push Mapping
                                    If (TrgDocType.ToUpper = "DOCUMENT") Then
                                        objDocUtils.ExecuteDocumentModuleModule(EID, TrgDoc, TrgDocType, CDocID, UID, con, trans)
                                    End If
                                    'Running User Patch
                                    'This module will take care Duplicacy check of User Table based on their Login field set on entity level
                                    If (TrgDocType.ToUpper = "USER") Then
                                        objDocUtils.ExecuteUserModule(EID, TrgDoc, TrgDocType, CDocID, UID, con, trans)
                                    End If
                                    'All transactional activity accomplished 
                                    trans.Commit()
                                    con.Close()
                                    'Executing other module
                                    'Other module is collection of
                                    '1.mail sending
                                    '2.Calling web service for pushing data to other web service
                                    Try
                                        objDocUtils.ExecuteOtherUtils(EID, TrgDoc, TrgDocType, CDocID, UID)
                                    Catch ex As Exception
                                        'Please note.Not throwing any exception from this block because it wont impact any BPM Activity
                                    End Try
                                    'Executing User Module
                                    'This module will used for
                                    '1.Sending well come mail to user for user activation
                                    'update MMM_MST_FIELDS SET Lookupvalue='7311-fld1,1112-fld11,11136-S,7352-S,11933-fld14,' where EID=46 AND FieldID=7310
                                    If (TrgDocType.ToUpper = "USER") Then
                                        Try
                                            ''Change Required we will optimise it n future
                                            Dim DMS As New DMSUtil
                                            DMS.notificationMail(CDocID, EID, "USER", "USER CREATED")
                                        Catch ex As Exception
                                        End Try
                                    End If
                                Next
                            End If
                        End If
                    Next
                Else
                    ret = "No Any Setting Exists."
                End If
            Catch ex As Exception
            End Try
            Return ret
        Catch ex As Exception
            trans.Rollback()
            Throw New Exception("Error occured in scheduled document.")
        Finally
            con.Close()
            con.Dispose()
        End Try
    End Function

    Private Function ExecuteDataset(qry As String) As DataSet
        Try
            Dim ds As New DataSet()
            Using con As New SqlConnection(conStr)
                Using da As New SqlDataAdapter(qry, con)
                    da.Fill(ds)
                End Using
            End Using
            Return ds
        Catch ex As Exception
            Throw
        End Try
    End Function

    Private Function GenerateSelectQuery(formType As String, DocumentType As String, RelationType As String, PCond As String, SCond As String, DOCID As Integer, EID As Integer) As String

        Dim tableName As String = ""
        formType = formType.ToUpper.ToUpper
        Dim SbQuery As New StringBuilder()
        Select Case formType
            Case "USER"
                SbQuery.Append("SELECT * FROM MMM_MST_USER WHERE IsAuth='1' AND  EID= ").Append(EID)
                If (DOCID > 0) Then
                    SbQuery.Append(" AND UID=" & DOCID)
                End If
            Case "MASTER"
                SbQuery.Append("SELECT * FROM MMM_MST_MASTER WHERE   EID= ").Append(EID).Append(" AND DocumentType='" & DocumentType & "'")
                If (DOCID > 0) Then
                    SbQuery.Append(" AND tid=" & DOCID)
                End If
            Case "DOCUMENT"
                SbQuery.Append("SELECT * FROM MMM_MST_DOC WHERE   EID= ").Append(EID).Append(" AND DocumentType='" & DocumentType & "'")
                If (DOCID > 0) Then
                    SbQuery.Append(" AND tid=" & DOCID)
                End If
            Case "DETAIL"
                SbQuery.Append("SELECT * FROM MMM_MST_DOC_Item WHERE  DocumentType='" & DocumentType & "'").Append(" AND DOCID=").Append(DOCID)
        End Select
        If (RelationType.Trim.ToUpper = "PRIMARY" And PCond.Trim <> "") Then
            SbQuery.Append(" AND ").Append(PCond)
        End If
        If (RelationType.Trim.ToUpper = "SECONDARY" And SCond.Trim <> "") Then
            SbQuery.Append("  ").Append(SCond)
        End If
        Return SbQuery.ToString.Trim

    End Function

    Private Function GetTableName(formname As String) As String
        Dim tableName As String = ""
        formname = formname.ToUpper.ToUpper
        Select Case formname
            Case "USER"
                tableName = "MMM_MST_USER"
            Case "MASTER"
                tableName = "MMM_MST_MASTER"
            Case "DOCUMENT"
                tableName = "MMM_MST_DOC"
            Case "DETAIL"
                tableName = "MMM_MST_DOC_ITEM"
        End Select
        Return tableName
    End Function

    Function IsValidScheduleTime(Time As String) As Boolean
        Dim b As Boolean = False
        Try
            Dim time1 As DateTime = Now.AddMinutes(-5).ToShortTimeString()
            Dim time2 As DateTime = Now.AddMinutes(+5).ToShortTimeString()
            Dim i As Integer = 0
            ' Dim date1f As String 
            If (Time <> "") Then
                'Dim schedule As String = "*|*|*|*|*"
                Dim schedule As String = Time
                Dim str_array As [String]() = schedule.Split("|")
                Dim stringa As [String] = str_array(0)
                Dim stringb As [String] = str_array(1)
                Dim stringc As [String] = str_array(2)
                Dim stringd As [String] = str_array(3)
                Dim stringe As [String] = str_array(4)
                Dim currentTime As System.DateTime = System.DateTime.Now
                Dim currentdate As String = currentTime.Date.Day
                Dim str_datte As [String]() = stringb.Split(",")
                Dim str_hours As [String]() = stringd.Split(",")
                Dim str_mintus As [String]() = stringe.Split(",")
                If Trim(stringa) = "*" And Trim(stringb) = "*" And Trim(stringc) = "*" And Trim(stringd) = "*" And Trim(stringe) = "*" Then
                    b = True
                    Return b
                    ' Exit For
                End If
                If stringb <> "*" Then
                    For j As Integer = 0 To str_datte.Length - 1
                        Dim A As [String] = str_datte(j)
                        If A.Contains("-") Then
                            Dim o As [String]() = A.Split("-")
                            Dim f As [String] = o(0)
                            Dim g As [String] = o(1)
                            If (f <= currentdate And g >= currentdate) Then
                                For n As Integer = 0 To str_hours.Length - 1
                                    For m As Integer = 0 To str_mintus.Length - 1
                                        Dim x As DateTime = (Convert.ToDateTime(str_hours(n) & ":" & str_mintus(m) & ":" & "00").ToShortTimeString)
                                        If x <= time2 And x >= time1 Then
                                            b = True
                                            Exit For
                                        End If
                                    Next
                                Next
                            End If
                        Else
                            If ((currentdate = A)) Then
                                For n As Integer = 0 To str_hours.Length - 1
                                    For m As Integer = 0 To str_mintus.Length - 1
                                        Dim x As DateTime = (Convert.ToDateTime(str_hours(n) & ":" & str_mintus(m) & ":" & "00").ToShortTimeString)
                                        If x <= time2 And x >= time1 Then
                                            b = True
                                            Exit For
                                        End If
                                    Next
                                Next
                            End If
                        End If
                    Next
                ElseIf ((currentdate = stringb) Or (stringb.Trim() = "*") Or (stringd.Trim() <> "") Or (stringe.Trim() <> "")) Then
                    For n As Integer = 0 To str_hours.Length - 1
                        For m As Integer = 0 To str_mintus.Length - 1
                            Dim x As DateTime = (Convert.ToDateTime(str_hours(n) & ":" & str_mintus(m) & ":" & "00").ToShortTimeString)
                            '  Dim x As DateTime = (Convert.ToDateTime(stringd & ":" & stringe & ":" & "00").ToShortTimeString)
                            If x <= time2 And x >= time1 Then
                                b = True
                                Exit For
                            End If
                        Next
                    Next
                ElseIf ((stringa.Trim() = "*") Or (stringb.Trim() = "*") Or (stringc.Trim() = "*") Or (stringd.Trim() = "*") Or (stringe.Trim() = "*")) Then
                    b = True
                    'Exit For
                End If
            End If
            Return b
        Catch ex As Exception
            Return False
        End Try
    End Function


End Class
