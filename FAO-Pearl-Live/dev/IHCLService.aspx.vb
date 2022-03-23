
Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports System.Net
Imports System.Xml


Partial Class IHCLService
    Inherits System.Web.UI.Page

    Protected Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If Len(TextBox2.Text.Trim) < 2 Then
            lblError.Text = "Enter Valid Pearl IDs First!"
            Exit Sub
        End If
        lblError.Text = ""
        GridView1.DataSource = Nothing
        GridView1.DataBind()
        IHCLWebServiceRunOnButtonClick.PearlIds = TextBox2.Text.Trim()
        Dim instance As New IHCLWebServiceRunOnButtonClick()
        'Dim response As String = instance.EntryPoint("material_rcpt_url")
        Dim response As String = instance.EntryPoint("vendor_creation")
        GridView1.DataSource = IHCLWebServiceRunOnButtonClick.ReportQueryResult
        GridView1.DataBind()
        TextBox1.Text = response
    End Sub

    Protected Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        If Len(TextBox2.Text.Trim) < 2 Then
            lblError.Text = "Enter Valid Pearl IDs First!"
            Exit Sub
        End If
        lblError.Text = ""
        GridView1.DataSource = Nothing
        GridView1.DataBind()
        IHCLWebServiceRunOnButtonClick.PearlIds = TextBox2.Text.Trim()
        Dim instance As New IHCLWebServiceRunOnButtonClick()
        Dim response As String = instance.EntryPoint("vendor_modfn")
        GridView1.DataSource = IHCLWebServiceRunOnButtonClick.ReportQueryResult
        GridView1.DataBind()
        TextBox1.Text = response
    End Sub


    Protected Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        If Len(TextBox2.Text.Trim) < 2 Then
            lblError.Text = "Enter Valid Pearl IDs First!"
            Exit Sub
        End If
        lblError.Text = ""
        GridView1.DataSource = Nothing
        GridView1.DataBind()
        IHCLWebServiceRunOnButtonClick.PearlIds = TextBox2.Text.Trim()
        Dim instance As New IHCLWebServiceRunOnButtonClick()
        Dim response As String = instance.EntryPoint("vendor_siteextension")
        GridView1.DataSource = IHCLWebServiceRunOnButtonClick.ReportQueryResult
        GridView1.DataBind()
        TextBox1.Text = response
    End Sub


    Protected Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        If Len(TextBox2.Text.Trim) < 2 Then
            lblError.Text = "Enter Valid Pearl IDs First!"
            Exit Sub
        End If
        lblError.Text = ""
        GridView1.DataSource = Nothing
        GridView1.DataBind()
        IHCLWebServiceRunOnButtonClick.PearlIds = TextBox2.Text.Trim()
        Dim instance As New IHCLWebServiceRunOnButtonClick()
        Dim response As String = instance.EntryPoint("material_rcpt_url")
        GridView1.DataSource = IHCLWebServiceRunOnButtonClick.ReportQueryResult
        GridView1.DataBind()
        TextBox1.Text = response
    End Sub

    Protected Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
        If Len(TextBox2.Text.Trim) < 2 Then
            lblError.Text = "Enter Valid Pearl IDs First!"
            Exit Sub
        End If
        lblError.Text = ""
        GridView1.DataSource = Nothing
        GridView1.DataBind()
        IHCLWebServiceRunOnButtonClick.PearlIds = TextBox2.Text.Trim()
        Dim instance As New IHCLWebServiceRunOnButtonClick()
        Dim response As String = instance.EntryPoint("inv_non_ers_crtn")
        GridView1.DataSource = IHCLWebServiceRunOnButtonClick.ReportQueryResult
        GridView1.DataBind()
        TextBox1.Text = response
    End Sub
End Class
