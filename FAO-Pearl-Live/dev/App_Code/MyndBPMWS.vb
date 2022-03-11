Imports System.Data
Imports System.Data.SqlClient
Imports System.Net
Imports System.IO
Imports System.Xml.Serialization
Imports System.Runtime.Serialization
Imports System.ServiceModel
Imports System.ServiceModel.Web
Imports System.Xml
Imports System.Web.Hosting


' NOTE: You can use the "Rename" command on the context menu to change the class name "ImportMaster" in code, svc and config file together.
Public Class MyndBPMWS
    Implements IMyndBPMWS

    Function SaveData(Data As Stream) As String Implements IMyndBPMWS.SaveData
        Dim Result = ""
        Dim Key As String = "", EID As String = "", DocType As String = "", UID As String = 0
        Try
            'Convert steam into string 
            Dim reader As New StreamReader(Data)
            Dim strData As String = reader.ReadToEnd()
            'Decode it to UTF-8
            strData = HttpUtility.UrlDecode(strData)
            Dim Data1 As New StringBuilder(strData)
            'Save string into database
            'SaveServicerequest

            'Dim arrData As String() = Split(strData, "~")
            Dim arrData As String() = strData.Split({"~"c}, 3)
            For i As Integer = 0 To arrData.Length - 1
                Dim ar = Split(arrData(i), "$$")
                If ar(0).ToUpper().Trim() = "KEY" Then
                    Key = ar(1)
                ElseIf ar(0).ToUpper().Trim() = "DOCTYPE" Then
                    DocType = ar(1)
                ElseIf ar(0).ToUpper().Trim() = "DATA" Then
                    strData = ar(1)
                End If
            Next
            Dim DS As New DataSet()
            DS = AuthenticateWSRequest(Key)
            If DS.Tables(0).Rows.Count > 0 Then
                EID = DS.Tables(0).Rows(0).Item("EID")
                UID = DS.Tables(1).Rows(0).Item("uid")
                Result = CommanUtil.ValidateParameterByDocumentType(EID, DocType, UID, strData)
            Else
                Result = "Sorry!!! Authentication failed."
            End If
            CommanUtil.SaveServicerequest(Data1, "MyndBPMWS", "SaveData", Result)
        Catch ex As Exception
            ErrorLog.sendMail("MyndBPMWS.SaveData", ex.Message)
            Return "RTO"
        End Try
        Return Result
    End Function



    Function UpdateData(Data As Stream) As String Implements IMyndBPMWS.UpdateData
        Dim Result = ""
        Dim Key As String = "", EID As String = "", DocType As String = "", UID As String = 0
        Dim DOCID As Integer = 0
        Try
            'Convert steam into string 
            Dim reader As New StreamReader(Data)
            Dim strData As String = reader.ReadToEnd()
            'Decode it to UTF-8
            strData = HttpUtility.UrlDecode(strData)
            Dim Data1 As New StringBuilder(strData)
            'Save string into database

            'Dim arrData As String() = Split(strData, "~")
            Dim arrData As String() = strData.Split({"~"c}, 3)
            For i As Integer = 0 To arrData.Length - 1
                Dim ar = Split(arrData(i), "$$")
                If ar(0).ToUpper().Trim() = "KEY" Then
                    Key = ar(1)
                ElseIf ar(0).ToUpper().Trim() = "DOCTYPE" Then
                    DocType = ar(1)
                ElseIf ar(0).ToUpper().Trim() = "DATA" Then
                    strData = ar(1)
                End If
            Next
            Dim DS As New DataSet()
            DS = AuthenticateWSRequest(Key)
            If DS.Tables(0).Rows.Count > 0 Then
                EID = DS.Tables(0).Rows(0).Item("EID")
                UID = DS.Tables(1).Rows(0).Item("uid")
                Dim objUp As New UpdateData()
                Result = objUp.UpdateData(EID, DocType, UID, strData, DOCID:=Key)
            Else
                Result = "Sorry!!! Authentication failed."
            End If
            CommanUtil.SaveServicerequest(Data1, "MyndBPMWS", "UpdateData", Result)
        Catch ex As Exception
            ErrorLog.sendMail("MyndBPMWS.UpdateData", ex.Message)
            Return "RTO"
        End Try
        Return Result
    End Function

    Function UpdateDataA(Data As Stream) As String Implements IMyndBPMWS.UpdateDataA
        Dim Result = ""
        Dim Key As String = "", EID As String = "", DocType As String = "", UID As String = 0
        Dim DOCID As Integer = 0
        Try
            'Convert steam into string 
            Dim reader As New StreamReader(Data)
            Dim strData As String = reader.ReadToEnd()
            'Decode it to UTF-8
            strData = HttpUtility.UrlDecode(strData)
            Dim Data1 As New StringBuilder(strData)
            'Save string into database

            'Dim arrData As String() = Split(strData, "~")
            Dim arrData As String() = strData.Split({"~"c}, 3)
            For i As Integer = 0 To arrData.Length - 1
                Dim ar = Split(arrData(i), "$$")
                If ar(0).ToUpper().Trim() = "KEY" Then
                    Key = ar(1)
                ElseIf ar(0).ToUpper().Trim() = "DOCTYPE" Then
                    DocType = ar(1)
                ElseIf ar(0).ToUpper().Trim() = "DATA" Then
                    strData = ar(1)
                End If
            Next
            Dim DS As New DataSet()
            DS = AuthenticateWSRequest(Key)
            If DS.Tables(0).Rows.Count > 0 Then
                EID = DS.Tables(0).Rows(0).Item("EID")
                UID = DS.Tables(1).Rows(0).Item("uid")
                Dim objUp As New UpdateData()
                Dim DocExist As Boolean = False
                Dim Keys As String = ""
                DocExist = objUp.GetDOCID(EID, Keys, DocType, strData)
                If DocExist = True Then
                    Dim dsDoc As New DataSet()
                    If DocType.Trim.ToUpper <> "USER" Then
                        dsDoc = objUp.GetDocDetails(DocType, EID, Keys)
                        If dsDoc.Tables(0).Rows.Count > 0 Then
                            For Each column As DataColumn In dsDoc.Tables(0).Columns
                                If Not IsFieldSupplyed(strData, column.ColumnName) Then
                                    strData = strData & "|" & column.ColumnName & "::" & dsDoc.Tables(0).Rows(0).Item(column.ColumnName)
                                End If
                            Next
                        End If
                    End If
                    Result = objUp.UpdateData(EID, DocType, UID, strData, DOCID:=Keys)
                Else
                    If (ChkEnableInsertOnEdit(EID, DocType)) Then
                        Result = CommanUtil.ValidateParameterByDocumentType(EID, DocType, UID, strData)
                    End If
                End If

            Else
                Result = "Sorry!!! Authentication failed."
            End If
            CommanUtil.SaveServicerequest(Data1, "MyndBPMWS", "UpdateData", Result)
        Catch ex As Exception
            ErrorLog.sendMail("MyndBPMWS.UpdateDataA", ex.Message)
            Return "RTO"
        End Try
        Return Result
    End Function

    Public Function ChkEnableInsertOnEdit(EID As Integer, DocType As String) As Boolean
        Dim ret As Boolean = False
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim ds As New DataSet()
        Try
            Dim Query = "SELECT FormName FROM MMM_MST_FORMS WHERE EID= " & EID & " AND FormName='" & DocType & "' AND EnableInsertOneditFail='1'"
            Using con = New SqlConnection(conStr)
                Using da = New SqlDataAdapter(Query, con)
                    con.Open()
                    da.Fill(ds)
                    If ds.Tables(0).Rows.Count > 0 Then
                        ret = True
                    End If
                End Using
            End Using
        Catch ex As Exception

        End Try
        Return ret
    End Function


    Public Function IsFieldSupplyed(Data As String, FieldName As String) As Boolean
        Dim ret As Boolean = False
        Try
            Dim arr = Data.Split("|")
            For i As Integer = 0 To arr.Length - 1
                Dim ar = arr(i).Split("::")
                If ar(0).Trim.ToUpper = FieldName.Trim.ToUpper Then
                    ret = True
                    Exit For
                End If
            Next
        Catch ex As Exception
            Throw
        End Try
        Return ret
    End Function
    Function DocumentApproval(Data As Stream) As String Implements IMyndBPMWS.DocumentApproval
        Dim Result = ""
        Dim Key As String = "", EID As String = "", DocType As String = "", UID As String = 0, CurStatus As String = ""
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim DOCID As Integer = 0
        Dim ActionType As String = ""
        Try
            'Convert steam into string 
            Dim reader As New StreamReader(Data)
            Dim strData As String = reader.ReadToEnd()
            'Decode it to UTF-8
            strData = HttpUtility.UrlDecode(strData)
            Dim Data1 As New StringBuilder(strData)
            'Dim arrData As String() = Split(strData, "~")
            Dim arrData As String() = strData.Split({"~"c}, 4)
            For i As Integer = 0 To arrData.Length - 1
                Dim ar = Split(arrData(i), "$$")
                If ar(0).ToUpper().Trim() = "KEY" Then
                    Key = ar(1)
                ElseIf ar(0).ToUpper().Trim() = "DOCTYPE" Then
                    DocType = ar(1).Replace("--", "").Replace("'", "")
                ElseIf ar(0).ToUpper().Trim() = "DATA" Then
                    strData = ar(1)
                ElseIf ar(0).ToUpper().Trim() = "ACTIONTYPE" Then
                    ActionType = ar(1)
                End If
            Next
            Dim arr = strData.Split("|")
            For j As Integer = 0 To arr.Length - 1
                If arr(j).Trim <> "" Then
                    Dim a = Split(arr(j), "::")
                    If a(0).Trim.ToUpper = "DOCID" Then
                        DOCID = a(1)
                    End If
                End If
            Next
            Dim DS As New DataSet()
            Dim DSDOC As New DataSet()
            'Authanticating request
            DS = AuthenticateWSRequest(Key)
            Dim DocumentType = ""
            If Convert.ToString(ActionType) = "APPROVAL" Or Convert.ToString(ActionType) = "RECONSIDER" Or Convert.ToString(ActionType) = "REJECT" Or Convert.ToString(ActionType) = "CRM" Then
                If DS.Tables(0).Rows.Count > 0 Then
                    EID = DS.Tables(0).Rows(0).Item("EID")
                    'finding main doctype
                    Dim dsD As New DataSet()
                    'Getting main documenttype
                    Dim Query = "SELECT Eventname FROM MMM_MST_FORMS WHERE EID=" & EID & " AND FormName='" & DocType & "'"
                    Using con = New SqlConnection(conStr)
                        Using da = New SqlDataAdapter(Query, con)
                            da.Fill(dsD)
                        End Using
                    End Using
                    'Finding docid 
                    If dsD.Tables(0).Rows.Count > 0 Then
                        DocumentType = dsD.Tables(0).Rows(0).Item("EventName").ToString.Trim
                        If DOCID = 0 Then
                            DOCID = GetDOCID(EID, DocumentType, strData, DocType)
                        End If
                        If DOCID > 0 Then
                            'Now Finding CurStatus And current userID OF Document
                            Using con = New SqlConnection(conStr)
                                Using da = New SqlDataAdapter("GETCURRENTUSER", con)
                                    da.SelectCommand.CommandType = CommandType.StoredProcedure
                                    da.SelectCommand.Parameters.AddWithValue("@DOCID", DOCID)
                                    da.SelectCommand.Parameters.AddWithValue("@EID", EID)
                                    da.Fill(DSDOC)
                                End Using
                            End Using
                            If DSDOC.Tables(0).Rows.Count > 0 Then
                                UID = DSDOC.Tables(0).Rows(0).Item("userid")
                                CurStatus = DSDOC.Tables(0).Rows(0).Item("curstatus")
                                'Now Checking CurrStatus
                                Dim BaseDocType = DSDOC.Tables(0).Rows(0).Item("documenttype")

                                Dim objUp As New DocumentApproval()
                                Dim dscurr As New DataSet()
                                DS = objUp.ValidateDocStatus(EID, DOCID, DocType, CurStatus)
                                If DS.Tables(0).Rows.Count > 0 Then
                                    Result = objUp.ApproveDocument(EID, DocType, UID, strData, DOCID, CurStatus, BaseDocType, ActionType)
                                Else
                                    Result = "Invalid Document"
                                End If
                            Else
                                Result = "Invalid document."
                            End If
                        Else
                            Result = "No matching document found based on the supplyed keys"
                        End If
                    Else
                        Result = "Invalid document."
                    End If

                Else
                    Result = "Sorry!!! Authentication failed."
                End If
            Else
                Result = "Action Type is Invalid or Not supplied"
            End If
            CommanUtil.SaveServicerequest(Data1, "MyndBPMWS", "DocumentApproval", Result)
        Catch ex As Exception
            ErrorLog.sendMail("MyndBPMWS.DocumentApproval", ex.Message)
            Return "RTO"
        End Try
        Return Result
    End Function


    Public Function GetDOCID(EID As Integer, BaseDocumentType As String, data As String, Optional ActionFormName As String = "") As Integer
        Dim DOCID As Integer = 0
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim DocumentType = ""
        If ActionFormName.Trim = "" Then
            DocumentType = BaseDocumentType
        Else
            DocumentType = ActionFormName.Trim
        End If
        Dim ds As New DataSet()
        Try
            Dim objU As New DocumentApproval()
            Dim keys As String = ""
            Dim ret As Boolean = False
            ret = objU.ValidateKeys(EID, DocumentType, data, keys)
            If ret = True Then
                Dim Query As String = "SELECT tid FROM MMM_MST_DOC WHERE EID=" & EID & " AND DocumentType='" & BaseDocumentType & "'" & keys
                Using con = New SqlConnection(conStr)
                    Using da = New SqlDataAdapter(Query, con)
                        da.Fill(ds)
                    End Using
                End Using
                If ds.Tables(0).Rows.Count > 0 Then
                    DOCID = ds.Tables(0).Rows(0).Item("tid")
                End If
            End If
        Catch ex As Exception
            Return 0
        End Try
        Return DOCID
    End Function

    Function DeleteDOCList(Data As Stream) As String Implements IMyndBPMWS.DeleteDOCList
        Dim Result = ""
        Dim Key As String = "", EID As String = "", DocType As String = "", UID As String = 0
        Try
            'Convert steam into string 
            Dim reader As New StreamReader(Data)
            Dim strData As String = reader.ReadToEnd()
            'Decode it to UTF-8
            strData = HttpUtility.UrlDecode(strData)
            Dim Data1 As New StringBuilder(strData)
            'Save string into database
            'SaveServicerequest

            Dim arrData As String() = Split(strData, "~")
            For i As Integer = 0 To arrData.Length - 1
                Dim ar = Split(arrData(i), "$$")
                If ar(0).ToUpper().Trim() = "KEY" Then
                    Key = ar(1)
                ElseIf ar(0).ToUpper().Trim() = "DOCTYPE" Then
                    Dim arr2 = Split(ar(1).Trim, "::")
                    DocType = arr2(0)
                    strData = arr2(1)
                ElseIf ar(0).ToUpper().Trim() = "DATA" Then
                    'strData = ar(1)
                End If
            Next
            Dim DS As New DataSet()
            DS = AuthenticateWSRequest(Key)
            If DS.Tables(0).Rows.Count > 0 Then
                EID = DS.Tables(0).Rows(0).Item("EID")
                UID = DS.Tables(1).Rows(0).Item("uid")
                Dim obj As New DeleteDoc()
                Result = obj.DeleteDocument(EID, DocType, UID, strData)
            Else
                Result = "Sorry!!! Authentication failed."
            End If
            CommanUtil.SaveServicerequest(Data1, "MyndBPMWS", "DeleteDOCList", Result)
        Catch ex As Exception
            ErrorLog.sendMail("DeleteDOCList.SaveData", ex.Message)
            Return "RTO"
        End Try
        Return Result
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

    Function M1_Disocunt_Detail_Update(data As M1Discounting) As Grofers_Res Implements IMyndBPMWS.M1_Disocunt_Detail_Update
        Dim res As New Grofers_Res()
        Dim ResultMsg As New ArrayList()
        Dim ResultSrc As New ArrayList()
        Dim objDC As New DataClass()
        Dim objDT As New DataTable()
        Try
            Dim tid = objDC.ExecuteQryScaller("insert into M1UpdateDiscounting_ServiceLog(docid,M1_Factoring_Unit_No,Payable_Date,Payable_Amount,CreateOn,ServiceRequest) values(0,'" & data.M1_FACTORING_UNIT_NO & "','" & data.PAYABLE_DATE & "','" & data.PAYABLE_AMOUNT & "',getdate(),'{""M1_FACTORING_UNIT_NO"":""" & data.M1_FACTORING_UNIT_NO & """,""PAYABLE_DATE"":""" & data.PAYABLE_DATE & """,""PAYABLE_AMOUNT"":""" & data.PAYABLE_AMOUNT & """}') SELECT SCOPE_IDENTITY()")
            If data.M1_FACTORING_UNIT_NO = "" Then
                ResultMsg.Add("M1 Factoring Unit Number Not Found")
                ResultSrc.Add("204")
            End If
            If data.PAYABLE_DATE = "" Then
                ResultMsg.Add("Payable Date Not Found")
                ResultSrc.Add("204")
            End If
            If data.PAYABLE_AMOUNT = "" Then
                ResultMsg.Add("Payable Amount Not Found")
                ResultSrc.Add("204")
            End If
            If ResultMsg.Count > 0 Then
                res.resStr = String.Join(",", ResultMsg.ToArray())
                res.resCode = String.Join(",", ResultSrc.ToArray())
                objDC.ExecuteQryScaller("Update M1UpdateDiscounting_ServiceLog set ServiceResponse='{""resStr"": """ & res.resStr.ToString() & """,""resCode"": """ & res.resCode.ToString() & """}' where tid= " & tid)
                Return res
            End If
            objDT = objDC.ExecuteQryDT("select docid from M1Discoyunting_ServiceLog where serviceresoponse like '%""" & data.M1_FACTORING_UNIT_NO & """%'")
            If objDT.Rows.Count = 0 Then
                res.resStr = "M1 Factoring Unit Number does not exist in our database"
                res.resCode = "204"
                objDC.ExecuteQryScaller("Update M1UpdateDiscounting_ServiceLog set ServiceResponse='{""resStr"": """ & res.resStr.ToString() & """,""resCode"": """ & res.resCode.ToString() & """}' where tid= " & tid)
                Return res
            Else
                Dim docid As Int32 = objDT.Rows(0)(0)
                objDC.ExecuteNonQuery("insert into M1UpdateDiscounting_ServiceLog(docid,M1_Factoring_Unit_No,Payable_Date,Payable_Amount,CreateOn) values(" & docid & ",'" & data.M1_FACTORING_UNIT_NO & "','" & data.PAYABLE_DATE & "','" & data.PAYABLE_AMOUNT & "',getdate())")
                Dim objDTMapping As New DataTable()
                objDTMapping = objDC.ExecuteQryDT("declare @doctype nvarchar(max),@eid int select @doctype= documenttype,@eid = eid from mmm_mst_doc where tid=" & docid & " select M1DiscountUpdate,FieldMapping,displayname,eid  from mmm_mst_fields where documenttype=@doctype and eid=@eid and M1DiscountUpdate is not null")
                Dim str As New StringBuilder()
                str.Append(" Update mmm_mst_doc set ")
                Dim ar As New ArrayList()
                For Each dr As DataRow In objDTMapping.Rows
                    Select Case dr("M1DiscountUpdate")
                        Case "PAYABLE_AMOUNT"
                            ar.Add(dr("FieldMapping") & "='" & data.PAYABLE_AMOUNT & "'")
                        Case "PAYABLE_DATE"
                            ar.Add(dr("FieldMapping") & "='" & data.PAYABLE_DATE & "'")
                    End Select
                Next
                If ar.Count > 0 Then
                    str.Append(String.Join(",", ar.ToArray()))
                    str.Append("  where tid = " & docid)
                    objDC.ExecuteNonQuery(str.ToString())
                End If
                'Call approval web service   Key$$ISEOJOHHJBA000391OOS ~DOCTYPE$$SAP ID Updated~ACTIONTYPE$$APPROVAL~Data$$Company Code::1000|Parking SAP Doc ID::1000007539|Parking Fiscal Year::2015|BPM ID::AP77136 working code
                'Dim arrApprovalStr As New StringBuilder()
                'arrApprovalStr.Append("key$$")
                'If objDTMapping.Rows.Count > 0 Then
                '    arrApprovalStr.Append(objDC.ExecuteQryScaller("select apikey from mmm_mst_entity where eid=" & objDTMapping.Rows(0)("eid") & ""))
                '    arrApprovalStr.Append(" ~DOCTYPE$$Initiate_M1_Dis")
                '    arrApprovalStr.Append("~ACTIONTYPE$$APPROVAL~Data$$")
                '    For Each dr As DataRow In objDTMapping.Rows
                '        Select Case dr("M1DiscountUpdate").ToString().ToUpper
                '            Case "PAYABLE_AMOUNT"
                '                arrApprovalStr.Append(dr("displayname") & "::" & data.PAYABLE_AMOUNT & "|")
                '            Case "PAYABLE_DATE"
                '                arrApprovalStr.Append(dr("displayname") & "::" & data.PAYABLE_DATE)
                '            Case "BPMID"
                '                arrApprovalStr.Append(dr("displayname") & "::" & objDC.ExecuteQryScaller(" select " & dr("fieldMapping") & " from mmm_mst_doc where tid=" & docid) & "|")
                '        End Select
                '    Next


                '    Dim sURL As String = "http://www.myndsaas.com/MyndBPMWS.svc/DocumentApproval"
                '    '' temp remove after testing
                '    Dim request As HttpWebRequest = HttpWebRequest.Create(sURL)
                '    Dim encoding As New ASCIIEncoding()
                '    Dim strResult As String = ""

                '    Dim data1 As Byte() = encoding.GetBytes(arrApprovalStr.ToString())
                '    Dim webrequest__1 As HttpWebRequest = DirectCast(WebRequest.Create(Trim(sURL)), HttpWebRequest)

                '    webrequest__1.Method = "POST"
                '    ' set content type
                '    webrequest__1.ContentType = "application/x-www-form-urlencoded"
                '    ' set content length
                '    webrequest__1.ContentLength = data1.Length
                '    ' get stream data out of webrequest object
                '    Dim newStream As Stream = webrequest__1.GetRequestStream()
                '    newStream.Write(data1, 0, data1.Length)
                '    newStream.Close()
                '    ' declare & read response from service
                '    Dim webresponse As HttpWebResponse = DirectCast(webrequest__1.GetResponse(), HttpWebResponse)
                '    Dim enc As Encoding = System.Text.Encoding.GetEncoding("utf-8")
                '    ' read response stream from response object
                '    Dim loResponseStream As New StreamReader(webresponse.GetResponseStream(), enc)

                '    strResult = loResponseStream.ReadToEnd()

                '    loResponseStream.Close()
                '    ' close the response object
                '    webresponse.Close()

                '    Dim regex As New Regex("\<[^\>]*\>")
                '    strResult = regex.Replace(strResult, [String].Empty)
                '    strResult = strResult.Replace("' ", " ")
                '    strResult = strResult.Replace(" '", " ")

                '    If (strResult.Contains("Your DocID is") Or strResult.ToUpper().Contains("SUCCESSFULLY")) Then
                '        res.resStr = "M1 discounitng information successfully updated into our database"
                '        res.resCode = "200"
                '    ElseIf (strResult.Contains("RTO")) Then
                '        res.resStr = "RTO"
                '        res.resCode = "204"
                '    End If
                'End If
                res.resStr = "M1 discounting information successfully updated!"
                res.resCode = "200"
                objDC.ExecuteQryScaller("Update M1UpdateDiscounting_ServiceLog set ServiceResponse='{""resStr"": """ & res.resStr.ToString() & """,""resCode"": """ & res.resCode.ToString() & """}', docid=" & docid & " where tid= " & tid)
                Return res
            End If
        Catch ex As Exception

        End Try
        Return res
    End Function
    Function RegisterationWithM1(data As M1Registration) As Grofers_Res Implements IMyndBPMWS.RegisterationWithM1
        Dim objDC As New DataClass()
        Dim res As New Grofers_Res()
        Dim ResultMsg As New ArrayList()
        Dim ResultSrc As New ArrayList()
        Try
            If data.BUYER_PANNUMBER = "" Then
                ResultMsg.Add("BUYER PAN Number Not Found")
                ResultSrc.Add("BUYER_PANNUMBER")
            End If
            If data.SUPPLIER_PANNUMBER = "" Then
                ResultMsg.Add("SUPPLIER PAN Number Not Found")
                ResultSrc.Add("SUPPLIER_PANNUMBER")
            End If
            If ResultMsg.Count > 0 Then
                res.resStr = String.Join(", ", ResultMsg.ToArray())
                res.resCode = String.Join(", ", ResultSrc.ToArray())
                Return res
            End If
            Dim objDT As New DataTable()
            objDT = objDC.ExecuteQryDT("select * from mmm_mst_entity where pan='" & data.BUYER_PANNUMBER & "'")
            If objDT.Rows.Count > 0 Then
                If Convert.ToString(objDT.Rows(0)("supplier_formname")) = "" Then
                    res.resStr = "Suppler Form Name does not exist in our database please contact admin"
                    res.resCode = "supplier_formname does not exists"
                    Return res
                Else
                    data.DOCUMENTTYPE = Convert.ToString(objDT.Rows(0)("supplier_formname"))
                    data.EID = objDT.Rows(0)("EID")
                    objDC.ExecuteQryScaller("Update mmm_mst_entity set M1RegStatus=1 where pan='" & data.BUYER_PANNUMBER & "' and eid=" & data.EID)
                End If
                Dim fldMapping As String = objDC.ExecuteQryScaller("select fieldmapping from mmm_mst_fields where documenttype='" & data.DOCUMENTTYPE & "' and eid=" & data.EID & " and ispan=1")
                If String.IsNullOrEmpty(fldMapping) Then
                    res.resStr = "Document ISPAN Property not found please contact to admin"
                    res.resCode = "SUPPLIER_PANNUMBER ISPAN property not activated"
                    Return res
                End If
                If (objDC.ExecuteQryScaller("if exists(select * from mmm_mst_master where documenttype='" & data.DOCUMENTTYPE & "' and eid=" & data.EID & " and " & fldMapping & " ='" & data.SUPPLIER_PANNUMBER & "') select 'SUCCESS' else Select 'FAIL'") = "SUCCESS") Then
                    objDC.ExecuteQryDT("Update mmm_mst_master set registerwithM1=1 where " & fldMapping & "='" & data.SUPPLIER_PANNUMBER & "' and documenttype='" & data.DOCUMENTTYPE & "' and eid=" & data.EID & " ")
                Else
                    res.resStr = "PAN Number does not exist in BPM"
                    res.resCode = "PAN_NUMBER"
                    Return res
                End If
                res.resStr = "M1 Vendor Registration with BPM successfully updated"
                res.resCode = "200"
                Return res

            Else
                res.resStr = "BUYER PAN Number does not exists in our database !"
                res.resCode = "BUYER_NUMBER does not exists"
                Return res
            End If
        Catch ex As Exception


        End Try
    End Function
    Function SetTransaction(Data As Stream) As Grofers_Res Implements IMyndBPMWS.SetTransaction
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim res As New Grofers_Res()
        Try
            Dim reader As New StreamReader(Data)
            Dim strData As String = reader.ReadToEnd()
            Dim coll As NameValueCollection = HttpUtility.ParseQueryString(strData)
            'Getting Credential from header Tag
            Dim UserName = WebOperationContext.Current.IncomingRequest.Headers("authKey")
            Dim Password = WebOperationContext.Current.IncomingRequest.Headers("authToken")
            'Hard coded UserName and password. As suggested by sunil Sir
            Dim DocumentType = "Card Cash Withdrawal"
            'Getting field definition form data base for creating request string
            Dim TypeOfTransaction As String = Convert.ToString(coll("typeOfTransaction"))
            Dim sb As New StringBuilder()
            If (UserName.Trim = "Grofers_HappayCard" And Password = "Gr!(*$13hPc") Then
                'We will insert only DB transaction 
                If (Not (TypeOfTransaction Is Nothing)) Then
                    If (TypeOfTransaction.Trim.ToUpper = "DB") Then
                        Dim StrQry = "set nocount on;Select DisplayName, InWardXMLTagName from MMM_MST_Fields With(nolock) where EID= 83 And documenttype ='" & DocumentType & "'"
                        Dim dsFields As New DataSet()
                        Using con As New SqlConnection(conStr)
                            Using da As New SqlDataAdapter(StrQry, con)
                                da.Fill(dsFields)
                            End Using
                        End Using
                        For Each Key In coll.Keys
                            'Get it's equivalent BPM key field from database
                            Dim dvRow As DataView = dsFields.Tables(0).DefaultView
                            dvRow.RowFilter = "InWardXMLTagName='" & Key & "'"
                            Dim tbl As DataTable = dvRow.ToTable
                            If (tbl.Rows.Count > 0) Then
                                sb.Append("|").Append(tbl.Rows(0).Item("DisplayName")).Append("::").Append(coll(Key))
                            End If
                        Next
                        strData = sb.ToString()
                        Dim Result = CommanUtil.ValidateParameterByDocumentType(83, DocumentType, 7917, strData)
                        If Result.Contains("Your DocID is") Then
                            res.resCode = "0"
                            res.resStr = "Transaction received successfully"
                        Else
                            res.resCode = "Transaction failed."
                            Result = Result.Replace("Error(s) in document " & DocumentType & ".", String.Empty)
                            res.resStr = Result
                        End If
                    Else
                        res.resCode = "Invalid Transaction"
                        res.resStr = "Only DB transactions allowed"
                    End If
                Else
                    res.resCode = "typeOfTransaction required."
                    res.resStr = "typeOfTransaction required."
                End If
            Else
                res.resCode = "Invalid Credential."
                res.resStr = "Authentication failed.Invalid UeserName or Password supplyed."
            End If
            Return res
        Catch ex As Exception
            res.resCode = "fail"
            res.resStr = "RTO"
            Return res
        End Try
    End Function

    Public Function GenerateIdentifier(length As Integer) As String
        Dim AvailableCharacters As Char() = {"A"c, "B"c, "C"c, "D"c, "E"c, "F"c,
                                            "G"c, "H"c, "I"c, "J"c, "K"c, "L"c,
                                            "M"c, "N"c, "O"c, "P"c, "Q"c, "R"c,
                                            "S"c, "T"c, "U"c, "V"c, "W"c, "X"c,
                                            "Y"c, "Z"c, "a"c, "b"c, "c"c, "d"c,
                                            "e"c, "f"c, "g"c, "h"c, "i"c, "j"c,
                                            "k"c, "l"c, "m"c, "n"c, "o"c, "p"c,
                                            "q"c, "r"c, "s"c, "t"c, "u"c, "v"c,
                                            "w"c, "x"c, "y"c, "z"c, "0"c, "1"c,
                                            "2"c, "3"c, "4"c, "5"c, "6"c, "7"c,
                                            "8"c, "9"c, "-"c, "_"c}
        Dim identifier As Char() = New Char(length - 1) {}
        Dim randomData As Byte() = New Byte(length - 1) {}

        Using rng As New System.Security.Cryptography.RNGCryptoServiceProvider()
            rng.GetBytes(randomData)
        End Using
        For idx As Integer = 0 To identifier.Length - 1
            Dim pos As Integer = randomData(idx) Mod AvailableCharacters.Length
            identifier(idx) = AvailableCharacters(pos)
        Next
        Return New String(identifier)
    End Function

    Public Function DecryptTripleDES(ByVal sOut As String, ByVal sKey As String) As String
        Try
            Dim DES As New System.Security.Cryptography.TripleDESCryptoServiceProvider
            DES.Key = UTF8Encoding.UTF8.GetBytes(sKey)
            ' Set the cipher mode.
            DES.Mode = System.Security.Cryptography.CipherMode.ECB
            ' Create the decryptor.
            Dim DESDecrypt As System.Security.Cryptography.ICryptoTransform = DES.CreateDecryptor()
            Dim Buffer As Byte() = Convert.FromBase64String(sOut)
            ' Transform and return the string.
            Return System.Text.UTF8Encoding.UTF8.GetString(DESDecrypt.TransformFinalBlock(Buffer, 0, Buffer.Length))
        Catch ex As Exception
            Throw New Exception()
        End Try
    End Function

    '' bakcup of ssoauth on 17-Oct-18 before imp. for taj - removed hard coding of hcl - 46 
    'Function SsoAuth(Data As Stream) As Hcl_Res Implements IMyndBPMWS.SsoAuth
    '    Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
    '    Dim res As New Hcl_Res()
    '    res.resCode = "0"
    '    res.resUrl = ""
    '    Dim Data1 As New StringBuilder()
    '    Dim ApiKey = ""
    '    Dim EmopoyeeCode As String = ""
    '    Dim RetUrl As String = ""
    '    Try
    '        Dim reader As New StreamReader(Data)
    '        Dim strData As String = reader.ReadToEnd()
    '        Dim coll As NameValueCollection = HttpUtility.ParseQueryString(strData)
    '        'Getting field definition form data base for creating request string

    '        ApiKey = Convert.ToString(coll("ApiKey")).Trim()
    '        EmopoyeeCode = Convert.ToString(coll("EmopoyeeCode")).Trim()
    '        RetUrl = Convert.ToString(coll("RetUrl"))
    '        If (ApiKey = "") Then
    '            res.resMsg = "ApiKey key required."
    '            Return res
    '        End If
    '        If (EmopoyeeCode = "") Then
    '            res.resMsg = "EmopoyeeCode key required."
    '            Return res
    '        End If
    '        'AuthSso
    '        Dim Key = "234342343423434234342343"
    '        Data1.Append("ApiKey:" & ApiKey & "|EmopoyeeCode:" & EmopoyeeCode & "|RetUrl:" & RetUrl)
    '        CommanUtil.SaveServicerequest(Data1, "MyndBPMWS", "SsoAuth", "")
    '        'sendMail("myndbpmws.SSOauth", "IN   -  ApiKey:" & ApiKey & "|EmopoyeeCode:" & EmopoyeeCode & "|RetUrl:" & RetUrl)
    '        ApiKey = DecryptTripleDES(ApiKey, Key)
    '        EmopoyeeCode = DecryptTripleDES(EmopoyeeCode, Key)
    '        Dim ds As New DataSet()
    '        Using con As New SqlConnection(conStr)
    '            Using da As New SqlDataAdapter("AuthSso", con)
    '                da.SelectCommand.CommandType = CommandType.StoredProcedure
    '                da.SelectCommand.Parameters.AddWithValue("@EmpCode", EmopoyeeCode)
    '                da.SelectCommand.Parameters.AddWithValue("@ApiKey", ApiKey)
    '                da.Fill(ds)
    '            End Using
    '        End Using
    '        If (ds.Tables(0).Rows.Count > 0) Then
    '            If (ds.Tables(1).Rows.Count > 0) Then
    '                Dim IsAuth = "0"
    '                IsAuth = Convert.ToString(ds.Tables(1).Rows(0).Item("IsAuth"))
    '                Dim UID = Convert.ToString(ds.Tables(1).Rows(0).Item("UID"))
    '                If (IsAuth = "1") Then
    '                    Dim token = GenerateIdentifier(100)
    '                    Using con As New SqlConnection(conStr)
    '                        Using da As New SqlDataAdapter("InserSsoToken", con)
    '                            con.Open()
    '                            da.SelectCommand.CommandType = CommandType.StoredProcedure
    '                            da.SelectCommand.Parameters.AddWithValue("@EID", EmopoyeeCode)
    '                            da.SelectCommand.Parameters.AddWithValue("@Key", token)
    '                            da.SelectCommand.Parameters.AddWithValue("@UID", UID)
    '                            Dim Succ = Convert.ToInt32(da.SelectCommand.ExecuteScalar())
    '                            If (Succ > 0) Then
    '                                res.resCode = "1"
    '                                res.resUrl = "https://hcl.myndsaas.com/ssologin.aspx?token=" & token
    '                                res.resMsg = "Success"
    '                                Return res
    '                            Else
    '                                Throw New Exception()
    '                            End If
    '                        End Using
    '                    End Using
    '                ElseIf (IsAuth = "0") Then
    '                    res.resCode = "100"
    '                    res.resMsg = "User Created in BPM but account not activated."
    '                    Return res
    '                Else
    '                    res.resCode = "3"
    '                    res.resMsg = "Login Issue in BPM."
    '                    Return res
    '                End If
    '            Else
    '                res.resMsg = "Invalid EmopoyeeCode."
    '                Return res
    '            End If
    '        Else
    '            res.resMsg = "Invalid ApiKey."
    '            Return res
    '        End If

    '        Return res
    '    Catch ex As Exception
    '        res.resCode = "0"
    '        res.resMsg = "Login Issue in BPM."
    '        Data1.Append("ApiKey:" & ApiKey & "|EmopoyeeCode:" & EmopoyeeCode & "|RetUrl:" & RetUrl)
    '        CommanUtil.SaveServicerequest(Data1, "MyndBPMWS", "SsoAuth", "SSOauthfail_TCatch")
    '        '  sendMail("myndbpmws.SSOauth", ex.Message)
    '        Return res
    '    End Try
    'End Function

    Function SsoAuth(Data As Stream) As Hcl_Res Implements IMyndBPMWS.SsoAuth
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim res As New Hcl_Res()
        res.resCode = "0"
        res.resUrl = ""
        Dim Data1 As New StringBuilder()
        Dim ApiKey = ""
        Dim EmopoyeeCode As String = ""
        Dim RetUrl As String = ""
        Try
            Dim reader As New StreamReader(Data)
            Dim strData As String = reader.ReadToEnd()
            Dim coll As NameValueCollection = HttpUtility.ParseQueryString(strData)
            'Getting field definition form data base for creating request string

            ApiKey = Convert.ToString(coll("ApiKey"))
            ApiKey = ApiKey.Replace(" ", "+")
            EmopoyeeCode = Convert.ToString(coll("EmopoyeeCode"))
            EmopoyeeCode = EmopoyeeCode.Replace(" ", "+")
            RetUrl = Convert.ToString(coll("RetUrl"))
            If (ApiKey = "") Then
                res.resMsg = "ApiKey key required."
                Return res
            End If
            If (EmopoyeeCode = "") Then
                res.resMsg = "EmopoyeeCode key required."
                Return res
            End If
            'AuthSso
            Dim Key = "234342343423434234342343"
            Data1.Append("ApiKey:" & ApiKey & "|EmopoyeeCode:" & EmopoyeeCode & "|RetUrl:" & RetUrl)
            CommanUtil.SaveServicerequest(Data1, "MyndBPMWS", "SsoAuth", "")
            'sendMail("myndbpmws.SSOauth", "IN   -  ApiKey:" & ApiKey & "|EmopoyeeCode:" & EmopoyeeCode & "|RetUrl:" & RetUrl)
            ApiKey = DecryptTripleDES(ApiKey, Key)
            EmopoyeeCode = DecryptTripleDES(EmopoyeeCode, Key)
            Dim ds As New DataSet()
            Using con As New SqlConnection(conStr)
                Using da As New SqlDataAdapter("AuthSso", con)
                    da.SelectCommand.CommandType = CommandType.StoredProcedure
                    da.SelectCommand.Parameters.AddWithValue("@EmpCode", EmopoyeeCode)
                    da.SelectCommand.Parameters.AddWithValue("@ApiKey", ApiKey)
                    da.Fill(ds)
                End Using
            End Using
            If (ds.Tables(0).Rows.Count > 0) Then
                If (ds.Tables(1).Rows.Count > 0) Then
                    Dim IsAuth = "0"
                    IsAuth = Convert.ToString(ds.Tables(1).Rows(0).Item("IsAuth"))
                    Dim UID = Convert.ToString(ds.Tables(1).Rows(0).Item("UID"))
                    If (IsAuth = "1") Then
                        Dim token = GenerateIdentifier(100)
                        Using con As New SqlConnection(conStr)
                            Using da As New SqlDataAdapter("InserSsoToken", con)
                                con.Open()
                                da.SelectCommand.CommandType = CommandType.StoredProcedure
                                da.SelectCommand.Parameters.AddWithValue("@EID", Convert.ToInt32(ds.Tables(0)(0)("EID")))
                                da.SelectCommand.Parameters.AddWithValue("@Key", token)
                                da.SelectCommand.Parameters.AddWithValue("@UID", UID)
                                Dim Succ = Convert.ToInt32(da.SelectCommand.ExecuteScalar())
                                If (Succ > 0) Then
                                    res.resCode = "1"
                                    res.resUrl = "https://" & Convert.ToString(ds.Tables(0)(0)("code")) & ".myndsaas.com/ssologin.aspx?token=" & token
                                    res.resMsg = "Success"
                                    Return res
                                Else
                                    Throw New Exception()
                                End If
                            End Using
                        End Using
                    ElseIf (IsAuth = "0") Then
                        res.resCode = "100"
                        res.resMsg = "User Created in BPM but account not activated."
                        Return res
                    Else
                        res.resCode = "3"
                        res.resMsg = "Login Issue in BPM."
                        Return res
                    End If
                Else
                    res.resMsg = "Invalid EmopoyeeCode."
                    Return res
                End If
            Else
                res.resMsg = "Invalid ApiKey."
                Return res
            End If

            Return res
        Catch ex As Exception
            res.resCode = "0"
            res.resMsg = "Login Issue in BPM."
            Data1.Append("ApiKey:" & ApiKey & "|EmopoyeeCode:" & EmopoyeeCode & "|RetUrl:" & RetUrl)
            CommanUtil.SaveServicerequest(Data1, "MyndBPMWS", "SsoAuth", "SSOauthfail_TCatch")
            '  sendMail("myndbpmws.SSOauth", ex.Message)
            Return res
        End Try
    End Function

    Public Shared Function sendMail(Source As String, Message As String) As String
        Dim ret As String = ""
        Dim mBody As String = "Hi Support Team (Live Server), <br/><br/> <b>Error Source : </b>" & Source & " <br/> <b>Error Message is </b><br/>" & Message & " <br/>"
        mBody = mBody & "<br/><br/>Error in web service "
        Dim Email As New System.Net.Mail.MailMessage("no-reply@myndsol.com", "sunil.pareek@myndsol.com", "Error In sso call", mBody)
        Dim mailClient As New System.Net.Mail.SmtpClient()
        ' Email.Bcc.Add("ajeet.kumar@myndsol.com")
        Email.IsBodyHtml = True
        Dim basicAuthenticationInfo As New System.Net.NetworkCredential("no-reply@myndsol.com", "vc79aK123AJ&$kL0")
        mailClient.Host = "mail.myndsol.com"
        mailClient.UseDefaultCredentials = False
        mailClient.Credentials = basicAuthenticationInfo
        Try
            mailClient.Send(Email)
        Catch ex As Exception
        End Try

        Return ret
    End Function


    Public Function GeneratePDF(EmpCode As String, Month As String, Year As String) As String Implements IMyndBPMWS.GeneratePDF
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStrPayRoll").ConnectionString
        Try
            Dim ret As String = "Data not found"
            Dim Qry = "select * FROM PDFImage with(nolock) where Emp_Code=@Emp_Code and month(PayDate)=@Month AND Year(PayDate)=@Year"
            Dim Ds As New DataSet()
            Using con As New SqlConnection(conStr)
                Using cmd As New SqlCommand(Qry, con)
                    cmd.Parameters.AddWithValue("@Emp_Code", EmpCode)
                    cmd.Parameters.AddWithValue("@Month", Month)
                    cmd.Parameters.AddWithValue("@Year", Year)
                    Using da As New SqlDataAdapter(cmd)
                        da.Fill(Ds)
                    End Using
                End Using
            End Using
            If (Ds.Tables(0).Rows.Count > 0) Then
                Dim imageData As Byte() = DirectCast(Ds.Tables(0).Rows(0).Item("FileData"), Byte())
                Dim FileName As String = Convert.ToString(Ds.Tables(0).Rows(0).Item("FileName"))
                'Convert the byte array to Image using Memory Stream
                Dim StrPath = HostingEnvironment.MapPath("~/Docs/PaySlip/") + FileName
                'Dim Base64String = Convert.ToBase64String(imageData)
                System.IO.File.WriteAllBytes(StrPath, imageData)
                ret = "https://myndsaas.com/Docs/PaySlip/" & FileName
                'Dim sPDFDecoded As Byte() = Convert.FromBase64String(Base64String)
                'Dim FileName1 As String = Convert.ToString("FromBase64.PDF")
                'Convert the byte array to Image using Memory Stream
                'Dim StrPath1 = Server.MapPath("~/Docs/PaySlip/") + FileName1
                'System.IO.File.WriteAllBytes(StrPath1, sPDFDecoded)
                'Using ms As New MemoryStream(imageData, 0, imageData.Length)
                '    ms.Write(imageData, 0, imageData.Length)
                '    'Set image variable value using memory stream. 
                '    Dim newImage = Image.FromStream(ms, True)
                'End Using
            Else
                ret = "Data not found"
            End If
            Return ret
        Catch ex As Exception
            Return "Error"
        End Try
    End Function


    Public Function Base64DecodedImageName(ByVal base64EncodedString As String, ByVal filePath As String) As String Implements IMyndBPMWS.Base64DecodedImageName
        Dim fileName As String = ""
        Dim imgName As String = ""
        Try
            imgName = System.DateTime.Now.Day & System.DateTime.Now.Year & System.DateTime.Now.Ticks & ".jpeg"
            imgName.Replace("/", "_")
            imgName.Replace(":", "_")
            Dim completeFilePath As String = HostingEnvironment.MapPath("~/" & filePath & "/" & imgName)
            File.WriteAllBytes(completeFilePath, Convert.FromBase64String(base64EncodedString))
            Return imgName
        Catch ex As Exception
            Return "Error Please Try Again"
        End Try

    End Function

    Public Function Base64DecodedImageNameNew(ByVal base64EncodedString As String, ByVal filePath As String, Optional ByVal ext As String = "jpg") As String Implements IMyndBPMWS.Base64DecodedImageNameNew
        Dim fileName As String = ""
        Dim imgName As String = ""
        Try
            imgName = System.DateTime.Now.Day & System.DateTime.Now.Year & System.DateTime.Now.Ticks & "." & ext
            imgName.Replace("/", "_")
            imgName.Replace(":", "_")
            Dim completeFilePath As String = HostingEnvironment.MapPath("~/" & filePath & "/" & imgName)
            File.WriteAllBytes(completeFilePath, Convert.FromBase64String(base64EncodedString))
            Return imgName
        Catch ex As Exception
            Return "Error Please Try Again"
        End Try

    End Function

    Function OcrInvoiceValues(Data As Stream) As XElement Implements IMyndBPMWS.OcrInvoiceValues
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim objDC As New DataClass
        Dim res As New HCL_VendorInvoiceVPRes()
        Try
            Dim reader As New StreamReader(Data)
            Dim strData As String = reader.ReadToEnd()
            Dim errorMsg As New StringBuilder()
            errorMsg.Append("<RESULT>")
            'Getting Credential from header Tag
            Dim UserName = WebOperationContext.Current.IncomingRequest.Headers("authKey")
            Dim Password = WebOperationContext.Current.IncomingRequest.Headers("authToken")
            'Hard coded UserName and password. As suggested by sunil Sir
            Dim DocumentType = "Test Vendor Invoice VP"
            'Getting field definition form data base for creating request string

            If (UserName.Trim = "Mynd_OcrIps" And Password = "Gr!(*$13hPc") Then
                Dim objDT As New DataTable()
                objDT = objDC.ExecuteQryDT("    set nocount on;Select DisplayName,InWardXMLTagName from MMM_MST_Fields with(nolock) where EID=124 and DocumentType='" & DocumentType & "'")
                Dim xmlDocRead As New XmlDocument()
                xmlDocRead.LoadXml(strData)
                If xmlDocRead.ChildNodes.Count >= 1 Then
                    Dim sb As New StringBuilder()
                    Dim SelNodesTxt As String = xmlDocRead.DocumentElement.Name
                    Dim nodes As XmlNodeList = xmlDocRead.SelectNodes(SelNodesTxt)
                    For Each node As XmlNode In nodes
                        For c As Integer = 0 To node.ChildNodes.Count - 1
                            Dim dvRow As DataView = objDT.DefaultView
                            dvRow.RowFilter = "InWardXMLTagName='" & Convert.ToString(node.ChildNodes.Item(c).Name) & "'"
                            Dim tbl As DataTable = dvRow.ToTable
                            If (tbl.Rows.Count > 0) Then
                                sb.Append("|").Append(tbl.Rows(0).Item("DisplayName")).Append("::").Append(Convert.ToString(node.ChildNodes.Item(c).InnerText))
                            End If
                        Next
                    Next
                    strData = sb.ToString()
                End If
                Dim Result = CommanUtil.ValidateParameterByDocumentType(124, DocumentType, 14207, strData)
                If Result.Contains("Your DocID is") Then
                    errorMsg.Append("<MESSAGE> Transaction received successfully</MESSAGE>")
                    res.resCode = "0"
                    res.resStr = "Transaction received successfully"
                Else
                    errorMsg.Append("<MESSAGE> " & Result.Replace("Error(s) in document " & DocumentType & ".", String.Empty) & "</MESSAGE>")
                    res.resCode = "Transaction failed."
                    Result = Result.Replace("Error(s) in document " & DocumentType & ".", String.Empty)
                    res.resStr = Result
                End If
                errorMsg.Append("</RESULT>")
                Dim xmldoc As XDocument = XDocument.Parse(errorMsg.ToString())
                Return xmldoc.Root
            End If
        Catch ex As Exception
        End Try
    End Function

End Class

<DataContractAttribute(Namespace:="")>
Public Class M1Discounting
    <DataMember(Name:="M1_FACTORING_UNIT_NO", Order:=1)>
    Public Property M1_FACTORING_UNIT_NO As String

    <DataMember(Name:="PAYABLE_DATE", Order:=1)>
    Public Property PAYABLE_DATE As String

    <DataMember(Name:="PAYABLE_AMOUNT", Order:=2)>
    Public Property PAYABLE_AMOUNT As String

End Class




<DataContractAttribute(Namespace:="")>
Public Class M1Registration
    <DataMember(Name:="BUYER_PANNUMBER", Order:=1)>
    Public Property BUYER_PANNUMBER As String

    <DataMember(Name:="SUPPLIER_PANNUMBER", Order:=1)>
    Public Property SUPPLIER_PANNUMBER As String

    <DataMember(Name:="DOCUMENTTYPE", Order:=2)>
    Public Property DOCUMENTTYPE As String

    <DataMember(Name:="EID", Order:=3)>
    Public Property EID As Integer
End Class


<DataContractAttribute(Namespace:="", Name:="Res")>
Public Class Grofers_Res
    <DataMember(Name:="resStr", Order:=1)>
    Public Property resStr As String
    <DataMember(Name:="resCode", Order:=2)>
    Public Property resCode As String
End Class

<DataContractAttribute(Namespace:="", Name:="Res")>
Public Class Hcl_Res
    <DataMember(Name:="resUrl", Order:=2)>
    Public Property resUrl As String
    <DataMember(Name:="resCode", Order:=1)>
    Public Property resCode As String

    <DataMember(Name:="resMsg", Order:=3)>
    Public Property resMsg As String
End Class

<DataContractAttribute(Namespace:="", Name:="Res")>
Public Class HCL_VendorInvoiceVPRes
    <DataMember(Name:="resStr", Order:=1)>
    Public Property resStr As String
    <DataMember(Name:="resCode", Order:=2)>
    Public Property resCode As String
End Class


