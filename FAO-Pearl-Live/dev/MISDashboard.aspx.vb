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
Imports Ionic.Zip
Imports Microsoft.Office.Interop
Imports System.Web.Hosting

Partial Class MISDashboard
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            ' session("eid") = "0"
            Try
                Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
                Dim con As New SqlConnection(conStr)
                'fill Product 
                If Session("USERROLE").ToString.ToUpper = "SU" Then
                Else
                    Dim da As New SqlDataAdapter("select FormName,DocMapping from mmm_mst_forms where DocMapping is not null and eid=" & Session("EID") & "", con)
                    Dim ds As New DataSet
                    da.Fill(ds, "data")

                    da.SelectCommand.CommandText = "select " & ds.Tables("data").Rows(0).Item("DocMapping") & " from MMM_Ref_Role_User where eid=" & Session("EID") & " and uid=" & Session("UID") & " and rolename in ('" & Session("USERROLE") & "') "
                    Dim id As String = ""
                    If con.State <> ConnectionState.Open Then
                        con.Open()
                    End If
                    da.SelectCommand.CommandType = CommandType.Text
                    id = da.SelectCommand.ExecuteScalar().ToString
                    Dim str As String = ""
                    da.SelectCommand.CommandText = "select FieldMapping,documenttype from MMM_MST_FIELDS where eid=" & Session("EID") & " and dropdown like '%" & ds.Tables("data").Rows(0).Item(0).ToString & "%' and documenttype in (select distinct formname from mmm_mst_forms where eid=" & Session("EID") & " and formtype='document' and formsource='menu driven' and isactive=1)"
                    da.Fill(ds, "data1")
                    If ds.Tables("data1").Rows.Count > 0 Then
                        For i As Integer = 0 To ds.Tables("data1").Rows.Count - 1
                            str = str & " when documenttype='" & ds.Tables("data1").Rows(i).Item("documenttype").ToString & "' then " & ds.Tables("data1").Rows(i).Item("FieldMapping").ToString & ""
                        Next
                    End If
                    str = " and (case " & str & " end  in (" & id.ToString & "))"
                    Session("FilterRole") = str
                    da.Dispose()
                    con.Dispose()
                End If
            Catch ex As Exception

            End Try
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
    

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Shared Function GetDataExpenseBreakup(Type As String, Dtf As String, Sdate As String, Edate As String) As String
        Dim jsonData As String = ""
        Try
            Dim ds As New DataSet()
            Dim UID As Integer = 0
            Dim URole As String = ""
            Dim qry As String = ""
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As New SqlConnection(conStr)
            Dim dt As New DataTable
            Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
            If (Type.ToUpper = "DEPARTMENT") Then
                qry = "SELECT rootqry_D from mmm_mst_misdb where eid=" & HttpContext.Current.Session("EID") & " and DBName='Expense Breakup'"
            Else
                qry = "SELECT rootquery_e from mmm_mst_misdb where eid=" & HttpContext.Current.Session("EID") & " and DBName='Expense Breakup'"
            End If

            oda.SelectCommand.CommandType = CommandType.Text
            oda.SelectCommand.CommandText = qry
            oda.Fill(ds, "qry")
            If ds.Tables("qry").Rows.Count > 0 Then
                Dim str = ds.Tables("qry").Rows(0).Item(0).ToString
                If Dtf.ToUpper = "CURRENT FY" Then
                    str = str.Replace("@Date", "convert(date,adate)>=(dateadd(year, datepart(year, getdate()-89) - 1900, '1900-04-01')) and convert(date,adate)<=(dateadd(day, -1, dateadd(month, 12, (dateadd(year, datepart(year, getdate()-89) - 1900, '1900-04-01')))))")
                ElseIf Dtf.ToUpper = "LAST FY" Then
                    str = str.Replace("@Date", "convert(date,adate)>=dateadd(year, -1, (dateadd(year, datepart(year, getdate()-89) - 1900, '1900-04-01'))) and convert(date,adate)<=dateadd(year, -1, (dateadd(day, -1, dateadd(month, 12, (dateadd(year, datepart(year, getdate()-89) - 1900, '1900-04-01'))))))")
                ElseIf Dtf.ToUpper = "LAST MONTH" Then
                    str = str.Replace("@Date", "convert(date,adate)>=(DATEADD(mm, DATEDIFF(mm, 0, GETDATE())-1, 0)) and convert(date,adate)<=(DATEADD(ms, -3, DATEADD(mm, DATEDIFF(m, 0, GETDATE()), 0)))")
                ElseIf Dtf.ToUpper = "CURRENT MONTH" Then
                    str = str.Replace("@Date", "convert(date,adate)>=(DATEADD(mm, DATEDIFF(mm, 0, GETDATE()), 0)) and convert(date,adate)<=(DATEADD(ms, -3, DATEADD(mm, DATEDIFF(m, 0, GETDATE())+1, 0)))")
                ElseIf Dtf.ToUpper = "CURRENT QUARTER" Then
                    str = str.Replace("@Date", "convert(date,adate)>=DATEADD(qq, DATEDIFF(qq, 0, GETDATE()), 0) and convert(date,adate)<=DATEADD (dd, -1, DATEADD(qq, DATEDIFF(qq, 0, GETDATE()) +1, 0))")
                ElseIf Dtf.ToUpper = "LAST QUARTER" Then
                    str = str.Replace("@Date", "convert(date,adate)>=DATEADD(qq, DATEDIFF(qq, 0, GETDATE()) - 1, 0) and convert(date,adate)<=DATEADD (dd, -1, DATEADD(qq, DATEDIFF(qq, 0, GETDATE()), 0))")
                Else
                    str = str.Replace("@Date", "convert(date,adate)>=convert(date,'" & Sdate.ToString & "')  and convert(date,adate)<=convert(date,'" & Edate.ToString & "')")
                End If
                If HttpContext.Current.Session("USERROLE").ToString.ToUpper = "SU" Then
                Else
                    str = str.Replace("eid=" & HttpContext.Current.Session("EID").ToString & "", "eid=" & HttpContext.Current.Session("EID").ToString & " " & HttpContext.Current.Session("FilterRole").ToString)
                End If
                oda.SelectCommand.CommandType = CommandType.Text
                oda.SelectCommand.CommandText = str '"select  top 5 dms.udf_split('MASTER-Doc Nature Master-fld1',fld4)[category],convert(numeric(10,2),sum(convert(numeric(10,2),fld37))/1000000)[value] from mmm_mst_doc where eid=152 and documenttype in ('invoice non po') and  convert(date,adate)>=(DATEADD(mm, DATEDIFF(mm, 0, GETDATE()), 0)) and convert(date,adate)<=(DATEADD(ms, -3, DATEADD(mm, DATEDIFF(m, 0, GETDATE()) + 1, 0))) and curstatus<>'rejected' group by dms.udf_split('MASTER-Doc Nature Master-fld1',fld4) order by sum(convert(numeric(10,2),fld37)) desc"

                oda.Fill(ds, "data")
                oda.Fill(dt)
                con.Close()
                dt.Dispose()
                Dim lstColumns As New List(Of String)
                Dim serializerSettings As New JsonSerializerSettings()
                Dim json_serializer As New JavaScriptSerializer()
                serializerSettings.Converters.Add(New DataTableConverter())
                jsonData = JsonConvert.SerializeObject(dt, Newtonsoft.Json.Formatting.None, serializerSettings)
            End If
        Catch Ex As Exception
            Throw
        End Try
        Return jsonData
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Shared Function GetDataSuppSpendBreakup(Dtf As String, Sdate As String, Edate As String) As String
        Dim jsonData As String = ""
        Try
            Dim ds As New DataSet()
            Dim UID As Integer = 0
            Dim URole As String = ""
            Dim qry As String = ""
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As New SqlConnection(conStr)
            Dim dt As New DataTable
            Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
            qry = "Select Rootqry_D from mmm_mst_MISDB where eid=" & HttpContext.Current.Session("EID") & " and DBName='Supplier Spend Breakup'"
            oda.SelectCommand.CommandType = CommandType.Text
            oda.SelectCommand.CommandText = qry
            oda.Fill(ds, "qry")
            If ds.Tables("qry").Rows.Count > 0 Then
                Dim str = ds.Tables("qry").Rows(0).Item(0).ToString
                If Dtf.ToUpper = "CURRENT FY" Then
                    str = str.Replace("@Date", "convert(date,adate)>=(dateadd(year, datepart(year, getdate()-89) - 1900, '1900-04-01')) and convert(date,adate)<=(dateadd(day, -1, dateadd(month, 12, (dateadd(year, datepart(year, getdate()-89) - 1900, '1900-04-01')))))")
                ElseIf Dtf.ToUpper = "LAST FY" Then
                    str = str.Replace("@Date", "convert(date,adate)>=dateadd(year, -1, (dateadd(year, datepart(year, getdate()-89) - 1900, '1900-04-01'))) and convert(date,adate)<=dateadd(year, -1, (dateadd(day, -1, dateadd(month, 12, (dateadd(year, datepart(year, getdate()-89) - 1900, '1900-04-01'))))))")
                ElseIf Dtf.ToUpper = "LAST MONTH" Then
                    str = str.Replace("@Date", "convert(date,adate)>=(DATEADD(mm, DATEDIFF(mm, 0, GETDATE())-1, 0)) and convert(date,adate)<=(DATEADD(ms, -3, DATEADD(mm, DATEDIFF(m, 0, GETDATE()), 0)))")
                ElseIf Dtf.ToUpper = "CURRENT MONTH" Then
                    str = str.Replace("@Date", "convert(date,adate)>=(DATEADD(mm, DATEDIFF(mm, 0, GETDATE()), 0)) and convert(date,adate)<=(DATEADD(ms, -3, DATEADD(mm, DATEDIFF(m, 0, GETDATE())+1, 0)))")
                ElseIf Dtf.ToUpper = "CURRENT QUARTER" Then
                    str = str.Replace("@Date", "convert(date,adate)>=DATEADD(qq, DATEDIFF(qq, 0, GETDATE()), 0) and convert(date,adate)<=DATEADD (dd, -1, DATEADD(qq, DATEDIFF(qq, 0, GETDATE()) +1, 0))")
                ElseIf Dtf.ToUpper = "LAST QUARTER" Then
                    str = str.Replace("@Date", "convert(date,adate)>=DATEADD(qq, DATEDIFF(qq, 0, GETDATE()) - 1, 0) and convert(date,adate)<=DATEADD (dd, -1, DATEADD(qq, DATEDIFF(qq, 0, GETDATE()), 0))")
                Else
                    str = str.Replace("@Date", "convert(date,adate)>=convert(date,'" & Sdate.ToString & "')  and convert(date,adate)<=convert(date,'" & Edate.ToString & "')")
                End If
                If HttpContext.Current.Session("USERROLE").ToString.ToUpper = "SU" Then
                Else
                    str = str.Replace("eid=" & HttpContext.Current.Session("EID").ToString & "", "eid=" & HttpContext.Current.Session("EID").ToString & " " & HttpContext.Current.Session("FilterRole").ToString)
                End If
                oda.SelectCommand.CommandType = CommandType.Text
                oda.SelectCommand.CommandText = str '"select  top 5 dms.udf_split('MASTER-Vendor Master-fld2',fld5)[category], case when max(documenttype)='invoice non po' then convert(numeric(10,2),sum(convert(numeric(10,2),fld37))/1000000) when max(documenttype)='invoice po' then convert(numeric(10,2),sum(convert(numeric(10,2),fld92))/1000000) end [value] from mmm_mst_doc where eid=152 and documenttype in ('invoice po','invoice non po','invoice on hold') and curstatus<>'rejected' group by dms.udf_split('MASTER-Vendor Master-fld2',fld5) order by [value] desc"
                oda.Fill(ds, "data")
                oda.Fill(dt)
                con.Close()
                dt.Dispose()
                Dim lstColumns As New List(Of String)
                Dim serializerSettings As New JsonSerializerSettings()
                Dim json_serializer As New JavaScriptSerializer()
                serializerSettings.Converters.Add(New DataTableConverter())
                jsonData = JsonConvert.SerializeObject(dt, Newtonsoft.Json.Formatting.None, serializerSettings)
            End If
        Catch Ex As Exception
            Throw
        End Try
        Return jsonData
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Shared Function GetDataSLAPerformance(Type As String, Dtf As String, Status As String, Sdate As String, Edate As String) As vdashboard
        Dim ds As New DataSet
        Dim dt As New DataTable
        Dim Res As New vdashboard()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Try
            Using oda As New SqlDataAdapter("", conStr)
                Dim qry As String = ""

                qry = "SELECT rootqry_D from mmm_mst_misdb where eid=" & HttpContext.Current.Session("EID") & " and DBName='SLA Performance'"

                oda.SelectCommand.CommandType = CommandType.Text
                oda.SelectCommand.CommandText = qry
                oda.Fill(ds, "qry")
                If ds.Tables("qry").Rows.Count > 0 Then
                    Dim str As String = ds.Tables("qry").Rows(0).Item(0).ToString  '"select 'Finance'[category],12[0-15 Days],20[16-30 Days],16[31-45 Days],6[>45 Days] union select 'Human Resource'[Category],10[0-5 Days],12[16-30 Days],13[31-45 Days],9[>45 Days] union select 'Administration'[Category],8[0-5 Days],22[16-30 Days],10[31-45 Days],11[>45 Days] union select 'Sales and Marketing'[Category],12[0-5 Days],18[16-30 Days],7[31-45 Days],9[>45 Days] union select 'Others'[Category],17[0-5 Days],8[16-30 Days],17[31-45 Days],12[>45 Days]"
                    If Dtf.ToUpper = "CURRENT FY" Then
                        str = str.Replace("@Date", "convert(date,adate)>=(dateadd(year, datepart(year, getdate()-89) - 1900, '1900-04-01')) and convert(date,adate)<=(dateadd(day, -1, dateadd(month, 12, (dateadd(year, datepart(year, getdate()-89) - 1900, '1900-04-01')))))")
                    ElseIf Dtf.ToUpper = "LAST FY" Then
                        str = str.Replace("@Date", "convert(date,adate)>=dateadd(year, -1, (dateadd(year, datepart(year, getdate()-89) - 1900, '1900-04-01'))) and convert(date,adate)<=dateadd(year, -1, (dateadd(day, -1, dateadd(month, 12, (dateadd(year, datepart(year, getdate()-89) - 1900, '1900-04-01'))))))")
                    ElseIf Dtf.ToUpper = "LAST MONTH" Then
                        str = str.Replace("@Date", "convert(date,adate)>=(DATEADD(mm, DATEDIFF(mm, 0, GETDATE())-1, 0)) and convert(date,adate)<=(DATEADD(ms, -3, DATEADD(mm, DATEDIFF(m, 0, GETDATE()), 0)))")
                    ElseIf Dtf.ToUpper = "CURRENT MONTH" Then
                        str = str.Replace("@Date", "convert(date,adate)>=(DATEADD(mm, DATEDIFF(mm, 0, GETDATE()), 0)) and convert(date,adate)<=(DATEADD(ms, -3, DATEADD(mm, DATEDIFF(m, 0, GETDATE())+1, 0)))")
                    ElseIf Dtf.ToUpper = "CURRENT QUARTER" Then
                        str = str.Replace("@Date", "convert(date,adate)>=DATEADD(qq, DATEDIFF(qq, 0, GETDATE()), 0) and convert(date,adate)<=DATEADD (dd, -1, DATEADD(qq, DATEDIFF(qq, 0, GETDATE()) +1, 0))")
                    ElseIf Dtf.ToUpper = "LAST QUARTER" Then
                        str = str.Replace("@Date", "convert(date,adate)>=DATEADD(qq, DATEDIFF(qq, 0, GETDATE()) - 1, 0) and convert(date,adate)<=DATEADD (dd, -1, DATEADD(qq, DATEDIFF(qq, 0, GETDATE()), 0))")
                    Else
                        str = str.Replace("@Date", "convert(date,adate)>=convert(date,'" & Sdate.ToString & "')  and convert(date,adate)<=convert(date,'" & Edate.ToString & "')")
                    End If
                    If (Type.ToUpper = "DEPARTMENT") Or (Type.ToUpper = "ALL DEPARTMENTS") Then
                        str = str.Replace("@Dept", "d.tid=d.tid")
                    Else
                        str = str.Replace("@Dept", "(case when documenttype='invoice po' then dms.udf_split('MASTER-Department Master-fld2',fld65) when documenttype='invoice non po' then dms.udf_split('MASTER-Department Master-fld2',fld3)end)='" & Type.ToString & "'")
                    End If

                    If (Status.ToUpper = "PAID") Or (Status.ToString = "Paid") Then
                        str = str.Replace("@status", "curstatus in ('archive')")
                    ElseIf (Status.ToUpper = "UNPAID") Or (Status.ToString = "Unpaid") Then
                        str = str.Replace("@status", "curstatus not in ('rejected','archive')")
                    Else
                        str = str.Replace("@status", "curstatus=curstatus")
                    End If

                    If HttpContext.Current.Session("USERROLE").ToString.ToUpper = "SU" Then
                    Else
                        str = str.Replace("eid=" & HttpContext.Current.Session("EID").ToString & "", "eid=" & HttpContext.Current.Session("EID").ToString & " " & HttpContext.Current.Session("FilterRole").ToString)
                    End If

                    oda.SelectCommand.CommandType = CommandType.Text
                    oda.SelectCommand.CommandText = str
                    'oda.SelectCommand.CommandText = "Select 'TCS'[category],'48000'[value] union select 'CGI','37000' union select 'IBM','26000' union select 'Infosys','25000' union select 'Wipro','25000'  "
                    oda.Fill(ds, "data")


                    Dim CatList As New List(Of String)
                    Dim objWSLA As New series()
                    objWSLA.name = "Within SLA"

                    Dim objSLAB As New series()

                    objSLAB.name = "SLA Breached"
                    Dim LstData As New List(Of Integer)
                    Dim LstData1 As New List(Of Integer)

                    For i = 0 To ds.Tables("data").Rows.Count - 1
                        CatList.Add(ds.Tables("data").Rows(i).Item("category"))
                        LstData.Add(ds.Tables("data").Rows(i).Item("Within SLA"))
                        LstData1.Add(ds.Tables("data").Rows(i).Item("SLA Breached"))
                    Next
                    objWSLA.data = LstData
                    objSLAB.data = LstData1

                    Dim LstSeries As New List(Of series)
                    LstSeries.Add(objWSLA)
                    LstSeries.Add(objSLAB)
                    Res.categoryAxis = CatList
                    Res.series = LstSeries
                End If
            End Using
            Return Res
        Catch ex As Exception
            Throw
        Finally
            ds.Dispose()
            dt.Dispose()
        End Try


    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Shared Function GetDataInvoiceLifeCycle(Type As String, Dtf As String, Invdt As String, Sdate As String, Edate As String) As vdashboard
        Dim ds As New DataSet
        Dim dt As New DataTable
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim Res As New vdashboard()
        Try
            Using oda As New SqlDataAdapter("", conStr)
                Dim qry As String = ""
                If (Type.ToUpper = "DEPARTMENT") Then
                    qry = "SELECT rootqry_D from mmm_mst_misdb where eid=" & HttpContext.Current.Session("EID") & " and DBName='Invoice LifeCycle'"
                Else
                    qry = "SELECT rootquery_e from mmm_mst_misdb where eid=" & HttpContext.Current.Session("EID") & " and DBName='Invoice LifeCycle'"
                End If
                oda.SelectCommand.CommandType = CommandType.Text
                oda.SelectCommand.CommandText = qry
                oda.Fill(ds, "qry")
                If ds.Tables("qry").Rows.Count > 0 Then
                    Dim str As String = ds.Tables("qry").Rows(0).Item(0).ToString  '"select 'Finance'[category],12[0-15 Days],20[16-30 Days],16[31-45 Days],6[>45 Days] union select 'Human Resource'[Category],10[0-5 Days],12[16-30 Days],13[31-45 Days],9[>45 Days] union select 'Administration'[Category],8[0-5 Days],22[16-30 Days],10[31-45 Days],11[>45 Days] union select 'Sales and Marketing'[Category],12[0-5 Days],18[16-30 Days],7[31-45 Days],9[>45 Days] union select 'Others'[Category],17[0-5 Days],8[16-30 Days],17[31-45 Days],12[>45 Days]"
                    If Dtf.ToUpper = "CURRENT FY" Then
                        str = str.Replace("@Date", "convert(date,adate)>=(dateadd(year, datepart(year, getdate()-89) - 1900, '1900-04-01')) and convert(date,adate)<=(dateadd(day, -1, dateadd(month, 12, (dateadd(year, datepart(year, getdate()-89) - 1900, '1900-04-01')))))")
                    ElseIf Dtf.ToUpper = "LAST FY" Then
                        str = str.Replace("@Date", "convert(date,adate)>=dateadd(year, -1, (dateadd(year, datepart(year, getdate()-89) - 1900, '1900-04-01'))) and convert(date,adate)<=dateadd(year, -1, (dateadd(day, -1, dateadd(month, 12, (dateadd(year, datepart(year, getdate()-89) - 1900, '1900-04-01'))))))")
                    ElseIf Dtf.ToUpper = "LAST MONTH" Then
                        str = str.Replace("@Date", "convert(date,adate)>=(DATEADD(mm, DATEDIFF(mm, 0, GETDATE())-1, 0)) and convert(date,adate)<=(DATEADD(ms, -3, DATEADD(mm, DATEDIFF(m, 0, GETDATE()), 0)))")
                    ElseIf Dtf.ToUpper = "CURRENT MONTH" Then
                        str = str.Replace("@Date", "convert(date,adate)>=(DATEADD(mm, DATEDIFF(mm, 0, GETDATE()), 0)) and convert(date,adate)<=(DATEADD(ms, -3, DATEADD(mm, DATEDIFF(m, 0, GETDATE())+1, 0)))")
                    ElseIf Dtf.ToUpper = "CURRENT QUARTER" Then
                        str = str.Replace("@Date", "convert(date,adate)>=DATEADD(qq, DATEDIFF(qq, 0, GETDATE()), 0) and convert(date,adate)<=DATEADD (dd, -1, DATEADD(qq, DATEDIFF(qq, 0, GETDATE()) +1, 0))")
                    ElseIf Dtf.ToUpper = "LAST QUARTER" Then
                        str = str.Replace("@Date", "convert(date,adate)>=DATEADD(qq, DATEDIFF(qq, 0, GETDATE()) - 1, 0) and convert(date,adate)<=DATEADD (dd, -1, DATEADD(qq, DATEDIFF(qq, 0, GETDATE()), 0))")
                    Else
                        str = str.Replace("@Date", "convert(date,adate)>=convert(date,'" & Sdate.ToString & "')  and convert(date,adate)<=convert(date,'" & Edate.ToString & "')")
                    End If
                    If HttpContext.Current.Session("USERROLE").ToString.ToUpper = "SU" Then
                    Else
                        str = str.Replace("eid=" & HttpContext.Current.Session("EID").ToString & "", "eid=" & HttpContext.Current.Session("EID").ToString & " " & HttpContext.Current.Session("FilterRole").ToString)
                    End If

                    oda.SelectCommand.CommandText = "select * from mmm_mst_misdb_dtl where refTid in (SELECT tid from mmm_mst_misdb where eid=" & HttpContext.Current.Session("EID") & " and DBName='Invoice LifeCycle')"
                    oda.Fill(ds, "fld")

                    If (Invdt.ToUpper = "BY INVOICE DATE") Then
                        If ds.Tables("fld").Rows.Count > 0 Then
                            For i As Integer = 0 To ds.Tables("fld").Rows.Count - 1
                                str = str.Replace("@" & ds.Tables("fld").Rows(i).Item("Documenttype").ToString.Replace(" ", "_") & "Date", ds.Tables("fld").Rows(i).Item("InvDatefield").ToString)
                            Next
                        End If
                    Else
                        If ds.Tables("fld").Rows.Count > 0 Then
                            For i As Integer = 0 To ds.Tables("fld").Rows.Count - 1
                                str = str.Replace("@" & ds.Tables("fld").Rows(i).Item("Documenttype").ToString.Replace(" ", "_") & "Date", ds.Tables("fld").Rows(i).Item("RecDatefield").ToString)
                            Next
                        End If
                    End If

                    oda.SelectCommand.CommandType = CommandType.Text
                    oda.SelectCommand.CommandText = str
                    'oda.SelectCommand.CommandText = "Select 'TCS'[category],'48000'[value] union select 'CGI','37000' union select 'IBM','26000' union select 'Infosys','25000' union select 'Wipro','25000'  "
                    oda.Fill(ds, "data")



                    Dim CatList As New List(Of String)

                    Dim count As New List(Of String)

                    Dim obj0to15 As New series()
                    'Dim cobj0to15 As New series()
                    obj0to15.name = "0-15 Days"

                    'obj0to15.color = "rgb(0, 128, 0)"

                    Dim obj16 As New series()
                    'obj16.color = "rgb(128, 128, 128)"
                    obj16.name = "16-30 Days"

                    Dim obj31 As New series()
                    'obj31.color = "rgb(128, 128, 0)"
                    obj31.name = "31-45 Days"

                    Dim obj45 As New series()
                    'obj45.color = "rgb(128, 0, 0)"
                    obj45.name = ">45 Days"


                    'Dim cobj0to15 As New series1()
                    ''Dim cobj0to15 As New series()
                    'cobj0to15.name = "count15"

                    ''obj0to15.color = "rgb(0, 128, 0)"

                    'Dim cobj16 As New series1()
                    ''obj16.color = "rgb(128, 128, 128)"
                    'cobj16.name = "count16"

                    'Dim cobj31 As New series1()
                    ''obj31.color = "rgb(128, 128, 0)"
                    'cobj31.name = "count31"

                    'Dim cobj45 As New series1()
                    ''obj45.color = "rgb(128, 0, 0)"
                    'cobj45.name = "count45"


                    Dim LstData As New List(Of Integer)
                    Dim LstData1 As New List(Of Integer)
                    Dim LstData2 As New List(Of Integer)
                    Dim LstData3 As New List(Of Integer)
                    Dim cLstData As New List(Of Integer)
                    Dim cLstData1 As New List(Of Integer)
                    Dim cLstData2 As New List(Of Integer)
                    Dim cLstData3 As New List(Of Integer)
                    For i = 0 To ds.Tables("data").Rows.Count - 1
                        CatList.Add(ds.Tables("data").Rows(i).Item("category"))
                        LstData.Add(ds.Tables("data").Rows(i).Item("0-15 Days"))
                        ' cLstData.Add(ds.Tables("data").Rows(i).Item("count15"))
                        LstData1.Add(ds.Tables("data").Rows(i).Item("16-30 Days"))
                        'cLstData1.Add(ds.Tables("data").Rows(i).Item("count16"))
                        LstData2.Add(ds.Tables("data").Rows(i).Item("31-45 Days"))
                        'cLstData2.Add(ds.Tables("data").Rows(i).Item("count31"))
                        LstData3.Add(ds.Tables("data").Rows(i).Item(">45 Days"))
                        'cLstData3.Add(ds.Tables("data").Rows(i).Item("count45"))

                    Next
                    obj0to15.data = LstData
                    obj16.data = LstData1
                    obj31.data = LstData2
                    obj45.data = LstData3
                    'cobj0to15.data = cLstData
                    'cobj16.data = cLstData1
                    'cobj31.data = cLstData2
                    'cobj45.data = cLstData3
                    Dim LstSeries As New List(Of series)
                    'Dim cLstSeries As New List(Of series1)

                    LstSeries.Add(obj0to15)
                    LstSeries.Add(obj16)
                    LstSeries.Add(obj31)
                    LstSeries.Add(obj45)

                    'cLstSeries.Add(cobj0to15)
                    'cLstSeries.Add(cobj16)
                    'cLstSeries.Add(cobj31)
                    'cLstSeries.Add(cobj45)


                    Res.categoryAxis = CatList
                    Res.series = LstSeries
                    'Res.countseries = cLstSeries

                End If
            End Using
            Return Res
        Catch ex As Exception
            Throw
        Finally
            ds.Dispose()
            dt.Dispose()
        End Try
    End Function
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Shared Function GetDataInvoiceAgeing(Type As String, Dtf As String, Invdt As String) As vdashboard
        Dim ds As New DataSet
        Dim dt As New DataTable
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim Res As New vdashboard()
        Try
            Using oda As New SqlDataAdapter("", conStr)
                Dim qry As String = ""
                If (Type.ToUpper = "DEPARTMENT") Then
                    qry = "SELECT rootqry_D from mmm_mst_misdb where eid=" & HttpContext.Current.Session("EID") & " and DBName='Open Invoice Ageing'"
                Else
                    qry = "SELECT rootqry_D from mmm_mst_misdb where eid=" & HttpContext.Current.Session("EID") & " and DBName='Open Invoice Ageing'"
                End If
                oda.SelectCommand.CommandType = CommandType.Text
                oda.SelectCommand.CommandText = qry
                oda.Fill(ds, "qry")
                If ds.Tables("qry").Rows.Count > 0 Then
                    Dim str As String = ds.Tables("qry").Rows(0).Item(0).ToString  '"select 'Finance'[category],12[0-15 Days],20[16-30 Days],16[31-45 Days],6[>45 Days] union select 'Human Resource'[Category],10[0-5 Days],12[16-30 Days],13[31-45 Days],9[>45 Days] union select 'Administration'[Category],8[0-5 Days],22[16-30 Days],10[31-45 Days],11[>45 Days] union select 'Sales and Marketing'[Category],12[0-5 Days],18[16-30 Days],7[31-45 Days],9[>45 Days] union select 'Others'[Category],17[0-5 Days],8[16-30 Days],17[31-45 Days],12[>45 Days]"

                    If (Type.ToUpper = "DEPARTMENT") Or (Type.ToUpper = "ALL DEPARTMENTS") Then
                        str = str.Replace("@Dept", "d.tid=d.tid")
                    Else
                        str = str.Replace("@Dept", "(case when documenttype='invoice po' then dms.udf_split('MASTER-Department Master-fld2',fld65) when documenttype='invoice non po' then dms.udf_split('MASTER-Department Master-fld2',fld3)end)='" & Type.ToString & "'")
                    End If

                    If HttpContext.Current.Session("USERROLE").ToString.ToUpper = "SU" Then
                    Else
                        str = str.Replace("eid=" & HttpContext.Current.Session("EID").ToString & "", "eid=" & HttpContext.Current.Session("EID").ToString & " " & HttpContext.Current.Session("FilterRole").ToString)
                    End If

                    oda.SelectCommand.CommandText = "select * from mmm_mst_misdb_dtl where refTid in (SELECT tid from mmm_mst_misdb where eid=" & HttpContext.Current.Session("EID") & " and DBName='Open Invoice Ageing')"
                    oda.Fill(ds, "fld")

                    If (Invdt.ToUpper = "BY INVOICE DATE") Then
                        If ds.Tables("fld").Rows.Count > 0 Then
                            For i As Integer = 0 To ds.Tables("fld").Rows.Count - 1
                                str = str.Replace("@" & ds.Tables("fld").Rows(i).Item("Documenttype").ToString.Replace(" ", "_") & "Date", ds.Tables("fld").Rows(i).Item("InvDatefield").ToString)
                            Next
                        End If
                    Else
                        If ds.Tables("fld").Rows.Count > 0 Then
                            For i As Integer = 0 To ds.Tables("fld").Rows.Count - 1
                                str = str.Replace("@" & ds.Tables("fld").Rows(i).Item("Documenttype").ToString.Replace(" ", "_") & "Date", ds.Tables("fld").Rows(i).Item("RecDatefield").ToString)
                            Next
                        End If
                    End If


                    oda.SelectCommand.CommandType = CommandType.Text
                    oda.SelectCommand.CommandText = str
                    'oda.SelectCommand.CommandText = "Select 'TCS'[category],'48000'[value] union select 'CGI','37000' union select 'IBM','26000' union select 'Infosys','25000' union select 'Wipro','25000'  "
                    oda.Fill(ds, "data")


                    Dim CatList As New List(Of String)
                    Dim obj0to5 As New series()
                    obj0to5.name = "0-5 Days"
                    ' obj0to15.color = "rgb(0, 128, 0)"

                    Dim obj6 As New series()
                    'obj16.color = "rgb(0, 128, 128)"
                    obj6.name = "6-10 Days"

                    Dim obj11 As New series()
                    'obj31.color = "rgb(128, 128, 10)"
                    obj11.name = "11-15 Days"

                    Dim obj15 As New series()
                    'obj45.color = "rgb(220, 0, 0)"
                    obj15.name = ">15 Days"


                    Dim LstData As New List(Of Integer)
                    Dim LstData1 As New List(Of Integer)
                    Dim LstData2 As New List(Of Integer)
                    Dim LstData3 As New List(Of Integer)
                    For i = 0 To ds.Tables("data").Rows.Count - 1
                        CatList.Add(ds.Tables("data").Rows(i).Item("category"))
                        LstData.Add(ds.Tables("data").Rows(i).Item("0-5 Days"))
                        LstData1.Add(ds.Tables("data").Rows(i).Item("6-10 Days"))
                        LstData2.Add(ds.Tables("data").Rows(i).Item("11-15 Days"))
                        LstData3.Add(ds.Tables("data").Rows(i).Item(">15 Days"))
                    Next
                    obj0to5.data = LstData
                    obj6.data = LstData1
                    obj11.data = LstData2
                    obj15.data = LstData3
                    Dim LstSeries As New List(Of series)

                    LstSeries.Add(obj0to5)
                    LstSeries.Add(obj6)
                    LstSeries.Add(obj11)
                    LstSeries.Add(obj15)
                    Res.categoryAxis = CatList
                    Res.series = LstSeries
                End If
            End Using
            Return Res
        Catch ex As Exception
            Throw
        Finally
            ds.Dispose()
            dt.Dispose()
        End Try

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Shared Function InvoiceDist(Type As String, Dtf As String, Sdate As String, Edate As String) As vdashboard
        Dim jsonData As String = ""
        Try
            Dim ds As New DataSet()
            Dim UID As Integer = 0
            Dim URole As String = ""
            Dim qry As String = ""
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As New SqlConnection(conStr)
            Dim dt As New DataTable
            Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
            Dim Res As New vdashboard()
            If (Type.ToUpper = "DEPARTMENT") Then
                qry = "SELECT rootqry_D from mmm_mst_misdb where eid=" & HttpContext.Current.Session("EID") & " and DBName='Invoice Distribution'"
            Else
                qry = "SELECT rootquery_e from mmm_mst_misdb where eid=" & HttpContext.Current.Session("EID") & " and DBName='Invoice Distribution'"
            End If
            oda.SelectCommand.CommandType = CommandType.Text
            oda.SelectCommand.CommandText = qry
            oda.Fill(ds, "qry")
            If ds.Tables("qry").Rows.Count > 0 Then

                Dim str = ds.Tables("qry").Rows(0).Item(0).ToString '"select 'Recruitment Cost'[category],'PO'[Type],10[Value] union select 'Operational Cost'[category],'PO'[Type],21[Value] union select 'Printing'[category],'PO'[Type],25[Value] union select 'Others'[category],'PO'[Type],34[Value] union select 'Recruitment Cost'[category],'NON PO'[Type],20[Value] union select 'Operational Cost'[category],'NON PO'[Type],12[Value]union select 'Printing'[category],'NON PO'[Type],17[Value]union select 'Others'[category],'NON PO'[Type],13[Value]"
                If Dtf.ToUpper = "CURRENT FY" Then
                    str = str.Replace("@Date", "convert(date,adate)>=(dateadd(year, datepart(year, getdate()-89) - 1900, '1900-04-01')) and convert(date,adate)<=(dateadd(day, -1, dateadd(month, 12, (dateadd(year, datepart(year, getdate()-89) - 1900, '1900-04-01')))))")
                ElseIf Dtf.ToUpper = "LAST FY" Then
                    str = str.Replace("@Date", "convert(date,adate)>=dateadd(year, -1, (dateadd(year, datepart(year, getdate()-89) - 1900, '1900-04-01'))) and convert(date,adate)<=dateadd(year, -1, (dateadd(day, -1, dateadd(month, 12, (dateadd(year, datepart(year, getdate()-89) - 1900, '1900-04-01'))))))")
                ElseIf Dtf.ToUpper = "LAST MONTH" Then
                    str = str.Replace("@Date", "convert(date,adate)>=(DATEADD(mm, DATEDIFF(mm, 0, GETDATE())-1, 0)) and convert(date,adate)<=(DATEADD(ms, -3, DATEADD(mm, DATEDIFF(m, 0, GETDATE()), 0)))")
                ElseIf Dtf.ToUpper = "CURRENT MONTH" Then
                    str = str.Replace("@Date", "convert(date,adate)>=(DATEADD(mm, DATEDIFF(mm, 0, GETDATE()), 0)) and convert(date,adate)<=(DATEADD(ms, -3, DATEADD(mm, DATEDIFF(m, 0, GETDATE())+1, 0)))")
                ElseIf Dtf.ToUpper = "CURRENT QUARTER" Then
                    str = str.Replace("@Date", "convert(date,adate)>=DATEADD(qq, DATEDIFF(qq, 0, GETDATE()), 0) and convert(date,adate)<=DATEADD (dd, -1, DATEADD(qq, DATEDIFF(qq, 0, GETDATE()) +1, 0))")
                ElseIf Dtf.ToUpper = "LAST QUARTER" Then
                    str = str.Replace("@Date", "convert(date,adate)>=DATEADD(qq, DATEDIFF(qq, 0, GETDATE()) - 1, 0) and convert(date,adate)<=DATEADD (dd, -1, DATEADD(qq, DATEDIFF(qq, 0, GETDATE()), 0))")
                Else
                    str = str.Replace("@Date", "convert(date,adate)>=convert(date,'" & Sdate.ToString & "')  and convert(date,adate)<=convert(date,'" & Edate.ToString & "')")
                End If

                If HttpContext.Current.Session("USERROLE").ToString.ToUpper = "SU" Then
                Else
                    str = str.Replace("eid=" & HttpContext.Current.Session("EID").ToString & "", "eid=" & HttpContext.Current.Session("EID").ToString & " " & HttpContext.Current.Session("FilterRole").ToString)
                End If
                oda.SelectCommand.CommandType = CommandType.Text
                oda.SelectCommand.CommandText = str
                oda.Fill(ds, "data")

                Dim CatList As New List(Of String)

                Dim objPO As New series()
                objPO.name = "PO"
                ' obj0to15.color = "rgb(0, 128, 0)"

                Dim objNONPO As New series()
                'obj16.color = "rgb(0, 128, 128)"
                objNONPO.name = "NON PO"

                Dim LstData As New List(Of Integer)
                Dim LstData1 As New List(Of Integer)
                Dim LstData2 As New List(Of Integer)
                Dim LstData3 As New List(Of Integer)
                For i = 0 To ds.Tables("data").Rows.Count - 1
                    CatList.Add(ds.Tables("data").Rows(i).Item("category"))
                    LstData.Add(ds.Tables("data").Rows(i).Item("PO"))
                    LstData1.Add(ds.Tables("data").Rows(i).Item("NON PO"))
                Next
                objPO.data = LstData
                objNONPO.data = LstData1

                Dim LstSeries As New List(Of series)
                LstSeries.Add(objPO)
                LstSeries.Add(objNONPO)
                Res.categoryAxis = CatList
                Res.series = LstSeries
            End If
            Return Res
        Catch Ex As Exception
            Throw
        End Try

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Shared Function getExpenseBreakupDtl(Type As String, Doc As String, Dtf As String, Sdate As String, Edate As String) As DGrid
        Dim jsonData As String = ""
        Dim grid As New DGrid()
        Try
            Dim ds As New DataSet()
            Dim Query As String = ""
            Dim dt As New DataTable
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Using con As New SqlConnection(conStr)
                Using oda As New SqlDataAdapter(Query, con)
                    Dim qry As String = ""
                    If Type.ToUpper = "DEPARTMENT" Then
                        qry = "SELECT firstlevelqry from mmm_mst_MISDB where eid=" & HttpContext.Current.Session("EID") & " and DBName='Expense Breakup'"
                    Else
                        qry = "SELECT secondlevelqry from mmm_mst_MISDB where eid=" & HttpContext.Current.Session("EID") & " and DBName='Expense Breakup'"
                    End If

                    oda.SelectCommand.CommandType = CommandType.Text
                    oda.SelectCommand.CommandText = qry
                    oda.Fill(dt)
                    Dim str = dt.Rows(0).Item(0).ToString
                    If Dtf.ToUpper = "CURRENT FY" Then
                        str = str.Replace("@Date", "convert(date,adate)>=(dateadd(year, datepart(year, getdate()-89) - 1900, '1900-04-01')) and convert(date,adate)<=(dateadd(day, -1, dateadd(month, 12, (dateadd(year, datepart(year, getdate()-89) - 1900, '1900-04-01')))))")
                    ElseIf Dtf.ToUpper = "LAST FY" Then
                        str = str.Replace("@Date", "convert(date,adate)>=dateadd(year, -1, (dateadd(year, datepart(year, getdate()-89) - 1900, '1900-04-01'))) and convert(date,adate)<=dateadd(year, -1, (dateadd(day, -1, dateadd(month, 12, (dateadd(year, datepart(year, getdate()-89) - 1900, '1900-04-01'))))))")
                    ElseIf Dtf.ToUpper = "LAST MONTH" Then
                        str = str.Replace("@Date", "convert(date,adate)>=(DATEADD(mm, DATEDIFF(mm, 0, GETDATE())-1, 0)) and convert(date,adate)<=(DATEADD(ms, -3, DATEADD(mm, DATEDIFF(m, 0, GETDATE()), 0)))")
                    ElseIf Dtf.ToUpper = "CURRENT MONTH" Then
                        str = str.Replace("@Date", "convert(date,adate)>=(DATEADD(mm, DATEDIFF(mm, 0, GETDATE()), 0)) and convert(date,adate)<=(DATEADD(ms, -3, DATEADD(mm, DATEDIFF(m, 0, GETDATE())+1, 0)))")
                    ElseIf Dtf.ToUpper = "CURRENT QUARTER" Then
                        str = str.Replace("@Date", "convert(date,adate)>=DATEADD(qq, DATEDIFF(qq, 0, GETDATE()), 0) and convert(date,adate)<=DATEADD (dd, -1, DATEADD(qq, DATEDIFF(qq, 0, GETDATE()) +1, 0))")
                    ElseIf Dtf.ToUpper = "LAST QUARTER" Then
                        str = str.Replace("@Date", "convert(date,adate)>=DATEADD(qq, DATEDIFF(qq, 0, GETDATE()) - 1, 0) and convert(date,adate)<=DATEADD (dd, -1, DATEADD(qq, DATEDIFF(qq, 0, GETDATE()), 0))")
                    Else
                        str = str.Replace("@Date", "convert(date,adate)>=convert(date,'" & Sdate.ToString & "')  and convert(date,adate)<=convert(date,'" & Edate.ToString & "')")
                    End If
                    str = str.Replace("@dept", "'" & Doc & "'")
                    If HttpContext.Current.Session("USERROLE").ToString.ToUpper = "SU" Then
                    Else
                        str = str.Replace("eid=" & HttpContext.Current.Session("EID").ToString & "", "eid=" & HttpContext.Current.Session("EID").ToString & " " & HttpContext.Current.Session("FilterRole").ToString)
                    End If
                    oda.SelectCommand.CommandType = CommandType.Text
                    oda.SelectCommand.CommandText = str
                    oda.Fill(ds)
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

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Shared Function getSupplierBreakupDtl(Type As String, Doc As String, Dtf As String, Sdate As String, Edate As String) As DGrid
        Dim jsonData As String = ""
        Dim grid As New DGrid()
        Try
            Dim ds As New DataSet()
            Dim Query As String = ""
            Dim dt As New DataTable
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Using con As New SqlConnection(conStr)
                Using oda As New SqlDataAdapter(Query, con)
                    Dim qry As String = ""
                    qry = "SELECT firstlevelqry from mmm_mst_MISDB where eid=" & HttpContext.Current.Session("EID") & " and DBName='Supplier Spend Breakup'"
                    oda.SelectCommand.CommandType = CommandType.Text
                    oda.SelectCommand.CommandText = qry
                    oda.Fill(dt)
                    Dim str = dt.Rows(0).Item(0).ToString
                    str = str.Replace("@vendor", "'" & Doc & "'")
                    If Dtf.ToUpper = "CURRENT FY" Then
                        str = str.Replace("@Date", "convert(date,adate)>=(dateadd(year, datepart(year, getdate()-89) - 1900, '1900-04-01')) and convert(date,adate)<=(dateadd(day, -1, dateadd(month, 12, (dateadd(year, datepart(year, getdate()-89) - 1900, '1900-04-01')))))")
                    ElseIf Dtf.ToUpper = "LAST FY" Then
                        str = str.Replace("@Date", "convert(date,adate)>=dateadd(year, -1, (dateadd(year, datepart(year, getdate()-89) - 1900, '1900-04-01'))) and convert(date,adate)<=dateadd(year, -1, (dateadd(day, -1, dateadd(month, 12, (dateadd(year, datepart(year, getdate()-89) - 1900, '1900-04-01'))))))")
                    ElseIf Dtf.ToUpper = "LAST MONTH" Then
                        str = str.Replace("@Date", "convert(date,adate)>=(DATEADD(mm, DATEDIFF(mm, 0, GETDATE())-1, 0)) and convert(date,adate)<=(DATEADD(ms, -3, DATEADD(mm, DATEDIFF(m, 0, GETDATE()), 0)))")
                    ElseIf Dtf.ToUpper = "CURRENT MONTH" Then
                        str = str.Replace("@Date", "convert(date,adate)>=(DATEADD(mm, DATEDIFF(mm, 0, GETDATE()), 0)) and convert(date,adate)<=(DATEADD(ms, -3, DATEADD(mm, DATEDIFF(m, 0, GETDATE())+1, 0)))")
                    ElseIf Dtf.ToUpper = "CURRENT QUARTER" Then
                        str = str.Replace("@Date", "convert(date,adate)>=DATEADD(qq, DATEDIFF(qq, 0, GETDATE()), 0) and convert(date,adate)<=DATEADD (dd, -1, DATEADD(qq, DATEDIFF(qq, 0, GETDATE()) +1, 0))")
                    ElseIf Dtf.ToUpper = "LAST QUARTER" Then
                        str = str.Replace("@Date", "convert(date,adate)>=DATEADD(qq, DATEDIFF(qq, 0, GETDATE()) - 1, 0) and convert(date,adate)<=DATEADD (dd, -1, DATEADD(qq, DATEDIFF(qq, 0, GETDATE()), 0))")
                    Else
                        str = str.Replace("@Date", "convert(date,adate)>=convert(date,'" & Sdate.ToString & "')  and convert(date,adate)<=convert(date,'" & Edate.ToString & "')")
                    End If
                    If HttpContext.Current.Session("USERROLE").ToString.ToUpper = "SU" Then
                    Else
                        str = str.Replace("eid=" & HttpContext.Current.Session("EID").ToString & "", "eid=" & HttpContext.Current.Session("EID").ToString & " " & HttpContext.Current.Session("FilterRole").ToString)
                    End If
                    oda.SelectCommand.CommandType = CommandType.Text
                    oda.SelectCommand.CommandText = str
                    oda.Fill(ds)
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

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Shared Function getInvoiceDistDtl(Type As String, Doc As String, Name As String, Dtf As String, Sdate As String, Edate As String) As DGrid
        Dim jsonData As String = ""
        Dim grid As New DGrid()
        Try
            Dim ds As New DataSet()
            Dim Query As String = ""
            Dim dt As New DataTable
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Using con As New SqlConnection(conStr)
                Using oda As New SqlDataAdapter(Query, con)
                    Dim qry As String = ""
                    If Type.ToUpper = "DEPARTMENT" Then
                        qry = "SELECT firstlevelqry from mmm_mst_MISDB where eid=" & HttpContext.Current.Session("EID") & " and DBName='Invoice Distribution'"
                    Else
                        qry = "SELECT secondlevelqry from mmm_mst_MISDB where eid=" & HttpContext.Current.Session("EID") & " and DBName='Invoice Distribution'"
                    End If

                    oda.SelectCommand.CommandType = CommandType.Text
                    oda.SelectCommand.CommandText = qry
                    oda.Fill(dt)
                    Dim str = dt.Rows(0).Item(0).ToString
                    str = str.Replace("@dept", "'" & Doc & "'")
                    If Name.ToUpper = "PO" Then
                        str = str.Replace("@doctype", "'Invoice PO'")
                    Else
                        str = str.Replace("@doctype", "'Invoice Non PO'")
                    End If
                    If Dtf.ToUpper = "CURRENT FY" Then
                        str = str.Replace("@Date", "convert(date,adate)>=(dateadd(year, datepart(year, getdate()-89) - 1900, '1900-04-01')) and convert(date,adate)<=(dateadd(day, -1, dateadd(month, 12, (dateadd(year, datepart(year, getdate()-89) - 1900, '1900-04-01')))))")
                    ElseIf Dtf.ToUpper = "LAST FY" Then
                        str = str.Replace("@Date", "convert(date,adate)>=dateadd(year, -1, (dateadd(year, datepart(year, getdate()-89) - 1900, '1900-04-01'))) and convert(date,adate)<=dateadd(year, -1, (dateadd(day, -1, dateadd(month, 12, (dateadd(year, datepart(year, getdate()-89) - 1900, '1900-04-01'))))))")
                    ElseIf Dtf.ToUpper = "LAST MONTH" Then
                        str = str.Replace("@Date", "convert(date,adate)>=(DATEADD(mm, DATEDIFF(mm, 0, GETDATE())-1, 0)) and convert(date,adate)<=(DATEADD(ms, -3, DATEADD(mm, DATEDIFF(m, 0, GETDATE()), 0)))")
                    ElseIf Dtf.ToUpper = "CURRENT MONTH" Then
                        str = str.Replace("@Date", "convert(date,adate)>=(DATEADD(mm, DATEDIFF(mm, 0, GETDATE()), 0)) and convert(date,adate)<=(DATEADD(ms, -3, DATEADD(mm, DATEDIFF(m, 0, GETDATE())+1, 0)))")
                    ElseIf Dtf.ToUpper = "CURRENT QUARTER" Then
                        str = str.Replace("@Date", "convert(date,adate)>=DATEADD(qq, DATEDIFF(qq, 0, GETDATE()), 0) and convert(date,adate)<=DATEADD (dd, -1, DATEADD(qq, DATEDIFF(qq, 0, GETDATE()) +1, 0))")
                    ElseIf Dtf.ToUpper = "LAST QUARTER" Then
                        str = str.Replace("@Date", "convert(date,adate)>=DATEADD(qq, DATEDIFF(qq, 0, GETDATE()) - 1, 0) and convert(date,adate)<=DATEADD (dd, -1, DATEADD(qq, DATEDIFF(qq, 0, GETDATE()), 0))")
                    Else
                        str = str.Replace("@Date", "convert(date,adate)>=convert(date,'" & Sdate.ToString & "')  and convert(date,adate)<=convert(date,'" & Edate.ToString & "')")
                    End If
                    If HttpContext.Current.Session("USERROLE").ToString.ToUpper = "SU" Then
                    Else
                        str = str.Replace("eid=" & HttpContext.Current.Session("EID").ToString & "", "eid=" & HttpContext.Current.Session("EID").ToString & " " & HttpContext.Current.Session("FilterRole").ToString)
                    End If
                    oda.SelectCommand.CommandType = CommandType.Text
                    oda.SelectCommand.CommandText = str
                    oda.Fill(ds)
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

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Shared Function getInvoiceLifeCycleDtl(Type As String, Doc As String, Name As String, Dtf As String, Invdt As String, Sdate As String, Edate As String) As DGrid
        Dim jsonData As String = ""
        Dim grid As New DGrid()
        Try
            Dim ds As New DataSet()
            Dim ds1 As New DataSet()
            Dim Query As String = ""
            Dim dt As New DataTable
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Using con As New SqlConnection(conStr)
                Using oda As New SqlDataAdapter(Query, con)
                    Dim qry As String = ""
                    If Type.ToUpper = "DEPARTMENT" Then
                        qry = "SELECT firstlevelqry from mmm_mst_MISDB where eid=" & HttpContext.Current.Session("EID") & " and DBName='Invoice LifeCycle'"
                    Else
                        qry = "SELECT secondlevelqry from mmm_mst_MISDB where eid=" & HttpContext.Current.Session("EID") & " and DBName='Invoice LifeCycle'"
                    End If

                    oda.SelectCommand.CommandType = CommandType.Text
                    oda.SelectCommand.CommandText = qry
                    oda.Fill(dt)
                    Dim str = dt.Rows(0).Item(0).ToString
                    str = str.Replace("@Dept", "'" & Doc & "'")
                    str = str.Replace("@dept", "'" & Doc & "'")
                    If Dtf.ToUpper = "CURRENT FY" Then
                        str = str.Replace("@Date", "convert(date,adate)>=(dateadd(year, datepart(year, getdate()-89) - 1900, '1900-04-01')) and convert(date,adate)<=(dateadd(day, -1, dateadd(month, 12, (dateadd(year, datepart(year, getdate()-89) - 1900, '1900-04-01')))))")
                    ElseIf Dtf.ToUpper = "LAST FY" Then
                        str = str.Replace("@Date", "convert(date,adate)>=dateadd(year, -1, (dateadd(year, datepart(year, getdate()-89) - 1900, '1900-04-01'))) and convert(date,adate)<=dateadd(year, -1, (dateadd(day, -1, dateadd(month, 12, (dateadd(year, datepart(year, getdate()-89) - 1900, '1900-04-01'))))))")
                    ElseIf Dtf.ToUpper = "LAST MONTH" Then
                        str = str.Replace("@Date", "convert(date,adate)>=(DATEADD(mm, DATEDIFF(mm, 0, GETDATE())-1, 0)) and convert(date,adate)<=(DATEADD(ms, -3, DATEADD(mm, DATEDIFF(m, 0, GETDATE()), 0)))")
                    ElseIf Dtf.ToUpper = "CURRENT MONTH" Then
                        str = str.Replace("@Date", "convert(date,adate)>=(DATEADD(mm, DATEDIFF(mm, 0, GETDATE()), 0)) and convert(date,adate)<=(DATEADD(ms, -3, DATEADD(mm, DATEDIFF(m, 0, GETDATE())+1, 0)))")
                    ElseIf Dtf.ToUpper = "CURRENT QUARTER" Then
                        str = str.Replace("@Date", "convert(date,adate)>=DATEADD(qq, DATEDIFF(qq, 0, GETDATE()), 0) and convert(date,adate)<=DATEADD (dd, -1, DATEADD(qq, DATEDIFF(qq, 0, GETDATE()) +1, 0))")
                    ElseIf Dtf.ToUpper = "LAST QUARTER" Then
                        str = str.Replace("@Date", "convert(date,adate)>=DATEADD(qq, DATEDIFF(qq, 0, GETDATE()) - 1, 0) and convert(date,adate)<=DATEADD (dd, -1, DATEADD(qq, DATEDIFF(qq, 0, GETDATE()), 0))")
                    Else
                        str = str.Replace("@Date", "convert(date,adate)>=convert(date,'" & Sdate.ToString & "')  and convert(date,adate)<=convert(date,'" & Edate.ToString & "')")
                    End If
                    If Name.ToUpper = "0-15 DAYS" Then
                        str = str.Replace("@days1", "0")
                        str = str.Replace("@days2", "15")
                    ElseIf Name.ToUpper = "16-30 DAYS" Then
                        str = str.Replace("@days1", "16")
                        str = str.Replace("@days2", "30")
                    ElseIf Name.ToUpper = "31-45 DAYS" Then
                        str = str.Replace("@days1", "31")
                        str = str.Replace("@days2", "45")
                    Else
                        str = str.Replace("@days1", "46")
                        str = str.Replace("@days2", "4600")
                    End If
                    If HttpContext.Current.Session("USERROLE").ToString.ToUpper = "SU" Then
                    Else
                        str = str.Replace("eid=" & HttpContext.Current.Session("EID").ToString & "", "eid=" & HttpContext.Current.Session("EID").ToString & " " & HttpContext.Current.Session("FilterRole").ToString)
                    End If

                    oda.SelectCommand.CommandText = "select * from mmm_mst_misdb_dtl where refTid in (SELECT tid from mmm_mst_misdb where eid=" & HttpContext.Current.Session("EID") & " and DBName='Invoice LifeCycle')"
                    oda.Fill(ds, "fld")

                    If (Invdt.ToUpper = "BY INVOICE DATE") Then
                        If ds.Tables("fld").Rows.Count > 0 Then
                            For i As Integer = 0 To ds.Tables("fld").Rows.Count - 1
                                str = str.Replace("@" & ds.Tables("fld").Rows(i).Item("Documenttype").ToString.Replace(" ", "_") & "Date", ds.Tables("fld").Rows(i).Item("InvDatefield").ToString)
                            Next
                        End If
                    Else
                        If ds.Tables("fld").Rows.Count > 0 Then
                            For i As Integer = 0 To ds.Tables("fld").Rows.Count - 1
                                str = str.Replace("@" & ds.Tables("fld").Rows(i).Item("Documenttype").ToString.Replace(" ", "_") & "Date", ds.Tables("fld").Rows(i).Item("RecDatefield").ToString)
                            Next
                        End If
                    End If
                    oda.SelectCommand.CommandType = CommandType.Text
                    oda.SelectCommand.CommandText = str
                    oda.Fill(ds1)
                End Using
            End Using
            Dim strError = ""
            grid = DynamicGrid.GridData(ds1.Tables(0), strError)
            If ds1.Tables(0).Rows.Count = 0 Then
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

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Shared Function getOpenInvoiceAgeingDtl(Type As String, Doc As String, Name As String, Dtf As String) As DGrid
        Dim jsonData As String = ""
        Dim grid As New DGrid()
        Try
            Dim ds As New DataSet()
            Dim Query As String = ""
            Dim dt As New DataTable
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Using con As New SqlConnection(conStr)
                Using oda As New SqlDataAdapter(Query, con)
                    Dim qry As String = ""
                    If Type.ToUpper = "DEPARTMENT" Then
                        qry = "SELECT firstlevelqry from mmm_mst_MISDB where eid=" & HttpContext.Current.Session("EID") & " and DBName='Open Invoice Ageing'"
                    Else
                        qry = "SELECT firstlevelqry from mmm_mst_MISDB where eid=" & HttpContext.Current.Session("EID") & " and DBName='Open Invoice Ageing'"
                    End If

                    oda.SelectCommand.CommandType = CommandType.Text
                    oda.SelectCommand.CommandText = qry
                    oda.Fill(dt)
                    Dim str = dt.Rows(0).Item(0).ToString

                    If Doc.ToUpper = "VHD" Then
                        str = str.Replace("@status", "'UPLOADED'")
                    ElseIf Doc.ToUpper = "IHCL" Then
                        str = str.Replace("@status", "'Approver 1','Approver 2','Approver 3','Referral Recommendation','RR Entry'")
                    Else
                        str = str.Replace("@status", "'" & Doc & "'")
                    End If


                    If (Type.ToUpper = "DEPARTMENT") Or (Type.ToUpper = "ALL DEPARTMENTS") Then
                        str = str.Replace("@Dept", "d.tid=d.tid")
                    Else
                        str = str.Replace("@Dept", "(case when documenttype='invoice po' then dms.udf_split('MASTER-Department Master-fld2',fld65) when documenttype='invoice non po' then dms.udf_split('MASTER-Department Master-fld2',fld3)end)='" & Type.ToString & "'")
                    End If
                    'str = str.Replace("@Dept", "'" & Doc & "'")
                    'str = str.Replace("@dept", "'" & Doc & "'")
                    If Name.ToUpper = "0-5 DAYS" Then
                        str = str.Replace("@days1", "0")
                        str = str.Replace("@days2", "5")
                    ElseIf Name.ToUpper = "6-10 DAYS" Then
                        str = str.Replace("@days1", "6")
                        str = str.Replace("@days2", "10")
                    ElseIf Name.ToUpper = "11-15 DAYS" Then
                        str = str.Replace("@days1", "11")
                        str = str.Replace("@days2", "15")
                    Else
                        str = str.Replace("@days1", "16")
                        str = str.Replace("@days2", "4600")
                    End If
                    If HttpContext.Current.Session("USERROLE").ToString.ToUpper = "SU" Then
                    Else
                        str = str.Replace("eid=" & HttpContext.Current.Session("EID").ToString & "", "eid=" & HttpContext.Current.Session("EID").ToString & " " & HttpContext.Current.Session("FilterRole").ToString)
                    End If
                    oda.SelectCommand.CommandType = CommandType.Text
                    oda.SelectCommand.CommandText = str
                    oda.Fill(ds)
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

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Shared Function getSLAPerformanceDtl(Type As String, Doc As String, Name As String, Dtf As String, Sdate As String, Edate As String) As DGrid
        Dim jsonData As String = ""
        Dim grid As New DGrid()
        Try
            Dim ds As New DataSet()
            Dim Query As String = ""
            Dim dt As New DataTable
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Using con As New SqlConnection(conStr)
                Using oda As New SqlDataAdapter(Query, con)
                    Dim qry As String = ""
                    If Type.ToUpper = "DEPARTMENT" Then
                        qry = "SELECT firstlevelqry from mmm_mst_MISDB where eid=" & HttpContext.Current.Session("EID") & " and DBName='SLA Performance'"
                    Else
                        qry = "SELECT firstlevelqry from mmm_mst_MISDB where eid=" & HttpContext.Current.Session("EID") & " and DBName='SLA Performance'"
                    End If

                    oda.SelectCommand.CommandType = CommandType.Text
                    oda.SelectCommand.CommandText = qry
                    oda.Fill(dt)
                    Dim str = dt.Rows(0).Item(0).ToString
                    If Dtf.ToUpper = "CURRENT FY" Then
                        str = str.Replace("@Date", "convert(date,adate)>=(dateadd(year, datepart(year, getdate()-89) - 1900, '1900-04-01')) and convert(date,adate)<=(dateadd(day, -1, dateadd(month, 12, (dateadd(year, datepart(year, getdate()-89) - 1900, '1900-04-01')))))")
                    ElseIf Dtf.ToUpper = "LAST FY" Then
                        str = str.Replace("@Date", "convert(date,adate)>=dateadd(year, -1, (dateadd(year, datepart(year, getdate()-89) - 1900, '1900-04-01'))) and convert(date,adate)<=dateadd(year, -1, (dateadd(day, -1, dateadd(month, 12, (dateadd(year, datepart(year, getdate()-89) - 1900, '1900-04-01'))))))")
                    ElseIf Dtf.ToUpper = "LAST MONTH" Then
                        str = str.Replace("@Date", "convert(date,adate)>=(DATEADD(mm, DATEDIFF(mm, 0, GETDATE())-1, 0)) and convert(date,adate)<=(DATEADD(ms, -3, DATEADD(mm, DATEDIFF(m, 0, GETDATE()), 0)))")
                    ElseIf Dtf.ToUpper = "CURRENT MONTH" Then
                        str = str.Replace("@Date", "convert(date,adate)>=(DATEADD(mm, DATEDIFF(mm, 0, GETDATE()), 0)) and convert(date,adate)<=(DATEADD(ms, -3, DATEADD(mm, DATEDIFF(m, 0, GETDATE())+1, 0)))")
                    ElseIf Dtf.ToUpper = "CURRENT QUARTER" Then
                        str = str.Replace("@Date", "convert(date,adate)>=DATEADD(qq, DATEDIFF(qq, 0, GETDATE()), 0) and convert(date,adate)<=DATEADD (dd, -1, DATEADD(qq, DATEDIFF(qq, 0, GETDATE()) +1, 0))")
                    ElseIf Dtf.ToUpper = "LAST QUARTER" Then
                        str = str.Replace("@Date", "convert(date,adate)>=DATEADD(qq, DATEDIFF(qq, 0, GETDATE()) - 1, 0) and convert(date,adate)<=DATEADD (dd, -1, DATEADD(qq, DATEDIFF(qq, 0, GETDATE()), 0))")
                    Else
                        str = str.Replace("@Date", "convert(date,adate)>=convert(date,'" & Sdate.ToString & "')  and convert(date,adate)<=convert(date,'" & Edate.ToString & "')")
                    End If

                    If Doc.ToUpper = "VHD" Then
                        str = str.Replace("@status", "'UPLOADED'")
                    ElseIf Doc.ToUpper = "IHCL" Then
                        str = str.Replace("@status", "'Approver 1','Approver 2','Approver 3','Referral Recommendation','RR Entry'")
                    Else
                        str = str.Replace("@status", "'" & Doc & "'")
                    End If

                    If (Type.ToUpper = "DEPARTMENT") Or (Type.ToUpper = "ALL DEPARTMENTS") Then
                        str = str.Replace("@Dept", "d.tid=d.tid")
                    Else
                        str = str.Replace("@Dept", "(case when documenttype='invoice po' then dms.udf_split('MASTER-Department Master-fld2',fld65) when documenttype='invoice non po' then dms.udf_split('MASTER-Department Master-fld2',fld3)end)='" & Type.ToString & "'")
                    End If
                    'str = str.Replace("@Dept", "'" & Doc & "'")
                    'str = str.Replace("@dept", "'" & Doc & "'")
                    If Name.ToUpper = "WITHIN SLA" Then
                        str = str.Replace("@SLA", "ptat<=atat")
                    Else
                        str = str.Replace("@SLA", "ptat>atat")
                    End If
                    If HttpContext.Current.Session("USERROLE").ToString.ToUpper = "SU" Then
                    Else
                        str = str.Replace("eid=" & HttpContext.Current.Session("EID").ToString & "", "eid=" & HttpContext.Current.Session("EID").ToString & " " & HttpContext.Current.Session("FilterRole").ToString)
                    End If
                    oda.SelectCommand.CommandType = CommandType.Text
                    oda.SelectCommand.CommandText = str
                    oda.Fill(ds)
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

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Shared Function getExpenseBreakupAllDtl(Type As String, Dtf As String, Sdate As String, Edate As String) As DGrid
        Dim jsonData As String = ""
        Dim grid As New DGrid()
        Try
            Dim ds As New DataSet()
            Dim Query As String = ""
            Dim dt As New DataTable
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Using con As New SqlConnection(conStr)
                Using oda As New SqlDataAdapter(Query, con)
                    Dim qry As String = ""

                    qry = "SELECT AllDetailsqry from mmm_mst_MISDB where eid=" & HttpContext.Current.Session("EID") & " and DBName='Expense Breakup'"


                    oda.SelectCommand.CommandType = CommandType.Text
                    oda.SelectCommand.CommandText = qry
                    oda.Fill(dt)
                    Dim str = dt.Rows(0).Item(0).ToString
                    If Dtf.ToUpper = "CURRENT FY" Then
                        str = str.Replace("@Date", "convert(date,adate)>=(dateadd(year, datepart(year, getdate()-89) - 1900, '1900-04-01')) and convert(date,adate)<=(dateadd(day, -1, dateadd(month, 12, (dateadd(year, datepart(year, getdate()-89) - 1900, '1900-04-01')))))")
                    ElseIf Dtf.ToUpper = "LAST FY" Then
                        str = str.Replace("@Date", "convert(date,adate)>=dateadd(year, -1, (dateadd(year, datepart(year, getdate()-89) - 1900, '1900-04-01'))) and convert(date,adate)<=dateadd(year, -1, (dateadd(day, -1, dateadd(month, 12, (dateadd(year, datepart(year, getdate()-89) - 1900, '1900-04-01'))))))")
                    ElseIf Dtf.ToUpper = "LAST MONTH" Then
                        str = str.Replace("@Date", "convert(date,adate)>=(DATEADD(mm, DATEDIFF(mm, 0, GETDATE())-1, 0)) and convert(date,adate)<=(DATEADD(ms, -3, DATEADD(mm, DATEDIFF(m, 0, GETDATE()), 0)))")
                    ElseIf Dtf.ToUpper = "CURRENT MONTH" Then
                        str = str.Replace("@Date", "convert(date,adate)>=(DATEADD(mm, DATEDIFF(mm, 0, GETDATE()), 0)) and convert(date,adate)<=(DATEADD(ms, -3, DATEADD(mm, DATEDIFF(m, 0, GETDATE())+1, 0)))")
                    ElseIf Dtf.ToUpper = "CURRENT QUARTER" Then
                        str = str.Replace("@Date", "convert(date,adate)>=DATEADD(qq, DATEDIFF(qq, 0, GETDATE()), 0) and convert(date,adate)<=DATEADD (dd, -1, DATEADD(qq, DATEDIFF(qq, 0, GETDATE()) +1, 0))")
                    ElseIf Dtf.ToUpper = "LAST QUARTER" Then
                        str = str.Replace("@Date", "convert(date,adate)>=DATEADD(qq, DATEDIFF(qq, 0, GETDATE()) - 1, 0) and convert(date,adate)<=DATEADD (dd, -1, DATEADD(qq, DATEDIFF(qq, 0, GETDATE()), 0))")
                    Else
                        str = str.Replace("@Date", "convert(date,adate)>=convert(date,'" & Sdate.ToString & "')  and convert(date,adate)<=convert(date,'" & Edate.ToString & "')")
                    End If

                    If HttpContext.Current.Session("USERROLE").ToString.ToUpper = "SU" Then
                    Else
                        str = str.Replace("eid=" & HttpContext.Current.Session("EID").ToString & "", "eid=" & HttpContext.Current.Session("EID").ToString & " " & HttpContext.Current.Session("FilterRole").ToString)
                    End If
                    oda.SelectCommand.CommandType = CommandType.Text
                    oda.SelectCommand.CommandText = str
                    oda.Fill(ds)
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

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Shared Function getSupplierBreakupAllDtl(Type As String, Dtf As String, Sdate As String, Edate As String) As DGrid
        Dim jsonData As String = ""
        Dim grid As New DGrid()
        Try
            Dim ds As New DataSet()
            Dim Query As String = ""
            Dim dt As New DataTable
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Using con As New SqlConnection(conStr)
                Using oda As New SqlDataAdapter(Query, con)
                    Dim qry As String = ""

                    qry = "SELECT AllDetailsqry from mmm_mst_MISDB where eid=" & HttpContext.Current.Session("EID") & " and DBName='Supplier Spend Breakup'"


                    oda.SelectCommand.CommandType = CommandType.Text
                    oda.SelectCommand.CommandText = qry
                    oda.Fill(dt)
                    Dim str = dt.Rows(0).Item(0).ToString
                    If Dtf.ToUpper = "CURRENT FY" Then
                        str = str.Replace("@Date", "convert(date,adate)>=(dateadd(year, datepart(year, getdate()-89) - 1900, '1900-04-01')) and convert(date,adate)<=(dateadd(day, -1, dateadd(month, 12, (dateadd(year, datepart(year, getdate()-89) - 1900, '1900-04-01')))))")
                    ElseIf Dtf.ToUpper = "LAST FY" Then
                        str = str.Replace("@Date", "convert(date,adate)>=dateadd(year, -1, (dateadd(year, datepart(year, getdate()-89) - 1900, '1900-04-01'))) and convert(date,adate)<=dateadd(year, -1, (dateadd(day, -1, dateadd(month, 12, (dateadd(year, datepart(year, getdate()-89) - 1900, '1900-04-01'))))))")
                    ElseIf Dtf.ToUpper = "LAST MONTH" Then
                        str = str.Replace("@Date", "convert(date,adate)>=(DATEADD(mm, DATEDIFF(mm, 0, GETDATE())-1, 0)) and convert(date,adate)<=(DATEADD(ms, -3, DATEADD(mm, DATEDIFF(m, 0, GETDATE()), 0)))")
                    ElseIf Dtf.ToUpper = "CURRENT MONTH" Then
                        str = str.Replace("@Date", "convert(date,adate)>=(DATEADD(mm, DATEDIFF(mm, 0, GETDATE()), 0)) and convert(date,adate)<=(DATEADD(ms, -3, DATEADD(mm, DATEDIFF(m, 0, GETDATE())+1, 0)))")
                    ElseIf Dtf.ToUpper = "CURRENT QUARTER" Then
                        str = str.Replace("@Date", "convert(date,adate)>=DATEADD(qq, DATEDIFF(qq, 0, GETDATE()), 0) and convert(date,adate)<=DATEADD (dd, -1, DATEADD(qq, DATEDIFF(qq, 0, GETDATE()) +1, 0))")
                    ElseIf Dtf.ToUpper = "LAST QUARTER" Then
                        str = str.Replace("@Date", "convert(date,adate)>=DATEADD(qq, DATEDIFF(qq, 0, GETDATE()) - 1, 0) and convert(date,adate)<=DATEADD (dd, -1, DATEADD(qq, DATEDIFF(qq, 0, GETDATE()), 0))")
                    Else
                        str = str.Replace("@Date", "convert(date,adate)>=convert(date,'" & Sdate.ToString & "')  and convert(date,adate)<=convert(date,'" & Edate.ToString & "')")
                    End If

                    If HttpContext.Current.Session("USERROLE").ToString.ToUpper = "SU" Then
                    Else
                        str = str.Replace("eid=" & HttpContext.Current.Session("EID").ToString & "", "eid=" & HttpContext.Current.Session("EID").ToString & " " & HttpContext.Current.Session("FilterRole").ToString)
                    End If
                    oda.SelectCommand.CommandType = CommandType.Text
                    oda.SelectCommand.CommandText = str
                    oda.Fill(ds)
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

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Shared Function getInvoiceDistAllDtl(Type As String, Dtf As String, Sdate As String, Edate As String) As DGrid
        Dim jsonData As String = ""
        Dim grid As New DGrid()
        Try
            Dim ds As New DataSet()
            Dim Query As String = ""
            Dim dt As New DataTable
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Using con As New SqlConnection(conStr)
                Using oda As New SqlDataAdapter(Query, con)
                    Dim qry As String = ""

                    qry = "SELECT AllDetailsqry from mmm_mst_MISDB where eid=" & HttpContext.Current.Session("EID") & " and DBName='Invoice Distribution'"


                    oda.SelectCommand.CommandType = CommandType.Text
                    oda.SelectCommand.CommandText = qry
                    oda.Fill(dt)
                    Dim str = dt.Rows(0).Item(0).ToString
                    If Dtf.ToUpper = "CURRENT FY" Then
                        str = str.Replace("@Date", "convert(date,adate)>=(dateadd(year, datepart(year, getdate()-89) - 1900, '1900-04-01')) and convert(date,adate)<=(dateadd(day, -1, dateadd(month, 12, (dateadd(year, datepart(year, getdate()-89) - 1900, '1900-04-01')))))")
                    ElseIf Dtf.ToUpper = "LAST FY" Then
                        str = str.Replace("@Date", "convert(date,adate)>=dateadd(year, -1, (dateadd(year, datepart(year, getdate()-89) - 1900, '1900-04-01'))) and convert(date,adate)<=dateadd(year, -1, (dateadd(day, -1, dateadd(month, 12, (dateadd(year, datepart(year, getdate()-89) - 1900, '1900-04-01'))))))")
                    ElseIf Dtf.ToUpper = "LAST MONTH" Then
                        str = str.Replace("@Date", "convert(date,adate)>=(DATEADD(mm, DATEDIFF(mm, 0, GETDATE())-1, 0)) and convert(date,adate)<=(DATEADD(ms, -3, DATEADD(mm, DATEDIFF(m, 0, GETDATE()), 0)))")
                    ElseIf Dtf.ToUpper = "CURRENT MONTH" Then
                        str = str.Replace("@Date", "convert(date,adate)>=(DATEADD(mm, DATEDIFF(mm, 0, GETDATE()), 0)) and convert(date,adate)<=(DATEADD(ms, -3, DATEADD(mm, DATEDIFF(m, 0, GETDATE())+1, 0)))")
                    ElseIf Dtf.ToUpper = "CURRENT QUARTER" Then
                        str = str.Replace("@Date", "convert(date,adate)>=DATEADD(qq, DATEDIFF(qq, 0, GETDATE()), 0) and convert(date,adate)<=DATEADD (dd, -1, DATEADD(qq, DATEDIFF(qq, 0, GETDATE()) +1, 0))")
                    ElseIf Dtf.ToUpper = "LAST QUARTER" Then
                        str = str.Replace("@Date", "convert(date,adate)>=DATEADD(qq, DATEDIFF(qq, 0, GETDATE()) - 1, 0) and convert(date,adate)<=DATEADD (dd, -1, DATEADD(qq, DATEDIFF(qq, 0, GETDATE()), 0))")
                    Else
                        str = str.Replace("@Date", "convert(date,adate)>=convert(date,'" & Sdate.ToString & "')  and convert(date,adate)<=convert(date,'" & Edate.ToString & "')")
                    End If

                    If HttpContext.Current.Session("USERROLE").ToString.ToUpper = "SU" Then
                    Else
                        str = str.Replace("eid=" & HttpContext.Current.Session("EID").ToString & "", "eid=" & HttpContext.Current.Session("EID").ToString & " " & HttpContext.Current.Session("FilterRole").ToString)
                    End If
                    oda.SelectCommand.CommandType = CommandType.Text
                    oda.SelectCommand.CommandText = str
                    oda.Fill(ds)
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

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Shared Function getInvoiceLifeCycleAllDtl(Type As String, Dtf As String, Sdate As String, Edate As String) As DGrid
        Dim jsonData As String = ""
        Dim grid As New DGrid()
        Try
            Dim ds As New DataSet()
            Dim Query As String = ""
            Dim dt As New DataTable
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Using con As New SqlConnection(conStr)
                Using oda As New SqlDataAdapter(Query, con)
                    Dim qry As String = ""

                    qry = "SELECT AllDetailsqry from mmm_mst_MISDB where eid=" & HttpContext.Current.Session("EID") & " and DBName='Invoice LifeCycle'"


                    oda.SelectCommand.CommandType = CommandType.Text
                    oda.SelectCommand.CommandText = qry
                    oda.Fill(dt)
                    Dim str = dt.Rows(0).Item(0).ToString
                    If Dtf.ToUpper = "CURRENT FY" Then
                        str = str.Replace("@Date", "convert(date,adate)>=(dateadd(year, datepart(year, getdate()-89) - 1900, '1900-04-01')) and convert(date,adate)<=(dateadd(day, -1, dateadd(month, 12, (dateadd(year, datepart(year, getdate()-89) - 1900, '1900-04-01')))))")
                    ElseIf Dtf.ToUpper = "LAST FY" Then
                        str = str.Replace("@Date", "convert(date,adate)>=dateadd(year, -1, (dateadd(year, datepart(year, getdate()-89) - 1900, '1900-04-01'))) and convert(date,adate)<=dateadd(year, -1, (dateadd(day, -1, dateadd(month, 12, (dateadd(year, datepart(year, getdate()-89) - 1900, '1900-04-01'))))))")
                    ElseIf Dtf.ToUpper = "LAST MONTH" Then
                        str = str.Replace("@Date", "convert(date,adate)>=(DATEADD(mm, DATEDIFF(mm, 0, GETDATE())-1, 0)) and convert(date,adate)<=(DATEADD(ms, -3, DATEADD(mm, DATEDIFF(m, 0, GETDATE()), 0)))")
                    ElseIf Dtf.ToUpper = "CURRENT MONTH" Then
                        str = str.Replace("@Date", "convert(date,adate)>=(DATEADD(mm, DATEDIFF(mm, 0, GETDATE()), 0)) and convert(date,adate)<=(DATEADD(ms, -3, DATEADD(mm, DATEDIFF(m, 0, GETDATE())+1, 0)))")
                    ElseIf Dtf.ToUpper = "CURRENT QUARTER" Then
                        str = str.Replace("@Date", "convert(date,adate)>=DATEADD(qq, DATEDIFF(qq, 0, GETDATE()), 0) and convert(date,adate)<=DATEADD (dd, -1, DATEADD(qq, DATEDIFF(qq, 0, GETDATE()) +1, 0))")
                    ElseIf Dtf.ToUpper = "LAST QUARTER" Then
                        str = str.Replace("@Date", "convert(date,adate)>=DATEADD(qq, DATEDIFF(qq, 0, GETDATE()) - 1, 0) and convert(date,adate)<=DATEADD (dd, -1, DATEADD(qq, DATEDIFF(qq, 0, GETDATE()), 0))")
                    Else
                        str = str.Replace("@Date", "convert(date,adate)>=convert(date,'" & Sdate.ToString & "')  and convert(date,adate)<=convert(date,'" & Edate.ToString & "')")
                    End If

                    If HttpContext.Current.Session("USERROLE").ToString.ToUpper = "SU" Then
                    Else
                        str = str.Replace("eid=" & HttpContext.Current.Session("EID").ToString & "", "eid=" & HttpContext.Current.Session("EID").ToString & " " & HttpContext.Current.Session("FilterRole").ToString)
                    End If
                    oda.SelectCommand.CommandType = CommandType.Text
                    oda.SelectCommand.CommandText = str
                    oda.Fill(ds)
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

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Shared Function getOpenInvoiceAgeingAllDtl(Type As String, Dtf As String, Sdate As String, Edate As String) As DGrid
        Dim jsonData As String = ""
        Dim grid As New DGrid()
        Try
            Dim ds As New DataSet()
            Dim Query As String = ""
            Dim dt As New DataTable
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Using con As New SqlConnection(conStr)
                Using oda As New SqlDataAdapter(Query, con)
                    Dim qry As String = ""
                    qry = "SELECT AllDetailsqry from mmm_mst_MISDB where eid=" & HttpContext.Current.Session("EID") & " and DBName='Open Invoice Ageing'"
                    oda.SelectCommand.CommandType = CommandType.Text
                    oda.SelectCommand.CommandText = qry
                    oda.Fill(dt)
                    Dim str = dt.Rows(0).Item(0).ToString
                    If HttpContext.Current.Session("USERROLE").ToString.ToUpper = "SU" Then
                    Else
                        str = str.Replace("eid=" & HttpContext.Current.Session("EID").ToString & "", "eid=" & HttpContext.Current.Session("EID").ToString & " " & HttpContext.Current.Session("FilterRole").ToString)
                    End If
                    oda.SelectCommand.CommandType = CommandType.Text
                    oda.SelectCommand.CommandText = str
                    oda.Fill(ds)
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

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Shared Function getSLAPerformanceAllDtl(Type As String, Dtf As String, Sdate As String, Edate As String) As DGrid
        Dim jsonData As String = ""
        Dim grid As New DGrid()
        Try
            Dim ds As New DataSet()
            Dim Query As String = ""
            Dim dt As New DataTable
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Using con As New SqlConnection(conStr)
                Using oda As New SqlDataAdapter(Query, con)
                    Dim qry As String = ""

                    qry = "SELECT AllDetailsqry from mmm_mst_MISDB where eid=" & HttpContext.Current.Session("EID") & " and DBName='SLA Performance'"


                    oda.SelectCommand.CommandType = CommandType.Text
                    oda.SelectCommand.CommandText = qry
                    oda.Fill(dt)
                    Dim str = dt.Rows(0).Item(0).ToString
                    If Dtf.ToUpper = "CURRENT FY" Then
                        str = str.Replace("@Date", "convert(date,adate)>=(dateadd(year, datepart(year, getdate()-89) - 1900, '1900-04-01')) and convert(date,adate)<=(dateadd(day, -1, dateadd(month, 12, (dateadd(year, datepart(year, getdate()-89) - 1900, '1900-04-01')))))")
                    ElseIf Dtf.ToUpper = "LAST FY" Then
                        str = str.Replace("@Date", "convert(date,adate)>=dateadd(year, -1, (dateadd(year, datepart(year, getdate()-89) - 1900, '1900-04-01'))) and convert(date,adate)<=dateadd(year, -1, (dateadd(day, -1, dateadd(month, 12, (dateadd(year, datepart(year, getdate()-89) - 1900, '1900-04-01'))))))")
                    ElseIf Dtf.ToUpper = "LAST MONTH" Then
                        str = str.Replace("@Date", "convert(date,adate)>=(DATEADD(mm, DATEDIFF(mm, 0, GETDATE())-1, 0)) and convert(date,adate)<=(DATEADD(ms, -3, DATEADD(mm, DATEDIFF(m, 0, GETDATE()), 0)))")
                    ElseIf Dtf.ToUpper = "CURRENT MONTH" Then
                        str = str.Replace("@Date", "convert(date,adate)>=(DATEADD(mm, DATEDIFF(mm, 0, GETDATE()), 0)) and convert(date,adate)<=(DATEADD(ms, -3, DATEADD(mm, DATEDIFF(m, 0, GETDATE())+1, 0)))")
                    ElseIf Dtf.ToUpper = "CURRENT QUARTER" Then
                        str = str.Replace("@Date", "convert(date,adate)>=DATEADD(qq, DATEDIFF(qq, 0, GETDATE()), 0) and convert(date,adate)<=DATEADD (dd, -1, DATEADD(qq, DATEDIFF(qq, 0, GETDATE()) +1, 0))")
                    ElseIf Dtf.ToUpper = "LAST QUARTER" Then
                        str = str.Replace("@Date", "convert(date,adate)>=DATEADD(qq, DATEDIFF(qq, 0, GETDATE()) - 1, 0) and convert(date,adate)<=DATEADD (dd, -1, DATEADD(qq, DATEDIFF(qq, 0, GETDATE()), 0))")
                    Else
                        str = str.Replace("@Date", "convert(date,adate)>=convert(date,'" & Sdate.ToString & "')  and convert(date,adate)<=convert(date,'" & Edate.ToString & "')")
                    End If

                    If HttpContext.Current.Session("USERROLE").ToString.ToUpper = "SU" Then
                    Else
                        str = str.Replace("eid=" & HttpContext.Current.Session("EID").ToString & "", "eid=" & HttpContext.Current.Session("EID").ToString & " " & HttpContext.Current.Session("FilterRole").ToString)
                    End If
                    oda.SelectCommand.CommandType = CommandType.Text
                    oda.SelectCommand.CommandText = str
                    oda.Fill(ds)
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

    ' <WebMethod()>
    '<Script.Services.ScriptMethod()>
    ' Public Shared Function GetDataCountInvoiceLifeCycle() As String
    '     Dim jsonData As String = ""
    '     Try
    '         Dim ds As New DataSet()
    '         Dim UID As Integer = 0
    '         Dim URole As String = ""
    '         Dim qry As String = ""
    '         Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
    '         Dim con As New SqlConnection(conStr)
    '         Dim dt As New DataTable
    '         Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)

    '         'qry = "select distinct case when documenttype='invoice po' then fld65 when documenttype='invoice non po' then fld3 end[Value],case when documenttype='invoice po' then dms.udf_split('MASTER-Department Master-fld2',fld65) when documenttype='invoice non po' then dms.udf_split('MASTER-Department Master-fld2',fld3)end[Display]  from mmm_mst_doc  where eid=152 and documenttype in ('invoice po','Invoice non po')"
    '         qry = "select count(tid)[Count] from mmm_mst_doc d where eid=152 and documenttype in ('invoice PO','invoice non PO') and curstatus='archive' and datediff(dd,convert(date,case when documenttype='Invoice PO' then fld85 when documenttype='Invoice Non PO' then fld87 end ,3),(select max(fdate) from mmm_doc_dtl with(nolock) where docid=d.tid ))>=0 and datediff(dd,convert(date,case when documenttype='Invoice PO' then fld85 when documenttype='Invoice Non PO' then fld87 end ,3),(select max(fdate) from mmm_doc_dtl with(nolock) where docid=d.tid ))<=15  and convert(date,adate)>=(DATEADD(mm, DATEDIFF(mm, 0, GETDATE()), 0)) and convert(date,adate)<=(DATEADD(ms, -3, DATEADD(mm, DATEDIFF(m, 0, GETDATE())+1, 0))) and (dms.udf_split('MASTER-Department Master-fld2',fld65)='Information Systems' or dms.udf_split('MASTER-Department Master-fld2',fld3)='Information Systems')"

    '         oda.SelectCommand.CommandType = CommandType.Text
    '         oda.SelectCommand.CommandText = qry '"select  top 5 dms.udf_split('MASTER-Doc Nature Master-fld1',fld4)[category],convert(numeric(10,2),sum(convert(numeric(10,2),fld37))/1000000)[value] from mmm_mst_doc where eid=152 and documenttype in ('invoice non po') and  convert(date,adate)>=(DATEADD(mm, DATEDIFF(mm, 0, GETDATE()), 0)) and convert(date,adate)<=(DATEADD(ms, -3, DATEADD(mm, DATEDIFF(m, 0, GETDATE()) + 1, 0))) and curstatus<>'rejected' group by dms.udf_split('MASTER-Doc Nature Master-fld1',fld4) order by sum(convert(numeric(10,2),fld37)) desc"

    '         oda.Fill(dt)
    '         con.Close()
    '         dt.Dispose()
    '         'jsonData = dt.Rows(0).Item(0).ToString
    '         Dim lstColumns As New List(Of String)
    '         Dim serializerSettings As New JsonSerializerSettings()
    '         Dim json_serializer As New JavaScriptSerializer()
    '         serializerSettings.Converters.Add(New DataTableConverter())
    '         jsonData = JsonConvert.SerializeObject(dt, Newtonsoft.Json.Formatting.None, serializerSettings)
    '     Catch Ex As Exception
    '         Throw
    '     End Try
    '     Return jsonData
    ' End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Shared Function GetSLADepartments() As String
        Dim jsonData As String = ""
        Try
            Dim ds As New DataSet()
            Dim UID As Integer = 0
            Dim URole As String = ""
            Dim qry As String = ""
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As New SqlConnection(conStr)
            Dim dt As New DataTable
            Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)

            qry = "select distinct case when documenttype='invoice po' then fld65 when documenttype='invoice non po' then fld3 end[Value],case when documenttype='invoice po' then dms.udf_split('MASTER-Department Master-fld2',fld65) when documenttype='invoice non po' then dms.udf_split('MASTER-Department Master-fld2',fld3)end[Display]  from mmm_mst_doc  where eid=152 and documenttype in ('invoice po','Invoice non po')"
            

            oda.SelectCommand.CommandType = CommandType.Text
            oda.SelectCommand.CommandText = qry '"select  top 5 dms.udf_split('MASTER-Doc Nature Master-fld1',fld4)[category],convert(numeric(10,2),sum(convert(numeric(10,2),fld37))/1000000)[value] from mmm_mst_doc where eid=152 and documenttype in ('invoice non po') and  convert(date,adate)>=(DATEADD(mm, DATEDIFF(mm, 0, GETDATE()), 0)) and convert(date,adate)<=(DATEADD(ms, -3, DATEADD(mm, DATEDIFF(m, 0, GETDATE()) + 1, 0))) and curstatus<>'rejected' group by dms.udf_split('MASTER-Doc Nature Master-fld1',fld4) order by sum(convert(numeric(10,2),fld37)) desc"

            oda.Fill(dt)
            con.Close()
            dt.Dispose()
            Dim lstColumns As New List(Of String)
            Dim serializerSettings As New JsonSerializerSettings()
            Dim json_serializer As New JavaScriptSerializer()
            serializerSettings.Converters.Add(New DataTableConverter())
            jsonData = JsonConvert.SerializeObject(dt, Newtonsoft.Json.Formatting.None, serializerSettings)
        Catch Ex As Exception
            Throw
        End Try
        Return jsonData
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Shared Function GetAgeingDepartments() As String
        Dim jsonData As String = ""
        Try
            Dim ds As New DataSet()
            Dim UID As Integer = 0
            Dim URole As String = ""
            Dim qry As String = ""
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As New SqlConnection(conStr)
            Dim dt As New DataTable
            Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)

            qry = "select distinct case when documenttype='invoice po' then fld65 when documenttype='invoice non po' then fld3 end[Value],case when documenttype='invoice po' then dms.udf_split('MASTER-Department Master-fld2',fld65) when documenttype='invoice non po' then dms.udf_split('MASTER-Department Master-fld2',fld3)end[Display]  from mmm_mst_doc  where eid=152 and documenttype in ('invoice po','Invoice non po')"


            oda.SelectCommand.CommandType = CommandType.Text
            oda.SelectCommand.CommandText = qry '"select  top 5 dms.udf_split('MASTER-Doc Nature Master-fld1',fld4)[category],convert(numeric(10,2),sum(convert(numeric(10,2),fld37))/1000000)[value] from mmm_mst_doc where eid=152 and documenttype in ('invoice non po') and  convert(date,adate)>=(DATEADD(mm, DATEDIFF(mm, 0, GETDATE()), 0)) and convert(date,adate)<=(DATEADD(ms, -3, DATEADD(mm, DATEDIFF(m, 0, GETDATE()) + 1, 0))) and curstatus<>'rejected' group by dms.udf_split('MASTER-Doc Nature Master-fld1',fld4) order by sum(convert(numeric(10,2),fld37)) desc"

            oda.Fill(dt)
            con.Close()
            dt.Dispose()
            Dim lstColumns As New List(Of String)
            Dim serializerSettings As New JsonSerializerSettings()
            Dim json_serializer As New JavaScriptSerializer()
            serializerSettings.Converters.Add(New DataTableConverter())
            jsonData = JsonConvert.SerializeObject(dt, Newtonsoft.Json.Formatting.None, serializerSettings)
        Catch Ex As Exception
            Throw
        End Try
        Return jsonData
    End Function

    Public Class vdashboard
        Public Property series As List(Of series)
        Public Property countseries As List(Of series1)
        Public Property categoryAxis As List(Of String)
        Public Property HasSession As String
    End Class
    Public Class series
        Public Property name As String
        Public Property data As List(Of Integer)
        Public Property color As String
    End Class
    Public Class series1
        Public Property name As String
        Public Property data As List(Of Integer)
        Public Property color As String
    End Class
    Public Class categoryAxisTkm
        Public Property categories As List(Of String)
    End Class
End Class
