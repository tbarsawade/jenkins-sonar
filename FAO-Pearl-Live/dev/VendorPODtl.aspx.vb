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

Partial Class VendorPODtl
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
            Dim objDC As New DataClass()
            Dim documenttype As String = "Purchase Order"
            Dim eid As Int32 = HttpContext.Current.Session("EID")
            documenttype = objDC.ExecuteQryScaller("select top 1 documenttype from mmm_mst_venpoinv where eid=" & eid & " and RenderLevel=1")

            'Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            'Dim con As New SqlConnection(conStr)
            'Dim da As New SqlDataAdapter("", con)
            'Dim dl As New DataTable
            'da.SelectCommand.CommandType = CommandType.Text
            'da.SelectCommand.CommandText = "select fld2[PO No.],fld6[PO Date],dms.udf_split('MASTER-Plant Master-fld1',fld7) [Plant] , fld17[PO Value without Tax],fld20 [Total Tax Value] from MMM_MST_doc WITH(NOLOCK) where  EID=" & HttpContext.Current.Session("EID").ToString() & " and documenttype='purchase order'  and fld30 in ('" & HttpContext.Current.Session("UID").ToString() & "')"
            Dim dt As New DataTable
            'da.Fill(dt)
            Dim arrFields As New ArrayList()
            Dim arrFilterFields As New ArrayList()
            Dim objDTFields As New DataTable()

            Dim query As String = ""
            objDTFields = objDC.ExecuteQryDT("select * from  MMM_MST_VENPOINV where eid=" & eid & " and documenttype='" & documenttype & "'")
            If objDTFields.Rows.Count > 0 Then
                For Each dr As DataRow In objDTFields.Rows
                    arrFields.Add(dr("fieldMapping") & " as [" & dr("ReportDisplayName") & "]")
                Next
                If arrFields.Count > 0 Then
                    query = "select " & String.Join(",", arrFields.ToArray()) & "  from mmm_mst_doc  where documenttype='" & documenttype & "' and eid=" & eid
                End If
                Dim objDTFilterFields As New DataTable()
                objDTFilterFields = objDC.ExecuteQryDT("select * from mmm_mst_venpoinv_filter where documenttype='" & documenttype & "' and eid=" & eid & "")
                For Each drFilterFields As DataRow In objDTFilterFields.Rows
                    Dim CurrentValue As String = ""
                    If Convert.ToString(drFilterFields("filterdatavalue")).Contains("SESSION") Then
                        CurrentValue = drFilterFields("filterdatavalue").ToString().ToUpper.Replace("SESSION[", "").Replace("]", "")
                        CurrentValue = HttpContext.Current.Session("" & CurrentValue & "")
                    Else
                        CurrentValue = Convert.ToString(drFilterFields("filterdatavalue"))
                    End If
                    arrFilterFields.Add(drFilterFields("fieldmapping") & "='" & CurrentValue & "'")
                Next
                If arrFilterFields.Count > 0 Then
                    query &= " and " & String.Join(" and ", arrFilterFields.ToArray())
                End If
                dt = objDC.ExecuteQryDT(query)
            End If
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
                'con.Close()
                'da.Dispose()
                'con.Dispose()
            End Try
        Catch ex As Exception

        End Try
        Return grid
    End Function
    <WebMethod()>
    Public Shared Function getDtl(Pnum As String) As DGrid
        Dim jsonData As String = ""
        Dim grid As New DGrid()
        Try

            'Dim ds As New DataSet()
            Dim dt As New DataTable()
            Dim Query As String = ""
            Dim documenttype = "Vendor Invoice"
            Dim eid = HttpContext.Current.Session("EID")
            Dim objDC As New DataClass()
            Dim arrFields As New ArrayList()
            Dim arrFilterFields As New ArrayList()
            Dim objDTFields As New DataTable()
            documenttype = objDC.ExecuteQryScaller("select top 1 documenttype from mmm_mst_venpoinv where eid=" & eid & " and RenderLevel=2")
            objDTFields = objDC.ExecuteQryDT("select * from  MMM_MST_VENPOINV where eid=" & eid & " and documenttype='" & documenttype & "'")
            If objDTFields.Rows.Count > 0 Then
                For Each dr As DataRow In objDTFields.Rows
                    arrFields.Add(dr("fieldMapping") & " as [" & dr("ReportDisplayName") & "]")
                Next
                If arrFields.Count > 0 Then
                    Query = "select " & String.Join(",", arrFields.ToArray()) & "  from mmm_mst_doc  where documenttype='" & documenttype & "' and eid=" & eid
                End If
                Dim objDTFilterFields As New DataTable()
                objDTFilterFields = objDC.ExecuteQryDT("select * from mmm_mst_venpoinv_filter where documenttype='" & documenttype & "' and eid=" & eid & "")
                For Each drFilterFields As DataRow In objDTFilterFields.Rows
                    Dim CurrentValue As String = ""
                    If Convert.ToString(drFilterFields("filterdatavalue")).Contains("SESSION") Then
                        CurrentValue = drFilterFields("filterdatavalue").ToString().ToUpper.Replace("SESSION[", "").Replace("]", "")
                        CurrentValue = HttpContext.Current.Session("" & CurrentValue & "")
                    ElseIf Convert.ToString(drFilterFields("filterdatavalue")).Contains("Pnum") Then
                        CurrentValue = Pnum
                    Else
                        CurrentValue = Convert.ToString(drFilterFields("filterdatavalue"))
                    End If
                    arrFilterFields.Add(drFilterFields("fieldmapping") & "='" & CurrentValue & "'")
                Next
                If arrFilterFields.Count > 0 Then
                    Query &= " and " & String.Join(" and ", arrFilterFields.ToArray())
                End If
                dt = objDC.ExecuteQryDT(Query)
            End If
            'Query = "Select dms.udf_split('DOCUMENT-Purchase Order-fld2',d.fld2)[PO No.],d.fld11[Invoice No.],d.fld22[Invoice Date],dms.udf_split('MASTER-Plant Master-fld1',d.fld6)[Plant],d.fld33[Invoice Amount without Tax],"
            'Query &= "case d.curstatus when 'REJECTED' THEN 'Rejected / Cancelled' when 'UPLOADED' THEN 'Pending with Submitter' else d.curstatus end [Invoice Status in PEARL]"
            'Query &= " from mmm_mst_doc d where EID=" & HttpContext.Current.Session("EID").ToString() & " and documenttype='Vendor Invoice' and dms.udf_split('DOCUMENT-Purchase Order-fld2',d.fld2)='" & Pnum & "'"

            'Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            'Using con As New SqlConnection(conStr)
            '    Using da As New SqlDataAdapter(Query, con)
            '        da.Fill(ds)
            '    End Using
            'End Using

            Dim strError = ""
            grid = DynamicGrid.GridData(dt, strError)
            If dt.Rows.Count = 0 Then
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


