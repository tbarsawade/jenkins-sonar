Imports System.Net
Imports System.Net.Mail
Imports System.Threading
Imports System.IO
Imports System.Data
Imports System.Data.SqlClient

Partial Class NewAccount
    Inherits System.Web.UI.Page

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
    Protected Sub btnlogin_Click(sender As Object, e As System.EventArgs) Handles btnlogin.Click

        If txtAcCode.Text.Length < 2 Then
            lblMsg.Text = "Please Enter Account Code"
            Exit Sub
        End If

        If txtAcName.Text.Length < 2 Then
            lblMsg.Text = "Please Enter Account Name"
            Exit Sub
        End If

        Dim localfilesystem As Integer = 0

        If rdServer.Checked Then
            localfilesystem = 1
            If txtIPAddress.Text.Length < 5 Then
                lblMsg.Text = "Please Enter File Server IP address"
                Exit Sub
            End If

            If txtServerUserID.Text.Length < 2 Then
                lblMsg.Text = "Please Enter Server User ID"
                Exit Sub
            End If

            If txtServerPWD.Text.Length < 2 Then
                lblMsg.Text = "Please Enter Server Password"
                Exit Sub
            End If
        End If

        If txtUserName.Text.Length < 5 Then
            lblMsg.Text = "Please Enter Super User Name"
            Exit Sub
        End If


        If txtEmail.Text.Length < 5 Then
            lblMsg.Text = "Please Enter Valid Email ID"
            Exit Sub
        End If

        If txtPWD.Text.Length < 2 Then
            lblMsg.Text = "Please Enter Valid Password Minimum 5 Characters. Valid Chars are [A-Z,a-z,0-9]"
            Exit Sub
        End If

        If txtPWD.Text <> txtRePwd.Text Then
            lblMsg.Text = "Password doesn't match"
            txtRePwd.Focus()
            Exit Sub
        End If

        Dim sDefFolder As String = "YES"

        If rdDefNo.Checked Then
            sDefFolder = "NO"
        End If

        Dim UserType As String = "SU"
        Dim oUser As New User()
        Dim ist As Integer = oUser.InsertNewAccount("APP", txtAcCode.Text, txtAcName.Text, txtIPAddress.Text, txtServerUserID.Text, txtServerPWD.Text, txtUserName.Text, txtEmail.Text, txtPWD.Text, localfilesystem, sDefFolder) ' , txtFactoringType.Text, txtDiscountType.Text, chkM1Registration.Checked, txtSupplierFormName.Text, txtAddress.Text, txtPAN.Text, txtContact_dtl.Text, txtGSTNStatus.Text, txtGSTN_No.Text, txtContact_Person.Text)
        If ist = 2 Then
            lblMsg.Text = "Account Already exist"
        Else
            '' new add
            Dim folder = Server.MapPath("~/DOCS/" & ist)
            If Not Directory.Exists(folder) Then
                Directory.CreateDirectory(folder)
            End If
            txtAcCode.Text = ""
            txtAcName.Text = ""
            Response.Redirect("~/success.aspx?fn=" & txtAcName.Text)
        End If
    End Sub
    '' comented email id is present or not at entity creation  'balmiki'
    'Protected Sub txtEmail_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtEmail.TextChanged
    '    Dim ouser As New User()
    '    If ouser.CheckUserIsExist(txtEmail.Text) Then
    '        lblEmailChk.Text = "<img src=""Images/av.png"" alt=""Available"" /> Available"
    '    Else
    '        lblEmailChk.Text = "<img src=""Images/notav.png"" alt=""Not Available"" /> Not Available"
    '    End If
    'End Sub

End Class
