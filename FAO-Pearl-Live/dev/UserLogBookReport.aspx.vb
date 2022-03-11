Imports System.Data.SqlClient
Imports System.Data
Imports System.Web.Services
Imports System.IO
Imports Newtonsoft.Json.Converters
Imports Newtonsoft.Json
Imports System.Web.Script.Serialization
Imports System.Web.Script.Services


Partial Class UserLogBookReport
    Inherits System.Web.UI.Page

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
    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        hdnUID.Value = Session("UID")
        hdnurole.Value = Session("ROLES")
        hdnEid.Value = Session("Eid")

        Dim ConStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim da As New SqlDataAdapter("", ConStr)
        Dim dtMap As New DataTable
        da.SelectCommand.CommandText = "Select  maptype,APIKey from mmm_mst_Entity with(nolock) where Eid=" & Session("Eid")
        da.Fill(dtMap)
        If dtMap.Rows.Count > 0 Then
            hdnMap.Value = dtMap.Rows(0).Item("maptype").ToString()
        End If
    End Sub

    Public Function bindGrid() As DataSet
        Dim ds As New DataSet()
        Dim ConStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim d1 = Session("Sdate")
        Dim d2 = Session("edate")
        Using con As New SqlConnection(ConStr)
            Using da As New SqlDataAdapter("getindusLogBook1000_user", con)
                da.SelectCommand.CommandType = CommandType.StoredProcedure
                da.SelectCommand.Parameters.AddWithValue("@uid", Session("UID"))
                da.SelectCommand.Parameters.AddWithValue("@rolename", Session("USERROLE"))
                da.SelectCommand.Parameters.AddWithValue("@d1", d1)
                da.SelectCommand.Parameters.AddWithValue("@d2", d2)
                da.SelectCommand.CommandTimeout = 120
                da.Fill(ds)
            End Using
        End Using
        Return ds
    End Function

    Public Function GetLogBook(d1 As String, d2 As String) As DataSet
        Dim ds As New DataSet()
        Dim ConStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Using con As New SqlConnection(ConStr)
            Using da As New SqlDataAdapter("getindusLogBook1000_user", con)
                da.SelectCommand.CommandType = CommandType.StoredProcedure
                da.SelectCommand.Parameters.AddWithValue("@uid", Session("UID"))
                da.SelectCommand.Parameters.AddWithValue("@rolename", Session("USERROLE"))
                da.SelectCommand.Parameters.AddWithValue("@d1", d1)
                da.SelectCommand.Parameters.AddWithValue("@d2", d2)
                da.SelectCommand.CommandTimeout = 120
                da.Fill(ds)
            End Using
        End Using
        Return ds
    End Function
    <WebMethod()>
    Public Shared Function GetJSON(d1 As String, d2 As String) As pichartColUserLog
        Dim ret As New pichartColUserLog()
        Dim ds As New DataSet()
        Dim dt As New DataTable()
        Dim Query As String = ""
        Dim Query2 As String = ""
        HttpContext.Current.Session("Sdate") = d1
        HttpContext.Current.Session("edate") = d2

        If HttpContext.Current.Session("EID") = 58 Then
            If HttpContext.Current.Session("USERROLE").ToString.ToUpper = "SU" Or HttpContext.Current.Session("USERROLE").ToString.ToUpper = "CORPORATEUSER" Then

                If d1 = "" And d2 = "" Then
                    Query = "select  dms.udf_split('MASTER-Vehicle Type-fld1',m.fld14)[Vehicle Type], (select count(distinct e.vehicle_no) from MMM_MST_MASTER m1 inner join  MMM_MST_ELOGBOOK e on e.vehicle_no = m1.fld1 where m1.eid =58 and e.eid=58 and convert(date, Trip_start_datetime) >=convert(date,getdate()-1) and convert(date,Trip_end_datetime)<= convert(date,getdate()-1) and  m1.fld14 = m.fld14"
                    Query &= " ) [Total Vehicle Count],(select count(m3.fld14) from mmm_mst_elogbook e with (nolock) inner join mmm_mst_master m3 with (nolock) on e.vehicle_no=m3.fld1 "
                    Query &= "where sitevisiteid<>0 and m3.fld14=m.fld14 and Convert(date,trip_start_datetime)>=convert(date,getdate()-1)  and Convert(date,trip_End_datetime)<=convert(date,getdate()-1) )[Total Site Visit Count],sum(convert(int,round(convert(numeric(10,4),isnull(e.total_distance,0)),0)))[Total Km.] from mmm_mst_elogbook e with (nolock) join "
                    Query &= "mmm_mst_master m with (nolock) on m.fld1=e.vehicle_no where m.eid=58 and e.eid=58 and Convert(date,trip_start_datetime)>=convert(date,getdate()-1) and Convert(date,trip_End_datetime)<=convert(date,getdate()-1) and e.Triptype = 'proxycard' group by m.fld14 " &
                     " select Count( Distinct CardID) CardID, (select fld1 from mmm_mst_Master emp where emp.eid =58 and emp.documenttype ='employee master' and emp.fld11 = e.CardID) [User Name]" &
   "from mmm_mst_elogbook e with (nolock) join mmm_mst_master m with (nolock) on m.fld1=e.vehicle_no  where m.eid=58 and e.eid=58 and e.triptype ='proxycard' group by e.CardID"
                Else
                    Query &= "select  dms.udf_split('MASTER-Vehicle Type-fld1',m.fld14)[Vehicle Type],(select count(distinct e.vehicle_no) from MMM_MST_MASTER m1 inner join  MMM_MST_ELOGBOOK e on e.vehicle_no = m1.fld1 where m1.eid = 58 And e.eid = 58 And Convert( Date, Trip_start_datetime)>= ' " & d1 & "' And Convert( Date, Trip_end_datetime) <= '" & d2 & "' And m1.fld14 = m.fld14"
                    Query &= ") [Total Vehicle Count],(select count(m3.fld14) from mmm_mst_elogbook e with (nolock) inner join mmm_mst_master m3 with (nolock) on e.vehicle_no=m3.fld1 "
                    Query &= "where sitevisiteid<>0 and m3.fld14=m.fld14 and Convert(date,trip_start_datetime)>='" & d1 & "'  and Convert(date,trip_End_datetime)<='" & d2 & "')[Total Site Visit Count],sum(convert(int,round(convert(numeric(10,4),isnull(e.total_distance,0)),0)))[Total Km.] from mmm_mst_elogbook e with (nolock) join "
                    Query &= "mmm_mst_master m with (nolock) on m.fld1=e.vehicle_no where m.eid=58 and e.eid=58 and Convert(date,trip_start_datetime)>='" & d1 & "' and Convert(date,trip_End_datetime)<='" & d2 & "' and Convert(date,trip_start_datetime)>='" & d1 & "' and Convert(date,trip_End_datetime)<='" & d2 & "' and e.Triptype = 'proxycard' group by m.fld14" &
                         " select Count( Distinct CardID) CardID, (select fld1 from mmm_mst_Master emp where emp.eid =58 and emp.documenttype ='employee master' and emp.fld11 = e.CardID) [User Name]" &
   "from mmm_mst_elogbook e with (nolock) join mmm_mst_master m with (nolock) on m.fld1=e.vehicle_no   where m.eid=58 and e.eid=58  and Convert(date,trip_start_datetime)>='" & d1 & "' and Convert(date,trip_End_datetime)<='" & d2 & "' and e.triptype ='proxycard' group by e.CardID"
                End If

            Else

                If d1 = "" And d2 = "" Then
                    Query &= "select  dms.udf_split('MASTER-Vehicle Type-fld1',m.fld14)[Vehicle Type],(select count(distinct e.vehicle_no) from MMM_MST_MASTER m1 inner join  MMM_MST_ELOGBOOK e on e.vehicle_no = m1.fld1 where m1.eid =" & HttpContext.Current.Session("EID") & " and e.eid=" & HttpContext.Current.Session("EID") & " and convert(date, Trip_start_datetime) >=convert(date,getdate()-1) and convert(date,Trip_end_datetime)<=convert(date,getdate()-1) and  m1.fld14 = m.fld14"
                    Query &= ") [Total Vehicle Count],(select count(m3.fld14) from mmm_mst_elogbook e with (nolock) inner join mmm_mst_master m3 with (nolock) on e.vehicle_no=m3.fld1 "
                    Query &= "where sitevisiteid<>0 and e.compID=(Select fld12 from mmm_mst_user where uid=" & HttpContext.Current.Session("UID") & " and eid=58) and m3.fld14=m.fld14 and Convert(date,trip_start_datetime)>=convert(date,getdate()-1)  and Convert(date,trip_End_datetime)<=convert(date,getdate()-1) )[Total Site Visit Count],sum(convert(int,round(convert(numeric(10,4),isnull(e.total_distance,0)),0)))[Total Km.] from mmm_mst_elogbook e with (nolock) join "
                    Query &= "mmm_mst_master m with (nolock) on m.fld1=e.vehicle_no where m.eid=58 and e.eid=58 and Convert(date,trip_start_datetime)>=convert(date,getdate()-1) and Convert(date,trip_End_datetime)<=convert(date,getdate()-1) and e.CompID in (select items from dbo.split((select fld1 from mmm_ref_role_user with (nolock) where eid=" & HttpContext.Current.Session("EID") & " and uid=" & HttpContext.Current.Session("UID") & " and rolename='" & HttpContext.Current.Session("USERROLE") & "' ), ',')) and triptype='proxycard' group by m.fld14" &
                           " select Count( Distinct CardID) CardID , (select fld1 from mmm_mst_Master emp where emp.eid =58 and emp.documenttype ='employee master' and emp.fld11 = e.CardID) [User Name]" &
   "from mmm_mst_elogbook e with (nolock) join mmm_mst_master m with (nolock) on m.fld1=e.vehicle_no  inner join  dbo.split((select fld1 from mmm_ref_role_user where uid=" & HttpContext.Current.Session("UID") & " and rolename='" & HttpContext.Current.Session("USERROLE") & "' and eid=58), ',') s on s.items in (select items from dbo.split(m.fld16, ',')) where m.eid=58 and e.eid=58  and e.triptype ='proxycard' group by e.CardID"

                Else
                    Query &= "select  dms.udf_split('MASTER-Vehicle Type-fld1',m.fld14)[Vehicle Type],  (select count(distinct e.vehicle_no) from MMM_MST_MASTER m1 inner join  MMM_MST_ELOGBOOK e on e.vehicle_no = m1.fld1 where m1.eid =" & HttpContext.Current.Session("EID") & " and e.eid=" & HttpContext.Current.Session("EID") & " and convert(date, Trip_start_datetime) >='" & d1 & "' and convert(date,Trip_end_datetime)<='" & d2 & "' and  m1.fld14 = m.fld14"
                    Query &= ") [Total Vehicle Count],(select count(m3.fld14) from mmm_mst_elogbook e with (nolock) inner join mmm_mst_master m3 with (nolock) on e.vehicle_no=m3.fld1 "
                    Query &= "where sitevisiteid<>0 and e.compID=(Select fld12 from mmm_mst_user where uid=" & HttpContext.Current.Session("UID") & " and eid=58) and m3.fld14=m.fld14 and Convert(date,trip_start_datetime)>='" & d1 & "'  and Convert(date,trip_End_datetime)<='" & d2 & "' )[Total Site Visit Count],sum(convert(int,round(convert(numeric(10,4),isnull(e.total_distance,0)),0)))[Total Km.] from mmm_mst_elogbook e with (nolock) join "
                    Query &= "mmm_mst_master m with (nolock) on m.fld1=e.vehicle_no where m.eid=58 and e.eid=58  and Convert(date,trip_start_datetime)>='" & d1 & "' and Convert(date,trip_End_datetime)<='" & d2 & "' and e.CompID in (select items from dbo.split((select fld1 from mmm_ref_role_user with (nolock) where eid=" & HttpContext.Current.Session("EID") & " and uid=" & HttpContext.Current.Session("UID") & " and rolename='" & HttpContext.Current.Session("USERROLE") & "' ), ','))  and triptype ='proxycard' group by m.fld14" &
                     " select Count( Distinct CardID) CardID, (select fld1 from mmm_mst_Master emp where emp.eid =58 and emp.documenttype ='employee master' and emp.fld11 = e.CardID) [User Name]" &
   "from mmm_mst_elogbook e with (nolock) join mmm_mst_master m with (nolock) on m.fld1=e.vehicle_no inner join  dbo.split((select fld1 from mmm_ref_role_user where uid=" & HttpContext.Current.Session("UID") & " and rolename='" & HttpContext.Current.Session("USERROLE") & "' and eid=58), ',') s on s.items in (select items from dbo.split(m.fld16, ','))  where m.eid=58 and e.eid=58  and Convert(date,trip_start_datetime)>='" & d1 & "' and Convert(date,trip_End_datetime)<='" & d2 & "' and e.triptype ='proxycard' group by e.CardID"

                End If

            End If

        Else

            If HttpContext.Current.Session("USERROLE").ToString.ToUpper = "SU" Or HttpContext.Current.Session("USERROLE").ToString.ToUpper = "CORPORATEUSER" Then
                If d1 = "" And d2 = "" Then
                    Query = "select  dms.udf_split('MASTER-Vehicle Type-fld1',m.fld14)[Vehicle Type],(select count(distinct e.vehicle_no) from MMM_MST_MASTER m1 inner join  MMM_MST_ELOGBOOK e on e.vehicle_no = m1.fld1 where m1.eid =" & HttpContext.Current.Session("EID") & " and e.eid=" & HttpContext.Current.Session("EID") & " and convert(date, Trip_start_datetime) >=convert(date,getdate()-1) and convert(date,Trip_end_datetime)<=convert(date,getdate()-1) and  m1.fld14 = m.fld14"
                    Query &= ") [Total Vehicle Count],(select count(m3.fld14) from mmm_mst_elogbook e with (nolock) inner join mmm_mst_master m3 with (nolock) on e.vehicle_no=m3.fld1 where sitevisiteid<>0 and m3.fld14=m.fld14 and Convert(date,trip_start_datetime)>=convert(date,getdate()-1) and Convert(date,trip_End_datetime)<=convert(date,getdate()-1) )[Total Site Visit Count],sum(convert(int,round(convert(numeric(10,4),isnull(e.total_distance,0)),0)))[Total Km.] from mmm_mst_elogbook e with (nolock) join mmm_mst_master m with (nolock) on m.fld1=e.vehicle_no  where m.eid=" & HttpContext.Current.Session("Eid") & " and e.eid=" & HttpContext.Current.Session("Eid") & " and Convert(date,trip_start_datetime)>=convert(date,getdate()-1) and Convert(date,trip_End_datetime)<=convert(date,getdate()-1) and e.triptype ='proxycard' group by m.fld14" &
                        " select Count( Distinct CardID) CardID, (select fld1 from mmm_mst_Master emp where emp.eid =" & HttpContext.Current.Session("EID") & " and emp.documenttype ='employee master' and emp.fld11 = e.CardID) [User Name]" &
   "from mmm_mst_elogbook e with (nolock) join mmm_mst_master m with (nolock) on m.fld1=e.vehicle_no  where m.eid=" & HttpContext.Current.Session("EID") & " and e.eid=" & HttpContext.Current.Session("EID") & "  and e.triptype ='proxycard' group by e.CardID"
                Else
                    Query = "select  dms.udf_split('MASTER-Vehicle Type-fld1',m.fld14)[Vehicle Type],(select count(distinct e.vehicle_no) from MMM_MST_MASTER m1 inner join  MMM_MST_ELOGBOOK e on e.vehicle_no = m1.fld1 where m1.eid =" & HttpContext.Current.Session("EID") & " and e.eid=" & HttpContext.Current.Session("EID") & " and convert(date, Trip_start_datetime) >='" & d1 & "' and convert(date,Trip_end_datetime)<='" & d2 & "' and  m1.fld14 = m.fld14"
                    Query &= ") [Total Vehicle Count],(select count(m3.fld14) from mmm_mst_elogbook e with (nolock) inner join mmm_mst_master m3 with (nolock) on e.vehicle_no=m3.fld1 where sitevisiteid<>0 and m3.fld14=m.fld14 and Convert(date,trip_start_datetime)>='" & d1 & "' and Convert(date,trip_End_datetime)<='" & d2 & "' )[Total Site Visit Count],sum(convert(int,round(convert(numeric(10,4),isnull(e.total_distance,0)),0)))[Total Km.] from mmm_mst_elogbook e with (nolock) join mmm_mst_master m with (nolock) on m.fld1=e.vehicle_no  where m.eid=" & HttpContext.Current.Session("Eid") & " and e.eid=" & HttpContext.Current.Session("Eid") & " and Convert(date,trip_start_datetime)>='" & d1 & "' and Convert(date,trip_End_datetime)<='" & d2 & "' and e.triptype ='proxycard'  group by m.fld14" &
                         " select Count( Distinct CardID) CardID, (select fld1 from mmm_mst_Master emp where emp.eid =" & HttpContext.Current.Session("EID") & " and emp.documenttype ='employee master' and emp.fld11 = e.CardID) [User Name]" &
   "from mmm_mst_elogbook e with (nolock) join mmm_mst_master m with (nolock) on m.fld1=e.vehicle_no   where m.eid=" & HttpContext.Current.Session("EID") & " and e.eid=" & HttpContext.Current.Session("EID") & "  and Convert(date,trip_start_datetime)>='" & d1 & "' and Convert(date,trip_End_datetime)<='" & d2 & "' and e.triptype ='proxycard' group by e.CardID"
                End If
            Else
                If d1 = "" And d2 = "" Then
                    Query = "select  dms.udf_split('MASTER-Vehicle Type-fld1',m.fld14)[Vehicle Type],(select count(distinct e.vehicle_no) from MMM_MST_MASTER m1 inner join  MMM_MST_ELOGBOOK e on e.vehicle_no = m1.fld1 where m1.eid =" & HttpContext.Current.Session("EID") & " and e.eid=" & HttpContext.Current.Session("EID") & " and convert(date, Trip_start_datetime) >=convert(date,getdate()-1) and convert(date,Trip_end_datetime)<=convert(date,getdate()-1) and  m1.fld14 = m.fld14"
                    Query &= ") [Total Vehicle Count],(select count(m3.fld14) from mmm_mst_elogbook e with (nolock) inner join mmm_mst_master m3 with (nolock) on e.vehicle_no=m3.fld1 where sitevisiteid<>0 and m3.fld14=m.fld14 and Convert(date,trip_start_datetime)>=convert(date,getdate()-1)  and Convert(date,trip_End_datetime)<=convert(date,getdate()-1)  )[Total Site Visit Count],sum(convert(int,round(convert(numeric(10,4),isnull(e.total_distance,0)),0)))[Total Km.] from mmm_mst_elogbook e with (nolock) join mmm_mst_master m with (nolock) on m.fld1=e.vehicle_no inner join  dbo.split((select fld1 from mmm_ref_role_user where uid=" & HttpContext.Current.Session("UID") & " and rolename='" & HttpContext.Current.Session("USERROLE") & "' and eid=" & HttpContext.Current.Session("EID") & "), ',') s on s.items in (select items from dbo.split(m.fld16, ','))  where m.eid=" & HttpContext.Current.Session("Eid") & " and e.eid=" & HttpContext.Current.Session("Eid") & " and Convert(date,trip_start_datetime)>=convert(date,getdate()-1)  and Convert(date,trip_End_datetime)<=convert(date,getdate()-1)  and e.triptype ='proxycard' group by m.fld14" &
                          " select Count( Distinct CardID) CardID , (select fld1 from mmm_mst_Master emp where emp.eid =" & HttpContext.Current.Session("EID") & " and emp.documenttype ='employee master' and emp.fld11 = e.CardID) [User Name]" &
   "from mmm_mst_elogbook e with (nolock) join mmm_mst_master m with (nolock) on m.fld1=e.vehicle_no  inner join  dbo.split((select fld1 from mmm_ref_role_user where uid=" & HttpContext.Current.Session("UID") & " and rolename='" & HttpContext.Current.Session("USERROLE") & "' and eid=" & HttpContext.Current.Session("EID") & "), ',') s on s.items in (select items from dbo.split(m.fld16, ',')) where m.eid=" & HttpContext.Current.Session("EID") & " and e.eid=" & HttpContext.Current.Session("EID") & "  and e.triptype ='proxycard' group by e.CardID"
                Else
                    Query = "select  dms.udf_split('MASTER-Vehicle Type-fld1',m.fld14)[Vehicle Type],(select count(distinct e.vehicle_no) from MMM_MST_MASTER m1 inner join  MMM_MST_ELOGBOOK e on e.vehicle_no = m1.fld1 where m1.eid =" & HttpContext.Current.Session("EID") & " and e.eid=" & HttpContext.Current.Session("EID") & " and convert(date, Trip_start_datetime) >='" & d1 & "' and convert(date,Trip_end_datetime)<='" & d2 & "' and  m1.fld14 = m.fld14"
                    Query &= ") [Total Vehicle Count],(select count(m3.fld14) from mmm_mst_elogbook e with (nolock) inner join mmm_mst_master m3 with (nolock) on e.vehicle_no=m3.fld1 where sitevisiteid<>0 and m3.fld14=m.fld14 and Convert(date,trip_start_datetime)>='" & d1 & "' and Convert(date,trip_End_datetime)<='" & d2 & "' )[Total Site Visit Count],sum(convert(int,round(convert(numeric(10,4),isnull(e.total_distance,0)),0)))[Total Km.] from mmm_mst_elogbook e with (nolock) join mmm_mst_master m with (nolock) on m.fld1=e.vehicle_no inner join  dbo.split((select fld1 from mmm_ref_role_user where uid=" & HttpContext.Current.Session("UID") & " and rolename='" & HttpContext.Current.Session("USERROLE") & "' and eid=" & HttpContext.Current.Session("EID") & "), ',') s on s.items in (select items from dbo.split(m.fld16, ','))  where m.eid=" & HttpContext.Current.Session("Eid") & " and e.eid=" & HttpContext.Current.Session("Eid") & " and Convert(date,trip_start_datetime)>='" & d1 & "' and Convert(date,trip_End_datetime)<='" & d2 & "' and e.triptype ='proxycard'  group by m.fld14" &
                         " select Count( Distinct CardID) CardID, (select fld1 from mmm_mst_Master emp where emp.eid =" & HttpContext.Current.Session("EID") & " and emp.documenttype ='employee master' and emp.fld11 = e.CardID) [User Name]" &
   "from mmm_mst_elogbook e with (nolock) join mmm_mst_master m with (nolock) on m.fld1=e.vehicle_no inner join  dbo.split((select fld1 from mmm_ref_role_user where uid=" & HttpContext.Current.Session("UID") & " and rolename='" & HttpContext.Current.Session("USERROLE") & "' and eid=" & HttpContext.Current.Session("EID") & "), ',') s on s.items in (select items from dbo.split(m.fld16, ','))  where m.eid=" & HttpContext.Current.Session("EID") & " and e.eid=" & HttpContext.Current.Session("EID") & "  and Convert(date,trip_start_datetime)>='" & d1 & "' and Convert(date,trip_End_datetime)<='" & d2 & "' and e.triptype ='proxycard' group by e.CardID"
                End If
            End If
        End If



        Dim ConStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Using con As New SqlConnection(ConStr)
            Using da As New SqlDataAdapter(Query, con)
                da.Fill(ds)
            End Using
        End Using
        Try
            If ds.Tables(0).Rows.Count > 0 Then
                'For Each column As DataColumn In ds.Tables(0).Columns
                'Next
                Dim obj As pichartUserLog
                Dim o1 As New List(Of pichartUserLog)()
                Dim o2 As New List(Of pichartUserLog)()
                Dim o3 As New List(Of pichartUserLog)()

                For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                    obj = New pichartUserLog()
                    obj.category = ds.Tables(0).Rows(i).Item("Vehicle Type")
                    obj.value = ds.Tables(0).Rows(i).Item("Total Vehicle Count")
                    o1.Add(obj)
                    obj = New pichartUserLog()
                    obj.category = ds.Tables(0).Rows(i).Item("Vehicle Type")
                    obj.value = ds.Tables(0).Rows(i).Item("Total Site Visit Count")
                    o2.Add(obj)

                Next

                obj = New pichartUserLog()
                obj.category = ds.Tables(1).Rows(0).Item("User Name")
                obj.value = ds.Tables(1).Rows(0).Item("CardID")
                o3.Add(obj)

                ret.data1 = o1
                ret.data2 = o2
                ret.data3 = o3
            End If
        Catch Ex As Exception
            Throw
        End Try
        Return ret
    End Function

    Protected Sub btnExcelExport_Click(sender As Object, e As System.Web.UI.ImageClickEventArgs) Handles btnExcelExport.Click
        Dim ds1 As New DataSet()
        ds1 = bindGrid()
        Dim grdTripData As New GridView
        grdTripData.DataSource = ds1
        grdTripData.DataBind()
        grdTripData.ShowHeader = True
        grdTripData.RowHeaderColumn = True
        grdTripData.AllowPaging = False
        Response.Clear()
        Response.Buffer = True
        Response.Write("<br/><div align=""center"" style=""border:1px solid red"" ><h3>VEHICLE LOG BOOK (Electronic)</h3></div> <br/>")
        Response.AddHeader("content-disposition", "attachment;filename=Trip Report.xls")
        Response.Charset = ""
        Response.ContentType = "application/vnd.ms-excel"
        Dim sw As New StringWriter()
        Dim hw As New HtmlTextWriter(sw)
        For i As Integer = 0 To grdTripData.Rows.Count - 1
            'Apply text style to each Row 
            grdTripData.Rows(i).Attributes.Add("class", "textmode")
        Next
        grdTripData.RenderControl(hw)
        'style to format numbers to string 
        Dim style As String = "<style> .textmode{mso-number-format:\@;}</style>"
        Response.Write(style)
        Response.Output.Write(sw.ToString())
        Response.Flush()
        Response.End()
    End Sub

    <WebMethod()>
    Public Shared Function FillUserDetails(ByVal vehno As String, ByVal start As String, ByVal enddate As String) As String
        Dim ds As New DataSet()
        Dim ret = ""
        Dim ConStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Using con As New SqlConnection(ConStr)
            Using da As New SqlDataAdapter("getLogBookDetailsP_User", con)
                da.SelectCommand.CommandType = CommandType.StoredProcedure
                da.SelectCommand.Parameters.AddWithValue("@eid", HttpContext.Current.Session("Eid"))
                da.SelectCommand.Parameters.AddWithValue("@vehno", vehno)
                da.SelectCommand.Parameters.AddWithValue("@d1", start)
                da.SelectCommand.Parameters.AddWithValue("@d2", enddate)
                da.SelectCommand.CommandTimeout = 120
                da.Fill(ds)
                If ds.Tables(0).Rows.Count > 0 Then
                    For j As Integer = 0 To ds.Tables(0).Columns.Count - 1
                        ds.Tables(0).Columns(j).ColumnName = ds.Tables(0).Columns(j).ColumnName.Replace(" ", "_")
                    Next
                End If
                Dim serializerSettings As New JsonSerializerSettings()
                Dim json_serializer As New JavaScriptSerializer()
                serializerSettings.Converters.Add(New DataTableConverter())
                Dim jsonData As [String] = JsonConvert.SerializeObject(ds.Tables(0), Formatting.None, serializerSettings)
                ret = jsonData
            End Using
        End Using
        Return ret
    End Function

End Class
Public Class pichartUserLog
    Public category As String
    Public value As String

End Class
Public Class pichartColUserLog
    Public data1 As New List(Of pichartUserLog)()
    Public data2 As New List(Of pichartUserLog)()
    Public data3 As New List(Of pichartUserLog)()
End Class