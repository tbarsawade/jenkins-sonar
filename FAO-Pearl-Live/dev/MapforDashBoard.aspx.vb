Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports System.Web.UI.Adapters.ControlAdapter
Imports iTextSharp.text
Imports System.Drawing
Imports System.Web.Services
Partial Class MapforDashBoard
    Inherits System.Web.UI.Page
    Protected Shared dt As New DataTable
    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim BCode As String = Request.QueryString("Branch Code").ToString()
        Try
            Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
            Dim str As String = "select d.fld1[Policy Number],d.fld10[product Name],d.fld11[Owner First Name],d.fld18[Gender],d.fld19[Advisor Code],m1.fld10[Branch],rtrim(ltrim(d.fld20))[GeoPoint],''[GeoFence] from mmm_mst_doc d with (nolock)  join mmm_mst_master m1 with (nolock) on m1.tid=d.fld22 where d.eid=" & Session("EID") & " and m1.eid=" & Session("EID") & " and d.documenttype='policy' and m1.documenttype='branch' and m1.fld1='" & BCode & "' and d.fld20<>'' and d.fld20 not like '%error%' and d.fld20 like '%,7%'   union select '','','','','','','',fld13[Geofence] from mmm_mst_master where eid=" & Session("EID") & " and documenttype='branch' and fld1='" & BCode & "'"
            oda.SelectCommand.CommandText = str
            dt.Clear()
            oda.Fill(dt)
        Catch ex As Exception
        Finally
            con.Close()
            con.Dispose()
        End Try
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
    <WebMethod>
    Public Shared Function ConvertDataTabletoString() As String
        Dim Str As String = ""
        Dim Sb As New StringBuilder()
        Dim geopoint As String = ""
        Dim geofence As String = ""
        Dim mtch As String = ""
        Dim fnl As String = ""
        If dt.Rows.Count > 0 Then
            For i As Integer = 0 To dt.Rows.Count - 1
                If dt.Rows(i).Item("GeoFence").ToString.Trim <> "" And IsDBNull(dt.Rows(i).Item("GeoFence").ToString) = False Then
                    Dim arr1 = dt.Rows(i).Item("GeoFence").ToString.Split("#")
                    Dim var1 = ""
                    If mtch <> dt.Rows(i).Item("GeoFence").ToString.Trim Then
                        For j As Integer = 0 To arr1.Length - 1
                            Dim arr = arr1(j).ToString.Split(",")
                            For a = 0 To arr.Count - 1
                                If a < arr.Count - 1 Then
                                    If var1 = "" Or (var1.Contains("#") = True And a = 0) Then
                                        var1 = arr(a + 1) & " " & arr(a)
                                    Else
                                        If var1.Contains("#") = True Then
                                            If a = 0 Then
                                                var1 = var1 & arr(a + 1) & " " & arr(a)
                                            Else
                                                var1 = var1 & "," & arr(a + 1) & " " & arr(a)
                                            End If
                                        Else
                                            var1 = var1 & "," & arr(a + 1) & " " & arr(a)
                                        End If

                                    End If
                                    a = a + 1
                                End If
                            Next
                            If j <> arr1.Length - 1 Then
                                var1 = var1 & "#"
                            End If
                            fnl = fnl & var1
                        Next
                        ' fnl = "72.8489 19.1754,72.8553 19.1759,72.8568 19.1758,72.8585 19.1753,72.8588 19.1743,72.8655 19.1757,72.8719 19.1757,72.874 19.1758,72.8744 19.1726,72.874 19.1721,72.8738 19.1711,72.8724 19.1698,72.8723 19.1689,72.8712 19.1683,72.871 19.1677,72.8702 19.1675,72.8697 19.1672,72.8684 19.1668,72.8671 19.1659,72.8668 19.166,72.8663 19.1657,72.8652 19.1637,72.8651 19.1634,72.8641 19.1627,72.8637 19.1628,72.8637 19.1629,72.863 19.1627,72.8625 19.1623,72.8605 19.1622,72.86 19.1621,72.8594 19.1626,72.8584 19.1601,72.8578 19.161,72.8567 19.1536,72.8559 19.1448,72.8549 19.1418,72.8538 19.142,72.8523 19.1431,72.8501 19.144,72.8502 19.1496,72.8494 19.1646#72.8351 19.2007,72.8353 19.2007,72.837 19.201,72.8397 19.2003,72.842 19.1994,72.8422 19.1993,72.8424 19.1993,72.8425 19.198,72.8432 19.1981,72.8433 19.1955,72.8451 19.1956,72.8452 19.1954,72.8466 19.1955,72.8471 19.1954,72.8475 19.1944,72.8492 19.1943,72.8493 19.1944,72.8496 19.1944,72.8502 19.1942,72.8486 19.1846,72.849 19.174,72.8454 19.1735,72.8436 19.1736,72.8431 19.1738,72.8368 19.1738,72.8297 19.1756,72.8301 19.179,72.8298 19.1796,07 19.1,19.1816 72.8299,19.1822 72.8297,19.1834 72.8295,19.185 72.8285,19.1861 72.8283,19.1876 72.8287,19.1925 72.8296,19.1948 72.829,19.1964 72.8297,19.1979 72.831,19.1986 72.8314,19.1989 72.831,19.1994"
                        mtch = dt.Rows(i).Item("GeoFence").ToString.Trim
                        If geofence.Contains(var1) = False Then
                            geofence = geofence & "#" & fnl
                        End If
                    End If
                    'Sb.Append("#").Append(var1)
                End If
                If dt.Rows(i).Item("GeoPoint").ToString.Trim <> "" And dt.Rows(i).Item("GeoPoint").ToString.Contains("Error") = False Then
                    geopoint = geopoint & "['Policy No: " & dt.Rows(i).Item("Policy Number").ToString & "<br>Advisor Code: " & dt.Rows(i).Item("Advisor Code").ToString & "'," & dt.Rows(i).Item("GeoPoint").ToString & ",'images/human.png'],"
                End If
            Next
            'geopoint =  geopoint 
            If geopoint <> "" Then
                Sb.Append(geopoint).Append("|").Append(geofence)
            End If
        End If
        Str = Sb.ToString()
        'Page.ClientScript.RegisterClientScriptBlock(Page.GetType(), "jhdgh", "bindMap()")
        Return Str
    End Function
End Class
