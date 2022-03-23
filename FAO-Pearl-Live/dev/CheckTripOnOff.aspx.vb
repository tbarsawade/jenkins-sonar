Imports System.Data.SqlClient
Imports System.Data
Imports System.IO

Partial Class CheckTripOnOff
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
        Try
            oda.SelectCommand.CommandText = "select top 10 Imieno[IMEINO],DATEADD(minute,DATEDIFF(minute,0,ctime),0)[CTime],DATEADD(minute,DATEDIFF(minute,0,recordtime),0)[RecordTime],case tripon when 1 then 'Switch On' else 'Switch Off' end [Switch Status] from mmm_mst_gpsdata with (nolock) where imieno='" & txtIMEI.Text.ToString & "' order by DATEADD(minute,DATEDIFF(minute,0,ctime),0) desc"
            Dim ds As New DataSet
            oda.SelectCommand.CommandTimeout = 180
            oda.Fill(ds, "data")
            If ds.Tables("data").Rows.Count > 0 Then
                GVReport.DataSource = ds
                GVReport.DataBind()
                ViewState("xlexport") = ds.Tables("data")
            Else
                lblmsg.Text = "Data Not Found..."
            End If
        Catch ex As Exception
        Finally
            con.Close()
            con.Dispose()
        End Try

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

        Response.Write("<br/><div align=""center"" style=""border:1px solid red"" ><h3>" & "Switch On Off Data" & "</h3></div> <br/>")
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
End Class
