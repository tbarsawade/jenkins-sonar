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
Partial Class ReallocationRightsNew
    Inherits System.Web.UI.Page
    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            'binddldoctype()
            'xrow.Visible = False
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

    <System.Web.Services.WebMethod()>
    Public Shared Function GetDocumentType(documentType As String) As kGridReallocation
        Dim jsonData As String = ""
        Dim ret As New kGridReallocation()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim strQuery1 As String = ""
        Try
            Dim ds As New DataSet
            strQuery1 = "Select distinct doctype from mmm_mst_Reallocation where eid=" & HttpContext.Current.Session("EID") & " and role='" & HttpContext.Current.Session("USERROLE") & "' "
            ds = DataLib.ExecuteDataSet(conStr, CommandType.Text, strQuery1)
            jsonData = JsonConvert.SerializeObject(ds.Tables(0))
            ret.Data = jsonData
        Catch ex As Exception
            Throw
        End Try
        Return ret
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function Remarksdata(documentType As String, status As String) As kGridReallocation
        Dim jsonData As String = ""
        Dim ret As New kGridReallocation()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim strQuery1 As String = ""
        Try
            Dim ds As New DataSet
            strQuery1 = "Select  Remarks from mmm_mst_reallocation where eid=" & HttpContext.Current.Session("EID") & " and doctype='" & documentType & "' and role='" & HttpContext.Current.Session("USERROLE") & "' and status ='" & status & "'  "
            ds = DataLib.ExecuteDataSet(conStr, CommandType.Text, strQuery1)
            jsonData = JsonConvert.SerializeObject(ds.Tables(0))
            ret.Data = jsonData
        Catch ex As Exception
            Throw
        End Try
        Return ret
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function GetStatus(documentType As String) As kGridReallocation
        Dim jsonData As String = ""
        Dim ret As New kGridReallocation()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim strQuery1 As String = ""
        Try
            Dim ds As New DataSet
            strQuery1 = "Select  * from mmm_mst_reallocation where eid=" & HttpContext.Current.Session("EID") & " and doctype='" & documentType & "' and role='" & HttpContext.Current.Session("USERROLE") & "' order by status"
            ds = DataLib.ExecuteDataSet(conStr, CommandType.Text, strQuery1)
            jsonData = JsonConvert.SerializeObject(ds.Tables(0))
            ret.Data = jsonData
        Catch ex As Exception
            Throw
        End Try
        Return ret
    End Function
    <System.Web.Services.WebMethod()>
    Public Shared Function getresult(documentType As String, status As String, currUser As String) As kGridReallocation
        Dim jsonData As String = ""
        Dim ret As New kGridReallocation()
        Try
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As New SqlConnection(conStr)
            Dim da As New SqlDataAdapter("Select cfield,remarks from mmm_mst_Reallocation where eid=" & HttpContext.Current.Session("EID") & " and doctype='" & documentType & "' and status='" & status & "'", con)
            Dim dss As New DataSet
            da.Fill(dss, "cr")
            If dss.Tables("cr").Rows.Count > 0 Then
                Dim objdc As New DataClass()
                da.SelectCommand.CommandText = "Select dropdown from mmm_mst_fields where eid=" & HttpContext.Current.Session("EID") & " and documenttype='" & documentType & "' and fieldmapping='" & dss.Tables("cr").Rows(0).Item("cfield").ToString() & "'"
                If con.State <> ConnectionState.Open Then
                    con.Open()
                End If
                Dim sse As String() = da.SelectCommand.ExecuteScalar().ToString.Split("-")

                Dim ff As String = String.Empty
                If sse.Length > 1 Then
                    da.SelectCommand.CommandText = "Select docmapping from mmm_mst_forms where eid=" & HttpContext.Current.Session("EID") & " and formname='" & sse(1).ToString & "'"
                    ff = da.SelectCommand.ExecuteScalar()
                    HttpContext.Current.Session("ff") = ff
                Else
                    Dim value As String = objdc.ExecuteQryScaller("Select DDLlookupValueSource from mmm_mst_fields where eid=" & HttpContext.Current.Session("EID") & " and documenttype='" & documentType & "' and fieldmapping='" & dss.Tables("cr").Rows(0).Item("cfield").ToString() & "'")
                    ff = objdc.ExecuteQryScaller("Select docmapping from mmm_mst_forms where eid=" & HttpContext.Current.Session("EID") & " and formname='" & value & "'")
                    HttpContext.Current.Session("ff") = ff
                End If
                Dim fld As String = dss.Tables("cr").Rows(0).Item("cfield").ToString()
                Dim remarks As String = dss.Tables("cr").Rows(0).Item("remarks").ToString()
                HttpContext.Current.Session("fld") = fld.ToString()
                HttpContext.Current.Session("remarks") = remarks.ToString()
                'Get Dynamic Data
                da.SelectCommand.CommandText = "Select fieldid,displayname,fieldmapping,dropdown,datatype,dropdowntype from mmm_mst_fields where eid=" & HttpContext.Current.Session("EID") & " and documenttype='" & documentType & "' and showonReallocation=1  order by displayorder"
                Dim dsDynamic As New DataSet
                da.Fill(dsDynamic)
                Dim dynamicCol As String = ""
                Dim dynamiccol1 As String = ""
                If dsDynamic.Tables(0).Rows.Count > 0 Then
                    For i As Integer = 0 To dsDynamic.Tables(0).Rows.Count - 1
                        If dsDynamic.Tables(0).Rows(i)("dropdowntype") = "MASTER VALUED" Then
                            dynamicCol += ",(select dms.udf_split('" + dsDynamic.Tables(0).Rows(i)("dropdown") + "'," + dsDynamic.Tables(0).Rows(i)("fieldmapping") + ")[" + dsDynamic.Tables(0).Rows(i)("displayname") + "]) " + dsDynamic.Tables(0).Rows(i)("fieldmapping") + ""
                        Else
                            dynamicCol += "," + dsDynamic.Tables(0).Rows(i)("fieldmapping") + ""
                        End If
                        dynamiccol1 += "," + dsDynamic.Tables(0).Rows(i)("fieldmapping") + ""
                    Next
                End If
                '----------------------------------
                da.SelectCommand.CommandTimeout = 700
                
                da.SelectCommand.CommandText = "SELECT DocID,CurrentUser,[LastTID],[ptat],[ordering],[fdate],NextUser" & dynamiccol1 & " FROM (select d.tid[DocID],(select distinct username from mmm_mst_user where eid=" & HttpContext.Current.Session("EID") & " and uid= dd.userid )[CurrentUser],dd.userid,d.lasttid[Lasttid],dd.ptat[ptat],dd.ordering[ordering],dd.fdate,'""' NextUser " & dynamicCol & " from mmm_mst_doc d inner join mmm_doc_dtl dd on d.lasttid=dd.tid where d.eid=" & HttpContext.Current.Session("EID") & " and d.documenttype='" & documentType & "' and d.curstatus='" & status & "' and dd.userid is not null and dd.userid not in (217,363)  and " & fld & " in (select * from  InputString((select " & ff.ToString() & " from mmm_ref_role_user where eid=" & HttpContext.Current.Session("EID") & " and uid=" & HttpContext.Current.Session("UID") & " and rolename='" & HttpContext.Current.Session("USERROLE") & "'))))A  where [currentuser] is not null and [Currentuser]<>'' "
                

                ' Dim da As New SqlDataAdapter("select d.tid[DocID],(select username from mmm_mst_user where eid=" & Session("EID") & " and uid= (select userid from mmm_doc_dtl where tid=d.lasttid and docid=d.tid))[Current User],* from mmm_mst_doc d where d.eid=" & Session("EID") & " and d.documenttype='VRF FIXED_POOL' and d.curstatus='approved' ", con)
                Dim ds As New DataSet
                da.Fill(ds, "data")
                Dim dtfilter As DataTable
                If currUser <> "" Then
                    dtfilter = ds.Tables(0).AsEnumerable().Where(Function(r) r.Field(Of String)("CurrentUser") = currUser).CopyToDataTable()

                    'ds.Tables(0).[Select]("[CurrentUser] ='" & currUser & "'").CopyToDataTable()
                Else
                    dtfilter = ds.Tables(0)
                End If

                HttpContext.Current.Session("getresultdata") = dtfilter
                jsonData = JsonConvert.SerializeObject(dtfilter)
                ret.Data = jsonData
                ret.Column = CreateStaticColCollectionForReallocation(dsDynamic.Tables(0))
            End If

        Catch ex As Exception
        End Try
        Return ret
    End Function
    <System.Web.Services.WebMethod()>
    Public Shared Function GetCurrentUsers(documentType As String, status As String) As kGridReallocation
        Dim jsonData As String = ""
        Dim ret As New kGridReallocation()
        Try
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As New SqlConnection(conStr)
            Dim da As New SqlDataAdapter("Select cfield,remarks from mmm_mst_Reallocation where eid=" & HttpContext.Current.Session("EID") & " and doctype='" & documentType & "' and status='" & status & "'", con)
            Dim dss As New DataSet
            da.Fill(dss, "cr")
            If dss.Tables("cr").Rows.Count > 0 Then
                'TXTRemarks.Value = dss.Tables("cr").Rows.Count

                Dim objdc As New DataClass()
                da.SelectCommand.CommandText = "Select dropdown from mmm_mst_fields where eid=" & HttpContext.Current.Session("EID") & " and documenttype='" & documentType & "' and fieldmapping='" & dss.Tables("cr").Rows(0).Item("cfield").ToString() & "'"
                If con.State <> ConnectionState.Open Then
                    con.Open()
                End If
                Dim sse As String() = da.SelectCommand.ExecuteScalar().ToString.Split("-")

                Dim ff As String = String.Empty
                If sse.Length > 1 Then
                    da.SelectCommand.CommandText = "Select docmapping from mmm_mst_forms where eid=" & HttpContext.Current.Session("EID") & " and formname='" & sse(1).ToString & "'"
                    ff = da.SelectCommand.ExecuteScalar()
                Else
                    Dim value As String = objdc.ExecuteQryScaller("Select DDLlookupValueSource from mmm_mst_fields where eid=" & HttpContext.Current.Session("EID") & " and documenttype='" & documentType & "' and fieldmapping='" & dss.Tables("cr").Rows(0).Item("cfield").ToString() & "'")
                    ff = objdc.ExecuteQryScaller("Select docmapping from mmm_mst_forms where eid=" & HttpContext.Current.Session("EID") & " and formname='" & value & "'")
                End If
                Dim fld As String = dss.Tables("cr").Rows(0).Item("cfield").ToString()
                Dim remarks As String = dss.Tables("cr").Rows(0).Item("remarks").ToString()

                Dim da1 As New SqlDataAdapter("select distinct (select distinct username from mmm_mst_user where eid=" & HttpContext.Current.Session("EID") & " and uid= (select userid from mmm_doc_dtl where tid=d.lasttid and docid=d.tid and userid is not null and userid not in (217,363) ))[CurrentUser],dd.userid[UID]  from mmm_mst_doc d inner join mmm_doc_dtl dd on d.lasttid=dd.tid where d.eid=" & HttpContext.Current.Session("EID") & " and d.documenttype='" & documentType & "' and d.curstatus='" & status & "' and " & fld & " in (Select * from inputstring((select " & ff & " from mmm_ref_role_user where eid=" & HttpContext.Current.Session("EID") & " and uid=" & HttpContext.Current.Session("UID") & " and rolename='" & HttpContext.Current.Session("USERROLE") & "'))) ", con)
                da.SelectCommand.CommandTimeout = 600
                Dim ds As New DataSet
                da1.Fill(ds, "dt")
                jsonData = JsonConvert.SerializeObject(ds.Tables(0))
                ret.Data = jsonData
            End If
        Catch ex As Exception

        End Try
        Return ret
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function GetTargetUsers(documentType As String, status As String) As kGridReallocation
        Dim jsonData As String = ""
        Dim ret As New kGridReallocation()
        Try
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As New SqlConnection(conStr)
            Dim da As New SqlDataAdapter("Select cfield,remarks from mmm_mst_Reallocation where eid=" & HttpContext.Current.Session("EID") & " and doctype='" & documentType & "' and status='" & status & "'", con)
            Dim dss As New DataSet
            da.Fill(dss, "cr")
            If dss.Tables("cr").Rows.Count > 0 Then
                Dim objdc As New DataClass()
                da.SelectCommand.CommandText = "Select dropdown from mmm_mst_fields where eid=" & HttpContext.Current.Session("EID") & " and documenttype='" & documentType & "' and fieldmapping='" & dss.Tables("cr").Rows(0).Item("cfield").ToString() & "'"
                If con.State <> ConnectionState.Open Then
                    con.Open()
                End If
                Dim sse As String() = da.SelectCommand.ExecuteScalar().ToString.Split("-")

                Dim ff As String = String.Empty
                If sse.Length > 1 Then
                    da.SelectCommand.CommandText = "Select docmapping from mmm_mst_forms where eid=" & HttpContext.Current.Session("EID") & " and formname='" & sse(1).ToString & "'"
                    ff = da.SelectCommand.ExecuteScalar()
                Else
                    Dim value As String = objdc.ExecuteQryScaller("Select DDLlookupValueSource from mmm_mst_fields where eid=" & HttpContext.Current.Session("EID") & " and documenttype='" & documentType & "' and fieldmapping='" & dss.Tables("cr").Rows(0).Item("cfield").ToString() & "'")
                    ff = objdc.ExecuteQryScaller("Select docmapping from mmm_mst_forms where eid=" & HttpContext.Current.Session("EID") & " and formname='" & value & "'")
                End If
                Dim fld As String = dss.Tables("cr").Rows(0).Item("cfield").ToString()
                Dim remarks As String = dss.Tables("cr").Rows(0).Item("remarks").ToString()

                da.SelectCommand.CommandTimeout = 700
                da.SelectCommand.CommandText = "SELECT DocID,[Current User],[LastTID],[ptat],[ordering],[fdate] FROM (select d.tid[DocID],(select distinct username from mmm_mst_user where eid=" & HttpContext.Current.Session("EID") & " and uid= dd.userid )[Current User],dd.userid,d.lasttid[Lasttid],dd.ptat[ptat],dd.ordering[ordering],dd.fdate from mmm_mst_doc d inner join mmm_doc_dtl dd on d.lasttid=dd.tid where d.eid=" & HttpContext.Current.Session("EID") & " and d.documenttype='" & documentType & "' and d.curstatus='" & status & "' and dd.userid is not null and dd.userid not in (217,363)  and " & fld & " in (select * from  InputString((select " & ff.ToString() & " from mmm_ref_role_user where eid=" & HttpContext.Current.Session("EID") & " and uid=" & HttpContext.Current.Session("UID") & " and rolename='" & HttpContext.Current.Session("USERROLE") & "'))))A  where [current user] is not null and [Current user]<>'' "

                ' Dim da As New SqlDataAdapter("select d.tid[DocID],(select username from mmm_mst_user where eid=" & Session("EID") & " and uid= (select userid from mmm_doc_dtl where tid=d.lasttid and docid=d.tid))[Current User],* from mmm_mst_doc d where d.eid=" & Session("EID") & " and d.documenttype='VRF FIXED_POOL' and d.curstatus='approved' ", con)
                Dim ds As New DataSet
                da.Fill(ds, "data")
                Dim dt As New DataTable
                dt = ds.Tables(0)
                Dim tid As Integer
                If dt.Rows.Count > 0 Then
                    Dim dsTU As New DataSet
                    tid = dt.Rows(dt.Rows.Count() - 1)("DocID").ToString()
                    da.SelectCommand.Parameters.Clear()
                    'da.SelectCommand.CommandText = "USP_get_targetusersdropdown"
                    'da.SelectCommand.CommandType = CommandType.StoredProcedure
                    'da.SelectCommand.CommandTimeout = 5000
                    'da.SelectCommand.Parameters.AddWithValue("@TID", tid)
                    'da.SelectCommand.Parameters.AddWithValue("@EID", Session("EID"))
                    'da.SelectCommand.Parameters.AddWithValue("@DOCTYPE", Trim(Session("ddlselecteddoc")))
                    'da.SelectCommand.Parameters.AddWithValue("@STATUS", Trim(Session("ddlselectedstatus")))
                    'da.Fill(ds, "users")
                    da.SelectCommand.CommandText = "USP_get_targetusers_with_Role_Restriction" '"USP_get_targetusers"
                    da.SelectCommand.CommandType = CommandType.StoredProcedure
                    da.SelectCommand.CommandTimeout = 5000
                    da.SelectCommand.Parameters.AddWithValue("@TID", tid)
                    da.SelectCommand.Parameters.AddWithValue("@EID", HttpContext.Current.Session("EID"))
                    da.SelectCommand.Parameters.AddWithValue("@DOCTYPE", Trim(documentType))
                    da.SelectCommand.Parameters.AddWithValue("@STATUS", Trim(status))
                    da.Fill(dsTU, "data")
                    jsonData = JsonConvert.SerializeObject(dsTU.Tables(0))
                    ret.Data = jsonData
                    'ddltu.DataSource = ds.Tables("data")
                    'ddltu.DataTextField = "username"
                    'ddltu.DataValueField = "uid"
                    'ddltu.DataBind()
                    'ddltu.Items.Insert(0, New ListItem("Select"))
                    'getresulfilter()
                End If
            End If
        Catch ex As Exception
        End Try
        Return ret
    End Function


    <System.Web.Services.WebMethod()>
    Public Shared Function getNextUser(docid As Integer, documentType As String, status As String) As kGridReallocation
        Dim jsonData As String = ""
        Dim ret As New kGridReallocation()
        Try
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As New SqlConnection(conStr)
            Dim da As New SqlDataAdapter("USP_get_targetusers_with_Role_Restriction", con) 'USP_get_targetusers_031220
            da.SelectCommand.CommandType = CommandType.StoredProcedure
            da.SelectCommand.CommandTimeout = 500
            da.SelectCommand.Parameters.AddWithValue("@TID", docid)
            da.SelectCommand.Parameters.AddWithValue("@EID", HttpContext.Current.Session("EID"))
            da.SelectCommand.Parameters.AddWithValue("@DOCTYPE", documentType)
            da.SelectCommand.Parameters.AddWithValue("@STATUS", status)
            Dim ds As New DataSet
            da.Fill(ds, "data")
            jsonData = JsonConvert.SerializeObject(ds.Tables(0))
            ret.Data = jsonData
        Catch ex As Exception
            Throw
        End Try
        Return ret
    End Function
    <System.Web.Services.WebMethod()>
    Public Shared Function SaveNextUser(ClsNextUser As List(Of ClsNextUser), Status As String) As String
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim da As SqlDataAdapter = New SqlDataAdapter("", con)
        Try
            Dim cnt As Integer = 0
            Dim dt As New DataTable
            dt = HttpContext.Current.Session("getresultdata")
            Dim Remarks As String = HttpContext.Current.Session("remarks")
            For i As Integer = 0 To ClsNextUser.Count - 1
                da.SelectCommand.Parameters.Clear()
                da.SelectCommand.CommandText = "SaveReallocationRights"
                da.SelectCommand.CommandType = CommandType.StoredProcedure
                da.SelectCommand.Parameters.AddWithValue("@EID", HttpContext.Current.Session("EID"))
                da.SelectCommand.Parameters.AddWithValue("@NextUID", ClsNextUser(i).NextUser)
                da.SelectCommand.Parameters.AddWithValue("@DOCID", ClsNextUser(i).DocID)
                da.SelectCommand.Parameters.AddWithValue("@STATUS", Status)
                Dim ReallactionRemaks = "R:-" + ClsNextUser(i).Remarks
                da.SelectCommand.Parameters.AddWithValue("@Remarks", ReallactionRemaks)
                If con.State <> ConnectionState.Open Then
                    con.Open()
                End If
                Dim result As Integer = da.SelectCommand.ExecuteScalar()
                'Dim da As New SqlDataAdapter("update mmm_doc_dtl set tdate=getdate(),atat=datediff(day,'" & ClsNextUser(i).fdate & "' ,getdate() ),remarks='" & Remarks & "',aprstatus='" & Status & "' where tid=(select lasttid from mmm_mst_doc where eid=" & HttpContext.Current.Session("EID") & " and tid=" & ClsNextUser(i).DocID & ")", con)
                ''String remarks = HttpContext.Current.Session("remarks")
                'If con.State <> ConnectionState.Open Then
                '    con.Open()
                'End If
                'da.SelectCommand.ExecuteNonQuery()
                'da.SelectCommand.CommandText = "insert into mmm_doc_dtl (userid,docid,fdate,tdate,ptat,atat ,aprstatus,remarks,pathID,Ordering,DocNature)values(" & ClsNextUser(i).NextUser & "," & ClsNextUser(i).DocID & ", getdate(),null," & ClsNextUser(i).ptat & ",null,null,null,0," & ClsNextUser(i).ordering & ",'CREATE' );UPdate mmm_mst_doc set lasttid=(select scope_identity()) where tid=" & ClsNextUser(i).DocID & ""
                'da.SelectCommand.ExecuteNonQuery()
            Next
            HttpContext.Current.Session("getresultdata") = Nothing
        Catch ex As Exception
        Finally
            con.Close()
            da.Dispose()
            con.Dispose()
        End Try
        Return "Submitted successfully."
    End Function
    Public Shared Function CreateStaticColCollectionForReallocation(dt As DataTable) As List(Of kColumnReallocation)
        Dim listcol As New List(Of kColumnReallocation)()
        Dim i As Integer = 0
        Dim obj As kColumnReallocation
        'Logic For adding Static Column into datatable By Nidhi.
        listcol.Add(New kColumnReallocation("DocID", "DocID", "number", ""))
        listcol.Add(New kColumnReallocation("CurrentUser", "Current User", "string", ""))
        'hh:mm:ss tt
        'listcol.Add(New kColumn("PRIORITY", "PRIORITY", "number", "", 100))
        For f As Integer = 0 To dt.Rows.Count - 1
            obj = New kColumnReallocation()
            obj.field = dt.Rows(f).Item("fieldMapping")
            obj.title = dt.Rows(f).Item("Displayname")
            obj.width = 200
            obj.type = "string"
            obj.filterable = True

            '{0:MM-dd-yyyy}
            'for dynamic column filtering..
            If (dt.Rows(f).Item("datatype")) = "Numeric" Then
                obj.type = "number"
                obj.format = ""
                'obj.filterable = ""
            ElseIf (dt.Rows(f).Item("datatype")) = "Datetime" Then
                obj.type = "string"
                'obj.format = "{0:dd/MM/yy}"
            Else
            End If

            listcol.Add(obj)
        Next
        ' listcol.Add(New kColumnReallocation("Remarks", "Remarks", "string", ""))
        Return listcol
    End Function

End Class

Public Class ClsNextUser
    Public Property DocID As Integer
    Public Property NextUser As String
    Public Property Remarks As String
    Public Property fdate As String
    Public Property ptat As String
    Public Property ordering As String

End Class

Public Class kGridReallocation
    Public Data As String = ""
    Public Count As String = ""
    Public total As Integer = 0
    Public Column As New List(Of kColumnReallocation)
End Class
Public Class kColumnReallocation
    Public Sub New()

    End Sub
    Public Sub New(staticfield As [String], statictitle As [String], statictype As String, staticFormat As String)
        field = staticfield
        title = statictitle
        type = statictype
        format = staticFormat
        filterable = True
        If (statictype = "number") Then
            filterable = ""
        End If
        'width = staticwidth
    End Sub

    Public field As String = ""
    Public title As String = ""
    Public width As Integer = 200
    Public format As String = ""
    Public filterable As String = ""
    'Public locked As Boolean = True
    'Public locked As Boolean = True
    Public type As String = ""
    Public FieldID As String = ""

End Class
