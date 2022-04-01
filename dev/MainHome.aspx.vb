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

Partial Class MainHome
    Inherits System.Web.UI.Page
    Protected dsHolidays As DataSet

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Init
        Session("EDITonEDIT") = Nothing
    End Sub

    Protected Sub Page_PreInit1(ByVal sender As Object, ByVal e As EventArgs) Handles Me.PreInit
        Try
            If Not Session("CTheme") Is Nothing And Not Session("CTheme") = String.Empty Then
                Page.Theme = Convert.ToString(Session("CTheme"))
                'Page.Theme = "Blue"

            Else
                Page.Theme = "Default"
            End If
        Catch ex As Exception
        End Try

    End Sub
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
        If Not IsNothing(Session("Approve")) Then
            ShowApprove(Session("DocID"))
        End If
        If Not IsNothing(Session("Reject")) Then
            ShowPermanentReject(Session("DocID"))
        End If
        If Not IsNothing(Session("Reconsider")) Then
            ShowReconsider(Session("DocID"))
        End If
        If Not IsPostBack Then
            Session("Pending") = Nothing
            If IsNothing(Session("PassExp")).ToString() Then
                PassExpMsgAlert()
            End If
            'For Menu Dashboard
            bindDocumentType()
            If ddldynamic.Items.Count > 1 Then
                Dim MainHome_DefaultDocument As String = ""
                oda.SelectCommand.CommandText = "select isnull(MainHome_DefaultDcoument,'0') from mmm_mst_role where eid=" & Session("EID") & " and RoleName='" & Session("USERROLE").ToString() & "'"
                con.Open()
                MainHome_DefaultDocument = Convert.ToString(oda.SelectCommand.ExecuteScalar())
                con.Dispose()
                If MainHome_DefaultDocument.Length > 1 Then
                    ddldynamic.SelectedValue = MainHome_DefaultDocument.Trim()
                    ddldynamic_SelectedIndexChanged(Me.ddldynamic, EventArgs.Empty)
                Else
                    ddldynamic.SelectedIndex = ddldynamic.Items.IndexOf(ddldynamic.Items.Item(1))
                    ddldynamic_SelectedIndexChanged(Me.ddldynamic, EventArgs.Empty)
                End If

            End If
            ViewState("doctype") = ddldynamic.SelectedItem.Text
            If IsBindDefaultDashBoard() Then
                BindDashBoard()
            Else

            End If
            'PendingGrdBind()
            Session("PassExp") = "1"
            FillSubordinates(Session("UID"))   '        '' by sp for getting subordinates of logged user recursively  - 20_feb_14
            ' migrationtionAlert()  ' changes by balli for showing alert mess to sequel client
            ModalApprove.Hide()
            btncalendar_modalPopupExtender.Hide()
            btnPerRejectModalpopup.Hide()
        Else
            btnpendinggrdcl_Click(sender, New EventArgs())
            'gvPending.DataSource = Nothing
            'gvPending.DataBind()
            'ScriptManager.RegisterStartupScript(Me, Me.GetType(), "refresh", " <script type='text/javascript'>document.getElementById('" + btnpendinggrdcl.ClientID + "').click() ;</script>", False)
        End If
    End Sub

    Public Sub migrationtionAlert()

        'prev - commented on 02-jul - ' Dim eidStr As String = "32,37,41,42,43,44,45,48,53,56,57,58,59,60,61,63,64,65,66,67,68,69,70,71,72,73,75,77,79,81,82,85,86,87"
        '' below is latest list only live accounts in sequel. by SP
        Dim eidStr As String = "32,56,57,58,60,66,67,79,85,89,95"
        If eidStr.Contains(Session("EID")) Then
            lblAlertMes.Text = "Please note that we are soon moving to a new URL " & Session("CODE") & ".bpm.sequelone.com"
            updAlert.Update()
            MP_Alert.Show()
        Else
            MP_Alert.Hide()
        End If
    End Sub

    Public Sub HideAlert()
        MP_Alert.Hide()
    End Sub
#Region "DashBoard Filtering"
    Private Sub bindDocumentType()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
        'oda.SelectCommand.CommandText = "select uid,rolename,documenttype from MMM_ref_role_user where eid=" & Session("EID") & " and uid=" & Session("UID") & " and rolename='" & Session("ROLES") & "' "
        Try
            'oda.SelectCommand.CommandText = "select MID,MENUNAME,PAGELINK,pmenu,image,roles,dord from mmm_mst_menu where ROLES <> '0' and roles like '" & "%{" & Session("USERROLE") & "%" & "' AND EID=" & Session("EID") & " AND Mtype='DYNAMIC' order by dord   "
            oda.SelectCommand.CommandText = "Select documenttype  from MMM_MST_DOC M with (nolock)  LEFT OUTER JOIN MMM_DOC_DTL D with (nolock) on D.tid=M.lasttid  LEFT OUTER JOIN MMM_MST_USER U with (nolock) ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP with (nolock) ON AP.uid=D.UserID where D.userid = " & Session("UID") & "  union Select  distinct documenttype from MMM_MST_DOC M with (nolock)  LEFT OUTER JOIN MMM_DOC_DTL D with (nolock) on D.docid =M.tid  LEFT OUTER JOIN MMM_MST_USER U with (nolock) ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP with (nolock) ON AP.uid=D.UserID where D.UserID = " & Session("UID") & " and aprstatus is not null and m.curstatus <> 'ARCHIVE'  union  Select distinct documenttype from MMM_MST_DOC M with (nolock)  LEFT OUTER JOIN MMM_DOC_DTL D with (nolock) on D.tid=M.lasttid  LEFT OUTER JOIN MMM_MST_USER U with (nolock) ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP with (nolock) ON AP.uid=D.UserID where M.oUid = " & Session("UID") & " and curstatus<> 'ARCHIVE' "
            Dim ds As New DataSet()
            oda.Fill(ds, "data")

            '' by sunil for selecting item if there is only single entry in ddn list - 22_jan_15
            ddldynamic.Items.Clear()
            ddldynamic.Items.Add("SELECT ALL")

            If ds.Tables("data").Rows.Count > 0 Then
                Dim k As Integer = 0
                For j As Integer = 0 To ds.Tables("data").Rows.Count - 1
                    If ds.Tables("data").Rows(j).Item(0).ToString() <> "" Then
                        ddldynamic.Items.Add(ds.Tables("data").Rows(j).Item(0).ToString)
                        ddldynamic.Items(k + 1).Value = ds.Tables("data").Rows(j).Item(0).ToString
                        k += 1
                    End If
                Next
            End If
            '' by sunil for selecting item if there is only single entry in ddn list - 22_jan_15

        Finally
            con.Close()
            con.Dispose()
            oda.Dispose()
        End Try

    End Sub
    ' methods for binding grid saperately
    Protected Sub PendingGrdBind(Optional ByVal searchingpart As String = Nothing, Optional ByVal sortingpart As String = Nothing)

        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        'With(nolock) added by Himank on 29th sep 2015
        Dim oda As SqlDataAdapter = New SqlDataAdapter("select * from MMM_MST_FIELDS   WITH(NOLOCK)  where eid=" & Session("EID").ToString() & " and Documenttype='" & ViewState("doctype") & "' and showondashboard=1 order by displayorder; select ShowStaticfieldonDashBoard,OrderOfDeshBoardFields from mmm_mst_entity   WITH(NOLOCK)  where eid=" & Session("EID") & ";select * from mmm_mst_needtoact_config where eid=" & Session("EID") & " and DocType='" & ViewState("doctype") & "' order by DisplayOrder", con)
        Try
            oda.SelectCommand.CommandType = CommandType.Text
            Dim ds As New DataSet()
            oda.Fill(ds, "data")

            Dim sStatement As String = ""
            Dim qrycolumn As String = ""
            Dim ShowStaticfieldonDashBoard As Integer = 0
            Dim OrderOfDeshBoardFields As String = ""
            If ds.Tables("data2").Rows.Count > 0 Then
                If Not IsNothing(searchingpart) Then
                    sStatement = "Select  distinct  M.TID as [SYSTEM ID],"
                Else
                    sStatement = "Select  distinct Top 50 M.TID as [SYSTEM ID],"
                End If

                Dim TempQry As New ArrayList
                For p As Integer = 0 To ds.Tables("data2").Rows.Count - 1
                    If ds.Tables("data2").Rows(p).Item("Type").ToString().ToUpper = "STATIC" Then
                        TempQry.Add(ds.Tables("data2").Rows(p).Item("FieldMapping") & " As [" & ds.Tables("data2").Rows(p).Item("FieldName") & "] ")
                    Else
                        TempQry.Add("M." & ds.Tables("data2").Rows(p).Item("FieldMapping") & " As [" & ds.Tables("data2").Rows(p).Item("FieldName") & "] ")
                    End If

                Next
                qrycolumn = sStatement & String.Join(",", TempQry.ToArray) & "from MMM_MST_DOC M with (nolock)  LEFT OUTER JOIN MMM_DOC_DTL D with (nolock) on D.tid=M.lasttid  LEFT OUTER JOIN MMM_MST_USER U with (nolock) ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP with (nolock) ON AP.uid=D.UserID where D.userid = " & Val(Session("UID").ToString()) & " and M.documenttype='" & ViewState("doctype") & "' and m.curstatus<>'ARCHIVE' order by m.tid desc"
            Else
                sStatement = "Select   distinct Top 50 M.tid[SYSTEM ID], M.DocumentType[SUBJECT],M.curstatus[STATUS],U.Username[CREATED BY],(replace(convert(char(11),d.fdate,113),' ','-') + ' ' + left(convert(varchar(13),d.fdate,108),8))[RECEIVED ON],datediff(day,fdate,getdate())[PENDING DAYS],M.PRIORITY  ,"
                qrycolumn = Left(sStatement, Len(sStatement) - 1) & " from MMM_MST_DOC M with (nolock)  LEFT OUTER JOIN MMM_DOC_DTL D with (nolock) on D.tid=M.lasttid  LEFT OUTER JOIN MMM_MST_USER U with (nolock) ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP with (nolock) ON AP.uid=D.UserID where D.userid = " & Val(Session("UID").ToString()) & " and M.documenttype='" & ViewState("doctype") & "' and m.curstatus<>'ARCHIVE' order by m.tid desc"
            End If
            'sStatement = Left(sStatement, Len(sStatement) - 1) & " FROM MMM_MST_DOC WHERE eid=" & Session("EID").ToString() & " and Documenttype='" & fmname & "'"
            'If OrderOfDeshBoardFields.ToUpper = "STATIC" Then
            '    qrycolumn = Left(qrycolumn, Len(qrycolumn) - 1) & " from MMM_MST_DOC M with (nolock)  LEFT OUTER JOIN MMM_DOC_DTL D with (nolock) on D.tid=M.lasttid  LEFT OUTER JOIN MMM_MST_USER U with (nolock) ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP with (nolock) ON AP.uid=D.UserID where D.userid = " & Val(Session("UID").ToString()) & " and M.documenttype='" & ViewState("doctype") & "' and m.curstatus<>'ARCHIVE' order by m.priority desc ,m.tid desc"
            'Else
            '    qrycolumn = Left(qrycolumn, Len(qrycolumn) - 1) & " from MMM_MST_DOC M with (nolock)  LEFT OUTER JOIN MMM_DOC_DTL D with (nolock) on D.tid=M.lasttid  LEFT OUTER JOIN MMM_MST_USER U with (nolock) ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP with (nolock) ON AP.uid=D.UserID where D.userid = " & Val(Session("UID").ToString()) & " and M.documenttype='" & ViewState("doctype") & "' and m.curstatus<>'ARCHIVE' order by m.tid desc"
            'End If




            'oda.SelectCommand.CommandText = "Select distinct  M.tid,curstatus,U.username,(replace(convert(char(11),d.fdate,113),' ','-') + ' ' + left(convert(varchar(13),d.fdate,108),8))[fdate],AP.Username [apusername],datediff(day,fdate,getdate())[Pending Days],m.priority ," & fields & " from MMM_MST_DOC M with (nolock)  LEFT OUTER JOIN MMM_DOC_DTL D with (nolock) on D.tid=M.lasttid  LEFT OUTER JOIN MMM_MST_USER U with (nolock) ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP with (nolock) ON AP.uid=D.UserID where D.userid = " & Val(Session("UID").ToString()) & " and M.documenttype='" & ddlDocType.SelectedItem.Text & "' order by M.priority desc,m.tid desc
            If Not IsNothing(searchingpart) Then
                qrycolumn = qrycolumn & searchingpart
            End If
            If Not IsNothing(sortingpart) Then
                qrycolumn = qrycolumn & sortingpart
            Else

            End If
            oda.SelectCommand.CommandText = qrycolumn
            ViewState("pendingqry") = oda.SelectCommand.CommandText
            oda.Fill(ds, "pending")
            ViewState("pending") = ds.Tables("Pending")

            'Dim sStatement As String = "Select distinct M.tid[SYSTEM ID],M.DocumentType[SUBJECT],M.curstatus[STATUS],U.Username[CREATED BY],(replace(convert(char(11),d.fdate,113),' ','-') + ' ' + left(convert(varchar(13),d.fdate,108),8))[RECEIVED ON],datediff(day,fdate,getdate())[PENDING DAYS],M.PRIORITY  ,"
            'For i As Integer = 0 To ds.Tables("data").Rows.Count - 1
            '    sStatement &= "M." & ds.Tables("data").Rows(i).Item("FieldMapping").ToString().ToUpper & " AS [" & ds.Tables("data").Rows(i).Item("displayname").ToString().ToUpper & "],"
            'Next
            'Dim qrycolumn As String = sStatement
            ''sStatement = Left(sStatement, Len(sStatement) - 1) & " FROM MMM_MST_DOC WHERE eid=" & Session("EID").ToString() & " and Documenttype='" & fmname & "'"
            'qrycolumn = Left(qrycolumn, Len(qrycolumn) - 1) & " from MMM_MST_DOC M with (nolock)  LEFT OUTER JOIN MMM_DOC_DTL D with (nolock) on D.tid=M.lasttid  LEFT OUTER JOIN MMM_MST_USER U with (nolock) ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP with (nolock) ON AP.uid=D.UserID where D.userid = " & Val(Session("UID").ToString()) & " and M.documenttype='" & ViewState("doctype") & "' order by m.priority desc ,m.tid desc"
            ''oda.SelectCommand.CommandText = "Select distinct  M.tid,curstatus,U.username,(replace(convert(char(11),d.fdate,113),' ','-') + ' ' + left(convert(varchar(13),d.fdate,108),8))[fdate],AP.Username [apusername],datediff(day,fdate,getdate())[Pending Days],m.priority ," & fields & " from MMM_MST_DOC M with (nolock)  LEFT OUTER JOIN MMM_DOC_DTL D with (nolock) on D.tid=M.lasttid  LEFT OUTER JOIN MMM_MST_USER U with (nolock) ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP with (nolock) ON AP.uid=D.UserID where D.userid = " & Val(Session("UID").ToString()) & " and M.documenttype='" & ddlDocType.SelectedItem.Text & "' order by M.priority desc,m.tid desc
            'oda.SelectCommand.CommandText = qrycolumn
            'oda.Fill(ds, "pending")
            'ViewState("pending") = ds.Tables("Pending")
            ' binding need to act Tab



            If IsNothing(Session("Pending")) Then
                gvPending.Columns.Clear()
                Dim tfield As New TemplateField()
                tfield.HeaderText = "Actions"
                gvPending.Columns.Add(tfield)
                Dim x As Integer = 0
                For Each col As DataColumn In ds.Tables("pending").Columns
                    Dim tmpfield As New TemplateField()
                    If x = 0 Then
                        tmpfield.HeaderText = "View Detail"
                        x = x + 1
                    Else

                        ' tmpfield.HeaderStyle.Font.Underline = True
                        tmpfield.SortExpression = col.ColumnName

                        tmpfield.HeaderText = col.ColumnName
                        x = x + 1
                    End If

                    gvPending.Columns.Add(tmpfield)
                Next
            End If
            gvPending.DataSource = ds.Tables("pending")
            gvPending.DataBind()

            lblpending.Text = ""
            lblpending.Text = "(" & ds.Tables("pending").Rows.Count & ")"

            'ClientScript.RegisterStartupScript(Me.GetType(), "$('#ContentPlaceHolder1_lblpending').html('" & ds.Tables("pending").Rows.Count & "');", "script", True)
            'ScriptManager.RegisterStartupScript(Me.updPnlGrid, Me.updPnlGrid.GetType(), "Mayank", "$('#ContentPlaceHolder1_lblpending').html('" & ds.Tables("pending").Rows.Count & "');", True)
            Session("Pending") = "1"
        Catch ex As Exception
        Finally
            con.Close()
            oda.Dispose()
        End Try

        For i As Integer = 0 To gvPending.Rows.Count - 1
            FillMainPage(gvPending.DataKeys(i).Value, i)
        Next
    End Sub


    'Make parametarased method 

    Private Sub MyRequestBind()

        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        'With(nolock) added by Himank on 29th sep 2015
        Dim oda As SqlDataAdapter = New SqlDataAdapter("select * from MMM_MST_FIELDS   WITH(NOLOCK)  where eid=" & Session("EID").ToString() & " and Documenttype='" & ViewState("doctype") & "' and showondashboard=1 order by displayorder; select ShowStaticfieldonDashBoard,OrderOfDeshBoardFields from mmm_mst_entity   WITH(NOLOCK)  where eid=" & Session("EID") & ";select * from mmm_mst_needtoact_config where eid=" & Session("EID") & " and DocType='" & ViewState("doctype") & "' order by DisplayOrder", con)
        Try
            oda.SelectCommand.CommandType = CommandType.Text
            Dim ds As New DataSet()
            oda.Fill(ds, "data")

            Dim sStatement As String = ""
            Dim qrycolumn As String = ""
            Dim ShowStaticfieldonDashBoard As Integer = 0
            Dim OrderOfDeshBoardFields As String = ""
            If ds.Tables("data2").Rows.Count > 0 Then
                sStatement = "Select distinct TOP 8  M.TID as [SYSTEM ID] ,"
                Dim TempQry As New ArrayList
                For p As Integer = 0 To ds.Tables("data2").Rows.Count - 1
                    If ds.Tables("data2").Rows(p).Item("Type").ToString().ToUpper = "STATIC" Then
                        TempQry.Add(ds.Tables("data2").Rows(p).Item("FieldMapping") & " As [" & ds.Tables("data2").Rows(p).Item("FieldName") & "] ")
                    Else
                        TempQry.Add("M." & ds.Tables("data2").Rows(p).Item("FieldMapping") & " As [" & ds.Tables("data2").Rows(p).Item("FieldName") & "] ")
                    End If

                Next
                qrycolumn = sStatement & String.Join(",", TempQry.ToArray) & "from MMM_MST_DOC M with (nolock)  LEFT OUTER JOIN MMM_DOC_DTL D with (nolock) on D.tid=M.lasttid  LEFT OUTER JOIN MMM_MST_USER U with (nolock) ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP with (nolock) ON AP.uid=D.UserID where M.oUid = " & Val(Session("UID").ToString()) & " and M.documenttype='" & ViewState("doctype") & "' and curstatus<> 'ARCHIVE' order by M.Tid desc"
            Else
                If ds.Tables("data1").Rows.Count > 0 Then
                    sStatement = "Select distinct TOP 8 M.tid[SYSTEM ID],M.DocumentType[SUBJECT],M.curstatus[STATUS],U.Username[CREATED BY],(replace(convert(char(11),d.fdate,113),' ','-') + ' ' + left(convert(varchar(13),d.fdate,108),8))[RECEIVED ON],"
                    qrycolumn = sStatement
                    qrycolumn = Left(qrycolumn, Len(qrycolumn) - 1) & " from MMM_MST_DOC M with (nolock)  LEFT OUTER JOIN MMM_DOC_DTL D with (nolock) on D.tid=M.lasttid  LEFT OUTER JOIN MMM_MST_USER U with (nolock) ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP with (nolock) ON AP.uid=D.UserID where M.oUid = " & Val(Session("UID").ToString()) & " and M.documenttype='" & ViewState("doctype") & "' and curstatus<> 'ARCHIVE' order by M.Tid desc"
                End If
            End If

            oda.SelectCommand.CommandText = qrycolumn
            oda.Fill(ds, "myrequest")
            ViewState("myrequest") = ds.Tables("myrequest")
            gvMyUpload.DataSource = ds.Tables("myrequest")
            gvMyUpload.DataBind()

            lblaction.Text = ""
            lblaction.Text = "(" & ds.Tables("myrequest").Rows.Count & ")"

        Catch ex As Exception
        Finally
            con.Close()
            oda.Dispose()
        End Try
    End Sub

    Protected Sub MyActionGRdBind()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        'With(nolock) added by Himank on 29th sep 2015
        Dim oda As SqlDataAdapter = New SqlDataAdapter("select * from MMM_MST_FIELDS   WITH(NOLOCK)  where eid=" & Session("EID").ToString() & " and Documenttype='" & ViewState("doctype") & "' and showondashboard=1 order by displayorder;select ShowStaticfieldonDashBoard,OrderOfDeshBoardFields from mmm_mst_entity where eid=" & Session("EID") & ";select * from mmm_mst_needtoact_config where eid=" & Session("EID") & " and DocType='" & ViewState("doctype") & "' order by DisplayOrder", con)
        Try
            oda.SelectCommand.CommandType = CommandType.Text
            Dim ds As New DataSet()
            oda.Fill(ds, "data")

            Dim sStatement As String = ""
            Dim qrycolumn As String = ""
            Dim ShowStaticfieldonDashBoard As Integer = 0
            Dim OrderOfDeshBoardFields As String = ""
            If ds.Tables("data2").Rows.Count > 0 Then
                sStatement = "Select distinct TOP 8  M.TID as [SYSTEM ID] ,"
                Dim TempQry As New ArrayList
                For p As Integer = 0 To ds.Tables("data2").Rows.Count - 1
                    If ds.Tables("data2").Rows(p).Item("Type").ToString().ToUpper = "STATIC" Then
                        TempQry.Add(ds.Tables("data2").Rows(p).Item("FieldMapping") & " As [" & ds.Tables("data2").Rows(p).Item("FieldName") & "] ")
                    Else
                        TempQry.Add("M." & ds.Tables("data2").Rows(p).Item("FieldMapping") & " As [" & ds.Tables("data2").Rows(p).Item("FieldName") & "] ")
                    End If

                Next
                qrycolumn = sStatement & String.Join(",", TempQry.ToArray) & "from MMM_MST_DOC M with (nolock)  LEFT OUTER JOIN MMM_DOC_DTL D with (nolock) on D.docid =M.tid  LEFT OUTER JOIN MMM_MST_USER U with (nolock) ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP with (nolock) ON AP.uid=D.UserID where D.UserID = " & Val(Session("UID").ToString()) & " and aprstatus is not null and m.curstatus <> 'ARCHIVE'  and m.documenttype='" & ViewState("doctype") & "' order by M.Tid desc"
            Else
                If ds.Tables("data1").Rows.Count > 0 Then

                    ShowStaticfieldonDashBoard = Convert.ToInt32(ds.Tables("data1").Rows(0)("ShowStaticfieldonDashBoard"))
                    OrderOfDeshBoardFields = Convert.ToString(ds.Tables("data1").Rows(0)("OrderOfDeshBoardFields"))
                    If ShowStaticfieldonDashBoard = 1 Then
                        If OrderOfDeshBoardFields.ToUpper = "STATIC" Then
                            sStatement = "Select distinct TOP 8 M.tid[SYSTEM ID],M.DocumentType[SUBJECT],M.curstatus[STATUS],U.Username[CREATED BY],(replace(convert(char(11),d.fdate,113),' ','-') + ' ' + left(convert(varchar(13),d.fdate,108),8))[RECEIVED ON],datediff(day,fdate,getdate())[PENDING DAYS],"
                            For i As Integer = 0 To ds.Tables("data").Rows.Count - 1
                                sStatement &= "M." & ds.Tables("data").Rows(i).Item("FieldMapping").ToString().ToUpper & " AS [" & ds.Tables("data").Rows(i).Item("displayname").ToString().ToUpper & "],"
                            Next
                            qrycolumn = sStatement
                        Else
                            sStatement = "Select distinct TOP 8 M.tid[SYSTEM ID],"
                            For i As Integer = 0 To ds.Tables("data").Rows.Count - 1
                                sStatement &= "M." & ds.Tables("data").Rows(i).Item("FieldMapping").ToString().ToUpper & " AS [" & ds.Tables("data").Rows(i).Item("displayname").ToString().ToUpper & "],"
                            Next
                            qrycolumn = sStatement
                        End If
                    Else
                        sStatement = "Select distinct TOP 8 M.tid[SYSTEM ID],"
                        For i As Integer = 0 To ds.Tables("data").Rows.Count - 1
                            sStatement &= "M." & ds.Tables("data").Rows(i).Item("FieldMapping").ToString().ToUpper & " AS [" & ds.Tables("data").Rows(i).Item("displayname").ToString().ToUpper & "],"
                        Next
                        qrycolumn = sStatement
                    End If
                End If
                qrycolumn = Left(qrycolumn, Len(qrycolumn) - 1) & " from MMM_MST_DOC M with (nolock)  LEFT OUTER JOIN MMM_DOC_DTL D with (nolock) on D.docid =M.tid  LEFT OUTER JOIN MMM_MST_USER U with (nolock) ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP with (nolock) ON AP.uid=D.UserID where D.UserID = " & Val(Session("UID").ToString()) & " and aprstatus is not null and m.curstatus <> 'ARCHIVE'  and m.documenttype='" & ViewState("doctype") & "' order by M.Tid desc"
            End If

            oda.SelectCommand.CommandText = qrycolumn
            oda.Fill(ds, "History")
            ViewState("History") = ds.Tables("History")
            gvAction.DataSource = ds.Tables("History")
            gvAction.DataBind()
            lbluploading.Text = ""
            lbluploading.Text = "(" & ds.Tables("History").Rows.Count & ")"

        Catch ex As Exception
        Finally
            con.Close()
            oda.Dispose()
        End Try
    End Sub

    'BIND DRAFT GRID
    Protected Sub MyDRAFTGRdBind()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        'With(nolock) added by Himank on 29th sep 2015
        Dim oda As SqlDataAdapter = New SqlDataAdapter("select * from MMM_MST_FIELDS   WITH(NOLOCK)  where eid=" & Session("EID").ToString() & " and Documenttype='" & ViewState("doctype") & "' and showondashboard=1;select ShowStaticfieldonDashBoard,OrderOfDeshBoardFields from mmm_mst_entity where eid=" & Session("EID") & ";select * from mmm_mst_needtoact_config where eid=" & Session("EID") & " and DocType='" & ViewState("doctype") & "' order by DisplayOrder", con)
        Try
            oda.SelectCommand.CommandType = CommandType.Text
            Dim ds As New DataSet()
            oda.Fill(ds, "data")

            Dim sStatement As String = ""
            Dim qrycolumn As String = ""
            Dim ShowStaticfieldonDashBoard As Integer = 0
            Dim OrderOfDeshBoardFields As String = ""
            If ds.Tables("data2").Rows.Count > 0 Then
                sStatement = "Select distinct TOP 8  M.TID as [SYSTEM ID] ,"
                Dim TempQry As New ArrayList
                For p As Integer = 0 To ds.Tables("data2").Rows.Count - 1
                    If ds.Tables("data2").Rows(p).Item("Type").ToString().ToUpper = "STATIC" Then
                        TempQry.Add(ds.Tables("data2").Rows(p).Item("FieldMapping") & " As [" & ds.Tables("data2").Rows(p).Item("FieldName") & "] ")
                    Else
                        TempQry.Add("M." & ds.Tables("data2").Rows(p).Item("FieldMapping") & " As [" & ds.Tables("data2").Rows(p).Item("FieldName") & "] ")
                    End If

                Next
                qrycolumn = sStatement & String.Join(",", TempQry.ToArray) & "from MMM_MST_DOC_draft M with (nolock)  LEFT OUTER JOIN MMM_MST_USER U with (nolock) ON U.uid=oUID  where m.ouID = " & Val(Session("UID").ToString()) & " and  m.curstatus <> 'ARCHIVE'  and m.documenttype='" & ViewState("doctype") & "' order by M.Tid desc"
            Else
                If ds.Tables("data1").Rows.Count > 0 Then

                    ShowStaticfieldonDashBoard = Convert.ToInt32(ds.Tables("data1").Rows(0)("ShowStaticfieldonDashBoard"))
                    OrderOfDeshBoardFields = Convert.ToString(ds.Tables("data1").Rows(0)("OrderOfDeshBoardFields"))
                    If ShowStaticfieldonDashBoard = 1 Then
                        If OrderOfDeshBoardFields.ToUpper = "STATIC" Then
                            sStatement = "Select distinct TOP 8 M.tid[SYSTEM ID],M.DocumentType[SUBJECT],M.curstatus[STATUS],U.Username[CREATED BY],"
                            For i As Integer = 0 To ds.Tables("data").Rows.Count - 1
                                sStatement &= "M." & ds.Tables("data").Rows(i).Item("FieldMapping").ToString().ToUpper & " AS [" & ds.Tables("data").Rows(i).Item("displayname").ToString().ToUpper & "],"
                            Next
                            qrycolumn = sStatement
                        Else
                            sStatement = "Select distinct TOP 8 M.tid[SYSTEM ID],"
                            For i As Integer = 0 To ds.Tables("data").Rows.Count - 1
                                sStatement &= "M." & ds.Tables("data").Rows(i).Item("FieldMapping").ToString().ToUpper & " AS [" & ds.Tables("data").Rows(i).Item("displayname").ToString().ToUpper & "],"
                            Next
                            qrycolumn = sStatement
                        End If
                    Else
                        sStatement = "Select distinct TOP 8 M.tid[SYSTEM ID],"
                        For i As Integer = 0 To ds.Tables("data").Rows.Count - 1
                            sStatement &= "M." & ds.Tables("data").Rows(i).Item("FieldMapping").ToString().ToUpper & " AS [" & ds.Tables("data").Rows(i).Item("displayname").ToString().ToUpper & "],"
                        Next
                        qrycolumn = sStatement
                    End If
                End If

                'qrycolumn = Left(qrycolumn, Len(qrycolumn) - 1) & " from MMM_MST_DOC_draft M with (nolock)  LEFT OUTER JOIN MMM_MST_USER U with (nolock) ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP with (nolock) ON AP.uid=D.UserID where D.UserID = " & Val(Session("UID").ToString()) & " and aprstatus is not null and m.curstatus <> 'ARCHIVE'  and m.documenttype='" & ViewState("doctype") & "' order by M.Tid desc"

                qrycolumn = Left(qrycolumn, Len(qrycolumn) - 1) & " from MMM_MST_DOC_draft M with (nolock)  LEFT OUTER JOIN MMM_MST_USER U with (nolock) ON U.uid=oUID  where m.ouID = " & Val(Session("UID").ToString()) & " and  m.curstatus <> 'ARCHIVE'  and m.documenttype='" & ViewState("doctype") & "' order by M.Tid desc"

            End If


            oda.SelectCommand.CommandText = qrycolumn
            oda.Fill(ds, "MyDraft")
            ViewState("MyDraft") = ds.Tables("MyDraft")
            gvDraft.DataSource = ds.Tables("MyDraft")
            gvDraft.DataBind()
            lblDraft.Text = ""
            lblDraft.Text = "(" & ds.Tables("MyDraft").Rows.Count & ")"

        Catch ex As Exception
        Finally
            con.Close()
            oda.Dispose()
        End Try
    End Sub

    'Protected Sub mnuDocTypeNeedToAct_MenuItemClick(sender As Object, e As MenuEventArgs) Handles mnuDocTypeNeedToAct.MenuItemClick
    '    'Take formname
    '    Dim fmname As String = mnuDocTypeNeedToAct.SelectedValue
    '    ViewState("doctype") = fmname
    '    Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
    '    Dim con As New SqlConnection(conStr)
    '    Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
    '    ' code for binding drop down DDlPending 
    '    Try
    '        PendingGrdBind()
    '        MyRequestBind()
    '        MyActionGRdBind()

    '        oda.SelectCommand.CommandText = "select displayname,fieldmapping from MMM_MST_Fields where eid='" & Session("EID") & "' and documenttype='" & fmname & "' and showonDashBoard=1 "
    '        Dim dt As New DataTable
    '        oda.Fill(dt)
    '        ddlPendinggrdHdr.DataSource = dt
    '        ddlPendinggrdHdr.DataTextField = "displayname"
    '        ddlPendinggrdHdr.DataValueField = "fieldmapping"
    '        ddlPendinggrdHdr.DataBind()
    '        ddlPendinggrdHdr.Items.Insert(0, "SELECT")
    '        ddlPendinggrdHdr.Items.Insert(1, "RECIEVED ON")
    '        ddlPendinggrdHdr.Items.Insert(2, "DOC ID")
    '        ddlPendinggrdHdr.Items.Insert(3, "CREATED BY")
    '        ddlPendinggrdHdr.Items.Insert(3, "CURRENT STATUS")

    '        ddlMyReqHdr.DataSource = dt
    '        ddlMyReqHdr.DataTextField = "displayname"
    '        ddlMyReqHdr.DataValueField = "fieldmapping"
    '        ddlMyReqHdr.DataBind()
    '        ddlMyReqHdr.Items.Insert(0, "SELECT")
    '        ddlMyReqHdr.Items.Insert(1, "RECIEVED ON")
    '        ddlMyReqHdr.Items.Insert(2, "DOC ID")
    '        ddlMyReqHdr.Items.Insert(3, "CREATED BY")
    '        ddlMyReqHdr.Items.Insert(3, "CURRENT STATUS")

    '        ddlGrdHdr.DataSource = dt
    '        ddlGrdHdr.DataTextField = "displayname"
    '        ddlGrdHdr.DataValueField = "fieldmapping"
    '        ddlGrdHdr.DataBind()
    '        ddlGrdHdr.Items.Insert(0, "SELECT")
    '        ddlGrdHdr.Items.Insert(1, "RECIEVED ON")
    '        ddlGrdHdr.Items.Insert(2, "DOC ID")
    '        ddlGrdHdr.Items.Insert(3, "CREATED BY")
    '        ddlGrdHdr.Items.Insert(3, "CURRENT STATUS")
    '        ' here bind the grid of pending need to act   

    '        oda.Dispose()
    '        dt.Dispose()
    '    Catch ex As Exception
    '    Finally
    '        con.Dispose()
    '        con.Close()
    '    End Try
    'End Sub
    ' this is for search click button
    Protected Sub btnpendinggrdcl_Click(sender As Object, e As EventArgs) Handles btnpendinggrdcl.Click

        Dim flag As Char = "0"
        Dim datatable As DataTable = ViewState("pending")
        Dim DBDT As DataView = datatable.DefaultView
        'Change By Ajeet Dated on 17 dec 2014
        Dim DOCID As Integer = 0
        Try
            DOCID = Convert.ToInt32(txtPendinggrdval.Text.Trim)
        Catch ex As Exception
            DOCID = 0
        End Try
        If IsBindDefaultDashBoard() Then
            Select Case ddlPendinggrdHdr.SelectedItem.Text.ToUpper()
                Case "SELECT"
                    Exit Sub
                Case "RECIEVED ON"
                    DBDT.RowFilter = "[RECEIVED ON] = '" & txtPendinggrdval.Text & "' "
                Case "DOC ID"
                    DBDT.RowFilter = "[SYSTEM ID]= '" & DOCID & "' "
                Case "CREATED BY"
                    DBDT.RowFilter = "[CREATED BY] = '" & txtPendinggrdval.Text & "' "
                Case "CURRENT STATUS"
                    DBDT.RowFilter = "STATUS = '" & txtPendinggrdval.Text & "' "
                Case Else
                    'Dim str As String = "[" & ddlPendinggrdHdr.SelectedItem.Text.ToString() & "]='" & txtPendinggrdval.Text & "'"
                    DBDT.RowFilter = "[" & ddlPendinggrdHdr.SelectedItem.Text.ToString() & "]='" & txtPendinggrdval.Text & "'"
            End Select
        Else
            Dim datafields As New DataTable()
            Dim qry As String = Convert.ToString(ViewState("pendingqry")).ToUpper
            Dim passconval As String = Nothing
            Dim searchalias As String() = qry.Split(",")
            Select Case ddlPendinggrdHdr.SelectedItem.Text.ToUpper()
                Case "SELECT"
                    PendingGrdBind()
                    Exit Sub
                Case "DOC ID"
                    passconval = " and M.TID like '%" & txtPendinggrdval.Text & "%'"
                Case Else
                    For i As Integer = 0 To searchalias.Length - 1
                        If searchalias(i).Contains("[" & ddlPendinggrdHdr.SelectedItem.Text.ToUpper() & "]") Then
                            Dim exactpart As String = Convert.ToString(searchalias(i)).Replace("[" & ddlPendinggrdHdr.SelectedItem.Text.ToUpper() & "]", "")
                            exactpart = exactpart.ToUpper().Replace("AS", "")
                            qry = qry & "  and " & Convert.ToString(exactpart) & " like '%" & txtPendinggrdval.Text & "%'"
                            passconval = "  and " & Convert.ToString(exactpart) & " like '%" & txtPendinggrdval.Text & "%'"
                            Exit For
                        End If
                    Next
            End Select
            PendingGrdBind(passconval)
        End If
    End Sub


    Protected Sub btnmyreqcl_Click(sender As Object, e As EventArgs) Handles btnmyreqcl.Click
        Dim flag As Char = "0"
        Dim datatable As DataTable = ViewState("myrequest")
        Dim DBDT As DataView = datatable.DefaultView
        'Change By Ajeet Dated on 17 dec 2014
        Dim DOCID As Integer = 0
        Try
            DOCID = Convert.ToInt32(txtmyreqval.Text.Trim)
        Catch ex As Exception
            DOCID = 0
        End Try
        Select Case ddlMyReqHdr.SelectedItem.Text.ToUpper()
            Case "SELECT"
                Exit Sub
            Case "RECIEVED ON"
                DBDT.RowFilter = "[RECEIVED ON] = '" & txtmyreqval.Text & "' "
            Case "DOC ID"
                DBDT.RowFilter = "[SYSTEM ID] = '" & DOCID & "' "
            Case "CREATED BY"
                DBDT.RowFilter = "[CREATED BY] = '" & txtmyreqval.Text & "' "
            Case "CURRENT STATUS"
                DBDT.RowFilter = "STATUS = '" & txtmyreqval.Text & "' "
            Case Else
                'Dim str As String = "[" & ddlPendinggrdHdr.SelectedItem.Text.ToString() & "]='" & txtPendinggrdval.Text & "'"
                DBDT.RowFilter = "[" & ddlMyReqHdr.SelectedItem.Text.ToString() & "]='" & txtmyreqval.Text & "'"
        End Select
        Dim datafields As DataTable = DBDT.Table.DefaultView.ToTable()
        gvMyUpload.DataSource = datafields
        gvMyUpload.DataBind()
    End Sub

    Protected Sub btngrdcl_Click(sender As Object, e As EventArgs) Handles btngrdcl.Click

        Dim flag As Char = "0"
        Dim datatable As DataTable = ViewState("History")
        Dim DBDT As DataView = datatable.DefaultView
        'Change By Ajeet Dated on 17 dec 2014
        Dim DOCID As Integer = 0
        Try
            DOCID = Convert.ToInt32(txtgrdval.Text.Trim)
        Catch ex As Exception
            DOCID = 0
        End Try
        Select Case ddlGrdHdr.SelectedItem.Text.ToUpper()
            Case "SELECT"
                Exit Sub
            Case "RECIEVED ON"
                DBDT.RowFilter = "[RECEIVED ON]  = '" & txtgrdval.Text & "' "
            Case "DOC ID"
                DBDT.RowFilter = "[SYSTEM ID] = '" & DOCID & "' "
            Case "CREATED BY"
                DBDT.RowFilter = "[CREATED BY] = '" & txtgrdval.Text & "' "
            Case "CURRENT STATUS"
                DBDT.RowFilter = "STATUS = '" & txtgrdval.Text & "' "
            Case Else
                'Dim str As String = "[" & ddlPendinggrdHdr.SelectedItem.Text.ToString() & "]='" & txtPendinggrdval.Text & "'"
                DBDT.RowFilter = "[" & ddlGrdHdr.SelectedItem.Text.ToString() & "]='" & txtgrdval.Text & "'"
        End Select
        Dim datafields As DataTable = DBDT.Table.DefaultView.ToTable()
        gvAction.DataSource = datafields
        gvAction.DataBind()
    End Sub

    ' for page index change in gvPending
    Protected Sub gvPending_PageIndexChanging(sender As Object, e As GridViewPageEventArgs) Handles gvPending.PageIndexChanging
        gvPending.PageIndex = e.NewPageIndex
        Dim qry As String = Convert.ToString(ViewState("pendingqry")).ToUpper
        Dim passconval As String = Nothing
        Dim passconvalSorting As String = Nothing
        Dim searchalias As String() = qry.Split(",")
        For i As Integer = 0 To searchalias.Length - 1
            If searchalias(i).Contains("[" & ddlPendinggrdHdr.SelectedItem.Text.ToUpper() & "]") Then
                Dim exactpart As String = Convert.ToString(searchalias(i)).Replace("[" & ddlPendinggrdHdr.SelectedItem.Text.ToUpper() & "]", "")
                exactpart = exactpart.ToUpper().Replace("AS", "")
                qry = qry & "  and " & Convert.ToString(exactpart) & " like '%" & txtPendinggrdval.Text & "%'"
                passconval = "  and " & Convert.ToString(exactpart) & " like '%" & txtPendinggrdval.Text & "%'"
                Exit For
            End If
        Next
        PendingGrdBind(passconval)
        'PendingGrdBind()

    End Sub

    ' for page index change in Myrequest
    Protected Sub gvMyUpload_PageIndexChanging(ByVal sender As Object, ByVal e As GridViewPageEventArgs) Handles gvMyUpload.PageIndexChanging
        gvMyUpload.PageIndex = e.NewPageIndex
        MyRequestBind()
    End Sub
    ' for page index change in History
    Protected Sub gvAction_PageIndexChanging(ByVal sender As Object, ByVal e As GridViewPageEventArgs) Handles gvAction.PageIndexChanging
        gvAction.PageIndex = e.NewPageIndex
        MyActionGRdBind()
    End Sub
#Region "UserRole"
    <System.Web.Services.WebMethod()>
    Shared Function GetCurDocUserRole(ByVal DOCID As String, ByVal CURUSERROLE As String) As String
        Dim ret As String = ""
        Dim objDC As New DataClass()
        Dim objDT As DataTable
        Try
            objDT = objDC.ExecuteQryDT("select curstatus,EID from mmm_mst_doc where tid=" & DOCID & " and DocumentType='Vendor Invoice VP'")
            If objDT.Rows.Count > 0 Then
                Select Case objDT.Rows(0)(0).ToString().ToUpper
                    Case "APPROVER 1"
                        HttpContext.Current.Session("USERROLE") = "L1_APPROVER"
                    Case "APPROVER 2"
                        HttpContext.Current.Session("USERROLE") = "L2_APPROVER"
                    Case "APPROVER 3"
                        HttpContext.Current.Session("USERROLE") = "L3_APPROVER"
                    Case "APPROVER 4"
                        HttpContext.Current.Session("USERROLE") = "L4_APPROVER"
                End Select
            Else
                HttpContext.Current.Session("USERROLE") = CURUSERROLE
            End If
            ret = HttpContext.Current.Session("USERROLE")
        Catch ex As Exception
        End Try
        Return ret
    End Function
#End Region
    ' for page index change in Draft
    Protected Sub gvDraft_PageIndexChanging(ByVal sender As Object, ByVal e As GridViewPageEventArgs) Handles gvDraft.PageIndexChanging
        gvDraft.PageIndex = e.NewPageIndex
        MyDRAFTGRdBind()
    End Sub


    ' apply link for pending click docdetail page 
    Protected Sub gvPending_RowDataBound(sender As Object, e As GridViewRowEventArgs) Handles gvPending.RowDataBound
        'If e.Row.RowType = DataControlRowType.Header Then
        '    e.Row.Cells(6).Visible = False
        'End If
        If e.Row.RowType = DataControlRowType.DataRow Then
            'e.Row.Cells(1).Attributes.Add("onclick", "javascript:return OpenWindow('DocDetail.aspx?DOCID=" & gvPending.DataKeys(e.Row.RowIndex).Value & "')")
            e.Row.Cells(1).Attributes.Add("onclick", "javascript:return OpenWindow('DocDetail.aspx?DOCID=" & gvPending.DataKeys(e.Row.RowIndex).Value & "')")
            e.Row.Cells(1).Attributes("style") = "cursor:pointer"
            e.Row.Cells(1).Attributes("class") = "tdhover"
            Dim ViewDetail As New ImageButton()
            ViewDetail.ImageUrl = "~/images/ViewImage.png"
            ViewDetail.ID = "btnView"
            e.Row.Cells(1).Width = "80"
            e.Row.Cells(1).Controls.Add(ViewDetail)
            Dim text As String = ""
            text = e.Row.Cells(1).Text
            'e.Row.Attributes.Add("onclick", "javascript:OpenWindow('DocDetail.aspx?DOCID=" & gvPending.DataKeys(e.Row.RowIndex).Value & "')")
            'e.Row.Attributes("style") = "cursor:pointer"
            Dim approve As New ImageButton()
            approve.ImageUrl = "~/images/approve.png"
            approve.ID = "btnApprove"
            Dim reject As New ImageButton()
            reject.ImageUrl = "~/images/reject.png"
            reject.ID = "btnReject"
            Dim reconsider As New ImageButton()
            reconsider.ImageUrl = "~/images/reconsider.png"
            reconsider.ID = "btnReconsider"
            e.Row.Cells(0).Width = "80"
            e.Row.Cells(0).Controls.Add(approve)
            e.Row.Cells(0).Controls.Add(reject)
            e.Row.Cells(0).Controls.Add(reconsider)

            For c As Integer = 0 To gvPending.Columns.Count - 2
                If gvPending.Columns(c).HeaderText.ToUpper = "PRIORITY" Then
                    Dim Pid As Integer = Convert.ToInt32(DataBinder.Eval(e.Row.DataItem, "Priority"))
                    Select Case Pid
                        Case "1"
                            e.Row.BackColor = System.Drawing.Color.Yellow
                            Exit Select
                    End Select
                End If
                Dim lbl As New Label()
                lbl.Text = e.Row.DataItem(c).ToString()
                If c = 0 Then
                    lbl.Visible = False
                End If
                e.Row.Cells(c + 1).Controls.Add(lbl)
                e.Row.Cells(c + 1).HorizontalAlign = HorizontalAlign.Center
            Next

        End If
    End Sub

    ' apply link for  docdetail page 
    Protected Sub gvMyUpload_RowDataBound(sender As Object, e As GridViewRowEventArgs) Handles gvMyUpload.RowDataBound
        If e.Row.RowType = DataControlRowType.Header Then
            e.Row.Cells(0).Text = "View Details"
        End If
        If e.Row.RowType = DataControlRowType.DataRow Then
            e.Row.Attributes.Add("onclick", "javascript:return OpenWindow('DocDetail.aspx?DOCID=" & gvMyUpload.DataKeys(e.Row.RowIndex).Value & "')")
            e.Row.Attributes("style") = "cursor:pointer"
            Dim ViewDetail As New ImageButton()
            ViewDetail.ImageUrl = "~/images/ViewImage.png"
            ViewDetail.ID = "btnViewUpload"
            e.Row.Cells(0).Width = "80"
            e.Row.Cells(0).Controls.Add(ViewDetail)
            For c As Integer = 0 To gvMyUpload.Columns.Count - 2
                Dim lbl As New Label()
                lbl.Text = e.Row.DataItem(c).ToString()
                If c = 0 Then
                    lbl.Visible = False
                    e.Row.Cells(c).Controls.Add(lbl)
                    e.Row.Cells(c).HorizontalAlign = HorizontalAlign.Center
                End If
            Next
        End If
    End Sub
    ' apply link for docdetail page 
    Protected Sub gvAction_RowDataBound(sender As Object, e As GridViewRowEventArgs) Handles gvAction.RowDataBound
        If e.Row.RowType = DataControlRowType.Header Then
            e.Row.Cells(0).Text = "View Details"
        End If
        If e.Row.RowType = DataControlRowType.DataRow Then
            e.Row.Attributes.Add("onclick", "javascript:return OpenWindow('DocDetail.aspx?DOCID=" & gvAction.DataKeys(e.Row.RowIndex).Value & "')")
            e.Row.Attributes("style") = "cursor:pointer"
            Dim ViewDetail As New ImageButton()
            ViewDetail.ImageUrl = "~/images/ViewImage.png"
            ViewDetail.ID = "btnViewAction"
            e.Row.Cells(0).Width = "80"
            e.Row.Cells(0).Controls.Add(ViewDetail)
            For c As Integer = 0 To gvAction.Columns.Count - 2
                Dim lbl As New Label()
                lbl.Text = e.Row.DataItem(c).ToString()
                If c = 0 Then
                    lbl.Visible = False
                    e.Row.Cells(c).Controls.Add(lbl)
                    e.Row.Cells(c).HorizontalAlign = HorizontalAlign.Center
                End If
            Next
        End If
    End Sub

    ' apply link for docdetail page 
    Protected Sub gvDraft_RowDataBound(sender As Object, e As GridViewRowEventArgs) Handles gvDraft.RowDataBound
        'If e.Row.RowType = DataControlRowType.DataRow Then
        '    e.Row.Attributes.Add("onclick", "javascript:OpenWindow('DocDetail.aspx?DOCID=" & gvDraft.DataKeys(e.Row.RowIndex).Value & "')")
        '    e.Row.Attributes("style") = "cursor:pointer"
        'End If
    End Sub
#End Region


    Private Sub FillSubordinates(ByVal Luid As Integer)
        '' by sp for getting subordinates of logged user recursively
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim od As New SqlDataAdapter("", con)
        Try
            'With(nolock) added by Himank on 29th sep 2015
            Dim str1 As String = "SELECT SUBSTRING((SELECT ',' + fieldmapping  FROM MMM_mst_fields   WITH(NOLOCK)   where EID=" & Session("EID").ToString() & " and documenttype='USER' AND isnull(isSuperVisor,0)=1  ORDER BY displayorder FOR XML PATH('')),2,1000) AS CSV"
            Dim dt As New DataTable
            od.SelectCommand.CommandText = str1
            od.Fill(dt)
            Dim flds As String = ""
            If dt.Rows.Count <> 0 Then
                flds = dt.Rows(0).Item(0).ToString
            End If

            Dim ResAll As String = ""
            Dim dtU As New DataTable
            Dim str As String = ""
            Dim counter As Boolean = True
            If flds <> "" Then
                'With(nolock) added by Himank on 29th sep 2015
                str = "select uid," & flds & " from mmm_mst_user   WITH(NOLOCK)  where eid='" & Session("EID").ToString() & "' and userrole<>'SU'"
                od.SelectCommand.CommandText = str
                od.Fill(dtU)

                Dim res As String = Luid.ToString
                If dtU.Rows.Count > 0 Then
                    Do While counter
                        res = GetSubordinateUsers(res, dtU)
                        If res = "" Then
                            counter = False
                        Else
                            ResAll &= res
                        End If
                    Loop
                End If
            End If

            If ResAll <> "" Then
                If Right(ResAll, 1) = "," Then
                    ResAll = Left(ResAll, ResAll.Length - 1).ToString
                End If
                Session("SUBUID") = ResAll
            End If
            dt.Dispose()
            dtU.Dispose()
        Catch ex As Exception
        Finally
            con.Close()
            con.Dispose()
            od.Dispose()
        End Try

    End Sub


    Private Function GetSubordinateUsers(ByVal uInp As String, ByVal dt As DataTable) As String
        Dim str As String = ""
        Dim Uids As String = ""
        If uInp.ToString <> "" And Right(uInp, 1) = "," Then
            uInp = Left(uInp, uInp.Length - 1).ToString
        End If

        Dim uArr() As String = uInp.Split(",")
        For k As Integer = 0 To uArr.Length - 1
            For i As Integer = 0 To dt.Columns.Count - 1
                If dt.Columns(i).ColumnName.ToUpper <> "UID" Then
                    If uArr(k).ToString <> "" Then
                        Dim DR() As DataRow = dt.Select(dt.Columns(i).ColumnName & "='" & uArr(k) & "'")
                        For aa As Integer = 0 To DR.Length - 1
                            If Session("UID").ToString() = DR(aa).Item("UID").ToString() Then
                            Else
                                Uids &= DR(aa).Item("UID").ToString & ","
                            End If
                        Next
                        DR = Nothing
                    End If
                End If
            Next
        Next
        Return Uids
    End Function



    'Private Function GetSubordinateUsers(ByVal uInp As String, ByVal dt As DataTable) As String
    '    Dim str As String = ""
    '    Dim Uids As String = ""
    '    If uInp.ToString <> "" And Right(uInp, 1) = "," Then
    '        uInp = Left(uInp, uInp.Length - 1).ToString
    '    End If

    '    Dim uArr() As String = uInp.Split(",")
    '    For k As Integer = 0 To uArr.Length - 1
    '        For i As Integer = 0 To dt.Columns.Count - 1
    '            If dt.Columns(i).ColumnName.ToUpper <> "UID" Then
    '                If uArr(k).ToString <> "" Then
    '                    Dim DR() As DataRow = dt.Select(dt.Columns(i).ColumnName & "='" & uArr(k) & "'")
    '                    For aa As Integer = 0 To DR.Length - 1
    '                        Uids &= DR(aa).Item("UID").ToString & ","
    '                    Next
    '                    DR = Nothing
    '                End If
    '            End If
    '        Next
    '    Next
    '    Return Uids
    'End Function


    Public Sub PassExpMsgAlert()

        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim Passexp As Integer
        Dim PassExpMsgDays As Integer
        Dim Modifydate As String = ""
        Dim days As Integer
        Dim daydiff As Integer
        Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
        Try
            'With(nolock) added by Himank on 29th sep 2015
            oda.SelectCommand.CommandText = "select * from mmm_mst_entity   WITH(NOLOCK)  where eid='" & Session("EID").ToString & "'"
            oda.SelectCommand.CommandType = CommandType.Text
            Dim ds As New DataSet()
            oda.Fill(ds, "data")
            If ds.Tables("data").Rows.Count > 0 Then
                Passexp = ds.Tables("data").Rows(0).Item("passExpDays").ToString
                PassExpMsgDays = ds.Tables("data").Rows(0).Item("PassExpMsgDays").ToString
                daydiff = CInt(Passexp - PassExpMsgDays)
            End If
            'With(nolock) added by Himank on 29th sep 2015
            oda.SelectCommand.CommandText = "select * from mmm_mst_user  WITH(NOLOCK)   where  EID=" & Session("EID") & " and uid=" & Session("UID") & ""
            oda.SelectCommand.CommandType = CommandType.Text
            oda.Fill(ds, "data1")
            If ds.Tables("data1").Rows.Count > 0 Then
                Modifydate = ds.Tables("data1").Rows(0).Item("modifydate").ToString
                days = DateDiff(DateInterval.Day, CDate(Modifydate), Date.Now)
                If days >= daydiff Then
                    lblPassexpmsg.Text = " Dear User your password will expire in " & Passexp - days & " days. "
                    modalpopuppassexp.Show()
                Else
                    Exit Sub
                End If
            End If


        Catch ex As Exception
        Finally
            con.Close()
            con.Dispose()
            oda.Dispose()
        End Try
    End Sub



    Protected Sub btnYes_Click(sender As Object, e As System.EventArgs) Handles btnYes.Click
        Response.Redirect("Profile.aspx")
    End Sub
    'Commented by Ajeet on 10-october-2014 because new feature came into fashion
    'Private Sub BindSSOLinks()
    '    Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
    '    Dim con As SqlConnection = New SqlConnection(conStr)
    '    Dim od As New SqlDataAdapter("Select * from MMM_SSO_ZENDESK WHERE EID=" & Session("EID").ToString(), con)
    '    Try
    '        Dim dt As New DataTable
    '        od.Fill(dt)
    '        od.Dispose()
    '        con.Dispose()
    '        lnkZendesk.Visible = False
    '        lnkEW.Visible = False
    '        lnkPayroll.Visible = False
    '        lblAlert.Visible = False
    '        BindUseFullLinks()

    '        If dt.Rows.Count = 1 Then
    '            'If alert is there then show
    '            lblAlert.Visible = True
    '            lblAlert.Text = "Please click on Help Desk to create support ticket or email us at support@myndhrohd.zendesk.com<br/><br/>Helpdesk No: 0124-4724693<br/>"
    '            lnkZendesk.Text = dt.Rows(0).Item("DisplayName").ToString()
    '            lnkZendesk.Visible = True
    '        End If
    '    Catch ex As Exception
    '    Finally
    '        con.Close()
    '        con.Dispose()
    '        od.Dispose()
    '    End Try
    'End Sub

    'Private Sub BindUseFullLinks()
    '    Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
    '    Dim con As SqlConnection = New SqlConnection(conStr)
    '    Dim od As New SqlDataAdapter("Select * from MMM_MST_SU_LINKS WHERE LinkType='HOME' and EID=" & Session("EID").ToString() & " order by displayorder", con)
    '    Try
    '        Dim dt As New DataTable
    '        od.Fill(dt)
    '        od.Dispose()
    '        con.Dispose()

    '        lblUseFullLinks.Text = "<ul>"

    '        For i As Integer = 0 To dt.Rows.Count - 1
    '            lblUseFullLinks.Text &= "<li><a href=""" & dt.Rows(i).Item("url").ToString() & """>" & dt.Rows(i).Item("displayname").ToString() & "</a><li />"
    '        Next
    '        lblUseFullLinks.Text &= "</ul>"
    '    Catch ex As Exception
    '    Finally
    '        con.Close()
    '        con.Dispose()
    '        od.Dispose()
    '    End Try
    'End Sub

    Protected Sub SSOLink(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim ob As New MainUtility()
        Dim timestamp As Integer = ob.DateTimeToEpoch(DateTime.UtcNow)
        Dim name As String = Session("USERNAME")
        Dim email As String = Session("EMAIL")
        Dim ext_id As String = ""
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        'With(nolock) added by Himank on 29th sep 2015
        Dim od As New SqlDataAdapter("Select * from MMM_SSO_ZENDESK   WITH(NOLOCK)  WHERE EID=" & Session("EID").ToString(), con)
        Try
            Dim dt As New DataTable
            od.Fill(dt)
            If dt.Rows.Count = 1 Then
                Dim org As String = dt.Rows(0).Item("orgName").ToString()
                Dim token As String = dt.Rows(0).Item("zKey").ToString()
                Dim link As String = dt.Rows(0).Item("zendesklink").ToString()
                Dim hash As String = ob.Md5(name & email & ext_id & org & token & timestamp)
                Dim retUrl As String = link & "name=" & name & "&email=" & email & "&external_id=" & ext_id & "&organization=" & org & "&timestamp=" & timestamp & "&hash=" & hash
                ScriptManager.RegisterStartupScript(Page, GetType(Page), "OpenWindow", "window.open('" & retUrl & "');", True)
                'Response.Redirect(retUrl)
            End If
        Catch ex As Exception
        Finally
            od.Dispose()
            con.Close()
            con.Dispose()
        End Try

    End Sub


    Public Function IsBindDefaultDashBoard() As Boolean
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim od As New SqlDataAdapter("select count(*) from mmm_mst_needtoact_config where eid=" & Session("EID"), con)
        If con.State = ConnectionState.Closed Then
            con.Open()
        End If
        Dim intval As Integer = Convert.ToInt32(od.SelectCommand.ExecuteScalar())
        con.Close()
        Dim Flag As Boolean
        Flag = True
        If intval > 0 Then
            Flag = False
        End If
        Return Flag
    End Function



    '27 may commentted
    Public Sub BindDashBoard()
        ' last updated by sunil for removing archive docs frm history tab - 13-Dec-13
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)

        'Dim od As New SqlDataAdapter("Select top 20 M.tid,FName,Case oUID when " & Val(Session("UID").ToString()) & " then 'ME' else UserName end [UserName],adate,curstatus from MMM_MST_DOC M with (nolock)  LEFT OUTER JOIN MMM_DOC_DTL D with (nolock) on D.tid=M.lasttid  LEFT OUTER JOIN MMM_MST_USER ON uid=oUID where D.userid = " & Val(Session("UID").ToString()) & " order by M.Tid desc", con)
        Dim od As New SqlDataAdapter("Select  top 8 M.tid[SYSTEM ID], documenttype[SUBJECT],curstatus[STATUS],U.username[CREATED BY],(replace(convert(char(11),d.fdate,113),' ','-') + ' ' + left(convert(varchar(13),d.fdate,108),8))[RECEIVED ON],AP.Username [CURRENT USER],datediff(day,fdate,getdate())[PENDING DAYS],m.PRIORITY  from MMM_MST_DOC M with (nolock)  LEFT OUTER JOIN MMM_DOC_DTL D with (nolock) on D.tid=M.lasttid  LEFT OUTER JOIN MMM_MST_USER U with (nolock) ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP with (nolock) ON AP.uid=D.UserID where D.userid = " & Val(Session("UID").ToString()) & " order by M.priority desc,m.tid desc; select ShowStaticfieldonDashBoard,OrderOfDeshBoardFields from mmm_mst_entity where eid=" & Session("EID") & "", con)
        Try
            Dim ds As New DataSet
            od.Fill(ds, "pending")
            ViewState("pending") = ds.Tables("pending")
            'counting rows
            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            lblpending.Text = "(" & ds.Tables("pending").Rows.Count & ")"
            'Change for Search
            ViewState("pendingqry") = od.SelectCommand.CommandText
            'ScriptManager.RegisterStartupScript(Me.updPnlGrid, Me.updPnlGrid.GetType(), "Mayank", "$('#ContentPlaceHolder1_lblpending').html('" & ds.Tables("pending").Rows.Count & "');", True)
            'od.SelectCommand.CommandText = "Select distinct  top 20  M.tid,FName,Case oUID when " & Val(Session("UID").ToString()) & " then 'ME' else UserName end [UserName],adate,curstatus [curstatus] from MMM_MST_DOC M with (nolock)  LEFT OUTER JOIN MMM_DOC_DTL D with (nolock) on D.docid=M.tid LEFT OUTER JOIN MMM_MST_USER ON uid=oUID where D.userid = " & Val(Session("UID").ToString()) & " and aprstatus is not null order by M.Tid desc"
            ' od.SelectCommand.CommandText = "Select distinct  M.tid,documenttype,curstatus,U.username,adate ,AP.Username [apusername] ,datediff(day,fdate,getdate())[fdate] from MMM_MST_DOC M with (nolock)  LEFT OUTER JOIN MMM_DOC_DTL D with (nolock) on D.docid =M.tid  LEFT OUTER JOIN MMM_MST_USER U with (nolock) ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP with (nolock) ON AP.uid=D.UserID where D.UserID = " & Val(Session("UID").ToString()) & " and aprstatus is not null order by M.Tid desc"
            ' NEW for remove archive     
            od.SelectCommand.CommandText = "Select  top 8  M.tid[SYSTEM ID],documenttype[SUBJECT],curstatus[STATUS],U.username[CREATED BY],(replace(convert(char(11),d.fdate,113),' ','-') + ' ' + left(convert(varchar(13),d.fdate,108),8))[RECEIVED ON] ,AP.Username [CURRENT USER] ,datediff(day,fdate,getdate())[PENDING DAYS] from MMM_MST_DOC M with (nolock)  LEFT OUTER JOIN MMM_DOC_DTL D with (nolock) on D.docid =M.tid  LEFT OUTER JOIN MMM_MST_USER U with (nolock) ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP with (nolock) ON AP.uid=D.UserID where D.UserID = " & Val(Session("UID").ToString()) & " and aprstatus is not null and m.curstatus <> 'ARCHIVE'  order by M.Tid desc"

            od.Fill(ds, "action")
            ViewState("action") = ds.Tables("action")
            ViewState("myrequest") = ds.Tables("action")
            'counting rows
            'od.SelectCommand.CommandText = "Select count(*) from MMM_MST_DOC M with (nolock)  LEFT OUTER JOIN MMM_DOC_DTL D with (nolock) on D.docid =M.tid  LEFT OUTER JOIN MMM_MST_USER U with (nolock) ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP with (nolock) ON AP.uid=D.UserID where D.UserID = " & Val(Session("UID").ToString()) & " and aprstatus is not null "
            lbluploading.Text = "(" & ds.Tables("action").Rows.Count & ")"

            'od.SelectCommand.CommandText = "Select top 20  M.tid,FName,Case oUID when " & Val(Session("UID").ToString()) & " then 'ME' else UserName end [UserName],adate,aprstatus [curstatus] from MMM_MST_DOC M with (nolock)  LEFT OUTER JOIN MMM_DOC_DTL D with (nolock) on D.docid=M.tid LEFT OUTER JOIN MMM_MST_USER ON uid=oUID where D.userid = " & Val(Session("UID").ToString()) & " and aprstatus ='UPLOADED' order by M.Tid desc"
            od.SelectCommand.CommandText = "Select top 8  M.tid[SYSTEM ID],documenttype[SUBJECT],curstatus[STATUS],U.username[CREATED BY],(replace(convert(char(11),d.fdate,113),' ','-') + ' ' + left(convert(varchar(13),d.fdate,108),8))[RECEIVED ON] ,AP.Username [CURRENT USER] ,datediff(day,fdate,getdate())[PENDING DAYS] from MMM_MST_DOC M with (nolock)  LEFT OUTER JOIN MMM_DOC_DTL D with (nolock) on D.tid=M.lasttid  LEFT OUTER JOIN MMM_MST_USER U with (nolock) ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP with (nolock) ON AP.uid=D.UserID where M.oUid = " & Val(Session("UID").ToString()) & " and curstatus<> 'ARCHIVE' order by M.Tid desc"
            od.Fill(ds, "upload")
            ''counting rows
            ViewState("History") = ds.Tables("upload")
            lblaction.Text = "(" & ds.Tables("upload").Rows.Count & ")"
            ' '' my draft documents
            'od.SelectCommand.CommandText = "Select distinct M.tid,documenttype,curstatus,U.username,adate ,AP.Username [apusername] ,datediff(day,fdate,getdate())[fdate] from MMM_MST_DOC_DRAFT M  LEFT OUTER JOIN MMM_DOC_DTL D on D.tid=M.lasttid  LEFT OUTER JOIN MMM_MST_USER U ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP ON AP.uid=D.UserID where M.oUid = " & Val(Session("UID").ToString()) & " and curstatus<> 'ARCHIVE' order by M.Tid desc"
            od.SelectCommand.CommandText = "Select top 8  M.tid[SYSTEM ID],documenttype[SUBJECT],curstatus[STATUS],U.username[CREATED BY]  from MMM_MST_DOC_draft M with (nolock) LEFT OUTER JOIN MMM_MST_USER U with (nolock) ON U.uid=oUID  where m.ouID = " & Val(Session("UID").ToString()) & " and curstatus<> 'ARCHIVE' order by M.Tid desc "
            od.Fill(ds, "Mydraft")
            ViewState("MyDraft") = ds.Tables("Mydraft")
            gvDraft.DataSource = ds.Tables("Mydraft")
            gvDraft.DataBind()
            '    lblDraft.Text = "(" & ds.Tables("Mydraft").Rows.Count & ")"

            If ds.Tables("MyDraft").Rows.Count > 0 Then
                tabDraf.Attributes.Add("display", "block")
                lblDraft.Text = "<a href='#ContentPlaceHolder1_tabDraf'>My Draft " & "(" & ds.Tables("Mydraft").Rows.Count & ")" & "</a>"
            Else
                tabDraf.Attributes.Add("Style", "display:none;")
            End If

            ' gvPending.DataSource = ds.Tables("pending")
            'gvPending.DataBind()
            gvMyUpload.DataSource = ds.Tables("upload")
            gvMyUpload.DataBind()
            gvAction.DataSource = ds.Tables("action")
            gvAction.DataBind()
            BindDropdownoncondition(0)
            'Dim ShowStaticfieldonDashBoard As Integer = 0
            'Dim OrderOfDeshBoardFields As String = ""
            'If ds.Tables("pending1").Rows.Count > 0 Then
            '    ShowStaticfieldonDashBoard = Convert.ToInt32(ds.Tables("pending1").Rows(0)("ShowStaticfieldonDashBoard"))
            '    OrderOfDeshBoardFields = Convert.ToString(ds.Tables("pending1").Rows(0)("OrderOfDeshBoardFields"))
            '    If ShowStaticfieldonDashBoard = 1 Then
            '        If OrderOfDeshBoardFields.ToUpper = "STATIC" Then
            '            ' binding drop down for pending 
            '            ddlPendinggrdHdr.Items.Clear()
            '            ddlPendinggrdHdr.Items.Insert(0, "SELECT")
            '            ddlPendinggrdHdr.Items.Insert(1, "RECIEVED ON")
            '            ddlPendinggrdHdr.Items.Insert(2, "DOC ID")
            '            ddlPendinggrdHdr.Items.Insert(3, "CREATED BY")
            '            ddlPendinggrdHdr.Items.Insert(3, "CURRENT STATUS")
            '            ' binding drop down for MyRequest
            '            ddlMyReqHdr.Items.Clear()
            '            ddlMyReqHdr.Items.Insert(0, "SELECT")
            '            ddlMyReqHdr.Items.Insert(1, "RECIEVED ON")
            '            ddlMyReqHdr.Items.Insert(2, "DOC ID")
            '            ddlMyReqHdr.Items.Insert(3, "CREATED BY")
            '            ddlMyReqHdr.Items.Insert(3, "CURRENT STATUS")
            '            ' binding drop down for action
            '            ddlGrdHdr.Items.Clear()
            '            ddlGrdHdr.Items.Insert(0, "SELECT")
            '            ddlGrdHdr.Items.Insert(1, "RECIEVED ON")
            '            ddlGrdHdr.Items.Insert(2, "DOC ID")
            '            ddlGrdHdr.Items.Insert(3, "CREATED BY")
            '            ddlGrdHdr.Items.Insert(3, "CURRENT STATUS")

            '            'binding dropdown for draft
            '            ddlDraftHdr.Items.Clear()
            '            ddlDraftHdr.Items.Insert(0, "SELECT")
            '            'ddlDraftHdr.Items.Insert(1, "RECIEVED ON")
            '            ddlDraftHdr.Items.Insert(1, "DOC ID")
            '            ddlDraftHdr.Items.Insert(2, "CREATED BY")
            '            ddlDraftHdr.Items.Insert(3, "CURRENT STATUS")
            '        Else
            '            ddlPendinggrdHdr.Items.Clear()
            '            ddlPendinggrdHdr.Items.Insert(0, "SELECT")
            '            ddlPendinggrdHdr.Items.Insert(2, "DOC ID")

            '            ddlMyReqHdr.Items.Clear()
            '            ddlMyReqHdr.Items.Insert(0, "SELECT")
            '            ddlMyReqHdr.Items.Insert(2, "DOC ID")

            '            ddlGrdHdr.Items.Clear()
            '            ddlGrdHdr.Items.Insert(0, "SELECT")
            '            ddlGrdHdr.Items.Insert(2, "DOC ID")

            '            ddlDraftHdr.Items.Clear()
            '            ddlDraftHdr.Items.Insert(0, "SELECT")
            '            ddlDraftHdr.Items.Insert(1, "DOC ID")
            '        End If
            '    Else
            '        ddlPendinggrdHdr.Items.Clear()
            '        ddlPendinggrdHdr.Items.Insert(0, "SELECT")
            '        ddlPendinggrdHdr.Items.Insert(1, "DOC ID")

            '        ddlMyReqHdr.Items.Clear()
            '        ddlMyReqHdr.Items.Insert(0, "SELECT")
            '        ddlMyReqHdr.Items.Insert(1, "DOC ID")

            '        ddlGrdHdr.Items.Clear()
            '        ddlGrdHdr.Items.Insert(0, "SELECT")
            '        ddlGrdHdr.Items.Insert(1, "DOC ID")

            '        ddlDraftHdr.Items.Clear()
            '        ddlDraftHdr.Items.Insert(0, "SELECT")
            '        ddlDraftHdr.Items.Insert(1, "DOC ID")
            '    End If

            'End If

        Catch ex As Exception
        Finally
            con.Close()
            con.Dispose()
            od.Dispose()
        End Try

    End Sub

    Public Sub BindDashBoardonddl()
        ' last updated by sunil for removing archive docs frm history tab - 13-Dec-13
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)

        'Dim od As New SqlDataAdapter("Select top 20 M.tid,FName,Case oUID when " & Val(Session("UID").ToString()) & " then 'ME' else UserName end [UserName],adate,curstatus from MMM_MST_DOC M with (nolock)  LEFT OUTER JOIN MMM_DOC_DTL D with (nolock) on D.tid=M.lasttid  LEFT OUTER JOIN MMM_MST_USER ON uid=oUID where D.userid = " & Val(Session("UID").ToString()) & " order by M.Tid desc", con)
        Dim od As New SqlDataAdapter("Select  M.tid[SYSTEM ID],documenttype[SUBJECT],curstatus[STATUS],U.username[CREATED BY],(replace(convert(char(11),d.fdate,113),' ','-') + ' ' + left(convert(varchar(13),d.fdate,108),8))[RECEIVED ON],AP.Username [CURRENT USER],datediff(day,fdate,getdate())[PENDING DAYS],m.PRIORITY  from MMM_MST_DOC M with (nolock)  LEFT OUTER JOIN MMM_DOC_DTL D with (nolock) on D.tid=M.lasttid  LEFT OUTER JOIN MMM_MST_USER U with (nolock) ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP with (nolock) ON AP.uid=D.UserID where D.userid = " & Val(Session("UID").ToString()) & " order by M.priority desc,m.tid desc", con)
        Try
            Dim ds As New DataSet
            od.Fill(ds, "pending")
            ViewState("pending") = ds.Tables("pending")
            'counting rows
            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            lblpending.Text = "(" & ds.Tables("pending").Rows.Count & ")"
            'ScriptManager.RegisterStartupScript(Me.updPnlGrid, Me.updPnlGrid.GetType(), "Mayank", "$('#ContentPlaceHolder1_lblpending').html('" & ds.Tables("pending").Rows.Count & "');", True)
            'od.SelectCommand.CommandText = "Select distinct  top 20  M.tid,FName,Case oUID when " & Val(Session("UID").ToString()) & " then 'ME' else UserName end [UserName],adate,curstatus [curstatus] from MMM_MST_DOC M with (nolock)  LEFT OUTER JOIN MMM_DOC_DTL D with (nolock) on D.docid=M.tid LEFT OUTER JOIN MMM_MST_USER ON uid=oUID where D.userid = " & Val(Session("UID").ToString()) & " and aprstatus is not null order by M.Tid desc"
            ' od.SelectCommand.CommandText = "Select distinct  M.tid,documenttype,curstatus,U.username,adate ,AP.Username [apusername] ,datediff(day,fdate,getdate())[fdate] from MMM_MST_DOC M with (nolock)  LEFT OUTER JOIN MMM_DOC_DTL D with (nolock) on D.docid =M.tid  LEFT OUTER JOIN MMM_MST_USER U with (nolock) ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP with (nolock) ON AP.uid=D.UserID where D.UserID = " & Val(Session("UID").ToString()) & " and aprstatus is not null order by M.Tid desc"
            ' NEW for remove archive     
            od.SelectCommand.CommandText = "Select   M.tid[SYSTEM ID],documenttype[SUBJECT],curstatus[STATUS],U.username[CREATED BY],(replace(convert(char(11),d.fdate,113),' ','-') + ' ' + left(convert(varchar(13),d.fdate,108),8))[RECEIVED ON],AP.Username [CURRENT USER],datediff(day,fdate,getdate())[PENDING DAYS],m.PRIORITY  from MMM_MST_DOC M with (nolock)  LEFT OUTER JOIN MMM_DOC_DTL D with (nolock) on D.docid =M.tid  LEFT OUTER JOIN MMM_MST_USER U with (nolock) ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP with (nolock) ON AP.uid=D.UserID where D.UserID = " & Val(Session("UID").ToString()) & " and aprstatus is not null and m.curstatus <> 'ARCHIVE'  order by M.Tid desc"

            od.Fill(ds, "action")
            ViewState("action") = ds.Tables("action")

            'counting rows
            'od.SelectCommand.CommandText = "Select count(*) from MMM_MST_DOC M with (nolock)  LEFT OUTER JOIN MMM_DOC_DTL D with (nolock) on D.docid =M.tid  LEFT OUTER JOIN MMM_MST_USER U with (nolock) ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP with (nolock) ON AP.uid=D.UserID where D.UserID = " & Val(Session("UID").ToString()) & " and aprstatus is not null "
            lbluploading.Text = "(" & ds.Tables("action").Rows.Count & ")"

            'od.SelectCommand.CommandText = "Select top 20  M.tid,FName,Case oUID when " & Val(Session("UID").ToString()) & " then 'ME' else UserName end [UserName],adate,aprstatus [curstatus] from MMM_MST_DOC M with (nolock)  LEFT OUTER JOIN MMM_DOC_DTL D with (nolock) on D.docid=M.tid LEFT OUTER JOIN MMM_MST_USER ON uid=oUID where D.userid = " & Val(Session("UID").ToString()) & " and aprstatus ='UPLOADED' order by M.Tid desc"
            od.SelectCommand.CommandText = "Select  M.tid[SYSTEM ID],documenttype[SUBJECT],curstatus[STATUS],U.username[CREATED BY],(replace(convert(char(11),d.fdate,113),' ','-') + ' ' + left(convert(varchar(13),d.fdate,108),8))[RECEIVED ON],AP.Username [CURRENT USER],datediff(day,fdate,getdate())[PENDING DAYS],m.PRIORITY  from MMM_MST_DOC M with (nolock)  LEFT OUTER JOIN MMM_DOC_DTL D with (nolock) on D.tid=M.lasttid  LEFT OUTER JOIN MMM_MST_USER U with (nolock) ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP with (nolock) ON AP.uid=D.UserID where M.oUid = " & Val(Session("UID").ToString()) & " and curstatus<> 'ARCHIVE' order by M.Tid desc"
            od.Fill(ds, "upload")
            ''counting rows
            lblaction.Text = "(" & ds.Tables("upload").Rows.Count & ")"


            'od.SelectCommand.CommandText = "Select  M.tid[SYSTEM ID],documenttype[SUBJECT],curstatus[STATUS],U.username[CREATED BY]  from MMM_MST_DOC_draft M with (nolock)   LEFT OUTER JOIN MMM_MST_USER U with (nolock) ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP with (nolock) ON AP.uid=m.ouID where M.oUid = " & Val(Session("UID").ToString()) & " and curstatus<> 'ARCHIVE' order by M.Tid desc"
            od.SelectCommand.CommandText = "Select  M.tid[SYSTEM ID],documenttype[SUBJECT],curstatus[STATUS],U.username[CREATED BY]  from MMM_MST_DOC_draft M with (nolock)   LEFT OUTER JOIN MMM_MST_USER U with (nolock) ON U.uid=oUID where M.oUid = " & Val(Session("UID").ToString()) & " and curstatus<> 'ARCHIVE' order by M.Tid desc"
            od.Fill(ds, "MyDraft")
            ''counting rows
            lblDraft.Text = "(" & ds.Tables("MyDraft").Rows.Count & ")"


            'gvPending.DataSource = ds.Tables("pending")
            'gvPending.DataBind()
            gvMyUpload.DataSource = ds.Tables("upload")
            gvMyUpload.DataBind()
            gvAction.DataSource = ds.Tables("action")
            gvAction.DataBind()

            gvDraft.DataSource = ds.Tables("MyDraft")
            gvDraft.DataBind()


            ' binding drop down for pending 
            ddlPendinggrdHdr.Items.Clear()
            ddlPendinggrdHdr.Items.Insert(0, "SELECT")
            ddlPendinggrdHdr.Items.Insert(1, "RECEIVED ON")
            ddlPendinggrdHdr.Items.Insert(2, "SYSTEM ID")
            ddlPendinggrdHdr.Items.Insert(3, "CREATED BY")
            ddlPendinggrdHdr.Items.Insert(3, "STATUS")
            ' binding drop down for MyRequest
            ddlMyReqHdr.Items.Clear()
            ddlMyReqHdr.Items.Insert(0, "SELECT")
            ddlMyReqHdr.Items.Insert(1, "RECIEVED ON")
            ddlMyReqHdr.Items.Insert(2, "SYSTEM ID")
            ddlMyReqHdr.Items.Insert(3, "CREATED BY")
            ddlMyReqHdr.Items.Insert(3, "STATUS")
            ' binding drop down for action
            ddlGrdHdr.Items.Clear()
            ddlGrdHdr.Items.Insert(0, "SELECT")
            ddlGrdHdr.Items.Insert(1, "RECIEVED ON")
            ddlGrdHdr.Items.Insert(2, "SYSTEM ID")
            ddlGrdHdr.Items.Insert(3, "CREATED BY")
            ddlGrdHdr.Items.Insert(3, "STATUS")
            ' binding drop down for draft
            ddlDraftHdr.Items.Clear()
            ddlDraftHdr.Items.Insert(0, "SELECT")
            'ddlDraftHdr.Items.Insert(1, "RECIEVED ON")
            ddlDraftHdr.Items.Insert(1, "SYSTEM ID")
            ddlDraftHdr.Items.Insert(2, "CREATED BY")
            ddlDraftHdr.Items.Insert(3, "STATUS")

        Catch ex As Exception

        Finally
            con.Close()
            con.Dispose()
            od.Dispose()
        End Try

    End Sub

    Protected Sub RefreshPanel(ByVal sender As Object, ByVal e As EventArgs)
        ' BindDashBoard()
    End Sub


    Protected Sub ShowHit(ByVal sender As Object, ByVal e As EventArgs)
        'get the file name and server credentials
        Dim btnDetails As ImageButton = TryCast(sender, ImageButton)
        Dim row As GridViewRow = DirectCast(btnDetails.NamingContainer, GridViewRow)
        Dim pid As Integer = Convert.ToString(Me.gvAction.DataKeys(row.RowIndex).Value)

        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        'With(nolock) added by Himank on 29th sep 2015
        Dim da As SqlDataAdapter = New SqlDataAdapter("SELECT fname,docurl  from MMM_MST_DOC  WITH(NOLOCK)   where TID=" & pid, con)
        Try
            Dim ds As New DataSet()
            da.Fill(ds, "data")

            If ds.Tables("data").Rows.Count <> 1 Then
                Exit Sub
            End If
            Dim docURl As String = ds.Tables("data").Rows(0).Item("docurl").ToString()
            Dim sb As StringBuilder = New StringBuilder("")
            Dim strRoot As String
            strRoot = Request.Url.GetLeftPart(UriPartial.Authority)
            sb.Append("window.open('" & strRoot & "/DOCS/" & docURl & "');")
            'sb.Append("window.open('https://Mynd SaaS.myndsolution.com/DOCS/" & docURl & "');")
            ScriptManager.RegisterClientScriptBlock(Me.updPnlGrid, Me.updPnlGrid.GetType(), "NewClientScript", sb.ToString(), True)
        Catch ex As Exception
        Finally
            con.Close()
            con.Dispose()
            da.Dispose()
        End Try

    End Sub

    'Protected Sub FillHolidayDataset()
    '    Dim firstDate As New DateTime(Calendar1.VisibleDate.Year, Calendar1.VisibleDate.Month, 1)
    '    Dim lastDate As DateTime = GetFirstDayOfNextMonth()
    '    dsHolidays = GetCurrentMonthData(firstDate, lastDate)
    'End Sub

    'Protected Function GetFirstDayOfNextMonth() As DateTime
    '    Dim monthNumber, yearNumber As Integer
    '    If Calendar1.VisibleDate.Month = 12 Then
    '        monthNumber = 1
    '        yearNumber = Calendar1.VisibleDate.Year + 1
    '    Else
    '        monthNumber = Calendar1.VisibleDate.Month + 1
    '        yearNumber = Calendar1.VisibleDate.Year
    '    End If
    '    Dim lastDate As New DateTime(yearNumber, monthNumber, 1)
    '    Return lastDate
    'End Function

    'Protected Sub Calendar1_SelectionChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles Calendar1.SelectionChanged
    '    Dim start As String
    '    Dim est As String

    '    Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
    '    Dim con As SqlConnection = New SqlConnection(conStr)
    '    Dim sda As SqlDataAdapter = New SqlDataAdapter("select workingStartTime,workingEndTime from MMM_MST_USER where uid='" & Session("UID").ToString() & "' ", con)
    '    sda.SelectCommand.CommandType = CommandType.Text
    '    Dim ds As New DataSet()
    '    sda.Fill(ds, "data")
    '    Dim add As Integer

    '    start = Right(Trim(ds.Tables("data").Rows(0).Item("workingStartTime").ToString()), 2)
    '    est = Right(Trim(ds.Tables("data").Rows(0).Item("workingEndTime").ToString()), 2)

    '    ViewState("AM") = start
    '    ViewState("PM") = est

    '    'If ViewState("wst").ToString() > ViewState("wet").ToString() Then

    '    '    ViewState("wet") = CInt(ViewState("wst")) + CInt(ViewState("wet"))

    '    'End If

    '    Dim dt As New DataTable("mytable")
    '    dt.Columns.Add("Time", GetType(String))
    '    dt.Columns.Add("Task", GetType(String))

    '    For i As Integer = 0 To 23
    '        Dim dr As DataRow
    '        dr = dt.NewRow
    '        dr(0) = i.ToString("00") & " : 00"
    '        dr(1) = ""
    '        dt.Rows.Add(dr)
    '    Next

    '    grdTask.DataSource = dt
    '    grdTask.DataBind()

    '    If start = "PM" And est = "AM" Then
    '        add = 12 - (Left(ds.Tables("data").Rows(0).Item("workingStartTime").ToString(), 2))
    '        'add = add + (Left(ds.Tables("data").Rows(0).Item("workingEndTime").ToString(), 2))
    '        ViewState("wst") = 12 + (Left(ds.Tables("data").Rows(0).Item("workingStartTime").ToString(), 2))
    '        ViewState("wet") = (Left(ds.Tables("data").Rows(0).Item("workingEndTime").ToString(), 2))

    '        For i As Integer = 0 To grdTask.Rows.Count - 1
    '            If i >= Val(ViewState("wst")) Then
    '                grdTask.Rows(i).BackColor = Drawing.Color.Gray
    '            End If
    '            If i <= Val(ViewState("wet")) Then
    '                grdTask.Rows(i).BackColor = Drawing.Color.Gray
    '            End If
    '        Next

    '    ElseIf start = "AM" And est = "AM" Then
    '        ViewState("wst") = (Left(ds.Tables("data").Rows(0).Item("workingStartTime").ToString(), 2))
    '        ViewState("wet") = (Left(ds.Tables("data").Rows(0).Item("workingEndTime").ToString(), 2))
    '        For i As Integer = 0 To grdTask.Rows.Count - 1
    '            If i >= Val(ViewState("wst")) And i <= Val(ViewState("wet")) Then
    '                grdTask.Rows(i).BackColor = Drawing.Color.Gray
    '            End If
    '        Next

    '    ElseIf start = "PM" And est = "PM" Then

    '        ViewState("wst") = 12 + (Left(ds.Tables("data").Rows(0).Item("workingStartTime").ToString(), 2))
    '        ViewState("wet") = 12 + (Left(ds.Tables("data").Rows(0).Item("workingEndTime").ToString(), 2))
    '        For i As Integer = 0 To grdTask.Rows.Count - 1
    '            If i >= Val(ViewState("wst")) And i <= Val(ViewState("wet")) Then
    '                grdTask.Rows(i).BackColor = Drawing.Color.Gray
    '            End If
    '        Next

    '    ElseIf start = "AM" And est = "PM" Then

    '        add = 12 - (Left(ds.Tables("data").Rows(0).Item("workingStartTime").ToString(), 2))
    '        add = add + (Left(ds.Tables("data").Rows(0).Item("workingEndTime").ToString(), 2))

    '        ViewState("wet") = add + (Left(ds.Tables("data").Rows(0).Item("workingStartTime").ToString(), 2))
    '        ViewState("wst") = Left(ds.Tables("data").Rows(0).Item("workingStartTime").ToString(), 2)

    '        For i As Integer = 0 To grdTask.Rows.Count - 1
    '            If i >= Val(ViewState("wst")) And i <= Val(ViewState("wet")) Then
    '                grdTask.Rows(i).BackColor = Drawing.Color.Gray
    '            End If
    '        Next
    '    Else
    '        ViewState("wst") = Left(ds.Tables("data").Rows(0).Item("workingStartTime").ToString(), 2)
    '        ViewState("wet") = Left(ds.Tables("data").Rows(0).Item("workingEndTime").ToString(), 2)
    '        For i As Integer = 0 To grdTask.Rows.Count - 1
    '            If Val(ViewState("wst")) < Val(ViewState("wet")) Then
    '                If i >= Val(ViewState("wst")) And i <= Val(ViewState("wet")) Then
    '                    grdTask.Rows(i).BackColor = Drawing.Color.Gray
    '                End If
    '            Else
    '                If i >= Val(ViewState("wst")) Then
    '                    grdTask.Rows(i).BackColor = Drawing.Color.Gray
    '                End If
    '                If i <= Val(ViewState("wet")) Then
    '                    grdTask.Rows(i).BackColor = Drawing.Color.Gray
    '                End If
    '            End If
    '        Next
    '    End If
    '    updTaskPanel.Update()
    '    btncalendar_modalPopupExtender.Show()
    'End Sub

    'Protected Sub Calendar1_VisibleMonthChanged(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.MonthChangedEventArgs) Handles Calendar1.VisibleMonthChanged
    '    FillHolidayDataset()
    'End Sub

    'Function GetCurrentMonthData(ByVal firstDate As DateTime, ByVal lastDate As DateTime) As DataSet
    '    Dim dsMonth As New DataSet
    '    Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
    '    Dim con As SqlConnection = New SqlConnection(conStr)
    '    Dim oda As SqlDataAdapter = New SqlDataAdapter("select * from MMM_MST_HOLIDAYS WHERE HoliDate >= @firstDate AND HoliDate < @lastDate and locid=" & Session("LID").ToString(), con)
    '    oda.SelectCommand.Parameters.Add(New SqlParameter("@firstDate", firstDate))
    '    oda.SelectCommand.Parameters.Add(New SqlParameter("@lastDate", lastDate))
    '    oda.Fill(dsMonth, "holidays")

    '    'For weekly Off
    '    oda.SelectCommand.CommandText = "select weeklyHoliday from mmm_mst_user where uid=" & Session("UID").ToString()
    '    oda.SelectCommand.Parameters.Clear()
    '    oda.Fill(dsMonth, "woff")

    '    oda.SelectCommand.CommandText = "getCurrentAppTask"
    '    oda.SelectCommand.Parameters.Clear()
    '    oda.SelectCommand.CommandType = CommandType.StoredProcedure
    '    oda.SelectCommand.Parameters.AddWithValue("@uid", Session("UID").ToString())
    '    oda.SelectCommand.Parameters.AddWithValue("@firstDate", firstDate)
    '    oda.SelectCommand.Parameters.AddWithValue("@lastDate", lastDate)
    '    oda.Fill(dsMonth, "aprTask")
    '    oda.Dispose()
    '    con.Dispose()
    '    Return dsMonth
    'End Function

    'Protected Sub Calendar1_DayRender(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.DayRenderEventArgs) Handles Calendar1.DayRender
    '    If Not dsHolidays Is Nothing Then
    '        If e.Day.Date >= New DateTime(Calendar1.VisibleDate.Year, Calendar1.VisibleDate.Month, 1) And e.Day.Date <= New DateTime(Calendar1.VisibleDate.Year, Calendar1.VisibleDate.Month, DateTime.DaysInMonth(Calendar1.VisibleDate.Year, Calendar1.VisibleDate.Month)) Then
    '            e.Day.IsSelectable = True
    '            'For weekly off
    '            Dim woff As String = dsHolidays.Tables("woff").Rows(0).Item("weeklyHoliday").ToString
    '            If InStr(woff, Weekday(e.Day.Date, Microsoft.VisualBasic.FirstDayOfWeek.Monday)) > 0 Then
    '                e.Cell.BackColor = Drawing.Color.LightGray
    '                e.Cell.ToolTip = "Weekly Holiday"
    '            End If

    '            Dim nextDate As DateTime
    '            For Each dr As DataRow In dsHolidays.Tables("holidays").Rows
    '                nextDate = CType(dr("HoliDate"), DateTime)
    '                If nextDate = e.Day.Date Then
    '                    e.Cell.BackColor = System.Drawing.Color.LemonChiffon
    '                    e.Cell.ToolTip = dr.Item("holidayname").ToString()
    '                End If
    '            Next


    '            For Each dr As DataRow In dsHolidays.Tables("aprTask").Rows
    '                If Val(dr("dt").ToString()) = e.Day.Date.Day Then
    '                    e.Cell.BackColor = System.Drawing.Color.LightGoldenrodYellow
    '                    e.Cell.ToolTip = dr.Item("curstatus").ToString()

    '                    Dim lt As New LiteralControl()
    '                    lt.Text = "<br /> " & dr("curstatus") & "(" & dr("cnt") & ")"
    '                    e.Cell.Controls.Add(lt)

    '                    'Dim ln As New LinkButton()
    '                    'ln.Text = "<br />"
    '                    ' AddHandler ln.Click, AddressOf ShowDayDetail
    '                    ' e.Cell.Controls.Add(ln)
    '                End If
    '            Next
    '        Else
    '            e.Day.IsSelectable = False
    '            e.Cell.BackColor = Drawing.Color.LightCyan
    '        End If
    '    End If
    'End Sub
    Private Const ASCENDING As String = " ASC"
    Private Const DESCENDING As String = " DESC"

    'Public Property GridViewSortDirection As SortDirection
    '    Get
    '        If Val(ViewState("sortDirection")) = Val(DBNull.Value.ToString) Then
    '            ViewState("sortDirection") = SortDirection.Ascending

    '        End If
    '        Return CType(ViewState("sortDirection"), SortDirection)

    '    End Get
    '    Set(ByVal value As SortDirection)
    '        ViewState("sortDirection") = value
    '    End Set
    'End Property

    'Protected Sub GridView1_PageIndexChanging(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewPageEventArgs) Handles gvMyUpload.PageIndexChanging
    '    Try
    '        gvMyUpload.PageIndex = e.NewPageIndex
    '        currentPageNumberu = e.NewPageIndex + 1
    '        'Session("Index") = e.NewPageIndex
    '        Session("Grid") = "gvMyUpload"
    '        Session("PageIndex") = gvMyUpload.PageIndex
    '        ViewState("piU") = gvMyUpload.PageIndex + 1

    '        '  BindDashBoard()  ' 27 may
    '        'bindMyReqGrid()
    '    Catch ex As Exception
    '    End Try
    'End Sub

    '27 may
    'Protected Sub GridView3_PageIndexChanging(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewPageEventArgs) Handles gvPending.PageIndexChanging
    '    Try
    '        gvPending.PageIndex = e.NewPageIndex
    '        currentPageNumberp = e.NewPageIndex + 1
    '        ' Session("Index") = e.NewPageIndex
    '        Session("PageIndex") = gvPending.PageIndex
    '        ViewState("piP") = gvPending.PageIndex + 1
    '        Session("Grid") = "gvPending"
    '        bindNeedActGrid()
    '        BindDashBoard()
    '    Catch ex As Exception
    '    End Try
    'End Sub

    '    27 may
    'Protected Sub gvPending_Sorting(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewSortEventArgs) Handles gvPending.Sorting
    '    Dim sortExpression As String = e.SortExpression

    '    If (GridViewSortDirection = SortDirection.Ascending) Then

    '        GridViewSortDirection = SortDirection.Descending

    '        SortGridView(sortExpression, DESCENDING)

    '    Else

    '        GridViewSortDirection = SortDirection.Ascending
    '        SortGridView(sortExpression, ASCENDING)


    '    End If
    'End Sub

    '27 may

    'Private Sub SortGridView(ByVal sortExpression As String, ByVal direction As String)
    '    'You can cache the DataTable for improving performance
    '    Dim dt As DataTable = CType(ViewState("pending"), DataTable)

    '    dt.DefaultView.Sort = sortExpression + direction
    '    Dim dt1 As DataTable = dt.DefaultView.ToTable()
    '    gvPending.DataSource = ViewState("pending")
    '    gvPending.DataBind()
    '    'ViewState("data") = dt1
    'End Sub

    '27 may
    'Protected Sub GridView2_PageIndexChanging(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewPageEventArgs) Handles gvAction.PageIndexChanging
    '    Try
    '        gvAction.PageIndex = e.NewPageIndex
    '        currentPageNumber = e.NewPageIndex + 1
    '        'Session("Index") = e.NewPageIndex
    '        Session("PageIndex") = gvAction.PageIndex
    '        ViewState("piA") = gvAction.PageIndex + 1
    '        Session("Grid") = "gvAction"
    '        BindDashBoard()
    '        bindFilteGrid()

    '    Catch ex As Exception
    '    End Try
    'End Sub

    '27 may
    'Protected Sub gvAction_Sorting(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewSortEventArgs) Handles gvAction.Sorting
    '    Dim sortExpression As String = e.SortExpression
    '    If (GridViewSortDirection = SortDirection.Ascending) Then
    '        GridViewSortDirection = SortDirection.Descending
    '        SortGridViewa(sortExpression, DESCENDING)
    '    Else
    '        GridViewSortDirection = SortDirection.Ascending
    '        SortGridViewa(sortExpression, ASCENDING)
    '    End If
    'End Sub

    '27 may
    'Private Sub SortGridViewa(ByVal sortExpression As String, ByVal direction As String)
    '    'You can cache the DataTable for improving performance
    '    Dim dt As DataTable = CType(ViewState("action"), DataTable)

    '    dt.DefaultView.Sort = sortExpression + direction
    '    Dim dt1 As DataTable = dt.DefaultView.ToTable()
    '    gvAction.DataSource = ViewState("action")
    '    bindFilteGrid()
    '    gvAction.DataBind()
    '    'ViewState("data") = dt1
    'End Sub


    'Protected Sub gvMyUpload_PageIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles gvMyUpload.PageIndexChanged

    'End Sub

    'Protected Sub GridView1_PageIndexChanging(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewPageEventArgs) Handles gvMyUpload.PageIndexChanging
    '    Try
    '        gvMyUpload.PageIndex = e.NewPageIndex
    '        BindDashBoard()
    '    Catch ex As Exception
    '    End Try
    'End Sub

    'Protected Sub gvMyUpload_Sorting(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewSortEventArgs) Handles gvMyUpload.Sorting
    '    Dim sortExpression As String = e.SortExpression
    '    If (GridViewSortDirection = SortDirection.Ascending) Then
    '        GridViewSortDirection = SortDirection.Descending
    '        SortGridViewu(sortExpression, DESCENDING)
    '    Else
    '        GridViewSortDirection = SortDirection.Ascending
    '        SortGridViewu(sortExpression, ASCENDING)
    '    End If
    'End Sub

    'Private Sub SortGridViewu(ByVal sortExpression As String, ByVal direction As String)
    '    'You can cache the DataTable for improving performance
    '    Dim dt As DataTable = CType(ViewState("upload"), DataTable)

    '    dt.DefaultView.Sort = sortExpression + direction
    '    Dim dt1 As DataTable = dt.DefaultView.ToTable()
    '    gvMyUpload.DataSource = ViewState("upload")
    '    gvMyUpload.DataBind()
    '    'ViewState("data") = dt1
    'End Sub


    'Custom paging on GridView functionality

    Protected currentPageNumber As Integer = 1
    Protected currentPageNumberu As Integer = 1
    Protected currentPageNumberp As Integer = 1

    '27 may
    'Protected Sub ChangePage(ByVal sender As Object, ByVal e As CommandEventArgs)
    '    Select Case e.CommandName
    '        Case "Previous"
    '            currentPageNumber = Int32.Parse(ViewState("cpn")) - 1
    '            If gvAction.PageIndex > 0 Then
    '                gvAction.PageIndex = gvAction.PageIndex - 1
    '                ViewState("piA") = gvAction.PageIndex - 1
    '                'bindFilteGrid()
    '                BindDashBoard()
    '            End If
    '            Exit Select
    '        Case "Next"
    '            currentPageNumber = Int32.Parse(ViewState("cpn")) + 1
    '            gvAction.PageIndex = gvAction.PageIndex + 1
    '            ViewState("piA") = gvAction.PageIndex + 1
    '            'bindFilteGrid()
    '            BindDashBoard()
    '            Exit Select
    '    End Select
    '    BindDashBoard()
    'End Sub

    ' 27 may

    'Protected Sub ChangePageU(ByVal sender As Object, ByVal e As CommandEventArgs)
    '    Select Case e.CommandName
    '        Case "Previous"
    '            currentPageNumberu = Int32.Parse(ViewState("cpnu")) - 1
    '            If gvMyUpload.PageIndex > 0 Then
    '                gvMyUpload.PageIndex = gvMyUpload.PageIndex - 1
    '                ViewState("piU") = gvMyUpload.PageIndex - 1
    '                BindDashBoard()
    '            End If
    '            Exit Select
    '        Case "Next"
    '            currentPageNumberu = Int32.Parse(ViewState("cpnu")) + 1
    '            gvMyUpload.PageIndex = gvMyUpload.PageIndex + 1
    '            ViewState("piU") = gvMyUpload.PageIndex + 1
    '            BindDashBoard()
    '            Exit Select
    '    End Select
    '    BindDashBoard()
    'End Sub
    '27 may
    'Protected Sub ChangePagep(ByVal sender As Object, ByVal e As CommandEventArgs)
    '    Select Case e.CommandName
    '        Case "Previous"
    '            currentPageNumberp = Int32.Parse(ViewState("cpnp")) - 1
    '            If gvPending.PageIndex > 0 Then
    '                gvPending.PageIndex = gvPending.PageIndex - 1
    '                ViewState("piP") = gvPending.PageIndex - 1
    '                Session("pageindex") = gvPending.PageIndex
    '            End If
    '            Exit Select
    '        Case "Next"
    '            currentPageNumberp = Int32.Parse(ViewState("cpnp")) + 1
    '            gvPending.PageIndex = gvPending.PageIndex + 1
    '            ViewState("piP") = gvPending.PageIndex + 1
    '            Session("pageindex") = gvPending.PageIndex
    '            Exit Select
    '    End Select
    '    BindDashBoard()
    '    bindNeedActGrid()
    '    ' BindDashBoard()
    'End Sub

    'Private Function CalculateTotalPages(ByVal totalRows As Double) As Integer
    '    Dim totalPages As Integer = CInt(Math.Ceiling(totalRows / PAGE_SIZE))
    '    Return totalPages
    'End Function

    'Search filter for history
    Private Sub bindFilterDDl()

        'Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        'Dim con As SqlConnection = New SqlConnection(conStr)
        'Dim oda As New SqlDataAdapter("", con)
        'Try
        '    oda.SelectCommand.CommandType = CommandType.Text
        '    oda.SelectCommand.CommandText = "Select distinct M.tid[System Doc. ID],documenttype[Subject],curstatus[Status],U.username[Created By],(replace(convert(char(11),d.fdate,113),' ','-') + ' ' + left(convert(varchar(13),d.fdate,108),8))[Received On] ,AP.Username [To] ,datediff(day,fdate,getdate())[Pending Days]  from MMM_MST_DOC M with (nolock)  LEFT OUTER JOIN MMM_DOC_DTL D with (nolock) on D.docid =M.tid  LEFT OUTER JOIN MMM_MST_USER U with (nolock) ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP with (nolock) ON AP.uid=D.UserID where D.UserID = " & Val(Session("UID").ToString()) & " and aprstatus is not null "
        '    'oda.SelectCommand.CommandText = "Select distinct M.tid[System Doc. ID],documenttype[Subject],curstatus[Status],U.username[Created By],adate[Applied On] ,AP.Username [To] ,datediff(day,fdate,getdate())[Pending From] from MMM_MST_DOC M with (nolock)  LEFT OUTER JOIN MMM_DOC_DTL D with (nolock) on D.tid=M.lasttid  LEFT OUTER JOIN MMM_MST_USER U with (nolock) ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP with (nolock) ON AP.uid=D.UserID where M.oUid = " & Val(Session("UID").ToString()) & " and curstatus<> 'ARCHIVE' order by M.Tid desc"
        '    Dim ds As New DataTable
        '    oda.Fill(ds)
        '    oda.SelectCommand.CommandText = "Select distinct M.tid,documenttype,curstatus,U.username,(replace(convert(char(11),d.fdate,113),' ','-') + ' ' + left(convert(varchar(13),d.fdate,108),8))[fdate] ,AP.Username[To],datediff(day,fdate,getdate())[Pending Days] from MMM_MST_DOC M with (nolock)  LEFT OUTER JOIN MMM_DOC_DTL D with (nolock) on D.docid =M.tid  LEFT OUTER JOIN MMM_MST_USER U with (nolock) ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP with (nolock) ON AP.uid=D.UserID where D.UserID = " & Val(Session("UID").ToString()) & " and aprstatus is not null "

        '    'oda.SelectCommand.CommandText = "Select distinct M.tid,documenttype,curstatus,U.username,adate ,AP.Username[To],datediff(day,fdate,getdate())[fdate] from MMM_MST_DOC M with (nolock)  LEFT OUTER JOIN MMM_DOC_DTL D with (nolock) on D.tid=M.lasttid  LEFT OUTER JOIN MMM_MST_USER U with (nolock) ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP with (nolock) ON AP.uid=D.UserID where M.oUid = " & Val(Session("UID").ToString()) & " and curstatus<> 'ARCHIVE' order by M.Tid desc"
        '    Dim ds1 As New DataTable
        '    oda.Fill(ds1)
        '    For i As Integer = 0 To ds.Columns.Count - 1
        '        ddlGrdHdr.Items.Add(ds.Columns.Item(i).ToString)
        '        ddlGrdHdr.Items(i).Value = ds1.Columns.Item(i).ToString
        '    Next

        '    'ddlGrdHdr.DataValueField = ds
        'Catch ex As Exception
        'Finally
        '    con.Close()
        '    con.Dispose()
        '    oda.Dispose()
        'End Try

    End Sub

    Protected Sub ddlGrdHdr_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlGrdHdr.SelectedIndexChanged
        'BindDashBoard()
        'binddropdown()

    End Sub

    'Protected Sub binddropdown()
    '    ' last updated by sunil for removing archive docs frm history tab - 13-Dec-13
    '    Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
    '    Dim con As SqlConnection = New SqlConnection(conStr)
    '    Dim oda As New SqlDataAdapter("", con)
    '    Try
    '        oda.SelectCommand.CommandType = CommandType.Text
    '        If ddlGrdHdr.SelectedItem.Text = "System Doc. ID" Then
    '            ' oda.SelectCommand.CommandText = "Select distinct  M." & ddlGrdHdr.SelectedValue & " from MMM_MST_DOC M with (nolock)  LEFT OUTER JOIN MMM_DOC_DTL D with (nolock) on D.tid=M.lasttid  LEFT OUTER JOIN MMM_MST_USER U with (nolock) ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP with (nolock) ON AP.uid=D.UserID where M.oUid = " & Val(Session("UID").ToString()) & " "
    '            oda.SelectCommand.CommandText = "Select distinct  M." & ddlGrdHdr.SelectedValue & " from MMM_MST_DOC M with (nolock)  LEFT OUTER JOIN MMM_DOC_DTL D with (nolock) on D.docid =M.tid  LEFT OUTER JOIN MMM_MST_USER U with (nolock) ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP with (nolock) ON AP.uid=D.UserID where D.UserID = " & Val(Session("UID").ToString()) & " and aprstatus is not null  and m.curstatus<>'ARCHIVE' order by m.tid  "
    '        ElseIf ddlGrdHdr.SelectedItem.Text = "Created By" Then
    '            'oda.SelectCommand.CommandText = "Select distinct  u." & ddlGrdHdr.SelectedValue & " from MMM_MST_DOC M with (nolock)  LEFT OUTER JOIN MMM_DOC_DTL D with (nolock) on D.tid=M.lasttid  LEFT OUTER JOIN MMM_MST_USER U with (nolock) ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP with (nolock) ON AP.uid=D.UserID where M.oUid = " & Val(Session("UID").ToString()) & " "
    '            oda.SelectCommand.CommandText = "Select distinct  u." & ddlGrdHdr.SelectedValue & " from MMM_MST_DOC M with (nolock)  LEFT OUTER JOIN MMM_DOC_DTL D with (nolock) on D.docid =M.tid  LEFT OUTER JOIN MMM_MST_USER U with (nolock) ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP with (nolock) ON AP.uid=D.UserID where D.UserID = " & Val(Session("UID").ToString()) & " and aprstatus is not null  and m.curstatus<>'ARCHIVE' order by u.username "
    '        ElseIf ddlGrdHdr.SelectedItem.Text = "To" Then
    '            'oda.SelectCommand.CommandText = "Select distinct AP.Username from MMM_MST_DOC M with (nolock)  LEFT OUTER JOIN MMM_DOC_DTL D with (nolock) on D.tid=M.lasttid  LEFT OUTER JOIN MMM_MST_USER U with (nolock) ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP with (nolock) ON AP.uid=D.UserID where M.oUid = " & Val(Session("UID").ToString()) & " "
    '            oda.SelectCommand.CommandText = "Select distinct AP.Username from MMM_MST_DOC M with (nolock)  LEFT OUTER JOIN MMM_DOC_DTL D with (nolock) on D.docid =M.tid  LEFT OUTER JOIN MMM_MST_USER U with (nolock) ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP with (nolock) ON AP.uid=D.UserID where D.UserID = " & Val(Session("UID").ToString()) & " and aprstatus is not null  and m.curstatus<>'ARCHIVE' order by ap.username  "
    '        ElseIf ddlGrdHdr.SelectedItem.Text = "Pending Days" Then
    '            'oda.SelectCommand.CommandText = "Select distinct datediff(day,fdate,getdate()) from MMM_MST_DOC M with (nolock)  LEFT OUTER JOIN MMM_DOC_DTL D with (nolock) on D.tid=M.lasttid  LEFT OUTER JOIN MMM_MST_USER U with (nolock) ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP with (nolock) ON AP.uid=D.UserID where M.oUid = " & Val(Session("UID").ToString()) & "  "
    '            oda.SelectCommand.CommandText = "Select distinct datediff(day,fdate,getdate())[Pending Days] from MMM_MST_DOC M with (nolock)  LEFT OUTER JOIN MMM_DOC_DTL D with (nolock) on D.docid =M.tid  LEFT OUTER JOIN MMM_MST_USER U with (nolock) ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP with (nolock) ON AP.uid=D.UserID where D.UserID = " & Val(Session("UID").ToString()) & " and aprstatus is not null  and m.curstatus<>'ARCHIVE'"
    '        ElseIf ddlGrdHdr.SelectedItem.Text = "Received On" Then
    '            'oda.SelectCommand.CommandText = "Select distinct datediff(day,fdate,getdate()) from MMM_MST_DOC M with (nolock)  LEFT OUTER JOIN MMM_DOC_DTL D with (nolock) on D.tid=M.lasttid  LEFT OUTER JOIN MMM_MST_USER U with (nolock) ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP with (nolock) ON AP.uid=D.UserID where M.oUid = " & Val(Session("UID").ToString()) & "  "
    '            oda.SelectCommand.CommandText = "Select distinct (replace(convert(char(11),d.fdate,113),' ','-'))[fdate] from MMM_MST_DOC M with (nolock)  LEFT OUTER JOIN MMM_DOC_DTL D with (nolock) on D.docid =M.tid  LEFT OUTER JOIN MMM_MST_USER U with (nolock) ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP with (nolock) ON AP.uid=D.UserID where D.UserID = " & Val(Session("UID").ToString()) & " and aprstatus is not null  and m.curstatus<>'ARCHIVE'"
    '        Else
    '            'oda.SelectCommand.CommandText = "Select distinct  " & ddlGrdHdr.SelectedValue & " from MMM_MST_DOC M with (nolock)  LEFT OUTER JOIN MMM_DOC_DTL D with (nolock) on D.tid=M.lasttid  LEFT OUTER JOIN MMM_MST_USER U with (nolock) ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP with (nolock) ON AP.uid=D.UserID where M.oUid = " & Val(Session("UID").ToString()) & " "
    '            oda.SelectCommand.CommandText = "Select distinct  " & ddlGrdHdr.SelectedValue & " from MMM_MST_DOC M with (nolock)  LEFT OUTER JOIN MMM_DOC_DTL D with (nolock) on D.docid =M.tid  LEFT OUTER JOIN MMM_MST_USER U with (nolock) ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP with (nolock) ON AP.uid=D.UserID where D.UserID = " & Val(Session("UID").ToString()) & " and aprstatus is not null  and m.curstatus<>'ARCHIVE' "
    '        End If
    '        Dim ds As New DataTable
    '        oda.Fill(ds)
    '        ddlGrdVal.Items.Clear()
    '        ddlGrdVal.Items.Add("--please Select--")
    '        For j As Integer = 0 To ds.Rows.Count - 1
    '            ddlGrdVal.Items.Add(ds.Rows(j).Item(0).ToString)
    '            ddlGrdVal.Items(j + 1).Value = ds.Rows(j).Item(0).ToString
    '        Next
    '    Catch ex As Exception
    '    Finally
    '        con.Close()
    '        con.Dispose()
    '        oda.Dispose()
    '    End Try

    'End Sub
    '27 may
    'Private Sub bindFilteGrid()
    '    ' last updated by sunil for removing archive docs frm history tab - 13-Dec-13
    '    Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
    '    Dim con As SqlConnection = New SqlConnection(conStr)
    '    Dim oda As New SqlDataAdapter("", con)
    '    Try
    '        oda.SelectCommand.CommandType = CommandType.Text
    '        If ddlGrdHdr.SelectedItem.Text = "System Doc. ID" Then

    '            If txtgrdval.Text <> "" Then
    '                oda.SelectCommand.CommandText = "Select distinct  M.tid,documenttype,curstatus,U.username,(replace(convert(char(11),d.fdate,113),' ','-') + ' ' + left(convert(varchar(13),d.fdate,108),8))[fdate],AP.Username [apusername],datediff(day,fdate,getdate())[Pending Days] from MMM_MST_DOC M with (nolock)  LEFT OUTER JOIN MMM_DOC_DTL D with (nolock) on D.docid =M.tid  LEFT OUTER JOIN MMM_MST_USER U with (nolock) ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP with (nolock) ON AP.uid=D.UserID where D.UserID = " & Val(Session("UID").ToString()) & " and aprstatus is not null  and M." & ddlGrdHdr.SelectedValue & " like '%" & Trim(txtgrdval.Text) & "%'  and m.curstatus<>'ARCHIVE' "
    '            Else
    '                oda.SelectCommand.CommandText = "Select distinct  M.tid,documenttype,curstatus,U.username,(replace(convert(char(11),d.fdate,113),' ','-') + ' ' + left(convert(varchar(13),d.fdate,108),8))[fdate],AP.Username [apusername],datediff(day,fdate,getdate())[Pending Days] from MMM_MST_DOC M with (nolock)  LEFT OUTER JOIN MMM_DOC_DTL D with (nolock) on D.docid =M.tid  LEFT OUTER JOIN MMM_MST_USER U with (nolock) ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP with (nolock) ON AP.uid=D.UserID where D.UserID = " & Val(Session("UID").ToString()) & " and aprstatus is not null and m.curstatus<>'ARCHIVE' order by m.tid desc  "
    '            End If

    '            'If ddlGrdVal.SelectedItem.Text = "--please Select--" Then
    '            '    oda.SelectCommand.CommandText = "Select distinct  M.tid,documenttype,curstatus,U.username,(replace(convert(char(11),d.fdate,113),' ','-') + ' ' + left(convert(varchar(13),d.fdate,108),8))[fdate],AP.Username [apusername],datediff(day,fdate,getdate())[Pending Days] from MMM_MST_DOC M with (nolock)  LEFT OUTER JOIN MMM_DOC_DTL D with (nolock) on D.docid =M.tid  LEFT OUTER JOIN MMM_MST_USER U with (nolock) ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP with (nolock) ON AP.uid=D.UserID where D.UserID = " & Val(Session("UID").ToString()) & " and aprstatus is not null and m.curstatus<>'ARCHIVE' order by m.tid desc  "
    '            'Else
    '            '    oda.SelectCommand.CommandText = "Select distinct  M.tid,documenttype,curstatus,U.username,(replace(convert(char(11),d.fdate,113),' ','-') + ' ' + left(convert(varchar(13),d.fdate,108),8))[fdate],AP.Username [apusername],datediff(day,fdate,getdate())[Pending Days] from MMM_MST_DOC M with (nolock)  LEFT OUTER JOIN MMM_DOC_DTL D with (nolock) on D.docid =M.tid  LEFT OUTER JOIN MMM_MST_USER U with (nolock) ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP with (nolock) ON AP.uid=D.UserID where D.UserID = " & Val(Session("UID").ToString()) & " and aprstatus is not null  and M." & ddlGrdHdr.SelectedValue & "=" & ddlGrdVal.SelectedItem.Text & "  and m.curstatus<>'ARCHIVE' "
    '            'End If
    '        ElseIf ddlGrdHdr.SelectedItem.Text = "Pending Days" Then
    '            If txtgrdval.Text <> "" Then
    '                oda.SelectCommand.CommandText = "Select distinct  M.tid,documenttype,curstatus,U.username,(replace(convert(char(11),d.fdate,113),' ','-') + ' ' + left(convert(varchar(13),d.fdate,108),8))[fdate],AP.Username [apusername],datediff(day,fdate,getdate())[Pending Days] from MMM_MST_DOC M with (nolock)  LEFT OUTER JOIN MMM_DOC_DTL D with (nolock) on D.docid =M.tid  LEFT OUTER JOIN MMM_MST_USER U with (nolock) ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP with (nolock) ON AP.uid=D.UserID where D.UserID = " & Val(Session("UID").ToString()) & " and aprstatus is not null  and datediff(day,fdate,getdate()) like '%" & Trim(txtgrdval.Text) & "%'  and m.curstatus<>'ARCHIVE' "
    '            Else
    '                oda.SelectCommand.CommandText = "Select distinct  M.tid,documenttype,curstatus,U.username,(replace(convert(char(11),d.fdate,113),' ','-') + ' ' + left(convert(varchar(13),d.fdate,108),8))[fdate],AP.Username [apusername],datediff(day,fdate,getdate())[Pending Days] from MMM_MST_DOC M with (nolock)  LEFT OUTER JOIN MMM_DOC_DTL D with (nolock) on D.docid =M.tid  LEFT OUTER JOIN MMM_MST_USER U with (nolock) ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP with (nolock) ON AP.uid=D.UserID where D.UserID = " & Val(Session("UID").ToString()) & " and aprstatus is not null  and m.curstatus<>'ARCHIVE'  "
    '            End If

    '            'If ddlGrdVal.SelectedItem.Text = "--please Select--" Then
    '            '    oda.SelectCommand.CommandText = "Select distinct  M.tid,documenttype,curstatus,U.username,(replace(convert(char(11),d.fdate,113),' ','-') + ' ' + left(convert(varchar(13),d.fdate,108),8))[fdate],AP.Username [apusername],datediff(day,fdate,getdate())[Pending Days] from MMM_MST_DOC M with (nolock)  LEFT OUTER JOIN MMM_DOC_DTL D with (nolock) on D.docid =M.tid  LEFT OUTER JOIN MMM_MST_USER U with (nolock) ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP with (nolock) ON AP.uid=D.UserID where D.UserID = " & Val(Session("UID").ToString()) & " and aprstatus is not null  and m.curstatus<>'ARCHIVE'  "
    '            'Else
    '            '    oda.SelectCommand.CommandText = "Select distinct  M.tid,documenttype,curstatus,U.username,(replace(convert(char(11),d.fdate,113),' ','-') + ' ' + left(convert(varchar(13),d.fdate,108),8))[fdate],AP.Username [apusername],datediff(day,fdate,getdate())[Pending Days] from MMM_MST_DOC M with (nolock)  LEFT OUTER JOIN MMM_DOC_DTL D with (nolock) on D.docid =M.tid  LEFT OUTER JOIN MMM_MST_USER U with (nolock) ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP with (nolock) ON AP.uid=D.UserID where D.UserID = " & Val(Session("UID").ToString()) & " and aprstatus is not null  and datediff(day,fdate,getdate()) = '" & ddlGrdVal.SelectedItem.Text & "'  and m.curstatus<>'ARCHIVE' "
    '            'End If
    '        ElseIf ddlGrdHdr.SelectedItem.Text = "Received On" Then
    '            If txtgrdval.Text <> "" Then
    '                oda.SelectCommand.CommandText = "Select distinct  M.tid,documenttype,curstatus,U.username,(replace(convert(char(11),d.fdate,113),' ','-') + ' ' + left(convert(varchar(13),d.fdate,108),8))[fdate],AP.Username [apusername],datediff(day,fdate,getdate())[Pending Days] from MMM_MST_DOC M with (nolock)  LEFT OUTER JOIN MMM_DOC_DTL D with (nolock) on D.docid =M.tid  LEFT OUTER JOIN MMM_MST_USER U with (nolock) ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP with (nolock) ON AP.uid=D.UserID where D.UserID = " & Val(Session("UID").ToString()) & " and aprstatus is not null  and m.curstatus<>'ARCHIVE' and replace(convert(char(11)," & ddlGrdHdr.SelectedValue & ",113),' ','-') like '%" & Trim(txtgrdval.Text) & "%'"
    '            Else
    '                oda.SelectCommand.CommandText = "Select distinct  M.tid,documenttype,curstatus,U.username,(replace(convert(char(11),d.fdate,113),' ','-') + ' ' + left(convert(varchar(13),d.fdate,108),8))[fdate],AP.Username [apusername],datediff(day,fdate,getdate())[Pending Days] from MMM_MST_DOC M with (nolock)  LEFT OUTER JOIN MMM_DOC_DTL D with (nolock) on D.docid =M.tid  LEFT OUTER JOIN MMM_MST_USER U with (nolock) ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP with (nolock) ON AP.uid=D.UserID where D.UserID = " & Val(Session("UID").ToString()) & " and aprstatus is not null  and m.curstatus<>'ARCHIVE'  "
    '            End If


    '            'If ddlGrdVal.SelectedItem.Text = "--please Select--" Then
    '            '    oda.SelectCommand.CommandText = "Select distinct  M.tid,documenttype,curstatus,U.username,(replace(convert(char(11),d.fdate,113),' ','-') + ' ' + left(convert(varchar(13),d.fdate,108),8))[fdate],AP.Username [apusername],datediff(day,fdate,getdate())[Pending Days] from MMM_MST_DOC M with (nolock)  LEFT OUTER JOIN MMM_DOC_DTL D with (nolock) on D.docid =M.tid  LEFT OUTER JOIN MMM_MST_USER U with (nolock) ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP with (nolock) ON AP.uid=D.UserID where D.UserID = " & Val(Session("UID").ToString()) & " and aprstatus is not null  and m.curstatus<>'ARCHIVE'  "
    '            'Else
    '            '    oda.SelectCommand.CommandText = "Select distinct  M.tid,documenttype,curstatus,U.username,(replace(convert(char(11),d.fdate,113),' ','-') + ' ' + left(convert(varchar(13),d.fdate,108),8))[fdate],AP.Username [apusername],datediff(day,fdate,getdate())[Pending Days] from MMM_MST_DOC M with (nolock)  LEFT OUTER JOIN MMM_DOC_DTL D with (nolock) on D.docid =M.tid  LEFT OUTER JOIN MMM_MST_USER U with (nolock) ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP with (nolock) ON AP.uid=D.UserID where D.UserID = " & Val(Session("UID").ToString()) & " and aprstatus is not null  and m.curstatus<>'ARCHIVE' and replace(convert(char(11)," & ddlGrdHdr.SelectedValue & ",113),' ','-') = replace(convert(char(11),'" & ddlGrdVal.SelectedItem.Text & "',113),' ','-')"
    '            'End If
    '        ElseIf ddlGrdHdr.SelectedItem.Text = "Created By" Then
    '            If txtgrdval.Text <> "" Then
    '                oda.SelectCommand.CommandText = "Select distinct  M.tid,documenttype,curstatus,U.username,(replace(convert(char(11),d.fdate,113),' ','-') + ' ' + left(convert(varchar(13),d.fdate,108),8))[fdate],AP.Username [apusername],datediff(day,fdate,getdate())[Pending Days] from MMM_MST_DOC M with (nolock)  LEFT OUTER JOIN MMM_DOC_DTL D with (nolock) on D.docid =M.tid  LEFT OUTER JOIN MMM_MST_USER U with (nolock) ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP with (nolock) ON AP.uid=D.UserID where  aprstatus is not null  and u." & ddlGrdHdr.SelectedValue & " like '%" & Trim(txtgrdval.Text) & "%'  and m.curstatus<>'ARCHIVE' "
    '            Else
    '                oda.SelectCommand.CommandText = "Select distinct  M.tid,documenttype,curstatus,U.username,(replace(convert(char(11),d.fdate,113),' ','-') + ' ' + left(convert(varchar(13),d.fdate,108),8))[fdate],AP.Username [apusername],datediff(day,fdate,getdate())[Pending Days] from MMM_MST_DOC M with (nolock)  LEFT OUTER JOIN MMM_DOC_DTL D with (nolock) on D.docid =M.tid  LEFT OUTER JOIN MMM_MST_USER U with (nolock) ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP with (nolock) ON AP.uid=D.UserID where D.UserID = " & Val(Session("UID").ToString()) & " and aprstatus is not null  and m.curstatus<>'ARCHIVE' "

    '            End If

    '            'If ddlGrdVal.SelectedItem.Text = "--please Select--" Then
    '            '    oda.SelectCommand.CommandText = "Select distinct  M.tid,documenttype,curstatus,U.username,(replace(convert(char(11),d.fdate,113),' ','-') + ' ' + left(convert(varchar(13),d.fdate,108),8))[fdate],AP.Username [apusername],datediff(day,fdate,getdate())[Pending Days] from MMM_MST_DOC M with (nolock)  LEFT OUTER JOIN MMM_DOC_DTL D with (nolock) on D.docid =M.tid  LEFT OUTER JOIN MMM_MST_USER U with (nolock) ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP with (nolock) ON AP.uid=D.UserID where D.UserID = " & Val(Session("UID").ToString()) & " and aprstatus is not null  and m.curstatus<>'ARCHIVE' "
    '            'Else
    '            '    oda.SelectCommand.CommandText = "Select distinct  M.tid,documenttype,curstatus,U.username,(replace(convert(char(11),d.fdate,113),' ','-') + ' ' + left(convert(varchar(13),d.fdate,108),8))[fdate],AP.Username [apusername],datediff(day,fdate,getdate())[Pending Days] from MMM_MST_DOC M with (nolock)  LEFT OUTER JOIN MMM_DOC_DTL D with (nolock) on D.docid =M.tid  LEFT OUTER JOIN MMM_MST_USER U with (nolock) ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP with (nolock) ON AP.uid=D.UserID where  aprstatus is not null  and u." & ddlGrdHdr.SelectedValue & "='" & ddlGrdVal.SelectedItem.Text & "'  and m.curstatus<>'ARCHIVE' "
    '            'End If
    '        ElseIf ddlGrdHdr.SelectedItem.Text = "To" Then
    '            If txtgrdval.Text <> "" Then
    '                oda.SelectCommand.CommandText = "Select distinct  M.tid,documenttype,curstatus,U.username,(replace(convert(char(11),d.fdate,113),' ','-') + ' ' + left(convert(varchar(13),d.fdate,108),8))[fdate],AP.Username [apusername],datediff(day,fdate,getdate())[Pending Days] from MMM_MST_DOC M with (nolock)  LEFT OUTER JOIN MMM_DOC_DTL D with (nolock) on D.docid =M.tid  LEFT OUTER JOIN MMM_MST_USER U with (nolock) ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP with (nolock) ON AP.uid=D.UserID where  aprstatus is not null  and AP.username like '%" & Trim(txtgrdval.Text) & "%'  and m.curstatus<>'ARCHIVE' "
    '            Else
    '                oda.SelectCommand.CommandText = "Select distinct  M.tid,documenttype,curstatus,U.username,(replace(convert(char(11),d.fdate,113),' ','-') + ' ' + left(convert(varchar(13),d.fdate,108),8))[fdate],AP.Username [apusername],datediff(day,fdate,getdate())[Pending Days] from MMM_MST_DOC M with (nolock)  LEFT OUTER JOIN MMM_DOC_DTL D with (nolock) on D.docid =M.tid  LEFT OUTER JOIN MMM_MST_USER U with (nolock) ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP with (nolock) ON AP.uid=D.UserID where D.UserID = " & Val(Session("UID").ToString()) & " and aprstatus is not null  and m.curstatus<>'ARCHIVE'  "
    '            End If

    '            'If ddlGrdVal.SelectedItem.Text = "--please Select--" Then
    '            '    oda.SelectCommand.CommandText = "Select distinct  M.tid,documenttype,curstatus,U.username,(replace(convert(char(11),d.fdate,113),' ','-') + ' ' + left(convert(varchar(13),d.fdate,108),8))[fdate],AP.Username [apusername],datediff(day,fdate,getdate())[Pending Days] from MMM_MST_DOC M with (nolock)  LEFT OUTER JOIN MMM_DOC_DTL D with (nolock) on D.docid =M.tid  LEFT OUTER JOIN MMM_MST_USER U with (nolock) ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP with (nolock) ON AP.uid=D.UserID where D.UserID = " & Val(Session("UID").ToString()) & " and aprstatus is not null  and m.curstatus<>'ARCHIVE'  "
    '            'Else
    '            '    oda.SelectCommand.CommandText = "Select distinct  M.tid,documenttype,curstatus,U.username,(replace(convert(char(11),d.fdate,113),' ','-') + ' ' + left(convert(varchar(13),d.fdate,108),8))[fdate],AP.Username [apusername],datediff(day,fdate,getdate())[Pending Days] from MMM_MST_DOC M with (nolock)  LEFT OUTER JOIN MMM_DOC_DTL D with (nolock) on D.docid =M.tid  LEFT OUTER JOIN MMM_MST_USER U with (nolock) ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP with (nolock) ON AP.uid=D.UserID where  aprstatus is not null  and AP.username='" & ddlGrdVal.SelectedItem.Text & "'  and m.curstatus<>'ARCHIVE' "
    '            'End If
    '        Else
    '            If txtgrdval.Text <> "" Then
    '                oda.SelectCommand.CommandText = "Select distinct  M.tid,documenttype,curstatus,U.username,(replace(convert(char(11),d.fdate,113),' ','-') + ' ' + left(convert(varchar(13),d.fdate,108),8))[fdate],AP.Username [apusername],datediff(day,fdate,getdate())[Pending Days] from MMM_MST_DOC M with (nolock)  LEFT OUTER JOIN MMM_DOC_DTL D with (nolock) on D.docid =M.tid  LEFT OUTER JOIN MMM_MST_USER U with (nolock) ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP with (nolock) ON AP.uid=D.UserID where D.UserID = " & Val(Session("UID").ToString()) & " and aprstatus is not null  and " & ddlGrdHdr.SelectedValue & " like '%" & Trim(txtgrdval.Text) & "%' and m.curstatus<>'ARCHIVE' "
    '            Else
    '                oda.SelectCommand.CommandText = "Select distinct  M.tid,documenttype,curstatus,U.username,(replace(convert(char(11),d.fdate,113),' ','-') + ' ' + left(convert(varchar(13),d.fdate,108),8))[fdate],AP.Username [apusername],datediff(day,fdate,getdate())[Pending Days] from MMM_MST_DOC M with (nolock)  LEFT OUTER JOIN MMM_DOC_DTL D with (nolock) on D.docid =M.tid  LEFT OUTER JOIN MMM_MST_USER U with (nolock) ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP with (nolock) ON AP.uid=D.UserID where D.UserID = " & Val(Session("UID").ToString()) & " and aprstatus is not null  and m.curstatus<>'ARCHIVE' "

    '            End If

    '            'If ddlGrdVal.SelectedItem.Text = "--please Select--" Then
    '            '    oda.SelectCommand.CommandText = "Select distinct  M.tid,documenttype,curstatus,U.username,(replace(convert(char(11),d.fdate,113),' ','-') + ' ' + left(convert(varchar(13),d.fdate,108),8))[fdate],AP.Username [apusername],datediff(day,fdate,getdate())[Pending Days] from MMM_MST_DOC M with (nolock)  LEFT OUTER JOIN MMM_DOC_DTL D with (nolock) on D.docid =M.tid  LEFT OUTER JOIN MMM_MST_USER U with (nolock) ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP with (nolock) ON AP.uid=D.UserID where D.UserID = " & Val(Session("UID").ToString()) & " and aprstatus is not null  and m.curstatus<>'ARCHIVE' "
    '            'Else
    '            '    oda.SelectCommand.CommandText = "Select distinct  M.tid,documenttype,curstatus,U.username,(replace(convert(char(11),d.fdate,113),' ','-') + ' ' + left(convert(varchar(13),d.fdate,108),8))[fdate],AP.Username [apusername],datediff(day,fdate,getdate())[Pending Days] from MMM_MST_DOC M with (nolock)  LEFT OUTER JOIN MMM_DOC_DTL D with (nolock) on D.docid =M.tid  LEFT OUTER JOIN MMM_MST_USER U with (nolock) ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP with (nolock) ON AP.uid=D.UserID where D.UserID = " & Val(Session("UID").ToString()) & " and aprstatus is not null  and " & ddlGrdHdr.SelectedValue & "='" & ddlGrdVal.SelectedItem.Text & "' and m.curstatus<>'ARCHIVE' "
    '            'End If
    '        End If

    '        Dim ds As New DataSet
    '        oda.Fill(ds, "action")
    '        gvAction.DataSource = ds.Tables("action")
    '        gvAction.DataBind()
    '        ViewState("count") = CInt(Math.Ceiling(ds.Tables("action").Rows.Count / gvAction.PageSize))
    '        If Session("Grid") = "gvAction" Then
    '            currentPageNumber = Val(Session("pageindex")) + 1
    '        End If
    '        lbltotalpages.Text = Trim("Displaying page no. " & currentPageNumber & " of total page no(s) " & ViewState("count"))
    '        'lbltotpending.Enabled = False
    '        'lnknextpending.Enabled = False
    '        'lnkprevpending.Enabled = False
    '    Catch ex As Exception
    '    Finally
    '        con.Close()
    '        con.Dispose()
    '        oda.Dispose()
    '    End Try

    'End Sub

    'Protected Sub ddlGrdVal_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlGrdVal.SelectedIndexChanged
    '    ' bindFilteGrid()

    'End Sub

    ''Search filter for Need to act  27 may
    'Private Sub bindFilteNeedActddl()
    '    Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
    '    Dim con As SqlConnection = New SqlConnection(conStr)
    '    Dim oda As New SqlDataAdapter("", con)
    '    Try
    '        oda.SelectCommand.CommandType = CommandType.Text
    '        oda.SelectCommand.CommandText = "Select distinct M.tid[System Doc. ID],documenttype[Subject],curstatus[Status],U.username[Created By],(replace(convert(char(11),d.fdate,113),' ','-') + ' ' + left(convert(varchar(13),d.fdate,108),8))[Received On] ,datediff(day,fdate,getdate())[Pending Days]   from MMM_MST_DOC M with (nolock)  LEFT OUTER JOIN MMM_DOC_DTL D with (nolock) on D.tid=M.lasttid  LEFT OUTER JOIN MMM_MST_USER U with (nolock) ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP with (nolock) ON AP.uid=D.UserID where D.userid = " & Val(Session("UID").ToString()) & ""
    '        'oda.SelectCommand.CommandText = "Select distinct M.tid[System Doc. ID],documenttype[Subject],curstatus[Status],U.username[Created By],adate[Applied On] ,AP.Username [To] ,datediff(day,fdate,getdate())[Pending From] from MMM_MST_DOC M with (nolock)  LEFT OUTER JOIN MMM_DOC_DTL D with (nolock) on D.tid=M.lasttid  LEFT OUTER JOIN MMM_MST_USER U with (nolock) ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP with (nolock) ON AP.uid=D.UserID where M.oUid = " & Val(Session("UID").ToString()) & " and curstatus<> 'ARCHIVE' order by M.Tid desc"
    '        Dim ds As New DataTable
    '        oda.Fill(ds)
    '        oda.SelectCommand.CommandText = "Select distinct M.tid,documenttype,curstatus,U.username,(replace(convert(char(11),d.fdate,113),' ','-') + ' ' + left(convert(varchar(13),d.fdate,108),8))[fdate] ,datediff(day,fdate,getdate())[Pending Days]  from MMM_MST_DOC M with (nolock)  LEFT OUTER JOIN MMM_DOC_DTL D with (nolock) on D.tid=M.lasttid  LEFT OUTER JOIN MMM_MST_USER U with (nolock) ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP with (nolock) ON AP.uid=D.UserID where D.userid = " & Val(Session("UID").ToString()) & ""

    '        'oda.SelectCommand.CommandText = "Select distinct M.tid,documenttype,curstatus,U.username,adate ,AP.Username[To],datediff(day,fdate,getdate())[fdate] from MMM_MST_DOC M with (nolock)  LEFT OUTER JOIN MMM_DOC_DTL D with (nolock) on D.tid=M.lasttid  LEFT OUTER JOIN MMM_MST_USER U with (nolock) ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP with (nolock) ON AP.uid=D.UserID where M.oUid = " & Val(Session("UID").ToString()) & " and curstatus<> 'ARCHIVE' order by M.Tid desc"
    '        Dim ds1 As New DataTable
    '        oda.Fill(ds1)
    '        For i As Integer = 0 To ds.Columns.Count - 1
    '            ddlPendinggrdHdr.Items.Add(ds.Columns.Item(i).ToString)
    '            ddlPendinggrdHdr.Items(i).Value = ds1.Columns.Item(i).ToString
    '        Next
    '    Catch ex As Exception
    '    Finally
    '        con.Close()
    '        con.Dispose()
    '        oda.Dispose()
    '    End Try

    'End Sub

    'Protected Sub ddlPendinggrdHdr_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlPendinggrdHdr.SelectedIndexChanged
    '    'BindDashBoard()
    '    '   bindPendingddl()
    'End Sub


    '27 may
    'Private Sub bindNeedActGrid()
    '    Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
    '    Dim con As SqlConnection = New SqlConnection(conStr)
    '    Dim oda As New SqlDataAdapter("", con)
    '    Try
    '        oda.SelectCommand.CommandType = CommandType.Text
    '        Dim StrQry As String = ""
    '        If ddlPendinggrdHdr.SelectedItem.Text = "System Doc. ID" Then
    '            If txtPendinggrdval.Text <> "" Then
    '                StrQry = "Select distinct  M.tid,documenttype,curstatus,U.username,(replace(convert(char(11),d.fdate,113),' ','-') + ' ' + left(convert(varchar(13),d.fdate,108),8))[fdate],AP.Username [apusername],datediff(day,fdate,getdate())[Pending Days],m.priority from MMM_MST_DOC M with (nolock)  LEFT OUTER JOIN MMM_DOC_DTL D with (nolock) on D.tid=M.lasttid  LEFT OUTER JOIN MMM_MST_USER U with (nolock) ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP with (nolock) ON AP.uid=D.UserID where D.userid = " & Val(Session("UID").ToString()) & " and M." & ddlPendinggrdHdr.SelectedValue & " like '%" & Trim(txtPendinggrdval.Text) & "%'  order by M.priority desc,m.tid desc"
    '            Else
    '                StrQry = "Select distinct  M.tid,documenttype,curstatus,U.username,(replace(convert(char(11),d.fdate,113),' ','-') + ' ' + left(convert(varchar(13),d.fdate,108),8))[fdate],AP.Username [apusername],datediff(day,fdate,getdate())[Pending Days],m.priority from MMM_MST_DOC M with (nolock)  LEFT OUTER JOIN MMM_DOC_DTL D with (nolock) on D.tid=M.lasttid  LEFT OUTER JOIN MMM_MST_USER U with (nolock) ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP with (nolock) ON AP.uid=D.UserID where D.userid = " & Val(Session("UID").ToString()) & " order by M.priority desc,m.tid desc"
    '            End If

    '            'If ddlPendinggrdVal.SelectedItem.Text = "--please Select--" Then
    '            '    StrQry = "Select distinct  M.tid,documenttype,curstatus,U.username,(replace(convert(char(11),d.fdate,113),' ','-') + ' ' + left(convert(varchar(13),d.fdate,108),8))[fdate],AP.Username [apusername],datediff(day,fdate,getdate())[Pending Days],m.priority from MMM_MST_DOC M with (nolock)  LEFT OUTER JOIN MMM_DOC_DTL D with (nolock) on D.tid=M.lasttid  LEFT OUTER JOIN MMM_MST_USER U with (nolock) ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP with (nolock) ON AP.uid=D.UserID where D.userid = " & Val(Session("UID").ToString()) & " order by M.priority desc,m.tid desc"
    '            'Else
    '            '    StrQry = "Select distinct  M.tid,documenttype,curstatus,U.username,(replace(convert(char(11),d.fdate,113),' ','-') + ' ' + left(convert(varchar(13),d.fdate,108),8))[fdate],AP.Username [apusername],datediff(day,fdate,getdate())[Pending Days],m.priority from MMM_MST_DOC M with (nolock)  LEFT OUTER JOIN MMM_DOC_DTL D with (nolock) on D.tid=M.lasttid  LEFT OUTER JOIN MMM_MST_USER U with (nolock) ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP with (nolock) ON AP.uid=D.UserID where D.userid = " & Val(Session("UID").ToString()) & " and M." & ddlPendinggrdHdr.SelectedValue & "=" & ddlPendinggrdVal.SelectedItem.Text & " order by M.priority desc,m.tid desc"
    '            'End If
    '        ElseIf ddlPendinggrdHdr.SelectedItem.Text = "Pending Days" Then
    '            If txtPendinggrdval.Text <> "" Then
    '                StrQry = "Select distinct  M.tid,documenttype,curstatus,U.username,(replace(convert(char(11),d.fdate,113),' ','-') + ' ' + left(convert(varchar(13),d.fdate,108),8))[fdate],AP.Username [apusername],datediff(day,fdate,getdate())[Pending Days],m.priority from MMM_MST_DOC M with (nolock)  LEFT OUTER JOIN MMM_DOC_DTL D with (nolock) on D.tid=M.lasttid  LEFT OUTER JOIN MMM_MST_USER U with (nolock) ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP with (nolock) ON AP.uid=D.UserID where D.userid = " & Val(Session("UID").ToString()) & " and datediff(day,fdate,getdate()) like  '%" & Trim(txtPendinggrdval.Text) & "%' order by M.priority desc,m.tid desc "

    '            Else
    '                StrQry = "Select distinct  M.tid,documenttype,curstatus,U.username,(replace(convert(char(11),d.fdate,113),' ','-') + ' ' + left(convert(varchar(13),d.fdate,108),8))[fdate],AP.Username [apusername],datediff(day,fdate,getdate())[Pending Days],m.priority from MMM_MST_DOC M with (nolock)  LEFT OUTER JOIN MMM_DOC_DTL D with (nolock) on D.tid=M.lasttid  LEFT OUTER JOIN MMM_MST_USER U with (nolock) ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP with (nolock) ON AP.uid=D.UserID where D.userid = " & Val(Session("UID").ToString()) & "  order by M.priority desc,m.tid desc"
    '            End If

    '            'If ddlPendinggrdVal.SelectedItem.Text = "--please Select--" Then
    '            '    StrQry = "Select distinct  M.tid,documenttype,curstatus,U.username,(replace(convert(char(11),d.fdate,113),' ','-') + ' ' + left(convert(varchar(13),d.fdate,108),8))[fdate],AP.Username [apusername],datediff(day,fdate,getdate())[Pending Days],m.priority from MMM_MST_DOC M with (nolock)  LEFT OUTER JOIN MMM_DOC_DTL D with (nolock) on D.tid=M.lasttid  LEFT OUTER JOIN MMM_MST_USER U with (nolock) ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP with (nolock) ON AP.uid=D.UserID where D.userid = " & Val(Session("UID").ToString()) & "  order by M.priority desc,m.tid desc"
    '            'Else
    '            '    StrQry = "Select distinct  M.tid,documenttype,curstatus,U.username,(replace(convert(char(11),d.fdate,113),' ','-') + ' ' + left(convert(varchar(13),d.fdate,108),8))[fdate],AP.Username [apusername],datediff(day,fdate,getdate())[Pending Days],m.priority from MMM_MST_DOC M with (nolock)  LEFT OUTER JOIN MMM_DOC_DTL D with (nolock) on D.tid=M.lasttid  LEFT OUTER JOIN MMM_MST_USER U with (nolock) ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP with (nolock) ON AP.uid=D.UserID where D.userid = " & Val(Session("UID").ToString()) & " and datediff(day,fdate,getdate()) = '" & ddlPendinggrdVal.SelectedItem.Text & "' order by M.priority desc,m.tid desc "
    '            'End If
    '        ElseIf ddlPendinggrdHdr.SelectedItem.Text = "Received On" Then
    '            If txtPendinggrdval.Text <> "" Then
    '                StrQry = "Select distinct  M.tid,documenttype,curstatus,U.username,(replace(convert(char(11),d.fdate,113),' ','-') + ' ' + left(convert(varchar(13),d.fdate,108),8))[fdate],AP.Username [apusername],datediff(day,fdate,getdate())[Pending Days],m.priority from MMM_MST_DOC M with (nolock)  LEFT OUTER JOIN MMM_DOC_DTL D with (nolock) on D.tid=M.lasttid  LEFT OUTER JOIN MMM_MST_USER U with (nolock) ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP with (nolock) ON AP.uid=D.UserID where D.userid = " & Val(Session("UID").ToString()) & " and replace(convert(char(11)," & ddlPendinggrdHdr.SelectedValue & ",113),' ','-')  like '%" & Trim(txtPendinggrdval.Text) & "%' order by M.priority desc,m.tid desc"
    '            Else
    '                StrQry = "Select distinct  M.tid,documenttype,curstatus,U.username,(replace(convert(char(11),d.fdate,113),' ','-') + ' ' + left(convert(varchar(13),d.fdate,108),8))[fdate],AP.Username [apusername],datediff(day,fdate,getdate())[Pending Days],m.priority from MMM_MST_DOC M with (nolock)  LEFT OUTER JOIN MMM_DOC_DTL D with (nolock) on D.tid=M.lasttid  LEFT OUTER JOIN MMM_MST_USER U with (nolock) ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP with (nolock) ON AP.uid=D.UserID where D.userid = " & Val(Session("UID").ToString()) & " order by M.priority desc,m.tid desc"
    '            End If


    '            'If ddlPendinggrdVal.SelectedItem.Text = "--please Select--" Then
    '            '    StrQry = "Select distinct  M.tid,documenttype,curstatus,U.username,(replace(convert(char(11),d.fdate,113),' ','-') + ' ' + left(convert(varchar(13),d.fdate,108),8))[fdate],AP.Username [apusername],datediff(day,fdate,getdate())[Pending Days],m.priority from MMM_MST_DOC M with (nolock)  LEFT OUTER JOIN MMM_DOC_DTL D with (nolock) on D.tid=M.lasttid  LEFT OUTER JOIN MMM_MST_USER U with (nolock) ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP with (nolock) ON AP.uid=D.UserID where D.userid = " & Val(Session("UID").ToString()) & " order by M.priority desc,m.tid desc"
    '            'Else
    '            '    StrQry = "Select distinct  M.tid,documenttype,curstatus,U.username,(replace(convert(char(11),d.fdate,113),' ','-') + ' ' + left(convert(varchar(13),d.fdate,108),8))[fdate],AP.Username [apusername],datediff(day,fdate,getdate())[Pending Days],m.priority from MMM_MST_DOC M with (nolock)  LEFT OUTER JOIN MMM_DOC_DTL D with (nolock) on D.tid=M.lasttid  LEFT OUTER JOIN MMM_MST_USER U with (nolock) ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP with (nolock) ON AP.uid=D.UserID where D.userid = " & Val(Session("UID").ToString()) & " and replace(convert(char(11)," & ddlPendinggrdHdr.SelectedValue & ",113),' ','-') = replace(convert(char(11),'" & ddlPendinggrdVal.SelectedItem.Text & "',113),' ','-') order by M.priority desc,m.tid desc"
    '            'End If
    '            'ElseIf ddlPendinggrdHdr.SelectedItem.Text = "Current User" Then
    '            '    If ddlPendinggrdVal.SelectedItem.Text = "--please Select--" Then
    '            '        StrQry = "Select distinct  M.tid,documenttype,curstatus,U.username,(replace(convert(char(11),d.fdate,113),' ','-') + ' ' + left(convert(varchar(13),d.fdate,108),8))[fdate],AP.Username [apusername],datediff(day,fdate,getdate())[Pending Days],m.priority from MMM_MST_DOC M with (nolock)  LEFT OUTER JOIN MMM_DOC_DTL D with (nolock) on D.tid=M.lasttid  LEFT OUTER JOIN MMM_MST_USER U with (nolock) ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP with (nolock) ON AP.uid=D.UserID where D.userid = " & Val(Session("UID").ToString()) & " order by M.priority desc,m.tid desc  "
    '            '    Else
    '            '        StrQry = "Select distinct  M.tid,documenttype,curstatus,U.username,(replace(convert(char(11),d.fdate,113),' ','-') + ' ' + left(convert(varchar(13),d.fdate,108),8))[fdate],AP.Username [apusername],datediff(day,fdate,getdate())[Pending Days],m.priority from MMM_MST_DOC M with (nolock)  LEFT OUTER JOIN MMM_DOC_DTL D with (nolock) on D.tid=M.lasttid  LEFT OUTER JOIN MMM_MST_USER U with (nolock) ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP with (nolock) ON AP.uid=D.UserID where D.userid = " & Val(Session("UID").ToString()) & "  order by M.priority desc,m.tid desc"
    '            '    End If
    '        ElseIf ddlPendinggrdHdr.SelectedItem.Text = "Created By" Then

    '            If txtPendinggrdval.Text <> "" Then
    '                StrQry = "Select distinct  M.tid,documenttype,curstatus,U.username,(replace(convert(char(11),d.fdate,113),' ','-') + ' ' + left(convert(varchar(13),d.fdate,108),8))[fdate],AP.Username [apusername],datediff(day,fdate,getdate())[Pending Days],m.priority from MMM_MST_DOC M with (nolock)  LEFT OUTER JOIN MMM_DOC_DTL D with (nolock) on D.tid=M.lasttid  LEFT OUTER JOIN MMM_MST_USER U with (nolock) ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP with (nolock) ON AP.uid=D.UserID where D.userid = " & Val(Session("UID").ToString()) & "  and u." & ddlPendinggrdHdr.SelectedValue & " like '%" & Trim(txtPendinggrdval.Text) & "%'  order by M.priority desc,m.tid desc"
    '            Else
    '                StrQry = "Select distinct  M.tid,documenttype,curstatus,U.username,(replace(convert(char(11),d.fdate,113),' ','-') + ' ' + left(convert(varchar(13),d.fdate,108),8))[fdate],AP.Username [apusername],datediff(day,fdate,getdate())[Pending Days],m.priority from MMM_MST_DOC M with (nolock)  LEFT OUTER JOIN MMM_DOC_DTL D with (nolock) on D.tid=M.lasttid  LEFT OUTER JOIN MMM_MST_USER U with (nolock) ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP with (nolock) ON AP.uid=D.UserID where D.userid = " & Val(Session("UID").ToString()) & " order by M.priority desc,m.tid desc"
    '            End If


    '            'If ddlPendinggrdVal.SelectedItem.Text = "--please Select--" Then
    '            '    StrQry = "Select distinct  M.tid,documenttype,curstatus,U.username,(replace(convert(char(11),d.fdate,113),' ','-') + ' ' + left(convert(varchar(13),d.fdate,108),8))[fdate],AP.Username [apusername],datediff(day,fdate,getdate())[Pending Days],m.priority from MMM_MST_DOC M with (nolock)  LEFT OUTER JOIN MMM_DOC_DTL D with (nolock) on D.tid=M.lasttid  LEFT OUTER JOIN MMM_MST_USER U with (nolock) ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP with (nolock) ON AP.uid=D.UserID where D.userid = " & Val(Session("UID").ToString()) & " order by M.priority desc,m.tid desc"
    '            'Else
    '            '    StrQry = "Select distinct  M.tid,documenttype,curstatus,U.username,(replace(convert(char(11),d.fdate,113),' ','-') + ' ' + left(convert(varchar(13),d.fdate,108),8))[fdate],AP.Username [apusername],datediff(day,fdate,getdate())[Pending Days],m.priority from MMM_MST_DOC M with (nolock)  LEFT OUTER JOIN MMM_DOC_DTL D with (nolock) on D.tid=M.lasttid  LEFT OUTER JOIN MMM_MST_USER U with (nolock) ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP with (nolock) ON AP.uid=D.UserID where D.userid = " & Val(Session("UID").ToString()) & "  and u." & ddlPendinggrdHdr.SelectedValue & "='" & ddlPendinggrdVal.SelectedItem.Text & "' order by M.priority desc,m.tid desc"
    '            'End If
    '        Else

    '            If txtPendinggrdval.Text <> "" Then
    '                StrQry = "Select distinct  M.tid,documenttype,curstatus,U.username,(replace(convert(char(11),d.fdate,113),' ','-') + ' ' + left(convert(varchar(13),d.fdate,108),8))[fdate],AP.Username [apusername],datediff(day,fdate,getdate())[Pending Days],m.priority from MMM_MST_DOC M with (nolock)  LEFT OUTER JOIN MMM_DOC_DTL D with (nolock) on D.tid=M.lasttid  LEFT OUTER JOIN MMM_MST_USER U with (nolock) ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP with (nolock) ON AP.uid=D.UserID where D.userid = " & Val(Session("UID").ToString()) & "  and " & ddlPendinggrdHdr.SelectedValue & " like '%" & Trim(txtPendinggrdval.Text) & "%' order by M.priority desc,m.tid desc"
    '            Else
    '                StrQry = "Select distinct  M.tid,documenttype,curstatus,U.username,(replace(convert(char(11),d.fdate,113),' ','-') + ' ' + left(convert(varchar(13),d.fdate,108),8))[fdate],AP.Username [apusername],datediff(day,fdate,getdate())[Pending Days],m.priority from MMM_MST_DOC M with (nolock)  LEFT OUTER JOIN MMM_DOC_DTL D with (nolock) on D.tid=M.lasttid  LEFT OUTER JOIN MMM_MST_USER U with (nolock) ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP with (nolock) ON AP.uid=D.UserID where D.userid = " & Val(Session("UID").ToString()) & " order by M.priority desc,m.tid desc"
    '            End If


    '            'If ddlPendinggrdVal.SelectedItem.Text = "--please Select--" Then
    '            '    StrQry = "Select distinct  M.tid,documenttype,curstatus,U.username,(replace(convert(char(11),d.fdate,113),' ','-') + ' ' + left(convert(varchar(13),d.fdate,108),8))[fdate],AP.Username [apusername],datediff(day,fdate,getdate())[Pending Days],m.priority from MMM_MST_DOC M with (nolock)  LEFT OUTER JOIN MMM_DOC_DTL D with (nolock) on D.tid=M.lasttid  LEFT OUTER JOIN MMM_MST_USER U with (nolock) ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP with (nolock) ON AP.uid=D.UserID where D.userid = " & Val(Session("UID").ToString()) & " order by M.priority desc,m.tid desc"
    '            '    'oda.SelectCommand.CommandText = "Select distinct  M.tid,documenttype,curstatus,U.username,convert(nvarchar,adate,106)[adate],AP.Username [apusername],datediff(day,fdate,getdate())[fdate] from MMM_MST_DOC M with (nolock)  LEFT OUTER JOIN MMM_DOC_DTL D with (nolock) on D.tid=M.lasttid  LEFT OUTER JOIN MMM_MST_USER U with (nolock) ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP with (nolock) ON AP.uid=D.UserID where D.userid = " & Val(Session("UID").ToString()) & ""
    '            'Else
    '            '    StrQry = "Select distinct  M.tid,documenttype,curstatus,U.username,(replace(convert(char(11),d.fdate,113),' ','-') + ' ' + left(convert(varchar(13),d.fdate,108),8))[fdate],AP.Username [apusername],datediff(day,fdate,getdate())[Pending Days],m.priority from MMM_MST_DOC M with (nolock)  LEFT OUTER JOIN MMM_DOC_DTL D with (nolock) on D.tid=M.lasttid  LEFT OUTER JOIN MMM_MST_USER U with (nolock) ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP with (nolock) ON AP.uid=D.UserID where D.userid = " & Val(Session("UID").ToString()) & "  and " & ddlPendinggrdHdr.SelectedValue & "='" & ddlPendinggrdVal.SelectedItem.Text & "' order by M.priority desc,m.tid desc"
    '            'End If
    '        End If

    '        'If StrQry.Length > 2 Then
    '        '    StrQry &= " Order by M.tid"
    '        'End If

    '        oda.SelectCommand.CommandText = StrQry

    '        Dim ds As New DataSet
    '        oda.Fill(ds, "pending")
    '        gvPending.DataSource = ds.Tables("pending")
    '        gvPending.DataBind()
    '        ViewState("count") = CInt(Math.Ceiling(ds.Tables("pending").Rows.Count / gvPending.PageSize))
    '        If Session("Grid") = "gvPending" Then
    '            currentPageNumberp = Val(Session("pageindex")) + 1
    '        End If
    '        lbltotpending.Text = Trim("Displaying page no. " & currentPageNumberp & " of total page no(s) " & ViewState("count"))
    '        'lbltotalpages.Text = ""
    '        'lbltotpending.Enabled = False
    '        'lnknextpending.Enabled = False
    '        'lnkprevpending.Enabled = False
    '    Catch ex As Exception
    '    Finally
    '        con.Close()
    '        con.Dispose()
    '        oda.Dispose()
    '    End Try

    'End Sub

    'Protected Sub bindPendingddl()
    '    Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
    '    Dim con As SqlConnection = New SqlConnection(conStr)
    '    Dim oda As New SqlDataAdapter("", con)
    '    oda.SelectCommand.CommandType = CommandType.Text

    '    If ddlPendinggrdHdr.SelectedItem.Text = "System Doc. ID" Then
    '        oda.SelectCommand.CommandText = "Select distinct  M." & ddlPendinggrdHdr.SelectedValue & "  from MMM_MST_DOC M with (nolock)  LEFT OUTER JOIN MMM_DOC_DTL D with (nolock) on D.tid=M.lasttid  LEFT OUTER JOIN MMM_MST_USER U with (nolock) ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP with (nolock) ON AP.uid=D.UserID where D.userid = " & Val(Session("UID").ToString()) & " order by m.tid"
    '    ElseIf ddlPendinggrdHdr.SelectedItem.Text = "Created By" Then
    '        oda.SelectCommand.CommandText = "Select distinct  u." & ddlPendinggrdHdr.SelectedValue & " from MMM_MST_DOC M with (nolock)  LEFT OUTER JOIN MMM_DOC_DTL D with (nolock) on D.tid=M.lasttid  LEFT OUTER JOIN MMM_MST_USER U with (nolock) ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP with (nolock) ON AP.uid=D.UserID where D.userid = " & Val(Session("UID").ToString()) & " order by u.username"
    '    ElseIf ddlPendinggrdHdr.SelectedItem.Text = "Current User" Then
    '        oda.SelectCommand.CommandText = "Select distinct AP.Username from MMM_MST_DOC M with (nolock)  LEFT OUTER JOIN MMM_DOC_DTL D with (nolock) on D.tid=M.lasttid  LEFT OUTER JOIN MMM_MST_USER U with (nolock) ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP with (nolock) ON AP.uid=D.UserID where D.userid = " & Val(Session("UID").ToString()) & " order by ap.username"
    '    ElseIf ddlPendinggrdHdr.SelectedItem.Text = "Pending Days" Then
    '        oda.SelectCommand.CommandText = "Select distinct datediff(day,fdate,getdate()) [Pending Days] from MMM_MST_DOC M with (nolock)  LEFT OUTER JOIN MMM_DOC_DTL D with (nolock) on D.tid=M.lasttid  LEFT OUTER JOIN MMM_MST_USER U with (nolock) ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP with (nolock) ON AP.uid=D.UserID where D.userid = " & Val(Session("UID").ToString()) & " order by [Pending Days]"
    '    ElseIf ddlPendinggrdHdr.SelectedItem.Text = "Received On" Then
    '        oda.SelectCommand.CommandText = "Select distinct replace(convert(char(11),d.fdate,113),' ','-')[fdate]  from MMM_MST_DOC M with (nolock)  LEFT OUTER JOIN MMM_DOC_DTL D with (nolock) on D.tid=M.lasttid  LEFT OUTER JOIN MMM_MST_USER U with (nolock) ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP with (nolock) ON AP.uid=D.UserID where D.userid = " & Val(Session("UID").ToString()) & " order by fdate"
    '    Else
    '        oda.SelectCommand.CommandText = "Select distinct  " & ddlPendinggrdHdr.SelectedValue & " from MMM_MST_DOC M with (nolock)  LEFT OUTER JOIN MMM_DOC_DTL D with (nolock) on D.tid=M.lasttid  LEFT OUTER JOIN MMM_MST_USER U with (nolock) ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP with (nolock) ON AP.uid=D.UserID where D.userid = " & Val(Session("UID").ToString()) & ""
    '    End If
    '    Dim ds As New DataTable
    '    oda.Fill(ds)
    '    ddlPendinggrdVal.Items.Clear()
    '    ddlPendinggrdVal.Items.Add("--please Select--")

    '    For j As Integer = 0 To ds.Rows.Count - 1
    '        ddlPendinggrdVal.Items.Add(ds.Rows(j).Item(0).ToString)
    '        ddlPendinggrdVal.Items(j + 1).Value = ds.Rows(j).Item(0).ToString
    '    Next
    'End Sub

    Protected Sub ddlPendinggrdVal_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlPendinggrdVal.SelectedIndexChanged
        'bindNeedActGrid()
    End Sub



    'Search filter for My Request
    Private Sub bindFilteMyReqddl()
        'Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        'Dim con As SqlConnection = New SqlConnection(conStr)
        'Dim oda As New SqlDataAdapter("", con)
        'Try
        '    oda.SelectCommand.CommandType = CommandType.Text
        '    oda.SelectCommand.CommandText = "Select distinct M.tid[System Doc. ID],documenttype[Subject],curstatus[Status],U.username[Created By],(replace(convert(char(11),d.fdate,113),' ','-') + ' ' + left(convert(varchar(13),d.fdate,108),8))[Received On] ,AP.Username [To] ,datediff(day,fdate,getdate())[Pending Days]   from MMM_MST_DOC M with (nolock)  LEFT OUTER JOIN MMM_DOC_DTL D with (nolock) on D.tid=M.lasttid  LEFT OUTER JOIN MMM_MST_USER U with (nolock) ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP with (nolock) ON AP.uid=D.UserID where M.oUid = " & Val(Session("UID").ToString()) & " and curstatus<> 'ARCHIVE' "
        '    'oda.SelectCommand.CommandText = "Select distinct M.tid[System Doc. ID],documenttype[Subject],curstatus[Status],U.username[Created By],adate[Applied On] ,AP.Username [To] ,datediff(day,fdate,getdate())[Pending From] from MMM_MST_DOC M with (nolock)  LEFT OUTER JOIN MMM_DOC_DTL D with (nolock) on D.tid=M.lasttid  LEFT OUTER JOIN MMM_MST_USER U with (nolock) ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP with (nolock) ON AP.uid=D.UserID where M.oUid = " & Val(Session("UID").ToString()) & " and curstatus<> 'ARCHIVE' order by M.Tid desc"
        '    Dim ds As New DataTable
        '    oda.Fill(ds)
        '    oda.SelectCommand.CommandText = "Select distinct M.tid,documenttype,curstatus,U.username,(replace(convert(char(11),d.fdate,113),' ','-') + ' ' + left(convert(varchar(13),d.fdate,108),8))[fdate] ,ap.username,datediff(day,fdate,getdate())[Pending Days]  from MMM_MST_DOC M with (nolock)  LEFT OUTER JOIN MMM_DOC_DTL D with (nolock) on D.tid=M.lasttid  LEFT OUTER JOIN MMM_MST_USER U with (nolock) ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP with (nolock) ON AP.uid=D.UserID where M.oUid = " & Val(Session("UID").ToString()) & " and curstatus<> 'ARCHIVE'  order by m.tid  "

        '    'oda.SelectCommand.CommandText = "Select distinct M.tid,documenttype,curstatus,U.username,adate ,AP.Username[To],datediff(day,fdate,getdate())[fdate] from MMM_MST_DOC M with (nolock)  LEFT OUTER JOIN MMM_DOC_DTL D with (nolock) on D.tid=M.lasttid  LEFT OUTER JOIN MMM_MST_USER U with (nolock) ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP with (nolock) ON AP.uid=D.UserID where M.oUid = " & Val(Session("UID").ToString()) & " and curstatus<> 'ARCHIVE' order by M.Tid desc"
        '    Dim ds1 As New DataTable
        '    oda.Fill(ds1)
        '    For i As Integer = 0 To ds.Columns.Count - 1
        '        ddlMyReqHdr.Items.Add(ds.Columns.Item(i).ToString)
        '        ddlMyReqHdr.Items(i).Value = ds1.Columns.Item(i).ToString
        '    Next
        'Catch ex As Exception
        'Finally
        '    con.Close()
        '    con.Dispose()
        '    oda.Dispose()
        'End Try

    End Sub

    Protected Sub ddlMyReqHdr_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlMyReqHdr.SelectedIndexChanged
        ' BindDashBoard()
        '   bindMyReqDDl()

        'ddlGrdVal.Items.Insert(0, "--Select Value-- ")

    End Sub
    '27 may
    'Private Sub bindMyReqGrid()
    '    Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
    '    Dim con As SqlConnection = New SqlConnection(conStr)
    '    Dim oda As New SqlDataAdapter("", con)
    '    oda.SelectCommand.CommandType = CommandType.Text
    '    Try
    '        If ddlMyReqHdr.SelectedItem.Text = "System Doc. ID" Then

    '            If txtmyreqval.Text <> "" Then
    '                oda.SelectCommand.CommandText = "Select distinct  M.tid,documenttype,curstatus,U.username,(replace(convert(char(11),d.fdate,113),' ','-') + ' ' + left(convert(varchar(13),d.fdate,108),8))[fdate],AP.Username [apusername],datediff(day,fdate,getdate())[Pending Days] from MMM_MST_DOC M with (nolock)  LEFT OUTER JOIN MMM_DOC_DTL D with (nolock) on D.tid=M.lasttid  LEFT OUTER JOIN MMM_MST_USER U with (nolock) ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP with (nolock) ON AP.uid=D.UserID where M.oUid = " & Val(Session("UID").ToString()) & " and curstatus<> 'ARCHIVE'  and M." & ddlMyReqHdr.SelectedValue & " like '%" & Trim(txtmyreqval.Text) & "%' "
    '            Else
    '                oda.SelectCommand.CommandText = "Select distinct  M.tid,documenttype,curstatus,U.username,(replace(convert(char(11),d.fdate,113),' ','-') + ' ' + left(convert(varchar(13),d.fdate,108),8))[fdate],AP.Username [apusername],datediff(day,fdate,getdate())[Pending Days] from MMM_MST_DOC M with (nolock)  LEFT OUTER JOIN MMM_DOC_DTL D with (nolock) on D.tid=M.lasttid  LEFT OUTER JOIN MMM_MST_USER U with (nolock) ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP with (nolock) ON AP.uid=D.UserID where M.oUid = " & Val(Session("UID").ToString()) & " and curstatus<> 'ARCHIVE'  "
    '            End If


    '            'If ddlMyReqVal.SelectedItem.Text = "--please Select--" Then
    '            '    oda.SelectCommand.CommandText = "Select distinct  M.tid,documenttype,curstatus,U.username,(replace(convert(char(11),d.fdate,113),' ','-') + ' ' + left(convert(varchar(13),d.fdate,108),8))[fdate],AP.Username [apusername],datediff(day,fdate,getdate())[Pending Days] from MMM_MST_DOC M with (nolock)  LEFT OUTER JOIN MMM_DOC_DTL D with (nolock) on D.tid=M.lasttid  LEFT OUTER JOIN MMM_MST_USER U with (nolock) ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP with (nolock) ON AP.uid=D.UserID where M.oUid = " & Val(Session("UID").ToString()) & " and curstatus<> 'ARCHIVE'  "
    '            'Else
    '            '    oda.SelectCommand.CommandText = "Select distinct  M.tid,documenttype,curstatus,U.username,(replace(convert(char(11),d.fdate,113),' ','-') + ' ' + left(convert(varchar(13),d.fdate,108),8))[fdate],AP.Username [apusername],datediff(day,fdate,getdate())[Pending Days] from MMM_MST_DOC M with (nolock)  LEFT OUTER JOIN MMM_DOC_DTL D with (nolock) on D.tid=M.lasttid  LEFT OUTER JOIN MMM_MST_USER U with (nolock) ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP with (nolock) ON AP.uid=D.UserID where M.oUid = " & Val(Session("UID").ToString()) & " and curstatus<> 'ARCHIVE'  and M." & ddlMyReqHdr.SelectedValue & "=" & ddlMyReqVal.SelectedItem.Text & " "
    '            'End If

    '        ElseIf ddlMyReqHdr.SelectedItem.Text = "Pending Days" Then
    '            If txtmyreqval.Text <> "" Then
    '                oda.SelectCommand.CommandText = "Select distinct  M.tid,documenttype,curstatus,U.username,(replace(convert(char(11),d.fdate,113),' ','-') + ' ' + left(convert(varchar(13),d.fdate,108),8))[fdate],AP.Username [apusername],datediff(day,fdate,getdate())[Pending Days] from MMM_MST_DOC M with (nolock)  LEFT OUTER JOIN MMM_DOC_DTL D with (nolock) on D.tid=M.lasttid  LEFT OUTER JOIN MMM_MST_USER U with (nolock) ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP with (nolock) ON AP.uid=D.UserID where M.oUid = " & Val(Session("UID").ToString()) & " and curstatus<> 'ARCHIVE'  and datediff(day,fdate,getdate()) like '%" & Trim(txtmyreqval.Text) & "%' "
    '            Else
    '                oda.SelectCommand.CommandText = "Select distinct  M.tid,documenttype,curstatus,U.username,(replace(convert(char(11),d.fdate,113),' ','-') + ' ' + left(convert(varchar(13),d.fdate,108),8))[fdate],AP.Username [apusername],datediff(day,fdate,getdate())[Pending Days] from MMM_MST_DOC M with (nolock)  LEFT OUTER JOIN MMM_DOC_DTL D with (nolock) on D.tid=M.lasttid  LEFT OUTER JOIN MMM_MST_USER U with (nolock) ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP with (nolock) ON AP.uid=D.UserID where M.oUid = " & Val(Session("UID").ToString()) & " "
    '            End If


    '            'If ddlMyReqVal.SelectedItem.Text = "--please Select--" Then
    '            '    oda.SelectCommand.CommandText = "Select distinct  M.tid,documenttype,curstatus,U.username,(replace(convert(char(11),d.fdate,113),' ','-') + ' ' + left(convert(varchar(13),d.fdate,108),8))[fdate],AP.Username [apusername],datediff(day,fdate,getdate())[Pending Days] from MMM_MST_DOC M with (nolock)  LEFT OUTER JOIN MMM_DOC_DTL D with (nolock) on D.tid=M.lasttid  LEFT OUTER JOIN MMM_MST_USER U with (nolock) ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP with (nolock) ON AP.uid=D.UserID where M.oUid = " & Val(Session("UID").ToString()) & " "
    '            'Else
    '            '    oda.SelectCommand.CommandText = "Select distinct  M.tid,documenttype,curstatus,U.username,(replace(convert(char(11),d.fdate,113),' ','-') + ' ' + left(convert(varchar(13),d.fdate,108),8))[fdate],AP.Username [apusername],datediff(day,fdate,getdate())[Pending Days] from MMM_MST_DOC M with (nolock)  LEFT OUTER JOIN MMM_DOC_DTL D with (nolock) on D.tid=M.lasttid  LEFT OUTER JOIN MMM_MST_USER U with (nolock) ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP with (nolock) ON AP.uid=D.UserID where M.oUid = " & Val(Session("UID").ToString()) & " and curstatus<> 'ARCHIVE'  and datediff(day,fdate,getdate()) = '" & ddlMyReqVal.SelectedItem.Text & "' "
    '            'End If


    '        ElseIf ddlMyReqHdr.SelectedItem.Text = "Received On" Then
    '            If txtmyreqval.Text <> "" Then
    '                oda.SelectCommand.CommandText = "Select distinct  M.tid,documenttype,curstatus,U.username,(replace(convert(char(11),d.fdate,113),' ','-') + ' ' + left(convert(varchar(13),d.fdate,108),8))[fdate],AP.Username [apusername],datediff(day,fdate,getdate())[Pending Days] from MMM_MST_DOC M with (nolock)  LEFT OUTER JOIN MMM_DOC_DTL D with (nolock) on D.tid=M.lasttid  LEFT OUTER JOIN MMM_MST_USER U with (nolock) ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP with (nolock) ON AP.uid=D.UserID where M.oUid = " & Val(Session("UID").ToString()) & " and curstatus<> 'ARCHIVE' and  replace(convert(char(11)," & ddlMyReqHdr.SelectedValue & ",113),' ','-')  like '%" & Trim(txtmyreqval.Text) & "%'"
    '            Else
    '                oda.SelectCommand.CommandText = "Select distinct  M.tid,documenttype,curstatus,U.username,(replace(convert(char(11),d.fdate,113),' ','-') + ' ' + left(convert(varchar(13),d.fdate,108),8))[fdate],AP.Username [apusername],datediff(day,fdate,getdate())[Pending Days] from MMM_MST_DOC M with (nolock)  LEFT OUTER JOIN MMM_DOC_DTL D with (nolock) on D.tid=M.lasttid  LEFT OUTER JOIN MMM_MST_USER U with (nolock) ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP with (nolock) ON AP.uid=D.UserID where M.oUid = " & Val(Session("UID").ToString()) & " and curstatus<> 'ARCHIVE'  "
    '            End If


    '            'If ddlMyReqVal.SelectedItem.Text = "--please Select--" Then
    '            '    oda.SelectCommand.CommandText = "Select distinct  M.tid,documenttype,curstatus,U.username,(replace(convert(char(11),d.fdate,113),' ','-') + ' ' + left(convert(varchar(13),d.fdate,108),8))[fdate],AP.Username [apusername],datediff(day,fdate,getdate())[Pending Days] from MMM_MST_DOC M with (nolock)  LEFT OUTER JOIN MMM_DOC_DTL D with (nolock) on D.tid=M.lasttid  LEFT OUTER JOIN MMM_MST_USER U with (nolock) ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP with (nolock) ON AP.uid=D.UserID where M.oUid = " & Val(Session("UID").ToString()) & " and curstatus<> 'ARCHIVE'  "
    '            'Else
    '            '    oda.SelectCommand.CommandText = "Select distinct  M.tid,documenttype,curstatus,U.username,(replace(convert(char(11),d.fdate,113),' ','-') + ' ' + left(convert(varchar(13),d.fdate,108),8))[fdate],AP.Username [apusername],datediff(day,fdate,getdate())[Pending Days] from MMM_MST_DOC M with (nolock)  LEFT OUTER JOIN MMM_DOC_DTL D with (nolock) on D.tid=M.lasttid  LEFT OUTER JOIN MMM_MST_USER U with (nolock) ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP with (nolock) ON AP.uid=D.UserID where M.oUid = " & Val(Session("UID").ToString()) & " and curstatus<> 'ARCHIVE' and  replace(convert(char(11)," & ddlMyReqHdr.SelectedValue & ",113),' ','-') = replace(convert(char(11),'" & ddlMyReqVal.SelectedItem.Text & "',113),' ','-')"
    '            'End If



    '        ElseIf ddlMyReqHdr.SelectedItem.Text = "To" Then
    '            If txtmyreqval.Text <> "" Then
    '                oda.SelectCommand.CommandText = "Select distinct  M.tid,documenttype,curstatus,U.username,(replace(convert(char(11),d.fdate,113),' ','-') + ' ' + left(convert(varchar(13),d.fdate,108),8))[fdate],AP.Username [apusername],datediff(day,fdate,getdate())[Pending Days] from MMM_MST_DOC M with (nolock)  LEFT OUTER JOIN MMM_DOC_DTL D with (nolock) on D.tid=M.lasttid  LEFT OUTER JOIN MMM_MST_USER U with (nolock) ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP with (nolock) ON AP.uid=D.UserID where M.oUid = " & Val(Session("UID").ToString()) & " and curstatus<> 'ARCHIVE'  and AP." & Left(ddlMyReqHdr.SelectedValue, ddlMyReqHdr.SelectedValue.Length - 1) & " like '%" & Trim(txtmyreqval.Text) & "%' "
    '            Else
    '                oda.SelectCommand.CommandText = "Select distinct  M.tid,documenttype,curstatus,U.username,(replace(convert(char(11),d.fdate,113),' ','-') + ' ' + left(convert(varchar(13),d.fdate,108),8))[fdate],AP.Username [apusername],datediff(day,fdate,getdate())[Pending Days] from MMM_MST_DOC M with (nolock)  LEFT OUTER JOIN MMM_DOC_DTL D with (nolock) on D.tid=M.lasttid  LEFT OUTER JOIN MMM_MST_USER U with (nolock) ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP with (nolock) ON AP.uid=D.UserID where M.oUid = " & Val(Session("UID").ToString()) & " and curstatus<> 'ARCHIVE' "
    '            End If


    '            'If ddlMyReqVal.SelectedItem.Text = "--please Select--" Then
    '            '    oda.SelectCommand.CommandText = "Select distinct  M.tid,documenttype,curstatus,U.username,(replace(convert(char(11),d.fdate,113),' ','-') + ' ' + left(convert(varchar(13),d.fdate,108),8))[fdate],AP.Username [apusername],datediff(day,fdate,getdate())[Pending Days] from MMM_MST_DOC M with (nolock)  LEFT OUTER JOIN MMM_DOC_DTL D with (nolock) on D.tid=M.lasttid  LEFT OUTER JOIN MMM_MST_USER U with (nolock) ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP with (nolock) ON AP.uid=D.UserID where M.oUid = " & Val(Session("UID").ToString()) & " and curstatus<> 'ARCHIVE' "
    '            'Else
    '            '    oda.SelectCommand.CommandText = "Select distinct  M.tid,documenttype,curstatus,U.username,(replace(convert(char(11),d.fdate,113),' ','-') + ' ' + left(convert(varchar(13),d.fdate,108),8))[fdate],AP.Username [apusername],datediff(day,fdate,getdate())[Pending Days] from MMM_MST_DOC M with (nolock)  LEFT OUTER JOIN MMM_DOC_DTL D with (nolock) on D.tid=M.lasttid  LEFT OUTER JOIN MMM_MST_USER U with (nolock) ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP with (nolock) ON AP.uid=D.UserID where M.oUid = " & Val(Session("UID").ToString()) & " and curstatus<> 'ARCHIVE'  and AP." & Left(ddlMyReqHdr.SelectedValue, ddlMyReqHdr.SelectedValue.Length - 1) & "='" & ddlMyReqVal.SelectedItem.Text & "' "
    '            'End If



    '        ElseIf ddlMyReqHdr.SelectedItem.Text = "Created By" Then

    '            If txtmyreqval.Text <> "" Then
    '                oda.SelectCommand.CommandText = "Select distinct  M.tid,documenttype,curstatus,U.username,(replace(convert(char(11),d.fdate,113),' ','-') + ' ' + left(convert(varchar(13),d.fdate,108),8))[fdate],AP.Username [apusername],datediff(day,fdate,getdate())[Pending Days] from MMM_MST_DOC M with (nolock)  LEFT OUTER JOIN MMM_DOC_DTL D with (nolock) on D.tid=M.lasttid  LEFT OUTER JOIN MMM_MST_USER U with (nolock) ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP with (nolock) ON AP.uid=D.UserID where M.oUid = " & Val(Session("UID").ToString()) & " and and u." & ddlMyReqHdr.SelectedValue & " like '%" & Trim(txtmyreqval.Text) & "%' and curstatus<> 'ARCHIVE' "
    '            Else
    '                oda.SelectCommand.CommandText = "Select distinct  M.tid,documenttype,curstatus,U.username,(replace(convert(char(11),d.fdate,113),' ','-') + ' ' + left(convert(varchar(13),d.fdate,108),8))[fdate],AP.Username [apusername],datediff(day,fdate,getdate())[Pending Days] from MMM_MST_DOC M with (nolock)  LEFT OUTER JOIN MMM_DOC_DTL D with (nolock) on D.tid=M.lasttid  LEFT OUTER JOIN MMM_MST_USER U with (nolock) ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP with (nolock) ON AP.uid=D.UserID where M.oUid = " & Val(Session("UID").ToString()) & " and curstatus<> 'ARCHIVE'"
    '            End If

    '            'If ddlMyReqVal.SelectedItem.Text = "--please Select--" Then
    '            '    oda.SelectCommand.CommandText = "Select distinct  M.tid,documenttype,curstatus,U.username,(replace(convert(char(11),d.fdate,113),' ','-') + ' ' + left(convert(varchar(13),d.fdate,108),8))[fdate],AP.Username [apusername],datediff(day,fdate,getdate())[Pending Days] from MMM_MST_DOC M with (nolock)  LEFT OUTER JOIN MMM_DOC_DTL D with (nolock) on D.tid=M.lasttid  LEFT OUTER JOIN MMM_MST_USER U with (nolock) ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP with (nolock) ON AP.uid=D.UserID where M.oUid = " & Val(Session("UID").ToString()) & " and curstatus<> 'ARCHIVE'"
    '            'Else
    '            '    oda.SelectCommand.CommandText = "Select distinct  M.tid,documenttype,curstatus,U.username,(replace(convert(char(11),d.fdate,113),' ','-') + ' ' + left(convert(varchar(13),d.fdate,108),8))[fdate],AP.Username [apusername],datediff(day,fdate,getdate())[Pending Days] from MMM_MST_DOC M with (nolock)  LEFT OUTER JOIN MMM_DOC_DTL D with (nolock) on D.tid=M.lasttid  LEFT OUTER JOIN MMM_MST_USER U with (nolock) ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP with (nolock) ON AP.uid=D.UserID where M.oUid = " & Val(Session("UID").ToString()) & " and curstatus<> 'ARCHIVE' "
    '            'End If

    '        Else

    '            If txtmyreqval.Text <> "" Then
    '                oda.SelectCommand.CommandText = "Select distinct  M.tid,documenttype,curstatus,U.username,(replace(convert(char(11),d.fdate,113),' ','-') + ' ' + left(convert(varchar(13),d.fdate,108),8))[fdate],AP.Username [apusername],datediff(day,fdate,getdate())[Pending Days] from MMM_MST_DOC M with (nolock)  LEFT OUTER JOIN MMM_DOC_DTL D with (nolock) on D.tid=M.lasttid  LEFT OUTER JOIN MMM_MST_USER U with (nolock) ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP with (nolock) ON AP.uid=D.UserID where M.oUid = " & Val(Session("UID").ToString()) & " and curstatus<> 'ARCHIVE'  and " & ddlMyReqHdr.SelectedValue & " like '%" & Trim(txtmyreqval.Text) & "%' "
    '            Else
    '                oda.SelectCommand.CommandText = "Select distinct  M.tid,documenttype,curstatus,U.username,(replace(convert(char(11),d.fdate,113),' ','-') + ' ' + left(convert(varchar(13),d.fdate,108),8))[fdate],AP.Username [apusername],datediff(day,fdate,getdate())[Pending Days] from MMM_MST_DOC M with (nolock)  LEFT OUTER JOIN MMM_DOC_DTL D with (nolock) on D.tid=M.lasttid  LEFT OUTER JOIN MMM_MST_USER U with (nolock) ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP with (nolock) ON AP.uid=D.UserID where M.oUid = " & Val(Session("UID").ToString()) & " and curstatus<> 'ARCHIVE'"
    '            End If


    '            'If ddlMyReqVal.SelectedItem.Text = "--please Select--" Then
    '            '    oda.SelectCommand.CommandText = "Select distinct  M.tid,documenttype,curstatus,U.username,(replace(convert(char(11),d.fdate,113),' ','-') + ' ' + left(convert(varchar(13),d.fdate,108),8))[fdate],AP.Username [apusername],datediff(day,fdate,getdate())[Pending Days] from MMM_MST_DOC M with (nolock)  LEFT OUTER JOIN MMM_DOC_DTL D with (nolock) on D.tid=M.lasttid  LEFT OUTER JOIN MMM_MST_USER U with (nolock) ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP with (nolock) ON AP.uid=D.UserID where M.oUid = " & Val(Session("UID").ToString()) & " and curstatus<> 'ARCHIVE'"
    '            'Else
    '            '    oda.SelectCommand.CommandText = "Select distinct  M.tid,documenttype,curstatus,U.username,(replace(convert(char(11),d.fdate,113),' ','-') + ' ' + left(convert(varchar(13),d.fdate,108),8))[fdate],AP.Username [apusername],datediff(day,fdate,getdate())[Pending Days] from MMM_MST_DOC M with (nolock)  LEFT OUTER JOIN MMM_DOC_DTL D with (nolock) on D.tid=M.lasttid  LEFT OUTER JOIN MMM_MST_USER U with (nolock) ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP with (nolock) ON AP.uid=D.UserID where M.oUid = " & Val(Session("UID").ToString()) & " and curstatus<> 'ARCHIVE'  and " & ddlMyReqHdr.SelectedValue & "='" & ddlMyReqVal.SelectedItem.Text & "' "
    '            'End If

    '        End If

    '        Dim ds As New DataSet
    '        oda.Fill(ds, "upload")
    '        gvMyUpload.DataSource = ds.Tables("upload")
    '        gvMyUpload.DataBind()


    '        ViewState("cntUpload") = CInt(Math.Ceiling(ds.Tables("upload").Rows.Count / gvMyUpload.PageSize))
    '        If Session("Grid") = "gvMyUpload" Then
    '            currentPageNumber = Val(Session("pageindex")) + 1
    '        End If
    '        lbltotup.Text = Trim("Displaying page no. " & currentPageNumber & " of total page no(s) " & ViewState("cntUpload"))
    '        'lbltotpending.Enabled = False
    '        'lnknextpending.Enabled = False
    '        'lnkprevpending.Enabled = False
    '        'lbltotalpages.Text = ""
    '    Catch ex As Exception
    '    Finally
    '        con.Close()
    '        con.Dispose()
    '        oda.Dispose()
    '    End Try

    'End Sub
    'Protected Sub ddlMyReqVal_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlMyReqVal.SelectedIndexChanged
    '    'bindMyReqGrid()

    'End Sub

    'Protected Sub bindMyReqDDl()
    '    Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
    '    Dim con As SqlConnection = New SqlConnection(conStr)
    '    Dim oda As New SqlDataAdapter("", con)
    '    oda.SelectCommand.CommandType = CommandType.Text
    '    If ddlMyReqHdr.SelectedItem.Text = "System Doc. ID" Then
    '        oda.SelectCommand.CommandText = "Select distinct  M." & ddlMyReqHdr.SelectedValue & "  from MMM_MST_DOC M with (nolock)  LEFT OUTER JOIN MMM_DOC_DTL D with (nolock) on D.tid=M.lasttid  LEFT OUTER JOIN MMM_MST_USER U with (nolock) ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP with (nolock) ON AP.uid=D.UserID where M.oUid = " & Val(Session("UID").ToString()) & " and curstatus<> 'ARCHIVE' order by m.tid desc "
    '    ElseIf ddlMyReqHdr.SelectedItem.Text = "Created By" Then
    '        oda.SelectCommand.CommandText = "Select distinct  u." & ddlMyReqHdr.SelectedValue & " from MMM_MST_DOC M with (nolock)  LEFT OUTER JOIN MMM_DOC_DTL D with (nolock) on D.tid=M.lasttid  LEFT OUTER JOIN MMM_MST_USER U with (nolock) ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP with (nolock) ON AP.uid=D.UserID where M.oUid = " & Val(Session("UID").ToString()) & " and curstatus<> 'ARCHIVE' order by u.username  "

    '    ElseIf ddlMyReqHdr.SelectedItem.Text = "To" Then
    '        oda.SelectCommand.CommandText = "Select distinct AP.Username from MMM_MST_DOC M with (nolock)  LEFT OUTER JOIN MMM_DOC_DTL D with (nolock) on D.tid=M.lasttid  LEFT OUTER JOIN MMM_MST_USER U with (nolock) ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP with (nolock) ON AP.uid=D.UserID where M.oUid = " & Val(Session("UID").ToString()) & " and curstatus<> 'ARCHIVE' "
    '    ElseIf ddlMyReqHdr.SelectedItem.Text = "Pending Days" Then

    '        oda.SelectCommand.CommandText = "Select distinct datediff(day,fdate,getdate())[Pending Days] from MMM_MST_DOC M with (nolock)  LEFT OUTER JOIN MMM_DOC_DTL D with (nolock) on D.tid=M.lasttid  LEFT OUTER JOIN MMM_MST_USER U with (nolock) ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP with (nolock) ON AP.uid=D.UserID where M.oUid = " & Val(Session("UID").ToString()) & " and curstatus<> 'ARCHIVE' "
    '    ElseIf ddlMyReqHdr.SelectedItem.Text = "Received On" Then
    '        oda.SelectCommand.CommandText = "Select distinct (replace(convert(char(11),d.fdate,113),' ','-'))[fdate] from MMM_MST_DOC M with (nolock)  LEFT OUTER JOIN MMM_DOC_DTL D with (nolock) on D.tid=M.lasttid  LEFT OUTER JOIN MMM_MST_USER U with (nolock) ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP with (nolock) ON AP.uid=D.UserID where M.oUid = " & Val(Session("UID").ToString()) & " and curstatus<> 'ARCHIVE' "
    '    Else
    '        oda.SelectCommand.CommandText = "Select distinct  " & ddlMyReqHdr.SelectedValue & " from MMM_MST_DOC M with (nolock)  LEFT OUTER JOIN MMM_DOC_DTL D with (nolock) on D.tid=M.lasttid  LEFT OUTER JOIN MMM_MST_USER U with (nolock) ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP with (nolock) ON AP.uid=D.UserID where M.oUid = " & Val(Session("UID").ToString()) & " and curstatus<> 'ARCHIVE' "
    '    End If
    '    Dim ds As New DataTable
    '    oda.Fill(ds)
    '    ddlMyReqVal.Items.Clear()
    '    ddlMyReqVal.Items.Add("--please Select--")
    '    For j As Integer = 0 To ds.Rows.Count - 1
    '        ddlMyReqVal.Items.Add(ds.Rows(j).Item(0).ToString)
    '        ddlMyReqVal.Items(j + 1).Value = ds.Rows(j).Item(0).ToString
    '    Next
    'End Sub


    'Private Sub bindDDlSyDocIDHistory()
    '    Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
    '    Dim con As SqlConnection = New SqlConnection(conStr)
    '    Dim oda As New SqlDataAdapter("", con)
    '    Try
    '        oda.SelectCommand.CommandType = CommandType.Text
    '        If ddlGrdHdr.SelectedItem.Text = "System Doc. ID" Then
    '            '' changed for remove archive 13-dec - sp
    '            oda.SelectCommand.CommandText = "Select distinct  M." & ddlGrdHdr.SelectedValue & " from MMM_MST_DOC M with (nolock)  LEFT OUTER JOIN MMM_DOC_DTL D with (nolock) on D.docid =M.tid  LEFT OUTER JOIN MMM_MST_USER U with (nolock) ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP with (nolock) ON AP.uid=D.UserID where D.UserID = " & Val(Session("UID").ToString()) & " and aprstatus is not null and m.curstatus <> 'ARCHIVE'  "
    '        End If
    '        Dim ds As New DataTable
    '        oda.Fill(ds)
    '        ddlGrdVal.Items.Clear()
    '        For j As Integer = 0 To ds.Rows.Count - 1
    '            ddlGrdVal.Items.Add(ds.Rows(j).Item(0).ToString)
    '        Next
    '    Catch ex As Exception
    '    Finally
    '        con.Close()
    '        con.Dispose()
    '        oda.Dispose()
    '    End Try
    'End Sub
    Private Sub bindDDlSyDocIDNeedact()
        'Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        'Dim con As SqlConnection = New SqlConnection(conStr)
        'Dim oda As New SqlDataAdapter("", con)
        'oda.SelectCommand.CommandType = CommandType.Text
        'If ddlPendinggrdHdr.SelectedItem.Text = "System Doc. ID" Then
        '    oda.SelectCommand.CommandText = "Select distinct  M." & ddlPendinggrdHdr.SelectedValue & "  from MMM_MST_DOC M with (nolock)  LEFT OUTER JOIN MMM_DOC_DTL D with (nolock) on D.tid=M.lasttid  LEFT OUTER JOIN MMM_MST_USER U with (nolock) ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP with (nolock) ON AP.uid=D.UserID where D.userid = " & Val(Session("UID").ToString()) & ""
        'End If
        'Dim ds As New DataTable
        'oda.Fill(ds)
        'ddlPendinggrdVal.Items.Clear()
        'For j As Integer = 0 To ds.Rows.Count - 1
        '    ddlPendinggrdVal.Items.Add(ds.Rows(j).Item(0).ToString)
        'Next
    End Sub
    Private Sub bindDDlSyDocIDMyReq()
        'Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        'Dim con As SqlConnection = New SqlConnection(conStr)
        'Dim oda As New SqlDataAdapter("", con)
        'oda.SelectCommand.CommandType = CommandType.Text
        'If ddlMyReqHdr.SelectedItem.Text = "System Doc. ID" Then
        '    oda.SelectCommand.CommandText = "Select distinct  M." & ddlMyReqHdr.SelectedValue & "  from MMM_MST_DOC M with (nolock)  LEFT OUTER JOIN MMM_DOC_DTL D with (nolock) on D.tid=M.lasttid  LEFT OUTER JOIN MMM_MST_USER U with (nolock) ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP with (nolock) ON AP.uid=D.UserID where M.oUid = " & Val(Session("UID").ToString()) & " and curstatus<> 'ARCHIVE' "
        'End If
        'Dim ds As New DataTable
        'oda.Fill(ds)
        'ddlMyReqVal.Items.Clear()
        'For j As Integer = 0 To ds.Rows.Count - 1
        '    ddlMyReqVal.Items.Add(ds.Rows(j).Item(0).ToString)
        'Next
    End Sub
    Protected Sub BindDropdownoncondition(ByVal isclear As Integer)
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
        'With(nolock) added by Himank on 29th sep 2015
        'oda.SelectCommand.CommandText = " select ShowStaticfieldonDashBoard,OrderOfDeshBoardFields from mmm_mst_entity  WITH(NOLOCK)   where eid=" & Session("EID") & ""
        'Dim dt As New DataTable
        'oda.Fill(dt)
        'Changes for Caparo
        oda.SelectCommand.CommandText = "select * from mmm_mst_needtoact_config  where eid=" & Session("EID") & "  and docType='" & ViewState("doctype") & "' "
        Dim dtact As New DataTable
        oda.Fill(dtact)
        If dtact.Rows.Count > 0 Then
            If isclear = 0 Then
                If ddlPendinggrdHdr.Items.Count > 0 Then
                    ddlPendinggrdHdr.Items.Clear()
                End If
            End If

            ddlPendinggrdHdr.Items.Insert(0, "SELECT")
            ddlPendinggrdHdr.Items.Insert(1, "DOC ID")
            If isclear = 0 Then
                If ddlMyReqHdr.Items.Count > 0 Then
                    ddlMyReqHdr.Items.Clear()
                End If
            End If

            ddlMyReqHdr.Items.Insert(0, "SELECT")
            ddlMyReqHdr.Items.Insert(1, "DOC ID")
            If isclear = 0 Then
                If ddlGrdHdr.Items.Count > 0 Then
                    ddlGrdHdr.Items.Clear()
                End If
            End If

            ddlGrdHdr.Items.Insert(0, "SELECT")
            ddlGrdHdr.Items.Insert(1, "DOC ID")
            If isclear = 0 Then
                If ddlDraftHdr.Items.Count > 0 Then
                    ddlDraftHdr.Items.Clear()
                End If
            End If

            ddlDraftHdr.Items.Insert(0, "SELECT")
            ddlDraftHdr.Items.Insert(1, "DOC ID")
        Else
            If ddlPendinggrdHdr.Items.Count > 0 Then
                ddlPendinggrdHdr.Items.Clear()
            End If
            ddlPendinggrdHdr.Items.Insert(0, "SELECT")
            ddlPendinggrdHdr.Items.Insert(1, "RECIEVED ON")
            ddlPendinggrdHdr.Items.Insert(2, "DOC ID")
            ddlPendinggrdHdr.Items.Insert(3, "CREATED BY")
            ddlPendinggrdHdr.Items.Insert(3, "CURRENT STATUS")
            If ddlMyReqHdr.Items.Count > 0 Then
                ddlMyReqHdr.Items.Clear()
            End If
            ddlMyReqHdr.Items.Insert(0, "SELECT")
            ddlMyReqHdr.Items.Insert(1, "RECIEVED ON")
            ddlMyReqHdr.Items.Insert(2, "DOC ID")
            ddlMyReqHdr.Items.Insert(3, "CREATED BY")
            ddlMyReqHdr.Items.Insert(3, "CURRENT STATUS")
            If ddlGrdHdr.Items.Count > 0 Then
                ddlGrdHdr.Items.Clear()
            End If
            ddlGrdHdr.Items.Insert(0, "SELECT")
            ddlGrdHdr.Items.Insert(1, "RECIEVED ON")
            ddlGrdHdr.Items.Insert(2, "DOC ID")
            ddlGrdHdr.Items.Insert(3, "CREATED BY")
            ddlGrdHdr.Items.Insert(3, "CURRENT STATUS")
            ddlDraftHdr.Items.Insert(0, "SELECT")
            'ddlDraftHdr.Items.Insert(1, "RECIEVED ON")
            If ddlDraftHdr.Items.Count > 0 Then
                ddlDraftHdr.Items.Clear()
            End If
            ddlDraftHdr.Items.Insert(1, "DOC ID")
            ddlDraftHdr.Items.Insert(2, "CREATED BY")
            ddlDraftHdr.Items.Insert(3, "CURRENT STATUS")
        End If

    End Sub
    Protected Sub ddldynamic_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddldynamic.SelectedIndexChanged
        Session("Pending") = Nothing
        If ddldynamic.SelectedItem.Text = "SELECT ALL" Then
            PendingGrdBind()
            MyRequestBind()
            MyActionGRdBind()
            MyDRAFTGRdBind()
        Else
            Dim fmname As String = Trim(ddldynamic.SelectedValue)
            ViewState("doctype") = fmname
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As New SqlConnection(conStr)
            Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
            ' code for binding drop down DDlPending 
            Try
                PendingGrdBind()
                MyRequestBind()
                MyActionGRdBind()
                MyDRAFTGRdBind()
                'With(nolock) added by Himank on 29th sep 2015
                oda.SelectCommand.CommandText = "select upper(displayname)[displayname],fieldmapping from MMM_MST_Fields   WITH(NOLOCK)  where eid='" & Session("EID") & "' and documenttype='" & fmname & "' and showonDashBoard=1 "
                Dim dt As New DataTable
                oda.Fill(dt)
                oda.SelectCommand.CommandText = "select Upper(fieldname) displayname,upper(fieldMapping) fieldmapping from  mmm_mst_needtoact_config where eid=" & Session("EID") & " and docType='" & fmname & "' order by displayorder"
                Dim dt1 As New DataTable
                oda.Fill(dt1)
                If dt1.Rows.Count > 0 Then
                    ddlPendinggrdHdr.DataSource = dt1
                    ddlPendinggrdHdr.DataTextField = "displayname"
                    ddlPendinggrdHdr.DataValueField = "fieldmapping"
                    ddlPendinggrdHdr.DataBind()

                    ddlMyReqHdr.DataSource = dt1
                    ddlMyReqHdr.DataTextField = "displayname"
                    ddlMyReqHdr.DataValueField = "fieldmapping"
                    ddlMyReqHdr.DataBind()

                    ddlGrdHdr.DataSource = dt1
                    ddlGrdHdr.DataTextField = "displayname"
                    ddlGrdHdr.DataValueField = "fieldmapping"
                    ddlGrdHdr.DataBind()


                    ddlDraftHdr.DataSource = dt1
                    ddlDraftHdr.DataTextField = "displayname"
                    ddlDraftHdr.DataValueField = "fieldmapping"
                    ddlDraftHdr.DataBind()
                Else
                    ddlPendinggrdHdr.DataSource = dt
                    ddlPendinggrdHdr.DataTextField = "displayname"
                    ddlPendinggrdHdr.DataValueField = "fieldmapping"
                    ddlPendinggrdHdr.DataBind()

                    ddlMyReqHdr.DataSource = dt
                    ddlMyReqHdr.DataTextField = "displayname"
                    ddlMyReqHdr.DataValueField = "fieldmapping"
                    ddlMyReqHdr.DataBind()

                    ddlGrdHdr.DataSource = dt
                    ddlGrdHdr.DataTextField = "displayname"
                    ddlGrdHdr.DataValueField = "fieldmapping"
                    ddlGrdHdr.DataBind()


                    ddlDraftHdr.DataSource = dt
                    ddlDraftHdr.DataTextField = "displayname"
                    ddlDraftHdr.DataValueField = "fieldmapping"
                    ddlDraftHdr.DataBind()
                End If
                BindDropdownoncondition(1)


                oda.Dispose()
                dt.Dispose()

            Catch ex As Exception
            Finally
                con.Dispose()
                con.Close()
            End Try
        End If
    End Sub


    ' my draft
    Protected Sub printHit(ByVal sender As Object, ByVal e As EventArgs)
        Dim imgbtn As ImageButton = TryCast(sender, ImageButton)
        Dim row As GridViewRow = CType(imgbtn.NamingContainer, GridViewRow)
        Dim docid As Integer = Convert.ToString(Me.gvDraft.DataKeys(row.RowIndex).Value)
        'Response.Redirect("viewpdf.aspx?docid=" & docid & "&mod=draft")
        Dim doctype As String = row.Cells(2).Text.Trim()
        'Call PrintDOC(doctype, docid)
        Dim aa As New DMSUtil
        Dim str As String = aa.GetDraftPDF(Session("EID"), docid)

        If str = "fail" Then
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "popup", "alert('Print Template is not available to Print Draft, Please contact Admin!');", True)
            Exit Sub
        End If


        Dim link As String = "https://myndsaas.com" & str   ' for live url

        Dim s As String = "window.open('" & link + "', 'alert', 'width=600,height=400,left=100,top=100,resizable=yes');"
        ScriptManager.RegisterStartupScript(Me, Me.GetType(), "script", s, True)

    End Sub
    Protected Sub DocDiscardHit(ByVal sender As Object, ByVal e As EventArgs)
        Dim imgbtn As ImageButton = TryCast(sender, ImageButton)
        Dim row As GridViewRow = CType(imgbtn.NamingContainer, GridViewRow)
        Dim docid As Integer = Convert.ToString(Me.gvDraft.DataKeys(row.RowIndex).Value)
        ViewState("docid") = docid
        lblMsg.Text = "Are you sure to delete this DraftID:  " & ViewState("docid")
        updMsg.Update()
        btnConfirm.Show()

    End Sub


    Protected Sub EditHit(ByVal sender As Object, ByVal e As EventArgs)
        Dim imgbtn As ImageButton = TryCast(sender, ImageButton)
        Dim row As GridViewRow = CType(imgbtn.NamingContainer, GridViewRow)
        Dim docid As Integer = Convert.ToString(Me.gvDraft.DataKeys(row.RowIndex).Value)
        Session("DRAFT") = "ISDRAFT"
        Dim doctype As String = row.Cells(2).Text.Trim()
        Response.Redirect("Documents.Aspx?SC=" & doctype.ToString() & "&tid=" & Val(docid) & "")

    End Sub


    ''Public Sub PrintDOC(ByVal doctype As String, ByVal DocID As Integer)
    ''    Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
    ''    Dim con As New SqlConnection(conStr)
    ''    Dim da As New SqlDataAdapter("select * from MMM_Print_Template where documenttype='" & doctype & "' and draft='draft'", con)
    ''    Dim ds As New DataSet
    ''    da.Fill(ds, "data")
    ''    If ds.Tables("data").Rows.Count <> 1 Then
    ''        Response.Redirect("docdetail.aspx?DOCID=" & DocID & " &Draft=TRUE", True)
    ''    End If

    ''    Dim body As String = ds.Tables("data").Rows(0).Item("body").ToString()
    ''    Dim MainQry As String = ds.Tables("data").Rows(0).Item("qry").ToString()
    ''    Dim childQry As String = ds.Tables("data").Rows(0).Item("SQL_CHILDITEM").ToString()
    ''    'Dim DocType As String = ds.Tables("data").Rows(0).Item("Documenttype").ToString()

    ''    da.SelectCommand.CommandText = MainQry.Replace("@tid", DocID)
    ''    da.Fill(ds, "main")

    ''    For j As Integer = 0 To ds.Tables("main").Columns.Count - 1
    ''        body = body.Replace("[" & ds.Tables("main").Columns(j).ColumnName & "]", ds.Tables("main").Rows(0).Item(j).ToString())
    ''    Next

    ''    da.SelectCommand.CommandText = childQry.Replace("@tid", DocID)
    ''    da.Fill(ds, "child")

    ''    ds.Dispose()
    ''    con.Dispose()

    ''    Dim strChildItem As String = "<div><table width=""100%"" border=""0.5""  >"
    ''    Dim prevVal As String = ""
    ''    For i As Integer = 0 To ds.Tables("child").Rows.Count - 1
    ''        If prevVal = ds.Tables("child").Rows(i).Item(0).ToString() Then
    ''            prevVal = ds.Tables("child").Rows(i).Item(0).ToString()
    ''            ds.Tables("child").Rows(i).Item(0) = ""
    ''        Else
    ''            prevVal = ds.Tables("child").Rows(i).Item(0).ToString()
    ''        End If
    ''    Next

    ''    For i As Integer = 0 To ds.Tables("child").Rows.Count
    ''        strChildItem &= "<tr>"
    ''        For j As Integer = 0 To ds.Tables("child").Columns.Count - 1
    ''            strChildItem &= "<td>"
    ''            If i = 0 Then
    ''                strChildItem &= ds.Tables("child").Columns(j).ColumnName
    ''            Else
    ''                strChildItem &= ds.Tables("child").Rows(i - 1).Item(j).ToString()
    ''            End If
    ''            strChildItem &= "</td>"
    ''        Next
    ''        strChildItem &= "</tr>"
    ''    Next
    ''    strChildItem &= "</table></div>"

    ''    body = body.Replace("[child item]", strChildItem)
    ''    Dim pnl As New Panel
    ''    pnl.Controls.Add(New LiteralControl(body))

    ''    Dim attachment As String = "attachment; filename=" & DocType & "_" & DocID & ".pdf"
    ''    System.Web.HttpContext.Current.Response.ClearContent()
    ''    HttpContext.Current.Response.AddHeader("content-disposition", attachment)
    ''    HttpContext.Current.Response.ContentType = "application/pdf"

    ''    pnl.Font.Size = 7
    ''    Dim stw As StringWriter = New StringWriter()
    ''    Dim htextw As HtmlTextWriter = New HtmlTextWriter(stw)
    ''    pnl.RenderControl(htextw)
    ''    Dim document As iTextSharp.text.Document = New iTextSharp.text.Document()
    ''    ' Dim font As New iTextSharp.text.Font(iTextSharp.text.FontFactory.GetFont("", 8, iTextSharp.text.Font.NORMAL, New iTextSharp.text.BaseColor(80, 80, 80)))
    ''    PdfWriter.GetInstance(document, HttpContext.Current.Response.OutputStream)
    ''    document.Open()
    ''    'document.Add(iTextSharp.text.FontFactory.GetFont("Verdana", 8, iTextSharp.text.Font.NORMAL, New iTextSharp.text.BaseColor(80, 80, 80)))
    ''    ' document.Add(New Phrase("Report Name: ", FontFactory.GetFont("Verdana", 8, iTextSharp.text.Font.BOLD, New iTextSharp.text.BaseColor(21, 66, 157))))
    ''    Dim Str As StringReader = New StringReader(stw.ToString())
    ''    Dim htmlworker As iTextSharp.text.html.simpleparser.HTMLWorker = New iTextSharp.text.html.simpleparser.HTMLWorker(document)
    ''    htmlworker.Parse(Str)
    ''    document.Close()

    ''    HttpContext.Current.Response.Write(document)
    ''    HttpContext.Current.Response.End()
    ''End Sub
    Public Sub ConfirmDelete()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim da As New SqlDataAdapter("delete MMM_MST_DOC_DRAFT where eid=" & Session("EID") & " and tid=" & ViewState("docid") & " ", con)
        da.SelectCommand.CommandType = CommandType.Text
        If con.State <> ConnectionState.Open Then
            con.Open()
        End If
        da.SelectCommand.ExecuteNonQuery()

        btnConfirm.Hide()
        BindDashBoard()
        ScriptManager.RegisterStartupScript(Me, Me.GetType(), "popup", "alert('Deleted SuccessFully!!!!!!!!');window.location='mainhome.aspx'", True)
        con.Close()
        da.Dispose()

    End Sub


    Private Sub bindddlMydraft()
        'Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        'Dim con As SqlConnection = New SqlConnection(conStr)
        'Dim oda As New SqlDataAdapter("", con)
        'oda.SelectCommand.CommandType = CommandType.Text
        'oda.SelectCommand.CommandText = "Select distinct M.tid[System Doc. ID],documenttype[Subject],curstatus[Status],U.username[Created By],adate[Applied On] ,AP.Username [Current User] ,datediff(day,fdate,getdate())[Pending From]   from MMM_MST_DOC_DRAFT M  LEFT OUTER JOIN MMM_DOC_DTL D on D.tid=M.lasttid  LEFT OUTER JOIN MMM_MST_USER U ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP ON AP.uid=D.UserID where D.userid = " & Val(Session("UID").ToString()) & ""
        ''oda.SelectCommand.CommandText = "Select distinct M.tid[System Doc. ID],documenttype[Subject],curstatus[Status],U.username[Created By],adate[Applied On] ,AP.Username [To] ,datediff(day,fdate,getdate())[Pending From] from MMM_MST_DOC M  LEFT OUTER JOIN MMM_DOC_DTL D on D.tid=M.lasttid  LEFT OUTER JOIN MMM_MST_USER U ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP ON AP.uid=D.UserID where M.oUid = " & Val(Session("UID").ToString()) & " and curstatus<> 'ARCHIVE' order by M.Tid desc"
        'Dim ds As New DataTable
        'oda.Fill(ds)
        'oda.SelectCommand.CommandText = "Select distinct M.tid,documenttype,curstatus,U.username,adate ,AP.Username[Current User],datediff(day,fdate,getdate())[fdate]  from MMM_MST_DOC_draft M  LEFT OUTER JOIN MMM_DOC_DTL D on D.tid=M.lasttid  LEFT OUTER JOIN MMM_MST_USER U ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP ON AP.uid=D.UserID where D.userid = " & Val(Session("UID").ToString()) & ""

        ''oda.SelectCommand.CommandText = "Select distinct M.tid,documenttype,curstatus,U.username,adate ,AP.Username[To],datediff(day,fdate,getdate())[fdate] from MMM_MST_DOC M  LEFT OUTER JOIN MMM_DOC_DTL D on D.tid=M.lasttid  LEFT OUTER JOIN MMM_MST_USER U ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP ON AP.uid=D.UserID where M.oUid = " & Val(Session("UID").ToString()) & " and curstatus<> 'ARCHIVE' order by M.Tid desc"
        'Dim ds1 As New DataTable
        'oda.Fill(ds1)
        'For i As Integer = 0 To ds.Columns.Count - 1
        '    ddlDraftHdr.Items.Add(ds.Columns.Item(i).ToString)
        '    ddlDraftHdr.Items(i).Value = ds1.Columns.Item(i).ToString
        'Next
    End Sub

    Protected Sub ddlDraftHdr_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlDraftHdr.SelectedIndexChanged
        BindDashBoard()
        bindDraftDdl()
    End Sub
    Protected Sub bindDraftDdl()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As New SqlDataAdapter("", con)
        oda.SelectCommand.CommandType = CommandType.Text
        'With(nolock) added by Himank on 29th sep 2015
        If ddlDraftHdr.SelectedItem.Text = "System Doc. ID" Then
            oda.SelectCommand.CommandText = "Select distinct  M." & ddlDraftHdr.SelectedValue & "  from MMM_MST_DOC_DRAFT M    WITH(NOLOCK)  LEFT OUTER JOIN MMM_DOC_DTL D on D.tid=M.lasttid  LEFT OUTER JOIN MMM_MST_USER U  WITH(NOLOCK)   ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP  WITH(NOLOCK)   ON AP.uid=D.UserID where D.userid = " & Val(Session("UID").ToString()) & " order by m.tid"
        ElseIf ddlDraftHdr.SelectedItem.Text = "Created By" Then
            oda.SelectCommand.CommandText = "Select distinct  u." & ddlDraftHdr.SelectedValue & " from MMM_MST_DOC_Draft M   WITH(NOLOCK)   LEFT OUTER JOIN MMM_DOC_DTL D  WITH(NOLOCK)   on D.tid=M.lasttid  LEFT OUTER JOIN MMM_MST_USER U  WITH(NOLOCK)   ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP  WITH(NOLOCK)   ON AP.uid=D.UserID where D.userid = " & Val(Session("UID").ToString()) & " order by u.username"
        ElseIf ddlDraftHdr.SelectedItem.Text = "Current User" Then
            oda.SelectCommand.CommandText = "Select distinct AP.Username from MMM_MST_DOC_Draft M   WITH(NOLOCK)   LEFT OUTER JOIN MMM_DOC_DTL D  WITH(NOLOCK)   on D.tid=M.lasttid  LEFT OUTER JOIN MMM_MST_USER U  WITH(NOLOCK)   ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP  WITH(NOLOCK)   ON AP.uid=D.UserID where D.userid = " & Val(Session("UID").ToString()) & " order by ap.username"
        ElseIf ddlDraftHdr.SelectedItem.Text = "Pending From" Then
            oda.SelectCommand.CommandText = "Select distinct datediff(day,fdate,getdate()) [fdate] from MMM_MST_DOC_draft M   WITH(NOLOCK)   LEFT OUTER JOIN MMM_DOC_DTL D  WITH(NOLOCK)   on D.tid=M.lasttid  LEFT OUTER JOIN MMM_MST_USER U  WITH(NOLOCK)   ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP   WITH(NOLOCK)  ON AP.uid=D.UserID where D.userid = " & Val(Session("UID").ToString()) & " order by fdate"
        ElseIf ddlDraftHdr.SelectedItem.Text = "Applied On" Then
            oda.SelectCommand.CommandText = "Select distinct convert(nvarchar,adate,23) [adate] from MMM_MST_DOC_draft M   WITH(NOLOCK)   LEFT OUTER JOIN MMM_DOC_DTL D  WITH(NOLOCK)   on D.tid=M.lasttid  LEFT OUTER JOIN MMM_MST_USER U  WITH(NOLOCK)   ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP  WITH(NOLOCK)   ON AP.uid=D.UserID where D.userid = " & Val(Session("UID").ToString()) & " order by adate"
        Else
            oda.SelectCommand.CommandText = "Select distinct  " & ddlDraftHdr.SelectedValue & " from MMM_MST_DOC_draft M  WITH(NOLOCK)    LEFT OUTER JOIN MMM_DOC_DTL D  WITH(NOLOCK)   on D.tid=M.lasttid  LEFT OUTER JOIN MMM_MST_USER U  WITH(NOLOCK)   ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP  WITH(NOLOCK)   ON AP.uid=D.UserID where D.userid = " & Val(Session("UID").ToString()) & ""
        End If
        'Dim ds As New DataTable
        'oda.Fill(ds)
        'ddlDraftVal.Items.Clear()
        'ddlDraftVal.Items.Add("--please Select--")

        'For j As Integer = 0 To ds.Rows.Count - 1
        '    ddlDraftVal.Items.Add(ds.Rows(j).Item(0).ToString)
        '    ddlDraftVal.Items(j + 1).Value = ds.Rows(j).Item(0).ToString
        'Next
    End Sub

    Private Sub bindDDLsysDOCidDraft()
        'Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        'Dim con As SqlConnection = New SqlConnection(conStr)
        'Dim oda As New SqlDataAdapter("", con)
        'Dim str As String = String.Empty

        'If ddlPendinggrdHdr.SelectedItem.Text = "System Doc. ID" Then
        '    str = "Select distinct  M." & ddlDraftHdr.SelectedValue & "  from MMM_MST_DOC_DRAFT M  LEFT OUTER JOIN MMM_DOC_DTL D on D.tid=M.lasttid  LEFT OUTER JOIN MMM_MST_USER U ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP ON AP.uid=D.UserID where D.userid = " & Val(Session("UID").ToString()) & ""
        'End If

        'oda.SelectCommand.CommandText = str
        'oda.SelectCommand.CommandType = CommandType.Text
        'Dim ds As New DataTable
        'oda.Fill(ds)
        'ddlPendinggrdVal.Items.Clear()
        'For j As Integer = 0 To ds.Rows.Count - 1
        '    ddlDraftVal.Items.Add(ds.Rows(j).Item(0).ToString)
        'Next
    End Sub
    'mydraft

    Protected Sub btndraftval_Click(sender As Object, e As EventArgs) Handles btndraftval.Click
        Dim flag As Char = "0"
        Dim datatable As DataTable = ViewState("MyDraft")
        Dim DBDT As DataView = datatable.DefaultView
        Select Case ddlDraftHdr.SelectedItem.Text.ToUpper()
            Case "SELECT"
                Exit Sub
            Case "RECIEVED ON"
                DBDT.RowFilter = "[RECEIVED ON]  = '" & txtDraftVal.Text & "' "
            Case "DOC ID"
                DBDT.RowFilter = "[SYSTEM ID] = '" & txtDraftVal.Text & "' "
            Case "CREATED BY"
                DBDT.RowFilter = "[CREATED BY] = '" & txtDraftVal.Text & "' "
            Case "CURRENT STATUS"
                DBDT.RowFilter = "STATUS = '" & txtDraftVal.Text & "' "
            Case Else
                'Dim str As String = "[" & ddlPendinggrdHdr.SelectedItem.Text.ToString() & "]='" & txtPendinggrdval.Text & "'"
                DBDT.RowFilter = "[" & ddlDraftHdr.SelectedItem.Text.ToString() & "]='" & txtDraftVal.Text & "'"
        End Select
        Dim datafields As DataTable = DBDT.Table.DefaultView.ToTable()
        gvDraft.DataSource = datafields
        gvDraft.DataBind()
    End Sub


    'Code to Print

    'Public Sub PrintDoc(ByVal SENDER As Object, ByVal E As EventArgs)
    '    Dim pid As Integer = Convert.ToString(Request.QueryString("TID").ToString())
    '    Dim imgBut As ImageButton = TryCast(SENDER, ImageButton)
    '    Dim TemplateID As String = imgBut.AlternateText.ToString()

    '    If TemplateID = "DEFAULT" Then
    '        PrintDefault(pid)
    '        'Print Normal Form
    '    Else
    '        Print(TemplateID, pid)
    '    End If
    'End Sub

    Public Sub PrintDefault(ByVal DocID As Integer)
        ClientScript.RegisterStartupScript([GetType](), "print", "window.print();", True)

    End Sub

    ''Public Sub Print(ByVal TemplateID As Integer, ByVal DocID As Integer)
    ''    Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
    ''    Dim con As New SqlConnection(conStr)

    ''    Dim da As New SqlDataAdapter("select * from MMM_Print_Template where tid=" & TemplateID, con)
    ''    Dim ds As New DataSet
    ''    da.Fill(ds, "data")

    ''    If ds.Tables("data").Rows.Count <> 1 Then
    ''        da.Dispose()
    ''        con.Dispose()
    ''        Exit Sub
    ''    End If

    ''    Dim body As String = ds.Tables("data").Rows(0).Item("body").ToString()
    ''    Dim MainQry As String = ds.Tables("data").Rows(0).Item("qry").ToString()
    ''    Dim childQry As String = ds.Tables("data").Rows(0).Item("SQL_CHILDITEM").ToString()
    ''    Dim DocType As String = ds.Tables("data").Rows(0).Item("Documenttype").ToString()
    ''    Dim OtherQry As String = ds.Tables("data").Rows(0).Item("sql_mov_dtl").ToString()

    ''    da.SelectCommand.CommandText = MainQry.Replace("@tid", DocID)
    ''    da.Fill(ds, "main")

    ''    For j As Integer = 0 To ds.Tables("main").Columns.Count - 1
    ''        body = body.Replace("[" & ds.Tables("main").Columns(j).ColumnName & "]", ds.Tables("main").Rows(0).Item(j).ToString())
    ''    Next

    ''    If OtherQry <> "" Then
    ''        da.SelectCommand.CommandText = OtherQry.Replace("@tid", DocID)
    ''        da.Fill(ds, "other")
    ''        For k As Integer = 0 To ds.Tables("other").Columns.Count - 1
    ''            body = body.Replace("[" & ds.Tables("other").Columns(k).ColumnName & "]", ds.Tables("other").Rows(0).Item(k).ToString())
    ''        Next
    ''    End If

    ''    da.SelectCommand.CommandText = childQry.Replace("@tid", DocID)
    ''    da.Fill(ds, "child")

    ''    ds.Dispose()
    ''    con.Dispose()

    ''    Dim strChildItem As String = "<div><table width=""100%"" border=""0.5""  >"
    ''    Dim prevVal As String = ""
    ''    For i As Integer = 0 To ds.Tables("child").Rows.Count - 1
    ''        If prevVal = ds.Tables("child").Rows(i).Item(0).ToString() Then
    ''            prevVal = ds.Tables("child").Rows(i).Item(0).ToString()
    ''            ds.Tables("child").Rows(i).Item(0) = ""
    ''        Else
    ''            prevVal = ds.Tables("child").Rows(i).Item(0).ToString()
    ''        End If
    ''    Next

    ''    For i As Integer = 0 To ds.Tables("child").Rows.Count
    ''        strChildItem &= "<tr>"
    ''        For j As Integer = 0 To ds.Tables("child").Columns.Count - 1
    ''            strChildItem &= "<td>"
    ''            If i = 0 Then
    ''                strChildItem &= ds.Tables("child").Columns(j).ColumnName
    ''            Else
    ''                strChildItem &= ds.Tables("child").Rows(i - 1).Item(j).ToString()
    ''            End If
    ''            strChildItem &= "</td>"
    ''        Next
    ''        strChildItem &= "</tr>"
    ''    Next
    ''    strChildItem &= "</table></div>"

    ''    body = body.Replace("[child item]", strChildItem)
    ''    Dim pnl As New Panel
    ''    pnl.Controls.Add(New LiteralControl(body))

    ''    Dim attachment As String = "attachment; filename=" & DocType & "_" & DocID & ".pdf"
    ''    System.Web.HttpContext.Current.Response.ClearContent()
    ''    HttpContext.Current.Response.AddHeader("content-disposition", attachment)
    ''    HttpContext.Current.Response.ContentType = "application/pdf"

    ''    pnl.Font.Size = 7
    ''    Dim stw As StringWriter = New StringWriter()
    ''    Dim htextw As HtmlTextWriter = New HtmlTextWriter(stw)
    ''    pnl.RenderControl(htextw)
    ''    Dim document As iTextSharp.text.Document = New iTextSharp.text.Document()
    ''    ' Dim font As New iTextSharp.text.Font(iTextSharp.text.FontFactory.GetFont("", 8, iTextSharp.text.Font.NORMAL, New iTextSharp.text.BaseColor(80, 80, 80)))
    ''    PdfWriter.GetInstance(document, HttpContext.Current.Response.OutputStream)
    ''    document.Open()
    ''    'document.Add(iTextSharp.text.FontFactory.GetFont("Verdana", 8, iTextSharp.text.Font.NORMAL, New iTextSharp.text.BaseColor(80, 80, 80)))
    ''    ' document.Add(New Phrase("Report Name: ", FontFactory.GetFont("Verdana", 8, iTextSharp.text.Font.BOLD, New iTextSharp.text.BaseColor(21, 66, 157))))
    ''    Dim Str As StringReader = New StringReader(stw.ToString())
    ''    Dim htmlworker As iTextSharp.text.html.simpleparser.HTMLWorker = New iTextSharp.text.html.simpleparser.HTMLWorker(document)
    ''    htmlworker.Parse(Str)
    ''    document.Close()

    ''    HttpContext.Current.Response.Write(document)
    ''    HttpContext.Current.Response.End()
    ''End Sub
#Region "DashBoard/Widgrts"
    <WebMethod()> _
    Public Shared Function GetWidgets() As String
        Dim ret As String = ""
        Dim EID As Integer, DBName As String, Roles As String
        Dim obj As New Widget()
        Try
            EID = Convert.ToInt32(HttpContext.Current.Session("EID"))
            DBName = "MainHome"
            Roles = Convert.ToString(HttpContext.Current.Session("UserRole"))
            Dim arr = Roles.Split(",")
            ret = obj.GetWidgets(EID, DBName, Roles)
        Catch ex As Exception
        End Try
        Return ret
    End Function
    <WebMethod()> _
    Public Shared Function GetUsefullLink() As UseFullLink
        Dim ret As New UseFullLink()
        Dim EID As Integer
        Dim obj As New Widget()
        Try
            EID = Convert.ToInt32(HttpContext.Current.Session("EID"))
            ret = obj.GetUsefullLink(EID)
        Catch ex As Exception
        End Try
        Return ret
    End Function

    <WebMethod()> _
    Public Shared Function GetCustomWidget(tid As Integer) As kGrid
        Dim ret As New kGrid()
        Try
            Dim obj As New Widget()
            Dim UID = Convert.ToInt32(HttpContext.Current.Session("UID"))
            Dim Roles = Convert.ToString(HttpContext.Current.Session("Roles"))
            ret = obj.GetCustomWidget(tid, UID, Roles)
        Catch ex As Exception
        End Try
        Return ret
    End Function

    <WebMethod()> _
    Public Shared Function SholinkURL() As String
        Dim ret = ""
        Dim ob As New MainUtility()
        Dim timestamp As Integer = ob.DateTimeToEpoch(DateTime.UtcNow)
        Dim name As String = HttpContext.Current.Session("USERNAME")
        Dim email As String = HttpContext.Current.Session("EMAIL")
        Dim ext_id As String = ""
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim od As New SqlDataAdapter("Select * from MMM_SSO_ZENDESK WHERE EID=" & HttpContext.Current.Session("EID").ToString(), con)
        Try
            Dim dt As New DataTable
            od.Fill(dt)
            If dt.Rows.Count = 1 Then
                Dim org As String = dt.Rows(0).Item("orgName").ToString()
                Dim token As String = dt.Rows(0).Item("zKey").ToString()
                Dim link As String = dt.Rows(0).Item("zendesklink").ToString()
                Dim hash As String = ob.Md5(name & email & ext_id & org & token & timestamp)
                Dim retUrl As String = link & "name=" & name & "&email=" & email & "&external_id=" & ext_id & "&organization=" & org & "&timestamp=" & timestamp & "&hash=" & hash
                ret = retUrl
            End If
        Catch ex As Exception
        Finally
            od.Dispose()
            con.Close()
            con.Dispose()
        End Try
        Return ret
    End Function
    <WebMethod()> _
    Public Shared Function GetPiChartWidget(tid As Integer) As pichartCol
        Dim ret As New pichartCol()
        Try
            Dim obj As New Widget()
            Dim UID = Convert.ToInt32(HttpContext.Current.Session("UID"))
            Dim Roles = Convert.ToString(HttpContext.Current.Session("UserRole"))
            'ret = obj.GetPiChartWidget(tid, UID, Roles)
        Catch ex As Exception
        End Try
        Return ret
    End Function

#End Region

    Public Sub ExecuteControllevelRule(ctrlID As String, pnlP As Panel, pnlC As Panel, screenname As String, dtparent As DataTable, dtChild As DataTable, Optional ErrorLbl As Label = Nothing, Optional IsChildForm As Boolean = False)
        Try
            'Code For Rule Engine Execution
            'Ajeet'
            Dim ObjRet As New RuleResponse()
            Dim objD As New DynamicForm()
            Dim ActionPanel As Panel = pnlP
            Dim lstData As New List(Of UserData)
            Dim lstDataC As List(Of UserData) = Nothing
            'in case of child item create collection of both panel
            If IsChildForm Then
                ActionPanel = pnlC
                lstDataC = New List(Of UserData)
                lstDataC = objD.CreateCollection(pnlP, dtChild)
            End If
            'Initialising rule Object
            Dim dt As DataTable = dtparent
            lstData = objD.CreateCollection(pnlP, dt)
            Dim ctrlvalue As String = ""
            For Each obj In lstData
                If obj.FieldID = ctrlID Then
                    ctrlvalue = obj.Values
                End If
            Next
            Dim ObjRule As New RuleEngin(Session("EID"), screenname, "CREATED", "CONTROL", ctrlID.ToString, ctrlvalue.ToString())
            Dim dsrule As DataSet = ObjRule.GetRules()
            Dim dtrule As New DataTable
            dtrule = dsrule.Tables(1)
            For Each dr As DataRow In dsrule.Tables(0).Rows
                'Getting 
                ObjRet = ObjRule.ExecuteRule(lstData, lstDataC, IsChildForm, "", dr, dtrule)
                If Not String.IsNullOrEmpty(ObjRet.TargetField.Trim) And ObjRet.HasRule = True Then
                    'code for displaying rule message
                    'changes by himank on 11th september 2015
                    Dim lbmsg = TryCast(Session("lbl"), Label)
                    If Not lbmsg Is Nothing Then
                        If Not ObjRet.Success Then
                            'lbmsg.Text = ObjRet.ErrorMessage
                            'lbmsg.ForeColor = System.Drawing.Color.Red
                            'lbmsg.Visible = True
                        Else
                            lbmsg.Text = ""
                        End If

                    End If
                    'ErrorLbl.Text = ObjRet.Message
                    'ErrorLbl.Text = ObjRet.ErrorMessage
                    'Spliting it with "," because there can be more than one target controls
                    Dim arr = ObjRet.TargetField.Split(",")
                    Dim trgCtrl = Nothing
                    Dim lbl As New Label()
                    'Dim ActionType As String = ObjRet.ActionType.Trim.ToUpper
                    For i As Integer = 0 To arr.Length - 1
                        trgCtrl = Nothing
                        If (arr(i).Trim <> "") Then
                            Dim ctrl = ActionPanel.FindControl("fld" & arr(i))
                            If Not ctrl Is Nothing Then
                                lbl = ActionPanel.FindControl("lbl" & arr(i))
                                If ctrl.GetType() Is GetType(System.Web.UI.WebControls.DropDownList) Then
                                    trgCtrl = CType(ctrl, DropDownList)
                                Else
                                    trgCtrl = CType(ctrl, TextBox)
                                End If
                                'changes on controls according to configured rule
                                'changes by Himank on 11th september
                                If ObjRet.Success Then
                                    Select Case ObjRet.SuccActionType.ToUpper
                                        Case "CHANGE DROPDOWN COLOR"
                                            Dim df As New DynamicForm()
                                            Dim style As String = df.ExecuteScalervalue("mmm_mst_fields", "isnull(style,'')", " fieldid=" & arr(i) & " and eid=" & Session("EID") & "")
                                            If style <> "" Then
                                                Dim txt As String() = style.Split(",")
                                                trgCtrl.CssClass = ""
                                                lbl.CssClass = ""
                                            Else
                                                trgCtrl.CssClass = "txtBox"
                                            End If

                                        Case "DISABLE"
                                            trgCtrl.Enabled = False
                                        Case "INVISIBLE"
                                            trgCtrl.CssClass = "invisible"
                                            lbl.CssClass = "invisible"
                                        Case "ENABLE"
                                            trgCtrl.Enabled = True
                                        Case "VISIBLE"
                                            Dim df As New DynamicForm()
                                            Dim style As String = df.ExecuteScalervalue("mmm_mst_fields", "isnull(style,'')", " fieldid=" & arr(i) & " and eid=" & Session("EID") & "")
                                            If style <> "" Then
                                                Dim txt As String() = style.Split(",")
                                                trgCtrl.CssClass = ""
                                                lbl.CssClass = ""
                                            Else
                                                trgCtrl.CssClass = "txtBox"
                                            End If
                                        Case "NO ACTION"

                                    End Select
                                Else
                                    Select Case ObjRet.FailActionType.ToUpper
                                        Case "CHANGE DROPDOWN COLOR"
                                            trgCtrl.CssClass = "Heiglight"
                                            lbl.CssClass = "Heiglight"
                                        Case "DISABLE"
                                            trgCtrl.Enabled = False
                                        Case "ENABLE"
                                            trgCtrl.Enabled = False
                                        Case "INVISIBLE"
                                            trgCtrl.CssClass = "invisible"
                                            lbl.CssClass = "invisible"
                                        Case "VISIBLE"
                                            Dim df As New DynamicForm()
                                            Dim style As String = df.ExecuteScalervalue("mmm_mst_fields", "isnull(style,'')", " fieldid=" & arr(i) & " and eid=" & Session("EID") & "")
                                            If style <> "" Then
                                                Dim txt As String() = style.Split(",")
                                                trgCtrl.CssClass = ""
                                                lbl.CssClass = ""
                                            Else
                                                trgCtrl.CssClass = "txtBox"
                                            End If
                                    End Select
                                End If
                            End If
                        End If
                    Next
                    'Exit Sub
                End If
            Next
        Catch ex As Exception
            Throw New Exception("Exception in control level Rule Ececution")
        End Try
    End Sub
    Public Sub AddHandlerOnControl(dt As DataTable, pnl As Panel, Optional errlbl As Label = Nothing)
        Try
            Session("lbl") = errlbl
            Dim RuleRow() As DataRow = dt.Select("HasRule='1'")
            If RuleRow.Length > 0 Then
                For r As Integer = 0 To RuleRow.Length - 1
                    Dim FieldType = RuleRow(r).Item("FieldType").ToString()
                    Select Case FieldType.ToUpper
                        Case "TEXT BOX"
                            Dim TextBox As TextBox = TryCast(pnlPerReject.FindControl("fld" & RuleRow(r).Item("FieldID").ToString()), TextBox)
                            If Not TextBox Is Nothing Then
                                Dim id As String = Right(TextBox.ID, TextBox.ID.Length - 3)
                                TextBox.AutoPostBack = True
                                AddHandler TextBox.TextChanged, AddressOf TextBoxRule_TextChanged
                            End If
                        Case "CHECK BOX"
                            Dim CheckBox As CheckBox = TryCast(pnl.FindControl("fld" & RuleRow(r).Item("FieldID").ToString()), CheckBox)
                            Dim id As String = Right(CheckBox.ID, CheckBox.ID.Length - 3)
                            CheckBox.AutoPostBack = True
                            AddHandler CheckBox.CheckedChanged, AddressOf TextBoxRule_TextChanged
                    End Select
                Next
            End If
        Catch ex As Exception

        End Try
    End Sub
    Protected Sub TextBoxRule_TextChanged(sender As Object, e As EventArgs)
        Try
            Dim txt As Control = TryCast(sender, Control)
            Dim id As String = Right(txt.ID, txt.ID.Length - 3)
            Dim id1 As Integer = CInt(id)
            Dim screenname = Convert.ToString(Session("actionform"))
            Dim dt As DataTable = Session("FIELDS")
            Dim pnlFields As Panel = TryCast(Session("Panel"), Panel)
            ExecuteControllevelRule(id1, pnlFields, Nothing, screenname, dt, Nothing, Nothing, False)
        Catch ex As Exception
            Throw
        End Try
    End Sub
    Public Sub ShowApprove(DocID As String)
        If IsNothing(Session("DocID")) Then
            Exit Sub
        End If
        lblTabApprove.Text = ""
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("uspSelectEventScreen", con)
        oda.SelectCommand.CommandType = CommandType.StoredProcedure
        oda.SelectCommand.Parameters.AddWithValue("docid", DocID)
        oda.SelectCommand.Parameters.AddWithValue("subevent", "APPROVAL")
        Dim ds As New DataSet
        oda.Fill(ds, "fields")
        If ds.Tables("fields").Rows.Count > 0 Then
            Session("ActionForm") = ds.Tables("fields").Rows(0).Item("documenttype").ToString()
        Else
            Dim oda1 As SqlDataAdapter = New SqlDataAdapter("uspSelectEventScreen1", con)
            oda1.SelectCommand.CommandType = CommandType.StoredProcedure
            oda1.SelectCommand.Parameters.Clear()
            oda1.SelectCommand.Parameters.AddWithValue("@docid", DocID)
            oda1.SelectCommand.Parameters.AddWithValue("@subevent", "APPROVAL")
            Dim ds1 As New DataSet
            oda1.Fill(ds1, "Action")
            If ds1.Tables("Action").Rows.Count > 0 Then
                Session("ActionForm") = Convert.ToString(ds1.Tables("Action").Rows(0).Item("documenttype"))
            End If
        End If
        Session("FIELDS") = ds.Tables("fields")
        Session("ACTION") = "APPROVAL"
        pnlApprove.Width = 500
        pnlApprove.Height = 300
        pnlApprove.Controls.Clear()
        updatePanelApprove.Update()
        'UpdatePnl4.Update()
        If ds.Tables("fields").Rows.Count > 0 Then
            '           lblHeaderPopUp.Text = ds.Tables("fields").Rows(0).Item("Formcaption").ToString()
            Dim ob As New DynamicForm()
            ob.CreateControlsOnPanel(ds.Tables("fields"), pnlApprove, updatePanelApprove, btnApprove1, 1, Nothing, Nothing, True, True)
            Dim ROW1() As DataRow = ds.Tables("fields").Select("fieldtype='DROP DOWN' and dropdowntype='MASTER VALUED' and lookupvalue is not null")
            If ROW1.Length > 0 Then
                For i As Integer = 0 To ROW1.Length - 1
                    Dim DDL As DropDownList = TryCast(pnlApprove.FindControl("fld" & ROW1(i).Item("FieldID").ToString()), DropDownList)
                    DDL.AutoPostBack = True
                    AddHandler DDL.TextChanged, AddressOf bindvalue2
                Next
            End If

            ob.FillControlsOnPanel(ds.Tables("fields"), pnlApprove, "DOCUMENT", DocID)
            Dim RuleRow() As DataRow = ds.Tables("fields").Select("HasRule='1'")
            If RuleRow.Length > 0 Then
                For r As Integer = 0 To RuleRow.Length - 1
                    Dim ControlID As Integer = CInt(RuleRow(r).Item("FieldID"))
                    Dim screenname = ds.Tables("fields").Rows(0).Item("documenttype").ToString()
                    ExecuteControllevelRule(ControlID, pnlApprove, Nothing, screenname, ds.Tables("fields"), Nothing, Nothing, False)
                Next
            End If
            'Dim conF As New SqlConnection(ConfigurationManager.ConnectionStrings("conStr").ConnectionString)
            'Dim odaField As SqlDataAdapter = New SqlDataAdapter("SELECT FF.*,F.*  FROM MMM_MST_FIELDS FF left outer JOIN MMM_MST_FORMS F on F.FormName = FF.DocumentType and F.EID = FF.EID   where FF.isactive=1 and F.EID=" & Session("EID").ToString() & " and FormName ='" & Convert.ToString(Session("ActionForm")) & "' and eventname in(select documenttype from MMM_MST_DOC where eid=" & Session("EID") & " and tid=" & DocID & ")   order by displayOrder", conF)
            'Dim dtFileds As New DataTable()
            'odaField.Fill(dtFileds)
            Session("Panel") = pnlApprove
            AddHandlerOnControl(ds.Tables("fields"), pnlApprove, lblMsgRule2)
        End If
        con.Close()
        oda.Dispose()
        con.Dispose()
        If (Not IsNothing(Session("ActionForm"))) Then
            Dim conStrs As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim cons As SqlConnection = New SqlConnection(conStrs)
            Dim actionform As String = Session("ActionForm")
            Dim childActionvalueRights As String = String.Empty
            Dim oda12 As SqlDataAdapter = New SqlDataAdapter("", cons)
            oda12.SelectCommand.CommandType = CommandType.Text
            'With(nolock) added by Himank on 29th sep 2015
            oda12.SelectCommand.CommandText = "select ChidFormforActionScreen from MMM_MST_FORMS  WITH(NOLOCK)   WHERE FORMNAME='" & actionform & "' and eid=" & Session("EID").ToString()
            If (cons.State = ConnectionState.Closed) Then
                cons.Open()
            End If
            Dim childActionvalue = oda12.SelectCommand.ExecuteScalar()
            oda12.Dispose()
            Dim oda123 As SqlDataAdapter = New SqlDataAdapter("", cons)
            oda123.SelectCommand.CommandType = CommandType.Text
            'With(nolock) added by Himank on 29th sep 2015
            oda123.SelectCommand.CommandText = "select isnull(ChidFormforActionScreen_Rights,'') from MMM_MST_FORMS  WITH(NOLOCK)   WHERE FORMNAME='" & actionform & "' and eid=" & Session("EID").ToString()
            If (cons.State = ConnectionState.Closed) Then
                cons.Open()
            End If
            childActionvalueRights = oda123.SelectCommand.ExecuteScalar()
            Session("CHILDACTIONSCREEN") = Nothing
            Session("CHILDACTIONSCREEN") = childActionvalue
            Session("CHILDACTIONSCREENRIGHTS") = Nothing
            Session("CHILDACTIONSCREENRIGHTS") = childActionvalueRights
            If Convert.ToString(childActionvalue).Length > 0 Then
                If (Not IsNothing(Session("ActionForm"))) Then
                    pnlApprove.Controls.Add(New LiteralControl("<tr>"))
                    pnlApprove.Controls.Add(New LiteralControl("<td colspan='4'>"))
                    pnlApprove.Controls.Add(New LiteralControl("&nbsp;</td>"))
                    pnlApprove.Controls.Add(New LiteralControl("</tr>"))
                    pnlApprove.Controls.Add(New LiteralControl("<tr>"))
                    pnlApprove.Controls.Add(New LiteralControl("<td colspan='4'>"))
                    pnlApprove.Controls.Add(New LiteralControl("<div id=""tabs"" ><ul>"))
                    ' pnlApprove.Controls.Add(New LiteralControl("<li><a href=""#tabPending0"">" & Session("ActionForm").ToString() & "<asp:Label ID=""lblpending0"" runat=""server""></asp:Label></a></li>"))

                    Dim strs As String() = Session("CHILDACTIONSCREEN").ToString().Split(",")

                    If (Not IsNothing(strs)) Then
                        For j As Integer = 1 To strs.Length
                            If Convert.ToString(strs(j - 1)) <> "" Then
                                Dim childvalue As String() = strs(j - 1).ToString().Split(".")
                                pnlApprove.Controls.Add(New LiteralControl("<li><a href=""#tabPending" & j.ToString() & """>" & childvalue(1).ToString() & "<asp:Label ID=""lblpending" & j & " "" runat=""server""></asp:Label></a></li>"))
                            End If
                        Next
                    End If

                    pnlApprove.Controls.Add(New LiteralControl("</ul>"))

                    Dim conStr1 As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
                    Dim con1 As New SqlConnection(conStr1)
                    Dim oda1 As New SqlDataAdapter("", con1)
                    Dim ds1 As New DataSet
                    If (Not IsNothing(strs)) Then
                        For i As Integer = 1 To strs.Length
                            Dim childvalue As String() = strs(i - 1).ToString().Split(".")
                            Dim conStrsS As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
                            Dim consS As New SqlConnection(conStrsS)
                            Dim dasS As SqlDataAdapter = New SqlDataAdapter("", consS)
                            Dim odaN As SqlDataAdapter = New SqlDataAdapter("", consS)
                            Dim dtChildS As New DataTable()
                            Dim create As Boolean = False, delete As Boolean = False, Edit As Boolean = False
                            'dasS.SelectCommand.CommandText = "SELECT F.FieldID,F.displayName,F.FieldType,F.displayOrder,F.dropdown,case F.isRequired when 1 then 'Mandatory' else 'Nullable' END [isrequired],case F.isActive when 0 then 'InActive' else 'Active' end [isActive],isnull((select isEditable from MMM_MST_FIELDS WHERE DOCUMENTTYPE='" & childvalue(1).ToString() & "' and EID=" & Session("EID").ToString() & " and displayName =f.displayname),F.ISEDITABLE) [isEditable],isnull((select count(displayname) from MMM_MST_FIELDS where DocumentType ='CAS." & childvalue(1).ToString() & "' and EID=" & Session("EID").ToString() & " and displayName =f.displayname),0) [AF] FROM MMM_MST_FIELDS F   WHERE F.documenttype='" & childvalue(1).ToString() & "' and  F.EID = " & Session("EID").ToString() & "   Order By Displayorder"
                            'With(nolock) added by Himank on 29th sep 2015
                            dasS.SelectCommand.CommandText = "select fieldid,displayname,fieldtype from mmm_mst_fields  WITH(NOLOCK)   where documenttype='" & strs(i - 1).ToString() & "' and eid=" & Session("EID")
                            dasS.Fill(dtChildS)
                            If (dtChildS.Rows.Count > 0) Then
                                pnlApprove.Controls.Add(New LiteralControl("<div id=""tabPending" & i.ToString() & """style=""max-height:200px;overflow-x:scroll;overflow-y:scroll;max-width:720px;"">"))

                                'Dim conStrss As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
                                'Dim conss As SqlConnection = New SqlConnection(conStrss)
                                'Dim actionform As String = Session("ActionForm")

                                odaN.SelectCommand.CommandType = CommandType.StoredProcedure
                                odaN.SelectCommand.Parameters.Clear()
                                odaN.SelectCommand.CommandText = "uspGetDetailITEMByDocidonAction"
                                odaN.SelectCommand.Parameters.AddWithValue("docid", DocID)
                                odaN.SelectCommand.Parameters.AddWithValue("FN", strs(i - 1).ToString())
                                odaN.SelectCommand.Parameters.AddWithValue("NFN", childvalue(1))
                                odaN.SelectCommand.Parameters.AddWithValue("EID", Session("EID").ToString())
                                Dim dtItem As New DataTable()
                                odaN.Fill(dtItem)
                                odaN.SelectCommand.CommandType = CommandType.StoredProcedure
                                odaN.SelectCommand.Parameters.Clear()
                                odaN.SelectCommand.CommandText = "uspGetValuesFromChildonEditonAction"
                                odaN.SelectCommand.Parameters.AddWithValue("DOcid", DocID)
                                odaN.SelectCommand.Parameters.AddWithValue("FN", strs(i - 1).ToString())
                                odaN.SelectCommand.Parameters.AddWithValue("NFN", childvalue(1))
                                odaN.SelectCommand.Parameters.AddWithValue("EID", Session("EID").ToString())
                                Dim dtITemVal As New DataTable()
                                odaN.Fill(dtITemVal)
                                If (dtITemVal.Rows.Count = 0) Then
                                    Dim drs As DataRow = dtITemVal.NewRow()
                                    dtITemVal.Rows.Add(drs)
                                End If
                                If (dtItem.Rows.Count = 0) Then
                                    Dim drs1 As DataRow = dtItem.NewRow()
                                    If (drs1.Table.Columns.Contains("Ctid")) Then
                                        drs1("ctid") = 0
                                    ElseIf (drs1.Table.Columns.Contains("tid")) Then
                                        drs1("tid") = 0
                                    End If
                                    dtItem.Rows.Add(drs1)
                                End If
                                Dim GRDCHLDVIEW As New GridView
                                GRDCHLDVIEW.AutoGenerateColumns = "TRUE"
                                GRDCHLDVIEW.ID = "GRD_" & strs(i - 1).ToString().Replace(" ", "__").Replace(".", "_")
                                GRDCHLDVIEW.Width = "500"
                                GRDCHLDVIEW.CssClass = "mGrid"
                                GRDCHLDVIEW.AlternatingRowStyle.CssClass = "alt"
                                AddHandler GRDCHLDVIEW.RowDataBound, AddressOf GRDCHLDVIEW_InlineEdit
                                If (Not IsNothing(childActionvalueRights)) Then
                                    If (Not childActionvalueRights = String.Empty) Then
                                        Dim rights As String() = childActionvalueRights.ToString().Split(",")
                                        For a As Integer = 0 To rights.Length - 1
                                            If (rights(a).ToString().Contains(strs(i - 1).ToString() & "-")) Then
                                                Dim finalval As String() = rights(a).ToString().Split(New String() {strs(i - 1).ToString() & "-"}, StringSplitOptions.None)
                                                If (finalval.Length > 1) Then
                                                    Dim rightval = finalval(1)

                                                    If (Convert.ToInt32(rightval) = 14) Then
                                                        create = True
                                                        delete = True
                                                        Edit = True
                                                    ElseIf (Convert.ToInt32(rightval) = 12) Then
                                                        create = False
                                                        Edit = True
                                                        delete = True
                                                    ElseIf (Convert.ToInt32(rightval) = 10) Then
                                                        create = True
                                                        Edit = False
                                                        delete = True
                                                    ElseIf (Convert.ToInt32(rightval) = 8) Then
                                                        create = False
                                                        Edit = False
                                                        delete = True
                                                    ElseIf (Convert.ToInt32(rightval) = 6) Then
                                                        create = True
                                                        Edit = True
                                                        delete = False
                                                    ElseIf (Convert.ToInt32(rightval) = 4) Then
                                                        create = False
                                                        Edit = True
                                                        delete = False
                                                    ElseIf (Convert.ToInt32(rightval) = 2) Then
                                                        create = True
                                                        Edit = False
                                                        delete = False
                                                    End If

                                                End If
                                            End If
                                        Next

                                    End If
                                End If
                                If (delete = True) Then
                                    AddHandler GRDCHLDVIEW.RowCommand, AddressOf deletedrow
                                    AddHandler GRDCHLDVIEW.RowDeleting, AddressOf Deleting
                                    Session(strs(i - 1).ToString().Replace(" ", "__").Replace(".", "_") & "Delete") = "YES"
                                Else
                                    Session(strs(i - 1).ToString().Replace(" ", "__").Replace(".", "_") & "Delete") = Nothing
                                End If

                                Session(strs(i - 1).ToString() & "VAL") = dtITemVal
                                Session("ChildDoctypeAction") = strs(i - 1).ToString()
                                If (Not IsNothing(Session(strs(i - 1).ToString()))) Then
                                    GRDCHLDVIEW.DataSource = Session(strs(i - 1).ToString())
                                Else
                                    GRDCHLDVIEW.DataSource = dtItem
                                End If
                                GRDCHLDVIEW.DataBind()
                                Session(strs(i - 1)) = dtItem
                                pnlApprove.Controls.Add(GRDCHLDVIEW)
                                If (create = True) Then

                                    Session(strs(i - 1).ToString().Replace(" ", "__").Replace(".", "_") & "Create") = "YES"
                                    Dim btn As New Button
                                    btn.ID = strs(i - 1).ToString().Replace(" ", "__").Replace(".", "_") & "ADD"
                                    btn.Text = "ADD ROW"
                                    btn.CssClass = "btnDyn"
                                    btn.EnableViewState = False
                                    AddHandler btn.Click, AddressOf btnAddRow_Click
                                    pnlApprove.Controls.Add(btn)
                                Else
                                    Session(strs(i - 1).ToString().Replace(" ", "__").Replace(".", "_") & "Create") = Nothing
                                End If
                                pnlApprove.Controls.Add(New LiteralControl("</div>"))

                            End If
                        Next
                        pnlApprove.Controls.Add(New LiteralControl("</td></tr>"))
                    End If
                End If
                pnlApprove.Controls.Add(New LiteralControl("</div>"))
            End If
        End If
    End Sub
    Public Sub bindvalue2(ByVal sender As Object, ByVal e As EventArgs)
        Dim c As Control = GetPostBackControl(Me.Page)
        '...
        If c IsNot Nothing Then
        End If
        If TypeOf c Is System.Web.UI.WebControls.DropDownList Then
            Dim ddl As DropDownList = TryCast(c, DropDownList)
            Dim id As String = Right(ddl.ID, ddl.ID.Length - 3)
            Dim id1 As Integer = CInt(id)
            Dim ob As New DynamicForm()
            ob.bind(id, pnlApprove, ddl)
            ModalApprove.Show()
        End If
    End Sub
    Public Shared Function GetPostBackControl(ByVal page As Page) As Control
        Dim control As Control = Nothing

        Dim ctrlname As String = page.Request.Params.[Get]("__EVENTTARGET")
        If ctrlname IsNot Nothing AndAlso ctrlname <> String.Empty Then
            control = page.FindControl(ctrlname)
        Else
            For Each ctl As String In page.Request.Form
                Dim c As Control = page.FindControl(ctl)
                If TypeOf c Is System.Web.UI.WebControls.Button Then
                    control = c
                    Exit For
                ElseIf TypeOf c Is System.Web.UI.WebControls.ImageButton Then
                    control = c
                    Exit For
                End If
            Next
        End If
        Return control
    End Function

    Protected Sub GRDCHLDVIEW_InlineEdit(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs)
        Try
            Dim fn As String = ""
            Dim dtfld1 As New DataTable
            Dim row As GridViewRow = e.Row
            Dim GID As String = row.Parent.Parent.ID
            Dim GIDS As String() = row.Parent.Parent.ID.ToString().Split(New String() {"GRD_"}, StringSplitOptions.None)
            fn = GIDS(1).Replace("__", " ").Replace("_", ".")
            Dim grd As GridView = DirectCast(sender, GridView)
            If e.Row.RowType = DataControlRowType.Header Then
                For i As Integer = 0 To e.Row.Cells.Count - 1
                    If e.Row.Cells(i).Text.ToUpper() = "CMASTERTID" Or e.Row.Cells(i).Text.ToUpper() = "TID" Then
                        If Not IsNothing(Session(fn.ToString().Replace(" ", "__").Replace(".", "_") & "Delete")) Then
                            e.Row.Cells(i).Text = "Action"
                            e.Row.ForeColor = Drawing.Color.Black
                        Else
                            e.Row.Cells(i).Visible = False
                        End If
                    End If
                Next
                If (e.Row.Cells.Count > 2) Then
                    If e.Row.Cells(2).Text.ToUpper() = "TID" Then
                        If Not IsNothing(Session(fn.ToString().Replace(" ", "__").Replace(".", "_") & "Delete")) Then
                            e.Row.Cells(2).Text = "Action"
                            e.Row.ForeColor = Drawing.Color.Black
                        Else
                            e.Row.Cells(2).Visible = False
                        End If
                    End If
                End If
            End If
            If row.RowType = DataControlRowType.DataRow Then
                If row.Cells(0).Text.ToUpper <> "TOTAL" Then
                    Dim conStr1 As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
                    Dim con1 As New SqlConnection(conStr1)
                    'With(nolock) added by Himank on 29th sep 2015
                    Dim oda1 As New SqlDataAdapter("select * from mmm_mst_fields  WITH(NOLOCK)   where documenttype='" & fn & "' and eid=" & Session("EID"), con1)
                    oda1.Fill(dtfld1)
                    If Not IsNothing(Session(fn.ToString().Replace(" ", "__").Replace(".", "_") & "Delete")) Then
                        Dim cnt As Integer = e.Row.Cells.Count - 1
                        Dim img As ImageButton = New ImageButton()
                        img.ID = e.Row.Cells(cnt).Text
                        img.ImageUrl = "~/images/Cancel.gif"
                        img.CommandName = "Remove"
                        img.CommandArgument = e.Row.Cells(cnt).Text
                        img.Height = Unit.Parse("16")
                        img.Width = Unit.Parse("16")
                        img.OnClientClick = "return DeleteConfirm();"
                        e.Row.Cells(cnt).Controls.Add(img)
                        e.Row.Cells(cnt).Controls.Add(New LiteralControl("&nbsp;"))
                    Else
                        If grd.HeaderRow.Cells(1).Text.ToUpper() = "CMASTERTID" Then
                            row.Cells(1).Visible = False
                        ElseIf grd.HeaderRow.Cells(1).Text.ToUpper() = "TID" Then
                            row.Cells(1).Visible = False
                        End If
                        If (grd.HeaderRow.Cells.Count > 2) Then
                            If grd.HeaderRow.Cells(2).Text.ToUpper() = "TID" Then
                                row.Cells(2).Visible = False
                            End If
                        End If
                        For i As Integer = 0 To grd.HeaderRow.Cells.Count - 1
                            If grd.HeaderRow.Cells(i).Text.ToUpper() = "CMASTERTID" Then
                                row.Cells(i).Visible = False
                            ElseIf grd.HeaderRow.Cells(i).Text.ToUpper() = "TID" Then
                                row.Cells(i).Visible = False
                            End If
                            If (grd.HeaderRow.Cells.Count > 2) Then
                                If grd.HeaderRow.Cells(i).Text.ToUpper() = "TID" Then
                                    row.Cells(i).Visible = False
                                End If
                            End If
                        Next
                    End If
                    If Not IsNothing(Session(fn.ToString().Replace(" ", "__").Replace(".", "_") & "Create")) Then
                        Dim dr As DataRow = dtfld1.NewRow()
                        dtfld1.Rows.Add(dr)
                    End If
                    For j As Integer = 0 To dtfld1.Rows.Count - 1
                        Dim ftype As String = dtfld1.Rows(j).Item("fieldtype").ToString()
                        Dim FldID As String = dtfld1.Rows(j).Item("fieldid").ToString()
                        Dim Formula = Convert.ToString(dtfld1.Rows(j).Item("cal_text"))
                        Dim DisplayName = Convert.ToString(dtfld1.Rows(j).Item("DisplayName"))
                        Dim datatype As String = dtfld1.Rows(j).Item("datatype").ToString() 'Prashant_10_7
                        Dim controlWdth As Integer = 100
                        If ftype.ToUpper() = "TEXT BOX" Or ftype.ToUpper() = "CALCULATIVE FIELD" Or ftype.ToUpper() = "LOOKUP" Then
                            Dim cb As New TextBox
                            'cb.ID = "fld" & j.ToString() & "_" & row.RowIndex
                            'cb.Width = 55
                            cb.ID = "fld" & FldID & row.RowIndex
                            'Prashant_10_7
                            If dtfld1.Rows(j).Item("defaultfieldval").ToString().Length > 0 Then
                                cb.Text = dtfld1.Rows(j).Item("defaultfieldval").ToString()
                            Else
                                If datatype.ToUpper() = "NUMERIC" Then
                                    cb.Text = "0"
                                End If
                            End If
                            If Val(dtfld1.Rows(j).Item("isDescription").ToString()) = 1 Then
                                cb.ToolTip = dtfld1.Rows(j).Item("Description").ToString()
                            End If
                            If dtfld1.Rows(j).Item("iseditable") = 1 Then
                                cb.Enabled = True
                            Else
                                cb.Enabled = False
                            End If
                            If dtfld1.Rows(j).Item("alloweditonedit") = 1 Then
                                cb.Enabled = True
                            Else
                                cb.Enabled = False
                            End If
                            cb.Width = controlWdth

                            'Code For calculative field
                            'Code End Here !!!!!!
                            If ftype.ToUpper() = "CALCULATIVE FIELD" Then
                                cb.Attributes.Add("READONLY", "READONLY")
                                cb.Attributes.Add("COLOR", "GRAY")
                                cb.Text = "0"
                                If dtfld1.Rows(j).Item("alloweditonedit") = 1 Then
                                    cb.Enabled = True
                                Else
                                    cb.Enabled = False
                                End If
                                ' cb.ReadOnly = True

                            End If

                            Dim Formula1 = Convert.ToString(dtfld1.Rows(j).Item("cal_text"))

                            If Not Formula1 Is Nothing And Formula1 <> "" Then
                                'Dim arrFor As String() = Formula.Split(",")
                                Dim jScript = GenerateJQueryScript(dtfld1, GID, row.RowIndex, Formula1, FldID)
                                If jScript <> "" Then
                                    cb.Attributes.Add("onblur", jScript)
                                End If
                            End If


                            If ViewState("chkadd") = 1 Then
                                j = j + 1
                            End If
                            Dim colValue As String = row.DataItem(j).ToString()
                            If Not colValue = "&nbsp;" And Not colValue = "" Then
                                cb.Text = colValue
                            End If

                            row.Cells(j).Controls.Add(cb)

                            If datatype.ToUpper.Trim = "DATETIME" Then
                                Dim CLNDR As New AjaxControlToolkit.CalendarExtender
                                CLNDR.Controls.Clear()
                                CLNDR.ID = "CLNDR" & dtfld1.Rows(j).Item("FieldID").ToString()
                                CLNDR.Format = "dd/MM/yy"
                                CLNDR.TargetControlID = cb.ID
                                cb.Enabled = True
                                If row.Cells(j).Text <> "" And row.Cells(j).Text <> "&nbsp;" Then
                                    cb.Text = String.Format("{0:dd/MM/yy}", row.Cells(j).Text)
                                Else
                                    cb.Text = String.Format("{0:dd/MM/yy}", Date.Now())
                                End If
                                row.Cells(j).Controls.Add(CLNDR) '
                                If dtfld1.Rows(j).Item("alloweditonedit") = 1 Then
                                    cb.Enabled = True
                                Else
                                    cb.Enabled = False
                                End If
                            End If

                            If Val(colValue) = 0 Then
                                If ViewState("chkadd") = 1 Then  'added by balli in case of checkboxes
                                    If ftype.ToUpper() = "TEXT BOX" And dtfld1.Rows(j - 1).Item("datatype").ToString().ToUpper() = "NUMERIC" Then
                                        cb.Text = "0"
                                    End If
                                Else
                                    If ftype.ToUpper() = "TEXT BOX" And dtfld1.Rows(j).Item("datatype").ToString().ToUpper() = "NUMERIC" Then
                                        cb.Text = "0"
                                    End If
                                End If
                            End If

                            If Not Formula Is Nothing And Formula <> "" Then
                                'Dim arrFor As String() = Formula.Split(",")
                                Dim jScript = GenerateJQueryScript(dtfld1, GID, row.RowIndex, Formula, FldID)
                                If jScript <> "" Then
                                    cb.Attributes.Add("onblur", jScript)
                                End If
                            End If
                        ElseIf ftype.ToUpper() = "MULTI LOOKUP" Then
                            Dim txtBox As New TextBox
                            txtBox.ID = "fld" & dtfld1.Rows(j).Item("FieldID").ToString() & row.RowIndex
                            txtBox.Width = controlWdth - 10
                            txtBox.CssClass = "txtBox"
                            If dtfld1.Rows(j).Item("isEditable").ToString() = "1" Then
                                txtBox.Enabled = True
                            Else
                                txtBox.Enabled = False
                            End If
                            row.Cells(j).Controls.Add(txtBox)
                        ElseIf ftype.ToUpper() = "DROP DOWN" Then
                            Dim ddl As New DropDownList
                            ddl.ID = "fld" & FldID & "_" & row.RowIndex
                            ddl.CssClass = "txtBox"
                            If dtfld1.Rows(j).Item("alloweditonedit") = 1 Then
                                ddl.Enabled = True
                            Else
                                ddl.Enabled = False
                            End If
                            ddl.Width = 70
                            Dim ddlText As String = dtfld1.Rows(j).Item("dropdown").ToString()
                            Dim dropdowntype As String = dtfld1.Rows(j).Item("dropdowntype").ToString()
                            Dim arr() As String
                            'Dim arrMid() As String
                            If dtfld1.Rows(j).Item("dropdowntype").ToString() = "FIX VALUED" Then
                                Dim cb As New DropDownList
                                cb.ID = "fld" & FldID & "_" & row.RowIndex
                                Dim ARR1() As String = dtfld1.Rows(j).Item("dropdown").ToString().Split(",")

                                For K As Integer = 0 To ARR1.Length - 1
                                    cb.Items.Add(ARR1(K).ToString.Trim)
                                    cb.Items(K).Value = ARR1(K).ToString.Trim
                                Next
                                cb.Items.Insert(0, "Select")
                                cb.Items(0).Value = 0
                                'Prashant_30_7
                                If dtfld1.Rows(j).Item("iseditable") = 1 Then
                                    ddl.Enabled = True
                                Else
                                    ddl.Enabled = False
                                End If
                                If ViewState("chkadd") = 1 Then
                                    j = j + 1
                                End If

                                If cb.Items.FindByText(row.Cells(j).Text.Trim) IsNot Nothing Then
                                    cb.Items.FindByText(row.Cells(j).Text.Trim).Selected = True
                                End If

                                row.Cells(j).Controls.Add(cb)


                            ElseIf dtfld1.Rows(j).Item("dropdowntype").ToString() = "MASTER VALUED" Then
                                '' code for getting master valued 
                                Dim ob As New DynamicForm
                                arr = ddlText.Split("-")
                                Dim TID As String = "TID"
                                Dim TABLENAME As String = ""
                                If UCase(arr(0).ToString()) = "MASTER" Then
                                    TABLENAME = "MMM_MST_MASTER"
                                ElseIf UCase(arr(0).ToString()) = "DOCUMENT" Then
                                    TABLENAME = "MMM_MST_DOC"
                                ElseIf UCase(arr(0).ToString()) = "CHILD" Then
                                    TABLENAME = "MMM_MST_DOC_ITEM"
                                ElseIf UCase(arr(0).ToString) = "STATIC" Then
                                    If arr(1).ToString.ToUpper = "USER" Then
                                        TABLENAME = "MMM_MST_USER"
                                        TID = "UID"
                                    ElseIf arr(1).ToString().ToUpper = "LOCATION" Then
                                        TABLENAME = "MMM_MST_LOCATION"
                                        TID = "LOCID"
                                    End If
                                End If
                                Dim lookUpqry As String = ""
                                Dim str As String = ""
                                If arr(0).ToUpper() = "CHILD" Then
                                    'With(nolock) added by Himank on 29th sep 2015
                                    str = "select " & arr(2).ToString() & "," & TID & "[tid]  from " & TABLENAME & " M  WITH(NOLOCK)   WHERE   DOCUMENTTYPE='" & arr(1).ToString() & "'"
                                ElseIf arr(0).ToUpper() <> "STATIC" Then
                                    str = "select " & arr(2).ToString() & "," & TID & "[tid]" & lookUpqry & " from " & TABLENAME & " M  WITH(NOLOCK)   WHERE EID=" & HttpContext.Current.Session("EID") & " AND  DOCUMENTTYPE='" & arr(1).ToString() & "'"
                                Else
                                    If arr(2).ToString.ToUpper = "LOCATIONSTATE" Then
                                        str = "select DISTINCT " & arr(2).ToString() & ",SID [tid]" & lookUpqry & " from " & TABLENAME & " M   WITH(NOLOCK)  "
                                    Else
                                        str = "select " & arr(2).ToString() & "," & TID & "[tid]" & lookUpqry & " from " & TABLENAME & " M   WITH(NOLOCK)  WHERE EID=" & HttpContext.Current.Session("EID") & " "
                                    End If
                                End If

                                Dim xwhr As String = ""
                                Dim tids As String = ""

                                ''FILTER THE DATA ACCORDING TO USER 
                                tids = ob.UserDataFilter(dtfld1.Rows(j).Item("documenttype").ToString(), arr(1).ToString())

                                If tids.Length >= 2 Then

                                    xwhr = " AND CONVERT(NVARCHAR(10),TID) IN (" & tids & ")"
                                ElseIf tids = "0" Then
                                    ' pnlFields.Visible = False
                                    ' btnActEdit.Visible = False
                                    ' updMain.Update()
                                    xwhr = ""
                                End If
                                str = str & "  AND M.isauth=1 " & xwhr
                                Dim AutoFilter As String = dtfld1.Rows(j).Item("AutoFilter").ToString()
                                If AutoFilter.Length > 0 Then
                                    If arr(0).ToUpper() = "CHILD" Then
                                        If AutoFilter.ToUpper = "DOCID" Then
                                            str = ob.GetQuery1(arr(1).ToString, arr(2).ToString())
                                        Else
                                            str = ob.GetQuery(arr(1).ToString, arr(2).ToString)
                                        End If
                                        'With(nolock) added by Himank on 29th sep 2015
                                    ElseIf arr(0).ToUpper() <> "STATIC" Then
                                        str = "select " & arr(2).ToString() & ",convert(nvarchar(10),tid)  [tid]" & lookUpqry & " from " & TABLENAME & " M  WITH(NOLOCK)   WHERE EID=" & Session("EID") & " AND  DOCUMENTTYPE='" & arr(1).ToString() & "'"
                                        str = str & "  AND M.isauth=1 " & xwhr
                                    Else
                                        str = "select " & arr(2).ToString() & ",convert(nvarchar(10)," & TID & ")  [tid]" & lookUpqry & " from " & TABLENAME & " M   WITH(NOLOCK)  WHERE EID=" & Session("EID") & " "
                                        str = str & "  AND M.isauth=1 " & xwhr
                                    End If
                                End If

                                Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
                                Dim con As SqlConnection = New SqlConnection(conStr)
                                Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
                                Dim dss As New DataSet

                                If str.Length > 0 Then
                                    If dtfld1.Rows(j).Item("fillwithexistingonEdit").ToString().ToUpper = "TRUE" Then
                                        'With(nolock) added by Himank on 29th sep 2015
                                        str = str & " AND TID IN (select distinct " & dtfld1.Rows(j).Item("fieldmapping").ToString() & " from mmm_mst_doc_item  WITH(NOLOCK)   where docid=" & Convert.ToInt32(Session("DocID")) & ") order by " & arr(2).ToString()
                                    Else
                                        str = str & "order by " & arr(2).ToString()
                                    End If

                                    oda.SelectCommand.CommandText = str
                                    oda.Fill(dss, "FV")
                                    Dim isAddJquery As Integer = 0
                                    ddl.Items.Add("Select")
                                    ddl.Items(0).Value = "0"
                                    For J1 As Integer = 0 To dss.Tables("FV").Rows.Count - 1
                                        ddl.Items.Add(dss.Tables("FV").Rows(J1).Item(0).ToString())
                                        Dim lookddlVal As String = dss.Tables("FV").Rows(J1).Item(1).ToString()
                                        ddl.Items(J1 + 1).Value = lookddlVal
                                    Next
                                    oda.Dispose()
                                    dss.Dispose()


                                    Dim lookupvalue As String = dtfld1.Rows(j).Item("lookupvalue").ToString()
                                    Dim multilookup As String = dtfld1.Rows(j).Item("multilookUpVal").ToString()

                                    If lookupvalue.Length > 0 Or multilookup.Length > 0 Then
                                        ddl.AutoPostBack = True
                                        AddHandler ddl.TextChanged, AddressOf ActionGridDdl_TextChanged
                                    End If

                                    If ViewState("chkadd") = 1 Then
                                        j = j + 1
                                    End If

                                    If ddl.Items.FindByText(row.Cells(j).Text) IsNot Nothing Then
                                        ddl.Items.FindByText(row.Cells(j).Text).Selected = True
                                    End If

                                    row.Cells(j).Controls.Add(ddl)
                                    If isAddJquery = 1 Then
                                        Dim JQuertStr As String = "var r1 = $('#ContentPlaceHolder1_" & ddl.ClientID & "').val(); var l = 0; var mycars = new Array(); for (var i = 0; i < r1.length; i++) { if (r1[i] == '|') { l++; mycars[l] = i; } } for (var i1 = 1; i1 < l; i1++) { var outpu = r1.substring(mycars[i1] + 1, mycars[i1 + 1]); var outpu1 = outpu.substring(0, outpu.indexOf(':')); var outpu2 = outpu.substring(outpu.indexOf(':') + 1); if (outpu2 == 'S') { var out = r1.substring(0, mycars[1]); var x = $('#ContentPlaceHolder1_tmp' + outpu1 + ' option').length; var options = ''; txt = ''; for (i = 0; i < x; i++) { var strUser = $('#ContentPlaceHolder1_tmp' + outpu1 + ' option:eq(' + i + ')').val(); var sel = strUser.substring(strUser.indexOf('-') + 1);  if (out == sel) { var finalshow = $('#ContentPlaceHolder1_tmp' + outpu1 + ' option:eq(' + i + ')').text();  options = options + '<option value=' + finalshow + '>' + finalshow + '</option>\n'; } } $('#ContentPlaceHolder1_' + outpu1 + '').html(options); } else { $('#ContentPlaceHolder1_' + outpu1 + '').val(outpu2); } $('#ContentPlaceHolder1_' + outpu1 + '').val(outpu2); }"
                                    End If
                                End If
                                con.Dispose()
                                oda.Dispose()
                                dss.Dispose()
                                '' ends here for getting master valued 
                            End If
                        ElseIf ftype.ToUpper() = "MULTI LOOKUPDDL" Then

                            Dim ddl As New DropDownList
                            ddl.ID = "fld" & FldID & "_" & row.RowIndex
                            ddl.CssClass = "txtBox"
                            ddl.Width = 150

                            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
                            Dim con As SqlConnection = New SqlConnection(conStr)
                            Dim ddlText As String = dtfld1.Rows(j).Item("dropdown").ToString()
                            Dim od As SqlDataAdapter = New SqlDataAdapter("", con)
                            Dim dt As New DataTable
                            Dim dtval As New DataTable
                            Dim fieldid As String()
                            Dim ddlval As String()
                            If (ddlText.ToString() <> String.Empty) Then
                                fieldid = ddlText.ToString().Split(",")
                                'With(nolock) added by Himank on 29th sep 2015
                                od.SelectCommand.CommandText = "select ddlMultilookupval from mmm_mst_fields  WITH(NOLOCK)   where fieldid = " & fieldid(0).ToString() & ""
                                od.Fill(dtval)
                                If (dtval.Rows.Count > 0) Then
                                    ddlval = dtval.Rows(0)("ddlMultilookupval").ToString().Split(",")
                                    For q As Integer = 0 To ddlval.Length - 1
                                        If (ddlval(q).ToString().Contains("-" & dtfld1.Rows(j).Item("FieldID").ToString())) Then
                                            Dim temp() As String = ddlval(q).ToString().Split("-")
                                            'With(nolock) added by Himank on 29th sep 2015
                                            od.SelectCommand.CommandText = "select * from MMM_MST_FIELDS  WITH(NOLOCK)   where DOCUMENTTYPE='" & temp(0).ToString() & "' and eid=" & HttpContext.Current.Session("EID") & " and fieldmapping in('" & temp(1).ToString() & "')"
                                            Dim checkval As New DataTable
                                            od.Fill(checkval)
                                            Dim ddlText1 As String = checkval.Rows(0).Item("dropdown").ToString()
                                            Dim dropdowntype As String = checkval.Rows(0).Item("dropdowntype").ToString()
                                            Dim arr() As String
                                            If UCase(dropdowntype) = "MASTER VALUED" Then
                                                Dim ob As New DynamicForm
                                                arr = ddlText1.Split("-")
                                                Dim TID As String = "TID"
                                                Dim TABLENAME As String = ""
                                                If UCase(arr(0).ToString()) = "MASTER" Then
                                                    TABLENAME = "MMM_MST_MASTER"
                                                ElseIf UCase(arr(0).ToString()) = "DOCUMENT" Then
                                                    TABLENAME = "MMM_MST_DOC"
                                                ElseIf UCase(arr(0).ToString()) = "CHILD" Then
                                                    TABLENAME = "MMM_MST_DOC_ITEM"
                                                ElseIf UCase(arr(0).ToString) = "STATIC" Then
                                                    If arr(1).ToString.ToUpper = "USER" Then
                                                        TABLENAME = "MMM_MST_USER"
                                                        TID = "UID"
                                                    ElseIf arr(1).ToString().ToUpper = "LOCATION" Then
                                                        TABLENAME = "MMM_MST_LOCATION"
                                                        TID = "LOCID"
                                                    End If
                                                End If
                                                Dim lookUpqry As String = ""
                                                Dim str As String = ""
                                                'With(nolock) added by Himank on 29th sep 2015
                                                If arr(0).ToUpper() = "CHILD" Then
                                                    str = "select " & arr(2).ToString() & "," & TID & "[tid]  from " & TABLENAME & " M  WITH(NOLOCK)   WHERE   DOCUMENTTYPE='" & arr(1).ToString() & "'"
                                                ElseIf arr(0).ToUpper() <> "STATIC" Then
                                                    str = "select " & arr(2).ToString() & "," & TID & "[tid]" & lookUpqry & " from " & TABLENAME & " M  WITH(NOLOCK)   WHERE EID=" & HttpContext.Current.Session("EID") & " AND  DOCUMENTTYPE='" & arr(1).ToString() & "'"
                                                Else
                                                    If arr(2).ToString.ToUpper = "LOCATIONSTATE" Then
                                                        str = "select DISTINCT " & arr(2).ToString() & ",SID [tid]" & lookUpqry & " from " & TABLENAME & " M   WITH(NOLOCK)  "
                                                    Else
                                                        str = "select " & arr(2).ToString() & "," & TID & "[tid]" & lookUpqry & " from " & TABLENAME & " M   WITH(NOLOCK)  WHERE EID=" & HttpContext.Current.Session("EID") & " "
                                                    End If
                                                End If

                                                Dim xwhr As String = ""
                                                Dim tids As String = ""
                                                'Dim tidarr() As String

                                                ''FILTER THE DATA ACCORDING TO USER 
                                                tids = ob.UserDataFilter(dtfld1.Rows(j).Item("documenttype").ToString(), arr(1).ToString())

                                                If tids.Length >= 2 Then
                                                    'tidarr = tids.Split("-")
                                                    xwhr = " AND CONVERT(NVARCHAR(10),TID) IN (" & tids & ")"
                                                ElseIf tids = "0" Then
                                                    'pnlFields.Visible = False
                                                    ' btnActEdit.Visible = False
                                                    '  updMain.Update()
                                                    xwhr = ""
                                                End If
                                                str = str & "  AND M.isauth=1 " & xwhr & " order by " & arr(2).ToString()
                                                Dim AutoFilter As String = dtfld1.Rows(j).Item("AutoFilter").ToString()
                                                If AutoFilter.Length > 0 Then
                                                    If arr(0).ToUpper() = "CHILD" Then
                                                        If AutoFilter.ToUpper = "DOCID" Then
                                                            str = ob.GetQuery1(arr(1).ToString, arr(2).ToString())
                                                        Else
                                                            str = ob.GetQuery(arr(1).ToString, arr(2).ToString)
                                                        End If
                                                        'With(nolock) added by Himank on 29th sep 2015
                                                    ElseIf arr(0).ToUpper() <> "STATIC" Then
                                                        str = "select " & arr(2).ToString() & ",convert(nvarchar(10),tid)  [tid]" & lookUpqry & " from " & TABLENAME & " M  WITH(NOLOCK)   WHERE EID=" & Session("EID") & " AND  DOCUMENTTYPE='" & arr(1).ToString() & "'"
                                                        str = str & "  AND M.isauth=1 " & xwhr & " order by " & arr(2).ToString()
                                                    Else
                                                        str = "select " & arr(2).ToString() & ",convert(nvarchar(10)," & TID & ")  [tid]" & lookUpqry & " from " & TABLENAME & " M  WITH(NOLOCK)   WHERE EID=" & Session("EID") & " "
                                                        str = str & "  AND M.isauth=1 " & xwhr & " order by " & arr(2).ToString()
                                                    End If
                                                End If

                                                Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
                                                Dim dss As New DataSet
                                                If str.Length > 0 Then
                                                    oda.SelectCommand.CommandText = str
                                                    oda.Fill(dss, "FV")
                                                    Dim isAddJquery As Integer = 0
                                                    ddl.Items.Add("Select")
                                                    ddl.Items(0).Value = "0"
                                                    For J1 As Integer = 0 To dss.Tables("FV").Rows.Count - 1
                                                        ddl.Items.Add(dss.Tables("FV").Rows(J1).Item(0).ToString())
                                                        Dim lookddlVal As String = dss.Tables("FV").Rows(J1).Item(1).ToString()
                                                        ddl.Items(J1 + 1).Value = lookddlVal
                                                    Next
                                                    oda.Dispose()
                                                    dss.Dispose()
                                                    'prashant_2_7

                                                    Dim lookupvalue As String = dtfld1.Rows(j).Item("lookupvalue").ToString()
                                                    Dim multilookup As String = dtfld1.Rows(j).Item("multilookUpVal").ToString()
                                                    Dim ddlmultilookup As String = dtfld1.Rows(j).Item("ddlmultilookUpVal").ToString()

                                                    If lookupvalue.Length > 0 Or multilookup.Length > 0 Or ddlmultilookup.Length > 0 Then
                                                        ddl.AutoPostBack = True
                                                        AddHandler ddl.TextChanged, AddressOf GridDdl_TextChanged
                                                    End If

                                                    If ddl.Items.FindByText(row.Cells(j).Text) IsNot Nothing Then
                                                        ddl.Items.FindByText(row.Cells(j).Text).Selected = True
                                                    End If
                                                    If ViewState("chkadd") = 1 Then
                                                        j = j + 1
                                                    End If

                                                    If ddl.Items.FindByText(row.Cells(j).Text) IsNot Nothing Then
                                                        'ddl.Items.FindByText(row.Cells(j).Text).Selected = True
                                                        ddl.SelectedValue = ddl.Items.FindByText(row.Cells(j).Text).Value
                                                    End If

                                                    row.Cells(j).Controls.Add(ddl)

                                                    If isAddJquery = 1 Then
                                                        Dim JQuertStr As String = "var r1 = $('#ContentPlaceHolder1_" & ddl.ClientID & "').val(); var l = 0; var mycars = new Array(); for (var i = 0; i < r1.length; i++) { if (r1[i] == '|') { l++; mycars[l] = i; } } for (var i1 = 1; i1 < l; i1++) { var outpu = r1.substring(mycars[i1] + 1, mycars[i1 + 1]); var outpu1 = outpu.substring(0, outpu.indexOf(':')); var outpu2 = outpu.substring(outpu.indexOf(':') + 1); if (outpu2 == 'S') { var out = r1.substring(0, mycars[1]); var x = $('#ContentPlaceHolder1_tmp' + outpu1 + ' option').length; var options = ''; txt = ''; for (i = 0; i < x; i++) { var strUser = $('#ContentPlaceHolder1_tmp' + outpu1 + ' option:eq(' + i + ')').val(); var sel = strUser.substring(strUser.indexOf('-') + 1);  if (out == sel) { var finalshow = $('#ContentPlaceHolder1_tmp' + outpu1 + ' option:eq(' + i + ')').text();  options = options + '<option value=' + finalshow + '>' + finalshow + '</option>\n'; } } $('#ContentPlaceHolder1_' + outpu1 + '').html(options); } else { $('#ContentPlaceHolder1_' + outpu1 + '').val(outpu2); } $('#ContentPlaceHolder1_' + outpu1 + '').val(outpu2); }"
                                                    End If
                                                End If
                                                con.Dispose()
                                                oda.Dispose()
                                                dss.Dispose()
                                            ElseIf UCase(dropdowntype) = "SESSION VALUED" Then

                                            End If
                                        End If
                                    Next
                                End If
                            End If
                        ElseIf ftype.ToUpper() = "CHECKBOX LIST" Then
                            Dim CHKlIST As New CheckBoxList()
                            Dim dynmdiv As System.Web.UI.HtmlControls.HtmlGenericControl = New System.Web.UI.HtmlControls.HtmlGenericControl("DIV")
                            dynmdiv.ID = "div_" & FldID
                            Dim ddlText As String = dtfld1.Rows(j).Item("dropdown").ToString()
                            CHKlIST.ID = "fld" & FldID & "_" & row.RowIndex
                            If dtfld1.Rows(j).Item("dropdowntype").ToString() = "FIX VALUED" Then
                                Dim arr = ddlText.Split(",")
                                For ii As Integer = 0 To arr.Count - 1
                                    CHKlIST.Items.Add(arr(ii).ToUpper())
                                Next

                            ElseIf dtfld1.Rows(j).Item("dropdowntype").ToString() = "MASTER VALUED" Then
                                Dim arr = ddlText.Split("-")
                                Dim TID As String = "TID"
                                Dim TABLENAME As String = ""
                                If UCase(arr(0).ToString()) = "MASTER" Then
                                    TABLENAME = "MMM_MST_MASTER"
                                ElseIf UCase(arr(0).ToString()) = "DOCUMENT" Then
                                    TABLENAME = "MMM_MST_DOC"
                                ElseIf UCase(arr(0).ToString()) = "CHILD" Then
                                    TABLENAME = "MMM_MST_DOC_ITEM"
                                ElseIf UCase(arr(0).ToString) = "STATIC" Then
                                    If arr(1).ToString.ToUpper = "USER" Then
                                        TABLENAME = "MMM_MST_USER"
                                        TID = "UID"
                                    ElseIf arr(1).ToString().ToUpper = "LOCATION" Then
                                        TABLENAME = "MMM_MST_LOCATION"
                                        TID = "LOCID"
                                    End If
                                End If
                                Dim lookUpqry As String = ""
                                Dim str As String = ""
                                'With(nolock) added by Himank on 29th sep 2015
                                If arr(0).ToUpper() = "CHILD" Then
                                    str = "select " & arr(2).ToString() & "," & TID & "[tid]  from " & TABLENAME & " M  WITH(NOLOCK)   WHERE   DOCUMENTTYPE='" & arr(1).ToString() & "'"
                                ElseIf arr(0).ToUpper() <> "STATIC" Then
                                    str = "select " & arr(2).ToString() & "," & TID & "[tid]" & lookUpqry & " from " & TABLENAME & " M  WITH(NOLOCK)   WHERE EID=" & HttpContext.Current.Session("EID") & " AND  DOCUMENTTYPE='" & arr(1).ToString() & "'"
                                Else
                                    If arr(2).ToString.ToUpper = "LOCATIONSTATE" Then
                                        str = "select DISTINCT " & arr(2).ToString() & ",SID [tid]" & lookUpqry & " from " & TABLENAME & " M  WITH(NOLOCK)   "
                                    Else
                                        str = "select " & arr(2).ToString() & "," & TID & "[tid]" & lookUpqry & " from " & TABLENAME & " M  WITH(NOLOCK)   WHERE EID=" & HttpContext.Current.Session("EID") & " "
                                    End If
                                End If
                                Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
                                Dim con As SqlConnection = New SqlConnection(conStr)
                                Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
                                oda = New SqlDataAdapter("", con)
                                Dim dss As New DataSet
                                oda.SelectCommand.CommandText = str
                                oda.Fill(dss, "FV")
                                For JJ As Integer = 0 To dss.Tables("FV").Rows.Count - 1
                                    CHKlIST.Items.Add(dss.Tables("FV").Rows(JJ).Item(0))
                                    CHKlIST.Items(JJ).Value = dss.Tables("FV").Rows(JJ).Item(1)
                                Next
                                oda.Dispose()
                                dss.Dispose()
                            End If

                            dynmdiv.Style.Add(HtmlTextWriterStyle.Width, "200px")
                            dynmdiv.Style.Add(HtmlTextWriterStyle.Height, "100px")
                            dynmdiv.Style.Add(HtmlTextWriterStyle.Overflow, "Scroll")
                            dynmdiv.Controls.Add(CHKlIST)

                            Dim txt As String = row.Cells(j).Text


                            If txt.Trim = "&nbsp;" Or txt.Trim = "" Then
                                Dim arr = txt.Split(",")
                                For i As Integer = 0 To arr.Length - 1
                                    If Not IsNothing(CHKlIST.Items.FindByValue(arr(i))) Then
                                        CHKlIST.Items.FindByValue(arr(i)).Selected = True
                                    End If
                                Next
                            End If
                            row.Cells(j).Controls.Add(dynmdiv)

                        ElseIf ftype.ToUpper() = "FILE UPLOADER" Then
                            Dim txtBox As New FileUpload

                            Dim txtBox1 As New HiddenField

                            Dim lbl1 As New Label
                            lbl1.ID = "lblf_" & FldID
                            lbl1.Text = ""
                            lbl1.ClientIDMode = ClientIDMode.Static

                            txtBox1.ID = "hdn_" & FldID
                            txtBox1.Value = ""
                            txtBox1.ClientIDMode = ClientIDMode.Static
                            txtBox.Attributes.Add("onchange", "UtilJs.UploadFile1(this,'" & txtBox1.ID.ToString() & "','" & lbl1.ID.ToString() & "')")

                            Dim hdnflag As New HiddenField
                            hdnflag.ID = "hdnf_" & FldID
                            hdnflag.Value = "0"

                            'txtBox.Visible = False
                            txtBox.ID = "fld_" & FldID & "_" & row.RowIndex
                            txtBox.CssClass = "txtBox"

                            txtBox.Width = 180


                        End If
                    Next 'Field loop End


                End If
            End If

        Catch ex As Exception

        End Try
    End Sub
    Public Sub deletedrow(ByVal sender As Object, ByVal e As GridViewCommandEventArgs)

    End Sub
    Public Sub Deleting(ByVal Sender As Object, ByVal e As GridViewDeleteEventArgs)
        Dim btnDelete As GridView = TryCast(Sender, GridView)
        btnDelete.DataBind()
    End Sub

    Protected Sub btnAddRow_Click(ByVal SENDER As Object, ByVal E As System.EventArgs)
        Dim btn As Button = CType(SENDER, Button)
        Dim grd As GridView = TryCast(pnlApprove.FindControl("GRD_" & btn.ID.ToString().Replace("ADD", "")), GridView)
        Dim dt As DataTable = Session(btn.ID.ToString().Replace("__", " ").Replace("_", ".").Replace("ADD", "") & "VAL")
        Dim dr1 As DataRow
        dr1 = dt.NewRow()
        dt.Rows.Add(dr1)
        Session(btn.ID.ToString().Replace("__", " ").Replace("_", ".").Replace("ADD", "") & "VAL") = dt
        Dim Mdt As DataTable = Session(btn.ID.ToString().Replace("__", " ").Replace("_", ".").Replace("ADD", ""))
        Dim dr2 As DataRow
        dr2 = Mdt.NewRow()
        dr2.Item("tid") = 0 'Convert.ToInt32(Mdt.Rows(Mdt.Rows.Count - 1)("tid")) + 1
        Mdt.Rows.Add(dr2)
        Session(btn.ID.ToString().Replace("__", " ").Replace("_", ".").Replace("ADD", "")) = Mdt
        grd.DataSource = Mdt
        grd.DataBind()
        ModalApprove.Show()
    End Sub

    Public Function GenerateJQueryScript(ByVal dt As DataTable, ByVal GridID As String, ByVal RowID As Integer, ByVal Formula As String, ByVal FieldID As Integer) As String
        Dim result = ""
        Try
            Dim CurRow As DataRow() = dt.Select("FieldID='" & FieldID & "'")
            Dim DisplayName = Convert.ToString(CurRow(0).Item("DisplayName"))
            Dim strFormula = ""
            'appending , in case when only one formula exists
            Formula = "," & Formula
            Dim FormulaField = ""
            'Formula = Formula.Remove("{", "").Remove("}", "")
            'Spliting all the formula with ,
            Dim arrFor As String() = Formula.Split(",")
            Dim liverFormula = From fldMapping In arrFor Where fldMapping <> "" And Not fldMapping Is Nothing Select fldMapping
            For Each Formula1 In liverFormula
                Dim arr As String() = Formula1.Split("=")
                FormulaField = arr(0).Replace("{", "").Replace("}", "")
                strFormula = arr(1)
                If arr(1).Contains("{" & DisplayName & "}") Then
                    Dim str As String() = arr(1).Split("+", "-", "*", "/", "%")
                    For Each str1 In str
                        str1 = str1.Replace("{", "").Replace("}", "")
                        Dim DR As DataRow() = dt.Select("DisplayName='" & str1 & "'")
                        If strFormula.Contains("{" & DR(0).Item("DisplayName") & "}") Then
                            strFormula = strFormula.Replace("{" & DR(0).Item("DisplayName") & "}", "parseFloat($('#GRD" & GridID & "_fld" & DR(0).Item("FieldID") & RowID & "_" & RowID & "').val())")
                        End If
                    Next
                    Dim DR1 As DataRow() = dt.Select("DisplayName='" & FormulaField & "'")
                    strFormula = "parseFloat($('#GRD" & GridID & "_fld" & DR1(0).Item("FieldID") & RowID & "_" & RowID & "').val(" & strFormula & "))"
                    If result = "" Then
                        result = strFormula
                    Else
                        result = result & ";" & strFormula
                    End If
                End If
            Next
        Catch ex As Exception
            Return ""
        End Try
        Return result
    End Function

    Public Sub ActionGridDdl_TextChanged(ByVal sender As Object, ByVal e As EventArgs)
        Dim ddl As DropDownList = TryCast(sender, DropDownList)
        Dim id As String = Right(ddl.ID, ddl.ID.Length - 3)
        Dim ar() As String = id.Split("_")
        Dim id1 As Integer = CInt(ar(0))
        ' BindGRidLookup(ar(1), id1, ddl)
        Dim ob = New DynamicForm()
        ob.GridMultiLookup(ar(1), id1, ddl, lblTabApprove, pnlApprove)
        updatePanelApprove.Update()
        'UpdatePnl4.Update()
        ModalApprove.Show()
    End Sub


    Public Sub GridDdl_TextChanged(ByVal sender As Object, ByVal e As EventArgs)
        Dim ddl As DropDownList = TryCast(sender, DropDownList)
        Dim id As String = Right(ddl.ID, ddl.ID.Length - 3)
        Dim ar() As String = id.Split("_")
        Dim id1 As Integer = CInt(ar(0))
        Dim ob As New DynamicForm()
        ob.GridMultiLookup(ar(1), id1, ddl, lblTabApprove, pnlApprove)
        BindGRidLookup(ar(1), id1, ddl)
    End Sub

    Private Sub BindGRidLookup(ByVal rowIndex As Integer, ByVal ddlid As Integer, ByRef ddl As DropDownList)
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        If con.State <> ConnectionState.Open Then
            con.Open()
        End If
        'With(nolock) added by Himank on 29th sep 2015
        Dim oda As SqlDataAdapter = New SqlDataAdapter("select LOOKUPVALUE,dropdown,documenttype,dropdowntype from MMM_MST_FIELDS  WITH(NOLOCK)   WHERE FIELDID=" & ddlid & "", con)
        Try
            Dim DS As New DataSet
            Dim xwhr As String = ""
            Dim fldType As String = ""
            oda.Fill(DS, "data")
            Dim LOOKUPVALUE As String = DS.Tables("data").Rows(0).Item("lookupvalue").ToString()
            Dim documenttype() As String = DS.Tables("data").Rows(0).Item("dropdown").ToString.Split("-")
            If LOOKUPVALUE.Length > 0 Then
                Dim lookfld() As String = LOOKUPVALUE.ToString().Split(",")  '' get all controls to fill in lookup
                If lookfld.Length > 0 Then

                    For iLookFld As Integer = 0 To lookfld.Length - 1
                        Dim fldPair() As String = lookfld(iLookFld).Split("-")   '' get fieldid and mapping
                        If fldPair.Length > 1 Then
                            Dim GRD As GridView = TryCast(ddl.Parent.Parent.Parent.Parent, GridView)
                            Dim grdRow As GridViewRow
                            Dim control As Control = Nothing

                            If IsNothing(GRD) = False Then
                                'With(nolock) added by Himank on 29th sep 2015
                                oda = New SqlDataAdapter("SELECT * FROM MMM_MST_FIELDS  WITH(NOLOCK)   WHERE FIELDID=" & fldPair(0) & "", con)
                                Dim dt As New DataTable
                                oda.Fill(dt)
                                If IsLookupField(rowIndex, GRD.ID, "fld" & Val(fldPair(0)).ToString(), dt.Rows(0).Item("FieldType").ToString()) Then  '' check if control to be filled exists
                                    ' get fld dtl from fld master
                                    Dim STR As String = ""
                                    If fldPair(1).ToString.ToUpper = "C" Then    '' fldpair(0) = fieldid to be filled 
                                        Dim proc As String = dt.Rows(0).Item("CAL_FIELDS").ToString()
                                        If proc.Length > 1 Then
                                            Dim DROPDOWN1 As String = dt.Rows(0).Item("AUTOFILTER").ToString()
                                            Dim DDL0 As DropDownList
                                            If IsNothing(GRD) = False Then
                                                grdRow = GRD.Rows(rowIndex)
                                                Dim ctrlId As String = "fld" & DROPDOWN1
                                                For i As Integer = 0 To grdRow.Cells.Count - 1
                                                    control = grdRow.Cells(i).FindControl(ctrlId & "_" & rowIndex)
                                                    If IsNothing(control) = True Then
                                                        DDL0 = CType(control, DropDownList)
                                                    End If
                                                Next
                                            End If

                                            ' = TryCast(pnlFields.FindControl("fld" & DROPDOWN1), DropDownList)
                                            If DDL0.SelectedItem.Text.ToUpper <> "SELECT" Then
                                                oda.SelectCommand.CommandType = CommandType.StoredProcedure
                                                oda.SelectCommand.Parameters.Clear()
                                                oda.SelectCommand.CommandText = proc
                                                oda.SelectCommand.Parameters.AddWithValue("DOCID", DDL0.SelectedValue)
                                                oda.SelectCommand.Parameters.AddWithValue("FIELDID", CInt(DROPDOWN1))
                                                oda.SelectCommand.Parameters.AddWithValue("VALUE", ddl.SelectedValue)
                                                Dim dss As New DataTable
                                                oda.Fill(dss)
                                                Dim ddl1 As DropDownList '= TryCast(pnlFields.FindControl("fld" & fldPair(0).ToString()), DropDownList)
                                                If IsNothing(GRD) = False Then
                                                    grdRow = GRD.Rows(rowIndex)
                                                    Dim ctrlId As String = "fld" & Val(fldPair(0)).ToString()
                                                    For i As Integer = 0 To grdRow.Cells.Count - 1
                                                        control = grdRow.Cells(i).FindControl(ctrlId & "_" & rowIndex)
                                                        If IsNothing(control) = True Then
                                                            ddl1 = CType(control, DropDownList)
                                                        End If

                                                    Next
                                                End If
                                                If IsNothing(ddl1) = False Then
                                                    ddl1.Items.Clear()
                                                    ddl1.Items.Add("SELECT")
                                                    ddl1.Items(0).Value = "0"
                                                    For i As Integer = 0 To dss.Rows.Count - 1
                                                        ddl1.Items.Add(dss.Rows(i).Item(0).ToString())
                                                        ddl1.Items(i + 1).Value = dss.Rows(i).Item("tID")
                                                    Next
                                                End If

                                            End If
                                        End If
                                    ElseIf fldPair(1).ToString.ToUpper = "R" Then
                                        Dim TAB1 As String = ""
                                        Dim TAB2 As String = ""
                                        Dim STID As String = ""
                                        Dim TID As String = ""
                                        If documenttype(0).ToString.ToUpper = "MASTER" Then
                                            TAB2 = "MMM_MST_MASTER"
                                            TID = "TID"
                                        ElseIf documenttype(0).ToString.ToUpper = "DOCUMENT" Then
                                            TAB2 = "MMM_MST_DOC"
                                            TID = "TID"
                                        ElseIf documenttype(1).ToString.ToUpper = "USER" Then
                                            TAB2 = "MMM_MST_USER"
                                            TID = "UID"
                                        End If
                                        Dim DOCTYPE() As String = dt.Rows(0).Item("DROPDOWN").ToString.Split("-")
                                        If DOCTYPE(0).ToString.ToUpper = "MASTER" Then
                                            TAB1 = "MMM_MST_MASTER"
                                            STID = "TID"
                                        ElseIf DOCTYPE(0).ToString.ToUpper = "DOCUMENT" Then
                                            TAB1 = "MMM_MST_DOC"
                                            STID = "TID"
                                        ElseIf DOCTYPE(1).ToString.ToUpper = "USER" Then
                                            TAB1 = "MMM_MST_USER"
                                            STID = "UID"
                                        End If
                                        Dim DROPDOWN1 As String = dt.Rows(0).Item("AUTOFILTER").ToString()
                                        ''Dim DDL0 As DropDownList = TryCast(pnlFields.FindControl("fld" & DROPDOWN1), DropDownList)
                                        oda.SelectCommand.CommandType = CommandType.StoredProcedure
                                        oda.SelectCommand.Parameters.Clear()
                                        oda.SelectCommand.CommandText = "USP_GETMANNUALFILTER"
                                        oda.SelectCommand.Parameters.AddWithValue("@EID", HttpContext.Current.Session("EID"))
                                        oda.SelectCommand.Parameters.AddWithValue("@TAB1", TAB1)
                                        oda.SelectCommand.Parameters.AddWithValue("@TAB2", TAB2)
                                        oda.SelectCommand.Parameters.AddWithValue("@DOCUMENTTYPE", DOCTYPE(1).ToString)
                                        oda.SelectCommand.Parameters.AddWithValue("@FLDMAPPING", DOCTYPE(2).ToString)
                                        oda.SelectCommand.Parameters.AddWithValue("@AUTOFILTER", dt.Rows(0).Item("AUTOFILTER").ToString())
                                        oda.SelectCommand.Parameters.AddWithValue("@TID", TID)
                                        oda.SelectCommand.Parameters.AddWithValue("@STID", STID)
                                        oda.SelectCommand.Parameters.AddWithValue("@VAL", ddl.SelectedValue)
                                        Dim dss As New DataTable
                                        oda.Fill(dss)
                                        Dim ddl1 As DropDownList
                                        If IsNothing(GRD) = False Then
                                            grdRow = GRD.Rows(rowIndex)
                                            Dim ctrlId As String = "fld" & Val(fldPair(0)).ToString()
                                            For i As Integer = 0 To grdRow.Cells.Count - 1
                                                control = grdRow.Cells(i).FindControl(ctrlId & "_" & rowIndex)
                                                If IsNothing(control) = True Then
                                                    ddl1 = CType(control, DropDownList)
                                                End If

                                            Next
                                        End If
                                        If IsNothing(ddl1) = False Then
                                            ddl1.Items.Clear()
                                            ddl1.Items.Add("SELECT")
                                            ddl1.Items(0).Value = "0"
                                            For i As Integer = 0 To dss.Rows.Count - 1
                                                ddl1.Items.Add(dss.Rows(i).Item(0).ToString())
                                                ddl1.Items(i + 1).Value = dss.Rows(i).Item("tID")
                                            Next
                                        End If


                                    Else

                                        Dim DROPDOWN As String() = dt.Rows(0).Item("DROPDOWN").ToString.Split("-")  ' this contains to be filled values 
                                        Dim TABLENAME As String = ""
                                        Dim TID As String = "TID"
                                        If UCase(DROPDOWN(0).ToString()) = "MASTER" Then
                                            TABLENAME = "MMM_MST_MASTER"
                                        ElseIf UCase(DROPDOWN(0).ToString()) = "DOCUMENT" Then
                                            TABLENAME = "MMM_MST_DOC"
                                        ElseIf UCase(DROPDOWN(0).ToString()) = "CHILD" Then
                                            TABLENAME = "MMM_MST_DOC_ITEM"
                                        ElseIf UCase(DROPDOWN(0).ToString()) = "STATIC" Then
                                            If UCase(DROPDOWN(1).ToString()) = "USER" Then
                                                TABLENAME = "MMM_MST_USER"
                                                TID = "UID"
                                            ElseIf UCase(DROPDOWN(1).ToString()) = "LOCATION" Then
                                                TABLENAME = "MMM_MST_LOCATION"
                                                If DROPDOWN(2).ToString.ToUpper = "LOCATIONSTATE" Then
                                                    TID = "SID"
                                                Else
                                                    TID = "LOCID"
                                                End If
                                            Else
                                                TABLENAME = dt.Rows(0).Item("DBTABLENAME").ToString
                                            End If
                                        ElseIf UCase(DROPDOWN(0).ToString()) = "SESSION" Then
                                            TABLENAME = "MMM_MST_USER"
                                            TID = "UID"
                                        End If
                                        Dim SLVALUE As String() = ddl.SelectedValue.Split("|")
                                        If dt.Rows(0).Item("fieldtype").ToString.ToUpper() = "AUTOCOMPLETE" Then

                                            If IsLookupField(rowIndex, GRD.ID, "fld" & Val(fldPair(0)).ToString(), dt.Rows(0).Item("FieldType").ToString()) Then


                                                Dim txtBox As TextBox
                                                grdRow = GRD.Rows(rowIndex)
                                                For i As Integer = 0 To grdRow.Cells.Count - 1
                                                    control = grdRow.Cells(i).FindControl("fld" & Val(fldPair(0)).ToString())
                                                    If IsNothing(control) = True Then
                                                        txtBox = CType(control, TextBox)
                                                        txtBox.Text = String.Empty
                                                    End If

                                                Next


                                            End If

                                        ElseIf dt.Rows(0).Item("fieldtype").ToString.ToUpper() = "DROP DOWN" Then
                                            Dim ob As New DynamicForm
                                            Dim AUTOFILTER As String = dt.Rows(0).Item("AUTOFILTER").ToString()
                                            Dim tids As String = ""

                                            ''Filter Data according to Userid
                                            tids = ob.UserDataFilter(DS.Tables("data").Rows(0).Item("documenttype").ToString(), DROPDOWN(1).ToString())
                                            If tids.Length > 2 Then
                                                xwhr = " AND CONVERT(NVARCHAR(10),TID) IN (" & tids & ")"
                                            Else
                                                xwhr = ""
                                            End If

                                            Dim ChildDoctype As String = ""

                                            If DS.Tables("data").Rows(0).Item("dropdowntype") = "CHILD VALUED" Then
                                                Dim ChildMid() As String = documenttype(1).ToString.Split(":")
                                                ChildDoctype = ChildMid(1).ToString
                                            End If

                                            oda.SelectCommand.CommandType = CommandType.StoredProcedure
                                            oda.SelectCommand.CommandText = "USP_BINDDDL"
                                            oda.SelectCommand.Parameters.Clear()
                                            oda.SelectCommand.Parameters.AddWithValue("@EID", HttpContext.Current.Session("EID"))
                                            oda.SelectCommand.Parameters.AddWithValue("@TableName", TABLENAME)
                                            oda.SelectCommand.Parameters.AddWithValue("@Val", SLVALUE(0))
                                            oda.SelectCommand.Parameters.AddWithValue("@xwhr", xwhr)
                                            oda.SelectCommand.Parameters.AddWithValue("@tid", TID)
                                            If ChildDoctype.Length > 0 Then
                                                oda.SelectCommand.Parameters.AddWithValue("@documenttype", ChildDoctype)
                                            Else
                                                oda.SelectCommand.Parameters.AddWithValue("@documenttype", DROPDOWN(1))
                                            End If
                                            oda.SelectCommand.Parameters.AddWithValue("@fldmapping", DROPDOWN(2))
                                            oda.SelectCommand.Parameters.AddWithValue("@autofilter", AUTOFILTER)
                                            'oda.SelectCommand.CommandText = STR & " AND isAuth=1 " & xwhr & " Order by " & DROPDOWN(2).ToString()
                                            Dim dtFinal As New DataTable
                                            oda.Fill(dtFinal)
                                            Dim ddlo As DropDownList

                                            If IsNothing(GRD) = False Then
                                                grdRow = GRD.Rows(rowIndex)
                                                Dim ctrlId As String = "fld" & Val(fldPair(0)).ToString()
                                                'For i As Integer = 0 To grdRow.Cells.Count - 1
                                                control = grdRow.FindControl(ctrlId & "_" & rowIndex)
                                                'control = grdRow.Cells(i).FindControl(ctrlId & "_" & rowIndex)
                                                If IsNothing(control) = False Then
                                                    ddlo = CType(control, DropDownList)
                                                End If

                                                ' Next
                                            End If
                                            If IsNothing(ddlo) = False Then
                                                ddlo.Items.Clear()
                                                ddlo.Items.Add("SELECT")
                                                ddlo.Items(0).Value = "0"
                                                For i As Integer = 0 To dtFinal.Rows.Count - 1
                                                    ddlo.Items.Add(dtFinal.Rows(i).Item(0).ToString())
                                                    ddlo.Items(i + 1).Value = dtFinal.Rows(i).Item("tID")
                                                Next
                                            End If

                                        Else
                                            Dim TID1 As String() = ddl.SelectedValue.ToString.Split("|")
                                            Dim SELTID As String = ""
                                            If TID1.Length > 1 Then
                                                SELTID = TID1(1).ToString
                                            Else
                                                SELTID = TID1(0).ToString
                                            End If
                                            Dim value As String = ""
                                            Dim ChildDoctype As String = ""
                                            If DS.Tables("data").Rows(0).Item("dropdowntype") = "CHILD VALUED" Then
                                                Dim ChildMid() As String = documenttype(1).ToString.Split(":")
                                                ChildDoctype = ChildMid(1).ToString
                                            End If

                                            If SELTID.ToString <> "0" And SELTID.ToString <> "" Then
                                                oda = New SqlDataAdapter("", con)
                                                oda.SelectCommand.CommandType = CommandType.StoredProcedure
                                                oda.SelectCommand.Parameters.Clear()
                                                oda.SelectCommand.CommandText = "uspGetMasterValue"
                                                oda.SelectCommand.Parameters.AddWithValue("EID", HttpContext.Current.Session("EID"))
                                                If ChildDoctype.Length > 0 Then
                                                    oda.SelectCommand.Parameters.AddWithValue("documentType", ChildDoctype)
                                                Else
                                                    oda.SelectCommand.Parameters.AddWithValue("documentType", documenttype(1))
                                                End If
                                                oda.SelectCommand.Parameters.AddWithValue("Type", documenttype(0))
                                                oda.SelectCommand.Parameters.AddWithValue("TID", SELTID)
                                                oda.SelectCommand.Parameters.AddWithValue("FLDMAPPING", fldPair(1))
                                                If con.State <> ConnectionState.Open Then
                                                    con.Open()
                                                End If
                                                value = oda.SelectCommand.ExecuteScalar().ToString()
                                            End If

                                            If IsLookupField(rowIndex, GRD.ID, "fld" & Val(fldPair(0)).ToString(), dt.Rows(0).Item("FieldType").ToString()) Then

                                                Dim txtBox As TextBox
                                                grdRow = GRD.Rows(rowIndex)
                                                For i As Integer = 0 To grdRow.Cells.Count - 1
                                                    control = grdRow.FindControl("fld" & Val(fldPair(0) & rowIndex).ToString())
                                                    If IsNothing(control) = False Then
                                                        txtBox = CType(control, TextBox)
                                                        txtBox.Text = value
                                                    End If

                                                Next


                                            End If

                                        End If


                                    End If
                                End If
                            End If
                        End If
                    Next
                End If
            End If


        Catch ex As Exception
        Finally
            If Not con Is Nothing Then
                con.Dispose()
            End If
            If Not oda Is Nothing Then
                oda.Dispose()
            End If
        End Try


    End Sub

    Private Function IsLookupField(ByVal rowIndex As Integer, ByVal grdId As String, ByVal ctrlId As String, ByVal fldType As String) As Boolean
        Try
            Dim GRD As GridView = TryCast(pnlApprove.FindControl(grdId), GridView)
            Dim grdRow As GridViewRow
            Dim control As Control = Nothing

            If IsNothing(GRD) = False Then
                grdRow = GRD.Rows(rowIndex)
                If fldType = "Drop Down" Then
                    control = grdRow.FindControl(ctrlId & "_" & rowIndex)
                Else
                    control = grdRow.FindControl(ctrlId & rowIndex)
                End If

            End If
            If IsNothing(control) = False Then
                Return True
            Else
                Return False
            End If
        Catch ex As Exception
            Return False
        End Try

    End Function
    Private Sub FillMainPage(DocId As String, rowIndex As Integer)
        'Session("FIELDS") = Nothing
        Session("ACTION") = Nothing
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim da As SqlDataAdapter = New SqlDataAdapter("SELECT M.fname,M.docurl,D.Userid,m.ouid,M.curstatus,m.curdocNature, f.FormDESC,F.FormName,M.DocumentType  from MMM_MST_DOC M  LEFT OUTER JOIN MMM_DOC_DTL D on D.tid =M.lasttid LEFT OUTER JOIN MMM_MST_FORMS F on F.FormName=M.documenttype  and F.EID=M.EID where M.TID=" & DocId, con)
        Dim ds As New DataSet()
        da.Fill(ds, "data")

        da.SelectCommand.CommandText = "select * from MMM_MST_WORKFLOW_STATUS with(nolock) where eid=" & Session("EID") & " and documenttype='" & ds.Tables("data").Rows(0).Item("DocumentType").ToString() & "' and statusname='" & ds.Tables("data").Rows(0).Item("curstatus").ToString() & "' "
        da.Fill(ds, "status")

        Dim approve As ImageButton = DirectCast(gvPending.Rows(rowIndex).FindControl("btnApprove"), ImageButton)
        Dim reject As ImageButton = DirectCast(gvPending.Rows(rowIndex).FindControl("btnReject"), ImageButton)
        Dim reconsider As ImageButton = DirectCast(gvPending.Rows(rowIndex).FindControl("btnReconsider"), ImageButton)

        If ds.Tables("status").Rows.Count = 0 Then
            approve.Visible = False
            reject.Visible = False
            reconsider.Visible = False
            Exit Sub
        End If

        For Each ctl As Control In gvPending.Rows(rowIndex).Cells(0).Controls
            If TypeOf (ctl) Is ImageButton Then
                If ctl.ID = "btnApprove" Then
                    approve = DirectCast(ctl, ImageButton)
                    approve.CommandName = "Approve"
                    approve.CommandArgument = DocId
                ElseIf ctl.ID = "btnReject" Then
                    reject = DirectCast(ctl, ImageButton)
                    reject.CommandName = "Reject"
                    reject.CommandArgument = DocId
                ElseIf ctl.ID = "btnReconsider" Then
                    reconsider = DirectCast(ctl, ImageButton)
                    reconsider.CommandName = "Reconsider"
                    reconsider.CommandArgument = DocId
                End If
            End If
        Next

        If ds.Tables("status").Rows.Count = 1 Then
            If IsDBNull(ds.Tables("status").Rows(0).Item("approve")) = True Then
                approve.Visible = False
            Else
                approve.Visible = True
                btnApprove1.Text = ds.Tables("status").Rows(0).Item("approve").ToString()
                approve.ToolTip = ds.Tables("status").Rows(0).Item("approve").ToString()
                lblApp.Text = ds.Tables("status").Rows(0).Item("approve").ToString()
            End If
            If IsDBNull(ds.Tables("status").Rows(0).Item("Reject")) = True Then
                reject.Visible = False
            Else
                reject.Visible = True
                'btnreject.Text = ds.Tables("status").Rows(0).Item("approve").ToString()
                reject.ToolTip = ds.Tables("status").Rows(0).Item("Reject").ToString()
                btnPerReject.Text = ds.Tables("status").Rows(0).Item("Reject").ToString()
                lblPRej.Text = ds.Tables("status").Rows(0).Item("Reject").ToString()
            End If
            If IsDBNull(ds.Tables("status").Rows(0).Item("Reconsider")) = True Then
                reconsider.Visible = False
            Else
                reconsider.Visible = True
                'reconsider.Text = ds.Tables("status").Rows(0).Item("Reconsider").ToString()
                reconsider.ToolTip = ds.Tables("status").Rows(0).Item("Reconsider").ToString()
                btnReject.Text = ds.Tables("status").Rows(0).Item("Reconsider").ToString()
                lblRej.Text = ds.Tables("status").Rows(0).Item("Reconsider").ToString()
            End If
        End If

        If ds.Tables("status").Rows(0).Item("AllowOnDashBoard").ToString() = "0" Then
            approve.Visible = False
            reject.Visible = False
            reconsider.Visible = False
        End If


        Dim qry = "Select count(ouid) from mmm_mst_Doc  with(nolock) where tid=" & DocId & " and curstatus='UPLOADED' and ouid=" & Session("UID")
        da.SelectCommand.CommandText = qry
        con.Open()
        Dim c = Convert.ToInt32(da.SelectCommand.ExecuteScalar())
        con.Close()
        If c > 0 Then
            reconsider.Visible = False
            reject.ToolTip = "Cancel"
            btnPerReject.Text = "CANCEL"
            btnApprove1.Text = "SUBMIT"
            approve.ToolTip = "SUMMIT"
        End If

    End Sub

    Protected Sub editBtnApprove(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnApprove1.Click

        'ShowApprove(Session("DocID").ToString)

        Dim dt As DataTable
        dt = CType(Session("FIELDS"), DataTable)
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        con.Open()
        Dim trans As SqlTransaction = Nothing
        trans = con.BeginTransaction()
        Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
        oda.SelectCommand.Transaction = trans
        Dim strfld As String = ""
        Dim insertSql As String = ""
        Try
            If IsNothing(dt) Then
                trans.Rollback()
                Exit Sub
            End If
            If dt.Rows.Count = 0 Then
            Else
                For k As Integer = 0 To dt.Rows.Count - 1
                    strfld &= dt.Rows(k).Item("fieldmapping") & ","
                Next
                strfld = Left(strfld, strfld.Length - 1)
                oda.SelectCommand.CommandType = CommandType.Text
                oda.SelectCommand.CommandText = "select " & strfld & " from MMM_MST_DOC   where eid=" & Session("EID") & " and tid=" & Session("DocID").ToString() & " "
                Dim flddt As New DataTable
                oda.Fill(flddt)
                Dim strFldVal As String = ""
                For j As Integer = 0 To flddt.Columns.Count - 1
                    strFldVal &= "'" & flddt.Rows(0).Item(j).ToString & "',"
                Next
                strFldVal = Left(strFldVal, strFldVal.Length - 1)

                '''' prev disabled becoz this is moved from here to approve procedure below by sunil on 07-oct-13 
                ' oda.SelectCommand.CommandText = "Insert Into MMM_MST_HISTORY(DOCID,documenttype,EID,Tablename,uid,uaction,adate," & strfld & ")Values(" & Session("DocID").ToString() & ",'" & Session("docType") & "'," & Session("EID") & " ,'MMM_MST_DOC'," & Session("UID") & ",'ACTION',GETDATE()," & strFldVal & ")"
                ' oda.SelectCommand.ExecuteNonQuery()
                ' oda.Dispose()
                insertSql = "Insert Into MMM_MST_HISTORY(DOCID,documenttype,EID,Tablename,uid,uaction,adate," & strfld & ")Values(" & Session("DocID").ToString() & ",'" & Session("docType") & "'," & Session("EID") & " ,'MMM_MST_DOC'," & Session("UID") & ",'ACTION',GETDATE()," & strFldVal & ")"

            End If

            Dim Updateqry As String = "UPDATE MMM_MST_DOC SET "
            Dim ob As New DynamicForm
            Dim retMsg As String = ob.ValidateAndGenrateQueryForControls("UPDATE", "UPDATE MMM_MST_DOC SET ", "", dt, pnlApprove, Session("DocID").ToString())
            If Trim(Left(retMsg, 6)).ToUpper() = "PLEASE" Then
                lblTabApprove.Text = retMsg
                updatePanelApprove.Update()
                'UpdatePnl4.Update()
                ModalApprove.Show()
                Exit Sub
            Else
                If (Not IsNothing(Session("ActionForm"))) Then

                    Dim strs As String() = Session("CHILDACTIONSCREEN").ToString().Split(",")
                    For i As Integer = 0 To strs.Length - 1
                        If String.IsNullOrEmpty(strs(i)) Then
                            Continue For
                        End If
                        Dim childvalue As String() = strs(i).ToString().Split(".")
                        Dim conStrsS As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
                        Dim consS As New SqlConnection(conStrsS)
                        Dim dasS As SqlDataAdapter = New SqlDataAdapter("", consS)
                        Dim odaN As SqlDataAdapter = New SqlDataAdapter("", consS)
                        Dim dtChildS As New DataTable()
                        Dim create As Boolean = False, delete As Boolean = False, Edit As Boolean = False
                        'With(nolock) added by Himank on 29th sep 2015
                        dasS.SelectCommand.CommandText = "select fieldid,displayname,fieldtype from mmm_mst_fields  WITH(NOLOCK)   where documenttype='" & strs(i).ToString() & "' and eid=" & Session("EID")
                        dasS.Fill(dtChildS)
                        Dim gv As GridView = DirectCast(pnlApprove.FindControl("GRD_" & strs(i).ToString().Replace(" ", "__").Replace(".", "_")), GridView)

                        Dim childvalidation As String = ""

                        childvalidation = ValidatingActionFormChildItem(strs(i).ToString())

                        If childvalidation.Length > 5 Then
                            lblTabApprove.Text = childvalidation
                            lblTabApprove.Visible = True
                            trans.Rollback()
                            updatePanelApprove.Update()
                            'UpdatePnl4.Update()
                            ModalApprove.Show()
                            Exit Sub
                        Else
                            lblTabApprove.Text = ""
                            lblTabApprove.Visible = False
                        End If
                        'SavingActionChildItem(strs(i).ToString(), Convert.ToInt32(Session("DocID")), con, trans)

                    Next 'Child Items loop End


                End If

                Dim RDocType As String = "NO Rule configured"
                If (dt.Rows.Count > 0) Then
                    RDocType = dt.Rows(0).Item("documenttype").ToString()
                End If
                'Code For Rule Engine By Ajeet Kumar Dated :30-july-2014
                Dim dv As DataView = dt.DefaultView
                dv.RowFilter = "IsActive=1"
                Dim theFields As DataTable = dv.ToTable
                Dim lstData As New List(Of UserData)
                Dim obj As New DynamicForm()
                'Creating collection for rule engine execution
                lstData = obj.CreateCollection(pnlApprove, theFields)
                'Setting it to session for getting it's value for child Item validation
                'Creating object of rule response
                Dim ObjRet As New RuleResponse()
                'Initialising rule Object
                Dim ObjRule As New RuleEngin(Session("EID"), RDocType, "CREATED", "SUBMIT")
                'Uncomment
                ObjRet = ObjRule.ExecuteRule(lstData, Nothing, False)
                If ObjRet.Success = False Then
                    lblTabApprove.Text = ObjRet.Message
                    updatePanelApprove.Update()
                    'UpdatePnl4.Update()
                    ModalApprove.Show()
                    Exit Sub
                Else
                    retMsg = retMsg & " WHERE tid=" & Session("DocID").ToString()
                    If insertSql.Length > 1 Then
                        retMsg = insertSql & ";" & retMsg
                    End If

                    '' code to update action screen fields in main document. 
                    '' written by sunil on 25-mar-14 for document user selection in movement ( HCL)
                    If retMsg.Length > 45 Then
                        oda.SelectCommand.CommandType = CommandType.Text
                        oda.SelectCommand.CommandText = retMsg
                        If con.State <> ConnectionState.Open Then con.Open()
                        Dim R As String = oda.SelectCommand.ExecuteNonQuery().ToString
                        ' ScriptManager.RegisterStartupScript(Me, Me.GetType(), "popup", "alert('Action completed successfully!');", True)
                    End If
                    '' code to update action screen fields in main document. 

                    '' new for checking priority field and setting doc's static field to 1 if found - by sunil on 28_apr_14
                    Dim docID As String = Session("DocID").ToString()
                    Dim df As New DynamicForm()
                    Call df.ChecknUpdatePriorityFieldvaluesT(docID, con, trans)
                    '' new for checking priority field and setting doc's static field to 1 if found - by sunil on 28_apr_14

                    '' changed from here by sunil on 07-oct-13  ' then again on 06_July_15 by sp
                    Dim ob1 As New DMSUtil()
                    Dim res As String
                    Dim sretMsgArr() As String

                    res = ob1.GetNextUserFromRolematrixT(Val(Session("DocID").ToString()), Val(Session("EID").ToString()), Val(Session("UID").ToString()), retMsg, Val(Session("UID").ToString()), con, trans)
                    sretMsgArr = res.Split(":")
                    '''' check if no skip setting and if not allowed then don't move doc and show msg to user

                    If res = "mismatch, try again later" Then
                        lblTabApprove.Text = "Temporarily unable to approve document, Try again later!"
                        trans.Rollback()
                        Exit Sub
                    End If

                    If sretMsgArr(0).ToUpper() = "NO SKIP" Then
                        lblTabApprove.Text = "Next Approvar/User not found, please contact Admin"
                        trans.Rollback()
                        Exit Sub
                    End If

                    If res = "User Not Authorised" Then
                        lblTabApprove.Text = "You are not authorised to Approve this document"
                        trans.Rollback()
                        Exit Sub
                    End If

                    If res = "Can not Approve, Reached to ARCHIVE" Then
                        lblTabApprove.Text = "Can not Approve, Reached to ARCHIVE"
                        trans.Rollback()
                        Exit Sub
                    End If

                    If res = "Current and Next Status cannot be same" Then
                        lblTabApprove.Text = "Current and Next Status cannot be same"
                        trans.Rollback()
                        Exit Sub
                    End If

                    '' changed from here by sunil on 07-oct-13  ' then again on 06_July_15 by sp

                    Session("ACTION") = Nothing
                    Session("FIELDS") = Nothing
                    'Dim sretMsgArr() As String = res.Split(":")

                    ''Update Kicking Fields
                    Dim strQry As String = ""
                    Dim rw() As DataRow = dt.Select("kc_value is not null and kc_status is not null")
                    For i As Integer = 0 To rw.Length - 1
                        If rw(i).Item("KC_STATUS").ToString().ToUpper = sretMsgArr(0).ToUpper() Then
                            Dim ODF As New DynamicForm()
                            Dim TXTBOX As TextBox = TryCast(pnlApprove.FindControl("fld" & rw(i).Item("fieldid").ToString()), TextBox)
                            strQry &= ODF.UPDATEKICKING(rw(i).Item("KC_VALUE").ToString(), TXTBOX.Text, pnlApprove)
                            strQry &= ";"
                        End If
                    Next
                    If strQry.Length > 1 Then
                        strQry = strQry.Substring(0, strQry.Length - 1)
                        oda.SelectCommand.CommandType = CommandType.Text
                        oda.SelectCommand.CommandText = strQry
                        oda.SelectCommand.ExecuteScalar()
                    End If
                    dt.Dispose()
                    oda.Dispose()
                    If sretMsgArr(0) = "ARCHIVE" Then
                        Dim Op As New Exportdata()
                        Op.PushdataT(Val(Session("DocID").ToString()), sretMsgArr(1), Session("EID"), con, trans)
                    Else
                        ' below lines moved from here to above for mail sending on archive status 
                        'Dim ob1 As New DMSUtil()
                        'ob1.TemplateCalling(Val(Session("DocID").ToString()), Session("EID").ToString(), "", "APPROVE")
                    End If
                    Try
                        Trigger.ExecuteTriggerT(Session("ActionForm"), Session("EID"), Session("DocID").ToString(), con, trans, TriggerNature:="Create")
                    Catch ex As Exception
                        Throw
                    End Try

                    If (Not IsNothing(Session("ActionForm"))) Then

                        Dim strs As String() = Session("CHILDACTIONSCREEN").ToString().Split(",")
                        For i As Integer = 0 To strs.Length - 1
                            If String.IsNullOrEmpty(strs(i)) Then
                                Continue For
                            End If
                            SavingActionChildItem(strs(i).ToString(), Convert.ToInt32(Session("DocID")), con, trans)
                        Next
                    End If
                    trans.Commit()
                    Try
                        ob1.TemplateCalling(Val(Session("DocID").ToString()), Session("EID").ToString(), "", "APPROVE")
                    Catch ex As Exception
                    End Try

                    ModalApprove.Hide()
                    ScriptManager.RegisterStartupScript(Me, Me.GetType(), "popup", "alert('Action completed successfully!'); ", True)

                End If
            End If
            dt.Dispose()
            oda.Dispose()

            ' code clear panel
            ob.CLEARDYNAMICFIELDS(pnlApprove)
            pnlApprove.Controls.Clear()
            updatePanelApprove.Update()
            'UpdatePnl4.Update()
            '
        Catch ex As Exception
            trans.Rollback()
            Throw
            ' ScriptManager.RegisterStartupScript(Me, Me.GetType(), "popup", "alert('Error occured at server!');", True)
        Finally
            If Not con Is Nothing Then
                con.Close()
                con.Dispose()
                trans.Dispose()
            End If
        End Try
        Session("Approve") = Nothing
        Session("Pending") = Nothing
        ModalApprove.Hide()
        btncalendar_modalPopupExtender.Hide()
        btnPerRejectModalpopup.Hide()
        ScriptManager.RegisterStartupScript(Me, Me.GetType(), "refresh", "<script type='text/javascript'>document.getElementById('" + btnpendinggrdcl.ClientID + "').click() ;</script>", False)
    End Sub

    Protected Function ValidatingActionFormChildItem(ByVal formname As String) As String
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim ODA As SqlDataAdapter = New SqlDataAdapter("", con)
        Dim dtField As New DataTable
        Dim updquery As String = "SAVE"

        'Prashant_22_7
        Dim dateFields As Integer = 0
        Dim Flag1 As Integer = 0
        Dim ErrFlag As String = ""
        Dim skipRow As Integer = 0
        Dim lastRow As Integer = 0
        'Prashant_22_7

        Dim ob As New DynamicForm()
        ' dtField = ViewState(formname)
        'dtField = Session("D" & formname)
        Dim dtvalue As DataTable = Session(formname & "VAL")
        If con.State <> ConnectionState.Open Then
            con.Open()
        End If
        'Session("id")

        Dim MaxVal As Double = -999

        Dim dt As New DataTable
        'With(nolock) added by Himank on 29th sep 2015
        ODA.SelectCommand.CommandText = "select * from mmm_mst_fields  WITH(NOLOCK)   where eid=" & Session("eid") & " and documenttype='" & formname & "'"
        ODA.SelectCommand.CommandType = CommandType.Text
        ODA.SelectCommand.Parameters.Clear()
        ODA.Fill(dt)
        dtField = dt

        Dim dtTotal As New DataTable
        ODA.SelectCommand.CommandType = CommandType.Text
        'With(nolock) added by Himank on 29th sep 2015
        ODA.SelectCommand.CommandText = "SELECT F1.FieldID [MainFieldID],F2.FieldID [ChildFieldID],F2.displayName [CdisplayName],f1.fieldtype [MFIELDTYPE] ,F1.dropdown [mDropDown],F1.displayName [MdisplayName],F1.FieldMapping [MFieldMapping]  FROM MMM_MST_FIELDS F1  WITH(NOLOCK)   INNER JOIN MMM_MST_FIELDS F2  WITH(NOLOCK)   ON F1.dropdown =CONVERT(NVARCHAR(20),F2.Fieldid)  WHERE F1.EID=" & Session("EID") & " AND F1.FieldType in ('CHILD ITEM MAX','CHILD ITEM TOTAL') AND F2.DocumentType ='" & formname & "' AND F1.DOCUMENTTYPE='" & Session("Document") & "'"
        ODA.Fill(dtTotal)

        Dim isTotal As Boolean = False
        Dim cDispName As String = ""
        Dim Childtotal As Double = 0
        If dtTotal.Rows.Count = 1 Then
            isTotal = True
            cDispName = dtTotal.Rows(0).Item("cdisplayname").ToString
        End If

        Dim strs As String() = Session("CHILDACTIONSCREEN").ToString().Split(",")

        Dim GV As GridView = DirectCast(pnlApprove.FindControl("GRD_" & formname.Replace(" ", "__").Replace(".", "_")), GridView)
        If IsNothing(GV) Then
            Return ""
        End If

        Dim cnt As Integer = 0
        For Each GR As GridViewRow In GV.Rows
            If GR.RowType = DataControlRowType.DataRow Then
                If GR.Cells(0).Text.ToUpper <> "TOTAL" Then

                    Dim STRFld As String = ""
                    Dim STRVal As String = ""

                    dateFields = 0
                    ErrFlag = ""
                    skipRow = 0

                    For j As Integer = 0 To dtField.Rows.Count - 1
                        STRFld &= dtField.Rows(j).Item("fieldmapping").ToString & ","
                        Dim colValue As String = ""
                        Dim FldID As String = ""
                        FldID = dtField.Rows(j).Item("fieldid").ToString()
                        If dtField.Rows(j).Item("inlineediting").ToString = "1" Or 1 = 1 Then
                            Dim ftype As String = dtField.Rows(j).Item("fieldtype").ToString()
                            If ftype.ToUpper() = "TEXT BOX" Or ftype.ToUpper() = "CALCULATIVE FIELD" Or ftype.ToUpper() = "LOOKUP" Then
                                If dtField.Rows(j).Item("datatype").ToString().ToUpper = "NUMERIC" And dtField.Rows(j).Item("isrequired") = 1 Then
                                    Dim cb As TextBox = CType(GR.FindControl("fld" & FldID & GR.RowIndex), TextBox)
                                    If cb IsNot Nothing Then
                                        colValue = cb.Text
                                        If cb.Text.Trim = "" Then
                                            ErrFlag &= "Entered Child Value " & dtField.Rows(j).Item("displayName").ToString() & " is required, "
                                            Flag1 += 1
                                        ElseIf cb.Text.Trim = "0" Then
                                            dateFields += 1
                                        Else
                                            skipRow = 1
                                            lastRow = GR.RowIndex
                                        End If
                                    Else
                                        colValue = "0"
                                    End If
                                    If dtField.Rows(j).Item("displayname") = cDispName Then  '' new for getting child total
                                        If colValue <> "" Then
                                            Childtotal = Childtotal + Convert.ToDouble(colValue)
                                        End If
                                    End If
                                ElseIf dtField.Rows(j).Item("datatype").ToString().ToUpper = "DATETIME" Then
                                    dateFields += 1
                                ElseIf dtField.Rows(j).Item("datatype").ToString().ToUpper = "TEXT" And dtField.Rows(j).Item("isrequired") = 1 Then
                                    Dim cb As TextBox = CType(GR.FindControl("fld" & FldID & GR.RowIndex), TextBox)
                                    If cb IsNot Nothing Then
                                        If cb.Text.Trim = "" Then
                                            ErrFlag &= "Entered Child Value " & dtField.Rows(j).Item("displayName").ToString() & " is required, "
                                            Flag1 += 1
                                        Else
                                            skipRow = 1
                                            lastRow = GR.RowIndex
                                        End If
                                    End If
                                ElseIf dtField.Rows(j).Item("datatype").ToString().ToUpper = "TEXT" And dtField.Rows(j).Item("isrequired") = 0 Then
                                    dateFields += 1
                                    Dim cb As TextBox = CType(GR.FindControl("fld" & FldID & GR.RowIndex), TextBox)
                                    If cb.Text.Trim <> "" Then
                                        skipRow = 1
                                        lastRow = GR.RowIndex
                                    End If
                                ElseIf dtField.Rows(j).Item("datatype").ToString().ToUpper = "NUMERIC" And dtField.Rows(j).Item("isrequired") = 0 Then
                                    dateFields += 1
                                    Dim cb As TextBox = CType(GR.FindControl("fld" & FldID & GR.RowIndex), TextBox)
                                    If cb.Text.Trim <> "" And cb.Text.Trim <> "0" Then
                                        skipRow = 1
                                        lastRow = GR.RowIndex
                                    End If

                                Else
                                    'skipRow = 1
                                    'lastRow = GR.RowIndex
                                End If
                            End If

                            If ftype.ToUpper() = "DROP DOWN" Then
                                Dim dtFilter As New DataTable

                                Dim cb As DropDownList = CType(GR.FindControl("fld" & FldID & "_" & cnt), DropDownList)
                                If dtField.Rows(j).Item("dropdowntype").ToString() = "FIX VALUED" Then
                                    If dtField.Rows(j).Item("isrequired") = 1 Then

                                        If cb IsNot Nothing Then
                                            colValue = cb.SelectedValue.ToString

                                        Else
                                            colValue = "0"
                                        End If
                                        If colValue = "0" OrElse colValue = "" Then
                                            ErrFlag &= "Entered Child Value " & dtField.Rows(j).Item("displayName").ToString() & " is required, "
                                            Flag1 += 1
                                        Else
                                        End If
                                    Else
                                        colValue = cb.SelectedValue.ToString
                                        dateFields += 1
                                        If colValue <> "0" And colValue <> "" Then
                                            skipRow = 1
                                            lastRow = GR.RowIndex
                                        End If
                                    End If

                                ElseIf dtField.Rows(j).Item("dropdowntype").ToString() = "MASTER VALUED" Then
                                    Dim strQuery As String = "select SUBSTRING(KC_Logic,0,CHARINDEX('-', KC_Logic)) ParentDropdown,SUBSTRING(KC_Logic,CHARINDEX('-', KC_Logic) + 1,len(KC_Logic)- CHARINDEX(',', KC_Logic)-2) Childdropdown"
                                    strQuery += "  from MMM_MST_fields where documenttype ='" & Session("Doctype") & "' and eid=" & Session("EID") & " and fieldtype='Child Item' and dropdown='" & formname & "'"
                                    strQuery += " and  SUBSTRING(KC_Logic,CHARINDEX('-', KC_Logic) + 1,len(KC_Logic)- CHARINDEX(',', KC_Logic)-2)='" & FldID & "'"
                                    ODA.SelectCommand.CommandText = strQuery
                                    ODA.Fill(dtFilter)

                                    If dtFilter.Rows.Count = 0 Then

                                        If dtField.Rows(j).Item("isrequired") = 1 Then
                                            If cb IsNot Nothing Then
                                                colValue = cb.SelectedValue.ToString

                                            Else
                                                colValue = "0"
                                            End If
                                            If colValue = "0" OrElse colValue = "" OrElse colValue.ToUpper() = "SELECT" Then
                                                ErrFlag &= "Entered Child Value " & dtField.Rows(j).Item("displayName").ToString() & " is required, "
                                                Flag1 += 1
                                            Else
                                                skipRow = 1
                                                lastRow = GR.RowIndex
                                            End If
                                        Else
                                            colValue = cb.SelectedValue.ToString
                                            dateFields += 1
                                            If colValue <> "0" And colValue <> "" And colValue.ToUpper() <> "SELECT" Then
                                                skipRow = 1
                                                lastRow = GR.RowIndex
                                            End If
                                        End If

                                    Else

                                        colValue = IIf(IsNothing(cb), "", cb.SelectedValue.ToString)
                                        dateFields += 1
                                    End If

                                End If

                            End If
                            If ftype.ToUpper() = "MULTI LOOKUPDDL" Then
                                Dim cb As DropDownList = CType(GR.FindControl("fld" & FldID & "_" & GR.RowIndex), DropDownList)
                                If dtField.Rows(j).Item("isrequired") = 1 Then
                                    If cb IsNot Nothing Then
                                        colValue = cb.SelectedValue.ToString

                                    Else
                                        colValue = "0"
                                    End If
                                    If colValue = "0" OrElse colValue = "" OrElse colValue.ToUpper() = "SELECT" Then
                                        ErrFlag &= "Entered Child Value " & dtField.Rows(j).Item("displayName").ToString() & " is required, "
                                        Flag1 += 1
                                    Else
                                        skipRow = 1
                                        lastRow = GR.RowIndex
                                    End If
                                Else
                                    colValue = cb.SelectedValue.ToString
                                    dateFields += 1
                                    If colValue <> "0" And colValue <> "" And colValue.ToUpper() <> "SELECT" Then
                                        skipRow = 1
                                        lastRow = GR.RowIndex
                                    End If
                                End If
                            End If

                            If ftype.ToUpper() = "FILE UPLOADER" Then
                                Dim cell As Integer = -1
                                Dim txtBox As FileUpload = CType(GR.FindControl("fld_" & FldID & "_" & cnt), FileUpload)

                                'Prashant 30-6-2014
                                Dim txtBox1 As Label = CType(GR.FindControl("lblf_" & FldID), Label)

                                Dim hdn As HiddenField = CType(GR.FindControl("hdn_" & FldID), HiddenField)
                                txtBox1.Text = hdn.Value


                                If txtBox1.Text IsNot "" Then
                                    'Prashant
                                    skipRow = 1
                                    lastRow = GR.RowIndex

                                    Dim ext As String = ""
                                    Dim flag As Integer = 0
                                    Dim Validext As String = ""
                                    Dim sourceFile As String = ""
                                    Dim curFileSize As Integer = 0
                                    If Not Request.QueryString("TID") Is Nothing Then
                                        sourceFile = Server.MapPath("~/DOCS/" & Session("EID") & "/" + txtBox1.Text)
                                        Dim finfo As New FileInfo(Server.MapPath("~/DOCS/" & Session("EID") & "/" + txtBox1.Text))

                                        ext = txtBox1.Text.Substring(txtBox1.Text.LastIndexOf("."), txtBox1.Text.Length - txtBox1.Text.LastIndexOf("."))
                                        curFileSize = finfo.Length
                                    Else
                                        sourceFile = Server.MapPath("~/DOCS/temp/" + txtBox1.Text)

                                        Dim finfo
                                        Dim hdnf As HiddenField = CType(GR.FindControl("hdnf_" & FldID), HiddenField)
                                        If hdnf.Value = "0" Then
                                            finfo = New FileInfo(Server.MapPath("~/DOCS/" & Session("EID") & "/" + txtBox1.Text))
                                        Else
                                            finfo = New FileInfo(Server.MapPath("~/DOCS/temp/" + txtBox1.Text))
                                        End If
                                        ' Dim finfo As New FileInfo(Server.MapPath("~/DOCS/temp/" + txtBox1.Text))

                                        ext = txtBox1.Text.Substring(txtBox1.Text.LastIndexOf("."), txtBox1.Text.Length - txtBox1.Text.LastIndexOf("."))
                                        curFileSize = finfo.Length
                                    End If
                                    If IsDBNull(dtField.Rows(j).Item("UploadAllowedTypes")) = True OrElse Trim(dtField.Rows(j).Item("UploadAllowedTypes")) = "" Then
                                    Else
                                        Dim type As String() = Split(dtField.Rows(j).Item("UploadAllowedTypes").ToString(), ",")
                                        For k As Integer = 0 To type.Length - 1
                                            If type(k) = ext Then
                                                flag = 0 ' if file type is match then passed and saved into DB
                                                Exit For
                                            Else
                                                flag = 1 ' check for type of the not matched
                                                Validext &= type(k).ToString() & ","
                                            End If
                                        Next
                                        If flag = 0 Then
                                        Else
                                            ErrFlag &= "Invalid file type in (" & formname & ") " & dtField.Rows(j).Item("displayName").ToString() & ", "
                                            Flag1 += 1
                                        End If
                                    End If

                                Else
                                    If dtField.Rows(j).Item("isrequired").ToString = "1" Then
                                        ErrFlag &= "Entered Child Value " & dtField.Rows(j).Item("displayName").ToString() & " is required, "
                                        Flag1 += 1
                                    Else
                                        'colValue = txtBox1.Text.ToString
                                        dateFields += 1
                                        'If txtBox1.Text.ToString.Trim = "" Then
                                        '    skipRow = 1
                                        '    lastRow = GR.RowIndex
                                        'End If
                                    End If

                                End If
                            End If
                        End If
                    Next
                    cnt += 1
                End If
            End If

            If Flag1 < GV.Rows(0).Cells.Count - 1 - dateFields And ErrFlag.Trim <> "" Then
                If skipRow = 1 Then
                    updquery = ErrFlag
                    Return updquery.Substring(0, updquery.Length - 2)
                    Exit Function
                End If

            Else

                'For Rule Engin
                'If GR.RowType = DataControlRowType.DataRow Then
                '    If GR.Cells(0).Text.ToUpper <> "TOTAL" And GR.RowIndex <= lastRow Then

                '        Dim retObj As New RuleResponse()
                '        Dim lstData As New List(Of UserData)
                '        lstData = CType(Session("pColl"), List(Of UserData))
                '        Dim lstChildData As New List(Of UserData)
                '        Dim obj As New DynamicForm()

                '        obj.CreateCollection(pnlFields, dtField)
                '        Dim onlyFiltered As DataView = dtField.DefaultView
                '        onlyFiltered.RowFilter = "InlineEditing='1'"
                '        Dim dtFields As DataTable = onlyFiltered.Table.DefaultView.ToTable()

                '        lstChildData = obj.CreateGVCollection(GR, dtFields, GR.RowIndex)
                '        'Initialising Rule objects
                '        Dim objRule As New RuleEngin(Session("EID"), formname, "CREATED", "Submit")
                '        retObj = objRule.ExecuteRule(lstData, lstChildData, True)
                '        If retObj.Success = False Then
                '            Return retObj.Message
                '            Exit Function
                '        End If
                '    End If
                'End If

            End If

        Next
        ''Return "Child form validated validated ..." & lastRow
        'Now Code For Unique validation
        Dim objU As New UpdateData()

        Dim dskeys As New DataSet()

        Dim DisplayMsg = ""

        dskeys = objU.GetKeys(Session("EID"), formname)

        Dim Keys As String = ""
        Dim arrlist As New ArrayList()
        If dskeys.Tables(0).Rows.Count > 0 Then
            If dskeys.Tables(0).Rows.Count > 0 Then
                For Each GR As GridViewRow In GV.Rows
                    Keys = ""
                    If GR.RowIndex <= lastRow Then
                        If GR.RowType = DataControlRowType.DataRow Then
                            If GR.Cells(0).Text.ToUpper <> "TOTAL" Then
                                DisplayMsg = ""
                                For m As Integer = 0 To dskeys.Tables(0).Rows.Count - 1
                                    Dim FieldID = dskeys.Tables(0).Rows(m).Item("FieldID").ToString
                                    Dim FieldType = dskeys.Tables(0).Rows(m).Item("FieldType").ToString
                                    If DisplayMsg = "" Then
                                        DisplayMsg = dskeys.Tables(0).Rows(m).Item("DisplayName").ToString
                                    Else
                                        DisplayMsg = DisplayMsg & ", " & dskeys.Tables(0).Rows(m).Item("DisplayName").ToString
                                    End If
                                    Select Case FieldType.Trim.ToUpper
                                        Case "TEXT BOX"
                                            Dim txtBox As TextBox = CType(GR.FindControl("fld" & FieldID & GR.RowIndex), TextBox)
                                            Keys = Keys & txtBox.Text.Trim.ToUpper()
                                        Case "DROP DOWN"
                                            Dim DDL As DropDownList = CType(GR.FindControl("fld" & FieldID & "_" & GR.RowIndex), DropDownList)
                                            Keys = Keys & DDL.SelectedItem.Value
                                        Case "CALCULATIVE FIELD"
                                            Dim txtBox As TextBox = CType(GR.FindControl("fld" & FieldID & GR.RowIndex), TextBox)
                                            Keys = Keys & txtBox.Text.Trim.ToUpper()
                                        Case "LOOKUP"
                                            Dim txtBox As TextBox = CType(GR.FindControl("fld" & FieldID & GR.RowIndex), TextBox)
                                            Keys = Keys & txtBox.Text.Trim.ToUpper()
                                    End Select
                                Next
                                'Code For checking duplicate rows
                                Dim Test = Keys
                                Dim IsDuplicate = False
                                For Each item In arrlist
                                    If Keys.Trim.ToUpper = item.ToString.Trim.ToUpper Then
                                        IsDuplicate = True
                                    End If
                                Next
                                If IsDuplicate = False Then
                                    arrlist.Add(Test)
                                Else
                                    Return "Please check line number " & (GR.RowIndex + 1) & " of " & formname & "," & " Combination of " & DisplayMsg & " must be unique."
                                    Exit Function
                                End If
                            End If
                        End If
                    End If

                Next
            End If
        End If
        'Code for unique validation end here 

        Session(formname & "LastRow") = lastRow
        ViewState("LastRow") = lastRow
        ODA.Dispose()
        con.Close()
        con.Dispose()
        Return updquery

    End Function

    Protected Sub SavingActionChildItem(ByVal formname As String, ByVal docid As Integer, con As SqlConnection, trans As SqlTransaction)
        Try
            Dim ODA As SqlDataAdapter = New SqlDataAdapter("", con)
            ODA.SelectCommand.Transaction = trans
            Dim dtField As New DataTable
            ' Dim dtFD As New DataTable
            'Dim drnew As DataRow
            Dim updquery As String = ""
            Dim ob As New DynamicForm()
            If Session(formname & "VAL") Is Nothing Then
                Exit Sub
            End If

            ' dtField = ViewState(formname)
            'dtField = Session("D" & formname)
            Dim dtvalue As DataTable = Session(formname & "VAL")
            'Session("id")
            Dim dt As New DataTable
            ODA.SelectCommand.CommandText = "select * from mmm_mst_fields   where eid=" & Session("eid") & " and documenttype='" & formname & "' "
            ODA.SelectCommand.CommandType = CommandType.Text
            ODA.SelectCommand.Parameters.Clear()
            ODA.Fill(dt)
            dtField = dt
            Dim dtTotal As New DataTable
            ODA.SelectCommand.CommandType = CommandType.Text
            ODA.SelectCommand.CommandText = "SELECT F1.FieldID [MainFieldID],F2.FieldID [ChildFieldID],F2.displayName [CdisplayName] ,F1.dropdown [mDropDown],F1.displayName [MdisplayName],F1.FieldMapping [MFieldMapping],f1.fieldtype [MFIELDTYPE]  FROM MMM_MST_FIELDS F1   INNER JOIN MMM_MST_FIELDS F2  ON F1.dropdown =CONVERT(NVARCHAR(20),F2.Fieldid)  WHERE F1.EID=" & Session("EID") & " AND F1.FieldType in ('CHILD ITEM MAX','CHILD ITEM TOTAL') AND F2.DocumentType ='" & formname & "' AND F1.DOCUMENTTYPE='" & ViewState("MDOCTYPE") & "'"
            ODA.Fill(dtTotal)

            Dim isTotal As Boolean = False
            Dim cDispName As String = ""
            Dim Childtotal As Double = 0
            If dtTotal.Rows.Count = 1 Then
                isTotal = True
                cDispName = dtTotal.Rows(0).Item("cdisplayname").ToString
            End If

            Dim GID As String = dt.Rows(0).Item("fieldId").ToString
            Dim dataitem As DataTable = Session("ITEM")

            Dim GV As GridView = DirectCast(pnlApprove.FindControl("GRD_" & formname.Replace(" ", "__").Replace(".", "_")), GridView)

            If IsNothing(GV) Then
                Exit Sub
            End If
            'Prashant_21_7
            Dim gridCols As DataTable = GV.DataSource
            dataitem = gridCols
            Dim MaxVal As Double = -999
            'Prashant_17_7
            Dim LastRow As Integer = 0

            If IsNothing(Session(formname & "LastRow")) = False Then
                If Session(formname & "LastRow").ToString.Trim <> "" Then
                    LastRow = Convert.ToInt32(Session(formname & "LastRow"))
                Else
                    LastRow = GV.Rows.Count - 1
                End If
            Else
                LastRow = GV.Rows.Count - 1
            End If

            'Prashant_17_7
            Dim cnt As Integer = 0
            ' drnew = dtFD.NewRow()
            For Each GR As GridViewRow In GV.Rows
                If GR.RowIndex <= LastRow Then
                    If GR.RowType = DataControlRowType.DataRow Then
                        If GR.Cells(0).Text.ToUpper <> "TOTAL" Then

                            Dim str As String = ""
                            Dim STRFld As String = ""
                            Dim STRVal As String = ""
                            updquery = ""
                            ' 24 dec 2014 by ball start from here
                            Dim tid = 0 'IIf(IsNothing(dataitem.Rows(GR.RowIndex)), 0, dataitem.Rows(GR.RowIndex).Item("TID"))
                            If dataitem.Rows.Count > GR.RowIndex Then
                                tid = IIf(String.IsNullOrEmpty(dataitem.Rows(GR.RowIndex).Item("TID").ToString), 0, dataitem.Rows(GR.RowIndex).Item("TID"))
                            End If

                            If Val(tid) = 0 Then  ' here is the searching for tid for insertion or updation the record
                                str = "INSERT INTO MMM_MST_DOC_ITEM("
                                For j As Integer = 0 To dtField.Rows.Count - 1  ' decreasing 2 because we are not considering the total row data 
                                    STRFld &= dtField.Rows(j).Item("fieldmapping").ToString & ","  'adding  j+1 for incremental
                                    Dim colValue As String = ""
                                    Dim FldID As String = ""

                                    Dim DispName = dtField.Rows(j).Item("DisplayName").ToString

                                    FldID = dtField.Rows(j).Item("fieldid").ToString()
                                    If dtField.Rows(j).Item("inlineediting").ToString = "1" Or 1 = 1 Then

                                        Dim ftype As String = dtField.Rows(j).Item("fieldtype").ToString()
                                        If ftype.ToUpper() = "TEXT BOX" Or ftype.ToUpper() = "CALCULATIVE FIELD" Or ftype.ToUpper() = "LOOKUP" Or ftype.ToUpper() = "MULTI LOOKUP" Then
                                            Dim cb As TextBox = CType(GR.FindControl("fld" & FldID & GR.RowIndex), TextBox)
                                            If cb IsNot Nothing Then
                                                colValue = cb.Text.ToString
                                            Else
                                                colValue = "0"
                                            End If

                                            If dtField.Rows(j).Item("displayname") = cDispName Then  '' new for getting child total
                                                If colValue <> "" Then
                                                    If dtTotal.Rows(0).Item("mfieldType").ToString().ToUpper = "CHILD ITEM MAX" Then  ' by sunil for maxval
                                                        Childtotal = Childtotal + Convert.ToDouble(colValue)
                                                    ElseIf dtTotal.Rows(0).Item("mfieldType").ToString().ToUpper = "CHILD ITEM MAX" Then  ' by sunil for maxval
                                                        If Convert.ToDouble(colValue) > MaxVal Then
                                                            MaxVal = Convert.ToDouble(colValue)
                                                            Childtotal = MaxVal
                                                        End If
                                                    End If
                                                End If
                                            End If
                                        ElseIf ftype.ToUpper() = "DROP DOWN" And dtField.Rows(j).Item("dropdowntype").ToString() = "FIX VALUED" Then

                                            Dim cb As DropDownList = CType(GR.FindControl("fld" & FldID & "_" & GR.RowIndex), DropDownList)
                                            If cb IsNot Nothing Then
                                                colValue = cb.SelectedItem.Text.ToString
                                            Else
                                                colValue = "0"
                                            End If
                                        ElseIf ftype.ToUpper() = "DROP DOWN" And dtField.Rows(j).Item("dropdownTYPE") = "MASTER VALUED" Then
                                            Dim cb As DropDownList = CType(GR.FindControl("fld" & FldID & "_" & GR.RowIndex), DropDownList)
                                            If cb IsNot Nothing Then
                                                colValue = cb.SelectedItem.Value.ToString  ' use value bcoz it's mastervalued - needs tid
                                            Else
                                                colValue = "0"
                                            End If

                                        ElseIf ftype.ToUpper() = "CHECKBOX LIST" Then
                                            Dim cb As CheckBoxList = CType(GR.FindControl("fld" & FldID & "_" & cnt), CheckBoxList)
                                            If cb IsNot Nothing Then
                                                For Each item As ListItem In cb.Items
                                                    If item.Selected Then
                                                        colValue &= item.Value & ","
                                                    End If
                                                Next
                                                If colValue.Length > 0 Then
                                                    colValue = colValue.Substring(0, colValue.Length - 1)
                                                End If
                                            Else
                                                colValue = ""
                                            End If
                                        ElseIf ftype.ToUpper() = "MULTI LOOKUPDDL" Then
                                            Dim cb As DropDownList = CType(GR.FindControl("fld" & FldID & "_" & GR.RowIndex), DropDownList)
                                            If cb IsNot Nothing Then
                                                colValue = cb.SelectedItem.Value.ToString  ' use value bcoz it's mastervalued - needs tid
                                            Else
                                                colValue = "0"
                                            End If
                                        ElseIf ftype.ToUpper() = "FILE UPLOADER" Then
                                            '' here to code for getting file 
                                            Dim txtBox As FileUpload = CType(GR.FindControl("fld" & FldID & "_" & GR.RowIndex), FileUpload)
                                            'Prashant 
                                            Dim txtBox1 As Label = CType(GR.FindControl("lblf_" & FldID), Label)

                                            Dim hdn As HiddenField = CType(GR.FindControl("hdn_" & FldID), HiddenField)

                                            Dim hdnflg As HiddenField = CType(GR.FindControl("hdnf_" & FldID), HiddenField)
                                            txtBox1.Text = hdn.Value

                                            If txtBox1.Text IsNot "" And hdnflg.Value IsNot "0" Then
                                                Dim errormsg As String = ""
                                                Dim FN As String = ""
                                                Dim ext As String = ""
                                                Dim flag As Integer = 0
                                                Dim sourceFile As String = Server.MapPath("~/DOCS/temp/" + txtBox1.Text)
                                                Dim finfo As New FileInfo(Server.MapPath("~/DOCS/temp/" + txtBox1.Text))
                                                Dim curFileSize As Integer = finfo.Length
                                                ext = txtBox1.Text.Substring(txtBox1.Text.LastIndexOf("."), txtBox1.Text.Length - txtBox1.Text.LastIndexOf("."))
                                                FN = Left(txtBox1.Text, txtBox1.Text.LastIndexOf("."))
                                                colValue = Session("EID").ToString() & "/" & getSafeString(FN) & "_" & Now.Day & Now.Month & Now.Year & Now.Minute & Now.Second & Now.Millisecond & dtField.Rows(j).Item("FieldID").ToString() & "" & ext

                                                File.Copy(sourceFile, Server.MapPath("DOCS/") & Session("EID").ToString() & "/" & getSafeString(FN) & "_" & Now.Day & Now.Month & Now.Year & Now.Minute & Now.Second & Now.Millisecond & dtField.Rows(j).Item("FieldID").ToString() & ext)

                                            ElseIf txtBox1.Text IsNot "" And hdnflg.Value IsNot "1" Then
                                                colValue = txtBox1.Text
                                            End If
                                        Else

                                            For index As Integer = 0 To gridCols.Columns.Count - 1
                                                If DispName = gridCols.Columns(index).ColumnName.ToString Then
                                                    colValue = GR.Cells(index).Text.ToString()
                                                    Exit For
                                                Else
                                                    colValue = ""
                                                End If
                                            Next


                                            If dtField.Rows(j).Item("displayname") = cDispName Then  '' new for getting child total
                                                If colValue <> "" Then
                                                    If dtTotal.Rows(0).Item("mfieldType").ToString().ToUpper = "CHILD ITEM TOTAL" Then  ' by sunil for maxval
                                                        Childtotal = Childtotal + Convert.ToDouble(colValue)
                                                    ElseIf dtTotal.Rows(0).Item("mfieldType").ToString().ToUpper = "CHILD ITEM MAX" Then  ' by sunil for maxval
                                                        If Convert.ToDouble(colValue) > MaxVal Then
                                                            MaxVal = Convert.ToDouble(colValue)
                                                            Childtotal = MaxVal
                                                        End If
                                                    End If
                                                End If
                                            End If
                                        End If
                                    Else  ' if no inline editing 
                                        If dtField.Rows(j).Item("dropdownTYPE") = "MASTER VALUED" And dtField.Rows(j).Item("fieldtype") = "Drop Down" Then
                                            '' here code to get tid from session arrays by sp 13-jan-13
                                            Dim CH() As String = Session("COLHEAD")  ' {}
                                            Dim txt() As String = Session("DDLTXT") ' {}
                                            Dim Vals() As String = Session("DDLVAL") '{}
                                            Dim found As Boolean = False
                                            Dim searchHdr As String = formname & "_" & dtField.Rows(j).Item("displayname")
                                            For a As Integer = 0 To CH.Length - 1
                                                If CH(a).ToString = searchHdr And txt(a).ToString = GR.Cells(j).Text.ToString() Then ' if match found in array 
                                                    colValue = Vals(a).ToString
                                                    found = True
                                                    Exit For
                                                End If
                                            Next
                                            If found = False Then
                                                'Prashant_21_7
                                                For index As Integer = 0 To gridCols.Columns.Count - 1
                                                    If DispName = gridCols.Columns(index).ColumnName.ToString Then
                                                        colValue = dtvalue.Rows(cnt).Item(DispName).ToString()
                                                        Exit For
                                                    Else
                                                        colValue = ""
                                                    End If
                                                Next
                                                ' +1 IS ADDED FOR CELL ADJUSTMENT 
                                                'Else
                                                '    colValue = dtvalue.Rows(cnt).Item(j).ToString() ' added by balli on 2 june 
                                            End If
                                            'ElseIf dtField.Rows(j).Item("fieldtype") = "Child Item Total" Then

                                        Else

                                            For index As Integer = 0 To gridCols.Columns.Count - 1
                                                If DispName = gridCols.Columns(index).ColumnName.ToString Then
                                                    colValue = GR.Cells(index).Text.ToString()
                                                    Exit For
                                                Else
                                                    colValue = ""
                                                End If
                                            Next
                                            ' +1 IS ADDED FOR CELL ADJUSTMENT 
                                            If dtField.Rows(j).Item("displayname") = cDispName Then  '' new for getting child total
                                                If colValue <> "" Then
                                                    If dtTotal.Rows(0).Item("mfieldType").ToString().ToUpper = "CHILD ITEM TOTAL" Then  ' by sunil for maxval
                                                        Childtotal = Childtotal + Convert.ToDouble(colValue)
                                                    ElseIf dtTotal.Rows(0).Item("mfieldType").ToString().ToUpper = "CHILD ITEM MAX" Then  ' by sunil for maxval
                                                        If Convert.ToDouble(colValue) > MaxVal Then
                                                            MaxVal = Convert.ToDouble(colValue)
                                                            Childtotal = MaxVal
                                                        End If
                                                    End If
                                                End If
                                            End If
                                        End If
                                    End If
                                    STRVal &= "'" & colValue & "'" & ","
                                Next
                                ' code for insertion of document 
                                STRFld &= "DOCID,documenttype,isauth,cmastertid)" ' 24 dec inserting cmastertid

                                Dim cMasterTid As String = ""
                                If dataitem.Rows.Count > GR.RowIndex Then
                                    Dim cols As DataColumnCollection = dataitem.Columns
                                    If cols.Contains("cmasterTid") Then
                                        cMasterTid = IIf(String.IsNullOrEmpty(dataitem.Rows(GR.RowIndex).Item("cmasterTid")), "", dataitem.Rows(GR.RowIndex).Item("cmasterTid").ToString())
                                    End If
                                End If
                                Dim orgDoc = formname.Split(".")
                                STRVal &= docid & "," & "'" & orgDoc(1) & "'" & ",1," & IIf(cMasterTid = "", "null", cMasterTid) & ")"
                                'STRVal &= docid & "," & "'" & formname & "'" & ",1," & gridCols.Columns("cmasterTid").ColumnName & ")"

                                str &= STRFld & "values(" & STRVal
                                ODA.SelectCommand.CommandText = str
                                ODA.SelectCommand.ExecuteNonQuery()
                            Else
                                str = "update MMM_MST_DOC_ITEM Set "
                                Dim querry As String = ""
                                For j As Integer = 0 To dtField.Rows.Count - 1
                                    STRFld &= dtField.Rows(j).Item("fieldmapping").ToString & ","
                                    Dim colValue As String = ""
                                    Dim FldID As String = ""
                                    FldID = dtField.Rows(j).Item("fieldid").ToString()
                                    If dtField.Rows(j).Item("inlineediting").ToString = "1" Or 1 = 1 Then
                                        '' new for getting child item total 
                                        Dim ftype As String = dtField.Rows(j).Item("fieldtype").ToString()
                                        Dim fldmapping As String = dtField.Rows(j).Item("fieldmapping").ToString()
                                        If ftype.ToUpper() = "TEXT BOX" Or ftype.ToUpper() = "CALCULATIVE FIELD" Then
                                            ' Dim cb As TextBox = CType(GR.FindControl("fld" & j.ToString() & "_" & cnt), TextBox)
                                            'Dim cb As TextBox = CType(GR.FindControl("fld" & FldID & cnt), TextBox)
                                            Dim cb As TextBox = CType(GR.FindControl("fld" & FldID & GR.RowIndex), TextBox)
                                            If cb IsNot Nothing Then
                                                colValue = Replace(cb.Text.ToString, "'", "")
                                                querry = fldmapping & "='" & colValue & "' " & ","
                                            Else
                                                colValue = "0"
                                            End If
                                            str &= querry

                                            If dtField.Rows(j).Item("displayname") = cDispName Then  '' new for getting child total
                                                If colValue <> "" Then
                                                    Childtotal = Childtotal + Convert.ToDouble(colValue)
                                                End If
                                            End If
                                        ElseIf ftype.ToUpper() = "DROP DOWN" And dtField.Rows(j).Item("dropdowntype").ToString() = "FIX VALUED" Then
                                            'Dim cb As New DropDownList
                                            Dim cb As DropDownList = CType(GR.FindControl("fld" & FldID & "_" & GR.RowIndex), DropDownList)
                                            If cb IsNot Nothing Then
                                                colValue = cb.SelectedItem.Text.ToString
                                                querry = fldmapping & "='" & colValue & "' " & ","
                                            Else
                                                colValue = "0"
                                            End If

                                            str &= querry
                                        ElseIf ftype.ToUpper() = "DROP DOWN" And dtField.Rows(j).Item("dropdownTYPE") = "MASTER VALUED" Then
                                            Dim cb As DropDownList = CType(GR.FindControl("fld" & FldID & "_" & GR.RowIndex), DropDownList)
                                            If cb IsNot Nothing Then
                                                colValue = cb.SelectedItem.Value.ToString  ' use value bcoz it's mastervalued - needs tid
                                                querry = fldmapping & "='" & colValue & "' " & ","
                                            Else
                                                colValue = "0"
                                            End If

                                            str &= querry
                                        ElseIf ftype.ToUpper() = "MULTI LOOKUPDDL" Then
                                            Dim cb As DropDownList = CType(GR.FindControl("fld" & FldID & "_" & GR.RowIndex), DropDownList)
                                            If cb IsNot Nothing Then
                                                colValue = cb.SelectedItem.Value.ToString  ' use value bcoz it's mastervalued - needs tid
                                                querry = fldmapping & "='" & colValue & "' " & ","
                                            Else
                                                colValue = "0"
                                            End If
                                            str &= querry
                                        ElseIf ftype.ToUpper() = "CHECKBOX LIST" Then
                                            Dim cb As CheckBoxList = CType(GR.FindControl("fld" & FldID & "_" & cnt), CheckBoxList)
                                            If cb IsNot Nothing Then
                                                For Each item As ListItem In cb.Items
                                                    If item.Selected Then
                                                        colValue &= item.Value & ","
                                                    End If
                                                Next
                                                If colValue.Length > 0 Then
                                                    colValue = colValue.Substring(0, colValue.Length - 1)
                                                    querry = fldmapping & "='" & colValue & "' " & ","
                                                Else
                                                    colValue = ""
                                                End If
                                            Else
                                                colValue = ""
                                            End If
                                            str &= querry

                                        ElseIf ftype.ToUpper() = "FILE UPLOADER" Then
                                            '' here to code for getting file 
                                            Dim txtBox As FileUpload = CType(GR.FindControl("fld_" & FldID & "_" & GR.RowIndex), FileUpload)

                                            Dim txtBox1 As Label = CType(GR.FindControl("lblf_" & FldID), Label)
                                            Dim hdn As HiddenField = CType(GR.FindControl("hdn_" & FldID), HiddenField)
                                            Dim hdnflg As HiddenField = CType(GR.FindControl("hdnf_" & FldID), HiddenField)

                                            txtBox1.Text = hdn.Value

                                            If txtBox1.Text IsNot "" And hdnflg.Value IsNot "0" Then
                                                Dim errormsg As String = ""
                                                Dim FN As String = ""
                                                Dim ext As String = ""
                                                Dim flag As Integer = 0
                                                Dim sourceFile As String = Server.MapPath("~/DOCS/temp/" + txtBox1.Text)
                                                Dim finfo As New FileInfo(Server.MapPath("~/DOCS/temp/" + txtBox1.Text))
                                                Dim curFileSize As Integer = finfo.Length
                                                ext = txtBox1.Text.Substring(txtBox1.Text.LastIndexOf("."), txtBox1.Text.Length - txtBox1.Text.LastIndexOf("."))
                                                FN = Left(txtBox1.Text, txtBox1.Text.LastIndexOf("."))
                                                colValue = Session("EID").ToString() & "/" & getSafeString(FN) & "_" & Now.Day & Now.Month & Now.Year & Now.Minute & Now.Second & Now.Millisecond & dtField.Rows(j).Item("FieldID").ToString() & "" & ext
                                                File.Copy(sourceFile, Server.MapPath("DOCS/") & Session("EID").ToString() & "/" & getSafeString(FN) & "_" & Now.Day & Now.Month & Now.Year & Now.Minute & Now.Second & Now.Millisecond & dtField.Rows(j).Item("FieldID").ToString() & ext)
                                                querry = fldmapping & "='" & colValue & "' " & ","
                                            ElseIf txtBox1.Text IsNot "" And hdnflg.Value IsNot "1" Then
                                                colValue = Session("EID").ToString() & "/" & txtBox1.Text
                                                querry = fldmapping & "='" & colValue & "' " & ","
                                            Else
                                                colValue = ""
                                                querry = fldmapping & "='" & colValue & "' " & ","
                                            End If

                                            str &= querry

                                        Else
                                            colValue = GR.Cells(j).Text.ToString()
                                            querry = fldmapping & "='" & colValue & "' " & ","
                                            If dtField.Rows(j).Item("displayname") = cDispName Then  '' new for getting child total
                                                If colValue <> "" Then
                                                    Childtotal = Childtotal + Convert.ToDouble(colValue)
                                                End If
                                            End If

                                        End If
                                    Else  ' if no inline editing 
                                        If dtField.Rows(j).Item("dropdownTYPE") = "MASTER VALUED" And dtField.Rows(j).Item("fieldtype") = "Drop Down" Then
                                            '' here code to get tid from session arrays by sp 13-jan-13
                                            Dim CH() As String = Session("COLHEAD")  ' {}
                                            Dim txt() As String = Session("DDLTXT") ' {}
                                            Dim Vals() As String = Session("DDLVAL") '{}
                                            Dim found As Boolean = False
                                            Dim searchHdr As String = formname & "_" & dtField.Rows(j).Item("displayname")
                                            For a As Integer = 0 To CH.Length - 1
                                                If CH(a).ToString = searchHdr And txt(a).ToString = GR.Cells(j).Text.ToString() Then ' if match found in array 
                                                    colValue = Vals(a).ToString
                                                    found = True
                                                    Exit For
                                                End If
                                            Next
                                            If found = False Then
                                                colValue = dtvalue.Rows(cnt).Item(j).ToString()
                                            End If
                                            'ElseIf dtField.Rows(j).Item("fieldtype") = "Child Item Total" Then

                                        Else
                                            colValue = GR.Cells(j).Text.ToString()
                                            If dtField.Rows(j).Item("displayname") = cDispName Then  '' new for getting child total
                                                If colValue <> "" Then
                                                    Childtotal = Childtotal + Convert.ToDouble(colValue)
                                                End If
                                            End If
                                        End If
                                    End If
                                    'STRVal &= "'" & colValue & "'" & ","
                                    colValue = colValue
                                Next
                                If Right(str, 1) = "," Then
                                    str = Left(str, str.Length - 1)
                                End If
                                Dim str1 As String = ""
                                str = str & "  where TID=" & dataitem.Rows(GR.RowIndex).Item("tid").ToString()
                                ODA.SelectCommand.CommandText = str
                                ODA.SelectCommand.ExecuteNonQuery()
                            End If
                            '  End If
                        End If
                    End If
                End If
            Next

            If isTotal Then
                Dim childTotalUpdstr As String = ""
                childTotalUpdstr = "UPDATE mmm_mst_doc set " & dtTotal.Rows(0).Item("mFieldMapping").ToString & "=" & Childtotal & " where tid=" & docid
                ODA.SelectCommand.CommandType = CommandType.Text
                ODA.SelectCommand.CommandText = childTotalUpdstr
                ODA.SelectCommand.ExecuteNonQuery()
            End If

            dt.Dispose()
            dtTotal.Dispose()
            dtField.Dispose()
            dtvalue.Dispose()

            Session(formname) = Nothing
            Session(formname & "VAL") = Nothing
            'ViewState("MDOCTYPE") = Nothing
            ODA.Dispose()
        Catch ex As Exception
            Throw New Exception(ex.ToString() & "Exception Occured in DocDetail.SavingActionChildItem")
        End Try
    End Sub
    Public Function getSafeString(ByVal strVar As String) As String
        Trim(strVar)
        strVar = Replace(strVar, "'", "")
        strVar = Replace(strVar, ";", "")
        strVar = Replace(strVar, "--", "")
        strVar = Replace(strVar, "%", "")
        strVar = Replace(strVar, "&", "")
        Return strVar
    End Function


    Protected Sub ShowReconsider(DocID As String)  'Handles btnDocReject.Click

        ' No Value in Session just fill the Edit Form and Show two button
        'two methods.. either show data from Grid or Show data from Database.
        If IsNothing(Session("DocID")) Then

            Exit Sub
        End If
        ViewState("pid") = DocID
        lblTabRej.Text = ""
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("uspSelectEventScreen", con)
        oda.SelectCommand.CommandType = CommandType.StoredProcedure
        oda.SelectCommand.Parameters.AddWithValue("docid", DocID)
        oda.SelectCommand.Parameters.AddWithValue("subevent", "REJECTION")
        Dim dt As New DataTable
        oda.Fill(dt)
        Session("FIELDS") = dt
        Session("ACTION") = "REJECTION"
        'Code By Ajeet For Trigger
        If dt.Rows.Count > 0 Then
            Session("ActionForm") = dt.Rows(0).Item("documenttype").ToString()
        Else
            Dim oda1 As SqlDataAdapter = New SqlDataAdapter("uspSelectEventScreen1", con)
            oda1.SelectCommand.CommandType = CommandType.StoredProcedure
            oda1.SelectCommand.Parameters.Clear()
            oda1.SelectCommand.Parameters.AddWithValue("@docid", DocID)
            oda1.SelectCommand.Parameters.AddWithValue("@subevent", "REJECTION")
            Dim ds1 As New DataSet
            oda1.Fill(ds1, "Action")
            If ds1.Tables("Action").Rows.Count > 0 Then
                Session("ActionForm") = Convert.ToString(ds1.Tables("Action").Rows(0).Item("documenttype"))
            End If
        End If
        'END'

        pnlFieldsRej.Width = 500
        pnlFieldsRej.Height = 10
        If dt.Rows.Count > 0 Then
            '            lblHeaderPopUp.Text = dt.Rows(0).Item("Formcaption").ToString()

            ''Clear and Hide the Existing Panel and ModalPoup
            pnlApprove.Controls.Clear()
            ModalApprove.Hide()
            Dim ob As New DynamicForm()
            pnlFieldsRej.Controls.Clear()

            ob.CreateControlsOnPanel(dt, pnlFieldsRej, updatePanelReject, btnReject, 1)
            Dim xx As Integer = Val(pnlFieldsRej.Width.ToString())
            Dim ww As Integer = Val(pnlFieldsRej.Height.ToString())
            pnlPopupReject.Width = xx + 20
            pnlPopupReject.Height = ww + 70 'panel height + button height + header height


            'Dim conF As New SqlConnection(ConfigurationManager.ConnectionStrings("conStr").ConnectionString)
            'Dim odaField As SqlDataAdapter = New SqlDataAdapter("SELECT FF.*,F.*  FROM MMM_MST_FIELDS FF left outer JOIN MMM_MST_FORMS F on F.FormName = FF.DocumentType and F.EID = FF.EID   where FF.isactive=1 and F.EID=" & Session("EID").ToString() & " and FormName ='" & Convert.ToString(Session("ActionForm")) & "' and eventname in(select documenttype from MMM_MST_DOC where eid=" & Session("EID") & " and tid=" & DocID & ")   order by displayOrder", conF)
            'Dim dtFileds As New DataTable()
            'odaField.Fill(dtFileds)
            'Session("Panel") = pnlFieldsRej
            'AddHandlerOnControl(dt, pnlFieldsRej, lblRuleMsg3)

            Dim ROW1() As DataRow = dt.Select("fieldtype='DROP DOWN' and dropdowntype='MASTER VALUED' and lookupvalue is not null")
            If ROW1.Length > 0 Then
                For i As Integer = 0 To ROW1.Length - 1
                    Dim DDL As DropDownList = TryCast(pnlFieldsRej.FindControl("fld" & ROW1(i).Item("FieldID").ToString()), DropDownList)
                    DDL.AutoPostBack = True
                    AddHandler DDL.TextChanged, AddressOf bindvalue1
                Next
            End If
            ob.FillControlsOnPanel(dt, pnlApprove, "DOCUMENT", DocID)
        End If
        con.Close()
        oda.Dispose()
        con.Dispose()

        Me.updatePanelReject.Update()
        Me.btnReject_ModalPopupExtender.Show()
    End Sub
    Public Sub bindvalue1(ByVal sender As Object, ByVal e As EventArgs)
        Dim c As Control = GetPostBackControl(Me.Page)
        ResetGrid()
        '...
        If c IsNot Nothing Then
        End If
        If TypeOf c Is System.Web.UI.WebControls.DropDownList Then
            Dim ddl As DropDownList = TryCast(c, DropDownList)
            Dim id As String = Right(ddl.ID, ddl.ID.Length - 3)
            Dim id1 As Integer = CInt(id)
            Dim ob As New DynamicForm()
            ob.bind(id, pnlFieldsRej, ddl)
        End If
    End Sub

    Private Sub ResetGrid()

        Dim grd As GridView = DirectCast(Session("Grid"), GridView)

        If IsNothing(grd) = False Then

            For Each row As GridViewRow In grd.Rows

                If row.RowIndex < grd.Rows.Count - 1 Then

                    For i As Integer = 0 To row.Cells.Count - 1
                        For Each ctl As Control In row.Cells(i).Controls
                            If TypeOf ctl Is FileUpload Then
                                Dim fld As New FileUpload
                                fld = DirectCast(ctl, FileUpload)
                                Dim ar = fld.ID.Split("_")
                                Dim hdn As HiddenField = DirectCast(row.FindControl("hdn_" & ar(1)), HiddenField)
                                Dim lbl As Label = DirectCast(row.FindControl("lblf_" & ar(1)), Label)
                                If IsNothing(hdn) = False Then
                                    Dim str As String = hdn.Value
                                    Dim id As String = hdn.ID
                                    lbl.Text = str
                                End If
                            End If
                        Next
                    Next

                End If
            Next

        End If

        grd = Nothing
        Session("Grid") = Nothing

    End Sub

    Protected Sub editBtnReject(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim dt As DataTable
        dt = CType(Session("FIELDS"), DataTable)
        Dim ob As New DynamicForm
        Dim pid = Session("DocID").ToString()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim trans As SqlTransaction = Nothing
        con.Open()
        trans = con.BeginTransaction()
        Try
            Dim retMsg As String = ob.ValidateAndGenrateQueryForControls("UPDATE", "UPDATE MMM_MST_DOC SET ", "", dt, pnlFieldsRej, Session("DocID"))
            If Trim(Left(retMsg, 6)).ToUpper() = "PLEASE" Then
                lblTabRej.Text = retMsg
            Else
                'Code For Rule Engine By Ajeet Kumar Dated :30-july-2014
                Dim RDocType As String = "NO Rule configured"
                If (dt.Rows.Count > 0) Then
                    RDocType = dt.Rows(0).Item("documenttype").ToString()
                End If

                Dim dv As DataView = dt.DefaultView
                dv.RowFilter = "IsActive=1"
                Dim theFields As DataTable = dv.ToTable
                Dim lstData As New List(Of UserData)
                Dim obj As New DynamicForm()
                'Creating collection for rule engine execution
                lstData = obj.CreateCollection(pnlPopupReject, theFields)
                'Setting it to session for getting it's value for child Item validation
                'Creating object of rule response
                Dim ObjRet As New RuleResponse()
                'Initialising rule Object
                Dim ObjRule As New RuleEngin(Session("EID"), RDocType, "CREATED", "SUBMIT")
                'Uncomment
                ObjRet = ObjRule.ExecuteRule(lstData, Nothing, False)
                If ObjRet.Success = False Then
                    lblTabRej.Text = ObjRet.Message
                    Exit Sub
                Else
                    retMsg = retMsg & " WHERE tid=" & Session("DocID").ToString()
                    ' Dim oda As SqlDataAdapter = New SqlDataAdapter("RejectWorkFlowNew", con)    ' this was prev b4 new rolematrix imple. by sunil
                    Dim oda As SqlDataAdapter = New SqlDataAdapter("ReconsiderWorkFlow_RM", con)
                    oda.SelectCommand.Transaction = trans
                    oda.SelectCommand.CommandType = CommandType.StoredProcedure
                    oda.SelectCommand.Parameters.AddWithValue("TID", Session("DocID").ToString())
                    oda.SelectCommand.Parameters.AddWithValue("remarks", "RECONSIDERED")
                    oda.SelectCommand.Parameters.AddWithValue("qry", retMsg)
                    oda.SelectCommand.Parameters.AddWithValue("auid", Val(Session("UID").ToString()))
                    Dim iSt As Integer = oda.SelectCommand.ExecuteScalar()
                    oda.Dispose()
                    Session("ACTION") = Nothing
                    Session("FIELDS") = Nothing
                    Try
                        Trigger.ExecuteTriggerT(Session("ActionForm"), Session("EID"), pid, con, trans, 0, "Create")
                    Catch ex As Exception
                        Throw
                    End Try
                    trans.Commit()
                    Dim ob1 As New DMSUtil()
                    ob1.TemplateCalling(Val(Session("DocID").ToString()), Session("EID").ToString(), "", "RECONSIDER")
                    If Not Session("ActionForm") Is Nothing Then
                        Session("ActionForm") = Nothing
                    End If
                    updatePanelReject.Update()
                    btnReject_ModalPopupExtender.Hide()
                    Session("DocID") = Nothing
                    Session("Approve") = Nothing
                    ScriptManager.RegisterStartupScript(Me, Me.GetType(), "popup", "alert('Done Successfully!!');", True)


                End If
            End If
        Catch ex As Exception
            trans.Rollback()
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "popup", "alert('Error occured at server!!');", True)
        Finally
            If Not con Is Nothing Then
                con.Close()
                con.Dispose()
                trans.Dispose()
            End If
        End Try
        Session("Reject") = Nothing
        Session("Pending") = Nothing
        ModalApprove.Hide()
        btncalendar_modalPopupExtender.Hide()
        btnPerRejectModalpopup.Hide()
        ScriptManager.RegisterStartupScript(Me, Me.GetType(), "refresh", " <script type='text/javascript'>document.getElementById('" + btnpendinggrdcl.ClientID + "').click() ;</script>", False)
    End Sub

    Protected Sub ShowPermanentReject(DocID As String)

        If IsNothing(Session("DocID")) Then
            Exit Sub
        End If
        ViewState("pid") = DocID
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("uspSelectEventScreen", con)
        oda.SelectCommand.CommandType = CommandType.StoredProcedure
        oda.SelectCommand.Parameters.AddWithValue("docid", DocID)
        oda.SelectCommand.Parameters.AddWithValue("subevent", "P_REJECTION")
        Dim dt As New DataTable
        oda.Fill(dt)
        Session("FIELDS") = dt
        Session("ACTION") = "P_REJECTION"

        PanelPerReject.Width = 500
        PanelPerReject.Height = 10
        If dt.Rows.Count > 0 Then
            Session("ActionForm") = dt.Rows(0).Item("documenttype").ToString()
            'lblHeaderPopUp.Text = dt.Rows(0).Item("Formcaption").ToString()
            'Clear and Hide the Existing Panel and ModalPoup
            pnlApprove.Controls.Clear()
            ModalApprove.Hide()
            Dim ob As New DynamicForm()
            PanelPerReject.Controls.Clear()
            ob.CreateControlsOnPanel(dt, PanelPerReject, updPerReject, btnPerReject, 1)
            Dim xx As Integer = Val(PanelPerReject.Width.ToString())
            Dim ww As Integer = Val(PanelPerReject.Height.ToString())
            pnlPerReject.Width = xx + 20
            pnlPerReject.Height = ww + 70 'panel height + button height + header height


            Dim ROW1() As DataRow = dt.Select("fieldtype='DROP DOWN' and dropdowntype='MASTER VALUED' and lookupvalue is not null")
            If ROW1.Length > 0 Then
                For i As Integer = 0 To ROW1.Length - 1
                    Dim DDL As DropDownList = TryCast(PanelPerReject.FindControl("fld" & ROW1(i).Item("FieldID").ToString()), DropDownList)
                    DDL.AutoPostBack = True
                    AddHandler DDL.TextChanged, AddressOf bindvalue1
                Next
            End If
            ob.FillControlsOnPanel(dt, PanelPerReject, "DOCUMENT", DocID)

            'Dim conF As New SqlConnection(ConfigurationManager.ConnectionStrings("conStr").ConnectionString)
            'Dim odaField As SqlDataAdapter = New SqlDataAdapter("SELECT FF.*,F.*  FROM MMM_MST_FIELDS FF left outer JOIN MMM_MST_FORMS F on F.FormName = FF.DocumentType and F.EID = FF.EID   where FF.isactive=1 and F.EID=" & Session("EID").ToString() & " and FormName ='" & Convert.ToString(Session("ActionForm")) & "' and eventname in(select documenttype from MMM_MST_DOC where eid=" & Session("EID") & " and tid=" & DocID & ")   order by displayOrder", conF)
            'Dim dtFileds As New DataTable()
            'odaField.Fill(dtFileds)
            'Session("Panel") = PanelPerReject
            'AddHandlerOnControl(dt, PanelPerReject, lblMsgRule1)

        Else
            Try
                con = New SqlConnection(conStr)
                oda = New SqlDataAdapter("getActionForm", con)
                oda.SelectCommand.CommandType = CommandType.StoredProcedure
                oda.SelectCommand.Parameters.AddWithValue("@docid", DocID)
                oda.SelectCommand.Parameters.AddWithValue("@subevent", "P_REJECTION")
                Dim dtA As New DataTable
                If con.State <> ConnectionState.Open Then
                    con.Open()
                End If
                oda.Fill(dtA)
                Session("ActionForm") = dtA.Rows(0).Item("formname").ToString()
                con.Close()
                oda.Dispose()
                con.Dispose()
            Catch ex As Exception
                con.Close()
                oda.Dispose()
                con.Dispose()
            End Try

        End If

        con.Close()
        oda.Dispose()
        con.Dispose()
        Me.updPerReject.Update()
        Me.btnPerRejectModalpopup.Show()
    End Sub

    Protected Sub editBtnPerReject(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim dt As DataTable
        dt = CType(Session("FIELDS"), DataTable)
        Dim ob As New DynamicForm
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim trans As SqlTransaction = Nothing
        con.Open()
        trans = con.BeginTransaction()
        Try
            Dim retMsg As String = ob.ValidateAndGenrateQueryForControls("UPDATE", "UPDATE MMM_MST_DOC SET ", "", dt, PanelPerReject, Session("DocID"))
            If Trim(Left(retMsg, 6)).ToUpper() = "PLEASE" Then
                lblPerRej.Text = retMsg
            Else
                Dim RDocType As String = "NO Rule configured"
                If (dt.Rows.Count > 0) Then
                    RDocType = dt.Rows(0).Item("documenttype").ToString()
                End If
                'Code For Executing Rule Engine
                Dim dv As DataView = dt.DefaultView
                dv.RowFilter = "IsActive=1"
                Dim theFields As DataTable = dv.ToTable
                Dim lstData As New List(Of UserData)
                Dim obj As New DynamicForm()
                'Creating collection for rule engine execution
                lstData = obj.CreateCollection(pnlPerReject, theFields)
                'Setting it to session for getting it's value for child Item validation
                'Creating object of rule response
                Dim ObjRet As New RuleResponse()
                'Initialising rule Object
                Dim ObjRule As New RuleEngin(Session("EID"), RDocType, "CREATED", "SUBMIT")
                'Uncomment
                ObjRet = ObjRule.ExecuteRule(lstData, Nothing, False)
                If ObjRet.Success = False Then
                    lblPerRej.Text = ObjRet.Message
                    Exit Sub
                Else
                    retMsg = retMsg & " WHERE tid=" & Session("DocID").ToString()
                    ' Dim oda As SqlDataAdapter = New SqlDataAdapter("PermanentrejectDoc", con)  ' this was prev b4 new rolematrix imple. by sunil
                    Dim oda As SqlDataAdapter = New SqlDataAdapter("PermanentRejectDoc_RM", con)
                    oda.SelectCommand.Transaction = trans
                    oda.SelectCommand.CommandType = CommandType.StoredProcedure
                    oda.SelectCommand.Parameters.AddWithValue("TID", Session("DocID").ToString())
                    oda.SelectCommand.Parameters.AddWithValue("remarks", "REJECTED")
                    oda.SelectCommand.Parameters.AddWithValue("qry", retMsg)
                    oda.SelectCommand.Parameters.AddWithValue("auid", Val(Session("UID").ToString()))

                    Dim iSt As Integer = oda.SelectCommand.ExecuteScalar()
                    oda.Dispose()
                    Session("ACTION") = Nothing
                    Session("FIELDS") = Nothing
                    Try
                        ' fill the actionform in session 
                        Trigger.ExecuteTriggerT(Session("ActionForm"), Session("EID"), Session("DocID").ToString(), con, trans, TriggerNature:="Create")
                    Catch ex As Exception
                        Throw
                    End Try
                    trans.Commit()
                    Dim ob1 As New DMSUtil()
                    Try
                        ob1.TemplateCalling(Val(Session("DocID").ToString()), Session("EID").ToString(), "", "REJECT")
                    Catch ex As Exception
                    End Try
                    Session("Reject") = Nothing
                    ScriptManager.RegisterStartupScript(Me, Me.GetType(), "popup", "alert('Done Successfully!!');", True)

                    updPerReject.Update()
                    btnPerRejectModalpopup.Hide()
                    Session("Reject") = Nothing
                End If

            End If
        Catch ex As Exception
            trans.Rollback()
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "popup", "alert('Error occured at server!!');", True)
        Finally
            If Not con Is Nothing Then
                con.Dispose()
                trans.Dispose()
            End If
        End Try
        Session("Reject") = Nothing
        Session("Pending") = Nothing
        ModalApprove.Hide()
        btncalendar_modalPopupExtender.Hide()
        btnPerRejectModalpopup.Hide()
        ScriptManager.RegisterStartupScript(Me, Me.GetType(), "refresh", "<script type='text/javascript'>document.getElementById('" + btnpendinggrdcl.ClientID + "').click() ;</script>", False)

    End Sub

    Protected Sub gvPending_RowCommand(sender As Object, e As GridViewCommandEventArgs) Handles gvPending.RowCommand
        If e.CommandName = "Approve" Then
            Session("DocID") = e.CommandArgument
            Session("Approve") = e.CommandArgument
            ShowApprove(e.CommandArgument.ToString)
            btnPerRejectModalpopup.Hide()
            btnReject_ModalPopupExtender.Hide()
            ModalApprove.Show()
        ElseIf e.CommandName = "Reject" Then
            Session("DocID") = e.CommandArgument
            Session("Reject") = e.CommandArgument
            ShowPermanentReject(Convert.ToInt32(e.CommandArgument))
            btnReject_ModalPopupExtender.Hide()
            ModalApprove.Hide()
            btnPerRejectModalpopup.Show()
        ElseIf e.CommandName = "Reconsider" Then
            Session("DocID") = e.CommandArgument
            Session("Reconsider") = e.CommandArgument
            ShowReconsider(Convert.ToInt32(e.CommandArgument))
            btnPerRejectModalpopup.Hide()
            ModalApprove.Hide()
            btnReject_ModalPopupExtender.Show()
        End If
    End Sub

    Protected Sub gvPending_Sorting(sender As Object, e As GridViewSortEventArgs)
        Dim qry As String = Convert.ToString(ViewState("pendingqry")).ToUpper
        Dim passconval As String = Nothing
        Dim passconvalSorting As String = Nothing
        Dim searchalias As String() = qry.Split(",")
        For i As Integer = 0 To searchalias.Length - 1
            If searchalias(i).Contains("[" & ddlPendinggrdHdr.SelectedItem.Text.ToUpper() & "]") Then
                Dim exactpart As String = Convert.ToString(searchalias(i)).Replace("[" & ddlPendinggrdHdr.SelectedItem.Text.ToUpper() & "]", "")
                exactpart = exactpart.ToUpper().Replace("AS", "")
                qry = qry & "  and " & Convert.ToString(exactpart) & " like '%" & txtPendinggrdval.Text & "%'"
                passconval = "  and " & Convert.ToString(exactpart) & " like '%" & txtPendinggrdval.Text & "%'"
                Exit For
            End If
        Next
        Dim sortingalias As String() = qry.Split(",")
        If Not IsNothing(ViewState("PendingGridDir")) Then
            If ViewState("PendingGridDir") = "DESC" Then
                e.SortDirection = SortDirection.Descending
            End If
        End If
        Dim dir As String = SetSortDirection(e.SortDirection)
        For i As Integer = 0 To sortingalias.Length - 1
            If sortingalias(i).Contains("[" & e.SortExpression.ToUpper() & "]") Then
                Dim exactpart As String = Convert.ToString(sortingalias(i)).Replace("[" & e.SortExpression.ToUpper() & "]", "")
                exactpart = exactpart.ToUpper().Replace("AS", "")

                passconvalSorting = "  order by " & Convert.ToString(exactpart) & " " & dir.ToString()
                Exit For
            End If
        Next
        PendingGrdBind(passconval, passconvalSorting)
    End Sub
    Protected Function SetSortDirection(ByVal sortDirection As SortDirection) As String
        Dim sortdir As String = "ASC"
        If Convert.ToString(sortDirection) = "0" Then
            sortDirection = sortDirection.Descending
            sortdir = "DESC"
            ViewState("PendingGridDir") = "DESC"
        Else
            sortDirection = sortDirection.Ascending
            ViewState("PendingGridDir") = "ASC"
        End If
        Return sortdir
    End Function
End Class
