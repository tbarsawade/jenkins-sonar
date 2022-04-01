Imports System.Web.UI.Adapters.ControlAdapter
Imports iTextSharp.text
Imports System.Drawing
Imports iTextSharp.text.pdf
Imports System.IO
Imports System.Threading
Imports System.Net.Mail
Imports System.Net
Imports System.Net.HttpWebRequest
Imports System.Net.HttpWebResponse
Imports System
Imports System.Collections.Specialized
Imports System.Text
Imports System.Net.Security
Imports System.IO.Stream
Imports System.Web.Hosting
Imports iTextSharp.text.html.simpleparser
Imports System.Web.Services
Imports System.Data.SqlClient
Imports System.Data

Partial Class VendorPODtlTGBL
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
        If Not IsPostBack Then

        End If
    End Sub

    <WebMethod()>
    Public Shared Function GetData() As DGrid
        Dim grid As New DGrid()
        Dim strError = ""
        Try
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As New SqlConnection(conStr)
            Dim da As New SqlDataAdapter("", con)
            Dim dl As New DataTable
            da.SelectCommand.CommandType = CommandType.Text
            da.SelectCommand.CommandText = "select fld2[PO No.],fld6[PO Date],dms.udf_split('MASTER-Plant Master-fld1',fld7) [Plant] , fld17[PO Value without Tax],fld20 [Total Tax Value] from MMM_MST_doc WITH(NOLOCK) where  EID=" & HttpContext.Current.Session("EID").ToString() & " and documenttype='purchase order'  and fld30 in ('" & HttpContext.Current.Session("UID").ToString() & "')"
            Dim dt As New DataTable
            da.Fill(dt)
            Try
                Try
                    If dt.Rows.Count = 0 Then
                        grid.Success = False
                        grid.Message = "No data found."
                    Else
                        grid = DynamicGrid.GridData(dt, strError)
                    End If
                Catch exption As Exception
                    grid.Success = False
                    grid.Message = "Dear User please enter valid data."
                End Try
            Catch ex As Exception
                grid.Success = False
                grid.Message = "No data found."
            Finally
                con.Close()
                da.Dispose()
                con.Dispose()
            End Try
        Catch ex As Exception

        End Try
        Return grid
    End Function
    <WebMethod()> _
    Public Shared Function getDtl(Pnum As String) As DGrid
        Dim jsonData As String = ""
        Dim grid As New DGrid()
        Try

            Dim ds As New DataSet()
            Dim Query As String = ""
            Query = "select dms.udf_split('DOCUMENT-Purchase Order-fld2',d.fld2)[PO No.],d.fld11[Invoice No.],d.fld22[Invoice Date],dms.udf_split('MASTER-Plant Master-fld1',d.fld6)[Plant],d.fld33[Invoice Amount without Tax],"
            Query &= "case d.curstatus when 'REJECTED' THEN 'Rejected / Cancelled' when 'UPLOADED' THEN 'Pending with Submitter' else d.curstatus end [Invoice Status in PEARL]"
            Query &= " from mmm_mst_doc d where EID=" & HttpContext.Current.Session("EID").ToString() & " and documenttype='Vendor Invoice' and dms.udf_split('DOCUMENT-Purchase Order-fld2',d.fld2)='" & Pnum & "'"

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
End Class


