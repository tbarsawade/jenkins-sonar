' NOTE: You can use the "Rename" command on the context menu to change the class name "BPMMobile" in code, svc and config file together.
' NOTE: In order to launch WCF Test Client for testing this service, please select BPMMobile.svc or BPMMobile.svc.vb at the Solution Explorer and start debugging.
Imports System.Data
Imports System.Data.SqlClient
Imports System.Net
Imports System.IO
Imports System.Xml.Serialization
Imports System.Runtime.Serialization
Imports System.ServiceModel
Imports System.ServiceModel.Web
Imports System.Xml
Imports System.Web.Script.Serialization

Public Class BPMCustomWS
    Implements IBPMCustomWS
    Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
    Dim BPMKEY As String = ConfigurationManager.AppSettings("BPMKEY").ToString()


    'Function GETEIDDATA(Data As EIDDETAIL) As List(Of EIDRESPONSE) Implements IBPMCustomWS.GETEIDDATA
    '    Dim res As New List(Of EIDRESPONSE)
    '    Try
    '        Dim EID As Int32 = Data.EID
    '        Dim dt As New DataTable
    '        Dim dc As New DataClass()
    '        dt = dc.ExecuteQryDT("select * from mmm_mst_ftpfiletransfer where eid=" & EID)
    '        If dt.Rows.Count > 0 Then
    '            For Each dr As DataRow In dt.Rows
    '                Dim response As New EIDRESPONSE
    '                response.ftpid = dr("ftpid")
    '                response.EID = dr("EID")
    '                response.uid = dr("UID")
    '                response.gid = dr("GID")
    '                response.docType = Convert.ToString(dr("doctype"))
    '                response.fup_FieldMapping = Convert.ToString(dr("FUP_FIELDMAPPING"))
    '                response.loc_FieldMapping = Convert.ToString(dr("loc_FieldMapping"))
    '                response.BarCode = Convert.ToString(dr("barcode"))
    '                response.locid = dr("locid")
    '                response.ReadMode = Convert.ToString(dr("readmode"))
    '                response.HostName = Convert.ToString(dr("hostname"))
    '                response.UserName = Convert.ToString(dr("username"))
    '                response.Password = Convert.ToString(dr("Password"))
    '                response.Port = Convert.ToString(dr("Port"))
    '                response.PostFix = Convert.ToString(dr("PostFix"))
    '                res.Add(response)
    '            Next
    '        End If
    '    Catch ex As Exception

    '    End Try

    '    'Return res
    '    'Dim json As New JavaScriptSerializer()
    '    'Dim s As String = json.Serialize(res)
    '    Return res

    'End Function

    Function INWARD(Data As Stream) As XElement Implements IBPMCustomWS.INWARD
        Dim Result = ""
        Dim msg As String = ""
        Dim Key As String = "", EID As String = "", DocType As String = "", UID As String = 0
        Try
            'Convert steam into string 
            Dim reader As New StreamReader(Data)
            Dim strData As String = reader.ReadToEnd()
            'Decode it to UTF-8
            strData = HttpUtility.UrlDecode(strData)
            Dim Data1 As New StringBuilder(strData)

            Dim strr As String = readxmlandgivestring(strData)

            If strr.Length > 5 Then
                strr = strr.Replace("&", "&amp;")
            End If

            If strr.ToUpper().Contains("YOUR DOCID IS") = True Then
                msg = "<ENVELOPE> <HEADER><TALLYREQUEST>EXPORT VOUCHERS</TALLYREQUEST></HEADER> <BODY>  <DATA>   <VALIDATION> <RESULT> " & Trim(Replace(strr.ToUpper(), "YOUR DOCID IS", "")) & " </RESULT> <STATUS> SUCCESS </STATUS> </VALIDATION></DATA> </BODY></ENVELOPE>"
            Else
                msg = "<ENVELOPE> <HEADER><TALLYREQUEST>EXPORT VOUCHERS</TALLYREQUEST></HEADER> <BODY>  <DATA>   <VALIDATION> <RESULT> " & strr & " </RESULT> <STATUS> ERROR </STATUS> </VALIDATION></DATA> </BODY></ENVELOPE>"
            End If

            ' msg = "<ENVELOPE> <HEADER><TALLYREQUEST>EXPORT VOUCHERS</TALLYREQUEST></HEADER> <BODY>  <DATA>   <VALIDATION> <RESULT> " & strr & " </RESULT></VALIDATION></DATA> </BODY></ENVELOPE>"

            CommanUtil.SaveServicerequest(Data1, "BPMTallyInt", "Inward", msg)

        Catch ex As Exception
            ErrorLog.sendMail("BPMTallyInt.Inward", ex.Message)
            'Return "RTO"
        End Try
        Dim xmldoc As XDocument = XDocument.Parse(msg)
        Return xmldoc.Root


    End Function

    Function INWARDBULK(Data As Stream) As XElement Implements IBPMCustomWS.INWARDBULK
        Dim Result = ""
        Dim strr As String = ""
        Dim Key As String = "", EID As String = "", DocType As String = "", UID As String = 0
        Try
            'Convert steam into string 
            Dim reader As New StreamReader(Data)
            Dim strData As String = reader.ReadToEnd()
            'Decode it to UTF-8
            strData = HttpUtility.UrlDecode(strData)
            Dim Data1 As New StringBuilder(strData)

            strr = InsertInward_Bulk(strData)

            If strr.Length > 5 Then
                strr = strr.Replace("&", "&amp;")
            End If

            CommanUtil.SaveServicerequest(Data1, "BPMTallyInt", "InwardBulk", strr)

        Catch ex As Exception
            ErrorLog.sendMail("BPMTallyInt.InwardBulk", ex.Message)
            'Return "RTO"
        End Try
        Dim xmldoc As XDocument = XDocument.Parse(strr)
        Return xmldoc.Root


    End Function

    Function INWARDCANCEL(Data As Stream) As XElement Implements IBPMCustomWS.INWARDCANCEL
        Dim Result = ""
        Dim msg As String = ""
        Dim Key As String = "", EID As String = "", DocType As String = "", UID As String = 0
        Try
            '' here code to read xml for doc type and field name and create xml output string and retun xml string
            'Convert steam into string 
            Dim reader As New StreamReader(Data)
            Dim strData As String = reader.ReadToEnd()
            'Decode it to UTF-8
            strData = HttpUtility.UrlDecode(strData)
            Dim Data1 As New StringBuilder(strData)
            Dim strr As String = ReadXmlandgiveStringCancel(strData)

            If strr.Length > 5 Then
                strr = strr.Replace("&", "&amp;")
            End If

            Dim ResArr() As String = strr.Split("|")
            Dim Sdocid As Integer = 0
            If ResArr.Length = 2 Then
                Sdocid = ResArr(1).ToString
            End If

            If strr.ToUpper().Contains("DOCUMENT CANCELLED SUCCESSFULLY") = True Then
                msg = "<ENVELOPE> <HEADER><TALLYREQUEST>CANCEL VOUCHERS</TALLYREQUEST></HEADER> <BODY>  <DATA>   <VALIDATION> <RESULT> " & Sdocid & " </RESULT> <STATUS> SUCCESS </STATUS> </VALIDATION></DATA> </BODY></ENVELOPE>"
            Else
                msg = "<ENVELOPE> <HEADER><TALLYREQUEST>CANCEL VOUCHERS</TALLYREQUEST></HEADER> <BODY>  <DATA>   <VALIDATION> <RESULT> " & Sdocid & " </RESULT> <STATUS> ERROR </STATUS> </VALIDATION></DATA> </BODY></ENVELOPE>"
            End If
            CommanUtil.SaveServicerequest(Data1, "BPMTallyInt", "InwardCancel", msg)
        Catch ex As Exception
            ErrorLog.sendMail("BPMTallyInt.InwardCancel", ex.Message)
        End Try
        Dim xmldoc As XDocument = XDocument.Parse(msg)
        Return xmldoc.Root
    End Function

    Function OUTWARD(Data As Stream) As XElement Implements IBPMCustomWS.OUTWARD
        Dim Result As String = ""
        Dim Key As String = "", EID As Integer = 0, DocType As String = "", UID As String = 0
        Try
            '' here code to read xml for doc type and field name and create xml output string and retun xml string
            'Convert steam into string 
            Dim reader As New StreamReader(Data)
            Dim strData As String = reader.ReadToEnd()
            'Decode it to UTF-8
            strData = HttpUtility.UrlDecode(strData)
            Dim Data1 As New StringBuilder(strData)

            Dim strinput As String = ReturnInputParaValues(strData)
            Dim strinputArr() As String = strinput.Split("|")

            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As New SqlConnection(conStr)
            Dim da As New SqlDataAdapter("", con)
            If strinputArr.Length = 4 Then

                da.SelectCommand.CommandText = "Select eid from mmm_mst_entity where code='" & strinputArr(1).ToString().Trim & "'"
                If con.State <> ConnectionState.Open Then
                    con.Open()
                End If
                EID = Convert.ToInt32(da.SelectCommand.ExecuteScalar())

                da.SelectCommand.CommandText = "select rowfilterbpmfield from mmm_mst_forms where formname ='" & strinputArr(0).ToString & "' and eid=" & EID & ""


                Dim rwfield As String = da.SelectCommand.ExecuteScalar.ToString()
                Result = POrder(strinputArr(0).ToString, EID, strinputArr(2).ToString, rwfield.ToString(), strinputArr(3).ToString())
                If Result.Length > 5 Then
                    Result = Result.Replace("&", "&amp;")
                End If
                CommanUtil.SaveServicerequest(Data1, "BPMTallyInt", "Outward", Result)
            End If
            Dim xmldoc As XDocument = XDocument.Parse(Result)
            Return xmldoc.Root
        Catch ex As Exception
            ErrorLog.sendMail("BPMTallyInt.Outward", ex.Message & " : " & Result)
        End Try

    End Function

    Function OUTWARDWM(Data As Stream) As XElement Implements IBPMCustomWS.OUTWARDWM
        Dim Result As String = ""
        Dim Key As String = "", EID As Integer = 0, DocType As String = "", UID As String = 0
        Try
            '' here code to read xml for doc type and field name and create xml output string and retun xml string
            'Convert steam into string 
            Dim reader As New StreamReader(Data)
            Dim strData As String = reader.ReadToEnd()
            'Decode it to UTF-8
            strData = HttpUtility.UrlDecode(strData)
            Dim Data1 As New StringBuilder(strData)

            Dim strinput As String = ReturnInputParaValues(strData)
            Dim strinputArr() As String = strinput.Split("|")

            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As New SqlConnection(conStr)
            Dim da As New SqlDataAdapter("", con)
            If strinputArr.Length = 4 Then

                da.SelectCommand.CommandText = "Select eid from mmm_mst_entity where code='" & strinputArr(1).ToString().Trim & "'"
                If con.State <> ConnectionState.Open Then
                    con.Open()
                End If
                EID = Convert.ToInt32(da.SelectCommand.ExecuteScalar())

                da.SelectCommand.CommandText = "select rowfilterbpmfield from mmm_mst_forms where formname ='" & strinputArr(0).ToString & "' and eid=" & EID & ""


                Dim rwfield As String = da.SelectCommand.ExecuteScalar.ToString()
                Result = Return_OUtwardWithMaster(strinputArr(0).ToString, EID, strinputArr(2).ToString, rwfield.ToString(), strinputArr(3).ToString())
                If Result.Length > 5 Then
                    Result = Result.Replace("&", "&amp;")
                End If
            End If
            CommanUtil.SaveServicerequest(Data1, "BPMTallyWS", "Outward", "")
        Catch ex As Exception
            ErrorLog.sendMail("BPMTallyWS.Outward", ex.Message)
        End Try
        Dim xmldoc As XDocument = XDocument.Parse(Result)
        Return xmldoc.Root
    End Function

    Function REGISTER(Data As Stream) As XElement Implements IBPMCustomWS.REGISTER
        Dim Result = ""
        Try
            Dim reader As New StreamReader(Data)
            Dim strData As String = reader.ReadToEnd()
            strData = HttpUtility.UrlDecode(strData)
            Dim Data1 As New StringBuilder(strData)
            Result = readxmlandgivestringREGISTRATION(strData)
            If Result.Length > 5 Then
                Result = Result.Replace("&", "&amp;")
            End If
            CommanUtil.SaveServicerequest(Data1, "BPMTallyInt", "REGISTER", Result)
        Catch ex As Exception
            ErrorLog.sendMail("BPMTallyInt.REGISTER", ex.Message)
        End Try
        Dim xmldoc As XDocument = XDocument.Parse(Result)
        Return xmldoc.Root
    End Function

    Function CONFIRMATION(Data As Stream) As XElement Implements IBPMCustomWS.CONFIRMATION
        Dim Result = ""
        Try
            Dim reader As New StreamReader(Data)
            Dim strData As String = reader.ReadToEnd()

            strData = HttpUtility.UrlDecode(strData)
            Dim Data1 As New StringBuilder(strData)

            Result = readxmlandgivestringConfirmation(strData)

            If Result.Length > 5 Then
                Result = Result.Replace("&", "&amp;")
            End If

            CommanUtil.SaveServicerequest(Data1, "BPMTallyInt", "CONFIRMATION", Result)
        Catch ex As Exception
            ErrorLog.sendMail("BPMTallyInt.CONFIRMATION", ex.Message)
        End Try
        Dim xmldoc As XDocument = XDocument.Parse(Result)
        Return xmldoc.Root
    End Function

    Public Function ReadXmlandgiveStringCancel(ByRef str As String) As String
        Dim result As String = String.Empty
        Dim msg As String = "YOU ARE NOT AUTHORIZED"
        Dim xmlDocRead As New XmlDocument()
        xmlDocRead.LoadXml(str)

        Dim TransCountforLog As Integer = 0

        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim da As New SqlDataAdapter("", con)
        Dim ds As New DataSet

        Dim Data1 As New StringBuilder(str)

        Dim MainVar As String = String.Empty
        Dim globalVar As String = String.Empty
        Dim ChildVar As String = String.Empty
        Dim xmldoctype As String = String.Empty
        Dim xmlecode As String = String.Empty
        Dim apikey As String = String.Empty
        Dim xmlBPMTID As Integer = 0

        Dim xmleid As Integer = 0

        If xmlDocRead.ChildNodes.Count >= 1 Then

            Dim SelNodesTxt As String = xmlDocRead.DocumentElement.Name
            Dim Cnt As Integer = 0
            Dim Cntt As Integer = 0
            Dim nodes As XmlNodeList = xmlDocRead.SelectNodes(SelNodesTxt)
            For Each node As XmlNode In nodes
                Cnt += 1
                MainVar = String.Empty
                globalVar = String.Empty
                ChildVar = String.Empty
                xmldoctype = String.Empty
                xmlecode = String.Empty
                apikey = String.Empty
                xmlBPMTID = 0
                Dim docid As Integer = 0
                Dim Key As String = "", EID As String = "", DocType As String = "", UID As String = 0
                For c As Integer = 0 To node.ChildNodes.Count - 1
                    If UCase(node.ChildNodes.Item(c).Name) = "BODY" Then
                        For C1 As Integer = 0 To node.Item("BODY").ChildNodes.Count - 1
                            If node.Item("BODY").ChildNodes.Item(C1).Name = "DATA" Then
                                For c2 As Integer = 0 To node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Count - 1
                                    If node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).Name = "VALIDATION" Then
                                        For P As Integer = 0 To node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Count - 1
                                            If UCase(node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).Name) = "FORMNAME" Then
                                                xmldoctype = node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).InnerText.ToString()
                                            ElseIf UCase(node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).Name) = "ENTITY" Then
                                                xmlecode = node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).InnerText.ToString()
                                            ElseIf UCase(node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).Name) = "BPMTID" Then
                                                xmlBPMTID = Val(node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).InnerText)
                                            End If
                                        Next
                                        xmleid = 0
                                        If Not String.IsNullOrEmpty(xmlBPMTID) Then
                                            Dim cmd As New SqlCommand("select isnull(TallyIsActive,0)[Count] from mmm_mst_master where tid =" & xmlBPMTID & " ", con)

                                            If con.State <> ConnectionState.Open Then
                                                con.Open()
                                            End If
                                            Cntt = Convert.ToInt32(cmd.ExecuteScalar())

                                            If Cntt = 1 Then
                                                If Not String.IsNullOrEmpty(xmldoctype) And Not String.IsNullOrEmpty(xmlecode) Then
                                                    da.SelectCommand.CommandText = "Select eid from mmm_mst_entity where code='" & xmlecode.ToString() & "'"
                                                    da.SelectCommand.CommandType = CommandType.Text
                                                    If con.State <> ConnectionState.Open Then
                                                        con.Open()
                                                    End If
                                                    xmleid = Convert.ToInt32(da.SelectCommand.ExecuteScalar())

                                                    da.SelectCommand.CommandText = "Select * from mmm_mst_fields where eid=" & xmleid & " and documenttype='" & xmldoctype.ToString() & "' order by displayname "
                                                    da.SelectCommand.CommandType = CommandType.Text
                                                    da.Fill(ds, "data")
                                                End If
                                                'code to get API key of the entity
                                                If xmleid > 0 Then
                                                    da.SelectCommand.CommandText = "Select apikey from mmm_mst_entity where eid=" & xmleid & ""
                                                    da.Fill(ds, "apikey")
                                                    If ds.Tables("apikey").Rows.Count > 0 Then
                                                        apikey = ds.Tables("apikey").Rows(0).Item("apikey").ToString()
                                                    End If
                                                End If
                                            End If

                                        End If

                                    End If

                                    If Cntt = 1 Then
                                        Dim rowno As Integer = 0
                                        If node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).Name = "REQUESTDATA" Then
                                            For P As Integer = 0 To node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Count - 1
                                                If UCase(node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).Name) = "TALLYMESSAGE" Then
                                                    For d As Integer = 0 To node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).ChildNodes.Count - 1
                                                        If UCase(node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).ChildNodes(d).Name) = "DOCID" Then
                                                            docid = node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).ChildNodes(d).InnerText
                                                        End If
                                                    Next
                                                    Try
                                                        result = "Key$$" & apikey & "~DOCTYPE$$" & xmldoctype.ToString() & "~DOCID$$" & docid.ToString() & "~Data$$" & globalVar & ChildVar
                                                        Dim arrData As String() = Split(result, "~")
                                                        For i As Integer = 0 To arrData.Length - 1
                                                            Dim ar = Split(arrData(i), "$$")
                                                            If ar(0).ToUpper().Trim() = "KEY" Then
                                                                Key = ar(1)
                                                            ElseIf ar(0).ToUpper().Trim() = "DOCTYPE" Then
                                                                DocType = ar(1)
                                                            ElseIf ar(0).ToUpper().Trim() = "DATA" Then
                                                                result = ar(1)
                                                            End If
                                                        Next
                                                        Dim DsS As New DataSet()
                                                        DsS = AuthenticateWSRequest(Key)
                                                        If DsS.Tables(0).Rows.Count > 0 Then
                                                            EID = DsS.Tables(0).Rows(0).Item("EID")
                                                            UID = DsS.Tables(1).Rows(0).Item("uid")
                                                            Dim objUp As New UpdateData()
                                                            result = objUp.CancelDocument(DocType, EID, docid, UID)
                                                            'result = CommanUtil.ValidateParameterByDocumentType(EID, DocType, UID, result)
                                                        Else
                                                            result = "Sorry!!! Authentication failed."
                                                        End If
                                                        CommanUtil.SaveServicerequest(Data1, "BPMTallyWS", "InwardCancel", result)
                                                        msg = msg & result & "|" & docid
                                                        result = ""
                                                        globalVar = ""
                                                        ChildVar = ""
                                                    Catch ex As Exception
                                                        ErrorLog.sendMail("BPMTallyInt.InwardCancel", ex.Message)
                                                        result = ""
                                                        globalVar = ""
                                                        ChildVar = ""
                                                    End Try
                                                End If
                                                TransCountforLog += 1
                                            Next
                                        End If
                                    Else
                                        msg = "YOU ARE NOT AUTHORIZED"
                                    End If

                                Next
                            End If
                        Next
                    End If
                Next
            Next
        End If
        '' here
        If msg.ToUpper().Contains("DOCUMENT CANCELLED SUCCESSFULLY") Then
            Call InsertinTallyIntegrationLog(xmleid, xmlBPMTID, xmldoctype, "INWARDCANCEL", xmldoctype, TransCountforLog, "SUCCESS", msg)
        Else
            Call InsertinTallyIntegrationLog(xmleid, xmlBPMTID, xmldoctype, "INWARDCANCEL", xmldoctype, TransCountforLog, "FAIL", msg)
        End If
        Return msg
    End Function

    Public Function POrder(ByVal Dtype As String, ByVal eid As Integer, ByVal DistCode As String, ByVal rowfiltercolumn As String, ByVal BPMTID As String) As String
        Try


            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As SqlConnection = New SqlConnection(conStr)
            Dim oda As SqlDataAdapter = New SqlDataAdapter("InsertSMSALERTLog", con)
            Dim doc As XmlDocument = New XmlDocument()
            Dim Vdt As New DataTable
            Dim SITdt As New DataTable
            Dim Invtrdt As New DataTable

            Dim TransCountforLog As Integer = 0

            'Dim ds As New DataSet
            Dim strB As StringBuilder = New System.Text.StringBuilder()
            Dim objRel As New Relation()
            Dim ds As New DataSet()
            Dim dsD As New DataSet()
            ds = objRel.GetAllFields(eid)
            Dim StrQuery = GenearateQuery1(eid, Dtype, ds)
            Dim check As Integer = 0
            strB.Append("<ENVELOPE>")
            strB.Append("<HEADER>")
            strB.Append("<RESPONSE>" & "IMPORT DATA")
            strB.Append("</RESPONSE>")
            strB.Append("</HEADER>")
            strB.Append("<BODY>")
            strB.Append("<DATA>")
            strB.Append("<VALIDATION>")
            If Not String.IsNullOrEmpty(BPMTID) Then
                oda.SelectCommand.CommandText = "Select isnull(TallyIsActive,0) from mmm_mst_master where tid=" & Val(BPMTID) & ""
                If con.State <> ConnectionState.Open Then
                    con.Open()
                End If
                check = Convert.ToInt32(oda.SelectCommand.ExecuteScalar())


                If check = 1 Then
                    If DistCode Is Nothing Then
                        StrQuery = "SELECT  v" & eid & Dtype.Trim.Replace(" ", "_") & ".tid  AS DOCID, " & StrQuery
                    Else
                        Dim filter As String = " and v" & eid & Dtype.Trim.Replace(" ", "_") & ".[" & rowfiltercolumn & "]='" & DistCode & "' and v" & eid & Dtype.Trim.Replace(" ", "_") & ".[ISEXPORT]=1"
                        StrQuery = "SELECT  v" & eid & Dtype.Trim.Replace(" ", "_") & ".tid  AS DOCID, " & StrQuery & filter
                    End If

                    oda = New SqlDataAdapter(StrQuery, con)
                    oda.Fill(Vdt)

                    Dim flds As New DataTable
                    'query to get fields of the documenttype
                    Dim fldqry As String = "Select displayname,fieldmapping,fieldtype,dropdown,OUTWARDXMLTAGNAME,DROPDOWNTYPE  from mmm_mst_fields where eid=" & eid & " and documenttype='" & Dtype.ToString.Trim() & "' and OUTWARDXMLTAGNAME is not null"
                    oda = New SqlDataAdapter(fldqry, con)
                    oda.Fill(flds)

                    strB.Append("<BILLINGCODE>" & "IN013205004B")
                    strB.Append("</BILLINGCODE>")
                    strB.Append("<PASSWD>" & "XXXXX")
                    strB.Append("</PASSWD>")
                    strB.Append("</VALIDATION>")
                    strB.Append("<IMPORTDATA>")
                    strB.Append("<TALLYMESSAGE>")
                    strB.Append("<VOUCHERS>")

                    Dim opentag As String = ""
                    Dim closetag As String = ""
                    Dim val As String = ""
                    Dim str As String = ""
                    Dim strchild As String = ""
                    Dim childopentag As String = ""
                    Dim childclosetag As String = ""
                    Dim childval As String = ""

                    Dim masteropentag As String = ""
                    Dim masterclosetag As String = ""
                    Dim masterval As String = ""
                    Dim asd As String = ""
                    Dim stockitems As String = ""
                    Dim INVENTORYENTRIES As String = ""
                    Dim maindoc As String = ""
                    Dim SSS As String = ""
                    Dim xxx As String = ""
                    Dim kk As String = ""
                    For j As Integer = 0 To Vdt.Rows.Count - 1
                        maindoc = "<BPMTID>" & Vdt.Rows(j).Item("DOCID").ToString() & "</BPMTID>"
                        INVENTORYENTRIES = ""
                        stockitems = ""
                        For i As Integer = 0 To flds.Rows.Count - 1
                            opentag = "<" & flds.Rows(i).Item("OUTWARDXMLTAGNAME").ToString() & ">"
                            val = ""
                            strchild = ""

                            ''
                            If UCase(flds.Rows(i).Item("fieldtype")).ToString.Trim() = "CHILD ITEM" Then
                                oda.SelectCommand.CommandText = "Select displayname,fieldmapping,fieldtype,dropdown,dropdowntype,OUTWARDXMLTAGNAME,documenttype  from mmm_mst_fields where eid=" & eid & " and documenttype='" & UCase(flds.Rows(i).Item("dropdown")).ToString.Trim() & "' and OUTWARDXMLTAGNAME is not null "
                                Dim child As New DataTable
                                oda.Fill(child)

                                If child.Rows.Count > 0 Then
                                    Dim childquery = GenearateQuery1(eid, UCase(flds.Rows(i).Item("dropdown")).ToString.Trim(), ds)
                                    Dim filter As String = " where  DOCID='" & Vdt.Rows(j).Item("DOCID").ToString() & "'"
                                    childquery = "SELECT * from  v" & eid & flds.Rows(i).Item("dropdown").ToString().Trim.Replace(" ", "_") & filter
                                    oda = New SqlDataAdapter(childquery, con)
                                    oda.SelectCommand.CommandTimeout = 5000
                                    oda.SelectCommand.CommandType = CommandType.Text
                                    Dim childv As New DataTable
                                    Dim Qry As String = ""
                                    oda.Fill(childv)

                                    For c As Integer = 0 To childv.Rows.Count - 1

                                        For k As Integer = 0 To child.Rows.Count - 1
                                            For Each col As DataColumn In childv.Columns
                                                If UCase(child.Rows(k).Item("displayname")).ToString() = UCase(col.ColumnName) Then
                                                    childopentag = "<" & child.Rows(k).Item("OUTWARDXMLTAGNAME").ToString() & ">"
                                                    childval = ""
                                                    masterval = ""
                                                    If child.Rows(k).Item("dropdowntype") = "MASTER VALUED" Then
                                                        Qry = "select dms.udf_split('" & child.Rows(k).Item("dropdown").ToString() & "'," & child.Rows(k).Item("fieldmapping").ToString() & ") [value] " & " from MMM_MST_DOC_item  where  documenttype='" & child.Rows(k).Item("documenttype").ToString() & "' and docid=" & Vdt.Rows(j).Item("DOCID").ToString() & " and tid=" & childv.Rows(c).Item("tid")
                                                        oda.SelectCommand.CommandType = CommandType.Text
                                                        Dim mval As String = oda.SelectCommand.ExecuteScalar().ToString
                                                        Dim ss As String() = child.Rows(k).Item("dropdown").ToString().ToString.Split("-")
                                                        oda.SelectCommand.CommandText = "select " & ss(2).ToString() & IIf(UCase(ss(0)).ToString = "MASTER", " from mmm_mst_master ", " from mmm_mst_doc ") & " where documenttype='" & ss(1).ToString() & "' and eid=" & eid & " and tid=" & childv.Rows(c).Item(col.ColumnName)
                                                        oda.SelectCommand.CommandTimeout = 5000
                                                        If con.State <> ConnectionState.Open Then
                                                            con.Open()
                                                        End If
                                                        childval = oda.SelectCommand.ExecuteScalar()
                                                    Else
                                                        childval = childv.Rows(c).Item(col.ColumnName).ToString()
                                                    End If
                                                End If
                                            Next
                                            childclosetag = "</" & child.Rows(k).Item("OUTWARDXMLTAGNAME").ToString() & ">"
                                            strchild = strchild & childopentag & childval & childclosetag
                                        Next
                                        strchild = strchild & "</INVENTORYENTRY>"
                                        kk = kk & "<INVENTORYENTRY>" & strchild
                                        strchild = ""
                                    Next

                                End If
                                closetag = "</" & flds.Rows(i).Item("OUTWARDXMLTAGNAME").ToString() & ">"
                                str = str & opentag & strchild & val & closetag
                                INVENTORYENTRIES = kk
                                kk = ""
                            ElseIf UCase(flds.Rows(i).Item("fieldtype")).ToString.Trim() = "DROP DOWN" Then
                                ' to be changed here by sp for master valued 
                                'select dms.udf_split('MASTER-Super Distributor-fld1',d.fld10) [iname] from MMM_MST_DOC D  where  D.EID=43  and D.TID=456374
                                '  Qry = "select dms.udf_split('" & child.Rows(k).Item("dropdown").ToString() & "'," & child.Rows(k).Item("fieldmapping").ToString() & ") [value] " & " from MMM_MST_DOC_item  where  documenttype='" & child.Rows(k).Item("documenttype").ToString() & "' and docid=" & Vdt.Rows(j).Item("DOCID").ToString()

                                For Each column As DataColumn In Vdt.Columns
                                    If UCase(column.ColumnName) = UCase(flds.Rows(i).Item("OUTWARDXMLTAGNAME")).ToString() Then
                                        val = Vdt.Rows(j).Item(column.ColumnName).ToString()
                                    End If
                                Next
                                closetag = "</" & flds.Rows(i).Item("OUTWARDXMLTAGNAME").ToString() & ">"
                                str = str & opentag & strchild & val & closetag
                                maindoc = maindoc & opentag & val & closetag
                                ' masterval = mastervalued(flds.Rows(i).Item("dropdown").ToString(), Vdt.Rows(j).Item("DOCID").ToString(), eid)
                            Else
                                For Each column As DataColumn In Vdt.Columns
                                    If UCase(column.ColumnName) = UCase(flds.Rows(i).Item("OUTWARDXMLTAGNAME")).ToString() Then
                                        val = Vdt.Rows(j).Item(column.ColumnName).ToString()
                                    End If
                                Next
                                closetag = "</" & flds.Rows(i).Item("OUTWARDXMLTAGNAME").ToString() & ">"
                                str = str & opentag & strchild & val & closetag
                                maindoc = maindoc & opentag & val & closetag
                            End If
                        Next
                        maindoc = "<VOUCHER>" & maindoc & "<INVENTORYENTRIES>" & INVENTORYENTRIES & "</INVENTORYENTRIES></VOUCHER>"
                        xxx = xxx & maindoc
                    Next

                    strB.Append(xxx)
                    strB.Append("</VOUCHERS>")
                    strB.Append("</TALLYMESSAGE>")
                    strB.Append("</IMPORTDATA>")

                    strB.Append("</DATA>")
                    strB.Append("</BODY>")
                    strB.Append("</ENVELOPE>")
                Else
                    strB.Append("<RESULT>")
                    strB.Append("YOU ARE NOT AUTHORIZED")
                    strB.Append("</RESULT>")
                    strB.Append("</VALIDATION>")
                    strB.Append("</DATA>")
                    strB.Append("</BODY>")
                    strB.Append("</ENVELOPE>")
                End If

            Else
                strB.Append("<RESULT>")
                strB.Append("YOU ARE NOT AUTHORIZED")
                strB.Append("</RESULT>")
                strB.Append("</VALIDATION>")
                strB.Append("</DATA>")
                strB.Append("</BODY>")
                strB.Append("</ENVELOPE>")

            End If
            TransCountforLog = Vdt.Rows.Count
            If TransCountforLog > 0 Then
                Call InsertinTallyIntegrationLog(eid, BPMTID, Dtype, "OUTWARD", Dtype, TransCountforLog, "SUCCESS", "Records")
            Else
                Call InsertinTallyIntegrationLog(eid, BPMTID, Dtype, "OUTWARD", Dtype, TransCountforLog, "FAIL", "No records")
            End If

            con.Dispose()
            ' ErrorLog.sendMail("BPMTallyInt.PORDER", strB.ToString())
            Return strB.ToString()
        Catch ex As Exception
            Throw
            ErrorLog.sendMail("BPMTallyInt.Porder", ex.Message)
        End Try
    End Function

    Function MASTERSYNC(Data As Stream) As XElement Implements IBPMCustomWS.MASTERSYNC
        Dim Result As String = ""
        Dim Key As String = "", EID As Integer = 0, DocType As String = "", UID As String = 0, BPMTID As Integer = 0
        Try
            '' here code to read xml for doc type and field name and create xml output string and retun xml string
            'Convert steam into string 
            Dim reader As New StreamReader(Data)
            Dim strData As String = reader.ReadToEnd()
            'Decode it to UTF-8
            strData = HttpUtility.UrlDecode(strData)
            Dim Data1 As New StringBuilder(strData)

            Dim strinput As String = ReturnSyncInputParaValues(strData)
            Dim strinputArr() As String = strinput.Split("|")

            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As New SqlConnection(conStr)
            Dim da As New SqlDataAdapter("", con)
            If strinputArr.Length = 2 Then

                da.SelectCommand.CommandText = "Select eid from mmm_mst_entity where code='" & strinputArr(0).ToString().Trim & "'"
                If con.State <> ConnectionState.Open Then
                    con.Open()
                End If
                EID = Convert.ToInt32(da.SelectCommand.ExecuteScalar())
                BPMTID = strinputArr(1).ToString().Trim

                Result = ReturnMasterbySync(EID, BPMTID)
                If Result.Length > 5 Then
                    Result = Result.Replace("&", "&amp;")
                End If

                ' da.SelectCommand.CommandText = "update MMM_MST_TallyRegInfo set lastupdatedon=getdate() where bpmtid=" & BPMTID & " and eid=" & EID & ""
                If con.State <> ConnectionState.Open Then
                    con.Open()
                End If
                '    Convert.ToInt32(da.SelectCommand.ExecuteNonQuery())
                CommanUtil.SaveServicerequest(Data1, "BPMTallyInt", "MASTERSYNC", Result)
            End If
            Dim xmldoc As XDocument = XDocument.Parse(Result)
            Return xmldoc.Root
        Catch ex As Exception
            Throw
            ErrorLog.sendMail("BPMTallyInt.MASTERSYNC", ex.Message & " : " & Result)
        End Try
    End Function

    Protected Function mastervalued(ByVal str As String, ByVal tid As String, ByVal eid As Integer) As String
        Try
            Dim Result As String = ""
            If tid = "" Or tid = "0" Then
                Return ""
                Exit Function
            End If

            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As SqlConnection = New SqlConnection(conStr)
            Dim oda As New SqlDataAdapter("", con)
            Dim ds As New DataSet
            Dim masterval As String = ""
            Dim val As String = ""
            Dim hold As String() = str.ToString.Split("-")
            Dim mopentag As String = ""
            Dim mclosetag As String = ""

            If hold.Length > 0 Then
                oda.SelectCommand.CommandText = "Select displayname,fieldmapping,dropdown,dropdowntype,outwardxmltagname from mmm_mst_fields where eid=" & eid & " and documenttype='" & hold(1).ToString() & "' and outwardxmltagname is not null"
                oda.Fill(ds, "data")

                If ds.Tables("data").Rows.Count > 0 Then
                    oda.SelectCommand.CommandText = "Select * from v" & eid & hold(1).ToString().Trim.Replace(" ", "_") & " where tid=" & tid & " "
                    oda.Fill(ds, "master")
                    If ds.Tables("master").Rows.Count > 0 Then

                        For j As Integer = 0 To ds.Tables("master").Rows.Count - 1
                            For i As Integer = 0 To ds.Tables("data").Rows.Count - 1
                                mopentag = "<" & ds.Tables("data").Rows(i).Item("outwardxmltagname").ToString() & ">"
                                For Each column As DataColumn In ds.Tables("master").Columns
                                    If UCase(column.ColumnName) = UCase(ds.Tables("data").Rows(i).Item("displayname")).ToString() Then
                                        'If ds.Tables("data").Rows(i).Item("dropdowntype").ToString() = "MASTER VALUED" Then
                                        '    Dim ab As String() = ds.Tables("data").Rows(i).Item("dropdown").ToString().Split("-")
                                        '    If UCase(ab(0)).ToString = "MASTER" Then
                                        '        oda.SelectCommand.CommandText = "Select " & ab(2).ToString() & " from mmm_mst_master where eid=" & eid & " and tid=" & ds.Tables("master").Rows(j).Item(column.ColumnName).ToString() & ""
                                        '        oda.SelectCommand.CommandTimeout = 5000
                                        '        If con.State <> ConnectionState.Open Then
                                        '            con.Open()
                                        '        End If
                                        '        val = oda.SelectCommand.ExecuteScalar()
                                        '    ElseIf UCase(ab(0)).ToString = "STATIC" Then
                                        '        oda.SelectCommand.CommandText = "Select " & ab(2).ToString() & " from mmm_mst_doc where eid=" & eid & " and tid=" & ds.Tables("master").Rows(j).Item(column.ColumnName).ToString() & ""
                                        '        oda.SelectCommand.CommandTimeout = 5000
                                        '        val = oda.SelectCommand.ExecuteScalar()
                                        '    End If
                                        'Else
                                        val = ds.Tables("master").Rows(j).Item(column.ColumnName).ToString()
                                        ' End If
                                    End If
                                Next
                                mclosetag = "</" & ds.Tables("data").Rows(i).Item("outwardxmltagname").ToString() & ">"
                                Result = Result & mopentag & val & mclosetag
                            Next
                        Next
                    End If
                End If
            End If
            Return Result
        Catch ex As Exception
            ErrorLog.sendMail("BPMCustWS.Outward_porder_mastervalued", ex.Message)
            Throw
        End Try
    End Function

    Public Function ReturnMasterbySync(ByVal eid As Integer, ByVal BPMTID As String) As String
        Try

            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As SqlConnection = New SqlConnection(conStr)
            Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
            Dim doc As XmlDocument = New XmlDocument()
            Dim Vdt As New DataTable
            Dim SITdt As New DataTable
            Dim Invtrdt As New DataTable
            'Dim ds As New DataSet
            Dim strB As StringBuilder = New System.Text.StringBuilder()
            Dim objRel As New Relation()
            Dim ds As New DataSet()
            Dim dsD As New DataSet()
            ds = objRel.GetAllFields(eid)


            Dim check As Integer = 0
            Dim count As Integer = 0
            Dim dtReg As New DataTable
            strB.Append("<ENVELOPE>")
            strB.Append("<HEADER>")
            strB.Append("<RESPONSE>" & "IMPORT DATA")
            strB.Append("</RESPONSE>")
            strB.Append("</HEADER>")
            strB.Append("<BODY>")
            strB.Append("<DATA>")
            strB.Append("<VALIDATION>")
            strB.Append("<BPMTID>" & BPMTID & "</BPMTID>")


            If Not String.IsNullOrEmpty(BPMTID) Then
                oda.SelectCommand.CommandText = "select * from MMM_MST_TallyRegInfo where bpmtid=" & BPMTID & " and eid=" & eid & " and isactive=1"
                oda.Fill(dtReg)
                count = dtReg.Rows.Count
                'count = Convert.ToInt32(oda.SelectCommand.ExecuteScalar())

                oda.SelectCommand.CommandText = "Select isnull(TallyIsActive,0) from mmm_mst_master where tid=" & Val(BPMTID) & ""
                If con.State <> ConnectionState.Open Then
                    con.Open()
                End If
                check = Convert.ToInt32(oda.SelectCommand.ExecuteScalar())


                If check = 1 And count = 1 Then
                    '' for getting master for enabled to sync
                    Dim dtfrm As New DataTable
                    oda.SelectCommand.CommandText = "select formname,formid,eid,formsource from MMM_MST_FORMS where eid=" & eid & " AND isnull(enablesync_tally,0)=1 order by Sync_Tally_Ordering "
                    oda.Fill(dtfrm)

                    Dim LastupdatedOn As DateTime
                    Dim isDateempty As Boolean = False
                    If Not IsDBNull(dtReg.Rows(0).Item("lastupdatedon")) Then
                        LastupdatedOn = Convert.ToDateTime(dtReg.Rows(0).Item("lastupdatedon"))
                    Else
                        isDateempty = True
                    End If

                    Dim FormOpenTag As String = "" : Dim FormCloseTag As String = ""

                    Dim opentag As String = "" : Dim closetag As String = "" : Dim val As String = "" : Dim str As String = "" : Dim strchild As String = ""
                    Dim childopentag As String = "" : Dim childclosetag As String = "" : Dim childval As String = ""
                    Dim masteropentag As String = "" : Dim masterclosetag As String = "" : Dim masterval As String = "" : Dim asd As String = ""
                    Dim stockitems As String = "" : Dim INVENTORYENTRIES As String = "" : Dim maindoc As String = "" : Dim SSS As String = ""
                    Dim xxx As String = "" : Dim kk As String = ""

                    strB.Append("</VALIDATION>")
                    strB.Append("<TALLYMESSAGE>")

                    For F As Integer = 0 To dtfrm.Rows.Count - 1  ' ONE BY ONE ALL FORMS LOOP 
                        Dim Dtype As String
                        Dtype = dtfrm.Rows(F).Item("formname").ToString
                        strB.Append("<" & Dtype.Trim.Replace(" ", "_") & ">")
                        Dim StrQuery = GenearateQuery1(eid, Dtype, ds)
                        If isDateempty = True Then
                            StrQuery = "SELECT  " & StrQuery
                        Else
                            Dim filter As String = " AND convert(datetime, v" & eid & Dtype.Trim.Replace(" ", "_") & ".[LASTUPDATE])>'" & LastupdatedOn & "'"
                            StrQuery = "SELECT   " & StrQuery & filter
                        End If
                        ''start check if filter is set on any master to be sync - by sp 06_apr_15
                        Dim SyncFilterCond As String = CheckandReturnFilterCond(Dtype, BPMTID, eid)
                        If Len(SyncFilterCond) > 2 Then
                            StrQuery = StrQuery & " AND " & SyncFilterCond
                        End If
                        ''ends check if filter is set on any master to be sync - by sp

                        Vdt = New DataTable()
                        oda = New SqlDataAdapter(StrQuery, con)
                        oda.Fill(Vdt)
                        For j As Integer = 0 To Vdt.Rows.Count - 1
                            strB.Append("<ROW>")
                            For Each column As DataColumn In Vdt.Columns
                                strB.Append("<").Append(column.ColumnName).Append(">").Append(Vdt.Rows(j).Item(column.ColumnName)).Append("</").Append(column.ColumnName).Append(">")
                            Next
                            strB.Append("</ROW>")
                        Next    ' rows loop 
                        strB.Append("</" & Dtype.Trim.Replace(" ", "_") & ">")

                        '' for log by sp
                        If Vdt.Rows.Count > 0 Then
                            Call InsertinTallyIntegrationLog(eid, BPMTID, Dtype, "MASTERSYNC", Dtype, Vdt.Rows.Count, "SUCCESS", "Records")
                        Else
                            Call InsertinTallyIntegrationLog(eid, BPMTID, Dtype, "MASTERSYNC", Dtype, Vdt.Rows.Count, "FAIL", "No records")
                        End If
                        '' for log by sp
                    Next
                    strB.Append("</TALLYMESSAGE>")
                    strB.Append("</DATA>")
                    strB.Append("</BODY>")
                    strB.Append("</ENVELOPE>")
                Else
                    strB.Append("<RESULT>")
                    strB.Append("YOU ARE NOT AUTHORIZED")
                    strB.Append("</RESULT>")
                    strB.Append("</VALIDATION>")
                    strB.Append("</DATA>")
                    strB.Append("</BODY>")
                    strB.Append("</ENVELOPE>")
                End If
            Else
                strB.Append("<RESULT>")
                strB.Append("YOU ARE NOT AUTHORIZED")
                strB.Append("</RESULT>")
                strB.Append("</VALIDATION>")
                strB.Append("</DATA>")
                strB.Append("</BODY>")
                strB.Append("</ENVELOPE>")
            End If

            con.Dispose()
            ''ErrorLog.sendMail("BPMCustomWS.PORDER", strB.ToString())
            Return strB.ToString()
        Catch ex As Exception
            Throw
            ErrorLog.sendMail("BPMTallyInt.MASTERSYNC", ex.Message)
        End Try
    End Function

    Private Function CheckandReturnFilterCond(ByVal SyncForm As String, ByVal RegTID As Integer, EID As Integer) As String
        '' by sp on 06_apr_15 for master filter
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
        Dim FilterCond As String = ""
        Try
            oda.SelectCommand.CommandText = "select FORMNAME from MMM_MST_TallyRegInfo where bpmtid=" & RegTID & " and eid=" & EID & " and isactive=1"

            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            Dim RegForm As String = ""
            RegForm = Convert.ToString(oda.SelectCommand.ExecuteScalar())

            Dim StrQry As String = ""
            StrQry = "select * from mmm_TallySync_filter where eid=" & EID & " and sourceform='" & RegForm & "' and targetform='" & SyncForm & "'"

            oda.SelectCommand.CommandText = StrQry

            Dim dtA As New DataTable

            oda.Fill(dtA)
            For i As Integer = 0 To dtA.Rows.Count - 1
                If dtA.Rows(i).Item("Operands").ToString().Trim() = "=" Then
                    StrQry = "select " & dtA.Rows(i).Item("sourceFields").ToString() & " from mmm_mst_master where eid=" & EID & " and documenttype='" & dtA.Rows(i).Item("sourceform").ToString() & "' and tid=" & RegTID
                    Dim RegFieldVal As String = ""
                    oda.SelectCommand.CommandText = StrQry
                    RegFieldVal = Convert.ToString(oda.SelectCommand.ExecuteScalar())

                    StrQry = "select displayname from mmm_mst_fields where fieldmapping='" & dtA.Rows(i).Item("TargetFields").ToString() & "' and eid=" & EID & " and documenttype='" & dtA.Rows(i).Item("Targetform").ToString() & "'"
                    oda.SelectCommand.CommandText = StrQry
                    Dim TDispName As String
                    TDispName = Convert.ToString(oda.SelectCommand.ExecuteScalar())

                    FilterCond = FilterCond & " v" & EID & dtA.Rows(i).Item("Targetform").Trim.Replace(" ", "_") & ".[" & TDispName & "]='" & RegFieldVal & "' and "
                Else
                    StrQry = "select isnull(" & dtA.Rows(i).Item("sourceFields").ToString() & ",0) from mmm_mst_master where documenttype='" & dtA.Rows(i).Item("sourceform").ToString() & "' and eid=" & EID & " and tid=" & RegTID
                    Dim RegFieldVal As String = ""
                    oda.SelectCommand.CommandText = StrQry
                    RegFieldVal = Convert.ToString(oda.SelectCommand.ExecuteScalar())
                    StrQry = "select displayname from mmm_mst_fields  where  documenttype='" & dtA.Rows(i).Item("Targetform").ToString() & "' and fieldmapping='" & dtA.Rows(i).Item("TargetDepFldMapping").ToString() & "' and eid=" & EID
                    oda.SelectCommand.CommandText = StrQry
                    Dim TDispName As String
                    TDispName = Convert.ToString(oda.SelectCommand.ExecuteScalar())
                    Dim tempRegFieldVal As String() = RegFieldVal.ToString().Split(",")
                    For ival As Integer = 0 To tempRegFieldVal.Length - 1
                        'If Convert.ToString(tempRegFieldVal(ival)) <> 0 Then
                        FilterCond = FilterCond & "','+ v" & EID & dtA.Rows(i).Item("Targetform").Trim.Replace(" ", "_") & ".[" & TDispName & "]+',' like '%," & Convert.ToString(tempRegFieldVal(ival)) & ",%' or "
                        'End If
                    Next
                End If

            Next
            If FilterCond.Length > 1 Then
                FilterCond = Left(FilterCond, Len(FilterCond) - 4)
            End If

            dtA = Nothing
            Return FilterCond
        Catch ex As Exception
            Return FilterCond
            con.Close()
            con.Dispose()
            oda.Dispose()
        Finally
            con.Close()
            con.Dispose()
            oda.Dispose()
        End Try
    End Function



    '' prev without contains filter
    'Private Function CheckandReturnFilterCond(ByVal SyncForm As String, ByVal RegTID As Integer, EID As Integer) As String
    '    '' by sp on 06_apr_15 for master filter
    '    Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
    '    Dim con As SqlConnection = New SqlConnection(conStr)
    '    Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
    '    Dim FilterCond As String = ""
    '    Try
    '        oda.SelectCommand.CommandText = "select FORMNAME from MMM_MST_TallyRegInfo where bpmtid=" & RegTID & " and eid=" & EID & " and isactive=1"

    '        If con.State <> ConnectionState.Open Then
    '            con.Open()
    '        End If
    '        Dim RegForm As String = ""
    '        RegForm = Convert.ToString(oda.SelectCommand.ExecuteScalar())

    '        Dim StrQry As String = ""
    '        StrQry = "select * from mmm_TallySync_filter where eid=" & EID & " and sourceform='" & RegForm & "' and targetform='" & SyncForm & "'"

    '        oda.SelectCommand.CommandText = StrQry

    '        Dim dtA As New DataTable

    '        oda.Fill(dtA)
    '        For i As Integer = 0 To dtA.Rows.Count - 1
    '            StrQry = "select " & dtA.Rows(i).Item("sourceFields").ToString() & " from mmm_mst_master where eid=" & EID & " and documenttype='" & dtA.Rows(i).Item("sourceform").ToString() & "' and tid=" & RegTID
    '            Dim RegFieldVal As String = ""
    '            oda.SelectCommand.CommandText = StrQry
    '            RegFieldVal = Convert.ToString(oda.SelectCommand.ExecuteScalar())

    '            StrQry = "select displayname from mmm_mst_fields where fieldmapping='" & dtA.Rows(i).Item("TargetFields").ToString() & "' and eid=" & EID & " and documenttype='" & dtA.Rows(i).Item("Targetform").ToString() & "'"
    '            oda.SelectCommand.CommandText = StrQry
    '            Dim TDispName As String
    '            TDispName = Convert.ToString(oda.SelectCommand.ExecuteScalar())

    '            FilterCond = FilterCond & " v" & EID & dtA.Rows(i).Item("Targetform").Trim.Replace(" ", "_") & ".[" & TDispName & "]='" & RegFieldVal & "' and "
    '        Next
    '        If FilterCond.Length > 1 Then
    '            FilterCond = Left(FilterCond, Len(FilterCond) - 4)
    '        End If

    '        dtA = Nothing
    '        Return FilterCond
    '    Catch ex As Exception
    '        Return FilterCond
    '        con.Close()
    '        con.Dispose()
    '        oda.Dispose()
    '    Finally
    '        con.Close()
    '        con.Dispose()
    '        oda.Dispose()
    '    End Try
    'End Function

    Public Function GenearateQuery1(EID As Integer, DocumentType As String, ds As DataSet, Optional EnableEdit As String = "") As String
        Dim ret As String = ""
        Dim View As DataView
        Dim tbl As DataTable
        Dim tblRe As DataTable
        Dim StrColumn As String = ""
        Dim StrJoinString As String = ""
        Dim SchemaString As String = DocumentType
        If ds.Tables(0).Rows.Count > 0 Then
            View = ds.Tables(0).DefaultView
            View.RowFilter = "DocumentType='" & DocumentType & "' and OUTWARDXMLTAGNAME <>''"
            If EnableEdit <> "" Then
                View.RowFilter = "DocumentType='" & DocumentType & "' AND EnableEdit=1 AND isEditable=1"
            End If
            'Getting all the field of base tabel() Note: in case of Action form base table will be name Action form
            tbl = View.Table.DefaultView.ToTable()
            Dim ViewName = "[V" & EID & DocumentType.Replace(" ", "_") & "]"
            Dim ddlDocType = ""
            For i As Integer = 0 To tbl.Rows.Count - 1
                'If Not (tbl.Rows(i).Item("DROPDOWNTYPE") = "MASTER VALUED" And tbl.Rows(i).Item("FieldType") = "Drop Down") Then
                '    StrColumn = StrColumn & "," & ViewName & ".[" & tbl.Rows(i).Item("DisplayName") & "] AS [" & tbl.Rows(i).Item("OUTWARDXMLTAGNAME") & "]"
                'End If

                If Not ((tbl.Rows(i).Item("DROPDOWNTYPE") = "MASTER VALUED" And (tbl.Rows(i).Item("FieldType") = "Drop Down" Or tbl.Rows(i).Item("FieldType") = "AutoComplete"))) Then
                    StrColumn = StrColumn & "," & ViewName & ".[" & tbl.Rows(i).Item("DisplayName") & "] AS [" & tbl.Rows(i).Item("OUTWARDXMLTAGNAME") & "]"
                End If

            Next
            'View.RowFilter = "DocumentType='" & DocumentType & "' AND  OUTWARDXMLTAGNAME <>'' AND  FieldType ='DROP DOWN' AND (DropDownType='MASTER VALUED' OR DropDownType='SESSION VALUED')"
            View.RowFilter = "DocumentType='" & DocumentType & "' AND (FieldType ='DROP DOWN' OR FieldType='AutoComplete') AND (DropDownType='MASTER VALUED' OR DropDownType='SESSION VALUED') and OUTWARDXMLTAGNAME <>''"
            tblRe = View.Table.DefaultView.ToTable()
            Dim FieldMapping As String = ""
            Dim Allias As String = ""
            Dim dRoW As DataRow()
            For j As Integer = 0 To tblRe.Rows.Count - 1
                Dim arrddl = tblRe.Rows(j).Item("Dropdown").ToString().Split("-")
                ddlDocType = arrddl(1)
                SchemaString = ddlDocType
                FieldMapping = arrddl(2)
                Dim ddlview = "[V" & EID & ddlDocType.Trim.Replace(" ", "_") & "]"
                Dim joincolumn = "tid"
                Dim DisPlayName = tblRe.Rows(j).Item("DisplayName").ToString().Trim
                Dim columnName = ""

                dRoW = ds.Tables(0).Select("DocumentType='" & ddlDocType & "' AND Fieldmapping='" & FieldMapping & "'")
                If dRoW.Length > 0 Then
                    columnName = dRoW(0).Item("DisplayName")
                    If ddlDocType.Trim.ToUpper = "USER" Then
                        joincolumn = "UID"
                        ddlview = "[MMM_MST_USER]"
                        Allias = "U" & j
                        columnName = arrddl(2)
                    Else
                        Allias = "V" & EID & ddlDocType.Trim.Replace(" ", "_") & j
                    End If
                    'Condition for Stoping Repeated join String in the case when same ddocument type repeates

                    If Not StrJoinString.Contains("[" & ddlview & "]") Then
                        StrJoinString = StrJoinString & "left outer join " & ddlview & " As " & Allias & "  on convert(nvarchar, " & Allias & "." & joincolumn & ") = " & ViewName & ".[" & DisPlayName & "]"
                    End If
                    'Dim Filter = "DocumentType='" & ddlDocType & "' AND Fieldmapping='" & FieldMapping & "'"

                    StrColumn = StrColumn & "," & Allias & ".[" & columnName & "] AS [" & tblRe.Rows(j).Item("OUTWARDXMLTAGNAME") & "]"
                    'GenearateQuery2(EID, StrColumn, StrJoinString, SchemaString, ds)
                End If
            Next
            View.RowFilter = "DocumentType='" & DocumentType & "' AND FieldType ='LookupDDL'"
            tblRe = View.Table.DefaultView.ToTable()
            Dim ArrDDlDocType As String()
            Dim ArrDDlDocfld As String()
            Dim DDlDocfld As String = ""
            Dim DDlDocfld1 As String = ""
            Dim ddllSource As String = ""
            For k As Integer = 0 To tblRe.Rows.Count - 1
                dRoW = ds.Tables(0).Select("DocumentType='" & DocumentType & "' AND fieldID='" & tblRe.Rows(0).Item("DropDown") & "'")
                If dRoW.Count > 0 Then
                    ArrDDlDocType = dRoW(0).Item("Dropdown").Split("-")
                    ArrDDlDocfld = dRoW(0).Item("DDllookupvalue").Split(",")
                    For m As Integer = 0 To ArrDDlDocfld.Length - 1
                        If ArrDDlDocfld(m).Trim <> "" Then
                            Dim d = ArrDDlDocfld(m).Split("-")
                            If d(0).Trim = tblRe.Rows(k).Item("FieldID").ToString.Trim Then
                                DDlDocfld = d(1)
                                Exit For
                            End If
                        End If
                    Next
                    If DDlDocfld.Trim <> "" Then
                        Dim DisplayName As String = ""
                        dRoW = ds.Tables(0).Select("DocumentType='" & ArrDDlDocType(1) & "' AND FieldMapping='" & DDlDocfld & "'")
                        ArrDDlDocType = dRoW(0).Item("DropDown").Split("-")
                        ddllSource = ArrDDlDocType(1).Trim
                        If ddllSource.Trim.ToUpper <> "USER" Then
                            dRoW = ds.Tables(0).Select("DocumentType='" & ddllSource & "' AND FieldMapping='" & ArrDDlDocType(2) & "'")
                            DisplayName = "[" & dRoW(0).Item("DisplayName") & "]"
                            Dim ddlQ = " (SELECT " & DisplayName & "FROM " & "[V" & EID & ddllSource & "] where tid= " & ViewName & ".[" & tblRe.Rows(k).Item("DisplayName") & "]) AS '" & tblRe.Rows(k).Item("OUTWARDXMLTAGNAME") & "'"
                            StrColumn = StrColumn & " ," & ddlQ
                        Else
                            Dim ddlQ = " (SELECT " & ArrDDlDocType(2) & "FROM " & "[MMM_MST_USER] where UID= " & ViewName & ".[" & tblRe.Rows(k).Item("DisplayName") & "]) AS '" & tblRe.Rows(k).Item("OUTWARDXMLTAGNAME") & "'"
                            StrColumn = StrColumn & " ," & ddlQ
                        End If

                    End If
                    'ddlDocType = ArrDDlDocType(1)
                    'dRoW = ds.Tables(0).Select("DocumentType='" & ddlDocType & "' AND fieldID='" & tblRe.Rows(0).Item("DropDown") & "'")
                End If
            Next
            Dim Query = StrColumn.Substring(1, StrColumn.Length - 1) & " FROM " & ViewName & " " & StrJoinString
            ret = Query & " WHERE 1=1 "
            'If tid <> 0 Then
            '    Query = Query & " WHERE " & ViewName & ".tid  = " & tid
            'End If
        End If
        Return ret
    End Function

    Public Function ReturnSyncInputParaValues(ByVal str As String) As String
        Dim xmlDocRead As New XmlDocument()
        xmlDocRead.LoadXml(str)
        Dim ReturnVal As String = "NA"
        Dim filtervalue As String = String.Empty
        Dim DocumentType As String = String.Empty
        Dim xmlEcode As String = String.Empty
        Dim xmlBPMTID As Integer = 0

        If xmlDocRead.ChildNodes.Count >= 1 Then
            Dim SelNodesTxt As String = xmlDocRead.DocumentElement.Name
            Dim nodes As XmlNodeList = xmlDocRead.SelectNodes(SelNodesTxt)

            For Each node As XmlNode In nodes
                For c As Integer = 0 To node.ChildNodes.Count - 1
                    If UCase(node.ChildNodes.Item(c).Name) = "BODY" Then
                        For C1 As Integer = 0 To node.Item("BODY").ChildNodes.Count - 1
                            If node.Item("BODY").ChildNodes.Item(C1).Name = "DATA" Then
                                For c2 As Integer = 0 To node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Count - 1
                                    If node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).Name = "VALIDATION" Then
                                        For P As Integer = 0 To node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Count - 1
                                            If UCase(node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).Name) = "ENTITY" Then
                                                xmlEcode = node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).InnerText.ToString()
                                            ElseIf UCase(node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).Name) = "BPMTID" Then
                                                xmlBPMTID = Convert.ToInt32(node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).InnerText)
                                            End If
                                        Next
                                        ReturnVal = xmlEcode & "|" & xmlBPMTID  ' documenttype|EID|rowfiltervlaue|BPMTID by which we add it to where condition to filter rows according to it
                                    End If
                                Next
                            End If
                        Next
                    End If
                Next
            Next
        End If
        'ErrorLog.sendMail("BPMTallyWS.Outward", ReturnVal)
        Return ReturnVal
    End Function

    Public Function ReturnInputParaValues(ByVal str As String) As String
        Dim xmlDocRead As New XmlDocument()
        xmlDocRead.LoadXml(str)
        Dim ReturnVal As String = "NA"
        Dim filtervalue As String = String.Empty
        Dim DocumentType As String = String.Empty
        Dim xmlEcode As String = String.Empty
        Dim xmlBPMTID As Integer = 0

        If xmlDocRead.ChildNodes.Count >= 1 Then
            Dim SelNodesTxt As String = xmlDocRead.DocumentElement.Name
            Dim nodes As XmlNodeList = xmlDocRead.SelectNodes(SelNodesTxt)

            For Each node As XmlNode In nodes
                For c As Integer = 0 To node.ChildNodes.Count - 1
                    If UCase(node.ChildNodes.Item(c).Name) = "BODY" Then
                        For C1 As Integer = 0 To node.Item("BODY").ChildNodes.Count - 1
                            If node.Item("BODY").ChildNodes.Item(C1).Name = "DATA" Then
                                For c2 As Integer = 0 To node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Count - 1
                                    If node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).Name = "VALIDATION" Then
                                        For P As Integer = 0 To node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Count - 1
                                            If UCase(node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).Name) = "FORMNAME" Then
                                                DocumentType = node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).InnerText.ToString()
                                            ElseIf UCase(node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).Name) = "ENTITY" Then
                                                xmlEcode = node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).InnerText.ToString()
                                            ElseIf UCase(node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).Name) = "DISTRIBUTORCODE" Then
                                                filtervalue = node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).InnerText.ToString()
                                            ElseIf UCase(node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).Name) = "BPMTID" Then
                                                xmlBPMTID = Convert.ToInt32(node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).InnerText)
                                            End If
                                        Next
                                        ReturnVal = DocumentType & "|" & xmlEcode & "|" & filtervalue & "|" & xmlBPMTID  ' documenttype|EID|rowfiltervlaue|BPMTID by which we add it to where condition to filter rows according to it

                                    End If
                                Next
                            End If
                        Next
                    End If
                Next
            Next
        End If
        'ErrorLog.sendMail("BPMTallyWS.Outward", ReturnVal)
        Return ReturnVal
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

    'prev
    'Public Function Prev_ReadxmlandGiveString(ByRef str As String) As String
    '    Dim result As String = String.Empty
    '    Dim xmlDocRead As New XmlDocument()
    '    xmlDocRead.LoadXml(str)

    '    Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
    '    Dim con As New SqlConnection(conStr)
    '    Dim da As New SqlDataAdapter("", con)
    '    Dim ds As New DataSet



    '    If xmlDocRead.ChildNodes.Count >= 1 Then

    '        Dim SelNodesTxt As String = xmlDocRead.DocumentElement.Name
    '        Dim Cnt As Integer = 0
    '        Dim nodes As XmlNodeList = xmlDocRead.SelectNodes(SelNodesTxt)
    '        For Each node As XmlNode In nodes
    '            Cnt += 1
    '            Dim MainVar As String = String.Empty

    '            Dim globalVar As String = String.Empty
    '            Dim ChildVar As String = String.Empty
    '            Dim xmldoctype As String = String.Empty
    '            Dim xmlecode As String = String.Empty
    '            Dim apikey As String = String.Empty

    '            For c As Integer = 0 To node.ChildNodes.Count - 1
    '                If UCase(node.ChildNodes.Item(c).Name) = "BODY" Then
    '                    For C1 As Integer = 0 To node.Item("BODY").ChildNodes.Count - 1
    '                        If node.Item("BODY").ChildNodes.Item(C1).Name = "DATA" Then
    '                            For c2 As Integer = 0 To node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Count - 1
    '                                If node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).Name = "VALIDATION" Then
    '                                    For P As Integer = 0 To node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Count - 1
    '                                        'globalVar = globalVar & node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).Name & "::" & node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).InnerText & "|"
    '                                        If UCase(node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).Name) = "FORMNAME" Then
    '                                            ' globalVar = "FORM NAME" & "::" & node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).InnerText & "|"
    '                                            xmldoctype = node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).InnerText.ToString()
    '                                        ElseIf UCase(node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).Name) = "ENTITYCODE" Then
    '                                            ' globalVar = globalVar & "ENTITY CODE" & "::" & node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).InnerText & "|"
    '                                            xmlecode = node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).InnerText.ToString()
    '                                        End If
    '                                    Next

    '                                    If Not xmldoctype Is Nothing And Not xmlecode Is Nothing Then
    '                                        da.SelectCommand.CommandText = "Select * from mmm_mst_fields where eid=" & xmlecode & " and documenttype='" & xmldoctype.ToString() & "' order by displayname "
    '                                        da.SelectCommand.CommandType = CommandType.Text
    '                                        da.Fill(ds, "data")
    '                                    End If
    '                                    'code to get API key of the entity
    '                                    If Not xmlecode Is Nothing Then
    '                                        da.SelectCommand.CommandText = "Select apikey from mmm_mst_entity where eid=" & xmlecode & ""
    '                                        da.Fill(ds, "apikey")
    '                                        If ds.Tables("apikey").Rows.Count > 0 Then
    '                                            apikey = ds.Tables("apikey").Rows(0).Item("apikey").ToString()
    '                                        End If
    '                                    End If

    '                                End If



    '                                If node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).Name = "REQUESTDATA" Then
    '                                    For P As Integer = 0 To node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Count - 1
    '                                        If UCase(node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).Name) = "TALLYMESSAGE" Then
    '                                            For d As Integer = 0 To node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).ChildNodes.Count - 1
    '                                                For s As Integer = 0 To ds.Tables("data").Rows.Count - 1
    '                                                    If UCase(ds.Tables("data").Rows(s).Item("inwardxmltagname").ToString()) = UCase(node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).ChildNodes.Item(d).Name) Then

    '                                                        If UCase(ds.Tables("data").Rows(s).Item("fieldtype").ToString()) = "CHILD ITEM" Then
    '                                                            da.SelectCommand.CommandType = CommandType.Text
    '                                                            da.SelectCommand.CommandText = "Select * from mmm_mst_fields where eid=43 and documenttype='" & ds.Tables("data").Rows(s).Item("DROPDOWN").ToString().Trim() & "'"
    '                                                            da.Fill(ds, "child")
    '                                                            ChildVar = ds.Tables("data").Rows(s).Item("DROPDOWN").ToString().Trim() & "::{}"
    '                                                            For x As Integer = 0 To ds.Tables("child").Rows.Count - 1
    '                                                                For l As Integer = 0 To node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).ChildNodes.Item(d).ChildNodes.Count - 1
    '                                                                    If UCase(node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).ChildNodes.Item(d).ChildNodes.Item(l).Name).ToString() = UCase(ds.Tables("child").Rows(x).Item("inwardxmltagname").ToString()) Then
    '                                                                        ChildVar = ChildVar & "()" & UCase(ds.Tables("child").Rows(x).Item("displayname").ToString()) & "<>" & node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).ChildNodes.Item(d).ChildNodes.Item(l).InnerText
    '                                                                    End If
    '                                                                Next

    '                                                            Next
    '                                                            ChildVar = ChildVar & "{}"
    '                                                        Else
    '                                                            globalVar = globalVar & UCase(ds.Tables("data").Rows(s).Item("displayname").ToString()) & "::" & node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).ChildNodes.Item(d).InnerText & "|"
    '                                                        End If

    '                                                    End If
    '                                                Next
    '                                            Next
    '                                        End If
    '                                    Next
    '                                End If
    '                            Next
    '                        End If
    '                    Next
    '                End If
    '            Next
    '            If ChildVar.Length > 2 Then
    '                ChildVar = ChildVar.Remove(ChildVar.Length - 2)
    '            End If
    '            'ChildVar = ChildVar.Remove(ChildVar.Length - 2)
    '            result = "Key$$" & apikey & "~DOCTYPE$$" & xmldoctype.ToString() & "~Data$$" & globalVar & ChildVar
    '        Next
    '    End If

    '    Return result
    'End Function

    Public Function readxmlandgivestring(ByRef str As String) As String
        Dim result As String = String.Empty
        Dim msg As String = String.Empty
        Dim xmlDocRead As New XmlDocument()
        xmlDocRead.LoadXml(str)
        Dim TransCountforLog As Integer = 0

        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim da As New SqlDataAdapter("", con)
        Dim ds As New DataSet

        Dim Data1 As New StringBuilder(str)
        Dim xmleid As Integer = 0


        Dim MainVar As String = String.Empty
        Dim cntt As Integer = 0
        Dim globalVar As String = String.Empty
        Dim ChildVar As String = String.Empty
        Dim xmldoctype As String = String.Empty
        Dim xmlecode As String = String.Empty
        Dim apikey As String = String.Empty
        Dim xmlBPMTID As Integer = 0

        If xmlDocRead.ChildNodes.Count >= 1 Then

            Dim SelNodesTxt As String = xmlDocRead.DocumentElement.Name
            Dim Cnt As Integer = 0
            Dim nodes As XmlNodeList = xmlDocRead.SelectNodes(SelNodesTxt)
            For Each node As XmlNode In nodes
                Cnt += 1
                MainVar = String.Empty
                cntt = 0
                globalVar = String.Empty
                ChildVar = String.Empty
                xmldoctype = String.Empty
                xmlecode = String.Empty
                apikey = String.Empty
                xmlBPMTID = 0
                Dim Key As String = "", EID As String = "", DocType As String = "", UID As String = 0
                For c As Integer = 0 To node.ChildNodes.Count - 1
                    If UCase(node.ChildNodes.Item(c).Name) = "BODY" Then
                        For C1 As Integer = 0 To node.Item("BODY").ChildNodes.Count - 1
                            If node.Item("BODY").ChildNodes.Item(C1).Name = "DATA" Then
                                For c2 As Integer = 0 To node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Count - 1
                                    If node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).Name = "VALIDATION" Then
                                        For P As Integer = 0 To node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Count - 1
                                            If UCase(node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).Name) = "FORMNAME" Then
                                                xmldoctype = node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).InnerText.ToString()
                                            ElseIf UCase(node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).Name) = "ENTITY" Then
                                                xmlecode = node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).InnerText.ToString()
                                            ElseIf UCase(node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).Name) = "BPMTID" Then
                                                xmlBPMTID = Val(node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).InnerText)
                                            End If
                                        Next

                                        If Not String.IsNullOrEmpty(xmlBPMTID) Then
                                            Dim cmd As New SqlCommand("select isnull(TallyIsActive,0)[Count] from mmm_mst_master where tid =" & xmlBPMTID & " ", con)

                                            If con.State <> ConnectionState.Open Then
                                                con.Open()
                                            End If
                                            cntt = Convert.ToInt32(cmd.ExecuteScalar())

                                            If cntt = 1 Then
                                                If Not String.IsNullOrEmpty(xmldoctype) And Not String.IsNullOrEmpty(xmlecode) Then
                                                    da.SelectCommand.CommandText = "Select eid from mmm_mst_entity where code='" & xmlecode.ToString() & "'"
                                                    da.SelectCommand.CommandType = CommandType.Text
                                                    If con.State <> ConnectionState.Open Then
                                                        con.Open()
                                                    End If
                                                    xmleid = Convert.ToInt32(da.SelectCommand.ExecuteScalar())

                                                    da.SelectCommand.CommandText = "Select * from mmm_mst_fields where eid=" & xmleid & " and documenttype='" & xmldoctype.ToString() & "' order by displayname "
                                                    da.SelectCommand.CommandType = CommandType.Text
                                                    da.Fill(ds, "data")
                                                End If
                                                'code to get API key of the entity
                                                If xmleid > 0 Then
                                                    da.SelectCommand.CommandText = "Select apikey from mmm_mst_entity where eid=" & xmleid & ""
                                                    da.Fill(ds, "apikey")
                                                    If ds.Tables("apikey").Rows.Count > 0 Then
                                                        apikey = ds.Tables("apikey").Rows(0).Item("apikey").ToString()
                                                    End If
                                                End If
                                            End If
                                        End If
                                    End If

                                    If cntt = 1 Then
                                        Dim rowno As Integer = 0
                                        If node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).Name = "REQUESTDATA" Then
                                            For P As Integer = 0 To node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Count - 1
                                                If UCase(node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).Name) = "TALLYMESSAGE" Then
                                                    For d As Integer = 0 To node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).ChildNodes.Count - 1
                                                        For s As Integer = 0 To ds.Tables("data").Rows.Count - 1
                                                            If UCase(ds.Tables("data").Rows(s).Item("inwardxmltagname").ToString()) = UCase(node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).ChildNodes.Item(d).Name) Then

                                                                If UCase(ds.Tables("data").Rows(s).Item("fieldtype").ToString()) = "CHILD ITEM" Then
                                                                    da.SelectCommand.CommandType = CommandType.Text
                                                                    da.SelectCommand.CommandText = "Select * from mmm_mst_fields where eid=" & xmleid & " and documenttype='" & ds.Tables("data").Rows(s).Item("DROPDOWN").ToString().Trim() & "'"
                                                                    da.Fill(ds, "child")
                                                                    If (ChildVar.Trim = "") Then
                                                                        ChildVar = ds.Tables("data").Rows(s).Item("DROPDOWN").ToString().Trim() & "::{}"
                                                                    End If
                                                                    For x As Integer = 0 To ds.Tables("child").Rows.Count - 1
                                                                        For l As Integer = 0 To node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).ChildNodes.Item(d).ChildNodes.Count - 1
                                                                            If UCase(node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).ChildNodes.Item(d).ChildNodes.Item(l).Name).ToString() = UCase(ds.Tables("child").Rows(x).Item("inwardxmltagname").ToString()) Then
                                                                                ChildVar = ChildVar & "()" & UCase(ds.Tables("child").Rows(x).Item("displayname").ToString()) & "<>" & node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).ChildNodes.Item(d).ChildNodes.Item(l).InnerText
                                                                            End If
                                                                        Next
                                                                    Next
                                                                    ChildVar = ChildVar & "{}"
                                                                Else
                                                                    globalVar = globalVar & UCase(ds.Tables("data").Rows(s).Item("displayname").ToString()) & "::" & node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).ChildNodes.Item(d).InnerText & "|"
                                                                End If

                                                            End If
                                                        Next

                                                    Next
                                                    If ChildVar.Length > 2 Then
                                                        ChildVar = ChildVar.Remove(ChildVar.Length - 2)
                                                    End If
                                                    Try
                                                        result = "Key$$" & apikey & "~DOCTYPE$$" & xmldoctype.ToString() & "~Data$$" & globalVar & ChildVar
                                                        Dim arrData As String() = Split(result, "~")
                                                        For i As Integer = 0 To arrData.Length - 1
                                                            Dim ar = Split(arrData(i), "$$")
                                                            If ar(0).ToUpper().Trim() = "KEY" Then
                                                                Key = ar(1)
                                                            ElseIf ar(0).ToUpper().Trim() = "DOCTYPE" Then
                                                                DocType = ar(1)
                                                            ElseIf ar(0).ToUpper().Trim() = "DATA" Then
                                                                result = ar(1)
                                                            End If
                                                        Next
                                                        Dim DsS As New DataSet()
                                                        DsS = AuthenticateWSRequest(Key)
                                                        If DsS.Tables(0).Rows.Count > 0 Then
                                                            EID = DsS.Tables(0).Rows(0).Item("EID")
                                                            UID = DsS.Tables(1).Rows(0).Item("uid")
                                                            result = CommanUtil.ValidateParameterByDocumentType(EID, DocType, UID, result)
                                                        Else
                                                            result = "Sorry!!! Authentication failed."
                                                        End If
                                                        CommanUtil.SaveServicerequest(Data1, "BPMTallyInt", "Inward", result)
                                                        msg = msg & result
                                                        result = ""
                                                        globalVar = ""
                                                        ChildVar = ""
                                                    Catch ex As Exception
                                                        ErrorLog.sendMail("BPMTallyInt.Inward", ex.Message)
                                                        result = ""
                                                        globalVar = ""
                                                        ChildVar = ""
                                                    End Try
                                                End If
                                                TransCountforLog = TransCountforLog + 1
                                            Next
                                        End If
                                    Else
                                        msg = "YOU ARE NOT AUTHORIZED "
                                    End If

                                Next
                            End If
                        Next
                    End If
                Next

            Next
        End If
        If msg.ToUpper().Contains("YOUR DOCID IS") Then
            Call InsertinTallyIntegrationLog(xmleid, xmlBPMTID, xmldoctype, "INWARD", xmldoctype, TransCountforLog, "SUCCESS", msg)
        Else
            Call InsertinTallyIntegrationLog(xmleid, xmlBPMTID, xmldoctype, "INWARD", xmldoctype, TransCountforLog, "FAIL", msg)
        End If

        Return msg
    End Function

    Public Function InsertinTallyIntegrationLog(ByVal EID As Integer, ByVal BPMRegID As Integer, ByVal BPMRegDtype As String, ByVal ServiceName As String, ByVal BpmDoctype As String, ByVal RecProcessed As Integer, ByVal Result As String, ByVal RtnMessage As String) As String
        Try
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As New SqlConnection(conStr)

            If con.State <> ConnectionState.Open Then
                con.Open()
            End If

            Dim da As New SqlDataAdapter("", con)
            Dim ds As New DataSet

            da.SelectCommand.CommandText = "select isnull(formname,'') from MMM_MST_TallyRegInfo where eid=" & EID & " and bpmtid=" & BPMRegID & ""
            ' Dim REgDtype As String = Convert.ToString(da.SelectCommand.ExecuteScalar())
            Dim dt As New DataTable
            da.Fill(dt)
            Dim REgDtype As String = ""
            If dt.Rows.Count > 0 Then
                REgDtype = dt.Rows(0).Item(0).ToString()
            End If

            da.SelectCommand.CommandType = CommandType.StoredProcedure
            da.SelectCommand.CommandText = "uspInsertTallyWSrequestLog"
            da.SelectCommand.Parameters.AddWithValue("@EID", EID)
            da.SelectCommand.Parameters.AddWithValue("@BPMREGID", BPMRegID)
            da.SelectCommand.Parameters.AddWithValue("@BPMREGDOCTYPE", REgDtype)
            da.SelectCommand.Parameters.AddWithValue("@servicename", ServiceName)
            da.SelectCommand.Parameters.AddWithValue("@bpmDocType", BpmDoctype)
            da.SelectCommand.Parameters.AddWithValue("@recordsProcessed", RecProcessed)
            da.SelectCommand.Parameters.AddWithValue("@result", Result)
            da.SelectCommand.Parameters.AddWithValue("@returnMessage", RtnMessage)
            Dim res As String = ""
            da.SelectCommand.ExecuteScalar()
            con.Dispose()
            da.Dispose()
            ds.Dispose()
            Return "Success"
        Catch ex As Exception
            Return "Error " & ex.InnerException.Message.ToString()
        End Try
    End Function


    Public Function InsertInward_Bulk(ByVal str As String) As String
        Dim result As String = String.Empty
        Dim msg As String = "<ENVELOPE> <HEADER><TALLYREQUEST>EXPORT VOUCHERS</TALLYREQUEST></HEADER> <BODY>  <DATA>   <VALIDATION>"
        Dim xmlDocRead As New XmlDocument()
        xmlDocRead.LoadXml(str)
        Dim TransCountforLog As Integer = 0

        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim da As New SqlDataAdapter("", con)
        Dim ds As New DataSet

        Dim Data1 As New StringBuilder(str)
        Dim xmleid As Integer = 0

        Dim MainVar As String = String.Empty
        Dim cntt As Integer = 0
        Dim globalVar As String = String.Empty
        Dim ChildVar As String = String.Empty
        Dim xmldoctype As String = String.Empty
        Dim xmlecode As String = String.Empty
        Dim apikey As String = String.Empty
        Dim ReturnTagName As String = String.Empty
        Dim ReturnTagValue As String = String.Empty
        Dim xmlBPMTID As Integer = 0

        If xmlDocRead.ChildNodes.Count >= 1 Then

            Dim SelNodesTxt As String = xmlDocRead.DocumentElement.Name
            Dim Cnt As Integer = 0
            Dim nodes As XmlNodeList = xmlDocRead.SelectNodes(SelNodesTxt)
            For Each node As XmlNode In nodes
                Cnt += 1
                MainVar = String.Empty
                cntt = 0
                globalVar = String.Empty
                ChildVar = String.Empty
                xmldoctype = String.Empty
                xmlecode = String.Empty
                apikey = String.Empty
                ReturnTagName = String.Empty
                ReturnTagValue = String.Empty
                xmlBPMTID = 0
                Dim Key As String = "", EID As String = "", DocType As String = "", UID As String = 0
                For c As Integer = 0 To node.ChildNodes.Count - 1
                    If UCase(node.ChildNodes.Item(c).Name) = "BODY" Then
                        For C1 As Integer = 0 To node.Item("BODY").ChildNodes.Count - 1
                            If node.Item("BODY").ChildNodes.Item(C1).Name = "DATA" Then
                                For c2 As Integer = 0 To node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Count - 1
                                    If node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).Name = "VALIDATION" Then
                                        For P As Integer = 0 To node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Count - 1
                                            If UCase(node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).Name) = "FORMNAME" Then
                                                xmldoctype = node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).InnerText.ToString()
                                            ElseIf UCase(node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).Name) = "ENTITY" Then
                                                xmlecode = node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).InnerText.ToString()
                                            ElseIf UCase(node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).Name) = "BPMTID" Then
                                                xmlBPMTID = Val(node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).InnerText)
                                            End If
                                        Next

                                        If Not String.IsNullOrEmpty(xmlBPMTID) Then
                                            Dim cmd As New SqlCommand("select isnull(TallyIsActive,0)[Count] from mmm_mst_master where tid =" & xmlBPMTID & " ", con)

                                            If con.State <> ConnectionState.Open Then
                                                con.Open()
                                            End If
                                            cntt = Convert.ToInt32(cmd.ExecuteScalar())

                                            If cntt = 1 Then
                                                If Not String.IsNullOrEmpty(xmldoctype) And Not String.IsNullOrEmpty(xmlecode) Then
                                                    da.SelectCommand.CommandText = "Select eid from mmm_mst_entity where code='" & xmlecode.ToString() & "'"
                                                    da.SelectCommand.CommandType = CommandType.Text
                                                    If con.State <> ConnectionState.Open Then
                                                        con.Open()
                                                    End If
                                                    xmleid = Convert.ToInt32(da.SelectCommand.ExecuteScalar())

                                                    da.SelectCommand.CommandText = "Select * from mmm_mst_fields where eid=" & xmleid & " and documenttype='" & xmldoctype.ToString() & "' order by displayname "
                                                    da.SelectCommand.CommandType = CommandType.Text
                                                    da.Fill(ds, "data")
                                                End If
                                                'code to get API key of the entity
                                                If xmleid > 0 Then
                                                    da.SelectCommand.CommandText = "Select apikey from mmm_mst_entity where eid=" & xmleid & ""
                                                    da.Fill(ds, "apikey")
                                                    If ds.Tables("apikey").Rows.Count > 0 Then
                                                        apikey = ds.Tables("apikey").Rows(0).Item("apikey").ToString()
                                                    End If

                                                    da.SelectCommand.CommandText = "Select Tally_BulkInward_ReturnField from mmm_mst_forms where eid=" & xmleid & " and FormName='" & xmldoctype.ToString() & "'"
                                                    da.SelectCommand.CommandType = CommandType.Text
                                                    ReturnTagName = Convert.ToString(da.SelectCommand.ExecuteScalar())
                                                End If
                                            Else
                                                msg = msg & "<RESULT>BPMTID IN INVALID in BPM</RESULT> <STATUS>INVALID BPMTID</STATUS> </VALIDATION></DATA> </BODY></ENVELOPE>"
                                                Call InsertinTallyIntegrationLog(xmleid, xmlBPMTID, xmldoctype, "INWARDBULK", xmldoctype, TransCountforLog, "FAIL", msg)
                                                Return msg
                                                Exit Function
                                            End If
                                        End If
                                    End If

                                    If ReturnTagName = "" Then
                                        msg = msg & "<RESULT>Return Tag Name not configuered in BPM</RESULT> <STATUS>Configuration Required</STATUS> </VALIDATION></DATA> </BODY></ENVELOPE>"
                                        Call InsertinTallyIntegrationLog(xmleid, xmlBPMTID, xmldoctype, "INWARDBULK", xmldoctype, TransCountforLog, "FAIL", msg)
                                        Return msg
                                        Exit Function
                                    End If

                                    If cntt = 1 Then
                                        Dim rowno As Integer = 0
                                        If node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).Name = "REQUESTDATA" Then
                                            For P As Integer = 0 To node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Count - 1
                                                If UCase(node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).Name) = "TALLYMESSAGE" Then
                                                    For d As Integer = 0 To node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).ChildNodes.Count - 1
                                                        For s As Integer = 0 To ds.Tables("data").Rows.Count - 1
                                                            If UCase(ds.Tables("data").Rows(s).Item("inwardxmltagname").ToString()) = UCase(node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).ChildNodes.Item(d).Name) Then
                                                                ' new by sp for return tag from record with value - 04_apr_15
                                                                If UCase(node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).ChildNodes.Item(d).Name) = ReturnTagName Then
                                                                    ReturnTagValue = node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).ChildNodes.Item(d).InnerText
                                                                End If
                                                                '' by sp
                                                                If UCase(ds.Tables("data").Rows(s).Item("fieldtype").ToString()) = "CHILD ITEM" Then
                                                                    da.SelectCommand.CommandType = CommandType.Text
                                                                    da.SelectCommand.CommandText = "Select * from mmm_mst_fields where eid=" & xmleid & " and documenttype='" & ds.Tables("data").Rows(s).Item("DROPDOWN").ToString().Trim() & "'"
                                                                    da.Fill(ds, "child")
                                                                    If (ChildVar.Trim = "") Then
                                                                        ChildVar = ds.Tables("data").Rows(s).Item("DROPDOWN").ToString().Trim() & "::{}"
                                                                    End If
                                                                    For x As Integer = 0 To ds.Tables("child").Rows.Count - 1
                                                                        For l As Integer = 0 To node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).ChildNodes.Item(d).ChildNodes.Count - 1
                                                                            If UCase(node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).ChildNodes.Item(d).ChildNodes.Item(l).Name).ToString() = UCase(ds.Tables("child").Rows(x).Item("inwardxmltagname").ToString()) Then
                                                                                ChildVar = ChildVar & "()" & UCase(ds.Tables("child").Rows(x).Item("displayname").ToString()) & "<>" & node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).ChildNodes.Item(d).ChildNodes.Item(l).InnerText
                                                                            End If
                                                                        Next
                                                                    Next
                                                                    ChildVar = ChildVar & "{}"
                                                                Else
                                                                    globalVar = globalVar & UCase(ds.Tables("data").Rows(s).Item("displayname").ToString()) & "::" & node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).ChildNodes.Item(d).InnerText & "|"
                                                                End If

                                                            End If
                                                        Next

                                                    Next
                                                    If ChildVar.Length > 2 Then
                                                        ChildVar = ChildVar.Remove(ChildVar.Length - 2)
                                                    End If
                                                    Try
                                                        result = "Key$$" & apikey & "~DOCTYPE$$" & xmldoctype.ToString() & "~Data$$" & globalVar & ChildVar
                                                        Dim arrData As String() = Split(result, "~")
                                                        For i As Integer = 0 To arrData.Length - 1
                                                            Dim ar = Split(arrData(i), "$$")
                                                            If ar(0).ToUpper().Trim() = "KEY" Then
                                                                Key = ar(1)
                                                            ElseIf ar(0).ToUpper().Trim() = "DOCTYPE" Then
                                                                DocType = ar(1)
                                                            ElseIf ar(0).ToUpper().Trim() = "DATA" Then
                                                                result = ar(1)
                                                            End If
                                                        Next
                                                        Dim DsS As New DataSet()
                                                        DsS = AuthenticateWSRequest(Key)
                                                        If DsS.Tables(0).Rows.Count > 0 Then
                                                            EID = DsS.Tables(0).Rows(0).Item("EID")
                                                            UID = DsS.Tables(1).Rows(0).Item("uid")
                                                            result = CommanUtil.ValidateParameterByDocumentType(EID, DocType, UID, result)
                                                        Else
                                                            result = "Sorry!!! Authentication failed."
                                                        End If
                                                        CommanUtil.SaveServicerequest(Data1, "BPMTallyWS", "Inward", result)
                                                        'msg = msg & result
                                                        '' add here multiple records
                                                        If result.ToUpper().Contains("YOUR DOCID IS") = True Then
                                                            msg = msg & "<RESULT> " & Trim(Replace(result.ToUpper(), "YOUR DOCID IS", "")) & " </RESULT> <STATUS>SUCCESS</STATUS> " & "<" & ReturnTagName & ">" & ReturnTagValue & "</" & ReturnTagName & ">  "
                                                        Else
                                                            msg = msg & "<RESULT>" & result & "</RESULT> <STATUS>ERROR</STATUS> " & "<" & ReturnTagName & ">" & ReturnTagValue & "</" & ReturnTagName & ">  "
                                                        End If
                                                        '' add here for bulk ends
                                                        result = ""
                                                        globalVar = ""
                                                        ChildVar = ""
                                                    Catch ex As Exception
                                                        ErrorLog.sendMail("BPMTallyInt.InwardBulk", ex.Message)
                                                        result = ""
                                                        globalVar = ""
                                                        ChildVar = ""
                                                        msg = msg & "<RESULT>ERROR</RESULT> <STATUS>ERROR</STATUS>" & "<" & ReturnTagName & ">" & ReturnTagValue & "</" & ReturnTagName & ">  "
                                                    End Try
                                                    TransCountforLog = TransCountforLog + 1
                                                End If
                                            Next
                                            msg = msg & "</VALIDATION></DATA> </BODY></ENVELOPE>"
                                        End If
                                    Else
                                        msg = msg & "<RESULT>YOU ARE NOT AUTHORIZED</RESULT> <STATUS>YOU ARE NOT AUTHORIZED</STATUS> </VALIDATION></DATA> </BODY></ENVELOPE>"
                                    End If
                                Next
                            End If
                        Next
                    End If
                Next
            Next
        End If

        If msg.ToUpper().Contains("YOUR DOCID IS") Then
            Call InsertinTallyIntegrationLog(xmleid, xmlBPMTID, xmldoctype, "INWARDBULK", xmldoctype, TransCountforLog, "SUCCESS", msg)
        Else
            Call InsertinTallyIntegrationLog(xmleid, xmlBPMTID, xmldoctype, "INWARDBULK", xmldoctype, TransCountforLog, "FAIL", msg)
        End If
        Return msg
    End Function

    Public Function readxmlandgivestringREGISTRATION(ByRef str As String) As String
        Dim result As String = String.Empty
        Dim msg As String = String.Empty
        Dim xmlDocRead As New XmlDocument()
        xmlDocRead.LoadXml(str)
        Dim xmldoctype As String = String.Empty
        Dim xmlecode As String = String.Empty
        Dim xmlfldname As String = String.Empty
        Dim xmlRegvalue As String = String.Empty
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim da As New SqlDataAdapter("", con)
        Dim ds As New DataSet
        Dim isActive As Integer = 0
        Dim eid As Integer = 0

        If xmlDocRead.ChildNodes.Count >= 1 Then

            Dim SelNodesTxt As String = xmlDocRead.DocumentElement.Name
            Dim Cnt As Integer = 0
            Dim nodes As XmlNodeList = xmlDocRead.SelectNodes(SelNodesTxt)
            For Each node As XmlNode In nodes
                Cnt += 1
                Dim globalVar As String = String.Empty
                Dim ChildVar As String = String.Empty

                Dim apikey As String = String.Empty
                Dim Key As String = "", DocType As String = "", UID As String = 0
                For c As Integer = 0 To node.ChildNodes.Count - 1
                    If UCase(node.ChildNodes.Item(c).Name) = "BODY" Then
                        For C1 As Integer = 0 To node.Item("BODY").ChildNodes.Count - 1
                            If node.Item("BODY").ChildNodes.Item(C1).Name = "DATA" Then
                                For c2 As Integer = 0 To node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Count - 1
                                    If node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).Name = "VALIDATION" Then
                                        For P As Integer = 0 To node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Count - 1
                                            If UCase(node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).Name) = "FORMNAME" Then
                                                xmldoctype = node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).InnerText.ToString()
                                            ElseIf UCase(node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).Name) = "REGVALUE" Then
                                                xmlRegvalue = node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).InnerText.ToString()
                                            ElseIf UCase(node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).Name) = "ENTITY" Then
                                                xmlecode = node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).InnerText.ToString()
                                            End If
                                        Next
                                    End If
                                Next
                            End If
                        Next
                    End If
                Next

            Next

            If Not String.IsNullOrEmpty(xmldoctype) And Not String.IsNullOrEmpty(xmlecode) And Not String.IsNullOrEmpty(xmlRegvalue) Then
                'getting EID 
                da.SelectCommand.CommandText = "Select eid from mmm_mst_entity where code='" & xmlecode.ToString() & "'"
                If con.State <> ConnectionState.Open Then
                    con.Open()
                End If
                eid = Val(da.SelectCommand.ExecuteScalar())

                'getting Tally Registration FIeld from FORM Master
                da.SelectCommand.CommandText = "select TallyRegField from mmm_mst_forms where eid=" & Val(eid) & " and formname ='" & xmldoctype.ToString() & "'"
                xmlfldname = da.SelectCommand.ExecuteScalar().ToString()

                Dim StrQuery As String = "SELECT tid from  v" & eid & xmldoctype.Trim.Replace(" ", "_") & " where  v" & eid & xmldoctype.Trim.Replace(" ", "_") & ".[" & xmlfldname.ToString() & "] = '" & xmlRegvalue.ToString() & "'"
                da.SelectCommand.CommandText = StrQuery

                result = da.SelectCommand.ExecuteScalar().ToString()

                StrQuery = "SELECT case TallyIsActive when 1 then 1 else 0 end  from  v" & eid & xmldoctype.Trim.Replace(" ", "_") & " where  v" & eid & xmldoctype.Trim.Replace(" ", "_") & ".[" & xmlfldname.ToString() & "] = '" & xmlRegvalue.ToString() & "'"
                da.SelectCommand.CommandText = StrQuery

                isActive = Convert.ToInt32(da.SelectCommand.ExecuteScalar().ToString())
            End If
        End If
        Dim sb As New StringBuilder
        sb.Append("<ENVELOPE>")
        sb.Append("<HEADER>")
        sb.Append("<TALLYREQUEST>")
        sb.Append("REGISTRATION")
        sb.Append("</TALLYREQUEST>")
        sb.Append("</HEADER>")
        sb.Append("<BODY>")

        sb.Append("<DATA>")
        sb.Append("<VALIDATION>")
        sb.Append("<FORMNAME>")
        sb.Append(xmldoctype)
        sb.Append("</FORMNAME>")
        sb.Append("<ENTITY>")
        sb.Append(xmlecode)
        sb.Append("</ENTITY>")
        sb.Append("<REGVALUE>")
        sb.Append(xmlRegvalue)
        sb.Append("</REGVALUE>")
        sb.Append("<BPMTID>")
        If Trim(isActive) = 1 Then
            sb.Append("ALREADY REGISTERED")
        Else
            If Not String.IsNullOrEmpty(result) And result <> "0" Then
                sb.Append(result)
            Else
                sb.Append("NOT FOUND")
            End If
        End If

        sb.Append("</BPMTID>")
        sb.Append("</VALIDATION>")
        sb.Append("</DATA>")
        sb.Append("</BODY>")
        sb.Append("</ENVELOPE>")

        'If Not String.IsNullOrEmpty(result) And result <> "0" Then
        '    Call InsertinTallyIntegrationLog(eid, xmlBPMTID, xmldoctype, "INWARDBULK", xmldoctype, 1, "SUCCESS", msg)
        'Else
        '    Call InsertinTallyIntegrationLog(eid, xmlBPMTID, xmldoctype, "INWARDBULK", xmldoctype, 0, "FAIL", msg)
        'End If

        Return sb.ToString()

    End Function


    Public Function readxmlandgivestringConfirmation(ByRef str As String) As String
        Dim result As String = String.Empty
        Dim msg As String = String.Empty
        Dim xmlDocRead As New XmlDocument()
        xmlDocRead.LoadXml(str)
        Dim xmldoctype As String = String.Empty
        Dim xmlecode As String = String.Empty
        Dim xmlBPMTid As Integer = 0
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim da As New SqlDataAdapter("", con)
        Dim ds As New DataSet
        Dim EID As String = ""
        If xmlDocRead.ChildNodes.Count >= 1 Then
            Dim SelNodesTxt As String = xmlDocRead.DocumentElement.Name
            Dim Cnt As Integer = 0
            Dim nodes As XmlNodeList = xmlDocRead.SelectNodes(SelNodesTxt)
            For Each node As XmlNode In nodes
                Cnt += 1
                Dim globalVar As String = String.Empty
                Dim ChildVar As String = String.Empty

                Dim apikey As String = String.Empty
                Dim Key As String = "", DocType As String = "", UID As String = 0
                For c As Integer = 0 To node.ChildNodes.Count - 1
                    If UCase(node.ChildNodes.Item(c).Name) = "BODY" Then
                        For C1 As Integer = 0 To node.Item("BODY").ChildNodes.Count - 1
                            If node.Item("BODY").ChildNodes.Item(C1).Name = "DATA" Then
                                For c2 As Integer = 0 To node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Count - 1
                                    If node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).Name = "VALIDATION" Then
                                        For P As Integer = 0 To node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Count - 1
                                            If UCase(node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).Name) = "FORMNAME" Then
                                                xmldoctype = node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).InnerText.ToString()
                                            ElseIf UCase(node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).Name) = "BPMTID" Then
                                                xmlBPMTid = Val(node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).InnerText)
                                            ElseIf UCase(node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).Name) = "ENTITY" Then
                                                xmlecode = node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).InnerText.ToString()
                                            End If
                                        Next

                                    End If
                                Next
                            End If
                        Next
                    End If
                Next

            Next


            If Not String.IsNullOrEmpty(xmlBPMTid) Then

                If con.State <> ConnectionState.Open Then
                    con.Open()
                End If

                da.SelectCommand.CommandText = "Select isnull(EID,0) from mmm_mst_entity where code='" & xmlecode & "'"
                da.SelectCommand.CommandType = CommandType.Text
                EID = Convert.ToInt32(da.SelectCommand.ExecuteScalar())

                Dim cntt As Integer = 0
                da.SelectCommand.CommandText = "Select count(tid) from mmm_mst_master where tid=" & Val(xmlBPMTid) & " and eid=" & EID
                If con.State <> ConnectionState.Open Then
                    con.Open()
                End If
                cntt = Val(da.SelectCommand.ExecuteScalar())
                If Cnt = 1 Then
                    Dim StrQuery As String = ""
                    StrQuery = "select count(TallyIsActive) from mmm_mst_master  where tid=" & Val(xmlBPMTid) & " and eid=" & EID & " and isnull(TallyIsActive,0)=1"
                    da.SelectCommand.CommandText = StrQuery
                    cntt = Val(da.SelectCommand.ExecuteScalar())

                    If cntt = 0 Then
                        StrQuery = "Update mmm_mst_master  set TallyIsActive= 1 where tid=" & Val(xmlBPMTid) & " "
                        da.SelectCommand.CommandText = StrQuery
                        da.SelectCommand.ExecuteNonQuery()
                        result = "SUCCESS"
                    Else
                        result = "ALREADY CONFIRMED"
                    End If
                Else
                    result = "FAIL"
                End If
            End If

        End If

        Dim sb As New StringBuilder
        sb.Append("<ENVELOPE>")
        sb.Append("<HEADER>")
        sb.Append("<TALLYREQUEST>")
        sb.Append("CONFIRMATION")
        sb.Append("</TALLYREQUEST>")
        sb.Append("</HEADER>")
        sb.Append("<BODY>")

        sb.Append("<DATA>")
        sb.Append("<VALIDATION>")
        sb.Append("<FORMNAME>")
        sb.Append(xmldoctype)
        sb.Append("</FORMNAME>")
        sb.Append("<ENTITY>")
        sb.Append(xmlecode)
        sb.Append("</ENTITY>")
        sb.Append("<BPMTID>")
        sb.Append(xmlBPMTid)
        sb.Append("</BPMTID>")
        sb.Append("<RESULT>")
        sb.Append(result)
        sb.Append("</RESULT>")
        sb.Append("</VALIDATION>")
        sb.Append("</DATA>")
        sb.Append("</BODY>")
        sb.Append("</ENVELOPE>")

        Dim StrInsert As String
        If Trim(UCase(result)) = "SUCCESS" Then
            StrInsert = "insert into MMM_MST_TallyRegInfo ( BPMTID, EID,FORMNAME, isActive) values (" & xmlBPMTid & "," & EID & ",'" & xmldoctype & "',1)"
            da.SelectCommand.CommandText = StrInsert
            da.SelectCommand.ExecuteNonQuery()
        End If

        If Trim(UCase(result)) = "SUCCESS" Then
            Call InsertinTallyIntegrationLog(EID, xmlBPMTid, xmldoctype, "CONFIRMATION", xmldoctype, 1, "SUCCESS", msg)
        Else
            Call InsertinTallyIntegrationLog(EID, xmlBPMTid, xmldoctype, "CONFIRMATION", xmldoctype, 0, "FAIL", msg)
        End If

        Return sb.ToString()
    End Function

    Function UPDATIONFLAG(Data As Stream) As XElement Implements IBPMCustomWS.UPDATIONFLAG
        Dim Result = ""
        Dim msg As String = ""
        Dim Key As String = "", EID As String = "", DocType As String = "", UID As String = 0
        Try
            'Convert steam into string 
            Dim reader As New StreamReader(Data)
            Dim strData As String = reader.ReadToEnd()
            'Decode it to UTF-8
            strData = HttpUtility.UrlDecode(strData)
            Dim Data1 As New StringBuilder(strData)

            Dim strr As String = readxmlandUpdate(strData)

            msg = "<ENVELOPE> <HEADER><TALLYREQUEST>UPDATE</TALLYREQUEST></HEADER> <BODY>  <DATA>   <VALIDATION> " & strr & " </VALIDATION></DATA> </BODY></ENVELOPE>"

        Catch ex As Exception
            ErrorLog.sendMail("BPMTallyInt.UpdationFlag", ex.Message)
            'Return "RTO"
        End Try
        Dim xmldoc As XDocument = XDocument.Parse(msg)
        Return xmldoc.Root
    End Function

    Public Function readxmlandUpdate(ByRef str As String) As String
        Dim result As String = String.Empty
        Dim msg As String = String.Empty
        Dim xmlDocRead As New XmlDocument()
        xmlDocRead.LoadXml(str)

        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim da As New SqlDataAdapter("", con)
        Dim ds As New DataSet

        Dim Data1 As New StringBuilder(str)
        Try


            If xmlDocRead.ChildNodes.Count >= 1 Then

                Dim SelNodesTxt As String = xmlDocRead.DocumentElement.Name
                Dim Cnt As Integer = 0
                Dim nodes As XmlNodeList = xmlDocRead.SelectNodes(SelNodesTxt)
                For Each node As XmlNode In nodes
                    Cnt += 1
                    Dim MainVar As String = String.Empty
                    Dim globalVar As String = String.Empty
                    Dim ChildVar As String = String.Empty
                    Dim xmldoctype As String = String.Empty
                    Dim xmlecode As String = String.Empty
                    Dim apikey As String = String.Empty
                    Dim xmlBPMTIDS As String = String.Empty
                    Dim Key As String = "", EID As String = "", DocType As String = "", UID As String = 0
                    For c As Integer = 0 To node.ChildNodes.Count - 1
                        If UCase(node.ChildNodes.Item(c).Name) = "BODY" Then
                            For C1 As Integer = 0 To node.Item("BODY").ChildNodes.Count - 1
                                If node.Item("BODY").ChildNodes.Item(C1).Name = "DATA" Then
                                    For c2 As Integer = 0 To node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Count - 1
                                        If node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).Name = "VALIDATION" Then
                                            For P As Integer = 0 To node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Count - 1
                                                If UCase(node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).Name) = "BPMTIDS" Then
                                                    xmlBPMTIDS = node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).InnerText.ToString()
                                                End If
                                            Next
                                            If Not String.IsNullOrEmpty(xmlBPMTIDS) Then
                                                Dim cmd As New SqlCommand("Update mmm_mst_doc set isexport=0 where tid in ( SELECT * FROM dms.inputstring1('" & xmlBPMTIDS & "')) ", con)

                                                If con.State <> ConnectionState.Open Then
                                                    con.Open()
                                                End If
                                                cmd.ExecuteNonQuery()
                                                msg = "<BPMTIDS>" & xmlBPMTIDS.ToString() & "</BPMTIDS><RESULT>SUCCESS</RESULT>"
                                            Else
                                                msg = "<RESULT>FAIL</RESULT>"
                                            End If

                                        End If
                                    Next
                                End If
                            Next
                        End If
                    Next

                Next
            End If
        Catch ex As Exception
            msg = "<RESULT>FAIL</RESULT>"
        End Try

        Return msg
    End Function

    Function INWARDEDIT(Data As Stream) As XElement Implements IBPMCustomWS.INWARDEDIT
        Dim Result = ""
        Dim msg As String = ""
        Dim Key As String = "", EID As String = "", DocType As String = "", UID As String = 0
        Try
            'Convert steam into string 
            Dim reader As New StreamReader(Data)
            Dim strData As String = reader.ReadToEnd()
            'Decode it to UTF-8
            strData = HttpUtility.UrlDecode(strData)
            Dim Data1 As New StringBuilder(strData)

            Dim strr As String = readxmlandgivestringINWARDEDIT(strData)

            msg = "<ENVELOPE> <HEADER><TALLYREQUEST>EXPORT VOUCHERS</TALLYREQUEST></HEADER> <BODY>  <DATA>   <VALIDATION> " & strr & " </VALIDATION></DATA> </BODY></ENVELOPE>"
        Catch ex As Exception
            ErrorLog.sendMail("BPMTallyInt.InwardEdit", ex.Message)
            'Return "RTO"
        End Try
        Dim xmldoc As XDocument = XDocument.Parse(msg)
        Return xmldoc.Root
    End Function

    Public Function readxmlandgivestringINWARDEDIT(ByRef str As String) As String
        Dim result As String = String.Empty
        Dim msg As String = String.Empty
        Dim xmlDocRead As New XmlDocument()
        xmlDocRead.LoadXml(str)

        Dim TransCountforLog As Integer = 0

        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim da As New SqlDataAdapter("", con)
        Dim ds As New DataSet
        Dim Data1 As New StringBuilder(str)
        Dim isVAlidDoctype As Integer = 0

        Dim MainVar As String = String.Empty
        Dim cntt As Integer = 0
        Dim globalVar As String = String.Empty
        Dim ChildVar As String = String.Empty
        Dim xmldoctype As String = String.Empty
        Dim xmlecode As String = String.Empty
        Dim apikey As String = String.Empty
        Dim xmlBPMTID As Integer = 0
        Dim xmleid As Integer = 0

        If xmlDocRead.ChildNodes.Count >= 1 Then
            Dim SelNodesTxt As String = xmlDocRead.DocumentElement.Name
            Dim Cnt As Integer = 0
            Dim nodes As XmlNodeList = xmlDocRead.SelectNodes(SelNodesTxt)
            For Each node As XmlNode In nodes
                Cnt += 1
                MainVar = String.Empty
                cntt = 0
                globalVar = String.Empty
                ChildVar = String.Empty
                xmldoctype = String.Empty
                xmlecode = String.Empty
                apikey = String.Empty
                xmlBPMTID = 0
                xmleid = 0

                Dim xmltallycanceltag As String = String.Empty
                Dim Key As String = "", EID As String = "", DocType As String = "", UID As String = 0
                For c As Integer = 0 To node.ChildNodes.Count - 1
                    If UCase(node.ChildNodes.Item(c).Name) = "BODY" Then
                        For C1 As Integer = 0 To node.Item("BODY").ChildNodes.Count - 1
                            If node.Item("BODY").ChildNodes.Item(C1).Name = "DATA" Then
                                For c2 As Integer = 0 To node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Count - 1
                                    If node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).Name = "VALIDATION" Then
                                        For P As Integer = 0 To node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Count - 1
                                            If UCase(node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).Name) = "FORMNAME" Then
                                                xmldoctype = node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).InnerText.ToString()
                                            ElseIf UCase(node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).Name) = "ENTITY" Then
                                                xmlecode = node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).InnerText.ToString()
                                            ElseIf UCase(node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).Name) = "BPMTID" Then
                                                xmlBPMTID = Val(node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).InnerText)
                                            End If
                                        Next

                                        If Not String.IsNullOrEmpty(xmlBPMTID) Then
                                            Dim cmd As New SqlCommand("select isnull(TallyIsActive,0)[Count] from mmm_mst_master where tid =" & xmlBPMTID & " ", con)

                                            If con.State <> ConnectionState.Open Then
                                                con.Open()
                                            End If
                                            cntt = Convert.ToInt32(cmd.ExecuteScalar())

                                            If cntt = 1 Then
                                                If Not String.IsNullOrEmpty(xmldoctype) And Not String.IsNullOrEmpty(xmlecode) Then
                                                    da.SelectCommand.CommandText = "Select eid from mmm_mst_entity where code='" & xmlecode.ToString() & "'"
                                                    da.SelectCommand.CommandType = CommandType.Text
                                                    If con.State <> ConnectionState.Open Then
                                                        con.Open()
                                                    End If
                                                    xmleid = Convert.ToInt32(da.SelectCommand.ExecuteScalar())

                                                    da.SelectCommand.CommandText = "Select * from mmm_mst_fields where eid=" & xmleid & " and documenttype='" & xmldoctype.ToString() & "' order by displayname "
                                                    da.SelectCommand.CommandType = CommandType.Text
                                                    da.Fill(ds, "data")
                                                Else
                                                    msg = msg & " <RESULT>FORMNAME OR ENTITY NOT SUPPLIED</RESULT> <STATUS>ERROR</STATUS> "
                                                End If
                                                'code to get API key of the entity
                                                If xmleid > 0 Then
                                                    da.SelectCommand.CommandText = "Select isnull(apikey,'NA') from mmm_mst_entity where eid=" & xmleid & ""
                                                    ' da.Fill(ds, "apikey")
                                                    apikey = Convert.ToString(da.SelectCommand.ExecuteScalar())
                                                    If apikey = "NA" Then
                                                        msg = msg & " <RESULT>API KEY NOT CONFIGUERED FOR ENTITY</RESULT> <STATUS>ERROR</STATUS> "
                                                    End If
                                                    da.SelectCommand.CommandText = "Select count(*) from mmm_mst_forms where eid=" & xmleid & " and formname='" & xmldoctype.ToString() & "'"
                                                    da.SelectCommand.CommandType = CommandType.Text
                                                    isVAlidDoctype = Convert.ToInt32(da.SelectCommand.ExecuteScalar())
                                                Else
                                                    msg = msg & " <RESULT>INVALID ENTITY</RESULT> <STATUS>ERROR</STATUS> "
                                                End If
                                                ''code here is for cancel field
                                                'da.SelectCommand.CommandText = "select TallyCancelXMLTag from mmm_mst_forms where eid=" & xmleid & " and formname='" & xmldoctype.ToString() & "'"
                                                'If con.State <> ConnectionState.Open Then
                                                '    con.Open()
                                                'End If
                                                'xmltallycanceltag = da.SelectCommand.ExecuteScalar().ToString()
                                            End If
                                        End If
                                    End If

                                    If cntt = 1 Then
                                        Dim rowno As Integer = 0
                                        If node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).Name = "REQUESTDATA" Then
                                            For P As Integer = 0 To node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Count - 1
                                                If UCase(node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).Name) = "TALLYMESSAGE" Then
                                                    For d As Integer = 0 To node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).ChildNodes.Count - 1
                                                        For s As Integer = 0 To ds.Tables("data").Rows.Count - 1
                                                            If UCase(ds.Tables("data").Rows(s).Item("inwardxmltagname").ToString()) = UCase(node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).ChildNodes.Item(d).Name) Then

                                                                If UCase(ds.Tables("data").Rows(s).Item("fieldtype").ToString()) = "CHILD ITEM" Then
                                                                    da.SelectCommand.CommandType = CommandType.Text
                                                                    da.SelectCommand.CommandText = "Select * from mmm_mst_fields where eid=" & xmleid & " and documenttype='" & ds.Tables("data").Rows(s).Item("DROPDOWN").ToString().Trim() & "'"
                                                                    da.Fill(ds, "child")
                                                                    ChildVar = ds.Tables("data").Rows(s).Item("DROPDOWN").ToString().Trim() & "::{}"
                                                                    For x As Integer = 0 To ds.Tables("child").Rows.Count - 1
                                                                        For l As Integer = 0 To node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).ChildNodes.Item(d).ChildNodes.Count - 1
                                                                            If UCase(node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).ChildNodes.Item(d).ChildNodes.Item(l).Name).ToString() = UCase(ds.Tables("child").Rows(x).Item("inwardxmltagname").ToString()) Then
                                                                                ChildVar = ChildVar & "()" & UCase(ds.Tables("child").Rows(x).Item("displayname").ToString()) & "<>" & node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).ChildNodes.Item(d).ChildNodes.Item(l).InnerText
                                                                            End If
                                                                        Next
                                                                    Next
                                                                    ChildVar = ChildVar & "{}"
                                                                Else
                                                                    globalVar = globalVar & UCase(ds.Tables("data").Rows(s).Item("displayname").ToString()) & "::" & node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).ChildNodes.Item(d).InnerText & "|"
                                                                End If
                                                            End If
                                                        Next
                                                        If Not String.IsNullOrEmpty(xmltallycanceltag) Then
                                                            If UCase(xmltallycanceltag) = UCase(node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).ChildNodes.Item(d).Name) Then
                                                                globalVar = xmltallycanceltag & "::" & node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).ChildNodes.Item(d).InnerText & "|" & globalVar
                                                            End If
                                                        End If
                                                    Next
                                                    If ChildVar.Length > 2 Then
                                                        ChildVar = ChildVar.Remove(ChildVar.Length - 2)
                                                    End If
                                                    Try
                                                        result = "Key$$" & apikey & "~DOCTYPE$$" & xmldoctype.ToString() & "~Data$$" & globalVar & ChildVar
                                                        Dim arrData As String() = Split(result, "~")
                                                        For i As Integer = 0 To arrData.Length - 1
                                                            Dim ar = Split(arrData(i), "$$")
                                                            If ar(0).ToUpper().Trim() = "KEY" Then
                                                                Key = ar(1)
                                                            ElseIf ar(0).ToUpper().Trim() = "DOCTYPE" Then
                                                                DocType = ar(1)
                                                            ElseIf ar(0).ToUpper().Trim() = "DATA" Then
                                                                result = ar(1)
                                                            End If
                                                        Next
                                                        Dim DsS As New DataSet()
                                                        DsS = AuthenticateWSRequest(Key)
                                                        If DsS.Tables(0).Rows.Count > 0 Then
                                                            EID = DsS.Tables(0).Rows(0).Item("EID")
                                                            UID = DsS.Tables(1).Rows(0).Item("uid")
                                                            Dim objUp As New UpdateData()
                                                            Dim DocExist As Boolean = False
                                                            Dim Keys As String = ""
                                                            DocExist = objUp.GetDOCID(EID, Keys, DocType, result)
                                                            If DocExist = True Then
                                                                Dim dsDoc As New DataSet()
                                                                If DocType.Trim.ToUpper <> "USER" Then
                                                                    dsDoc = objUp.GetDocDetails(DocType, EID, Keys)
                                                                    If dsDoc.Tables(0).Rows.Count > 0 Then
                                                                        For Each column As DataColumn In dsDoc.Tables(0).Columns
                                                                            If Not IsFieldSupplyed(result, column.ColumnName) Then
                                                                                result = result & "|" & column.ColumnName & "::" & dsDoc.Tables(0).Rows(0).Item(column.ColumnName)
                                                                            End If
                                                                        Next
                                                                    End If
                                                                End If
                                                                result = objUp.UpdateData(EID, DocType, UID, result)
                                                            Else
                                                                If (ChkEnableInsertOnEdit(EID, DocType)) Then
                                                                    result = CommanUtil.ValidateParameterByDocumentType(EID, DocType, UID, result)
                                                                Else
                                                                    result = "Based on supplied keys, document does not exist!"
                                                                End If
                                                            End If
                                                            'result = CommanUtil.ValidateParameterByDocumentType(EID, DocType, UID, result)
                                                        Else
                                                            result = "Sorry!!! Authentication failed."
                                                        End If
                                                        CommanUtil.SaveServicerequest(Data1, "BPMTallyWS", "InwardEdit", result)
                                                        'msg = msg & result & "" Record updated successfully.
                                                        If result.ToUpper().Contains("UPDATED SUCCESSFULLY") = True Then
                                                            msg = msg & " <RESULT>" & result & "</RESULT> <STATUS>SUCCESS</STATUS> "
                                                        Else
                                                            msg = msg & " <RESULT>" & result & "</RESULT> <STATUS>ERROR</STATUS> "
                                                        End If
                                                        result = ""
                                                        globalVar = ""
                                                        ChildVar = ""
                                                    Catch ex As Exception
                                                        ErrorLog.sendMail("BPMTallyInt.InwardEdit", ex.Message)
                                                        result = ""  ' ex.Message.ToString
                                                        msg = msg & " <RESULT>" & ex.Message.ToString & "</RESULT> <STATUS>ERROR</STATUS> "
                                                        globalVar = ""
                                                        ChildVar = ""
                                                    End Try
                                                End If
                                                TransCountforLog += 1
                                            Next
                                        End If
                                    Else
                                        msg = msg & " <RESULT>YOU ARE NOT AUTHORIZED</RESULT> <STATUS> ERROR </STATUS> "
                                    End If
                                Next
                            End If
                        Next
                    End If
                Next
            Next
        End If
        If isVAlidDoctype = 0 Then
            msg = " <RESULT>INVALID FORMNAME</RESULT> <STATUS>ERROR</STATUS> "
        End If

        If msg.ToUpper().Contains("UPDATED SUCCESSFULLY") Then
            Call InsertinTallyIntegrationLog(xmleid, xmlBPMTID, xmldoctype, "INWARDCANCEL", xmldoctype, TransCountforLog, "SUCCESS", msg)
        Else
            Call InsertinTallyIntegrationLog(xmleid, xmlBPMTID, xmldoctype, "INWARDCANCEL", xmldoctype, TransCountforLog, "FAIL", msg)
        End If

        Return msg
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


    '' for demo purpose - luxotica outward record with master records in child item 
    Public Function Return_OUtwardWithMaster(ByVal Dtype As String, ByVal eid As Integer, ByVal DistCode As String, ByVal rowfiltercolumn As String, ByVal BPMTID As String) As String

        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("InsertSMSALERTLog", con)
        Dim doc As XmlDocument = New XmlDocument()
        Dim Vdt As New DataTable
        Dim SITdt As New DataTable
        Dim Invtrdt As New DataTable
        'Dim ds As New DataSet
        Dim strB As StringBuilder = New System.Text.StringBuilder()
        Dim objRel As New Relation()
        Dim ds As New DataSet()
        Dim dsD As New DataSet()
        ds = objRel.GetAllFields(eid)
        Dim StrQuery = GenearateQuery1(eid, Dtype, ds)
        'Dim StrQuery = objRel.GenearateQuery1(eid, Dtype, ds)
        Dim check As Integer = 0
        strB.Append("<ENVELOPE>")
        strB.Append("<HEADER>")
        strB.Append("<RESPONSE>" & "IMPORT DATA")
        strB.Append("</RESPONSE>")
        strB.Append("</HEADER>")
        strB.Append("<BODY>")
        strB.Append("<DATA>")
        strB.Append("<VALIDATION>")
        If Not String.IsNullOrEmpty(BPMTID) Then


            oda.SelectCommand.CommandText = "Select isnull(TallyIsActive,0) from mmm_mst_master where tid=" & Val(BPMTID) & ""
            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            check = Convert.ToInt32(oda.SelectCommand.ExecuteScalar())


            If check = 1 Then
                If DistCode Is Nothing Then
                    StrQuery = "SELECT  v" & eid & Dtype.Trim.Replace(" ", "_") & ".tid  AS DOCID, " & StrQuery
                Else
                    Dim filter As String = " and v" & eid & Dtype.Trim.Replace(" ", "_") & ".[" & rowfiltercolumn & "]='" & DistCode & "'and v" & eid & Dtype.Trim.Replace(" ", "_") & ".[ISEXPORT]=1"
                    StrQuery = "SELECT  v" & eid & Dtype.Trim.Replace(" ", "_") & ".tid  AS DOCID, " & StrQuery & filter

                    ' Dim filter As String = " and v" & eid & Dtype.Trim.Replace(" ", "_") & ".[" & rowfiltercolumn & "]='" & DistCode & "' and v" & eid & Dtype.Trim.Replace(" ", "_") & ".[ISEXPORT]=1"
                    ' StrQuery = "SELECT  v" & eid & Dtype.Trim.Replace(" ", "_") & ".tid  AS DOCID, " & StrQuery & filter
                End If

                oda = New SqlDataAdapter(StrQuery, con)
                oda.Fill(Vdt)

                Dim flds As New DataTable
                'query to get fields of the documenttype
                Dim fldqry As String = "Select displayname,fieldmapping,fieldtype,dropdown,OUTWARDXMLTAGNAME  from mmm_mst_fields where eid=" & eid & " and documenttype='" & Dtype.ToString.Trim() & "' and OUTWARDXMLTAGNAME is not null"
                oda = New SqlDataAdapter(fldqry, con)
                oda.Fill(flds)




                strB.Append("<BILLINGCODE>" & "IN013205004B")
                strB.Append("</BILLINGCODE>")
                strB.Append("<PASSWD>" & "XXXXX")
                strB.Append("</PASSWD>")
                strB.Append("</VALIDATION>")
                strB.Append("<IMPORTDATA>")
                strB.Append("<TALLYMESSAGE>")
                strB.Append("<VOUCHERS>")

                Dim opentag As String = ""
                Dim closetag As String = ""
                Dim val As String = ""
                Dim str As String = ""
                Dim strchild As String = ""
                Dim childopentag As String = ""
                Dim childclosetag As String = ""
                Dim childval As String = ""

                Dim masteropentag As String = ""
                Dim masterclosetag As String = ""
                Dim masterval As String = ""
                Dim asd As String = ""
                Dim stockitems As String = ""
                Dim INVENTORYENTRIES As String = ""
                Dim maindoc As String = ""
                Dim SSS As String = ""
                Dim xxx As String = ""
                Dim kk As String = ""
                For j As Integer = 0 To Vdt.Rows.Count - 1
                    maindoc = "<BPMTID>" & Vdt.Rows(j).Item("DOCID").ToString() & "</BPMTID>"
                    INVENTORYENTRIES = ""
                    stockitems = ""
                    For i As Integer = 0 To flds.Rows.Count - 1
                        opentag = "<" & flds.Rows(i).Item("OUTWARDXMLTAGNAME").ToString() & ">"
                        val = ""
                        strchild = ""


                        ''
                        If UCase(flds.Rows(i).Item("fieldtype")).ToString.Trim() = "CHILD ITEM" Then
                            oda.SelectCommand.CommandText = "Select displayname,fieldmapping,fieldtype,dropdown,dropdowntype,OUTWARDXMLTAGNAME  from mmm_mst_fields where eid=" & eid & " and documenttype='" & UCase(flds.Rows(i).Item("dropdown")).ToString.Trim() & "' and OUTWARDXMLTAGNAME is not null "
                            Dim child As New DataTable
                            oda.Fill(child)

                            If child.Rows.Count > 0 Then
                                Dim childquery = GenearateQuery1(eid, UCase(flds.Rows(i).Item("dropdown")).ToString.Trim(), ds)
                                Dim filter As String = " where  DOCID='" & Vdt.Rows(j).Item("DOCID").ToString() & "'"
                                childquery = "SELECT top 5 * from  v" & eid & flds.Rows(i).Item("dropdown").ToString().Trim.Replace(" ", "_") & filter
                                oda = New SqlDataAdapter(childquery, con)
                                oda.SelectCommand.CommandTimeout = 5000
                                oda.SelectCommand.CommandType = CommandType.Text
                                Dim childv As New DataTable
                                oda.Fill(childv)

                                For c As Integer = 0 To childv.Rows.Count - 1

                                    For k As Integer = 0 To child.Rows.Count - 1


                                        For Each col As DataColumn In childv.Columns
                                            If UCase(child.Rows(k).Item("displayname")).ToString() = UCase(col.ColumnName) Then
                                                childopentag = "<" & child.Rows(k).Item("OUTWARDXMLTAGNAME").ToString() & ">"
                                                childval = ""
                                                masterval = ""
                                                If child.Rows(k).Item("dropdowntype") = "MASTER VALUED" Then
                                                    masterval = mastervalued_Outwardwithmaster(child.Rows(k).Item("dropdown").ToString(), childv.Rows(c).Item(col.ColumnName).ToString(), eid)
                                                    Dim ss As String() = child.Rows(k).Item("dropdown").ToString().ToString.Split("-")
                                                    oda.SelectCommand.CommandText = "select " & ss(2).ToString() & " from mmm_mst_" & ss(0).ToString() & " where eid=" & eid & " and tid=" & childv.Rows(c).Item(col.ColumnName).ToString() & ""
                                                    oda.SelectCommand.CommandTimeout = 5000
                                                    If con.State <> ConnectionState.Open Then
                                                        con.Open()
                                                    End If
                                                    stockitems = stockitems & "<STOCKITEM>" & masterval & "</STOCKITEM>"
                                                    childval = oda.SelectCommand.ExecuteScalar()
                                                Else
                                                    childval = childv.Rows(c).Item(col.ColumnName).ToString()
                                                End If
                                            End If
                                        Next
                                        childclosetag = "</" & child.Rows(k).Item("OUTWARDXMLTAGNAME").ToString() & ">"
                                        strchild = strchild & childopentag & childval & childclosetag
                                    Next
                                    strchild = strchild & "</INVENTORYENTRY>"
                                    kk = kk & "<INVENTORYENTRY>" & strchild
                                    strchild = ""
                                Next

                            End If
                            closetag = "</" & flds.Rows(i).Item("OUTWARDXMLTAGNAME").ToString() & ">"
                            str = str & opentag & strchild & val & closetag
                            INVENTORYENTRIES = kk
                            kk = ""

                        Else
                            For Each column As DataColumn In Vdt.Columns
                                If UCase(column.ColumnName) = UCase(flds.Rows(i).Item("OUTWARDXMLTAGNAME")).ToString() Then
                                    val = Vdt.Rows(j).Item(column.ColumnName).ToString()
                                End If
                            Next
                            closetag = "</" & flds.Rows(i).Item("OUTWARDXMLTAGNAME").ToString() & ">"
                            str = str & opentag & strchild & val & closetag
                            maindoc = maindoc & opentag & val & closetag
                        End If
                    Next
                    maindoc = "<VOUCHER>" & maindoc & "<STOCKITEMS>" & stockitems & "</STOCKITEMS>" & "<INVENTORYENTRIES>" & INVENTORYENTRIES & "</INVENTORYENTRIES></VOUCHER>"
                    xxx = xxx & maindoc
                Next

                strB.Append(xxx)
                strB.Append("</VOUCHERS>")
                strB.Append("</TALLYMESSAGE>")
                strB.Append("</IMPORTDATA>")

                strB.Append("</DATA>")
                strB.Append("</BODY>")
                strB.Append("</ENVELOPE>")
            Else
                strB.Append("<RESULT>")
                strB.Append("YOU ARE NOT AUTHORIZED")
                strB.Append("</RESULT>")
                strB.Append("</VALIDATION>")
                strB.Append("</DATA>")
                strB.Append("</BODY>")
                strB.Append("</ENVELOPE>")
            End If

        Else
            strB.Append("<RESULT>")
            strB.Append("YOU ARE NOT AUTHORIZED")
            strB.Append("</RESULT>")
            strB.Append("</VALIDATION>")
            strB.Append("</DATA>")
            strB.Append("</BODY>")
            strB.Append("</ENVELOPE>")

        End If


        con.Dispose()

        Return strB.ToString()
    End Function


    Protected Function mastervalued_Outwardwithmaster(ByVal str As String, ByVal tid As Integer, ByVal eid As Integer) As String
        Dim Result As String = ""
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As New SqlDataAdapter("", con)
        Dim ds As New DataSet
        Dim masterval As String = ""
        Dim val As String = ""
        Dim hold As String() = str.ToString.Split("-")
        Dim mopentag As String = ""
        Dim mclosetag As String = ""

        If hold.Length > 0 Then
            oda.SelectCommand.CommandText = "Select displayname,fieldmapping,dropdown,dropdowntype,outwardxmltagname from mmm_mst_fields where eid=" & eid & " and documenttype='" & hold(1).ToString() & "' and outwardxmltagname is not null"
            oda.Fill(ds, "data")

            If ds.Tables("data").Rows.Count > 0 Then
                oda.SelectCommand.CommandText = "Select * from v" & eid & hold(1).ToString().Trim.Replace(" ", "_") & " where tid=" & tid & " "
                oda.Fill(ds, "master")
                If ds.Tables("master").Rows.Count > 0 Then

                    For j As Integer = 0 To ds.Tables("master").Rows.Count - 1
                        For i As Integer = 0 To ds.Tables("data").Rows.Count - 1
                            mopentag = "<" & ds.Tables("data").Rows(i).Item("outwardxmltagname").ToString() & ">"
                            For Each column As DataColumn In ds.Tables("master").Columns
                                If UCase(column.ColumnName) = UCase(ds.Tables("data").Rows(i).Item("displayname")).ToString() Then

                                    If ds.Tables("data").Rows(i).Item("dropdowntype").ToString() = "MASTER VALUED" Then
                                        Dim ab As String() = ds.Tables("data").Rows(i).Item("dropdown").ToString().Split("-")
                                        If UCase(ab(0)).ToString = "MASTER" Then
                                            oda.SelectCommand.CommandText = "Select " & ab(2).ToString() & " from mmm_mst_master where eid=" & eid & " and tid=" & ds.Tables("master").Rows(j).Item(column.ColumnName).ToString() & ""
                                            oda.SelectCommand.CommandTimeout = 5000
                                            If con.State <> ConnectionState.Open Then
                                                con.Open()
                                            End If
                                            val = oda.SelectCommand.ExecuteScalar()
                                        ElseIf UCase(ab(0)).ToString = "STATIC" Then
                                            oda.SelectCommand.CommandText = "Select " & ab(2).ToString() & " from mmm_mst_doc where eid=" & eid & " and tid=" & ds.Tables("master").Rows(j).Item(column.ColumnName).ToString() & ""
                                            oda.SelectCommand.CommandTimeout = 5000
                                            val = oda.SelectCommand.ExecuteScalar()
                                        End If
                                    Else
                                        val = ds.Tables("master").Rows(j).Item(column.ColumnName).ToString()
                                    End If
                                End If
                            Next
                            mclosetag = "</" & ds.Tables("data").Rows(i).Item("outwardxmltagname").ToString() & ">"
                            Result = Result & mopentag & val & mclosetag
                        Next
                    Next
                End If
            End If
        End If
        Return Result
    End Function




    'code below is for bpm.sequelone.com

    Function OUTWARDBULK(Data As Stream) As XElement Implements IBPMCustomWS.OUTWARDBULK
        Dim Result As String = ""
        Dim Key As String = "", EID As Integer = 0, DocType As String = "", UID As String = 0
        Try
            '' here code to read xml for doc type and field name and create xml output string and retun xml string
            'Convert steam into string 
            Dim reader As New StreamReader(Data)
            Dim strData As String = reader.ReadToEnd()
            'Decode it to UTF-8
            strData = HttpUtility.UrlDecode(strData)
            Dim Data1 As New StringBuilder(strData)

            Dim strinput As String = ReturnInputParaValuesOutwardBulk(strData)
            Dim strinputArr() As String = strinput.Split("|")

            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As New SqlConnection(conStr)
            Dim da As New SqlDataAdapter("", con)
            If strinputArr.Length = 3 Then

                da.SelectCommand.CommandText = "Select eid from mmm_mst_entity where code='" & strinputArr(1).ToString().Trim & "'"
                If con.State <> ConnectionState.Open Then
                    con.Open()
                End If
                EID = Convert.ToInt32(da.SelectCommand.ExecuteScalar())

                da.SelectCommand.CommandText = "select isnull(rowfilterbpmfield,0) from mmm_mst_forms where formname ='" & strinputArr(0).ToString & "' and eid=" & EID & ""


                Dim rwfield As String = da.SelectCommand.ExecuteScalar.ToString()
                Result = POrderOutwardBulk(strinputArr(0).ToString, EID, strinputArr(2).ToString, rwfield.ToString())
                If Result.Length > 5 Then
                    Result = Result.Replace("&", "&amp;")
                End If
                CommanUtil.SaveServicerequest(Data1, "BPMCustomWS", "OutwardBULK", Result)
            Else
                Result = "Please check you Request Format"
            End If
            Dim xmldoc As XDocument = XDocument.Parse(Result)
            Return xmldoc.Root
        Catch ex As Exception
            ErrorLog.sendMail("BPMCustomeWS.OutwardBulk", ex.Message & " : " & Result)
        End Try

    End Function


    Public Function POrderOutwardBulk(ByVal Dtype As String, ByVal eid As Integer, ByVal RequestID As String, ByVal ROWFILTER As String) As String
        Try


            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As SqlConnection = New SqlConnection(conStr)
            Dim oda As SqlDataAdapter = New SqlDataAdapter("InsertSMSALERTLog", con)
            Dim doc As XmlDocument = New XmlDocument()
            Dim Vdt As New DataTable
            Dim SITdt As New DataTable
            Dim Invtrdt As New DataTable
            Dim SuccessBPMTID As Integer = 0
            Dim FailBPMTID As Integer = 0
            Dim TransCountforLog As Integer = 0
            Dim ENV As StringBuilder = New System.Text.StringBuilder()
            'Dim ds As New DataSet
            Dim strB As StringBuilder = New System.Text.StringBuilder()
            Dim objRel As New Relation()
            Dim ds As New DataSet()
            Dim dsD As New DataSet()
            ds = objRel.GetAllFields(eid)
            Dim StrQuery = GenearateQuery1(eid, Dtype, ds)
            Dim check As Integer = 0
            ENV.Append("<ENVELOPES>")
            ENV.Append("<HEADER>")
            ENV.Append("<REQUESTID>" & RequestID.ToString() & "</REQUESTID>")
            ENV.Append("</HEADER>")


            'StrQuery = "SELECT  v" & eid & Dtype.Trim.Replace(" ", "_") & ".tid  AS DOCID, " & StrQuery

            ' Dim filter As String = " and v" & eid & Dtype.Trim.Replace(" ", "_") & ".[ISEXPORT]=1"
            StrQuery = "SELECT  v" & eid & Dtype.Trim.Replace(" ", "_") & ".tid  AS DOCID, " & StrQuery & " and v" & eid & Dtype.Trim.Replace(" ", "_") & ".[ISEXPORT]=1  order by v" & eid & Dtype.Trim.Replace(" ", "_") & ".[" & ROWFILTER.ToString() & "] "

            oda = New SqlDataAdapter(StrQuery, con)
            oda.Fill(Vdt)

            If Vdt.Rows.Count > 0 Then

                Dim fStrQuery As String = "SELECT  distinct v" & eid & Dtype.Trim.Replace(" ", "_") & ".[" & ROWFILTER.ToString() & "] as [" & ROWFILTER.ToString() & "]  from v" & eid & Dtype.Trim.Replace(" ", "_") & " where  v" & eid & Dtype.Trim.Replace(" ", "_") & ".[ISEXPORT]=1"
                Dim f3a As New DataTable
                oda = New SqlDataAdapter(fStrQuery, con)
                oda.Fill(f3a)

                Dim flds As New DataTable
                'query to get fields of the documenttype
                Dim fldqry As String = "Select displayname,fieldmapping,fieldtype,dropdown,OUTWARDXMLTAGNAME,DROPDOWNTYPE  from mmm_mst_fields where eid=" & eid & " and documenttype='" & Dtype.ToString.Trim() & "' and OUTWARDXMLTAGNAME is not null"
                oda = New SqlDataAdapter(fldqry, con)
                oda.Fill(flds)


                Dim opentag As String = ""
                Dim closetag As String = ""
                Dim val As String = ""
                Dim str As String = ""
                Dim strchild As String = ""
                Dim childopentag As String = ""
                Dim childclosetag As String = ""
                Dim childval As String = ""

                Dim masteropentag As String = ""
                Dim masterclosetag As String = ""
                Dim masterval As String = ""
                Dim asd As String = ""
                Dim stockitems As String = ""
                Dim INVENTORYENTRIES As String = ""
                Dim maindoc As String = ""
                Dim SSS As String = ""
                Dim xxx As String = ""
                Dim kk As String = ""

                For n As Integer = 0 To f3a.Rows.Count - 1

                    Dim View As DataView
                    Dim tbl As DataTable
                    View = Vdt.DefaultView
                    View.RowFilter = "[" & ROWFILTER.Trim.Replace(" ", "") & "]='" & f3a.Rows(n).Item(ROWFILTER).ToString() & "'"
                    tbl = View.Table.DefaultView.ToTable()
                    ENV.Append("<ENVELOPE>")
                    ENV.Append("<HEADER>")
                    ENV.Append("<TALLYREQUEST>IMPORT VOUCHERS</TALLYREQUEST>")
                    ENV.Append("</HEADER>")
                    ENV.Append("<BODY>")
                    ENV.Append("<DATA>")
                    ENV.Append("<VALIDATION>")

                    ENV.Append("<BILLINGCODE>" & "IN013205004B")
                    ENV.Append("</BILLINGCODE>")
                    ENV.Append("<PASSWD>XXXXX</PASSWD>")
                    ENV.Append("</VALIDATION>")
                    ENV.Append("<IMPORTDATA>")
                    ENV.Append("<TALLYMESSAGE>")
                    ENV.Append("<VOUCHERS>")
                    maindoc = ""
                    For j As Integer = 0 To tbl.Rows.Count - 1
                        maindoc = maindoc & "<VOUCHER><BPMTID>" & tbl.Rows(j).Item("DOCID").ToString() & "</BPMTID>"
                        SuccessBPMTID = SuccessBPMTID & tbl.Rows(j).Item("DOCID").ToString() & ","
                        INVENTORYENTRIES = ""
                        stockitems = ""
                        For i As Integer = 0 To flds.Rows.Count - 1
                            opentag = "<" & flds.Rows(i).Item("OUTWARDXMLTAGNAME").ToString() & ">"
                            val = ""
                            strchild = ""

                            ''
                            If UCase(flds.Rows(i).Item("fieldtype")).ToString.Trim() = "CHILD ITEM" Then
                                oda.SelectCommand.CommandText = "Select displayname,fieldmapping,fieldtype,dropdown,dropdowntype,OUTWARDXMLTAGNAME,documenttype  from mmm_mst_fields where eid=" & eid & " and documenttype='" & UCase(flds.Rows(i).Item("dropdown")).ToString.Trim() & "' and OUTWARDXMLTAGNAME is not null "
                                Dim child As New DataTable
                                oda.Fill(child)

                                If child.Rows.Count > 0 Then
                                    Dim childquery = GenearateQuery1(eid, UCase(flds.Rows(i).Item("dropdown")).ToString.Trim(), ds)
                                    'Dim filter As String = " and  DOCID='" & Vdt.Rows(j).Item("DOCID").ToString() & "'"
                                    'childquery = "SELECT v" & eid & flds.Rows(i).Item("dropdown").ToString().Trim.Replace(" ", "_") & ".tid as [TID]," & childquery & filter
                                    Dim filter As String = " where  DOCID='" & tbl.Rows(j).Item("DOCID").ToString() & "'"
                                    childquery = "SELECT * from  v" & eid & flds.Rows(i).Item("dropdown").ToString().Trim.Replace(" ", "_") & filter
                                    oda = New SqlDataAdapter(childquery, con)
                                    oda.SelectCommand.CommandTimeout = 5000
                                    oda.SelectCommand.CommandType = CommandType.Text
                                    Dim childv As New DataTable
                                    Dim Qry As String = ""
                                    oda.Fill(childv)

                                    For c As Integer = 0 To childv.Rows.Count - 1

                                        For k As Integer = 0 To child.Rows.Count - 1
                                            For Each col As DataColumn In childv.Columns
                                                If UCase(child.Rows(k).Item("displayname")).ToString() = UCase(col.ColumnName) Then
                                                    childopentag = "<" & child.Rows(k).Item("OUTWARDXMLTAGNAME").ToString() & ">"
                                                    childval = ""
                                                    masterval = ""
                                                    If child.Rows(k).Item("dropdowntype") = "MASTER VALUED" Then
                                                        Qry = "select dms.udf_split('" & child.Rows(k).Item("dropdown").ToString() & "'," & child.Rows(k).Item("fieldmapping").ToString() & ") [value] " & " from MMM_MST_DOC_item  where  documenttype='" & child.Rows(k).Item("documenttype").ToString() & "' and docid=" & tbl.Rows(j).Item("DOCID").ToString() & " and tid=" & childv.Rows(c).Item("tid")
                                                        oda.SelectCommand.CommandType = CommandType.Text
                                                        If con.State <> ConnectionState.Open Then
                                                            con.Open()
                                                        End If
                                                        Dim mval As String = oda.SelectCommand.ExecuteScalar().ToString
                                                        Dim ss As String() = child.Rows(k).Item("dropdown").ToString().ToString.Split("-")
                                                        oda.SelectCommand.CommandText = "select " & ss(2).ToString() & IIf(UCase(ss(0)).ToString = "MASTER", " from mmm_mst_master ", " from mmm_mst_doc ") & " where documenttype='" & ss(1).ToString() & "' and eid=" & eid & " and tid=" & childv.Rows(c).Item(col.ColumnName)
                                                        oda.SelectCommand.CommandTimeout = 5000
                                                        If con.State <> ConnectionState.Open Then
                                                            con.Open()
                                                        End If
                                                        childval = oda.SelectCommand.ExecuteScalar()
                                                    Else
                                                        childval = childv.Rows(c).Item(col.ColumnName).ToString()
                                                    End If
                                                End If
                                            Next
                                            childclosetag = "</" & child.Rows(k).Item("OUTWARDXMLTAGNAME").ToString() & ">"
                                            strchild = strchild & childopentag & childval & childclosetag
                                        Next
                                        If childv.Rows.Count > 0 Then
                                            strchild = strchild & "</INVENTORYENTRY>"
                                            kk = kk & "<INVENTORYENTRY>" & strchild
                                            strchild = ""
                                        End If
                                    Next

                                End If
                                closetag = "</" & flds.Rows(i).Item("OUTWARDXMLTAGNAME").ToString() & ">"
                                str = str & opentag & strchild & val & closetag
                                INVENTORYENTRIES = kk
                                kk = ""
                            ElseIf UCase(flds.Rows(i).Item("fieldtype")).ToString.Trim() = "DROP DOWN" Then
                                For Each column As DataColumn In Vdt.Columns
                                    If UCase(column.ColumnName) = UCase(flds.Rows(i).Item("OUTWARDXMLTAGNAME")).ToString() Then
                                        val = tbl.Rows(j).Item(column.ColumnName).ToString()
                                    End If
                                Next
                                closetag = "</" & flds.Rows(i).Item("OUTWARDXMLTAGNAME").ToString() & ">"
                                str = str & opentag & strchild & val & closetag
                                maindoc = maindoc & opentag & val & closetag
                                ' masterval = mastervalued(flds.Rows(i).Item("dropdown").ToString(), Vdt.Rows(j).Item("DOCID").ToString(), eid)
                            Else
                                For Each column As DataColumn In Vdt.Columns
                                    If UCase(column.ColumnName) = UCase(flds.Rows(i).Item("OUTWARDXMLTAGNAME")).ToString() Then
                                        val = tbl.Rows(j).Item(column.ColumnName).ToString()
                                    End If
                                Next
                                closetag = "</" & flds.Rows(i).Item("OUTWARDXMLTAGNAME").ToString() & ">"
                                str = str & opentag & strchild & val & closetag
                                maindoc = maindoc & opentag & val & closetag
                            End If
                        Next

                        maindoc = maindoc & "<INVENTORYENTRIES>" & INVENTORYENTRIES & "</INVENTORYENTRIES></VOUCHER>"
                        'maindoc = "<VOUCHER>" & maindoc & "<INVENTORYENTRIES>" & INVENTORYENTRIES & "</INVENTORYENTRIES></VOUCHER>"


                    Next

                    ENV.Append(maindoc)
                    ENV.Append("</VOUCHERS>")
                    ENV.Append("</TALLYMESSAGE>")
                    ENV.Append("</IMPORTDATA>")
                    ENV.Append("</DATA>")
                    ENV.Append("</BODY>")
                    ENV.Append("</ENVELOPE>")

                Next
                ENV.Append("</ENVELOPES>")
            ElseIf Vdt.Rows.Count = 0 Then

                ENV.Append("<ENVELOPE>")
                ENV.Append("<BODY>")
                ENV.Append("<DATA>")
                ENV.Append("<VALIDATION>")
                ENV.Append("No Records Found")
                ENV.Append("</VALIDATION>")
                ENV.Append("</DATA>")
                ENV.Append("</BODY>")
                ENV.Append("</ENVELOPE>")
                ENV.Append("</ENVELOPES>")
            End If

            TransCountforLog = Vdt.Rows.Count
            If TransCountforLog > 0 Then
                Call InsertinTallyIntegrationLog(eid, SuccessBPMTID, Dtype, "OUTWARDBULK", Dtype, TransCountforLog, "SUCCESS", "Records")
            Else
                Call InsertinTallyIntegrationLog(eid, 0, Dtype, "OUTWARDBULK", Dtype, TransCountforLog, "FAIL", "No records")
            End If

            con.Dispose()
            ' ErrorLog.sendMail("BPMTallyInt.PORDER", strB.ToString())
            Return ENV.ToString()
        Catch ex As Exception
            Throw
            ErrorLog.sendMail("BPMTallyCustomws.PorderOUTWARDBULK", ex.Message)
        End Try
    End Function

    Public Function ReturnInputParaValuesOutwardBulk(ByVal str As String) As String
        Dim xmlDocRead As New XmlDocument()
        xmlDocRead.LoadXml(str)
        Dim ReturnVal As String = "NA"
        Dim filtervalue As String = String.Empty
        Dim DocumentType As String = String.Empty
        Dim xmlEcode As String = String.Empty
        Dim xmlBPMTID As Integer = 0
        Dim xmlRequestID As Integer = 0
        If xmlDocRead.ChildNodes.Count >= 1 Then
            Dim SelNodesTxt As String = xmlDocRead.DocumentElement.Name
            Dim nodes As XmlNodeList = xmlDocRead.SelectNodes(SelNodesTxt)

            For Each node As XmlNode In nodes
                For c As Integer = 0 To node.ChildNodes.Count - 1
                    'CODE BELOW IS FOR READING REQUESTID FROM REQUEST DONE BY USER
                    If UCase(node.ChildNodes.Item(c).Name) = "HEADER" Then
                        For C1 As Integer = 0 To node.Item("HEADER").ChildNodes.Count - 1
                            If node.Item("HEADER").ChildNodes.Item(C1).Name = "REQUESTID" Then
                                xmlRequestID = node.Item("HEADER").ChildNodes.Item(C1).InnerText.ToString()
                            End If
                        Next
                    End If

                    If UCase(node.ChildNodes.Item(c).Name) = "BODY" Then
                        For C1 As Integer = 0 To node.Item("BODY").ChildNodes.Count - 1
                            If node.Item("BODY").ChildNodes.Item(C1).Name = "DATA" Then
                                For c2 As Integer = 0 To node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Count - 1
                                    If node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).Name = "VALIDATION" Then

                                        For P As Integer = 0 To node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Count - 1


                                            If UCase(node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).Name) = "FORMNAME" Then
                                                DocumentType = node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).InnerText.ToString()
                                            ElseIf UCase(node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).Name) = "ENTITY" Then
                                                xmlEcode = node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).InnerText.ToString()
                                                'ElseIf UCase(node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).Name) = "DISTRIBUTORCODE" Then
                                                '    filtervalue = node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).InnerText.ToString()
                                                'ElseIf UCase(node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).Name) = "BPMTID" Then
                                                '    xmlBPMTID = Convert.ToInt32(node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).InnerText)
                                            End If
                                        Next
                                        ReturnVal = DocumentType & "|" & xmlEcode & "|" & xmlRequestID
                                        'below code is commented by vinay
                                        'ReturnVal = DocumentType & "|" & xmlEcode & "|" & filtervalue & "|" & xmlBPMTID  ' documenttype|EID|rowfiltervlaue|BPMTID by which we add it to where condition to filter rows according to it

                                    End If
                                Next
                            End If
                        Next
                    End If
                Next
            Next
        End If
        Return ReturnVal
        'ErrorLog.sendMail("BPMTallyWS.Outward", ReturnVal)
    End Function
    '

    Function INWARDBULKMASTER(Data As Stream) As XElement Implements IBPMCustomWS.INWARDBULKMASTER
        Dim Result = ""
        Dim strr As String = ""
        Dim Key As String = "", EID As String = "", DocType As String = "", UID As String = 0
        Try
            'Convert steam into string 
            Dim reader As New StreamReader(Data)
            Dim strData As String = reader.ReadToEnd()
            'Decode it to UTF-8
            strData = HttpUtility.UrlDecode(strData)
            Dim Data1 As New StringBuilder(strData)

            strr = InsertInward_BulkMASTER(strData)

            If strr.Length > 5 Then
                strr = strr.Replace("&", "&amp;")
            End If

            CommanUtil.SaveServicerequest(Data1, "BPMCUSTOMWS", "InwardBulkMASTER", strr)

        Catch ex As Exception
            ErrorLog.sendMail("BPMCUSTOMWS.InwardBulkMASTER", ex.Message)
            'Return "RTO"
        End Try
        Dim xmldoc As XDocument = XDocument.Parse(strr)
        Return xmldoc.Root


    End Function

    Public Function InsertInward_BulkMASTER(ByVal str As String) As String
        Dim result As String = String.Empty
        Dim msg As String = "<ENVELOPE> <HEADER><TALLYREQUEST>EXPORT VOUCHERS</TALLYREQUEST></HEADER> <BODY><DATA>"
        Dim msgclose As String = "</DATA></BODY></ENVELOPE>"
        Dim VALIDATION As String = ""
        Dim xmlDocRead As New XmlDocument()
        xmlDocRead.LoadXml(str)
        Dim TransCountforLog As Integer = 0


        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim da As New SqlDataAdapter("", con)
        Dim ds As New DataSet

        Dim REQEUSTID As String = ""

        Dim Data1 As New StringBuilder(str)
        Dim xmleid As Integer = 0

        Dim MainVar As String = String.Empty
        Dim cntt As Integer = 0
        Dim globalVar As String = String.Empty
        Dim ChildVar As String = String.Empty
        Dim xmldoctype As String = String.Empty
        Dim xmlecode As String = String.Empty
        Dim apikey As String = String.Empty
        Dim ReturnTagName As String = String.Empty
        Dim ReturnTagValue As String = String.Empty
        Dim xmlBPMTID As Integer = 0

        If xmlDocRead.ChildNodes.Count >= 1 Then

            Dim SelNodesTxt As String = xmlDocRead.DocumentElement.Name
            Dim Cnt As Integer = 0
            Dim nodes As XmlNodeList = xmlDocRead.SelectNodes(SelNodesTxt)
            For Each node As XmlNode In nodes
                Cnt += 1
                MainVar = String.Empty
                cntt = 0
                globalVar = String.Empty
                ChildVar = String.Empty
                xmldoctype = String.Empty
                xmlecode = String.Empty
                apikey = String.Empty
                ReturnTagName = String.Empty
                ReturnTagValue = String.Empty
                xmlBPMTID = 0
                Dim Key As String = "", EID As String = "", DocType As String = "", UID As String = 0
                For c As Integer = 0 To node.ChildNodes.Count - 1
                    'If UCase(node.ChildNodes.Item(c).Name) = "HEADER" Then
                    '    For C1 As Integer = 0 To node.Item("HEADER").ChildNodes.Count - 1
                    '        If node.Item("HEADER").ChildNodes.Item(C1).Name = "REQUESTID" Then
                    '            REQEUSTID = node.Item("HEADER").ChildNodes.Item(C1).InnerText.ToString()
                    '        End If
                    '    Next


                    'End If
                    If UCase(node.ChildNodes.Item(c).Name) = "BODY" Then

                        For C1 As Integer = 0 To node.Item("BODY").ChildNodes.Count - 1
                            If node.Item("BODY").ChildNodes.Item(C1).Name = "DATA" Then
                                For c2 As Integer = 0 To node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Count - 1
                                    If node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).Name = "VALIDATION" Then
                                        For P As Integer = 0 To node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Count - 1
                                            If UCase(node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).Name) = "FORMNAME" Then
                                                xmldoctype = node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).InnerText.ToString()
                                            ElseIf UCase(node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).Name) = "ENTITY" Then
                                                xmlecode = node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).InnerText.ToString()
                                                'ElseIf UCase(node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).Name) = "BPMTID" Then
                                                '    xmlBPMTID = Val(node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).InnerText)
                                            End If
                                        Next


                                        If Not String.IsNullOrEmpty(xmldoctype) And Not String.IsNullOrEmpty(xmlecode) Then
                                            da.SelectCommand.CommandText = "Select eid from mmm_mst_entity where code='" & xmlecode.ToString() & "'"
                                            da.SelectCommand.CommandType = CommandType.Text
                                            If con.State <> ConnectionState.Open Then
                                                con.Open()
                                            End If
                                            xmleid = Convert.ToInt32(da.SelectCommand.ExecuteScalar())

                                            da.SelectCommand.CommandText = "Select * from mmm_mst_fields where eid=" & xmleid & " and documenttype='" & xmldoctype.ToString() & "' order by displayname "
                                            da.SelectCommand.CommandType = CommandType.Text
                                            da.Fill(ds, "data")
                                        Else
                                            msg = msg & "<VALIDATION><RESULT>FORMNAME OR ENTITY IS INVALID</RESULT> <STATUS>FAILED</STATUS> </VALIDATION></DATA> </BODY></ENVELOPE>"
                                            Call InsertinTallyIntegrationLog(xmleid, xmlBPMTID, xmldoctype, "INWARDBULKMASTER", xmldoctype, TransCountforLog, "FAIL", msg)
                                            Return msg
                                            Exit Function
                                        End If
                                        'code to get API key of the entity
                                        If xmleid > 0 Then
                                            da.SelectCommand.CommandText = "Select apikey from mmm_mst_entity where eid=" & xmleid & ""
                                            da.Fill(ds, "apikey")
                                            If ds.Tables("apikey").Rows.Count > 0 Then
                                                apikey = ds.Tables("apikey").Rows(0).Item("apikey").ToString()
                                            End If

                                            da.SelectCommand.CommandText = "Select Tally_BulkInward_ReturnField from mmm_mst_forms where eid=" & xmleid & " and FormName='" & xmldoctype.ToString() & "'"
                                            da.SelectCommand.CommandType = CommandType.Text
                                            ReturnTagName = Convert.ToString(da.SelectCommand.ExecuteScalar())
                                        Else
                                            msg = msg & "<VALIDATION><RESULT>FORMNAME OR ENTITY IS INVALID</RESULT> <STATUS>FAILED</STATUS> </VALIDATION></DATA> </BODY></ENVELOPE>"
                                            Call InsertinTallyIntegrationLog(xmleid, xmlBPMTID, xmldoctype, "INWARDBULKMASTER", xmldoctype, TransCountforLog, "FAIL", msg)
                                            Return msg
                                            Exit Function
                                        End If

                                    End If

                                    If ReturnTagName = "" Then
                                        msg = msg & "<VALIDATION><RESULT>Return Tag Name not configuered in BPM</RESULT> <STATUS>Configuration Required</STATUS> </VALIDATION>" & msgclose.ToString()
                                        Call InsertinTallyIntegrationLog(xmleid, xmlBPMTID, xmldoctype, "INWARDBULKMASTER", xmldoctype, TransCountforLog, "FAIL", msg)
                                        Return msg
                                        Exit Function
                                    End If

                                    ' If cntt = 1 Then
                                    Dim rowno As Integer = 0
                                    If node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).Name = "REQUESTDATA" Then
                                        For P As Integer = 0 To node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Count - 1
                                            If UCase(node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).Name) = "ROW" Then
                                                For d As Integer = 0 To node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).ChildNodes.Count - 1
                                                    For s As Integer = 0 To ds.Tables("data").Rows.Count - 1
                                                        If UCase(ds.Tables("data").Rows(s).Item("inwardxmltagname").ToString()) = UCase(node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).ChildNodes.Item(d).Name) Then
                                                            ' new by sp for return tag from record with value - 04_apr_15
                                                            If UCase(node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).ChildNodes.Item(d).Name) = ReturnTagName Then
                                                                ReturnTagValue = node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).ChildNodes.Item(d).InnerText
                                                            End If
                                                            '' by sp
                                                            If UCase(ds.Tables("data").Rows(s).Item("fieldtype").ToString()) = "CHILD ITEM" Then
                                                                da.SelectCommand.CommandType = CommandType.Text
                                                                da.SelectCommand.CommandText = "Select * from mmm_mst_fields where eid=" & xmleid & " and documenttype='" & ds.Tables("data").Rows(s).Item("DROPDOWN").ToString().Trim() & "'"
                                                                da.Fill(ds, "child")
                                                                If (ChildVar.Trim = "") Then
                                                                    ChildVar = ds.Tables("data").Rows(s).Item("DROPDOWN").ToString().Trim() & "::{}"
                                                                End If
                                                                For x As Integer = 0 To ds.Tables("child").Rows.Count - 1
                                                                    For l As Integer = 0 To node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).ChildNodes.Item(d).ChildNodes.Count - 1
                                                                        If UCase(node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).ChildNodes.Item(d).ChildNodes.Item(l).Name).ToString() = UCase(ds.Tables("child").Rows(x).Item("inwardxmltagname").ToString()) Then
                                                                            ChildVar = ChildVar & "()" & UCase(ds.Tables("child").Rows(x).Item("displayname").ToString()) & "<>" & node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).ChildNodes.Item(d).ChildNodes.Item(l).InnerText
                                                                        End If
                                                                    Next
                                                                Next
                                                                ChildVar = ChildVar & "{}"
                                                            Else
                                                                globalVar = globalVar & UCase(ds.Tables("data").Rows(s).Item("displayname").ToString()) & "::" & node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).ChildNodes.Item(d).InnerText & "|"
                                                            End If

                                                        End If
                                                    Next

                                                Next
                                                If ChildVar.Length > 2 Then
                                                    ChildVar = ChildVar.Remove(ChildVar.Length - 2)
                                                End If
                                                Try
                                                    result = "Key$$" & apikey & "~DOCTYPE$$" & xmldoctype.ToString() & "~Data$$" & globalVar & ChildVar
                                                    Dim arrData As String() = Split(result, "~")
                                                    For i As Integer = 0 To arrData.Length - 1
                                                        Dim ar = Split(arrData(i), "$$")
                                                        If ar(0).ToUpper().Trim() = "KEY" Then
                                                            Key = ar(1)
                                                        ElseIf ar(0).ToUpper().Trim() = "DOCTYPE" Then
                                                            DocType = ar(1)
                                                        ElseIf ar(0).ToUpper().Trim() = "DATA" Then
                                                            result = ar(1)
                                                        End If
                                                    Next
                                                    Dim DsS As New DataSet()
                                                    DsS = AuthenticateWSRequest(Key)
                                                    If DsS.Tables(0).Rows.Count > 0 Then
                                                        EID = DsS.Tables(0).Rows(0).Item("EID")
                                                        UID = DsS.Tables(1).Rows(0).Item("uid")
                                                        result = CommanUtil.ValidateParameterByDocumentType(EID, DocType, UID, result)
                                                    Else
                                                        result = "Sorry!!! Authentication failed."
                                                    End If
                                                    CommanUtil.SaveServicerequest(Data1, "BPMCUSTOMWS", "InwardBULKMASTER", result)
                                                    'msg = msg & result
                                                    '' add here multiple records
                                                    If result.ToUpper().Contains("YOUR DOCID IS") = True Then
                                                        VALIDATION = VALIDATION & "<VALIDATION><BPMTID> " & Trim(Replace(result.ToUpper(), "YOUR DOCID IS", "")) & " </BPMTID><RESULT>Inserted Successfully</RESULT><STATUS>SUCCESS</STATUS> " & "<" & ReturnTagName & ">" & ReturnTagValue & "</" & ReturnTagName & "> </VALIDATION> "
                                                    Else
                                                        VALIDATION = VALIDATION & "<VALIDATION><BPMTID></BPMTID> <RESULT>" & result & "</RESULT> <STATUS>ERROR</STATUS> " & "<" & ReturnTagName & ">" & ReturnTagValue & "</" & ReturnTagName & "></VALIDATION>"
                                                    End If
                                                    '' add here for bulk ends
                                                    result = ""
                                                    globalVar = ""
                                                    ChildVar = ""
                                                Catch ex As Exception
                                                    ErrorLog.sendMail("BPMCUSTOMWS.InwardBulkMASTER", ex.Message)
                                                    result = ""
                                                    globalVar = ""
                                                    ChildVar = ""
                                                    msg = msg & "<VALIDATION><RESULT>UNEXPECTED ERROR OCCURRED, KINDLY CONTACT ADMINISTRATOR</RESULT> <STATUS>ERROR</STATUS>" & "<" & ReturnTagName & ">" & ReturnTagValue & "</" & ReturnTagName & "> </VALIDATION> "
                                                End Try
                                                TransCountforLog = TransCountforLog + 1
                                            End If
                                        Next
                                        msg = msg & VALIDATION & msgclose
                                    End If

                                Next
                            End If
                        Next
                    End If
                Next
            Next
        End If

        If msg.ToUpper().Contains("YOUR DOCID IS") Then
            Call InsertinTallyIntegrationLog(xmleid, xmlBPMTID, xmldoctype, "INWARDBULKMASTER", xmldoctype, TransCountforLog, "SUCCESS", msg)
        Else
            Call InsertinTallyIntegrationLog(xmleid, xmlBPMTID, xmldoctype, "INWARDBULKMASTER", xmldoctype, TransCountforLog, "FAIL", msg)
        End If
        Return msg
    End Function

    Function INWARDBULKALL(Data As Stream) As XElement Implements IBPMCustomWS.INWARDBULKALL
        Dim Result = ""
        Dim strr As String = ""
        Dim Key As String = "", EID As String = "", DocType As String = "", UID As String = 0
        Try
            'Convert steam into string 
            Dim reader As New StreamReader(Data)
            Dim strData As String = reader.ReadToEnd()
            'Decode it to UTF-8
            strData = HttpUtility.UrlDecode(strData)
            Dim Data1 As New StringBuilder(strData)

            strr = InsertInward_BulkALL(strData)

            If strr.Length > 5 Then
                strr = strr.Replace("&", "&amp;")
            End If

            CommanUtil.SaveServicerequest(Data1, "BPMCUSTOMWS", "INWARDBULKALL", strr)

        Catch ex As Exception
            ErrorLog.sendMail("BPMCUSTOMWS.INWARDBULKALL", ex.Message)
            'Return "RTO"
        End Try
        Dim xmldoc As XDocument = XDocument.Parse(strr)
        Return xmldoc.Root


    End Function


    Public Function InsertInward_BulkALL(ByVal str As String) As String
        Dim result As String = String.Empty
        Dim msg As String = ""
        Dim msgOPEN As String = "<ENVELOPE> <HEADER><TALLYREQUEST>EXPORT VOUCHERS</TALLYREQUEST></HEADER> <BODY>  <DATA>"
        Dim msgCLOSE As String = "</DATA></BODY></ENVELOPE>"

        Dim xmlDocRead As New XmlDocument()
        xmlDocRead.LoadXml(str)
        Dim TransCountforLog As Integer = 0
        Dim REQEUSTID As String = ""
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim da As New SqlDataAdapter("", con)
        Dim ds As New DataSet

        Dim Data1 As New StringBuilder(str)
        Dim xmleid As Integer = 0

        Dim MainVar As String = String.Empty
        Dim cntt As Integer = 0
        Dim globalVar As String = String.Empty
        Dim ChildVar As String = String.Empty
        Dim xmldoctype As String = String.Empty
        Dim xmlecode As String = String.Empty
        Dim apikey As String = String.Empty
        Dim ReturnTagName As String = String.Empty
        Dim ReturnTagValue As String = String.Empty
        Dim xmlBPMTID As Integer = 0

        If xmlDocRead.ChildNodes.Count >= 1 Then

            Dim SelNodesTxt As String = xmlDocRead.DocumentElement.Name
            Dim Cnt As Integer = 0
            Dim nodes As XmlNodeList = xmlDocRead.SelectNodes(SelNodesTxt)
            For Each node As XmlNode In nodes
                Cnt += 1
                MainVar = String.Empty
                cntt = 0
                globalVar = String.Empty
                ChildVar = String.Empty
                xmldoctype = String.Empty
                xmlecode = String.Empty
                apikey = String.Empty
                ReturnTagName = String.Empty
                ReturnTagValue = String.Empty
                xmlBPMTID = 0
                Dim Key As String = "", EID As String = "", DocType As String = "", UID As String = 0
                For c As Integer = 0 To node.ChildNodes.Count - 1

                    If UCase(node.ChildNodes.Item(c).Name) = "ENVELOPE" Then
                        For vl As Integer = 0 To node.ChildNodes.Item(c).ChildNodes.Count - 1

                            If UCase(node.ChildNodes.Item(c).ChildNodes.Item(vl).Name) = "HEADER" Then
                                For C1 As Integer = 0 To node.ChildNodes.Item(c).ChildNodes.Item(vl).ChildNodes.Count - 1

                                    If node.ChildNodes.Item(c).ChildNodes.Item(vl).ChildNodes.Item(C1).Name = "REQUESTID" Then
                                        REQEUSTID = node.ChildNodes.Item(c).ChildNodes.Item(vl).ChildNodes.Item(C1).InnerText.ToString()
                                    End If


                                Next
                            End If
                            If UCase(node.ChildNodes.Item(c).ChildNodes.Item(vl).Name) = "BODY" Then
                                For C1 As Integer = 0 To node.ChildNodes.Item(c).ChildNodes.Item(vl).ChildNodes.Count - 1
                                    If UCase(node.ChildNodes.Item(c).ChildNodes.Item(vl).ChildNodes.Item(C1).Name) = "DATA" Then
                                        For c2 As Integer = 0 To node.ChildNodes.Item(c).ChildNodes.Item(vl).ChildNodes.Item(C1).ChildNodes.Count - 1
                                            If node.ChildNodes.Item(c).ChildNodes.Item(vl).ChildNodes.Item(C1).ChildNodes.Item(c2).Name = "VALIDATION" Then
                                                For P As Integer = 0 To node.ChildNodes.Item(c).ChildNodes.Item(vl).ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Count - 1
                                                    If UCase(node.ChildNodes.Item(c).ChildNodes.Item(vl).ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).Name) = "FORMNAME" Then
                                                        xmldoctype = node.ChildNodes.Item(c).ChildNodes.Item(vl).ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).InnerText.ToString()
                                                    ElseIf UCase(node.ChildNodes.Item(c).ChildNodes.Item(vl).ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).Name) = "ENTITY" Then
                                                        xmlecode = node.ChildNodes.Item(c).ChildNodes.Item(vl).ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).InnerText.ToString()
                                                    ElseIf UCase(node.ChildNodes.Item(c).ChildNodes.Item(vl).ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).Name) = "BPMTID" Then
                                                        xmlBPMTID = Val(node.ChildNodes.Item(c).ChildNodes.Item(vl).ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).InnerText)
                                                    End If
                                                Next

                                                If Not String.IsNullOrEmpty(xmlBPMTID) Then
                                                    Dim cmd As New SqlCommand("select isnull(TallyIsActive,0)[Count] from mmm_mst_master where tid =" & xmlBPMTID & " ", con)

                                                    If con.State <> ConnectionState.Open Then
                                                        con.Open()
                                                    End If
                                                    cntt = Convert.ToInt32(cmd.ExecuteScalar())

                                                    If cntt = 1 Then
                                                        If Not String.IsNullOrEmpty(xmldoctype) And Not String.IsNullOrEmpty(xmlecode) Then
                                                            da.SelectCommand.CommandText = "Select eid from mmm_mst_entity where code='" & xmlecode.ToString() & "'"
                                                            da.SelectCommand.CommandType = CommandType.Text
                                                            If con.State <> ConnectionState.Open Then
                                                                con.Open()
                                                            End If
                                                            xmleid = Convert.ToInt32(da.SelectCommand.ExecuteScalar())

                                                            da.SelectCommand.CommandText = "Select * from mmm_mst_fields where eid=" & xmleid & " and documenttype='" & xmldoctype.ToString() & "' order by displayname "
                                                            da.SelectCommand.CommandType = CommandType.Text
                                                            da.Fill(ds, "data")
                                                        End If
                                                        'code to get API key of the entity
                                                        If xmleid > 0 Then
                                                            da.SelectCommand.CommandText = "Select apikey from mmm_mst_entity where eid=" & xmleid & ""
                                                            da.Fill(ds, "apikey")
                                                            If ds.Tables("apikey").Rows.Count > 0 Then
                                                                apikey = ds.Tables("apikey").Rows(0).Item("apikey").ToString()
                                                            End If

                                                            da.SelectCommand.CommandText = "Select Tally_BulkInward_ReturnField from mmm_mst_forms where eid=" & xmleid & " and FormName='" & xmldoctype.ToString() & "'"
                                                            da.SelectCommand.CommandType = CommandType.Text
                                                            ReturnTagName = Convert.ToString(da.SelectCommand.ExecuteScalar())
                                                        End If
                                                    Else
                                                        msg = msg & "<VALIDATION><REQUESTID>" & REQEUSTID.ToString() & "</REQUESTID><BPMTID></BPMTID><RESULT>BPMTID IN INVALID in BPM</RESULT> <STATUS>ERROR</STATUS> </VALIDATION>"
                                                        Call InsertinTallyIntegrationLog(xmleid, xmlBPMTID, xmldoctype, "INWARDBULKALL", xmldoctype, TransCountforLog, "FAIL", msg)
                                                        ' Return msg
                                                        Exit For
                                                    End If
                                                End If
                                            End If

                                            If ReturnTagName = "" Then
                                                msg = msg & "<VALIDATION><REQUESTID>" & REQEUSTID.ToString() & "</REQUESTID><BPMTID></BPMTID><RESULT>Return Tag Name not configuered in BPM</RESULT> <STATUS>ERROR</STATUS></VALIDATION>"
                                                Call InsertinTallyIntegrationLog(xmleid, xmlBPMTID, xmldoctype, "INWARDBULKD", xmldoctype, TransCountforLog, "FAIL", msg)
                                                '     Return msg
                                                Exit For
                                                '     Exit Function
                                            End If

                                            If cntt = 1 Then
                                                Dim rowno As Integer = 0
                                                If node.ChildNodes.Item(c).ChildNodes.Item(vl).ChildNodes.Item(C1).ChildNodes.Item(c2).Name = "IMPORTDATA" Then
                                                    For P As Integer = 0 To node.ChildNodes.Item(c).ChildNodes.Item(vl).ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Count - 1
                                                        If UCase(node.ChildNodes.Item(c).ChildNodes.Item(vl).ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).Name) = "TALLYMESSAGE" Then
                                                            For d As Integer = 0 To node.ChildNodes.Item(c).ChildNodes.Item(vl).ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).ChildNodes.Count - 1
                                                                For s As Integer = 0 To ds.Tables("data").Rows.Count - 1
                                                                    If UCase(ds.Tables("data").Rows(s).Item("inwardxmltagname").ToString()) = UCase(node.ChildNodes.Item(c).ChildNodes.Item(vl).ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).ChildNodes.Item(d).Name) Then
                                                                        ' new by sp for return tag from record with value - 04_apr_15
                                                                        If UCase(node.ChildNodes.Item(c).ChildNodes.Item(vl).ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).ChildNodes.Item(d).Name) = ReturnTagName Then
                                                                            ReturnTagValue = node.ChildNodes.Item(c).ChildNodes.Item(vl).ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).ChildNodes.Item(d).InnerText
                                                                        End If
                                                                        '' by sp
                                                                        If UCase(ds.Tables("data").Rows(s).Item("fieldtype").ToString()) = "CHILD ITEM" Then
                                                                            da.SelectCommand.CommandType = CommandType.Text
                                                                            da.SelectCommand.CommandText = "Select * from mmm_mst_fields where eid=" & xmleid & " and documenttype='" & ds.Tables("data").Rows(s).Item("DROPDOWN").ToString().Trim() & "'"
                                                                            da.Fill(ds, "child")
                                                                            If (ChildVar.Trim = "") Then
                                                                                ChildVar = ds.Tables("data").Rows(s).Item("DROPDOWN").ToString().Trim() & "::{}"
                                                                            End If
                                                                            For x As Integer = 0 To ds.Tables("child").Rows.Count - 1
                                                                                For l As Integer = 0 To node.ChildNodes.Item(c).ChildNodes.Item(vl).ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).ChildNodes.Item(d).ChildNodes.Count - 1
                                                                                    If UCase(node.ChildNodes.Item(c).ChildNodes.Item(vl).ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).ChildNodes.Item(d).ChildNodes.Item(l).Name).ToString() = UCase(ds.Tables("child").Rows(x).Item("inwardxmltagname").ToString()) Then
                                                                                        ChildVar = ChildVar & "()" & UCase(ds.Tables("child").Rows(x).Item("displayname").ToString()) & "<>" & node.ChildNodes.Item(c).ChildNodes.Item(vl).ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).ChildNodes.Item(d).ChildNodes.Item(l).InnerText
                                                                                    End If
                                                                                Next
                                                                            Next
                                                                            ChildVar = ChildVar & "{}"
                                                                        Else
                                                                            globalVar = globalVar & UCase(ds.Tables("data").Rows(s).Item("displayname").ToString()) & "::" & node.ChildNodes.Item(c).ChildNodes.Item(vl).ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).ChildNodes.Item(d).InnerText & "|"
                                                                        End If

                                                                    End If
                                                                Next

                                                            Next
                                                            If ChildVar.Length > 2 Then
                                                                ChildVar = ChildVar.Remove(ChildVar.Length - 2)
                                                            End If
                                                            Try
                                                                result = "Key$$" & apikey & "~DOCTYPE$$" & xmldoctype.ToString() & "~Data$$" & globalVar & ChildVar
                                                                Dim arrData As String() = Split(result, "~")
                                                                For i As Integer = 0 To arrData.Length - 1
                                                                    Dim ar = Split(arrData(i), "$$")
                                                                    If ar(0).ToUpper().Trim() = "KEY" Then
                                                                        Key = ar(1)
                                                                    ElseIf ar(0).ToUpper().Trim() = "DOCTYPE" Then
                                                                        DocType = ar(1)
                                                                    ElseIf ar(0).ToUpper().Trim() = "DATA" Then
                                                                        result = ar(1)
                                                                    End If
                                                                Next
                                                                Dim DsS As New DataSet()
                                                                DsS = AuthenticateWSRequest(Key)
                                                                If DsS.Tables(0).Rows.Count > 0 Then
                                                                    EID = DsS.Tables(0).Rows(0).Item("EID")
                                                                    UID = DsS.Tables(1).Rows(0).Item("uid")
                                                                    result = CommanUtil.ValidateParameterByDocumentType(EID, DocType, UID, result)
                                                                Else
                                                                    result = "Sorry!!! Authentication failed."
                                                                    Call InsertinTallyIntegrationLog(xmleid, xmlBPMTID, xmldoctype, "INWARDBULKALL", xmldoctype, TransCountforLog, "FAIL", msg)
                                                                    'CommanUtil.SaveServicerequest(Data1, "BPMTallyWS", "INWARDBULKALL", result)
                                                                End If
                                                                '                 
                                                                'msg = msg & result
                                                                '' add here multiple records
                                                                If result.ToUpper().Contains("YOUR DOCID IS") = True Then
                                                                    msg = msg & "<VALIDATION><REQUESTID>" & REQEUSTID.ToString() & "</REQUESTID><BPMTID>" & Trim(Replace(result.ToUpper(), "YOUR DOCID IS", "")) & "</BPMTID><RESULT>CREATED SUCCCESSFULLY </RESULT> <STATUS>SUCCESS</STATUS> " & "<" & ReturnTagName & ">" & ReturnTagValue & "</" & ReturnTagName & ">  </VALIDATION>"
                                                                Else
                                                                    msg = msg & "<VALIDATION><REQUESTID>" & REQEUSTID.ToString() & "</REQUESTID><BPMTID></BPMTID><RESULT>" & result & "</RESULT> <STATUS>ERROR</STATUS> " & "<" & ReturnTagName & ">" & ReturnTagValue & "</" & ReturnTagName & ">  </VALIDATION>"
                                                                End If
                                                                '' add here for bulk ends
                                                                result = ""
                                                                globalVar = ""
                                                                ChildVar = ""
                                                            Catch ex As Exception
                                                                ErrorLog.sendMail("BPMCUSTOMWS.INWARDBULKALL", ex.Message)
                                                                result = ""
                                                                globalVar = ""
                                                                ChildVar = ""
                                                                msg = msg & "<VALIDATION><REQUESTID>" & REQEUSTID.ToString() & "</REQUESTID><BPMTID></BPMTID><RESULT>ERROR</RESULT> <STATUS>ERROR</STATUS>" & "<" & ReturnTagName & ">" & ReturnTagValue & "</" & ReturnTagName & "> </VALIDATION> "
                                                            End Try
                                                            TransCountforLog = TransCountforLog + 1
                                                        End If
                                                    Next
                                                    ' msg = msg & "</DATA> </BODY></ENVELOPE>"
                                                End If
                                            Else
                                                msg = msg & "<VALIDATION><REQUESTID>" & REQEUSTID.ToString() & "</REQUESTID><BPMTID></BPMTID><RESULT>YOU ARE NOT AUTHORIZED</RESULT> <STATUS>ERROR</STATUS> </VALIDATION>"
                                                Exit For
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
        msg = msgOPEN & msg & msgCLOSE
        If msg.ToUpper().Contains("YOUR DOCID IS") Then
            Call InsertinTallyIntegrationLog(xmleid, xmlBPMTID, xmldoctype, "INWARDBULKALL", xmldoctype, TransCountforLog, "SUCCESS", msg)
        Else
            Call InsertinTallyIntegrationLog(xmleid, xmlBPMTID, xmldoctype, "INWARDBULKALL", xmldoctype, TransCountforLog, "FAIL", msg)
        End If
        Return msg
    End Function




    '  code ends here




    'Function InwardPAL(Data As Stream) As String Implements IBPMCustomWS.InwardPAL
    '    Dim Result = ""
    '    Dim Key As String = "", EID As String = "", DocType As String = "", UID As String = 0
    '    Try
    '        'Convert steam into string 
    '        Dim reader As New StreamReader(Data)
    '        Dim strData As String = reader.ReadToEnd()
    '        'Decode it to UTF-8
    '        strData = HttpUtility.UrlDecode(strData)
    '        Dim Data1 As New StringBuilder(strData)
    '        'Save string into database
    '        'SaveServicerequest
    '        Dim strr As String = readxmlandgivestring(strData)
    '        Dim arrData As String() = Split(strr, "~")
    '        For i As Integer = 0 To arrData.Length - 1
    '            Dim ar = Split(arrData(i), "$$")
    '            If ar(0).ToUpper().Trim() = "KEY" Then
    '                Key = ar(1)
    '            ElseIf ar(0).ToUpper().Trim() = "DOCTYPE" Then
    '                DocType = ar(1)
    '            ElseIf ar(0).ToUpper().Trim() = "DATA" Then
    '                strr = ar(1)
    '            End If
    '        Next
    '        Dim DS As New DataSet()
    '        DS = AuthenticateWSRequest(Key)
    '        If DS.Tables(0).Rows.Count > 0 Then
    '            EID = DS.Tables(0).Rows(0).Item("EID")
    '            UID = DS.Tables(1).Rows(0).Item("uid")
    '            Result = CommanUtil.ValidateParameterByDocumentType(EID, DocType, UID, strr)
    '        Else
    '            Result = "Sorry!!! Authentication failed."
    '        End If
    '        CommanUtil.SaveServicerequest(Data1, "BPMCustomWS", "InwardPal", Result)
    '    Catch ex As Exception
    '        ErrorLog.sendMail("BPMCustomWS.InwardPal", ex.Message)
    '        Return "RTO"
    '    End Try
    '    Return Result
    'End Function
    'Function OutwardPAL(Data As Stream) As ENVELOPE Implements IBPMCustomWS.OutwardPAL
    '    Dim Result As New ENVELOPE()
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
    '        If strinputArr.Length <> 2 Then
    '            ErrorLog.sendMail("BPMCustomWS.OutwardPAL", "INPUT PARAMETERS NOT CORRECT")
    '        Else
    '            Result = POrder(strinputArr(0).ToString, strinputArr(1).ToString)
    '        End If
    '        CommanUtil.SaveServicerequest(Data1, "BPMCustomWS", "OutwardPal", "")
    '    Catch ex As Exception
    '        ErrorLog.sendMail("BPMCustomWS.OutwardPAL", ex.Message)
    '    End Try
    '    Return Result
    'End Function

    'Function REGISTER(Data As Stream) As XElement Implements IBPMCustomWS.REGISTER
    '    Dim Result = ""

    '    Try
    '        Dim reader As New StreamReader(Data)
    '        Dim strData As String = reader.ReadToEnd()
    '        strData = HttpUtility.UrlDecode(strData)
    '        Dim Data1 As New StringBuilder(strData)
    '        Result = readxmlandgivestringREGISTRATION(strData)
    '        If Result.Length > 5 Then
    '            Result = Result.Replace("&", "&amp;")
    '        End If
    '        CommanUtil.SaveServicerequest(Data1, "BPMCustomWs", "REGISTER", Result)
    '    Catch ex As Exception
    '        ErrorLog.sendMail("BPMCustomWs.REGISTER", ex.Message)
    '    End Try
    '    Dim xmldoc As XDocument = XDocument.Parse(Result)
    '    Return xmldoc.Root
    'End Function

    'Public Function readxmlandgivestringREGISTRATION(ByRef str As String) As String
    '    Dim result As String = String.Empty
    '    Dim msg As String = String.Empty
    '    Dim xmlDocRead As New XmlDocument()
    '    xmlDocRead.LoadXml(str)
    '    Dim xmldoctype As String = String.Empty
    '    Dim xmlecode As String = String.Empty
    '    Dim xmlfldname As String = String.Empty
    '    Dim xmlRegvalue As String = String.Empty
    '    Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
    '    Dim con As New SqlConnection(conStr)
    '    Dim da As New SqlDataAdapter("", con)
    '    Dim ds As New DataSet

    '    If xmlDocRead.ChildNodes.Count >= 1 Then

    '        Dim SelNodesTxt As String = xmlDocRead.DocumentElement.Name
    '        Dim Cnt As Integer = 0
    '        Dim nodes As XmlNodeList = xmlDocRead.SelectNodes(SelNodesTxt)
    '        For Each node As XmlNode In nodes
    '            Cnt += 1
    '            Dim globalVar As String = String.Empty
    '            Dim ChildVar As String = String.Empty

    '            Dim apikey As String = String.Empty
    '            Dim Key As String = "", EID As String = "", DocType As String = "", UID As String = 0
    '            For c As Integer = 0 To node.ChildNodes.Count - 1
    '                If UCase(node.ChildNodes.Item(c).Name) = "BODY" Then
    '                    For C1 As Integer = 0 To node.Item("BODY").ChildNodes.Count - 1
    '                        If node.Item("BODY").ChildNodes.Item(C1).Name = "DATA" Then
    '                            For c2 As Integer = 0 To node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Count - 1
    '                                If node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).Name = "VALIDATION" Then
    '                                    For P As Integer = 0 To node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Count - 1
    '                                        If UCase(node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).Name) = "FORMNAME" Then
    '                                            xmldoctype = node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).InnerText.ToString()
    '                                        ElseIf UCase(node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).Name) = "REGVALUE" Then
    '                                            xmlRegvalue = node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).InnerText.ToString()
    '                                        ElseIf UCase(node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).Name) = "ENTITY" Then
    '                                            xmlecode = node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).InnerText.ToString()
    '                                        End If
    '                                    Next
    '                                End If
    '                            Next
    '                        End If
    '                    Next
    '                End If
    '            Next

    '        Next

    '        If Not String.IsNullOrEmpty(xmldoctype) And Not String.IsNullOrEmpty(xmlecode) And Not String.IsNullOrEmpty(xmlRegvalue) Then
    '            'getting EID 
    '            da.SelectCommand.CommandText = "Select eid from mmm_mst_entity where code='" & xmlecode.ToString() & "'"
    '            If con.State <> ConnectionState.Open Then
    '                con.Open()
    '            End If
    '            Dim eid As Integer = Val(da.SelectCommand.ExecuteScalar())

    '            'getting Tally Registration FIeld from FORM Master
    '            da.SelectCommand.CommandText = "select TallyRegField from mmm_mst_forms where eid=" & Val(eid) & " and formname ='" & xmldoctype.ToString() & "'"
    '            xmlfldname = da.SelectCommand.ExecuteScalar().ToString()

    '            Dim StrQuery As String = "SELECT tid from  v" & eid & xmldoctype.Trim.Replace(" ", "_") & " where  v" & eid & xmldoctype.Trim.Replace(" ", "_") & ".[" & xmlfldname.ToString() & "] = '" & xmlRegvalue.ToString() & "'"
    '            da.SelectCommand.CommandText = StrQuery

    '            result = da.SelectCommand.ExecuteScalar().ToString()
    '        End If
    '    End If
    '    Dim sb As New StringBuilder
    '    sb.Append("<ENVELOPE>")
    '    sb.Append("<HEADER>")
    '    sb.Append("<TALLYREQUEST>")
    '    sb.Append("REGISTRATION")
    '    sb.Append("</TALLYREQUEST>")
    '    sb.Append("</HEADER>")
    '    sb.Append("<BODY>")

    '    sb.Append("<DATA>")
    '    sb.Append("<VALIDATION>")
    '    sb.Append("<FORMNAME>")
    '    sb.Append(xmldoctype)
    '    sb.Append("</FORMNAME>")
    '    sb.Append("<ENTITY>")
    '    sb.Append(xmlecode)
    '    sb.Append("</ENTITY>")
    '    sb.Append("<REGVALUE>")
    '    sb.Append(xmlRegvalue)
    '    sb.Append("</REGVALUE>")
    '    sb.Append("<BPMTID>")
    '    If Not String.IsNullOrEmpty(result) Then
    '        sb.Append(result)
    '    Else
    '        sb.Append("NOT FOUND")
    '    End If
    '    sb.Append("</BPMTID>")
    '    sb.Append("</VALIDATION>")
    '    sb.Append("</DATA>")
    '    sb.Append("</BODY>")
    '    sb.Append("</ENVELOPE>")
    '    Return sb.ToString()

    'End Function


    ''Function OutwardPAL(Data As Stream) As XmlDocument Implements IBPMCustomWS.OutwardPAL
    ''    Dim Result As String = ""
    ''    Dim resXML As New XmlDocument
    ''    Dim Key As String = "", EID As String = "", DocType As String = "", UID As String = 0
    ''    Try
    ''        '' here code to read xml for doc type and field name and create xml output string and retun xml string
    ''        'Convert steam into string 
    ''        Dim reader As New StreamReader(Data)
    ''        Dim strData As String = reader.ReadToEnd()
    ''        'Decode it to UTF-8
    ''        strData = HttpUtility.UrlDecode(strData)
    ''        Dim Data1 As New StringBuilder(strData)
    ''        'Save string into database
    ''        'SaveServicerequest

    ''        Dim strinput As String = ReturnInputParaValues(strData)

    ''        Dim strinputArr() As String = strinput.Split("|")

    ''        Result = "<ERROR>Not Called</ERROR>"
    ''        If strinputArr.Length <> 2 Then
    ''            ErrorLog.sendMail("BPMCustomWS.OutwardPAL", "INPUT PARAMETERS NOT CORRECT")
    ''        Else
    ''            Result = POrder(strinputArr(0).ToString, strinputArr(1).ToString)
    ''        End If

    ''        resXML.LoadXml(Result)
    ''        CommanUtil.SaveServicerequest(Data1, "BPMCustomWS", "OutwardPal", Result)
    ''    Catch ex As Exception
    ''        ErrorLog.sendMail("BPMCustomWS.OutwardPAL", ex.Message)
    ''        resXML.LoadXml("<ERROR>RTO</ERROR>")
    ''        Return resXML
    ''    End Try
    ''    Return resXML
    ''End Function
    'Function POrder(ByVal Dtype As String, ByVal DistCode As String) As ENVELOPE
    '    ' Dtype = "Purchase Document"
    '    ' DistCode = "0001"
    '   
    '    Dim con As SqlConnection = New SqlConnection(conStr)
    '    Dim oda As SqlDataAdapter = New SqlDataAdapter("InsertSMSALERTLog", con)
    '    Dim doc As XmlDocument = New XmlDocument()
    '    Dim Vdt As New DataTable
    '    Dim SITdt As New DataTable
    '    Dim Invtrdt As New DataTable
    '    Dim ds As New DataSet
    '    oda.SelectCommand.CommandText = "select tid[DocID],dms.udf_split('MASTER-Distributor-fld1',fld10)[Distributor Name],fld11[Distributor Code],fld12[STATE],replace(fld13,'/','-')[BILLDATE],fld14[SupplierInvoiceNo],fld15[VENDORNAME],fld16[VENDORADDRESS],fld17[VENDORTINNO.],isnull(fld18,0)[GROSSAMOUNT],isnull(fld2,0)[TAX],isnull(fld21,0)[TOTALBILLAMOUNT] from mmm_mst_doc where eid=43 and documenttype='" & Dtype & "' and fld11='" & DistCode & "' and isExport=1"
    '    oda.Fill(Vdt)

    '    'If Vdt.Rows.Count < 1 Then
    '    '    Return "Data Not Found"
    '    'End If
    '    'Dim root As XmlElement
    '    'root = doc.CreateElement("ROOT")
    '    'doc.AppendChild(root)
    '    'Dim strB As StringBuilder = New System.Text.StringBuilder()
    '    'strB.Append("<ENVELOPE>")
    '    'strB.Append("<HEADER>")
    '    'strB.Append("<RESPONSE>" & "IMPORT DATA")
    '    'strB.Append("</RESPONSE>")
    '    'strB.Append("</HEADER>")
    '    'strB.Append("<BODY>")
    '    'strB.Append("<DATA>")
    '    'strB.Append("<VALIDATION>")
    '    'strB.Append("<BILLINGCODE>" & "IN013205004B")
    '    'strB.Append("</BILLINGCODE>")
    '    'strB.Append("<PASSWD>" & "XXXXX")
    '    'strB.Append("</PASSWD>")
    '    'strB.Append("</VALIDATION>")
    '    'strB.Append("<IMPORTDATA>")
    '    Dim docid As String = ""
    '    Dim ret As New ENVELOPE()
    '    Dim Header As New HEADER()
    '    Dim BODY As New BODY()
    '    Dim DATA As New DATA1()
    '    Dim VALIDATION As New VALIDATION()
    '    VALIDATION.BILLINGCODE = "IN013205004B"
    '    VALIDATION.PASSWD = "XXXXX"
    '    DATA.VALIDATION = VALIDATION
    '    Header.RESPONSE = "IMPORT DATA"
    '    ret.HEADER = Header

    '    Dim IMPORTDATA As New IMPORTDATA()

    '    Dim TALLYMESSAGE As New TALLYMESSAGE()



    '    Dim STOCKITEM As New STOCKITEM()
    '    Dim LSTSTOCKITEM As New List(Of STOCKITEM)

    '    Dim INVENTORYENTRIES As New INVENTORYENTRIES()
    '    Dim LSTINVENTORYENTRIES As New List(Of INVENTORYENTRIES)

    '    Dim VOUCHER As New VOUCHER()
    '    Dim LSTVOUCHER As New List(Of VOUCHER)

    '    For i = 0 To Vdt.Rows.Count - 1
    '        LSTSTOCKITEM = New List(Of STOCKITEM)()
    '        LSTINVENTORYENTRIES = New List(Of INVENTORYENTRIES)
    '        SITdt = New DataTable
    '        Invtrdt = New DataTable
    '        oda.SelectCommand.CommandText = "select dms.udf_split('MASTER-Item-fld1',dt.fld1)[Item],m.fld10[Item_Code-SKU],m.fld25[Batch],dms.udf_split('MASTER-ItemGroup1-fld1',m.fld12)[Product Group],dms.udf_split('MASTER-Units-fld1',m.fld16)[Primary UOM] from mmm_mst_doc_item dt inner join mmm_mst_master m on m.tid=dt.fld1 where docid=" & Vdt.Rows(i).Item(0).ToString & ""
    '        oda.Fill(SITdt)
    '        oda.SelectCommand.CommandText = "select dms.udf_split('MASTER-Item-fld1',dt.fld1)[ITEMNAME],dt.fld10[BATCHNAME],dt.fld11[Quantity],dms.udf_split('MASTER-Units-fld1',dt.fld15)[UOM],dt.fld12[RATE],dt.fld13[Product Discount],dt.fld14[Amount] from mmm_mst_doc_item dt where docid=" & Vdt.Rows(i).Item(0).ToString & ""
    '        oda.Fill(Invtrdt)
    '        VOUCHER = New VOUCHER()
    '        'select tid[DocID],dms.udf_split('MASTER-Distributor-fld1',fld10)[Distributor Name],fld11[Distributor Code],fld12[STATE],replace(fld13,'/','-')[BILLDATE],fld14[SupplierInvoiceNo],fld15[VENDORNAME],fld16[VENDORADDRESS],fld17[VENDORTINNO.],isnull(fld18,0)[GROSSAMOUNT],isnull(fld2,0)[TAX],isnull(fld21,0)[TOTALBILLAMOUNT]
    '        VOUCHER.DISTRIBUTORNAME = Vdt.Rows(i).Item("Distributor Name")
    '        VOUCHER.DISTRIBUTORCODE = Vdt.Rows(i).Item("Distributor Code")
    '        VOUCHER.STATE = Vdt.Rows(i).Item("STATE")
    '        VOUCHER.BILLDATE = Vdt.Rows(i).Item("BILLDATE")
    '        VOUCHER.SUPPLIERINVOICENO = Vdt.Rows(i).Item("SupplierInvoiceNo")
    '        VOUCHER.VENDORNAME = Vdt.Rows(i).Item("VENDORNAME")
    '        VOUCHER.VENDORADDRESS = Vdt.Rows(i).Item("VENDORADDRESS")
    '        VOUCHER.VENDORTINNO = Vdt.Rows(i).Item("VENDORTINNO.")
    '        VOUCHER.GROSSAMOUNT = Vdt.Rows(i).Item("GROSSAMOUNT")
    '        VOUCHER.TAX = Vdt.Rows(i).Item("TAX")
    '        VOUCHER.TOTALBILLAMOUNT = Vdt.Rows(i).Item("TOTALBILLAMOUNT")
    '        'Creating Stock Item
    '        For k = 0 To SITdt.Rows.Count - 1
    '            'select dms.udf_split('MASTER-Item-fld1',dt.fld1)[Item],m.fld10[Item_Code-SKU],m.fld25[Batch],dms.udf_split('MASTER-ItemGroup1-fld1',
    '            'm.fld12)[Product Group],dms.udf_split('MASTER-Units-fld1',m.fld16)[Primary UOM]
    '            STOCKITEM = New STOCKITEM()
    '            STOCKITEM.BATCH = SITdt.Rows(k).Item("Batch")
    '            STOCKITEM.ITEM = SITdt.Rows(k).Item("Item")
    '            STOCKITEM.ITEM_CODE_SKU = SITdt.Rows(k).Item("Item_Code-SKU")
    '            STOCKITEM.PRIMARYUOM = SITdt.Rows(k).Item("Primary UOM")
    '            STOCKITEM.PRODUCTGROUP = SITdt.Rows(k).Item("Product Group")
    '            LSTSTOCKITEM.Add(STOCKITEM)
    '        Next
    '        VOUCHER.STOCKITEMS = LSTSTOCKITEM
    '        For d = 0 To Invtrdt.Rows.Count - 1
    '            INVENTORYENTRIES = New INVENTORYENTRIES()
    '            INVENTORYENTRIES.AMOUNT = Invtrdt.Rows(d).Item("Amount")
    '            INVENTORYENTRIES.BATCHNAME = Invtrdt.Rows(d).Item("BATCHNAME")
    '            INVENTORYENTRIES.ITEMNAME = Invtrdt.Rows(d).Item("ITEMNAME")
    '            INVENTORYENTRIES.PRODUCTDISCOUNT = Invtrdt.Rows(d).Item("Product Discount")
    '            INVENTORYENTRIES.QUANTITY = Invtrdt.Rows(d).Item("Quantity")
    '            INVENTORYENTRIES.RATE = Invtrdt.Rows(d).Item("RATE")
    '            INVENTORYENTRIES.UOM = Invtrdt.Rows(d).Item("UOM")
    '            LSTINVENTORYENTRIES.Add(INVENTORYENTRIES)
    '        Next
    '        VOUCHER.INVENTORYENTRIES = LSTINVENTORYENTRIES
    '        LSTVOUCHER.Add(VOUCHER)
    '        docid = docid & Vdt.Rows(i).Item(0).ToString & ","
    '    Next
    '    TALLYMESSAGE.VOUCHER = LSTVOUCHER
    '    IMPORTDATA.TALLYMESSAGE = TALLYMESSAGE
    '    DATA.IMPORTDATA = IMPORTDATA
    '    BODY.DATA = DATA
    '    ret.HEADER = Header
    '    ret.BODY = BODY
    '    If docid <> "" Then
    '        docid = Left(docid, docid.Length - 1)
    '    End If
    '    '   oda.SelectCommand.CommandText = "update mmm_mst_doc set isexport=0 where eid=43 and documenttype='" & Dtype & "' and tid in (" & docid & ")"
    '    '  If con.State <> ConnectionState.Open Then
    '    'con.Open()
    '    ' End If
    '    'oda.SelectCommand.ExecuteNonQuery()
    '    con.Dispose()
    '    Return ret
    'End Function
    'Public Function AuthenticateWSRequest(Key As String) As DataSet
    '    Dim ds As New DataSet()
    '    Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
    '    Dim con As SqlConnection = Nothing
    '    Dim da As SqlDataAdapter = Nothing
    '    Try
    '        con = New SqlConnection(conStr)
    '        da = New SqlDataAdapter("AuthenticateWSRequest", con)
    '        da.SelectCommand.CommandType = CommandType.StoredProcedure
    '        da.SelectCommand.Parameters.AddWithValue("@APIKey", Key)
    '        da.Fill(ds)
    '    Catch ex As Exception
    '        Throw
    '    Finally
    '        If Not con Is Nothing Then
    '            con.Close()
    '            con.Dispose()
    '        End If
    '        If Not da Is Nothing Then
    '            da.Dispose()
    '        End If
    '    End Try
    '    Return ds
    'End Function

    'Public Function ReturnInputParaValues(ByVal str As String) As String
    '    Dim xmlDocRead As New XmlDocument()
    '    xmlDocRead.LoadXml(str)
    '    Dim ReturnVal As String = "NA"
    '    If xmlDocRead.ChildNodes.Count >= 1 Then
    '        Dim SelNodesTxt As String = xmlDocRead.DocumentElement.Name
    '        Dim nodes As XmlNodeList = xmlDocRead.SelectNodes(SelNodesTxt)

    '        For Each node As XmlNode In nodes
    '            For c As Integer = 0 To node.ChildNodes.Count - 1
    '                If UCase(node.ChildNodes.Item(c).Name) = "BODY" Then
    '                    For C1 As Integer = 0 To node.Item("BODY").ChildNodes.Count - 1
    '                        If node.Item("BODY").ChildNodes.Item(C1).Name = "DATA" Then
    '                            For c2 As Integer = 0 To node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Count - 1
    '                                If node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).Name = "REQUESTDATA" Then
    '                                    For P As Integer = 0 To node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Count - 1
    '                                        If UCase(node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).Name) = "TALLYMESSAGE" Then
    '                                            For d As Integer = 0 To node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).ChildNodes.Count - 1
    '                                                If UCase(node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).ChildNodes.Item(d).Name) = "DOCUMENTTYPE" Then
    '                                                    ReturnVal = node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).ChildNodes.Item(d).InnerText & "|"
    '                                                ElseIf UCase(node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).ChildNodes.Item(d).Name) = "DISTRIBUTORCODE" Then
    '                                                    ReturnVal &= node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).ChildNodes.Item(d).InnerText
    '                                                End If
    '                                            Next
    '                                        End If
    '                                    Next
    '                                End If
    '                            Next
    '                        End If
    '                    Next
    '                End If
    '            Next
    '        Next
    '    End If
    '    Return ReturnVal
    'End Function


    'Public Function readxmlandgivestring(ByVal str As String) As String
    '    Dim result As String = String.Empty
    '    Dim xmlDocRead As New XmlDocument()
    '    xmlDocRead.LoadXml(str)
    '    If xmlDocRead.ChildNodes.Count >= 1 Then

    '        Dim SelNodesTxt As String = xmlDocRead.DocumentElement.Name
    '        Dim Cnt As Integer = 0
    '        Dim nodes As XmlNodeList = xmlDocRead.SelectNodes(SelNodesTxt)
    '        For Each node As XmlNode In nodes
    '            Cnt += 1
    '            Dim MainVar As String = String.Empty

    '            Dim globalVar As String = String.Empty
    '            Dim ChildVar As String = String.Empty
    '            For c As Integer = 0 To node.ChildNodes.Count - 1
    '                If UCase(node.ChildNodes.Item(c).Name) = "BODY" Then
    '                    For C1 As Integer = 0 To node.Item("BODY").ChildNodes.Count - 1
    '                        If node.Item("BODY").ChildNodes.Item(C1).Name = "DATA" Then
    '                            For c2 As Integer = 0 To node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Count - 1
    '                                If node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).Name = "REQUESTDATA" Then
    '                                    For P As Integer = 0 To node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Count - 1
    '                                        If UCase(node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).Name) = "TALLYMESSAGE" Then
    '                                            For d As Integer = 0 To node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).ChildNodes.Count - 1
    '                                                If UCase(node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).ChildNodes.Item(d).Name) = "DISTRIBUTORNAME" Then
    '                                                    globalVar = "Distributor Name" & "::" & node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).ChildNodes.Item(d).InnerText & "|"
    '                                                ElseIf UCase(node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).ChildNodes.Item(d).Name) = "DISTRIBUTORCODE" Then
    '                                                    globalVar = globalVar & "Distributor Code" & "::" & node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).ChildNodes.Item(d).InnerText & "|"
    '                                                ElseIf UCase(node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).ChildNodes.Item(d).Name) = "SHIPTOPARTY" Then
    '                                                    globalVar = globalVar & "Ship to party" & "::" & node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).ChildNodes.Item(d).InnerText & "|"
    '                                                ElseIf UCase(node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).ChildNodes.Item(d).Name) = "RETAILER" Then
    '                                                    globalVar = globalVar & "Retailer" & "::" & node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).ChildNodes.Item(d).InnerText & "|"
    '                                                ElseIf UCase(node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).ChildNodes.Item(d).Name) = "INVOICENO." Then
    '                                                    globalVar = globalVar & "Invoice No." & "::" & node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).ChildNodes.Item(d).InnerText & "|"
    '                                                ElseIf UCase(node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).ChildNodes.Item(d).Name) = "INVOICEDATE" Then
    '                                                    globalVar = globalVar & "Invoice date" & "::" & node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).ChildNodes.Item(d).InnerText & "|"
    '                                                ElseIf UCase(node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).ChildNodes.Item(d).Name) = "BEAT" Then
    '                                                    globalVar = globalVar & "Beat" & "::" & node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).ChildNodes.Item(d).InnerText & "|"
    '                                                ElseIf UCase(node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).ChildNodes.Item(d).Name) = "GROSSAMOUNT" Then
    '                                                    globalVar = globalVar & "Gross Amount" & "::" & node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).ChildNodes.Item(d).InnerText & "|"
    '                                                ElseIf UCase(node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).ChildNodes.Item(d).Name) = "TAX" Then
    '                                                    globalVar = globalVar & "Tax" & "::" & node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).ChildNodes.Item(d).InnerText & "|"
    '                                                ElseIf UCase(node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).ChildNodes.Item(d).Name) = "TOTALINVOICEAMOUNT" Then
    '                                                    globalVar = globalVar & "Total Invoice amount" & "::" & node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).ChildNodes.Item(d).InnerText & "|"
    '                                                ElseIf UCase(node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).ChildNodes.Item(d).Name) = "DETAILSOFSALES" Then ''child item starts
    '                                                    ChildVar = "Details of Sales::{}"
    '                                                    For x As Integer = 0 To node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).ChildNodes.Item(d).ChildNodes.Count - 1
    '                                                        If UCase(node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).ChildNodes.Item(d).ChildNodes.Item(x).Name) = "ITEMNAME" Then
    '                                                            ChildVar = ChildVar & "()Item Name<>" & node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).ChildNodes.Item(d).ChildNodes.Item(x).InnerText
    '                                                        ElseIf UCase(node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).ChildNodes.Item(d).ChildNodes.Item(x).Name) = "BATCHNAME" Then
    '                                                            ChildVar = ChildVar & "()Batch Name<>" & node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).ChildNodes.Item(d).ChildNodes.Item(x).InnerText
    '                                                        ElseIf UCase(node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).ChildNodes.Item(d).ChildNodes.Item(x).Name) = "QUANTITY" Then
    '                                                            ChildVar = ChildVar & "()Quantity<>" & node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).ChildNodes.Item(d).ChildNodes.Item(x).InnerText
    '                                                        ElseIf UCase(node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).ChildNodes.Item(d).ChildNodes.Item(x).Name) = "UOM" Then
    '                                                            ChildVar = ChildVar & "()UOM<>" & node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).ChildNodes.Item(d).ChildNodes.Item(x).InnerText
    '                                                        ElseIf UCase(node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).ChildNodes.Item(d).ChildNodes.Item(x).Name) = "RATE" Then
    '                                                            ChildVar = ChildVar & "()Rate<>" & node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).ChildNodes.Item(d).ChildNodes.Item(x).InnerText
    '                                                        ElseIf UCase(node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).ChildNodes.Item(d).ChildNodes.Item(x).Name) = "PRODUCT DISCOUNT" Then
    '                                                            ChildVar = ChildVar & "()Product Discount<>" & node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).ChildNodes.Item(d).ChildNodes.Item(x).InnerText
    '                                                        ElseIf UCase(node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).ChildNodes.Item(d).ChildNodes.Item(x).Name) = "AMOUNT" Then
    '                                                            ChildVar = ChildVar & "()Amount<>" & node.Item("BODY").ChildNodes.Item(C1).ChildNodes.Item(c2).ChildNodes.Item(P).ChildNodes.Item(d).ChildNodes.Item(x).InnerText
    '                                                        End If

    '                                                    Next
    '                                                    ChildVar = ChildVar & "{}"
    '                                                End If
    '                                            Next
    '                                        End If
    '                                    Next
    '                                End If
    '                            Next
    '                        End If
    '                    Next
    '                End If
    '            Next
    '            ChildVar = ChildVar.Remove(ChildVar.Length - 2)
    '            result = "Key$$GLDSOOKGBLH000391HSG ~DOCTYPE$$Sales Document~Data$$" & globalVar & ChildVar

    '        Next
    '    End If

    '    Return result
    'End Function

