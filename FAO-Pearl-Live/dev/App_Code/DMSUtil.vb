Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Data.SqlClient
Imports System.Web.UI.Adapters.ControlAdapter
Imports iTextSharp.text
Imports System.Drawing
Imports iTextSharp.text.pdf
Imports System.IO
Imports System.Threading
Imports System.Net.Mail
Imports System.Net
Imports System.Net.HttpWebRequest
Imports System.Net.HttpWebResponse
Imports System
Imports System.Collections.Specialized
Imports System.Text
Imports System.Net.Security
Imports System.IO.Stream
Imports System.Web.Hosting
Imports iTextSharp.text.html.simpleparser


Public Class DMSUtil

    Public Function GetNextUserFromRolematrix(ByVal docID As Long, ByVal EID As Long, ByVal CUID As Integer, ByVal qry As String, ByVal Auid As Integer, Optional ByRef UserRole As String = "") As String
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
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
            Dim CurStatus As String = dtDoc.Rows(0).Item("CurStatus").ToString


            If Trim(CurStatus.ToUpper) = "ARCHIVE" Then
                Return "Can not Approve, Reached to ARCHIVE"
                Exit Function
            End If

            '''' get all rows after current ordering 
            'prev b4 skip feature
            'da.SelectCommand.CommandText = "select * from MMM_MST_AuthMetrix where EID=" & EID & " and doctype='" & docType & "' and docnature='" & CurDocNature & "' AND ordering >" & CurOrdering & " order by ordering"
            da.SelectCommand.CommandText = "select am.*,wf.isallowskip from MMM_MST_AuthMetrix am inner join MMM_MST_WORKFLOW_STATUS wf on am.aprStatus=wf.StatusName and am.doctype=wf.Documenttype and am.eid=wf.eid  where am.EID=" & EID & " and am.doctype='" & docType & "' and am.docnature='" & CurDocNature & "' AND am.ordering >" & CurOrdering & " order by am.ordering"


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
                            ' AuthMetrixCustomQry(dtRM.Rows(k).Item("customqry").ToString, docid:=docID, con:=con, tran:=tran, statusName:=curAprStatus, roleName:=CurRoleName)
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
                                ' prev dis. on 28-sep for optimization by sunil  - DDLlookupValueSource
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
                                                ' strFldQry &= " and ','+" & dtRRef.Rows(i).Item("docmapping") & " +',' like '%," & dtDR.Rows(0).Item(0).ToString & ",%'"
                                                strFldQry &= " and (','+" & dtRRef.Rows(i).Item("docmapping") & " +',' like '%," & dtDR.Rows(0).Item(0).ToString & ",%' or ltrim(" & dtRRef.Rows(i).Item("docmapping") & ")='*' )"
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
                                                ' strFldQry &= " and ','+" & dtRRef.Rows(i).Item("docmapping") & " +',' like '%," & dtDR.Rows(0).Item(0).ToString & ",%'"
                                                strFldQry &= " and (','+" & dtRRef.Rows(i).Item("docmapping") & " +',' like '%," & dtDR.Rows(0).Item(0).ToString & ",%' or ltrim(" & dtRRef.Rows(i).Item("docmapping") & ")='*' )"
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
                                            da.SelectCommand.CommandText = "select COUNT(userid) [loadcount] from MMM_DOC_DTL dt  with (nolock) left outer join MMM_MST_DOC D  with (nolock) on dt.tid=d.lasttid where d.EID=" & EID & " and d.DocumentType='" & docType & "' and dt.userid in (" & usrs & ")  and d.curstatus= '" & curAprStatus & "' and dt.tdate is null and dt.aprstatus is null  group by userid "
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
                    Dim dtTmp As New DataTable
                    Select Case CurRoleName
                        Case "#SELF"
                            'AuthMetrixCustomQry(dtRM.Rows(k).Item("customqry").ToString, docid:=docID)
                            Dim dtFlds As New DataTable
                            da.SelectCommand.CommandText = "select * from MMM_MST_FIELDS where EID=" & EID & " and documenttype='" & docType & "' AND ISWORKFLOW=1"
                            da.Fill(dtFlds)

                            ' Dim strQry As String = "select top 1 uid from MMM_MST_AuthMetrix A left outer join MMM_MST_WORKFLOW_STATUS S on A.EID=S.EID and S.statusname=a.aprStatus and S.Documenttype=A.doctype where A.EID=" & EID & " and doctype='" & docType & "' and A.ordering>" & CurOrdering
                            Dim strQry As String = "select rolename from MMM_MST_AuthMetrix  where EID=" & EID & " and doctype='" & docType & "' and ordering=" & dtRM.Rows(k).Item("ordering").ToString  'CurOrdering
                            For i As Integer = 0 To dtFlds.Rows.Count - 1
                                Dim dType As String = dtFlds.Rows(i).Item("datatype").ToString()
                                Dim fldMapping As String = dtFlds.Rows(i).Item("FieldMapping").ToString()
                                If dtDoc.Rows(0).Item(fldMapping).ToString().Length > 0 Then
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
                            If dtK.Rows.Count = 1 Then
                                nxtUser = Creator
                                ' nxtUser = dtK.Rows(0).Item(0).ToString
                            End If
                        Case "#CURRENTUSER"

                            Dim dtFlds As New DataTable
                            da.SelectCommand.CommandText = "select * from MMM_MST_FIELDS where EID=" & EID & " and documenttype='" & docType & "' AND ISWORKFLOW=1"
                            da.Fill(dtFlds)

                            ' Dim strQry As String = "select top 1 uid from MMM_MST_AuthMetrix A left outer join MMM_MST_WORKFLOW_STATUS S on A.EID=S.EID and S.statusname=a.aprStatus and S.Documenttype=A.doctype where A.EID=" & EID & " and doctype='" & docType & "' and A.ordering>" & CurOrdering
                            Dim strQry As String = "select rolename from MMM_MST_AuthMetrix  where EID=" & EID & " and doctype='" & docType & "' and ordering=" & dtRM.Rows(k).Item("ordering").ToString  'CurOrdering
                            For i As Integer = 0 To dtFlds.Rows.Count - 1
                                Dim dType As String = dtFlds.Rows(i).Item("datatype").ToString()
                                Dim fldMapping As String = dtFlds.Rows(i).Item("FieldMapping").ToString()
                                If dtDoc.Rows(0).Item(fldMapping).ToString().Length > 0 Then
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
                            If dtK.Rows.Count = 1 Then
                                nxtUser = CurrentUser
                            End If

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
                            Dim dtFlds As New DataTable
                            Dim strQry As String = "select rolename from MMM_MST_AuthMetrix  where EID=" & EID & " and doctype='" & docType & "' and ordering=" & dtRM.Rows(k).Item("ordering").ToString  'CurOrdering
                            For i As Integer = 0 To dtFlds.Rows.Count - 1
                                Dim dType As String = dtFlds.Rows(i).Item("datatype").ToString()
                                Dim fldMapping As String = dtFlds.Rows(i).Item("FieldMapping").ToString()
                                If dtDoc.Rows(0).Item(fldMapping).ToString().Length > 0 Then
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
                            Dim dtFlds As New DataTable
                            Dim strQry As String = "select rolename from MMM_MST_AuthMetrix  where EID=" & EID & " and doctype='" & docType & "' and ordering=" & dtRM.Rows(k).Item("ordering").ToString  'CurOrdering
                            For i As Integer = 0 To dtFlds.Rows.Count - 1
                                Dim dType As String = dtFlds.Rows(i).Item("datatype").ToString()
                                Dim fldMapping As String = dtFlds.Rows(i).Item("FieldMapping").ToString()
                                If dtDoc.Rows(0).Item(fldMapping).ToString().Length > 0 Then
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


                            If tmpUser <> 0 And nxtUser <> 0 Then
                                nxtUser = tmpUser
                            End If
                        Case "#USER"
                            'AuthMetrixCustomQry(dtRM.Rows(k).Item("customqry").ToString, docid:=docID)
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

                            ''for gen. query of document flds in role based user
                            Dim dtFlds As New DataTable
                            da.SelectCommand.CommandText = "select * from MMM_MST_FIELDS with(nolock) where EID=" & EID & " and documenttype='" & docType & "' AND ISWORKFLOW=1"
                            da.Fill(dtFlds)

                            Dim strQry As String = ""
                            strQry = "select rolename from MMM_MST_AuthMetrix with(nolock) where docnature='" & CurDocNature & "' and EID=" & EID & " and doctype='" & docType & "' and ordering=" & dtRM.Rows(k).Item("ordering").ToString  'CurOrdering
                            For i As Integer = 0 To dtFlds.Rows.Count - 1
                                Dim dType As String = dtFlds.Rows(i).Item("datatype").ToString()
                                Dim fldMapping As String = dtFlds.Rows(i).Item("FieldMapping").ToString()
                                If dtDoc.Rows(0).Item(fldMapping).ToString().Length > 0 Then
                                    Select Case dType.ToUpper()
                                        Case "TEXT"
                                            strQry &= " AND case " & fldMapping & " WHEN '*' Then '" & dtDoc.Rows(0).Item(fldMapping) & "' ELSE " & fldMapping & "  END  like '%" & dtDoc.Rows(0).Item(fldMapping) & "%' "
                                        Case "NUMERIC"
                                            strQry &= " and convert(numeric(15,0),PARSENAME(REPLACE(" & fldMapping & ", '-', '.'), 2)) <= " & dtDoc.Rows(0).Item(fldMapping) & " and convert(numeric(15,0),PARSENAME(REPLACE(" & fldMapping & ", '-', '.'), 1)) >= " & dtDoc.Rows(0).Item(fldMapping)
                                    End Select
                                End If
                            Next
                            strQry &= " order by ordering"
                            Dim dtK As New DataTable
                            da.SelectCommand.CommandType = CommandType.Text
                            da.SelectCommand.Parameters.Clear()
                            da.SelectCommand.CommandText = strQry
                            da.Fill(dtK)

                            If Not (dtK.Rows.Count > 0 And nxtUser > 0) Then
                                nxtUser = 0
                                'objDC.TranExecuteQryDT(Qry:="insert into GetNextuserRoleMatrixHistory (Query,Curstatus,EID,uid,CurrentOrdering,documenttype)values('" & strQry.ToString().Replace("'", "''") & "','" & CurStatus & "'," & EID & "," & CUID & "," & CurOrdering & ",'" & docType & "')", con:=con, tran:=tran)
                            End If

                        Case Else
                            '' any other role 
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
                                                ' strFldQry &= " and ','+" & dtRRef.Rows(i).Item("docmapping") & " +',' like '%," & dtDR.Rows(0).Item(0).ToString & ",%'"
                                                strFldQry &= " and (','+" & dtRRef.Rows(i).Item("docmapping") & " +',' like '%," & dtDR.Rows(0).Item(0).ToString & ",%' or ltrim(" & dtRRef.Rows(i).Item("docmapping") & ")='*' )"
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
                                                ' strFldQry &= " and ','+" & dtRRef.Rows(i).Item("docmapping") & " +',' like '%," & dtDR.Rows(0).Item(0).ToString & ",%'"
                                                strFldQry &= " and (','+" & dtRRef.Rows(i).Item("docmapping") & " +',' like '%," & dtDR.Rows(0).Item(0).ToString & ",%' or ltrim(" & dtRRef.Rows(i).Item("docmapping") & ")='*' )"
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
                                            da.SelectCommand.CommandText = "select COUNT(userid) [loadcount] from MMM_DOC_DTL dt  with (nolock) left outer join MMM_MST_DOC D  with (nolock) on dt.tid=d.lasttid where d.EID=" & EID & " and d.DocumentType='" & docType & "' and dt.userid in (" & usrs & ")  and d.curstatus= '" & curAprStatus & "' and dt.tdate is null and dt.aprstatus is null  group by userid "

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
                                End If

                                ''for gen. query of document flds in role based user
                                Dim dtFlds As New DataTable
                                da.SelectCommand.CommandText = "select * from MMM_MST_FIELDS where EID=" & EID & " and documenttype='" & docType & "' AND ISWORKFLOW=1"
                                da.Fill(dtFlds)

                                Dim strQry As String = "select rolename from MMM_MST_AuthMetrix  where EID=" & EID & " and doctype='" & docType & "' and ordering=" & dtRM.Rows(k).Item("ordering").ToString  'CurOrdering
                                For i As Integer = 0 To dtFlds.Rows.Count - 1
                                    Dim dType As String = dtFlds.Rows(i).Item("datatype").ToString()
                                    Dim fldMapping As String = dtFlds.Rows(i).Item("FieldMapping").ToString()
                                    If dtDoc.Rows(0).Item(fldMapping).ToString().Length > 0 Then
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
                                        If dtRU.Rows.Count > 1 Then
                                            '''' NEXT USER WILL BE FROM QUEUING WHICH HAVE MINIMUM DOCUMENT  (MINUSERID)
                                        Else
                                            nxtUser = dtRU.Rows(0).Item("uid").ToString ' IF ONLY ONE USER EXIST  
                                        End If

                                    End If
                                Else
                                    nxtUser = 0
                                End If
                                ''for gen. query of document flds in role based user
                            End If
                    End Select
                ElseIf dtRM.Rows(k).Item("type").ToString = "USER" Then
                    Dim dtFlds As New DataTable
                    da.SelectCommand.CommandText = "select * from MMM_MST_FIELDS where EID=" & EID & " and documenttype='" & docType & "' AND ISWORKFLOW=1"
                    da.Fill(dtFlds)

                    ' Dim strQry As String = "select top 1 uid from MMM_MST_AuthMetrix A left outer join MMM_MST_WORKFLOW_STATUS S on A.EID=S.EID and S.statusname=a.aprStatus and S.Documenttype=A.doctype where A.EID=" & EID & " and doctype='" & docType & "' and A.ordering>" & CurOrdering
                    Dim strQry As String = "select rolename from MMM_MST_AuthMetrix  where EID=" & EID & " and doctype='" & docType & "' and ordering=" & dtRM.Rows(k).Item("ordering").ToString  'CurOrdering
                    For i As Integer = 0 To dtFlds.Rows.Count - 1
                        Dim dType As String = dtFlds.Rows(i).Item("datatype").ToString()
                        Dim fldMapping As String = dtFlds.Rows(i).Item("FieldMapping").ToString()
                        If dtDoc.Rows(0).Item(fldMapping).ToString().Length > 0 Then
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
                        If CurStatus = dtRM.Rows(k).Item("aprstatus").ToString() Then
                            sRetMsg = "Current and Next Status cannot be same"
                            Exit For
                        End If
                        da.SelectCommand.CommandText = "ApproveWorkFlow_RM_with_Isauth_2"
                        da.SelectCommand.CommandType = CommandType.StoredProcedure
                        da.SelectCommand.Parameters.Clear()
                        da.SelectCommand.Parameters.AddWithValue("tid", docID)
                        da.SelectCommand.Parameters.AddWithValue("nUid", nxtUser)
                        da.SelectCommand.Parameters.AddWithValue("NxtStatus", dtRM.Rows(k).Item("aprstatus").ToString)
                        da.SelectCommand.Parameters.AddWithValue("nOrder", dtRM.Rows(k).Item("ordering").ToString)
                        da.SelectCommand.Parameters.AddWithValue("nSLA", dtRM.Rows(k).Item("SLA").ToString)
                        da.SelectCommand.Parameters.AddWithValue("UserRole", UserRole)
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
                dtDoc.Dispose() : da.Dispose() : con.Close() : con.Dispose()
                Return sRetMsg
            End If

            If nxtUser <> 0 Then
                dtDoc.Dispose() : da.Dispose() : con.Close() : con.Dispose()
                Return sRetMsg
            Else
                'Return "NO USERS IN AUTHMATRIX"
                da.SelectCommand.CommandText = "InsertDefaultMovement_with_Isauth_2"
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

    Public Function CallM1Service(ByVal con As SqlConnection, ByVal tran As SqlTransaction, ByVal docid As Integer, ByVal documenttype As String, ByVal eid As Integer) As String
        Dim objDc As New DataClass()
        Dim objDT As New DataTable()
        Dim sb As New StringBuilder()
        Dim strResult As String = "FAIL"
        Try


            sb.Append("param={")
            objDT = objDc.TranExecuteQryDT("select * from mmm_mst_fields with(nolock) where documenttype='" & documenttype & "' and eid=" & eid & " and (M1Outward is not null and M1Outward<>'')", con, tran)
            Dim dynQry As New StringBuilder()
            dynQry.Append("Select ")
            Dim arlist As New ArrayList()
            For Each dr As DataRow In objDT.Rows
                Select Case dr("DropDownType").ToString().ToUpper
                    Case "MASTER VALUED"
                        arlist.Add("dms.udf_split('" & dr("dropdown") & "'," & dr("fieldmapping") & ") as [" & dr("M1Outward") & "]")
                    Case Else
                        arlist.Add("isnull(" & dr("fieldmapping") & ",'') as [" & dr("M1Outward") & "]")
                End Select
            Next
            If arlist.Count > 0 Then
                Dim qry As String = dynQry.ToString() & (String.Join(", ", arlist.ToArray())).ToString() & " from mmm_mst_doc where tid=" & docid
                objDT = objDc.TranExecuteQryDT(qry, con, tran)
                GetString(objDT, sb)
                Dim tranid As Int32 = objDc.ExecuteQryScaller("insert into M1Discoyunting_ServiceLog (docid,ServiceRequest,CreatedDate) values(" & docid & ",'" & sb.ToString() & "',getdate());select @@identity")
                Dim responseService As String = WSPost(sb.ToString())
                objDc.ExecuteQryScaller("Update  M1Discoyunting_ServiceLog set serviceResoponse='" & responseService & "' where tid=" & tranid)
                'Dim Str As String = "param={""Payment_Date_of_Buyer"": ""03/10/2017"",""Underlying_Commodity_Description"": ""Some Services"",""Buyer_Name"": ""mynd solution"",""PV_Sending_date"":""03/10/2017"",""Invoices"": [{""Invoice_Doc_No"": ""57124"",""PV_Unique_Ref_No"": ""D8403833"",""Invoice_NO"": ""INV34343"",""Invoice_Amount"": ""50000"",""GRN_SRN_Number"":""8699"",""GRN_SRN_Date"": ""03/10/2017"",""Invoice_Date"": ""03/10/2017""},{""Invoice_Doc_No"": ""7497"",""PV_Unique_Ref_No"": ""PV_INV_69"",""Invoice_NO"": ""INV90670"",""Invoice_Amount"": ""79999"",""GRN_SRN_Number"":""8462"",""GRN_SRN_Date"": ""03/10/2017"",""Invoice_Date"": ""03/10/2017""}],""Supplier_Name"": ""flipkart"",""Payment_Due_Date_for_supplier"": ""03/10/2017"",""Supplier_Code"": ""FK1009"",""PV_Factoring_Unit_No"": ""9823423"",""Underlying_Commodity_Type"": ""Services""}"
                'Dim Str As String = "param={""Payment_Date_of_Buyer"":""03/10/2017"",""Underlying_Commodity_Description"":""AMC Others"",""Buyer_Name"":""ABC Infosystems Ltd."",""PV_Sending_date"":"""",""GRN_SRN_Date"":"""",""Invoices"":[{""Invoice_Doc_No"":"""",""PV_Unique_Ref_No"":""D8403833"",""Invoice_NO"":""123456789"",""Invoice_Amount"":""0"",""GRN_SRN_Number"":"""",""Invoice_Date"":""24/07/17"" }],""Supplier_Name"":""flipkart"",""Payment_Due_Date_for_supplier"":"""",""Supplier_Code"":""FK1009"",""PV_Factoring_Unit_No"":"""",""Underlying_Commodity_Type"":""MATERIAL""}"
                'WSPost(Str.ToString())

                'Add condition for error
                If responseService.ToString().Contains("errorCode") Then
                    tran.Rollback()
                    Dim json = Newtonsoft.Json.JsonConvert.DeserializeObject(responseService.ToString())
                    strResult = json("error")("errorDescription")
                    Return strResult
                Else
                    strResult = "SUCCESS"
                    Return strResult
                End If

                Return strResult
            End If
        Catch ex As Exception
            Return strResult
        End Try
    End Function

    Public Function CallM2Service(ByVal con As SqlConnection, ByVal tran As SqlTransaction, ByVal docid As Integer, ByVal documenttype As String, ByVal eid As Integer) As String
        Dim objDc As New DataClass()
        Dim objDT As New DataTable()
        Dim sb As New StringBuilder()
        Dim strResult As String = "FAIL"
        Try


            sb.Append("param={")
            objDT = objDc.TranExecuteQryDT("select * from mmm_mst_fields with(nolock) where documenttype='" & documenttype & "' and eid=" & eid & " and (M1Outward is not null and M1Outward<>'')", con, tran)
            Dim dynQry As New StringBuilder()
            dynQry.Append("Select ")
            Dim arlist As New ArrayList()
            For Each dr As DataRow In objDT.Rows
                Select Case dr("DropDownType").ToString().ToUpper
                    Case "MASTER VALUED"
                        arlist.Add("dms.udf_split('" & dr("dropdown") & "'," & dr("fieldmapping") & ") as [" & dr("M1Outward") & "]")
                    Case Else
                        arlist.Add("isnull(" & dr("fieldmapping") & ",'') as [" & dr("M1Outward") & "]")
                End Select
            Next
            If arlist.Count > 0 Then
                Dim qry As String = dynQry.ToString() & (String.Join(", ", arlist.ToArray())).ToString() & " from mmm_mst_doc where tid=" & docid
                objDT = objDc.TranExecuteQryDT(qry, con, tran)
                GetString(objDT, sb)
                Dim tranid As Int32 = objDc.ExecuteQryScaller("insert into M1Discoyunting_ServiceLog (docid,ServiceRequest,CreatedDate) values(" & docid & ",'" & sb.ToString() & "',getdate());select @@identity")
                Dim responseService As String = WSPost2(sb.ToString())
                objDc.ExecuteQryScaller("Update  M1Discoyunting_ServiceLog set serviceResoponse='" & responseService & "' where tid=" & tranid)
                'Dim Str As String = "param={""Payment_Date_of_Buyer"": ""03/10/2017"",""Underlying_Commodity_Description"": ""Some Services"",""Buyer_Name"": ""mynd solution"",""PV_Sending_date"":""03/10/2017"",""Invoices"": [{""Invoice_Doc_No"": ""57124"",""PV_Unique_Ref_No"": ""D8403833"",""Invoice_NO"": ""INV34343"",""Invoice_Amount"": ""50000"",""GRN_SRN_Number"":""8699"",""GRN_SRN_Date"": ""03/10/2017"",""Invoice_Date"": ""03/10/2017""},{""Invoice_Doc_No"": ""7497"",""PV_Unique_Ref_No"": ""PV_INV_69"",""Invoice_NO"": ""INV90670"",""Invoice_Amount"": ""79999"",""GRN_SRN_Number"":""8462"",""GRN_SRN_Date"": ""03/10/2017"",""Invoice_Date"": ""03/10/2017""}],""Supplier_Name"": ""flipkart"",""Payment_Due_Date_for_supplier"": ""03/10/2017"",""Supplier_Code"": ""FK1009"",""PV_Factoring_Unit_No"": ""9823423"",""Underlying_Commodity_Type"": ""Services""}"
                'Dim Str As String = "param={""Payment_Date_of_Buyer"":""03/10/2017"",""Underlying_Commodity_Description"":""AMC Others"",""Buyer_Name"":""ABC Infosystems Ltd."",""PV_Sending_date"":"""",""GRN_SRN_Date"":"""",""Invoices"":[{""Invoice_Doc_No"":"""",""PV_Unique_Ref_No"":""D8403833"",""Invoice_NO"":""123456789"",""Invoice_Amount"":""0"",""GRN_SRN_Number"":"""",""Invoice_Date"":""24/07/17"" }],""Supplier_Name"":""flipkart"",""Payment_Due_Date_for_supplier"":"""",""Supplier_Code"":""FK1009"",""PV_Factoring_Unit_No"":"""",""Underlying_Commodity_Type"":""MATERIAL""}"
                'WSPost(Str.ToString())
                'Add condition for error
                If responseService.ToString().Contains("errorCode") Then
                    tran.Rollback()
                    Dim json = Newtonsoft.Json.JsonConvert.DeserializeObject(responseService.ToString())
                    strResult = json("error")("errorDescription")
                    Return strResult
                Else
                    strResult = "SUCCESS"
                    Return strResult
                End If
                Return strResult
            End If
        Catch ex As Exception
            Return strResult
        End Try
    End Function


    Public Function GetNextUserFromRolematrixT(ByVal docID As Long, ByVal EID As Long, ByVal CUID As Integer, ByVal qry As String, ByVal Auid As Integer, ByVal con As SqlConnection, ByVal tran As SqlTransaction, Optional ByRef UserRole As String = "") As String

        Dim da As New SqlDataAdapter("select d.*, dt.ordering,dt.userid from MMM_MST_DOC D with(nolock)  left outer join MMM_DOC_DTL   dt with(nolock) on d.LastTID=dt.tid where EID=" & EID & " and d.tid=" & docID, con)
        'try Catch Block Added by Ajeet 
        Try
            Dim dtDoc As New DataTable
            da.SelectCommand.Transaction = tran
            da.Fill(dtDoc)

            Dim docType As String = dtDoc.Rows(0).Item("documenttype").ToString
            Dim CurOrdering As Integer = dtDoc.Rows(0).Item("ordering").ToString
            Dim Creator As Integer = dtDoc.Rows(0).Item("ouid").ToString
            Dim CurDocNature As String = dtDoc.Rows(0).Item("CurdocNature").ToString
            Dim CurrentUser As Integer = dtDoc.Rows(0).Item("userid").ToString
            Dim CurStatus As String = dtDoc.Rows(0).Item("CurStatus").ToString
            Dim objDC As New DataClass()

            If Trim(CurStatus.ToUpper) = "ARCHIVE" Then
                Return "Can not Approve, Reached to ARCHIVE"
                Exit Function
            End If

            '' '''' ******* start  new code for reconsidered docs to sent to users who did reconsideration (skip middle stages) by sp on 06_july_18
            Dim sRetMsg As String = ""
            'Dim dtRC As New DataTable
            ' ''wf.isallowskip,wf.IsSend_Recon_User,wf.IsSend_Recon_User_Cond_Field
            'If CurStatus = "UPLOADED" Then
            '    da.SelectCommand.CommandText = "select IsSend_Recon_User_Cond_Field from MMM_MST_WORKFLOW_STATUS_config with(nolock) where EID=" & EID & " and Documenttype='" & docType & "' and statusname='" & CurStatus & "' and isnull(IsSend_Recon_User,0)=1"
            'Else
            '    da.SelectCommand.CommandText = "select IsSend_Recon_User_Cond_Field from MMM_MST_WORKFLOW_STATUS with(nolock) where EID=" & EID & " and Documenttype='" & docType & "' and statusname='" & CurStatus & "' and isnull(IsSend_Recon_User,0)=1"
            'End If
            'da.Fill(dtRC)
            ' If dtRC.Rows.Count <> 0 Then
            da.SelectCommand.Parameters.Clear()
            da.SelectCommand.CommandText = "Approve_ReconsideredTo_orig_user"
            da.SelectCommand.CommandType = CommandType.StoredProcedure
            da.SelectCommand.Parameters.AddWithValue("tid", docID)
            Dim dtRecon As New DataTable
            da.Fill(dtRecon)
            sRetMsg = dtRecon.Rows(0).Item(0).ToString()

            If sRetMsg = "SUCCESS" Then
                Return sRetMsg
                Exit Function
            End If
            '  End If
            '''' '''' *******Ends  new code for reconsidered docs to sent to users who did reconsideration (skip middle stages) by sp on 06_july_18

            '''' get all rows after current ordering 
            'prev b4 skip feature
            'da.SelectCommand.CommandText = "select * from MMM_MST_AuthMetrix where EID=" & EID & " and doctype='" & docType & "' and docnature='" & CurDocNature & "' AND ordering >" & CurOrdering & " order by ordering"

            da.SelectCommand.CommandType = CommandType.Text
            da.SelectCommand.Parameters.Clear()
            da.SelectCommand.CommandText = "select am.*,wf.isallowskip,wf.IsSend_Recon_User,wf.IsSend_Recon_User_Cond_Field from MMM_MST_AuthMetrix am  with(nolock)  inner join MMM_MST_WORKFLOW_STATUS   wf  with(nolock) on am.aprStatus=wf.StatusName and am.doctype=wf.Documenttype and am.eid=wf.eid  where am.EID=" & EID & " and am.doctype='" & docType & "' and am.docnature='" & CurDocNature & "' AND am.ordering >" & CurOrdering & " order by am.ordering"

            Dim dtRM As New DataTable
            da.Fill(dtRM)

            Dim FoundUsers As Boolean = False
            Dim CurRoleName As String = ""
            Dim curAprStatus As String = ""
            Dim nxtUser As Integer
            Dim AllowSkip As Integer = 0
            Dim CheckSkipfeat As Boolean = False
            nxtUser = 0 '' intialize with zero 
            Dim strQry As String = ""
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
                'Write code for M1 to call webservice and supplied vendor portal data code start here Mayank

                '  If dtRM.Rows(k).Item("Initiate_M1EX").ToString = 1 Then
                '      CallM1Service(con:=con, tran:=tran, docid:=docID, documenttype:=docType, eid:=EID)
                '  End If
                'Write code for M1 to call webservice and supplied vendor portal data code start here 



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
                            da.SelectCommand.CommandText = "select " & dtRM.Rows(k).Item("fieldName").ToString & " from MMM_MST_user with(nolock) where EID=" & EID & " and uid=" & Creator
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
                                da.SelectCommand.CommandText = "select " & dtRM.Rows(k).Item("fieldName").ToString & " from MMM_MST_user with(nolock) where EID=" & EID & " and uid=" & LScreator
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
                            Dim dtFlds As New DataTable
                            strQry = "select rolename from MMM_MST_AuthMetrix with(nolock) where EID=" & EID & " and doctype='" & docType & "' and ordering=" & dtRM.Rows(k).Item("ordering").ToString  'CurOrdering
                            For i As Integer = 0 To dtFlds.Rows.Count - 1
                                Dim dType As String = dtFlds.Rows(i).Item("datatype").ToString()
                                Dim fldMapping As String = dtFlds.Rows(i).Item("FieldMapping").ToString()
                                If dtDoc.Rows(0).Item(fldMapping).ToString().Length > 0 Then
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
                            If dtK.Rows.Count = 1 Then
                                nxtUser = tmpUser
                            Else
                                nxtUser = 0
                                objDC.TranExecuteQryDT(Qry:="insert into GetNextuserRoleMatrixHistory (Query,Curstatus,EID,uid,CurrentOrdering,documenttype)values('" & strQry.ToString().Replace("'", "''") & "','" & CurStatus & "'," & EID & "," & CUID & "," & CurOrdering & ",'" & docType & "')", con:=con, tran:=tran)
                            End If
                            If tmpUser <> 0 And nxtUser <> 0 Then
                                nxtUser = tmpUser
                            End If
                        Case "#USER"
                            da.SelectCommand.CommandType = CommandType.Text
                            da.SelectCommand.Parameters.Clear()
                            da.SelectCommand.CommandText = "select " & dtRM.Rows(k).Item("fieldName").ToString & " from MMM_MST_Doc  with(nolock) where EID=" & EID & " and tid=" & docID
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
                            da.SelectCommand.CommandText = "select formid, docmapping, FORMNAME from MMM_MST_FORMS with(nolock) where EID=" & EID & " and isRoleDef=1"
                            Dim dtRRef As New DataTable
                            da.Fill(dtRRef)
                            Dim strMainQry As String = ""
                            Dim strFldQry As String = ""
                            strMainQry = "select uid from MMM_Ref_Role_User with(nolock) where Eid=" & EID & " and ',' + documenttype + ','  like '%," & docType & ",%' and rolename='" & CurRoleName & "'"

                            For i As Integer = 0 To dtRRef.Rows.Count - 1
                                Dim Found As Boolean = False
                                da.SelectCommand.CommandType = CommandType.Text
                                da.SelectCommand.Parameters.Clear()
                                da.SelectCommand.CommandText = "select fieldmapping from MMM_MST_FIELDS with(nolock) where  Eid=" & EID & " and DocumentType='" & docType & "' and dropdowntype='MASTER VALUED' and substring(dropdown,8,(charindex('-',dropdown,8)-8)) = '" & dtRRef.Rows(i).Item("formname").ToString & "'"
                                Dim dtDF As New DataTable
                                da.Fill(dtDF)
                                If dtDF.Rows.Count > 0 Then   '' write in else of this to add def % in condition. 
                                    da.SelectCommand.CommandType = CommandType.Text
                                    da.SelectCommand.Parameters.Clear()
                                    ''  get fld value from document table 
                                    da.SelectCommand.CommandText = "select " & dtDF.Rows(0).Item(0).ToString & " from MMM_MST_doc with(nolock) where EID=" & EID & " and  Tid=" & docID
                                    Dim dtDR As New DataTable
                                    da.Fill(dtDR)
                                    If dtDR.Rows.Count <> 0 Then
                                        If dtDR.Rows(0).Item(0).ToString <> "" Then
                                            strFldQry &= " and (','+ convert(nvarchar(max), " & dtRRef.Rows(i).Item("docmapping") & ") +',' like '%," & dtDR.Rows(0).Item(0).ToString & ",%' or ltrim( convert(nvarchar(max), " & dtRRef.Rows(i).Item("docmapping") & "))='*' )"
                                        End If
                                    End If
                                End If

                                '' ddllookupvalue added  by sunil on 04th October 14 - starts
                                If Found = False Then
                                    da.SelectCommand.CommandType = CommandType.Text
                                    da.SelectCommand.Parameters.Clear()
                                    ' prev dis. on 28-sep for optimization by sunil  - DDLlookupValueSource
                                    da.SelectCommand.CommandText = "select fieldmapping from MMM_MST_FIELDS with(nolock) where  Eid=" & EID & " and DocumentType='" & docType & "' and fieldtype='LookupDDL' and DDLlookupValueSource='" & dtRRef.Rows(i).Item("formname").ToString & "'"
                                    Dim dtDFddl As New DataTable
                                    da.Fill(dtDFddl)
                                    If dtDFddl.Rows.Count > 0 Then   '' write in else of this to add def % in condition. 
                                        da.SelectCommand.CommandType = CommandType.Text
                                        da.SelectCommand.Parameters.Clear()
                                        ''  get fld value from document table 
                                        da.SelectCommand.CommandText = "select " & dtDFddl.Rows(0).Item(0).ToString & " from MMM_MST_doc with(nolock) where EID=" & EID & " and  Tid=" & docID
                                        Dim dtDR As New DataTable
                                        da.Fill(dtDR)
                                        If dtDR.Rows.Count <> 0 Then
                                            If dtDR.Rows(0).Item(0).ToString <> "" Then
                                                strFldQry &= " and (','+" & dtRRef.Rows(i).Item("docmapping") & " +',' like '%," & dtDR.Rows(0).Item(0).ToString & ",%' or ltrim(" & dtRRef.Rows(i).Item("docmapping") & ")='*' )"
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
                                            da.SelectCommand.CommandText = "select COUNT(userid) [loadcount] from MMM_DOC_DTL dt  with (nolock) left outer join MMM_MST_DOC D  with (nolock) on dt.tid=d.lasttid where d.EID=" & EID & " and d.DocumentType='" & docType & "' and dt.userid in (" & usrs & ")  and d.curstatus= '" & curAprStatus & "' and dt.tdate is null and dt.aprstatus is null  group by userid "
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

                    Dim dtTmp As New DataTable
                    Select Case CurRoleName
                        Case "#SELF"
                            AuthMetrixCustomQry(dtRM.Rows(k).Item("customqry").ToString, docid:=docID, con:=con, tran:=tran, statusName:=curAprStatus, roleName:=CurRoleName)

                            Dim dtFlds As New DataTable
                            da.SelectCommand.CommandText = "select * from MMM_MST_FIELDS with(nolock) where EID=" & EID & " and documenttype='" & docType & "' AND ISWORKFLOW=1"
                            da.Fill(dtFlds)

                            ' Dim strQry As String = "select top 1 uid from MMM_MST_AuthMetrix A left outer join MMM_MST_WORKFLOW_STATUS S on A.EID=S.EID and S.statusname=a.aprStatus and S.Documenttype=A.doctype where A.EID=" & EID & " and doctype='" & docType & "' and A.ordering>" & CurOrdering
                            strQry = "select rolename from MMM_MST_AuthMetrix with(nolock)  where  docnature='" & CurDocNature & "' and EID=" & EID & " and doctype='" & docType & "' and ordering=" & dtRM.Rows(k).Item("ordering").ToString  'CurOrdering
                            For i As Integer = 0 To dtFlds.Rows.Count - 1
                                Dim dType As String = dtFlds.Rows(i).Item("datatype").ToString()
                                Dim fldMapping As String = dtFlds.Rows(i).Item("FieldMapping").ToString()
                                If dtDoc.Rows(0).Item(fldMapping).ToString().Length > 0 Then
                                    Select Case dType.ToUpper()
                                        Case "TEXT"
                                            strQry &= " AND case " & fldMapping & " WHEN '*' Then '" & dtDoc.Rows(0).Item(fldMapping) & "' ELSE " & fldMapping & "  END  like '%" & dtDoc.Rows(0).Item(fldMapping) & "%' "
                                        Case "NUMERIC"
                                            strQry &= " and convert(numeric(15,0),PARSENAME(REPLACE(" & fldMapping & ", '-', '.'), 2)) <= " & dtDoc.Rows(0).Item(fldMapping) & " and convert(numeric(15,0),PARSENAME(REPLACE(" & fldMapping & ", '-', '.'), 1)) >= " & dtDoc.Rows(0).Item(fldMapping)
                                    End Select
                                End If
                            Next
                            strQry &= " order by ordering"
                            Dim dtK As New DataTable
                            da.SelectCommand.CommandType = CommandType.Text
                            da.SelectCommand.Parameters.Clear()
                            da.SelectCommand.CommandText = strQry
                            da.Fill(dtK)
                            If dtK.Rows.Count = 1 Then
                                nxtUser = Creator
                                ' nxtUser = dtK.Rows(0).Item(0).ToString
                            End If

                        Case "#CURRENTUSER"

                            Dim dtFlds As New DataTable
                            da.SelectCommand.CommandText = "select * from MMM_MST_FIELDS with(nolock) where EID=" & EID & " and documenttype='" & docType & "' AND ISWORKFLOW=1"
                            da.Fill(dtFlds)

                            ' Dim strQry As String = "select top 1 uid from MMM_MST_AuthMetrix A left outer join MMM_MST_WORKFLOW_STATUS S on A.EID=S.EID and S.statusname=a.aprStatus and S.Documenttype=A.doctype where A.EID=" & EID & " and doctype='" & docType & "' and A.ordering>" & CurOrdering
                            strQry = "select rolename from MMM_MST_AuthMetrix with(nolock) where docnature='" & CurDocNature & "' and EID=" & EID & " and doctype='" & docType & "' and ordering=" & dtRM.Rows(k).Item("ordering").ToString  'CurOrdering
                            For i As Integer = 0 To dtFlds.Rows.Count - 1
                                Dim dType As String = dtFlds.Rows(i).Item("datatype").ToString()
                                Dim fldMapping As String = dtFlds.Rows(i).Item("FieldMapping").ToString()
                                If dtDoc.Rows(0).Item(fldMapping).ToString().Length > 0 Then
                                    Select Case dType.ToUpper()
                                        Case "TEXT"
                                            strQry &= " AND case " & fldMapping & " WHEN '*' Then '" & dtDoc.Rows(0).Item(fldMapping) & "' ELSE " & fldMapping & "  END  like '%" & dtDoc.Rows(0).Item(fldMapping) & "%' "
                                        Case "NUMERIC"
                                            strQry &= " and convert(numeric(15,0),PARSENAME(REPLACE(" & fldMapping & ", '-', '.'), 2)) <= " & dtDoc.Rows(0).Item(fldMapping) & " and convert(numeric(15,0),PARSENAME(REPLACE(" & fldMapping & ", '-', '.'), 1)) >= " & dtDoc.Rows(0).Item(fldMapping)
                                    End Select
                                End If
                            Next
                            strQry &= " order by ordering"
                            Dim dtK As New DataTable
                            da.SelectCommand.CommandType = CommandType.Text
                            da.SelectCommand.Parameters.Clear()
                            da.SelectCommand.CommandText = strQry
                            da.Fill(dtK)
                            If dtK.Rows.Count = 1 Then
                                nxtUser = CurrentUser
                            End If

                        Case "#SUPERVISOR"
                            da.SelectCommand.CommandType = CommandType.Text
                            da.SelectCommand.Parameters.Clear()
                            da.SelectCommand.CommandText = "select " & dtRM.Rows(k).Item("fieldName").ToString & " from MMM_MST_user with(nolock) where EID=" & EID & " and uid=" & Creator
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
                                da.SelectCommand.CommandText = "select " & dtRM.Rows(k).Item("fieldName").ToString & " from MMM_MST_user with(nolock) where EID=" & EID & " and uid=" & LScreator
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

                            Dim dtFlds As New DataTable
                            da.SelectCommand.CommandText = "select * from MMM_MST_FIELDS with(nolock) where EID=" & EID & " and documenttype='" & docType & "' AND ISWORKFLOW=1"
                            da.Fill(dtFlds)

                            ' Dim strQry As String = "select top 1 uid from MMM_MST_AuthMetrix A left outer join MMM_MST_WORKFLOW_STATUS S on A.EID=S.EID and S.statusname=a.aprStatus and S.Documenttype=A.doctype where A.EID=" & EID & " and doctype='" & docType & "' and A.ordering>" & CurOrdering
                            strQry = "select rolename from MMM_MST_AuthMetrix with(nolock)  where docnature='" & CurDocNature & "' and EID=" & EID & " and doctype='" & docType & "' and ordering=" & dtRM.Rows(k).Item("ordering").ToString  'CurOrdering


                            For i As Integer = 0 To dtFlds.Rows.Count - 1
                                Dim dType As String = dtFlds.Rows(i).Item("datatype").ToString()
                                Dim fldMapping As String = dtFlds.Rows(i).Item("FieldMapping").ToString()
                                If dtDoc.Rows(0).Item(fldMapping).ToString().Length > 0 Then
                                    Select Case dType.ToUpper()
                                        Case "TEXT"
                                            strQry &= " AND case " & fldMapping & " WHEN '*' Then '" & dtDoc.Rows(0).Item(fldMapping) & "' ELSE " & fldMapping & "  END  like '%" & dtDoc.Rows(0).Item(fldMapping) & "%' "
                                        Case "NUMERIC"
                                            strQry &= " and convert(numeric(15,0),PARSENAME(REPLACE(" & fldMapping & ", '-', '.'), 2)) <= " & dtDoc.Rows(0).Item(fldMapping) & " and convert(numeric(15,0),PARSENAME(REPLACE(" & fldMapping & ", '-', '.'), 1)) >= " & dtDoc.Rows(0).Item(fldMapping)
                                    End Select
                                End If
                            Next
                            strQry &= " order by ordering"
                            Dim dtK As New DataTable
                            da.SelectCommand.CommandType = CommandType.Text
                            da.SelectCommand.Parameters.Clear()
                            da.SelectCommand.CommandText = strQry
                            da.Fill(dtK)
                            If dtK.Rows.Count = 1 Then
                                nxtUser = tmpUser
                            Else
                                nxtUser = 0
                                objDC.TranExecuteQryDT(Qry:="insert into GetNextuserRoleMatrixHistory (Query,Curstatus,EID,uid,CurrentOrdering,documenttype)values('" & strQry.ToString().Replace("'", "''") & "','" & CurStatus & "'," & EID & "," & CUID & "," & CurOrdering & ",'" & docType & "')", con:=con, tran:=tran)
                            End If

                            If tmpUser <> 0 And nxtUser <> 0 Then
                                nxtUser = tmpUser
                            End If
                        Case "#USER"
                            AuthMetrixCustomQry(dtRM.Rows(k).Item("customqry").ToString, docid:=docID, con:=con, tran:=tran, statusName:=curAprStatus, roleName:=CurRoleName)
                            da.SelectCommand.CommandType = CommandType.Text
                            da.SelectCommand.Parameters.Clear()
                            da.SelectCommand.CommandText = "select " & dtRM.Rows(k).Item("fieldName").ToString & " from MMM_MST_Doc with(nolock) where EID=" & EID & " and tid=" & docID
                            dtTmp.Clear()

                            da.Fill(dtTmp)
                            If dtTmp.Rows.Count <> 0 Then
                                If IsNumeric(dtTmp.Rows(0).Item(0).ToString) = True Then
                                    nxtUser = dtTmp.Rows(0).Item(0).ToString
                                End If
                            End If

                            ''for gen. query of document flds in role based user
                            Dim dtFlds As New DataTable
                            da.SelectCommand.CommandText = "select * from MMM_MST_FIELDS with(nolock) where EID=" & EID & " and documenttype='" & docType & "' AND ISWORKFLOW=1"
                            da.Fill(dtFlds)

                            strQry = "select rolename from MMM_MST_AuthMetrix with(nolock) where docnature='" & CurDocNature & "' and EID=" & EID & " and doctype='" & docType & "' and ordering=" & dtRM.Rows(k).Item("ordering").ToString  'CurOrdering
                            For i As Integer = 0 To dtFlds.Rows.Count - 1
                                Dim dType As String = dtFlds.Rows(i).Item("datatype").ToString()
                                Dim fldMapping As String = dtFlds.Rows(i).Item("FieldMapping").ToString()
                                If dtDoc.Rows(0).Item(fldMapping).ToString().Length > 0 Then
                                    Select Case dType.ToUpper()
                                        Case "TEXT"
                                            strQry &= " AND case " & fldMapping & " WHEN '*' Then '" & dtDoc.Rows(0).Item(fldMapping) & "' ELSE " & fldMapping & "  END  like '%" & dtDoc.Rows(0).Item(fldMapping) & "%' "
                                        Case "NUMERIC"
                                            strQry &= " and convert(numeric(15,0),PARSENAME(REPLACE(" & fldMapping & ", '-', '.'), 2)) <= " & dtDoc.Rows(0).Item(fldMapping) & " and convert(numeric(15,0),PARSENAME(REPLACE(" & fldMapping & ", '-', '.'), 1)) >= " & dtDoc.Rows(0).Item(fldMapping)
                                    End Select
                                End If
                            Next
                            strQry &= " order by ordering"
                            Dim dtK As New DataTable
                            da.SelectCommand.CommandType = CommandType.Text
                            da.SelectCommand.Parameters.Clear()
                            da.SelectCommand.CommandText = strQry
                            da.Fill(dtK)

                            If Not (dtK.Rows.Count > 0 And nxtUser > 0) Then
                                nxtUser = 0
                                objDC.TranExecuteQryDT(Qry:="insert into GetNextuserRoleMatrixHistory (Query,Curstatus,EID,uid,CurrentOrdering,documenttype)values('" & strQry.ToString().Replace("'", "''") & "','" & CurStatus & "'," & EID & "," & CUID & "," & CurOrdering & ",'" & docType & "')", con:=con, tran:=tran)
                            End If
                        Case Else
                            '' any other role  '' FOR NEW role with document fields   by sunil 14_july
                            da.SelectCommand.CommandType = CommandType.Text
                            da.SelectCommand.Parameters.Clear()
                            da.SelectCommand.CommandText = "select formid, docmapping, FORMNAME from MMM_MST_FORMS with(nolock) where EID=" & EID & " and isRoleDef=1"
                            Dim dtRRef As New DataTable
                            da.Fill(dtRRef)
                            Dim strMainQry As String = ""
                            Dim strFldQry As String = ""
                            strMainQry = "select uid,rolename from MMM_Ref_Role_User  with(nolock) where Eid=" & EID & " and ',' + documenttype + ','  like '%," & docType & ",%' and rolename='" & CurRoleName & "'"

                            For i As Integer = 0 To dtRRef.Rows.Count - 1
                                Dim Found As Boolean = False
                                da.SelectCommand.CommandType = CommandType.Text
                                da.SelectCommand.Parameters.Clear()
                                da.SelectCommand.CommandText = "select fieldmapping from MMM_MST_FIELDS with(nolock) where  Eid=" & EID & " and DocumentType='" & docType & "' and dropdowntype='MASTER VALUED' and substring(dropdown,8,(charindex('-',dropdown,8)-8)) = '" & dtRRef.Rows(i).Item("formname").ToString & "'"
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
                                        Found = True
                                        If dtDR.Rows(0).Item(0).ToString <> "" Then
                                            strFldQry &= " and (','+ convert(nvarchar(max), " & dtRRef.Rows(i).Item("docmapping") & " ) +',' like '%," & dtDR.Rows(0).Item(0).ToString & ",%' or ltrim( convert(nvarchar(max), " & dtRRef.Rows(i).Item("docmapping") & "))='*' )"
                                        End If
                                    End If
                                End If

                                '' ddllookupvalue added  by sunil on 04th October 14 - starts
                                If Found = False Then
                                    da.SelectCommand.CommandType = CommandType.Text
                                    da.SelectCommand.Parameters.Clear()
                                    ' prev dis. on 28-sep for optimization by sunil  - DDLlookupValueSource
                                    da.SelectCommand.CommandText = "select fieldmapping from MMM_MST_FIELDS  with(nolock) where  Eid=" & EID & " and DocumentType='" & docType & "' and fieldtype='LookupDDL' and DDLlookupValueSource='" & dtRRef.Rows(i).Item("formname").ToString & "'"
                                    Dim dtDFddl As New DataTable
                                    da.Fill(dtDFddl)
                                    If dtDFddl.Rows.Count > 0 Then   '' write in else of this to add def % in condition. 
                                        da.SelectCommand.CommandType = CommandType.Text
                                        da.SelectCommand.Parameters.Clear()
                                        ''  get fld value from document table 
                                        da.SelectCommand.CommandText = "select " & dtDFddl.Rows(0).Item(0).ToString & " from MMM_MST_doc with(nolock) where EID=" & EID & " and  Tid=" & docID
                                        Dim dtDR As New DataTable
                                        da.Fill(dtDR)
                                        If dtDR.Rows.Count <> 0 Then
                                            If dtDR.Rows(0).Item(0).ToString <> "" Then
                                                strFldQry &= " and (','+" & dtRRef.Rows(i).Item("docmapping") & " +',' like '%," & dtDR.Rows(0).Item(0).ToString & ",%' or ltrim(" & dtRRef.Rows(i).Item("docmapping") & ")='*' )"
                                            End If
                                        End If
                                    End If
                                End If
                                '' ddllookupvalue added  by sunil on 04th October 14 - ends
                                Found = False
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
                                            da.SelectCommand.CommandText = "select COUNT(userid) [loadcount] from MMM_DOC_DTL dt  with (nolock) left outer join MMM_MST_DOC D  with (nolock) on dt.tid=d.lasttid where d.EID=" & EID & " and d.DocumentType='" & docType & "' and dt.userid in (" & usrs & ")  and d.curstatus= '" & curAprStatus & "' and dt.tdate is null and dt.aprstatus is null  group by userid "
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
                                End If

                                ''for gen. query of document flds in role based user
                                Dim dtFlds As New DataTable
                                da.SelectCommand.CommandText = "select * from MMM_MST_FIELDS with(nolock) where EID=" & EID & " and documenttype='" & docType & "' AND ISWORKFLOW=1"
                                da.Fill(dtFlds)

                                strQry = "select rolename from MMM_MST_AuthMetrix with(nolock) where docnature='" & CurDocNature & "' and EID=" & EID & " and doctype='" & docType & "' and ordering=" & dtRM.Rows(k).Item("ordering").ToString  'CurOrdering
                                For i As Integer = 0 To dtFlds.Rows.Count - 1
                                    Dim dType As String = dtFlds.Rows(i).Item("datatype").ToString()
                                    Dim fldMapping As String = dtFlds.Rows(i).Item("FieldMapping").ToString()
                                    If dtDoc.Rows(0).Item(fldMapping).ToString().Length > 0 Then
                                        Select Case dType.ToUpper()
                                            Case "TEXT"
                                                strQry &= " AND case " & fldMapping & " WHEN '*' Then '" & dtDoc.Rows(0).Item(fldMapping) & "' ELSE " & fldMapping & "  END  like '%" & dtDoc.Rows(0).Item(fldMapping) & "%' "
                                            Case "NUMERIC"
                                                strQry &= " and convert(numeric(15,0),PARSENAME(REPLACE(" & fldMapping & ", '-', '.'), 2)) <= " & dtDoc.Rows(0).Item(fldMapping) & " and convert(numeric(15,0),PARSENAME(REPLACE(" & fldMapping & ", '-', '.'), 1)) >= " & dtDoc.Rows(0).Item(fldMapping)
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
                                        If dtRU.Rows.Count > 1 Then  '''' NEXT USER WILL BE FROM QUEUING WHICH HAVE MINIMUM DOCUMENT  (MINUSERID)
                                        Else
                                            nxtUser = dtRU.Rows(0).Item("uid").ToString ' IF ONLY ONE USER EXIST  
                                        End If
                                    End If
                                Else
                                    nxtUser = 0
                                    objDC.TranExecuteQryDT(Qry:="insert into GetNextuserRoleMatrixHistory (Query,Curstatus,EID,uid,CurrentOrdering,documenttype)values('" & strQry.ToString().Replace("'", "''") & "','" & CurStatus & "'," & EID & "," & CUID & "," & CurOrdering & ",'" & docType & "')", con:=con, tran:=tran)
                                End If
                                ''for gen. query of document flds in role based user
                            End If
                    End Select
                ElseIf dtRM.Rows(k).Item("type").ToString = "USER" Then
                    Dim dtFlds As New DataTable
                    da.SelectCommand.CommandText = "select * from MMM_MST_FIELDS with(nolock) where EID=" & EID & " and documenttype='" & docType & "' AND ISWORKFLOW=1"
                    da.Fill(dtFlds)

                    ' Dim strQry As String = "select top 1 uid from MMM_MST_AuthMetrix A left outer join MMM_MST_WORKFLOW_STATUS S on A.EID=S.EID and S.statusname=a.aprStatus and S.Documenttype=A.doctype where A.EID=" & EID & " and doctype='" & docType & "' and A.ordering>" & CurOrdering
                    strQry = "select rolename from MMM_MST_AuthMetrix with(nolock)  where docnature='" & CurDocNature & "' and EID=" & EID & " and doctype='" & docType & "' and ordering=" & dtRM.Rows(k).Item("ordering").ToString  'CurOrdering
                    For i As Integer = 0 To dtFlds.Rows.Count - 1
                        Dim dType As String = dtFlds.Rows(i).Item("datatype").ToString()
                        Dim fldMapping As String = dtFlds.Rows(i).Item("FieldMapping").ToString()
                        If dtDoc.Rows(0).Item(fldMapping).ToString().Length > 0 Then
                            Select Case dType.ToUpper()
                                Case "TEXT"
                                    strQry &= " AND case " & fldMapping & " WHEN '*' Then '" & dtDoc.Rows(0).Item(fldMapping) & "' ELSE " & fldMapping & "  END  like '%" & dtDoc.Rows(0).Item(fldMapping) & "%' "
                                Case "NUMERIC"
                                    strQry &= " and convert(numeric(15,0),PARSENAME(REPLACE(" & fldMapping & ", '-', '.'), 2)) <= " & dtDoc.Rows(0).Item(fldMapping) & " and convert(numeric(15,0),PARSENAME(REPLACE(" & fldMapping & ", '-', '.'), 1)) >= " & dtDoc.Rows(0).Item(fldMapping)
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
                    objDC.TranExecuteQryDT(Qry:="insert into GetNextuserRoleMatrixHistory (Query,Curstatus,EID,uid,CurrentOrdering,documenttype)values('" & strQry.ToString().Replace("'", "''") & "','" & CurStatus & "'," & EID & "," & CUID & "," & CurOrdering & ",'" & docType & "')", con:=con, tran:=tran)
                    CheckSkipfeat = True
                    Exit For
                Else
                    If nxtUser <> 0 Then
                        If CurStatus = dtRM.Rows(k).Item("aprstatus").ToString() Then
                            sRetMsg = "Current and Next Status cannot be same"
                            Exit For
                        End If
                        da.SelectCommand.CommandText = "ApproveWorkFlow_RM_with_Isauth_2"
                        da.SelectCommand.CommandType = CommandType.StoredProcedure
                        da.SelectCommand.Parameters.Clear()
                        da.SelectCommand.Parameters.AddWithValue("tid", docID)
                        da.SelectCommand.Parameters.AddWithValue("nUid", nxtUser)
                        da.SelectCommand.Parameters.AddWithValue("NxtStatus", dtRM.Rows(k).Item("aprstatus").ToString)
                        da.SelectCommand.Parameters.AddWithValue("nOrder", dtRM.Rows(k).Item("ordering").ToString)
                        da.SelectCommand.Parameters.AddWithValue("nSLA", dtRM.Rows(k).Item("SLA").ToString)
                        da.SelectCommand.Parameters.AddWithValue("UserRole", UserRole)
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
                da.SelectCommand.CommandText = "InsertDefaultMovement_with_Isauth_2"
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


    Public Function AuthMetrixCustomQry(Optional ByVal customQry As String = "", Optional ByVal docid As Integer = 0, Optional ByVal con As SqlConnection = Nothing, Optional ByVal tran As SqlTransaction = Nothing, Optional ByVal statusName As String = "", Optional ByVal roleName As String = "", Optional ByVal action As String = "ADD") As Boolean
        Dim result As Boolean = False
        Try
            If Not (IsNothing(con) And IsNothing(tran)) Then
                If Not String.IsNullOrEmpty(customQry) Then
                    If customQry.ToString().Contains("@docid") Then
                        customQry = customQry.ToString().Replace("@docid", docid)
                        Dim objDC As New DataClass()
                        Dim tranQry As String = customQry.Replace("'", "''")
                        objDC.TranExecuteQryDT(customQry, con:=con, tran:=tran)
                        objDC.TranExecuteQryDT("insert into AuthMetrixCustromHistory (CustomQry,docid,curstatus,rolename,action,createdon)values('" & tranQry.ToString() & "'," & docid & ",'" & statusName & "','" & roleName & "','" & action & "',getdate())", con:=con, tran:=tran)

                    End If
                End If
            End If
        Catch ex As Exception
        End Try
        Return result
    End Function



    Public Function CheckNextUserT(ByVal docID As Long, ByVal EID As Long, ByVal CUID As Integer, ByVal qry As String, ByVal Auid As Integer, ByVal con As SqlConnection, ByVal tran As SqlTransaction) As String
        'Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        'Dim con As New SqlConnection(conStr)
        Dim da As New SqlDataAdapter("select d.*, dt.ordering,dt.userid from MMM_MST_DOC D left outer join MMM_DOC_DTL dt on d.LastTID=dt.tid where EID=" & EID & " and d.tid=" & docID, con)
        'try Catch Block Added by Ajeet 
        Try
            Dim dtDoc As New DataTable
            da.SelectCommand.Transaction = tran
            da.Fill(dtDoc)

            Dim docType As String = dtDoc.Rows(0).Item("documenttype").ToString
            Dim CurOrdering As Integer = dtDoc.Rows(0).Item("ordering").ToString
            Dim Creator As Integer = dtDoc.Rows(0).Item("ouid").ToString
            Dim CurDocNature As String = dtDoc.Rows(0).Item("CurdocNature").ToString
            Dim CurrentUser As Integer = dtDoc.Rows(0).Item("userid").ToString
            Dim CurStatus As String = dtDoc.Rows(0).Item("CurStatus").ToString


            If Trim(CurStatus.ToUpper) = "ARCHIVE" Then
                Return "Can not Approve, Reached to ARCHIVE"
                Exit Function
            End If

            '''' get all rows after current ordering 
            'prev b4 skip feature
            'da.SelectCommand.CommandText = "select * from MMM_MST_AuthMetrix where EID=" & EID & " and doctype='" & docType & "' and docnature='" & CurDocNature & "' AND ordering >" & CurOrdering & " order by ordering"
            da.SelectCommand.CommandText = "select am.*,wf.isallowskip from MMM_MST_AuthMetrix am inner join MMM_MST_WORKFLOW_STATUS wf on am.aprStatus=wf.StatusName and am.doctype=wf.Documenttype  where am.EID=" & EID & " and am.doctype='" & docType & "' and am.docnature='" & CurDocNature & "' AND am.ordering >" & CurOrdering & " order by am.ordering"

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
                                                '  strFldQry &= " and ','+" & dtRRef.Rows(i).Item("docmapping") & " +',' like '%," & dtDR.Rows(0).Item(0).ToString & ",%'"
                                                strFldQry &= " and (','+" & dtRRef.Rows(i).Item("docmapping") & " +',' like '%," & dtDR.Rows(0).Item(0).ToString & ",%' or ltrim(" & dtRRef.Rows(i).Item("docmapping") & ")='*' )"
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
                                                ' strFldQry &= " and ','+" & dtRRef.Rows(i).Item("docmapping") & " +',' like '%," & dtDR.Rows(0).Item(0).ToString & ",%'"
                                                strFldQry &= " and (','+" & dtRRef.Rows(i).Item("docmapping") & " +',' like '%," & dtDR.Rows(0).Item(0).ToString & ",%' or ltrim(" & dtRRef.Rows(i).Item("docmapping") & ")='*' )"
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
                                            da.SelectCommand.CommandText = "select COUNT(userid) [loadcount] from MMM_DOC_DTL dt  with (nolock) left outer join MMM_MST_DOC D  with (nolock) on dt.tid=d.lasttid where d.EID=" & EID & " and d.DocumentType='" & docType & "' and dt.userid in (" & usrs & ")  and d.curstatus= '" & curAprStatus & "' and dt.tdate is null and dt.aprstatus is null  group by userid "
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
                                End If
                            End If
                    End Select
                ElseIf dtRM.Rows(k).Item("type").ToString = "NEWROLE" Then
                    CurRoleName = dtRM.Rows(k).Item("Rolename").ToString
                    curAprStatus = dtRM.Rows(k).Item("aprstatus").ToString

                    Dim dtTmp As New DataTable
                    Select Case CurRoleName
                        Case "#SELF"


                            Dim dtFlds As New DataTable
                            da.SelectCommand.CommandText = "select * from MMM_MST_FIELDS where EID=" & EID & " and documenttype='" & docType & "' AND ISWORKFLOW=1"
                            da.Fill(dtFlds)

                            ' Dim strQry As String = "select top 1 uid from MMM_MST_AuthMetrix A left outer join MMM_MST_WORKFLOW_STATUS S on A.EID=S.EID and S.statusname=a.aprStatus and S.Documenttype=A.doctype where A.EID=" & EID & " and doctype='" & docType & "' and A.ordering>" & CurOrdering
                            Dim strQry As String = "select rolename from MMM_MST_AuthMetrix  where EID=" & EID & " and doctype='" & docType & "' and ordering=" & dtRM.Rows(k).Item("ordering").ToString  'CurOrdering
                            For i As Integer = 0 To dtFlds.Rows.Count - 1
                                Dim dType As String = dtFlds.Rows(i).Item("datatype").ToString()
                                Dim fldMapping As String = dtFlds.Rows(i).Item("FieldMapping").ToString()
                                If dtDoc.Rows(0).Item(fldMapping).ToString().Length > 0 Then
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
                            If dtK.Rows.Count = 1 Then
                                nxtUser = Creator
                                ' nxtUser = dtK.Rows(0).Item(0).ToString
                            End If

                        Case "#CURRENTUSER"

                            Dim dtFlds As New DataTable
                            da.SelectCommand.CommandText = "select * from MMM_MST_FIELDS where EID=" & EID & " and documenttype='" & docType & "' AND ISWORKFLOW=1"
                            da.Fill(dtFlds)

                            ' Dim strQry As String = "select top 1 uid from MMM_MST_AuthMetrix A left outer join MMM_MST_WORKFLOW_STATUS S on A.EID=S.EID and S.statusname=a.aprStatus and S.Documenttype=A.doctype where A.EID=" & EID & " and doctype='" & docType & "' and A.ordering>" & CurOrdering
                            Dim strQry As String = "select rolename from MMM_MST_AuthMetrix  where EID=" & EID & " and doctype='" & docType & "' and ordering=" & dtRM.Rows(k).Item("ordering").ToString  'CurOrdering
                            For i As Integer = 0 To dtFlds.Rows.Count - 1
                                Dim dType As String = dtFlds.Rows(i).Item("datatype").ToString()
                                Dim fldMapping As String = dtFlds.Rows(i).Item("FieldMapping").ToString()
                                If dtDoc.Rows(0).Item(fldMapping).ToString().Length > 0 Then
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
                            If dtK.Rows.Count = 1 Then
                                nxtUser = CurrentUser
                            End If

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
                                                '    strFldQry &= " and ','+" & dtRRef.Rows(i).Item("docmapping") & " +',' like '%," & dtDR.Rows(0).Item(0).ToString & ",%'"
                                                strFldQry &= " and (','+" & dtRRef.Rows(i).Item("docmapping") & " +',' like '%," & dtDR.Rows(0).Item(0).ToString & ",%' or ltrim(" & dtRRef.Rows(i).Item("docmapping") & ")='*' )"
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
                                                '   strFldQry &= " and ','+" & dtRRef.Rows(i).Item("docmapping") & " +',' like '%," & dtDR.Rows(0).Item(0).ToString & ",%'"
                                                strFldQry &= " and (','+" & dtRRef.Rows(i).Item("docmapping") & " +',' like '%," & dtDR.Rows(0).Item(0).ToString & ",%' or ltrim(" & dtRRef.Rows(i).Item("docmapping") & ")='*' )"
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
                                            da.SelectCommand.CommandText = "select COUNT(userid) [loadcount] from MMM_DOC_DTL dt  with (nolock) left outer join MMM_MST_DOC D  with (nolock) on dt.tid=d.lasttid where d.EID=" & EID & " and d.DocumentType='" & docType & "' and dt.userid in (" & usrs & ")  and d.curstatus= '" & curAprStatus & "' and dt.tdate is null and dt.aprstatus is null  group by userid "
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
                                End If

                                ''for gen. query of document flds in role based user
                                Dim dtFlds As New DataTable
                                da.SelectCommand.CommandText = "select * from MMM_MST_FIELDS where EID=" & EID & " and documenttype='" & docType & "' AND ISWORKFLOW=1"
                                da.Fill(dtFlds)

                                Dim strQry As String = "select rolename from MMM_MST_AuthMetrix  where EID=" & EID & " and doctype='" & docType & "' and ordering=" & dtRM.Rows(k).Item("ordering").ToString  'CurOrdering
                                For i As Integer = 0 To dtFlds.Rows.Count - 1
                                    Dim dType As String = dtFlds.Rows(i).Item("datatype").ToString()
                                    Dim fldMapping As String = dtFlds.Rows(i).Item("FieldMapping").ToString()
                                    If dtDoc.Rows(0).Item(fldMapping).ToString().Length > 0 Then
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
                                        If dtRU.Rows.Count > 1 Then
                                            '''' NEXT USER WILL BE FROM QUEUING WHICH HAVE MINIMUM DOCUMENT  (MINUSERID)
                                        Else
                                            nxtUser = dtRU.Rows(0).Item("uid").ToString ' IF ONLY ONE USER EXIST  
                                        End If

                                    End If
                                Else
                                    nxtUser = 0
                                End If
                                ''for gen. query of document flds in role based user
                            End If
                    End Select
                ElseIf dtRM.Rows(k).Item("type").ToString = "USER" Then
                    Dim dtFlds As New DataTable
                    da.SelectCommand.CommandText = "select * from MMM_MST_FIELDS where EID=" & EID & " and documenttype='" & docType & "' AND ISWORKFLOW=1"
                    da.Fill(dtFlds)

                    ' Dim strQry As String = "select top 1 uid from MMM_MST_AuthMetrix A left outer join MMM_MST_WORKFLOW_STATUS S on A.EID=S.EID and S.statusname=a.aprStatus and S.Documenttype=A.doctype where A.EID=" & EID & " and doctype='" & docType & "' and A.ordering>" & CurOrdering
                    Dim strQry As String = "select rolename from MMM_MST_AuthMetrix  where EID=" & EID & " and doctype='" & docType & "' and ordering=" & dtRM.Rows(k).Item("ordering").ToString  'CurOrdering
                    For i As Integer = 0 To dtFlds.Rows.Count - 1
                        Dim dType As String = dtFlds.Rows(i).Item("datatype").ToString()
                        Dim fldMapping As String = dtFlds.Rows(i).Item("FieldMapping").ToString()
                        If dtDoc.Rows(0).Item(fldMapping).ToString().Length > 0 Then
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
                        If CurStatus = dtRM.Rows(k).Item("aprstatus").ToString() Then
                            sRetMsg = "Current and Next Status cannot be same"
                            Exit For
                        End If
                        sRetMsg = nxtUser
                        Exit For
                    End If
                End If
            Next  '' K loop till user founds for a role type (end)

            If CheckSkipfeat = True Then
                dtDoc.Dispose() : da.Dispose()
                Return sRetMsg
            End If

            If nxtUser <> 0 Then
                dtDoc.Dispose() : da.Dispose()
                Return sRetMsg
            Else
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
        End Try
    End Function

    Public Function ApplyDynamicAuthMatrixNew(ByVal docID As Long, ByVal EID As Long, ByVal CUID As Integer) As String
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim da As New SqlDataAdapter("select * from MMM_MST_DOC where EID=" & EID & " and tid=" & docID, con)
        'try Catch Block Added by Ajeet Kumar :Date::22 May 2014
        Try
            Dim dtDoc As New DataTable
            da.Fill(dtDoc)

            Dim docType As String = dtDoc.Rows(0).Item("documenttype").ToString
            Dim dtFlds As New DataTable
            da.SelectCommand.CommandText = "select * from MMM_MST_FIELDS where EID=" & EID & " and documenttype='" & docType & "' AND ISWORKFLOW=1"
            da.Fill(dtFlds)

            Dim strQry As String = "INSERT INTO MMM_MOVPATH_DOC(Docid,UID,SLA,wfstatus,isauth,ordering,onRejection) select distinct " & docID & ",uid,sla,aprstatus,S.Dord,A.ordering,RejectStatus from MMM_MST_AuthMetrix A left outer join MMM_MST_WORKFLOW_STATUS S on A.EID=S.EID and S.statusname =a.aprStatus and S.Documenttype=A.doctype where A.EID=" & EID & " and doctype='" & docType & "'"
            For i As Integer = 0 To dtFlds.Rows.Count - 1
                Dim dType As String = dtFlds.Rows(i).Item("datatype").ToString()
                Dim fldMapping As String = dtFlds.Rows(i).Item("FieldMapping").ToString()
                If dtDoc.Rows(0).Item(fldMapping).ToString().Length > 0 Then
                    Select Case dType.ToUpper()
                        Case "TEXT"
                            strQry &= " AND case " & fldMapping & " WHEN '*' Then '" & dtDoc.Rows(0).Item(fldMapping) & "' ELSE " & fldMapping & "  END  like '%" & dtDoc.Rows(0).Item(fldMapping) & "%' "
                        Case "NUMERIC"
                            strQry &= " and convert(numeric(15,0),PARSENAME(REPLACE(" & fldMapping & ", '-', '.'), 2)) < " & dtDoc.Rows(0).Item(fldMapping) & " and convert(numeric(15,0),PARSENAME(REPLACE(" & fldMapping & ", '-', '.'), 1)) > " & dtDoc.Rows(0).Item(fldMapping)
                    End Select
                End If
            Next
            strQry &= " order by S.Dord,A.ordering"
            dtFlds.Dispose()
            dtDoc.Dispose()
            da.SelectCommand.CommandText = "InsertWorkFlowDynamic"
            da.SelectCommand.CommandType = CommandType.StoredProcedure
            da.SelectCommand.Parameters.Clear()
            da.SelectCommand.Parameters.AddWithValue("tid", docID)
            da.SelectCommand.Parameters.AddWithValue("strqry", strQry)
            da.SelectCommand.Parameters.AddWithValue("CUID", CUID)
            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            da.SelectCommand.ExecuteNonQuery()

            Return strQry
        Catch ex As Exception
            Throw
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

    Public Sub InsertAction(ByVal uid As Integer, ByVal actiontype As String, ByVal actiondesc As String)
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("uspInsertAction", con)
        'try Catch Block Added by Ajeet Kumar :Date::22 May 2014
        Try
            oda.SelectCommand.CommandType = CommandType.StoredProcedure
            oda.SelectCommand.Parameters.AddWithValue("uid", uid)
            oda.SelectCommand.Parameters.AddWithValue("actiontype", actiontype)
            oda.SelectCommand.Parameters.AddWithValue("actiondesc", actiondesc)
            If HttpContext.Current Is Nothing Then
                oda.SelectCommand.Parameters.AddWithValue("ipaddress", "AUTO SHEDULAR")
                oda.SelectCommand.Parameters.AddWithValue("macaddress", "AUTO SHEDULAR")
            Else
                oda.SelectCommand.Parameters.AddWithValue("ipaddress", HttpContext.Current.Session("IPADDRESS").ToString())
                oda.SelectCommand.Parameters.AddWithValue("macaddress", HttpContext.Current.Session("MACADDRESS").ToString())
            End If
            oda.SelectCommand.Parameters.AddWithValue("username", HttpContext.Current.Session("USERNAME").ToString())
            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            oda.SelectCommand.ExecuteNonQuery()
            con.Close()
            oda.Dispose()
            con.Dispose()
        Catch ex As Exception
            Throw
        Finally
            If Not oda Is Nothing Then
                oda.Dispose()
            End If
            If Not con Is Nothing Then
                con.Close()
                con.Dispose()
            End If
        End Try

    End Sub

    Public Sub SendMailOfChanges(ByVal changetype As String, ByVal ids As String)
        Dim MailSubject As String = ""
        Dim MailBody As String = ""

        Dim con As SqlConnection = New SqlConnection(ConfigurationManager.ConnectionStrings("conStr").ConnectionString)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("SELECT tid,d.gID,folderName,FName,aDate,ouid,UserName,grpName  FROM MMM_MST_DOC D LEFT OUTER JOIN MMM_MST_USER U on U.uid= D.ouid LEFT OUTER JOIN MMM_MST_GROUP G on G.gid=D.gid where d.tid in (" & ids & ")", con)
        'try Catch Block Added by Ajeet Kumar :Date::22 May 2014
        Try
            Dim ds As New DataSet
            Select Case changetype
                Case "FOLDERCREATION"
                    oda.Fill(ds, "data")
                    Dim grpID As Integer = ds.Tables("data").Rows(0).Item("gid").ToString()
                    oda.SelectCommand.CommandText = "SELECT Username,emailID from MMM_MST_USER where uid in (SELECT distinct uId  from MMM_REF_GRP_USR where grpId=" & grpID & ")"
                    oda.Fill(ds, "mail")
                    MailSubject = "A New Folder is created in your Project"
                    MailBody &= " <br/><br/>A new folder is created in Your Project : <b> " & ds.Tables("data").Rows(0).Item("grpname").ToString() & "</b>"
                    MailBody &= "<br/><br/>"
                    MailBody &= "<table width=""100%"" border=""3px"" cellpadding=""2px"" cellspacing=""2px"">"
                    MailBody &= "<tr style=""background-color:Blue""><td>Folder Name</td><td>Created By</td><td>Creation Time</td></tr>"
                    MailBody &= "<tr><td>" & ds.Tables("data").Rows(0).Item("foldername").ToString() & "</td><td>" & ds.Tables("data").Rows(0).Item("Username").ToString() & "</td><td>" & ds.Tables("data").Rows(0).Item("adate").ToString() & "</td></tr>"
                    MailBody &= "<tr style=""background-color:Blue;text-align:left""><td colspan=""3"">&nbsp;</td></tr></table>"
                    MailBody &= "<br /><br />Thanks and regards<br />My eDMS Team"

                Case "FILEUPLOADED"
                    oda.Fill(ds, "data")
                    Dim grpID As Integer = ds.Tables("data").Rows(0).Item("gid").ToString()
                    oda.SelectCommand.CommandText = "SELECT Username,emailID from MMM_MST_USER where uid in (SELECT distinct uId  from MMM_REF_GRP_USR where grpId=" & grpID & ")"
                    oda.Fill(ds, "mail")
                    MailSubject = "A New File is Uploaded in your Project"
                    MailBody &= " <br/><br/>A new file is uploaded in Your Project : <b> " & ds.Tables("data").Rows(0).Item("grpname").ToString() & "</b>"
                    MailBody &= "<br/><br/>"
                    MailBody &= "<table width=""100%"" border=""3px"" cellpadding=""2px"" cellspacing=""2px"">"
                    MailBody &= "<tr style=""background-color:Blue""><td>File Name</td><td>Created By</td><td>Creation Time</td></tr>"
                    MailBody &= "<tr><td>" & ds.Tables("data").Rows(0).Item("fname").ToString() & "</td><td>" & ds.Tables("data").Rows(0).Item("Username").ToString() & "</td><td>" & ds.Tables("data").Rows(0).Item("adate").ToString() & "</td></tr>"
                    MailBody &= "<tr style=""background-color:Blue;text-align:left""><td colspan=""3"">&nbsp;</td></tr></table>"
                    MailBody &= "<br /><br />Thanks and regards<br />My eDMS Team"

                Case "FILEDELETED"
                    MailSubject = "A Folder is Deleted in your Project"
                    Dim ar() As String = ids.Split("|")
                    MailBody &= " <br/><br/>A folder is Deleted in Your Project :"
                    MailBody &= "<br/><br/>"
                    MailBody &= "<table width=""100%"" border=""3px"" cellpadding=""2px"" cellspacing=""2px"">"
                    MailBody &= "<tr style=""background-color:Blue""><td>Folder Name</td><td>Deleted By</td><td>Deletion Time</td></tr>"
                    MailBody &= "<tr><td>" & ar(0).ToString() & "</td><td>" & ar(1).ToString() & "</td><td>" & Now & "</td></tr>"
                    MailBody &= "<tr style=""background-color:Blue;text-align:left""><td colspan=""3"">&nbsp;</td></tr></table>"
                    MailBody &= "<br /><br />Thanks and regards<br />My eDMS Team"
                    Dim grpID As Integer = ar(2).ToString()
                    oda.SelectCommand.CommandText = "SELECT Username,emailID from MMM_MST_USER where uid in (SELECT distinct uId  from MMM_REF_GRP_USR where grpId=" & grpID & ")"
                    oda.Fill(ds, "mail")

                Case "FOLDERDELETION"
                    MailSubject = "A Folder is Deleted in your Project"
                    Dim ar() As String = ids.Split("|")
                    MailBody &= " <br/><br/>A folder is Deleted in Your Project :"
                    MailBody &= "<br/><br/>"
                    MailBody &= "<table width=""100%"" border=""3px"" cellpadding=""2px"" cellspacing=""2px"">"
                    MailBody &= "<tr style=""background-color:Blue""><td>Folder Name</td><td>Deleted By</td><td>Deletion Time</td></tr>"
                    MailBody &= "<tr><td>" & ar(0).ToString() & "</td><td>" & ar(1).ToString() & "</td><td>" & Now & "</td></tr>"
                    MailBody &= "<tr style=""background-color:Blue;text-align:left""><td colspan=""3"">&nbsp;</td></tr></table>"
                    MailBody &= "<br /><br />Thanks and regards<br />My eDMS Team"
                    Dim grpID As Integer = ar(2).ToString()
                    oda.SelectCommand.CommandText = "SELECT Username,emailID from MMM_MST_USER where uid in (SELECT distinct uId  from MMM_REF_GRP_USR where grpId=" & grpID & ")"
                    oda.Fill(ds, "mail")
                Case Else
            End Select

            For Each rw As DataRow In ds.Tables("mail").Rows
                Try
                    'sendMail(rw("emailid").ToString(), MailSubject, "Dear <b>" & rw("username").ToString() & ",</b>" & MailBody)
                    Dim obj As New MailUtill(eid:=100)
                    obj.SendMail(ToMail:=rw("emailid").ToString(), Subject:=MailSubject, MailBody:="Dear <b>" & rw("username").ToString() & ",</b>" & MailBody, CC:="", BCC:="")
                Catch ex As Exception
                    Continue For
                End Try
            Next
        Catch ex As Exception
            Throw
        Finally
            If Not oda Is Nothing Then
                oda.Dispose()
            End If
            If Not con Is Nothing Then
                con.Close()
                con.Dispose()
            End If
        End Try

    End Sub

    'Private Sub sendMail(ByVal Mto As String, ByVal MSubject As String, ByVal MBody As String)
    '    Try
    '        'Dim Email As New System.Net.Mail.MailMessage("MYNDSAAS<no-reply@myndsol.com>", "manish@myndsol.com", MSubject, MBody)
    '        Dim Email As New System.Net.Mail.MailMessage("MYNDSAAS<no-reply@myndsol.com>", Mto, MSubject, MBody)
    '        Dim mailClient As New System.Net.Mail.SmtpClient()
    '        Email.IsBodyHtml = True
    '        Dim basicAuthenticationInfo As New System.Net.NetworkCredential("no-reply@myndsol.com", "vc79aK123AJ&$kL0")
    '        mailClient.Host = "mail.myndsol.com"
    '        mailClient.UseDefaultCredentials = False
    '        mailClient.Credentials = basicAuthenticationInfo
    '        'mailClient.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.PickupDirectoryFromIis
    '        mailClient.Send(Email)
    '    Catch ex As Exception
    '        Exit Sub
    '    End Try
    'End Sub

    Private Sub InsertWorkFlow(ByVal docid As Integer, ByVal wfid As Integer)
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("InsertWorkFlow", con)
        'try Catch Block Added by Ajeet Kumar :Date::22 May 2014
        Try
            oda.SelectCommand.CommandType = CommandType.StoredProcedure
            oda.SelectCommand.Parameters.AddWithValue("tid", docid)
            oda.SelectCommand.Parameters.AddWithValue("wfid", wfid)
            Dim ds As New DataSet
            oda.Fill(ds, "nothing")
            oda.Dispose()
            con.Dispose()
        Catch ex As Exception
            Throw
        Finally
            If Not oda Is Nothing Then
                oda.Dispose()
            End If
            If Not con Is Nothing Then
                con.Close()
                con.Dispose()
            End If
        End Try

    End Sub

    Public Sub CheckWorkFlow(ByVal docid As Integer, ByVal eid As Integer)
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
        'try Catch Block Added by Ajeet Kumar :Date::22 May 2014
        Try
            oda.SelectCommand.CommandText = "select isWorkFlow, WorkFlowType from MMM_MST_ENTITY  where EID=" & eid
            oda.SelectCommand.CommandType = CommandType.Text
            Dim ds As New DataSet
            oda.Fill(ds, "type")

            Dim isWF As Integer
            Dim WFtype As String
            isWF = ds.Tables("type").Rows(0).Item("isworkflow")
            WFtype = ds.Tables("type").Rows(0).Item("workflowType").ToString

            If isWF = 1 Then
                If WFtype = "DYNAMIC" Then
                    '''' following is for tecumseh company (queueing) 
                    ''CheckandApplyWorkFlowTecum(docid, eid)
                    oda.SelectCommand.CommandText = "select ouid from MMM_MST_DOC where tid=" & docid & ""
                    oda.SelectCommand.CommandType = CommandType.Text
                    oda.Fill(ds, "UID")
                    oda.SelectCommand.CommandText = "InsertDefaultMovement"
                    oda.SelectCommand.CommandType = CommandType.StoredProcedure
                    oda.SelectCommand.Parameters.Clear()
                    oda.SelectCommand.Parameters.AddWithValue("tid", docid)
                    oda.SelectCommand.Parameters.AddWithValue("CUID", ds.Tables("UID").Rows(0).Item("oUID"))
                    oda.SelectCommand.Parameters.AddWithValue("what", "UPLOADED")
                    If con.State <> ConnectionState.Open Then
                        con.Open()
                    End If
                    oda.SelectCommand.ExecuteNonQuery()
                    GetNextUserFromRolematrix(docid, eid, ds.Tables("UID").Rows(0).Item("oUID"), "", ds.Tables("UID").Rows(0).Item("oUID"))

                ElseIf WFtype = "STANDARD" Then
                    oda.SelectCommand.CommandText = "select wfid,wflogic from MMM_MST_WORKFLOW  where EID=" & eid
                    oda.SelectCommand.CommandType = CommandType.Text

                    oda.Fill(ds, "workflow")
                    For i As Integer = 0 To ds.Tables("workflow").Rows.Count - 1
                        Dim strQry As String = "Select tid from MMM_MST_DOC WHERE tid=" & docid & " AND " & ds.Tables("workflow").Rows(i).Item("wflogic").ToString()
                        Dim dt As New DataTable()
                        oda.SelectCommand.CommandText = strQry
                        oda.Fill(dt)
                        If dt.Rows.Count = 1 Then
                            InsertWorkFlow(docid, Val(ds.Tables("workflow").Rows(i).Item("wfid").ToString()))
                            oda.Dispose()
                            con.Dispose()
                            Exit For
                        End If
                    Next
                End If
            End If
        Catch ex As Exception
        Finally
            If Not oda Is Nothing Then
                oda.Dispose()
            End If
            If Not con Is Nothing Then
                con.Close()
                con.Dispose()
            End If
        End Try

    End Sub

    Public Sub CheckandApplyWorkFlowTecum(ByVal docid As Integer, ByVal eid As Integer)
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("uspGetNextUserforTecum", con)
        'try Catch Block Added by Ajeet Kumar :Date::22 May 2014
        Try
            oda.SelectCommand.Parameters.AddWithValue("eid", eid)
            oda.SelectCommand.CommandType = CommandType.StoredProcedure
            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            Dim ApplyWFID As Integer = oda.SelectCommand.ExecuteScalar
            InsertWorkFlow(docid, ApplyWFID)
        Catch ex As Exception
            Throw
        Finally
            If Not oda Is Nothing Then
                oda.Dispose()
            End If
            If Not con Is Nothing Then
                con.Close()
                con.Dispose()
            End If
        End Try

    End Sub

    Private Sub sendMail1(ByVal Mto As String, ByVal cc As String, ByVal bcc As String, ByVal MSubject As String, ByVal MBody As String, Optional ByVal DocType As String = "", Optional ByVal EID As Integer = 0, Optional ByVal UpCommingFrom As String = "", Optional ByVal attachementListString As String = "", Optional ByVal tid As Integer = 0, Optional ByVal listOfDisplayNameForAttachments As String = "")
        'Dim Email As New System.Net.Mail.MailMessage("no-reply@myndsol.com", "manish@myndsol.com", MSubject, MBody & Mto)
        Try
            If Left(Mto, 1) = "{" Then
                Exit Sub
            End If

            ''new for hd mail sending
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As SqlConnection = New SqlConnection(conStr)
            Dim da As SqlDataAdapter = New SqlDataAdapter("", con)
            Dim query As String = "select * from mmm_hdmail_schdule where eid=" & EID & " and documenttype='" & DocType & "' and IsSendMailFromDefaultEMailID=0"
            If UpCommingFrom <> "" Then
                query = query & " and mdmailid='" & UpCommingFrom & "'"
            End If
            da.SelectCommand.CommandText = query
            da.SelectCommand.CommandType = CommandType.Text
            Dim dtSch As New DataTable
            da.Fill(dtSch)
            If dtSch.Rows.Count >= 1 Then
                Dim Email As New System.Net.Mail.MailMessage(dtSch.Rows(0).Item("mdmailid"), Mto, MSubject, MBody)
                Dim mailClient As New System.Net.Mail.SmtpClient()
                Email.IsBodyHtml = True
                If cc <> "" Then
                    Email.CC.Add(cc)
                End If

                If bcc <> "" Then
                    Email.Bcc.Add(bcc)
                End If
                'ds.Tables("workflow").Rows(i).Item("wfid").ToString()
                Dim basicAuthenticationInfo As New System.Net.NetworkCredential(Convert.ToString(dtSch.Rows(0).Item("mdmailid")), Convert.ToString(dtSch.Rows(0).Item("mdpwd")))
                mailClient.Host = dtSch.Rows(0).Item("hostname")
                mailClient.UseDefaultCredentials = False
                mailClient.Credentials = basicAuthenticationInfo
                'mailClient.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.PickupDirectoryFromIis
                'Code for attachment dated 7/25/2018 by Arpit
                'start
                If attachementListString <> "" Then
                    Try
                        Dim docspath As String = HostingEnvironment.MapPath("~/Docs/")
                        Dim attachmentNames As String() = attachementListString.Split(New Char() {","c})
                        Dim attachmentName As String
                        Dim index As Integer = 1
                        Dim ListOfDisplayNames As String() = listOfDisplayNameForAttachments.Split(New Char() {","c})



                        For Each attachmentName In attachmentNames

                            If attachmentName <> "" Then

                                If File.Exists(docspath + attachmentName) Then

                                    Dim attachmentToBeAdded As New Attachment(docspath + attachmentName)
                                    Dim extension As String = Path.GetExtension(attachmentName)
                                    'attachmentToBeAdded.ContentDisposition.FileName = EID.ToString() + "_" + tid.ToString() + "_" + index.ToString() + extension
                                    attachmentToBeAdded.ContentDisposition.FileName = ListOfDisplayNames.GetValue(index - 1) + extension

                                    Email.Attachments.Add(attachmentToBeAdded)

                                    index = index + 1

                                End If



                            End If
                        Next
                    Catch ex As Exception
                    End Try

                End If
                'end
                dtSch.Dispose()
                con.Close()
                da.Dispose()
                Try
                    mailClient.Send(Email)
                Catch ex As Exception
                    Exit Sub
                End Try
            Else
                Dim Email As New System.Net.Mail.MailMessage("MYNDSAAS<no-reply@myndsol.com>", Mto, MSubject, MBody)
                Dim mailClient As New System.Net.Mail.SmtpClient()
                Email.IsBodyHtml = True
                If cc <> "" Then
                    Email.CC.Add(cc)
                End If

                If bcc <> "" Then
                    Email.Bcc.Add(bcc)
                End If
                Dim basicAuthenticationInfo As New System.Net.NetworkCredential("no-reply@myndsol.com", "vc79aK123AJ&$kL0")
                mailClient.Host = "mail.myndsol.com"
                mailClient.UseDefaultCredentials = False
                mailClient.Credentials = basicAuthenticationInfo
                'mailClient.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.PickupDirectoryFromIis
                If attachementListString <> "" Then
                    Try
                        Dim docspath As String = HostingEnvironment.MapPath("~/Docs/")
                        Dim ListOfDisplayNames As String() = listOfDisplayNameForAttachments.Split(New Char() {","c})
                        Dim attachmentNames As String() = attachementListString.Split(New Char() {","c})
                        Dim attachmentName As String
                        Dim index As Integer = 1

                        For Each attachmentName In attachmentNames

                            If attachmentName <> "" Then

                                If File.Exists(docspath + attachmentName) Then

                                    Dim attachmentToBeAdded As New Attachment(docspath + attachmentName)

                                    Dim extension As String = Path.GetExtension(attachmentName)
                                    attachmentToBeAdded.ContentDisposition.FileName = ListOfDisplayNames.GetValue(index - 1) + extension

                                    Email.Attachments.Add(attachmentToBeAdded)

                                    index = index + 1

                                End If



                            End If
                        Next
                    Catch ex As Exception
                    End Try

                End If
                Try
                    mailClient.Send(Email)
                Catch ex As Exception
                    Exit Sub
                End Try
            End If


        Catch ex As Exception
            Exit Sub
        End Try
    End Sub

    Public Sub DEFAULT_TEMPLATE(ByVal UID As Integer, ByVal EN As String, ByVal SUBEVENT As String)
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim MAILID As String
        Dim DA As SqlDataAdapter = New SqlDataAdapter("", con)
        'try Catch Block Added by Ajeet Kumar :Date::22 May 2014
        Try
            Dim mSub As String = ""
            Dim bBody As New StringBuilder()
            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            DA.SelectCommand.CommandText = "SELECT EMailid FROM MMM_MST_USER WHERE UID=" & UID & ""
            MAILID = DA.SelectCommand.ExecuteScalar()
            Select Case SUBEVENT
                Case "USER CREATED"
                    mSub = "Welcome to MyeDMS"
                    bBody.Append("<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Transitional//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd""><html xmlns=""http://www.w3.org/1999/xhtml""><head>    <title>Welcome To MyeDMS</title></head><body style=""margin: 10px; padding: 10px;     font: 11px/1.6em  Verdana, Tahoma, Arial,sans-serif; background-color:#fff;color: #555;text-align: center;""><table cellspacing=""0px"" cellpadding=""0px"" width=""100%""><tr><td style=""width:20%"">&nbsp;</td><td style=""width:60%;text-align:left ""><div style=""background-color:#fbc39b;text-align:left;width:100%;padding:20px;border:3px double green""><br />Dear <b>")
                    bBody.Append(MAILID)
                    bBody.Append("</b>,<br /> <br />Greetings from Mynd Solutions Pvt. Ltd.<br /><br />A new account has been created for you at <b>http://myedms.myndsolution.com</b> <br /> <br />Please Follow below steps for using your account-")
                    bBody.Append("<br /><br /><b>Step 1 : </b>Click on following link to activate your account.<br />")
                    bBody.Append("<a href=""http://MyeDMS.myndsolution.com/activate.aspx?UID=JSHUDYHG675GHJSDUY8976KHJSUHDG&UIID=0MOJHUKSHGYDTEGJSJAHSGDVJKJJHGDU&UIIID=0QWERASDFZXCVYTHGNB" & UID.ToString() & """>http://myedms.myndsolution.com/activate.aspx?UID=JSHUDYHG675GHJSDUY8976KHJSUHDG&UIID=MOJHUKSHGYDTEGJSJAHSGDVJKJJHGDU&UIIID=0QWERASDFZXCVYTHGNB" & UID.ToString() & "</a>")
                    bBody.Append("<br /><br /><b>Step 2 : </b>This will take you to Account Activated web page. There follow the instruction to go to login page.<br />")
                    bBody.Append("<br /><b>Step 3 : </b>Use the following credentials for Login:<br /> <br />Email Address (LoginID):")
                    'bBody.Append(txtProductDesc.Text & "<br />Password:" & pwd & "<br /> <br />")
                    ' NOT NEED TO SENT PASSWORD 
                    bBody.Append("<br /><b>Step 4 : </b>After logging in, Select <b>Explorer</b> option under <b>Tools</b> Menu. In explorer page – you can view/download documents assigned to you")
                    bBody.Append("<br /><br /><br /><br />Regards,<br />MyeDMS Team<br />www.myndsol.com</div></td><td style=""width:20%"">&nbsp;</td></tr></table></body></html>")
                Case "FORGET PASSWORD"
                    mSub = "MyeDMS Password Reset"
                    bBody.Append("<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Transitional//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd""><html xmlns=""http://www.w3.org/1999/xhtml""><head>    <title>Welcome To MyeDMS</title></head><body><table style=""height: 172px; width: 573px""><tr><td align=""center"" colspan=""3"" style=""background-color:#D0F5A9;"">Myedms Password Reset </td></tr>  <tr><td colspan=""3"">Visit the following URL  to set a new Password </td></tr>")
                    bBody.Append("<br/>")
                    bBody.Append("<tr><td colspan=""3""><a href=""http://MyeDMS.myndsolution.com/activate.aspx?UID=JSHUDYHG675GHJSDUY8976KHJSUHDG&UIID=MOJHUKSHGYDTEGJSJAHSGDVJKJJHGDU&UIIID=1QWERASDFZXCVYTHGNB" & UID & """>http://myedms.myndsolution.com/activate.aspx?UID=JSHUDYHG675GHJSDUY8976KHJSUHDG&UIID=MOJHUKSHGYDTEGJSJAHSGDVJKJJHGDU&UIIID=1QWERASDFZXCVYTHGNB" & UID & "</a></td></tr>")
                    bBody.Append("<tr><td colspan=""3"">You can do Regular Login at <a href=""http://localhost:4116/MyeDMS/Default.aspx"">http://localhost:4116/MyeDMS/Default.aspx</a></td></tr>")
                    bBody.Append("<br/><br/>")
                    bBody.Append("<tr><td align=""center"" colspan=""3"" style=""background-color:#D0F5A9;"">This email is Service from Mynd Solution MyeDms Service</td></tr>")
            End Select
            ' sendMail(MAILID, mSub, bBody.ToString())
            Dim obj As New MailUtill(eid:=100)
            obj.SendMail(ToMail:=MAILID, Subject:=mSub, MailBody:=bBody.ToString(), CC:="", BCC:="")

            con.Close()
            DA.Dispose()
        Catch ex As Exception
        Finally
            If Not DA Is Nothing Then
                DA.Dispose()
            End If
            If Not con Is Nothing Then
                con.Close()
                con.Dispose()
            End If
        End Try

    End Sub

    Public Sub DEFAULT_FILE_TEMPLATE(ByVal TID As Integer, ByVal EN As String, ByVal SUBEVENT As String)
        Select Case EN
            Case "FILE"
                Select Case SUBEVENT
                    Case "CREATED"
                        SendMailOfChanges("FILEUPLOADED", TID.ToString())
                    Case "DELETED"
                        SendMailOfChanges("FILEDELETED", TID.ToString())
                End Select
            Case "FOLDER"

        End Select
    End Sub


    '' old before multi mail temp. on on stage enh. by sp on 16-jul-15
    'Public Sub TemplateCalling(ByVal tid As Integer, ByVal eid As Integer, ByVal en As String, ByVal SUBEVENT As String)
    '    Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
    '    Dim con As New SqlConnection(conStr)
    '    Dim STR As String = ""
    '    Dim MAILTO As String = ""
    '    Dim MAILID As String = ""
    '    Dim subject As String = ""
    '    Dim MSG As String = ""
    '    Dim cc As String = ""
    '    Dim Bcc As String = ""
    '    Dim MainEvent As String = ""
    '    Dim fn As String = ""
    '    Dim da As SqlDataAdapter = New SqlDataAdapter("select documenttype,curstatus,curdocnature from MMM_MST_DOC where tid=" & tid, con)
    '    Dim DS As New DataSet
    '    Dim WFstatus As String = "NOT REQUIRED"
    '    Dim curdocnature As String = ""
    '    Dim obj As New MailUtill(eid:=eid)
    '    Dim Filepath As String = HostingEnvironment.MapPath("~/MailAttach/")
    '    'try Catch Block Added by Ajeet Kumar :Date::22 May 2014

    '    Try
    '        da.Fill(DS, "doctype")

    '        If en.Length < 2 Then
    '            en = DS.Tables("doctype").Rows(0).Item("documenttype").ToString()
    '        End If

    '        If DS.Tables("doctype").Rows.Count <> 0 Then
    '            WFstatus = DS.Tables("doctype").Rows(0).Item("curstatus").ToString()
    '        End If

    '        If SUBEVENT = "REJECT" Then
    '            WFstatus = "REJECTED"
    '        End If



    '        If DS.Tables("doctype").Rows.Count <> 0 Then
    '            curdocnature = DS.Tables("doctype").Rows(0).Item("curdocnature").ToString()
    '        End If


    '        If SUBEVENT.ToString.ToUpper = "APMQDM" Then
    '            curdocnature = "MODIFY"
    '            SUBEVENT = "CREATED"
    '        End If

    '        da.SelectCommand.CommandText = "select Code FROM MMM_MST_Entity where EID=" & eid
    '        da.Fill(DS, "eCode")
    '        Dim ECode = DS.Tables("eCode").Rows(0).Item("Code")

    '        da.SelectCommand.CommandText = "select T.TID,T.msgBody,T.subject,T.MAILTO,T.CC,T.BCC,T.EventName,T.qry,T.specialfieldflag from MMM_MST_TEMPLATE T  WHERE T.EVENTNAME='" & en & "' AND T.SUBEVENT='" & SUBEVENT & "' AND t.statusName='" & WFstatus & "' and EID=" & eid & " and t.docnature='" & curdocnature & "' "
    '        da.Fill(DS, "TEMP")
    '        If DS.Tables("TEMP").Rows.Count > 0 Then
    '            MSG = HttpUtility.HtmlDecode(DS.Tables("TEMP").Rows(0).Item("msgbody").ToString())
    '            subject = DS.Tables("TEMP").Rows(0).Item("SUBJECT").ToString()
    '            MAILTO = DS.Tables("TEMP").Rows(0).Item("MAILTO").ToString()
    '            cc = DS.Tables("TEMP").Rows(0).Item("CC").ToString()
    '            Bcc = DS.Tables("TEMP").Rows(0).Item("BCC").ToString()
    '            MainEvent = DS.Tables("TEMP").Rows(0).Item("EVENTNAME").ToString()
    '            STR = DS.Tables("TEMP").Rows(0).Item("qry").ToString()
    '        Else
    '            da.SelectCommand.CommandText = "select T.TID,T.msgBody,T.subject,T.MAILTO,T.CC,T.BCC,T.EventName,T.qry,T.specialfieldflag from MMM_MST_TEMPLATE T  WHERE T.EVENTNAME='" & en & "' AND T.SUBEVENT='" & SUBEVENT & "' AND t.statusName='" & WFstatus & "' AND EID=0 and t.docnature='" & curdocnature & "' "
    '            da.Fill(DS, "TEMP")
    '            If DS.Tables("TEMP").Rows.Count <> 0 Then
    '                MSG = HttpUtility.HtmlDecode(DS.Tables("TEMP").Rows(0).Item("msgbody").ToString())
    '                subject = DS.Tables("TEMP").Rows(0).Item("SUBJECT").ToString()
    '                MAILTO = DS.Tables("TEMP").Rows(0).Item("MAILTO").ToString()
    '                cc = DS.Tables("TEMP").Rows(0).Item("CC").ToString()
    '                Bcc = DS.Tables("TEMP").Rows(0).Item("BCC").ToString()
    '                MainEvent = DS.Tables("TEMP").Rows(0).Item("EVENTNAME").ToString()
    '                STR = DS.Tables("TEMP").Rows(0).Item("qry")
    '            End If
    '        End If
    '        Dim publicLink As String = ""
    '        ' Dim link As String = ConfigurationManager.AppSettings("pubUrl").ToString
    '        Dim ControlCase As String = ""
    '        If DS.Tables("TEMP").Rows.Count <> 0 Then
    '            STR &= " WHERE TID=" & tid & ""
    '            da.SelectCommand.CommandText = STR
    '            da.Fill(DS, "qry")
    '            For Each dc As DataColumn In DS.Tables("qry").Columns
    '                fn = "{" & dc.ColumnName.ToString() & "}"
    '                MSG = MSG.Replace(fn, DS.Tables("qry").Rows(0).Item(dc.ColumnName).ToString())
    '                subject = subject.Replace(fn, DS.Tables("qry").Rows(0).Item(dc.ColumnName).ToString())
    '                MAILTO = MAILTO.Replace(fn, DS.Tables("qry").Rows(0).Item(dc.ColumnName).ToString())
    '                cc = cc.Replace(fn, DS.Tables("qry").Rows(0).Item(dc.ColumnName).ToString())
    '            Next
    '            ' added public access link by balli
    '            Dim Pdffile As String = ""
    '            Dim RES As Integer = 0
    '            If UCase(en) <> "STATIC SURVEY FORM" Then
    '                For i As Integer = 0 To DS.Tables("TEMP").Rows.Count - 1
    '                    If Val(DS.Tables("TEMP").Rows(i).Item("specialFieldflag").ToString) = 1 Then
    '                        da.SelectCommand.CommandText = "select * from MMM_MST_Template_extrafields where tempid=" & Val(DS.Tables("TEMP").Rows(i).Item("TID")) & ""
    '                        da.SelectCommand.CommandType = CommandType.Text
    '                        da.Fill(DS, "TempExtrafld")
    '                        For j As Integer = 0 To DS.Tables("TempExtrafld").Rows.Count - 1
    '                            ControlCase = DS.Tables("TempExtrafld").Rows(j).Item("controlName").ToString()
    '                            Select Case ControlCase
    '                                Case "{DOCUMENT PUBLIC VIEW LINK}"
    '                                    If DS.Tables("TempExtrafld").Rows(j).Item("PvMode").ToString() = "EDIT" Then
    '                                        If DS.Tables("TempExtrafld").Rows(j).Item("PvRelationship").ToString().ToUpper = "DOCID" Then
    '                                            publicLink = "http://" & ECode & ".myndsaas.com/publicdocument.aspx?EID=" & eid & "&PvDoctype=" & DS.Tables("TempExtrafld").Rows(j).Item("PvDoctype").ToString() & "&date=" & Date.Now & "&docid=" & tid & "&docRef=0" & "emailId=" & MAILTO
    '                                        Else
    '                                            publicLink = "http://" & ECode & ".myndsaas.com/publicdocument.aspx?EID=" & eid & "&PvDoctype=" & DS.Tables("TempExtrafld").Rows(j).Item("PvDoctype").ToString() & "&date=" & Date.Now & "&docid=0&docRef=" & tid & "&emailId=" & MAILTO
    '                                        End If
    '                                    Else
    '                                        publicLink = "http://" & ECode & ".myndsaas.com/publicdocument.aspx?EID=" & eid & "&PvDoctype=" & DS.Tables("TempExtrafld").Rows(j).Item("PvDoctype").ToString() & "&date=" & Date.Now & "&docid=0&docRef=0 " & "emailId = " & MAILTO
    '                                    End If
    '                                    publicLink = "<a href='" & publicLink & "'> " & DS.Tables("TempExtrafld").Rows(j).Item("PvLinkCaption").ToString() & "</a>"
    '                                    Exit For
    '                            End Select
    '                        Next
    '                    End If
    '                Next
    '                MSG = Replace(MSG, ControlCase, publicLink)


    '                Dim mailevent As String = en & "-" & SUBEVENT
    '                da.SelectCommand.Parameters.Clear()
    '                da.SelectCommand.CommandText = "INSERT_MAILLOG"
    '                da.SelectCommand.CommandType = CommandType.StoredProcedure
    '                da.SelectCommand.Parameters.AddWithValue("@MAILTO", MAILTO)
    '                da.SelectCommand.Parameters.AddWithValue("@CC", cc)
    '                da.SelectCommand.Parameters.AddWithValue("@MSG", MSG)
    '                da.SelectCommand.Parameters.AddWithValue("@ALERTTYPE", "MAIL")
    '                da.SelectCommand.Parameters.AddWithValue("@MAILEVENT", mailevent)
    '                da.SelectCommand.Parameters.AddWithValue("@EID", eid)

    '                If con.State <> ConnectionState.Open Then
    '                    con.Open()
    '                End If
    '                RES = da.SelectCommand.ExecuteScalar()
    '            End If
    '            If MSG.Contains("MailAttach") Then
    '                Dim da1 As SqlDataAdapter = New SqlDataAdapter("select * from MMM_Print_Template where documenttype='" & en & "' and draft='original'", con)
    '                Dim ds1 As New DataSet
    '                da1.Fill(ds1, "dataset")
    '                da.SelectCommand.CommandType = CommandType.Text
    '                da.SelectCommand.CommandText = "select formcaption from mmm_mst_forms where formname='" & en & "' and eid=" & eid
    '                da.Fill(ds1, "dataset1")
    '                For k As Integer = 0 To ds1.Tables("dataset").Rows.Count - 1
    '                    If ds1.Tables("dataset").Rows(k).Item("SendType").ToString() = "EXCEL" Then
    '                    Else
    '                        Pdffile = GenerateMailPdf(ds1.Tables("dataset1").Rows(0).Item(0).ToString() & "_" & eid & "_" & "print" & ds1.Tables("dataset").Rows(k).Item("tid").ToString(), tid, en, "NA", curdocnature)
    '                    End If


    '                    If Not Pdffile Is Nothing And Not String.IsNullOrEmpty(Pdffile) And ds1.Tables("dataset").Rows(k).Item("SendType").ToString() = "PDF" Then
    '                        Pdffile = HostingEnvironment.MapPath("~/MailAttach/" & Pdffile & ".pdf")
    '                    ElseIf ds1.Tables("dataset").Rows(k).Item("SendType").ToString() = "EXCEL" Then
    '                        Pdffile = ""
    '                        Dim qry As String = ds1.Tables("dataset").Rows(k).Item("qry").ToString()
    '                        Dim exlsub As String = ds1.Tables("dataset").Rows(k).Item("ExlSubject").ToString()
    '                        Dim exlmsg As String = ds1.Tables("dataset").Rows(k).Item("ExlMailBody").ToString()
    '                        qry = Replace(qry, "@tid", tid)
    '                        da.SelectCommand.CommandText = qry
    '                        da.SelectCommand.CommandType = CommandType.Text
    '                        Dim exceldata As New DataTable
    '                        da.Fill(exceldata)
    '                        Try
    '                            If exlmsg.Contains("{EnquiryNumber}") = True Then
    '                                da.SelectCommand.CommandText = "select m.fld1[EnquiryNumber] from mmm_mst_master m left outer join mmm_mst_doc d on  m.documenttype='enquiry master' and m.eid=" & eid & " and m.tid=d.fld1  where d.tid=" & tid & " and d.documenttype='" & en & "' and d.eid=" & eid
    '                                da.Fill(ds1, "datamsg")
    '                                Dim enqnum As String = ds1.Tables("datamsg").Rows(0).Item("EnquiryNumber").ToString()
    '                                exlmsg = exlmsg.Replace("{EnquiryNumber}", enqnum)
    '                            End If
    '                            If exlmsg.Contains("{DocNumber}") = True Then
    '                                exlmsg = exlmsg.Replace("{DocNumber}", tid)
    '                            End If

    '                            If ds1.Tables("dataset").Rows(k).Item("iscustomer").ToString() = "1" Then
    '                                Dim cm As String = DS.Tables("qry").Rows(0).Item("OWNER EMAIL").ToString
    '                                MAILTO = Replace(MAILTO, cm, "")
    '                                MAILTO = MAILTO.Trim().Substring(0, MAILTO.Length - 1)

    '                                'sendMailExl(MAILTO, cc, Bcc, exlsub, exlmsg, CreateCSVR(exceldata, exlsub))

    '                                obj.SendMail(ToMail:=MAILTO, Subject:=exlsub, MailBody:=exlmsg, CC:=cc, Attachments:=Filepath + CreateCSVR(exceldata, exlsub), BCC:=Bcc)

    '                                MAILTO = MAILTO & "," & cm
    '                            Else
    '                                ' ExportToPDF("ravi.sharma@myndsol.com", cc, Bcc, exlsub, exlmsg, exceldata, tid, eid)
    '                                obj.SendMail(ToMail:=MAILTO, Subject:=exlsub, MailBody:=exlmsg, CC:=cc, Attachments:=Filepath + CreateCSVR(exceldata, exlsub), BCC:=Bcc)
    '                            End If

    '                        Catch ex As Exception
    '                        End Try

    '                    ElseIf ds1.Tables("dataset").Rows(k).Item("SendType").ToString() = "FIX" Then
    '                        Pdffile = ""
    '                        Dim qry As String = ds1.Tables("dataset").Rows(k).Item("qry").ToString()
    '                        Dim exlsub As String = ds1.Tables("dataset").Rows(k).Item("ExlSubject").ToString()
    '                        Dim strmsgBdy As String = ds1.Tables("dataset").Rows(k).Item("ExlMailBody").ToString()
    '                        Dim EMailto As String = ds1.Tables("dataset").Rows(k).Item("emailto").ToString()
    '                        Dim cc1 As String = ds1.Tables("dataset").Rows(k).Item("cc").ToString()
    '                        Dim bcc1 As String = ds1.Tables("dataset").Rows(k).Item("bcc").ToString()
    '                        qry = Replace(qry, "@tid", tid)
    '                        da.SelectCommand.CommandText = qry
    '                        da.SelectCommand.CommandType = CommandType.Text
    '                        Dim fxdata As New DataTable
    '                        da.Fill(fxdata)
    '                        If fxdata.Rows.Count > 0 Then
    '                            Dim MailTable As New StringBuilder()
    '                            MailTable.Append("<table border=""1"" width=""100%"">")
    '                            MailTable.Append("<tr style=""background-color:#990000"" Font-Bold=""True""> ")
    '                            For l As Integer = 0 To fxdata.Columns.Count - 1
    '                                MailTable.Append("<td >" & fxdata.Columns(l).ColumnName & "</td>")
    '                            Next

    '                            For m As Integer = 0 To fxdata.Rows.Count - 1 ' binding the tr tab in table
    '                                MailTable.Append("</tr><tr>") ' for row records
    '                                For t As Integer = 0 To fxdata.Columns.Count - 1
    '                                    MailTable.Append("<td>" & fxdata.Rows(m).Item(t).ToString() & " </td>")
    '                                Next
    '                                MailTable.Append("</tr>")
    '                            Next
    '                            MailTable.Append("</table>")

    '                            If strmsgBdy.Contains("@body") Then
    '                                strmsgBdy = Replace(strmsgBdy, "@body", MailTable.ToString())
    '                                MSG = strmsgBdy
    '                            Else
    '                                MSG = MailTable.ToString()
    '                            End If
    '                            MSG = strmsgBdy
    '                            ' sendMail1(EMailto, cc1, bcc1, exlsub, MSG)
    '                            obj.SendMail(ToMail:=EMailto, Subject:=exlsub, MailBody:=MSG, CC:=cc1, BCC:=Bcc)
    '                            MSG.Replace("{MailAttach}", "")
    '                        End If


    '                        ' webservice call start

    '                        'ElseIf ds1.Tables("dataset").Rows(k).Item("SendType").ToString() = "WS" Then

    '                        '    Try
    '                        '        Dim ws As New WSOutward()
    '                        '        Dim URLIST As String = ws.WBSREPORT(en, eid, tid)
    '                        '    Catch ex As Exception

    '                        '    End Try

    '                        'web service call end



    '                    Else
    '                        Pdffile = ""
    '                    End If
    '                    If MSG.Contains("MailAttach") And UCase(en) <> "STATIC SURVEY FORM" And UCase(en) <> "COMPETITION GUARD INFORMATION" = True Then
    '                        MSG.Replace("{MailAttach}", "")
    '                        ' SendMailPdf(MAILTO, cc, Bcc, subject, MSG, Pdffile)
    '                        obj.SendMail(ToMail:=MAILTO, Subject:=subject, MailBody:=MSG, CC:=cc, Attachments:=Pdffile, BCC:=Bcc)
    '                    End If
    '                Next
    '            Else
    '                '' mail without attachment  12_june
    '                'sendMail1(MAILTO, cc, Bcc, subject, MSG)
    '                obj.SendMail(ToMail:=MAILTO, Subject:=subject, MailBody:=MSG, CC:=cc, BCC:=Bcc)
    '            End If

    '        End If
    '        DS.Dispose()
    '    Catch ex As Exception
    '    Finally
    '        If Not da Is Nothing Then
    '            da.Dispose()
    '        End If
    '        If Not con Is Nothing Then
    '            con.Close()
    '            con.Dispose()
    '        End If
    '    End Try
    'End Sub

    Public Sub TemplateCalling(ByVal tid As Integer, ByVal eid As Integer, ByVal en As String, ByVal SUBEVENT As String, Optional ByVal upCommingFrom As String = "", Optional ByVal TicketCC As String = "")
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim objDC As New DataClass()
        Dim con As New SqlConnection(conStr)
        Dim code As String = ""
        Dim STR As String = ""
        Dim MAILTO As String = ""
        Dim MAILID As String = ""
        Dim subject As String = ""
        Dim MSG As String = ""
        Dim cc As String = ""
        Dim Bcc As String = ""
        Dim MainEvent As String = ""
        Dim fn As String = ""
        Dim STR_child1 As String = ""
        Dim STR_child2 As String = ""
        Dim da As SqlDataAdapter = New SqlDataAdapter("select documenttype,curstatus,curdocnature from MMM_MST_DOC where tid=" & tid, con)
        Dim DS As New DataSet
        Dim WFstatus As String = "NOT REQUIRED"
        Dim curdocnature As String = ""
        code = objDC.ExecuteQryScaller("select code from mmm_mst_entity where eid=" & eid)
        Dim obj As New MailUtill(eid:=eid)
        Dim Filepath As String = HostingEnvironment.MapPath("~/MailAttach/")
        'try Catch Block Added by Ajeet Kumar :Date::22 May 2014
        Dim fieldsForAttachment As String = "" 'added by Arpit for Attachment Work on 7/25/2018
        Dim listOfAttachmentString As String = "" 'added by Arpit for Attachment Work on 7/25/2018
        Dim listOfDisplayNameForAttachments As String = "" 'added by Arpit for Attachment Work on 7/25/2018

        Try
            da.Fill(DS, "doctype")

            If en.Length < 2 Then
                en = DS.Tables("doctype").Rows(0).Item("documenttype").ToString()
            End If

            If DS.Tables("doctype").Rows.Count <> 0 Then
                WFstatus = DS.Tables("doctype").Rows(0).Item("curstatus").ToString()
            End If

            If SUBEVENT = "REJECT" Then
                WFstatus = "REJECTED"
            End If


            If DS.Tables("doctype").Rows.Count <> 0 Then
                curdocnature = DS.Tables("doctype").Rows(0).Item("curdocnature").ToString()
            End If


            If SUBEVENT.ToString.ToUpper = "APMQDM" Then
                curdocnature = "MODIFY"
                SUBEVENT = "CREATED"
            End If

            da.SelectCommand.CommandText = "select Code FROM MMM_MST_Entity where EID=" & eid
            da.Fill(DS, "eCode")
            Dim ECode = DS.Tables("eCode").Rows(0).Item("Code")
            'commented by Arpit for Attachment Task on 7/25/2018
            'da.SelectCommand.CommandText = "select T.TID,T.msgBody,T.subject,T.MAILTO,T.CC,T.BCC,T.EventName,T.qry,T.specialfieldflag,isnull(T.condition,'') as condition from MMM_MST_TEMPLATE T  WHERE T.EVENTNAME='" & en & "' AND T.SUBEVENT='" & SUBEVENT & "' AND t.statusName='" & WFstatus & "' and EID=" & eid & " and t.docnature='" & curdocnature & "' "
            'added by Arpit for Attachment Task on 7/25/2018
            da.SelectCommand.CommandText = "select T.TID,T.msgBody,T.subject,T.MAILTO,T.CC,T.BCC,T.EventName,T.qry,T.qry_child1,T.qry_child2,T.specialfieldflag,isnull(T.condition,'') as condition, T.AttachmentFields from MMM_MST_TEMPLATE T  WHERE T.EVENTNAME='" & en & "' AND T.SUBEVENT='" & SUBEVENT & "' AND t.statusName='" & WFstatus & "' and EID=" & eid & " and t.docnature='" & curdocnature & "' "
            da.Fill(DS, "TEMP")
            If DS.Tables("TEMP").Rows.Count > 0 Then
                MSG = HttpUtility.HtmlDecode(DS.Tables("TEMP").Rows(0).Item("msgbody").ToString())
                subject = DS.Tables("TEMP").Rows(0).Item("SUBJECT").ToString()
                MAILTO = DS.Tables("TEMP").Rows(0).Item("MAILTO").ToString()
                cc = DS.Tables("TEMP").Rows(0).Item("CC").ToString()
                Bcc = DS.Tables("TEMP").Rows(0).Item("BCC").ToString()
                MainEvent = DS.Tables("TEMP").Rows(0).Item("EVENTNAME").ToString()
                STR = DS.Tables("TEMP").Rows(0).Item("qry").ToString()
                fieldsForAttachment = DS.Tables("TEMP").Rows(0).Item("AttachmentFields").ToString() 'added by Arpit for Attachment Task on 7/25/2018                
            Else
                'commented by Arpit for Attachment Task on 7/25/2018
                'da.SelectCommand.CommandText = "select T.TID,T.msgBody,T.subject,T.MAILTO,T.CC,T.BCC,T.EventName,T.qry,T.specialfieldflag, isnull(T.condition,'') as condition from MMM_MST_TEMPLATE T  WHERE T.EVENTNAME='" & en & "' AND T.SUBEVENT='" & SUBEVENT & "' AND t.statusName='" & WFstatus & "' AND EID=0 and t.docnature='" & curdocnature & "' "
                'added by Arpit for Attachment Task on 7/25/2018
                da.SelectCommand.CommandText = "select T.TID,T.msgBody,T.subject,T.MAILTO,T.CC,T.BCC,T.EventName,T.qry,T.qry_child1,T.qry_child2,T.specialfieldflag, isnull(T.condition,'') as condition,T.AttachmentFields from MMM_MST_TEMPLATE T  WHERE T.EVENTNAME='" & en & "' AND T.SUBEVENT='" & SUBEVENT & "' AND t.statusName='" & WFstatus & "' AND EID=0 and t.docnature='" & curdocnature & "' "
                da.Fill(DS, "TEMP")
                If DS.Tables("TEMP").Rows.Count <> 0 Then
                    MSG = HttpUtility.HtmlDecode(DS.Tables("TEMP").Rows(0).Item("msgbody").ToString())
                    subject = DS.Tables("TEMP").Rows(0).Item("SUBJECT").ToString()
                    MAILTO = DS.Tables("TEMP").Rows(0).Item("MAILTO").ToString()
                    cc = DS.Tables("TEMP").Rows(0).Item("CC").ToString()
                    Bcc = DS.Tables("TEMP").Rows(0).Item("BCC").ToString()
                    MainEvent = DS.Tables("TEMP").Rows(0).Item("EVENTNAME").ToString()
                    STR = DS.Tables("TEMP").Rows(0).Item("qry")
                    fieldsForAttachment = DS.Tables("TEMP").Rows(0).Item("AttachmentFields").ToString() 'added by Arpit for Attachment Task on 7/25/2018                 
                End If
            End If
            Dim publicLink As String = ""
            ' Dim link As String = ConfigurationManager.AppSettings("pubUrl").ToString
            Dim ControlCase As String = ""
            If DS.Tables("TEMP").Rows.Count <> 0 Then
                For rindex As Integer = 0 To DS.Tables("TEMP").Rows.Count - 1 'Loop for multiple mailing templates on same wfstatus and same subevent starts here (Pallavi)
                    'added by pallavi for multiple temps on 15 July 15
                    Dim sendmailflag As Boolean = True
                    If Not String.IsNullOrEmpty(DS.Tables("TEMP").Rows(rindex).Item("condition")) Then
                        Dim dtresult As New DataTable
                        dtresult = objDC.ExecuteQryDT(" select count(*) from mmm_mst_doc  where tid=" & tid & " and " & Convert.ToString(DS.Tables("TEMP").Rows(rindex).Item("condition")))
                        If Convert.ToInt32(dtresult.Rows(0)(0)) = 0 Then
                            sendmailflag = False
                        End If
                    End If
                    If sendmailflag Then
                        MSG = HttpUtility.HtmlDecode(DS.Tables("TEMP").Rows(rindex).Item("msgbody").ToString())
                        subject = DS.Tables("TEMP").Rows(rindex).Item("SUBJECT").ToString()
                        MAILTO = DS.Tables("TEMP").Rows(rindex).Item("MAILTO").ToString()
                        cc = DS.Tables("TEMP").Rows(rindex).Item("CC").ToString()
                        Bcc = DS.Tables("TEMP").Rows(rindex).Item("BCC").ToString()
                        MainEvent = DS.Tables("TEMP").Rows(rindex).Item("EVENTNAME").ToString()
                        STR = DS.Tables("TEMP").Rows(rindex).Item("qry").ToString()
                        fieldsForAttachment = DS.Tables("TEMP").Rows(rindex).Item("AttachmentFields").ToString() 'added by Arpit for Attachment Task on 7/25/2018
                        'added by pallavi for multiple temps on 15 July 15
                        STR_child1 = DS.Tables("TEMP").Rows(rindex).Item("qry_child1").ToString() ''New code jayant
                        STR_child2 = DS.Tables("TEMP").Rows(rindex).Item("qry_child2").ToString() '' new code jayant

                        STR &= " WHERE TID=" & tid & ""

                        Dim daqry As New SqlDataAdapter(STR, con)
                        Dim dtqry As New DataTable()
                        daqry.Fill(dtqry)
                        For Each dc As DataColumn In dtqry.Columns
                            fn = "{" & dc.ColumnName.ToString() & "}"
                            'If Not fn.ToString().ToUpper.Contains("{AGENT COMMENTS}") Then
                            MSG = MSG.Replace(fn, dtqry.Rows(0).Item(dc.ColumnName).ToString())
                            'End If
                            subject = subject.Replace(fn, dtqry.Rows(0).Item(dc.ColumnName).ToString())
                            MAILTO = MAILTO.Replace(fn, dtqry.Rows(0).Item(dc.ColumnName).ToString())
                            cc = cc.Replace(fn, dtqry.Rows(0).Item(dc.ColumnName).ToString())
                        Next
                        ' add child item code ends here by Jayant + SP
                        Try
                            If Not String.IsNullOrEmpty(STR_child1) Then
                                STR_child1 &= " WHERE DocID=" & tid & "" '' new code jayant
                                '  STR_child2 &= " WHERE DocID=" & tid & "" '' new code jayant
                                Dim dtDtl As New DataTable
                                Dim dtDocs As New DataTable
                                Dim dtMail As New DataTable

                                Dim dtQry_child As New DataTable
                                da.SelectCommand.Parameters.Clear()
                                da.SelectCommand.CommandType = CommandType.Text
                                da.SelectCommand.CommandText = STR_child1
                                da.Fill(dtQry_child)

                                Dim NoofTimes As Integer = 0
                                NoofTimes = 3
                                For Cnt As Integer = 1 To NoofTimes
                                    Dim Spos As Integer = MSG.IndexOf("{@table" & Cnt & "}")
                                    Dim Epos As Integer = MSG.IndexOf("{@/table" & Cnt & "}") 'MSG.IndexOf("{@/table1}")
                                    If Spos > 0 And Epos > 0 Then
                                        Dim TblText As String = MSG.Substring(Spos + 9, (Epos - (Spos + 9)))
                                        Dim ArrVars() As String = TblText.Split("|")

                                        'Dim MailBody As String = "<table width=""100%"" border=""3px"" cellpadding=""2px"" border=""1"" cellspacing=""2px""> <thead> <tr>"
                                        Dim MailBody As String = "<table border=""0"" cellspacing=""0"" cellpadding=""0"" width=""550"" style=""border-spacing:0;font-family:sans-serif;color:#333333;font-family:'Open Sans', Helvetica, Arial, sans-serif;font-size:14px;border:1px solid #d0d0d0;border-bottom:none;width:550px""> <thead> <tr>"

                                        Dim sI As Integer = 0, eI As Integer = 0
                                        Dim ColValue As String = "Header"
                                        For m As Integer = 0 To ArrVars.Length - 1
                                            Dim ArrCaption() As String = ArrVars(m).Split("&")
                                            sI = ArrVars(m).IndexOf("{")
                                            eI = ArrVars(m).IndexOf("}")
                                            ArrVars(m) = ArrVars(m).Substring(sI + 1, (eI - sI) - 1)
                                            If ArrCaption.Length > 1 Then
                                                ColValue = ArrCaption(1).ToString
                                            Else
                                                ColValue = ArrCaption(0).ToString
                                                sI = ColValue.IndexOf("{")
                                                eI = ColValue.IndexOf("}")
                                                ColValue = ColValue.Substring(sI + 1, (eI - sI) - 1)
                                            End If
                                            ' Prev MailBody &= "<td style=""padding:9px 10px 9px 15px;border-bottom:1px solid #d0d0d0;border-right:1px solid #d0d0d0;text-align:left;"">" & ArrVars(m).ToString & " </td>"
                                            MailBody &= "<td style=""padding:9px 10px 9px 15px;border-bottom:1px solid #d0d0d0;border-right:1px solid #d0d0d0;text-align:left;""> <b> " & ColValue & " </b>  </td>"
                                        Next
                                        MailBody &= " </tr> </thead><tbody> "

                                        For j As Integer = 0 To dtQry_child.Rows.Count - 1 '''' this is for no. of rows of docs - outer most
                                            MailBody &= "<tr> "
                                            For m As Integer = 0 To ArrVars.Length - 1
                                                'Dim Colnm As String = ArrVars(m).Substring(1, ArrVars(m).Length - 1)
                                                Dim Colnm As String = ArrVars(m)
                                                If dtQry_child.Columns.Contains(Colnm) Then
                                                    MailBody &= "<td style=""padding:9px 10px 9px 15px;border-bottom:1px solid #d0d0d0;border-right:1px solid #d0d0d0;text-align:left;""> " & "" & dtQry_child.Rows(j).Item(Colnm).ToString() & "</td>"
                                                End If
                                            Next
                                            MailBody &= "</tr>"
                                        Next
                                        MailBody &= "</tbody> </table>"
                                        '' code here to replace between {@TABLE} and {@/TABLE}
                                        Spos = MSG.IndexOf("{@table" & Cnt & "}")  ' MSG.IndexOf("{@table1}")
                                        Epos = MSG.IndexOf("{@/table" & Cnt & "}")  ' MSG.IndexOf("{@/table1}")
                                        Dim repVal As String = MSG.Substring(Spos, ((Epos + 10) - Spos))
                                        MSG = MSG.Replace(repVal, MailBody)
                                    End If
                                Next
                            End If
                        Catch ex As Exception

                        End Try
                        ' add child item code ends here by Jayant + SP
                        'For Ticket Mail Template
                        If MSG.ToString().ToUpper.Contains("{TICKET AGENT COMMENTS}") Then
                            Dim mailbodyAndAttachment As New StringBuilder()
                            Dim dt = objDC.ExecuteQryDT("select * from MMM_MST_FIELDS where documenttype ='" & en & "' and eid=" & eid & " and mdfieldname='Remarks'")
                            If dt.Rows.Count > 0 Then
                                For Each dr As DataRow In dt.Rows
                                    If Convert.ToString(dr("MDfieldName")).ToUpper = "REMARKS" Then
                                        Dim dttemp As DataTable = objDC.ExecuteQryDT(" select tid, m." & Convert.ToString(dr("FieldMapping")) & ",u.username, convert(nvarchar,adate,109) as creationdate from mmm_mst_history as m inner join mmm_mst_user as u on m.uid=u.uid  where docid=" & tid & " order by tid desc")
                                        For Each drtemp As DataRow In dttemp.Rows
                                            mailbodyAndAttachment.Append("      <b>" & drtemp("username") & "</b> <span style=""font-size:11px !important;"">" & drtemp("creationdate") & "</span> <br/><br/>")
                                            mailbodyAndAttachment.Append(Convert.ToString(drtemp(1)).Replace("''", "'"))
                                            Dim dtAttachment As New DataTable
                                            dtAttachment = objDC.ExecuteQryDT(" declare @col as nvarchar(max) ,@Qry nvarchar(max) select @col= fieldmapping from mmm_mst_fields where documenttype in (select documenttype from mmm_mst_doc_item where  docid=" & tid & ") and eid=" & eid & " and MDfieldName='Attachment'           set @Qry='   select '+  @col +',attachment from mmm_mst_doc_item where sourcetid=" & drtemp("tid") & "' exec sp_executesql @Qry")
                                            Dim attachmentcontent As New StringBuilder()
                                            If dtAttachment.Rows.Count > 0 Then
                                                attachmentcontent.Append("<br/><div  style=""width:auto; height:auto;border:1px solid #ccc; display:inline-block; margin:0px; padding:12px;"">")
                                                For Each drattachment As DataRow In dtAttachment.Rows
                                                    If drattachment(1).ToString().ToUpper.Contains(".PNG") Or drattachment(1).ToString().ToUpper.Contains(".GIF") Or drattachment(1).ToString().ToUpper.Contains(".JPEG") Or drattachment(1).ToString().ToUpper.Contains(".JPG") Or drattachment(1).ToString().ToUpper.Contains(".TIFF") Then
                                                        'attachmentcontent.Append(" <div style=""border:1px solid #ccc; padding:7px; display:inline-block;text-align:center; width:100px;""><img src=""https://" & code & "myndsaas.com/DOCS/" & drattachment(0).ToString() & """ width=""100px"" height=""60px"" alt="" class=""> <input type=""button"" style=""display:block; font-size: 10px; line-height: 10px;  padding: 3px; border: 1px solid #ccc; cursor: pointer; overflow:hidden; width:100%; text-decoration:underline; color: blue; white-space: normal;word-wrap: break-word;"" onclick=""OnChangeCheckbox ('" & "DOCS/" & drattachment(0).ToString() & "')"" value=" & drattachment(1).ToString() & " />  </div>")
                                                        attachmentcontent.Append(" <div style=""border:1px solid #ccc; padding:7px; display:inline-block;text-align:center; width:100px;""><img src=""https://" & code & ".myndsaas.com/DOCS/" & drattachment(0).ToString() & """ width=""100px"" height=""60px"" alt="" > <a style=""display:block; font-size: 10px; line-height: 10px;  padding: 3px; border: 1px solid #ccc; cursor: pointer; overflow:hidden; width:100%; text-decoration:underline; color: blue; white-space: normal;word-wrap: break-word;""  href=""https://" & code & ".myndsaas.com/DOCS/" & drattachment(0).ToString() & """ >" & drattachment(1).ToString() & " </a>  </div>")
                                                    Else
                                                        attachmentcontent.Append(" <div style=""border:1px solid #ccc; padding:7px; display:inline-block;text-align:center; width:100px;""><a  style=""display:block; font-size: 10px; line-height: 10px;  padding: 3px; border: 1px solid #ccc; cursor: pointer; overflow:hidden; width:100%; text-decoration:underline; color: blue; white-space: normal;word-wrap: break-word;"" href=""https://" & code & ".myndsaas.com/DOCS/" & drattachment(0).ToString() & """ >" & drattachment(1).ToString() & " </a>  </div>")
                                                    End If
                                                Next
                                                attachmentcontent.Append("</div>")
                                                mailbodyAndAttachment.Append(attachmentcontent.ToString() & "<hr/>")
                                            Else
                                                mailbodyAndAttachment.Append("<hr/>")
                                            End If
                                        Next
                                    End If
                                Next
                                MSG = MSG.Replace("{TICKET AGENT COMMENTS}", mailbodyAndAttachment.ToString())
                            End If
                        End If
                        'For Ticket Mail Template
                        If MSG.Contains("{ApprovalThroughMail}") Then
                            da.SelectCommand.Parameters.Clear()
                            da.SelectCommand.CommandText = "select userid from mmm_doc_dtl where tid in ( select lasttid from  MMM_MST_DOC where tid=" & tid & ")"
                            If con.State = ConnectionState.Closed Then
                                con.Open()
                            End If
                            Dim USERID As String = ""
                            Dim objEncryptionDescription As New EncryptionDescryption()
                            Dim Skey As Integer = 12345 ' IIf((eid * 65) < 10000, (eid * 65) * 10, (eid * 65))
                            Dim Q1 As String = "DOCID=" & tid
                            Q1 = objEncryptionDescription.EncryptTripleDES(Q1, Skey)
                            'EncryptTripleDES(Q1, eid)
                            Dim Q2 As String = "USERID=" & Convert.ToString(da.SelectCommand.ExecuteScalar())
                            Q2 = objEncryptionDescription.EncryptTripleDES(Q2, Skey)
                            'Q2 = EncryptTripleDES(Q2, eid)
                            Dim Q3 As String = "CURSTATUS=" & WFstatus
                            'Q3 = EncryptTripleDES(Q3, eid)
                            Q3 = objEncryptionDescription.EncryptTripleDES(Q3, Skey)
                            'MSG = MSG.Replace("{ApprovalThroughMail}", "<br/><br/><table cellspacing=""0"" cellpadding=""0""><tr><td align=""center"" width=""300"" height=""40"" bgcolor=""#000091"" style=""-webkit-border-radius: 5px; -moz-border-radius: 5px; border-radius: 5px; color: #ffffff; display: block;""><a href=""http://" & ECode.ToString() & ".myndsaas.com/ActionMailApproval.aspx?Q1=" & Q1 & "&Q2= " & Q2 & "&Q3=" & Q3 & """  style=""font-size:16px; font-weight: bold; font-family: Helvetica, Arial, sans-serif; text-decoration: none; line-height:40px;width:100%; display:inline-block""><span style=""color: #FFFFFF"">Approve Document</span></a></td></tr></table>")
                            '' latest bkup MSG = MSG.Replace("{ApprovalThroughMail}", "<br/><br/><table cellspacing=""0"" cellpadding=""0""><tr><td align=""center"" width=""300"" height=""40"" bgcolor=""#000091"" style=""-webkit-border-radius: 5px; -moz-border-radius: 5px; border-radius: 5px; color: #ffffff; display: block;""><a href=""http://" & ECode.ToString() & ".myndsaas.com/ActionMailApproval.aspx?Q1=" & Q1 & "&Q2= " & Q2 & "&Q3=" & Q3 & """  style=""font-size:16px; font-weight: bold; font-family: Helvetica, Arial, sans-serif; text-decoration: none; line-height:40px;width:100%; display:inline-block""><span style=""color: #FFFFFF"">Approve Document</span></a></td></tr></table>")
                            MSG = MSG.Replace("{ApprovalThroughMail}", "<br/><table cellspacing=""0"" cellpadding=""0""><tr><td align=""center"" width=""200"" height=""35"" bgcolor=""#000091"" style=""-webkit-border-radius: 5px; -moz-border-radius: 5px; border-radius: 5px; color: #ffffff; display: block;""><a href=""http://" & ECode.ToString() & ".myndsaas.com/ActionMailApproval.aspx?Q1=" & Q1 & "&Q2= " & Q2 & "&Q3=" & Q3 & """  style=""font-size:14px; font-weight: bold; font-family: Helvetica, Arial, sans-serif; text-decoration: none; line-height:35px;width:100%; display:inline-block""><span style=""color: #FFFFFF"">APPROVE</span></a></td></tr></table>")
                        End If

                        ' added public access link by balli
                        Dim Pdffile As String = ""
                        Dim RES As Integer = 0
                        If UCase(en) <> "STATIC SURVEY FORM" Then
                            For i As Integer = 0 To DS.Tables("TEMP").Rows.Count - 1
                                If Val(DS.Tables("TEMP").Rows(i).Item("specialFieldflag").ToString) = 1 Then
                                    da.SelectCommand.CommandText = "select * from MMM_MST_Template_extrafields where tempid=" & Val(DS.Tables("TEMP").Rows(i).Item("TID")) & ""
                                    da.SelectCommand.CommandType = CommandType.Text
                                    da.Fill(DS, "TempExtrafld")
                                    For j As Integer = 0 To DS.Tables("TempExtrafld").Rows.Count - 1
                                        ControlCase = DS.Tables("TempExtrafld").Rows(j).Item("controlName").ToString()
                                        Select Case ControlCase
                                            Case "{DOCUMENT PUBLIC VIEW LINK}"
                                                If DS.Tables("TempExtrafld").Rows(j).Item("PvMode").ToString() = "EDIT" Then
                                                    If DS.Tables("TempExtrafld").Rows(j).Item("PvRelationship").ToString().ToUpper = "DOCID" Then
                                                        publicLink = "http://" & ECode & ".myndsaas.com/publicdocument.aspx?EID=" & eid & "&PvDoctype=" & DS.Tables("TempExtrafld").Rows(j).Item("PvDoctype").ToString() & "&date=" & Date.Now & "&docid=" & tid & "&docRef=0" & "emailId=" & MAILTO
                                                    Else
                                                        publicLink = "http://" & ECode & ".myndsaas.com/publicdocument.aspx?EID=" & eid & "&PvDoctype=" & DS.Tables("TempExtrafld").Rows(j).Item("PvDoctype").ToString() & "&date=" & Date.Now & "&docid=0&docRef=" & tid & "&emailId=" & MAILTO
                                                    End If
                                                Else
                                                    publicLink = "http://" & ECode & ".myndsaas.com/publicdocument.aspx?EID=" & eid & "&PvDoctype=" & DS.Tables("TempExtrafld").Rows(j).Item("PvDoctype").ToString() & "&date=" & Date.Now & "&docid=0&docRef=0 " & "emailId = " & MAILTO
                                                End If
                                                publicLink = "<a href='" & publicLink & "'> " & DS.Tables("TempExtrafld").Rows(j).Item("PvLinkCaption").ToString() & "</a>"
                                                Exit For
                                        End Select
                                    Next
                                End If
                            Next
                            MSG = Replace(MSG, ControlCase, publicLink)


                            Dim mailevent As String = en & "-" & SUBEVENT
                            da.SelectCommand.Parameters.Clear()
                            da.SelectCommand.CommandText = "INSERT_MAILLOG"
                            da.SelectCommand.CommandType = CommandType.StoredProcedure
                            da.SelectCommand.Parameters.AddWithValue("@MAILTO", MAILTO)
                            da.SelectCommand.Parameters.AddWithValue("@CC", cc)
                            da.SelectCommand.Parameters.AddWithValue("@MSG", MSG)
                            da.SelectCommand.Parameters.AddWithValue("@ALERTTYPE", "MAIL")
                            da.SelectCommand.Parameters.AddWithValue("@MAILEVENT", mailevent)
                            da.SelectCommand.Parameters.AddWithValue("@EID", eid)
                            da.SelectCommand.Parameters.AddWithValue("@DocID", tid)

                            If con.State <> ConnectionState.Open Then
                                con.Open()
                            End If
                            RES = da.SelectCommand.ExecuteScalar()
                            con.Close()
                        End If

                        'section for Attachment Work added by Arpit on 7/25/2018 @x
                        'start
                        If fieldsForAttachment <> "" Then

                            Try
                                Dim docspath As String = HostingEnvironment.MapPath("~/Docs/")
                                Dim fieldsForAttachmentNames As String() = fieldsForAttachment.Split(New Char() {","c})
                                fieldsForAttachment = ""
                                Dim fieldsForAttachmentName As String
                                Dim index As Integer = 1
                                Dim helper As Boolean = True
                                For Each fieldsForAttachmentName In fieldsForAttachmentNames

                                    Dim fieldParts As String() = fieldsForAttachmentName.Split(New Char() {"|"c})
                                    Dim SingleFieldPart As String
                                    For Each SingleFieldPart In fieldParts
                                        If helper Then
                                            fieldsForAttachment += SingleFieldPart
                                            fieldsForAttachment += ","
                                            helper = False
                                        Else
                                            helper = True
                                            listOfDisplayNameForAttachments += SingleFieldPart
                                            listOfDisplayNameForAttachments += ","


                                        End If

                                    Next
                                Next
                                'If helper Then
                                '    fieldsForAttachment += fieldsForAttachmentName
                                '    fieldsForAttachment += ","
                                '    helper = False
                                'Else
                                '    helper = True
                                '    listOfDisplayNameForAttachments += fieldsForAttachmentName
                                '    listOfDisplayNameForAttachments += ","


                                'End If
                                'If fieldsForAttachmentName <> "" Then

                                '    If File.Exists(docspath + attachmentName) Then

                                '        Dim attachmentToBeAdded As New Attachment(docspath + attachmentName)
                                '        Dim extension As String = Path.GetExtension(attachmentName)
                                '        attachmentToBeAdded.ContentDisposition.FileName = eid.ToString() + "_" + tid.ToString() + "_" + index.ToString() + extension

                                '        Email.Attachments.Add(attachmentToBeAdded)

                                '        index = index + 1

                                '    End If



                                'End If

                            Catch ex As Exception
                            End Try


                            fieldsForAttachment = fieldsForAttachment.Substring(0, fieldsForAttachment.Length - 1)

                            Dim DS_DocumentNames As New DataSet
                            Dim query123 As String = "select  " + fieldsForAttachment + " from mmm_mst_doc where tid = " + tid.ToString()
                            Try
                                Dim da_DocumentNames As SqlDataAdapter = New SqlDataAdapter(query123, con)
                                da_DocumentNames.Fill(DS_DocumentNames, "DocumentsName")

                                ' If DS_DocumentNames.Tables("DocumentsName") IsNot Nothing Then
                                If DS_DocumentNames.Tables("DocumentsName").Rows.Count <> 0 Then


                                    For index = 0 To DS_DocumentNames.Tables("DocumentsName").Columns.Count - 1

                                        Dim attach As String = DS_DocumentNames.Tables("DocumentsName").Rows(0).Item(index).ToString()
                                        If attach IsNot Nothing Then
                                            If attach <> "" Then
                                                listOfAttachmentString += attach
                                                listOfAttachmentString += ","
                                            End If


                                        End If

                                    Next

                                End If

                            Catch ex As Exception
                            End Try
                        End If

                        'end

                        If MSG.Contains("MailAttach") Then
                            Dim da1 As SqlDataAdapter = New SqlDataAdapter("select * from MMM_Print_Template where documenttype='" & en & "' and draft='original' and eid=" & eid, con)
                            Dim ds1 As New DataSet
                            da1.Fill(ds1, "dataset")
                            da.SelectCommand.CommandType = CommandType.Text
                            da.SelectCommand.CommandText = "select formcaption from mmm_mst_forms where formname='" & en & "' and eid=" & eid
                            da.Fill(ds1, "dataset1")
                            For k As Integer = 0 To ds1.Tables("dataset").Rows.Count - 1
                                Pdffile = GenerateMailPdf(ds1.Tables("dataset1").Rows(0).Item(0).ToString() & "_" & eid & "_" & "print" & ds1.Tables("dataset").Rows(k).Item("tid").ToString(), tid, en, "NA", curdocnature)
                                If Not Pdffile Is Nothing And Not String.IsNullOrEmpty(Pdffile) And ds1.Tables("dataset").Rows(k).Item("SendType").ToString() = "PDF" Then
                                    Pdffile = HostingEnvironment.MapPath("~/MailAttach/" & Pdffile & ".pdf")
                                ElseIf ds1.Tables("dataset").Rows(k).Item("SendType").ToString() = "EXCEL" Then
                                    Pdffile = ""
                                    Dim qry As String = ds1.Tables("dataset").Rows(k).Item("qry").ToString()
                                    Dim exlsub As String = ds1.Tables("dataset").Rows(k).Item("ExlSubject").ToString()
                                    Dim exlmsg As String = ds1.Tables("dataset").Rows(k).Item("ExlMailBody").ToString()
                                    qry = Replace(qry, "@tid", tid)
                                    da.SelectCommand.CommandText = qry
                                    da.SelectCommand.CommandType = CommandType.Text
                                    Dim exceldata As New DataTable
                                    da.Fill(exceldata)
                                    Try
                                        If exlmsg.Contains("{EnquiryNumber}") = True Then
                                            da.SelectCommand.CommandText = "select m.fld1[EnquiryNumber] from mmm_mst_master m left outer join mmm_mst_doc d on  m.documenttype='enquiry master' and m.eid=" & eid & " and m.tid=d.fld1  where d.tid=" & tid & " and d.documenttype='" & en & "' and d.eid=" & eid
                                            da.Fill(ds1, "datamsg")
                                            Dim enqnum As String = ds1.Tables("datamsg").Rows(0).Item("EnquiryNumber").ToString()
                                            exlmsg = exlmsg.Replace("{EnquiryNumber}", enqnum)
                                        End If

                                        If exlmsg.Contains("{DocNumber}") = True Then
                                            exlmsg = exlmsg.Replace("{DocNumber}", tid)
                                        End If

                                        If ds1.Tables("dataset").Rows(k).Item("iscustomer").ToString() = "1" Then
                                            Dim cm As String = dtqry.Rows(0).Item("OWNER EMAIL").ToString
                                            MAILTO = Replace(MAILTO, cm, "")
                                            MAILTO = MAILTO.Trim().Substring(0, MAILTO.Length - 1)

                                            '' disabled by sunil for protocol upgradation to TLS - 587 Port 
                                            ' sendMailExl(MAILTO, cc, Bcc, exlsub, exlmsg, CreateCSVR(exceldata, exlsub))

                                            obj.SendMail(ToMail:=MAILTO, Subject:=exlsub, MailBody:=exlmsg, CC:=cc, Attachments:=Filepath + CreateCSVR(exceldata, exlsub), BCC:=Bcc)

                                            'sendMail(ToMail:=MAILTO, subject:=exlsub, MailBody:=exlmsg, cc:=cc, Attachments:=Filepath + CreateCSVR(exceldata, exlsub), Bcc:=Bcc)
                                            MAILTO = MAILTO & "," & cm
                                        Else
                                            ExportToPDF(MAILTO, cc, Bcc, exlsub, exlmsg, exceldata, tid, eid)
                                            'sendMailExl(MAILTO, cc, Bcc, exlsub, exlmsg, CreateCSVR(exceldata, exlsub))
                                        End If

                                    Catch ex As Exception
                                    End Try

                                ElseIf ds1.Tables("dataset").Rows(k).Item("SendType").ToString() = "FIX" Then
                                    Pdffile = ""
                                    Dim qry As String = ds1.Tables("dataset").Rows(k).Item("qry").ToString()
                                    Dim exlsub As String = ds1.Tables("dataset").Rows(k).Item("ExlSubject").ToString()
                                    Dim strmsgBdy As String = ds1.Tables("dataset").Rows(k).Item("ExlMailBody").ToString()
                                    Dim EMailto As String = ds1.Tables("dataset").Rows(k).Item("emailto").ToString()
                                    Dim cc1 As String = ds1.Tables("dataset").Rows(k).Item("cc").ToString()
                                    Dim bcc1 As String = ds1.Tables("dataset").Rows(k).Item("bcc").ToString()
                                    qry = Replace(qry, "@tid", tid)
                                    da.SelectCommand.CommandText = qry
                                    da.SelectCommand.CommandType = CommandType.Text
                                    Dim fxdata As New DataTable
                                    da.Fill(fxdata)
                                    If fxdata.Rows.Count > 0 Then
                                        Dim MailTable As New StringBuilder()
                                        MailTable.Append("<table border=""1"" width=""100%"">")
                                        MailTable.Append("<tr style=""background-color:#990000"" Font-Bold=""True""> ")
                                        For l As Integer = 0 To fxdata.Columns.Count - 1
                                            MailTable.Append("<td >" & fxdata.Columns(l).ColumnName & "</td>")
                                        Next

                                        For m As Integer = 0 To fxdata.Rows.Count - 1 ' binding the tr tab in table
                                            MailTable.Append("</tr><tr>") ' for row records
                                            For t As Integer = 0 To fxdata.Columns.Count - 1
                                                MailTable.Append("<td>" & fxdata.Rows(m).Item(t).ToString() & " </td>")
                                            Next
                                            MailTable.Append("</tr>")
                                        Next
                                        MailTable.Append("</table>")

                                        If strmsgBdy.Contains("@body") Then
                                            strmsgBdy = Replace(strmsgBdy, "@body", MailTable.ToString())
                                            MSG = strmsgBdy
                                        Else
                                            MSG = MailTable.ToString()
                                        End If
                                        MSG = strmsgBdy
                                        '' disabled by sunil for protocol upgradation to TLS - 587 Port  - 17-Jan-19
                                        'sendMail1(EMailto, cc1, bcc1, exlsub, MSG, en, eid, "", listOfAttachmentString, tid, listOfDisplayNameForAttachments)

                                        obj.SendMail(ToMail:=EMailto, Subject:=exlsub, MailBody:=MSG, CC:=cc1, BCC:=bcc1)
                                        'sendMail(ToMail:=EMailto, subject:=exlsub, MailBody:=MSG, cc:=cc1, Bcc:=Bcc)
                                        MSG.Replace("{MailAttach}", "")
                                    End If
                                    ' webservice call start
                                    'ElseIf ds1.Tables("dataset").Rows(k).Item("SendType").ToString() = "WS" Then
                                    '    Try
                                    '        Dim ws As New WSOutward()
                                    '        Dim URLIST As String = ws.WBSREPORT(en, eid, tid)
                                    '    Catch ex As Exception
                                    '    End Try
                                    'web service call end
                                Else
                                    Pdffile = ""
                                End If
                                If MSG.Contains("MailAttach") And UCase(en) <> "STATIC SURVEY FORM" = True Then
                                    MSG.Replace("{MailAttach}", "")
                                    '' disabled by sunil for protocol upgradation to TLS - 587 Port  - 17-Jan-19
                                    ' SendMailPdf(MAILTO, cc, Bcc, subject, MSG, Pdffile)
                                    obj.SendMail(ToMail:=MAILTO, Subject:=subject, MailBody:=MSG, CC:=cc, Attachments:=Pdffile, BCC:=Bcc)
                                    'sendMail(ToMail:=MAILTO, subject:=subject, MailBody:=MSG, cc:=cc, Attachments:=Pdffile, Bcc:=Bcc)
                                End If
                            Next
                        Else
                            '' mail without attachment  12_june
                            If TicketCC <> "" Then
                                If cc = "" Then
                                    cc &= TicketCC
                                Else
                                    cc &= "," & TicketCC
                                End If
                            End If
                            '' disabled by sunil for protocol upgradation to TLS - 587 Port  - 17-Jan-19
                            ' sendMail1(MAILTO, cc, Bcc, subject, MSG, en, eid, upCommingFrom, listOfAttachmentString, tid, listOfDisplayNameForAttachments)
                            obj.SendMail1(ToMail:=MAILTO, Subject:=subject, MailBody:=MSG, CC:=cc, BCC:=Bcc, DocType:=en, eid:=eid, upCommingFrom:=upCommingFrom, attachementListString:=listOfAttachmentString, tid:=tid, listOfDisplayNameForAttachments:=listOfDisplayNameForAttachments)
                            'sendMail(ToMail:=MAILTO, subject:=subject, MailBody:=MSG, cc:=cc, Bcc:=Bcc)
                        End If
                    End If
                Next ' Loop for multiple mailing templates on same wfstatus and same subevent ends here (Pallavi)
            End If
            DS.Dispose()
        Catch ex As Exception
        Finally
            If Not da Is Nothing Then
                da.Dispose()
            End If
            If Not con Is Nothing Then
                con.Close()
                con.Dispose()
            End If
        End Try
    End Sub



    '' bkup b4 MailApproval button feature 19-jan-15
    'Public Sub TemplateCalling(ByVal tid As Integer, ByVal eid As Integer, ByVal en As String, ByVal SUBEVENT As String)
    '    Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
    '    Dim con As New SqlConnection(conStr)
    '    Dim STR As String = ""
    '    Dim MAILTO As String = ""
    '    Dim MAILID As String = ""
    '    Dim subject As String = ""
    '    Dim MSG As String = ""
    '    Dim cc As String = ""
    '    Dim Bcc As String = ""
    '    Dim MainEvent As String = ""
    '    Dim fn As String = ""
    '    Dim da As SqlDataAdapter = New SqlDataAdapter("select documenttype,curstatus,curdocnature from MMM_MST_DOC where tid=" & tid, con)
    '    Dim DS As New DataSet
    '    Dim WFstatus As String = "NOT REQUIRED"
    '    Dim curdocnature As String = ""
    '    'Dim obj As New MailUtill(eid:=eid)
    '    Dim Filepath As String = HostingEnvironment.MapPath("~/MailAttach/")
    '    'try Catch Block Added by Ajeet Kumar :Date::22 May 2014

    '    Try
    '        da.Fill(DS, "doctype")

    '        If en.Length < 2 Then
    '            en = DS.Tables("doctype").Rows(0).Item("documenttype").ToString()
    '        End If

    '        If DS.Tables("doctype").Rows.Count <> 0 Then
    '            WFstatus = DS.Tables("doctype").Rows(0).Item("curstatus").ToString()
    '        End If

    '        If SUBEVENT = "REJECT" Then
    '            WFstatus = "REJECTED"
    '        End If


    '        If DS.Tables("doctype").Rows.Count <> 0 Then
    '            curdocnature = DS.Tables("doctype").Rows(0).Item("curdocnature").ToString()
    '        End If


    '        If SUBEVENT.ToString.ToUpper = "APMQDM" Then
    '            curdocnature = "MODIFY"
    '            SUBEVENT = "CREATED"
    '        End If

    '        da.SelectCommand.CommandText = "select Code FROM MMM_MST_Entity where EID=" & eid
    '        da.Fill(DS, "eCode")
    '        Dim ECode = DS.Tables("eCode").Rows(0).Item("Code")

    '        da.SelectCommand.CommandText = "select T.TID,T.msgBody,T.subject,T.MAILTO,T.CC,T.BCC,T.EventName,T.qry,T.specialfieldflag from MMM_MST_TEMPLATE T  WHERE T.EVENTNAME='" & en & "' AND T.SUBEVENT='" & SUBEVENT & "' AND t.statusName='" & WFstatus & "' and EID=" & eid & " and t.docnature='" & curdocnature & "' "
    '        da.Fill(DS, "TEMP")
    '        If DS.Tables("TEMP").Rows.Count > 0 Then
    '            MSG = HttpUtility.HtmlDecode(DS.Tables("TEMP").Rows(0).Item("msgbody").ToString())
    '            subject = DS.Tables("TEMP").Rows(0).Item("SUBJECT").ToString()
    '            MAILTO = DS.Tables("TEMP").Rows(0).Item("MAILTO").ToString()
    '            cc = DS.Tables("TEMP").Rows(0).Item("CC").ToString()
    '            Bcc = DS.Tables("TEMP").Rows(0).Item("BCC").ToString()
    '            MainEvent = DS.Tables("TEMP").Rows(0).Item("EVENTNAME").ToString()
    '            STR = DS.Tables("TEMP").Rows(0).Item("qry").ToString()
    '        Else
    '            da.SelectCommand.CommandText = "select T.TID,T.msgBody,T.subject,T.MAILTO,T.CC,T.BCC,T.EventName,T.qry,T.specialfieldflag from MMM_MST_TEMPLATE T  WHERE T.EVENTNAME='" & en & "' AND T.SUBEVENT='" & SUBEVENT & "' AND t.statusName='" & WFstatus & "' AND EID=0 and t.docnature='" & curdocnature & "' "
    '            da.Fill(DS, "TEMP")
    '            If DS.Tables("TEMP").Rows.Count <> 0 Then
    '                MSG = HttpUtility.HtmlDecode(DS.Tables("TEMP").Rows(0).Item("msgbody").ToString())
    '                subject = DS.Tables("TEMP").Rows(0).Item("SUBJECT").ToString()
    '                MAILTO = DS.Tables("TEMP").Rows(0).Item("MAILTO").ToString()
    '                cc = DS.Tables("TEMP").Rows(0).Item("CC").ToString()
    '                Bcc = DS.Tables("TEMP").Rows(0).Item("BCC").ToString()
    '                MainEvent = DS.Tables("TEMP").Rows(0).Item("EVENTNAME").ToString()
    '                STR = DS.Tables("TEMP").Rows(0).Item("qry")
    '            End If
    '        End If
    '        Dim publicLink As String = ""
    '        ' Dim link As String = ConfigurationManager.AppSettings("pubUrl").ToString
    '        Dim ControlCase As String = ""
    '        If DS.Tables("TEMP").Rows.Count <> 0 Then
    '            For rindex As Integer = 0 To DS.Tables("TEMP").Rows.Count - 1 'Loop for multiple mailing templates on same wfstatus and same subevent starts here (Pallavi)
    '                'added by pallavi for multiple temps on 15 July 15
    '                MSG = HttpUtility.HtmlDecode(DS.Tables("TEMP").Rows(rindex).Item("msgbody").ToString())
    '                subject = DS.Tables("TEMP").Rows(rindex).Item("SUBJECT").ToString()
    '                MAILTO = DS.Tables("TEMP").Rows(rindex).Item("MAILTO").ToString()
    '                cc = DS.Tables("TEMP").Rows(rindex).Item("CC").ToString()
    '                Bcc = DS.Tables("TEMP").Rows(rindex).Item("BCC").ToString()
    '                MainEvent = DS.Tables("TEMP").Rows(rindex).Item("EVENTNAME").ToString()
    '                STR = DS.Tables("TEMP").Rows(rindex).Item("qry").ToString()
    '                'added by pallavi for multiple temps on 15 July 15

    '                STR &= " WHERE TID=" & tid & ""

    '                Dim daqry As New SqlDataAdapter(STR, con)
    '                Dim dtqry As New DataTable()
    '                daqry.Fill(dtqry)
    '                For Each dc As DataColumn In dtqry.Columns
    '                    fn = "{" & dc.ColumnName.ToString() & "}"
    '                    MSG = MSG.Replace(fn, dtqry.Rows(0).Item(dc.ColumnName).ToString())
    '                    subject = subject.Replace(fn, dtqry.Rows(0).Item(dc.ColumnName).ToString())
    '                    MAILTO = MAILTO.Replace(fn, dtqry.Rows(0).Item(dc.ColumnName).ToString())
    '                    cc = cc.Replace(fn, dtqry.Rows(0).Item(dc.ColumnName).ToString())
    '                Next

    '                If MSG.Contains("{ApprovalThroughMail}") Then
    '                    da.SelectCommand.Parameters.Clear()
    '                    da.SelectCommand.CommandText = "select userid from mmm_doc_dtl where tid in ( select lasttid from  MMM_MST_DOC where tid=" & tid & ")"
    '                    If con.State = ConnectionState.Closed Then
    '                        con.Open()
    '                    End If
    '                    Dim USERID As String = ""
    '                    Dim Q1 As String = "DOCID=" & tid
    '                    Q1 = EncryptTripleDES(Q1, eid)
    '                    Dim Q2 As String = "USERID=" & Convert.ToString(da.SelectCommand.ExecuteScalar())
    '                    Q2 = EncryptTripleDES(Q2, eid)
    '                    Dim Q3 As String = "CURSTATUS=" & WFstatus
    '                    Q3 = EncryptTripleDES(Q3, eid)
    '                    MSG = MSG.Replace("{ApprovalThroughMail}", "<br/><br/><table cellspacing=""0"" cellpadding=""0""><tr><td align=""center"" width=""300"" height=""40"" bgcolor=""#000091"" style=""-webkit-border-radius: 5px; -moz-border-radius: 5px; border-radius: 5px; color: #ffffff; display: block;""><a href=""http://" & ECode.ToString() & ".myndsaas.com/mailApproval.aspx?Q1=" & Q1 & "&Q2= " & Q2 & "&Q3=" & Q3 & """  style=""font-size:16px; font-weight: bold; font-family: Helvetica, Arial, sans-serif; text-decoration: none; line-height:40px;width:100%; display:inline-block""><span style=""color: #FFFFFF"">Approve Document</span></a></td></tr></table>")
    '                End If

    '                ' added public access link by balli
    '                Dim Pdffile As String = ""
    '                Dim RES As Integer = 0
    '                If UCase(en) <> "STATIC SURVEY FORM" Then
    '                    For i As Integer = 0 To DS.Tables("TEMP").Rows.Count - 1
    '                        If Val(DS.Tables("TEMP").Rows(i).Item("specialFieldflag").ToString) = 1 Then
    '                            da.SelectCommand.CommandText = "select * from MMM_MST_Template_extrafields where tempid=" & Val(DS.Tables("TEMP").Rows(i).Item("TID")) & ""
    '                            da.SelectCommand.CommandType = CommandType.Text
    '                            da.Fill(DS, "TempExtrafld")
    '                            For j As Integer = 0 To DS.Tables("TempExtrafld").Rows.Count - 1
    '                                ControlCase = DS.Tables("TempExtrafld").Rows(j).Item("controlName").ToString()
    '                                Select Case ControlCase
    '                                    Case "{DOCUMENT PUBLIC VIEW LINK}"
    '                                        If DS.Tables("TempExtrafld").Rows(j).Item("PvMode").ToString() = "EDIT" Then
    '                                            If DS.Tables("TempExtrafld").Rows(j).Item("PvRelationship").ToString().ToUpper = "DOCID" Then
    '                                                publicLink = "http://" & ECode & ".myndsaas.com/publicdocument.aspx?EID=" & eid & "&PvDoctype=" & DS.Tables("TempExtrafld").Rows(j).Item("PvDoctype").ToString() & "&date=" & Date.Now & "&docid=" & tid & "&docRef=0" & "emailId=" & MAILTO
    '                                            Else
    '                                                publicLink = "http://" & ECode & ".myndsaas.com/publicdocument.aspx?EID=" & eid & "&PvDoctype=" & DS.Tables("TempExtrafld").Rows(j).Item("PvDoctype").ToString() & "&date=" & Date.Now & "&docid=0&docRef=" & tid & "&emailId=" & MAILTO
    '                                            End If
    '                                        Else
    '                                            publicLink = "http://" & ECode & ".myndsaas.com/publicdocument.aspx?EID=" & eid & "&PvDoctype=" & DS.Tables("TempExtrafld").Rows(j).Item("PvDoctype").ToString() & "&date=" & Date.Now & "&docid=0&docRef=0 " & "emailId = " & MAILTO
    '                                        End If
    '                                        publicLink = "<a href='" & publicLink & "'> " & DS.Tables("TempExtrafld").Rows(j).Item("PvLinkCaption").ToString() & "</a>"
    '                                        Exit For
    '                                End Select
    '                            Next
    '                        End If
    '                    Next
    '                    MSG = Replace(MSG, ControlCase, publicLink)


    '                    Dim mailevent As String = en & "-" & SUBEVENT
    '                    da.SelectCommand.Parameters.Clear()
    '                    da.SelectCommand.CommandText = "INSERT_MAILLOG"
    '                    da.SelectCommand.CommandType = CommandType.StoredProcedure
    '                    da.SelectCommand.Parameters.AddWithValue("@MAILTO", MAILTO)
    '                    da.SelectCommand.Parameters.AddWithValue("@CC", cc)
    '                    da.SelectCommand.Parameters.AddWithValue("@MSG", MSG)
    '                    da.SelectCommand.Parameters.AddWithValue("@ALERTTYPE", "MAIL")
    '                    da.SelectCommand.Parameters.AddWithValue("@MAILEVENT", mailevent)
    '                    da.SelectCommand.Parameters.AddWithValue("@EID", eid)

    '                    If con.State <> ConnectionState.Open Then
    '                        con.Open()
    '                    End If
    '                    RES = da.SelectCommand.ExecuteScalar()
    '                    con.Close()
    '                End If
    '                If MSG.Contains("MailAttach") Then
    '                    Dim da1 As SqlDataAdapter = New SqlDataAdapter("select * from MMM_Print_Template where documenttype='" & en & "' and draft='original'", con)
    '                    Dim ds1 As New DataSet
    '                    da1.Fill(ds1, "dataset")
    '                    da.SelectCommand.CommandType = CommandType.Text
    '                    da.SelectCommand.CommandText = "select formcaption from mmm_mst_forms where formname='" & en & "' and eid=" & eid
    '                    da.Fill(ds1, "dataset1")
    '                    For k As Integer = 0 To ds1.Tables("dataset").Rows.Count - 1
    '                        Pdffile = GenerateMailPdf(ds1.Tables("dataset1").Rows(0).Item(0).ToString() & "_" & eid & "_" & "print" & ds1.Tables("dataset").Rows(k).Item("tid").ToString(), tid, en, "NA", curdocnature)
    '                        If Not Pdffile Is Nothing And Not String.IsNullOrEmpty(Pdffile) And ds1.Tables("dataset").Rows(k).Item("SendType").ToString() = "PDF" Then
    '                            Pdffile = HostingEnvironment.MapPath("~/MailAttach/" & Pdffile & ".pdf")
    '                        ElseIf ds1.Tables("dataset").Rows(k).Item("SendType").ToString() = "EXCEL" Then
    '                            Pdffile = ""
    '                            Dim qry As String = ds1.Tables("dataset").Rows(k).Item("qry").ToString()
    '                            Dim exlsub As String = ds1.Tables("dataset").Rows(k).Item("ExlSubject").ToString()
    '                            Dim exlmsg As String = ds1.Tables("dataset").Rows(k).Item("ExlMailBody").ToString()
    '                            qry = Replace(qry, "@tid", tid)
    '                            da.SelectCommand.CommandText = qry
    '                            da.SelectCommand.CommandType = CommandType.Text
    '                            Dim exceldata As New DataTable
    '                            da.Fill(exceldata)
    '                            Try
    '                                If exlmsg.Contains("{EnquiryNumber}") = True Then
    '                                    da.SelectCommand.CommandText = "select m.fld1[EnquiryNumber] from mmm_mst_master m left outer join mmm_mst_doc d on  m.documenttype='enquiry master' and m.eid=" & eid & " and m.tid=d.fld1  where d.tid=" & tid & " and d.documenttype='" & en & "' and d.eid=" & eid
    '                                    da.Fill(ds1, "datamsg")
    '                                    Dim enqnum As String = ds1.Tables("datamsg").Rows(0).Item("EnquiryNumber").ToString()
    '                                    exlmsg = exlmsg.Replace("{EnquiryNumber}", enqnum)
    '                                End If

    '                                If exlmsg.Contains("{DocNumber}") = True Then
    '                                    exlmsg = exlmsg.Replace("{DocNumber}", tid)
    '                                End If

    '                                If ds1.Tables("dataset").Rows(k).Item("iscustomer").ToString() = "1" Then
    '                                    Dim cm As String = dtqry.Rows(0).Item("OWNER EMAIL").ToString
    '                                    MAILTO = Replace(MAILTO, cm, "")
    '                                    MAILTO = MAILTO.Trim().Substring(0, MAILTO.Length - 1)

    '                                    sendMailExl(MAILTO, cc, Bcc, exlsub, exlmsg, CreateCSVR(exceldata, exlsub))

    '                                    'obj.SendMail(ToMail:=MAILTO, Subject:=exlsub, MailBody:=exlmsg, CC:=cc, Attachments:=Filepath + CreateCSVR(exceldata, exlsub), BCC:=Bcc)

    '                                    'sendMail(ToMail:=MAILTO, subject:=exlsub, MailBody:=exlmsg, cc:=cc, Attachments:=Filepath + CreateCSVR(exceldata, exlsub), Bcc:=Bcc)
    '                                    MAILTO = MAILTO & "," & cm
    '                                Else
    '                                    ExportToPDF(MAILTO, cc, Bcc, exlsub, exlmsg, exceldata, tid, eid)
    '                                    'sendMailExl(MAILTO, cc, Bcc, exlsub, exlmsg, CreateCSVR(exceldata, exlsub))
    '                                End If

    '                            Catch ex As Exception
    '                            End Try

    '                        ElseIf ds1.Tables("dataset").Rows(k).Item("SendType").ToString() = "FIX" Then
    '                            Pdffile = ""
    '                            Dim qry As String = ds1.Tables("dataset").Rows(k).Item("qry").ToString()
    '                            Dim exlsub As String = ds1.Tables("dataset").Rows(k).Item("ExlSubject").ToString()
    '                            Dim strmsgBdy As String = ds1.Tables("dataset").Rows(k).Item("ExlMailBody").ToString()
    '                            Dim EMailto As String = ds1.Tables("dataset").Rows(k).Item("emailto").ToString()
    '                            Dim cc1 As String = ds1.Tables("dataset").Rows(k).Item("cc").ToString()
    '                            Dim bcc1 As String = ds1.Tables("dataset").Rows(k).Item("bcc").ToString()
    '                            qry = Replace(qry, "@tid", tid)
    '                            da.SelectCommand.CommandText = qry
    '                            da.SelectCommand.CommandType = CommandType.Text
    '                            Dim fxdata As New DataTable
    '                            da.Fill(fxdata)
    '                            If fxdata.Rows.Count > 0 Then
    '                                Dim MailTable As New StringBuilder()
    '                                MailTable.Append("<table border=""1"" width=""100%"">")
    '                                MailTable.Append("<tr style=""background-color:#990000"" Font-Bold=""True""> ")
    '                                For l As Integer = 0 To fxdata.Columns.Count - 1
    '                                    MailTable.Append("<td >" & fxdata.Columns(l).ColumnName & "</td>")
    '                                Next

    '                                For m As Integer = 0 To fxdata.Rows.Count - 1 ' binding the tr tab in table
    '                                    MailTable.Append("</tr><tr>") ' for row records
    '                                    For t As Integer = 0 To fxdata.Columns.Count - 1
    '                                        MailTable.Append("<td>" & fxdata.Rows(m).Item(t).ToString() & " </td>")
    '                                    Next
    '                                    MailTable.Append("</tr>")
    '                                Next
    '                                MailTable.Append("</table>")

    '                                If strmsgBdy.Contains("@body") Then
    '                                    strmsgBdy = Replace(strmsgBdy, "@body", MailTable.ToString())
    '                                    MSG = strmsgBdy
    '                                Else
    '                                    MSG = MailTable.ToString()
    '                                End If
    '                                MSG = strmsgBdy
    '                                sendMail1(EMailto, cc1, bcc1, exlsub, MSG, EN, EID)
    '                                'obj.SendMail(ToMail:=EMailto, Subject:=exlsub, MailBody:=MSG, CC:=cc1, BCC:=Bcc)
    '                                'sendMail(ToMail:=EMailto, subject:=exlsub, MailBody:=MSG, cc:=cc1, Bcc:=Bcc)
    '                                MSG.Replace("{MailAttach}", "")
    '                            End If


    '                            ' webservice call start

    '                            'ElseIf ds1.Tables("dataset").Rows(k).Item("SendType").ToString() = "WS" Then

    '                            '    Try
    '                            '        Dim ws As New WSOutward()
    '                            '        Dim URLIST As String = ws.WBSREPORT(en, eid, tid)
    '                            '    Catch ex As Exception

    '                            '    End Try

    '                            'web service call end



    '                        Else
    '                            Pdffile = ""
    '                        End If
    '                        If MSG.Contains("MailAttach") And UCase(en) <> "STATIC SURVEY FORM" = True Then
    '                            MSG.Replace("{MailAttach}", "")
    '                            SendMailPdf(MAILTO, cc, Bcc, subject, MSG, Pdffile)
    '                            'obj.SendMail(ToMail:=MAILTO, Subject:=subject, MailBody:=MSG, CC:=cc, Attachments:=Pdffile, BCC:=Bcc)
    '                            'sendMail(ToMail:=MAILTO, subject:=subject, MailBody:=MSG, cc:=cc, Attachments:=Pdffile, Bcc:=Bcc)
    '                        End If
    '                    Next
    '                Else
    '                    '' mail without attachment  12_june
    '                    sendMail1(MAILTO, cc, Bcc, subject, MSG, en, eid)
    '                    'obj.SendMail(ToMail:=MAILTO, Subject:=subject, MailBody:=MSG, CC:=cc, BCC:=Bcc)
    '                    'sendMail(ToMail:=MAILTO, subject:=subject, MailBody:=MSG, cc:=cc, Bcc:=Bcc)
    '                End If

    '            Next ' Loop for multiple mailing templates on same wfstatus and same subevent ends here (Pallavi)
    '        End If
    '        DS.Dispose()
    '    Catch ex As Exception
    '    Finally
    '        If Not da Is Nothing Then
    '            da.Dispose()
    '        End If
    '        If Not con Is Nothing Then
    '            con.Close()
    '            con.Dispose()
    '        End If
    '    End Try
    'End Sub

    Private Function EncryptTripleDES(ByVal sIn As String, ByVal sKey As String) As String
        Dim DES As New System.Security.Cryptography.TripleDESCryptoServiceProvider
        Dim hashMD5 As New System.Security.Cryptography.MD5CryptoServiceProvider
        ' scramble the key
        sKey = ScrambleKey(sKey)
        ' Compute the MD5 hash.
        DES.Key = hashMD5.ComputeHash(System.Text.ASCIIEncoding.ASCII.GetBytes(sKey))
        ' Set the cipher mode.
        DES.Mode = System.Security.Cryptography.CipherMode.ECB
        ' Create the encryptor.
        Dim DESEncrypt As System.Security.Cryptography.ICryptoTransform = DES.CreateEncryptor()
        ' Get a byte array of the string.
        Dim Buffer As Byte() = System.Text.ASCIIEncoding.ASCII.GetBytes(sIn)
        ' Transform and return the string.
        Return Convert.ToBase64String(DESEncrypt.TransformFinalBlock(Buffer, 0, Buffer.Length))
    End Function

    Private Function ScrambleKey(ByVal v_strKey As String) As String
        Dim sbKey As New System.Text.StringBuilder
        Dim intPtr As Integer
        For intPtr = 1 To v_strKey.Length
            Dim intIn As Integer = v_strKey.Length - intPtr + 1
            sbKey.Append(Mid(v_strKey, intIn, 1))
        Next
        Dim strKey As String = sbKey.ToString
        Return sbKey.ToString
    End Function


    Private Function CreateCSVR(ByVal dt As DataTable, ByVal subject As String) As String

        Dim Folder As New DirectoryInfo(HostingEnvironment.MapPath("~/MailAttach/"))
        Dim fname1 As String = ""
        Try
            If Folder.Exists Then
                'For Each fl As FileInfo In Folder.GetFiles()
                '    Try
                '        fl.Delete()
                '    Catch ex As Exception
                '    End Try
                'Next
            End If

            fname1 = Now.Year & Now.Month & Now.Day & Now.Hour & Now.Minute & Now.Second & ".xls"
            Dim FPath As String = HostingEnvironment.MapPath("~/MailAttach/")
            'If File.Exists(FPath & fname1) Then
            '    File.Delete(FPath & fname1)
            'End If

            Dim sw As StreamWriter

            sw = New StreamWriter(FPath & fname1, True)
            Dim hw As New HtmlTextWriter(sw)
            sw.Flush()
            Dim iColCount As Integer = dt.Columns.Count
            Dim grd As New GridView
            grd.DataSource = dt
            grd.DataBind()
            If subject.ToUpper = "DOMESTIC SURVEY REPORT" Then
                grd.Rows(0).BackColor = System.Drawing.Color.Green
                grd.Rows(1).BackColor = System.Drawing.Color.Green
                grd.Rows(0).ForeColor = Color.White
                grd.Rows(1).ForeColor = Color.White
                grd.HeaderRow.Style.Add("background-color", "black")
                'grd.HeaderRow.Style.Add("fore-color", "white")
            End If

            For i As Integer = 0 To grd.Rows.Count - 1
                grd.Rows(i).Attributes.Add("class", "textmode")
                'Apply text style to each Row 
            Next

            'grd.CssClass = style
            grd.RenderControl(hw)
            Dim style As String = "<style> .textmode { mso-number-format:\""0000""; } </style>"
            sw.WriteLine(style)
            sw.Write(sw.NewLine)

            ' Now write all the rows.

            sw.Close()
        Catch ex As Exception

        End Try


        Return fname1
        'sw.Close()
    End Function


    Private Function ExportToPDF(ByVal mailto As String, ByVal cc As String, ByVal bcc As String, ByVal subject As String, ByVal msg As String, ByVal dt As DataTable, ByVal tid As Integer, ByVal eid As Integer) As String
        Dim Folder As New DirectoryInfo(HostingEnvironment.MapPath("~/MailAttach/"))
        Dim fname1 As String = ""
        Dim obj As New MailUtill(eid:=eid)
        If Folder.Exists Then
        End If
        Dim FPath As String = HostingEnvironment.MapPath("~/MailAttach/")
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim da As New SqlDataAdapter("", con)
        da.SelectCommand.CommandText = "select fld40[dtl] from mmm_mst_doc where tid=" & tid & ""
        Dim cdata As New DataSet
        da.Fill(cdata, "custdtl")
        Dim enqnum As String = cdata.Tables("custdtl").Rows(0).Item("dtl").ToString
        fname1 = "DomesticSurveyReport" & "_" & tid & ".pdf"
        subject = "Article List-" & enqnum
        Dim FS As FileStream = Nothing
        Using sw As New StringWriter()
            Using hw As New HtmlTextWriter(sw)
                Dim GridView1 As New GridView
                GridView1.DataSource = dt
                GridView1.DataBind()
                GridView1.ForeColor = Color.Black
                GridView1.HeaderRow.ForeColor = Color.White
                GridView1.HeaderStyle.ForeColor = Color.White
                GridView1.Rows(0).ForeColor = Color.Red
                GridView1.HeaderStyle.BackColor = Color.DimGray
                GridView1.HeaderRow.BackColor = Color.Cyan
                GridView1.HeaderStyle.BorderColor = Color.Blue
                GridView1.Rows(0).Height = 0
                GridView1.Rows(0).BorderColor = Color.Blue
                GridView1.Rows(1).Height = 0
                GridView1.Rows(1).BorderColor = Color.Blue
                GridView1.Rows(1).BackColor = Color.Cyan
                GridView1.Rows(0).BorderColor = Color.Cyan
                GridView1.Rows(1).BorderColor = Color.Cyan
                GridView1.BorderStyle = BorderStyle.Inset
                GridView1.BorderColor = Color.DarkGray
                GridView1.HeaderStyle.ForeColor = Color.White
                GridView1.Font.Size = 7
                GridView1.RenderControl(hw)
                Dim sr As New StringReader(sw.ToString())
                Dim pdfDoc As New Document(PageSize.A4, 10.0F, 10.0F, 10.0F, 0.0F)
                Dim htmlparser As New HTMLWorker(pdfDoc)
                Using memoryStream As New MemoryStream()
                    PdfWriter.GetInstance(pdfDoc, memoryStream)
                    pdfDoc.Open()
                    htmlparser.Parse(sr)
                    pdfDoc.Close()
                    Dim bytes As Byte() = memoryStream.ToArray()
                    memoryStream.Close()
                    FS = New FileStream(FPath + fname1, System.IO.FileMode.Create)
                    FS.Write(bytes, 0, New MemoryStream(bytes).Length)
                    FS.Close()
                    obj.SendMail(ToMail:=mailto, Subject:=subject, MailBody:=msg, CC:=cc, Attachments:=FPath + fname1, BCC:=bcc)
                    'sendMail(ToMail:=mailto, subject:=subject, MailBody:=msg, cc:=cc, Attachments:=FPath + fname1, bcc:=bcc)
                    '' disabled by sunil for protocol upgradation to TLS - 587 Port  - 17-Jan-19
                    'SendMailPdf(Mto:=mailto, cc:=cc, bcc:=bcc, MSubject:=subject, MBody:=msg, backuppath:=FPath + fname1)

                End Using
            End Using
        End Using
    End Function


    Private Sub sendMailExl(ByVal Mto As String, ByVal cc As String, ByVal bcc As String, ByVal MSubject As String, ByVal MBody As String, ByVal backuppath As String)
        Try
            If Left(Mto, 1) = "{" Then
                Exit Sub
            End If
            Dim fname1 As String = ""
            Dim fname2 As String = ""
            Dim path As String = HostingEnvironment.MapPath("~/MailAttach/")
            fname2 = Now.Year & Now.Month & Now.Day & Now.Hour & Now.Minute & ".CSV"
            Dim att As Attachment
            '  Dim path As String = "rajat/"
            att = New Attachment(path + backuppath)
            Dim Email As New System.Net.Mail.MailMessage("MYNDSAAS<no-reply@myndsol.com>", Mto, MSubject, MBody)
            Dim mailClient As New System.Net.Mail.SmtpClient()
            Email.IsBodyHtml = True
            If cc <> "" Then
                Email.CC.Add(cc)
                'Email.Attachments.Add(att)

            End If


            If bcc <> "" Then
                Email.Bcc.Add(bcc)
                'Email.Attachments.Add(att)

            End If
            Email.Attachments.Add(att)
            Dim basicAuthenticationInfo As New System.Net.NetworkCredential("no-reply@myndsol.com", "vc79aK123AJ&$kL0")
            mailClient.Host = "mail.myndsol.com"
            mailClient.UseDefaultCredentials = False
            mailClient.Credentials = basicAuthenticationInfo
            Try
                mailClient.Send(Email)
            Catch ex As Exception
                Exit Sub
            End Try
        Catch ex As Exception
            Exit Sub
        End Try
    End Sub

    '' bkup 03_nov_17
    'Public Function GenerateMailPdf(ByVal filename As String, ByVal docid As Integer, ByVal dc As String, Optional ISDraft As String = "NA", Optional ByVal curdocnature As String = "SE") As String
    '    Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
    '    Dim con As New SqlConnection(conStr)
    '    Dim da As New SqlDataAdapter("", con)
    '    'try Catch Block Added by Ajeet Kumar :Date::22 May 2014
    '    Try
    '        If ISDraft <> "NA" Then
    '            da.SelectCommand.CommandText = "select * from MMM_Print_Template where template_name='" & filename & "' and Draft='DRAFT'"
    '        Else
    '            da.SelectCommand.CommandText = "select * from MMM_Print_Template where template_name='" & filename & "' and draft='original'"
    '        End If
    '        Dim ds As New DataSet
    '        Dim path As String = HostingEnvironment.MapPath("~/MailAttach/")

    '        Dim FPath As String = HostingEnvironment.MapPath("~/MailAttach/")
    '        Dim Folder As New DirectoryInfo(HostingEnvironment.MapPath("~/MailAttach/"))


    '        da.Fill(ds, "data1")
    '        da.SelectCommand.CommandType = CommandType.Text
    '        da.SelectCommand.CommandText = "select formcaption from mmm_mst_forms where formname='" & dc & "'"
    '        da.Fill(ds, "data2")

    '        ' Dim fname1 As String = ds.Tables("data2").Rows(0).Item(0).ToString() & "_" & docid & "_" & ds.Tables("data1").Rows(0).Item(0).ToString()

    '        If ds.Tables("data1").Rows.Count <> 1 Then
    '            Exit Function
    '        End If
    '        Dim body As String = ds.Tables("data1").Rows(0).Item("body").ToString()
    '        Dim MainQry As String = ds.Tables("data1").Rows(0).Item("qry").ToString()
    '        Dim childQry As String = ds.Tables("data1").Rows(0).Item("SQL_CHILDITEM").ToString()
    '        Dim DocType As String = ds.Tables("data1").Rows(0).Item("Documenttype").ToString()
    '        Dim moveqry As String = ds.Tables("data1").Rows(0).Item("SQL_MOV_DTL").ToString()
    '        ' Start  signature image
    '        Try
    '            Dim EID As String = ds.Tables("data1").Rows(0).Item("EID").ToString()
    '            da.SelectCommand.CommandText = "select fieldmapping from mmm_mst_fields where documenttype='" & dc & "' and fieldtype='signature'"
    '            da.Fill(ds, "data3")
    '            Dim fldmap As String = ""

    '            For l As Integer = 0 To ds.Tables("data3").Rows.Count - 1
    '                Dim dtSig As New DataTable()
    '                fldmap = ds.Tables("data3").Rows(l).Item("fieldmapping").ToString()
    '                If ISDraft <> "NA" Then
    '                    da.SelectCommand.CommandText = "select " & fldmap & " from mmm_mst_doc_draft where documenttype='" & dc & "' and tid=" & docid & ""
    '                Else
    '                    da.SelectCommand.CommandText = "select " & fldmap & " from mmm_mst_doc where documenttype='" & dc & "' and tid=" & docid & ""
    '                End If

    '                da.Fill(dtSig)
    '                Dim Sig_fldVal As String = dtSig.Rows(0).Item(0).ToString()
    '                'Dim Sig_fldVal As String = ""
    '                If dtSig.Rows.Count <> 0 Then
    '                    Sig_fldVal = dtSig.Rows(0).Item(0).ToString()
    '                    If Sig_fldVal.ToUpper() = "NOSIGNATURE.PNG" Then
    '                        If body.Contains("{SignImg_" & fldmap & "}") Then
    '                            body = body.Replace("{SignImg_" & fldmap & "}", "images/NoSignature.png")
    '                        End If
    '                    ElseIf Len(Sig_fldVal) > 4 Then
    '                        If body.Contains("{SignImg_" & fldmap & "}") Then

    '                            ' CHANGE ON 1 JULY FOR SIG PROBLEM IN QUATATION
    '                            ' body = body.Replace("{SignImg_" & fldmap & "}", "docs/" & EID & "/" & Sig_fldVal)
    '                            body = body.Replace("{SignImg_" & fldmap & "}", "docs/" & Sig_fldVal)
    '                            'body = body.Replace("{SignImg_" & fldmap & "}", "docs/" & Sig_fldVal)
    '                        End If
    '                    ElseIf Sig_fldVal = "" Then
    '                        If body.Contains("{SignImg_" & fldmap & "}") Then
    '                            body = body.Replace("{SignImg_" & fldmap & "}", "images/NoSignature.png")
    '                        End If
    '                    Else
    '                        If body.Contains("{SignImg_" & fldmap & "}") Then
    '                            body = body.Replace("{SignImg_" & fldmap & "}", "images/NoSignature.png")
    '                        End If
    '                    End If
    '                End If

    '                dtSig.Dispose()
    '            Next
    '        Catch ex As Exception

    '        End Try
    '        ' End signature image

    '        da.SelectCommand.CommandText = MainQry.Replace("@tid", docid)
    '        da.Fill(ds, "main")
    '        If dc.ToString.ToUpper = "QUOTATION DOMESTIC MOVEMENT" And curdocnature = "CREATE" Then
    '            body = body.Replace("Discount", "")
    '            body = body.Replace("-[]", "")
    '        End If

    '        For j As Integer = 0 To ds.Tables("main").Columns.Count - 1
    '            body = body.Replace("[" & ds.Tables("main").Columns(j).ColumnName & "]", ds.Tables("main").Rows(0).Item(j).ToString())
    '        Next

    '        da.SelectCommand.CommandText = childQry.Replace("@tid", docid)
    '        da.Fill(ds, "child")

    '        'ds.Tables("child").DefaultView.Sort = ds.Tables("child").Columns(0).ColumnName & " ASC"
    '        ds.Dispose()
    '        Dim strChildItem As String = "<div><table width=""100%"" border=""0.5"" text-align=""left""  >"
    '        Dim prevVal As String = ""
    '        For i As Integer = 0 To ds.Tables("child").Rows.Count - 1
    '            If prevVal = ds.Tables("child").Rows(i).Item(0).ToString() Then
    '                prevVal = ds.Tables("child").Rows(i).Item(0).ToString()
    '                ds.Tables("child").Rows(i).Item(0) = ""
    '            Else
    '                prevVal = ds.Tables("child").Rows(i).Item(0).ToString()
    '            End If
    '        Next

    '        For i As Integer = 0 To ds.Tables("child").Rows.Count
    '            strChildItem &= "<tr>"
    '            For j As Integer = 0 To ds.Tables("child").Columns.Count - 1
    '                strChildItem &= "<td text-align:left>"
    '                If i = 0 Then
    '                    strChildItem &= ds.Tables("child").Columns(j).ColumnName
    '                Else
    '                    strChildItem &= ds.Tables("child").Rows(i - 1).Item(j).ToString()
    '                End If
    '                strChildItem &= "</td>"
    '            Next
    '            strChildItem &= "</tr>"
    '        Next
    '        strChildItem &= "</table></div>"
    '        body = body.Replace("[child item]", strChildItem)

    '        If body.Contains("[movdtl]") Then
    '            Dim hub As String = ds.Tables("main").Rows(0).Item("Hub Name").ToString()
    '            da.SelectCommand.CommandText = moveqry.Replace("@hub", hub)
    '            da.Fill(ds, "movdtl")
    '            Dim stmov As String = "<div><table width=""100%"" border=""0.5"" text-align=""left""  >"
    '            Dim preMovvVal As String = ""
    '            'For i As Integer = 0 To ds.Tables("movdtl").Rows.Count - 1
    '            '    If preMovvVal = ds.Tables("movdtl").Rows(i).Item(0).ToString() Then
    '            '        preMovvVal = ds.Tables("movdtl").Rows(i).Item(0).ToString()
    '            '        ds.Tables("movdtl").Rows(i).Item(0) = ""
    '            '    Else
    '            '        preMovvVal = ds.Tables("movdtl").Rows(i).Item(0).ToString()
    '            '    End If
    '            'Next

    '            For i As Integer = 0 To ds.Tables("movdtl").Rows.Count
    '                stmov &= "<tr>"
    '                For j As Integer = 0 To ds.Tables("movdtl").Columns.Count - 1
    '                    stmov &= "<td text-align:left>"
    '                    If i = 0 Then
    '                        stmov &= ds.Tables("movdtl").Columns(j).ColumnName
    '                    Else
    '                        stmov &= ds.Tables("movdtl").Rows(i - 1).Item(j).ToString()
    '                    End If
    '                    stmov &= "</td>"
    '                Next
    '                stmov &= "</tr>"
    '            Next
    '            stmov &= "</table></div>"
    '            body = body.Replace("[movdtl]", stmov)
    '        End If

    '        If dc.ToString.ToUpper = "QUOTATION DOMESTIC MOVEMENT" And curdocnature = "MODIFY" Then
    '            filename = filename & "_" & docid & "_" & Now.Millisecond
    '        Else
    '            filename = filename & "_" & docid
    '        End If



    '        WritePdf(body, filename)
    '        Return filename
    '    Catch ex As Exception
    '        Throw
    '    Finally
    '        If Not da Is Nothing Then
    '            da.Dispose()
    '        End If
    '        If Not con Is Nothing Then
    '            con.Close()
    '            con.Dispose()
    '        End If
    '    End Try

    'End Function

    Public Function GenerateMailPdf(ByVal filename As String, ByVal docid As Integer, ByVal dc As String, Optional ISDraft As String = "NA", Optional ByVal curdocnature As String = "SE") As String
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim da As New SqlDataAdapter("", con)
        'try Catch Block Added by Ajeet Kumar :Date::22 May 2014
        Try
            Dim ds As New DataSet
            Dim NewEID As Integer = 0
            da.SelectCommand.CommandText = "select eid from MMM_mst_doc where tid=" & docid
            Dim dtE As New DataTable
            da.Fill(dtE)
            If dtE.Rows.Count = 1 Then
                NewEID = dtE.Rows(0).Item(0).ToString()
            End If
            dtE.Dispose()

            If ISDraft <> "NA" Then
                da.SelectCommand.CommandText = "select * from MMM_Print_Template where template_name='" & filename & "' and Draft='DRAFT' and EID=" & NewEID
            Else
                da.SelectCommand.CommandText = "select * from MMM_Print_Template where template_name='" & filename & "' and draft='original' and EID=" & NewEID
            End If

            Dim path As String = HostingEnvironment.MapPath("~/MailAttach/")

            Dim FPath As String = HostingEnvironment.MapPath("~/MailAttach/")
            Dim Folder As New DirectoryInfo(HostingEnvironment.MapPath("~/MailAttach/"))


            da.Fill(ds, "data1")
            da.SelectCommand.CommandType = CommandType.Text
            da.SelectCommand.CommandText = "select formcaption from mmm_mst_forms where formname='" & dc & "' and eid='" & ds.Tables("data1").Rows(0).Item("EID").ToString() & "'"
            da.Fill(ds, "data2")

            ' Dim fname1 As String = ds.Tables("data2").Rows(0).Item(0).ToString() & "_" & docid & "_" & ds.Tables("data1").Rows(0).Item(0).ToString()

            If ds.Tables("data1").Rows.Count <> 1 Then
                Exit Function
            End If
            Dim body As String = ds.Tables("data1").Rows(0).Item("body").ToString()
            Dim MainQry As String = ds.Tables("data1").Rows(0).Item("qry").ToString()
            Dim childQry As String = ds.Tables("data1").Rows(0).Item("SQL_CHILDITEM").ToString()
            Dim childQryFirst As String = ds.Tables("data1").Rows(0).Item("sql_child1").ToString()
            Dim childQrySecond As String = ds.Tables("data1").Rows(0).Item("sql_child2").ToString()
            Dim childQryThird As String = ds.Tables("data1").Rows(0).Item("sql_child3").ToString()
            Dim childQryFourth As String = ds.Tables("data1").Rows(0).Item("sql_child4").ToString()
            Dim childQryFifth As String = ds.Tables("data1").Rows(0).Item("sql_child5").ToString()
            Dim DocType As String = ds.Tables("data1").Rows(0).Item("Documenttype").ToString()
            Dim moveqry As String = ds.Tables("data1").Rows(0).Item("SQL_MOV_DTL").ToString()
            ' Start  signature image
            Try
                Dim EID As String = ds.Tables("data1").Rows(0).Item("EID").ToString()
                da.SelectCommand.CommandText = "select fieldmapping from mmm_mst_fields where documenttype='" & dc & "' and fieldtype='signature'"
                da.Fill(ds, "data3")
                Dim fldmap As String = ""

                For l As Integer = 0 To ds.Tables("data3").Rows.Count - 1
                    Dim dtSig As New DataTable()
                    fldmap = ds.Tables("data3").Rows(l).Item("fieldmapping").ToString()
                    If ISDraft <> "NA" Then
                        da.SelectCommand.CommandText = "select " & fldmap & " from mmm_mst_doc_draft where documenttype='" & dc & "' and tid=" & docid & ""
                    Else
                        da.SelectCommand.CommandText = "select " & fldmap & " from mmm_mst_doc where documenttype='" & dc & "' and tid=" & docid & ""
                    End If

                    da.Fill(dtSig)
                    Dim Sig_fldVal As String = dtSig.Rows(0).Item(0).ToString()
                    'Dim Sig_fldVal As String = ""
                    If dtSig.Rows.Count <> 0 Then
                        Sig_fldVal = dtSig.Rows(0).Item(0).ToString()
                        If Sig_fldVal.ToUpper() = "NOSIGNATURE.PNG" Then
                            If body.Contains("{SignImg_" & fldmap & "}") Then
                                body = body.Replace("{SignImg_" & fldmap & "}", "images/NoSignature.png")
                            End If
                        ElseIf Len(Sig_fldVal) > 4 Then
                            If body.Contains("{SignImg_" & fldmap & "}") Then

                                ' CHANGE ON 1 JULY FOR SIG PROBLEM IN QUATATION
                                ' body = body.Replace("{SignImg_" & fldmap & "}", "docs/" & EID & "/" & Sig_fldVal)
                                body = body.Replace("{SignImg_" & fldmap & "}", "docs/" & Sig_fldVal)
                                'body = body.Replace("{SignImg_" & fldmap & "}", "docs/" & Sig_fldVal)
                            End If
                        ElseIf Sig_fldVal = "" Then
                            If body.Contains("{SignImg_" & fldmap & "}") Then
                                body = body.Replace("{SignImg_" & fldmap & "}", "images/NoSignature.png")
                            End If
                        Else
                            If body.Contains("{SignImg_" & fldmap & "}") Then
                                body = body.Replace("{SignImg_" & fldmap & "}", "images/NoSignature.png")
                            End If
                        End If
                    End If

                    dtSig.Dispose()
                Next
            Catch ex As Exception

            End Try
            ' End signature image

            da.SelectCommand.CommandText = MainQry.Replace("@tid", docid)
            da.Fill(ds, "main")
            If dc.ToString.ToUpper = "QUOTATION DOMESTIC MOVEMENT" And curdocnature = "CREATE" Then
                body = body.Replace("Discount", "")
                body = body.Replace("-[]", "")
            End If

            For j As Integer = 0 To ds.Tables("main").Columns.Count - 1
                body = body.Replace("[" & ds.Tables("main").Columns(j).ColumnName & "]", ds.Tables("main").Rows(0).Item(j).ToString())
            Next
            da.SelectCommand.CommandType = CommandType.Text
            da.SelectCommand.CommandText = childQry.Replace("@tid", docid)
            da.Fill(ds, "child")
            'ds.Tables("child").DefaultView.Sort = ds.Tables("child").Columns(0).ColumnName & " ASC"
            ds.Dispose()


            Dim strChildItem As String = "<div><table width=""100%"" border=""0.5"" text-align=""left""  >"
            If (filename <> "Purchase Order_180_print177") Then
                Dim prevVal As String = ""
                For i As Integer = 0 To ds.Tables("child").Rows.Count - 1
                    If prevVal = ds.Tables("child").Rows(i).Item(0).ToString() Then
                        prevVal = ds.Tables("child").Rows(i).Item(0).ToString()
                        ds.Tables("child").Rows(i).Item(0) = ""
                    Else
                        prevVal = ds.Tables("child").Rows(i).Item(0).ToString()
                    End If
                Next
            End If


            If (filename = "Purchase Order_180_print177") Then
                For i As Integer = 0 To ds.Tables("child").Rows.Count
                    strChildItem &= "<tr>"
                    For j As Integer = 0 To ds.Tables("child").Columns.Count - 1
                        Dim columnname As String = ds.Tables("child").Columns(j).ColumnName
                        If i = 0 Then
                            If (columnname = "Material Description") Then
                                strChildItem &= "<td  width=""300"">"
                                strChildItem &= ds.Tables("child").Columns(j).ColumnName
                                strChildItem &= "</td>"
                            ElseIf (columnname = "HSN") Then
                                strChildItem &= "<td  width=""50"">"
                                strChildItem &= ds.Tables("child").Columns(j).ColumnName
                                strChildItem &= "</td>"
                            ElseIf (columnname = "UOM") Then
                                strChildItem &= "<td  width=""50"">"
                                strChildItem &= ds.Tables("child").Columns(j).ColumnName
                                strChildItem &= "</td>"
                            ElseIf (columnname = "Quantity") Then
                                strChildItem &= "<td  width=""80"">"
                                strChildItem &= ds.Tables("child").Columns(j).ColumnName
                                strChildItem &= "</td>"
                            ElseIf (columnname = "Unit Price") Then
                                strChildItem &= "<td  width=""80"">"
                                strChildItem &= ds.Tables("child").Columns(j).ColumnName
                                strChildItem &= "</td>"
                            ElseIf (columnname = "Total Price") Then
                                strChildItem &= "<td  width=""100"">"
                                strChildItem &= ds.Tables("child").Columns(j).ColumnName
                                strChildItem &= "</td>"
                            ElseIf (columnname = "Remarks") Then
                                strChildItem &= "<td  width=""250"">"
                                strChildItem &= ds.Tables("child").Columns(j).ColumnName
                                strChildItem &= "</td>"
                            Else
                                strChildItem &= "<td>"
                                strChildItem &= ds.Tables("child").Columns(j).ColumnName
                                strChildItem &= "</td>"
                            End If
                        Else
                            If (columnname = "Material Description") Then
                                strChildItem &= "<td  width=""300"">"
                                strChildItem &= ds.Tables("child").Rows(i - 1).Item(j).ToString()
                                strChildItem &= "</td>"
                            ElseIf (columnname = "HSN") Then
                                strChildItem &= "<td  width=""50"">"
                                strChildItem &= ds.Tables("child").Rows(i - 1).Item(j).ToString()
                                strChildItem &= "</td>"
                            ElseIf (columnname = "UOM") Then
                                strChildItem &= "<td  width=""80"">"
                                strChildItem &= ds.Tables("child").Rows(i - 1).Item(j).ToString()
                                strChildItem &= "</td>"
                            ElseIf (columnname = "Quantity") Then
                                strChildItem &= "<td  width=""80"">"
                                strChildItem &= ds.Tables("child").Rows(i - 1).Item(j).ToString()
                                strChildItem &= "</td>"
                            ElseIf (columnname = "Unit Price") Then
                                strChildItem &= "<td  width=""80"">"
                                strChildItem &= ds.Tables("child").Rows(i - 1).Item(j).ToString()
                                strChildItem &= "</td>"
                            ElseIf (columnname = "Total Price") Then
                                strChildItem &= "<td  width=""100"">"
                                strChildItem &= ds.Tables("child").Rows(i - 1).Item(j).ToString()
                                strChildItem &= "</td>"
                            ElseIf (columnname = "Remarks") Then
                                strChildItem &= "<td  width=""250"">"
                                strChildItem &= ds.Tables("child").Rows(i - 1).Item(j).ToString()
                                strChildItem &= "</td>"
                            Else
                                strChildItem &= "<td>"
                                strChildItem &= ds.Tables("child").Rows(i - 1).Item(j).ToString()
                                strChildItem &= "</td>"
                            End If


                            'strChildItem &= "<td>"
                            'strChildItem &= ds.Tables("child").Rows(i - 1).Item(j).ToString()
                            'strChildItem &= "</td>"
                        End If
                    Next
                    strChildItem &= "</tr>"
                Next
                strChildItem &= "</table></div>"
                body = body.Replace("[child item]", strChildItem)
            Else
                For i As Integer = 0 To ds.Tables("child").Rows.Count
                    strChildItem &= "<tr>"
                    For j As Integer = 0 To ds.Tables("child").Columns.Count - 1
                        strChildItem &= "<td>"
                        If i = 0 Then
                            strChildItem &= ds.Tables("child").Columns(j).ColumnName
                        Else
                            strChildItem &= ds.Tables("child").Rows(i - 1).Item(j).ToString()
                        End If
                        strChildItem &= "</td>"
                    Next
                    strChildItem &= "</tr>"
                Next
                strChildItem &= "</table></div>"

                body = body.Replace("[child item]", strChildItem)
            End If

            'For i As Integer = 0 To ds.Tables("child").Rows.Count
            '    strChildItem &= "<tr>"
            '    For j As Integer = 0 To ds.Tables("child").Columns.Count - 1
            '        strChildItem &= "<td text-align:left>"
            '        If i = 0 Then
            '            strChildItem &= ds.Tables("child").Columns(j).ColumnName
            '        Else
            '            strChildItem &= ds.Tables("child").Rows(i - 1).Item(j).ToString()
            '        End If
            '        strChildItem &= "</td>"
            '    Next
            '    strChildItem &= "</tr>"
            'Next
            'strChildItem &= "</table></div>"
            'body = body.Replace("[child item]", strChildItem)
            'Print first child item added on 30 Oct 2017
            da.SelectCommand.CommandType = CommandType.Text
            da.SelectCommand.CommandText = childQryFirst.Replace("@tid", docid)
            Dim childFirst As New DataTable
            da.Fill(childFirst)

            If childFirst.Rows.Count > 0 Then
                Dim strChildItemFirst As String = "<div><table width=""100%"" border=""0.5"" text-align=""left""  >"
                For i As Integer = 0 To childFirst.Rows.Count
                    strChildItemFirst &= "<tr>"
                    For j As Integer = 0 To childFirst.Columns.Count - 1
                        strChildItemFirst &= "<td text-align:left>"
                        If i = 0 Then
                            strChildItemFirst &= childFirst.Columns(j).ColumnName
                        Else
                            strChildItemFirst &= childFirst.Rows(i - 1).Item(j).ToString()
                        End If
                        strChildItemFirst &= "</td>"
                    Next
                    strChildItemFirst &= "</tr>"
                Next
                strChildItemFirst &= "</table></div>"
                body = body.Replace("[child item1]", strChildItemFirst)
            End If
            'Print Second child item added on 30 Oct 2017
            If childQrySecond <> "" Then
                da.SelectCommand.CommandType = CommandType.Text
                da.SelectCommand.CommandText = childQrySecond.Replace("@tid", docid)
                Dim childSecond As New DataTable
                da.Fill(childSecond)
                If childSecond.Rows.Count > 0 Then
                    Dim strChildItemSecond As String = "<div><table width=""100%"" border=""0.5"" text-align=""left""  >"
                    For i As Integer = 0 To childSecond.Rows.Count
                        strChildItemSecond &= "<tr>"
                        For j As Integer = 0 To childSecond.Columns.Count - 1
                            strChildItemSecond &= "<td text-align:left>"
                            If i = 0 Then
                                strChildItemSecond &= childSecond.Columns(j).ColumnName
                            Else
                                strChildItemSecond &= childSecond.Rows(i - 1).Item(j).ToString()
                            End If
                            strChildItemSecond &= "</td>"
                        Next
                        strChildItemSecond &= "</tr>"
                    Next
                    strChildItemSecond &= "</table></div>"
                    body = body.Replace("[child item2]", strChildItemSecond)
                End If
            End If

            'Print Third child item added on 30 Oct 2017
            If childQryThird <> "" Then
                da.SelectCommand.CommandType = CommandType.Text
                da.SelectCommand.CommandText = childQryThird.Replace("@tid", docid)
                Dim childThird As New DataTable
                da.Fill(childThird)
                If childThird.Rows.Count > 0 Then
                    Dim strChildItemThird As String = "<div><table width=""100%"" border=""0.5"" text-align=""left""  >"
                    For i As Integer = 0 To childThird.Rows.Count
                        strChildItemThird &= "<tr>"
                        For j As Integer = 0 To childThird.Columns.Count - 1
                            strChildItemThird &= "<td text-align:left>"
                            If i = 0 Then
                                strChildItemThird &= childThird.Columns(j).ColumnName
                            Else
                                strChildItemThird &= childThird.Rows(i - 1).Item(j).ToString()
                            End If
                            strChildItemThird &= "</td>"
                        Next
                        strChildItemThird &= "</tr>"
                    Next
                    strChildItemThird &= "</table></div>"
                    body = body.Replace("[child item3]", strChildItemThird)
                End If
            End If

            'Print Fourth child item added on 30 Oct 2017
            If childQryFourth <> "" Then
                da.SelectCommand.CommandType = CommandType.Text
                da.SelectCommand.CommandText = childQryFourth.Replace("@tid", docid)
                Dim childFourth As New DataTable
                da.Fill(childFourth)
                If childFourth.Rows.Count > 0 Then
                    Dim strChildItemFourth As String = "<div><table width=""100%"" border=""0.5"" text-align=""left""  >"
                    For i As Integer = 0 To childFourth.Rows.Count
                        strChildItemFourth &= "<tr>"
                        For j As Integer = 0 To childFourth.Columns.Count - 1
                            strChildItemFourth &= "<td text-align:left>"
                            If i = 0 Then
                                strChildItemFourth &= childFourth.Columns(j).ColumnName
                            Else
                                strChildItemFourth &= childFourth.Rows(i - 1).Item(j).ToString()
                            End If
                            strChildItemFourth &= "</td>"
                        Next
                        strChildItemFourth &= "</tr>"
                    Next
                    strChildItemFourth &= "</table></div>"
                    body = body.Replace("[child item4]", strChildItemFourth)
                End If
            End If

            'Print Fifth child item added on 30 Oct 2017
            If childQryFifth <> "" Then
                da.SelectCommand.CommandType = CommandType.Text
                da.SelectCommand.CommandText = childQryFifth.Replace("@tid", docid)
                Dim childFifth As New DataTable
                da.Fill(childFifth)
                If childFifth.Rows.Count > 0 Then
                    Dim strChildItemFifth As String = "<div><table width=""100%"" border=""0.5"" text-align=""left""  >"
                    For i As Integer = 0 To childFifth.Rows.Count
                        strChildItemFifth &= "<tr>"
                        For j As Integer = 0 To childFifth.Columns.Count - 1
                            strChildItemFifth &= "<td text-align:left>"
                            If i = 0 Then
                                strChildItemFifth &= childFifth.Columns(j).ColumnName
                            Else
                                strChildItemFifth &= childFifth.Rows(i - 1).Item(j).ToString()
                            End If
                            strChildItemFifth &= "</td>"
                        Next
                        strChildItemFifth &= "</tr>"
                    Next
                    strChildItemFifth &= "</table></div>"
                    body = body.Replace("[child item5]", strChildItemFifth)
                End If
            End If

            If body.Contains("[movdtl]") Then
                Dim hub As String = ds.Tables("main").Rows(0).Item("Hub Name").ToString()
                da.SelectCommand.CommandText = moveqry.Replace("@hub", hub)
                da.Fill(ds, "movdtl")
                Dim stmov As String = "<div><table width=""100%"" border=""0.5"" text-align=""left""  >"
                Dim preMovvVal As String = ""
                'For i As Integer = 0 To ds.Tables("movdtl").Rows.Count - 1
                '    If preMovvVal = ds.Tables("movdtl").Rows(i).Item(0).ToString() Then
                '        preMovvVal = ds.Tables("movdtl").Rows(i).Item(0).ToString()
                '        ds.Tables("movdtl").Rows(i).Item(0) = ""
                '    Else
                '        preMovvVal = ds.Tables("movdtl").Rows(i).Item(0).ToString()
                '    End If
                'Next

                For i As Integer = 0 To ds.Tables("movdtl").Rows.Count
                    stmov &= "<tr>"
                    For j As Integer = 0 To ds.Tables("movdtl").Columns.Count - 1
                        stmov &= "<td text-align:left>"
                        If i = 0 Then
                            stmov &= ds.Tables("movdtl").Columns(j).ColumnName
                        Else
                            stmov &= ds.Tables("movdtl").Rows(i - 1).Item(j).ToString()
                        End If
                        stmov &= "</td>"
                    Next
                    stmov &= "</tr>"
                Next
                stmov &= "</table></div>"
                body = body.Replace("[movdtl]", stmov)
            End If

            If dc.ToString.ToUpper = "QUOTATION DOMESTIC MOVEMENT" And curdocnature = "MODIFY" Then
                filename = filename & "_" & docid & "_" & Now.Millisecond
            Else
                filename = filename & "_" & docid
            End If

            body = body.Replace("[child item]", "")
            body = body.Replace("[child item1]", "")
            body = body.Replace("[child item2]", "")
            body = body.Replace("[child item3]", "")
            body = body.Replace("[child item4]", "")
            body = body.Replace("[child item5]", "")

            WritePdf(body, filename)
            Return filename
        Catch ex As Exception
            Throw
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

    Public Function GetDraftPDF(ByVal EID As String, ByVal draftID As Integer) As String
        Dim ret As String = "fail"
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString.ToString()
        Dim con As SqlConnection = Nothing
        Dim da As SqlDataAdapter = Nothing
        Try
            Dim Query = "select F.FormCaption,D.* from MMM_MST_doc_draft D INNER JOIN MMM_MST_Forms F ON F.FormName=D.DocumentType where D.eid=" & EID & " and D.tid=" & draftID & ""
            Query = Query & ";" & "SELECT * FROM MMM_PRINT_Template where EID=" & EID & " and  Draft='Draft' AND DocumentType=(" & "select Documenttype from MMM_MST_doc_draft where tid=" & draftID & ")"
            con = New SqlConnection(conStr)
            da = New SqlDataAdapter(Query, con)
            Dim ds As New DataSet
            da.Fill(ds)
            If ds.Tables(0).Rows.Count > 0 And ds.Tables(1).Rows.Count > 0 Then
                Dim ob As New DMSUtil
                'ds1.Tables("dataset1").Rows(0).Item(0).ToString() & "_" & eid & "_" & "print" & ds1.Tables("dataset").Rows(k).Item("tid").ToString(), tid, en)
                Dim pdffile As String = ""
                Dim FileName = ds.Tables(0).Rows(0).Item("FormCaption") & "_" & EID & "_" & "print" & ds.Tables(1).Rows(0).Item("tid").ToString()
                pdffile = ob.GenerateMailPdf(FileName, draftID, ds.Tables(0).Rows(0).Item("documenttype"), "DRAFT")
                Dim path As String = "/MailAttach/" & pdffile & ".pdf"
                ret = path
            End If
        Catch ex As Exception
            Return "fail"
        Finally
            If Not con Is Nothing Then
                con.Close()
                con.Dispose()
            End If
            If Not da Is Nothing Then
                da.Dispose()
            End If
        End Try
        Return ret

    End Function

    Public Function GeneratePdfPOFlip(ByVal filename As String, ByVal docid As Integer, ByVal dc As String, ByVal EID As Integer, Optional ISDraft As String = "NA", Optional ByVal curdocnature As String = "SE") As String
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim da As New SqlDataAdapter("", con)
        'try Catch Block Added by Ajeet Kumar :Date::22 May 2014
        Try
            If ISDraft <> "NA" Then
                da.SelectCommand.CommandText = "select * from MMM_Print_Template where template_name='" & filename & "' and Draft='DRAFT' and eid=" & EID & ""
            Else
                da.SelectCommand.CommandText = "select * from MMM_Print_Template where template_name='" & filename & "' and draft='original' and eid=" & EID & ""
            End If
            Dim ds As New DataSet
            Dim path As String = HostingEnvironment.MapPath("~/MailAttach/")

            Dim FPath As String = HostingEnvironment.MapPath("~/MailAttach/")
            Dim Folder As New DirectoryInfo(HostingEnvironment.MapPath("~/MailAttach/"))


            da.Fill(ds, "data1")
            da.SelectCommand.CommandType = CommandType.Text
            da.SelectCommand.CommandText = "select formcaption from mmm_mst_forms where formname='" & dc & "' and eid='" & ds.Tables("data1").Rows(0).Item("EID").ToString() & "'"
            da.Fill(ds, "data2")

            ' Dim fname1 As String = ds.Tables("data2").Rows(0).Item(0).ToString() & "_" & docid & "_" & ds.Tables("data1").Rows(0).Item(0).ToString()

            If ds.Tables("data1").Rows.Count <> 1 Then
                Exit Function
            End If
            Dim body As String = ds.Tables("data1").Rows(0).Item("body").ToString()
            Dim MainQry As String = ds.Tables("data1").Rows(0).Item("qry").ToString()
            Dim childQry As String = ds.Tables("data1").Rows(0).Item("SQL_CHILDITEM").ToString()
            Dim childQryFirst As String = ds.Tables("data1").Rows(0).Item("sql_child1").ToString()
            Dim childQrySecond As String = ds.Tables("data1").Rows(0).Item("sql_child2").ToString()
            Dim childQryThird As String = ds.Tables("data1").Rows(0).Item("sql_child3").ToString()
            Dim childQryFourth As String = ds.Tables("data1").Rows(0).Item("sql_child4").ToString()
            Dim childQryFifth As String = ds.Tables("data1").Rows(0).Item("sql_child5").ToString()
            Dim DocType As String = ds.Tables("data1").Rows(0).Item("Documenttype").ToString()
            Dim moveqry As String = ds.Tables("data1").Rows(0).Item("SQL_MOV_DTL").ToString()
            ' Start  signature image
            Try
                'Dim EID As String = ds.Tables("data1").Rows(0).Item("EID").ToString()
                da.SelectCommand.CommandText = "select fieldmapping from mmm_mst_fields where documenttype='" & dc & "' and fieldtype='signature'"
                da.Fill(ds, "data3")
                Dim fldmap As String = ""

                For l As Integer = 0 To ds.Tables("data3").Rows.Count - 1
                    Dim dtSig As New DataTable()
                    fldmap = ds.Tables("data3").Rows(l).Item("fieldmapping").ToString()
                    If ISDraft <> "NA" Then
                        da.SelectCommand.CommandText = "select " & fldmap & " from mmm_mst_doc_draft where documenttype='" & dc & "' and tid=" & docid & ""
                    Else
                        da.SelectCommand.CommandText = "select " & fldmap & " from mmm_mst_doc where documenttype='" & dc & "' and tid=" & docid & ""
                    End If

                    da.Fill(dtSig)
                    Dim Sig_fldVal As String = dtSig.Rows(0).Item(0).ToString()
                    'Dim Sig_fldVal As String = ""
                    If dtSig.Rows.Count <> 0 Then
                        Sig_fldVal = dtSig.Rows(0).Item(0).ToString()
                        If Sig_fldVal.ToUpper() = "NOSIGNATURE.PNG" Then
                            If body.Contains("{SignImg_" & fldmap & "}") Then
                                body = body.Replace("{SignImg_" & fldmap & "}", "images/NoSignature.png")
                            End If
                        ElseIf Len(Sig_fldVal) > 4 Then
                            If body.Contains("{SignImg_" & fldmap & "}") Then

                                ' CHANGE ON 1 JULY FOR SIG PROBLEM IN QUATATION
                                ' body = body.Replace("{SignImg_" & fldmap & "}", "docs/" & EID & "/" & Sig_fldVal)
                                body = body.Replace("{SignImg_" & fldmap & "}", "docs/" & Sig_fldVal)
                                'body = body.Replace("{SignImg_" & fldmap & "}", "docs/" & Sig_fldVal)
                            End If
                        ElseIf Sig_fldVal = "" Then
                            If body.Contains("{SignImg_" & fldmap & "}") Then
                                body = body.Replace("{SignImg_" & fldmap & "}", "images/NoSignature.png")
                            End If
                        Else
                            If body.Contains("{SignImg_" & fldmap & "}") Then
                                body = body.Replace("{SignImg_" & fldmap & "}", "images/NoSignature.png")
                            End If
                        End If
                    End If

                    dtSig.Dispose()
                Next
            Catch ex As Exception

            End Try
            ' End signature image

            da.SelectCommand.CommandText = MainQry.Replace("@tid", docid)
            da.Fill(ds, "main")
            If dc.ToString.ToUpper = "QUOTATION DOMESTIC MOVEMENT" And curdocnature = "CREATE" Then
                body = body.Replace("Discount", "")
                body = body.Replace("-[]", "")
            End If

            For j As Integer = 0 To ds.Tables("main").Columns.Count - 1
                body = body.Replace("[" & ds.Tables("main").Columns(j).ColumnName & "]", ds.Tables("main").Rows(0).Item(j).ToString())
            Next
            If Len(childQry) > 2 Then
                da.SelectCommand.CommandType = CommandType.Text
                da.SelectCommand.CommandText = childQry.Replace("@tid", docid)
                da.Fill(ds, "child")
                'ds.Tables("child").DefaultView.Sort = ds.Tables("child").Columns(0).ColumnName & " ASC"
                ds.Dispose()
                Dim strChildItem As String = "<div><table width=""100%"" border=""0.5"" text-align=""left""  >"
                Dim prevVal As String = ""
                For i As Integer = 0 To ds.Tables("child").Rows.Count - 1
                    If prevVal = ds.Tables("child").Rows(i).Item(0).ToString() Then
                        prevVal = ds.Tables("child").Rows(i).Item(0).ToString()
                        ds.Tables("child").Rows(i).Item(0) = ""
                    Else
                        prevVal = ds.Tables("child").Rows(i).Item(0).ToString()
                    End If
                Next

                For i As Integer = 0 To ds.Tables("child").Rows.Count
                    strChildItem &= "<tr>"
                    For j As Integer = 0 To ds.Tables("child").Columns.Count - 1
                        strChildItem &= "<td text-align:left>"
                        If i = 0 Then
                            strChildItem &= ds.Tables("child").Columns(j).ColumnName
                        Else
                            strChildItem &= ds.Tables("child").Rows(i - 1).Item(j).ToString()
                        End If
                        strChildItem &= "</td>"
                    Next
                    strChildItem &= "</tr>"
                Next
                strChildItem &= "</table></div>"
                body = body.Replace("[child item]", strChildItem)
                'Print first child item added on 30 Oct 2017
                If body.Contains("[movdtl]") Then
                    Dim hub As String = ds.Tables("main").Rows(0).Item("Hub Name").ToString()
                    da.SelectCommand.CommandText = moveqry.Replace("@hub", hub)
                    da.Fill(ds, "movdtl")
                    Dim stmov As String = "<div><table width=""100%"" border=""0.5"" text-align=""left""  >"
                    Dim preMovvVal As String = ""
                    'For i As Integer = 0 To ds.Tables("movdtl").Rows.Count - 1
                    '    If preMovvVal = ds.Tables("movdtl").Rows(i).Item(0).ToString() Then
                    '        preMovvVal = ds.Tables("movdtl").Rows(i).Item(0).ToString()
                    '        ds.Tables("movdtl").Rows(i).Item(0) = ""
                    '    Else
                    '        preMovvVal = ds.Tables("movdtl").Rows(i).Item(0).ToString()
                    '    End If
                    'Next

                    For i As Integer = 0 To ds.Tables("movdtl").Rows.Count
                        stmov &= "<tr>"
                        For j As Integer = 0 To ds.Tables("movdtl").Columns.Count - 1
                            stmov &= "<td text-align:left>"
                            If i = 0 Then
                                stmov &= ds.Tables("movdtl").Columns(j).ColumnName
                            Else
                                stmov &= ds.Tables("movdtl").Rows(i - 1).Item(j).ToString()
                            End If
                            stmov &= "</td>"
                        Next
                        stmov &= "</tr>"
                    Next
                    stmov &= "</table></div>"
                    body = body.Replace("[movdtl]", stmov)
                End If
            End If
            da.SelectCommand.CommandText = childQryFirst.Replace("@tid", docid)
            da.Fill(ds, "Child1")
            For j As Integer = 0 To ds.Tables("Child1").Columns.Count - 1
                body = body.Replace("[" & ds.Tables("Child1").Columns(j).ColumnName & "]", ds.Tables("Child1").Rows(0).Item(j).ToString())
            Next

            If dc.ToString.ToUpper = "QUOTATION DOMESTIC MOVEMENT" And curdocnature = "MODIFY" Then
                filename = filename & "_" & docid & "_" & Now.Millisecond
            Else
                filename = filename & "_" & docid
            End If

            body = body.Replace("[child item]", "")
            body = body.Replace("[child item1]", "")

            WritePdf(body, filename)
            Return filename
        Catch ex As Exception
            Throw
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


    '' bkup 
    'Public Function GeneratePdfPOFlip(ByVal filename As String, ByVal docid As Integer, ByVal dc As String, ByVal EID As Integer, Optional ISDraft As String = "NA", Optional ByVal curdocnature As String = "SE") As String
    '    Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
    '    Dim con As New SqlConnection(conStr)
    '    Dim da As New SqlDataAdapter("", con)
    '    'try Catch Block Added by Ajeet Kumar :Date::22 May 2014
    '    Try
    '        If ISDraft <> "NA" Then
    '            da.SelectCommand.CommandText = "select * from MMM_Print_Template where template_name='" & filename & "' and Draft='DRAFT' and eid=" & EID & ""
    '        Else
    '            da.SelectCommand.CommandText = "select * from MMM_Print_Template where template_name='" & filename & "' and draft='original' and eid=" & EID & ""
    '        End If
    '        Dim ds As New DataSet
    '        Dim path As String = HostingEnvironment.MapPath("~/MailAttach/")

    '        Dim FPath As String = HostingEnvironment.MapPath("~/MailAttach/")
    '        Dim Folder As New DirectoryInfo(HostingEnvironment.MapPath("~/MailAttach/"))


    '        da.Fill(ds, "data1")
    '        da.SelectCommand.CommandType = CommandType.Text
    '        da.SelectCommand.CommandText = "select formcaption from mmm_mst_forms where formname='" & dc & "' and eid='" & ds.Tables("data1").Rows(0).Item("EID").ToString() & "'"
    '        da.Fill(ds, "data2")

    '        ' Dim fname1 As String = ds.Tables("data2").Rows(0).Item(0).ToString() & "_" & docid & "_" & ds.Tables("data1").Rows(0).Item(0).ToString()

    '        If ds.Tables("data1").Rows.Count <> 1 Then
    '            Exit Function
    '        End If
    '        Dim body As String = ds.Tables("data1").Rows(0).Item("body").ToString()
    '        Dim MainQry As String = ds.Tables("data1").Rows(0).Item("qry").ToString()
    '        Dim childQry As String = ds.Tables("data1").Rows(0).Item("SQL_CHILDITEM").ToString()
    '        Dim childQryFirst As String = ds.Tables("data1").Rows(0).Item("sql_child1").ToString()
    '        Dim childQrySecond As String = ds.Tables("data1").Rows(0).Item("sql_child2").ToString()
    '        Dim childQryThird As String = ds.Tables("data1").Rows(0).Item("sql_child3").ToString()
    '        Dim childQryFourth As String = ds.Tables("data1").Rows(0).Item("sql_child4").ToString()
    '        Dim childQryFifth As String = ds.Tables("data1").Rows(0).Item("sql_child5").ToString()
    '        Dim DocType As String = ds.Tables("data1").Rows(0).Item("Documenttype").ToString()
    '        Dim moveqry As String = ds.Tables("data1").Rows(0).Item("SQL_MOV_DTL").ToString()
    '        ' Start  signature image
    '        Try
    '            'Dim EID As String = ds.Tables("data1").Rows(0).Item("EID").ToString()
    '            da.SelectCommand.CommandText = "select fieldmapping from mmm_mst_fields where documenttype='" & dc & "' and fieldtype='signature'"
    '            da.Fill(ds, "data3")
    '            Dim fldmap As String = ""

    '            For l As Integer = 0 To ds.Tables("data3").Rows.Count - 1
    '                Dim dtSig As New DataTable()
    '                fldmap = ds.Tables("data3").Rows(l).Item("fieldmapping").ToString()
    '                If ISDraft <> "NA" Then
    '                    da.SelectCommand.CommandText = "select " & fldmap & " from mmm_mst_doc_draft where documenttype='" & dc & "' and tid=" & docid & ""
    '                Else
    '                    da.SelectCommand.CommandText = "select " & fldmap & " from mmm_mst_doc where documenttype='" & dc & "' and tid=" & docid & ""
    '                End If

    '                da.Fill(dtSig)
    '                Dim Sig_fldVal As String = dtSig.Rows(0).Item(0).ToString()
    '                'Dim Sig_fldVal As String = ""
    '                If dtSig.Rows.Count <> 0 Then
    '                    Sig_fldVal = dtSig.Rows(0).Item(0).ToString()
    '                    If Sig_fldVal.ToUpper() = "NOSIGNATURE.PNG" Then
    '                        If body.Contains("{SignImg_" & fldmap & "}") Then
    '                            body = body.Replace("{SignImg_" & fldmap & "}", "images/NoSignature.png")
    '                        End If
    '                    ElseIf Len(Sig_fldVal) > 4 Then
    '                        If body.Contains("{SignImg_" & fldmap & "}") Then

    '                            ' CHANGE ON 1 JULY FOR SIG PROBLEM IN QUATATION
    '                            ' body = body.Replace("{SignImg_" & fldmap & "}", "docs/" & EID & "/" & Sig_fldVal)
    '                            body = body.Replace("{SignImg_" & fldmap & "}", "docs/" & Sig_fldVal)
    '                            'body = body.Replace("{SignImg_" & fldmap & "}", "docs/" & Sig_fldVal)
    '                        End If
    '                    ElseIf Sig_fldVal = "" Then
    '                        If body.Contains("{SignImg_" & fldmap & "}") Then
    '                            body = body.Replace("{SignImg_" & fldmap & "}", "images/NoSignature.png")
    '                        End If
    '                    Else
    '                        If body.Contains("{SignImg_" & fldmap & "}") Then
    '                            body = body.Replace("{SignImg_" & fldmap & "}", "images/NoSignature.png")
    '                        End If
    '                    End If
    '                End If

    '                dtSig.Dispose()
    '            Next
    '        Catch ex As Exception

    '        End Try
    '        ' End signature image

    '        da.SelectCommand.CommandText = MainQry.Replace("@tid", docid)
    '        da.Fill(ds, "main")
    '        If dc.ToString.ToUpper = "QUOTATION DOMESTIC MOVEMENT" And curdocnature = "CREATE" Then
    '            body = body.Replace("Discount", "")
    '            body = body.Replace("-[]", "")
    '        End If

    '        For j As Integer = 0 To ds.Tables("main").Columns.Count - 1
    '            body = body.Replace("[" & ds.Tables("main").Columns(j).ColumnName & "]", ds.Tables("main").Rows(0).Item(j).ToString())
    '        Next
    '        da.SelectCommand.CommandType = CommandType.Text
    '        da.SelectCommand.CommandText = childQry.Replace("@tid", docid)
    '        da.Fill(ds, "child")
    '        'ds.Tables("child").DefaultView.Sort = ds.Tables("child").Columns(0).ColumnName & " ASC"
    '        ds.Dispose()
    '        Dim strChildItem As String = "<div><table width=""100%"" border=""0.5"" text-align=""left""  >"
    '        Dim prevVal As String = ""
    '        For i As Integer = 0 To ds.Tables("child").Rows.Count - 1
    '            If prevVal = ds.Tables("child").Rows(i).Item(0).ToString() Then
    '                prevVal = ds.Tables("child").Rows(i).Item(0).ToString()
    '                ds.Tables("child").Rows(i).Item(0) = ""
    '            Else
    '                prevVal = ds.Tables("child").Rows(i).Item(0).ToString()
    '            End If
    '        Next

    '        For i As Integer = 0 To ds.Tables("child").Rows.Count
    '            strChildItem &= "<tr>"
    '            For j As Integer = 0 To ds.Tables("child").Columns.Count - 1
    '                strChildItem &= "<td text-align:left>"
    '                If i = 0 Then
    '                    strChildItem &= ds.Tables("child").Columns(j).ColumnName
    '                Else
    '                    strChildItem &= ds.Tables("child").Rows(i - 1).Item(j).ToString()
    '                End If
    '                strChildItem &= "</td>"
    '            Next
    '            strChildItem &= "</tr>"
    '        Next
    '        strChildItem &= "</table></div>"
    '        body = body.Replace("[child item]", strChildItem)
    '        'Print first child item added on 30 Oct 2017
    '        da.SelectCommand.CommandText = childQryFirst.Replace("@tid", docid)
    '        da.Fill(ds, "Child1")
    '        For j As Integer = 0 To ds.Tables("Child1").Columns.Count - 1
    '            body = body.Replace("[" & ds.Tables("Child1").Columns(j).ColumnName & "]", ds.Tables("Child1").Rows(0).Item(j).ToString())
    '        Next

    '        If body.Contains("[movdtl]") Then
    '            Dim hub As String = ds.Tables("main").Rows(0).Item("Hub Name").ToString()
    '            da.SelectCommand.CommandText = moveqry.Replace("@hub", hub)
    '            da.Fill(ds, "movdtl")
    '            Dim stmov As String = "<div><table width=""100%"" border=""0.5"" text-align=""left""  >"
    '            Dim preMovvVal As String = ""
    '            'For i As Integer = 0 To ds.Tables("movdtl").Rows.Count - 1
    '            '    If preMovvVal = ds.Tables("movdtl").Rows(i).Item(0).ToString() Then
    '            '        preMovvVal = ds.Tables("movdtl").Rows(i).Item(0).ToString()
    '            '        ds.Tables("movdtl").Rows(i).Item(0) = ""
    '            '    Else
    '            '        preMovvVal = ds.Tables("movdtl").Rows(i).Item(0).ToString()
    '            '    End If
    '            'Next

    '            For i As Integer = 0 To ds.Tables("movdtl").Rows.Count
    '                stmov &= "<tr>"
    '                For j As Integer = 0 To ds.Tables("movdtl").Columns.Count - 1
    '                    stmov &= "<td text-align:left>"
    '                    If i = 0 Then
    '                        stmov &= ds.Tables("movdtl").Columns(j).ColumnName
    '                    Else
    '                        stmov &= ds.Tables("movdtl").Rows(i - 1).Item(j).ToString()
    '                    End If
    '                    stmov &= "</td>"
    '                Next
    '                stmov &= "</tr>"
    '            Next
    '            stmov &= "</table></div>"
    '            body = body.Replace("[movdtl]", stmov)
    '        End If

    '        If dc.ToString.ToUpper = "QUOTATION DOMESTIC MOVEMENT" And curdocnature = "MODIFY" Then
    '            filename = filename & "_" & docid & "_" & Now.Millisecond
    '        Else
    '            filename = filename & "_" & docid
    '        End If

    '        body = body.Replace("[child item]", "")
    '        body = body.Replace("[child item1]", "")

    '        WritePdf(body, filename)
    '        Return filename
    '    Catch ex As Exception
    '        Throw
    '    Finally
    '        If Not da Is Nothing Then
    '            da.Dispose()
    '        End If
    '        If Not con Is Nothing Then
    '            con.Close()
    '            con.Dispose()
    '        End If
    '    End Try
    'End Function

    'Public Function GenerateMailPdf(ByVal filename As String, ByVal docid As Integer, ByVal dc As String) As String
    '    Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
    '    Dim con As New SqlConnection(conStr)
    '    Dim da As SqlDataAdapter = New SqlDataAdapter("select * from MMM_Print_Template where template_name='" & filename & "'", con)
    '    Dim ds As New DataSet
    '    Dim path As String = HostingEnvironment.MapPath("~/MailAttach/")

    '    'Dim FLAG As Boolean = DS.Tables("TEMP").Rows(0).Item("MSGBODY").Contains("MailAttach")
    '    Dim FPath As String = HostingEnvironment.MapPath("~/MailAttach/")
    '    Dim Folder As New DirectoryInfo(HostingEnvironment.MapPath("~/MailAttach/"))
    '    'If Folder.Exists Then
    '    '    For Each fl As FileInfo In Folder.GetFiles()
    '    '        Try
    '    '            fl.Delete()
    '    '        Catch ex As Exception
    '    '        End Try
    '    '    Next
    '    'End If

    '    'If File.Exists(FPath & fname1 & ".pdf") Then
    '    '    File.Delete(FPath & fname1 & ".pdf")
    '    'End If

    '    'If DS.Tables("TEMP").Rows(0).Item("MSGBODY").Contains = "MailAttach" Then

    '    da.Fill(ds, "data1")
    '    da.SelectCommand.CommandType = CommandType.Text
    '    da.SelectCommand.CommandText = "select formcaption from mmm_mst_forms where formname='" & dc & "'"
    '    da.Fill(ds, "data2")
    '    Dim fname1 As String = ds.Tables("data2").Rows(0).Item(0).ToString() & "_" & docid & "_" & ds.Tables("data1").Rows(0).Item(0).ToString()
    '    If ds.Tables("data1").Rows.Count <> 1 Then
    '        da.Dispose()
    '        con.Dispose()
    '        Exit Function
    '    End If
    '    Dim body As String = ds.Tables("data1").Rows(0).Item("body").ToString()
    '    Dim MainQry As String = ds.Tables("data1").Rows(0).Item("qry").ToString()
    '    Dim childQry As String = ds.Tables("data1").Rows(0).Item("SQL_CHILDITEM").ToString()
    '    Dim DocType As String = ds.Tables("data1").Rows(0).Item("Documenttype").ToString()

    '    da.SelectCommand.CommandText = MainQry.Replace("@tid", docid)
    '    da.Fill(ds, "main")

    '    For j As Integer = 0 To ds.Tables("main").Columns.Count - 1
    '        body = body.Replace("[" & ds.Tables("main").Columns(j).ColumnName & "]", ds.Tables("main").Rows(0).Item(j).ToString())
    '    Next

    '    da.SelectCommand.CommandText = childQry.Replace("@tid", docid)
    '    da.Fill(ds, "child")

    '    'ds.Tables("child").DefaultView.Sort = ds.Tables("child").Columns(0).ColumnName & " ASC"
    '    ds.Dispose()
    '    con.Dispose()

    '    Dim strChildItem As String = "<div><table width=""100%"" border=""0.5"" text-align=""left""  >"
    '    Dim prevVal As String = ""
    '    For i As Integer = 0 To ds.Tables("child").Rows.Count - 1
    '        If prevVal = ds.Tables("child").Rows(i).Item(0).ToString() Then
    '            prevVal = ds.Tables("child").Rows(i).Item(0).ToString()
    '            ds.Tables("child").Rows(i).Item(0) = ""
    '        Else
    '            prevVal = ds.Tables("child").Rows(i).Item(0).ToString()
    '        End If
    '    Next

    '    For i As Integer = 0 To ds.Tables("child").Rows.Count
    '        strChildItem &= "<tr>"
    '        For j As Integer = 0 To ds.Tables("child").Columns.Count - 1
    '            strChildItem &= "<td text-align:left>"
    '            If i = 0 Then
    '                strChildItem &= ds.Tables("child").Columns(j).ColumnName
    '            Else
    '                strChildItem &= ds.Tables("child").Rows(i - 1).Item(j).ToString()
    '            End If
    '            strChildItem &= "</td>"
    '        Next
    '        strChildItem &= "</tr>"
    '    Next
    '    strChildItem &= "</table></div>"
    '    body = body.Replace("[child item]", strChildItem)
    '    WritePdf(body, fname1)
    '    Return fname1
    'End Function

    Private Sub WritePdf(ByVal mBody As String, ByVal filename As String)
        Try
            Dim _strRepeater As New StringBuilder(mBody)
            Dim _ObjHtm As New Html32TextWriter(New System.IO.StringWriter(_strRepeater))
            'StringBuilder _strRepeater = new StringBuilder();
            'Html32TextWriter _ObjHtm = new Html32TextWriter(new System.IO.StringWriter(_strRepeater));
            'lblHTML.Text = mBody;
            'lblHTML.RenderControl(_ObjHtm);

            Dim _str As String = _strRepeater.ToString()
            'CREATE A UNIQUE NUMBER FOR PDF FILE NAMING USING DATE TIME STAMP
            'string filename = string.Format("{0:d7}", (DateTime.Now.Ticks / 10) % 10000000);
            'CREATE DOCUMENT
            Dim document As New Document(New iTextSharp.text.Rectangle(850.0F, 1100.0F))
			ServicePointManager.Expect100Continue = True
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
            'SAVE DOCUMENT. CHECK IF YOU HAVE WRITE PERMISSION OR NOT
            PdfWriter.GetInstance(document, New FileStream(HostingEnvironment.MapPath("~/MailAttach/" & filename & ".pdf"), FileMode.Create))
            document.Open()
            Dim htmlarraylist As List(Of IElement) = iTextSharp.text.html.simpleparser.HTMLWorker.ParseToList(New StringReader(_str), Nothing)
            'add the collection to the document
            For k As Integer = 0 To htmlarraylist.Count - 1
                document.Add(DirectCast(htmlarraylist(k), IElement))
            Next
            'NOW SEND EMAIL TO USER WITH ATTACHMENT
            'sm.sendMailWithAttachment(toMail, subject, mBody, Server.MapPath("~/pdfs/" + filename + ".pdf").ToString());
            document.Close()
        Catch generatedExceptionName As Exception
        End Try
    End Sub

    Private Sub SendMailPdf(ByVal Mto As String, ByVal cc As String, ByVal bcc As String, ByVal MSubject As String, ByVal MBody As String, ByVal backuppath As String)
        Try
            If Left(Mto, 1) = "{" Then
                Exit Sub
            End If
            'Dim fname1 As String = ""
            Dim fname2 As String = ""
            Dim att As Attachment = Nothing
            ''Dim path As String = System.Web.HttpContext.Current.Server.MapPath("~/MailAttach/")
            If Not String.IsNullOrEmpty(backuppath.Trim) And Not backuppath Is Nothing Then
                fname2 = Now.Year & Now.Month & Now.Day & Now.Hour & Now.Minute & ".pdf"
                '  Dim path As String = "rajat/"
                att = New Attachment(backuppath)
            End If

            Dim Email As New System.Net.Mail.MailMessage("MYNDSAAS<no-reply@myndsol.com>", Mto, MSubject, MBody)
            Dim mailClient As New System.Net.Mail.SmtpClient()
            Email.IsBodyHtml = True
            If cc <> "" Then
                Email.CC.Add(cc)
                'Email.Attachments.Add(att)
            End If
            If bcc <> "" Then
                Email.Bcc.Add(bcc)
                ' Email.Attachments.Add(att)
            End If
            If Not att Is Nothing Then
                Email.Attachments.Add(att)
            End If

            Dim basicAuthenticationInfo As New System.Net.NetworkCredential("no-reply@myndsol.com", "vc79aK123AJ&$kL0")
            mailClient.Host = "mail.myndsol.com"
            mailClient.UseDefaultCredentials = False
            mailClient.Credentials = basicAuthenticationInfo
            Try
                mailClient.Send(Email)
            Catch ex As Exception
                Exit Sub


            End Try
        Catch ex As Exception
            Exit Sub
        End Try
    End Sub
    '' bkup 10-may17 
    'Public Sub notificationMail(ByVal uID As Integer, ByVal eid As Integer, ByVal en As String, ByVal SUBEVENT As String)
    '    Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
    '    Dim con As New SqlConnection(conStr)
    '    Dim da As SqlDataAdapter = Nothing
    '    'try Catch Block Added by Ajeet Kumar :Date::22 May 2014
    '    Try
    '        If con.State <> ConnectionState.Open Then
    '            con.Open()
    '        End If
    '        Dim STR As String = ""
    '        Dim MAILTO As String = ""
    '        Dim MAILID As String = ""
    '        Dim subject As String = ""
    '        Dim MSG As String = ""
    '        Dim cc As String = ""
    '        Dim Bcc As String = ""
    '        Dim MainEvent As String = ""
    '        Dim fn As String = ""
    '        'Dim obj As New MailUtill(eid:=eid)
    '        'fill Product  
    '        da = New SqlDataAdapter("select T.msgBody,T.subject,T.MAILTO,T.CC,T.EventName,T.QRY from MMM_MST_TEMPLATE T where eventnAME='" & en & "' and subevent='" & SUBEVENT & "' AND EID=" & eid & " ", con)
    '        Dim ds As New DataSet
    '        da.Fill(ds, "TEMP")
    '        If ds.Tables("TEMP").Rows.Count > 0 Then
    '            MSG = HttpUtility.HtmlDecode(ds.Tables("TEMP").Rows(0).Item("msgbody").ToString())
    '            subject = ds.Tables("TEMP").Rows(0).Item("SUBJECT").ToString()
    '            MAILTO = ds.Tables("TEMP").Rows(0).Item("MAILTO").ToString()
    '            cc = ds.Tables("TEMP").Rows(0).Item("CC").ToString()
    '            'Bcc = ds.Tables("TEMP").Rows(0).Item("BCC").ToString()

    '            MainEvent = ds.Tables("TEMP").Rows(0).Item("EVENTNAME").ToString()
    '            STR = ds.Tables("TEMP").Rows(0).Item("QRY")
    '        Else
    '            da.SelectCommand.CommandText = "select T.msgBody,T.subject,T.MAILTO,T.CC,T.BCC,T.EventName,T.qry from MMM_MST_TEMPLATE T  WHERE T.EVENTNAME='" & en & "' AND T.SUBEVENT='" & SUBEVENT & "' AND EID=0 "
    '            da.Fill(ds, "TEMP")
    '            MSG = HttpUtility.HtmlDecode(ds.Tables("TEMP").Rows(0).Item("msgbody").ToString())
    '            subject = ds.Tables("TEMP").Rows(0).Item("SUBJECT").ToString()
    '            MAILTO = ds.Tables("TEMP").Rows(0).Item("MAILTO").ToString()
    '            cc = ds.Tables("TEMP").Rows(0).Item("CC").ToString()
    '            Bcc = ds.Tables("TEMP").Rows(0).Item("BCC").ToString()
    '            MainEvent = ds.Tables("TEMP").Rows(0).Item("EVENTNAME").ToString()
    '            STR = ds.Tables("TEMP").Rows(0).Item("qry")
    '        End If


    '        STR &= " WHERE UID=" & uID & ""
    '        da.SelectCommand.CommandText = STR
    '        da.Fill(ds, "qry")
    '        'da.SelectCommand.CommandText = "select DISPLAYNAME,FieldMapping,DBTABLENAME FROM MMM_MST_FIELDS where DOCUMENTTYPE='" & MainEvent & "' AND (EID=" & eid & " OR EID=0)"
    '        'da.Fill(ds, "Fields")
    '        For Each dc As DataColumn In ds.Tables("qry").Columns
    '            fn = "{" & dc.ColumnName.ToString() & "}"
    '            MSG = MSG.Replace(fn, ds.Tables("qry").Rows(0).Item(dc.ColumnName).ToString())
    '            subject = subject.Replace(fn, ds.Tables("qry").Rows(0).Item(dc.ColumnName).ToString())
    '            MAILTO = MAILTO.Replace(fn, ds.Tables("qry").Rows(0).Item(dc.ColumnName).ToString())
    '            cc = cc.Replace(fn, ds.Tables("qry").Rows(0).Item(dc.ColumnName).ToString())
    '        Next
    '        Dim mailevent As String = en & "-" & SUBEVENT
    '        da.SelectCommand.Parameters.Clear()
    '        da.SelectCommand.CommandText = "INSERT_MAILLOG"
    '        da.SelectCommand.CommandType = CommandType.StoredProcedure
    '        da.SelectCommand.Parameters.AddWithValue("@MAILTO", MAILTO)
    '        da.SelectCommand.Parameters.AddWithValue("@CC", cc)
    '        da.SelectCommand.Parameters.AddWithValue("@MSG", MSG)
    '        da.SelectCommand.Parameters.AddWithValue("@ALERTTYPE", "MAIL")
    '        da.SelectCommand.Parameters.AddWithValue("@MAILEVENT", mailevent)
    '        da.SelectCommand.Parameters.AddWithValue("@EID", eid)
    '        Dim RES As Integer = da.SelectCommand.ExecuteScalar()
    '        If RES > 0 Then
    '            'obj.SendMail(ToMail:=MAILTO, Subject:=subject, MailBody:=MSG, CC:=cc, BCC:=Bcc)
    '            'sendMail(ToMail:=MAILTO, subject:=subject, MailBody:=MSG, cc:=cc, Bcc:=Bcc)
    '            sendMail1(MAILTO, cc, Bcc, subject, MSG, en, eid)
    '        End If
    '    Catch ex As Exception
    '    Finally
    '        If Not da Is Nothing Then
    '            da.Dispose()
    '        End If
    '        If Not con Is Nothing Then
    '            con.Dispose()
    '        End If
    '    End Try
    'End Sub

    '' new for password reset link security testing observation by mayank


    Public Sub notificationMail(ByVal uID As Integer, ByVal eid As Integer, ByVal en As String, ByVal SUBEVENT As String)
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim da As SqlDataAdapter = Nothing
        'try Catch Block Added by Ajeet Kumar :Date::22 May 2014
        Try
            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            Dim STR As String = ""
            Dim MAILTO As String = ""
            Dim MAILID As String = ""
            Dim subject As String = ""
            Dim MSG As String = ""
            Dim cc As String = ""
            Dim Bcc As String = ""
            Dim MainEvent As String = ""
            Dim fn As String = ""
            Dim obj As New MailUtill(eid:=eid)
            'fill Product  
            'Reset Password functionality user can reset only once if he clicked over there 
            If SUBEVENT.ToUpper.Trim = "FORGET PASSWORD" Or SUBEVENT.ToUpper.Trim = "USER CREATED" Then
                Dim objDC As New DataClass()
                objDC.ExecuteNonQuery("Update mmm_mst_user set ResetAccessString=replace(NEWID(),'-',''), ResetFlag=1 where uid=" & uID)
            End If
            'Reset Password functionality user can reset only once if he clicked over there 

            da = New SqlDataAdapter("select T.msgBody,T.subject,T.MAILTO,T.CC,T.BCC,T.EventName,T.QRY from MMM_MST_TEMPLATE T where eventnAME='" & en & "' and subevent='" & SUBEVENT & "' AND EID=" & eid & " ", con)
            Dim ds As New DataSet
            da.Fill(ds, "TEMP")
            If ds.Tables("TEMP").Rows.Count > 0 Then
                MSG = HttpUtility.HtmlDecode(ds.Tables("TEMP").Rows(0).Item("msgbody").ToString())
                subject = ds.Tables("TEMP").Rows(0).Item("SUBJECT").ToString()
                MAILTO = ds.Tables("TEMP").Rows(0).Item("MAILTO").ToString()
                cc = ds.Tables("TEMP").Rows(0).Item("CC").ToString()
                Bcc = ds.Tables("TEMP").Rows(0).Item("BCC").ToString()

                MainEvent = ds.Tables("TEMP").Rows(0).Item("EVENTNAME").ToString()
                STR = ds.Tables("TEMP").Rows(0).Item("QRY")
            Else
                da.SelectCommand.CommandText = "select T.msgBody,T.subject,T.MAILTO,T.CC,T.EventName,T.qry from MMM_MST_TEMPLATE T  WHERE T.EVENTNAME='" & en & "' AND T.SUBEVENT='" & SUBEVENT & "' AND EID=0 "
                da.Fill(ds, "TEMP")
                MSG = HttpUtility.HtmlDecode(ds.Tables("TEMP").Rows(0).Item("msgbody").ToString())
                subject = ds.Tables("TEMP").Rows(0).Item("SUBJECT").ToString()
                MAILTO = ds.Tables("TEMP").Rows(0).Item("MAILTO").ToString()
                cc = ds.Tables("TEMP").Rows(0).Item("CC").ToString()
                '  Bcc = ds.Tables("TEMP").Rows(0).Item("BCC").ToString()
                MainEvent = ds.Tables("TEMP").Rows(0).Item("EVENTNAME").ToString()
                STR = ds.Tables("TEMP").Rows(0).Item("qry")
            End If


            STR &= " WHERE UID=" & uID & ""


            da.SelectCommand.CommandText = STR
            da.Fill(ds, "qry")
            'da.SelectCommand.CommandText = "select DISPLAYNAME,FieldMapping,DBTABLENAME FROM MMM_MST_FIELDS where DOCUMENTTYPE='" & MainEvent & "' AND (EID=" & eid & " OR EID=0)"
            'da.Fill(ds, "Fields")
            For Each dc As DataColumn In ds.Tables("qry").Columns
                fn = "{" & dc.ColumnName.ToString() & "}"
                MSG = MSG.Replace(fn, ds.Tables("qry").Rows(0).Item(dc.ColumnName).ToString())
                subject = subject.Replace(fn, ds.Tables("qry").Rows(0).Item(dc.ColumnName).ToString())
                MAILTO = MAILTO.Replace(fn, ds.Tables("qry").Rows(0).Item(dc.ColumnName).ToString())
                cc = cc.Replace(fn, ds.Tables("qry").Rows(0).Item(dc.ColumnName).ToString())
                'Changes for Password Reset Functionality
            Next
            Dim mailevent As String = en & "-" & SUBEVENT
            da.SelectCommand.Parameters.Clear()
            da.SelectCommand.CommandText = "INSERT_MAILLOG"
            da.SelectCommand.CommandType = CommandType.StoredProcedure
            da.SelectCommand.Parameters.AddWithValue("@MAILTO", MAILTO)
            da.SelectCommand.Parameters.AddWithValue("@CC", cc)
            da.SelectCommand.Parameters.AddWithValue("@MSG", MSG)
            da.SelectCommand.Parameters.AddWithValue("@ALERTTYPE", "MAIL")
            da.SelectCommand.Parameters.AddWithValue("@MAILEVENT", mailevent)
            da.SelectCommand.Parameters.AddWithValue("@EID", eid)
            Dim RES As Integer = da.SelectCommand.ExecuteScalar()
            If RES > 0 Then
                ' obj.SendMail1(ToMail:=MAILTO, Subject:=subject, MailBody:=MSG, CC:=cc, BCC:=Bcc, DocType:=en, EID:=eid)
                obj.SendMail(ToMail:=MAILTO, Subject:=subject, MailBody:=MSG, CC:=cc, BCC:=Bcc)
                'sendMail(ToMail:=MAILTO, subject:=subject, MailBody:=MSG, cc:=cc, Bcc:=Bcc)
                '' disabled by sunil for protocol upgradation to TLS - 587 Port  - 17-Jan-19
                ' sendMail1(MAILTO, cc, Bcc, subject, MSG, en, eid)
            End If
        Catch ex As Exception
        Finally
            If Not da Is Nothing Then
                da.Dispose()
            End If
            If Not con Is Nothing Then
                con.Dispose()
            End If
        End Try
    End Sub


    Public Sub notificationMailT(ByVal uID As Integer, ByVal eid As Integer, ByVal en As String, ByVal SUBEVENT As String, con As SqlConnection, trans As SqlTransaction)
        'Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        'Dim con As New SqlConnection(conStr)

        'try Catch Block Added by Ajeet Kumar :Date::22 May 2014
        Try
            Dim STR As String = ""
            Dim MAILTO As String = ""
            Dim MAILID As String = ""
            Dim subject As String = ""
            Dim MSG As String = ""
            Dim cc As String = ""
            Dim Bcc As String = ""
            Dim MainEvent As String = ""
            Dim fn As String = ""
            Dim obj As New MailUtill(eid:=eid)
            'fill Product  

            'Reset Password functionality user can reset only once if he clicked over there  by sunil on 22_June
            ''Reset Password functionality user can reset only once if he clicked over there 
            'If SUBEVENT.ToUpper.Trim = "FORGET PASSWORD" Or SUBEVENT.ToUpper.Trim = "USER CREATED" Then
            '    Dim objDC As New DataClass()
            '    objDC.ExecuteNonQuery("Update mmm_mst_user set ResetAccessString=replace(NEWID(),'-',''), ResetFlag=1 where uid=" & uID)
            'End If
            ''Reset Password functionality user can reset only once if he clicked over there 

            Dim da As SqlDataAdapter = New SqlDataAdapter("select T.msgBody,T.subject,T.MAILTO,T.CC,T.EventName,T.QRY from MMM_MST_TEMPLATE T where eventnAME='" & en & "' and subevent='" & SUBEVENT & "' AND EID=" & eid & " ", con)
            da.SelectCommand.Transaction = trans
            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            Dim ds As New DataSet
            da.Fill(ds, "TEMP")
            If ds.Tables("TEMP").Rows.Count > 0 Then
                MSG = HttpUtility.HtmlDecode(ds.Tables("TEMP").Rows(0).Item("msgbody").ToString())
                subject = ds.Tables("TEMP").Rows(0).Item("SUBJECT").ToString()
                MAILTO = ds.Tables("TEMP").Rows(0).Item("MAILTO").ToString()
                cc = ds.Tables("TEMP").Rows(0).Item("CC").ToString()
                'Bcc = ds.Tables("TEMP").Rows(0).Item("BCC").ToString()

                MainEvent = ds.Tables("TEMP").Rows(0).Item("EVENTNAME").ToString()
                STR = ds.Tables("TEMP").Rows(0).Item("QRY")
            Else
                da.SelectCommand.CommandText = "select T.msgBody,T.subject,T.MAILTO,T.CC,T.BCC,T.EventName,T.qry from MMM_MST_TEMPLATE T  WHERE T.EVENTNAME='" & en & "' AND T.SUBEVENT='" & SUBEVENT & "' AND EID=0 "
                da.Fill(ds, "TEMP")
                MSG = HttpUtility.HtmlDecode(ds.Tables("TEMP").Rows(0).Item("msgbody").ToString())
                subject = ds.Tables("TEMP").Rows(0).Item("SUBJECT").ToString()
                MAILTO = ds.Tables("TEMP").Rows(0).Item("MAILTO").ToString()
                cc = ds.Tables("TEMP").Rows(0).Item("CC").ToString()
                Bcc = ds.Tables("TEMP").Rows(0).Item("BCC").ToString()
                MainEvent = ds.Tables("TEMP").Rows(0).Item("EVENTNAME").ToString()
                STR = ds.Tables("TEMP").Rows(0).Item("qry")
            End If


            STR &= " WHERE UID=" & uID & ""
            da.SelectCommand.CommandText = STR
            da.Fill(ds, "qry")
            'da.SelectCommand.CommandText = "select DISPLAYNAME,FieldMapping,DBTABLENAME FROM MMM_MST_FIELDS where DOCUMENTTYPE='" & MainEvent & "' AND (EID=" & eid & " OR EID=0)"
            'da.Fill(ds, "Fields")
            For Each dc As DataColumn In ds.Tables("qry").Columns
                fn = "{" & dc.ColumnName.ToString() & "}"
                MSG = MSG.Replace(fn, ds.Tables("qry").Rows(0).Item(dc.ColumnName).ToString())
                subject = subject.Replace(fn, ds.Tables("qry").Rows(0).Item(dc.ColumnName).ToString())
                MAILTO = MAILTO.Replace(fn, ds.Tables("qry").Rows(0).Item(dc.ColumnName).ToString())
                cc = cc.Replace(fn, ds.Tables("qry").Rows(0).Item(dc.ColumnName).ToString())
            Next
            Dim mailevent As String = en & "-" & SUBEVENT
            da.SelectCommand.Parameters.Clear()
            da.SelectCommand.CommandText = "INSERT_MAILLOG"
            da.SelectCommand.CommandType = CommandType.StoredProcedure
            da.SelectCommand.Parameters.AddWithValue("@MAILTO", MAILTO)
            da.SelectCommand.Parameters.AddWithValue("@CC", cc)
            da.SelectCommand.Parameters.AddWithValue("@MSG", MSG)
            da.SelectCommand.Parameters.AddWithValue("@ALERTTYPE", "MAIL")
            da.SelectCommand.Parameters.AddWithValue("@MAILEVENT", mailevent)
            da.SelectCommand.Parameters.AddWithValue("@EID", eid)
            Dim RES As Integer = da.SelectCommand.ExecuteScalar()
            If RES > 0 Then
                ' obj.SendMail1(ToMail:=MAILTO, Subject:=subject, MailBody:=MSG, CC:=cc, BCC:=Bcc, DocType:=en, EID:=eid)
                obj.SendMail(ToMail:=MAILTO, Subject:=subject, MailBody:=MSG, CC:=cc, BCC:=Bcc)
                'sendMail(ToMail:=MAILTO, subject:=subject, MailBody:=MSG, cc:=cc, Bcc:=Bcc)
                '' disabled by sunil for protocol upgradation to TLS - 587 Port  - 17-Jan-19
                ' sendMail1(MAILTO, cc, Bcc, subject, MSG, en, eid)
            End If
        Catch ex As Exception
        Finally
            'If Not da Is Nothing Then
            '    da.Dispose()
            'End If
            If Not con Is Nothing Then
                'con.Dispose()
            End If
        End Try
    End Sub



    Protected Function GetCommanValues(ByVal Sele As String, ByVal Curr As String) As String
        If Right(Sele, 1) = "," Then Sele = Left(Sele, Len(Sele) - 1)
        If Right(Curr, 1) = "," Then Curr = Left(Curr, Len(Curr) - 1)


        Dim ArrSele() As String = Split(Sele, ",")
        Dim ArrCurr() As String = Split(Curr, ",")
        Dim ReturnStr As String = ""
        Dim foundinCurr As Boolean = False
        For i As Integer = 0 To ArrSele.Length - 1
            foundinCurr = False
            For j As Integer = 0 To ArrCurr.Length - 1
                If ArrSele(i) = ArrCurr(j) Then
                    foundinCurr = True
                    Exit For
                End If
            Next
            If foundinCurr = True Then
                ReturnStr = ReturnStr & ArrSele(i) & ","
            End If
        Next
        If Right(ReturnStr, 1) = "," Then
            ReturnStr = Left(ReturnStr, Len(ReturnStr) - 1)
        End If
        Return ReturnStr
    End Function

    Public Function GetString(objDt As DataTable, ByRef str As StringBuilder) As String
        Dim inputs() As String = {"Payment_Date_of_Buyer", "Underlying_Commodity_Description", "Buyer_Name", "PV_Sending_date", "Invoices", "Invoice_Doc_No", "PV_Unique_Ref_No", "Invoice_NO", "Invoice_Amount", "Approved_Amount", "GRN_SRN_Number", "GRN_SRN_Date", "Invoice_Date", "Supplier_Name", "Payment_Due_Date_for_supplier", "Supplier_Code", "PV_Factoring_Unit_No", "Underlying_Commodity_Type"}
        Dim lstOfString As List(Of String) = New List(Of String)(inputs)
        Dim arbuilder As New ArrayList()
        Dim chbuilder As New ArrayList()
        Dim invoice As String = ""
        Dim IsInvoiceInvoke As Boolean = False
        For Each strdata As String In lstOfString
            If (strdata.ToString() = "Invoices" Or strdata.ToString() = "Invoice_Doc_No" Or strdata.ToString() = "PV_Unique_Ref_No" Or strdata.ToString() = "Invoice_NO" Or strdata.ToString() = "Invoice_Amount" Or strdata.ToString() = "GRN_SRN_Number" Or strdata.ToString() = "GRN_SRN_Date" Or strdata.ToString() = "Invoice_Date" Or strdata.ToString() = "Approved_Amount") Then
                If strdata.ToString() = "Invoices" Then
                    invoice = """Invoices""" & ":" & "[{"
                    IsInvoiceInvoke = False
                ElseIf strdata.ToString() = "Invoice_Date" Then
                    If (Not objDt.Columns.Contains(strdata)) Then
                        chbuilder.Add("""" & strdata.ToString() & "" & ":" & """""")
                    Else
                        If Convert.ToString(objDt.Rows(0)(strdata)) <> "" Then
                            Dim strDate As String() = Convert.ToString(objDt.Rows(0)(strdata)).Split("/")
                            chbuilder.Add("""" & strdata.ToString() & """" & ":" & """" & strDate(0) & "/" & strDate(1) & "/20" & strDate(2) & """")
                        Else
                            chbuilder.Add("""" & strdata.ToString() & """" & ":" & """" & Convert.ToString(objDt.Rows(0)(strdata)) & """")
                        End If

                    End If
                    IsInvoiceInvoke = True
                    invoice &= String.Join(",", chbuilder.ToArray()) & " }]"
                Else
                    If (Not objDt.Columns.Contains(strdata)) Then
                        chbuilder.Add("""" & strdata.ToString() & ("""" & ":" & """"""))
                    Else
                        If strdata.ToString().Contains("_Date") And Convert.ToString(objDt.Rows(0)(strdata)) <> "" Then
                            Dim strDate As String() = Convert.ToString(objDt.Rows(0)(strdata)).Split("/")
                            chbuilder.Add("""" & strdata.ToString() & """" & ":" & """" & strDate(0) & "/" & strDate(1) & "/20" & strDate(2) & """")
                        Else
                            chbuilder.Add("""" & strdata.ToString() & """" & ":" & """" & Convert.ToString(objDt.Rows(0)(strdata)) & """")
                        End If


                    End If
                    IsInvoiceInvoke = False
                End If
            Else
                If IsInvoiceInvoke = True Then
                    arbuilder.Add(invoice.ToString())
                    IsInvoiceInvoke = False
                End If
                If (Not objDt.Columns.Contains(strdata)) Then
                    arbuilder.Add("""" & strdata.ToString() & """" & ":" & """""")
                Else
                    If strdata.ToString().Contains("_Date") Or strdata.ToString().Contains("_date") And Convert.ToString(objDt.Rows(0)(strdata)) <> "" Then
                        Dim strDate As String() = Convert.ToString(objDt.Rows(0)(strdata)).Split("/")
                        arbuilder.Add("""" & strdata.ToString() & """" & ":" & """" & strDate(0) & "/" & strDate(1) & "/20" & strDate(2) & """")
                    Else
                        arbuilder.Add("""" & strdata.ToString() & """" & ":" & """" & Convert.ToString(objDt.Rows(0)(strdata)) & """")
                    End If
                End If
            End If
        Next
        str.Append(String.Join(",", arbuilder.ToArray()))
        str.Append("}")
        Return str.ToString()
    End Function

    Public Function WSPost(Str As String) As String

        Dim ret As String = ""

        Try
            'url = "http://www.myndsaas.com/MyndBPMWS.svc/SaveData";


            Dim url As String = "http://103.25.172.132:8092/PVService/createReq"

            ' declare ascii encoding
            Dim encoding As New ASCIIEncoding()

            Dim strResult As String = String.Empty

            ' sample xml sent to Service & this data is sent in POST
            Dim postData As String = Str

            ' convert xmlstring to byte using ascii encoding
            Dim data As Byte() = encoding.GetBytes(postData)
            ' declare httpwebrequet wrt url defined above

            Dim webrequest__1 As HttpWebRequest = DirectCast(WebRequest.Create(url), HttpWebRequest)
            ' set method as post
            webrequest__1.Method = "POST"
            ' set content type
            webrequest__1.ContentType = "application/x-www-form-urlencoded"
            ' set content length
            webrequest__1.ContentLength = data.Length
            ' get stream data out of webrequest object
            Dim newStream As Stream = webrequest__1.GetRequestStream()
            newStream.Write(data, 0, data.Length)
            newStream.Close()
            ' declare & read response from service
            Dim webresponse As HttpWebResponse = DirectCast(webrequest__1.GetResponse(), HttpWebResponse)

            ' set utf8 encoding
            Dim enc As Encoding = System.Text.Encoding.GetEncoding("utf-8")
            ' read response stream from response object
            Dim loResponseStream As New StreamReader(webresponse.GetResponseStream(), enc)
            ' read string from stream data
            strResult = loResponseStream.ReadToEnd()
            ' close the stream object
            loResponseStream.Close()
            ' close the response object
            webresponse.Close()
            ' below steps remove unwanted data from response string
            ret = strResult
        Catch ex As Exception
            Return ex.ToString()
        End Try
        Return ret
    End Function



    Public Function SUActivityLog(Optional ByVal EID As Integer = 0, Optional ByVal CreatedBy As Integer = 0, Optional ByVal actionTakenScreen As String = "USER", Optional ByVal actionRemarks As String = "", Optional ByVal TargetUID As Integer = 0) As String
        Dim ret As String = String.Empty
        Dim objDC As New DataClass()
        Dim ht As New Hashtable()
        ht.Add("@EID", EID)
        ht.Add("@createby", CreatedBy)
        ht.Add("@actiontakenscreen", actionTakenScreen)
        ht.Add("@actionRemarks", actionRemarks)
        ht.Add("@TargetUID", TargetUID)
        objDC.ExecuteProDT("sp_mmm_suactivity_log", ht)
        Return ret
    End Function

    Public Function WSPost2(Str As String) As String

        Dim ret As String = ""

        Try
            'url = "http://www.myndsaas.com/MyndBPMWS.svc/SaveData";


            Dim url As String = "http://103.25.172.132:8092/PVService/createReq"

            ' declare ascii encoding
            Dim encoding As New ASCIIEncoding()

            Dim strResult As String = String.Empty

            ' sample xml sent to Service & this data is sent in POST
            Dim postData As String = Str

            ' convert xmlstring to byte using ascii encoding
            Dim data As Byte() = encoding.GetBytes(postData)
            ' declare httpwebrequet wrt url defined above

            Dim webrequest__1 As HttpWebRequest = DirectCast(WebRequest.Create(url), HttpWebRequest)
            ' set method as post
            webrequest__1.Method = "POST"
            ' set content type
            webrequest__1.ContentType = "application/x-www-form-urlencoded"
            ' set content length
            webrequest__1.ContentLength = data.Length
            ' get stream data out of webrequest object
            Dim newStream As Stream = webrequest__1.GetRequestStream()
            newStream.Write(data, 0, data.Length)
            newStream.Close()
            ' declare & read response from service
            Dim webresponse As HttpWebResponse = DirectCast(webrequest__1.GetResponse(), HttpWebResponse)

            ' set utf8 encoding
            Dim enc As Encoding = System.Text.Encoding.GetEncoding("utf-8")
            ' read response stream from response object
            Dim loResponseStream As New StreamReader(webresponse.GetResponseStream(), enc)
            ' read string from stream data
            strResult = loResponseStream.ReadToEnd()
            ' close the stream object
            loResponseStream.Close()
            ' close the response object
            webresponse.Close()
            ' below steps remove unwanted data from response string
            ret = strResult
        Catch ex As Exception
            Return ex.ToString()
        End Try
        Return ret
    End Function


End Class
