Imports System.Data
Imports System.Data.SqlClient
Imports System.Net
Imports System.IO
Imports System.Xml
Imports System.Collections.Generic
Imports System.Text
Partial Class SMSTest
    Inherits System.Web.UI.Page
    ' Private FPath As String = "D:\MailAttach\"
    Private FPath As String = "D:\VHDJDE\MailAttach\"
    Protected Sub btnCheck_Click(sender As Object, e As EventArgs) Handles btnCheck.Click
        txtResult.Text = MyndVAS(txtNo.Text, txtMSG.Text)
    End Sub

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
    Public Function MyndVAS(ByVal mn As String, ByVal msg As String) As String
        Dim para() As String
        para = msg.Split(",")
        Dim keywd As String = para(0)
        If para.Length < 2 Or keywd.Length() < 1 Then
            'Invalid keyword message
            SendReply("9036", mn, "Dear User, Keyword entered by you is invalid, Please retry with valid keyword.")
            'SendReply("8737", mn, "Dear User, You are not authorized to use this service")
            Return "Error"
            Exit Function
        End If
        'check if keyword is help
        'Since parameter is recieved.. just make entry in log table  
        InsertSMSLog(mn, keywd, msg.Replace(keywd, ""))

        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim da As SqlDataAdapter = New SqlDataAdapter("SELECT * FROM MMM_MST_KEYWORDS where ", con)
        Dim dt As New DataTable()

        If keywd.ToUpper() = "HELP" Then
            Dim hpTopic As String = para(1)
            da.SelectCommand.CommandText = "SELECT helpingmsg from MMM_MST_SMSKEYWORDS where keywordname='" & hpTopic & "'"
            da.Fill(dt)
            If dt.Rows.Count = 1 Then
                SendReply("9037", mn, "Usage of " & hpTopic & ": " & dt.Rows(0).Item("helpingmsg").ToString() & " in exact sequence")
            Else
                SendReply("9036", mn, "Dear User, Keyword entered by you is invalid, Please retry with valid keyword.")
            End If
            da.Dispose()
            con.Close()
            con.Dispose()
            Return "DONE"
            Exit Function
        End If
        'Now lets find out the keyword saved in DataBase
        da.SelectCommand.CommandText = "SELECT * from MMM_MST_SMSKEYWORDS where keywordname='" & keywd & "'"
        da.Fill(dt)
        If dt.Rows.Count <> 1 Then
            SendReply("9036", mn, "Dear User, Keyword entered by you is invalid, Please retry with valid keyword.")
            da.Dispose()
            con.Close()
            con.Dispose()
            Return "ERROR"
            Exit Function
        End If
        If para.Count - 1 <> Val(dt.Rows(0).Item("ParaCount").ToString()) Then
            SendReply("8759", mn, "This keywords require " & dt.Rows(0).Item("ParaCount").ToString() & " parameters and you supplied " & para.Count - 1 & " parameters")
            da.Dispose()
            con.Close()
            con.Dispose()
            Return "ERROR"
            Exit Function
        End If
        'Athorization process
        da.SelectCommand.CommandText = "select count(*) from mmm_sms_settings where keyword='" & keywd & "' and settingtype='Authentication' "
        Dim ds As New DataSet
        Dim cnt As Integer
        If con.State <> ConnectionState.Open Then
            con.Open()
        End If
        cnt = da.SelectCommand.ExecuteScalar()
        If cnt > 0 Then
            da.SelectCommand.CommandText = "select * from mmm_sms_settings where keyword='" & keywd & "' and settingtype='Authentication' "
            da.Fill(ds, "Auth")
            If con.State <> ConnectionState.Open Then
                con.Open()
            End If

            For i As Integer = 0 To ds.Tables("Auth").Rows.Count - 1
                If ds.Tables("Auth").Rows(i).Item("documenttype").ToString.ToUpper = "USER" Then
                    If ds.Tables("Auth").Rows(i).Item("paratype").ToString.ToUpper = "STATIC" Then
                        da.SelectCommand.CommandText = "select count(*) from " & ds.Tables("Auth").Rows(i).Item("TableName").ToString & "  where  " & ds.Tables("Auth").Rows(i).Item("fieldmapping").ToString & "= '" & ds.Tables("Auth").Rows(i).Item("paravalue").ToString & "'"
                    Else
                        If ds.Tables("Auth").Rows(i).Item("paravalue").ToString.ToUpper = "MOBILENO" Then
                            da.SelectCommand.CommandText = "select count(*) from " & ds.Tables("Auth").Rows(i).Item("TableName").ToString & "  where  " & ds.Tables("Auth").Rows(i).Item("fieldmapping").ToString & "= '" & mn & "'"
                        Else
                            da.SelectCommand.CommandText = "select count(*) from " & ds.Tables("Auth").Rows(i).Item("TableName").ToString & "  where  " & ds.Tables("Auth").Rows(i).Item("fieldmapping").ToString & "= '" & para("" & ds.Tables("Auth").Rows(i).Item("paravalue").ToString) & "'"
                        End If
                    End If
                Else
                    If ds.Tables("Auth").Rows(i).Item("paratype").ToString.ToUpper = "STATIC" Then
                        da.SelectCommand.CommandText = "select count(*) from " & ds.Tables("Auth").Rows(i).Item("TableName").ToString & "  where documenttype='" & ds.Tables("Auth").Rows(i).Item("Documenttype").ToString & "' and " & ds.Tables("Auth").Rows(i).Item("fieldmapping").ToString & "= '" & ds.Tables("Auth").Rows(i).Item("paravalue").ToString & "'"
                    Else
                        If ds.Tables("Auth").Rows(i).Item("paravalue").ToString.ToUpper = "MOBILENO" Then
                            da.SelectCommand.CommandText = "select count(*) from " & ds.Tables("Auth").Rows(i).Item("TableName").ToString & "  where documenttype='" & ds.Tables("Auth").Rows(i).Item("Documenttype").ToString & "' and " & ds.Tables("Auth").Rows(i).Item("fieldmapping").ToString & "= '" & mn & "'"
                        Else
                            da.SelectCommand.CommandText = "select count(*) from " & ds.Tables("Auth").Rows(i).Item("TableName").ToString & "  where documenttype='" & ds.Tables("Auth").Rows(i).Item("Documenttype").ToString & "' and " & ds.Tables("Auth").Rows(i).Item("fieldmapping").ToString & "= '" & para("" & ds.Tables("Auth").Rows(i).Item("paravalue").ToString) & "'"
                        End If
                    End If
                End If

                cnt = da.SelectCommand.ExecuteScalar()
                If cnt = 0 Then
                    Return dt.Rows(0).Item("mErrAuth").ToString
                    con.Dispose()
                    Exit Function
                End If
            Next
        End If


        'Existence process
        da.SelectCommand.CommandText = "select count(*) from mmm_sms_settings where keyword='" & keywd & "' and settingtype='Existence' "
        If con.State <> ConnectionState.Open Then
            con.Open()
        End If
        cnt = da.SelectCommand.ExecuteScalar()
        If cnt > 0 Then
            da.SelectCommand.CommandText = "select * from mmm_sms_settings where keyword='" & keywd & "' and settingtype='Existence' "
            da.Fill(ds, "Exist")
            For i As Integer = 0 To ds.Tables("Exist").Rows.Count - 1
                If ds.Tables("Exist").Rows(i).Item("documenttype").ToString.ToUpper = "USER" Then
                    If ds.Tables("Exist").Rows(i).Item("paravalue").ToString.ToUpper = "MOBILENO" Then
                        da.SelectCommand.CommandText = "select count(*) from " & ds.Tables("Exist").Rows(i).Item("TableName").ToString & "  where  " & ds.Tables("Exist").Rows(i).Item("fieldmapping").ToString & "= '" & mn & "'"
                    Else
                        da.SelectCommand.CommandText = "select count(*) from " & ds.Tables("Exist").Rows(i).Item("TableName").ToString & "  where  " & ds.Tables("Exist").Rows(i).Item("fieldmapping").ToString & "= '" & para("" & ds.Tables("Exist").Rows(i).Item("paravalue").ToString) & "'"
                    End If
                Else
                    If ds.Tables("Exist").Rows(i).Item("paravalue").ToString.ToUpper = "MOBILENO" Then
                        da.SelectCommand.CommandText = "select count(*) from " & ds.Tables("Exist").Rows(i).Item("TableName").ToString & "  where documenttype='" & ds.Tables("Exist").Rows(i).Item("Documenttype").ToString & "' and " & ds.Tables("Exist").Rows(i).Item("fieldmapping").ToString & "= '" & mn & "'"
                    Else
                        da.SelectCommand.CommandText = "select count(*) from " & ds.Tables("Exist").Rows(i).Item("TableName").ToString & "  where documenttype='" & ds.Tables("Exist").Rows(i).Item("Documenttype").ToString & "' and " & ds.Tables("Exist").Rows(i).Item("fieldmapping").ToString & "= '" & para("" & ds.Tables("Exist").Rows(i).Item("paravalue").ToString) & "'"
                    End If
                End If
                cnt = da.SelectCommand.ExecuteScalar()
                If cnt = 1 Then
                    Return dt.Rows(0).Item("mErrExist").ToString
                    con.Dispose()
                    Exit Function
                End If
            Next
        End If


        'Processing
        da.SelectCommand.CommandText = "select count(*) from mmm_sms_settings where keyword='" & keywd & "' and settingtype in ('Processing','Where') "
        If con.State <> ConnectionState.Open Then
            con.Open()
        End If
        cnt = da.SelectCommand.ExecuteScalar()
        Dim insfld As String
        Dim insval As String
        Dim Updateval As String
        Dim cnd As String
        If cnt > 0 Then
            da.SelectCommand.CommandText = "select * from mmm_sms_settings where keyword='" & keywd & "' and settingtype in ('Processing','Where') ORDER BY SETTINGTYPE "
            da.Fill(ds, "Process")
            For j As Integer = 0 To ds.Tables("Process").Rows.Count - 1
                If ds.Tables("Process").Rows(j).Item("ProcType").ToString.ToUpper = "INSERT" Then
                    insfld = insfld & ds.Tables("Process").Rows(j).Item("fieldmapping").ToString & ","
                    If ds.Tables("Process").Rows(j).Item("paravalue").ToString.ToUpper = "MOBILENO" Then
                        insval = insval & "'" & mn & "'" & ","
                    ElseIf ds.Tables("Process").Rows(j).Item("ParaType").ToString.ToUpper = "STATIC" Then
                        insval = insval & "'" & ds.Tables("Process").Rows(j).Item("paravalue").ToString & "'" & ","
                    Else
                        insval = insval & "'" & para("" & ds.Tables("Process").Rows(j).Item("paravalue").ToString) & "'" & ","
                    End If
                ElseIf ds.Tables("Process").Rows(j).Item("SettingType").ToString.ToUpper = "PROCESSING" And ds.Tables("Process").Rows(j).Item("CType").ToString.ToUpper = "APPEND" Then
                    If ds.Tables("Process").Rows(j).Item("ParaValue").ToString.ToUpper = "MOBILENO" Then
                        Updateval = Updateval & ds.Tables("Process").Rows(j).Item("fieldmapping").ToString & "= " & ds.Tables("Process").Rows(j).Item("fieldmapping").ToString & " + '," & mn & "'" & ","
                    ElseIf ds.Tables("Process").Rows(j).Item("ParaType").ToString.ToUpper = "STATIC" Then
                        Updateval = Updateval & ds.Tables("Process").Rows(j).Item("fieldmapping").ToString & "= " & ds.Tables("Process").Rows(j).Item("fieldmapping").ToString & " + '," & ds.Tables("Process").Rows(j).Item("paravalue").ToString & "'" & ","
                    Else
                        Updateval = Updateval & ds.Tables("Process").Rows(j).Item("fieldmapping").ToString & "= " & ds.Tables("Process").Rows(j).Item("fieldmapping").ToString & " + '," & para("" & ds.Tables("Process").Rows(j).Item("paravalue").ToString) & "'" & ","
                    End If
                ElseIf ds.Tables("Process").Rows(j).Item("SettingType").ToString.ToUpper = "PROCESSING" And ds.Tables("Process").Rows(j).Item("CType").ToString.ToUpper = "=" Then
                    If ds.Tables("Process").Rows(j).Item("ParaValue").ToString.ToUpper = "MOBILENO" Then
                        Updateval = Updateval & ds.Tables("Process").Rows(j).Item("fieldmapping").ToString & "= '" & mn & "'" & ","
                    ElseIf ds.Tables("Process").Rows(j).Item("ParaType").ToString.ToUpper = "STATIC" Then
                        Updateval = Updateval & ds.Tables("Process").Rows(j).Item("fieldmapping").ToString & "= '" & ds.Tables("Process").Rows(j).Item("paravalue").ToString & "'" & ","
                    Else
                        Updateval = Updateval & ds.Tables("Process").Rows(j).Item("fieldmapping").ToString & "= '" & para("" & ds.Tables("Process").Rows(j).Item("paravalue").ToString) & "'" & ","
                    End If
                ElseIf ds.Tables("Process").Rows(j).Item("SettingType").ToString.ToUpper = "PROCESSING" And ds.Tables("Process").Rows(j).Item("CType").ToString.ToUpper = "REMOVE" Then
                    If ds.Tables("Process").Rows(j).Item("ParaValue").ToString.ToUpper = "MOBILENO" Then
                        Updateval = Updateval & ds.Tables("Process").Rows(j).Item("fieldmapping").ToString & "= '" & mn & "'" & ","
                    ElseIf ds.Tables("Process").Rows(j).Item("ParaType").ToString.ToUpper = "STATIC" Then
                        Updateval = Updateval & ds.Tables("Process").Rows(j).Item("fieldmapping").ToString & "= '" & ds.Tables("Process").Rows(j).Item("paravalue").ToString & "'" & ","
                    Else
                        Updateval = Updateval & ds.Tables("Process").Rows(j).Item("fieldmapping").ToString & "= replace(" & ds.Tables("Process").Rows(j).Item("fieldmapping").ToString & ",'" & para("" & ds.Tables("Process").Rows(j).Item("paravalue").ToString) & "','')" & ","
                    End If
                ElseIf ds.Tables("Process").Rows(j).Item("SettingType").ToString.ToUpper = "WHERE" Then
                    If ds.Tables("Process").Rows(j).Item("ParaValue").ToString.ToUpper = "MOBILENO" Then
                        cnd = cnd & ds.Tables("Process").Rows(j).Item("fieldmapping").ToString & "= '" & mn & "' and "
                    ElseIf ds.Tables("Process").Rows(j).Item("ParaType").ToString.ToUpper = "STATIC" Then
                        cnd = cnd & ds.Tables("Process").Rows(j).Item("fieldmapping").ToString & "= '" & ds.Tables("Process").Rows(j).Item("paravalue").ToString & "' and "
                    Else
                        cnd = cnd & ds.Tables("Process").Rows(j).Item("fieldmapping").ToString & "= '" & para("" & ds.Tables("Process").Rows(j).Item("paravalue").ToString) & "' and "
                    End If
                End If
            Next

            If Updateval.ToString <> "" Then
                Updateval = Left(Updateval, Updateval.Length - 1)
            End If
            If cnd.ToString <> "" Then
                cnd = Left(cnd, cnd.Length - 4)
            End If

            For i As Integer = ds.Tables("Process").Rows.Count - 1 To ds.Tables("Process").Rows.Count - 1
                If ds.Tables("Process").Rows(i).Item("ProcType").ToString.ToUpper = "UPDATE" And ds.Tables("Process").Rows(i).Item("SettingType").ToString.ToUpper = "WHERE" Then
                    If ds.Tables("Process").Rows(i).Item("documenttype").ToString.ToUpper = "USER" Then
                        da.SelectCommand.CommandText = "" & ds.Tables("Process").Rows(i).Item("proctype") & "  " & ds.Tables("Process").Rows(i).Item("TableName").ToString & " set  " & Updateval & " where " & cnd
                    Else
                        da.SelectCommand.CommandText = "" & ds.Tables("Process").Rows(i).Item("proctype") & "  " & ds.Tables("Process").Rows(i).Item("TableName").ToString & " set  " & Updateval & " where " & cnd & "  and   documenttype='" & ds.Tables("process").Rows(i).Item("Documenttype").ToString & "'"
                    End If
                ElseIf ds.Tables("Process").Rows(i).Item("ProcType").ToString.ToUpper = "INSERT" Then
                    If insfld.Length > 1 And insval.Length > 1 Then
                        insfld = Left(insfld, insfld.Length - 1)
                        insval = Left(insval, insval.Length - 1)
                    End If
                    If ds.Tables("Process").Rows(i).Item("documenttype").ToString.ToUpper = "USER" Then
                        da.SelectCommand.CommandText = "" & ds.Tables("Process").Rows(i).Item("proctype") & " into  " & ds.Tables("Process").Rows(i).Item("TableName").ToString & " (" & insfld & ") values (" & insval & ")"
                    ElseIf ds.Tables("Process").Rows(i).Item("TableName").ToString.ToUpper = "M_MST_MASTER" Then
                        da.SelectCommand.CommandText = "" & ds.Tables("Process").Rows(i).Item("proctype") & " into " & ds.Tables("Process").Rows(i).Item("TableName").ToString & " ( " & insfld & ",documenttype,eid,createdby,updateddate ) values (" & insval & ",'" & ds.Tables("process").Rows(i).Item("Documenttype").ToString & "'," & Session("EID") & "," & Session("UID") & ",getdate())"
                    Else

                    End If
                End If
            Next
            Try
                da.SelectCommand.ExecuteNonQuery()
                Return dt.Rows(0).Item("msuccess").ToString
            Catch ex As Exception
                Return dt.Rows(0).Item("mErrProcess").ToString
                con.Dispose()
                Exit Function
            End Try
        End If
        con.Dispose()
    End Function
    Private Sub SendReply(templateID As String, MobileNumber As String, ByVal msg As String)
        Dim msgString As String = "http://smsalertbox.com/api/sms.php?uid=6d796e646270&pin=51f36abe9f80a&sender=MYNDBP&route=5&tempid=" & templateID & "&mobile=" & MobileNumber & "&message=" & msg & "&pushid=1"
        Dim result As String = apicall(msgString)
    End Sub
    Public Sub InsertSMSLog(ByVal sendorNumer As String, ByVal keyword As String, ByVal msgtext As String)
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("sp_InsertSMSLog", con)
        oda.SelectCommand.CommandType = CommandType.StoredProcedure
        oda.SelectCommand.Parameters.AddWithValue("SendorNumber", sendorNumer)
        oda.SelectCommand.Parameters.AddWithValue("Keyword", keyword)
        oda.SelectCommand.Parameters.AddWithValue("MsgText", msgtext)
        If con.State <> ConnectionState.Open Then
            con.Open()
        End If
        Dim iSt As Integer = oda.SelectCommand.ExecuteScalar()
        con.Close()
        oda.Dispose()
        con.Dispose()
    End Sub
    Protected Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        TripCreateFromSwitch()
    End Sub
    Public Sub TripCreateFromSwitch()

        Dim constr As String = "server=172.17.109.152;initial catalog=DMS;uid=DMS;pwd=Ztsu93#u"
        Dim con As New SqlConnection(constr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
        Dim ds As New DataSet()
        Dim dt As New DataTable
        Dim dt1 As New DataTable
        Try
            'oda.SelectCommand.CommandText = "select count(*) from MMM_MST_ELOGBOOK where convert(date,trip_start_datetime)=convert(nvarchar,getdate()-1,23) and triptype='switch'"
            'If con.State <> ConnectionState.Open Then
            '    con.Open()
            'End If
            'Dim cnt As Integer = oda.SelectCommand.ExecuteScalar()
            Dim cnt As Integer = 0
            If cnt = 0 Then
                oda.SelectCommand.CommandText = "select distinct imieno,CONVERT(nvarchar,ctime,23)[cdate] from mmm_mst_gpsdata where tripon=1 and convert(nvarchar,ctime,23)='" & txtsdate.Text & "' group by imieno,CONVERT(nvarchar,ctime,23)"
                oda.Fill(dt)
                For i As Integer = 0 To dt.Rows.Count - 1
                    oda.SelectCommand.CommandText = "CreateTripfromSwitchNew"
                    oda.SelectCommand.CommandType = CommandType.StoredProcedure
                    oda.SelectCommand.Parameters.Clear()
                    oda.SelectCommand.Parameters.AddWithValue("@imieno", dt.Rows(i).Item("imieno").ToString)
                    oda.SelectCommand.Parameters.AddWithValue("Date", dt.Rows(i).Item("cdate").ToString())
                    If con.State <> ConnectionState.Open Then
                        con.Open()
                    End If
                    oda.SelectCommand.CommandTimeout = 600
                    oda.SelectCommand.ExecuteNonQuery()
                    'oda.SelectCommand.CommandType = CommandType.Text
                    'oda.SelectCommand.CommandText = "select tid,start_latitude,start_longitude,end_latitude,end_longitude from mmm_mst_elogbook where imei_no='" & dt.Rows(i).Item("imieno").ToString & "' and Trip_start_location is null and Trip_end_location is null and convert(nvarchar,trip_start_datetime,23)='" & dt.Rows(i).Item("cdate").ToString & "' and convert(nvarchar,trip_end_datetime,23)='" & dt.Rows(i).Item("cdate").ToString & "'"
                    'oda.Fill(dt1)
                    'For j As Integer = 0 To dt1.Rows.Count - 1
                    '    oda.SelectCommand.CommandText = "USPUpdateTripLocationNew"
                    '    oda.SelectCommand.CommandType = CommandType.StoredProcedure
                    '    oda.SelectCommand.Parameters.Clear()
                    '    oda.SelectCommand.Parameters.AddWithValue("@tid", dt1.Rows(j).Item("tid").ToString)
                    '    'oda.SelectCommand.Parameters.AddWithValue("@stlocation", Location(dt1.Rows(j).Item("start_latitude"), dt1.Rows(j).Item("start_longitude")))
                    '    'oda.SelectCommand.Parameters.AddWithValue("@edlocation", Location(dt1.Rows(j).Item("end_latitude"), dt1.Rows(j).Item("end_longitude")))
                    '    oda.SelectCommand.ExecuteNonQuery()
                    'Next
                Next
                oda.SelectCommand.CommandType = CommandType.Text
                'oda.SelectCommand.CommandText = "insert into mmm_schedulerlog_Switch values ('Window Service END Trip Created By Switch',getdate())"
                'If con.State <> ConnectionState.Open Then
                '    con.Open()
                'End If
                'oda.SelectCommand.ExecuteNonQuery()
            End If
            lblmsg.Text = "Trip created successfully."
        Catch ex As Exception
            oda.SelectCommand.CommandType = CommandType.Text
            ' oda.SelectCommand.CommandText = "insert into mmm_schedulerlog_Switch values ('" & ex.ToString & "',getdate())"
            'If con.State <> ConnectionState.Open Then
            '    con.Open()
            'End If
            'oda.SelectCommand.ExecuteNonQuery()
            con.Dispose()
            con.Close()
            oda.Dispose()
        Finally
            con.Dispose()
            con.Close()
            oda.Dispose()
        End Try

    End Sub
    'Public Function Location(lat As String, log As String) As String
    '    Dim constr As String = "server=172.17.109.152;initial catalog=DMS;uid=DMS;pwd=Ztsu93#u"
    '    Dim con As New SqlConnection(constr)
    '    Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
    '    If lat = "" And log = "" Then
    '        If Not con Is Nothing Then
    '            con.Close()
    '        End If
    '        Return Nothing
    '        Exit Function
    '    End If
    '    oda.SelectCommand.CommandText = "select top 1 * from gpsLocation where Lat_start  <=" + lat + " and  lat_end >= " + lat + " and long_start <= " + log + " and long_end >= " + log + " "
    '    Dim locatoinr As DataTable = New DataTable()
    '    oda.Fill(locatoinr)
    '    If locatoinr.Rows.Count > 0 Then
    '        con.Close()
    '        Return locatoinr.Rows(0).Item(1).ToString
    '        Exit Function
    '    Else
    '        Dim url As String = "http://maps.googleapis.com/maps/api/geocode/xml?latlng=" & lat & "," & log & "&sensor=false"
    '        Dim request As HttpWebRequest = DirectCast(WebRequest.Create(url), HttpWebRequest)
    '        Dim response As WebResponse = request.GetResponse()
    '        Dim dataStream As Stream = response.GetResponseStream()
    '        Dim sreader As New StreamReader(dataStream)
    '        Dim responsereader As String = sreader.ReadToEnd()
    '        response.Close()
    '        Dim xmldoc As New XmlDocument()
    '        xmldoc.LoadXml(responsereader)
    '        If xmldoc.GetElementsByTagName("status")(0).ChildNodes(0).InnerText = "OK" Then
    '            oda.SelectCommand.CommandText = "gpsinsertlocation"
    '            oda.SelectCommand.CommandType = CommandType.StoredProcedure
    '            oda.SelectCommand.Parameters.AddWithValue("complete_latitude", lat)
    '            oda.SelectCommand.Parameters.AddWithValue("complete_longitude", log)
    '            oda.SelectCommand.Parameters.AddWithValue("Lat_start", Convert.ToDouble(lat.Substring(0, 5)))
    '            oda.SelectCommand.Parameters.AddWithValue("lat_end", Convert.ToDouble(lat.Substring(0, 5)) + 0.01)
    '            oda.SelectCommand.Parameters.AddWithValue("long_start", Convert.ToDouble(log.Substring(0, 5)))
    '            oda.SelectCommand.Parameters.AddWithValue("long_end", Convert.ToDouble(log.Substring(0, 5)) + 0.01)
    '            Dim fulladdress As String = String.Empty
    '            Try
    '                If xmldoc.ChildNodes.Count > 0 Then
    '                    Dim SelNodesTxt As String = xmldoc.DocumentElement.Name
    '                    Dim Cnt As Integer = 0
    '                    Dim nodes As XmlNodeList = xmldoc.SelectNodes(SelNodesTxt)

    '                    Dim other As Int32 = 0
    '                    For Each node As XmlNode In nodes
    '                        For c As Integer = 0 To node.ChildNodes.Count - 1
    '                            If node.ChildNodes(c).Name = "result" Then
    '                                Cnt += 1
    '                                For c2 As Integer = 0 To node.ChildNodes.Item(c).ChildNodes.Count - 1
    '                                    If node.ChildNodes.Item(c).ChildNodes.Item(c2).Name = "address_component" Then
    '                                        For j As Integer = 0 To node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Count - 1
    '                                            If node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Count = 2 And other = 0 Then
    '                                                oda.SelectCommand.Parameters.AddWithValue("other", node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).InnerText)
    '                                                other = 1
    '                                            End If
    '                                            If node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).Name = "type" Then
    '                                                If node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).InnerXml = "street_address" Then
    '                                                    oda.SelectCommand.Parameters.AddWithValue("street_address", node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j - 2).InnerText) '''local address
    '                                                End If

    '                                                If node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).InnerXml = "floor" Then
    '                                                    oda.SelectCommand.Parameters.AddWithValue("floor", node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j - 2).InnerText) '''local address
    '                                                End If
    '                                                If node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).InnerXml = "parking" Then
    '                                                    oda.SelectCommand.Parameters.AddWithValue("parking", node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j - 2).InnerText) '''local address
    '                                                End If
    '                                                If node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).InnerXml = "post_box" Then
    '                                                    oda.SelectCommand.Parameters.AddWithValue("post_box", node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j - 2).InnerText) '''local address
    '                                                End If
    '                                                If node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).InnerXml = "postal_town" Then
    '                                                    oda.SelectCommand.Parameters.AddWithValue("postal_town", node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j - 2).InnerText) '''local address
    '                                                End If
    '                                                If node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).InnerXml = "room" Then
    '                                                    oda.SelectCommand.Parameters.AddWithValue("room", node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j - 2).InnerText) '''local address
    '                                                End If
    '                                                If node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).InnerXml = "train_station" Then
    '                                                    oda.SelectCommand.Parameters.AddWithValue("train_station", node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j - 2).InnerText) '''local address
    '                                                End If
    '                                                If node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).InnerXml = "establishment" Then
    '                                                    oda.SelectCommand.Parameters.AddWithValue("establishment_address", node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j - 2).InnerText) '''local address
    '                                                End If
    '                                                If node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).InnerXml = "street_number" Then
    '                                                    oda.SelectCommand.Parameters.AddWithValue("street_number", node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j - 2).InnerText) '''local address
    '                                                End If
    '                                                If node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).InnerXml = "bus_station" Then
    '                                                    oda.SelectCommand.Parameters.AddWithValue("stationaddress", node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j - 2).InnerText) '''local address
    '                                                End If
    '                                                If node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).InnerXml = "route" Then
    '                                                    oda.SelectCommand.Parameters.AddWithValue("rld", node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j - 2).InnerText) '''local address
    '                                                End If
    '                                                If node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).InnerXml = "neighborhood" Then
    '                                                    oda.SelectCommand.Parameters.AddWithValue("npa", node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j - 2).InnerText)  ''neighborhood address
    '                                                End If
    '                                                If node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).InnerXml = "sublocality" Then
    '                                                    oda.SelectCommand.Parameters.AddWithValue("sublocalityaddress", node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j - 2).InnerText)
    '                                                End If
    '                                                If node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).InnerXml = "locality" Then
    '                                                    oda.SelectCommand.Parameters.AddWithValue("locPaddre", node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j - 2).InnerText)   ''locality
    '                                                End If
    '                                                If node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).InnerXml = "administrative_area_level_3" Then
    '                                                    oda.SelectCommand.Parameters.AddWithValue("admini3address", node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j - 2).InnerText)   ''city
    '                                                End If
    '                                                If node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).InnerXml = "administrative_area_level_2" Then
    '                                                    oda.SelectCommand.Parameters.AddWithValue("adminpoladdress", node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j - 2).InnerText)   ''city
    '                                                End If
    '                                                If node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).InnerXml = "administrative_area_level_1" Then
    '                                                    oda.SelectCommand.Parameters.AddWithValue("addressLongName", node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j - 2).InnerText)   ''city
    '                                                    oda.SelectCommand.Parameters.AddWithValue("addShortName", node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j - 1).InnerText)
    '                                                End If
    '                                                If node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).InnerXml = "country" Then
    '                                                    oda.SelectCommand.Parameters.AddWithValue("countryLong", node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j - 2).InnerText)
    '                                                    oda.SelectCommand.Parameters.AddWithValue("countryShort", node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j - 1).InnerText)
    '                                                End If
    '                                                If node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).InnerXml = "postal_code" Then
    '                                                    oda.SelectCommand.Parameters.AddWithValue("postalLong", node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j - 2).InnerText)
    '                                                    oda.SelectCommand.Parameters.AddWithValue("postalShort", node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j - 1).InnerText)
    '                                                End If
    '                                                If node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).InnerXml = "airport" Then
    '                                                    oda.SelectCommand.Parameters.AddWithValue("airport", node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j - 2).InnerText) '''local address
    '                                                End If
    '                                                If node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).InnerXml = "point_of_interest" Then
    '                                                    oda.SelectCommand.Parameters.AddWithValue("point_of_interest", node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j - 2).InnerText) '''local address
    '                                                End If
    '                                                If node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).InnerXml = "park" Then
    '                                                    oda.SelectCommand.Parameters.AddWithValue("park", node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j - 2).InnerText) '''local address
    '                                                End If
    '                                                If node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).InnerXml = "intersection" Then
    '                                                    oda.SelectCommand.Parameters.AddWithValue("intersection", node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j - 2).InnerText) '''local address
    '                                                End If
    '                                                If node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).InnerXml = "colloquial_area" Then
    '                                                    oda.SelectCommand.Parameters.AddWithValue("colloquial_area", node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j - 2).InnerText) '''local address
    '                                                End If
    '                                                If node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).InnerXml = "premise" Then
    '                                                    oda.SelectCommand.Parameters.AddWithValue("premise", node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j - 2).InnerText) '''local address
    '                                                End If
    '                                                If node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).InnerXml = "subpremise" Then
    '                                                    oda.SelectCommand.Parameters.AddWithValue("subpremise", node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j - 2).InnerText) '''local address
    '                                                End If
    '                                            End If
    '                                        Next
    '                                    ElseIf node.ChildNodes.Item(c).ChildNodes.Item(c2).Name = "formatted_address" Then
    '                                        fulladdress = node.ChildNodes.Item(c).ChildNodes.Item(c2).InnerText
    '                                        oda.SelectCommand.Parameters.AddWithValue("location_namer", node.ChildNodes.Item(c).ChildNodes.Item(c2).InnerText)
    '                                    ElseIf node.ChildNodes.Item(c).ChildNodes.Item(c2).Name = "geometry" Then
    '                                        For j As Integer = 0 To node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Count - 1
    '                                            If node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).Name = "location" Then
    '                                                For k As Integer = 0 To node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).ChildNodes.Count - 1
    '                                                    If node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).ChildNodes.Item(k).Name = "lat" Then
    '                                                        oda.SelectCommand.Parameters.AddWithValue("geometrylat", node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).ChildNodes.Item(k).InnerText)
    '                                                    ElseIf node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).ChildNodes.Item(k).Name = "lng" Then
    '                                                        oda.SelectCommand.Parameters.AddWithValue("geometrylng", node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).ChildNodes.Item(k).InnerText)
    '                                                    End If
    '                                                Next
    '                                            ElseIf node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).Name = "viewport" Then
    '                                                For k As Integer = 0 To node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).ChildNodes.Count - 1
    '                                                    If node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).ChildNodes.Item(k).Name = "southwest" Then
    '                                                        For l As Integer = 0 To node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).ChildNodes.Item(k).ChildNodes.Count - 1
    '                                                            If node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).ChildNodes.Item(k).ChildNodes.Item(l).Name = "lat" Then
    '                                                                oda.SelectCommand.Parameters.AddWithValue("vSWlat", node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).ChildNodes.Item(k).ChildNodes.Item(l).InnerText)
    '                                                            ElseIf node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).ChildNodes.Item(k).ChildNodes.Item(l).Name = "lng" Then
    '                                                                oda.SelectCommand.Parameters.AddWithValue("vSWlng", node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).ChildNodes.Item(k).ChildNodes.Item(l).InnerText)
    '                                                            End If
    '                                                        Next
    '                                                    ElseIf node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).ChildNodes.Item(k).Name = "northeast" Then
    '                                                        For l As Integer = 0 To node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).ChildNodes.Item(k).ChildNodes.Count - 1
    '                                                            If node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).ChildNodes.Item(k).ChildNodes.Item(l).Name = "lat" Then
    '                                                                oda.SelectCommand.Parameters.AddWithValue("vNElat", node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).ChildNodes.Item(k).ChildNodes.Item(l).InnerText)
    '                                                            ElseIf node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).ChildNodes.Item(k).ChildNodes.Item(l).Name = "lng" Then
    '                                                                oda.SelectCommand.Parameters.AddWithValue("vNElng", node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).ChildNodes.Item(k).ChildNodes.Item(l).InnerText)
    '                                                            End If
    '                                                        Next
    '                                                    End If
    '                                                Next
    '                                            ElseIf node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).Name = "bounds" Then
    '                                                For k As Integer = 0 To node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).ChildNodes.Count - 1
    '                                                    If node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).ChildNodes.Item(k).Name = "southwest" Then
    '                                                        For l As Integer = 0 To node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).ChildNodes.Item(k).ChildNodes.Count - 1
    '                                                            If node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).ChildNodes.Item(k).ChildNodes.Item(l).Name = "lat" Then
    '                                                                oda.SelectCommand.Parameters.AddWithValue("bSWlat", node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).ChildNodes.Item(k).ChildNodes.Item(l).InnerText)
    '                                                            ElseIf node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).ChildNodes.Item(k).ChildNodes.Item(l).Name = "lng" Then
    '                                                                oda.SelectCommand.Parameters.AddWithValue("bSWlng", node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).ChildNodes.Item(k).ChildNodes.Item(l).InnerText)
    '                                                            End If
    '                                                        Next
    '                                                    ElseIf node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).ChildNodes.Item(k).Name = "northeast" Then
    '                                                        For l As Integer = 0 To node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).ChildNodes.Item(k).ChildNodes.Count - 1
    '                                                            If node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).ChildNodes.Item(k).ChildNodes.Item(l).Name = "lat" Then
    '                                                                oda.SelectCommand.Parameters.AddWithValue("bNElat", node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).ChildNodes.Item(k).ChildNodes.Item(l).InnerText)
    '                                                            ElseIf node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).ChildNodes.Item(k).ChildNodes.Item(l).Name = "lng" Then
    '                                                                oda.SelectCommand.Parameters.AddWithValue("bNElng", node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).ChildNodes.Item(k).ChildNodes.Item(l).InnerText)
    '                                                            End If
    '                                                        Next
    '                                                    End If
    '                                                Next
    '                                            End If
    '                                        Next
    '                                    End If
    '                                Next
    '                            End If
    '                            If Cnt = 1 Then
    '                                Exit For
    '                            End If
    '                        Next
    '                    Next
    '                End If
    '                If con.State <> ConnectionState.Open Then
    '                    con.Open()
    '                End If
    '                oda.SelectCommand.ExecuteNonQuery()
    '                oda.Dispose()
    '                locatoinr.Dispose()
    '            Catch ex As Exception

    '            Finally
    '                con.Dispose()
    '                oda.Dispose()
    '            End Try
    '            Return fulladdress
    '        End If
    '    End If
    '    Return Nothing
    'End Function
    Public Function Location(ByVal lat As String, ByVal log As String) As String
        Dim connectionString As String = "server=172.17.109.152;initial catalog=DMS;uid=DMS;pwd=Ztsu93#u"
        Dim selectConnection As New SqlConnection(connectionString)
        Dim adapter As New SqlDataAdapter("", selectConnection)
        If ((lat = "") And (log = "")) Then
            If (Not selectConnection Is Nothing) Then
                selectConnection.Close()
            End If
            Return Nothing
        End If
        adapter.SelectCommand.CommandText = String.Concat(New String() {"select top 1 * from gpsLocation where Lat_start  <=", lat, " and  lat_end >= ", lat, " and long_start <= ", log, " and long_end >= ", log, " "})
        Dim dataTable As New DataTable
        adapter.Fill(dataTable)
        If (dataTable.Rows.Count > 0) Then
            Return dataTable.Rows.Item(0).Item(1).ToString
        End If
        Dim response As WebResponse = DirectCast(WebRequest.Create(String.Concat(New String() {"http://maps.googleapis.com/maps/api/geocode/xml?latlng=", lat, ",", log, "&sensor=false"})), HttpWebRequest).GetResponse
        Dim xml As String = New StreamReader(response.GetResponseStream).ReadToEnd
        response.Close()
        Dim document As New XmlDocument
        document.LoadXml(xml)
        If (document.GetElementsByTagName("status").ItemOf(0).ChildNodes.ItemOf(0).InnerText = "OK") Then
            adapter.SelectCommand.CommandText = "gpsinsertlocation"
            adapter.SelectCommand.CommandType = CommandType.StoredProcedure
            adapter.SelectCommand.Parameters.AddWithValue("complete_latitude", lat)
            adapter.SelectCommand.Parameters.AddWithValue("complete_longitude", log)
            adapter.SelectCommand.Parameters.AddWithValue("Lat_start", Convert.ToDouble(lat.Substring(0, 5)))
            adapter.SelectCommand.Parameters.AddWithValue("lat_end", (Convert.ToDouble(lat.Substring(0, 5)) + 0.01))
            adapter.SelectCommand.Parameters.AddWithValue("long_start", Convert.ToDouble(log.Substring(0, 5)))
            adapter.SelectCommand.Parameters.AddWithValue("long_end", (Convert.ToDouble(log.Substring(0, 5)) + 0.01))
            Dim innerText As String = String.Empty
            Try
                If (document.ChildNodes.Count > 0) Then
                    Dim enumerator As IEnumerator
                    Dim name As String = document.DocumentElement.Name
                    Dim num As Integer = 0
                    Dim list As XmlNodeList = document.SelectNodes(name)
                    Dim num2 As Integer = 0
                    Try
                        enumerator = list.GetEnumerator
                        Do While enumerator.MoveNext
                            Dim current As XmlNode = DirectCast(enumerator.Current, XmlNode)
                            Dim num14 As Integer = (current.ChildNodes.Count - 1)
                            Dim i As Integer = 0
                            Do While (i <= num14)
                                If (current.ChildNodes.ItemOf(i).Name = "result") Then
                                    num += 1
                                    Dim num15 As Integer = (current.ChildNodes.Item(i).ChildNodes.Count - 1)
                                    Dim j As Integer = 0
                                    Do While (j <= num15)
                                        If (current.ChildNodes.Item(i).ChildNodes.Item(j).Name = "address_component") Then
                                            Dim num16 As Integer = (current.ChildNodes.Item(i).ChildNodes.Item(j).ChildNodes.Count - 1)
                                            Dim k As Integer = 0
                                            Do While (k <= num16)
                                                If ((current.ChildNodes.Item(i).ChildNodes.Item(j).ChildNodes.Count = 2) And (num2 = 0)) Then
                                                    adapter.SelectCommand.Parameters.AddWithValue("other", current.ChildNodes.Item(i).ChildNodes.Item(j).ChildNodes.Item(k).InnerText)
                                                    num2 = 1
                                                End If
                                                If (current.ChildNodes.Item(i).ChildNodes.Item(j).ChildNodes.Item(k).Name = "type") Then
                                                    If (current.ChildNodes.Item(i).ChildNodes.Item(j).ChildNodes.Item(k).InnerXml = "street_address") Then
                                                        adapter.SelectCommand.Parameters.AddWithValue("street_address", current.ChildNodes.Item(i).ChildNodes.Item(j).ChildNodes.Item((k - 2)).InnerText)
                                                    End If
                                                    If (current.ChildNodes.Item(i).ChildNodes.Item(j).ChildNodes.Item(k).InnerXml = "floor") Then
                                                        adapter.SelectCommand.Parameters.AddWithValue("floor", current.ChildNodes.Item(i).ChildNodes.Item(j).ChildNodes.Item((k - 2)).InnerText)
                                                    End If
                                                    If (current.ChildNodes.Item(i).ChildNodes.Item(j).ChildNodes.Item(k).InnerXml = "parking") Then
                                                        adapter.SelectCommand.Parameters.AddWithValue("parking", current.ChildNodes.Item(i).ChildNodes.Item(j).ChildNodes.Item((k - 2)).InnerText)
                                                    End If
                                                    If (current.ChildNodes.Item(i).ChildNodes.Item(j).ChildNodes.Item(k).InnerXml = "post_box") Then
                                                        adapter.SelectCommand.Parameters.AddWithValue("post_box", current.ChildNodes.Item(i).ChildNodes.Item(j).ChildNodes.Item((k - 2)).InnerText)
                                                    End If
                                                    If (current.ChildNodes.Item(i).ChildNodes.Item(j).ChildNodes.Item(k).InnerXml = "postal_town") Then
                                                        adapter.SelectCommand.Parameters.AddWithValue("postal_town", current.ChildNodes.Item(i).ChildNodes.Item(j).ChildNodes.Item((k - 2)).InnerText)
                                                    End If
                                                    If (current.ChildNodes.Item(i).ChildNodes.Item(j).ChildNodes.Item(k).InnerXml = "room") Then
                                                        adapter.SelectCommand.Parameters.AddWithValue("room", current.ChildNodes.Item(i).ChildNodes.Item(j).ChildNodes.Item((k - 2)).InnerText)
                                                    End If
                                                    If (current.ChildNodes.Item(i).ChildNodes.Item(j).ChildNodes.Item(k).InnerXml = "train_station") Then
                                                        adapter.SelectCommand.Parameters.AddWithValue("train_station", current.ChildNodes.Item(i).ChildNodes.Item(j).ChildNodes.Item((k - 2)).InnerText)
                                                    End If
                                                    If (current.ChildNodes.Item(i).ChildNodes.Item(j).ChildNodes.Item(k).InnerXml = "establishment") Then
                                                        adapter.SelectCommand.Parameters.AddWithValue("establishment_address", current.ChildNodes.Item(i).ChildNodes.Item(j).ChildNodes.Item((k - 2)).InnerText)
                                                    End If
                                                    If (current.ChildNodes.Item(i).ChildNodes.Item(j).ChildNodes.Item(k).InnerXml = "street_number") Then
                                                        adapter.SelectCommand.Parameters.AddWithValue("street_number", current.ChildNodes.Item(i).ChildNodes.Item(j).ChildNodes.Item((k - 2)).InnerText)
                                                    End If
                                                    If (current.ChildNodes.Item(i).ChildNodes.Item(j).ChildNodes.Item(k).InnerXml = "bus_station") Then
                                                        adapter.SelectCommand.Parameters.AddWithValue("stationaddress", current.ChildNodes.Item(i).ChildNodes.Item(j).ChildNodes.Item((k - 2)).InnerText)
                                                    End If
                                                    If (current.ChildNodes.Item(i).ChildNodes.Item(j).ChildNodes.Item(k).InnerXml = "route") Then
                                                        adapter.SelectCommand.Parameters.AddWithValue("rld", current.ChildNodes.Item(i).ChildNodes.Item(j).ChildNodes.Item((k - 2)).InnerText)
                                                    End If
                                                    If (current.ChildNodes.Item(i).ChildNodes.Item(j).ChildNodes.Item(k).InnerXml = "neighborhood") Then
                                                        adapter.SelectCommand.Parameters.AddWithValue("npa", current.ChildNodes.Item(i).ChildNodes.Item(j).ChildNodes.Item((k - 2)).InnerText)
                                                    End If
                                                    If (current.ChildNodes.Item(i).ChildNodes.Item(j).ChildNodes.Item(k).InnerXml = "sublocality") Then
                                                        adapter.SelectCommand.Parameters.AddWithValue("sublocalityaddress", current.ChildNodes.Item(i).ChildNodes.Item(j).ChildNodes.Item((k - 2)).InnerText)
                                                    End If
                                                    If (current.ChildNodes.Item(i).ChildNodes.Item(j).ChildNodes.Item(k).InnerXml = "locality") Then
                                                        adapter.SelectCommand.Parameters.AddWithValue("locPaddre", current.ChildNodes.Item(i).ChildNodes.Item(j).ChildNodes.Item((k - 2)).InnerText)
                                                    End If
                                                    If (current.ChildNodes.Item(i).ChildNodes.Item(j).ChildNodes.Item(k).InnerXml = "administrative_area_level_3") Then
                                                        adapter.SelectCommand.Parameters.AddWithValue("admini3address", current.ChildNodes.Item(i).ChildNodes.Item(j).ChildNodes.Item((k - 2)).InnerText)
                                                    End If
                                                    If (current.ChildNodes.Item(i).ChildNodes.Item(j).ChildNodes.Item(k).InnerXml = "administrative_area_level_2") Then
                                                        adapter.SelectCommand.Parameters.AddWithValue("adminpoladdress", current.ChildNodes.Item(i).ChildNodes.Item(j).ChildNodes.Item((k - 2)).InnerText)
                                                    End If
                                                    If (current.ChildNodes.Item(i).ChildNodes.Item(j).ChildNodes.Item(k).InnerXml = "administrative_area_level_1") Then
                                                        adapter.SelectCommand.Parameters.AddWithValue("addressLongName", current.ChildNodes.Item(i).ChildNodes.Item(j).ChildNodes.Item((k - 2)).InnerText)
                                                        adapter.SelectCommand.Parameters.AddWithValue("addShortName", current.ChildNodes.Item(i).ChildNodes.Item(j).ChildNodes.Item((k - 1)).InnerText)
                                                    End If
                                                    If (current.ChildNodes.Item(i).ChildNodes.Item(j).ChildNodes.Item(k).InnerXml = "country") Then
                                                        adapter.SelectCommand.Parameters.AddWithValue("countryLong", current.ChildNodes.Item(i).ChildNodes.Item(j).ChildNodes.Item((k - 2)).InnerText)
                                                        adapter.SelectCommand.Parameters.AddWithValue("countryShort", current.ChildNodes.Item(i).ChildNodes.Item(j).ChildNodes.Item((k - 1)).InnerText)
                                                    End If
                                                    If (current.ChildNodes.Item(i).ChildNodes.Item(j).ChildNodes.Item(k).InnerXml = "postal_code") Then
                                                        adapter.SelectCommand.Parameters.AddWithValue("postalLong", current.ChildNodes.Item(i).ChildNodes.Item(j).ChildNodes.Item((k - 2)).InnerText)
                                                        adapter.SelectCommand.Parameters.AddWithValue("postalShort", current.ChildNodes.Item(i).ChildNodes.Item(j).ChildNodes.Item((k - 1)).InnerText)
                                                    End If
                                                    If (current.ChildNodes.Item(i).ChildNodes.Item(j).ChildNodes.Item(k).InnerXml = "airport") Then
                                                        adapter.SelectCommand.Parameters.AddWithValue("airport", current.ChildNodes.Item(i).ChildNodes.Item(j).ChildNodes.Item((k - 2)).InnerText)
                                                    End If
                                                    If (current.ChildNodes.Item(i).ChildNodes.Item(j).ChildNodes.Item(k).InnerXml = "point_of_interest") Then
                                                        adapter.SelectCommand.Parameters.AddWithValue("point_of_interest", current.ChildNodes.Item(i).ChildNodes.Item(j).ChildNodes.Item((k - 2)).InnerText)
                                                    End If
                                                    If (current.ChildNodes.Item(i).ChildNodes.Item(j).ChildNodes.Item(k).InnerXml = "park") Then
                                                        adapter.SelectCommand.Parameters.AddWithValue("park", current.ChildNodes.Item(i).ChildNodes.Item(j).ChildNodes.Item((k - 2)).InnerText)
                                                    End If
                                                    If (current.ChildNodes.Item(i).ChildNodes.Item(j).ChildNodes.Item(k).InnerXml = "intersection") Then
                                                        adapter.SelectCommand.Parameters.AddWithValue("intersection", current.ChildNodes.Item(i).ChildNodes.Item(j).ChildNodes.Item((k - 2)).InnerText)
                                                    End If
                                                    If (current.ChildNodes.Item(i).ChildNodes.Item(j).ChildNodes.Item(k).InnerXml = "colloquial_area") Then
                                                        adapter.SelectCommand.Parameters.AddWithValue("colloquial_area", current.ChildNodes.Item(i).ChildNodes.Item(j).ChildNodes.Item((k - 2)).InnerText)
                                                    End If
                                                    If (current.ChildNodes.Item(i).ChildNodes.Item(j).ChildNodes.Item(k).InnerXml = "premise") Then
                                                        adapter.SelectCommand.Parameters.AddWithValue("premise", current.ChildNodes.Item(i).ChildNodes.Item(j).ChildNodes.Item((k - 2)).InnerText)
                                                    End If
                                                    If (current.ChildNodes.Item(i).ChildNodes.Item(j).ChildNodes.Item(k).InnerXml = "subpremise") Then
                                                        adapter.SelectCommand.Parameters.AddWithValue("subpremise", current.ChildNodes.Item(i).ChildNodes.Item(j).ChildNodes.Item((k - 2)).InnerText)
                                                    End If
                                                End If
                                                k += 1
                                            Loop
                                        ElseIf (current.ChildNodes.Item(i).ChildNodes.Item(j).Name = "formatted_address") Then
                                            innerText = current.ChildNodes.Item(i).ChildNodes.Item(j).InnerText
                                            adapter.SelectCommand.Parameters.AddWithValue("location_namer", current.ChildNodes.Item(i).ChildNodes.Item(j).InnerText)
                                        ElseIf (current.ChildNodes.Item(i).ChildNodes.Item(j).Name = "geometry") Then
                                            Dim num17 As Integer = (current.ChildNodes.Item(i).ChildNodes.Item(j).ChildNodes.Count - 1)
                                            Dim m As Integer = 0
                                            Do While (m <= num17)
                                                If (current.ChildNodes.Item(i).ChildNodes.Item(j).ChildNodes.Item(m).Name = "location") Then
                                                    Dim num18 As Integer = (current.ChildNodes.Item(i).ChildNodes.Item(j).ChildNodes.Item(m).ChildNodes.Count - 1)
                                                    Dim n As Integer = 0
                                                    Do While (n <= num18)
                                                        If (current.ChildNodes.Item(i).ChildNodes.Item(j).ChildNodes.Item(m).ChildNodes.Item(n).Name = "lat") Then
                                                            adapter.SelectCommand.Parameters.AddWithValue("geometrylat", current.ChildNodes.Item(i).ChildNodes.Item(j).ChildNodes.Item(m).ChildNodes.Item(n).InnerText)
                                                        ElseIf (current.ChildNodes.Item(i).ChildNodes.Item(j).ChildNodes.Item(m).ChildNodes.Item(n).Name = "lng") Then
                                                            adapter.SelectCommand.Parameters.AddWithValue("geometrylng", current.ChildNodes.Item(i).ChildNodes.Item(j).ChildNodes.Item(m).ChildNodes.Item(n).InnerText)
                                                        End If
                                                        n += 1
                                                    Loop
                                                ElseIf (current.ChildNodes.Item(i).ChildNodes.Item(j).ChildNodes.Item(m).Name = "viewport") Then
                                                    Dim num19 As Integer = (current.ChildNodes.Item(i).ChildNodes.Item(j).ChildNodes.Item(m).ChildNodes.Count - 1)
                                                    Dim num8 As Integer = 0
                                                    Do While (num8 <= num19)
                                                        If (current.ChildNodes.Item(i).ChildNodes.Item(j).ChildNodes.Item(m).ChildNodes.Item(num8).Name = "southwest") Then
                                                            Dim num20 As Integer = (current.ChildNodes.Item(i).ChildNodes.Item(j).ChildNodes.Item(m).ChildNodes.Item(num8).ChildNodes.Count - 1)
                                                            Dim num9 As Integer = 0
                                                            Do While (num9 <= num20)
                                                                If (current.ChildNodes.Item(i).ChildNodes.Item(j).ChildNodes.Item(m).ChildNodes.Item(num8).ChildNodes.Item(num9).Name = "lat") Then
                                                                    adapter.SelectCommand.Parameters.AddWithValue("vSWlat", current.ChildNodes.Item(i).ChildNodes.Item(j).ChildNodes.Item(m).ChildNodes.Item(num8).ChildNodes.Item(num9).InnerText)
                                                                ElseIf (current.ChildNodes.Item(i).ChildNodes.Item(j).ChildNodes.Item(m).ChildNodes.Item(num8).ChildNodes.Item(num9).Name = "lng") Then
                                                                    adapter.SelectCommand.Parameters.AddWithValue("vSWlng", current.ChildNodes.Item(i).ChildNodes.Item(j).ChildNodes.Item(m).ChildNodes.Item(num8).ChildNodes.Item(num9).InnerText)
                                                                End If
                                                                num9 += 1
                                                            Loop
                                                        ElseIf (current.ChildNodes.Item(i).ChildNodes.Item(j).ChildNodes.Item(m).ChildNodes.Item(num8).Name = "northeast") Then
                                                            Dim num21 As Integer = (current.ChildNodes.Item(i).ChildNodes.Item(j).ChildNodes.Item(m).ChildNodes.Item(num8).ChildNodes.Count - 1)
                                                            Dim num10 As Integer = 0
                                                            Do While (num10 <= num21)
                                                                If (current.ChildNodes.Item(i).ChildNodes.Item(j).ChildNodes.Item(m).ChildNodes.Item(num8).ChildNodes.Item(num10).Name = "lat") Then
                                                                    adapter.SelectCommand.Parameters.AddWithValue("vNElat", current.ChildNodes.Item(i).ChildNodes.Item(j).ChildNodes.Item(m).ChildNodes.Item(num8).ChildNodes.Item(num10).InnerText)
                                                                ElseIf (current.ChildNodes.Item(i).ChildNodes.Item(j).ChildNodes.Item(m).ChildNodes.Item(num8).ChildNodes.Item(num10).Name = "lng") Then
                                                                    adapter.SelectCommand.Parameters.AddWithValue("vNElng", current.ChildNodes.Item(i).ChildNodes.Item(j).ChildNodes.Item(m).ChildNodes.Item(num8).ChildNodes.Item(num10).InnerText)
                                                                End If
                                                                num10 += 1
                                                            Loop
                                                        End If
                                                        num8 += 1
                                                    Loop
                                                ElseIf (current.ChildNodes.Item(i).ChildNodes.Item(j).ChildNodes.Item(m).Name = "bounds") Then
                                                    Dim num22 As Integer = (current.ChildNodes.Item(i).ChildNodes.Item(j).ChildNodes.Item(m).ChildNodes.Count - 1)
                                                    Dim num11 As Integer = 0
                                                    Do While (num11 <= num22)
                                                        If (current.ChildNodes.Item(i).ChildNodes.Item(j).ChildNodes.Item(m).ChildNodes.Item(num11).Name = "southwest") Then
                                                            Dim num23 As Integer = (current.ChildNodes.Item(i).ChildNodes.Item(j).ChildNodes.Item(m).ChildNodes.Item(num11).ChildNodes.Count - 1)
                                                            Dim num12 As Integer = 0
                                                            Do While (num12 <= num23)
                                                                If (current.ChildNodes.Item(i).ChildNodes.Item(j).ChildNodes.Item(m).ChildNodes.Item(num11).ChildNodes.Item(num12).Name = "lat") Then
                                                                    adapter.SelectCommand.Parameters.AddWithValue("bSWlat", current.ChildNodes.Item(i).ChildNodes.Item(j).ChildNodes.Item(m).ChildNodes.Item(num11).ChildNodes.Item(num12).InnerText)
                                                                ElseIf (current.ChildNodes.Item(i).ChildNodes.Item(j).ChildNodes.Item(m).ChildNodes.Item(num11).ChildNodes.Item(num12).Name = "lng") Then
                                                                    adapter.SelectCommand.Parameters.AddWithValue("bSWlng", current.ChildNodes.Item(i).ChildNodes.Item(j).ChildNodes.Item(m).ChildNodes.Item(num11).ChildNodes.Item(num12).InnerText)
                                                                End If
                                                                num12 += 1
                                                            Loop
                                                        ElseIf (current.ChildNodes.Item(i).ChildNodes.Item(j).ChildNodes.Item(m).ChildNodes.Item(num11).Name = "northeast") Then
                                                            Dim num24 As Integer = (current.ChildNodes.Item(i).ChildNodes.Item(j).ChildNodes.Item(m).ChildNodes.Item(num11).ChildNodes.Count - 1)
                                                            Dim num13 As Integer = 0
                                                            Do While (num13 <= num24)
                                                                If (current.ChildNodes.Item(i).ChildNodes.Item(j).ChildNodes.Item(m).ChildNodes.Item(num11).ChildNodes.Item(num13).Name = "lat") Then
                                                                    adapter.SelectCommand.Parameters.AddWithValue("bNElat", current.ChildNodes.Item(i).ChildNodes.Item(j).ChildNodes.Item(m).ChildNodes.Item(num11).ChildNodes.Item(num13).InnerText)
                                                                ElseIf (current.ChildNodes.Item(i).ChildNodes.Item(j).ChildNodes.Item(m).ChildNodes.Item(num11).ChildNodes.Item(num13).Name = "lng") Then
                                                                    adapter.SelectCommand.Parameters.AddWithValue("bNElng", current.ChildNodes.Item(i).ChildNodes.Item(j).ChildNodes.Item(m).ChildNodes.Item(num11).ChildNodes.Item(num13).InnerText)
                                                                End If
                                                                num13 += 1
                                                            Loop
                                                        End If
                                                        num11 += 1
                                                    Loop
                                                End If
                                                m += 1
                                            Loop
                                        End If
                                        j += 1
                                    Loop
                                End If
                                If (num = 1) Then
                                    Exit Do
                                End If
                                i += 1
                            Loop
                        Loop
                    Finally
                        If TypeOf enumerator Is IDisposable Then
                            TryCast(enumerator, IDisposable).Dispose()
                        End If
                    End Try
                End If
                If (selectConnection.State <> ConnectionState.Open) Then
                    selectConnection.Open()
                End If
                adapter.SelectCommand.ExecuteNonQuery()
                adapter.Dispose()
                dataTable.Dispose()
                selectConnection.Dispose()
            Catch exception1 As Exception

            Finally
                selectConnection.Dispose()
                adapter.Dispose()
            End Try
            Return innerText
        End If
        Return Nothing
    End Function

    Protected Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Dim constr As String = "server=172.17.109.152;initial catalog=DMS;uid=DMS;pwd=Ztsu93#u"
        Dim con As New SqlConnection(constr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
        Dim ds As New DataSet()
        Dim dt1 As New DataTable
        oda.SelectCommand.CommandText = "select tid,start_latitude,start_longitude,end_latitude,end_longitude from mmm_mst_elogbook where triptype in ('switch') and (Trip_start_location is null or Trip_end_location is null or Trip_Start_Location='' or Trip_End_Location='') order by tid desc "
        oda.Fill(dt1)
        If con.State <> ConnectionState.Open Then
            con.Open()
        End If
        For j As Integer = 0 To dt1.Rows.Count - 1
            oda.SelectCommand.CommandText = "USPUpdateTripLocationOnly"
            oda.SelectCommand.CommandType = CommandType.StoredProcedure
            oda.SelectCommand.Parameters.Clear()
            oda.SelectCommand.Parameters.AddWithValue("@tid", dt1.Rows(j).Item("tid").ToString)
            oda.SelectCommand.Parameters.AddWithValue("@stlocation", Location(dt1.Rows(j).Item("start_latitude").ToString, dt1.Rows(j).Item("start_longitude").ToString))
            oda.SelectCommand.Parameters.AddWithValue("@edlocation", Location(dt1.Rows(j).Item("end_latitude").ToString, dt1.Rows(j).Item("end_longitude").ToString))
            oda.SelectCommand.ExecuteNonQuery()
        Next
        lblmsg.Text = "Location updated successfully."
        con.Close()
    End Sub
    Protected Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        Dim constr As String = "server=172.17.109.152;initial catalog=DMS;uid=DMS;pwd=Ztsu93#u"
        Dim con As New SqlConnection(constr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
        Dim ds As New DataSet()
        Dim dt As New DataTable
        Dim dt1 As New DataTable
        Try
            oda.SelectCommand.CommandText = "select TID,imei_no,trip_start_datetime,trip_end_datetime,total_distance[Trip KM],(select sum(devdist) from mmm_mst_gpsdata where imieno=e.imei_no and ctime>=e.trip_start_datetime and ctime<=e.trip_end_datetime and ((devdist>0.03 and speed=0) or (devdist>0 and speed>0)))[GPS KM] from mmm_mst_elogbook e where total_distance<>(select sum(devdist) from mmm_mst_gpsdata where imieno=e.imei_no and ctime>=e.trip_start_datetime and ctime<=e.trip_end_datetime and ((devdist>0.03 and speed=0) or (devdist>0 and speed>0))) and convert(date,trip_start_datetime)= convert(date,getdate()-52) and e.imei_no='356307042842871' "
            oda.Fill(dt1)
            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            For j As Integer = 0 To dt1.Rows.Count - 1
                oda.SelectCommand.CommandText = "USPUpdateTripKM"
                oda.SelectCommand.CommandType = CommandType.StoredProcedure
                oda.SelectCommand.Parameters.Clear()
                oda.SelectCommand.Parameters.AddWithValue("@TID", dt1.Rows(j).Item("tid").ToString)
                oda.SelectCommand.Parameters.AddWithValue("@Totdist", dt1.Rows(j).Item("GPS KM").ToString)
                oda.SelectCommand.CommandTimeout = 300
                oda.SelectCommand.ExecuteNonQuery()
            Next
        Catch ex As Exception
        Finally
            con.Close()
            oda.Dispose()
            con.Dispose()
        End Try
    End Sub
    Public Function GetAllFields(EID As Integer) As DataSet
        Dim ds As New DataSet()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Try
            Dim Query = "SET NOCOUNT ON;select F.FieldID,F.FieldType,F.FieldMapping,F.FieldID,F.DropDown,F.lookupvalue,F.DROPDOWNTYPE,F.DisplayName ,F.DocumentType,FF.FormSource,FF.EventName  FROM MMM_MST_FIELDS F INNER JOIN MMM_MST_FORMS FF ON FF.FormName=F.DocumentType WHERE F.EID= " & EID & " AND FF.EID= " & EID & ";"
            Using con = New SqlConnection(conStr)
                Using da = New SqlDataAdapter(Query, con)
                    da.Fill(ds)
                End Using
            End Using
        Catch ex As Exception
            Throw
        End Try
        Return ds
    End Function
    Public Function GenearateQuery(EID As Integer, DocumentType As String, IsActionForm As Boolean) As String
        Dim ret As String = ""
        Try
            Dim ds As New DataSet()
            'Geiing all the field of Entity becouse all the field might be required
            ds = GetAllFields(EID)
            Dim BaseView As DataView
            Dim BaseTable As DataTable
            If ds.Tables(0).Rows.Count > 0 Then
                BaseView = ds.Tables(0).DefaultView
                BaseView.RowFilter = "DocumentType='" & DocumentType & "'"
                'Getting all the field of base tabel() Note: in case of Action form base table will be name Action form
                BaseTable = BaseView.Table.DefaultView.ToTable()
                If IsActionForm = True Then
                    DocumentType = ds.Tables(0).Rows(0).Item("EventName").ToString
                    BaseView.RowFilter = "DocumentType='" & DocumentType & "'"
                    BaseTable = BaseView.Table.DefaultView.ToTable()
                End If
                GenearateQuery1(EID, DocumentType, ds)
                'Now find all object relation 
            End If
        Catch ex As Exception
        End Try
        Return ret
    End Function

    Public Function GenearateQuery1(EID As Integer, DocumentType As String, ds As DataSet, Optional tid As Integer = 148289) As String
        Dim ret As String = ""
        Dim View As DataView
        Dim tbl As DataTable
        Dim tblRe As DataTable
        Dim tbCh As DataTable
        Dim tbchitem As DataTable
        Dim StrColumn As String = ""
        Dim StrJoinString As String = ""
        Dim cDoc As String = ""
        Dim SchemaString As String = DocumentType
        If ds.Tables(0).Rows.Count > 0 Then
            View = ds.Tables(0).DefaultView
            View.RowFilter = "DocumentType='" & DocumentType & "'"
            'Getting all the field of base tabel() Note: in case of Action form base table will be name Action form
            tbl = View.Table.DefaultView.ToTable()
            Dim ViewName = "V" & EID & DocumentType.Replace(" ", "_")
            Dim ddlDocType = ""
            For i As Integer = 0 To tbl.Rows.Count - 1
                If Not (tbl.Rows(i).Item("DROPDOWNTYPE") = "MASTER VALUED" And tbl.Rows(i).Item("FieldType") = "Drop Down") Then
                    If tbl.Rows(i).Item("Fieldtype").ToString = "Child Item" Then
                        cDoc = tbl.Rows(i).Item("Dropdown").ToString
                    End If
                    StrColumn = StrColumn & "," & ViewName & ".[" & tbl.Rows(i).Item("DisplayName") & "] AS [" & tbl.Rows(i).Item("DisplayName") & "]"
                End If
            Next
            View.RowFilter = "DocumentType='" & DocumentType & "' AND FieldType ='DROP DOWN' AND DropDownType='MASTER VALUED'"
            tblRe = View.Table.DefaultView.ToTable()
            For j As Integer = 0 To tblRe.Rows.Count - 1
                Dim arrddl = tblRe.Rows(j).Item("Dropdown").ToString().Split("-")
                ddlDocType = arrddl(1)
                SchemaString = SchemaString & "." & ddlDocType
                Dim ddlview = "v" & EID & ddlDocType.Trim.Replace(" ", "_")
                Dim joincolumn = "tid"
                Dim DisPlayName = tblRe.Rows(j).Item("DisplayName").ToString().Trim
                If ddlDocType.Trim.ToUpper = "USER" Then
                    joincolumn = "UID"
                End If
                StrJoinString = StrJoinString & " left outer join " & ddlview & " on " & ddlview & "." & joincolumn & " = " & ViewName & ".[" & DisPlayName & "]"
                GenearateQuery2(EID, StrColumn, StrJoinString, SchemaString, ddlDocType, ds, arrddl(2))
            Next

            If cDoc <> "" Then
                Dim ViewNamech = "V" & EID & cDoc.Replace(" ", "_")
                View.RowFilter = "DocumentType='" & cDoc & "'"
                tbCh = View.Table.DefaultView.ToTable()
                For i As Integer = 0 To tbCh.Rows.Count - 1
                    If Not (tbCh.Rows(i).Item("DROPDOWNTYPE") = "MASTER VALUED" And tbl.Rows(i).Item("FieldType") = "Drop Down") Then
                        StrColumn = StrColumn & "," & ViewNamech & ".[" & tbCh.Rows(i).Item("DisplayName") & "] AS [" & tbCh.Rows(i).Item("DisplayName") & "]"
                    End If
                Next

                StrJoinString = StrJoinString & " left outer join " & ViewNamech & " on " & ViewNamech & ".docid = " & ViewName & ".TID"

                View.RowFilter = "DocumentType='" & cDoc & "' AND FieldType ='DROP DOWN' AND DropDownType='MASTER VALUED'"
                tbchitem = View.Table.DefaultView.ToTable()
                For k As Integer = 0 To tbchitem.Rows.Count - 1
                    Dim arrddl = tbchitem.Rows(k).Item("Dropdown").ToString().Split("-")
                    ddlDocType = arrddl(1)
                    SchemaString = SchemaString & "." & ddlDocType
                    Dim ddlview = "v" & EID & ddlDocType.Trim.Replace(" ", "_")
                    Dim joincolumn = "tid"
                    Dim DisPlayName = tbchitem.Rows(k).Item("DisplayName").ToString().Trim
                    If ddlDocType.Trim.ToUpper = "USER" Then
                        joincolumn = "UID"
                    End If
                    If StrJoinString.Contains("left outer join " & ddlview & " on " & ddlview & "." & joincolumn & " = ") Then
                    Else
                        StrJoinString = StrJoinString & " left outer join " & ddlview & " on " & ddlview & "." & joincolumn & " = " & ViewNamech & ".[" & DisPlayName & "]"
                    End If
                    GenearateQuery3(EID, StrColumn, StrJoinString, SchemaString, cDoc, ds, arrddl(2))
                Next
            End If
            Dim Query = "SELECT " & StrColumn.Substring(1, StrColumn.Length - 1) & " FROM " & ViewName & " " & StrJoinString
            If tid <> 0 Then
                Query = Query & " WHERE " & ViewName & ".tid  = " & tid
            End If
        End If
        Return ret
    End Function

    Public Function GenearateQuery2(EID As Integer, ByRef StrColumn As String, StrJoinString As String, SchemaString As String, DocumentType As String, ds As DataSet, fld As String) As String
        Dim ret As String = ""
        Dim View As DataView
        Dim tbl As DataTable
        Dim dispname As String = ""
        'StrColumn = ""
        If ds.Tables(0).Rows.Count > 0 Then
            View = ds.Tables(0).DefaultView
            View.RowFilter = "DocumentType='" & DocumentType & "'"
            'Getting all the field of base tabel() Note: in case of Action form base table will be name Action form
            tbl = View.Table.DefaultView.ToTable()
            Dim ViewName = "V" & EID & DocumentType.Replace(" ", "_")
            Dim ddlDocType = ""
            For i As Integer = 0 To tbl.Rows.Count - 1
                If fld.ToUpper = tbl.Rows(i).Item("Fieldmapping").ToString.ToUpper Then
                    'If (tbl.Rows(i).Item("DROPDOWNTYPE") = "MASTER VALUED" And tbl.Rows(i).Item("FieldType") = "Drop Down") Then
                    '    Dim arrddl = tbl.Rows(i).Item("Dropdown").ToString().Split("-")
                    '    ddlDocType = arrddl(1)
                    '    Dim FieldMalling = arrddl(2)
                    '    Dim DR As DataRow() = ds.Tables(0).Select("FieldMapping='" & FieldMalling & "' AND DocumentType='" & arrddl(1) & "'")
                    '    If DR.Count > 0 Then
                    '        Dim DisplayName = DR(0).Item("DisplayName")
                    '        Dim str1 = "(SELECT isnull([" & DR(0).Item("DisplayName") & "],'')  from [V" & EID & arrddl(1).Replace(" ", "_") & "] s WHERE CAST(s.tid AS VARCHAR)=CAST(" & ViewName & ".[" & tbl.Rows(i).Item("DisplayName") & "] AS VARCHAR))[" & tbl.Rows(i).Item("DisplayName") & "]"
                    '        StrColumn = StrColumn & "," & str1
                    '    End If
                    'Else

                    'End If
                    dispname = tbl.Rows(i).Item("displayname").ToString
                    StrColumn = StrColumn & "," & ViewName & ".[" & dispname & "] AS [" & dispname & "]"
                End If
            Next
        End If
        Return StrColumn
    End Function

    Public Function GenearateQuery3(EID As Integer, ByRef StrColumn As String, StrJoinString As String, SchemaString As String, DocumentType As String, ds As DataSet, fld As String) As String
        Dim ret As String = ""
        Dim View As DataView
        Dim tbl As DataTable
        Dim dispname As String = ""
        'StrColumn = ""
        If ds.Tables(0).Rows.Count > 0 Then
            View = ds.Tables(0).DefaultView
            View.RowFilter = "DocumentType='" & DocumentType & "'"
            'Getting all the field of base tabel() Note: in case of Action form base table will be name Action form
            tbl = View.Table.DefaultView.ToTable()
            Dim ViewName = "V" & EID & DocumentType.Replace(" ", "_")
            Dim ddlDocType = ""
            For i As Integer = 0 To tbl.Rows.Count - 1
                'If fld.ToUpper = tbl.Rows(i).Item("Fieldmapping").ToString.ToUpper Then
                If (tbl.Rows(i).Item("DROPDOWNTYPE") = "MASTER VALUED" And tbl.Rows(i).Item("FieldType") = "Drop Down") Then
                    Dim arrddl = tbl.Rows(i).Item("Dropdown").ToString().Split("-")
                    ddlDocType = arrddl(1)
                    Dim FieldMalling = arrddl(2)
                    Dim DR As DataRow() = ds.Tables(0).Select("FieldMapping='" & FieldMalling & "' AND DocumentType='" & arrddl(1) & "'")
                    If DR.Count > 0 Then
                        Dim DisplayName = DR(0).Item("DisplayName")
                        Dim str1 = "(SELECT isnull([" & DR(0).Item("DisplayName") & "],'')  from [V" & EID & arrddl(1).Replace(" ", "_") & "] s WHERE CAST(s.tid AS VARCHAR)=CAST(" & ViewName & ".[" & tbl.Rows(i).Item("DisplayName") & "] AS VARCHAR))[" & tbl.Rows(i).Item("DisplayName") & "]"
                        StrColumn = StrColumn & "," & str1
                    End If
                Else
                    'End If
                    dispname = tbl.Rows(i).Item("displayname").ToString
                    StrColumn = StrColumn & "," & ViewName & ".[" & dispname & "] AS [" & dispname & "]"
                End If
            Next
        End If
        Return StrColumn
    End Function
    Protected Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        Dim EID As Integer = 32
        'EID = Convert.ToInt32(Session("EID"))
        'GetAllFields(EID)
        GenearateQuery(EID, "Expense claim fixed pool", False)
    End Sub
    Protected Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
        Dim connectionString As String = "server=172.17.109.152;initial catalog=DMS;uid=DMS;pwd=Ztsu93#u"
        Dim selectConnection As New SqlConnection(connectionString)
        Dim adapter As New SqlDataAdapter("", selectConnection)
        ' Dim set As New DataSet
        Dim table As New DataTable
        Dim dataTable As New DataTable
        Try
            adapter.SelectCommand.CommandText = "select tid,start_latitude,start_longitude,end_latitude,end_longitude from mmm_mst_elogbook with (nolock) where triptype in ('Auto','night autotrip') and (Trip_start_location is null and Trip_end_location is null or Trip_Start_Location='' and Trip_End_Location='')  and (convert(nvarchar,trip_start_datetime,23)=convert(nvarchar,getdate()-1,23) or convert(nvarchar,trip_start_datetime,23)=convert(nvarchar,getdate(),23)) and eid=54 and end_latitude<>'' order by tid desc"
            adapter.Fill(dataTable)
            Dim num2 As Integer = (dataTable.Rows.Count - 1)
            Dim i As Integer = 0
            Do While (i <= num2)
                adapter.SelectCommand.CommandText = "USPUpdateTripLocation"
                adapter.SelectCommand.CommandType = CommandType.StoredProcedure
                adapter.SelectCommand.Parameters.Clear()
                adapter.SelectCommand.Parameters.AddWithValue("@tid", dataTable.Rows.Item(i).Item("tid").ToString)
                adapter.SelectCommand.Parameters.AddWithValue("@stlocation", Me.Location(dataTable.Rows.Item(i).Item("start_latitude").ToString, dataTable.Rows.Item(i).Item("start_longitude").ToString))
                adapter.SelectCommand.Parameters.AddWithValue("@edlocation", Me.Location(dataTable.Rows.Item(i).Item("end_latitude").ToString, dataTable.Rows.Item(i).Item("end_longitude").ToString))
                adapter.SelectCommand.CommandTimeout = 120
                If (selectConnection.State <> ConnectionState.Open) Then
                    selectConnection.Open()
                End If
                adapter.SelectCommand.ExecuteNonQuery()
                i += 1
            Loop
        Catch exception1 As Exception
            Dim exception As Exception = exception1
            adapter.SelectCommand.CommandType = CommandType.Text
            adapter.SelectCommand.CommandText = ("insert into mmm_schedulerlog_Switch values ('" & exception.ToString & "',getdate())")
            If (selectConnection.State <> ConnectionState.Open) Then
                selectConnection.Open()
            End If
            adapter.SelectCommand.ExecuteNonQuery()
        Finally
            selectConnection.Close()
            adapter.Dispose()
            selectConnection.Dispose()
        End Try
    End Sub
    Protected Sub Button6_Click(sender As Object, e As EventArgs) Handles Button6.Click
        ' TripCreateForIndus()
    End Sub
    Sub FerringFarmaAbsentReportConsolidated()
        Dim constr As String = "server=172.17.109.152;initial catalog=DMS;uid=DMS;pwd=Ztsu93#u"
        Dim con As New SqlConnection(constr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
        Dim ds As New DataSet()
        Dim dt11 As New DataTable
        Dim dt As New DataTable
        Dim dt1 As New DataTable
        Dim dt2 As New DataTable
        Dim lv As New DataTable
        Dim fndt As New DataTable
        Dim mnt As Integer
        Dim currmntName As String
        Dim prmntName As String
        mnt = Microsoft.VisualBasic.DateAndTime.Month(Now.Date)
        If mnt = 1 Then
            prmntName = Microsoft.VisualBasic.DateAndTime.MonthName(12)
        Else
            prmntName = Microsoft.VisualBasic.DateAndTime.MonthName(mnt - 1)
        End If
        currmntName = Microsoft.VisualBasic.DateAndTime.MonthName(mnt)
        Try
            oda.SelectCommand.CommandText = "select * from mmm_mst_reportscheduler with(nolock) where eid=57 and iscommon<>'1' and tid=292 order by hh,mm,ordering"
            oda.SelectCommand.CommandType = CommandType.Text
            oda.Fill(ds, "rpt")
            For d As Integer = 0 To ds.Tables("rpt").Rows.Count - 1
                ' If ReportScheduler(ds.Tables("rpt").Rows(d).Item("tid")) = True Then
                If ds.Tables("rpt").Rows(d).Item("sendto").ToString.ToUpper = "EMAIL" Then
                    Dim eid As String = ds.Tables("rpt").Rows(d).Item("eid").ToString()
                    Dim MAILTO As String = ds.Tables("rpt").Rows(d).Item("emailto").ToString()
                    Dim CC As String = ds.Tables("rpt").Rows(d).Item("cc").ToString()
                    Dim Bcc As String = ds.Tables("rpt").Rows(d).Item("bcc").ToString()
                    Dim mailsub As String = ds.Tables("rpt").Rows(d).Item("reportsubject").ToString()
                    Dim msg As String = ds.Tables("rpt").Rows(d).Item("msgbody").ToString()
                    Dim fdate As String = ds.Tables("rpt").Rows(d).Item("fromdate").ToString()
                    Dim tdate As String = ds.Tables("rpt").Rows(d).Item("todate").ToString()
                    oda.SelectCommand.CommandText = "select " & fdate & "[fdate]," & tdate & "[tdate]"
                    oda.Fill(ds, "date")
                    Dim Nfdate As String = ds.Tables("date").Rows(0).Item("fdate").ToString
                    Dim Ntdate As String = ds.Tables("date").Rows(0).Item("tdate").ToString
                    oda.SelectCommand.CommandType = CommandType.Text
                    oda.SelectCommand.CommandText = "select distinct fld1 from mmm_mst_master with(nolock) where eid=57 and documenttype='Employee Master' and isauth=1 and fld15<>'Alumni'"
                    oda.Fill(dt)
                    If dt.Rows.Count > 0 Then
                        For i As Integer = 0 To dt.Rows.Count - 1
                            oda.SelectCommand.CommandType = CommandType.StoredProcedure
                            oda.SelectCommand.CommandText = "uspgetAtt_mismatchnew"
                            oda.SelectCommand.Parameters.Clear()
                            oda.SelectCommand.Parameters.AddWithValue("empcode", dt.Rows(i).Item(0).ToString)
                            oda.SelectCommand.Parameters.AddWithValue("EID", 57)
                            oda.SelectCommand.Parameters.AddWithValue("fdate", Nfdate)
                            oda.SelectCommand.Parameters.AddWithValue("tdate", Ntdate)
                            oda.Fill(dt1)
                        Next
                    End If
                    oda.SelectCommand.CommandType = CommandType.Text
                    oda.SelectCommand.CommandText = "set dateformat dmy;select distinct d.fld1,d.fld11,d.fld12 from mmm_mst_doc d with(nolock) inner join mmm_mst_master m on m.fld1=d.fld1 where d.eid=57 and d.documenttype='Leave Application' and m.documenttype='Employee Master' and m.isauth=1 and m.fld15<>'Alumni' and convert(date,d.fld11)>=convert(date," & fdate & ") and convert(date,d.fld12)<=convert(date," & tdate & ") and d.fld14 in ('approved')"
                    oda.Fill(dt2)
                    If dt2.Rows.Count > 0 Then
                        For i As Integer = 0 To dt2.Rows.Count - 1
                            oda.SelectCommand.CommandType = CommandType.StoredProcedure
                            oda.SelectCommand.CommandText = "uspgetEmpLeaveDtl"
                            oda.SelectCommand.Parameters.Clear()
                            oda.SelectCommand.Parameters.AddWithValue("empcode", dt2.Rows(i).Item(0).ToString)
                            oda.SelectCommand.Parameters.AddWithValue("dt1", dt2.Rows(i).Item(1).ToString)
                            oda.SelectCommand.Parameters.AddWithValue("dt2", dt2.Rows(i).Item(2).ToString)
                            oda.Fill(lv)
                        Next
                    End If
                    Dim rows_to_remove As New List(Of DataRow)()
                    For Each row1 As DataRow In dt1.Rows
                        For Each row2 As DataRow In lv.Rows
                            If (row1.Item(0).ToString() = row2.Item(0).ToString()) And (row1.Item(2).ToString() = row2.Item(2).ToString()) And (row1.Item(3).ToString() = row2.Item(3).ToString()) Then
                                rows_to_remove.Add(row1)
                            End If
                        Next
                    Next
                    For Each row As DataRow In rows_to_remove
                        dt1.Rows.Remove(row)
                        dt1.AcceptChanges()
                    Next
                    Dim fname As String = ""
                    fname = CreateCSV(dt1, FPath)
                    mailsub = mailsub.Replace("{Previous Month}", prmntName)
                    mailsub = mailsub.Replace("{Current Month}", currmntName)
                    msg = msg.Replace("{Previous Month}", prmntName)
                    msg = msg.Replace("{Current Month}", currmntName)
                    Dim obj As New MailUtill(eid:=eid)
                    obj.SendMail(ToMail:=MAILTO, Subject:=mailsub, MailBody:=msg, CC:=CC, Attachments:=FPath + fname, BCC:=Bcc)
                End If
                ' End If
                oda.SelectCommand.CommandType = CommandType.Text
                oda.SelectCommand.CommandText = "UPDATE  mmm_mst_reportscheduler SET LASTSCHEDULEDDATE=GETDATE() WHERE TID=" & ds.Tables("rpt").Rows(d).Item("tid") & ""
                If con.State <> ConnectionState.Open Then
                    con.Open()
                End If
                oda.SelectCommand.ExecuteNonQuery()
            Next
        Catch ex As Exception
            oda.SelectCommand.CommandText = "INSERT_ERRORLOG"
            oda.SelectCommand.CommandType = CommandType.StoredProcedure
            oda.SelectCommand.Parameters.Clear()
            oda.SelectCommand.Parameters.AddWithValue("@ERRORMSG", ex.ToString())
            oda.SelectCommand.Parameters.AddWithValue("@ALERTNAME", "Ferring")
            oda.SelectCommand.Parameters.AddWithValue("@EID", 57)
            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            oda.SelectCommand.ExecuteNonQuery()
        Finally
            oda.Dispose()
            con.Close()
            con.Dispose()
        End Try
    End Sub
    Sub FerringFarmaAbsentReport()
        Dim constr As String = "server=172.17.109.152;initial catalog=DMS;uid=DMS;pwd=Ztsu93#u"
        Dim con As New SqlConnection(constr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
        Dim ds As New DataSet()
        Dim dt11 As New DataTable
        Dim dt As New DataTable
        Dim dt1 As New DataTable
        Dim dt2 As New DataTable
        Dim lv As New DataTable
        Dim fndt As New DataTable
        Dim mnt As Integer
        Dim currmntName As String
        Dim prmntName As String
        mnt = Microsoft.VisualBasic.DateAndTime.Month(Now.Date)
        If mnt = 1 Then
            prmntName = Microsoft.VisualBasic.DateAndTime.MonthName(12)
        Else
            prmntName = Microsoft.VisualBasic.DateAndTime.MonthName(mnt - 1)
        End If
        currmntName = Microsoft.VisualBasic.DateAndTime.MonthName(mnt)
        Try
            oda.SelectCommand.CommandText = "select * from mmm_mst_reportscheduler with(nolock) where eid=57 and iscommon<>'1' and tid=284 order by hh,mm,ordering"
            oda.SelectCommand.CommandType = CommandType.Text
            oda.Fill(ds, "rpt")
            For d As Integer = 0 To ds.Tables("rpt").Rows.Count - 1
                'If ReportScheduler(ds.Tables("rpt").Rows(d).Item("tid")) = True Then
                If ds.Tables("rpt").Rows(d).Item("sendto").ToString.ToUpper = "EMAIL" Then
                    Dim eid As String = ds.Tables("rpt").Rows(d).Item("eid").ToString()
                    Dim MAILTO As String = ds.Tables("rpt").Rows(d).Item("emailto").ToString()
                    Dim CC As String = ds.Tables("rpt").Rows(d).Item("cc").ToString()
                    Dim Bcc As String = ds.Tables("rpt").Rows(d).Item("bcc").ToString()
                    'Dim mailsub As String = ds.Tables("rpt").Rows(d).Item("reportsubject").ToString()
                    Dim msg As String = ds.Tables("rpt").Rows(d).Item("msgbody").ToString()
                    Dim fdate As String = ds.Tables("rpt").Rows(d).Item("fromdate").ToString()
                    Dim tdate As String = ds.Tables("rpt").Rows(d).Item("todate").ToString()
                    oda.SelectCommand.CommandText = "select " & fdate & "[fdate]," & tdate & "[tdate]"
                    oda.Fill(ds, "date")
                    Dim Nfdate As String = ds.Tables("date").Rows(0).Item("fdate").ToString
                    Dim Ntdate As String = ds.Tables("date").Rows(0).Item("tdate").ToString
                    oda.SelectCommand.CommandText = "select distinct fld1,fld19,fld23 from mmm_mst_master with(nolock) where eid=57 and documenttype='Employee Master' and fld1 in ('FPPL743','FPPL745','FPPL751','FPPL755','FPPL756','FPPL757','FPPL758','FPPL759','FPPL760','FPPL761','FPPL762','FPPL763','FPPL764','FPPL767') and isauth=1 and fld15<>'Alumni'"
                    oda.Fill(dt)
                    If dt.Rows.Count > 0 Then
                        For i As Integer = 0 To dt.Rows.Count - 1
                            dt2.Clear()
                            lv.Clear()
                            dt1.Clear()
                            oda.SelectCommand.CommandType = CommandType.StoredProcedure
                            oda.SelectCommand.CommandText = "uspgetAtt_mismatchnew"
                            oda.SelectCommand.Parameters.Clear()
                            oda.SelectCommand.Parameters.AddWithValue("empcode", dt.Rows(i).Item(0).ToString)
                            oda.SelectCommand.Parameters.AddWithValue("EID", 57)
                            oda.SelectCommand.Parameters.AddWithValue("fdate", Nfdate)
                            oda.SelectCommand.Parameters.AddWithValue("tdate", Ntdate)
                            oda.Fill(dt1)
                            If dt1.Rows.Count > 0 Then
                            Else
                                Continue For
                            End If
                            oda.SelectCommand.CommandType = CommandType.Text
                            oda.SelectCommand.CommandText = "set dateformat dmy;select distinct fld1,fld11,fld12 from mmm_mst_doc with(nolock) where eid=57 and documenttype='Leave Application' and convert(date,fld11)>=convert(date," & fdate & ") and convert(date,fld12)<=convert(date," & tdate & ") and fld1 ='" & dt.Rows(i).Item(0).ToString & "' and fld14 in ('approved')"
                            oda.Fill(dt2)
                            If dt2.Rows.Count > 0 Then
                                For j As Integer = 0 To dt2.Rows.Count - 1
                                    oda.SelectCommand.CommandType = CommandType.StoredProcedure
                                    oda.SelectCommand.CommandText = "uspgetEmpLeaveDtl"
                                    oda.SelectCommand.Parameters.Clear()
                                    oda.SelectCommand.Parameters.AddWithValue("empcode", dt2.Rows(j).Item(0).ToString)
                                    oda.SelectCommand.Parameters.AddWithValue("dt1", dt2.Rows(j).Item(1).ToString)
                                    oda.SelectCommand.Parameters.AddWithValue("dt2", dt2.Rows(j).Item(2).ToString)
                                    oda.Fill(lv)
                                Next
                            End If
                            Dim rows_to_remove As New List(Of DataRow)()
                            If lv.Rows.Count > 0 Then
                                For Each row1 As DataRow In dt1.Rows
                                    For Each row2 As DataRow In lv.Rows
                                        If (row1.Item(0).ToString() = row2.Item(0).ToString()) And (row1.Item(2).ToString() = row2.Item(2).ToString()) And (row1.Item(3).ToString() = row2.Item(3).ToString()) Then
                                            rows_to_remove.Add(row1)
                                        End If
                                    Next
                                Next
                                For Each row As DataRow In rows_to_remove
                                    dt1.Rows.Remove(row)
                                    dt1.AcceptChanges()
                                Next
                            End If

                            Dim fname As String = ""
                            ' fname = CreateCSV(dt1, FPath)
                            Dim MailTable As New StringBuilder()
                            MailTable.Append("<table border=""1"" width=""100%"">")
                            MailTable.Append("<tr style=""background-color:#990000"" Font-Bold=""True""> ")

                            For l As Integer = 0 To dt1.Columns.Count - 1
                                MailTable.Append("<td >" & dt1.Columns(l).ColumnName & "</td>")
                            Next

                            For k As Integer = 0 To dt1.Rows.Count - 1 ' binding the tr tab in table
                                MailTable.Append("</tr><tr>") ' for row records
                                For t As Integer = 0 To dt1.Columns.Count - 1
                                    MailTable.Append("<td>" & dt1.Rows(k).Item(t).ToString() & " </td>")
                                Next
                                MailTable.Append("</tr>")
                            Next
                            MailTable.Append("</table>")

                            Dim mailsub As String = ds.Tables("rpt").Rows(d).Item("reportsubject").ToString()
                            mailsub = mailsub.Replace("{Previous Month}", prmntName)
                            mailsub = mailsub.Replace("{Current Month}", currmntName)
                            Dim strmsgBdy As String = ds.Tables("rpt").Rows(d).Item("msgbody").ToString()
                            If strmsgBdy.Contains("@body") Then
                                strmsgBdy = Replace(strmsgBdy, "@body", MailTable.ToString())
                                msg = strmsgBdy
                            Else
                                msg = MailTable.ToString()
                            End If
                            msg = strmsgBdy
                            msg = msg.Replace("{Employee Name}", dt1.Rows(0).Item(1).ToString)
                            msg = msg.Replace("{Previous Month}", prmntName)
                            msg = msg.Replace("{Current Month}", currmntName)
                            Dim obj As New MailUtill(eid:=eid)
                            obj.SendMail(ToMail:=dt.Rows(i).Item(1).ToString, Subject:=mailsub, MailBody:=msg, CC:=dt.Rows(i).Item(2).ToString, Attachments:="", BCC:=Bcc)
                            'obj.SendMail(ToMail:="vishal.kumar@myndsol.com", Subject:=mailsub, MailBody:=msg, CC:="vishal.kumar@myndsol.com", Attachments:="", BCC:=Bcc)
                        Next
                    End If
                End If
                'End If
            Next
        Catch ex As Exception
            oda.SelectCommand.CommandText = "INSERT_ERRORLOG"
            oda.SelectCommand.CommandType = CommandType.StoredProcedure
            oda.SelectCommand.Parameters.Clear()
            oda.SelectCommand.Parameters.AddWithValue("@ERRORMSG", ex.ToString())
            oda.SelectCommand.Parameters.AddWithValue("@ALERTNAME", "Ferring")
            oda.SelectCommand.Parameters.AddWithValue("@EID", 57)
            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            oda.SelectCommand.ExecuteNonQuery()
        Finally
            oda.Dispose()
            con.Close()
            con.Dispose()
        End Try
    End Sub
    Private Function CreateCSV(ByVal dt As DataTable, ByVal path As String) As String
        ' Dim fname As String = "F014Z1_" & DateTime.Now.ToString("yyyy-MM-dd") & ".CSV"
        Dim fname As String = Now.Year & Now.Month & Now.Day & Now.Hour & Now.Minute & Now.Millisecond & ".CSV"
        'Dim fname As String = "F014Z1_" & DateTime.Now.ToString("yyyy-MM-dd") & ".CSV"
        If File.Exists(path & fname) Then
            File.Delete(path & fname)
        End If
        Dim sw As StreamWriter = New StreamWriter(Path & fname, False)
        sw.Flush()
        'First we will write the headers.
        Dim iColCount As Integer = dt.Columns.Count
        For i As Integer = 0 To iColCount - 1
            sw.Write(dt.Columns(i))
            If (i < iColCount - 1) Then
                sw.Write(",")
            End If
        Next
        sw.Write(sw.NewLine)
        ' Now write all the rows.
        Dim dr As DataRow
        For Each dr In dt.Rows
            For i As Integer = 0 To iColCount - 1
                If Not Convert.IsDBNull(dr(i)) Then
                    sw.Write(dr(i).ToString)
                End If
                If (i < iColCount - 1) Then
                    sw.Write(",")
                End If
            Next
            sw.Write(sw.NewLine)
        Next
        sw.Close()

        Return fname
    End Function
    Public Function ReportScheduler(ByVal tid As Integer) As Boolean
        Dim b As Boolean = False
        Dim conStr As String = "server=172.17.109.152;initial catalog=DMS;uid=DMS;pwd=Ztsu93#u"
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim time1 As DateTime = Now.AddMinutes(-5).ToShortTimeString()
        Dim time2 As DateTime = Now.AddMinutes(+5).ToShortTimeString()
        Dim da As New SqlDataAdapter("select HH,MM,reporttype,TID,date from MMM_MST_ReportScheduler where tid=" & tid & " ", con)
        Dim dt As New DataTable()
        da.Fill(dt)
        Dim SchType As String = dt.Rows(0).Item("reporttype").ToString()
        If UCase(SchType) = "DAILY" Then
            Dim x As DateTime = (Convert.ToDateTime(Trim(dt.Rows(0)(0)) & ":" & Trim(dt.Rows(0)(1)) & ":" & "00").ToShortTimeString)
            If x <= time2 And x >= time1 Then
                b = True
            End If
        End If
        If UCase(SchType) = "WEEKLY" Then
            Dim dayName As String = DateTime.Now.ToString("dddd")
            Dim currentweek As String = dt.Rows(0).Item("Date").ToString()
            If currentweek = 1 Then
                currentweek = "Monday"
            ElseIf currentweek = 2 Then
                currentweek = "Tuesday"
            ElseIf currentweek = 3 Then
                currentweek = "Wednesday"
            ElseIf currentweek = 4 Then
                currentweek = "Thursday"
            ElseIf currentweek = 5 Then
                currentweek = "Friday"
            ElseIf currentweek = 6 Then
                currentweek = "Saturday"
            ElseIf currentweek = 7 Then
                currentweek = "Sunday"
            End If
            If currentweek = dayName Then
                Dim x As DateTime = (Convert.ToDateTime(Trim(dt.Rows(0)(0)) & ":" & Trim(dt.Rows(0)(1)) & ":" & "00").ToShortTimeString)
                If x <= time2 And x >= time1 Then
                    b = True
                End If
            End If
        End If
        If UCase(SchType) = "MONTHLY" Then
            Dim currentDate As DateTime = DateTime.Now
            Dim dateofMonth As Integer = currentDate.Day
            Dim dateMail As Integer = dt.Rows(0).Item("Date").ToString()
            If dateofMonth = dateMail Then
                Dim x As DateTime = (Convert.ToDateTime(Trim(dt.Rows(0)(0)) & ":" & Trim(dt.Rows(0)(1)) & ":" & "00").ToShortTimeString)
                If x <= time2 And x >= time1 Then
                    b = True
                End If
            End If
        End If
        con.Close()
        con.Dispose()
        da.Dispose()
        dt.Dispose()
        Return b
    End Function
    Protected Sub Button7_Click(sender As Object, e As EventArgs) Handles Button7.Click
        FerringFarmaAbsentReport()
    End Sub
    Protected Sub Button8_Click(sender As Object, e As EventArgs) Handles Button8.Click
        SMSAlert()
    End Sub
    Public Sub SMSAlert()
        Dim connectionString As String = "server=172.17.109.152;initial catalog=DMS;uid=DMS;pwd=Ztsu93#u"
        Dim con As New SqlConnection(connectionString)
        Dim adapter As New SqlDataAdapter("", con)
        ' Dim set As New DataSet
        Dim dt As New DataTable
        Dim dtt As New DataTable
        Dim gfdtfld As New DataTable
        Dim geofencedt As New DataTable
        Dim smsdt As New DataTable
        Dim imeidt As New DataTable
        Dim mbnimedt As New DataTable
        Dim gpsdt As New DataTable
        Dim cnt As Integer = 0
        Try
            adapter.SelectCommand.CommandText = "select * from mmm_mst_reportscheduler with(nolock) where alerttype='GPS SMS Alert' "
            adapter.Fill(dt)
            If dt.Rows.Count > 0 Then
                For i As Integer = 0 To dt.Rows.Count - 1
                    adapter.SelectCommand.CommandText = " select fieldmapping,geofencetype from mmm_mst_fields with(nolock) where eid=" & dt.Rows(i).Item("EID").ToString & " and documenttype='" & dt.Rows(i).Item("GeofenceDoctype").ToString & "' and fieldtype='Geo fence'"
                    gfdtfld.Clear()
                    adapter.Fill(gfdtfld)
                    adapter.SelectCommand.CommandText = "select " & gfdtfld.Rows(0).Item(0).ToString & " from mmm_mst_master with(nolock) where eid=" & dt.Rows(i).Item("EID").ToString & " and documenttype='" & dt.Rows(i).Item("GeofenceDoctype").ToString & "' and " & gfdtfld.Rows(0).Item(0).ToString & " is not null and " & gfdtfld.Rows(0).Item(0).ToString & " <>''"
                    geofencedt.Clear()
                    adapter.Fill(geofencedt)
                    adapter.SelectCommand.CommandText = "select FieldMapping,dropdown,dropdowntype from mmm_mst_fields with(nolock) where eid=" & dt.Rows(i).Item("EID").ToString & " and documenttype='" & dt.Rows(i).Item("GeofenceDoctype").ToString & "' and fieldmapping in ('" & dt.Rows(i).Item("MobNofield").ToString & "')"
                    smsdt.Clear()
                    adapter.Fill(smsdt)
                    adapter.SelectCommand.CommandText = "select FieldMapping,dropdown,dropdowntype from mmm_mst_fields with(nolock) where eid=" & dt.Rows(i).Item("EID").ToString & " and documenttype='" & dt.Rows(i).Item("GeofenceDoctype").ToString & "' and fieldmapping in ('" & dt.Rows(i).Item("imeifield").ToString & "')"
                    imeidt.Clear()
                    adapter.Fill(imeidt)
                    Dim arr() As String
                    Dim ret As Boolean = False
                    If geofencedt.Rows.Count > 0 Then
                        For j As Integer = 0 To geofencedt.Rows.Count - 1
                            If gfdtfld.Rows(0).Item(1).ToString.ToUpper = "CIRCULAR" Then
                                arr = geofencedt.Rows(j).Item(0).ToString.Split(",")
                            End If
                            adapter.SelectCommand.CommandText = "select " & smsdt.Rows(0).Item(0).ToString & "[MobNo]," & imeidt.Rows(0).Item(0).ToString & "[IMEINo]," & dt.Rows(i).Item("SMSDispFields").ToString & " from mmm_mst_master with(nolock) where eid=" & dt.Rows(i).Item("EID").ToString & " and documenttype='" & dt.Rows(i).Item("GeofenceDoctype").ToString & "' and " & gfdtfld.Rows(0).Item(0).ToString & "='" & geofencedt.Rows(j).Item(0).ToString & "'"
                            mbnimedt.Clear()
                            adapter.Fill(mbnimedt)
                            Dim datetm As String = ""
                            If mbnimedt.Rows.Count > 0 Then
                                adapter.SelectCommand.CommandText = "select convert(varchar,ctime,0)[ctime],lattitude,longitude from mmm_mst_gpsdata with(nolock) where imieno='" & mbnimedt.Rows(0).Item(1).ToString & "' and DATEADD(minute,DATEDIFF(minute,0,ctime),0)>=DATEADD(minute,DATEDIFF(minute,0,'" & getdate(dt.Rows(i).Item("lastscheduleddate").ToString) & "'),0) and DATEADD(minute,DATEDIFF(minute,0,ctime),0)<=DATEADD(minute,DATEDIFF(minute,0,GETDATE()),0) "
                                gpsdt.Clear()
                                adapter.Fill(gpsdt)
                                If gpsdt.Rows.Count > 0 Then
                                    For k As Integer = 0 To gpsdt.Rows.Count - 1
                                        If gfdtfld.Rows(0).Item(1).ToString.ToUpper = "CIRCULAR" Then
                                            adapter.SelectCommand.CommandText = "select dms.CalculateDistanceInMeter (" & arr(0) & "," & arr(1) & "," & gpsdt.Rows(k).Item(1) & "," & gpsdt.Rows(k).Item(2) & ")"
                                            If con.State <> ConnectionState.Open Then
                                                con.Open()
                                            End If
                                            Dim mtr As Integer = adapter.SelectCommand.ExecuteScalar()
                                            If mtr >= Convert.ToInt32(arr(2).ToString) Then
                                                cnt = cnt + 1
                                                If cnt = 1 Then
                                                    datetm = gpsdt.Rows(k).Item(0).ToString
                                                End If
                                            End If
                                        Else
                                            ret = IsPointInPolygon(geofencedt.Rows(j).Item(0).ToString, gpsdt.Rows(k).Item(1).ToString & "," & gpsdt.Rows(k).Item(2).ToString)
                                            If ret = True Then
                                                cnt = cnt + 1
                                                If cnt = 1 Then
                                                    datetm = gpsdt.Rows(k).Item(0).ToString
                                                End If
                                            End If
                                        End If
                                    Next
                                    If cnt > 0 Then
                                        Me.SendReply1("25728", mbnimedt.Rows(0).Item(0).ToString, "Dear Sir, Please note that Guard of Site " & mbnimedt.Rows(0).Item(2).ToString & " is out of Site from " & datetm & ". Thanks")
                                        adapter.SelectCommand.CommandText = "update mmm_mst_reportscheduler set lastscheduleddate=getdate() where eid=" & dt.Rows(i).Item("EID").ToString & " and tid=" & dt.Rows(i).Item(0) & ""
                                        If con.State <> ConnectionState.Open Then
                                            con.Open()
                                        End If
                                        adapter.SelectCommand.ExecuteNonQuery()
                                    End If
                                    cnt = 0
                                Else
                                    Me.SendReply1("25822", mbnimedt.Rows(0).Item(0).ToString, "Dear Sir, We have not received signals from Site " & mbnimedt.Rows(0).Item(2).ToString & " from last 15 minutes. Please check GPRS and Switch Off.")
                                    adapter.SelectCommand.CommandText = "update mmm_mst_reportscheduler set lastscheduleddate=getdate() where eid=" & dt.Rows(i).Item("EID").ToString & " and tid=" & dt.Rows(i).Item(0) & ""
                                    If con.State <> ConnectionState.Open Then
                                        con.Open()
                                    End If
                                    adapter.SelectCommand.ExecuteNonQuery()
                                End If
                            End If
                        Next
                    End If
                Next
            End If
        Catch exception1 As Exception
            Dim exception As Exception = exception1
            adapter.SelectCommand.CommandType = CommandType.Text
            adapter.SelectCommand.CommandText = ("insert into mmm_schedulerlog_Switch values ('" & exception.ToString & "',getdate())")
            If (con.State <> ConnectionState.Open) Then
                con.Open()
            End If
            adapter.SelectCommand.ExecuteNonQuery()
        Finally
            con.Close()
            adapter.Dispose()
            con.Dispose()
        End Try
    End Sub
    Public Shared Function getdate1(ByVal dbt As String) As String
        Dim dtArr() As String
        dtArr = Split(dbt, "/")
        If dtArr.GetUpperBound(0) = 2 Then
            Dim dd, mm, yy As String
            dd = dtArr(0)
            mm = dtArr(1)
            yy = dtArr(2)
            'Dim dt As DateTime
            Dim ab As String = mm & "-" & dd & "-" & yy
            Try
                'dt = DateTime.Parse(ab.ToString())
                Return ab
            Catch ex As Exception
                Return Now.Date.ToString
            End Try
        Else
            Return Now.Date.ToString
        End If
    End Function
    Public Function IsPointInPolygon(PolyPoints As String, StrPoint As String) As Boolean
        Dim result As Boolean = False
        Dim polygon = CreatePloyCol(PolyPoints)
        Dim Point As New geopoint()
        Try
            Dim arr = StrPoint.Trim.Split(",")
            Point.X = arr(0)
            Point.Y = arr(1)
            Dim j As Integer = polygon.Count() - 1
            For i As Integer = 0 To polygon.Count() - 1
                If polygon(i).Y < Point.Y AndAlso polygon(j).Y >= Point.Y OrElse polygon(j).Y < Point.Y AndAlso polygon(i).Y >= Point.Y Then
                    If polygon(i).X + (Point.Y - polygon(i).Y) / (polygon(j).Y - polygon(i).Y) * (polygon(j).X - polygon(i).X) < Point.X Then
                        result = Not result
                    End If
                End If
                j = i
            Next
        Catch ex As Exception
            Return False
        End Try
        Return result
    End Function
    Public Shared Function getdate(ByVal dbt As String) As String
        Dim dtArr() As String
        dtArr = Split(dbt, "-")
        If dtArr.GetUpperBound(0) = 2 Then
            Dim dd, mm, yy As String
            dd = dtArr(0)
            mm = dtArr(1)
            yy = dtArr(2)
            'Dim dt As DateTime
            Dim ab As String = mm & "-" & dd & "-" & yy
            Try
                'dt = DateTime.Parse(ab.ToString())
                Return ab
            Catch ex As Exception
                Return Now.Date.ToString
            End Try
        Else
            Return Now.Date.ToString
        End If
    End Function
    Private Function CreatePloyCol(points As String) As List(Of geopoint)
        Dim polygon As New List(Of geopoint)
        Dim arr = points.Split(",")
        Dim obj As geopoint
        For i As Integer = 0 To arr.Count - 1

            Try
                If i < arr.Length - 1 Then
                    If arr(i).Trim <> "" And arr(i + 1).Trim <> "" Then
                        obj = New geopoint()
                        obj.X = arr(i)
                        obj.Y = arr(i + 1)
                        polygon.Add(obj)
                    End If
                End If
            Catch ex As Exception

            End Try

            i = i + 1
        Next
        Return polygon
    End Function
    Public Class geopoint
        Public X As Decimal
        Public Y As Decimal
    End Class
    Private Sub SendReply1(ByVal templateID As String, ByVal MobileNumber As String, ByVal msg As String)
        Dim msgString As String = "http://smsalertbox.com/api/sms.php?uid=6d796e646270&pin=51f36abe9f80a&sender=MYNDBP&route=5&tempid=" & templateID & "&mobile=" & MobileNumber & "&message=" & msg & "&pushid=1"
        Dim conStr As String = "server=172.17.109.152;initial catalog=DMS;uid=DMS;pwd=Ztsu93#u"
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("InsertSMSALERTLog", con)
        oda.SelectCommand.CommandType = CommandType.StoredProcedure
        oda.SelectCommand.Parameters.AddWithValue("SNO", MobileNumber)
        oda.SelectCommand.Parameters.AddWithValue("MSGTEXT", msg)
        If con.State <> ConnectionState.Open Then
            con.Open()
        End If
        oda.SelectCommand.ExecuteNonQuery()
        con.Close()
        oda.Dispose()
        con.Dispose()
        Dim result As String = apicall(msgString)
    End Sub
    Public Function apicall(ByVal url As String) As String
        Dim httpreq As HttpWebRequest = DirectCast(WebRequest.Create(url), HttpWebRequest)
        Try
            Dim httpres As HttpWebResponse = DirectCast(httpreq.GetResponse(), HttpWebResponse)
            Dim sr As New StreamReader(httpres.GetResponseStream())
            Dim results As String = sr.ReadToEnd()
            sr.Close()
            Return results
        Catch
            Return "0"
        End Try
    End Function
    Protected Sub XMLfile()
        Dim conStr As String = "server=172.17.109.152;initial catalog=DMS;uid=DMS;pwd=Ztsu93#u"
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("InsertSMSALERTLog", con)
        Dim doc As XmlDocument = New XmlDocument()
        Dim Vdt As New DataTable
        Dim SITdt As New DataTable
        Dim Invtrdt As New DataTable
        Dim ds As New DataSet
        oda.SelectCommand.CommandText = "select tid[DocID],dms.udf_split('MASTER-Distributor-fld1',fld10)[Distributor Name],fld11[Distributor Code],fld12[STATE],replace(fld13,'/','-')[BILLDATE],fld14[SupplierInvoiceNo],fld15[VENDORNAME],fld16[VENDORADDRESS],fld17[VENDORTINNO.],isnull(fld18,0)[GROSSAMOUNT],isnull(fld2,0)[TAX],isnull(fld21,0)[TOTALBILLAMOUNT] from mmm_mst_doc where eid=43 and documenttype='Purchase Document'"
        oda.Fill(Vdt)
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
        Dim xdoc As New XmlDocument()
        xdoc.LoadXml(strB.ToString())
        xdoc.Save("d:\TEST1.xml")
    End Sub
    Protected Sub Button9_Click(sender As Object, e As EventArgs) Handles Button9.Click
        POrder("Purchase Document", "0001")
    End Sub
    Function POrder(ByVal Dtype As String, ByVal DistCode As String) As String
        Dtype = "Purchase Document"
        DistCode = "0001"
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
    'Function OutwardPAL(Data As Stream) As String Implements IBPMCustomWS.OutwardPAL
    '    Dim Result = ""
    '    Dim Key As String = "", EID As String = "", DocType As String = "", UID As String = 0
    '    Try
    '        '' here code to read xml for doc type and field name and create xml output string and retun xml string

    '    Catch ex As Exception
    '        ErrorLog.sendMail("BPMCustomWS.OutwardPAL", ex.Message)
    '        Return "RTO"
    '    End Try
    '    Return Result
    'Dim xmldoc As New XmlDataDocument()
    'Dim xmlnode As XmlNodeList
    'Dim i As Integer
    'Dim str As String
    'Dim fs As New FileStream("products.xml", FileMode.Open, FileAccess.Read)
    '    xmldoc.Load(fs)
    '    xmlnode = xmldoc.GetElementsByTagName("Product")
    '    For i = 0 To xmlnode.Count - 1
    '        xmlnode(i).ChildNodes.Item(0).InnerText.Trim()
    '        str = xmlnode(i).ChildNodes.Item(0).InnerText.Trim() & "  " & xmlnode(i).ChildNodes.Item(1).InnerText.Trim() & "  " & xmlnode(i).ChildNodes.Item(2).InnerText.Trim()
    '        MsgBox(str)
    '    Next
    'End Function
    Protected Sub Button10_Click(sender As Object, e As EventArgs) Handles Button10.Click
        getvehicle()
    End Sub
    Protected Function getvehicle() As String
        Dim returnString As New StringBuilder()
        returnString.Clear()

        Dim dtImieNo As New DataTable
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim da As SqlDataAdapter = New SqlDataAdapter("select distinct fld12[IMIENO] from mmm_mst_master where documenttype='vehicle' and eid=54 and fld12<>'' and fld12<>'0' ", con)
        da.Fill(dtImieNo)

        For i As Integer = 0 To dtImieNo.Rows.Count - 1

            Dim dtveh As New DataTable
            Dim icon As String
            Dim str As String = "select convert(nvarchar,g.tid)[TID],IMIENO,max(m2.fld10)[Site_Name],DMS.IdealTime(g.imieno) IdealTime,convert(nvarchar,Lattitude)[Lat],convert(nvarchar,longitude)[Long],convert(nvarchar,ctime,101)ctime,max(m2.fld14)[Group],0 as Isvehicle, m2.fld1 [VhNo] , g.Speed"
            str = str & " from mmm_mst_gpsdata g with (nolock) join mmm_mst_master m2 with (nolock) on m2.fld12=g.imieno   where m2.documenttype='vehicle' and m2.eid=54 group by g.tid,imieno,ctime,m2.fld1,msensor,lattitude,longitude,speed having ctime=(select max(ctime) from mmm_mst_gpsdata with (nolock) where imieno=g.imieno) and Ltrim(IMIENO) <> '' and  IMIENO='" & dtImieNo.Rows(i).Item("IMIENO") & "'"
            da.SelectCommand.CommandText = str
            da.SelectCommand.CommandTimeout = 180
            da.Fill(dtveh)
            If dtveh.Rows.Count = 0 Then
                Continue For
            End If

            Dim todayDt As String = (DirectCast(Now, Date)).ToShortDateString()

            returnString.Append(dtveh.Rows(0).Item("VhNo"))
            returnString.Append(",")
            returnString.Append(dtveh.Rows(0).Item("IMIENO"))
            returnString.Append(",")
            returnString.Append(dtveh.Rows(0).Item("Site_Name"))
            returnString.Append(",")
            returnString.Append(dtveh.Rows(0).Item("Speed"))
            returnString.Append(",")
            returnString.Append(dtveh.Rows(0).Item("Lat"))
            returnString.Append(",")
            returnString.Append(dtveh.Rows(0).Item("Long"))
            returnString.Append(",")

            If dtveh.Rows(0).Item("IdealTime") > 10 And dtveh.Rows(0).Item("IdealTime") <= 600 And dtveh.Rows(0).Item("ctime") = todayDt Then
                icon = "#999999"
            ElseIf dtveh.Rows(0).Item("IdealTime") >= 0 And dtveh.Rows(0).Item("IdealTime") < 10 And dtveh.Rows(0).Item("ctime") = todayDt Then
                icon = "#009900"
            ElseIf (dtveh.Rows(0).Item("IdealTime") > 600 And dtveh.Rows(0).Item("IdealTime") <= 1440) Or (dtveh.Rows(0).Item("ctime") = todayDt And dtveh.Rows(0).Item("IdealTime") > 1440) Then
                icon = "#FF4DFF"
            Else
                icon = "#FF1919"
            End If
            returnString.Append(icon)
            returnString.Append(":")
        Next
        Return returnString.ToString()
    End Function
    Protected Sub Button11_Click(sender As Object, e As EventArgs) Handles Button11.Click
        Dim constr As String = "server=172.17.109.152;initial catalog=DMS;uid=DMS;pwd=Ztsu93#u"
        Dim con As SqlConnection = New SqlConnection(constr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter(" ", con)
        Dim ds As New DataSet()
        Try
            oda.SelectCommand.CommandText = "select * from mmm_mst_reportscheduler with(nolock) where AlertType='SMS' and eid=42 order by hh,mm,ordering"
            oda.Fill(ds, "rpt")
            For d As Integer = 0 To ds.Tables("rpt").Rows.Count - 1
                If ReportScheduler(ds.Tables("rpt").Rows(d).Item("tid")) = True Then
                    Dim SMSTO As String = ds.Tables("rpt").Rows(d).Item("emailto").ToString()
                    Dim str As String = ds.Tables("rpt").Rows(d).Item("reportquery").ToString()
                    Dim eid As String = ds.Tables("rpt").Rows(d).Item("eid").ToString()
                    oda.SelectCommand.CommandText = str
                    oda.SelectCommand.CommandType = CommandType.Text
                    oda.SelectCommand.CommandTimeout = 120
                    Dim count As String
                    Dim Common As New DataTable
                    oda.Fill(Common)
                    count = Common.Rows(0).Item("Count").ToString()
                    If Common.Rows.Count > 0 Then
                        SendReply1("26312", "9958358128", "Dear Sir, " & count & " Enquiries have been integrated from ERP to the BPM application.")
                    End If
                End If
            Next
        Catch ex As Exception
        Finally
            con.Close()
            ds.Dispose()
            oda.Dispose()
            con.Dispose()
        End Try
    End Sub
    Protected Sub Button12_Click(sender As Object, e As EventArgs) Handles Button12.Click
        FerringFarmaAbsentReportConsolidated()
    End Sub
    Protected Sub Button13_Click(sender As Object, e As EventArgs) Handles Button13.Click
        Dim str As String = TripCreateForIndus()
    End Sub
    Function TripCreateForIndus() As String
        'If Me.SchedulerIndus Then
        Dim connectionString As String = "server=172.17.109.152;initial catalog=DMS;uid=DMS;pwd=Ztsu93#u"
        Dim selectConnection As New SqlConnection(connectionString)
        Dim adapter As New SqlDataAdapter("", selectConnection)
        ' Dim set As New DataSet()
        Dim dataTable As New DataTable
        Dim table2 As New DataTable
        Try
            Dim num As Integer = 0
            If (num = 0) Then
                adapter.SelectCommand.CommandText = "select distinct imieno,CONVERT(nvarchar,ctime,23)[cdate] from mmm_mst_gpsdata g with (nolock) inner join mmm_mst_master m with (nolock) on m.fld12=g.imieno  where convert(nvarchar,ctime,23)=convert(nvarchar,getdate()-1,23) and m.fld12<>'0' and speed<>0 and m.eid=54 and m.documenttype='vehicle' group by imieno,CONVERT(nvarchar,ctime,23)"
                adapter.Fill(dataTable)
                Dim num3 As Integer = (dataTable.Rows.Count - 1)
                Dim i As Integer = 0
                Do While (i <= num3)
                    adapter.SelectCommand.CommandText = "CreateAutomaticTripforIndus"
                    adapter.SelectCommand.CommandType = CommandType.StoredProcedure
                    adapter.SelectCommand.Parameters.Clear()
                    adapter.SelectCommand.Parameters.AddWithValue("@imieno", dataTable.Rows.Item(i).Item("imieno").ToString)
                    adapter.SelectCommand.Parameters.AddWithValue("Date", dataTable.Rows.Item(i).Item("cdate").ToString)
                    If (selectConnection.State <> ConnectionState.Open) Then
                        selectConnection.Open()
                    End If
                    adapter.SelectCommand.CommandTimeout = 300
                    adapter.SelectCommand.ExecuteNonQuery()
                    i += 1
                Loop
                adapter.SelectCommand.CommandType = CommandType.Text
                adapter.SelectCommand.CommandText = "select distinct imieno,CONVERT(nvarchar,ctime,23)[cdate] from mmm_mst_gpsdata g with (nolock) inner join mmm_mst_master m with (nolock) on m.fld12=g.imieno  where DATEADD(minute,DATEDIFF(minute,0,ctime),0)>convert(nvarchar,convert(date,getdate()-1))+' 23:00' and DATEADD(minute,DATEDIFF(minute,0,ctime),0)<convert(nvarchar,convert(date,getdate()))+' 04:00' and m.fld12<>'0' and speed<>0 and m.eid=54 and m.documenttype='vehicle' group by imieno,CONVERT(nvarchar,ctime,23)"
                dataTable.Clear()
                adapter.Fill(dataTable)
                Dim num4 As Integer = (dataTable.Rows.Count - 1)
                Dim j As Integer = 0
                Do While (j <= num4)
                    adapter.SelectCommand.CommandText = "CreateAutomaticNightTripforIndus"
                    adapter.SelectCommand.CommandType = CommandType.StoredProcedure
                    adapter.SelectCommand.Parameters.Clear()
                    adapter.SelectCommand.Parameters.AddWithValue("@imieno", dataTable.Rows.Item(j).Item("imieno").ToString)
                    adapter.SelectCommand.Parameters.AddWithValue("Date", dataTable.Rows.Item(j).Item("cdate").ToString)
                    If (selectConnection.State <> ConnectionState.Open) Then
                        selectConnection.Open()
                    End If
                    adapter.SelectCommand.CommandTimeout = 300
                    adapter.SelectCommand.ExecuteNonQuery()
                    j += 1
                Loop
            End If
        Catch exception1 As Exception
            Dim exception As Exception = exception1
            adapter.SelectCommand.CommandType = CommandType.Text
            adapter.SelectCommand.CommandText = ("insert into mmm_schedulerlog_Switch values ('" & exception.ToString & "',getdate())")
            If (selectConnection.State <> ConnectionState.Open) Then
                selectConnection.Open()
            End If
            adapter.SelectCommand.ExecuteNonQuery()
            selectConnection.Dispose()
            selectConnection.Close()
            adapter.Dispose()
        Finally
            selectConnection.Dispose()
            selectConnection.Close()
            adapter.Dispose()
        End Try
        'End If
        Return "True"
    End Function
    Function UpdateLocationForIndus() As String
        'If Me.SchedulerIndus1 Then
        Dim connectionString As String = "server=172.17.109.152;initial catalog=DMS;uid=DMS;pwd=Ztsu93#u"
        Dim selectConnection As New SqlConnection(connectionString)
        Dim adapter As New SqlDataAdapter("", selectConnection)
        ' Dim set As New DataSet
        Dim table As New DataTable
        Dim dataTable As New DataTable
        Try
            adapter.SelectCommand.CommandText = "select tid,start_latitude,start_longitude,end_latitude,end_longitude from mmm_mst_elogbook where triptype in ('Auto','Night AutoTrip') and (Trip_start_location is null and Trip_end_location is null or Trip_Start_Location='' and Trip_End_Location='')  and (convert(nvarchar,trip_start_datetime,23)=convert(nvarchar,getdate()-8,23) or convert(nvarchar,trip_start_datetime,23)=convert(nvarchar,getdate(),23)) and eid=54 order by tid desc"
            adapter.Fill(dataTable)
            Dim num2 As Integer = (dataTable.Rows.Count - 1)
            Dim i As Integer = 0
            Do While (i <= num2)
                adapter.SelectCommand.CommandText = "USPUpdateTripLocation"
                adapter.SelectCommand.CommandType = CommandType.StoredProcedure
                adapter.SelectCommand.Parameters.Clear()
                adapter.SelectCommand.Parameters.AddWithValue("@tid", dataTable.Rows.Item(i).Item("tid").ToString)
                adapter.SelectCommand.Parameters.AddWithValue("@stlocation", Me.Location(dataTable.Rows.Item(i).Item("start_latitude").ToString, dataTable.Rows.Item(i).Item("start_longitude").ToString))
                adapter.SelectCommand.Parameters.AddWithValue("@edlocation", Me.Location(dataTable.Rows.Item(i).Item("end_latitude").ToString, dataTable.Rows.Item(i).Item("end_longitude").ToString))
                adapter.SelectCommand.CommandTimeout = 120
                If (selectConnection.State <> ConnectionState.Open) Then
                    selectConnection.Open()
                End If
                adapter.SelectCommand.ExecuteNonQuery()
                i += 1
            Loop
        Catch exception1 As Exception
            Dim exception As Exception = exception1
            adapter.SelectCommand.CommandType = CommandType.Text
            adapter.SelectCommand.CommandText = ("insert into mmm_schedulerlog_Switch values ('" & exception.ToString & "',getdate())")
            If (selectConnection.State <> ConnectionState.Open) Then
                selectConnection.Open()
            End If
            adapter.SelectCommand.ExecuteNonQuery()
        Finally
            selectConnection.Close()
            adapter.Dispose()
            selectConnection.Dispose()
        End Try
        'End If
        Return True
    End Function
    Public Function SchedulerIndus() As Boolean
        Dim flag As Boolean = False
        Try
            Dim time As DateTime = DateAndTime.Now.AddMinutes(-9).ToShortTimeString
            Dim time2 As DateTime = DateAndTime.Now.AddMinutes(9).ToShortTimeString
            Dim time3 As DateTime = Convert.ToDateTime("13:50:00").ToShortTimeString
            If ((DateTime.Compare(time3, time2) <= 0) And (DateTime.Compare(time3, time) >= 0)) Then
                flag = True
            End If
        Catch exception1 As Exception
        End Try
        Return flag
    End Function
    Public Function SchedulerIndus1() As Boolean
        Dim flag As Boolean = False
        Try
            Dim time As DateTime = DateAndTime.Now.AddMinutes(-9).ToShortTimeString
            Dim time2 As DateTime = DateAndTime.Now.AddMinutes(9).ToShortTimeString
            Dim time3 As DateTime = Convert.ToDateTime("13:59:00").ToShortTimeString
            If ((DateTime.Compare(time3, time2) <= 0) And (DateTime.Compare(time3, time) >= 0)) Then
                flag = True
            End If
        Catch exception1 As Exception
        End Try
        Return flag
    End Function
    Protected Sub Button14_Click(sender As Object, e As EventArgs) Handles Button14.Click
        Dim str1 As String = UpdateLocationForIndus()
    End Sub

    Protected Sub Button15_Click(sender As Object, e As EventArgs) Handles Button15.Click
        TripCreateFromSwitchTest()
    End Sub
    Sub TripCreateFromSwitchTest()
        ' If Me.Scheduler Then
        Dim connectionString As String = "server=172.17.109.152;initial catalog=DMS;uid=DMS;pwd=Ztsu93#u"
        Dim selectConnection As New SqlConnection(connectionString)
        Dim adapter As New SqlDataAdapter("", selectConnection)
        'Dim set As New DataSet
        Dim dataTable As New DataTable
        Dim dt2 As New DataTable
        Dim dt3 As New DataTable
        Dim dt As New DataTable
        adapter.SelectCommand.CommandText = "select * from mmm_mst_JobScheduler where Jobtype='AutotripfromSwitch'"
        adapter.Fill(dt)
        Try
            For k As Integer = 0 To dt.Rows.Count - 1
                If RunSchedulerTime(dt.Rows(k).Item("TID").ToString) = True Then
                    adapter.SelectCommand.CommandText = "select vidtype,vivehiclefield,viimeifield from mmm_mst_entity where eid=" & dt.Rows(k).Item("EID") & ""
                    dt2.Clear()
                    adapter.Fill(dt2)
                    adapter.SelectCommand.CommandText = "select distinct imieno,CONVERT(nvarchar,ctime,23)[cdate] from mmm_mst_gpsdata with (nolock) where tripon=1 and convert(nvarchar,ctime,23)=convert(nvarchar,getdate(),23) and imieno in (select " & dt2.Rows(0).Item(2) & " from mmm_mst_master with(nolock) where eid=" & dt.Rows(k).Item("EID") & " and documenttype='" & dt2.Rows(0).Item(0).ToString & "' and len(" & dt2.Rows(0).Item(2) & ")=15)  group by imieno,CONVERT(nvarchar,ctime,23)"
                    adapter.SelectCommand.CommandTimeout = 300
                    dataTable.Clear()
                    adapter.Fill(dataTable)
                    Dim num3 As Integer = (dataTable.Rows.Count - 1)
                    Dim i As Integer = 0
                    Do While (i <= num3)
                        adapter.SelectCommand.CommandType = CommandType.Text
                        adapter.SelectCommand.CommandText = "select tid," & dt2.Rows(0).Item(1).ToString & " from mmm_mst_master with(nolock) where eid=" & dt.Rows(k).Item("EID") & " and documenttype='" & dt2.Rows(0).Item(0).ToString & "' and " & dt2.Rows(0).Item(2) & "='" & dataTable.Rows.Item(i).Item("imieno").ToString & "'"
                        dt3.Clear()
                        adapter.Fill(dt3)
                        adapter.SelectCommand.CommandText = "CreateTripfromSwitchNewTest"
                        adapter.SelectCommand.CommandType = CommandType.StoredProcedure
                        adapter.SelectCommand.Parameters.Clear()
                        adapter.SelectCommand.Parameters.AddWithValue("imieno", dataTable.Rows.Item(i).Item("imieno").ToString)
                        adapter.SelectCommand.Parameters.AddWithValue("Date", dataTable.Rows.Item(i).Item("cdate").ToString)
                        adapter.SelectCommand.Parameters.AddWithValue("tid", dt3.Rows(0).Item(0))
                        adapter.SelectCommand.Parameters.AddWithValue("vno", dt3.Rows(0).Item(1).ToString)
                        adapter.SelectCommand.Parameters.AddWithValue("eid", dt.Rows(k).Item("EID"))
                        If (selectConnection.State <> ConnectionState.Open) Then
                            selectConnection.Open()
                        End If
                        adapter.SelectCommand.CommandTimeout = 120
                        adapter.SelectCommand.ExecuteNonQuery()
                        i += 1
                    Loop
                    ' UpdateLoginDB("Trip Created successfully")
                End If
            Next
        Catch exception1 As Exception
            Dim exception As Exception = exception1
            adapter.SelectCommand.CommandType = CommandType.Text
            adapter.SelectCommand.CommandText = ("insert into mmm_schedulerlog_Switch values ('" & exception.ToString & "',getdate())")
            If (selectConnection.State <> ConnectionState.Open) Then
                selectConnection.Open()
            End If
            adapter.SelectCommand.ExecuteNonQuery()
            selectConnection.Dispose()
            selectConnection.Close()
            adapter.Dispose()
        Finally
            selectConnection.Dispose()
            selectConnection.Close()
            adapter.Dispose()
        End Try
        'End If
    End Sub
    Public Function RunSchedulerTime(ByVal tid As Integer) As Boolean
        Dim b As Boolean = False
        Dim conStr As String = "server=172.17.109.152;initial catalog=DMS;uid=DMS;pwd=Ztsu93#u"
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim time1 As DateTime = Now.AddMinutes(-5).ToShortTimeString()
        Dim time2 As DateTime = Now.AddMinutes(+5).ToShortTimeString()
        Dim da As New SqlDataAdapter("select HH,MM,Frequency,TID,JobDate from MMM_MST_JobScheduler where tid=" & tid & " ", con)
        Dim dt As New DataTable()
        da.Fill(dt)
        Dim SchType As String = dt.Rows(0).Item("Frequency").ToString()
        If UCase(SchType) = "DAILY" Then
            Dim x As DateTime = (Convert.ToDateTime(Trim(dt.Rows(0)(0)) & ":" & Trim(dt.Rows(0)(1)) & ":" & "00").ToShortTimeString)
            If x <= time2 And x >= time1 Then
                b = True
            End If
        End If
        If UCase(SchType) = "WEEKLY" Then
            Dim dayName As String = DateTime.Now.ToString("dddd")
            Dim currentweek As String = dt.Rows(0).Item("JobDate").ToString()
            If currentweek = 1 Then
                currentweek = "Monday"
            ElseIf currentweek = 2 Then
                currentweek = "Tuesday"
            ElseIf currentweek = 3 Then
                currentweek = "Wednesday"
            ElseIf currentweek = 4 Then
                currentweek = "Thursday"
            ElseIf currentweek = 5 Then
                currentweek = "Friday"
            ElseIf currentweek = 6 Then
                currentweek = "Saturday"
            ElseIf currentweek = 7 Then
                currentweek = "Sunday"
            End If
            If currentweek = dayName Then
                Dim x As DateTime = (Convert.ToDateTime(Trim(dt.Rows(0)(0)) & ":" & Trim(dt.Rows(0)(1)) & ":" & "00").ToShortTimeString)
                If x <= time2 And x >= time1 Then
                    b = True
                End If
            End If
        End If
        If UCase(SchType) = "MONTHLY" Then
            Dim currentDate As DateTime = DateTime.Now
            Dim dateofMonth As Integer = currentDate.Day
            Dim dateMail As Integer = dt.Rows(0).Item("JobDate").ToString()
            If dateofMonth = dateMail Then
                Dim x As DateTime = (Convert.ToDateTime(Trim(dt.Rows(0)(0)) & ":" & Trim(dt.Rows(0)(1)) & ":" & "00").ToShortTimeString)
                If x <= time2 And x >= time1 Then
                    b = True
                End If
            End If
        End If
        con.Close()
        con.Dispose()
        da.Dispose()
        dt.Dispose()
        Return b
    End Function

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        txtNo.Text = DateAndTime.Now.ToString
    End Sub
End Class
