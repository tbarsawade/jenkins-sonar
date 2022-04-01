Imports System.Data.SqlClient
Imports System.Data
Partial Class AccessTFS
    Inherits System.Web.UI.Page
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
    Protected Sub btnSerach_Click(sender As Object, e As EventArgs) Handles btnSerach.Click
        Dim conStr As String = "server=172.16.30.5\sqldev;initial catalog=DMSTFS;uid=DMS;pwd=Ztsu93#u"
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
        'If con.State <> ConnectionState.Open Then
        '    con.Open()
        'End If
        Try
            oda.SelectCommand.CommandText = "select CardNo[Card No],officepunch[Punch Time] from tempdata where cardno= '" & txtEmpcode.Text.ToString & "'"
            oda.SelectCommand.CommandType = CommandType.Text
            Dim dt As New DataSet
            oda.Fill(dt)
            grdData.DataSource = dt
            grdData.DataBind()
        Catch ex As Exception
        Finally
            con.Close()
            con.Dispose()
        End Try

    End Sub

End Class
