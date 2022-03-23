Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Xml
Imports System.Data.SqlClient
Imports System.IO
Imports System.Security
Imports System.Globalization
Imports System.Net
Imports System.Web.Script.Serialization
Imports System.Security.Authentication

Public Class WSOutward

    Public Function WBS(ByVal doctype As String, ByVal entity As Integer, ByVal docid As Integer, Optional ByVal wstype As String = "Create") As String
        Dim wstypelog As String = wstype.ToString()
        Dim result As String = ""
        Dim exemail As String = ""
        Dim strResult As String = String.Empty
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)

        Dim da As New SqlDataAdapter("Select count(doctype) from mmm_mst_wsoutward where eid=" & entity & " and doctype='" & doctype & "' and wstype='" & wstype & "' and isreport=0", con)
        Dim ds As New DataSet

        If con.State <> ConnectionState.Open Then
            con.Open()
        End If

        Try

            Dim cnt As Integer = da.SelectCommand.ExecuteScalar()
            If cnt > 0 Then
                da.SelectCommand.CommandText = "select * from mmm_mst_wsoutward where eid=" & entity & " and doctype='" & doctype & "' and wstype='" & wstype & "' and isreport=0"
                da.Fill(ds, "outward")

                'da.SelectCommand.CommandText = "select substring((select ',' + FieldMapping + '[' + displayname + ']'  from mmm_mst_fields where eid=" & entity & " and documenttype='" & doctype & "' for xml path ('')),2,20000) as displayname "
                da.SelectCommand.CommandText = "select * from mmm_mst_fields where eid=" & entity & " and documenttype='" & doctype & "' order by  displayname "
                da.Fill(ds, "data")

                da.SelectCommand.CommandText = "select * from mmm_mst_forms where eid=" & entity & " and formname ='" & doctype & "' "
                da.Fill(ds, "form")
                Dim dtype As String = ds.Tables("form").Rows(0).Item("Formtype").ToString
                Dim Plist As String = ""
                Dim url As String = ""
                ' Dim fm As String = ds.Tables("data").Rows(0).Item("displayname").ToString()
                Dim fm As String = ""
                Dim psep As String = ""
                Dim dformat As String = ""

                For j As Integer = 0 To ds.Tables("outward").Rows.Count - 1
                    psep = ds.Tables("outward").Rows(j).Item("Paraseprator").ToString()
                    Plist = ds.Tables("outward").Rows(j).Item("Paralist").ToString()

                    url = ds.Tables("outward").Rows(j).Item("URI").ToString()
                    dformat = ds.Tables("outward").Rows(j).Item("dateformat").ToString()
                    Dim type As String = UCase(ds.Tables("outward").Rows(j).Item("type").ToString())

                    If type = "GET" Then

                        For k As Integer = 0 To ds.Tables("data").Rows.Count - 1
                            Dim dn As String = ds.Tables("data").Rows(k).Item("displayname").ToString()
                            Dim fld As String = ds.Tables("data").Rows(k).Item("fieldmapping").ToString()
                            Dim mv As String = ds.Tables("data").Rows(k).Item("dropdowntype").ToString()
                            Dim DD As String = ds.Tables("data").Rows(k).Item("dropdown").ToString()
                            Dim datatype As String = ds.Tables("data").Rows(k).Item("datatype").ToString()

                            Dim d3 As String = ""
                            Dim dw As String = ""

                            If UCase(mv) = "MASTER VALUED" Then
                                Dim ss As String() = DD.ToString().Split("-")
                                If UCase(ss(0)).ToString = "MASTER" Then
                                    d3 = "(select " & ss(2).ToString() & " from mmm_mst_master where eid=" & entity & " and tid=m." & fld & ")"
                                ElseIf UCase(ss(0)).ToString = "DOCUMENT" Then
                                    d3 = "(select " & ss(2).ToString() & " from mmm_mst_doc where eid=" & entity & " and tid=m." & fld & ")"
                                ElseIf UCase(ss(1)).ToString = "USER" Then
                                    d3 = "(select " & ss(2).ToString() & " from mmm_mst_USER where eid=" & entity & " and tid=m." & fld & ")"
                                End If
                                dw = dw & d3
                                fm = fm & dw & "[" & dn & "]" & ","
                            ElseIf UCase(mv) = "SESSION VALUED" Then
                                d3 = "(select username from mmm_mst_USER U where eid=" & entity & " and U.uid= m." & fld & ")"
                                dw = dw & d3
                                fm = fm & dw & "[" & dn & "]" & ","
                            Else
                                fm = fm & fld & "[" & dn & "]" & ","
                            End If

                        Next
                        fm = fm.Remove(fm.Length - 1)
                        If UCase(dtype) = "MASTER" Then
                            da.SelectCommand.CommandText = "SELECT  " & fm & " FROM MMM_MST_MASTER m WHERE EID=" & entity & " AND DOCUMENTTYPE='" & doctype & "' and tid=" & docid & ""
                            da.Fill(ds, "master")
                            If ds.Tables("master").Rows.Count > 0 Then
                                For i As Integer = 0 To ds.Tables("master").Columns.Count - 1
                                    Dim a As String = "{" & UCase(Trim(ds.Tables("master").Columns(i).ColumnName.ToString())) & "}"
                                    Plist = Plist.Replace(a, ds.Tables("master").Rows(0).Item(i).ToString())
                                Next
                            End If
                        ElseIf UCase(dtype) = "DOCUMENT" Then
                            da.SelectCommand.CommandText = "SELECT " & fm & "  FROM MMM_MST_DOC m WHERE EID=" & entity & " AND DOCUMENTTYPE='" & doctype & "' and tid=" & docid & " "
                            da.Fill(ds, "document")
                            If ds.Tables("document").Rows.Count > 0 Then
                                For i As Integer = 0 To ds.Tables("document").Columns.Count - 1
                                    da.SelectCommand.CommandText = "select * from mmm_mst_fields where eid=" & entity & " and documenttype='" & doctype & "' and displayname='" & ds.Tables("document").Columns(i).ColumnName.ToString() & "'"
                                    da.Fill(ds, "df")
                                    Dim a As String = ""
                                    If ds.Tables("df").Rows(i).Item("datatype").ToString() = "Datetime" Then
                                        a = "{" & UCase(Trim(ds.Tables("document").Columns(i).ColumnName.ToString())) & "}"
                                        '     Dim z As String = Format(Convert.ToDateTime(ds.Tables("document").Columns(i).ColumnName.ToString()), dformat)
                                        ' Dim addd As Date = ds.Tables("document").Columns(i).ColumnName.ToString()
                                        Dim res As DateTime = getdate(ds.Tables("document").Rows(0).Item(i).ToString())
                                        ' Dim ddt As DateTime = DateTime.ParseExact(res, "dd/MM/yyyy", Nothing)

                                        Dim newDate As String = res.ToString(dformat)

                                        Plist = Plist.Replace(a, newDate)
                                    Else
                                        a = "{" & UCase(Trim(ds.Tables("document").Columns(i).ColumnName.ToString())) & "}"
                                        Plist = Plist.Replace(a, ds.Tables("document").Rows(0).Item(i).ToString())
                                    End If

                                    'Plist = Plist.Replace(a, ds.Tables("document").Rows(0).Item(i).ToString())
                                Next
                            End If

                        End If

                        result = url & Plist

                        ServicePointManager.SecurityProtocol = CType(3072, SecurityProtocolType)

                        'Const _Tls12 As SslProtocols = DirectCast(&HC00, SslProtocols)
                        'Const Tls12 As SecurityProtocolType = DirectCast(_Tls12, SecurityProtocolType)
                        'ServicePointManager.SecurityProtocol = Tls12

                        Dim request As HttpWebRequest = HttpWebRequest.Create(result)
                        request.Timeout = 1000 * 1000
                        Dim oResponse As HttpWebResponse = DirectCast(request.GetResponse(), HttpWebResponse)
                        Dim reader As New StreamReader(oResponse.GetResponseStream())
                        Dim enc As Encoding = System.Text.Encoding.GetEncoding("utf-8")
                        Dim loResponseStream As New StreamReader(oResponse.GetResponseStream(), enc)
                        strResult = loResponseStream.ReadToEnd()
                        Dim regex As New Regex("\<[^\>]*\>")

                        strResult = regex.Replace(strResult, [String].Empty)

                        If Not strResult.Contains("You requested product") Then
                            da.SelectCommand.CommandText = "Select wserrorEmail from mmm_mst_entity where eid=" & entity & ""
                            If con.State <> ConnectionState.Open Then
                                con.Open()
                            End If
                            Dim emailto As String = da.SelectCommand.ExecuteScalar()
                            exemail = emailto.ToString()
                            Dim TheMessage As String
                            TheMessage = "<HTML><BODY>" _
                                & "Hi, <BR/><BR/>" _
                                & "Your DocID no. " & docid & " has not been inserted to client's Database <BR/><BR/>" _
                                & "Error Occured: " & strResult & "  <BR/><BR/>" _
                                & "WebService Created for this DocID: " & docid & " given below:<BR/><BR/>" _
                                & "" & result & " <BR/><BR/>" _
                                & "Thanks & Regards<BR/><BR/>" _
                                & "IT Support Team" _
                                & "</BODY></HTML>"
                            sendMail(emailto.ToString(), "Document Not Called For WebService", TheMessage.ToString())

                            savewslog(docid, strResult.ToString(), entity, result, "FAILED", 0, "", doctype.ToString(), wstypelog.ToString())
                        Else
                            savewslog(docid, strResult.ToString(), entity, result, "SUCCESS", 0, "", doctype.ToString(), wstypelog.ToString())

                        End If


                    ElseIf type = "POST" Then


                        For k As Integer = 0 To ds.Tables("data").Rows.Count - 1
                            Dim dn As String = ds.Tables("data").Rows(k).Item("displayname").ToString()
                            Dim fld As String = ds.Tables("data").Rows(k).Item("fieldmapping").ToString()
                            Dim mv As String = ds.Tables("data").Rows(k).Item("dropdowntype").ToString()
                            Dim DD As String = ds.Tables("data").Rows(k).Item("dropdown").ToString()
                            Dim datatype As String = ds.Tables("data").Rows(k).Item("datatype").ToString()

                            Dim d3 As String = ""
                            Dim dw As String = ""

                            If UCase(mv) = "MASTER VALUED" Then
                                Dim ss As String() = DD.ToString().Split("-")
                                If UCase(ss(0)).ToString = "MASTER" Then
                                    d3 = "(select " & ss(2).ToString() & " from mmm_mst_master where eid=" & entity & " and tid=m." & fld & ")"
                                ElseIf UCase(ss(0)).ToString = "DOCUMENT" Then
                                    d3 = "(select " & ss(2).ToString() & " from mmm_mst_doc where eid=" & entity & " and tid=m." & fld & ")"
                                ElseIf UCase(ss(1)).ToString = "USER" Then
                                    d3 = "(select " & ss(2).ToString() & " from mmm_mst_USER where eid=" & entity & " and tid=m." & fld & ")"
                                End If
                                dw = dw & d3
                                fm = fm & dw & "[" & dn & "]" & ","
                            ElseIf UCase(mv) = "SESSION VALUED" Then
                                d3 = "(select username from mmm_mst_USER U where eid=" & entity & " and U.uid= m." & fld & ")"
                                dw = dw & d3
                                fm = fm & dw & "[" & dn & "]" & ","
                            Else
                                fm = fm & fld & "[" & dn & "]" & ","
                            End If

                        Next
                        fm = fm.Remove(fm.Length - 1)
                        If UCase(dtype) = "MASTER" Then
                            da.SelectCommand.CommandText = "SELECT  " & fm & " FROM MMM_MST_MASTER m WHERE EID=" & entity & " AND DOCUMENTTYPE='" & doctype & "' and tid=" & docid & ""
                            da.Fill(ds, "master")
                            If ds.Tables("master").Rows.Count > 0 Then
                                For i As Integer = 0 To ds.Tables("master").Columns.Count - 1
                                    Dim a As String = "{" & UCase(Trim(ds.Tables("master").Columns(i).ColumnName.ToString())) & "}"
                                    Plist = Plist.Replace(a, ds.Tables("master").Rows(0).Item(i).ToString())
                                Next
                            End If
                        ElseIf UCase(dtype) = "DOCUMENT" Then
                            da.SelectCommand.CommandText = "SELECT " & fm & "  FROM MMM_MST_DOC m WHERE EID=" & entity & " AND DOCUMENTTYPE='" & doctype & "' and tid=" & docid & " "
                            da.Fill(ds, "document")
                            If ds.Tables("document").Rows.Count > 0 Then
                                For i As Integer = 0 To ds.Tables("document").Columns.Count - 1
                                    da.SelectCommand.CommandText = "select * from mmm_mst_fields where eid=" & entity & " and documenttype='" & doctype & "' and displayname='" & ds.Tables("document").Columns(i).ColumnName.ToString() & "'"
                                    da.Fill(ds, "df")
                                    Dim a As String = ""
                                    If ds.Tables("df").Rows(i).Item("datatype").ToString() = "Datetime" Then
                                        a = "{" & UCase(Trim(ds.Tables("document").Columns(i).ColumnName.ToString())) & "}"
                                        '     Dim z As String = Format(Convert.ToDateTime(ds.Tables("document").Columns(i).ColumnName.ToString()), dformat)
                                        ' Dim addd As Date = ds.Tables("document").Columns(i).ColumnName.ToString()
                                        Dim res As DateTime = getdate(ds.Tables("document").Rows(0).Item(i).ToString())


                                        Dim newDate As String = res.ToString(dformat)

                                        Plist = Plist.Replace(a, newDate)
                                    Else
                                        a = "{" & UCase(Trim(ds.Tables("document").Columns(i).ColumnName.ToString())) & "}"
                                        Plist = Plist.Replace(a, ds.Tables("document").Rows(0).Item(i).ToString())
                                    End If


                                Next
                            End If

                        End If

                        result = url & Plist

                        Dim encoding As New ASCIIEncoding()
                        Dim postData As String = Plist.ToString()

                        ' convert xmlstring to byte using ascii encoding
                        Dim data As Byte() = encoding.GetBytes(postData)
                        ' declare httpwebrequet wrt url defined above

                        ServicePointManager.SecurityProtocol = CType(3072, SecurityProtocolType)

                        'Const _Tls12 As SslProtocols = DirectCast(&HC00, SslProtocols)
                        'Const Tls12 As SecurityProtocolType = DirectCast(_Tls12, SecurityProtocolType)
                        'ServicePointManager.SecurityProtocol = Tls12

                        Dim webrequest__1 As HttpWebRequest = DirectCast(WebRequest.Create(url), HttpWebRequest)
                        ' set method as post
                        webrequest__1.Method = "POST"
                        ' set content type
                        webrequest__1.ContentType = "application/x-www-form-urlencoded"
                        ' set content length
                        webrequest__1.ContentLength = data.Length
                        ' get stream data out of webrequest object
                        Dim newStream As Stream = webrequest__1.GetRequestStream()
                        newStream.Write(data, 0, data.Length)
                        newStream.Close()
                        ' declare & read response from service
                        Dim webresponse As HttpWebResponse = DirectCast(webrequest__1.GetResponse(), HttpWebResponse)

                        ' set utf8 encoding
                        Dim enc As Encoding = System.Text.Encoding.GetEncoding("utf-8")
                        ' read response stream from response object
                        Dim loResponseStream As New StreamReader(webresponse.GetResponseStream(), enc)
                        ' read string from stream data
                        strResult = loResponseStream.ReadToEnd()
                        ' close the stream object
                        loResponseStream.Close()
                        ' close the response object
                        webresponse.Close()
                        'Dim request As HttpWebRequest = HttpWebRequest.Create(result)
                        'request.Timeout = 1000 * 1000
                        'Dim oResponse As HttpWebResponse = DirectCast(request.GetResponse(), HttpWebResponse)
                        'Dim reader As New StreamReader(oResponse.GetResponseStream())
                        'Dim enc As Encoding = System.Text.Encoding.GetEncoding("utf-8")
                        'Dim loResponseStream As New StreamReader(oResponse.GetResponseStream(), enc)
                        'strResult = loResponseStream.ReadToEnd()
                        Dim regex As New Regex("\<[^\>]*\>")

                        strResult = regex.Replace(strResult, [String].Empty)
                        'strResult = strResult.Replace("<XMLDataResponse xmlns=""http://tempuri.org/""><XMLDataResult>", "")
                        'strResult = strResult.Replace("</XMLDataResult></XMLDataResponse>", "")
                        'Dim ssl As String = strResult.Substring(0, 21)



                        If Not strResult.Contains("You requested product") Then
                            da.SelectCommand.CommandText = "Select wserrorEmail from mmm_mst_entity where eid=" & entity & ""
                            If con.State <> ConnectionState.Open Then
                                con.Open()
                            End If
                            Dim emailto As String = da.SelectCommand.ExecuteScalar()
                            exemail = emailto.ToString()
                            Dim TheMessage As String
                            TheMessage = "<HTML><BODY>" _
                                & "Hi, <BR/><BR/>" _
                                & "Your DocID no. " & docid & " has not been inserted to client's Database <BR/><BR/>" _
                                & "Error Occured: " & strResult & "  <BR/><BR/>" _
                                & "WebService Created for this DocID: " & docid & " given below:<BR/><BR/>" _
                                & "" & result & " <BR/><BR/>" _
                                & "Thanks & Regards<BR/><BR/>" _
                                & "IT Support Team" _
                                & "</BODY></HTML>"
                            sendMail(emailto.ToString(), "Document Not Called For WebService", TheMessage.ToString())

                            savewslog(docid, strResult.ToString(), entity, result, "FAILED", 0, "", doctype.ToString(), wstypelog.ToString())
                        Else
                            savewslog(docid, strResult.ToString(), entity, result, "SUCCESS", 0, "", doctype.ToString(), wstypelog.ToString())

                        End If

                    End If
                Next
            End If
        Catch ex As Exception
            Dim exmessage As String
            exmessage = "<HTML><BODY>" _
                & "Hi, <BR/><BR/>" _
                & "Your DocID no. " & docid & " has not been inserted to client's Database <BR/><BR/>" _
                & "Error Occured: " & ex.Message.ToString() & "  <BR/><BR/>" _
                & "WebService Created for this DocID: " & docid & " given below:<BR/><BR/>" _
                & "" & result & " <BR/><BR/>" _
                & "Thanks & Regards<BR/><BR/>" _
                & "IT Support Team" _
                & "</BODY></HTML>"
            sendMail(exemail.ToString(), "Document Not Called For WebService", exmessage.ToString())

            savewslog(docid, "", entity, result, "FAILED", 0, ex.Message.ToString(), doctype.ToString(), wstypelog.ToString())
        Finally
            If Not con Is Nothing Then
                con.Close()
                con.Dispose()
            End If
            If Not da Is Nothing Then
                da.Dispose()
            End If

        End Try

        Return strResult
    End Function

    Public Function WBSREPORT(ByVal doctype As String, ByVal entity As Integer, ByVal docid As Integer, Optional ByVal wstype As String = "Create") As String
        Dim wstypelog As String = wstype.ToString()
        Dim result As String = ""
        Dim exemail As String = ""
        Dim strResult As String = String.Empty
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)

        Dim da As New SqlDataAdapter("Select count(doctype) from mmm_mst_wsoutward where eid=" & entity & " and doctype='" & doctype & "' and wstype='" & wstype & "' and isreport='1'", con)
        Dim ds As New DataSet

        If con.State <> ConnectionState.Open Then
            con.Open()
        End If

        Try

            Dim cnt As Integer = da.SelectCommand.ExecuteScalar()
            If cnt > 0 Then
                da.SelectCommand.CommandText = "select * from mmm_mst_wsoutward where eid=" & entity & " and doctype='" & doctype & "' and wstype='" & wstype & "' and isreport in ('1','2')"
                da.Fill(ds, "outward")

                'da.SelectCommand.CommandText = "select substring((select ',' + FieldMapping + '[' + displayname + ']'  from mmm_mst_fields where eid=" & entity & " and documenttype='" & doctype & "' for xml path ('')),2,20000) as displayname "
                da.SelectCommand.CommandText = "select * from mmm_print_template where eid=" & entity & " and documenttype='" & doctype & "' and sendtype='ws'"
                da.Fill(ds, "data")
                Dim Plist As String = ""
                Dim url As String = ""
                ' Dim fm As String = ds.Tables("data").Rows(0).Item("displayname").ToString()
                Dim fm As String = ""
                Dim psep As String = ""
                Dim dformat As String = ""

                da.SelectCommand.CommandText = "select fld1 from mmm_mst_doc where tid='" & docid & "'"
                da.Fill(ds, "survey")
                Dim sid As String = ds.Tables("survey").Rows(0).Item(0).ToString

                For j As Integer = 0 To ds.Tables("outward").Rows.Count - 1
                    psep = ds.Tables("outward").Rows(j).Item("Paraseprator").ToString()
                    Plist = Trim(ds.Tables("outward").Rows(j).Item("Paralist").ToString())
                    url = ds.Tables("outward").Rows(j).Item("URI").ToString()
                    dformat = ds.Tables("outward").Rows(j).Item("dateformat").ToString()
                    Dim type As String = UCase(ds.Tables("outward").Rows(j).Item("type").ToString())

                    If type = "GET" Then

                    ElseIf ds.Tables("outward").Rows(j).Item("isreport").ToString() = "1" And type = "POST" Then
                        Dim qry As String = ds.Tables("data").Rows(0).Item("qry").ToString()
                        qry = qry.Replace("@tid", sid)
                        da.SelectCommand.CommandText = qry.Replace("@did", docid)
                        da.Fill(ds, "main")
                        Dim a As String = ""
                        For i As Integer = 0 To ds.Tables("main").Columns.Count - 1
                            a = "{" & Trim(ds.Tables("main").Columns(i).ColumnName.ToString()) & "}"
                            Plist = Plist.Replace(a, ds.Tables("main").Rows(0).Item(i).ToString())
                        Next

                    ElseIf ds.Tables("outward").Rows(j).Item("isreport").ToString() = "2" And type = "POST" Then
                        Dim qry As String = ds.Tables("data").Rows(0).Item("sql_childitem").ToString()
                        Dim fnstr As String = Plist
                        Dim fn1str As String = ""
                        qry = qry.Replace("@tid", sid)
                        da.SelectCommand.CommandText = qry.Replace("@did", docid)
                        da.Fill(ds, "mainqry")
                        da.SelectCommand.CommandText = "select fld1 from mmm_mst_doc where tid='" & sid & "'"
                        da.Fill(ds, "enqid")
                        Dim enqid As String = ds.Tables("enqid").Rows(0).Item(0).ToString()

                        da.SelectCommand.CommandText = "select fld1,CONVERT(CHAR(19), CONVERT(DATE,fld10, 3), 110)[date] from mmm_mst_master where tid='" & enqid & "'"
                        da.Fill(ds, "enqno")
                        Dim enqno As String = ds.Tables("enqno").Rows(0).Item("fld1").ToString()
                        Dim enqdate As String = ds.Tables("enqno").Rows(0).Item("date").ToString()
                        Dim str As String = "EnquiryNO **" & enqno & "||Furniture Fixtures ** "
                        Dim inq As String = "|| Inquiry_Date ** " & enqdate & ""
                        Dim a As String = ""
                        For i As Integer = 0 To ds.Tables("mainqry").Rows.Count - 1
                            For k As Integer = 0 To ds.Tables("mainqry").Columns.Count - 1
                                a = "{" & Trim(ds.Tables("mainqry").Columns(k).ColumnName.ToString()) & "}"
                                If k = 0 Then
                                    fnstr = Plist.Replace(a, ds.Tables("mainqry").Rows(i).Item(k).ToString())
                                Else
                                    fnstr = fnstr.Replace(a, ds.Tables("mainqry").Rows(i).Item(k).ToString())
                                End If
                            Next
                            fn1str = fn1str & Environment.NewLine & Environment.NewLine & fnstr
                            fnstr = Plist
                        Next
                        Dim l As Integer = 0

                        fn1str = str & Environment.NewLine & fn1str
                        fn1str = Left((fn1str), (fn1str).Length - 2)
                        Plist = fn1str & inq
                    End If

                    'Next

                    Dim encoding As New ASCIIEncoding()
                    Dim postData As String = Plist.ToString()

                    ' convert xmlstring to byte using ascii encoding
                    Dim data As Byte() = encoding.GetBytes(postData)
                    ' declare httpwebrequet wrt url defined above

                    ServicePointManager.SecurityProtocol = CType(3072, SecurityProtocolType)

                    'Const _Tls12 As SslProtocols = DirectCast(&HC00, SslProtocols)
                    'Const Tls12 As SecurityProtocolType = DirectCast(_Tls12, SecurityProtocolType)
                    'ServicePointManager.SecurityProtocol = Tls12

                    Dim webrequest__1 As HttpWebRequest = DirectCast(WebRequest.Create(Trim(url)), HttpWebRequest)
                    ' set method as post
                    webrequest__1.Method = "POST"
                    ' set content type
                    webrequest__1.ContentType = "application/x-www-form-urlencoded"
                    ' set content length
                    webrequest__1.ContentLength = data.Length
                    ' get stream data out of webrequest object
                    Dim newStream As Stream = webrequest__1.GetRequestStream()
                    newStream.Write(data, 0, data.Length)
                    newStream.Close()
                    ' declare & read response from service
                    Dim webresponse As HttpWebResponse = DirectCast(webrequest__1.GetResponse(), HttpWebResponse)

                    ' set utf8 encoding
                    Dim enc As Encoding = System.Text.Encoding.GetEncoding("utf-8")
                    ' read response stream from response object
                    Dim loResponseStream As New StreamReader(webresponse.GetResponseStream(), enc)
                    ' read string from stream data
                    strResult = loResponseStream.ReadToEnd()
                    ' close the stream object
                    loResponseStream.Close()
                    ' close the response object
                    webresponse.Close()
                    'Dim request As HttpWebRequest = HttpWebRequest.Create(result)
                    'request.Timeout = 1000 * 1000
                    'Dim oResponse As HttpWebResponse = DirectCast(request.GetResponse(), HttpWebResponse)
                    'Dim reader As New StreamReader(oResponse.GetResponseStream())
                    'Dim enc As Encoding = System.Text.Encoding.GetEncoding("utf-8")
                    'Dim loResponseStream As New StreamReader(oResponse.GetResponseStream(), enc)
                    'strResult = loResponseStream.ReadToEnd()
                    Dim regex As New Regex("\<[^\>]*\>")

                    strResult = regex.Replace(strResult, [String].Empty)
                    'strResult = strResult.Replace("<XMLDataResponse xmlns=""http://tempuri.org/""><XMLDataResult>", "")
                    'strResult = strResult.Replace("</XMLDataResult></XMLDataResponse>", "")
                    'Dim ssl As String = strResult.Substring(0, 21)

                    result = url & Plist

                    If Not strResult.Contains("You requested product") Then
                        da.SelectCommand.CommandText = "Select wserrorEmail from mmm_mst_entity where eid=" & entity & ""
                        If con.State <> ConnectionState.Open Then
                            con.Open()
                        End If
                        Dim emailto As String = da.SelectCommand.ExecuteScalar()
                        exemail = emailto.ToString()
                        Dim TheMessage As String
                        TheMessage = "<HTML><BODY>" _
                            & "Hi, <BR/><BR/>" _
                            & "Your DocID no. " & docid & " has not been inserted to client's Database <BR/><BR/>" _
                            & "Error Occured: " & strResult & "  <BR/><BR/>" _
                            & "WebService Created for this DocID: " & docid & " given below:<BR/><BR/>" _
                            & "" & result & " <BR/><BR/>" _
                            & "Thanks & Regards<BR/><BR/>" _
                            & "IT Support Team" _
                            & "</BODY></HTML>"
                        sendMail(emailto.ToString(), "Document Not Called For WebService", TheMessage.ToString())

                        savewslog(docid, strResult.ToString(), entity, result, "FAILED", 0, "", doctype.ToString(), wstypelog.ToString())
                    Else
                        savewslog(docid, strResult.ToString(), entity, result, "SUCCESS", 0, "", doctype.ToString(), wstypelog.ToString())

                    End If
                Next
            End If

            'End If
        Catch ex As Exception
            Dim exmessage As String
            exmessage = "<HTML><BODY>" _
                & "Hi, <BR/><BR/>" _
                & "Your DocID no. " & docid & " has not been inserted to client's Database <BR/><BR/>" _
                & "Error Occured: " & ex.Message.ToString() & "  <BR/><BR/>" _
                & "WebService Created for this DocID: " & docid & " given below:<BR/><BR/>" _
                & "" & result & " <BR/><BR/>" _
                & "Thanks & Regards<BR/><BR/>" _
                & "IT Support Team" _
                & "</BODY></HTML>"
            sendMail(exemail.ToString(), "Document Not Called For WebService", exmessage.ToString())

            savewslog(docid, "", entity, result, "FAILED", 0, ex.Message.ToString(), doctype.ToString(), wstypelog.ToString())
        Finally
            If Not con Is Nothing Then
                con.Close()
                con.Dispose()
            End If
            If Not da Is Nothing Then
                da.Dispose()
            End If

        End Try

        Return strResult
    End Function

    Public Shared Function getdate(ByVal dbt As String) As DateTime
        Dim dtArr() As String
        dtArr = Split(dbt, "/")
        If dtArr.GetUpperBound(0) = 2 Then
            Dim dd, mm, yy As String
            dd = dtArr(0)
            mm = dtArr(1)
            yy = dtArr(2)
            Dim dt As DateTime
            Dim ab As String = mm & "/" & dd & "/" & 20 & yy
            Try

                dt = Date.Parse(ab.ToString())

                Return dt
            Catch ex As Exception
                Return Now.Date
            End Try
        Else
            Return Now.Date
        End If
    End Function


    'prev
    'Public Shared Function getdate(ByVal dbt As String) As DateTime
    '    Dim dtArr() As String
    '    dtArr = Split(dbt, "/")
    '    If dtArr.GetUpperBound(0) = 2 Then
    '        Dim dd, mm, yy As String
    '        dd = dtArr(0)
    '        mm = dtArr(1)
    '        yy = dtArr(2)
    '        Dim dt As DateTime
    '        Dim ab As String = mm & "/" & dd & "/" & 20 & yy
    '        Try
    '            dt = Date.ParseExact(ab.ToString(), "dd/mm/yyyy", System.Globalization.DateTimeFormatInfo.InvariantInfo, DateTimeStyles.RoundtripKind)
    '            Return dt
    '        Catch ex As Exception
    '            Return Now.Date
    '        End Try
    '    Else
    '        Return Now.Date
    '    End If
    'End Function

    

    Protected Sub sendMail(ByVal Mto As String, ByVal MSubject As String, ByVal MBody As String)
        Try
            Dim Email As New System.Net.Mail.MailMessage("no-reply@myndsol.com", Mto, MSubject, MBody)
            Dim mailClient As New System.Net.Mail.SmtpClient()
            Email.IsBodyHtml = True
            Dim basicAuthenticationInfo As New System.Net.NetworkCredential("no-reply@myndsol.com", "$up90rt#534")
            mailClient.Host = "mail.myndsol.com"
            mailClient.UseDefaultCredentials = False
            mailClient.Credentials = basicAuthenticationInfo

            mailClient.Send(Email)
        Catch ex As Exception
            Exit Sub
        End Try
    End Sub

    Protected Sub savewslog(ByVal DOCID As Integer, ByVal ERRORTYPE As String, ByVal EID As Integer, ByVal URLSTRING As String, ByVal RESULT As String, ByVal ERRORTRY As Integer, ByValtrycatcherror As String, ByVal doctype As String, ByVal wstypelog As String)
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim da As New SqlDataAdapter("", con)
        Try
            da.SelectCommand.CommandText = "USPINSERT_WSLOG"
            da.SelectCommand.CommandType = CommandType.StoredProcedure
            da.SelectCommand.Parameters.AddWithValue("@eid", EID)
            da.SelectCommand.Parameters.AddWithValue("@DOCID", DOCID)
            da.SelectCommand.Parameters.AddWithValue("@LOGTIME", DateAndTime.Now)
            da.SelectCommand.Parameters.AddWithValue("@ERRORTYPE", ERRORTYPE)
            da.SelectCommand.Parameters.AddWithValue("@URLSTRING", URLSTRING)
            da.SelectCommand.Parameters.AddWithValue("@RESULT", RESULT)
            da.SelectCommand.Parameters.AddWithValue("@ERRORTRY", ERRORTRY)
            da.SelectCommand.Parameters.AddWithValue("@trycatcherror", ByValtrycatcherror)
            da.SelectCommand.Parameters.AddWithValue("@doctype", doctype.ToString())
            da.SelectCommand.Parameters.AddWithValue("@wstype", wstypelog.ToString())

            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            da.SelectCommand.ExecuteNonQuery()


        Catch ex As Exception


        Finally
            If Not con Is Nothing Then
                con.Dispose()
                con.Close()
            End If
            If Not da Is Nothing Then
                da.Dispose()
            End If


        End Try

    End Sub

End Class
