Partial Class closeWindow
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Page.ClientScript.RegisterStartupScript(Me.GetType(), "myCloseScript", "window.close()", True)
    End Sub
End Class
