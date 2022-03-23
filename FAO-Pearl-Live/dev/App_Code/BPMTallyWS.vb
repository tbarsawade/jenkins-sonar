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
Public Class BPMTallyWS
    Implements IBPMTallyWS
    Dim conStr As String = ConfigurationManager.ConnectionStrings("conStrLive").ConnectionString
    Dim BPMKEY As String = ConfigurationManager.AppSettings("BPMKEY").ToString()

    Function INWARD(Data As Stream) As XElement Implements IBPMTallyWS.INWARD
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
           
            Dim strr As String = ReadxmlandGiveString(strData)
           
            msg = "<ENVELOPE> <HEADER><TALLYREQUEST>EXPORT VOUCHERS</TALLYREQUEST></HEADER> <BODY>  <DATA>   <VALIDATION> <RESULT> " & strr & " </RESULT></VALIDATION></DATA> </BODY></ENVELOPE>"

        Catch ex As Exception
            ErrorLog.sendMail("BPMTallyWS.Inward", ex.Message)
            'Return "RTO"
        End Try
        Dim xmldoc As XDocument = XDocument.Parse(msg)
        Return xmldoc.Root

        
    End Function
    Function OUTWARD(Data As Stream) As XElement Implements IBPMTallyWS.OUTWARD
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

            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStrLive").ConnectionString
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
            End If
            CommanUtil.SaveServicerequest(Data1, "BPMTallyWS", "Outward", "")
        Catch ex As Exception
            ErrorLog.sendMail("BPMTallyWS.Outward", ex.Message)
        End Try
        Dim xmldoc As XDocument = XDocument.Parse(Result)
        Return xmldoc.Root
    End Function

    '' new 

    Public Function POrder(ByVal Dtype As String, ByVal eid As Integer, ByVal DistCode As String, ByVal rowfiltercolumn As String, ByVal BPMTID As String) As String


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
        Dim StrQuery = objRel.GenearateQuery1(eid, Dtype, ds)
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
                    Dim filter As String = " and v" & eid & Dtype.Trim.Replace(" ", "_") & ".[" & rowfiltercolumn & "]='" & DistCode & "'"
                    StrQuery = "SELECT  v" & eid & Dtype.Trim.Replace(" ", "_") & ".tid  AS DOCID, " & StrQuery & filter
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
                                Dim childquery = objRel.GenearateQuery1(eid, UCase(flds.Rows(i).Item("dropdown")).ToString.Trim(), ds)
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
                                                    masterval = mastervalued(child.Rows(k).Item("dropdown").ToString(), childv.Rows(c).Item(col.ColumnName).ToString(), eid)
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
                                If UCase(column.ColumnName) = UCase(flds.Rows(i).Item("displayname")).ToString() Then
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

    Protected Function mastervalued(ByVal str As String, ByVal tid As Integer, ByVal eid As Integer) As String
        Dim Result As String = ""

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
        ErrorLog.sendMail("BPMTallyWS.Outward", ReturnVal)
        Return ReturnVal
    End Function


    
    Public Function AuthenticateWSRequest(Key As String) As DataSet
        Dim ds As New DataSet()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStrLive").ConnectionString
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
    'Public Function ReadxmlandGiveString(ByRef str As String) As String
    '    Dim result As String = String.Empty
    '    Dim xmlDocRead As New XmlDocument()
    '    xmlDocRead.LoadXml(str)

    '    Dim conStr As String = ConfigurationManager.ConnectionStrings("conStrLive").ConnectionString
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
    '            Dim GrdCode As String = ""

    '            Dim varINTALL As Double = 0
    '            Dim varBASIC As Double = 0
    '            Dim varHRA As Double = 0
    '            Dim varSODCOP As Double = 0
    '            Dim varCONV As Double = 0
    '            Dim varLTA As Double = 0
    '            Dim varMSCALL As Double = 0
    '            Dim varCTC_PF As Double = 0
    '            Dim varSPLALL As Double = 0
    '            Dim varMED As Double = 0
    '            Dim varCompCode As String = ""
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

        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStrLive").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim da As New SqlDataAdapter("", con)
        Dim ds As New DataSet

        Dim Data1 As New StringBuilder(str)

        If xmlDocRead.ChildNodes.Count >= 1 Then

            Dim SelNodesTxt As String = xmlDocRead.DocumentElement.Name
            Dim Cnt As Integer = 0
            Dim nodes As XmlNodeList = xmlDocRead.SelectNodes(SelNodesTxt)
            For Each node As XmlNode In nodes
                Cnt += 1
                Dim MainVar As String = String.Empty
                Dim GrdCode As String = ""
                Dim cntt As Integer = 0
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
                Dim xmldoctype As String = String.Empty
                Dim xmlecode As String = String.Empty
                Dim apikey As String = String.Empty
                Dim xmlBPMTID As Integer = 0
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
                                        Dim xmleid As Integer = 0
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
                                                                    da.SelectCommand.CommandText = "Select * from mmm_mst_fields where eid=43 and documenttype='" & ds.Tables("data").Rows(s).Item("DROPDOWN").ToString().Trim() & "'"
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
                                                        msg = msg & "Record # " & P + 1 & "- " & result & " :"
                                                        result = ""
                                                        globalVar = ""
                                                        ChildVar = ""
                                                    Catch ex As Exception
                                                        ErrorLog.sendMail("BPMTallyWS.Inward", ex.Message)
                                                        result = ""
                                                        globalVar = ""
                                                        ChildVar = ""
                                                    End Try
                                                End If
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

        Return msg
    End Function

    Function REGISTRATION(Data As Stream) As XElement Implements IBPMTallyWS.REGISTRATION
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
            CommanUtil.SaveServicerequest(Data1, "BPMTallyWS", "AUTHENTICATION", Result)
        Catch ex As Exception
            ErrorLog.sendMail("BPMTally.AUTHENTICATION", ex.Message)
        End Try
        Dim xmldoc As XDocument = XDocument.Parse(Result)
        Return xmldoc.Root
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
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStrLive").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim da As New SqlDataAdapter("", con)
        Dim ds As New DataSet

        If xmlDocRead.ChildNodes.Count >= 1 Then

            Dim SelNodesTxt As String = xmlDocRead.DocumentElement.Name
            Dim Cnt As Integer = 0
            Dim nodes As XmlNodeList = xmlDocRead.SelectNodes(SelNodesTxt)
            For Each node As XmlNode In nodes
                Cnt += 1
                Dim globalVar As String = String.Empty
                Dim ChildVar As String = String.Empty

                Dim apikey As String = String.Empty
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
                Dim eid As Integer = Val(da.SelectCommand.ExecuteScalar())

                'getting Tally Registration FIeld from FORM Master
                da.SelectCommand.CommandText = "select TallyRegField from mmm_mst_forms where eid=" & Val(eid) & " and formname ='" & xmldoctype.ToString() & "'"
                xmlfldname = da.SelectCommand.ExecuteScalar().ToString()

                Dim StrQuery As String = "SELECT tid from  v" & eid & xmldoctype.Trim.Replace(" ", "_") & " where  v" & eid & xmldoctype.Trim.Replace(" ", "_") & ".[" & xmlfldname.ToString() & "] = '" & xmlRegvalue.ToString() & "'"
                da.SelectCommand.CommandText = StrQuery

                result = da.SelectCommand.ExecuteScalar().ToString()
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
        If Not String.IsNullOrEmpty(result) Then
            sb.Append(result)
        Else
            sb.Append("NOT FOUND")
        End If
        sb.Append("</BPMTID>")
        sb.Append("</VALIDATION>")
        sb.Append("</DATA>")
        sb.Append("</BODY>")
        sb.Append("</ENVELOPE>")
        Return sb.ToString()

    End Function

    Function CONFIRMATION(Data As Stream) As XElement Implements IBPMTallyWS.CONFIRMATION
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
            CommanUtil.SaveServicerequest(Data1, "BPMTallyWS", "REGISTRATION", Result)
        Catch ex As Exception
            ErrorLog.sendMail("BPMTally.REGISTRATION", ex.Message)
        End Try
        Dim xmldoc As XDocument = XDocument.Parse(Result)
        Return xmldoc.Root
    End Function


    Public Function readxmlandgivestringConfirmation(ByRef str As String) As String
        Dim result As String = String.Empty
        Dim msg As String = String.Empty
        Dim xmlDocRead As New XmlDocument()
        xmlDocRead.LoadXml(str)
        Dim xmldoctype As String = String.Empty
        Dim xmlecode As String = String.Empty
        Dim xmlBPMTid As Integer = 0
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStrLive").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim da As New SqlDataAdapter("", con)
        Dim ds As New DataSet
        If xmlDocRead.ChildNodes.Count >= 1 Then
            Dim SelNodesTxt As String = xmlDocRead.DocumentElement.Name
            Dim Cnt As Integer = 0
            Dim nodes As XmlNodeList = xmlDocRead.SelectNodes(SelNodesTxt)
            For Each node As XmlNode In nodes
                Cnt += 1
                Dim globalVar As String = String.Empty
                Dim ChildVar As String = String.Empty

                Dim apikey As String = String.Empty
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
                Dim cntt As Integer
                da.SelectCommand.CommandText = "Select count(tid) from mmm_mst_master where tid=" & Val(xmlBPMTid) & ""
                If con.State <> ConnectionState.Open Then
                    con.Open()
                End If
                cntt = Val(da.SelectCommand.ExecuteScalar())
                If Cnt = 1 Then
                    Dim StrQuery As String = "Update mmm_mst_master  set TallyIsActive= 1 where tid=" & Val(xmlBPMTid) & " "
                    da.SelectCommand.CommandText = StrQuery
                    da.SelectCommand.ExecuteNonQuery()
                    result = "SUCCESS"
                Else
                    result = "FAIL"
                End If


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

        Return sb.ToString()
    End Function





    Function UPDATIONFLAG(Data As Stream) As XElement Implements IBPMTallyWS.UPDATIONFLAG
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
            ErrorLog.sendMail("BPMTallyWS.Inward", ex.Message)
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

        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStrLive").ConnectionString
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
                    Dim GrdCode As String = ""
                    Dim cntt As Integer = 0
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
                                                Dim cmd As New SqlCommand("Update mmm_mst_doc set isexport=1 where tid in ( SELECT * FROM dms.inputstring1('" & xmlBPMTIDS & "')) ", con)

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
End Class

