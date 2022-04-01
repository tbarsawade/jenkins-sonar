Imports Microsoft.VisualBasic
Imports System
Imports System.Data.SqlClient
Imports System.Data

Public Class Exportdata

    Public Function Pushdata(ByVal docid As Integer, ByVal sdoctype As String, ByVal EID As Integer) As Integer
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
        Try
            Dim ds As New DataSet
            Dim insertqry As String = ""
            Dim Datafield As String = ""
            oda.SelectCommand.CommandText = "Select tdoctype from MMM_mst_exportmapping where sdoctype='" & sdoctype & "' and eid= " & EID & " group by tdoctype "
            oda.Fill(ds, "tdoctype")
            If ds.Tables("tdoctype").Rows.Count > 0 Then
                Dim isPushtoUser As String = ""
                Dim UserUID As String = ""
                Dim MastTID As String = ""
                For i As Integer = 0 To ds.Tables("tdoctype").Rows.Count - 1
                    Dim dtU As New DataTable
                    Dim dss As New DataSet()
                    oda.SelectCommand.CommandText = "Select * from MMM_mst_exportmapping where sdoctype='" & sdoctype & "' and eid= " & EID & " and tdoctype='" & ds.Tables("tdoctype").Rows(i).Item("tdoctype").ToString() & "' "
                    oda.Fill(dss, "Fields")
                    If dss.Tables("Fields").Rows(0).Item("tdoctype").ToString.ToUpper = "USER" Then
                        insertqry = "Insert into MMM_MST_USER(Eid,locationID,isAuth,modifydate,"
                        Datafield = " Select  EID,2072,100,getdate(),"
                    Else
                        insertqry = "Insert into MMM_MST_MASTER(DOCUMENTTYPE,Eid,createdby,"
                        Datafield = " Select  '" & dss.Tables("Fields").Rows(0).Item("tdoctype") & "',EID,oUID,"

                        oda.SelectCommand.CommandText = "Select isUserCreation from MMM_mst_forms where formname='" & ds.Tables("tdoctype").Rows(i).Item("tdoctype").ToString() & "' and eid='" & EID & "'"
                        oda.Fill(dtU)

                        If dtU.Rows.Count > 0 Then
                            isPushtoUser = dtU.Rows(0).Item(0).ToString()
                        End If
                    End If

                    For Each row As DataRow In dss.Tables("Fields").Rows
                        insertqry &= row.Item("tfldmapping").ToString() & ","
                        Datafield &= row.Item("sfldmapping").ToString() & ","
                    Next
                    insertqry = Left(insertqry, insertqry.Length - 1) & ")"
                    Datafield = Left(Datafield, Datafield.Length - 1)
                    Dim qry As String = insertqry & Datafield & " FROM MMM_MST_DOC where tid=" & docid & " ; select scope_identity() "
                    oda.SelectCommand.CommandText = qry
                    If con.State <> ConnectionState.Open Then
                        con.Open()
                    End If
                    Dim IST As Integer = oda.SelectCommand.ExecuteScalar()
                    If dss.Tables("Fields").Rows(0).Item("tdoctype").ToString.ToUpper = "USER" Then
                        If isPushtoUser = "1" Then
                            UserUID = IST
                        End If
                        Try
                            Dim DMS As New DMSUtil
                            DMS.notificationMail(IST, EID, "USER", "USER CREATED")
                        Catch ex As Exception
                        End Try
                    Else
                        ' If isPushtoUser = "1" Then
                        MastTID = IST
                        '  End If
                    End If
                    dss.Dispose()
                Next

                If isPushtoUser = "1" Then
                    Dim str As String = "update mmm_mst_user set extID='" & MastTID & "' where uid='" & UserUID & "' and eid='" & EID & "'"
                    oda.SelectCommand.CommandText = str
                    If con.State <> ConnectionState.Open Then
                        con.Open()
                    End If
                    Dim IST As Integer = oda.SelectCommand.ExecuteNonQuery()
                End If
                Return 1
            Else
                Return 0
            End If

            ds.Dispose()
        Catch ex As Exception
            Throw
        Finally
            If Not con Is Nothing Then
                con.Close()
                con.Dispose()
            End If
            If Not oda Is Nothing Then
                oda.Dispose()
            End If
        End Try
    End Function
    'This Code is added by Ajeet Kumar for the sake of transaction control Date:11-july-2014
    Public Function PushdataT(ByVal docid As Integer, ByVal sdoctype As String, ByVal EID As Integer, con As SqlConnection, tran As SqlTransaction) As Integer
        'Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        'Now Connection is in argument
        'Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
        oda.SelectCommand.Transaction = tran
        Try
            Dim ds As New DataSet
            Dim insertqry As String = ""
            Dim Datafield As String = ""
            oda.SelectCommand.CommandText = "Select tdoctype from MMM_mst_exportmapping where sdoctype='" & sdoctype & "' and eid= " & EID & " group by tdoctype "
            oda.Fill(ds, "tdoctype")
            If ds.Tables("tdoctype").Rows.Count > 0 Then
                Dim isPushtoUser As String = ""
                Dim userCreateCond As String = ""
                Dim UserUID As String = ""
                Dim MastTID As String = ""
                For i As Integer = 0 To ds.Tables("tdoctype").Rows.Count - 1
                    Dim dtU As New DataTable
                    Dim dss As New DataSet()
                    oda.SelectCommand.CommandText = "Select * from MMM_mst_exportmapping where sdoctype='" & sdoctype & "' and eid= " & EID & " and tdoctype='" & ds.Tables("tdoctype").Rows(i).Item("tdoctype").ToString() & "' "
                    oda.Fill(dss, "Fields")

                    'oda.SelectCommand.CommandText = "Select isUserCreation,isnull(ExMapUserCreateCond,'') ExMapUserCreateCond from MMM_mst_forms where formname='" & ds.Tables("tdoctype").Rows(i).Item("tdoctype").ToString() & "' and eid='" & EID & "'"
                    oda.SelectCommand.CommandText = "Select isUserCreation,isnull(ExMapUserCreateCond,'') ExMapUserCreateCond from MMM_mst_forms where formname='" & sdoctype & "' and eid='" & EID & "'"
                    oda.Fill(dtU)

                    If dtU.Rows.Count > 0 Then
                        isPushtoUser = dtU.Rows(0).Item(0).ToString()
                        userCreateCond = dtU.Rows(0).Item(1).ToString() ' condition
                    End If

                    ' If userCreateCond.Contains("=") Then

                    If dss.Tables("Fields").Rows(0).Item("tdoctype").ToString.ToUpper = "USER" Then
                        Dim objDC As New DataClass()
                        Dim CreatedBy As String = "0"
                        Dim objDT = objDC.ExecuteQryDT("select uid from MMM_MST_USER where eid='" & EID & "' and UserName IN ('System_User','Superuser') order by CreatedOn desc")
                        If objDT IsNot Nothing And objDT.Rows.Count > 0 Then
                            CreatedBy = objDT.Rows(0).Item(0).ToString()
                        End If
                        insertqry = "Insert into MMM_MST_USER(Eid,locationID,isAuth,modifydate,resetflag,ResetAccessString,CreatedBy,"
                        Datafield = " Select  EID,2072,100,getdate(),1,replace(NEWID(),'-',''),'" & CreatedBy & "',"
                    Else
                        insertqry = "Insert into MMM_MST_MASTER(DOCUMENTTYPE,Eid,createdby,"
                        Datafield = " Select  '" & dss.Tables("Fields").Rows(0).Item("tdoctype") & "',EID,oUID,"
                    End If

                    For Each row As DataRow In dss.Tables("Fields").Rows
                        insertqry &= row.Item("tfldmapping").ToString() & ","
                        Datafield &= row.Item("sfldmapping").ToString() & ","
                    Next
                    insertqry = Left(insertqry, insertqry.Length - 1) & ")"
                    Datafield = Left(Datafield, Datafield.Length - 1)

                    Dim qry As String = insertqry & Datafield & " FROM MMM_MST_DOC where tid=" & docid & IIf(userCreateCond.Contains("="), " and " & userCreateCond, "") & "; select scope_identity() "
                    oda.SelectCommand.CommandText = qry
                    If con.State <> ConnectionState.Open Then
                        con.Open()
                    End If
                    Dim IST As Integer = oda.SelectCommand.ExecuteScalar()
                    If dss.Tables("Fields").Rows(0).Item("tdoctype").ToString.ToUpper = "USER" Then
                        If isPushtoUser = "1" Then
                            UserUID = IST
                        End If
                        Try
                            Dim DMS As New DMSUtil
                            'DMS.notificationMail(IST, EID, "USER", "USER CREATED")
                            DMS.notificationMailT(IST, EID, "USER", "USER CREATED", con, tran)

                        Catch ex As Exception
                        End Try
                    Else
                        ' If isPushtoUser = "1" Then
                        MastTID = IST
                        '  End If
                    End If
                    dss.Dispose()
                    '  End If
                Next

                If isPushtoUser = "1" Then
                    Dim str As String = "update mmm_mst_user set extID='" & MastTID & "' where uid='" & UserUID & "' and eid='" & EID & "'"
                    oda.SelectCommand.CommandText = str
                    If con.State <> ConnectionState.Open Then
                        con.Open()
                    End If
                    Dim IST As Integer = oda.SelectCommand.ExecuteNonQuery()
                End If
                Return 1
            Else
                Return 0
            End If

            ds.Dispose()
        Catch ex As Exception
            Throw
        Finally
            'If Not con Is Nothing Then
            '    con.Close()
            '    con.Dispose()
            'End If
            If Not oda Is Nothing Then
                oda.Dispose()
            End If
        End Try
    End Function


    'last running by sunil
    'Public Function Pushdata(ByVal docid As Integer, ByVal sdoctype As String, ByVal EID As Integer) As Integer
    '    Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
    '    Dim con As SqlConnection = New SqlConnection(conStr)
    '    Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
    '    Try
    '        Dim ds As New DataSet
    '        Dim insertqry As String = ""
    '        Dim Datafield As String = ""
    '        oda.SelectCommand.CommandText = "Select tdoctype from MMM_mst_exportmapping where sdoctype='" & sdoctype & "' and eid= " & EID & " group by tdoctype "
    '        oda.Fill(ds, "tdoctype")
    '        If ds.Tables("tdoctype").Rows.Count > 0 Then
    '            For i As Integer = 0 To ds.Tables("tdoctype").Rows.Count - 1
    '                Dim dss As New DataSet()
    '                oda.SelectCommand.CommandText = "Select * from MMM_mst_exportmapping where sdoctype='" & sdoctype & "' and eid= " & EID & " and tdoctype='" & ds.Tables("tdoctype").Rows(i).Item("tdoctype").ToString() & "' "
    '                oda.Fill(dss, "Fields")
    '                If dss.Tables("Fields").Rows(0).Item("tdoctype").ToString.ToUpper = "USER" Then
    '                    insertqry = "Insert into MMM_MST_USER(Eid,locationID,isAuth,modifydate,"
    '                    Datafield = " Select  EID,2072,100,getdate(),"
    '                Else
    '                    insertqry = "Insert into MMM_MST_MASTER(DOCUMENTTYPE,Eid,createdby,"
    '                    Datafield = " Select  '" & dss.Tables("Fields").Rows(0).Item("tdoctype") & "',EID,oUID,"
    '                End If

    '                For Each row As DataRow In dss.Tables("Fields").Rows
    '                    insertqry &= row.Item("tfldmapping").ToString() & ","
    '                    Datafield &= row.Item("sfldmapping").ToString() & ","
    '                Next
    '                insertqry = Left(insertqry, insertqry.Length - 1) & ")"
    '                Datafield = Left(Datafield, Datafield.Length - 1)
    '                Dim qry As String = insertqry & Datafield & " FROM MMM_MST_DOC where tid=" & docid & " ; select scope_identity() "
    '                oda.SelectCommand.CommandText = qry
    '                If con.State <> ConnectionState.Open Then
    '                    con.Open()
    '                End If
    '                Dim IST As Integer = oda.SelectCommand.ExecuteScalar()
    '                If dss.Tables("Fields").Rows(0).Item("tdoctype").ToString.ToUpper = "USER" Then
    '                    Try
    '                        Dim DMS As New DMSUtil
    '                        DMS.notificationMail(IST, EID, "USER", "USER CREATED")
    '                    Catch ex As Exception
    '                    End Try
    '                End If
    '                dss.Dispose()
    '            Next
    '            Return 1
    '        Else
    '            Return 0
    '        End If

    '        ds.Dispose()
    '    Catch ex As Exception
    '        Throw
    '    Finally
    '        If Not con Is Nothing Then
    '            con.Close()
    '            con.Dispose()
    '        End If
    '        If Not oda Is Nothing Then
    '            oda.Dispose()
    '        End If
    '    End Try
    'End Function

    '' prev used by ajeet . now obselete 27-feb-14
    Public Function PushdataA(ByVal docid As Integer, ByVal sdoctype As String, ByVal EID As Integer) As Integer
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
        Dim ds As New DataSet
        oda.SelectCommand.CommandText = "Select * from MMM_mst_exportmapping where sdoctype='" & sdoctype & "' and eid= " & EID & ""
        oda.Fill(ds, "Fields")
        Dim insertqry As String = ""
        Dim Datafield As String = ""
        If ds.Tables("Fields").Rows.Count > 0 Then
            If ds.Tables("Fields").Rows(0).Item("tdoctype").ToString.ToUpper = "USER" Then
                insertqry = "Insert into MMM_MST_USER(Eid,locationID,isAuth,modifydate,"
                Datafield = " Select  EID,2072,100,getdate(),"
            Else

                insertqry = "Insert into MMM_MST_MASTER(DOCUMENTTYPE,Eid,createdby,"
                Datafield = " Select  '" & ds.Tables("Fields").Rows(0).Item("tdoctype") & "',EID,oUID,"
            End If
            For Each row As DataRow In ds.Tables("Fields").Rows
                insertqry &= row.Item("tfldmapping").ToString() & ","
                Datafield &= row.Item("sfldmapping").ToString() & ","
            Next
            insertqry = Left(insertqry, insertqry.Length - 1) & ")"
            Datafield = Left(Datafield, Datafield.Length - 1)
            Dim qry As String = insertqry & Datafield & " FROM MMM_MST_DOC where tid=" & docid & " ; select @@identity "
            oda.SelectCommand.CommandText = qry
            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            Dim IST As Integer = oda.SelectCommand.ExecuteScalar()
            Dim DMS As New DMSUtil
            DMS.notificationMail(IST, EID, "USER", "USER CREATED")
            Return 1
        Else
            Return 0
        End If
    End Function

    '' prev code 
    'Public Function Pushdata(ByVal docid As Integer, ByVal sdoctype As String) As Integer
    '    Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
    '    Dim con As SqlConnection = New SqlConnection(conStr)
    '    Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
    '    Dim ds As New DataSet
    '    oda.SelectCommand.CommandText = "Select * from MMM_mst_exportmapping where sdoctype='" & sdoctype & "' and eid= " & HttpContext.Current.Session("eid") & ""
    '    oda.Fill(ds, "Fields")
    '    Dim insertqry As String = ""
    '    Dim Datafield As String = ""
    '    If ds.Tables("Fields").Rows.Count > 0 Then
    '        If ds.Tables("Fields").Rows(0).Item("tdoctype").ToString.ToUpper = "USER" Then
    '            insertqry = "Insert into MMM_MST_USER(Eid,isAuth,modifydate,"
    '            Datafield = " Select  EID,100,getdate(),"
    '        Else

    '            insertqry = "Insert into MMM_MST_MASTER(DOCUMENTTYPE,Eid,createdby,"
    '            Datafield = " Select  '" & ds.Tables("Fields").Rows(0).Item("tdoctype") & "',EID,oUID,"
    '        End If
    '        For Each row As DataRow In ds.Tables("Fields").Rows
    '            insertqry &= row.Item("tfldmapping").ToString() & ","
    '            Datafield &= row.Item("sfldmapping").ToString() & ","
    '        Next
    '        insertqry = Left(insertqry, insertqry.Length - 1) & ")"
    '        Datafield = Left(Datafield, Datafield.Length - 1)
    '        Dim qry As String = insertqry & Datafield & " FROM MMM_MST_DOC where tid=" & docid & ""
    '        oda.SelectCommand.CommandText = qry
    '        If con.State <> ConnectionState.Open Then
    '            con.Open()
    '        End If
    '        oda.SelectCommand.ExecuteNonQuery()
    '        Return 1
    '    Else
    '        Return 0
    '    End If
    'End Function
End Class
