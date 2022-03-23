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

Public Class Relation

    Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString

    Public Function GetRelationDocType(eid As Integer, DocumentType As String) As DataSet
        Dim ds As New DataSet()
        Try
            Using con = New SqlConnection(conStr)
                Using da = New SqlDataAdapter("GetRelationDocType", con)
                    da.SelectCommand.CommandType = CommandType.StoredProcedure
                    da.SelectCommand.Parameters.AddWithValue("@eid", eid)
                    da.SelectCommand.Parameters.AddWithValue("@DocumentType", DocumentType)
                    da.Fill(ds)
                    For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                        Dim arr = ds.Tables(0).Rows(i).Item("Dropdown").ToString.Split("-")
                        ds.Tables(0).Rows(i).Item("Dropdown") = arr(1)
                    Next
                End Using
            End Using
        Catch ex As Exception
            Throw
        End Try
        Return ds
    End Function

    Public Function GetRelation(eid As Integer, TDOCType As String) As DataSet
        Dim ds As New DataSet()
        Try
            Using con = New SqlConnection(conStr)
                Using da = New SqlDataAdapter("getRelation", con)
                    da.SelectCommand.CommandType = CommandType.StoredProcedure
                    da.SelectCommand.Parameters.AddWithValue("@eid", eid)
                    da.SelectCommand.Parameters.AddWithValue("@T_DOC", TDOCType)
                    da.Fill(ds)
                End Using
            End Using
        Catch ex As Exception
            Throw
        End Try
        Return ds
    End Function

    Private Function GetExecutableRelation(eid As Integer, CreatedDocType As String) As DataSet
        Dim ds As New DataSet()
        Try
            Using con = New SqlConnection(conStr)
                Using da = New SqlDataAdapter("GetExecutableRelation", con)
                    da.SelectCommand.CommandType = CommandType.StoredProcedure
                    da.SelectCommand.Parameters.AddWithValue("@eid", eid)
                    da.SelectCommand.Parameters.AddWithValue("@S_DOC", CreatedDocType)
                    da.Fill(ds)
                End Using
            End Using
        Catch ex As Exception
            Throw
        End Try
        Return ds
    End Function

    Public Function GetExecutableRelationTDocType(eid As Integer, CreatedDocType As String) As RelationResponse

        Dim objRet As New RelationResponse()
        Dim dsRel As New DataSet()
        Try
            dsRel = GetExecutableRelation(eid, CreatedDocType)
            If dsRel.Tables(0).Rows.Count > 0 Then
                Dim T_DOCType = "", T_DOC = "", S_DOC1 = "", S_DOC2 = ""
                T_DOCType = dsRel.Tables(0).Rows(0).Item("T_DOCTYPE")
                T_DOC = dsRel.Tables(0).Rows(0).Item("T_DOC")
                S_DOC1 = dsRel.Tables(0).Rows(0).Item("S_DOC1")
                S_DOC2 = dsRel.Tables(0).Rows(0).Item("S_DOC2")
                Dim ToBePubDoc = ""
                If S_DOC1 <> CreatedDocType Then
                    ToBePubDoc = S_DOC1
                Else
                    ToBePubDoc = S_DOC2
                End If
                objRet.Success = True
                objRet.SDocType = ToBePubDoc
            Else
                objRet.Success = True
                objRet.Message = "No Any relation configured"
            End If
        Catch ex As Exception
            Throw
        End Try
        Return objRet
    End Function

    Public Function GetAllFields(EID As Integer, Optional Documenttype As String = "", Optional EnableEdit As String = "") As DataSet
        Dim ds As New DataSet()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Try
            'F.IsEditable, F.defaultfieldval ,F.datatype,
            Dim Query = "SET NOCOUNT ON;select F.FieldID,F.IsEditable, F.defaultfieldval ,F.EnableEdit,F.OUTWARDXMLTAGNAME,F.datatype,F.FieldType,F.DDllookupvalue ,F.FieldMapping,F.FieldID,F.DropDown,F.lookupvalue,F.DROPDOWNTYPE,F.DisplayName ,F.DocumentType,FF.FormSource,FF.EventName  FROM MMM_MST_FIELDS F INNER JOIN MMM_MST_FORMS FF ON FF.FormName=F.DocumentType WHERE F.EID= " & EID & " AND FF.EID= " & EID & ""
            If Documenttype <> "" Then
                Query = Query & " AND F.IsActive=1 AND F.DocumentType='" & Documenttype & "'"
            End If
            If EnableEdit <> "" Then
                Query = Query & " AND EnableEdit=" & EnableEdit & ""
            End If
            Using con = New SqlConnection(conStr)
                Using da = New SqlDataAdapter(Query, con)
                    da.Fill(ds)
                End Using
            End Using
        Catch ex As Exception
            Throw
        End Try
        Return ds
    End Function


    Public Function AddUpdateRelation(eid As Integer, Name As String, Type As String, T_DOC As String, S_DOC1 As String, S_DOC2 As String, ShowExtend As String, TID As Integer, IsActive As String, RQ As String, BMODE As String) As Integer
        Dim ret As Integer = 0
        Try
            Using con = New SqlConnection(conStr)
                Using da = New SqlDataAdapter("addUpdateRelation", con)
                    con.Open()
                    da.SelectCommand.CommandType = CommandType.StoredProcedure
                    da.SelectCommand.Parameters.AddWithValue("@eid", eid)
                    da.SelectCommand.Parameters.AddWithValue("@Name", Name)
                    da.SelectCommand.Parameters.AddWithValue("@Type", Type)
                    da.SelectCommand.Parameters.AddWithValue("@T_DOCType", "")
                    da.SelectCommand.Parameters.AddWithValue("@T_DOC", T_DOC)
                    da.SelectCommand.Parameters.AddWithValue("@S_DOC1", S_DOC1)
                    da.SelectCommand.Parameters.AddWithValue("@S_DOC2", S_DOC2)
                    da.SelectCommand.Parameters.AddWithValue("@ShowExtend", ShowExtend)
                    da.SelectCommand.Parameters.AddWithValue("@TID", TID)
                    da.SelectCommand.Parameters.AddWithValue("@RQ", RQ)
                    da.SelectCommand.Parameters.AddWithValue("@BMODE", BMODE)
                    'RQ BMODE
                    da.SelectCommand.Parameters.AddWithValue("@IsActive", IsActive)
                    ret = da.SelectCommand.ExecuteScalar()
                End Using
            End Using
        Catch ex As Exception
            Throw
        End Try
        Return ret
    End Function

    'Public Function UpDateBalINFO(EID As String, FrmID As String, Balance_Maintenance_Mode As String, Effective_Date_Field As String, Balance_Field As String, Item_Number As String) As Integer
    '    Dim ret As Integer = 0
    '    Try
    '        Using con = New SqlConnection(conStr)
    '            Using da = New SqlDataAdapter("UpDateBalINFO", con)
    '                con.Open()
    '                da.SelectCommand.CommandType = CommandType.StoredProcedure
    '                da.SelectCommand.Parameters.AddWithValue("@eid", EID)
    '                da.SelectCommand.Parameters.AddWithValue("@FormID", FrmID)
    '                da.SelectCommand.Parameters.AddWithValue("@Balance_Maintenance_Mode", Balance_Maintenance_Mode)
    '                da.SelectCommand.Parameters.AddWithValue("@Effective_Date_Field", Effective_Date_Field)
    '                da.SelectCommand.Parameters.AddWithValue("@Balance_Field", Balance_Field)
    '                da.SelectCommand.Parameters.AddWithValue("@Item_Number", Item_Number)
    '                ret = da.SelectCommand.ExecuteNonQuery()
    '            End Using
    '        End Using
    '    Catch ex As Exception
    '        Throw
    '    End Try
    '    Return ret
    'End Function


    'Changes By Mayank 24-jan-2015
    Public Function UpDateBalINFO(EID As String, FrmID As String, Effective_Date_Field As String, Balance_Field As String, Item_Number As String, Relation_Doc_Type As String, Action As String, S_T_DDN As String, Relation_Type As String) As Integer
        Dim ret As Integer = 0
        Try
            Using con = New SqlConnection(conStr)
                Using da = New SqlDataAdapter("UpDateBalINFO", con)
                    con.Open()
                    da.SelectCommand.CommandType = CommandType.StoredProcedure
                    da.SelectCommand.Parameters.AddWithValue("@eid", EID)
                    da.SelectCommand.Parameters.AddWithValue("@FormID", FrmID)
                    da.SelectCommand.Parameters.AddWithValue("@Balance_Type", Action)
                    da.SelectCommand.Parameters.AddWithValue("@Effective_Date_Field", Effective_Date_Field)
                    da.SelectCommand.Parameters.AddWithValue("@Balance_Field", Balance_Field)
                    da.SelectCommand.Parameters.AddWithValue("@Item_Number", Item_Number)
                    da.SelectCommand.Parameters.AddWithValue("@Relation_Doc_Type", Relation_Doc_Type)
                    da.SelectCommand.Parameters.AddWithValue("@S_T_DDN", S_T_DDN)
                    da.SelectCommand.Parameters.AddWithValue("@Relation_Type", Relation_Type)
                    ret = da.SelectCommand.ExecuteNonQuery()
                End Using
            End Using
        Catch ex As Exception
            Throw
        End Try
        Return ret
    End Function


    Public Function ExtendRelation(eid As Integer, CreatedDocType As String, DOCID As Integer, UID As Integer, DOCList As String, ForceExtend As Boolean) As RelationResponse
        Dim ret As New RelationResponse()
        Dim dsRel As New DataSet()
        Dim con As New SqlConnection(conStr)
        Dim da As SqlDataAdapter
        Dim tran As SqlTransaction = Nothing
        Try
            'Getting All the Relations
            dsRel = GetExecutableRelation(eid, CreatedDocType)
            If dsRel.Tables(0).Rows.Count > 0 Then
                Dim T_DOCType = "", T_DOC = "", S_DOC1 = "", S_DOC2 = ""
                Dim ShowExtend As Boolean = False
                T_DOCType = dsRel.Tables(0).Rows(0).Item("T_DOCTYPE")
                T_DOC = dsRel.Tables(0).Rows(0).Item("T_DOC")
                S_DOC1 = dsRel.Tables(0).Rows(0).Item("S_DOC1")
                S_DOC2 = dsRel.Tables(0).Rows(0).Item("S_DOC2")
                If Convert.ToString(dsRel.Tables(0).Rows(0).Item("ShowExtend")) = "1" Then
                    ShowExtend = True
                End If
                If ShowExtend = False Or ForceExtend = True Then
                    Dim TbepubDoc = ""
                    If S_DOC1 <> CreatedDocType Then
                        TbepubDoc = S_DOC1
                    Else
                        TbepubDoc = S_DOC2
                    End If
                    Dim SyncFilterCond As String = FilterConditiononRelation(eid, CreatedDocType, TbepubDoc, DOCID)

                    Dim vName = ""
                    Dim Query = ""
                    vName = "V" & eid & TbepubDoc.Trim.Replace(" ", "_")
                    Query = "SELECT tid, DocumentType FROM " & vName
                    If Query.Trim <> "" Then
                        If SyncFilterCond.Length > 2 Then
                            If DOCList <> "" Then
                                Query = Query & " where [" & vName & "].tid in (" & DOCList & ") and " & SyncFilterCond
                            Else
                                Query = Query & " where [" & vName & "].tid in (SELECT tid FROM " & vName & ") and " & SyncFilterCond
                            End If

                        Else
                            If DOCList <> "" Then
                                Query = Query & " where [" & vName & "].tid in (" & DOCList & ")"
                            Else
                                Query = Query & " where [" & vName & "].tid in (SELECT tid FROM " & vName & ")"
                            End If

                        End If
                    End If

                    'Query = Query & Query
                    Dim dsDocList As New DataSet()
                    'Getting another Document 
                    Using con1 = New SqlConnection(conStr)
                        Using da1 As New SqlDataAdapter(Query, con1)
                            con1.Open()
                            da1.Fill(dsDocList)
                        End Using
                    End Using
                    Dim strColumn As New StringBuilder()
                    Dim strValue As New StringBuilder()
                    Dim StrChkUnique As New StringBuilder()
                    Dim StrQuery = ""
                    Dim fld As String = ""
                    Dim Value As String = ""
                    Dim FileID As Integer = 0
                    Dim SCount As Integer = 0
                    Dim DupCount As Integer = 0
                    'Insert into data base for relation extension 
                    If dsDocList.Tables(0).Rows.Count > 0 Then

                        Dim dsfield As New DataSet()
                        dsfield = GetAllFields(eid, T_DOC) 'GetFieldDef(eid, T_DOC)
                        Dim FieldType As String = "", IsEditable As String = "0"

                        'Lopp For all doc list
                        Dim ddlVal As String = ""
                        Dim DefaultVal As String = "", Datatype = ""
                        'Datetime
                        If con.State <> ConnectionState.Open Then
                            con.Open()
                        End If
                        tran = con.BeginTransaction()

                        For d As Integer = 0 To dsDocList.Tables(0).Rows.Count - 1
                            strColumn = New StringBuilder()
                            strValue = New StringBuilder()
                            StrChkUnique = New StringBuilder()
                            StrQuery = ""
                            If T_DOCType = "DOCUMENT" Then
                                StrChkUnique.Append("IF NOT  Exists(SELECT EID FROM MMM_MST_DOC WHERE EID=" & eid & " AND DocumentType='" & T_DOC & "'")
                                strColumn.Append("INSERT INTO MMM_MST_DOC(EID,Documenttype,oUID,adate,Source")
                                strValue.Append("VALUES (").Append(eid).Append(",'").Append(T_DOC).Append("',").Append(UID).Append(",getdate(),'RExt'")
                            Else
                                StrChkUnique.Append("IF NOT  Exists(SELECT EID FROM MMM_MST_Master WHERE EID=" & eid & " AND DocumentType='" & T_DOC & "'")
                                strColumn.Append("INSERT INTO MMM_MST_Master(EID,Documenttype,CreatedBy,UpdatedDate,Source")
                                strValue.Append(" VALUES").Append("(").Append(eid).Append(",'").Append(T_DOC).Append("',").Append(UID).Append(",getdate(),'RExt' ")
                            End If
                            'IsEditable, defaultfieldval ,datatype
                            For f As Integer = 0 To dsfield.Tables(0).Rows.Count - 1
                                Value = ""
                                IsEditable = Convert.ToString(dsfield.Tables(0).Rows(f).Item("IsEditable"))
                                DefaultVal = Convert.ToString(dsfield.Tables(0).Rows(f).Item("defaultfieldval"))
                                Datatype = Convert.ToString(dsfield.Tables(0).Rows(f).Item("datatype"))
                                Dim arr = Convert.ToString(dsfield.Tables(0).Rows(f).Item("DropDown")).Split("-")
                                FieldType = Convert.ToString(dsfield.Tables(0).Rows(f).Item("FieldType"))
                                Select Case FieldType
                                    Case "Drop Down"
                                        If String.IsNullOrEmpty(DefaultVal.Trim) Then
                                            Value = "0"
                                        Else
                                            Value = DefaultVal
                                        End If
                                    Case "LookupDDL"
                                        If String.IsNullOrEmpty(DefaultVal.Trim) Then
                                            Value = "0"
                                        Else
                                            Value = DefaultVal
                                        End If
                                End Select
                                'Seting current docid to concern document
                                If FieldType = "Drop Down" Then
                                    If arr(1).Trim.ToUpper = CreatedDocType.Trim.ToUpper Then
                                        Value = DOCID
                                        'Generating Query For Duplicacy check
                                        StrChkUnique.Append(" AND " & Convert.ToString(dsfield.Tables(0).Rows(f).Item("FieldMapping")) & " = '" & Value & "'")
                                    End If
                                    If arr(1).Trim.ToUpper = Convert.ToString(dsDocList.Tables(0).Rows(d).Item("DocumentType")).Trim.ToUpper Then
                                        Value = Convert.ToString(dsDocList.Tables(0).Rows(d).Item("tid"))
                                        StrChkUnique.Append(" AND " & Convert.ToString(dsfield.Tables(0).Rows(f).Item("FieldMapping")) & " = '" & Value & "'")
                                    End If
                                End If
                                If FieldType = "Text Box" And Datatype = "Datetime" And IsEditable = "0" Then
                                    Dim Result As String = ""
                                    Dim dt As Date = Date.Today
                                    Result = dt.ToString("dd/MM/yy", CultureInfo.InvariantCulture)
                                    Value = Result
                                End If
                                If Not String.IsNullOrEmpty(DefaultVal.Trim) And String.IsNullOrEmpty(Value.Trim) Then
                                    Value = DefaultVal
                                End If
                                strColumn.Append(",").Append(Convert.ToString(dsfield.Tables(0).Rows(f).Item("FieldMapping")))
                                Value = Value.Replace("'", "''")
                                strValue.Append(",'").Append(Value).Append("'")
                            Next
                            strColumn.Append(")")
                            strValue.Append(")")
                            'Ending if Exists Block
                            StrChkUnique.Append(") BEGIN ")
                            StrQuery = strColumn.ToString & strValue.ToString
                            StrQuery = StrQuery & ";Select @@identity"
                            StrQuery = StrChkUnique.ToString & StrQuery & " END" & " ELSE SELECT 0"
                            'Save loging is going on from here
                            da = New SqlDataAdapter(StrQuery, con)
                            da.SelectCommand.Transaction = tran
                            'Transactional Query
                            FileID = da.SelectCommand.ExecuteScalar()
                            If FileID > 0 Then
                                'Excecuting Auto Number

                                Dim AutoNumQ = "select * FROM MMM_MST_FIELDS WHERE EID=" & eid & " AND DocumentType='" & T_DOC & "' AND (FieldType='Auto Number' or Fieldtype='New Auto Number')"
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
                                            da.SelectCommand.Parameters.AddWithValue("FormType", T_DOCType)
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
                                            da.SelectCommand.Parameters.AddWithValue("FormType", T_DOCType)
                                            Dim an As String = da.SelectCommand.ExecuteScalar()
                                            da.SelectCommand.Parameters.Clear()
                                    End Select
                                Next
                                'Executing Formula Field Now
                                Dim FormulaQ = "select * FROM MMM_MST_FIELDS WHERE EID=" & eid & " AND DocumentType='" & T_DOC & "' AND FieldType='Formula Field' order by displayorder"
                                Dim DSF As New DataSet()
                                da.SelectCommand.Parameters.Clear()
                                da.SelectCommand.CommandText = FormulaQ
                                da.SelectCommand.CommandType = CommandType.Text
                                da.Fill(DSF)
                                'Ececuting Formula Fields
                                Try
                                    Dim viewdoc As String = T_DOC
                                    viewdoc = viewdoc.Replace(" ", "_")
                                    If DSF.Tables(0).Rows.Count > 0 Then
                                        For f As Integer = 0 To DSF.Tables(0).Rows.Count - 1
                                            Dim formulaeditorr As New formulaEditor
                                            Dim forvalue As String = String.Empty
                                            forvalue = formulaeditorr.ExecuteFormulaT(DSF.Tables(0).Rows(f).Item("KC_LOGIC"), FileID, "v" + eid.ToString + viewdoc, eid, 0, con, tran)
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

                                'Executing Workflow now
                                If T_DOCType.Trim.ToUpper = "DOCUMENT" Then
                                    Dim ob2 As New DMSUtil()
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
                                    res = ob2.GetNextUserFromRolematrixT(FileID, eid, UID, "", UID, con, tran)
                                    Dim sretMsgArr() As String = res.Split(":")
                                    'Code for Mapping
                                    If sretMsgArr(0) = "ARCHIVE" Then
                                        Dim Op As New Exportdata()
                                        'Transactional Query
                                        Op.PushdataT(FileID, sretMsgArr(1), eid, con, tran)
                                    End If

                                End If
                                'Excecuting Code For Triggers
                                Trigger.ExecuteTriggerT(T_DOC, eid, FileID, con, tran, 1, TriggerNature:="Create", FormType:="Document")

                                'Code for calling outward webservice
                                'Try
                                '    If FileID > 0 Then
                                '        Dim WSOUT As New WSOutward()
                                '        Dim WSOret = WSOUT.WBS(CreatedDocType, eid, FileID)
                                '    End If
                                'Catch ex As Exception
                                'End Try
                                'Dim ob1 As New DMSUtil()
                                'Try
                                '    'Change Required
                                '    ob1.TemplateCalling(FileID, eid, CreatedDocType, "CREATED")
                                'Catch ex As Exception
                                'End Try
                                'Try
                                '    da.SelectCommand.CommandType = CommandType.Text
                                '    da.SelectCommand.Parameters.Clear()
                                '    da.SelectCommand.CommandText = "select isworkflow from mmm_mst_doc where TID=" & FileID & " and eid='" & eid & "'"
                                '    Dim dt As New DataTable
                                '    da.Fill(dt)
                                '    If dt.Rows.SCount = 1 Then
                                '        If dt.Rows(0).Item(0).ToString = "1" Then
                                '            'Change Required
                                '            ob1.TemplateCalling(FileID, eid, CreatedDocType, "APPROVE")
                                '        End If
                                '    End If
                                'Catch ex As Exception
                                'End Try
                            End If
                            If FileID > 0 Then
                                SCount = SCount + 1
                            Else
                                DupCount = DupCount + 1
                            End If

                        Next
                        tran.Commit()
                        ret.Success = True
                        ret.Message = "Total Extended Relation Document:: Success count:- " & SCount & ".Duplicate Count:- " & DupCount
                        ret.ShowExtend = ShowExtend
                    Else
                        'This logic may be change later
                        ret.Success = True
                        ret.Message = "No any relation Exists."
                        ret.ShowExtend = False
                    End If
                Else
                    ret.Success = True
                    ret.Message = "Extension through new page."
                    ret.ShowExtend = True
                End If
            Else
                ret.Success = True
                ret.Message = "No any relation Exists."
                ret.ShowExtend = False
            End If
        Catch ex As Exception
            ret.Success = False
            ret.Message = "Error occured at server."
            ret.ShowExtend = False
            If Not tran Is Nothing Then
                tran.Rollback()
                tran.Dispose()
            End If
        End Try
        Return ret
    End Function


    Public Function FilterConditiononRelation(eid As Integer, SourceForm As String, TargetForm As String, RegTID As Integer) As String
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
        Dim dtA As New DataTable()
        Dim StrQry As String = ""
        Dim FilterCond As String = ""
        Try
            StrQry = "select * from mmm_TallySync_filter where eid=" & eid & " and sourceform='" & SourceForm & "' and targetform='" & TargetForm & "'"

            'oda.SelectCommand.CommandText = StrQry
            Using con1 = New SqlConnection(conStr)
                Using da1 As New SqlDataAdapter(StrQry, con1)
                    con1.Open()
                    da1.Fill(dtA)
                End Using
            End Using
            For i As Integer = 0 To dtA.Rows.Count - 1
                If dtA.Rows(i).Item("Operands").ToString().Trim() = "=" Then
                    StrQry = "select " & dtA.Rows(i).Item("sourceFields").ToString() & " from mmm_mst_master where eid=" & eid & " and documenttype='" & dtA.Rows(i).Item("sourceform").ToString() & "' and tid=" & RegTID
                    Dim RegFieldVal As String = ""
                    oda.SelectCommand.Parameters.Clear()
                    oda.SelectCommand.CommandText = StrQry
                    If con.State = ConnectionState.Closed Then
                        con.Open()
                    End If
                    RegFieldVal = Convert.ToString(oda.SelectCommand.ExecuteScalar())

                    StrQry = "select displayname from mmm_mst_fields where fieldmapping='" & dtA.Rows(i).Item("TargetFields").ToString() & "' and eid=" & eid & " and documenttype='" & dtA.Rows(i).Item("Targetform").ToString() & "'"
                    oda.SelectCommand.CommandText = StrQry
                    Dim TDispName As String
                    TDispName = Convert.ToString(oda.SelectCommand.ExecuteScalar())

                    FilterCond = FilterCond & " v" & eid & dtA.Rows(i).Item("Targetform").Trim.Replace(" ", "_") & ".[" & TDispName & "]='" & RegFieldVal & "' and "
                Else
                    StrQry = "select isnull(" & dtA.Rows(i).Item("sourceFields").ToString() & ",0) from mmm_mst_master where documenttype='" & dtA.Rows(i).Item("sourceform").ToString() & "' and eid=" & eid & " and tid=" & RegTID
                    Dim RegFieldVal As String = ""
                    oda.SelectCommand.CommandText = StrQry
                    RegFieldVal = Convert.ToString(oda.SelectCommand.ExecuteScalar())
                    StrQry = "select displayname from mmm_mst_fields  where  documenttype='" & dtA.Rows(i).Item("Targetform").ToString() & "' and fieldmapping='" & dtA.Rows(i).Item("TargetDepFldMapping").ToString() & "' and eid=" & eid
                    oda.SelectCommand.CommandText = StrQry
                    Dim TDispName As String
                    TDispName = Convert.ToString(oda.SelectCommand.ExecuteScalar())
                    Dim tempRegFieldVal As String() = RegFieldVal.ToString().Split(",")
                    For ival As Integer = 0 To tempRegFieldVal.Length - 1
                        'If Convert.ToString(tempRegFieldVal(ival)) <> "0" Then
                        FilterCond = FilterCond & "','+ v" & eid & dtA.Rows(i).Item("Targetform").Trim.Replace(" ", "_") & ".[" & TDispName & "]+',' like '%," & Convert.ToString(tempRegFieldVal(ival)) & ",%' or "
                        'End If
                    Next
                End If
            Next
            If FilterCond.Length > 1 Then
                FilterCond = Left(FilterCond, Len(FilterCond) - 4)
            End If

            dtA = Nothing
            Return FilterCond
        Catch ex As Exception
            Return FilterCond
            oda.Dispose()
            con.Dispose()
        Finally
            oda.Dispose()
            con.Dispose()
        End Try
    End Function





    '' bkup 
    'Public Function ExtendRelation(eid As Integer, CreatedDocType As String, DOCID As Integer, UID As Integer, DOCList As String, ForceExtend As Boolean) As RelationResponse
    '    Dim ret As New RelationResponse()
    '    Dim dsRel As New DataSet()
    '    Dim con As New SqlConnection(conStr)
    '    Dim da As SqlDataAdapter
    '    Dim tran As SqlTransaction = Nothing
    '    Try
    '        'Getting All the Relations
    '        dsRel = GetExecutableRelation(eid, CreatedDocType)
    '        If dsRel.Tables(0).Rows.Count > 0 Then
    '            Dim T_DOCType = "", T_DOC = "", S_DOC1 = "", S_DOC2 = ""
    '            Dim ShowExtend As Boolean = False
    '            T_DOCType = dsRel.Tables(0).Rows(0).Item("T_DOCTYPE")
    '            T_DOC = dsRel.Tables(0).Rows(0).Item("T_DOC")
    '            S_DOC1 = dsRel.Tables(0).Rows(0).Item("S_DOC1")
    '            S_DOC2 = dsRel.Tables(0).Rows(0).Item("S_DOC2")
    '            If Convert.ToString(dsRel.Tables(0).Rows(0).Item("ShowExtend")) = "1" Then
    '                ShowExtend = True
    '            End If
    '            If ShowExtend = False Or ForceExtend = True Then
    '                Dim TbepubDoc = ""
    '                If S_DOC1 <> CreatedDocType Then
    '                    TbepubDoc = S_DOC1
    '                Else
    '                    TbepubDoc = S_DOC2
    '                End If
    '                Dim vName = ""
    '                Dim Query = ""
    '                vName = "V" & eid & TbepubDoc.Trim.Replace(" ", "_")
    '                Query = "SELECT tid,DocumentType FROM " & vName
    '                If DOCList.Trim <> "" Then
    '                    Query = Query & " where [" & vName & "].tid in (" & DOCList & ")"
    '                End If
    '                Dim dsDocList As New DataSet()
    '                'Getting another Document 
    '                Using con1 = New SqlConnection(conStr)
    '                    Using da1 As New SqlDataAdapter(Query, con1)
    '                        con1.Open()
    '                        da1.Fill(dsDocList)
    '                    End Using
    '                End Using
    '                Dim strColumn As New StringBuilder()
    '                Dim strValue As New StringBuilder()
    '                Dim StrChkUnique As New StringBuilder()
    '                Dim StrQuery = ""
    '                Dim fld As String = ""
    '                Dim Value As String = ""
    '                Dim FileID As Integer = 0
    '                Dim SCount As Integer = 0
    '                Dim DupCount As Integer = 0
    '                'Insert into data base for relation extension 
    '                If dsDocList.Tables(0).Rows.Count > 0 Then

    '                    Dim dsfield As New DataSet()
    '                    dsfield = GetAllFields(eid, T_DOC) 'GetFieldDef(eid, T_DOC)
    '                    Dim FieldType As String = "", IsEditable As String = "0"

    '                    'Lopp For all doc list
    '                    Dim ddlVal As String = ""
    '                    Dim DefaultVal As String = "", Datatype = ""
    '                    'Datetime
    '                    If con.State <> ConnectionState.Open Then
    '                        con.Open()
    '                    End If
    '                    tran = con.BeginTransaction()

    '                    For d As Integer = 0 To dsDocList.Tables(0).Rows.Count - 1
    '                        strColumn = New StringBuilder()
    '                        strValue = New StringBuilder()
    '                        StrChkUnique = New StringBuilder()
    '                        StrQuery = ""
    '                        If T_DOCType = "DOCUMENT" Then
    '                            StrChkUnique.Append("IF NOT  Exists(SELECT EID FROM MMM_MST_DOC WHERE EID=" & eid & " AND DocumentType='" & T_DOC & "'")
    '                            strColumn.Append("INSERT INTO MMM_MST_DOC(EID,Documenttype,oUID,adate,Source")
    '                            strValue.Append("VALUES (").Append(eid).Append(",'").Append(T_DOC).Append("',").Append(UID).Append(",getdate(),'RExt'")
    '                        Else
    '                            StrChkUnique.Append("IF NOT  Exists(SELECT EID FROM MMM_MST_Master WHERE EID=" & eid & " AND DocumentType='" & T_DOC & "'")
    '                            strColumn.Append("INSERT INTO MMM_MST_Master(EID,Documenttype,CreatedBy,UpdatedDate,Source")
    '                            strValue.Append(" VALUES").Append("(").Append(eid).Append(",'").Append(T_DOC).Append("',").Append(UID).Append(",getdate(),'RExt' ")
    '                        End If
    '                        'IsEditable, defaultfieldval ,datatype
    '                        For f As Integer = 0 To dsfield.Tables(0).Rows.Count - 1
    '                            Value = ""
    '                            IsEditable = Convert.ToString(dsfield.Tables(0).Rows(f).Item("IsEditable"))
    '                            DefaultVal = Convert.ToString(dsfield.Tables(0).Rows(f).Item("defaultfieldval"))
    '                            Datatype = Convert.ToString(dsfield.Tables(0).Rows(f).Item("datatype"))
    '                            Dim arr = Convert.ToString(dsfield.Tables(0).Rows(f).Item("DropDown")).Split("-")
    '                            FieldType = Convert.ToString(dsfield.Tables(0).Rows(f).Item("FieldType"))
    '                            Select Case FieldType
    '                                Case "Drop Down"
    '                                    If String.IsNullOrEmpty(DefaultVal.Trim) Then
    '                                        Value = "0"
    '                                    Else
    '                                        Value = DefaultVal
    '                                    End If
    '                                Case "LookupDDL"
    '                                    If String.IsNullOrEmpty(DefaultVal.Trim) Then
    '                                        Value = "0"
    '                                    Else
    '                                        Value = DefaultVal
    '                                    End If
    '                            End Select
    '                            'Seting current docid to concern document
    '                            If FieldType = "Drop Down" Then
    '                                If arr(1).Trim.ToUpper = CreatedDocType.Trim.ToUpper Then
    '                                    Value = DOCID
    '                                    'Generating Query For Duplicacy check
    '                                    StrChkUnique.Append(" AND " & Convert.ToString(dsfield.Tables(0).Rows(f).Item("FieldMapping")) & " = '" & Value & "'")
    '                                End If
    '                                If arr(1).Trim.ToUpper = Convert.ToString(dsDocList.Tables(0).Rows(d).Item("DocumentType")).Trim.ToUpper Then
    '                                    Value = Convert.ToString(dsDocList.Tables(0).Rows(d).Item("tid"))
    '                                    StrChkUnique.Append(" AND " & Convert.ToString(dsfield.Tables(0).Rows(f).Item("FieldMapping")) & " = '" & Value & "'")
    '                                End If
    '                            End If
    '                            If FieldType = "Text Box" And Datatype = "Datetime" And IsEditable = "0" Then
    '                                Dim Result As String = ""
    '                                Dim dt As Date = Date.Today
    '                                Result = dt.ToString("dd/MM/yy", CultureInfo.InvariantCulture)
    '                                Value = Result
    '                            End If
    '                            If Not String.IsNullOrEmpty(DefaultVal.Trim) And String.IsNullOrEmpty(Value.Trim) Then
    '                                Value = DefaultVal
    '                            End If
    '                            strColumn.Append(",").Append(Convert.ToString(dsfield.Tables(0).Rows(f).Item("FieldMapping")))
    '                            Value = Value.Replace("'", "''")
    '                            strValue.Append(",'").Append(Value).Append("'")
    '                        Next
    '                        strColumn.Append(")")
    '                        strValue.Append(")")
    '                        'Ending if Exists Block
    '                        StrChkUnique.Append(") BEGIN ")
    '                        StrQuery = strColumn.ToString & strValue.ToString
    '                        StrQuery = StrQuery & ";Select @@identity"
    '                        StrQuery = StrChkUnique.ToString & StrQuery & " END" & " ELSE SELECT 0"
    '                        'Save loging is going on from here
    '                        da = New SqlDataAdapter(StrQuery, con)
    '                        da.SelectCommand.Transaction = tran
    '                        'Transactional Query
    '                        FileID = da.SelectCommand.ExecuteScalar()
    '                        If FileID > 0 Then
    '                            'Excecuting Auto Number

    '                            Dim AutoNumQ = "select * FROM MMM_MST_FIELDS WHERE EID=" & eid & " AND DocumentType='" & T_DOC & "' AND (FieldType='Auto Number' or Fieldtype='New Auto Number')"
    '                            Dim DSAuto As New DataSet()
    '                            da.SelectCommand.CommandText = AutoNumQ
    '                            da.Fill(DSAuto)
    '                            For AutoNumC As Integer = 0 To DSAuto.Tables(0).Rows.Count - 1
    '                                Select Case DSAuto.Tables(0).Rows(AutoNumC).Item("fieldtype").ToString
    '                                    Case "Auto Number"
    '                                        da.SelectCommand.Parameters.Clear()
    '                                        If da.SelectCommand.Transaction Is Nothing Then
    '                                            da.SelectCommand.Transaction = tran
    '                                        End If
    '                                        da.SelectCommand.CommandText = "usp_GetAutoNoNew_WS"
    '                                        da.SelectCommand.CommandType = CommandType.StoredProcedure
    '                                        da.SelectCommand.Parameters.AddWithValue("Fldid", DSAuto.Tables(0).Rows(AutoNumC).Item("FieldID"))
    '                                        da.SelectCommand.Parameters.AddWithValue("docid", FileID)
    '                                        da.SelectCommand.Parameters.AddWithValue("fldmapping", DSAuto.Tables(0).Rows(AutoNumC).Item("FieldMapping"))
    '                                        da.SelectCommand.Parameters.AddWithValue("FormType", T_DOCType)
    '                                        'Transactional Query
    '                                        Dim an As String = Convert.ToString(da.SelectCommand.ExecuteScalar())
    '                                        da.SelectCommand.Parameters.Clear()
    '                                    Case "New Auto Number"
    '                                        da.SelectCommand.Parameters.Clear()
    '                                        da.SelectCommand.CommandText = "usp_GetAutoNoNew_New"
    '                                        da.SelectCommand.CommandType = CommandType.StoredProcedure
    '                                        da.SelectCommand.Parameters.AddWithValue("Fldid", DSAuto.Tables(0).Rows(AutoNumC).Item("FieldID"))
    '                                        da.SelectCommand.Parameters.AddWithValue("SearchFldid", DSAuto.Tables(0).Rows(AutoNumC).Item("Dropdown"))
    '                                        da.SelectCommand.Parameters.AddWithValue("docid", FileID)
    '                                        da.SelectCommand.Parameters.AddWithValue("fldmapping", DSAuto.Tables(0).Rows(AutoNumC).Item("FieldMapping"))
    '                                        da.SelectCommand.Parameters.AddWithValue("FormType", T_DOCType)
    '                                        Dim an As String = da.SelectCommand.ExecuteScalar()
    '                                        da.SelectCommand.Parameters.Clear()
    '                                End Select
    '                            Next
    '                            'Executing Formula Field Now
    '                            Dim FormulaQ = "select * FROM MMM_MST_FIELDS WHERE EID=" & eid & " AND DocumentType='" & T_DOC & "' AND FieldType='Formula Field' order by displayorder"
    '                            Dim DSF As New DataSet()
    '                            da.SelectCommand.Parameters.Clear()
    '                            da.SelectCommand.CommandText = FormulaQ
    '                            da.SelectCommand.CommandType = CommandType.Text
    '                            da.Fill(DSF)
    '                            'Ececuting Formula Fields
    '                            Try
    '                                Dim viewdoc As String = T_DOC
    '                                viewdoc = viewdoc.Replace(" ", "_")
    '                                If DSF.Tables(0).Rows.Count > 0 Then
    '                                    For f As Integer = 0 To DSF.Tables(0).Rows.Count - 1
    '                                        Dim formulaeditorr As New formulaEditor
    '                                        Dim forvalue As String = String.Empty
    '                                        forvalue = formulaeditorr.ExecuteFormulaT(DSF.Tables(0).Rows(f).Item("KC_LOGIC"), FileID, "v" + eid.ToString + viewdoc, eid, 0, con, tran)
    '                                        'Transactional Query
    '                                        Dim upquery As String = "update " & DSF.Tables(0).Rows(f).Item("DBTableName").ToString & "  set  " & DSF.Tables(0).Rows(f).Item("FieldMapping").ToString & "='" & forvalue.ToString & "'  where tid =" & FileID & ""
    '                                        da.SelectCommand.CommandText = upquery
    '                                        da.SelectCommand.CommandType = CommandType.Text
    '                                        da.SelectCommand.ExecuteNonQuery()
    '                                    Next
    '                                End If
    '                            Catch ex As Exception
    '                                Throw
    '                            End Try

    '                            'Executing Workflow now
    '                            If T_DOCType.Trim.ToUpper = "DOCUMENT" Then
    '                                Dim ob2 As New DMSUtil()
    '                                da.SelectCommand.CommandText = "InsertDefaultMovement"
    '                                da.SelectCommand.CommandType = CommandType.StoredProcedure
    '                                da.SelectCommand.Parameters.Clear()
    '                                da.SelectCommand.Parameters.AddWithValue("tid", FileID)
    '                                da.SelectCommand.Parameters.AddWithValue("CUID", UID.ToString())
    '                                da.SelectCommand.Parameters.AddWithValue("what", "UPLOADED")
    '                                'If con.State <> ConnectionState.Open Then
    '                                '    con.Open()
    '                                'End If
    '                                'Transactional Query
    '                                da.SelectCommand.ExecuteNonQuery()
    '                                Dim res As String
    '                                'Transactional Query
    '                                res = ob2.GetNextUserFromRolematrixT(FileID, eid, UID, "", UID, con, tran)
    '                                Dim sretMsgArr() As String = res.Split(":")
    '                                'Code for Mapping
    '                                If sretMsgArr(0) = "ARCHIVE" Then
    '                                    Dim Op As New Exportdata()
    '                                    'Transactional Query
    '                                    Op.PushdataT(FileID, sretMsgArr(1), eid, con, tran)
    '                                End If

    '                            End If
    '                            'Excecuting Code For Triggers
    '                            Trigger.ExecuteTriggerT(T_DOC, eid, FileID, con, tran, 1, TriggerNature:="Create", FormType:="Document")

    '                            'Code for calling outward webservice
    '                            'Try
    '                            '    If FileID > 0 Then
    '                            '        Dim WSOUT As New WSOutward()
    '                            '        Dim WSOret = WSOUT.WBS(CreatedDocType, eid, FileID)
    '                            '    End If
    '                            'Catch ex As Exception
    '                            'End Try
    '                            'Dim ob1 As New DMSUtil()
    '                            'Try
    '                            '    'Change Required
    '                            '    ob1.TemplateCalling(FileID, eid, CreatedDocType, "CREATED")
    '                            'Catch ex As Exception
    '                            'End Try
    '                            'Try
    '                            '    da.SelectCommand.CommandType = CommandType.Text
    '                            '    da.SelectCommand.Parameters.Clear()
    '                            '    da.SelectCommand.CommandText = "select isworkflow from mmm_mst_doc where TID=" & FileID & " and eid='" & eid & "'"
    '                            '    Dim dt As New DataTable
    '                            '    da.Fill(dt)
    '                            '    If dt.Rows.SCount = 1 Then
    '                            '        If dt.Rows(0).Item(0).ToString = "1" Then
    '                            '            'Change Required
    '                            '            ob1.TemplateCalling(FileID, eid, CreatedDocType, "APPROVE")
    '                            '        End If
    '                            '    End If
    '                            'Catch ex As Exception
    '                            'End Try
    '                        End If
    '                        If FileID > 0 Then
    '                            SCount = SCount + 1
    '                        Else
    '                            DupCount = DupCount + 1
    '                        End If

    '                    Next
    '                    tran.Commit()
    '                    ret.Success = True
    '                    ret.Message = "Total Extended Relation Document:: Success count:- " & SCount & ".Duplicate Count:- " & DupCount
    '                    ret.ShowExtend = ShowExtend
    '                Else
    '                    'This logic may be change later
    '                    ret.Success = True
    '                    ret.Message = "No any relation Exists."
    '                    ret.ShowExtend = False
    '                End If
    '            Else
    '                ret.Success = True
    '                ret.Message = "Extension through new page."
    '                ret.ShowExtend = True
    '            End If
    '        Else
    '            ret.Success = True
    '            ret.Message = "No any relation Exists."
    '            ret.ShowExtend = False
    '        End If
    '    Catch ex As Exception
    '        ret.Success = False
    '        ret.Message = "Error occured at server."
    '        ret.ShowExtend = False
    '        If Not tran Is Nothing Then
    '            tran.Rollback()
    '            tran.Dispose()
    '        End If
    '    End Try
    '    Return ret
    'End Function



    Public Function ExtendSourceRelation(eid As Integer, CreatedDocType As String, DOCID As Integer) As Integer
        Dim ret As Integer = 0
        'Will do it later
        Return ret
    End Function

    Public Function ExtendTargetRelation(eid As Integer, CreatedDocType As String, DOCID As Integer) As Integer
        Dim ret As Integer = 0
        'Will do it later
        Return ret
    End Function
    Public Function GenearateQuery(FileID As Integer, EID As Integer, DocumentType As String, IsActionForm As Boolean, ActionNature As String) As String
        Dim ret As String = ""
        Try
            Dim ds As New DataSet()
            'Geiing all the field of Entity becouse all the field might be required
            Dim ObjRule As New RuleEngin()
            ds = ObjRule.GetAllFields(EID)
            Dim BaseView As DataView
            Dim BaseTable As DataTable
            If ds.Tables(0).Rows.Count > 0 Then
                BaseView = ds.Tables(0).DefaultView
                BaseView.RowFilter = "DocumentType='" & DocumentType & "'"
                'Getting all the field of base tabel() Note: in case of Action form base table will be name Action form
                BaseTable = BaseView.Table.DefaultView.ToTable()
                If IsActionForm = True Then
                    DocumentType = ds.Tables(0).Rows(0).Item("EventName").ToString
                    BaseView.RowFilter = "DocumentType='" & DocumentType & "'"
                    BaseTable = BaseView.Table.DefaultView.ToTable()
                End If
                GenearateQuery1(EID, DocumentType, ds)
                'Now find all object relation 
            End If
        Catch ex As Exception
        End Try
        Return ret
    End Function

    '' prev backup by sp on 07-jan-19
    'Public Function GenearateQuery1(EID As Integer, DocumentType As String, ds As DataSet, Optional EnableEdit As String = "") As String
    '    Dim ret As String = ""
    '    Dim View As DataView
    '    Dim tbl As DataTable
    '    Dim tblRe As DataTable
    '    Dim StrColumn As String = ""
    '    Dim StrJoinString As String = ""
    '    Dim SchemaString As String = DocumentType
    '    If ds.Tables(0).Rows.Count > 0 Then
    '        View = ds.Tables(0).DefaultView
    '        View.RowFilter = "DocumentType='" & DocumentType & "'"
    '        If EnableEdit <> "" Then
    '            View.RowFilter = "DocumentType='" & DocumentType & "' AND EnableEdit=1 AND isEditable=1"
    '        End If
    '        'Getting all the field of base tabel() Note: in case of Action form base table will be name Action form
    '        tbl = View.Table.DefaultView.ToTable()
    '        Dim ViewName = "[V" & EID & DocumentType.Trim().Replace(" ", "_") & "]"
    '        Dim ddlDocType = ""
    '        For i As Integer = 0 To tbl.Rows.Count - 1
    '            If Not (tbl.Rows(i).Item("DROPDOWNTYPE") = "MASTER VALUED" And tbl.Rows(i).Item("FieldType") = "Drop Down") Then
    '                StrColumn = StrColumn & "," & ViewName & ".[" & tbl.Rows(i).Item("DisplayName") & "] AS [" & tbl.Rows(i).Item("DisplayName") & "]"
    '            End If
    '        Next
    '        View.RowFilter = "DocumentType='" & DocumentType & "' AND FieldType ='DROP DOWN' AND (DropDownType='MASTER VALUED' OR DropDownType='SESSION VALUED')"
    '        tblRe = View.Table.DefaultView.ToTable()
    '        Dim FieldMapping As String = ""
    '        Dim Allias As String = ""
    '        Dim dRoW As DataRow()
    '        For j As Integer = 0 To tblRe.Rows.Count - 1
    '            Dim arrddl = tblRe.Rows(j).Item("Dropdown").ToString().Split("-")
    '            ddlDocType = arrddl(1)
    '            SchemaString = ddlDocType
    '            FieldMapping = arrddl(2)
    '            Dim ddlview = "[V" & EID & ddlDocType.Trim.Replace(" ", "_") & "]"
    '            Dim joincolumn = "tid"
    '            Dim DisPlayName = tblRe.Rows(j).Item("DisplayName").ToString().Trim
    '            Dim columnName = ""
    '            dRoW = ds.Tables(0).Select("DocumentType='" & ddlDocType & "' AND Fieldmapping='" & FieldMapping & "'")
    '            columnName = dRoW(0).Item("DisplayName")
    '            If ddlDocType.Trim.ToUpper = "USER" Then
    '                joincolumn = "UID"
    '                ddlview = "[MMM_MST_USER]"
    '                Allias = "U" & j
    '                columnName = arrddl(2)
    '            Else
    '                Allias = "V" & EID & ddlDocType.Trim.Replace(" ", "_") & j
    '            End If
    '            'Condition for Stoping Repeated join String in the case when same ddocument type repeates

    '            If Not StrJoinString.Contains("[" & ddlview & "]") Then
    '                StrJoinString = StrJoinString & "left outer join " & ddlview & " As " & Allias & "  on convert(nvarchar, " & Allias & "." & joincolumn & ") = " & ViewName & ".[" & DisPlayName & "]"
    '            End If
    '            'Dim Filter = "DocumentType='" & ddlDocType & "' AND Fieldmapping='" & FieldMapping & "'"

    '            StrColumn = StrColumn & "," & Allias & ".[" & columnName & "] AS [" & tblRe.Rows(j).Item("DisplayName") & "]"
    '            'GenearateQuery2(EID, StrColumn, StrJoinString, SchemaString, ds)
    '        Next
    '        View.RowFilter = "DocumentType='" & DocumentType & "' AND FieldType ='LookupDDL'"
    '        tblRe = View.Table.DefaultView.ToTable()
    '        Dim ArrDDlDocType As String()
    '        Dim ArrDDlDocfld As String()
    '        Dim DDlDocfld As String = ""
    '        Dim DDlDocfld1 As String = ""
    '        Dim ddllSource As String = ""
    '        For k As Integer = 0 To tblRe.Rows.Count - 1
    '            dRoW = ds.Tables(0).Select("DocumentType='" & DocumentType & "' AND fieldID='" & tblRe.Rows(0).Item("DropDown") & "'")
    '            If dRoW.Count > 0 Then
    '                ArrDDlDocType = dRoW(0).Item("Dropdown").Split("-")
    '                ArrDDlDocfld = dRoW(0).Item("DDllookupvalue").Split(",")
    '                For m As Integer = 0 To ArrDDlDocfld.Length - 1
    '                    If ArrDDlDocfld(m).Trim <> "" Then
    '                        Dim d = ArrDDlDocfld(m).Split("-")
    '                        If d(0).Trim = tblRe.Rows(k).Item("FieldID").ToString.Trim Then
    '                            DDlDocfld = d(1)
    '                            Exit For
    '                        End If
    '                    End If
    '                Next
    '                If DDlDocfld.Trim <> "" Then
    '                    Dim DisplayName As String = ""
    '                    dRoW = ds.Tables(0).Select("DocumentType='" & ArrDDlDocType(1) & "' AND FieldMapping='" & DDlDocfld & "'")
    '                    ArrDDlDocType = dRoW(0).Item("DropDown").Split("-")
    '                    ddllSource = ArrDDlDocType(1).Trim
    '                    If ddllSource.Trim.ToUpper <> "USER" Then
    '                        dRoW = ds.Tables(0).Select("DocumentType='" & ddllSource & "' AND FieldMapping='" & ArrDDlDocType(2) & "'")
    '                        DisplayName = "[" & dRoW(0).Item("DisplayName") & "]"
    '                        Dim ddlQ = " (SELECT " & DisplayName & " FROM " & "[V" & EID & ddllSource & "] where tid= " & ViewName & ".[" & tblRe.Rows(k).Item("DisplayName") & "]) AS '" & tblRe.Rows(k).Item("DisplayName") & "'"
    '                        StrColumn = StrColumn & " ," & ddlQ
    '                    Else
    '                        Dim ddlQ = " (SELECT " & ArrDDlDocType(2) & " FROM " & "[MMM_MST_USER] where UID= " & ViewName & ".[" & tblRe.Rows(k).Item("DisplayName") & "]) AS '" & tblRe.Rows(k).Item("DisplayName") & "'"
    '                        StrColumn = StrColumn & " ," & ddlQ
    '                    End If

    '                End If
    '                'ddlDocType = ArrDDlDocType(1)
    '                'dRoW = ds.Tables(0).Select("DocumentType='" & ddlDocType & "' AND fieldID='" & tblRe.Rows(0).Item("DropDown") & "'")
    '            End If
    '        Next
    '        Dim Query = StrColumn.Substring(1, StrColumn.Length - 1) & " FROM " & ViewName & " " & StrJoinString
    '        ret = Query & " WHERE 1=1 "
    '        'If tid <> 0 Then
    '        '    Query = Query & " WHERE " & ViewName & ".tid  = " & tid
    '        'End If
    '    End If
    '    Return ret
    'End Function


    Public Function GenearateQuery1(EID As Integer, DocumentType As String, ds As DataSet, Optional EnableEdit As String = "") As String
        Dim ret As String = ""
        Dim View As DataView
        Dim tbl As DataTable
        Dim tblRe As DataTable
        Dim StrColumn As String = ""
        Dim StrJoinString As String = ""
        Dim SchemaString As String = DocumentType
        If ds.Tables(0).Rows.Count > 0 Then
            View = ds.Tables(0).DefaultView
            View.RowFilter = "DocumentType='" & DocumentType & "'"
            If EnableEdit <> "" Then
                View.RowFilter = "DocumentType='" & DocumentType & "' AND EnableEdit=1 AND isEditable=1"
            End If
            'Getting all the field of base tabel() Note: in case of Action form base table will be name Action form
            tbl = View.Table.DefaultView.ToTable()
            Dim ViewName = "[V" & EID & DocumentType.Trim().Replace(" ", "_") & "]"
            Dim ddlDocType = ""
            For i As Integer = 0 To tbl.Rows.Count - 1
                If Not (tbl.Rows(i).Item("DROPDOWNTYPE") = "MASTER VALUED" And tbl.Rows(i).Item("FieldType") = "Drop Down") Then
                    StrColumn = StrColumn & "," & ViewName & ".[" & tbl.Rows(i).Item("DisplayName") & "] AS [" & tbl.Rows(i).Item("DisplayName") & "]"
                End If
            Next
            View.RowFilter = "DocumentType='" & DocumentType & "' AND FieldType ='DROP DOWN' AND (DropDownType='MASTER VALUED' OR DropDownType='SESSION VALUED')"
            tblRe = View.Table.DefaultView.ToTable()
            Dim FieldMapping As String = ""
            Dim Allias As String = ""
            Dim dRoW As DataRow()
            For j As Integer = 0 To tblRe.Rows.Count - 1
                Dim arrddl = tblRe.Rows(j).Item("Dropdown").ToString().Split("-")
                ddlDocType = arrddl(1)
                SchemaString = ddlDocType
                FieldMapping = arrddl(2)
                Dim ddlview = "[V" & EID & ddlDocType.Trim.Replace(" ", "_") & "]"
                Dim joincolumn = "tid"
                Dim DisPlayName = tblRe.Rows(j).Item("DisplayName").ToString().Trim
                Dim columnName = ""
                dRoW = ds.Tables(0).Select("DocumentType='" & ddlDocType & "' AND Fieldmapping='" & FieldMapping & "'")
                columnName = dRoW(0).Item("DisplayName")
                If ddlDocType.Trim.ToUpper = "USER" Then
                    joincolumn = "UID"
                    ddlview = "[MMM_MST_USER]"
                    Allias = "U" & j
                    columnName = arrddl(2)
                Else
                    Allias = "V" & EID & ddlDocType.Trim.Replace(" ", "_") & j
                End If
                'Condition for Stoping Repeated join String in the case when same ddocument type repeates

                If Not StrJoinString.Contains("[" & ddlview & "]") Then
                    StrJoinString = StrJoinString & "left outer join " & ddlview & " As " & Allias & "  on convert(nvarchar, " & Allias & "." & joincolumn & ") = " & ViewName & ".[" & DisPlayName & "]"
                End If
                'Dim Filter = "DocumentType='" & ddlDocType & "' AND Fieldmapping='" & FieldMapping & "'"

                StrColumn = StrColumn & "," & Allias & ".[" & columnName & "] AS [" & tblRe.Rows(j).Item("DisplayName") & "]"
                'GenearateQuery2(EID, StrColumn, StrJoinString, SchemaString, ds)
            Next
            View.RowFilter = "DocumentType='" & DocumentType & "' AND FieldType ='LookupDDL'"
            tblRe = View.Table.DefaultView.ToTable()
            Dim ArrDDlDocType As String()
            Dim ArrDDlDocfld As String()
            Dim DDlDocfld As String = ""
            Dim DDlDocfld1 As String = ""
            Dim ddllSource As String = ""
            For k As Integer = 0 To tblRe.Rows.Count - 1
                dRoW = ds.Tables(0).Select("DocumentType='" & DocumentType & "' AND fieldID='" & tblRe.Rows(0).Item("DropDown") & "'")
                If dRoW.Count > 0 Then
                    ArrDDlDocType = dRoW(0).Item("Dropdown").Split("-")
                    ArrDDlDocfld = dRoW(0).Item("DDllookupvalue").Split(",")
                    For m As Integer = 0 To ArrDDlDocfld.Length - 1
                        If ArrDDlDocfld(m).Trim <> "" Then
                            Dim d = ArrDDlDocfld(m).Split("-")
                            If d(0).Trim = tblRe.Rows(k).Item("FieldID").ToString.Trim Then
                                DDlDocfld = d(1)
                                Exit For
                            End If
                        End If
                    Next
                    '' commmented for ddllookup doctype view name error issue by vivek
                    'If DDlDocfld.Trim <> "" Then
                    '    Dim DisplayName As String = ""
                    '    dRoW = ds.Tables(0).Select("DocumentType='" & ArrDDlDocType(1) & "' AND FieldMapping='" & DDlDocfld & "'")
                    '    ArrDDlDocType = dRoW(0).Item("DropDown").Split("-")
                    '    ddllSource = ArrDDlDocType(1).Trim
                    '    If ddllSource.Trim.ToUpper <> "USER" Then
                    '        dRoW = ds.Tables(0).Select("DocumentType='" & ddllSource & "' AND FieldMapping='" & ArrDDlDocType(2) & "'")
                    '        DisplayName = "[" & dRoW(0).Item("DisplayName") & "]"
                    '        Dim ddlQ = " (SELECT " & DisplayName & " FROM " & "[V" & EID & ddllSource & "] where tid= " & ViewName & ".[" & tblRe.Rows(k).Item("DisplayName") & "]) AS '" & tblRe.Rows(k).Item("DisplayName") & "'"
                    '        StrColumn = StrColumn & " ," & ddlQ
                    '    Else
                    '        Dim ddlQ = " (SELECT " & ArrDDlDocType(2) & " FROM " & "[MMM_MST_USER] where UID= " & ViewName & ".[" & tblRe.Rows(k).Item("DisplayName") & "]) AS '" & tblRe.Rows(k).Item("DisplayName") & "'"
                    '        StrColumn = StrColumn & " ," & ddlQ
                    '    End If

                    'End If
                    ''ddlDocType = ArrDDlDocType(1)
                    'dRoW = ds.Tables(0).Select("DocumentType='" & ddlDocType & "' AND fieldID='" & tblRe.Rows(0).Item("DropDown") & "'")
                    'here 'here updaetd block by vivek on 04-may-20
                    If DDlDocfld.Trim <> "" Then
                        Dim DisplayName As String = ""
                        dRoW = ds.Tables(0).Select("DocumentType='" & ArrDDlDocType(1) & "' AND FieldMapping='" & DDlDocfld & "'")
                        Dim dt1 As DataTable = dRoW.CopyToDataTable()
                        ArrDDlDocType = dRoW(0).Item("DropDown").Split("-")
                        ddllSource = ArrDDlDocType(1).Trim
                        If ddllSource.Trim.ToUpper <> "USER" Then
                            dRoW = ds.Tables(0).Select("DocumentType='" & ddllSource & "' AND FieldMapping='" & ArrDDlDocType(2) & "'")
                            Dim ddllookupview = "[V" & EID & ddllSource.Trim.Replace(" ", "_") & "]"
                            DisplayName = "[" & dRoW(0).Item("DisplayName") & "]"
                            Dim ddlQ = " (SELECT " & DisplayName & " FROM " & ddllookupview & " where tid= " & ViewName & ".[" & tblRe.Rows(k).Item("DisplayName") & "]) AS '" & tblRe.Rows(k).Item("DisplayName") & "'"
                            StrColumn = StrColumn & " ," & ddlQ
                        Else
                            Dim ddlQ = " (SELECT " & ArrDDlDocType(2) & " FROM " & "[MMM_MST_USER] where UID= " & ViewName & ".[" & tblRe.Rows(k).Item("DisplayName") & "]) AS '" & tblRe.Rows(k).Item("DisplayName") & "'"
                            StrColumn = StrColumn & " ," & ddlQ
                        End If
                    End If
                    'here updaetd block by vivek on 04-may-20
                End If
            Next
            Dim Query = StrColumn.Substring(1, StrColumn.Length - 1) & " FROM " & ViewName & " " & StrJoinString
            ret = Query & " WHERE 1=1 "
            'If tid <> 0 Then
            '    Query = Query & " WHERE " & ViewName & ".tid  = " & tid
            'End If
        End If
        Return ret
    End Function


    Public Function GenearateQuery2(EID As Integer, ByRef StrColumn As String, StrJoinString As String, DocumentType As String, ds As DataSet) As String
        Dim ret As String = ""
        Dim View As DataView
        Dim tbl As DataTable
        'StrColumn = ""
        If ds.Tables(0).Rows.Count > 0 Then
            View = ds.Tables(0).DefaultView
            View.RowFilter = "DocumentType='" & DocumentType & "'"
            'Getting all the field of base tabel() Note: in case of Action form base table will be name Action form
            tbl = View.Table.DefaultView.ToTable()
            Dim ViewName = "V" & EID & DocumentType.Replace(" ", "_")
            Dim ddlDocType = ""
            For i As Integer = 0 To tbl.Rows.Count - 1
                If (tbl.Rows(i).Item("DROPDOWNTYPE") = "MASTER VALUED" And tbl.Rows(i).Item("FieldType") = "Drop Down") Then
                    Dim arrddl = tbl.Rows(i).Item("Dropdown").ToString().Split("-")
                    ddlDocType = arrddl(1)
                    Dim FieldMalling = arrddl(2)
                    Dim DR As DataRow() = ds.Tables(0).Select("FieldMapping='" & FieldMalling & "' AND DocumentType='" & arrddl(1) & "'")
                    If DR.Count > 0 Then
                        Dim DisplayName = DR(0).Item("DisplayName")
                        Dim str1 = "(SELECT isnull([" & DR(0).Item("DisplayName") & "],'')  from [V" & EID & arrddl(1).Replace(" ", "_") & "] s WHERE CAST(s.tid AS VARCHAR)=CAST(" & ViewName & ".[" & tbl.Rows(i).Item("DisplayName") & "] AS VARCHAR))[" & tbl.Rows(i).Item("DisplayName") & "]"
                        StrColumn = StrColumn & "," & str1
                    End If
                Else
                    StrColumn = StrColumn & "," & ViewName & ".[" & tbl.Rows(i).Item("DisplayName") & "] AS [" & tbl.Rows(i).Item("DisplayName") & "]"
                End If
            Next
        End If
        Return StrColumn
    End Function

    Private Function GetUpdatableRelation(eid As Integer, DocumetType As String) As DataSet
        Dim ds As New DataSet()
        Try
            Using con = New SqlConnection(conStr)
                Using da = New SqlDataAdapter("GetUpdateRelation", con)
                    da.SelectCommand.CommandType = CommandType.StoredProcedure
                    da.SelectCommand.Parameters.AddWithValue("@EID", eid)
                    da.SelectCommand.Parameters.AddWithValue("@DocumentType", DocumetType)
                    da.Fill(ds)
                End Using
            End Using
        Catch ex As Exception
            Throw
        End Try
        Return ds
    End Function

    Private Function UpdateRDOCID(EID As Integer, DocID As Integer, DocumentType As String, con As SqlConnection, trans As SqlTransaction) As Boolean
        Dim ret As Boolean = False
        Try
            Dim ds As New DataSet()
            Dim da = New SqlDataAdapter("GetUpdateRelationA", con)
            da.SelectCommand.Transaction = trans
            da.SelectCommand.CommandType = CommandType.StoredProcedure
            da.SelectCommand.Parameters.AddWithValue("@DocumentType", DocumentType)
            da.SelectCommand.Parameters.AddWithValue("@EID", EID)
            da.Fill(ds)
            If (ds.Tables(0).Rows.Count > 0) Then
                Dim TDocType As String = ""
                Dim TDOC As String = ""
                Dim STRelation = ""
                Dim RelationType = ""
                TDocType = Convert.ToString(ds.Tables(0).Rows(0).Item("T_DOCTYPE"))
                TDOC = Convert.ToString(ds.Tables(0).Rows(0).Item("T_DOC"))
                STRelation = Convert.ToString(ds.Tables(0).Rows(0).Item("S_T_DDN"))
                RelationType = Convert.ToString(ds.Tables(0).Rows(0).Item("Relation_type"))
                Dim RDOCTable = "MMM_MST_MASTER"
                If (TDocType.Trim.ToUpper = "DOCUMENT") Then
                    RDOCTable = "MMM_MST_DOC"
                End If
                Dim ChildName As String = "No Child Item Found"
                Dim UpdateQ = "UPDATE MMM_MST_DOC SET RDOCID=@MID{R} WHERE EID=" & EID & " AND tid={D_tid} "
                If (RelationType.Trim.ToUpper = "CHILD") Then
                    UpdateQ = "UPDATE MMM_MST_DOC_Item SET RDOCID=@MID{R} WHERE DOCID=" & DocID & " AND tid={C_tid} "
                End If
                Dim Rwhere As New StringBuilder("")
                Dim Columns As New StringBuilder("")
                Dim arr = STRelation.Split(",")
                Dim q1 = "DECLARE @MID{R} INT;SET @MID{R}=(SELECT tid FROM " & RDOCTable & " WHERE EID=" & EID & " AND DocumentType='" & TDOC & "'  {R_Query} )"
                Dim strQuery As New StringBuilder("SELECT MMM_MST_DOC.tid As D_tid,MMM_MST_DOC_ITEM.tid As C_tid")
                For i As Integer = 0 To arr.Length - 1
                    If (Not String.IsNullOrEmpty(arr(i).Trim())) Then
                        Dim arr1 = arr(i).Split(":")
                        Dim arrRD = arr1(1).Split(".")
                        Dim arrMD = arr1(0).Split(".")
                        If (arrMD(0).ToUpper.Trim = "MAIN") Then
                            Rwhere.Append(" AND " & arrRD(1).Trim & " = {D_" & arrMD(2) & "} ")
                            Columns.Append(",").Append("MMM_MST_DOC.").Append(arrMD(2)).Append(" As D_").Append(arrMD(2))
                        Else
                            Rwhere.Append(" AND " & arrRD(1).Trim & " = {C_" & arrMD(2) & "} ")
                            ChildName = arrMD(1).ToString.Trim
                            Columns.Append(",").Append("MMM_MST_DOC_ITEM.").Append(arrMD(2)).Append(" As C_").Append(arrMD(2))
                        End If
                    End If
                Next
                Dim MainQ As String = strQuery.ToString & Columns.ToString.Trim & " FROM MMM_MST_DOC LEFT OUTER JOIN (SELECT * FROM MMM_MST_DOC_ITEM WHERE DOCID=" & DocID & " AND DocumentType='" & ChildName & "') AS MMM_MST_DOC_Item ON MMM_MST_DOC_Item.DOCID=MMM_MST_DOC.tid WHERE EID=" & EID & " AND MMM_MST_DOC.tid=" & DocID & " AND MMM_MST_DOC.DocumentType='" + DocumentType + "'"
                da = New SqlDataAdapter(MainQ, con)
                da.SelectCommand.Transaction = trans
                Dim dsD = New DataSet()
                da.Fill(dsD)
                q1 = q1.Replace("{R_Query}", Rwhere.ToString)
                Dim rStr As String = ""
                Dim SBUQuery As New StringBuilder()
                If (dsD.Tables(0).Rows.Count > 0) Then
                    For i As Integer = 0 To dsD.Tables(0).Rows.Count - 1
                        rStr = q1 & ";" & UpdateQ
                        rStr = rStr.Replace("{R}", i)
                        For Each column As DataColumn In dsD.Tables(0).Columns
                            ' column.ColumnName
                            Dim test As String = "{" & column.ColumnName & "}"
                            If rStr.Contains("{" & column.ColumnName & "}") Then
                                rStr = rStr.Replace("{" & column.ColumnName & "}", "'" & dsD.Tables(0).Rows(i).Item(column.ColumnName) & "'")
                            End If
                        Next
                        SBUQuery.Append(";").Append(rStr)
                    Next
                End If
                da = New SqlDataAdapter(SBUQuery.ToString.Trim, con)
                da.SelectCommand.Transaction = trans
                Dim resRID = da.SelectCommand.ExecuteNonQuery()
                If Convert.ToInt32(resRID) > 0 Then
                    ret = True
                End If
            End If
        Catch ex As Exception
            Throw
        End Try
        Return ret
    End Function

    Private Function ChkCancleDOCID(EID As Integer, DocID As Integer, DocumentType As String, con As SqlConnection, trans As SqlTransaction) As Boolean
        Dim ret As Boolean = False
        Try
            Dim ds As New DataSet()
            Dim da = New SqlDataAdapter("GetUpdateRelationA", con)
            da.SelectCommand.Transaction = trans
            da.SelectCommand.CommandType = CommandType.StoredProcedure
            da.SelectCommand.Parameters.AddWithValue("@DocumentType", DocumentType)
            da.SelectCommand.Parameters.AddWithValue("@EID", EID)
            da.Fill(ds)
            If (ds.Tables(0).Rows.Count > 0) Then
                ret = True
            End If
        Catch ex As Exception
            Throw
        End Try
        Return ret

    End Function

    Public Function ExecutePeriodWiseBalance(EID As Integer, DocID As Integer, DocumentType As String, con As SqlConnection, trans As SqlTransaction) As String
        Dim ret As String = ""
        Try
            'Update RDOCID
            Dim UpRFlg = False
            UpRFlg = UpdateRDOCID(EID, DocID, DocumentType, con, trans)
            'Checking Period Wise Balance Setting
            If UpRFlg = True Then
                Dim ds As New DataSet()
                Dim da As New SqlDataAdapter("GETPwiseSetting", con)
                da.SelectCommand.CommandType = CommandType.StoredProcedure
                da.SelectCommand.Parameters.AddWithValue("@EID", EID)
                da.SelectCommand.Parameters.AddWithValue("@DOCID", DocID)
                da.SelectCommand.Parameters.AddWithValue("@DocumentType", DocumentType)
                da.SelectCommand.Transaction = trans
                da.Fill(ds)
                Dim DRDOCID As String = ""
                Dim CRDOCID As String = ""
                Dim CDOCID As String = ""
                Dim EffDate As String = ""
                Dim Amount As String = ""
                Dim RDOCTYpe As String = ""
                Dim RType As String = ""
                Dim OperationType As String = ""
                ''Updating RDOC
                For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                    DRDOCID = ds.Tables(0).Rows(i).Item("DRDOCID")
                    CRDOCID = ds.Tables(0).Rows(i).Item("CRDOCID")
                    CDOCID = ds.Tables(0).Rows(i).Item("CDOCID")
                    EffDate = ds.Tables(0).Rows(i).Item("EffDate")
                    Amount = ds.Tables(0).Rows(i).Item("Amount")
                    RDOCTYpe = ds.Tables(0).Rows(i).Item("RDOCTYpe")
                    RType = ds.Tables(0).Rows(i).Item("RType")
                    OperationType = ds.Tables(0).Rows(i).Item("OperationType")
                    'Update All the inventroy one by one
                    UpdateRelation(EID, DocID, DocumentType, DRDOCID, CRDOCID, CDOCID, EffDate, Amount, RDOCTYpe, RType, OperationType, con, trans)
                Next
            End If
        Catch ex As Exception
            Throw New Exception("Error In Period Wise Balance Execution" & ex.InnerException.ToString)
        End Try
        Return ret
    End Function

    Public Function UpdateRelation(eid As Integer, DOCID As Integer, DocumetType As String, DRDOCID As String, CRDOCID As String, CDOCID As String, EffDate As String, Amount As String, RDOCTYpe As String, RType As String, OperationType As String, con As SqlConnection, trans As SqlTransaction) As RelationResponse

        Dim ds As New DataSet()
        Dim ret As New RelationResponse()
        Dim Balance_Field As String = ""
        Dim Item_Number As String = ""
        Dim Relation_Doc_type As String = ""
        Dim balance_Maintenance_mode As String = ""
        Dim RfldMapping As String = ""
        Dim FormType1 As String = "MMM_MST_DOC"
        Dim Oparator As String = "+"
        Try
            'Getting Info of relational DOC
            ds = GetUpdatableRelation(eid, DocumetType)
            If ds.Tables(0).Rows.Count > 0 Then
                Dim S_DOCType1 As String = ""

                Dim S_DOCType2 As String = ""
                Dim T_DOCTYPE As String = ""
                Dim T_DOC As String = ""
                Dim Quantity As String = ""
                Dim Mode As String = ""
                Dim Query As New StringBuilder()
                'Setting value of relational DOC into variable
                S_DOCType1 = Convert.ToString(ds.Tables(0).Rows(0).Item("S_DOC1"))
                S_DOCType2 = Convert.ToString(ds.Tables(0).Rows(0).Item("S_DOC2"))
                T_DOCTYPE = Convert.ToString(ds.Tables(0).Rows(0).Item("T_DOC"))
                T_DOC = Convert.ToString(ds.Tables(0).Rows(0).Item("T_DOCType"))
                Mode = Convert.ToString(ds.Tables(0).Rows(0).Item("Mode"))
                Quantity = Convert.ToString(ds.Tables(0).Rows(0).Item("Quantity"))
                Dim tblName As String = ""
                'Finding Target table
                If T_DOC.ToUpper.Trim = "MASTER" Then
                    tblName = "MMM_MST_MASTER"
                Else
                    tblName = "MMM_MST_DOC"
                End If

                Select Case OperationType.Trim.ToUpper
                    Case "SUBTRACT"
                        Oparator = "-"
                    Case "ADD"
                        Oparator = "+"
                    Case "MULYIPLY"
                        Oparator = "*"
                End Select

                'FormType
                'Updating Relational DOC
                'Getting RDOCID,Note:relatio doc ID might reside on child or on document
                Dim targetDocID As Integer = DRDOCID
                If RType.Trim.ToUpper = "CHILD" Then
                    targetDocID = CRDOCID
                End If
                'Dim dsF As New DataSet()
                'dsF = GetRelationFldInfo(DocumetType, eid, T_DOCTYPE)
                'RfldMapping = Convert.ToString(dsF.Tables(0).Rows(0).Item("FieldMapping"))
                'Query.Append("DECLARE @Quintity INT;SET @Quintity=(Select " & Balance_Field & " FROM " & FormType1 & " where tid= " & DOCID & ");")
                Query.Append("UPDATE " & tblName & " SET  " & Quantity & "=CAST(CAST(isnull(" & Quantity & ",0) As decimal(18,2))" & Oparator & "CAST(" & Amount & " As INT) AS INT) WHERE Documenttype='" & T_DOCTYPE & "' AND EID= " & eid & " AND tid=" & targetDocID)
                Dim Query1 = Query.ToString
                Dim da As New SqlDataAdapter(Query.ToString, con)
                da.SelectCommand.Transaction = trans
                Dim ret1 = da.SelectCommand.ExecuteNonQuery()
                'Now inserting it into trans table
                Query = New StringBuilder()
                Query.Append("INSERT INTO MMM_SSV_TRANS(DOCID,RDocID,TransDate,Quantity,EffectiveDate,ChildID) values(")
                Query.Append(DOCID & "," & targetDocID & ",getdate()," & Amount & ",'" & EffDate & "'," & CDOCID & ");SELECT SCOPE_IDENTITY();")
                Query1 = Query.ToString
                Query1 = Query.ToString
                da.SelectCommand.CommandText = Query.ToString
                Dim TransID As Integer = Convert.ToInt32(da.SelectCommand.ExecuteScalar())
                'Inserting into period table
                da = New SqlDataAdapter("UpdatePeriodWiseBalance", con)
                da.SelectCommand.CommandType = CommandType.StoredProcedure
                da.SelectCommand.Transaction = trans
                da.SelectCommand.Parameters.AddWithValue("@RDocID", targetDocID)
                da.SelectCommand.Parameters.AddWithValue("@EID", eid)
                da.SelectCommand.Parameters.AddWithValue("@EffectiveDate", COnvertDateFormate(EffDate))
                da.SelectCommand.Parameters.AddWithValue("@BMODE", Mode)
                da.SelectCommand.Parameters.AddWithValue("@Operator", Oparator)
                da.SelectCommand.Parameters.AddWithValue("@AmtMapping", Quantity)
                da.SelectCommand.Parameters.AddWithValue("@tableName", tblName)
                da.SelectCommand.Parameters.AddWithValue("@DOCAAMOUNT", Amount)
                Dim pRes = da.SelectCommand.ExecuteNonQuery()
                'UpdatePeriodWiseBalance
            Else
                ret.Success = True
                ret.Message = "Update Relation not required."
            End If
        Catch ex As Exception
            Throw
        End Try
        Return ret
    End Function

    Public Function CanclePeriodWiseBalance(EID As Integer, DocID As Integer, DocumentType As String, con As SqlConnection, trans As SqlTransaction) As String
        Dim ret As String = ""
        Try
            'Update RDOCID
            Dim UpRFlg = False
            UpRFlg = ChkCancleDOCID(EID, DocID, DocumentType, con, trans)
            'Checking Period Wise Balance Setting
            If UpRFlg = True Then
                Dim ds As New DataSet()
                Dim da As New SqlDataAdapter("GETPwiseSetting", con)
                da.SelectCommand.CommandType = CommandType.StoredProcedure
                da.SelectCommand.Parameters.AddWithValue("@EID", EID)
                da.SelectCommand.Parameters.AddWithValue("@DOCID", DocID)
                da.SelectCommand.Parameters.AddWithValue("@DocumentType", DocumentType)
                da.SelectCommand.Transaction = trans
                da.Fill(ds)
                Dim DRDOCID As String = ""
                Dim CRDOCID As String = ""
                Dim CDOCID As String = ""
                Dim EffDate As String = ""
                Dim Amount As String = ""
                Dim RDOCTYpe As String = ""
                Dim RType As String = ""
                Dim OperationType As String = ""
                ''Updating RDOC
                For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                    DRDOCID = ds.Tables(0).Rows(i).Item("DRDOCID")
                    CRDOCID = ds.Tables(0).Rows(i).Item("CRDOCID")
                    CDOCID = ds.Tables(0).Rows(i).Item("CDOCID")
                    EffDate = ds.Tables(0).Rows(i).Item("EffDate")
                    Amount = ds.Tables(0).Rows(i).Item("Amount")
                    RDOCTYpe = ds.Tables(0).Rows(i).Item("RDOCTYpe")
                    RType = ds.Tables(0).Rows(i).Item("RType")
                    OperationType = ds.Tables(0).Rows(i).Item("OperationType")
                    Select Case OperationType.Trim.ToUpper
                        Case "SUBTRACT"
                            OperationType = "ADD"
                        Case "ADD"
                            OperationType = "SUBTRACT"
                    End Select

                    'Update All the inventroy one by one
                    UpdateRelation(EID, DocID, DocumentType, DRDOCID, CRDOCID, CDOCID, EffDate, Amount, RDOCTYpe, RType, OperationType, con, trans)
                Next
            End If
        Catch ex As Exception
            Throw New Exception("Error In Period Wise Balance Execution" & ex.InnerException.ToString)
        End Try
        Return ret
    End Function

    Public Function COnvertDateFormate(strdate As String) As String
        Dim str = ""
        Dim arrData As String() = Split(strdate, "/")
        str = arrData(1) & "/" & arrData(0) & "/" & arrData(2)
        Return str
    End Function




    Private Function GetPeriodBalanceInfo(DocumentType As String, EID As Integer) As DataSet
        Dim ds As New DataSet()
        Dim Query = "SET NOCOUNT ON;SELECT Effective_Date_Field,FormType,Balance_Field,Item_Number,Relation_Doc_type,Balance_type,balance_Maintenance_mode FROM MMM_MST_FORMS WHERE EID=" & EID & " AND FormName = '" & DocumentType & "'"
        Try
            Using con = New SqlConnection(conStr)
                Using da = New SqlDataAdapter(Query, con)
                    da.Fill(ds)
                End Using
            End Using
        Catch ex As Exception
        End Try
        Return ds
    End Function

    Private Function GetRelationFldInfo(DocumentType As String, EID As Integer, RelDOC As String) As DataSet
        Dim ds As New DataSet()
        Dim Query = "SET NOCOUNT ON;SELECT * FROM MMM_MST_Fields WHERE DocumentType='" & DocumentType & "' AND EID=" & EID & " AND DropDown like '%-" & RelDOC & "-%'"
        Try
            Using con = New SqlConnection(conStr)
                Using da = New SqlDataAdapter(Query, con)
                    da.Fill(ds)
                End Using
            End Using
        Catch ex As Exception
        End Try
        Return ds
    End Function

End Class

Public Class RelationResponse
    Public Property Success As Boolean
    Public Property Message As String
    Public Property SDocType As String
    Public Property ShowExtend As Boolean = False
End Class
