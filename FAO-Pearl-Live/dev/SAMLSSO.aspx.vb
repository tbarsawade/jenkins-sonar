
Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports System.Xml

Partial Class SAMLSSO
    Inherits System.Web.UI.Page

    Private Sub SAMLSSO_Load(sender As Object, e As EventArgs) Handles Me.Load


        Dim rawSamlData As String = Request("SAMLResponse")
        'rawSamlData = "PD94bWwgdmVyc2lvbj0iMS4wIiBlbmNvZGluZz0iVVRGLTgiIHN0YW5kYWxvbmU9Im5vIj8+PHNhbWwycDpSZXNwb25zZSB4bWxuczpzYW1sMnA9InVybjpvYXNpczpuYW1lczp0YzpTQU1MOjIuMDpwcm90b2NvbCIgRGVzdGluYXRpb249Imh0dHBzOi8vUXdpa2NpbHZlci5teW5kY2VudHJhbC5jb20vU1NPTE9HSU4iIElEPSJfY2QzMDdmMWViZTFmNzY0ZDQ3NjhhZTFmNDg0MTE0MGYiIEluUmVzcG9uc2VUbz0iXzIyZTE4NGFkLTBmMjItNDk1Ny04ZTc3LWM4NmE5MmQxMDM2ZCIgSXNzdWVJbnN0YW50PSIyMDIwLTA1LTE5VDE2OjAzOjQ1LjcxOFoiIFZlcnNpb249IjIuMCI+PHNhbWwyOklzc3VlciB4bWxuczpzYW1sMj0idXJuOm9hc2lzOm5hbWVzOnRjOlNBTUw6Mi4wOmFzc2VydGlvbiI+aHR0cHM6Ly9hY2NvdW50cy5nb29nbGUuY29tL28vc2FtbDI/aWRwaWQ9QzAweXlra3M0PC9zYW1sMjpJc3N1ZXI+PHNhbWwycDpTdGF0dXM+PHNhbWwycDpTdGF0dXNDb2RlIFZhbHVlPSJ1cm46b2FzaXM6bmFtZXM6dGM6U0FNTDoyLjA6c3RhdHVzOlN1Y2Nlc3MiLz48L3NhbWwycDpTdGF0dXM+PHNhbWwyOkFzc2VydGlvbiB4bWxuczpzYW1sMj0idXJuOm9hc2lzOm5hbWVzOnRjOlNBTUw6Mi4wOmFzc2VydGlvbiIgSUQ9Il85MGZmOTU0ZjFmZmE5YTc3ODlmOGQxNWI3YTZkNzg2OSIgSXNzdWVJbnN0YW50PSIyMDIwLTA1LTE5VDE2OjAzOjQ1LjcxOFoiIFZlcnNpb249IjIuMCI+PHNhbWwyOklzc3Vlcj5odHRwczovL2FjY291bnRzLmdvb2dsZS5jb20vby9zYW1sMj9pZHBpZD1DMDB5eWtrczQ8L3NhbWwyOklzc3Vlcj48ZHM6U2lnbmF0dXJlIHhtbG5zOmRzPSJodHRwOi8vd3d3LnczLm9yZy8yMDAwLzA5L3htbGRzaWcjIj48ZHM6U2lnbmVkSW5mbz48ZHM6Q2Fub25pY2FsaXphdGlvbk1ldGhvZCBBbGdvcml0aG09Imh0dHA6Ly93d3cudzMub3JnLzIwMDEvMTAveG1sLWV4Yy1jMTRuIyIvPjxkczpTaWduYXR1cmVNZXRob2QgQWxnb3JpdGhtPSJodHRwOi8vd3d3LnczLm9yZy8yMDAxLzA0L3htbGRzaWctbW9yZSNyc2Etc2hhMjU2Ii8+PGRzOlJlZmVyZW5jZSBVUkk9IiNfOTBmZjk1NGYxZmZhOWE3Nzg5ZjhkMTViN2E2ZDc4NjkiPjxkczpUcmFuc2Zvcm1zPjxkczpUcmFuc2Zvcm0gQWxnb3JpdGhtPSJodHRwOi8vd3d3LnczLm9yZy8yMDAwLzA5L3htbGRzaWcjZW52ZWxvcGVkLXNpZ25hdHVyZSIvPjxkczpUcmFuc2Zvcm0gQWxnb3JpdGhtPSJodHRwOi8vd3d3LnczLm9yZy8yMDAxLzEwL3htbC1leGMtYzE0biMiLz48L2RzOlRyYW5zZm9ybXM+PGRzOkRpZ2VzdE1ldGhvZCBBbGdvcml0aG09Imh0dHA6Ly93d3cudzMub3JnLzIwMDEvMDQveG1sZW5jI3NoYTI1NiIvPjxkczpEaWdlc3RWYWx1ZT50Y3JvWlovV3hJSWlaVHFoWW45bVdrUEhNQXBrRlZBYUdDenlqaUZEMzRvPTwvZHM6RGlnZXN0VmFsdWU+PC9kczpSZWZlcmVuY2U+PC9kczpTaWduZWRJbmZvPjxkczpTaWduYXR1cmVWYWx1ZT5qdjRjNnlxb01RakxiQXNpMlNsR282UFBoc1pHd3Q3d05UMzRYZy9VYzBIc0pGOHNKSnpZak5Hc3lMSjVUY3hpZzA1MllxNFhPZXFnCm1EcTJFVHZaSjJyNG16Yk55UlZ6RnEzeUJjeTFXSnF1T25mcjZreFpxZ3VmWkRSMGVwSm1xZUk1MWRUWFJEMEkvYjZWSmtST2k1dHMKZlAxWDdiQXd1TEVEU3hKckxnQUpEOHpGZWhOREFwdVJsVUNCclRBVU9FQ2RvVXV3Sk5uZ1JCcWZHQUxBeWh1akoyM3hNWk4zbDBJTwpSTjQzeUxNUWFDdlZxVG9wUW91M1gxVUhtZENNejJjU2F3V2tmekd6NXZXVE1lZ2V2WEQrR1pqYTAzK1dGamZvQVBNZkVkS0xTZ1F3ClFlaXRDbWhqcGdqQnVvTUVpelhSSTV0YU1YWmtvY2lLbjJLb3JRPT08L2RzOlNpZ25hdHVyZVZhbHVlPjxkczpLZXlJbmZvPjxkczpYNTA5RGF0YT48ZHM6WDUwOVN1YmplY3ROYW1lPlNUPUNhbGlmb3JuaWEsQz1VUyxPVT1Hb29nbGUgRm9yIFdvcmssQ049R29vZ2xlLEw9TW91bnRhaW4gVmlldyxPPUdvb2dsZSBJbmMuPC9kczpYNTA5U3ViamVjdE5hbWU+PGRzOlg1MDlDZXJ0aWZpY2F0ZT5NSUlEZERDQ0FseWdBd0lCQWdJR0FXS1AyTHhHTUEwR0NTcUdTSWIzRFFFQkN3VUFNSHN4RkRBU0JnTlZCQW9UQzBkdmIyZHNaU0JKCmJtTXVNUll3RkFZRFZRUUhFdzFOYjNWdWRHRnBiaUJXYVdWM01ROHdEUVlEVlFRREV3WkhiMjluYkdVeEdEQVdCZ05WQkFzVEQwZHYKYjJkc1pTQkdiM0lnVjI5eWF6RUxNQWtHQTFVRUJoTUNWVk14RXpBUkJnTlZCQWdUQ2tOaGJHbG1iM0p1YVdFd0hoY05NVGd3TkRBMApNRGcwT1RJM1doY05Nak13TkRBek1EZzBPVEkzV2pCN01SUXdFZ1lEVlFRS0V3dEhiMjluYkdVZ1NXNWpMakVXTUJRR0ExVUVCeE1OClRXOTFiblJoYVc0Z1ZtbGxkekVQTUEwR0ExVUVBeE1HUjI5dloyeGxNUmd3RmdZRFZRUUxFdzlIYjI5bmJHVWdSbTl5SUZkdmNtc3gKQ3pBSkJnTlZCQVlUQWxWVE1STXdFUVlEVlFRSUV3cERZV3hwWm05eWJtbGhNSUlCSWpBTkJna3Foa2lHOXcwQkFRRUZBQU9DQVE4QQpNSUlCQ2dLQ0FRRUFvTjc5c21TS3ZEVERMZ0d5MVU3ZzBUcTNKMUNzYm9IbUh6eFltcXZLTTd2S1Fwb2hzWkgxUnRhS2ZpcDZ5NHExClFzL0hmTEJwM25UeDBLaWh6NGZRVmRtakRobWZ4VlFFamQvd3dEbXpTeVU0Z0l2Rytob0Ric1FlMWpQWE1la0pYTHdNRVVxNFRqNjEKUGRBemlsTXMwUFc0eGcwQjVFekViVGNuZ2QwNm5zZ0lJc3IvSWp6ZFI4SzhyYlYzWTVXekp1WURQM0U1YlJGQkxMTDR1ckJmcHNJZgpJS1dhL3A5RGFhRHJoWUh0eHdSdVo2ZnJWZk5BaGY0NndTTmxPQVVRb3VwOXdmZDhGVGlCbmZEbHlDc1FxZitOTG02a0FBSFFFVWpQCldKVHFySU1NV0dLSWdKbjdVQ1J6MzNQdU1uTHdEQlROdVZpUUh2amJTSUU5c0MzWGV3SURBUUFCTUEwR0NTcUdTSWIzRFFFQkN3VUEKQTRJQkFRQXd5bTk3N2x6ZmxUR3YxOFova3R0MmN2YlRBMzFOOVIyeWFIeStUZTVHWEhZZGpjZkw4N3lJOTlud0p5VkMvdnMrcXpXdgplaVdiZ0EvU1lqMlBCSmhXR0lzcjF5VG16U05IUHhrNVM2U1RpenIxdWtMQk9FU3FXUkNsei9OVUQzRXNDM2dHODVGTHJ4YnVuNlVECmZvSXNxZ044d25za1BuemUrZFNQcmdqRHJZWjBPaDNKNUR2ZWhGRk9PaVFkdUJBcG9WLzhKbjcyNzhsaEN6RWJBYVlsRGV5UiszbGoKdEhDWWhGQnFrTGRxejJWRFFackRpcDVTOFhQR2xNZlZ1T0RWOXM4TGxQaFZabTg3ZGNnd3lDUS9FYzdGdFVMUm84a3o4VS96Y0x1awovcXlLVC9BLysvSzIrSS9NM0srQms0NG5NVGNyVEhiSHprVE8yeEltRDQ0VjwvZHM6WDUwOUNlcnRpZmljYXRlPjwvZHM6WDUwOURhdGE+PC9kczpLZXlJbmZvPjwvZHM6U2lnbmF0dXJlPjxzYW1sMjpTdWJqZWN0PjxzYW1sMjpOYW1lSUQgRm9ybWF0PSJ1cm46b2FzaXM6bmFtZXM6dGM6U0FNTDoxLjE6bmFtZWlkLWZvcm1hdDp1bnNwZWNpZmllZCI+c3Jpbml2YXNhcmFvLmJhdHR1bGFAcXdpa2NpbHZlci5jb208L3NhbWwyOk5hbWVJRD48c2FtbDI6U3ViamVjdENvbmZpcm1hdGlvbiBNZXRob2Q9InVybjpvYXNpczpuYW1lczp0YzpTQU1MOjIuMDpjbTpiZWFyZXIiPjxzYW1sMjpTdWJqZWN0Q29uZmlybWF0aW9uRGF0YSBJblJlc3BvbnNlVG89Il8yMmUxODRhZC0wZjIyLTQ5NTctOGU3Ny1jODZhOTJkMTAzNmQiIE5vdE9uT3JBZnRlcj0iMjAyMC0wNS0xOVQxNjowODo0NS43MThaIiBSZWNpcGllbnQ9Imh0dHBzOi8vUXdpa2NpbHZlci5teW5kY2VudHJhbC5jb20vU1NPTE9HSU4iLz48L3NhbWwyOlN1YmplY3RDb25maXJtYXRpb24+PC9zYW1sMjpTdWJqZWN0PjxzYW1sMjpDb25kaXRpb25zIE5vdEJlZm9yZT0iMjAyMC0wNS0xOVQxNTo1ODo0NS43MThaIiBOb3RPbk9yQWZ0ZXI9IjIwMjAtMDUtMTlUMTY6MDg6NDUuNzE4WiI+PHNhbWwyOkF1ZGllbmNlUmVzdHJpY3Rpb24+PHNhbWwyOkF1ZGllbmNlPmh0dHBzOi8vUXdpa2NpbHZlci5teW5kY2VudHJhbC5jb208L3NhbWwyOkF1ZGllbmNlPjwvc2FtbDI6QXVkaWVuY2VSZXN0cmljdGlvbj48L3NhbWwyOkNvbmRpdGlvbnM+PHNhbWwyOkF0dHJpYnV0ZVN0YXRlbWVudD48c2FtbDI6QXR0cmlidXRlIE5hbWU9Im1haWwiPjxzYW1sMjpBdHRyaWJ1dGVWYWx1ZSB4bWxuczp4cz0iaHR0cDovL3d3dy53My5vcmcvMjAwMS9YTUxTY2hlbWEiIHhtbG5zOnhzaT0iaHR0cDovL3d3dy53My5vcmcvMjAwMS9YTUxTY2hlbWEtaW5zdGFuY2UiIHhzaTp0eXBlPSJ4czphbnlUeXBlIj5zcmluaXZhc2FyYW8uYmF0dHVsYUBxd2lrY2lsdmVyLmNvbTwvc2FtbDI6QXR0cmlidXRlVmFsdWU+PC9zYW1sMjpBdHRyaWJ1dGU+PC9zYW1sMjpBdHRyaWJ1dGVTdGF0ZW1lbnQ+PHNhbWwyOkF1dGhuU3RhdGVtZW50IEF1dGhuSW5zdGFudD0iMjAyMC0wNS0xOVQxNDo0NzowOC4wMDBaIiBTZXNzaW9uSW5kZXg9Il85MGZmOTU0ZjFmZmE5YTc3ODlmOGQxNWI3YTZkNzg2OSI+PHNhbWwyOkF1dGhuQ29udGV4dD48c2FtbDI6QXV0aG5Db250ZXh0Q2xhc3NSZWY+dXJuOm9hc2lzOm5hbWVzOnRjOlNBTUw6Mi4wOmFjOmNsYXNzZXM6dW5zcGVjaWZpZWQ8L3NhbWwyOkF1dGhuQ29udGV4dENsYXNzUmVmPjwvc2FtbDI6QXV0aG5Db250ZXh0Pjwvc2FtbDI6QXV0aG5TdGF0ZW1lbnQ+PC9zYW1sMjpBc3NlcnRpb24+PC9zYW1sMnA6UmVzcG9uc2U+"
        Dim strSamlXML As String = Base64Decode(rawSamlData)
        Dim doc As XmlDocument = New XmlDocument()
        doc.LoadXml(strSamlXML)
        Dim FileName As String = "SSO" & DateTime.Now.Year.ToString() + DateTime.Now.Month + DateTime.Now.Date.DayOfYear + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second + DateTime.Now.Millisecond & ".xml"
        Dim directoryPath As String = Server.MapPath("~/SSOResponse/")
        Dim path As String = directoryPath + FileName
        If Not Directory.Exists(directoryPath) Then
            Directory.CreateDirectory(directoryPath)
        End If
        Using writer As XmlTextWriter = New XmlTextWriter(path, Nothing)
            writer.Formatting = Formatting.Indented
            doc.Save(writer)
        End Using
        Dim responseDoc As XDocument = XDocument.Load(path)
        Dim pr As XNamespace = "urn:oasis:names:tc:SAML:2.0:protocol"
        Dim ast As XNamespace = "urn:oasis:names:tc:SAML:2.0:assertion"
        'Dim attStatement As XElement = responseDoc.Element(pr + "Response").Element(ast + "Assertion").Element(ast + "AttributeStatement")
        ' updated below three lines for commented one line - for Noon one - login - 02-03-21
        Dim attStatement As XElement = responseDoc.Element(pr + "Response").Element(ast + "Assertion").Element(ast + "AttributeStatement")
        If IsNothing(attStatement) Then
            attStatement = responseDoc.Element(pr + "Response").Element(ast + "Assertion").Element(ast + "Subject")
        End If

        Dim _objvalue As String = attStatement.Value
        '_objvalue = "mayank@chargebee.tools"
        'Now check wheater emailid exist or not ?'
        'Now check which company code'
        Dim Name As String = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority).ToUpper()
        Name = Name.ToString().ToUpper().Replace("HTTPS://", "").Replace("HTTP://", "")
        Dim arr() As String = Name.Split(".")
        Dim ECode As String = arr(0)
        'ECode = "testchargebee"
        validateUser(_objvalue, ecode:=ECode)

    End Sub

    Public Shared Function Base64Decode(ByVal base64EncodedData As String) As String
        Try
            Dim base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData)
            Return System.Text.Encoding.UTF8.GetString(base64EncodedBytes)
        Catch e As Exception
            Throw New Exception("Error in base64Encode" & e.Message)
        End Try
    End Function

    Public Function validateUser(ByVal UN As String, Optional ecode As String = "Qwikcilver") As Integer
        'Dim ecode As String = "Qwikcilver"
        Dim objDC As New DataClass()
        Dim dt As New DataTable
        Dim sqlq As String
        sqlq = "SELECT U.UID,U.UserName,U.userRole,E.defaultpage,U.EmailID, E.Ctheme, u.pwd [pwd],u.isauth [uisauth],u.sKey,minPassAttempt,passtry,passExpDays,passExpMsgDays,autoUnlockHour,datediff(hour,ModifyDate,getdate()) [hourElapsed],locationID,E.EID,isnull(E.logo,'mynd.gif') logo, e.DocDeatilVersion [Docversion] FROM MMM_MST_USER U left outer join MMM_MST_ENTITY E on u.EID=E.EID where U.userid='" & UN & "' and E.CODE='" & ecode & "'"
        dt = objDC.ExecuteQryDT(sqlq)
        Try
            If dt.Rows.Count() = 0 Then
                Dim userName() As String = UN.Split(".")
                objDC.ExecuteQryDT("insert into mmm_mst_user (UserName,emailID,userrole,isAuth,EID,userid) values('" & userName(0).ToString() & "','" & UN & "','END_USER',1,190,'" & UN & "')")
                sqlq = "SELECT U.UID,U.UserName,U.userRole,E.defaultpage,U.EmailID, E.Ctheme, u.pwd [pwd],u.isauth [uisauth],u.sKey,minPassAttempt,passtry,passExpDays,passExpMsgDays,autoUnlockHour,datediff(hour,ModifyDate,getdate()) [hourElapsed],locationID,E.EID,isnull(E.logo,'mynd.gif') logo, e.DocDeatilVersion [Docversion] FROM MMM_MST_USER U left outer join MMM_MST_ENTITY E on u.EID=E.EID where U.userid='" & UN & "' and E.CODE='" & ecode & "'"
                dt = objDC.ExecuteQryDT(sqlq)
            End If
            Dim i As Integer
            i = dt.Rows.Count

            Select Case i
                Case 0
                    Return 0
                Case 1
                    Dim enitity As Integer = Val(dt.Rows(0).Item("EID").ToString())
                    'Dim sKey As String = ds.Tables("user").Rows(0).Item("sKey").ToString()
                    'Dim sPwd As String = ""
                    'If sKey <> "" And Convert.ToString(ds.Tables("user").Rows(0).Item("pwd")) <> "" Then
                    '    sPwd = DecryptTripleDES(Convert.ToString(ds.Tables("user").Rows(0).Item("pwd")), sKey)
                    'End If
                    HttpContext.Current.Session("CTheme") = Convert.ToString(dt.Rows(0).Item("CTHEME").ToString())
                    HttpContext.Current.Session("UID") = dt.Rows(0).Item("UID")
                    HttpContext.Current.Session("USERNAME") = dt.Rows(0).Item("UserName")
                    HttpContext.Current.Session("USERROLE") = dt.Rows(0).Item("userrole")
                    HttpContext.Current.Session("EMAIL") = dt.Rows(0).Item("emailID")
                    'HttpContext.Current.Session("USERIMAGE") = dt.Rows(0).Item("USERIMAGE")
                    HttpContext.Current.Session("CLOGO") = dt.Rows(0).Item("logo")
                    HttpContext.Current.Session("EID") = dt.Rows(0).Item("EID")
                    'HttpContext.Current.Session("IPADDRESS") = oUser.ipAddress
                    'HttpContext.Current.Session("MACADDRESS") = oUser.macAddress
                    'HttpContext.Current.Session("HEADERSTRIP") = obj.ViewState("HEADERSTRIP")
                    'HttpContext.Current.Session("EXTID") = oUser.ExtID
                    'If oUser.islocal = 0 Then
                    '    HttpContext.Current.Session("ISLOCAL") = "TRUE"
                    'Else
                    '    HttpContext.Current.Session("ISLOCAL") = "TRUE"
                    'End If
                    HttpContext.Current.Session("INTIME") = Now
                    'HttpContext.Current.Session("LID") = oUser.locationID
                    'HttpContext.Current.Session("OFFSET") = oUser.offSet
                    ' HttpContext.Current.Session("CODE") = dt.Rows(0).Item("CODE") 'oUser.strCode
                    HttpContext.Current.Session("Roles") = dt.Rows(0).Item("userrole")
                    HttpContext.Current.Session("Docversion") = dt.Rows(0).Item("Docversion")

                    Dim objW As New Widget()
                    Dim EID = Convert.ToInt32(dt.Rows(0).Item("EID"))
                    Dim UID As Integer = dt.Rows(0).Item("UID")
                    Dim DBName = "DashBoard"
                    'Dim Roles = Convert.ToString(dt.Rows(0).Item("userrole"))

                    '' code for getting addl roles by sunil - 29_July_20

                    Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
                    Dim str As String = ""
                    str &= "select distinct rolename [userrole] from MMM_REF_ROLE_USER where eid=" & EID & " AND UID=" & UID & " union select rolename from MMM_ref_PreRole_user where eid=" & EID & " AND UID=" & UID & ""
                    Dim ds As New DataSet()
                    Using con = New SqlConnection(conStr)
                        Using da As New SqlDataAdapter(str, con)
                            da.Fill(ds)
                        End Using
                    End Using
                    Dim Roles = ""
                    For Each dr As DataRow In ds.Tables(0).Rows
                        If dr.Item("userrole").ToString.ToUpper <> UCase(HttpContext.Current.Session("USERROLE")) Then
                            Roles &= UCase(dr.Item("userrole").ToString()) & ","
                        End If
                    Next
                    Roles &= Session("USERROLE").ToUpper()
                    'Session("LID") = ouser.locationID
                    'Session("OFFSET") = ouser.offSet
                    Session("Roles") = Roles
                    'Dim objDC As New DataClass()
                    str = "UPDATE MMM_MST_USER SET passtry=0,isauth=1,userloggedin=1,sessionvalue='" & HttpContext.Current.Request.Cookies("ASP.NET_SessionId").Value.ToString & "' where UID=" & UID & " and eid=" & EID & ""
                    Using con = New SqlConnection(conStr)
                        Using da As New SqlDataAdapter(str, con)
                            If con.State <> ConnectionState.Open Then
                                con.Open()
                            End If
                            da.SelectCommand.ExecuteNonQuery()
                            con.Dispose()
                        End Using
                    End Using

                    '' code for getting addl roles by sunil ends here


                    Dim dsD As New DataSet()
                    dsD = objW.GetWidgets(EID, DBName, Roles, 0)
                    If dsD.Tables(0).Rows.Count > 0 Then
                        Response.RedirectPermanent("~/" & "dashboard1.aspx" & "")

                        'name.Add("1", "dashboard1.aspx")
                        'HttpContext.Current.Response.RedirectPermanent("~/" & "dashboard1.aspx" & "")
                    Else
                        Response.RedirectPermanent("~/" & dt.Rows(0).Item("defaultpage") & "")
                        'name.Add("1", "" & defaultpage & "")
                        'HttpContext.Current.Response.RedirectPermanent("~/" & defaultpage & "")
                    End If

                    'Dim con As SqlConnection = New SqlConnection(ConfigurationManager.ConnectionStrings("conStr").ConnectionString)
                    'objDC.ExecuteNonQuery("UPDATE MMM_MST_USER SET passtry=0,isauth=1,userloggedin=1,sessionvalue='" & HttpContext.Current.Request.Cookies("ASP.NET_SessionId").Value.ToString & "' where userid='" & username & "' and eid=" & HttpContext.Current.Session("EID") & "")

                    'Dim sqlq As String
                    'sqlq = "UPDATE MMM_MST_USER SET passtry=0,isauth=1 where userid='" & Request.Form(txtUserID.UniqueID) & "' and eid=" & Session("EID") & ""
                    'Dim oda As SqlDataAdapter = New SqlDataAdapter(sqlq, con)
                    'If con.State <> ConnectionState.Open Then
                    '    con.Open()
                    'End If
                    'oda.SelectCommand.ExecuteNonQuery()

                    'Dim sPwd As String = dt.Tables("user").Rows(0).Item("pwd").ToString()

                    'If sPwd = pwd Then
                    '    'If Val(dt.Tables("user").Rows(0).Item("minPassAttempt").ToString()) <= Val(dt.Tables("user").Rows(0).Item("passtry").ToString()) Then
                    '    '    'password retry reached to limit set isauth to lock mode and exit sub
                    '    '    Return 4
                    '    '    Exit Function
                    '    'End If

                    '    Select Case Val(dt.Tables("user").Rows(0).Item("uisauth").ToString())
                    '        Case 100
                    '            Return 100

                    '        Case 0
                    '            Return 7


                    '        Case 1
                    '            If Val(dt.Tables("user").Rows(0).Item("passExpMsgDays").ToString()) * 24 <= Val(dt.Tables("user").Rows(0).Item("hourElapsed").ToString()) Then
                    '                'Send him a message to change the password
                    '                HttpContext.Current.Session("MESSAGE") = "Please change your password, Your password will be expired soon"
                    '            End If

                    '            If Val(dt.Tables("user").Rows(0).Item("passExpDays").ToString()) * 24 <= Val(dt.Tables("user").Rows(0).Item("hourElapsed").ToString()) Then
                    '                Return 10
                    '                'Lock the password and send message password expired
                    '            Else
                    '                getValueByEmail(UN, enitity)
                    '                Return 1
                    '            End If

                    '        Case 3
                    '            If Val(dt.Tables("user").Rows(0).Item("hourElapsed").ToString()) >= Val(dt.Tables("user").Rows(0).Item("autoUnlockHour").ToString()) Then
                    '                'Unlock and let him login - change the isauth to 1
                    '                getValueByEmail(UN, enitity)
                    '                Return 1
                    '            Else
                    '                Return 5
                    '            End If
                    '        Case 2
                    '            Return 6
                    '        Case Else
                    '            Return 2
                    '    End Select

                    'Else
                    '    If Val(dt.Tables("user").Rows(0).Item("minPassAttempt").ToString()) <= Val(dt.Tables("user").Rows(0).Item("passtry").ToString()) Then
                    '        'password retry reached to limit set isauth to lock mode and exit sub
                    '        Return 4
                    '        Exit Function
                    '    End If
                    '    Return 3
                    'End If
                Case Else
                    Return 0
            End Select
        Catch ex As Exception
            Throw
        Finally
        End Try
    End Function
End Class
