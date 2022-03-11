Imports System.Data.SqlClient
Imports System.Data
Imports System.Web.Services
Imports Newtonsoft.Json
Imports System.Web.Script.Serialization
Imports Newtonsoft.Json.Converters

Partial Class EcomplianceReport
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

            FillDDL()

        End If

    End Sub

    Sub FillDDL()
        Try


            'Dim constr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            'Dim con As New SqlConnection(constr)
            'Dim dt As New DataTable

            'Dim oda As SqlDataAdapter = New SqlDataAdapter("select fld1 ,tid  from mmm_mst_master  where eid =98 and documenttype ='Company master' ", con)
            'oda.Fill(dt)
            'ddlCompany.DataSource = dt
            'ddlCompany.DataTextField = "fld1"
            'ddlCompany.DataValueField = "tid"
            'ddlCompany.DataBind()
            'ddlCompany.Items.Insert(0, New ListItem("Select Company", "-1"))
            'dt.Clear()

            '' Fill Site DDL
            'oda = New SqlDataAdapter("select fld100 ,tid from mmm_mst_master  where eid =98 and documenttype ='Site master' ", con)
            'oda.Fill(dt)
            'ddlSite.DataSource = dt
            'ddlSite.DataTextField = "fld100"
            'ddlSite.DataValueField = "tid"
            'ddlSite.DataBind()
            '    ddlSite.Items.Insert(0, New ListItem("All", "0"))
            'dt.Clear()

            For i As Integer = 2015 To DateTime.Now.Year
                ddlYear.Items.Add(New ListItem(i.ToString(), i.ToString()))
            Next
            ddlYear.SelectedValue = DateTime.Now.Year.ToString()
            ddlMonth.SelectedValue = DateTime.Now.Month.ToString()
        Catch ex As Exception

        End Try

    End Sub

    <WebMethod> _
    Public Shared Function GetJSON(Company As String, Site As String, Month As String, year As String, UID As String, Urole As String) As eReport
        Dim jsonData As String = ""

        Dim res As New eReport()

        Try
            Dim ds As New DataSet()
            Dim constr1 As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con1 As New SqlConnection(constr1)
            Dim dt1 As New DataTable
            Dim cmd As SqlCommand = New SqlCommand("select docmapping from mmm_mst_forms where eid =98 and  formname ='Site Master' and isroledef=1", con1)
            con1.Open()
            Dim sitedocmapping = cmd.ExecuteScalar()
            con1.Close()

            Dim str As String = " (case when Frequency='As Needed' then '01/01/1900'  when frequency='Daily' then convert(datetime, '01/'+@month +'/'+@year,105) " &
       " when frequency ='Monthly' then   dateadd(day , -(convert(int,[RemindDays])), convert(datetime, DOM +'/'+ cast( @Month as varchar) +'/'+ cast(@year as varchar),105)) " &
                     " when frequency ='Yearly' then dateadd(day , -(convert(int,[RemindDays])), convert(datetime, DOM +'/'+ [Month]+'/'+ convert(varchar,@year),105)) " &
            " when frequency ='Quarterly' then  (case when Month( dateadd(day , -(convert(int,[RemindDays])), convert(datetime, DOM +'/'+  [Month] +'/'+ convert(varchar,@year),105)) ) = @Month " &
              " then dateadd(day , -(convert(int,[RemindDays])), convert(datetime, DOM +'/'+ [Month]+'/'+ convert(varchar,@year),105)) " &
               " when Month(dateadd(Month,3, dateadd(day , -(convert(int,[RemindDays])), convert(datetime, DOM +'/'+ [Month] +'/'+ convert(varchar,@year),105))) ) = @Month " &
              " then dateadd(month,3, dateadd(day , -(convert(int,[RemindDays])), convert(datetime, DOM +'/' + [Month]+'/'+ convert(varchar,@year),105)) )" &
              " when Month(dateadd(Month,6, dateadd(day , -(convert(int,[RemindDays])), convert(datetime, DOM+'/' + [Month]+'/' + convert(varchar,@year),105))) ) = @Month" &
              " then dateadd(month,6, dateadd(day , -(convert(int,[RemindDays])), convert(datetime, DOM +'/'+ [Month]+'/'+ convert(varchar,@year),105)) )" &
               " when Month(dateadd(Month,9, dateadd(day , -(convert(int,[RemindDays])), convert(datetime, DOM +'/' + [Month]+'/' + convert(varchar,@year),105))) ) = @Month " &
                " then dateadd(month,9, dateadd(day , -(convert(int,[RemindDays])), convert(datetime, DOM +'/' + [Month]+'/'+ convert(varchar,@year),105)) )" &
                                "		else convert(datetime,'',101) end ) " &
    " when frequency='Half Yearly' then  (case when Month(  dateadd(day , -(convert(int,[RemindDays])), convert(datetime, DOM +'/' + [Month]+'/'+ convert(varchar,@year),105)))=@Month then " &
            "		  	    dateadd(day , -(convert(int,[RemindDays])), convert(datetime, DOM +'/'+ [Month]+'/'+ convert(varchar,@year),105)) " &
            "						 when Month( dateadd( Month,6,dateadd(day , -(convert(int,[RemindDays])), convert(datetime, DOM +'/'+ [Month]+'/'+ convert(varchar,@year),105))))=@Month " &
            "  then dateadd(Month,6,dateadd(day , -(convert(int,[RemindDays])), convert(datetime, DOM +'/'+ [Month]+'/'+ convert(varchar,@year),105))) " &
            "  else  convert(datetime,'',101) end)" &
            "  when frequency ='bi-annual' then  dateadd(day , -(convert(int,[RemindDays])), convert(datetime, DOM +'/'+ [Month]+'/'+ convert(varchar,@year),105))" &
            "   else dateadd(day , -(convert(int,[RemindDays])), convert(datetime, DOM +'/' + [Month]+'/'+ convert(varchar,@year),105))   end ) [Due Date]"

            Dim Query As String = ""
            Query &= " declare @Month int  =" & Month & " ; declare @Year int =" & year & ";  select PEActDoc.Tid,isnull(PEActDoc.File1,'')File1,isnull(PEActDoc.File2,'')File2,isnull(PEActDoc.File3,'')File3,isnull(PEActDoc.File4,'')File4,isnull(PEActDoc.File5,'')File5," &
                " isnull(PEActDoc.File1D,'')[File1D],isnull(PEActDoc.File2D,'')[File2D],isnull(PEActDoc.File3D,'')[File3D],isnull(PEActDoc.File4D,'')[File4D],isnull(PEActDoc.File5D,'')[File5D]," &
                " [Company],[Site],ActsActivity.Act,ActsActivity.Activity, " &
                " ( case when PEActDoc.PE is not null then  PEActDoc.PE else '-' end ) PE, " &
  " (case when  PEActDoc.PE ='Not Performed' then 0 when PEActDoc.PE is null or PEActDoc.PE ='-' then null  else 100 end )[Percentage],	convert(varchar(10),  [Due Date],3)[Due Date] from  (select dms.udf_split('MASTER-Company Master-fld1',Actsapplicable.fld5) [Company], Activity.[Due Date], Actsapplicable.fld3 [ActID],Actsapplicable.fld4 [Act],Activity.fld112 [Activity], " &
 "  Activity.tid [activityID] ,SiteMaster.fld100[Site], sitemaster.tid [siteid],Actsapplicable.fld5 [Companyid] from mmm_mst_master SiteMaster  inner join mmm_mst_master Actsapplicable on" &
