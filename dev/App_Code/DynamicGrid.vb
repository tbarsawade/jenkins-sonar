Imports Microsoft.VisualBasic
Imports System.Data
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Converters

Public Class DynamicGrid
    Public Shared Function CreateColCollection(dt As DataTable) As List(Of dColumn)
        Dim lstColName As New List(Of dColumn)()
        Dim i As Integer = 0
        Dim objG As dColumn
        'objG = New dColumn()
        'lstColName.Add(objG)
        For Each col As DataColumn In dt.Columns
            If (col.ToString.ToUpper <> "TID") Then
                objG = New dColumn()
                objG.field = System.Text.RegularExpressions.Regex.Replace(col.ColumnName, "[^a-zA-Z0-9]", "_")
                objG.title = col.ColumnName.ToString
                objG.width = 150 ' col.ColumnName.Length * 4
                lstColName.Add(objG)
            End If
        Next
        Return lstColName
    End Function

    Public Shared Function JsonTableSerializer(dt As DataTable) As String
        For Each col As DataColumn In dt.Columns
            If (col.ToString.ToUpper <> "TID") Then
                dt.Columns(col.ColumnName).ColumnName = System.Text.RegularExpressions.Regex.Replace(col.ColumnName, "[^a-zA-Z0-9]", "_")
            End If
        Next
        Dim jsonData As String = ""
        Dim serializerSettings As New JsonSerializerSettings()
        Try
            If (dt.Rows.Count > 0) Then
                serializerSettings.Converters.Add(New DataTableConverter())
                jsonData = JsonConvert.SerializeObject(dt, Newtonsoft.Json.Formatting.None, serializerSettings)
            End If
            Return jsonData
        Catch ex As Exception
            Throw New Exception("DataLib.JsonTableSerializer")
        End Try
    End Function

    Public Shared Function GridData(dt As DataTable, msg As String) As DGrid
        Dim obj As New DGrid()
        obj.Success = IIf(msg.Trim = "", True, False)
        obj.Message = msg
        obj.Count = dt.Rows.Count
        If Not obj.Success Then
            Return obj
        End If
        If dt.Rows.Count > 0 Then
            obj.Column = CreateColCollection(dt)
            obj.Data = JsonTableSerializer(dt)
        End If
        Return obj
    End Function

End Class

Public Class DGrid
    Public Data As String = ""
    Public Column As New List(Of dColumn)
    Public Success As Boolean
    Public Message As String
    Public Count As Integer = 0
    Public Chart As String = ""
End Class

Public Class dColumn
    Public Sub New()
    End Sub
    Public Sub New(fld As String, ttl As String)
        Me.field = System.Text.RegularExpressions.Regex.Replace(fld, "[^a-zA-Z0-9]", "_")
        Me.title = ttl
    End Sub
    Public field As String = ""
    Public title As String
    Public width As Integer
End Class
