Imports System.Collections.Generic
Imports System.Linq
Imports System.Web
Imports System.Web.Services
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Data.SqlClient
Imports System.Data
Imports System.Configuration
Imports System.Web.UI.Adapters.ControlAdapter
Imports System.Drawing
Imports System.Threading
Imports System
Imports System.Collections.Specialized
Imports System.Text
Imports System.Net.Security
Imports System.IO
Imports Newtonsoft.Json.Converters
Imports Newtonsoft.Json
Imports System.Web.Script.Serialization
Imports System.Web.Script.Services
Imports iTextSharp.text.pdf

Partial Class DepositeSlip
    Inherits System.Web.UI.Page

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


    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load

    End Sub

    <WebMethod()>
    Public Shared Function GetDSlip(dslipnum As String) As DGrid
        Dim grid As New DGrid()
        Dim strError = ""
        'dslipnum = "656565"
        Try
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As SqlConnection = New SqlConnection(conStr)
            Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
            Dim dt As New DataTable
            Dim qry As String = ""
            'qry = "select fld30[Bank Name],fld21[Bank Charges],fld31[Branch Name],fld28[fld28],fld10[Company code],fld5[Deposit Slip No] "
            'qry &= "from mmm_mst_doc where documenttype='pra' and fld5='" & dslipnum & "' and  eid=" & HttpContext.Current.Session("EID") & "  order by tid"

            qry = "select top 1 fld12[Customer Name],fld5[Deposit Reference No],fld18[G/L Account],dms.udf_split('MASTER-Location-fld1',fld51)[Location], "
            qry &= "fld50[Deposite Date],fld11[Client Code],fld17[Bank Name],fld18[Bank Account],fld47[Amount] from mmm_mst_doc where documenttype='pra' and fld5='" & dslipnum & "' and  eid=46"

            oda.SelectCommand.CommandText = qry
            Dim ds As New DataSet()

            Try

                Try
                    oda.SelectCommand.CommandTimeout = 300
                    oda.Fill(ds, "data")
                    If ds.Tables("data").Rows.Count = 0 Then
                        grid.Success = False
                        grid.Message = "No data found."
                    Else
                        grid = DynamicGrid.GridData(ds.Tables("data"), strError)
                    End If
                Catch exption As Exception
                    grid.Success = False
                    grid.Message = "Dear User please enter short date range."
                End Try
            Catch ex As Exception
                grid.Success = False
                grid.Message = "No data found."
            Finally
                con.Close()
                oda.Dispose()
                con.Dispose()
            End Try
        Catch ex As Exception

        End Try
        Return grid
    End Function

    Protected Sub btnDownload_Click(sender As Object, e As System.EventArgs) Handles btnDownload.Click
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim da As New SqlDataAdapter("select * from MMM_Print_Template where tid=111", con)
        Dim ds As New DataSet
        da.Fill(ds, "data")

        If ds.Tables("data").Rows.Count <> 1 Then
            da.Dispose()
            con.Dispose()
            Exit Sub
        End If
        Dim body As String = ds.Tables("data").Rows(0).Item("body").ToString()
        Dim MainQry As String = ds.Tables("data").Rows(0).Item("qry").ToString()
        Dim childQry As String = ds.Tables("data").Rows(0).Item("SQL_CHILDITEM").ToString()


        da.SelectCommand.CommandText = MainQry.Replace("@Dref", Trim(txtDslip.Text))
        da.Fill(ds, "main")

        For j As Integer = 0 To ds.Tables("main").Columns.Count - 1
            body = body.Replace("[" & ds.Tables("main").Columns(j).ColumnName & "]", ds.Tables("main").Rows(0).Item(j).ToString())
        Next

        da.SelectCommand.CommandText = childQry.Replace("@Dref", Trim(txtDslip.Text))
        da.Fill(ds, "child")
        ds.Dispose()
        con.Dispose()
        Dim strChildItem As String = "<div><table width=""100%"" border=""0.5""  >"
        Dim prevVal As String = ""
        For i As Integer = 0 To ds.Tables("child").Rows.Count - 1
            If prevVal = ds.Tables("child").Rows(i).Item(0).ToString() Then
                prevVal = ds.Tables("child").Rows(i).Item(0).ToString()
                ds.Tables("child").Rows(i).Item(0) = ""
            Else
                prevVal = ds.Tables("child").Rows(i).Item(0).ToString()
            End If
        Next

        For i As Integer = 0 To ds.Tables("child").Rows.Count
            strChildItem &= "<tr>"
            For j As Integer = 0 To ds.Tables("child").Columns.Count - 1
                strChildItem &= "<td>"
                If i = 0 Then
                    strChildItem &= ds.Tables("child").Columns(j).ColumnName
                Else
                    strChildItem &= ds.Tables("child").Rows(i - 1).Item(j).ToString()
                End If
                strChildItem &= "</td>"
            Next
            strChildItem &= "</tr>"
        Next
        strChildItem &= "</table></div>"
        body = body.Replace("[child item]", strChildItem)
        Dim pnl As New Panel
        pnl.Controls.Add(New LiteralControl(body))
        Dim attachment As String = "attachment; filename=" & "DepositeSlip" & txtDslip.Text & "_" & Now.Millisecond & ".pdf"
        System.Web.HttpContext.Current.Response.ClearContent()
        HttpContext.Current.Response.AddHeader("content-disposition", attachment)
        HttpContext.Current.Response.ContentType = "application/pdf"
        pnl.Font.Size = 7
        Dim stw As StringWriter = New StringWriter()
        Dim htextw As HtmlTextWriter = New HtmlTextWriter(stw)
        pnl.RenderControl(htextw)
       Dim document As iTextSharp.text.Document = New iTextSharp.text.Document()
        PdfWriter.GetInstance(document, HttpContext.Current.Response.OutputStream)
        document.Open()
        Dim Str As StringReader = New StringReader(stw.ToString())
        Dim htmlworker As iTextSharp.text.html.simpleparser.HTMLWorker = New iTextSharp.text.html.simpleparser.HTMLWorker(document)
        htmlworker.Parse(Str)
        document.Close()
        HttpContext.Current.Response.Write(document)
        HttpContext.Current.Response.End()

    End Sub
End Class
