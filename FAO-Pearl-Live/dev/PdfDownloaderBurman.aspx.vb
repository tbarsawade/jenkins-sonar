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


Partial Class PdfDownloaderBurman
    Inherits System.Web.UI.Page
    Private Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click
        TemplateCallingDownload(Trim(txtDocid.Text), 162, "QC Document", "CREATED")
    End Sub

    Private Sub btnSendEmail_Click(sender As Object, e As EventArgs) Handles btnSendEmail.Click
        TemplateCallingMail(Trim(txtDocid.Text), 162, "QC Document", "CREATED")
    End Sub
    Public Sub TemplateCallingDownload(ByVal tid As Integer, ByVal eid As Integer, ByVal en As String, ByVal SUBEVENT As String, Optional ByVal upCommingFrom As String = "", Optional ByVal TicketCC As String = "")
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
        Dim da As SqlDataAdapter = New SqlDataAdapter("Select documenttype,curstatus,curdocnature from MMM_MST_DOC where tid=" & tid, con)
        Dim DS As New DataSet
        Dim WFstatus As String = "Not REQUIRED"
        Dim curdocnature As String = ""
        code = objDC.ExecuteQryScaller("Select code from mmm_mst_entity where eid=" & eid)
        'Dim obj As New MailUtill(eid:=eid)
        Dim Filepath As String = HostingEnvironment.MapPath("~/MailAttach/")
        'try Catch Block Added by Ajeet Kumar :Date::22 May 2014

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

            da.SelectCommand.CommandText = "Select Code FROM MMM_MST_Entity where EID=" & eid
            da.Fill(DS, "eCode")
            Dim ECode = DS.Tables("eCode").Rows(0).Item("Code")

            da.SelectCommand.CommandText = "Select T.TID,T.msgBody,T.subject,T.MAILTO,T.CC,T.BCC,T.EventName,T.qry,T.specialfieldflag,isnull(T.condition,'') as condition from MMM_MST_TEMPLATE T  WHERE T.EVENTNAME='" & en & "' AND T.SUBEVENT='" & SUBEVENT & "' AND t.statusName='" & WFstatus & "' and EID=" & eid & " and t.docnature='" & curdocnature & "' "
            da.Fill(DS, "TEMP")
            If DS.Tables("TEMP").Rows.Count > 0 Then
                MSG = HttpUtility.HtmlDecode(DS.Tables("TEMP").Rows(0).Item("msgbody").ToString())
                subject = DS.Tables("TEMP").Rows(0).Item("SUBJECT").ToString()
                MAILTO = DS.Tables("TEMP").Rows(0).Item("MAILTO").ToString()
                cc = DS.Tables("TEMP").Rows(0).Item("CC").ToString()
                Bcc = DS.Tables("TEMP").Rows(0).Item("BCC").ToString()
                MainEvent = DS.Tables("TEMP").Rows(0).Item("EVENTNAME").ToString()
                STR = DS.Tables("TEMP").Rows(0).Item("qry").ToString()
            Else
                da.SelectCommand.CommandText = "select T.TID,T.msgBody,T.subject,T.MAILTO,T.CC,T.BCC,T.EventName,T.qry,T.specialfieldflag, isnull(T.condition,'') as condition from MMM_MST_TEMPLATE T  WHERE T.EVENTNAME='" & en & "' AND T.SUBEVENT='" & SUBEVENT & "' AND t.statusName='" & WFstatus & "' AND EID=0 and t.docnature='" & curdocnature & "' "
                da.Fill(DS, "TEMP")
                If DS.Tables("TEMP").Rows.Count <> 0 Then
                    MSG = HttpUtility.HtmlDecode(DS.Tables("TEMP").Rows(0).Item("msgbody").ToString())
                    subject = DS.Tables("TEMP").Rows(0).Item("SUBJECT").ToString()
                    MAILTO = DS.Tables("TEMP").Rows(0).Item("MAILTO").ToString()
                    cc = DS.Tables("TEMP").Rows(0).Item("CC").ToString()
                    Bcc = DS.Tables("TEMP").Rows(0).Item("BCC").ToString()
                    MainEvent = DS.Tables("TEMP").Rows(0).Item("EVENTNAME").ToString()
                    STR = DS.Tables("TEMP").Rows(0).Item("qry")
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
                            Dim Q1 As String = "DOCID=" & tid
                            Q1 = objEncryptionDescription.EncryptTripleDES(Q1, eid)
                            'EncryptTripleDES(Q1, eid)
                            Dim Q2 As String = "USERID=" & Convert.ToString(da.SelectCommand.ExecuteScalar())
                            Q2 = objEncryptionDescription.EncryptTripleDES(Q2, eid)
                            'Q2 = EncryptTripleDES(Q2, eid)
                            Dim Q3 As String = "CURSTATUS=" & WFstatus
                            'Q3 = EncryptTripleDES(Q3, eid)
                            Q3 = objEncryptionDescription.EncryptTripleDES(Q3, eid)
                            'MSG = MSG.Replace("{ApprovalThroughMail}", "<br/><br/><table cellspacing=""0"" cellpadding=""0""><tr><td align=""center"" width=""300"" height=""40"" bgcolor=""#000091"" style=""-webkit-border-radius: 5px; -moz-border-radius: 5px; border-radius: 5px; color: #ffffff; display: block;""><a href=""http://" & ECode.ToString() & ".myndsaas.com/ActionMailApproval.aspx?Q1=" & Q1 & "&Q2= " & Q2 & "&Q3=" & Q3 & """  style=""font-size:16px; font-weight: bold; font-family: Helvetica, Arial, sans-serif; text-decoration: none; line-height:40px;width:100%; display:inline-block""><span style=""color: #FFFFFF"">Approve Document</span></a></td></tr></table>")
                            MSG = MSG.Replace("{ApprovalThroughMail}", "<br/><table cellspacing=""0"" cellpadding=""0""><tr><td align=""center"" width=""300"" height=""40"" bgcolor=""#000091"" style=""-webkit-border-radius: 5px; -moz-border-radius: 5px; border-radius: 5px; color: #ffffff; display: block;""><a href=""http://" & ECode.ToString() & ".myndsaas.com/ActionMailApproval.aspx?Q1=" & Q1 & "&Q2= " & Q2 & "&Q3=" & Q3 & """  style=""font-size:16px; font-weight: bold; font-family: Helvetica, Arial, sans-serif; text-decoration: none; line-height:40px;width:100%; display:inline-block""><span style=""color: #FFFFFF"">Approve</span></a></td></tr></table>")
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

                            If con.State <> ConnectionState.Open Then
                                con.Open()
                            End If
                            RES = da.SelectCommand.ExecuteScalar()
                            con.Close()
                        End If
                        If MSG.Contains("MailAttach") Then
                            Dim da1 As SqlDataAdapter = New SqlDataAdapter("select * from MMM_Print_Template where documenttype='" & en & "' and draft='original'", con)
                            Dim ds1 As New DataSet
                            da1.Fill(ds1, "dataset")
                            da.SelectCommand.CommandType = CommandType.Text
                            da.SelectCommand.CommandText = "select formcaption from mmm_mst_forms where formname='" & en & "' and eid=" & eid
                            da.Fill(ds1, "dataset1")
                            For k As Integer = 0 To ds1.Tables("dataset").Rows.Count - 1
                                Pdffile = GenerateMailPdf(ds1.Tables("dataset1").Rows(0).Item(0).ToString() & "_" & eid & "_" & "print" & ds1.Tables("dataset").Rows(k).Item("tid").ToString(), tid, en, "NA", curdocnature)
                                If Not Pdffile Is Nothing And Not String.IsNullOrEmpty(Pdffile) And ds1.Tables("dataset").Rows(k).Item("SendType").ToString() = "PDF" Then
                                    Pdffile = HostingEnvironment.MapPath("~/MailAttach/" & Pdffile & ".pdf")
                                    Response.ContentType = "Application/pdf"
                                    Response.AppendHeader("Content-Disposition", "attachment; filename=" & ds1.Tables("dataset1").Rows(0).Item(0).ToString() & "_" & eid & "_" & "print" & ds1.Tables("dataset").Rows(k).Item("tid").ToString() & ".pdf")
                                    Response.TransmitFile(Pdffile)
                                    Response.End()
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

                                            '  sendMailExl(MAILTO, cc, Bcc, exlsub, exlmsg, CreateCSVR(exceldata, exlsub))

                                            'obj.SendMail(ToMail:=MAILTO, Subject:=exlsub, MailBody:=exlmsg, CC:=cc, Attachments:=Filepath + CreateCSVR(exceldata, exlsub), BCC:=Bcc)

                                            'sendMail(ToMail:=MAILTO, subject:=exlsub, MailBody:=exlmsg, cc:=cc, Attachments:=Filepath + CreateCSVR(exceldata, exlsub), Bcc:=Bcc)
                                            MAILTO = MAILTO & "," & cm
                                        Else
                                            '  ExportToPDF(MAILTO, cc, Bcc, exlsub, exlmsg, exceldata, tid, eid)
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
                                        '   sendMail1(EMailto, cc1, bcc1, exlsub, MSG, en, eid)
                                        'obj.SendMail(ToMail:=EMailto, Subject:=exlsub, MailBody:=MSG, CC:=cc1, BCC:=Bcc)
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
                                    '  SendMailPdf(MAILTO, cc, Bcc, subject, MSG, Pdffile)
                                    'obj.SendMail(ToMail:=MAILTO, Subject:=subject, MailBody:=MSG, CC:=cc, Attachments:=Pdffile, BCC:=Bcc)
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
                            Else
                                'Code uncommented if you want to get cc value from specific docid 


                                'TicketCC = objDC.ExecuteQryScaller("if exists (select count(tid) from mmm_hdmail_schdule where documenttype in(select documenttype from mmm_mst_doc where tid =" & tid & ") and eid=" & eid & " having count(*)>0) begin  declare @qry nvarchar(max)='', @SalesOrderOUT nvarchar(200)=''  set @qry='select '+  (select fieldmapping from mmm_mst_fields where documenttype in (select documenttype from mmm_mst_doc where tid =" & tid & ") and eid=" & eid & " and MDfieldName='CC')+' from mmm_mst_doc where tid='+ convert(nvarchar," & tid & ") exec sp_executesql @qry,N'@SalesOrderOUT nvarchar(200) output',@SalesOrderOUT output print @SalesOrderOUT end else select '0'")
                                'If TicketCC <> "0" Then
                                '    If cc = "" Then
                                '        cc &= TicketCC
                                '    Else
                                '        cc &= "," & TicketCC
                                '    End If
                                'End If
                            End If
                            '  sendMail1(MAILTO, cc, Bcc, subject, MSG, en, eid, upCommingFrom)
                            'obj.SendMail(ToMail:=MAILTO, Subject:=subject, MailBody:=MSG, CC:=cc, BCC:=Bcc)
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
    Public Function GenerateMailPdf(ByVal filename As String, ByVal docid As Integer, ByVal dc As String, Optional ISDraft As String = "NA", Optional ByVal curdocnature As String = "SE") As String
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim da As New SqlDataAdapter("", con)
        'try Catch Block Added by Ajeet Kumar :Date::22 May 2014
        Try
            If ISDraft <> "NA" Then
                da.SelectCommand.CommandText = "select * from MMM_Print_Template where template_name='" & filename & "' and Draft='DRAFT'"
            Else
                da.SelectCommand.CommandText = "select * from MMM_Print_Template where template_name='" & filename & "' and draft='original'"
            End If
            Dim ds As New DataSet
            Dim path As String = HostingEnvironment.MapPath("~/MailAttach/")

            Dim FPath As String = HostingEnvironment.MapPath("~/MailAttach/")
            Dim Folder As New DirectoryInfo(HostingEnvironment.MapPath("~/MailAttach/"))


            da.Fill(ds, "data1")
            da.SelectCommand.CommandType = CommandType.Text
            da.SelectCommand.CommandText = "select formcaption from mmm_mst_forms where formname='" & dc & "'"
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
                    If j = 0 Then
                        strChildItem &= "<td text-align:""left"" width=""40"">"
                    ElseIf j = 1 Then
                        strChildItem &= "<td text-align:""left"" width=""90"">"
                    ElseIf j = 2 Then
                        strChildItem &= "<td text-align:""left"" width=""210"">"
                    ElseIf j = 7 Then
                        strChildItem &= "<td text-align:""left"" width=""100"">"
                    ElseIf j = 5 Then
                        strChildItem &= "<td text-align:""left"" width=""60"">"
                    Else
                        strChildItem &= "<td text-align:""left"" width=""80"">"
                    End If

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
                filename = filename & "_" & docid & "_" & Now.Millisecond
            End If
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

    Private Sub sendMail(ByVal Mto As String, ByVal MSubject As String, ByVal MBody As String)
        Try
            'Dim Email As New System.Net.Mail.MailMessage("no-reply@myndsol.com", "manish@myndsol.com", MSubject, MBody)
            Dim Email As New System.Net.Mail.MailMessage("no-reply@myndsol.com", Mto, MSubject, MBody)
            Dim mailClient As New System.Net.Mail.SmtpClient()
            Email.IsBodyHtml = True
            Dim basicAuthenticationInfo As New System.Net.NetworkCredential("no-reply@myndsol.com", "vc79aK123AJ&$kL0")
            mailClient.Host = "mail.myndsol.com"
            mailClient.UseDefaultCredentials = False
            mailClient.Credentials = basicAuthenticationInfo
            'mailClient.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.PickupDirectoryFromIis
            mailClient.Send(Email)
        Catch ex As Exception
            Exit Sub
        End Try
    End Sub
    Private Sub sendMail1(ByVal Mto As String, ByVal cc As String, ByVal bcc As String, ByVal MSubject As String, ByVal MBody As String, Optional ByVal DocType As String = "", Optional ByVal EID As Integer = 0, Optional ByVal UpCommingFrom As String = "")
        'Dim Email As New System.Net.Mail.MailMessage("no-reply@myndsol.com", "manish@myndsol.com", MSubject, MBody & Mto)

        Try
            If Left(Mto, 1) = "{" Then
                Exit Sub
            End If

            ''new for hd mail sending
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As SqlConnection = New SqlConnection(conStr)
            Dim da As SqlDataAdapter = New SqlDataAdapter("", con)
            Dim query As String = "select * from mmm_hdmail_schdule where eid=" & EID & " and documenttype='" & DocType & "' and IsSendMailFromDefaultEMailID=0 "
            If UpCommingFrom <> "" Then
                query = query & " and mdmailid='" & UpCommingFrom & "' "
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
                dtSch.Dispose()
                con.Close()
                da.Dispose()
                Try
                    mailClient.Send(Email)
                Catch ex As Exception
                    Exit Sub
                End Try
            Else
                Dim Email As New System.Net.Mail.MailMessage("no-reply@myndsol.com", Mto, MSubject, MBody)
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

            Dim Email As New System.Net.Mail.MailMessage("no-reply@myndsol.com", Mto, MSubject, MBody)
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

    Public Sub TemplateCallingMail(ByVal tid As Integer, ByVal eid As Integer, ByVal en As String, ByVal SUBEVENT As String, Optional ByVal upCommingFrom As String = "", Optional ByVal TicketCC As String = "")
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
        Dim da As SqlDataAdapter = New SqlDataAdapter("select documenttype,curstatus,curdocnature from MMM_MST_DOC where tid=" & tid, con)
        Dim DS As New DataSet
        Dim WFstatus As String = "NOT REQUIRED"
        Dim curdocnature As String = ""
        code = objDC.ExecuteQryScaller("select code from mmm_mst_entity where eid=" & eid)
        'Dim obj As New MailUtill(eid:=eid)
        Dim Filepath As String = HostingEnvironment.MapPath("~/MailAttach/")
        'try Catch Block Added by Ajeet Kumar :Date::22 May 2014

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

            da.SelectCommand.CommandText = "select T.TID,T.msgBody,T.subject,T.MAILTO,T.CC,T.BCC,T.EventName,T.qry,T.specialfieldflag,isnull(T.condition,'') as condition from MMM_MST_TEMPLATE T  WHERE T.EVENTNAME='" & en & "' AND T.SUBEVENT='" & SUBEVENT & "' AND t.statusName='" & WFstatus & "' and EID=" & eid & " and t.docnature='" & curdocnature & "' "
            da.Fill(DS, "TEMP")
            If DS.Tables("TEMP").Rows.Count > 0 Then
                MSG = HttpUtility.HtmlDecode(DS.Tables("TEMP").Rows(0).Item("msgbody").ToString())
                subject = DS.Tables("TEMP").Rows(0).Item("SUBJECT").ToString()
                MAILTO = DS.Tables("TEMP").Rows(0).Item("MAILTO").ToString()
                cc = DS.Tables("TEMP").Rows(0).Item("CC").ToString()
                Bcc = DS.Tables("TEMP").Rows(0).Item("BCC").ToString()
                MainEvent = DS.Tables("TEMP").Rows(0).Item("EVENTNAME").ToString()
                STR = DS.Tables("TEMP").Rows(0).Item("qry").ToString()
            Else
                da.SelectCommand.CommandText = "select T.TID,T.msgBody,T.subject,T.MAILTO,T.CC,T.BCC,T.EventName,T.qry,T.specialfieldflag, isnull(T.condition,'') as condition from MMM_MST_TEMPLATE T  WHERE T.EVENTNAME='" & en & "' AND T.SUBEVENT='" & SUBEVENT & "' AND t.statusName='" & WFstatus & "' AND EID=0 and t.docnature='" & curdocnature & "' "
                da.Fill(DS, "TEMP")
                If DS.Tables("TEMP").Rows.Count <> 0 Then
                    MSG = HttpUtility.HtmlDecode(DS.Tables("TEMP").Rows(0).Item("msgbody").ToString())
                    subject = DS.Tables("TEMP").Rows(0).Item("SUBJECT").ToString()
                    MAILTO = DS.Tables("TEMP").Rows(0).Item("MAILTO").ToString()
                    cc = DS.Tables("TEMP").Rows(0).Item("CC").ToString()
                    Bcc = DS.Tables("TEMP").Rows(0).Item("BCC").ToString()
                    MainEvent = DS.Tables("TEMP").Rows(0).Item("EVENTNAME").ToString()
                    STR = DS.Tables("TEMP").Rows(0).Item("qry")
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
                        'added by pallavi for multiple temps on 15 July 15

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
                            Dim Q1 As String = "DOCID=" & tid
                            Q1 = objEncryptionDescription.EncryptTripleDES(Q1, eid)
                            'EncryptTripleDES(Q1, eid)
                            Dim Q2 As String = "USERID=" & Convert.ToString(da.SelectCommand.ExecuteScalar())
                            Q2 = objEncryptionDescription.EncryptTripleDES(Q2, eid)
                            'Q2 = EncryptTripleDES(Q2, eid)
                            Dim Q3 As String = "CURSTATUS=" & WFstatus
                            'Q3 = EncryptTripleDES(Q3, eid)
                            Q3 = objEncryptionDescription.EncryptTripleDES(Q3, eid)
                            'MSG = MSG.Replace("{ApprovalThroughMail}", "<br/><br/><table cellspacing=""0"" cellpadding=""0""><tr><td align=""center"" width=""300"" height=""40"" bgcolor=""#000091"" style=""-webkit-border-radius: 5px; -moz-border-radius: 5px; border-radius: 5px; color: #ffffff; display: block;""><a href=""http://" & ECode.ToString() & ".myndsaas.com/ActionMailApproval.aspx?Q1=" & Q1 & "&Q2= " & Q2 & "&Q3=" & Q3 & """  style=""font-size:16px; font-weight: bold; font-family: Helvetica, Arial, sans-serif; text-decoration: none; line-height:40px;width:100%; display:inline-block""><span style=""color: #FFFFFF"">Approve Document</span></a></td></tr></table>")
                            MSG = MSG.Replace("{ApprovalThroughMail}", "<br/><table cellspacing=""0"" cellpadding=""0""><tr><td align=""center"" width=""300"" height=""40"" bgcolor=""#000091"" style=""-webkit-border-radius: 5px; -moz-border-radius: 5px; border-radius: 5px; color: #ffffff; display: block;""><a href=""http://" & ECode.ToString() & ".myndsaas.com/ActionMailApproval.aspx?Q1=" & Q1 & "&Q2= " & Q2 & "&Q3=" & Q3 & """  style=""font-size:16px; font-weight: bold; font-family: Helvetica, Arial, sans-serif; text-decoration: none; line-height:40px;width:100%; display:inline-block""><span style=""color: #FFFFFF"">Approve</span></a></td></tr></table>")
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

                            If con.State <> ConnectionState.Open Then
                                con.Open()
                            End If
                            RES = da.SelectCommand.ExecuteScalar()
                            con.Close()
                        End If
                        If MSG.Contains("MailAttach") Then
                            Dim da1 As SqlDataAdapter = New SqlDataAdapter("select * from MMM_Print_Template where documenttype='" & en & "' and draft='original'", con)
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
                                            MAILTO = MAILTO & "," & cm
                                        Else
                                            ' ExportToPDF(MAILTO, cc, Bcc, exlsub, exlmsg, exceldata, tid, eid)
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
                                        sendMail1(EMailto, cc1, bcc1, exlsub, MSG, en, eid)
                                        'obj.SendMail(ToMail:=EMailto, Subject:=exlsub, MailBody:=MSG, CC:=cc1, BCC:=Bcc)
                                        'sendMail(ToMail:=EMailto, subject:=exlsub, MailBody:=MSG, cc:=cc1, Bcc:=Bcc)
                                        MSG.Replace("{MailAttach}", "")
                                    End If

                                Else
                                    Pdffile = ""
                                End If
                                If MSG.Contains("MailAttach") And UCase(en) <> "STATIC SURVEY FORM" = True Then
                                    MSG.Replace("{MailAttach}", "")
                                    SendMailPdf(MAILTO, cc, Bcc, subject, MSG, Pdffile)
                                    'obj.SendMail(ToMail:=MAILTO, Subject:=subject, MailBody:=MSG, CC:=cc, Attachments:=Pdffile, BCC:=Bcc)
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
                            Else
                                'Code uncommented if you want to get cc value from specific docid 


                                'TicketCC = objDC.ExecuteQryScaller("if exists (select count(tid) from mmm_hdmail_schdule where documenttype in(select documenttype from mmm_mst_doc where tid =" & tid & ") and eid=" & eid & " having count(*)>0) begin  declare @qry nvarchar(max)='', @SalesOrderOUT nvarchar(200)=''  set @qry='select '+  (select fieldmapping from mmm_mst_fields where documenttype in (select documenttype from mmm_mst_doc where tid =" & tid & ") and eid=" & eid & " and MDfieldName='CC')+' from mmm_mst_doc where tid='+ convert(nvarchar," & tid & ") exec sp_executesql @qry,N'@SalesOrderOUT nvarchar(200) output',@SalesOrderOUT output print @SalesOrderOUT end else select '0'")
                                'If TicketCC <> "0" Then
                                '    If cc = "" Then
                                '        cc &= TicketCC
                                '    Else
                                '        cc &= "," & TicketCC
                                '    End If
                                'End If
                            End If
                            sendMail1(MAILTO, cc, Bcc, subject, MSG, en, eid, upCommingFrom)
                            'obj.SendMail(ToMail:=MAILTO, Subject:=subject, MailBody:=MSG, CC:=cc, BCC:=Bcc)
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

End Class
