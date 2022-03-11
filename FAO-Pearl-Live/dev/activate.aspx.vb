Imports System.Data
Imports System.Data.SqlClient


Partial Class activate1
    Inherits System.Web.UI.Page
    Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString


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

    Protected Sub ValidateAccessString(ByRef uid As Integer, ByVal ResetlinkString As String)
        Dim validateString As String = ResetlinkString.Substring(ResetlinkString.Length - 32, 32)
        uid = ResetlinkString.Substring(0, ResetlinkString.Length - 32)
        Dim objDC As New DataClass()
        Dim count As Integer = 0
        count = objDC.ExecuteQryScaller("select count(*) from mmm_mst_user where uid=" & uid & " and ResetAccessString='" & validateString & "' and ResetFlag=1")
        If count = 0 Then
            uid = 0
            'Else
            '   objDC.ExecuteNonQuery("Update mmm_mst_user set ResetFlag=0  where uid =" & uid)
        End If
    End Sub

    Protected Sub ShowMessage(ByVal msg As String)
        Dim strconfirm As String = "<script>if(!window.confirm('" & msg.ToString() & "')){window.location.href='Default.aspx'}</script>"
        ScriptManager.RegisterStartupScript(Page, Page.GetType, "confirm", "myTestFunction('" & msg.ToString() & "');", True)
        'ScriptManager.RegisterStartupScript(Page, Page.GetType, "confirm", "myTestFunction();", True)
    End Sub
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Response.Cache.SetNoStore()
        'txtNP.Attributes.Add("autocomplete", "off")
        'txtRP.Attributes.Add("autocomplete", "off")
        Dim entity As Integer = 0
        If Not IsPostBack Then

            Dim stractivate As String = ""
            Dim con As SqlConnection = New SqlConnection(conStr)
            If Request.QueryString("UIIID").ToString() IsNot Nothing Then
                stractivate = Request.QueryString("UIIID").ToString()
            End If

            Dim uid As Integer
            If Left(stractivate, 1) = "0" Then
                ViewState("UIID") = Replace(Request.QueryString("UIIID").ToString(), "0QWERASDFZXCVYTHGNB", "")

                ValidateAccessString(uid:=uid, ResetlinkString:=Replace(Request.QueryString("UIIID").ToString(), "0QWERASDFZXCVYTHGNB", ""))
                If uid = 0 Then
                    ShowMessage("You have already reset your password !!! For security reason kindly  click again On FORGOT PASSWORD For reset password ")
                    wrap.Visible = False
                    Exit Sub
                Else
                    wrap.Visible = True
                End If
                ViewState("UIID") = uid
                Dim oda As SqlDataAdapter = New SqlDataAdapter("Select isAuth From MMM_MST_USER where uid='" & ViewState("UIID") & "'", con)
                oda.SelectCommand.CommandType = CommandType.Text
                Dim ds As New DataSet
                oda.Fill(ds, "data")
                If ds.Tables("data").Rows(0).Item("isAuth") = 1 Then
                    Response.Redirect("default.aspx")
                End If
            Else
                ValidateAccessString(uid:=uid, ResetlinkString:=Replace(Request.QueryString("UIIID").ToString(), "1QWERASDFZXCVYTHGNB", ""))
                If uid = 0 Then
                    ShowMessage("You have already reset your password !!! for security reason kindly  click again on FORGOT PASSWORD for reset password ")
                    wrap.Visible = False
                    Exit Sub
                Else
                    wrap.Visible = True
                End If
                ViewState("UIID") = uid
            End If



            Dim od1 As SqlDataAdapter = New SqlDataAdapter("Select e.EID,e.Code,e.name,e.logo from MMM_MST_ENTITY E inner join mmm_mst_user U on e.eid=u.eid where u.uid=" & ViewState("UIID") & "", con)
            od1.SelectCommand.CommandType = CommandType.Text
            Dim ds1 As New DataSet()
            od1.Fill(ds1, "code")
            If ds1.Tables("code").Rows.Count = 1 Then
                'lblMsg.Text = "Your session has expired and you have been logged off from your account. Thanks for using " & ds.Tables("code").Rows(0).Item("Name").ToString() & " System."
                lblLogo.Text = "<img src=""logo/" & ds1.Tables("code").Rows(0).Item("logo").ToString() & """ alt=""" & ds1.Tables("code").Rows(0).Item("Name").ToString() & """  />"
                ' ViewState("company") = ds.Tables("code").Rows(0).Item("Code").ToString()
            End If
            con.Close()
            ds1.Dispose()
            od1.Dispose()



            Dim od As SqlDataAdapter = New SqlDataAdapter("select E.EID,minchar ,maxchar, passType,isnull(PassHisChkCount,3) as PassHisChkCount from MMM_MST_ENTITY E inner join MMM_MST_USER U on U.EID=E.EID where U.uid='" & ViewState("UIID") & "'", con)
            od.SelectCommand.CommandType = CommandType.Text
            Dim dt As New DataSet
            od.Fill(dt, "data")
            If dt.Tables("data").Rows().Count = 1 Then
                ViewState("EID") = dt.Tables("data").Rows(0).Item("EID")
                ViewState("min") = dt.Tables("data").Rows(0).Item("minchar")
                ViewState("max") = dt.Tables("data").Rows(0).Item("maxchar")
                ViewState("passtype") = dt.Tables("data").Rows(0).Item("passType").ToString()
                If Val(dt.Tables("data").Rows(0).Item("PassHisChkCount")) < 3 Then
                    ViewState("PassHisChkCount") = "3"
                Else
                    ViewState("PassHisChkCount") = dt.Tables("data").Rows(0).Item("PassHisChkCount")
                End If
            End If

            Select Case ViewState("passtype")
                Case "ANY CHARACTER"
                    txtNP.MaxLength = ViewState("max")
                    PasswordStrength2.PreferredPasswordLength = ViewState("min")
                    txtNP.Attributes.Add("onkeyup", "javascript:var r=document.getElementById('txtNP').value.length;   if (r >= " & ViewState("min") & " && r <= " & ViewState("max") & "  ) {  document.getElementById('chkMnMxPs').checked=true;  document.getElementById('chkPassType').checked = true;  }  else { document.getElementById('chkMnMxPs').checked=false; }")


                    ' do nothing
                Case "ALPHA NUMERIC"
                    txtNP.MaxLength = ViewState("max")
                    PasswordStrength2.PreferredPasswordLength = ViewState("min")

                    PasswordStrength2.MinimumNumericCharacters = 1
                    txtNP.Attributes.Add("onkeyup", "javascript:var r=document.getElementById('txtNP').value.length;   if (r >= " & ViewState("min") & " && r <= " & ViewState("max") & "  ) {  document.getElementById('chkMnMxPs').checked=true;  }  else { document.getElementById('chkMnMxPs').checked=false; } str =document.getElementById('txtNP').value; var upperCase= new RegExp('[A-Z]'); var lowerCase= new RegExp('[a-z]'); var numbers = new RegExp('[0-9]');   if( (str.match(upperCase) && str.match(numbers)) || (str.match(lowerCase) && str.match(numbers))   ) { document.getElementById('chkPassType').checked = true }  else { document.getElementById('chkPassType').checked=false; }")
                Case "ALPHA NUMERIC WITH CAPS LETTER"
                    'must be alphanumeric with one Capital letter
                    txtNP.MaxLength = ViewState("max")
                    PasswordStrength2.PreferredPasswordLength = ViewState("min")
                    PasswordStrength2.RequiresUpperAndLowerCaseCharacters = "true"
                    PasswordStrength2.MinimumLowerCaseCharacters = "0"
                    PasswordStrength2.MinimumUpperCaseCharacters = "1"
                    PasswordStrength2.MinimumNumericCharacters = "1"
                    txtNP.Attributes.Add("onkeyup", "javascript:var r=document.getElementById('txtNP').value.length;   if (r >= " & ViewState("min") & " && r <= " & ViewState("max") & "  ) {  document.getElementById('chkMnMxPs').checked=true;  }  else { document.getElementById('chkMnMxPs').checked=false; } str =document.getElementById('txtNP').value;                     var upperCase= new RegExp('[A-Z]'); var lowerCase= new RegExp('[a-z]'); var numbers = new RegExp('[0-9]');   if( (str.match(upperCase) && str.match(numbers)) || (str.match(lowerCase) && str.match(numbers) && str.match(upperCase) )   ){ document.getElementById('chkPassType').checked = true }  else { document.getElementById('chkPassType').checked=false; }")
                Case "ALPHA NUMERIC WITH SPECIAL CHARACTER"
                    txtNP.MaxLength = ViewState("max")

                    PasswordStrength2.PreferredPasswordLength = ViewState("min")
                    PasswordStrength2.MinimumSymbolCharacters = "1"

                    PasswordStrength2.MinimumNumericCharacters = 1
                    txtNP.Attributes.Add("onkeyup", "javascript:var r=document.getElementById('txtNP').value.length;   if (r >= " & ViewState("min") & " && r <= " & ViewState("max") & "  ) {  document.getElementById('chkMnMxPs').checked=true;  }  else { document.getElementById('chkMnMxPs').checked=false; } str =document.getElementById('txtNP').value;  var upperCase= new RegExp('[A-Z]'); var lowerCase= new RegExp('[a-z]'); var numbers = new RegExp('[0-9]');   var SPE = new RegExp('[!@#$%^&*-/{}()+?.\_=]');   if( (str.match(upperCase) && str.match(numbers) && str.match(SPE) ) || (str.match(lowerCase) && str.match(numbers) && str.match(upperCase) && str.match(SPE)  )   ) { document.getElementById('chkPassType').checked = true }  else { document.getElementById('chkPassType').checked=false; }")
            End Select

            PasswordStrength2.TextStrengthDescriptions = "Not at all;Very Low compliance;Low Compliance;Average Compliance;Good Compliance;Very High Compliance;Yes"

            '5’ and maximum ‘10’ characters
            lblMinMaxChar.Text = "Password should contain minimum """ & ViewState("min") & """ and maximum """ & ViewState("max") & """ characters"
            lblPassType.Text = "Password Policy is """ & ViewState("passtype") & """"

        End If
    End Sub

    Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
        If Request.Form(txtNP.UniqueID) <> Request.Form(txtRP.UniqueID) Then
            lblMsgSave.Text = "Password' and confirm password should be equal."
            Exit Sub
        End If

        If Request.Form(txtNP.UniqueID).Length() < ViewState("min") Then
            lblmes.Text = "Password should not be less than """ & ViewState("min") & """ characters"
            Exit Sub
        End If

        If Request.Form(txtNP.UniqueID).Length() > ViewState("max") Then
            lblmes.Text = "Password shall not exceed """ & ViewState("max") & """ characters"
            Exit Sub
        End If

        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim sqlq As String
        sqlq = "select top " & ViewState("PassHisChkCount") & "  * from mmm_mst_password_history where Uid=" & ViewState("UIID") & " and eid=" & ViewState("EID") & " order by entrydate desc"
        Dim oda As SqlDataAdapter = New SqlDataAdapter(sqlq, con)
        oda.SelectCommand.CommandType = CommandType.Text
        Dim dtR As New DataTable
        oda.Fill(dtR)

        'If dtR.Rows.Count = 3 Then
        For c As Integer = 0 To dtR.Rows.Count - 1
            If Request.Form(txtNP.UniqueID) = Convert.ToString(dtR.Rows(c).Item("pwdused")) Then
                lblmes.Text = "Password must be different from last '" & ViewState("PassHisChkCount") & "' used Passwords!"
                Exit Sub
            End If
        Next
        'End If

        Dim obj As New User

        Select Case ViewState("passtype")
            Case "ANY CHARACTER"
                ' do nothing
            Case "ALPHA NUMERIC"
                If Not obj.isAlphaNumeric(Request.Form(txtNP.UniqueID)) Then
                    lblmes.Text = "Password must be alphanumeric "
                    Exit Sub
                End If
                'must be alphanumeric
            Case "ALPHA NUMERIC WITH CAPS LETTER"
                'must be alphanumeric with one Capital letter
                If Not obj.isAlphaNumericAndCapital(Request.Form(txtNP.UniqueID)) Then
                    lblmes.Text = "Password must be alphanumeric and contains atleast one capital letter "
                    Exit Sub
                End If
            Case "ALPHA NUMERIC WITH SPECIAL CHARACTER"
                If Not obj.isAlphaNumericAndSpecial(Request.Form(txtNP.UniqueID)) Then
                    lblmes.Text = "Password must be alphanumeric and must contain one special character "
                    Exit Sub
                End If
        End Select

        If obj.ActivateUser(ViewState("UIID")) = "A" Then
            Dim pwd As String
            Dim sKey As Integer
            Dim Generator As System.Random = New System.Random()
            sKey = Generator.Next(10000, 99999)
            pwd = Request.Form(txtNP.UniqueID)
            Dim strPwd As String = obj.EncryptTripleDES(pwd, sKey)

            ' Dim con As SqlConnection = New SqlConnection(conStr)
            ' Dim oda As SqlDataAdapter = New SqlDataAdapter("uspInsertPassActive", con)
            oda.SelectCommand.Parameters.Clear()
            oda.SelectCommand.CommandText = "uspInsertPassActive"
            oda.SelectCommand.CommandType = CommandType.StoredProcedure
            oda.SelectCommand.Parameters.AddWithValue("uid", Val(ViewState("UIID").ToString()))
            oda.SelectCommand.Parameters.AddWithValue("sKey", sKey)
            oda.SelectCommand.Parameters.AddWithValue("pwd", strPwd)

            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            oda.SelectCommand.ExecuteScalar()

            ' ##### for inserting new password in history ##### by sp - 29-Jan-19
            oda.SelectCommand.CommandType = CommandType.Text
            oda.SelectCommand.Parameters.Clear()
            oda.SelectCommand.CommandText = "insert into mmm_mst_password_history (eid,uid,pwdused) values (" & ViewState("EID") & "," & Val(ViewState("UIID").ToString()) & ",'" & pwd & "')"
            oda.SelectCommand.ExecuteNonQuery()
            ' ##### for inserting new password in history ##### by sp - 29-Jan-19

            con.Close()
            oda.Dispose()
            '' for sending encrypted EID as string to page
            Dim encEid As String = obj.EncryptTripleDES(ViewState("EID").ToString(), "12345")
            Response.Redirect("passwordAccepted.aspx?urlcode=" & encEid & "")
        Else
            lblMsgSave.Text = "Please Contact to super User ."
        End If
        'lblMsgSave.Visible = False
        'lblmes.Text = "Your change password request has been successfully accepted. Please click on link hereunder to log into the application"
        'lbllink.Visible = True
        'lbllink.Text = "https://hfcl.myndsaas.com/"

    End Sub

    'Public Function isAlphaNumericAndSpecial(ByVal strToCheck As String) As [Boolean]
    '    Dim isnumeric As Boolean = False
    '    Dim isOneSpecial As Boolean = False

    '    For Each c As Char In strToCheck
    '        If Char.IsNumber(c) Then
    '            isnumeric = True
    '        End If
    '        '  If System.Text.RegularExpressions.Regex.IsMatch(c, "^[a-zA-Z0-9\x20]+$") Then

    '        'If Char.IsSymbol(c) Then
    '        '    isOneSpecial = True
    '        'End If
    '    Next
    '    Dim str = "!, -, #, $, % ,& , (,), *, +, .,/,:,;, <, =,>,?, @,[,],^,_,`,{,|,},~,"
    '    Dim arr As String() = str.Split(",")
    '    For s As Integer = 0 To arr.Length
    '        Dim s1 = arr(s).Trim()
    '        If strToCheck.Contains(s1) Then
    '            isOneSpecial = True
    '            Exit For
    '        End If
    '    Next
    '    Return isnumeric And isOneSpecial
    'End Function

End Class
