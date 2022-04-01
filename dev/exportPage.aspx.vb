Imports System.Collections.Generic
Imports System.Linq
Imports System.Web
Imports System.Web.UI
Imports System.Web.UI.WebControls
Partial Class exportPage
    Inherits System.Web.UI.Page

    Protected Sub Page_PreLoad(sender As Object, e As EventArgs) Handles Me.PreLoad
        Dim data As String = Request.Form("data")
        data = "<html><body><table border=""1px"">" & data & "</table></body></html>"
        data = HttpUtility.UrlDecode(data)
        Response.Clear()
        Response.AddHeader("content-disposition", "attachment;filename=Data.xls")
        Response.Charset = ""
        Response.ContentType = "application/excel"
        HttpContext.Current.Response.Write(data)
        HttpContext.Current.Response.Flush()
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
End Class
