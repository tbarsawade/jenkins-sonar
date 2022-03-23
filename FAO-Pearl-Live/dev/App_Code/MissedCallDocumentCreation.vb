Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Data.SqlClient
Imports System.Configuration
Imports System.Diagnostics
Public Class MissedCallDocumentCreation
    Dim Eid As Integer = 66
    Dim datatable As New DataTable()
    Dim DefaultUser As Integer = 7251
    Dim doccolums As New DataTable()
    'Created for previous document creation
    Public Function GetMissedCalls1(PhNo As String, hour As String) As DataTable
        Dim dtMissedCalls As New DataTable()
        Try
            'Dim con = New SqlConnection(ConfigurationManager.ConnectionStrings("conStr").ConnectionString)
            'Dim qry = "Select Distinct MissCallNo from mmm_mst_missedCall where MissCallNo='" & PhNo & "' and convert(date,MissCallDate) >=convert(date,GetDate()) and datePart(Hour, dateadd(HOUR,1,MissCallDate)) =datePart(Hour, getdate())"


            Dim con = New SqlConnection(ConfigurationManager.ConnectionStrings("conStr").ConnectionString)
            Dim qry = "Select Distinct MissCallNo from mmm_mst_missedCall where MissCallNo='" & PhNo & "' and convert(date,MissCallDate) >=convert(date,GetDate()) and datePart(Hour, dateadd(HOUR," & CType((CType(hour, Integer) + 1), String) & ",MissCallDate)) =datePart(Hour,dateadd(HOUR," & hour & ",getdate()) )"
            Dim da As New SqlDataAdapter(qry, con)
            da.Fill(dtMissedCalls)
        Catch ex As Exception

        End Try
        Return dtMissedCalls
    End Function

    Public Function GetMissedCalls(PhNo As String) As DataTable
        Dim dtMissedCalls As New DataTable()
        Try
            'Dim con = New SqlConnection(ConfigurationManager.ConnectionStrings("conStr").ConnectionString)
            'Dim qry = "Select Distinct MissCallNo from mmm_mst_missedCall where MissCallNo='" & PhNo & "' and convert(date,MissCallDate) >=convert(date,GetDate()) and datePart(Hour, dateadd(HOUR,1,MissCallDate)) =datePart(Hour, getdate())"


            Dim con = New SqlConnection(ConfigurationManager.ConnectionStrings("conStr").ConnectionString)
            Dim qry = "Select Distinct MissCallNo from mmm_mst_missedCall where MissCallNo='" & PhNo & "' and convert(date,MissCallDate) >=convert(date,GetDate()) and datePart(Hour, dateadd(HOUR,1,MissCallDate)) =datePart(Hour,getdate()) )"
            Dim da As New SqlDataAdapter(qry, con)
            da.Fill(dtMissedCalls)
        Catch ex As Exception

        End Try
        Return dtMissedCalls
    End Function

    Public Function GEtRoster() As DataTable
        Dim dt As New DataTable()
        Try
            Dim con = New SqlConnection(ConfigurationManager.ConnectionStrings("conStr").ConnectionString)
            'Dim qry = "select  * from mmm_mst_DOc where Eid=" & Eid & " and documenttype ='Night Guard Roster' and convert(date,fld18,3)=convert(date,getDate(),3)"
            Dim qry = "select  d.fld12, d.fld13, d.fld14, d.fld15, d.fld16, d.fld17, d.fld18, x.fld1,x.fld10,x.fld11 "
            qry &= " from mmm_mst_DOc d left join (Select DOcId,fld1, fld10, fld11 from mmm_mst_DOc_Item where documenttype='Guard Roster') x "
            qry &= " on d.tid=x.DocId where Eid=" & Eid & " and documenttype='Night Guard Roster' and convert(date,d.fld18,3)=convert(date,getDate()-1,3)"
            Dim da As New SqlDataAdapter("", con)
            da.SelectCommand.CommandText = qry
            da.Fill(dt)
        Catch ex As Exception
        Finally
        End Try
        Return dt
    End Function
    Public Function ExecuteExpression(Tid As Integer, Formula As String, TableName As String, tran As SqlTransaction, con As SqlConnection) As String
        Try
            Dim arr() As String = Formula.Split("*", "/", "-", "(", ")", "+", ",", "=")
            Dim Exp = Formula
            Dim da As SqlDataAdapter = New SqlDataAdapter("", con)
            If arr.Length > 0 Then
                Dim fldmapping As String = String.Join("','", arr)
                fldmapping = "'" & fldmapping & "'"
                fldmapping = fldmapping.Replace("}", "")
                fldmapping = fldmapping.Replace("{", "")
                fldmapping = fldmapping.Replace("(", "")
                fldmapping = fldmapping.Replace(")", "")
                fldmapping = fldmapping.Replace("[", "")
                fldmapping = fldmapping.Replace("]", "")

                Dim qry = "Select * from " & TableName & " where Tid=" & Tid
                Dim dtDoc As New DataTable()
                da.SelectCommand.CommandText = qry
                da.SelectCommand.Transaction = tran
                da.Fill(dtDoc)
                qry = "Select FieldMapping,DisplayName from mmm_mst_Fields where Eid=" & dtDoc.Rows(0).Item("Eid") & " and DocumentType='" & dtDoc.Rows(0).Item("DocumentType") & "' and DisplayName in (" & fldmapping & ")"
                da.SelectCommand.CommandText = qry
                Dim dtFld As New DataTable()
                da.Fill(dtFld)

                For i As Integer = 0 To dtFld.Rows.Count - 1
                    Exp = Exp.Replace(dtFld.Rows(i).Item("DisplayName"), dtFld.Rows(i).Item("FieldMapping"))
                Next
                For i As Integer = 0 To dtFld.Rows.Count - 1
                    Exp = Exp.Replace(dtFld.Rows(i).Item("FieldMapping"), dtDoc.Rows(0).Item(dtFld.Rows(i).Item("FieldMapping").ToString))
                Next

                Exp = Exp.Replace("}", "")
                Exp = Exp.Replace("{", "")
                Exp = Exp.Replace("(", "")
                Exp = Exp.Replace(")", "")
                Exp = Exp.Replace("[", "")
                Exp = Exp.Replace("]", "")

                Dim val = Convert.ToString(dtDoc.Compute(Exp, ""))
                Return val
            End If

        Catch ex As Exception

        End Try
        Return ""
    End Function
    'Function created by omkar for creating previous document
    Public Function PreviousDocumentCreate(timeinterval1 As String, fld1 As String, tran As SqlTransaction, con As SqlConnection, fld2 As String) As Integer
        '   Dim con = New SqlConnection(ConfigurationManager.ConnectionStrings("conStr").ConnectionString)

        Dim da As New SqlDataAdapter("", con)
        Dim qry As String

        ' con.Open()

        Dim fld3 = fld2
        'Dim timeInterval = "convert(varchar,datePart(Hour, dateadd(Mi, 5, getdate()))) + ':' + convert(varchar,datePart(Hour, dateadd(Mi, 65, getdate())))"
        Dim timeInterval = ""
        Dim time() As String = timeinterval1.Split(":")
        Dim l As Integer = 0
        Dim k As Integer
        For k = 1 To 6
            For j As Integer = 0 To time.Length - 1
                timeInterval = timeInterval + CType(CType(time(j), Integer) - k, String)
                timeInterval = timeInterval + ":"
            Next
            timeInterval = timeInterval.Substring(0, timeInterval.Length - 1)
            qry = "Select count(*) from mmm_mst_DOc where Documenttype='Missed Call' and Eid=" & Eid
            qry &= " and convert(date,adate)=convert(date,getdate())"
            qry &= " and fld15='" & timeInterval & "'"
            '  qry &= " and convert(varchar,datePart(Hour,adate))=convert(varchar,datePart(Hour, dateadd(Mi, 5, getdate()))-" & k & ")"
            qry &= " and fld13='" & fld1 & "'" 'Added for minute check on 9/21/2015 by omka
            da.SelectCommand.CommandText = qry
            da.SelectCommand.Transaction = tran
            Dim time1 = timeInterval.Split(":")
            Dim time2 As String
            If (CType(time1(0), Integer) > -1) Then
                time2 = "-" & time1(0)
                'Added for checking whether user has call in particular hour or not  Added on 29/09/15
                Dim i = GetMissedCalls1(fld3, time2)
                If i.Rows.Count > 0 Then
                    Return k - 1
                End If
            End If
            timeInterval = ""
            Dim curCount = Convert.ToInt32(da.SelectCommand.ExecuteScalar())

            If curCount = 0 And k < CType(time(0), Integer) Then
                ' tran.Commit()
                l = k
                Continue For
            Else
                Return k - 1
                Exit For
            End If
        Next

        'con.Close()

        Return l                                'returning last time when document has to be  created
    End Function

    '' Public Sub CreateDocument()
    'Dim con = New SqlConnection(ConfigurationManager.ConnectionStrings("conStr").ConnectionString)
    'Dim da As New SqlDataAdapter("", con)
    '    Try
    'Dim dtRoster = GEtRoster()
    '        For i As Integer = 0 To dtRoster.Rows.Count - 1
    'Dim tran As SqlTransaction = Nothing
    '            Try
    '                If Not con.State = ConnectionState.Open Then
    '                    con.Open()
    '                End If
    '                tran = con.BeginTransaction()
    '                da.SelectCommand.Transaction = tran
    'Dim qry = "Select fld10 from mmm_mst_Master where Documenttype='Delivery Branch' and Eid=" & Eid & " and Tid=" & dtRoster.Rows(i).Item("fld12")
    '                da.SelectCommand.CommandText = qry

    'Dim BranchCode = Convert.ToString(da.SelectCommand.ExecuteScalar())

    ''Dim timeInterval = "convert(varchar,datePart(Hour, dateadd(Mi, 5, getdate()))) + ':' + convert(varchar,datePart(Hour, dateadd(Mi, 65, getdate())))"
    'Dim timeInterval = ""
    '                If Now.Minute <= 35 Then
    '                    timeInterval = (Now.Hour - 1).ToString & ":" & Now.Hour.ToString
    '                Else
    '                    timeInterval = (Now.Hour).ToString & ":" & (Now.Hour + 1).ToString
    '                End If

    'Dim FineAmount = ""
    '                qry = "Select kc_logic from mmm_mst_Fields where Eid=" & Eid & " and documenttype ='Missed Call' and IsActive=1 and FieldMapping='fld2'"
    '                da.SelectCommand.CommandText = qry
    '                da.SelectCommand.Transaction = tran
    'Dim farmula As String = Convert.ToString(da.SelectCommand.ExecuteScalar())

    ''skip if created a document for this hour and guardID
    '                qry = "Select count(*) from mmm_mst_DOc where Documenttype='Missed Call' and Eid=" & Eid
    '                qry &= " and convert(date,adate)=convert(date,getdate())"
    '                qry &= " and convert(varchar,datePart(Hour,adate))=convert(varchar,datePart(Hour, dateadd(Mi, 5, getdate())))"
    '                qry &= " and fld13='" & dtRoster.Rows(i).Item("fld1") & "'"
    '                da.SelectCommand.CommandText = qry

    'Dim curCount = Convert.ToInt32(da.SelectCommand.ExecuteScalar())

    '                If curCount > 0 Then
    '                    tran.Commit()
    '                    Continue For
    '                End If

    'Dim DefaultCount = Convert.ToString(GetDefaultCount(dtRoster.Rows(i)))
    'Dim dtMissedCall = GetMissedCalls(dtRoster.Rows(i).Item("fld11"))

    '                If dtMissedCall.Rows.Count > 0 Then
    '                    tran.Commit()
    '                    Continue For
    '                End If

    '                DefaultCount = IIf(dtMissedCall.Rows.Count > 0, DefaultCount, (Convert.ToInt32(DefaultCount) + 1).ToString())

    'Dim DocDate = IIf(System.DateTime.Now.Day < 10, "0" & System.DateTime.Now.Day.ToString(), System.DateTime.Now.Day.ToString()) & "/" & IIf(System.DateTime.Now.Month < 10, "0" & System.DateTime.Now.Month.ToString, System.DateTime.Now.Month.ToString()) & "/" & System.DateTime.Now.Year.ToString.Substring(2, 2)

    'Dim StrStatus = GetStatus(dtRoster.Rows(i).Item("fld12").ToString)
    'Dim Ouid = ""
    'Dim curStatus = ""
    'Dim ordering = 0
    '                If Not StrStatus = "" Then
    '                    Ouid = StrStatus.Split(",")(0)
    '                    curStatus = StrStatus.Split(",")(1)
    '                    ordering = Convert.ToInt32(StrStatus.Split(",")(2))
    '                End If

    '                qry = "insert into mmm_mst_Doc(Eid,Documenttype,fld1,fld10,fld11,fld12,fld13,fld14,fld15,fld16,fld17,fld18,fld19,fld2,adate, IsAuth, Ouid, CurStatus, IsWorkFlow)"
    '                qry &= " Values(" & Eid & ", 'Missed Call','" & DocDate & "','" & dtRoster.Rows(i).Item("fld12") & "', '" & BranchCode & "', '" & dtRoster.Rows(i).Item("fld13") & "'"
    '                qry &= ", '" & dtRoster.Rows(i).Item("fld1") & "', '" & dtRoster.Rows(i).Item("fld10") & "', '" & timeInterval & "', '" & DefaultCount & "','" & dtRoster.Rows(i).Item("fld14") & "','" & dtRoster.Rows(i).Item("fld16") & "', '" & dtRoster.Rows(i).Item("fld15") & "', '" & FineAmount & "', getdate(),1, '" & DefaultUser & "', '" & curStatus & "', 1) ; Select @@identity"

    '                da.SelectCommand.CommandText = qry
    '                da.SelectCommand.Transaction = tran
    'Dim DocId As Integer = da.SelectCommand.ExecuteScalar()
    '                tran.Commit()
    'Dim val = ExecuteExpression(DocId, farmula, "mmm_mst_Doc")

    '                qry = "update mmm_mst_Doc set fld2='" & val & "' where Tid=" & DocId

    '                da.SelectCommand.CommandText = qry
    '                da.SelectCommand.ExecuteNonQuery()

    'Dim query = "insert into MMM_DOC_DTL(UserID, DocId, fdate, tdate, ptat, atat, aprStatus, ordering, DocNature, Lastupdate)"
    '                query &= " Values(" & DefaultUser & ", '" & DocId & "', getdate(), getdate(), 0,0,'UPLOADED',0, 'CREATED', getdate())"
    '                da.SelectCommand.CommandText = query
    '                da.SelectCommand.ExecuteNonQuery()

    '                query = "insert into MMM_DOC_DTL(UserID, DocId, fdate, ptat,  ordering, DocNature, Lastupdate)"
    '                query &= " Values(" & Ouid & ", '" & DocId & "', getdate(), 1,1, 'CREATED', getdate()) ; Select @@identity "
    '                da.SelectCommand.CommandText = query
    'Dim LastId As Integer = da.SelectCommand.ExecuteScalar()

    '                qry = "Update mmm_mst_Doc set LastTid='" & LastId & "' where Tid=" & DocId
    '                da.SelectCommand.CommandText = query
    '                da.SelectCommand.ExecuteNonQuery()

    '                con.Close()
    '            Catch ex As Exception
    '                tran.Rollback()
    '                con.Close()
    '                InsertError(ex)
    '            End Try
    '        Next

    '    Catch ex As Exception
    '        InsertError(ex)
    '    Finally

    '    End Try
    'End Sub

    Public Sub CreateDocument()
        Dim con = New SqlConnection(ConfigurationManager.ConnectionStrings("conStr").ConnectionString)
        Dim da As New SqlDataAdapter("", con)
        Try
            Dim dtRoster = GEtRoster()
            For i As Integer = 0 To dtRoster.Rows.Count - 1
                Dim tran As SqlTransaction = Nothing
                Try
                    If Not con.State = ConnectionState.Open Then
                        con.Open()
                    End If
                    tran = con.BeginTransaction()
                    da.SelectCommand.Transaction = tran
                    Dim qry = "Select fld10 from mmm_mst_Master where Documenttype='Delivery Branch' and Eid=" & Eid & " and Tid=" & dtRoster.Rows(i).Item("fld12")
                    da.SelectCommand.CommandText = qry

                    Dim BranchCode = Convert.ToString(da.SelectCommand.ExecuteScalar())

                    'Dim timeInterval = "convert(varchar,datePart(Hour, dateadd(Mi, 5, getdate()))) + ':' + convert(varchar,datePart(Hour, dateadd(Mi, 65, getdate())))"
                    Dim timeInterval = ""
                    If Now.Minute <= 35 Then
                        timeInterval = (Now.Hour - 1).ToString & ":" & Now.Hour.ToString
                    Else
                        timeInterval = (Now.Hour).ToString & ":" & (Now.Hour + 1).ToString
                    End If

                    Dim FineAmount = ""
                    qry = "Select kc_logic from mmm_mst_Fields with(nolock) where Eid=" & Eid & " and documenttype ='Missed Call' and IsActive=1 and FieldMapping='fld2'"
                    ''for fine formula calculation
                    da.SelectCommand.CommandText = qry
                    da.SelectCommand.Transaction = tran
                    Dim farmula As String = Convert.ToString(da.SelectCommand.ExecuteScalar())

                    'skip if created a document for this hour and guardID
                    'qry = "Select count(*) from mmm_mst_DOc where Documenttype='Missed Call' and Eid=" & Eid
                    'qry &= " and convert(date,adate)=convert(date,getdate())"
                    'qry &= " and convert(varchar,datePart(Hour,adate))=convert(varchar,datePart(Hour, dateadd(Mi, 5, getdate())))"
                    'qry &= " and fld13='" & dtRoster.Rows(i).Item("fld1") & "'"
                    'da.SelectCommand.CommandText = qry

                    qry = "Select count(*) from mmm_mst_DOc where Documenttype='Missed Call' and Eid=" & Eid    ' checking if misscall is created or not , if not then create document .
                    qry &= " and convert(date,adate)=convert(date,getdate())"
                    qry &= " and fld15='" & timeInterval & "'"
                    qry &= " and fld13='" & dtRoster.Rows(i).Item("fld1") & "'"
                    da.SelectCommand.CommandText = qry

                    ' chekcing only on condition guard id and timeinterval and date

                    Dim curCount = Convert.ToInt32(da.SelectCommand.ExecuteScalar())

                    If curCount > 0 Then  ' if count is greater then zero then do not create the document 
                        tran.Commit()
                        Continue For
                    End If
                    Dim k As Integer
                    k = PreviousDocumentCreate(timeInterval, dtRoster.Rows(i).Item("fld1"), tran, con, dtRoster.Rows(i).Item("fld11"))  ' check previous hour document is created or not

                    'If (k < 5) Then
                    '    k = k - 1
                    'End If
                    Dim time() As String = timeInterval.Split(":")

                    Dim DefaultCount = Convert.ToString(GetDefaultCount(dtRoster.Rows(i)))     ' 
                    Dim dtMissedCall = GetMissedCalls(dtRoster.Rows(i).Item("fld11"))

                    If dtMissedCall.Rows.Count > 0 Then
                        tran.Commit()
                        Continue For
                    End If

                    DefaultCount = IIf(dtMissedCall.Rows.Count > 0, DefaultCount, (Convert.ToInt32(DefaultCount) + 1).ToString())
                    If (CType(defaultcount, Integer) > CType(time(1), Integer)) Then
                        tran.Commit()
                        Continue For
                    End If
                    Dim DocDate = IIf(System.DateTime.Now.Day < 10, "0" & System.DateTime.Now.Day.ToString(), System.DateTime.Now.Day.ToString()) & "-" & IIf(System.DateTime.Now.Month < 10, "0" & System.DateTime.Now.Month.ToString, System.DateTime.Now.Month.ToString()) & "-" & System.DateTime.Now.Year.ToString.Substring(2, 2)

                    Dim StrStatus = GetStatus(dtRoster.Rows(i).Item("fld12").ToString)
                    Dim Ouid = ""
                    Dim curStatus = ""
                    Dim ordering = 0
                    If Not StrStatus = "" Then
                        Ouid = StrStatus.Split(",")(0)
                        curStatus = StrStatus.Split(",")(1)
                        ordering = Convert.ToInt32(StrStatus.Split(",")(2))
                    End If

                    Dim query As String

                    'Code for minutes check added by omkar on 21/09/2015
                    Dim a As Integer
                    a = k
                    Dim val As String
                    For l = k To 0 Step -1
                        timeInterval = ""
                        For j As Integer = 0 To time.Length - 1
                            timeInterval = timeInterval + CType(CType(time(j), Integer) - l, String)
                            timeInterval = timeInterval + ":"
                        Next
                        'time = timeInterval.Split(":")
                        timeInterval = timeInterval.Substring(0, timeInterval.Length - 1)
                        'If (l = 0) Then 'checking for current hour only
                        '    qry = "Select count(*) from mmm_mst_DOc where Documenttype='Missed Call' and Eid=" & Eid
                        '    qry &= " and convert(date,adate)=convert(date,getdate())"
                        '    '  qry &= " and convert(varchar,datePart(Hour,adate))=convert(varchar,datePart(Hour, dateadd(Mi, 5, getdate())))"
                        '    qry &= " and fld13='" & dtRoster.Rows(i).Item("fld1") & "'"
                        '    '  qry &= " and convert(varchar,datePart(minute,adate))=convert(varchar,datePart(minute, dateadd(second, 10, getdate()))) "  'for minute check
                        '    qry &= " and fld15='" & timeInterval & "'"                    'for timeinterval check
                        '    da.SelectCommand.CommandText = qry
                        '    curCount = Convert.ToInt32(da.SelectCommand.ExecuteScalar())

                        '    If curCount > 0 Then
                        '        ' tran.Commit()
                        '        Continue For
                        '    End If
                        'End If

                        qry = "insert into mmm_mst_Doc(Eid,Documenttype,fld1,fld10,fld11,fld12,fld13,fld14,fld15,fld16,fld17,fld18,fld19,fld2,adate, IsAuth, Ouid, CurStatus, IsWorkFlow)"
                        qry &= " Values(" & Eid & ", 'Missed Call','" & DocDate & "','" & dtRoster.Rows(i).Item("fld12") & "', '" & BranchCode & "', '" & dtRoster.Rows(i).Item("fld13") & "'"
                        qry &= ", '" & dtRoster.Rows(i).Item("fld1") & "', '" & dtRoster.Rows(i).Item("fld10") & "', '" & timeInterval & "', '" & DefaultCount & "','" & dtRoster.Rows(i).Item("fld14") & "','" & dtRoster.Rows(i).Item("fld16") & "', '" & dtRoster.Rows(i).Item("fld15") & "', '" & FineAmount & "', getdate() ,1, '" & Ouid & "', '" & curStatus & "', 1) ; Select @@identity"
                        DefaultCount = DefaultCount + 1
                        da.SelectCommand.CommandText = qry
                        'da.SelectCommand.Transaction = tran
                        Dim time5 = timeinterval.split(":")
                        If (CType(defaultcount, Integer) > CType(time5(1), Integer)) Then
                            Continue For
                        End If
                        Dim DocId As Integer = da.SelectCommand.ExecuteScalar()

                        If (l = k) Then
                            val = ExecuteExpression(DocId, farmula, "mmm_mst_Doc", tran, con)
                        End If

                        If (a = 0) Then
                            qry = "update mmm_mst_Doc set fld2='" & val & "' where Tid=" & DocId
                        End If
                        If (l = k) Then
                            qry = "update mmm_mst_Doc set fld2='" & val & "' where Tid=" & DocId

                            da.SelectCommand.CommandText = qry
                            da.SelectCommand.ExecuteNonQuery()


                            query = "insert into MMM_DOC_DTL(UserID, DocId, fdate, tdate, ptat, atat, aprStatus, ordering, DocNature, Lastupdate)"
                            query &= " Values(" & DefaultUser & ", '" & DocId & "', getdate(), getdate(), 0,0,'UPLOADED',0, 'CREATED', getdate())"
                            da.SelectCommand.CommandText = query
                            da.SelectCommand.ExecuteNonQuery()



                        Else
                            val = CType(CType(val, Integer) + 50, String)
                            qry = "update mmm_mst_Doc set fld2='" & val & "' where Tid=" & DocId

                            da.SelectCommand.CommandText = qry
                            da.SelectCommand.ExecuteNonQuery()


                            query = "insert into MMM_DOC_DTL(UserID, DocId, fdate, tdate, ptat, atat, aprStatus, ordering, DocNature, Lastupdate)"
                            query &= " Values(" & DefaultUser & ", '" & DocId & "', getdate(), getdate(), 0,0,'UPLOADED',0, 'CREATED', getdate())"
                            da.SelectCommand.CommandText = query
                            da.SelectCommand.ExecuteNonQuery()



                        End If

                    Next
                    tran.Commit()
                    con.Close()


                Catch ex As Exception
                    tran.Rollback()
                    con.Close()
                    InsertError(ex)
                End Try
            Next

        Catch ex As Exception
            InsertError(ex)
        Finally

        End Try
    End Sub

    Public Function GetDefaultCount(dtRoster As DataRow) As Integer
        Dim DefaultCount As Integer = 0
        Dim con = New SqlConnection(ConfigurationManager.ConnectionStrings("conStr").ConnectionString)
        Dim da As New SqlDataAdapter("", con)

        Try
            Dim qry = ""
            qry = "select * from mmm_mst_DOc where Eid=" & Eid & " and documenttype ='Missed Call' and fld13='" & dtRoster.Item("fld1") & "' and convert(date, fld1, 3)=convert(date,getdate())"
            da.SelectCommand.CommandText = qry
            Dim dtDocs As New DataTable()
            da.Fill(dtDocs)
            If dtDocs.Rows.Count > 0 Then
                Dim dr = dtDocs.Compute("MAX(fld16)", "")
                DefaultCount = Convert.ToInt32(dtDocs.Compute("MAX(fld16)", ""))
            End If
        Catch ex As Exception
        Finally
        End Try
        Return DefaultCount
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

            query = query.Replace(" ", String.Empty)

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

    'Public Function ExecuteExpression(Tid As Integer, Formula As String, TableName As String) As String
    '    Dim con = New SqlConnection(ConfigurationManager.ConnectionStrings("conStr").ConnectionString)
    '    Dim da As New SqlDataAdapter("", con)
    '    Try
    '        Dim arr() As String = Formula.Split("*", "/", "-", "(", ")", "+", ",", "=")
    '        Dim Exp = Formula
    '        If arr.Length > 0 Then
    '            Dim fldmapping As String = String.Join("','", arr)
    '            fldmapping = "'" & fldmapping & "'"
    '            fldmapping = fldmapping.Replace("}", "")
    '            fldmapping = fldmapping.Replace("{", "")
    '            fldmapping = fldmapping.Replace("(", "")
    '            fldmapping = fldmapping.Replace(")", "")
    '            fldmapping = fldmapping.Replace("[", "")
    '            fldmapping = fldmapping.Replace("]", "")

    '            Dim qry = "Select * from " & TableName & " where Tid=" & Tid
    '            Dim dtDoc As New DataTable()
    '            da.SelectCommand.CommandText = qry
    '            da.Fill(dtDoc)
    '            qry = "Select FieldMapping,DisplayName from mmm_mst_Fields where Eid=" & dtDoc.Rows(0).Item("Eid") & " and DocumentType='" & dtDoc.Rows(0).Item("DocumentType") & "' and DisplayName in (" & fldmapping & ")"
    '            da.SelectCommand.CommandText = qry
    '            Dim dtFld As New DataTable()
    '            da.Fill(dtFld)

    '            For i As Integer = 0 To dtFld.Rows.Count - 1
    '                Exp = Exp.Replace(dtFld.Rows(i).Item("DisplayName"), dtFld.Rows(i).Item("FieldMapping"))
    '            Next
    '            For i As Integer = 0 To dtFld.Rows.Count - 1
    '                Exp = Exp.Replace(dtFld.Rows(i).Item("FieldMapping"), dtDoc.Rows(0).Item(dtFld.Rows(i).Item("FieldMapping").ToString))
    '            Next

    '            Exp = Exp.Replace("}", "")
    '            Exp = Exp.Replace("{", "")
    '            Exp = Exp.Replace("(", "")
    '            Exp = Exp.Replace(")", "")
    '            Exp = Exp.Replace("[", "")
    '            Exp = Exp.Replace("]", "")

    '            Dim val = Convert.ToString(dtDoc.Compute(Exp, ""))
    '            Return val
    '        End If

    '    Catch ex As Exception

    '    End Try
    '    Return ""
    'End Function

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

    Public Function GetStatus(BranchID As String) As String
        Dim con = New SqlConnection(ConfigurationManager.ConnectionStrings("conStr").ConnectionString)
        Dim da As New SqlDataAdapter("", con)
        Dim ds As New DataSet()
        Try
            Dim qry = "Select top 1 aprStatus, RoleName, ordering from mmm_mst_AuthMetrix where Eid=66 and Doctype='Missed call' order by ordering"
            da.SelectCommand.CommandText = qry
            da.Fill(ds, "AuthMatrix")
            If ds.Tables("AuthMatrix").Rows.Count = 0 Then
                Return ""
            End If
            qry = "Select Uid from mmm_ref_Role_user where Eid=" & Eid & " and RoleName='" & ds.Tables("AuthMatrix").Rows(0).Item("RoleName") & "' and  fld1 like '%" & BranchID & "%' and IsCreate=1"
            da.SelectCommand.CommandText = qry
            da.Fill(ds, "User")
            If ds.Tables("User").Rows.Count = 0 Then
                Return ""
            End If

            Return ds.Tables("User").Rows(0).Item("Uid") & "," & ds.Tables("AuthMatrix").Rows(0).Item("aprStatus") & "," & ds.Tables("AuthMatrix").Rows(0).Item("ordering")

        Catch ex As Exception
        End Try
        Return ""
    End Function

    Public Function GetNextUserFromRolematrixT(ByVal docID As Long, ByVal EID As Long, ByVal CUID As Integer, ByVal qry As String, ByVal Auid As Integer, ByVal con As SqlConnection) As String
        Dim da As New SqlDataAdapter("select d.*, dt.ordering,dt.userid from MMM_MST_DOC D left outer join MMM_DOC_DTL dt on d.LastTID=dt.tid where EID=" & EID & " and d.tid=" & docID, con)
        'try Catch Block Added by Ajeet 
        Try
            Dim dtDoc As New DataTable
            da.Fill(dtDoc)
            Dim docType As String = dtDoc.Rows(0).Item("documenttype").ToString
            Dim CurOrdering As Integer = dtDoc.Rows(0).Item("ordering").ToString
            Dim Creator As Integer = dtDoc.Rows(0).Item("ouid").ToString
            Dim CurDocNature As String = dtDoc.Rows(0).Item("CurdocNature").ToString
            Dim CurrentUser As Integer = dtDoc.Rows(0).Item("userid").ToString
            ' Dim CurStatus As String = dtDoc.Rows(0).Item("CurStatus").ToString

            '''' get all rows after current ordering 
            'prev b4 skip feature
            'da.SelectCommand.CommandText = "select * from MMM_MST_AuthMetrix where EID=" & EID & " and doctype='" & docType & "' and docnature='" & CurDocNature & "' AND ordering >" & CurOrdering & " order by ordering"
            da.SelectCommand.CommandText = "select am.*,wf.isallowskip from MMM_MST_AuthMetrix am left join MMM_MST_WORKFLOW_STATUS wf on am.aprStatus=wf.StatusName and am.doctype=wf.Documenttype  where am.EID=" & EID & " and am.doctype='" & docType & "' and am.docnature='" & CurDocNature & "' AND am.ordering >" & CurOrdering & " order by am.ordering"

            Dim dtRM As New DataTable
            da.Fill(dtRM)
            Dim FoundUsers As Boolean = False
            Dim CurRoleName As String = ""
            Dim curAprStatus As String = ""
            Dim nxtUser As Integer
            Dim sRetMsg As String = ""
            Dim AllowSkip As Integer = 0
            Dim CheckSkipfeat As Boolean = False
            nxtUser = 0 '' intialize with zero 

            For k As Integer = 0 To dtRM.Rows.Count - 1  '' K loop till user founds for a role type
                'If k = 0 Then
                AllowSkip = dtRM.Rows(k).Item("isallowskip").ToString
                If AllowSkip = 1 Then
                    CheckSkipfeat = False
                Else
                    CheckSkipfeat = True
                End If
                'Else
                'CheckSkipfeat = False
                'End If

                If dtRM.Rows(k).Item("type").ToString = "ROLE" Then
                    CurRoleName = dtRM.Rows(k).Item("Rolename").ToString
                    curAprStatus = dtRM.Rows(k).Item("aprstatus").ToString
                    Dim dtTmp As New DataTable
                    Select Case CurRoleName
                        Case "#SELF"
                            nxtUser = Creator
                        Case "#CURRENTUSER"
                            nxtUser = CurrentUser
                        Case "#SUPERVISOR"
                            da.SelectCommand.CommandType = CommandType.Text
                            da.SelectCommand.Parameters.Clear()
                            da.SelectCommand.CommandText = "select " & dtRM.Rows(k).Item("fieldName").ToString & " from MMM_MST_user where EID=" & EID & " and uid=" & Creator
                            dtTmp.Clear()
                            da.Fill(dtTmp)
                            If dtTmp.Rows.Count <> 0 Then
                                If IsNumeric(dtTmp.Rows(0).Item(0).ToString()) Then
                                    nxtUser = dtTmp.Rows(0).Item(0).ToString()
                                End If
                            End If
                        Case "#LAST SUPERVISOR"
                            Dim slvl As Integer = Val(dtRM.Rows(k).Item("sLevel").ToString)
                            Dim LScreator As Integer = Creator
                            Dim tmpUser As Integer
                            Dim LSfound As Boolean = True
                            For m As Integer = 1 To slvl
                                da.SelectCommand.CommandType = CommandType.Text
                                da.SelectCommand.Parameters.Clear()
                                da.SelectCommand.CommandText = "select " & dtRM.Rows(k).Item("fieldName").ToString & " from MMM_MST_user where EID=" & EID & " and uid=" & LScreator
                                dtTmp.Clear()
                                da.Fill(dtTmp)
                                If dtTmp.Rows.Count <> 0 Then
                                    If IsNumeric(dtTmp.Rows(0).Item(0).ToString) = True Then
                                        tmpUser = dtTmp.Rows(0).Item(0).ToString
                                    Else
                                        'nxtUser = tmpUser
                                        LSfound = False
                                        Exit For
                                    End If
                                End If
                                LScreator = tmpUser
                            Next
                            If LSfound = True Then
                                nxtUser = tmpUser
                            End If
                        Case "#USER"
                            da.SelectCommand.CommandType = CommandType.Text
                            da.SelectCommand.Parameters.Clear()
                            da.SelectCommand.CommandText = "select " & dtRM.Rows(k).Item("fieldName").ToString & " from MMM_MST_Doc where EID=" & EID & " and tid=" & docID
                            dtTmp.Clear()

                            da.Fill(dtTmp)
                            If dtTmp.Rows.Count <> 0 Then
                                If IsNumeric(dtTmp.Rows(0).Item(0).ToString) = True Then
                                    nxtUser = dtTmp.Rows(0).Item(0).ToString
                                End If
                            End If
                        Case Else
                            '' any other role 
                            da.SelectCommand.CommandType = CommandType.Text
                            da.SelectCommand.Parameters.Clear()
                            da.SelectCommand.CommandText = "select formid, docmapping, FORMNAME from MMM_MST_FORMS where EID=" & EID & " and isRoleDef=1"
                            Dim dtRRef As New DataTable
                            da.Fill(dtRRef)
                            Dim strMainQry As String = ""
                            Dim strFldQry As String = ""
                            strMainQry = "select uid from MMM_Ref_Role_User where Eid=" & EID & " and ',' + documenttype + ','  like '%," & docType & ",%' and rolename='" & CurRoleName & "'"

                            For i As Integer = 0 To dtRRef.Rows.Count - 1
                                da.SelectCommand.CommandType = CommandType.Text
                                da.SelectCommand.Parameters.Clear()
                                da.SelectCommand.CommandText = "select fieldmapping from MMM_MST_FIELDS where  Eid=" & EID & " and DocumentType='" & docType & "' and dropdowntype='MASTER VALUED' and substring(dropdown,8,(charindex('-',dropdown,8)-8)) = '" & dtRRef.Rows(i).Item("formname").ToString & "'"
                                Dim dtDF As New DataTable
                                da.Fill(dtDF)
                                If dtDF.Rows.Count > 0 Then   '' write in else of this to add def % in condition. 
                                    da.SelectCommand.CommandType = CommandType.Text
                                    da.SelectCommand.Parameters.Clear()
                                    ''  get fld value from document table 
                                    da.SelectCommand.CommandText = "select " & dtDF.Rows(0).Item(0).ToString & " from MMM_MST_doc where EID=" & EID & " and  Tid=" & docID
                                    Dim dtDR As New DataTable
                                    da.Fill(dtDR)
                                    If dtDR.Rows.Count <> 0 Then
                                        If dtDR.Rows(0).Item(0).ToString <> "" Then
                                            If dtDR.Rows(0).Item(0).ToString <> "" Then
                                                strFldQry &= " and ','+" & dtRRef.Rows(i).Item("docmapping") & " +',' like '%," & dtDR.Rows(0).Item(0).ToString & ",%'"
                                            End If
                                        End If
                                    End If
                                End If

                                '' ddllookupvalue added  by sunil on 04th October 14 - starts
                                da.SelectCommand.CommandType = CommandType.Text
                                da.SelectCommand.Parameters.Clear()
                                ' prev dis. on 28-sep for optimization by sunil  - DDLlookupValueSource
                                da.SelectCommand.CommandText = "select fieldmapping from MMM_MST_FIELDS where  Eid=" & EID & " and DocumentType='" & docType & "' and fieldtype='LookupDDL' and DDLlookupValueSource='" & dtRRef.Rows(i).Item("formname").ToString & "'"
                                Dim dtDFddl As New DataTable
                                da.Fill(dtDFddl)
                                If dtDFddl.Rows.Count > 0 Then   '' write in else of this to add def % in condition. 
                                    da.SelectCommand.CommandType = CommandType.Text
                                    da.SelectCommand.Parameters.Clear()
                                    ''  get fld value from document table 
                                    da.SelectCommand.CommandText = "select " & dtDFddl.Rows(0).Item(0).ToString & " from MMM_MST_doc where EID=" & EID & " and  Tid=" & docID
                                    Dim dtDR As New DataTable
                                    da.Fill(dtDR)
                                    If dtDR.Rows.Count <> 0 Then
                                        If dtDR.Rows(0).Item(0).ToString <> "" Then
                                            If dtDR.Rows(0).Item(0).ToString <> "" Then
                                                strFldQry &= " and ','+" & dtRRef.Rows(i).Item("docmapping") & " +',' like '%," & dtDR.Rows(0).Item(0).ToString & ",%'"
                                            End If
                                        End If
                                    End If
                                End If
                                '' ddllookupvalue added  by sunil on 04th October 14 - ends
                            Next
                            If Len(strFldQry) > 1 Then
                                da.SelectCommand.CommandType = CommandType.Text
                                da.SelectCommand.Parameters.Clear()
                                ''  get final rows from role assignment table 
                                da.SelectCommand.CommandText = strMainQry & strFldQry
                                Dim dtRU As New DataTable
                                da.Fill(dtRU)
                                Dim usrs As String = ""
                                If dtRU.Rows.Count = 1 Then
                                    nxtUser = dtRU.Rows(0).Item(0).ToString
                                ElseIf dtRU.Rows.Count > 1 Then
                                    '' new for queueing issue 01-April-14
                                    Dim mindoc As Integer = 999999
                                    Dim MinUserID As Integer = dtRU.Rows(0).Item(0).ToString

                                    For H As Integer = 0 To dtRU.Rows.Count - 1
                                        usrs = dtRU.Rows(H).Item(0).ToString()
                                        If Val(usrs) <> 0 Then
                                            da.SelectCommand.CommandType = CommandType.Text
                                            da.SelectCommand.CommandText = "select COUNT(userid) [loadcount] from MMM_DOC_DTL dt left outer join MMM_MST_DOC D on dt.tid=d.lasttid where d.EID=" & EID & " and d.DocumentType='" & docType & "' and dt.userid in (" & usrs & ")  and d.curstatus= '" & curAprStatus & "' and dt.tdate is null and dt.aprstatus is null  group by userid "
                                            da.SelectCommand.Parameters.Clear()
                                            If con.State <> ConnectionState.Open Then
                                                con.Open()
                                            End If
                                            Dim res As Integer = Val(Convert.ToString(da.SelectCommand.ExecuteScalar()))
                                            If Val(res) < mindoc Then
                                                mindoc = res
                                                MinUserID = usrs
                                            End If
                                        End If
                                    Next
                                    nxtUser = MinUserID
                                    '' new for queueing issue  ends 01-April-14

                                    '' prev b4 01-apr-14
                                    'For H As Integer = 0 To dtRU.Rows.Count - 1
                                    '    usrs &= dtRU.Rows(H).Item(0).ToString & ","
                                    'Next
                                    'If usrs.Length() > 0 Then
                                    '    usrs = Left(usrs, Len(usrs) - 1)
                                    'End If
                                    '' pass doctype and status for getting less loaded user from queue by sunil
                                    'da.SelectCommand.CommandType = CommandType.Text
                                    ''  da.SelectCommand.CommandText = "select top 1 userid  from (select COUNT(userid) [loadcount], userid from MMM_DOC_DTL dt left outer join MMM_MST_DOC D on dt.docid=d.tid where d.EID=" & EID & " and d.DocumentType='" & docType & "' and dt.userid in (" & usrs & ") and dt.tdate is null and dt.aprstatus is null  group by userid ) a order by loadcount asc"
                                    'da.SelectCommand.CommandText = "select top 1 userid  from (select COUNT(userid) [loadcount], userid from MMM_DOC_DTL dt left outer join MMM_MST_DOC D on dt.docid=d.tid where d.EID=" & EID & " and d.DocumentType='" & docType & "' and dt.userid in (" & usrs & ")  and d.curstatus= '" & curAprStatus & "' and dt.tdate is null and dt.aprstatus is null  group by userid ) a order by loadcount asc"
                                    'da.SelectCommand.Parameters.Clear()
                                    'If con.State <> ConnectionState.Open Then
                                    '    con.Open()
                                    'End If
                                    'Dim res As Integer = da.SelectCommand.ExecuteScalar()
                                    ' '' return user 
                                    'nxtUser = res
                                    'If nxtUser = 0 Then '' check if queuing not returned any user then take first user 
                                    '    nxtUser = dtRU.Rows(0).Item(0).ToString()
                                    'End If
                                    '' prev ends
                                End If
                            End If
                    End Select
                ElseIf dtRM.Rows(k).Item("type").ToString = "NEWROLE" Then
                    CurRoleName = dtRM.Rows(k).Item("Rolename").ToString
                    curAprStatus = dtRM.Rows(k).Item("aprstatus").ToString
                    '' FOR NEW role with document fields   by sunil 14_july
                    '' here on 15_july_14 at 7.57 pm
                    da.SelectCommand.CommandType = CommandType.Text
                    da.SelectCommand.Parameters.Clear()
                    da.SelectCommand.CommandText = "select formid, docmapping, FORMNAME from MMM_MST_FORMS where EID=" & EID & " and isRoleDef=1"
                    Dim dtRRef As New DataTable
                    da.Fill(dtRRef)
                    Dim strMainQry As String = ""
                    Dim strFldQry As String = ""
                    strMainQry = "select uid,rolename from MMM_Ref_Role_User where Eid=" & EID & " and ',' + documenttype + ','  like '%," & docType & ",%' and rolename='" & CurRoleName & "'"

                    For i As Integer = 0 To dtRRef.Rows.Count - 1
                        da.SelectCommand.CommandType = CommandType.Text
                        da.SelectCommand.Parameters.Clear()
                        da.SelectCommand.CommandText = "select fieldmapping from MMM_MST_FIELDS where  Eid=" & EID & " and DocumentType='" & docType & "' and dropdowntype='MASTER VALUED' and substring(dropdown,8,(charindex('-',dropdown,8)-8)) = '" & dtRRef.Rows(i).Item("formname").ToString & "'"
                        Dim dtDF As New DataTable
                        da.Fill(dtDF)
                        If dtDF.Rows.Count > 0 Then   '' write in else of this to add def % in condition. 
                            da.SelectCommand.CommandType = CommandType.Text
                            da.SelectCommand.Parameters.Clear()
                            ''  get fld value from document table 
                            da.SelectCommand.CommandText = "select " & dtDF.Rows(0).Item(0).ToString & " from MMM_MST_doc where EID=" & EID & " and  Tid=" & docID
                            Dim dtDR As New DataTable
                            da.Fill(dtDR)
                            If dtDR.Rows.Count <> 0 Then
                                If dtDR.Rows(0).Item(0).ToString <> "" Then
                                    If dtDR.Rows(0).Item(0).ToString <> "" Then
                                        strFldQry &= " and ','+" & dtRRef.Rows(i).Item("docmapping") & " +',' like '%," & dtDR.Rows(0).Item(0).ToString & ",%'"
                                    End If
                                End If
                            End If
                        End If

                        '' ddllookupvalue added  by sunil on 04th October 14 - starts
                        da.SelectCommand.CommandType = CommandType.Text
                        da.SelectCommand.Parameters.Clear()
                        ' prev dis. on 28-sep for optimization by sunil  - DDLlookupValueSource
                        da.SelectCommand.CommandText = "select fieldmapping from MMM_MST_FIELDS where  Eid=" & EID & " and DocumentType='" & docType & "' and fieldtype='LookupDDL' and DDLlookupValueSource='" & dtRRef.Rows(i).Item("formname").ToString & "'"
                        Dim dtDFddl As New DataTable
                        da.Fill(dtDFddl)
                        If dtDFddl.Rows.Count > 0 Then   '' write in else of this to add def % in condition. 
                            da.SelectCommand.CommandType = CommandType.Text
                            da.SelectCommand.Parameters.Clear()
                            ''  get fld value from document table 
                            da.SelectCommand.CommandText = "select " & dtDFddl.Rows(0).Item(0).ToString & " from MMM_MST_doc where EID=" & EID & " and  Tid=" & docID
                            Dim dtDR As New DataTable
                            da.Fill(dtDR)
                            If dtDR.Rows.Count <> 0 Then
                                If dtDR.Rows(0).Item(0).ToString <> "" Then
                                    If dtDR.Rows(0).Item(0).ToString <> "" Then
                                        strFldQry &= " and ','+" & dtRRef.Rows(i).Item("docmapping") & " +',' like '%," & dtDR.Rows(0).Item(0).ToString & ",%'"
                                    End If
                                End If
                            End If
                        End If
                        '' ddllookupvalue added  by sunil on 04th October 14 - ends
                    Next
                    If Len(strFldQry) > 1 Then
                        da.SelectCommand.CommandType = CommandType.Text
                        da.SelectCommand.Parameters.Clear()
                        ''  get final rows from role assignment table 
                        da.SelectCommand.CommandText = strMainQry & strFldQry
                        Dim dtRU As New DataTable
                        da.Fill(dtRU)
                        Dim usrs As String = ""



                        ''for gen. query of document flds in role based user
                        Dim dtFlds As New DataTable
                        da.SelectCommand.CommandText = "select * from MMM_MST_FIELDS where EID=" & EID & " and documenttype='" & docType & "' AND ISWORKFLOW=1"
                        da.Fill(dtFlds)

                        Dim strQry As String = "select rolename from MMM_MST_AuthMetrix  where EID=" & EID & " and doctype='" & docType & "' and ordering=" & dtRM.Rows(k).Item("ordering").ToString  'CurOrdering
                        For i As Integer = 0 To dtFlds.Rows.Count - 1
                            Dim dType As String = dtFlds.Rows(i).Item("datatype").ToString()
                            Dim fldMapping As String = dtFlds.Rows(i).Item("FieldMapping").ToString()
                            If dtDoc.Rows(0).Item(fldMapping).ToString().Length > 1 Then
                                Select Case dType.ToUpper()
                                    Case "TEXT"
                                        strQry &= " AND case " & fldMapping & " WHEN '*' Then '" & dtDoc.Rows(0).Item(fldMapping) & "' ELSE " & fldMapping & "  END  like '%" & dtDoc.Rows(0).Item(fldMapping) & "%' "
                                    Case "NUMERIC"
                                        strQry &= " and convert(numeric(15,0),PARSENAME(REPLACE(" & fldMapping & ", '-', '.'), 2)) < " & dtDoc.Rows(0).Item(fldMapping) & " and convert(numeric(15,0),PARSENAME(REPLACE(" & fldMapping & ", '-', '.'), 1)) > " & dtDoc.Rows(0).Item(fldMapping)
                                End Select
                            End If
                        Next
                        strQry &= " order by ordering"
                        Dim dtK As New DataTable
                        da.SelectCommand.CommandType = CommandType.Text
                        da.SelectCommand.Parameters.Clear()
                        da.SelectCommand.CommandText = strQry
                        da.Fill(dtK)

                        If dtK.Rows.Count > 0 And dtRU.Rows.Count > 0 Then
                            If dtK.Rows(0).Item("rolename").ToString.Trim().ToUpper() = dtRU.Rows(0).Item("rolename").ToString.Trim().ToUpper() Then
                                nxtUser = dtRU.Rows(0).Item("uid").ToString
                            End If
                        End If
                        ''for gen. query of document flds in role based user
                    End If
                ElseIf dtRM.Rows(k).Item("type").ToString = "USER" Then
                    Dim dtFlds As New DataTable
                    da.SelectCommand.CommandText = "select * from MMM_MST_FIELDS where EID=" & EID & " and documenttype='" & docType & "' AND ISWORKFLOW=1"
                    da.Fill(dtFlds)

                    ' Dim strQry As String = "select top 1 uid from MMM_MST_AuthMetrix A left outer join MMM_MST_WORKFLOW_STATUS S on A.EID=S.EID and S.statusname=a.aprStatus and S.Documenttype=A.doctype where A.EID=" & EID & " and doctype='" & docType & "' and A.ordering>" & CurOrdering
                    Dim strQry As String = "select rolename from MMM_MST_AuthMetrix  where EID=" & EID & " and doctype='" & docType & "' and ordering=" & dtRM.Rows(k).Item("ordering").ToString  'CurOrdering
                    For i As Integer = 0 To dtFlds.Rows.Count - 1
                        Dim dType As String = dtFlds.Rows(i).Item("datatype").ToString()
                        Dim fldMapping As String = dtFlds.Rows(i).Item("FieldMapping").ToString()
                        If dtDoc.Rows(0).Item(fldMapping).ToString().Length > 1 Then
                            Select Case dType.ToUpper()
                                Case "TEXT"
                                    strQry &= " AND case " & fldMapping & " WHEN '*' Then '" & dtDoc.Rows(0).Item(fldMapping) & "' ELSE " & fldMapping & "  END  like '%" & dtDoc.Rows(0).Item(fldMapping) & "%' "
                                Case "NUMERIC"
                                    strQry &= " and convert(numeric(15,0),PARSENAME(REPLACE(" & fldMapping & ", '-', '.'), 2)) < " & dtDoc.Rows(0).Item(fldMapping) & " and convert(numeric(15,0),PARSENAME(REPLACE(" & fldMapping & ", '-', '.'), 1)) > " & dtDoc.Rows(0).Item(fldMapping)
                            End Select
                        End If
                    Next
                    strQry &= " order by ordering"
                    Dim dtK As New DataTable
                    da.SelectCommand.CommandType = CommandType.Text
                    da.SelectCommand.Parameters.Clear()
                    da.SelectCommand.CommandText = strQry
                    da.Fill(dtK)
                    If dtK.Rows.Count <> 0 Then
                        nxtUser = dtK.Rows(0).Item(0).ToString
                    End If
                End If

                If (CheckSkipfeat = True) And (nxtUser = 0) Then
                    '' exit from func with bcoz skip is not allowed and user not found
                    sRetMsg = "NO SKIP" & ":" & docType
                    CheckSkipfeat = True
                    Exit For
                Else
                    If nxtUser <> 0 Then
                        da.SelectCommand.CommandText = "ApproveWorkFlow_RM"
                        da.SelectCommand.CommandType = CommandType.StoredProcedure
                        da.SelectCommand.Parameters.Clear()
                        da.SelectCommand.Parameters.AddWithValue("tid", docID)
                        da.SelectCommand.Parameters.AddWithValue("nUid", nxtUser)
                        da.SelectCommand.Parameters.AddWithValue("NxtStatus", dtRM.Rows(k).Item("aprstatus").ToString)
                        da.SelectCommand.Parameters.AddWithValue("nOrder", dtRM.Rows(k).Item("ordering").ToString)
                        da.SelectCommand.Parameters.AddWithValue("nSLA", dtRM.Rows(k).Item("SLA").ToString)
                        If Len(qry) > 1 Then
                            da.SelectCommand.Parameters.AddWithValue("qry", qry)
                        End If
                        If Auid <> 0 Then
                            da.SelectCommand.Parameters.AddWithValue("auid", Auid)
                        End If

                        Dim dtt As New DataTable
                        da.Fill(dtt)

                        sRetMsg = dtt.Rows(0).Item(0).ToString()
                        'If sRetMsg = "User not authorised" Then
                        'Return "You are not authorised to approve this document"
                        Exit For
                        ' End If
                    End If
                End If
            Next  '' K loop till user founds for a role type (end)

            If CheckSkipfeat = True Then
                'Commentted by ajeet for managing transactions
                'dtDoc.Dispose() : da.Dispose() : con.Close() : con.Dispose()
                dtDoc.Dispose() : da.Dispose()
                Return sRetMsg
            End If

            If nxtUser <> 0 Then
                'dtDoc.Dispose() : da.Dispose() : con.Close() : con.Dispose()
                dtDoc.Dispose() : da.Dispose()
                Return sRetMsg
            Else
                'Return "NO USERS IN AUTHMATRIX"
                da.SelectCommand.CommandText = "InsertDefaultMovement"
                da.SelectCommand.CommandType = CommandType.StoredProcedure
                da.SelectCommand.Parameters.Clear()
                da.SelectCommand.Parameters.AddWithValue("tid", docID)
                da.SelectCommand.Parameters.AddWithValue("what", "ARCHIVE")
                da.SelectCommand.Parameters.AddWithValue("qry", qry)
                If con.State <> ConnectionState.Open Then
                    con.Open()
                End If
                da.SelectCommand.ExecuteNonQuery()
                dtDoc.Dispose() : da.Dispose()
                Return "ARCHIVE:" & docType
            End If
            dtDoc.Dispose()

        Catch ex As Exception
            Throw
            'Finally  block Added By Ajeet
        Finally
            If Not da Is Nothing Then
                da.Dispose()
            End If
            'If Not con Is Nothing Then
            '    con.Close()
            '    con.Dispose()
            'End If
        End Try
    End Function

    'Public Sub WorkFlowEntry(DocID As Integer, DefaultUserID As Integer, NextStatus As String, NextOrder As Integer)
    '    Dim con = New SqlConnection(ConfigurationManager.ConnectionStrings("conStr").ConnectionString)
    '    Dim da As New SqlDataAdapter("", con)
    '    Try
    '    Catch ex As Exception
    '    End Try
    'End Sub

    Public Sub InsertError(Ex As Exception)
        Dim con = New SqlConnection(ConfigurationManager.ConnectionStrings("conStr").ConnectionString)
        Dim da As New SqlDataAdapter("", con)
        Dim sta As New StackTrace(Ex, True)
        Dim fr = sta.GetFrame(sta.FrameCount - 1)
        Dim frline As String = fr.GetFileLineNumber.ToString()
        Dim methodname = fr.GetMethod.ToString()
        da.SelectCommand.CommandText = "INSERT_ERRORLOG"
        da.SelectCommand.CommandType = CommandType.StoredProcedure
        da.SelectCommand.Parameters.Clear()
        da.SelectCommand.Parameters.AddWithValue("@ERRORMSG", Ex.Message & ": LineNo-" & frline & "MethodName" & methodname)
        da.SelectCommand.Parameters.AddWithValue("@ALERTNAME", "MissedDocumentCreation")
        da.SelectCommand.Parameters.AddWithValue("@EID", Eid)
        If con.State <> ConnectionState.Open Then
            con.Open()
        End If
        da.SelectCommand.ExecuteNonQuery()
        con.Close()
    End Sub

End Class
