Imports System.Data.SqlClient
Imports System.Data
Imports System.Web.Services
Imports iTextSharp.text
Imports System.IO
Imports iTextSharp.text.html.simpleparser
Imports iTextSharp.text.pdf
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Converters
Imports System.Web.Script.Serialization

Partial Class PlanVsActualTimeBased
    Inherits System.Web.UI.Page
    Shared dsstatic As DataSet = New DataSet()
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As SqlConnection = New SqlConnection(conStr)
            Dim Udtype As String
            Dim Ufld As String
            Dim UVfld As String
            Dim Vdtype As String
            Dim Vfld As String
            Dim vemei As String
            Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
            Try
                oda.SelectCommand.CommandText = "select * from mmm_mst_entity where eid=" & Session("EID") & " "
                Dim ds As New DataSet()
                oda.Fill(ds, "data")
                If ds.Tables("data").Rows.Count > 0 Then
                    Udtype = ds.Tables("data").Rows(0).Item("uvdtype").ToString
                    Ufld = ds.Tables("data").Rows(0).Item("uvuserfield").ToString
                    UVfld = ds.Tables("data").Rows(0).Item("uvvehiclefield").ToString
                    Vdtype = ds.Tables("data").Rows(0).Item("VIDType").ToString
                    Vfld = ds.Tables("data").Rows(0).Item("vivehiclefield").ToString
                    vemei = ds.Tables("data").Rows(0).Item("viimeifield").ToString
                End If
                If Session("EID") = 32 Then
                    lbltxt.Text = "User-Vehicle"
                    'ddldhm.Items.Add("One Hourly")
                    'ddldhm.Items.Add("Two Hourly")
                    'ddlrtype.Items.Add("No Signal")
                    'ddlrtype.Items.Add("Pending Trip Approval")
                    'ddlrtype.Items.Add("Consolidated Trip Report")
                    If Session("USERROLE") = "SU" Or Session("USERROLE") = "FCAGGN" Or Session("USERROLE") = "BNK" Or Session("USERROLE") = "CADMIN" Or Session("USERROLE") = "FCANHQ" Then
                        'oda.SelectCommand.CommandText = "select  distinct m." & vemei & "[IMIENO], (select username from mmm_mst_user where eid=" & Session("EID") & " and uid=d." & Ufld & ")[UserName],m." & Vfld & "[VehicleNo] from mmm_mst_Doc d inner join mmm_mst_master m on m.tid=d." & UVfld & " inner join mmm_mst_gpsdata g on g.imieno=m." & vemei & " where d.eid=" & Session("EID") & " and  d.documenttype='" & Udtype & "'  and imieno is not null order by username "
                        oda.SelectCommand.CommandText = "select  distinct m." & vemei & "[IMIENO], u.username,m." & Vfld & "[VehicleNo] from mmm_mst_Doc d inner join mmm_mst_master m on convert(nvarchar,m.tid)=d." & UVfld & "  inner join mmm_mst_user u on u.uid=d." & Ufld & " where d.eid=" & Session("EID") & " and  d.documenttype='" & Udtype & "'  and  m." & vemei & " is not null and  m." & vemei & " <>'' order by username "
                    ElseIf Session("USERROLE").ToString.ToUpper() = "USER" Then
                        oda.SelectCommand.CommandText = "select  distinct m." & vemei & "[IMIENO], u.username,m." & Vfld & "[VehicleNo] from mmm_mst_Doc d inner join mmm_mst_master m on convert(nvarchar,m.tid)=d." & UVfld & " inner join mmm_mst_gpsdata g on g.imieno=m." & vemei & " inner join mmm_mst_user u on u.uid=d." & Ufld & " where d.eid=" & Session("EID") & " and  d.documenttype='" & Udtype & "' and d." & Ufld & "= " & Session("UID").ToString() & "  and imieno is not null order by username "
                    ElseIf Session("USERROLE").ToString.ToUpper() = "VENDOR" Then
                        oda.SelectCommand.CommandText = "select distinct m." & vemei & "[IMIENO], m." & Vfld & "[VehicleNo] from  mmm_mst_user u inner join mmm_mst_master m on m.fld12=convert(nvarchar,u.extid) where m.documenttype='vehicle' and extid=" & Session("EXTID") & " and m." & vemei & " is not null and m." & vemei & "<>''"
                    Else
                        'If IsNothing(Session("SUBUID")) Then
                        '    oda.SelectCommand.CommandText = "select  distinct m." & vemei & "[IMIENO], u.username,m." & Vfld & "[VehicleNo] from mmm_mst_Doc d inner join mmm_mst_master m on m.tid=d." & UVfld & " inner join mmm_mst_gpsdata g on g.imieno=m." & vemei & " inner join mmm_mst_user u on u.uid=d." & Ufld & " where d.eid=" & Session("EID") & " and  d.documenttype='" & Udtype & "' and d." & Ufld & " in (" & Session("UID").ToString() & ")  and imieno is not null order by username "
                        'Else
                        '    oda.SelectCommand.CommandText = "select  distinct m." & vemei & "[IMIENO], u.username,m." & Vfld & "[VehicleNo] from mmm_mst_Doc d inner join mmm_mst_master m on m.tid=d." & UVfld & " inner join mmm_mst_gpsdata g on g.imieno=m." & vemei & " inner join mmm_mst_user u on u.uid=d." & Ufld & " where d.eid=" & Session("EID") & " and  d.documenttype='" & Udtype & "' and d." & Ufld & " in (" & Session("SUBUID").ToString() & ")  and imieno is not null order by username "
                        'End If
                        oda.SelectCommand.CommandType = CommandType.StoredProcedure
                        ' oda.SelectCommand.CommandText = "uspGetRoleUID"
                        oda.SelectCommand.CommandText = "uspGetRoleUIDWithSUID"
                        oda.SelectCommand.Parameters.Clear()
                        oda.SelectCommand.Parameters.AddWithValue("uid", Session("UID"))
                        oda.SelectCommand.Parameters.AddWithValue("eid", Session("EID"))
                        If IsNothing(Session("SUBUID")) Then
                            oda.SelectCommand.Parameters.AddWithValue("SUID", Session("UID"))
                        Else
                            oda.SelectCommand.Parameters.AddWithValue("SUID", Session("SUBUID"))
                        End If
                        oda.SelectCommand.Parameters.AddWithValue("role", Session("USERROLE"))
                    End If
                ElseIf Session("EID") = 66 Or Session("EID") = 63 Then
                    lbltxt.Text = "PhoneName-IMEI"
                    If Session("USERROLE").ToString.ToUpper = "SU" Or Session("USERROLE").ToString.ToUpper = "CADMIN" Or Session("USERROLE").ToString.ToUpper = "CORPORATEUSER" Then
                        oda.SelectCommand.CommandText = "select " & vemei & "[IMEI]," & Vfld & "[PhoneUserName] from mmm_mst_master with(nolock) where documenttype='" & Vdtype & "' and eid=" & Session("EID") & ""
                    Else
                        oda.SelectCommand.CommandText = "select " & vemei & "[IMEI]," & Vfld & "[PhoneUserName] from mmm_mst_master with(nolock) where documenttype='" & Vdtype & "' and eid=" & Session("EID") & ""
                    End If
                Else
                    lbltxt.Text = "VehicleName-VehicleNo."
                    If Session("USERROLE").ToString.ToUpper = "SU" Or Session("USERROLE").ToString.ToUpper = "CADMIN" Or Session("USERROLE").ToString.ToUpper = "CORPORATEUSER" Then
                        oda.SelectCommand.CommandText = "select fld12[IMEI],fld10[VehicleName],fld1[VehicleNo] from mmm_mst_master where documenttype='vehicle' and eid=" & Session("EID") & " and len(fld12)=15 and fld12<>''"
                    Else
                        oda.SelectCommand.CommandType = CommandType.StoredProcedure
                        ' oda.SelectCommand.CommandText = "uspGetRoleUID"
                        oda.SelectCommand.CommandText = "vehiclerightforIndus"
                        oda.SelectCommand.Parameters.Clear()
                        oda.SelectCommand.Parameters.AddWithValue("uid", Session("UID"))
                        oda.SelectCommand.Parameters.AddWithValue("eid", Session("EID"))
                        oda.SelectCommand.Parameters.AddWithValue("rolename", Session("USERROLE"))
                        oda.SelectCommand.Parameters.AddWithValue("docType", "Vehicle")
                    End If
                End If
                'oda.SelectCommand.CommandText = "select imieno,username,VehicleNo from ( select  distinct m." & vemei & "[IMIENO], (select username from mmm_mst_user where eid=" & Session("EID") & " and uid=d." & Ufld & ")[UserName],m." & Vfld & "[VehicleNo] from mmm_mst_Doc d inner join mmm_mst_master m on m.tid=d." & UVfld & " inner join mmm_mst_gpsdata g on g.imieno=m." & vemei & " where d.eid=" & Session("EID") & " and  d.documenttype='" & Udtype & "'  and imieno is not null) as table1 order by username  "
                ds.Clear()
                oda.SelectCommand.CommandTimeout = 300
                oda.Fill(ds, "vemei")
                For i As Integer = 0 To ds.Tables("vemei").Rows.Count - 1
                    If Session("USERROLE").ToString.ToUpper = "VENDOR" Then
                        UsrVeh.Items.Add(ds.Tables("vemei").Rows(i).Item(1).ToString())
                    Else
                        If Session("EID") = 66 Or Session("EID") = 63 Then
                            UsrVeh.Items.Add(ds.Tables("vemei").Rows(i).Item(1).ToString() & "-" & ds.Tables("vemei").Rows(i).Item(0).ToString())
                        Else
                            UsrVeh.Items.Add(ds.Tables("vemei").Rows(i).Item(1).ToString() & "-" & ds.Tables("vemei").Rows(i).Item(2).ToString())
                        End If
                    End If
                    UsrVeh.Items(i).Value = ds.Tables("vemei").Rows(i).Item(0).ToString()
                Next
            Catch ex As Exception
            Finally
                con.Close()
                oda.Dispose()
                con.Dispose()
            End Try
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
    <WebMethod()> _
    Public Shared Function GetReportDataSequence(str As Dictionary(Of String, Object)) As PlanVsActualData
        dsstatic.Tables.Clear()
        Dim ret = ""
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
        Dim dt As New DataTable
        Dim objcls As PlanVsActualData = New PlanVsActualData()
        Try

            Dim sdate As String = str("SDate").ToString()
            Dim tDate As String = str("TDate").ToString()
            Dim arr = DirectCast(str("Vehicles"), Array)


            Dim Imieno As String = ""
            Dim crrdate As String = Date.Now.ToString("yyyy-MM-dd HH:mm")
            If sdate = "" Then
                objcls.ErrMessage = "Please enter Start Date"
            ElseIf tDate = "" Then
                objcls.ErrMessage = "Please enter To Date"

            ElseIf CDate(sdate) > CDate(tDate) Then
                objcls.ErrMessage = "Date selection is not correct "
                ' Exit Function

            ElseIf DateTime.Parse(sdate) > DateTime.Parse(crrdate) Then
                objcls.ErrMessage = "Future start date is not allowed "
                '  Exit Function
            ElseIf DateTime.Parse(tDate) > DateTime.Parse(crrdate) Then
                objcls.ErrMessage = "Future end date is not allowed "
                ' Exit Function
            Else
                Dim imeinoallow As Integer = 0
                For i As Integer = 0 To arr.Length - 1

                    Imieno = Imieno & "'" & arr(i).ToString() & "',"
                    imeinoallow = imeinoallow + 1

                Next
                If Imieno.ToString = "" Then
                    objcls.ErrMessage = "Please select any User vehicle."
                    ' Exit Function
                Else
                    Imieno = Left(Imieno, Imieno.Length - 1)
                End If
            End If
            If (objcls.ErrMessage = "") Then
                Dim TimeVariance As Integer = 0
                Dim QueryString As String
                QueryString = "select "
                oda.SelectCommand.CommandType = CommandType.Text
                oda.SelectCommand.CommandText = "select VehicleName [Name],VehicleIMEI [Device_IMEI_No],PlannedDate [Planned_Date],  " &
                    " SiteID [Planned_Site_Name],PlannedSequenceNo [Planned_Sequence], PlannedTime, DocType,ElogbookSitefld from  mmm_mst_PlanSettings where eid =" & HttpContext.Current.Session("EID")
                Dim dtMapping As DataTable = New DataTable
                oda.Fill(dtMapping)
                If (dtMapping.Rows.Count > 0) Then
                    Dim MappingSiteElogbook As String = ""

                    For i As Integer = 0 To dtMapping.Columns.Count - 1
                        Dim strFld = dtMapping.Rows(0)(i).ToString()
                        Dim ColNm = dtMapping.Columns(i).ColumnName

                        If Not (ColNm = "ElogbookSitefld") Then
                            oda.SelectCommand.CommandType = CommandType.Text
                            oda.SelectCommand.CommandText = "select DisplayName,FieldType,FieldMapping,DropDownType,dropdown from MMM_MST_FIELDS where documenttype='" & dtMapping.Rows(0)("DocType").ToString() &
                                "' and eid=" & HttpContext.Current.Session("EID") & " and fieldmapping='" & strFld & "'"
                            Dim dt2 As New DataTable
                            oda.Fill(dt2)
                            If (dt2.Rows.Count > 0) Then

                                If (dt2.Rows(0)("DropDownType").ToString() = "MASTER VALUED") Then
                                    If (ColNm = "Planned_Site_Name") Then
                                        MappingSiteElogbook = dt2.Rows(0).Item("dropDown")
                                    End If
                                    QueryString &= " dms.udf_split('" & dt2.Rows(0).Item("dropDown") & "'," & "tblPlan." & strFld & ")[" & ColNm & "],"
                                Else

                                    QueryString &= "tblPLan." & strFld & "[" & ColNm & "],"
                                End If

                            End If
                        End If
                    Next

                    QueryString = QueryString.Remove(QueryString.LastIndexOf(","))
                    QueryString &= " , isnull(convert(varchar(50), tblOriginal.Original),'Not Visited') [Actual_Sequence]," &
    "(case when Trip_end_DateTime is null then '-'  else convert(varchar(10),tblOriginal.Trip_end_DateTime,103) end) [Actual_Visit_Date],(case when Trip_end_DateTime  is null then '-' else convert(varchar(5),tblOriginal.Trip_end_DateTime,108) end ) [Actual_Visit_Time] from (  select  d." & dtMapping.Rows(0)("Planned_Site_Name").ToString() & ",d." & dtMapping.Rows(0)("PlannedTime").ToString() & ",d." & dtMapping.Rows(0)("Name").ToString() & ",d." & dtMapping.Rows(0)("Device_IMEI_No").ToString() &
      ",d." & dtMapping.Rows(0)("Planned_Date").ToString() & ",d." & dtMapping.Rows(0)("Planned_Sequence").ToString() & ",e.Vehicle_no, e.Trip_end_datetime,row_number () over (partition by Vehicle_no order by Trip_end_datetime  ) as Original, " &
      " e." & dtMapping.Rows(0)("ElogbookSitefld") & " from mmm_mst_Doc  d left outer join mmm_mst_elogbook e on d." & dtMapping.Rows(0)("Planned_Site_Name").ToString() & " = e." & dtMapping.Rows(0)("ElogbookSitefld").ToString() & " and d." & dtMapping.Rows(0)("Device_IMEI_No").ToString() & " = e.IMEI_No" &
      " and convert(date, trip_end_datetime ) = convert(date,convert(datetime, d." & dtMapping.Rows(0)("Planned_Date").ToString() & ",3)) " &
    " where d.Documenttype ='" & dtMapping.Rows(0)("DocType").ToString() & "' and isnull(d." & dtMapping.Rows(0)("PlannedTime").ToString() & ",'') ='' and d.eid =" & HttpContext.Current.Session("EID") & " and d." & dtMapping.Rows(0)("Planned_Sequence").ToString() & " is not null " &
    " and convert(date,convert(datetime, d." & dtMapping.Rows(0)("Planned_Date").ToString() & ",3)) between convert(date, '" & sdate & "') and convert(date, '" & tDate & "') and" &
     " convert(date, e.Trip_end_datetime ) between convert(date, '" & sdate & "') and convert(date, '" & tDate & "')) tblOriginal right outer join " &
     " (select d." & dtMapping.Rows(0)("Planned_Site_Name").ToString() & ",d." & dtMapping.Rows(0)("PlannedTime").ToString() & ",d." & dtMapping.Rows(0)("Name").ToString() & ",d." & dtMapping.Rows(0)("Device_IMEI_No").ToString() &
      ",d." & dtMapping.Rows(0)("Planned_Date").ToString() & ",d." & dtMapping.Rows(0)("Planned_Sequence").ToString() & " from mmm_mst_doc d where " &
     " d.Documenttype ='" & dtMapping.Rows(0)("DocType").ToString() & "' and d.eid =" & HttpContext.Current.Session("EID") & " and d." & dtMapping.Rows(0)("Planned_Sequence").ToString() & " is not null and " &
     " convert(date,convert(datetime, d." & dtMapping.Rows(0)("Planned_Date").ToString() & ",3)) between convert(date, '" & sdate & "') and convert(date, '" & tDate & "') " &
     " and isnull(d." & dtMapping.Rows(0)("PlannedTime").ToString() & ",'') =''  and eid =" & HttpContext.Current.Session("EID") & " and d." & dtMapping.Rows(0)("Device_IMEI_No").ToString() & " in ( " & Imieno & ") ) tblPlan " &
     " on tblPlan." & dtMapping.Rows(0)("Planned_Site_Name").ToString() & " = tblOriginal." & dtMapping.Rows(0)("Planned_Site_Name").ToString() & " and tblPlan." & dtMapping.Rows(0)("Device_IMEI_No").ToString() & " = tblOriginal." & dtMapping.Rows(0)("Device_IMEI_No").ToString() &
     " order by [Name], [Planned_Date],[Planned_Sequence] "

                    oda.SelectCommand.CommandType = CommandType.Text
                    oda.SelectCommand.CommandText = QueryString
                    oda.SelectCommand.Parameters.Clear()
                    dt.Clear()
                    oda.Fill(dt)

                    dt.Columns.Remove("PlannedTime")
                    dsstatic.Tables.Add(dt)

                End If

                If dt.Rows.Count > 0 Then
                    For j As Integer = 0 To dt.Columns.Count - 1
                        dt.Columns(j).ColumnName = dt.Columns(j).ColumnName.Replace(" ", "_")
                    Next
                Else : objcls.ErrMessage = "No Record Found!"
                End If
                Dim serializerSettings As New JsonSerializerSettings()
                Dim json_serializer As New JavaScriptSerializer()
                serializerSettings.Converters.Add(New DataTableConverter())
                Dim jsonData As [String] = JsonConvert.SerializeObject(dt, Formatting.None, serializerSettings)
                objcls.Data = jsonData
            Else
                objcls.Data = ""
                objcls.Success = False

            End If
        Catch ex As Exception

            Throw
        Finally
            con.Dispose()
            oda.Dispose()
        End Try



        Return objcls
    End Function



    Protected Sub ToPdf(ByVal newDataSet As DataSet)
        Try


            Dim GridView1 As New GridView()
            GridView1.AllowPaging = False
            GridView1.DataSource = dsstatic
            GridView1.DataBind()

            Response.ContentType = "application/pdf"
            Response.AddHeader("content-disposition", _
                   "attachment;filename=PlanVsActualTimebased.pdf")
            Response.Cache.SetCacheability(HttpCacheability.NoCache)
            Dim sw As New StringWriter()
            Dim hw As New HtmlTextWriter(sw)
            GridView1.RenderControl(hw)
            Dim sr As New StringReader(sw.ToString())
            Dim pdfDoc As New Document(PageSize.A4, 10.0F, 10.0F, 10.0F, 0.0F)
            Dim htmlparser As New HTMLWorker(pdfDoc)
            PdfWriter.GetInstance(pdfDoc, Response.OutputStream)
            pdfDoc.Open()
            htmlparser.Parse(sr)
            pdfDoc.Close()
            Response.Write(pdfDoc)
            Response.End()
        Catch ex As Exception
        Finally
        End Try
    End Sub


    <WebMethod()> _
    Public Shared Function GetReportData(str As Dictionary(Of String, Object)) As PlanVsActualData
        dsstatic.Tables.Clear()
        Dim ret = ""
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
        Dim dt As New DataTable
        Dim objcls As PlanVsActualData = New PlanVsActualData()

        Try

            Dim sdate As String = str("SDate").ToString()
            Dim tDate As String = str("TDate").ToString()
            Dim arr = DirectCast(str("Vehicles"), Array)


            Dim Imieno As String = ""
            Dim crrdate As String = Date.Now.ToString("yyyy-MM-dd HH:mm")
            If sdate = "" Then
                objcls.ErrMessage = "Please enter Start Date"
            ElseIf tDate = "" Then
                objcls.ErrMessage = "Please enter To Date"

            ElseIf CDate(sdate) > CDate(tDate) Then
                objcls.ErrMessage = "Date selection is not correct "
                ' Exit Function

            ElseIf DateTime.Parse(sdate) > DateTime.Parse(crrdate) Then
                objcls.ErrMessage = "Future start date is not allowed "
                '  Exit Function
            ElseIf DateTime.Parse(tDate) > DateTime.Parse(crrdate) Then
                objcls.ErrMessage = "Future end date is not allowed "
                ' Exit Function
            Else
                Dim imeinoallow As Integer = 0
                For i As Integer = 0 To arr.Length - 1

                    Imieno = Imieno & "'" & arr(i).ToString() & "',"
                    imeinoallow = imeinoallow + 1

                Next
                If Imieno.ToString = "" Then
                    objcls.ErrMessage = "Please select any User vehicle."
                    ' Exit Function
                Else
                    Imieno = Left(Imieno, Imieno.Length - 1)
                End If
            End If
            If (objcls.ErrMessage = "") Then


                Dim TimeVariance As Integer = 0
                Dim QueryString As String
                QueryString = "select "
                oda.SelectCommand.CommandType = CommandType.Text
                oda.SelectCommand.CommandText = "select VehicleName[Name],VehicleIMEI[Device_IMEI_No],SiteID[Planned_Site_Name],PlannedDate,TimeVariance,PlannedTime,PlannedSequenceNo,DocType,SiteLatLong,SiteDoc from  mmm_mst_PlanSettings where eid =" &
                     HttpContext.Current.Session("EID")
                Dim dtMapping As DataTable = New DataTable
                oda.Fill(dtMapping)
                If (dtMapping.Rows.Count > 0) Then
                    TimeVariance = Convert.ToInt16(dtMapping.Rows(0)("TimeVariance"))

                    For i As Integer = 0 To dtMapping.Columns.Count - 1
                        Dim strFld = dtMapping.Rows(0)(i).ToString()
                        Dim ColNm = dtMapping.Columns(i).ColumnName

                        oda.SelectCommand.CommandType = CommandType.Text
                        oda.SelectCommand.CommandText = "select DisplayName,FieldType,FieldMapping,DropDownType,dropdown from MMM_MST_FIELDS where documenttype='" & dtMapping.Rows(0)("DocType").ToString() & "' and eid=" & HttpContext.Current.Session("EID") & " and fieldmapping='" & strFld & "'"
                        Dim dt2 As New DataTable
                        oda.Fill(dt2)

                        If (dt2.Rows.Count > 0) Then

                            If (dt2.Rows(0)("DropDownType").ToString() = "MASTER VALUED") Then

                                QueryString &= " dms.udf_split('" & dt2.Rows(0).Item("dropDown") & "'," & strFld & ")[" & ColNm & "],"

                            Else

                                QueryString &= strFld & "[" & ColNm & "],"

                            End If

                        End If

                    Next

                    QueryString = QueryString.Remove(QueryString.LastIndexOf(","))
                    QueryString &= " ," & dtMapping.Rows(0)("PlannedDate").ToString() & " + ' '+" & dtMapping.Rows(0)("PlannedTime") & "[Planned_DateTime], row_number() over (order by " & dtMapping.Rows(0)("PlannedDate").ToString() & "," & dtMapping.Rows(0)("PlannedTime") & ") [Visit_Sequence], 'NO' [Status], (select " & dtMapping.Rows(0)("SiteLatLong").ToString() & " from mmm_mst_master where Documenttype='" & dtMapping.Rows(0)("SiteDoc").ToString() & "' and tid =d." & dtMapping.Rows(0)("Planned_Site_Name") & ") as LatLong from mmm_mst_Doc d where Documenttype='" & dtMapping.Rows(0)("DocType").ToString() & "'" &
                    " and convert(datetime," & dtMapping.Rows(0)("PlannedDate") & ",3) between  convert(date,'" & sdate & "') and convert(date,'" & tDate & "' ) and isnull(" & dtMapping.Rows(0)("PlannedSequenceNo").ToString() & ",'')='' and " & dtMapping.Rows(0)("Device_IMEI_No").ToString() & " in (" & Replace(Imieno, "'", "") & ")  "

                End If

                oda.SelectCommand.CommandType = CommandType.Text
                oda.SelectCommand.CommandText = QueryString
                oda.SelectCommand.Parameters.Clear()
                dt.Clear()
                oda.Fill(dt)

                For i As Integer = 0 To dt.Rows.Count - 1

                    Dim cmd As SqlCommand = New SqlCommand("select Count(*)  from mmm_mst_elogbook where IMEI_NO ='" & dt.Rows(i)("Device_IMEI_No").ToString() & "' and" &
     "[DMS].[CalculateDistanceInMeter](" & dt.Rows(i)("LatLong").ToString().Split(",")(0) & "," & dt.Rows(i)("LatLong").ToString().Split(",")(1) & ",end_latitude, end_Longitude) < 100" &
     "and trip_end_Datetime between dateadd(mi,-" & TimeVariance & ",convert(datetime,convert(varchar(12), convert(date,'" & dt.Rows(i)("PlannedDate").ToString() & "',3))+' " & dt.Rows(i)("PlannedTime").ToString() & "')) " &
     "and dateadd(mi," & TimeVariance & ",convert(datetime,convert(varchar(12), convert(date,'" & dt.Rows(i)("PlannedDate").ToString() & "',3))+' " & dt.Rows(i)("PlannedTime").ToString() & "')) " &
     "  ", con)

                    con.Open()
                    Dim Count As Integer = DirectCast(cmd.ExecuteScalar(), Integer)
                    con.Close()
                    If (Count > 0) Then
                        dt.Rows(i).Item("Status") = "Yes"
                    End If

                Next
                dt.Columns.Remove("LatLong")
                dt.Columns.Remove("PlannedSequenceNo")
                dt.Columns.Remove("PlannedDate")
                dt.Columns.Remove("PlannedTime")

                dsstatic.Tables.Add(dt)

                If dt.Rows.Count > 0 Then
                    For j As Integer = 0 To dt.Columns.Count - 1
                        dt.Columns(j).ColumnName = dt.Columns(j).ColumnName.Replace(" ", "_")
                    Next
                Else : objcls.ErrMessage = "No Record Found!"
                End If
                Dim serializerSettings As New JsonSerializerSettings()
                Dim json_serializer As New JavaScriptSerializer()
                serializerSettings.Converters.Add(New DataTableConverter())
                Dim jsonData As [String] = JsonConvert.SerializeObject(dt, Formatting.None, serializerSettings)
                objcls.Data = jsonData
            Else
                objcls.Data = ""
                objcls.Success = False

            End If
        Catch ex As Exception

            Throw
        Finally
            con.Dispose()
            oda.Dispose()
        End Try



        Return objcls
    End Function

    Public Overloads Overrides Sub VerifyRenderingInServerForm(ByVal control As Control)

    End Sub

    Public Class PlanVsActualData
        Public Data As String = ""
        Public Success As Boolean
        Public ErrMessage As String = ""

    End Class


    Protected Sub checkuncheck(sender As Object, e As System.EventArgs) Handles CheckBox1.CheckedChanged
        If CheckBox1.Checked = True Then

            For Each chkitem As System.Web.UI.WebControls.ListItem In UsrVeh.Items
                chkitem.Selected = True
            Next
            'For Each smsreport As System.Web.UI.WebControls.ListItem In smsreports.Items
            '    smsreport.Selected = True
            'Next
            'UpdatePanel1.Update()
        Else
            For Each chkitem As System.Web.UI.WebControls.ListItem In UsrVeh.Items
                chkitem.Selected = False
            Next
            'For Each smsreport As System.Web.UI.WebControls.ListItem In smsreports.Items
            '    smsreport.Selected = False
            'Next
        End If
        UpdatePanel1.Update()
    End Sub



    Function getdate(ByVal dbt As String) As String
        Dim dtArr() As String
        dtArr = Split(dbt, "/")
        If dtArr.GetUpperBound(0) = 2 Then
            Dim dd, mm, yy As String
            dd = dtArr(0)
            mm = dtArr(1)
            yy = dtArr(2)
            Dim dt As String
            dt = yy & "-" & mm & "-" & dd
            Return dt
        Else
            Return Now.Date
        End If
    End Function


    Protected Sub btnExportPDF_Click(sender As Object, e As ImageClickEventArgs) Handles btnExportPDF.Click

        If (dsstatic.Tables.Count = 0) Then
            lblmsg.Text = "No record found to export."
        ElseIf (dsstatic.Tables(0).Rows.Count = 0) Then
            lblmsg.Text = "No record found to export."
        Else
            For c As Integer = 0 To dsstatic.Tables(0).Columns.Count - 1
                dsstatic.Tables(0).Columns(c).ColumnName = dsstatic.Tables(0).Columns(c).ColumnName.Replace("_", " ")
            Next

            ToPdf(dsstatic)

        End If

    End Sub

    Protected Sub btnexportxl_Click(sender As Object, e As ImageClickEventArgs) Handles btnexportxl.Click
        If (dsstatic.Tables.Count = 0) Then
            lblmsg.Text = "No record found to export."
        ElseIf (dsstatic.Tables(0).Rows.Count = 0) Then
            lblmsg.Text = "No record found to export."
        Else
            For c As Integer = 0 To dsstatic.Tables(0).Columns.Count - 1
                dsstatic.Tables(0).Columns(c).ColumnName = dsstatic.Tables(0).Columns(c).ColumnName.Replace("_", " ")
            Next
            Dim GridView1 As New GridView()
            GridView1.AllowPaging = False
            GridView1.DataSource = dsstatic
            GridView1.DataBind()

            Response.Clear()
            Response.Buffer = True
            Response.AddHeader("content-disposition", _
                 "attachment;filename=PlanVsActualTimeBased.xls")
            Response.Charset = ""
            Response.ContentType = "application/vnd.ms-excel"
            Dim sw As New StringWriter()
            Dim hw As New HtmlTextWriter(sw)

            For i As Integer = 0 To GridView1.Rows.Count - 1
                'Apply text style to each Row
                GridView1.Rows(i).Attributes.Add("class", "textmode")
            Next

            GridView1.RenderControl(hw)
            'style to format numbers to string
            Dim style As String = "<style> .textmode{mso-number-format:\@;}</style>"
            Response.Write(style)
            Response.Output.Write(sw.ToString())
            Response.Flush()
            Response.End()
        End If
    End Sub


End Class
