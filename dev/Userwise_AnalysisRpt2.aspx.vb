Imports Newtonsoft.Json.Converters
Imports System.IO
Imports iTextSharp.text
Imports iTextSharp.text.html.simpleparser
Imports iTextSharp.text.pdf
Imports Newtonsoft.Json
Imports System.Data.SqlClient
Imports System.Data
Imports System.Web.Services
Imports System.Web.Script.Serialization
Partial Class Userwise_AnalysisRpt2
    Inherits System.Web.UI.Page
    Shared dsstatic As DataSet = New DataSet()
    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not (IsPostBack) Then
            FillUserDDL()
        End If

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
    <WebMethod()> _
    Public Shared Function GetReportData(ByVal Startdate As DateTime, ByVal Enddate As DateTime, ByVal UID As Integer, ByVal Userrole As String, ByVal UserID As Integer) As clsHFCLReport2
        dsstatic.Tables.Clear()
        Dim ret = ""
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)

        Dim dt As New DataTable
        Dim objcls As clsHFCLReport2 = New clsHFCLReport2()

        Try

            If (Convert.ToString(Startdate) = "") Then
                objcls.ErrMessage = "Please enter Start Date"
            ElseIf (Convert.ToString(Enddate) = "") Then
                objcls.ErrMessage = "Please enter End Date"

            End If


            If (objcls.ErrMessage = "") Then
                oda.SelectCommand.CommandType = CommandType.StoredProcedure
                oda.SelectCommand.CommandText = "Sp_Userwise_AnalysisRpt_HFCL"
                oda.SelectCommand.Parameters.AddWithValue("@StartDate", Convert.ToDateTime(Startdate))
                oda.SelectCommand.Parameters.AddWithValue("@EndDate", Convert.ToDateTime(Enddate))
                oda.SelectCommand.Parameters.AddWithValue("@UID", UID)
                oda.SelectCommand.Parameters.AddWithValue("@UserRole", Userrole)
                oda.SelectCommand.Parameters.AddWithValue("@UserId", UserID)
                oda.SelectCommand.CommandTimeout = 900
                oda.Fill(dt)
                dsstatic.Tables.Add(dt)
                If dt.Rows.Count > 0 Then
                    For j As Integer = 0 To dt.Columns.Count - 1
                        dt.Columns(j).ColumnName = dt.Columns(j).ColumnName.Replace(" ", "_")
                    Next
                Else : objcls.ErrMessage = "No Record Found!"
                End If
                Dim serializerSettings As New JsonSerializerSettings()
                Dim json_serializer As New JavaScriptSerializer()
                serializerSettings.Converters.Add(New DataTableConverter())
                Dim jsonData As [String] = JsonConvert.SerializeObject(dt, Formatting.None, serializerSettings)
                objcls.Data = jsonData
                objcls.Success = True
            Else
                objcls.Data = ""
                objcls.Success = False
            End If


        Catch ex As Exception
            objcls.Data = ""
            objcls.ErrMessage = "Error Occured"
            objcls.Success = False

            Throw
        Finally
            con.Dispose()
            oda.Dispose()
        End Try



        Return objcls
    End Function

    Public Class clsHFCLReport2

        Public Data As String = ""
        Public Success As Boolean
        Public ErrMessage As String = ""

    End Class


    Protected Sub btnExportPDF_Click(sender As Object, e As ImageClickEventArgs) Handles btnExportPDF.Click

        If (dsstatic.Tables.Count = 0) Then
            lblmsg.Text = "No record found to export."
        ElseIf (dsstatic.Tables(0).Rows.Count = 0) Then
            lblmsg.Text = "No record found to export."
        Else
            For c As Integer = 0 To dsstatic.Tables(0).Columns.Count - 1
                dsstatic.Tables(0).Columns(c).ColumnName = dsstatic.Tables(0).Columns(c).ColumnName.Replace("_", " ")
            Next

            ToPdf(dsstatic)

        End If

    End Sub
    Protected Sub ToPdf(ByVal newDataSet As DataSet)
        Try


            Dim GridView1 As New GridView()
            GridView1.AllowPaging = False
            GridView1.DataSource = dsstatic
            GridView1.DataBind()

            Response.ContentType = "application/pdf"
            Response.AddHeader("content-disposition", _
                   "attachment;filename=Userwise_AnalysisRpt2.pdf")
            Response.Cache.SetCacheability(HttpCacheability.NoCache)
            Dim sw As New StringWriter()
            Dim hw As New HtmlTextWriter(sw)
            GridView1.RenderControl(hw)
            Dim sr As New StringReader(sw.ToString())
            Dim pdfDoc As New Document(PageSize.A4, 10.0F, 10.0F, 10.0F, 0.0F)
            Dim htmlparser As New HTMLWorker(pdfDoc)
            PdfWriter.GetInstance(pdfDoc, Response.OutputStream)
            pdfDoc.Open()
            htmlparser.Parse(sr)
            pdfDoc.Close()
            Response.Write(pdfDoc)
            Response.End()
        Catch ex As Exception
        Finally
        End Try
    End Sub

    Protected Sub btnexportxl_Click(sender As Object, e As ImageClickEventArgs) Handles btnexportxl.Click
        If (dsstatic.Tables.Count = 0) Then
            lblmsg.Text = "No record found to export."
        ElseIf (dsstatic.Tables(0).Rows.Count = 0) Then
            lblmsg.Text = "No record found to export."
        Else
            For c As Integer = 0 To dsstatic.Tables(0).Columns.Count - 1
                dsstatic.Tables(0).Columns(c).ColumnName = dsstatic.Tables(0).Columns(c).ColumnName.Replace("_", " ")
            Next
            Dim GridView1 As New GridView()
            GridView1.AllowPaging = False
            GridView1.DataSource = dsstatic
            GridView1.DataBind()

            Response.Clear()
            Response.Buffer = True
            Response.AddHeader("content-disposition", _
                 "attachment;filename=Userwise_AnalysisRpt2.xls")
            Response.Charset = ""
            Response.ContentType = "application/vnd.ms-excel"
            Dim sw As New StringWriter()
            Dim hw As New HtmlTextWriter(sw)

            For i As Integer = 0 To GridView1.Rows.Count - 1
                'Apply text style to each Row
                GridView1.Rows(i).Attributes.Add("class", "textmode")
            Next

            GridView1.RenderControl(hw)
            'style to format numbers to string
            Dim style As String = "<style> .textmode{mso-number-format:\@;}</style>"
            Response.Write(style)
            Response.Output.Write(sw.ToString())
            Response.Flush()
            Response.End()
        End If
    End Sub

    Sub FillUserDDL()

        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
        Dim dt As New DataTable
        Dim dtusers As New DataTable
        Dim UserRole = Session("USERROLE")
        con.Close()
        If (UserRole.ToString().ToUpper() = "USER") Then
            oda.SelectCommand.CommandType = CommandType.Text
            oda.SelectCommand.CommandText = "select UID, UserName from mmm_mst_user where eid =" & Session("EID") & " and isauth=1  and uid =" & Session("UID")
            oda.Fill(dtusers)

        Else
            If (UserRole <> "SU") Then
                oda.SelectCommand.CommandType = CommandType.StoredProcedure
                oda.SelectCommand.CommandText = "uspGetUserRightID"
                oda.SelectCommand.Parameters.AddWithValue("@UID", Session("UID"))
                oda.SelectCommand.Parameters.AddWithValue("@EID", Session("EID"))
                oda.SelectCommand.Parameters.AddWithValue("@rolename", Session("USERROLE"))
                oda.Fill(dt)
                Dim str As String = String.Join(",", (From row In dt.AsEnumerable Select row("UID")).ToArray)
                If Not (str = "") Then
                    oda.SelectCommand.CommandType = CommandType.Text
                    oda.SelectCommand.CommandText = "select UID, UserName from mmm_mst_user where eid =" & Session("EID") & " and UID in (" & str & ") and isauth=1 order by UserName"
                    oda.Fill(dtusers)
                End If
            Else
                oda.SelectCommand.CommandType = CommandType.Text
                oda.SelectCommand.CommandText = "select UID, UserName from mmm_mst_user where eid =" & Session("EID") & " and isauth=1  and uid <>" & Session("UID") & "order by UserName"
                oda.Fill(dtusers)
            End If
        End If

        ddlUsers.DataSource = dtusers
        ddlUsers.DataTextField = "UserName"
        ddlUsers.DataValueField = "UID"
        ddlUsers.DataBind()
        ddlUsers.Items.Insert(0, "Select User")

    End Sub
End Class
