Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Net
Imports System.Net.Mail
Imports System.Threading
Imports System.IO


Partial Class NewAccount
    Inherits System.Web.UI.Page
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As SqlConnection = New SqlConnection(conStr)
            Dim oda As SqlDataAdapter = New SqlDataAdapter("Select eid,code from MMM_MST_ENTITY order by Code", con)
            Dim ds As New DataSet
            oda.Fill(ds, "entity")
            ddlentityname.DataSource = ds.Tables("entity")
            ddlentityname.DataTextField = "code"
            ddlentityname.DataValueField = "eid"
            ddlentityname.DataBind()
            ddlentityname.Items.Insert(0, "Select")
            oda.Dispose()
            ds.Dispose()
            reset()
        End If
        ' reset()
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
    'Protected Sub Page_PreInit1(ByVal sender As Object, ByVal e As EventArgs) Handles Me.PreInit
    '    Try
    '        If Not Session("CTheme") Is Nothing And Not Session("CTheme") = String.Empty Then
    '            Page.Theme = Convert.ToString(Session("CTheme"))
    '        Else
    '            Page.Theme = "Default"
    '        End If
    '    Catch ex As Exception
    '    End Try

    'End Sub

    Protected Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click

        If ddlentityname.SelectedItem.Text.ToUpper = "SELECT" Then
            lblMsg.Text = "Please select Entity Name First!"
            Exit Sub
        End If
        If Trim(txtAcCode.Text) = "" Or txtAcCode.Text.Contains(" ") = True Then
            lblMsg.Text = "Please Type Valid Account Code (Without spaces)!"
            Exit Sub
        End If
        
        If txtAcName.Text = "" Then
            lblMsg.Text = "Please Type Valid Account Name!"
            Exit Sub
        End If

        If txtEmail.Text = "" Or txtEmail.Text.Contains("@") = False Then
            lblMsg.Text = "Please Type Valid Email ID!"
            Exit Sub
        End If
        If Trim(txtUserName.Text) = "" Or txtUserName.Text.Contains(" ") = True Then
            lblMsg.Text = "Please Type Valid Superuser Login ID (without spaces) !"
            Exit Sub
        End If

        ' If Trim(txtPWD.Text) = "" Or txtRePwd.Text = "" Then
        ' lblMsg.Text = "Please Type Password!"
        ' Exit Sub
        ' End If
        '  lblMsg.Text = "validation success"
        '  Exit Sub

        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim tran As SqlTransaction = Nothing
        Dim con As SqlConnection = New SqlConnection(conStr)
        con.Open()
        tran = con.BeginTransaction()
        Try
            'Dim oda As SqlDataAdapter = New SqlDataAdapter("select * from MMM_MST_ENTITY Where Code='" & ddlentityname.SelectedItem.Text.ToString() & "'", con)
            Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
            Dim ds As New DataSet

            oda.SelectCommand.CommandText = "select * from MMM_MST_ENTITY Where Code='" & ddlentityname.SelectedItem.Text.ToString() & "'"
            oda.SelectCommand.CommandType = CommandType.Text
            oda.SelectCommand.Transaction = tran
            oda.SelectCommand.Parameters.Clear()
            oda.Fill(ds, "fullentity")

            oda.SelectCommand.CommandText = "Accountcopy"
            oda.SelectCommand.CommandType = CommandType.StoredProcedure
            oda.SelectCommand.Transaction = tran
            oda.SelectCommand.Parameters.Clear()
            Dim obj As New User
            Dim sKey As Integer
            Dim pwd As String = txtPWD.Text
            Dim Generator As System.Random = New System.Random()
            sKey = Generator.Next(10000, 99999)
            Dim strPwd As String = obj.EncryptTripleDES(pwd, sKey)
            oda.SelectCommand.Parameters.AddWithValue("sKey", sKey)
            oda.SelectCommand.Parameters.AddWithValue("emailID", txtEmail.Text)
            oda.SelectCommand.Parameters.AddWithValue("pwdu", strPwd)
            oda.SelectCommand.Parameters.AddWithValue("Code", txtAcCode.Text)
            oda.SelectCommand.Parameters.AddWithValue("Name", txtAcName.Text)
            oda.SelectCommand.Parameters.AddWithValue("UserID", txtUserName.Text)
            'oda.SelectCommand.Parameters.AddWithValue("startmonth", ds.Tables("fullentity").Rows(0).Item("startmonth").ToString())
            'oda.SelectCommand.Parameters.AddWithValue("Endmonth", ds.Tables("fullentity").Rows(0).Item("Endmonth").ToString())
            oda.SelectCommand.Parameters.AddWithValue("pwd", pwd)
            oda.SelectCommand.Parameters.AddWithValue("isAuth", ds.Tables("fullentity").Rows(0).Item("isAuth").ToString())

            oda.SelectCommand.Parameters.AddWithValue("AccountType", ds.Tables("fullentity").Rows(0).Item("AccountType").ToString())
            oda.SelectCommand.Parameters.AddWithValue("CreatedDate", ds.Tables("fullentity").Rows(0).Item("CreatedDate").ToString())
            oda.SelectCommand.Parameters.AddWithValue("addsticker", ds.Tables("fullentity").Rows(0).Item("addsticker").ToString())
            oda.SelectCommand.Parameters.AddWithValue("domainname", ds.Tables("fullentity").Rows(0).Item("domainname").ToString())
            oda.SelectCommand.Parameters.AddWithValue("URL", ds.Tables("fullentity").Rows(0).Item("URL").ToString())
            oda.SelectCommand.Parameters.AddWithValue("Theme", ds.Tables("fullentity").Rows(0).Item("Theme").ToString())
            oda.SelectCommand.Parameters.AddWithValue("layout", ds.Tables("fullentity").Rows(0).Item("layout").ToString())
            oda.SelectCommand.Parameters.AddWithValue("pVisit", ds.Tables("fullentity").Rows(0).Item("pVisit").ToString())
            oda.SelectCommand.Parameters.AddWithValue("webHead", ds.Tables("fullentity").Rows(0).Item("webHead").ToString())
            oda.SelectCommand.Parameters.AddWithValue("logo", ds.Tables("fullentity").Rows(0).Item("logo").ToString())

            oda.SelectCommand.Parameters.AddWithValue("footer", ds.Tables("fullentity").Rows(0).Item("footer").ToString())
            oda.SelectCommand.Parameters.AddWithValue("ipaddress", ds.Tables("fullentity").Rows(0).Item("ipaddress").ToString())
            oda.SelectCommand.Parameters.AddWithValue("localfilesystem", ds.Tables("fullentity").Rows(0).Item("localfilesystem").ToString())
            oda.SelectCommand.Parameters.AddWithValue("minchar", ds.Tables("fullentity").Rows(0).Item("minchar").ToString())
            oda.SelectCommand.Parameters.AddWithValue("maxchar", ds.Tables("fullentity").Rows(0).Item("maxchar").ToString())
            oda.SelectCommand.Parameters.AddWithValue("passType", ds.Tables("fullentity").Rows(0).Item("passType").ToString())
            oda.SelectCommand.Parameters.AddWithValue("passExpDays", ds.Tables("fullentity").Rows(0).Item("passExpDays").ToString())
            oda.SelectCommand.Parameters.AddWithValue("passExpMsgDays", ds.Tables("fullentity").Rows(0).Item("passExpMsgDays").ToString())
            oda.SelectCommand.Parameters.AddWithValue("autoUnlockHour", ds.Tables("fullentity").Rows(0).Item("autoUnlockHour").ToString())
            oda.SelectCommand.Parameters.AddWithValue("minPassAttempt", ds.Tables("fullentity").Rows(0).Item("minPassAttempt").ToString())

            oda.SelectCommand.Parameters.AddWithValue("isWorkFlow", ds.Tables("fullentity").Rows(0).Item("isWorkFlow").ToString())
            oda.SelectCommand.Parameters.AddWithValue("WorkFlowType", ds.Tables("fullentity").Rows(0).Item("WorkFlowType").ToString())
            oda.SelectCommand.Parameters.AddWithValue("LoginField", ds.Tables("fullentity").Rows(0).Item("LoginField").ToString())
            oda.SelectCommand.Parameters.AddWithValue("headerImage", ds.Tables("fullentity").Rows(0).Item("headerImage").ToString())
            oda.SelectCommand.Parameters.AddWithValue("headerstrip", ds.Tables("fullentity").Rows(0).Item("headerstrip").ToString())
            oda.SelectCommand.Parameters.AddWithValue("UVDType", ds.Tables("fullentity").Rows(0).Item("UVDType").ToString())
            oda.SelectCommand.Parameters.AddWithValue("UVUserField", ds.Tables("fullentity").Rows(0).Item("UVUserField").ToString())
            oda.SelectCommand.Parameters.AddWithValue("UVVehicleField", ds.Tables("fullentity").Rows(0).Item("UVVehicleField").ToString())
            oda.SelectCommand.Parameters.AddWithValue("VIDType", ds.Tables("fullentity").Rows(0).Item("VIDType").ToString())
            oda.SelectCommand.Parameters.AddWithValue("VIVehicleField", ds.Tables("fullentity").Rows(0).Item("VIVehicleField").ToString())

            oda.SelectCommand.Parameters.AddWithValue("VIImeiField", ds.Tables("fullentity").Rows(0).Item("VIImeiField").ToString())
            oda.SelectCommand.Parameters.AddWithValue("isgpsactivated", ds.Tables("fullentity").Rows(0).Item("isgpsactivated").ToString())
            oda.SelectCommand.Parameters.AddWithValue("mapType", ds.Tables("fullentity").Rows(0).Item("mapType").ToString())
            oda.SelectCommand.Parameters.AddWithValue("APIKey", "")
            oda.SelectCommand.Parameters.AddWithValue("UVStartDateTime", ds.Tables("fullentity").Rows(0).Item("UVStartDateTime").ToString())
            oda.SelectCommand.Parameters.AddWithValue("UVEndDateTime", ds.Tables("fullentity").Rows(0).Item("UVEndDateTime").ToString())
            oda.SelectCommand.Parameters.AddWithValue("CurStatus", ds.Tables("fullentity").Rows(0).Item("CurStatus").ToString())
            oda.SelectCommand.Parameters.AddWithValue("VIDriverNamefield", ds.Tables("fullentity").Rows(0).Item("isAuth").ToString())
            oda.SelectCommand.Parameters.AddWithValue("VIDrivermnofield", ds.Tables("fullentity").Rows(0).Item("VIDrivermnofield").ToString())
            oda.SelectCommand.Parameters.AddWithValue("AppType", ds.Tables("fullentity").Rows(0).Item("AppType").ToString())

            oda.SelectCommand.Parameters.AddWithValue("wserrorEmail", ds.Tables("fullentity").Rows(0).Item("wserrorEmail").ToString())
            oda.SelectCommand.Parameters.AddWithValue("SMSAlertMNo", ds.Tables("fullentity").Rows(0).Item("SMSAlertMNo").ToString())
            oda.SelectCommand.Parameters.AddWithValue("MailAlertID", ds.Tables("fullentity").Rows(0).Item("MailAlertID").ToString())
            oda.SelectCommand.Parameters.AddWithValue("ReloadSeconds", Convert.ToString(ds.Tables("fullentity").Rows(0).Item("ReloadSeconds")))
            oda.SelectCommand.Parameters.AddWithValue("MailSmtp", Convert.ToString(ds.Tables("fullentity").Rows(0).Item("MailSmtp")))
            oda.SelectCommand.Parameters.AddWithValue("MailPassword", Convert.ToString(ds.Tables("fullentity").Rows(0).Item("MailPassword")))
            oda.SelectCommand.Parameters.AddWithValue("MailUserName", Convert.ToString(ds.Tables("fullentity").Rows(0).Item("MailUserName")))
            oda.SelectCommand.Parameters.AddWithValue("defaultpage", Convert.ToString(ds.Tables("fullentity").Rows(0).Item("defaultpage")))
            oda.SelectCommand.Parameters.AddWithValue("Startmonth", Convert.ToString(ds.Tables("fullentity").Rows(0).Item("Startmonth")))
            oda.SelectCommand.Parameters.AddWithValue("Endmonth", Convert.ToString(ds.Tables("fullentity").Rows(0).Item("Endmonth")))


            oda.SelectCommand.Parameters.AddWithValue("FtpAddress", Convert.ToString(ds.Tables("fullentity").Rows(0).Item("FtpAddress")))
            oda.SelectCommand.Parameters.AddWithValue("FtpID", Convert.ToString(ds.Tables("fullentity").Rows(0).Item("FtpID")))
            oda.SelectCommand.Parameters.AddWithValue("FtpPwd", Convert.ToString(ds.Tables("fullentity").Rows(0).Item("FtpPwd")))
            oda.SelectCommand.Parameters.AddWithValue("FtpFolder", Convert.ToString(ds.Tables("fullentity").Rows(0).Item("FtpFolder")))
            oda.SelectCommand.Parameters.AddWithValue("MapHomePage", Convert.ToString(ds.Tables("fullentity").Rows(0).Item("MapHomePage")))
            oda.SelectCommand.Parameters.AddWithValue("MapReportPage", Convert.ToString(ds.Tables("fullentity").Rows(0).Item("MapReportPage")))
            oda.SelectCommand.Parameters.AddWithValue("CTheme", Convert.ToString(ds.Tables("fullentity").Rows(0).Item("CTheme")))
            oda.SelectCommand.Parameters.AddWithValue("CheckDeviceReg", Convert.ToString(ds.Tables("fullentity").Rows(0).Item("CheckDeviceReg")))
            oda.SelectCommand.Parameters.AddWithValue("lastupdate", Convert.ToString(ds.Tables("fullentity").Rows(0).Item("lastupdate")))
            oda.SelectCommand.Parameters.AddWithValue("EnableMap", Convert.ToString(ds.Tables("fullentity").Rows(0).Item("EnableMap")))


            oda.SelectCommand.Parameters.AddWithValue("EnableTracking", Convert.ToString(ds.Tables("fullentity").Rows(0).Item("EnableTracking")))
            'oda.SelectCommand.Parameters.AddWithValue("angle", Convert.ToString(ds.Tables("fullentity").Rows(0).Item("angle")))
            'oda.SelectCommand.Parameters.AddWithValue("Time", Convert.ToString(ds.Tables("fullentity").Rows(0).Item("Time")))
            'oda.SelectCommand.Parameters.AddWithValue("Distance", Convert.ToString(ds.Tables("fullentity").Rows(0).Item("Distance")))
            'oda.SelectCommand.Parameters.AddWithValue("SendingFrequency", Convert.ToString(ds.Tables("fullentity").Rows(0).Item("SendingFrequency")))
            '' new added
            oda.SelectCommand.Parameters.AddWithValue("SiteDoc", Convert.ToString(ds.Tables("fullentity").Rows(0).Item("SiteDoc")))
            oda.SelectCommand.Parameters.AddWithValue("SiteNamefld", Convert.ToString(ds.Tables("fullentity").Rows(0).Item("SiteNamefld")))
            oda.SelectCommand.Parameters.AddWithValue("SiteFencefld", Convert.ToString(ds.Tables("fullentity").Rows(0).Item("SiteFencefld")))
            oda.SelectCommand.Parameters.AddWithValue("SiteMobilefld", Convert.ToString(ds.Tables("fullentity").Rows(0).Item("SiteMobilefld")))
            oda.SelectCommand.Parameters.AddWithValue("SiteCompanyfld", Convert.ToString(ds.Tables("fullentity").Rows(0).Item("SiteCompanyfld")))

            oda.SelectCommand.Parameters.AddWithValue("VehicleDoc", Convert.ToString(ds.Tables("fullentity").Rows(0).Item("VehicleDoc")))
            oda.SelectCommand.Parameters.AddWithValue("VehicleNofld", Convert.ToString(ds.Tables("fullentity").Rows(0).Item("VehicleNofld")))
            oda.SelectCommand.Parameters.AddWithValue("IMEIfld", Convert.ToString(ds.Tables("fullentity").Rows(0).Item("IMEIfld")))
            oda.SelectCommand.Parameters.AddWithValue("VehicleCompanyfld", Convert.ToString(ds.Tables("fullentity").Rows(0).Item("VehicleCompanyfld")))
            oda.SelectCommand.Parameters.AddWithValue("ExtendedSetting", Convert.ToString(ds.Tables("fullentity").Rows(0).Item("ExtendedSetting")))
            oda.SelectCommand.Parameters.AddWithValue("SiteEmailfld", Convert.ToString(ds.Tables("fullentity").Rows(0).Item("SiteEmailfld")))
            'New Added Field
            oda.SelectCommand.Parameters.AddWithValue("Supplier_formname_fieldMapping", Convert.ToString(ds.Tables("fullentity").Rows(0).Item("Supplier_formname_fieldMapping")))
            oda.SelectCommand.Parameters.AddWithValue("TypeOfDiscounting", Convert.ToString(ds.Tables("fullentity").Rows(0).Item("TypeOfDiscounting")))
            oda.SelectCommand.Parameters.AddWithValue("isDiscountingMapping", Convert.ToString(ds.Tables("fullentity").Rows(0).Item("isDiscountingMapping")))
            oda.SelectCommand.Parameters.AddWithValue("DiscountingMaster", Convert.ToString(ds.Tables("fullentity").Rows(0).Item("DiscountingMaster")))


            oda.SelectCommand.Parameters.AddWithValue("Contact_Person", Convert.ToString(ds.Tables("fullentity").Rows(0).Item("Contact_Person")))
            oda.SelectCommand.Parameters.AddWithValue("GSTN_No", Convert.ToString(ds.Tables("fullentity").Rows(0).Item("GSTN_No")))
            oda.SelectCommand.Parameters.AddWithValue("GSTN_status", Convert.ToString(ds.Tables("fullentity").Rows(0).Item("GSTN_status")))
            oda.SelectCommand.Parameters.AddWithValue("Contact_dtl", Convert.ToString(ds.Tables("fullentity").Rows(0).Item("Contact_dtl")))
            oda.SelectCommand.Parameters.AddWithValue("PAN", Convert.ToString(ds.Tables("fullentity").Rows(0).Item("PAN")))
            oda.SelectCommand.Parameters.AddWithValue("Address", Convert.ToString(ds.Tables("fullentity").Rows(0).Item("Address")))
            oda.SelectCommand.Parameters.AddWithValue("Supplier_formName", Convert.ToString(ds.Tables("fullentity").Rows(0).Item("Supplier_formName")))
            oda.SelectCommand.Parameters.AddWithValue("M1RegStatus", Convert.ToString(ds.Tables("fullentity").Rows(0).Item("M1RegStatus")))
            oda.SelectCommand.Parameters.AddWithValue("logo_Text", Convert.ToString(ds.Tables("fullentity").Rows(0).Item("logo_Text")))

            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            Dim res As String = oda.SelectCommand.ExecuteScalar()
            If res <> "" Then
                Session("AccountEID") = res
            End If

            Dim Eidsource As Integer = ds.Tables("fullentity").Rows(0).Item("eid")
            oda.SelectCommand.CommandText = "InsertForm"
            oda.SelectCommand.CommandType = CommandType.StoredProcedure
            oda.SelectCommand.Transaction = tran
            oda.SelectCommand.Parameters.Clear()
            oda.SelectCommand.Parameters.AddWithValue("Eidtarget", res)
            oda.SelectCommand.Parameters.AddWithValue("Eidsource", Eidsource)
            oda.SelectCommand.ExecuteNonQuery()

            Dim odaa As SqlDataAdapter = New SqlDataAdapter("", con)
            '' '''''''''''''''''''''''''''''''''''' (Insert Trigger and workflow and authmatrix,Menu,Prerole_datafilter,Role,TEMPLATE,REPORT,reportscheduler,print_template)'''''''''''''''''''''''''''''''''''''''
            odaa.SelectCommand.CommandText = "select Eid from MMM_MST_Triggers Where Eid='" & Eidsource & "'"
            odaa.SelectCommand.CommandType = CommandType.Text
            odaa.SelectCommand.Transaction = tran
            Dim dttri As New DataTable
            odaa.Fill(dttri)
            odaa.SelectCommand.CommandText = "InsertTriggers"
            odaa.SelectCommand.CommandType = CommandType.StoredProcedure
            odaa.SelectCommand.Transaction = tran
            odaa.SelectCommand.Parameters.Clear()
            odaa.SelectCommand.Parameters.AddWithValue("Eidtarget", res)
            odaa.SelectCommand.Parameters.AddWithValue("Eidsource", Eidsource)
            odaa.SelectCommand.ExecuteNonQuery()
            odaa.SelectCommand.CommandText = "Update MMM_MST_Triggers set TriggerText=replace(TriggerText,'=" & Eidsource & "','=" & res & "') Where eid='" & res & "' "
            odaa.SelectCommand.CommandType = CommandType.Text
            odaa.SelectCommand.Parameters.Clear()
            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            odaa.SelectCommand.ExecuteNonQuery()



            odaa.SelectCommand.CommandText = "select Eid from mmm_TallySync_filter Where Eid='" & Eidsource & "'"
            odaa.SelectCommand.CommandType = CommandType.Text
            odaa.SelectCommand.Transaction = tran
            Dim dtfilter As New DataTable
            odaa.Fill(dtfilter)
            odaa.SelectCommand.CommandText = "InsertTallySyncFilter"
            odaa.SelectCommand.CommandType = CommandType.StoredProcedure
            odaa.SelectCommand.Transaction = tran
            odaa.SelectCommand.Parameters.Clear()
            odaa.SelectCommand.Parameters.AddWithValue("Eidtarget", res)
            odaa.SelectCommand.Parameters.AddWithValue("Eidsource", Eidsource)
            odaa.SelectCommand.ExecuteNonQuery()
            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            odaa.SelectCommand.ExecuteNonQuery()


            'Dim Eidsource As String = "49"
            'Dim res As String = "59"
            odaa.SelectCommand.CommandText = "select Eid from MMM_MST_Workflow_Status Where Eid='" & Eidsource & "'"
            odaa.SelectCommand.CommandType = CommandType.Text
            odaa.SelectCommand.Transaction = tran
            Dim dtwrk As New DataTable
            odaa.Fill(dtwrk)
            odaa.SelectCommand.CommandText = "InsertWorkflowStatus"
            odaa.SelectCommand.CommandType = CommandType.StoredProcedure
            odaa.SelectCommand.Transaction = tran
            odaa.SelectCommand.Parameters.Clear()
            odaa.SelectCommand.Parameters.AddWithValue("Eidtarget", res)
            odaa.SelectCommand.Parameters.AddWithValue("Eidsource", Eidsource)
            odaa.SelectCommand.ExecuteNonQuery()

            ''''''''qry starts for ExportMapping

            odaa.SelectCommand.CommandText = "select Eid from mmm_mst_exportmapping Where Eid='" & Eidsource & "'"
            odaa.SelectCommand.CommandType = CommandType.Text
            odaa.SelectCommand.Transaction = tran
            Dim dtexpmapp As New DataTable
            odaa.Fill(dtexpmapp)
            odaa.SelectCommand.CommandText = "InsertExportMapping"
            odaa.SelectCommand.CommandType = CommandType.StoredProcedure
            odaa.SelectCommand.Transaction = tran
            odaa.SelectCommand.Parameters.Clear()
            odaa.SelectCommand.Parameters.AddWithValue("Eidtarget", res)
            odaa.SelectCommand.Parameters.AddWithValue("Eidsource", Eidsource)
            odaa.SelectCommand.ExecuteNonQuery()



            ''End of Code

            odaa.SelectCommand.CommandText = "select Eid from MMM_MST_RULES Where Eid='" & Eidsource & "'"
            odaa.SelectCommand.CommandType = CommandType.Text
            odaa.SelectCommand.Transaction = tran
            Dim dtRULES As New DataTable
            odaa.Fill(dtRULES)
            odaa.SelectCommand.CommandText = "InsertRULES"
            odaa.SelectCommand.CommandType = CommandType.StoredProcedure
            odaa.SelectCommand.Transaction = tran
            odaa.SelectCommand.Parameters.Clear()
            odaa.SelectCommand.Parameters.AddWithValue("Eidtarget", res)
            odaa.SelectCommand.Parameters.AddWithValue("Eidsource", Eidsource)
            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            odaa.SelectCommand.ExecuteNonQuery()





            odaa.SelectCommand.CommandText = "select Eid from MMM_MST_RuleRelation Where Eid='" & Eidsource & "'"
            odaa.SelectCommand.CommandType = CommandType.Text
            odaa.SelectCommand.Transaction = tran
            Dim dtruEng As New DataTable
            odaa.Fill(dtruEng)
            odaa.SelectCommand.CommandText = "InsertRuleRelation"
            odaa.SelectCommand.CommandType = CommandType.StoredProcedure
            odaa.SelectCommand.Transaction = tran
            odaa.SelectCommand.Parameters.Clear()
            odaa.SelectCommand.Parameters.AddWithValue("Eidtarget", res)
            odaa.SelectCommand.Parameters.AddWithValue("Eidsource", Eidsource)
            odaa.SelectCommand.ExecuteNonQuery()

            'Dim objRuleRelationDT As New DataTable()
            'objRuleRelationDT = objDC.TranExecuteQryDT("select * from MMM_MST_RuleRelation where eid=" & Eidsource, con, tran)
            'For Each drRelation As DataRow In objRuleRelationDT.Rows
            '    objDC.TranExecuteQryDT(" update mmm_mst_rulerelation set ruleid=(select ruleid from mmm_mst_rules where oldid=" & drRelation("Ruleid") & " and eid=" & res & ") where oldruleid=" & drRelation("ruleid") & " and eid= " & res & "", con, tran)
            'Next
            'Insert for Rules Relation   code already written by someone else because fields will be created leater don't do uncomment 06-02-2018 Mayank


            odaa.SelectCommand.CommandText = "select Eid from MMM_MST_Relation Where Eid='" & Eidsource & "'"
            odaa.SelectCommand.CommandType = CommandType.Text
            odaa.SelectCommand.Transaction = tran
            Dim dtres As New DataTable
            odaa.Fill(dtres)
            odaa.SelectCommand.CommandText = "InsertRelation"
            odaa.SelectCommand.CommandType = CommandType.StoredProcedure
            odaa.SelectCommand.Transaction = tran
            odaa.SelectCommand.Parameters.Clear()
            odaa.SelectCommand.Parameters.AddWithValue("Eidtarget", res)
            odaa.SelectCommand.Parameters.AddWithValue("Eidsource", Eidsource)
            odaa.SelectCommand.ExecuteNonQuery()

            odaa.SelectCommand.CommandText = "select Eid from MMM_MST_AuthMetrix Where Eid='" & Eidsource & "'"
            odaa.SelectCommand.CommandType = CommandType.Text
            odaa.SelectCommand.Transaction = tran
            Dim dtauth As New DataTable
            odaa.Fill(dtauth)
            odaa.SelectCommand.CommandText = "InsertAuthMetrix"
            odaa.SelectCommand.CommandType = CommandType.StoredProcedure
            odaa.SelectCommand.Transaction = tran
            odaa.SelectCommand.Parameters.Clear()
            odaa.SelectCommand.Parameters.AddWithValue("Eidtarget", res)
            odaa.SelectCommand.Parameters.AddWithValue("Eidsource", Eidsource)
            odaa.SelectCommand.ExecuteNonQuery()

            odaa.SelectCommand.CommandText = "select Eid from mmm_mst_menu Where Eid='" & Eidsource & "'"
            odaa.SelectCommand.CommandType = CommandType.Text
            odaa.SelectCommand.Transaction = tran
            Dim dtmenu As New DataTable
            odaa.Fill(dtmenu)
            odaa.SelectCommand.CommandText = "InsertMenu"
            odaa.SelectCommand.CommandType = CommandType.StoredProcedure
            odaa.SelectCommand.Transaction = tran
            odaa.SelectCommand.Parameters.Clear()
            odaa.SelectCommand.Parameters.AddWithValue("Eidtarget", res)
            odaa.SelectCommand.Parameters.AddWithValue("Eidsource", Eidsource)
            odaa.SelectCommand.ExecuteNonQuery()

            odaa.SelectCommand.CommandText = "select Eid from MMM_PreRole_dataFilter Where Eid='" & Eidsource & "'"
            odaa.SelectCommand.CommandType = CommandType.Text
            odaa.SelectCommand.Transaction = tran
            Dim dtpre As New DataTable
            odaa.Fill(dtpre)
            odaa.SelectCommand.CommandText = "InsertPreRoledataFilteraccount"
            odaa.SelectCommand.CommandType = CommandType.StoredProcedure
            odaa.SelectCommand.Transaction = tran
            odaa.SelectCommand.Parameters.Clear()
            odaa.SelectCommand.Parameters.AddWithValue("Eidtarget", res)
            odaa.SelectCommand.Parameters.AddWithValue("Eidsource", Eidsource)
            odaa.SelectCommand.ExecuteNonQuery()

            odaa.SelectCommand.CommandText = "select Eid from mmm_mst_role Where Eid='" & Eidsource & "'"
            odaa.SelectCommand.CommandType = CommandType.Text
            odaa.SelectCommand.Transaction = tran
            Dim dtrole As New DataTable
            odaa.Fill(dtrole)
            odaa.SelectCommand.CommandText = "InsertRole"
            odaa.SelectCommand.CommandType = CommandType.StoredProcedure
            odaa.SelectCommand.Transaction = tran
            odaa.SelectCommand.Parameters.Clear()
            odaa.SelectCommand.Parameters.AddWithValue("Eidtarget", res)
            odaa.SelectCommand.Parameters.AddWithValue("Eidsource", Eidsource)
            odaa.SelectCommand.ExecuteNonQuery()

            odaa.SelectCommand.CommandText = "select Eid from MMM_MST_TEMPLATE Where Eid='" & Eidsource & "'"
            odaa.SelectCommand.CommandType = CommandType.Text
            odaa.SelectCommand.Transaction = tran
            Dim dttemp As New DataTable
            odaa.Fill(dttemp)
            odaa.SelectCommand.CommandText = "InsertTEMPLATE"
            odaa.SelectCommand.CommandType = CommandType.StoredProcedure
            odaa.SelectCommand.Transaction = tran
            odaa.SelectCommand.Parameters.Clear()
            odaa.SelectCommand.Parameters.AddWithValue("Eidtarget", res)
            odaa.SelectCommand.Parameters.AddWithValue("Eidsource", Eidsource)
            odaa.SelectCommand.ExecuteNonQuery()

            odaa.SelectCommand.CommandText = "select Eid from MMM_MST_REPORT Where Eid='" & Eidsource & "'"
            odaa.SelectCommand.CommandType = CommandType.Text
            odaa.SelectCommand.Transaction = tran
            Dim dtreport As New DataTable
            odaa.Fill(dtreport)
            odaa.SelectCommand.CommandText = "InsertREPORT"
            odaa.SelectCommand.CommandType = CommandType.StoredProcedure
            odaa.SelectCommand.Transaction = tran
            odaa.SelectCommand.Parameters.Clear()
            odaa.SelectCommand.Parameters.AddWithValue("Eidtarget", res)
            odaa.SelectCommand.Parameters.AddWithValue("Eidsource", Eidsource)
            odaa.SelectCommand.ExecuteNonQuery()

            odaa.SelectCommand.CommandText = "select Eid from mmm_mst_reportscheduler Where Eid='" & Eidsource & "'"
            odaa.SelectCommand.CommandType = CommandType.Text
            odaa.SelectCommand.Transaction = tran
            Dim dtsch As New DataTable
            odaa.Fill(dtsch)
            odaa.SelectCommand.CommandText = "Insertreportscheduler"
            odaa.SelectCommand.CommandType = CommandType.StoredProcedure
            odaa.SelectCommand.Transaction = tran
            odaa.SelectCommand.Parameters.Clear()
            odaa.SelectCommand.Parameters.AddWithValue("Eidtarget", res)
            odaa.SelectCommand.Parameters.AddWithValue("Eidsource", Eidsource)
            odaa.SelectCommand.ExecuteNonQuery()

            odaa.SelectCommand.CommandText = "select Eid from mmm_print_template Where Eid='" & Eidsource & "'"
            odaa.SelectCommand.CommandType = CommandType.Text
            odaa.SelectCommand.Transaction = tran
            Dim dtprnttem As New DataTable
            odaa.Fill(dtprnttem)
            odaa.SelectCommand.CommandText = "Insertprint_template"
            odaa.SelectCommand.CommandType = CommandType.StoredProcedure
            odaa.SelectCommand.Transaction = tran
            odaa.SelectCommand.Parameters.Clear()
            odaa.SelectCommand.Parameters.AddWithValue("Eidtarget", res)
            odaa.SelectCommand.Parameters.AddWithValue("Eidsource", Eidsource)
            odaa.SelectCommand.ExecuteNonQuery()


            odaa.SelectCommand.CommandText = "select Eid from mmm_mst_gpssetting Where Eid='" & Eidsource & "'"
            odaa.SelectCommand.CommandType = CommandType.Text
            odaa.SelectCommand.Transaction = tran
            Dim dtgps As New DataTable
            odaa.Fill(dtgps)
            odaa.SelectCommand.CommandText = "Insertgpssetting"
            odaa.SelectCommand.CommandType = CommandType.StoredProcedure
            odaa.SelectCommand.Transaction = tran
            odaa.SelectCommand.Parameters.Clear()
            odaa.SelectCommand.Parameters.AddWithValue("Eidtarget", res)
            odaa.SelectCommand.Parameters.AddWithValue("Eidsource", Eidsource)
            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            odaa.SelectCommand.ExecuteNonQuery()

            '''''''''''''''''''''''''''''''GPSSETTING''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            odaa.SelectCommand.CommandText = "select * from mmm_mst_gpssetting Where Eid='" & res & "'"
            odaa.SelectCommand.CommandType = CommandType.Text
            odaa.SelectCommand.Transaction = tran
            Dim gp As New DataTable
            odaa.Fill(gp)
            Dim valg As String = ""
            Dim flagp As Boolean = False
            For x As Integer = 0 To gp.Rows.Count - 1
                Dim Cab_Vehicle As String = gp.Rows(x).Item("Cab_Vehicle_doc").ToString()
                Dim Cab_rate As String = gp.Rows(x).Item("Cab_rate_card_doc").ToString()
                Dim Cab_Customer As String = gp.Rows(x).Item("Cab_Customer_doc").ToString()
                odaa.SelectCommand.CommandText = "Update mmm_mst_gpssetting set Cab_Vehicle_doc=replace(Cab_Vehicle_doc,'V" & Eidsource & "','V" & res & "'),Cab_rate_card_doc=replace(Cab_rate_card_doc,'V" & Eidsource & "','V" & res & "'),Cab_Customer_doc=replace(Cab_Customer_doc,'V" & Eidsource & "','V" & res & "') Where eid='" & res & "' and tid='" & gp.Rows(x).Item("tid").ToString() & "'"
                odaa.SelectCommand.Transaction = tran
                If con.State <> ConnectionState.Open Then
                    con.Open()
                End If
                odaa.SelectCommand.ExecuteNonQuery()
            Next

            '''''''''''''''''''''''''''''''''''FOR MENU''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            odaa.SelectCommand.CommandText = "select * from mmm_mst_menu Where Eid='" & res & "'"
            odaa.SelectCommand.CommandType = CommandType.Text

            odaa.SelectCommand.Transaction = tran
            Dim mv As New DataTable
            odaa.Fill(mv)
            Dim val As String = ""
            Dim flagg As Boolean = False
            For j As Integer = 0 To mv.Rows.Count - 1
                odaa.SelectCommand.CommandText = "select mid from mmm_mst_menu Where Eid='" & res & "' and oldmid='" & mv.Rows(j).Item("oldmid").ToString() & "'"
                odaa.SelectCommand.CommandType = CommandType.Text
                odaa.SelectCommand.Transaction = tran
                Dim inr As New DataTable
                odaa.Fill(inr)
                If inr.Rows.Count > 0 Then
                    val = inr.Rows(0).Item("mid").ToString()
                End If
                odaa.SelectCommand.CommandText = "Update mmm_mst_menu set Pmenu='" & val & "'  where eid='" & res & "' and pmenu='" & mv.Rows(j).Item("oldmid").ToString() & "'"
                odaa.SelectCommand.Transaction = tran
                If con.State <> ConnectionState.Open Then
                    con.Open()
                End If
                odaa.SelectCommand.ExecuteNonQuery()
            Next
            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            '''''''''''''''''''''''''''''''''''FOR Rules''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            odaa.SelectCommand.CommandText = "select RuleID,oldid from MMM_MST_RULES Where Eid='" & res & "'"
            odaa.SelectCommand.CommandType = CommandType.Text
            odaa.SelectCommand.Transaction = tran
            Dim mvs As New DataTable
            odaa.Fill(mvs)
            For j As Integer = 0 To mvs.Rows.Count - 1
                odaa.SelectCommand.CommandText = "select tid,RuleID from MMM_MST_RuleRelation Where Eid='" & res & "' and RuleID=" & mvs.Rows(j).Item("oldid").ToString() & ""
                odaa.SelectCommand.CommandType = CommandType.Text
                odaa.SelectCommand.Transaction = tran
                Dim inr As New DataTable
                odaa.Fill(inr)
                If inr.Rows.Count > 0 Then
                    odaa.SelectCommand.CommandText = "Update MMM_MST_RuleRelation set RuleID='" & mvs.Rows(j).Item("RuleID").ToString() & "'  where eid='" & res & "' and tid='" & inr.Rows(0).Item("tid").ToString() & "'"
                    If con.State <> ConnectionState.Open Then
                        con.Open()
                    End If
                    odaa.SelectCommand.Transaction = tran
                    odaa.SelectCommand.ExecuteNonQuery()
                End If
            Next
            ' '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''


            odaa.SelectCommand.CommandText = "select Eid,FormName from MMM_Mst_Forms Where Eid='" & Eidsource & "'"
            ' odaa.SelectCommand.CommandText = "select Eid,FormName from MMM_Mst_Forms Where Eid='32'"
            odaa.SelectCommand.CommandType = CommandType.Text
            odaa.SelectCommand.Transaction = tran
            Dim dt As New DataTable
            odaa.Fill(dt)
            For w As Integer = 0 To dt.Rows.Count - 1
                odaa.SelectCommand.CommandText = "AccountInsertFields"
                odaa.SelectCommand.CommandType = CommandType.StoredProcedure
                odaa.SelectCommand.Transaction = tran
                odaa.SelectCommand.Parameters.Clear()
                odaa.SelectCommand.Parameters.AddWithValue("Eidtarget", res)
                odaa.SelectCommand.Parameters.AddWithValue("Eidsource", Eidsource)
                odaa.SelectCommand.Parameters.AddWithValue("Documenttype", dt.Rows(w).Item("FormName").ToString().Trim())
                odaa.SelectCommand.ExecuteNonQuery()
            Next
            For i As Integer = 0 To dt.Rows.Count - 1
                Dim dt11 As New DataTable
                Dim docname As String = dt.Rows(i).Item("FormName").ToString().Trim()
                Dim oda1 As SqlDataAdapter = New SqlDataAdapter("Select * from MMM_Mst_Fields Where Eid='" & res & "' and Documenttype='" & dt.Rows(i).Item("FormName").ToString().Trim() & "'", con)
                oda1.SelectCommand.Transaction = tran

                ' Dim oda1 As SqlDataAdapter = New SqlDataAdapter("Select * from MMM_Mst_Fields Where Eid='146' and Documenttype ='" & docname & "'", con)
                oda1.Fill(dt11)
                For j As Integer = 0 To dt11.Rows.Count - 1
                    Dim documenttype As String = dt11.Rows(j).Item("Documenttype")
                    Dim oldfieldid As String = dt11.Rows(j).Item("Oldtid")
                    Dim newfieldid As String = dt11.Rows(j).Item("Fieldid")
                    oda1.SelectCommand.CommandText = "Select * from MMM_Mst_Fields Where Eid='" & res & "' and Documenttype='" & dt.Rows(i).Item("FormName").ToString().Trim() & "'  and oldtid <> '" & dt11.Rows(j).Item("Oldtid").ToString().Trim() & "'"
                    oda1.SelectCommand.CommandType = CommandType.Text
                    oda1.SelectCommand.Transaction = tran
                    Dim dt1 As New DataTable
                    oda1.Fill(dt1)

                    For k As Integer = 0 To dt1.Rows.Count - 1
                        Dim changelukupvalue As String = String.Empty
                        Dim changelukupvaluenew As String = String.Empty
                        Dim changeddllukupvalue As String = String.Empty
                        Dim changedropdownvalue As String = String.Empty
                        Dim changekclogicvalue As String = String.Empty
                        Dim changedependentONvalue As String = String.Empty
                        Dim changeinitialFiltervalue As String = String.Empty
                        Dim lookupvalue As String = dt1.Rows(k).Item("lookupvalue").ToString().Trim()
                        Dim ddllookupvalue As String = dt1.Rows(k).Item("ddllookupvalue").ToString().Trim()
                        Dim dropdown As String = dt1.Rows(k).Item("dropdown").ToString().Trim()
                        Dim KC_LOGIC As String = dt1.Rows(k).Item("KC_LOGIC").ToString().Trim()

                        Dim KC_VALUE As String = dt1.Rows(k).Item("KC_VALUE").ToString().Trim()

                        Dim dependentON As String = dt1.Rows(k).Item("dependentON").ToString().Trim()
                        Dim initialFilter As String = dt1.Rows(k).Item("initialFilter").ToString().Trim()
                        Dim cal_fields As String = dt1.Rows(k).Item("cal_fields").ToString().Trim()
                        Dim flag As Boolean = False


                        '''''''''''''''''''''''' For LukUp'''''''''''''''''''''''
                        If (lookupvalue <> "" And lookupvalue.Length > 1) Then
                            lookupvalue = lookupvalue.Substring(0, lookupvalue.Length - 1)
                            Dim splitlukup As String() = Split(lookupvalue, ",")
                            Dim modifylukupvalue As String = ""
                            Dim modifylukupvaluenew As String = ""
                            For l As Integer = 0 To splitlukup.Length - 1
                                Dim A As String() = Split(splitlukup(l), "-")
                                If A.Length > 1 Then
                                    Dim mainvalue As String = A(0)
                                    If mainvalue.Trim() = oldfieldid.Trim() Then
                                        modifylukupvalue &= newfieldid.Trim() & "-" & A(1) & ","
                                        flag = True
                                    Else
                                        modifylukupvalue &= mainvalue.Trim() & "-" & A(1) & ","
                                    End If
                                End If
                            Next
                            changelukupvalue = modifylukupvalue.Trim()
                            changelukupvaluenew = modifylukupvaluenew.Trim()
                        End If
                        '''''''''''''''''''''''' For DDlLukUp'''''''''''''''''''''''''''
                        If (ddllookupvalue <> "" And ddllookupvalue.Length > 1) Then
                            Dim modifyddllukupvalue As String = ""
                            ddllookupvalue = ddllookupvalue.Substring(0, ddllookupvalue.Length - 1)
                            Dim splitddllukup As String() = Split(ddllookupvalue, ",")
                            For n As Integer = 0 To splitddllukup.Length - 1
                                Dim B As String() = Split(splitddllukup(n), "-")
                                If B.Length > 1 Then
                                    Dim mainvalueddllukup As String = B(0)
                                    If mainvalueddllukup.Trim() = oldfieldid.Trim() Then
                                        modifyddllukupvalue &= newfieldid.Trim() & "-" & B(1) & ","
                                        flag = True
                                    Else
                                        modifyddllukupvalue &= mainvalueddllukup.Trim() & "-" & B(1) & ","
                                    End If
                                End If
                            Next
                            changeddllukupvalue = modifyddllukupvalue.Trim()
                        End If
                        ''''''''''''''''''''' For DropDown''''''''''''''''''''''''''''''''''''
                        If (dropdown <> "" And dropdown.Length > 1) Then
                            If dropdown.Trim() = oldfieldid.Trim() Then
                                changedropdownvalue = newfieldid.Trim()
                                flag = True
                            Else
                                changedropdownvalue = dropdown.Trim()
                            End If
                        End If
                        ''''''''''''''''''''' For KC_LOgic''''''''''''''''''''''''''''''''''''
                        If (KC_LOGIC <> "" And KC_LOGIC.Length > 1) Then
                            Dim modifykclogicvalue As String = ""
                            Dim splitkclogic As String() = Split(KC_LOGIC, "-")
                            For p As Integer = 0 To splitkclogic.Length - 1
                                If splitkclogic.Length > 1 Then
                                    Dim mainkclogicvalue As String = splitkclogic(p)
                                    If mainkclogicvalue.Trim() = oldfieldid.Trim() Then
                                        modifykclogicvalue &= newfieldid.Trim() & "-"
                                        flag = True
                                    Else
                                        modifykclogicvalue &= mainkclogicvalue.Trim() & "-"
                                    End If
                                    changekclogicvalue = modifykclogicvalue.Trim()
                                End If
                            Next
                            If changekclogicvalue <> "" Then
                                changekclogicvalue = changekclogicvalue.Substring(0, changekclogicvalue.Length - 1)
                            End If
                        End If
                        ''''''''''''''''''''' For dependentON''''''''''''''''''''''''''''''''''''
                        If (dependentON <> "" And dependentON.Length > 1) Then
                            dependentON = dependentON.Substring(0, dependentON.Length - 1)
                            Dim modifydependentONvalue As String = ""
                            Dim splitdependentON As String() = Split(dependentON, ",")
                            For r As Integer = 0 To splitdependentON.Length - 1
                                If splitdependentON(r).Length > 0 Then
                                    Dim maindependentONvalue As String = splitdependentON(r)
                                    If maindependentONvalue.Trim() = oldfieldid.Trim() Then
                                        modifydependentONvalue &= newfieldid.Trim() & ","
                                        flag = True
                                    Else
                                        modifydependentONvalue &= maindependentONvalue.Trim() & ","
                                    End If
                                End If
                            Next
                            changedependentONvalue = modifydependentONvalue.Trim()
                        End If
                        ''''''''''''''''''''' For initialFilter''''''''''''''''''''''''''''''''''''
                        If (initialFilter <> "" And initialFilter.Length > 1) Then
                            initialFilter = initialFilter.Substring(0, initialFilter.Length - 1)
                            Dim modifyinitialFiltervalue As String = ""
                            Dim splitinitialFilter As String() = Split(initialFilter, ":")
                            For r As Integer = 0 To splitinitialFilter.Length - 1
                                If splitinitialFilter.Length > 1 Then
                                    Dim maininitialFiltervalue As String = splitinitialFilter(r)
                                    If maininitialFiltervalue.Trim() = oldfieldid.Trim() Then
                                        modifyinitialFiltervalue &= newfieldid.Trim() & ":"
                                        flag = True
                                    Else
                                        modifyinitialFiltervalue &= maininitialFiltervalue.Trim() & ":"
                                    End If
                                    changeinitialFiltervalue = modifyinitialFiltervalue.Trim()
                                End If
                            Next
                            If changeinitialFiltervalue <> "" Then
                                changeinitialFiltervalue = changeinitialFiltervalue.Substring(0, changeinitialFiltervalue.Length - 1)
                            End If

                        End If

                        ''''''''''''''''''''''''''Update Query'''''''''''''''''''''''''''''''''' 
                        If flag = True Then
                            oda1.SelectCommand.CommandText = "Update MMM_Mst_Fields set lookupvalue='" & changelukupvalue & "',ddllookupvalue='" & changeddllukupvalue & "',dropdown='" & changedropdownvalue & "',KC_LOGIC='" & changekclogicvalue & "',dependentON='" & changedependentONvalue & "',initialFilter='" & changeinitialFiltervalue & "'  where eid='" & res & "' and fieldid='" & dt1.Rows(k).Item("fieldid") & "'"
                            ' oda1.SelectCommand.CommandText = "Update MMM_Mst_Fields set tempcount=tempcount+1,templukupval='" & changelukupvaluenew & "',lookupvalue='" & changelukupvalue & "'  where Eid='" & res & "' and fieldid='" & dt1.Rows(k).Item("fieldid") & "'"
                            oda1.SelectCommand.CommandType = CommandType.Text
                            oda1.SelectCommand.Transaction = tran
                            If con.State <> ConnectionState.Open Then
                                con.Open()
                            End If
                            oda1.SelectCommand.ExecuteNonQuery()
                        End If
                        If (cal_fields <> "") Then
                            ' oda1.SelectCommand.CommandText = "update MMM_Mst_Fields set cal_fields=replace(cal_fields,'fld" & oldfieldid & "'')','fld" & newfieldid & "'')') where Eid='" & res & "' and fieldid='" & dt1.Rows(k).Item("fieldid") & "'"
                            oda1.SelectCommand.CommandText = "update MMM_Mst_Fields set cal_fields=replace(cal_fields,'fld" & oldfieldid & "'')','fld" & newfieldid & "'')') where eid='" & res & "' and fieldid='" & dt1.Rows(k).Item("fieldid") & "'"
                            oda1.SelectCommand.CommandType = CommandType.Text
                            oda1.SelectCommand.Transaction = tran
                            If con.State <> ConnectionState.Open Then
                                con.Open()
                            End If
                            oda1.SelectCommand.ExecuteNonQuery()
                        End If
                        ' End If
                    Next
                    dt1.Dispose()
                Next

            Next



            'Add condition for Child Item Total and Child Item Max in fields table
            Dim objChildItemTotal As New DataTable()
            Dim objDC As New DataClass
            objChildItemTotal = objDC.TranExecuteQryDT("select fieldid,dropdown from mmm_mst_fields where fieldtype in  ('Child Item Total','Child Item MAX') and  eid=" & res & " and len(dropdown)>1", con:=con, tran:=tran)
            If objChildItemTotal.Rows.Count > 0 Then
                For Each drChildItemTotal As DataRow In objChildItemTotal.Rows
                    Dim objDTdropDown As New DataTable
                    objDTdropDown = objDC.TranExecuteQryDT(" select fieldid from mmm_mst_fields where oldtid =" & drChildItemTotal("dropdown") & " and eid=" & res, con:=con, tran:=tran)
                    If objDTdropDown.Rows.Count > 0 Then
                        Dim dropdownValue As String = objDTdropDown.Rows(0)(0)
                        objDC.TranExecuteQryDT(" Update mmm_mst_fields set dropdown='" & dropdownValue & "' where fieldid= " & drChildItemTotal("fieldid") & " and eid=" & res, con:=con, tran:=tran)
                    End If
                Next
            End If

            'Add condition for Child Item Total and Child Item Max in fields table

            'Add condition for Replace fieldid in rule condition

            Dim objRuleDT As New DataTable()
            objRuleDT = objDC.TranExecuteQryDT("Select * from mmm_mst_rules where eid=" & Eidsource, con, tran)
            For Each dr As DataRow In objRuleDT.Rows
                Dim objTargetRuleDT As New DataTable()
                objTargetRuleDT = objDC.TranExecuteQryDT("Select ruleid, isnull(ControlField,'') as ControlField,isnull(TargetControlField,'') as TargetControlField,isnull(TargetSpecificControl,'') as TargetSpecificControl from mmm_mst_rules where eid=" & res & " and oldid=" & dr("ruleid"), con, tran)
                If objTargetRuleDT.Rows.Count > 0 Then
                    Dim ControlField As String = objTargetRuleDT.Rows(0)("ControlField")
                    Dim TargetControlField As String = objTargetRuleDT.Rows(0)("TargetControlField")
                    Dim TargetSpecificControl As String = objTargetRuleDT.Rows(0)("TargetSpecificControl")
                    If ControlField <> "" And ControlField <> String.Empty Then
                        objDC.TranExecuteQryDT("update mmm_mst_rules set controlfield= (select fieldid from mmm_mst_fields where oldtid=" & ControlField & " and eid=" & res & ") where ruleid=" & objTargetRuleDT.Rows(0)("ruleid") & "", con, tran)
                    End If
                    If TargetControlField <> "" And TargetControlField <> String.Empty Then
                        Dim listofTargetControlFields As String() = TargetControlField.Split(",")
                        Dim targetcontrollistArray As New ArrayList()

                        For li As Integer = 0 To listofTargetControlFields.Length - 1
                            Dim fieldid As String = objDC.TranExecuteQryScaller("select fieldid from mmm_mst_fields where oldtid=" & listofTargetControlFields(li) & " and eid=" & res & "", con, tran)
                            If fieldid <> "" Then
                                targetcontrollistArray.Add(fieldid)
                            End If

                        Next

                        If targetcontrollistArray.Count > 0 Then
                            Dim targetControlArrayData As String = String.Join(",", targetcontrollistArray.ToArray())
                            objDC.TranExecuteQryDT("update mmm_mst_rules set TargetControlField= '" & targetControlArrayData.ToString() & "' where ruleid=" & objTargetRuleDT.Rows(0)("ruleid") & "", con, tran)
                        End If

                    End If
                End If
            Next
            'or fieldType='Calculative Field'
            'Replace values for Calculative fields
            Dim objDTCalCulativeFields As New DataTable
            objDTCalCulativeFields = objDC.TranExecuteQryDT("select oldtid,fieldid,isnull(cal_fields,'') as cal_fields  from mmm_mst_fields where eid=" & res & " and cal_fields is not null", con:=con, tran:=tran)
            If objDTCalCulativeFields.Rows.Count > 0 Then
                For Each drCalCulativeFields As DataRow In objDTCalCulativeFields.Rows
                    If drCalCulativeFields("cal_fields") <> "" Then
                        objDC.TranExecuteQryDT("update MMM_Mst_Fields set cal_fields=replace(cal_fields,'fld" & drCalCulativeFields("oldtid") & "'')','fld" & drCalCulativeFields("fieldid") & "'')') where eid='" & res & "' and fieldid='" & drCalCulativeFields("fieldid") & "'", con:=con, tran:=tran)
                    End If
                Next
            End If
            'Replace values for Calculative fields

            'Insert for Rules
            Dim daa As SqlDataAdapter = New SqlDataAdapter("select * from MMM_MST_FORMVALIDATION Where Eid='" & Eidsource & "'", con)
            daa.SelectCommand.CommandType = CommandType.Text
            daa.SelectCommand.Transaction = tran
            Dim dtf As New DataTable
            daa.Fill(dtf)
            daa.SelectCommand.CommandText = "InsertFORMVALIDATION"
            daa.SelectCommand.CommandType = CommandType.StoredProcedure
            daa.SelectCommand.Transaction = tran
            daa.SelectCommand.Parameters.Clear()
            daa.SelectCommand.Parameters.AddWithValue("Eidtarget", res)
            daa.SelectCommand.Parameters.AddWithValue("Eidsource", Eidsource)
            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            daa.SelectCommand.ExecuteNonQuery()
            dtf.Dispose()

            ' Dim res As String = "165"
            Dim da As SqlDataAdapter = New SqlDataAdapter("Select * from MMM_MST_FORMVALIDATION Where Eid='" & res & "'", con)
            da.SelectCommand.CommandType = CommandType.Text
            da.SelectCommand.Transaction = tran
            Dim dv As New DataTable
            da.Fill(dv)
            For j As Integer = 0 To dv.Rows.Count - 1
                Dim documenttype As String = dv.Rows(j).Item("DocTYPE")
                Dim fldID As String = dv.Rows(j).Item("fldID")
                Dim Oprator As String = dv.Rows(j).Item("Operator")
                Dim Value As String = dv.Rows(j).Item("Value")
                Dim changefldIDvalue As String = ""
                Dim changeOpratorvalue As String = ""
                Dim changevalue As String = ""
                Dim flagV As Boolean = False

                If (fldID <> "") Or (Oprator <> "") Then
                    If (fldID <> "") Then
                        Dim valuefldid As String() = Split(fldID, "fld")
                        Dim a1 As String = valuefldid(1)
                        da.SelectCommand.CommandText = "Select fieldid from MMM_Mst_Fields Where Eid='" & res & "' and oldtid='" & a1 & "'"
                        da.SelectCommand.CommandType = CommandType.Text
                        da.SelectCommand.Transaction = tran
                        Dim dte As New DataTable
                        da.Fill(dte)
                        If dte.Rows.Count > 0 Then
                            changefldIDvalue = "fld" & dte.Rows(0).Item("fieldid").ToString()
                            flagV = True
                        Else
                            changefldIDvalue = fldID
                        End If
                        dte.Dispose()
                    End If
                    If (Oprator <> "") Then
                        Dim dto As New DataTable
                        If Oprator.Contains("fld") Then
                            Dim valueOprator As String() = Split(Oprator, "fld")
                            Dim ba1 As String = valueOprator(1)
                            da.SelectCommand.CommandText = "Select fieldid from MMM_Mst_Fields Where Eid='" & res & "' and oldtid='" & ba1 & "'"
                            da.SelectCommand.CommandType = CommandType.Text
                            da.SelectCommand.Transaction = tran
                            da.Fill(dto)
                            If dto.Rows.Count > 0 Then
                                changeOpratorvalue = "fld" & dto.Rows(0).Item("fieldid").ToString()
                                flagV = True
                            End If
                        Else
                            changeOpratorvalue = Oprator
                        End If
                        dto.Dispose()
                    End If
                End If
                If (Value <> "") Then
                    Dim valueOprator As String = Value & ":"
                    Dim ba1 As String() = Split(valueOprator, ":")
                    'valueOprator = valueOprator.Substring(0, valueOprator.Length - 1)
                    For er As Integer = 0 To ba1.Length - 1
                        If ba1(er).ToString() <> "" Then
                            Dim dev As String = ba1(er) & ":"
                            Dim ert As String() = Split(dev, "=")
                            For g As Integer = 0 To ert.Length - 1
                                If ert(g).Contains(":") Then
                                    If ert(g).Contains("fld") Then
                                        Dim y As String = ert(g).Replace("fld", "")
                                        Dim q As String = y.Replace(":", "")
                                        da.SelectCommand.CommandText = "Select fieldid from MMM_Mst_Fields Where Eid='" & res & "' and oldtid='" & q & "'"
                                        da.SelectCommand.CommandType = CommandType.Text
                                        da.SelectCommand.Transaction = tran
                                        Dim dtv As New DataTable
                                        da.Fill(dtv)
                                        If dtv.Rows.Count > 0 Then
                                            Dim oldfld As String = "fld" & q & ":"
                                            Dim newfld As String = "fld" & dtv.Rows(0).Item("fieldid").ToString() & ":"
                                            valueOprator = valueOprator.Replace(oldfld, newfld)
                                        End If
                                    End If
                                End If
                            Next
                        End If
                    Next
                    valueOprator = valueOprator.Substring(0, valueOprator.Length - 1)
                    da.SelectCommand.CommandText = "Update MMM_MST_FORMVALIDATION set Value='" & valueOprator & "'  where eid='" & res & "' and TID='" & dv.Rows(j).Item("TID") & "'"
                    If con.State <> ConnectionState.Open Then
                        con.Open()
                    End If
                    da.SelectCommand.ExecuteNonQuery()
                End If


                ''''''''''''''''''''''''''Update Query'''''''''''''''''''''''''''''''''' 
                If flagV = True Then
                    da.SelectCommand.CommandText = "Update MMM_MST_FORMVALIDATION set fldID='" & changefldIDvalue & "',Operator='" & changeOpratorvalue & "' where eid='" & res & "' and TID='" & dv.Rows(j).Item("TID") & "'"
                    If con.State <> ConnectionState.Open Then
                        con.Open()
                    End If
                    da.SelectCommand.ExecuteNonQuery()
                End If
                'If (Value <> "") Then
                '    da.SelectCommand.CommandText = "Update MMM_MST_FORMVALIDATION set Value=replace(Value + ':','fld' +'" & oldfieldidp & "' +':','fld' +'" & newfieldidp & "' +':')  where eid='" & res & "' and TID='" & dt1.Rows(j).Item("TID") & "'"
                '    If con.State <> ConnectionState.Open Then
                '        con.Open()
                '    End If
                '    da.SelectCommand.ExecuteNonQuery()
                '    da.SelectCommand.CommandText = "Update MMM_MST_FORMVALIDATION set Value=left(value,len(value)-1)  where eid='" & res & "' and TID='" & dt1.Rows(j).Item("TID") & "'"
                '    If con.State <> ConnectionState.Open Then
                '        con.Open()
                '    End If
                '    da.SelectCommand.ExecuteNonQuery()
                'End If
            Next
            da.Dispose()

            ''''''''''''''''''''''''''''''''''''''''''''Create view''''''''''''''''''''''''''''''''''''''
            Dim odv As SqlDataAdapter = New SqlDataAdapter("", con)
            odv.SelectCommand.CommandText = "CreateNewView"
            odv.SelectCommand.CommandType = CommandType.StoredProcedure
            odv.SelectCommand.Transaction = tran
            odv.SelectCommand.Parameters.Clear()
            odv.SelectCommand.Parameters.AddWithValue("Eid", res)
            odv.SelectCommand.ExecuteNonQuery()
            ' ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

            'Add codition for Need to act config 
            Dim ht As New Hashtable()
            ht.Add("@Eidtarget", res)
            ht.Add("@Eidsource", Eidsource)
            objDC.TranExecuteProDT("Insert_needtoactconfig", ht:=ht, con:=con, tran:=tran)
            'Add codition for Need to act config


            'Add condition for doc panel Entry
            ht.Clear()
            ht.Add("@Eidtarget", res)
            ht.Add("@Eidsource", Eidsource)
            objDC.TranExecuteProDT("Insert_MainFormDocPanel", ht:=ht, con:=con, tran:=tran)

            Dim dtDocPanel As New DataTable
            dtDocPanel = objDC.TranExecuteQryDT("select * from  docPanel where eid=" & res, con:=con, tran:=tran)
            If dtDocPanel.Rows.Count > 0 Then
                Dim updatePanelQry As New ArrayList()
                For Each drDocPanel As DataRow In dtDocPanel.Rows
                    updatePanelQry.Add("update mmm_mst_fields set displaypanelid=" & drDocPanel("docPanelID") & " where documenttype='" & drDocPanel("documenttype") & "' and  eid=" & res & " and displaypanelid=" & drDocPanel("oldPanelID") & " ;")
                Next
                If updatePanelQry.Count > 0 Then
                    objDC.TranExecuteQryDT(String.Join(" ", updatePanelQry.ToArray()), con:=con, tran:=tran)
                End If
            End If
            'Add condition for doc panel Entry

            'Add condition for doc detail panel Entry
            ht.Clear()
            ht.Add("@Eidtarget", res)
            ht.Add("@Eidsource", Eidsource)
            objDC.TranExecuteProDT("Insert_doc_dtl_panel", ht:=ht, con:=con, tran:=tran)

            Dim docdtlPanel As New DataTable
            docdtlPanel = objDC.TranExecuteQryDT("select * from mmm_doc_dtl_panel where eid=" & res, con:=con, tran:=tran)
            If docdtlPanel.Rows.Count > 0 Then
                For Each drdocDtlPanel As DataRow In docdtlPanel.Rows
                    Dim newFieldID As String = objDC.TranExecuteQryDT("SELECT STUFF((SELECT ',' + convert(varchar(max), fieldid) from mmm_mst_fields where oldtid in (" & drdocDtlPanel("AccessFieldId") & ") and eid=" & res & " and documenttype='" & drdocDtlPanel("documenttype") & "' FOR XML PATH('')) ,1,1,'') as Response", con:=con, tran:=tran).Rows(0)(0)
                    objDC.TranExecuteQryDT("Update mmm_doc_dtl_panel set AccessFieldId='" & newFieldID & "' where panel_id=" & drdocDtlPanel("panel_id") & " and eid=" & res, con:=con, tran:=tran)
                Next
            End If

            'Add condition for doc detail panel Entry



            reset()
            lblmsgg.Visible = True
            lblmsgg.Text = "New Account created through Cloning Successfully!"
            'Commiting transaction 
            tran.Commit()

            '' new added by sunil 13_aug_18 for dir. creation
            Dim folder = Server.MapPath("~/DOCS/" & res)
            If Not Directory.Exists(folder) Then
                Directory.CreateDirectory(folder)
            End If
            '' new added

        Catch ex As Exception
            If Not tran Is Nothing Then
                tran.Rollback()
            End If
            ' Throw
            lblMsg.Text = "Error found - " & ex.Message.ToString()
        Finally
            If Not con Is Nothing Then
                con.Close()
            End If
        End Try
    End Sub

    Public Sub reset()

        txtAcCode.Text = ""
        txtAcName.Text = ""
        txtUserName.Text = ""
        txtEmail.Text = ""
        ' txtPWD.Text = "1@million"
        '  txtRePwd.Text = ""
        '  ddlitemcopy.SelectedIndex = 0
        ddlentityname.SelectedIndex = 0
    End Sub


End Class