End Class



<DataContractAttribute(Namespace:="", Name:="ENVELOPE")>
Public Class ENVELOPE
    <DataMember(Name:="HEADER", Order:=1)> _
    Public Property HEADER As New HEADER()

    <DataMember(Name:="BODY", Order:=2)> _
    Public Property BODY As New BODY()

End Class

<DataContractAttribute(Namespace:="", Name:="HEADER")>
Public Class HEADER
    <DataMember(Name:="RESPONSE")> _
    Public Property RESPONSE As String
End Class

<DataContractAttribute(Namespace:="", Name:="BODY")>
Public Class BODY
    <DataMember(Name:="DATA")> _
    Public Property DATA As New DATA1()
End Class

<DataContractAttribute(Namespace:="", Name:="DATA")>
Public Class DATA1
    <DataMember(Name:="VALIDATION", Order:=1)> _
    Public Property VALIDATION As New VALIDATION()

    <DataMember(Name:="IMPORTDATA", Order:=2)> _
    Public Property IMPORTDATA As New IMPORTDATA()

End Class

<DataContractAttribute(Namespace:="", Name:="VALIDATION")>
Public Class VALIDATION
    <DataMember(Name:="BILLINGCODE")> _
    Public Property BILLINGCODE As String
    <DataMember(Name:="PASSWD")> _
    Public Property PASSWD As String
