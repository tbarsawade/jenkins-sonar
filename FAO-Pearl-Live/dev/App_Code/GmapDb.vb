Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Data.SqlClient

Public Class GmapDb
    Private Shared connectionString As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
    ''Vehical Types
    Public Shared Function GetVehicalTypes(EId As Integer) As List(Of Gmap)
        Dim Types As New List(Of Gmap)()
        Dim con As New SqlConnection(connectionString)
        Dim cmd As New SqlCommand("select tid[TID], fld1[VType] from mmm_mst_master where eid=" & EId & " and documenttype='vehicle type' ", con)
        Dim dt As New DataTable()
        Dim da As New SqlDataAdapter(cmd)
        da.Fill(dt)
        For Each dr As DataRow In dt.Rows
            Dim vtype As New Gmap()
            vtype.TID = CInt(dr.Item("TID"))
            vtype.VType = DirectCast(dr.Item("VType"), String)
            Types.Add(vtype)
        Next
        Return Types
    End Function

    ''Site Types
    Public Shared Function GetSiteTypes(EId As Integer) As List(Of Gmap)
        Dim Types As New List(Of Gmap)()
        Dim con As New SqlConnection(connectionString)
        Dim cmd As New SqlCommand("select distinct top 20 tid[TID],fld1[SiteType] from mmm_mst_master where DOCUMENTTYPE='Site Type' and eid=" & EId & " and fld1<>'' order by fld1", con)
        Dim dt As New DataTable()
        Dim da As New SqlDataAdapter(cmd)
        da.Fill(dt)
        For Each dr As DataRow In dt.Rows
            Dim vtype As New Gmap()
            vtype.TID = CInt(dr.Item("TID"))
            vtype.VType = DirectCast(dr.Item("SiteType"), String)
            Types.Add(vtype)
        Next
        Return Types
    End Function

    Public Shared Function GetVechicalMarkers(strList As String) As List(Of Gmap)
        Dim Types As New List(Of Gmap)()
        Dim con As New SqlConnection(connectionString)

        Dim str As String = "select max(g.tid)[TID],IMIENO[SiteID],max(ctime)[SiteName],max(m2.fld1)[Address],(select msensor from mmm_mst_gpsdata where tid=max(g.tid))[Site] ,datediff(minute,max(ctime),getdate())[Info],(select Lattitude from mmm_mst_gpsdata where tid=max(g.tid))[Lat],(select longitude from mmm_mst_gpsdata where tid=max(g.tid))[Long], max(m2.fld14)[Group],max(m2.fld10)[Site_Name],(select speed from mmm_mst_gpsdata where tid=max(g.tid))[Site_Address],''[OandM_Head],''[Maintenance_Head],''[Opex_Manager],''[Security_Manager],''[Zonal_Head],''[Cluster_Manager],''[Supervisor],''[Technician]"
        str = str & " from mmm_mst_gpsdata g with (nolock) left outer join mmm_mst_master m1 with (nolock) on m1.fld1=g.imieno left outer join mmm_mst_master m2 with (nolock) on m2.fld12=convert(nvarchar,m1.fld1)  where m2.documenttype='vehicle' and m2.eid=54 and m1.eid=54 and m1.documenttype='GPS Device' and m2.fld14 in (" & strList & ") group by imieno"

        Dim cmd As New SqlCommand(str, con)
        Dim dt As New DataTable()
        Dim da As New SqlDataAdapter(cmd)
        da.Fill(dt)
        For Each dr As DataRow In dt.Rows
            Dim vtype As New Gmap()
            vtype.TID = DirectCast(dr.Item("TID"), String)
            vtype.SiteID = DirectCast(dr.Item("SiteID"), String)
            vtype.SiteName = DirectCast(dr.Item("SiteName"), String)
            vtype.Address = DirectCast(dr.Item("Address"), String)
            vtype.Site = DirectCast(dr.Item("Site"), String)
            vtype.Info = DirectCast(dr.Item("Info"), String)
            vtype.Lat = DirectCast(dr.Item("Lat"), String)
            vtype.Longt = DirectCast(dr.Item("Long"), String)
            vtype.Group = DirectCast(dr.Item("Group"), String)
            vtype.Site_Name = DirectCast(dr.Item("Site_Name"), String)
            vtype.Site_Address = DirectCast(dr.Item("Site_Address"), String)
            vtype.OandM_Head = DirectCast(dr.Item("OandM_Head"), String)
            vtype.Maintenance_Head = DirectCast(dr.Item("Maintenance_Head"), String)
            vtype.Opex_Manager = DirectCast(dr.Item("Opex_Manager"), String)
            vtype.Security_Manager = DirectCast(dr.Item("Security_Manager"), String)
            vtype.Zonal_Head = DirectCast(dr.Item("Zonal_Head"), String)
            vtype.Cluster_Manager = DirectCast(dr.Item("Cluster_Manager"), String)
            vtype.Supervisor = DirectCast(dr.Item("Supervisor"), String)
            vtype.Technician = DirectCast(dr.Item("Technician"), String)
            Types.Add(vtype)
        Next
        Return Types
    End Function


End Class
