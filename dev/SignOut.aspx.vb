Imports System.Data.SqlClient
Imports System.Data

Partial Class SignOut
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        lblYear.InnerHtml = DateAndTime.Now.Year.ToString()

        Dim sURL As String = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority).ToUpper()

        sURL = Replace(sURL, "HTTP://", "")
        sURL = Replace(sURL, "HTTPS://", "")
        sURL = Replace(sURL, "WWW.", "")
        sURL = Replace(sURL, ".MYNDSAAS.COM", "")
        sURL = Replace(sURL, "MYNDSAAS.COM", "")
        sURL = Replace(sURL, "/SIGNOUT.ASPX", "")

        ViewState("company") = sURL

        If Not IsPostBack Then
            Try
                Dim oUser As New User()
                Dim res = oUser.UpdateUserLoginLog(Session("EID"), Session("UID"))
            Catch ex As Exception
            End Try

            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As SqlConnection = New SqlConnection(conStr)
            Dim eid As String = ""

            If Session("EID") Is Nothing Then
            Else
                eid = Session("EID")
            End If
            Dim od As SqlDataAdapter = New SqlDataAdapter("Select * from MMM_MST_ENTITY where EID='" & eid & "' ", con)
            od.SelectCommand.CommandType = CommandType.Text
            Dim ds As New DataSet()
            od.Fill(ds, "code")
            If ds.Tables("code").Rows.Count = 1 Then
                lblmsg.Text = Replace(ds.Tables("code").Rows(0).Item("Name").ToString(), "Welcome To", "")
                lblLogo.Text = "<img src=""logo/" & ds.Tables("code").Rows(0).Item("logo").ToString() & """ class=""img-responsive"" alt=""" & ds.Tables("code").Rows(0).Item("Name").ToString() & """  />"
                ' lbllogin.Text = 
                'ViewState("company") = ds.Tables("code").Rows(0).Item("Code").ToString()
            End If

            If Session("RETURL") Is Nothing Then
                Session("USERROLE") = Nothing
                Dim dtLogOut As Date = Now
                lblOutTime.Text = dtLogOut
                Dim dt As Integer
                Try
                    If Session("INTIME") Is Nothing Then
                        dt = DateDiff(DateInterval.Second, Now, Now.AddMinutes(20))
                    Else

                        lblIntime.Text = Session("INTIME").ToString()
                        dt = DateDiff(DateInterval.Second, Session("INTIME"), dtLogOut)
                    End If
                Catch ex As Exception

                End Try

                '    dt = 876543
                Dim diffSec As Integer = dt Mod 60
                Dim diffMin As Integer = (dt / 60) Mod 60
                Dim diffHour As Integer = (dt / 3600) Mod 60

                lblDuration.Text = diffHour.ToString("00") & ":" & diffMin.ToString("00") & ":" & diffSec.ToString("00")
                ' added for security testing observation by Torrid netword
                Session.Abandon()
                Session.Clear()
                If Request.Cookies("ASP.NET_SessionId") Is Nothing Then
                Else
                    ' Response.Cookies("ASP.NET_SessionId").Value = String.Empty
                    ' Response.Cookies("ASP.NET_SessionId").Expires = DateTime.Now.AddMonths(-20)
                    Response.Cookies.Add(New HttpCookie("ASP.NET_SessionId", ""))
                End If
                    'Response.Redirect("default.aspx")
                Else
                Dim returl As String = Session("RETURL").ToString()
                Session("USERROLE") = Nothing
                Session.Abandon()
                Session.Clear()
                If Request.Cookies("ASP.NET_SessionId") Is Nothing Then
                Else
                    ' Response.Cookies("ASP.NET_SessionId").Value = String.Empty
                    ' Response.Cookies("ASP.NET_SessionId").Expires = DateTime.Now.AddMonths(-20)
                    Response.Cookies.Add(New HttpCookie("ASP.NET_SessionId", ""))
                End If
                'Dim page As String = "HTTPS://" & ViewState("company") & ".myndsaas.com"
                'Response.Redirect(returl)
            End If
        End If
    End Sub

    Protected Sub btndefaultpage_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btndefaultpage.Click
        Dim page As String = ""
        If ViewState("company") Is Nothing Then
            page = "HTTPS://myndsaas.com"
        Else
            page = "HTTPS://" & ViewState("company") & ".myndsaas.com"
        End If
        Response.Redirect(page)
    End Sub
End Class
