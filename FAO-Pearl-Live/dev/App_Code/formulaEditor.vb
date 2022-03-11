Imports Microsoft.VisualBasic
Imports System.Data.SqlClient
Imports System.Data

Public Class formulaEditor
    Dim datatable As New DataTable()
    Dim doccolums As New DataTable()
    ' Dim doccolumsvalue As New DataTable()
    'Dim docview As String=String=String.Empty
    'Commented By Komal on 28March2014
    'Public Function ExecuteFormula(query As String, tid As Integer, tablename As String) As String
    Public Function ExecuteFormula(query As String, tid As Integer, tablename As String, eid As String, Is_Draft As Integer) As String
        Dim cal As String = String.Empty
        Dim contrs As String = ConfigurationManager.ConnectionStrings("constr").ConnectionString
        Dim con As New SqlConnection(contrs)
        Dim oda As New SqlDataAdapter("", con)
        Try
            '  docview = tablename
            '  Dim dt As New DataTable
            '  Dim docname As String = docview
            Dim docname As String = tablename.ToUpper
            'Commented By Komal on 28March2014
            'docname = docname.Replace("V" + HttpContext.Current.Session("eid").ToString + "", "")
            'docname = docname.Replace("_", " ")
            If Is_Draft = 0 Then
                docname = docname.Replace("V" + eid.ToString + "", "")
                docname = docname.Replace("_", " ")
            Else
                docname = docname.Replace("DV" + eid.ToString + "", "")
                docname = docname.Replace("_", " ")
            End If
            'Commented By Komal on 28March2014
            'oda.SelectCommand.CommandText = "select  * from mmm_mst_fields where   FieldType in ('Drop Down','List box','CheckBox List') and eid=" + HttpContext.Current.Session("eid").ToString + " and DocumentType='" + docname + "' and DropDownType='MASTER VALUED'  "
            oda.SelectCommand.CommandText = "select  * from mmm_mst_fields where   FieldType in ('Drop Down','List box','CheckBox List') and eid=" + eid.ToString + " and DocumentType='" + docname + "' and DropDownType='MASTER VALUED'  "
            oda.Fill(doccolums)
            'oda.SelectCommand.CommandText = "select R2,	B1	,r1	,T1 from " + tablename + " where tid =" + tid.ToString + " "
            oda.SelectCommand.CommandText = "select * from [" + tablename + "] where tid =" + tid.ToString + " "
            oda.SelectCommand.CommandType = CommandType.Text
            oda.Fill(datatable)
            query = query.Replace("{", "")
            query = query.Replace("}", "")
            'query = query.Replace(" ", "")

            Dim fun As Integer = 0
            query = query.Replace(query, query.ToUpper)

            'Dim f As Integer = query.IndexOf("ROUND")

            Dim roval As Integer
            If query.IndexOf("ROUND") >= 0 Then
                fun = 1
                'query = query.Replace("ROUND{", "")
                query = query.Remove(0, 6)

                query = query.Remove(query.Length - 1)
                Dim rforround() As String = query.Split(",")
                roval = Convert.ToInt32(rforround(rforround.Length - 1))
                query = query.Remove(query.Length - 1 - roval.ToString.Length)

            End If
            'Dim roundarr() As String = query.Split("ROUND")
            'If roundarr(0).ToUpper = "ROUND" Then
            '    fun = 1
            '    query = query.Replace(roundarr(0), "")


            '    Dim rforround() As String = query.Split(",")
            '    roval = Convert.ToInt32(rforround(rforround.Length - 1))
            '    query = query.Remove(query.Length - 1 - roval.ToString.Length)
            'End If

            Dim check As Boolean
            'Dim arr() As String = query.Split("*", "/", "-", "(", ")", "+")
            Dim arr() As String = query.Split("*", "/", "-", "(", ")", "+", ",", "=")
            Try
                For i As Integer = 0 To arr.Length - 1
                    If arr(i).Length > 1 Then
                        Dim urrfunction() As String = arr(i).Split("["c, "]", ",")
                        'Added By Komal on 1Feb2014
                        'If arr(0).ToUpper = "MAX" Then
                        '    If (urrfunction.Length >= 1) Then
                        '        query = urfunction(urrfunction, query)
                        '    End If
                        '    check = checkRcolumn(arr(i))
                        '    If check = True Then
                        '        Dim temp As String = giveRvalue(arr(i))
                        '        '   query = query.Replace("{" + arr(i) + "}", temp)
                        '        query = query.Replace(arr(i), temp)

                        '        arr(i) = temp
                        '    End If
                        '    'ElseIf arr(0).ToUpper = "IF" Then
                        '    '    Dim arr2() As String = query.Split("*", "/", "-", "(", ")", "+", ",", "=")
                        '    '    If (urrfunction.Length >= 1) Then
                        '    '        query = urfunction(urrfunction, query)
                        '    '    End If
                        '    '    check = checkRcolumn(arr2(i))
                        '    '    If check = True Then
                        '    '        Dim temp As String = giveRvalue(arr2(i))
                        '    '        '   query = query.Replace("{" + arr(i) + "}", temp)
                        '    '        query = query.Replace(arr2(i), temp)

                        '    '        arr(i) = temp
                        '    '    End If
                        'Else
                        If (urrfunction.Length >= 1) Then
                            query = urfunction(urrfunction, query)
                        End If
                        check = checkRcolumn(arr(i))
                        If check = True Then
                            Dim temp As String = giveRvalue(arr(i))
                            '   query = query.Replace("{" + arr(i) + "}", temp)
                            query = query.Replace(arr(i), temp)

                            arr(i) = temp
                        End If
                    End If
                    '  End If
                Next

                ' query = query.Replace(" ", String.Empty)

                ' cal = datatable.Compute(Convert.ToString(query), "").ToString()
                ' cal = dt.Compute(Convert.ToString(query), "").ToString()
                'Dim dt1 As New DataTable()
                'Dim call1 As String
                'call1 = dt1.Compute(Convert.ToString("1*30"), "").ToString()


                'Dim cal1 As Object

                'cal1 = datatable.Compute(query, Nothing)
                'Dim str As String = cal1.ToString

            Catch ex As Exception
                ' cal = ex.ToString
            End Try
            Try
                If fun = 1 Then
                    'Try
                    cal = datatable.Compute(Convert.ToString(query), "").ToString()
                    '    Dim roundoff As Double = Math.Round(Convert.ToDouble(cal), roval)
                    cal = Convert.ToString(Math.Round(Convert.ToDouble(cal), roval))
                    'Dim poi As Integer = cal.IndexOf(".")
                    'poi = poi + 1 + roval
                    'If cal.IndexOf(".") > 0 Then
                    '    cal = cal.Substring(0, poi)
                    'End If
                    'Catch ex As Exception
                    'End Try
                    'Else
                    '    '  Try

                    '    If arr(0).ToUpper = "MAX" Then
                    '        Dim removeSC = query.Remove(0, 4)
                    '        removeSC = removeSC.Trim(")").ToString
                    '        Dim Result As List(Of String) = removeSC.Split(",").ToList()
                    '        If Convert.ToInt32(Result(0)) > Convert.ToInt32(Result(1)) Then
                    '            cal = Convert.ToInt32(Result(0))
                    '        Else
                    '            cal = Convert.ToInt32(Result(1))
                    '        End If
                    'ElseIf arr(0).ToUpper = "IF" Then
                    '    'Dim contains As Boolean
                    '    Dim newQuery As String = ""
                    '    Dim sb As StringBuilder = New StringBuilder()
                    '    query = query.Replace("'", "")
                    '    Dim arr1() As String = query.Split("(", ")", ",", "+", "=")
                    '    Dim start1 = query.IndexOf("(")
                    '    Dim end1 As Integer = query.IndexOf(",")
                    '    Dim Cond As String = query.Substring(start1 + 1, end1 - start1 - 1)

                    '    If arr1(1) = arr1(2) Then
                    '        cal = datatable.Compute(Convert.ToString(arr1(3)), "").ToString()
                    '    Else
                    '        cal = datatable.Compute(Convert.ToString(arr1(4)), "").ToString()
                    '    End If
                    'Else
                    '    cal = datatable.Compute(Convert.ToString(query), "").ToString()
                    'End If
                    '    Catch ex As Exception
                Else
                    cal = CalculateFormulaonFly(query)
                    If (cal <> "") Then
                        cal = datatable.Compute(Convert.ToString(query), "").ToString()
                        '    Catch ex As Exception
                    End If
                End If
            Catch ex As Exception
            End Try
            
            datatable.Dispose()
            '  Dim dt As New DataTable()
            Return cal
        Catch ex As Exception
            Throw
        Finally
            con.Close()
            oda.Dispose()
            con.Dispose()
        End Try
    End Function
    'Out of use Now 

    Public Function ExecuteFormulaT(query As String, tid As Integer, tablename As String, eid As String, Is_Draft As Integer, con As SqlConnection, tran As SqlTransaction) As String
        Dim cal As String = String.Empty
        Dim contrs As String = ConfigurationManager.ConnectionStrings("constr").ConnectionString
        'Dim con As New SqlConnection(contrs)
        Dim oda As New SqlDataAdapter("", con)
        oda.SelectCommand.Transaction = tran
        Try
            '  docview = tablename
            '  Dim dt As New DataTable
            '  Dim docname As String = docview
            Dim docname As String = tablename.ToUpper
            'Commented By Komal on 28March2014
            'docname = docname.Replace("V" + HttpContext.Current.Session("eid").ToString + "", "")
            'docname = docname.Replace("_", " ")
            If Is_Draft = 0 Then
                docname = docname.Replace("V" + eid.ToString + "", "")
                docname = docname.Replace("_", " ")
            Else
                docname = docname.Replace("DV" + eid.ToString + "", "")
                docname = docname.Replace("_", " ")
            End If
            'Commented By Komal on 28March2014
            'oda.SelectCommand.CommandText = "select  * from mmm_mst_fields where   FieldType in ('Drop Down','List box','CheckBox List') and eid=" + HttpContext.Current.Session("eid").ToString + " and DocumentType='" + docname + "' and DropDownType='MASTER VALUED'  "
            oda.SelectCommand.CommandText = "select  * from mmm_mst_fields where   FieldType in ('Drop Down','List box','CheckBox List') and eid=" + eid.ToString + " and DocumentType='" + docname + "' and DropDownType='MASTER VALUED'  "
            oda.Fill(doccolums)
            'oda.SelectCommand.CommandText = "select R2,	B1	,r1	,T1 from " + tablename + " where tid =" + tid.ToString + " "
            oda.SelectCommand.CommandText = "select * from [" + tablename + "] where tid =" + tid.ToString + " "
            oda.SelectCommand.CommandType = CommandType.Text
            oda.Fill(datatable)
            query = query.Replace("{", "")
            query = query.Replace("}", "")
            'query = query.Replace(" ", "")

            Dim fun As Integer = 0
            query = query.Replace(query, query.ToUpper)

            'Dim f As Integer = query.IndexOf("ROUND")

            Dim roval As Integer
            If query.IndexOf("ROUND") >= 0 Then
                fun = 1
                'query = query.Replace("ROUND{", "")
                query = query.Remove(0, 6)

                query = query.Remove(query.Length - 1)
                Dim rforround() As String = query.Split(",")
                roval = Convert.ToInt32(rforround(rforround.Length - 1))
                query = query.Remove(query.Length - 1 - roval.ToString.Length)

            End If
            'Dim roundarr() As String = query.Split("ROUND")
            'If roundarr(0).ToUpper = "ROUND" Then
            '    fun = 1
            '    query = query.Replace(roundarr(0), "")


            '    Dim rforround() As String = query.Split(",")
            '    roval = Convert.ToInt32(rforround(rforround.Length - 1))
            '    query = query.Remove(query.Length - 1 - roval.ToString.Length)
            'End If

            Dim check As Boolean
            'Dim arr() As String = query.Split("*", "/", "-", "(", ")", "+")
            Dim arr() As String = query.Split("*", "/", "-", "(", ")", "+", ",", "=")
            Try
                For i As Integer = 0 To arr.Length - 1
                    If arr(i).Length > 1 Then
                        Dim urrfunction() As String = arr(i).Split("["c, "]", ",")
                        'Added By Komal on 1Feb2014
                        'If arr(0).ToUpper = "MAX" Then
                        '    If (urrfunction.Length >= 1) Then
                        '        query = urfunction(urrfunction, query)
                        '    End If
                        '    check = checkRcolumn(arr(i))
                        '    If check = True Then
                        '        Dim temp As String = giveRvalue(arr(i))
                        '        '   query = query.Replace("{" + arr(i) + "}", temp)
                        '        query = query.Replace(arr(i), temp)

                        '        arr(i) = temp
                        '    End If
                        '    'ElseIf arr(0).ToUpper = "IF" Then
                        '    '    Dim arr2() As String = query.Split("*", "/", "-", "(", ")", "+", ",", "=")
                        '    '    If (urrfunction.Length >= 1) Then
                        '    '        query = urfunction(urrfunction, query)
                        '    '    End If
                        '    '    check = checkRcolumn(arr2(i))
                        '    '    If check = True Then
                        '    '        Dim temp As String = giveRvalue(arr2(i))
                        '    '        '   query = query.Replace("{" + arr(i) + "}", temp)
                        '    '        query = query.Replace(arr2(i), temp)

                        '    '        arr(i) = temp
                        '    '    End If
                        'Else
                        If (urrfunction.Length >= 1) Then
                            query = urfunction(urrfunction, query)
                        End If
                        check = checkRcolumn(arr(i))
                        If check = True Then
                            Dim temp As String = giveRvalue(arr(i))
                            '   query = query.Replace("{" + arr(i) + "}", temp)
                            query = query.Replace(arr(i), temp)

                            arr(i) = temp
                        End If
                    End If
                    '  End If
                Next

                '  query = query.Replace(" ", String.Empty)

                ' cal = datatable.Compute(Convert.ToString(query), "").ToString()
                ' cal = dt.Compute(Convert.ToString(query), "").ToString()
                'Dim dt1 As New DataTable()
                'Dim call1 As String
                'call1 = dt1.Compute(Convert.ToString("1*30"), "").ToString()


                'Dim cal1 As Object

                'cal1 = datatable.Compute(query, Nothing)
                'Dim str As String = cal1.ToString

            Catch ex As Exception
                ' cal = ex.ToString
            End Try
            Try
                If fun = 1 Then
                    'Try
                    cal = datatable.Compute(Convert.ToString(query), "").ToString()
                    '    Dim roundoff As Double = Math.Round(Convert.ToDouble(cal), roval)
                    cal = Convert.ToString(Math.Round(Convert.ToDouble(cal), roval))
                    'Dim poi As Integer = cal.IndexOf(".")
                    'poi = poi + 1 + roval
                    'If cal.IndexOf(".") > 0 Then
                    '    cal = cal.Substring(0, poi)
                    'End If
                    'Catch ex As Exception
                    'End Try
                    'Else
                    '    '  Try

                    '    If arr(0).ToUpper = "MAX" Then
                    '        Dim removeSC = query.Remove(0, 4)
                    '        removeSC = removeSC.Trim(")").ToString
                    '        Dim Result As List(Of String) = removeSC.Split(",").ToList()
                    '        If Convert.ToInt32(Result(0)) > Convert.ToInt32(Result(1)) Then
                    '            cal = Convert.ToInt32(Result(0))
                    '        Else
                    '            cal = Convert.ToInt32(Result(1))
                    '        End If
                    'ElseIf arr(0).ToUpper = "IF" Then
                    '    'Dim contains As Boolean
                    '    Dim newQuery As String = ""
                    '    Dim sb As StringBuilder = New StringBuilder()
                    '    query = query.Replace("'", "")
                    '    Dim arr1() As String = query.Split("(", ")", ",", "+", "=")
                    '    Dim start1 = query.IndexOf("(")
                    '    Dim end1 As Integer = query.IndexOf(",")
                    '    Dim Cond As String = query.Substring(start1 + 1, end1 - start1 - 1)

                    '    If arr1(1) = arr1(2) Then
                    '        cal = datatable.Compute(Convert.ToString(arr1(3)), "").ToString()
                    '    Else
                    '        cal = datatable.Compute(Convert.ToString(arr1(4)), "").ToString()
                    '    End If
                    'Else
                    '    cal = datatable.Compute(Convert.ToString(query), "").ToString()
                    'End If
                    '    Catch ex As Exception
                Else
                    cal = CalculateFormulaonFly(query)
                    If (cal <> "") Then
                        cal = datatable.Compute(Convert.ToString(query), "").ToString()
                        '    Catch ex As Exception
                    End If
                End If
            Catch ex As Exception
            End Try

            datatable.Dispose()
            '  Dim dt As New DataTable()
            Return cal
        Catch ex As Exception
            Throw
        Finally
            'con.Close()
            oda.Dispose()
            'con.Dispose()
        End Try
    End Function
    Public Function ExecuteFormula(query As String, tid As Integer, tablename As String, EID As String) As String
        Dim cal As String = String.Empty
        Dim contrs As String = ConfigurationManager.ConnectionStrings("constr").ConnectionString
        Dim con As New SqlConnection(contrs)
        Dim oda As New SqlDataAdapter("", con)
        '  docview = tablename
        '  Dim dt As New DataTable
        '  Dim docname As String = docview
        Dim docname As String = tablename.ToUpper
        docname = docname.Replace("V" + EID.ToString + "", "")
        docname = docname.Replace("_", " ")
        oda.SelectCommand.CommandText = "select  * from mmm_mst_fields where   FieldType in ('Drop Down','List box','CheckBox List') and eid=" + EID.ToString + " and DocumentType='" + docname + "' and DropDownType='MASTER VALUED'  "
        oda.Fill(doccolums)
        'oda.SelectCommand.CommandText = "select R2,	B1	,r1	,T1 from " + tablename + " where tid =" + tid.ToString + " "
        oda.SelectCommand.CommandText = "select * from " + "[" + tablename + "]" + " where tid =" + tid.ToString + " "
        oda.SelectCommand.CommandType = CommandType.Text
        oda.Fill(datatable)
        query = query.Replace("{", "")
        query = query.Replace("}", "")
        'query = query.Replace(" ", "")

        Dim fun As Integer = 0
        query = query.Replace(query, query.ToUpper)

        'Dim f As Integer = query.IndexOf("ROUND")

        Dim roval As Integer
        If query.IndexOf("ROUND") >= 0 Then
            fun = 1
            'query = query.Replace("ROUND{", "")
            query = query.Remove(0, 6)

            query = query.Remove(query.Length - 1)
            Dim rforround() As String = query.Split(",")
            roval = Convert.ToInt32(rforround(rforround.Length - 1))
            query = query.Remove(query.Length - 1 - roval.ToString.Length)

        End If
        'Dim roundarr() As String = query.Split("ROUND")
        'If roundarr(0).ToUpper = "ROUND" Then
        '    fun = 1
        '    query = query.Replace(roundarr(0), "")


        '    Dim rforround() As String = query.Split(",")
        '    roval = Convert.ToInt32(rforround(rforround.Length - 1))
        '    query = query.Remove(query.Length - 1 - roval.ToString.Length)
        'End If

        Dim check As Boolean
        'Dim arr() As String = query.Split("*", "/", "-", "(", ")", "+")
        Dim arr() As String = query.Split("*", "/", "-", "(", ")", "+", ",", "=")
        Try
            For i As Integer = 0 To arr.Length - 1
                If arr(i).Length > 1 Then
                    Dim urrfunction() As String = arr(i).Split("["c, "]", ",")
                    'Added By Komal on 1Feb2014
                    'If arr(0).ToUpper = "MAX" Then
                    '    If (urrfunction.Length >= 1) Then
                    '        query = urfunction(urrfunction, query)
                    '    End If
                    '    check = checkRcolumn(arr(i))
                    '    If check = True Then
                    '        Dim temp As String = giveRvalue(arr(i))
                    '        '   query = query.Replace("{" + arr(i) + "}", temp)
                    '        query = query.Replace(arr(i), temp)

                    '        arr(i) = temp
                    '    End If
                    '    'ElseIf arr(0).ToUpper = "IF" Then
                    '    '    Dim arr2() As String = query.Split("*", "/", "-", "(", ")", "+", ",", "=")
                    '    '    If (urrfunction.Length >= 1) Then
                    '    '        query = urfunction(urrfunction, query)
                    '    '    End If
                    '    '    check = checkRcolumn(arr2(i))
                    '    '    If check = True Then
                    '    '        Dim temp As String = giveRvalue(arr2(i))
                    '    '        '   query = query.Replace("{" + arr(i) + "}", temp)
                    '    '        query = query.Replace(arr2(i), temp)

                    '    '        arr(i) = temp
                    '    '    End If
                    'Else
                    If (urrfunction.Length >= 1) Then
                        query = urfunction(urrfunction, query)
                    End If
                    check = checkRcolumn(arr(i))
                    If check = True Then
                        Dim temp As String = giveRvalue(arr(i))
                        '   query = query.Replace("{" + arr(i) + "}", temp)
                        query = query.Replace(arr(i), temp)

                        arr(i) = temp
                    End If
                End If
                '  End If
            Next

            ' query = query.Replace(" ", String.Empty)

            ' cal = datatable.Compute(Convert.ToString(query), "").ToString()
            ' cal = dt.Compute(Convert.ToString(query), "").ToString()
            'Dim dt1 As New DataTable()
            'Dim call1 As String
            'call1 = dt1.Compute(Convert.ToString("1*30"), "").ToString()


            'Dim cal1 As Object

            'cal1 = datatable.Compute(query, Nothing)
            'Dim str As String = cal1.ToString

        Catch ex As Exception
            ' cal = ex.ToString
        End Try
        Try
            If fun = 1 Then
                'Try
                cal = datatable.Compute(Convert.ToString(query), "").ToString()
                '    Dim roundoff As Double = Math.Round(Convert.ToDouble(cal), roval)
                cal = Convert.ToString(Math.Round(Convert.ToDouble(cal), roval))
                'Dim poi As Integer = cal.IndexOf(".")
                'poi = poi + 1 + roval
                'If cal.IndexOf(".") > 0 Then
                '    cal = cal.Substring(0, poi)
                'End If
                'Catch ex As Exception
                'End Try
            Else
                '  Try

                If arr(0).ToUpper = "MAX" Then
                    Dim removeSC = query.Remove(0, 4)
                    removeSC = removeSC.Trim(")").ToString
                    Dim Result As List(Of String) = removeSC.Split(",").ToList()
                    If Convert.ToDouble(Result(0)) > Convert.ToDouble(Result(1)) Then
                        cal = Convert.ToDouble(Result(0))
                    Else
                        cal = Convert.ToDouble(Result(1))
                    End If
                ElseIf arr(0).ToUpper = "IF" Then
                    'Dim contains As Boolean
                    Dim newQuery As String = ""
                    Dim sb As StringBuilder = New StringBuilder()
                    query = query.Replace("'", "")
                    Dim arr1() As String = query.Split("(", ")", ",", "+", "=")
                    Dim start1 = query.IndexOf("(")
                    Dim end1 As Integer = query.IndexOf(",")
                    Dim Cond As String = query.Substring(start1 + 1, end1 - start1 - 1)

                    If arr1(1) = arr1(2) Then
                        cal = datatable.Compute(Convert.ToString(arr1(3)), "").ToString()
                    Else
                        cal = datatable.Compute(Convert.ToString(arr1(4)), "").ToString()
                    End If
                Else
                    cal = datatable.Compute(Convert.ToString(query), "").ToString()
                End If
                '    Catch ex As Exception
            End If
        Catch ex As Exception
        End Try
        con.Close()
        oda.Dispose()
        con.Dispose()
        datatable.Dispose()
        '  Dim dt As New DataTable()
        Return cal
    End Function


    Private Function checkRcolumn(column As String) As Boolean
        Dim check As Boolean = False
        For i As Integer = 0 To datatable.Columns.Count - 1
            If datatable.Columns(i).ToString.ToUpper() = column.ToUpper Then
                check = True
            End If
        Next
        Return check
    End Function
    Private Function checkRcolumnvalue(column As String) As String
        Dim check As Boolean = False
        For i As Integer = 0 To datatable.Columns.Count - 1
            If datatable.Columns(i).ToString.ToUpper() = column.ToUpper Then
                check = True
            End If
        Next
        If check = True Then
            Return giveRvalue(column)
        End If
        Return column
    End Function
    Private Function giveRvalue(field As String) As String
        Dim ret As String = String.Empty
        'Dim constr As String = ConfigurationManager.ConnectionStrings("").ConnectionString
        'Dim con As New SqlConnection(constr)
        'Dim oda As New SqlDataAdapter("", con)
        'Dim dt As New DataTable
        'Dim docname As String = docview
        'docname = docname.Replace("V" + HttpContext.Current.Session("eid").ToString + "", "")
        'docname = docname.Replace("_", " ")
        'oda.SelectCommand.CommandText = "select  * from mmm_mst_fields where displayName='" + field + "' and FieldType='Drop Down' and eid=" + HttpContext.Current.Session("eid").ToString + " and DocumentType='" docname"'  "
        'oda.Fill(dt)

        Dim constr As String = ConfigurationManager.ConnectionStrings("constr").ConnectionString
        Dim con As New SqlConnection(constr)
        Dim oda As New SqlDataAdapter("", con)
        Try
            If doccolums.Rows.Count > 0 Then
                If doccolums.Rows(0)("displayName").ToString.ToUpper = field.ToUpper Then

                    Dim ddarr() As String = doccolums.Rows(0)("dropdown").ToString().Split("-")
                    If ddarr(0).ToUpper = "MASTER" Then
                        oda.SelectCommand.CommandText = "select " + ddarr(2) + " from mmm_mst_master where DocumentType='" + ddarr(1) + "'    and tid =" + datatable.Rows(0)(field).ToString + ""
                        'oda.SelectCommand.CommandText = "select " + ddarr(2) + " from " + doccolums.Rows(0)("DBTableName").ToString + " where DocumentType='" + ddarr(1) + "'    and tid =" + datatable.Rows(0)(field).ToString + ""
                    ElseIf ddarr(0).ToUpper = "DOCUMENT" Then
                        oda.SelectCommand.CommandText = "select " + ddarr(2) + " from mmm_mst_doc where DocumentType='" + ddarr(1) + "'    and tid =" + datatable.Rows(0)(field).ToString + ""
                    End If
                    ' doccolums.Rows(0)("DBTableName").ToString.ToUpper()
                    Dim dt As New DataTable
                    oda.Fill(dt)
                    If dt.Rows.Count > 0 Then
                        ret = dt.Rows(0).Item(ddarr(2)).ToString()
                    End If
                    dt.Dispose()
                    
                Else
                    If Convert.ToString(datatable.Columns(field).DataType) = "System.String" Then
                        If datatable.Rows(0)(field).ToString().ToCharArray().Any(AddressOf Char.IsNumber) = True Then
                            ret = datatable.Rows(0)(field).ToString
                        Else
                            ret = "'" + datatable.Rows(0)(field).ToString + "'"
                            'Added By Komal on 22Jan2013
                            If ret = "''" Then
                                ret = "0"
                            End If
                            'End
                        End If
                    Else
                        ret = datatable.Rows(0)(field).ToString
                    End If
                End If
            Else
                If Convert.ToString(datatable.Columns(field).DataType) = "System.String" Then
                    If datatable.Rows(0)(field).ToString().ToCharArray().Any(AddressOf Char.IsNumber) = True Then
                        ret = datatable.Rows(0)(field).ToString
                    Else
                        ret = "'" + datatable.Rows(0)(field).ToString + "'"
                    End If
                Else
                    ret = datatable.Rows(0)(field).ToString
                End If
            End If
            Return ret
        Catch ex As Exception
            Throw
        Finally
            con.Close()
            con.Dispose()
            oda.Dispose()
        End Try
        
    End Function


    Function urfunction(urr() As String, rr As String) As String
        'If urr(0) = "DaysInMonth" Then
        If urr(0).ToUpper = "DAYSINMONTH" Then
            '  rr = rr.Replace("DaysInMonth{".ToUpper + urr(1) + "}", DaysInMonth1(giveRvalue(urr(1))).ToString())
            'rr = rr.Replace("DaysInMonth{" + urr(1) + "}", DaysInMonth1(giveRvalue(urr(1))).ToString())
            rr = rr.Replace("DAYSINMONTH[" + urr(1) + "]", DaysInMonth1(giveRvalue(urr(1))).ToString())

        ElseIf urr(0).ToUpper = "DATEDIFF" Then
            rr = rr.Replace("DATEDIFF[".ToUpper + urr(1) + "," + urr(2) + "]", "(" + datediff(giveRvalue(urr(1)), giveRvalue(urr(2))).ToString() + "+1)")
        ElseIf urr(0).ToUpper = "ROUND" Then
            If urr(1).ToCharArray.Any(AddressOf Char.IsNumber) = True Then
                rr = rr.Replace("ROUND[" + urr(1) + "," + urr(2) + "]", rround(urr(1), urr(2)).ToString)
            Else
                rr = rr.Replace("ROUND[" + urr(1) + "," + urr(2) + "]", rround(giveRvalue(urr(1)), urr(2)).ToString)

            End If
        ElseIf urr(0).ToUpper = "DATEPART" Then
            rr = rr.Replace("DATEPART[" + urr(1) + "," + urr(2) + "]", datepart(urr(1), giveRvalue(urr(2))))
        ElseIf urr(0).ToUpper = "R" Then
            '  rr = rr.Replace("ROUNDING[" + urr(1) + "," + urr(2) + "]", rrounds(urr(1), giveRvalue(urr(2))))
            rr = rr.Replace("R[" + urr(1) + "," + urr(2) + "]", rrounds(urr(1), giveRvalue(urr(2))))
        ElseIf urr(0).ToUpper() = "EXPCLAIMHFCL" Then
            rr = rr.Replace("EXPCLAIMHFCL[" + urr(1) + "," + urr(2) + "," + urr(3) + "," + urr(4) + "]", EXPCLAIMHFCL(giveRvalue(urr(1)), giveRvalue(urr(2)), giveRvalue(urr(3)), giveRvalue(urr(4))))

        ElseIf urr(0).ToUpper = "MIN" Then
            '  rr = rr.Replace("ROUNDING[" + urr(1) + "," + urr(2) + "]", rrounds(urr(1), giveRvalue(urr(2))))
            If urr.Length > 1 Then
                Dim str As String = String.Empty
                str += "MIN["
                'Dim str As New StringBuilder()
                'str.Clear()
                '    str.Append(urr(0).ToUpper + "[")
                For i As Integer = 1 To urr.Length - 2
                    ' Str.Append(urr(i) + ",")
                    str += urr(i) + ","
                Next
                str = str.Remove(str.Length - 1)
                str += "]"
                '    str.Append("]")
                'urr.
                Dim dt(urr.Length - 3) As Double
                '  Dim dt(urr.Length - 3) As String
                '   urr.
                For i As Integer = 0 To urr.Length - 3
                    ' dt(i) = urr(i + 1)
                    dt(i) = Convert.ToDouble(checkRcolumnvalue(urr(i + 1)))
                Next

                ' Array.ConvertAll(urr.split(','), Double.Parse)
                '   Array.Resize(urr, urr.Length - 2)
                Dim st As String = dt.Min()
                '    Dim result() As String = urr.Skip(urr(0)).ToArray()
                ' rr = rr.Replace("MIN["        ' , rrounds(urr(1), giveRvalue(urr(2))))
                ' Dim result() As String = New String(urr.Length - 1)
                'Array.Copy((urr, 0, result, 0, result.Length)
                rr = rr.Replace(str.ToString, st)

                '  rr = rr.Replace(str.ToString, Convert.ToString(urr.Min()))
                'Dim result As String() = New String(urr.Length - 1)
                'Array.Copy((source, valueIndex, result, 0, result.Length)
                'For i As Integer = 1 To urr.Length

                'Next
            End If
        ElseIf urr(0).ToUpper = "MAX" Then
            If urr.Length > 1 Then
                Dim str As String = String.Empty
                str += "MAX["
                For i As Integer = 1 To urr.Length - 2

                    str += urr(i) + ","
                Next
                str = str.Remove(str.Length - 1)
                str += "]"

                Dim dt(urr.Length - 3) As Double

                For i As Integer = 0 To urr.Length - 3

                    dt(i) = Convert.ToDouble(checkRcolumnvalue(urr(i + 1)))
                Next

                Dim st As String = dt.Max()

                rr = rr.Replace(str.ToString, st)

            End If

        End If

        Return rr
    End Function

