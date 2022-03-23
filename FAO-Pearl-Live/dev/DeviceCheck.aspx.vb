Imports System.Data
Imports System.Data.SqlClient


Partial Class DeviceCheck
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        lblmsgName.Text = "Welcome To GPS Device Checking Tool"
        lblLogo.Text = "<img src=""Images/logo.gif"" alt=""Mynd Saas"" />"
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
    Protected Sub btnlogin_Click(sender As Object, e As EventArgs) Handles btnlogin.Click
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim od As SqlDataAdapter = New SqlDataAdapter("select IMIENo from MMM_MST_GPSDATA where DType=1 and IMIEno='" & txtUserID.Text & "' ", con)
        od.SelectCommand.CommandType = CommandType.Text
        Dim ds As New DataSet()
        od.Fill(ds, "code")
        If ds.Tables("code").Rows.Count > 0 Then
            lblError.Text = "Device Activated."
            btnDelete.Visible = True
        Else
            btnDelete.Visible = False
            lblError.Text = "Some Issue! Wait for Some Time"
        End If


    End Sub

    Protected Sub btnDelete_Click(sender As Object, e As EventArgs) Handles btnDelete.Click
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim od As SqlDataAdapter = New SqlDataAdapter("DELETE MMM_MST_GPSDATA where Dtype=1 and IMIEno='" & txtUserID.Text & "' ", con)
        od.SelectCommand.CommandType = CommandType.Text
        Dim ds As New DataSet()
        od.Fill(ds, "code")
        btnDelete.Visible = False
        lblError.Text = "Please Enter Another IMIE no To Test"
        txtUserID.Text = ""
    End Sub
End Class
