Imports System.Data
Imports System.Data.SqlClient
Partial Class PublicView

    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
        Dim ds As New DataSet
        oda.SelectCommand.Parameters.Clear()
        oda.SelectCommand.CommandType = CommandType.StoredProcedure
        oda.SelectCommand.CommandText = "USP_GETPUBLICVIEW_FIELDS"
        oda.SelectCommand.Parameters.AddWithValue("EID", 34)
        oda.SelectCommand.Parameters.AddWithValue("DOCUMENTTYPE", "PUBLIC FORM")
        Dim dt1 As DataTable = New DataTable("Public")
        oda.Fill(dt1)
        Dim dt As DataTable = New DataTable("Temp")
        Dim layoutdata As String = ""


        For Each dc As DataColumn In dt1.Columns
            dt.Columns.Add(dc.ColumnName)
        Next
        'Dim drnew As DataRow = dt.NewRow
       
        For Each DR As DataRow In dt1.Rows
            layoutdata = DR.Item("layoutdata").ToString()
            Dim drnew As DataRow = dt.NewRow
            For i As Integer = 0 To dt1.Columns.Count - 1
                Dim fld As String = "{" & dt1.Columns(i).ColumnName & "}"
                layoutdata = layoutdata.Replace(fld, DR.Item(i).ToString)
                drnew(i) = DR.Item(i).ToString()
            Next
            drnew.Item("layoutdata") = layoutdata
            dt.Rows.Add(drnew)
        Next
            repeater1.DataSource = dt
            repeater1.DataBind()
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
End Class
