Imports System.Net
Imports System.IO
Imports System.Data.SqlClient
Imports System.Data

Partial Class ActionMailApproval
    Inherits System.Web.UI.Page
    Dim objDC As New DataClass()
    Dim objDC1 As New DataTable()
    Dim objDT As New DataTable
    Dim objDT1 As New DataTable
    Dim isInputthroughMail As Int32
    Dim EnableMailInput As Int32

    Dim inputfieldCaption As String = ""
    Dim inputfieldValue As String = ""
    Dim TargetMIField As String = ""
    Dim Dtype As String = ""
    Dim objEncryptionDescription As New EncryptionDescryption()
    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            lblMsg.Text = ""
            Dim Ecode As String()
            If Not IsNothing(Request.QueryString("Q1")) Then
                Dim strPathAndQuery As String = HttpContext.Current.Request.Url.PathAndQuery
                Dim strUrl As String = HttpContext.Current.Request.Url.AbsoluteUri.Replace(strPathAndQuery, "/")
                If strUrl.Contains("localhost") Or strUrl.Contains("myndsaas.myndsolution.com") Then
                    strUrl = "http://paytm.myndsaas.com/"
                    strUrl = strUrl.ToUpper.Trim()
                    strUrl = Replace(strUrl, "HTTP://", "")
                    strUrl = Replace(strUrl, "HTTPS://", "")
                    Ecode = strUrl.Split(".")
                Else
                    ' strUrl = "http://hcl.myndsaas.com/"
                    strUrl = strUrl.ToUpper.Trim()
                    strUrl = Replace(strUrl, "HTTP://", "")
                    strUrl = Replace(strUrl, "HTTPS://", "")
                    Ecode = strUrl.Split(".")
                End If
                'Dim FECode As String() = Ecode(0).ToString().Split("//")

                Dim Q1 As String = Request.QueryString("Q1").ToString().Trim()
                Q1 = Q1.Replace(" ", "+")
                Dim Q2 As String = Request.QueryString("Q2").ToString().Trim()
                Q2 = Q2.Replace(" ", "+")
                Dim Q3 As String = Request.QueryString("Q3").ToString().Trim()
                Q3 = Q3.Replace(" ", "+")
                Dim eid As Integer = objDC.ExecuteQryDT("select EID from mmm_mst_entity  with (nolock) where code ='" & Ecode(0).ToString().Trim() & "'").Rows(0)(0)

                Dim Skey As Integer = 12345  'IIf((eid * 65) < 10000, (eid * 65) * 10, (eid * 65))

                Dim DOCID As String = "0", USERID = "0", CURSTATUS = ""
                DOCID = Replace(objEncryptionDescription.DecryptTripleDES(Q1, Skey), "DOCID=", "").Trim()
                USERID = Replace(objEncryptionDescription.DecryptTripleDES(Q2, Skey), "USERID=", "").Trim()
                CURSTATUS = Replace(objEncryptionDescription.DecryptTripleDES(Q3, Skey), "CURSTATUS=", "").Trim()

                Dim isAskpwd4Appr As Int32 = 0
                hdnAction.Value = 0
                'Added By Manvendra 

                objDT = objDC.ExecuteQryDT("select DocumentType from mmm_mst_doc with (nolock) where eid =" & eid & " and tid=" & DOCID & "")
                If objDT.Rows.Count > 0 Then
                    Dtype = objDT.Rows(0).Item("DocumentType")
                    hdndoctype.Value = Dtype
                End If
                objDC1 = objDC.ExecuteQryDT("select isnull(isInputthroughMail,0)[isInputthroughMail],isnull(inputfieldCaption,'')[inputfieldCaption],isnull(inputfieldValue,'')[inputfieldValue],isnull(TargetMIField,'') [TargetMIField] FROM MMM_MST_FORMS with (nolock) where EID=" & eid & " and FormName='" & Dtype & "'")

                If objDC1.Rows.Count > 0 Then
                    isInputthroughMail = objDC1.Rows(0).Item("isInputthroughMail")
                    inputfieldCaption = objDC1.Rows(0).Item("inputfieldCaption")
                    inputfieldValue = objDC1.Rows(0).Item("inputfieldValue")
                    dvCaption.InnerHtml = inputfieldCaption
                    BindDropdown(eid, Dtype)
                    If isInputthroughMail = 1 Then
                        hdnAction.Value = 3
                    End If

                End If

                objDT1 = objDC.ExecuteQryDT("select isnull(EnableMailInput,0) [EnableMailInput] from MMM_MST_WORKFLOW_STATUS with (nolock) where documenttype='" & Dtype & "' and eid=" & eid & " and StatusName='" & CURSTATUS & "' ")

                If objDT1.Rows.Count > 0 Then
                    EnableMailInput = objDT1.Rows(0).Item("EnableMailInput")
                End If

                isAskpwd4Appr = objDC.ExecuteQryScaller("select isnull(askpwd4Appr,0) from mmm_mst_entity with (nolock) where eid=" & eid & "")

                If (isAskpwd4Appr = 0 And (EnableMailInput = 0 Or hdnAction.Value <> 3)) Then
                    hdnAction.Value = 0
                    showHidePWDMPIN()
                    loginPassword.Visible = False
                    loginMpin.Visible = False
                    thanksNote.InnerText = "Action completed successfully, You may close this browser window now!"
                    CallManualApproval(docid:=DOCID, userid:=USERID, curstatus:=CURSTATUS, eid:=eid)
                ElseIf hdnAction.Value = 3 And EnableMailInput = 1 Then
                    hdnUserID.Value = USERID
                    hdnDocID.Value = DOCID
                    hdnEID.Value = eid
                    hdnCurrStatus.Value = CURSTATUS
                    usriptmappr.Visible = True
                    loginPassword.Visible = False
                    loginMpin.Visible = False
                    happySign.Visible = False
                    sadsign.Visible = False
                ElseIf isAskpwd4Appr <> 0 Then
                    hdnAction.Value = isAskpwd4Appr
                    hdnUserID.Value = USERID
                    hdnDocID.Value = DOCID
                    hdnEID.Value = eid
                    hdnCurrStatus.Value = CURSTATUS
                    showHidePWDMPIN()
                End If
                'workign with web services in
                'result = CallServiceMethod(objEncryptionDescription.DecryptTripleDES(Q1, "0"))

            End If
        End If
    End Sub
    Private Sub showHidePWDMPIN()
        happySign.Visible = False
        sadsign.Visible = False
        If hdnAction.Value = 1 Then
            loginMpin.Visible = True
            loginPassword.Visible = False
            usriptmappr.Visible = False
            Dim ht As New Hashtable()
            ht.Add("@eid", hdnEID.Value)
            ht.Add("@Type", "PIN")
            spnPin.InnerText = objDC.ExecuteProScaller("GetMSG", ht)
        ElseIf hdnAction.Value = 2 Then
            loginPassword.Visible = True
            loginMpin.Visible = False
            usriptmappr.Visible = False
            Dim ht As New Hashtable()
            ht.Add("@eid", hdnEID.Value)
            ht.Add("@Type", "PASSWORD")
            spnPass.InnerText = objDC.ExecuteProScaller("GetMSG", ht)
        ElseIf hdnAction.Value = 3 Then
            usriptmappr.Visible = True
            loginPassword.Visible = False
            loginMpin.Visible = False
        End If
    End Sub

    Public Sub BindDropdown(eid As Int32, Documenttype As String)
        txtusriptmappr.Text = ""
        'Dim DtTable As DataTable
        'DtTable = objDC.ExecuteQryDT("select inputfieldValue from MMM_MST_FORMS where eid=" & eid & " and FormName='" & Documenttype & "'")
        'If DtTable.Rows.Count > 0 Then
        '    Dim Chkvalues As String = DtTable.Rows(0)("inputfieldValue").ToString()
        '    For Each tech As String In Chkvalues.Split(","c)
        '        Dim items As New ListItem()
        '        items.Text = tech
        '        items.Value = tech
        '        ddlusriptmappr.Items.Add(items)
        '    Next
        '    Dim newListItem As ListItem
        '    newListItem = New ListItem("Select", "0")
        '    ddlusriptmappr.Items.Add(newListItem)
        '    newListItem.Selected = True
        'End If
    End Sub


    Private Function CallManualApproval(Optional ByRef docid As Integer = 0, Optional ByRef userid As Integer = 0, Optional ByRef curstatus As String = "", Optional ByRef eid As Integer = 0, Optional ByRef qry As String = "") As String
        Dim result As String = "FAIL"
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        con.Open()
        Dim trans As SqlTransaction = Nothing
        Try
            trans = con.BeginTransaction()
            Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
            oda.SelectCommand.Transaction = trans
            Dim objdtFieldmapping As New DataTable()
            Dim ht As New Hashtable()
            Dim DocType As String = ""
            Dim CURUSER As Integer = 0
            Dim MainDoctype As String = Convert.ToString(objDC.TranExecuteQryDT("select documenttype from mmm_mst_doc  with (nolock) where tid=" & docid, con, trans).Rows(0)(0))
            Dim doccurstatus As String = ""
            ht.Add("@docid", docid)
            ht.Add("@subevent", "APPROVAL")
            objdtFieldmapping = objDC.TranExecuteProDT("uspSelectEventScreen", ht:=ht, con:=con, tran:=trans)

            Dim arryColumn As New ArrayList
            Dim arryColumnData As New ArrayList
            Dim isMandFldinActionScr As Integer = 0 '' by sunil for checking mand. field and denying mail approval if there is any one found

            For Each dr As DataRow In objdtFieldmapping.Rows
                arryColumn.Add(Convert.ToString(dr("fieldMapping")))
                '' by sunil for checking mand. field and denying mail approval if there is any one found
                If dr("isrequired") = 1 Then
                    isMandFldinActionScr = 1
                End If
                '' by sunil for checking mand. field and denying mail approval if there is any one found
            Next
            '' by sunil for checking mand. field and denying mail approval if there is any one found
            If isMandFldinActionScr = 1 Then
                sorrynote.InnerText = "This Document cannot be Approved thru Mail because there is/are mandatory field(s) to fill in this action!"
                sadsign.Visible = True
                happySign.Visible = False
                trans.Rollback()
                result = "FAIL"
                Exit Function
            End If
            '' by sunil for checking mand. field and denying mail approval if there is any one found
            If arryColumn.Count > 0 Then
                DocType = objdtFieldmapping.Rows(0)("documenttype")
                ht.Clear()
                ht.Add("@Docid", docid)
                Dim dtUser As New DataTable
                CURUSER = Convert.ToString(objDC.TranExecuteProDT("UDP_GetCurrentUser", ht, con, trans).Rows(0)(0))
                doccurstatus = Convert.ToString(objDC.TranExecuteQryDT("select curstatus from mmm_mst_doc with (nolock) where tid= " & docid, con, trans).Rows(0)(0))
                If CURUSER = userid And doccurstatus = curstatus Then
                    Dim objdtData As New DataTable()
                    objdtData = objDC.TranExecuteQryDT("select " & String.Join(",", arryColumn.ToArray()) & " from MMM_MST_DOC  with (nolock) where eid=" & eid & " and tid=" & docid & " ", con, trans)
                    For Each drcolumn As DataColumn In objdtData.Columns
                        arryColumnData.Add("'" & Convert.ToString(objdtData.Rows(0)(drcolumn)) & "'")
                    Next
                End If
            End If
            'Insert data into history table
            Dim ob As New DMSUtil()
            If arryColumn.Count > 0 And arryColumnData.Count > 0 Then
                objDC.TranExecuteQryDT("Insert Into MMM_MST_HISTORY(DOCID,documenttype,EID,Tablename,uid,uaction,adate," & String.Join(",", arryColumn.ToArray()) & ")Values(" & docid & ",'" & MainDoctype & "'," & eid & " ,'MMM_MST_DOC'," & CURUSER & ",'ACTION',GETDATE()," & String.Join(",", arryColumnData.ToArray()) & ")", con, trans)
                Dim sretMsgArr() As String
                Dim res As String = ""

                res = ob.GetNextUserFromRolematrixT(Val(docid), Val(eid), Val(CURUSER), qry, Val(CURUSER), con, trans)

                sretMsgArr = res.Split(":")
                If res = "mismatch, try again later" Then
                    sorrynote.InnerText = "Temporarily unable to approve document, Try again later or contact Portal Admin!"
                    sadsign.Visible = True
                    happySign.Visible = False
                    trans.Rollback()
                    result = "FAIL"
                    Exit Function
                End If
                If sretMsgArr(0).ToUpper() = "NO SKIP" Then
                    sorrynote.InnerText = "Next Approvar/User not found, please contact Portal Admin"
                    trans.Rollback()
                    result = "FAIL"
                    sadsign.Visible = True
                    happySign.Visible = False
                    Exit Function
                End If
                If res = "User Not Authorised" Then
                    sorrynote.InnerText = "You are not authorised to Approve this document"
                    trans.Rollback()
                    result = "FAIL"
                    sadsign.Visible = True
                    happySign.Visible = False
                    Exit Function
                End If
                If res = "Can not Approve, Reached to ARCHIVE" Then
                    sorrynote.InnerText = "Can not Approve, Reached to ARCHIVE"
                    trans.Rollback()
                    result = "FAIL"
                    sadsign.Visible = True
                    happySign.Visible = False
                    Exit Function
                End If
                If res = "Current and Next Status cannot be same" Then
                    sorrynote.InnerText = "Current and Next Status cannot be same"
                    trans.Rollback()
                    sadsign.Visible = True
                    happySign.Visible = False
                    result = "FAIL"
                    Exit Function
                End If
                If sretMsgArr(0) = "ARCHIVE" Then
                    Dim Op As New Exportdata()
                    Op.PushdataT(docid, sretMsgArr(1), eid, con, trans)
                End If
                Trigger.ExecuteTriggerT(DocType, eid, docid, con, trans, TriggerNature:="Create")
            Else
                sorrynote.InnerText = "You have already approved this document!"
                trans.Rollback()
                result = "FAIL"
                happySign.Visible = False
                sadsign.Visible = True
                Exit Function
            End If
            trans.Commit()
            objDC.ExecuteQryDT("Update mmm_doc_dtl set APPThrewMail=1 where tid in( select top 1 tid from mmm_doc_dtl with (nolock) where docid=" & docid & " and aprstatus is not null order by tid desc)")
            happySign.Visible = True
            sadsign.Visible = False
            ob.TemplateCalling(docid, eid, MainDoctype, "APPROVE")
        Catch ex As Exception
            trans.Rollback()
            sorrynote.InnerText = "Sorry! Your request can't be processed right now, Please contact Portal Admin!"
            sadsign.Visible = True
            happySign.Visible = False
        Finally
            con.Dispose()
        End Try
        Return result
    End Function


    Private Function CallServiceMethod(Optional ByRef str As String = "") As String
        Dim result As String = ""
        'Dim sURL As String = "http://myndsaas.com/MyndBPMWS.svc/DeleteDOCList"
        Dim sURL As String = "http://localhost:6631/Myndsaas.myndsolution.com%20Test/MyndBPMWS.svc/DocumentApproval"
        Dim request As HttpWebRequest = HttpWebRequest.Create(sURL)
        Dim encoding As New ASCIIEncoding()
        Dim data As Byte() = encoding.GetBytes(str)
        Dim webrequest__1 As HttpWebRequest = DirectCast(WebRequest.Create(Trim(sURL)), HttpWebRequest)

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
        Dim enc As Encoding = System.Text.Encoding.GetEncoding("utf-8")
        ' read response stream from response object
        Dim loResponseStream As New StreamReader(webresponse.GetResponseStream(), enc)
        result = loResponseStream.ReadToEnd()
        loResponseStream.Close()
        ' close the response object
        webresponse.Close()
        Dim regex As New Regex("\<[^\>]*\>")
        result = regex.Replace(result, [String].Empty)
        result = result.Replace("' ", " ")
        result = result.Replace(" '", " ")
        If (result.Contains("Your DocID is") Or result.Contains("Record updated successfully.")) Then
            result = "SUCCESS"
        ElseIf (result.Contains("RTO")) Then
            result = "FAILED"
        Else
            result = "FAILED"
        End If
        Return result
    End Function

    Protected Sub btnLogin_Click(sender As Object, e As EventArgs)
        Dim objDT As New DataTable()
        objDT = objDC.ExecuteQryDT("select pwd,skey,isauth,uid from mmm_mst_user with (nolock) where uid=" & hdnUserID.Value)
        If (objDT.Rows.Count > 0) Then
            Dim pwd As String = ""
            pwd = objEncryptionDescription.DecryptTripleDES(objDT.Rows(0)("pwd"), objDT.Rows(0)("skey"))
            If (Convert.ToString(pwd) = txtPassword.Text.Trim()) Then
                lblMsg.Text = "Successfully login!"
                thanksNote.InnerText = "Action completed successfully, You may close this browser window now!"
                CallManualApproval(docid:=hdnDocID.Value, userid:=hdnUserID.Value, curstatus:=hdnCurrStatus.Value, eid:=hdnEID.Value)
                loginPassword.Visible = False
                loginMpin.Visible = False
            Else
                lblMsg.Visible = True
                lblMsg.Text = "Please enter correct password"
            End If
        End If
    End Sub
    Protected Sub btnMpin_Click(sender As Object, e As EventArgs)
        Dim objDT As New DataTable()
        objDT = objDC.ExecuteQryDT("select mpin,mpinkey,uid from mmm_mst_user with (nolock) where uid=" & hdnUserID.Value)
        If (objDT.Rows.Count > 0) Then
            Dim pwd As String = ""
            Try
                pwd = objEncryptionDescription.DecryptTripleDES(objDT.Rows(0)("mpin"), objDT.Rows(0)("mpinkey"))
                If (Convert.ToInt32(pwd) = Convert.ToInt32(txtMpin.Text.Trim())) Then
                    lblMsg.Text = "Successfully login!"
                    thanksNote.InnerText = "Action completed successfully, You may close this browser window now!"
                    CallManualApproval(docid:=hdnDocID.Value, userid:=hdnUserID.Value, curstatus:=hdnCurrStatus.Value, eid:=hdnEID.Value)
                    loginPassword.Visible = False
                    loginMpin.Visible = False
                Else
                    lblMpinMsg.Visible = True
                    lblMpinMsg.Text = "Please enter correct pin"
                End If
            Catch ex As Exception
                lblMpinMsg.Visible = True
                lblMpinMsg.Text = "Please register pin"
            End Try

        End If
    End Sub
    'Added By Manvendra 15-01-2020
    Protected Sub btnddlusriptmappr_Click(sender As Object, e As EventArgs)
        Dim objDT As New DataTable()
        Dim tMI As String = ""
        'objDT = objDC.ExecuteQryDT("select mpin,mpinkey,uid from mmm_mst_user  where uid=" & hdnUserID.Value)
        If (txtusriptmappr.Text = "") Then
            lblusripmapprmsg.Visible = True
            lblusripmapprmsg.Text = "Please Enter any Name in text box!"

        Else
            Dim pwd As String = ""
            Try
                ' pwd = objEncryptionDescription.DecryptTripleDES(objDT.Rows(0)("mpin"), objDT.Rows(0)("mpinkey"))
                'If (Convert.ToInt32(pwd) = Convert.ToInt32(txtMpin.Text.Trim())) Then
                objDT = objDC.ExecuteQryDT("select TargetMIField  FROM MMM_MST_FORMS with (nolock) where EID=" & hdnEID.Value & " and FormName='" & hdndoctype.Value & "'")
                If objDT.Rows.Count > 0 Then
                    tMI = objDT.Rows(0).Item("TargetMIField")
                End If
                Dim qry = "Update mmm_mst_doc set " & tMI & " = '" & txtusriptmappr.Text & "' where Tid=" & hdnDocID.Value & " and eid=" & hdnEID.Value & " "
                lblMsg.Text = "Successfully login!"
                thanksNote.InnerText = "Action completed successfully, You may close this browser window now!"
                CallManualApproval(docid:=hdnDocID.Value, userid:=hdnUserID.Value, curstatus:=hdnCurrStatus.Value, eid:=hdnEID.Value, qry:=qry)
                usriptmappr.Visible = False
                loginPassword.Visible = False
                loginMpin.Visible = False

                'Else
                ' lblMpinMsg.Visible = True
                'lblMpinMsg.Text = "Please enter correct pin"
                'End If
            Catch ex As Exception
                lblusripmapprmsg.Visible = True
                lblusripmapprmsg.Text = "Somthing went Wrong !!"
            End Try

        End If
    End Sub
End Class
