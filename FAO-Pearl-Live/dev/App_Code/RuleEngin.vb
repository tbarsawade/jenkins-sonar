Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports System.Globalization
Imports System.Drawing
Imports ExpressionEvaluator

Public Class RuleEngin
    Dim EID As Integer
    Dim Documenttype As String
    Dim DOCNature As String
    Dim WhenToRun As String
    Dim SControlID As String = ""
    Dim ControlValue As String = ""
    Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString

    Public Sub New(EID1 As Integer, DocumentType1 As String, DocNature1 As String, Whentorun1 As String)   'constructor
        EID = EID1
        Documenttype = DocumentType1
        DOCNature = DocNature1
        WhenToRun = Whentorun1
    End Sub
    Public Sub New(EID1 As Integer, DocumentType1 As String, DocNature1 As String, Whentorun1 As String, CtrlID As String)   'constructor
        EID = EID1
        Documenttype = DocumentType1
        DOCNature = DocNature1
        WhenToRun = Whentorun1
        SControlID = CtrlID
    End Sub
    Public Sub New()   'constructor
    End Sub

    Public Sub New(EID1 As Integer, DocumentType1 As String, DocNature1 As String, Whentorun1 As String, CtrlID As String, CtrlValue As String)   'constructor
        EID = EID1
        Documenttype = DocumentType1
        DOCNature = DocNature1
        WhenToRun = Whentorun1
        SControlID = CtrlID
        ControlValue = CtrlValue
    End Sub

    Public Function GetRules() As DataSet
        Dim ds As New DataSet()
        Using con As New SqlConnection(conStr)
            Using da As New SqlDataAdapter("getRules0", con)
                da.SelectCommand.CommandType = CommandType.StoredProcedure
                da.SelectCommand.Parameters.AddWithValue("@EID", EID)
                da.SelectCommand.Parameters.AddWithValue("@DOCUMENTTYPE", Documenttype)
                da.SelectCommand.Parameters.AddWithValue("@DOCNature", DOCNature)
                da.SelectCommand.Parameters.AddWithValue("@WhenToRun", WhenToRun)
                da.SelectCommand.Parameters.AddWithValue("@ControlField", SControlID)
                da.SelectCommand.Parameters.AddWithValue("@TargetSpecificControl", ControlValue)
                da.Fill(ds)
            End Using
        End Using
        Return ds
    End Function

    'Public Function ExecuteRule(lstParentField As List(Of UserData), lstChildField As List(Of UserData), IsChildForm As Boolean, Optional BaseDocType As String = "") As RuleResponse
    '    Dim ObjRet As New RuleResponse()
    '    Try
    '        Dim ds As New DataSet()
    '        'Getting all the concern rule of DocumentType
    '        ds = GetRules()
    '        If ds.Tables(0).Rows.Count > 0 Then
    '            'For the case where value resides on the form only 
    '            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
    '                Dim Expression = ""
    '                Expression = ds.Tables(0).Rows(i).Item("Condition").ToString.ToUpper
    '                For Each obj In lstParentField
    '                    Dim TestStr = "{FORM." & obj.DocumentType.Trim.ToUpper & "." & obj.DisplayName.ToUpper.Trim & "}"
    '                    If Expression.Contains(TestStr) Then
    '                        Expression = Expression.Replace(TestStr, obj.Values)
    '                    End If
    '                Next
    '                'in the Case of child item child field might be compared with parent
    '                If IsChildForm = True Then
    '                    For Each obj In lstChildField
    '                        Dim TestStr = "{FORM." & obj.DocumentType.Trim.ToUpper & "." & obj.DisplayName.ToUpper.Trim & "}"
    '                        If Expression.Contains(TestStr) Then
    '                            Expression = Expression.Replace(TestStr, obj.Values)
    '                        End If
    '                    Next
    '                End If
    '                'check any other document involved or not
    '                Dim dv As DataView = ds.Tables(1).DefaultView
    '                dv.RowFilter = "RuleID=" & ds.Tables(0).Rows(i).Item("RuleID")
    '                Dim FilterTable As DataTable = dv.ToTable
    '                If FilterTable.Rows.Count > 0 Then
    '                    Dim dsdata As New DataSet()
    '                    For k As Integer = 0 To FilterTable.Rows.Count - 1
    '                        'Getting where condition from Relation Identifier
    '                        'Creating where condition for getting result 
    '                        Dim OrderBy = ""
    '                        Dim whcond = ""
    '                        Dim sourceType As String = ""
    '                        Dim SourceName As String = Convert.ToString(FilterTable.Rows(k).Item("SourceName"))
    '                        sourceType = Convert.ToString(FilterTable.Rows(k).Item("sourceType"))
    '                        'Getting DocumentType Of main Form in case of document Type
    '                        Dim dsSName As New DataSet()
    '                        If sourceType.Trim.ToUpper = "ACTION DRIVEN" Then
    '                            Using con = New SqlConnection(conStr)
    '                                Using da = New SqlDataAdapter("SELECT EventName FROM MMM_MST_FORMS WHERE EID=" & EID & " AND FormName='" & SourceName & "'", con)
    '                                    da.Fill(dsSName)
    '                                End Using
    '                            End Using
    '                            If dsSName.Tables(0).Rows.Count > 0 Then
    '                                SourceName = Convert.ToString(dsSName.Tables(0).Rows(0).Item("EventName").ToString.Trim)
    '                            End If
    '                        End If
    '                        If BaseDocType = "" Then
    '                            whcond = "v" & EID & FilterTable.Rows(k).Item("TargetName").ToString.Trim.Replace(" ", "_") & "." & "[" & FilterTable.Rows(k).Item("T_RelationIdentifierField") & "]=" & "'{" & SourceName.ToString.Trim & "." & FilterTable.Rows(k).Item("S_relationidentifierField") & "}'"
    '                        Else
    '                            whcond = "v" & EID & BaseDocType.ToString.Trim.Replace(" ", "_") & "." & "[" & FilterTable.Rows(k).Item("T_RelationIdentifierField") & "]=" & "'{" & SourceName.ToString.Trim & "." & FilterTable.Rows(k).Item("S_relationidentifierField") & "}'"
    '                        End If

    '                        If Not String.IsNullOrEmpty(ds.Tables(1).Rows(k).Item("sortingfield").ToString.Trim) Then
    '                            OrderBy = "order by CAST(v" & EID & FilterTable.Rows(k).Item("TargetName").ToString.Trim.Replace(" ", "_") & "." & "[" & FilterTable.Rows(k).Item("sortingfield") & "] AS DATE) DESC"
    '                        End If
    '                        whcond = whcond.ToString

    '                        For Each obj In lstParentField
    '                            Dim TestStr = "{" & SourceName & "." & obj.DisplayName.ToUpper.Trim & "}"
    '                            If whcond.Contains(TestStr) Then
    '                                whcond = whcond.Replace(TestStr, obj.Values)
    '                            End If
    '                        Next
    '                        'in the Case of child item child field might be compared with parent
    '                        If IsChildForm = True Then
    '                            For Each obj In lstChildField
    '                                Dim TestStr = "{" & obj.DocumentType.Trim.ToUpper & "." & obj.DisplayName.ToUpper.Trim & "}"
    '                                If whcond.Contains(TestStr) Then
    '                                    whcond = whcond.Replace(TestStr, obj.Values)
    '                                End If
    '                            Next
    '                        End If
    '                        dsdata = GenerateQueryForPreLoad(EID, ds.Tables(1).Rows(k).Item("TargetName"), whcond, OrderBy)
    '                        If dsdata.Tables(0).Rows.Count > 0 Then
    '                            For Each column As DataColumn In dsdata.Tables(0).Columns
    '                                Dim TestStr = "{DS." & column.ColumnName.Trim.ToUpper & "}"
    '                                If Expression.Contains(TestStr) Then
    '                                    Expression = Expression.Replace(TestStr, dsdata.Tables(0).Rows(0).Item(column.ColumnName))
    '                                End If
    '                            Next
    '                        Else
    '                            Throw New Exception(ds.Tables(0).Rows(i).Item("ErrorMsg"))
    '                        End If
    '                    Next
    '                End If
    '                Dim Result = SplitFun("", Expression)
    '                ObjRet.HasRule = True
    '                If Result.ToString.ToUpper = "TRUE" Then
    '                    ObjRet.Success = True
    '                    ObjRet.Message = ""
    '                    ObjRet.ControlField = ds.Tables(0).Rows(i).Item("ControlField")
    '                    ObjRet.TargetField = ds.Tables(0).Rows(i).Item("TargetControlField")
    '                    ObjRet.SuccActionType = ds.Tables(0).Rows(i).Item("SuccActiontype")
    '                    ObjRet.SucessMessage = Convert.ToString(ds.Tables(0).Rows(i).Item("SuccMsg"))

    '                Else
    '                    ObjRet.Success = False
    '                    ObjRet.ErrorMessage = ds.Tables(0).Rows(i).Item("ErrorMsg")
    '                    ObjRet.ControlField = ds.Tables(0).Rows(i).Item("ControlField")
    '                    ObjRet.TargetField = ds.Tables(0).Rows(i).Item("TargetControlField")
    '                    ObjRet.FailActionType = ds.Tables(0).Rows(i).Item("FailActiontype")
    '                    ObjRet.SucessMessage = Convert.ToString(ds.Tables(0).Rows(i).Item("SuccMsg"))
    '                    Exit For
    '                End If
    '            Next
    '        Else
    '            'For the condition when there is no rule exists
    '            ObjRet.Success = True
    '            ObjRet.Message = "No rule exists for current documentType"
    '            ObjRet.HasRule = False
    '            ObjRet.ControlField = ""
    '            ObjRet.TargetField = ""
    '            ObjRet.SuccActionType = ""
    '        End If
    '    Catch ex As Exception
    '        ObjRet.Success = False
    '        ObjRet.ErrorMessage = ex.Message
    '        ObjRet.HasRule = True
    '        ObjRet.ControlField = ""
    '        ObjRet.TargetField = ""
    '        ObjRet.FailActionType = ""
    '    End Try
    '    Return ObjRet

    'End Function

    Public Function ExecuteRule(lstParentField As List(Of UserData), lstChildField As List(Of UserData), IsChildForm As Boolean, Optional BaseDocType As String = "", Optional Dr As DataRow = Nothing, Optional dRel As DataTable = Nothing) As RuleResponse
        Dim ObjRet As New RuleResponse()
        Try
            'Dim ds As New DataSet()
            ''Getting all the concern rule of DocumentType
            'ds = GetRules()
            If Not Dr Is Nothing Then
                'For the case where value resides on the form only 
                Dim Expression = ""
                Expression = Dr("Condition").ToString.ToUpper
                For Each obj In lstParentField
                    Dim TestStr = "{FORM." & obj.DocumentType.Trim.ToUpper & "." & obj.DisplayName.ToUpper.Trim & "}"
                    If Expression.Contains(TestStr) Then
                        'If obj.FieldType.ToUpper = "CHILD ITEM TOTAL" Then
                        '    If obj.Values = "" Then
                        '        obj.Values = "0"
                        '    End If
                        'End If

                        If obj.DataType.ToUpper() = "DATETIME" And obj.Values.Trim() = "" Then
                            obj.Values = DateTime.Now().ToString("dd/MM/yy")
                        End If

                        If Expression.Contains("|BLANK|") And obj.isBlankDate Then
                            obj.Values = ""
                            Expression = Expression.Replace("|BLANK|", "")
                        End If

                        Expression = Expression.Replace(TestStr, obj.Values.Replace(",", ""))

                    End If
                    ''And HttpContext.Current.Request.QueryString("SC").ToString() <> "PES"
                Next
                'Coading for Session Valued data
                If Expression.ToUpper.Contains("[SESSION(") Then
                    Dim stringSeparators() As String = New String() {"[SESSION("}
                    Dim results() As String
                    ' ...
                    results = Expression.ToUpper.Split(stringSeparators, StringSplitOptions.None)
                    For i As Integer = 0 To results.Length - 1
                        If results(i).ToString().Contains(")]") Then
                            Dim value As String = HttpContext.Current.Session("" & results(i).Substring(1, results(i).IndexOf("]") - 3) & "")
                            Expression = Expression.ToUpper.Replace("[SESSION(""" & results(i).Substring(1, results(i).IndexOf("]") - 3) & """)]", value)
                        End If
                    Next
                    'Dim tokenString As String() = Expression.ToUpper.Split("SESSION(")
                    'For i As Integer = 0 To tokenString.Length - 1
                    'Next
                End If
                'in the Case of child item child field might be compared with parent
                If IsChildForm = True Then
                    For Each obj In lstChildField
                        Dim TestStr = "{FORM." & obj.DocumentType.Trim.ToUpper & "." & obj.DisplayName.ToUpper.Trim & "}"
                        If Expression.Contains(TestStr) Then
                            Expression = Expression.Replace(TestStr, obj.Values.Replace(",", ""))
                        End If
                    Next
                End If
                'check any other document involved or not
                Dim ruleid As Integer = Dr("RuleID")
                Dim dv As DataView = dRel.DefaultView
                dv.RowFilter = "RuleID=" & Dr("RuleID")
                Dim FilterTable As DataTable = dv.ToTable
                If FilterTable.Rows.Count > 0 Then
                    Dim dsdata As New DataSet()
                    For k As Integer = 0 To FilterTable.Rows.Count - 1
                        'Getting where condition from Relation Identifier
                        'Creating where condition for getting result 
                        Dim OrderBy = ""
                        Dim whcond = ""
                        Dim sourceType As String = ""
                        Dim SourceName As String = Convert.ToString(FilterTable.Rows(k).Item("SourceName"))
                        sourceType = Convert.ToString(FilterTable.Rows(k).Item("sourceType"))
                        'Getting DocumentType Of main Form in case of document Type
                        Dim dsSName As New DataSet()
                        If Not String.IsNullOrEmpty(FilterTable.Rows(k).Item("AdvanceSearch").ToString.Trim) Then
                            Dim AdvanceSearch As String = FilterTable.Rows(k).Item("AdvanceSearch").ToString.Trim.ToUpper()

                            For Each obj In lstParentField
                                Dim TestStr = "{" & SourceName & "." & obj.DisplayName.ToUpper.Trim & "}"
                                If AdvanceSearch.Contains(TestStr) Then
                                    If String.IsNullOrEmpty(obj.Values) Then
                                        AdvanceSearch = AdvanceSearch.Replace(TestStr, "''")
                                    Else
                                        AdvanceSearch = AdvanceSearch.Replace(TestStr, obj.Values)
                                    End If
                                End If
                            Next
                            For Each obj In lstParentField
                                Dim TestStr = "{FORM." & obj.DocumentType.Trim.ToUpper & "." & obj.DisplayName.ToUpper.Trim & "}"
                                If AdvanceSearch.Contains(TestStr) Then
                                    If String.IsNullOrEmpty(obj.Values) Then
                                        AdvanceSearch = AdvanceSearch.Replace(TestStr, "''")
                                    Else
                                        AdvanceSearch = AdvanceSearch.Replace(TestStr, obj.Values)
                                    End If
                                End If
                            Next
                            'for Child Item
                            If Not IsNothing(lstChildField) Then
                                For Each obj In lstChildField
                                    Dim TestStr = "{FORM." & obj.DocumentType.Trim.ToUpper & "." & obj.DisplayName.ToUpper.Trim & "}"
                                    If AdvanceSearch.Contains(TestStr) Then
                                        If String.IsNullOrEmpty(obj.Values) Then
                                            AdvanceSearch = AdvanceSearch.Replace(TestStr, "''")
                                        Else
                                            AdvanceSearch = AdvanceSearch.Replace(TestStr, obj.Values)
                                        End If
                                    End If
                                Next
                            End If
                            Using con = New SqlConnection(conStr)
                                Using da = New SqlDataAdapter(AdvanceSearch.ToString(), con)
                                    da.Fill(dsdata)
                                End Using
                            End Using
                        Else
                            If sourceType.Trim.ToUpper = "ACTION DRIVEN" Then
                                Using con = New SqlConnection(conStr)
                                    Using da = New SqlDataAdapter("SELECT EventName FROM MMM_MST_FORMS WHERE EID=" & EID & " AND FormName='" & SourceName & "'", con)
                                        da.Fill(dsSName)
                                    End Using
                                End Using
                                If dsSName.Tables(0).Rows.Count > 0 Then
                                    SourceName = Convert.ToString(dsSName.Tables(0).Rows(0).Item("EventName").ToString.Trim)
                                End If
                            End If
                            If (UCase(BaseDocType) = "PETTY CASH VOUCHER HUB" And EID = 62 And ruleid = 80) Or (ruleid = 561 And EID = 46) Or ((UCase(BaseDocType) = "PETTY CASH VOUCHER STORE") And EID = 144) Or ((UCase(BaseDocType) = "PETTY CASH VOUCHER HUB") And EID = 165) Or (UCase(BaseDocType) Like "PETTY CASH VOUCHER%") Then
                                BaseDocType = ""
                            End If
                            If BaseDocType = "" Then
                                whcond = "v" & EID & FilterTable.Rows(k).Item("TargetName").ToString.Trim.Replace(" ", "_") & "." & "[" & FilterTable.Rows(k).Item("T_RelationIdentifierField") & "]=" & "'{" & SourceName.ToString.Trim & "." & FilterTable.Rows(k).Item("S_relationidentifierField") & "}'"
                            Else
                                whcond = "v" & EID & BaseDocType.ToString.Trim.Replace(" ", "_") & "." & "[" & FilterTable.Rows(k).Item("T_RelationIdentifierField") & "]=" & "'{" & SourceName.ToString.Trim & "." & FilterTable.Rows(k).Item("S_relationidentifierField") & "}'"
                            End If

                            If Not String.IsNullOrEmpty(dRel.Rows(k).Item("sortingfield").ToString.Trim) Then
                                OrderBy = "order by CAST(v" & EID & FilterTable.Rows(k).Item("TargetName").ToString.Trim.Replace(" ", "_") & "." & "[" & FilterTable.Rows(k).Item("sortingfield") & "] AS DATE) DESC"
                            End If
                            whcond = whcond.ToString

                            For Each obj In lstParentField
                                Dim TestStr = "{" & SourceName & "." & obj.DisplayName.ToUpper.Trim & "}"
                                If whcond.Contains(TestStr) Then
                                    whcond = whcond.Replace(TestStr, obj.Values)
                                End If
                            Next
                            'in the Case of child item child field might be compared with parent
                            If IsChildForm = True Then
                                For Each obj In lstChildField
                                    Dim TestStr = "{" & obj.DocumentType.Trim.ToUpper & "." & obj.DisplayName.ToUpper.Trim & "}"
                                    If whcond.Contains(TestStr) Then
                                        whcond = whcond.Replace(TestStr, obj.Values)
                                    End If
                                Next
                            End If
                            dsdata = GenerateQueryForPreLoad(EID, dRel.Rows(k).Item("TargetName"), whcond, OrderBy)
                        End If

                        If dsdata.Tables(0).Rows.Count > 0 Then
                            For Each column As DataColumn In dsdata.Tables(0).Columns
                                Dim TestStr = "{DS." & column.ColumnName.Trim.ToUpper & "}"
                                If Expression.Contains(TestStr) Then
                                    Expression = Expression.Replace(TestStr, dsdata.Tables(0).Rows(0).Item(column.ColumnName))
                                End If
                            Next
                        Else
                            Throw New Exception(Dr("ErrorMsg").ToString())
                        End If
                    Next
                End If
                Dim Result = SplitFun("", Expression)
                ObjRet.HasRule = True
                '' new for duplicacy check through rule engine
                If Result.ToString.ToUpper = "TRUE" And Dr("SuccActiontype").ToString.ToUpper = "DUPLICACY" Then
                    Result = CheckDuplicate(Dr("TargetControlField").ToString(), Documenttype, EID, lstParentField)
                ElseIf Result.ToString.ToUpper = "TRUE" And Dr("SuccActiontype").ToString.ToUpper = "DUPLICACYMASTER" Then
                    Result = CheckDuplicateFromMaster(Dr("TargetControlField").ToString(), Documenttype, EID, lstParentField, Dr("TargetSpecificControl").ToString())
                ElseIf Result.ToString.ToUpper = "FALSE" And Dr("SuccActiontype").ToString.ToUpper = "DUPLICACYMASTER" Then
                    'Result = CheckDuplicateFromMaster(Dr("TargetControlField").ToString(), Documenttype, EID, lstParentField, Dr("TargetSpecificControl").ToString())
                    Result = "TRUE"
                ElseIf Result.ToString.ToUpper = "FALSE" And Dr("SuccActiontype").ToString.ToUpper = "DUPLICACY" Then
                    Result = "TRUE"
                End If
                '' new for duplicacy check through rule engine

                If Result.ToString.ToUpper = "TRUE" Then
                    ObjRet.Success = True
                    ObjRet.Message = ""
                    ObjRet.ControlField = Dr("ControlField")
                    ObjRet.TargetField = Dr("TargetControlField")
                    ObjRet.SuccActionType = Dr("SuccActiontype")
                    ObjRet.SucessMessage = Convert.ToString(Dr("SuccMsg"))
                Else
                    ObjRet.Success = False
                    ObjRet.ErrorMessage = Dr("ErrorMsg")
                    ObjRet.ControlField = Dr("ControlField")
                    ObjRet.TargetField = Dr("TargetControlField")
                    ObjRet.FailActionType = Dr("FailActiontype")
                    ObjRet.SucessMessage = Convert.ToString(Dr("SuccMsg"))
                End If
            Else
                'For the condition when there is no rule exists
                ObjRet.Success = True
                ObjRet.Message = "No rule exists for current documentType"
                ObjRet.HasRule = False
                ObjRet.ControlField = ""
                ObjRet.TargetField = ""
                ObjRet.SuccActionType = ""
            End If
        Catch ex As Exception
            ObjRet.Success = False
            ObjRet.ErrorMessage = ex.Message
            ObjRet.HasRule = True
            ObjRet.ControlField = ""
            ObjRet.TargetField = ""
            ObjRet.FailActionType = ""
        End Try
        Return ObjRet
    End Function

    Public Function ExecuteRuleActionChild(lstParentField As List(Of UserData), lstChildField As List(Of UserData), IsChildForm As Boolean, Optional BaseDocType As String = "", Optional Dr As DataRow = Nothing, Optional dRel As DataTable = Nothing) As RuleResponse
        Dim ObjRet As New RuleResponse()
        Try
            'Dim ds As New DataSet()
            ''Getting all the concern rule of DocumentType
            'ds = GetRules()
            If Not Dr Is Nothing Then
                'For the case where value resides on the form only 
                Dim Expression = ""
                Expression = Dr("Condition").ToString.ToUpper

                If IsChildForm = True Then
                    For Each obj In lstChildField
                        Dim TestStr = "{FORM." & obj.DocumentType.Trim.ToUpper & "." & obj.DisplayName.ToUpper.Trim & "}"
                        If Expression.Contains(TestStr) Then
                            Expression = Expression.Replace(TestStr, obj.Values.Replace(",", ""))
                        End If
                    Next
                End If
                'check any other document involved or not
                Dim ruleid As Integer = Dr("RuleID")
                Dim dv As DataView = dRel.DefaultView
                dv.RowFilter = "RuleID=" & Dr("RuleID")
                Dim FilterTable As DataTable = dv.ToTable
                If FilterTable.Rows.Count > 0 Then
                    Dim dsdata As New DataSet()
                    For k As Integer = 0 To FilterTable.Rows.Count - 1
                        'Getting where condition from Relation Identifier
                        'Creating where condition for getting result 
                        Dim OrderBy = ""
                        Dim whcond = ""
                        Dim sourceType As String = ""
                        Dim SourceName As String = Convert.ToString(FilterTable.Rows(k).Item("SourceName"))
                        sourceType = Convert.ToString(FilterTable.Rows(k).Item("sourceType"))
                        'Getting DocumentType Of main Form in case of document Type
                        Dim dsSName As New DataSet()
                        If Not String.IsNullOrEmpty(FilterTable.Rows(k).Item("AdvanceSearch").ToString.Trim) Then
                            Dim AdvanceSearch As String = FilterTable.Rows(k).Item("AdvanceSearch").ToString.Trim.ToUpper()

                            For Each obj In lstParentField
                                Dim TestStr = "{" & SourceName & "." & obj.DisplayName.ToUpper.Trim & "}"
                                If AdvanceSearch.Contains(TestStr) Then
                                    If String.IsNullOrEmpty(obj.Values) Then
                                        AdvanceSearch = AdvanceSearch.Replace(TestStr, "''")
                                    Else
                                        AdvanceSearch = AdvanceSearch.Replace(TestStr, obj.Values)
                                    End If
                                End If
                            Next
                            For Each obj In lstParentField
                                Dim TestStr = "{FORM." & obj.DocumentType.Trim.ToUpper & "." & obj.DisplayName.ToUpper.Trim & "}"
                                If AdvanceSearch.Contains(TestStr) Then
                                    If String.IsNullOrEmpty(obj.Values) Then
                                        AdvanceSearch = AdvanceSearch.Replace(TestStr, "''")
                                    Else
                                        AdvanceSearch = AdvanceSearch.Replace(TestStr, obj.Values)
                                    End If
                                End If
                            Next
                            'for Child Item
                            If Not IsNothing(lstChildField) Then
                                For Each obj In lstChildField
                                    Dim TestStr = "{FORM." & obj.DocumentType.Trim.ToUpper & "." & obj.DisplayName.ToUpper.Trim & "}"
                                    If AdvanceSearch.Contains(TestStr) Then
                                        If String.IsNullOrEmpty(obj.Values) Then
                                            AdvanceSearch = AdvanceSearch.Replace(TestStr, "''")
                                        Else
                                            AdvanceSearch = AdvanceSearch.Replace(TestStr, obj.Values)
                                        End If
                                    End If
                                Next
                            End If
                            Using con = New SqlConnection(conStr)
                                Using da = New SqlDataAdapter(AdvanceSearch.ToString(), con)
                                    da.Fill(dsdata)
                                End Using
                            End Using
                        Else
                            If sourceType.Trim.ToUpper = "ACTION DRIVEN" Then
                                Using con = New SqlConnection(conStr)
                                    Using da = New SqlDataAdapter("SELECT EventName FROM MMM_MST_FORMS WHERE EID=" & EID & " AND FormName='" & SourceName & "'", con)
                                        da.Fill(dsSName)
                                    End Using
                                End Using
                                If dsSName.Tables(0).Rows.Count > 0 Then
                                    SourceName = Convert.ToString(dsSName.Tables(0).Rows(0).Item("EventName").ToString.Trim)
                                End If
                            End If
                            If (UCase(BaseDocType) = "PETTY CASH VOUCHER HUB" And EID = 62 And ruleid = 80) Or (ruleid = 561 And EID = 46) Or ((UCase(BaseDocType) = "PETTY CASH VOUCHER STORE") And EID = 144) Or ((UCase(BaseDocType) = "PETTY CASH VOUCHER HUB") And EID = 165) Or (UCase(BaseDocType) Like "PETTY CASH VOUCHER%") Then
                                BaseDocType = ""
                            End If
                            If BaseDocType = "" Then
                                whcond = "v" & EID & FilterTable.Rows(k).Item("TargetName").ToString.Trim.Replace(" ", "_") & "." & "[" & FilterTable.Rows(k).Item("T_RelationIdentifierField") & "]=" & "'{" & SourceName.ToString.Trim & "." & FilterTable.Rows(k).Item("S_relationidentifierField") & "}'"
                            Else
                                whcond = "v" & EID & BaseDocType.ToString.Trim.Replace(" ", "_") & "." & "[" & FilterTable.Rows(k).Item("T_RelationIdentifierField") & "]=" & "'{" & SourceName.ToString.Trim & "." & FilterTable.Rows(k).Item("S_relationidentifierField") & "}'"
                            End If

                            If Not String.IsNullOrEmpty(dRel.Rows(k).Item("sortingfield").ToString.Trim) Then
                                OrderBy = "order by CAST(v" & EID & FilterTable.Rows(k).Item("TargetName").ToString.Trim.Replace(" ", "_") & "." & "[" & FilterTable.Rows(k).Item("sortingfield") & "] AS DATE) DESC"
                            End If
                            whcond = whcond.ToString

                            For Each obj In lstParentField
                                Dim TestStr = "{" & SourceName & "." & obj.DisplayName.ToUpper.Trim & "}"
                                If whcond.Contains(TestStr) Then
                                    whcond = whcond.Replace(TestStr, obj.Values)
                                End If
                            Next
                            'in the Case of child item child field might be compared with parent
                            If IsChildForm = True Then
                                For Each obj In lstChildField
                                    Dim TestStr = "{" & obj.DocumentType.Trim.ToUpper & "." & obj.DisplayName.ToUpper.Trim & "}"
                                    If whcond.Contains(TestStr) Then
                                        whcond = whcond.Replace(TestStr, obj.Values)
                                    End If
                                Next
                            End If
                            dsdata = GenerateQueryForPreLoad(EID, dRel.Rows(k).Item("TargetName"), whcond, OrderBy)
                        End If

                        If dsdata.Tables(0).Rows.Count > 0 Then
                            For Each column As DataColumn In dsdata.Tables(0).Columns
                                Dim TestStr = "{DS." & column.ColumnName.Trim.ToUpper & "}"
                                If Expression.Contains(TestStr) Then
                                    Expression = Expression.Replace(TestStr, dsdata.Tables(0).Rows(0).Item(column.ColumnName))
                                End If
                            Next
                        Else
                            Throw New Exception(Dr("ErrorMsg").ToString())
                        End If
                    Next
                End If
                Dim Result = SplitFun("", Expression)
                ObjRet.HasRule = True
                '' new for duplicacy check through rule engine
                If Result.ToString.ToUpper = "TRUE" And Dr("SuccActiontype").ToString.ToUpper = "DUPLICACY" Then
                    Result = CheckDuplicate(Dr("TargetControlField").ToString(), Documenttype, EID, lstParentField)
                ElseIf Result.ToString.ToUpper = "TRUE" And Dr("SuccActiontype").ToString.ToUpper = "DUPLICACYMASTER" Then
                    Result = CheckDuplicateFromMaster(Dr("TargetControlField").ToString(), Documenttype, EID, lstParentField, Dr("TargetSpecificControl").ToString())
                ElseIf Result.ToString.ToUpper = "FALSE" And Dr("SuccActiontype").ToString.ToUpper = "DUPLICACYMASTER" Then
                    Result = CheckDuplicateFromMaster(Dr("TargetControlField").ToString(), Documenttype, EID, lstParentField, Dr("TargetSpecificControl").ToString())
                ElseIf Result.ToString.ToUpper = "FALSE" And Dr("SuccActiontype").ToString.ToUpper = "DUPLICACY" Then
                    Result = "TRUE"
                End If
                '' new for duplicacy check through rule engine

                If Result.ToString.ToUpper = "TRUE" Then
                    ObjRet.Success = True
                    ObjRet.Message = ""
                    ObjRet.ControlField = Dr("ControlField")
                    ObjRet.TargetField = Dr("TargetControlField")
                    ObjRet.SuccActionType = Dr("SuccActiontype")
                    ObjRet.SucessMessage = Convert.ToString(Dr("SuccMsg"))
                Else
                    ObjRet.Success = False
                    ObjRet.ErrorMessage = Dr("ErrorMsg")
                    ObjRet.ControlField = Dr("ControlField")
                    ObjRet.TargetField = Dr("TargetControlField")
                    ObjRet.FailActionType = Dr("FailActiontype")
                    ObjRet.SucessMessage = Convert.ToString(Dr("SuccMsg"))
                End If
            Else
                'For the condition when there is no rule exists
                ObjRet.Success = True
                ObjRet.Message = "No rule exists for current documentType"
                ObjRet.HasRule = False
                ObjRet.ControlField = ""
                ObjRet.TargetField = ""
                ObjRet.SuccActionType = ""
            End If
        Catch ex As Exception
            ObjRet.Success = False
            ObjRet.ErrorMessage = ex.Message
            ObjRet.HasRule = True
            ObjRet.ControlField = ""
            ObjRet.TargetField = ""
            ObjRet.FailActionType = ""
        End Try
        Return ObjRet
    End Function
    Public Function ExecuteRuleTNE(lstParentField As List(Of UserData), lstChildField As List(Of UserData), IsChildForm As Boolean, Optional BaseDocType As String = "", Optional Dr As DataRow = Nothing, Optional dRel As DataTable = Nothing) As RuleResponse
        Dim ObjRet As New RuleResponse()
        Try
            'Dim ds As New DataSet()
            ''Getting all the concern rule of DocumentType
            'ds = GetRules()
            If Not Dr Is Nothing Then
                'For the case where value resides on the form only 
                Dim Expression = ""
                Expression = Dr("Condition").ToString.ToUpper
                For Each obj In lstParentField
                    Dim TestStr = "{FORM." & obj.DocumentType.Trim.ToUpper & "." & obj.DisplayName.ToUpper.Trim & "}"
                    If Expression.Contains(TestStr) Then
                        'If obj.FieldType.ToUpper = "CHILD ITEM TOTAL" Then
                        '    If obj.Values = "" Then
                        '        obj.Values = "0"
                        '    End If
                        'End If
                        Expression = Expression.Replace(TestStr, obj.Values)
                    End If

                Next
                'in the Case of child item child field might be compared with parent
                If IsChildForm = True Then
                    For Each obj In lstChildField
                        Dim TestStr = "{FORM." & obj.DocumentType.Trim.ToUpper & "." & obj.DisplayName.ToUpper.Trim & "}"
                        If Expression.Contains(TestStr) Then
                            Expression = Expression.Replace(TestStr, obj.Values)
                        End If
                    Next
                End If
                'check any other document involved or not
                Dim Result = ""
                Dim ruleid As Integer = Dr("RuleID")
                Dim dv As DataView = dRel.DefaultView
                dv.RowFilter = "RuleID=" & Dr("RuleID")
                Dim FilterTable As DataTable = dv.ToTable
                If FilterTable.Rows.Count > 0 Then
                    Dim dsdata As New DataSet()
                    For k As Integer = 0 To FilterTable.Rows.Count - 1
                        'Getting where condition from Relation Identifier
                        'Creating where condition for getting result 
                        Dim OrderBy = ""
                        Dim whcond = ""
                        Dim sourceType As String = ""
                        Dim SourceName As String = Convert.ToString(FilterTable.Rows(k).Item("SourceName"))
                        sourceType = Convert.ToString(FilterTable.Rows(k).Item("sourceType"))
                        'Getting DocumentType Of main Form in case of document Type
                        Dim dsSName As New DataSet()
                        'Getting customise functin for gettig specific data from database
                        If Not String.IsNullOrEmpty(FilterTable.Rows(k).Item("AdvanceSearch").ToString.Trim) Then
                            Dim AdvanceSearch As String = FilterTable.Rows(k).Item("AdvanceSearch").ToString.Trim.ToUpper()
                            For Each obj In lstParentField
                                Dim TestStr = "{" & SourceName & "." & obj.DisplayName.ToUpper.Trim & "}"
                                If AdvanceSearch.Contains(TestStr) Then
                                    If String.IsNullOrEmpty(obj.Values) Then
                                        AdvanceSearch = AdvanceSearch.Replace(TestStr, "''")
                                    Else
                                        AdvanceSearch = AdvanceSearch.Replace(TestStr, obj.Values)
                                    End If

                                End If
                            Next
                            Using con = New SqlConnection(conStr)
                                Using da = New SqlDataAdapter(AdvanceSearch.ToString(), con)
                                    da.Fill(dsdata)
                                End Using
                            End Using
                        Else
                            If sourceType.Trim.ToUpper = "ACTION DRIVEN" Then
                                Using con = New SqlConnection(conStr)
                                    Using da = New SqlDataAdapter("SELECT EventName FROM MMM_MST_FORMS WHERE EID=" & EID & " AND FormName='" & SourceName & "'", con)
                                        da.Fill(dsSName)
                                    End Using
                                End Using
                                If dsSName.Tables(0).Rows.Count > 0 Then
                                    SourceName = Convert.ToString(dsSName.Tables(0).Rows(0).Item("EventName").ToString.Trim)
                                End If
                            End If
                            If UCase(BaseDocType) = "PETTY CASH VOUCHER HUB" And EID = 62 And ruleid = 80 Then
                                BaseDocType = ""
                            End If
                            If BaseDocType = "" Then
                                whcond = "v" & EID & FilterTable.Rows(k).Item("TargetName").ToString.Trim.Replace(" ", "_") & "." & "[" & FilterTable.Rows(k).Item("T_RelationIdentifierField") & "]=" & "'{" & SourceName.ToString.Trim & "." & FilterTable.Rows(k).Item("S_relationidentifierField") & "}'"
                            Else
                                whcond = "v" & EID & BaseDocType.ToString.Trim.Replace(" ", "_") & "." & "[" & FilterTable.Rows(k).Item("T_RelationIdentifierField") & "]=" & "'{" & SourceName.ToString.Trim & "." & FilterTable.Rows(k).Item("S_relationidentifierField") & "}'"
                            End If
                            If Not String.IsNullOrEmpty(dRel.Rows(k).Item("sortingfield").ToString.Trim) Then
                                OrderBy = "order by CAST(v" & EID & FilterTable.Rows(k).Item("TargetName").ToString.Trim.Replace(" ", "_") & "." & "[" & FilterTable.Rows(k).Item("sortingfield") & "] AS DATE) DESC"
                            End If
                            whcond = whcond.ToString

                            For Each obj In lstParentField
                                Dim TestStr = "{" & SourceName & "." & obj.DisplayName.ToUpper.Trim & "}"
                                If whcond.Contains(TestStr) Then
                                    whcond = whcond.Replace(TestStr, obj.Values)
                                End If
                            Next
                            'in the Case of child item child field might be compared with parent
                            If IsChildForm = True Then
                                For Each obj In lstChildField
                                    Dim TestStr = "{" & obj.DocumentType.Trim.ToUpper & "." & obj.DisplayName.ToUpper.Trim & "}"
                                    If whcond.Contains(TestStr) Then
                                        whcond = whcond.Replace(TestStr, obj.Values)
                                    End If
                                Next
                            End If
                            dsdata = GenerateQueryForPreLoad(EID, dRel.Rows(k).Item("TargetName"), whcond, OrderBy)
                        End If
                        If dsdata.Tables(0).Rows.Count > 0 Then
                            For Each drdata As DataRow In dsdata.Tables(0).Rows
                                For Each column As DataColumn In dsdata.Tables(0).Columns
                                    Dim TestStr = "{DS." & column.ColumnName.Trim.ToUpper & "}"
                                    If Expression.Contains(TestStr) Then
                                        Expression = Expression.Replace(TestStr, drdata.Item(column.ColumnName))
                                    End If
                                Next
                                Result = SplitFun("", Expression)
                                If Result.ToString.ToUpper = "TRUE" Then
                                    Exit For
                                End If
                            Next

                        Else
                            Throw New Exception(Dr("ErrorMsg").ToString())
                        End If
                    Next
                End If
                'Dim Result = SplitFun("", Expression)
                ObjRet.HasRule = True
                If Result.ToString.ToUpper = "TRUE" And Dr("SuccActiontype").ToString.ToUpper = "DUPLICACY" Then
                    Result = CheckDuplicate(Dr("TargetControlField").ToString(), Documenttype, EID, lstParentField)
                ElseIf Result.ToString.ToUpper = "TRUE" And Dr("SuccActiontype").ToString.ToUpper = "DUPLICACYMASTER" Then
                    Result = CheckDuplicateFromMaster(Dr("TargetControlField").ToString(), Documenttype, EID, lstParentField, Dr("TargetSpecificControl").ToString())
                ElseIf Result.ToString.ToUpper = "FALSE" And Dr("SuccActiontype").ToString.ToUpper = "DUPLICACYMASTER" Then
                    Result = CheckDuplicateFromMaster(Dr("TargetControlField").ToString(), Documenttype, EID, lstParentField, Dr("TargetSpecificControl").ToString())
                ElseIf Result.ToString.ToUpper = "FALSE" And Dr("SuccActiontype").ToString.ToUpper = "DUPLICACY" Then
                    Result = "TRUE"
                End If
                If Result.ToString.ToUpper = "TRUE" Then
                    ObjRet.Success = True
                    ObjRet.Message = ""
                    ObjRet.ControlField = Dr("ControlField")
                    ObjRet.TargetField = Dr("TargetControlField")
                    ObjRet.SuccActionType = Dr("SuccActiontype")
                    ObjRet.SucessMessage = Convert.ToString(Dr("SuccMsg"))

                Else
                    ObjRet.Success = False
                    ObjRet.ErrorMessage = Dr("ErrorMsg")
                    ObjRet.ControlField = Dr("ControlField")
                    ObjRet.TargetField = Dr("TargetControlField")
                    ObjRet.FailActionType = Dr("FailActiontype")
                    ObjRet.SucessMessage = Convert.ToString(Dr("SuccMsg"))
                End If

            Else
                'For the condition when there is no rule exists
                ObjRet.Success = True
                ObjRet.Message = "No rule exists for current documentType"
                ObjRet.HasRule = False
                ObjRet.ControlField = ""
                ObjRet.TargetField = ""
                ObjRet.SuccActionType = ""
            End If
        Catch ex As Exception
            ObjRet.Success = False
            ObjRet.ErrorMessage = ex.Message
            ObjRet.HasRule = True
            ObjRet.ControlField = ""
            ObjRet.TargetField = ""
            ObjRet.FailActionType = ""
        End Try
        Return ObjRet

    End Function

    Public Function ADD(Parameter As String) As Double
        Dim Result As Double = 0
        If Parameter.Contains(",") Then
            Dim arr = Parameter.Split(",")
            For i As Integer = 0 To arr.Length - 1
                Dim var As Double
                Try
                    var = Double.Parse(arr(i))
                Catch ex As Exception
                    var = 0
                End Try
                Result = Result + var
            Next
        Else
            Result = Parameter
        End If
        Return Result
    End Function

    Public Function Max(Parameter As String) As Double
        Dim Result As Double = 0
        Dim arr = Parameter.Split(New String() {","}, StringSplitOptions.RemoveEmptyEntries)
        Dim arrList As New ArrayList
        For i As Integer = 0 To arr.Count - 1
            Try
                arrList.Add(Double.Parse(arr(i)))
            Catch ex As Exception
                arrList.Add(Double.Parse(arr(i)))
            End Try
        Next
        Result = arrList.Cast(Of Double)().Max()
        Return Result
    End Function

    Public Function Min(Parameter As String) As Double
        Dim Result As Double = 0
        Dim arr = Parameter.Split(New String() {","}, StringSplitOptions.RemoveEmptyEntries)
        Dim arrList As New ArrayList
        For i As Integer = 0 To arr.Count - 1
            Try
                arrList.Add(Double.Parse(arr(i)))
            Catch ex As Exception
                arrList.Add(Double.Parse(arr(i)))
            End Try
        Next
        Result = arrList.Cast(Of Double)().Min()
        Return Result
    End Function

    Public Function Ceiling(Parameter As String) As Double
        Dim Result As Double = 0
        Try
            Result = Math.Ceiling(Double.Parse(Parameter))
        Catch ex As Exception
            Result = 0
            Return Result
        End Try
        Return Result
    End Function

    Public Function Floor(Parameter As String) As Double
        Dim Result As Double = 0
        Try
            Result = Math.Floor(Double.Parse(Parameter))
        Catch ex As Exception
            Result = 0
            Return Result
        End Try
        Return Result
    End Function

    Public Function Abs(Parameter As String) As Double
        Dim Result As Double = 0
        Try
            Result = Math.Abs(Double.Parse(Parameter))
        Catch ex As Exception
            Result = 0
            Return Result
        End Try
        Return Result
    End Function

    'List OF datetime function

    Public Function GETCURDATE() As String
        Dim Result As String = ""
        Dim dt As Date = Date.Today
        Result = dt.ToString("dd/MM/yy", CultureInfo.InvariantCulture)
        Return Result
    End Function

    'Public Function GETDATE() As String
    '    Dim Result As String = ""
    '    Dim dt As Date = Date.Today
    '    Result = dt.ToString("dd/MM/yy", CultureInfo.InvariantCulture)
    '    Return Result
    'End Function

    'Public Function GetDays() As String
    '    Dim Month As Integer
    '    Dim Year As Integer
    '    Dim dt As Date = Date.Today
    '    Month = dt.Month()
    '    Year = dt.Year()
    '    ' daysInJuly gets 31. '
    '    Dim daysInMonth As Integer = System.DateTime.DaysInMonth(Year, Month)
    '    Return daysInMonth
    'End Function       

    Public Function GetDays(Parameter As String) As String
        Dim Month As String
        Dim Year As String
        Dim arr = Parameter.Split("/")
        Month = arr(0)
        Year = "20" & arr(1)
        ' daysInJuly gets 31. '
        Dim daysInMonth As Integer = System.DateTime.DaysInMonth(Year, Month)
        Return daysInMonth
    End Function

    Public Function GETDATE(Parameter As String) As String
        Dim Date1 As String
        Dim arr = Parameter.Split("/")
        Date1 = arr(0)
        Return Date1
    End Function

    Public Function GETYEAR(Parameter As String) As String
        Dim Year As String
        Dim arr = Parameter.Split("/")
        Year = arr(2)
        Return Year
    End Function

    Public Function GETMONTH(Parameter As String) As String
        Dim Month As String
        Dim arr = Parameter.Split("/")
        Month = arr(1)
        Return Month
    End Function

    Public Function DATERANGE(Parameter As String) As String
        Dim ret = ""
        Dim arr = Parameter.Split(",")
        Dim d1 = arr(0).Split("/")
        Dim d2 = arr(1).Split("/")
        Dim a As New Date(d1(2), d1(1), d1(0))
        Dim b As New Date(d2(2), d2(1), d2(0))
        ret = (a - b).TotalDays
        Return ret
    End Function

    Public Function MonthDiff(Parameter As String) As String
        Dim arrDates = Parameter.Split(",")
        Dim arr = arrDates(0).Split("/")
        Dim arr1 = arrDates(1).Split("/")
        Dim dt1 As New Date(arr(2), arr(1), arr(0))
        Dim dt2 As New Date(arr1(2), arr1(1), arr1(0))
        Dim Result = ""
        Result = ((dt1.Year - dt2.Year) * 12) + dt1.Month - dt2.Month
        Return Result

    End Function

    Public Function GetLastWorkingDayOFCurMonth(Parameter As String) As String
        Dim Result = ""
        Dim lastBusinessDay As New DateTime()
        Dim arr = Parameter.Split("/")
        'Dim i = DateTime.DaysInMonth(arr(1), arr(2))
        Dim date1 As New Date(arr(2), arr(1), arr(0))
        Dim date2 = date1.AddMonths(-1)
        Dim lastDay = DateTime.DaysInMonth(date2.Year, date2.Month)
        Dim dayOfWeek__1 = date1.AddDays(lastDay - date1.Day).DayOfWeek
        'consider other holidays --implement from here
        'Result = (If(dayOfWeek__1 = DayOfWeek.Saturday, lastDay - 1, If(dayOfWeek__1 = DayOfWeek.Sunday, lastDay - 2, lastDay)))
        Result = (If(dayOfWeek__1 = DayOfWeek.Sunday, lastDay - 1, lastDay))
        Return Result
    End Function


    Public Function GetLastWorkingDayOFMonth(Parameter As String) As String
        Dim Result = ""
        Dim lastBusinessDay As New DateTime()
        Dim arr = Parameter.Split("/")
        'Dim i = DateTime.DaysInMonth(arr(1), arr(2))
        Dim date1 As New Date(arr(2), arr(1), arr(0))
        'Dim date2 = date1.AddMonths(-1)
        Dim lastDay = DateTime.DaysInMonth(date1.Year, date1.Month)
        Dim dayOfWeek__1 = date1.AddDays(lastDay - date1.Day).DayOfWeek
        'consider other holidays --implement from here
        'Result = (If(dayOfWeek__1 = DayOfWeek.Saturday, lastDay - 1, If(dayOfWeek__1 = DayOfWeek.Sunday, lastDay - 2, lastDay)))
        Result = (If(dayOfWeek__1 = DayOfWeek.Sunday, lastDay - 1, lastDay))
        Return Result
    End Function

    Public Function GetLastDayOFMonth(Parameter As String) As String
        Dim Result = ""
        Dim arr = Parameter.Split("/")
        Result = DateTime.DaysInMonth(arr(2), arr(1))
        Return Result
    End Function



    Public Function GETDAY(Parameter As String) As String
        Dim Day As String = ""
        Dim arr = Parameter.Split("/")
        Dim dt As New Date(arr(2), arr(1), arr(0))
        Dim Day1 = Convert.ToInt32(dt.DayOfWeek.ToString("d"))
        ' ''dt.ToString("dd/MM/yy", CultureInfo.InvariantCulture)
        'If dt.DayOfWeek = DayOfWeek.Sunday Then
        '    Day = "Sunday"
        'End If
        Select Case Day1
            Case 1
                Day = "Monday"
            Case 2
                Day = "Tuesday"
            Case 3
                Day = "Wednesday"
            Case 4
                Day = "Thursday"
            Case 5
                Day = "Friday"
            Case 6
                Day = "Saturday"
            Case 0
                Day = "Sunday"
        End Select
        Day = """" & Day & """"
        Return Day.ToUpper
    End Function
    Public Function CONCATE(Parameter As String) As String
        Dim Result = ""
        Result = Parameter.Replace(",", String.Empty)
        Return Result
    End Function
    Public Function SplitFun(Spliter As String, Str As String) As String
        Str = Str.Replace(" ", String.Empty)
        Dim ret As String = ""
        Dim CharAr As Char() = Str
        Dim Start_Pos = 0
        Dim END_Pos = 0
        Dim IsStarterFound = False
        'Checking nested formula 
        If HasNestedFormula(Str) = True Then
            For i As Integer = 0 To CharAr.Count - 1
                'Finding Start position of formula
                If CharAr(i).ToString = "[" Then
                    Start_Pos = i
                    IsStarterFound = True
                End If
                'Finding End position of formula
                If CharAr(i).ToString = "]" Then
                    END_Pos = i
                End If
                'Get innermost Formula!!!! Now take slice of that formula
                If Start_Pos > 0 And END_Pos > 0 Then
                    Dim Formula_StartPOS = 0
                    Dim Own_Bracket_Found = False
                    For k As Integer = Start_Pos - 1 To 0 Step -1
                        'If CharAr(k).ToString = "(" And Own_Bracket_Found = False Then
                        '    Own_Bracket_Found = True
                        '    Continue For
                        'End If
                        'If (CharAr(k).ToString = "(" Or CharAr(k) = ",") And Own_Bracket_Found = True Then
                        If (CharAr(k).ToString = "[" Or CharAr(k) = "," Or CharAr(k) = "(" Or CharAr(k) = ")" Or CharAr(k) = "!" Or CharAr(k) = "=") Then
                            Formula_StartPOS = k + 1
                            Dim StrTest = CharAr(k).ToString
                            Exit For
                        End If
                        'k = k - 1
                    Next
                    'Now getting formula from the string 
                    Dim ForMulaSlice = Str.Substring(Formula_StartPOS, END_Pos - Formula_StartPOS + 1)
                    Dim Result = ExecuteFormula(ForMulaSlice)
                    'Replace the innermostformula with the values
                    Str = Str.Replace(ForMulaSlice, Result)
                    CharAr = Str
                    Start_Pos = 0
                    END_Pos = 0
                    IsStarterFound = False
                    i = 0
                    'checking is there any more nested formula 
                    If (HasNestedFormula(Str) = True) Then
                        Continue For
                    Else
                        Exit For
                    End If
                End If
            Next
        End If
        'Executing Final Formula

        Dim arrList As String() = Str.Split(">", ">=", "<=", "=", "!=", "&&", "||", "+", "-", "*", "%", "<")
        For m As Integer = 0 To arrList.Length - 1
            If ISFormula(arrList(m)) Then
                If arrList(m).ToString.Contains("(") Or arrList(m).ToString.Contains(")") Then
                    arrList(m) = arrList(m).ToString.Replace("(", "").Replace(")", "")
                End If
                ret = ExecuteFormula(arrList(m))
                Str = Str.Replace(arrList(m).Trim, ret.Trim)
            End If
        Next
        'ret = Str
        Str = Str.ToUpper
        Dim res As New CompiledExpression(Str)

        Dim result1 = res.Eval()
        ret = result1.ToString()
        Return ret
    End Function

    Public Function ISFormula(Exp As String) As Boolean
        Dim ret = False
        If Exp.Contains("[") And Exp.Contains("]") Then
            ret = True
        End If
        Return ret
    End Function

    Public Function HasNestedFormula(Exp As String) As Boolean
        Dim ret = False
        Dim arrList As String() = Exp.Split(">", ">=", "<=", "=", "!=", "&&", "||", "+", "-", "*", "%", "<")
        Dim ISBracketFound = 0
        For m As Integer = 0 To arrList.Length - 1
            Dim CharAr As Char() = arrList(m)
            ISBracketFound = 0
            For i As Integer = 0 To CharAr.Count - 1
                If CharAr(i).ToString = "[" Then
                    ISBracketFound = ISBracketFound + 1
                End If
                If ISBracketFound > 1 Then
                    ret = True
                    Exit For
                End If
            Next
        Next
        Return ret
    End Function

    Public Function GenerateQueryForPreLoad(EID As Integer, DocumentType As String, WHCOND As String, OrderBy As String) As DataSet
        Dim ds As New DataSet()
        Dim ds1 As New DataSet()
        'Geiing all the field of Entity becouse all the field might be required
        ds = GetAllFields(EID)
        Dim BaseView As DataView
        Dim BaseTable As DataTable
        If ds.Tables(0).Rows.Count > 0 Then
            BaseView = ds.Tables(0).DefaultView
            BaseView.RowFilter = "DocumentType='" & DocumentType & "'"
            'Getting all the field of base tabel() Note: in case of Action form base table will be name Action form
            BaseTable = BaseView.Table.DefaultView.ToTable()
            Dim StrQuery = GenearateQuery1(EID, DocumentType, ds)
            StrQuery = "SELECT TOP 1 " & StrQuery & " AND " & WHCOND & OrderBy
            StrQuery = "SET DATEFORMAT DMY; " & StrQuery
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Try
                Using con = New SqlConnection(conStr)
                    Using da = New SqlDataAdapter(StrQuery, con)
                        da.Fill(ds1)
                    End Using
                End Using
            Catch ex As Exception
                Throw
            End Try
        End If
        Return ds1
    End Function

    Public Function ExecuteFormula(Exp As String) As String

        Dim Result As String = "10"
        Dim Start_POS As Integer = 0
        Dim END_POS As Integer = 0
        Start_POS = Exp.IndexOf("[")
        END_POS = Exp.IndexOf("]")
        Dim Strparameter = ""
        Strparameter = Exp.Substring(Start_POS + 1, END_POS - Start_POS - 1)
        Dim FunctionName = Exp.Substring(0, Start_POS)
        'Calling the main formulas
        If FunctionName.ToUpper.Trim = "ADD" Then
            Result = ADD(Strparameter)
        ElseIf FunctionName.ToUpper.Trim = "MIN" Then
            Result = Min(Strparameter)
        ElseIf FunctionName.ToUpper.Trim = "MAX" Then
            Result = Max(Strparameter)
        ElseIf FunctionName.ToUpper.Trim = "DAY" Then
            Result = GETDAY(Strparameter)
        ElseIf FunctionName.ToUpper.Trim = "DATE" Then
            Result = GETDATE(Strparameter)
        ElseIf FunctionName.ToUpper.Trim = "MONTH" Then
            Result = Month(Strparameter)
        ElseIf FunctionName.ToUpper.Trim = "YEAR" Then
            Result = Year(Strparameter)
        ElseIf FunctionName.ToUpper.Trim = "CONCATENATE" Then
            Result = CONCATE(Strparameter)
        ElseIf FunctionName.ToUpper.Trim = "DATERANGE" Then
            Result = DATERANGE(Strparameter)
        ElseIf FunctionName.ToUpper.Trim = "GETCURDATE" Then
            Result = GETCURDATE()
        ElseIf FunctionName.ToUpper.Trim = "GETLASTWORKINGDAYOFMONTH" Then
            'update111 GetLastDayOFMonth
            Result = GetLastWorkingDayOFMonth(Strparameter)
        ElseIf FunctionName.ToUpper.Trim = "GETLASTDAYOFMONTH" Then
            Result = GetLastDayOFMonth(Strparameter)
        ElseIf FunctionName.ToUpper.Trim = "MONTHDIFF" Then
            Result = MonthDiff(Strparameter)
        ElseIf FunctionName.ToUpper.Trim = "GETMONTH" Then
            Result = GETMONTH(Strparameter)
        ElseIf FunctionName.ToUpper.Trim = "GETDATE" Then
            Result = GETDATE(Strparameter)
        ElseIf FunctionName.ToUpper.Trim = "UPDATESEQUENCE" Then
            UpdateSequence(Strparameter)
        ElseIf FunctionName.ToUpper.Trim = "UPDATEDESTINATIONS" Then
            UpdateDestinations(Strparameter)
        ElseIf FunctionName.ToUpper.Trim = "NUMERICCOMPARISION" Then
            Result = NumericComparision(Strparameter)
        ElseIf FunctionName.ToUpper = "MUSTBEPOSITIVE" Then
            Result = Mustbepositive(Strparameter)
        ElseIf FunctionName.ToUpper.Trim = "UPDATEETA" Then
            UpdateETA(Strparameter)
            'GETCURDATE()
            'GetLastWorkingDayOFMonth()
            'MonthDiff()
            'GETMONTH
            'GETDATE
        Else
            Throw New Exception("Formula not found")
        End If

        Return Result
    End Function
    Public Function NumericComparision(Parameter As String) As String
        Dim arr = Parameter.Split(New String() {","}, StringSplitOptions.None)
        Dim num1 As Decimal
        Dim num2 As Decimal
        If (String.IsNullOrEmpty(arr(0).Trim())) Then
            num1 = 0.0F
        Else
            num1 = Decimal.Parse(arr(0).Trim())
        End If

        If (String.IsNullOrEmpty(arr(1).Trim())) Then
            num2 = 0.0F
        Else
            num2 = Decimal.Parse(arr(1).Trim())
        End If
        If num1 = num2 Then
            Return "1==1"
        Else
            Return "1==0"
        End If
        Return num1 = num2
    End Function
    Public Function Mustbepositive(Parameter As String) As String
        Dim num1 As Decimal
        If (String.IsNullOrEmpty(Parameter)) Then
            num1 = 0.0F
        Else
            num1 = Int64.Parse(Parameter)
        End If
        If (num1 > 0) Then
            Return "1"
        Else

            Return "0"
        End If
    End Function
    Public Function NumericGreaterComparision(Parameter As String) As Boolean
        Dim arr = Parameter.Split(New String() {","}, StringSplitOptions.RemoveEmptyEntries)
        Dim num1 As Decimal
        Dim num2 As Decimal
        If (String.IsNullOrEmpty(arr(0).Trim())) Then
            num1 = 0.0F
        Else
            num1 = Int64.Parse(arr(0).Trim())
        End If

        If (String.IsNullOrEmpty(arr(1).Trim())) Then
            num2 = 0.0F
        Else
            num2 = Int64.Parse(arr(1).Trim())
        End If
        Return num1 > num2
    End Function

    Public Function Evaluate() As Boolean
        Dim ret As Boolean = False
        Dim res As New CompiledExpression("5>3 && 1<=1 ")
        Dim result = res.Eval()
        Return ret
    End Function
    Public Function GetAllFields(EID As Integer) As DataSet
        Dim ds As New DataSet()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Try
            Dim Query = "SET NOCOUNT ON;select F.FieldID,F.FieldType,F.FieldMapping,F.FieldID,F.DropDown,F.lookupvalue,F.DROPDOWNTYPE,F.DisplayName ,F.DocumentType,FF.FormSource,FF.EventName  FROM MMM_MST_FIELDS F INNER JOIN MMM_MST_FORMS FF ON FF.FormName=F.DocumentType WHERE F.EID= " & EID & " AND FF.EID= " & EID & ";"
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
    Public Function GenearateQuery(FileID As Integer, EID As Integer, DocumentType As String, IsActionForm As Boolean, ActionNature As String) As String
        Dim ret As String = ""
        Try
            Dim ds As New DataSet()
            'Geiing all the field of Entity becouse all the field might be required
            ds = GetAllFields(EID)
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
    Public Function GenearateQuery1(EID As Integer, DocumentType As String, ds As DataSet, Optional tid As Integer = 148289) As String
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
            'Getting all the field of base tabel() Note: in case of Action form base table will be name Action form
            tbl = View.Table.DefaultView.ToTable()
            Dim ViewName = "V" & EID & DocumentType.Replace(" ", "_")
            Dim ddlDocType = ""
            For i As Integer = 0 To tbl.Rows.Count - 1
                If Not (tbl.Rows(i).Item("DROPDOWNTYPE") = "MASTER VALUED" And tbl.Rows(i).Item("FieldType") = "Drop Down") Then
                    StrColumn = StrColumn & "," & ViewName & ".[" & tbl.Rows(i).Item("DisplayName") & "] AS [" & SchemaString & "." & tbl.Rows(i).Item("DisplayName") & "]"
                End If
            Next
            View.RowFilter = "DocumentType='" & DocumentType & "' AND FieldType ='DROP DOWN' AND DropDownType='MASTER VALUED'"
            tblRe = View.Table.DefaultView.ToTable()
            For j As Integer = 0 To tblRe.Rows.Count - 1
                Dim arrddl = tblRe.Rows(j).Item("Dropdown").ToString().Split("-")
                ddlDocType = arrddl(1)
                SchemaString = SchemaString & "." & ddlDocType
                Dim ddlview = "v" & EID & ddlDocType.Trim.Replace(" ", "_")
                Dim joincolumn = "tid"
                Dim DisPlayName = tblRe.Rows(j).Item("DisplayName").ToString().Trim
                If ddlDocType.Trim.ToUpper = "USER" Then
                    joincolumn = "UID"
                End If
                StrJoinString = StrJoinString & "left outer join " & ddlview & " on " & ddlview & "." & joincolumn & " = " & ViewName & ".[" & DisPlayName & "]"
                GenearateQuery2(EID, StrColumn, StrJoinString, SchemaString, ddlDocType, ds)
            Next
            Dim Query = StrColumn.Substring(1, StrColumn.Length - 1) & " FROM " & ViewName & " " & StrJoinString
            ret = Query & " WHERE 1=1 "
            'If tid <> 0 Then
            '    Query = Query & " WHERE " & ViewName & ".tid  = " & tid
            'End If
        End If
        Return ret
    End Function
    Public Function GenearateQuery2(EID As Integer, ByRef StrColumn As String, StrJoinString As String, SchemaString As String, DocumentType As String, ds As DataSet) As String
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
                        Dim str1 = "(SELECT isnull([" & DR(0).Item("DisplayName") & "],'')  from [V" & EID & arrddl(1).Replace(" ", "_") & "] s WHERE CAST(s.tid AS VARCHAR)=CAST(" & ViewName & ".[" & tbl.Rows(i).Item("DisplayName") & "] AS VARCHAR))[" & SchemaString & "." & tbl.Rows(i).Item("DisplayName") & "]"
                        StrColumn = StrColumn & "," & str1
                    End If
                Else
                    StrColumn = StrColumn & "," & ViewName & ".[" & tbl.Rows(i).Item("DisplayName") & "] AS [" & SchemaString & "." & tbl.Rows(i).Item("DisplayName") & "]"
                End If
            Next
        End If
        Return StrColumn
    End Function


    Public Sub UpdateSequence(parms As String)

        Dim arParams = parms.Split(",")
        Dim sequenceFldMapping As String = arParams(0)
        Dim TimeFldMapping As String = arParams(1)
        Dim DocId As Integer = Convert.ToInt32(arParams(2))
        Dim DocType As String = Convert.ToString(arParams(3))
        Dim Eid As Integer = Convert.ToInt32(arParams(4))

        Dim dal As New BpmHelper(Eid)
        Dim Qry = "Select * from mmm_mst_Doc_Item where DocId=" & DocId
        Dim Obj As ReturnObj = dal.EcecDataSet(Qry)
        Dim dsdocs As DataSet = IIf(Obj.Success, Obj.obj, Nothing)
        If dsdocs Is Nothing Then
            Exit Sub
        End If
        Dim dtOld As DataTable = dsdocs.Tables(0)

        'Dim chkdt = dtOld.Select(sequenceFldMapping & "<> '0' or " & sequenceFldMapping & "<> ''")
        'If chkdt.Length > 0 Then
        '    Exit Sub
        'End If

        Dim view As New DataView(dsdocs.Tables(0))
        view.Sort = TimeFldMapping & " ASC"
        Dim dtSorted As DataTable = view.ToTable()

        For i As Integer = 0 To dtOld.Rows.Count - 1
            Dim tid As Integer = Convert.ToInt32(dtOld.Rows(i).Item("Tid"))
            For j As Integer = 0 To dtSorted.Rows.Count - 1
                If tid = Convert.ToInt32(dtSorted.Rows(j).Item("Tid")) Then
                    Qry = "Update mmm_mst_Doc_Item set " & sequenceFldMapping & "='" & j & "' Where Tid=" & tid
                    dal.ExecDML(Qry)
                    Exit For
                End If
            Next
        Next
    End Sub

    Public Sub UpdateDestinations(parms As String)

        Dim arParams = parms.Split(",")
        Dim sequenceFldMapping As String = arParams(0)
        Dim DestiNationFldMapping As String = arParams(1)
        Dim SourceFldMapping As String = arParams(2)
        Dim DocId As Integer = Convert.ToInt32(arParams(3))
        Dim DocType As String = Convert.ToString(arParams(4))
        Dim Eid As Integer = Convert.ToInt32(arParams(5))

        Dim dal As New BpmHelper(Eid)
        Dim Qry = "Select * from mmm_mst_Doc_Item where DocId=" & DocId
        Dim Obj As ReturnObj = dal.EcecDataSet(Qry)
        Dim dsdocs As DataSet = IIf(Obj.Success, Obj.obj, Nothing)
        If dsdocs Is Nothing Then
            Exit Sub
        End If
        Dim dtOld As DataTable = dsdocs.Tables(0)
        Dim view As New DataView(dsdocs.Tables(0))
        view.Sort = sequenceFldMapping & " ASC"
        Dim dtSorted As DataTable = view.ToTable()
        For i As Integer = 0 To dtSorted.Rows.Count - 1
            Dim tid As Integer = Convert.ToInt32(dtSorted.Rows(i).Item("Tid"))

            Dim dr = dtSorted.Select(sequenceFldMapping & "=" & (i + 1))
            If dr.Length > 0 Then
                Qry = "Update mmm_mst_Doc_Item set " & DestiNationFldMapping & "='" & dr(0).Item(SourceFldMapping) & "' Where Tid=" & tid
                dal.ExecDML(Qry)
            End If
            'For j As Integer = 0 To dtSorted.Rows.Count - 1
            '    If tid = Convert.ToInt32(dtSorted.Rows(j).Item("Tid")) Then
            '        Qry = "Update mmm_mst_Doc_Item set " & DestiNationFldMapping & "='" & dtSorted.Rows(j).Item(SourceFldMapping) & "' Where Tid=" & tid
            '        dal.ExecDML(Qry)
            '        Exit For
            '    End If
            'Next
        Next
    End Sub

    Public Sub UpdateETA(parms As String)

        Dim arParams = parms.Split(",")
        Dim sourcefldMapping As String = arParams(0)
        Dim destfldMapping As String = arParams(1)
        Dim ETAfldMapping As String = arParams(2)
        Dim DocId As Integer = Convert.ToInt32(arParams(3))
        Dim DocType As String = Convert.ToString(arParams(4))
        Dim Eid As Integer = Convert.ToInt32(arParams(5))
        Dim sequenceFldMapping As String = Convert.ToString(arParams(6))
        Dim VehTypefldMapping As String = Convert.ToString(arParams(7))

        Dim dal As New BpmHelper(Eid)
        Dim Qry = "Select * from mmm_mst_Doc_Item where DocId=" & DocId & "order by " & sequenceFldMapping
        Dim Obj As ReturnObj = dal.EcecDataSet(Qry)
        Dim dsdocs As DataSet = IIf(Obj.Success, Obj.obj, Nothing)
        If dsdocs Is Nothing Then
            Exit Sub
        End If
        Dim dtOld As DataTable = dsdocs.Tables(0)
        dtOld.Columns.Add("Latt")
        dtOld.Columns.Add("Longt")
        Dim gisObj As New GisMethods()
        For i As Integer = 0 To dtOld.Rows.Count - 1
            Dim ob = gisObj.GoogleGeoCodeFreeText(dtOld.Rows(i).Item(sourcefldMapping).ToString())
            If ob.Success Then
                dtOld.Rows(i).Item("Latt") = ob.Latt
                dtOld.Rows(i).Item("Longt") = ob.Longt
            End If
        Next
        Dim sources As New StringBuilder()
        Dim Destinations As New StringBuilder()
        For i As Integer = 0 To dtOld.Rows.Count - 1
            sources.Append("&start" & i & "=" & dtOld.Rows(i).Item("Latt") & "," & dtOld.Rows(i).Item("Longt"))
            Destinations.Append("&destination" & i & "=" & dtOld.Rows(i).Item("Latt") & "," & dtOld.Rows(i).Item("Longt"))
        Next

        Qry = "Select * from mmm_mst_doc where Tid=" & DocId & "order by " & sequenceFldMapping
        Obj = dal.EcecDataSet(Qry)
        Dim dt = Obj.obj.Tables(0)
        Dim DistMatrix As List(Of DistanceMatrix) = GisMethods.GetHereDistanceTimeMatrix(sources.ToString, Destinations.ToString, dt.Rows(0).Item(VehTypefldMapping).ToString.ToLower())

        For i As Integer = 0 To dtOld.Rows.Count - 1
            Dim sourceIndex As Integer = i
            Dim destIndex As Integer
            Dim destiNation As String = dtOld.Rows(i).Item(destfldMapping)
            If destiNation.Trim() = "" Then
                Continue For
            End If
            Dim dr = dtOld.Select(sourcefldMapping & "='" & destiNation & "'")
            If dr.Length > 0 Then
                destIndex = dr(0).Item(sequenceFldMapping)
                'For j As Integer = 0 To dtOld.Rows.Count - 1
                '    If dtOld.Rows(i).Item(sourcefldMapping) = dtOld.Rows(j).Item(destfldMapping) Then
                '        destIndex = dtOld.Rows(j).Item(sequenceFldMapping)
                '        Exit For
                '    End If
                'Next
                Dim d = DistMatrix.FindAll(Function(p As DistanceMatrix) p.StartIndex = sourceIndex And p.DestinationIndex = destIndex)
                Qry = "Update mmm_mst_Doc_Item set " & ETAfldMapping & "='" & d.Item(0).BaseTime.ToString() & "' where Tid=" & dtOld.Rows(i).Item("Tid")
                dal.ExecDML(Qry)
            End If
        Next

    End Sub

    '' commendted due to error
    Public Function ExecuteControllevelRule(ctrlID As String, pnlP As Panel, pnlC As Panel, screenname As String, dtparent As DataTable, dtChild As DataTable, ErrorLbl As Label, Optional IsChildForm As Boolean = False, Optional page As Page = Nothing)
        Dim ObjRet As New RuleResponse()
        Dim objD As New DynamicForm()
        Dim ActionPanel As Panel = pnlP
        Dim lstData As New List(Of UserData)
        Dim lstDataC As List(Of UserData) = Nothing
        'in case of child item create collection of both panel
        If IsChildForm Then
            ActionPanel = pnlC
            lstDataC = New List(Of UserData)
            lstDataC = objD.CreateCollection(pnlP, dtChild)
        End If
        'Getting 
        Dim dt As DataTable = dtparent
        lstData = objD.CreateCollection(pnlP, dt)
        Dim ctrlvalue As String = ""
        For Each obj In lstData
            If obj.FieldID = ctrlID Then
                ctrlvalue = obj.Values
            End If
        Next
        Dim ObjRule As New RuleEngin(HttpContext.Current.Session("EID"), screenname, "CREATED", "CONTROL", ctrlID.ToString, ctrlvalue)
        Dim dsrule As DataSet = ObjRule.GetRules()
        Dim dtrule As New DataTable
        dtrule = dsrule.Tables(1)
        For Each dr As DataRow In dsrule.Tables(0).Rows
            ObjRet = ObjRule.ExecuteRule(lstData, lstDataC, IsChildForm, "", dr, dtrule)
            If Not ObjRet.Success Then
                If Convert.ToString(ObjRet.FailActionType).ToUpper = "ALERT" Then
                    ShowAlert(page, ObjRet.ErrorMessage)
                    'ScriptManager.RegisterStartupScript(Me, Me.GetType(), "popup", "alert('" + ObjRet.ErrorMessage + "');", True)
                    Exit Function
                End If
            End If
            If Not String.IsNullOrEmpty(ObjRet.TargetField.Trim) And ObjRet.HasRule = True Then
                'displaying rule message
                'changes by Himank on 11september 2015
                If Not ObjRet.Success Then
                    ErrorLbl.Text = ObjRet.ErrorMessage
                    ErrorLbl.ForeColor = System.Drawing.Color.Red
                Else
                    ErrorLbl.Text = ObjRet.SucessMessage
                    ErrorLbl.ForeColor = System.Drawing.Color.Black
                End If

                'Spliting it with "," because there can be more than one target controls
                Dim arr = ObjRet.TargetField.Split(",")

                Dim trgCtrl = Nothing
                Dim lbl As New Label()
                'Dim ActionType As String = ObjRet.ActionType.Trim.ToUpper
                For i As Integer = 0 To arr.Length - 1
                    trgCtrl = Nothing
                    If (arr(i).Trim <> "") Then
                        Dim ctrl = ActionPanel.FindControl("fld" & arr(i))
                        Dim ctrlHdn = ActionPanel.FindControl("HDN" & arr(i))
                        'Add by manvendra 03-12-2019
                        Dim imgbtn As HtmlGenericControl = CType(ActionPanel.FindControl("imgresset_" & arr(i)), HtmlGenericControl)
                        Dim myImg As System.Web.UI.WebControls.Image = CType(ActionPanel.FindControl("img" & arr(i)), System.Web.UI.WebControls.Image)
                        Dim myDiv As HtmlGenericControl = CType(ActionPanel.FindControl("div" & arr(i)), HtmlGenericControl)
                        Dim mySpan As HtmlGenericControl = CType(ActionPanel.FindControl("span" & arr(i)), HtmlGenericControl)
                        Try
                            If ctrl Is Nothing Then
                                ctrl = ActionPanel.FindControl("GRD" & arr(i))
                            End If
                        Catch ex As Exception

                        End Try
                        Dim cssClassInv = "INVISIBLE"
                        Dim CssClass = "txtBox"
                        If Not ctrl Is Nothing Then
                            lbl = ActionPanel.FindControl("lbl" & arr(i))
                            If ctrl.GetType() Is GetType(System.Web.UI.WebControls.DropDownList) Then
                                trgCtrl = CType(ctrl, DropDownList)

                            ElseIf ctrl.GetType() Is GetType(System.Web.UI.WebControls.TextBox) Then
                                trgCtrl = CType(ctrl, TextBox)
                            ElseIf ctrl.GetType() Is GetType(System.Web.UI.WebControls.FileUpload) Then
                                trgCtrl = CType(ctrl, FileUpload)
                            ElseIf ctrl.GetType() Is GetType(System.Web.UI.WebControls.CheckBoxList) Then
                                trgCtrl = CType(ctrl, CheckBoxList)
                            ElseIf ctrl.GetType() Is GetType(System.Web.UI.WebControls.GridView) Then
                                trgCtrl = CType(ctrl, GridView)
                                CssClass = "mGrid"
                            Else
                                trgCtrl = CType(ctrl, CheckBox)
                            End If
                            'changing control properties according to rule configuration
                            'changes by Himank on 11september 2015
                            If ObjRet.Success Then
                                Select Case ObjRet.SuccActionType.ToUpper
                                    Case "CHANGE DROPDOWN COLOR"
                                        trgCtrl.CssClass = CssClass
                                        If Not (lbl Is Nothing) Then
                                            lbl.CssClass = ""
                                        End If
                                    Case "DISABLE"
                                        trgCtrl.Enabled = False
                                    Case "INVISIBLE"
                                        trgCtrl.CssClass = "invisible"
                                        trgCtrl = Nothing
                                        If Not (lbl Is Nothing) Then
                                            lbl.CssClass = "invisible"
                                        End If
                                        'Add by manvendra 04-12-2019
                                        If Not IsNothing(imgbtn) Then
                                            If imgbtn.GetType() Is GetType(HtmlGenericControl) Then
                                                imgbtn.Visible = False
                                            End If
                                        End If

                                        If Not IsNothing(myImg) Then
                                            If myImg.GetType() Is GetType(System.Web.UI.WebControls.Image) Then
                                                myImg.CssClass = "invisible"
                                            End If
                                        End If
                                        If Not IsNothing(myDiv) Then
                                            If myDiv.GetType() Is GetType(HtmlGenericControl) Then
                                                myDiv.Visible = False
                                            End If
                                        End If
                                        If Not IsNothing(mySpan) Then
                                            If mySpan.GetType() Is GetType(HtmlGenericControl) Then
                                                mySpan.Visible = False
                                            End If
                                        End If

                                    Case "ENABLE"
                                        trgCtrl.Enabled = True
                                    Case "VISIBLE"
                                        trgCtrl.CssClass = CssClass
                                        If Not (lbl Is Nothing) Then
                                            lbl.CssClass = ""
                                        End If
                                        If Not IsNothing(myImg) Then
                                            If myImg.GetType() Is GetType(System.Web.UI.WebControls.Image) Then
                                                myImg.CssClass = ""
                                            End If
                                        End If
                                        'Add by manvendra 04-12-2019
                                        If Not IsNothing(imgbtn) Then
                                            If imgbtn.GetType() Is GetType(HtmlGenericControl) Then
                                                imgbtn.Visible = True
                                            End If
                                        End If


                                        If Not IsNothing(myDiv) Then
                                            If myDiv.GetType() Is GetType(HtmlGenericControl) Then
                                                myDiv.Visible = True
                                            End If
                                        End If
                                        If Not IsNothing(mySpan) Then
                                            If mySpan.GetType() Is GetType(HtmlGenericControl) Then
                                                mySpan.Visible = True
                                            End If
                                        End If
                                    Case "NO ACTION"
                                End Select
                            Else
                                Select Case ObjRet.FailActionType.ToUpper
                                    Case "CHANGE DROPDOWN COLOR"
                                        trgCtrl.CssClass = "Heiglight"
                                        If Not (lbl Is Nothing) Then
                                            lbl.CssClass = "Heiglight"
                                        End If

                                    Case "DISABLE"
                                        trgCtrl.Enabled = False
                                    Case "ENABLE"
                                        trgCtrl.Enabled = True
                                    Case "INVISIBLE"
                                        trgCtrl.CssClass = "invisible"

                                        If Not (lbl Is Nothing) Then
                                            lbl.CssClass = "invisible"
                                        End If
                                        trgCtrl = Nothing
                                        If Not IsNothing(myImg) Then
                                            If myImg.GetType() Is GetType(System.Web.UI.WebControls.Image) Then
                                                myImg.CssClass = "invisible"
                                            End If
                                        End If
                                        'Add by manvendra 04-12-2019
                                        If Not IsNothing(imgbtn) Then
                                            If imgbtn.GetType() Is GetType(HtmlGenericControl) Then
                                                imgbtn.Visible = False
                                            End If
                                        End If
                                        If Not IsNothing(myDiv) Then
                                            If myDiv.GetType() Is GetType(HtmlGenericControl) Then
                                                myDiv.Visible = False
                                            End If
                                        End If
                                        If Not IsNothing(mySpan) Then
                                            If mySpan.GetType() Is GetType(HtmlGenericControl) Then
                                                mySpan.Visible = False
                                            End If
                                        End If
                                    Case "VISIBLE"
                                        trgCtrl.CssClass = CssClass
                                        If Not (lbl Is Nothing) Then
                                            lbl.CssClass = ""
                                        End If

                                        If Not IsNothing(myImg) Then
                                            If myImg.GetType() Is GetType(System.Web.UI.WebControls.Image) Then
                                                myImg.CssClass = ""
                                            End If
                                        End If
                                        'Add by manvendra 04-12-2019
                                        If Not IsNothing(imgbtn) Then
                                            If imgbtn.GetType() Is GetType(HtmlGenericControl) Then
                                                imgbtn.Visible = True
                                            End If
                                        End If
                                        If Not IsNothing(myDiv) Then
                                            If myDiv.GetType() Is GetType(HtmlGenericControl) Then
                                                myDiv.Visible = True
                                            End If
                                        End If
                                        If Not IsNothing(mySpan) Then
                                            If mySpan.GetType() Is GetType(HtmlGenericControl) Then
                                                mySpan.Visible = True
                                            End If
                                        End If
                                End Select
                            End If
                        End If
                    End If
                Next
                'Exit Sub
            End If
        Next
    End Function

    ' bkup
    'Public Function ExecuteControllevelRule(ctrlID As String, pnlP As Panel, pnlC As Panel, screenname As String, dtparent As DataTable, dtChild As DataTable, ErrorLbl As Label, Optional IsChildForm As Boolean = False, Optional page As Page = Nothing)
    '    Dim ObjRet As New RuleResponse()
    '    Dim objD As New DynamicForm()
    '    Dim ActionPanel As Panel = pnlP
    '    Dim lstData As New List(Of UserData)
    '    Dim lstDataC As List(Of UserData) = Nothing
    '    'in case of child item create collection of both panel
    '    If IsChildForm Then
    '        ActionPanel = pnlC
    '        lstDataC = New List(Of UserData)
    '        lstDataC = objD.CreateCollection(pnlP, dtChild)
    '    End If
    '    'Getting 
    '    Dim dt As DataTable = dtparent
    '    lstData = objD.CreateCollection(pnlP, dt)
    '    Dim ctrlvalue As String = ""
    '    For Each obj In lstData
    '        If obj.FieldID = ctrlID Then
    '            ctrlvalue = obj.Values
    '        End If
    '    Next
    '    Dim ObjRule As New RuleEngin(HttpContext.Current.Session("EID"), screenname, "CREATED", "CONTROL", ctrlID.ToString, ctrlvalue)
    '    Dim dsrule As DataSet = ObjRule.GetRules()
    '    Dim dtrule As New DataTable
    '    dtrule = dsrule.Tables(1)
    '    For Each dr As DataRow In dsrule.Tables(0).Rows
    '        ObjRet = ObjRule.ExecuteRule(lstData, lstDataC, IsChildForm, "", dr, dtrule)
    '        If Not ObjRet.Success Then
    '            If Convert.ToString(ObjRet.FailActionType).ToUpper = "ALERT" Then
    '                ShowAlert(page, ObjRet.ErrorMessage)
    '                'ScriptManager.RegisterStartupScript(Me, Me.GetType(), "popup", "alert('" + ObjRet.ErrorMessage + "');", True)
    '                Exit Function
    '            End If
    '        End If
    '        If Not String.IsNullOrEmpty(ObjRet.TargetField.Trim) And ObjRet.HasRule = True Then
    '            'displaying rule message
    '            'changes by Himank on 11september 2015
    '            If Not ObjRet.Success Then
    '                ErrorLbl.Text = ObjRet.ErrorMessage
    '                ErrorLbl.ForeColor = System.Drawing.Color.Red
    '            Else
    '                ErrorLbl.Text = ObjRet.SucessMessage
    '                ErrorLbl.ForeColor = System.Drawing.Color.Black
    '            End If

    '            'Spliting it with "," because there can be more than one target controls
    '            Dim arr = ObjRet.TargetField.Split(",")

    '            Dim trgCtrl = Nothing
    '            Dim lbl As New Label()
    '            'Dim ActionType As String = ObjRet.ActionType.Trim.ToUpper
    '            For i As Integer = 0 To arr.Length - 1
    '                trgCtrl = Nothing
    '                If (arr(i).Trim <> "") Then
    '                    Dim ctrl = ActionPanel.FindControl("fld" & arr(i))
    '                    Dim ctrlHdn = ActionPanel.FindControl("HDN" & arr(i))
    '                    Dim myImg As System.Web.UI.WebControls.Image = CType(ActionPanel.FindControl("img" & arr(i)), System.Web.UI.WebControls.Image)
    '                    Dim myDiv As HtmlGenericControl = CType(ActionPanel.FindControl("div" & arr(i)), HtmlGenericControl)
    '                    If Not ctrl Is Nothing Then
    '                        lbl = ActionPanel.FindControl("lbl" & arr(i))
    '                        If ctrl.GetType() Is GetType(System.Web.UI.WebControls.DropDownList) Then
    '                            trgCtrl = CType(ctrl, DropDownList)
    '                        ElseIf ctrl.GetType() Is GetType(System.Web.UI.WebControls.TextBox) Then
    '                            trgCtrl = CType(ctrl, TextBox)
    '                        ElseIf ctrl.GetType() Is GetType(System.Web.UI.WebControls.FileUpload) Then
    '                            trgCtrl = CType(ctrl, FileUpload)
    '                        ElseIf ctrl.GetType() Is GetType(System.Web.UI.WebControls.CheckBoxList) Then
    '                            trgCtrl = CType(ctrl, CheckBoxList)
    '                        Else
    '                            trgCtrl = CType(ctrl, CheckBox)
    '                        End If
    '                        'changing control properties according to rule configuration
    '                        'changes by Himank on 11september 2015
    '                        If ObjRet.Success Then
    '                            Select Case ObjRet.SuccActionType.ToUpper
    '                                Case "CHANGE DROPDOWN COLOR"
    '                                    trgCtrl.CssClass = "txtBox"
    '                                    lbl.CssClass = ""
    '                                Case "DISABLE"
    '                                    trgCtrl.Enabled = False
    '                                Case "INVISIBLE"
    '                                    trgCtrl.CssClass = "invisible"
    '                                    trgCtrl = Nothing
    '                                    lbl.CssClass = "invisible"
    '                                    If Not IsNothing(myImg) Then
    '                                        If myImg.GetType() Is GetType(System.Web.UI.WebControls.Image) Then
    '                                            myImg.CssClass = "invisible"
    '                                        End If
    '                                    End If
    '                                    If Not IsNothing(myDiv) Then
    '                                        If myDiv.GetType() Is GetType(HtmlGenericControl) Then
    '                                            myDiv.Visible = False
    '                                        End If
    '                                    End If
    '                                Case "ENABLE"
    '                                    trgCtrl.Enabled = True
    '                                Case "VISIBLE"
    '                                    trgCtrl.CssClass = "txtBox"
    '                                    lbl.CssClass = ""
    '                                    If Not IsNothing(myImg) Then
    '                                        If myImg.GetType() Is GetType(System.Web.UI.WebControls.Image) Then
    '                                            myImg.CssClass = ""
    '                                        End If
    '                                    End If
    '                                    If Not IsNothing(myDiv) Then
    '                                        If myDiv.GetType() Is GetType(HtmlGenericControl) Then
    '                                            myDiv.Visible = True
    '                                        End If
    '                                    End If
    '                                Case "NO ACTION"

    '                            End Select
    '                        Else
    '                            Select Case ObjRet.FailActionType.ToUpper
    '                                Case "CHANGE DROPDOWN COLOR"
    '                                    trgCtrl.CssClass = "Heiglight"
    '                                    lbl.CssClass = "Heiglight"
    '                                Case "DISABLE"
    '                                    trgCtrl.Enabled = False
    '                                Case "ENABLE"
    '                                    trgCtrl.Enabled = True
    '                                Case "INVISIBLE"
    '                                    trgCtrl.CssClass = "invisible"
    '                                    trgCtrl = Nothing
    '                                    lbl.CssClass = "invisible"
    '                                    If Not IsNothing(myImg) Then
    '                                        If myImg.GetType() Is GetType(System.Web.UI.WebControls.Image) Then
    '                                            myImg.CssClass = "invisible"
    '                                        End If
    '                                    End If
    '                                    If Not IsNothing(myDiv) Then
    '                                        If myDiv.GetType() Is GetType(HtmlGenericControl) Then
    '                                            myDiv.Visible = False
    '                                        End If
    '                                    End If
    '                                Case "VISIBLE"
    '                                    trgCtrl.CssClass = "txtBox"
    '                                    lbl.CssClass = ""
    '                                    If Not IsNothing(myImg) Then
    '                                        If myImg.GetType() Is GetType(System.Web.UI.WebControls.Image) Then
    '                                            myImg.CssClass = ""
    '                                        End If
    '                                    End If
    '                                    If Not IsNothing(myDiv) Then
    '                                        If myDiv.GetType() Is GetType(HtmlGenericControl) Then
    '                                            myDiv.Visible = True
    '                                        End If
    '                                    End If
    '                            End Select
    '                        End If
    '                    End If
    '                End If
    '            Next
    '            'Exit Sub
    '        End If
    '    Next
    'End Function

    'Public Sub ShowAlert(currentPage As Page, errorMsg As String)
    '    currentPage.ClientScript.RegisterStartupScript(currentPage.GetType(), "key", "alert('" + errorMsg + "')", True)
    'End Sub

    Public Sub ShowAlert(currentPage As Page, errorMsg As String)
        ScriptManager.RegisterStartupScript(currentPage, currentPage.GetType(), "key", "alert('" + errorMsg + "')", True)
        'currentPage.ClientScript.RegisterStartupScript(currentPage.GetType(), "key", "alert('" + errorMsg + "')", True)
    End Sub


    Public Function ExecuteCustomFunction(EID As Integer, DOCID As Integer, DocumentType As String, Formula As String) As RuleResponse
        Dim ObjRet As New RuleResponse()
        Try
            Dim ds As New DataSet()
            Dim objR As New RuleEngin()
            If (Formula.Contains("{DOCID}")) Then
                Formula = Formula.Replace("{DOCID}", DOCID)
            End If
            'Dim Result = objR.SplitFun("", Formula)
            ExecuteFormula(Formula)
        Catch ex As Exception
            ObjRet.Success = False
            ObjRet.Message = ex.Message
        End Try
        Return ObjRet

    End Function

    Public Function CheckDuplicate(Parameter As String, DocumentType As String, EID As Integer, lstParentField As List(Of UserData)) As String
        Dim Result As String = "FALSE"
        Dim ColumnArray As New ArrayList()
        Dim DelimetedValue As String() = Parameter.Split(",")
        For i As Integer = 0 To DelimetedValue.Length - 1
            For Each obj In lstParentField
                If obj.FieldID.Trim = DelimetedValue(i) Then
                    ColumnArray.Add(obj.FieldMapping & "='" & obj.Values & "'")
                End If
            Next
        Next
        Dim conStrs As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim cons As New SqlConnection(conStrs)
        Try
            Dim das As SqlDataAdapter = New SqlDataAdapter("", cons)
            das.SelectCommand.CommandText = "select Count(*) from mmm_mst_doc where DocumentType='" & DocumentType & "' and eid=" & EID & " and " & String.Join(" and ", ColumnArray.ToArray()) & "   and curstatus<>'REJECTED'"
            If cons.State = ConnectionState.Closed Then
                cons.Open()
            End If
            If das.SelectCommand.ExecuteScalar() = 0 Then
                Result = "TRUE"
            End If
        Catch ex As Exception
            Throw
        Finally
            cons.Dispose()
        End Try
        Return Result
    End Function

    Public Function CheckDuplicateFromMaster(Parameter As String, DocumentType As String, EID As Integer, lstParentField As List(Of UserData), MDocumentType As String) As String
        Dim Result As String = "FALSE"
        Dim ColumnArray As New ArrayList()
        Dim DelimetedValue As String() = Parameter.Split(",")
        For i As Integer = 0 To DelimetedValue.Length - 1
            For Each obj In lstParentField
                Dim DelimetedValue1 As String() = DelimetedValue(i).Split("-")
                If obj.FieldID.Trim = DelimetedValue1(0) Then
                    ColumnArray.Add(DelimetedValue1(1) & "='" & obj.Values & "'")
                End If
            Next
        Next
        Dim conStrs As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim cons As New SqlConnection(conStrs)
        Dim das As SqlDataAdapter = New SqlDataAdapter("", cons)
        Try
            das.SelectCommand.CommandText = "select Count(*) from mmm_mst_master where DocumentType='" & MDocumentType & "' and eid=" & EID & " and " & String.Join(" and ", ColumnArray.ToArray()) & "  "
            If cons.State = ConnectionState.Closed Then
                cons.Open()
            End If
            If das.SelectCommand.ExecuteScalar() = 0 Then
                Result = "TRUE"
            End If
        Catch ex As Exception
            Throw
        Finally
            cons.Dispose()
        End Try
        Return Result
    End Function

    '' befor adding code for excluding rejected documents
    'Public Function CheckDuplicate(Parameter As String, DocumentType As String, EID As Integer, lstParentField As List(Of UserData)) As String
    '    Dim Result As String = "FALSE"
    '    Dim ColumnArray As New ArrayList()
    '    Dim DelimetedValue As String() = Parameter.Split(",")
    '    For i As Integer = 0 To DelimetedValue.Length - 1
    '        For Each obj In lstParentField
    '            If obj.FieldID.Trim = DelimetedValue(i) Then
    '                ColumnArray.Add(obj.FieldMapping & "='" & obj.Values & "'")
    '            End If
    '        Next
    '    Next
    '    Dim conStrs As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
    '    Dim cons As New SqlConnection(conStrs)
    '    Dim das As SqlDataAdapter = New SqlDataAdapter("", cons)
    '    das.SelectCommand.CommandText = "select Count(*) from mmm_mst_doc where DocumentType='" & DocumentType & "' and eid=" & EID & " and " & String.Join(" and ", ColumnArray.ToArray())
    '    If cons.State = ConnectionState.Closed Then
    '        cons.Open()
    '    End If
    '    If das.SelectCommand.ExecuteScalar() = 0 Then
    '        Result = "TRUE"
    '    End If
    '    cons.Dispose()
    '    Return Result
    'End Function

End Class

Public Class RuleResponse
    Public Property Success As Boolean
    Public Property Message As String
    Public Property SucessMessage As String
    Public Property ErrorMessage As String
    Public Property ControlField As String
    Public Property TargetField As String
    Public Property SuccActionType As String
    Public Property FailActionType As String
    Public Property ActionType As String
    Public Property HasRule As Boolean

End Class


