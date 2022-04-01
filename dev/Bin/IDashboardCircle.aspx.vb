Imports System.Data.SqlClient
Imports System.Data
Imports System.Web.Services
Imports Newtonsoft.Json
Imports System.Web.Script.Serialization
Imports Newtonsoft.Json.Converters
Public Class IDashboardCircle
    Inherits System.Web.UI.Page

    <WebMethod()> _
      <Script.Services.ScriptMethod()> _
    Public Shared Function Mileage() As VehicleTKM
        Dim vtm As New VehicleTKM


        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim Qry As String = "select circle[CircleName],vtype[VehicleType],count[TotalKm] from OverspeedChart"
        Dim ds As New DataSet
        Try
            Using con As New SqlConnection(conStr)
                Using da As New SqlDataAdapter(Qry, con)
                    da.Fill(ds, "data")
                    da.SelectCommand.CommandText = "select distinct fld1[Circle] from mmm_mst_master with(nolock)  where documenttype='circle' and eid=54 order by fld1"
                    da.Fill(ds, "circle")
                    da.SelectCommand.CommandText = "select distinct fld1[Type] from mmm_mst_master with(nolock)  where documenttype='vehicle type' and eid=54 order by fld1"
                    da.Fill(ds, "type")
                End Using
            End Using
            Dim slist As New List(Of seriesTKm)
            Dim c As New categoryAxisTkm
            Dim LST As New List(Of String)

            For i As Integer = 0 To ds.Tables("Type").Rows.Count - 1
                Dim s As New seriesTKm
                s.name = ds.Tables("Type").Rows(i).Item(0).ToString
                Dim Ser As New List(Of Decimal)
                For j = 0 To ds.Tables("circle").Rows.Count - 1
                    'If Convert.ToString(ds.Tables("data").Rows(i)(0)) = Convert.ToString(ds.Tables("circle").Rows(j)(0)) Then
                    Dim dt As New DataTable
                    Dim drs = ds.Tables("data").Select("CircleName='" & ds.Tables("circle").Rows(j).Item(0).ToString & "' and vehicletype='" & ds.Tables("Type").Rows(i).Item(0).ToString & "'")
                    Dim sum As Decimal = 0
                    If drs.Length > 0 Then
                        dt = drs.CopyToDataTable()
                        sum = Convert.ToDecimal(dt.Compute("SUM(TotalKm)", ""))
                    End If
                    Ser.Add(sum)

                    'End If
                    If i = 0 Then
                        Dim ciritem As String = ds.Tables("circle").Rows(j).Item(0).ToString
                        If ciritem = "MAHARASHTRA AND GOA" Then
                            ciritem = "M &G"
                        End If
                        If ciritem = "Andhra Pradesh" Then
                            ciritem = "AP"
                        End If
                        If ciritem = "Delhi NCR" Then
                            ciritem = "Delhi"
                        End If
                        LST.Add(ciritem)
                    End If
                Next
                s.data = Ser
                slist.Add(s)
            Next

            c.categories = LST
            vtm.series = slist
            vtm.cata = c
            vtm.success = True

        Catch ex As Exception
            vtm.success = False
            vtm.msg = "Error"
        Finally
            ds.Dispose()
        End Try
        Return vtm
    End Function
    <WebMethod()> _
    Public Shared Function getVehicle(Circle As String, vtype As String) As String
        Dim jsonData As String = ""

        Try
            Dim UID As Integer = 0
            Dim URole As String = ""
            Dim ds As New DataSet()
            Try
                UID = Convert.ToInt32(HttpContext.Current.Session("UID").ToString())
                URole = HttpContext.Current.Session("USERROLE").ToString()
                UID = 54
                URole = "SU"
                If (UID = 0 Or URole = "") Then
                    Return "NoSession"
                End If
            Catch ex As Exception
                Return "NoSession"
            End Try


            Dim Query As String = ""
            Dim objcls As New GprsRepots
            Query = "select IMEI,maxspeed,logdate from chart where CircleName='" & Circle & "' and VehicleType='" & vtype & "'"
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Using con As New SqlConnection(conStr)
                Using da As New SqlDataAdapter(Query, con)
                    da.Fill(ds)
                End Using
            End Using

            Dim serializerSettings As New JsonSerializerSettings()
            Dim json_serializer As New JavaScriptSerializer()
            serializerSettings.Converters.Add(New DataTableConverter())
            jsonData = JsonConvert.SerializeObject(ds.Tables(0), Newtonsoft.Json.Formatting.None, serializerSettings)

        Catch Ex As Exception
            Throw
        End Try
        Return jsonData

    End Function

    <WebMethod()> _
      <Script.Services.ScriptMethod()> _
    Public Shared Function OverSpeed() As VehicleTKM
        Dim vtm As New VehicleTKM
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim Qry As String = "select circle[CircleName],vtype[VehicleType],count[TotalKm] from OverspeedChart"
        Dim ds As New DataSet
        Try
            Using con As New SqlConnection(conStr)
                Using da As New SqlDataAdapter(Qry, con)
                    da.Fill(ds, "data")
                    da.SelectCommand.CommandText = "select distinct fld1[Circle] from mmm_mst_master with(nolock)  where documenttype='circle' and eid=54 order by fld1"
                    da.Fill(ds, "circle")
                    da.SelectCommand.CommandText = "select distinct fld1[Type] from mmm_mst_master with(nolock)  where documenttype='vehicle type' and eid=54 order by fld1"
                    da.Fill(ds, "type")
                End Using
            End Using
            Dim slist As New List(Of seriesTKm)
            Dim c As New categoryAxisTkm
            Dim LST As New List(Of String)

            For i As Integer = 0 To ds.Tables("Type").Rows.Count - 1
                Dim s As New seriesTKm
                s.name = ds.Tables("Type").Rows(i).Item(0).ToString
                Dim Ser As New List(Of Decimal)
                For j = 0 To ds.Tables("circle").Rows.Count - 1
                    'If Convert.ToString(ds.Tables("data").Rows(i)(0)) = Convert.ToString(ds.Tables("circle").Rows(j)(0)) Then
                    Dim dt As New DataTable
                    Dim drs = ds.Tables("data").Select("CircleName='" & ds.Tables("circle").Rows(j).Item(0).ToString & "' and vehicletype='" & ds.Tables("Type").Rows(i).Item(0).ToString & "'")
                    Dim sum As Decimal = 0
                    If drs.Length > 0 Then
                        dt = drs.CopyToDataTable()
                        sum = Convert.ToDecimal(dt.Compute("SUM(TotalKm)", ""))
                    End If
                    Ser.Add(sum)

                    'End If
                    If i = 0 Then
                        Dim ciritem As String = ds.Tables("circle").Rows(j).Item(0).ToString
                        If ciritem = "MAHARASHTRA AND GOA" Then
                            ciritem = "M &G"
                        End If
                        If ciritem = "Andhra Pradesh" Then
                            ciritem = "AP"
                        End If
                        If ciritem = "Delhi NCR" Then
                            ciritem = "Delhi"
                        End If
                        LST.Add(ciritem)
                    End If
                Next
                s.data = Ser
                slist.Add(s)
            Next

            c.categories = LST
            vtm.series = slist
            vtm.cata = c
            vtm.success = True

        Catch ex As Exception
            vtm.success = False
            vtm.msg = "Error"
        Finally
            ds.Dispose()
        End Try
        Return vtm
    End Function
    <WebMethod()> _
    Public Shared Function getOverSpeedDtl(Circle As String, vtype As String) As DGrid
        Dim jsonData As String = ""
        Dim grid As New DGrid()
        Try

            Dim ds As New DataSet()

            Dim Query As String = ""
            Dim objcls As New GprsRepots
            If Circle = "1 to 10" Then
                Query = "select max(m.CircleName)[Circle],max(m.VehicleName)[Vehicle_Name],max(m.VehicleNo)[Vehicle_No] "
                Query &= ",max(m.VehicleType)[Vehicle_Type],convert(nvarchar,convert(date,trip_start_datetime),101)[Date], "
                Query &= "isnull(max(MaxSpeed),0)[Maximum_Speed] from mmm_mst_elogbook e with (nolock) join "
                Query &= "mmm_mst_Vehiclemaster m with (nolock) on m.VehicleNo=e.vehicle_no where MaxSpeed>=80 and "
                Query &= "convert(date,trip_start_datetime)>=getdate()-10 and convert(date,trip_start_datetime)<=getdate() and m.VehicleType='" & vtype & "' and e.TripType='Auto' "
                Query &= "group by Vehicle_no,convert(date,trip_start_datetime) order by max(m.VehicleNo)"

            ElseIf Circle = "11 to 20" Then
                Query = "select max(m.CircleName)[Circle],max(m.VehicleName)[Vehicle_Name],max(m.VehicleNo)[Vehicle_No] "
                Query &= ",max(m.VehicleType)[Vehicle_Type],convert(nvarchar,convert(date,trip_start_datetime),101)[Date], "
                Query &= " isnull(max(MaxSpeed),0)[Maximum_Speed] from mmm_mst_elogbook e with (nolock) join "
                Query &= "mmm_mst_Vehiclemaster m with (nolock) on m.VehicleNo=e.vehicle_no where MaxSpeed>=80 and "
                Query &= "convert(date,trip_start_datetime)>=getdate()-20 and convert(date,trip_start_datetime)<=getdate()-11 and m.VehicleType='" & vtype & "' and e.TripType='Auto' "
                Query &= "group by Vehicle_no,convert(date,trip_start_datetime) order by max(m.VehicleNo)"

            ElseIf Circle = "21 to 30" Then
                Query = "select max(m.CircleName)[Circle],max(m.VehicleName)[Vehicle_Name],max(m.VehicleNo)[Vehicle_No] "
                Query &= ",max(m.VehicleType)[Vehicle_Type],convert(nvarchar,convert(date,trip_start_datetime),101)[Date], "
                Query &= "isnull(max(MaxSpeed),0)[Maximum_Speed] from mmm_mst_elogbook e with (nolock) join "
                Query &= "mmm_mst_Vehiclemaster m with (nolock) on m.VehicleNo=e.vehicle_no where MaxSpeed>=80 and "
                Query &= "convert(date,trip_start_datetime)>=getdate()-30 and convert(date,trip_start_datetime)<=getdate()-21 and m.VehicleType='" & vtype & "' and e.TripType='Auto' "
                Query &= "group by Vehicle_no,convert(date,trip_start_datetime) order by max(m.VehicleNo)"
            End If


            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Using con As New SqlConnection(conStr)
                Using da As New SqlDataAdapter(Query, con)
                    da.Fill(ds)
                End Using
            End Using

            Dim strError = ""
            grid = DynamicGrid.GridData(ds.Tables(0), strError)
            If ds.Tables(0).Rows.Count = 0 Then
                grid.Message = "No data found...!"
                grid.Success = False
            End If
        Catch Ex As Exception
            grid.Success = False
            grid.Message = "No data found...!"
            grid.Count = 0
        End Try
        Return grid

    End Function

    <WebMethod()> _
      <Script.Services.ScriptMethod()> _
    Public Shared Function MANPOWER() As VehicleTKM
        Dim vtm As New VehicleTKM
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim Qry As String = ""

        Qry = "select circle[Circle],role[Role],msiteid[MsiteID],vsiteid from Manpowercircle"

        'Qry = "select (select count(distinct mt.siteid)  from mmm_mst_ManpowerTransData mt with(nolock) join mmm_mst_ManpowerMaster m with(nolock) on m.UserID=mt.IMIENO  where m.Role in ('Cluster Manager') "
        'Qry &= " and convert(date,ctime) >= convert(date,getdate()-10) AND convert(date,ctime) <= convert(date,getdate()))[MsiteID],CONVERT(VARCHAR(11), getdate()-10, 106) + ' To ' + CONVERT(VARCHAR(11), getdate(), 106)[Circle],'IMAP CM'[Role] union "
        'Qry &= " select (select count(distinct e.sitevisiteID) from MMM_MST_ELOGBOOK e with(nolock) join mmm_mst_VehicleMaster v with(nolock) on v.DeviceIMEI=e.IMEI_NO where convert(date,Trip_Start_DateTime ) >="
        'Qry &= " convert(date,getdate()-10) AND convert(date,Trip_Start_DateTime ) <= convert(date,getdate()) and v.VehicleType='CM' and e.sitevisiteID>0"
        'Qry &= " )[MsiteID],CONVERT(VARCHAR(11), getdate()-10, 106) + ' To ' + CONVERT(VARCHAR(11), getdate(), 106)[Circle],'VTS CM'[Role]"
        Dim ds As New DataSet
        Dim dttbl As New DataTable
        dttbl.Columns.Add("Type", GetType(String))
        dttbl.Rows.Add("IMAP CM")
        dttbl.Rows.Add("VTS CM")
        dttbl.Rows.Add("IMAP ZH")
        dttbl.Rows.Add("VTS ZH")

        Try

            Using da As New SqlDataAdapter(Qry, conStr)
                da.Fill(ds, "data")
                Dim dateqry As String = ""
                dateqry &= "Select distinct fld1[Circle] from mmm_mst_master where eid=54 and documenttype='circle'"

                da.SelectCommand.CommandText = dateqry
                da.Fill(ds, "circle")
                Dim slist As New List(Of seriesTKm)
                Dim c As New categoryAxisTkm
                Dim LST As New List(Of String)

                For i As Integer = 0 To dttbl.Rows.Count - 1
                    Dim s As New seriesTKm
                    s.name = dttbl.Rows(i).Item(0).ToString
                    Dim Ser As New List(Of Decimal)
                    For j = 0 To ds.Tables("circle").Rows.Count - 1
                        Dim dt As New DataTable
                        Dim sum As Decimal = 0
                        If dttbl.Rows(i).Item(0).ToString = "IMAP CM" Then
                            Dim drs = ds.Tables("data").Select("Circle='" & ds.Tables("circle").Rows(j).Item(0).ToString & "'  and role='IMAP CM'")
                            If drs.Length > 0 Then
                                dt = drs.CopyToDataTable()
                                sum = dt.Rows(0).Item("msiteid")
                                dt.Rows.Clear()
                            End If

                        ElseIf dttbl.Rows(i).Item(0).ToString = "VTS CM" Then
                            Dim drs = ds.Tables("data").Select("Circle='" & ds.Tables("circle").Rows(j).Item(0).ToString & "'  and role='VTS CM'")
                            If drs.Length > 0 Then
                                dt = drs.CopyToDataTable()
                                sum = dt.Rows(0).Item("msiteid")
                                dt.Rows.Clear()
                            End If

                        ElseIf dttbl.Rows(i).Item(0).ToString = "IMAP ZH" Then
                            Dim drs = ds.Tables("data").Select("Circle='" & ds.Tables("circle").Rows(j).Item(0).ToString & "' and role='IMAP ZH'")
                            If drs.Length > 0 Then
                                dt = drs.CopyToDataTable()
                                sum = dt.Rows(0).Item("msiteid")
                                dt.Rows.Clear()
                            End If
                        ElseIf dttbl.Rows(i).Item(0).ToString = "VTS ZH" Then
                            Dim drs = ds.Tables("data").Select("Circle='" & ds.Tables("circle").Rows(j).Item(0).ToString & "'  and role='VTS ZH'")
                            If drs.Length > 0 Then
                                dt = drs.CopyToDataTable()
                                sum = dt.Rows(0).Item("msiteid")
                                dt.Rows.Clear()
                            End If
                        End If
                        Ser.Add(sum)
                        If i = 0 Then

                            Dim ciritem As String = ds.Tables("circle").Rows(j).Item(0).ToString
                            If ciritem = "MAHARASHTRA AND GOA" Then
                                ciritem = "M &G"
                            End If
                            If ciritem = "Andhra Pradesh" Then
                                ciritem = "AP"
                            End If
                            If ciritem = "Delhi NCR" Then
                                ciritem = "Delhi"
                            End If
                            LST.Add(ciritem)

                        End If
                    Next
                    s.data = Ser
                    slist.Add(s)
                Next

                c.categories = LST
                vtm.series = slist
                vtm.cata = c
                vtm.success = True
            End Using

        Catch ex As Exception
            vtm.success = False
            vtm.msg = "Error"
        Finally
            ds.Dispose()
        End Try
        Return vtm
    End Function
    <WebMethod()> _
      <Script.Services.ScriptMethod()> _
    Public Shared Function DVTS() As VehicleTKM
        Dim vtm As New VehicleTKM
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim Qry As String = "SELECT circle[Circle],dtscount[Count],role[Role] from  DVTScircle"

        'Qry &= " select CONVERT(VARCHAR(11), getdate()-30, 106) + ' To ' + CONVERT(VARCHAR(11), getdate()-21, 106)[Circle],count(distinct sitevisiteid)[Count],'VTS'[Role] from MMM_MST_ELOGBOOK e with(nolock) join mmm_mst_VehicleMaster v"
        'Qry &= " on v.DeviceIMEI=e.IMEI_NO where VehicleType='diesel filing' and convert(date,trip_start_datetime)>=convert(date,getdate()-30) and "
        'Qry &= " convert(date,trip_start_datetime)<=convert(date,getdate()-21) and TripType='auto'"
        Dim ds As New DataSet
        Dim dttbl As New DataTable
        dttbl.Columns.Add("Type", GetType(String))
        dttbl.Rows.Add("DTS")
        dttbl.Rows.Add("VTS")
        Try

            Using da As New SqlDataAdapter(Qry, conStr)
                da.Fill(ds, "data")
                Dim dateqry As String = ""
                dateqry &= "Select distinct fld1[Circle] from mmm_mst_master where eid=54 and documenttype='circle'"

                da.SelectCommand.CommandText = dateqry
                da.Fill(ds, "circle")
                Dim slist As New List(Of seriesTKm)
                Dim c As New categoryAxisTkm
                Dim LST As New List(Of String)

                For i As Integer = 0 To dttbl.Rows.Count - 1
                    Dim s As New seriesTKm
                    s.name = dttbl.Rows(i).Item(0).ToString
                    Dim Ser As New List(Of Decimal)
                    For j = 0 To ds.Tables("circle").Rows.Count - 1
                        Dim dt As New DataTable
                        Dim sum As Decimal = 0
                        If dttbl.Rows(i).Item(0).ToString = "DTS" Then
                            Dim drs = ds.Tables("data").Select("Circle='" & ds.Tables("circle").Rows(j).Item(0).ToString & "'  and role='DTS'")
                            If drs.Length > 0 Then
                                dt = drs.CopyToDataTable()
                                sum = dt.Rows(0).Item("count")
                                dt.Rows.Clear()
                            End If

                        ElseIf dttbl.Rows(i).Item(0).ToString = "VTS" Then
                            Dim drs = ds.Tables("data").Select("Circle='" & ds.Tables("circle").Rows(j).Item(0).ToString & "'  and role='VTS'")
                            If drs.Length > 0 Then
                                dt = drs.CopyToDataTable()
                                sum = dt.Rows(0).Item("count")
                                dt.Rows.Clear()
                            End If
                        End If

                        Ser.Add(sum)
                        If i = 0 Then

                            Dim ciritem As String = ds.Tables("circle").Rows(j).Item(0).ToString
                            If ciritem = "MAHARASHTRA AND GOA" Then
                                ciritem = "M &G"
                            End If
                            If ciritem = "Andhra Pradesh" Then
                                ciritem = "AP"
                            End If
                            If ciritem = "Delhi NCR" Then
                                ciritem = "Delhi"
                            End If
                            LST.Add(ciritem)

                        End If
                    Next
                    s.data = Ser
                    slist.Add(s)
                Next

                c.categories = LST
                vtm.series = slist
                vtm.cata = c
                vtm.success = True
            End Using

        Catch ex As Exception
            vtm.success = False
            vtm.msg = "Error"
        Finally
            ds.Dispose()
        End Try
        Return vtm
    End Function
    Public Class VehicleTKM
        Public Property success As Boolean
        Public Property msg As String
        Public Property series As Object
        Public Property cata As Object
    End Class
    Public Class seriesTKm
        Public Property name As String
        Public Property data As List(Of Decimal)
    End Class
    Public Class categoryAxisTkm
        Public Property categories As List(Of String)
    End Class

End Class