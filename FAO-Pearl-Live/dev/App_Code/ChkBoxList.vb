Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Data.SqlClient

Public Class ChkBoxList
    Public Shared Function GetCHKID(CHKLST As DataRow, EID As Integer, CHKText As String, obj As LineitemWrap) As String
        Dim StrResult = ""
        Dim ds As New DataSet()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim strQuery As String = ""
        Dim con As SqlConnection = Nothing
        Dim sda As SqlDataAdapter = Nothing
        ' Dim tableName = ""
        Dim DocumentType = ""
        Dim FieldMapping = ""
        Dim TID As String = "TID"
        Try
            Select Case CHKLST.Item("DropDownType").ToString.ToUpper
                Case "MASTER VALUED"
                    Dim arr = CHKLST.Item("DropDown").ToString().Split("-")
                    Dim FieldID = CHKLST.Item("FieldID").ToString()
                    Try
                        Dim TABLENAME As String = ""
                        If UCase(arr(0).ToString()) = "MASTER" Then
                            TABLENAME = "MMM_MST_MASTER"
                            TID = "TID"
                        ElseIf UCase(arr(0).ToString()) = "DOCUMENT" Then
                            TABLENAME = "MMM_MST_DOC"
                        ElseIf UCase(arr(0).ToString) = "STATIC" Then
                            If arr(1).ToString.ToUpper = "USER" Then
                                TABLENAME = "MMM_MST_USER"
                                TID = "UID"
                            Else
                            End If
                        End If
                        Dim DSIntl = New DataSet()
                        Dim CHKList = CHKText.Split(",")
                        For Each itemChk In CHKList
                            If TABLENAME = "MMM_MST_MASTER" Or TABLENAME = "MMM_MST_DOC" Then
                                strQuery = "select " & arr(2).ToString() & "," & TID & "[tid]  from " & TABLENAME & " M WHERE   DOCUMENTTYPE='" & arr(1).ToString() & "'"
                                strQuery = strQuery & " and M." & arr(arr.Length - 1) & " = '" & itemChk & "'"
                                strQuery = strQuery & " AND EID= " & EID
                            ElseIf TABLENAME = "MMM_MST_USER" Then
                                strQuery = "select " & arr(2).ToString() & "," & TID & " from " & TABLENAME & " M WHERE EID=" & EID & " " & ""
                                strQuery = strQuery & " and " & arr(2).ToString() & " = '" & itemChk & "'"
                            End If
                            If strQuery <> "" Then
                                sda = New SqlDataAdapter(strQuery, conStr)
                                ds = New DataSet()
                                sda.Fill(ds)
                                If ds.Tables(0).Rows.Count > 0 Then
                                    StrResult = StrResult & ds.Tables(0).Rows(0).Item(TID).ToString() & ","
                                Else
                                    StrResult = StrResult & "-1,"
                                End If
                            End If
                        Next
                        StrResult = StrResult.Substring(0, StrResult.Length - 1)
                    Catch ex As Exception
                    End Try
                Case "FIX VALUED"
                    strQuery = "select dropdown from mmm_mst_fields where documenttype='" + CHKLST.Item("DOCUMENTTYPE").ToString + "' and eid=" + EID.ToString + " and fieldid=" + CHKLST.Item("FieldId").ToString + ""
                    sda = New SqlDataAdapter(strQuery, conStr)
                    ds = New DataSet()
                    sda.Fill(ds)
                    Dim isFound As Boolean = False
                    If ds.Tables(0).Rows.Count > 0 Then
                        Dim strValue = ds.Tables(0).Rows(0).Item("dropdown").ToString.Split(",")
                        Dim ChkValue = CHKText.Split(",")
                        For Each item In ChkValue
                            For Each ItemValue In strValue
                                If item.ToUpper = ItemValue.ToUpper Then
                                    isFound = True
                                    Exit For
                                Else
                                    isFound = False
                                End If
                            Next
                            If (isFound) Then
                                StrResult = StrResult & item + ","
                            Else
                                StrResult = StrResult & "-1,"
                            End If
                        Next
                        StrResult = StrResult.Substring(0, StrResult.Length - 1)
                    Else
                    End If
            End Select
        Catch ex As Exception
        Finally
            If Not con Is Nothing Then
                con.Close()
                con.Dispose()
            End If
        End Try
        Return StrResult

    End Function
End Class
