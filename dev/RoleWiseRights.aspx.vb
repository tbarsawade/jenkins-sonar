Imports System.Data
Imports Newtonsoft.Json
Imports System.Web.Script.Serialization
Imports Newtonsoft.Json.Converters


Partial Class RoleWiseRights
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        'GetMenuData()
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
    Public Sub GetMenuData()
        'Dim objDC As New DataClass()
        'Try
        '    Dim table As New DataTable()
        '    table = objDC.ExecuteQryDT(" ;with mainMenu(RoleName,MenuName,MenuID) as( select m.rolename,stuff((select ','+ MenuName from MMM_MST_Menu  N  WHERE EID=" & Session("EID") & " AND  pmenu=0 and N.Roles LIKE '{%'+M.rolename+'%' 				FOR XML PATH('')				 ),1,1,''),stuff((select ','+cast(Mid as nvarchar) from MMM_MST_Menu  N  WHERE EID=" & Session("EID") & " AND  pmenu=0 and N.Roles LIKE '{%'+M.rolename+'%' 				FOR XML PATH('')				 ),1,1,'') from mmm_mst_role as m where eid=" & Session("EID") & ")	 select * from mainmenu")
        '    Dim dt = New DataTable()
        '    dt.Columns.Add("Role Name")
        '    dt.Columns.Add("Main Menu")
        '    dt.Columns.Add("Parent Menu")
        '    Try
        '        For Each dr As DataRow In table.Rows
        '            Dim MenuName As String() = Convert.ToString(dr("MenuName")).Split(",")
        '            Dim MenuID As String() = Convert.ToString(dr("MenuID")).Split(",")
        '            For i As Integer = 0 To MenuID.Length - 1
        '                If MenuID(i) <> String.Empty Then
        '                    Dim SubMenuDt As New DataTable()
        '                    SubMenuDt = AddChildItems("select MID,MenuName from mmm_mst_menu where pmenu=" & MenuID(i) & " and eid=" & Session("EID"))
        '                    If SubMenuDt.Rows.Count > 0 Then
        '                        dt.Rows.Add(dr("RoleName"), MenuName(i), MenuName(i))
        '                        For Each drsubmenu As DataRow In SubMenuDt.Rows
        '                            dt.Rows.Add(dr("RoleName"), MenuName(i), drsubmenu("MenuName"))

        '                            Dim PSubMenuDt As New DataTable()
        '                            PSubMenuDt = AddChildItems("select MID,MenuName from mmm_mst_menu where pmenu=" & drsubmenu("MID") & " and eid=" & Session("EID"))
        '                            For Each drpsubmenu As DataRow In PSubMenuDt.Rows
        '                                dt.Rows.Add(dr("RoleName"), drsubmenu("MenuName"), drpsubmenu("MenuName"))
        '                                Dim PPSubMenuDt As New DataTable()
        '                                PPSubMenuDt = AddChildItems("select MID,MenuName from mmm_mst_menu where pmenu=" & drpsubmenu("MID") & " and eid=" & Session("EID"))
        '                                For Each drppsubmenu As DataRow In PPSubMenuDt.Rows
        '                                    dt.Rows.Add(dr("RoleName"), drpsubmenu("MenuName"), PPSubMenuDt("MenuName"))
        '                                Next
        '                            Next
        '                        Next
        '                    Else
        '                        dt.Rows.Add(dr("RoleName"), MenuName(i), MenuName(i))
        '                    End If
        '                End If
        '            Next
        '        Next
        '    Catch ex As Exception

        '    End Try


        'Catch ex As Exception
        'Finally

        'End Try

    End Sub
    <System.Web.Services.WebMethod()>
    Public Shared Function GetUser() As ReturnCollection
        Dim ret As String = ""
        Dim objDC As New DataClass()
        Try
            Dim dt As New DataTable
            dt = objDC.ExecuteQryDT("select distinct username,cast(uid as nvarchar) as userid from mmm_mst_user where eid=" & HttpContext.Current.Session("EID") & " order by username")
            Dim serializerSettings As New JsonSerializerSettings()
            Dim json_serializer As New JavaScriptSerializer()
            serializerSettings.Converters.Add(New DataTableConverter())
            Dim returnCollection As New ReturnCollection()
            ret = JsonConvert.SerializeObject(dt, Newtonsoft.Json.Formatting.None, serializerSettings)
            returnCollection.ds = ret
            Return returnCollection
        Catch ex As Exception

        End Try
    End Function
    <System.Web.Services.WebMethod()>
    Public Shared Function GetRoleMenuData(UID As String) As Reportrsps
        Dim jsonData As String = ""
        Dim res As New Reportrsps()
        Dim objDC As New DataClass()
        Try
            Dim dtUser As New DataTable()
            dtUser = objDC.ExecuteQryDT("select distinct userrole,username,uid from mmm_mst_user where eid=" & HttpContext.Current.Session("EID") & " and uid in(" & UID & ")")
            'Read User Wise Data
            Dim dt = New DataTable()
            dt.Columns.Add("UserName")
            dt.Columns.Add("RoleName")
            dt.Columns.Add("MenuName")
            dt.Columns.Add("SubMenuName")
            For Each druser As DataRow In dtUser.Rows
                Dim arrRole As New ArrayList()
                arrRole.Add(druser("userrole"))
                Dim dtPreRole As New DataTable()
                dtPreRole = objDC.ExecuteQryDT(";with PreRole(PUID,PRoleName) as( select Distinct uid, stuff(( select ','+RoleName from MMM_ref_PreRole_user U   WHERE EID=" & HttpContext.Current.Session("EID") & " and U.UID=UU.UID 	FOR XML PATH('')),1,1,'') as RoleName from MMM_ref_PreRole_user UU   WHERE EID=" & HttpContext.Current.Session("EID") & " and UU.uid=" & druser("uid") & ") select * from PreRole")
                If dtPreRole.Rows.Count > 0 Then
                    arrRole.Add(dtPreRole.Rows(0)("PRoleName"))
                End If
                Dim dtPostRole As New DataTable()
                dtPostRole = objDC.ExecuteQryDT(";with PostRole(POUID,PORoleName)as(select Distinct uid, stuff(( select ','+RoleName from MMM_Ref_Role_User U   WHERE EID=" & HttpContext.Current.Session("EID") & " and U.UID=UU.UID 	FOR XML PATH('')),1,1,'') as RoleName from MMM_Ref_Role_User UU   WHERE EID=" & HttpContext.Current.Session("EID") & " and UU.uid=" & druser("uid") & ")select * from PostRole")
                If dtPostRole.Rows.Count > 0 Then
                    arrRole.Add(dtPostRole.Rows(0)("PORoleName"))
                End If
                arrRole.ToArray().Distinct()
                Dim finalRole As New ArrayList()
                Dim tempRoleval As String = String.Join(",", arrRole.ToArray().Distinct())
                Dim tempRoleValue As String() = tempRoleval.ToString().Split(",")
                For k As Integer = 0 To tempRoleValue.Length - 1
                    If Not finalRole.ToArray().Contains(tempRoleValue(k)) Then
                        finalRole.Add(tempRoleValue(k))
                    End If
                Next
                'Remove Duplicate
                Dim t As Integer = 0
                Do While (t < finalRole.Count)
                    Dim j As Integer = (t + 1)
                    Do While (j < finalRole.Count)
                        If (finalRole(t).ToString.Trim.ToUpper = finalRole(j).ToString.Trim.ToUpper) Then
                            finalRole.Remove(finalRole(j))
                        End If
                        j = (j + 1)
                    Loop
                    t = (t + 1)
                Loop
                'Remove Duplicate

                Dim table As New DataTable()
                Dim ht As New Hashtable()
                ht.Add("@EID", HttpContext.Current.Session("EID"))
                ht.Add("@SuppRoleName", String.Join(",", finalRole.ToArray()) & ",")
                ht.Add("@UserName", druser("username"))
                table = objDC.ExecuteProDT("GetUSERRights", ht)
                dt.Merge(table)
                'For z As Integer = 0 To finalRole.Count - 1
                '    'Old code
                '    Dim table As New DataTable()
                '    table = objDC.ExecuteQryDT(" ;with mainMenu(RoleName,MenuName,MenuID) as( select m.rolename,stuff((select ','+ MenuName from MMM_MST_Menu  N  WHERE EID=" & HttpContext.Current.Session("EID") & " AND  pmenu=0 and N.Roles LIKE '{%'+M.rolename+'%' 				FOR XML PATH('')				 ),1,1,''),stuff((select ','+cast(Mid as nvarchar) from MMM_MST_Menu  N  WHERE EID=" & HttpContext.Current.Session("EID") & " AND  pmenu=0 and N.Roles LIKE '{%'+M.rolename+'%' 				FOR XML PATH('')				 ),1,1,'') from mmm_mst_role as m where eid=" & HttpContext.Current.Session("EID") & ")	 select * from mainmenu where rolename='" & finalRole(z) & "'")

                '    For Each dr As DataRow In table.Rows
                '        Dim MenuName As String() = Convert.ToString(dr("MenuName")).Split(",")
                '        Dim MenuID As String() = Convert.ToString(dr("MenuID")).Split(",")
                '        For i As Integer = 0 To MenuID.Length - 1
                '            If MenuID(i) <> String.Empty Then
                '                Dim SubMenuDt As New DataTable()
                '                SubMenuDt = AddChildItems("select MID,MenuName from mmm_mst_menu where pmenu=" & MenuID(i) & " and eid=" & HttpContext.Current.Session("EID"))
                '                If SubMenuDt.Rows.Count > 0 Then
                '                    dt.Rows.Add(druser("username"), dr("RoleName"), MenuName(i), MenuName(i))
                '                    For Each drsubmenu As DataRow In SubMenuDt.Rows
                '                        dt.Rows.Add(druser("username"), dr("RoleName"), MenuName(i), drsubmenu("MenuName"))

                '                        Dim PSubMenuDt As New DataTable()
                '                        PSubMenuDt = AddChildItems("select MID,MenuName from mmm_mst_menu where pmenu=" & drsubmenu("MID") & " and eid=" & HttpContext.Current.Session("EID"))
                '                        For Each drpsubmenu As DataRow In PSubMenuDt.Rows
                '                            dt.Rows.Add(druser("username"), dr("RoleName"), drsubmenu("MenuName"), drpsubmenu("MenuName"))
                '                            Dim PPSubMenuDt As New DataTable()
                '                            PPSubMenuDt = AddChildItems("select MID,MenuName from mmm_mst_menu where pmenu=" & drpsubmenu("MID") & " and eid=" & HttpContext.Current.Session("EID"))
                '                            For Each drppsubmenu As DataRow In PPSubMenuDt.Rows
                '                                dt.Rows.Add(druser("username"), dr("RoleName"), drpsubmenu("MenuName"), PPSubMenuDt("MenuName"))
                '                            Next
                '                        Next
                '                    Next
                '                Else
                '                    dt.Rows.Add(druser("username"), dr("RoleName"), MenuName(i), MenuName(i))
                '                End If
                '            End If
                '        Next
                '    Next
                '    'Old code
                'Next
            Next
            Dim serializerSettings As New JsonSerializerSettings()
            Dim json_serializer As New JavaScriptSerializer()

            serializerSettings.Converters.Add(New DataTableConverter())
            Dim lstColumns As New List(Of egrdcolumns1rsps)
            Dim objColumn As egrdcolumns1rsps
            '  res.aggregate = "["

            For Each dc As DataColumn In dt.Columns
                objColumn = New egrdcolumns1rsps()
                objColumn.field = Replace(dc.ColumnName.Replace("-", ""), " ", "")
                objColumn.title = dc.ColumnName
                If (objColumn.title.Length > 5) Then
                    If (objColumn.title.Length < 11) Then
                        objColumn.width = objColumn.title.Length * 18
                    ElseIf (objColumn.title.Length < 16) Then
                        objColumn.width = objColumn.title.Length * 19
                    Else
                        objColumn.width = objColumn.title.Length * 11
                    End If

                End If

                lstColumns.Add(objColumn)
                dc.ColumnName = dc.ColumnName.Replace("-", "").Replace(" ", "")
            Next
            '  res.aggregate &= "]"
            jsonData = JsonConvert.SerializeObject(dt, Newtonsoft.Json.Formatting.None, serializerSettings)
            res.data = jsonData
            res.columns = lstColumns
            jsonData = JsonConvert.SerializeObject(dt, Newtonsoft.Json.Formatting.None)
            Return res
        Catch ex As Exception

        End Try
    End Function

    'best Work 21-Sept-2016
    '<System.Web.Services.WebMethod()>
    'Public Shared Function GetRoleMenuData() As Reportrsps
    '    Dim jsonData As String = ""
    '    Dim res As New Reportrsps()
    '    Dim objDC As New DataClass()
    '    Try
    '        Dim dtUser As New DataTable()
    '        dtUser = objDC.ExecuteQryDT("select distinct userrole,username,uid from mmm_mst_user where eid=" & HttpContext.Current.Session("EID"))
    '        'Read User Wise Data
    '        Dim dt = New DataTable()
    '        dt.Columns.Add("User Name")
    '        dt.Columns.Add("Role Name")
    '        dt.Columns.Add("Main Menu")
    '        dt.Columns.Add("Parent Menu")
    '        For Each druser As DataRow In dtUser.Rows
    '            Dim arrRole As New ArrayList()
    '            arrRole.Add(druser("userrole"))
    '            Dim dtPreRole As New DataTable()
    '            dtPreRole = objDC.ExecuteQryDT(";with PreRole(PUID,PRoleName) as( select Distinct uid, stuff(( select ','+RoleName from MMM_ref_PreRole_user U   WHERE EID=" & HttpContext.Current.Session("EID") & " and U.UID=UU.UID 	FOR XML PATH('')),1,1,'') as RoleName from MMM_ref_PreRole_user UU   WHERE EID=" & HttpContext.Current.Session("EID") & " and UU.uid=" & druser("uid") & ") select * from PreRole")
    '            If dtPreRole.Rows.Count > 0 Then
    '                arrRole.Add(dtPreRole.Rows(0)("PRoleName"))
    '            End If
    '            Dim dtPostRole As New DataTable()
    '            dtPostRole = objDC.ExecuteQryDT(";with PostRole(POUID,PORoleName)as(select Distinct uid, stuff(( select ','+RoleName from MMM_Ref_Role_User U   WHERE EID=" & HttpContext.Current.Session("EID") & " and U.UID=UU.UID 	FOR XML PATH('')),1,1,'') as RoleName from MMM_Ref_Role_User UU   WHERE EID=" & HttpContext.Current.Session("EID") & " and UU.uid=" & druser("uid") & ")select * from PostRole")
    '            If dtPostRole.Rows.Count > 0 Then
    '                arrRole.Add(dtPostRole.Rows(0)("PORoleName"))
    '            End If
    '            arrRole.ToArray().Distinct()
    '            Dim finalRole As New ArrayList()
    '            Dim tempRoleval As String = String.Join(",", arrRole.ToArray().Distinct())
    '            Dim tempRoleValue As String() = tempRoleval.ToString().Split(",")
    '            For k As Integer = 0 To tempRoleValue.Length - 1
    '                If Not finalRole.ToArray().Contains(tempRoleValue(k)) Then
    '                    finalRole.Add(tempRoleValue(k))
    '                End If
    '            Next

    '            'Remove Duplicate
    '            Dim t As Integer = 0
    '            Do While (t < finalRole.Count)
    '                Dim j As Integer = (t + 1)
    '                Do While (j < finalRole.Count)
    '                    If (finalRole(t).ToString.Trim.ToUpper = finalRole(j).ToString.Trim.ToUpper) Then
    '                        finalRole.Remove(finalRole(j))
    '                    End If
    '                    j = (j + 1)
    '                Loop
    '                t = (t + 1)
    '            Loop
    '            'Remove Duplicate
    '            For z As Integer = 0 To finalRole.Count - 1
    '                'Old code
    '                Dim table As New DataTable()
    '                table = objDC.ExecuteQryDT(" ;with mainMenu(RoleName,MenuName,MenuID) as( select m.rolename,stuff((select ','+ MenuName from MMM_MST_Menu  N  WHERE EID=" & HttpContext.Current.Session("EID") & " AND  pmenu=0 and N.Roles LIKE '{%'+M.rolename+'%' 				FOR XML PATH('')				 ),1,1,''),stuff((select ','+cast(Mid as nvarchar) from MMM_MST_Menu  N  WHERE EID=" & HttpContext.Current.Session("EID") & " AND  pmenu=0 and N.Roles LIKE '{%'+M.rolename+'%' 				FOR XML PATH('')				 ),1,1,'') from mmm_mst_role as m where eid=" & HttpContext.Current.Session("EID") & ")	 select * from mainmenu where rolename='" & finalRole(z) & "'")

    '                For Each dr As DataRow In table.Rows
    '                    Dim MenuName As String() = Convert.ToString(dr("MenuName")).Split(",")
    '                    Dim MenuID As String() = Convert.ToString(dr("MenuID")).Split(",")
    '                    For i As Integer = 0 To MenuID.Length - 1
    '                        If MenuID(i) <> String.Empty Then
    '                            Dim SubMenuDt As New DataTable()
    '                            SubMenuDt = AddChildItems("select MID,MenuName from mmm_mst_menu where pmenu=" & MenuID(i) & " and eid=" & HttpContext.Current.Session("EID"))
    '                            If SubMenuDt.Rows.Count > 0 Then
    '                                dt.Rows.Add(druser("username"), dr("RoleName"), MenuName(i), MenuName(i))
    '                                For Each drsubmenu As DataRow In SubMenuDt.Rows
    '                                    dt.Rows.Add(druser("username"), dr("RoleName"), MenuName(i), drsubmenu("MenuName"))

    '                                    Dim PSubMenuDt As New DataTable()
    '                                    PSubMenuDt = AddChildItems("select MID,MenuName from mmm_mst_menu where pmenu=" & drsubmenu("MID") & " and eid=" & HttpContext.Current.Session("EID"))
    '                                    For Each drpsubmenu As DataRow In PSubMenuDt.Rows
    '                                        dt.Rows.Add(druser("username"), dr("RoleName"), drsubmenu("MenuName"), drpsubmenu("MenuName"))
    '                                        Dim PPSubMenuDt As New DataTable()
    '                                        PPSubMenuDt = AddChildItems("select MID,MenuName from mmm_mst_menu where pmenu=" & drpsubmenu("MID") & " and eid=" & HttpContext.Current.Session("EID"))
    '                                        For Each drppsubmenu As DataRow In PPSubMenuDt.Rows
    '                                            dt.Rows.Add(druser("username"), dr("RoleName"), drpsubmenu("MenuName"), PPSubMenuDt("MenuName"))
    '                                        Next
    '                                    Next
    '                                Next
    '                            Else
    '                                dt.Rows.Add(druser("username"), dr("RoleName"), MenuName(i), MenuName(i))
    '                            End If
    '                        End If
    '                    Next
    '                Next
    '                'Old code
    '            Next
    '        Next
    '        Dim serializerSettings As New JsonSerializerSettings()
    '        Dim json_serializer As New JavaScriptSerializer()

    '        serializerSettings.Converters.Add(New DataTableConverter())
    '        Dim lstColumns As New List(Of egrdcolumns1rsps)
    '        Dim objColumn As egrdcolumns1rsps
    '        '  res.aggregate = "["

    '        For Each dc As DataColumn In dt.Columns
    '            objColumn = New egrdcolumns1rsps()
    '            objColumn.field = Replace(dc.ColumnName.Replace("-", ""), " ", "")
    '            objColumn.title = dc.ColumnName
    '            If (objColumn.title.Length > 5) Then
    '                If (objColumn.title.Length < 11) Then
    '                    objColumn.width = objColumn.title.Length * 18
    '                ElseIf (objColumn.title.Length < 16) Then
    '                    objColumn.width = objColumn.title.Length * 19
    '                Else
    '                    objColumn.width = objColumn.title.Length * 11
    '                End If

    '            End If

    '            lstColumns.Add(objColumn)
    '            dc.ColumnName = dc.ColumnName.Replace("-", "").Replace(" ", "")
    '        Next
    '        '  res.aggregate &= "]"
    '        jsonData = JsonConvert.SerializeObject(dt, Newtonsoft.Json.Formatting.None, serializerSettings)
    '        res.data = jsonData
    '        res.columns = lstColumns
    '        jsonData = JsonConvert.SerializeObject(dt, Newtonsoft.Json.Formatting.None)
    '        Return res
    '    Catch ex As Exception

    '    End Try
    'End Function

    Public Shared Function AddChildItems(ByRef query As String) As DataTable
        Dim dt As New DataTable
        Dim objDC As New DataClass()
        dt = objDC.ExecuteQryDT(query)
        Return dt
    End Function
    'Private Sub AddChildItems(ByVal table As DataTable, ByVal menuItem As MenuItem)
    '    Dim viewItem As New DataView(table)
    '    viewItem.RowFilter = "Pmenu= " + menuItem.Value + ""
    '    For Each childView As DataRowView In viewItem
    '        Dim abc As String() = childView("roles").ToString().Split(",")
    '        For i As Integer = 0 To abc.Length - 1
    '            Dim a As String() = abc(i).ToString().Split(":")
    '            If UCase(Session("USERROLE")) = UCase(a(0).Remove(0, 1).ToString()) Then
    '                Dim childItem As New MenuItem(childView("MenuName").ToString, childView("MID").ToString, "Images/" & childView("Image") & "", childView("Pagelink").ToString)
    '                If UCase(childView("Pagelink")).ToString = "MENU" Then
    '                    childItem.NavigateUrl = UCase(childView("Pagelink")).ToString().Replace("MENU", "#")
    '                Else
    '                    childItem.NavigateUrl = childView("Pagelink").ToString()
    '                End If


    '                Dim result As String = checkview(childView("roles").ToString())
    '                If UCase(result) = "EXIT" And UCase(menuItem.Text.ToString()) = "DOCUMENT" Then
    '                    'If Not UCase(childView("Pagelink")).ToString.Contains("DOCUMENTS.ASPX?SC=") Then
    '                    Continue For
    '                End If
    '                menuItem.ChildItems.Add(childItem)
    '                AddChildItems(table, childItem)
    '            End If
    '        Next
    '    Next
    'End Sub
End Class
Public Class Reportrsps
    Public Property data As String
    Public Property columns As List(Of egrdcolumns1rsps)
End Class
Public Class egrdcolumns1rsps
    Public Property field As String
    Public Property title As String
    Public Property type As String = ""
    Public Property width As Integer = 100
End Class
Public Class ReturnCollection
    Public Property ds As String
End Class