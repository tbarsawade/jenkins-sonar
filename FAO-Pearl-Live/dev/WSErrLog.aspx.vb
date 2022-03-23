Imports System.Data.SqlClient
Imports System.Data
Imports System.IO

Partial Class WSErrLog
    Inherits System.Web.UI.Page
    Protected Sub btnshow_Click(sender As Object, e As ImageClickEventArgs) Handles btnshow.Click
        Show()
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
    Protected Sub Show()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
        Dim dt As New DataTable
        Dim str As String = Nothing

        If ddlSuccess.SelectedItem.Text IsNot "-Select-" Then
            str = ddlSuccess.SelectedItem.Text
        End If

        oda.SelectCommand.CommandType = CommandType.StoredProcedure
        oda.SelectCommand.CommandText = "USP_ErrorLog"
        oda.SelectCommand.Parameters.AddWithValue("@From", txtsdate.Text.Trim())
        oda.SelectCommand.Parameters.AddWithValue("@to", txtedate.Text.Trim())
        oda.SelectCommand.Parameters.AddWithValue("@ResultFlag", str)
        oda.SelectCommand.Parameters.AddWithValue("@Option", 0)
        Dim ds As New DataSet
        oda.SelectCommand.CommandTimeout = 3000
        oda.Fill(ds, "data")
        GVReport.DataSource = ds
        GVReport.DataBind()
        ViewState("xlexport") = ds.Tables("data")
        oda.Dispose()
        con.Dispose()

    End Sub
    Public Overloads Overrides Sub VerifyRenderingInServerForm(ByVal control As Control)

    End Sub

    Protected Sub btnexportxl_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles btnexportxl.Click
        Show()
        Dim gv As New GridView()

        gv.DataSource = ViewState("xlexport")
        gv.DataBind()

        Response.Clear()
        Response.Buffer = True

        Response.Write("<br/><div align=""Right"" style=""border:1px solid red"" ><h3>" & "WS Error Log Report" & "</h3></div> <br/>")
        Response.AddHeader("content-disposition", "attachment;filename=wsErrLogreport.xls")
        Response.Charset = ""
        Response.ContentType = "application/vnd.ms-excel"
        Dim sw As New StringWriter()
        Dim hw As New HtmlTextWriter(sw)
        For i As Integer = 0 To gv.Rows.Count - 1
            'Apply text style to each Row 
            gv.Rows(i).Attributes.Add("class", "textmode")
        Next
        gv.RenderControl(hw)
        ''style to format numbers to string 
        Dim style As String = "<style> .textmode{mso-number-format:\@;}</style>"
        Response.Write(style)
        Response.Output.Write(sw.ToString())
        Response.Flush()
        Response.End()


    End Sub

    Protected Sub GVReport_PageIndexChanged(sender As Object, e As System.EventArgs) Handles GVReport.PageIndexChanged
        Show()
    End Sub


    Protected Sub GVReport_PageIndexChanging(sender As Object, e As System.Web.UI.WebControls.GridViewPageEventArgs) Handles GVReport.PageIndexChanging
        GVReport.PageIndex = e.NewPageIndex
    End Sub

    

    Protected Sub GVReport_RowDataBound(sender As Object, e As GridViewRowEventArgs) Handles GVReport.RowDataBound
        For i As Integer = 0 To e.Row.Cells.Count - 1
            ViewState("OrigData") = e.Row.Cells(i).Text
            If e.Row.Cells(i).Text.Length >= 30 Then
                e.Row.Cells(i).Text = e.Row.Cells(i).Text.Substring(0, 30) + "..."
                e.Row.Cells(i).ToolTip = ViewState("OrigData").ToString()
                e.Row.Cells(i).Wrap = True
            End If
        Next

    End Sub

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load

    End Sub

    Protected Sub GVReport_SelectedIndexChanged(sender As Object, e As EventArgs) Handles GVReport.SelectedIndexChanged

    End Sub
End Class
