Imports System.Web.HttpResponseBase
Imports System.IO
Imports System.Data.SqlClient
Imports System.Data

Public Class MasterBL

    Public Sub ExportDataSetToExcel(ds As DataSet, FileName As String)
        HttpContext.Current.Response.Clear()
        HttpContext.Current.Response.Buffer = True
        HttpContext.Current.Response.AddHeader("content-disposition", "attachment;filename=" + FileName + ".xls")
        HttpContext.Current.Response.Charset = ""
        HttpContext.Current.Response.ContentType = "application/vnd.ms-excel"
        Dim sw As New StringWriter()
        Dim hw As New HtmlTextWriter(sw)

        For i As Integer = ds.Tables.Count - 1 To 0 Step -1
            Dim dgt As New DataGrid()
            dgt.DataSource = ds.Tables(i)
            dgt.DataBind()
            dgt.RenderControl(hw)
            hw.InnerWriter.Write("<br/><br/><br/>")
        Next
        Dim dg As New DataGrid()
        Dim style As String = "<style> .textmode { mso-number-format:\@; } </style>"
        HttpContext.Current.Response.Write(style)
        HttpContext.Current.Response.Output.Write(sw.ToString())
        HttpContext.Current.Response.Flush()
        HttpContext.Current.Response.End()

    End Sub


    Function GetInversedDataTable(ByVal table As DataTable, ByVal columnX As String, ByVal nullValue As String) As DataTable

        Dim returnTable As New DataTable()
        'returnTable.Columns.Add("DocID")
        If columnX = "" Then
            columnX = table.Columns(0).ColumnName
        End If

        Dim columnXValues As New List(Of String)()

        For Each dr As DataRow In table.Rows
            Dim columnXTemp As String = dr(columnX).ToString()
            If Not columnXValues.Contains(columnXTemp) Then
                columnXValues.Add(columnXTemp)
                returnTable.Columns.Add(columnXTemp)
            End If
        Next
        If nullValue <> "" Then
            For Each dr As DataRow In returnTable.Rows
                For Each dc As DataColumn In returnTable.Columns
                    If dr(dc.ColumnName).ToString() = "" Then
                        dr(dc.ColumnName) = nullValue
                    End If
                Next
            Next
        End If
        Return returnTable
    End Function







    Public Function GetDataFromExcel(ByVal strDataFilePath As String) As DataTable
        ' GetDataFromExcel123(strDataFilePath)
        Try
            Dim sr As New StreamReader(HttpContext.Current.Server.MapPath("~/Import/" & strDataFilePath))
            Dim FileName = HttpContext.Current.Server.MapPath("~/Import/" & strDataFilePath)
            Dim fullFileStr As String = sr.ReadToEnd()
            sr.Close()
            sr.Dispose()
            Dim lines As String() = fullFileStr.Split(ControlChars.Lf)
            Dim recs As New DataTable()
            Dim sArr As String() = lines(0).Split(","c)
            For Each s As String In sArr
                recs.Columns.Add(New DataColumn(s.Trim()))
            Next
            Dim row As DataRow
            Dim finalLine As String = ""
            Dim i As Integer = 0
            For Each line As String In lines
                If i > 0 And Not String.IsNullOrEmpty(line.Trim()) Then
                    row = recs.NewRow()
                    finalLine = line.Replace(Convert.ToString(ControlChars.Cr), "")
                    row.ItemArray = finalLine.Split(","c)
                    recs.Rows.Add(row)
                End If
                i = i + 1
            Next
            Return recs
        Catch ex As Exception
            Throw ex
        End Try
    End Function
    Public Shared Function history(ByVal pid As Integer, ByVal doctype As String, ByVal eid As Integer) As DataSet
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
        Dim fldQry As String = ""
        oda.SelectCommand.CommandText = "uspGetHistoryDetail"
        oda.SelectCommand.CommandType = CommandType.StoredProcedure
        oda.SelectCommand.Parameters.AddWithValue("@DOcid", pid)
        oda.SelectCommand.Parameters.AddWithValue("@FN", doctype)
        oda.SelectCommand.Parameters.AddWithValue("@EID", eid)
        Dim ds As New DataSet()
        oda.Fill(ds, "data")
        Return ds
    End Function

End Class





