Imports System.Net
Imports System.Net.Mail
Imports System.Threading
Imports System.IO
Imports System.Data
Imports System.Data.SqlClient
Imports System.Management
Imports System.Xml
Imports System
Imports System.Net.Security
Imports System.Security.Cryptography.X509Certificates
Imports System.Collections.Specialized

Partial Class palmynd
    Inherits System.Web.UI.Page

    Private Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Try
            Dim st1 As String = "<ENVELOPE><HEADER><TALLYREQUEST>Register</TALLYREQUEST></HEADER><BODY><DATA><LGBILLINGCODE>IN013205004B</LGBILLINGCODE><LGOTP>P123#</LGOTP></DATA></BODY></ENVELOPE>"
            Dim st2 As String = "<ENVELOPE><HEADER><TALLYREQUEST>Confirm</TALLYREQUEST></HEADER><BODY><VALIDATION><LGBILLINGCODE>IN013205004B</LGBILLINGCODE><LGPASS>xxx</LGPASS></VALIDATION><DATA><CONFIRM>Yes</CONFIRM></DATA></BODY></ENVELOPE>"

            Dim xd As New XmlDocument

            Dim sr As New StreamReader(Request.InputStream)
            Response.Write("Exit from Main")
            Exit Sub

            xd.LoadXml(sr.ReadToEnd())
            'xd.LoadXml(st2);


            Dim Result As String = OutwardPAL(xd.InnerXml)


            'If xd.SelectSingleNode("//ENVELOPE/HEADER/TALLYREQUEST").InnerText = "Register" Then
            '    ' string st3 = xd.SelectSingleNode("//ENVELOPE/BODY").OuterXml;
            '    ' Response.Write(xd.SelectSingleNode("//ENVELOPE/BODY").OuterXml);
            '    Response.Write(RegisterDistributor(xd.SelectSingleNode("//ENVELOPE/BODY").OuterXml))
            'ElseIf xd.SelectSingleNode("//ENVELOPE/HEADER/TALLYREQUEST").InnerText = "Confirm" Then
            '    'string st4 = xd.SelectSingleNode("//ENVELOPE/BODY").OuterXml;
            '    Response.Write(ConfirmDistributor(xd.SelectSingleNode("//ENVELOPE/BODY").OuterXml))
            'End If
        Catch
            Response.Clear()
            Response.ClearContent()
            Response.Write("<ERROR>Wrong request in main </ERROR>")
        End Try
        Response.[End]()

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


    Function OutwardPAL(Data As String) As String
        Dim Result = ""
        Dim Key As String = "", EID As String = "", DocType As String = "", UID As String = 0
        Try
            '' here code to read xml for doc type and field name and create xml output string and retun xml string
            'Convert steam into string 
            'Dim reader As New StreamReader(Data)
            Dim strData As String = Data
            'Decode it to UTF-8
            'strData = HttpUtility.UrlDecode(strData)
            'Dim Data1 As New StringBuilder(strData)
            'Save string into database
            'SaveServicerequest

            Dim strinput As String = ReturnInputParaValues(strData)

            Dim strinputArr() As String = strinput.Split("|")

            Result = "Not Called"
            If strinputArr.Length <> 2 Then
                ErrorLog.sendMail("BPMCustomWS.OutwardPAL", "INPUT PARAMETERS NOT CORRECT")
            Else
                Result = POrder(strinputArr(0).ToString, strinputArr(1).ToString)
            End If
            ' CommanUtil.SaveServicerequest(Data, "BPMCustomWS", "OutwardPal", Result)
        Catch ex As Exception
            ErrorLog.sendMail("BPMCustomWS.OutwardPAL", ex.Message)
            Return "RTO"
        End Try
        Return Result
    End Function

    'Function OutwardPAL(Data As Stream) As XmlDocument Implements IBPMCustomWS.OutwardPAL
    '    Dim Result As String = ""
    '    Dim resXML As New XmlDocument
    '    Dim Key As String = "", EID As String = "", DocType As String = "", UID As String = 0
    '    Try
    '        '' here code to read xml for doc type and field name and create xml output string and retun xml string
    '        'Convert steam into string 
    '        Dim reader As New StreamReader(Data)
    '        Dim strData As String = reader.ReadToEnd()
    '        'Decode it to UTF-8
    '        strData = HttpUtility.UrlDecode(strData)
    '        Dim Data1 As New StringBuilder(strData)
    '        'Save string into database
    '        'SaveServicerequest

    '        Dim strinput As String = ReturnInputParaValues(strData)

    '        Dim strinputArr() As String = strinput.Split("|")

    '        Result = "<ERROR>Not Called</ERROR>"
    '        If strinputArr.Length <> 2 Then
    '            ErrorLog.sendMail("BPMCustomWS.OutwardPAL", "INPUT PARAMETERS NOT CORRECT")
    '        Else
    '            Result = POrder(strinputArr(0).ToString, strinputArr(1).ToString)
    '        End If

    '        resXML.LoadXml(Result)
    '        CommanUtil.SaveServicerequest(Data1, "BPMCustomWS", "OutwardPal", Result)
    '    Catch ex As Exception
    '        ErrorLog.sendMail("BPMCustomWS.OutwardPAL", ex.Message)
    '        resXML.LoadXml("<ERROR>RTO</ERROR>")
    '        Return resXML
    '    End Try
    '    Return resXML
    'End Function

    Function POrder(ByVal Dtype As String, ByVal DistCode As String) As String
        ' Dtype = "Purchase Document"
        ' DistCode = "0001"
        Dim conStr As String = "server=172.17.109.152;initial catalog=DMS;uid=DMS;pwd=Ztsu93#u"
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("InsertSMSALERTLog", con)
        Dim doc As XmlDocument = New XmlDocument()
        Dim Vdt As New DataTable
        Dim SITdt As New DataTable
        Dim Invtrdt As New DataTable
        Dim ds As New DataSet
        oda.SelectCommand.CommandText = "select tid[DocID],dms.udf_split('MASTER-Distributor-fld1',fld10)[Distributor Name],fld11[Distributor Code],fld12[STATE],replace(fld13,'/','-')[BILLDATE],fld14[SupplierInvoiceNo],fld15[VENDORNAME],fld16[VENDORADDRESS],fld17[VENDORTINNO.],isnull(fld18,0)[GROSSAMOUNT],isnull(fld2,0)[TAX],isnull(fld21,0)[TOTALBILLAMOUNT] from mmm_mst_doc where eid=43 and documenttype='" & Dtype & "' and fld11='" & DistCode & "'"
        oda.Fill(Vdt)

        If Vdt.Rows.Count < 1 Then
            Return "Data Not Found"
        End If
        'Dim root As XmlElement
        'root = doc.CreateElement("ROOT")
        'doc.AppendChild(root)
        Dim strB As StringBuilder = New System.Text.StringBuilder()
        strB.Append("<ENVELOPE>")
        strB.Append("<HEADER>")
        strB.Append("<RESPONSE>" & "IMPORT DATA")
        strB.Append("</RESPONSE>")
        strB.Append("</HEADER>")
        strB.Append("<BODY>")
        strB.Append("<DATA>")
        strB.Append("<VALIDATION>")
        strB.Append("<BILLINGCODE>" & "IN013205004B")
        strB.Append("</BILLINGCODE>")
        strB.Append("<PASSWD>" & "XXXXX")
        strB.Append("</PASSWD>")
        strB.Append("</VALIDATION>")
        strB.Append("<IMPORTDATA>")
        For i = 0 To Vdt.Rows.Count - 1
            strB.Append("<TALLYMESSAGE>")
            strB.Append("<VOUCHER>")
            oda.SelectCommand.CommandText = "select dms.udf_split('MASTER-Item-fld1',dt.fld1)[Item],m.fld10[Item_Code-SKU],m.fld25[Batch],dms.udf_split('MASTER-ItemGroup1-fld1',m.fld12)[Product Group],dms.udf_split('MASTER-Units-fld1',m.fld16)[Primary UOM] from mmm_mst_doc_item dt inner join mmm_mst_master m on m.tid=dt.fld1 where docid=" & Vdt.Rows(i).Item(0).ToString & ""
            oda.Fill(SITdt)
            oda.SelectCommand.CommandText = "select dms.udf_split('MASTER-Item-fld1',dt.fld1)[ITEMNAME],dt.fld10[BATCHNAME],dt.fld11[Quantity],dms.udf_split('MASTER-Units-fld1',dt.fld15)[UOM],dt.fld12[RATE],dt.fld13[Product Discount],dt.fld14[Amount] from mmm_mst_doc_item dt where docid=" & Vdt.Rows(i).Item(0).ToString & ""
            oda.Fill(Invtrdt)
            For j = 0 To Vdt.Columns.Count - 1
                If j = 0 Then
                Else
                    strB.Append("<" & Vdt.Columns(j).ColumnName.Replace(" ", "").ToUpper & ">" & Vdt.Rows(i).Item(j).ToString() & "</" & Vdt.Columns(j).ColumnName.Replace(" ", "").ToUpper & ">")
                End If
            Next
            For k = 0 To SITdt.Rows.Count - 1
                strB.Append("<STOCKITEM>")
                For j = 0 To SITdt.Columns.Count - 1
                    strB.Append("<" & SITdt.Columns(j).ColumnName.Replace(" ", "").ToUpper & ">" & SITdt.Rows(k).Item(j).ToString() & "</" & SITdt.Columns(j).ColumnName.Replace(" ", "").ToUpper & ">")
                Next
                strB.Append("</STOCKITEM>")
            Next
            For d = 0 To Invtrdt.Rows.Count - 1
                strB.Append("<INVENTORYENTRIES>")
                For j = 0 To Invtrdt.Columns.Count - 1
                    strB.Append("<" & Invtrdt.Columns(j).ColumnName.Replace(" ", "").ToUpper & ">" & Invtrdt.Rows(d).Item(j).ToString() & "</" & Invtrdt.Columns(j).ColumnName.Replace(" ", "").ToUpper & ">")
                Next
                strB.Append("</INVENTORYENTRIES>")
            Next
            strB.Append("</VOUCHER>")
            strB.Append("</TALLYMESSAGE>")
        Next
        strB.Append("</IMPORTDATA>")
        strB.Append("</DATA>")
        strB.Append("</BODY>")
        strB.Append("</ENVELOPE>")
        Return strB.ToString
    End Function



    Public Function AuthenticateWSRequest(Key As String) As DataSet
        Dim ds As New DataSet()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = Nothing
        Dim da As SqlDataAdapter = Nothing
        Try
            con = New SqlConnection(conStr)
            da = New SqlDataAdapter("AuthenticateWSRequest", con)
            da.SelectCommand.CommandType = CommandType.StoredProcedure
            da.SelectCommand.Parameters.AddWithValue("@APIKey", Key)
            da.Fill(ds)
        Catch ex As Exception
            Throw
        Finally
            If Not con Is Nothing Then
                con.Close()
                con.Dispose()
            End If
            If Not da Is Nothing Then
                da.Dispose()
            End If
        End Try
        Return ds
    End Function

    Public Function ReturnInputParaValues(ByVal str As String) As String
        Dim xmlDocRead As New XmlDocument()
        xmlDocRead.LoadXml(str)
        Dim ReturnVal As String = "NA"
        If xmlDocRead.ChildNodes.Count >= 1 Then
            Dim SelNodesTxt As String = xmlDocRead.DocumentElement.Name
            Dim nodes As XmlNodeList = xmlDocRead.SelectNodes(SelNodesTxt)

            For Each node As XmlNode In nodes
                For c As Integer = 0 To node.ChildNodes.Count - 1
                    If UCase(node.ChildNodes.Item(c).Name) = "BODY" Then
                        For C1 As Integer = 0 To node.Item("BODY").ChildNodes.Count - 1
                            If node.Item("BODY").ChildNodes.Item(C1).Name = "DATA" Then
                                For c2 As Integer = 0 To node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Count - 1
                                    If node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).Name = "REQUESTDATA" Then
                                        For P As Integer = 0 To node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Count - 1
                                            If UCase(node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).Name) = "TALLYMESSAGE" Then
                                                For d As Integer = 0 To node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).ChildNodes.Count - 1
                                                    If UCase(node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).ChildNodes.Item(d).Name) = "DOCUMENTTYPE" Then
                                                        ReturnVal = node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).ChildNodes.Item(d).InnerText & "|"
                                                    ElseIf UCase(node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).ChildNodes.Item(d).Name) = "DISTRIBUTORCODE" Then
                                                        ReturnVal &= node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).ChildNodes.Item(d).InnerText
                                                    End If
                                                Next
                                            End If
                                        Next
                                    End If
                                Next
                            End If
                        Next
                    End If
                Next
            Next
        End If
        Return ReturnVal
    End Function


    Public Function readxmlandgivestring(ByVal str As String) As String
        Dim result As String = String.Empty
        Dim xmlDocRead As New XmlDocument()
        xmlDocRead.LoadXml(str)
        If xmlDocRead.ChildNodes.Count >= 1 Then

            Dim SelNodesTxt As String = xmlDocRead.DocumentElement.Name
            Dim Cnt As Integer = 0
            Dim nodes As XmlNodeList = xmlDocRead.SelectNodes(SelNodesTxt)
            For Each node As XmlNode In nodes
                Cnt += 1
                Dim MainVar As String = String.Empty
                Dim GrdCode As String = ""

                Dim varINTALL As Double = 0
                Dim varBASIC As Double = 0
                Dim varHRA As Double = 0
                Dim varSODCOP As Double = 0
                Dim varCONV As Double = 0
                Dim varLTA As Double = 0
                Dim varMSCALL As Double = 0
                Dim varCTC_PF As Double = 0
                Dim varSPLALL As Double = 0
                Dim varMED As Double = 0
                Dim varCompCode As String = ""
                Dim globalVar As String = String.Empty
                Dim ChildVar As String = String.Empty
                For c As Integer = 0 To node.ChildNodes.Count - 1
                    If UCase(node.ChildNodes.Item(c).Name) = "BODY" Then
                        For C1 As Integer = 0 To node.Item("BODY").ChildNodes.Count - 1
                            If node.Item("BODY").ChildNodes.Item(C1).Name = "DATA" Then
                                For c2 As Integer = 0 To node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Count - 1
                                    If node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).Name = "REQUESTDATA" Then
                                        For P As Integer = 0 To node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Count - 1
                                            If UCase(node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).Name) = "TALLYMESSAGE" Then
                                                For d As Integer = 0 To node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).ChildNodes.Count - 1
                                                    If UCase(node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).ChildNodes.Item(d).Name) = "DISTRIBUTORNAME" Then
                                                        globalVar = "Distributor Name" & "::" & node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).ChildNodes.Item(d).InnerText & "|"
                                                    ElseIf UCase(node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).ChildNodes.Item(d).Name) = "DISTRIBUTORCODE" Then
                                                        globalVar = globalVar & "Distributor Code" & "::" & node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).ChildNodes.Item(d).InnerText & "|"
                                                    ElseIf UCase(node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).ChildNodes.Item(d).Name) = "SHIPTOPARTY" Then
                                                        globalVar = globalVar & "Ship to party" & "::" & node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).ChildNodes.Item(d).InnerText & "|"
                                                    ElseIf UCase(node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).ChildNodes.Item(d).Name) = "RETAILER" Then
                                                        globalVar = globalVar & "Retailer" & "::" & node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).ChildNodes.Item(d).InnerText & "|"
                                                    ElseIf UCase(node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).ChildNodes.Item(d).Name) = "INVOICENO." Then
                                                        globalVar = globalVar & "Invoice No." & "::" & node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).ChildNodes.Item(d).InnerText & "|"
                                                    ElseIf UCase(node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).ChildNodes.Item(d).Name) = "INVOICEDATE" Then
                                                        globalVar = globalVar & "Invoice date" & "::" & node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).ChildNodes.Item(d).InnerText & "|"
                                                    ElseIf UCase(node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).ChildNodes.Item(d).Name) = "BEAT" Then
                                                        globalVar = globalVar & "Beat" & "::" & node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).ChildNodes.Item(d).InnerText & "|"
                                                    ElseIf UCase(node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).ChildNodes.Item(d).Name) = "GROSSAMOUNT" Then
                                                        globalVar = globalVar & "Gross Amount" & "::" & node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).ChildNodes.Item(d).InnerText & "|"
                                                    ElseIf UCase(node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).ChildNodes.Item(d).Name) = "TAX" Then
                                                        globalVar = globalVar & "Tax" & "::" & node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).ChildNodes.Item(d).InnerText & "|"
                                                    ElseIf UCase(node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).ChildNodes.Item(d).Name) = "TOTALINVOICEAMOUNT" Then
                                                        globalVar = globalVar & "Total Invoice amount" & "::" & node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).ChildNodes.Item(d).InnerText & "|"
                                                    ElseIf UCase(node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).ChildNodes.Item(d).Name) = "DETAILSOFSALES" Then ''child item starts
                                                        ChildVar = "Details of Sales::{}"
                                                        For x As Integer = 0 To node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).ChildNodes.Item(d).ChildNodes.Count - 1
                                                            If UCase(node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).ChildNodes.Item(d).ChildNodes.Item(x).Name) = "ITEMNAME" Then
                                                                ChildVar = ChildVar & "()Item Name<>" & node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).ChildNodes.Item(d).ChildNodes.Item(x).InnerText
                                                            ElseIf UCase(node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).ChildNodes.Item(d).ChildNodes.Item(x).Name) = "BATCHNAME" Then
                                                                ChildVar = ChildVar & "()Batch Name<>" & node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).ChildNodes.Item(d).ChildNodes.Item(x).InnerText
                                                            ElseIf UCase(node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).ChildNodes.Item(d).ChildNodes.Item(x).Name) = "QUANTITY" Then
                                                                ChildVar = ChildVar & "()Quantity<>" & node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).ChildNodes.Item(d).ChildNodes.Item(x).InnerText
                                                            ElseIf UCase(node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).ChildNodes.Item(d).ChildNodes.Item(x).Name) = "UOM" Then
                                                                ChildVar = ChildVar & "()UOM<>" & node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).ChildNodes.Item(d).ChildNodes.Item(x).InnerText
                                                            ElseIf UCase(node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).ChildNodes.Item(d).ChildNodes.Item(x).Name) = "RATE" Then
                                                                ChildVar = ChildVar & "()Rate<>" & node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).ChildNodes.Item(d).ChildNodes.Item(x).InnerText
                                                            ElseIf UCase(node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).ChildNodes.Item(d).ChildNodes.Item(x).Name) = "PRODUCT DISCOUNT" Then
                                                                ChildVar = ChildVar & "()Product Discount<>" & node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).ChildNodes.Item(d).ChildNodes.Item(x).InnerText
                                                            ElseIf UCase(node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).ChildNodes.Item(d).ChildNodes.Item(x).Name) = "AMOUNT" Then
                                                                ChildVar = ChildVar & "()Amount<>" & node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).ChildNodes.Item(d).ChildNodes.Item(x).InnerText
                                                            End If

                                                        Next
                                                        ChildVar = ChildVar & "{}"
                                                    End If
                                                Next
                                            End If
                                        Next
                                    End If
                                Next
                            End If
                        Next
                    End If
                Next
                ChildVar = ChildVar.Remove(ChildVar.Length - 2)
                result = "Key$$GLDSOOKGBLH000391HSG ~DOCTYPE$$Sales Document~Data$$" & globalVar & ChildVar

            Next
        End If

        Return result
    End Function




End Class
