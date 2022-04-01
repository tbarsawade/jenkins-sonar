Imports Microsoft.VisualBasic
Imports System.Data.SqlClient
Imports System.Data
Imports OpenPop.Pop3
Imports System.IO
Imports OpenPop.Mime
Imports OpenPop.Mime.Header

Public Class TicketScheduler

    Dim Resultvalue As String
    Dim ReverseFieldMapping As New ArrayList()
    Dim ReverseFieldData As New ArrayList()
    Dim objDC As New DataClass()
    Public Sub CreateDocumentEntityWise()
        Try
            Dim conStrs As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim cons As New SqlConnection(conStrs)
            Dim das As SqlDataAdapter = New SqlDataAdapter("", cons)
            'das.SelectCommand.CommandText = "select * from mmm_hdmail_schdule where tid=25"
            das.SelectCommand.CommandText = "select * from mmm_hdmail_schdule with (nolock) where isactive=1   and  mdmailid is not null  and mdpwd is not null and mdport is not null and mdisssl is not null and hostname is not null "
            Dim dt As New DataTable
            das.Fill(dt)
            If dt.Rows.Count > 0 Then
                For Each dr As DataRow In dt.Rows
                    Try
                        If Not (String.IsNullOrEmpty(dr("mdmailid")) And String.IsNullOrEmpty(dr("mdpwd")) And String.IsNullOrEmpty(dr("mdport")) And String.IsNullOrEmpty(dr("mdisssl")) And String.IsNullOrEmpty(dr("hostname"))) Then
                            Dim pop3Client As Pop3Client
                            Dim objPOP As New POP()
                            objPOP.hostName = Convert.ToString(dr("hostname"))
                            objPOP.mdport = Convert.ToInt32(dr("mdport"))
                            objPOP.mdSSL = IIf(dr("mdisssl") = 1, True, False)
                            objPOP.mdMailID = Convert.ToString(dr("mdmailid"))
                            objPOP.mdPwd = Convert.ToString(dr("mdpwd"))
                            pop3Client = New Pop3Client
                            pop3Client.Connect(Convert.ToString(dr("hostname")), Convert.ToInt32(dr("mdport")), True)
                            pop3Client.Authenticate(Convert.ToString(dr("mdmailid")), Convert.ToString(dr("mdpwd")))
                            Dim count As Integer = pop3Client.GetMessageCount
                            Dim counter As Integer = 0
                            Dim i As Integer = count
                            Dim Emails As New List(Of Email)
                            Dim Doucmenttype As String = ""
                            Dim EID As Integer = 0
                            Do While (i >= 1)
                                Dim message As Message = pop3Client.GetMessage(i)
                                'Emails = New List(Of Email)
                                Dim email As New Email()
                                email.EID = Convert.ToInt32(dr("EID"))
                                email.DocumentType = Convert.ToString(dr("DocumentType"))
                                email.MessageNumber = i
                                email.MessageID = message.Headers.MessageId
                                email.Subject = message.Headers.Subject.ToString().Replace("'", "''")
                                email.DateSent = message.Headers.DateSent
                                email.From = message.Headers.From.Address
                                email.CC = message.Headers.Cc
                                email.BCC = message.Headers.Bcc
                                email.RolematrixfromOrganization = dr("RolematrixfromOrganization")
                                email.BlockEmailID = Convert.ToString(dr("BlockEmailID"))

                                'HttpContext.Current.Session("FROM") = message.Headers.From.Address
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
                                    email.Attachments.Add(New Attachment With {.FileName = attachment.FileName, .ContentType = attachment.ContentType.MediaType, .Content = attachment.Body})
                                Next
                                Emails.Add(email)
                                counter = counter + 1
                                i = i - 1
                                'If counter = 5 Then
                                '    Exit Do
                                'End If
                            Loop
                            DocumentCreationInBPM(Emails, Convert.ToInt32(dr("EID")), Convert.ToString(dr("DocumentType")), objPOP, dr("USERROLE"), dr("ADMINROLE"), dr("Ticket_Assign_Method"))
                        End If
                    Catch ex As Exception
                    End Try
                Next
            End If
        Catch ex As Exception
            Throw
        End Try
    End Sub
    Public Sub DocumentCreationInBPM(listOfEmails As List(Of Email), EID As Integer, Documenttype As String, client As POP, USERROLE As String, ADMINROLE As String, Optional ByVal TicketAssignMethod As String = "PULL")
        Try
            If listOfEmails.Count > 0 Then
                Dim conStrs As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
                Dim cons As New SqlConnection(conStrs)
                Dim das As SqlDataAdapter = New SqlDataAdapter("", cons)
                das.SelectCommand.CommandText = "select * from mmm_mst_fields with(nolock) where documenttype='" & Documenttype & "' and eid=" & EID & " and (MDFieldName is not null or ParseFromEmail=1) union all select * from MMM_MST_FIELDS  with(nolock) where DocumentType in (select dropdown from mmm_mst_fields  with(nolock) where documenttype='" & Documenttype & "' and eid=" & EID & "   and fieldType='Child Item' ) and eid=" & EID & " and MDFieldName is not null"
                Dim dt As New DataTable
                Dim DocID As String = "0"
                das.Fill(dt)
                Dim UID As Integer = 0
                Dim Status As String = ""
                If dt.Rows.Count > 0 Then
                    For Each emails As Email In listOfEmails
                        If emails.From.ToString().Trim().ToUpper = emails.BlockEmailID.ToString().ToUpper() Then
                            DeleteMessageByMessageId(client, emails.MessageID)
                        Else
                            Dim CurTicketStatus As String = ""
                            Resultvalue = Nothing
                            Dim LookupValue As String = ""
                            Dim LookupFieldMapping As String = ""
                            Dim ResultMaster As Integer = 0
                            Dim emailAttachment As New EmailAttachment()
                            Dim ArrayColumn As New ArrayList()
                            Dim ChildArrayColumn As New ArrayList()
                            Dim ChildArrayColumnValue As New ArrayList()
                            Dim ArrayColumnValue As New ArrayList()
                            Dim fieldName As String = String.Empty
                            For Each dr As DataRow In dt.Rows
                                Select Case dr("MDfieldName").ToString().ToUpper
                                    Case "EMAILID"
                                        ArrayColumn.Add(dr("FieldMapping"))
                                        ArrayColumnValue.Add("'" & emails.From & "'")
                                    Case "SUBJECT"
                                        ArrayColumn.Add(dr("FieldMapping"))
                                        ArrayColumnValue.Add("'" & emails.Subject & "'")
                                    Case "REMARKS"
                                        ArrayColumn.Add(dr("FieldMapping"))
                                        Dim email As String = emails.Body.ToString().Replace("'", "''''")
                                        ArrayColumnValue.Add("'" & email.ToString() & "'")
                                    Case "MESSAGEID"
                                        ArrayColumn.Add(dr("FieldMapping"))
                                        ArrayColumnValue.Add("'" & emails.MessageID & "'")
                                    Case "NAME"
                                        ArrayColumn.Add(dr("FieldMapping"))
                                        ArrayColumnValue.Add("'" & emails.DisplayName & "'")
                                    Case "STATUS"
                                        Dim Result As ArrayList
                                        Result = IsUserExist(emails.From, emails.IsAllowCreateUser, EID, emails.DisplayName, USERROLE, ADMINROLE)
                                        ArrayColumn.Add("oUID")
                                        UID = Result(0)
                                        ArrayColumnValue.Add(Result(0))
                                        ArrayColumn.Add(dr("FieldMapping"))
                                        DocID = IsExistMailDoc(emails.Body)
                                        If DocID <> "0" And DocID <> "" And DocID <> String.Empty Then
                                            CurTicketStatus = Convert.ToString(objDC.ExecuteQryDT("select " & dr("FieldMapping") & " from mmm_mst_doc where tid=" & DocID & " ").Rows(0)(0)).ToUpper
                                        End If
                                        If DocID = "0" Or DocID = "" Or DocID <> String.Empty Then
                                            ArrayColumnValue.Add(Result(1))
                                            Status = Result(1)
                                        Else
                                            Status = "'OPEN'"
                                            ArrayColumnValue.Add("'OPEN'")
                                        End If
                                    Case "CC"
                                        ArrayColumn.Add(dr("FieldMapping"))
                                        Dim data As New ArrayList()
                                        For Each rfc As RfcMailAddress In emails.CC
                                            Dim ccvalue As String() = rfc.ToString().Split("<")
                                            If ccvalue.Length > 1 Then
                                                data.Add(ccvalue(1).ToString().Replace(">", ""))
                                            Else
                                                data.Add(rfc.ToString())
                                            End If
                                        Next
                                        ArrayColumnValue.Add("'" & String.Join(";", data.ToArray()) & "'")
                                    Case "BCC"
                                        ArrayColumn.Add(dr("FieldMapping"))
                                        Dim data As New ArrayList()
                                        For Each rfc As RfcMailAddress In emails.BCC
                                            data.Add(rfc.ToString())
                                        Next
                                        ArrayColumnValue.Add("'" & String.Join(";", data.ToArray()) & "'")
                                    Case "ATTACHMENT"
                                        Dim attachmentsColumn As New ArrayList()
                                        Dim attachmentsColumnValue As New ArrayList()

                                        das.SelectCommand.CommandText = "select dropdown from mmm_mst_fields  with(nolock) where documenttype='" & Documenttype & "' and eid=" & EID & " and fieldType='Child Item'"
                                        If cons.State = ConnectionState.Closed Then
                                            cons.Open()
                                        End If
                                        Dim childDocumenttype As String = Convert.ToString(das.SelectCommand.ExecuteScalar())

                                        For Each att As Attachment In emails.Attachments
                                            Dim childAttachment As New ChildAttachment()
                                            Dim img As Byte() = att.Content
                                            If att.FileName.Contains(".") Then
                                                Dim ext As String = att.FileName.Substring(att.FileName.ToString().LastIndexOf("."), att.FileName.ToString().Length - att.FileName.ToString().LastIndexOf("."))
                                                Dim partPath = EID & "/" & getSafeString(Documenttype) & "_" & Now.Day & Now.Month & Now.Year & Now.Minute & Now.Second & Now.Millisecond & dr.Item("FieldID").ToString() & ext
                                                Dim Path As String = System.Web.Hosting.HostingEnvironment.MapPath("~\DOCS\") & partPath
                                                'HttpContext.Current.Server.MapPath("DOCS/") & partPath
                                                If Not Directory.Exists(System.Web.Hosting.HostingEnvironment.MapPath("~\DOCS\") & EID) Then
                                                    Directory.CreateDirectory(System.Web.Hosting.HostingEnvironment.MapPath("~\DOCS\") & EID)
                                                End If
                                                File.WriteAllBytes(Path, img)
                                                childAttachment.FieldMapping = dr("FieldMapping")
                                                childAttachment.FileName = att.FileName
                                                childAttachment.FilePath = partPath
                                                childAttachment.ChildDocumenttype = childDocumenttype
                                                emailAttachment.ChildAttachments.Add(New ChildAttachment With {.FieldMapping = Convert.ToString(dr("FieldMapping")), .FileName = att.FileName, .FilePath = partPath, .ChildDocumenttype = childDocumenttype})
                                            Else
                                                'For Message Body
                                                'HttpContext.Current.Server.MapPath("DOCS/") & partPath
                                                If Not Directory.Exists(System.Web.Hosting.HostingEnvironment.MapPath("~\DOCS\") & EID) Then
                                                    Directory.CreateDirectory(System.Web.Hosting.HostingEnvironment.MapPath("~\DOCS\") & EID)
                                                End If
                                                Dim partPath = EID & "/" & getSafeString(Documenttype) & "_" & Now.Day & Now.Month & Now.Year & Now.Minute & Now.Second & Now.Millisecond & dr.Item("FieldID").ToString() & ".MailBody"
                                                Dim Path As String = System.Web.Hosting.HostingEnvironment.MapPath("~\DOCS\") & partPath
                                                File.WriteAllBytes(Path, img)
                                                childAttachment.FieldMapping = dr("FieldMapping")
                                                childAttachment.FileName = att.FileName
                                                childAttachment.FilePath = partPath
                                                childAttachment.ChildDocumenttype = childDocumenttype
                                                emailAttachment.ChildAttachments.Add(New ChildAttachment With {.FieldMapping = Convert.ToString(dr("FieldMapping")), .FileName = att.FileName, .FilePath = partPath, .ChildDocumenttype = childDocumenttype})
                                            End If
                                        Next
                                        'ChildArrayColumn.Add()
                                        'ChildArrayColumnValue.Add(String.Join("|", attachments.ToArray()))
                                    Case Else
                                        ArrayColumn.Add(dr("FieldMapping"))
                                        If ResultMaster = 0 Then
                                            ResultMaster = GetMasterData(dr("dropdown"), EID, emails.Body & emails.Subject, dr("fieldid"), Documenttype, Convert.ToString(dr("TagFieldMapping")))
                                        End If
                                        ArrayColumnValue.Add(ResultMaster)
                                End Select
                            Next
                            If ResultMaster <> 0 Then
                                For x As Integer = 0 To ReverseFieldMapping.Count - 1
                                    ArrayColumn.Add(ReverseFieldMapping(x))
                                    ArrayColumnValue.Add(ReverseFieldData(x))
                                Next
                            End If
                            ArrayColumn.Add("EID")
                            ArrayColumnValue.Add(EID)
                            ArrayColumn.Add("Documenttype")
                            ArrayColumnValue.Add("'" & Documenttype & "'")
                            ArrayColumn.Add("adate")
                            ArrayColumnValue.Add("getdate()")
                            ArrayColumn.Add("TicketStatus")
                            ArrayColumnValue.Add(Status)
                            SavingMailDoc(ArrayColumn, ArrayColumnValue, emailAttachment.ChildAttachments, client, emails.MessageID, DocID, Documenttype, EID, UID, emails.From, emails.RolematrixfromOrganization, USERROLE, ADMINROLE, TicketAssignMethod, ResultMaster)
                        End If
                    Next
                End If
            End If
        Catch ex As Exception
            Throw
        End Try

    End Sub
    Private Shared Function GetUserID(ByVal emailId As String, ByVal EID As Integer) As Integer
        Dim ret As Integer = 0
        Dim dc As New DataClass()
        ret = Convert.ToInt32(dc.ExecuteQryScaller("select uid from mmm_mst_user where emailid='" & emailId & "' and eid=" & EID))
        Return ret
    End Function
    Private Shared Function HtmlToPlainText(ByVal html As String) As String
        Const tagWhiteSpace As String = "(>|$)(\W|\n|\r)+<"
        'matches one or more (white space or line breaks) between '>' and '<'
        Const stripFormatting As String = "<[^>]*(>|$)"
        'match any character between '<' and '>', even when end tag is missing
        Const lineBreak As String = "<(br|BR)\s{0,1}\/{0,1}>"
        'matches: <br>,<br/>,<br />,<BR>,<BR/>,<BR />
        Dim lineBreakRegex = New Regex(lineBreak, RegexOptions.Multiline)
        Dim stripFormattingRegex = New Regex(stripFormatting, RegexOptions.Multiline)
        Dim tagWhiteSpaceRegex = New Regex(tagWhiteSpace, RegexOptions.Multiline)
        Dim text = html
        'Decode html specific characters
        text = System.Net.WebUtility.HtmlDecode(text)
        'Remove tag whitespace/line breaks
        text = tagWhiteSpaceRegex.Replace(text, "><")
        'Replace <br /> with line breaks
        text = lineBreakRegex.Replace(text, Environment.NewLine)
        'Strip formatting
        text = stripFormattingRegex.Replace(text, String.Empty)
        Return text
    End Function
    Public Function IsExistMailDoc(messageBody As String) As String
        Dim Result As String = ""
        Dim myplainTextString As String = HtmlToPlainText(messageBody)
        Dim FirstIndex As Integer = 0
        Dim LastIndex As Integer = 0
        If (myplainTextString.ToString().Contains("SID#")) Then
            FirstIndex = myplainTextString.ToString().IndexOf("SID#")
            LastIndex = myplainTextString.ToString().IndexOf("::")
            If LastIndex = -1 Then
                LastIndex = myplainTextString.ToString().Length
            End If
            FirstIndex = FirstIndex + 4
            Result = myplainTextString.ToString().Substring(FirstIndex, LastIndex - FirstIndex)
            If Not IsNumeric(Result) Then
                Result = "0"
            End If
            Return Result
        Else
            Result = "0"
        End If
        Return Result
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
    Public Function GetMasterData(dropdown As String, eid As Integer, messageBody As String, Optional ByVal fieldid As String = "0", Optional ByVal Documenttype As String = "Ticket", Optional ByVal TagMapping As String = "") As Integer
        Dim Result As Integer = 0
        Dim obj As New DataValues()
        Dim TableName As String = String.Empty
        Dim strQuery As String = String.Empty
        Dim conStrs As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim cons As New SqlConnection(conStrs)
        Dim das As SqlDataAdapter = New SqlDataAdapter("", cons)
        Dim dt As New DataTable
        Dim dropdownvalue As String() = dropdown.ToString().Split("-")
        Try
            If TagMapping <> String.Empty Or TagMapping <> "" Then
                If dropdownvalue(0).ToString().Trim().ToUpper = "MASTER" Then
                    TableName = "MMM_MST_MASTER"
                    strQuery = "select TID," & TagMapping & " from " & TableName & " where documenttype='" & dropdownvalue(1) & "' and eid=" & eid & ""
                    das.SelectCommand.CommandText = strQuery
                    das.Fill(dt)
                    Result = ReturnMasterValueExistInMailBody(dt, messageBody, TagMapping, fieldid, eid, Documenttype)
                End If
            End If

        Catch ex As Exception
        Finally
        End Try
        Return Result
    End Function
    Public Function ReturnMasterValueExistInMailBody(dt As DataTable, messageBody As String, ColumnName As String, Optional ByVal fieldid As String = "", Optional ByVal eid As Integer = 0, Optional ByVal Document As String = "Ticket") As Integer
        Dim Result As Integer = 0
        For Each dr As DataRow In dt.Rows
            Dim tags As String() = Convert.ToString(dr(ColumnName)).Split(",")
            For l As Integer = 0 To tags.Length - 1
                If messageBody.ToString().ToUpper.Contains(tags(l).ToString().ToUpper) Then
                    Result = dr(0)
                    Dim objdc As New DataClass
                    Dim objdt As New DataTable
                    objdt = objdc.ExecuteQryDT("select LOOKUPVALUE,dropdown,documenttype,dropdowntype from MMM_MST_FIELDS  WITH(NOLOCK)  WHERE FIELDID=" & fieldid & "")
                    Dim LOOKUPVALUE As String = objdt.Rows(0).Item("lookupvalue").ToString()
                    Dim documenttype() As String = objdt.Rows(0).Item("dropdown").ToString.Split("-")
                    If LOOKUPVALUE.Length > 1 Then
                        Dim lookup As String() = LOOKUPVALUE.ToString().Split(",")
                        For i As Integer = 0 To lookup.Length - 1
                            If lookup(i).ToString().Contains("-R") Then
                                Dim fldid As String() = lookup(i).ToString().Split("-R")
                                If fldid.Length > 1 Then
                                    Dim dttemp As New DataTable
                                    dttemp = objdc.ExecuteQryDT("SELECT * FROM MMM_MST_FIELDS  WITH(NOLOCK)  WHERE FIELDID=" & fldid(0) & "")
                                    Dim TAB1 As String = ""
                                    Dim TAB2 As String = ""
                                    Dim STID As String = ""
                                    Dim TID As String = ""
                                    If documenttype(0).ToString.ToUpper = "MASTER" Then
                                        TAB2 = "MMM_MST_MASTER"
                                        TID = "TID"
                                    ElseIf documenttype(0).ToString.ToUpper = "DOCUMENT" Then
                                        TAB2 = "MMM_MST_DOC"
                                        TID = "TID"
                                    ElseIf documenttype(1).ToString.ToUpper = "USER" Then
                                        TAB2 = "MMM_MST_USER"
                                        TID = "UID"
                                    End If
                                    Dim DOCTYPE() As String = dttemp.Rows(0).Item("DROPDOWN").ToString.Split("-")
                                    If DOCTYPE(0).ToString.ToUpper = "MASTER" Then
                                        TAB1 = "MMM_MST_MASTER"
                                        STID = "TID"
                                    ElseIf DOCTYPE(0).ToString.ToUpper = "DOCUMENT" Then
                                        TAB1 = "MMM_MST_DOC"
                                        STID = "TID"
                                    ElseIf DOCTYPE(1).ToString.ToUpper = "USER" Then
                                        TAB1 = "MMM_MST_USER"
                                        STID = "UID"
                                    End If
                                    Dim objdNew As New DataTable
                                    ''Dim DDL0 As DropDownList = TryCast(pnlFields.FindControl("fld" & DROPDOWN1), DropDownList)
                                    Dim ht As New Hashtable
                                    ht.Add("@EID", eid)
                                    ht.Add("@TAB1", TAB1)
                                    ht.Add("@TAB2", TAB2)
                                    ht.Add("@DOCUMENTTYPE", DOCTYPE(1).ToString)
                                    ht.Add("@FLDMAPPING", DOCTYPE(2).ToString)
                                    ht.Add("@AUTOFILTER", dttemp.Rows(0).Item("AUTOFILTER").ToString())
                                    ht.Add("@TID", TID)
                                    ht.Add("@STID", STID)
                                    ht.Add("@VAL", dr(0))
                                    objdNew = objdc.ExecuteProDT("USP_GETMANNUALFILTER", ht)
                                    If objdNew.Rows.Count > 0 Then
                                        ReverseFieldData.Add(objdNew.Rows(0)(1))
                                        ReverseFieldMapping.Add(objdc.ExecuteQryScaller("select fieldmapping from mmm_mst_fields where documenttype='" & Document & "' and dropdown like '%-" & DOCTYPE(1).ToString & "-%' and eid=" & eid))
                                    End If

                                End If
                            ElseIf lookup(i).ToString().Contains("-fld") Then
                                Dim fldid As String() = lookup(i).ToString().Split("-")
                                If fldid.Length > 1 Then
                                    Dim dttemp As New DataTable
                                    dttemp = objdc.ExecuteQryDT("SELECT * FROM MMM_MST_FIELDS  WITH(NOLOCK)  WHERE FIELDID=" & fldid(0) & "")
                                    Dim TABLENAME As String = ""
                                    Dim TID As String = "TID"
                                    If UCase(documenttype(0).ToString) = "MASTER" Then
                                        TABLENAME = "MMM_MST_MASTER"
                                    ElseIf UCase(documenttype(0).ToString) = "DOCUMENT" Then
                                        TABLENAME = "MMM_MST_DOC"
                                    ElseIf UCase(documenttype(0).ToString) = "STATIC" Then
                                        TABLENAME = "MMM_MST_USER"
                                        TID = "UID"
                                    Else
                                        TABLENAME = objdt.Rows(0).Item("DBTABLENAME").ToString
                                    End If
                                    Dim ht As New Hashtable()
                                    ht.Add("@EID", eid)
                                    ht.Add("@documentType", documenttype(1))
                                    ht.Add("@Type", documenttype(0))
                                    ht.Add("@TID", dr(0))
                                    ht.Add("@FLDMAPPING", fldid(1))
                                    Dim objVal As New DataTable
                                    objVal = objdc.ExecuteProDT("uspGetMasterValue", ht)
                                    ReverseFieldData.Add("'" & objVal.Rows(0)(0) & "'")
                                    ReverseFieldMapping.Add(dttemp.Rows(0)("fieldmapping"))
                                End If
                            End If
                        Next
                    End If
                    Return Result
                End If
            Next
        Next
        Return Result
    End Function
    Public Function IsUserExist(From As String, IsAllowUserCreate As Boolean, EID As Integer, DisplayName As String, USERROLE As String, ADMINROLE As String) As ArrayList
        Dim Result As New ArrayList()
        Dim objDC As New DataClass()
        Dim dt As New DataTable
        Dim conStrs As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim cons As New SqlConnection(conStrs)
        Dim das As SqlDataAdapter = New SqlDataAdapter("", cons)
        'das.SelectCommand.CommandText = "select uid from mmm_mst_user  with(nolock) where eid=" & EID & " And emailid='" & From & "'"
        dt = objDC.ExecuteQryDT("select isnull(LoginField,'') as LoginField from mmm_mst_entity where eid=" & EID)
        If dt.Rows.Count = 1 Then
            dt = objDC.ExecuteQryDT("select uid from mmm_mst_user where " & dt.Rows(0)(0) & "='" & From & "' and eid=" & EID)
        End If
        If dt.Rows.Count = 0 Then
            dt = objDC.ExecuteQryDT("select uid from mmm_mst_user  with(nolock) where eid=" & EID & " And emailid='" & From & "'")
        End If
        'das.Fill(dt)
        If dt.Rows.Count > 0 Then
            Result.Add(dt.Rows(0)(0))
            Result.Add("'OPEN'")
        ElseIf IsAllowUserCreate = True Then
            Dim SPLITUSERROLE As String() = USERROLE.ToString().Split(",")
            das.SelectCommand.CommandText = "insert into mmm_mst_user (username,userid,emailid,userrole,isauth,eid,passtry,modifydate,locationid) values ('" & DisplayName & "','" & From & "','" & From & "','" & SPLITUSERROLE(0) & "',100," & EID & ",0,getdate(),2072);select scope_identity()"
            If cons.State = ConnectionState.Closed Then
                cons.Open()
            End If
            Dim UID As Integer = Convert.ToInt32(das.SelectCommand.ExecuteScalar())
            Result.Add(UID)
            Result.Add("'OPEN'")
            Dim obDMS As New DMSUtil()
            obDMS.notificationMail(UID, EID, "USER", "USER CREATED")
        Else
            'das.SelectCommand.CommandText = "select uid from mmm_mst_user  with(nolock) where eid=" & EID & " and userRole='" & ADMINROLE & "'"
            das.SelectCommand.CommandText = "select uid from mmm_mst_user  with(nolock) where userrole in (select  adminrole from mmm_hdmail_schdule where eid=" & EID & ") and eid=" & EID & " order by uid"
            If cons.State = ConnectionState.Closed Then
                cons.Open()
            End If
            Result.Add(Convert.ToInt32(das.SelectCommand.ExecuteScalar()))
            Result.Add("'SUSPENDED'")
        End If
        Return Result
    End Function
    Public Sub SavingMailDoc(arrayColumn As ArrayList, arrayColumnvalue As ArrayList, childDocumet As List(Of ChildAttachment), client As POP, messageID As String, DocID As String, DocumentType As String, EID As Integer, UID As Integer, From As String, Optional ByVal RolematrixfromOrganization As Boolean = False, Optional ByVal USERROLE As String = "END_USER", Optional ByVal ADMINROLE As String = "ADMIN", Optional ByVal TicketAssignMethod As String = "PULL", Optional ByVal ResultMaster As Integer = 0)
        Try
            If arrayColumn.Count > 0 And arrayColumnvalue.Count > 0 Then
                Dim OperationType As String = "UPDATE"
                Dim strQuery As String = String.Empty
                Dim IsAllow As Boolean = True
                Dim ob As New DynamicForm
                Dim conStrs As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
                Dim cons As New SqlConnection(conStrs)
                cons.Open()
                Dim tran As SqlTransaction = Nothing
                tran = cons.BeginTransaction()
                Dim das As SqlDataAdapter = New SqlDataAdapter("", cons)
                If DocID = "0" Or DocID = "" Then
                    das.SelectCommand.CommandText = "insert into mmm_mst_doc (" & String.Join(",", arrayColumn.ToArray()) & ") values (" & String.Join(",", arrayColumnvalue.ToArray()) & ");select scope_identity()"
                Else
                    Dim objDC As New DataClass()
                    If Convert.ToInt32(objDC.ExecuteQryScaller("select count(*) from mmm_mst_doc where documenttype='Ticket' and eid=" & EID & " and TicketStatus='CLOSED' and tid=" & DocID)) = 0 Then
                        Dim ht As New Hashtable
                        Dim dtresult As New DataTable
                        ht.Add("@DOCID", DocID)
                        dtresult = objDC.ExecuteProDT("TicketRevertlaststageforArchive", ht)

                        Dim query As New StringBuilder()
                        Dim status As String = ""
                        query.Append("Update mmm_mst_doc set  ")
                        For k As Integer = 0 To arrayColumn.Count - 1
                            If Not (arrayColumn(k) = "EID" Or arrayColumn(k) = "Documenttype" Or arrayColumn(k) = "adate") Then
                                If arrayColumn(k) = "Status" Then
                                    query.Append(arrayColumn(k) & " =  'OPEN' ,")
                                    status = "'OPEN'"
                                ElseIf arrayColumn(k) = "TicketStatus" Then
                                    query.Append(arrayColumn(k) & " =  'OPEN' ,")
                                    status = "'OPEN'"
                                ElseIf arrayColumn(k) = "oUID" Then

                                Else
                                    query.Append("" & arrayColumn(k) & " = " & arrayColumnvalue(k) & " ,")
                                    status = arrayColumnvalue(k)
                                End If
                            End If
                        Next
                        query.Append("lastupdate=getdate() ")

                        query.Append(" where tid=" & DocID & " and eid=" & EID)
                        das.SelectCommand.CommandText = query.ToString()
                    Else
                        IsAllow = False
                    End If
                End If
                If IsAllow = True Then
                    das.SelectCommand.Transaction = tran
                    If cons.State = ConnectionState.Closed Then
                        cons.Open()
                    End If
                    Dim tid As Integer = das.SelectCommand.ExecuteScalar()
                    Dim objDMSUtil As New DMSUtil()
                    If DocID = "0" Or DocID = "" Then
                        OperationType = "CREATE"
                        If childDocumet.Count > 0 Then
                            For Each item As ChildAttachment In childDocumet
                                das.SelectCommand.CommandText = "insert into mmm_mst_doc_item (DOCID,DOCUMENTTYPE,ISAUTH,LASTUPDATE,cmastertid," & item.FieldMapping & ",Attachment) values(" & tid & ",'" & item.ChildDocumenttype & "',1,getdate(),0,'" & item.FilePath & "','" & item.FileName & "')"
                                If cons.State = ConnectionState.Closed Then
                                    cons.Open()
                                End If
                                das.SelectCommand.ExecuteNonQuery()
                            Next
                        End If
                        CheckAutoNumber(DocumentType, tid, cons, tran, EID)
                        das.SelectCommand.CommandText = "InsertDefaultMovement"
                        das.SelectCommand.CommandType = CommandType.StoredProcedure
                        das.SelectCommand.Parameters.Clear()
                        das.SelectCommand.Parameters.AddWithValue("tid", tid)
                        das.SelectCommand.Parameters.AddWithValue("CUID", Val(UID))
                        das.SelectCommand.Parameters.AddWithValue("what", "UPLOADED")
                        das.SelectCommand.ExecuteNonQuery()

                        Dim res As String
                        Try
                            If RolematrixfromOrganization Then
                                res = GetNextUserFromOrganizatios(cons, tran, From, EID, ADMINROLE, ResultMaster)
                            Else
                                res = GetNextUserFromRolematrixT(tid, EID, UID, "", UID, cons, tran)
                                If String.IsNullOrEmpty(res) Then
                                    res = GetNextUserFromOrganizatios(cons, tran, From, EID, ADMINROLE, ResultMaster)
                                End If
                            End If
                            AssignTicketToUserBasedOnCondition(res, tid, "", UID, cons, tran, EID, RolematrixfromOrganization, TicketAssignMethod, DocumentType, OperationType)
                            'Insert data into mmm_mst_doc_item  table
                            ob.HistoryT(EID, tid, UID, DocumentType, "MMM_MST_DOC", "ADD", cons, tran)
                            Trigger.ExecuteTriggerT(DocumentType, EID, tid, cons, tran)
                            DeleteMessageByMessageId(client, messageID)
                            tran.Commit()
                            If IsNothing(Resultvalue) Then
                                objDMSUtil.TemplateCalling(tid, EID, DocumentType, "CREATED", client.mdMailID)
                                objDMSUtil.TemplateCalling(tid, EID, DocumentType, "APPROVE", client.mdMailID)
                            Else
                                objDMSUtil.TemplateCalling(tid, EID, DocumentType, "CREATED", client.mdMailID)
                                objDMSUtil.TemplateCalling(tid, EID, DocumentType, "UNASSIGNEDAGENT", client.mdMailID)
                            End If
                        Catch ex As Exception
                            tran.Rollback()
                            Throw
                        End Try
                    Else
                        Try
                            For Each item As ChildAttachment In childDocumet
                                das.SelectCommand.CommandText = "insert into mmm_mst_doc_item (DOCID,DOCUMENTTYPE,ISAUTH,LASTUPDATE,cmastertid," & item.FieldMapping & ",Attachment) values(" & DocID & ",'" & item.ChildDocumenttype & "',1,getdate(),0,'" & item.FilePath & "','" & item.FileName & "')"
                                If cons.State = ConnectionState.Closed Then
                                    cons.Open()
                                End If
                                das.SelectCommand.ExecuteNonQuery()
                            Next
                            ob.HistoryT(EID, DocID, UID, DocumentType, "MMM_MST_DOC", "ADD", cons, tran)
                            Trigger.ExecuteTriggerT(DocumentType, EID, DocID, cons, tran)
                            DeleteMessageByMessageId(client, messageID)
                            tran.Commit()
                            Dim objDC As New DataClass
                            Dim dt As New DataTable
                            dt = objDC.ExecuteQryDT("select rtrim(ltrim(userrole)) from mmm_mst_user where uid=" & UID & " and eid=" & EID & "")
                            If dt.Rows(0)(0).ToUpper = USERROLE Then
                                objDMSUtil.TemplateCalling(DocID, EID, DocumentType, "USERREPLY", client.mdMailID)
                            Else
                                objDMSUtil.TemplateCalling(DocID, EID, DocumentType, "AGENTREPLY", client.mdMailID)
                            End If
                            'objDMSUtil.TemplateCalling(DocID, EID, DocumentType, "APPROVE")
                        Catch ex As Exception
                            tran.Rollback()
                            Throw
                        End Try
                    End If
                Else
                    tran.Rollback()
                    cons.Dispose()
                End If
            End If
        Catch ex As Exception
            Throw
        End Try
    End Sub
    Public Function CheckAutoNumber(documentType As String, docID As String, ByVal con As SqlConnection, ByVal tran As SqlTransaction, ByVal EID As Integer)
        Dim da As New SqlDataAdapter("select * from mmm_mst_fields where eid=" & EID & " and isActive=1 and (Fieldtype='Auto Number' or Fieldtype='New Auto Number') and documenttype='" & documentType & "'", con)
        'try Catch Block Added by Ajeet 
        Try
            Dim dtDoc As New DataTable
            da.SelectCommand.Transaction = tran
            da.Fill(dtDoc)
            For Each dr As DataRow In dtDoc.Rows
                Select Case dr("fieldtype").ToString().ToUpper
                    Case "AUTO NUMBER"
                        da.SelectCommand.Parameters.Clear()
                        da.SelectCommand.Transaction = tran
                        da.SelectCommand.CommandText = "usp_GetAutoNoNew"
                        da.SelectCommand.CommandType = CommandType.StoredProcedure
                        da.SelectCommand.Parameters.AddWithValue("Fldid", dr("fieldid"))
                        da.SelectCommand.Parameters.AddWithValue("docid", docID)
                        da.SelectCommand.Parameters.AddWithValue("fldmapping", dr("fieldmapping"))
                        da.SelectCommand.Parameters.AddWithValue("FormType", "Document")
                        Dim an As String = da.SelectCommand.ExecuteScalar()
                        da.SelectCommand.Parameters.Clear()
                    Case "NEW AUTO NUMBER"
                        da.SelectCommand.Parameters.Clear()
                        da.SelectCommand.CommandText = "usp_GetAutoNoNew_New"
                        da.SelectCommand.CommandType = CommandType.StoredProcedure
                        da.SelectCommand.Parameters.AddWithValue("Fldid", dr("fieldid"))
                        da.SelectCommand.Parameters.AddWithValue("SearchFldid", dr("dropdown").ToString)
                        da.SelectCommand.Parameters.AddWithValue("docid", docID)
                        da.SelectCommand.Parameters.AddWithValue("fldmapping", dr("fieldmapping"))
                        da.SelectCommand.Parameters.AddWithValue("FormType", "Document")
                        Dim an As String = da.SelectCommand.ExecuteScalar()
                        da.SelectCommand.Parameters.Clear()
                End Select
            Next
        Catch ex As Exception
            Throw
        End Try

    End Function
    Public Function DeleteMessageByMessageId(objPOP As POP, messageId As String) As Boolean
        ' Get the number of messages on the POP3 server
        Dim pop3Client As Pop3Client
        pop3Client = New Pop3Client
        pop3Client.Connect(objPOP.hostName, objPOP.mdport, True)
        pop3Client.Authenticate(objPOP.mdMailID, objPOP.mdPwd)

        Dim messageCount As Integer = pop3Client.GetMessageCount()

        ' Run trough each of these messages and download the headers
        For messageItem As Integer = messageCount To 1 Step -1
            ' If the Message ID of the current message is the same as the parameter given, delete that message
            'Dim TempMessageID As String = client.GetMessageHeaders(messageItem).MessageId
            Dim TempMessageID As String = pop3Client.GetMessageUid(messageItem)
            'If TempMessageID = messageId Then
            If pop3Client.GetMessageHeaders(messageItem).MessageId = messageId Then
                ' Delete
                pop3Client.DeleteMessage(messageItem)
                pop3Client.Disconnect()
                Return True
            End If
        Next

        ' We did not find any message with the given messageId, report this back
        Return False
    End Function
    Public Function GetNextUserFromRolematrixT(ByVal docID As Long, ByVal EID As Long, ByVal CUID As Integer, ByVal qry As String, ByVal Auid As Integer, ByVal con As SqlConnection, ByVal tran As SqlTransaction) As String
        'Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        'Dim con As New SqlConnection(conStr)


        'Dim chkres1 As String
        'Dim chkres2 As String
        'Dim sretMsgArr() As String
        ' '' for retrial of next user  
        'chkres1 = CheckNextUserT(docID, EID, CUID, qry, Auid, con, tran)
        'System.Threading.Thread.Sleep(1500)
        'chkres2 = CheckNextUserT(docID, EID, CUID, qry, Auid, con, tran)

        'If chkres1 <> chkres2 Then
        '    Return "mismatch, try again later"
        '    Exit Function
        'End If


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
                                                'strFldQry &= " and ','+" & dtRRef.Rows(i).Item("docmapping") & " +',' like '%," & dtDR.Rows(0).Item(0).ToString & ",%'"
                                                ' strFldQry &= " and (','+" & dtRRef.Rows(i).Item("docmapping") & " +',' like '%," & dtDR.Rows(0).Item(0).ToString & ",%' or ltrim(" & dtRRef.Rows(i).Item("docmapping") & ")='*' )"
                                                strFldQry &= " and (','+ convert(nvarchar(max), " & dtRRef.Rows(i).Item("docmapping") & ") +',' like '%," & dtDR.Rows(0).Item(0).ToString & ",%' or ltrim( convert(nvarchar(max), " & dtRRef.Rows(i).Item("docmapping") & "))='*' )"
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

                    Dim dtTmp As New DataTable
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
                ElseIf dtRM.Rows(k).Item("type").ToString = "USER" Then
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
    'working Backup
    'Public Function GetNextUserFromRolematrixT(ByVal docID As Long, ByVal EID As Long, ByVal CUID As Integer, ByVal qry As String, ByVal Auid As Integer, ByVal con As SqlConnection, ByVal tran As SqlTransaction) As String
    '    Dim da As New SqlDataAdapter("select d.*, dt.ordering,dt.userid from MMM_MST_DOC D left outer join MMM_DOC_DTL dt on d.LastTID=dt.tid where EID=" & EID & " and d.tid=" & docID, con)
    '    Try
    '        Dim dtDoc As New DataTable
    '        da.SelectCommand.Transaction = tran
    '        da.Fill(dtDoc)
    '        Dim docType As String = dtDoc.Rows(0).Item("documenttype").ToString
    '        Dim CurOrdering As Integer = dtDoc.Rows(0).Item("ordering").ToString
    '        Dim Creator As Integer = dtDoc.Rows(0).Item("ouid").ToString
    '        Dim CurDocNature As String = dtDoc.Rows(0).Item("CurdocNature").ToString
    '        Dim CurrentUser As Integer = dtDoc.Rows(0).Item("userid").ToString
    '        Dim CurStatus As String = dtDoc.Rows(0).Item("CurStatus").ToString

    '        da.SelectCommand.CommandText = "select am.*,wf.isallowskip from MMM_MST_AuthMetrix am inner join MMM_MST_WORKFLOW_STATUS wf on am.aprStatus=wf.StatusName and am.doctype=wf.Documenttype  where am.EID=" & EID & " and am.doctype='" & docType & "' and am.docnature='" & CurDocNature & "' AND am.ordering >" & CurOrdering & " order by am.ordering"

    '        Dim dtRM As New DataTable
    '        da.Fill(dtRM)
    '        Dim FoundUsers As Boolean = False
    '        Dim CurRoleName As String = ""
    '        Dim curAprStatus As String = ""
    '        Dim nxtUser As Integer
    '        Dim sRetMsg As String = ""
    '        Dim AllowSkip As Integer = 0
    '        Dim CheckSkipfeat As Boolean = False
    '        nxtUser = 0 '' intialize with zero 
    '        For k As Integer = 0 To dtRM.Rows.Count - 1  '' K loop till user founds for a role type
    '            'If k = 0 Then
    '            AllowSkip = dtRM.Rows(k).Item("isallowskip").ToString
    '            If AllowSkip = 1 Then
    '                CheckSkipfeat = False
    '            Else
    '                CheckSkipfeat = True
    '            End If
    '            Dim dtTmp As New DataTable
    '            If dtRM.Rows(k).Item("type").ToString = "NEWROLE" Then
    '                CurRoleName = dtRM.Rows(k).Item("Rolename").ToString
    '                curAprStatus = dtRM.Rows(k).Item("aprstatus").ToString
    '                Select Case CurRoleName
    '                    Case "#SELF"


    '                        Dim dtFlds As New DataTable
    '                        da.SelectCommand.CommandText = "select * from MMM_MST_FIELDS where EID=" & EID & " and documenttype='" & docType & "' AND ISWORKFLOW=1"
    '                        da.Fill(dtFlds)

    '                        ' Dim strQry As String = "select top 1 uid from MMM_MST_AuthMetrix A left outer join MMM_MST_WORKFLOW_STATUS S on A.EID=S.EID and S.statusname=a.aprStatus and S.Documenttype=A.doctype where A.EID=" & EID & " and doctype='" & docType & "' and A.ordering>" & CurOrdering
    '                        Dim strQry As String = "select rolename from MMM_MST_AuthMetrix  where  docnature='" & CurDocNature & "' and EID=" & EID & " and doctype='" & docType & "' and ordering=" & dtRM.Rows(k).Item("ordering").ToString  'CurOrdering
    '                        For i As Integer = 0 To dtFlds.Rows.Count - 1
    '                            Dim dType As String = dtFlds.Rows(i).Item("datatype").ToString()
    '                            Dim fldMapping As String = dtFlds.Rows(i).Item("FieldMapping").ToString()
    '                            If dtDoc.Rows(0).Item(fldMapping).ToString().Length > 0 Then
    '                                Select Case dType.ToUpper()
    '                                    Case "TEXT"
    '                                        strQry &= " AND case " & fldMapping & " WHEN '*' Then '" & dtDoc.Rows(0).Item(fldMapping) & "' ELSE " & fldMapping & "  END  like '%" & dtDoc.Rows(0).Item(fldMapping) & "%' "
    '                                    Case "NUMERIC"
    '                                        strQry &= " and convert(numeric(15,0),PARSENAME(REPLACE(" & fldMapping & ", '-', '.'), 2)) < " & dtDoc.Rows(0).Item(fldMapping) & " and convert(numeric(15,0),PARSENAME(REPLACE(" & fldMapping & ", '-', '.'), 1)) > " & dtDoc.Rows(0).Item(fldMapping)
    '                                End Select
    '                            End If
    '                        Next
    '                        strQry &= " order by ordering"
    '                        Dim dtK As New DataTable
    '                        da.SelectCommand.CommandType = CommandType.Text
    '                        da.SelectCommand.Parameters.Clear()
    '                        da.SelectCommand.CommandText = strQry
    '                        da.Fill(dtK)
    '                        If dtK.Rows.Count = 1 Then
    '                            nxtUser = Creator
    '                            ' nxtUser = dtK.Rows(0).Item(0).ToString
    '                        End If

    '                    Case "#CURRENTUSER"

    '                        Dim dtFlds As New DataTable
    '                        da.SelectCommand.CommandText = "select * from MMM_MST_FIELDS where EID=" & EID & " and documenttype='" & docType & "' AND ISWORKFLOW=1"
    '                        da.Fill(dtFlds)

    '                        ' Dim strQry As String = "select top 1 uid from MMM_MST_AuthMetrix A left outer join MMM_MST_WORKFLOW_STATUS S on A.EID=S.EID and S.statusname=a.aprStatus and S.Documenttype=A.doctype where A.EID=" & EID & " and doctype='" & docType & "' and A.ordering>" & CurOrdering
    '                        Dim strQry As String = "select rolename from MMM_MST_AuthMetrix  where docnature='" & CurDocNature & "' and EID=" & EID & " and doctype='" & docType & "' and ordering=" & dtRM.Rows(k).Item("ordering").ToString  'CurOrdering
    '                        For i As Integer = 0 To dtFlds.Rows.Count - 1
    '                            Dim dType As String = dtFlds.Rows(i).Item("datatype").ToString()
    '                            Dim fldMapping As String = dtFlds.Rows(i).Item("FieldMapping").ToString()
    '                            If dtDoc.Rows(0).Item(fldMapping).ToString().Length > 0 Then
    '                                Select Case dType.ToUpper()
    '                                    Case "TEXT"
    '                                        strQry &= " AND case " & fldMapping & " WHEN '*' Then '" & dtDoc.Rows(0).Item(fldMapping) & "' ELSE " & fldMapping & "  END  like '%" & dtDoc.Rows(0).Item(fldMapping) & "%' "
    '                                    Case "NUMERIC"
    '                                        strQry &= " and convert(numeric(15,0),PARSENAME(REPLACE(" & fldMapping & ", '-', '.'), 2)) < " & dtDoc.Rows(0).Item(fldMapping) & " and convert(numeric(15,0),PARSENAME(REPLACE(" & fldMapping & ", '-', '.'), 1)) > " & dtDoc.Rows(0).Item(fldMapping)
    '                                End Select
    '                            End If
    '                        Next
    '                        strQry &= " order by ordering"
    '                        Dim dtK As New DataTable
    '                        da.SelectCommand.CommandType = CommandType.Text
    '                        da.SelectCommand.Parameters.Clear()
    '                        da.SelectCommand.CommandText = strQry
    '                        da.Fill(dtK)
    '                        If dtK.Rows.Count = 1 Then
    '                            nxtUser = CurrentUser
    '                        End If

    '                    Case "#SUPERVISOR"
    '                        da.SelectCommand.CommandType = CommandType.Text
    '                        da.SelectCommand.Parameters.Clear()
    '                        da.SelectCommand.CommandText = "select " & dtRM.Rows(k).Item("fieldName").ToString & " from MMM_MST_user where EID=" & EID & " and uid=" & Creator
    '                        dtTmp.Clear()
    '                        da.Fill(dtTmp)
    '                        If dtTmp.Rows.Count <> 0 Then
    '                            If IsNumeric(dtTmp.Rows(0).Item(0).ToString()) Then
    '                                nxtUser = dtTmp.Rows(0).Item(0).ToString()
    '                            End If
    '                        End If
    '                    Case "#LAST SUPERVISOR"
    '                        Dim slvl As Integer = Val(dtRM.Rows(k).Item("sLevel").ToString)
    '                        Dim LScreator As Integer = Creator
    '                        Dim tmpUser As Integer
    '                        Dim LSfound As Boolean = True
    '                        For m As Integer = 1 To slvl
    '                            da.SelectCommand.CommandType = CommandType.Text
    '                            da.SelectCommand.Parameters.Clear()
    '                            da.SelectCommand.CommandText = "select " & dtRM.Rows(k).Item("fieldName").ToString & " from MMM_MST_user where EID=" & EID & " and uid=" & LScreator
    '                            dtTmp.Clear()
    '                            da.Fill(dtTmp)
    '                            If dtTmp.Rows.Count <> 0 Then
    '                                If IsNumeric(dtTmp.Rows(0).Item(0).ToString) = True Then
    '                                    tmpUser = dtTmp.Rows(0).Item(0).ToString
    '                                Else
    '                                    'nxtUser = tmpUser
    '                                    LSfound = False
    '                                    Exit For
    '                                End If
    '                            End If
    '                            LScreator = tmpUser
    '                        Next
    '                        If LSfound = True Then
    '                            nxtUser = tmpUser
    '                        End If
    '                    Case "#USER"
    '                        da.SelectCommand.CommandType = CommandType.Text
    '                        da.SelectCommand.Parameters.Clear()
    '                        da.SelectCommand.CommandText = "select " & dtRM.Rows(k).Item("fieldName").ToString & " from MMM_MST_Doc where EID=" & EID & " and tid=" & docID
    '                        dtTmp.Clear()

    '                        da.Fill(dtTmp)
    '                        If dtTmp.Rows.Count <> 0 Then
    '                            If IsNumeric(dtTmp.Rows(0).Item(0).ToString) = True Then
    '                                nxtUser = dtTmp.Rows(0).Item(0).ToString
    '                            End If
    '                        End If
    '                    Case Else
    '                        '' any other role 
    '                        '' FOR NEW role with document fields   by sunil 14_july
    '                        '' here on 15_july_14 at 7.57 pm
    '                        da.SelectCommand.CommandType = CommandType.Text
    '                        da.SelectCommand.Parameters.Clear()
    '                        da.SelectCommand.CommandText = "select formid, docmapping, FORMNAME from MMM_MST_FORMS where EID=" & EID & " and isRoleDef=1"
    '                        Dim dtRRef As New DataTable
    '                        da.Fill(dtRRef)
    '                        Dim strMainQry As String = ""
    '                        Dim strFldQry As String = ""
    '                        strMainQry = "select uid,rolename from MMM_Ref_Role_User where Eid=" & EID & " and ',' + documenttype + ','  like '%," & docType & ",%' and rolename='" & CurRoleName & "'"

    '                        For i As Integer = 0 To dtRRef.Rows.Count - 1
    '                            da.SelectCommand.CommandType = CommandType.Text
    '                            da.SelectCommand.Parameters.Clear()
    '                            da.SelectCommand.CommandText = "select fieldmapping from MMM_MST_FIELDS where  Eid=" & EID & " and DocumentType='" & docType & "' and dropdowntype='MASTER VALUED' and substring(dropdown,8,(charindex('-',dropdown,8)-8)) = '" & dtRRef.Rows(i).Item("formname").ToString & "'"
    '                            Dim dtDF As New DataTable
    '                            da.Fill(dtDF)
    '                            If dtDF.Rows.Count > 0 Then   '' write in else of this to add def % in condition. 
    '                                da.SelectCommand.CommandType = CommandType.Text
    '                                da.SelectCommand.Parameters.Clear()
    '                                ''  get fld value from document table 
    '                                da.SelectCommand.CommandText = "select " & dtDF.Rows(0).Item(0).ToString & " from MMM_MST_doc where EID=" & EID & " and  Tid=" & docID
    '                                Dim dtDR As New DataTable
    '                                da.Fill(dtDR)
    '                                If dtDR.Rows.Count <> 0 Then
    '                                    If dtDR.Rows(0).Item(0).ToString <> "" Then
    '                                        If dtDR.Rows(0).Item(0).ToString <> "" Then
    '                                            ' strFldQry &= " and ','+" & dtRRef.Rows(i).Item("docmapping") & " +',' like '%," & dtDR.Rows(0).Item(0).ToString & ",%'"
    '                                            ' strFldQry &= " and (','+" & dtRRef.Rows(i).Item("docmapping") & " +',' like '%," & dtDR.Rows(0).Item(0).ToString & ",%' or ltrim(" & dtRRef.Rows(i).Item("docmapping") & ")='*' )"
    '                                            strFldQry &= " and (','+ convert(nvarchar(max), " & dtRRef.Rows(i).Item("docmapping") & " ) +',' like '%," & dtDR.Rows(0).Item(0).ToString & ",%' or ltrim( convert(nvarchar(max), " & dtRRef.Rows(i).Item("docmapping") & "))='*' )"
    '                                        End If
    '                                    End If
    '                                End If
    '                            End If

    '                            '' ddllookupvalue added  by sunil on 04th October 14 - starts
    '                            da.SelectCommand.CommandType = CommandType.Text
    '                            da.SelectCommand.Parameters.Clear()
    '                            ' prev dis. on 28-sep for optimization by sunil  - DDLlookupValueSource
    '                            da.SelectCommand.CommandText = "select fieldmapping from MMM_MST_FIELDS where  Eid=" & EID & " and DocumentType='" & docType & "' and fieldtype='LookupDDL' and DDLlookupValueSource='" & dtRRef.Rows(i).Item("formname").ToString & "'"
    '                            Dim dtDFddl As New DataTable
    '                            da.Fill(dtDFddl)
    '                            If dtDFddl.Rows.Count > 0 Then   '' write in else of this to add def % in condition. 
    '                                da.SelectCommand.CommandType = CommandType.Text
    '                                da.SelectCommand.Parameters.Clear()
    '                                ''  get fld value from document table 
    '                                da.SelectCommand.CommandText = "select " & dtDFddl.Rows(0).Item(0).ToString & " from MMM_MST_doc where EID=" & EID & " and  Tid=" & docID
    '                                Dim dtDR As New DataTable
    '                                da.Fill(dtDR)
    '                                If dtDR.Rows.Count <> 0 Then
    '                                    If dtDR.Rows(0).Item(0).ToString <> "" Then
    '                                        If dtDR.Rows(0).Item(0).ToString <> "" Then
    '                                            ' strFldQry &= " and ','+" & dtRRef.Rows(i).Item("docmapping") & " +',' like '%," & dtDR.Rows(0).Item(0).ToString & ",%'"
    '                                            strFldQry &= " and (','+" & dtRRef.Rows(i).Item("docmapping") & " +',' like '%," & dtDR.Rows(0).Item(0).ToString & ",%' or ltrim(" & dtRRef.Rows(i).Item("docmapping") & ")='*' )"
    '                                        End If
    '                                    End If
    '                                End If
    '                            End If
    '                            '' ddllookupvalue added  by sunil on 04th October 14 - ends
    '                        Next
    '                        If Len(strFldQry) > 1 Then
    '                            da.SelectCommand.CommandType = CommandType.Text
    '                            da.SelectCommand.Parameters.Clear()
    '                            ''  get final rows from role assignment table 
    '                            da.SelectCommand.CommandText = strMainQry & strFldQry
    '                            Dim dtRU As New DataTable
    '                            da.Fill(dtRU)
    '                            Dim usrs As String = ""
    '                            If dtRU.Rows.Count = 1 Then
    '                                nxtUser = dtRU.Rows(0).Item(0).ToString
    '                            ElseIf dtRU.Rows.Count > 1 Then
    '                                '' new for queueing issue 01-April-14
    '                                Dim mindoc As Integer = 999999
    '                                Dim MinUserID As Integer = dtRU.Rows(0).Item(0).ToString

    '                                For H As Integer = 0 To dtRU.Rows.Count - 1
    '                                    usrs = dtRU.Rows(H).Item(0).ToString()
    '                                    If Val(usrs) <> 0 Then
    '                                        da.SelectCommand.CommandType = CommandType.Text
    '                                        da.SelectCommand.CommandText = "select COUNT(userid) [loadcount] from MMM_DOC_DTL dt  with (nolock) left outer join MMM_MST_DOC D  with (nolock) on dt.tid=d.lasttid where d.EID=" & EID & " and d.DocumentType='" & docType & "' and dt.userid in (" & usrs & ")  and d.curstatus= '" & curAprStatus & "' and dt.tdate is null and dt.aprstatus is null  group by userid "
    '                                        da.SelectCommand.Parameters.Clear()
    '                                        If con.State <> ConnectionState.Open Then
    '                                            con.Open()
    '                                        End If
    '                                        Dim res As Integer = Val(Convert.ToString(da.SelectCommand.ExecuteScalar()))
    '                                        If Val(res) < mindoc Then
    '                                            mindoc = res
    '                                            MinUserID = usrs
    '                                        End If
    '                                    End If
    '                                Next
    '                                nxtUser = MinUserID
    '                            End If

    '                            ''for gen. query of document flds in role based user
    '                            Dim dtFlds As New DataTable
    '                            da.SelectCommand.CommandText = "select * from MMM_MST_FIELDS where EID=" & EID & " and documenttype='" & docType & "' AND ISWORKFLOW=1"
    '                            da.Fill(dtFlds)

    '                            Dim strQry As String = "select rolename from MMM_MST_AuthMetrix  where docnature='" & CurDocNature & "' and EID=" & EID & " and doctype='" & docType & "' and ordering=" & dtRM.Rows(k).Item("ordering").ToString  'CurOrdering
    '                            For i As Integer = 0 To dtFlds.Rows.Count - 1
    '                                Dim dType As String = dtFlds.Rows(i).Item("datatype").ToString()
    '                                Dim fldMapping As String = dtFlds.Rows(i).Item("FieldMapping").ToString()
    '                                If dtDoc.Rows(0).Item(fldMapping).ToString().Length > 0 Then
    '                                    Select Case dType.ToUpper()
    '                                        Case "TEXT"
    '                                            strQry &= " AND case " & fldMapping & " WHEN '*' Then '" & dtDoc.Rows(0).Item(fldMapping) & "' ELSE " & fldMapping & "  END  like '%" & dtDoc.Rows(0).Item(fldMapping) & "%' "
    '                                        Case "NUMERIC"
    '                                            strQry &= " and convert(numeric(15,0),PARSENAME(REPLACE(" & fldMapping & ", '-', '.'), 2)) < " & dtDoc.Rows(0).Item(fldMapping) & " and convert(numeric(15,0),PARSENAME(REPLACE(" & fldMapping & ", '-', '.'), 1)) > " & dtDoc.Rows(0).Item(fldMapping)
    '                                    End Select
    '                                End If
    '                            Next
    '                            strQry &= " order by ordering"
    '                            Dim dtK As New DataTable
    '                            da.SelectCommand.CommandType = CommandType.Text
    '                            da.SelectCommand.Parameters.Clear()
    '                            da.SelectCommand.CommandText = strQry
    '                            da.Fill(dtK)

    '                            If dtK.Rows.Count > 0 And dtRU.Rows.Count > 0 Then
    '                                If dtK.Rows(0).Item("rolename").ToString.Trim().ToUpper() = dtRU.Rows(0).Item("rolename").ToString.Trim().ToUpper() Then
    '                                    If dtRU.Rows.Count > 1 Then
    '                                        '''' NEXT USER WILL BE FROM QUEUING WHICH HAVE MINIMUM DOCUMENT  (MINUSERID)
    '                                    Else
    '                                        nxtUser = dtRU.Rows(0).Item("uid").ToString ' IF ONLY ONE USER EXIST  
    '                                    End If
    '                                End If
    '                            Else
    '                                nxtUser = 0
    '                            End If
    '                            ''for gen. query of document flds in role based user
    '                        End If
    '                End Select
    '            End If
    '            If (CheckSkipfeat = True) And (nxtUser = 0) Then
    '                '' exit from func with bcoz skip is not allowed and user not found
    '                sRetMsg = "NO SKIP" & ":" & docType
    '                CheckSkipfeat = True
    '                Exit For
    '            Else
    '                If nxtUser <> 0 Then
    '                    If CurStatus = dtRM.Rows(k).Item("aprstatus").ToString() Then
    '                        sRetMsg = "Current and Next Status cannot be same"
    '                        Exit For
    '                    End If
    '                    da.SelectCommand.CommandText = "ApproveWorkFlow_RM_with_Isauth_2"
    '                    da.SelectCommand.CommandType = CommandType.StoredProcedure
    '                    da.SelectCommand.Parameters.Clear()
    '                    da.SelectCommand.Parameters.AddWithValue("tid", docID)
    '                    da.SelectCommand.Parameters.AddWithValue("nUid", nxtUser)
    '                    da.SelectCommand.Parameters.AddWithValue("NxtStatus", dtRM.Rows(k).Item("aprstatus").ToString)
    '                    da.SelectCommand.Parameters.AddWithValue("nOrder", dtRM.Rows(k).Item("ordering").ToString)
    '                    da.SelectCommand.Parameters.AddWithValue("nSLA", dtRM.Rows(k).Item("SLA").ToString)
    '                    If Len(qry) > 1 Then
    '                        da.SelectCommand.Parameters.AddWithValue("qry", qry)
    '                    End If
    '                    If Auid <> 0 Then
    '                        da.SelectCommand.Parameters.AddWithValue("auid", Auid)
    '                    End If

    '                    Dim dtt As New DataTable
    '                    da.Fill(dtt)

    '                    sRetMsg = dtt.Rows(0).Item(0).ToString()
    '                    'If sRetMsg = "User not authorised" Then
    '                    'Return "You are not authorised to approve this document"
    '                    Exit For
    '                    ' End If
    '                End If
    '            End If
    '            If CheckSkipfeat = True Then
    '                'Commentted by ajeet for managing transactions
    '                'dtDoc.Dispose() : da.Dispose() : con.Close() : con.Dispose()
    '                dtDoc.Dispose() : da.Dispose()
    '                Return sRetMsg
    '            End If

    '            If nxtUser <> 0 Then
    '                'dtDoc.Dispose() : da.Dispose() : con.Close() : con.Dispose()
    '                dtDoc.Dispose() : da.Dispose()
    '                Return sRetMsg
    '            Else
    '                'Return "NO USERS IN AUTHMATRIX"
    '                'da.SelectCommand.CommandText = "InsertDefaultMovement_with_Isauth_2"
    '                'da.SelectCommand.CommandType = CommandType.StoredProcedure
    '                'da.SelectCommand.Parameters.Clear()
    '                'da.SelectCommand.Parameters.AddWithValue("tid", docID)
    '                'da.SelectCommand.Parameters.AddWithValue("what", "ARCHIVE")
    '                'da.SelectCommand.Parameters.AddWithValue("qry", qry)
    '                'If con.State <> ConnectionState.Open Then
    '                '    con.Open()
    '                'End If
    '                'da.SelectCommand.ExecuteNonQuery()
    '                dtDoc.Dispose() : da.Dispose()
    '                'Return "ARCHIVE:" & docType
    '                Return ""
    '            End IfSavingMailDoc
    '            dtDoc.Dispose()
    '        Next
    '    Catch ex As Exception

    '    End Try


    'End Function
    Public Function GetNextUserFromOrganizatios(ByVal From As String, ByVal EID As Integer, ByVal ADMINROLE As String, Optional ByVal ResultMaster As Integer = 0) As String
        Dim result As String = ""
        Dim Name As String = String.Empty
        Dim Domains As String = String.Empty
        Dim Group As String = String.Empty
        Dim Agents As String = String.Empty
        Dim Category As String = String.Empty
        Dim SubCategory As String = String.Empty
        Dim Domain As String() = From.Split("@")
        Dim dtOrganization As New DataTable()
        Dim objDC As New DataClass
        dtOrganization = objDC.ExecuteQryDT("select MDfieldName,fieldMapping from mmm_mst_fields  with(nolock) where documenttype in('Organizations','Groups','Department','Sub Department','subDeptTkt') and eid=" & EID & " and MDfieldName is not null")
        'dtOrganization = objDC.ExecuteQryDT("select MDfieldName,fieldMapping from mmm_mst_fields where documenttype in('Organizations','Groups') and eid=" & EID & " and MDfieldName is not null")
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
                    Case "SubCategory"
                        SubCategory = dr("fieldMapping")
                    Case "Category"
                        Category = dr("fieldMapping")
                End Select
            Next

            'If Not (String.IsNullOrEmpty(Name) And String.IsNullOrEmpty(Group) And String.IsNullOrEmpty(Domains) And String.IsNullOrEmpty(Agents)) Then
            '    result = objDC.ExecuteQryScaller("select " & Agents & " from mmm_mst_master where eid=" & EID & " and documenttype='Groups' and tid in (select " & Group & " from mmm_mst_master where eid=" & EID & " and documenttype='Organizations' and  ','+" & Domains & "+',' like '%," & Domain(1).Trim() & ",%' )")
            'Else
            '    result = "Mapping Not Found against Name,Group,Domain,Agents"
            'End If
            'Return result
            If Not (String.IsNullOrEmpty(Name) And String.IsNullOrEmpty(Group) And String.IsNullOrEmpty(Domains) And String.IsNullOrEmpty(Agents)) Then
                'mail should be go for perticular group based on subcategory old backup below
                'result = objDC.ExecuteQryScaller("select " & Agents & " from mmm_mst_master  with(nolock) where eid=" & EID & " and documenttype='Groups' and tid in (select " & Group & " from mmm_mst_master  with(nolock) where eid=" & EID & " and documenttype='Organizations' and  ','+" & Domains & "+',' like '%," & Domain(1).Trim() & ",%' and ','+" & SubCategory & "+',' like '%," & ResultMaster & ",%'  )")
                'commented below line on 15 feb 2018
                'result = objDC.ExecuteQryScaller("if(exists(select tid from mmm_mst_master  with(nolock) where eid=" & EID & " and documenttype='Organizations' and  ','+" & Domains & "+',' like '%," & Domain(1).Trim() & ",%' )) begin select " & Agents & " from mmm_mst_master  with(nolock) where eid=" & EID & " and documenttype='Groups' and tid in ( select " & Group & " from mmm_mst_master where tid=" & ResultMaster & ") end else begin select '' end")
                result = objDC.ExecuteQryScaller("if exists(select * from mmm_mst_master where eid=" & EID & " and documenttype='Organizations' and  ','+" & Domains & "+',' like '%," & Domain(1).Trim() & ",%'  and ','+" & SubCategory & "+',' like '%," & ResultMaster & ",%')  begin 			Select  " & Agents & " from mmm_mst_master where documenttype='Groups' and eid=" & EID & " and tid in( select " & Group & " from mmm_mst_master where documenttype='Sub department' and eid=" & EID & " and tid in  (" & ResultMaster & "))  end   else   begin  select ''  end ")

                If IsNothing(result) Then
                    result = objDC.ExecuteQryScaller("if exists(select * from mmm_mst_master where eid=" & EID & " and documenttype='Organizations' and  ','+" & Domains & "+',' like '%," & Domain(1).Trim() & ",%'  and ','+" & SubCategory & "+',' like '%," & ResultMaster & ",%')  begin 			Select  " & Agents & " from mmm_mst_master where documenttype='Groups' and eid=" & EID & " and tid in( select " & Group & " from mmm_mst_master where documenttype='SubDeptTkt' and eid=" & EID & " and tid in  (" & ResultMaster & "))  end   else   begin  select ''  end ")
                End If

                'result = objDC.ExecuteQryScaller("select " & Agents & " from mmm_mst_master  with(nolock) where eid=" & EID & " And documenttype='Sub department' and tid in(select " & SubCategory & " from mmm_mst_master where eid=" & EID & " and documenttype='Organizations' where   ','+" & Domains & "+',' like '%," & Domain(1).Trim() & ",%' and ','+" & SubCategory & "+',' like '%," & ResultMaster & ",%'  )")

                'Check if Domain is not register but subcategroy match with our database.
                'If IsNothing(result) Then
                '    result = objDC.ExecuteQryScaller("select " & Agents & " from mmm_mst_master  with(nolock) where eid=" & EID & " and documenttype='Groups' and tid in (select " & Group & " from mmm_mst_master  with(nolock) where eid=" & EID & " and documenttype='Organizations'  and ','+" & SubCategory & "+',' like '%," & ResultMaster & ",%'  )")
                'End If
                'Check if Domain is not register but subcategroy match with our database.
                If IsNothing(result) Then
                    Dim Tempresult As String = ""
                    Tempresult = objDC.ExecuteQryScaller("select  count(*)  from mmm_mst_master  with(nolock) where eid=" & EID & " and documenttype='Groups' and tid in (select " & Group & " from mmm_mst_master  with(nolock) where eid=" & EID & " and documenttype='Organizations' and  ','+" & Domains & "+',' like '%," & Domain(1).Trim() & ",%' )")
                    If Val(Tempresult) > 0 Then
                        Resultvalue = objDC.ExecuteQryScaller("select STUFF(( SELECT  ',' + cast( uid  as nvarchar)FROM    mmm_mst_user with (nolock)  WHERE   eid=" & EID & " and userrole='Agent' FOR XML PATH('') ), 1, 1, '') ")
                    Else
                        Resultvalue = ""
                    End If
                    result = objDC.ExecuteQryScaller("select uid from mmm_mst_user  with(nolock) where userrole='" & ADMINROLE & "' and eid=" & EID & " order by uid")
                End If
            Else
                result = "Mapping Not Found against Name,Group,Domain,Agents"
            End If
        End If
        Return result
    End Function
    Public Function GetNextUserFromOrganizatios(ByVal con As SqlConnection, ByVal tran As SqlTransaction, ByVal From As String, ByVal EID As Integer, ByVal ADMINROLE As String, Optional ByVal ResultMaster As Integer = 0) As String
        Dim result As String = ""
        Dim Name As String = String.Empty
        Dim Domains As String = String.Empty
        Dim Group As String = String.Empty
        Dim Agents As String = String.Empty
        Dim Category As String = String.Empty
        Dim SubCategory As String = String.Empty
        Dim Domain As String() = From.Split("@")
        Dim das As SqlDataAdapter = New SqlDataAdapter("", con)
        das.SelectCommand.CommandText = "select MDfieldName,fieldMapping from mmm_mst_fields  with(nolock) where documenttype in('Organizations','Groups','Department','Sub Department','subDeptTkt') and eid=" & EID & " and MDfieldName is not null"
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
                    Case "SubCategory"
                        SubCategory = dr("fieldMapping")
                    Case "Category"
                        Category = dr("fieldMapping")
                End Select
            Next
            'Name = dtOrganization.Rows(0)("Name")
            'Group = dtOrganization.Rows(0)("Group")
            'Domains = dtOrganization.Rows(0)("Domains")
            'Agents = dtOrganization.Rows(0)("Agents")
            If Not (String.IsNullOrEmpty(Name) And String.IsNullOrEmpty(Group) And String.IsNullOrEmpty(Domains) And String.IsNullOrEmpty(Agents)) Then
                das.SelectCommand.CommandText = "if exists(select * from mmm_mst_master where eid=" & EID & " and documenttype='Organizations' and  ','+" & Domains & "+',' like '%," & Domain(1).Trim() & ",%'  and ','+" & SubCategory & "+',' like '%," & ResultMaster & ",%')  begin 			Select  " & Agents & " from mmm_mst_master where documenttype='Groups' and eid=" & EID & " and tid in( select " & Group & " from mmm_mst_master where documenttype='Sub department' and eid=" & EID & " and tid in  (" & ResultMaster & "))  end   else   begin  select ''  end "
                If con.State = ConnectionState.Closed Then
                    con.Open()
                End If
                result = das.SelectCommand.ExecuteScalar()
                'Check if Domain is not register but subcategroy match with our database.
                If IsNothing(result) Then
                    das.SelectCommand.CommandText = "Select " & Agents & " from mmm_mst_master  With(nolock) where eid=" & EID & " And documenttype='Groups' and tid in (select " & Group & " from mmm_mst_master  with(nolock) where eid=" & EID & " and documenttype='Organizations'  and ','+" & SubCategory & "+',' like '%," & ResultMaster & ",%'  )"
                    If con.State = ConnectionState.Closed Then
                        con.Open()
                    End If
                    result = das.SelectCommand.ExecuteScalar()
                End If
                'Check if Domain is not register but subcategroy match with our database.
                'Change the condition for dcm shri ram it will return only those agents which exist in database based on domain based if mulitple account exist with single admin
                If IsNothing(result) Then
                    das.SelectCommand.CommandText = "select  " & Agents & "  from mmm_mst_master  with(nolock) where eid=" & EID & " and documenttype='Groups' and tid in (select " & Group & " from mmm_mst_master  with(nolock) where eid=" & EID & " and documenttype='Organizations' and  ','+" & Domains & "+',' like '%," & Domain(1).Trim() & ",%' )"
                    If con.State = ConnectionState.Closed Then
                        con.Open()
                    End If
                    Dim dtagents As New DataTable()
                    das.Fill(dtagents)
                    If dtagents.Rows.Count > 0 Then
                        'das.SelectCommand.CommandText = "select STUFF(( SELECT  ',' + cast( uid  as nvarchar)FROM    mmm_mst_user with (nolock)  WHERE   eid=" & EID & " and userrole='Agent' FOR XML PATH('') ), 1, 1, '') "
                        'Changes for UserRole
                        das.SelectCommand.CommandText = "declare @col nvarchar(max)='' select @col=AGENTROLE from mmm_hdmail_schdule where eid=" & EID & " ;with A (USERROLE) as (  select * from DMS.SplitString(@col,',') )select STUFF(( SELECT  ',' + cast( uid  as nvarchar)FROM    mmm_mst_user as b with (nolock),A    WHERE   eid=" & EID & " and b.userrole in (A.Userrole) and isauth=1 and b.uid in (" & dtagents.Rows(0)(0) & ") FOR XML PATH('') ), 1, 1, '') "
                        If con.State = ConnectionState.Closed Then
                            con.Open()
                        End If
                        Resultvalue = das.SelectCommand.ExecuteScalar()
                    Else
                        Resultvalue = ""
                    End If
                    'das.SelectCommand.CommandText = "select uid from mmm_mst_user  with(nolock) where userrole='" & ADMINROLE & "' and eid=" & EID & " order by uid"
                    'Changes for UserRole
                    das.SelectCommand.CommandText = "select uid from mmm_mst_user  with(nolock) where userrole in (select  adminrole from mmm_hdmail_schdule where eid=" & EID & ") and eid=" & EID & " order by uid"
                    If con.State = ConnectionState.Closed Then
                        con.Open()
                    End If
                    result = das.SelectCommand.ExecuteScalar()
                End If
            Else
                result = "Mapping Not Found against Name,Group,Domain,Agents"
            End If
            Return result
        End If
    End Function
    Public Function AssignTicketToUserBasedOnCondition(result As String, docID As Integer, ByVal qry As String, ByVal Auid As Integer, ByVal TicketAssignMethod As String, Optional ByVal DocumentType As String = "TICKET", Optional ByVal EID As Integer = 0) As String
        Dim Res As String = ""
        Dim uids As String() = result.ToString().Split(",")
        Dim objDC As New DataClass
        Try
            If uids.Length > 1 Then
                If Convert.ToString(TicketAssignMethod).ToUpper = "QUEUEING" Then
                    Dim mindoc As Integer = 999999
                    Dim MinUserID As Integer = uids(0)
                    For i As Integer = 0 To uids.Length - 1
                        Dim resval As Integer = Val(objDC.ExecuteQryScaller("select COUNT(userid) [loadcount] from MMM_DOC_DTL  dt  with (nolock) left outer join MMM_MST_DOC D  with (nolock) on dt.tid=d.lasttid where d.EID=" & EID & " and d.DocumentType='" & DocumentType & "' and dt.userid in (" & uids(i) & ")  and d.ticketstatus in ('OPEN','PENDING') and dt.tdate is null and dt.aprstatus is null  group by userid "))
                        If Val(resval) < mindoc Then
                            mindoc = resval
                            MinUserID = uids(i)
                        End If
                    Next
                    objDC.ExecuteQryDT("insert into mmm_doc_dtl (userid,docid,fdate,ptat,atat,pathID,Ordering,DocNature,lastupdate) values(" & MinUserID & "," & docID & ",getdate(),0,0,0,1,'CREATE',getdate()) declare @lastTID int Select @lastTID=@@IDENTITY  update mmm_mst_doc set lasttid= @lastTID,curstatus='OPEN' where tid= " & docID & "")
                Else
                    Dim arrColumnValues As New ArrayList
                    For i As Integer = 0 To uids.Length - 1
                        arrColumnValues.Add("(" & docID & "," & uids(i) & ",getdate(),null,null)")
                    Next
                    objDC.ExecuteQryDT("insert into mmm_doc_view(DocID,UserId,fdate,aprstatus,tdate) values" & String.Join(",", arrColumnValues.ToArray()) & "update mmm_mst_doc set curstatus='OPEN' where tid= " & docID & "")
                End If

            Else
                If IsNumeric(uids) Then
                    Dim dtDoc As New DataTable
                    dtDoc = (objDC.ExecuteQryDT("select d.*, dt.ordering,dt.userid from MMM_MST_DOC D  with(nolock) left outer join MMM_DOC_DTL dt  with(nolock) on d.LastTID=dt.tid where EID=" & HttpContext.Current.Session("EID") & " and d.tid=" & docID))
                    Try
                        Dim docType As String = dtDoc.Rows(0).Item("documenttype").ToString
                        Dim CurOrdering As Integer = dtDoc.Rows(0).Item("ordering").ToString
                        Dim Creator As Integer = dtDoc.Rows(0).Item("ouid").ToString
                        Dim CurDocNature As String = dtDoc.Rows(0).Item("CurdocNature").ToString
                        Dim CurrentUser As Integer = dtDoc.Rows(0).Item("userid").ToString
                        Dim CurStatus As String = dtDoc.Rows(0).Item("CurStatus").ToString
                        Dim dtRM As New DataTable
                        dtRM = objDC.ExecuteQryDT("select am.*,wf.isallowskip from MMM_MST_AuthMetrix am inner join MMM_MST_WORKFLOW_STATUS wf on am.aprStatus=wf.StatusName and am.doctype=wf.Documenttype  where am.EID=" & HttpContext.Current.Session("EID") & " and am.doctype='" & docType & "' and am.docnature='" & CurDocNature & "' AND am.ordering >" & CurOrdering & " order by am.ordering")
                        Dim FoundUsers As Boolean = False
                        Dim CurRoleName As String = ""
                        Dim curAprStatus As String = ""
                        Dim nxtUser As Integer
                        Dim sRetMsg As String = ""
                        Dim AllowSkip As Integer = 0
                        Dim CheckSkipfeat As Boolean = False
                        nxtUser = 0 '' intialize with zero 
                        For k As Integer = 0 To dtRM.Rows.Count - 1  '' K loop till user founds for a role type
                            Dim ht As New Hashtable
                            Dim dtt As New DataTable
                            ht.Add("@tid", docID)
                            ht.Add("@nUid", nxtUser)
                            ht.Add("@NxtStatus", dtRM.Rows(k).Item("aprstatus").ToString)
                            ht.Add("@nOrder", dtRM.Rows(k).Item("ordering").ToString)
                            ht.Add("@nSLA", dtRM.Rows(k).Item("SLA").ToString)
                            ht.Add("@qry", qry)
                            ht.Add("@auid", Auid)
                            dtt = objDC.ExecuteProDT("ApproveWorkFlow_RM_with_Isauth_2", ht)
                            'das.SelectCommand.CommandText = ""
                            'das.SelectCommand.CommandType = CommandType.StoredProcedure
                            'das.SelectCommand.Parameters.Clear()
                            'das.SelectCommand.Parameters.AddWithValue("tid", docID)
                            'das.SelectCommand.Parameters.AddWithValue("nUid", nxtUser)
                            'das.SelectCommand.Parameters.AddWithValue("NxtStatus", dtRM.Rows(k).Item("aprstatus").ToString)
                            'das.SelectCommand.Parameters.AddWithValue("nOrder", dtRM.Rows(k).Item("ordering").ToString)
                            'das.SelectCommand.Parameters.AddWithValue("nSLA", dtRM.Rows(k).Item("SLA").ToString)
                            'If Len(qry) > 1 Then
                            '    das.SelectCommand.Parameters.AddWithValue("qry", qry)
                            '    das.SelectCommand.Parameters.AddWithValue("qry", qry)
                            'End If
                            'If Auid <> 0 Then
                            '    das.SelectCommand.Parameters.AddWithValue("auid", Auid)
                            'End If
                            'das.Fill(dtt)

                            Return dtt.Rows(0).Item(0).ToString()
                        Next
                    Catch ex As Exception
                        Throw
                    End Try
                Else

                End If
            End If
        Catch ex As Exception
            Throw
        End Try
        Return Res
    End Function

    Public Function AssignTicketToUserBasedOnCondition(result As String, docID As Integer, ByVal qry As String, ByVal Auid As Integer, ByVal con As SqlConnection, ByVal tran As SqlTransaction, ByVal EID As Integer, Optional RoleMatrixFromOrganization As Boolean = False, Optional ByVal TicketAssignMethod As String = "PULL", Optional ByVal DocumentType As String = "TICKET", Optional ByVal OperationType As String = "UPDATE") As String
        Dim Res As String = ""
        Dim uids As String()
        uids = result.ToString().Split(",")
        Dim das As SqlDataAdapter = New SqlDataAdapter("", con)
        das.SelectCommand.Transaction = tran
        Try
            If uids.Length > 1 Then
                If Convert.ToString(TicketAssignMethod).ToUpper = "QUEUEING" Then
                    Dim mindoc As Integer = 999999
                    Dim MinUserID As Integer = uids(0)
                    For i As Integer = 0 To uids.Length - 1
                        das.SelectCommand.CommandText = "select COUNT(userid) [loadcount] from MMM_DOC_DTL dt  with (nolock) left outer join MMM_MST_DOC D  with (nolock) on dt.tid=d.lasttid where d.EID=" & EID & " and d.DocumentType='" & DocumentType & "' and dt.userid in (" & uids(i) & ")  and d.ticketstatus in ('OPEN','PENDING') and dt.tdate is null and dt.aprstatus is null  group by userid "
                        das.SelectCommand.Parameters.Clear()
                        If con.State <> ConnectionState.Open Then
                            con.Open()
                        End If
                        Dim resval As Integer = Val(Convert.ToString(das.SelectCommand.ExecuteScalar()))
                        If Val(resval) < mindoc Then
                            mindoc = resval
                            MinUserID = uids(i)
                        End If
                    Next
                    das.SelectCommand.CommandText = "insert into mmm_doc_dtl (userid,docid,fdate,ptat,atat,pathID,Ordering,DocNature,lastupdate) values(" & MinUserID & "," & docID & ",getdate(),0,0,0,1,'CREATE',getdate()) declare @lastTID int Select @lastTID=@@IDENTITY  update mmm_mst_doc set lasttid= @lastTID ,curstatus='OPEN',ticketstatus='OPEN' where tid= " & docID
                    If con.State = ConnectionState.Closed Then
                        con.Open()
                    End If
                    das.SelectCommand.ExecuteNonQuery()
                Else
                    Dim arrColumnValues As New ArrayList
                    For i As Integer = 0 To uids.Length - 1
                        arrColumnValues.Add("(" & docID & "," & uids(i) & ",getdate(),null,null)")
                    Next
                    das.SelectCommand.CommandText = "insert into mmm_doc_view(DocID,UserId,fdate,aprstatus,tdate) values" & String.Join(",", arrColumnValues.ToArray()) & "   update mmm_mst_doc set curstatus='OPEN',ticketstatus='OPEN' where tid= " & docID
                    If con.State = ConnectionState.Closed Then
                        con.Open()
                    End If
                    das.SelectCommand.ExecuteNonQuery()

                End If

            Else
                If IsNumeric(uids(0)) Then
                    If RoleMatrixFromOrganization Then
                        If OperationType = "CREATE" Then
                            'das.SelectCommand.CommandText = "select  count(*) from mmm_hdmail_schdule as a inner join mmm_mst_user as b on a.eid=b.eid and a.adminrole=b.userrole where a.eid=" & EID & " and uid=" & uids(0)
                            das.SelectCommand.CommandText = "select  count(*) from mmm_hdmail_schdule as a inner join mmm_mst_user as b on a.eid=b.eid  where a.eid=" & EID & "  and ','+a.adminrole+',' like '%,'+b.userrole+',%' and uid=" & uids(0)
                            If con.State = ConnectionState.Closed Then
                                con.Open()
                            End If
                            If Convert.ToInt32(das.SelectCommand.ExecuteScalar()) > 0 Then
                                If Not IsNothing(Resultvalue) Then
                                    If Resultvalue.Length > 1 Then
                                        Dim docUid As String() = Resultvalue.ToString().Split(",")
                                        If docUid.Length > 1 Then
                                            Dim arrColumnValues As New ArrayList
                                            For i As Integer = 0 To docUid.Length - 1
                                                arrColumnValues.Add("(" & docID & "," & docUid(i) & ",getdate(),null,null)")
                                            Next
                                            das.SelectCommand.CommandText = "insert into mmm_doc_view(DocID,UserId,fdate,aprstatus,tdate) values" & String.Join(",", arrColumnValues.ToArray()) & "   update mmm_mst_doc set curstatus='OPEN',ticketstatus='OPEN',fld9='OPEN' where tid= " & docID
                                            If con.State = ConnectionState.Closed Then
                                                con.Open()
                                            End If
                                            das.SelectCommand.ExecuteNonQuery()
                                        Else
                                            das.SelectCommand.CommandText = "insert into mmm_doc_dtl (userid,docid,fdate,pathID,Ordering,DocNature,lastupdate) values(" & docUid(0) & "," & docID & ",getdate(),0,1,'CREATE',getdate()) declare @lastTID int Select @lastTID=@@IDENTITY  update mmm_mst_doc set lasttid= @lastTID,ticketstatus='OPEN',curstatus='OPEN',fld9='OPEN' where tid= " & docID
                                            das.SelectCommand.ExecuteNonQuery()
                                        End If

                                    Else
                                        das.SelectCommand.CommandText = "insert into mmm_doc_dtl (userid,docid,fdate,pathID,Ordering,DocNature,lastupdate) values(" & uids(0) & "," & docID & ",getdate(),0,1,'CREATE',getdate()) declare @lastTID int Select @lastTID=@@IDENTITY  update mmm_mst_doc set lasttid= @lastTID,ticketstatus='SUSPENDED',curstatus='OPEN',fld9='SUSPENDED' where tid= " & docID
                                        das.SelectCommand.ExecuteNonQuery()
                                    End If
                                Else

                                End If
                            Else
                                    das.SelectCommand.CommandText = "insert into mmm_doc_dtl (userid,docid,fdate,pathID,Ordering,DocNature,lastupdate) values(" & uids(0) & "," & docID & ",getdate(),0,1,'CREATE',getdate()) declare @lastTID int Select @lastTID=@@IDENTITY  update mmm_mst_doc set lasttid= @lastTID,ticketstatus='OPEN',curstatus='OPEN' where tid= " & docID
                                das.SelectCommand.ExecuteNonQuery()
                            End If
                        Else
                            das.SelectCommand.CommandText = "insert into mmm_doc_dtl (userid,docid,fdate,pathID,Ordering,DocNature,lastupdate) values(" & uids(0) & "," & docID & ",getdate(),0,1,'CREATE',getdate()) declare @lastTID int Select @lastTID=@@IDENTITY  update mmm_mst_doc set lasttid= @lastTID,ticketstatus='OPEN',curstatus='OPEN' where tid= " & docID
                            das.SelectCommand.ExecuteNonQuery()
                        End If
                    Else
                        das.SelectCommand.CommandText = "select d.*, dt.ordering,dt.userid from MMM_MST_DOC D left outer join MMM_DOC_DTL dt on d.LastTID=dt.tid where EID=" & EID & " and d.tid=" & docID
                        Try
                            Dim dtDoc As New DataTable
                            das.Fill(dtDoc)
                            Dim docType As String = dtDoc.Rows(0).Item("documenttype").ToString
                            Dim CurOrdering As Integer = dtDoc.Rows(0).Item("ordering").ToString
                            Dim Creator As Integer = dtDoc.Rows(0).Item("ouid").ToString
                            Dim CurDocNature As String = dtDoc.Rows(0).Item("CurdocNature").ToString
                            Dim CurrentUser As Integer = dtDoc.Rows(0).Item("userid").ToString
                            Dim CurStatus As String = dtDoc.Rows(0).Item("CurStatus").ToString

                            das.SelectCommand.CommandText = "select am.*,wf.isallowskip from MMM_MST_AuthMetrix am inner join MMM_MST_WORKFLOW_STATUS wf on am.aprStatus=wf.StatusName and am.doctype=wf.Documenttype  where am.EID=" & EID & " and am.doctype='" & docType & "' and am.docnature='" & CurDocNature & "' AND am.ordering >" & CurOrdering & " order by am.ordering"

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
                                das.SelectCommand.Parameters.AddWithValue("nUid", uids(0))
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
                            Throw
                        End Try
                    End If

                Else

                End If
            End If
        Catch ex As Exception
            Throw
        End Try
        Return Res
    End Function
    Public Function ArchiveToReOpenDocument(DOCID As Integer) As Boolean
        Dim ret As Boolean = False

        Return ret
    End Function
    <Serializable()>
    Public Class EmailAttachment
        Public Sub New()
            MyBase.New()
            Me.ChildAttachments = New List(Of ChildAttachment)
        End Sub
        Public Property ChildAttachments As List(Of ChildAttachment)
    End Class
    <Serializable()>
    Public Class Email
        Public Sub New()
            MyBase.New()
            Me.Attachments = New List(Of Attachment)
        End Sub
        Public Property MessageID As String
        Public Property MessageNumber As Integer
        Public Property From As String
        Public Property CC As List(Of RfcMailAddress)
        Public Property BCC As List(Of RfcMailAddress)
        Public Property DisplayName As String
        Public Property Subject As String
        Public Property Body As String
        Public Property EID As Integer
        Public Property DocumentType As String
        Public Property IsAllowCreateUser As Boolean
        Public Property DateSent As DateTime
        Public Property Attachments As List(Of Attachment)
        Public Property RolematrixfromOrganization As Boolean

        Public Property BlockEmailID As String
    End Class
    <Serializable()>
    Public Class Attachment

        Public Property FileName As String
        Public Property ContentType As String
        Public Property Content As Byte()
    End Class
    <Serializable()>
    Public Class ChildAttachment

        Public Property FileName As String
        Public Property FilePath As String
        Public Property FieldMapping As String
        Public Property ChildDocumenttype As String
    End Class
    <Serializable()>
    Public Class POP
        Public Property hostName As String
        Public Property mdport As Integer
        Public Property mdSSL As Boolean
        Public Property mdMailID As String
        Public Property mdPwd As String
    End Class
    Public Class DataValues
        Public Property objText As String
        Public Property objValue As String
    End Class

End Class