"   SiteMaster.tid =Actsapplicable.fld1 inner join mmm_mst_master Company on Actsapplicable.fld5 = Company.tid  inner join (" &
 " select *, " + str + " from (select tid, fld1,fld112, dms.udf_split('MASTER-Frequency Master-fld1',Activity1.fld100)[Frequency], (case when (@Month=2 and cast( Activity1.fld101 as int) >28) then '28' else Activity1.fld101 end) DOM, " &
 " Activity1.fld109 [DOY],Activity1.fld110 [Month], Activity1.fld111 [DOW],Activity1.fld102[RemindDays],Activity1.fld113 [Year],(case when  Activity1.fld110 ='' then '' when  Activity1.fld110='JAN' then '1' when  Activity1.fld110='FEB' then '2'when  Activity1.fld110='MAR' then '3' " &
" when  Activity1.fld110 ='APR' then '4' when  Activity1.fld110='MAY' then 5 when  Activity1.fld110='JUN'then '6' when  Activity1.fld110='JUL' then '7' when  Activity1.fld110='AUG' then '8' when  Activity1.fld110='SEP' then '9' when  Activity1.fld110='OCT' then '10' when  Activity1.fld110='NOV' then '11' " &
" when  Activity1.fld110='DEC' then '12' end ) [MonthNum]   from mmm_mst_master Activity1 where eid =98 and documenttype ='Activity master' and isauth=1 and fld108 ='pe') tbl where  (case when Frequency='As Needed' then 0 " &
 " when frequency='Daily' then 1 when frequency ='Monthly' then  1 when frequency ='Yearly' then case when Month(  dateadd(day , -(convert(int,[RemindDays])), convert(datetime, DOM +'/'+ [Month]+'/'+ convert(varchar,@year),105))) = @Month then 1 else 0 end " &
 " when frequency ='Quarterly' then   (case when Month( dateadd(day , -(convert(int,[RemindDays])), convert(datetime, DOM +'/'+  [Month] +'/'+ convert(varchar,@year),105)) ) = @Month     then 1  when Month(dateadd(Month,3, dateadd(day , -(convert(int,[RemindDays])), convert(datetime, DOM +'/'+ [Month] +'/'+ convert(varchar,@year),105))) ) = @Month  " &
 " then 1  when Month(dateadd(Month,6, dateadd(day , -(convert(int,[RemindDays])), convert(datetime, DOM+'/' + [Month]+'/' + convert(varchar,@year),105))) ) = @Month    then 1 " &
 " when Month(dateadd(Month,9, dateadd(day , -(convert(int,[RemindDays])), convert(datetime, DOM +'/' + [Month]+'/' + convert(varchar,@year),105))) ) = @Month     then 1 else 0 end ) " &
 "  when frequency='Half Yearly' then  (case when Month(  dateadd(day , -(convert(int,[RemindDays])), convert(datetime, DOM +'/' + [Month]+'/'+ convert(varchar,@year),105)))=@Month  then 1 " &
  " when Month( dateadd( Month,6,dateadd(day , -(convert(int,[RemindDays])), convert(datetime, DOM +'/'+ [Month]+'/'+ convert(varchar,@year),105))))=@Month   then 1 else 0  end) " &
 " when frequency ='bi-annual' and (@Year-[year] %2) =0  then ( case when month( dateadd(day , -(convert(int,[RemindDays])), convert(datetime, DOM +'/'+ [Month]+'/'+ convert(varchar,@year),105))) = @Month then 1 else 0 end) else 0 end) =1)  Activity " &
  "  on Activity.fld1 = Actsapplicable.fld3  where  SiteMaster.eid=98 and  SiteMaster.Documenttype='Site Master' and SiteMaster.isauth=1 "

            ' Add when new 
            '  (case when convert(datetime,PE.fld4,3)<= convert(datetime, fld43,3) then  'Performed' 
            ' when ( convert(datetime,PE.fld4,3)> convert(datetime, fld43,3)  and PE.curstatus <> 'Archive')then 'Not Performed'
            '  else '-' end )  PE 
            If (Site <> 0) Then
                Query &= " and SiteMaster.tid= " & Site
            End If

            If sitedocmapping <> "" And Urole <> "SU" And Urole <> "SU" Then
                Query &= " and SiteMaster.tid in (select items from dbo.split((select " & sitedocmapping & " from  mmm_ref_role_user where eid =100 and uid =" & UID & " and rolename ='" & Urole & "'),',')) "
            End If

            Query &= " and Actsapplicable.eid=98  and Actsapplicable.fld5='" & Company & "'  and Actsapplicable.isauth=1 and Actsapplicable.documenttype ='Acts Applicable To Site' and dms.udf_split('MASTER-Company Master-fld1',actsapplicable.fld5) not like '%telenor%' " &
    " and Company.isauth=1  ) ActsActivity   left join (select PE.tid, (case when (Convert(datetime,PE.fld4 ,3) <= Convert(datetime,PE.fld43 ,3)) then  'Performed' when " &
                "   Convert(datetime,PE.fld4 ,3) > Convert(datetime,PE.fld43 ,3)   then 'Not Performed' " &
                " when fld4 is null and (convert(datetime,getdate(),3) > Convert(datetime,PE.fld43 ,3)) then 'Not Performed' " &
             " when fld4 is  null and (convert(datetime,getdate(),3) <= Convert(datetime,PE.fld43 ,3)) then '-' " &
                "else '-' end )  " &
   " PE , fld47,fld31,fld7,fld5 ,fld24, isnull( fld10,'') [File1] ,isnull(fld11,'')[File2],isnull(fld12,'')[File3],isnull(fld13,'')[File4],isnull(fld14,'') [File5], " &
   " isnull(fld48,'') [File1D],isnull(fld49,'') [File2D],isnull(fld50,'') [File3D],isnull(fld51,'') [File4D],isnull(fld52,'') [File5D]  from mmm_mst_doc  PE where eid =98 and Documenttype ='Act Document' and fld25='PE' and month(adate) =cast(" & Month & "   as int) and  year(adate) =cast(" & year & "  as int) ) " &
  "  PEActDoc  on ActsActivity.ActivityID = PEActDoc.fld24 and ActsActivity.Act = PEActDoc.fld31 and ActsActivity.[siteid] = PEActDoc.fld7 and ActsActivity.[Companyid] =PEActDoc.fld5 order by [Company],[Site],Act,Activity "


            Dim constr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As New SqlConnection(constr)
            Dim dt As New DataTable
            Dim oda As SqlDataAdapter = New SqlDataAdapter(Query, con)
            oda.Fill(dt)
            Dim lstColumns As New List(Of grdcolumns)
            Dim objColumn As grdcolumns

            For i As Integer = 0 To dt.Columns.Count - 1
                If (dt.Columns(i).ColumnName <> "Tid" And (Not dt.Columns(i).ColumnName.Contains("File1")) And (Not dt.Columns(i).ColumnName.Contains("File2")) And (Not dt.Columns(i).ColumnName.Contains("File3")) And (Not dt.Columns(i).ColumnName.Contains("File4")) And (Not dt.Columns(i).ColumnName.Contains("File5"))) Then
                    objColumn = New grdcolumns()
                    objColumn.field = Replace(dt.Columns(i).ColumnName, " ", "_")
                    objColumn.title = dt.Columns(i).ColumnName

                    If (dt.Columns(i).ColumnName.ToString = "Percentage") Then

                        objColumn.aggregates = "[""average""]"
                        objColumn.groupFooterTemplate = ""
                        objColumn.type = "number"

                        ' objColumn.groupFooterTemplate = ""


                    End If

                    If (dt.Columns(i).ColumnName.ToString() = "Company") Then
                        objColumn.groupHeaderTemplate = "# if( aggregates.Percentage.average == null ) {# #=value #  : (Average : -)#} else {# #=value #  : (Average : #=kendo.format('{0:n}',aggregates.Percentage.average)#%)#} #"
                        ' objColumn.groupHeaderTemplate = " #=value# : (Average : #=kendo.format('{0:n}',aggregates.Percentage.average)#%)"
                    End If
                    If (dt.Columns(i).ColumnName.ToString() = "Site") Then
                        objColumn.groupHeaderTemplate = "# if( aggregates.Percentage.average == null ) {# #=value #  : (Average : -)#} else {# #=value #  : (Average : #=kendo.format('{0:n}',aggregates.Percentage.average)#%)#} #" ' " #=value# : (Average : #=kendo.format('{0:n}',aggregates.Percentage.average)#%)"
                    End If
                    If (dt.Columns(i).ColumnName.ToString() = "Act") Then
                        objColumn.groupHeaderTemplate = "# if( aggregates.Percentage.average == null ) {# #=value #  : (Average : -)#} else {# #=value #  : (Average : #=kendo.format('{0:n}',aggregates.Percentage.average)#%)#} #" '  " #=value# : (Average : #=kendo.format('{0:n}',aggregates.Percentage.average)#%)"
                    End If

                    lstColumns.Add(objColumn)
                    dt.Columns(i).ColumnName = Replace(dt.Columns(i).ColumnName, " ", "_")
                End If
            Next


            Dim serializerSettings As New JsonSerializerSettings()
            Dim json_serializer As New JavaScriptSerializer()

            serializerSettings.Converters.Add(New DataTableConverter())

            jsonData = JsonConvert.SerializeObject(dt, Newtonsoft.Json.Formatting.None, serializerSettings)
            res.data = jsonData
            res.columns = lstColumns



        Catch Ex As Exception
            Throw
        End Try
        Return res

    End Function
    <WebMethod> _
    Public Shared Function GetJSONcontractor(Company As String, Site As String, Month As String, year As String, UID As String, Urole As String) As eReport
        Dim jsonData As String = ""
        Dim constr1 As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con1 As New SqlConnection(constr1)
        Dim cmd As SqlCommand = New SqlCommand("select docmapping from mmm_mst_forms where eid =98 and  formname ='Site Master' and isroledef=1", con1)
        con1.Open()
        Dim sitedocmapping = cmd.ExecuteScalar()
        con1.Close()
        Dim res As New eReport()

        Try
            Dim ds As New DataSet()

            Dim str As String = " (case when Frequency='As Needed' then '01/01/1900'  when frequency='Daily' then convert(datetime, '01/'+@month +'/'+@year,105) " &
       " when frequency ='Monthly' then   dateadd(day , -(convert(int,[RemindDays])), convert(datetime, DOM +'/'+ cast( @Month as varchar) +'/'+ cast(@year as varchar),105)) " &
                     " when frequency ='Yearly' then dateadd(day , -(convert(int,[RemindDays])), convert(datetime, DOM +'/'+ [Month]+'/'+ convert(varchar,@year),105)) " &
            " when frequency ='Quarterly' then  (case when Month( dateadd(day , -(convert(int,[RemindDays])), convert(datetime, DOM +'/'+  [Month] +'/'+ convert(varchar,@year),105)) ) = @Month " &
              " then dateadd(day , -(convert(int,[RemindDays])), convert(datetime, DOM +'/'+ [Month]+'/'+ convert(varchar,@year),105)) " &
               " when Month(dateadd(Month,3, dateadd(day , -(convert(int,[RemindDays])), convert(datetime, DOM +'/'+ [Month] +'/'+ convert(varchar,@year),105))) ) = @Month " &
              " then dateadd(month,3, dateadd(day , -(convert(int,[RemindDays])), convert(datetime, DOM +'/' + [Month]+'/'+ convert(varchar,@year),105)) )" &
              " when Month(dateadd(Month,6, dateadd(day , -(convert(int,[RemindDays])), convert(datetime, DOM+'/' + [Month]+'/' + convert(varchar,@year),105))) ) = @Month" &
              " then dateadd(month,6, dateadd(day , -(convert(int,[RemindDays])), convert(datetime, DOM +'/'+ [Month]+'/'+ convert(varchar,@year),105)) )" &
               " when Month(dateadd(Month,9, dateadd(day , -(convert(int,[RemindDays])), convert(datetime, DOM +'/' + [Month]+'/' + convert(varchar,@year),105))) ) = @Month " &
                " then dateadd(month,9, dateadd(day , -(convert(int,[RemindDays])), convert(datetime, DOM +'/' + [Month]+'/'+ convert(varchar,@year),105)) )" &
                                "		else convert(datetime,'',101) end ) " &
    " when frequency='Half Yearly' then  (case when Month(  dateadd(day , -(convert(int,[RemindDays])), convert(datetime, DOM +'/' + [Month]+'/'+ convert(varchar,@year),105)))=@Month then " &
            "		  	    dateadd(day , -(convert(int,[RemindDays])), convert(datetime, DOM +'/'+ [Month]+'/'+ convert(varchar,@year),105)) " &
            "						 when Month( dateadd( Month,6,dateadd(day , -(convert(int,[RemindDays])), convert(datetime, DOM +'/'+ [Month]+'/'+ convert(varchar,@year),105))))=@Month " &
            "  then dateadd(Month,6,dateadd(day , -(convert(int,[RemindDays])), convert(datetime, DOM +'/'+ [Month]+'/'+ convert(varchar,@year),105))) " &
            "  else  convert(datetime,'',101) end)" &
            "  when frequency ='bi-annual' then  dateadd(day , -(convert(int,[RemindDays])), convert(datetime, DOM +'/'+ [Month]+'/'+ convert(varchar,@year),105))" &
            "   else dateadd(day , -(convert(int,[RemindDays])), convert(datetime, DOM +'/' + [Month]+'/'+ convert(varchar,@year),105))   end ) [Due Date]"

            Dim Query As String = ""
            Query &= " declare @Month int  =" & Month & " ; declare @Year int =" & year & "; select ContractorDoc.Tid,isnull(ContractorDoc.File1,'')File1,isnull(ContractorDoc.File2,'')File2,isnull(ContractorDoc.File3,'')File3,isnull(ContractorDoc.File4,'')File4,isnull(ContractorDoc.File5,'')File5," &
                " isnull(ContractorDoc.File1D,'')[File1D],isnull(ContractorDoc.File2D,'')[File2D],isnull(ContractorDoc.File3D,'')[File3D],isnull(ContractorDoc.File4D,'')[File4D],isnull(ContractorDoc.File5D,'')[File5D]," &
                " [Company],[Site],ActsActivity.Act,ActsActivity.Activity,ActsActivity.[Contractor], case when ContractorDoc.Contractorstatus is not null then ContractorDoc.Contractorstatus else '-' end ContractorStatus, (case when " &