'<DataContractAttribute(Namespace:="", Name:="ENVELOPE")>
'Public Class ENVELOPE
'    <DataMember(Name:="HEADER", Order:=1)> _
'    Public Property HEADER As New HEADER()

'    <DataMember(Name:="BODY", Order:=2)> _
'    Public Property BODY As New BODY()

'End Class

'<DataContractAttribute(Namespace:="", Name:="HEADER")>
'Public Class HEADER
'    <DataMember(Name:="RESPONSE")> _
'    Public Property RESPONSE As String
'End Class

'<DataContractAttribute(Namespace:="", Name:="BODY")>
'Public Class BODY
'    <DataMember(Name:="DATA")> _
'    Public Property DATA As New DATA1()
'End Class

'<DataContractAttribute(Namespace:="", Name:="DATA")>
'Public Class DATA1
'    <DataMember(Name:="VALIDATION", Order:=1)> _
'    Public Property VALIDATION As New VALIDATION()

'    <DataMember(Name:="IMPORTDATA", Order:=2)> _
'    Public Property IMPORTDATA As New IMPORTDATA()

'End Class

'<DataContractAttribute(Namespace:="", Name:="VALIDATION")>
'Public Class VALIDATION
'    <DataMember(Name:="BILLINGCODE")> _
'    Public Property BILLINGCODE As String
'    <DataMember(Name:="PASSWD")> _
'    Public Property PASSWD As String
'End Class

