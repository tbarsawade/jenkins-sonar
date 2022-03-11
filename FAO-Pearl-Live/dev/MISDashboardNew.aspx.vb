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
Partial Class MISDashboardNew
    Inherits System.Web.UI.Page
    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            ' session("eid") = "0"
            Try

                Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
                Dim con As New SqlConnection(conStr)
                Dim str As String = ""
                'fill Product 
                If Session("USERROLE").ToString.ToUpper = "SU" Then
                Else
                    Dim da As New SqlDataAdapter("select FormName,DocMapping from mmm_mst_forms where DocMapping is not null and eid=" & Session("EID") & "", con)
                    Dim ds As New DataSet
                    da.Fill(ds, "data")
                    For j As Integer = 0 To ds.Tables("data").Rows.Count - 1
                        da.SelectCommand.CommandText = "select " & ds.Tables("data").Rows(j).Item("DocMapping") & " from MMM_Ref_Role_User where eid=" & Session("EID") & " and uid=" & Session("UID") & " and rolename in ('" & Session("USERROLE") & "') "
                        Dim id As String = ""
                        If con.State <> ConnectionState.Open Then
                            con.Open()
                        End If
                        da.SelectCommand.CommandType = CommandType.Text
                        id = da.SelectCommand.ExecuteScalar().ToString
                        Dim fltrstr As String = ""
                        da.SelectCommand.CommandText = "select FieldMapping,documenttype from MMM_MST_FIELDS where eid=" & Session("EID") & " and dropdown like '%" & ds.Tables("data").Rows(j).Item(0).ToString & "%' and documenttype in (select distinct formname from mmm_mst_forms where eid=" & Session("EID") & " and formtype='document' and formsource='menu driven' and isactive=1)"
                        'da.SelectCommand.CommandText = "select FieldMapping,documenttype from MMM_MST_FIELDS where eid=" & Session("EID") & " and dropdown like '%Sub department%' and documenttype in (select distinct formname from mmm_mst_forms where eid=" & Session("EID") & " and formtype='document' and formsource='menu driven' and isactive=1)"
                        da.Fill(ds, "data1")
                        If ds.Tables("data1").Rows.Count > 0 Then
                            For i As Integer = 0 To ds.Tables("data1").Rows.Count - 1
                                fltrstr = fltrstr & " when documenttype='" & ds.Tables("data1").Rows(i).Item("documenttype").ToString & "' then " & ds.Tables("data1").Rows(i).Item("FieldMapping").ToString & ""
                            Next
                        End If
                        If j <> ds.Tables("data").Rows.Count - 1 Then
                            str = str & "(case " & fltrstr & " end  in (" & id.ToString & ") or "
                        ElseIf str.Length > 1 Then
                            str = str & " case " & fltrstr & " end  in (" & id.ToString & "))"
                        Else
                            str = str & " (case " & fltrstr & " end  in (" & id.ToString & "))"
                        End If
                    Next
                    str = " and " & str
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
    Public Shared Function GetDataExpenseBreakup(Type As String, Dtf As String, Sdate As String, Edate As String, All As String) As String
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

                If All.ToUpper.ToString = "ALL" Then
                    str = str.Replace("top 5", " ")
                    str = str.Replace("Top 5", " ")
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
    Public Shared Function GetDataSuppSpendBreakup(Dtf As String, Sdate As String, Edate As String, All As String) As String
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
                If All.ToUpper.ToString = "ALL" Then
                    str = str.Replace("top 5", " top 20 ")
                    str = str.Replace("Top 5", " top 20 ")
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
    Public Shared Function GetDataSLAPerformance(Type As String, Dtf As String, Status As String, Sdate As String, Edate As String, All As String) As String
        Dim jsonData As String = ""
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
                        Dim dtlDeptquery = "select DocumentType,Type,catField,ValueField,RecdateField,InvDateField from mmm_mst_misdb_dtl where RefTid=(select tid from mmm_mst_misdb where DBName='SLA Performance' and eid=" & HttpContext.Current.Session("EID") & ")"
                        Dim tabledata As New DataTable
                        Dim SETDATA As New DataSet
                        Dim ANDDEPT As String = ""
                        Dim newQueryforall1 As String = ""
                        Dim CDATA As New DataSet
                        oda.SelectCommand.CommandType = CommandType.Text
                        oda.SelectCommand.CommandText = dtlDeptquery
                        oda.Fill(SETDATA)
                        If (SETDATA.Tables(0).Rows.Count > 0) Then
                            For MI As Integer = 0 To SETDATA.Tables(0).Rows.Count - 1
                                Dim documenttypAll = SETDATA.Tables(0).Rows(MI).Item("DocumentType")
                                Dim CatogoryField = SETDATA.Tables(0).Rows(MI).Item("catField")
                                newQueryforall1 = "select FieldID,FieldType,documenttype,DROPDOWNTYPE,dropdown,FieldMapping,displayName,Datatype from mmm_mst_fields where FieldMapping='" & CatogoryField & "' and DocumentType='" & documenttypAll & "' and Eid=" & HttpContext.Current.Session("EID") & " and isactive=1"
                                oda.SelectCommand.CommandType = CommandType.Text
                                oda.SelectCommand.CommandText = newQueryforall1
                                oda.Fill(CDATA)
                                If (CDATA.Tables(0).Rows.Count > 0) Then
                                    If (CDATA.Tables(0).Rows(MI).Item("DROPDOWNTYPE") = "MASTER VALUED") Then
                                        ANDDEPT = ANDDEPT & " when documenttype= '" & SETDATA.Tables(0).Rows(MI).Item("DocumentType").ToString & "'  then  dms.udf_split('" & CDATA.Tables(0).Rows(MI).Item("dropdown") & "'," & CatogoryField & ") "
                                    End If
                                End If
                            Next
                            ANDDEPT = " (case " & ANDDEPT & " end) ='" & Type.ToString & "' "
                        End If
                        str = str.Replace("@Dept", "" & ANDDEPT & "")
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

                    If All.ToUpper.ToString = "ALL" Then
                        str = str.Replace("top 5", " ")
                        str = str.Replace("Top 5", " ")
                    End If

                    oda.SelectCommand.CommandType = CommandType.Text
                    oda.SelectCommand.CommandText = str
                    'oda.SelectCommand.CommandText = "Select 'TCS'[category],'48000'[value] union select 'CGI','37000' union select 'IBM','26000' union select 'Infosys','25000' union select 'Wipro','25000'  "
                    oda.Fill(ds, "data")


                    oda.Fill(dt)
                    dt.Dispose()
                    Dim lstColumns As New List(Of String)
                    Dim serializerSettings As New JsonSerializerSettings()
                    Dim json_serializer As New JavaScriptSerializer()
                    serializerSettings.Converters.Add(New DataTableConverter())
                    jsonData = JsonConvert.SerializeObject(dt, Newtonsoft.Json.Formatting.None, serializerSettings)
                End If
            End Using
        Catch ex As Exception
            Throw
        Finally
            ds.Dispose()
            dt.Dispose()
        End Try
        Return jsonData

    End Function
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Shared Function GetDataInvoiceLifeCycle(Type As String, Dtf As String, Invdt As String, Sdate As String, Edate As String, All As String) As String
        Dim jsonData As String = ""
        Dim ds As New DataSet
        Dim dt As New DataTable
        Dim dt1 As New DataTable
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim Res As New vdashboard()
        Try
            Using oda As New SqlDataAdapter("", conStr)
                Dim qry As String = ""
                If (Type.ToUpper = "DEPARTMENT") Then
                    qry = "SELECT rootqry_D,RootqryFilter_D from mmm_mst_misdb where eid=" & HttpContext.Current.Session("EID") & " and DBName='Invoice LifeCycle'"
                Else
                    qry = "SELECT rootquery_e,RootqryFilter_E from mmm_mst_misdb where eid=" & HttpContext.Current.Session("EID") & " and DBName='Invoice LifeCycle'"
                End If
                oda.SelectCommand.CommandType = CommandType.Text
                oda.SelectCommand.CommandText = qry
                oda.Fill(ds, "qry")
                If ds.Tables("qry").Rows.Count > 0 Then
                    Dim str As String = ds.Tables("qry").Rows(0).Item(0).ToString()
                    Dim fltr As String = ds.Tables("qry").Rows(0).Item(1).ToString()
                    'Dim str As String = "select category,Days[DaysType],convert(numeric(10,2),sum(convert(numeric(10,2),Amount))/1000000)[Amount],count(tid)[Count]  from (select tid,case when documenttype='invoice po' then dms.udf_split('MASTER-Department Master-fld2',fld65) when documenttype='invoice non po' then dms.udf_split('MASTER-Department Master-fld2',fld3) end[category],case when documenttype='Invoice PO' then max(fld92) when documenttype='Invoice non PO' then max(fld37) end [Amount],case when datediff(dd,convert(date,case when documenttype='Invoice PO' then fld85 when documenttype='Invoice Non PO' then fld87 end ,3),(select max(fdate) from mmm_doc_dtl with(nolock) where docid=d.tid ))<=15 then '0-15 Days' when datediff(dd,convert(date,case when documenttype='Invoice PO' then fld85 when documenttype='Invoice Non PO' then fld87 end ,3),(select max(fdate) from mmm_doc_dtl with(nolock) where docid=d.tid ))>=16 and datediff(dd,convert(date,case when documenttype='Invoice PO' then fld85 when documenttype='Invoice Non PO' then fld87 end ,3),(select max(fdate) from mmm_doc_dtl with(nolock) where docid=d.tid ))<=30 then '16-30 Days' when datediff(dd,convert(date,case when documenttype='Invoice PO' then fld85 when documenttype='Invoice Non PO' then fld87 end ,3),(select max(fdate) from mmm_doc_dtl with(nolock) where docid=d.tid ))>=31 and datediff(dd,convert(date,case when documenttype='Invoice PO' then fld85 when documenttype='Invoice Non PO' then fld87 end ,3),(select max(fdate) from mmm_doc_dtl with(nolock) where docid=d.tid ))<=45 then '31-45 Days' when datediff(dd,convert(date,case when documenttype='Invoice PO' then fld85 when documenttype='Invoice Non PO' then fld87 end ,3),(select max(fdate) from mmm_doc_dtl with(nolock) where docid=d.tid ))>=46 and datediff(dd,convert(date,case when documenttype='Invoice PO' then fld85 when documenttype='Invoice Non PO' then fld87 end ,3),(select max(fdate) from mmm_doc_dtl with(nolock) where docid=d.tid ))<=3000 then '45+ Days' end [Days] from mmm_mst_doc d where eid=152 and documenttype in ('Invoice PO','Invoice Non PO') and curstatus in ('archive') and @Date  group by d.tid,documenttype,fld85,fld87,fld65,fld3) as t where t.category in ('Finance','administration','Information Systems','Sales and Marketing','Human Resources and Training') group by category,days order by DaysType desc"
                    If Dtf.ToUpper = "CURRENT FY" Then
                        str = str.Replace("@Date", "convert(date,adate)>=(dateadd(year, datepart(year, getdate()-89) - 1900, '1900-04-01')) and convert(date,adate)<=(dateadd(day, -1, dateadd(month, 12, (dateadd(year, datepart(year, getdate()-89) - 1900, '1900-04-01')))))")
                        fltr = fltr.Replace("@Date", "convert(date,adate)>=(dateadd(year, datepart(year, getdate()-89) - 1900, '1900-04-01')) and convert(date,adate)<=(dateadd(day, -1, dateadd(month, 12, (dateadd(year, datepart(year, getdate()-89) - 1900, '1900-04-01')))))")
                    ElseIf Dtf.ToUpper = "LAST FY" Then
                        str = str.Replace("@Date", "convert(date,adate)>=dateadd(year, -1, (dateadd(year, datepart(year, getdate()-89) - 1900, '1900-04-01'))) and convert(date,adate)<=dateadd(year, -1, (dateadd(day, -1, dateadd(month, 12, (dateadd(year, datepart(year, getdate()-89) - 1900, '1900-04-01'))))))")
                        fltr = fltr.Replace("@Date", "convert(date,adate)>=dateadd(year, -1, (dateadd(year, datepart(year, getdate()-89) - 1900, '1900-04-01'))) and convert(date,adate)<=dateadd(year, -1, (dateadd(day, -1, dateadd(month, 12, (dateadd(year, datepart(year, getdate()-89) - 1900, '1900-04-01'))))))")
                    ElseIf Dtf.ToUpper = "LAST MONTH" Then
                        str = str.Replace("@Date", "convert(date,adate)>=(DATEADD(mm, DATEDIFF(mm, 0, GETDATE())-1, 0)) and convert(date,adate)<=(DATEADD(ms, -3, DATEADD(mm, DATEDIFF(m, 0, GETDATE()), 0)))")
                        fltr = fltr.Replace("@Date", "convert(date,adate)>=(DATEADD(mm, DATEDIFF(mm, 0, GETDATE())-1, 0)) and convert(date,adate)<=(DATEADD(ms, -3, DATEADD(mm, DATEDIFF(m, 0, GETDATE()), 0)))")
                    ElseIf Dtf.ToUpper = "CURRENT MONTH" Then
                        str = str.Replace("@Date", "convert(date,adate)>=(DATEADD(mm, DATEDIFF(mm, 0, GETDATE()), 0)) and convert(date,adate)<=(DATEADD(ms, -3, DATEADD(mm, DATEDIFF(m, 0, GETDATE())+1, 0)))")
                        fltr = fltr.Replace("@Date", "convert(date,adate)>=(DATEADD(mm, DATEDIFF(mm, 0, GETDATE()), 0)) and convert(date,adate)<=(DATEADD(ms, -3, DATEADD(mm, DATEDIFF(m, 0, GETDATE())+1, 0)))")
                    ElseIf Dtf.ToUpper = "CURRENT QUARTER" Then
                        str = str.Replace("@Date", "convert(date,adate)>=DATEADD(qq, DATEDIFF(qq, 0, GETDATE()), 0) and convert(date,adate)<=DATEADD (dd, -1, DATEADD(qq, DATEDIFF(qq, 0, GETDATE()) +1, 0))")
                        fltr = fltr.Replace("@Date", "convert(date,adate)>=DATEADD(qq, DATEDIFF(qq, 0, GETDATE()), 0) and convert(date,adate)<=DATEADD (dd, -1, DATEADD(qq, DATEDIFF(qq, 0, GETDATE()) +1, 0))")
                    ElseIf Dtf.ToUpper = "LAST QUARTER" Then
                        str = str.Replace("@Date", "convert(date,adate)>=DATEADD(qq, DATEDIFF(qq, 0, GETDATE()) - 1, 0) and convert(date,adate)<=DATEADD (dd, -1, DATEADD(qq, DATEDIFF(qq, 0, GETDATE()), 0))")
                        fltr = fltr.Replace("@Date", "convert(date,adate)>=DATEADD(qq, DATEDIFF(qq, 0, GETDATE()) - 1, 0) and convert(date,adate)<=DATEADD (dd, -1, DATEADD(qq, DATEDIFF(qq, 0, GETDATE()), 0))")
                    Else
                        str = str.Replace("@Date", "convert(date,adate)>=convert(date,'" & Sdate.ToString & "')  and convert(date,adate)<=convert(date,'" & Edate.ToString & "')")
                        fltr = fltr.Replace("@Date", "convert(date,adate)>=convert(date,'" & Sdate.ToString & "')  and convert(date,adate)<=convert(date,'" & Edate.ToString & "')")
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

                    If All.ToUpper.ToString = "ALL" Then
                        str = str.Replace("top 5", " ")
                        str = str.Replace("Top 5", " ")
                        str = str.Replace("@category", "t.category")
                    End If

                    oda.SelectCommand.CommandText = fltr
                    oda.Fill(dt1)
                    Dim cat As String = ""

                    If dt1.Rows.Count > 0 Then
                        cat = dt1.Rows(0).Item(0).ToString
                        cat = cat.Replace(",", "','")
                        str = str.Replace("@category", "'" & cat & "'")
                    End If
                    oda.SelectCommand.CommandType = CommandType.Text
                    oda.SelectCommand.CommandText = str


                    'oda.Fill(ds, "data")
                    oda.Fill(dt)
                    dt.Dispose()
                    Dim lstColumns As New List(Of String)
                    Dim serializerSettings As New JsonSerializerSettings()
                    Dim json_serializer As New JavaScriptSerializer()
                    serializerSettings.Converters.Add(New DataTableConverter())
                    jsonData = JsonConvert.SerializeObject(dt, Newtonsoft.Json.Formatting.None, serializerSettings)
                End If
            End Using

        Catch ex As Exception
            Throw
        Finally
            ds.Dispose()
            dt.Dispose()
        End Try
        Return jsonData
    End Function
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Shared Function GetDataInvoiceAgeing(Type As String, Dtf As String, Invdt As String, All As String) As String
        Dim jsonData As String = ""
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
                        Dim dtlDeptquery = "select DocumentType,Type,catField,ValueField,RecdateField,InvDateField from mmm_mst_misdb_dtl where RefTid=(select tid from mmm_mst_misdb where DBName='Open Invoice Ageing' and eid=" & HttpContext.Current.Session("EID") & ")"
                        Dim tabledata As New DataTable
                        Dim SETDATA As New DataSet
                        Dim ANDDEPT As String = ""
                        Dim newQueryforall1 As String = ""
                        Dim CDATA As New DataSet
                        oda.SelectCommand.CommandType = CommandType.Text
                        oda.SelectCommand.CommandText = dtlDeptquery
                        oda.Fill(SETDATA)
                        If (SETDATA.Tables(0).Rows.Count > 0) Then
                            For MI As Integer = 0 To SETDATA.Tables(0).Rows.Count - 1
                                Dim documenttypAll = SETDATA.Tables(0).Rows(MI).Item("DocumentType")
                                Dim CatogoryField = SETDATA.Tables(0).Rows(MI).Item("catField")
                                newQueryforall1 = "select FieldID,FieldType,documenttype,DROPDOWNTYPE,dropdown,FieldMapping,displayName,Datatype from mmm_mst_fields where FieldMapping='" & CatogoryField & "' and DocumentType='" & documenttypAll & "' and Eid=" & HttpContext.Current.Session("EID") & " and isactive=1"
                                oda.SelectCommand.CommandType = CommandType.Text
                                oda.SelectCommand.CommandText = newQueryforall1
                                oda.Fill(CDATA)
                                If (CDATA.Tables(0).Rows.Count > 0) Then
                                    If (CDATA.Tables(0).Rows(MI).Item("DROPDOWNTYPE") = "MASTER VALUED") Then
                                        ANDDEPT = ANDDEPT & " when documenttype= '" & SETDATA.Tables(0).Rows(MI).Item("DocumentType").ToString & "'  then  dms.udf_split('" & CDATA.Tables(0).Rows(MI).Item("dropdown") & "'," & CatogoryField & ") "
                                    End If
                                End If
                            Next
                            ANDDEPT = " (case " & ANDDEPT & " end) ='" & Type.ToString & "' "
                        End If
                        str = str.Replace("@Dept", "" & ANDDEPT & "")
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

                    If All.ToUpper.ToString = "ALL" Then
                        str = str.Replace("top 5", " ")
                        str = str.Replace("Top 5", " ")
                    End If
                    oda.SelectCommand.CommandType = CommandType.Text
                    oda.SelectCommand.CommandText = str

                    oda.Fill(dt)
                    dt.Dispose()
                    Dim lstColumns As New List(Of String)
                    Dim serializerSettings As New JsonSerializerSettings()
                    Dim json_serializer As New JavaScriptSerializer()
                    serializerSettings.Converters.Add(New DataTableConverter())
                    jsonData = JsonConvert.SerializeObject(dt, Newtonsoft.Json.Formatting.None, serializerSettings)
                End If
            End Using

        Catch ex As Exception
            Throw
        Finally
            ds.Dispose()
            dt.Dispose()
        End Try
        Return jsonData
    End Function
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Shared Function InvoiceDist(Type As String, Dtf As String, Sdate As String, Edate As String, All As String) As String
        Dim jsonData As String = ""
        Try
            Dim ds As New DataSet()
            Dim UID As Integer = 0
            Dim URole As String = ""
            Dim qry As String = ""
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As New SqlConnection(conStr)
            Dim dt As New DataTable
            Dim dt1 As New DataTable
            Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
            Dim Res As New vdashboard()
            If (Type.ToUpper = "DEPARTMENT") Then
                qry = "SELECT rootqry_D,RootqryFilter_D from mmm_mst_misdb where eid=" & HttpContext.Current.Session("EID") & " and DBName='Invoice Distribution'"
            Else
                qry = "SELECT rootquery_e,RootqryFilter_E from mmm_mst_misdb where eid=" & HttpContext.Current.Session("EID") & " and DBName='Invoice Distribution'"
            End If
            oda.SelectCommand.CommandType = CommandType.Text
            oda.SelectCommand.CommandText = qry
            oda.Fill(ds, "qry")
            If ds.Tables("qry").Rows.Count > 0 Then

                Dim str As String = ds.Tables("qry").Rows(0).Item(0).ToString()
                Dim fltr As String = ds.Tables("qry").Rows(0).Item(1).ToString()
                'Dim str As String = "select category,Days[DaysType],convert(numeric(10,2),sum(convert(numeric(10,2),Amount))/1000000)[Amount],count(tid)[Count]  from (select tid,case when documenttype='invoice po' then dms.udf_split('MASTER-Department Master-fld2',fld65) when documenttype='invoice non po' then dms.udf_split('MASTER-Department Master-fld2',fld3) end[category],case when documenttype='Invoice PO' then max(fld92) when documenttype='Invoice non PO' then max(fld37) end [Amount],case when datediff(dd,convert(date,case when documenttype='Invoice PO' then fld85 when documenttype='Invoice Non PO' then fld87 end ,3),(select max(fdate) from mmm_doc_dtl with(nolock) where docid=d.tid ))<=15 then '0-15 Days' when datediff(dd,convert(date,case when documenttype='Invoice PO' then fld85 when documenttype='Invoice Non PO' then fld87 end ,3),(select max(fdate) from mmm_doc_dtl with(nolock) where docid=d.tid ))>=16 and datediff(dd,convert(date,case when documenttype='Invoice PO' then fld85 when documenttype='Invoice Non PO' then fld87 end ,3),(select max(fdate) from mmm_doc_dtl with(nolock) where docid=d.tid ))<=30 then '16-30 Days' when datediff(dd,convert(date,case when documenttype='Invoice PO' then fld85 when documenttype='Invoice Non PO' then fld87 end ,3),(select max(fdate) from mmm_doc_dtl with(nolock) where docid=d.tid ))>=31 and datediff(dd,convert(date,case when documenttype='Invoice PO' then fld85 when documenttype='Invoice Non PO' then fld87 end ,3),(select max(fdate) from mmm_doc_dtl with(nolock) where docid=d.tid ))<=45 then '31-45 Days' when datediff(dd,convert(date,case when documenttype='Invoice PO' then fld85 when documenttype='Invoice Non PO' then fld87 end ,3),(select max(fdate) from mmm_doc_dtl with(nolock) where docid=d.tid ))>=46 and datediff(dd,convert(date,case when documenttype='Invoice PO' then fld85 when documenttype='Invoice Non PO' then fld87 end ,3),(select max(fdate) from mmm_doc_dtl with(nolock) where docid=d.tid ))<=3000 then '45+ Days' end [Days] from mmm_mst_doc d where eid=152 and documenttype in ('Invoice PO','Invoice Non PO') and curstatus in ('archive') and @Date  group by d.tid,documenttype,fld85,fld87,fld65,fld3) as t where t.category in ('Finance','administration','Information Systems','Sales and Marketing','Human Resources and Training') group by category,days order by DaysType desc"
                If Dtf.ToUpper = "CURRENT FY" Then
                    str = str.Replace("@Date", "convert(date,adate)>=(dateadd(year, datepart(year, getdate()-89) - 1900, '1900-04-01')) and convert(date,adate)<=(dateadd(day, -1, dateadd(month, 12, (dateadd(year, datepart(year, getdate()-89) - 1900, '1900-04-01')))))")
                    fltr = fltr.Replace("@Date", "convert(date,adate)>=(dateadd(year, datepart(year, getdate()-89) - 1900, '1900-04-01')) and convert(date,adate)<=(dateadd(day, -1, dateadd(month, 12, (dateadd(year, datepart(year, getdate()-89) - 1900, '1900-04-01')))))")
                ElseIf Dtf.ToUpper = "LAST FY" Then
                    str = str.Replace("@Date", "convert(date,adate)>=dateadd(year, -1, (dateadd(year, datepart(year, getdate()-89) - 1900, '1900-04-01'))) and convert(date,adate)<=dateadd(year, -1, (dateadd(day, -1, dateadd(month, 12, (dateadd(year, datepart(year, getdate()-89) - 1900, '1900-04-01'))))))")
                    fltr = fltr.Replace("@Date", "convert(date,adate)>=dateadd(year, -1, (dateadd(year, datepart(year, getdate()-89) - 1900, '1900-04-01'))) and convert(date,adate)<=dateadd(year, -1, (dateadd(day, -1, dateadd(month, 12, (dateadd(year, datepart(year, getdate()-89) - 1900, '1900-04-01'))))))")
                ElseIf Dtf.ToUpper = "LAST MONTH" Then
                    str = str.Replace("@Date", "convert(date,adate)>=(DATEADD(mm, DATEDIFF(mm, 0, GETDATE())-1, 0)) and convert(date,adate)<=(DATEADD(ms, -3, DATEADD(mm, DATEDIFF(m, 0, GETDATE()), 0)))")
                    fltr = fltr.Replace("@Date", "convert(date,adate)>=(DATEADD(mm, DATEDIFF(mm, 0, GETDATE())-1, 0)) and convert(date,adate)<=(DATEADD(ms, -3, DATEADD(mm, DATEDIFF(m, 0, GETDATE()), 0)))")
                ElseIf Dtf.ToUpper = "CURRENT MONTH" Then
                    str = str.Replace("@Date", "convert(date,adate)>=(DATEADD(mm, DATEDIFF(mm, 0, GETDATE()), 0)) and convert(date,adate)<=(DATEADD(ms, -3, DATEADD(mm, DATEDIFF(m, 0, GETDATE())+1, 0)))")
                    fltr = fltr.Replace("@Date", "convert(date,adate)>=(DATEADD(mm, DATEDIFF(mm, 0, GETDATE()), 0)) and convert(date,adate)<=(DATEADD(ms, -3, DATEADD(mm, DATEDIFF(m, 0, GETDATE())+1, 0)))")
                ElseIf Dtf.ToUpper = "CURRENT QUARTER" Then
                    str = str.Replace("@Date", "convert(date,adate)>=DATEADD(qq, DATEDIFF(qq, 0, GETDATE()), 0) and convert(date,adate)<=DATEADD (dd, -1, DATEADD(qq, DATEDIFF(qq, 0, GETDATE()) +1, 0))")
                    fltr = fltr.Replace("@Date", "convert(date,adate)>=DATEADD(qq, DATEDIFF(qq, 0, GETDATE()), 0) and convert(date,adate)<=DATEADD (dd, -1, DATEADD(qq, DATEDIFF(qq, 0, GETDATE()) +1, 0))")
                ElseIf Dtf.ToUpper = "LAST QUARTER" Then
                    str = str.Replace("@Date", "convert(date,adate)>=DATEADD(qq, DATEDIFF(qq, 0, GETDATE()) - 1, 0) and convert(date,adate)<=DATEADD (dd, -1, DATEADD(qq, DATEDIFF(qq, 0, GETDATE()), 0))")
                    fltr = fltr.Replace("@Date", "convert(date,adate)>=DATEADD(qq, DATEDIFF(qq, 0, GETDATE()) - 1, 0) and convert(date,adate)<=DATEADD (dd, -1, DATEADD(qq, DATEDIFF(qq, 0, GETDATE()), 0))")
                Else
                    str = str.Replace("@Date", "convert(date,adate)>=convert(date,'" & Sdate.ToString & "')  and convert(date,adate)<=convert(date,'" & Edate.ToString & "')")
                    fltr = fltr.Replace("@Date", "convert(date,adate)>=convert(date,'" & Sdate.ToString & "')  and convert(date,adate)<=convert(date,'" & Edate.ToString & "')")
                End If
                If HttpContext.Current.Session("USERROLE").ToString.ToUpper = "SU" Then
                Else
                    str = str.Replace("eid=" & HttpContext.Current.Session("EID").ToString & "", "eid=" & HttpContext.Current.Session("EID").ToString & " " & HttpContext.Current.Session("FilterRole").ToString)
                End If
                If All.ToUpper.ToString = "ALL" Then
                    str = str.Replace("top 5", " ")
                    str = str.Replace("Top 5", " ")
                    str = str.Replace("@category", "t.category")
                End If

                oda.SelectCommand.CommandText = fltr
                oda.Fill(dt1)
                Dim cat As String = ""

                If dt1.Rows.Count > 0 Then
                    cat = dt1.Rows(0).Item(0).ToString
                    cat = cat.Replace(",", "','")
                    str = str.Replace("@category", "'" & cat & "'")
                End If
                oda.SelectCommand.CommandType = CommandType.Text
                oda.SelectCommand.CommandText = str
                oda.SelectCommand.CommandTimeout = 600


                'oda.Fill(ds, "data")
                oda.Fill(dt)
                dt.Dispose()
                Dim lstColumns As New List(Of String)
                Dim serializerSettings As New JsonSerializerSettings()
                Dim json_serializer As New JavaScriptSerializer()
                serializerSettings.Converters.Add(New DataTableConverter())
                jsonData = JsonConvert.SerializeObject(dt, Newtonsoft.Json.Formatting.None, serializerSettings)
            End If
            Return jsonData
        Catch Ex As Exception
            Throw
        End Try

    End Function
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Shared Function getExpenseBreakupDtl(Type As String, Doc As String, Dtf As String, Sdate As String, Edate As String) As DGrid
        Dim jsonData As String = ""
        Dim grid As New DGrid()
        If (HttpContext.Current.Session("UID") Is Nothing) Then
            grid.Message = "Your Session has been expired..!"
            grid.Success = False
            grid.Count = 0
            Return grid
            Exit Function
        End If
        Try
            If HttpContext.Current.Session("EID") = "" Or HttpContext.Current.Session("EID") Is Nothing Then
                HttpContext.Current.Response.Redirect("SessionOut.aspx")
            End If
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
        If (HttpContext.Current.Session("UID") Is Nothing) Then
            grid.Message = "Your Session has been expired..!"
            grid.Success = False
            grid.Count = 0
            Return grid
            Exit Function
        End If
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
        If (HttpContext.Current.Session("UID") Is Nothing) Then
            grid.Message = "Your Session has been expired..!"
            grid.Success = False
            grid.Count = 0
            Return grid
            Exit Function
        End If
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

                    Dim newQueryforall1 As String = ""
                    Dim CDATA As New DataSet
                    Dim dtlDeptquery = ""
                    If Name.ToUpper = "PO" Then
                        dtlDeptquery = "select DocumentType,Type,catField,ValueField,RecdateField,InvDateField,InvType,InvTypePOID,InvTypeNonPOID from mmm_mst_misdb_dtl where RefTid=(select tid from mmm_mst_misdb where DBName='Invoice Distribution' and eid=" & HttpContext.Current.Session("EID") & ") and documenttype not like '%Non%'"
                    Else
                        dtlDeptquery = "select DocumentType,Type,catField,ValueField,RecdateField,InvDateField,InvType,InvTypePOID,InvTypeNonPOID from mmm_mst_misdb_dtl where RefTid=(select tid from mmm_mst_misdb where DBName='Invoice Distribution' and eid=" & HttpContext.Current.Session("EID") & ") and documenttype not like '%Invoice PO%'"
                    End If
                    'Dim dtlDeptquery = "select DocumentType,Type,catField,ValueField,RecdateField,InvDateField from mmm_mst_misdb_dtl where RefTid=(select tid from mmm_mst_misdb where DBName='Invoice Distribution' and eid=" & HttpContext.Current.Session("EID") & ")"
                    Dim tabledata As New DataTable
                    Dim SETDATA As New DataSet
                    Dim ANDDEPT As String = ""

                    oda.SelectCommand.CommandType = CommandType.Text
                    oda.SelectCommand.CommandText = dtlDeptquery
                    oda.Fill(SETDATA)
                    If (SETDATA.Tables(0).Rows.Count > 0) Then
                        For MI As Integer = 0 To SETDATA.Tables(0).Rows.Count - 1
                            Dim documenttypAll = SETDATA.Tables(0).Rows(MI).Item("DocumentType")
                            Dim CatogoryField = SETDATA.Tables(0).Rows(MI).Item("catField")
                            newQueryforall1 = "select FieldID,FieldType,documenttype,DROPDOWNTYPE,dropdown,FieldMapping,displayName,Datatype from mmm_mst_fields where FieldMapping='" & CatogoryField & "' and DocumentType='" & documenttypAll & "' and Eid=" & HttpContext.Current.Session("EID") & " and isactive=1"
                            oda.SelectCommand.CommandType = CommandType.Text
                            oda.SelectCommand.CommandText = newQueryforall1
                            oda.Fill(CDATA)
                            If (CDATA.Tables(0).Rows.Count > 0) Then
                                If (CDATA.Tables(0).Rows(MI).Item("DROPDOWNTYPE") = "MASTER VALUED") Then
                                    If SETDATA.Tables(0).Rows(MI).Item("DocumentType").ToString.ToUpper.Contains("VENDOR INVOICE") Then
                                        If Name.ToUpper = "PO" Then
                                            ANDDEPT = ANDDEPT & " dms.udf_split('" & CDATA.Tables(0).Rows(MI).Item("dropdown") & "'," & CatogoryField & ")='" & Doc & "'  and documenttype='" & SETDATA.Tables(0).Rows(MI).Item("DocumentType").ToString & "' and " & SETDATA.Tables(0).Rows(MI).Item("InvType") & " = '" & SETDATA.Tables(0).Rows(MI).Item("InvTypePOID") & "'"
                                        Else
                                            ANDDEPT = ANDDEPT & " dms.udf_split('" & CDATA.Tables(0).Rows(MI).Item("dropdown") & "'," & CatogoryField & ")='" & Doc & "'  and documenttype='" & SETDATA.Tables(0).Rows(MI).Item("DocumentType").ToString & "' and " & SETDATA.Tables(0).Rows(MI).Item("InvType") & " = '" & SETDATA.Tables(0).Rows(MI).Item("InvTypeNonPOID") & "'"
                                        End If
                                    Else
                                        ANDDEPT = ANDDEPT & " dms.udf_split('" & CDATA.Tables(0).Rows(MI).Item("dropdown") & "'," & CatogoryField & ")='" & Doc & "' and documenttype='" & SETDATA.Tables(0).Rows(MI).Item("DocumentType").ToString & "' "
                                    End If

                                    If MI <> SETDATA.Tables(0).Rows.Count - 1 Then
                                        ANDDEPT = ANDDEPT & " Or "
                                    End If
                                End If

                            End If
                        Next
                        'ANDDEPT = " @dept"
                    End If

                    str = str.Replace("@dept", "" & ANDDEPT & "")
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
        If (HttpContext.Current.Session("UID") Is Nothing) Then
            grid.Message = "Your Session has been expired..!"
            grid.Success = False
            grid.Count = 0
            Return grid
            Exit Function
        End If
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
                        str = str.Replace("@Days1", "0")
                        str = str.Replace("@Days2", "15")
                    ElseIf Name.ToUpper = "16-30 DAYS" Then
                        str = str.Replace("@days1", "16")
                        str = str.Replace("@days2", "30")
                        str = str.Replace("@Days1", "16")
                        str = str.Replace("@Days2", "30")
                    ElseIf Name.ToUpper = "31-45 DAYS" Then
                        str = str.Replace("@days1", "31")
                        str = str.Replace("@days2", "45")
                        str = str.Replace("@Days1", "31")
                        str = str.Replace("@Days2", "45")
                    Else
                        str = str.Replace("@days1", "46")
                        str = str.Replace("@days2", "4600")
                        str = str.Replace("@Days1", "46")
                        str = str.Replace("@Days2", "4600")
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
    Public Shared Function getOpenInvoiceAgeingDtl(Type As String, Doc As String, Name As String, Invdt As String, Dtf As String) As DGrid
        Dim jsonData As String = ""
        Dim grid As New DGrid()
        If (HttpContext.Current.Session("UID") Is Nothing) Then
            grid.Message = "Your Session has been expired..!"
            grid.Success = False
            grid.Count = 0
            Return grid
            Exit Function
        End If
        Try
            Dim ds As New DataSet()
            Dim ds1 As New DataSet()
            Dim Query As String = ""
            Dim dt As New DataTable
            Dim dt1 As New DataTable
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

                    ''''WorkFlow Check and replace status
                    Dim sts = "select distinct alias,WorkFlow  from mmm_mst_misdb_workflow where reftid= (select tid from mmm_mst_misdb where eid=" & HttpContext.Current.Session("EID") & " and DBName='Open Invoice Ageing') and alias='" & Doc.ToString & "'"
                    oda.SelectCommand.CommandType = CommandType.Text
                    oda.SelectCommand.CommandText = sts
                    oda.Fill(dt1)
                    If dt1.Rows.Count > 0 Then
                        str = str.Replace("@status", "'" & Replace(dt1.Rows(0).Item("WorkFlow").ToString, ",", "','") & "'")
                    Else
                        str = str.Replace("@status", "'" & Doc & "'")
                    End If


                    If (Type.ToUpper = "DEPARTMENT") Or (Type.ToUpper = "ALL DEPARTMENTS") Then
                        str = str.Replace("@Dept", "d.tid=d.tid")
                    Else
                        Dim newQueryforall1 As String = ""
                        Dim CDATA As New DataSet
                        Dim dtlDeptquery = "select DocumentType,Type,catField,ValueField,RecdateField,InvDateField from mmm_mst_misdb_dtl where RefTid=(select tid from mmm_mst_misdb where DBName='Open Invoice Ageing' and eid=" & HttpContext.Current.Session("EID") & ")"
                        Dim tabledata As New DataTable
                        Dim SETDATA As New DataSet
                        Dim ANDDEPT As String = ""

                        oda.SelectCommand.CommandType = CommandType.Text
                        oda.SelectCommand.CommandText = dtlDeptquery
                        oda.Fill(SETDATA)
                        If (SETDATA.Tables(0).Rows.Count > 0) Then
                            For MI As Integer = 0 To SETDATA.Tables(0).Rows.Count - 1
                                Dim documenttypAll = SETDATA.Tables(0).Rows(MI).Item("DocumentType")
                                Dim CatogoryField = SETDATA.Tables(0).Rows(MI).Item("catField")
                                newQueryforall1 = "select FieldID,FieldType,documenttype,DROPDOWNTYPE,dropdown,FieldMapping,displayName,Datatype from mmm_mst_fields where FieldMapping='" & CatogoryField & "' and DocumentType='" & documenttypAll & "' and Eid=" & HttpContext.Current.Session("EID") & " and isactive=1"
                                oda.SelectCommand.CommandType = CommandType.Text
                                oda.SelectCommand.CommandText = newQueryforall1
                                oda.Fill(CDATA)
                                If (CDATA.Tables(0).Rows.Count > 0) Then
                                    If (CDATA.Tables(0).Rows(MI).Item("DROPDOWNTYPE") = "MASTER VALUED") Then
                                        ANDDEPT = ANDDEPT & "( dms.udf_split('" & CDATA.Tables(0).Rows(MI).Item("dropdown") & "'," & CatogoryField & ")='" & Type & "' and documenttype='" & SETDATA.Tables(0).Rows(MI).Item("DocumentType").ToString & "' )"
                                        If MI <> SETDATA.Tables(0).Rows.Count - 1 Then
                                            ANDDEPT = ANDDEPT & " Or "
                                        End If
                                    End If
                                End If
                            Next
                            str = str.Replace("@Dept", "(" & ANDDEPT & ")")
                        End If
                    End If

                    'str = str.Replace("@dept", "'" & Doc & "'")
                    If Name.ToUpper = " 0-5 DAYS" Then
                        str = str.Replace("@days1", "0")
                        str = str.Replace("@days2", "5")
                        str = str.Replace("@Days1", "0")
                        str = str.Replace("@Days2", "5")
                    ElseIf Name.ToUpper = " 6-10 DAYS" Then
                        str = str.Replace("@days1", "6")
                        str = str.Replace("@days2", "10")
                        str = str.Replace("@Days1", "6")
                        str = str.Replace("@Days2", "10")
                    ElseIf Name.ToUpper = "11-15 DAYS" Then
                        str = str.Replace("@days1", "11")
                        str = str.Replace("@days2", "15")
                        str = str.Replace("@Days1", "11")
                        str = str.Replace("@Days2", "15")
                    Else
                        str = str.Replace("@days1", "16")
                        str = str.Replace("@days2", "4600")
                        str = str.Replace("@Days1", "16")
                        str = str.Replace("@Days2", "4600")
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
    Public Shared Function getSLAPerformanceDtl(Type As String, Doc As String, Name As String, Status As String, Dtf As String, Sdate As String, Edate As String) As DGrid
        Dim jsonData As String = ""
        Dim grid As New DGrid()
        If (HttpContext.Current.Session("UID") Is Nothing) Then
            grid.Message = "Your Session has been expired..!"
            grid.Success = False
            grid.Count = 0
            Return grid
            Exit Function
        End If
        Try
            Dim ds As New DataSet()
            Dim Query As String = ""
            Dim dt As New DataTable
            Dim dt1 As New DataTable
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

                    ''''Date replace as per filter
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

                    ''''WorkFlow Check and replace status
                    Dim sts = "select distinct alias,WorkFlow  from mmm_mst_misdb_workflow where reftid= (select tid from mmm_mst_misdb where eid=" & HttpContext.Current.Session("EID") & " and DBName='SLA Performance') and alias='" & Doc.ToString & "'"
                    oda.SelectCommand.CommandType = CommandType.Text
                    oda.SelectCommand.CommandText = sts
                    oda.Fill(dt1)
                    If dt1.Rows.Count > 0 Then
                        str = str.Replace("@status", "'" & Replace(dt1.Rows(0).Item("WorkFlow").ToString, ",", "','") & "'")
                    Else
                        str = str.Replace("@status", "'" & Doc & "'")
                    End If

                    If (Status.ToUpper = "PAID") Or (Status.ToString = "Paid") Then
                        str = str.Replace("@curstatus", "curstatus in ('archive')")
                    ElseIf (Status.ToUpper = "UNPAID") Or (Status.ToString = "Unpaid") Then
                        str = str.Replace("@curstatus", "curstatus not in ('rejected','archive')")
                    Else
                        str = str.Replace("@curstatus", "curstatus=curstatus")
                    End If

                    If (Type.ToUpper = "DEPARTMENT") Or (Type.ToUpper = "ALL DEPARTMENTS") Then
                        str = str.Replace("@Dept", "d.tid=d.tid")
                    Else
                        Dim newQueryforall1 As String = ""
                        Dim CDATA As New DataSet
                        Dim dtlDeptquery = "select DocumentType,Type,catField,ValueField,RecdateField,InvDateField from mmm_mst_misdb_dtl where RefTid=(select tid from mmm_mst_misdb where DBName='SLA Performance' and eid=" & HttpContext.Current.Session("EID") & ")"
                        Dim tabledata As New DataTable
                        Dim SETDATA As New DataSet
                        Dim ANDDEPT As String = ""

                        oda.SelectCommand.CommandType = CommandType.Text
                        oda.SelectCommand.CommandText = dtlDeptquery
                        oda.Fill(SETDATA)
                        If (SETDATA.Tables(0).Rows.Count > 0) Then
                            For MI As Integer = 0 To SETDATA.Tables(0).Rows.Count - 1
                                Dim documenttypAll = SETDATA.Tables(0).Rows(MI).Item("DocumentType")
                                Dim CatogoryField = SETDATA.Tables(0).Rows(MI).Item("catField")
                                newQueryforall1 = "select FieldID,FieldType,documenttype,DROPDOWNTYPE,dropdown,FieldMapping,displayName,Datatype from mmm_mst_fields where FieldMapping='" & CatogoryField & "' and DocumentType='" & documenttypAll & "' and Eid=" & HttpContext.Current.Session("EID") & " and isactive=1"
                                oda.SelectCommand.CommandType = CommandType.Text
                                oda.SelectCommand.CommandText = newQueryforall1
                                oda.Fill(CDATA)
                                If (CDATA.Tables(0).Rows.Count > 0) Then
                                    If (CDATA.Tables(0).Rows(MI).Item("DROPDOWNTYPE") = "MASTER VALUED") Then
                                        ANDDEPT = ANDDEPT & "( dms.udf_split('" & CDATA.Tables(0).Rows(MI).Item("dropdown") & "'," & CatogoryField & ")='" & Type & "' and documenttype='" & SETDATA.Tables(0).Rows(MI).Item("DocumentType").ToString & "' )"
                                        If MI <> SETDATA.Tables(0).Rows.Count - 1 Then
                                            ANDDEPT = ANDDEPT & " Or "
                                        End If
                                    End If
                                End If
                            Next
                            str = str.Replace("@Dept", "(" & ANDDEPT & ")")
                        End If
                    End If

                    'str = str.Replace("@Dept", "'" & Doc & "'")
                    'str = str.Replace("@dept", "'" & Doc & "'")
                    If Name.ToUpper = "WITHIN SLA" Then
                        str = str.Replace("@SLA", "atat<=ptat")
                    Else
                        str = str.Replace("@SLA", "atat>ptat")
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
        If (HttpContext.Current.Session("UID") Is Nothing) Then
            grid.Message = "Your Session has been expired..!"
            grid.Success = False
            grid.Count = 0
            Return grid
            Exit Function
        End If
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
        If (HttpContext.Current.Session("UID") Is Nothing) Then
            grid.Message = "Your Session has been expired..!"
            grid.Success = False
            grid.Count = 0
            Return grid
            Exit Function
        End If
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
        If (HttpContext.Current.Session("UID") Is Nothing) Then
            grid.Message = "Your Session has been expired..!"
            grid.Success = False
            grid.Count = 0
            Return grid
            Exit Function
        End If
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
        If (HttpContext.Current.Session("UID") Is Nothing) Then
            grid.Message = "Your Session has been expired..!"
            grid.Success = False
            grid.Count = 0
            Return grid
            Exit Function
        End If
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
        If (HttpContext.Current.Session("UID") Is Nothing) Then
            grid.Message = "Your Session has been expired..!"
            grid.Success = False
            grid.Count = 0
            Return grid
            Exit Function
        End If
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
        If (HttpContext.Current.Session("UID") Is Nothing) Then
            grid.Message = "Your Session has been expired..!"
            grid.Success = False
            grid.Count = 0
            Return grid
            Exit Function
        End If
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
            Dim str As String = ""
            Dim qry As String = ""
            Dim QueryNew As String = ""
            Dim predata As New DataSet
            Dim csv As String = ""
            Dim lstDocumenettype As New ArrayList
            Dim Strcolmn As String = ""
            Dim ddlvalue As String = ""
            Dim newQuery As String = ""
            Dim StrColumn As String = ""
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As New SqlConnection(conStr)
            Dim dt As New DataTable
            Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)

            str = "select Tid,RefTid,DocumentType,type,CatField,Valuefield,RecdateField,InvDateField from mmm_mst_misdb_dtl where refTid in (SELECT tid from mmm_mst_misdb where eid=" & HttpContext.Current.Session("EID") & " and DBName='Open Invoice Ageing')"
            Dim Dtable As New DataTable
            oda.SelectCommand.CommandText = str
            oda.Fill(dt)
            If (dt.Rows.Count > 0) Then
                For t As Integer = 0 To dt.Rows.Count - 1
                    Dim std As String = ""
                    Dim ddoctype As String = ""
                    Dim valuefield As String = ""
                    Dim datefield As String = ""
                    Dim datefield1 As String = ""
                    std = dt.Rows(t).Item("CatField")
                    ddoctype = dt.Rows(t).Item("DocumentType")
                    valuefield = dt.Rows(t).Item("Valuefield")
                    datefield = dt.Rows(t).Item("RecdateField")
                    datefield1 = dt.Rows(t).Item("InvDateField")
                    lstDocumenettype.Add(Convert.ToString(dt.Rows(t).Item("DocumentType")))
                    newQuery = "select FieldID,FieldType,documenttype,DROPDOWNTYPE,dropdown,FieldMapping,displayName,Datatype from mmm_mst_fields where FieldMapping='" & std & "' and DocumentType='" & ddoctype & "' and Eid=" & HttpContext.Current.Session("EID") & " and isactive=1 and DropDownType='MASTER VALUED'"
                    oda.SelectCommand.CommandText = newQuery
                    oda.Fill(predata)
                    If (predata.Tables(0).Rows.Count > 0) Then
                        Strcolmn = Strcolmn & "when documenttype='" & ddoctype & "' then " & std & " "
                        StrColumn = StrColumn & "when documenttype='" & ddoctype & "' then dms.udf_split('" & predata.Tables(0).Rows(t).Item("dropdown") & "'," & std & ")"
                    End If
                Next
            End If


            qry = "select distinct case " & Strcolmn & " end[Value],case " & StrColumn & " end[Display]  from mmm_mst_doc  where eid=" & HttpContext.Current.Session("EID") & " and documenttype in('" & String.Join("','", lstDocumenettype.ToArray) & "')"
            dt.Clear()

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
            Dim str As String = ""
            Dim qry As String = ""
            Dim QueryNew As String = ""
            Dim predata As New DataSet
            Dim csv As String = ""
            Dim lstDocumenettype As New ArrayList
            Dim Strcolmn As String = ""
            Dim ddlvalue As String = ""
            Dim newQuery As String = ""
            Dim StrColumn As String = ""
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As New SqlConnection(conStr)
            Dim dt As New DataTable
            Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)

            str = "select Tid,RefTid,DocumentType,type,CatField,Valuefield,RecdateField,InvDateField from mmm_mst_misdb_dtl where refTid in (SELECT tid from mmm_mst_misdb where eid=" & HttpContext.Current.Session("EID") & " and DBName='Open Invoice Ageing')"
            Dim Dtable As New DataTable
            oda.SelectCommand.CommandText = str
            oda.Fill(dt)
            If (dt.Rows.Count > 0) Then
                For t As Integer = 0 To dt.Rows.Count - 1
                    Dim std As String = ""
                    Dim ddoctype As String = ""
                    Dim valuefield As String = ""
                    Dim datefield As String = ""
                    Dim datefield1 As String = ""
                    std = dt.Rows(t).Item("CatField")
                    ddoctype = dt.Rows(t).Item("DocumentType")
                    valuefield = dt.Rows(t).Item("Valuefield")
                    datefield = dt.Rows(t).Item("RecdateField")
                    datefield1 = dt.Rows(t).Item("InvDateField")
                    lstDocumenettype.Add(Convert.ToString(dt.Rows(t).Item("DocumentType")))
                    newQuery = "select FieldID,FieldType,documenttype,DROPDOWNTYPE,dropdown,FieldMapping,displayName,Datatype from mmm_mst_fields where FieldMapping='" & std & "' and DocumentType='" & ddoctype & "' and Eid=" & HttpContext.Current.Session("EID") & " and isactive=1 and DropDownType='MASTER VALUED'"
                    oda.SelectCommand.CommandText = newQuery
                    oda.Fill(predata)
                    If (predata.Tables(0).Rows.Count > 0) Then
                        Strcolmn = Strcolmn & "when documenttype='" & ddoctype & "' then " & std & " "
                        StrColumn = StrColumn & "when documenttype='" & ddoctype & "' then dms.udf_split('" & predata.Tables(0).Rows(t).Item("dropdown") & "'," & std & ")"
                    End If
                Next
            End If


            qry = "select distinct case " & Strcolmn & " end[Value],case " & StrColumn & " end[Display]  from mmm_mst_doc  where eid=" & HttpContext.Current.Session("EID") & " and documenttype in('" & String.Join("','", lstDocumenettype.ToArray) & "')"
            dt.Clear()

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
