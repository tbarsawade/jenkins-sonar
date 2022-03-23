Imports System.Data.SqlClient
Imports System.Data
Imports System.Web.Services
Imports Newtonsoft.Json
Imports System.Web.Script.Serialization
Imports Newtonsoft.Json.Converters
Imports AjaxControlToolkit
Imports System.IO
Imports System.Threading

Partial Class TicketDocDetail
    Inherits System.Web.UI.Page
    Public _CallerPage As Integer
    Protected Sub btnUpdate_click(sender As Object, e As EventArgs)
        Dim dyanamicForm As New DynamicForm()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim tran As SqlTransaction = Nothing
        con.Open()
        tran = con.BeginTransaction()
        'Try

        Dim value As String = hdnAction.Value
        If value = "" Then
            ClientScript.RegisterStartupScript(Me.GetType(), "Popup", "alert('Please take any  one user action');", True)
            Return
        End If

        Dim oda As SqlDataAdapter = New SqlDataAdapter("SELECT FF.*,F.*  FROM MMM_MST_FIELDS FF With(nolock)  left outer JOIN MMM_MST_FORMS F   With(nolock) on F.FormName = FF.DocumentType and F.EID = FF.EID   where (FF.isactive=1  and  MDfieldName is null) and F.EID=" & Session("EID").ToString() & " and FormName in (select documenttype from mmm_mst_doc with(nolock) where tid=" & Request.QueryString("DOCID") & ")  order by displayOrder", con)
        oda.SelectCommand.Transaction = tran
        Dim ds As New DataSet()
        oda.Fill(ds, "fields")
        Dim dv As DataView = ds.Tables("fields").DefaultView
        dv.RowFilter = "IsActive=1"
        Dim theFields As DataTable = dv.ToTable
        Dim lstData As New List(Of UserData)
        Dim obj As New DynamicForm()
        lstData = obj.CreateCollection(pnlnewfields, theFields)
        Dim FinalQry As String
        ViewState("tid") = Request.QueryString("DOCID")
        oda.SelectCommand.CommandText = "select * from mmm_mst_fields where documenttype in (select documenttype from mmm_mst_doc with(nolock) where tid=" & Request.QueryString("DOCID") & ") and eid=" & Session("EID") & " and mdfieldname is not null union all select * from mmm_mst_fields where documenttype in  (select dropdown from MMM_MST_FIELDS where DocumentType in(select documenttype from mmm_mst_doc with(nolock) where tid=" & Request.QueryString("DOCID") & ") and eid=" & Session("EID") & " and  fieldType='Child Item') and eid=" & Session("EID") & "  and mdfieldname is not null"
        Dim dt As New DataTable()
        oda.Fill(dt)
        Dim arrcolumn As New ArrayList()
        Dim ChildItemColumn As String = ""
        Dim ChildItemDocumentType As String = ""
        For Each dr As DataRow In dt.Rows
            Select Case dr("mdfieldname").ToString().ToUpper
                Case "EMAILID"
                    arrcolumn.Add(dr("FieldMapping").ToString() & "='" & [TO].InnerText & "'")
                Case "SUBJECT"
                    arrcolumn.Add(dr("FieldMapping").ToString() & "='" & txtSubject.Text & "'")
                Case "REMARKS"
                    arrcolumn.Add(dr("FieldMapping").ToString() & "='" & txtBody.Text.ToString().Replace("'", "''") & "'")
                Case "STATUS"
                    arrcolumn.Add(dr("FieldMapping").ToString() & "='" & value.ToString.ToUpper & "'")
                Case "CC"
                    arrcolumn.Add(dr("FieldMapping").ToString() & "='" & txtCC.Text.ToString() & "'")
                Case "ASSIGNEE"
                    arrcolumn.Add(dr("FieldMapping").ToString() & "=" & IIf(hdnAssignee.Value = String.Empty, "''", hdnAssignee.Value) & "")
                Case "ATTACHMENT"
                    ChildItemColumn = dr("FieldMapping").ToString()
                    ChildItemDocumentType = dr("documenttype").ToString()
            End Select
        Next
        FinalQry = dyanamicForm.ValidateAndGenrateQueryForControls("UPDATE", "UPDATE MMM_MST_DOC SET  ticketstatus='" & value.ToString() & "'," & String.Join(",", arrcolumn.ToArray()) & ",", "", ds.Tables("fields"), pnlnewfields, ViewState("tid"))
        If Trim(Left(FinalQry, 6)).ToUpper() = "PLEASE" Then
            FinalQry = FinalQry.Replace("<br/>", "\n")
            con.Close()
            ClientScript.RegisterStartupScript(Me.GetType(), "Popup", "alert('" & FinalQry.ToString() & "');", True)
            Return
        Else
            FinalQry = FinalQry & " WHERE tID=" & ViewState("tid")
            oda.SelectCommand.CommandText = FinalQry
            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            oda.SelectCommand.ExecuteNonQuery()
            If ChildItemColumn <> "" Then
                Dim splittedvalues As String() = hdnUploadedFileName.Value.Split(",")
                If splittedvalues(0) <> String.Empty Then
                    For x As Integer = 0 To splittedvalues.Length - 1
                        'Try
                        Dim tempFileName As String() = splittedvalues(x).ToString().Split("|")
                        Dim tempPath As String = Server.MapPath("~/DOCS/temp/" & tempFileName(0).ToString())
                        oda.SelectCommand.CommandText = "insert into TicketAccessFile values(" & ViewState("tid") & ",'" & tempPath & "',getdate()),(" & ViewState("tid") & ",'" & Server.MapPath("~/DOCS/") & Session("EID") & "/" & tempFileName(0) & "',getdate())"
                        If con.State = ConnectionState.Closed Then
                            con.Open()
                        End If
                        oda.SelectCommand.ExecuteNonQuery()
                        If Not Directory.Exists(Server.MapPath("~/DOCS/") & Session("EID")) Then
                            Directory.CreateDirectory(Server.MapPath("~/DOCS/") & Session("EID"))
                        End If
                        File.Copy(tempPath, Server.MapPath("~/DOCS/") & Session("EID") & "/" & tempFileName(0))
                        oda.SelectCommand.CommandText = "insert into mmm_mst_doc_item (DOCID,DOCUMENTTYPE,ISAUTH,LASTUPDATE,cmastertid," & ChildItemColumn & ",Attachment) values(" & ViewState("tid") & ",'" & ChildItemDocumentType & "',1,getdate(),0,'" & Session("EID") & "/" & tempFileName(0) & "','" & tempFileName(1).ToString() & "')"
                        If con.State = ConnectionState.Closed Then
                            con.Open()
                        End If
                        oda.SelectCommand.ExecuteNonQuery()
                        'Catch ex As Exception
                        '    Throw
                        '    ex.Message.ToString()
                        'End Try
                    Next
                End If
            End If
            Dim res As String
            Dim ob1 As New DMSUtil()
            If value.ToString().ToUpper = "CLOSED" Then
                res = ob1.GetNextUserFromRolematrixT(ViewState("tid"), Val(Session("EID").ToString()), Val(Session("UID").ToString()), "", Val(Session("UID").ToString()), con, tran)
                If String.IsNullOrEmpty(res) Then
                    res = GetNextUserFromOrganizatios(con, tran)
                End If
                AssignTicketToUserBasedOnCondition(res, ViewState("tid"), "", HttpContext.Current.Session("UID"), con, tran)
            End If
            dyanamicForm.HistoryT(Session("EID"), ViewState("tid"), HttpContext.Current.Session("UID"), Session("FORMNAME"), "MMM_MST_DOC", "UPDATE", con, tran)
            Trigger.ExecuteTriggerT(Session("FORMNAME"), Session("EID"), ViewState("tid"), con, tran)

            tran.Commit()

            Dim objDC As New DataClass
            'Change condition for new assignee if you wish to assign this ticket to someone else
            If hdnNewAssignee.Value <> "0" And hdnNewAssignee.Value <> "" Then
                objDC.ExecuteQryDT("update mmm_doc_dtl set userid=" & hdnNewAssignee.Value & " where tid in (select tid from mmm_doc_dtl where docid=" & ViewState("tid") & " and aprstatus is null)")
            End If

            If hdnPreStatus.Value.ToUpper = "SUSPENDED" Then
                objDC.ExecuteQryDT("update mmm_doc_dtl set userid=" & hdnAssignee.Value & " where tid in (select tid from mmm_doc_dtl where docid=" & ViewState("tid") & " and aprstatus is null)")
                objDC.ExecuteQryDT("update mmm_mst_doc set fld16=" & hdnOrganization.Value & " where tid =" & ViewState("tid") & "")
            End If

            'ThreadPool.QueueUserWorkItem(AddressOf ThreadProc, ViewState("tid") & "||" & HttpContext.Current.Session("EID") & "||" & HttpContext.Current.Session("FORMNAME") & "||" & "APPROVE" & "||" & "-" & "||" & IIf(txtCC.Text = "", "-", txtCC.Text))
            'Comment because user did not want to see self writing code


            Dim dttemp As New DataTable
            dttemp = objDC.ExecuteQryDT("select rtrim(ltrim(userrole)) from mmm_mst_user where uid=" & Val(Session("UID").ToString()) & " and eid=" & Val(Session("EID").ToString()) & "")
            Dim tmpUSER As String = objDC.ExecuteQryScaller("select ','+isnull(USERROLE,'')+',' from  mmm_hdmail_schdule where eid=" & Session("EID"))
            If hdnNewAssignee.Value <> "0" And hdnNewAssignee.Value <> "" Then
                ThreadPool.QueueUserWorkItem(AddressOf ThreadProc, ViewState("tid") & "||" & HttpContext.Current.Session("EID") & "||" & HttpContext.Current.Session("FORMNAME") & "||" & "ASSIGNEDTOOTHERAGENT" & "||" & "-" & "||" & "-")
            ElseIf tmpUSER.ToString().ToUpper.Contains("," & dttemp.Rows(0)(0).ToString().ToUpper & ",") Then
                ThreadPool.QueueUserWorkItem(AddressOf ThreadProc, ViewState("tid") & "||" & HttpContext.Current.Session("EID") & "||" & HttpContext.Current.Session("FORMNAME") & "||" & "USERREPLY" & "||" & "-" & "||" & "-")
                'ob1.TemplateCalling(ViewState("tid"), HttpContext.Current.Session("EID"), HttpContext.Current.Session("FORMNAME"), "USERREPLY")
            Else
                ThreadPool.QueueUserWorkItem(AddressOf ThreadProc, ViewState("tid") & "||" & HttpContext.Current.Session("EID") & "||" & HttpContext.Current.Session("FORMNAME") & "||" & "AGENTREPLY" & "||" & "-" & "||" & "-")
                'ob1.TemplateCalling(ViewState("tid"), HttpContext.Current.Session("EID"), HttpContext.Current.Session("FORMNAME"), "AGENTREPLY")

            End If
            'If tmpUSER.ToString().ToUpper.Contains("," & dttemp.Rows(0)(0).ToString().ToUpper & ",") Then

            'Else

            'End If
            Response.Redirect("~/thome.aspx", False)
        End If
        'Catch ex As Exception
        '    tran.Rollback()
        '    lblMessage.Text = "BtnUpdate Exeception error " & ex.Message.ToString()
        'End Try
    End Sub
    Sub ThreadProc(str As Object)
        Dim ob As New DMSUtil()

        Dim values As String() = str.ToString().Split("||")
        Dim docid As String = values(0)
        Dim EID As String = values(2)
        Dim FormName As String = values(4)
        Dim Subevent As String = IIf(values(6) = "-", "", values(6))
        Dim UpcommingFrom As String = IIf(values(8) = "-", "", values(8))
        Dim TicketCC As String = IIf(values(10) = "-", "", values(10))
        ob.TemplateCalling(docid, EID, FormName, Subevent, UpcommingFrom, TicketCC)
        ' No state object was passed to QueueUserWorkItem, so stateInfo is null.
        ' Dim obj As New testinfo()
        'obj = stateInfo

    End Sub

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
                If IsNumeric(uids) Then
                    das.SelectCommand.CommandText = "select d.*, dt.ordering,dt.userid from MMM_MST_DOC with(nolock) D left outer join MMM_DOC_DTL with(nolock) dt on d.LastTID=dt.tid where EID=" & HttpContext.Current.Session("EID") & " and d.tid=" & docID
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
                Else

                End If
            End If
        Catch ex As Exception

        End Try
        Return Res
    End Function
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

    Public Sub bindvalue(ByVal sender As Object, ByVal e As EventArgs)
        Dim ddl As DropDownList = TryCast(sender, DropDownList)
        Dim id As String = Right(ddl.ID, ddl.ID.Length - 3)
        Dim id1 As Integer = CInt(id)
        Dim ob As New DynamicForm()
        ob.bind(id, pnlnewfields, ddl)
        ob.bindlookupddl(id, pnlnewfields, ddl)
        ob.bindMultiLookUP(id, pnlnewfields, ddl)
        ob.bindddlMultiLookUP(id, pnlnewfields, ddl)
        ob.bindLTLookUP(id, pnlnewfields, ddl)
    End Sub
    Public Sub bindvalue2(ByVal sender As Object, ByVal e As EventArgs)
        Dim ddl As DropDownList = TryCast(sender, DropDownList)
        Session("DDL") = ddl
        Dim id As String = Right(ddl.ID, ddl.ID.Length - 3)
        Dim id1 As Integer = CInt(id)
        Dim ob As New DynamicForm()
        ob.bind(id, pnlnewfields, ddl)
        ob.bindMultiLookUP(id, pnlnewfields, ddl)
        ob.bindddlMultiLookUP(id, pnlnewfields, ddl)
    End Sub

    Protected Sub Page_Init(sender As Object, e As EventArgs) Handles Me.Init
        Try
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As SqlConnection = New SqlConnection(conStr)
            Dim oda As SqlDataAdapter = New SqlDataAdapter("SELECT FF.*,F.*  FROM MMM_MST_FIELDS FF With(nolock)  left outer JOIN MMM_MST_FORMS F   With(nolock) on F.FormName = FF.DocumentType and F.EID = FF.EID   where (FF.isactive=1 and  MDfieldName is null) and F.EID=" & Session("EID").ToString() & " and FormName in (select documenttype from mmm_mst_doc with(nolock) where tid=" & Request.QueryString("DOCID") & ") and fieldType<>'Child Item'  order by displayOrder", con)
            Dim ds As New DataSet()
            oda.Fill(ds, "data")
            Dim objDC As New DataClass
            CreateControlsOnPanel(ds.Tables(0), pnlnewfields, Nothing, Nothing, 0, Nothing, Nothing, False, False, hdnStatus.Value)
            Dim ROW1() As DataRow = ds.Tables(0).Select("fieldtype='DROP DOWN'   and (lookupvalue is not null or ddllookupvalue is not null or multilookUpVal is not null or ltlookupval is not null or HasRule='1')")
            If ROW1.Length > 0 Then
                For i As Integer = 0 To ROW1.Length - 1
                    Dim check As Integer = 0
                    Dim DDL As DropDownList = TryCast(pnlnewfields.FindControl("fld" & ROW1(i).Item("FieldID").ToString()), DropDownList)
                    Dim id As String = Right(DDL.ID, DDL.ID.Length - 3)
                    DDL.AutoPostBack = True
                    'Change By V 24 Dec
                    If ds.Tables("data").Rows.Count > 0 Then
                        For j As Integer = 0 To ds.Tables("data").Rows.Count - 1
                            If ds.Tables("data").Rows(j).Item("KC_LOGIC").ToString.Contains(id) = True Then
                                DDL.AutoPostBack = True
                                AddHandler DDL.TextChanged, AddressOf bindvalue2
                                check = check + 1
                            End If
                        Next
                    End If
                    If check = 0 Then
                        AddHandler DDL.TextChanged, AddressOf bindvalue
                        'ExecuteControllevelRule(CInt(id), pnlFields, Nothing, screenname, ds.Tables("data"), Nothing, lblTab, False)
                    End If
                Next
            End If
            Dim dyanamicForm As New DynamicForm()
            dyanamicForm.FillControlsOnPanel(ds.Tables(0), pnlnewfields, "Document", Convert.ToInt32(Request.QueryString("DOCID").ToString()))
            oda.SelectCommand.CommandText = "select documenttype from mmm_mst_doc with(nolock) where tid=" & Request.QueryString("DOCID")
            If con.State = ConnectionState.Closed Then
                con.Open()
            End If
            Session("FORMNAME") = Convert.ToString(oda.SelectCommand.ExecuteScalar())
            con.Close()
            imageSource.Src = "~/logo/" & objDC.ExecuteQryScaller("select logo_Text  from mmm_mst_entity where eid=" & Session("EID"))

        Catch ex As Exception
        End Try
    End Sub
    Public Function FollowUPshowHide() As Boolean
        Dim objDC As New DataClass
        Dim response As String = objDC.ExecuteQryScaller("declare @column nvarchar(max),@Qry nvarchar(max)set @Qry='select @column= (select '+ (select fieldmapping from mmm_mst_fields with(nolock) where documenttype in (select documenttype from mmm_mst_doc with(nolock) where tid =" & Request.QueryString("DOCID") & ") and eid=" & Session("EID") & " and mdfieldname ='Status') +' from mmm_mst_doc with(nolock) where tid=" & Request.QueryString("DOCID") & ")' exec sp_executesql @Qry,N'@column nvarchar(max) OUTPUT', @column = @column output select  @column")
        If response.ToString.ToUpper = "CLOSED" And hdnStatus.Value = "" Then
            hdnStatus.Value = "CLOSED"
            ddlAssignee.Attributes.Add("disabled", "disabled")
            txtCC.Attributes.Add("readonly", "readonly")
            'ddlPriority.Attributes.Add("disabled", "disabled")
            'txtTags.Attributes.Add("readonly", "readonly")
            useraction.Visible = False
            txtSubject.Attributes.Add("readonly", "readonly")
            HEE_body.Enabled = False
            noaction.Visible = True
            EnableDisableControl(False)
        Else
            ddlAssignee.Attributes.Remove("disabled")
            txtCC.Attributes.Remove("readonly")
            'ddlPriority.Attributes.Remove("disabled")
            'txtTags.Attributes.Remove("readonly")
            useraction.Visible = True
            txtSubject.Attributes.Remove("readonly")
            HEE_body.Enabled = True
            noaction.Visible = False
        End If
    End Function
    Public Function GetQuery1(ByVal doctype As String, ByVal fld As String) As String
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim da As New SqlDataAdapter("", con)
        Try
            Dim ds As New DataSet
            Dim str As String = ""
            da.SelectCommand.CommandText = "usp_GetMasterValued1"
            da.SelectCommand.CommandType = CommandType.StoredProcedure
            da.SelectCommand.Parameters.Clear()
            da.SelectCommand.Parameters.AddWithValue("@doctype", doctype)
            da.SelectCommand.Parameters.AddWithValue("@eid", HttpContext.Current.Session("eid"))
            da.SelectCommand.Parameters.AddWithValue("@fldmapping", fld)
            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            str = da.SelectCommand.ExecuteScalar()
            Return str
        Catch ex As Exception
            Throw
        Finally
            If Not con Is Nothing Then
                con.Dispose()
            End If
            If Not da Is Nothing Then
                da.Dispose()
            End If
        End Try
    End Function
    ''Function to Filter the Data according to User
    Public Function UserDataFilter(ByVal cdocumenttype As String, ByVal ddocumenttype As String) As String
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim da As New SqlDataAdapter("", con)
        Try
            Dim ds As New DataSet
            Dim fldmapping As String = ""
            Dim fldid As String = ""


            da.SelectCommand.CommandText = "select eventname,formsource from mmm_mst_forms where eid=" & HttpContext.Current.Session("Eid") & " and formname='" & cdocumenttype & "'"
            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            Dim FormSource As String = "" : Dim eventNm As String = ""
            Dim dtFT As New DataTable
            da.Fill(dtFT)
            If dtFT.Rows.Count > 0 Then
                FormSource = dtFT.Rows(0).Item("formsource").ToString
                eventNm = dtFT.Rows(0).Item("eventname").ToString
                If FormSource.ToUpper() = "ACTION DRIVEN" Then
                    cdocumenttype = eventNm
                End If
            End If

            dtFT.Dispose()

            da.SelectCommand.CommandText = "select docmapping from mmm_mst_forms where eid=" & HttpContext.Current.Session("Eid") & " and Formname='" & ddocumenttype & "'"

            fldmapping = Convert.ToString(da.SelectCommand.ExecuteScalar)
            If fldmapping.Length > 2 Then
                da.SelectCommand.CommandText = "select " & fldmapping & ",documenttype,iscreate,isedit from mmm_ref_role_user where eid=" & HttpContext.Current.Session("eid") & " and Uid=" & HttpContext.Current.Session("uid") & " and roleNAME='" & HttpContext.Current.Session("USERROLE") & "' and '" & cdocumenttype & "' in (select * from InputString1(documenttype))"
                da.Fill(ds, "FILTER")
                If ds.Tables("FILTER").Rows.Count = 0 Then
                    fldid = ""
                ElseIf ds.Tables("FILTER").Rows.Count = 1 And ds.Tables("FILTER").Rows(0).Item("iscreate").ToString() <> "0" Then
                    fldid = ds.Tables("FILTER").Rows(0).Item(0).ToString()
                Else
                    Dim RW() As DataRow = ds.Tables("FILTER").Select("ISCREATE=1")
                    If RW.Length > 0 Then
                        fldid = RW(0).Item(0).ToString()
                    Else
                        fldid = ""
                    End If
                    'For Each dr As DataRow In ds.Tables("FILTER").Rows
                    '    If dr.Item(0).ToString() <> "*" And dr.Item("iscreate").ToString() <> "0" Then
                    '        fldid = dr.Item(0).ToString()
                    '    Else
                    '        fldid = dr.Item("iscreate").ToString()
                    '    End If
                    'Next
                End If
            End If

            Return fldid
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
    ''Find control exists on the Panel or Not
    Public Shared Function GetControl(ByVal page As Panel, ByVal ctlid As String) As Boolean
        Dim control As Control = Nothing
        control = page.FindControl(ctlid)
        'Dim ctrlname As String = page.Request.Params.[Get]("__EVENTTARGET")
        'If ctrlname IsNot Nothing AndAlso ctrlname <> String.Empty Then
        '    control = page.FindControl(ctrlname)
        'Else
        '    For Each ctl As String In page.Request.Form
        '        Dim c As Control = page.FindControl(ctl)
        '        If TypeOf c Is System.Web.UI.WebControls.Button Then
        '            control = c
        '            Exit For
        '        End If
        '    Next
        'End If

        If control Is Nothing Then
            Return False
        Else
            Return True
        End If
    End Function
    Public Function BindForChild1(ByVal id1 As Integer, ByRef pnlFields As Panel, ByRef ddl As DropDownList) As String
        ' new by sunil on 09-dec
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        If con.State <> ConnectionState.Open Then
            con.Open()
        End If

        Dim oda As SqlDataAdapter = New SqlDataAdapter("select LOOKUPVALUE,dropdown,documenttype from MMM_MST_FIELDS WHERE FIELDID=" & id1 & "", con)
        Try
            Dim DS As New DataSet
            Dim xwhr As String = ""
            oda.Fill(DS, "data")
            'Dim LOOKUPVALUE As String = DS.Tables("data").Rows(0).Item("lookupvalue").ToString()
            Dim documenttype() As String = DS.Tables("data").Rows(0).Item("dropdown").ToString.Split("-")
            Dim STR As String = ""

            Dim str1 = documenttype(1).ToString
            Dim MidStr() As String = documenttype(1).ToString.Split(":")

            If GetControl(pnlFields, "fld" & MidStr(0).ToString) Then
                oda = New SqlDataAdapter("SELECT * FROM MMM_MST_FIELDS WHERE FIELDID=" & MidStr(0).ToString & "", con)
                Dim dt As New DataTable
                oda.Fill(dt)

                Dim proc As String = "usp_GetChildValuedFilterData"    'dt.Rows(0).Item("CAL_FIELDS").ToString()
                If proc.Length > 1 Then
                    Dim DROPDOWN1 As String = MidStr(0).ToString  'dt.Rows(0).Item("AUTOFILTER").ToString()
                    Dim DDL0 As DropDownList = TryCast(pnlFields.FindControl("fld" & DROPDOWN1), DropDownList)
                    ' bindsunil

                    'Change By V 24 Dec

                    If DDL0.SelectedValue <> 0 Then
                        HttpContext.Current.Session("Val") = DDL0.SelectedValue
                    Else
                        If IsNothing(ddl) Then
                        Else
                            HttpContext.Current.Session("Val") = ddl.SelectedValue
                        End If
                    End If

                    If HttpContext.Current.Session("Val") <> 0 Then
                        oda.SelectCommand.CommandType = CommandType.StoredProcedure
                        oda.SelectCommand.Parameters.Clear()
                        oda.SelectCommand.CommandText = proc
                        ' oda.SelectCommand.Parameters.AddWithValue("DOCID", DDL0.SelectedValue)
                        oda.SelectCommand.Parameters.AddWithValue("DOCID", HttpContext.Current.Session("Val"))
                        oda.SelectCommand.Parameters.AddWithValue("DOCTYPE", MidStr(1).ToString)
                        oda.SelectCommand.Parameters.AddWithValue("EID", HttpContext.Current.Session("EID").ToString)
                        oda.SelectCommand.Parameters.AddWithValue("fldmapping", documenttype(2).ToString)
                        STR = oda.SelectCommand.ExecuteScalar().ToString()
                    End If
                End If
            End If
            Return STR
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
    Public Function UserDataFilter_PreRole(ByVal ddocumenttype As String, ByVal TableName As String) As String
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim da As New SqlDataAdapter("", con)
        Try
            Dim ds As New DataSet
            Dim fldmapping As String = ""
            Dim fldid As String = ""
            da.SelectCommand.CommandText = "select * from mmm_prerole_datafilter where eid=" & HttpContext.Current.Session("Eid") & " and documenttype='" & ddocumenttype & "' and rolename='" & HttpContext.Current.Session("USERROLE").ToString & "'"
            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            Dim DT As New DataTable

            da.Fill(DT)
            If DT.Rows.Count <> 0 Then
                fldmapping = DT.Rows(0).Item("fldmapping").ToString

                If fldmapping.Length > 2 Then
                    'da.SelectCommand.CommandText = "select " & fldmapping & ",tid from " & TableName & "  where eid=" & HttpContext.Current.Session("eid") & " and " & fldmapping & "='" & HttpContext.Current.Session("uid") & "'"
                    da.SelectCommand.CommandText = "SELECT SUBSTRING((SELECT ',' + CONVERT(NVARCHAR,TID)  FROM " & TableName & " where EID=" & HttpContext.Current.Session("eid") & " and " & fldmapping & "='" & HttpContext.Current.Session("uid") & "' FOR XML PATH('')),2,1000) AS CSV"
                End If
                da.Fill(ds, "FILTER")
                If ds.Tables("FILTER").Rows.Count = 0 Then
                    fldid = ""
                ElseIf ds.Tables("FILTER").Rows.Count = 1 Then
                    fldid = ds.Tables("FILTER").Rows(0).Item(0).ToString()
                End If
                If fldid = "" Then
                    fldid = "0"
                End If
            End If

            ds.Dispose()
            DT.Dispose()
            Return fldid
        Catch ex As Exception
            Throw
        Finally
            If Not con Is Nothing Then
                con.Close()
                con.Dispose()
            End If
            If Not da Is Nothing Then
                da.Dispose()
            End If
        End Try

    End Function
    ''Get MasterValued query to bind dropdown
    Public Function GetQuery(ByVal doctype As String, ByVal fld As String) As String
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim da As New SqlDataAdapter("", con)
        Try
            Dim ds As New DataSet
            Dim str As String = ""
            da.SelectCommand.CommandText = "usp_GetMasterValued"
            da.SelectCommand.CommandType = CommandType.StoredProcedure
            da.SelectCommand.Parameters.Clear()
            da.SelectCommand.Parameters.AddWithValue("@doctype", doctype)
            da.SelectCommand.Parameters.AddWithValue("@eid", HttpContext.Current.Session("eid"))
            da.SelectCommand.Parameters.AddWithValue("@fldmapping", fld)

            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            str = da.SelectCommand.ExecuteScalar()
            Return str
        Catch ex As Exception
            Throw
        Finally
            If Not con Is Nothing Then
                con.Dispose()
            End If
            If Not da Is Nothing Then
                da.Dispose()
            End If
        End Try
    End Function
    Public Sub CreateControlsOnPanel(ByVal dsFields As DataTable, ByRef pnlFields As Panel, ByRef UpdatePanel1 As UpdatePanel, ByRef btnActEdit As Button, ByVal autolayout As Integer, Optional ByRef ddown As DropDownList = Nothing, Optional ByVal amendment As String = Nothing, Optional ByVal IsDocDetail As Boolean = False, Optional ByVal IsCallingFromMainHome As Boolean = False, Optional ByVal TicketStatus As String = "OPEN")  ' Optional ByRef ddown As DropDownList = Nothing
        pnlFields.Controls.Clear()

        Dim oda As SqlDataAdapter = Nothing
        Dim oda1 As SqlDataAdapter = Nothing
        Dim DataType As String = String.Empty
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = Nothing
        Try
            Dim onlyFiltered As New DataView()
            onlyFiltered = dsFields.DefaultView()
            If Not HttpContext.Current.Session("AMENDMENT") Is Nothing Then
                If HttpContext.Current.Session("AMENDMENT") = "AMENDMENT" Then
                    onlyFiltered.RowFilter = "Invisible=0 or iseditonamend=1 or showondocdetail=1"
                ElseIf HttpContext.Current.Session("AMENDMENT") = "COPYHIT" Then
                    onlyFiltered.RowFilter = "Invisible=0 "
                Else
                    onlyFiltered.RowFilter = "Invisible=0 "
                End If
            Else
                onlyFiltered.RowFilter = "Invisible=0"
            End If

            ' onlyFiltered.RowFilter = "Invisible=0 and FieldType<>'Check Box'"

            Dim ds As DataTable = onlyFiltered.Table.DefaultView.ToTable()

            If ds.Rows.Count > 0 Then
                con = New SqlConnection(conStr)
                'pnlFields.Controls.Add(New LiteralControl("<div class=""control-group"">"))
                For i As Integer = 0 To ds.Rows.Count - 1
                    pnlFields.Controls.Add(New LiteralControl("<div class=""control-group""><label class=""control-label"">" & ds.Rows(i).Item("displayname").ToString() & IIf(ds.Rows(i).Item("isRequired").ToString() = "1", "*", "") & "</label> <div class=""controls"">"))
                    Select Case ds.Rows(i).Item("FieldType").ToString().ToUpper()
                        Case "DROP DOWN"
                            Dim ddl As New DropDownList
                            ddl.ID = "fld" & ds.Rows(i).Item("FieldID").ToString()
                            'Add Condition By mayank dropdown should be no editable in case of child specific text has some value
                            If (ds.Rows(i).Item("child_specific_text").ToString() <> String.Empty) Then
                                ddl.Enabled = False
                            Else
                                If ds.Rows(i).Item("isEditable").ToString() = "1" Then
                                    ddl.Enabled = True
                                Else
                                    ddl.Enabled = False
                                End If
                            End If
                            If TicketStatus.ToUpper = "CLOSED" Then
                                ddl.Enabled = False
                            End If
                            ddl.CssClass = "span12 m-wrap"
                            Dim ddlText As String = ds.Rows(i).Item("dropdown").ToString()
                            Dim dropdowntype As String = ds.Rows(i).Item("dropdowntype").ToString()
                            Dim arr() As String
                            Dim arrMid() As String
                            If UCase(dropdowntype) = "FIX VALUED" Then
                                arr = ddlText.Split(",")
                                ddl.Items.Add("SELECT")
                                For ii As Integer = 0 To arr.Count - 1
                                    ddl.Items.Add(arr(ii).ToUpper().Trim())
                                Next
                            ElseIf UCase(dropdowntype) = "MASTER VALUED" Then
                                'If ds.Rows(i).Item("FORMSOURCE").ToString().ToUpper.Trim() = "DETAIL FORM" And ds.Rows(i).Item("KC_LOGIC").ToString().Length > 1 Then
                                'Else
                                arr = ddlText.Split("-")
                                Dim TID As String = "TID"
                                Dim TABLENAME As String = ""
                                If UCase(arr(0).ToString()) = "MASTER" Then
                                    TABLENAME = "MMM_MST_MASTER"
                                ElseIf UCase(arr(0).ToString()) = "DOCUMENT" Then
                                    TABLENAME = "MMM_MST_DOC"
                                ElseIf UCase(arr(0).ToString()) = "CHILD" Then
                                    TABLENAME = "MMM_MST_DOC_ITEM"
                                ElseIf UCase(arr(0).ToString) = "STATIC" Then
                                    If arr(1).ToString.ToUpper = "USER" Then
                                        TABLENAME = "MMM_MST_USER"
                                        TID = "UID"
                                    ElseIf arr(1).ToString().ToUpper = "LOCATION" Then
                                        TABLENAME = "MMM_MST_LOCATION"
                                        TID = "LOCID"
                                    End If
                                End If
                                Dim lookUpqry As String = ""
                                Dim str As String = ""
                                If arr(0).ToUpper() = "CHILD" Then
                                    str = "select " & arr(2).ToString() & "," & TID & "[tid]  from " & TABLENAME & " M WHERE   DOCUMENTTYPE='" & arr(1).ToString() & "'"
                                ElseIf arr(0).ToUpper() <> "STATIC" Then
                                    str = "select " & arr(2).ToString() & "," & TID & "[tid]" & lookUpqry & " from " & TABLENAME & " M WHERE EID=" & HttpContext.Current.Session("EID") & " AND  DOCUMENTTYPE='" & arr(1).ToString() & "'"
                                Else
                                    If arr(2).ToString.ToUpper = "LOCATIONSTATE" Then
                                        str = "select DISTINCT " & arr(2).ToString() & ",SID [tid]" & lookUpqry & " from " & TABLENAME & " M "
                                    Else
                                        str = "select " & arr(2).ToString() & "," & TID & "[tid]" & lookUpqry & " from " & TABLENAME & " M WHERE EID=" & HttpContext.Current.Session("EID") & " "
                                    End If
                                End If

                                Dim xwhr As String = ""
                                Dim tids As String = ""
                                Dim Rtids As String = ""   ' for prerole data filter
                                'Dim tidarr() As String

                                ''FILTER THE DATA ACCORDING TO USER 
                                tids = UserDataFilter(ds.Rows(i).Item("documenttype").ToString(), arr(1).ToString())
                                Rtids = UserDataFilter_PreRole(arr(1).ToString(), TABLENAME)  '' new by sunil for pre role data filter 22-feb

                                '' for multiuse of document by sp on 08_apr_15
                                Dim Sdtype As String = arr(1).ToString
                                Dim da As SqlDataAdapter = New SqlDataAdapter("", con)
                                da.SelectCommand.CommandType = CommandType.Text
                                da.SelectCommand.CommandText = "select isnull(Allowmultiuse,0) from mmm_mst_forms where eid='" & HttpContext.Current.Session("EID").ToString & "' AND FORMNAME='" & Sdtype & "'"
                                If con.State = ConnectionState.Closed Then
                                    con.Open()
                                End If
                                Dim isMultiUse As Integer = Convert.ToInt32(da.SelectCommand.ExecuteScalar())

                                Dim CurrDoctype As String = ds.Rows(i).Item("documenttype").ToString()
                                Dim CurrFieldMapping As String = ds.Rows(i).Item("fieldmapping").ToString()

                                Dim qry As String = ""
                                Dim MTids As String = ""
                                If UCase(arr(0).ToString()) <> "CHILD" And UCase(arr(0).ToString) <> "STATIC" Then
                                    da.SelectCommand.CommandType = CommandType.Text
                                    da.SelectCommand.CommandText = "SELECT SUBSTRING((SELECT ',' + CONVERT(NVARCHAR," & CurrFieldMapping & ")  FROM " & TABLENAME & " where EID=" & HttpContext.Current.Session("eid") & " and documenttype='" & CurrDoctype & "'" & " FOR XML PATH('')),2,1000) AS CSV"
                                    MTids = Convert.ToString(da.SelectCommand.ExecuteScalar())
                                End If

                                ''ends  for multiuse of document by sp on 08_apr_15


                                If tids.Length >= 2 Then
                                    xwhr = " AND CONVERT(NVARCHAR(10),TID) IN (" & tids & ")"
                                ElseIf tids = "0" Then
                                    pnlFields.Visible = False
                                    btnActEdit.Visible = False
                                    UpdatePanel1.Update()
                                    xwhr = ""
                                End If

                                ''ends  for multiuse of document by sp on 08_apr_15
                                '' new by sunil for pre role data filter 22-feb
                                If Rtids <> "" Then
                                    If xwhr.ToString = "" Then
                                        xwhr = " AND CONVERT(NVARCHAR(10),TID) IN (" & Rtids & ")"
                                    Else
                                        xwhr = " AND CONVERT(NVARCHAR(10),TID) IN (" & tids & "," & Rtids & ")"
                                    End If
                                End If

                                '' new by sunil for multiuse of docs
                                If MTids.Length > 2 And isMultiUse = 1 Then
                                    If Right(MTids, 1) = "," Then
                                        MTids = Left(MTids, Len(MTids) - 1)
                                    End If
                                    xwhr = xwhr & " AND CONVERT(NVARCHAR(10),TID) not IN (" & MTids & ") "
                                End If
                                '' new by sunil for multiuse of docs

                                If amendment = "AMENDMENT" Then
                                    str = str & "   " & xwhr & " order by " & arr(2).ToString()
                                Else
                                    'str = str & "  AND M.isauth=1 " & xwhr & " order by " & arr(2).ToString()  ' prev
                                    str = str & "  AND (M.isauth>0) " & xwhr & " order by " & arr(2).ToString()  ' changed by sp for multiuse
                                    ' str = str & "  AND (M.isauth=1 or M.isauth=2) " & xwhr & " AND CONVERT(NVARCHAR(10),TID) not IN (" & MTids & ") " & " order by " & arr(2).ToString()  ' changed by sp for multiuse
                                End If

                                Dim AutoFilter As String = ds.Rows(i).Item("AutoFilter").ToString()
                                Dim InitFilterArr As String() = ds.Rows(i).Item("InitialFilter").ToString().Split(":")
                                Dim SessionFieldvalue As String = Convert.ToString(ds.Rows(i).Item("SessionFieldVal"))
                                ' If InitFilter.Length > 0 Then
                                ' Dim initFilArr As String() = ds.Rows(i).Item("InitialFilter").ToString().Split(":")
                                '  End If


                                If AutoFilter.Length > 0 Then
                                    If arr(0).ToUpper() = "CHILD" Then
                                        If AutoFilter.ToUpper = "DOCID" Then
                                            str = GetQuery1(arr(1).ToString, arr(2).ToString())
                                        Else
                                            str = GetQuery(arr(1).ToString, arr(2).ToString)
                                        End If
                                    ElseIf arr(0).ToUpper() <> "STATIC" Then
                                        str = "select " & arr(2).ToString() & ",convert(nvarchar(10),tid)  [tid]" & lookUpqry & " from " & TABLENAME & " M WHERE EID=" & HttpContext.Current.Session("EID") & " AND  DOCUMENTTYPE='" & arr(1).ToString() & "'"
                                        str = str & "  AND (M.isauth>0) " & xwhr & " order by " & arr(2).ToString()
                                    Else
                                        str = "select " & arr(2).ToString() & ",convert(nvarchar(10)," & TID & ")  [tid]" & lookUpqry & " from " & TABLENAME & " M WHERE EID=" & HttpContext.Current.Session("EID") & " "
                                        str = str & "  AND (M.isauth>0) " & xwhr & " order by " & arr(2).ToString()
                                    End If
                                ElseIf SessionFieldvalue.Length > 0 Then
                                    Dim val As String() = SessionFieldvalue.ToString().Split("-")
                                    If arr(0).ToUpper() = "CHILD" Then
                                        If AutoFilter.ToUpper = "DOCID" Then
                                            str = GetQuery1(arr(1).ToString, arr(2).ToString())
                                        Else
                                            str = GetQuery(arr(1).ToString, arr(2).ToString)
                                        End If
                                    ElseIf arr(0).ToUpper() <> "STATIC" Then
                                        da.SelectCommand.Parameters.Clear()
                                        da.SelectCommand.CommandText = "select isnull(" & val(0) & ",0) from mmm_mst_user where eid=" & HttpContext.Current.Session("EID") & " and uid=" & HttpContext.Current.Session("UID")
                                        If con.State = ConnectionState.Closed Then
                                            con.Open()
                                        End If
                                        Dim Conval As String = Replace(Convert.ToString(da.SelectCommand.ExecuteScalar), ",", "','")
                                        str = "select " & arr(2).ToString() & ",convert(nvarchar(10)," & TID & ")  [tid] from " & TABLENAME & " M WHERE  EID=" & HttpContext.Current.Session("EID") & " AND  DOCUMENTTYPE='" & arr(1).ToString() & "'"
                                        If Conval.Length > 1 Then
                                            If SessionFieldvalue.Contains("-") Then
                                                str = str & "  AND (M.isauth>0) " & xwhr & " and " & val(2) & " in('" & Conval & "') order by " & arr(2).ToString()
                                            Else
                                                str = str & "  AND (M.isauth>0) " & xwhr & " and tid in ('" & Conval & "') order by " & arr(2).ToString()
                                            End If

                                        Else
                                            str = str & "  AND (M.isauth>0) " & xwhr & "  order by " & arr(2).ToString()
                                        End If

                                    Else
                                        str = "select " & arr(2).ToString() & ",convert(nvarchar(10)," & TID & ")  [tid]" & lookUpqry & " from " & TABLENAME & " M WHERE EID=" & HttpContext.Current.Session("EID") & " "
                                        str = str & "  AND (M.isauth>0) " & xwhr & " order by " & arr(2).ToString()
                                    End If

                                ElseIf InitFilterArr.Length > 1 Then
                                    '' for getting def. value from field master
                                    Dim row() As DataRow = dsFields.Select("fieldid=" & InitFilterArr(0).ToString())
                                    If arr(0).ToUpper() = "DOCUMENT" Or arr(0).ToUpper() = "MASTER" Then
                                        If row.Length > 0 Then
                                            str = " Select " & arr(2).ToString() & ", convert(nvarchar(10),tid) [TID] FROM " & TABLENAME & " M where EID=" & HttpContext.Current.Session("EID") & " AND  DOCUMENTTYPE='" & arr(1).ToString() & "'"
                                            str = str & " AND " & InitFilterArr(1).ToString() & "='" & row(0).Item("defaultFieldVal").ToString & "'"
                                            If amendment = "AMENDMENT" Then
                                                str = str & "  " & xwhr & " order by " & arr(2).ToString()
                                            Else
                                                str = str & "  AND (M.isauth>0) " & xwhr & " order by " & arr(2).ToString()
                                            End If
                                        End If
                                    ElseIf arr(0).ToUpper() = "STATIC" Then
                                        If row.Length > 0 Then
                                            str = " Select " & arr(2).ToString() & ",convert(nvarchar(10)," & TID & ")  [tid]" & "FROM " & TABLENAME & " M where EID=" & HttpContext.Current.Session("EID") & " "
                                            str = str & " AND " & InitFilterArr(1).ToString() & "='" & row(0).Item("defaultFieldVal").ToString & "'"
                                            str = str & " AND (M.isauth>0) " & xwhr
                                            ' to be used for apm user bind from role assignment also 12_sep_14
                                            str = str & " union Select  " & arr(2).ToString() & ",convert(nvarchar(10)," & TID & ")  [tid]" & "FROM " & TABLENAME & " where convert(nvarchar(10)," & TID & ") in (select uid from MMM_Ref_Role_User where eid=" & HttpContext.Current.Session("EID") & " and rolename='" & row(0).Item("defaultFieldVal").ToString & "') order by " & arr(2).ToString() & ""
                                        End If
                                    End If
                                End If

                                '' prev code bkup by sp on 17_feb
                                'If AutoFilter.Length > 0 Then
                                '    If arr(0).ToUpper() = "CHILD" Then
                                '        If AutoFilter.ToUpper = "DOCID" Then
                                '            str = GetQuery1(arr(1).ToString, arr(2).ToString())
                                '        Else
                                '            str = GetQuery(arr(1).ToString, arr(2).ToString)
                                '        End If
                                '    ElseIf arr(0).ToUpper() <> "STATIC" Then
                                '        str = "select " & arr(2).ToString() & ",convert(nvarchar(10),tid)  [tid]" & lookUpqry & " from " & TABLENAME & " M WHERE EID=" & HttpContext.Current.Session("EID") & " AND  DOCUMENTTYPE='" & arr(1).ToString() & "'"
                                '        str = str & "  AND M.isauth=1 " & xwhr & " order by " & arr(2).ToString()
                                '    Else
                                '        str = "select " & arr(2).ToString() & ",convert(nvarchar(10)," & TID & ")  [tid]" & lookUpqry & " from " & TABLENAME & " M WHERE EID=" & HttpContext.Current.Session("EID") & " "
                                '        str = str & "  AND M.isauth=1 " & xwhr & " order by " & arr(2).ToString()
                                '    End If
                                'End If
                                oda = New SqlDataAdapter("", con)
                                Dim dss As New DataSet

                                If str.Length > 0 Then
                                    oda.SelectCommand.CommandText = str
                                    oda.Fill(dss, "FV")
                                    Dim isAddJquery As Integer = 0
                                    ddl.Items.Add("Select")
                                    ddl.Items(0).Value = "0"
                                    For J As Integer = 0 To dss.Tables("FV").Rows.Count - 1
                                        ddl.Items.Add(dss.Tables("FV").Rows(J).Item(0).ToString())
                                        Dim lookddlVal As String = dss.Tables("FV").Rows(J).Item(1).ToString()
                                        ddl.Items(J + 1).Value = lookddlVal
                                    Next
                                    dss.Dispose()
                                    If isAddJquery = 1 Then
                                        Dim JQuertStr As String = "var r1 = $('#ContentPlaceHolder1_" & ddl.ClientID & "').val(); var l = 0; var mycars = new Array(); for (var i = 0; i < r1.length; i++) { if (r1[i] == '|') { l++; mycars[l] = i; } } for (var i1 = 1; i1 < l; i1++) { var outpu = r1.substring(mycars[i1] + 1, mycars[i1 + 1]); var outpu1 = outpu.substring(0, outpu.indexOf(':')); var outpu2 = outpu.substring(outpu.indexOf(':') + 1); if (outpu2 == 'S') { var out = r1.substring(0, mycars[1]); var x = $('#ContentPlaceHolder1_tmp' + outpu1 + ' option').length; var options = ''; txt = ''; for (i = 0; i < x; i++) { var strUser = $('#ContentPlaceHolder1_tmp' + outpu1 + ' option:eq(' + i + ')').val(); var sel = strUser.substring(strUser.indexOf('-') + 1);  if (out == sel) { var finalshow = $('#ContentPlaceHolder1_tmp' + outpu1 + ' option:eq(' + i + ')').text();  options = options + '<option value=' + finalshow + '>' + finalshow + '</option>\n'; } } $('#ContentPlaceHolder1_' + outpu1 + '').html(options); } else { $('#ContentPlaceHolder1_' + outpu1 + '').val(outpu2); } $('#ContentPlaceHolder1_' + outpu1 + '').val(outpu2); }"
                                    End If
                                End If

                                ' NEW ADDED BY SUNIL ON 07-12-13 FOR CHILD-CHILD FILTERING
                            ElseIf UCase(dropdowntype) = "CHILD VALUED" Then
                                ' you are here on 09-dec-13
                                arr = ddlText.Split("-")
                                Dim Midstr As String = arr(1).ToString()
                                Dim TID As String = "TID"
                                Dim TABLENAME As String = ""
                                If UCase(arr(0).ToString()) = "MASTER" Then
                                    TABLENAME = "MMM_MST_MASTER"
                                ElseIf UCase(arr(0).ToString()) = "DOCUMENT" Then
                                    TABLENAME = "MMM_MST_DOC"
                                ElseIf UCase(arr(0).ToString()) = "CHILD" Then
                                    TABLENAME = "MMM_MST_DOC_ITEM"
                                    arrMid = arr(1).Split(":")

                                End If
                                Dim lookUpqry As String = ""
                                Dim str As String = ""
                                If arr(0).ToUpper() = "CHILD" Then
                                    str = "select " & arr(2).ToString() & "," & TID & "[tid]  from " & TABLENAME & " M WHERE   DOCUMENTTYPE='" & arrMid(1).ToString() & "'"
                                End If

                                Dim xwhr As String = ""
                                Dim tids As String = ""

                                ''FILTER THE DATA ACCORDING TO USER 
                                tids = UserDataFilter(ds.Rows(i).Item("documenttype").ToString(), arrMid(1).ToString())

                                If tids.Length >= 2 Then
                                    xwhr = " AND CONVERT(NVARCHAR(10),TID) IN (" & tids & ")"
                                ElseIf tids = "0" Then
                                    pnlFields.Visible = False
                                    btnActEdit.Visible = False
                                    UpdatePanel1.Update()
                                    xwhr = ""
                                End If
                                str = str & "  AND M.isauth=1 " & xwhr & " order by " & arr(2).ToString()
                                'Dim AutoFilter As String = ds.Rows(i).Item("AutoFilter").ToString()
                                Dim AutoFilter As String = arrMid(0).ToString()
                                If AutoFilter.Length > 0 Then
                                    If arr(0).ToUpper() = "CHILD" Then
                                        ' str = GetQuery(arrMid(1).ToString, arr(2).ToString)
                                        'new added by sunil on 11-dec for child-child filter
                                        ' str = BindForChild1(ds.Rows(i).Item("FieldID").ToString(), pnlFields)
                                        str = BindForChild1(ds.Rows(i).Item("FieldID").ToString(), pnlFields, ddown)
                                    ElseIf arr(0).ToUpper() <> "STATIC" Then
                                        str = "select " & arr(2).ToString() & ",convert(nvarchar(10),tid)  [tid]" & lookUpqry & " from " & TABLENAME & " M WHERE EID=" & HttpContext.Current.Session("EID") & " AND  DOCUMENTTYPE='" & arr(1).ToString() & "'"
                                        str = str & "  AND M.isauth=1 " & xwhr & " order by " & arr(2).ToString()
                                    Else
                                        str = "select " & arr(2).ToString() & ",convert(nvarchar(10)," & TID & ")  [tid]" & lookUpqry & " from " & TABLENAME & " M WHERE EID=" & HttpContext.Current.Session("EID") & " "
                                        str = str & "  AND M.isauth=1 " & xwhr & " order by " & arr(2).ToString()
                                    End If
                                End If

                                oda = New SqlDataAdapter("", con)
                                Dim dss As New DataSet

                                If str.Length > 0 Then
                                    oda.SelectCommand.CommandText = str
                                    oda.Fill(dss, "FV")
                                    Dim isAddJquery As Integer = 0
                                    ddl.Items.Add("Select")
                                    ddl.Items(0).Value = "0"
                                    For J As Integer = 0 To dss.Tables("FV").Rows.Count - 1
                                        ddl.Items.Add(dss.Tables("FV").Rows(J).Item(0).ToString())
                                        Dim lookddlVal As String = dss.Tables("FV").Rows(J).Item(1).ToString()
                                        ddl.Items(J + 1).Value = lookddlVal
                                    Next
                                    oda.Dispose()
                                    dss.Dispose()
                                    If isAddJquery = 1 Then
                                        Dim JQuertStr As String = "var r1 = $('#ContentPlaceHolder1_" & ddl.ClientID & "').val(); var l = 0; var mycars = new Array(); for (var i = 0; i < r1.length; i++) { if (r1[i] == '|') { l++; mycars[l] = i; } } for (var i1 = 1; i1 < l; i1++) { var outpu = r1.substring(mycars[i1] + 1, mycars[i1 + 1]); var outpu1 = outpu.substring(0, outpu.indexOf(':')); var outpu2 = outpu.substring(outpu.indexOf(':') + 1); if (outpu2 == 'S') { var out = r1.substring(0, mycars[1]); var x = $('#ContentPlaceHolder1_tmp' + outpu1 + ' option').length; var options = ''; txt = ''; for (i = 0; i < x; i++) { var strUser = $('#ContentPlaceHolder1_tmp' + outpu1 + ' option:eq(' + i + ')').val(); var sel = strUser.substring(strUser.indexOf('-') + 1);  if (out == sel) { var finalshow = $('#ContentPlaceHolder1_tmp' + outpu1 + ' option:eq(' + i + ')').text();  options = options + '<option value=' + finalshow + '>' + finalshow + '</option>\n'; } } $('#ContentPlaceHolder1_' + outpu1 + '').html(options); } else { $('#ContentPlaceHolder1_' + outpu1 + '').val(outpu2); } $('#ContentPlaceHolder1_' + outpu1 + '').val(outpu2); }"
                                        'ddl.Attributes.Add("onchange", JQuertStr)
                                    End If
                                End If
                                'End If
                            ElseIf UCase(dropdowntype) = "SESSION VALUED" Then
                                oda1 = New SqlDataAdapter("", con)
                                Dim ds1 As New DataSet
                                Dim QRY As String = ""
                                Dim DROPDOWN As String() = ds.Rows(i).Item("DROPDOWN").ToString().Split("-")
                                If DROPDOWN(1).ToString.ToUpper = "USER" Then
                                    QRY = "SELECT USERNAME ,UID FROM MMM_MST_USER WHERE EID=" & HttpContext.Current.Session("EID") & " AND " & DROPDOWN(2) & "='" & HttpContext.Current.Session(DROPDOWN(2)) & "'"
                                ElseIf DROPDOWN(1).ToString.ToUpper = "LOCATION" Then
                                    QRY = "SELECT LOCATIONNAME ,LOCID FROM MMM_MST_LOCATION WHERE EID=" & HttpContext.Current.Session("EID") & " AND " & DROPDOWN(2) & "='" & HttpContext.Current.Session(DROPDOWN(2)) & "'"
                                End If
                                oda1.SelectCommand.CommandText = QRY
                                oda1.Fill(ds1, "SESSION")
                                ddl.Items.Clear()
                                For iI As Integer = 0 To ds1.Tables("SESSION").Rows.Count - 1
                                    ddl.Items.Add(ds1.Tables("SESSION").Rows(iI).Item(0))
                                    ddl.Items(iI).Value = ds1.Tables("SESSION").Rows(iI).Item(1)
                                Next
                                ddl.Items.Insert(0, "SELECT")
                            End If
                            pnlFields.Controls.Add(ddl)
                            'For Grofers
                            If Convert.ToString(ds.Rows(i).Item("Style")) <> "" Then
                                Dim txt As String() = Convert.ToString(ds.Rows(i).Item("Style")).Split(",")
                                ddl.Attributes.Add("style", txt(1))
                            Else
                                ddl.CssClass = "span12 m-wrap"
                            End If
                            pnlFields.Controls.Add(New LiteralControl("</div></div>"))
                        Case "LOOKUP"
                            DataType = ds.Rows(i).Item("datatype").ToString().ToUpper()
                            Dim FieldType As String = ""
                            'Changes for Check box if having there
                            Dim CheckControl As DataView = ds.AsDataView
                            CheckControl.RowFilter = "lookupvalue Like" & "'%" & ds.Rows(i).Item("Fieldid") & "-%'"
                            Dim tempdt As DataTable = CheckControl.ToTable()
                            If tempdt.Rows.Count > 0 Then
                                Dim arr() As String = Convert.ToString(tempdt.Rows(0)("lookupvalue")).Split(",")
                                Dim fldmapping As String = ""
                                If arr.Length > 0 Then
                                    For XYZ As Integer = 0 To arr.Length - 1
                                        If Convert.ToString(arr(XYZ)).Contains(ds.Rows(i).Item("Fieldid") & "-") Then
                                            fldmapping = arr(XYZ)
                                            Exit For
                                        End If
                                    Next
                                    Dim finalFld As String() = fldmapping.Split("-")
                                    Dim dropdown As String() = Convert.ToString(tempdt.Rows(0)("dropdown")).Split("-")
                                    Try
                                        Dim da As SqlDataAdapter = New SqlDataAdapter("", con)
                                        da.SelectCommand.CommandType = CommandType.Text
                                        da.SelectCommand.CommandText = "select FieldType from mmm_mst_fields where documenttype='" & dropdown(1) & "' and fieldMapping='" & finalFld(1) & "' and eid=" & HttpContext.Current.Session("EID")
                                        If con.State = ConnectionState.Closed Then
                                            con.Open()
                                        End If

                                        FieldType = Convert.ToString(da.SelectCommand.ExecuteScalar())
                                        con.Close()
                                    Catch ex As Exception
                                        con.Dispose()
                                    End Try
                                End If
                            End If
                            If FieldType.Trim() = "Check Box" Then
                                Dim chkBox As New CheckBox
                                chkBox.ID = "fld" & ds.Rows(i).Item("FieldID").ToString()
                                'chkBox.Width = controlWdth - 10
                                'txtBox.CssClass = "txtBox"
                                If ds.Rows(i).Item("isEditable").ToString() = "1" Then
                                    chkBox.Enabled = True
                                Else
                                    chkBox.Enabled = False
                                End If
                                pnlFields.Controls.Add(chkBox)
                                'For Grofers
                                If Convert.ToString(ds.Rows(i).Item("Style")) <> "" Then
                                    Dim txt As String() = Convert.ToString(ds.Rows(i).Item("Style")).Split(",")
                                    chkBox.Attributes.Add("style", txt(1))
                                Else
                                    chkBox.CssClass = ""
                                End If
                            Else
                                Dim txtBox As New TextBox
                                txtBox.ID = "fld" & ds.Rows(i).Item("FieldID").ToString()
                                'txtBox.Width = controlWdth - 10
                                'txtBox.CssClass = "txtBox"
                                If ds.Rows(i).Item("isEditable").ToString() = "1" Then
                                    txtBox.Enabled = True
                                Else
                                    txtBox.Enabled = False
                                    txtBox.BackColor = Drawing.Color.White
                                    txtBox.ForeColor = Drawing.Color.Gray
                                    txtBox.Font.Bold = True
                                End If
                                pnlFields.Controls.Add(txtBox)
                                'For Grofers
                                If Convert.ToString(ds.Rows(i).Item("Style")) <> "" Then
                                    Dim txt As String() = Convert.ToString(ds.Rows(i).Item("Style")).Split(",")
                                    txtBox.Attributes.Add("style", txt(1))
                                Else
                                    txtBox.CssClass = "span12 m-wrap"
                                End If

                            End If
                            pnlFields.Controls.Add(New LiteralControl("</div></div>"))
                        Case "TEXT BOX"
                            DataType = ds.Rows(i).Item("datatype").ToString().ToUpper()
                            Dim txtBox As New TextBox
                            txtBox.ID = "fld" & ds.Rows(i).Item("FieldID").ToString()
                            If ds.Rows(i).Item("defaultfieldval").ToString.Length > 0 Then
                                txtBox.Text = ds.Rows(i).Item("defaultfieldval").ToString
                            Else
                                If DataType = "NUMERIC" Then
                                    txtBox.Text = "0"
                                End If
                            End If
                            If Val(ds.Rows(i).Item("iseditable").ToString()) = 0 Then
                                txtBox.Enabled = False
                            End If
                            pnlFields.Controls.Add(txtBox)
                            If Val(ds.Rows(i).Item("isDescription").ToString()) = 1 Then
                                txtBox.ToolTip = ds.Rows(i).Item("Description").ToString()
                                txtBox.Text = String.Empty
                                txtBox.Attributes.Add("placeholder", "Please begin typing a " & ds.Rows(i).Item("displayname"))
                            End If
                            If DataType = "DATETIME" Then
                                Dim CLNDR As New AjaxControlToolkit.CalendarExtender
                                CLNDR.Controls.Clear()
                                CLNDR.ID = "CLNDR" & ds.Rows(i).Item("FieldID").ToString()
                                CLNDR.Format = "dd/MM/yy"
                                CLNDR.TargetControlID = txtBox.ID
                                txtBox.Enabled = True
                                txtBox.Text = String.Format("{0:dd/MM/yy}", Date.Now())
                                If HttpContext.Current.Session("EDITonEDIT") Is Nothing Then ' this session is inittialized on doc detail page by balli  in order to check this is coming from edit option  or not 
                                    If ds.Rows(i).Item("iseditable") = 1 Then
                                        Dim img As New Image
                                        img.ID = "img" & ds.Rows(i).Item("FieldID").ToString()
                                        img.ImageUrl = "~\images\Cal.png"
                                        pnlFields.Controls.Add(img)
                                        CLNDR.PopupButtonID = "img" & ds.Rows(i).Item("FieldID").ToString()
                                        pnlFields.Controls.Add(CLNDR)
                                    Else
                                        txtBox.Enabled = False
                                    End If
                                Else
                                    If HttpContext.Current.Session("EDITonEDIT") = "EDITonEDIT" Then  ' this session is inittialized on doc detail page by balli 
                                        If ds.Rows(i).Item("alloweditonedit") = 1 And ds.Rows(i).Item("iseditable") = 1 Then
                                            Dim img As New Image
                                            img.ID = "img" & ds.Rows(i).Item("FieldID").ToString()
                                            img.ImageUrl = "~\images\Cal.png"
                                            pnlFields.Controls.Add(img)
                                            CLNDR.PopupButtonID = "img" & ds.Rows(i).Item("FieldID").ToString()
                                            pnlFields.Controls.Add(CLNDR)
                                        Else
                                            txtBox.Enabled = False
                                        End If
                                    End If
                                End If

                            ElseIf DataType = "NEW DATETIME" Then
                                'Data -field = "datetime"
                                txtBox.Enabled = True
                                'txtBox.ReadOnly = True
                                txtBox.ID = "fld" & ds.Rows(i).Item("FieldID").ToString()
                                txtBox.Attributes.Add("data-field", "datetime")
                                txtBox.Attributes.Add("readonly", "readonly")
                                'readonly
                                pnlFields.Controls.Add(txtBox)
                            ElseIf DataType = "TIME" Then
                                'Data -field = "datetime"
                                txtBox.Enabled = True
                                'txtBox.ReadOnly = True
                                txtBox.ID = "fld" & ds.Rows(i).Item("FieldID").ToString()
                                '  txtBox.Attributes.Add("data-field", "time")
                                '  txtBox.Attributes.Add("readonly", "readonly")
                                'readonly
                                pnlFields.Controls.Add(txtBox)
                            ElseIf DataType = "FY START" Then 'To Render FY Start  (Pallavi) on 29th Apr 15
                                'Data -field = "datetime"
                                txtBox.Enabled = False
                                'txtBox.ReadOnly = True
                                txtBox.ID = "fld" & ds.Rows(i).Item("FieldID").ToString()
                                '  txtBox.Attributes.Add("data-field", "FYStart")
                                txtBox.Attributes.Add("readonly", "readonly")
                                'readonly
                                Dim da As SqlDataAdapter = New SqlDataAdapter("select StartMonth +'-' + convert(varchar, datepart(year,Getdate())) from mmm_mst_entity where eid =" & HttpContext.Current.Session("EID"), con)
                                Dim dt As DataTable = New DataTable
                                da.Fill(dt)
                                If (dt.Rows.Count > 0) Then
                                    txtBox.Text = Convert.ToString(dt.Rows(0)(0))
                                Else : txtBox.Text = ""
                                End If
                                pnlFields.Controls.Add(txtBox) '-- To Render FY Start End
                            ElseIf DataType = "FY END" Then 'To Render FY End (Pallavi) on 29th Apr 15
                                'Data -field = "datetime"
                                txtBox.Enabled = False
                                'txtBox.ReadOnly = True
                                txtBox.ID = "fld" & ds.Rows(i).Item("FieldID").ToString()
                                '  txtBox.Attributes.Add("data-field", "FYStart")
                                txtBox.Attributes.Add("readonly", "readonly")
                                'readonly
                                Dim da As SqlDataAdapter = New SqlDataAdapter("select replace( convert(varchar(50), dateadd(day,-1, dateadd(year,1 ,convert(datetime, StartMonth +'-' + convert(varchar, datepart(year,Getdate())),106))),106),' ','-') from mmm_mst_entity where eid =" & HttpContext.Current.Session("EID"), con)
                                Dim dt As DataTable = New DataTable
                                da.Fill(dt)
                                If (dt.Rows.Count > 0) Then
                                    txtBox.Text = Convert.ToString(dt.Rows(0)(0))
                                Else : txtBox.Text = ""
                                End If
                                pnlFields.Controls.Add(txtBox) '-- To Render FY End End
                            ElseIf DataType = "SCHEDULER" Then
                                Dim CLNDR As New AjaxControlToolkit.TextBoxWatermarkExtender
                                CLNDR.Controls.Clear()
                                CLNDR.ID = "watermark" & ds.Rows(i).Item("FieldID").ToString()
                                CLNDR.TargetControlID = txtBox.ID
                                CLNDR.WatermarkText = "*|*|*|*|*"
                                CLNDR.Enabled = True
                                pnlFields.Controls.Add(CLNDR)
                            Else
                                Dim KC_Value As String = ds.Rows(i).Item("Cal_Fields").ToString()
                                If ds.Rows(i).Item("Cal_Fields").ToString().Length() > 10 Then
                                    If _CallerPage <> 1 Then
                                        txtBox.Attributes.Add("onblur", ds.Rows(i).Item("Cal_Fields").ToString())
                                    Else
                                        KC_Value = KC_Value.Replace("ContentPlaceHolder1_", "")
                                        txtBox.Attributes.Add("onblur", KC_Value)
                                    End If
                                    If IsDocDetail And Not (IsCallingFromMainHome) Then
                                        KC_Value = KC_Value.Replace("ContentPlaceHolder1_", "")
                                        Dim script = GetScript(HttpContext.Current.Session("Eid").ToString(), ds.Rows(i).Item("Documenttype"), ds.Rows(i).Item("FieldID"))
                                        script = script.Replace("ContentPlaceHolder1_", "")
                                        txtBox.Attributes.Add("onblur", script)
                                    End If
                                End If
                            End If
                            'For Grofers
                            If Convert.ToString(ds.Rows(i).Item("Style")) <> "" Then
                                Dim txt As String() = Convert.ToString(ds.Rows(i).Item("Style")).Split(",")
                                txtBox.Attributes.Add("style", txt(1))
                            Else
                                txtBox.CssClass = "span12 m-wrap"
                            End If
                            pnlFields.Controls.Add(New LiteralControl("</div></div>"))
                    End Select
                Next
            End If
            Dim objDF As New DynamicForm
            objDF.RenderInvisibleField(dsFields, pnlFields)
        Catch ex As Exception

        End Try
    End Sub


    Public Function GetScript(Eid As String, DocumentType As String, FieldID As String) As String
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim da As New SqlDataAdapter("", con)
        Dim script As New StringBuilder()
        Try
            Dim qry = "Select * from mmm_mst_Fields where Eid=" & Eid.ToString & " and Documenttype='" & DocumentType & "'"
            da.SelectCommand.CommandText = qry
            Dim dt As New DataTable()
            da.Fill(dt)
            Dim dtCalfld = Convert.ToString(dt.Select("FieldID='" & FieldID & "'")(0).Item("Cal_text"))

            If dtCalfld.Trim = "" Then
                Return ""
            End If
            Dim arrFormula = dtCalfld.Split(",")

            For i As Integer = 0 To arrFormula.Length - 1
                Dim strFormula = arrFormula(i)
                If strFormula.Trim = "" Then
                    Continue For
                End If
                Dim separators() As String = {"-", "+", "*", "/"}
                Dim LHS = strFormula.Split("=")(0)
                Dim RHS = strFormula.Split("=")(1)
                Dim flds() As String = RHS.Split(separators, StringSplitOptions.RemoveEmptyEntries)

                Dim fldid = ""
                For Each fld In flds
                    If IsNumeric(fld.Replace("}", "").Replace("{", "")) Then
                        RHS = RHS.Replace(fld, fld.Replace("}", "").Replace("{", ""))
                    Else
                        fldid = Convert.ToString(dt.Select("DisplayName='" & fld.Replace("}", "").Replace("{", "") & "'")(0).Item("FieldID"))
                        RHS = RHS.Replace(fld, "parseFloat($('#ContentPlaceHolder1_fld" & fldid & "').val())")
                    End If


                Next

                fldid = Convert.ToString(dt.Select("DisplayName='" & LHS.Replace("}", "").Replace("{", "").Replace("Total ", "") & "'")(0).Item("FieldID"))
                LHS = "parseFloat($('#ContentPlaceHolder1_fld" & fldid & "').val(@@@@)).toPrecision(2);"
                script.Append(LHS.Replace("@@@@", RHS) & "; ")
            Next
            Return script.ToString()
        Catch ex As Exception

        Finally

        End Try
        Return ""
    End Function
    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        TakeItHide_Show()
        FollowUPshowHide()

        If Not IsPostBack Then
            hdnDOCID.Value = Request.QueryString("DOCID")
            BindTicketDocDetail()
        End If
    End Sub
    Protected Sub TakeItHide_Show()
        Dim objDC As New DataClass()
        Dim dt As New DataTable
        dt = objDC.ExecuteQryDT("select * from mmm_doc_view where docid=" & Request.QueryString("DOCID"))
        'Add condition because UserRole might be different from END_USER it's based on configuration part
        Dim _UserRole As String = objDC.ExecuteQryScaller("select ','+isnull(userrole,'')+',' as userrole from mmm_hdmail_schdule where eid=" & HttpContext.Current.Session("EID"))

        Dim _AllowEnableStatusToEndUser As Integer = objDC.ExecuteQryScaller("select Count(AllowEnableStatusToEndUser)  from mmm_hdmail_schdule where documenttype = (select documenttype from mmm_mst_doc with(nolock) where tid =" & Request.QueryString("DOCID") & ") And eid=" & HttpContext.Current.Session("EID") & " And AllowEnableStatusToEndUser=0")

        If dt.Rows.Count > 0 And Session("USERROLE") <> "END_USER" Then
            Takeit.Visible = True
            btnUpdate.Visible = False
            hdnChangeAssignee.Value = "DO NOT CHANGE ASSIGNEE"
        ElseIf _UserRole.ToString().ToUpper.Contains(("," & Session("USERROLE") & ",").ToString().ToUpper) Then
            Takeit.Visible = False
            hdnChangeAssignee.Value = "DO NOT CHANGE ASSIGNEE"
            If _AllowEnableStatusToEndUser > 0 Then
                hdnAllowEnableStatusToEndUser.Value = 1
            Else
                hdnAllowEnableStatusToEndUser.Value = 0
            End If
        ElseIf objDC.ExecuteQryScaller("declare @eid int=0,@documenttype nvarchar(500)='' select @eid=eid,@documenttype=documenttype from mmm_mst_doc where tid=" & Request.QueryString("DOCID") & " select count(*) from mmm_hdmail_schdule where documenttype=@documenttype and eid=@eid and ChangeAssignee=1") <> "0" Then
            Takeit.Visible = False
            btnUpdate.Visible = True

            If objDC.ExecuteQryScaller("select count(*) from mmm_mst_user where eid=" & HttpContext.Current.Session("EID") & " and userrole in(select agentrole from  mmm_hdmail_schdule where eid=" & HttpContext.Current.Session("EID") & ") and uid not in(" & HttpContext.Current.Session("UID") & ") ") <> "0" Then
                ChangeAssigne.Visible = True
                hdnChangeAssignee.Value = "CHANGE ASSIGNEE"
            Else
                ChangeAssigne.Visible = False
                hdnChangeAssignee.Value = "DO NOT CHANGE ASSIGNEE"
            End If
        Else
            hdnChangeAssignee.Value = "DO NOT CHANGE ASSIGNEE"
            Takeit.Visible = False
            btnUpdate.Visible = True
        End If


    End Sub
    Protected Sub BindTicketDocDetail()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim ODA As SqlDataAdapter = New SqlDataAdapter("", con)
        Dim dt As New DataTable()
        Dim dtDocData As New DataTable()
        ODA.SelectCommand.CommandText = "select * from mmm_mst_doc with(nolock) where tid=" & Request.QueryString("DOCID") & " "
        ODA.Fill(dtDocData)
        Dim Documenttype As String = Convert.ToString(dtDocData.Rows(0)("documenttype"))
        ODA.SelectCommand.CommandText = "select * from mmm_mst_fields where documenttype='" & Documenttype & "' and eid=" & Session("EID") & "  and fieldtype<>'Child Item'"
        ODA.Fill(dt)
        If dt.Rows.Count > 0 Then
            FillTicketDocDetail(dt, dtDocData)
        End If
    End Sub
    Public Sub FillTicketDocDetail(dt As DataTable, dtdocdatavalues As DataTable)
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim ODA As SqlDataAdapter = New SqlDataAdapter("", con)
        Dim dtSubject As New DataTable()
        Dim EmailID As String = ""
        Dim Displayname As String = ""
        For Each dr As DataRow In dt.Rows
            If Convert.ToString(dr("MDfieldName")).ToUpper = "SUBJECT" Then
                'ODA.SelectCommand.CommandText = "select  " & Convert.ToString(dr("FieldMapping")) & " from mmm_mst_doc where tid=" & Request.QueryString("DOCID") & ""
                'If con.State = ConnectionState.Closed Then
                '    con.Open()
                'End If
                txtSubject.Text = dtdocdatavalues.Rows(0)(Convert.ToString(dr("FieldMapping")))
                con.Close()
            ElseIf Convert.ToString(dr("MDfieldName")).ToUpper = "REMARKS" Then
                'ODA.SelectCommand.CommandText = "declare @Count int select  @Count=count(tid) from mmm_mst_history where docid=" & Request.QueryString("DOCID") & " if @count=1                 begin	select  " & Convert.ToString(dr("FieldMapping")) & " from mmm_mst_history where docid=" & Request.QueryString("DOCID") & "                 End             Else                 begin DECLARE @Currency varchar(Max) DECLARE @Consolidated_Currency varchar(Max) DECLARE Cur_Cursor CURSOR FOR SELECT " & Convert.ToString(dr("FieldMapping")) & " FROM mmm_mst_history where docid=" & Request.QueryString("DOCID") & " order by tid desc                OPEN Cur_Cursor 	FETCH NEXT FROM Cur_Cursor INTO @Currency 	WHILE @@FETCH_STATUS = 0                 BEGIN	 Set @Consolidated_Currency =ISNULL(@Consolidated_Currency,'') +'<hr>'+ ISNULL(@Currency + ' ','') 	 FETCH NEXT FROM Cur_Cursor INTO  @Currency                 End	 Select Left(@Consolidated_Currency,LEN(@Consolidated_Currency)-1) as [Currency]                 CLOSE Cur_Cursor DEALLOCATE Cur_Cursor end "
                Dim mailbodyAndAttachment As New StringBuilder()
                ODA.SelectCommand.CommandText = " select tid, m." & Convert.ToString(dr("FieldMapping")) & ",u.username, convert(nvarchar,adate,109) as creationdate from mmm_mst_history  as m with(nolock) inner join mmm_mst_user  as u with(nolock) on m.uid=u.uid  where docid=" & Request.QueryString("DOCID") & " order by tid desc"
                Dim dttemp As New DataTable
                ODA.Fill(dttemp)

                For Each drtemp As DataRow In dttemp.Rows
                    mailbodyAndAttachment.Append("      <b>" & drtemp("username") & "</b> <span style=""font-size:11px !important;"">" & drtemp("creationdate") & "</span> <br/><br/>")
                    mailbodyAndAttachment.Append(Convert.ToString(drtemp(1)).Replace("''", "'"))
                    Dim dtAttachment As New DataTable
                    ODA.SelectCommand.CommandText = " declare @col as nvarchar(max) ,@Qry nvarchar(max) select @col= fieldmapping from mmm_mst_fields with(nolock) where documenttype in (select documenttype from mmm_mst_doc_item with(nolock) where  docid=" & Request.QueryString("DOCID") & ") and eid=" & Session("EID") & " and MDfieldName='Attachment' 		 set @Qry='	 select '+  @col +',attachment from mmm_mst_doc_item with(nolock) where sourcetid=" & drtemp("tid") & "' exec sp_executesql @Qry"
                    ODA.Fill(dtAttachment)
                    Dim attachmentcontent As New StringBuilder()
                    If dtAttachment.Rows.Count > 0 Then
                        attachmentcontent.Append("<br/><div  style=""width:auto; height:auto;border:1px solid #ccc; display:inline-block; margin:0px; padding:12px;"">")
                        For Each drattachment As DataRow In dtAttachment.Rows
                            If drattachment(1).ToString().ToUpper.Contains(".PNG") Or drattachment(1).ToString().ToUpper.Contains(".GIF") Or drattachment(1).ToString().ToUpper.Contains(".JPEG") Or drattachment(1).ToString().ToUpper.Contains(".JPG") Or drattachment(1).ToString().ToUpper.Contains(".TIFF") Then
                                attachmentcontent.Append(" <div style=""border:1px solid #ccc; padding:7px; display:inline-block;text-align:center; width:100px;""><img src=""DOCS/" & drattachment(0).ToString() & """ width=""100px"" height=""60px"" alt="" class=""> <input type=""button"" style=""display:block; font-size: 10px; line-height: 10px;  padding: 3px; border: 1px solid #ccc; cursor: pointer; overflow:hidden; width:100%; text-decoration:underline; color: blue; white-space: normal;word-wrap: break-word;"" onclick=""OnChangeCheckbox ('" & "DOCS/" & drattachment(0).ToString() & "')"" value=" & drattachment(1).ToString() & " />  </div>")
                            Else
                                attachmentcontent.Append(" <div style=""border:1px solid #ccc; padding:7px; display:inline-block;text-align:center; width:100px;""><input type=""button"" style=""display:block; font-size: 10px; line-height: 10px;  padding: 3px; border: 1px solid #ccc; cursor: pointer; overflow:hidden; width:100%; text-decoration:underline; color: blue; white-space: normal;word-wrap: break-word;"" onclick=""OnChangeCheckbox ('" & "DOCS/" & drattachment(0).ToString() & "')"" value=" & drattachment(1).ToString() & " />  </div>")
                            End If
                        Next
                        attachmentcontent.Append("</div>")
                        mailbodyAndAttachment.Append(attachmentcontent.ToString() & "<hr/>")
                    Else
                        mailbodyAndAttachment.Append("<hr/>")
                    End If
                Next
                'txtHistory.Text = mailbodyAndAttachment.ToString()
                divHistory.InnerHtml = mailbodyAndAttachment.ToString()
                con.Close()
            ElseIf Convert.ToString(dr("MDfieldName")).ToUpper = "EMAILID" Then
                [TO].InnerText = Convert.ToString(dtdocdatavalues.Rows(0)(Convert.ToString(dr("FieldMapping"))))
            ElseIf Convert.ToString(dr("MDfieldName")).ToUpper = "NAME" Or Convert.ToString(dr("MDfieldName")).ToUpper = "MESSAGEID" Then

            ElseIf Convert.ToString(dr("MDfieldName")).ToUpper = "CC" Then
                txtCC.Text = Convert.ToString(dtdocdatavalues.Rows(0)(Convert.ToString(dr("FieldMapping"))))
            ElseIf Convert.ToString(dr("FieldType")).ToUpper = "DROP DOWN" Then
                Dim ddl As New DropDownList
                ddl = CType(pnlnewfields.FindControl("fld" & dr("FieldID").ToString()), DropDownList)
                If Not IsNothing(ddl) Then
                    ddl.SelectedValue = Convert.ToString(dtdocdatavalues.Rows(0)(Convert.ToString(dr("FieldMapping"))))
                End If

            ElseIf Convert.ToString(dr("FieldType")).ToUpper = "TEXT BOX" Then
                Dim txtBox As New TextBox
                txtBox = CType(pnlnewfields.FindControl("fld" & dr("FieldID").ToString()), TextBox)
                If Not IsNothing(txtBox) Then
                    txtBox.Text = Convert.ToString(dtdocdatavalues.Rows(0)(Convert.ToString(dr("FieldMapping"))))
                End If
            End If
        Next
    End Sub
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Shared Function GetOrganization(DocID As String) As ReturnCollection
        Dim ret As String = ""
        Dim objDC As New DataClass
        Dim objDT As New DataTable()
        Dim returnCollection As New ReturnCollection()
        If HttpContext.Current.Session("USERROLE").ToString().ToUpper = "ADMIN" Then
            If Convert.ToInt32(objDC.ExecuteQryScaller("select count(*) from mmm_mst_doc where tid=" & DocID & " and ticketstatus='SUSPENDED'")) > 0 Then
                objDT = objDC.ExecuteQryDT("select  tid,fld1 as organization from MMM_MST_master where documenttype='Organizations' and eid=" & HttpContext.Current.Session("EID"))
                Dim serializerSettings As New JsonSerializerSettings()
                Dim json_serializer As New JavaScriptSerializer()
                serializerSettings.Converters.Add(New DataTableConverter())

                ret = JsonConvert.SerializeObject(objDT, Newtonsoft.Json.Formatting.None, serializerSettings)
                returnCollection.ds = ret
                returnCollection.StatusData = "SUCCESS"
            Else
                returnCollection.StatusData = "ERROR"
            End If
        ElseIf HttpContext.Current.Session("USERROLE").ToString().ToUpper = "AGENT" Then
            objDT = objDC.ExecuteQryDT("select  tid,fld1 as organization from MMM_MST_master where documenttype='Organizations' and eid=" & HttpContext.Current.Session("EID"))
            Dim serializerSettings As New JsonSerializerSettings()
            Dim json_serializer As New JavaScriptSerializer()
            serializerSettings.Converters.Add(New DataTableConverter())
            ret = JsonConvert.SerializeObject(objDT, Newtonsoft.Json.Formatting.None, serializerSettings)
            returnCollection.ds = ret
            returnCollection.StatusData = "SUCCESS"
            returnCollection.SelectedValForOrgainzations = objDC.ExecuteQryScaller("declare @documenttype nvarchar(200),@query nvarchar(max)='',@colname nvarchar(200)='' select @documenttype=documenttype from mmm_mst_fields where mdfieldname='Domains' and eid=" & HttpContext.Current.Session("EID") & " select @colname=fieldmapping from mmm_mst_fields where documenttype='Ticket' and eid=" & HttpContext.Current.Session("EID") & " and dropdown like  '%-'+@documenttype+'-%' set @query= 'select '+ @colname +'  from mmm_mst_doc where tid=" & DocID & "' exec sp_executesql @query")
        Else
            returnCollection.StatusData = "ERROR"
        End If
        Return returnCollection
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Shared Function GetNewAssignee(OrgID As String, CurrentAssignee As String) As ReturnCollection
        Dim ret As String = ""
        Dim objDC As New DataClass
        Dim objDT As New DataTable()
        Dim returnCollection As New ReturnCollection()
        'Change condition for agents if we want to assign someelse who is agent in this entity
        objDT = objDC.ExecuteQryDT("declare @data nvarchar(max)select @data=fld2 from mmm_mst_master where documenttype='Groups' and eid=" & HttpContext.Current.Session("EID") & " and tid in (select  fld3 from MMM_MST_master where documenttype='Organizations' and eid=" & HttpContext.Current.Session("EID") & " and tid=" & OrgID & ")select username,uid as Userid from mmm_mst_user where uid not in(" & CurrentAssignee & ") and eid=" & HttpContext.Current.Session("EID") & " and userrole in(select agentrole from mmm_hdmail_schdule where eid=" & HttpContext.Current.Session("EID") & ")")
        'union select username,uid as Userid from mmm_mst_user where uid in(select  fld4 from MMM_MST_master where documenttype='Organizations' and eid=" & HttpContext.Current.Session("EID") & " and tid=" & OrgID & ")

        If objDT.Rows.Count > 0 Then
            Dim serializerSettings As New JsonSerializerSettings()
            Dim json_serializer As New JavaScriptSerializer()
            serializerSettings.Converters.Add(New DataTableConverter())
            ret = JsonConvert.SerializeObject(objDT, Newtonsoft.Json.Formatting.None, serializerSettings)
            returnCollection.ds = ret
            returnCollection.StatusData = "SUCCESS"
        Else
            returnCollection.StatusData = "Error"
        End If
        Return returnCollection
    End Function
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Shared Function GetAssignee(DocID As String) As ReturnCollection
        Dim ret As String = ""
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim ODA As SqlDataAdapter = New SqlDataAdapter("", con)
        'This code is working backup purpose

        'If HttpContext.Current.Session("USERROLE").ToString().ToUpper = "AGENT" Then
        '    ODA.SelectCommand.CommandText = "Select uid As uid_Id, username from mmm_mst_user where eid=" & HttpContext.Current.Session("EID") & " And userrole in ('AGENT','Admin')"
        'Else
        '    ODA.SelectCommand.CommandText = "declare @Count int=0 select @Count=count(*) from mmm_doc_view where docid=" & DocID & " if @Count=0         Begin 	  select  uid as uid_Id,username from mmm_mst_user where uid in(  select  top 1 userid from mmm_doc_dtl with(nolock) where docid=" & DocID & " order by tid desc)        End else        Begin 	select   uid as uid_Id,username from mmm_mst_user where uid in (select userid from  mmm_doc_view where docid=" & DocID & ") and eid=" & HttpContext.Current.Session("EID") & "	End"
        'End If
        If HttpContext.Current.Session("USERROLE").ToString().ToUpper = "AGENT" Then
            ODA.SelectCommand.CommandText = "select uid as uid_Id,username from mmm_mst_user with(nolock) where eid=" & HttpContext.Current.Session("EID") & " and uid=" & HttpContext.Current.Session("UID")
            'ElseIf HttpContext.Current.Session("USERROLE").ToString().ToUpper = "ADMIN" Then
            '    ODA.SelectCommand.CommandText = "declare @Qry nvarchar(max),@CountResult1 int set @Qry='select @CountResult1 = (select '+  (select FieldMapping from MMM_MST_FIELDS with(nolock) where eid=" & HttpContext.Current.Session("EID") & " and DocumentType in (select documenttype from mmm_mst_doc with(nolock) where tid =" & DocID & ")  and MDfieldName='Assignee')+' from mmm_mst_doc with(nolock) where tid=" & DocID & ")' exec sp_executesql  @Qry ,N'@CountResult1 int OUTPUT', @CountResult1 = @CountResult1 output select uid as uid_Id,username from mmm_mst_user with(nolock) where uid= @CountResult1"
        Else
            ODA.SelectCommand.CommandText = "declare @Qry nvarchar(max),@CountResult1 int set @Qry='select @CountResult1 = (select '+  (select FieldMapping from MMM_MST_FIELDS with(nolock) where eid=" & HttpContext.Current.Session("EID") & " and DocumentType in (select documenttype from mmm_mst_doc with(nolock) where tid =" & DocID & ")  and MDfieldName='Assignee')+' from mmm_mst_doc with(nolock) where tid=" & DocID & ")' exec sp_executesql  @Qry ,N'@CountResult1 int OUTPUT', @CountResult1 = @CountResult1 output select uid as uid_Id,username from mmm_mst_user with(nolock) where uid= @CountResult1"
        End If
        Dim ds As New DataSet()
        ODA.Fill(ds)
        Dim serializerSettings As New JsonSerializerSettings()
        Dim json_serializer As New JavaScriptSerializer()
        serializerSettings.Converters.Add(New DataTableConverter())
        Dim returnCollection As New ReturnCollection()
        ret = JsonConvert.SerializeObject(ds.Tables(0), Newtonsoft.Json.Formatting.None, serializerSettings)
        returnCollection.ds = ret
        ODA.SelectCommand.CommandText = "declare @Count int=0 select @Count=count(*) from mmm_doc_view with(nolock) where docid=" & DocID & " if @Count=0         Begin 	  select uid from mmm_mst_user with(nolock) where uid in(  select  top 1 userid from mmm_doc_dtl with(nolock) where docid=" & DocID & " order by tid desc)        End else        Begin 	select  uid from mmm_mst_user with(nolock) where uid in (select userid from  mmm_doc_view  with(nolock) where docid=" & DocID & ") and eid=" & HttpContext.Current.Session("EID") & "	End"
        If con.State = ConnectionState.Closed Then
            con.Open()
        End If
        returnCollection.dropdownlist = Convert.ToString(ODA.SelectCommand.ExecuteScalar())
        ODA.SelectCommand.CommandText = "declare @Column nvarchar(max),@Qry nvarchar(max)select @Column=(select fieldmapping from mmm_mst_fields with(nolock) where documenttype in( select documenttype from mmm_mst_doc with(nolock) where tid=" & DocID & ") and eid=" & HttpContext.Current.Session("EID") & " and mdfieldname='STATUS') set @Qry='select '+ @Column+' from mmm_mst_doc with(nolock) where  tid=" & DocID & "'exec sp_executesql @Qry"
        If con.State = ConnectionState.Closed Then
            con.Open()
        End If
        returnCollection.StatusData = Convert.ToString(ODA.SelectCommand.ExecuteScalar())
        con.Close()
        Return returnCollection
    End Function
    Public Class ReturnCollection
        Public Property ds As String
        Public Property dropdownlist As String
        Public Property StatusData As String

        Public Property SelectedValForOrgainzations As String
    End Class

    'Protected Sub btnUpload_Click(sender As Object, e As EventArgs)
    '    If hdnSpan.Value = "" Then
    '        hdnSpan.Value = "0"
    '    Else
    '        hdnSpan.Value = Convert.ToInt32(hdnSpan.Value) + 1
    '    End If
    '    Dim fileCollection As HttpFileCollection = Request.Files
    '    For i As Integer = 0 To fileCollection.Count - 1
    '        Dim uploadfile As HttpPostedFile = fileCollection(i)
    '        Dim fileName As String = Path.GetFileName(uploadfile.FileName)
    '        If uploadfile.ContentLength > 0 Then
    '            Dim ext As String = fileName.Substring(fileName.ToString().LastIndexOf("."), fileName.ToString().Length - fileName.ToString().LastIndexOf("."))
    '            Dim partPath = Session("EID") & "/" & Now.Day & Now.Month & Now.Year & Now.Minute & Now.Second & Now.Millisecond & ext
    '            Dim Path As String = HttpContext.Current.Server.MapPath("DOCS/") & partPath
    '            If Not Directory.Exists(HttpContext.Current.Server.MapPath("DOCS/") & Session("EID")) Then
    '                Directory.CreateDirectory(HttpContext.Current.Server.MapPath("DOCS/") & Session("EID"))
    '            End If
    '            'uploadfilelist.Add(partPath, True, fileName)
    '            uploadfile.SaveAs(Server.MapPath("DOCS/" & partPath))
    '            hdnUploadedFileName.Value = IIf(IsNothing(hdnUploadedFileName.Value), "'" & partPath & "'|'" & fileName & "',", hdnUploadedFileName.Value & "'" & partPath & "'|'" & fileName & "',")
    '            lblMessage.Text += "<span id=""spn" & hdnSpan.Value & """>  <a href=""#""   style=""color:red; border:solid; border-color:gray; font-size:10px; font-family:'Lucida Console';"" onclick=""return DeleteFile('DOCS/" & partPath & "','spn" & hdnSpan.Value & "');"">x</a>" & fileName + " Saved Successfully<br></span>"
    '        End If
    '    Next

    'End Sub
    <System.Web.Services.WebMethod()>
    Public Shared Function UploadFile(ByVal files As IEnumerable(Of HttpPostedFileBase)) As String
        Return "TRUE"

    End Function
    <System.Web.Services.WebMethod()>
    Public Shared Function DeleteFile(deletePathfilename As String) As String
        Dim ret As String = ""
        Dim Path As String = (HttpContext.Current.Server.MapPath(deletePathfilename))
        Dim file As FileInfo = New FileInfo(Path)
        If file.Exists Then
            file.Delete()
            Return "TRUE"
        Else
            Return "FALSE"
        End If
    End Function

    Protected Sub btnTest_Click(sender As Object, e As EventArgs)

    End Sub

    Protected Sub btnFollowup_Click(sender As Object, e As EventArgs)
        'EnableDisableControl(True)
    End Sub
    Public Function EnableDisableControl(Optional ByVal Enable As Boolean = True)
        Dim dt As DataTable
        Dim objDC As New DataClass
        dt = objDC.ExecuteQryDT("SELECT FF.*,F.*  FROM MMM_MST_FIELDS FF With(nolock)  left outer JOIN MMM_MST_FORMS F   With(nolock) on F.FormName = FF.DocumentType and F.EID = FF.EID   where (FF.isactive=1 and  MDfieldName is null) and F.EID=" & Session("EID").ToString() & " and FormName in (select documenttype from mmm_mst_doc with(nolock) where tid=" & Request.QueryString("DOCID") & ") and fieldType<>'Child Item'  order by displayOrder")
        For Each dr As DataRow In dt.Rows
            Dim trgCtrl = Nothing
            Dim ctrl = pnlnewfields.FindControl("fld" & dr("fieldid"))
            If Not ctrl Is Nothing Then
                If ctrl.GetType() Is GetType(System.Web.UI.WebControls.DropDownList) Then
                    trgCtrl = CType(ctrl, DropDownList)
                ElseIf ctrl.GetType() Is GetType(System.Web.UI.WebControls.TextBox) Then
                    trgCtrl = CType(ctrl, TextBox)
                ElseIf ctrl.GetType() Is GetType(System.Web.UI.WebControls.FileUpload) Then
                    trgCtrl = CType(ctrl, FileUpload)
                ElseIf ctrl.GetType() Is GetType(System.Web.UI.WebControls.CheckBoxList) Then
                    trgCtrl = CType(ctrl, CheckBoxList)
                Else
                    trgCtrl = CType(ctrl, CheckBox)
                End If
            End If
            trgCtrl.Enabled = Enable
        Next
    End Function

    Protected Sub lnktakeit_Click(sender As Object, e As EventArgs)
        Dim objDC As New DataClass()
        Dim objDT As New DataTable
        objDT = objDC.ExecuteQryDT("select * from mmm_doc_View where docid=" & Request.QueryString("DOCID"))
        If objDT.Rows.Count = 0 Then
            btnUpdate.Visible = False
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "popup", "alert('This ticket already taken by someone other Please take another ticket');", True)
        Else
            objDC.ExecuteQryDT("delete from  mmm_doc_View where docid=" & Request.QueryString("DOCID"))
            objDC.ExecuteQryDT("insert into mmm_doc_dtl (userid,docid,fdate,ptat,atat,pathID,Ordering,DocNature,lastupdate) values(" & HttpContext.Current.Session("UID") & "," & Request.QueryString("DOCID") & ",getdate(),0,0,0,1,'CREATE',getdate()) declare @lastTID int Select @lastTID=@@IDENTITY  update mmm_mst_doc set lasttid= @lastTID where tid= " & Request.QueryString("DOCID") & "")
            TakeItHide_Show()
        End If

    End Sub
    Protected Sub lnkAssigned_Click(sender As Object, e As EventArgs)
        ScriptManager.RegisterStartupScript(Me, Me.GetType(), "popup", "alert('This ticket has been successfully assigned');", True)
    End Sub
End Class
