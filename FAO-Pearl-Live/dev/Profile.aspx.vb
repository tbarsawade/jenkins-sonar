Imports System.Data
Imports System.Data.SqlClient

Partial Class Profile
    Inherits System.Web.UI.Page
    Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
    Dim objDC As New DataClass()

    Protected Sub btnSave_Click(sender As Object, e As System.EventArgs)

        If txtNP.Text <> txtRP.Text Then
            lblMsgSave.Text = "New Password Doesn't Match"
            txtNP.Text = ""
            txtRP.Text = ""
            Exit Sub
        End If

        Dim ob As New User()
        lblMsgSave.Text = ob.ChangePassword(Val(Session("UID").ToString()), txtCP.Text, txtNP.Text)
    End Sub
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
    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            lblImage.Text = "<img src=""logo/" & Session("USERIMAGE").ToString() & """ height=""150px"" width=""150px""  alt=""Me"" />"

            Dim conn As SqlConnection = New SqlConnection(conStr)
            Dim odop As SqlDataAdapter = New SqlDataAdapter("select minchar, maxchar,passtype,isnull(IsRegisteredMpin,0)IsRegisteredMpin from MMM_MST_ENTITY where EID='" & Session("EID") & "'", conn)
            odop.SelectCommand.CommandType = CommandType.Text
            Dim dt As New DataSet
            odop.Fill(dt, "data")
            If dt.Tables("data").Rows().Count = 1 Then
                Dim min As Integer = dt.Tables("data").Rows(0).Item("minchar")
                Dim max As Integer = dt.Tables("data").Rows(0).Item("maxchar")
                Dim passtype As String = dt.Tables("data").Rows(0).Item("passType").ToString()
                lblMinMaxChar.Text = " Password should contain minimum '" & min & "' and maximum '" & max & "' charachter"
                lblPassType.Text = "Password should '" & passtype & "'"
                If dt.Tables("data").Rows(0).Item("IsRegisteredMpin") = "1" Then
                    changePin.Visible = True
                Else
                    changePin.Visible = False
                End If
            End If



            Dim con As SqlConnection = New SqlConnection(conStr)
            Dim oda As SqlDataAdapter = New SqlDataAdapter("select locID,locationName from MMM_MST_LOCATION where EID='" & Session("EID").ToString() & "' ", con)
            Dim ds As New DataSet()
            oda.Fill(ds, "data")
            ddlLocationName.DataSource = ds.Tables("data")
            ddlLocationName.DataTextField = "locationName"
            ddlLocationName.DataValueField = "locID"
            ddlLocationName.DataBind()

            Dim od As SqlDataAdapter = New SqlDataAdapter("select timeFormat From MMM_MST_LOCATION where EID='" & Session("EID").ToString() & "'", con)
            od.SelectCommand.CommandType = CommandType.Text
            od.Fill(ds, "format")
            If ds.Tables("format").Rows(0).Item("timeFormat") = 24 Then

                ddlFrom.Items.Insert(0, "00:00 ")
                ddlFrom.Items.Insert(1, "00:30 ")
                ddlFrom.Items.Insert(2, "01:00 ")
                ddlFrom.Items.Insert(3, "01:30 ")
                ddlFrom.Items.Insert(4, "02:00 ")
                ddlFrom.Items.Insert(5, "02:30 ")
                ddlFrom.Items.Insert(6, "03:00 ")
                ddlFrom.Items.Insert(7, "03:30 ")
                ddlFrom.Items.Insert(8, "04:00 ")
                ddlFrom.Items.Insert(9, "04:30 ")
                ddlFrom.Items.Insert(10, "05:00 ")
                ddlFrom.Items.Insert(11, "05:30 ")
                ddlFrom.Items.Insert(12, "06:00 ")
                ddlFrom.Items.Insert(13, "06:30 ")
                ddlFrom.Items.Insert(14, "07:00 ")
                ddlFrom.Items.Insert(15, "07:30 ")
                ddlFrom.Items.Insert(16, "08:00 ")
                ddlFrom.Items.Insert(17, "08:30 ")
                ddlFrom.Items.Insert(18, "09:00 ")
                ddlFrom.Items.Insert(19, "09:30 ")
                ddlFrom.Items.Insert(20, "10:00 ")
                ddlFrom.Items.Insert(21, "10:30 ")
                ddlFrom.Items.Insert(22, "11:00 ")
                ddlFrom.Items.Insert(23, "11:30 ")
                ddlFrom.Items.Insert(24, "12:00 ")
                ddlFrom.Items.Insert(25, "12:30 ")
                ddlFrom.Items.Insert(26, "13:00 ")
                ddlFrom.Items.Insert(27, "13:30 ")
                ddlFrom.Items.Insert(28, "14:00 ")
                ddlFrom.Items.Insert(29, "14:30 ")
                ddlFrom.Items.Insert(30, "15:00 ")
                ddlFrom.Items.Insert(31, "15:30 ")
                ddlFrom.Items.Insert(32, "16:00 ")
                ddlFrom.Items.Insert(33, "16:30 ")
                ddlFrom.Items.Insert(34, "17:00 ")
                ddlFrom.Items.Insert(35, "17:30 ")
                ddlFrom.Items.Insert(36, "18:00 ")
                ddlFrom.Items.Insert(37, "18:30 ")
                ddlFrom.Items.Insert(38, "19:00 ")
                ddlFrom.Items.Insert(39, "19:30 ")
                ddlFrom.Items.Insert(40, "20:00 ")
                ddlFrom.Items.Insert(41, "20:30 ")
                ddlFrom.Items.Insert(42, "21:00 ")
                ddlFrom.Items.Insert(43, "21:30 ")
                ddlFrom.Items.Insert(44, "22:00 ")
                ddlFrom.Items.Insert(45, "22:30 ")
                ddlFrom.Items.Insert(46, "23:00 ")
                ddlFrom.Items.Insert(47, "23:30 ")

                ddlTo.Items.Insert(0, "00:00 ")
                ddlTo.Items.Insert(1, "00:30 ")
                ddlTo.Items.Insert(2, "01:00 ")
                ddlTo.Items.Insert(3, "01:30 ")
                ddlTo.Items.Insert(4, "02:00 ")
                ddlTo.Items.Insert(5, "02:30 ")
                ddlTo.Items.Insert(6, "03:00 ")
                ddlTo.Items.Insert(7, "03:30 ")
                ddlTo.Items.Insert(8, "04:00 ")
                ddlTo.Items.Insert(9, "04:30 ")
                ddlTo.Items.Insert(10, "05:00 ")
                ddlTo.Items.Insert(11, "05:30 ")
                ddlTo.Items.Insert(12, "06:00 ")
                ddlTo.Items.Insert(13, "06:30 ")
                ddlTo.Items.Insert(14, "07:00 ")
                ddlTo.Items.Insert(15, "07:30 ")
                ddlTo.Items.Insert(16, "08:00 ")
                ddlTo.Items.Insert(17, "08:30 ")
                ddlTo.Items.Insert(18, "09:00 ")
                ddlTo.Items.Insert(19, "09:30 ")
                ddlTo.Items.Insert(20, "10:00 ")
                ddlTo.Items.Insert(21, "10:30 ")
                ddlTo.Items.Insert(22, "11:00 ")
                ddlTo.Items.Insert(23, "11:30 ")
                ddlTo.Items.Insert(24, "12:00 ")
                ddlTo.Items.Insert(25, "12:30 ")
                ddlTo.Items.Insert(26, "13:00 ")
                ddlTo.Items.Insert(27, "13:30 ")
                ddlTo.Items.Insert(28, "14:00 ")
                ddlTo.Items.Insert(29, "14:30 ")
                ddlTo.Items.Insert(30, "15:00 ")
                ddlTo.Items.Insert(31, "15:30 ")
                ddlTo.Items.Insert(32, "16:00 ")
                ddlTo.Items.Insert(33, "16:30 ")
                ddlTo.Items.Insert(34, "17:00 ")
                ddlTo.Items.Insert(35, "17:30 ")
                ddlTo.Items.Insert(36, "18:00 ")
                ddlTo.Items.Insert(37, "18:30 ")
                ddlTo.Items.Insert(38, "19:00 ")
                ddlTo.Items.Insert(39, "19:30 ")
                ddlTo.Items.Insert(40, "20:00 ")
                ddlTo.Items.Insert(41, "20:30 ")
                ddlTo.Items.Insert(42, "21:00 ")
                ddlTo.Items.Insert(43, "21:30 ")
                ddlTo.Items.Insert(44, "22:00 ")
                ddlTo.Items.Insert(45, "22:30 ")
                ddlTo.Items.Insert(46, "23:00 ")
                ddlTo.Items.Insert(47, "23:30 ")

                ddlFrom.DataBind()
                ddlTo.DataBind()

            Else

                ddlFrom.Items.Insert(0, "00:00 ")
                ddlFrom.Items.Insert(1, "00:30 AM ")
                ddlFrom.Items.Insert(2, "01:00 AM  ")
                ddlFrom.Items.Insert(3, "01:30 AM  ")
                ddlFrom.Items.Insert(4, "02:00 AM  ")
                ddlFrom.Items.Insert(5, "02:30 AM  ")
                ddlFrom.Items.Insert(6, "03:00 AM  ")
                ddlFrom.Items.Insert(7, "03:30 AM  ")
                ddlFrom.Items.Insert(8, "04:00 AM  ")
                ddlFrom.Items.Insert(9, "04:30 AM  ")
                ddlFrom.Items.Insert(10, "05:00 AM  ")
                ddlFrom.Items.Insert(11, "05:30 AM  ")
                ddlFrom.Items.Insert(12, "06:00 AM  ")
                ddlFrom.Items.Insert(13, "06:30 AM  ")
                ddlFrom.Items.Insert(14, "07:00 AM  ")
                ddlFrom.Items.Insert(15, "07:30 AM  ")
                ddlFrom.Items.Insert(16, "08:00 AM  ")
                ddlFrom.Items.Insert(17, "08:30 AM  ")
                ddlFrom.Items.Insert(18, "09:00 AM  ")
                ddlFrom.Items.Insert(19, "09:30 AM  ")
                ddlFrom.Items.Insert(20, "10:00 AM   ")
                ddlFrom.Items.Insert(21, "10:30 AM  ")
                ddlFrom.Items.Insert(22, "11:00 AM  ")
                ddlFrom.Items.Insert(23, "11:30 AM  ")
                ddlFrom.Items.Insert(24, "12:00 PM  ")
                ddlFrom.Items.Insert(25, "12:30 PM  ")
                ddlFrom.Items.Insert(26, "01:00 PM  ")
                ddlFrom.Items.Insert(27, "01:30 PM  ")
                ddlFrom.Items.Insert(28, "02:00 PM  ")
                ddlFrom.Items.Insert(29, "02:30 PM  ")
                ddlFrom.Items.Insert(30, "03:00 PM  ")
                ddlFrom.Items.Insert(31, "03:30 PM  ")
                ddlFrom.Items.Insert(32, "04:00 PM  ")
                ddlFrom.Items.Insert(33, "04:30 PM  ")
                ddlFrom.Items.Insert(34, "05:00 PM  ")
                ddlFrom.Items.Insert(35, "05:30 PM  ")
                ddlFrom.Items.Insert(36, "06:00 PM  ")
                ddlFrom.Items.Insert(37, "06:30 PM  ")
                ddlFrom.Items.Insert(38, "07:00 PM  ")
                ddlFrom.Items.Insert(39, "07:30 PM  ")
                ddlFrom.Items.Insert(40, "08:00 PM  ")
                ddlFrom.Items.Insert(41, "08:30 PM  ")
                ddlFrom.Items.Insert(42, "09:00 PM  ")
                ddlFrom.Items.Insert(43, "09:30 PM  ")
                ddlFrom.Items.Insert(44, "10:00 PM  ")
                ddlFrom.Items.Insert(45, "10:30 PM  ")
                ddlFrom.Items.Insert(46, "11:00 PM  ")
                ddlFrom.Items.Insert(47, "11:30 PM  ")

                ddlTo.Items.Insert(0, "00:00   ")
                ddlTo.Items.Insert(1, "00:30 AM ")
                ddlTo.Items.Insert(2, "01:00 AM  ")
                ddlTo.Items.Insert(3, "01:30 AM  ")
                ddlTo.Items.Insert(4, "02:00 AM  ")
                ddlTo.Items.Insert(5, "02:30 AM  ")
                ddlTo.Items.Insert(6, "03:00 AM  ")
                ddlTo.Items.Insert(7, "03:30 AM  ")
                ddlTo.Items.Insert(8, "04:00 AM  ")
                ddlTo.Items.Insert(9, "04:30 AM  ")
                ddlTo.Items.Insert(10, "05:00 AM  ")
                ddlTo.Items.Insert(11, "05:30 AM  ")
                ddlTo.Items.Insert(12, "06:00 AM  ")
                ddlTo.Items.Insert(13, "06:30 AM  ")
                ddlTo.Items.Insert(14, "07:00 AM  ")
                ddlTo.Items.Insert(15, "07:30 AM  ")
                ddlTo.Items.Insert(16, "08:00 AM  ")
                ddlTo.Items.Insert(17, "08:30 AM  ")
                ddlTo.Items.Insert(18, "09:00 AM  ")
                ddlTo.Items.Insert(19, "09:30 AM  ")
                ddlTo.Items.Insert(20, "10:00 AM   ")
                ddlTo.Items.Insert(21, "10:30 AM  ")
                ddlTo.Items.Insert(22, "11:00 AM  ")
                ddlTo.Items.Insert(23, "11:30 AM  ")
                ddlTo.Items.Insert(24, "12:00 PM  ")
                ddlTo.Items.Insert(25, "12:30 PM  ")
                ddlTo.Items.Insert(26, "01:00 PM  ")
                ddlTo.Items.Insert(27, "01:30 PM  ")
                ddlTo.Items.Insert(28, "02:00 PM  ")
                ddlTo.Items.Insert(29, "02:30 PM  ")
                ddlTo.Items.Insert(30, "03:00 PM  ")
                ddlTo.Items.Insert(31, "03:30 PM  ")
                ddlTo.Items.Insert(32, "04:00 PM  ")
                ddlTo.Items.Insert(33, "04:30 PM  ")
                ddlTo.Items.Insert(34, "05:00 PM  ")
                ddlTo.Items.Insert(35, "05:30 PM  ")
                ddlTo.Items.Insert(36, "06:00 PM  ")
                ddlTo.Items.Insert(37, "06:30 PM  ")
                ddlTo.Items.Insert(38, "07:00 PM  ")
                ddlTo.Items.Insert(39, "07:30 PM  ")
                ddlTo.Items.Insert(40, "08:00 PM  ")
                ddlTo.Items.Insert(41, "08:30 PM  ")
                ddlTo.Items.Insert(42, "09:00 PM  ")
                ddlTo.Items.Insert(43, "09:30 PM  ")
                ddlTo.Items.Insert(44, "10:00 PM  ")
                ddlTo.Items.Insert(45, "10:30 PM  ")
                ddlTo.Items.Insert(46, "11:00 PM  ")
                ddlTo.Items.Insert(47, "11:30 PM  ")
                ddlTo.DataBind()
                ddlTo.DataBind()



            End If

            Dim val As String = ""
            val = objDC.ExecuteQryScaller("select isnull(IsRegisteredMpin,0)IsRegisteredMpin from mmm_mst_entity where eid=" & Session("EID"))
            If val.ToUpper = "TRUE" Or val = "1" Then
                btnSaveMpin.Visible = True
            Else
                btnSaveMpin.Visible = False
                lblMpinMsg.Text = "Please configure MPIN setting"
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


            oda.Dispose()
            con.Close()
            ds.Dispose()

        End If
    End Sub
    Protected Sub btnSaveMpin_Click(sender As Object, e As EventArgs)
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
                        lblMpinMsg.Text = "MPIN successfully registered"
                        Clear()
                    Else
                        lblMpinMsg.Text = "Curren MPIN did not match with our database, Please contact admin!"
                    End If
                End If
            Else


                encryptedMPIN = ob.EncryptTripleDES(txtNMPIN.Text, "0")
                objDC.ExecuteQryDT("Update mmm_mst_user set MPIN='" & encryptedMPIN & "',MPINKey=0 where uid=" & Session("UID"))
                lblMpinMsg.Text = "MPIN successfully registered"
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

End Class
