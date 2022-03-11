Imports System.Data
Imports System.Data.SqlClient
Imports System.Configuration
Imports System.Globalization
Imports System.IO
Imports System.Collections
Imports System.Web.Services

Partial Class NGroupMap
    Inherits System.Web.UI.Page
    Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
    Dim con As SqlConnection = New SqlConnection(conStr)
    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load

        ''for PAL
        'Session("UID") = 2042 '6231
        'Session("USERNAME") = "Prashant Singh Sengar"
        'Session("USERROLE") = "SU"
        'Session("CODE") = "PAL"
        'Session("USERIMAGE") = "2.jpg"
        'Session("CLOGO") = "hfcl.png"
        'Session("EID") = 43
        'Session("ISLOCAL") = "TRUE"
        'Session("IPADDRESS") = "Vinay"
        'Session("MACADDRESS") = "Vinay"
        'Session("INTIME") = Now
        'Session("EMAIL") = "vinay.kumar@myndsol.com"
        'Session("LID") = "25"
        'Session("HEADERSTRIP") = "hfclstrip.jpg"
        'Session("ROLES") = "SU"

        If Not IsPostBack Then
            Bindddl()
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
    Public Sub Bindddl()
        Dim da As New SqlDataAdapter("Select Distinct GroupName from mmm_mst_GroupMapSettings where Eid=" & Session("Eid"), con)
        Dim dt As New DataTable
        da.Fill(dt)
        ddlGroup.DataValueField = "GroupName"
        ddlGroup.DataTextField = "GroupName"
        ddlGroup.DataSource = dt
        ddlGroup.DataBind()
        ddlGroup.Items.Insert(0, "Select")
    End Sub

    <WebMethod()> _
  <Script.Services.ScriptMethod()> _
    Public Shared Function GetMarkers(GroupName As String) As String
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim CsvStr As New StringBuilder()
        Dim CsvIcons As New StringBuilder()

        Dim da As New SqlDataAdapter("Select * from mmm_mst_GroupMapSettings where GroupName='" & GroupName & "' and Eid=" & HttpContext.Current.Session("Eid"), con)
        Dim dt As New DataTable
        da.Fill(dt)
        For i As Integer = 0 To dt.Rows.Count - 1

            CsvIcons.Append(dt.Rows(i).Item("IconName") + ":")

            Dim qry = "Select * from mmm_mst_fields where Eid=" & HttpContext.Current.Session("Eid") & " and documenttype='" & dt.Rows(i).Item("DocType") & "' and isActive=1"
            da.SelectCommand.CommandText = qry
            Dim dtFields As New DataTable
            da.Fill(dtFields)
            Dim result() As DataRow = dtFields.Select("fieldtype='geo point'")
            Dim geoPoinFldMapping As String = result(0).Item("FieldMapping").ToString()
            qry = "Select Tid, DocumentType, " & geoPoinFldMapping & "[LatLong] from mmm_mst_Master "
            qry &= " where Eid=" & HttpContext.Current.Session("Eid") & " and documenttype='" & dt.Rows(i).Item("DocType") & "' and nullif(" & geoPoinFldMapping & ",'') is not null and " & geoPoinFldMapping & " <>'0'"

            da.SelectCommand.CommandText = qry
            Dim dtDocGeoPonts As New DataTable
            da.Fill(dtDocGeoPonts)
            For j As Integer = 0 To dtDocGeoPonts.Rows.Count - 1
                Dim arr = dtDocGeoPonts.Rows(j).Item("LatLong").ToString().Split(",")
                If arr.Length < 2 Then
                    Continue For
                ElseIf Not IsNumeric(arr(0)) Or Not IsNumeric(arr(1)) Then
                    Continue For
                End If
                CsvStr.Append(dtDocGeoPonts.Rows(j).Item("Tid").ToString() + "^")
                CsvStr.Append(dtDocGeoPonts.Rows(j).Item("DocumentType").ToString() + "^")
                CsvStr.Append(arr(0) + "^")
                CsvStr.Append(arr(1) + "^")
                CsvStr.Append(dt.Rows(i).Item("IconName") + "^")
                CsvStr.Append(result(0).Item("FieldId").ToString() + "|")
            Next
        Next

        Return CsvStr.ToString() & "=" & CsvIcons.ToString()

    End Function

    <WebMethod()> _
 <Script.Services.ScriptMethod()> _
    Public Shared Function GetInfo(Tid As Integer, Doc As String, FldId As Integer, GroupName As String) As String
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim InfoStr As New StringBuilder()
        Dim dtInfo As New DataTable
        Dim da As New SqlDataAdapter("Select * from mmm_mst_GroupMapSettings where Eid=" & HttpContext.Current.Session("Eid") & " and DocType='" & Doc & "' and GroupName='" & GroupName & "' ", con)
        Dim dtQry As New DataTable
        da.Fill(dtQry)
        If dtQry.Rows.Count = 0 Then
            Return "No information found."
        Else
            Dim strQry = IIf(IsDBNull(dtQry.Rows(0)("InfoQuery")), "", dtQry.Rows(0)("InfoQuery").ToString())
            If strQry.Trim() = "" Then
                'Return "No information found."
            Else
                strQry = Replace(strQry, "@Tid", Tid)
                strQry = Replace(strQry, "@tid", Tid)
                da.SelectCommand.CommandText = strQry
                da.Fill(dtInfo)
                If dtInfo.Rows.Count = 0 Then
                    Return "No information found."
                End If
                For i As Integer = 0 To dtInfo.Columns.Count - 1
                    InfoStr.Append("<b>" & dtInfo.Columns(i).ColumnName & "</b> : " & dtInfo.Rows(0).Item(dtInfo.Columns(i).ColumnName) & "<br>")
                Next
                Return InfoStr.ToString()
            End If
        End If


        da.SelectCommand.CommandText = "Select Kc_value from mmm_mst_fields where Fieldid=" & FldId
        Dim dt As New DataTable
        da.Fill(dt)
        If dt.Rows.Count = 0 Then
            Return "No information found."
        ElseIf dt.Rows(0)("Kc_value").ToString.Trim() = "" Then
            Return "No information found."
        End If
        Dim strFields As String() = dt.Rows(0).Item("Kc_value").ToString().Split(",")

        Dim InfoQry = ""
        Dim usr As String = ""
        For i As Integer = 0 To strFields.Length - 1
            Dim qry = "Select * from mmm_mst_Fields where Eid=" & HttpContext.Current.Session("Eid") & " and DocumentType='" & Doc & "' and FieldMapping='" & strFields(i) & "' and isActive=1 "
            Dim DtRow As New DataTable
            da.SelectCommand.CommandText = qry
            da.Fill(DtRow)
            If DtRow.Rows.Count = 0 Then
                Continue For
            End If
            If DtRow.Rows(0).Item("DropDownType").ToString = "MASTER VALUED" Then
                InfoQry &= "dms.udf_split('" & DtRow.Rows(0).Item("DropDown") & "'," & strFields(i) & ")[" & DtRow.Rows(0).Item("displayName") & "],"
            ElseIf DtRow.Rows(0).Item("DropDownType").ToString = "SESSION VALUED" Then
                ' InfoQry &= "dms.udf_split('" & DtRow.Rows(0).Item("DropDown") & "'," & strFields(i) & ")[" & DtRow.Rows(0).Item("displayName") & "],"
            Else
                InfoQry &= strFields(i) & "[" & DtRow.Rows(0).Item("displayName") & "],"
            End If
        Next
        InfoQry = InfoQry.Remove(InfoQry.LastIndexOf(","))
        da.SelectCommand.CommandText = "Select " & InfoQry & " from mmm_mst_master where Tid=" & Tid

        da.Fill(dtInfo)
        Dim strImg = ""
        For i As Integer = 0 To dtInfo.Columns.Count - 1
            If dtInfo.Columns(i).ColumnName.ToString() = "File Uploader" Then
                strImg &= "<img src='docs/" & dtInfo.Rows(0).Item(dtInfo.Columns(i).ColumnName) & "'  width='100%' height='200px' /><br>"
            Else
                InfoStr.Append("<b>" & dtInfo.Columns(i).ColumnName & "</b> : " & dtInfo.Rows(0).Item(dtInfo.Columns(i).ColumnName) & "<br>")
            End If
        Next
        Return InfoStr.ToString()

    End Function
End Class
