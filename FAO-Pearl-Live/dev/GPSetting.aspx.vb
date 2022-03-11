Imports System.Data
Imports System.Data.SqlClient
Partial Class GPSetting
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then

            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As SqlConnection = New SqlConnection(conStr)
            Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
            oda.SelectCommand.CommandText = "select * from  MMM_MST_FORMS where FormSource='MENU DRIVEN'  and Eid=" & Session("EID") & " order by FormName"
            Dim ds As New DataSet()
            oda.Fill(ds, "data")
            ddlDocumentType.Items.Clear()
            ddRatecarddoc.Items.Clear()
            ddlCustomerDoc.Items.Clear()
            'Dim viewform As New StringBuilder()
            'viewform.
            For j As Integer = 0 To ds.Tables("data").Rows.Count - 1
                Dim view As String = String.Empty
                view = ds.Tables("data").Rows(j).Item("formname").ToString.ToUpper
                view = view.Replace(" ", "_")
                view = "V" + Session("eid").ToString + view + ""
                ddlDocumentType.Items.Add(ds.Tables("data").Rows(j).Item("FormName").ToString().ToUpper())
     
                ddlDocumentType.Items(j).Value = view
                ddRatecarddoc.Items.Add(ds.Tables("data").Rows(j).Item("formname").ToString.ToUpper)
                ddRatecarddoc.Items(j).Value = view
                ddlCustomerDoc.Items.Add(ds.Tables("data").Rows(j).Item("formname").ToString.ToUpper)
                ddlCustomerDoc.Items(j).Value = view

            Next


            oda.SelectCommand.CommandText = "select * FROM mmm_mst_gpssetting WHERE  EID= " & Session("EID") & ""
            Dim dt As New DataTable
            oda.Fill(dt)
            ddlDocumentType.SelectedIndex = ddlDocumentType.Items.IndexOf(ddlDocumentType.Items.FindByValue(dt.Rows(0).Item("Cab_Vehicle_doc").ToString.ToUpper))
            ddRatecarddoc.SelectedIndex = ddRatecarddoc.Items.IndexOf(ddRatecarddoc.Items.FindByValue(dt.Rows(0).Item("Cab_rate_card_doc").ToString.ToUpper))
            ddlCustomerDoc.SelectedIndex = ddlCustomerDoc.Items.IndexOf(ddlCustomerDoc.Items.FindByValue(dt.Rows(0).Item("Cab_Customer_doc").ToString.ToUpper))

            ' Cab_Vehicle_doc

            If dt.Rows(0).Item("Cab_Vehicle_doc").ToString.Length > 0 Then
                Dim docname As String = dt.Rows(0).Item("Cab_Vehicle_doc").ToString
                docname = docname.Replace("V" + Session("eid").ToString + "", "")
                docname = docname.Replace("_", " ")
                '  oda.SelectCommand.CommandText = "select DISTINCT displayname,fieldmapping FROM MMM_MST_FIELDS WHERE DOCUMENTTYPE= '" & dt.Rows(0).Item("Cab_Vehicle_doc").ToString & "' AND EID= " & Session("EID") & ""
                oda.SelectCommand.CommandText = "select DISTINCT displayname,fieldmapping FROM MMM_MST_FIELDS WHERE DOCUMENTTYPE= '" & docname & "' AND EID= " & Session("EID") & ""
                oda.Fill(ds, "datafieldname")
                ddstatus.Items.Clear()
                ddowner.Items.Clear()
                For j As Integer = 0 To ds.Tables("datafieldname").Rows.Count - 1
                    ddstatus.Items.Add(ds.Tables("datafieldname").Rows(j).Item("displayname").ToString().ToUpper())
                    ddstatus.Items(j).Value = ds.Tables("datafieldname").Rows(j).Item("displayname").ToString.ToUpper
                    ddowner.Items.Add(ds.Tables("datafieldname").Rows(j).Item("displayname").ToString.ToUpper)
                    ddVehicleType.Items.Add(ds.Tables("datafieldname").Rows(j).Item("displayname").ToString.ToUpper)
                Next
                If dt.Rows(0).Item("cab_vehicle_status").ToString.Length > 0 Then

                    ddstatus.SelectedIndex = ddstatus.Items.IndexOf(ddstatus.Items.FindByText(dt.Rows(0).Item("cab_vehicle_status").ToString.ToUpper))
                    ddowner.SelectedIndex = ddowner.Items.IndexOf(ddowner.Items.FindByText(dt.Rows(0).Item("cab_owner").ToString.ToUpper))
                    ddVehicleType.SelectedIndex = ddVehicleType.Items.IndexOf(ddVehicleType.Items.FindByText(dt.Rows(0).Item("vehicle_type").ToString.ToUpper))
                End If

            End If
            ds.Dispose()
            oda.Dispose()


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
    Protected Sub FilldropDown(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim constr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(constr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
        oda.SelectCommand.CommandText = "select DISTINCT displayname,fieldmapping FROM MMM_MST_FIELDS WHERE DOCUMENTTYPE= '" & ddlDocumentType.SelectedItem.Text & "' AND EID= " & Session("EID") & " order by displayname"
        Dim ds As New DataSet()
        oda.Fill(ds, "data")
        ddstatus.Items.Clear()
        For r As Integer = 0 To ds.Tables("data").Rows.Count - 1
            ddstatus.Items.Add(ds.Tables("data").Rows(r).Item("displayname").ToString.ToUpper)
            ddstatus.Items(r).Value = ds.Tables("data").Rows(r).Item("displayname").ToString.ToUpper
            'ddstatus.Items.Add(ds.Tables("data").Rows(r).Item("").ToString.ToUpper)

        Next
        oda.SelectCommand.CommandText = "select * from  MMM_MST_FORMS where FormSource='MENU DRIVEN'  and Eid=" & Session("EID") & " order by FormName"
        oda.Fill(ds, "docname")
        ddRatecarddoc.Items.Clear()
        For r As Integer = 0 To ds.Tables("docname").Rows.Count - 1
            ddRatecarddoc.Items.Add(ds.Tables("docname").Rows(r).Item("FormName").ToString.ToUpper)
            ddRatecarddoc.Items(r).Value = ds.Tables("docname").Rows(r).Item("FormName").ToString.ToUpper
            'ddstatus.Items.Add(ds.Tables("data").Rows(r).Item("").ToString.ToUpper)

        Next

        ddlCustomerDoc.Items.Clear()
        For r As Integer = 0 To ds.Tables("docname").Rows.Count - 1
            ddlCustomerDoc.Items.Add(ds.Tables("docname").Rows(r).Item("FormName").ToString.ToUpper)
            ddlCustomerDoc.Items(r).Value = ds.Tables("docname").Rows(r).Item("FormName").ToString.ToUpper
            'ddstatus.Items.Add(ds.Tables("data").Rows(r).Item("").ToString.ToUpper)

        Next
        ddowner.Items.Clear()
        For r As Integer = 0 To ds.Tables("data").Rows.Count - 1
            ddowner.Items.Add(ds.Tables("data").Rows(r).Item("displayname").ToString.ToUpper)
            ddowner.Items(r).Value = ds.Tables("data").Rows(r).Item("displayname").ToString.ToUpper
            'ddstatus.Items.Add(ds.Tables("data").Rows(r).Item("").ToString.ToUpper)

        Next


        ddVehicleType.Items.Clear()
        For r As Integer = 0 To ds.Tables("data").Rows.Count - 1
            ddVehicleType.Items.Add(ds.Tables("data").Rows(r).Item("displayname").ToString.ToUpper)
            ddVehicleType.Items(r).Value = ds.Tables("data").Rows(r).Item("displayname").ToString.ToUpper
            'ddstatus.Items.Add(ds.Tables("data").Rows(r).Item("").ToString.ToUpper)
        Next


        UpdatePanel1.Update()

    End Sub

    Protected Sub btnUname_Click(sender As Object, e As System.EventArgs) Handles btnUname.Click
        Dim constr As String = ConfigurationManager.ConnectionStrings("constr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(constr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
        oda.SelectCommand.CommandText = "update mmm_mst_gpssetting set Cab_Vehicle_doc='" & ddlDocumentType.SelectedItem.Value & "',cab_vehicle_status='" & ddstatus.SelectedItem.Value & "',cab_owner='" & ddowner.SelectedItem.Value & "',Cab_rate_card_doc='" & ddRatecarddoc.SelectedItem.Value & "',Cab_Customer_doc='" & ddlCustomerDoc.SelectedItem.Value & "',Vehicle_Type='" + ddVehicleType.SelectedItem.Value + "'  where eid=" & Session("eid").ToString & "  "
        If con.State <> ConnectionState.Open Then
            con.Open()
        End If
        oda.SelectCommand.ExecuteNonQuery()
        savedvehiclemapping.Text = "GPS  Mapping has been saved"
        UpdatePanel1.Update()
        con.Close()
        oda.Dispose()

    End Sub
End Class
