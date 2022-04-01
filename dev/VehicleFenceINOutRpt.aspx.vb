Imports System.Data.SqlClient
Imports System.Data
Imports System.Web.Services
Imports Newtonsoft.Json
Imports System.Web.Script.Serialization
Imports Newtonsoft.Json.Converters

Partial Class VehicleFenceINOutRpt
    Inherits System.Web.UI.Page
    Dim ConStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
    Dim con As SqlConnection = New SqlConnection(ConStr)

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
        Try
            If Not IsPostBack Then
                FillVehicleList()
            End If
        Catch ex As Exception

        End Try
    End Sub

    Sub FillVehicleList()
        Try
            Dim da As New SqlDataAdapter("", ConStr)
            Dim dt As New DataTable
            da.SelectCommand.CommandText = "select distinct Vehicle_NO + ' (' + IMEI_NO+')' [Vehicle_NO], IMEI_NO from mmm_mst_elogbook where Eid=" & Session("Eid")
            da.Fill(dt)
            If dt.Rows.Count > 0 Then

                UsrVeh.DataSource = dt
                UsrVeh.DataTextField = "Vehicle_NO"
                UsrVeh.DataValueField = "IMEI_NO"
                UsrVeh.DataBind()

            End If

        Catch ex As Exception
        End Try

    End Sub

    Protected Sub checkuncheck(sender As Object, e As System.EventArgs) Handles CheckBox1.CheckedChanged
        Try

            If CheckBox1.Checked = True Then

                For Each chkitem As System.Web.UI.WebControls.ListItem In UsrVeh.Items
                    chkitem.Selected = True
                Next

            Else
                For Each chkitem As System.Web.UI.WebControls.ListItem In UsrVeh.Items
                    chkitem.Selected = False
                Next

            End If
        Catch ex As Exception

        End Try
    End Sub
    <WebMethod()> _
    Public Shared Function FillGrid(Sdate As String, EDate As String, Vehicles As String, EID As String) As String
        Dim ret = ""
        Try
            Dim ConStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim da As New SqlDataAdapter("", ConStr)
            Dim dt As New DataTable
            da.SelectCommand.CommandType = CommandType.StoredProcedure
            da.SelectCommand.CommandText = "Sp_VehicleINOUTFenceRpt"
            da.SelectCommand.Parameters.AddWithValue("@Eid", Convert.ToInt32(EID))
            da.SelectCommand.Parameters.AddWithValue("@SDate", Sdate)
            da.SelectCommand.Parameters.AddWithValue("@EDate", EDate)
            da.SelectCommand.Parameters.AddWithValue("@VehicleIMEIs", Vehicles)
            da.SelectCommand.CommandTimeout = 500
            da.Fill(dt)

            If dt.Rows.Count > 0 Then
                For j As Integer = 0 To dt.Columns.Count - 1
                    dt.Columns(j).ColumnName = dt.Columns(j).ColumnName.Replace(" ", "_")
                Next
            End If

            Dim serializerSettings As New JsonSerializerSettings()
            Dim json_serializer As New JavaScriptSerializer()
            serializerSettings.Converters.Add(New DataTableConverter())
            Dim jsonData As [String] = JsonConvert.SerializeObject(dt, Formatting.None, serializerSettings)
            ret = jsonData

        Catch ex As Exception

        End Try
        Return ret
    End Function

    Protected Sub btnshow_Click(sender As Object, e As ImageClickEventArgs)
        Try
            If (Validatecontrols()) Then
                '  FillGrid()
            End If
        Catch ex As Exception

        End Try
    End Sub

    Function Validatecontrols() As Boolean
        Dim check As Boolean = True

        Try

            If (txtsdate.Text = "") Then
                check = False
                lblmsg.Text = "Please select From Date"
                Return check
            ElseIf (Convert.ToDateTime(txtsdate.Text).Date > DateTime.Now.Date) Then
                check = False
                lblmsg.Text = "Future dates are not allowed"
                Return check
            ElseIf ((DateTime.Now.Date - Convert.ToDateTime(txtsdate.Text).Date).Days > 31) Then
                check = False
                lblmsg.Text = "Report for post 1 Month (31 days) can only be retrived"
                Return check
            ElseIf (txtedate.Text = "") Then
                check = False
                lblmsg.Text = "Please select To Date"
                Return check
            ElseIf (Convert.ToDateTime(txtsdate.Text).Date > Convert.ToDateTime(txtedate.Text).Date) Then
                check = False
                lblmsg.Text = "From Date cannot be greater than To Date"
                Return check
            ElseIf (UsrVeh.SelectedValue = "") Then
                check = False
                lblmsg.Text = "Please select atleast one Vehicle."
                Return check
            End If

        Catch ex As Exception

        End Try

        lblmsg.Text = ""
        Return check
    End Function

   

End Class
