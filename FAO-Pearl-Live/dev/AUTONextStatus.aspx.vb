Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports System.Net
Partial Class AUTONextStatus
    Inherits System.Web.UI.Page

    Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
    Dim con As New SqlConnection(constr)


    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then

            Dim da As New SqlDataAdapter("", con)
            Dim ds As New DataSet
            Try
                da.SelectCommand.CommandText = "Select Distinct documenttype,autostatus from mmm_mst_workflow_status where eid=42 and isactive=1"
                da.Fill(ds, "data")
                If ds.Tables("data").Rows.Count > 0 Then
                    ddlField.DataSource = ds.Tables("data")
                    ddlField.DataTextField = "documenttype"
                    ddlField.DataValueField = "autostatus"
                    ddlField.DataBind()
                    ddlField.Items.Insert(0, "Select")
                End If
            Catch ex As Exception
            Finally
                If Not con Is Nothing Then
                    con.Close()
                    con.Dispose()
                End If
                If Not da Is Nothing Then
                    da.Dispose()
                End If

            End Try

        End If
    End Sub

    Protected Sub ddlField_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlField.SelectedIndexChanged
        Dim DA As New SqlDataAdapter("", con)
        Dim ds As New DataSet
        Try
            DA.SelectCommand.CommandText = "select  count(*) from  MMM_MST_DOC d where d.DocumentType='" & ddlField.SelectedItem.Text.Trim & "' and d.curstatus='" & ddlField.SelectedValue.Trim() & "' and d.fld4 is not null   and d.EID=42"
            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            Label1.Text = DA.SelectCommand.ExecuteScalar

        Catch ex As Exception
        Finally
            If Not con Is Nothing Then
                con.Close()
                con.Dispose()
            End If
            If Not DA Is Nothing Then
                DA.Dispose()
            End If

        End Try
    End Sub


    Public Sub SendautoExpiryMails()
        Try
            Dim con As SqlConnection = New SqlConnection(constr)
            Dim da As SqlDataAdapter = New SqlDataAdapter("", con)
            Dim b As Boolean = False
            Dim time1 As DateTime = Now.AddMinutes(-5).ToShortTimeString()
            Dim time2 As DateTime = Now.AddMinutes(+5).ToShortTimeString()
            Dim x As DateTime = (Convert.ToDateTime(Trim("06") & ":50:00 AM").ToShortTimeString)
            If x <= time2 And x >= time1 Then
                b = True
            End If
            Dim dds As Integer = 0
            b = True
            If b = True Then
                da.SelectCommand.CommandText = "select * from MMM_MST_WorkFlow_status where  isactive=1 and eid=42  order by eid,documenttype"
                Dim dt As New DataTable
                da.Fill(dt)
                If dt.Rows.Count > 0 Then
                    For i As Integer = 0 To dt.Rows.Count - 1
                        Dim curEid As Integer = dt.Rows(i).Item("EID").ToString()
                        Dim CurTID As Integer = dt.Rows(i).Item("tid")
                        Dim WFstatus As String = dt.Rows(i).Item("StatusName").ToString
                        Dim expirydate As String = dt.Rows(i).Item("expirydate").ToString
                        Dim afterhours As Integer = dt.Rows(i).Item("afterhours").ToString
                        Dim Documenttype As String = dt.Rows(i).Item("Documenttype").ToString
                        Dim autostatus As String = dt.Rows(i).Item("autostatus").ToString()
                        Dim autoaction As String = dt.Rows(i).Item("autoaction").ToString()
                        Dim nextstatus As String = dt.Rows(i).Item("autonextstatus").ToString()
                        Dim remarks As String = dt.Rows(i).Item("remarks").ToString()


                        da.SelectCommand.CommandType = CommandType.Text
                        da.SelectCommand.Parameters.Clear()
                        'da.SelectCommand.CommandText = "select tid [DOCID],curstatus," & expirydate & " [EXDT],(select  userid from MMM_DOC_DTL where tid=(select max(tid) from MMM_DOC_DTL where docid=d.tid ))[auid]  from  MMM_MST_DOC d where d.DocumentType='" & Documenttype & "' and d.curstatus='" & WFstatus & "' and d." & expirydate & " is not null and d.EID=" & curEid
                        da.SelectCommand.CommandText = "select  tid [DOCID],curstatus," & expirydate & " [EXDT],(select  userid from MMM_DOC_DTL where tid=d.lasttid)[auid]  from  MMM_MST_DOC d where d.DocumentType='" & Documenttype & "' and d.curstatus='" & autostatus & "' and d." & expirydate & " is not null  and d.EID=" & curEid
                        Dim dtDoc As New DataTable
                        da.Fill(dtDoc)
                        Dim cntc As Integer = 0
                        For j As Integer = 0 To dtDoc.Rows.Count - 1
                            Dim ExDate As DateTime
                            Dim docID As Integer = dtDoc.Rows(j).Item("DOCID").ToString
                            Dim Auid As Integer = dtDoc.Rows(j).Item("auid").ToString
                            Dim cstat As String = dtDoc.Rows(j).Item("curstatus").ToString


                            da.SelectCommand.CommandText = "Select emailid from mmm_mst_user where eid=" & curEid & " and uid=" & Auid & ""
                            If con.State <> ConnectionState.Open Then
                                con.Open()
                            End If
                            Dim useremail As String = ""
                            If Auid = 217 Then
                                useremail = "sachin.madaan@myndsol.com"
                            Else
                                useremail = da.SelectCommand.ExecuteScalar().ToString
                            End If
                            If Not IsDBNull(dtDoc.Rows(j).Item("EXDT")) Then
                                If Len(dtDoc.Rows(j).Item("EXDT")) > 5 Then
                                    Dim ArrDateVals() As String
                                    Dim clubVals As String = ""
                                    Dim A1 As Integer = 0
                                    Dim A2 As Integer = 0
                                    If InStr(dtDoc.Rows(j).Item("EXDT"), "/") > 0 Then
                                        ArrDateVals = Split(dtDoc.Rows(j).Item("EXDT").ToString, "/")
                                        clubVals = Trim(ArrDateVals(1)) & "/" & Trim(ArrDateVals(0)) & "/" & Trim(ArrDateVals(2))
                                    ElseIf InStr(dtDoc.Rows(j).Item("EXDT"), "-") > 0 Then
                                        ArrDateVals = Split(dtDoc.Rows(j).Item("EXDT"), "-")
                                        clubVals = Trim(ArrDateVals(1)) & "/" & Trim(ArrDateVals(0)) & "/" & Trim(ArrDateVals(2))
                                    End If
                                    ExDate = clubVals
                                    ExDate = ExDate.AddHours(afterhours + 24)
                                    If ExDate.Date <= DateTime.Now.Date Then
                                        dds = dds + 1
                                        Dim resu As String = ""
                                        If UCase(autoaction) = "APPROVE" Then
                                            While nextstatus <> resu.ToString
                                                resu = GetNextUserFromRolematrix(docID, curEid, CurTID, "", Auid, remarks.ToString())
                                                Dim Msg() As String = resu.Split(":")
                                                resu = Msg(0).ToString()
                                            End While

                                        ElseIf UCase(autoaction) = "REJECT" Then
                                            autoReject(docID, Auid, remarks.ToString())
                                        ElseIf UCase(autoaction) = "RECONSIDER" Then
                                            autoReconsider(docID, Auid, remarks.ToString())
                                        End If

                                    End If
                                    Label1.Text = dds

                                End If
                            End If
                        Next
                    Next
                End If
            End If

            da.Dispose()

        Catch ex As Exception
        Finally
            If Not con Is Nothing Then
                con.Close()
                con.Dispose()
            End If
        End Try
        '   Call UpdateLoginDB("Fatal Error in sendDocumentexpiry function")

    End Sub

    Public Function GetNextUserFromRolematrix(ByVal docID As Long, ByVal EID As Long, ByVal CUID As Integer, ByVal qry As String, ByVal Auid As Integer, ByVal remarks As String) As String
        Dim con As New SqlConnection(constr)
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
                                ' prev dis. on 28-sep for optimization by sunil 
                                'da.SelectCommand.CommandText = "select fieldmapping from MMM_MST_FIELDS where  Eid=" & EID & " and DocumentType='" & docType & "' and dropdowntype='MASTER VALUED' and substring(dropdown,8,(charindex('-',dropdown,8)-8)) in ( select formname from MMM_MST_FORMS where  Eid=" & EID & " and isRoleDef=1 and formid=" & dtRRef.Rows(i).Item("formid").ToString & ")"
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
                        ' da.SelectCommand.CommandText = "ApproveWorkFlow_RM"
                        da.SelectCommand.CommandText = "ApproveWorkFlow_RM_New"
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
                        da.SelectCommand.Parameters.AddWithValue("remarks", remarks.ToString())

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
                dtDoc.Dispose() : da.Dispose() : con.Close() : con.Dispose()
                Return sRetMsg
            End If

            If nxtUser <> 0 Then
                dtDoc.Dispose() : da.Dispose() : con.Close() : con.Dispose()
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
                dtDoc.Dispose() : da.Dispose() : con.Close() : con.Dispose()
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
            If Not con Is Nothing Then
                con.Close()
                con.Dispose()
            End If
        End Try
    End Function

    Public Function autoReject(ByVal docid As Integer, ByVal auid As Integer, ByVal remarks As String) As String
        Dim ob As New DynamicForm
        Dim con As SqlConnection = New SqlConnection(constr)
        Dim trans As SqlTransaction = Nothing
        con.Open()
        trans = con.BeginTransaction()
        Try
            ' Dim oda As SqlDataAdapter = New SqlDataAdapter("PermanentrejectDoc", con)  ' this was prev b4 new rolematrix imple. by sunil
            Dim oda As SqlDataAdapter = New SqlDataAdapter("PermanentRejectDoc_RM", con)
            oda.SelectCommand.Transaction = trans
            oda.SelectCommand.CommandType = CommandType.StoredProcedure
            oda.SelectCommand.Parameters.AddWithValue("TID", Request.QueryString("DOCID").ToString())
            oda.SelectCommand.Parameters.AddWithValue("remarks", remarks.ToString())
            oda.SelectCommand.Parameters.AddWithValue("qry", "")
            oda.SelectCommand.Parameters.AddWithValue("auid", Val(auid))

            Dim iSt As Integer = oda.SelectCommand.ExecuteScalar()
            oda.Dispose()

            trans.Commit()
        Catch ex As Exception
            trans.Rollback()
        Finally
            If Not con Is Nothing Then
                con.Dispose()
                trans.Dispose()
            End If
        End Try

    End Function

    Public Function autoReconsider(ByVal docid As Integer, ByVal auid As Integer, ByVal remarks As String) As String


        Dim con As SqlConnection = New SqlConnection(constr)
        Dim trans As SqlTransaction = Nothing
        con.Open()
        trans = con.BeginTransaction()
        ' this was prev b4 new rolematrix imple. by sunil

        Try
            Dim oda As SqlDataAdapter = New SqlDataAdapter("ReconsiderWorkFlow_RM", con)
            oda.SelectCommand.Transaction = trans
            oda.SelectCommand.CommandType = CommandType.StoredProcedure
            oda.SelectCommand.Parameters.AddWithValue("TID", Request.QueryString("DOCID").ToString())
            oda.SelectCommand.Parameters.AddWithValue("remarks", remarks.ToString())
            oda.SelectCommand.Parameters.AddWithValue("qry", "")
            oda.SelectCommand.Parameters.AddWithValue("auid", Val(auid))
            Dim iSt As Integer = oda.SelectCommand.ExecuteScalar()
            oda.Dispose()

            trans.Commit()

        Catch ex As Exception
            trans.Rollback()

        Finally
            If Not con Is Nothing Then
                con.Close()
                con.Dispose()
                trans.Dispose()
            End If
        End Try

    End Function


    Protected Sub btntest_Click(sender As Object, e As EventArgs) Handles btntest.Click
        If ddlField.SelectedItem.Text = "Select" Then
            lblMsg.Text = "Please select a valid DocumentType!!!"
            Exit Sub
        End If

        SendautoExpiryMails()
    End Sub
End Class