End Class

<DataContractAttribute(Namespace:="", Name:="IMPORTDATA")>
Public Class IMPORTDATA
    <DataMember(Name:="TALLYMESSAGE")> _
    Public TALLYMESSAGE As New TALLYMESSAGE()
End Class

<DataContractAttribute(Namespace:="", Name:="TALLYMESSAGE")>
Public Class TALLYMESSAGE
    <DataMember(Name:="VOUCHERS", Order:=1)> _
    Public Property VOUCHER As List(Of VOUCHER)
End Class

<DataContractAttribute(Namespace:="", Name:="Test")>
Public Class Test
    <DataMember(Name:="t", Order:=1)> _
    Dim t As New String("1", "1")
End Class

<DataContractAttribute(Namespace:="", Name:="VOUCHER")>
Public Class VOUCHER
    <DataMember(Name:="DISTRIBUTORNAME", Order:=1)> _
    Public Property DISTRIBUTORNAME As String

    <DataMember(Name:="DISTRIBUTORCODE", Order:=2)> _
    Public Property DISTRIBUTORCODE As String

    <DataMember(Name:="STATE", Order:=3)> _
    Public Property STATE As String

    <DataMember(Name:="BILLDATE", Order:=4)> _
    Public Property BILLDATE As String

    <DataMember(Name:="SUPPLIERINVOICENO", Order:=5)> _
    Public Property SUPPLIERINVOICENO As String

    <DataMember(Name:="VENDORNAME", Order:=6)> _
    Public Property VENDORNAME As String

    <DataMember(Name:="VENDORADDRESS", Order:=7)> _
    Public Property VENDORADDRESS As String

    <DataMember(Name:="VENDORNAME.", Order:=8)> _
    Public Property VENDORTINNO As String

    <DataMember(Name:="GROSSAMOUNT", Order:=9)> _
    Public Property GROSSAMOUNT As String

    <DataMember(Name:="TAX", Order:=10)> _
    Public Property TAX As String

    <DataMember(Name:="TOTALBILLAMOUNT", Order:=11)> _
    Public Property TOTALBILLAMOUNT As String

    <DataMember(Name:="STOCKITEMS", Order:=12)> _
    Public Property STOCKITEMS As List(Of STOCKITEM)

    <DataMember(Name:="INVENTORYENTRIES", Order:=13)> _
    Public Property INVENTORYENTRIES As List(Of INVENTORYENTRIES)

