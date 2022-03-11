
Imports System.Data
Imports System.Data.SqlClient
Imports System.Web.Services

Partial Class RoleAssignmentLog
    Inherits System.Web.UI.Page

    Dim con As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString

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
        Dim ada As New SqlDataAdapter("select UserName,uid from MMM_Mst_User with(nolock) where isAuth=1 and eid=" & Session("EID") & " order by UserName asc", con)
        Dim dataset As New DataSet
        ada.Fill(dataset, "data")
        ddlUserList.DataSource = dataset
        ddlUserList.DataTextField = "UserName"
        ddlUserList.DataValueField = "uid"
        ddlUserList.DataBind()
        ddlUserList.Items.Insert("0", "-Select-")
    End Sub

    <WebMethod()>
    Public Shared Function GetLog(sdate As String, tdate As String, Uid As String) As DGrid
        Dim grid As New DGrid()
        Dim strError = ""
        Try
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As New SqlConnection(conStr)
            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
            oda.SelectCommand.CommandType = CommandType.StoredProcedure
            oda.SelectCommand.CommandText = "USP_RoleHISTORY"
            oda.SelectCommand.Parameters.AddWithValue("@eid", HttpContext.Current.Session("EID"))
            oda.SelectCommand.Parameters.AddWithValue("@UID", Uid)
            oda.SelectCommand.Parameters.AddWithValue("@sdate", sdate)
            oda.SelectCommand.Parameters.AddWithValue("@tdate", tdate)
            Dim ds As New DataSet()
            oda.SelectCommand.CommandTimeout = 900
            oda.Fill(ds, "Log")
            If ds.Tables("Log").Rows.Count = 0 Then
                grid.Success = False
                grid.Message = "No data found."
            Else
                grid = DynamicGrid.GridData(ds.Tables("Log"), strError)
            End If
        Catch ex As Exception

        End Try
        Return grid
    End Function

End Class