#Region "common"
    Function DaysInMonth1(dat As String) As Integer
        Dim dt As DateTime = DateTime.ParseExact(dat, "dd/MM/yy", Nothing)
        'Dim dt As DateTime = Convert.ToDateTime(dat)

        Dim days As Integer = DateTime.DaysInMonth(dt.Year, dt.Month)
        Return days
    End Function

    Function datepart(datepart1 As String, date1 As String) As String
        ' System.Microsoft.VisualBasic()

        Dim da As DateTime = DateTime.ParseExact(date1, "dd/MM/yy", Nothing)
        If datepart1 = "DAY" Then
            Return da.Day
        ElseIf datepart1 = "MONTH" Then
            Return da.Month
        ElseIf datepart1 = "YEAR" Then
            Return da.Year
            'ElseIf datepart1 = "YEAR1" Then
            '    Return da.DayOfYear
            '  Microsoft.VisualBasic.DatePart(DateInterval.Quarter, da)
            '    Microsoft.VisualBasic.DatePart(DateInterval.DayOfYear, da)

        ElseIf datepart1 = "QUARTER" Then
            If da.Month <= 3 Then                  'da.Month >= '4 And da.Month <= 6 Then
                Return "1"
            ElseIf da.Month <= 6 Then             'da.Month >= 4 And da.Month <= 6 Then
                Return "2"
            ElseIf da.Month <= 9 Then    'da.Month >= 4 And da.Month <= 12 Then
                Return "3"
            ElseIf da.Month <= 12 Then            'da.Month >= 4 And da.Month <= 12 Then
                Return "4"
            End If
        ElseIf datepart1 = "QUARTERDAYS" Then
            '  Dim i As Integer = DateTime.DaysInMonth(da.Year, 1) + DateTime.DaysInMonth(da.Year, 2) + DateTime.DaysInMonth(da.Year, 3)
            If da.Month <= 3 Then                  'da.Month >= '4 And da.Month <= 6 Then
                Return (DateTime.DaysInMonth(da.Year, 1) + DateTime.DaysInMonth(da.Year, 2) + DateTime.DaysInMonth(da.Year, 3)).ToString
            ElseIf da.Month <= 6 Then             'da.Month >= 4 And da.Month <= 6 Then
                Return (DateTime.DaysInMonth(da.Year, 4) + DateTime.DaysInMonth(da.Year, 5) + DateTime.DaysInMonth(da.Year, 6)).ToString
            ElseIf da.Month <= 9 Then    'da.Month >= 4 And da.Month <= 12 Then
                Return (DateTime.DaysInMonth(da.Year, 7) + DateTime.DaysInMonth(da.Year, 8) + DateTime.DaysInMonth(da.Year, 9)).ToString
            ElseIf da.Month <= 12 Then            'da.Month >= 4 And da.Month <= 12 Then
                Return (DateTime.DaysInMonth(da.Year, 10) + DateTime.DaysInMonth(da.Year, 11) + DateTime.DaysInMonth(da.Year, 12)).ToString
            End If
        ElseIf datepart1 = "HALF" Then
            If da.Month <= 6 Then
                Dim firstmonth As New DateTime(da.Year, 1, 1)
                Dim lastmonth As New DateTime(da.Year, 6, DateTime.DaysInMonth(da.Year, 6))
                Dim tp As TimeSpan = lastmonth - firstmonth
                Return (tp.TotalDays + 1).ToString

            ElseIf da.Month >= 7 Then
                Dim firstmonth As New DateTime(da.Year, 7, 1)
                Dim lastmonth As New DateTime(da.Year, 12, DateTime.DaysInMonth(da.Year, 12))
                Dim tp As TimeSpan = lastmonth - firstmonth
                Return (tp.TotalDays + 1).ToString

            End If

        End If
        Return Nothing
    End Function
    'Function quaterbydate()
    Function datediff(date1 As String, date2 As String) As Double
        Dim dt1 As DateTime = DateTime.ParseExact(date1, "dd/MM/yy", Nothing)
        Dim dt2 As DateTime = DateTime.ParseExact(date2, "dd/MM/yy", Nothing)
        Dim tp As TimeSpan = dt2 - dt1

        Return tp.TotalDays

    End Function
    Function rround(val As String, rr As String) As Double
        Return Math.Round(Convert.ToDouble(val), Convert.ToInt32(rr))
    End Function
    Function rrounds(val As String, rr As String) As Double
        Dim v As String = String.Empty
        Dim finval As Double
        If val = "QUARTER" Then

            If rr.IndexOf(".") >= 0 Then
                Dim values As String = rr.Substring(rr.IndexOf("."))
                ' Dim values1 As String = rr.Substring(rr.IndexOf("."), rr.Length - 1)

                'Dim values As String = rr.Substring(rr.IndexOf("."), rr.Length)
                '  Dim values As Double = Math.Truncate(Convert.ToDouble(rr))
                'Dim values As Double = Convert.ToDouble(rr) Mod 1

                If Convert.ToDouble(values) <= 0.25 Then
                    'Dim check As Double = 1 - Convert.ToDouble(values)
                    Dim check As Double = Convert.ToDouble(values)
                    If check >= 0.125 Then
                        finval = 0.25
                    End If
                ElseIf Convert.ToDouble(values) <= 0.5 Then
                    Dim check As Double = Convert.ToDouble(values)
                    If check >= 0.125 Then
                        finval = 0.5
                    End If
                ElseIf Convert.ToDouble(values) <= 0.75 Then
                    Dim check As Double = Convert.ToDouble(values)
                    If check >= 0.125 Then
                        finval = 0.5
                    End If
                ElseIf Convert.ToDouble(values) <= 0.3 * 1 / 3 Then
                    Dim check As Double = Convert.ToDouble(values)
                    If check >= 0.125 Then
                        finval = 0.5
                    End If
                End If
            Else
                Return rr
            End If

        ElseIf val = "HALF" Then
            Dim values As String = rr.Substring(rr.IndexOf("."))
            '   Dim values As String = rr.Substring(rr.IndexOf("."), rr.Length)
            If Convert.ToDouble(values) <= 0.5 Then
                'Dim check As Double = 1 - Convert.ToDouble(values)
                Dim check As Double = 0.125
                If check >= 0.125 Then
                    finval = 0.25
                End If
            ElseIf Convert.ToDouble(values) <= 0.3 * 1 / 3 Then
                Dim check As Double = 0.125
                If check >= 0.125 Then
                    finval = 0.5
                End If

            End If
        End If
        finval = Math.Floor(Convert.ToDouble(rr)) + finval
        Return finval
        'Return Math.Round(Convert.ToDouble(val), Convert.ToInt32(rr))
    End Function
    Function EXPCLAIMHFCL(field As String, field1 As String, field2 As String, field3 As String) As String
        If field.ToString = "0" Then
            '  Dim cal As Double = Convert.ToDouble(datediff(field3, field2)) / Convert.ToDouble(DaysInMonth1(field2))
            Dim cal As Double = Convert.ToDouble((datediff(field2, field3) + 1) * Convert.ToDouble(field1)) / Convert.ToDouble(DaysInMonth1(field2))
            Return cal.ToString()
        Else
            Dim cal As Double = Convert.ToDouble(field) * Convert.ToDouble(field1)
            Return cal.ToString()

        End If
        Return Nothing
    End Function
