Imports System.Collections.Generic
Imports System.Linq
Imports System.Web
Imports System.Web.Services
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Data.SqlClient
Imports System.Data
Imports System.Configuration
Imports System.Web.UI.Adapters.ControlAdapter
Imports System.Drawing
Imports System.Threading
Imports System
Imports System.Collections.Specialized
Imports System.Text
Imports System.Net.Security
Imports System.IO
Imports Newtonsoft.Json.Converters
Imports Newtonsoft.Json
Imports System.Web.Script.Serialization
Imports System.Web.Script.Services
Imports iTextSharp.text.pdf

Partial Class FieldSurvey
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


    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load

    End Sub

    <WebMethod()>
    Public Shared Function GetDSlip(d1 As String, d2 As String) As String
        Dim grid As New DGrid()
        Dim strError = ""
        Dim jsonData As String = ""
        Try
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As SqlConnection = New SqlConnection(conStr)
            Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
            Dim dt As New DataTable
            Dim qry As String = ""

            If d1 = "" Or d2 = "" Then
                qry = "select dms.udf_split('MASTER-Store Master-fld8',fld7)[StoreName],fld8[Distributor],fld9[Zone],"
                qry &= " fld15[DocomoFPP],fld16[DocomoPP],((convert(numeric(10,2),case when isnumeric(fld15)>0 then fld15 else '0' end)+convert(numeric(10,2),case when isnumeric(fld16)>0 then fld16 else '0' end))/2)[DocomoVI], "
                qry &= "fld19[VodafoneFPP],fld20[VodafonePP],((convert(numeric(10,2),case when isnumeric(fld19)>0 then fld19 else '0' end)+convert(numeric(10,2),case when isnumeric(fld20)>0 then fld20 else '0' end))/2)[VodafoneVI],"
                qry &= "fld13[AirtelFPP],fld14[AirtelPP],((convert(numeric(10,2),case when isnumeric(fld13)>0 then fld13 else '0' end)+convert(numeric(10,2),case when isnumeric(fld14)>0 then fld14 else '0' end))/2)[AirtelVI],"
                qry &= "fld17[IDEAFPP],fld18[IDEAPP],((convert(numeric(10,2),case when isnumeric(fld17)>0 then fld17 else '0' end)+convert(numeric(10,2),case when isnumeric(fld18)>0 then fld18 else '0' end))/2)[IDEAVI],"
                qry &= "fld21[OtherFPP],fld22[OtherPP],((convert(numeric(10,2),case when isnumeric(fld21)>0 then fld21 else '0' end)+convert(numeric(10,2),case when isnumeric(fld22)>0 then fld22 else '0' end))/2)[OtherVI]"
                qry &= "FROM MMM_MST_doc WITH(NOLOCK) WHERE EID=106 and documenttype='Survey Document' and fld7<>'' order by fld7 "

            Else
                qry = "select dms.udf_split('MASTER-Store Master-fld8',fld7)[StoreName],fld8[Distributor],fld9[Zone],"
                qry &= " fld15[DocomoFPP],fld16[DocomoPP],((convert(numeric(10,2),case when isnumeric(fld15)>0 then fld15 else '0' end)+convert(numeric(10,2),case when isnumeric(fld16)>0 then fld16 else '0' end))/2)[DocomoVI], "
                qry &= "fld19[VodafoneFPP],fld20[VodafonePP],((convert(numeric(10,2),case when isnumeric(fld19)>0 then fld19 else '0' end)+convert(numeric(10,2),case when isnumeric(fld20)>0 then fld20 else '0' end))/2)[VodafoneVI],"
                qry &= "fld13[AirtelFPP],fld14[AirtelPP],((convert(numeric(10,2),case when isnumeric(fld13)>0 then fld13 else '0' end)+convert(numeric(10,2),case when isnumeric(fld14)>0 then fld14 else '0' end))/2)[AirtelVI],"
                qry &= "fld17[IDEAFPP],fld18[IDEAPP],((convert(numeric(10,2),case when isnumeric(fld17)>0 then fld17 else '0' end)+convert(numeric(10,2),case when isnumeric(fld18)>0 then fld18 else '0' end))/2)[IDEAVI],"
                qry &= "fld21[OtherFPP],fld22[OtherPP],((convert(numeric(10,2),case when isnumeric(fld21)>0 then fld21 else '0' end)+convert(numeric(10,2),case when isnumeric(fld22)>0 then fld22 else '0' end))/2)[OtherVI]"
                qry &= " FROM MMM_MST_doc WITH(NOLOCK) WHERE EID=106 and documenttype='Survey Document' and convert(date,adate)>='" & d1 & "' and convert(date,adate)<='" & d2 & "'  order by fld7 "
            End If


            oda.SelectCommand.CommandText = qry
            Dim ds As New DataSet()

            Try

                Try
                    oda.SelectCommand.CommandTimeout = 300
                    oda.Fill(ds, "data")

                    Dim serializerSettings As New JsonSerializerSettings()
                    Dim json_serializer As New JavaScriptSerializer()
                    serializerSettings.Converters.Add(New DataTableConverter())
                    jsonData = JsonConvert.SerializeObject(ds.Tables(0), Newtonsoft.Json.Formatting.None, serializerSettings)
                Catch exption As Exception
                    
                End Try
            Catch ex As Exception
              
            Finally
                con.Close()
                oda.Dispose()
                con.Dispose()
            End Try
        Catch ex As Exception

        End Try
        Return jsonData
    End Function


End Class
