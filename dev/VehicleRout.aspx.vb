Imports System.Xml
Imports System.IO
Imports System.Net
Imports System.Web.Services
Imports System.Data
Imports System.Data.SqlClient
Imports System.Web.Script.Serialization

Partial Class VehicleRout
    Inherits System.Web.UI.Page
    Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
    Dim con As SqlConnection = New SqlConnection(conStr)
    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        'Session("UID") = 6955 '6231
        'Session("USERNAME") = "Prashant Singh Sengar"
        'Session("USERROLE") = "SU"
        'Session("CODE") = "PAL"
        'Session("USERIMAGE") = "2.jpg"
        'Session("CLOGO") = "hfcl.png"
        'Session("EID") = 61
        'Session("ISLOCAL") = "TRUE"
        'Session("IPADDRESS") = "Vinay"
        'Session("MACADDRESS") = "Vinay"
        'Session("INTIME") = Now
        'Session("EMAIL") = "vinay.kumar@myndsol.com"
        'Session("LID") = "25"
        'Session("HEADERSTRIP") = "hfclstrip.jpg"
        'Session("ROLES") = "SU"

        If Not IsPostBack Then
            BindDdl()
        End If

    End Sub
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
        Dim da As New SqlDataAdapter("select Tid [Value], fld1 + ' [ '+fld10+' ]' [Text]  from mmm_mst_master where documenttype='Vehicle Master' and Eid=" & Session("Eid"), con)
        Dim dt As New DataTable
        da.Fill(dt)
        ddlVehicle.DataValueField = "Value"
        ddlVehicle.DataTextField = "Text"
        ddlVehicle.DataSource = dt
        ddlVehicle.DataBind()
        ddlVehicle.Items.Insert(0, "Select")
    End Sub

    <WebMethod()> _
  <Script.Services.ScriptMethod()> _
    Public Shared Function GetLocations(Tid As Integer) As String
        Dim conStr1 As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con1 As SqlConnection = New SqlConnection(conStr1)
        Dim CsvStr As New StringBuilder()
        Dim startPoint As String = "0^0^"
        Dim Qry As String = ";with cte as(select Tid, fld1[Vehicle_Name], fld10[Vehicle_No.], fld11[Driver_Name], fld12[Vehicle_Contact_No.], fld13[Starting_Point], fld14[Seating_Capacity]"
        Qry += " from mmm_mst_master where Tid=" & Tid & " and documenttype='Vehicle Master' and Eid=" & HttpContext.Current.Session("Eid") & " and IsAuth=1 ),"
        Qry += " cte1 as(Select Tid, fld1[Site_Name], fld10[Address], fld11[City], fld12[State], fld14[Location], fld16[Vehicle_No.]"
        Qry += " from mmm_mst_master where documenttype='Pick and Drop Point' and Eid=" & HttpContext.Current.Session("Eid") & " and IsAuth=1) "
        Qry += " select Row_Number() over(order by (Select 1)), cte1.Tid[SiteId], cte.[Vehicle_No.],[Driver_Name],[Vehicle_Contact_No.],[Starting_Point],[Seating_Capacity],"
        Qry += " [Site_Name],[Address] + ', '+[City]+ ', '+[State]  as Address,[Location]"
        Qry += " from cte join cte1 on cte.Tid=cte1.[Vehicle_No.]"
        Dim da As New SqlDataAdapter(Qry, con1)
        Dim dt As New DataTable()
        da.Fill(dt)

        Dim StartPoints As New StringBuilder()
        Dim DestinationPoints As New StringBuilder()

        StartPoints.Append("&start0=" + dt.Rows(0).Item("Starting_Point").ToString())
        DestinationPoints.Append("&destination0=" + dt.Rows(0).Item("Starting_Point").ToString())

        For i As Integer = 0 To dt.Rows.Count - 1
            StartPoints.Append("&start" + (i + 1).ToString() + "=" + dt.Rows(i).Item("Location").ToString())
            DestinationPoints.Append("&destination" + (i + 1).ToString() + "=" + dt.Rows(i).Item("Location").ToString())
        Next

        Dim matrix = GisMethods.HereDistanceMatrix(StartPoints.ToString(), DestinationPoints.ToString(), dt.Rows.Count)

        For i As Integer = 0 To dt.Rows.Count - 1
            For j As Integer = 0 To matrix.DistancePath.Count - 1
                If matrix.DistancePath.Item(j).DestinationIndex = (i + 1) Then
                    Dim SiteInfo As New SiteInfo()
                    SiteInfo.SiteId = dt.Rows(i).Item("SiteId")
                    SiteInfo.SiteName = dt.Rows(i).Item("Site_Name")
                    SiteInfo.Address = dt.Rows(i).Item("Address")
                    SiteInfo.Latt = Convert.ToDouble(dt.Rows(i).Item("Location").ToString().Split(",")(0))
                    SiteInfo.Longt = Convert.ToDouble(dt.Rows(i).Item("Location").ToString().Split(",")(1))
                    matrix.DistancePath.Item(j).AdditionalData = SiteInfo
                End If
            Next
        Next

        For i As Integer = 0 To dt.Rows.Count - 1
            For j As Integer = 0 To matrix.TimePath.Count - 1
                If matrix.TimePath.Item(j).DestinationIndex = (i + 1) Then
                    Dim SiteInfo As New SiteInfo()
                    SiteInfo.SiteId = dt.Rows(i).Item("SiteId")
                    SiteInfo.SiteName = dt.Rows(i).Item("Site_Name")
                    SiteInfo.Address = dt.Rows(i).Item("Address")
                    SiteInfo.Latt = Convert.ToDouble(dt.Rows(i).Item("Location").ToString().Split(",")(0))
                    SiteInfo.Longt = Convert.ToDouble(dt.Rows(i).Item("Location").ToString().Split(",")(1))
                    matrix.TimePath.Item(j).AdditionalData = SiteInfo
                End If
            Next
        Next

        matrix.StartLatt = Convert.ToDouble(dt.Rows(0).Item("Starting_Point").ToString().Split(",")(0))
        matrix.StartLongt = Convert.ToDouble(dt.Rows(0).Item("Starting_Point").ToString().Split(",")(1))
        Dim js As New JavaScriptSerializer()
        Return js.Serialize(matrix)

    End Function

End Class
Public Class SiteInfo
    Public Property SiteId As Integer
    Public Property SiteName As String
    Public Property Address As String
    Public Property Latt As Double
    Public Property Longt As Double
End Class
