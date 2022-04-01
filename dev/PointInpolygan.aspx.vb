Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports System.Web.UI.Adapters.ControlAdapter
Imports iTextSharp.text
Imports System.Drawing
Imports System.Web.Services

Partial Class PointInpolygan
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim RID As Integer = 0
        If Not Request.QueryString("RID") Is Nothing Then
            RID = Convert.ToInt32(Request.QueryString("RID"))
            Dim Sb As New StringBuilder()
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim dsR As New DataSet()
            Dim Data As New DataSet()
            Dim fnstr As String = ""
            Using con = New SqlConnection(conStr)
                Using da = New SqlDataAdapter("SELECT qryfield FROM MMM_MST_REPORT WHERE EID=53 and ReportID= " & RID, con)
                    da.Fill(dsR)
                    Dim Query = ""
                    If dsR.Tables(0).Rows.Count > 0 Then
                        Query = dsR.Tables(0).Rows(0).Item("qryfield")
                        da.SelectCommand.CommandText = Query
                        da.Fill(Data)
                        If Data.Tables(0).Rows.Count > 0 Then
                            For i As Integer = 0 To Data.Tables(0).Rows.Count - 1
                                If Data.Tables(0).Rows(i).Item("GeoFence").ToString.Trim <> "" Then
                                    Dim arr = Data.Tables(0).Rows(i).Item("GeoFence").ToString.Split(",")
                                    Dim var1 = ""
                                    For a = 0 To arr.Count - 1
                                        If a < arr.Count - 1 Then
                                            If var1 = "" Then
                                                var1 = arr(a + 1) & " " & arr(a)
                                            Else
                                                var1 = var1 & "," & arr(a + 1) & " " & arr(a)
                                            End If
                                            a = a + 1
                                        End If
                                    Next
                                    Sb.Append("#").Append(var1)
                                End If
                                If Data.Tables(0).Rows(i).Item("GeoFence").ToString.Trim <> "" Then
                                    fnstr = fnstr & "['Product Name: " & Data.Tables(0).Rows(i).Item("Product Name").ToString & "<br>Advisor Code: " & Data.Tables(0).Rows(i).Item("Advisor Code").ToString & "'," & Data.Tables(0).Rows(i).Item("GeoPoint").ToString & ",'images/human.png'], "
                                End If
                            Next
                            fnstr = "[" & fnstr & "];"
                            Sb.Append("|").Append(fnstr)
                        End If
                    End If
                End Using
            End Using
            Dim Str = Sb.ToString()
            hdn.Value = Str

            'Page.ClientScript.RegisterClientScriptBlock(Page.GetType(), "jhdgh", "bindMap()")
        End If
        ' System.Web.UI.ScriptManager.RegisterClientScriptBlock(Page, GetType(Page), "Script", "bindMap();", True)
        'Page.ClientScript.RegisterClientScriptBlock(Me.GetType(), "Script", "bindMap();", True)
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
        Dim RID As Integer = 0
        RID = 149
        Dim Sb As New StringBuilder()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim dsR As New DataSet()
        Dim Data As New DataSet()
        Dim geopoint As String = ""
        Dim geofence As String = ""
        Using con = New SqlConnection(conStr)
            Using da = New SqlDataAdapter("SELECT qryfield FROM MMM_MST_REPORT WHERE EID=53 and ReportID= " & RID, con)
                da.Fill(dsR)
                Dim Query = ""
                If dsR.Tables(0).Rows.Count > 0 Then
                    Query = dsR.Tables(0).Rows(0).Item("qryfield")
                    da.SelectCommand.CommandText = Query
                    da.Fill(Data)
                    If Data.Tables(0).Rows.Count > 0 Then
                        For i As Integer = 0 To Data.Tables(0).Rows.Count - 1
                            If Data.Tables(0).Rows(i).Item("GeoFence").ToString.Trim <> "" Then
                                Dim arr = Data.Tables(0).Rows(i).Item("GeoFence").ToString.Split(",")
                                Dim var1 = ""
                                For a = 0 To arr.Count - 1
                                    If a < arr.Count - 1 Then
                                        If var1 = "" Then
                                            var1 = arr(a + 1) & " " & arr(a)
                                        Else
                                            var1 = var1 & "," & arr(a + 1) & " " & arr(a)
                                        End If
                                        a = a + 1
                                    End If
                                Next
                                geofence = geofence & "#" & var1
                                'Sb.Append("#").Append(var1)
                            End If
                            If Data.Tables(0).Rows(i).Item("GeoPoint").ToString.Trim <> "" And Data.Tables(0).Rows(i).Item("GeoPoint").ToString.Contains("Error") = False Then
                                geopoint = geopoint & "['Product Name: " & Data.Tables(0).Rows(i).Item("Product Name").ToString & "<br>Advisor Code: " & Data.Tables(0).Rows(i).Item("Advisor Code").ToString & "'," & Data.Tables(0).Rows(i).Item("GeoPoint").ToString & ",'images/human.png'], "
                            End If
                        Next
                        'geopoint =  geopoint 
                        If geopoint <> "" Then
                            Sb.Append(geopoint).Append("|").Append(geofence)
                        End If
                    End If
                End If
            End Using
        End Using
        Str = Sb.ToString()
        'Page.ClientScript.RegisterClientScriptBlock(Page.GetType(), "jhdgh", "bindMap()")
        Return Str
    End Function
    '<WebMethod>
    'Public Shared Function ConvertDataTabletoString() As List(Of Dictionary(Of String, Object))
    '    Dim Sb As New StringBuilder()
    '    Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
    '    Dim dsR As New DataSet()
    '    Dim dt As New DataTable
    '    Dim RID As Integer = 0
    '    RID = 149
    '    Using con = New SqlConnection(conStr)
    '        Using da = New SqlDataAdapter("SELECT qryfield FROM MMM_MST_REPORT WHERE EID=53 and ReportID= " & RID, con)
    '            da.Fill(dsR)
    '            Dim Query = ""
    '            If dsR.Tables(0).Rows.Count > 0 Then
    '                Query = dsR.Tables(0).Rows(0).Item("qryfield")
    '                da.SelectCommand.CommandText = Query
    '                da.Fill(dt)
    '                If dt.Columns.Contains("GeoPoint") Then
    '                    dt = dt.Select("GeoPoint<>''").CopyToDataTable()
    '                    Dim rows As New List(Of Dictionary(Of String, Object))()
    '                    Dim row As Dictionary(Of String, Object)
    '                    For Each dr As DataRow In dt.Rows
    '                        row = New Dictionary(Of String, Object)()
    '                        For Each col As DataColumn In dt.Columns
    '                            row.Add(col.ColumnName, dr(col))
    '                        Next
    '                        rows.Add(row)
    '                    Next
    '                    Return rows
    '                End If
    '            Else
    '                Return Nothing
    '            End If
    '        End Using
    '    End Using
    '    Return Nothing
    'End Function
End Class
