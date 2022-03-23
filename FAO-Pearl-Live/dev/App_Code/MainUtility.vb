Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports System.Security.Cryptography

Public Class MainUtility
    Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString

    Public Function ToCSV(ByVal dataTable As DataTable) As String
        'create the stringbuilder that would hold the data
        Dim sb As New StringBuilder()
        'check if there are columns in the datatable
        If dataTable.Columns.Count <> 0 Then
            'loop thru each of the columns for headers
            For Each column As DataColumn In dataTable.Columns
                'append the column name followed by the separator
                sb.Append(column.ColumnName + "^"c)

            Next
            sb.Remove(sb.Length - 1, 1)
            'append a carriage return
            sb.Append(vbCr & vbLf)

            'loop thru each row of the datatable
            For Each row As DataRow In dataTable.Rows
                'loop thru each column in the datatable

                For Each column As DataColumn In dataTable.Columns
                    'get the value for the row on the specified column
                    ' and append the separator
                    sb.Append(row(column).ToString() + "^"c)
                Next
                sb.Remove(sb.Length - 1, 1)
                'append a carriage return
                sb.Append(vbCr & vbLf)
            Next
        End If
        Return sb.ToString()
    End Function
    Public Function AmtInWord(ByVal Num As Decimal) As String
        'I have created this function for converting amount in indian rupees (INR). 
        'You can manipulate as you wish like decimal setting, Doller (any currency) Prefix.

        Dim strNum As String
        Dim strNumDec As String
        Dim StrWord As String
        strNum = Num

        If InStr(1, strNum, ".") <> 0 Then
            strNumDec = Mid(strNum, InStr(1, strNum, ".") + 1)

            If Len(strNumDec) = 1 Then
                strNumDec = strNumDec + "0"
            End If
            If Len(strNumDec) > 2 Then
                strNumDec = Mid(strNumDec, 1, 2)
            End If

            strNum = Mid(strNum, 1, InStr(1, strNum, ".") - 1)
            StrWord = IIf(CDbl(strNum) = 1, " Rupee ", " Rupees ") + NumToWord(CDbl(strNum)) + IIf(CDbl(strNumDec) > 0, " and Paise" + cWord3(CDbl(strNumDec)), "")
        Else
            StrWord = IIf(CDbl(strNum) = 1, " Rupee ", " Rupees ") + NumToWord(CDbl(strNum))
        End If
        AmtInWord = StrWord & " Only"
        Return AmtInWord
    End Function
    Function strReplicate(ByVal str As String, ByVal intD As Integer) As String
        'This fucntion padded "0" after the number to evaluate hundred, thousand and on....
        'using this function you can replicate any Charactor with given string.
        Dim i As Integer
        strReplicate = ""
        For i = 1 To intD
            strReplicate = strReplicate + str
        Next
        Return strReplicate
    End Function
    Function NumToWord(ByVal Num As Decimal) As String
        'I divided this function in two part.
        '1. Three or less digit number.
        '2. more than three digit number.
        Dim strNum As String
        Dim StrWord As String
        strNum = Num

        If Len(strNum) <= 3 Then
            StrWord = cWord3(CDbl(strNum))
        Else
            StrWord = cWordG3(CDbl(Mid(strNum, 1, Len(strNum) - 3))) + " " + cWord3(CDbl(Mid(strNum, Len(strNum) - 2)))
        End If
        NumToWord = StrWord
    End Function
    Function cWordG3(ByVal Num As Decimal) As String
        '2. more than three digit number.
        Dim strNum As String = ""
        Dim StrWord As String = ""
        Dim readNum As String = ""
        strNum = Num
        If Len(strNum) Mod 2 <> 0 Then
            readNum = CDbl(Mid(strNum, 1, 1))
            If readNum <> "0" Then
                StrWord = retWord(readNum)
                readNum = CDbl("1" + strReplicate("0", Len(strNum) - 1) + "000")
                StrWord = StrWord + " " + retWord(readNum)
            End If
            strNum = Mid(strNum, 2)
        End If
        While Not Len(strNum) = 0
            readNum = CDbl(Mid(strNum, 1, 2))
            If readNum <> "0" Then
                StrWord = StrWord + " " + cWord3(readNum)
                readNum = CDbl("1" + strReplicate("0", Len(strNum) - 2) + "000")
                StrWord = StrWord + " " + retWord(readNum)
            End If
            strNum = Mid(strNum, 3)
        End While
        cWordG3 = StrWord
        Return cWordG3
    End Function
    Function cWord3(ByVal Num As Decimal) As String
        '1. Three or less digit number.
        Dim strNum As String = ""
        Dim StrWord As String = ""
        Dim readNum As String = ""
        If Num < 0 Then Num = Num * -1
        strNum = Num

        If Len(strNum) = 3 Then
            readNum = CDbl(Mid(strNum, 1, 1))
            StrWord = retWord(readNum) + " Hundred"
            strNum = Mid(strNum, 2, Len(strNum))
        End If

        If Len(strNum) <= 2 Then
            If CDbl(strNum) >= 0 And CDbl(strNum) <= 20 Then
                StrWord = StrWord + " " + retWord(CDbl(strNum))
            Else
                StrWord = StrWord + " " + retWord(CDbl(Mid(strNum, 1, 1) + "0")) + " " + retWord(CDbl(Mid(strNum, 2, 1)))
            End If
        End If

        strNum = CStr(Num)
        cWord3 = StrWord
        Return cWord3
    End Function
    Function retWord(ByVal Num As Decimal) As String
        'This two dimensional array store the primary word convertion of number.
        retWord = ""
        Dim ArrWordList(,) As Object = {{0, ""}, {1, "One"}, {2, "Two"}, {3, "Three"}, {4, "Four"}, _
                                        {5, "Five"}, {6, "Six"}, {7, "Seven"}, {8, "Eight"}, {9, "Nine"}, _
                                        {10, "Ten"}, {11, "Eleven"}, {12, "Twelve"}, {13, "Thirteen"}, {14, "Fourteen"}, _
                                        {15, "Fifteen"}, {16, "Sixteen"}, {17, "Seventeen"}, {18, "Eighteen"}, {19, "Nineteen"}, _
                                        {20, "Twenty"}, {30, "Thirty"}, {40, "Forty"}, {50, "Fifty"}, {60, "Sixty"}, _
                                        {70, "Seventy"}, {80, "Eighty"}, {90, "Ninety"}, {100, "Hundred"}, {1000, "Thousand"}, _
                                        {100000, "Lakh"}, {10000000, "Crore"}}

        Dim i As Integer
        For i = 0 To UBound(ArrWordList)
            If Num = ArrWordList(i, 0) Then
                retWord = ArrWordList(i, 1)
                Exit For
            End If
        Next
        Return retWord
    End Function
    Public Function uploadImage(ByVal flu As FileUpload) As String

        Dim imgStream As System.IO.Stream
        Dim imgPhoto As System.Drawing.Image

        'Capture the image's stream
        imgStream = flu.PostedFile.InputStream
        'Create an image (which we will manipulate) based on the stream
        imgPhoto = System.Drawing.Image.FromStream(imgStream)

        'The image must fit within a 100x100 pixel box
        Const maxHeight As Integer = 100
        Const maxWidth As Integer = 100

        Dim imgHeight As Integer = imgPhoto.Height
        Dim imgWidth As Integer = imgPhoto.Width
        Dim imgScaleFactor As Double = 1

        If imgHeight > maxHeight Then
            imgScaleFactor = maxHeight / imgHeight
        End If
        If (imgWidth > maxWidth) And (maxWidth / imgWidth < imgScaleFactor) Then
            'The image is wider than it is tall
            imgScaleFactor = maxWidth / imgWidth
        End If

        'Resize the image based on the calculated scale factor
        imgPhoto = imgPhoto.GetThumbnailImage(imgWidth * imgScaleFactor, imgHeight * imgScaleFactor, Nothing, Nothing)

        Dim extension As String, fileName As String
        fileName = flu.PostedFile.FileName
        'Retrieve the extension of the uploaded file
        extension = Mid(fileName, InStrRev(fileName, ".") + 1, fileName.Length)

        'Save the file to the server
        'Note: you'll want to change the path and image name
        Dim fname As String = Now.Year & Now.Month & Now.Day & Now.Hour & Now.Minute & Now.Second & Now.Millisecond & "." & extension
        imgPhoto.Save(System.Web.HttpContext.Current.Server.MapPath("imgfolder") & "/" & fname)
        Return fname

    End Function

    Public Function DateTimeToEpoch(ByVal DateTimeValue As Date) As Integer
        Try
            Return CInt(DateTimeValue.Subtract(CDate("1.1.1970 00:00:00")).TotalSeconds)
        Catch ex As System.OverflowException
            Return -1
        End Try
    End Function

    Public Function Md5(ByVal strChange As String) As String
        Dim pass() As Byte = Encoding.UTF8.GetBytes(strChange)
        Dim md As MD5 = New MD5CryptoServiceProvider()
        md.ComputeHash(pass)
        Dim strPassword As String = ByteArrayToHexString(md.Hash)
        Return strPassword
    End Function

    Public Function ByteArrayToHexString(ByVal Bytes As Byte()) As String
        Dim result As StringBuilder
        Dim HexAlphabet As String = "0123456789abcdef"
        result = New StringBuilder()

        For Each B As Byte In Bytes
            result.Append(HexAlphabet(CInt(B >> 4)))
            result.Append(HexAlphabet(CInt(B And &HF)))
        Next

        Return result.ToString()
    End Function


End Class
