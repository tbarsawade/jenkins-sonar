Imports System.Data
Imports System.Data.SqlClient
Imports System.Net
Imports System.IO
Imports System.Xml.Serialization
Imports System.Runtime.Serialization
Imports System.ServiceModel
Imports System.ServiceModel.Web
Imports System.Xml
Imports Newtonsoft.Json.Converters
Imports Newtonsoft.Json
Imports System.Web.Script.Serialization

Public Class Widget
    Dim ConStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
    Dim serializerSettings As New JsonSerializerSettings()
    Dim json_serializer As New JavaScriptSerializer()

    Public Function GetUsefullLink(EID As Integer) As UseFullLink
        Dim ret As New UseFullLink()
        Dim ds As New DataSet()
        Dim jsonData As String = ""
        serializerSettings.Converters.Add(New DataTableConverter())
        Using con = New SqlConnection(ConStr)
            Using da = New SqlDataAdapter("GetUsefullLink", con)
                da.SelectCommand.CommandType = CommandType.StoredProcedure
                da.SelectCommand.Parameters.AddWithValue("@eid", EID)
                da.Fill(ds)
            End Using
        End Using
        If ds.Tables(0).Rows.Count > 0 Then
            jsonData = JsonConvert.SerializeObject(ds.Tables(0), Newtonsoft.Json.Formatting.None, serializerSettings)
            ret.DocList = jsonData
        End If
        If ds.Tables(1).Rows.Count > 0 Then
            jsonData = JsonConvert.SerializeObject(ds.Tables(1), Newtonsoft.Json.Formatting.None, serializerSettings)
            ret.SsoList = jsonData
        End If
        Return ret
    End Function

    Public Function GetWidgets(EID As Integer, DBName As String, Roles As String) As String
        Dim ret As String = ""
        Dim ds As New DataSet()
        Dim jsonData As String = ""
        serializerSettings.Converters.Add(New DataTableConverter())
        ds = GetWidgets(EID, DBName, Roles, 0)
        If ds.Tables(0).Rows.Count > 0 Then
            jsonData = JsonConvert.SerializeObject(ds.Tables(0), Newtonsoft.Json.Formatting.None, serializerSettings)
            ret = jsonData
        End If
        Return ret
    End Function

    Public Function GetWidgets(EID As Integer, DBName As String, Roles As String, tid As Integer) As DataSet
        Dim ds As New DataSet()
        Using con = New SqlConnection(ConStr)
            Using da = New SqlDataAdapter("GetWidgets", con)
                da.SelectCommand.CommandType = CommandType.StoredProcedure
                da.SelectCommand.Parameters.AddWithValue("@eid", EID)
                da.SelectCommand.Parameters.AddWithValue("@DBNAME", DBName)
                da.SelectCommand.Parameters.AddWithValue("@Roles", Roles)
                da.SelectCommand.Parameters.AddWithValue("@TID", tid)
                da.Fill(ds)
            End Using
        End Using
        Return ds
    End Function

    Public Function GetCustomWidget(tid As Integer, uid As Integer, Roles As String) As kGrid
        Dim ret As New kGrid()
        Dim EID As Integer
        Dim obj As New Widget()
        Dim dsQ As New DataSet()
        Dim jsonData As String = ""
        Try
            EID = Convert.ToInt32(HttpContext.Current.Session("EID"))
            Dim ds As New DataSet()
            ds = obj.GetWidgets(EID, "", Roles, tid)
            If ds.Tables(0).Rows.Count > 0 Then
                Dim Query = ds.Tables(0).Rows(0).Item("RootQuery")
                'Replacing Query with value
                Query = Query.ToString.Replace("{UID}", uid).Replace("{roles}", Roles)
                Using con = New SqlConnection(ConStr)
                    Using da As New SqlDataAdapter(Query, con)
                        da.Fill(dsQ)
                    End Using
                End Using
            End If
            If dsQ.Tables(0).Rows.Count > 0 Then
                For k As Integer = 0 To dsQ.Tables(0).Columns.Count - 1
                    dsQ.Tables(0).Columns(k).ColumnName = dsQ.Tables(0).Columns(k).ColumnName.Replace(" ", "_").Replace("(", "").Replace(")", "")
                Next
                serializerSettings.Converters.Add(New DataTableConverter())
                jsonData = JsonConvert.SerializeObject(dsQ.Tables(0), Newtonsoft.Json.Formatting.None, serializerSettings)
            End If
            ret.Data = jsonData
            ret.Column = CreateColCollection(dsQ.Tables(0))
        Catch ex As Exception
        End Try
        Return ret
    End Function

    Public Function GetPiChartWidget(tid As Integer, uid As Integer, Urole As String) As pichartCol
        Dim ret As New pichartCol()
        Dim EID As Integer
        Dim obj As New Widget()
        Dim dsQ As New DataSet()
        Dim jsonData As String = ""
        Dim objp As pichart
        Dim o1 As New List(Of pichart)()
        Try
            EID = Convert.ToInt32(HttpContext.Current.Session("EID"))
            Dim Roles = Convert.ToString(HttpContext.Current.Session("Roles"))
            Dim ds As New DataSet()
            ds = obj.GetWidgets(EID, "", Roles, tid)
            If ds.Tables(0).Rows.Count > 0 Then
                Dim Query = ds.Tables(0).Rows(0).Item("RootQuery")
                'Replacing Query with value
                Query = Query.ToString.Replace("{UID}", uid).Replace("{roles}", Roles)
                Using con = New SqlConnection(ConStr)
                    Using da As New SqlDataAdapter(Query, con)
                        da.Fill(dsQ)
                    End Using
                End Using
            End If
            If dsQ.Tables(0).Rows.Count > 0 Then
                For i As Integer = 0 To dsQ.Tables(0).Rows.Count - 1
                    objp = New pichart()
                    objp.category = dsQ.Tables(0).Rows(i).Item(0)
                    objp.value = dsQ.Tables(0).Rows(i).Item(1)
                    o1.Add(objp)
                Next
                ret.data = o1
            End If
        Catch ex As Exception
        End Try
        Return ret
    End Function

    Public Shared Function CreateColCollection(dt As DataTable) As List(Of kColumn)
        Dim lstColName As New List(Of kColumn)()
        Dim i As Integer = 0
        Dim objG As kColumn
        For Each column As DataColumn In dt.Columns
            objG = New kColumn()
            objG.field = column.ColumnName.Replace(" ", "_").Replace("(", "").Replace(")", "")
            objG.title = column.ColumnName
            lstColName.Add(objG)
        Next
        Return lstColName
    End Function
End Class

Public Class UseFullLink
    Public DocList As String = ""
    Public SsoList As String = ""
End Class

Public Class kGrid
    Public Data As String = ""
    Public Column As New List(Of kColumn)
End Class

Public Class kColumn
    Public field As String = ""
    Public title As String = ""
    Public width As String = "100px"
    Public FieldID As String = ""


End Class

Public Class kColumn1
    'Public Sub New(ByVal col1 As String, ByVal col2 As String, ByVal col3 As String, ByVal col4 As String, ByVal col5 As String)

    'End Sub
    Public FieldID As String = ""
    Public Title As String
    Public field As String = ""
    Public width As String = ""
    Public type As String = ""
    Public format As String = ""
    Public column As New List(Of String)
End Class

Public Class pichart
    Public category As String
    Public value As String
    'Public color As String
End Class
Public Class pichartCol
    Public data As New List(Of pichart)()
End Class