
Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports System.Net
Imports System.Security.Cryptography

Partial Class SSOoutward
    Inherits System.Web.UI.Page

    Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString


    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        '' write here code to call client's sso api's 
        If Session("EID").ToString = "187" Then  ' for Comviva
            '' write here code to call comviva sso api.
            '' Redirect to received url if successful else show received error message in lblmsg label.
            Dim uid As String = Session("UID").ToString()
            Dim email = "sdk@gmail.com"
            Dim con As New SqlConnection(conStr)
            Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
            Dim ds As New DataSet()
            oda.SelectCommand.CommandText = "select * from  MMM_MST_USER where UID='" & uid & "' "
            oda.SelectCommand.CommandType = CommandType.Text
            oda.Fill(ds, "ssodt")
            Dim EmailID As String = ds.Tables("ssodt").Rows(0).Item("emailID").ToString()
            Dim key = "abc123"
            Dim para As String = encrypt(EmailID)


            'Dim s As String = "http://35.223.111.205/tax2win_gst_mvp/sso/get_link?data=" & para
            Try

                Dim wb = New WebClient()
                Dim data = New NameValueCollection()
                data("data") = para

                Dim response = wb.UploadValues("https://gst.tax2win.in/sso/get_link", "POST", data)
                Dim responseInString As String = Encoding.UTF8.GetString(response)
                Dim values As Newtonsoft.Json.Linq.JObject
                values = Newtonsoft.Json.Linq.JObject.Parse(responseInString)
                Dim jsonStatus As String = values("status").ToString
                Dim jsonData As String = values("url").ToString
                If jsonStatus.ToUpper = "SUCCESS" Then
                    HttpContext.Current.Response.Redirect(jsonData, False)
                Else
                    lblMsg.Visible = True
                    lblMsg.Text = "Some Error encountered. Please contact the admin"
                End If

            Catch ex As Exception
                Dim s As String = ex.ToString()

            End Try

        ElseIf Session("EID").ToString <> "187" Then  ' for Comviva
            '' do something here
        Else
            lblMsg.Visible = True
            lblMsg.Text = "SSO Details for this entity not found!"
        End If
    End Sub

    Public Shared Function encrypt(ByVal strToEncrypt As String) As String
        Dim key = MD5.Create().ComputeHash(Encoding.UTF8.GetBytes("wbXZA6TGOBbElU1u"))
        Dim iv = New Byte() {0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15}
        Dim result As String = ""

        Using New RijndaelManaged()
            Dim array As Byte() = EncryptStringToBytes(strToEncrypt, key, iv)
            Dim stringBuilder As StringBuilder = New StringBuilder()

            For Each b As Byte In array
                stringBuilder.AppendFormat("{0:x2}", b)
            Next

            result = stringBuilder.ToString()
        End Using

        Return result
    End Function
    Private Shared Function EncryptStringToBytes(ByVal plainText As String, ByVal Key As Byte(), ByVal IV As Byte()) As Byte()
        If plainText Is Nothing OrElse plainText.Length <= 0 Then
            Throw New ArgumentNullException("plainText")
        End If

        If Key Is Nothing OrElse Key.Length <= 0 Then
            Throw New ArgumentNullException("Key")
        End If

        If IV Is Nothing OrElse IV.Length <= 0 Then
            Throw New ArgumentNullException("Key")
        End If

        Dim result As Byte()

        Using rijndaelManaged As RijndaelManaged = New RijndaelManaged()
            rijndaelManaged.Key = Key
            rijndaelManaged.IV = IV
            Dim transform As ICryptoTransform = rijndaelManaged.CreateEncryptor(rijndaelManaged.Key, rijndaelManaged.IV)

            Using memoryStream As MemoryStream = New MemoryStream()

                Using cryptoStream As CryptoStream = New CryptoStream(memoryStream, transform, CryptoStreamMode.Write)

                    Using streamWriter As StreamWriter = New StreamWriter(cryptoStream)
                        streamWriter.Write(plainText)
                    End Using

                    result = memoryStream.ToArray()
                End Using
            End Using
        End Using

        Return result
    End Function
End Class
