Imports System.IO
Imports System.Security
Imports System.Security.Cryptography
Imports System.Random
Imports System.Data
Imports System.Data.SqlClient

Partial Class EncriptionTest
    Inherits System.Web.UI.Page


    Protected Sub Button1_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Button1.Click

        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("select * from MMM_MST_USER where EID not in(3) ", con)
        oda.SelectCommand.CommandType = CommandType.Text
        Dim ds As New DataSet
        oda.Fill(ds, "data")
        Dim ob As New User

        If con.State <> ConnectionState.Open Then
            con.Open()
        End If

        For i As Integer = 0 To ds.Tables("data").Rows.Count - 1

            Dim paswd As String = ds.Tables("data").Rows(i).Item("pwd")

            Dim sKey As Integer
            Dim Generator As System.Random = New System.Random()
            sKey = Generator.Next(10000, 99999)

            Dim strPwd As String = ob.EncryptTripleDES(paswd, sKey)



            oda.SelectCommand.CommandText = "update MMM_MSt_USER set ENPW='" & strPwd & "' , sKEY='" & sKey & "' where uid='" & ds.Tables("data").Rows(i).Item("uid") & "' "

            oda.SelectCommand.ExecuteScalar()




        Next
        con.Close()
        oda.Dispose()

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
End Class
