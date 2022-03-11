
Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports System.Net
Imports System.Xml



Partial Class Lenskart
    Inherits System.Web.UI.Page


    Protected Sub Vendor_Master_Click(sender As Object, e As EventArgs) Handles Vendor_Master.Click
        Lenskartwebservicebuttononclick.PearlIds = TextBox2.Text.Trim()
        Dim instance As New Lenskartwebservicebuttononclick()
        Dim response As String = instance.EntryPoint("Vendor_Master")
        TextBox1.Text = response

    End Sub

    Protected Sub vendor_Invoice_Click(sender As Object, e As EventArgs) Handles vendor_Invoice.Click
        Lenskartwebservicebuttononclick.PearlIds = TextBox2.Text.Trim()
        Dim instance As New Lenskartwebservicebuttononclick()
        Dim response As String = instance.EntryPoint("Vendor_Invoice")
        TextBox1.Text = response
    End Sub

    Protected Sub Purchase_Requistion_Click(sender As Object, e As EventArgs) Handles Purchase_Requistion.Click
        Lenskartwebservicebuttononclick.PearlIds = TextBox2.Text.Trim()
        Dim instance As New Lenskartwebservicebuttononclick()
        Dim response As String = instance.EntryPoint("Purchase_Requistion")
        TextBox1.Text = response
    End Sub
End Class
