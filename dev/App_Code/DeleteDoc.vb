Imports System.Data
Imports System.Data.SqlClient
Imports System.Net
Imports System.IO
Imports System.Xml.Serialization
Imports System.Runtime.Serialization
Imports System.ServiceModel
Imports System.ServiceModel.Web
Imports System.Xml
Public Class DeleteDoc
    Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString

    Public Function DeleteDocument(EID As Integer, DocType As String, UID As String, strData As String) As String
        Dim ret = "RTO"
        'strData = "Key$$AAAEECALIDN000391CCO~DOCTYPE$$Leave Application~Data$${Start Date}>{getdate()}||{Start Date}<{getdate()-2}"
        Try
            Dim ds As New DataSet()
            Using con = New SqlConnection(conStr)
                Using da = New SqlDataAdapter("getDataOfForm100", con)
                    da.SelectCommand.CommandType = CommandType.StoredProcedure
                    da.SelectCommand.Parameters.AddWithValue("@EID", EID)
                    da.SelectCommand.Parameters.AddWithValue("@FormName", DocType)
                    da.Fill(ds)
                End Using
            End Using
            Dim Query = GenerateDeleteScript(EID, DocType, strData, ds)
            Using con = New SqlConnection(conStr)
                Using da = New SqlDataAdapter(Query, con)
                    con.Open()
                    Dim Count = da.SelectCommand.ExecuteScalar()

                    ret = Count & " Record(s) deleted successfully."
                End Using
            End Using
        Catch ex As Exception
            ErrorLog.sendMail("DeleteDoc.DeleteDocument", ex.Message)
            Return "RTO"
        End Try
        Return ret
    End Function
    Public Function GenerateDeleteScript(EID As Integer, DocType As String, strData As String, dsField As DataSet) As String
        Dim ret = ""
        Try
            'Getting Name of table
            Dim ds As New DataSet()
            DocType = DocType.Replace("-", String.Empty).Replace("'", String.Empty)
            Dim Query = "SELECT FormType FROM MMM_MST_FORMS WHERE EID= " & EID & " AND FormSource='MENU DRIVEN'  AND FormName='" & DocType & "'"
            Dim tableName = ""
            Using con = New SqlConnection(conStr)
                Using da = New SqlDataAdapter(Query, con)
                    da.Fill(ds)
                End Using
            End Using
            If ds.Tables(0).Rows.Count > 0 Then
                Dim tbl = ds.Tables(0).Rows(0).Item("FormType")
                If tbl.ToString.Trim.ToUpper = "MASTER" Then
                    tableName = "MMM_MST_MASTER"
                ElseIf tbl.ToString.Trim.ToUpper = "DOCUMENT" Then
                    tableName = "MMM_MST_DOC"
                End If
            Else
                Throw New Exception()
            End If
            Dim arr = strData.Split(">=", ">", "<", "=<", "<=", "||")
            For k As Integer = 0 To arr.Length - 1
                Dim str = arr(k).Trim.ToUpper.Replace("{", "").Replace("}", "")
                If str.Contains("GETDATE()") Then
                    'Dim v1 As String = arr(k).ToUpper.Trim
                    str = str.Replace("GETDATE()", "convert(date,getdate()")
                    str = str & ")"
                    strData = strData.Replace(arr(k), str)
                End If
            Next
            For i As Integer = 0 To dsField.Tables(0).Rows.Count - 1
                If strData.Contains("{" & dsField.Tables(0).Rows(i).Item("DisplayName") & "}") Then
                    If dsField.Tables(0).Rows(i).Item("datatype") = "Datetime" Then
                        strData = strData.Replace("{" & dsField.Tables(0).Rows(i).Item("DisplayName") & "}", "convert(date," & dsField.Tables(0).Rows(i).Item("FieldMapping") & ")")
                    Else
                        strData = strData.Replace("{" & dsField.Tables(0).Rows(i).Item("DisplayName") & "}", dsField.Tables(0).Rows(i).Item("FieldMapping"))
                    End If

                End If
            Next
            strData = strData.Replace("||", " AND ")
            ret = "set dateformat dmy;DECLARE @COUNT INT=0"
            ret = ret & ";SELECT @COUNT=COUNT(*) FROM  " & tableName & " WHERE EID= " & EID & " AND DocumentType='" & DocType & "' AND " & strData
            ret = ret & ";delete from " & tableName & " WHERE EID= " & EID & " AND DocumentType='" & DocType & "' AND " & strData
            ret = ret & "SELECT @COUNT;"
        Catch ex As Exception
            Throw
        End Try
        Return ret
    End Function
End Class