'<DataContractAttribute(Namespace:="", Name:="IMPORTDATA")>
'Public Class IMPORTDATA
'    <DataMember(Name:="TALLYMESSAGE")> _
'    Public TALLYMESSAGE As New TALLYMESSAGE()
'End Class

'<DataContractAttribute(Namespace:="", Name:="TALLYMESSAGE")>
'Public Class TALLYMESSAGE
'    <DataMember(Name:="VOUCHERS", Order:=1)> _
'    Public Property VOUCHER As List(Of VOUCHER)
'End Class

'<DataContractAttribute(Namespace:="", Name:="Test")>
'Public Class Test
'    <DataMember(Name:="t", Order:=1)> _
'    Dim t As New String("1", "1")
'End Class

'<DataContractAttribute(Namespace:="", Name:="VOUCHER")>
'Public Class VOUCHER
'    <DataMember(Name:="DISTRIBUTORNAME", Order:=1)> _
'    Public Property DISTRIBUTORNAME As String

'    <DataMember(Name:="DISTRIBUTORCODE", Order:=2)> _
'    Public Property DISTRIBUTORCODE As String

'    <DataMember(Name:="STATE", Order:=3)> _
'    Public Property STATE As String

'    <DataMember(Name:="BILLDATE", Order:=4)> _
'    Public Property BILLDATE As String

