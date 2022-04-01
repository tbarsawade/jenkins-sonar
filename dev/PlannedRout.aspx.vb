Imports System.Xml
Imports System.IO
Imports System.Net
Imports System.Web.Services
Imports System.Data
Imports System.Data.SqlClient
Imports System.Web.Script.Serialization

Partial Class PlannedRout
    Inherits System.Web.UI.Page
    Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
    Dim con As SqlConnection = New SqlConnection(conStr)
    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        Session("UID") = 6955 '6231
        Session("USERNAME") = "Prashant Singh Sengar"
        Session("USERROLE") = "SU"
        Session("CODE") = "PAL"
        Session("USERIMAGE") = "2.jpg"
        Session("CLOGO") = "hfcl.png"
        Session("EID") = 61
        Session("ISLOCAL") = "TRUE"
        Session("IPADDRESS") = "Vinay"
        Session("MACADDRESS") = "Vinay"
        Session("INTIME") = Now
        Session("EMAIL") = "vinay.kumar@myndsol.com"
        Session("LID") = "25"
        Session("HEADERSTRIP") = "hfclstrip.jpg"
        Session("ROLES") = "SU"

        If Not IsPostBack Then
            BindDdl()
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
        Dim da As New SqlDataAdapter("select Tid [Value], fld1 [Text]  from mmm_mst_Doc where documenttype='Route Planning' and Eid=" & Session("Eid"), con)
        Dim dt As New DataTable
        da.Fill(dt)
        ddlRoute.DataValueField = "Value"
        ddlRoute.DataTextField = "Text"
        ddlRoute.DataSource = dt
        ddlRoute.DataBind()
        ddlRoute.Items.Insert(0, "Select")
    End Sub

    <WebMethod()> _
  <Script.Services.ScriptMethod()> _
    Public Shared Function GetLocations(Tid As Integer) As String
        Dim conStr1 As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con1 As SqlConnection = New SqlConnection(conStr1)
        Dim CsvStr As New StringBuilder()
        Dim qry = "Select Tid, fld1[Source], fld10[Destination], fld11[Time], fld12[Sequence], fld13[ETA] from  mmm_mst_Doc_Item where DocID in (" & Tid & ") order by fld12"
        Dim da As New SqlDataAdapter(qry, con1)
        Dim dt As New DataTable()
        da.Fill(dt)

        Dim gisObj As New GisMethods()
        For i As Integer = 0 To dt.Rows.Count - 1
            CsvStr.Append(dt.Rows(i).Item("Tid").ToString() & "^")
            CsvStr.Append(dt.Rows(i).Item("Source").ToString() & "^")
            CsvStr.Append(dt.Rows(i).Item("Destination").ToString() & "^")
            CsvStr.Append(dt.Rows(i).Item("Time").ToString() & "^")
            CsvStr.Append(dt.Rows(i).Item("Sequence").ToString() & "^")
            CsvStr.Append(dt.Rows(i).Item("ETA").ToString() & "^")
            Dim Latt = ""
            Dim Longt = ""
            Dim ob = gisObj.GoogleGeoCodeFreeText(dt.Rows(i).Item("Source").ToString())
            If ob.Success Then
                Latt = ob.Latt
                Longt = ob.Longt
            End If
            CsvStr.Append(Latt & "^")
            CsvStr.Append(Longt & "^")
            CsvStr.Append("|")
        Next
        Return CsvStr.ToString()
    End Function
End Class
