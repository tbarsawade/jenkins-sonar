Imports System.Net.HttpWebRequest
Imports System.Net
Imports System.IO
Imports System.Data
Imports System.Configuration
Imports System.Data.SqlClient
Imports System.Collections.Generic
Imports System.Xml
Imports System.Web.Services


Partial Class Test

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
            Session("USERROLE") = "SU"
            Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
            Try
                oda.SelectCommand.CommandText = "select * from mmm_mst_entity where eid=54 "
                Dim ds As New DataSet()
                oda.Fill(ds, "Entity")
                hdnRefTime.Value = Convert.ToInt32(ds.Tables("Entity").Rows(0).Item("ReloadSeconds")) * 1000
                'hdnRefTime.Value = 70000
                If Session("userrole").ToString().ToUpper() = "SU" Then
                    hdnCluster.Value = "SU"
                Else
                    'oda.SelectCommand.CommandText = "select uid,fld1[ClusterID],rolename from mmm_ref_role_user  where eid=54 and uid= " & Session("uid") & " and rolename='" & Session("userrole") & "'"
                    'oda.Fill(ds, "Cluster")
                    'hdnCluster.Value = ds.Tables("Cluster").Rows(0).Item("ClusterID").ToString()
                    'Session("RID") = ds.Tables("Cluster").Rows(0).Item("ClusterID").ToString()
                End If


                Dim dtt As New DataTable()
                Circle.Items.Clear()
                oda.SelectCommand.CommandText = "select distinct top 20 tid,fld1 from mmm_mst_master where DOCUMENTTYPE='Site Type' and eid=54 and fld1<>'' order by fld1"
                oda.Fill(dtt)
                For i As Integer = 0 To dtt.Rows.Count - 1
                    If dtt.Rows(i).Item("fld1").ToString.ToUpper = "HUB" Then
                        Circle.Items.Add(Convert.ToString(dtt.Rows(i).Item("fld1").ToString))
                        Circle.Items(i).Value = dtt.Rows(i).Item("tid").ToString
                        Circle.Items(i).Attributes.Add("style", "color:Orange;")
                    ElseIf dtt.Rows(i).Item("fld1").ToString.ToUpper = "BSC" Then
                        Circle.Items.Add(Convert.ToString(dtt.Rows(i).Item("fld1").ToString))
                        Circle.Items(i).Value = dtt.Rows(i).Item("tid").ToString
                        Circle.Items(i).Attributes.Add("style", "color:lightblue;")
                    ElseIf dtt.Rows(i).Item("fld1").ToString.ToUpper = "STRATEGIC" Then
                        Circle.Items.Add(Convert.ToString(dtt.Rows(i).Item("fld1").ToString))
                        Circle.Items(i).Value = dtt.Rows(i).Item("tid").ToString
                        Circle.Items(i).Attributes.Add("style", "color:DarkBlue;")
                    ElseIf dtt.Rows(i).Item("fld1").ToString.ToUpper = "NON STRATEGIC" Then
                        Circle.Items.Add(Convert.ToString(dtt.Rows(i).Item("fld1").ToString))
                        Circle.Items(i).Value = dtt.Rows(i).Item("tid").ToString
                        Circle.Items(i).Attributes.Add("style", "color:Blueviolet;")
                    End If

                    Circle.Items(i).Selected = True
                    Circle.Items(i).Attributes.Add("onclick", "ShowHideMap(this)")
                Next


                oda.SelectCommand.CommandText = "select * from mmm_mst_master with(nolock) where eid=54 and documenttype='vehicle type' "
                dtt.Clear()
                oda.Fill(dtt)
                For i As Integer = 0 To dtt.Rows.Count - 1
                    chkvtype.Items.Add(Convert.ToString(dtt.Rows(i).Item("fld1").ToString))
                    chkvtype.Items(i).Value = dtt.Rows(i).Item("tid").ToString
                    chkvtype.Items(i).Attributes.Add("onclick", "ShowHideMap(this)")
                    chkvtype.Items(i).Selected = True
                Next
                BindAll()
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

    Private Sub BindAll()
        Dim id As String = ""
        For i As Integer = 0 To Circle.Items.Count - 1

            id = id & Circle.Items(i).Value & ","

        Next
        For i As Integer = 0 To chkvtype.Items.Count - 1

            id = id & chkvtype.Items(i).Value & ","

        Next
        If id.Length > 0 Then
            id = Left(id, id.Length - 1)
            Maprendorforsite(id)
        End If
    End Sub

    Protected Sub FilterSite(sender As Object, e As System.EventArgs)
        Dim id As String = ""
        For i As Integer = 0 To Circle.Items.Count - 1
            If Circle.Items(i).Selected = True Then
                id = id & Circle.Items(i).Value & ","
            End If
        Next
        For i As Integer = 0 To chkvtype.Items.Count - 1
            If chkvtype.Items(i).Selected = True Then
                id = id & chkvtype.Items(i).Value & ","
            End If
        Next
        If id.Length > 0 Then
            id = Left(id, id.Length - 1)
            Maprendorforsite(id)
        End If
    End Sub
    Protected Function Maprendorforsite(ByVal sid As String) As String
        Try
            LstVehicle.Items.Clear()
            'Change Check Box Text Color
            Circle.Items(0).Attributes.Add("style", "color:lightblue;")
            Circle.Items(1).Attributes.Add("style", "color:Orange;")
            Circle.Items(2).Attributes.Add("style", "color:Blueviolet;")
            Circle.Items(3).Attributes.Add("style", "color:DarkBlue;")
            Dim j As Integer = 0
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As New SqlConnection(conStr)
            Dim apikey As String = String.Empty
            Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
            Dim ds As New DataSet()
            ds.Tables.Add("table1")
            'Dim str As String = "select convert(nvarchar,g.tid)[TID],IMIENO[SiteID],convert(nvarchar,ctime,101)[SiteName],m2.fld1[Address],convert(nvarchar,msensor)[Site] ,convert(nvarchar,datediff(minute,ctime,getdate()))[info],convert(nvarchar,Lattitude)[Lat],convert(nvarchar,longitude)[Long], max(m2.fld14)[Group],max(m2.fld10)[Site_Name],DMS.IdealTime(g.imieno)[OandM_Head], g.Speed[Site_Address],''[Maintenance_Head],''[Opex_Manager],''[Security_Manager],''[Zonal_Head],''[Cluster_Manager],''[Supervisor],''[Technician],''[Anchor_OPCO],''[No_of_OPCOs],''[Cluster]"
            'str = str & "from mmm_mst_gpsdata g with (nolock) join mmm_mst_master m2 with (nolock) on m2.fld12=g.imieno   where m2.documenttype='vehicle' and m2.fld14 in (" & sid & ")  and m2.eid=54 group by g.tid,imieno,ctime,m2.fld1,msensor,lattitude,longitude,speed having ctime=(select max(ctime) from mmm_mst_gpsdata with (nolock) where imieno=g.imieno)"

            Dim str As String = "select convert(nvarchar,g.tid)[TID],IMIENO[SiteID],convert(nvarchar,ctime,101)[SiteName],m2.fld1[Address],convert(nvarchar,msensor)[Site] ,convert(nvarchar,datediff(minute,ctime,getdate()))[info],convert(nvarchar,Lattitude)[Lat],convert(nvarchar,longitude)[Long], max(m2.fld14)[Group],max(m2.fld10)[Site_Name],DMS.IdealTime(g.imieno)[OandM_Head], g.Speed[Site_Address],''[Maintenance_Head],''[Opex_Manager],''[Security_Manager],''[Zonal_Head],''[Cluster_Manager],''[Supervisor],''[Technician],''[Anchor_OPCO],''[No_of_OPCOs],''[Cluster]"
            If Session("USERROLE").ToString.ToUpper = "SU" Or Session("USERROLE").ToString.ToUpper = "CORPORATEUSER" Then
                str = str & "from mmm_mst_gpsdata g with (nolock) join mmm_mst_master m2 with (nolock) on m2.fld12=g.imieno   where m2.documenttype='vehicle' and m2.fld14 in (" & sid & ")  and m2.eid=54 group by g.tid,imieno,ctime,m2.fld1,msensor,lattitude,longitude,speed having ctime=(select max(ctime) from mmm_mst_gpsdata with (nolock) where imieno=g.imieno)"
            Else
                str = str & "from mmm_mst_gpsdata g with (nolock) join mmm_mst_master m2 with (nolock) on m2.fld12=g.imieno inner join  dbo.split('" & hdnCluster.Value.ToString & "', ',') s on s.items in (select items from dbo.split(m2.fld16, ','))   where m2.documenttype='vehicle' and m2.fld14 in (" & sid & ")  and m2.eid=54 group by g.tid,imieno,ctime,m2.fld1,msensor,lattitude,longitude,speed having ctime=(select max(ctime) from mmm_mst_gpsdata where imieno=g.imieno)"
            End If


            oda.SelectCommand.CommandText = str
            oda.SelectCommand.CommandTimeout = 180
            oda.Fill(ds, "table1")
            For i As Integer = 0 To ds.Tables("table1").Rows.Count - 1
                If Convert.ToInt32(ds.Tables("table1").Rows(i).Item("OandM_Head").ToString) > 10 And Convert.ToInt32(ds.Tables("table1").Rows(i).Item("OandM_Head").ToString) <= 600 Then
                    LstVehicle.Items.Add(ds.Tables("table1").Rows(i).Item("Address").ToString())
                    LstVehicle.Items(j).Text = ds.Tables("table1").Rows(i).Item("Site_Name").ToString()
                    LstVehicle.Items(j).Value = ds.Tables("table1").Rows(i).Item("SiteID").ToString()
                    LstVehicle.Items(j).Attributes.Add("style", "color:Black;")
                    LstVehicle.Items(j).Attributes.Add("onclick", "focusMarker(this);")
                    j = j + 1
                ElseIf Convert.ToInt32(ds.Tables("table1").Rows(i).Item("OandM_Head").ToString) >= 0 And Convert.ToInt32(ds.Tables("table1").Rows(i).Item("OandM_Head").ToString) <= 10 Then
                    LstVehicle.Items.Add(ds.Tables("table1").Rows(i).Item("Address").ToString())
                    LstVehicle.Items(j).Text = ds.Tables("table1").Rows(i).Item("Site_Name").ToString()
                    LstVehicle.Items(j).Value = ds.Tables("table1").Rows(i).Item("SiteID").ToString()
                    LstVehicle.Items(j).Attributes.Add("style", "color:Green;")
                    LstVehicle.Items(j).Attributes.Add("onclick", "focusMarker(this);")
                    j = j + 1
                ElseIf Convert.ToInt32(ds.Tables("table1").Rows(i).Item("OandM_Head").ToString) > 600 And Convert.ToInt32(ds.Tables("table1").Rows(i).Item("OandM_Head").ToString) <= 1440 Then
                    LstVehicle.Items.Add(ds.Tables("table1").Rows(i).Item("Address").ToString())
                    LstVehicle.Items(j).Text = ds.Tables("table1").Rows(i).Item("Site_Name").ToString()
                    LstVehicle.Items(j).Value = ds.Tables("table1").Rows(i).Item("SiteID").ToString()
                    LstVehicle.Items(j).Attributes.Add("style", "color:#B86A84;")
                    LstVehicle.Items(j).Attributes.Add("onclick", "focusMarker(this);")
                    j = j + 1
                Else
                    LstVehicle.Items.Add(ds.Tables("table1").Rows(i).Item("Address").ToString())
                    LstVehicle.Items(j).Text = ds.Tables("table1").Rows(i).Item("Site_Name").ToString()
                    LstVehicle.Items(j).Value = ds.Tables("table1").Rows(i).Item("SiteID").ToString()
                    LstVehicle.Items(j).Attributes.Add("style", "color:Red;")
                    LstVehicle.Items(j).Attributes.Add("onclick", "focusMarker(this);")
                    j = j + 1
                End If
                'End If
            Next
            Dim url As String
            Dim jqueryInclude As HtmlGenericControl = New HtmlGenericControl("script")
            If apikey = "" Then
                jqueryInclude.Attributes.Add("type", "text/javascript")
                'jqueryInclude.Attributes.Add("src", "http://maps.google.com/maps/api/js?sensor=false")
                url = ""
                Page.Header.Controls.Add(jqueryInclude)
            Else
                jqueryInclude.Attributes.Add("type", "text/javascript")
                'jqueryInclude.Attributes.Add("src", "http://www.google.com/jsapi?key=" + apikey + "")
                Page.Header.Controls.Add(jqueryInclude)
                url = "<script type='text/javascript'>google.load('maps', '4.7', { 'other_params': 'sensor=true' }); </script>"
            End If
            oda.Dispose()
            ds.Dispose()
            con.Dispose()
            Return "0"
        Catch ex As Exception
            Throw
        End Try
    End Function

    Public Shared strPathJson As String = HttpContext.Current.Server.MapPath("DOCS/Jasontext.json")
    <WebMethod()> _
    <Script.Services.ScriptMethod()> _
    Public Shared Function GetMarkerListJSON() As String
        'Dim strList As String = ""
        'strList = IDs.TrimEnd(CChar(","))
        'Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        'Dim con As New SqlConnection(conStr)
        'Dim apikey As String = String.Empty
        'Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
        Dim ds As New DataSet()
        ds.Tables.Add("table1")



        'Dim lines = IO.File.ReadAllLines(strPathJson1)

        'Dim tbl = New DataTable
        'Dim colCount = lines.First.Split(","c).Length
        'For i As Int32 = 0 To colCount - 1
        '    ds.Tables("table1").Columns.Add(New DataColumn(lines.First.Split(","c).ToArray(i).ToString, GetType(String)))
        '    'Exit For
        'Next
        'Dim k As Integer = 0
        'For Each line In lines
        '    If lines(0).ToString = lines(k).ToString Then
        '    Else
        '        Dim objFields = From field In line.Split(","c)
        '                 Select CType((field), Object)
        '        'Dim newRow = tbl.Rows.Add()
        '        Dim newRow = ds.Tables("table1").Rows.Add()
        '        newRow.ItemArray = objFields.ToArray()
        '    End If
        '    k = k + 1
        'Next

        'Dim tbl = New DataTable
        'Dim str As String = "select convert(nvarchar,g.tid)[TID],IMIENO[SiteID],convert(nvarchar,ctime,101)[SiteName],m2.fld1[Address],convert(nvarchar,msensor)[Site] ,convert(nvarchar,datediff(minute,ctime,getdate()))[info],convert(nvarchar,Lattitude)[Lat],convert(nvarchar,longitude)[Long], max(m2.fld14)[Group],max(m2.fld10)[Site_Name],DMS.IdealTime(g.imieno)[OandM_Head], g.Speed[Site_Address],''[Maintenance_Head],''[Opex_Manager],''[Security_Manager],''[Zonal_Head],''[Cluster_Manager],''[Supervisor],''[Technician],''[Anchor_OPCO],''[No_of_OPCOs],''[Cluster]"
        'str = str & "from mmm_mst_gpsdata g with (nolock) join mmm_mst_master m2 with (nolock) on m2.fld12=g.imieno  where m2.documenttype='vehicle' and m2.fld14 in (" & strList & ")  and m2.eid=54 group by g.tid,imieno,ctime,m2.fld1,msensor,lattitude,longitude,speed having ctime=(select max(ctime) from mmm_mst_gpsdata with(nolock) where imieno=g.imieno)"


        'Dim str As String = "select convert(nvarchar,g.tid)[TID],IMIENO[SiteID],convert(nvarchar,ctime,101)[SiteName],m2.fld1[Address],convert(nvarchar,msensor)[Site] ,convert(nvarchar,datediff(minute,ctime,getdate()))[info],convert(nvarchar,Lattitude)[Lat],convert(nvarchar,longitude)[Long], max(m2.fld14)[Group],max(m2.fld10)[Site_Name],DMS.IdealTime(g.imieno)[OandM_Head], g.Speed[Site_Address],''[Maintenance_Head],''[Opex_Manager],''[Security_Manager],''[Zonal_Head],''[Cluster_Manager],''[Supervisor],''[Technician],''[Anchor_OPCO],''[No_of_OPCOs],''[Cluster]"
        'If HttpContext.Current.Session("USERROLE").ToString.ToUpper = "SU" Or HttpContext.Current.Session("USERROLE").ToString.ToUpper = "CORPORATEUSER" Then
        '    str = str & "from mmm_mst_gpsdata g with (nolock) join mmm_mst_master m2 with (nolock) on m2.fld12=g.imieno   where m2.documenttype='vehicle' and m2.fld14 in (" & strList & ")  and m2.eid=54 group by g.tid,imieno,ctime,m2.fld1,msensor,lattitude,longitude,speed having ctime=(select max(ctime) from mmm_mst_gpsdata with (nolock) where imieno=g.imieno)"
        'Else
        '    str = str & "from mmm_mst_gpsdata g with (nolock) join mmm_mst_master m2 with (nolock) on m2.fld12=g.imieno inner join  dbo.split('" & HttpContext.Current.Session("RID") & "', ',') s on s.items in (select items from dbo.split(m2.fld16, ','))   where m2.documenttype='vehicle' and m2.fld14 in (" & strList & ")  and m2.eid=54 group by g.tid,imieno,ctime,m2.fld1,msensor,lattitude,longitude,speed having ctime=(select max(ctime) from mmm_mst_gpsdata where imieno=g.imieno)"
        'End If

        'oda.SelectCommand.CommandText = str
        'oda.SelectCommand.CommandTimeout = 180
        'oda.Fill(ds, "table1")
        'Dim d = ConvertDataTableTojSonString(ds.Tables("table1"))

        Dim d1 As String = IO.File.ReadAllText(strPathJson)
        'Dim d2 = d1 & Right(d, Len(d) - 1)
        Return d1

    End Function

    Public Shared Function ConvertDataTableTojSonString(dataTable As DataTable) As [String]
        Dim serializer As New System.Web.Script.Serialization.JavaScriptSerializer()
        Dim tableRows As New List(Of Dictionary(Of [String], [Object]))()
        Dim row As Dictionary(Of [String], [Object])

        For Each dr As DataRow In dataTable.Rows
            row = New Dictionary(Of [String], [Object])()
            For Each col As DataColumn In dataTable.Columns
                row.Add(col.ColumnName, dr(col))
            Next
            tableRows.Add(row)
        Next
        serializer.MaxJsonLength = 900000000
        Dim i As Integer = tableRows.Count
        Return serializer.Serialize(tableRows)
    End Function

End Class
