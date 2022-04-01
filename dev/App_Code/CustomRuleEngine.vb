Imports Microsoft.VisualBasic
Imports System.Data

Public Class CustomRuleEngine
    Dim _EID As Integer
    Dim _Documenttype As String
    Dim objDC As New DataClass()
    Public Sub New(EID As Integer, DocumentType As String)
        _EID = EID
        _Documenttype = DocumentType
    End Sub

    Public Function InvisibleDocIDForDocDetail(ByRef docID As Integer) As List(Of RuleResponse)
        Dim resultRuleResponse As New List(Of RuleResponse)
        'Rule for hide & show in doc detail
        Dim ObjDT As New DataTable()
        ObjDT = objDC.ExecuteQryDT("select * from MMM_MST_RULES where eid=" & _EID & " and whentoRun='Control' and documenttype='" & _Documenttype & "' and isactive=1 and executedocdetail=1 and TargetSpecificControl is not null and (SuccActiontype ='INVISIBLE' or SuccActiontype ='VISIBLE') and (FailActiontype='VISIBLE' or FailActiontype='INVISIBLE') order by executedocdetailorder")
        Dim ObjAllData As New DataTable()
        ObjAllData = objDC.ExecuteQryDT("SELECT FF.*,F.*  FROM MMM_MST_FIELDS FF With(nolock)  left outer JOIN MMM_MST_FORMS F   With(nolock) on F.FormName = FF.DocumentType and F.EID = FF.EID   where (FF.isactive=1) and F.EID=" & _EID & " and FormName in(select documenttype from mmm_mst_doc where tid=" & docID & " )  order by DocDtlDisplayOrder")
        Dim lstData As New List(Of UserData)
        Dim objD As New DynamicForm()
        lstData = objD.CreateCollectionForDocDetail(ObjAllData, docID)
        resultRuleResponse = ReturnRuleResponse(ObjDT:=ObjDT, ObjAllData:=ObjAllData, lstData:=lstData, docID:=docID, TagetSpecficControl:=True)
      
        'For Each dr As DataRow In ObjDT.Rows
        '    Dim conditionalField As String = objDC.ExecuteQryScaller("select fieldmapping from mmm_mst_fields where fieldid=" & dr("ControlField"))
        '    Dim resultField As String = objDC.ExecuteQryScaller("select " & conditionalField & " from mmm_mst_doc where tid= " & docID)
        '    If resultField = dr("TargetSpecificControl") Then
        '        Dim ObjRet As New RuleResponse()
        '        'Dim objD As New DynamicForm()
        '        'Dim lstData As New List(Of UserData)
        '        lstData = objD.CreateCollectionForDocDetail(ObjAllData, docID)
        '        Dim Expression = ""
        '        Expression = dr("Condition").ToString.ToUpper
        '        For Each obj In lstData
        '            Dim TestStr = "{FORM." & obj.DocumentType.Trim.ToUpper & "." & obj.DisplayName.ToUpper.Trim & "}"
        '            If Expression.Contains(TestStr) Then
        '                'If obj.FieldType.ToUpper = "CHILD ITEM TOTAL" Then
        '                '    If obj.Values = "" Then
        '                '        obj.Values = "0"
        '                '    End If
        '                'End If
        '                Expression = Expression.Replace(TestStr, obj.Values)
        '            End If
        '        Next
        '        'Coading for Session Valued data
        '        If Expression.ToUpper.Contains("[SESSION(") Then
        '            Dim stringSeparators() As String = New String() {"[SESSION("}
        '            Dim results() As String
        '            ' ...
        '            results = Expression.ToUpper.Split(stringSeparators, StringSplitOptions.None)
        '            For i As Integer = 0 To results.Length - 1
        '                If results(i).ToString().Contains(")]") Then
        '                    Dim value As String = HttpContext.Current.Session("" & results(i).Substring(1, results(i).IndexOf("]") - 3) & "")
        '                    Expression = Expression.ToUpper.Replace("[SESSION(""" & results(i).Substring(1, results(i).IndexOf("]") - 3) & """)]", value)
        '                End If
        '            Next
        '        End If
        '        Dim objRuleEngine As New RuleEngin()
        '        Dim ResponseResult = objRuleEngine.SplitFun("", Expression)
        '        ObjRet.HasRule = True
        '        If ResponseResult.ToString.ToUpper = "TRUE" Then
        '            ObjRet.Success = True
        '            ObjRet.Message = ""
        '            ObjRet.ControlField = dr("ControlField")
        '            ObjRet.TargetField = dr("TargetControlField")
        '            ObjRet.SuccActionType = dr("SuccActiontype")
        '            ObjRet.SucessMessage = Convert.ToString(dr("SuccMsg"))
        '        Else
        '            ObjRet.Success = False
        '            ObjRet.ErrorMessage = dr("ErrorMsg")
        '            ObjRet.ControlField = dr("ControlField")
        '            ObjRet.TargetField = dr("TargetControlField")
        '            ObjRet.FailActionType = dr("FailActiontype")
        '            ObjRet.SucessMessage = Convert.ToString(dr("SuccMsg"))
        '        End If
        '    End If
        'Next
        'Now check for exclude targetspecfic control
        ObjDT = objDC.ExecuteQryDT("select * from MMM_MST_RULES where eid=" & _EID & " and whentoRun='Control' and documenttype='" & _Documenttype & "' and isactive=1 and executedocdetail=1 and TargetSpecificControl is  null and (SuccActiontype ='INVISIBLE' or SuccActiontype ='VISIBLE') and (FailActiontype='VISIBLE' or FailActiontype='INVISIBLE') order by executedocdetailorder")
        resultRuleResponse.AddRange(ReturnRuleResponse(ObjDT:=ObjDT, ObjAllData:=ObjAllData, lstData:=lstData, docID:=docID, TagetSpecficControl:=False))
        Return resultRuleResponse
    End Function

    Public Function ReturnRuleResponse(ByRef ObjDT As DataTable, ByRef ObjAllData As DataTable, ByRef lstData As List(Of UserData), ByRef docID As Integer, Optional ByRef TagetSpecficControl As Boolean = False) As List(Of RuleResponse)
        Dim objRuleResponse As New List(Of RuleResponse)
        For Each dr As DataRow In ObjDT.Rows
            Dim ObjRet As New RuleResponse()
            'Add dataview instead of run the query
            Dim objDataView As New DataView()
            objDataView = ObjAllData.DefaultView
            objDataView.RowFilter = "fieldid=" & dr("ControlField")
            ' prev with error , below line was not required at all - by sunil P, 23Dec21
            ' Dim conditionalField As String = objDC.ExecuteQryScaller("select " & objDataView.ToTable().Rows(0)("fieldmapping") & " from mmm_mst_fields with(nolock) where fieldid=" & dr("ControlField"))
            Dim conditionalField As String = objDataView.ToTable().Rows(0)("fieldmapping")
            Dim resultField As String = objDC.ExecuteQryScaller("select " & conditionalField & " from mmm_mst_doc with(nolock) where tid= " & docID)
            If TagetSpecficControl Then
                If resultField = dr("TargetSpecificControl") Then
                    Dim Expression = ""
                    Expression = dr("Condition").ToString.ToUpper
                    For Each obj In lstData
                        Dim TestStr = "{FORM." & obj.DocumentType.Trim.ToUpper & "." & obj.DisplayName.ToUpper.Trim & "}"
                        If Expression.Contains(TestStr) Then
                            Expression = Expression.Replace(TestStr, obj.Values)
                        End If
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
                    End If
                    Dim objRuleEngine As New RuleEngin()
                    Dim ResponseResult = objRuleEngine.SplitFun("", Expression)
                    ObjRet.HasRule = True
                    If ResponseResult.ToString.ToUpper = "TRUE" Then
                        ObjRet.Success = True
                        ObjRet.Message = ""
                        ObjRet.ControlField = dr("ControlField")
                        ObjRet.TargetField = dr("TargetControlField")
                        ObjRet.SuccActionType = dr("SuccActiontype")
                        ObjRet.SucessMessage = Convert.ToString(dr("SuccMsg"))
                    Else
                        ObjRet.Success = False
                        ObjRet.ErrorMessage = dr("ErrorMsg")
                        ObjRet.ControlField = dr("ControlField")
                        ObjRet.TargetField = dr("TargetControlField")
                        ObjRet.FailActionType = dr("FailActiontype")
                        ObjRet.SucessMessage = Convert.ToString(dr("SuccMsg"))
                    End If
                End If
            Else
                Dim Expression = ""
                Expression = dr("Condition").ToString.ToUpper
                For Each obj In lstData
                    Dim TestStr = "{FORM." & obj.DocumentType.Trim.ToUpper & "." & obj.DisplayName.ToUpper.Trim & "}"
                    If Expression.Contains(TestStr) Then
                        Expression = Expression.Replace(TestStr, obj.Values)
                    End If
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
                End If
                Dim objRuleEngine As New RuleEngin()
                Dim ResponseResult = objRuleEngine.SplitFun("", Expression)
                ObjRet.HasRule = True
                If ResponseResult.ToString.ToUpper = "TRUE" Then
                    ObjRet.Success = True
                    ObjRet.Message = ""
                    ObjRet.ControlField = dr("ControlField")
                    ObjRet.TargetField = dr("TargetControlField")
                    ObjRet.SuccActionType = dr("SuccActiontype")
                    ObjRet.SucessMessage = Convert.ToString(dr("SuccMsg"))
                Else
                    ObjRet.Success = False
                    ObjRet.ErrorMessage = dr("ErrorMsg")
                    ObjRet.ControlField = dr("ControlField")
                    ObjRet.TargetField = dr("TargetControlField")
                    ObjRet.FailActionType = dr("FailActiontype")
                    ObjRet.SucessMessage = Convert.ToString(dr("SuccMsg"))
                End If
            End If
            objRuleResponse.Add(ObjRet)
        Next
        Return objRuleResponse
    End Function


End Class
