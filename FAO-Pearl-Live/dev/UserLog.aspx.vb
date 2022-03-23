Imports System.Data
Imports System.Data.SqlClient

Partial Class UserLog
    Inherits System.Web.UI.Page

    Protected Sub btnSearch_Click(sender As Object, e As System.Web.UI.ImageClickEventArgs) Handles btnSearch.Click
        gvData.DataSource = binddataset()
        gvData.DataBind()
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
    Private Function binddataset() As DataSet
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        'fill Product
        Dim strQry As String = "Select MMM_MST_ACTIONLOG.Username + ' (' + emailID + ')' [Username] ,ActionDate,Actiontype,Actiondesc,ipaddress,macaddress FROM MMM_MST_ACTIONLOG inner JOIN MMM_MST_USER on MMM_MST_USER.uid = MMM_MST_ACTIONLOG.uid "
        Dim strWhere As String = " WHERE 1=1 "

        If txtFDate.Text.Length() < 8 Then
        Else
            Dim dt As Date = txtFDate.Text
            strWhere = " AND Actiondate > '" & dt.Year & "-" & Format(dt.Month, "00") & "-" & Format(dt.Day, "00") & "'"
        End If

        If txtLDate.Text.Length() < 8 Then
        Else
            Dim dt As Date = txtLDate.Text
            dt = dt.AddDays(1)
            strWhere = strWhere & " AND Actiondate < '" & dt.Year & "-" & Format(dt.Month, "00") & "-" & Format(dt.Day, "00") & "'"
        End If
        strQry = strQry & strWhere & " ORDER BY ActionDate"
        Dim da As New SqlDataAdapter(strQry, con)
        Dim ds As New DataSet
        da.Fill(ds, "Search Report")
        con.Dispose()
        da.Dispose()
        Return ds
    End Function
End Class
