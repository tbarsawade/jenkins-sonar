Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports System.Globalization
Imports System.Drawing

Partial Class SU_LoginMgmt
    Inherits System.Web.UI.Page

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        'If Session("doctype") Is Nothing Then
        'Else
        '    Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        '    Dim con As SqlConnection = New SqlConnection(conStr)
        '    Dim oda As SqlDataAdapter = New SqlDataAdapter("SELECT FormDesc,formcaption,displayName,FieldType,DropDownType,dropdown,FieldMapping,LayoutType,isrequired,datatype,fieldid,cal_fields,autofilter  FROM MMM_MST_FIELDS FF left outer JOIN MMM_MST_FORMS F on F.FormName = FF.DocumentType and F.EID = FF.EID where FF.isactive=1 and F.EID=" & Session("EID").ToString() & " and FormName = '" & Session("doctype") & "' and FF.isworkFlow=1 order by displayOrder", con)
        '    Dim ds As New DataSet()
        '    oda.Fill(ds, "fields")
        '    Dim ob As New DynamicForm()
        '    'ob.CreateControlsOnAuthMetrix(ds.Tables("fields"), pnlFields)
        '    oda.Dispose()
        '    ds.Dispose()
        'End If
        Dim strPreviousPage As String = ""
        If Request.UrlReferrer <> Nothing Then
            strPreviousPage = Request.UrlReferrer.Segments(Request.UrlReferrer.Segments.Length - 1)
        End If
        If strPreviousPage = "" Then
            Response.Redirect("~/Invalidaction.aspx")
        End If
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
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As New SqlConnection(conStr)
            Dim i As Integer
            'fill Product  
            Dim da As New SqlDataAdapter("select TOP 1 case isauth when 1 then 'Active' when 0 then 'Inactive' else 'Others' end Status,uid  from MMM_MST_USER where EID='" & Session("EID").ToString() & "' and userrole='SU' and uid=28474 order by uid", con)
            Dim ds As New DataSet()
            da.Fill(ds, "user")
            ' for adding users as was in old format
            LblStatus.Text = ds.Tables("user").Rows(0).Item("status").ToString
            ViewState("Did") = ds.Tables("user").Rows(0).Item("uid").ToString
            da.Dispose()
            con.Dispose()
        End If
    End Sub

    Protected Sub DeleteRecord(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
        Dim str As String = ""
        Dim objDMSUtil As New DMSUtil()
        If LblStatus.Text.ToUpper() = "ACTIVE" Then
            str = "update MMM_MST_USER Set isauth=0 WHERE eid=" & Session("eid") & " and UID=" & ViewState("Did").ToString()
            LblStatus.Text = "Inactive"
            objDMSUtil.SUActivityLog(Session("EID"), Session("UID"), "USER", "SUPERUSER De-Activation", ViewState("Did").ToString())
        ElseIf LblStatus.Text.ToUpper() = "INACTIVE" Then
            str = "update MMM_MST_USER Set isauth=1 WHERE eid=" & Session("eid") & " and UID=" & ViewState("Did").ToString()
            LblStatus.Text = "Active"
            objDMSUtil.SUActivityLog(Session("EID"), Session("UID"), "USER", "SUPERUSER Activation", ViewState("Did").ToString())
        Else
            str = ""
        End If

        oda.SelectCommand.CommandType = CommandType.Text
        oda.SelectCommand.Parameters.Clear()
        oda.SelectCommand.CommandText = str

        If con.State <> ConnectionState.Open Then
            con.Open()
        End If
        oda.SelectCommand.ExecuteNonQuery()
        oda.Dispose()
        con.Close()
        con.Dispose()
        'bindgrid()
        btnDelete_ModalPopupExtender.Hide()
    End Sub

    Protected Sub DeleteHit(ByVal sender As Object, ByVal e As System.EventArgs)
        lblRecord.Visible = False
        Dim btnDetails As ImageButton = TryCast(sender, ImageButton)
        ' Dim row As GridViewRow = DirectCast(btnDetails.NamingContainer, GridViewRow)
        'Dim pid As Integer = 28474 ' Convert.ToString(Me.gvData.DataKeys(row.RowIndex).Value)
        ' ViewState("Did") = pid

        If LblStatus.Text.ToUpper().Trim() = "ACTIVE" Then
            lblMsgDelete.Text = "&nbsp;&nbsp;<b>Please confirm if you wish to lock Superuser Login ID!</b>"
            'LblStatus.Text = "Inactive"
        ElseIf LblStatus.Text.ToUpper().Trim() = "INACTIVE" Then
            lblMsgDelete.Text = "&nbsp;&nbsp;<b>Please confirm if you wish to Unlock Superuser Login ID!</b>"
            'LblStatus.Text = "Active"
        Else
            lblMsgDelete.Text = "&nbsp;&nbsp;<b>No action possible, Login status is others!</b>"
            ' LblStatus.Text = "NONE"
        End If
        'lblMsgDelete.Text = "Are you Sure Want to delete this Auth Matrix Record? " & pid
        btnActDelete.Visible = True
        updatePanelDelete.Update()
        btnDelete_ModalPopupExtender.Show()
    End Sub



End Class
