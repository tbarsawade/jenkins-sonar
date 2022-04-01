Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Diagnostics

Public Class UpdateSiteCsv



    Public Function UpdateCsv(Tid As Integer, Eid As Integer, Document As String) As Integer
        Dim MatchFound As Integer = 0
        Dim track
        Try
            Dim qry As String = "Select * from mmm_mst_SiteCSVSettings where Eid=" & Eid & " and SiteDoc='" & Document & "' and IsActive=1"
            Dim dal As New BpmHelper()
            Dim obj = dal.EcecDataSet(qry)

            If obj.Success Then
                Dim ds = obj.obj
                Dim dt As DataTable = ds.Tables(0)
                If dt.Rows.Count = 0 Then
                    Return 0
                End If
                Dim dtFields As DataTable = dal.EcecDataSet("Select * from mmm_mst_Fields where Eid=" & Eid & " and Documenttype='Site' and FieldMapping='" & dt.Rows(0).Item("SiteTypefld") & "'").obj.Tables(0)

                qry = "Select *, dms.udf_split('" & dtFields.Rows(0).Item("dropdown") & "', " & dt.Rows(0).Item("SiteTypefld") & ") as SiteType from mmm_mst_master where Tid=" & Tid
                Dim dtMaster = dal.EcecDataSet(qry).obj.Tables(0)

                Dim strPath As String = HttpContext.Current.Server.MapPath("Scripts/CsvJson_" & Eid & ".txt")
                Dim csvString As String = Nothing

                If System.IO.File.Exists(strPath) Then
                    csvString = IO.File.ReadAllText(strPath)
                Else
                    System.IO.File.Create(strPath)
                End If

                Dim updatedCsv As New StringBuilder()
                Dim lines As String() = csvString.Split("|")
                For i As Integer = 0 To lines.Length - 1
                    Try

                    
                    Dim cols = lines(i).Split("^")
                    track = cols
                    If Not cols(0) = Tid Then
                        MatchFound = IIf(MatchFound > 0, MatchFound, 0)
                        updatedCsv.Append((String.Join("^", cols)) & "|")
                    Else
                        MatchFound = Tid
                        If dtMaster.Rows(0).Item("IsAuth") = "1" Then
                            Dim latLong = dtMaster.Rows(0).Item(dt.Rows(0).Item("SiteLatLong")).ToString.Split(",")
                            cols(1) = dtMaster.Rows(0).Item(dt.Rows(0).Item("SiteIDfld"))
                            cols(2) = dtMaster.Rows(0).Item(dt.Rows(0).Item("SiteNamefld"))
                            cols(3) = dtMaster.Rows(0).Item("SiteType")
                            If latLong.Length > 1 Then
                                cols(4) = latLong(0)
                                cols(5) = latLong(1)
                            End If
                            cols(6) = dtMaster.Rows(0).Item(dt.Rows(0).Item("SiteTypefld"))
                                cols(7) = dtMaster.Rows(0).Item(dt.Rows(0).Item("SiteRightfld"))

                                Dim str = (String.Join("^", cols))
                                Dim lastLetter As Char = str.Substring(str.Length - 1, 1)
                                If lastLetter = "^" Then
                                    str = str.Substring(0, str.Length - 1)
                                End If
                                updatedCsv.Append(str & "|")
                        End If

                        End If

                    Catch ex As Exception
                        Dim str = i
                    End Try
                Next

                If MatchFound = 0 Then
                    Dim strArr(8) As String
                    Dim latLong = dtMaster.Rows(0).Item(dt.Rows(0).Item("SiteLatLong")).ToString.Split(",")
                    strArr(0) = Tid
                    strArr(1) = dtMaster.Rows(0).Item(dt.Rows(0).Item("SiteIDfld"))
                    strArr(2) = dtMaster.Rows(0).Item(dt.Rows(0).Item("SiteNamefld"))
                    strArr(3) = dtMaster.Rows(0).Item("SiteType")
                    If latLong.Length > 1 Then
                        strArr(4) = latLong(0)
                        strArr(5) = latLong(1)
                    End If
                    strArr(6) = dtMaster.Rows(0).Item(dt.Rows(0).Item("SiteTypefld"))
                    strArr(7) = dtMaster.Rows(0).Item(dt.Rows(0).Item("SiteRightfld"))

                    Dim str = (String.Join("^", strArr))
                    Dim lastLetter As Char = str.Substring(str.Length - 1, 1)
                    If lastLetter = "^" Then
                        str = str.Substring(0, str.Length - 1)
                    End If
                    updatedCsv.Append(str & "|")
                End If


                If System.IO.File.Exists(strPath) Then
                    System.IO.File.WriteAllText(strPath, updatedCsv.ToString())
                Else
                    System.IO.File.Create(strPath)
                    System.IO.File.WriteAllText(strPath, updatedCsv.ToString())
                End If

            End If

        Catch ex As Exception
            Dim st As New StackTrace(True)
            st = New StackTrace(ex, True)
            Dim str As String = st.GetFrame(0).GetFileLineNumber().ToString()
        End Try
        Return MatchFound
    End Function


    Function ReadCSV(csvString As String, colSeperator As String, rowSeperator As String, FirstRowHasColumns As Boolean, Optional ByVal Cols As String = Nothing) As DataTable
        Try
            Dim lines As String() = csvString.Split(rowSeperator)
            Dim recs As New DataTable()

            Dim colsArr As String()
            If FirstRowHasColumns Then
                colsArr = lines(0).Split(colSeperator)
            Else
                colsArr = Cols.Split(colSeperator)
            End If

            For Each s As String In colsArr
                recs.Columns.Add(s)
            Next
            Dim row As DataRow
            Dim finalLine As String = ""
            For Each line As String In lines
                row = recs.NewRow()
                finalLine = line.Replace(Convert.ToString(ControlChars.Cr), "")
                row.ItemArray = finalLine.Split(colSeperator)
                recs.Rows.Add(row)
            Next
            Return recs
        Catch ex As Exception
            Throw ex
        End Try
    End Function


    Function WriteCsv(dt As DataTable, colSeperator As String, rowSeperator As String, FirstRowAsColumn As Boolean) As String
        Dim csvStr As New StringBuilder()
        Try
            If FirstRowAsColumn Then
                For i As Integer = 0 To dt.Columns.Count - 1
                    csvStr.Append(dt.Columns(i).ColumnName & IIf(i = dt.Columns.Count - 1, rowSeperator, colSeperator))
                Next
            End If
            For Each row As DataRow In dt.Rows
                For i As Integer = 0 To dt.Columns.Count - 1
                    csvStr.Append(row.Item(i) & IIf(i = dt.Columns.Count - 1, rowSeperator, colSeperator))
                Next
            Next
            Return csvStr.ToString()
        Catch ex As Exception
            Throw ex
        End Try
    End Function


End Class
