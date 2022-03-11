Imports System.Data.SqlClient
Imports System.Data
Imports System.IO

Partial Class ConsolidatedLastSignal
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
        oda.SelectCommand.CommandText = "select IMIENO ,max(ctime)[Ctime],max(recordtime)[Record Time],datediff(ss,max(ctime),max(recordtime))[Diff. CTime to RecodTime(Seconds)],datediff(ss,max(ctime),max(recordtime))/60[Diff. CTime to RecodTime(Mintes)],datediff(ss,max(ctime),getdate())[Diff CTime to Current DateTime(Seconds)],datediff(ss,max(ctime),getdate())/60[Diff CTime to Current DateTime(Minutes)],count(imieno)[Count] from MMM_MST_GPSDATA where Dtype=1 AND recordtime >= '" & txtsdate.Text & " " & TxtStime.Text & "' AND recordtime <= '" & txtedate.Text & " " & txtetime.Text & "' group by IMIENO"
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
        GVReport.AllowPaging = False
        GVReport.DataBind()
        GVReport.DataSource = ViewState("xlexport")

        Response.Clear()
        Response.Buffer = True

        Response.Write("<br/><div align=""center"" style=""border:1px solid red"" ><h3>" & "Gps Signal Report" & "</h3></div> <br/>")
        Response.AddHeader("content-disposition", "attachment;filename=smsreport.xls")
        Response.Charset = ""
        Response.ContentType = "application/vnd.ms-excel"
        Dim sw As New StringWriter()
        Dim hw As New HtmlTextWriter(sw)
        For i As Integer = 0 To GVReport.Rows.Count - 1
            'Apply text style to each Row 
            GVReport.Rows(i).Attributes.Add("class", "textmode")
        Next
        GVReport.RenderControl(hw)
        'style to format numbers to string 
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
End Class
