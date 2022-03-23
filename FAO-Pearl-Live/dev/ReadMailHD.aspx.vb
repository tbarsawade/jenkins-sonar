Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports System.Globalization
Imports AjaxControlToolkit
Imports System.Net.HttpWebResponse
Imports System
Imports System.Collections.Specialized
Imports System.Net
Imports System.Text
Imports System.Net.Security
Imports System.Security.Cryptography.X509Certificates
Imports System.Delegate

Imports OpenPop.Pop3
Imports OpenPop.Mime
'Imports System.Data

Partial Class ReadMailHD
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            If Not Page.IsPostBack Then
            End If
        Catch ex As Exception
        End Try
    End Sub
    'Add Theme Code
    Protected Sub Page_PreInit1(ByVal sender As Object, ByVal e As EventArgs) Handles Me.PreInit
        Try
            If Not Session("CTheme") Is Nothing And Not Session("CTheme") = String.Empty Then
                Page.Theme = Convert.ToString(Session("CTheme"))
            Else
                Page.Theme = "Default"
            End If
        Catch ex As Exception
        End Try
    End Sub
    Public Sub CreateDocumentEntityWise()
        Dim conStrs As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim cons As New SqlConnection(conStrs)
        Dim das As SqlDataAdapter = New SqlDataAdapter("", cons)
        das.SelectCommand.CommandText = "select * from mmm_hdmail_schdule where isactive=1 and mdmailid is not null  and mdpwd is not null and mdport is not null and mdisssl is not null and hostname is not null"
        Dim dt As New DataTable
        das.Fill(dt)
        If dt.Rows.Count > 0 Then
            For Each dr As DataRow In dt.Rows
                If Not (String.IsNullOrEmpty(dr("mdmailid")) And String.IsNullOrEmpty(dr("mdpwd")) And String.IsNullOrEmpty(dr("mdport")) And String.IsNullOrEmpty(dr("mdisssl")) And String.IsNullOrEmpty(dr("hostname"))) Then
                    Dim pop3Client As Pop3Client
                    pop3Client = New Pop3Client
                    pop3Client.Connect(Convert.ToString(dr("hostname")), Convert.ToInt32(dr("mdport")), True)
                    pop3Client.Authenticate(Convert.ToString(dr("mdmailid")), Convert.ToString(dr("mdpwd")))
                    Dim count As Integer = pop3Client.GetMessageCount
                    Dim counter As Integer = 0
                    Dim i As Integer = count
                    Dim Emails As New List(Of Email)
                    Do While (i >= 1)
                        Dim message As Message = pop3Client.GetMessage(i)
                        'Emails = New List(Of Email)
                        Dim email As New Email()
                        email.EID = Convert.ToInt32(dr("EID"))
                        email.DocumentType = Convert.ToString(dr("DocumentType"))
                        HttpContext.Current.Session("FORMNAME") = Convert.ToString(dr("DocumentType"))
                        email.MessageNumber = i
                        email.MessageID = message.Headers.MessageId
                        email.Subject = message.Headers.Subject
                        email.DateSent = message.Headers.DateSent
                        email.From = message.Headers.From.Address
                        HttpContext.Current.Session("FROM") = message.Headers.From.Address
                        email.IsAllowCreateUser = dr("IsAllowCreateUser")
                        If String.IsNullOrEmpty(message.Headers.From.DisplayName) Then
                            Dim fromParsar As String() = message.Headers.From.Address.Split(".")
                            email.DisplayName = fromParsar(0)
                        Else
                            email.DisplayName = message.Headers.From.DisplayName
                        End If
                        Dim body As MessagePart = message.FindFirstHtmlVersion()
                        If Not IsNothing(body) Then
                            email.Body = body.GetBodyAsText()
                        Else
                            body = message.FindFirstPlainTextVersion()
                            If Not IsNothing(body) Then
                                email.Body = body.GetBodyAsText()
                            End If
                        End If

                        Dim attachments As List(Of MessagePart) = message.FindAllAttachments()

                        For Each attachment As MessagePart In attachments
                            Dim attachmentData As New Attachment()
                            email.Attachments.Add(New Attachment With {.FileName = attachment.FileName, .ContentType = attachment.ContentType.MediaType, .Content = attachment.Body})
                        Next
                        Emails.Add(email)
                        counter = counter + 1
                        i = i - 1
                        'If counter = 5 Then
                        '    Exit Do
                        'End If
                    Loop
                    DocumentCreationInBPM(Emails, Convert.ToInt32(dr("EID")), Convert.ToString(dr("DocumentType")))
                End If
            Next
        End If
    End Sub

    Public Sub DocumentCreationInBPM(listOfEmails As List(Of Email), EID As Integer, Documenttype As String)
        If listOfEmails.Count > 0 Then
            Dim conStrs As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim cons As New SqlConnection(conStrs)
            Dim das As SqlDataAdapter = New SqlDataAdapter("", cons)
            das.SelectCommand.CommandText = "select * from mmm_mst_fields where documenttype='" & Documenttype & "' and eid=" & EID & " and (MDFieldName is not null or ParseFromEmail=1)"
            Dim dt As New DataTable
            das.Fill(dt)
            If dt.Rows.Count > 0 Then
                For Each emails As Email In listOfEmails
                    Dim ArrayColumn As New ArrayList()
                    Dim ArrayColumnValue As New ArrayList()
                    For Each dr As DataRow In dt.Rows
                        Select Case dr("MDfieldName").ToString().ToUpper
                            Case "EMAILID"
                                ArrayColumn.Add(dr("FieldMapping"))
                                ArrayColumnValue.Add("'" & emails.From & "'")
                            Case "SUBJECT"
                                ArrayColumn.Add(dr("FieldMapping"))
                                ArrayColumnValue.Add("'" & emails.Subject & "'")
                            Case "BODY"
                                ArrayColumn.Add(dr("FieldMapping"))
                                ArrayColumnValue.Add("'" & emails.Body.ToString().Replace("'", "''") & "'")
                            Case "MESSAGEID"
                                ArrayColumn.Add(dr("FieldMapping"))
                                ArrayColumnValue.Add("'" & emails.MessageID & "'")
                            Case "STATUS"
                                Dim Result As ArrayList
                                Result = IsUserExist(emails.From, emails.IsAllowCreateUser, EID, emails.DisplayName)
                                ArrayColumn.Add("oUID")
                                ArrayColumnValue.Add(Result(0))
                                Session("UID") = Result(0)
                                ArrayColumn.Add(dr("FieldMapping"))
                                ArrayColumnValue.Add(Result(1))
                            Case "ATTACHMENT"
                                ArrayColumn.Add(dr("FieldMapping"))
                                Dim attachments As New ArrayList()
                                For Each att As Attachment In emails.Attachments
                                    Dim img As Byte() = att.Content
                                    Dim ext As String = att.FileName.Substring(att.FileName.ToString().LastIndexOf("."), att.FileName.ToString().Length - att.FileName.ToString().LastIndexOf("."))
                                    Dim Path As String = Server.MapPath("DOCS/") & EID & "/" & emails.MessageID & "/" & getSafeString(Documenttype) & "_" & att.FileName & "_" & Now.Day & Now.Month & Now.Year & Now.Minute & Now.Second & Now.Millisecond & dr.Item("FieldID").ToString() & ext
                                    If Not Directory.Exists(Server.MapPath("DOCS/") & EID & "/" & emails.MessageID) Then
                                        Directory.CreateDirectory(Server.MapPath("DOCS/") & EID & "/" & emails.MessageID)
                                    End If
                                    File.WriteAllBytes(Path, img)
                                    attachments.Add(emails.MessageID & "/" & getSafeString(Documenttype) & "_" & att.FileName & "_" & Now.Day & Now.Month & Now.Year & Now.Minute & Now.Second & Now.Millisecond & dr.Item("FieldID").ToString() & ext)
                                Next
                                ArrayColumnValue.Add("'" & String.Join(",", attachments.ToArray()) & "'")
                            Case Else
                                ArrayColumn.Add(dr("FieldMapping"))
                                ArrayColumnValue.Add(GetMasterData(dr("dropdown"), EID, emails.Body))
                        End Select
                    Next
                    ArrayColumn.Add("EID")
                    ArrayColumnValue.Add(EID)
                    ArrayColumn.Add("Documenttype")
                    ArrayColumnValue.Add("'" & Documenttype & "'")
                    ArrayColumn.Add("adate")
                    ArrayColumnValue.Add("getdate()")
                    SavingMailDoc(ArrayColumn, ArrayColumnValue)
                Next
            End If
        End If
    End Sub
    Public Function IsUserExist(From As String, IsAllowUserCreate As Boolean, EID As Integer, DisplayName As String) As ArrayList
        Dim Result As New ArrayList()
        Dim conStrs As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim cons As New SqlConnection(conStrs)
        Dim das As SqlDataAdapter = New SqlDataAdapter("", cons)
        das.SelectCommand.CommandText = "select uid from mmm_mst_user where eid=" & EID & " and emailid='" & From & "'"
        Dim dt As New DataTable
        das.Fill(dt)
        If dt.Rows.Count > 0 Then
            Result.Add(dt.Rows(0)(0))
            Result.Add("'NEW'")
        ElseIf IsAllowUserCreate = True Then
            das.SelectCommand.CommandText = "insert into mmm_mst_user (username,userid,emailid,userrole,isauth,eid,passtry,modifydate) values ('" & DisplayName & "','" & From & "','" & From & "','END USER',100," & EID & ",0,getdate());select scope_identity()"
            If cons.State = ConnectionState.Closed Then
                cons.Open()
            End If
            Result.Add(das.SelectCommand.ExecuteScalar())
            Result.Add("'NEW'")
        Else
            das.SelectCommand.CommandText = "select uid from mmm_mst_user where eid=" & EID & " and userRole='SU'"
            If cons.State = ConnectionState.Closed Then
                cons.Open()
            End If
            Result.Add(Convert.ToInt32(das.SelectCommand.ExecuteScalar()))
            Result.Add("'SUSPENDED'")
        End If
        Return Result
    End Function
    Public Sub SavingMailDoc(arrayColumn As ArrayList, arrayColumnvalue As ArrayList)
        If arrayColumn.Count > 0 And arrayColumnvalue.Count > 0 Then
            Dim strQuery As String = String.Empty
            Dim ob As New DynamicForm
            Dim conStrs As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim cons As New SqlConnection(conStrs)
            cons.Open()
            Dim tran As SqlTransaction = Nothing
            tran = cons.BeginTransaction()
            Dim das As SqlDataAdapter = New SqlDataAdapter("", cons)
            das.SelectCommand.CommandText = "insert into mmm_mst_doc (" & String.Join(",", arrayColumn.ToArray()) & ") values (" & String.Join(",", arrayColumnvalue.ToArray()) & ");select scope_identity()"
            das.SelectCommand.Transaction = tran
            If cons.State = ConnectionState.Closed Then
                cons.Open()
            End If
            Dim tid As Integer = das.SelectCommand.ExecuteScalar()
            das.SelectCommand.CommandText = "InsertDefaultMovement"
            das.SelectCommand.CommandType = CommandType.StoredProcedure
            das.SelectCommand.Parameters.Clear()
            das.SelectCommand.Parameters.AddWithValue("tid", tid)
            das.SelectCommand.Parameters.AddWithValue("CUID", Val(Session("UID").ToString()))
            das.SelectCommand.Parameters.AddWithValue("what", "UPLOADED")
            das.SelectCommand.ExecuteNonQuery()
            Dim res As String
            Try
                res = GetNextUserFromRolematrixT(tid, HttpContext.Current.Session("EID"), HttpContext.Current.Session("UID"), "", HttpContext.Current.Session("UID"), cons, tran)
                If String.IsNullOrEmpty(res) Then
                    res = GetNextUserFromOrganizatios(cons, tran)
                End If
                AssignTicketToUserBasedOnCondition(res, tid, "", HttpContext.Current.Session("UID"), cons, tran)
                ob.HistoryT(Session("EID"), tid, HttpContext.Current.Session("UID"), HttpContext.Current.Session("FORMNAME"), "MMM_MST_DOC", "ADD", cons, tran)
                Trigger.ExecuteTriggerT(HttpContext.Current.Session("FORMNAME"), HttpContext.Current.Session("EID"), tid, cons, tran)
            Catch ex As Exception
                tran.Rollback()
            End Try
           
        End If
    End Sub
    Public Function GetNextUserFromOrganizatios(ByVal con As SqlConnection, ByVal tran As SqlTransaction) As String
        Dim result As String = ""
        Dim From As String = HttpContext.Current.Session("FROM")
        Dim Name As String = String.Empty
        Dim Domains As String = String.Empty
        Dim Group As String = String.Empty
        Dim Agents As String = String.Empty
        Dim Domain As String() = From.Split("@")
        Dim das As SqlDataAdapter = New SqlDataAdapter("", con)
        das.SelectCommand.CommandText = "select MDfieldName,fieldMapping from mmm_mst_fields where documenttype in('Organizations','Groups') and eid=" & HttpContext.Current.Session("EID") & " and MDfieldName is not null"
        das.SelectCommand.Transaction = tran
        Dim dtOrganization As New DataTable()
        das.Fill(dtOrganization)
        If dtOrganization.Rows.Count > 0 Then
            For Each dr As DataRow In dtOrganization.Rows
                Select Case dr("MDfieldName")
                    Case "Name"
                        Name = dr("fieldMapping")
                    Case "Group"
                        Group = dr("fieldMapping")
                    Case "Domains"
                        Domains = dr("fieldMapping")
                    Case "Agents"
                        Agents = dr("fieldMapping")
                End Select
            Next
            'Name = dtOrganization.Rows(0)("Name")
            'Group = dtOrganization.Rows(0)("Group")
            'Domains = dtOrganization.Rows(0)("Domains")
            'Agents = dtOrganization.Rows(0)("Agents")
            If Not (String.IsNullOrEmpty(Name) And String.IsNullOrEmpty(Group) And String.IsNullOrEmpty(Domains) And String.IsNullOrEmpty(Agents)) Then
                das.SelectCommand.CommandText = "select " & Agents & " from mmm_mst_master where eid=" & HttpContext.Current.Session("EID") & " and documenttype='Groups' and tid in (select " & Group & " from mmm_mst_master where eid=" & HttpContext.Current.Session("EID") & " and documenttype='Organizations' and  ','+" & Domains & "+',' like '%," & Domain(1).Trim() & ",%' )"
                If con.State = ConnectionState.Closed Then
                    con.Open()
                End If
                result = das.SelectCommand.ExecuteScalar()
            Else
                result = "Mapping Not Found against Name,Group,Domain,Agents"
            End If
            Return result
        End If
    End Function

    Public Function AssignTicketToUserBasedOnCondition(result As String, docID As Integer, ByVal qry As String, ByVal Auid As Integer, ByVal con As SqlConnection, ByVal tran As SqlTransaction) As String
        Dim Res As String = ""
        Dim uids As String() = result.ToString().Split(",")
        Dim das As SqlDataAdapter = New SqlDataAdapter("", con)
        das.SelectCommand.Transaction = tran
        Try
            If uids.Length > 1 Then
                Dim arrColumnValues As New ArrayList
                For i As Integer = 0 To uids.Length - 1
                    arrColumnValues.Add("(" & docID & "," & uids(i) & ",getdate(),null,null)")
                Next
                das.SelectCommand.CommandText = "insert into mmm_doc_view(DocID,UserId,fdate,aprstatus,tdate) values" & String.Join(",", arrColumnValues.ToArray())
                If con.State = ConnectionState.Closed Then
                    con.Open()
                End If
                das.SelectCommand.ExecuteNonQuery()
            Else
                das.SelectCommand.CommandText = "select d.*, dt.ordering,dt.userid from MMM_MST_DOC D left outer join MMM_DOC_DTL dt on d.LastTID=dt.tid where EID=" & HttpContext.Current.Session("EID") & " and d.tid=" & docID
                Try
                    Dim dtDoc As New DataTable
                    das.Fill(dtDoc)
                    Dim docType As String = dtDoc.Rows(0).Item("documenttype").ToString
                    Dim CurOrdering As Integer = dtDoc.Rows(0).Item("ordering").ToString
                    Dim Creator As Integer = dtDoc.Rows(0).Item("ouid").ToString
                    Dim CurDocNature As String = dtDoc.Rows(0).Item("CurdocNature").ToString
                    Dim CurrentUser As Integer = dtDoc.Rows(0).Item("userid").ToString
                    Dim CurStatus As String = dtDoc.Rows(0).Item("CurStatus").ToString

                    das.SelectCommand.CommandText = "select am.*,wf.isallowskip from MMM_MST_AuthMetrix am inner join MMM_MST_WORKFLOW_STATUS wf on am.aprStatus=wf.StatusName and am.doctype=wf.Documenttype  where am.EID=" & HttpContext.Current.Session("EID") & " and am.doctype='" & docType & "' and am.docnature='" & CurDocNature & "' AND am.ordering >" & CurOrdering & " order by am.ordering"

                    Dim dtRM As New DataTable
                    das.Fill(dtRM)
                    Dim FoundUsers As Boolean = False
                    Dim CurRoleName As String = ""
                    Dim curAprStatus As String = ""
                    Dim nxtUser As Integer
                    Dim sRetMsg As String = ""
                    Dim AllowSkip As Integer = 0
                    Dim CheckSkipfeat As Boolean = False
                    nxtUser = 0 '' intialize with zero 
                    For k As Integer = 0 To dtRM.Rows.Count - 1  '' K loop till user founds for a role type
                        das.SelectCommand.CommandText = "ApproveWorkFlow_RM_with_Isauth_2"
                        das.SelectCommand.CommandType = CommandType.StoredProcedure
                        das.SelectCommand.Parameters.Clear()
                        das.SelectCommand.Parameters.AddWithValue("tid", docID)
                        das.SelectCommand.Parameters.AddWithValue("nUid", nxtUser)
                        das.SelectCommand.Parameters.AddWithValue("NxtStatus", dtRM.Rows(k).Item("aprstatus").ToString)
                        das.SelectCommand.Parameters.AddWithValue("nOrder", dtRM.Rows(k).Item("ordering").ToString)
                        das.SelectCommand.Parameters.AddWithValue("nSLA", dtRM.Rows(k).Item("SLA").ToString)
                        If Len(qry) > 1 Then
                            das.SelectCommand.Parameters.AddWithValue("qry", qry)
                        End If
                        If Auid <> 0 Then
                            das.SelectCommand.Parameters.AddWithValue("auid", Auid)
                        End If

                        Dim dtt As New DataTable
                        das.Fill(dtt)

                        Return dtt.Rows(0).Item(0).ToString()
                    Next
                Catch ex As Exception

                End Try

            End If
        Catch ex As Exception

        End Try
        Return Res
    End Function
    Public Function GetNextUserFromRolematrixT(ByVal docID As Long, ByVal EID As Long, ByVal CUID As Integer, ByVal qry As String, ByVal Auid As Integer, ByVal con As SqlConnection, ByVal tran As SqlTransaction) As String
        Dim da As New SqlDataAdapter("select d.*, dt.ordering,dt.userid from MMM_MST_DOC D left outer join MMM_DOC_DTL dt on d.LastTID=dt.tid where EID=" & EID & " and d.tid=" & docID, con)
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
                Dim dtTmp As New DataTable
                If dtRM.Rows(k).Item("type").ToString = "NEWROLE" Then
                    CurRoleName = dtRM.Rows(k).Item("Rolename").ToString
                    curAprStatus = dtRM.Rows(k).Item("aprstatus").ToString
                    Select Case CurRoleName
                        Case "#SELF"


                            Dim dtFlds As New DataTable
                            da.SelectCommand.CommandText = "select * from MMM_MST_FIELDS where EID=" & EID & " and documenttype='" & docType & "' AND ISWORKFLOW=1"
                            da.Fill(dtFlds)

                            ' Dim strQry As String = "select top 1 uid from MMM_MST_AuthMetrix A left outer join MMM_MST_WORKFLOW_STATUS S on A.EID=S.EID and S.statusname=a.aprStatus and S.Documenttype=A.doctype where A.EID=" & EID & " and doctype='" & docType & "' and A.ordering>" & CurOrdering
                            Dim strQry As String = "select rolename from MMM_MST_AuthMetrix  where  docnature='" & CurDocNature & "' and EID=" & EID & " and doctype='" & docType & "' and ordering=" & dtRM.Rows(k).Item("ordering").ToString  'CurOrdering
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
                            Dim strQry As String = "select rolename from MMM_MST_AuthMetrix  where docnature='" & CurDocNature & "' and EID=" & EID & " and doctype='" & docType & "' and ordering=" & dtRM.Rows(k).Item("ordering").ToString  'CurOrdering
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
                                                ' strFldQry &= " and ','+" & dtRRef.Rows(i).Item("docmapping") & " +',' like '%," & dtDR.Rows(0).Item(0).ToString & ",%'"
                                                ' strFldQry &= " and (','+" & dtRRef.Rows(i).Item("docmapping") & " +',' like '%," & dtDR.Rows(0).Item(0).ToString & ",%' or ltrim(" & dtRRef.Rows(i).Item("docmapping") & ")='*' )"
                                                strFldQry &= " and (','+ convert(nvarchar(max), " & dtRRef.Rows(i).Item("docmapping") & " ) +',' like '%," & dtDR.Rows(0).Item(0).ToString & ",%' or ltrim( convert(nvarchar(max), " & dtRRef.Rows(i).Item("docmapping") & "))='*' )"
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

                                Dim strQry As String = "select rolename from MMM_MST_AuthMetrix  where docnature='" & CurDocNature & "' and EID=" & EID & " and doctype='" & docType & "' and ordering=" & dtRM.Rows(k).Item("ordering").ToString  'CurOrdering
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
                    'da.SelectCommand.CommandText = "InsertDefaultMovement_with_Isauth_2"
                    'da.SelectCommand.CommandType = CommandType.StoredProcedure
                    'da.SelectCommand.Parameters.Clear()
                    'da.SelectCommand.Parameters.AddWithValue("tid", docID)
                    'da.SelectCommand.Parameters.AddWithValue("what", "ARCHIVE")
                    'da.SelectCommand.Parameters.AddWithValue("qry", qry)
                    'If con.State <> ConnectionState.Open Then
                    '    con.Open()
                    'End If
                    'da.SelectCommand.ExecuteNonQuery()
                    dtDoc.Dispose() : da.Dispose()
                    'Return "ARCHIVE:" & docType
                    Return ""
                End If
                dtDoc.Dispose()
            Next
        Catch ex As Exception

        End Try


    End Function
    Public Function GetMasterData(dropdown As String, eid As Integer, messageBody As String) As Integer
        Dim Result As Integer = 0
        Dim obj As New Data()
        Dim TableName As String = String.Empty
        Dim strQuery As String = String.Empty
        Dim conStrs As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim cons As New SqlConnection(conStrs)
        Dim das As SqlDataAdapter = New SqlDataAdapter("", cons)
        Dim dropdownvalue As String() = dropdown.ToString().Split("-")
        If dropdownvalue(0).ToString().Trim().ToUpper = "MASTER" Then
            TableName = "MMM_MST_MASTER"
            strQuery = "select TID," & dropdownvalue(2) & " from " & TableName & " where documenttype='" & dropdownvalue(1) & "' and eid=" & eid & ""

        End If
        das.SelectCommand.CommandText = strQuery
        Dim dt As New DataTable
        das.Fill(dt)
        Result = ReturnMasterValueExistInMailBody(dt, messageBody, dropdownvalue(2).ToString())
        Return Result
    End Function

    Public Function ReturnMasterValueExistInMailBody(dt As DataTable, messageBody As String, ColumnName As String) As Integer
        Dim Result As Integer = 0
        For Each dr As DataRow In dt.Rows
            If messageBody.ToString().Contains(dr(ColumnName)) Then
                Result = dr(0)
                Return Result
            End If
        Next
    End Function
    ''Safe String function to remove special character from string 
    Public Function getSafeString(ByVal strVar As String) As String
        Trim(strVar)
        strVar = Replace(strVar, "'", "")
        strVar = Replace(strVar, ";", "")
        strVar = Replace(strVar, "--", "")
        strVar = Replace(strVar, "%", "")
        strVar = Replace(strVar, "&", "")
        Return strVar
    End Function
    Protected Sub btnShowGrid_Click(sender As Object, e As EventArgs) Handles btnShowGrid.Click
        Try
            Dim TS As New TicketScheduler()
            TS.CreateDocumentEntityWise()
            'CreateDocumentEntityWise()
        Catch ex As Exception
            If ex.Message <> "Invalid password" Then
                lblMsg.Text = "Error in mail sending - " & Convert.ToSingle(ex.InnerException.Message)
            End If

        End Try
    End Sub




    Protected Sub btnDelMail_Click(sender As Object, e As EventArgs) Handles btnDelMail.Click
        If txtDelID.Text = "" Then
            lblMsg.Text = "Enter Dele msg id"
            Exit Sub
        End If
        Dim pop3Client As Pop3Client
        pop3Client = New Pop3Client
        pop3Client.Connect("mail.myndsol.com", 995, True)
        pop3Client.Authenticate("hd.myndsaas@myndsol.com", "MS0000#2")
        'If (Session("Pop3Client") Is Nothing) Then
        '    pop3Client = New Pop3Client
        '    pop3Client.Connect("mail.myndsol.com", 995, True)
        '    pop3Client.Authenticate("hd.myndsaas@myndsol.com", "MS0000#2")
        '    Session("Pop3Client") = pop3Client
        'Else
        '    pop3Client = CType(Session("Pop3Client"), Pop3Client)
        'End If
        ' DeleteMessageByMessageId("mail.myndsol.com", 995, True, "hd.myndsaas@myndsol.com", "MS0000#2", 1)
        If DeleteMessageByMessageId(pop3Client, txtDelID.Text) = True Then
            lblMsg.Text = "Message marked for deletion successfully"
        Else
            lblMsg.Text = "Issue while marking for deletion action"
        End If
    End Sub

    '''' <summary>
    '''' Example showing:
    ''''  - how to delete fetch an emails headers only
    ''''  - how to delete a message from the server
    '''' </summary>
    '''' <param name="client">A connected and authenticated Pop3Client from which to delete a message</param>
    '''' <param name="messageId">A message ID of a message on the POP3 server. Is located in <see cref="MessageHeader.MessageId"/></param>
    '''' <returns><see langword="true"/> if message was deleted, <see langword="false"/> otherwise</returns>
    Public Function DeleteMessageByMessageId(client As Pop3Client, messageId As String) As Boolean
        ' Get the number of messages on the POP3 server
        Dim messageCount As Integer = client.GetMessageCount()

        ' Run trough each of these messages and download the headers
        For messageItem As Integer = messageCount To 1 Step -1
            ' If the Message ID of the current message is the same as the parameter given, delete that message
            'Dim TempMessageID As String = client.GetMessageHeaders(messageItem).MessageId
            Dim TempMessageID As String = client.GetMessageUid(messageItem)
            'If TempMessageID = messageId Then
            If client.GetMessageHeaders(messageItem).MessageId = messageId Then
                ' Delete
                client.DeleteMessage(messageItem)
                client.Disconnect()
                Return True
            End If
        Next

        ' We did not find any message with the given messageId, report this back
        Return False
    End Function

    Protected Sub btnWebService_Click(sender As Object, e As EventArgs)
        Try
            apicall("http://myndsaas.com/DMSService.SVC/Run_Helpdesk_MailRead_Sch")
        Catch ex As Exception
        End Try

    End Sub
    Public Function apicall(ByVal url As String) As String
        Dim httpreq As HttpWebRequest = DirectCast(WebRequest.Create(url), HttpWebRequest)
        Try
            Dim httpres As HttpWebResponse = DirectCast(httpreq.GetResponse(), HttpWebResponse)
            Dim sr As New StreamReader(httpres.GetResponseStream())
            Dim results As String = sr.ReadToEnd()
            sr.Close()
            Return results
        Catch
            Return "0"
        End Try
    End Function


End Class
<Serializable()> _
Public Class Email
    Public Sub New()
        MyBase.New()
        Me.Attachments = New List(Of Attachment)
    End Sub
    Public Property MessageID As String
    Public Property MessageNumber As Integer
    Public Property From As String
    Public Property DisplayName As String
    Public Property Subject As String
    Public Property Body As String
    Public Property EID As Integer
    Public Property DocumentType As String
    Public Property IsAllowCreateUser As Boolean
    Public Property DateSent As DateTime
    Public Property Attachments As List(Of Attachment)
End Class
<Serializable()> _
Public Class Attachment

    Public Property FileName As String
    Public Property ContentType As String
    Public Property Content As Byte()
End Class
Public Class Data
    Public Property objText As String
    Public Property objValue As String
End Class
