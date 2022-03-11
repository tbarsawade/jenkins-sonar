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

Public Class DDLookUp

    Public Shared Function GetTID(DRDDL As DataRow, EID As Integer, DDlText As String, obj As LineitemWrap) As String

        DDlText = DDlText.Replace("'", "''")
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim ret As String = "-1"
        Dim ddlLFieldID As Integer = 0
        Dim ddlFieldID As Integer = 0
        Dim DocumentType As String = ""
        Dim ddllookupValues As String = ""
        Dim ddllookupFieldMapping As String = ""
        Dim ddllookupFieldDisplayName As String = ""
        Dim ddlDocType As String = ""
        Try
            ddlLFieldID = Convert.ToInt32(DRDDL.Item("DropDown").ToString)
            ddlFieldID = Convert.ToInt32(DRDDL.Item("FieldID").ToString)
            DocumentType = DRDDL.Item("DocumentType").ToString
            Dim dsDDl As New DataSet()
            'Getting Details of it's concern drop down defenition
            dsDDl = CommanUtil.GetFormFields(EID, DocumentType, ddlLFieldID)
            If dsDDl.Tables(0).Rows.Count > 0 Then
                ddllookupValues = Convert.ToString(dsDDl.Tables(0).Rows(0).Item("DDllookupvalue"))
                Dim arrDoc = Convert.ToString(dsDDl.Tables(0).Rows(0).Item("DropDown")).Split("-")
                ddlDocType = arrDoc(1).ToString.Trim
                'Breaking lookup value with , because there might be more than one ddllookuvalue can be configured over single dropdown
                Dim arrlook = ddllookupValues.Split(",")
                'Getting FieldMapping of concern 
                For i As Integer = 0 To arrlook.Length - 1
                    If arrlook(i).Trim() <> "" Then
                        Dim ar = arrlook(i).Split("-")
                        If ddlFieldID = ar(0).Trim Then
                            ddllookupFieldMapping = ar(1)
                            Exit For
                        End If
                    End If
                Next
                'Getting definition of target dropdown
                Dim Query As New StringBuilder()
                Query.Append("select dropdown,documenttype,FieldID,FieldMapping,DisplayName from MMM_MST_FIELDS where EID=" & EID & " AND DocumentType='" & ddlDocType & "' AND FieldMapping='" & ddllookupFieldMapping & "'")
                Dim dsC As New DataSet()
                Using con = New SqlConnection(conStr)
                    Using da = New SqlDataAdapter(Query.ToString, con)
                        da.Fill(dsC)
                    End Using
                End Using
                ddllookupFieldDisplayName = "[" & dsC.Tables(0).Rows(0).Item("DisplayName") & "]"
                Dim arrddltrg = dsC.Tables(0).Rows(0).Item("DropDown").ToString.Split("-")
                Dim trgDocType = arrddltrg(1).ToString.Trim
                Dim trgField = arrddltrg(2).ToString.Trim
                Dim tbl1 As String = ""
                Dim tbl2 As String = ""
                Dim al1 As String = ""
                Dim al2 As String = ""
                Dim tid1 As String = "tid"
                Dim tid2 As String = "tid"

                If trgDocType.ToUpper <> "USER" Then
                    tbl2 = "v" & EID & trgDocType.Trim.Replace(" ", "_")
                    al2 = "tbl1"
                    'Getting displayname of concern field
                    Dim dsM As New DataSet()
                    Using con = New SqlConnection(conStr)
                        Using da = New SqlDataAdapter("select dropdown,documenttype,FieldID,FieldMapping,DisplayName from MMM_MST_FIELDS where EID=" & EID & " AND DocumentType='" & trgDocType & "' AND FieldMapping='" & trgField & "'", con)
                            da.Fill(dsM)
                        End Using
                    End Using
                    trgField = "[" & dsM.Tables(0).Rows(0).Item("DisplayName") & "]"
                Else
                    tbl2 = "MMM_MST_USER"
                    al2 = "U"
                    tid2 = "UID"
                    If (DDlText.Contains("[]")) Then
                        Dim arrU = Split(DDlText, "[]")
                        DDlText = arrU(1).ToString.Trim
                    End If
                End If

                If ddlDocType.ToUpper <> "USER" Then
                    tbl1 = "v" & EID & ddlDocType.Trim.Replace(" ", "_")
                    al1 = "tbl2"
                Else
                    tbl1 = "MMM_MST_USER"
                    al1 = "U1"
                    tid1 = "UID"
                    ddllookupFieldDisplayName = dsC.Tables(0).Rows(0).Item("FieldMapping")
                End If
                'Query = New StringBuilder("select distinct " & al2 & "." & tid2 & " As tid" & " FROM " & tbl2 & " AS " & al2 & " left outer join " & tbl1 & " AS " & al1 & " on ")
                'Query.Append(al1 & ".[" & ddllookupFieldDisplayName & "] = " & al2 & "." & tid2 & " AND  " & al2 & "." & trgField & "='" & DDlText & "'")
                'Dim Q = Query.ToString
                'Dim b = 0

                'Query = New StringBuilder("select distinct " & al2 & "." & tid2 & " As tid" & " FROM " & tbl2 & " AS " & al2 & " WHERE " & al2 & "." & "EID= " & EID & " AND  " & al2 & "." & trgField & "='" & DDlText & "'")

                Query = New StringBuilder("select distinct " & al2 & "." & tid2 & " As tid" & " FROM " & tbl2 & " AS " & al2 & " WHERE " & al2 & "." & "EID= " & EID)
                If DDlText.Trim <> "" Then
                    Query = Query.Append(" AND  " & al2 & "." & trgField & "='" & DDlText & "'")
                End If
                'Query.Append(al1 & ".[" & ddllookupFieldDisplayName & "] = " & al2 & "." & tid2 & " AND  " & al2 & "." & trgField & "='" & DDlText & "'")
                Dim val = GetFilterOn(ddlLFieldID, obj)

                If val.Trim <> "0" Then
                    Dim Str As String = "(SELECT top 1 " & al1 & "." & ddllookupFieldDisplayName & " FROM " & tbl1 & " AS " & al1 & " WHERE EID= " & EID & " AND " & al1 & "." & tid1 & "='" & val & "')"
                    Query = Query.Append(" AND " & al2 & "." & tid2 & " = " & Str)
                End If
                Dim dsD As New DataSet()
                Using con = New SqlConnection(conStr)
                    Using da = New SqlDataAdapter(Query.ToString, con)
                        da.Fill(dsD)
                    End Using
                End Using
                If dsD.Tables(0).Rows.Count > 0 Then
                    ret = dsD.Tables(0).Rows(0).Item("tid")
                Else
                    If DDlText.Trim = "" Or DDlText.Trim = "0" Or DDlText.Trim.ToUpper = "SELECT" Then
                        ret = "0"
                    Else
                        ret = "-1"
                    End If
                End If
            Else
                Throw New Exception("Error in ddl lookup configuration.Defenition of parent dropdown not found.")
            End If
        Catch ex As Exception
            Return "-1"
        End Try
        Return ret
    End Function

    Public Shared Function GetFilterOn(FieldID As String, obj As LineitemWrap) As String
        Dim Result As String = ""
        Try
            For Each item In obj.DataItem
                If item.FieldID = FieldID Then
                    Result = item.Values
                    Exit For
                End If
            Next
        Catch ex As Exception
            Throw
        End Try
        Return Result
    End Function

End Class
