Imports System.Data.SqlClient
Imports System.Data

Partial Class companylogo
    Inherits System.Web.UI.Page


    Protected Sub btnlogo_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnlogo.Click
        Dim constr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim file As String = ""
        If flU.HasFile Then
            file = flU.PostedFile.FileName
            Dim img As String = Right(file, 3)
            If UCase(img) = "GIF" Or UCase(img) = "JPG" Or UCase(img) = "PNG" Then
            Else
                lblMsg.Text = "Upload File should be image file"
                Exit Sub
            End If
            Dim i As Integer
            Dim byt() As Byte
            byt = flU.FileBytes
            i = byt.Length / 1024
            If i > 100 Then
                lblMsg.Text = "image size should not be greater then 100kb"
                Exit Sub
            End If
            Dim file1 As String
            file1 = Server.MapPath("logo\") & "logo" & Session("EID") & "." & img
            file = "logo" & Session("EID") & "." & img
            flU.PostedFile.SaveAs(file1)

            Dim con As SqlConnection = New SqlConnection(constr)
            Dim da As SqlDataAdapter = New SqlDataAdapter("update MMM_MST_ENTITY set logo='" & file & "' where EID=" & Session("EID"), con)
            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            da.SelectCommand.ExecuteNonQuery()
            lblMsg.Text = "Your Image has been changed."
            Session("logo") = file
            lblImage.Text = "<img src=""logo/" & Session("logo").ToString() & """ height=""150px"" width=""150px""  alt=""Me"" />"
            con.Dispose()
            da.Dispose()
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
    Protected Sub btnHdr_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnHdr.Click

        Dim constr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim file As String = ""
        If updHeader.HasFile Then
            file = updHeader.PostedFile.FileName
            Dim img As String = Right(file, 3)
            If UCase(img) = "GIF" Or UCase(img) = "JPG" Or UCase(img) = "PNG" Then
            Else
                lblheaderMsg.Text = "Upload File should be image file"
                Exit Sub
            End If
            Dim i As Integer
            Dim byt() As Byte
            byt = updHeader.FileBytes
            i = byt.Length / 1024
            If i > 100 Then
                lblheaderMsg.Text = "image size should not be greater then 100kb"
                Exit Sub
            End If
            Dim file1 As String
            file1 = Server.MapPath("logo\") & "Header" & Session("EID") & "." & img
            file = "Header" & Session("EID") & "." & img
            updHeader.PostedFile.SaveAs(file1)

            Dim con As SqlConnection = New SqlConnection(constr)
            Dim da As SqlDataAdapter = New SqlDataAdapter("update MMM_MST_ENTITY set headerimage='" & file & "' where EID=" & Session("EID"), con)
            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            da.SelectCommand.ExecuteNonQuery()
            lblheaderMsg.Text = "Your Image has been changed."
            Session("Headerimage") = file
            lblHdr.Text = "<img src=""logo/" & Session("Headerimage").ToString() & """ height=""150px"" width=""150px""  alt=""Me"" />"
            con.Dispose()
            da.Dispose()
        End If
    End Sub

    Protected Sub btnHdrStrp_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnHdrStrp.Click

        Dim constr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim file As String = ""
        If updHstrip.HasFile Then
            file = updHstrip.PostedFile.FileName
            Dim img As String = Right(file, 3)
            If UCase(img) = "GIF" Or UCase(img) = "JPG" Or UCase(img) = "PNG" Then
            Else
                lblheaderMsg.Text = "Upload File should be image file"
                Exit Sub
            End If
            Dim i As Integer
            Dim byt() As Byte
            byt = updHstrip.FileBytes
            i = byt.Length / 1024
            If i > 100 Then
                lblMsg.Text = "image size should not be greater then 100kb"
                Exit Sub
            End If
            Dim file1 As String
            file1 = Server.MapPath("logo\") & "Strip" & Session("EID") & "." & img
            file = "Strip" & Session("EID") & "." & img
            updHstrip.PostedFile.SaveAs(file1)

            Dim con As SqlConnection = New SqlConnection(constr)
            Dim da As SqlDataAdapter = New SqlDataAdapter("update MMM_MST_ENTITY set headerstrip='" & file & "' where EID=" & Session("EID"), con)
            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            da.SelectCommand.ExecuteNonQuery()
            lblstrp.Text = "Your Image has been changed."
            Session("Strip") = file
            lblHdrStrp.Text = "<img src=""logo/" & Session("Strip").ToString() & """ height=""150px"" width=""150px""  alt=""Me"" />"
            con.Dispose()
            da.Dispose()
        End If
    End Sub
End Class
