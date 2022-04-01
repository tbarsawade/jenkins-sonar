Imports Microsoft.VisualBasic

Public Class EncryptionDescryption
    Public Function DecryptTripleDES(ByVal sOut As String, ByVal sKey As String) As String
        Dim DES As New System.Security.Cryptography.TripleDESCryptoServiceProvider
        Dim hashMD5 As New System.Security.Cryptography.MD5CryptoServiceProvider

        ' scramble the key
        sKey = ScrambleKey(sKey)
        ' Compute the MD5 hash.
        DES.Key = hashMD5.ComputeHash(System.Text.ASCIIEncoding.ASCII.GetBytes(sKey))
        ' Set the cipher mode.
        DES.Mode = System.Security.Cryptography.CipherMode.ECB
        ' Create the decryptor.
        Dim DESDecrypt As System.Security.Cryptography.ICryptoTransform = DES.CreateDecryptor()
        Dim Buffer As Byte() = Convert.FromBase64String(sOut)
        ' Transform and return the string.
        Return System.Text.ASCIIEncoding.ASCII.GetString(DESDecrypt.TransformFinalBlock(Buffer, 0, Buffer.Length))
    End Function
    Private Function ScrambleKey(ByVal v_strKey As String) As String
        Dim sbKey As New System.Text.StringBuilder
        Dim intPtr As Integer
        For intPtr = 1 To v_strKey.Length
            Dim intIn As Integer = v_strKey.Length - intPtr + 1
            sbKey.Append(Mid(v_strKey, intIn, 1))
        Next
        Dim strKey As String = sbKey.ToString
        Return sbKey.ToString
    End Function

    Public Function EncryptTripleDES(ByVal sIn As String, ByVal sKey As String) As String
        Dim DES As New System.Security.Cryptography.TripleDESCryptoServiceProvider
        Dim hashMD5 As New System.Security.Cryptography.MD5CryptoServiceProvider

        ' scramble the key
        sKey = ScrambleKey(sKey)
        ' Compute the MD5 hash.
        DES.Key = hashMD5.ComputeHash(System.Text.ASCIIEncoding.ASCII.GetBytes(sKey))
        ' Set the cipher mode.
        DES.Mode = System.Security.Cryptography.CipherMode.ECB
        ' Create the encryptor.
        Dim DESEncrypt As System.Security.Cryptography.ICryptoTransform = DES.CreateEncryptor()
        ' Get a byte array of the string.
        Dim Buffer As Byte() = System.Text.ASCIIEncoding.ASCII.GetBytes(sIn)
        ' Transform and return the string.
        Return Convert.ToBase64String(DESEncrypt.TransformFinalBlock(Buffer, 0, Buffer.Length))
    End Function
End Class
