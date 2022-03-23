Imports System.Data
Imports System.IO
Imports System.Web
Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.Xml

' To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line.
' <System.Web.Script.Services.ScriptService()> _
<WebService(Namespace:="http://tempuri.org/")>
<WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Public Class OCRInvoiceValue
    Inherits System.Web.Services.WebService

    <WebMethod()>
    Public Function OcrInvoiceValues(ByVal WORKITEMNUMBER As String, ByVal PROJECTNAME As String, VENDORNAME As String, ByVal UNIQUEREFERENCENUMBER As String, ByVal BARCODE As String, ByVal VENDORCODE As String, ByVal COMPANYCODE As String, ByVal CURRENCY As String, ByVal INVOICENUMBER As String, ByVal TOTALINVOICEAMOUNT As Double, ByVal FILENAME As String) As XmlDocument
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim objDC As New DataClass
        Dim res As New HCL_VendorInvoiceVPRes()
        Dim errorMsg As New StringBuilder()
        Dim xmlNewDocRead As New XmlDocument()
        Dim Data As New StringBuilder()
        Try
            'Dim reader As New StreamReader(Data)

            Data.Append("<IPCOCR> <WORKITEMNUMBER>" & WORKITEMNUMBER & "</WORKITEMNUMBER><PROJECTNAME>" & PROJECTNAME & "</PROJECTNAME><VENDORNAME>" & VENDORNAME & "</VENDORNAME><UNIQUEREFERENCENUMBER>" & UNIQUEREFERENCENUMBER & "</UNIQUEREFERENCENUMBER><BARCODE>" & BARCODE & "</BARCODE><VENDORCODE>" & VENDORCODE & "</VENDORCODE><COMPANYCODE>" & COMPANYCODE & "</COMPANYCODE><CURRENCY>" & CURRENCY & "</CURRENCY><INVOICENUMBER>" & INVOICENUMBER & "</INVOICENUMBER><TOTALINVOICEAMOUNT>" & TOTALINVOICEAMOUNT & "</TOTALINVOICEAMOUNT></IPCOCR>")
            Dim strData As String = Data.ToString()

            errorMsg.Append("<RESULT>")
            'Getting Credential from header Tag
            Dim objDTNew As New DataTable()
            Dim DocumentType As String = ""
            Dim TargetDatahistoryFieldMapping As String = ""
            Dim DisputeInOCRFieldMapping As String = ""
            Dim Eid As Int32 = 0
            Dim UID As Int32 = 0
            Dim TargetFieldMapping As String = 0
            Dim SourceScanFiePath As String = ""
            Dim Action As String = "CREATE"
            Dim TargetDocID As Int32 = 0
            Dim IsAllowSkip As Boolean = False
            objDTNew = objDC.ExecuteQryDT("select * from mmm_mst_ocrlog with(nolock) where FTPFileName='" & FILENAME & "' and isnull(ocrUploadStatus,0)<>1")
            If FILENAME.ToString().Contains("FLD5_") Then
                IsAllowSkip = True
            End If
            If objDTNew.Rows.Count = 1 Then
                DocumentType = objDTNew.Rows(0)("targetdocumenttype")
                TargetDatahistoryFieldMapping = objDTNew.Rows(0)("TargetDatahistoryFieldMapping")
                DisputeInOCRFieldMapping = objDTNew.Rows(0)("DisputeInOCRFieldMapping")
                Eid = objDTNew.Rows(0)("eid")
                UID = objDTNew.Rows(0)("uid")
                TargetFieldMapping = objDTNew.Rows(0)("TargetFieldMapping")
                SourceScanFiePath = objDTNew.Rows(0)("oldFileName")
                DocumentType = objDTNew.Rows(0)("targetdocumenttype")
                Dim objDTDocId As New DataTable
                If Eid <> 131 Then
                    Dim customQry As String = "select  tid from mmm_mst_doc_draft where " & TargetFieldMapping & " = '" & SourceScanFiePath & "' and documenttype='" & DocumentType & "' and eid=" & Eid
                    objDTDocId = objDC.ExecuteQryDT(customQry)
                    If (objDTDocId.Rows.Count > 0) Then
                        TargetDocID = objDTDocId.Rows(0)(0)
                        Action = "UPDATE"
                    End If
                ElseIf Eid = 131 Then
                    TargetDocID = objDTNew.Rows(0)("docid")
                    Action = "UPDATE"
                End If
            Else
                objDC.ExecuteQryDT(" insert into OCRHistory(Qry,documenttype,EID,step,ErrorMsg) values ('" & strData & "','" & DocumentType & "'," & Eid & ",1,'Please enter valid file FTP Name')")
                errorMsg.Append("<MESSAGE>Please enter valid file FTP Name</MESSAGE>")
                res.resCode = "Transaction failed."
                errorMsg.Append("</RESULT>")
                xmlNewDocRead.LoadXml(errorMsg.ToString())
                Return xmlNewDocRead
            End If
            'Hard coded UserName and password. As suggested by sunil Sir

            'Getting field definition form data base for creating request string

            objDC.ExecuteQryDT(" insert into OCRHistory (Qry,documenttype,EID,step,ErrorMsg) values ('" & strData & "','" & DocumentType & "'," & Eid & ",2,'No Error in Validation')")
            Dim objDT As New DataTable()
            objDT = objDC.ExecuteQryDT("    set nocount on;Select DisplayName,M1Inward,fieldmapping from MMM_MST_Fields with(nolock) where EID=" & Eid & " and DocumentType='" & DocumentType & "'")
            Dim xmlDocRead As New XmlDocument()
            xmlDocRead.LoadXml(strData)
            Dim strUpdateQuery As New ArrayList()
            If xmlDocRead.ChildNodes.Count >= 1 Then
                Dim sb As New StringBuilder()
                Dim SelNodesTxt As String = xmlDocRead.DocumentElement.Name
                Dim nodes As XmlNodeList = xmlDocRead.SelectNodes(SelNodesTxt)

                For Each node As XmlNode In nodes
                    For c As Integer = 0 To node.ChildNodes.Count - 1
                        Dim dvRow As DataView = objDT.DefaultView
                        dvRow.RowFilter = "M1Inward='" & Convert.ToString(node.ChildNodes.Item(c).Name) & "'"
                        Dim tbl As DataTable = dvRow.ToTable
                        If (tbl.Rows.Count > 0) Then
                            sb.Append("|").Append(tbl.Rows(0).Item("DisplayName")).Append("::").Append(Convert.ToString(node.ChildNodes.Item(c).InnerText))
                            strUpdateQuery.Add(tbl.Rows(0).Item("fieldmapping") & " = '" & Convert.ToString(node.ChildNodes.Item(c).InnerText) & "'")
                        End If
                    Next
                Next
                strData = sb.ToString()
            End If
            objDC.ExecuteQryDT(" insert into OCRHistory (Qry,documenttype,EID,step,ErrorMsg) values ('" & strData & "','" & DocumentType & "'," & Eid & ",3,'Supply string to get docid')")
            Dim Result = ""
            If Action = "CREATE" And Eid <> 131 Then
                'Result = CommanUtil.ValidateParameterByDocumentType(Eid, DocumentType, UID, strData)
                Result = CommanUtil.SaveDraft(EID:=Eid, DocType:=DocumentType, UID:=UID, Data:=strData, DOCID:=0)
            Else
                If Eid = 131 Then
                    'Dim update As New UpdateData
                    'Result = update.UpdateData(Eid, DocumentType, UID, strData, "EnableEdit", TargetDocID)
                    If Not IsAllowSkip Then
                        If strUpdateQuery.Count > 0 Then
                            Dim curStatus As String = objDC.ExecuteQryDT("select curstatus from mmm_mst_doc where tid=" & TargetDocID).Rows(0)(0)
                            If curStatus.ToUpper = "HEADER CREATION" Then
                                objDC.ExecuteQryDT("Update mmm_mst_doc set " & String.Join(" , ", strUpdateQuery.ToArray()) & " where tid=" & TargetDocID)
                                Result = "Your DocID is " & TargetDocID
                            End If

                        End If
                    End If
                Else
                    Result = CommanUtil.SaveDraft(EID:=Eid, DocType:=DocumentType, UID:=UID, Data:=strData, DOCID:=0)
                End If
                'Dim update As New UpdateData
                'Result = update.UpdateData(Eid, DocumentType, UID, strData, "EnableEdit", TargetDocID)

            End If
            'objDC.ExecuteQryDT(" Then insert into OCRHistory values ('" & Result & "','" & DocumentType & "',124,3)")
            If Result.Contains("Your DocID is") Then
                'Add condition for draft case 
                Result = Result.Replace("'", "")
                If Result.Contains("~") Then
                    Dim docIDPart As String() = Result.ToString().Split("~")
                    If docIDPart.Length > 1 Then
                        Result = docIDPart(0)
                    End If
                End If
                Result = Result.Replace("'", "")
                Dim docid As Int64 = Convert.ToInt64(Result.ToString.Replace("Your DocID is ", "").Trim())
                If docid = 0 Then
                    docid = TargetDocID
                End If
                If Eid <> 131 Then
                    objDC.ExecuteQryDT("Update mmm_mst_doc_draft set " & TargetFieldMapping & " ='" & SourceScanFiePath & "'," & TargetDatahistoryFieldMapping & " = '" & Data.ToString() & "'," & DisputeInOCRFieldMapping & "='NO' where tid =" & docid & " and eid=" & Eid)
                End If
                objDC.ExecuteQryDT(" insert into OCRHistory (Qry,documenttype,EID,step,ErrorMsg) values ('" & strData & "','" & DocumentType & "'," & Eid & ",4,'Transaction received successfully with docid " & docid & "')")
                objDC.ExecuteQryDT("Update mmm_mst_ocrlog set ocrUploadStatus=1, draftid=" & docid & " where FTPFileName='" & FILENAME & "' ")
                errorMsg.Append("<MESSAGE> Transaction received successfully with docid " & docid & " </MESSAGE>")
                res.resCode = "0"
                res.resStr = "Transaction received successfully"
            ElseIf Result.Contains("Record updated successfully.") Then
                objDC.ExecuteQryDT(" insert into OCRHistory (Qry,documenttype,EID,step,ErrorMsg) values ('" & strData & "','" & DocumentType & "'," & Eid & ",4,'Transaction received successfully Updated! ')")
                errorMsg.Append("<MESSAGE> Transaction received successfully Updated </MESSAGE>")
                res.resCode = "0"
                res.resStr = "Transaction received successfully Updated"
            Else
                Result = Result.Replace("'", "")
                objDC.ExecuteQryDT(" insert into OCRHistory (Qry,documenttype,EID,step,ErrorMsg) values ('" & strData & "','" & DocumentType & "'," & Eid & ",4,'Transaction failed " & Result.Replace("Error(s) In document " & DocumentType & ".", String.Empty) & "')")
                errorMsg.Append("<MESSAGE> " & Result.Replace("Error(s) In document " & DocumentType & ".", String.Empty) & "</MESSAGE>")
                res.resCode = "Transaction failed."
                Result = Result.Replace("Error(s) In document " & DocumentType & ".", String.Empty)
                res.resStr = Result
            End If
            errorMsg.Append("</RESULT>")

            xmlNewDocRead.LoadXml(errorMsg.ToString())
            'HttpContext.Current.Server.HtmlDecode(errorMsg.ToString())
            Return xmlNewDocRead

        Catch ex As Exception
            objDC.ExecuteQryDT(" insert into OCRHistory (Qry,documenttype,step,ErrorMsg) values ('" & Data.ToString() & "','',5,'Transaction failed Please contact to admnin department')")
            errorMsg.Append("<MESSAGE> There were some Error please contact admin.</MESSAGE>")
            res.resCode = "Transaction failed."
            errorMsg.Append("</RESULT>")
            xmlNewDocRead.LoadXml(errorMsg.ToString())
            Return xmlNewDocRead
        End Try
    End Function

    Protected Class HCL_VendorInvoiceVPRes
        Public Property resStr As String
        Public Property resCode As String
    End Class

End Class