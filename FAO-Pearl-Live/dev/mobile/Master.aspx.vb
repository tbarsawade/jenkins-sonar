Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports System.Globalization
Imports System.Drawing
Partial Class mobile_Master
    Inherits System.Web.UI.Page
    Public DocumentType As String = ""
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        DocumentType = Request.QueryString("SC").ToString()

        If Not IsPostBack Then
            'fill Product  
            Dim scrname As String = ""
            Try
                If Request.QueryString.HasKeys() Then
                    If Request.QueryString("SC") <> Nothing Then
                        scrname = Request.QueryString("SC").ToString()
                        DocumentType = scrname
                        BindDropDown(scrname)
                        BindGrid(DocumentType)
                    End If
                End If
            Catch ex As Exception
            End Try
        End If
        'gvData.Columns(1).Visible = False
    End Sub
    Protected Sub btnSearch_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles btnSearch.Click
        DocumentType = Request.QueryString("SC").ToString()
        BindGrid(DocumentType)
    End Sub
    Protected Sub BindGrid(scrname As String)
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = Nothing
        Dim da As SqlDataAdapter = Nothing
        Dim sField As String = ""
        Dim sValue As String = ""
        Dim eid As Integer = 0
        Try
            sValue = txtValue.Text.Trim()
            sValue = sValue
            sField = ddlField.SelectedItem.Value
            con = New SqlConnection(conStr)

            da = New SqlDataAdapter("uspGetGridByMasterType1NewMobile", con)
            da.SelectCommand.Parameters.Clear()
            da.SelectCommand.CommandType = CommandType.StoredProcedure
            da.SelectCommand.Parameters.AddWithValue("@sField", sField)
            da.SelectCommand.Parameters.AddWithValue("@sValue", sValue)
            da.SelectCommand.Parameters.AddWithValue("@eid", Val(Session("eid")))
            da.SelectCommand.Parameters.AddWithValue("@documentType", scrname)
            Dim ds As New DataSet
            con.Open()
            da.Fill(ds, "data")
            If ds.Tables("data").Rows().Count > 0 Then
                gvData.DataSource = ds.Tables("data")
                gvData.DataBind()
            End If
        Catch ex As Exception
            Dim strMsg = ex.Message
        Finally
            con.Dispose()
            da.Dispose()

        End Try
    End Sub

    Protected Sub BindDropDown(scrname As String)
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = Nothing
        Dim da As SqlDataAdapter = Nothing
        Try
            con = New SqlConnection(conStr)
            da = New SqlDataAdapter("SELECT displayName,FieldMapping FROM MMM_MST_FIELDS where EID =" & Session("EID").ToString() & " and DocumentType ='" & scrname & "' AND ISACTIVE=1 order by displayOrder", con)
            Dim ds As New DataSet
            con.Open()
            da.Fill(ds, "data")
            If ds.Tables("data").Rows().Count > 0 Then
                For i As Integer = 0 To ds.Tables("data").Rows.Count - 1
                    ddlField.Items.Add(ds.Tables("data").Rows(i).Item(0))
                    ddlField.Items(i).Value = ds.Tables("data").Rows(i).Item(1)
                Next
            End If
        Catch ex As Exception
        Finally
            con.Dispose()
            da.Dispose()

        End Try
    End Sub


    Protected Sub Add(ByVal sender As Object, ByVal e As System.EventArgs)
        Try
            Dim scrname As String = ""
            If Request.QueryString.HasKeys() Then
                If Request.QueryString("SC") <> Nothing Then
                    scrname = Request.QueryString("SC").ToString()
                    Response.Redirect("~/mobile/MasterNew.aspx?SC=" & scrname & "")
                End If
            End If
        Catch ex As Exception

        End Try
    End Sub
    Protected Sub gvData_RowDataBound(sender As Object, e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles gvData.RowDataBound
        e.Row.Cells(1).Visible = False
    End Sub
    Protected Sub gvData_PageIndexChanging(sender As Object, e As System.Web.UI.WebControls.GridViewPageEventArgs) Handles gvData.PageIndexChanging
        gvData.PageIndex = e.NewPageIndex
        DocumentType = Request.QueryString("SC").ToString()
        BindGrid(DocumentType)
    End Sub
End Class
