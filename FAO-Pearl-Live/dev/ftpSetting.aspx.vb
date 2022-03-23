Imports System.Data
Imports System.Data.SqlClient

Partial Class ftpSetting
    Inherits System.Web.UI.Page

    Protected Sub btnlogin_Click(sender As Object, e As System.EventArgs) Handles btnlogin.Click
        If txtIPAddress.Text.Length < 7 Then
            lblMsg.Text = "Please Enter Valid IP Address of Server"
            Exit Sub
        End If

        If txtServerUserID.Text.Length < 3 Then
            lblMsg.Text = "Please Enter Valid Server user ID"
            Exit Sub
        End If

        If txtServerPWD.Text.Length < 3 Then
            lblMsg.Text = "Please Enter Valid Server Password"
            Exit Sub
        End If

        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("UPDATE MMM_MST_ENTITY SET ipaddress='" & txtIPAddress.Text & "',UserID='" & txtServerUserID.Text & "',pwd='" & txtServerPWD.Text & "' Where EID=" & Session("EID").ToString(), con)
        If con.State <> ConnectionState.Open Then
            con.Open()
        End If
        oda.SelectCommand.ExecuteNonQuery()
        con.Close()
        oda.Dispose()
        con.Dispose()
        lblMsg.Text = "Server Details Updated"
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
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As SqlConnection = New SqlConnection(conStr)
            Dim oda As SqlDataAdapter = New SqlDataAdapter("SELECT ipaddress,UserID,pwd  FROM MMM_MST_ENTITY WHERE EID=" & Session("EID").ToString(), con)
            Dim ds As New DataSet
            oda.Fill(ds, "data")
            txtIPAddress.Text = ds.Tables("data").Rows(0).Item("ipaddress").ToString()
            txtServerUserID.Text = ds.Tables("data").Rows(0).Item("userid").ToString()
            txtServerPWD.Text = ds.Tables("data").Rows(0).Item("pwd").ToString()
        End If
    End Sub
End Class
