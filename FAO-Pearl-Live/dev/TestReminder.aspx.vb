Imports System.Data

Partial Class TestReminder


    Inherits System.Web.UI.Page

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Hcl_VENDOR_INVOICE_VP_alert_REMINDER()
    End Sub

    Private Sub Hcl_VENDOR_INVOICE_VP_alert_REMINDER()
        Dim objDC As New DataClass()
        Dim dtUsers As New DataTable()
        Dim arrWFStatus() = {"APPROVER 1", "APPROVER 2", "APPROVER 3", "APPROVER 4"}
        Dim arrInoviceType() = {"958564", "958565", "1078985"}
        Dim EventName As String = "Vendor Invoice VP"
        Dim bsla As Integer = 2
        Dim asla As Integer = 99999
        Dim eid As Integer = 46
        For i As Integer = 0 To arrWFStatus.Length - 1
            Dim MailTo As New ArrayList()
            Dim MailSubject As String = ""
            Dim MailCC As New ArrayList()
            Dim MailBody As New StringBuilder()
            Dim headerCount As Integer = 0
            For j As Integer = 0 To arrInoviceType.Length - 1
                Dim TempMailBody As New StringBuilder()
                Dim ht As New Hashtable()
                ht.Add("@En", EventName)
                ht.Add("@Ws", arrWFStatus(i))
                ht.Add("@Asla", asla)
                ht.Add("@Bsla", bsla)
                ht.Add("@Eid", eid)
                ht.Add("@FTYPE", arrInoviceType(j))
                dtUsers = objDC.ExecuteProDT("uspAlertMailSLA_getAllUsers_VIPHCL", ht)
                TempMailBody.Append("<table width=""100%"" border=""3px"" cellpadding=""1px"" cellspacing=""1px""> ")
                Dim flag As Boolean = False
                For k As Integer = 0 To dtUsers.Rows.Count - 1
                    If Not IsDBNull(dtUsers.Rows(k).Item("userid")) Then
                        Dim dtDtl As New DataTable()
                        Dim curUser As Integer = dtUsers.Rows(k).Item("userid").ToString()
                        ht.Clear()
                        ht.Add("@En", EventName)
                        ht.Add("@Ws", arrWFStatus(i))
                        ht.Add("@Asla", asla)
                        ht.Add("@Bsla", bsla)
                        ht.Add("@Eid", eid)
                        ht.Add("@curuser", curUser)
                        dtDtl = objDC.ExecuteProDT("uspAlertMailSLA_getUserIDList", ht)
                        Dim ColList As String = ""
                        If dtDtl.Rows.Count <> 0 Then
                            flag = True
                            ColList = dtDtl.Rows(0).Item(0).ToString()
                            Dim strQry As String = "SELECT  DMS.GetCurrentWorkflowUser(tid,eid) [CURRENT USER], DMS.GetCreatorEmail(oUid) [CREATOR EMAIL], DMS.GetMasterEmail(tid,eid) [OWNER EMAIL], DMS.GetAllCurrentWorkflowUser(tid,eid) [ALL WORKFLOW USERS], DMS.GetSysTicketID(tid) [SYSTICKETID], DMS.GetTicketDelimiter() [TicketDelimiter], DMS.GetFooterText(eid) [FooterText], dms.LastActionName(tid) [LastActionName], dms.LastActionDate(tid) [LastActionDate], dms.LastActionTaken(tid) [LastActionTaken], ( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld96,0) from mmm_mst_master where tid=isnull(d.fld96,0) and documenttype='Invoice Type Master')) [Invoice Type],( select username from mmm_mst_user where  uid=d.fld187) [Created By],( select fld11 from mmm_mst_master where  tid=(select isnull(d.fld70,0) from mmm_mst_master where tid=isnull(d.fld70,0) and documenttype='PO MASTER')) [PO No],fld26[PO Value WO Tax],fld153[Balance PO amount],fld121[Plant],( select fld2 from mmm_mst_master where  tid=(select isnull(d.fld128,0) from mmm_mst_master where tid=isnull(d.fld128,0) and documenttype='Plant Master')) [Plant Name],fld81[Valid From],fld82[Valid To],fld47[BPM ID],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld178,0) from mmm_mst_master where tid=isnull(d.fld178,0) and documenttype='Purchase Group')) [PUR GP],fld179[Tax Code],fld180[Payment Terms],fld181[INCOTerms],fld32[Location],( select fld25 from mmm_mst_master where  tid=(select isnull(d.fld25,0) from mmm_mst_master where tid=isnull(d.fld25,0) and documenttype='Department Master')) [Department],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld27,0) from mmm_mst_master where tid=isnull(d.fld27,0) and documenttype='WBS')) [WBS No],( select username from mmm_mst_user where  uid=d.fld120) [L2 Approver],fld160[WBS Description],( select username from mmm_mst_user where  uid=d.fld161) [L3 Approver],( select username from mmm_mst_user where  uid=d.fld162) [L4 Approver],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld60,0) from mmm_mst_master where tid=isnull(d.fld60,0) and documenttype='Profit Center')) [Profit Center],fld159[Profit Center Description],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld100,0) from mmm_mst_master where tid=isnull(d.fld100,0) and documenttype='Cost Center')) [Cost Center],fld158[Cost Center Description],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld137,0) from mmm_mst_master where tid=isnull(d.fld137,0) and documenttype='WBS')) [WBS (Non-PO)],fld110[sep3forPO],fld107[Sep2forPO],( select username from mmm_mst_user where  uid=d.fld119) [L1 Approver],fld106[sep1forPO],fld111[sep4forPO],fld95[sep5forPO],( select fld15 from mmm_mst_master where  tid=(select isnull(d.fld17,0) from mmm_mst_master where tid=isnull(d.fld17,0) and documenttype='Vendor')) [Vendor],fld68[Vendor Name],fld39[Vendor Recon Acc],fld18[Vendor Code],fld104[Vendor TIN],fld105[Vendor PAN],fld118[Service Tax Number],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld42,0) from mmm_mst_master where tid=isnull(d.fld42,0) and documenttype='Doc Nature')) [Doc Nature],( select fld10 from mmm_mst_master where  tid=(select isnull(d.fld14,0) from mmm_mst_master where tid=isnull(d.fld14,0) and documenttype='Company Master')) [Company Code],fld3[RAO Remarks],fld15[Company Name],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld136,0) from mmm_mst_master where tid=isnull(d.fld136,0) and documenttype='Profit Center')) [Profit Center (Non-PO)],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld135,0) from mmm_mst_master where tid=isnull(d.fld135,0) and documenttype='Cost Center')) [Cost Center (Non-PO)],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld5,0) from mmm_mst_master where tid=isnull(d.fld5,0) and documenttype='Clarification User')) [Received From User],fld58[Email Of User],fld2[Currency],fld31[Dispatch Remarks],fld34[Physical Doc (Recd)],fld54[Current Date 1],fld132[Is RCM Applicable],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld134,0) from mmm_mst_master where tid=isnull(d.fld134,0) and documenttype='Service Tax Category Master')) [Service Tax Category],fld12[SSC Processing Date],fld182[Service Tax Categor],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld102,0) from mmm_mst_master where tid=isnull(d.fld102,0) and documenttype='Service Type')) [Type of Service],fld156[PF Challan],fld157[ESI Challan],fld10[Invoice No],fld11[Invoice Date],fld20[Invoice Amount WO Tax],fld103[Service Tax Amount],fld22[GST Value],fld114[CST/VAT],fld115[Excise Duty],fld116[Total Invoice Amount],fld33[Invoice Attachment],fld124[Low TDS Applicable],fld125[Low TDS Certificate],fld129[Remarks If any],fld130[Invoicing Milestone Description],fld108[Sep1forChklist],fld83[Invoice as per PO Attachment],fld28[Invoice as per PO],fld29[Packing list with clear desc],fld23[Clarification Remarks],fld84[Packing list with clear desc Attachment],fld71[Transit Insurance Certificate],fld35[Remarks by HCL Receipt User],fld85[Transit Insurance Certificate Attachment],fld72[Warranty Certificate],fld52[Processor Cancellation Remarks],fld53[Processor Reconsider Remarks],fld86[Warranty Certificate Attachment],fld73[Performance Bank Guarantee],fld87[Performance Bank Guarantee Attachment],fld74[Inst and Commisioning Certificate],fld88[Inst and Commisioning Certificate Attachment],fld40[Courier Name ],fld75[Technical Compliance Certificate],fld4[Courier Docket No.],fld30[Dispatch Date],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld56,0) from mmm_mst_master where tid=isnull(d.fld56,0) and documenttype='Rejection Reasons')) [Rejection Reason],fld89[Technical Compliance Certificate Attachment],fld76[Proof of delivery],fld9[Proof of delivery Attachment],fld51[Query Sent To],fld77[Factory Test Reports],fld90[Factory Test Reports Attachment],fld78[Newness Certificate],fld91[Newness Certificate Attachment],fld57[Parking Date],fld79[Proof of Electronics delivery],fld36[SSC Processing Remarks],fld92[Proof of Electronics delivery Attachment],fld8[Work completion certificate],fld93[Work completion certificate Attachment],fld80[Work initiation certificate],fld94[Work initiation certificate Attachment],fld55[Current Date 2],fld46[Parking SAP Doc ID],fld13[Other Deduction],fld61[Parking Fiscal Year],fld131[Other Optional Document],fld69[SAP Doc ID Posted],( select fld10 from mmm_mst_master where  tid=(select isnull(d.fld6,0) from mmm_mst_master where tid=isnull(d.fld6,0) and documenttype='Clarification User')) [User Email],fld67[Open Amount],fld7[Fiscal Year Posted],fld62[HC Rejection Remarks],fld63[Invoice Received Date],fld43[MIGO No],fld64[Dispatch Reconsider Remarks],fld65[QC Reconsider Remarks],( select username from mmm_mst_user where  uid=d.fld50) [Clarification by user],fld21[TDS],fld19[Vendor Address],fld97[L1 Reconsider Remarks],fld99[L2 Reconsider Remarks],fld122[L5 Reconsider Remarks],fld123[Deduction Amount],fld41[GR or Service Entry No],fld138[RTV Courier Details],fld139[RTV Date],fld140[RTV Remarks],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld141,0) from mmm_mst_master where tid=isnull(d.fld141,0) and documenttype='Rejection Reasons')) [Physical Rejection Reason],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld49,0) from mmm_mst_master where tid=isnull(d.fld49,0) and documenttype='Workflow Rejection')) [is RTV],fld142[Physical Rejection Remarks],fld143[Additional Scan by Physical],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld144,0) from mmm_mst_master where tid=isnull(d.fld144,0) and documenttype='Rejection Reasons')) [Reason for RTV],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld145,0) from mmm_mst_master where tid=isnull(d.fld145,0) and documenttype='Rejection Reasons')) [L1 Reconsider Reason],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld146,0) from mmm_mst_master where tid=isnull(d.fld146,0) and documenttype='Rejection Reasons')) [L2 Reconsider Reason],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld147,0) from mmm_mst_master where tid=isnull(d.fld147,0) and documenttype='Rejection Reasons')) [GR Reconsider Reason],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld148,0) from mmm_mst_master where tid=isnull(d.fld148,0) and documenttype='Rejection Reasons')) [Processor Rejection Reason],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld149,0) from mmm_mst_master where tid=isnull(d.fld149,0) and documenttype='Rejection Reasons')) [QC Rejection Reason],fld150[Processor Rejection Remarks],fld151[GR Reconsider Remarks],fld154[GR Date],fld155[GR Amount],fld152[PO Number],( select fld3 from mmm_mst_master where  tid=(select isnull(d.fld164,0) from mmm_mst_master where tid=isnull(d.fld164,0) and documenttype='Retainer Master')) [Consultant Emp Code],fld166[Consultant Vendor Code],fld167[Consultant Name],fld168[Residence Address],fld169[Mobile Number],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld174,0) from mmm_mst_master where tid=isnull(d.fld174,0) and documenttype='Rejection Reasons')) [L3 Reconsider Reason],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld175,0) from mmm_mst_master where tid=isnull(d.fld175,0) and documenttype='Rejection Reasons')) [L4 Reconsider Reason],fld171[Contract Expiry],fld172[Retainer Department],fld176[L3 Reconsider Remarks],fld177[L4 Reconsider Remarks],fld173[Consultant PAN],fld185[Monthly Fee],fld189[Invoice_Type],fld190[Bank Ac No],( select fld10 from mmm_mst_master where  tid=(select isnull(d.fld191,0) from mmm_mst_master where tid=isnull(d.fld191,0) and documenttype='Company Master')) [Company Code Retainer],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld192,0) from mmm_mst_master where tid=isnull(d.fld192,0) and documenttype='Cost Center')) [Cost Center Retainer],fld59[Company_Name],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld38,0) from mmm_mst_master where tid=isnull(d.fld38,0) and documenttype='Location')) [Location_Name],fld44[Manager Name],fld37[Supplementary Bill],fld183[Bill Dated],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld188,0) from mmm_mst_master where tid=isnull(d.fld188,0) and documenttype='Pay Month')) [For Month],fld193[Total Leaves],fld195[Working Days],fld194[Amount Payable ],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld196,0) from mmm_mst_master where tid=isnull(d.fld196,0) and documenttype='WBS')) [WBS No (Project)],fld163[Supporting Attachment],fld197[Last Date],fld198[Amount Claimed],fld199[Payable Amount],( select username from mmm_mst_user where  uid=d.fld200) [L5 Approver],( select fld1 from mmm_mst_master where  tid=(select isnull(d.fld201,0) from mmm_mst_master where tid=isnull(d.fld201,0) and documenttype='Rejection Reasons')) [L5 Reconsider Reason],fld202[HRSS Remarks (If any)],fld203[Actual Working Days],fld204[Vendor Blocked],fld205[Central Posting Blocked],fld206[PO Deleted],( select username from mmm_mst_user where  uid=d.fld207) [GRN User],fld208[PO Date] from MMM_MST_DOC d  " & " where tid in (" & ColList & ")"
                            Dim dtQuery As New DataTable()
                            dtQuery = objDC.ExecuteQryDT(strQry)
                            If arrInoviceType(j) = "1078985" Then
                                TempMailBody.Append("<p style=""background-color:#6D7B8D;color:white"" ><b>Invoice Type :- RETAINERS</b></p><tr style=""background-color:Azure""> ")
                                TempMailBody.Append("<td>Consultant Emp. Code</td><td>Consultant Vendor Code</td><td>Consultant Name</td><td>Contract Expiry</td><td>Invoice for Month</td><td>Working Days</td><td>Amount Claimed</td><td>Mobile Number</td><td>Company Code</td><td>Cost Center</td><td>Department</td><td>Last Action by</td><td> Last Action Date</td><td>Last Action Taken</td>")
                            ElseIf arrInoviceType(j) = "958565" Then
                                'NON Po Based
                                TempMailBody.Append("<p style=""background-color:#6D7B8D;color:white"" ><b>Invoice Type :- NON PO Based</b></p><tr style=""background-color:Azure""> ")
                                TempMailBody.Append("<td>Vendor Code</td><td>Vendor Name</td><td>WBS No.</td><td>Department</td><td>Invoice No.</td><td>Invoice Date</td><td>Invoice Value (with Tax)</td><td>Last Action by</td><td> Last Action Date</td><td>Last Action Taken</td>")
                            Else
                                'PO Based
                                TempMailBody.Append("<p style=""background-color:#6D7B8D;color:white"" ><b>Invoice Type :- PO Based</b></p><tr style=""background-color:Azure""> ")
                                TempMailBody.Append("<td>PO No.</td><td>PO Date</td><td>PO Value (w/o tax)</td><td>Vendor Code</td><td>Vendor Name</td><td>WBS No.</td><td>Department</td><td>Invoice No.</td><td>Invoice Date</td><td>Invoice Value (with Tax)</td><td>Last Action by</td><td> Last Action Date</td><td>Last Action Taken</td>")
                            End If
                            If i = 0 Then
                                MailSubject = "Reminder - Pending Vendor Invoice Approvals (Level-1)"
                                TempMailBody.Append("<td>L2 Approver</td></tr>")
                                MailCC.Add(objDC.ExecuteQryScaller("SELECT STUFF((SELECT ',' +  emailid			  from mmm_mst_user where uid in ( select distinct fld120[L2] from mmm_mst_doc where  tid in (" & ColList & "))			  FOR XML PATH('')), 1, 1, '') AS [Output]"))
                            ElseIf i = 1 Then
                                MailSubject = "Reminder - Pending Vendor Invoice Approvals (Level-2)"
                                TempMailBody.Append("<td>L3 Approver</td></tr>")
                                MailCC.Add(objDC.ExecuteQryScaller("SELECT STUFF((SELECT ',' +  emailid			  from mmm_mst_user where uid in ( select distinct fld161[L3] from mmm_mst_doc where  tid in (" & ColList & "))			  FOR XML PATH('')), 1, 1, '') AS [Output]"))
                            ElseIf i = 2 Then
                                MailSubject = "Reminder - Pending Vendor Invoice Approvals (Level-3)"
                                TempMailBody.Append("<td>L4 Approver</td></tr>")
                                MailCC.Add(objDC.ExecuteQryScaller("SELECT STUFF((SELECT ',' +  emailid			  from mmm_mst_user where uid in ( select distinct fld162[L4] from mmm_mst_doc where  tid in (" & ColList & "))			  FOR XML PATH('')), 1, 1, '') AS [Output]"))
                            Else
                                MailSubject = "Reminder - Pending Vendor Invoice Approvals (Level-4)"
                                MailCC.Add(objDC.ExecuteQryScaller("SELECT STUFF((SELECT ',' +  emailid			  from mmm_mst_user where uid in ( select distinct fld200[L5] from mmm_mst_doc where  tid in (" & ColList & "))			  FOR XML PATH('')), 1, 1, '') AS [Output]"))
                                TempMailBody.Append("<td>L5 Approver</td></tr>")
                            End If
                            For Each dr As DataRow In dtQuery.Rows
                                'For Retainer
                                MailTo.Add(Convert.ToString(dr("CURRENT USER")))
                                If arrInoviceType(j) = "1078985" Then
                                    TempMailBody.Append("<tr><td>" & Convert.ToString(dr("Consultant Emp Code")) & "</td><td>" & Convert.ToString(dr("Consultant Vendor Code")) & "</td><td>" & Convert.ToString(dr("Consultant Name")) & "</td><td>" & Convert.ToString(dr("Contract Expiry")) & "</td><td>" & Convert.ToString(dr("For Month")) & "</td><td>" & Convert.ToString(dr("Working Days")) & "</td><td>(INR)" & Convert.ToString(dr("Amount Claimed")) & "</td><td>" & Convert.ToString(dr("Mobile Number")) & "</td><td>" & Convert.ToString(dr("Company Code Retainer")) & "</td><td>" & Convert.ToString(dr("Cost Center Retainer")) & "</td><td>" & Convert.ToString(dr("Department")) & "</td><td>" & Convert.ToString(dr("LastActionName")) & "</td><td>" & Convert.ToString(dr("LastActionDate")) & "</td><td>" & Convert.ToString(dr("LastActionTaken")) & "</td>")
                                ElseIf arrInoviceType(j) = "958565" Then
                                    TempMailBody.Append("<tr><td>" & Convert.ToString(dr("Vendor Code")) & "</td><td>" & Convert.ToString(dr("Vendor Name")) & "</td><td>" & Convert.ToString(dr("WBS No")) & "</td><td>" & Convert.ToString(dr("Department")) & "</td><td>" & Convert.ToString(dr("Invoice No")) & "</td><td>" & Convert.ToString(dr("Invoice Date")) & "</td><td>" & Convert.ToString(dr("Total Invoice Amount")) & "</td><td>" & Convert.ToString(dr("LastActionName")) & "</td><td>" & Convert.ToString(dr("LastActionDate")) & "</td><td>" & Convert.ToString(dr("LastActionTaken")) & "</td>")
                                Else
                                    TempMailBody.Append("<tr><td>" & Convert.ToString(dr("PO No")) & "</td><td>" & Convert.ToString(dr("PO DATE")) & "</td><td>" & Convert.ToString(dr("PO Value WO Tax")) & "</td><td>" & Convert.ToString(dr("Vendor Code")) & "</td><td>" & Convert.ToString(dr("Vendor Name")) & "</td><td>" & Convert.ToString(dr("WBS No")) & "</td><td>" & Convert.ToString(dr("Department")) & "</td><td>" & Convert.ToString(dr("Invoice No")) & "</td><td>" & Convert.ToString(dr("Invoice Date")) & "</td><td>(INR) " & Convert.ToString(dr("Total Invoice Amount")) & "</td><td>" & Convert.ToString(dr("LastActionName")) & "</td><td>" & Convert.ToString(dr("LastActionDate")) & "</td><td>" & Convert.ToString(dr("LastActionTaken")) & "</td>")
                                End If
                                If i = 0 Then
                                    TempMailBody.Append("<td>" & Convert.ToString(dr("L2 Approver")) & "</td></tr>")
                                ElseIf i = 1 Then
                                    TempMailBody.Append("<td>" & Convert.ToString(dr("L3 Approver")) & "</td></tr>")
                                ElseIf i = 2 Then
                                    TempMailBody.Append("<td>" & Convert.ToString(dr("L4 Approver")) & "</td></tr>")
                                Else
                                    TempMailBody.Append("<td>" & Convert.ToString(dr("L5 Approver")) & "</td></tr>")
                                End If
                            Next
                        Else
                            flag = False
                        End If
                    End If
                Next
                TempMailBody.Append("</table>")
                If flag = True Then
                    If headerCount = 0 Then
                        MailBody.Append("<p style=""margin: 0in 0in 0.0001pt;""><b style=""text-align: center; line-height: 1.6em;"">Hi, </b></p><br><font size=""2""><b>The SLA for the following Vendor Invoices have <u>Expired</u>. Please log in to your HCL Vendor Invoice Services Portal Account to Approve/Reconsider of these invoices immediately:<br><br></b></font>")
                        headerCount = headerCount + 1
                    End If
                    MailBody.Append(TempMailBody.ToString())
                End If
            Next
            Dim finalBody As String = MailBody.ToString()
            If finalBody.Length > 10 Then
                MailBody.Append("<p style=""margin: 0in 0in 0.0001pt;""><br></p><p style=""margin: 0in 0in 0.0001pt;""><br></p>&lt;h5&gt;<font size=""2"">If you wish to access your account, you need to <a href=""http://hcl.myndsaas.com/"">login Here</a></font>&lt;/h5&gt;<p style=""margin: 0in 0in 0.0001pt;""></p><p><b>Regards</b></p><p><b style=""font-size: medium; text-align: center; line-height: 1.6em; background-color: transparent;"">HCL Vendor Support Team</b></p><b style=""font-size: 12px; text-align: center; line-height: 1.6em; background-color: transparent;""><div><b style=""font-size: 12px; text-align: center; line-height: 1.6em; background-color: transparent;""><br></b></div>This eMail is a service from HCL Vendor Invoices Service Management System</b><b style=""font-size: 12px; text-align: center; line-height: 1.6em; background-color: transparent;"">!</b><br><br>")
                finalBody = MailBody.ToString()
                Dim TOs As String = String.Join(",", MailTo.ToArray().Distinct())
                Dim htTran As New Hashtable()
                htTran.Add("@MAILTO", TOs)
                htTran.Add("@CC", String.Join(",", MailCC.ToArray()))
                htTran.Add("@MSG", finalBody.ToString())
                htTran.Add("@ALERTTYPE", "ALERT MAIL")
                htTran.Add("@MAILEVENT", EventName & "-" & arrWFStatus(i).ToString())
                htTran.Add("@EID", eid)
                objDC.ExecuteProDT("INSERT_MAILLOG", htTran)
                sendMail1(TOs, String.Join(",", MailCC.ToArray()), "tanweer.alam@myndsol.com,mayank.garg@myndsol.com", MailSubject, finalBody)
            End If
        Next

    End Sub
    Private Sub sendMail1(ByVal Mto As String, ByVal cc As String, ByVal bcc As String, ByVal MSubject As String, ByVal MBody As String)
        'Dim Email As New System.Net.Mail.MailMessage("no-reply@myndsol.com", "manish@myndsol.com", MSubject, MBody & Mto)
        Try
            If Left(Mto, 1) = "{" Then
                Exit Sub
            End If
            Dim Email As New System.Net.Mail.MailMessage("no-reply@myndsol.com", Mto, MSubject, MBody)
            Dim mailClient As New System.Net.Mail.SmtpClient()
            Email.IsBodyHtml = True
            If cc <> "" Then
                Email.CC.Add(cc)
            End If

            If bcc <> "" Then
                Email.Bcc.Add(bcc)
            End If
            Dim basicAuthenticationInfo As New System.Net.NetworkCredential("no-reply@myndsol.com", "Dn#Ms@538Ti")
            mailClient.Host = "mail.myndsol.com"
            mailClient.UseDefaultCredentials = False
            mailClient.Credentials = basicAuthenticationInfo
            'mailClient.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.PickupDirectoryFromIis
            Try
                mailClient.Send(Email)
            Catch ex As Exception
                Exit Sub
            End Try
        Catch ex As Exception
            Exit Sub
        End Try
    End Sub


End Class