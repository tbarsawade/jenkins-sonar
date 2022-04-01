Imports Microsoft.VisualBasic
Imports System.Drawing
Imports System.Security.Cryptography
Imports System.Web
Imports ZXing
Imports System.Net
Imports System.IO
Imports ImageMagick
Imports Ghostscript.NET.Rasterizer
Imports System.Drawing.Imaging
Imports Inlite.ClearImageNet
Public Class QRCodeProcessor

    Public Function ProcessEInvoice(ByVal fileData As String, ByVal fileType As String, ByRef hashMatched As Boolean, ByRef innerError As String, ByRef qrCodeProcessed As Boolean) As InvoiceDetail
        If fileData Is Nothing Then
            Throw New ArgumentException("Input E-Invoice file data is null")
        End If
        hashMatched = False
        Dim recorderedError As String = String.Empty
        Dim fileHasBarCode = False
        If fileData Is Nothing Then
            Throw New ArgumentException("Input E-Invoice file data is null")
        End If

        Dim reader As Inlite.ClearImageNet.BarcodeReader = Nothing
        Dim invoiceDetail As InvoiceDetail = Nothing
        Dim file As Byte() = Convert.FromBase64String(fileData)
        Using stream As MemoryStream = New MemoryStream(file)
            Try
                reader = New Inlite.ClearImageNet.BarcodeReader()
                reader.Ean8 = True
                reader.Code128 = True
                reader.Code39 = True
                reader.Code32 = True
                reader.Upce = True
                reader.Upca = True
                reader.Pdf417 = True
                reader.Auto1D = True
                Dim barcodes As Barcode() = reader.Read(stream)
                For Each barcode As Barcode In barcodes
                    fileHasBarCode = True
                    Try
                        Dim token = New System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler().ReadJwtToken(barcode.Text)
                        Dim payloadData = token.Payload("data").ToString()
                        Try
                            Dim payLoadobj = Newtonsoft.Json.Linq.JObject.Parse(payloadData)
                            invoiceDetail = New InvoiceDetail(payLoadobj)
                            Dim hash = ComputeSha256Hash(invoiceDetail)
                            hashMatched = invoiceDetail.IRN = hash
                            qrCodeProcessed = True
                            Exit For
                        Catch ex As Exception
                            recorderedError = "Error in parsing QR Cod"
                            innerError = recorderedError

                            'Throw New Exception("Error in parsing QR Code: " + ex.Message)
                        End Try
                    Catch ex As Exception
                        recorderedError = "Error in extracting QR code from token"
                        innerError = recorderedError

                        'Throw New Exception("Error in extracting QR code from token: " + ex.Message)
                    End Try
                Next
                If Not qrCodeProcessed Then
                    recorderedError = "No QR Code found. "
                    innerError = recorderedError
                End If
            Catch ex As Exception
                recorderedError = "No BarCode found in file "
                innerError = recorderedError

                'Throw New Exception("No BarCode found in file:  " + ex.Message)
            Finally
                If reader IsNot Nothing Then
                    reader.Dispose()
                End If
            End Try

            If recorderedError IsNot String.Empty Then
                innerError = recorderedError
                invoiceDetail = ProcessEInvoice_Zxing(file, fileType, hashMatched, innerError, qrCodeProcessed)
                If qrCodeProcessed Then
                    innerError = String.Empty
                End If
                If Not Equals(innerError, String.Empty) Then
                    recorderedError = "Error from Inlite: " & recorderedError & " Error from Zxing processor: " & innerError
                    innerError = innerError 'recorderedError
                Else
                    If qrCodeProcessed Then
                        recorderedError = String.Empty
                    End If
                End If
            End If

        End Using
        Return invoiceDetail
    End Function

    Public Function ProcessEInvoice_Zxing(ByVal fileData As Byte(), ByVal fileType As String, ByRef hashMatched As Boolean, ByRef innerError As String, ByRef qrCodeProcessed As Boolean) As InvoiceDetail
        hashMatched = False
        Dim file As Byte() = fileData ' Convert.FromBase64String(fileData)
        System.Web.HttpContext.Current.Session("file") = file
        Dim contentType As String = "application/pdf"

        Try
            If Not (String.Equals(fileType, "image/jpg", StringComparison.OrdinalIgnoreCase) OrElse String.Equals(fileType, "image/png", StringComparison.OrdinalIgnoreCase)) Then
                Dim settings As MagickReadSettings = New MagickReadSettings()
                settings.Density = New Density(500, 500)
                Dim images As MagickImageCollection = New MagickImageCollection(file)
                images.Read(file, settings)
                settings.SetDefine(MagickFormat.Png, "size", "320x320")
                For Each image As MagickImage In images
                    settings.Height = image.Height
                    settings.Width = image.Width

                    Using stream As MemoryStream = New MemoryStream()
                        Try
                            image.Write(stream, MagickFormat.Png)
                        Catch ex As Exception
                            Throw New Exception("image can not be written")
                        End Try
                        Try
                            Dim barcodeReader As ZXing.BarcodeReader = New ZXing.BarcodeReader With {.AutoRotate = True}
                            Dim ZX As New ZXing.BarcodeWriter
                            barcodeReader.Options.PossibleFormats = New List(Of BarcodeFormat)
                            Dim options = New ZXing.QrCode.QrCodeEncodingOptions
                            options.PureBarcode = False
                            barcodeReader.Options.TryHarder = True
                            barcodeReader.Options.ReturnCodabarStartEnd = True
                            barcodeReader.Options.PureBarcode = False

                            ZX.Format = ZXing.BarcodeFormat.QR_CODE
                            ZX.Format = BarcodeFormat.CODE_128
                            ZX.Format = BarcodeFormat.UPC_E
                            ZX.Format = BarcodeFormat.UPC_A
                            ZX.Format = BarcodeFormat.CODE_39
                            ZX.Format = BarcodeFormat.PDF_417
                            ZX.Format = BarcodeFormat.All_1D
                            ZX.Format = BarcodeFormat.EAN_13
                            ZX.Format = BarcodeFormat.CODE_93
                            ZX.Format = BarcodeFormat.ITF
                            ZX.Format = BarcodeFormat.RSS_14
                            ZX.Format = BarcodeFormat.RSS_EXPANDED
                            ZX.Format = BarcodeFormat.DATA_MATRIX
                            ZX.Format = BarcodeFormat.AZTEC
                            ZX.Format = BarcodeFormat.MAXICODE

                            Dim barcodeBitmap As Bitmap = CType(System.Drawing.Image.FromStream(stream), Bitmap)
                            Dim barcodeResult = barcodeReader.Decode(barcodeBitmap)

                            If barcodeResult IsNot Nothing Then

                                Try
                                    Dim token = New System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler().ReadJwtToken(barcodeResult.Text)
                                    System.Web.HttpContext.Current.Session("token") = token

                                    Dim payloadData = token.Payload("data").ToString()
                                    System.Web.HttpContext.Current.Session("payloadData") = payloadData

                                    Dim payLoadobj = Newtonsoft.Json.Linq.JObject.Parse(payloadData)

                                    Dim invoiceDetail = New InvoiceDetail(payLoadobj)
                                    System.Web.HttpContext.Current.Session("invoiceDetail") = invoiceDetail
                                    System.Web.HttpContext.Current.Session("IRN") = invoiceDetail.IRN

                                    Dim hash = ComputeSha256Hash(invoiceDetail)
                                    hashMatched = invoiceDetail.IRN = hash
                                    qrCodeProcessed = True
                                    Return invoiceDetail
                                Catch ex As ArgumentException
                                Catch format As ZXing.FormatException
                                    innerError = "Error in processing of QR Code"
                                    'Throw New Exception("Error in processing of QR Code")

                                Catch ex As Exception
                                    innerError = "Error in extracting QR code from token"

                                End Try
                            End If
                        Catch ex As Exception
                            innerError = "Error in Decoding QR Code"
                            'Throw New Exception("Error in Decoding QR Code")
                        End Try
                    End Using
                Next
            End If
        Catch ex As Exception
            innerError = "Error Converting PDF to Image"
            'Throw New Exception("Error Converting PDF to Image")
        End Try
        Return Nothing
    End Function
    Private Function ComputeSha256Hash(ByVal data As InvoiceDetail) As String
        Dim hashInput = data.SellerGstin & DateTime.Now.Year & "-21" + data.DocType + data.DocNo

        Using sha256Hash As SHA256 = SHA256.Create()
            Dim bytes As Byte() = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(hashInput))
            Dim builder As StringBuilder = New StringBuilder()

            For i As Integer = 0 To bytes.Length - 1
                builder.Append(bytes(i).ToString("x2"))
            Next

            Return builder.ToString()
        End Using
    End Function

End Class