
Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports System.Net
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq


Partial Class VLCCOutWard
    Inherits System.Web.UI.Page
    Protected Sub Vendor_Registration_Click(sender As Object, e As EventArgs) Handles Vendor_Registration.Click
        VLCCWebServiceButtonClick.PearlIds = TextBox2.Text.Trim()
        Dim instance As New VLCCWebServiceButtonClick()
        Dim response As String = instance.EntryPoint("Vendor_Registration")
        TextBox1.Text = response

    End Sub
    Protected Sub GRN_Invoice_Click(sender As Object, e As EventArgs) Handles GRN_Invoice.Click
        VLCCWebServiceButtonClick.PearlIds = TextBox2.Text.Trim()
        Dim instance As New VLCCWebServiceButtonClick()
        Dim response As String = instance.EntryPoint("GRN_Invoice")
        TextBox1.Text = response

    End Sub

    Protected Sub Vendor_Modification_Click(sender As Object, e As EventArgs) Handles Vendor_Modification.Click
        VLCCWebServiceButtonClick.PearlIds = TextBox2.Text.Trim()
        Dim instance As New VLCCWebServiceButtonClick()
        Dim response As String = instance.EntryPoint("Vendor_Modification")
        TextBox1.Text = response

    End Sub

    Protected Sub purchase_Invoice_Click(sender As Object, e As EventArgs) Handles purchase_Invoice.Click
        VLCCWebServiceButtonClick.PearlIds = TextBox2.Text.Trim()
        Dim instance As New VLCCWebServiceButtonClick()
        Dim response As String = instance.EntryPoint("purchase_Invoice")
        TextBox1.Text = response

    End Sub

    Protected Sub purchase_Invoice_NON_PO_Click(sender As Object, e As EventArgs) Handles purchase_Invoice_NON_PO.Click
        VLCCWebServiceButtonClick.PearlIds = TextBox2.Text.Trim()
        Dim instance As New VLCCWebServiceButtonClick()
        Dim response As String = instance.EntryPoint("Invoice_Non_PO")
        TextBox1.Text = response

    End Sub
End Class
