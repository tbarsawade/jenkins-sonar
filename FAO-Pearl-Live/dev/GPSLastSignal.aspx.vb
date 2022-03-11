Imports System.Data
Imports System.Data.SqlClient
Imports System.Web.UI.Adapters.ControlAdapter
Imports iTextSharp.text
Imports System.Drawing
Imports System.IO
Partial Class GPSLastSignal
    Inherits System.Web.UI.Page
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
     
    End Sub
    Protected Sub btnshow_Click(sender As Object, e As ImageClickEventArgs) Handles btnshow.Click
        Show()
    End Sub
    Protected Sub Show()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
        Dim ds As New DataSet()
        oda.SelectCommand.CommandTimeout = 600
         
        If ddlsignal.SelectedItem.Text.ToString.ToUpper = "LAST SIGNAL REPORT" Then
            oda.SelectCommand.CommandText = "select distinct imieno[IMEI],convert(nvarchar,max(convert(date,ctime,6)),101)[Last Signal Date] from mmm_mst_gpsdata with (nolock) where imieno<>'' and imieno<>'0' group by  imieno"
        End If
        If ddlsignal.SelectedItem.Text.ToString.ToUpper = "LAST SWITCH ON REPORT" Then
            oda.SelectCommand.CommandText = "select distinct imieno[IMEI],convert(nvarchar,max(convert(date,ctime,6)),101)[Last Switch On Date] from mmm_mst_gpsdata with (nolock) where  imieno<>'' and imieno<>'0' and tripon=1 group by  imieno"
        End If
        oda.Fill(ds, "data")
        GVReport.DataSource = ds.Tables("data")
        GVReport.DataBind()
        ds.Dispose()
        oda.Dispose()
        con.Dispose()
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
    Public Overloads Overrides Sub VerifyRenderingInServerForm(ByVal control As Control)

    End Sub
    Protected Sub btnexportxl_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles btnexportxl.Click
        Show()
        GVReport.AllowPaging = False
        GVReport.DataBind()
        GVReport.DataSource = ViewState("xlexport")
        Response.Clear()
        Response.Buffer = True
        Response.Write("<br/><div align=""center"" style=""border:1px solid red"" ><h3>" & "GPSDataReport" & "</h3></div> <br/>")
        Response.AddHeader("content-disposition", "attachment;filename=" & "GPSDataReport" & ".xls")
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