#End Region

#Region "CHILD"

    Private datarowforr As DataRow
    Private datatablechield As New DataTable
    'Commented By Komal on 18March2014
    'Function executeformulachielditem(query As String, ByVal datarowchield As DataRow, ByVal datatablechield1 As DataTable) As String
    '    ' datarowforr = DirectCast(datarowchield.ItemArray.Clone(), DataRow)

    '    datarowforr = datarowchield
    '    'For i As Integer = 0 To datarowchield.ItemArray.Count() - 1
    '    '    '    datarowforr.ItemArray(i) = datarowchield(i).ToString
    '    '    datarowforr.ItemArray(i) = Convert.ToString(datarowchield(i))

    '    'Next

    '    '   datatablechield = datatablechield1
    '    datatablechield = datatablechield1.Clone()

    '    'For Each dr As DataRow In datarowchield
    '    '    datatablechield.Columns.Add(dcolumns.ColumnName, GetType(String))
    '    '    '  DTVALUE.Columns.Add(dr.Item("Displayname"), GetType(String))
    '    'Next

    '    'For Each dcolumns As DataColumn In datatablechield1.Columns
    '    '    datatablechield.Columns.Add(dcolumns.ColumnName, GetType(String))
    '    '    '  DTVALUE.Columns.Add(dr.Item("Displayname"), GetType(String))
    '    'Next
    '    datatablechield.Rows.Clear()
    '    '  datatablechield.ImportRow(datarowforr)
    '    ' Dim drr As DataRow = datatablechield.NewRow()
    '    '  datatablechield.ImportRow(datarowforr)

    '    datatablechield.Rows.Add(datarowforr.ItemArray)

    '    'datatablechield.Rows.Add(datar
    '    Dim cal As String = String.Empty



    '    query = query.Replace("{", "")
    '    query = query.Replace("}", "")
    '    'query = query.Replace(" ", "")

    '    Dim fun As Integer = 0
    '    query = query.Replace(query, query.ToUpper)

    '    'Dim f As Integer = query.IndexOf("ROUND")

    '    Dim roval As Integer
    '    If query.IndexOf("ROUND") >= 0 Then
    '        fun = 1
    '        'query = query.Replace("ROUND{", "")
    '        query = query.Remove(0, 6)

    '        query = query.Remove(query.Length - 1)
    '        Dim rforround() As String = query.Split(",")
    '        roval = Convert.ToInt32(rforround(rforround.Length - 1))
    '        query = query.Remove(query.Length - 1 - roval.ToString.Length)

    '    End If
    '    'Dim roundarr() As String = query.Split("ROUND")
    '    'If roundarr(0).ToUpper = "ROUND" Then
    '    '    fun = 1
    '    '    query = query.Replace(roundarr(0), "")


    '    '    Dim rforround() As String = query.Split(",")
    '    '    roval = Convert.ToInt32(rforround(rforround.Length - 1))
    '    '    query = query.Remove(query.Length - 1 - roval.ToString.Length)
    '    'End If


    '    Dim arr() As String = query.Split("*", "/", "-", "(", ")", "+")
    '    Try
    '        For i As Integer = 0 To arr.Length - 1
    '            If arr(i).Length > 1 Then
    '                Dim urrfunction() As String = arr(i).Split("["c, "]", ",")
    '                If (urrfunction.Length > 1) Then
    '                    query = urfunctionc(urrfunction, query)
    '                End If
    '                Dim check As Boolean = checkRcolumnc(arr(i))
    '                If check = True Then
    '                    Dim temp As String = giveRvaluec(arr(i))
    '                    '   query = query.Replace("{" + arr(i) + "}", temp)
    '                    query = query.Replace(arr(i), temp)

    '                    arr(i) = temp
    '                End If
    '            End If
    '        Next

    '        'query = query.Replace(" ", String.Empty)

    '        ' cal = datatable.Compute(Convert.ToString(query), "").ToString()
    '        ' cal = dt.Compute(Convert.ToString(query), "").ToString()
    '        'Dim dt1 As New DataTable()
    '        'Dim call1 As String
    '        'call1 = dt1.Compute(Convert.ToString("1*30"), "").ToString()


    '        'Dim cal1 As Object

    '        'cal1 = datatable.Compute(query, Nothing)
    '        'Dim str As String = cal1.ToString

    '    Catch ex As Exception
    '        ' cal = ex.ToString
    '    End Try
    '    Try
    '        If fun = 1 Then
    '            'Try
    '            cal = datatable.Compute(Convert.ToString(query), "").ToString()
    '            '    Dim roundoff As Double = Math.Round(Convert.ToDouble(cal), roval)
    '            cal = Convert.ToString(Math.Round(Convert.ToDouble(cal), roval))
    '            'Dim poi As Integer = cal.IndexOf(".")
    '            'poi = poi + 1 + roval
    '            'If cal.IndexOf(".") > 0 Then
    '            '    cal = cal.Substring(0, poi)
    '            'End If
    '            'Catch ex As Exception
    '            'End Try
    '        Else
    '            '  Try
    '            cal = datatable.Compute(Convert.ToString(query), "").ToString()
    '            '    Catch ex As Exception
    '        End If
    '    Catch ex As Exception
    '    End Try
    '    datatablechield.Clear()
    '    'datarowforr.Delete()
    '    'datarowchield.Delete()
    '    'datatablechield1.Clear()
    '    datatablechield.Dispose()
    '    datatablechield.Dispose()
    '    '  Dim dt As New DataTable()
    '    Return cal
    'End Function


    'Private Function checkRcolumnc(column As String) As Boolean
    '    Dim check As Boolean = False
    '    For i As Integer = 0 To datatable.Columns.Count - 1

    '        If datarowforr.ItemArray(i).ToString.ToUpper() = column.ToUpper Then
    '            check = True
    '        End If
    '    Next
    '    Return check
    'End Function

    'Private Function giveRvaluec(field As String) As String
    '    Dim ret As String = String.Empty
    '    If Convert.ToString(datatable.Columns(field).DataType) = "System.String" Then
    '        If datarowforr.Item(field).ToString().ToCharArray().Any(AddressOf Char.IsNumber) = True Then
    '            ret = datarowforr.Item(field).ToString
    '        Else
    '            ret = "'" + datarowforr.Item(field).ToString + "'"
    '        End If
    '    Else
    '        ret = datarowforr.Item(field).ToString
    '    End If
    '    Return ret
    'End Function

    Function executeformulachielditem(query As String, ByVal datarowchield As DataRow, ByVal datatablechield1 As DataTable) As String
        ' datarowforr = DirectCast(datarowchield.ItemArray.Clone(), DataRow)

        datarowforr = datarowchield
        'For i As Integer = 0 To datarowchield.ItemArray.Count() - 1
        '    '    datarowforr.ItemArray(i) = datarowchield(i).ToString
        '    datarowforr.ItemArray(i) = Convert.ToString(datarowchield(i))

        'Next

        '   datatablechield = datatablechield1
        datatablechield = datatablechield1.Clone()

        'For Each dr As DataRow In datarowchield
        '    datatablechield.Columns.Add(dcolumns.ColumnName, GetType(String))
        '    '  DTVALUE.Columns.Add(dr.Item("Displayname"), GetType(String))
        'Next

        'For Each dcolumns As DataColumn In datatablechield1.Columns
        '    datatablechield.Columns.Add(dcolumns.ColumnName, GetType(String))
        '    '  DTVALUE.Columns.Add(dr.Item("Displayname"), GetType(String))
        'Next
        datatablechield.Rows.Clear()
        '  datatablechield.ImportRow(datarowforr)
        ' Dim drr As DataRow = datatablechield.NewRow()
        '  datatablechield.ImportRow(datarowforr)

        datatablechield.Rows.Add(datarowforr.ItemArray)

        'datatablechield.Rows.Add(datar
        Dim cal As String = String.Empty



        query = query.Replace("{", "")
        query = query.Replace("}", "")
        'query = query.Replace(" ", "")

        Dim fun As Integer = 0
        query = query.Replace(query, query.ToUpper)

        'Dim f As Integer = query.IndexOf("ROUND")

        Dim roval As Integer
        If query.IndexOf("ROUND") >= 0 Then
            fun = 1
            'query = query.Replace("ROUND{", "")
            query = query.Remove(0, 6)

            query = query.Remove(query.Length - 1)
            Dim rforround() As String = query.Split(",")
            roval = Convert.ToInt32(rforround(rforround.Length - 1))
            query = query.Remove(query.Length - 1 - roval.ToString.Length)

        End If
        'Dim roundarr() As String = query.Split("ROUND")
        'If roundarr(0).ToUpper = "ROUND" Then
        '    fun = 1
        '    query = query.Replace(roundarr(0), "")


        '    Dim rforround() As String = query.Split(",")
        '    roval = Convert.ToInt32(rforround(rforround.Length - 1))
        '    query = query.Remove(query.Length - 1 - roval.ToString.Length)
        'End If


        Dim arr() As String
        If query.ToUpper.StartsWith("MAX") Or query.ToUpper.StartsWith("IF") Or query.ToUpper.StartsWith("CONCATENATE") Then
            arr = query.Split("*", "/", "-", "(", ")", "+", ",", "=", ">", "<")
        Else
            arr = query.Split("*", "/", "-", "(", ")", "+")
        End If
        Try
            For i As Integer = 0 To arr.Length - 1
                If arr(i).Length > 1 Then
                    Dim urrfunction() As String = arr(i).Split("["c, "]", ",")
                    If (urrfunction.Length > 1) Then
                        query = urfunctionc(urrfunction, query)
                    End If
                    Dim check As Boolean = checkRcolumnc(arr(i))
                    If check = True Then
                        Dim temp As String = giveRvaluec(arr(i))
                        '   query = query.Replace("{" + arr(i) + "}", temp)
                        query = query.Replace(arr(i), temp)

                        arr(i) = temp
                    End If
                End If
            Next

            'query = query.Replace(" ", String.Empty)

            ' cal = datatable.Compute(Convert.ToString(query), "").ToString()
            ' cal = dt.Compute(Convert.ToString(query), "").ToString()
            'Dim dt1 As New DataTable()
            'Dim call1 As String
            'call1 = dt1.Compute(Convert.ToString("1*30"), "").ToString()


            'Dim cal1 As Object

            'cal1 = datatable.Compute(query, Nothing)
            'Dim str As String = cal1.ToString

        Catch ex As Exception
            ' cal = ex.ToString
        End Try
        Try
            If fun = 1 Then
                'Try
                cal = datatable.Compute(Convert.ToString(query), "").ToString()
                '    Dim roundoff As Double = Math.Round(Convert.ToDouble(cal), roval)
                cal = Convert.ToString(Math.Round(Convert.ToDouble(cal), roval))
                'Dim poi As Integer = cal.IndexOf(".")
                'poi = poi + 1 + roval
                'If cal.IndexOf(".") > 0 Then
                '    cal = cal.Substring(0, poi)
                'End If
                'Catch ex As Exception
                'End Try
            Else
                If arr(0).ToUpper = "MAX" Then
                    Dim removeSC = query.Remove(0, 4)
                    removeSC = removeSC.Replace(")", "").ToString
                    Dim Result As List(Of String) = removeSC.Split(",").ToList()
                    If Convert.ToDouble(Result(0)) > Convert.ToDouble(Result(1)) Then
                        cal = Convert.ToDouble(Result(0))
                    Else
                        cal = Convert.ToDouble(Result(1))
                    End If
                ElseIf arr(0).ToUpper = "IF" Then
                    'Dim contains As Boolean
                    Dim newQuery As String = ""
                    Dim sb As StringBuilder = New StringBuilder()
                    query = query.Replace("'", "")
                    Dim arr1() As String = query.Split("(", ")", ",", "+", "=", "<", ">")
                    Dim start1 = query.IndexOf("(")
                    Dim end1 As Integer = query.IndexOf(",")
                    Dim Cond As String = query.Substring(start1 + 1, end1 - start1 - 1)

                    If arr1(1) = arr1(2) Then
                        cal = datatable.Compute(Convert.ToString(arr1(3)), "").ToString()
                    Else
                        cal = datatable.Compute(Convert.ToString(arr1(4)), "").ToString()
                    End If
                ElseIf arr(0).ToUpper = "CONCATENATE" Then
                    Dim start1 = query.IndexOf("(")
                    Dim end1 As Integer = query.LastIndexOf(")") ' query.IndexOf(")")-- by jayant
                    Dim ConcatData As String = query.Substring(start1 + 1, end1 - start1 - 1)
                    Dim SplitData() As String = ConcatData.Split(",")
                    For i As Integer = 0 To SplitData.Length - 1
                        'cal = String.Concat(cal, SplitData(i))  '' prev
                        cal = String.Concat(cal, Replace(SplitData(i), "'", ""))   '' by sunil for removing single quote in string 19_apr_14 
                    Next
                Else
                    cal = datatable.Compute(Convert.ToString(query), "").ToString()
                End If
            End If
        Catch ex As Exception
        End Try
        datatablechield.Clear()
        'datarowforr.Delete()
        'datarowchield.Delete()
        'datatablechield1.Clear()
        datatablechield.Dispose()
        datatablechield.Dispose()
        '  Dim dt As New DataTable()
        Return cal
    End Function
    Private Function checkRcolumnc(column As String) As Boolean
        Dim check As Boolean = False
        For i As Integer = 0 To datatablechield.Columns.Count - 1
            If datatablechield.Columns(i).ToString.ToUpper() = column.ToUpper Then
                check = True
            End If
        Next
        Return check
    End Function
    Private Function checkRcolumncval(column As String) As String
        Dim check As Boolean = False
        For i As Integer = 0 To datatablechield.Columns.Count - 1
            If datatablechield.Columns(i).ToString.ToUpper() = column.ToUpper Then
                check = True
            End If
        Next
        If check = True Then
            Return giveRvaluec(column)
        End If
        Return column
    End Function

    Private Function giveRvaluec(field As String) As String
        Dim ret As String = String.Empty
        field = LTrim(RTrim(field))
        If Convert.ToString(datatablechield.Columns(field).DataType) = "System.String" Then
            If datatablechield.Rows(0)(field).ToString().ToCharArray().Any(AddressOf Char.IsNumber) = True Then
                ret = datatablechield.Rows(0)(field).ToString
            Else
                ret = "'" + datatablechield.Rows(0)(field).ToString + "'"
            End If
        Else
            ret = datatablechield.Rows(0)(field).ToString
        End If
        Return ret
    End Function


    Function urfunctionc(urr() As String, rr As String) As String

        If urr(0).ToUpper = "DAYSINMONTH" Then
            rr = rr.Replace("DAYSINMONTH[" + urr(1) + "]", DaysInMonth1(giveRvaluec(urr(1))).ToString())

        ElseIf urr(0).ToUpper = "DATEDIFF" Then
            rr = rr.Replace("DATEDIFF[".ToUpper + urr(1) + "," + urr(2) + "]", "(" + datediff(giveRvaluec(urr(1)), giveRvaluec(urr(2))).ToString() + "+1)")
        ElseIf urr(0).ToUpper = "ROUND" Then
            If urr(1).ToCharArray.Any(AddressOf Char.IsNumber) = True Then
                rr = rr.Replace("ROUND[" + urr(1) + "," + urr(2) + "]", rround(urr(1), urr(2)).ToString)
            Else
                rr = rr.Replace("ROUND[" + urr(1) + "," + urr(2) + "]", rround(giveRvaluec(urr(1)), urr(2)).ToString)

            End If
        ElseIf urr(0).ToUpper = "DATEPART" Then
            rr = rr.Replace("DATEPART[" + urr(1) + "," + urr(2) + "]", datepart(urr(1), giveRvaluec(urr(2))))
        ElseIf urr(0).ToUpper = "ROUNDING" Then
            rr = rr.Replace("DATEPART[" + urr(1) + "," + urr(2) + "]", datepart(urr(1), giveRvaluec(urr(2))))
        ElseIf urr(0).ToUpper = "ROUNDING" Then
            rr = rr.Replace("ROUNDING[" + urr(1) + "," + urr(2) + "]", rrounds(urr(1), giveRvaluec(urr(2))))

        ElseIf urr(0).ToUpper = "R" Then

            rr = rr.Replace("R[" + urr(1) + "," + urr(2) + "]", rrounds(urr(1), giveRvaluec(urr(2))))
        ElseIf urr(0).ToUpper() = "EXPCLAIMHFCL" Then
            rr = rr.Replace("EXPCLAIMHFCL[" + urr(1) + "," + urr(2) + "," + urr(3) + "," + urr(4) + "]", EXPCLAIMHFCL(giveRvaluec(urr(1)), giveRvaluec(urr(2)), giveRvaluec(urr(3)), giveRvaluec(urr(4))))
        ElseIf urr(0).ToUpper = "MIN" Then

            If urr.Length > 1 Then
                Dim str As String = String.Empty
                str += "MIN["

                For i As Integer = 1 To urr.Length - 2

                    str += urr(i) + ","
                Next
                str = str.Remove(str.Length - 1)
                str += "]"

                Dim dt(urr.Length - 3) As Double

                For i As Integer = 0 To urr.Length - 3

                    dt(i) = Convert.ToDouble(checkRcolumncval(urr(i + 1)))
                Next


                Dim st As String = dt.Min()

                rr = rr.Replace(str.ToString, st)


            End If
        ElseIf urr(0).ToUpper = "MAX" Then
            If urr.Length > 1 Then
                Dim str As String = String.Empty
                str += "MAX["
                For i As Integer = 1 To urr.Length - 2

                    str += urr(i) + ","
                Next
                str = str.Remove(str.Length - 1)
                str += "]"

                Dim dt(urr.Length - 3) As Double

                For i As Integer = 0 To urr.Length - 3

                    dt(i) = Convert.ToDouble(checkRcolumncval(urr(i + 1)))
                Next

                Dim st As String = dt.Max()

                rr = rr.Replace(str.ToString, st)

            End If

        End If


        ' End If

        Return rr
    End Function
#End Region

    'Calculate Button by Komal on 18March2014

    Function CalculateFormulaonFly(ByVal query As String) As String
        Dim FormulaResult As String = ""
        query = query.Replace("{", "")
        query = query.Replace("}", "")
        'query = query.Replace(" ", "")
        Dim fun As Integer = 0
        query = query.Replace(query, query.ToUpper)
        Dim roval As Integer
        If query.IndexOf("ROUND") >= 0 Then
            fun = 1
            'query = query.Replace("ROUND{", "")
            query = query.Remove(0, 6)

            query = query.Remove(query.Length - 1)
            Dim rforround() As String = query.Split(",")
            roval = Convert.ToInt32(rforround(rforround.Length - 1))
            query = query.Remove(query.Length - 1 - roval.ToString.Length)

        End If

        Dim check As Boolean
        Dim arr() As String
        'Dim arr() As String = query.Split("*", "-", "(", ")", "+", ",", "=")
        If query.ToUpper.StartsWith("MAX") Or query.ToUpper.StartsWith("IF") Or query.ToUpper.StartsWith("CONCATENATE") Then
            arr = query.Split("*", "/", "-", "(", ")", "+", ",", "=", "<", ">")
        Else
            arr = query.Split("*", "/", "-", "(", ")", "+")
        End If
        Try
            For i As Integer = 0 To arr.Length - 1
                If arr(i).Length > 1 Then
                    Dim urrfunction() As String = arr(i).Split("["c, "]", ",")
                    If (urrfunction.Length >= 1) Then
                        query = urfunction(urrfunction, query)
                    End If
                    check = checkRcolumn(arr(i))
                    If check = True Then
                        Dim temp As String = giveRvalue(arr(i))
                        '   query = query.Replace("{" + arr(i) + "}", temp)
                        query = query.Replace(arr(i), temp)

                        arr(i) = temp
                    End If
                End If
                '  End If
            Next

            ' query = query.Replace(" ", String.Empty)

        Catch ex As Exception
            ' cal = ex.ToString
        End Try
        Try
            If fun = 1 Then
                'Try
                FormulaResult = datatable.Compute(Convert.ToString(query), "").ToString()
                '    Dim roundoff As Double = Math.Round(Convert.ToDouble(cal), roval)
                FormulaResult = Convert.ToString(Math.Round(Convert.ToDouble(FormulaResult), roval))
            Else
                If arr(0).ToUpper = "MAX" Then
                    Dim removeSC = query.Remove(0, 4)
                    removeSC = removeSC.Replace(")", "").ToString
                    Dim Result As List(Of String) = removeSC.Split(",").ToList()
                    If Convert.ToDouble(Result(0)) > Convert.ToDouble(Result(1)) Then
                        FormulaResult = Convert.ToDouble(Result(0))
                    Else
                        FormulaResult = Convert.ToDouble(Result(1))
                    End If
                ElseIf arr(0).ToUpper = "IF" Then
                    'Dim contains As Boolean
                    Dim newQuery As String = ""
                    Dim sb As StringBuilder = New StringBuilder()
                    query = query.Replace("'", "")
                    Dim arr1() As String = query.Split("(", ")", ",", "+", "=")
                    Dim start1 = query.IndexOf("(")
                    Dim end1 As Integer = query.IndexOf(",")
                    Dim Cond As String = query.Substring(start1 + 1, end1 - start1 - 1)

                    If arr1(1) = arr1(2) Then
                        FormulaResult = datatable.Compute(Convert.ToString(arr1(3)), "").ToString()
                    Else
                        FormulaResult = datatable.Compute(Convert.ToString(arr1(4)), "").ToString()
                    End If
                ElseIf arr(0).ToUpper = "CONCATENATE" Then
                    Dim start1 = query.IndexOf("(")
                    Dim end1 As Integer = query.LastIndexOf(")") '' query.IndexOf(")") --jayant
                    Dim ConcatData As String = query.Substring(start1 + 1, end1 - start1 - 1)
                    Dim SplitData() As String = ConcatData.Split(",")
                    For i As Integer = 0 To SplitData.Length - 1
                        'FormulaResult = String.Concat(FormulaResult, SplitData(i))  ' prev line b4 10_may
                        FormulaResult = String.Concat(Replace(FormulaResult, "'", ""), Replace(SplitData(i), "'", ""))   ' new by sunil 10 may
                    Next
                Else
                    FormulaResult = datatable.Compute(Convert.ToString(query), "").ToString()
                End If
            End If
        Catch ex As Exception
        End Try
        Return FormulaResult
    End Function

    Function CalculateFormulaonFlyc(ByVal query As String, ByVal datatablechield As DataTable) As String
        Dim FormulaResult As String = ""
        query = query.Replace("{", "")
        query = query.Replace("}", "")
        'query = query.Replace(" ", "")
        Dim fun As Integer = 0
        query = query.Replace(query, query.ToUpper)
        Dim roval As Integer
        If query.IndexOf("ROUND") >= 0 Then
            fun = 1
            'query = query.Replace("ROUND{", "")
            query = query.Remove(0, 6)

            query = query.Remove(query.Length - 1)
            Dim rforround() As String = query.Split(",")
            roval = Convert.ToInt32(rforround(rforround.Length - 1))
            query = query.Remove(query.Length - 1 - roval.ToString.Length)

        End If

        Dim check As Boolean
        'Dim arr() As String = query.Split("*", "/", "-", "(", ")", "+")
        Dim arr() As String
        'Dim arr() As String = query.Split("*", "-", "(", ")", "+", ",", "=")
        If query.ToUpper.StartsWith("MAX") Or query.ToUpper.StartsWith("IF") Or query.ToUpper.StartsWith("CONCATENATE") Then
            arr = query.Split("*", "/", "-", "(", ")", "+", ",", "=", "<", ">")
        Else
            arr = query.Split("*", "/", "-", "(", ")", "+")
        End If

        Try
            For i As Integer = 0 To arr.Length - 1
                If arr(i).Length > 1 Then
                    Dim urrfunction() As String = arr(i).Split("["c, "]", ",")
                    If (urrfunction.Length > 1) Then
                        query = urfunctionc(urrfunction, query)
                    End If
                    check = checkRcolumnc(arr(i))
                    If check = True Then
                        Dim temp As String = giveRvaluec(arr(i))
                        '   query = query.Replace("{" + arr(i) + "}", temp)
                        query = query.Replace(arr(i), temp)

                        arr(i) = temp
                    End If
                End If
            Next

            'query = query.Replace(" ", String.Empty)

            ' cal = datatable.Compute(Convert.ToString(query), "").ToString()
            ' cal = dt.Compute(Convert.ToString(query), "").ToString()
            'Dim dt1 As New DataTable()
            'Dim call1 As String
            'call1 = dt1.Compute(Convert.ToString("1*30"), "").ToString()


            'Dim cal1 As Object

            'cal1 = datatable.Compute(query, Nothing)
            'Dim str As String = cal1.ToString

        Catch ex As Exception
            ' cal = ex.ToString
        End Try
        Try
            If fun = 1 Then
                'Try
                FormulaResult = datatable.Compute(Convert.ToString(query), "").ToString()
                '    Dim roundoff As Double = Math.Round(Convert.ToDouble(cal), roval)
                FormulaResult = Convert.ToString(Math.Round(Convert.ToDouble(FormulaResult), roval))
                'Dim poi As Integer = cal.IndexOf(".")
                'poi = poi + 1 + roval
                'If cal.IndexOf(".") > 0 Then
                '    cal = cal.Substring(0, poi)
                'End If
                'Catch ex As Exception
                'End Try
            Else
                If arr(0).ToUpper = "MAX" Then
                    Dim removeSC = query.Remove(0, 4)
                    'removeSC = removeSC.Trim(")").ToString
                    removeSC = removeSC.Replace(")", "")
                    Dim Result As List(Of String) = removeSC.Split(",").ToList()
                    If Convert.ToDouble(Result(0)) > Convert.ToDouble(Result(1)) Then
                        FormulaResult = Convert.ToDouble(Result(0))
                    Else
                        FormulaResult = Convert.ToDouble(Result(1))
                    End If
                ElseIf arr(0).ToUpper = "IF" Then
                    'Dim contains As Boolean
                    Dim newQuery As String = ""
                    Dim sb As StringBuilder = New StringBuilder()
                    query = query.Replace("'", "")
                    Dim arr1() As String = query.Split("(", ")", ",", "+", "=")
                    Dim start1 = query.IndexOf("(")
                    Dim end1 As Integer = query.IndexOf(",")
                    Dim Cond As String = query.Substring(start1 + 1, end1 - start1 - 1)

                    If arr1(1) = arr1(2) Then
                        FormulaResult = datatable.Compute(Convert.ToString(arr1(3)), "").ToString()
                    Else
                        FormulaResult = datatable.Compute(Convert.ToString(arr1(4)), "").ToString()
                    End If
                ElseIf arr(0).ToUpper = "CONCATENATE" Then
                    Dim start1 = query.IndexOf("(")
                    Dim end1 As Integer = query.LastIndexOf(")") '' query.IndexOf(")") --jayant
                    Dim ConcatData As String = query.Substring(start1 + 1, end1 - start1 - 1)
                    Dim SplitData() As String = ConcatData.Split(",")
                    For i As Integer = 0 To SplitData.Length - 1
                        ' FormulaResult = String.Concat(FormulaResult, SplitData(i)) prev b4 10_may
                        FormulaResult = String.Concat(Replace(FormulaResult, "'", ""), Replace(SplitData(i), "'", ""))   ' new by sunil 10 may
                    Next
                Else
                    FormulaResult = datatable.Compute(Convert.ToString(query), "").ToString()
                End If
            End If
        Catch ex As Exception
        End Try
        Return FormulaResult
    End Function

End Class
