Imports System.Data
Imports System.Data.SqlClient

Partial Class MainHome
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        If Not Session("EID") Is Nothing Then
            If Not IsPostBack Then
                If Request.QueryString.HasKeys() Then
                    If Request.QueryString("MID") <> Nothing Then
                        Dim MID As Integer = Val(Request.QueryString("MID"))
                        BindMenu(Val(Session("EID").ToString()), MID, Session("USERROLE").ToString())
                    End If
                Else
                    BindMenu(Val(Session("EID").ToString()), 0, Session("USERROLE").ToString())
                End If
            End If
        Else
            Response.Redirect("~/mobile/login.aspx")
        End If
    End Sub

    Protected Sub BindMenu(ByVal EID As Integer, ByVal PmenuID As Integer, userrole As String)
        Try
            Dim StrMenu As New StringBuilder()
            Dim dsMenu As New DataSet
            dsMenu = GetMenu(EID, PmenuID, userrole)
            If dsMenu.Tables(0).Rows.Count > 0 Then
                'Adding table
                'dsMenu.Tables(0).Rows.Add("-1", "Dash Board", "0", "DashBoard.aspx", "home.png")
                StrMenu.Append("<table width=""100%"" border=""0"" cellspacing=""5px"" cellpadding=""2px"">")

                '<td style="width: 25%; text-align: center"></td>
                'pnlFields.Controls.Add(New LiteralControl("<td style=""width:" & lblWidth & "px;text-align:right"">"))
                Dim k As Integer = 0
                For i As Integer = 0 To dsMenu.Tables(0).Rows.Count - 1
                    StrMenu.Append("<tr>")

                    Dim pageLink As String = dsMenu.Tables(0).Rows(i).Item("PageLink").ToString()
                    If pageLink = "MENU" Then
                        pageLink = "MainHome.aspx?MID=" & dsMenu.Tables(0).Rows(i).Item("MID").ToString()
                    Else
                        pageLink = dsMenu.Tables(0).Rows(i).Item("PageLink").ToString()
                        If pageLink = "mainhome.aspx" Then
                            pageLink = "DashBoard.aspx"
                        Else
                            pageLink = pageLink & "&Req=2"
                        End If
                    End If
                    'mainhome.aspx

                    StrMenu.Append("<td style=""width: 33%;height:100px; text-align: center"" class=""BoxCurve"">")
                    StrMenu.Append("<a href=""" & pageLink & """><img width=""80px"" height=""80px"" alt=""" & dsMenu.Tables(0).Rows(i).Item("MenuName").ToString() & """ src=""../images/m" & dsMenu.Tables(0).Rows(i).Item("image").ToString() & """ /><br/> " & dsMenu.Tables(0).Rows(i).Item("menuname").ToString() & "</a>")
                    StrMenu.Append("</td>")


                    i = i + 1
                    StrMenu.Append("<td style=""width: 33%;height:100px; text-align: center"" class=""BoxCurve"">")

                    If i < dsMenu.Tables(0).Rows.Count Then
                        pageLink = dsMenu.Tables(0).Rows(i).Item("PageLink").ToString()
                        If pageLink = "MENU" Then
                            pageLink = "MainHome.aspx?MID=" & dsMenu.Tables(0).Rows(i).Item("MID").ToString()
                        Else
                            pageLink = dsMenu.Tables(0).Rows(i).Item("PageLink").ToString()
                            If pageLink = "mainhome.aspx" Then
                                pageLink = "DashBoard.aspx"
                            Else
                                pageLink = pageLink & "&Req=2"
                            End If
                        End If
                        StrMenu.Append("<a href=""" & pageLink & """><img width=""80px"" height=""80px"" alt=""" & dsMenu.Tables(0).Rows(i).Item("MenuName").ToString() & """ src=""../images/m" & dsMenu.Tables(0).Rows(i).Item("image").ToString() & """ /><br/> " & dsMenu.Tables(0).Rows(i).Item("menuname").ToString() & "</a>")
                    Else
                        StrMenu.Append(" ")
                    End If
                    StrMenu.Append("</td>")

                    i = i + 1
                    StrMenu.Append("<td style=""width: 33%;height:100px; text-align: center"" class=""BoxCurve"">")

                    If i < dsMenu.Tables(0).Rows.Count Then
                        pageLink = dsMenu.Tables(0).Rows(i).Item("PageLink").ToString()
                        If pageLink = "MENU" Then
                            pageLink = "MainHome.aspx?MID=" & dsMenu.Tables(0).Rows(i).Item("MID").ToString()
                        Else
                            pageLink = dsMenu.Tables(0).Rows(i).Item("PageLink").ToString()
                            If pageLink = "mainhome.aspx" Then
                                pageLink = "DashBoard.aspx"
                            Else
                                pageLink = pageLink & "&Req=2"
                            End If
                        End If
                        StrMenu.Append("<a href=""" & pageLink & """><img width=""80px"" height=""80px"" alt=""" & dsMenu.Tables(0).Rows(i).Item("MenuName").ToString() & """ src=""../images/m" & dsMenu.Tables(0).Rows(i).Item("image").ToString() & """ /><br/> " & dsMenu.Tables(0).Rows(i).Item("menuname").ToString() & "</a>")
                    Else
                        StrMenu.Append(" ")
                    End If
                    StrMenu.Append("</td>")
                    StrMenu.Append("</tr>")

                Next
                StrMenu.Append("</table>")
                lblMenu.Text = StrMenu.ToString()
            End If
        Catch ex As Exception

        End Try
    End Sub

    Public Function GetMenu(ByVal EID As Integer, ByVal PmenuID As Integer, userrole As String) As DataSet
        Dim dsMenu As New DataSet
        Dim con As SqlConnection = Nothing
        Dim da As SqlDataAdapter = Nothing
        Try
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            con = New SqlConnection(conStr)
            da = New SqlDataAdapter("getMenu", con)
            da.SelectCommand.CommandType = CommandType.StoredProcedure
            da.SelectCommand.Parameters.AddWithValue("@EID", EID)
            da.SelectCommand.Parameters.AddWithValue("@PmenuID", PmenuID)
            da.SelectCommand.Parameters.AddWithValue("@userRole", userrole)
            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            da.Fill(dsMenu)
            Return dsMenu
        Catch ex As Exception
            Throw
        Finally
            con.Close()
            con.Dispose()
            da.Dispose()
        End Try

    End Function

End Class
