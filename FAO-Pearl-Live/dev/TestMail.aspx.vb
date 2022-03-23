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


Partial Class TestMail
    Inherits System.Web.UI.Page
    Public Sub TemplateCalling(ByVal tid As Integer, ByVal eid As Integer, ByVal en As String, ByVal SUBEVENT As String)
        tid = 1460929
        eid = 118
        SUBEVENT = "APPROVE"
        en = "TTSL PV Sheet"
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
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
        Dim obj As New MailUtill(eid:=eid)
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

            da.SelectCommand.CommandText = "select T.TID,T.msgBody,T.subject,T.MAILTO,T.CC,T.BCC,T.EventName,T.qry,T.specialfieldflag from MMM_MST_TEMPLATE T  WHERE T.EVENTNAME='" & en & "' AND T.SUBEVENT='" & SUBEVENT & "' AND t.statusName='" & WFstatus & "' and EID=" & eid & " and t.docnature='" & curdocnature & "' "
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
                da.SelectCommand.CommandText = "select T.TID,T.msgBody,T.subject,T.MAILTO,T.CC,T.BCC,T.EventName,T.qry,T.specialfieldflag from MMM_MST_TEMPLATE T  WHERE T.EVENTNAME='" & en & "' AND T.SUBEVENT='" & SUBEVENT & "' AND t.statusName='" & WFstatus & "' AND EID=0 and t.docnature='" & curdocnature & "' "
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
                STR &= " WHERE TID=" & tid & ""
                da.SelectCommand.CommandText = STR
                da.Fill(DS, "qry")
                For Each dc As DataColumn In DS.Tables("qry").Columns
                    fn = "{" & dc.ColumnName.ToString() & "}"
                    MSG = MSG.Replace(fn, DS.Tables("qry").Rows(0).Item(dc.ColumnName).ToString())
                    subject = subject.Replace(fn, DS.Tables("qry").Rows(0).Item(dc.ColumnName).ToString())
                    MAILTO = MAILTO.Replace(fn, DS.Tables("qry").Rows(0).Item(dc.ColumnName).ToString())
                    cc = cc.Replace(fn, DS.Tables("qry").Rows(0).Item(dc.ColumnName).ToString())
                Next
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
                End If
                If MSG.Contains("MailAttach") Then
                    Dim da1 As SqlDataAdapter = New SqlDataAdapter("select * from MMM_Print_Template where documenttype='" & en & "' and draft='original'", con)
                    Dim ds1 As New DataSet
                    da1.Fill(ds1, "dataset")
                    da.SelectCommand.CommandType = CommandType.Text
                    da.SelectCommand.CommandText = "select formcaption from mmm_mst_forms where formname='" & en & "' and eid=" & eid
                    da.Fill(ds1, "dataset1")
                    For k As Integer = 0 To ds1.Tables("dataset").Rows.Count - 1
                        If ds1.Tables("dataset").Rows(k).Item("SendType").ToString() = "EXCEL" Then
                        Else
                            Pdffile = GenerateMailPdf(ds1.Tables("dataset1").Rows(0).Item(0).ToString() & "_" & eid & "_" & "print" & ds1.Tables("dataset").Rows(k).Item("tid").ToString(), tid, en, "NA", curdocnature)
                        End If


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
                                    Dim cm As String = DS.Tables("qry").Rows(0).Item("OWNER EMAIL").ToString
                                    MAILTO = Replace(MAILTO, cm, "")
                                    MAILTO = MAILTO.Trim().Substring(0, MAILTO.Length - 1)

                                    'sendMailExl(MAILTO, cc, Bcc, exlsub, exlmsg, CreateCSVR(exceldata, exlsub))

                                    obj.SendMail(ToMail:=MAILTO, Subject:=exlsub, MailBody:=exlmsg, CC:=cc, Attachments:=Filepath + CreateCSVR(exceldata, exlsub), BCC:=Bcc)

                                    MAILTO = MAILTO & "," & cm
                                Else
                                    ' ExportToPDF("ravi.sharma@myndsol.com", cc, Bcc, exlsub, exlmsg, exceldata, tid, eid)
                                    obj.SendMail(ToMail:=MAILTO, Subject:=exlsub, MailBody:=exlmsg, CC:=cc, Attachments:=Filepath + CreateCSVR(exceldata, exlsub), BCC:=Bcc)
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
                                ' sendMail1(EMailto, cc1, bcc1, exlsub, MSG)
                                obj.SendMail(ToMail:=EMailto, Subject:=exlsub, MailBody:=MSG, CC:=cc1, BCC:=Bcc)
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
                        If MSG.Contains("MailAttach") And UCase(en) <> "STATIC SURVEY FORM" And UCase(en) <> "COMPETITION GUARD INFORMATION" = True Then
                            MSG.Replace("{MailAttach}", "")
                            ' SendMailPdf(MAILTO, cc, Bcc, subject, MSG, Pdffile)
                            obj.SendMail(ToMail:=MAILTO, Subject:=subject, MailBody:=MSG, CC:=cc, Attachments:=Pdffile, BCC:=Bcc)
                        End If
                    Next
                Else
                    '' mail without attachment  12_june
                    'sendMail1(MAILTO, cc, Bcc, subject, MSG)
                    obj.SendMail(ToMail:=MAILTO, Subject:=subject, MailBody:=MSG, CC:=cc, BCC:=Bcc)
                End If

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
                                'body = body.Replace("{SignImg_" & fldmap & "}", "docs/" & EID & "/" & Sig_fldVal)
                                body = body.Replace("{SignImg_" & fldmap & "}", "docs/" & Sig_fldVal)
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

    Protected Sub Run_Click(sender As Object, e As System.EventArgs) Handles Run.Click
        TemplateCalling("1460929", "118", "APPROVE", "TTSL PV Sheet")
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
End Class
