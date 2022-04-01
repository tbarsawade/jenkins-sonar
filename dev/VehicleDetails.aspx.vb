Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports System.Web.UI.Adapters.ControlAdapter
Imports iTextSharp.text
Imports System.Drawing
Imports System.Threading
Imports System.Net.Mail
Imports System.Net
Imports System.Net.HttpWebRequest
Imports System.Net.HttpWebResponse
Imports System
Imports System.Collections.Specialized
Imports System.Text
Imports System.Net.Security
Partial Class VehicleDetails
    Inherits System.Web.UI.Page
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            Dim vno As String = Request.QueryString("Vehicle No").ToString()
            Dim d1 As String = Request.QueryString("Start Date").ToString()
            Dim d2 As String = Request.QueryString("End Date").ToString
            Dim ds As New DataSet()
            Dim ConStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Using con As New SqlConnection(ConStr)
                Using da As New SqlDataAdapter("getindusLogBookDetails", con)
                    da.SelectCommand.CommandType = CommandType.StoredProcedure
                    da.SelectCommand.Parameters.Clear()
                    da.SelectCommand.Parameters.AddWithValue("@vehno", vno)
                    da.SelectCommand.Parameters.AddWithValue("@d1", d1)
                    da.SelectCommand.Parameters.AddWithValue("@d2", d2)
                    da.Fill(ds)
                End Using
            End Using
            gvData.DataSource = ds
            gvData.DataBind()
            ViewState("Data") = ds
        Catch Ex As Exception
            Throw
        End Try
    End Sub
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
    Public Overloads Overrides Sub VerifyRenderingInServerForm(ByVal control As Control)
    End Sub
    Protected Sub btnExcelExport_Click(sender As Object, e As System.Web.UI.ImageClickEventArgs) Handles btnExcelExport.Click
        gvData.DataSource = ViewState("Data")
        gvData.DataBind()
        Response.Clear()
        Response.Buffer = True
        Response.Write("<br/><div align=""center"" style=""border:1px solid red"" ><h3>VEHICLE LOG BOOK (Electronic)</h3></div> <br/>")
        Response.AddHeader("content-disposition", "attachment;filename=Trip Report.xls")
        Response.Charset = ""
        Response.ContentType = "application/vnd.ms-excel"
        Dim sw As New StringWriter()
        Dim hw As New HtmlTextWriter(sw)
        For i As Integer = 0 To gvData.Rows.Count - 1
            'Apply text style to each Row 
            gvData.Rows(i).Attributes.Add("class", "textmode")
        Next
        gvData.RenderControl(hw)
        'style to format numbers to string 
        Dim style As String = "<style> .textmode{mso-number-format:\@;}</style>"
        Response.Write(style)
        Response.Output.Write(sw.ToString())
        Response.Flush()
        Response.End()
    End Sub
End Class