'    <DataMember(Name:="SUPPLIERINVOICENO", Order:=5)> _
'    Public Property SUPPLIERINVOICENO As String

'    <DataMember(Name:="VENDORNAME", Order:=6)> _
'    Public Property VENDORNAME As String

'    <DataMember(Name:="VENDORADDRESS", Order:=7)> _
'    Public Property VENDORADDRESS As String

'    <DataMember(Name:="VENDORNAME.", Order:=8)> _
'    Public Property VENDORTINNO As String

'    <DataMember(Name:="GROSSAMOUNT", Order:=9)> _
'    Public Property GROSSAMOUNT As String

'    <DataMember(Name:="TAX", Order:=10)> _
'    Public Property TAX As String

'    <DataMember(Name:="TOTALBILLAMOUNT", Order:=11)> _
'    Public Property TOTALBILLAMOUNT As String

'    <DataMember(Name:="STOCKITEMS", Order:=12)> _
'    Public Property STOCKITEMS As List(Of STOCKITEM)

'    <DataMember(Name:="INVENTORYENTRIES", Order:=13)> _
'    Public Property INVENTORYENTRIES As List(Of INVENTORYENTRIES)

'End Class

'<DataContractAttribute(Namespace:="", Name:="STOCKITEM")>
'Public Class STOCKITEM
'    <DataMember(Name:="ITEM", Order:=1)> _
'    Public Property ITEM As String

'    <DataMember(Name:="ITEM_CODE-SKU", Order:=2)> _
'    Public Property ITEM_CODE_SKU As String

'    <DataMember(Name:="BATCH", Order:=3)> _
'    Public Property BATCH As String

'    <DataMember(Name:="PRODUCTGROUP", Order:=4)> _
'    Public Property PRODUCTGROUP As String

'    <DataMember(Name:="PRIMARYUOM", Order:=5)> _
'    Public Property PRIMARYUOM As String
'End Class

'<DataContractAttribute(Namespace:="", Name:="INVENTORYENTRY")>
'Public Class INVENTORYENTRIES
'    <DataMember(Name:="ITEMNAME", Order:=1)> _
'    Public Property ITEMNAME As String

'    <DataMember(Name:="BATCHNAME", Order:=2)> _
'    Public Property BATCHNAME As String

'    <DataMember(Name:="QUANTITY", Order:=3)> _
'    Public Property QUANTITY As String

'    <DataMember(Name:="UOM", Order:=4)> _
'    Public Property UOM As String

'    <DataMember(Name:="RATE", Order:=5)> _
'    Public Property RATE As String

'    <DataMember(Name:="PRODUCTDISCOUNT", Order:=6)> _
'    Public Property PRODUCTDISCOUNT As String

'    <DataMember(Name:="AMOUNT", Order:=7)> _
'    Public Property AMOUNT As String
'End Class
