Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports System.Drawing
Imports System.Net
Imports System.Xml
Imports System.Net.Security
Imports System.Net.HttpWebRequest
Imports System.Net.HttpWebResponse
Imports System.Security.Cryptography.X509Certificates
Imports System.Web.Services
Imports Newtonsoft.Json.Linq
Imports Newtonsoft.Json

Imports System.Web.Script.Serialization
Imports Newtonsoft.Json.Converters
Imports AjaxControlToolkit
Imports System.Threading


Partial Class SubmitTicket
    Inherits System.Web.UI.Page
    Public _CallerPage As Integer


    Protected Sub ValidateData()
        Dim EID As Integer = 0
        EID = Convert.ToInt32(Session("Eid"))
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        'Tr
        Dim tran As SqlTransaction = Nothing
        Dim con As SqlConnection = New SqlConnection(conStr)
        con.Open()
        tran = con.BeginTransaction()
        Dim screenname As String = "Ticket" ' Request.QueryString("SC").ToString()
        Dim da As SqlDataAdapter = New SqlDataAdapter("SELECT FF.*,F.*  FROM MMM_MST_FIELDS FF left outer JOIN MMM_MST_FORMS F on F.FormName = FF.DocumentType and F.EID = FF.EID where (FF.isactive=1 or Fieldtype='auto number' or fieldtype='New Auto Number') and F.EID=" & Session("EID").ToString() & " and FormName = '" & screenname & "' order by displayOrder", con)
        da.SelectCommand.Transaction = tran
        Dim ds As New DataSet
        da.Fill(ds, "fields")
        Dim ob As New DynamicForm
        Dim FinalQry As String
        Dim msgAN As String = ""
        Dim HeaderConfirmation As String = IIf(Convert.IsDBNull(ds.Tables("fields").Rows(0)("HeaderConfirmation")) = True, "Confirmation", Convert.ToString(ds.Tables("fields").Rows(0)("HeaderConfirmation")))
        Dim MsgConfirmation As String = IIf(Convert.IsDBNull(ds.Tables("fields").Rows(0)("ConfirmationMessage")) = True, "System DOC ID is @docid  @msg ", Convert.ToString(ds.Tables("fields").Rows(0)("ConfirmationMessage")))
        Dim DocID As Integer = 0
        Dim childvalidation As String = ""
        ' here is the code for calculation inline grid 
        'Dim childDt() As DataRow = ds.Tables("fields").Select("fieldtype='child item'")
        'For K As Integer = 0 To childDt.Length - 1
        '    CalculateGRid(screenname, childDt(K).Item("fieldid").ToString())
        'Next
        'Code for Advance formula execution Dated:18-Feb-2015 added By Ajeet Kumar
        'Code For Rule Engine By Ajeet Kumar Dated :30-july-2014
        Dim dv As DataView = ds.Tables("fields").DefaultView
        dv.RowFilter = "IsActive=1"
        Dim theFields As DataTable = dv.ToTable
        Dim lstData As New List(Of UserData)
        'Creating collection for rule engine execution
        Dim obj As New DynamicForm()
        lstData = obj.CreateCollection(pnlnewfields, theFields)
        'Setting it to session for getting it's value for child Item validation
        Session("pColl") = lstData
        'Creating object of rule response
        Dim AdvanceFormula() As DataRow = ds.Tables("fields").Select("Fieldtype='Advance Formula'")
        Dim ObjFormula As New FormulaEngine()
        Dim AdFormula As String = ""
        If AdvanceFormula.Length > 0 Then
            For Each AFField As DataRow In AdvanceFormula
                Dim FieldID = AFField.Item("FieldID")
                ObjFormula.EID = Session("EID")
                ObjFormula.FormulaFIeld = FieldID
                AdFormula = AFField.Item("Advance_formula")
                Dim val = ObjFormula.ExecuteFormula(AdFormula, Session("EID"), lstData, Nothing, False)
                Dim fField As New TextBox()
                fField = CType(pnlnewfields.FindControl("fld" & FieldID), TextBox)
                If (Not fField Is Nothing) Then
                    fField.Text = Convert.ToString(val)
                    lstData = obj.CreateCollection(pnlnewfields, theFields)
                End If
                'forvalue = formulaeditorr.ExecuteFormula(CField.Item("KC_LOGIC"), DocID, "v" + Session("eid").ToString + viewdoc, Session("eid").ToString, 0)
            Next
        End If
        'End here
        Try

            FinalQry = ob.ValidateAndGenrateQueryForControls("ADD", "INSERT INTO MMM_MST_DOC(EID,Documenttype,oUID,adate,", "VALUES (" & Session("EID").ToString() & ",'" & screenname & "'," & Session("UID").ToString() & ",getdate(),", ds.Tables("fields"), pnlnewfields, 0)
            If Trim(Left(FinalQry, 6)).ToUpper() = "PLEASE" Then
                lblTab.Text = FinalQry
            Else
                '' for checking the value of allowed size if file is valid or not 
                If IsDBNull(ds.Tables("fields").Rows(0).Item("AttahcedFileSize")) = True Or Val(ds.Tables("fields").Rows(0).Item("AttahcedFileSize").ToString()) = 0 Then
                Else
                    If Session("FileSize") > Val(Val(ds.Tables("fields").Rows(0).Item("AttahcedFileSize").ToString()) * 1024 * 1024) Then
                        lblTab.Text = "Please note your attachment cannot be uploaded as it exceeds prescribed limit. Please ensure that file size does not exceed '" & ds.Tables("fields").Rows(0).Item("attahcedfilesize").ToString() & "' MB and try again"
                        Session("FileSize") = Nothing
                        Exit Sub
                    End If
                End If

                ''Code For Rule Engine By Ajeet Kumar Dated :30-july-2014
                ''Creating object of rule response
                'Dim ObjRet As New RuleResponse()
                'Dim errorlist As New ArrayList

                'Dim dtruleD As New DataTable

                ' '' new for duplicacy check rule - enhancement
                'Dim Ruleduplicacy As New RuleEngin(Session("EID"), screenname, "CREATED", "DUPLICACY")
                'Dim DSDuplicacyRule As DataSet = Ruleduplicacy.GetRules()
                'dtruleD = DSDuplicacyRule.Tables(1)
                'For Each dr As DataRow In DSDuplicacyRule.Tables(0).Rows
                '    ObjRet = Ruleduplicacy.ExecuteRule(lstData, Nothing, False, screenname, dr, dtruleD)
                '    If ObjRet.Success = False Then
                '        errorlist.Add(Convert.ToString(ObjRet.ErrorMessage))
                '    End If
                'Next
                'If errorlist.Count > 0 Then
                '    lblTab.Text = String.Join("<br/>", errorlist.ToArray())
                '    lblTab.ForeColor = Color.Red
                '    Exit Sub
                'End If
                ' '' new for duplicacy check rule - enhancement

                ''Initialising rule Object
                'Dim ObjRule As New RuleEngin(Session("EID"), screenname, "CREATED", "SUBMIT")
                ''Uncomment
                'Dim dsrule As DataSet = ObjRule.GetRules()
                'Dim dtrule As New DataTable
                'dtrule = dsrule.Tables(1)


                'For Each dr As DataRow In dsrule.Tables(0).Rows
                '    If dr("ruledesc").ToString().Contains("TNE") Then
                '        ObjRet = ObjRule.ExecuteRuleTNE(lstData, Nothing, False, screenname, dr, dtrule)
                '    Else
                '        ObjRet = ObjRule.ExecuteRule(lstData, Nothing, False, screenname, dr, dtrule)
                '    End If
                '    'ObjRet = ObjRule.ExecuteRule(lstData, Nothing, False, screenname, dr, dtrule)
                '    If ObjRet.Success = False Then
                '        errorlist.Add(Convert.ToString(ObjRet.ErrorMessage))
                '    End If
                'Next
                'If errorlist.Count > 0 Then
                '    lblTab.Text = String.Join("<br/>", errorlist.ToArray())
                '    lblTab.ForeColor = Color.Red
                '    Exit Sub
                'End If

                'For Each dr As DataRow In dsrule.Tables(0).Rows
                '    ObjRet = ObjRule.ExecuteRule(lstData, Nothing, False, screenname, dr, dtrule)
                '    If ObjRet.Success = False Then
                '        lblTab.Text = ObjRet.ErrorMessage
                '        Exit Sub
                '    End If
                'Next


                'validation for child item inline item 
                Dim validatechilditem() As DataRow = ds.Tables("fields").Select("Fieldtype='CHILD ITEM'")
                'If validatechilditem.Length > 0 Then
                '    'For Each DR As DataRow In validatechilditem
                '    '    '' new added for saving differently if def. value feature is on
                '    '    Dim strDF As String = "select * from mmm_mst_forms where formname='" & DR.Item("DROPDOWN") & "' and formsource='DETAIL FORM' and EID=" & Session("EID") & " and (isnull(HasDefaultValue,0)=1  or isnull(isinlineediting,0)=1 or isnull(IsDefaultRows,0)>0)"
                '    '    Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
                '    '    Dim dtIsdv As New DataTable
                '    '    oda.SelectCommand.CommandText = strDF
                '    '    oda.SelectCommand.Transaction = tran
                '    '    oda.Fill(dtIsdv)
                '    '    If dtIsdv.Rows.Count > 0 Then
                '    '        childvalidation = ValidatingChildItem_DV(DR.Item("DROPDOWN"))
                '    '    End If
                '    '    If childvalidation.Length > 5 Then
                '    '        lblTab.Text = childvalidation
                '    '        Exit Sub
                '    '    End If
                '    'Next
                'End If
                '' for saving child item (attachment) by sp on may-18

                'chidvalidation = ValidatingChildItem_DV(DR.Item("DROPDOWN"), fileid)
                FinalQry = FinalQry & ";Select @@identity"
                'save the data
                lblTab.Text = ""
                da.SelectCommand.CommandText = FinalQry
                Dim fileid As Integer = da.SelectCommand.ExecuteScalar()
                DocID = fileid
                Session("docid") = fileid
                
                '' change from here by sunil for new auto number 03_Aug_14
                Dim row As DataRow() = ds.Tables("fields").Select("Fieldtype='Auto Number' or Fieldtype='New Auto Number'")
                If row.Length > 0 Then
                    For I As Integer = 0 To row.Length - 1
                        Select Case row(I).Item("fieldtype").ToString
                            Case "Auto Number"
                                da.SelectCommand.Parameters.Clear()
                                da.SelectCommand.Transaction = tran
                                da.SelectCommand.CommandText = "usp_GetAutoNoNew"
                                da.SelectCommand.CommandType = CommandType.StoredProcedure
                                da.SelectCommand.Parameters.AddWithValue("Fldid", row(I).Item("fieldid"))
                                da.SelectCommand.Parameters.AddWithValue("docid", fileid)
                                da.SelectCommand.Parameters.AddWithValue("fldmapping", row(I).Item("fieldmapping"))
                                da.SelectCommand.Parameters.AddWithValue("FormType", "Document")
                                Dim an As String = da.SelectCommand.ExecuteScalar()
                                msgAN = "<br/> " & row(I).Item("displayname") & " is " & an & ""
                                da.SelectCommand.Parameters.Clear()
                            Case "New Auto Number"
                                da.SelectCommand.Parameters.Clear()
                                da.SelectCommand.CommandText = "usp_GetAutoNoNew_New"
                                da.SelectCommand.CommandType = CommandType.StoredProcedure
                                da.SelectCommand.Parameters.AddWithValue("Fldid", row(I).Item("fieldid"))
                                da.SelectCommand.Parameters.AddWithValue("SearchFldid", row(I).Item("dropdown").ToString)
                                da.SelectCommand.Parameters.AddWithValue("docid", fileid)
                                da.SelectCommand.Parameters.AddWithValue("fldmapping", row(I).Item("fieldmapping"))
                                da.SelectCommand.Parameters.AddWithValue("FormType", "Document")
                                Dim an As String = da.SelectCommand.ExecuteScalar()
                                msgAN = "<br/> " & row(I).Item("displayname") & " is " & an & ""
                                da.SelectCommand.Parameters.Clear()
                        End Select
                    Next
                End If


                'Dim df As New DynamicForm()
                '' change from here by sunil for new auto number 03_Aug_14

                Dim childitem() As DataRow = ds.Tables("fields").Select("Fieldtype='CHILD ITEM'")
                If childitem.Length > 0 Then

                    '    For Each DR As DataRow In childitem
                    '        '' new added for saving differently if def. value feature is on
                    '        '' by sunil for def value 13-jan-14 - starts
                    '        'aaaa
                    '        Dim strDF As String = "select * from mmm_mst_forms where formname='" & DR.Item("DROPDOWN") & "' and formsource='DETAIL FORM' and EID=" & Session("EID") & " and (isnull(HasDefaultValue,0)=1 or isnull(isinlineediting,0)=1 or isnull(IsDefaultRows,0)>0)"
                    '        Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
                    '        Dim dtIsdv As New DataTable
                    '        oda.SelectCommand.CommandText = strDF
                    '        oda.SelectCommand.Transaction = tran
                    '        oda.Fill(dtIsdv)
                    '        If dtIsdv.Rows.Count = 0 Then
                    '            'If Not Request.QueryString("TID") Is Nothing Then
                    '            '    oda.SelectCommand.CommandText = "InsertDocItemFromDraftItem"
                    '            '    oda.SelectCommand.CommandType = CommandType.StoredProcedure
                    '            '    oda.SelectCommand.Parameters.AddWithValue("@eid", Session("EID"))
                    '            '    oda.SelectCommand.Parameters.AddWithValue("@docid", Session("docid"))
                    '            '    oda.SelectCommand.Parameters.AddWithValue("@draftid", Request.QueryString("TID").ToString())
                    '            '    oda.SelectCommand.Transaction = tran
                    '            '    oda.SelectCommand.ExecuteNonQuery()
                    '            'Else
                    '            '    SavingChildItem(DR.Item("DROPDOWN"), fileid, con, tran)
                    '            'End If
                    '            SavingChildItem(DR.Item("DROPDOWN"), fileid, con, tran)
                    '        Else
                    '            SavingChildItem_DV(DR.Item("DROPDOWN"), fileid, con, tran)
                    '        End If
                    '        'here for checking and updating parent value field in child items. ' 22_apr_15

                    '        Try
                    '            Dim FormName As String = DR.Item("DROPDOWN")

                    '            EID = Convert.ToInt32(Session("EID"))
                    '            If (fileid > 0) And (FormName <> "") Then
                    '                'Last parameter is for child item
                    '                Trigger.ExecuteTriggerT(FormName, EID, fileid, con, tran, 1)
                    '            End If
                    '        Catch ex As Exception
                    '            Throw
                    '        End Try
                    '    Next
                End If


                Dim ChildItemColumn As String = "fld2"
                Dim ChildItemDocumentType As String = "Attachments"
                If ChildItemColumn <> "" Then
                    Dim splittedvalues As String() = hdnUploadedFileName.Value.Split(",")
                    If splittedvalues(0) <> String.Empty Then
                        For x As Integer = 0 To splittedvalues.Length - 1
                            'Try
                            Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
                            oda.SelectCommand.Transaction = tran
                            Dim tempFileName As String() = splittedvalues(x).ToString().Split("|")
                            Dim tempPath As String = Server.MapPath("~/DOCS/temp/" & tempFileName(0).ToString())
                            oda.SelectCommand.CommandText = "insert into TicketAccessFile values(" & DocID & ",'" & tempPath & "',getdate()),(" & DocID & ",'" & Server.MapPath("~/DOCS/") & Session("EID") & "/" & tempFileName(0) & "',getdate())"
                            If con.State = ConnectionState.Closed Then
                                con.Open()
                            End If
                            oda.SelectCommand.ExecuteNonQuery()
                            If Not Directory.Exists(Server.MapPath("~/DOCS/") & Session("EID")) Then
                                Directory.CreateDirectory(Server.MapPath("~/DOCS/") & Session("EID"))
                            End If
                            File.Copy(tempPath, Server.MapPath("~/DOCS/") & Session("EID") & "/" & tempFileName(0))
                            oda.SelectCommand.CommandText = "insert into mmm_mst_doc_item (DOCID,DOCUMENTTYPE,ISAUTH,LASTUPDATE,cmastertid," & ChildItemColumn & ",Attachment) values(" & DocID & ",'" & ChildItemDocumentType & "',1,getdate(),0,'" & Session("EID") & "/" & tempFileName(0) & "','" & tempFileName(1).ToString() & "')"
                            oda.SelectCommand.Transaction = tran
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


                Dim ob1 As New DMSUtil()

                Dim viewdoc As String = screenname
                viewdoc = viewdoc.Replace(" ", "_")

                '' insert default fiest movement of document - by sunil
                da.SelectCommand.CommandText = "InsertDefaultMovement"
                da.SelectCommand.CommandType = CommandType.StoredProcedure
                da.SelectCommand.Parameters.Clear()
                da.SelectCommand.Parameters.AddWithValue("tid", fileid)
                da.SelectCommand.Parameters.AddWithValue("CUID", Val(Session("UID").ToString()))
                da.SelectCommand.Parameters.AddWithValue("what", "UPLOADED")
                da.SelectCommand.ExecuteNonQuery()
                '' here code to approve document  - by sunil

                'Comman Code
                Dim res As String
                Dim objDC As New DataClass
                Dim objDTName As New DataTable
                'Comman Code


                objDTName = objDC.ExecuteQryDT("select RolematrixfromOrganization,Ticket_Assign_Method,AdminRole,AgentRole,Userrole from mmm_hdmail_schdule where eid=" & Session("EID") & "and documenttype='" & screenname & "'")
                If objDTName.Rows.Count > 0 Then
                    If objDTName.Rows(0)(0).ToString().ToUpper = "TRUE" Then

                        Dim tc As New TicketScheduler
                        'changes for Role ,You must assure role should be comming from configuration database

                        If ("," & Session("USERROLE").ToString().ToUpper & ",").ToString().Contains("," & Convert.ToString(objDTName.Rows(0)("AgentRole")).ToUpper() & ",") Then
                            Dim ddl As New DropDownList
                            ddl = CType(pnlnewfields.FindControl("fld" & objDC.ExecuteQryScaller("select fieldid from mmm_mst_fields where documenttype='" & screenname & "' and  dropdown='STATIC-USER-UserName' and eid=" & Session("EID") & "")), DropDownList)
                            Dim EmailID As String = objDC.ExecuteQryScaller("select emailid from mmm_mst_user where uid=" & ddl.SelectedValue)
                            Dim ddlSubCategory As New DropDownList
                            ddlSubCategory = CType(pnlnewfields.FindControl("fld" & objDC.ExecuteQryScaller("select fieldid from mmm_mst_fields where documenttype='" & screenname & "' and parsefromemail=1 and eid=" & Session("EID") & "")), DropDownList)
                            If Not IsNothing(ddl) Then
                                res = tc.GetNextUserFromOrganizatios(EmailID, Session("EID"), Convert.ToString(objDTName.Rows(0)("AdminRole")), ddlSubCategory.SelectedValue)
                                tc.AssignTicketToUserBasedOnCondition(res, fileid, "", Val(ddl.SelectedValue), con, tran, Session("EID"), objDTName.Rows(0)(0), objDTName.Rows(0)(1), screenname, "CREATE")
                                objDC.TranExecuteQryDT("Update mmm_mst_doc set " & objDC.ExecuteQryScaller("select fieldmapping from mmm_mst_fields where documenttype='Ticket' and eid=" & Session("EID") & " and mdfieldname='EmailID'") & "='" & EmailID & "'," & objDC.ExecuteQryScaller("select fieldmapping from mmm_mst_fields where documenttype='Ticket' and eid=" & Session("EID") & " and mdfieldname='Name'") & "='" & ddl.SelectedValue & "',  ouid=" & ddl.SelectedValue & " where tid=" & fileid, con, tran)
                            Else
                                res = tc.GetNextUserFromOrganizatios(EmailID, Session("EID"), Session("USERROLE"))
                                tc.AssignTicketToUserBasedOnCondition(res, fileid, "", Val(Session("UID").ToString()), con, tran, Session("EID"), objDTName.Rows(0)(0), objDTName.Rows(0)(1), screenname, "CREATE")
                            End If
                        Else
                            Dim txtBox As New TextBox
                            txtBox = CType(pnlnewfields.FindControl("fld" & objDC.ExecuteQryScaller("select fieldid from mmm_mst_fields where documenttype='" & screenname & "' and MDfieldName='Emailid' and eid=" & Session("EID") & "")), TextBox)
                            Dim ddl As New DropDownList
                            ddl = CType(pnlnewfields.FindControl("fld" & objDC.ExecuteQryScaller("select fieldid from mmm_mst_fields where documenttype='" & screenname & "' and parsefromemail=1 and eid=" & Session("EID") & "")), DropDownList)

                            If Not IsNothing(ddl) Then
                                res = tc.GetNextUserFromOrganizatios(txtBox.Text, Session("EID"), Convert.ToString(objDTName.Rows(0)("AdminRole")), ddl.SelectedValue)
                                tc.AssignTicketToUserBasedOnCondition(res, fileid, "", Val(Session("UID").ToString()), con, tran, Session("EID"), objDTName.Rows(0)(0), objDTName.Rows(0)(1), screenname, "CREATE")
                            Else
                                res = tc.GetNextUserFromOrganizatios(txtBox.Text, Session("EID"), Session("USERROLE"))
                                tc.AssignTicketToUserBasedOnCondition(res, fileid, "", Val(Session("UID").ToString()), con, tran, Session("EID"), objDTName.Rows(0)(0), objDTName.Rows(0)(1), screenname, "CREATE")
                            End If
                        End If
                    Else
                        res = ob1.GetNextUserFromRolematrixT(fileid, Val(Session("EID").ToString()), Val(Session("UID").ToString()), "", Val(Session("UID").ToString()), con, tran)
                    End If
                Else
                    res = ob1.GetNextUserFromRolematrixT(fileid, Val(Session("EID").ToString()), Val(Session("UID").ToString()), "", Val(Session("UID").ToString()), con, tran)
                End If

                'res = ob1.GetNextUserFromRolematrixT(fileid, Val(Session("EID").ToString()), Val(Session("UID").ToString()), "", Val(Session("UID").ToString()), con, tran)
                '' here code to approve document  - by sunil
                Dim sretMsgArr() As String = res.Split(":")
                'Code is commented by Ajeet Kumar becouse it will no longer be used Dated:20 August 2014
                'If ds.Tables("fields").Rows(0).Item("iscalendar").ToString = "1" And Session("dtnew") <> Nothing Then
                '    ADDTASK(fileid, screenname)
                'End If
                ob.CLEARDYNAMICFIELDS(pnlnewfields)
                ''INSERT INTO HISTORY 
                ob.HistoryT(Session("EID"), fileid, Session("UID"), screenname, "MMM_MST_DOC", "ADD", con, tran)
                '' new added for pusing data to master on docuement creation by sp 21-Jan-14
                If sretMsgArr(0) = "ARCHIVE" Then
                    Dim Op As New Exportdata()
                    Op.PushdataT(fileid, sretMsgArr(1), Session("EID"), con, tran)
                End If

                '''' check if no skip setting and if not allowed then don't move doc and show msg to user by sunil on 07-Oct
                If sretMsgArr(0).ToUpper() = "NO SKIP" Then
                    Dim Noskipmsg As String = "Next Approvar/User not found, please contact Admin"
                    'lblMsg.Text = "System Docid is " & fileid & " " & msgAN & "" & "<br/> " & Noskipmsg
                    'this code block is added by ajeet kumar for transaction to be rolled back
                    tran.Rollback()
                    lblTab.Text = Noskipmsg
                    Exit Sub
                Else
                    'If screenname.Trim.ToUpper = "VENDOR INVOICE VP" And Session("EID") = "46" Then
                    '    lblMsg.Text = "System DOC ID is " & fileid & " (For HCL Internal use) " & msgAN & " (For Vendors)"
                    'Else
                    '    lblMsg.Text = "System DOC ID is " & fileid & " " & msgAN & ""
                    'End If
                    If Not String.IsNullOrEmpty(msgAN) Then
                        lblMsg.Text = MsgConfirmation.Replace("@docid", fileid).Replace("@msg", msgAN)
                        panelHeaderConfimation.InnerText = HeaderConfirmation
                    Else
                        lblMsg.Text = MsgConfirmation.Replace("@docid", fileid).Replace("@msg", "")
                        panelHeaderConfimation.InnerText = HeaderConfirmation
                    End If


                End If
                ViewState("viewdocid") = fileid
                Try
                    Dim FormName As String = screenname
                    '     Dim EID As Integer = 0
                    EID = Convert.ToInt32(Session("EID"))
                    If (DocID > 0) And (FormName <> "") Then
                        Trigger.ExecuteTriggerT(FormName, EID, DocID, con, tran)
                    End If
                Catch ex As Exception
                    Throw
                End Try
                'Commiting transaction 
                'Code Added By Ajeet For Period Wise Balance Date:04-Feb-2015
                ''Try
                ''    Dim objRel As New Relation()
                ''    objRel.ExecutePeriodWiseBalance(EID, DocID, screenname, con, tran)
                ''Catch ex As Exception

                'End Try
                'Again Commit Commiting transaction

                'Add Code for Rate Card master
                'Dim custFields As String = ""
                'custFields = objDC.TranExecuteQryScaller("declare @Qry nvarchar(max) set @Qry=' select '+ (select ''+isnull(fieldMapping,0)+'' from MMM_MST_FIELDS where DocumentType='" & screenname & "' and eid=" & EID & " and IsRateCard=1)+'  from mmm_mst_doc where tid=" & DocID & "' exec sp_executesql @Qry ", con:=con, tran:=tran)
                'If custFields <> "" Then
                '    objDC.TranExecuteQryDT("if exists(select * from mmm_Ratecard_Vendor where ouid=" & Session("UID") & " and eid=" & EID & ")	begin		 insert into mmm_mst_RateCardVendorMaster(RefRateCardID,RefTable,RateCardDescription,ROI,ROIDays,CreateOn,oUID,docid) select RateCardID,'mmm_Ratecard_Vendor',RateCardDescription,ROI,ROIDays,getdate(),ouid," & DocID & " from mmm_Ratecard_Vendor where RateCardID in  (select * from [DMS].InputString('" & custFields & "'))	end else 	Begin 		 insert into mmm_mst_RateCardVendorMaster(RefRateCardID,RefTable,RateCardDescription,ROI,ROIDays,CreateOn,oUID,docid) select RateCardID,'mmm_Ratecard_master',RateCardDescription,ROI,ROIDays,getdate(),0," & DocID & " from mmm_Ratecard_master where RateCardID in (select * from [DMS].InputString('" & custFields & "')) 	End", con:=con, tran:=tran)
                'End If
                ''Add Code for Rate Card master



                'tran.Rollback()
                tran.Commit()


                ' below code used for calling webservice...

                '' code to send mail to first approver on document creation and to document owner also
                '' new added by sunil for mail sending 
                'Non transactional Query
                ob1.TemplateCalling(fileid, Session("EID"), screenname, "CREATED")
                da.SelectCommand.CommandType = CommandType.Text
                da.SelectCommand.Parameters.Clear()
                da.SelectCommand.CommandText = "select isworkflow from mmm_mst_doc where TID=" & fileid & " and eid='" & Session("EID") & "'"
                Dim dt As New DataTable
                da.Fill(dt)
                If dt.Rows.Count = 1 Then
                    If dt.Rows(0).Item(0).ToString = "1" Then
                        ob1.TemplateCalling(fileid, Session("EID"), screenname, "APPROVE")
                    End If
                End If

                'Try
                '    If ds.Tables("fields").Rows(0).Item("enablews").ToString = "1" Then
                '        'Dim ws As New WSOutward()
                '        'Dim URLIST As String = ws.WBS(screenname, Session("EID"), DocID)
                '        'lblMsg.Text = lblMsg.Text & Environment.NewLine & URLIST.ToString()
                '    End If
                'Catch ex As Exception
                'End Try
                ''Code For Schedule document. By Ajeet Kumar.Date:04-June-2015
                'Try
                '    Dim objS As New ScheduleDocument(EID, screenname, "save")
                '    objS.CreateDynamicDocument(fileid)
                'Catch ex As Exception

                'End Try


                'Try
                '    Dim objRel As New Relation()
                '    Dim CreatedDocType As String, UID As Integer
                '    EID = Convert.ToUInt32(Session("EID"))
                '    UID = Convert.ToUInt32(Session("UID"))
                '    Dim objRes As New RelationResponse()
                '    CreatedDocType = screenname
                '    DocID = fileid
                '    objRes = objRel.ExtendRelation(EID, CreatedDocType, DocID, UID, "", False)
                '    'In case of costom rule Creation

                '    'Added By mayank in case of variation configuration 20-Feb-2015
                '    Dim dtvarn As New DataTable

                '    'Resolve after query 
                '    'Dim conStrng As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
                '    'Dim con1 As SqlConnection = New SqlConnection(conStr)
                '    Dim dtvar As New DataTable
                '    Dim qry As String
                '    qry = "select fieldmapping,dropdown from mmm_mst_fields where eid=" & Session("EID") & " and documenttype='" & CreatedDocType & "' and fieldType='Variance'"
                '    Dim da1 As SqlDataAdapter = New SqlDataAdapter("", con)
                '    da1.SelectCommand.Transaction = tran
                '    da1.SelectCommand.CommandText = qry
                '    da1.SelectCommand.ExecuteNonQuery()
                '    da1.Fill(dtvar)
                '    Dim li As New ArrayList
                '    Dim liupdate As New ArrayList
                '    If (dtvar.Rows.Count > 0) Then
                '        For i As Integer = 0 To dtvar.Rows.Count - 1
                '            li.Add(dtvar.Rows(i).Item("dropdown").ToString())
                '            liupdate.Add(dtvar.Rows(i).Item("fieldmapping").ToString())
                '        Next
                '        'Dim i As Integer = 0
                '        'Do While (i - dtvar.Rows.Count - 1)
                '        '    li.Add(dtvar.Rows(i).Item("dropdown").ToString())
                '        '    i = (i + 1)
                '        'Loop
                '        Dim finalstr = String.Join(",", li.ToArray())
                '        Dim finalUpdate = String.Join(",", liupdate.ToArray())

                '        Dim ODAs As SqlDataAdapter = New SqlDataAdapter("", con)


                '        Dim DSvar As New DataSet
                '        ODAs.SelectCommand.Parameters.Clear()
                '        ODAs.SelectCommand.Transaction = tran
                '        ODAs.SelectCommand.CommandText = "uspGetHistoryDetails"
                '        ODAs.SelectCommand.CommandType = CommandType.StoredProcedure
                '        ODAs.SelectCommand.Parameters.AddWithValue("eid", Session("EID"))
                '        ODAs.SelectCommand.Parameters.AddWithValue("docid", Session("docid"))
                '        ODAs.SelectCommand.Parameters.AddWithValue("ParString", finalstr.ToString())
                '        DSvar.Tables().Clear()
                '        ODAs.Fill(DSvar, "variance")
                '        Dim str As New StringBuilder()
                '        str.Append("Update mmm_mst_doc set ")
                '        If (DSvar.Tables("variance2").Rows(0)(0).ToString() = "1") Then
                '            For Val As Integer = 0 To li.Count - 1
                '                str.Append(liupdate(Val).ToString() & "='" & DSvar.Tables("variance1").Rows(0)(Val).ToString() & "',")
                '            Next
                '            str.Remove(str.Length - 1, 1)
                '            str.Append("where tid=" & Session("docid") & " and eid=" & Session("EID") & "")
                '        Else
                '            For Val As Integer = 0 To li.Count - 1
                '                Dim d As Decimal
                '                d = Convert.ToDecimal(DSvar.Tables("variance1").Rows(0)(Val)) - Convert.ToDecimal(DSvar.Tables("variance").Rows(0)(Val))
                '                str.Append(liupdate(Val).ToString() & "='" & d.ToString() & "',")
                '            Next
                '            str.Remove(str.Length - 1, 1)
                '            str.Append("where tid=" & Session("docid") & " and eid=" & Session("EID") & "")
                '        End If

                '        Dim updateqry As String
                '        updateqry = str.ToString()
                '        Dim dts As New DataTable
                '        Dim da2 As SqlDataAdapter = New SqlDataAdapter("", con)
                '        da2.SelectCommand.Transaction = tran
                '        da2.SelectCommand.CommandText = updateqry
                '        da2.Fill(dts)
                '    End If

                '    'Added By mayank in case of variation configuration  20-Feb-2015

                '    If objRes.Success = True And objRes.ShowExtend = True Then
                '        Response.Redirect("ExtendRelation.aspx?DOCID=" & DocID & "&SC=" & CreatedDocType)
                '    End If

                'Catch ex As Exception
                'End Try
                'Code for UploadFile On FTP Server
                Dim objOCR As New OCR(EID, DocID)
                objOCR.SendToFTP()
                'Code for UploadFile On FTP Server

                'Code for Discounting Start Here
                'If Convert.ToString(objDC.ExecuteQryScaller("declare @Col nvarchar(200)=''declare @dynQuery nvarchar(max)select @Col=isnull(isDiscountingMapping,'') from mmm_mst_entity where  eid=" & EID & " if @Col<>''begin	 set @dynQuery='select '+ @Col+ ' from mmm_mst_doc where tid=" & DocID & "'	 exec sp_executesql @dynQuery End else 	begin select 'NO' 	End")).ToUpper = "YES" Then
                '    Response.Redirect("M1discounting.aspx?tid=" & DocID)
                'End If
                'Code for Discounting End here

                ' FinalQry = FinalQry.Replace("<br/>", "\n")
                ' con.Close()
                ' ClientScript.RegisterStartupScript(Me.GetType(), "Popup", "alert('" & FinalQry.ToString() & "');", True)
                ' Return
                updMsg.Update()
                btnForm_ModalPopupExtender.Show()
            End If
        Catch ex As Exception
            If Not tran Is Nothing Then
                tran.Rollback()
            End If
            Throw
        Finally
            If Not da Is Nothing Then
                da.Dispose()
            End If
            If Not con Is Nothing Then
                con.Dispose()
                con.Close()
            End If
        End Try
    End Sub

    Protected Sub btnsubmit_click(sender As Object, e As EventArgs)
        Call ValidateData()
    End Sub

    Protected Sub OpenWindow(ByVal sender As Object, ByVal e As EventArgs)
        'Dim fileid As String = ViewState("viewdocid").ToString()
        'Dim sb As StringBuilder = New StringBuilder("")
        'Dim str As String = "window.open('DocDetail.aspx?DOCID=" + fileid + "');"
        'sb.Append(str)
        'ScriptManager.RegisterClientScriptBlock(Me.updMsg, Me.updMsg.GetType(), "NewClientScript", sb.ToString(), True)
    End Sub

    Public Sub Reset(ByVal sender As Object, ByVal e As EventArgs)
        Dim scrname As String = "Ticket"
        Dim str As String = Request.RawUrl
        If str.Contains("&tid=") Then
            str = str.Replace("&tid=" & Request.QueryString("TID").ToString(), "")
        End If
        Response.Redirect(str)
    End Sub

    'Protected Sub btnsubmit_click(sender As Object, e As EventArgs)
    '    Dim dyanamicForm As New DynamicForm()
    '    Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
    '    Dim con As SqlConnection = New SqlConnection(conStr)
    '    Dim tran As SqlTransaction = Nothing
    '    con.Open()
    '    tran = con.BeginTransaction()
    '    'Try

    '    'Dim value As String = hdnAction.Value
    '    'If value = "" Then
    '    '    ClientScript.RegisterStartupScript(Me.GetType(), "Popup", "alert('Please take any  one user action');", True)
    '    '    Return
    '    'End If

    '    Dim oda As SqlDataAdapter = New SqlDataAdapter("SELECT FF.*,F.*  FROM MMM_MST_FIELDS FF With(nolock)  left outer JOIN MMM_MST_FORMS F   With(nolock) on F.FormName = FF.DocumentType and F.EID = FF.EID   where (FF.isactive=1  and  MDfieldName is null) and F.EID=" & Session("EID").ToString() & " and FormName='Ticket'  order by displayOrder", con)
    '    oda.SelectCommand.Transaction = tran
    '    Dim ds As New DataSet()
    '    oda.Fill(ds, "fields")
    '    Dim dv As DataView = ds.Tables("fields").DefaultView
    '    dv.RowFilter = "IsActive=1"
    '    Dim theFields As DataTable = dv.ToTable
    '    Dim lstData As New List(Of UserData)
    '    Dim obj As New DynamicForm()
    '    lstData = obj.CreateCollection(pnlnewfields, theFields)
    '    Dim FinalQry As String
    '    ViewState("tid") = Request.QueryString("DOCID")
    '    oda.SelectCommand.CommandText = "select * from mmm_mst_fields where documenttype='Ticket' and eid=" & Session("EID") & " and mdfieldname is not null union all select * from mmm_mst_fields where documenttype in  (select dropdown from MMM_MST_FIELDS where DocumentType='Ticket' and eid=" & Session("EID") & " and  fieldType='Child Item') and eid=" & Session("EID") & "  and mdfieldname is not null"
    '    Dim dt As New DataTable()
    '    oda.Fill(dt)
    '    Dim arrcolumn As New ArrayList()
    '    Dim ChildItemColumn As String = ""
    '    Dim ChildItemDocumentType As String = ""
    '    For Each dr As DataRow In dt.Rows
    '        Select Case dr("mdfieldname").ToString().ToUpper
    '            Case "EMAILID"
    '                arrcolumn.Add(dr("FieldMapping").ToString() & "='" & [TO].InnerText & "'")
    '            Case "SUBJECT"
    '                arrcolumn.Add(dr("FieldMapping").ToString() & "='" & txtSubject.Text & "'")
    '            Case "REMARKS"
    '                arrcolumn.Add(dr("FieldMapping").ToString() & "='" & txtBody.Text.ToString().Replace("'", "''") & "'")
    '            Case "STATUS"
    '                arrcolumn.Add(dr("FieldMapping").ToString() & "='" & value.ToString.ToUpper & "'")
    '            Case "CC"
    '                arrcolumn.Add(dr("FieldMapping").ToString() & "='" & txtCC.Text.ToString() & "'")
    '            Case "ASSIGNEE"
    '                arrcolumn.Add(dr("FieldMapping").ToString() & "=" & IIf(hdnAssignee.Value = String.Empty, "''", hdnAssignee.Value) & "")
    '            Case "ATTACHMENT"
    '                ChildItemColumn = dr("FieldMapping").ToString()
    '                ChildItemDocumentType = dr("documenttype").ToString()
    '        End Select
    '    Next
    '    FinalQry = dyanamicForm.ValidateAndGenrateQueryForControls("UPDATE", "UPDATE MMM_MST_DOC SET  ticketstatus='" & value.ToString() & "'," & String.Join(",", arrcolumn.ToArray()) & ",", "", ds.Tables("fields"), pnlnewfields, ViewState("tid"))
    '    If Trim(Left(FinalQry, 6)).ToUpper() = "PLEASE" Then
    '        FinalQry = FinalQry.Replace("<br/>", "\n")
    '        con.Close()
    '        ClientScript.RegisterStartupScript(Me.GetType(), "Popup", "alert('" & FinalQry.ToString() & "');", True)
    '        Return
    '    Else
    '        FinalQry = FinalQry & " WHERE tID=" & ViewState("tid")
    '        oda.SelectCommand.CommandText = FinalQry
    '        If con.State <> ConnectionState.Open Then
    '            con.Open()
    '        End If
    '        oda.SelectCommand.ExecuteNonQuery()
    '        If ChildItemColumn <> "" Then
    '            Dim splittedvalues As String() = hdnUploadedFileName.Value.Split(",")
    '            If splittedvalues(0) <> String.Empty Then
    '                For x As Integer = 0 To splittedvalues.Length - 1
    '                    'Try
    '                    Dim tempFileName As String() = splittedvalues(x).ToString().Split("|")
    '                    Dim tempPath As String = Server.MapPath("~/DOCS/temp/" & tempFileName(0).ToString())
    '                    oda.SelectCommand.CommandText = "insert into TicketAccessFile values(" & ViewState("tid") & ",'" & tempPath & "',getdate()),(" & ViewState("tid") & ",'" & Server.MapPath("~/DOCS/") & Session("EID") & "/" & tempFileName(0) & "',getdate())"
    '                    If con.State = ConnectionState.Closed Then
    '                        con.Open()
    '                    End If
    '                    oda.SelectCommand.ExecuteNonQuery()
    '                    If Not Directory.Exists(Server.MapPath("~/DOCS/") & Session("EID")) Then
    '                        Directory.CreateDirectory(Server.MapPath("~/DOCS/") & Session("EID"))
    '                    End If
    '                    File.Copy(tempPath, Server.MapPath("~/DOCS/") & Session("EID") & "/" & tempFileName(0))
    '                    oda.SelectCommand.CommandText = "insert into mmm_mst_doc_item (DOCID,DOCUMENTTYPE,ISAUTH,LASTUPDATE,cmastertid," & ChildItemColumn & ",Attachment) values(" & ViewState("tid") & ",'" & ChildItemDocumentType & "',1,getdate(),0,'" & Session("EID") & "/" & tempFileName(0) & "','" & tempFileName(1).ToString() & "')"
    '                    If con.State = ConnectionState.Closed Then
    '                        con.Open()
    '                    End If
    '                    oda.SelectCommand.ExecuteNonQuery()
    '                    'Catch ex As Exception
    '                    '    Throw
    '                    '    ex.Message.ToString()
    '                    'End Try
    '                Next
    '            End If
    '        End If
    '        Dim res As String
    '        Dim ob1 As New DMSUtil()
    '        If value.ToString().ToUpper = "CLOSED" Then
    '            res = ob1.GetNextUserFromRolematrixT(ViewState("tid"), Val(Session("EID").ToString()), Val(Session("UID").ToString()), "", Val(Session("UID").ToString()), con, tran)
    '            If String.IsNullOrEmpty(res) Then
    '                res = GetNextUserFromOrganizatios(con, tran)
    '            End If
    '            AssignTicketToUserBasedOnCondition(res, ViewState("tid"), "", HttpContext.Current.Session("UID"), con, tran)
    '        End If
    '        dyanamicForm.HistoryT(Session("EID"), ViewState("tid"), HttpContext.Current.Session("UID"), Session("FORMNAME"), "MMM_MST_DOC", "UPDATE", con, tran)
    '        Trigger.ExecuteTriggerT(Session("FORMNAME"), Session("EID"), ViewState("tid"), con, tran)

    '        tran.Commit()

    '        Dim objDC As New DataClass
    '        'Change condition for new assignee if you wish to assign this ticket to someone else
    '        If hdnNewAssignee.Value <> "0" And hdnNewAssignee.Value <> "" Then
    '            objDC.ExecuteQryDT("update mmm_doc_dtl set userid=" & hdnNewAssignee.Value & " where tid in (select tid from mmm_doc_dtl where docid=" & ViewState("tid") & " and aprstatus is null)")
    '        End If

    '        If hdnPreStatus.Value.ToUpper = "SUSPENDED" Then
    '            objDC.ExecuteQryDT("update mmm_doc_dtl set userid=" & hdnAssignee.Value & " where tid in (select tid from mmm_doc_dtl where docid=" & ViewState("tid") & " and aprstatus is null)")
    '            objDC.ExecuteQryDT("update mmm_mst_doc set fld16=" & hdnOrganization.Value & " where tid =" & ViewState("tid") & "")
    '        End If

    '        'ThreadPool.QueueUserWorkItem(AddressOf ThreadProc, ViewState("tid") & "||" & HttpContext.Current.Session("EID") & "||" & HttpContext.Current.Session("FORMNAME") & "||" & "APPROVE" & "||" & "-" & "||" & IIf(txtCC.Text = "", "-", txtCC.Text))
    '        'Comment because user did not want to see self writing code


    '        Dim dttemp As New DataTable
    '        dttemp = objDC.ExecuteQryDT("select rtrim(ltrim(userrole)) from mmm_mst_user where uid=" & Val(Session("UID").ToString()) & " and eid=" & Val(Session("EID").ToString()) & "")
    '        Dim tmpUSER As String = objDC.ExecuteQryScaller("select ','+isnull(USERROLE,'')+',' from  mmm_hdmail_schdule where eid=" & Session("EID"))
    '        If hdnNewAssignee.Value <> "0" And hdnNewAssignee.Value <> "" Then
    '            ThreadPool.QueueUserWorkItem(AddressOf ThreadProc, ViewState("tid") & "||" & HttpContext.Current.Session("EID") & "||" & HttpContext.Current.Session("FORMNAME") & "||" & "ASSIGNEDTOOTHERAGENT" & "||" & "-" & "||" & "-")
    '        ElseIf tmpUSER.ToString().ToUpper.Contains("," & dttemp.Rows(0)(0).ToString().ToUpper & ",") Then
    '            ThreadPool.QueueUserWorkItem(AddressOf ThreadProc, ViewState("tid") & "||" & HttpContext.Current.Session("EID") & "||" & HttpContext.Current.Session("FORMNAME") & "||" & "USERREPLY" & "||" & "-" & "||" & "-")
    '            'ob1.TemplateCalling(ViewState("tid"), HttpContext.Current.Session("EID"), HttpContext.Current.Session("FORMNAME"), "USERREPLY")
    '        Else
    '            ThreadPool.QueueUserWorkItem(AddressOf ThreadProc, ViewState("tid") & "||" & HttpContext.Current.Session("EID") & "||" & HttpContext.Current.Session("FORMNAME") & "||" & "AGENTREPLY" & "||" & "-" & "||" & "-")
    '            'ob1.TemplateCalling(ViewState("tid"), HttpContext.Current.Session("EID"), HttpContext.Current.Session("FORMNAME"), "AGENTREPLY")

    '        End If
    '        'If tmpUSER.ToString().ToUpper.Contains("," & dttemp.Rows(0)(0).ToString().ToUpper & ",") Then

    '        'Else

    '        'End If
    '        Response.Redirect("~/thome.aspx", False)
    '    End If
    '    'Catch ex As Exception
    '    '    tran.Rollback()
    '    '    lblMessage.Text = "BtnUpdate Exeception error " & ex.Message.ToString()
    '    'End Try
    'End Sub

    Protected Sub Page_Init(sender As Object, e As EventArgs) Handles Me.Init
        Try
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As SqlConnection = New SqlConnection(conStr)
            Dim oda As SqlDataAdapter = New SqlDataAdapter("SELECT FF.*,F.*  FROM MMM_MST_FIELDS FF With(nolock)  left outer JOIN MMM_MST_FORMS F   With(nolock) on F.FormName = FF.DocumentType and F.EID = FF.EID   where (FF.isactive=1 ) and F.EID=" & Session("EID").ToString() & " and FormName='ticket' " & " and fieldType<>'Child Item'  order by displayOrder", con)
            Dim ds As New DataSet()
            oda.Fill(ds, "data")
            Dim objDC As New DataClass
            CreateControlsOnPanel(ds.Tables(0), pnlnewfields, Nothing, Nothing, 0, Nothing, Nothing, False, False, "OPEN") 'hdnStatus.Value)
            Dim ROW123() As DataRow = ds.Tables(0).Select("fieldtype='DROP DOWN'  and (lookupvalue is not null or ddllookupvalue is not null or multilookUpVal is not null or ltlookupval is not null or HasRule='1')")
            If ROW123.Length > 0 Then
                For i As Integer = 0 To ROW123.Length - 1
                    Dim check As Integer = 0
                    Dim DDL As DropDownList = TryCast(pnlnewfields.FindControl("fld" & ROW123(i).Item("FieldID").ToString()), DropDownList)
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
            '  Dim dyanamicForm As New DynamicForm()
            '  dyanamicForm.FillControlsOnPanel(ds.Tables(0), pnlnewfields, "Document", Convert.ToInt32(Request.QueryString("DOCID").ToString()))
            ' oda.SelectCommand.CommandText = "select documenttype from mmm_mst_doc with(nolock) where tid=" & Request.QueryString("DOCID")
            ' If con.State = ConnectionState.Closed Then
            ' con.Open()
            ' End If
            ' Session("FORMNAME") = Convert.ToString(oda.SelectCommand.ExecuteScalar())
            con.Close()
            ' imageSource.Src = "~/logo/" & objDC.ExecuteQryScaller("select logo_Text  from mmm_mst_entity where eid=" & Session("EID"))

            '' mew by sp may-18
            Dim ROW1() As DataRow = ds.Tables("data").Select("fieldtype='DROP DOWN'   and (lookupvalue is not null or ddllookupvalue is not null or multilookUpVal is not null or ltlookupval is not null or HasRule='1')")
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
                        Dim screenname = Request.QueryString("SC").ToString
                        'ExecuteControllevelRule(CInt(id), pnlFields, Nothing, screenname, ds.Tables("data"), Nothing, lblTab, False)
                    End If
                Next
            End If
            'Code For Rule Post back handler for text box controls
            'Added By Himank :11th-september-2015
            'Dim RuleRow() As DataRow = ds.Tables("data").Select("fieldtype='Text Box' and  HasRule='1'")
            Dim RuleRow() As DataRow = ds.Tables("data").Select("HasRule='1'")
            If RuleRow.Length > 0 Then
                Session("ALLRULE") = RuleRow
                For r As Integer = 0 To RuleRow.Length - 1
                    Dim FieldType = RuleRow(r).Item("FieldType").ToString()
                    Select Case FieldType.ToUpper
                        Case "TEXT BOX"
                            Dim TextBox As TextBox = TryCast(pnlnewfields.FindControl("fld" & RuleRow(r).Item("FieldID").ToString()), TextBox)
                            Dim id As String = Right(TextBox.ID, TextBox.ID.Length - 3)
                            TextBox.AutoPostBack = True
                            AddHandler TextBox.TextChanged, AddressOf TextBoxRule_TextChanged
                        Case "CHECK BOX"
                            Dim CheckBox As CheckBox = TryCast(pnlnewfields.FindControl("fld" & RuleRow(r).Item("FieldID").ToString()), CheckBox)
                            Dim id As String = Right(CheckBox.ID, CheckBox.ID.Length - 3)
                            CheckBox.AutoPostBack = True
                            AddHandler CheckBox.CheckedChanged, AddressOf TextBoxRule_TextChanged
                    End Select
                Next
            End If
            '' for inline grid ddl filter from main ddl by sunil 31_july_14
            Dim ROWC() As DataRow = ds.Tables("data").Select("fieldtype='Child Item' and len(isnull(KC_LOGIC,''))>2")
            If ROWC.Length > 0 Then
                For i As Integer = 0 To ROWC.Length - 1
                    Dim st() As String = ROWC(i).Item("kc_logic").ToString().Split("-")

                    Dim DDL As DropDownList = TryCast(pnlnewfields.FindControl("fld" & st(0).ToString), DropDownList)
                    Dim id As String = Right(DDL.ID, DDL.ID.Length - 3)
                    DDL.AutoPostBack = True
                    AddHandler DDL.TextChanged, AddressOf bindvalue2
                Next
            End If
            '' for inline grid ddl filter from main ddl by sunil 31_july_14

            Dim dtchild As DataTable = ds.Tables("data")
            If ds.Tables("data").Rows(0).Item("Iscalendar").ToString() = "1" Then
                'Dim btncldr As Button = TryCast(pnlFields.FindControl("BTNCLNDR"), Button)
                'AddHandler btncldr.Click, AddressOf ADDTASK
                'Dim Grd1 As GridView = TryCast(pnlFields.FindControl("GRDCLNDR"), GridView)
                'AddHandler Grd1.RowDataBound, AddressOf addTemplateField
                'AddHandler Grd1.RowCommand, AddressOf DeleteTask
                'AddHandler Grd1.RowDeleting, AddressOf DeletedTask
                'Grd1.DataSource = Session("dtNew")
                'Grd1.DataBind()
            End If
            Dim row() As DataRow = dtchild.Select("FieldType='CHILD ITEM'")
            If Session("ALLCHILD") Is Nothing Then
                Session("ALLCHILD") = row
            End If

            If row.Length > 0 Then
                Dim btn1 As New Button
                For i As Integer = 0 To row.Length - 1
                    '' removed frm here by sp 13-jan-14
                    ''btn1 = pnlFields.FindControl("BTN" & row(i).Item("FieldID").ToString())
                    ''AddHandler btn1.Click, AddressOf ShowChildForm
                    Dim PRitem() As String = row(i).Item("dropdown").ToString().Split("-")
                    'If PRitem.Length > 1 Then
                    '    Dim BTN2 As Button = pnlnewfields.FindControl("BTN" & PRitem(1).ToString & "-" & row(i).Item("FIELDID").ToString())
                    '    AddHandler BTN2.Click, AddressOf INSERTCHILDITEM
                    'End If

                    'Session("FNS") = Session("FNS") & PRitem(0).ToString() & ":" & row(i).Item("Fieldid").ToString() & ":"

                    '' Dim array3Ds(,,) As String = New String(,,) {}

                    'Dim ColHEAD() As String = {}
                    'Dim DDLTXT() As String = {}
                    'Dim DDLVAL() As String = {}

                    'Session("COLHEAD") = ColHEAD
                    'Session("DDLTXT") = DDLTXT
                    'Session("DDLVAL") = DDLVAL
                    ' '' by sunil for def value 16-dec-13 - ends

                    Dim GRD As GridView = pnlnewfields.FindControl("GRD" & row(i).Item("Fieldid").ToString())
                    ''AddHandler GRD.RowDataBound, AddressOf totalrow  '' moved frm here to below


                    'AddHandler GRD.RowCommand, AddressOf Delete
                    'AddHandler GRD.RowDeleting, AddressOf Deleting
                    '' AddHandler GRD.RowDataBound, AddressOf addTemplateField


                    '' by sunil for def value insert for Aggrawal  16-dec-13 - starts
                    Dim strDF As String = "select * from mmm_mst_forms  With(nolock)  where formname='" & PRitem(0).ToString() & "' and formsource='DETAIL FORM' and EID=" & Session("EID").ToString() & " and (isnull(HasDefaultValue,0)=1 or isnull(isinlineediting,0)=1 or  isnull(NullIf(IsDefaultRows,''),0)<>0) "
                    Dim oda1 As SqlDataAdapter = New SqlDataAdapter("", con)
                    Dim dtIsdv As New DataTable
                    oda.SelectCommand.CommandText = strDF
                    oda.Fill(dtIsdv)

                    If dtIsdv.Rows.Count = 1 Then '' found hasdefvalue prop. true proceed to display button 
                        ' Dim btnIDV As New Button
                        ' btnIDV = pnlFields.FindControl("BTN_" & row(i).Item("FieldID").ToString())  '  "BTN" & "_" & ROWCHILD(i).Item("FIELDID").ToString()
                        'tbenabled                     AddHandler GRD.RowDataBound, AddressOf gvData_InlineEdit
                        'BTNFLTER
                        'By balli 
                        'If Not IsNothing(Session("BindGrdOnDdl")) = True Then
                        '    Dim ddlGrd As String() = Split(Session("BindGrdOnDdl"), "-")
                        '    If DynamicForm.GetControl(pnlFields, "fld" & ddlGrd(0)) Then
                        '        Dim ddl As DropDownList = CType(pnlFields.FindControl("fld" & ddlGrd(0)), DropDownList)
                        '        AddHandler ddl.SelectedIndexChanged, AddressOf DynamicGrdFilter
                        '    End If
                        'End If
                        'Dim filterROW1() As DataRow = dtIsdv.Select("inlinefilter is not null")
                        'If filterROW1.Length > 0 Then
                        '    Dim filter = filterROW1(0).Item("inlinefilter").ToString().Split("~")
                        '    For f As Integer = 0 To filter.Length - 1
                        '        Dim SD As String() = Split(filter(f), "-")
                        '        Dim Opr As String() = Split(SD(1), "|")
                        '        If SD(0).ToUpper = "D" Then
                        '            'Session("BindGrdOnDdl") = Opr(2) & "-" & filterROW1(0).Item("FormName")
                        '            Dim ddlfilter As DropDownList = CType(pnlFields.FindControl("fld" & Opr(2)), DropDownList)
                        '            If Not IsNothing(ddlfilter) Then
                        '                ddlfilter.AutoPostBack = True
                        '                AddHandler ddlfilter.SelectedIndexChanged, AddressOf DynamicGrdFilter
                        '            End If
                        '        End If
                        '    Next
                        'End If
                        'end here
                        'By ajeet 
                        'Dim str = "BTNFLTER" & row(i).Item("FIELDID").ToString() & "_" & row(i).Item("DROPDOWN").ToString().Replace(" ", "_")
                        'btn1 = pnlnewfields.FindControl(str)
                        'If Not btn1 Is Nothing Then
                        '    AddHandler btn1.Click, AddressOf Filter ' use for dynamic filter  
                        'End If
                        ''prashant_10_7

                        '  If Not String.IsNullOrEmpty(dtIsdv.Rows(0).Item("IsDefaultRows").ToString) Then
                        'Dim BTNUpload As Button = pnlFields.FindControl("BTNUpload" & row(i).Item("FIELDID").ToString())
                        ''ShowChildItemUploadForm
                        'If Not BTNUpload Is Nothing Then
                        '    AddHandler BTNUpload.Click, AddressOf ShowChildItemUploadForm
                        'End If

                        'Dim imgExport As ImageButton = pnlFields.FindControl("btnExportError" & row(i).Item("FIELDID").ToString())
                        ''ShowChildItemUploadForm
                        'If Not imgExport Is Nothing Then
                        '    AddHandler imgExport.Click, AddressOf Me.imgExport_Click
                        'End If

                        ' End If

                        'prashant_10_7


                        'prashant_11_7
                        'If Not String.IsNullOrEmpty(row(i).Item("isinlineediting").ToString.Trim()) Then
                        '    If Convert.ToInt32(row(i).Item("isinlineediting").ToString.Trim()) > 0 Then
                        '        Dim grid As GridView
                        '        grid = pnlFields.FindControl("GRD" & row(i).Item("FIELDID").ToString())
                        '        If Not grid Is Nothing Then
                        'Dim btn As Button
                        'btn = pnlnewfields.FindControl("bntCalFromGrid_" & row(i).Item("FIELDID").ToString())
                        'If Not btn Is Nothing Then
                        '    AddHandler btn.Click, AddressOf CalculateFromGrid
                        'End If

                        'Dim btn2 As Button
                        'btn2 = pnlnewfields.FindControl("BtnAddRow_" & row(i).Item("FIELDID").ToString())
                        'If Not btn2 Is Nothing Then
                        '    AddHandler btn2.Click, AddressOf AddRow
                        '    Dim doc = row(i).Item("dropdown").ToString()
                        '    Dim qry = "Select EnableAddRow from mmm_mst_Forms where formName='" & doc & "' and Eid=" & Session("Eid")
                        '    oda.SelectCommand.CommandText = qry
                        '    con.Open()
                        '    Dim enable As Integer = Convert.ToInt32(oda.SelectCommand.ExecuteScalar)
                        '    con.Close()
                        '    If enable = 0 Then
                        '        btn2.Visible = False
                        '    End If
                        'End If


                    Else


                        'AddHandler GRD.RowDataBound, AddressOf totalrow
                        'btn1 = pnlFields.FindControl("BTN" & row(i).Item("FieldID").ToString())
                        'AddHandler btn1.Click, AddressOf ShowChildForm
                        ''For Child Item uploder button
                        'Dim BTNUpload As Button = pnlFields.FindControl("BTNUpload" & row(i).Item("FIELDID").ToString())
                        ''ShowChildItemUploadForm
                        'If Not BTNUpload Is Nothing Then
                        '    AddHandler BTNUpload.Click, AddressOf ShowChildItemUploadForm
                        'End If
                        'Dim imgExport As ImageButton = pnlFields.FindControl("btnExportError" & row(i).Item("FIELDID").ToString())
                        'If Not imgExport Is Nothing Then
                        '    AddHandler imgExport.Click, AddressOf Me.imgExport_Click
                        'End If

                    End If

                    ' oda.Dispose()
                    dtIsdv.Dispose()
                    ' filter for grid bind by balli
                    Dim scrname As String = "Ticket"

                    oda.SelectCommand.CommandType = CommandType.StoredProcedure
                    oda.SelectCommand.CommandText = "uspGetDetailITEM"
                    oda.SelectCommand.Parameters.Clear()
                    oda.SelectCommand.CommandType = CommandType.StoredProcedure
                    oda.SelectCommand.Parameters.AddWithValue("SID", Session.SessionID)
                    oda.SelectCommand.Parameters.AddWithValue("FN", PRitem(0).ToString())
                    oda.SelectCommand.Parameters.AddWithValue("EID", Session("EID"))
                    ds.Tables.Clear()
                    oda.Fill(ds, "ITEM")
                    oda.SelectCommand.CommandType = CommandType.Text
                    'oda.SelectCommand.CommandText = "SELECT F1.FieldID,F2.displayName,F1.FieldType FROM MMM_MST_FIELDS F1 INNER JOIN MMM_MST_FIELDS F2 ON F1.dropdown =CONVERT(NVARCHAR(20),F2.Fieldid)  WHERE F1.EID=" & Session("EID") & " AND F1.FieldType  in ('CHILD ITEM MAX','CHILD ITEM TOTAL')  AND F2.DocumentType ='" & row(i).Item("dropdown").ToString() & "' AND F1.DOCUMENTTYPE='" & scrname & "'"
                    'With(nolock) added by Himank on 29th sep 2015
                    oda.SelectCommand.CommandText = "SELECT F1.FieldID,F2.displayName,F1.FieldType,F1.child_specific_text FROM MMM_MST_FIELDS F1   WITH(NOLOCK)  INNER JOIN MMM_MST_FIELDS F2  WITH(NOLOCK)  ON F1.dropdown =CONVERT(NVARCHAR(20),F2.Fieldid)  WHERE F1.EID=" & Session("EID") & " AND F1.FieldType  in ('CHILD ITEM MAX','CHILD ITEM TOTAL')  AND F2.DocumentType ='" & row(i).Item("dropdown").ToString() & "' AND F1.DOCUMENTTYPE='" & scrname & "' union all Select FieldID,displayName,fieldtype,child_specific_text  from mmm_mst_fields  WITH(NOLOCK)  where eid=" & Session("EID") & " and DOCUMENTTYPE='" & scrname & "' and Child_Specific_text is not null "
                    oda.Fill(ds, "TOTAL")

                    If Not IsPostBack Then
                        Session(PRitem(0)) = Nothing
                    End If

                    'If Not Session(PRitem(0)) Is Nothing Then
                    '    ob.BINDITEMGRID1(Session(PRitem(0)), pnlFields, row(i).Item("fieldid"), UpdatePanel1, ds.Tables("TOTAL"), -1, -1, scrname, row(i).Item("dropdown").ToString(), Convert.ToInt32(Session("EID")))
                    'Else

                    '      End If
                Next

                'If Not Session("CHILD") Is Nothing Then
                '    ob.CreateControlsOnPanel(Session("CHILD"), pnlFields1, updpnlchild, Button2, 0, Session("DDL"))
                '    'Code Block is Modified by Ajeet For control level Rule Engine Dated:21-Jan-2014
                '    Dim ROW2() As DataRow = Session("CHILD").Select("fieldtype='DROP DOWN'   and (lookupvalue is not null or ddllookupvalue is not null or multilookUpVal is not null or HasRule='1') ")
                '    If ROW2.Length > 0 Then
                '        For i As Integer = 0 To ROW2.Length - 1
                '            Dim DDL As DropDownList = TryCast(pnlFields1.FindControl("fld" & ROW2(i).Item("FieldID").ToString()), DropDownList)
                '            DDL.AutoPostBack = True
                '            AddHandler DDL.TextChanged, AddressOf bindvalue1
                '        Next
                '    End If
                '    ChildFormddlRendering(row, 2)
                'End If
            End If




        Catch ex As Exception
        End Try
    End Sub

    Public Sub CreateControlsOnPanel(ByVal dsFields As DataTable, ByRef pnlFields As Panel, ByRef UpdatePanel1 As UpdatePanel, ByRef btnActEdit As Button, ByVal autolayout As Integer, Optional ByRef ddown As DropDownList = Nothing, Optional ByVal amendment As String = Nothing, Optional ByVal IsDocDetail As Boolean = False, Optional ByVal IsCallingFromMainHome As Boolean = False, Optional ByVal TicketStatus As String = "OPEN")  ' Optional ByRef ddown As DropDownList = Nothing
        pnlFields.Controls.Clear()
        Dim objDC As New DataClass()
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
                '  pnlFields.Controls.Add(New LiteralControl("<div class=""container"" style=""text-align:center"" > <div class=""row""><div class=""col-md-6 col-md-offset-3""><div class=""login-panel panel panel-primary""><div class=""panel-heading""><h3 class=""panel-title"">Submit Request</h3></div><div class=""panel-body""><fieldset><div id=""message"" class=""form-group col-xs-12""></div>")))
                '<label style="display:block;text-align:left;"  class="control-label"> Attachment *</label>
                For i As Integer = 0 To ds.Rows.Count - 1
                    'pnlFields.Controls.Add(New LiteralControl("<div class=""form-group col-xs-6""><label class=""control-label"">" & ds.Rows(i).Item("displayname").ToString() & IIf(ds.Rows(i).Item("isRequired").ToString() = "1", "*", "") & "</label></div></div>"))
                    '  pnlFields.Controls.Add(New LiteralControl("<div class=""control-group""><label class=""control-label"">" & ds.Rows(i).Item("displayname").ToString() & IIf(ds.Rows(i).Item("isRequired").ToString() = "1", "*", "") & "</label> <div class=""controls"">"))
                    If ds.Rows(i).Item("FieldType").ToString().ToUpper() <> "TEXT AREA" Then
                        pnlFields.Controls.Add(New LiteralControl("<div class=""form-group col-xs-6""> <label style=""display:block;text-align:left;""  class=""control-label"">" & ds.Rows(i).Item("displayname").ToString() & IIf(ds.Rows(i).Item("isRequired").ToString() = "1", "*", "") & "</label> <div class=""controls"">"))
                    Else
                        pnlFields.Controls.Add(New LiteralControl("<div class=""form-group col-xs-12""><label style=""display:block;text-align:left;""  class=""control-label"">" & ds.Rows(i).Item("displayname").ToString() & IIf(ds.Rows(i).Item("isRequired").ToString() = "1", "*", "") & "</label> <div class=""controls"">"))
                    End If

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
                            ddl.CssClass = "form-control"
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
                                ddl.CssClass = "form-control"
                            End If

                            If Convert.ToString(ds.Rows(i).Item("displayname")) <> "" Then
                                ddl.Attributes.Add("placeholder", Convert.ToString(ds.Rows(i).Item("displayname")))
                            End If

                            pnlFields.Controls.Add(New LiteralControl("</div></div>"))
                        Case "TEXT AREA"
                            DataType = ds.Rows(i).Item("datatype").ToString().ToUpper()
                            Dim txtBox As New TextBox
                            txtBox.ID = "fld" & ds.Rows(i).Item("FieldID").ToString()
                            '  txtBox.Width = controlWdth - 10
                            txtBox.Rows = 4
                            txtBox.TextMode = TextBoxMode.MultiLine
                            If DataType.ToUpper = "MSGBODY" Then
                                ' newScreenHeight = newScreenHeight + 100
                                ' txtBox.Height = 180 'newScreenHeight
                                txtBox.CssClass = "msgbody"
                            Else
                                txtBox.CssClass = "form-control"
                                ' newScreenHeight = newScreenHeight + 60
                                ' txtBox.Height = "80"
                            End If


                            If ds.Rows(i).Item("defaultfieldval").ToString.Length > 0 Then
                                txtBox.Text = ds.Rows(i).Item("defaultfieldval")
                            Else
                                If DataType = "NUMERIC" Then
                                    txtBox.Text = "0"
                                End If
                            End If
                            pnlFields.Controls.Add(txtBox)
                            If Not DataType.ToUpper = "MSGBODY" Then
                                'For Grofers
                                If Convert.ToString(ds.Rows(i).Item("Style")) <> "" Then
                                    Dim txt As String() = Convert.ToString(ds.Rows(i).Item("Style")).Split(",")
                                    txtBox.Attributes.Add("style", txt(1))
                                Else
                                    txtBox.CssClass = "form-control"
                                End If
                            End If

                            If DataType.ToUpper = "MSGBODY" Then
                                'Dim htmlExt As New AjaxControlToolkit.HtmlEditorExtender
                                'htmlExt.ID = "msgbody" & ds.Rows(i).Item("FieldID").ToString()
                                'htmlExt.DisplaySourceTab = True
                                'htmlExt.TargetControlID = txtBox.ID
                                'htmlExt.EnableSanitization = False
                                'pnlFields.Controls.Add(htmlExt)
                                If Convert.ToString(ds.Rows(i).Item("description")) <> "" Then
                                    txtBox.Attributes.Add("placeholder", Convert.ToString(ds.Rows(i).Item("description")))
                                End If
                            End If
                            ' For tooltip
                            'If Convert.ToString(ds.Rows(i).Item("description")) <> "" Then
                            '    ' txtBox.Width = controlWdth - 30
                            '    Dim dynSpan As System.Web.UI.HtmlControls.HtmlGenericControl = New System.Web.UI.HtmlControls.HtmlGenericControl("span")
                            '    dynSpan.ID = "span" & ds.Rows(i).Item("FieldID").ToString()
                            '    dynSpan.Attributes.Add("class", "help-tip notification")
                            '    dynSpan.InnerText = "?"
                            '    dynSpan.InnerHtml = "? <div class="" tip"">" & Convert.ToString(ds.Rows(i).Item("description")) & "</div>"
                            '    pnlFields.Controls.Add(dynSpan)
                            '    'pnlFields.Controls.Add(New LiteralControl("<span id="" span" & ds.Rows(i).Item(" FieldID").ToString() & """ class="" help-tip notification"">? <div class="" tip"">" & Convert.ToString(ds.Rows(i).Item("description")) & "</div></span>"))
                            'End If
                            If Convert.ToString(ds.Rows(i).Item("description")) <> "" Then
                                txtBox.Attributes.Add("placeholder", Convert.ToString(ds.Rows(i).Item("description")))
                            End If
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
                                    txtBox.CssClass = "form-control"
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
                                        '  Dim img As New Image
                                        '  img.ID = "img" & ds.Rows(i).Item("FieldID").ToString()
                                        '  img.ImageUrl = "~\images\Cal.png"
                                        '  pnlFields.Controls.Add(img)
                                        CLNDR.PopupButtonID = "img" & ds.Rows(i).Item("FieldID").ToString()
                                        pnlFields.Controls.Add(CLNDR)
                                    Else
                                        txtBox.Enabled = False
                                    End If
                                Else
                                    If HttpContext.Current.Session("EDITonEDIT") = "EDITonEDIT" Then  ' this session is inittialized on doc detail page by balli 
                                        If ds.Rows(i).Item("alloweditonedit") = 1 And ds.Rows(i).Item("iseditable") = 1 Then
                                            'Dim img As New Image
                                            ' img.ID = "img" & ds.Rows(i).Item("FieldID").ToString()
                                            ' img.ImageUrl = "~\images\Cal.png"
                                            ' pnlFields.Controls.Add(img)
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
                                txtBox.CssClass = "form-control"
                            End If
                            If Convert.ToString(ds.Rows(i).Item("displayname")) <> "" Then
                                txtBox.Attributes.Add("placeholder", Convert.ToString(ds.Rows(i).Item("displayname")))
                            End If
                            pnlFields.Controls.Add(New LiteralControl("</div></div>"))
                    End Select
                Next
            End If
            Dim objDF As New DynamicForm
            objDF.RenderInvisibleField(dsFields, pnlFields)

            '' new added by sp -  may
            For i As Integer = 0 To ds.Rows.Count - 1
                Select Case ds.Rows(i).Item("FieldType").ToString().ToUpper()
                    Case "DROP DOWN"
                        Dim dropdowntype As String = ds.Rows(i).Item("dropdowntype").ToString()
                        Dim ddlText As String = ds.Rows(i).Item("dropdown").ToString()
                        If UCase(dropdowntype) = "FIX VALUED" Then
                            Dim ddl As New DropDownList
                            If ds.Rows(i).Item("ddlval").ToString().Length > 1 Then
                                ddl = CType(pnlFields.FindControl("fld" & ds.Rows(i).Item("FieldID").ToString()), DropDownList)
                                Dim dropdowntypeFill As String = ds.Rows(i).Item("ddlval").ToString().ToUpper()
                                ddl.SelectedIndex = ddl.Items.IndexOf(ddl.Items.FindByText(dropdowntypeFill))
                            End If

                            If ddl.Items.Count = 2 Then  '' perform here action of selecting only one item and firing filters
                                ddl.SelectedIndex = ddl.Items.IndexOf(ddl.Items.Item(1))
                                ddl.BackColor = Drawing.Color.LightGray
                                ddl.ForeColor = Drawing.Color.White
                                ddl.Font.Bold = False
                            End If

                        ElseIf UCase(dropdowntype) = "MASTER VALUED" Then
                            If ds.Rows(i).Item("ddlval").ToString().Length > 1 Then
                                Dim ddl As New DropDownList
                                ddl = CType(pnlFields.FindControl("fld" & ds.Rows(i).Item("FieldID").ToString()), DropDownList)
                                Dim dropdowntypeFill As String = ds.Rows(i).Item("ddlval").ToString().ToUpper().Trim()
                                ddl.SelectedIndex = ddl.Items.IndexOf(ddl.Items.FindByValue(dropdowntypeFill))
                                '''''For filling lookup and lookupddl according drop down'''''''
                                Dim id As String = Right(ddl.ID, ddl.ID.Length - 3)
                                Dim id1 As Integer = CInt(id)
                                bind(id, pnlFields, ddl)
                                bindlookupddl(id, pnlFields, ddl)
                            Else   '' new by sunil on 21_Oct_14
                                Dim ddl As New DropDownList
                                ddl = CType(pnlFields.FindControl("fld" & ds.Rows(i).Item("FieldID").ToString()), DropDownList)
                                If ddl.Items.Count = 2 Then  '' perform here action of selecting only one item and firing filters
                                    ddl.SelectedIndex = ddl.Items.IndexOf(ddl.Items.Item(1))
                                    Dim id As String = Right(ddl.ID, ddl.ID.Length - 3)
                                    Dim id1 As Integer = CInt(id)
                                    bind(id, pnlFields, ddl)
                                    bindlookupddl(id, pnlFields, ddl)
                                    ' bindMultiLookUP(id, pnlFields, ddl)
                                    ' bindddlMultiLookUP(id, pnlFields, ddl)
                                    ddl.BackColor = Drawing.Color.LightGray
                                    ddl.ForeColor = Drawing.Color.Black
                                    ddl.Font.Bold = False
                                End If
                            End If
                        ElseIf UCase(dropdowntype) = "SESSION VALUED" Then
                            ''code for filling lookup/filter etc. if session valued..  by sunil
                            Dim ddl As New DropDownList
                            ddl = CType(pnlFields.FindControl("fld" & ds.Rows(i).Item("FieldID").ToString()), DropDownList)
                            Dim dropdowntypeFill As String = ds.Rows(i).Item("dropdowntype").ToString()
                            ddl.SelectedIndex = ddl.Items.IndexOf(ddl.Items.Item(1))

                            Dim id As String = Right(ddl.ID, ddl.ID.Length - 3)
                            Dim id1 As Integer = CInt(id)
                            bind(id, pnlFields, ddl)
                            bindlookupddl(id, pnlFields, ddl)
                            ' bindMultiLookUP(id, pnlFields, ddl)
                            ' bindddlMultiLookUP(id, pnlFields, ddl)
                            ddl.BackColor = Drawing.Color.LightGray
                            ddl.ForeColor = Drawing.Color.Black
                            ddl.Font.Bold = False
                        End If

                        'code for autocomplete default selected before loaded the screen by Mayank 12-01-2017
                    Case "AUTOCOMPLETE"
                        Dim contextKey As String = ds.Rows(i).Item("dropdown").ToString() & "-" & ds.Rows(i).Item("FieldID").ToString() & "-" & ds.Rows(i).Item("dropdowntype").ToString() & "-" & ds.Rows(i).Item("autofilter").ToString() & "-" & ds.Rows(i).Item("InitialFilter").ToString() & "-" & Convert.ToString(ds.Rows(i).Item("conditionalFilter"))
                        Dim arr() As String
                        arr = contextKey.Split("-")
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
                        HttpContext.Current.Session("tableTID") = TABLENAME & "||" & TID  ' this is initializing because we need this at textbox changed event  in dynamic class
                        Dim panelfields As Panel = HttpContext.Current.Session("pnlFields")
                        Dim updpanel As UpdatePanel = HttpContext.Current.Session("updatepanel1")
                        Dim lookUpqry As String = ""
                        Dim str As String = ""
                        If arr(0).ToUpper() = "CHILD" Then
                            str = "select top 50  " & arr(2).ToString() & " As [name]," & TID & "[tid]  from " & TABLENAME & " M WHERE   DOCUMENTTYPE='" & arr(1).ToString() & "'"
                        ElseIf arr(0).ToUpper() <> "STATIC" Then
                            str = "select top 50  " & arr(2).ToString() & " As [name]," & TID & "[tid]" & lookUpqry & " from " & TABLENAME & " M WHERE EID=" & HttpContext.Current.Session("EID") & " AND  DOCUMENTTYPE='" & arr(1).ToString() & "'"
                        Else
                            If arr(2).ToString.ToUpper = "LOCATIONSTATE" Then
                                str = "select DISTINCT " & arr(2).ToString() & " As [name],SID [tid]" & lookUpqry & " from " & TABLENAME & " M "
                            Else
                                str = "select " & arr(2).ToString() & " As [name]," & TID & "[tid]" & lookUpqry & " from " & TABLENAME & " M WHERE EID=" & HttpContext.Current.Session("EID") & " "
                            End If
                        End If

                        Dim xwhr As String = ""
                        Dim tids As String = ""
                        Dim Rtids As String = ""   ' for prerole data filter
                        'Dim tidarr() As String

                        ''FILTER THE DATA ACCORDING TO USER 
                        Dim ob As New DynamicForm
                        tids = ob.UserDataFilter(contextKey.Split("-").ElementAt(1).ToString(), arr(1).ToString())
                        Rtids = ob.UserDataFilter_PreRole(arr(1).ToString(), TABLENAME)  '' new by sunil for pre role data filter 22-feb

                        If tids.Length >= 2 Then
                            xwhr = " AND CONVERT(NVARCHAR(10),TID) IN (" & tids & ")"
                        End If

                        If Rtids.Length >= 2 Then
                            If xwhr.ToString = "" Then
                                xwhr = " AND CONVERT(NVARCHAR(10),TID) IN (" & Rtids & ")"
                            Else
                                xwhr = " AND CONVERT(NVARCHAR(10),TID) IN (" & tids & "," & Rtids & ")"
                            End If
                        End If

                        str = str & "  AND M.isauth=1 " & xwhr
                        Dim AutoFilter As String = arr(5) 'ds.Rows(i).Item("AutoFilter").ToString()
                        Dim InitFilterArr As String() = arr(6).Split(":") 'ds.Rows(i).Item("InitialFilter").ToString().Split(":")
                        'Dim da1 As New SqlDataAdapter(, con)
                        'If con.State = ConnectionState.Closed Then
                        '    con.Open()
                        'End If
                        'Dim SessionFieldvalue As String = Convert.ToString(da1.SelectCommand.ExecuteScalar())
                        Dim SessionFieldvalue As String = Convert.ToString(objDC.ExecuteQryScaller("select isnull(sessionfieldVal,0) as sessionfieldVal from mmm_mst_fields where fieldid=" & arr(3) & " and eid=" & HttpContext.Current.Session("EID")))
                        If AutoFilter.Length > 0 Then
                            Dim filteriD As String = arr(3)
                            Dim mval As String = ""
                            Dim filterMasVal As String = ""
                            'Dim ODA As New SqlDataAdapter("", con)
                            ' ODA.SelectCommand.CommandText = "select top 1 * from MMM_MSt_Fields where eid='" & HttpContext.Current.Session("EID") & "' and fieldtype in ('Drop Down','AutoComplete') and lookupvalue like '%" & filteriD & "-S%'"
                            'With(nolock) added by Himank on 29th sep 2015
                            'oda.SelectCommand.CommandText =
                            Dim dt As New DataTable()
                            dt = objDC.ExecuteQryDT("select top 1 * from MMM_MSt_Fields  WITH(NOLOCK)  where eid='" & HttpContext.Current.Session("EID") & "' and fieldtype in ('Drop Down','AutoComplete') and lookupvalue like '%" & filteriD & "-S%'")
                            'oda.Fill(dt, "filtrId")
                            If dt.Rows.Count > 0 Then
                                Dim fieldtype As String = dt.Rows(0).Item("fieldtype").ToString
                                If fieldtype.ToUpper = "DROP DOWN" Then
                                    If dt.Rows(0).Item("DropDownType").ToString = "SESSION VALUED" Then
                                        ' 16 march balli  
                                        'str = "select " & arr(2).ToString() & " As [name]," & TID & "[tid]" & lookUpqry & " from MMM_MST_USER M WHERE EID=" & HttpContext.Current.Session("EID") & " AND  DOCUMENTTYPE='" & arr(1).ToString() & "'"
                                        'Dim ddl As DropDownList = CType(panelfields.FindControl("fld" & dt.Tables("filtrId").Rows(0).Item("fieldid")), DropDownList)
                                        'mval = ddl.SelectedItem.Value
                                        'filterMasVal = " AND CONVERT(NVARCHAR(10)," & AutoFilter & ") = '" & mval & "' "
                                    Else
                                        Dim ddl As DropDownList = CType(panelfields.FindControl("fld" & dt.Rows(0).Item("fieldid")), DropDownList)
                                        mval = ddl.SelectedItem.Value
                                        filterMasVal = " AND CONVERT(NVARCHAR(10)," & AutoFilter & ") = '" & mval & "' "
                                    End If
                                Else
                                    Dim ddl As HiddenField = CType(panelfields.FindControl("HDN" & dt.Rows(0).Item("fieldid")), HiddenField)  ' for hidden field
                                    'Dim ddl As TextBox = CType(panelfields.FindControl("fld" & dt.Tables("filtrId").Rows(0).Item("fieldid")), TextBox)
                                    'Dim MId As String() = Split(ddl.Text, "-")
                                    'mval = Replace(MId(1), "[", "")
                                    'mval = mval.Replace("]", "")
                                    ' filterMasVal = " AND CONVERT(NVARCHAR(10)," & AutoFilter & ") = '" & mval & "' "
                                    filterMasVal = " AND CONVERT(NVARCHAR(10)," & AutoFilter & ") = '" & Val(ddl.Value) & "' "
                                End If
                            End If
                            ' for binding autocomplete with the selection of dropdo
                            If arr(0).ToUpper() = "CHILD" Then
                                If AutoFilter.ToUpper = "DOCID" Then
                                    str = ob.GetQuery1(arr(1).ToString, arr(2).ToString())
                                Else
                                    str = ob.GetQuery(arr(1).ToString, arr(2).ToString)
                                End If
                            ElseIf arr(0).ToUpper() <> "STATIC" Then
                                'With(nolock) added by Himank on 29th sep 2015
                                str = "select top 50 " & arr(2).ToString() & " As [name],convert(nvarchar(10),tid)  [tid]" & lookUpqry & " from " & TABLENAME & " M   WITH(NOLOCK) WHERE EID=" & HttpContext.Current.Session("EID") & " AND  DOCUMENTTYPE='" & arr(1).ToString() & "'"
                                str = str & "  AND M.isauth=1 " & xwhr
                                If filterMasVal.Length > 1 Then
                                    str = str & filterMasVal
                                End If
                            Else
                                'With(nolock) added by Himank on 29th sep 2015
                                str = "select top 50 " & arr(2).ToString() & " As [name],convert(nvarchar(10)," & TID & ")  [tid]" & lookUpqry & " from " & TABLENAME & " M  WITH(NOLOCK)  WHERE EID=" & HttpContext.Current.Session("EID") & " "
                                str = str & "  AND M.isauth=1 " & xwhr
                                If filterMasVal.Length > 1 Then
                                    str = str & filterMasVal
                                End If
                            End If
                        ElseIf SessionFieldvalue.Length > 0 Then
                            Dim val As String() = SessionFieldvalue.ToString().Split("-")
                            If arr(0).ToUpper() = "CHILD" Then
                                If AutoFilter.ToUpper = "DOCID" Then
                                    str = ob.GetQuery1(arr(1).ToString, arr(2).ToString())
                                Else
                                    str = ob.GetQuery(arr(1).ToString, arr(2).ToString)
                                End If
                            ElseIf arr(0).ToUpper() <> "STATIC" Then

                                Dim Conval As String = Replace(Convert.ToString(objDC.ExecuteQryScaller("select isnull(" & val(0) & ",0) from mmm_mst_user where eid=" & HttpContext.Current.Session("EID") & " and uid=" & HttpContext.Current.Session("UID"))), ",", "','")
                                str = "select " & arr(2).ToString() & "  As [name],convert(nvarchar(10)," & TID & ")  [tid] from " & TABLENAME & " M WHERE  EID=" & HttpContext.Current.Session("EID") & " AND  DOCUMENTTYPE='" & arr(1).ToString() & "'"
                                If Conval.Length > 1 Then
                                    If SessionFieldvalue.Contains("-") Then
                                        str = str & "  AND (M.isauth>0) " & xwhr & " and " & val(2) & " in('" & Conval & "') "
                                    Else
                                        str = str & "  AND (M.isauth>0) " & xwhr & " and  tid in ('" & Conval & "') "
                                    End If

                                Else
                                    str = str & "  AND (M.isauth>0) " & xwhr
                                End If

                            Else
                                str = "select " & arr(2).ToString() & "  As [name],convert(nvarchar(10)," & TID & ")  [tid]" & lookUpqry & " from " & TABLENAME & " M WHERE EID=" & HttpContext.Current.Session("EID") & " "
                                str = str & "  AND (M.isauth>0) " & xwhr
                            End If
                        ElseIf InitFilterArr.Length > 1 Then
                            Dim daa As New SqlDataAdapter("select * from MMM_MST_Fields where fieldid=" & InitFilterArr(0) & "", con)
                            Dim dss As New DataSet
                            daa.Fill(dss, "data")
                            Dim row() As DataRow = dss.Tables("data").Select("fieldid=" & InitFilterArr(0).ToString())
                            If arr(0).ToUpper() = "DOCUMENT" Or arr(0).ToUpper() = "MASTER" Then
                                If row.Length > 0 Then
                                    'With(nolock) added by Himank on 29th sep 2015
                                    str = " Select top 50 " & arr(2).ToString() & "  As [name], convert(nvarchar(10),tid) [TID] FROM " & TABLENAME & " M  WITH(NOLOCK)  where EID=" & HttpContext.Current.Session("EID") & " AND  DOCUMENTTYPE='" & arr(1).ToString() & "'"
                                    str = str & " AND " & InitFilterArr(1).ToString() & "='" & row(0).Item("defaultFieldVal").ToString & "'"
                                    str = str & "  AND M.isauth=1 " & xwhr
                                End If
                            ElseIf arr(0).ToUpper() = "STATIC" Then
                                If row.Length > 0 Then
                                    'With(nolock) added by Himank on 29th sep 2015
                                    str = " Select top 50 " & arr(2).ToString() & "As [name],convert(nvarchar(10)," & TID & ")  [tid]" & "FROM " & TABLENAME & " M  WITH(NOLOCK)  where EID=" & HttpContext.Current.Session("EID") & " "
                                    str = str & " AND " & InitFilterArr(1).ToString() & "='" & row(0).Item("defaultFieldVal").ToString & "'"
                                    str = str & "  AND M.isauth=1 " & xwhr
                                End If
                            End If
                            dss.Dispose()
                            daa.Dispose()
                        End If
                        ' Dim da As New SqlDataAdapter("select top 50 " & contextKey.Split("-").ElementAt(2) & " As [name] , tid from " & TABLENAME & "  where EID ='" & HttpContext.Current.Session("EID") & "' and documenttype ='" & contextKey.Split("-").ElementAt(1).ToString() & "' and " & contextKey.Split("-").ElementAt(2).ToString() & " like '" & prefixText & "%' ", con)
                        ' for hidden field value check
                        'Dim pnlHdnfld As HiddenField = CType(panelfields.FindControl("HDN" & arr(3)), HiddenField)


                        ' Dim da As New SqlDataAdapter(str & " and " & contextKey.Split("-").ElementAt(2).ToString() & " like '%" & prefixText & "%'  order by " & arr(2).ToString(), con)
                        'Dim da As New SqlDataAdapter(, con)
                        Dim dsdata As New DataTable()
                        dsdata = objDC.ExecuteQryDT(str & "  order by LEN(" & arr(2).ToString() & ") ")
                        Dim items As New List(Of String)
                        Dim items1 As String = String.Empty
                        If dsdata.Rows.Count = 1 Then
                            items1 = AjaxControlToolkit.AutoCompleteExtender.CreateAutoCompleteItem(dsdata.Rows(0).Item("name").ToString(), dsdata.Rows(0).Item("tid").ToString())
                            items.Add(items1)
                            Dim txtBox As New TextBox
                            txtBox = CType(pnlFields.FindControl("fld" & ds.Rows(i).Item("FieldID").ToString()), TextBox)
                            HttpContext.Current.Session("fieldid") = contextKey.Split("-").ElementAt(3)  ' this session id we need dynamic form for calculation the other field
                            txtBox.Text = dsdata.Rows(0).Item("name").ToString()
                            Dim HDN As HiddenField = CType(panelfields.FindControl("HDN" & ds.Rows(i).Item("FieldID").ToString()), HiddenField)
                            HDN.Value = dsdata.Rows(0).Item("tid").ToString()
                            txtbox_TextChanged(txtBox, New EventArgs())
                        End If
                End Select
            Next

            '' new added by sp - may ends

        Catch ex As Exception

        End Try
    End Sub

    Private Sub txtbox_TextChanged(ByVal sender As Object, ByVal e As EventArgs)
        Try
            Dim Ac As TextBox = TryCast(sender, TextBox)
            Dim panelfields As Panel = HttpContext.Current.Session("pnlFields")
            Dim HDN As HiddenField = CType(panelfields.FindControl("HDN" & HttpContext.Current.Session("fieldid")), HiddenField)
            Dim hdnVal As Integer = HDN.Value
            Dim tablename As String() = Split(HttpContext.Current.Session("tableTID"), "||")
            If tablename.Length > 1 Then
                Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString.ToString()
                Dim con As New SqlConnection(conStr)
                Dim da As New SqlDataAdapter("select * from " & tablename(0) & " where eid=" & HttpContext.Current.Session("EID") & " and " & tablename(1) & "=" & hdnVal & " ", con)
                Dim ds As New DataSet
                da.Fill(ds, "fldID")
                If ds.Tables("fldID").Rows.Count = 1 Then
                    da.SelectCommand.CommandText = "select * from MMM_MST_FIElds where fieldid=" & HttpContext.Current.Session("fieldid") & ""
                    da.Fill(ds, "AcFldid")
                    Dim ddlText As String = ds.Tables("AcFldid").Rows(0).Item("dropdown").ToString()
                    Dim arr As String() = ddlText.Split("-")
                    If ds.Tables("AcFldid").Rows(0).Item("lookupvalue").ToString.Length > 2 Then
                        Dim lookupvalue() As String = ds.Tables("AcFldid").Rows(0).Item("lookupvalue").ToString.Split(",")
                        'For ii As Integer = 0 To lookupvalue.Length - 1
                        If ds.Tables("AcFldid").Rows(0).Item("lookupvalue").ToString.Contains("-S") Or ds.Tables("AcFldid").Rows(0).Item("lookupvalue").ToString.Contains("-R") Or ds.Tables("AcFldid").Rows(0).Item("lookupvalue").ToString.Contains("-C") Or ds.Tables("AcFldid").Rows(0).Item("lookupvalue").ToString.Contains("-fld") Then
                            bindLookUp(ds.Tables("AcFldid").Rows(0).Item("FIELDID"), HttpContext.Current.Session("pnlFields"), hdnVal)
                            bindDDlLookUpAC(ds.Tables("AcFldid").Rows(0).Item("FIELDID"), HttpContext.Current.Session("pnlFields"), hdnVal)
                        End If
                        'Next
                    End If

                End If
            End If
        Catch ex As Exception
            Exit Sub
        End Try

    End Sub

    Public Sub bindDDlLookUpAC(ByVal id1 As Integer, ByRef pnlFields As Panel, ByRef AcID As Integer)
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        If con.State <> ConnectionState.Open Then
            con.Open()
        End If
        Dim oda As SqlDataAdapter = New SqlDataAdapter("select DDllookupvalue,dropdown,documenttype,dropdowntype from MMM_MST_FIELDS WHERE FIELDID=" & id1 & "", con)
        Try
            Dim DS As New DataSet
            Dim xwhr As String = ""
            oda.Fill(DS, "data")
            Dim LOOKUPVALUE As String = DS.Tables("data").Rows(0).Item("DDllookupvalue").ToString()
            Dim documenttype() As String = DS.Tables("data").Rows(0).Item("dropdown").ToString.Split("-")
            If LOOKUPVALUE.Length > 0 Then
                Dim lookfld() As String = LOOKUPVALUE.ToString().Split(",")  '' get all controls to fill in lookup
                If lookfld.Length > 0 Then
                    For iLookFld As Integer = 0 To lookfld.Length - 1            '' loop for lookup vals 
                        Dim fldPair() As String = lookfld(iLookFld).Split("-")   '' get fieldid and mapping
                        If fldPair.Length > 1 Then
                            If GetControl(pnlFields, "fld" & Val(fldPair(0)).ToString()) Then   '' check if control to be filled exists
                                oda = New SqlDataAdapter("SELECT * FROM MMM_MST_FIELDS WHERE FIELDID=" & fldPair(0) & "", con)  ' get fld dtl from fld master
                                Dim dt As New DataTable
                                oda.Fill(dt)
                                Dim STR As String = ""
                                If fldPair(1).ToString.ToUpper = "C" Then    '' fldpair(0) = fieldid to be filled 
                                    Dim proc As String = dt.Rows(0).Item("CAL_FIELDS").ToString()
                                    If proc.Length > 1 Then
                                        Dim DROPDOWN1 As String = dt.Rows(0).Item("AUTOFILTER").ToString()
                                        Dim DDL0 As DropDownList = TryCast(pnlFields.FindControl("fld" & DROPDOWN1), DropDownList)
                                        ' bindsunil
                                        If DDL0.SelectedItem.Text.ToUpper <> "SELECT" Then
                                            oda.SelectCommand.CommandType = CommandType.StoredProcedure
                                            oda.SelectCommand.Parameters.Clear()
                                            oda.SelectCommand.CommandText = proc
                                            oda.SelectCommand.Parameters.AddWithValue("DOCID", DDL0.SelectedValue)
                                            oda.SelectCommand.Parameters.AddWithValue("FIELDID", CInt(DROPDOWN1))
                                            oda.SelectCommand.Parameters.AddWithValue("VALUE", AcID)
                                            Dim dss As New DataTable
                                            oda.Fill(dss)
                                            Dim ddl1 As DropDownList = TryCast(pnlFields.FindControl("fld" & fldPair(0).ToString()), DropDownList)
                                            ddl1.Items.Clear()
                                            ddl1.Items.Add("SELECT")
                                            ddl1.Items(0).Value = "0"
                                            For i As Integer = 0 To dss.Rows.Count - 1
                                                ddl1.Items.Add(dss.Rows(i).Item(0).ToString())
                                                ddl1.Items(i + 1).Value = dss.Rows(i).Item(1)
                                            Next
                                        End If
                                    End If
                                ElseIf fldPair(1).ToString.ToUpper = "R" Then
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
                                    Dim DOCTYPE() As String = dt.Rows(0).Item("DROPDOWN").ToString.Split("-")
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
                                    Dim DROPDOWN1 As String = dt.Rows(0).Item("AUTOFILTER").ToString()
                                    ''Dim DDL0 As DropDownList = TryCast(pnlFields.FindControl("fld" & DROPDOWN1), DropDownList)
                                    oda.SelectCommand.CommandType = CommandType.StoredProcedure
                                    oda.SelectCommand.Parameters.Clear()
                                    oda.SelectCommand.CommandText = "USP_GETMANNUALFILTER"
                                    oda.SelectCommand.Parameters.AddWithValue("@EID", HttpContext.Current.Session("EID"))
                                    oda.SelectCommand.Parameters.AddWithValue("@TAB1", TAB1)
                                    oda.SelectCommand.Parameters.AddWithValue("@TAB2", TAB2)
                                    oda.SelectCommand.Parameters.AddWithValue("@DOCUMENTTYPE", DOCTYPE(1).ToString)
                                    oda.SelectCommand.Parameters.AddWithValue("@FLDMAPPING", DOCTYPE(2).ToString)
                                    oda.SelectCommand.Parameters.AddWithValue("@AUTOFILTER", dt.Rows(0).Item("AUTOFILTER").ToString())
                                    oda.SelectCommand.Parameters.AddWithValue("@TID", TID)
                                    oda.SelectCommand.Parameters.AddWithValue("@STID", STID)
                                    oda.SelectCommand.Parameters.AddWithValue("@VAL", AcID)
                                    Dim dss As New DataTable
                                    oda.Fill(dss)
                                    Dim ddl1 As DropDownList = TryCast(pnlFields.FindControl("fld" & fldPair(0).ToString()), DropDownList)
                                    ddl1.Items.Clear()
                                    ddl1.Items.Add("SELECT")
                                    ddl1.Items(0).Value = "0"
                                    For i As Integer = 0 To dss.Rows.Count - 1
                                        ddl1.Items.Add(dss.Rows(i).Item(0).ToString())
                                        ddl1.Items(i + 1).Value = dss.Rows(i).Item(1)
                                    Next
                                Else    '' else of case 'R' and 'C' lookup (for -fld)
                                    Dim DROPDOWN As String() = dt.Rows(0).Item("DROPDOWN").ToString.Split("-")  ' this contains to be filled values 
                                    Dim TABLENAME As String = ""
                                    Dim TID As String = "TID"
                                    If UCase(DROPDOWN(0).ToString()) = "MASTER" Then
                                        TABLENAME = "MMM_MST_MASTER"
                                    ElseIf UCase(DROPDOWN(0).ToString()) = "DOCUMENT" Then
                                        TABLENAME = "MMM_MST_DOC"
                                    ElseIf UCase(DROPDOWN(0).ToString()) = "CHILD" Then
                                        TABLENAME = "MMM_MST_DOC_ITEM"
                                    ElseIf UCase(DROPDOWN(0).ToString()) = "STATIC" Then
                                        If UCase(DROPDOWN(1).ToString()) = "USER" Then
                                            TABLENAME = "MMM_MST_USER"
                                            TID = "UID"
                                        ElseIf UCase(DROPDOWN(1).ToString()) = "LOCATION" Then
                                            TABLENAME = "MMM_MST_LOCATION"
                                            If DROPDOWN(2).ToString.ToUpper = "LOCATIONSTATE" Then
                                                TID = "SID"
                                            Else
                                                TID = "LOCID"
                                            End If
                                        Else
                                            TABLENAME = dt.Rows(0).Item("DBTABLENAME").ToString
                                        End If
                                    ElseIf UCase(DROPDOWN(0).ToString()) = "SESSION" Then
                                        TABLENAME = "MMM_MST_USER"
                                        TID = "UID"
                                    End If
                                    'Dim SLVALUE As String() = DDL.SelectedValue.Split("|")


                                    If dt.Rows(0).Item("fieldtype").ToString.ToUpper() = "AUTOCOMPLETE" Then  '' IF AUTOCOMPLETE THEN EXIT SUB  ' BY SUNIL
                                        '' DO NOTHING BECAUSE CONTROL TYPE IS AUTO COMPLETE   ' BY SUNIL - 19_apr_14
                                        If GetControl(pnlFields, "fld" & Val(fldPair(0)).ToString()) Then
                                            Dim txtBox As TextBox = CType(pnlFields.FindControl("fld" & Val(fldPair(0)).ToString()), TextBox)
                                            txtBox.Text = String.Empty
                                        End If
                                    ElseIf dt.Rows(0).Item("fieldtype").ToString.ToUpper() = "DROP DOWN" Then  '' check field type 
                                        Dim AUTOFILTER As String = dt.Rows(0).Item("AUTOFILTER").ToString()
                                        Dim tids As String = ""

                                        ''Filter Data according to Userid
                                        tids = UserDataFilter(DS.Tables("data").Rows(0).Item("documenttype").ToString(), DROPDOWN(1).ToString())
                                        If tids.Length > 2 Then
                                            xwhr = " AND CONVERT(NVARCHAR(10),TID) IN (" & tids & ")"
                                        Else
                                            xwhr = ""
                                        End If

                                        Dim ChildDoctype As String = ""

                                        If DS.Tables("data").Rows(0).Item("dropdowntype") = "CHILD VALUED" Then
                                            Dim ChildMid() As String = documenttype(1).ToString.Split(":")
                                            ChildDoctype = ChildMid(1).ToString
                                        End If

                                        oda.SelectCommand.CommandType = CommandType.StoredProcedure
                                        oda.SelectCommand.CommandText = "USP_BINDDDL"
                                        oda.SelectCommand.Parameters.Clear()
                                        oda.SelectCommand.Parameters.AddWithValue("@EID", HttpContext.Current.Session("EID"))
                                        oda.SelectCommand.Parameters.AddWithValue("@TableName", TABLENAME)
                                        oda.SelectCommand.Parameters.AddWithValue("@Val", AcID)
                                        oda.SelectCommand.Parameters.AddWithValue("@xwhr", xwhr)
                                        oda.SelectCommand.Parameters.AddWithValue("@tid", TID)
                                        If ChildDoctype.Length > 0 Then
                                            oda.SelectCommand.Parameters.AddWithValue("@documenttype", ChildDoctype)
                                        Else
                                            oda.SelectCommand.Parameters.AddWithValue("@documenttype", DROPDOWN(1))
                                        End If
                                        oda.SelectCommand.Parameters.AddWithValue("@fldmapping", DROPDOWN(2))
                                        oda.SelectCommand.Parameters.AddWithValue("@autofilter", AUTOFILTER)
                                        'oda.SelectCommand.CommandText = STR & " AND isAuth=1 " & xwhr & " Order by " & DROPDOWN(2).ToString()
                                        Dim dtFinal As New DataTable
                                        oda.Fill(dtFinal)

                                        Dim ddlo As DropDownList = TryCast(pnlFields.FindControl("fld" & fldPair(0).ToString()), DropDownList)
                                        ddlo.Items.Clear()
                                        ddlo.Items.Add("SELECT")
                                        ddlo.Items(0).Value = "0"
                                        For i As Integer = 0 To dtFinal.Rows.Count - 1
                                            ddlo.Items.Add(dtFinal.Rows(i).Item(0).ToString())
                                            ddlo.Items(i + 1).Value = dtFinal.Rows(i).Item("tID")
                                        Next
                                    Else
                                        'Dim TID1 As String() = ddl.SelectedValue.ToString.Split("|")
                                        'Dim SELTID As String = ""
                                        'If TID1.Length > 1 Then
                                        '    SELTID = TID1(1).ToString
                                        'Else
                                        '    SELTID = TID1(0).ToString
                                        'End If
                                        Dim value As String = ""

                                        '' below changes made by sunil for child valued lookup on 12-dec-13
                                        Dim ChildDoctype As String = ""
                                        If DS.Tables("data").Rows(0).Item("dropdowntype") = "CHILD VALUED" Then
                                            Dim ChildMid() As String = documenttype(1).ToString.Split(":")
                                            ChildDoctype = ChildMid(1).ToString
                                        End If

                                        If AcID.ToString() <> "0" And AcID.ToString() <> "" Then
                                            oda = New SqlDataAdapter("", con)
                                            oda.SelectCommand.CommandType = CommandType.StoredProcedure
                                            oda.SelectCommand.Parameters.Clear()
                                            oda.SelectCommand.CommandText = "uspGetMasterValue"
                                            oda.SelectCommand.Parameters.AddWithValue("EID", HttpContext.Current.Session("EID"))
                                            If ChildDoctype.Length > 0 Then
                                                oda.SelectCommand.Parameters.AddWithValue("documentType", ChildDoctype)
                                            Else
                                                oda.SelectCommand.Parameters.AddWithValue("documentType", documenttype(1))
                                            End If
                                            oda.SelectCommand.Parameters.AddWithValue("Type", documenttype(0))
                                            oda.SelectCommand.Parameters.AddWithValue("TID", AcID)
                                            oda.SelectCommand.Parameters.AddWithValue("FLDMAPPING", fldPair(1))
                                            If con.State <> ConnectionState.Open Then
                                                con.Open()
                                            End If
                                            value = oda.SelectCommand.ExecuteScalar().ToString()
                                        End If
                                        Dim ddllukup As DropDownList = TryCast(pnlFields.FindControl("fld" & fldPair(0).ToString().Trim()), DropDownList)
                                        ddllukup.SelectedIndex = ddllukup.Items.IndexOf(ddllukup.Items.FindByText(value))
                                        '   bindMultiLookUP(fldPair(0), pnlFields, ddllukup)
                                        '   bindddlMultiLookUP(fldPair(0), pnlFields, ddllukup)
                                        ddllukup.BackColor = Drawing.Color.LightGray
                                        ddllukup.ForeColor = Drawing.Color.Black
                                        ddllukup.Font.Bold = True
                                    End If
                                End If
                            End If
                        End If
                    Next
                End If
            End If

        Catch ex As Exception
        Finally
            If Not con Is Nothing Then
                con.Dispose()
            End If
            If Not oda Is Nothing Then
                oda.Dispose()
            End If
        End Try
    End Sub


    Private Sub bindLookUp(ByVal id1 As Integer, ByVal pnlFields As Panel, ByVal Acid As Integer)
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        If con.State <> ConnectionState.Open Then
            con.Open()
        End If
        Dim oda As SqlDataAdapter = New SqlDataAdapter("select LOOKUPVALUE,dropdown,documenttype,dropdowntype from MMM_MST_FIELDS WHERE FIELDID=" & id1 & "", con)
        Dim DS As New DataSet
        Dim xwhr As String = ""
        oda.Fill(DS, "data")
        Dim ScreenName As String = DS.Tables("data").Rows(0).Item("documenttype").ToString()
        Dim LOOKUPVALUE As String = DS.Tables("data").Rows(0).Item("lookupvalue").ToString()
        Dim documenttype() As String = DS.Tables("data").Rows(0).Item("dropdown").ToString.Split("-")
        If LOOKUPVALUE.Length > 0 Then
            Dim lookfld() As String = LOOKUPVALUE.ToString().Split(",")  '' get all controls to fill in lookup
            If lookfld.Length > 0 Then
                For iLookFld As Integer = 0 To lookfld.Length - 1            '' loop for lookup vals 
                    Dim fldPair() As String = lookfld(iLookFld).Split("-")   '' get fieldid and mapping
                    If fldPair.Length > 1 Then
                        If GetControl(pnlFields, "fld" & Val(fldPair(0)).ToString()) Then   '' check if control to be filled exists
                            oda = New SqlDataAdapter("SELECT * FROM MMM_MST_FIELDS WHERE FIELDID=" & fldPair(0) & "", con)  ' get fld dtl from fld master
                            Dim dt As New DataTable
                            oda.Fill(dt)
                            Dim STR As String = ""
                            If fldPair(1).ToString.ToUpper = "C" Then    '' fldpair(0) = fieldid to be filled 
                                Dim proc As String = dt.Rows(0).Item("CAL_FIELDS").ToString()
                                If proc.Length > 1 Then
                                    Dim DROPDOWN1 As String = dt.Rows(0).Item("AUTOFILTER").ToString()
                                    Dim DDL0 As DropDownList = TryCast(pnlFields.FindControl("fld" & DROPDOWN1), DropDownList)
                                    ' bindsunil
                                    If DDL0.SelectedItem.Text.ToUpper <> "SELECT" Then
                                        oda.SelectCommand.CommandType = CommandType.StoredProcedure
                                        oda.SelectCommand.Parameters.Clear()
                                        oda.SelectCommand.CommandText = proc
                                        oda.SelectCommand.Parameters.AddWithValue("DOCID", DDL0.SelectedValue)
                                        oda.SelectCommand.Parameters.AddWithValue("FIELDID", CInt(DROPDOWN1))
                                        oda.SelectCommand.Parameters.AddWithValue("VALUE", Acid)
                                        Dim dss As New DataTable
                                        oda.Fill(dss)
                                        Dim ddl1 As DropDownList = TryCast(pnlFields.FindControl("fld" & fldPair(0).ToString()), DropDownList)
                                        ddl1.Items.Clear()
                                        ddl1.Items.Add("SELECT")
                                        ddl1.Items(0).Value = "0"
                                        For i As Integer = 0 To dss.Rows.Count - 1
                                            ddl1.Items.Add(dss.Rows(i).Item(0).ToString())
                                            ddl1.Items(i + 1).Value = dss.Rows(i).Item(1)
                                        Next
                                    End If
                                End If
                            ElseIf fldPair(1).ToString.ToUpper = "R" Then
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
                                Dim DOCTYPE() As String = dt.Rows(0).Item("DROPDOWN").ToString.Split("-")
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
                                Dim DROPDOWN1 As String = dt.Rows(0).Item("AUTOFILTER").ToString()
                                ''Dim DDL0 As DropDownList = TryCast(pnlFields.FindControl("fld" & DROPDOWN1), DropDownList)
                                oda.SelectCommand.CommandType = CommandType.StoredProcedure
                                oda.SelectCommand.Parameters.Clear()
                                oda.SelectCommand.CommandText = "USP_GETMANNUALFILTER"
                                oda.SelectCommand.Parameters.AddWithValue("@EID", HttpContext.Current.Session("EID"))
                                oda.SelectCommand.Parameters.AddWithValue("@TAB1", TAB1)
                                oda.SelectCommand.Parameters.AddWithValue("@TAB2", TAB2)
                                oda.SelectCommand.Parameters.AddWithValue("@DOCUMENTTYPE", DOCTYPE(1).ToString)
                                oda.SelectCommand.Parameters.AddWithValue("@FLDMAPPING", DOCTYPE(2).ToString)
                                oda.SelectCommand.Parameters.AddWithValue("@AUTOFILTER", dt.Rows(0).Item("AUTOFILTER").ToString())
                                oda.SelectCommand.Parameters.AddWithValue("@TID", TID)
                                oda.SelectCommand.Parameters.AddWithValue("@STID", STID)
                                oda.SelectCommand.Parameters.AddWithValue("@VAL", Acid)
                                Dim dss As New DataTable
                                oda.Fill(dss)

                                Dim ddl1 As DropDownList = TryCast(pnlFields.FindControl("fld" & fldPair(0).ToString()), DropDownList)
                                If Not IsNothing(ddl1) Then
                                    ddl1.Items.Clear()
                                    ddl1.Items.Add("SELECT")
                                    ddl1.Items(0).Value = "0"
                                    For i As Integer = 0 To dss.Rows.Count - 1
                                        ddl1.Items.Add(dss.Rows(i).Item(0).ToString())
                                        ddl1.Items(i + 1).Value = dss.Rows(i).Item(1)
                                    Next
                                    If dss.Rows.Count = 1 Then
                                        ddl1.SelectedIndex = 1
                                    End If
                                    If dss.Rows.Count = 1 Then
                                        ddl1.SelectedIndex = 1
                                        Dim idTemp As String = Right(ddl1.ID, ddl1.ID.Length - 3)
                                        bind(idTemp, pnlFields, ddl1)
                                        bindlookupddl(idTemp, pnlFields, ddl1)
                                    End If
                                    ddl1.BackColor = Drawing.Color.LightGray
                                    ddl1.ForeColor = Drawing.Color.Black
                                    ddl1.Font.Bold = False
                                Else
                                    Dim txtBox As TextBox = TryCast(pnlFields.FindControl("fld" & fldPair(0).ToString()), TextBox)
                                    If Not IsNothing(txtBox) Then
                                        If dss.Rows.Count > 0 Then
                                            txtBox.Text = dss.Rows(0)(DOCTYPE(2))
                                            Dim HDN As HiddenField = CType(pnlFields.FindControl("HDN" & fldPair(0).ToString()), HiddenField)
                                            HDN.Value = dss.Rows(0)("tID")
                                            HttpContext.Current.Session("Fieldid") = fldPair(0).ToString()
                                            Dim autofilExtender As AjaxControlToolkit.AutoCompleteExtender = CType(pnlFields.FindControl("extenderID" & fldPair(0).ToString()), AjaxControlToolkit.AutoCompleteExtender)
                                            oda.SelectCommand.Parameters.Clear()
                                            oda.SelectCommand.CommandType = CommandType.Text
                                            oda.SelectCommand.CommandText = " select * from mmm_mst_fields where fieldid =" & fldPair(0) & " and eid =" & HttpContext.Current.Session("EID")
                                            Dim dtfld As New DataTable()
                                            oda.Fill(dtfld)
                                            If dtfld.Rows.Count > 0 Then
                                                autofilExtender.ContextKey = dtfld.Rows(0).Item("dropdown").ToString() & "-" & dtfld.Rows(0).Item("FieldID").ToString() & "-" & dtfld.Rows(0).Item("dropdowntype").ToString() & "-" & dtfld.Rows(0).Item("autofilter").ToString() & "-" & dtfld.Rows(0).Item("InitialFilter").ToString()
                                                HttpContext.Current.Session("tableTID") = TAB1 & "||" & TID
                                                txtbox_TextChanged(txtBox, New EventArgs())
                                                txtBox.BackColor = Drawing.Color.LightGray
                                                txtBox.ForeColor = Drawing.Color.Black
                                                txtBox.Font.Bold = False
                                                txtBox.Attributes.Add("readonly", "readonly")
                                            End If
                                        ElseIf dss.Rows.Count = 0 Then
                                            'Add Code for HCL autofilter is not clear after postback
                                            Dim HDN As HiddenField = CType(pnlFields.FindControl("HDN" & fldPair(0).ToString()), HiddenField)
                                            HDN.Value = ""
                                            txtBox.Text = ""

                                            txtBox.BackColor = Drawing.Color.LightGray
                                            txtBox.ForeColor = Drawing.Color.Black
                                            txtBox.Font.Bold = False
                                            txtBox.Attributes.Add("readonly", "readonly")
                                        End If
                                    Else
                                        'For Check Box list'
                                        Dim chkBoxList As CheckBoxList = TryCast(pnlFields.FindControl("fld" & fldPair(0).ToString()), CheckBoxList)
                                        If Not IsNothing(chkBoxList) Then
                                            If chkBoxList.Items.Count > 0 Then
                                                chkBoxList.Items.Clear()
                                            End If
                                            chkBoxList.DataSource = dss
                                            chkBoxList.DataTextField = dss.Columns(0).ToString()
                                            chkBoxList.DataValueField = dss.Columns(1).ToString()
                                            chkBoxList.DataBind()
                                            chkBoxList.BackColor = Drawing.Color.LightGray
                                            chkBoxList.ForeColor = Drawing.Color.Black
                                            chkBoxList.Font.Bold = False
                                            ' For Each li As ListItem In chkBoxList.Items
                                            '  li.Selected = True
                                            ' Next
                                        End If
                                    End If
                                End If
                            Else    '' else of case 'R' and 'C' lookup (for -fld)
                                Dim DROPDOWN As String() = dt.Rows(0).Item("DROPDOWN").ToString.Split("-")  ' this contains tobe filled values 
                                Dim TABLENAME As String = ""
                                Dim TID As String = "TID"
                                If UCase(DROPDOWN(0).ToString()) = "MASTER" Then
                                    TABLENAME = "MMM_MST_MASTER"
                                ElseIf UCase(DROPDOWN(0).ToString()) = "DOCUMENT" Then
                                    TABLENAME = "MMM_MST_DOC"
                                ElseIf UCase(DROPDOWN(0).ToString()) = "CHILD" Then
                                    TABLENAME = "MMM_MST_DOC_ITEM"
                                ElseIf UCase(DROPDOWN(0).ToString()) = "STATIC" Then
                                    If UCase(DROPDOWN(1).ToString()) = "USER" Then
                                        TABLENAME = "MMM_MST_USER"
                                        TID = "UID"
                                    ElseIf UCase(DROPDOWN(1).ToString()) = "LOCATION" Then
                                        TABLENAME = "MMM_MST_LOCATION"
                                        If DROPDOWN(2).ToString.ToUpper = "LOCATIONSTATE" Then
                                            TID = "SID"
                                        Else
                                            TID = "LOCID"
                                        End If
                                    Else
                                        TABLENAME = dt.Rows(0).Item("DBTABLENAME").ToString
                                    End If
                                ElseIf UCase(DROPDOWN(0).ToString()) = "SESSION" Then
                                    TABLENAME = "MMM_MST_USER"
                                    TID = "UID"
                                End If
                                ' Dim SLVALUE As String() = Acid ''ddl.SelectedValue.Split("|")

                                If dt.Rows(0).Item("fieldtype").ToString.ToUpper() = "AUTOCOMPLETE" Then  '' check field type 
                                    If GetControl(pnlFields, "fld" & Val(fldPair(0)).ToString()) Then
                                        Dim txtBox As TextBox = CType(pnlFields.FindControl("fld" & Val(fldPair(0)).ToString()), TextBox)
                                        txtBox.Text = String.Empty
                                    End If

                                ElseIf dt.Rows(0).Item("fieldtype").ToString.ToUpper() = "DROP DOWN" Then  '' check field type 
                                    Dim AUTOFILTER As String = dt.Rows(0).Item("AUTOFILTER").ToString()
                                    Dim tids As String = ""

                                    ''Filter Data according to Userid
                                    tids = UserDataFilter(DS.Tables("data").Rows(0).Item("documenttype").ToString(), DROPDOWN(1).ToString())
                                    If tids.Length > 2 Then
                                        xwhr = " AND CONVERT(NVARCHAR(10),TID) IN (" & tids & ")"
                                    Else
                                        xwhr = ""
                                    End If

                                    Dim ChildDoctype As String = ""

                                    If DS.Tables("data").Rows(0).Item("dropdowntype") = "CHILD VALUED" Then
                                        Dim ChildMid() As String = documenttype(1).ToString.Split(":")
                                        ChildDoctype = ChildMid(1).ToString
                                    End If

                                    oda.SelectCommand.CommandType = CommandType.StoredProcedure
                                    oda.SelectCommand.CommandText = "USP_BINDDDL"
                                    oda.SelectCommand.Parameters.Clear()
                                    oda.SelectCommand.Parameters.AddWithValue("@EID", HttpContext.Current.Session("EID"))
                                    oda.SelectCommand.Parameters.AddWithValue("@TableName", TABLENAME)
                                    oda.SelectCommand.Parameters.AddWithValue("@Val", Acid)
                                    oda.SelectCommand.Parameters.AddWithValue("@xwhr", xwhr)
                                    oda.SelectCommand.Parameters.AddWithValue("@tid", TID)
                                    If ChildDoctype.Length > 0 Then
                                        oda.SelectCommand.Parameters.AddWithValue("@documenttype", ChildDoctype)
                                    Else
                                        oda.SelectCommand.Parameters.AddWithValue("@documenttype", DROPDOWN(1))
                                    End If
                                    oda.SelectCommand.Parameters.AddWithValue("@fldmapping", DROPDOWN(2))
                                    oda.SelectCommand.Parameters.AddWithValue("@autofilter", AUTOFILTER)
                                    'oda.SelectCommand.CommandText = STR & " AND isAuth=1 " & xwhr & " Order by " & DROPDOWN(2).ToString()
                                    Dim dtFinal As New DataTable
                                    oda.Fill(dtFinal)

                                    Dim ddlo As DropDownList = TryCast(pnlFields.FindControl("fld" & fldPair(0).ToString()), DropDownList)
                                    ddlo.Items.Clear()
                                    ddlo.Items.Add("SELECT")
                                    ddlo.Items(0).Value = "0"
                                    For i As Integer = 0 To dtFinal.Rows.Count - 1
                                        ddlo.Items.Add(dtFinal.Rows(i).Item(0).ToString())
                                        ddlo.Items(i + 1).Value = dtFinal.Rows(i).Item("tID")
                                    Next
                                Else
                                    'Dim TID1 As String() = "" ''ddl.SelectedValue.ToString.Split("|")
                                    'Dim SELTID As String = ""
                                    'If TID1.Length > 1 Then
                                    '    SELTID = TID1(1).ToString
                                    'Else
                                    '    SELTID = TID1(0).ToString
                                    'End If
                                    Dim value As String = ""

                                    '' below changes made by sunil for child valued lookup on 12-dec-13
                                    Dim ChildDoctype As String = ""
                                    If DS.Tables("data").Rows(0).Item("dropdowntype") = "CHILD VALUED" Then
                                        Dim ChildMid() As String = documenttype(1).ToString.Split(":")
                                        ChildDoctype = ChildMid(1).ToString
                                    End If

                                    If Acid.ToString <> "0" And Acid.ToString <> "" Then
                                        oda = New SqlDataAdapter("", con)
                                        oda.SelectCommand.CommandType = CommandType.StoredProcedure
                                        oda.SelectCommand.Parameters.Clear()
                                        oda.SelectCommand.CommandText = "uspGetMasterValue"
                                        oda.SelectCommand.Parameters.AddWithValue("EID", HttpContext.Current.Session("EID"))
                                        If ChildDoctype.Length > 0 Then
                                            oda.SelectCommand.Parameters.AddWithValue("documentType", ChildDoctype)
                                        Else
                                            oda.SelectCommand.Parameters.AddWithValue("documentType", documenttype(1))
                                        End If
                                        oda.SelectCommand.Parameters.AddWithValue("Type", documenttype(0))
                                        oda.SelectCommand.Parameters.AddWithValue("TID", Acid)
                                        oda.SelectCommand.Parameters.AddWithValue("FLDMAPPING", fldPair(1))
                                        If con.State <> ConnectionState.Open Then
                                            con.Open()
                                        End If
                                        value = oda.SelectCommand.ExecuteScalar().ToString()
                                    End If
                                    Dim TXTBOX As TextBox = TryCast(pnlFields.FindControl("fld" & fldPair(0).ToString().Trim()), TextBox)
                                    If Not IsNothing(TXTBOX) Then
                                        TXTBOX.Text = value
                                        TXTBOX.BackColor = Drawing.Color.LightGray
                                        TXTBOX.ForeColor = Drawing.Color.Black
                                        TXTBOX.Font.Bold = False
                                    Else
                                        Dim CHKBOX As CheckBox = TryCast(pnlFields.FindControl("fld" & fldPair(0).ToString().Trim()), CheckBox)
                                        If value.ToUpper = "YES" Then
                                            CHKBOX.Checked = True
                                        Else
                                            CHKBOX.Checked = False
                                        End If
                                        CHKBOX.BackColor = Drawing.Color.LightGray
                                        CHKBOX.ForeColor = Drawing.Color.Black
                                    End If

                                    'Dim proc As String = dt.Rows(0).Item("dropdowntype").ToString()
                                    'If proc.Length > 1 Then
                                    '    Dim DROPDOWN1 As String = dt.Rows(0).Item("DROPDOWN").ToString()
                                    '    Dim DDL0 As DropDownList = TryCast(pnlFields.FindControl("fld" & DROPDOWN1), DropDownList)
                                    '    oda.SelectCommand.CommandType = CommandType.StoredProcedure
                                    '    oda.SelectCommand.Parameters.Clear()
                                    '    oda.SelectCommand.CommandText = proc
                                    '    oda.SelectCommand.Parameters.AddWithValue("VcNo", value)
                                    '    oda.SelectCommand.Parameters.AddWithValue("fldmapping", fldPair(1))
                                    '    oda.SelectCommand.Parameters.AddWithValue("FIELDID", CInt(DROPDOWN1))
                                    '    oda.SelectCommand.Parameters.AddWithValue("VALUE", DDL0.SelectedValue)
                                    '    Dim dss As New DataTable
                                    '    oda.Fill(dss)
                                    '    Dim ddl1 As DropDownList = TryCast(pnlFields.FindControl("fld" & dt.Rows(0).Item("autofilter").ToString()), DropDownList)
                                    '    ddl1.Items.Clear()
                                    '    ddl1.Items.Add("SELECT")
                                    '    ddl1.Items(0).Value = "0"
                                    '    For i As Integer = 0 To dss.Rows.Count - 1
                                    '        ddl1.Items.Add(dss.Rows(i).Item(0).ToString())
                                    '        ddl1.Items(i + 1).Value = dss.Rows(i).Item("tID")
                                    '    Next
                                    'End If
                                End If
                            End If
                        End If
                    End If
                Next
                oda = New SqlDataAdapter("", con)
                oda.SelectCommand.CommandText = "select * from mmm_mst_fields where documenttype='" & ScreenName & "' and eid=" & HttpContext.Current.Session("EID") & " and isActive=1"
                Dim dts As New DataTable
                oda.Fill(dts)
                ExecuteControllevelRule(id1, pnlFields, Nothing, ScreenName, dts, Nothing, Nothing, False)
            End If
        End If
        con.Dispose()
        oda.Dispose()



    End Sub


    Public Sub bind(ByVal id1 As Integer, ByRef pnlFields As Panel, ByRef ddl As DropDownList)
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        If con.State <> ConnectionState.Open Then
            con.Open()
        End If
        Dim oda As SqlDataAdapter = New SqlDataAdapter("select LOOKUPVALUE,dropdown,documenttype,dropdowntype from MMM_MST_FIELDS WHERE FIELDID=" & id1 & "", con)
        Try
            Dim DS As New DataSet
            Dim xwhr As String = ""
            oda.Fill(DS, "data")
            Dim LOOKUPVALUE As String = DS.Tables("data").Rows(0).Item("lookupvalue").ToString()
            Dim documenttype() As String = DS.Tables("data").Rows(0).Item("dropdown").ToString.Split("-")
            If LOOKUPVALUE.Length > 0 Then
                Dim lookfld() As String = LOOKUPVALUE.ToString().Split(",")  '' get all controls to fill in lookup
                If lookfld.Length > 0 Then
                    For iLookFld As Integer = 0 To lookfld.Length - 1            '' loop for lookup vals 
                        Dim fldPair() As String = lookfld(iLookFld).Split("-")   '' get fieldid and mapping
                        If fldPair.Length > 1 Then
                            If GetControl(pnlFields, "fld" & Val(fldPair(0)).ToString()) Then   '' check if control to be filled exists
                                oda = New SqlDataAdapter("SELECT * FROM MMM_MST_FIELDS WHERE FIELDID=" & fldPair(0) & "", con)  ' get fld dtl from fld master
                                Dim dt As New DataTable
                                oda.Fill(dt)
                                Dim STR As String = ""
                                If fldPair(1).ToString.ToUpper = "C" Then    '' fldpair(0) = fieldid to be filled 
                                    Dim proc As String = dt.Rows(0).Item("CAL_FIELDS").ToString()
                                    If proc.Length > 1 Then
                                        Dim DROPDOWN1 As String = dt.Rows(0).Item("AUTOFILTER").ToString()
                                        Dim DDL0 As DropDownList = TryCast(pnlFields.FindControl("fld" & DROPDOWN1), DropDownList)
                                        ' bindsunil
                                        If DDL0.SelectedItem.Text.ToUpper <> "SELECT" Then
                                            oda.SelectCommand.CommandType = CommandType.StoredProcedure
                                            oda.SelectCommand.Parameters.Clear()
                                            oda.SelectCommand.CommandText = proc
                                            oda.SelectCommand.Parameters.AddWithValue("DOCID", DDL0.SelectedValue)
                                            oda.SelectCommand.Parameters.AddWithValue("FIELDID", CInt(DROPDOWN1))
                                            oda.SelectCommand.Parameters.AddWithValue("VALUE", ddl.SelectedValue)
                                            Dim dss As New DataTable
                                            oda.Fill(dss)
                                            Dim ddl1 As DropDownList = TryCast(pnlFields.FindControl("fld" & fldPair(0).ToString()), DropDownList)
                                            ddl1.Items.Clear()
                                            ddl1.Items.Add("SELECT")
                                            ddl1.Items(0).Value = "0"
                                            For i As Integer = 0 To dss.Rows.Count - 1
                                                ddl1.Items.Add(dss.Rows(i).Item(0).ToString())
                                                ddl1.Items(i + 1).Value = dss.Rows(i).Item(1)
                                            Next
                                        End If
                                    End If
                                ElseIf fldPair(1).ToString.ToUpper = "R" Then
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
                                    Dim DOCTYPE() As String = dt.Rows(0).Item("DROPDOWN").ToString.Split("-")
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
                                    Dim DROPDOWN1 As String = dt.Rows(0).Item("AUTOFILTER").ToString()
                                    ''Dim DDL0 As DropDownList = TryCast(pnlFields.FindControl("fld" & DROPDOWN1), DropDownList)
                                    oda.SelectCommand.CommandType = CommandType.StoredProcedure
                                    oda.SelectCommand.Parameters.Clear()
                                    oda.SelectCommand.CommandText = "USP_GETMANNUALFILTER"
                                    oda.SelectCommand.Parameters.AddWithValue("@EID", HttpContext.Current.Session("EID"))
                                    oda.SelectCommand.Parameters.AddWithValue("@TAB1", TAB1)
                                    oda.SelectCommand.Parameters.AddWithValue("@TAB2", TAB2)
                                    oda.SelectCommand.Parameters.AddWithValue("@DOCUMENTTYPE", DOCTYPE(1).ToString)
                                    oda.SelectCommand.Parameters.AddWithValue("@FLDMAPPING", DOCTYPE(2).ToString)
                                    oda.SelectCommand.Parameters.AddWithValue("@AUTOFILTER", dt.Rows(0).Item("AUTOFILTER").ToString())
                                    oda.SelectCommand.Parameters.AddWithValue("@TID", TID)
                                    oda.SelectCommand.Parameters.AddWithValue("@STID", STID)
                                    oda.SelectCommand.Parameters.AddWithValue("@VAL", ddl.SelectedValue)
                                    Dim dss As New DataTable
                                    oda.Fill(dss)
                                    Dim ddl1 As DropDownList = TryCast(pnlFields.FindControl("fld" & fldPair(0).ToString()), DropDownList)
                                    If Not IsNothing(ddl1) Then
                                        ddl1.Items.Clear()
                                        ddl1.Items.Add("SELECT")
                                        ddl1.Items(0).Value = "0"
                                        For i As Integer = 0 To dss.Rows.Count - 1
                                            ddl1.Items.Add(dss.Rows(i).Item(0).ToString())
                                            ddl1.Items(i + 1).Value = dss.Rows(i).Item(1)
                                        Next
                                        If dss.Rows.Count = 1 Then
                                            ddl1.SelectedIndex = 1
                                            Dim idTemp As String = Right(ddl1.ID, ddl1.ID.Length - 3)
                                            bind(idTemp, pnlFields, ddl1)
                                            bindlookupddl(idTemp, pnlFields, ddl1)
                                            ddl1.BackColor = Drawing.Color.LightGray
                                            ddl1.ForeColor = Drawing.Color.Black
                                            ddl1.Font.Bold = False
                                        End If
                                    Else
                                        Dim TXTBOX As TextBox = TryCast(pnlFields.FindControl("fld" & fldPair(0).ToString()), TextBox)
                                        If dss.Rows.Count > 0 Then
                                            TXTBOX.Text = dss.Rows(0)(DOCTYPE(2))
                                            Dim HDN As HiddenField = CType(pnlFields.FindControl("HDN" & fldPair(0).ToString()), HiddenField)
                                            HDN.Value = dss.Rows(0)("tID")
                                            HttpContext.Current.Session("Fieldid") = fldPair(0).ToString()
                                            Dim autofilExtender As AjaxControlToolkit.AutoCompleteExtender = CType(pnlFields.FindControl("extenderID" & fldPair(0).ToString()), AjaxControlToolkit.AutoCompleteExtender)
                                            oda.SelectCommand.Parameters.Clear()
                                            oda.SelectCommand.CommandType = CommandType.Text
                                            oda.SelectCommand.CommandText = " select * from mmm_mst_fields where fieldid =" & fldPair(0) & " and eid =" & HttpContext.Current.Session("EID")
                                            Dim dtfld As New DataTable()
                                            oda.Fill(dtfld)
                                            If dtfld.Rows.Count > 0 Then
                                                autofilExtender.ContextKey = dtfld.Rows(0).Item("dropdown").ToString() & "-" & dtfld.Rows(0).Item("FieldID").ToString() & "-" & dtfld.Rows(0).Item("dropdowntype").ToString() & "-" & dtfld.Rows(0).Item("autofilter").ToString() & "-" & dtfld.Rows(0).Item("InitialFilter").ToString()
                                                HttpContext.Current.Session("tableTID") = TAB1 & "||" & TID
                                                txtbox_TextChanged(TXTBOX, New EventArgs())
                                                TXTBOX.BackColor = Drawing.Color.LightGray
                                                TXTBOX.ForeColor = Drawing.Color.Black
                                                TXTBOX.Font.Bold = False
                                            End If
                                        End If
                                    End If
                                    'ddl1.Items.Clear()
                                    'ddl1.Items.Add("SELECT")
                                    'ddl1.Items(0).Value = "0"
                                    'For i As Integer = 0 To dss.Rows.Count - 1
                                    '    ddl1.Items.Add(dss.Rows(i).Item(0).ToString())
                                    '    ddl1.Items(i + 1).Value = dss.Rows(i).Item("tID")
                                    'Next
                                Else    '' else of case 'R' and 'C' lookup (for -fld)
                                    Dim DROPDOWN As String() = dt.Rows(0).Item("DROPDOWN").ToString.Split("-")  ' this contains to be filled values 
                                    Dim TABLENAME As String = ""
                                    Dim TID As String = "TID"
                                    If UCase(DROPDOWN(0).ToString()) = "MASTER" Then
                                        TABLENAME = "MMM_MST_MASTER"
                                    ElseIf UCase(DROPDOWN(0).ToString()) = "DOCUMENT" Then
                                        TABLENAME = "MMM_MST_DOC"
                                    ElseIf UCase(DROPDOWN(0).ToString()) = "CHILD" Then
                                        TABLENAME = "MMM_MST_DOC_ITEM"
                                    ElseIf UCase(DROPDOWN(0).ToString()) = "STATIC" Then
                                        If UCase(DROPDOWN(1).ToString()) = "USER" Then
                                            TABLENAME = "MMM_MST_USER"
                                            TID = "UID"
                                        ElseIf UCase(DROPDOWN(1).ToString()) = "LOCATION" Then
                                            TABLENAME = "MMM_MST_LOCATION"
                                            If DROPDOWN(2).ToString.ToUpper = "LOCATIONSTATE" Then
                                                TID = "SID"
                                            Else
                                                TID = "LOCID"
                                            End If
                                        Else
                                            TABLENAME = dt.Rows(0).Item("DBTABLENAME").ToString
                                        End If
                                    ElseIf UCase(DROPDOWN(0).ToString()) = "SESSION" Then
                                        TABLENAME = "MMM_MST_USER"
                                        TID = "UID"
                                    End If
                                    Dim SLVALUE As String() = ddl.SelectedValue.Split("|")
                                    If dt.Rows(0).Item("fieldtype").ToString.ToUpper() = "AUTOCOMPLETE" Then  '' IF AUTOCOMPLETE THEN EXIT SUB  ' BY SUNIL
                                        '' DO NOTHING BECAUSE CONTROL TYPE IS AUTO COMPLETE   ' BY SUNIL - 19_apr_14
                                        If GetControl(pnlFields, "fld" & Val(fldPair(0)).ToString()) Then
                                            Dim txtBox As TextBox = CType(pnlFields.FindControl("fld" & Val(fldPair(0)).ToString()), TextBox)
                                            txtBox.Text = String.Empty
                                        End If
                                    ElseIf dt.Rows(0).Item("fieldtype").ToString.ToUpper() = "DROP DOWN" Then  '' check field type 
                                        Dim AUTOFILTER As String = dt.Rows(0).Item("AUTOFILTER").ToString()
                                        Dim tids As String = ""

                                        ''Filter Data according to Userid
                                        tids = UserDataFilter(DS.Tables("data").Rows(0).Item("documenttype").ToString(), DROPDOWN(1).ToString())
                                        If tids.Length > 2 Then
                                            xwhr = " AND CONVERT(NVARCHAR(10),TID) IN (" & tids & ")"
                                        Else
                                            xwhr = ""
                                        End If

                                        Dim ChildDoctype As String = ""

                                        If DS.Tables("data").Rows(0).Item("dropdowntype") = "CHILD VALUED" Then
                                            Dim ChildMid() As String = documenttype(1).ToString.Split(":")
                                            ChildDoctype = ChildMid(1).ToString
                                        End If

                                        oda.SelectCommand.CommandType = CommandType.StoredProcedure
                                        oda.SelectCommand.CommandText = "USP_BINDDDL"
                                        oda.SelectCommand.Parameters.Clear()
                                        oda.SelectCommand.Parameters.AddWithValue("@EID", HttpContext.Current.Session("EID"))
                                        oda.SelectCommand.Parameters.AddWithValue("@TableName", TABLENAME)
                                        oda.SelectCommand.Parameters.AddWithValue("@Val", SLVALUE(0))
                                        oda.SelectCommand.Parameters.AddWithValue("@xwhr", xwhr)
                                        oda.SelectCommand.Parameters.AddWithValue("@tid", TID)
                                        If ChildDoctype.Length > 0 Then
                                            oda.SelectCommand.Parameters.AddWithValue("@documenttype", ChildDoctype)
                                        Else
                                            oda.SelectCommand.Parameters.AddWithValue("@documenttype", DROPDOWN(1))
                                        End If
                                        oda.SelectCommand.Parameters.AddWithValue("@fldmapping", DROPDOWN(2))
                                        oda.SelectCommand.Parameters.AddWithValue("@autofilter", AUTOFILTER)
                                        'oda.SelectCommand.CommandText = STR & " AND isAuth=1 " & xwhr & " Order by " & DROPDOWN(2).ToString()
                                        Dim dtFinal As New DataTable
                                        oda.Fill(dtFinal)

                                        Dim ddlo As DropDownList = TryCast(pnlFields.FindControl("fld" & fldPair(0).ToString()), DropDownList)
                                        ddlo.Items.Clear()
                                        ddlo.Items.Add("SELECT")
                                        ddlo.Items(0).Value = "0"
                                        For i As Integer = 0 To dtFinal.Rows.Count - 1
                                            ddlo.Items.Add(dtFinal.Rows(i).Item(0).ToString())
                                            ddlo.Items(i + 1).Value = dtFinal.Rows(i).Item("tID")
                                        Next
                                        If ddlo.Items.Count = 2 Then
                                            ddlo.SelectedIndex = 1
                                            'Add lookup value for child item 01-Nov-2107
                                            Dim idTemp As String = Right(ddlo.ID, ddlo.ID.Length - 3)
                                            '           FilterChildGridDDL(idTemp, pnlFields:=pnlFields)
                                        End If
                                    Else
                                        Dim TID1 As String() = ddl.SelectedValue.ToString.Split("|")
                                        Dim SELTID As String = ""
                                        If TID1.Length > 1 Then
                                            SELTID = TID1(1).ToString
                                        Else
                                            SELTID = TID1(0).ToString
                                        End If
                                        Dim value As String = ""

                                        '' below changes made by sunil for child valued lookup on 12-dec-13
                                        Dim ChildDoctype As String = ""
                                        If DS.Tables("data").Rows(0).Item("dropdowntype") = "CHILD VALUED" Then
                                            Dim ChildMid() As String = documenttype(1).ToString.Split(":")
                                            ChildDoctype = ChildMid(1).ToString
                                        End If

                                        If SELTID.ToString <> "0" And SELTID.ToString <> "" Then
                                            oda = New SqlDataAdapter("", con) 'For show text option in lookupfield By Pallavi
                                            oda.SelectCommand.CommandType = CommandType.Text
                                            oda.SelectCommand.CommandText = "select isnull(IflookupMVShowValue,1) from mmm_mst_fields where fieldid =" & fldPair(0) & ""
                                            If con.State <> ConnectionState.Open Then
                                                con.Open()
                                            End If

                                            Dim IfLookupMVShowValue = oda.SelectCommand.ExecuteScalar().ToString()

                                            If (IfLookupMVShowValue = "False") Then ' if iflookupmvshowvalue = false , then show tids 
                                                oda = New SqlDataAdapter("", con)
                                                oda.SelectCommand.CommandType = CommandType.Text
                                                Dim TBLNM As String = ""
                                                If Convert.ToString(documenttype(0)) = "MASTER" Then
                                                    TBLNM = "mmm_mst_master"
                                                ElseIf Convert.ToString(documenttype(0)) = "MASTER" Then
                                                    TBLNM = "mmm_mst_doc"
                                                ElseIf Convert.ToString(documenttype(0)) = "CHILD" Then
                                                    TBLNM = "mmm_mst_doc_item"
                                                End If
                                                If ChildDoctype.Length > 0 Then
                                                    oda.SelectCommand.CommandText = "select " & fldPair(1) & " from " & TBLNM & " where tid =" & SELTID & ""
                                                Else
                                                    oda.SelectCommand.CommandText = "select " & fldPair(1) & " from " & TBLNM & " where tid =" & SELTID & ""
                                                End If

                                                If con.State <> ConnectionState.Open Then
                                                    con.Open()
                                                End If
                                                value = oda.SelectCommand.ExecuteScalar().ToString()

                                            Else ' else show respective text
                                                oda = New SqlDataAdapter("", con)
                                                oda.SelectCommand.CommandType = CommandType.StoredProcedure
                                                oda.SelectCommand.Parameters.Clear()
                                                oda.SelectCommand.CommandText = "uspGetMasterValue"
                                                oda.SelectCommand.Parameters.AddWithValue("EID", HttpContext.Current.Session("EID"))
                                                If ChildDoctype.Length > 0 Then
                                                    oda.SelectCommand.Parameters.AddWithValue("documentType", ChildDoctype)
                                                Else
                                                    oda.SelectCommand.Parameters.AddWithValue("documentType", documenttype(1))
                                                End If
                                                oda.SelectCommand.Parameters.AddWithValue("Type", documenttype(0))
                                                oda.SelectCommand.Parameters.AddWithValue("TID", SELTID)
                                                oda.SelectCommand.Parameters.AddWithValue("FLDMAPPING", fldPair(1))
                                                If con.State <> ConnectionState.Open Then
                                                    con.Open()
                                                End If
                                                value = oda.SelectCommand.ExecuteScalar().ToString()
                                            End If
                                        End If

                                        Dim TXTBOX As TextBox = TryCast(pnlFields.FindControl("fld" & fldPair(0).ToString().Trim()), TextBox)
                                        If TXTBOX Is Nothing Then
                                            Dim chkBox As CheckBox = TryCast(pnlFields.FindControl("fld" & fldPair(0).ToString().Trim()), CheckBox)
                                            If value.ToUpper = "YES" Then
                                                chkBox.Checked = True
                                            Else
                                                chkBox.Checked = False
                                            End If
                                        Else
                                            TXTBOX.Text = value
                                        End If

                                    End If
                                End If
                            End If
                        End If
                    Next
                End If
            End If

        Catch ex As Exception
        Finally
            If Not con Is Nothing Then
                con.Dispose()
            End If
            If Not oda Is Nothing Then
                oda.Dispose()
            End If
        End Try
    End Sub

    Public Sub bindlookupddl(ByVal id1 As Integer, ByRef pnlFields As Panel, ByRef ddl As DropDownList)
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        If con.State <> ConnectionState.Open Then
            con.Open()
        End If
        Dim oda As SqlDataAdapter = New SqlDataAdapter("select DDllookupvalue,dropdown,documenttype,dropdowntype from MMM_MST_FIELDS WHERE FIELDID=" & id1 & "", con)
        Try
            Dim DS As New DataSet
            Dim xwhr As String = ""
            oda.Fill(DS, "data")
            Dim LOOKUPVALUE As String = DS.Tables("data").Rows(0).Item("DDllookupvalue").ToString()
            Dim documenttype() As String = DS.Tables("data").Rows(0).Item("dropdown").ToString.Split("-")
            If LOOKUPVALUE.Length > 0 Then
                Dim lookfld() As String = LOOKUPVALUE.ToString().Split(",")  '' get all controls to fill in lookup
                If lookfld.Length > 0 Then
                    For iLookFld As Integer = 0 To lookfld.Length - 1            '' loop for lookup vals 
                        Dim fldPair() As String = lookfld(iLookFld).Split("-")   '' get fieldid and mapping
                        If fldPair.Length > 1 Then
                            If GetControl(pnlFields, "fld" & Val(fldPair(0)).ToString()) Then   '' check if control to be filled exists
                                oda = New SqlDataAdapter("SELECT * FROM MMM_MST_FIELDS WHERE FIELDID=" & fldPair(0) & "", con)  ' get fld dtl from fld master
                                Dim dt As New DataTable
                                oda.Fill(dt)
                                Dim STR As String = ""
                                If fldPair(1).ToString.ToUpper = "C" Then    '' fldpair(0) = fieldid to be filled 
                                    Dim proc As String = dt.Rows(0).Item("CAL_FIELDS").ToString()
                                    If proc.Length > 1 Then
                                        Dim DROPDOWN1 As String = dt.Rows(0).Item("AUTOFILTER").ToString()
                                        Dim DDL0 As DropDownList = TryCast(pnlFields.FindControl("fld" & DROPDOWN1), DropDownList)
                                        ' bindsunil
                                        If DDL0.SelectedItem.Text.ToUpper <> "SELECT" Then
                                            oda.SelectCommand.CommandType = CommandType.StoredProcedure
                                            oda.SelectCommand.Parameters.Clear()
                                            oda.SelectCommand.CommandText = proc
                                            oda.SelectCommand.Parameters.AddWithValue("DOCID", DDL0.SelectedValue)
                                            oda.SelectCommand.Parameters.AddWithValue("FIELDID", CInt(DROPDOWN1))
                                            oda.SelectCommand.Parameters.AddWithValue("VALUE", ddl.SelectedValue)
                                            Dim dss As New DataTable
                                            oda.Fill(dss)
                                            Dim ddl1 As DropDownList = TryCast(pnlFields.FindControl("fld" & fldPair(0).ToString()), DropDownList)
                                            ddl1.Items.Clear()
                                            ddl1.Items.Add("SELECT")
                                            ddl1.Items(0).Value = "0"
                                            For i As Integer = 0 To dss.Rows.Count - 1
                                                ddl1.Items.Add(dss.Rows(i).Item(0).ToString())
                                                ddl1.Items(i + 1).Value = dss.Rows(i).Item(1)
                                            Next
                                        End If
                                    End If
                                ElseIf fldPair(1).ToString.ToUpper = "R" Then
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
                                    Dim DOCTYPE() As String = dt.Rows(0).Item("DROPDOWN").ToString.Split("-")
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
                                    Dim DROPDOWN1 As String = dt.Rows(0).Item("AUTOFILTER").ToString()
                                    ''Dim DDL0 As DropDownList = TryCast(pnlFields.FindControl("fld" & DROPDOWN1), DropDownList)
                                    oda.SelectCommand.CommandType = CommandType.StoredProcedure
                                    oda.SelectCommand.Parameters.Clear()
                                    oda.SelectCommand.CommandText = "USP_GETMANNUALFILTER"
                                    oda.SelectCommand.Parameters.AddWithValue("@EID", HttpContext.Current.Session("EID"))
                                    oda.SelectCommand.Parameters.AddWithValue("@TAB1", TAB1)
                                    oda.SelectCommand.Parameters.AddWithValue("@TAB2", TAB2)
                                    oda.SelectCommand.Parameters.AddWithValue("@DOCUMENTTYPE", DOCTYPE(1).ToString)
                                    oda.SelectCommand.Parameters.AddWithValue("@FLDMAPPING", DOCTYPE(2).ToString)
                                    oda.SelectCommand.Parameters.AddWithValue("@AUTOFILTER", dt.Rows(0).Item("AUTOFILTER").ToString())
                                    oda.SelectCommand.Parameters.AddWithValue("@TID", TID)
                                    oda.SelectCommand.Parameters.AddWithValue("@STID", STID)
                                    oda.SelectCommand.Parameters.AddWithValue("@VAL", ddl.SelectedValue)
                                    Dim dss As New DataTable
                                    oda.Fill(dss)
                                    Dim ddl1 As DropDownList = TryCast(pnlFields.FindControl("fld" & fldPair(0).ToString()), DropDownList)
                                    ddl1.Items.Clear()
                                    ddl1.Items.Add("SELECT")
                                    ddl1.Items(0).Value = "0"
                                    For i As Integer = 0 To dss.Rows.Count - 1
                                        ddl1.Items.Add(dss.Rows(i).Item(0).ToString())
                                        ddl1.Items(i + 1).Value = dss.Rows(i).Item(1)
                                    Next
                                Else    '' else of case 'R' and 'C' lookup (for -fld)
                                    Dim DROPDOWN As String() = dt.Rows(0).Item("DROPDOWN").ToString.Split("-")  ' this contains to be filled values 
                                    Dim TABLENAME As String = ""
                                    Dim TID As String = "TID"
                                    If UCase(DROPDOWN(0).ToString()) = "MASTER" Then
                                        TABLENAME = "MMM_MST_MASTER"
                                    ElseIf UCase(DROPDOWN(0).ToString()) = "DOCUMENT" Then
                                        TABLENAME = "MMM_MST_DOC"
                                    ElseIf UCase(DROPDOWN(0).ToString()) = "CHILD" Then
                                        TABLENAME = "MMM_MST_DOC_ITEM"
                                    ElseIf UCase(DROPDOWN(0).ToString()) = "STATIC" Then
                                        If UCase(DROPDOWN(1).ToString()) = "USER" Then
                                            TABLENAME = "MMM_MST_USER"
                                            TID = "UID"
                                        ElseIf UCase(DROPDOWN(1).ToString()) = "LOCATION" Then
                                            TABLENAME = "MMM_MST_LOCATION"
                                            If DROPDOWN(2).ToString.ToUpper = "LOCATIONSTATE" Then
                                                TID = "SID"
                                            Else
                                                TID = "LOCID"
                                            End If
                                        Else
                                            TABLENAME = dt.Rows(0).Item("DBTABLENAME").ToString
                                        End If
                                    ElseIf UCase(DROPDOWN(0).ToString()) = "SESSION" Then
                                        TABLENAME = "MMM_MST_USER"
                                        TID = "UID"
                                    End If
                                    Dim SLVALUE As String() = ddl.SelectedValue.Split("|")
                                    If dt.Rows(0).Item("fieldtype").ToString.ToUpper() = "AUTOCOMPLETE" Then  '' IF AUTOCOMPLETE THEN EXIT SUB  ' BY SUNIL
                                        '' DO NOTHING BECAUSE CONTROL TYPE IS AUTO COMPLETE   ' BY SUNIL - 19_apr_14
                                        If GetControl(pnlFields, "fld" & Val(fldPair(0)).ToString()) Then
                                            Dim txtBox As TextBox = CType(pnlFields.FindControl("fld" & Val(fldPair(0)).ToString()), TextBox)
                                            txtBox.Text = String.Empty
                                        End If
                                    ElseIf dt.Rows(0).Item("fieldtype").ToString.ToUpper() = "DROP DOWN" Then  '' check field type 
                                        Dim AUTOFILTER As String = dt.Rows(0).Item("AUTOFILTER").ToString()
                                        Dim tids As String = ""

                                        ''Filter Data according to Userid
                                        tids = UserDataFilter(DS.Tables("data").Rows(0).Item("documenttype").ToString(), DROPDOWN(1).ToString())
                                        If tids.Length > 2 Then
                                            xwhr = " AND CONVERT(NVARCHAR(10),TID) IN (" & tids & ")"
                                        Else
                                            xwhr = ""
                                        End If

                                        Dim ChildDoctype As String = ""

                                        If DS.Tables("data").Rows(0).Item("dropdowntype") = "CHILD VALUED" Then
                                            Dim ChildMid() As String = documenttype(1).ToString.Split(":")
                                            ChildDoctype = ChildMid(1).ToString
                                        End If

                                        oda.SelectCommand.CommandType = CommandType.StoredProcedure
                                        oda.SelectCommand.CommandText = "USP_BINDDDL"
                                        oda.SelectCommand.Parameters.Clear()
                                        oda.SelectCommand.Parameters.AddWithValue("@EID", HttpContext.Current.Session("EID"))
                                        oda.SelectCommand.Parameters.AddWithValue("@TableName", TABLENAME)
                                        oda.SelectCommand.Parameters.AddWithValue("@Val", SLVALUE(0))
                                        oda.SelectCommand.Parameters.AddWithValue("@xwhr", xwhr)
                                        oda.SelectCommand.Parameters.AddWithValue("@tid", TID)
                                        If ChildDoctype.Length > 0 Then
                                            oda.SelectCommand.Parameters.AddWithValue("@documenttype", ChildDoctype)
                                        Else
                                            oda.SelectCommand.Parameters.AddWithValue("@documenttype", DROPDOWN(1))
                                        End If
                                        oda.SelectCommand.Parameters.AddWithValue("@fldmapping", DROPDOWN(2))
                                        oda.SelectCommand.Parameters.AddWithValue("@autofilter", AUTOFILTER)
                                        'oda.SelectCommand.CommandText = STR & " AND isAuth=1 " & xwhr & " Order by " & DROPDOWN(2).ToString()
                                        Dim dtFinal As New DataTable
                                        oda.Fill(dtFinal)

                                        Dim ddlo As DropDownList = TryCast(pnlFields.FindControl("fld" & fldPair(0).ToString()), DropDownList)
                                        ddlo.Items.Clear()
                                        ddlo.Items.Add("SELECT")
                                        ddlo.Items(0).Value = "0"
                                        For i As Integer = 0 To dtFinal.Rows.Count - 1
                                            ddlo.Items.Add(dtFinal.Rows(i).Item(0).ToString())
                                            ddlo.Items(i + 1).Value = dtFinal.Rows(i).Item("tID")
                                        Next
                                    Else
                                        Dim TID1 As String() = ddl.SelectedValue.ToString.Split("|")
                                        Dim SELTID As String = ""
                                        If TID1.Length > 1 Then
                                            SELTID = TID1(1).ToString
                                        Else
                                            SELTID = TID1(0).ToString
                                        End If
                                        Dim value As String = ""

                                        '' below changes made by sunil for child valued lookup on 12-dec-13
                                        Dim ChildDoctype As String = ""
                                        If DS.Tables("data").Rows(0).Item("dropdowntype") = "CHILD VALUED" Then
                                            Dim ChildMid() As String = documenttype(1).ToString.Split(":")
                                            ChildDoctype = ChildMid(1).ToString
                                        End If

                                        If SELTID.ToString <> "0" And SELTID.ToString <> "" Then
                                            oda = New SqlDataAdapter("", con)
                                            oda.SelectCommand.CommandType = CommandType.StoredProcedure
                                            oda.SelectCommand.Parameters.Clear()
                                            oda.SelectCommand.CommandText = "uspGetMasterValue"
                                            oda.SelectCommand.Parameters.AddWithValue("EID", HttpContext.Current.Session("EID"))
                                            If ChildDoctype.Length > 0 Then
                                                oda.SelectCommand.Parameters.AddWithValue("documentType", ChildDoctype)
                                            Else
                                                oda.SelectCommand.Parameters.AddWithValue("documentType", documenttype(1))
                                            End If
                                            oda.SelectCommand.Parameters.AddWithValue("Type", documenttype(0))
                                            oda.SelectCommand.Parameters.AddWithValue("TID", SELTID)
                                            oda.SelectCommand.Parameters.AddWithValue("FLDMAPPING", fldPair(1))
                                            If con.State <> ConnectionState.Open Then
                                                con.Open()
                                            End If
                                            value = oda.SelectCommand.ExecuteScalar().ToString()
                                        End If
                                        Dim ddllukup As DropDownList = TryCast(pnlFields.FindControl("fld" & fldPair(0).ToString().Trim()), DropDownList)
                                        ddllukup.SelectedIndex = ddllukup.Items.IndexOf(ddllukup.Items.FindByText(value))
                                        ' bindMultiLookUP(fldPair(0), pnlFields, ddllukup)
                                       ' bindddlMultiLookUP(fldPair(0), pnlFields, ddllukup)
                                        ddllukup.BackColor = Drawing.Color.LightGray
                                        ddllukup.ForeColor = Drawing.Color.Black
                                        ddllukup.Font.Bold = False

                                    End If
                                End If
                            End If
                        End If
                    Next
                End If
            End If

        Catch ex As Exception
        Finally
            If Not con Is Nothing Then
                con.Dispose()
            End If
            If Not oda Is Nothing Then
                oda.Dispose()
            End If
        End Try
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

    'Public Function FollowUPshowHide() As Boolean
    '    Dim objDC As New DataClass
    '    Dim response As String = objDC.ExecuteQryScaller("declare @column nvarchar(max),@Qry nvarchar(max)set @Qry='select @column= (select '+ (select fieldmapping from mmm_mst_fields with(nolock) where documenttype in (select documenttype from mmm_mst_doc with(nolock) where tid =" & Request.QueryString("DOCID") & ") and eid=" & Session("EID") & " and mdfieldname ='Status') +' from mmm_mst_doc with(nolock) where tid=" & Request.QueryString("DOCID") & ")' exec sp_executesql @Qry,N'@column nvarchar(max) OUTPUT', @column = @column output select  @column")
    '    If response.ToString.ToUpper = "CLOSED" And hdnStatus.Value = "" Then
    '        hdnStatus.Value = "CLOSED"
    '        ddlAssignee.Attributes.Add("disabled", "disabled")
    '        txtCC.Attributes.Add("readonly", "readonly")
    '        'ddlPriority.Attributes.Add("disabled", "disabled")
    '        'txtTags.Attributes.Add("readonly", "readonly")
    '        useraction.Visible = False
    '        txtSubject.Attributes.Add("readonly", "readonly")
    '        HEE_body.Enabled = False
    '        noaction.Visible = True
    '        EnableDisableControl(False)
    '    Else
    '        ddlAssignee.Attributes.Remove("disabled")
    '        txtCC.Attributes.Remove("readonly")
    '        'ddlPriority.Attributes.Remove("disabled")
    '        'txtTags.Attributes.Remove("readonly")
    '        useraction.Visible = True
    '        txtSubject.Attributes.Remove("readonly")
    '        HEE_body.Enabled = True
    '        noaction.Visible = False
    '    End If
    'End Function
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

    Protected Sub TextBoxRule_TextChanged(sender As Object, e As EventArgs)
        Try
            Dim txt As Control = TryCast(sender, Control)
            Dim id As String = Right(txt.ID, txt.ID.Length - 3)
            Dim id1 As Integer = CInt(id)
            Dim screenname = Request.QueryString("SC").ToString
            Dim dt As DataTable = Session("PFieldsData")
            ExecuteControllevelRule(id1, pnlnewfields, Nothing, screenname, dt, Nothing, lblTab, False)
        Catch ex As Exception
            Throw
        End Try
    End Sub



    Public Sub ExecuteControllevelRule(ctrlID As String, pnlP As Panel, pnlC As Panel, screenname As String, dtparent As DataTable, dtChild As DataTable, ErrorLbl As Label, Optional IsChildForm As Boolean = False)
        Dim RE As New RuleEngin()
        RE.ExecuteControllevelRule(ctrlID, pnlP, pnlC, screenname, dtparent, dtChild, ErrorLbl, IsChildForm, HttpContext.Current.CurrentHandler)
    End Sub

End Class
