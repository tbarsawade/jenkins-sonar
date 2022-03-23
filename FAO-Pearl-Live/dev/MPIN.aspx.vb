
Imports System.Data

Partial Class MPIN
    Inherits System.Web.UI.Page
    Dim objDC As New DataClass()
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

    Protected Sub btnSave_Click(sender As Object, e As EventArgs)
        Try
            Dim encryptedMPIN = ""
            Dim decryptedMPIN = ""
            Dim ob As New User()
            If txtCMPIN.Text <> "" Then
                Dim objDT As New DataTable()
                objDT = objDC.ExecuteQryDT("select isnull(MPIN,0)MPIN,isnull(MPINKEY,0)MPINKEY from mmm_mst_user where uid=" & Session("uid"))
                If objDT.Rows.Count > 0 Then
                    decryptedMPIN = ob.DecryptTripleDES(objDT.Rows(0)("MPIN"), "0")
                    If Convert.ToInt32(decryptedMPIN.Trim()) = Convert.ToInt32(txtCMPIN.Text.Trim()) Then
                        encryptedMPIN = ob.EncryptTripleDES(txtNMPIN.Text, "0")
                        objDC.ExecuteQryDT("Update mmm_mst_user set MPIN='" & encryptedMPIN & "',MPINKey=0 where uid=" & Session("UID"))
                        lblMsgSave.Text = "MPIN successfully registered"
                        Clear()
                    Else
                        lblMsgSave.Text = "Curren MPIN did not match with our database, Please contact admin!"
                    End If
                End If
                Else


                encryptedMPIN = ob.EncryptTripleDES(txtNMPIN.Text, "0")
                objDC.ExecuteQryDT("Update mmm_mst_user set MPIN='" & encryptedMPIN & "',MPINKey=0 where uid=" & Session("UID"))
                lblMsgSave.Text = "MPIN successfully registered"
                Clear()
            End If



        Catch ex As Exception

        End Try
    End Sub
    Public Sub Clear()
        txtCMPIN.Text = ""
        txtNMPIN.Text = ""
        txtCMPIN.Text = ""
        Dim objDT As New DataTable()
        objDT = objDC.ExecuteQryDT("select isnull(MPIN,0)MPIN,isnull(MPINKEY,0)MPINKEY from mmm_mst_user where uid=" & Session("UID"))
        If objDT.Rows.Count > 0 Then
            Dim ob As New User()
            If Convert.ToString(objDT.Rows(0)("MPIN")) <> "0" Then
                CMPINROW.Visible = True
            Else
                CMPINROW.Visible = False
                txtCMPIN.Text = ""
            End If
        End If
    End Sub
    Private Sub MPIN_Load(sender As Object, e As EventArgs) Handles Me.Load
        Try
            If Not IsPostBack Then

                Dim val As String = ""
                val = objDC.ExecuteQryScaller("select isnull(IsRegisteredMpin,0)IsRegisteredMpin from mmm_mst_entity where eid=" & Session("EID"))
                If val.ToUpper = "TRUE" Or val = "1" Then
                    btnSave.Visible = True
                Else
                    btnSave.Visible = False
                    lblMsgSave.Text = "Please configure MPIN setting"
                End If
                Dim objDT As New DataTable()
                objDT = objDC.ExecuteQryDT("select isnull(MPIN,0)MPIN,isnull(MPINKEY,0)MPINKEY from mmm_mst_user where uid=" & Session("UID"))
                If objDT.Rows.Count > 0 Then
                    Dim ob As New User()
                    If Convert.ToString(objDT.Rows(0)("MPIN")) <> "0" Then
                        CMPINROW.Visible = True
                    Else
                        CMPINROW.Visible = False
                        txtCMPIN.Text = ""
                    End If
                End If
            End If
        Catch ex As Exception

        End Try

    End Sub
End Class
