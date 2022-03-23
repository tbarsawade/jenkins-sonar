Imports System.Net.HttpWebRequest
Imports System.Net
Imports System.IO
Imports System.Data
Imports System.Configuration
Imports System.Data.SqlClient
Imports System.Collections.Generic
Imports System.Xml
Imports System.Web.Services
Partial Class Report_Map
    Inherits System.Web.UI.Page
    Dim DocumentName As String = ""
    Shared dt As DataTable
    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        dt = DirectCast(Session("MyDatatable"), DataTable)
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
    Public Shared Function ConvertDataTabletoString() As List(Of Dictionary(Of String, Object))
        If dt.Columns.Contains("GeoPoint") Then
            dt = dt.Select("GeoPoint<>''").CopyToDataTable()
            Dim rows As New List(Of Dictionary(Of String, Object))()
            Dim row As Dictionary(Of String, Object)
            For Each dr As DataRow In dt.Rows
                row = New Dictionary(Of String, Object)()
                For Each col As DataColumn In dt.Columns
                    row.Add(col.ColumnName, dr(col))
                Next
                rows.Add(row)
            Next
            Return rows
        Else
            Return Nothing
        End If
    End Function

End Class