End Class

<DataContractAttribute(Namespace:="", Name:="STOCKITEM")>
Public Class STOCKITEM
    <DataMember(Name:="ITEM", Order:=1)> _
    Public Property ITEM As String

    <DataMember(Name:="ITEM_CODE-SKU", Order:=2)> _
    Public Property ITEM_CODE_SKU As String

    <DataMember(Name:="BATCH", Order:=3)> _
    Public Property BATCH As String

    <DataMember(Name:="PRODUCTGROUP", Order:=4)> _
    Public Property PRODUCTGROUP As String

    <DataMember(Name:="PRIMARYUOM", Order:=5)> _
    Public Property PRIMARYUOM As String
End Class

<DataContractAttribute(Namespace:="", Name:="INVENTORYENTRY")>
Public Class INVENTORYENTRIES
    <DataMember(Name:="ITEMNAME", Order:=1)>
    Public Property ITEMNAME As String

    <DataMember(Name:="BATCHNAME", Order:=2)>
    Public Property BATCHNAME As String

    <DataMember(Name:="QUANTITY", Order:=3)>
    Public Property QUANTITY As String

    <DataMember(Name:="UOM", Order:=4)>
    Public Property UOM As String

    <DataMember(Name:="RATE", Order:=5)>
    Public Property RATE As String

    <DataMember(Name:="PRODUCTDISCOUNT", Order:=6)>
    Public Property PRODUCTDISCOUNT As String

    <DataMember(Name:="AMOUNT", Order:=7)>
    Public Property AMOUNT As String
