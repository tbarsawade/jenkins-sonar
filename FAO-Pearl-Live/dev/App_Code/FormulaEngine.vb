Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports System.Globalization
Imports System.Drawing
Imports AdvFormulaLib = ciloci.FormulaEngine


Public Class FormulaEngine
    Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
    Public EID As Integer
    Public FormulaFIeld As String = 0

    Public Function ExecuteFormula(Formula As String, EID As Integer, lstParentField As List(Of UserData), lstChildField As List(Of UserData), IsChildForm As Boolean, Optional BaseDocType As String = "") As String
        Dim StrRet As String = ""
        Dim ds As New DataSet()

        Try
            Dim Expression = ""
            Dim ObjRUle As New RuleEngin()
            Expression = Formula.ToString.ToUpper
            For Each obj In lstParentField
                Dim TestStr = "{FORM." & obj.DocumentType.Trim.ToUpper & "." & obj.DisplayName.ToUpper.Trim & "}"
                If Expression.Contains(TestStr) Then
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
            ds = GetFormulaRelation()
            If ds.Tables(0).Rows.Count > 0 Then
                Dim dsdata As New DataSet()
                For k As Integer = 0 To ds.Tables(0).Rows.Count - 1
                    'Getting where condition from Relation Identifier
                    'Creating where condition for getting result 
                    Dim OrderBy = ""
                    Dim whcond = ""
                    Dim sourceType As String = ""
                    Dim SourceName As String = Convert.ToString(ds.Tables(0).Rows(k).Item("SourceName"))
                    sourceType = Convert.ToString(ds.Tables(0).Rows(k).Item("sourceType"))
                    'Getting DocumentType Of main Form in case of document Type
                    Dim dsSName As New DataSet()
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
                    If BaseDocType = "" Then
                        whcond = "v" & EID & ds.Tables(0).Rows(k).Item("TargetName").ToString.Trim.Replace(" ", "_") & "." & "[" & ds.Tables(0).Rows(k).Item("T_RelationIdentifierField") & "]=" & "'{" & SourceName.ToString.Trim & "." & ds.Tables(0).Rows(k).Item("S_relationidentifierField") & "}'"
                    Else
                        whcond = "v" & EID & BaseDocType.ToString.Trim.Replace(" ", "_") & "." & "[" & ds.Tables(0).Rows(k).Item("T_RelationIdentifierField") & "]=" & "'{" & SourceName.ToString.Trim & "." & ds.Tables(0).Rows(k).Item("S_relationidentifierField") & "}'"
                    End If
                    If Not String.IsNullOrEmpty(ds.Tables(1).Rows(k).Item("sortingfield").ToString.Trim) Then
                        OrderBy = "order by CAST(v" & EID & ds.Tables(0).Rows(k).Item("TargetName").ToString.Trim.Replace(" ", "_") & "." & "[" & ds.Tables(0).Rows(k).Item("sortingfield") & "] AS DATE) DESC"
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
                    dsdata = ObjRUle.GenerateQueryForPreLoad(EID, ds.Tables(1).Rows(k).Item("TargetName"), whcond, OrderBy)
                    If dsdata.Tables(0).Rows.Count > 0 Then
                        For Each column As DataColumn In dsdata.Tables(0).Columns
                            Dim TestStr = "{DS." & column.ColumnName.Trim.ToUpper & "}"
                            If Expression.Contains(TestStr) Then
                                Expression = Expression.Replace(TestStr, dsdata.Tables(0).Rows(0).Item(column.ColumnName))
                            End If
                        Next
                    Else
                        Throw New Exception("Error in formula execution")
                    End If
                Next
            End If
            'executing frimula here
            If Not String.IsNullOrEmpty(Expression.Trim) Then
                Dim obj As New AdvFormulaLib.FormulaEngine()
                Dim o As AdvFormulaLib.Formula = obj.CreateFormula(Expression)
                Dim t = o.Evaluate()
                StrRet = t.ToString
            End If
        Catch ex As Exception
            Throw (New Exception("Error in formula Execution."))
        End Try
        Return StrRet

    End Function

    Public Function GetFormulaRelation() As DataSet
        Dim ds As New DataSet()
        Using con As New SqlConnection(conStr)
            Using da As New SqlDataAdapter("GetFormulaRelation", con)
                da.SelectCommand.CommandType = CommandType.StoredProcedure
                da.SelectCommand.Parameters.AddWithValue("@EID", EID)
                da.SelectCommand.Parameters.AddWithValue("@FormulaID", 0)
                da.Fill(ds)
            End Using
        End Using
        Return ds
    End Function

End Class
