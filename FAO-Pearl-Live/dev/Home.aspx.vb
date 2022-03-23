Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports iTextSharp.text.pdf
Imports System.Security.Policy
Imports System.Net.Security
Imports System.Net.HttpWebRequest
Imports System.Net.HttpWebResponse
Imports System.Security.Cryptography.X509Certificates
Imports System.Web.Services
Imports Newtonsoft
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Converters
Imports System.Web.Script.Serialization
Partial Class Home
    Inherits System.Web.UI.Page
    Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            bindDocumentType()
            ShowCustomizedAlertsPayU()  ' this is for showing Alert messages on login entity wise role wise configurable and content is dynamic, with setting on/off by pareek
            Session("EDIT") = 0
            Session("Pending") = Nothing
            If IsNothing(Session("PassExp")).ToString() Then
                PassExpMsgAlert()
            End If

            Session("PassExp") = "1"
        End If
    End Sub


    Public Sub PassExpMsgAlert()

        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim Passexp As Integer
        Dim PassExpMsgDays As Integer
        Dim Modifydate As String = ""
        Dim days As Integer
        Dim daydiff As Integer
        Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
        Try
            'With(nolock) added by Himank on 29th sep 2015
            oda.SelectCommand.CommandText = "select * from mmm_mst_entity   WITH(NOLOCK)  where eid='" & Session("EID").ToString & "'"
            oda.SelectCommand.CommandType = CommandType.Text
            Dim ds As New DataSet()
            oda.Fill(ds, "data")
            If ds.Tables("data").Rows.Count > 0 Then
                Passexp = ds.Tables("data").Rows(0).Item("passExpDays").ToString
                PassExpMsgDays = ds.Tables("data").Rows(0).Item("PassExpMsgDays").ToString
                daydiff = CInt(Passexp - PassExpMsgDays)
            End If
            'With(nolock) added by Himank on 29th sep 2015
            oda.SelectCommand.CommandText = "select * from mmm_mst_user  WITH(NOLOCK)   where  EID=" & Session("EID") & " and uid=" & Session("UID") & ""
            oda.SelectCommand.CommandType = CommandType.Text
            oda.Fill(ds, "data1")
            If ds.Tables("data1").Rows.Count > 0 Then
                Modifydate = ds.Tables("data1").Rows(0).Item("modifydate").ToString
                days = DateDiff(DateInterval.Day, CDate(Modifydate), Date.Now)
                If days >= daydiff Then
                    lblPassexpmsg.Text = " Dear User your password will expire in " & Passexp - days & " days. "
                    modalpopuppassexp.Show()
                Else
                    Exit Sub
                End If
            End If


        Catch ex As Exception
        Finally
            con.Close()
            con.Dispose()
            oda.Dispose()
        End Try
    End Sub

    Protected Sub btnYes_Click(sender As Object, e As System.EventArgs) Handles btnYes.Click
        Response.Redirect("Profile.aspx")
    End Sub


    Public Sub ShowCustomizedAlertsPayU()
        'prev - commented on 02-jul - ' Dim eidStr As String = "32,37,41,42,43,44,45,48,53,56,57,58,59,60,61,63,64,65,66,67,68,69,70,71,72,73,75,77,79,81,82,85,86,87"
        '' below is latest list only live accounts in sequel. by SP
        'If eidStr = "137" Then
        '    Dim ShowStr As String = ""
        '    Dim P1 As String = "" : Dim P2 As String = "" : Dim P3 As String = "" : Dim T1 As String = "" : Dim C1 As String = "" : Dim C2 As String = ""
        '    Dim Close As String = ""
        '    ShowStr = "<html><body> <font face=""arial, helvetica, sans-serif"" size=""6""><p style=""line-height: 17.6px;""> <b>Dear Vendor</b> </p> <p> We would like to inform you that Citrus Payment Solutions Private Limited has merged into PayU Payments Private Limited (<b><u>'PayU'</b></u>) pursuant to a scheme of the arrangement between PayU and Citrus which was duly approved by the National Company Law Tribunal, Mumbai via order dated 4th April 2018. Pursuant to the scheme of arrangement, from the effective date, Citrus will cease to exist and the entire business will be transferred to PayU.</p> <p> In furtherance to the foresaid order, we wish to inform you that all the existing Agreement/P.O. would be assigned to PayU. Upon assignment, you are requested to provide your services to PayU on the same terms and conditions as stated under the Agreement/P.O. and raise all your future invoices in the name of <b><u>'PayU Payments Private Limited' with effect from 30th April,2018</b></u> considering below address and GST no. (Basis the place of supply).</p> </font></br><table border=""yes"" cellspacing=""10px"" cellpadding=""14px""><tbody><tr> <th> <font face=""arial, helvetica, sans-serif"" size=""2""> Location</font></th><th><font face=""arial, helvetica, sans-serif"" size=""2"">Address</font></th><th><font face=""arial, helvetica, sans-serif"" size=""2"">GST No.</font></th></tr><tr> <td> <font face=""arial, helvetica, sans-serif"" size=""2""> <b>KHAR MUMBAI</b> </font></td><td><font face=""arial, helvetica, sans-serif"" size=""2"">32, Viraj Building, 3rd Floor, Opp Bank Of Maharastra, Above HDFC Bank, S.V. Road, Khar(W) Mumbai–400052</font></td><td><font face=""arial, helvetica, sans-serif"" size=""2"">27AAJCS9091D1Z0</font></td></tr><tr> <td> <font face=""arial, helvetica, sans-serif"" size=""2""> <b>ANDHERI</b> </font></td><td><font face=""arial, helvetica, sans-serif"" size=""2"">P&G Plaza, 2nd floor Cardinal Gracious Road, Chakala, Andheri East, Mumbai, Maharashtra 400099.</font></td><td><font face=""arial, helvetica, sans-serif"" size=""2"">27AAJCS9091D1Z0</font></td></tr><tr> <td> <font face=""arial, helvetica, sans-serif"" size=""2""> <b>GURUGRAM</b> </font></td><td><font face=""arial, helvetica, sans-serif"" size=""2"">9th Floor, Bestech Business Tower, Sohna Road, Sector 48, Gurugram 122002- Haryana , India</font></td><td><font face=""arial, helvetica, sans-serif"" size=""2"">06AAJCS9091D1Z4</font></td></tr><tr> <td> <font face=""arial, helvetica, sans-serif"" size=""2""> <b>BANGALORE</b> </font></td><td><font face=""arial, helvetica, sans-serif"" size=""2"">5th floor - No 124 , Surya chambers HAL old airport road, Murugeshpalya Bangalore - 560 017</font></td><td><font face=""arial, helvetica, sans-serif"" size=""2"">29AAJCS9091D1ZW</font></td></tr></tbody></table></br><font face=""arial, helvetica, sans-serif"" size=""2""><p>You are also requested to submit all<u>invoices pertaining to previous period</u>, if not submitted already in the name of <b><u>'Citrus Payment Solutions Pvt Ltd' by 30th April 2018.</b></u></p> </br><p style=""line-height: 6.6px;""> In case you need any further clarification on the same, please drop a mail to : <a href=""mailto:payuprocurement@payu.in"">payuprocurement@payu.in</a></p></br><p style=""line-height: 23.6px;"">Thanks and Regards</p><b><p style=""line-height: 15.6px;"">Procurement Team</p></b></font></body></html>"
        '    'ShowStr = "<html><BODY><h3> Dear Vendor, </h3>"
        '    'P1 = "We would like to inform you that Citrus Payment Solutions Private Limited has merged into PayU Payments Private Limited ('PayU') pursuant to a scheme of the arrangement between PayU and Citrus which was duly approved by the National Company Law Tribunal, Mumbai via order dated 4th April 2018. Pursuant to the scheme of arrangement, from the effective date, Citrus will cease to exist and the entire business will be transferred to PayU."
        '    'P2 = "In furtherance to the foresaid order, we wish to inform you that all the existing Agreement/P.O. would be assigned to PayU. Upon assignment, you are requested to provide your services to PayU on the same terms and conditions as stated under the Agreement/P.O. and raise all your future invoices in the name of 'PayU Payments Private Limited' with effect from 30th April, 2018 considering below address and GST no. (Basis the place of supply)."
        '    'C1 = "You are also requested to submit all invoices pertaining to previous period, if not submitted already in the name of 'Citrus Payment Solutions Pvt Ltd' by 30th April 2018."
        '    'C2 = "In case you need any further clarification on the same, please drop a mail to payuprocurement@payu.in"
        '    'Close = "Thanks and Regards </br> Procurement Team"

        '    lblAlertMes.Text = ShowStr
        '    updAlert.Update()
        '    MP_Alert.Show()
        'Else
        '    MP_Alert.Hide()
        'End If

        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
        Try
            Dim eidStr As String = Session("EID")
            Dim uid As String = Session("UID")
            oda.SelectCommand.CommandText = "Select isAlertActive , isnull(alertmsgonlogin,'') [alertmsgonlogin], isnull(AlertTncMsgonLogin,'') AlertTncMsgonLogin from mmm_mst_entity where isAlertActive = 1 and eid=" & eidStr & "Select isnull(PaytmTncExcept,'') PaytmTncExcept from mmm_mst_user where eid=" & eidStr & " and uid=" & uid
            Dim ds As New DataSet()
            'Dim dt As New DataTable()
            'oda.Fill(dt)
            oda.Fill(ds)
            If ds.Tables(0).Rows.Count = 1 Then
                If Len(ds.Tables(0).Rows(0).Item("alertmsgonlogin").ToString) > 10 Then
                    Dim ShowStr As String = ""
                    ShowStr = ds.Tables(0).Rows(0).Item("alertmsgonlogin").ToString()
                    lblAlertMes.Text = ShowStr
                    updAlert.Update()
                    MP_Alert.Show()

                End If
                If Len(ds.Tables(0).Rows(0).Item("AlertTncMsgonLogin").ToString) > 10 And Convert.ToBoolean(ds.Tables(1).Rows(0).Item("PaytmTncExcept")) = False Then
                    Dim ShowStr As String = ""
                    ShowStr = ds.Tables(0).Rows(0).Item("AlertTncMsgonLogin").ToString()
                    lblTCAlertMes.Text = ShowStr
                    updTCAlert.Update()
                    TC_Alert.Show()

                End If
            Else
                ' updAlert.Update()
                lblWarningMsg.Text = ""
                TC_Alert.Hide()
                MP_Alert.Hide()
            End If
        Finally
            con.Close()
            con.Dispose()
            oda.Dispose()
        End Try
    End Sub

    Public Sub HideAlert()
        MP_Alert.Hide()
    End Sub

    Public Sub TCHideAlert()
        Dim eidStr As String = Session("EID")
        Dim uid As String = Session("UID")
        Dim objDC As New DataClass()
        If chkTermAndCondition.Checked Then
            objDC.ExecuteNonQuery(" update mmm_mst_user set PaytmTncExcept=1 where  eid=" & eidStr & " and uid=" & uid)
            lblWarningMsg.Text = ""
            TC_Alert.Hide()
        Else
            lblWarningMsg.Text = "Please check above box to proceed."
        End If

    End Sub
    Protected Sub chkTCChanged(sender As Object, e As System.EventArgs)
        Dim id As String = ""
        If chkTermAndCondition.Checked = True Then
            lblWarningMsg.Text = ""
        End If
    End Sub

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
    Private Sub bindDocumentType()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
        'oda.SelectCommand.CommandText = "Select uid, rolename, documenttype from MMM_ref_role_user where eid=" & Session("EID") & " And uid=" & Session("UID") & " And rolename='" & Session("ROLES") & "' "
        Try
            'oda.SelectCommand.CommandText = "select MID,MENUNAME,PAGELINK,pmenu,image,roles,dord from mmm_mst_menu where ROLES <> '0' and roles like '" & "%{" & Session("USERROLE") & "%" & "' AND EID=" & Session("EID") & " AND Mtype='DYNAMIC' order by dord   "
            ' bkup 23 Jan - oda.SelectCommand.CommandText = "Select documenttype  from MMM_MST_DOC M with (nolock)  LEFT OUTER JOIN MMM_DOC_DTL D with (nolock) on D.tid=M.lasttid  LEFT OUTER JOIN MMM_MST_USER U with (nolock) ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP with (nolock) ON AP.uid=D.UserID where D.userid = " & Session("UID") & " and documenttype<>'Ticket' union Select  distinct documenttype from MMM_MST_DOC M with (nolock)  LEFT OUTER JOIN MMM_DOC_DTL D with (nolock) on D.docid =M.tid  LEFT OUTER JOIN MMM_MST_USER U with (nolock) ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP with (nolock) ON AP.uid=D.UserID where D.UserID = " & Session("UID") & " and aprstatus is not null and m.curstatus <> 'ARCHIVE' and documenttype<>'Ticket'  union  Select distinct documenttype from MMM_MST_DOC M with (nolock)  LEFT OUTER JOIN MMM_DOC_DTL D with (nolock) on D.tid=M.lasttid  LEFT OUTER JOIN MMM_MST_USER U with (nolock) ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP with (nolock) ON AP.uid=D.UserID where M.oUid = " & Session("UID") & " and curstatus<> 'ARCHIVE' and documenttype<>'Ticket'  "
            oda.SelectCommand.CommandText = "Select documenttype  from MMM_MST_DOC M with (nolock)  LEFT OUTER JOIN MMM_DOC_DTL D with (nolock) on D.tid=M.lasttid  LEFT OUTER JOIN MMM_MST_USER U with (nolock) ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP with (nolock) ON AP.uid=D.UserID where D.userid = " & Session("UID") & " and documenttype<>'Ticket' union Select  distinct documenttype from MMM_MST_DOC M with (nolock)  LEFT OUTER JOIN MMM_DOC_DTL D with (nolock) on D.docid =M.tid  LEFT OUTER JOIN MMM_MST_USER U with (nolock) ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP with (nolock) ON AP.uid=D.UserID where D.UserID = " & Session("UID") & " and aprstatus is not null and m.curstatus <> 'ARCHIVE' and documenttype<>'Ticket'  union  Select distinct documenttype from MMM_MST_DOC M with (nolock)  LEFT OUTER JOIN MMM_DOC_DTL D with (nolock) on D.tid=M.lasttid  LEFT OUTER JOIN MMM_MST_USER U with (nolock) ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP with (nolock) ON AP.uid=D.UserID where M.oUid = " & Session("UID") & " and curstatus<> 'ARCHIVE' and documenttype<>'Ticket' union Select  distinct documenttype from mmm_mst_doc_draft with (nolock)  where ouid=" & Session("UID")
            Dim ds As New DataSet()
            oda.Fill(ds, "data")

            '' by sunil for selecting item if there is only single entry in ddn list - 22_jan_15
            ddldocType.Items.Clear()
            'ddldocType.Items.Add("SELECT ALL")

            If ds.Tables("data").Rows.Count > 0 Then
                Dim k As Integer = 0
                For j As Integer = 0 To ds.Tables("data").Rows.Count - 1
                    If ds.Tables("data").Rows(j).Item(0).ToString() <> "" Then
                        'ddldocType.Items.Add(ds.Tables("data").Rows(j).Item(0).ToString)
                        ddldocType.Items.Add(RtnFormCaption(ds.Tables("data").Rows(j).Item(0).ToString, Session("eid")))
                        ddldocType.Items(k).Value = ds.Tables("data").Rows(j).Item(0).ToString
                        k += 1
                    End If
                Next
            End If
            '' by sunil for selecting item if there is only single entry in ddn list - 22_jan_15
            'Check if Account has Ticket Configuration
            CheckTicketStatus()
            'Check if Account has Ticket Configuration

        Finally
            con.Close()
            con.Dispose()
            oda.Dispose()
        End Try

    End Sub

    Public Function RtnFormCaption(ByVal Dtype As String, ByVal eid As Integer) As String
        Dim objDC As New DataClass()
        Dim FormCap As String = Convert.ToString(objDC.ExecuteQryScaller("select FormCaption from mmm_mst_forms where eid=" & eid & " and formname='" & Dtype & "'"))
        Return FormCap
    End Function

    Protected Sub CheckTicketStatus()
        Dim objDC As New DataClass()
        Dim txt As String = Convert.ToString(objDC.ExecuteQryScaller("if exists(select * from mmm_hdmail_schdule where eid=" & Session("EID") & " and isactive=1) select ' HelpDesk '"))
        If txt <> "" Then
            btnTicket.Visible = True
            btnTicket.Value = txt.ToString()
        Else
            btnTicket.Visible = False
        End If
    End Sub
#Region "Grids"
    Private Shared Function Sortgrid(sorting As List(Of SortDescription)) As String
        Dim sortingStr As String = ""
        If sorting IsNot Nothing Then
            If sorting.Count <> 0 Then
                For i As Integer = 0 To sorting.Count - 1

                    If sorting(i).field = "fdate" Or sorting(i).field = "RECEIVEDON" Then
                        sortingStr += ", d." + sorting(i).field + " " + sorting(i).dir
                    ElseIf sorting(i).field = "P_Days" Or sorting(i).field = "PENDINGDAYS" Then
                        sortingStr += ", datediff(day,fdate,getdate()) " + sorting(i).dir
                    Else
                        sortingStr += ", M." + sorting(i).field + " " + sorting(i).dir
                    End If

                Next
            End If
        End If
        Return sortingStr
    End Function
    Private Shared Function filtergrid(filter As FilterContainer) As String
        Dim filters As String = ""
        Dim logic As String
        Dim condition As String = ""
        Dim c As Integer = 1
        If filter IsNot Nothing Then
            For i As Integer = 0 To filter.filters.Count - 1
                logic = filter.logic
                If filter.filters(i).[operator] = "eq" Then
                    If (filter.filters(i).field = "fdate" Or filter.filters(i).field = "fld43" Or filter.filters(i).field = "RECEIVEDON") Then
                        Dim [date] As DateTime = DateTime.Parse(filter.filters(i).value)
                        Dim dateInString As [String] = [date].ToString("yyyy-MM-dd")
                        condition = " = CAST('" + dateInString + "' AS DATE) "
                    Else
                        condition = " = '" + filter.filters(i).value + "' "
                    End If

                End If
                If filter.filters(i).[operator] = "neq" Then
                    If filter.filters(i).field = "fdate" Or filter.filters(i).field = "fld43" Or filter.filters(i).field = "RECEIVEDON" Then
                        Dim [date] As DateTime = DateTime.Parse(filter.filters(i).value)
                        Dim dateInString As [String] = [date].ToString("yyyy-MM-dd")
                        condition = " != CAST('" + dateInString + "' AS DATE) "
                    Else
                        condition = " != '" + filter.filters(i).value + "' "
                    End If
                    'condition = " != '" + filter.filters(i).value + "' "
                End If
                If filter.filters(i).[operator] = "startswith" Then
                    If filter.filters(i).field = "fdate" Or filter.filters(i).field = "fld43" Or filter.filters(i).field = "RECEIVEDON" Then
                        Dim [date] As DateTime = DateTime.Parse(filter.filters(i).value)
                        Dim dateInString As [String] = [date].ToString("yyyy-MM-dd")
                        condition = " Like CAST('" + dateInString + "' AS DATE) %"
                    Else
                        condition = " Like '" + filter.filters(i).value + "%' "
                    End If

                    condition = " Like '" + filter.filters(i).value + "%' "
                End If
                If filter.filters(i).[operator] = "contains" Then
                    condition = " Like '%" + filter.filters(i).value + "%' "
                End If
                If filter.filters(i).[operator] = "doesnotcontains" Then
                    condition = " Not Like '%" + filter.filters(i).value + "%' "
                End If
                If filter.filters(i).[operator] = "endswith" Then
                    condition = " Like '%" + filter.filters(i).value + "' "
                End If
                If filter.filters(i).[operator] = "gte" Then
                    condition = " >= '" + filter.filters(i).value + "' "
                End If
                If filter.filters(i).[operator] = "gt" Then
                    condition = " > '" + filter.filters(i).value + "' "
                End If
                If filter.filters(i).[operator] = "lte" Then
                    condition = " <= '" + filter.filters(i).value + "' "
                End If
                If filter.filters(i).[operator] = "lt" Then
                    condition = "< '" + filter.filters(i).value + "' "
                End If

                If filter.filters(i).field = "P_Days" Or filter.filters(i).field = "PENDINGDAYS" Then
                    filters += "datediff(day,fdate,getdate()) " & condition
                ElseIf filter.filters(i).field = "fdate" Or filter.filters(i).field = "RECEIVEDON" Then
                    filters += "CAST(d.fdate AS DATE) " & condition
                ElseIf filter.filters(i).field = "fld43" Then
                    filters += "CONVERT(date,M.fld43,3) " & condition

                Else
                    filters += "M." & filter.filters(i).field + condition
                End If

                If filter.filters.Count > c Then
                    filters += logic
                    filters += " "
                End If
                c += 1
            Next
        End If
        Return filters
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function GetColumn(documentType As String, type As String) As kGridHome
        Dim ret As New kGridHome()
        Dim dsFields As New DataSet()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim strQuery1 As String = ""
        Try
            strQuery1 = "select FieldName,FieldMapping,Type,datatype,ColWidth from mmm_mst_needtoact_config where eid=" & HttpContext.Current.Session("EID") & " and DocType='" & documentType & "' order by DisplayOrder"
            dsFields = DataLib.ExecuteDataSet(conStr, CommandType.Text, strQuery1)
            If (dsFields.Tables(0).Rows().Count > 0) Then
                ret.Column = CreateColCollection(dsFields.Tables(0))

            Else
                strQuery1 = "select * from MMM_MST_FIELDS where eid=" & HttpContext.Current.Session("EID").ToString() & " and Documenttype='" + documentType + "' and showondashboard=1"
                dsFields = DataLib.ExecuteDataSet(conStr, CommandType.Text, strQuery1)
                ret.Column = CreateStaticColCollection(dsFields.Tables(0))
            End If
        Catch ex As Exception

        End Try
        Return ret
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function GetNeedToAct(documentType As String, page As Integer, pageSize As Integer, skip As Integer, take As Integer, sorting As List(Of SortDescription), filter As FilterContainer) As kGridHome
        Dim dataobj As New DataLib()
        Dim ret As New kGridHome()
        Dim jsonData As String = ""
        Dim dsFields As New DataSet()
        Dim QryFields As String = ""
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim strQuery As String = ""
        Dim strQueryTotalCount As String = ""
        Dim dsData As New DataSet()
        Dim dtCount As New DataTable()
        Try
            'paging logic==================================
            Dim from1 As Integer = skip + 1 '(page - 1) * pageSize + 1;
            Dim to1 As Integer = take * page 'page * pageSize;
            Dim sortingStr As String = ""

            '===============================================
            'Sorting logic====================================
            If sorting IsNot Nothing Then
                sortingStr = Sortgrid(sorting)
                'If sorting.Count <> 0 Then
                '    For i As Integer = 0 To sorting.Count - 1
                '        sortingStr += ", M." + sorting(i).field + " " + sorting(i).dir
                '    Next
                'End If
            End If
            '==================================================
            'filtering logic====================================
            Dim filters As String = ""
            If filter IsNot Nothing Then
                filters = filtergrid(filter)
            End If
            '==================================================

            sortingStr = sortingStr.TrimStart(","c)

            Dim SBQry As New StringBuilder()
            Dim StaticColumns As String = ""
            If sortingStr <> "" Then
                SBQry.Append("Select ROW_NUMBER() OVER (ORDER BY " & sortingStr & ") AS RowNumber,CAST(M.tid as varchar) as [DocDetID], CAST(M.tid as varchar) as [SYSTEMID], ")

                StaticColumns = "Select ROW_NUMBER() OVER (ORDER BY " & sortingStr & ") AS RowNumber, M.TID, CAST(M.tid as varchar) as [DocDetID], M.DocumentType,M.curstatus,U.Username,CONVERT(date,d.fdate,3)[RECEIVEDON],datediff(day,fdate,getdate())[PENDINGDAYS],"
            Else
                SBQry.Append("Select ROW_NUMBER() OVER (ORDER BY M.tid desc) AS RowNumber, CAST(M.tid as varchar) as [DocDetID], CAST(M.tid as varchar) as [SYSTEMID], ")

                StaticColumns = "Select ROW_NUMBER() OVER (ORDER BY M.tid desc) AS RowNumber, M.TID, CAST(M.tid as varchar) as [DocDetID], M.DocumentType,M.curstatus,U.Username,CONVERT(date,d.fdate,3)[RECEIVEDON],datediff(day,fdate,getdate())[PENDINGDAYS],"
            End If
            'SBQry.Append("Select Top CAST(M.TID as varchar) TID,CAST(M.tid as varchar) as [SYSTEMID], ")
            'query get total count--------------------------
            strQueryTotalCount = "SELECT COUNT(*) Total from MMM_MST_DOC M with (nolock) LEFT OUTER JOIN MMM_DOC_DTL D with (nolock) on D.tid=M.lasttid LEFT OUTER JOIN MMM_MST_USER U with (nolock) ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP with (nolock) ON AP.uid=D.UserID where M.documenttype='" & documentType & "' and m.curstatus<>'ARCHIVE' AND D.userid = " & Val(HttpContext.Current.Session("UID").ToString()) & ""
            If filters <> "" Then
                strQueryTotalCount = strQueryTotalCount + " AND " & filters
            Else
                'strQueryTotalCount = strQuery
            End If
            '---------------------------------
            strQuery = "select FieldName,FieldMapping,Type,datatype from mmm_mst_needtoact_config where eid=" & HttpContext.Current.Session("EID") & " and DocType='" & documentType & "' order by DisplayOrder"
            dsFields = DataLib.ExecuteDataSet(conStr, CommandType.Text, strQuery)
            If (dsFields.Tables(0).Rows().Count > 0) Then
                For i As Integer = 0 To dsFields.Tables(0).Rows.Count - 1
                    If (dsFields.Tables(0).Rows(i).Item("Type").ToString.Trim.ToUpper = "STATIC") Then
                        If (dsFields.Tables(0).Rows(i).Item("FieldMapping").ToString = "datediff(day,fdate,getdate())") Then
                            SBQry.Append(dsFields.Tables(0).Rows(i).Item("FieldMapping") & " As P_Days").Append(",")
                        Else
                            If (dsFields.Tables(0).Rows(i).Item("FieldMapping").ToString = "d.fdate") Then
                                SBQry.Append("CONVERT(date,d.fdate,3) as fdate").Append(",")
                            ElseIf (dsFields.Tables(0).Rows(i).Item("FieldName").ToString = "Creation Date") Then
                                SBQry.Append("CONVERT(date,M.adate,3) as [Column2] ").Append(",")
                            Else
                                SBQry.Append(dsFields.Tables(0).Rows(i).Item("FieldMapping")).Append(",")
                            End If
                        End If

                    Else
                        If (dsFields.Tables(0).Rows(i).Item("FieldName").ToString = "Expiry Date") Then
                            SBQry.Append("CONVERT(date,M.fld43,3) as fld43").Append(",")
                        Else
                            If dsFields.Tables(0).Rows(i).Item("FieldMapping").ToString().Contains("dms.udf_split") Then
                                SBQry.Append(dsFields.Tables(0).Rows(i).Item("FieldMapping")).Append(",")
                            Else
                                SBQry.Append("M." & dsFields.Tables(0).Rows(i).Item("FieldMapping")).Append(",")
                            End If
                        End If
                    End If

                Next
                strQuery = Left(SBQry.ToString(), Len(SBQry.ToString()) - 1) & " from MMM_MST_DOC M with (nolock) LEFT OUTER JOIN MMM_DOC_DTL D with (nolock) on D.tid=M.lasttid LEFT OUTER JOIN MMM_MST_USER U with (nolock) ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP with (nolock) ON AP.uid=D.UserID where M.documenttype='" & documentType & "' and m.curstatus<>'ARCHIVE' AND D.userid = " & Val(HttpContext.Current.Session("UID").ToString()) & ""

                strQuery = "WITH Data AS (" & strQuery
                If filters <> "" Then
                    strQuery = strQuery + " AND " & filters
                End If

                strQuery = strQuery + ") SELECT * FROM Data WHERE RowNumber BETWEEN " & from1 & "AND " & to1

                'IF @SQLSortString IS NOT NULL


                dsData = DataLib.ExecuteDataSet(conStr, CommandType.Text, strQuery)
                'ret.Column = CreateColCollection(dsFields.Tables(0))

            Else
                strQuery = "select * from MMM_MST_FIELDS where eid=" & HttpContext.Current.Session("EID").ToString() & " and Documenttype='" + documentType + "' and showondashboard=1"
                dsFields = DataLib.ExecuteDataSet(conStr, CommandType.Text, strQuery)
                For f As Integer = 0 To dsFields.Tables(0).Rows.Count - 1
                    StaticColumns = StaticColumns & "M." & dsFields.Tables(0).Rows(f).Item("FieldMapping") & ","
                Next

                Dim qrycolumn As String = StaticColumns
                qrycolumn = qrycolumn.Replace("M.fld10", "CONVERT(date,M.fld10,3) AS fld10")
                qrycolumn = Left(qrycolumn, Len(qrycolumn) - 1) & " from MMM_MST_DOC M with (nolock) LEFT OUTER JOIN MMM_DOC_DTL D with (nolock) on D.tid=M.lasttid LEFT OUTER JOIN MMM_MST_USER U with (nolock) ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP with (nolock) ON AP.uid=D.UserID where M.documenttype='" & documentType & "' and m.curstatus<>'ARCHIVE' AND D.userid = " & Val(HttpContext.Current.Session("UID").ToString()) & " "

                qrycolumn = "WITH Data AS (" & qrycolumn
                If filters <> "" Then
                    qrycolumn = qrycolumn + " AND " & filters
                End If

                qrycolumn = qrycolumn + ") SELECT * FROM Data WHERE RowNumber BETWEEN " & from1 & "AND " & to1
                dsData = DataLib.ExecuteDataSet(conStr, CommandType.Text, qrycolumn)
                'dsFields = DataLib.ExecuteDataSet(conStr, CommandType.Text, QryFields)
                If dsData.Tables(0).Rows.Count > 0 Then
                    'ret.Column = CreateStaticColCollection(dsFields.Tables(0))
                    'jsonData = DataLib.JsonTableSerializer(dsData.Tables(0))
                End If
            End If

            'dsFields = DataLib.ExecuteDataSet(dataobj.conObj, CommandType.Text, QryFields)
            If dsData.Tables(0).Rows.Count > 0 Then

                'jsonData = JsonConvert.SerializeObject(dsData.Tables(0), Newtonsoft.Json.Formatting.None, serializerSettings)
                jsonData = JsonConvert.SerializeObject(dsData.Tables(0)) 'DataLib.JsonTableSerializer(dsData.Tables(0))
            Else
                jsonData = JsonConvert.SerializeObject(dsData.Tables(0))
            End If
            ret.total = DataLib.ExecuteDataSet(conStr, CommandType.Text, strQueryTotalCount).Tables(0).Rows(0)("Total")
            ret.Data = jsonData
            Return ret
            'Json(ret, JsonRequestBehavior.AllowGet)
            'Return dsMaster
        Catch ex As Exception
            Throw
        Finally
            'oda.Dispose()

        End Try
    End Function

    'Print Draft Functionality
    <System.Web.Services.WebMethod()>
    Public Shared Function DraftPrint(docid As String) As String
        Dim result As String = ""
        Dim objDMSUtil As New DMSUtil
        Dim str As String = objDMSUtil.GetDraftPDF(HttpContext.Current.Session("EID"), docid)
        If str = "fail" Then
            result = "Print Template is not available to Print Draft, Please contact Admin!"
        End If
        Return result
    End Function
    <System.Web.Services.WebMethod()>
    Public Shared Function DraftEdit(docid As String) As String
        Dim result As String = ""
        HttpContext.Current.Session("DRAFT") = "ISDRAFT"
        Dim objDC As New DataClass()
        Dim doctype As String = objDC.ExecuteQryScaller("select documenttype from mmm_mst_doc_draft where tid=" & docid)
        result = "/Documents.Aspx?SC=" & doctype.ToString() & "&tid=" & Val(docid) & ""
        Return result
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function DraftDelete(docid As String) As String
        Dim result As String = ""
        Dim objDC As New DataClass()
        Try
            objDC.ExecuteNonQuery("delete from mmm_mst_doc_item_draft where docid=" & docid)
            objDC.ExecuteNonQuery("delete from mmm_mst_doc_draft where tid=" & docid)
            result = "SUCCESS"
        Catch ex As Exception
            result = "FAIL"
        End Try
        Return result
    End Function


    'Draft functionality
    <System.Web.Services.WebMethod()>
    Public Shared Function GetDraftAct(documentType As String, page As Integer, pageSize As Integer, skip As Integer, take As Integer, sorting As List(Of SortDescription), filter As FilterContainer) As kGridHome
        Dim dataobj As New DataLib()
        Dim ret As New kGridHome()
        Dim jsonData As String = ""
        Dim dsFields As New DataSet()
        Dim QryFields As String = ""
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim strQuery As String = ""
        Dim strQueryTotalCount As String = ""
        Dim dsData As New DataSet()
        Dim dtCount As New DataTable()
        Try
            'paging logic==================================
            Dim from1 As Integer = skip + 1 '(page - 1) * pageSize + 1;
            Dim to1 As Integer = take * page 'page * pageSize;
            Dim sortingStr As String = ""

            '===============================================
            'Sorting logic====================================
            If sorting IsNot Nothing Then
                sortingStr = Sortgrid(sorting)
                'If sorting.Count <> 0 Then
                '    For i As Integer = 0 To sorting.Count - 1
                '        sortingStr += ", M." + sorting(i).field + " " + sorting(i).dir
                '    Next
                'End If
            End If
            '==================================================
            'filtering logic====================================
            Dim filters As String = ""
            If filter IsNot Nothing Then
                filters = filtergrid(filter)
            End If
            '==================================================

            sortingStr = sortingStr.TrimStart(","c)

            Dim SBQry As New StringBuilder()
            Dim StaticColumns As String = ""
            If sortingStr <> "" Then
                SBQry.Append("Select ROW_NUMBER() OVER (ORDER BY " & sortingStr & ") AS RowNumber,CAST(M.tid as varchar) as [DocDetID], CAST(M.tid as varchar) as [SYSTEMID], ")

                StaticColumns = "Select ROW_NUMBER() OVER (ORDER BY " & sortingStr & ") AS RowNumber, M.TID, CAST(M.tid as varchar) as [DocDetID], M.DocumentType,M.curstatus,U.Username,CONVERT(date,d.fdate,3)[RECEIVEDON],datediff(day,fdate,getdate())[PENDINGDAYS],"
            Else
                SBQry.Append("Select ROW_NUMBER() OVER (ORDER BY M.tid desc) AS RowNumber, CAST(M.tid as varchar) as [DocDetID], CAST(M.tid as varchar) as [SYSTEMID], ")

                StaticColumns = "Select ROW_NUMBER() OVER (ORDER BY M.tid desc) AS RowNumber, M.TID, CAST(M.tid as varchar) as [DocDetID], M.DocumentType,M.curstatus,U.Username,CONVERT(date,d.fdate,3)[RECEIVEDON],datediff(day,fdate,getdate())[PENDINGDAYS],"
            End If
            'SBQry.Append("Select Top CAST(M.TID as varchar) TID,CAST(M.tid as varchar) as [SYSTEMID], ")
            'query get total count--------------------------
            strQueryTotalCount = "SELECT COUNT(*) Total from MMM_MST_DOC_draft M with (nolock)  LEFT OUTER JOIN MMM_MST_USER U with (nolock) ON U.uid=oUID where M.documenttype='" & documentType & "' and m.curstatus<>'ARCHIVE' AND M.OUID = " & Val(HttpContext.Current.Session("UID").ToString()) & ""
            If filters <> "" Then
                strQueryTotalCount = strQueryTotalCount + " AND " & filters
            Else
                'strQueryTotalCount = strQuery
            End If
            '---------------------------------
            'strQuery = "select FieldName,FieldMapping,Type,datatype from mmm_mst_needtoact_config where eid=" & HttpContext.Current.Session("EID") & " and DocType='" & documentType & "' order by DisplayOrder"
            strQuery = "select con.FieldName,con.FieldMapping,con.Type,con.datatype from mmm_mst_needtoact_config con left join mmm_mst_fields fld on con.fieldMapping= fld.fieldMapping and con.doctype=fld.documenttype and con.eid=fld.eid   where con.eid=" & HttpContext.Current.Session("EID") & " and con.DocType='" & documentType & "' and (fld.fieldType <>'Auto Number' or fld.fieldType is null)  order by con.DisplayOrder"
            dsFields = DataLib.ExecuteDataSet(conStr, CommandType.Text, strQuery)
            If (dsFields.Tables(0).Rows().Count > 0) Then
                For i As Integer = 0 To dsFields.Tables(0).Rows.Count - 1
                    If (dsFields.Tables(0).Rows(i).Item("Type").ToString.Trim.ToUpper = "STATIC") Then
                        If (dsFields.Tables(0).Rows(i).Item("FieldMapping").ToString = "datediff(day,fdate,getdate())") Then
                            SBQry.Append(dsFields.Tables(0).Rows(i).Item("FieldMapping") & " As P_Days").Append(",")
                        ElseIf dsFields.Tables(0).Rows(i).Item("FieldMapping").ToString().Contains("COALESCE(D.AprStatus,M.curstatus)") Then
                            SBQry.Append("M.curstatus as curstatus").Append(",")
                        Else
                            If (dsFields.Tables(0).Rows(i).Item("FieldMapping").ToString = "d.fdate") Then
                                SBQry.Append("CONVERT(date,d.fdate,3) as fdate").Append(",")
                            ElseIf (dsFields.Tables(0).Rows(i).Item("FieldName").ToString = "Creation Date") Then
                                SBQry.Append("CONVERT(date,M.adate,3) as [Column2] ").Append(",")
                            Else
                                SBQry.Append(dsFields.Tables(0).Rows(i).Item("FieldMapping")).Append(",")
                            End If
                        End If

                    Else
                        If (dsFields.Tables(0).Rows(i).Item("FieldName").ToString = "Expiry Date") Then
                            SBQry.Append("CONVERT(date,M.fld43,3) as fld43").Append(",")
                        Else
                            If dsFields.Tables(0).Rows(i).Item("FieldMapping").ToString().Contains("dms.udf_split") Then
                                SBQry.Append(dsFields.Tables(0).Rows(i).Item("FieldMapping")).Append(",")
                            Else
                                SBQry.Append("M." & dsFields.Tables(0).Rows(i).Item("FieldMapping")).Append(",")
                            End If
                        End If
                    End If

                Next
                strQuery = Left(SBQry.ToString(), Len(SBQry.ToString()) - 1) & " from MMM_MST_DOC_draft M with (nolock)  LEFT OUTER JOIN MMM_MST_USER U with (nolock) ON U.uid=oUID  where M.documenttype='" & documentType & "' and m.curstatus<>'ARCHIVE' AND M.OUID = " & Val(HttpContext.Current.Session("UID").ToString()) & ""

                strQuery = "WITH Data AS (" & strQuery
                If filters <> "" Then
                    strQuery = strQuery + " AND " & filters
                End If

                strQuery = strQuery + ") SELECT * FROM Data WHERE RowNumber BETWEEN " & from1 & "AND " & to1

                'IF @SQLSortString IS NOT NULL
                strQuery = strQuery.Replace("d.fdate", "M.fdate")

                dsData = DataLib.ExecuteDataSet(conStr, CommandType.Text, strQuery)
                'ret.Column = CreateColCollection(dsFields.Tables(0))

            Else
                strQuery = "select * from MMM_MST_FIELDS where eid=" & HttpContext.Current.Session("EID").ToString() & " and Documenttype='" + documentType + "' and showondashboard=1"
                dsFields = DataLib.ExecuteDataSet(conStr, CommandType.Text, strQuery)
                For f As Integer = 0 To dsFields.Tables(0).Rows.Count - 1
                    StaticColumns = StaticColumns & "M." & dsFields.Tables(0).Rows(f).Item("FieldMapping") & ","
                Next

                Dim qrycolumn As String = StaticColumns
                qrycolumn = qrycolumn.Replace("M.fld10", "CONVERT(date,M.fld10,3) AS fld10")
                qrycolumn = Left(qrycolumn, Len(qrycolumn) - 1) & " from MMM_MST_DOC_draft M with (nolock)  LEFT OUTER JOIN MMM_MST_USER U with (nolock) ON U.uid=oUID  where M.documenttype='" & documentType & "' and m.curstatus<>'ARCHIVE' AND M.OUID = " & Val(HttpContext.Current.Session("UID").ToString()) & " "

                qrycolumn = "WITH Data AS (" & qrycolumn
                If filters <> "" Then
                    qrycolumn = qrycolumn + " AND " & filters
                End If

                qrycolumn = qrycolumn + ") SELECT * FROM Data WHERE RowNumber BETWEEN " & from1 & "AND " & to1
                qrycolumn = qrycolumn.Replace("d.fdate", "M.fdate")
                dsData = DataLib.ExecuteDataSet(conStr, CommandType.Text, qrycolumn)
                'dsFields = DataLib.ExecuteDataSet(conStr, CommandType.Text, QryFields)
                If dsData.Tables(0).Rows.Count > 0 Then
                    'ret.Column = CreateStaticColCollection(dsFields.Tables(0))
                    'jsonData = DataLib.JsonTableSerializer(dsData.Tables(0))
                End If
            End If

            'dsFields = DataLib.ExecuteDataSet(dataobj.conObj, CommandType.Text, QryFields)
            If dsData.Tables(0).Rows.Count > 0 Then

                'jsonData = JsonConvert.SerializeObject(dsData.Tables(0), Newtonsoft.Json.Formatting.None, serializerSettings)
                jsonData = JsonConvert.SerializeObject(dsData.Tables(0)) 'DataLib.JsonTableSerializer(dsData.Tables(0))
            Else
                jsonData = JsonConvert.SerializeObject(dsData.Tables(0))
            End If
            ret.total = DataLib.ExecuteDataSet(conStr, CommandType.Text, strQueryTotalCount).Tables(0).Rows(0)("Total")
            ret.Data = jsonData
            Return ret
            'Json(ret, JsonRequestBehavior.AllowGet)
            'Return dsMaster
        Catch ex As Exception
            Throw
        Finally
            'oda.Dispose()

        End Try
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function GetDataMyRequestGrid(documentType As String, page As Integer, pageSize As Integer, skip As Integer, take As Integer, sorting As List(Of SortDescription), filter As FilterContainer) As kGridHome
        Dim dataobj As New DataLib()
        Dim ret As New kGridHome()
        Dim jsonData As String = ""
        Dim dsFields As New DataSet()
        Dim QryFields As String = ""
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim strQuery As String = ""
        Dim strQueryTotalCount As String = ""
        Dim dsData As New DataSet()
        Dim dtCount As New DataTable()
        Try
            'paging logic==================================
            Dim from1 As Integer = skip + 1 '(page - 1) * pageSize + 1;
            Dim to1 As Integer = take * page 'page * pageSize;
            Dim sortingStr As String = ""

            '===============================================
            'Sorting logic====================================
            If sorting IsNot Nothing Then
                sortingStr = Sortgrid(sorting)
                'If sorting.Count <> 0 Then
                '    For i As Integer = 0 To sorting.Count - 1
                '        sortingStr += ", M." + sorting(i).field + " " + sorting(i).dir
                '    Next
                'End If
            End If
            '==================================================
            'filtering logic====================================
            Dim filters As String = ""
            If filter IsNot Nothing Then
                filters = filtergrid(filter)
            End If
            '==================================================

            sortingStr = sortingStr.TrimStart(","c)

            Dim SBQry As New StringBuilder()
            Dim StaticColumns As String = ""
            If sortingStr <> "" Then
                SBQry.Append("Select ROW_NUMBER() OVER (ORDER BY " & sortingStr & ") AS RowNumber, CAST(M.tid as varchar) as [DocDetID], CAST(M.tid as varchar) as [SYSTEMID], ")

                StaticColumns = "Select ROW_NUMBER() OVER (ORDER BY " & sortingStr & ") AS RowNumber, CAST(M.tid as varchar) as [DocDetID], M.TID,M.DocumentType,M.curstatus,U.Username,CONVERT(date,d.fdate,3)[RECEIVEDON],datediff(day,fdate,getdate())[PENDINGDAYS],"
            Else
                SBQry.Append("Select ROW_NUMBER() OVER (ORDER BY M.tid desc) AS RowNumber, CAST(M.tid as varchar) as [DocDetID], CAST(M.tid as varchar) as [SYSTEMID], ")

                StaticColumns = "Select ROW_NUMBER() OVER (ORDER BY M.tid desc) AS RowNumber, CAST(M.tid as varchar) as [DocDetID], M.TID,M.DocumentType,M.curstatus,U.Username,CONVERT(date,d.fdate,3)[RECEIVEDON],datediff(day,fdate,getdate())[PENDINGDAYS],"
            End If
            'SBQry.Append("Select Top CAST(M.TID as varchar) TID,CAST(M.tid as varchar) as [SYSTEMID], ")
            'query get total count--------------------------
            strQueryTotalCount = "SELECT COUNT(*) Total from MMM_MST_DOC M with (nolock) LEFT OUTER JOIN MMM_DOC_DTL D with (nolock) on D.tid=M.lasttid LEFT OUTER JOIN MMM_MST_USER U with (nolock) ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP with (nolock) ON AP.uid=D.UserID where M.oUid = " & Val(HttpContext.Current.Session("UID").ToString()) & "AND M.documenttype='" & documentType & "' and m.curstatus  not in ('ARCHIVE','REJECTED') "
            If filters <> "" Then
                strQueryTotalCount = strQueryTotalCount + " AND " & filters
            Else
                'strQueryTotalCount = strQuery
            End If
            '---------------------------------
            strQuery = "select FieldName,FieldMapping,Type,datatype from mmm_mst_needtoact_config where eid=" & HttpContext.Current.Session("EID") & " and DocType='" & documentType & "' order by DisplayOrder"
            dsFields = DataLib.ExecuteDataSet(conStr, CommandType.Text, strQuery)
            If (dsFields.Tables(0).Rows().Count > 0) Then
                For i As Integer = 0 To dsFields.Tables(0).Rows.Count - 1
                    If (dsFields.Tables(0).Rows(i).Item("Type").ToString.Trim.ToUpper = "STATIC") Then
                        If (dsFields.Tables(0).Rows(i).Item("FieldMapping").ToString = "datediff(day,fdate,getdate())") Then
                            SBQry.Append(dsFields.Tables(0).Rows(i).Item("FieldMapping") & " As P_Days").Append(",")
                        Else
                            If (dsFields.Tables(0).Rows(i).Item("FieldMapping").ToString = "d.fdate") Then
                                SBQry.Append("CONVERT(date,d.fdate,3) as fdate").Append(",")
                            ElseIf (dsFields.Tables(0).Rows(i).Item("FieldName").ToString = "Creation Date") Then
                                SBQry.Append("CONVERT(date,M.adate,3)[Column2]").Append(",")
                            Else
                                SBQry.Append(dsFields.Tables(0).Rows(i).Item("FieldMapping")).Append(",")
                            End If
                        End If

                    Else
                        If (dsFields.Tables(0).Rows(i).Item("FieldName").ToString = "Expiry Date") Then
                            SBQry.Append("CONVERT(date,M.fld43,3) as fld43").Append(",")
                        Else
                            If dsFields.Tables(0).Rows(i).Item("FieldMapping").ToString().Contains("dms.udf_split(") Then
                                SBQry.Append(dsFields.Tables(0).Rows(i).Item("FieldMapping")).Append(",")
                            Else
                                SBQry.Append("M." & dsFields.Tables(0).Rows(i).Item("FieldMapping")).Append(",")
                            End If

                        End If
                    End If

                Next
                strQuery = Left(SBQry.ToString(), Len(SBQry.ToString()) - 1) & " from MMM_MST_DOC M with (nolock) LEFT OUTER JOIN MMM_DOC_DTL D with (nolock) on D.tid=M.lasttid LEFT OUTER JOIN MMM_MST_USER U with (nolock) ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP with (nolock) ON AP.uid=D.UserID where M.oUid = " & Val(HttpContext.Current.Session("UID").ToString()) & " and curstatus not in ('ARCHIVE','REJECTED') AND M.documenttype='" & documentType & "'"


                strQuery = "WITH Data AS (" & strQuery
                If filters <> "" Then
                    strQuery = strQuery + " AND " & filters
                End If

                strQuery = strQuery + ") SELECT * FROM Data WHERE RowNumber BETWEEN " & from1 & "AND " & to1

                'IF @SQLSortString IS NOT NULL


                dsData = DataLib.ExecuteDataSet(conStr, CommandType.Text, strQuery)
                'ret.Column = CreateColCollection(dsFields.Tables(0))

            Else
                strQuery = "select * from MMM_MST_FIELDS where eid=" & HttpContext.Current.Session("EID").ToString() & " and Documenttype='" + documentType + "' and showondashboard=1"
                dsFields = DataLib.ExecuteDataSet(conStr, CommandType.Text, strQuery)
                For f As Integer = 0 To dsFields.Tables(0).Rows.Count - 1
                    StaticColumns = StaticColumns & "M." & dsFields.Tables(0).Rows(f).Item("FieldMapping") & ","
                Next

                Dim qrycolumn As String = StaticColumns
                qrycolumn = qrycolumn.Replace("M.fld10", "CONVERT(date,M.fld10,3) AS fld10")
                qrycolumn = Left(qrycolumn, Len(qrycolumn) - 1) & " from MMM_MST_DOC M with (nolock) LEFT OUTER JOIN MMM_DOC_DTL D with (nolock) on D.tid=M.lasttid LEFT OUTER JOIN MMM_MST_USER U with (nolock) ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP with (nolock) ON AP.uid=D.UserID where M.oUid = " & Val(HttpContext.Current.Session("UID").ToString()) & "AND M.documenttype='" & documentType & "' and m.curstatus not in ('ARCHIVE','REJECTED')"

                qrycolumn = "WITH Data AS (" & qrycolumn
                If filters <> "" Then
                    qrycolumn = qrycolumn + " AND " & filters
                End If

                qrycolumn = qrycolumn + ") SELECT * FROM Data WHERE RowNumber BETWEEN " & from1 & "AND " & to1
                dsData = DataLib.ExecuteDataSet(conStr, CommandType.Text, qrycolumn)
                'dsFields = DataLib.ExecuteDataSet(conStr, CommandType.Text, QryFields)
                If dsData.Tables(0).Rows.Count > 0 Then
                    'ret.Column = CreateStaticColCollection(dsFields.Tables(0))
                    'jsonData = DataLib.JsonTableSerializer(dsData.Tables(0))
                End If
            End If

            'dsFields = DataLib.ExecuteDataSet(dataobj.conObj, CommandType.Text, QryFields)
            If dsData.Tables(0).Rows.Count > 0 Then
                'jsonData = JsonConvert.SerializeObject(dsData.Tables(0), Newtonsoft.Json.Formatting.None, serializerSettings)
                jsonData = JsonConvert.SerializeObject(dsData.Tables(0)) 'DataLib.JsonTableSerializer(dsData.Tables(0))
            Else
                jsonData = JsonConvert.SerializeObject(dsData.Tables(0))
            End If
            ret.total = DataLib.ExecuteDataSet(conStr, CommandType.Text, strQueryTotalCount).Tables(0).Rows(0)("Total")
            ret.Data = jsonData
            Return ret
            'Json(ret, JsonRequestBehavior.AllowGet)
            'Return dsMaster
        Catch ex As Exception
            Throw
        Finally
            'oda.Dispose()

        End Try
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function GetDataHistory(documentType As String, page As Integer, pageSize As Integer, skip As Integer, take As Integer, sorting As List(Of SortDescription), filter As FilterContainer) As kGridHome
        Dim dataobj As New DataLib()
        Dim ret As New kGridHome()
        Dim jsonData As String = ""
        Dim dsFields As New DataSet()
        Dim QryFields As String = ""
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim strQuery As String = ""
        Dim strQueryTotalCount As String = ""
        Dim dsData As New DataSet()
        Dim dtCount As New DataTable()
        Try
            'paging logic==================================
            Dim from1 As Integer = skip + 1 '(page - 1) * pageSize + 1;
            Dim to1 As Integer = take * page 'page * pageSize;
            Dim sortingStr As String = ""

            '===============================================
            'Sorting logic====================================
            If sorting IsNot Nothing Then
                sortingStr = Sortgrid(sorting)
                'If sorting.Count <> 0 Then
                '    For i As Integer = 0 To sorting.Count - 1
                '        sortingStr += ", M." + sorting(i).field + " " + sorting(i).dir
                '    Next
                'End If
            End If
            '==================================================
            'filtering logic====================================
            Dim filters As String = ""
            If filter IsNot Nothing Then
                filters = filtergrid(filter)
            End If
            '==================================================

            sortingStr = sortingStr.TrimStart(","c)

            Dim SBQry As New StringBuilder()
            Dim StaticColumns As String = ""
            If sortingStr <> "" Then
                SBQry.Append("Select ROW_NUMBER() OVER (ORDER BY " & sortingStr & ") AS RowNumber, CAST(M.tid as varchar) as [DocDetID], CAST(M.tid as varchar) as [SYSTEMID], ")
                StaticColumns = "Select ROW_NUMBER() OVER (ORDER BY " & sortingStr & ") AS RowNumber, CAST(M.tid as varchar) as [DocDetID], M.TID,M.DocumentType,M.curstatus,U.Username,CONVERT(date,d.fdate,3)[RECEIVEDON],datediff(day,fdate,getdate())[PENDINGDAYS],"
            Else
                SBQry.Append("Select ROW_NUMBER() OVER (ORDER BY M.tid desc) AS RowNumber, CAST(M.tid as varchar) as [DocDetID], CAST(M.tid as varchar) as [SYSTEMID], ")
                StaticColumns = "Select ROW_NUMBER() OVER (ORDER BY M.tid desc) AS RowNumber, CAST(M.tid as varchar) as [DocDetID], M.TID,M.DocumentType,M.curstatus,U.Username,CONVERT(date,d.fdate,3)[RECEIVEDON],datediff(day,fdate,getdate())[PENDINGDAYS],"
            End If
            'query get total count--------------------------
            'strQueryTotalCount = "SELECT COUNT(*) Total from MMM_MST_DOC M with (nolock) LEFT OUTER JOIN MMM_DOC_DTL D with (nolock) on D.docid =M.tid LEFT OUTER JOIN MMM_MST_USER U with (nolock) ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP with (nolock) ON AP.uid=D.UserID where D.UserID = " & Val(HttpContext.Current.Session("UID").ToString()) & " and curstatus<> 'ARCHIVE' and aprstatus is not null AND M.documenttype='" & documentType & "'"
            strQueryTotalCount = "SELECT COUNT(distinct M.tid) Total from MMM_MST_DOC M with (nolock) JOIN MMM_DOC_DTL D with (nolock) on D.docid=M.tid LEFT OUTER JOIN MMM_MST_USER U with (nolock) ON U.uid=oUID where D.userid = " & Val(HttpContext.Current.Session("UID").ToString()) & " and d.tid=((select max(tid) from mmm_doc_dtl where docid=m.tid and userid=" & Val(HttpContext.Current.Session("UID").ToString()) & ")) and curstatus not in ('ARCHIVE','REJECTED') AND M.documenttype='" & documentType & "'"
            If filters <> "" Then
                strQueryTotalCount = strQueryTotalCount + " AND " & filters
            Else
                'strQueryTotalCount = strQuery
            End If
            '---------------------------------
            strQuery = "select FieldName,FieldMapping,Type,datatype from mmm_mst_needtoact_config where eid=" & HttpContext.Current.Session("EID") & " and DocType='" & documentType & "' order by DisplayOrder"
            dsFields = DataLib.ExecuteDataSet(conStr, CommandType.Text, strQuery)
            If (dsFields.Tables(0).Rows().Count > 0) Then
                For i As Integer = 0 To dsFields.Tables(0).Rows.Count - 1
                    If (dsFields.Tables(0).Rows(i).Item("Type").ToString.Trim.ToUpper = "STATIC") Then
                        If (dsFields.Tables(0).Rows(i).Item("FieldMapping").ToString = "datediff(day,fdate,getdate())") Then
                            'SBQry.Append(dsFields.Tables(0).Rows(i).Item("FieldMapping") & " As P_Days").Append(",")
                            SBQry.Append("(select datediff(day,max(fdate),getdate()) from mmm_doc_dtl where docid=m.tid) As P_Days").Append(",")
                        ElseIf dsFields.Tables(0).Rows(i).Item("FieldMapping").ToString().Contains("COALESCE(D.AprStatus,M.curstatus)") Then
                            'SBQry.Append("iif(COALESCE(D.AprStatus,M.curstatus)='UPLOADED', + isnull( D.Remarks ,'Re-Submitted') ,COALESCE(D.AprStatus,M.curstatus) +'') as curstatus").Append(",")
                            SBQry.Append("curstatus as curstatus").Append(",")
                        Else
                            If (dsFields.Tables(0).Rows(i).Item("FieldMapping").ToString = "d.fdate") Then
                                'SBQry.Append("CONVERT(date,d.fdate,3) as fdate").Append(",")
                                SBQry.Append("(select CONVERT(date,max(fdate),3) from mmm_doc_dtl where docid=m.tid) As [ReceivedON] ").Append(",")
                            ElseIf (dsFields.Tables(0).Rows(i).Item("FieldName").ToString = "Creation Date") Then
                                SBQry.Append("CONVERT(date,M.adate,3) [Column2]").Append(",")
                            Else
                                SBQry.Append(dsFields.Tables(0).Rows(i).Item("FieldMapping")).Append(",")
                            End If
                        End If

                    Else
                        If (dsFields.Tables(0).Rows(i).Item("FieldName").ToString = "Expiry Date") Then
                            SBQry.Append("CONVERT(date,M.fld43,3) as fld43").Append(",")
                        Else
                            If Convert.ToString(dsFields.Tables(0).Rows(i).Item("FieldMapping")).Contains("dms.udf_split(") Then
                                SBQry.Append(dsFields.Tables(0).Rows(i).Item("FieldMapping")).Append(",")
                            Else
                                SBQry.Append("M." & dsFields.Tables(0).Rows(i).Item("FieldMapping")).Append(",")
                            End If

                        End If
                    End If

                Next
                strQuery = Left(SBQry.ToString(), Len(SBQry.ToString()) - 1) & " from MMM_MST_DOC M with (nolock) JOIN MMM_DOC_DTL D with (nolock) on D.docid=M.tid   LEFT OUTER JOIN MMM_MST_USER U with (nolock) ON U.uid=oUID where D.userid = " & Val(HttpContext.Current.Session("UID").ToString()) & " and d.tid=((select max(tid) from mmm_doc_dtl where docid=m.tid and userid=" & Val(HttpContext.Current.Session("UID").ToString()) & ")) and curstatus  not in ('ARCHIVE','REJECTED')  AND M.documenttype='" & documentType & "'"


                strQuery = "WITH Data AS (" & strQuery
                'strQuery = strQuery
                If filters <> "" Then
                    strQuery = strQuery + " AND " & filters
                End If

                strQuery = strQuery + ") SELECT * FROM Data WHERE RowNumber BETWEEN " & from1 & "AND " & to1
                'strQuery = strQuery
                'IF @SQLSortString IS NOT NULL

                dsData = DataLib.ExecuteDataSet(conStr, CommandType.Text, strQuery)
                'ret.Column = CreateColCollection(dsFields.Tables(0))
            Else
                strQuery = "select * from MMM_MST_FIELDS where eid=" & HttpContext.Current.Session("EID").ToString() & " and Documenttype='" + documentType + "' and showondashboard=1"
                dsFields = DataLib.ExecuteDataSet(conStr, CommandType.Text, strQuery)
                For f As Integer = 0 To dsFields.Tables(0).Rows.Count - 1
                    StaticColumns = StaticColumns & "M." & dsFields.Tables(0).Rows(f).Item("FieldMapping") & ","
                Next

                Dim qrycolumn As String = StaticColumns
                qrycolumn = qrycolumn.Replace("M.fld10", "CONVERT(date,M.fld10,3) AS fld10")
                'qrycolumn = Left(qrycolumn, Len(qrycolumn) - 1) & " from MMM_MST_DOC M with (nolock) LEFT OUTER JOIN MMM_DOC_DTL D with (nolock) on D.docid =M.tid LEFT OUTER JOIN MMM_MST_USER U with (nolock) ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP with (nolock) ON AP.uid=D.UserID where D.UserID = " & Val(HttpContext.Current.Session("UID").ToString()) & " and curstatus<> 'ARCHIVE' and aprstatus is not null AND M.documenttype='" & documentType & "'"
                qrycolumn = Left(qrycolumn, Len(qrycolumn) - 1) & " from MMM_MST_DOC M with (nolock) JOIN MMM_DOC_DTL D with (nolock) on D.docid=M.tid  LEFT OUTER JOIN MMM_MST_USER U with (nolock) ON U.uid=oUID where D.userid = " & Val(HttpContext.Current.Session("UID").ToString()) & " and d.tid=((select max(tid) from mmm_doc_dtl where docid=m.tid and userid=" & Val(HttpContext.Current.Session("UID").ToString()) & ")) and curstatus  not in ('ARCHIVE','REJECTED') AND M.documenttype='" & documentType & "'"

                qrycolumn = "WITH Data AS (" & qrycolumn
                If filters <> "" Then
                    qrycolumn = qrycolumn + " AND " & filters
                End If

                qrycolumn = qrycolumn + ") SELECT * FROM Data WHERE RowNumber BETWEEN " & from1 & "AND " & to1
                dsData = DataLib.ExecuteDataSet(conStr, CommandType.Text, qrycolumn)
                'dsFields = DataLib.ExecuteDataSet(conStr, CommandType.Text, QryFields)
                If dsData.Tables(0).Rows.Count > 0 Then
                    'ret.Column = CreateStaticColCollection(dsFields.Tables(0))
                    'jsonData = DataLib.JsonTableSerializer(dsData.Tables(0))
                End If
            End If

            'dsFields = DataLib.ExecuteDataSet(dataobj.conObj, CommandType.Text, QryFields)
            If dsData.Tables(0).Rows.Count > 0 Then

                'jsonData = JsonConvert.SerializeObject(dsData.Tables(0), Newtonsoft.Json.Formatting.None, serializerSettings)
                jsonData = JsonConvert.SerializeObject(dsData.Tables(0)) 'DataLib.JsonTableSerializer(dsData.Tables(0))
            Else
                jsonData = JsonConvert.SerializeObject(dsData.Tables(0))
            End If
            ret.total = DataLib.ExecuteDataSet(conStr, CommandType.Text, strQueryTotalCount).Tables(0).Rows(0)("Total")
            ret.Data = jsonData
            Return ret
            'Json(ret, JsonRequestBehavior.AllowGet)
            'Return dsMaster
        Catch ex As Exception
            Throw
        Finally
            'oda.Dispose()

        End Try
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function GetDataHistory22(DocumentType As String) As kGridHome

        Dim dataobj As New DataLib()
        Dim ret As New kGridHome()
        Dim jsonData As String = ""
        Dim dsFields As New DataSet()
        Dim QryFields As String = ""
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim strQuery As String = ""
        Dim dsData As New DataSet()
        Try
            'Dim StrQry 
            Dim SBQry As New StringBuilder()
            Dim StaticColumns As String = "Select distinct M.TID,M.DocumentType,M.curstatus,U.Username,CONVERT(date,d.fdate,3)[RECEIVEDON],datediff(day,fdate,getdate())[PENDINGDAYS],"
            SBQry.Append("Select M.TID, M.tid as [SYSTEM ID], ")
            strQuery = "select FieldName,FieldMapping,Type,datatype from mmm_mst_needtoact_config where eid=" & HttpContext.Current.Session("EID") & " and DocType='" & DocumentType & "' order by DisplayOrder"
            dsFields = DataLib.ExecuteDataSet(conStr, CommandType.Text, strQuery)
            If (dsFields.Tables(0).Rows().Count > 0) Then
                For i As Integer = 0 To dsFields.Tables(0).Rows.Count - 1
                    If (dsFields.Tables(0).Rows(i).Item("Type").ToString.Trim.ToUpper = "STATIC") Then
                        If (dsFields.Tables(0).Rows(i).Item("Type").ToString = "datediff(day,fdate,getdate())") Then
                            SBQry.Append(dsFields.Tables(0).Rows(i).Item("FieldMapping") & " As P_Days").Append(",")
                        Else
                            If (dsFields.Tables(0).Rows(i).Item("FieldMapping").ToString = "d.fdate") Then
                                SBQry.Append("CONVERT(date,d.fdate,3) as fdate").Append(",")
                            ElseIf (dsFields.Tables(0).Rows(i).Item("FieldName").ToString = "Creation Date") Then
                                SBQry.Append("CONVERT(date,M.adate,3)[Column2]").Append(",")
                            Else
                                SBQry.Append(dsFields.Tables(0).Rows(i).Item("FieldMapping")).Append(",")
                            End If
                        End If

                    Else
                        If (dsFields.Tables(0).Rows(i).Item("FieldName").ToString = "Expiry Date") Then
                            SBQry.Append("CONVERT(date,M.fld43,3) as fld43").Append(",")
                        Else
                            SBQry.Append("M." & dsFields.Tables(0).Rows(i).Item("FieldMapping")).Append(",")
                        End If
                    End If

                Next
                strQuery = Left(SBQry.ToString(), Len(SBQry.ToString()) - 1) & " from MMM_MST_DOC M with (nolock) LEFT OUTER JOIN MMM_DOC_DTL D with (nolock) on D.docid =M.tid LEFT OUTER JOIN MMM_MST_USER U with (nolock) ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP with (nolock) ON AP.uid=D.UserID where D.UserID = " & Val(HttpContext.Current.Session("UID").ToString()) & " and curstatus<> 'ARCHIVE' and aprstatus is not null AND M.documenttype='" & DocumentType & "' order by M.tid desc"

                dsData = DataLib.ExecuteDataSet(conStr, CommandType.Text, strQuery)
                ret.Column = CreateColCollection(dsFields.Tables(0))
            Else
                strQuery = "select * from MMM_MST_FIELDS where eid=" & HttpContext.Current.Session("EID").ToString() & " and Documenttype='" + DocumentType + "' and showondashboard=1"
                dsFields = DataLib.ExecuteDataSet(conStr, CommandType.Text, strQuery)
                For f As Integer = 0 To dsFields.Tables(0).Rows.Count - 1
                    StaticColumns = StaticColumns & "M." & dsFields.Tables(0).Rows(f).Item("FieldMapping") & ","
                Next

                Dim qrycolumn As String = StaticColumns
                qrycolumn = qrycolumn.Replace("M.fld10", "CONVERT(date,M.fld10,3) AS fld10")
                qrycolumn = Left(qrycolumn, Len(qrycolumn) - 1) & " from MMM_MST_DOC M with (nolock) LEFT OUTER JOIN MMM_DOC_DTL D with (nolock) on D.docid =M.tid LEFT OUTER JOIN MMM_MST_USER U with (nolock) ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP with (nolock) ON AP.uid=D.UserID where D.UserID = " & Val(HttpContext.Current.Session("UID").ToString()) & " and curstatus<> 'ARCHIVE' and aprstatus is not null AND M.documenttype='" & DocumentType & "' order by M.tid desc"
                dsData = DataLib.ExecuteDataSet(conStr, CommandType.Text, qrycolumn)
                'dsFields = DataLib.ExecuteDataSet(conStr, CommandType.Text, QryFields)
                If dsData.Tables(0).Rows.Count > 0 Then
                    ret.Column = CreateStaticColCollection(dsFields.Tables(0))
                    'jsonData = DataLib.JsonTableSerializer(dsData.Tables(0))
                End If
            End If

            'dsFields = DataLib.ExecuteDataSet(dataobj.conObj, CommandType.Text, QryFields)
            If dsData.Tables(0).Rows.Count > 0 Then
                'jsonData = JsonConvert.SerializeObject(dsData.Tables(0), Newtonsoft.Json.Formatting.None, serializerSettings)
                jsonData = JsonConvert.SerializeObject(dsData.Tables(0)) 'DataLib.JsonTableSerializer(dsData.Tables(0))
            Else
                jsonData = JsonConvert.SerializeObject(dsData.Tables(0))
            End If
            ret.Data = jsonData
            ret.Count = dsData.Tables(0).Rows.Count
            Return ret
            'Json(ret, JsonRequestBehavior.AllowGet)
            'Return dsMaster
        Catch ex As Exception
            Throw
        Finally
            'oda.Dispose()

        End Try
    End Function

    Public Shared Function CreateStaticColCollection(dt As DataTable) As List(Of kColumnHome)
        Dim listcol As New List(Of kColumnHome)()
        Dim i As Integer = 0
        Dim obj As kColumnHome
        'Logic For adding Static Column into datatable By Nidhi.
        listcol.Add(New kColumnHome("TID", "SYSTEM ID", "number", ""))
        listcol.Add(New kColumnHome("DocumentType", "SUBJECT", "string", ""))
        listcol.Add(New kColumnHome("curstatus", "STATUS", "string", ""))
        listcol.Add(New kColumnHome("Username", "CREATED BY", "string", ""))
        'hh:mm:ss tt
        listcol.Add(New kColumnHome("RECEIVEDON", "RECEIVEDON", "date", "{0:dd/MM/yyyy}"))
        listcol.Add(New kColumnHome("PENDINGDAYS", "PENDING DAYS", "number", ""))
        'listcol.Add(New kColumn("PRIORITY", "PRIORITY", "number", "", 100))
        For f As Integer = 0 To dt.Rows.Count - 1
            obj = New kColumnHome()
            obj.field = dt.Rows(f).Item("fieldMapping")
            obj.title = dt.Rows(f).Item("Displayname")
            obj.width = 200
            obj.type = "string"
            '{0:MM-dd-yyyy}
            'for dynamic column filtering..
            If (dt.Rows(f).Item("datatype")) = "Numeric" Then
                obj.type = "number"
                obj.format = ""
                'obj.filterable = ""
            ElseIf (dt.Rows(f).Item("datatype")) = "Datetime" Then
                obj.type = "date"
                obj.format = "{0:dd/MM/yyyy}"
            Else
            End If
            listcol.Add(obj)
        Next
        Return listcol
    End Function
    Public Shared Function CreateColCollection(dt As DataTable) As List(Of kColumnHome)
        Dim listcol As New List(Of kColumnHome)()
        Dim obj As kColumnHome
        Dim strQuery As String = ""
        Dim Ds As New DataSet()
        For p As Integer = 0 To dt.Rows.Count - 1
            obj = New kColumnHome()
            If (Convert.ToString(dt.Rows(p).Item("FieldMapping")) = "datediff(day,fdate,getdate())") Then
                obj.field = "P_Days"
                obj.title = dt.Rows(p).Item("FieldName")
                obj.width = IIf(IsNothing(dt.Rows(p).Item("ColWidth")), 200, dt.Rows(p).Item("ColWidth"))
                'obj.type = "number"
            ElseIf (Convert.ToString(dt.Rows(p).Item("FieldMapping")) = "CONVERT(VARCHAR(10),M.adate,105)") Then
                obj.field = "Column2"
                obj.title = dt.Rows(p).Item("FieldName")
                obj.width = IIf(IsNothing(dt.Rows(p).Item("ColWidth")), 200, dt.Rows(p).Item("ColWidth"))
                'obj.type = "string"
                'obj.format = "{0:dd/MM/yy}"
            ElseIf Convert.ToString(dt.Rows(p).Item("FieldMapping")).Contains("dms.udf_split") Then
                Dim text As String() = Convert.ToString(dt.Rows(p).Item("FieldMapping")).Split(")")
                If text.Length > 0 Then
                    obj.field = Replace(text(1), "as", "")
                    obj.title = dt.Rows(p).Item("FieldName")
                    obj.width = IIf(IsNothing(dt.Rows(p).Item("ColWidth")), 200, dt.Rows(p).Item("ColWidth"))
                End If
            ElseIf Convert.ToString(dt.Rows(p).Item("FieldMapping")).Contains(" as ") Then
                Dim text As String() = Convert.ToString(dt.Rows(p).Item("FieldMapping")).Split(")")
                If text.Length > 0 Then
                    obj.field = Replace(text(1), "as", "")
                    obj.title = dt.Rows(p).Item("FieldName")
                    obj.width = IIf(IsNothing(dt.Rows(p).Item("ColWidth")), 200, dt.Rows(p).Item("ColWidth"))
                End If
            Else
                obj.field = dt.Rows(p).Item("FieldMapping").ToString.Replace("M.", "").Replace("d.", "").Replace("U.", "").Replace("m.", "")
                obj.title = dt.Rows(p).Item("FieldName")
                obj.width = IIf(IsNothing(dt.Rows(p).Item("ColWidth")), 200, dt.Rows(p).Item("ColWidth"))
                'obj.type = "string"
            End If

            If (Convert.ToString(dt.Rows(p).Item("datatype"))) = "Numeric" Then
                obj.type = "number"
                'obj.filterable = "{ui: function (element) {element.kendoNumericTextBox({format: 'n0'});}}"

            ElseIf (Convert.ToString(dt.Rows(p).Item("datatype"))) = "Datetime" Then
                obj.type = "date"
                obj.format = "{0:dd/MM/yyyy}"
            Else
            End If
            'obj.width = 100

            listcol.Add(obj)
        Next
        Return listcol
    End Function
#End Region

#Region "Widgets"
    <System.Web.Services.WebMethod()>
    Shared Function GetWidgets() As String
        Dim ret As String = ""
        Dim EID As Integer, DBName As String, Roles As String
        Dim obj As New WidgetHome()
        Try
            EID = Convert.ToInt32(HttpContext.Current.Session("EID"))
            DBName = "MainHome"
            Roles = Convert.ToString(HttpContext.Current.Session("UserRole"))
            Dim arr = Roles.Split(",")
            ret = obj.GetWidgets(EID, DBName, Roles)
        Catch ex As Exception
        End Try
        Return ret
    End Function
    <System.Web.Services.WebMethod()>
    Shared Function GetWidgets1(tid As Integer) As String
        Dim ret As String = ""
        Dim EID As Integer, DBName As String, Roles As String
        Dim obj As New WidgetHome()
        Dim ds As New DataSet()
        Dim jsonData As String = ""
        Dim serializerSettings As New JsonSerializerSettings()
        serializerSettings.Converters.Add(New DataTableConverter())
        Try
            EID = Convert.ToInt32(HttpContext.Current.Session("EID"))
            DBName = "MainHome"
            Roles = Convert.ToString(HttpContext.Current.Session("UserRole"))
            Dim arr = Roles.Split(",")
            ds = obj.GetWidgets(EID, DBName, Roles, Convert.ToInt32(tid))
            If ds.Tables(0).Rows.Count > 0 Then
                jsonData = JsonConvert.SerializeObject(ds.Tables(0), Newtonsoft.Json.Formatting.None, serializerSettings)
                ret = jsonData
            End If
            Return ret
        Catch ex As Exception
        End Try
        Return ret
    End Function

    <System.Web.Services.WebMethod()>
    Shared Function GetUsefullLink() As UseFullLinkWeidget
        Dim ret As New UseFullLinkWeidget()
        Dim EID As Integer
        Dim obj As New WidgetHome()
        Try
            EID = Convert.ToInt32(HttpContext.Current.Session("EID"))
            ret = obj.GetUsefullLink(EID)
        Catch ex As Exception
        End Try
        Return ret 'JsonConvert.SerializeObject(ret)
    End Function
    <System.Web.Services.WebMethod()>
    Shared Function GetCustomWidget(tid As Integer) As kGridWeidget
        Dim ret As New kGridWeidget()
        Try
            Dim obj As New WidgetHome()
            Dim UID = Convert.ToInt32(HttpContext.Current.Session("UID"))
            Dim Roles = Convert.ToString(HttpContext.Current.Session("UserRole"))
            ret = obj.GetCustomWidget(tid, UID, Roles:=Roles)
        Catch ex As Exception
        End Try
        Return ret
    End Function

    <System.Web.Services.WebMethod()>
    Shared Function GetFirstlevelWidget(data As String, tid As String) As kGridWeidget
        Dim ret As New kGridWeidget()
        Dim EID As Integer, DBName As String
        Dim obj As New WidgetHome()
        Dim arrItem As String()
        Dim jsonData As String = ""
        Dim arrDataChild As String()
        'Dim obj As New Widget()
        Dim ds As New DataSet()
        Dim dsQ As New DataSet()
        Dim serializerSettings As New JsonSerializerSettings()
        Try
            Dim UID = Convert.ToInt32(HttpContext.Current.Session("UID"))
            Dim Roles = Convert.ToString(HttpContext.Current.Session("UserRole"))
            EID = Convert.ToInt32(HttpContext.Current.Session("EID"))
            DBName = "MainHome"
            arrItem = data.Split("|")
            ds = obj.GetWidgets(EID, DBName, Roles, Convert.ToInt32(tid))
            If ds.Tables(0).Rows.Count > 0 Then
                If ds.Tables(0).Rows(0).Item("SecondlevelQuery").ToString.Length > 10 Then
                    ret.ShowAction = "yes"
                Else
                    ret.ShowAction = "no"
                End If
                ret.WindowWidth = ds.Tables(0).Rows(0).Item("Firstlevelwidth").ToString()
                Dim Query = ds.Tables(0).Rows(0).Item("FirstlevelQuery")
                'Replacing Query with value
                Query = Query.ToString.Replace("{UID}", UID)

                For k As Integer = 0 To arrItem.Length - 1
                    arrDataChild = arrItem(k).Trim().Split("::")
                    Dim search As String = "{" & arrDataChild(0).Trim() & "}"
                    Query = Query.ToString.Replace(search.ToString(), arrDataChild(2).Trim().ToString())
                Next
                Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
                Using con = New SqlConnection(conStr)
                    Using da As New SqlDataAdapter(Query, con)
                        da.Fill(dsQ)
                    End Using
                End Using
            End If
            If dsQ.Tables(0).Rows.Count > 0 Then
                For k As Integer = 0 To dsQ.Tables(0).Columns.Count - 1
                    dsQ.Tables(0).Columns(k).ColumnName = dsQ.Tables(0).Columns(k).ColumnName.Replace(" ", "_").Replace("(", "").Replace(")", "")
                Next
                serializerSettings.Converters.Add(New DataTableConverter())
                jsonData = JsonConvert.SerializeObject(dsQ.Tables(0), Newtonsoft.Json.Formatting.None, serializerSettings)
            End If
            ret.Data = jsonData
            Dim objG As kColumnWeidget
            Dim lstColName As New List(Of kColumnWeidget)()
            For Each column As DataColumn In dsQ.Tables(0).Columns
                objG = New kColumnWeidget()
                If column.ColumnName <> "Key_Company" And column.ColumnName <> "Key_Act" Then
                    objG.field = column.ColumnName.Replace(" ", "_").Replace("(", "").Replace(")", "")
                    objG.title = column.ColumnName
                    lstColName.Add(objG)
                End If

            Next

            ret.Column = lstColName

        Catch ex As Exception
        End Try
        Return ret
    End Function

    <System.Web.Services.WebMethod()>
    Shared Function GetSecondlevelWidget(data As String, tid As String) As kGridWeidget
        Dim ret As New kGridWeidget()
        Dim EID As Integer, DBName As String
        Dim obj As New WidgetHome()
        Dim arrItem As String()
        Dim jsonData As String = ""
        Dim arrDataChild As String()
        'Dim obj As New Widget()
        Dim ds As New DataSet()
        Dim dsQ As New DataSet()
        Dim serializerSettings As New JsonSerializerSettings()
        Try
            Dim UID = Convert.ToInt32(HttpContext.Current.Session("UID"))
            Dim Roles = Convert.ToString(HttpContext.Current.Session("UserRole"))
            EID = Convert.ToInt32(HttpContext.Current.Session("EID"))
            DBName = "MainHome"
            arrItem = data.Split("|")
            ds = obj.GetWidgets(EID, DBName, Roles, Convert.ToInt32(tid))
            If ds.Tables(0).Rows.Count > 0 Then
                'If ds.Tables(0).Rows(0).Item("SecondlevelQuery").ToString.Length > 10 Then
                '    ret.ShowAction = "yes"
                'Else
                '    ret.ShowAction = "no"
                'End If
                ret.ShowAction = "no"
                Dim Query = ds.Tables(0).Rows(0).Item("SecondlevelQuery")
                'Replacing Query with value
                Query = Query.ToString.Replace("{UID}", UID)

                For k As Integer = 0 To arrItem.Length - 1
                    arrDataChild = arrItem(k).Trim().Split("::")
                    Dim search As String = "{" & arrDataChild(0).Trim() & "}"
                    Query = Query.ToString.Replace(search.ToString(), arrDataChild(2).Trim().ToString())
                Next
                Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
                Using con = New SqlConnection(conStr)
                    Using da As New SqlDataAdapter(Query, con)
                        da.Fill(dsQ)
                    End Using
                End Using
            End If
            If dsQ.Tables(0).Rows.Count > 0 Then
                For k As Integer = 0 To dsQ.Tables(0).Columns.Count - 1
                    dsQ.Tables(0).Columns(k).ColumnName = dsQ.Tables(0).Columns(k).ColumnName.Replace(" ", "_").Replace("(", "").Replace(")", "")
                Next
                serializerSettings.Converters.Add(New DataTableConverter())
                jsonData = JsonConvert.SerializeObject(dsQ.Tables(0), Newtonsoft.Json.Formatting.None, serializerSettings)
            End If
            ret.Data = jsonData
            Dim objG As kColumnWeidget
            Dim lstColName As New List(Of kColumnWeidget)()
            For Each column As DataColumn In dsQ.Tables(0).Columns
                objG = New kColumnWeidget()
                If column.ColumnName <> "Key_Company" And column.ColumnName <> "Key_Act" Then
                    objG.field = column.ColumnName.Replace(" ", "_").Replace("(", "").Replace(")", "")
                    objG.title = column.ColumnName
                    lstColName.Add(objG)
                End If

            Next

            ret.Column = lstColName

        Catch ex As Exception
        End Try
        Return ret
    End Function

    <System.Web.Services.WebMethod()>
    Shared Function SholinkURL() As String
        Dim ret = ""
        Dim ob As New MainUtility()
        Dim timestamp As Integer = ob.DateTimeToEpoch(DateTime.UtcNow)
        Dim name As String = HttpContext.Current.Session("USERNAME")
        Dim email As String = HttpContext.Current.Session("EMAIL")
        Dim ext_id As String = ""
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim od As New SqlDataAdapter("Select * from MMM_SSO_ZENDESK WHERE EID=" & HttpContext.Current.Session("EID").ToString(), con)
        Try
            Dim dt As New DataTable
            od.Fill(dt)
            If dt.Rows.Count = 1 Then
                Dim org As String = dt.Rows(0).Item("orgName").ToString()
                Dim token As String = dt.Rows(0).Item("zKey").ToString()
                Dim link As String = dt.Rows(0).Item("zendesklink").ToString()
                Dim hash As String = ob.Md5(name & email & ext_id & org & token & timestamp)
                Dim retUrl As String = link & "name=" & name & "&email=" & email & "&external_id=" & ext_id & "&organization=" & org & "&timestamp=" & timestamp & "&hash=" & hash
                ret = retUrl
            End If
        Catch ex As Exception
        Finally
            od.Dispose()
            con.Close()
            con.Dispose()
        End Try
        Return ret
    End Function
    <System.Web.Services.WebMethod()>
    Public Shared Function GetPiChartWidget(tid As String) As pichartColWeidget
        Dim ret As New pichartColWeidget()
        Try
            Dim obj As New WidgetHome()
            Dim UID = Convert.ToInt32(HttpContext.Current.Session("UID"))
            Dim Roles = Convert.ToString(HttpContext.Current.Session("UserRole"))
            ret = obj.GetPiChartWidget(tid, UID, Roles)
        Catch ex As Exception
        End Try
        Return ret 'JsonConvert.SerializeObject(ret)
    End Function
#End Region
End Class
Public Class SortDescription
    Public field As String = ""
    Public dir As String = ""

End Class
Public Class FilterContainer
    Public Property filters() As List(Of FilterDescription)
    Public logic As String = ""
End Class

Public Class FilterDescription
    Public [operator] As String = ""
    Public field As String = ""
    Public value As String = ""

End Class
Public Class kGridHome
    Public Data As String = ""
    Public Count As String = ""
    Public total As Integer = 0
    Public Column As New List(Of kColumnHome)
End Class
Public Class kColumnHome
    Public Sub New()

    End Sub
    Public Sub New(staticfield As [String], statictitle As [String], statictype As String, staticFormat As String)
        field = staticfield
        title = statictitle
        type = statictype
        format = staticFormat
        If (statictype = "number") Then
            filterable = ""
        End If
        'width = staticwidth
    End Sub

    Public field As String = ""
    Public title As String = ""
    Public width As Integer = 200
    Public format As String = ""
    Public filterable As String = ""
    'Public locked As Boolean = True
    'Public locked As Boolean = True
    Public type As String = ""
    Public FieldID As String = ""


End Class