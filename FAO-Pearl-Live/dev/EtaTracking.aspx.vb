Imports System.Xml
Imports System.IO
Imports System.Net
Imports System.Web.Services
Imports System.Data
Imports System.Data.SqlClient
Imports System.Web.Script.Serialization
Imports Newtonsoft.Json.Linq


Partial Class EtaTracking
    Inherits System.Web.UI.Page
    Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
    Dim con As SqlConnection = New SqlConnection(conStr)

    Shared dtSettings As New DataTable()

    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            Dim qry = "Select * from mmm_mst_RoutePlanSettings where Eid=" & Session("Eid") & " and IsActive=1"
            Dim da As New SqlDataAdapter(qry, con)
            da.Fill(dtSettings)
            If dtSettings.Rows.Count > 0 Then
                BindDdl()
            Else
                lblMsg.Text = "Settings not found..!"
                lblMsg.ForeColor = Drawing.Color.Red
            End If

        End If
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
    Private Sub BindDdl()
        Dim qry As String = ""
        Dim ds As New DataSet()
        Dim da As New SqlDataAdapter("", con)
        Try
            Dim _vhDoc As String = dtSettings.Rows(0).Item("VehicleDoc")
            Dim _vhnofld As String = dtSettings.Rows(0).Item("VehicleNofld")
            Dim _vhImeifld As String = dtSettings.Rows(0).Item("VehicleIMEIfld")
            Dim _VhRoleDefDoc As String = dtSettings.Rows(0).Item("RoleDefDoc")
            Dim _vhRoleDefDocMapping As String = dtSettings.Rows(0).Item("VehicleRoleDefDocMap")

            qry = "Select * from mmm_mst_Forms where FormName='" & _VhRoleDefDoc & "' and Eid=" & Session("Eid")
            da.SelectCommand.CommandText = qry
            da.Fill(ds, "RoleDefDoc")

            Dim str As String = ""
            If Not HttpContext.Current.Session("USERROLE").ToString.ToUpper = "SU" Then
                qry = "Select uid,isnull(" & ds.Tables("RoleDefDoc").Rows(0).Item("DocMapping").ToString & ",0)[" & _VhRoleDefDoc & "] from mmm_ref_role_user where eid=" & Session("Eid") & " and uid=" & Session("Uid") & " and rolename='" & Session("USERROLE") & "'"
                da.SelectCommand.CommandText = qry
                da.Fill(ds, "RoleDef")

                If ds.Tables("RoleDef").Rows.Count = 0 Then
                    Exit Sub
                End If
                Dim ar = ds.Tables("RoleDef").Rows(0).Item(_VhRoleDefDoc).ToString.Split(",")

                For i As Integer = 0 To ar.Length - 1
                    str &= "'" & ar(i) & "'" & IIf(i = ar.Length - 1, "", ",")
                Next
            Else

            End If
            qry = "Select Tid [Value], " & _vhnofld & "[Text] from mmm_mst_Master where documenttype='" & _vhDoc & "' and eid=" & Session("Eid") & " and " & _vhImeifld & "<>'' and " & _vhImeifld & "<>'0' "
            If Not HttpContext.Current.Session("USERROLE").ToString.ToUpper = "SU" Then
                qry &= " and " & _vhRoleDefDocMapping & " In (" & str & ")"
            End If
            da.SelectCommand.CommandText = qry
            da.Fill(ds, "Vehicles")

            ddlVehicle.DataValueField = "Value"
            ddlVehicle.DataTextField = "Text"
            ddlVehicle.DataSource = ds.Tables("Vehicles")
            ddlVehicle.DataBind()
            ddlVehicle.Items.Insert(0, "Select")
            ddlVehicle.Items(0).Value = 0
        Catch ex As Exception

        Finally
            ds.Dispose()
            da.Dispose()
        End Try
    End Sub

    <WebMethod()> _
  <Script.Services.ScriptMethod()> _
    Public Shared Function GetLocations(Tid As Integer) As EtaResponce
        Dim conStr1 As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con1 As SqlConnection = New SqlConnection(conStr1)
        Dim Eid = HttpContext.Current.Session("Eid").ToString
        Dim ds As New DataSet()
        Dim EtaResponce As New EtaResponce()
        Dim lstPLans As New List(Of EtaHeader)
        Try

            Dim _RoutePlanDoc As String = dtSettings.Rows(0).Item("RoutePlanDoc")
            Dim _PlanVehicleNofld As String = dtSettings.Rows(0).Item("PlanVehicleNofld")
            Dim _PlanVehicleTypefld As String = dtSettings.Rows(0).Item("PlanVehicleTypefld")
            Dim _SiteDoc As String = dtSettings.Rows(0).Item("SiteDoc")
            Dim _SiteNamefld As String = dtSettings.Rows(0).Item("SiteNamefld")
            Dim _Destinationfld As String = dtSettings.Rows(0).Item("Destinationfld")
            Dim _Datefld As String = dtSettings.Rows(0).Item("Datefld")
            Dim _Timefld As String = dtSettings.Rows(0).Item("Timefld")
            Dim _SiteLatLongfld As String = dtSettings.Rows(0).Item("SiteLatLongfld")
            Dim _IMEIfld As String = dtSettings.Rows(0).Item("VehicleIMEIfld")
            Dim _HaultDurationfld As String = dtSettings.Rows(0).Item("HaultDurationFld")


            Dim qry = "Select " & _PlanVehicleNofld & ", " & _IMEIfld & " from mmm_mst_Master where Eid=" & HttpContext.Current.Session("Eid") & " and Tid=" & Tid
            Dim da As New SqlDataAdapter(qry, con1)
            da.Fill(ds, "Veh")
            qry = "Select top 1 Lattitude, longitude from mmm_mst_GPSData where IMIENO='" & ds.Tables("Veh").Rows(0).Item(_IMEIfld) & "' order by Tid desc"
            da.SelectCommand.CommandText = qry
            da.Fill(ds, "VehGps")
            EtaResponce.VehLocation = ds.Tables("VehGps").Rows(0).Item("Lattitude") & "," & ds.Tables("VehGps").Rows(0).Item("longitude")
            qry = "Select * from mmm_mst_Doc where documenttype='" & _RoutePlanDoc & "' and Eid=" & Eid & " and " & _PlanVehicleNofld & "='" & Tid & "'"
            da.SelectCommand.CommandText = qry
            da.Fill(ds, "Doc")

            For i As Integer = 0 To ds.Tables("Doc").Rows.Count - 1

                qry = "Select count(*) from mmm_mst_Doc_Item d where d.IsAuth=1 and convert(datetime, d." & _Datefld & "+' '+d." & _Timefld & ",3) >=(Getdate()-1) and d.DocID=" & ds.Tables("Doc").Rows(i).Item("Tid")
                da.SelectCommand.CommandText = qry
                con1.Open()
                Dim count = DirectCast(da.SelectCommand.ExecuteScalar(), Integer)
                con1.Close()
                If count > 0 Then
                    qry = "Select m.Tid, m." & _Destinationfld & " DestinationId,dms.udf_Split('MASTER-" & _SiteDoc & "-" & _SiteNamefld & "', d." & _Destinationfld & ")[Destination], convert(varchar(17), convert(datetime, d." & _Datefld & "+' '+d." & _Timefld & ",3),113) PlannedDate, m." & _SiteLatLongfld & " LatLong,  d." & _HaultDurationfld & " Hault from mmm_mst_Doc_Item d left join mmm_mst_Master m on d." & _Destinationfld & "=m.tid where d.DocID=" & ds.Tables("Doc").Rows(i).Item("Tid")
                    qry &= " and d.IsAuth=1 order by convert(datetime, d." & _Datefld & "+' '+d." & _Timefld & ",3)"
                    'and convert(datetime, d." & _Datefld & "+' '+d." & _Timefld & ",3) >=(Getdate()-2)
                    da.SelectCommand.CommandText = qry
                    da.Fill(ds, "Plan_" & i.ToString)
                    Dim obEta As New EtaHeader()
                    obEta.DocId = ds.Tables("Doc").Rows(i).Item("Tid")
                    obEta.Plan = "Plan " & (i + 1).ToString
                    obEta.VehType = ds.Tables("Doc").Rows(i).Item(_PlanVehicleTypefld).ToString.ToLower()
                    If ds.Tables("Plan_" & i.ToString).Rows.Count > 0 Then
                        For j As Integer = 0 To ds.Tables("Plan_" & i.ToString).Rows.Count - 1
                            Dim obEtaDetail As New EtaDetail()
                            obEtaDetail.Tid = ds.Tables("Plan_" & i.ToString).Rows(j).Item("Tid")
                            obEtaDetail.DestinationId = ds.Tables("Plan_" & i.ToString).Rows(j).Item("DestinationId")
                            obEtaDetail.Destination = ds.Tables("Plan_" & i.ToString).Rows(j).Item("Destination")
                            obEtaDetail.PlannedDate = ds.Tables("Plan_" & i.ToString).Rows(j).Item("PlannedDate")
                            obEtaDetail.LetLong = ds.Tables("Plan_" & i.ToString).Rows(j).Item("LatLong")
                            obEtaDetail.Hault = ds.Tables("Plan_" & i.ToString).Rows(j).Item("Hault")
                            obEta.DocDetail.Add(obEtaDetail)
                            obEta.Plan = Convert.ToDateTime(obEtaDetail.PlannedDate).ToString("dd-MM-yyyy")
                        Next
                        lstPLans.Add(obEta)
                    End If
                End If
            Next

            EtaResponce.Result = lstPLans
            EtaResponce.Success = True

        Catch ex As Exception

        End Try
        Return EtaResponce
    End Function

    <WebMethod()> _
 <Script.Services.ScriptMethod()> _
    Public Shared Function GetETA(Source As String, Destination As String, VehType As String, mode As String, traffic As String) As Eta
        Dim ResponseMatrix As Response = Nothing
        Dim ReturnObj As New Eta()
        Try
            Dim StartPoints As New StringBuilder()
            Dim DestinationPoints As New StringBuilder()

            StartPoints.Append("&start0=" + Source)
            DestinationPoints.Append("&destination0=" + Destination)

            Dim url As String = "http://route.st.nlp.nokia.com/routing/6.2/calculatematrix.json?app_id=DemoAppId01082013GAL&app_code=AJKnXv84fjrb0KIHawS0Tg&mode=" & mode & ";" & VehType & ";traffic:" & traffic & DestinationPoints.ToString & StartPoints.ToString & ""
            Dim request As System.Net.WebRequest = WebRequest.Create(url)
            Dim response As HttpWebResponse = request.GetResponse()
            If response.StatusCode = HttpStatusCode.OK Then
                Dim reader As StreamReader = New StreamReader(response.GetResponseStream())
                Dim rawresp As String
                rawresp = reader.ReadToEnd()
                Dim jResults As JObject = JObject.Parse(rawresp)
                Dim jo = JObject.Parse(jResults.ToString())
                Dim ser As New System.Web.Script.Serialization.JavaScriptSerializer
                ResponseMatrix = ser.Deserialize(Of Response)(jo("Response").ToString())

                Dim time As Double = Convert.ToDouble(ResponseMatrix.MatrixEntry(0).Route.Summary.BaseTime) / 60
                Dim dist As Double = Convert.ToDouble(ResponseMatrix.MatrixEntry(0).Route.Summary.Distance) / 1000

                Dim str = time.ToString.Split(".")
                Dim strTime = ""

                Dim Dec As String = "" 'IIf(str(1).Length >= 2, str(1).Substring(0, 2), str(1))
                If str(1).Length >= 2 Then
                    Dec = str(1).Substring(0, 2)
                Else
                    Dec = str(1).ToString()
                End If

                If Convert.ToInt32(Dec) Mod 60 > 0 Then
                    strTime = (Convert.ToInt32(str(0)) + Convert.ToInt32(Convert.ToInt32(Dec) / 60)).ToString & "." & (Convert.ToInt32(Dec) Mod 60).ToString()
                Else
                    strTime = str(0) & ":" & str(1)
                End If

                ReturnObj.Time = strTime & " Min(s)" ' time.ToString("F2") & " Min(s)"
                ReturnObj.Distance = dist.ToString("F2") & " KM(s)"
                ReturnObj.Success = True
                Return ReturnObj
            End If
        Catch ex As Exception
            Return ReturnObj
        End Try
        Return ReturnObj
    End Function

    <WebMethod()> _
    <Script.Services.ScriptMethod()> _
    Public Shared Function RefreshVehicle(Tid As Integer) As String
        Dim conStr1 As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con1 As SqlConnection = New SqlConnection(conStr1)
        Dim CsvStr As New StringBuilder()
        Dim ds As New DataSet()
        Dim Eid = HttpContext.Current.Session("Eid").ToString
        Dim _PlanVehicleNofld As String = dtSettings.Rows(0).Item("PlanVehicleNofld")
        Dim _IMEIfld As String = dtSettings.Rows(0).Item("VehicleIMEIfld")

        Dim qry = "Select " & _PlanVehicleNofld & ", " & _IMEIfld & " from mmm_mst_Master where Eid=" & HttpContext.Current.Session("Eid") & " and Tid=" & Tid
        Dim da As New SqlDataAdapter(qry, con1)
        da.Fill(ds, "Veh")
        qry = "Select top 1 Lattitude, longitude from mmm_mst_GPSData where IMIENO='" & ds.Tables("Veh").Rows(0).Item(_IMEIfld) & "' order by Tid desc"
        da.SelectCommand.CommandText = qry
        da.Fill(ds, "VehGps")
        CsvStr.Append(ds.Tables("VehGps").Rows(0).Item("Lattitude") & "," & ds.Tables("VehGps").Rows(0).Item("longitude"))
        Return CsvStr.ToString()
    End Function

    <WebMethod()> _
  <Script.Services.ScriptMethod()> _
    Public Shared Function GetEtaViaWaypoints(wayPointsArr() As String, Haults() As Integer, VehType As String, mode As String, traffic As String) As Eta
        Dim retObj As New Eta()
        Try
            Dim sources As New StringBuilder()
            Dim Destinations As New StringBuilder()
            For i As Integer = 0 To wayPointsArr.Length - 1
                Dim arr = wayPointsArr(i).Split(",")
                sources.Append("&start" & i.ToString & "=" & arr(0) & "," & arr(1))
                Destinations.Append("&destination" & i.ToString & "=" & arr(0) & "," & arr(1))
            Next
            Dim iList = GisMethods.GetHereDistanceTimeMatrix(sources.ToString, Destinations.ToString, VehType, mode, traffic)
            Dim iList1 = iList
            Dim dsPrev As New DistanceMatrix()
            dsPrev.StartIndex = 0
            dsPrev.DestinationIndex = 0
            Dim startPoint = 1

            Dim time As Double
            Dim dist As Double
            For i As Integer = 0 To wayPointsArr.Length - 1
                Dim result As List(Of DistanceMatrix)
                Dim c = i
                result = iList.FindAll(Function(p As DistanceMatrix) p.StartIndex = c And p.DestinationIndex = (c + 1))
                If result.Count > 0 Then
                    time = time + Convert.ToDouble(result(0).BaseTime)
                    dist = dist + Convert.ToDouble(result(0).Distance)
                End If
            Next
            For i As Integer = 0 To Haults.Length - 1
                time = time + Convert.ToDouble(Haults(i))
            Next

            time = time / 60
            dist = dist / 1000


            Dim str = time.ToString.Split(".")
            Dim strTime = ""
            Dim Dec As String = ""
            If str.Length > 1 Then
                If str(1).Length >= 2 Then
                    Dec = str(1).Substring(0, 2)
                Else
                    Dec = str(1).ToString()
                End If
            Else
                Dec = "0"
            End If
            
            If Convert.ToInt32(Dec) Mod 60 > 0 Then
                strTime = (Convert.ToInt32(str(0)) + Convert.ToInt32(Convert.ToInt32(Dec) / 60)).ToString & "." & (Convert.ToInt32(Dec) Mod 60).ToString()
            Else
                If Dec = "0" Then
                    strTime = str(0) & ":00"
                Else
                    strTime = str(0) & ":" & str(1)
                End If

            End If

            retObj.Time = strTime & " Min(s)"
            retObj.Distance = dist.ToString("F2") & " KM(s)"
            retObj.Success = True

        Catch ex As Exception
            retObj.Success = False
        End Try
        Return retObj
    End Function

End Class

Public Class Eta
    Public Property Success As Boolean
    Public Property Distance As String
    Public Property Time As String
End Class

Public Class EtaResponce
    Public Property Success As Boolean
    Public Property VehLocation As String
    Public Property Result As New List(Of EtaHeader)
End Class

Public Class EtaHeader
    Public Property DocId As Integer
    Public Property Plan As String
    Public Property VehType As String
    Public Property DocDetail As New List(Of EtaDetail)
End Class

Public Class EtaDetail
    Public Property Tid As Integer
    Public Property DestinationId As Integer
    Public Property Destination As String
    Public Property PlannedDate As String
    Public Property Hault As String
    Public Property LetLong As String
End Class