End Class


'<DataContractAttribute(Namespace:="", Name:="EIDDETAIL")>
'Public Class EIDDETAIL
'    <DataMember(Name:="EID", Order:=1)>
'    Public Property EID As Int32
'End Class

'<DataContractAttribute(Namespace:="", Name:="EIDRESPONSE")>
'Public Class EIDRESPONSE
'    <DataMember(Name:="EID", Order:=1)>
'    Public Property EID As Int32
'    <DataMember(Name:="FTPID", Order:=2)>
'    Public Property ftpid As Int32
'    <DataMember(Name:="UID", Order:=3)>
'    Public Property uid As Int32

'    <DataMember(Name:="GID", Order:=3)>
'    Public Property gid As Int32


'    <DataMember(Name:="DOCTYPE", Order:=3)>
'    Public Property docType As String

'    <DataMember(Name:="FUP_FieldMapping", Order:=3)>
'    Public Property fup_FieldMapping As String
'    <DataMember(Name:="LOC_FIELDMAPPING", Order:=3)>
'    Public Property loc_FieldMapping As String
'    <DataMember(Name:="LOCID", Order:=3)>
'    Public Property locid As String
'    <DataMember(Name:="ReadMode", Order:=3)>
'    Public Property ReadMode As String
'    <DataMember(Name:="HostName", Order:=3)>
'    Public Property HostName As String
'    <DataMember(Name:="UserName", Order:=3)>
'    Public Property UserName As String
'    <DataMember(Name:="Password", Order:=3)>
'    Public Property Password As String
'    <DataMember(Name:="Port", Order:=3)>
'    Public Property Port As String
'    <DataMember(Name:="PostFix", Order:=3)>
'    Public Property PostFix As String

'    <DataMember(Name:="BarCode", Order:=3)>
'    Public Property BarCode As String

'End Class