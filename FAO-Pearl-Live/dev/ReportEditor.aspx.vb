Imports System.Data
Imports System.Data.SqlClient
Imports AjaxControlToolkit
Imports iTextSharp.text.pdf
Imports System.Web.HttpContext
Imports Microsoft.VisualBasic
Imports System.IO

Partial Class ReportEditor
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        If Not IsPostBack Then

            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As New SqlConnection(conStr)
            Dim da As New SqlDataAdapter("select distinct documenttype from mmm_mst_fields where eid='" & Session("EID") & "' ", con)
            Dim ds As New DataSet
            da.Fill(ds, "data")
            ddlDocType.DataSource = ds

            ddlDocType.DataTextField = "documenttype"
            ddlDocType.DataValueField = "documenttype"
            ddlDocType.DataBind()
            da.Dispose()
            con.Dispose()
            Accord()
        End If

        txtEditor.Visible = True

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
    Protected Sub btnAccSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAccSave.Click
        'If txtTemp.Text = "" Then
        '    lblmsg.Text = "Enter Template Name"
        '    Exit Sub
        'End If
        ViewState("DocType") = "Value Contract"
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)

        Dim da As New SqlDataAdapter("usp_AccordEditor", con)
        Dim cnt As Integer
        da.SelectCommand.CommandText = "select count(*)  from MMM_Print_Template where EID=" & Session("EID").ToString() & " and documenttype='" & ViewState("DocType") & "' "

        If con.State <> ConnectionState.Open Then
            con.Open()
        End If
        cnt = da.SelectCommand.ExecuteScalar
        'If cnt = 0 Then
        da.SelectCommand.CommandText = "usp_AccordEditor"
        da.SelectCommand.CommandType = CommandType.StoredProcedure
        da.SelectCommand.Parameters.Clear()
        da.SelectCommand.Parameters.AddWithValue("eid", Session("EID"))
        da.SelectCommand.Parameters.AddWithValue("temp", txtTemp.Text)
        da.SelectCommand.Parameters.AddWithValue("editor", txtEditor.Text)
        ' da.SelectCommand.Parameters.AddWithValue("doctype", ViewState("DocType"))
        da.SelectCommand.ExecuteNonQuery()
        'End If
        Dim da3 As New SqlDataAdapter("select body  from MMM_Print_Template where EID=" & Session("EID").ToString() & " and documenttype='" & ViewState("DocType") & "' ", con)
        Dim dt3 As New DataTable()
        da3.Fill(dt3)



        ViewState("body") = dt3.Rows(0).Item("body").ToString()
        Dim s As String = ViewState("body").ToString
        Dim words As String() = s.Split("{")
        Dim spltwrd As String()
        Dim builder As New StringBuilder
        Dim s1 As String

        For i = 0 To words.Length - 1


            If words(i).Contains("}") Then
                spltwrd = Split(words(i), "}")

                builder.Clear()
                builder.Append(spltwrd(0))
                'builder.AppendLine()
                s1 = builder.ToString

                ddlfld.Items.Add(Trim(s1))



                'Else
                '    ddlDyFildsname.DataSource = words
                '    ddlDyFildsname.DataBind()
            End If


        Next
        Dim fld As String
        Dim strReplace1 As String
        Dim da4 As New SqlDataAdapter("", con)
        For j As Integer = 0 To ddlfld.Items.Count - 1
            Dim txt As String = ddlfld.Items(j).ToString
            txt = Trim(txt.ToString)
            da4.SelectCommand.CommandText = "select fieldmapping from MMM_MST_FIELDS where EID='" & Session("EID") & "' and displayname='" & txt.ToString() & "' and documenttype='" & ViewState("DocType") & "'"
            'Dim da As New DataSet
            'da4.Fill(da, "data")
            'ViewState("strbody") = da.Tables("data").Rows(0).Item("body").ToString()
            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            fld = da4.SelectCommand.ExecuteScalar()
            ddlfld.Items(j).Value() = fld
        Next

        da4.Dispose()
        For j As Integer = 0 To ddlfld.Items.Count - 1

            If j = 0 Then
                strReplace1 = Replace(ViewState("body"), "{" & Trim(ddlfld.Items(j).ToString) & "}", "{" & ddlfld.Items(j).Value & "}")
            Else
                strReplace1 = Replace(strReplace1, "{" & Trim(ddlfld.Items(j).ToString) & "}", "{" & ddlfld.Items(j).Value & "}")


                'Dim stfld As String = ddlfld.Items(j).ToString

                'ViewState("strfld") = stfld
            End If


        Next
        da.SelectCommand.CommandText = "usp_AccordEditor"
        da.SelectCommand.CommandType = CommandType.StoredProcedure
        da.SelectCommand.Parameters.Clear()
        da.SelectCommand.Parameters.AddWithValue("eid", Session("EID"))
        da.SelectCommand.Parameters.AddWithValue("editor", strReplace1.ToString())
        da.SelectCommand.Parameters.AddWithValue("temp", txtTemp.Text)
        'da.SelectCommand.Parameters.AddWithValue("doctype", ViewState("DocType"))
        If con.State <> ConnectionState.Open Then
            con.Open()
        End If


        da.SelectCommand.ExecuteNonQuery()
        lblmsg.Text = "Template Saved Successfully"
        Accord()
        upeditor.Update()

    End Sub
    Public Sub Print(ByRef Tid As Integer, ByRef panel1 As Panel)
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)

        Dim da As New SqlDataAdapter("select * from MMM_Print_Template where   Tid='" & Tid & " ' ", con)
        Dim ds As New DataSet
        da.Fill(ds, "data")

        Dim MSG As String = ""
        Dim MSG1 As String = ""
        Dim MSG4 As String = ""

        Dim MSGtemp As String = ""
        Dim MSGtemp1 As String = ""
        Dim MSGtemp4 As String = ""

        Dim msg2 As String = ""
        Dim msg3 As String = ""
        Dim msg5 As String = ""
        MSG = ds.Tables("data").Rows(0)("body")
        MSG1 = ds.Tables("data").Rows(0)("body")
        MSG4 = ds.Tables("data").Rows(0)("body")

        Dim da6 As New SqlDataAdapter("select formtype,formname from mmm_mst_forms where eid=" & Session("EID") & " and formname='" & ViewState("DocType") & "' ", con)
        Dim ds6 As New DataSet
        da6.Fill(ds6, "qry")
        Dim s2 As String
        s2 = ds6.Tables("qry").Rows(0).Item("formtype")
        If s2 = "MASTER" Then
            Dim da1 As New SqlDataAdapter("Select * from mmm_mst_master where eid=" & Session("EID") & "  and documenttype='" & ViewState("DocType") & "'", con)
            Dim ds1 As New DataSet
            da1.Fill(ds1, "qry")
            For i As Integer = 0 To ds1.Tables("qry").Rows.Count - 1
                MSGtemp = MSG

                For j As Integer = 0 To ds1.Tables("qry").Columns.Count - 1
                    MSGtemp = MSGtemp.Replace("{" & ds1.Tables("qry").Columns(j).ColumnName & "}", ds1.Tables("qry").Rows(i).Item(j).ToString())

                Next
                msg2 = msg2 & "<br>" & MSGtemp
            Next
            If ds1.Tables("qry").Rows.Count > 0 Then
                panel1.Controls.Add(New LiteralControl(msg2))
            End If
        End If

        If s2 = "DOCUMENT" Then
            Dim da1 As New SqlDataAdapter("Select * from mmm_mst_doc where eid=" & Session("EID") & " and tid=10340 and documenttype='" & ViewState("DocType") & "'", con)
            Dim ds1 As New DataSet
            da1.Fill(ds1, "qry")
            For i As Integer = 0 To ds1.Tables("qry").Rows.Count - 1
                MSGtemp = MSG

                For j As Integer = 0 To ds1.Tables("qry").Columns.Count - 1
                    MSGtemp = MSGtemp.Replace("{" & ds1.Tables("qry").Columns(j).ColumnName & "}", ds1.Tables("qry").Rows(i).Item(j).ToString())

                Next
                msg2 = msg2 & "<br>" & MSGtemp
            Next

            If ds1.Tables("qry").Rows.Count > 0 Then
                panel1.Controls.Add(New LiteralControl(msg2))
            End If

        End If


        Dim da2 As New SqlDataAdapter("select * from mmm_mst_calendar c  inner join mmm_mst_doc d on c.docid=d.tid where d.eid=" & Session("EID") & " and d.documenttype='" & ViewState("DocType") & "' ", con)
        Dim ds2 As New DataSet
        da2.Fill(ds2, "qry")
        For i As Integer = 0 To ds2.Tables("qry").Rows.Count - 1
            MSGtemp1 = MSG1

            For j As Integer = 0 To ds2.Tables("qry").Columns.Count - 1
                MSGtemp1 = MSGtemp1.Replace("{" & ds2.Tables("qry").Columns(j).ColumnName & "}", ds2.Tables("qry").Rows(i).Item(j).ToString())

            Next
            msg3 = msg3 & "<br>" & MSGtemp1
        Next

        Dim cnt As Integer = 0
        For i As Integer = 0 To ddlfld.Items.Count - 1
            For x As Integer = 0 To ddlap2.Items.Count - 1
                If ddlfld.Items(i).Text = ddlap2.Items(x).Text Then
                    cnt = cnt + 1
                    Exit For
                End If
            Next
        Next

        If cnt > 0 Then
            Dim da3 As New SqlDataAdapter("select * from mmm_doc_dtl c  inner join mmm_mst_doc d on c.docid=d.tid where d.eid=" & Session("EID") & "  and d.documenttype='" & ViewState("DocType") & "' and d.tid=46397  ", con)
            Dim ds3 As New DataSet
            da3.Fill(ds3, "qry")
            For n As Integer = 0 To ds3.Tables("qry").Rows.Count - 1
                MSGtemp4 = MSG4

                For j As Integer = 0 To ds3.Tables("qry").Columns.Count - 1
                    MSGtemp4 = MSGtemp4.Replace("{" & ds3.Tables("qry").Columns(j).ColumnName & "}", ds3.Tables("qry").Rows(n).Item(j).ToString())

                Next
                msg5 = msg5 & "<br>" & MSGtemp4
            Next
            If ds3.Tables("qry").Rows.Count > 0 Then
                panel1.Controls.Add(New LiteralControl(msg5))
            End If
        End If


        If ds2.Tables("qry").Rows.Count > 0 Then
            panel1.Controls.Add(New LiteralControl(msg3))
        End If





        Dim attachment As String = "attachment; filename=ApplicationForm.pdf"

        System.Web.HttpContext.Current.Response.ClearContent()

        HttpContext.Current.Response.AddHeader("content-disposition", attachment)

        HttpContext.Current.Response.ContentType = "application/pdf"

        Dim stw As StringWriter = New StringWriter()

        Dim htextw As HtmlTextWriter = New HtmlTextWriter(stw)

        panel1.RenderControl(htextw)

        Dim document As iTextSharp.text.Document = New iTextSharp.text.Document()

        PdfWriter.GetInstance(document, HttpContext.Current.Response.OutputStream)

        document.Open()

        Dim Str As StringReader = New StringReader(stw.ToString())

        Dim htmlworker As iTextSharp.text.html.simpleparser.HTMLWorker = New iTextSharp.text.html.simpleparser.HTMLWorker(document)

        htmlworker.Parse(Str)

        document.Close()
        HttpContext.Current.Response.Write(document)
        HttpContext.Current.Response.End()
        ds.Dispose()
        con.Dispose()
    End Sub


    Protected Sub imgbtnPrint_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles imgbtnPrint.Click
        Print(42, panel1)
    End Sub
    Protected Sub Accord()
        Dim ap1 As New AccordionPane()
        Dim ap2 As New AccordionPane()
        Dim ap3 As New AccordionPane()
        Dim ap4 As New AccordionPane()
        Dim ap5 As New AccordionPane()
        ViewState("DocType") = ddlDocType.SelectedValue.ToString()

        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)

        Dim da As New SqlDataAdapter("select distinct documenttype,fieldmapping,'{'+displayname+'}'[displayname] from mmm_mst_fields where documenttype= '" & ViewState("DocType").ToString() & "' and  EID = " & Session("EID").ToString() & " ", con)
        Dim ds As New DataSet
        da.Fill(ds, "data")

        If ds.Tables("data").Rows.Count > 0 Then
            ap1.HeaderContainer.Controls.Add(New LiteralControl(ds.Tables("data").Rows(0).Item("documenttype").ToString))

            For i As Integer = 0 To ds.Tables("data").Rows.Count - 1
                ap1.ContentContainer.Controls.Add(New LiteralControl(ds.Tables("data").Rows(i).Item("displayname").ToString & "</br>"))
            Next
        End If
        ap1.ID = "ap1"
        ap2.ID = "ap2"
        ap3.ID = "ap3"
        ap4.ID = "ap4"
        ap5.ID = "ap5"
        acc1.Panes.Add(ap1)


        ap2.HeaderContainer.Controls.Add(New LiteralControl("Doc Mov. Detail"))

        ap2.ContentContainer.Controls.Add(New LiteralControl("{userid}" & "</br>"))
        ap2.ContentContainer.Controls.Add(New LiteralControl("{docid}" & "</br>"))
        ap2.ContentContainer.Controls.Add(New LiteralControl("{fdate}" & "</br>"))
        ap2.ContentContainer.Controls.Add(New LiteralControl("{tdate}" & "</br>"))
        ap2.ContentContainer.Controls.Add(New LiteralControl("{aprstatus}" & "</br>"))
        ' ap2.ContentContainer.Controls.Add(New LiteralControl("{sla}" & "</br>"))
        ap2.ContentContainer.Controls.Add(New LiteralControl("{remarks}" & "</br>"))

        acc1.Panes.Add(ap2)



        'ap5.HeaderContainer.Controls.Add(New LiteralControl("Child Item Dtl."))

        'ap5.ContentContainer.Controls.Add(New LiteralControl("Formula One" & "</br>"))
        'ap5.ContentContainer.Controls.Add(New LiteralControl("Formula Two " & "</br>"))
        'ap5.ContentContainer.Controls.Add(New LiteralControl("Formula Three " & "</br>"))

        'acc1.Panes.Add(ap5)



        ap3.HeaderContainer.Controls.Add(New LiteralControl("Calendar Detail"))
        'ap3.ContentContainer.Controls.Add(New LiteralControl("{DOCID}" & "</br>"))
        ap3.ContentContainer.Controls.Add(New LiteralControl("{DUE_DATE}" & "</br>"))
        ap3.ContentContainer.Controls.Add(New LiteralControl("{COMPLETION_DATE}" & "</br>"))
        ap3.ContentContainer.Controls.Add(New LiteralControl("{DOCUMENTTYPE}" & "</br>"))
        ap3.ContentContainer.Controls.Add(New LiteralControl("{STATUS}" & "</br>"))

        acc1.Panes.Add(ap3)
        ap4.HeaderContainer.Controls.Add(New LiteralControl("Formula"))

        acc1.Panes.Add(ap4)


    End Sub

    Protected Sub ddlDocType_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlDocType.SelectedIndexChanged
        acc1.Visible = True
        Accord()

    End Sub
End Class
