Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports iTextSharp.text.pdf
Imports System.Security.Policy
Imports System.Net.Security
Imports System.Net.HttpWebRequest
Imports System.Net.HttpWebResponse
Imports System.Security.Cryptography.X509Certificates
Imports System.Web.Services
Imports Newtonsoft
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Converters
Imports System.Web.Script.Serialization
Partial Class THome
    Inherits System.Web.UI.Page
    Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString

    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load

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
    Public Shared Function GetSuspendedStatus() As DGrid
        Dim grid As New DGrid()
        Dim strError = ""
        Dim objDC As New DataClass()
        Dim adminRole As String = objDC.ExecuteQryScaller("select ADMINROLE from mmm_hdmail_schdule where eid=" & HttpContext.Current.Session("EID"))
        Try
            If ("," & HttpContext.Current.Session("USERROLE").ToString().ToUpper() & ",").ToString().Contains("," & adminRole.ToString().ToUpper() & ",") Then
                Dim objDT As New DataTable()
                objDT = objDC.ExecuteQryDT("select   'SUSPENDED Ticket ('+ cast(count(*) as nvarchar)+')' as [Suspended Tickets] from mmm_mst_doc where documenttype='Ticket' and eid=" & HttpContext.Current.Session("EID") & " and TicketStatus='SUSPENDED'")
                If objDT.Rows.Count > 0 Then
                    grid = DynamicGrid.GridData(objDT, strError)
                Else
                    grid.Success = False
                    grid.Message = "No data found."
                End If
            Else
                grid.Success = False
                grid.Message = "INVISIBLE"
            End If
        Catch ex As Exception
            grid.Success = False
            grid.Message = "Dear User please enter short date range."
        End Try
        Return grid
    End Function

    <WebMethod()>
    Public Shared Function GetDStatus() As DGrid
        Dim grid As New DGrid()
        Dim strError = ""

        Try
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As SqlConnection = New SqlConnection(conStr)
            Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
            Dim dt As New DataTable()
            Dim qry As String = ""
            Dim ds As New DataSet()
            Dim objDC As New DataClass()
            oda.SelectCommand.CommandType = CommandType.Text
            Dim objRole As New DataTable()
            objRole = objDC.ExecuteQryDT("select USERROLE,AGENTROLE,ADMINROLE from mmm_hdmail_schdule where eid=" & HttpContext.Current.Session("EID"))
            If ("," & objRole.Rows(0)("USERROLE").ToString().Trim().ToUpper & ",").ToString().Contains("," & HttpContext.Current.Session("USERROLE").ToString().Trim().ToUpper & ",") Then
                qry &= "Create table #TicketStatus(ID int primary key identity(1,1),TicketStatus nvarchar(max),displayorder int) "
                qry &= "insert into #TicketStatus values('OPEN',1),('PENDING',2),('SOLVED',3),('CLOSED',4) "
                qry &= ";with X (Organizations,ticketstatus,Counts)as "
                qry &= "(select  dms.udf_split('MASTER-Organizations-fld1',doc.fld16) as Organizations,ticketstatus,count(doc.tid) as Views from mmm_mst_doc as doc  where documenttype='Ticket'and eid=" & HttpContext.Current.Session("EID") & " and doc.ouid=" & HttpContext.Current.Session("UID") & " and doc.fld16<>0  group by ticketstatus,doc.fld16 )"
                qry &= ",Y(Organizations,TicketStatus,displayorder)as (select  dms.udf_split('MASTER-Organizations-fld1',fld16) as Organizations,#TicketStatus.TicketStatus,displayorder from mmm_mst_doc cross join #TicketStatus where documenttype='Ticket' and eid=" & HttpContext.Current.Session("EID") & " and fld16<>0  and ouid=" & HttpContext.Current.Session("UID") & "  group by fld16,#TicketStatus.TicketStatus,displayorder )"
                qry &= "select Y.Organizations as Organizations,Y.TicketStatus+ ' Tickets ('+ cast(isnull(counts,0) as nvarchar) +')' as Views from X right join Y on X.Organizations=Y.Organizations and X.ticketstatus=Y.TicketStatus order by  Y.Organizations, displayorder drop table #TicketStatus"
                dt = objDC.ExecuteQryDT(qry.ToString())
            ElseIf ("," & objRole.Rows(0)("AGENTROLE").ToString().Trim().ToUpper & ",").ToString().Contains("," & HttpContext.Current.Session("USERROLE").ToString().Trim().ToUpper & ",") Then
                qry &= "Create table #TicketStatus(ID int primary key identity(1,1),TicketStatus nvarchar(max),displayorder int) "
                qry &= "insert into #TicketStatus values('OPEN',1),('PENDING',2),('SOLVED',3),('CLOSED',4) "
                qry &= ";with X (Organizations,ticketstatus,Counts)as "
                qry &= " (select  Organizations,ticketstatus,Sum(Views) As views from "
                qry &= "(select  dms.udf_split('MASTER-Organizations-fld1',doc.fld16) as Organizations,ticketstatus,count(doc.tid) as Views from mmm_mst_doc as doc inner join mmm_doc_dtl as dtl on doc.tid=dtl.docid where documenttype='Ticket'and eid=" & HttpContext.Current.Session("EID") & " and dtl.userid=" & HttpContext.Current.Session("UID") & " and doc.fld16<>0 and aprstatus is null group by ticketstatus,doc.fld16 "
                qry &= " Union  select Organizations,ticketstatus,Views from (select [Organization] as [Organizations],'Closed' [ticketstatus],Count(*) [Views] from ( select * from ( select doc.TicketStatus,dms.udf_split('MASTER-Organizations-fld1',doc.fld16)[Organization],doc.tid[Docid1], mmm_doc_dtl.*, rank() over (partition by docid order  by mmm_doc_dtl.tid desc)[c] from mmm_mst_doc doc inner join mmm_doc_dtl on doc.tid=docid  and doc.TicketStatus='CLOSED' and doc.Documenttype='Ticket' and doc.eid=" & HttpContext.Current.Session("EID") & ") tt where tt.c=2) tbl where Userid=" & HttpContext.Current.Session("UID") & " group by [Organization]) A "
                qry &= "UNION  select distinct dms.udf_split('MASTER-Organizations-fld1',doc.fld16) as Organizations,ticketstatus,count(doc.tid) from mmm_mst_doc as doc inner join mmm_doc_view as mdv on doc.tid=mdv.docid where documenttype='Ticket' and eid=" & HttpContext.Current.Session("EID") & " and mdv.userid=" & HttpContext.Current.Session("UID") & " and doc.fld16<>0 group by ticketstatus,doc.fld16) A group by Organizations,ticketstatus) "
                qry &= " ,Y(Organizations,TicketStatus,displayorder)as (select  dms.udf_split('MASTER-Organizations-fld1',fld16) as Organizations,#TicketStatus.TicketStatus,displayorder from mmm_mst_doc cross join #TicketStatus where documenttype='Ticket' and eid=" & HttpContext.Current.Session("EID") & " and fld16<>0  group by fld16,#TicketStatus.TicketStatus,displayorder ) "
                qry &= " select Y.Organizations as Organizations,Y.TicketStatus+ ' Tickets ('+ cast(isnull(counts,0) as nvarchar) +')' as Views from X right join Y on X.Organizations=Y.Organizations and X.ticketstatus=Y.TicketStatus order by  Y.Organizations, displayorder drop table #TicketStatus "
                dt = objDC.ExecuteQryDT(qry.ToString())
            ElseIf ("," & objRole.Rows(0)("ADMINROLE").ToString().Trim().ToUpper & ",").ToString().Contains("," & HttpContext.Current.Session("USERROLE").ToString().Trim().ToUpper & ",") Then
                dt = objDC.ExecuteQryDT(" Create table #TicketStatus(ID int primary key identity(1,1),TicketStatus nvarchar(max),displayorder int)insert into #TicketStatus values('OPEN',1),('PENDING',2),('SOLVED',3),('CLOSED',4) ;with X (Organizations,ticketstatus,Counts)as(select  Organizations,     ticketstatus,Sum(Views) As views from  (select  dms.udf_split('MASTER-Organizations-fld1',doc.fld16) as Organizations,ticketstatus,count(doc.tid) as Views from mmm_mst_doc as doc inner join mmm_doc_dtl as dtl on doc.tid=dtl.docid where documenttype='Ticket'and eid=" & HttpContext.Current.Session("EID") & " and doc.fld16<>0 and aprstatus is null group by ticketstatus,doc.fld16 UNION ALL select distinct dms.udf_split('MASTER-Organizations-fld1',doc.fld16) as Organizations,ticketstatus,count(distinct doc.tid) from mmm_mst_doc as doc inner join mmm_doc_view as mdv on doc.tid=mdv.docid where documenttype='Ticket' and eid=" & HttpContext.Current.Session("EID") & " and doc.fld16<>0 group by ticketstatus,doc.fld16,doc.tid) A  group by Organizations,ticketstatus),Y(Organizations,TicketStatus,displayorder)as (select  dms.udf_split('MASTER-Organizations-fld1',fld16) as Organizations,#TicketStatus.TicketStatus,displayorder from mmm_mst_doc cross join #TicketStatus where documenttype='Ticket' and eid=" & HttpContext.Current.Session("EID") & " and fld16<>0  group by fld16,#TicketStatus.TicketStatus,displayorder )select Y.Organizations as Organizations,Y.TicketStatus+ ' Tickets ('+ cast(isnull(counts,0) as nvarchar) +')' as Views from X right join Y on X.Organizations=Y.Organizations and X.ticketstatus=Y.TicketStatus order by  Y.Organizations, displayorder drop table #TicketStatus")
            Else
                qry &= "Create table #TicketStatus(ID int primary key identity(1,1),TicketStatus nvarchar(max),displayorder int) "
                qry &= "insert into #TicketStatus values('OPEN',1),('PENDING',2),('SOLVED',3),('CLOSED',4) "
                qry &= ";with X (Organizations,ticketstatus,Counts)as "
                qry &= " (select  Organizations,ticketstatus,Sum(Views) As views from "
                qry &= "(select  dms.udf_split('MASTER-Organizations-fld1',doc.fld16) as Organizations,ticketstatus,count(doc.tid) as Views from mmm_mst_doc as doc inner join mmm_doc_dtl as dtl on doc.tid=dtl.docid where documenttype='Ticket'and eid=" & HttpContext.Current.Session("EID") & " and dtl.userid=0 and doc.fld16<>0 and aprstatus is null group by ticketstatus,doc.fld16 "
                qry &= " Union  select Organizations,ticketstatus,Views from (select [Organization] as [Organizations],'Closed' [ticketstatus],Count(*) [Views] from ( select * from ( select doc.TicketStatus,dms.udf_split('MASTER-Organizations-fld1',doc.fld16)[Organization],doc.tid[Docid1], mmm_doc_dtl.*, rank() over (partition by docid order  by mmm_doc_dtl.tid desc)[c] from mmm_mst_doc doc inner join mmm_doc_dtl on doc.tid=docid  and doc.TicketStatus='CLOSED' and doc.Documenttype='Ticket' and doc.eid=" & HttpContext.Current.Session("EID") & ") tt where tt.c=2) tbl where Userid=0 group by [Organization]) A "
                qry &= "UNION  select distinct dms.udf_split('MASTER-Organizations-fld1',doc.fld16) as Organizations,ticketstatus,count(doc.tid) from mmm_mst_doc as doc inner join mmm_doc_view as mdv on doc.tid=mdv.docid where documenttype='Ticket' and eid=" & HttpContext.Current.Session("EID") & " and mdv.userid=0 and doc.fld16<>0 group by ticketstatus,doc.fld16) A group by Organizations,ticketstatus) "
                qry &= " ,Y(Organizations,TicketStatus,displayorder)as (select  dms.udf_split('MASTER-Organizations-fld1',fld16) as Organizations,#TicketStatus.TicketStatus,displayorder from mmm_mst_doc cross join #TicketStatus where documenttype='Ticket' and eid=" & HttpContext.Current.Session("EID") & " and fld16<>0  group by fld16,#TicketStatus.TicketStatus,displayorder ) "
                qry &= " select Y.Organizations as Organizations,Y.TicketStatus+ ' Tickets ('+ cast(isnull(counts,0) as nvarchar) +')' as Views from X right join Y on X.Organizations=Y.Organizations and X.ticketstatus=Y.TicketStatus order by  Y.Organizations, displayorder drop table #TicketStatus "
                dt = objDC.ExecuteQryDT(qry.ToString())
            End If
            Try
                Try
                    If dt.Rows.Count = 0 Then
                        Dim obj As New DGrid()
                        obj.Success = False
                        obj.Message = "No data found."
                        obj.Column = DynamicGrid.CreateColCollection(dt)
                        grid = obj
                    Else
                        grid = DynamicGrid.GridData(dt, strError)
                    End If
                Catch exption As Exception
                    grid.Success = False
                    grid.Message = "Dear User please enter short date range."
                End Try
            Catch ex As Exception
                grid.Success = False
                grid.Message = "No data found."
            Finally
                con.Close()
                oda.Dispose()
                con.Dispose()
            End Try
        Catch ex As Exception

        End Try
        Return grid
    End Function


    <WebMethod()>
    Public Shared Function GetDetail(reportname As String, organizationName As String) As DGrid
        Dim grid As New DGrid()
        Dim strError = ""

        Try
            Dim objDC As New DataClass()
            Dim objDT As New DataTable()
            Dim UID As Integer = 0
            Dim EID As Integer = 0
            EID = HttpContext.Current.Session("EID")
            UID = HttpContext.Current.Session("UID")
            If reportname <> "undefined" Then
                reportname = reportname.Substring(0, reportname.IndexOf(" "))
                Dim qry As String = ""
                Dim roleid As String = ""
                Dim OrganizationID As Integer = objDC.ExecuteQryScaller("select tid from mmm_mst_master where documenttype='Organizations' and eid=" & EID & " and fld1='" & organizationName.Trim() & "'")
                Dim objRole As New DataTable()
                objRole = objDC.ExecuteQryDT("select USERROLE,AGENTROLE,ADMINROLE from mmm_hdmail_schdule where eid=" & HttpContext.Current.Session("EID"))
                If ("," & Convert.ToString(objRole.Rows(0)("USERROLE")).ToUpper() & ",").ToString().Contains(("," & HttpContext.Current.Session("USERROLE") & ",").ToString().ToUpper()) Then
                    'oda.SelectCommand.CommandText = "Select qryfield from mmm_mst_report where eid='" & HttpContext.Current.Session("EID") & "' and Actualfilter='Home' and reportdescription='" & reportname & "' and ticketuser='" & HttpContext.Current.Session("USERROLE") & "' "
                    qry &= "select d.tid[TID],fld2[Requestor],fld3[Subject],(select username from mmm_mst_user with(nolock) where uid=d.fld7)[Assignee], "
                    qry &= " dms.udf_split('MASTER-Department-fld1',d.fld8)[Category],fld9[Ticket Status],convert(varchar(17),adate,113)[Creation Date] from mmm_mst_doc d  "
                    qry &= " where eid=" & EID & " and documenttype='Ticket' and fld9='" & reportname & "' and d.ouid='" & UID & "' and d.fld16='" & OrganizationID & "' order by tid desc"
                    objDT = objDC.ExecuteQryDT(qry)
                ElseIf ("," & HttpContext.Current.Session("USERROLE") & ",").ToString().ToUpper().Contains("," & Convert.ToString(objRole.Rows(0)("AGENTROLE")).ToUpper() & ",") Then
                    'objDT = objDC.ExecuteQryDT("Select qryfield from mmm_mst_report where eid='" & HttpContext.Current.Session("EID") & "' and Actualfilter='Home' and reportdescription='" & reportname & "' and ticketuser='" & HttpContext.Current.Session("UID") & "'")

                    If reportname.ToString().ToUpper.Trim = "CLOSED" Then
                        qry &= "select d.tid[TID],d.fld2[Requestor Email],d.fld3[Subject],(select username from mmm_mst_user with(nolock) where uid=d.fld7)[Assignee],"
                        qry &= "dms.udf_split('MASTER-Department-fld1',d.fld8)[Category],d.fld9[Ticket Status],convert(varchar(12),d.adate,113)[Creation Date],"
                        qry &= "convert(varchar(17),d.lastupdate,113)[Last Updated On],(select username from mmm_mst_user with(nolock) where uid=d.fld14)[Last Updated By]" '(select username from mmm_mst_user with(nolock) where uid=(select top 1 uid from mmm_mst_history with(nolock) where eid=" & EID & " and documenttype='Ticket' and docid=d.tid))[Last Updated By]"
                        qry &= "from MMM_MST_DOC d with(nolock) where tid in"
                        qry &= "(select Docid1 from ( select * from( "
                        qry &= "select doc.tid[Docid1], mmm_doc_dtl.*, rank() over (partition by docid order  by mmm_doc_dtl.tid desc)[c] from mmm_mst_doc doc inner join mmm_doc_dtl on doc.tid=docid  and doc.TicketStatus='" & reportname & "' and doc.Documenttype='Ticket' and doc.eid=" & EID & " and doc.fld16='" & OrganizationID & "') tt where tt.c=2   ) tbl"
                        qry &= "  where Userid=" & UID & ") order by tid desc"
                    Else
                        qry &= "select d.tid[TID],d.fld2[Requestor Email],d.fld3[Subject],(select username from mmm_mst_user with(nolock) where uid=d.fld7)[Assignee],dms.udf_split('MASTER-Department-fld1',d.fld8)[Category],d.fld9[Ticket Status],"
                        qry &= "convert(varchar(12),d.adate,113)[Creation Date],convert(varchar(17),d.lastupdate,113)[Last Updated On],(select username from mmm_mst_user with(nolock) where uid=d.fld14)[Last Updated By]" '(select username from mmm_mst_user with(nolock) where uid=(select top 1 uid from mmm_mst_history with(nolock) where eid=" & EID & " and documenttype='Ticket' and docid=d.tid))[Last Updated By]"
                        qry &= "from MMM_MST_DOC d with(nolock) inner join mmm_doc_dtl dd with(nolock) on d.lasttid=dd.tid"
                        qry &= " where d.eid=" & EID & " and d.documenttype='Ticket' and d.fld9='" & reportname & "' and dd.userid='" & UID & "' and d.fld16='" & OrganizationID & "'"
                        qry &= "union "
                        qry &= "select d.tid[TID],d.fld2[Requestor Email],d.fld3[Subject],(select username from mmm_mst_user with(nolock) where uid=d.fld7)[Assignee],dms.udf_split('MASTER-Department-fld1',d.fld8)[Category],d.fld9[Ticket Status],"
                        qry &= "convert(varchar(12),d.adate,113)[Creation Date],convert(varchar(17),d.lastupdate,113)[Last Updated On],(select username from mmm_mst_user with(nolock) where uid=d.fld14)[Last Updated By]" '(select username from mmm_mst_user with(nolock) where uid=(select top 1 uid from mmm_mst_history with(nolock) where eid=" & EID & " and documenttype='Ticket' and docid=d.tid))[Last Updated By]"
                        qry &= "from MMM_MST_DOC d with(nolock) inner join mmm_doc_view dd with(nolock) on d.tid=dd.docid"
                        qry &= " where d.eid=" & EID & " and d.documenttype='Ticket' and d.fld9='" & reportname & "' and dd.userid='" & UID & "' and d.fld16='" & OrganizationID & "' order by tid desc"
                    End If
                    objDT = objDC.ExecuteQryDT(qry)
                ElseIf ("," & HttpContext.Current.Session("USERROLE") & ",").ToString().ToUpper().Contains("," & Convert.ToString(objRole.Rows(0)("ADMINROLE")).ToUpper() & ",") Then
                    qry &= "select d.tid[TID],d.fld2[Requestor Email],d.fld3[Subject],(select username from mmm_mst_user with(nolock) where uid=d.fld7)[Assignee],dms.udf_split('MASTER-Department-fld1',d.fld8)[Category],d.fld9[Ticket Status],"
                    qry &= "convert(varchar(12),d.adate,113)[Creation Date],convert(varchar(17),d.lastupdate,113)[Last Updated On],(select username from mmm_mst_user with(nolock) where uid=d.fld14)[Last Updated By]" '(select username from mmm_mst_user with(nolock) where uid=(select top 1 uid from mmm_mst_history with(nolock) where eid=" & EID & " and documenttype='Ticket' and docid=d.tid))[Last Updated By]"
                    qry &= "from MMM_MST_DOC d with(nolock) inner join mmm_doc_dtl dd with(nolock) on d.lasttid=dd.tid"
                    qry &= " where d.eid=" & EID & " and d.documenttype='Ticket' and d.fld9='" & reportname & "' and d.fld16='" & OrganizationID & "'"
                    qry &= "union "
                    qry &= "select d.tid[TID],d.fld2[Requestor Email],d.fld3[Subject],(select username from mmm_mst_user with(nolock) where uid=d.fld7)[Assignee],dms.udf_split('MASTER-Department-fld1',d.fld8)[Category],d.fld9[Ticket Status],"
                    qry &= "convert(varchar(12),d.adate,113)[Creation Date],convert(varchar(17),d.lastupdate,113)[Last Updated On],(select username from mmm_mst_user with(nolock) where uid=d.fld14)[Last Updated By]" '(select username from mmm_mst_user with(nolock) where uid=(select top 1 uid from mmm_mst_history with(nolock) where eid=" & EID & " and documenttype='Ticket' and docid=d.tid))[Last Updated By]"
                    qry &= "from MMM_MST_DOC d with(nolock) where  d.tid in "
                    qry &= "(select docid from( select  mmm_doc_view.*,  rank() over (partition by docid order  by mmm_doc_view.tid desc)[c] from mmm_mst_doc doc inner join mmm_doc_view on doc.tid=docid  and doc.TicketStatus='" & reportname & "' and doc.Documenttype='Ticket' and doc.eid=" & EID & ") tt where tt.c=1 ) and "
                    qry &= " d.eid=" & EID & " and d.documenttype='Ticket' and d.fld9='" & reportname & "' and d.fld16='" & OrganizationID & "' order by tid desc"
                    objDT = objDC.ExecuteQryDT(qry)
                End If
                Try
                    If objDT.Rows.Count = 0 Then
                        Dim obj As New DGrid()
                        obj.Success = False
                        obj.Message = "No data found."
                        obj.Column = DynamicGrid.CreateColCollection(objDT)
                        obj.Data = DynamicGrid.JsonTableSerializer(objDT)
                        grid = obj
                    Else
                        grid = DynamicGrid.GridData(objDT, strError)
                    End If
                Catch exption As Exception
                    grid.Success = False
                    grid.Message = "Dear User please enter short date range."
                End Try
            Else
                'For Suspended Ticket Status
                Dim qry As String = ""
                qry &= "select d.tid[TID],fld2[Requestor],fld3[Subject],(select username from mmm_mst_user with(nolock) where uid=d.fld7)[Assignee], "
                qry &= " dms.udf_split('MASTER-Department-fld1',d.fld8)[Category],fld9[Ticket Status],convert(varchar(17),adate,113)[Creation Date] from mmm_mst_doc d  "
                qry &= " where eid=" & EID & " and documenttype='Ticket' and fld9='SUSPENDED' order by tid desc"
                objDT = objDC.ExecuteQryDT(qry)
                Try
                    If objDT.Rows.Count = 0 Then
                        grid.Success = False
                        grid.Message = "No data found."
                    Else
                        grid = DynamicGrid.GridData(objDT, strError)
                    End If
                Catch exption As Exception
                    grid.Success = False
                    grid.Message = "Dear User please enter short date range."
                End Try
            End If
        Catch ex As Exception
            grid.Success = False
            grid.Message = "No data found."
        End Try
        Return grid
    End Function

    <WebMethod()>
    Public Shared Function GetJSON() As String
        Dim jsonData As String = ""
        Try
            Dim ds As New DataSet()
            Dim UID As Integer = 0
            Dim EID As Integer = 0
            Dim URole As String = ""
            Try
                EID = Convert.ToInt32(HttpContext.Current.Session("EID").ToString())
                UID = Convert.ToInt32(HttpContext.Current.Session("UID").ToString())
                URole = HttpContext.Current.Session("USERROLE").ToString()
                If (UID = 0 Or URole = "") Then
                    Return "NoSession"
                End If
            Catch ex As Exception
                Return "NoSession"
            End Try
            Dim qry As String = ""
            Dim objDC As New DataClass()
            Dim objRole As New DataTable()
            objRole = objDC.ExecuteQryDT("select USERROLE,AGENTROLE,ADMINROLE from mmm_hdmail_schdule where eid=" & HttpContext.Current.Session("EID"))
            If ("," & Convert.ToString(objRole.Rows(0)("USERROLE")).ToUpper() & ",").ToUpper.Contains("," & HttpContext.Current.Session("USERROLE").ToString().Trim().ToUpper & ",") Then
                ' qry &= "select 'New'[category],count(tid)[value] from mmm_mst_doc d  where eid='" & HttpContext.Current.Session("EID") & "' and documenttype='Ticket' and fld9='new' and d.ouid='" & HttpContext.Current.Session("UID") & "' union all "
                'qry &= "select 'Open'[category],count(tid)[value] from mmm_mst_doc d  where eid='" & HttpContext.Current.Session("EID") & "' and documenttype='Ticket' and fld9='Open' and d.ouid='" & HttpContext.Current.Session("UID") & "'  union all "
                'qry &= "select 'Pending'[category],count(tid)[value] from mmm_mst_doc d  where eid='" & HttpContext.Current.Session("EID") & "' and documenttype='Ticket' and fld9='Pending' and d.ouid='" & HttpContext.Current.Session("UID") & "'  union all "
                'qry &= "select 'Solved'[category],count(tid)[value] from mmm_mst_doc d  where eid='" & HttpContext.Current.Session("EID") & "' and documenttype='Ticket' and fld9='solved' and d.ouid='" & HttpContext.Current.Session("UID") & "'  union all "
                'qry &= "select 'closed'[category],count(tid)[value] from mmm_mst_doc d  where eid='" & HttpContext.Current.Session("EID") & "' and documenttype='Ticket' and fld9='closed' and d.ouid='" & HttpContext.Current.Session("UID") & "'"
                qry &= "Create table #TicketStatus(ID int primary key identity(1,1),TicketStatus nvarchar(max),displayorder int)insert into #TicketStatus values('OPEN',1),('PENDING',2),('SOLVED',3),('CLOSED',4);with X (Organizations,ticketstatus,Counts)as(select  dms.udf_split('MASTER-Organizations-fld1',doc.fld16) as Organizations,ticketstatus,count(doc.tid) as Views from mmm_mst_doc as doc where documenttype='Ticket'and eid=" & EID & " and doc.ouid=" & UID & " and doc.fld16<>0  group by ticketstatus,doc.fld16),Y(Organizations,TicketStatus,displayorder)as (select  dms.udf_split('MASTER-Organizations-fld1',fld16) as Organizations,#TicketStatus.TicketStatus,displayorder from mmm_mst_doc cross join #TicketStatus where documenttype='Ticket' and eid=" & EID & " and fld16<>0  group by fld16,#TicketStatus.TicketStatus,displayorder )select Y.TicketStatus as [category],sum(isnull(counts,0))  as [value] from X right join Y on X.ticketstatus=Y.TicketStatus and X.Organizations=Y.Organizations  group by Y.TicketStatus,Y.displayorder order by Y.displayorder drop table #TicketStatus"
            ElseIf ("," & Convert.ToString(objRole.Rows(0)("ADMINROLE")).ToUpper() & ",").ToUpper.Contains("," & HttpContext.Current.Session("USERROLE").ToString().Trim().ToUpper & ",") Then
                qry &= ";WITH CTE AS ( "
                qry &= " select 'Open'[categ],count(d.tid)[val] from mmm_mst_doc d with (nolock) where d.eid='" & HttpContext.Current.Session("EID") & "' and d.documenttype='Ticket' and d.fld9='Open' and d.fld16 in (select tid  from mmm_mst_master with(nolock) where eid='" & HttpContext.Current.Session("EID") & "' and documenttype='Organizations' and fld4='" & HttpContext.Current.Session("UID") & "') union all "
                ' qry &= " select 'Open'[categ],count(d.tid)[val] from mmm_doc_view d with (nolock) where d.eid='" & HttpContext.Current.Session("EID") & "' and d.documenttype='Ticket' and d.fld9='Open' and d.fld16 in (select tid  from mmm_mst_master with(nolock) where eid='" & HttpContext.Current.Session("EID") & "' and documenttype='Organizations' and fld4='" & HttpContext.Current.Session("UID") & "') union all "
                qry &= " select 'Pending'[categ],count(d.tid)[val] from mmm_mst_doc d with (nolock) where d.eid='" & HttpContext.Current.Session("EID") & "' and d.documenttype='Ticket' and d.fld9='Pending' and d.fld16 in (select tid  from mmm_mst_master with(nolock) where eid='" & HttpContext.Current.Session("EID") & "' and documenttype='Organizations' and fld4='" & HttpContext.Current.Session("UID") & "') union all "
                'qry &= " select 'Pending'[categ],count(d.tid)[val] from mmm_doc_view d with (nolock) where d.eid='" & HttpContext.Current.Session("EID") & "' and d.documenttype='Ticket' and d.fld9='Pending' and d.fld16 in (select tid  from mmm_mst_master with(nolock) where eid='" & HttpContext.Current.Session("EID") & "' and documenttype='Organizations' and fld4='" & HttpContext.Current.Session("UID") & "') union all "
                qry &= " select 'Solved'[categ],count(d.tid)[val] from mmm_mst_doc d with (nolock) where d.eid='" & HttpContext.Current.Session("EID") & "' and d.documenttype='Ticket' and d.fld9='Solved' and d.fld16 in (select tid  from mmm_mst_master with(nolock) where eid='" & HttpContext.Current.Session("EID") & "' and documenttype='Organizations' and fld4='" & HttpContext.Current.Session("UID") & "')  union all "
                'qry &= " select 'Solved'[categ],count(d.tid)[val] from mmm_doc_view d with (nolock) where d.eid='" & HttpContext.Current.Session("EID") & "' and d.documenttype='Ticket' and d.fld9='Solved'  and d.fld16 in (select tid  from mmm_mst_master with(nolock) where eid='" & HttpContext.Current.Session("EID") & "' and documenttype='Organizations' and fld4='" & HttpContext.Current.Session("UID") & "')  union all "
                qry &= " select 'Closed'[categ],count(d.tid)[val] from mmm_mst_doc d  with (nolock) where d.eid='" & HttpContext.Current.Session("EID") & "' and d.documenttype='Ticket' and d.fld9='Closed' and d.fld16 in (select tid  from mmm_mst_master with(nolock) where eid='" & HttpContext.Current.Session("EID") & "' and documenttype='Organizations' and fld4='" & HttpContext.Current.Session("UID") & "'))"
                'qry &= " select 'Closed'[categ],count(d.tid)[val] from mmm_doc_view d with (nolock)  where d.eid='" & HttpContext.Current.Session("EID") & "' and d.documenttype='Ticket' and d.fld9='Closed' and d.fld16 in (select tid  from mmm_mst_master with(nolock) where eid='" & HttpContext.Current.Session("EID") & "' and documenttype='Organizations' and fld4='" & HttpContext.Current.Session("UID") & "'))"
                qry &= " SELECT 'Open'[category],sum(val)[value] FROM CTE with(nolock) where categ='Open'"
                qry &= " union all"
                qry &= " SELECT 'Pending'[category],sum(val)[value] FROM CTE with(nolock) where categ='Pending'"
                qry &= " union all"
                qry &= " SELECT 'Solved'[category],sum(val)[value] FROM CTE with(nolock) where categ='Solved'"
                qry &= " union all"
                qry &= " SELECT 'Closed'[category],sum(val)[value] FROM CTE with(nolock) where categ='Closed'"

            ElseIf ("," & Convert.ToString(objRole.Rows(0)("AGENTROLE")).ToUpper() & ",").ToUpper.Contains("," & HttpContext.Current.Session("USERROLE").ToString().Trim().ToUpper & ",") Then

                qry &= ";WITH CTE AS ( "
                qry &= " select 'Open'[categ],count(d.tid)[val] from mmm_mst_doc d inner join mmm_doc_dtl dd with (nolock) on d.tid=dd.docid and dd.userid='" & HttpContext.Current.Session("UID") & "'  where d.eid='" & HttpContext.Current.Session("EID") & "' and d.documenttype='Ticket' and d.fld9='Open' and aprstatus is null union  "
                qry &= " select 'Open'[categ],count(d.tid)[val] from mmm_mst_doc d inner join mmm_doc_view dd with (nolock) on d.tid=dd.docid and dd.userid='" & HttpContext.Current.Session("UID") & "'  where d.eid='" & HttpContext.Current.Session("EID") & "' and d.documenttype='Ticket' and d.fld9='Open' union  "
                qry &= " select 'Pending'[categ],count(d.tid)[val] from mmm_mst_doc d inner join mmm_doc_dtl dd with (nolock) on d.tid=dd.docid and dd.userid='" & HttpContext.Current.Session("UID") & "'  where d.eid='" & HttpContext.Current.Session("EID") & "' and d.documenttype='Ticket' and d.fld9='Pending' and aprstatus is null union  "
                qry &= " select 'Pending'[categ],count(d.tid)[val] from mmm_mst_doc d inner join mmm_doc_view dd with (nolock) on d.tid=dd.docid and dd.userid='" & HttpContext.Current.Session("UID") & "'  where d.eid='" & HttpContext.Current.Session("EID") & "' and d.documenttype='Ticket' and d.fld9='Pending' union  "
                qry &= " select 'Solved'[categ],count(d.tid)[val] from mmm_mst_doc d inner join mmm_doc_dtl dd with (nolock) on d.tid=dd.docid and dd.userid='" & HttpContext.Current.Session("UID") & "'  where d.eid='" & HttpContext.Current.Session("EID") & "' and d.documenttype='Ticket' and d.fld9='Solved' and aprstatus is null union  "
                qry &= " select 'Solved'[categ],count(d.tid)[val] from mmm_mst_doc d inner join mmm_doc_view dd with (nolock) on d.tid=dd.docid and dd.userid='" & HttpContext.Current.Session("UID") & "'  where d.eid='" & HttpContext.Current.Session("EID") & "' and d.documenttype='Ticket' and d.fld9='Solved' union  "
                qry &= "select  'Closed'[categ],count(*)[Val] from( select  mmm_doc_dtl.*, rank() over (partition by docid order  by mmm_doc_dtl.tid desc)[c] from mmm_mst_doc doc inner join mmm_doc_dtl on doc.tid=docid  and doc.TicketStatus='CLOSED' and doc.Documenttype='Ticket' and doc.eid='" & HttpContext.Current.Session("EID") & "') tt where tt.c=2 and Userid='" & HttpContext.Current.Session("UID") & "'  )"
                'qry &= " select 'Closed'[categ],count(d.tid)[val] from mmm_mst_doc d inner join mmm_doc_dtl dd with (nolock) on d.tid=dd.docid and dd.userid='" & HttpContext.Current.Session("UID") & "'  where d.eid='" & HttpContext.Current.Session("EID") & "' and d.documenttype='Ticket' and d.fld9='Closed' and aprstatus is null union  "
                'qry &= " select 'Closed'[categ],count(d.tid)[val] from mmm_mst_doc d inner join mmm_doc_view dd with (nolock) on d.tid=dd.docid and dd.userid='" & HttpContext.Current.Session("UID") & "'  where d.eid='" & HttpContext.Current.Session("EID") & "' and d.documenttype='Ticket' and d.fld9='Closed')"

                qry &= " SELECT 'Open'[category],sum(val)[value] FROM CTE with(nolock) where categ='Open'"
                qry &= " union "
                qry &= " SELECT 'Pending'[category],sum(val)[value] FROM CTE with(nolock) where categ='Pending'"
                qry &= " union "
                qry &= " SELECT 'Solved'[category],sum(val)[value] FROM CTE with(nolock) where categ='Solved'"
                qry &= " union "
                qry &= " SELECT 'Closed'[category],sum(val)[value] FROM CTE with(nolock) where categ='Closed'"
            Else
                qry &= "Create table #TempData (category nvarchar(100),value int)insert into #TempData values('Closed',0),('Open',0),('Pending',0),('Solved',0) select * from #TempData drop table #TempData"
            End If

            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As New SqlConnection(conStr)
            Dim dt As New DataTable
            If qry <> String.Empty Then
                Dim oda As SqlDataAdapter = New SqlDataAdapter(qry, con)
                oda.Fill(dt)
            Else

            End If
            con.Close()

            Dim lstColumns As New List(Of String)
            Dim serializerSettings As New JsonSerializerSettings()
            Dim json_serializer As New JavaScriptSerializer()
            serializerSettings.Converters.Add(New DataTableConverter())
            jsonData = JsonConvert.SerializeObject(dt, Newtonsoft.Json.Formatting.None, serializerSettings)
            ' res.data = jsonData
            'res.columns = lstColumns



        Catch Ex As Exception
            Throw
        End Try
        Return jsonData

    End Function
    Public Class vdashboard
        Public Property series As List(Of series)
        Public Property categoryAxis As List(Of String)
        Public Property HasSession As String
    End Class
    Public Class series
        Public Property name As String
        Public Property stack As String
        Public Property data As List(Of Integer)
        Public Property color As String
    End Class
End Class

