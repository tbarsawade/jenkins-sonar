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

Public Class Logbook
    Implements ILogbook

    
    Public Function GetLogBook1(d1 As String, d2 As String, uid As Integer, urole As String, IsUserRpt As Boolean) As DataSet

        Dim ds As New DataSet()
        Dim ConStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Using con As New SqlConnection(ConStr)
            Dim ProcedureName As String = "getindusLogBook1000"
            If (IsUserRpt = True) Then
                ProcedureName = "getindusLogBook1000_User"
            End If
            Using da As New SqlDataAdapter(ProcedureName, con)
                da.SelectCommand.CommandType = CommandType.StoredProcedure
                da.SelectCommand.Parameters.AddWithValue("@uid", uid)
                da.SelectCommand.Parameters.AddWithValue("@rolename", urole)
                da.SelectCommand.Parameters.AddWithValue("@d1", d1)
                da.SelectCommand.Parameters.AddWithValue("@d2", d2)
                da.SelectCommand.CommandTimeout = 120
                da.Fill(ds)
            End Using
        End Using
        Return ds
    End Function

    Public Function GetlogBook(sdate As String, tdate As String, UID As Integer, UROLE As String, Optional ByVal IsUserRpt As Boolean = False) As Demo Implements ILogbook.GetlogBook
        Dim ds As New DataSet()
        Dim ret = ""
        Dim d1 = "2014-09-23"
        Dim d2 = "2014-09-24"
        Dim response As New Demo()
        Dim uid1 As Integer = 5897
        Dim urole1 = "SU"

        ds = GetLogBook1(sdate, tdate, UID, UROLE, IsUserRpt)

        Try
            For j As Integer = 0 To ds.Tables(0).Columns.Count - 1
                ds.Tables(0).Columns(j).ColumnName = ds.Tables(0).Columns(j).ColumnName.Replace(" ", "_").Replace(".", "")
            Next
            Dim serializerSettings As New JsonSerializerSettings()
            Dim json_serializer As New JavaScriptSerializer()
            serializerSettings.Converters.Add(New DataTableConverter())
            Dim jsonData As String = JsonConvert.SerializeObject(ds.Tables(0), Newtonsoft.Json.Formatting.None, serializerSettings)
            ret = jsonData
            response.data = jsonData
            response.total = ds.Tables(0).Rows.Count
        Catch Ex As Exception
            Throw
        End Try
            Return response
    End Function


   

    
End Class

<DataContract> _
Public Class Demo
    <DataMember> _
    Public Property data() As String
        Get
            Return m_data
        End Get
        Set(value As String)
            m_data = value
        End Set
    End Property
    Private m_data As [String]
    <DataMember> _
    Public Property total() As Integer
        Get
            Return m_total
        End Get
        Set(value As Integer)
            m_total = Value
        End Set
    End Property
    Private m_total As Integer
    <DataMember> _
    Public Fields As [String]
    '<DataMember> _
    ''Public columns As New List(Of BPMColumns)()
End Class