" ContractorDoc.Contractorstatus ='Not Performed' then 0 when ContractorDoc.Contractorstatus is null or ContractorDoc.Contractorstatus='-' then null else 100 end )[Percentage] ,	convert(varchar(10),  [Due Date],3)[Due Date] from (select dms.udf_split('MASTER-Company Master-fld1',Actsapplicable.fld5) [Company], Actsapplicable.fld3 [ActID],Actsapplicable.fld4 [Act],Activity.fld112 [Activity]," &
 " Activity.tid [activityID] ,Activity.[Due Date],SiteMaster.fld100[Site], sitemaster.tid [siteid],Actsapplicable.fld5 [Companyid] ,sitecontractor.fld5[Contractor]   from mmm_mst_master SiteMaster  inner join mmm_mst_master Actsapplicable on" &
" SiteMaster.tid = Actsapplicable.fld1  inner join mmm_mst_master Company on Actsapplicable.fld5 = company.tid  inner join " &
 " ( select * ," + str + " from ( select tid, fld1,fld112, dms.udf_split('MASTER-Frequency Master-fld1',Activity1.fld100)[Frequency], (case when (@Month=2 and cast( Activity1.fld101 as int) >28) then '28' else Activity1.fld101 end) DOM," &
" Activity1.fld109 [DOY],Activity1.fld110 [Month], Activity1.fld111 [DOW],Activity1.fld102[RemindDays], Activity1.fld113 [Year],(case when  Activity1.fld110 ='' then '' when  Activity1.fld110='JAN' then '1' when  Activity1.fld110='FEB' then '2'when  Activity1.fld110='MAR' then '3' when  Activity1.fld110 ='APR' then '4' when  Activity1.fld110='MAY' then 5 when  Activity1.fld110='JUN'then '6' when  Activity1.fld110='JUL' then '7'  " &
 " when  Activity1.fld110='AUG' then '8' when  Activity1.fld110='SEP' then '9' when  Activity1.fld110='OCT' then '10' when  Activity1.fld110='NOV' then '11'  when  Activity1.fld110='DEC' then '12' end ) [MonthNum]   from mmm_mst_master Activity1 where eid =98 and documenttype ='Activity master' and fld108 ='contractor' and  isauth=1) tbl where  (case when Frequency='As Needed' then 0  when frequency='Daily' then 1  when frequency ='Monthly' then  1 " &
 " when frequency ='Yearly' then case when Month(  dateadd(day , -(convert(int,[RemindDays])), convert(datetime, DOM +'/'+ [Month]+'/'+ convert(varchar,@year),105))) = @Month then 1 else 0 end when frequency ='Quarterly' then  (case when Month( dateadd(day , -(convert(int,[RemindDays])), convert(datetime, DOM +'/'+  [Month] +'/'+ convert(varchar,@year),105)) ) = @Month  " &
  " then 1  when Month(dateadd(Month,3, dateadd(day , -(convert(int,[RemindDays])), convert(datetime, DOM +'/'+ [Month] +'/'+ convert(varchar,@year),105))) ) = @Month 	 then 1   when Month(dateadd(Month,6, dateadd(day , -(convert(int,[RemindDays])), convert(datetime, DOM+'/' + [Month]+'/' + convert(varchar,@year),105))) ) = @Month " &
  "  then 1  when Month(dateadd(Month,9, dateadd(day , -(convert(int,[RemindDays])), convert(datetime, DOM +'/' + [Month]+'/' + convert(varchar,@year),105))) ) = @Month    then 1  else 0 	end )  when frequency='Half Yearly' then " &
 " (case when Month(  dateadd(day , -(convert(int,[RemindDays])), convert(datetime, DOM +'/' + [Month]+'/'+ convert(varchar,@year),105)))=@Month  then 1  when Month( dateadd( Month,6,dateadd(day , -(convert(int,[RemindDays])), convert(datetime, DOM +'/'+ [Month]+'/'+ convert(varchar,@year),105))))=@Month    then 1 else 0  end) " &
 " when frequency ='bi-annual' and (@Year-[year] %2) =0  then ( case when month( dateadd(day , -(convert(int,[RemindDays])), convert(datetime, DOM +'/'+ [Month]+'/'+ convert(varchar,@year),105))) = @Month then 1 else 0 end)  else 0 end) =1)  Activity " &
 " on Activity.fld1 = Actsapplicable.fld3  left join mmm_mst_master sitecontractor on  Actsapplicable.fld1 = sitecontractor.fld3  where sitecontractor.eid =98 and sitecontractor.Documenttype ='Site contractor master' and  SiteMaster.eid=98 and " &
  " SiteMaster.Documenttype='Site Master' and sitecontractor.isauth=1 and Actsapplicable.isauth=1  and  SiteMaster.isauth=1  and company.isauth=1 "

            If (Site <> 0) Then
                Query &= " and SiteMaster.tid= " & Site
            End If
            If sitedocmapping <> "" And Urole <> "SU" And Urole <> "SU" Then
                Query &= " and SiteMaster.tid in (select items from dbo.split((select " & sitedocmapping & " from  mmm_ref_role_user where eid =100 and uid =" & UID & " and rolename ='" & Urole & "'),',')) "
            End If

            Query &= " and Actsapplicable.eid=98  and Actsapplicable.fld5='" & Company & "' and Actsapplicable.documenttype ='Acts Applicable To Site' ) ActsActivity " &
 "   left join (select PE.tid,(case when (Convert(datetime,PE.fld4 ,3) <= Convert(datetime,PE.fld43 ,3)) then  'Performed' when " &
                  " Convert(datetime,PE.fld4 ,3) > Convert(datetime,PE.fld43 ,3)   then 'Not Performed' " &
                   " when fld4 is null and (convert(datetime,getdate(),3) > Convert(datetime,PE.fld43 ,3)) then 'Not Performed' " &
             " when fld4 is null and (convert(datetime,getdate(),3) <= Convert(datetime,PE.fld43 ,3)) then '-' " &
                  "else '-' end ) Contractorstatus, fld44 [Contractor],fld46 ,fld19, fld31 ,fld1,fld7,fld5, isnull( fld10,'') [File1] ,isnull(fld11,'')[File2],isnull(fld12,'')[File3],isnull(fld13,'')[File4],isnull(fld14,'') [File5]," &
 "  isnull(fld48,'') [File1D],isnull(fld49,'') [File2D],isnull(fld50,'') [File3D],isnull(fld51,'') [File4D],isnull(fld52,'') [File5D] from mmm_mst_doc  PE where eid =98 and Documenttype ='Act Document' and fld25='Contractor' " &
     " and month(adate) = cast( " & Month & "  as int) and year(adate) =cast( " & year & "   as int)  and isauth=1 )  ContractorDoc on ActsActivity.ActivityID = ContractorDoc.fld19 and   ActsActivity.Act = ContractorDoc.fld31 And  ActsActivity.[siteid] = ContractorDoc.fld7 And ActsActivity.[Companyid] = ContractorDoc.fld5 " &
    " and  ActsActivity.Contractor = ContractorDoc.Contractor order by [Company],[Site],Act,Activity,[Contractor]"

            Dim constr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As New SqlConnection(constr)
            Dim dt As New DataTable
            Dim oda As SqlDataAdapter = New SqlDataAdapter(Query, con)
            oda.SelectCommand.CommandTimeout = 200

            oda.Fill(dt)
            Dim lstColumns As New List(Of grdcolumns)
            Dim objColumn As grdcolumns

            For i As Integer = 0 To dt.Columns.Count - 1
                If (dt.Columns(i).ColumnName <> "Tid" And (Not dt.Columns(i).ColumnName.Contains("File1")) And (Not dt.Columns(i).ColumnName.Contains("File2")) And (Not dt.Columns(i).ColumnName.Contains("File3")) And (Not dt.Columns(i).ColumnName.Contains("File4")) And (Not dt.Columns(i).ColumnName.Contains("File5"))) Then


                    objColumn = New grdcolumns()
                    objColumn.field = Replace(dt.Columns(i).ColumnName, " ", "_")
                    objColumn.title = dt.Columns(i).ColumnName

                    If (dt.Columns(i).ColumnName.ToString = "Percentage") Then

                        objColumn.aggregates = "[""average""]"
                        objColumn.groupFooterTemplate = ""
                        objColumn.type = "number"
                        ' objColumn.groupFooterTemplate = ""


                    End If

                    If (dt.Columns(i).ColumnName.ToString() = "Company") Then
                        objColumn.groupHeaderTemplate = "# if( aggregates.Percentage.average == null ) {# #=value #  : (Average : -)#} else {# #=value #  : (Average : #=kendo.format('{0:n}',aggregates.Percentage.average)#%)#} #"
                        'objColumn.groupHeaderTemplate = " #=value# : (Average : #=kendo.format('{0:n}',aggregates.Percentage.average)#%)"
                    End If
                    If (dt.Columns(i).ColumnName.ToString() = "Site") Then
                        objColumn.groupHeaderTemplate = "# if( aggregates.Percentage.average == null ) {# #=value #  : (Average : -)#} else {# #=value #  : (Average : #=kendo.format('{0:n}',aggregates.Percentage.average)#%)#} #"
                        'objColumn.groupHeaderTemplate = " #=value# : (Average : #=kendo.format('{0:n}',aggregates.Percentage.average)#%)"
                    End If
                    If (dt.Columns(i).ColumnName.ToString() = "Act") Then
                        objColumn.groupHeaderTemplate = "# if( aggregates.Percentage.average == null ) {# #=value #  : (Average : -)#} else {# #=value #  : (Average : #=kendo.format('{0:n}',aggregates.Percentage.average)#%)#} #"
                        ' objColumn.groupHeaderTemplate = " #=value# : (Average : #=kendo.format('{0:n}',aggregates.Percentage.average)#%)"
                    End If
                    If (dt.Columns(i).ColumnName.ToString() = "Contractor") Then
                        objColumn.groupHeaderTemplate = "# if( aggregates.Percentage.average == null ) {# #=value #  : (Average : -)#} else {# #=value #  : (Average : #=kendo.format('{0:n}',aggregates.Percentage.average)#%)#} #"
                        'objColumn.groupHeaderTemplate = " #=value# : (Average : #=kendo.format('{0:n}',aggregates.Percentage.average)#%)"
                    End If
                    If (dt.Columns(i).ColumnName.ToString() = "") Then

                    End If
                    lstColumns.Add(objColumn)
                    dt.Columns(i).ColumnName = Replace(dt.Columns(i).ColumnName, " ", "_")
                End If
            Next



            Dim serializerSettings As New JsonSerializerSettings()
            Dim json_serializer As New JavaScriptSerializer()

            serializerSettings.Converters.Add(New DataTableConverter())



            jsonData = JsonConvert.SerializeObject(dt, Newtonsoft.Json.Formatting.None, serializerSettings)
            res.data = jsonData
            res.columns = lstColumns



        Catch Ex As Exception
            Throw
        End Try
        Return res

    End Function

    'Protected Sub ddlCompany_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlCompany.SelectedIndexChanged

    '    Try

    '        Dim constr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
    '        Dim con As New SqlConnection(constr)
    '        Dim dt As New DataTable

    '        'Fill Site DDL
    '        Dim oda = New SqlDataAdapter("select fld100 ,tid from mmm_mst_master  where eid =98 and documenttype ='Site master' and  fld1 =" & ddlCompany.SelectedValue & "", con)
    '        oda.Fill(dt)
    '        ddlSite.Items.Clear()
    '        ddlSite.DataSource = dt
    '        ddlSite.DataTextField = "fld100"
    '        ddlSite.DataValueField = "tid"
    '        ddlSite.DataBind()
    '        ddlSite.Items.Insert(0, New ListItem("All", "0"))
    '        'dt.Clear()

    '        UpdatePanel2.Update()

    '    Catch ex As Exception

    '    End Try
    'End Sub

    <System.Web.Services.WebMethod()>
    Public Shared Function GetCompany(str As String, uid As String, urole As String) As String
        Dim jsonData As String = ""
        Dim conStr As String = ConfigurationManager.ConnectionStrings("ConStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
        '   Dim strquery As String = ""

        If (urole = "SU") Then
            oda.SelectCommand.CommandText = "select tid, fld1 [CompanyName] from mmm_mst_master  where eid =98 and documenttype ='Company Master' and fld1 like '%" & str & "%' and isauth=1 order by fld1"
        Else
            oda.SelectCommand.CommandText = " if EXISTS (select docmapping from mmm_mst_forms where eid =98 and  formname ='company Master' and isroledef=1) " &
   " select tid,fld1 [CompanyName] from mmm_mst_master comp inner join   dbo.split((select fld1 from mmm_ref_role_user where eid =98 and uid =" & uid & " and rolename ='" & urole & "'),',') s on s.items = comp.tid " &
" where eid =98 and documenttype ='Company Master' and fld1 like '%" & str & "%' order by fld1 else select tid,fld1[CompanyName]  from mmm_mst_master comp where eid =98 and documenttype ='Company Master' and isauth=1  and fld1 like '%" & str & "%' order by fld1"

        End If
        'oda.SelectCommand.CommandText = "select tid, fld1 [CompanyName] from mmm_mst_master  where eid =98 and documenttype ='Company Master' and fld1 like '%" & str & "%' order by fld1"
        Try
            Dim ds As New DataSet()
            oda.Fill(ds)
            jsonData = JsonConvert.SerializeObject(ds.Tables(0), Newtonsoft.Json.Formatting.None)
            Return jsonData
        Catch ex As Exception
            Throw
        End Try
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function GetSite(CompID As String, uid As String, urole As String) As String
        Dim jsonData As String = ""
        Dim conStr As String = ConfigurationManager.ConnectionStrings("ConStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
        If (urole = "ADMIN" Or urole = "SU") Then
            oda.SelectCommand.CommandText = "select fld100 ,tid from mmm_mst_master  where eid =98 and documenttype ='Site master' and  fld1 =" & CompID & " order by fld100"
        Else
            oda.SelectCommand.CommandText = " if EXISTS (select docmapping from mmm_mst_forms where eid =98 and  formname ='Site Master' and isroledef=1) " &
            " begin declare @docmapping as varchar(200) select @docmapping = docmapping from mmm_mst_forms where eid =98 and  formname ='Site Master' and isroledef=1 " &
" declare @Qrystr as varchar(2000) set @Qrystr = 'select fld100,tid from mmm_mst_master where eid =98 and documenttype =''Site Master''  " &
" and tid in (select items from dbo.split((select '+ @docmapping +' from mmm_ref_role_user where eid =98 and uid =" & uid & " and rolename =''" & urole & "''),'','')  ) and  fld1 =''" & CompID & "'' order by fld100 ' exec (@Qrystr) End else  " &
" select fld100 ,tid from mmm_mst_master  where eid =98 and documenttype ='Site master'  and  fld1 =" & CompID & " order by fld100"

        End If
        'oda.SelectCommand.CommandText = "select fld100 ,tid from mmm_mst_master  where eid =98 and documenttype ='Site master' and  fld1 =" & CompID & ""

        Try
            Dim ds As New DataSet()
            oda.Fill(ds)
            jsonData = JsonConvert.SerializeObject(ds.Tables(0), Newtonsoft.Json.Formatting.None)
            Return jsonData
        Catch ex As Exception
            Throw
        End Try
    End Function

End Class

Public Class eReport

    Public Property data As String
    Public Property columns As List(Of grdcolumns)

End Class

Public Class grdcolumns

    Public Property field As String
    Public Property title As String
    Public Property groupFooterTemplate As String = ""
    Public Property groupHeaderTemplate As String = ""
    Public Property aggregates As String = ""
    Public Property type As String = "string"

End Class

