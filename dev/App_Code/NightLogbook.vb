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

Public Class NightLogbook
    Implements INLogbook
    Public Function GetLogBook1(d1 As String, d2 As String, uid As Integer, urole As String, Eid As Integer) As DataSet
        Dim ds As New DataSet()
        Dim ConStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Using con As New SqlConnection(ConStr)
            Using da As New SqlDataAdapter("getIndusnightLogBook", con)
                da.SelectCommand.CommandType = CommandType.StoredProcedure
                da.SelectCommand.Parameters.AddWithValue("@uid", uid)
                da.SelectCommand.Parameters.AddWithValue("@rolename", urole)
                da.SelectCommand.Parameters.AddWithValue("@d1", d1)
                da.SelectCommand.Parameters.AddWithValue("@d2", d2)
                da.SelectCommand.Parameters.AddWithValue("@eid", Eid)
                da.SelectCommand.CommandTimeout = 120
                da.Fill(ds)
            End Using
        End Using
        Return ds
    End Function
    Public Function GetOverSpeed1(d1 As String, d2 As String, uid As Integer, urole As String, Eid As Integer) As DataSet
        Dim ds As New DataSet()
        Dim ConStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Using con As New SqlConnection(ConStr)
            Using da As New SqlDataAdapter("getIndusOverSpeed", con)
                da.SelectCommand.CommandType = CommandType.StoredProcedure
                da.SelectCommand.Parameters.AddWithValue("@uid", uid)
                da.SelectCommand.Parameters.AddWithValue("@rolename", urole)
                da.SelectCommand.Parameters.AddWithValue("@d1", d1)
                da.SelectCommand.Parameters.AddWithValue("@d2", d2)
                da.SelectCommand.Parameters.AddWithValue("@eid", Eid)
                da.SelectCommand.CommandTimeout = 120
                da.Fill(ds)
            End Using
        End Using
        Return ds
    End Function

    Public Function GetOverSpeedFromRawData(d1 As String, d2 As String, UID As Integer, UROLE As String, Eid As Integer) As DataSet
        Dim ds As New DataSet()
        Dim dtData As New DataTable

        dtData.Columns.Add("")
        dtData.Columns.Add("")

        Dim ConStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString

        Dim dtVehicle As New DataTable
        Dim qry As String = ""

        Dim str As String = "Select VIDType[Doc], VIVehicleField[VhNo], VIImeiField[VhIMEI] from mmm_mst_Entity where Eid=" & Eid
        Dim dtEntity As New DataTable
        Using con As New SqlConnection(ConStr)
            Using da As New SqlDataAdapter(str, con)
                da.Fill(dtEntity)
            End Using
        End Using

        If IsDBNull(dtEntity.Rows(0).Item(0)) Then
            Return ds
        End If
        Dim Rid As String = Nothing

        If UROLE = "SU" Or UROLE = "CORPORATEUSER" Then
            qry = "select distinct " & dtEntity.Rows(0).Item(2).ToString & "[IMIENO], " & dtEntity.Rows(0).Item(1).ToString & "[VhNo] from mmm_mst_master where documenttype='" & dtEntity.Rows(0).Item(0).ToString & "' and eid=" & Eid & " and " & dtEntity.Rows(0).Item(2).ToString & "<>'' and " & dtEntity.Rows(0).Item(2).ToString & "<>'0'"
        Else
            Dim dtCluster As New DataTable
            Using con As New SqlConnection(ConStr)
                Using da As New SqlDataAdapter("select uid,fld1[ClusterID],rolename from mmm_ref_role_user  where eid=" & Eid & " and uid= " & UID & " and rolename='" & UROLE & "'", con)
                    da.Fill(dtCluster)
                End Using
            End Using
            If dtCluster.Rows.Count = 0 Then
                Rid = "0"
            Else
                Rid = dtCluster.Rows(0).Item("ClusterID").ToString()
            End If
            qry = "select distinct " & dtEntity.Rows(0).Item(2).ToString & "[IMIENO], " & dtEntity.Rows(0).Item(1).ToString & "[VhNo] from mmm_mst_master inner join  dbo.split('" & Rid & "', ',') s on s.items in (select items from dbo.split(fld16, ','))  where documenttype='" & dtEntity.Rows(0).Item(0).ToString & "' and eid=" & Eid & " and " & dtEntity.Rows(0).Item(2).ToString & "<>'' and " & dtEntity.Rows(0).Item(2).ToString & "<>'0'"
        End If

        Using con As New SqlConnection(ConStr)
            Using da As New SqlDataAdapter(qry, con)
                da.Fill(dtVehicle)
            End Using
        End Using

        For i As Integer = 0 To dtVehicle.Rows.Count - 1
            Using con As New SqlConnection(ConStr)
                Using da As New SqlDataAdapter("getIndusOverSpeedFromRaw", con)
                    da.SelectCommand.CommandType = CommandType.StoredProcedure
                    da.SelectCommand.Parameters.AddWithValue("@IMEINo", dtVehicle.Rows(i).Item(0).ToString)
                    da.SelectCommand.Parameters.AddWithValue("@d1", d1)
                    da.SelectCommand.Parameters.AddWithValue("@d2", d2)
                    da.SelectCommand.Parameters.AddWithValue("@eid", Eid)
                    da.SelectCommand.Parameters.AddWithValue("@vehNo", dtVehicle.Rows(i).Item(1).ToString)
                    da.SelectCommand.Parameters.AddWithValue("@VehName", dtVehicle.Rows(i).Item(0).ToString)
                    da.SelectCommand.CommandTimeout = 120
                    da.Fill(dtData)
                End Using
            End Using
        Next
        ds.Tables.Add(dtData)
        Return ds
    End Function

    Public Function GetOverSpeed(sdate As String, tdate As String, UID As Integer, UROLE As String, Eid As Integer) As Demo1 Implements INLogbook.GetOverSpeed
        Dim ds As New DataSet()
        Dim ret = ""
        Dim d1 = "2014-09-23"
        Dim d2 = "2014-09-24"
        Dim response As New Demo1()
        If Eid = 54 Then
            ds = GetOverSpeed1(sdate, tdate, UID, UROLE, Eid)
        Else
            ds = GetOverSpeedFromRawData(sdate, tdate, UID, UROLE, Eid)
        End If

      
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
    Public Function GetlogBook(sdate As String, tdate As String, UID As Integer, UROLE As String, Eid As Integer) As Demo1 Implements INLogbook.GetlogBook
        Dim ds As New DataSet()
        Dim ret = ""
        Dim d1 = "2014-09-23"
        Dim d2 = "2014-09-24"
        Dim response As New Demo1()
        'Dim uid1 As Integer = HttpContext.Current.Session("UID")
        'Dim urole1 = HttpContext.Current.Session("USERROLE")

        Dim uid1 As Integer = UID
        Dim urole1 = UROLE

        ds = GetLogBook1(sdate, tdate, UID, UROLE, Eid)
        ' HttpContext.Current.Session("Sdate") = sdate.ToString
        'HttpContext.Current.Session("Edate") = tdate.ToString
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
            'Return response
        Catch Ex As Exception
            Throw
        End Try
        Return response
    End Function

    
    Public Function GetUnderSpeed(sdate As String, tdate As String, UID As Integer, UROLE As String, Eid As Integer) As Demo1 Implements INLogbook.GetUnderSpeed
        Dim response As New Demo1()
        Dim ds As New DataSet()
        Dim ret = ""
        Dim d1 = ""
        Dim d2 = ""
        Try

            ds = GetUnderSpeedFromRaw(sdate, tdate, UID, UROLE, Eid)

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
        Catch ex As Exception

        End Try
        Return response
    End Function

    Public Function GetUnderSpeedFromRaw(d1 As String, d2 As String, UID As Integer, UROLE As String, Eid As Integer) As DataSet
        Dim ds As New DataSet()
        Dim dtData As New DataTable

        Dim ConStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString

        Dim dtVehicle As New DataTable
        Dim qry As String = ""

        Dim str As String = "Select VIDType[Doc], VIVehicleField[VhNo], VIImeiField[VhIMEI] from mmm_mst_Entity where Eid=" & Eid
        Dim dtEntity As New DataTable
        Using con As New SqlConnection(ConStr)
            Using da As New SqlDataAdapter(str, con)
                da.Fill(dtEntity)
            End Using
        End Using

        If IsDBNull(dtEntity.Rows(0).Item(0)) Then
            Return ds
        End If
        Dim Rid As String = Nothing

        If UROLE = "SU" Or UROLE = "CORPORATEUSER" Then
            qry = "select distinct " & dtEntity.Rows(0).Item(2).ToString & "[IMIENO], " & dtEntity.Rows(0).Item(1).ToString & "[VhNo] from mmm_mst_master where documenttype='" & dtEntity.Rows(0).Item(0).ToString & "' and eid=" & Eid & " and " & dtEntity.Rows(0).Item(2).ToString & "<>'' and " & dtEntity.Rows(0).Item(2).ToString & "<>'0'"
        Else
            Dim dtCluster As New DataTable
            Using con As New SqlConnection(ConStr)
                Using da As New SqlDataAdapter("select uid,fld1[ClusterID],rolename from mmm_ref_role_user  where eid=" & Eid & " and uid= " & UID & " and rolename='" & UROLE & "'", con)
                    da.Fill(dtCluster)
                End Using
            End Using
            If dtCluster.Rows.Count = 0 Then
                Rid = "0"
            Else
                Rid = dtCluster.Rows(0).Item("ClusterID").ToString()
            End If
            qry = "select distinct " & dtEntity.Rows(0).Item(2).ToString & "[IMIENO], " & dtEntity.Rows(0).Item(1).ToString & "[VhNo] from mmm_mst_master inner join  dbo.split('" & Rid & "', ',') s on s.items in (select items from dbo.split(fld16, ','))  where documenttype='" & dtEntity.Rows(0).Item(0).ToString & "' and eid=" & Eid & " and " & dtEntity.Rows(0).Item(2).ToString & "<>'' and " & dtEntity.Rows(0).Item(2).ToString & "<>'0'"
        End If

        Using con As New SqlConnection(ConStr)
            Using da As New SqlDataAdapter(qry, con)
                da.Fill(dtVehicle)
            End Using
        End Using

        For i As Integer = 0 To dtVehicle.Rows.Count - 1
            Using con As New SqlConnection(ConStr)
                Using da As New SqlDataAdapter("getUnderSpeedFromRaw", con)
                    da.SelectCommand.CommandType = CommandType.StoredProcedure
                    da.SelectCommand.Parameters.AddWithValue("@IMEINo", dtVehicle.Rows(i).Item(0).ToString)
                    da.SelectCommand.Parameters.AddWithValue("@d1", d1)
                    da.SelectCommand.Parameters.AddWithValue("@d2", d2)
                    da.SelectCommand.Parameters.AddWithValue("@vehNo", dtVehicle.Rows(i).Item(1).ToString)
                    da.SelectCommand.Parameters.AddWithValue("@VehName", dtVehicle.Rows(i).Item(0).ToString)
                    da.SelectCommand.CommandTimeout = 120
                    da.Fill(dtData)
                End Using
            End Using
        Next
        ds.Tables.Add(dtData)
        Return ds
    End Function

End Class

<DataContract> _
Public Class Demo1
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
            m_total = value
        End Set
    End Property
    Private m_total As Integer
    <DataMember> _
    Public Fields As [String]
    '<DataMember> _
    ''Public columns As New List(Of BPMColumns)()
End Class
