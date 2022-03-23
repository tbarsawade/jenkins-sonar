Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports System.Globalization
Imports AjaxControlToolkit

Partial Class ExtendRelation
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            
            If Not Page.IsPostBack Then
                Dim EID As Integer

                Dim ObjRel As New RelationResponse()
                Dim Obj As New Relation()
                If Request.QueryString.HasKeys Then
                    If Not Request.QueryString("DOCID") Is Nothing And Not Request.QueryString("SC") Is Nothing Then
                        Dim DocID = Convert.ToInt32(Request.QueryString("DOCID"))
                        EID = Convert.ToInt32(Session("EID"))
                        Dim DocumentType = Request.QueryString("SC")
                        ObjRel = Obj.GetExecutableRelationTDocType(EID, DocumentType)

                    End If
                Else
                    Response.Redirect("MainHome.aspx")
                End If
                If ObjRel.Success = True Then
                    GetFormField(EID, ObjRel.SDocType)
                    BindData(ObjRel.SDocType)
                End If
            End If
        Catch ex As Exception
        End Try
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



    Protected Sub bindGrid(dsData As DataSet)
        Try

            If dsData.Tables(0).Rows.Count > 0 Then
                gvData.DataSource = dsData.Tables(0)
                gvData.DataBind()
                lblDataCount.Text = dsData.Tables(0).Rows.Count
                pnlData.Visible = True


            Else
                gvData.DataSource = Nothing
                gvData.DataBind()
                Session("AppRData") = Nothing
            End If
        Catch ex As Exception

        End Try
    End Sub




    Protected Sub BindData(DocumentType As String)
        Try
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim EID As Integer
            EID = Convert.ToUInt32(Session("EID"))
            Dim objRel As New Relation()
            Dim ds As New DataSet()
            Dim dsD As New DataSet()
            ds = objRel.GetAllFields(EID)
            Dim StrQuery = objRel.GenearateQuery1(EID, DocumentType, ds)
            'StrQuery = StrQuery.Replace(DocumentType & ".", "")
            StrQuery = "SELECT  v" & EID & DocumentType.Trim.Replace(" ", "_") & ".tid  AS DOCID, " & StrQuery
            Using con = New SqlConnection(conStr)
                Using da = New SqlDataAdapter(StrQuery, con)
                    da.Fill(dsD, "tbldata")
                End Using
            End Using
            Session("AppRData") = dsD
            bindGrid(dsD)
        Catch ex As Exception

        End Try
    End Sub
    Protected Sub ddlFieldName_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlFieldName.SelectedIndexChanged
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Try
            Dim Documenttype = ""
            Dim FieldMapping As String = ""
            Dim ds As New DataSet()
            Dim StrCol = ddlFieldName.SelectedItem.Value
            'StrCol = "[" & StrCol & "]"
            ds = DirectCast(Session("AppRData"), DataSet)
            Dim view As DataView = New DataView(ds.Tables(0))
            view.RowFilter = "[" & StrCol & "]" & "<>' '"
            Dim distinctValues As DataTable = view.ToTable(True, StrCol)
            'Dim distinctValues As DataTable = view.RowFilter=""
            If distinctValues.Rows.Count > 0 Then
                Dim li As New ListItem("--Select--", 0)
                ddlFormValue.Items.Insert(0, li)
                ddlFormValue.DataSource = distinctValues
                ddlFormValue.DataTextField = StrCol
                ddlFormValue.DataValueField = StrCol
                ddlFormValue.DataBind()
                ddlFormValue.Items.Insert(0, li)
            End If
        Catch ex As Exception
        End Try
    End Sub
    Public Sub GetFormField(EID As Integer, DocumentType As String)
        Dim li As New ListItem("--Select--", 0)
        Try
            Dim obj As New Relation()
            Dim ds As New DataSet()
            ds = obj.GetAllFields(EID, DocumentType)
            If ds.Tables(0).Rows.Count > 0 Then
                ddlFieldName.DataSource = ds
                ddlFieldName.DataTextField = "DisplayName"
                ddlFieldName.DataValueField = "DisplayName"
                ddlFieldName.DataBind()
            End If
            ddlFieldName.Items.Insert(0, li)
        Catch ex As Exception

        End Try
    End Sub

    Protected Sub btnFilter_Click(sender As Object, e As EventArgs) Handles btnFilter.Click
        Dim DS As New DataSet()
        Dim DSF As New DataSet()
        Dim StrCol As String = "0"
        Dim StrVal As String = "0"
        StrCol = ddlFieldName.SelectedItem.Value
        StrVal = ddlFormValue.SelectedItem.Value
        Try
            If Not Session("AppRData") Is Nothing Then
                DS = DirectCast(Session("AppRData"), DataSet)
                If DS.Tables(0).Rows.Count > 0 Then
                    If StrCol <> "0" And StrVal <> "0" Then
                        Dim tbldata As New DataTable("tbldata")
                        Dim view As DataView = New DataView(DS.Tables(0))
                        view.RowFilter = "[" & StrCol & "]" & "='" & StrVal & "'"
                        tbldata = view.ToTable
                        DSF.Tables.Add(tbldata)
                        bindGrid(DSF)
                    Else
                        bindGrid(DS)
                    End If
                End If
            End If

        Catch ex As Exception

        End Try
    End Sub

    Protected Sub btnExtend_Click(sender As Object, e As EventArgs) Handles btnExtend.Click
        Dim HeaderRow As GridViewRow
        Dim DocIds As New StringBuilder()
        Dim StrDocIds As String = ""
        Dim IsChecked As Boolean = False
        Dim RowNumber As Integer = 0
        Try
            HeaderRow = gvData.HeaderRow
            If gvData.Rows.Count > 0 Then
                'loop through all rows of grid
                For Each row As GridViewRow In gvData.Rows
                    'Navigating each column of grid
                    RowNumber = RowNumber + 1
                    IsChecked = False
                    If row.RowType = DataControlRowType.DataRow Then
                        Dim t1 = row.Cells(0).Controls(1)
                        Dim chkBox As CheckBox
                        chkBox = CType(t1, CheckBox)
                        If Not (chkBox Is Nothing) And chkBox.Checked = True Then
                            DocIds.Append(",").Append(Convert.ToString(Me.gvData.DataKeys(row.RowIndex).Value))
                        End If
                    End If
                Next
                StrDocIds = DocIds.ToString.Substring(1, DocIds.ToString.Length - 1)
                'Now Invocking Extend Relation Code
                Dim objRel As New Relation()
                Dim objRelRes As New RelationResponse()
                Dim DocID = Convert.ToInt32(Request.QueryString("DOCID"))
                Dim EID = Convert.ToInt32(Session("EID"))
                Dim DocumentType = Request.QueryString("SC")
                Dim UID As Integer = Convert.ToInt32(Session("UID"))
                objRelRes = objRel.ExtendRelation(EID, DocumentType, DocID, UID, StrDocIds, True)
                ScriptManager.RegisterStartupScript(Me, Me.GetType(), "popup", "alert('" & objRelRes.Message & "');", True)
                'If objRelRes.Success = True Then
                '    Response.Redirect("MainHome.aspx")
                'End If
            End If
        Catch ex As Exception
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "popup", "alert('Error occured!!');", True)
        End Try
    End Sub
End Class
