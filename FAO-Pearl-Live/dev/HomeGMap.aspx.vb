Imports System.Data.SqlClient
Imports System.Data
Imports System.Web.Services
Partial Class HomeGMap
    Inherits System.Web.UI.Page
    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As SqlConnection = New SqlConnection(conStr)
            Dim Udtype As String
            Dim Ufld As String
            Dim UVfld As String
            Dim Vdtype As String
            Dim Vfld As String
            Dim vemei As String
            Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
            Try
                oda.SelectCommand.CommandText = "select * from mmm_mst_entity where eid=" & Session("EID") & " "
                Dim ds As New DataSet()
                oda.Fill(ds, "data")
                If ds.Tables("data").Rows.Count > 0 Then
                    Udtype = ds.Tables("data").Rows(0).Item("uvdtype").ToString
                    Ufld = ds.Tables("data").Rows(0).Item("uvuserfield").ToString
                    UVfld = ds.Tables("data").Rows(0).Item("uvvehiclefield").ToString
                    Vdtype = ds.Tables("data").Rows(0).Item("VIDType").ToString
                    Vfld = ds.Tables("data").Rows(0).Item("vivehiclefield").ToString
                    vemei = ds.Tables("data").Rows(0).Item("viimeifield").ToString
                End If
                Dim dtt As New DataTable()
                Circle.Items.Clear()
                oda.SelectCommand.CommandText = "select distinct top 20 tid,fld1 from mmm_mst_master where DOCUMENTTYPE='Site Type' and eid=" & Session("EID") & " and fld1<>'' order by fld1"
                oda.Fill(dtt)
                For i As Integer = 0 To dtt.Rows.Count - 1
                    Circle.Items.Add(Convert.ToString(dtt.Rows(i).Item("fld1").ToString))
                    Circle.Items(i).Value = dtt.Rows(i).Item("tid").ToString
                    Circle.Items(i).Attributes.Add("onclick", "javascript:ViewMap();")
                Next
            Catch ex As Exception
            Finally
                If Not con Is Nothing Then
                    con.Close()
                    oda.Dispose()
                    con.Dispose()
                End If
                If Not oda Is Nothing Then
                    oda.Dispose()
                End If
            End Try
        End If
    End Sub
    'Protected Sub FilterUser(sender As Object, e As System.EventArgs) Handles Circle.SelectedIndexChanged
    '    Dim id As String = ""
    '    For i As Integer = 0 To Circle.Items.Count - 1
    '        If Circle.Items(i).Selected = True Then
    '            id = id & Circle.Items(i).Value & ","
    '        End If
    '    Next
    'End Sub
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
    Public Shared Function ConvertDataTabletoString(ByVal eid As Integer, ByVal doctype As String) As List(Of Dictionary(Of String, Object))
        Dim geopoint As String = String.Empty
        Dim dt As New DataTable()
        Dim str As String = ""
        str = CType(eid, String)
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        If doctype.ToString = "" Then
            doctype = "0"
        End If
        Dim oda As SqlDataAdapter = New SqlDataAdapter("select distinct top 500 m.fld10[SiteID],m.fld11[SiteName],m.fld13[Address],m1.fld1[Site],m.fld21[Geopoint] from mmm_mst_master m left outer join mmm_mst_master m1 on convert(nvarchar,m1.tid)=m.fld12  where m.documenttype='Site' and m1.documenttype='Site Type' and m1.eid=" + str + " and m.eid=" + str + " and m.fld12 in (" & doctype & ") and m.fld21<>'' and m.fld21 is not null and m.fld21<>'0'", con)
        oda.Fill(dt)
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
    End Function
End Class
