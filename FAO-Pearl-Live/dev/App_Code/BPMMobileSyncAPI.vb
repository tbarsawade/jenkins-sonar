Imports System.Data
Imports System.Data.SqlClient
Imports System.Net
Imports System.IO
Imports System.Xml.Serialization
Imports System.Runtime.Serialization
Imports System.ServiceModel
Imports System.ServiceModel.Web
Imports System.Xml


' NOTE: You can use the "Rename" command on the context menu to change the class name "BPMMobileSyncAPI" in code, svc and config file together.
Public Class BPMMobileSyncAPI
    Implements IBPMMobileSyncAPI

    Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString 'conStrLive

    Dim BPMKEY As String = ConfigurationManager.AppSettings("BPMKEY").ToString()


    Function AuthanticateUser(UserID As String, Password As String, ECode As String, IMINumber As String) As Userdetails Implements IBPMMobileSyncAPI.AuthanticateUser
        Dim ret As String = "Fail"
        Dim uObj As New User()
        Dim ds As New DataSet
        Dim objU As New Userdetails()
        Dim enitity As Integer = 0
        Try
            objU = CommanUtil.AuthanticateUser(UserID, Password, ECode)
            'Code For Authantication for mobile registration 
            If objU.UID > 0 Then
                Dim objDiv As New DeviceInfo()
                objDiv.IMINumber = IMINumber
                Dim dsD As New DataSet()
                dsD = objDiv.ValidateDevice(objU.EID, objU.UID)
                If dsD.Tables(0).Rows.Count = 0 Then
                    'reset User details
                    objU = New Userdetails()
                End If
            End If
        Catch ex As Exception
        Finally
        End Try
        Return objU
    End Function
    Function AuthanticateUserMultiRole(UserID As String, Password As String, ECode As String, IMINumber As String) As Userdetails Implements IBPMMobileSyncAPI.AuthanticateUserMultiRole
        Dim ret As String = "Fail"
        Dim uObj As New User()
        Dim ds As New DataSet
        Dim objU As New Userdetails()
        Dim enitity As Integer = 0
        Try
            objU = CommanUtil.AuthanticateUser(UserID, Password, ECode)
            'Code For Authantication for mobile registration 
            If objU.UID > 0 Then
                Dim objDiv As New DeviceInfo()
                objDiv.IMINumber = IMINumber
                Dim dsD As New DataSet()
                dsD = objDiv.ValidateDevice(objU.EID, objU.UID)
                If dsD.Tables(0).Rows.Count = 0 Then
                    'reset User details
                    objU = New Userdetails()
                Else
                    Dim DSRoles As New DataSet()
                    Dim Query = "SELECT SUBSTRING((SELECT ',' + t.userrole  from(select distinct rolename [userrole] from MMM_REF_ROLE_USER where eid=" & objU.EID & " AND UID=" & objU.UID & " union select rolename [userrole] from MMM_ref_PreRole_user where eid=" & objU.EID & " AND UID=" & objU.UID & " union select userrole from MMM_MST_user where eid=" & objU.EID & " AND UID=" & objU.UID & ") t for XML PATH('')),2,2000) AS 'UserRole' "
                    Using con = New SqlConnection(conStr)
                        Using da = New SqlDataAdapter(Query, con)
                            da.Fill(DSRoles)
                        End Using
                    End Using
                    If DSRoles.Tables(0).Rows.Count > 0 Then
                        Dim oUser As New User
                        '' restriction code starts
                        Dim hh As String = ""
                        Dim cr As String = ""
                        Dim rr As String = ""
                        Dim rls As String() = oUser.roles.ToString.Split(",")

                        For i As Integer = 0 To rls.Length - 1
                            Using con = New SqlConnection(conStr)
                                Using da = New SqlDataAdapter("Select accesstype from mmm_mst_role where eid= " & oUser.EID & " and rolename='" & rls(i).ToString() & "'", con)
                                    da.Fill(DSRoles, "access")
                                End Using
                            End Using
                            ' da.SelectCommand.CommandText = "Select accesstype from mmm_mst_role where eid= " & oUser.EID & " and rolename='" & rls(i).ToString() & "'"
                            hh = DSRoles.Tables("access").Rows(0).Item(0).ToString
                            If Val(hh) > 0 Then
                                cr = cr & rls(i).ToString().Trim() & ","
                            End If
                        Next
                        If cr.Length > 0 Then
                            rr = cr
                            rr = rr.Remove(rr.Length - 1)
                            cr = cr.Substring(0, cr.IndexOf(","))
                            'cr = cr.Substring(0, (cr.ToString = ",").ToString.Length)
                            objU.Roles = cr
                        End If
                        'Ends access check code
                        objU.Roles = cr
                    End If
                End If
            End If
        Catch ex As Exception
        Finally
        End Try
        Return objU
    End Function




    Function SyncDataA(Key As String, UID As String, EID As String, URole As String, IMINum As String, ST As String) As String Implements IBPMMobileSyncAPI.SyncDataA
        Try
            Dim result As String = ""
            Dim ret = ""
            Dim con As SqlConnection = Nothing
            Dim da As SqlDataAdapter = Nothing
            Dim DocQuery As New StringBuilder()
            Dim MstQuery As New StringBuilder()
            Dim ChildQuery As New StringBuilder()
            Dim lstData As New List(Of DocData)
            Dim objDev As New DeviceInfo()
            Dim SBData As New StringBuilder()
            Dim IsValidrequest = False
            Try
                con = New SqlConnection(conStr)
                objDev.IMINumber = IMINum
                Dim dsD = New DataSet()
                dsD = objDev.ValidateDevice(EID, UID)
                If dsD.Tables(0).Rows.Count > 0 Then
                    IsValidrequest = True
                    If Not ST.ToUpper() = "COMPLETE" Then
                        ret = dsD.Tables(0).Rows(0).Item("LastUpdatedOn").ToString()
                    Else
                        ret = "01-01-1900 12:25:25.010"
                    End If
                End If
                If (Key = BPMKEY) And IsValidrequest = True Then
                    'Getting all the form name
                    Dim Doclist = ""
                    Dim DocListCr = GetFilteredDocA(EID, UID)
                    Dim arrCurDoc = DocListCr.ToString.Split(",")
                    Dim arrPDoc = GetFilteredDoc(EID, URole).ToString.Split(",")
                    Dim lstDOC = arrCurDoc.Except(arrPDoc)
                    Dim comma = ""
                    For Each item In lstDOC
                        Doclist = Doclist + comma + item
                        comma = ","
                    Next
                    If Doclist.Trim <> "" Then
                        Dim FormQuery = "select * FROM MMM_MST_FORMS where EID=" & EID & " AND FormType in('DOCUMENT','MASTER') and FormName in (" & Doclist & ")"
                        If con.State <> ConnectionState.Open Then
                            con.Open()
                        End If
                        da = New SqlDataAdapter(FormQuery, con)
                        Dim dsForm As New DataSet()
                        da.Fill(dsForm)
                        'getting the setting of last sync
                        Dim dsLstSync = New DataSet()
                        Using dalastSync As New SqlDataAdapter("SELECT SYNC.* FROM mmm_mst_lastMobileSync SYNC JOIN MMM_MST_ROLE ROLE ON SYNC.ROLEID=ROLE.ROLEID WHERE SYNC.eid=" + EID + "and ROLE.RoleName='" + URole + "'", con)
                            dalastSync.Fill(dsLstSync)
                        End Using
                        Dim dtSyncSetting = New DataTable()
                        'Getting last synk Date
                        'Creating Query
                        If dsForm.Tables(0).Rows.Count > 0 Then
                            For f As Integer = 0 To dsForm.Tables(0).Rows.Count - 1
                                'filtering the lastsync setting for particular datatype from datatable
                                dtSyncSetting = (From myRow In dsLstSync.Tables(0).AsEnumerable() Where myRow.Field(Of String)("Documenttype") = dsForm.Tables(0).Rows(f).Item("FormName").ToString() AndAlso myRow.Field(Of Boolean)("IsActive") = True).AsDataView().ToTable()
                                If dsForm.Tables(0).Rows(f).Item("FormType").ToString().ToUpper() = "MASTER" Then
                                    If String.IsNullOrEmpty(MstQuery.ToString()) Then
                                        If ret = "NEW" Then
                                            MstQuery.Append("select *,0 As DOCID,createdBy AS 'ouid',0 AS lasttid,'' As Curstatus,'' AS CurDocNature,0 As Priority FROM MMM_MST_MASTER with (nolock) WHERE EID=" & EID & " and isauth=1 and DocumentType='" & dsForm.Tables(0).Rows(f).Item("FormName") & "'")
                                        Else
                                            MstQuery.Append("select *,0 As DOCID,createdBy AS 'ouid' ,0 AS lasttid,'' As Curstatus,'' AS CurDocNature,0 As Priority FROM MMM_MST_MASTER with (nolock) WHERE EID=" & EID & "  and DocumentType='" & dsForm.Tables(0).Rows(f).Item("FormName") & "'")
                                        End If
                                    Else
                                        If ret = "NEW" Then
                                            MstQuery.Append(" union all select *,0 As DOCID,createdBy AS 'ouid' ,0 AS lasttid,'' As Curstatus,'' AS CurDocNature,0 As Priority FROM MMM_MST_MASTER with (nolock) WHERE EID=" & EID & " and isauth=1 and DocumentType='" & dsForm.Tables(0).Rows(f).Item("FormName") & "'")
                                        Else
                                            MstQuery.Append(" union all select *,0 As DOCID,createdBy AS 'ouid' ,0 AS lasttid,'' As Curstatus,'' AS CurDocNature,0 As Priority FROM MMM_MST_MASTER with (nolock) WHERE EID=" & EID & "  and DocumentType='" & dsForm.Tables(0).Rows(f).Item("FormName") & "'")
                                        End If

                                    End If
                                    'appending 'and' condition in MstQuery for lastsync depending on 'NoOfDays' and 'field' column
                                    If (dtSyncSetting.Rows.Count > 0) Then
                                        Dim NoOfDays = Convert.ToInt32(dtSyncSetting.Rows(0).Item("NoOfDays"))
                                        MstQuery.Append(" and convert(date," + dtSyncSetting.Rows(0).Item("Field").ToString + ",3)>=convert(DATE, GETDATE()-" + NoOfDays.ToString + ",3)")
                                    End If
                                    Dim Where = UserDataFilter_PreRole(dsForm.Tables(0).Rows(f).Item("FormName"), "MMM_MST_MASTER", EID, URole, UID)
                                    If Where <> "" Then
                                        MstQuery.Append(" AND tid in(" & Where & ")")
                                    End If
                                    If ret.ToString.ToUpper <> "NEW" Then
                                        MstQuery.Append(" AND " & "lastupdate >= " & "'" & ret & "'")
                                    End If
                                End If
                                If dsForm.Tables(0).Rows(f).Item("FormType").ToString().ToUpper() = "DOCUMENT" And dsForm.Tables(0).Rows(f).Item("FormSource").ToString().ToUpper() = "MENU DRIVEN" Then
                                    If String.IsNullOrEmpty(DocQuery.ToString()) Then
                                        If ret = "NEW" Then
                                            DocQuery.Append("select  *,0 As DOCID FROM MMM_MST_DOC with (nolock) WHERE EID=" & EID & " and isauth=1 and DocumentType='" & dsForm.Tables(0).Rows(f).Item("FormName") & "'")
                                        Else
                                            DocQuery.Append("select  *,0 As DOCID FROM MMM_MST_DOC with (nolock) WHERE EID=" & EID & " and DocumentType='" & dsForm.Tables(0).Rows(f).Item("FormName") & "'")
                                        End If
                                    Else
                                        If ret = "NEW" Then
                                            DocQuery.Append(" union all  select  *,0 As DOCID FROM MMM_MST_DOC with (nolock) WHERE EID=" & EID & " and isauth=1 and DocumentType='" & dsForm.Tables(0).Rows(f).Item("FormName") & "'")
                                        Else
                                            DocQuery.Append(" union all  select  *,0 As DOCID FROM MMM_MST_DOC with (nolock) WHERE EID=" & EID & "  and DocumentType='" & dsForm.Tables(0).Rows(f).Item("FormName") & "'")
                                        End If
                                    End If
                                    'appending 'and' condition in DocQuery for lastsync depending on 'NoOfDays' and 'field' column
                                    If (dtSyncSetting.Rows.Count > 0) Then
                                        Dim NoOfDays = Convert.ToInt32(dtSyncSetting.Rows(0).Item("NoOfDays"))
                                        DocQuery.Append(" and convert(date," + dtSyncSetting.Rows(0).Item("Field").ToString + ",3)>=convert(DATE, GETDATE()-" + NoOfDays.ToString + ",3)")
                                    End If
                                    Dim Where = UserDataFilter_PreRole(dsForm.Tables(0).Rows(f).Item("FormName"), "MMM_MST_DOC", EID, URole, UID)

                                    If Where <> "" Then
                                        DocQuery.Append(" AND tid in(" & Where & ")")
                                    End If
                                    If ret.ToString.ToUpper <> "NEW" Then
                                        DocQuery.Append("AND " & "lastupdate >= " & "'" & ret & "'")
                                    End If

                                End If
                            Next
                        End If
                        ChildQuery.Append("select *,1 As IsAuth FROM MMM_MST_DOC_Item where DOCID in(SELECT DOC.tid from ( " & DocQuery.ToString() & " ) As DOC )")
                    End If

                    'return only delta data 

                    If ret.ToString.ToUpper <> "NEW" Then
                        ChildQuery.Append("AND " & "lastupdate >= " & "'" & ret & "'")
                    End If
                    'Dim FinalQuery = MstQuery.ToString() & ";" & DocQuery.ToString() & ";" & ChildQuery.ToString()
                    'Appending Current Bucket Query to main Document Query

                    If DocQuery.ToString().Trim = "" Then
                        DocQuery.Append("Select  M.*,0 As DOCID  from MMM_MST_DOC M with (nolock)  LEFT OUTER JOIN MMM_DOC_DTL D with (nolock) on D.tid=M.lasttid where D.userid = " & UID & "  AND EID=" & EID & "")
                    Else
                        DocQuery.Append("UNION Select  M.*,0 As DOCID  from MMM_MST_DOC M with (nolock)  LEFT OUTER JOIN MMM_DOC_DTL D with (nolock) on D.tid=M.lasttid where D.userid = " & UID & "  AND EID=" & EID & "")
                    End If

                    Dim FinalQuery = MstQuery.ToString() & ";" & DocQuery.ToString()
                    If con.State <> ConnectionState.Open Then
                        con.Open()
                    End If
                    da = New SqlDataAdapter(FinalQuery, con)
                    Dim dsData As New DataSet()

                    da.Fill(dsData)

                    If dsData.Tables.Count > 0 Then
                        For i As Integer = 0 To dsData.Tables.Count - 1
                            If dsData.Tables(i).Rows.Count > 0 Then
                                For j As Integer = 0 To dsData.Tables(i).Rows.Count - 1
                                    SBData.Append("::")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("tid").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("DOCID").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("DocumentType").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("IsAuth").ToString() & "|")

                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld1").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld2").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld3").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld4").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld5").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld6").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld7").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld8").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld9").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld10").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld11").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld12").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld13").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld14").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld15").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld16").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld17").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld18").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld19").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld20").ToString() & "|")

                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld21").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld22").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld23").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld24").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld25").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld26").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld27").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld28").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld29").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld30").ToString() & "|")

                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld31").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld32").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld33").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld34").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld35").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld36").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld37").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld38").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld39").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld40").ToString() & "|")

                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld41").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld42").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld43").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld44").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld45").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld46").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld47").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld48").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld49").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld50").ToString() & "|")

                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld51").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld52").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld53").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld54").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld55").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld56").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld57").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld58").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld59").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld60").ToString() & "|")


                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld61").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld62").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld63").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld64").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld65").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld66").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld67").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld68").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld69").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld70").ToString() & "|")

                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld71").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld72").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld73").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld74").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld75").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld76").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld77").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld78").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld79").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld80").ToString() & "|")

                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld81").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld82").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld83").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld84").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld85").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld86").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld87").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld88").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld89").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld90").ToString() & "|")

                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld91").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld92").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld93").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld94").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld95").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld96").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld97").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld98").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld99").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld100").ToString() & "|")

                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld101").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld102").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld103").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld104").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld105").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld106").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld107").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld108").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld109").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld110").ToString() & "|")

                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld111").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld112").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld113").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld114").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld115").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld116").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld117").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld118").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld119").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld120").ToString() & "|")


                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld121").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld122").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld123").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld124").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld125").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld126").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld127").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld128").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld129").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld130").ToString() & "|")


                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld131").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld132").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld133").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld134").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld135").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld136").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld137").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld138").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld139").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld140").ToString() & "|")

                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld141").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld142").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld143").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld144").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld145").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld146").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld147").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld148").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld149").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld150").ToString() & "|")

                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld151").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld152").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld153").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld154").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld155").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld156").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld157").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld158").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld159").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld160").ToString() & "|")

                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld161").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld162").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld163").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld164").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld165").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld166").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld167").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld168").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld169").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld170").ToString() & "|")

                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld171").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld172").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld173").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld174").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld175").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld176").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld177").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld178").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld179").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld180").ToString() & "|")

                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld181").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld182").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld183").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld184").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld185").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld186").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld187").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld188").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld189").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld190").ToString() & "|")

                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld191").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld192").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld193").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld194").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld195").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld196").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld197").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld198").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld199").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld200").ToString() & "|")

                                    SBData.Append(dsData.Tables(i).Rows(j).Item("OUID").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("lasttid").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("curstatus").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("CurDocNature").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("Priority").ToString())
                                Next
                            End If
                        Next
                    End If

                    result = SBData.ToString()
                Else
                    result = "Sorry!!! Your Authantication failed."
                End If
            Catch ex As Exception
                Throw New FaultException("RTO")
            Finally
                If Not con Is Nothing Then
                    con.Close()
                    con.Dispose()
                End If
                If Not da Is Nothing Then
                    da.Dispose()
                End If
            End Try
            Try

                Return result
            Catch ex As Exception
                Throw New FaultException(ex.Message)
            End Try
        Catch ex As Exception
            Throw New FaultException(ex.Message)
        End Try
    End Function

    Function SyncData(Key As String, UID As String, EID As String, URole As String, IMINum As String, ST As String) As String Implements IBPMMobileSyncAPI.SyncData

        Try
            Dim result As String = ""
            Dim ret = ""
            Dim con As SqlConnection = Nothing
            Dim da As SqlDataAdapter = Nothing
            Dim DocQuery As New StringBuilder()
            Dim MstQuery As New StringBuilder()
            Dim ChildQuery As New StringBuilder()
            Dim lstData As New List(Of DocData)
            Dim objDev As New DeviceInfo()
            Dim SBData As New StringBuilder()
            Dim IsValidrequest = False
            Try
                con = New SqlConnection(conStr)
                objDev.IMINumber = IMINum
                Dim dsD = New DataSet()
                dsD = objDev.ValidateDevice(EID, UID)
                If dsD.Tables(0).Rows.Count > 0 Then
                    IsValidrequest = True
                    If Not ST.ToUpper() = "COMPLETE" Then
                        ret = dsD.Tables(0).Rows(0).Item("LastUpdatedOn").ToString()
                    Else
                        ret = "NEW"
                    End If
                End If
                If (Key = BPMKEY) And IsValidrequest = True Then
                    'Getting all the form name
                    Dim Doclist = GetFilteredDoc(EID, URole).ToString
                    If Doclist Is Nothing Or String.IsNullOrEmpty(Doclist.ToString.Trim) Then
                        Doclist = "'No Any Right founds for the supplyed role'"
                    End If
                    Dim FormQuery = "select * FROM MMM_MST_FORMS where EID=" & EID & " AND FormType in('DOCUMENT','MASTER') and FormName in (" & Doclist & ")"
                    If con.State <> ConnectionState.Open Then
                        con.Open()
                    End If
                    da = New SqlDataAdapter(FormQuery, con)
                    Dim dsForm As New DataSet()
                    da.Fill(dsForm)
                    'getting the setting of last sync
                    Dim dsLstSync = New DataSet()
                    Using dalastSync As New SqlDataAdapter("SELECT SYNC.* FROM mmm_mst_lastMobileSync SYNC JOIN MMM_MST_ROLE ROLE ON SYNC.ROLEID=ROLE.ROLEID WHERE SYNC.eid=" + EID + "and ROLE.RoleName='" + URole + "'", con)
                        dalastSync.Fill(dsLstSync)
                    End Using
                    Dim dtSyncSetting = New DataTable()
                    'Getting last synk Date
                    'Creating Query
                    If dsForm.Tables(0).Rows.Count > 0 Then
                        For f As Integer = 0 To dsForm.Tables(0).Rows.Count - 1
                            Dim doct = dsForm.Tables(0).Rows(f).Item("FormName").ToString()
                            If doct = "Night Guard Roster" Then
                                doct = doct
                            End If
                            'filtering the lastsync setting for particular datatype from datatable
                            dtSyncSetting = (From myRow In dsLstSync.Tables(0).AsEnumerable() Where myRow.Field(Of String)("Documenttype") = dsForm.Tables(0).Rows(f).Item("FormName").ToString() AndAlso myRow.Field(Of Boolean)("IsActive") = True).AsDataView().ToTable()
                            If dsForm.Tables(0).Rows(f).Item("FormType").ToString().ToUpper() = "MASTER" Then
                                If String.IsNullOrEmpty(MstQuery.ToString()) Then
                                    If ret = "NEW" Then
                                        MstQuery.Append("select *,0 As DOCID,createdBy AS 'ouid' FROM MMM_MST_MASTER WHERE EID=" & EID & " and isauth=1 and DocumentType='" & dsForm.Tables(0).Rows(f).Item("FormName") & "'")
                                    Else
                                        MstQuery.Append("select *,0 As DOCID,createdBy AS 'ouid' FROM MMM_MST_MASTER WHERE EID=" & EID & "  and DocumentType='" & dsForm.Tables(0).Rows(f).Item("FormName") & "'")
                                    End If
                                Else
                                    If ret = "NEW" Then
                                        MstQuery.Append(" union all select *,0 As DOCID,createdBy AS 'ouid' FROM MMM_MST_MASTER WHERE EID=" & EID & " and isauth=1 and DocumentType='" & dsForm.Tables(0).Rows(f).Item("FormName") & "'")
                                    Else
                                        MstQuery.Append(" union all select *,0 As DOCID,createdBy AS 'ouid' FROM MMM_MST_MASTER WHERE EID=" & EID & "  and DocumentType='" & dsForm.Tables(0).Rows(f).Item("FormName") & "'")
                                    End If

                                End If
                                'appending 'and' condition in MstQuery for lastsync depending on 'NoOfDays' and 'field' column
                                If (dtSyncSetting.Rows.Count > 0) Then
                                    Dim NoOfDays = Convert.ToInt32(dtSyncSetting.Rows(0).Item("NoOfDays"))
                                    MstQuery.Append(" and convert(date," + dtSyncSetting.Rows(0).Item("Field").ToString + ",3)>=convert(DATE, GETDATE()-" + NoOfDays.ToString + ",3)")
                                End If
                                Dim Where = UserDataFilter_PreRole(dsForm.Tables(0).Rows(f).Item("FormName"), "MMM_MST_MASTER", EID, URole, UID)
                                If Where <> "" Then
                                    MstQuery.Append(" AND tid in(" & Where & ")")
                                End If
                                If ret.ToString.ToUpper <> "NEW" Then
                                    MstQuery.Append(" AND " & "lastupdate >= " & "'" & ret & "'")
                                End If
                            End If
                            If dsForm.Tables(0).Rows(f).Item("FormType").ToString().ToUpper() = "DOCUMENT" And dsForm.Tables(0).Rows(f).Item("FormSource").ToString().ToUpper() = "MENU DRIVEN" Then
                                If String.IsNullOrEmpty(DocQuery.ToString()) Then
                                    If ret = "NEW" Then
                                        DocQuery.Append("select  *,0 As DOCID FROM MMM_MST_DOC WHERE EID=" & EID & " and isauth=1 and DocumentType='" & dsForm.Tables(0).Rows(f).Item("FormName") & "'")
                                    Else
                                        DocQuery.Append("select  *,0 As DOCID FROM MMM_MST_DOC WHERE EID=" & EID & " and DocumentType='" & dsForm.Tables(0).Rows(f).Item("FormName") & "'")
                                    End If
                                Else
                                    If ret = "NEW" Then
                                        DocQuery.Append(" union all  select  *,0 As DOCID FROM MMM_MST_DOC WHERE EID=" & EID & " and isauth=1 and DocumentType='" & dsForm.Tables(0).Rows(f).Item("FormName") & "'")
                                    Else
                                        DocQuery.Append(" union all  select  *,0 As DOCID FROM MMM_MST_DOC WHERE EID=" & EID & "  and DocumentType='" & dsForm.Tables(0).Rows(f).Item("FormName") & "'")
                                    End If
                                End If
                                'appending 'and' condition in DocQuery for lastsync depending on 'NoOfDays' and 'field' column
                                If (dtSyncSetting.Rows.Count > 0) Then
                                    Dim NoOfDays = Convert.ToInt32(dtSyncSetting.Rows(0).Item("NoOfDays"))
                                    DocQuery.Append(" and convert(date," + dtSyncSetting.Rows(0).Item("Field").ToString + ",3)>=convert(DATE, GETDATE()-" + NoOfDays.ToString + ",3)")
                                End If
                                Dim Where = UserDataFilter_PreRole(dsForm.Tables(0).Rows(f).Item("FormName"), "MMM_MST_DOC", EID, URole, UID)
                                If Where <> "" Then
                                    DocQuery.Append(" AND tid in(" & Where & ")")
                                End If
                                If ret.ToString.ToUpper <> "NEW" Then
                                    DocQuery.Append("AND " & "lastupdate >= " & "'" & ret & "'")
                                End If

                            End If
                        Next
                    End If

                    ChildQuery.Append("select *,1 As IsAuth FROM MMM_MST_DOC_Item where DOCID in(SELECT DOC.tid from ( " & DocQuery.ToString() & " ) As DOC )")
                    'return only delta data 

                    If ret.ToString.ToUpper <> "NEW" Then
                        ChildQuery.Append("AND " & "lastupdate >= " & "'" & ret & "'")
                    End If
                    'Dim FinalQuery = MstQuery.ToString() & ";" & DocQuery.ToString() & ";" & ChildQuery.ToString()
                    Dim FinalQuery = MstQuery.ToString() & ";" & DocQuery.ToString()
                    If con.State <> ConnectionState.Open Then
                        con.Open()
                    End If
                    da = New SqlDataAdapter(FinalQuery, con)
                    Dim dsData As New DataSet()
                    da.Fill(dsData)

                    If dsData.Tables.Count > 0 Then
                        For i As Integer = 0 To dsData.Tables.Count - 1
                            If dsData.Tables(i).Rows.Count > 0 Then
                                For j As Integer = 0 To dsData.Tables(i).Rows.Count - 1

                                    SBData.Append("::")

                                    SBData.Append(dsData.Tables(i).Rows(j).Item("tid").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("DOCID").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("DocumentType").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("IsAuth").ToString() & "|")

                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld1").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld2").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld3").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld4").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld5").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld6").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld7").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld8").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld9").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld10").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld11").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld12").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld13").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld14").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld15").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld16").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld17").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld18").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld19").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld20").ToString() & "|")

                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld21").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld22").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld23").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld24").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld25").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld26").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld27").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld28").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld29").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld30").ToString() & "|")

                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld31").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld32").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld33").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld34").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld35").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld36").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld37").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld38").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld39").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld40").ToString() & "|")

                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld41").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld42").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld43").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld44").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld45").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld46").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld47").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld48").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld49").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld50").ToString() & "|")

                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld51").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld52").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld53").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld54").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld55").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld56").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld57").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld58").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld59").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld60").ToString() & "|")


                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld61").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld62").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld63").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld64").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld65").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld66").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld67").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld68").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld69").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld70").ToString() & "|")
                                    'new
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld71").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld72").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld73").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld74").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld75").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld76").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld77").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld78").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld79").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld80").ToString() & "|")

                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld81").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld82").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld83").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld84").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld85").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld86").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld87").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld88").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld89").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld90").ToString() & "|")

                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld91").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld92").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld93").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld94").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld95").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld96").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld97").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld98").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld99").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld100").ToString() & "|")

                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld101").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld102").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld103").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld104").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld105").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld106").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld107").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld108").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld109").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld110").ToString() & "|")

                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld111").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld112").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld113").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld114").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld115").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld116").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld117").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld118").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld119").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld120").ToString() & "|")


                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld121").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld122").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld123").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld124").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld125").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld126").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld127").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld128").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld129").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld130").ToString() & "|")


                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld131").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld132").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld133").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld134").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld135").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld136").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld137").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld138").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld139").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld140").ToString() & "|")

                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld141").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld142").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld143").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld144").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld145").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld146").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld147").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld148").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld149").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld150").ToString() & "|")

                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld151").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld152").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld153").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld154").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld155").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld156").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld157").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld158").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld159").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld160").ToString() & "|")

                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld161").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld162").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld163").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld164").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld165").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld166").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld167").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld168").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld169").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld170").ToString() & "|")

                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld171").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld172").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld173").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld174").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld175").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld176").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld177").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld178").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld179").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld180").ToString() & "|")

                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld181").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld182").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld183").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld184").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld185").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld186").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld187").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld188").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld189").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld190").ToString() & "|")

                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld191").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld192").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld193").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld194").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld195").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld196").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld197").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld198").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld199").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld200").ToString() & "|")

                                    SBData.Append(dsData.Tables(i).Rows(j).Item("OUID").ToString())
                                Next
                            End If
                        Next
                    End If
                    result = SBData.ToString()
                Else
                    result = "Sorry!!! Your Authantication failed."
                End If
            Catch ex As Exception
                Throw New FaultException("RTO")
            Finally
                If Not con Is Nothing Then
                    con.Close()
                    con.Dispose()
                End If
                If Not da Is Nothing Then
                    da.Dispose()
                End If
            End Try
            Try

                Return result
            Catch ex As Exception
                Throw New FaultException(ex.Message)
            End Try
        Catch ex As Exception
            Throw New FaultException(ex.Message)
        End Try
    End Function

    Function SyncDocDetails(Key As String, UID As String, EID As String, URole As String, IMINum As String, ST As String) As XElement Implements IBPMMobileSyncAPI.SyncDocDetails
        Try
            Dim result As String = ""
            Dim ret = ""
            Dim con As SqlConnection = Nothing
            Dim da As SqlDataAdapter = Nothing
            Dim DocQuery As New StringBuilder()
            Dim objDev As New DeviceInfo()
            Dim SBData As New StringBuilder("<docdetails>")
            Dim doc As New XDocument()
            Dim IsValidrequest = False
            Try
                con = New SqlConnection(conStr)
                objDev.IMINumber = IMINum
                Dim dsD = New DataSet()
                dsD = objDev.ValidateDevice(EID, UID)
                If dsD.Tables(0).Rows.Count > 0 Then
                    IsValidrequest = True
                    If Not ST.ToUpper() = "COMPLETE" Then
                        ret = dsD.Tables(0).Rows(0).Item("LastUpdatedOn").ToString()
                    Else
                        ret = "1900-01-01 12:25:25.010"
                    End If
                End If
                If (Key = BPMKEY) And IsValidrequest = True Then
                    DocQuery.Append(" SET NOCOUNT ON;select * FROM MMM_DOC_DTL where lastupdate >= '" & ret & "' and DOCID IN( Select   M.tid  from MMM_MST_DOC M with (nolock) LEFT OUTER JOIN MMM_DOC_DTL D with (nolock) on D.tid=M.lasttid  where D.userid = " & UID & " And EID = " & EID & ")")
                    DocQuery.Append(" union all select * FROM MMM_DOC_DTL where lastupdate >= '" & ret & "' and DOCID IN( Select   M.tid  from MMM_MST_DOC M with (nolock) LEFT OUTER JOIN MMM_DOC_DTL D with (nolock) on D.tid=M.lasttid  where M.OUID = " & UID & " And EID = " & EID & ")")
                    Dim dsData As New DataSet()
                    da = New SqlDataAdapter(DocQuery.ToString(), con)
                    da.Fill(dsData)
                    For i As Integer = 0 To dsData.Tables(0).Rows.Count - 1
                        SBData.Append("<Rows>")
                        SBData.Append("<tid>").Append(System.Security.SecurityElement.Escape(Convert.ToString(dsData.Tables(0).Rows(i).Item("tid")))).Append("</tid>")
                        SBData.Append("<userid>").Append(System.Security.SecurityElement.Escape(Convert.ToString(dsData.Tables(0).Rows(i).Item("userid")))).Append("</userid>")
                        SBData.Append("<docid>").Append(System.Security.SecurityElement.Escape(Convert.ToString(dsData.Tables(0).Rows(i).Item("docid")))).Append("</docid>")
                        SBData.Append("<fdate>").Append(System.Security.SecurityElement.Escape(Convert.ToString(dsData.Tables(0).Rows(i).Item("fdate")))).Append("</fdate>")
                        SBData.Append("<tdate>").Append(System.Security.SecurityElement.Escape(Convert.ToString(dsData.Tables(0).Rows(i).Item("tdate")))).Append("</tdate>")
                        SBData.Append("<ptat>").Append(System.Security.SecurityElement.Escape(Convert.ToString(dsData.Tables(0).Rows(i).Item("ptat")))).Append("</ptat>")
                        SBData.Append("<atat>").Append(System.Security.SecurityElement.Escape(Convert.ToString(dsData.Tables(0).Rows(i).Item("atat")))).Append("</atat>")
                        SBData.Append("<aprstatus>").Append(System.Security.SecurityElement.Escape(Convert.ToString(dsData.Tables(0).Rows(i).Item("aprstatus")))).Append("</aprstatus>")
                        SBData.Append("<remarks>").Append(System.Security.SecurityElement.Escape(Convert.ToString(dsData.Tables(0).Rows(i).Item("remarks")))).Append("</remarks>")
                        SBData.Append("<pathID>").Append(System.Security.SecurityElement.Escape(Convert.ToString(dsData.Tables(0).Rows(i).Item("pathID")))).Append("</pathID>")
                        SBData.Append("<Ordering>").Append(System.Security.SecurityElement.Escape(Convert.ToString(dsData.Tables(0).Rows(i).Item("Ordering")))).Append("</Ordering>")
                        SBData.Append("<DocNature>").Append(System.Security.SecurityElement.Escape(Convert.ToString(dsData.Tables(0).Rows(i).Item("DocNature")))).Append("</DocNature>")
                        SBData.Append("</Rows>")
                    Next
                End If
                SBData.Append("</docdetails>")
                result = SBData.ToString()
            Catch ex As Exception
                Throw New FaultException("RTO")
            Finally
                If Not con Is Nothing Then
                    con.Close()
                    con.Dispose()
                End If
                If Not da Is Nothing Then
                    da.Dispose()
                End If
            End Try
            Try
                doc = XDocument.Parse(result.ToString)
                Return doc.Root
            Catch ex As Exception
                Throw New FaultException(ex.Message)
            End Try
        Catch ex As Exception
            Throw New FaultException(ex.Message)
        End Try

    End Function

    Function SyncChild(Key As String, UID As String, EID As String, URole As String, IMINum As String, ST As String) As String Implements IBPMMobileSyncAPI.SyncChild
        Dim result As String = ""
        Dim ret = ""
        Dim con As SqlConnection = Nothing
        Dim da As SqlDataAdapter = Nothing
        Dim DocQuery As New StringBuilder()
        Dim objDev As New DeviceInfo()
        Dim SBData As New StringBuilder()
        Dim doc As New XDocument()
        Dim IsValidrequest = False
        Try
            con = New SqlConnection(conStr)
            objDev.IMINumber = IMINum
            Dim dsD = New DataSet()
            dsD = objDev.ValidateDevice(EID, UID)
            If dsD.Tables(0).Rows.Count > 0 Then
                IsValidrequest = True
                If Not ST.ToUpper() = "COMPLETE" Then
                    ret = dsD.Tables(0).Rows(0).Item("LastUpdatedOn").ToString()
                Else
                    ret = "1900-01-01 12:25:25.010"
                End If
            End If

            If (Key = BPMKEY) And IsValidrequest = True Then
                DocQuery.Append("SET NOCOUNT ON; select * FROM MMM_MST_DOC_Item where  lastupdate>= '" & ret & "' and  DOCID IN( Select   M.tid  from MMM_MST_DOC M with (nolock) LEFT OUTER JOIN MMM_DOC_DTL D with (nolock) on D.tid=M.lasttid  where D.userid = " & UID & " And EID = " & EID & ")")
                Dim dsData As New DataSet()
                da = New SqlDataAdapter(DocQuery.ToString(), con)
                da.Fill(dsData)
                For i As Integer = 0 To dsData.Tables(0).Rows.Count - 1
                    For j As Integer = 0 To dsData.Tables(i).Rows.Count - 1
                        SBData.Append("::")
                        SBData.Append(dsData.Tables(i).Rows(j).Item("tid").ToString() & "|")
                        SBData.Append(dsData.Tables(i).Rows(j).Item("DOCID").ToString() & "|")
                        SBData.Append(dsData.Tables(i).Rows(j).Item("DocumentType").ToString() & "|")
                        SBData.Append(dsData.Tables(i).Rows(j).Item("IsAuth").ToString() & "|")
                        SBData.Append(dsData.Tables(i).Rows(j).Item("fld1").ToString() & "|")
                        SBData.Append(dsData.Tables(i).Rows(j).Item("fld2").ToString() & "|")
                        SBData.Append(dsData.Tables(i).Rows(j).Item("fld3").ToString() & "|")
                        SBData.Append(dsData.Tables(i).Rows(j).Item("fld4").ToString() & "|")
                        SBData.Append(dsData.Tables(i).Rows(j).Item("fld5").ToString() & "|")
                        SBData.Append(dsData.Tables(i).Rows(j).Item("fld6").ToString() & "|")
                        SBData.Append(dsData.Tables(i).Rows(j).Item("fld7").ToString() & "|")
                        SBData.Append(dsData.Tables(i).Rows(j).Item("fld8").ToString() & "|")
                        SBData.Append(dsData.Tables(i).Rows(j).Item("fld9").ToString() & "|")
                        SBData.Append(dsData.Tables(i).Rows(j).Item("fld10").ToString() & "|")
                        SBData.Append(dsData.Tables(i).Rows(j).Item("fld11").ToString() & "|")
                        SBData.Append(dsData.Tables(i).Rows(j).Item("fld12").ToString() & "|")
                        SBData.Append(dsData.Tables(i).Rows(j).Item("fld13").ToString() & "|")
                        SBData.Append(dsData.Tables(i).Rows(j).Item("fld14").ToString() & "|")
                        SBData.Append(dsData.Tables(i).Rows(j).Item("fld15").ToString() & "|")
                        SBData.Append(dsData.Tables(i).Rows(j).Item("fld16").ToString() & "|")
                        SBData.Append(dsData.Tables(i).Rows(j).Item("fld17").ToString() & "|")
                        SBData.Append(dsData.Tables(i).Rows(j).Item("fld18").ToString() & "|")
                        SBData.Append(dsData.Tables(i).Rows(j).Item("fld19").ToString() & "|")
                        SBData.Append(dsData.Tables(i).Rows(j).Item("fld20").ToString() & "|")
                        SBData.Append(dsData.Tables(i).Rows(j).Item("fld21").ToString() & "|")
                        SBData.Append(dsData.Tables(i).Rows(j).Item("fld22").ToString() & "|")
                        SBData.Append(dsData.Tables(i).Rows(j).Item("fld23").ToString() & "|")
                        SBData.Append(dsData.Tables(i).Rows(j).Item("fld24").ToString() & "|")
                        SBData.Append(dsData.Tables(i).Rows(j).Item("fld25").ToString() & "|")
                        SBData.Append(dsData.Tables(i).Rows(j).Item("fld26").ToString() & "|")
                        SBData.Append(dsData.Tables(i).Rows(j).Item("fld27").ToString() & "|")
                        SBData.Append(dsData.Tables(i).Rows(j).Item("fld28").ToString() & "|")
                        SBData.Append(dsData.Tables(i).Rows(j).Item("fld29").ToString() & "|")
                        SBData.Append(dsData.Tables(i).Rows(j).Item("fld30").ToString() & "|")
                        SBData.Append(dsData.Tables(i).Rows(j).Item("fld31").ToString() & "|")
                        SBData.Append(dsData.Tables(i).Rows(j).Item("fld32").ToString() & "|")
                        SBData.Append(dsData.Tables(i).Rows(j).Item("fld33").ToString() & "|")
                        SBData.Append(dsData.Tables(i).Rows(j).Item("fld34").ToString() & "|")
                        SBData.Append(dsData.Tables(i).Rows(j).Item("fld35").ToString() & "|")
                        SBData.Append(dsData.Tables(i).Rows(j).Item("fld36").ToString() & "|")
                        SBData.Append(dsData.Tables(i).Rows(j).Item("fld37").ToString() & "|")
                        SBData.Append(dsData.Tables(i).Rows(j).Item("fld38").ToString() & "|")
                        SBData.Append(dsData.Tables(i).Rows(j).Item("fld39").ToString() & "|")
                        SBData.Append(dsData.Tables(i).Rows(j).Item("fld40").ToString() & "|")
                        SBData.Append(dsData.Tables(i).Rows(j).Item("fld41").ToString() & "|")
                        SBData.Append(dsData.Tables(i).Rows(j).Item("fld42").ToString() & "|")
                        SBData.Append(dsData.Tables(i).Rows(j).Item("fld43").ToString() & "|")
                        SBData.Append(dsData.Tables(i).Rows(j).Item("fld44").ToString() & "|")
                        SBData.Append(dsData.Tables(i).Rows(j).Item("fld45").ToString() & "|")
                        SBData.Append(dsData.Tables(i).Rows(j).Item("fld46").ToString() & "|")
                        SBData.Append(dsData.Tables(i).Rows(j).Item("fld47").ToString() & "|")
                        SBData.Append(dsData.Tables(i).Rows(j).Item("fld48").ToString() & "|")
                        SBData.Append(dsData.Tables(i).Rows(j).Item("fld49").ToString() & "|")
                        SBData.Append(dsData.Tables(i).Rows(j).Item("fld50").ToString() & "|")
                        SBData.Append(dsData.Tables(i).Rows(j).Item("fld51").ToString() & "|")
                        SBData.Append(dsData.Tables(i).Rows(j).Item("fld52").ToString() & "|")
                        SBData.Append(dsData.Tables(i).Rows(j).Item("fld53").ToString() & "|")
                        SBData.Append(dsData.Tables(i).Rows(j).Item("fld54").ToString() & "|")
                        SBData.Append(dsData.Tables(i).Rows(j).Item("fld55").ToString() & "|")
                        SBData.Append(dsData.Tables(i).Rows(j).Item("fld56").ToString() & "|")
                        SBData.Append(dsData.Tables(i).Rows(j).Item("fld57").ToString() & "|")
                        SBData.Append(dsData.Tables(i).Rows(j).Item("fld58").ToString() & "|")
                        SBData.Append(dsData.Tables(i).Rows(j).Item("fld59").ToString() & "|")
                        SBData.Append(dsData.Tables(i).Rows(j).Item("fld60").ToString() & "|")
                        SBData.Append(dsData.Tables(i).Rows(j).Item("fld61").ToString() & "|")
                        SBData.Append(dsData.Tables(i).Rows(j).Item("fld62").ToString() & "|")
                        SBData.Append(dsData.Tables(i).Rows(j).Item("fld63").ToString() & "|")
                        SBData.Append(dsData.Tables(i).Rows(j).Item("fld64").ToString() & "|")
                        SBData.Append(dsData.Tables(i).Rows(j).Item("fld65").ToString() & "|")
                        SBData.Append(dsData.Tables(i).Rows(j).Item("fld66").ToString() & "|")
                        SBData.Append(dsData.Tables(i).Rows(j).Item("fld67").ToString() & "|")
                        SBData.Append(dsData.Tables(i).Rows(j).Item("fld68").ToString() & "|")
                        SBData.Append(dsData.Tables(i).Rows(j).Item("fld69").ToString() & "|")
                        SBData.Append(dsData.Tables(i).Rows(j).Item("fld70").ToString())
                    Next
                Next
            End If
            result = SBData.ToString()
            Return result
        Catch ex As Exception
            Throw New FaultException("RTO")
        Finally
            If Not con Is Nothing Then
                con.Close()
                con.Dispose()
            End If
            If Not da Is Nothing Then
                da.Dispose()
            End If
        End Try

    End Function


    Public Function GetFilteredDocA(EID As Integer, UID As Integer) As String

        Dim con As New SqlConnection(conStr)
        Dim Document As New StringBuilder()
        Dim StrDocument = "", StrMaster = "", StrDocList = "'No Any Document Found'"
        Dim arrayListDoc As New List(Of String), item As String = ""
        Dim arrayListMas As New List(Of String)
        Dim arrayListDoc1 As New List(Of String)
        Try
            'get name of document and master on which user has right
            Dim MQuerry = "SET NOCOUNT ON;Select  Distinct DocumentType  from MMM_MST_DOC M with (nolock) LEFT OUTER JOIN MMM_DOC_DTL D with (nolock) on D.tid=M.lasttid  where D.userid = " & UID & " And EID = " & EID & "   UNION SELECT Distinct DocumentType FROM MMM_MST_DOC_ITEM where DOCID  IN(Select   M.tid  from MMM_MST_DOC M with (nolock)  LEFT OUTER JOIN MMM_DOC_DTL D with (nolock) on D.tid=M.lasttid where D.userid = " & UID & " AND EID=" & EID & ")"
            Dim dsDOC As New DataSet()
            Using con1 = New SqlConnection(conStr)
                Using da1 = New SqlDataAdapter(MQuerry, con)
                    da1.Fill(dsDOC)
                End Using
            End Using
            'creating list of all the document on which user has right
            For i As Integer = 0 To dsDOC.Tables(0).Rows.Count - 1
                item = Convert.ToString(dsDOC.Tables(0).Rows(i).Item("DocumentType")).Trim
                CreateDocList(arrayListDoc, item)
            Next
            'generating string for creating Query
            StrDocument = CreateStr(arrayListDoc)
            If StrDocument.Trim <> "" Then
                StrDocList = StrDocument
            End If
            Dim da As New SqlDataAdapter("select FieldID,DropDown,DocumentType,isnull(lookupvalue,'') AS lookupvalue,ISnull(DDllookupvalue,'') As DDllookupvalue FROM MMM_MST_FIELDS where EID= " & EID & " AND DocumentType in (" & StrDocList & ") and FieldType='Drop Down' AND DropDownType='Master valued';select FieldID,displayName,FieldType,FieldMapping,DropDown,DocumentType,DropDownType FROM MMM_MST_FIELDS where EID= " & EID & " and FieldType='DROP DOWN' AND DropDownType='MASTER VALUED'", con)
            Dim ds As New DataSet
            da.Fill(ds)
            Dim lookupValue = "", lookUpDDL = ""
            If ds.Tables(0).Rows.Count > 0 Then
                For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                    If (ds.Tables(0).Rows(i).Item("Dropdown").ToString.Contains("-")) Then
                        Dim arrFormType = ds.Tables(0).Rows(i).Item("DropDown").ToString.Split("-")
                        If arrFormType(0).ToUpper.Trim = "MASTER" Then
                            item = arrFormType(1).Trim
                            CreateDocList(arrayListMas, item)
                            lookupValue = Convert.ToString(ds.Tables(0).Rows(i).Item("lookupvalue"))
                            lookUpDDL = Convert.ToString(ds.Tables(0).Rows(i).Item("ddllookupvalue"))
                            If lookupValue.Trim <> "" And lookupValue.Contains("fld") Then
                                Dim arr = lookupValue.Split(",")
                                For a As Integer = 0 To arr.Length - 1
                                    If arr(a).Trim <> "" And arr(a).Contains("fld") Then
                                        Dim arrb = arr(a).Split("-")
                                        Dim Fld = arrb(1)
                                        Dim onlyFiltered As DataView = ds.Tables(1).DefaultView
                                        onlyFiltered.RowFilter = "DocumentType='" & item & "' AND FieldType = 'DROP DOWN'  AND DropDownType='MASTER VALUED' and FieldMapping='" & Fld & "'"
                                        Dim theFlds As DataTable = onlyFiltered.Table.DefaultView.ToTable()
                                        If theFlds.Rows.Count > 0 Then
                                            Dim arrFormType1 = theFlds.Rows(0).Item("DropDown").ToString.Split("-")
                                            Dim item1 = item
                                            If arrFormType1(0).ToUpper.Trim = "MASTER" Then
                                                item1 = arrFormType1(1).Trim
                                                CreateDocList(arrayListMas, item)
                                            Else
                                                item1 = arrFormType1(1).Trim
                                                CreateDocList(arrayListDoc, item)
                                            End If
                                        End If
                                    End If
                                Next
                            End If
                            If lookUpDDL.Trim <> "" And lookUpDDL.Contains("fld") Then
                                Dim arr = lookUpDDL.Split(",")
                                For a As Integer = 0 To arr.Length - 1
                                    If arr(a).Trim <> "" And arr(a).Contains("fld") Then
                                        Dim arrb = arr(a).Split("-")
                                        Dim Fld = arrb(1)
                                        Dim onlyFiltered As DataView = ds.Tables(1).DefaultView
                                        onlyFiltered.RowFilter = "DocumentType='" & item & "' AND FieldType = 'DROP DOWN'  AND DropDownType='MASTER VALUED' and FieldMapping='" & Fld & "'"
                                        Dim theFlds As DataTable = onlyFiltered.Table.DefaultView.ToTable()
                                        If theFlds.Rows.Count > 0 Then
                                            Dim arrFormType1 = theFlds.Rows(0).Item("DropDown").ToString.Split("-")
                                            Dim item1 = ""
                                            If arrFormType1(0).ToUpper.Trim = "MASTER" Then
                                                item1 = arrFormType1(1).Trim
                                                CreateDocList(arrayListMas, item1)
                                            Else
                                                item1 = arrFormType1(1).Trim
                                                CreateDocList(arrayListDoc, item1)
                                            End If
                                        End If
                                    End If
                                Next
                            End If
                        ElseIf arrFormType(0).ToUpper.Trim = "DOCUMENT" Then
                            item = arrFormType(1).Trim
                            CreateDocList(arrayListDoc, item)
                            lookupValue = Convert.ToString(ds.Tables(0).Rows(i).Item("lookupvalue"))
                            lookUpDDL = Convert.ToString(ds.Tables(0).Rows(i).Item("ddllookupvalue"))
                            If lookupValue.Trim <> "" And lookupValue.Contains("fld") Then
                                Dim arr = lookupValue.Split(",")
                                For a As Integer = 0 To arr.Length - 1
                                    If arr(a).Trim <> "" And arr(a).Contains("fld") Then
                                        Dim arrb = arr(a).Split("-")
                                        Dim Fld = arrb(1)
                                        Dim onlyFiltered As DataView = ds.Tables(1).DefaultView
                                        onlyFiltered.RowFilter = "DocumentType='" & item & "' AND FieldType = 'DROP DOWN'  AND DropDownType='MASTER VALUED' and FieldMapping='" & Fld & "'"
                                        Dim theFlds As DataTable = onlyFiltered.Table.DefaultView.ToTable()
                                        If theFlds.Rows.Count > 0 Then
                                            Dim arrFormType1 = theFlds.Rows(0).Item("DropDown").ToString.Split("-")
                                            Dim item1 = ""
                                            If arrFormType1(0).ToUpper.Trim = "MASTER" Then
                                                item1 = arrFormType1(1).Trim
                                                CreateDocList(arrayListMas, item1)
                                            Else
                                                item1 = arrFormType1(1).Trim
                                                CreateDocList(arrayListDoc, item1)
                                            End If
                                        End If
                                    End If
                                Next
                            End If
                            If lookUpDDL.Trim <> "" And lookUpDDL.Contains("fld") Then
                                Dim arr = lookUpDDL.Split(",")
                                For a As Integer = 0 To arr.Length - 1
                                    If arr(a).Trim <> "" And arr(a).Contains("fld") Then
                                        Dim arrb = arr(a).Split("-")
                                        Dim Fld = arrb(1)
                                        Dim onlyFiltered As DataView = ds.Tables(1).DefaultView
                                        onlyFiltered.RowFilter = "DocumentType='" & item & "' AND FieldType = 'DROP DOWN'  AND DropDownType='MASTER VALUED' and FieldMapping='" & Fld & "'"
                                        Dim theFlds As DataTable = onlyFiltered.Table.DefaultView.ToTable()
                                        If theFlds.Rows.Count > 0 Then
                                            Dim arrFormType1 = theFlds.Rows(0).Item("DropDown").ToString.Split("-")
                                            Dim item1 = item
                                            If arrFormType1(0).ToUpper.Trim = "MASTER" Then
                                                item1 = arrFormType1(1).Trim
                                                CreateDocList(arrayListMas, item1)
                                            Else
                                                item1 = arrFormType1(1).Trim
                                                CreateDocList(arrayListDoc, item1)
                                            End If
                                        End If
                                    End If
                                Next
                            End If
                        End If
                    End If
                Next

            End If
            Dim StrDocument1 As String = CreateStr(arrayListDoc)
            StrMaster = CreateStr(arrayListMas)
            Dim StrFinal As String = ""
            'StrDocList = StrDocument

            If StrMaster.Trim <> "" Then
                If StrFinal.Trim <> "" Then
                    StrFinal = StrFinal & "," & StrMaster
                Else
                    StrFinal = StrMaster
                End If
            End If

            If StrDocument1.Trim <> "" Then
                If StrFinal.Trim <> "" Then
                    StrFinal = StrFinal & "," & StrDocument1
                Else
                    StrFinal = StrDocument1
                End If
            End If

            Return StrFinal

        Catch ex As Exception
            Throw
        Finally
            con.Close()
            con.Dispose()
        End Try
    End Function




    Public Function GetFilteredDoc(EID As Integer, uRole As String) As String
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim Document As New StringBuilder()
        Dim StrDocument = "", StrMaster = "", StrDocList = ""
        Dim arrayListDoc As New List(Of String), item As String = ""
        Dim arrayListMas As New List(Of String)
        Try
            'get name of document and master on which user has right
            Dim MQuerry = "SELECT MID,MenuName,PageLink FROM MMM_MST_MENU WHERE MID NOT IN (SELECT DISTINCT PMENU FROM MMM_MST_MENU WHERE PMENU IS NOT NULL and EID=" & EID & " and ismobile=1) and eid=" & EID & " AND PageLink like 'Documents.Aspx?SC%' and ismobile=1 and roles like '%{" & uRole & "%" & "';SELECT MID,MenuName,PageLink FROM MMM_MST_MENU WHERE MID NOT IN (SELECT DISTINCT PMENU FROM MMM_MST_MENU WHERE PMENU IS NOT NULL and EID=" & EID & ") and eid=" & EID & "  and ismobile=1  AND pagelink   like 'Masters.ASPX?SC%' and roles like '%{" & uRole & "%" & "'"
            Dim dsMenu As New DataSet()
            Using con1 = New SqlConnection(conStr)
                Using da1 = New SqlDataAdapter(MQuerry, con)
                    da1.Fill(dsMenu)
                End Using
            End Using
            'creating list of all the document on which user has right
            If dsMenu.Tables(0).Rows.Count > 0 Then
                For i As Integer = 0 To dsMenu.Tables(0).Rows.Count - 1
                    If (dsMenu.Tables(0).Rows(i).Item("PageLink").ToString.Contains("=")) Then
                        Dim arrPage = dsMenu.Tables(0).Rows(i).Item("PageLink").ToString.Split("=")
                        item = arrPage(1).Trim
                        CreateDocList(arrayListDoc, item)
                    End If
                Next

            End If
            'creating list of all the Master on which user has right
            If dsMenu.Tables(1).Rows.Count > 0 Then
                For i As Integer = 0 To dsMenu.Tables(1).Rows.Count - 1
                    If (dsMenu.Tables(1).Rows(i).Item("PageLink").ToString.Contains("=")) Then
                        Dim arrPage = dsMenu.Tables(1).Rows(i).Item("PageLink").ToString.Split("=")
                        item = arrPage(1).Trim
                        CreateDocList(arrayListMas, item)
                    End If
                Next
            End If
            'generating string for creating Query
            StrDocument = CreateStr(arrayListDoc)
            StrMaster = CreateStr(arrayListMas)
            If StrDocument.Trim <> "" Then
                StrDocList = StrDocument
            End If
            If StrMaster.Trim <> "" Then
                If StrDocList.Trim <> "" Then
                    StrDocList = StrDocList & "," & StrMaster
                Else
                    StrDocList = StrMaster
                End If
            End If
            If String.IsNullOrEmpty(StrDocList.Trim()) Then
                StrDocList = "'No Any Document Found'"
            End If

            Dim da As New SqlDataAdapter("select FieldID,DropDown,DocumentType,isnull(lookupvalue,'') AS lookupvalue,ISnull(DDllookupvalue,'') As DDllookupvalue FROM MMM_MST_FIELDS where EID= " & EID & " AND DocumentType in (" & StrDocList & ") and FieldType='Drop Down' AND DropDownType='Master valued';select FieldID,displayName,FieldType,FieldMapping,DropDown,DocumentType,DropDownType FROM MMM_MST_FIELDS where EID= " & EID & " and FieldType='DROP DOWN' AND DropDownType='MASTER VALUED'", con)
            Dim ds As New DataSet
            da.Fill(ds)
            Dim lookupValue = "", lookUpDDL = ""
            If ds.Tables(0).Rows.Count > 0 Then
                For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                    If (ds.Tables(0).Rows(i).Item("Dropdown").ToString.Contains("-")) Then
                        Dim arrFormType = ds.Tables(0).Rows(i).Item("DropDown").ToString.Split("-")
                        If arrFormType(0).ToUpper.Trim = "MASTER" Then
                            item = arrFormType(1).Trim
                            CreateDocList(arrayListMas, item)
                            lookupValue = Convert.ToString(ds.Tables(0).Rows(i).Item("lookupvalue"))
                            lookUpDDL = Convert.ToString(ds.Tables(0).Rows(i).Item("ddllookupvalue"))
                            If lookupValue.Trim <> "" And lookupValue.Contains("fld") Then
                                Dim arr = lookupValue.Split(",")
                                For a As Integer = 0 To arr.Length - 1
                                    If arr(a).Trim <> "" And arr(a).Contains("fld") Then
                                        Dim arrb = arr(a).Split("-")
                                        Dim Fld = arrb(1)
                                        Dim onlyFiltered As DataView = ds.Tables(1).DefaultView
                                        onlyFiltered.RowFilter = "DocumentType='" & item & "' AND FieldType = 'DROP DOWN'  AND DropDownType='MASTER VALUED' and FieldMapping='" & Fld & "'"
                                        Dim theFlds As DataTable = onlyFiltered.Table.DefaultView.ToTable()
                                        If theFlds.Rows.Count > 0 Then
                                            Dim arrFormType1 = theFlds.Rows(0).Item("DropDown").ToString.Split("-")
                                            Dim item1 = item
                                            If arrFormType1(0).ToUpper.Trim = "MASTER" Then
                                                item1 = arrFormType1(1).Trim
                                                CreateDocList(arrayListMas, item)
                                            Else
                                                item1 = arrFormType1(1).Trim
                                                CreateDocList(arrayListDoc, item)
                                            End If
                                        End If
                                    End If
                                Next
                            End If
                            If lookUpDDL.Trim <> "" And lookUpDDL.Contains("fld") Then
                                Dim arr = lookUpDDL.Split(",")
                                For a As Integer = 0 To arr.Length - 1
                                    If arr(a).Trim <> "" And arr(a).Contains("fld") Then
                                        Dim arrb = arr(a).Split("-")
                                        Dim Fld = arrb(1)
                                        Dim onlyFiltered As DataView = ds.Tables(1).DefaultView
                                        onlyFiltered.RowFilter = "DocumentType='" & item & "' AND FieldType = 'DROP DOWN'  AND DropDownType='MASTER VALUED' and FieldMapping='" & Fld & "'"
                                        Dim theFlds As DataTable = onlyFiltered.Table.DefaultView.ToTable()
                                        If theFlds.Rows.Count > 0 Then
                                            Dim arrFormType1 = theFlds.Rows(0).Item("DropDown").ToString.Split("-")
                                            Dim item1 = ""
                                            If arrFormType1(0).ToUpper.Trim = "MASTER" Then
                                                item1 = arrFormType1(1).Trim
                                                CreateDocList(arrayListMas, item1)
                                            Else
                                                item1 = arrFormType1(1).Trim
                                                CreateDocList(arrayListDoc, item1)
                                            End If
                                        End If
                                    End If
                                Next
                            End If
                        ElseIf arrFormType(0).ToUpper.Trim = "DOCUMENT" Then
                            item = arrFormType(1).Trim
                            CreateDocList(arrayListDoc, item)
                            lookupValue = Convert.ToString(ds.Tables(0).Rows(i).Item("lookupvalue"))
                            lookUpDDL = Convert.ToString(ds.Tables(0).Rows(i).Item("ddllookupvalue"))
                            If lookupValue.Trim <> "" And lookupValue.Contains("fld") Then
                                Dim arr = lookupValue.Split(",")
                                For a As Integer = 0 To arr.Length - 1
                                    If arr(a).Trim <> "" And arr(a).Contains("fld") Then
                                        Dim arrb = arr(a).Split("-")
                                        Dim Fld = arrb(1)
                                        Dim onlyFiltered As DataView = ds.Tables(1).DefaultView
                                        onlyFiltered.RowFilter = "DocumentType='" & item & "' AND FieldType = 'DROP DOWN'  AND DropDownType='MASTER VALUED' and FieldMapping='" & Fld & "'"
                                        Dim theFlds As DataTable = onlyFiltered.Table.DefaultView.ToTable()
                                        If theFlds.Rows.Count > 0 Then
                                            Dim arrFormType1 = theFlds.Rows(0).Item("DropDown").ToString.Split("-")
                                            Dim item1 = ""
                                            If arrFormType1(0).ToUpper.Trim = "MASTER" Then
                                                item1 = arrFormType1(1).Trim
                                                CreateDocList(arrayListMas, item1)
                                            Else
                                                item1 = arrFormType1(1).Trim
                                                CreateDocList(arrayListDoc, item1)
                                            End If
                                        End If
                                    End If
                                Next
                            End If
                            If lookUpDDL.Trim <> "" And lookUpDDL.Contains("fld") Then
                                Dim arr = lookUpDDL.Split(",")
                                For a As Integer = 0 To arr.Length - 1
                                    If arr(a).Trim <> "" And arr(a).Contains("fld") Then
                                        Dim arrb = arr(a).Split("-")
                                        Dim Fld = arrb(1)
                                        Dim onlyFiltered As DataView = ds.Tables(1).DefaultView
                                        onlyFiltered.RowFilter = "DocumentType='" & item & "' AND FieldType = 'DROP DOWN'  AND DropDownType='MASTER VALUED' and FieldMapping='" & Fld & "'"
                                        Dim theFlds As DataTable = onlyFiltered.Table.DefaultView.ToTable()
                                        If theFlds.Rows.Count > 0 Then
                                            Dim arrFormType1 = theFlds.Rows(0).Item("DropDown").ToString.Split("-")
                                            Dim item1 = item
                                            If arrFormType1(0).ToUpper.Trim = "MASTER" Then
                                                item1 = arrFormType1(1).Trim
                                                CreateDocList(arrayListMas, item1)
                                            Else
                                                item1 = arrFormType1(1).Trim
                                                CreateDocList(arrayListDoc, item1)
                                            End If
                                        End If
                                    End If
                                Next
                            End If
                        End If
                    End If
                Next

            End If
            StrDocument = CreateStr(arrayListDoc)
            StrMaster = CreateStr(arrayListMas)

            StrDocList = StrDocument

            If StrMaster.Trim <> "" Then
                If StrDocList.Trim <> "" Then
                    StrDocList = StrDocList & "," & StrMaster
                Else
                    StrDocList = StrMaster
                End If
            End If
            'For handling user documentType
            If String.IsNullOrEmpty(StrDocList.Trim()) Then
                StrDocList = "'user'"
            Else
                StrDocList = StrDocList + ",'user'"
            End If
            Return StrDocList

        Catch ex As Exception
            Throw
        Finally
            con.Close()
            con.Dispose()
        End Try
    End Function
    Private Sub CreateDocList(ByRef arrayList As List(Of String), item As String)
        Dim IsExists As Boolean = False
        For i As Integer = 0 To arrayList.Count - 1
            If arrayList(i).ToString.ToUpper.Trim = item.ToString.ToUpper.Trim Then
                IsExists = True
                Exit For
            End If
        Next
        If IsExists = False Then
            arrayList.Add(item)
        End If
    End Sub

    Private Function CreateStr(arrayList As List(Of String)) As String
        Dim ret = ""
        Dim Sb As New StringBuilder()
        For i As Integer = 0 To arrayList.Count - 1
            Sb.Append(",").Append("'").Append(arrayList(i)).Append("'")
        Next
        If Sb.ToString.Trim <> "" Then
            ret = Sb.ToString.Substring(1, Sb.ToString().Length - 1)
        End If

        Return ret
    End Function

    Function SyncUser(Key As String, EID As String, UID As Integer, IMINum As String, ST As String) As String Implements IBPMMobileSyncAPI.SyncUser

        Dim result = ""
        Dim objDev As New DeviceInfo()
        Dim ret = "NEW"
        objDev.IMINumber = IMINum
        Dim dsD = New DataSet()
        Dim IsValidrequest = False
        dsD = objDev.ValidateDevice(EID, UID)
        If dsD.Tables(0).Rows.Count > 0 Then
            IsValidrequest = True
            If Not ST.ToUpper() = "COMPLETE" Then
                ret = dsD.Tables(0).Rows(0).Item("LastUpdatedOn").ToString()
            Else
                ret = "1900-01-01 00:00:00.000"
            End If
        End If
        Try
            If BPMKEY = Key And IsValidrequest = True Then
                Dim DsUser As New DataSet()
                'For avoiding sqlinjection 
                EID = EID.Replace("'", String.Empty).Replace("--", "")
                Using con = New SqlConnection(conStr)
                    Using da = New SqlDataAdapter("select * FROM MMM_MST_USER WHERE EID= " & EID & " AND  lastupdate >'" & ret & "' AND UserRole<>'SU'", con)
                        con.Open()
                        da.Fill(DsUser)
                    End Using
                End Using
                Dim SBData As New StringBuilder()
                If DsUser.Tables(0).Rows.Count > 0 Then
                    For i As Integer = 0 To DsUser.Tables(0).Rows.Count - 1
                        SBData.Append("::")

                        SBData.Append(DsUser.Tables(0).Rows(i).Item("UID").ToString() & "|")
                        SBData.Append(DsUser.Tables(0).Rows(i).Item("UserName").ToString() & "|")
                        SBData.Append(DsUser.Tables(0).Rows(i).Item("emailID").ToString() & "|")
                        SBData.Append(DsUser.Tables(0).Rows(i).Item("UserRole").ToString() & "|")
                        SBData.Append(DsUser.Tables(0).Rows(i).Item("UserID").ToString() & "|")

                        SBData.Append(DsUser.Tables(0).Rows(i).Item("fld1").ToString() & "|")
                        SBData.Append(DsUser.Tables(0).Rows(i).Item("fld2").ToString() & "|")
                        SBData.Append(DsUser.Tables(0).Rows(i).Item("fld3").ToString() & "|")
                        SBData.Append(DsUser.Tables(0).Rows(i).Item("fld4").ToString() & "|")
                        SBData.Append(DsUser.Tables(0).Rows(i).Item("fld5").ToString() & "|")
                        SBData.Append(DsUser.Tables(0).Rows(i).Item("fld6").ToString() & "|")
                        SBData.Append(DsUser.Tables(0).Rows(i).Item("fld7").ToString() & "|")
                        SBData.Append(DsUser.Tables(0).Rows(i).Item("fld8").ToString() & "|")
                        SBData.Append(DsUser.Tables(0).Rows(i).Item("fld9").ToString() & "|")
                        SBData.Append(DsUser.Tables(0).Rows(i).Item("fld10").ToString() & "|")
                        SBData.Append(DsUser.Tables(0).Rows(i).Item("fld11").ToString() & "|")
                        SBData.Append(DsUser.Tables(0).Rows(i).Item("fld12").ToString() & "|")
                        SBData.Append(DsUser.Tables(0).Rows(i).Item("fld13").ToString() & "|")
                        SBData.Append(DsUser.Tables(0).Rows(i).Item("fld14").ToString() & "|")
                        SBData.Append(DsUser.Tables(0).Rows(i).Item("fld15").ToString() & "|")
                        SBData.Append(DsUser.Tables(0).Rows(i).Item("fld16").ToString() & "|")
                        SBData.Append(DsUser.Tables(0).Rows(i).Item("fld17").ToString() & "|")
                        SBData.Append(DsUser.Tables(0).Rows(i).Item("fld18").ToString() & "|")
                        SBData.Append(DsUser.Tables(0).Rows(i).Item("fld19").ToString() & "|")
                        SBData.Append(DsUser.Tables(0).Rows(i).Item("fld20").ToString() & "|")
                        SBData.Append(DsUser.Tables(0).Rows(i).Item("fld21").ToString() & "|")
                        SBData.Append(DsUser.Tables(0).Rows(i).Item("fld22").ToString() & "|")
                        SBData.Append(DsUser.Tables(0).Rows(i).Item("fld23").ToString() & "|")
                        SBData.Append(DsUser.Tables(0).Rows(i).Item("fld24").ToString() & "|")
                        SBData.Append(DsUser.Tables(0).Rows(i).Item("fld25").ToString() & "|")
                        SBData.Append(DsUser.Tables(0).Rows(i).Item("fld26").ToString() & "|")
                        SBData.Append(DsUser.Tables(0).Rows(i).Item("fld27").ToString() & "|")
                        SBData.Append(DsUser.Tables(0).Rows(i).Item("fld28").ToString() & "|")
                        SBData.Append(DsUser.Tables(0).Rows(i).Item("fld29").ToString() & "|")
                        SBData.Append(DsUser.Tables(0).Rows(i).Item("fld30").ToString() & "|")
                        SBData.Append(DsUser.Tables(0).Rows(i).Item("fld31").ToString() & "|")
                        SBData.Append(DsUser.Tables(0).Rows(i).Item("fld32").ToString() & "|")
                        SBData.Append(DsUser.Tables(0).Rows(i).Item("fld33").ToString() & "|")
                        SBData.Append(DsUser.Tables(0).Rows(i).Item("fld34").ToString() & "|")
                        SBData.Append(DsUser.Tables(0).Rows(i).Item("fld35").ToString() & "|")
                        SBData.Append(DsUser.Tables(0).Rows(i).Item("fld36").ToString() & "|")
                        SBData.Append(DsUser.Tables(0).Rows(i).Item("fld37").ToString() & "|")
                        SBData.Append(DsUser.Tables(0).Rows(i).Item("fld38").ToString() & "|")
                        SBData.Append(DsUser.Tables(0).Rows(i).Item("fld39").ToString() & "|")
                        SBData.Append(DsUser.Tables(0).Rows(i).Item("fld40").ToString() & "|")
                        SBData.Append(DsUser.Tables(0).Rows(i).Item("fld41").ToString() & "|")
                        SBData.Append(DsUser.Tables(0).Rows(i).Item("fld42").ToString() & "|")
                        SBData.Append(DsUser.Tables(0).Rows(i).Item("fld43").ToString() & "|")
                        SBData.Append(DsUser.Tables(0).Rows(i).Item("fld44").ToString() & "|")
                        SBData.Append(DsUser.Tables(0).Rows(i).Item("fld45").ToString() & "|")
                        SBData.Append(DsUser.Tables(0).Rows(i).Item("fld46").ToString() & "|")
                        SBData.Append(DsUser.Tables(0).Rows(i).Item("fld47").ToString() & "|")
                        SBData.Append(DsUser.Tables(0).Rows(i).Item("fld48").ToString() & "|")
                        SBData.Append(DsUser.Tables(0).Rows(i).Item("fld49").ToString() & "|")
                        SBData.Append(DsUser.Tables(0).Rows(i).Item("fld50").ToString() & "|")
                        SBData.Append(DsUser.Tables(0).Rows(i).Item("fld51").ToString() & "|")
                        SBData.Append(DsUser.Tables(0).Rows(i).Item("fld52").ToString() & "|")
                        SBData.Append(DsUser.Tables(0).Rows(i).Item("fld53").ToString() & "|")
                        SBData.Append(DsUser.Tables(0).Rows(i).Item("fld54").ToString() & "|")
                        SBData.Append(DsUser.Tables(0).Rows(i).Item("fld55").ToString() & "|")
                        SBData.Append(DsUser.Tables(0).Rows(i).Item("fld56").ToString() & "|")
                        SBData.Append(DsUser.Tables(0).Rows(i).Item("fld57").ToString() & "|")
                        SBData.Append(DsUser.Tables(0).Rows(i).Item("fld58").ToString() & "|")
                        SBData.Append(DsUser.Tables(0).Rows(i).Item("fld59").ToString() & "|")
                        SBData.Append(DsUser.Tables(0).Rows(i).Item("fld60").ToString() & "|")
                        SBData.Append(DsUser.Tables(0).Rows(i).Item("fld61").ToString() & "|")
                        SBData.Append(DsUser.Tables(0).Rows(i).Item("fld62").ToString() & "|")
                        SBData.Append(DsUser.Tables(0).Rows(i).Item("fld63").ToString() & "|")
                        SBData.Append(DsUser.Tables(0).Rows(i).Item("fld64").ToString() & "|")
                        SBData.Append(DsUser.Tables(0).Rows(i).Item("fld65").ToString() & "|")
                        SBData.Append(DsUser.Tables(0).Rows(i).Item("fld66").ToString() & "|")
                        SBData.Append(DsUser.Tables(0).Rows(i).Item("fld67").ToString() & "|")
                        SBData.Append(DsUser.Tables(0).Rows(i).Item("fld68").ToString() & "|")
                        SBData.Append(DsUser.Tables(0).Rows(i).Item("fld69").ToString() & "|")
                        SBData.Append(DsUser.Tables(0).Rows(i).Item("fld70").ToString())
                    Next
                    result = SBData.ToString()
                End If
            Else
                result = "Sorry!!! Your Authantication failed."
            End If
        Catch ex As Exception
            Return "RTO"
        End Try
        Return result
    End Function

    Public Function GetMenu(ByVal EID As Integer, userrole As String, Optional LastUpdate As String = "1900-01-01 12:25:25.010") As DataSet
        Dim dsMenu As New DataSet
        Dim con As SqlConnection = Nothing
        Dim da As SqlDataAdapter = Nothing
        Try
            con = New SqlConnection(conStr)
            da = New SqlDataAdapter("getMenuMob", con)
            da.SelectCommand.CommandType = CommandType.StoredProcedure
            da.SelectCommand.Parameters.AddWithValue("@EID", EID)
            da.SelectCommand.Parameters.AddWithValue("@userRole", userrole)
            da.SelectCommand.Parameters.AddWithValue("@LastUpdate", LastUpdate)
            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            da.Fill(dsMenu)
            Return dsMenu
        Catch ex As Exception
            Throw
        Finally
            con.Close()
            con.Dispose()
            da.Dispose()
        End Try
    End Function

    Public Function UserDataFilter_PreRole(ByVal ddocumenttype As String, ByVal TableName As String, ByVal EID As String, ByVal URole As String, ByVal UID As String) As String

        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim da As New SqlDataAdapter("", con)
        Dim ds As New DataSet
        Dim fldmapping As String = ""
        Dim fldid As String = ""
        da.SelectCommand.CommandText = "select * from mmm_prerole_datafilter where eid=" & EID & " and documenttype='" & ddocumenttype & "' and rolename='" & URole & "'"
        If con.State <> ConnectionState.Open Then
            con.Open()
        End If
        Try
            Dim DT As New DataTable
            da.Fill(DT)
            If DT.Rows.Count <> 0 Then
                fldmapping = DT.Rows(0).Item("fldmapping").ToString
                If fldmapping.Length > 2 Then
                    da.SelectCommand.CommandText = "SELECT SUBSTRING((SELECT ',' + CONVERT(NVARCHAR,TID)  FROM " & TableName & " where EID=" & EID & " and " & fldmapping & "='" & UID & "' FOR XML PATH('')),2,10000) AS CSV"
                End If
                da.Fill(ds, "FILTER")
                If ds.Tables("FILTER").Rows.Count = 0 Then
                    fldid = ""
                ElseIf ds.Tables("FILTER").Rows.Count = 1 Then
                    fldid = ds.Tables("FILTER").Rows(0).Item(0).ToString()
                End If
                If fldid = "" Then
                    fldid = "0"
                End If
            End If

            Return fldid
        Catch ex As Exception
            Throw
        Finally
            da.Dispose()
            ds.Dispose()
            con.Dispose()
        End Try
    End Function
    Function GetLastMobileSync(Key As String, UID As String, EID As String, URole As String, IMINum As String, ST As String) As XElement Implements IBPMMobileSyncAPI.GetLastMobileSync
        Dim result As String = ""
        Dim ret = ""
        Dim con As SqlConnection = Nothing
        Dim da As SqlDataAdapter = Nothing
        Dim SBData As New StringBuilder("<LastMobileSyncSetting>")
        Dim Query As String = ""
        Dim doc As New XDocument()
        Dim IsValidrequest = False
        Try
            Dim objDev As New DeviceInfo()
            con = New SqlConnection(conStr)
            objDev.IMINumber = IMINum
            Dim dsD = New DataSet()
            dsD = objDev.ValidateDevice(EID, UID)
            If dsD.Tables(0).Rows.Count > 0 Then
                IsValidrequest = True
                If Not ST.ToUpper() = "COMPLETE" Then
                    ret = dsD.Tables(0).Rows(0).Item("LastUpdatedOn").ToString()
                Else
                    ret = "1900-01-01 12:25:25.010"
                End If
            End If
            If (Key = BPMKEY) And IsValidrequest = True Then
                Dim DsLastSync As New DataSet()
                Query = "select sync.* from mmm_mst_lastmobilesync sync join mmm_mst_role roles on sync.RoleId=roles.RoleId where sync.eid=" & EID & " and roles.RoleName='" & URole & "' and sync.lastupdate >= '" & ret & "'"
                da = New SqlDataAdapter(Query, con)
                da.Fill(DsLastSync)
                If DsLastSync.Tables.Count > 0 Then
                    If DsLastSync.Tables(0).Rows.Count > 0 Then
                        For i As Integer = 0 To DsLastSync.Tables(0).Rows.Count - 1
                            SBData.Append("<Rows>")
                            SBData.Append("<SyncId>").Append(Convert.ToString(DsLastSync.Tables(0).Rows(i).Item("SyncId"))).Append("</SyncId>")
                            SBData.Append("<DocumentType>").Append(Convert.ToString(DsLastSync.Tables(0).Rows(i).Item("DocumentType"))).Append("</DocumentType>")
                            SBData.Append("<EID>").Append(Convert.ToString(DsLastSync.Tables(0).Rows(i).Item("EID"))).Append("</EID>")
                            SBData.Append("<NoOfDays>").Append(Convert.ToString(DsLastSync.Tables(0).Rows(i).Item("NoOfDays"))).Append("</NoOfDays>")
                            SBData.Append("<LastUpdate>").Append(Convert.ToString(DsLastSync.Tables(0).Rows(i).Item("LastUpdate"))).Append("</LastUpdate>")
                            SBData.Append("<IsActive>").Append(Convert.ToString(DsLastSync.Tables(0).Rows(i).Item("IsActive"))).Append("</IsActive>")
                            SBData.Append("<RoleId>").Append(Convert.ToString(DsLastSync.Tables(0).Rows(i).Item("RoleId"))).Append("</RoleId>")
                            SBData.Append("<Field>").Append(Convert.ToString(DsLastSync.Tables(0).Rows(i).Item("Field"))).Append("</Field>")
                            SBData.Append("</Rows>")
                        Next
                        result = SBData.ToString()
                    End If
                End If
            Else
                result = "Sorry!!! Your Authantication failed."
            End If

        Catch ex As Exception
            Throw New FaultException("RTO")
        Finally
            If Not con Is Nothing Then
                con.Close()
                con.Dispose()
            End If
            If Not da Is Nothing Then
                da.Dispose()
            End If
        End Try
        SBData.Append("</LastMobileSyncSetting>")
        Dim StrXml As String = SBData.ToString.Replace("&", "&amp;")
        doc = XDocument.Parse(StrXml)
        Return doc.Root
    End Function

    Function GetDataOFAllForm(Key As String, EID As String, UID As Integer, URole As String, IMINum As String, ST As String) As List(Of FormData) Implements IBPMMobileSyncAPI.GetDataOFAllForm

        Dim lstFlds As New List(Of FormData)
        Dim dsfld As New DataSet()
        Dim objFD As FormData
        Dim con As SqlConnection = Nothing
        Dim da As SqlDataAdapter = Nothing
        Dim objDev As New DeviceInfo()
        Dim ret = ""
        Dim Query = ""
        Dim Query1 = ""
        Dim Query2 = ""
        Dim Query3 = ""
        Try
            'Dim BPMKEY As String = ConfigurationManager.AppSettings("BPMKEY").ToString()
            Dim IsValidrequest = False
            Dim dsD As New DataSet()
            objDev.IMINumber = IMINum
            dsD = objDev.ValidateDevice(EID, UID)
            If dsD.Tables(0).Rows.Count > 0 Then
                IsValidrequest = True
                If Not ST.ToUpper() = "COMPLETE" Then
                    ret = dsD.Tables(0).Rows(0).Item("LastUpdatedOn").ToString()
                Else
                    ret = "NEW"
                End If
            End If
            con = New SqlConnection(conStr)
            If BPMKEY = Key And IsValidrequest = True Then
                Dim Doclist = GetFilteredDoc(EID, URole).ToString
                If Doclist Is Nothing Or String.IsNullOrEmpty(Doclist.ToString.Trim) Then
                    Doclist = "'No Any Right founds for the supplyed role'"
                End If
                Query = "SELECT FF.*,F.*  FROM MMM_MST_FIELDS FF left outer JOIN MMM_MST_FORMS F on F.FormName = FF.DocumentType and F.EID = FF.EID   where FF.DocumentType  in(" & Doclist & ") and F.EID=" & EID
                Query1 = " union all SELECT FF.*,F.*  FROM MMM_MST_FIELDS FF left outer JOIN MMM_MST_FORMS F on F.FormName = FF.DocumentType and F.EID = FF.EID   where F.EID= " & EID & " AND FF.EID=" & EID & " AND  FF.DocumentType  in(select FORMNAME FROM MMM_MST_Forms WHERE EID= " & EID & " AND FormSource='DETAIL FORM' AND FormName in (SELECT DROPDOWN FROM MMM_MST_FIELDS WHERE EID=" & EID & " AND FieldType='CHILD ITEM' AND IsActive=1 AND DocumentType  in(" & Doclist & ") ))"
                Query2 = " union all SELECT FF.*,F.*  FROM MMM_MST_FIELDS FF left outer JOIN MMM_MST_FORMS F on F.FormName = FF.DocumentType and F.EID = FF.EID   where  FF.EID=" & EID & " AND F.EID= " & EID & " AND  FF.DocumentType  in(select FORMNAME FROM MMM_MST_Forms WHERE EID=" & EID & " AND FormSource='DETAIL FORM' AND EventName in (" & Doclist & "))"
                Query3 = " union all SELECT FF.*,F.*  FROM MMM_MST_FIELDS FF left outer JOIN MMM_MST_FORMS F on F.FormName = FF.DocumentType and F.EID = FF.EID   where   FF.EID=" & EID & " AND F.EID= " & EID & " AND  FF.DocumentType  in(select FORMNAME FROM MMM_MST_Forms WHERE EID=" & EID & " AND FormSource='ACTION DRIVEN')"
                If ret.ToUpper() <> "NEW" Then
                    Query = Query & " AND " & " FF.lastupdate >= " & "'" & ret & "'"
                    Query1 = Query1 & " AND " & " FF.lastupdate >= " & "'" & ret & "'"
                    Query2 = Query2 & " AND " & " FF.lastupdate >= " & "'" & ret & "'"
                    Query3 = Query3 & " AND " & " FF.lastupdate >= " & "'" & ret & "'"
                End If
                Dim StrQUery = Query & Query1 & Query2 & Query3
                da = New SqlDataAdapter(StrQUery, con)
                If con.State <> ConnectionState.Open Then
                    con.Open()
                End If
                da.Fill(dsfld)
                If dsfld.Tables(0).Rows.Count > 0 Then
                    For i As Integer = 0 To dsfld.Tables(0).Rows.Count - 1
                        objFD = New FormData()
                        objFD.FieldID = dsfld.Tables(0).Rows(i).Item("FieldID").ToString()
                        objFD.EID = dsfld.Tables(0).Rows(i).Item("EID").ToString()
                        objFD.displayName = dsfld.Tables(0).Rows(i).Item("displayName").ToString()
                        objFD.FieldType = dsfld.Tables(0).Rows(i).Item("FieldType").ToString()
                        objFD.displayOrder = dsfld.Tables(0).Rows(i).Item("displayOrder").ToString()
                        objFD.FieldMapping = dsfld.Tables(0).Rows(i).Item("FieldMapping").ToString()
                        objFD.isRequired = dsfld.Tables(0).Rows(i).Item("isRequired").ToString()
                        objFD.isActive = dsfld.Tables(0).Rows(i).Item("isActive").ToString()
                        objFD.dropdown = dsfld.Tables(0).Rows(i).Item("dropdown").ToString()
                        objFD.DocumentType = dsfld.Tables(0).Rows(i).Item("DocumentType").ToString()
                        objFD.DBTableName = dsfld.Tables(0).Rows(i).Item("DBTableName").ToString()
                        objFD.FieldNature = dsfld.Tables(0).Rows(i).Item("FieldNature").ToString()
                        objFD.DropDownType = dsfld.Tables(0).Rows(i).Item("DropDownType").ToString()
                        objFD.isEditable = dsfld.Tables(0).Rows(i).Item("isEditable").ToString()
                        objFD.datatype = dsfld.Tables(0).Rows(i).Item("datatype").ToString()
                        objFD.MinLen = dsfld.Tables(0).Rows(i).Item("MinLen").ToString()
                        objFD.MaxLen = dsfld.Tables(0).Rows(i).Item("MaxLen").ToString()
                        objFD.isWorkFlow = dsfld.Tables(0).Rows(i).Item("isWorkFlow").ToString()
                        objFD.lookupvalue = dsfld.Tables(0).Rows(i).Item("lookupvalue").ToString()
                        objFD.Cal_Fields = dsfld.Tables(0).Rows(i).Item("Cal_Fields").ToString()
                        objFD.AutoFilter = dsfld.Tables(0).Rows(i).Item("AutoFilter").ToString()
                        objFD.KC_VALUE = dsfld.Tables(0).Rows(i).Item("KC_VALUE").ToString()
                        objFD.KC_LOGIC = dsfld.Tables(0).Rows(i).Item("KC_LOGIC").ToString()
                        objFD.isunique = dsfld.Tables(0).Rows(i).Item("isunique").ToString()
                        objFD.ShowOnDocDetail = dsfld.Tables(0).Rows(i).Item("ShowOnDocDetail").ToString()
                        objFD.IsSession = dsfld.Tables(0).Rows(i).Item("IsSession").ToString()
                        objFD.isMail = dsfld.Tables(0).Rows(i).Item("isMail").ToString()
                        objFD.KC_STATUS = dsfld.Tables(0).Rows(i).Item("KC_STATUS").ToString()
                        objFD.ShowOnAmendment = dsfld.Tables(0).Rows(i).Item("ShowOnAmendment").ToString()
                        objFD.isSearch = dsfld.Tables(0).Rows(i).Item("isSearch").ToString()
                        objFD.isImieno = dsfld.Tables(0).Rows(i).Item("isImieno").ToString()
                        objFD.isPhoneNo = dsfld.Tables(0).Rows(i).Item("isPhoneNo").ToString()
                        objFD.iseditonAmend = dsfld.Tables(0).Rows(i).Item("iseditonAmend").ToString()
                        objFD.InLineEditing = dsfld.Tables(0).Rows(i).Item("InLineEditing").ToString()
                        objFD.dependentON = dsfld.Tables(0).Rows(i).Item("dependentON").ToString()
                        objFD.Invisible = dsfld.Tables(0).Rows(i).Item("Invisible").ToString()
                        objFD.Cal_Text = dsfld.Tables(0).Rows(i).Item("cal_text").ToString()
                        objFD.DefaultValue = dsfld.Tables(0).Rows(i).Item("defaultfieldval").ToString()
                        objFD.FixedValuedFilter = dsfld.Tables(0).Rows(i).Item("initialFilter").ToString()
                        objFD.ShowOnDashBoard = dsfld.Tables(0).Rows(i).Item("showOnDashBoard").ToString()
                        objFD.DDllookupvalue = dsfld.Tables(0).Rows(i).Item("DDllookupvalue").ToString()
                        objFD.externallookupformobile = Convert.ToString(dsfld.Tables(0).Rows(i).Item("externallookupformobile"))
                        'DDllookupvalue
                        objFD.IsTotal = dsfld.Tables(0).Rows(i).Item("IsTotal").ToString()
                        objFD.iscatchment = dsfld.Tables(0).Rows(i).Item("iscatchment").ToString()
                        objFD.alloweditonedit = dsfld.Tables(0).Rows(i).Item("alloweditonedit").ToString()
                        objFD.ChildTemp_column = dsfld.Tables(0).Rows(i).Item("ChildTemp_column").ToString()
                        objFD.ddlval = dsfld.Tables(0).Rows(i).Item("ddlval").ToString()
                        objFD.DDLlookupValueSource = dsfld.Tables(0).Rows(i).Item("DDLlookupValueSource").ToString()
                        objFD.geofencetype = dsfld.Tables(0).Rows(i).Item("geofencetype").ToString()

                        objFD.MultiLookUpVal = dsfld.Tables(0).Rows(i).Item("MultiLookUpVal").ToString()
                        objFD.Child_Specific_text = dsfld.Tables(0).Rows(i).Item("Child_Specific_text").ToString()
                        objFD.IsCardNo = dsfld.Tables(0).Rows(i).Item("IsCardNo").ToString()
                        objFD.ddlMultilookupval = dsfld.Tables(0).Rows(i).Item("ddlMultilookupval").ToString()
                        objFD.isgeofence_filter = dsfld.Tables(0).Rows(i).Item("isgeofence_filter").ToString()

                        objFD.geofensefiltersourcefield = Convert.ToString(dsfld.Tables(0).Rows(i).Item("geofensefiltersourcefield"))
                        objFD.Show_on_CRM = Convert.ToString(dsfld.Tables(0).Rows(i).Item("Show_on_CRM"))
                        objFD.Edit_on_CRM = Convert.ToString(dsfld.Tables(0).Rows(i).Item("Edit_on_CRM"))
                        lstFlds.Add(objFD)
                    Next
                End If

            End If
        Catch ex As Exception
            Throw New FaultException("Error occured at server")
        Finally
            If Not con Is Nothing Then
                con.Close()
                con.Dispose()
            End If
            If Not da Is Nothing Then
                da.Dispose()
            End If
        End Try
        'Dim str = WebOperationContext.Current.OutgoingResponse.ToString()

        Return lstFlds
    End Function

    Function GetDataOFAllFormForApproval(Key As String, EID As String, UID As Integer, URole As String, IMINum As String, ST As String) As List(Of FormData) Implements IBPMMobileSyncAPI.GetDataOFAllFormForApproval

        Dim lstFlds As New List(Of FormData)
        Dim dsfld As New DataSet()
        Dim objFD As FormData
        Dim con As SqlConnection = Nothing
        Dim da As SqlDataAdapter = Nothing
        Dim objDev As New DeviceInfo()
        Dim ret = ""
        Dim Query = ""
        Dim Query1 = ""
        Dim Query2 = ""
        Dim Query3 = ""
        Try
            'Dim BPMKEY As String = ConfigurationManager.AppSettings("BPMKEY").ToString()
            Dim IsValidrequest = False
            Dim dsD As New DataSet()
            objDev.IMINumber = IMINum
            dsD = objDev.ValidateDevice(EID, UID)
            If dsD.Tables(0).Rows.Count > 0 Then
                IsValidrequest = True
                If Not ST.ToUpper() = "COMPLETE" Then
                    ret = dsD.Tables(0).Rows(0).Item("LastUpdatedOn").ToString()
                Else
                    ret = "NEW"
                End If
            End If
            con = New SqlConnection(conStr)
            If BPMKEY = Key And IsValidrequest = True Then
                Dim Doclist = GetFilteredDocA(EID, UID).ToString
                If Doclist Is Nothing Or String.IsNullOrEmpty(Doclist.ToString.Trim) Then
                    Doclist = "'No Any Right founds for the supplyed role'"
                End If
                Query = "SELECT FF.*,F.*  FROM MMM_MST_FIELDS FF left outer JOIN MMM_MST_FORMS F on F.FormName = FF.DocumentType and F.EID = FF.EID   where (FF.isactive=1 or FieldType='Auto Number') AND FF.DocumentType  in(" & Doclist & ") and F.EID=" & EID
                Query1 = " union all SELECT FF.*,F.*  FROM MMM_MST_FIELDS FF left outer JOIN MMM_MST_FORMS F on F.FormName = FF.DocumentType and F.EID = FF.EID   where (FF.isactive=1 or FieldType='Auto Number') AND F.EID= " & EID & " AND FF.EID=" & EID & " AND  FF.DocumentType  in(select FORMNAME FROM MMM_MST_Forms WHERE EID= " & EID & " AND FormSource='DETAIL FORM' AND FormName in (SELECT DROPDOWN FROM MMM_MST_FIELDS WHERE EID=" & EID & " AND FieldType='CHILD ITEM' AND IsActive=1 AND DocumentType  in(" & Doclist & ") ))"
                Query2 = " union all SELECT FF.*,F.*  FROM MMM_MST_FIELDS FF left outer JOIN MMM_MST_FORMS F on F.FormName = FF.DocumentType and F.EID = FF.EID   where (FF.isactive=1 or FieldType='Auto Number') AND FF.EID=" & EID & " AND F.EID= " & EID & " AND  FF.DocumentType  in(select FORMNAME FROM MMM_MST_Forms WHERE EID=" & EID & " AND FormSource='DETAIL FORM' AND EventName in (" & Doclist & "))"
                If ret.ToUpper() <> "NEW" Then
                    Query = Query & " AND " & " FF.lastupdate >= " & "'" & ret & "'"
                    Query1 = Query1 & " AND " & " FF.lastupdate >= " & "'" & ret & "'"
                    Query2 = Query2 & " AND " & " FF.lastupdate >= " & "'" & ret & "'"
                    Query3 = Query3 & " AND " & " FF.lastupdate >= " & "'" & ret & "'"
                End If
                Dim StrQUery = Query & Query1 & Query2 & Query3
                da = New SqlDataAdapter(StrQUery, con)
                If con.State <> ConnectionState.Open Then
                    con.Open()
                End If
                da.Fill(dsfld)
                If dsfld.Tables(0).Rows.Count > 0 Then
                    For i As Integer = 0 To dsfld.Tables(0).Rows.Count - 1
                        objFD = New FormData()
                        objFD.FieldID = dsfld.Tables(0).Rows(i).Item("FieldID").ToString()
                        objFD.EID = dsfld.Tables(0).Rows(i).Item("EID").ToString()
                        objFD.displayName = dsfld.Tables(0).Rows(i).Item("displayName").ToString()
                        objFD.FieldType = dsfld.Tables(0).Rows(i).Item("FieldType").ToString()
                        objFD.displayOrder = dsfld.Tables(0).Rows(i).Item("displayOrder").ToString()
                        objFD.FieldMapping = dsfld.Tables(0).Rows(i).Item("FieldMapping").ToString()
                        objFD.isRequired = dsfld.Tables(0).Rows(i).Item("isRequired").ToString()
                        objFD.isActive = dsfld.Tables(0).Rows(i).Item("isActive").ToString()
                        objFD.dropdown = dsfld.Tables(0).Rows(i).Item("dropdown").ToString()
                        objFD.DocumentType = dsfld.Tables(0).Rows(i).Item("DocumentType").ToString()
                        objFD.DBTableName = dsfld.Tables(0).Rows(i).Item("DBTableName").ToString()
                        objFD.FieldNature = dsfld.Tables(0).Rows(i).Item("FieldNature").ToString()
                        objFD.DropDownType = dsfld.Tables(0).Rows(i).Item("DropDownType").ToString()
                        objFD.isEditable = dsfld.Tables(0).Rows(i).Item("isEditable").ToString()
                        objFD.datatype = dsfld.Tables(0).Rows(i).Item("datatype").ToString()
                        objFD.MinLen = dsfld.Tables(0).Rows(i).Item("MinLen").ToString()
                        objFD.MaxLen = dsfld.Tables(0).Rows(i).Item("MaxLen").ToString()
                        objFD.isWorkFlow = dsfld.Tables(0).Rows(i).Item("isWorkFlow").ToString()
                        objFD.lookupvalue = dsfld.Tables(0).Rows(i).Item("lookupvalue").ToString()
                        objFD.Cal_Fields = dsfld.Tables(0).Rows(i).Item("Cal_Fields").ToString()
                        objFD.AutoFilter = dsfld.Tables(0).Rows(i).Item("AutoFilter").ToString()
                        objFD.KC_VALUE = dsfld.Tables(0).Rows(i).Item("KC_VALUE").ToString()
                        objFD.KC_LOGIC = dsfld.Tables(0).Rows(i).Item("KC_LOGIC").ToString()
                        objFD.isunique = dsfld.Tables(0).Rows(i).Item("isunique").ToString()
                        objFD.ShowOnDocDetail = dsfld.Tables(0).Rows(i).Item("ShowOnDocDetail").ToString()
                        objFD.IsSession = dsfld.Tables(0).Rows(i).Item("IsSession").ToString()
                        objFD.isMail = dsfld.Tables(0).Rows(i).Item("isMail").ToString()
                        objFD.KC_STATUS = dsfld.Tables(0).Rows(i).Item("KC_STATUS").ToString()
                        objFD.ShowOnAmendment = dsfld.Tables(0).Rows(i).Item("ShowOnAmendment").ToString()
                        objFD.isSearch = dsfld.Tables(0).Rows(i).Item("isSearch").ToString()
                        objFD.isImieno = dsfld.Tables(0).Rows(i).Item("isImieno").ToString()
                        objFD.isPhoneNo = dsfld.Tables(0).Rows(i).Item("isPhoneNo").ToString()
                        objFD.iseditonAmend = dsfld.Tables(0).Rows(i).Item("iseditonAmend").ToString()
                        objFD.InLineEditing = dsfld.Tables(0).Rows(i).Item("InLineEditing").ToString()
                        objFD.dependentON = dsfld.Tables(0).Rows(i).Item("dependentON").ToString()
                        objFD.Invisible = dsfld.Tables(0).Rows(i).Item("Invisible").ToString()
                        objFD.Cal_Text = dsfld.Tables(0).Rows(i).Item("cal_text").ToString()
                        objFD.DefaultValue = dsfld.Tables(0).Rows(i).Item("defaultfieldval").ToString()
                        objFD.FixedValuedFilter = dsfld.Tables(0).Rows(i).Item("initialFilter").ToString()
                        objFD.ShowOnDashBoard = dsfld.Tables(0).Rows(i).Item("showOnDashBoard").ToString()
                        objFD.DDllookupvalue = dsfld.Tables(0).Rows(i).Item("DDllookupvalue").ToString()
                        objFD.externallookupformobile = Convert.ToString(dsfld.Tables(0).Rows(i).Item("externallookupformobile"))

                        objFD.IsTotal = dsfld.Tables(0).Rows(i).Item("IsTotal").ToString()
                        objFD.iscatchment = dsfld.Tables(0).Rows(i).Item("iscatchment").ToString()
                        objFD.alloweditonedit = dsfld.Tables(0).Rows(i).Item("alloweditonedit").ToString()
                        objFD.ChildTemp_column = dsfld.Tables(0).Rows(i).Item("ChildTemp_column").ToString()
                        objFD.ddlval = dsfld.Tables(0).Rows(i).Item("ddlval").ToString()
                        objFD.DDLlookupValueSource = dsfld.Tables(0).Rows(i).Item("DDLlookupValueSource").ToString()
                        objFD.geofencetype = dsfld.Tables(0).Rows(i).Item("geofencetype").ToString()

                        objFD.MultiLookUpVal = dsfld.Tables(0).Rows(i).Item("MultiLookUpVal").ToString()
                        objFD.Child_Specific_text = dsfld.Tables(0).Rows(i).Item("Child_Specific_text").ToString()
                        objFD.IsCardNo = dsfld.Tables(0).Rows(i).Item("IsCardNo").ToString()
                        objFD.ddlMultilookupval = dsfld.Tables(0).Rows(i).Item("ddlMultilookupval").ToString()
                        objFD.isgeofence_filter = dsfld.Tables(0).Rows(i).Item("isgeofence_filter").ToString()
                        objFD.geofensefiltersourcefield = Convert.ToString(dsfld.Tables(0).Rows(i).Item("geofensefiltersourcefield"))

                        'DDllookupvalue
                        lstFlds.Add(objFD)
                    Next
                End If

            End If
        Catch ex As Exception
            Throw New FaultException("Error occured at server")
        Finally
            If Not con Is Nothing Then
                con.Close()
                con.Dispose()
            End If
            If Not da Is Nothing Then
                da.Dispose()
            End If
        End Try
        'Dim str = WebOperationContext.Current.OutgoingResponse.ToString()

        Return lstFlds
    End Function



    Function GetChildItemDefaultValueALL(Key As String, EID As String, UID As Integer, URole As String, IMINum As String, ST As String) As List(Of ChildItemDefaultValue) Implements IBPMMobileSyncAPI.GetChildItemDefaultValueALL

        Dim lstFlds As New List(Of ChildItemDefaultValue)
        Dim da As SqlDataAdapter = Nothing
        Dim con As SqlConnection = Nothing
        Dim objDev As New DeviceInfo()
        Dim ret = "NEW"
        Try
            Dim IsValidrequest = False
            Dim dsD As New DataSet()
            objDev.IMINumber = IMINum
            dsD = objDev.ValidateDevice(EID, UID)

            If dsD.Tables(0).Rows.Count > 0 Then
                IsValidrequest = True
                If Not ST.ToUpper() = "COMPLETE" Then
                    ret = dsD.Tables(0).Rows(0).Item("LastUpdatedOn").ToString()
                Else
                    ret = "NEW"
                End If
            End If
            If IsValidrequest = True And BPMKEY = Key Then
                Dim Doclist = GetFilteredDoc(EID, URole).ToString

                Dim ds As New DataSet()
                Dim strQuery = "SELECT Substring(DocumentType,1,len(DocumentType)-7)'DocType', * FROM MMM_MST_MASTER WHERE EID=" & EID & " AND  isauth =1 AND DocumentType in(select FormName+'_Master' FROM MMM_MST_FORMS where EID=" & EID & " and hasdefaultvalue=1 and FormSource='DETAIL FORM' AND FormName in(SELECT DROPDOWN FROM MMM_MST_FIELDS WHERE EID= " & EID & " AND IsActive=1 AND FieldType='Child item' AND DocumentType in( " & Doclist & " ) )) "
                If ret.ToUpper() <> "NEW" Then
                    strQuery = strQuery & " and lastUpdate >'" & ret & "'"
                End If
                strQuery = strQuery & " order by DocumentType"
                'order by DocumentType
                con = New SqlConnection(conStr)
                con.Open()
                da = New SqlDataAdapter(strQuery, con)
                da.Fill(ds)
                Dim obj As ChildItemDefaultValue
                'ds = CommanUtil.GetDropDownDefaultValue(EID, ChildItemName)

                If ds.Tables(0).Rows.Count > 0 Then
                    For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                        obj = New ChildItemDefaultValue()
                        obj.DocumentType = ds.Tables(0).Rows(i).Item("DocType").ToString()
                        obj.fld1 = ds.Tables(0).Rows(i).Item("fld1").ToString()
                        obj.fld2 = ds.Tables(0).Rows(i).Item("fld2").ToString()
                        obj.fld3 = ds.Tables(0).Rows(i).Item("fld3").ToString()
                        obj.fld4 = ds.Tables(0).Rows(i).Item("fld4").ToString()
                        obj.fld5 = ds.Tables(0).Rows(i).Item("fld5").ToString()
                        obj.fld6 = ds.Tables(0).Rows(i).Item("fld6").ToString()
                        obj.fld7 = ds.Tables(0).Rows(i).Item("fld7").ToString()
                        obj.fld8 = ds.Tables(0).Rows(i).Item("fld8").ToString()
                        obj.fld9 = ds.Tables(0).Rows(i).Item("fld9").ToString()
                        obj.fld10 = ds.Tables(0).Rows(i).Item("fld10").ToString()
                        obj.fld11 = ds.Tables(0).Rows(i).Item("fld11").ToString()
                        obj.fld12 = ds.Tables(0).Rows(i).Item("fld12").ToString()
                        obj.fld13 = ds.Tables(0).Rows(i).Item("fld13").ToString()
                        obj.fld14 = ds.Tables(0).Rows(i).Item("fld14").ToString()
                        obj.fld15 = ds.Tables(0).Rows(i).Item("fld15").ToString()
                        obj.fld16 = ds.Tables(0).Rows(i).Item("fld16").ToString()
                        obj.fld17 = ds.Tables(0).Rows(i).Item("fld17").ToString()
                        obj.fld18 = ds.Tables(0).Rows(i).Item("fld18").ToString()
                        obj.fld19 = ds.Tables(0).Rows(i).Item("fld19").ToString()
                        obj.fld20 = ds.Tables(0).Rows(i).Item("fld20").ToString()

                        obj.fld21 = ds.Tables(0).Rows(i).Item("fld21").ToString()
                        obj.fld22 = ds.Tables(0).Rows(i).Item("fld22").ToString()
                        obj.fld23 = ds.Tables(0).Rows(i).Item("fld23").ToString()
                        obj.fld24 = ds.Tables(0).Rows(i).Item("fld24").ToString()
                        obj.fld25 = ds.Tables(0).Rows(i).Item("fld25").ToString()
                        obj.fld26 = ds.Tables(0).Rows(i).Item("fld26").ToString()
                        obj.fld27 = ds.Tables(0).Rows(i).Item("fld27").ToString()
                        obj.fld28 = ds.Tables(0).Rows(i).Item("fld28").ToString()
                        obj.fld29 = ds.Tables(0).Rows(i).Item("fld29").ToString()
                        obj.fld30 = ds.Tables(0).Rows(i).Item("fld30").ToString()

                        obj.fld30 = ds.Tables(0).Rows(i).Item("fld30").ToString()
                        obj.fld31 = ds.Tables(0).Rows(i).Item("fld31").ToString()
                        obj.fld32 = ds.Tables(0).Rows(i).Item("fld32").ToString()
                        obj.fld33 = ds.Tables(0).Rows(i).Item("fld33").ToString()
                        obj.fld34 = ds.Tables(0).Rows(i).Item("fld34").ToString()
                        obj.fld35 = ds.Tables(0).Rows(i).Item("fld35").ToString()
                        obj.fld36 = ds.Tables(0).Rows(i).Item("fld36").ToString()
                        obj.fld37 = ds.Tables(0).Rows(i).Item("fld37").ToString()
                        obj.fld38 = ds.Tables(0).Rows(i).Item("fld38").ToString()
                        obj.fld39 = ds.Tables(0).Rows(i).Item("fld39").ToString()
                        obj.fld40 = ds.Tables(0).Rows(i).Item("fld40").ToString()

                        obj.fld41 = ds.Tables(0).Rows(i).Item("fld41").ToString()
                        obj.fld42 = ds.Tables(0).Rows(i).Item("fld42").ToString()
                        obj.fld43 = ds.Tables(0).Rows(i).Item("fld43").ToString()
                        obj.fld44 = ds.Tables(0).Rows(i).Item("fld44").ToString()
                        obj.fld45 = ds.Tables(0).Rows(i).Item("fld45").ToString()
                        obj.fld46 = ds.Tables(0).Rows(i).Item("fld46").ToString()
                        obj.fld47 = ds.Tables(0).Rows(i).Item("fld47").ToString()
                        obj.fld48 = ds.Tables(0).Rows(i).Item("fld48").ToString()
                        obj.fld49 = ds.Tables(0).Rows(i).Item("fld49").ToString()
                        obj.fld50 = ds.Tables(0).Rows(i).Item("fld50").ToString()
                        lstFlds.Add(obj)
                    Next
                End If

            End If
        Catch ex As Exception
            Throw New FaultException("Error occured at server")
        Finally
            If Not con Is Nothing Then
                con.Close()
                con.Dispose()
            End If
            If Not da Is Nothing Then
                da.Dispose()
            End If
        End Try
        Return lstFlds
    End Function

    Function GetUserMenu(Key As String, EID As String, UserRole As String, UID As Integer, IMINum As String, ST As String) As List(Of UserMenu) Implements IBPMMobileSyncAPI.GetUserMenu
        Dim lstMenu As New List(Of UserMenu)
        Dim dsMenu As New DataSet()
        Dim objM As UserMenu
        Dim objDev As New DeviceInfo()
        Dim ret = "NEW"
        Try
            Dim IsValidrequest = False
            Dim dsD As New DataSet()
            objDev.IMINumber = IMINum
            dsD = objDev.ValidateDevice(EID, UID)
            If dsD.Tables(0).Rows.Count > 0 Then
                IsValidrequest = True
                If Not ST.ToUpper() = "COMPLETE" Then
                    ret = dsD.Tables(0).Rows(0).Item("LastUpdatedOn").ToString()
                Else
                    ret = "1900-01-01 12:25:25.010"
                End If
            End If

            If BPMKEY = Key And IsValidrequest = True Then
                UserRole = "{" & UserRole
                dsMenu = GetMenu(EID:=EID, userrole:=UserRole, LastUpdate:=ret)
                Dim FormName As String = ""
                If dsMenu.Tables(0).Rows.Count > 0 Then
                    For i As Integer = 0 To dsMenu.Tables(0).Rows.Count - 1
                        objM = New UserMenu()
                        objM.MenuName = dsMenu.Tables(0).Rows(i).Item("MenuName")
                        objM.MID = dsMenu.Tables(0).Rows(i).Item("MID")
                        FormName = dsMenu.Tables(0).Rows(i).Item("PageLink")

                        'Dim arStr As New String()=dsMenu.Tables(0).Rows(i).Item("PageLink").ToString().Split('=')
                        If dsMenu.Tables(0).Rows(i).Item("PageLink").ToString().Contains("=") Then
                            Dim arStr As String() = dsMenu.Tables(0).Rows(i).Item("PageLink").ToString().Split("=")
                            FormName = arStr(1)
                        End If
                        'ajeettunnu@gmail.com,ajeetkumar11@hotmail.com
                        objM.FormName = FormName
                        objM.ImageName = dsMenu.Tables(0).Rows(i).Item("Image")
                        objM.PMenu = dsMenu.Tables(0).Rows(i).Item("Pmenu")
                        'Dim onlyFiltered As DataView = dsMenu.Tables(1).DefaultView
                        'onlyFiltered.RowFilter = "FormName='" & FormName & "'" & ""
                        'Dim theFlds As DataTable = onlyFiltered.Table.DefaultView.ToTable()
                        'objM.EnableDraft = theFlds.Rows(0).Item("enableDraft")
                        lstMenu.Add(objM)
                    Next
                End If
            End If
        Catch ex As Exception
            Throw New FaultException("RTO")
        End Try
        Return lstMenu
    End Function

    Function GetFormValidaion(Key As String, EID As String, UID As Integer, IMINum As String, ST As String) As List(Of FormValidation) Implements IBPMMobileSyncAPI.GetFormValidaion
        Dim lstFLV As New List(Of FormValidation)
        Dim ds As New DataSet()
        Dim objM As New FormValidation()
        Dim objDev As New DeviceInfo()
        Dim ret = "NEW"
        Try
            Dim IsValidrequest = False
            Dim dsD As New DataSet()
            objDev.IMINumber = IMINum
            dsD = objDev.ValidateDevice(EID, UID)
            If dsD.Tables(0).Rows.Count > 0 Then
                IsValidrequest = True
                If Not ST.ToUpper() = "COMPLETE" Then
                    ret = dsD.Tables(0).Rows(0).Item("LastUpdatedOn").ToString()
                Else
                    ret = "1900-01-01 00:00:00.000"
                End If
            End If
            If BPMKEY = Key And IsValidrequest = True Then
                ds = objM.GetFormValidation(EID:=EID, lastUpdate:=ret)
                Dim FormName As String = ""
                If ds.Tables(0).Rows.Count > 0 Then
                    For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                        objM = New FormValidation()
                        objM.EID = Convert.ToString(ds.Tables(0).Rows(i).Item("EID"))
                        objM.DocType = Convert.ToString(ds.Tables(0).Rows(i).Item("DocType"))
                        objM.ValType = Convert.ToString(ds.Tables(0).Rows(i).Item("ValType"))
                        objM.fldID = Convert.ToString(ds.Tables(0).Rows(i).Item("fldID"))
                        objM.Operator1 = Convert.ToString(ds.Tables(0).Rows(i).Item("Operator"))
                        objM.Value = Convert.ToString(ds.Tables(0).Rows(i).Item("Value"))
                        objM.Err_MSG = Convert.ToString(ds.Tables(0).Rows(i).Item("Err_Msg"))
                        objM.WF_Status = Convert.ToString(ds.Tables(0).Rows(i).Item("WF_Status"))
                        objM.docNature = Convert.ToString(ds.Tables(0).Rows(i).Item("docNature"))
                        objM.TID = Convert.ToString(ds.Tables(0).Rows(i).Item("tid"))
                        lstFLV.Add(objM)
                    Next
                End If
            End If
        Catch ex As Exception
            Throw New FaultException("RTO")
        End Try
        Return lstFLV

    End Function

    Function GetPDFTemplet(Key As String, EID As String, UID As Integer, IMINum As String, ST As String) As List(Of PDFTemplet) Implements IBPMMobileSyncAPI.GetPDFTemplet
        Dim lstFLV As New List(Of PDFTemplet)
        Dim ds As New DataSet()
        Dim objM As New PDFTemplet()
        Dim objDev As New DeviceInfo()
        Dim ret = "NEW"
        Try
            Dim IsValidrequest = False
            Dim dsD As New DataSet()
            objDev.IMINumber = IMINum
            dsD = objDev.ValidateDevice(EID, UID)
            If dsD.Tables(0).Rows.Count > 0 Then
                IsValidrequest = True
                If Not ST.ToUpper() = "COMPLETE" Then
                    ret = dsD.Tables(0).Rows(0).Item("LastUpdatedOn").ToString()
                Else
                    ret = "1900-01-01 00:00:00.000"
                End If
            End If
            If BPMKEY = Key And IsValidrequest = True Then
                ds = objM.GetPDFTemplet(EID:=EID, lastUpdate:=ret)
                Dim FormName As String = ""
                If ds.Tables(0).Rows.Count > 0 Then
                    For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                        objM = New PDFTemplet()
                        objM.TID = Convert.ToString(ds.Tables(0).Rows(i).Item("TID"))
                        objM.Template_name = Convert.ToString(ds.Tables(0).Rows(i).Item("Template_name"))
                        objM.Body = Convert.ToString(ds.Tables(0).Rows(i).Item("Body"))
                        objM.DocumentType = Convert.ToString(ds.Tables(0).Rows(i).Item("DocumentType"))
                        objM.TID = Convert.ToString(ds.Tables(0).Rows(i).Item("tid"))
                        lstFLV.Add(objM)
                    Next
                End If
            End If
        Catch ex As Exception
            Throw New FaultException("RTO")
        End Try
        Return lstFLV
    End Function

    Function GetTriggers(Key As String, EID As String, UID As Integer, IMINum As String, ST As String) As List(Of Trigger) Implements IBPMMobileSyncAPI.GetTriggers
        Dim lstTrg As New List(Of Trigger)
        Dim ds As New DataSet()
        Dim objT As New Trigger()
        Dim objDev As New DeviceInfo()
        Dim ret = "NEW"
        Try
            Dim IsValidrequest = False
            Dim dsD As New DataSet()
            objDev.IMINumber = IMINum
            dsD = objDev.ValidateDevice(EID, UID)
            If dsD.Tables(0).Rows.Count > 0 Then
                IsValidrequest = True
                If Not ST.ToUpper() = "COMPLETE" Then
                    ret = dsD.Tables(0).Rows(0).Item("LastUpdatedOn").ToString()
                Else
                    ret = "1900-01-01 00:00:00.000"
                End If
            End If
            If BPMKEY = Key And IsValidrequest = True Then

                'GetFormValidation(EID As String, lastUpdate As String)
                'GetTriggers(ByVal TId As Integer, ByVal EID As Integer, ByVal FORMID As Integer, Optional LastUpdated As String = "1900-01-01 16:09:49.613")
                ds = objT.GetTriggers(TId:=0, EID:=EID, FORMID:=0, DocumentType:="", LastUpdated:=ret)
                Dim FormName As String = ""
                If ds.Tables(0).Rows.Count > 0 Then
                    For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                        objT = New Trigger()
                        objT.TID = Convert.ToString(ds.Tables(0).Rows(i).Item("TID"))
                        objT.TriggerText = Convert.ToString(ds.Tables(0).Rows(i).Item("TriggerText"))
                        objT.DocumentType = Convert.ToString(ds.Tables(0).Rows(i).Item("DocType"))
                        objT.onCreate = Convert.ToString(ds.Tables(0).Rows(i).Item("onCreate"))
                        objT.onEdit = Convert.ToString(ds.Tables(0).Rows(i).Item("onEdit"))
                        lstTrg.Add(objT)
                    Next
                End If
            End If
        Catch ex As Exception
            Throw New FaultException("RTO")
        End Try
        Return lstTrg
    End Function

    Function SyncDeletedFields(Key As String, EID As String, UID As Integer, IMINum As String, ST As String) As String Implements IBPMMobileSyncAPI.SyncDeletedFields
        Dim objDev As New DeviceInfo()
        Dim ret = "NEW"
        Dim Result = ""
        Try
            Dim IsValidrequest = False
            Dim dsD As New DataSet()
            objDev.IMINumber = IMINum
            dsD = objDev.ValidateDevice(EID, UID)
            If dsD.Tables(0).Rows.Count > 0 Then
                IsValidrequest = True
                If Not ST.ToUpper() = "COMPLETE" Then
                    ret = dsD.Tables(0).Rows(0).Item("LastUpdatedOn").ToString()
                Else
                    ret = "1900-01-01 00:00:00.000"
                End If
            End If
            If BPMKEY = Key And IsValidrequest = True Then
                Dim ds As New DataSet()
                Using con = New SqlConnection(conStr)
                    Using da = New SqlDataAdapter("SyncdeletedFields", con)
                        da.SelectCommand.CommandType = CommandType.StoredProcedure
                        da.SelectCommand.Parameters.AddWithValue("@EID", EID)
                        da.SelectCommand.Parameters.AddWithValue("@lastUpdatedOn", ret)
                        da.Fill(ds)
                    End Using
                End Using
                If ds.Tables(0).Rows.Count > 0 Then
                    Result = Convert.ToString(ds.Tables(0).Rows(0).Item("Data"))
                End If
            End If
        Catch ex As Exception
            Throw New FaultException("RTO")
        End Try
        Return Result
    End Function

    Function GetFormDetails(Key As String, EID As String, UID As Integer, URole As String, IMINum As String, ST As String) As List(Of FormDetails) Implements IBPMMobileSyncAPI.GetFormDetails

        Dim lstForms As New List(Of FormDetails)
        Dim ds As New DataSet()
        Dim objM As New FormDetails()
        Dim objDev As New DeviceInfo()
        Dim ret = "NEW"
        Try
            Dim IsValidrequest = False
            Dim dsD As New DataSet()
            objDev.IMINumber = IMINum
            dsD = objDev.ValidateDevice(EID, UID)
            If dsD.Tables(0).Rows.Count > 0 Then
                IsValidrequest = True
                If Not ST.ToUpper() = "COMPLETE" Then
                    ret = dsD.Tables(0).Rows(0).Item("LastUpdatedOn").ToString()
                Else
                    ret = "1900-01-01 00:00:00.000"
                End If
            End If

            Dim Doclist = GetFilteredDoc(EID, URole).ToString

            If (String.IsNullOrEmpty(Doclist.ToString.Trim)) Then
                Doclist = "'No Any Access provided'"
            End If

            Dim Query = "select * FROM MMM_MST_FORMS  where eid=" & EID & " AND FormName in(" & Doclist & ")"
            Dim Query1 = " union all select * FROM MMM_MST_Forms WHERE EID=" & EID & " AND FormSource='DETAIL FORM' AND FormName in (SELECT DROPDOWN FROM MMM_MST_FIELDS WHERE EID=" & EID & " AND FieldType='CHILD ITEM' AND IsActive=1)"
            Dim Query2 = " union all select * FROM MMM_MST_Forms WHERE EID=" & EID & " AND FormSource='Action driven'"

            If BPMKEY = Key And IsValidrequest = True Then
                Query = Query & " AND LastUpdate > '" & ret & "'"
                Query1 = Query1 & " AND LastUpdate > '" & ret & "'"
                Query2 = Query2 & " AND LastUpdate > '" & ret & "'"
                Dim StrQuery = Query & Query1 & Query2
                Using con = New SqlConnection(conStr)
                    Using da = New SqlDataAdapter(StrQuery, con)
                        da.Fill(ds)
                    End Using
                End Using
                If ds.Tables(0).Rows.Count > 0 Then
                    For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                        objM = New FormDetails()
                        objM.FormName = Convert.ToString(ds.Tables(0).Rows(i).Item("FormName"))
                        objM.EID = Convert.ToString(ds.Tables(0).Rows(i).Item("EID"))
                        objM.FormType = Convert.ToString(ds.Tables(0).Rows(i).Item("FormType"))
                        objM.FormSource = Convert.ToString(ds.Tables(0).Rows(i).Item("FormSource"))
                        objM.IsActive = Convert.ToString(ds.Tables(0).Rows(i).Item("IsActive"))
                        objM.EventName = Convert.ToString(ds.Tables(0).Rows(i).Item("EventName"))
                        objM.SubEvent = Convert.ToString(ds.Tables(0).Rows(i).Item("SubEvent"))
                        objM.FormCaption = Convert.ToString(ds.Tables(0).Rows(i).Item("FormCaption"))
                        objM.FormDesc = Convert.ToString(ds.Tables(0).Rows(i).Item("FormDesc"))
                        objM.LayoutType = Convert.ToString(ds.Tables(0).Rows(i).Item("LayoutType"))
                        objM.curstatus = Convert.ToString(ds.Tables(0).Rows(i).Item("curstatus"))
                        objM.layoutdata = Convert.ToString(ds.Tables(0).Rows(i).Item("layoutdata"))
                        objM.Iscalendar = Convert.ToString(ds.Tables(0).Rows(i).Item("Iscalendar"))
                        objM.isWorkFlow = Convert.ToString(ds.Tables(0).Rows(i).Item("isWorkFlow"))
                        objM.History = Convert.ToString(ds.Tables(0).Rows(i).Item("History"))
                        objM.isPush = Convert.ToString(ds.Tables(0).Rows(i).Item("isPush"))
                        objM.isRoleDef = Convert.ToString(ds.Tables(0).Rows(i).Item("isRoleDef"))
                        objM.DocMapping = Convert.ToString(ds.Tables(0).Rows(i).Item("DocMapping"))
                        objM.PUBLICVIEW = Convert.ToString(ds.Tables(0).Rows(i).Item("PUBLICVIEW"))
                        objM.PUBLICENTRY = Convert.ToString(ds.Tables(0).Rows(i).Item("PUBLICENTRY"))
                        objM.DocNature = Convert.ToString(ds.Tables(0).Rows(i).Item("DocNature"))
                        objM.hasdefaultvalue = Convert.ToString(ds.Tables(0).Rows(i).Item("hasdefaultvalue"))
                        objM.AttahcedFileSize = Convert.ToString(ds.Tables(0).Rows(i).Item("AttahcedFileSize"))
                        objM.ShowUploader = Convert.ToString(ds.Tables(0).Rows(i).Item("ShowUploader"))
                        objM.EnableWS = Convert.ToString(ds.Tables(0).Rows(i).Item("EnableWS"))
                        objM.lastupdate = Convert.ToString(ds.Tables(0).Rows(i).Item("lastupdate"))
                        objM.enableDraft = Convert.ToString(ds.Tables(0).Rows(i).Item("enableDraft"))
                        objM.isinlineediting = Convert.ToString(ds.Tables(0).Rows(i).Item("isinlineediting"))
                        objM.inlinetype = Convert.ToString(ds.Tables(0).Rows(i).Item("inlinetype"))
                        objM.inlinesourcedoc = Convert.ToString(ds.Tables(0).Rows(i).Item("inlinesourcedoc"))
                        objM.inlinefilter = Convert.ToString(ds.Tables(0).Rows(i).Item("inlinefilter"))
                        objM.inlinefilterdisplay = Convert.ToString(ds.Tables(0).Rows(i).Item("inlinefilterdisplay"))
                        objM.UniqueKeys = Convert.ToString(ds.Tables(0).Rows(i).Item("UniqueKeys"))
                        objM.UniqueKeys_ExChars = Convert.ToString(ds.Tables(0).Rows(i).Item("UniqueKeys_ExChars"))
                        objM.SortingFields = Convert.ToString(ds.Tables(0).Rows(i).Item("SortingFields"))
                        objM.enableCRM = Convert.ToString(ds.Tables(0).Rows(i).Item("enableCRM"))
                        objM.isUserCreation = Convert.ToString(ds.Tables(0).Rows(i).Item("isUserCreation"))
                        objM.enable_mail_On_CRM = Convert.ToString(ds.Tables(0).Rows(i).Item("enable_mail_On_CRM"))
                        objM.IsDefaultRows = Convert.ToString(ds.Tables(0).Rows(i).Item("IsDefaultRows"))
                        objM.inlinemappingdisplay = Convert.ToString(ds.Tables(0).Rows(i).Item("inlinemappingdisplay"))
                        objM.autosaveinterval = Convert.ToString(ds.Tables(0).Rows(i).Item("autosaveinterval"))
                        objM.Primarykey = Convert.ToString(ds.Tables(0).Rows(i).Item("Primarykey"))
                        objM.JsonQuery = Convert.ToString(ds.Tables(0).Rows(i).Item("JsonQuery"))
                        objM.Balance_Maintenance_Mode = Convert.ToString(ds.Tables(0).Rows(i).Item("Balance_Maintenance_Mode"))
                        objM.Effective_Date_Field = Convert.ToString(ds.Tables(0).Rows(i).Item("Effective_Date_Field"))
                        objM.Balance_Field = Convert.ToString(ds.Tables(0).Rows(i).Item("Balance_Field"))
                        objM.Item_Number = Convert.ToString(ds.Tables(0).Rows(i).Item("Item_Number"))
                        objM.Relation_Doc_Type = Convert.ToString(ds.Tables(0).Rows(i).Item("Relation_Doc_Type"))
                        objM.Balance_Type = Convert.ToString(ds.Tables(0).Rows(i).Item("Balance_Type"))
                        objM.EnableInsertOneditFail = Convert.ToString(ds.Tables(0).Rows(i).Item("EnableInsertOneditFail"))
                        objM.XMLInwardInputDocType = Convert.ToString(ds.Tables(0).Rows(i).Item("XMLInwardInputDocType"))
                        objM.XMLInwardInputEntityCode = Convert.ToString(ds.Tables(0).Rows(i).Item("XMLInwardInputEntityCode"))
                        objM.XMLOutWardDocType = Convert.ToString(ds.Tables(0).Rows(i).Item("XMLOutWardDocType"))
                        objM.XMLOutWardEntityCOde = Convert.ToString(ds.Tables(0).Rows(i).Item("XMLOutWardEntityCOde"))
                        objM.RowFilterXMLTag = Convert.ToString(ds.Tables(0).Rows(i).Item("RowFilterXMLTag"))
                        objM.RowFilterBPMField = Convert.ToString(ds.Tables(0).Rows(i).Item("RowFilterBPMField"))
                        objM.ChildMasterField = Convert.ToString(ds.Tables(0).Rows(i).Item("ChildMasterField"))
                        objM.ChildFilterRule = Convert.ToString(ds.Tables(0).Rows(i).Item("ChildFilterRule"))
                        objM.S_T_DDN = Convert.ToString(ds.Tables(0).Rows(i).Item("S_T_DDN"))
                        objM.Relation_Type = Convert.ToString(ds.Tables(0).Rows(i).Item("Relation_Type"))
                        objM.TallyRegField = Convert.ToString(ds.Tables(0).Rows(i).Item("TallyRegField"))
                        objM.TallyCancelXMlTag = Convert.ToString(ds.Tables(0).Rows(i).Item("TallyCancelXMlTag"))
                        objM.Allowmultiuse = Convert.ToString(ds.Tables(0).Rows(i).Item("Allowmultiuse"))
                        objM.EnableSync_Tally = Convert.ToString(ds.Tables(0).Rows(i).Item("EnableSync_Tally"))
                        objM.Sync_Tally_Ordering = Convert.ToString(ds.Tables(0).Rows(i).Item("Sync_Tally_Ordering"))
                        lstForms.Add(objM)
                    Next
                End If
            End If
        Catch ex As Exception
            Throw New FaultException("RTO")
        End Try
        Return lstForms
    End Function

    Function SyncAuthMatrix(Key As String, EID As String, UID As Integer, IMINum As String, ST As String) As String Implements IBPMMobileSyncAPI.SyncAuthMatrix

        Dim result = ""
        Dim objDev As New DeviceInfo()
        Dim ret = "NEW"
        objDev.IMINumber = IMINum
        Dim dsD = New DataSet()
        Dim IsValidrequest = False
        dsD = objDev.ValidateDevice(EID, UID)
        If dsD.Tables(0).Rows.Count > 0 Then
            IsValidrequest = True
            If Not ST.ToUpper() = "COMPLETE" Then
                ret = dsD.Tables(0).Rows(0).Item("LastUpdatedOn").ToString()
            Else
                ret = "1900-01-01 00:00:00.000"
            End If
        End If
        Try
            If BPMKEY = Key And IsValidrequest = True Then
                Dim DsUser As New DataSet()
                'For avoiding sqlinjection 
                EID = EID.Replace("'", String.Empty).Replace("--", "")
                Using con = New SqlConnection(conStr)
                    Using da = New SqlDataAdapter("select * FROM MMM_MST_AuthMetrix WHERE EID= " & EID & " AND  lastupdate >'" & ret & "'", con)
                        con.Open()
                        da.Fill(DsUser)
                    End Using
                End Using
                Dim SBData As New StringBuilder()

                'uid	doctype	sla	aprStatus	ordering EID,TID
                If DsUser.Tables(0).Rows.Count > 0 Then
                    For i As Integer = 0 To DsUser.Tables(0).Rows.Count - 1
                        SBData.Append("::")
                        SBData.Append(DsUser.Tables(0).Rows(i).Item("TID").ToString() & "|")
                        SBData.Append(DsUser.Tables(0).Rows(i).Item("EID").ToString() & "|")
                        SBData.Append(DsUser.Tables(0).Rows(i).Item("doctype").ToString() & "|")
                        SBData.Append(DsUser.Tables(0).Rows(i).Item("uid").ToString() & "|")
                        SBData.Append(DsUser.Tables(0).Rows(i).Item("sla").ToString() & "|")
                        SBData.Append(DsUser.Tables(0).Rows(i).Item("aprStatus").ToString() & "|")
                        SBData.Append(DsUser.Tables(0).Rows(i).Item("ordering").ToString() & "|")

                        SBData.Append(DsUser.Tables(0).Rows(i).Item("RoleName").ToString() & "|")
                        SBData.Append(DsUser.Tables(0).Rows(i).Item("fieldname").ToString() & "|")
                        SBData.Append(DsUser.Tables(0).Rows(i).Item("slevel").ToString() & "|")
                        SBData.Append(DsUser.Tables(0).Rows(i).Item("DocNature").ToString() & "|")
                        SBData.Append(DsUser.Tables(0).Rows(i).Item("Type").ToString() & "|")
                        'RoleName, fieldname(), slevel(), DocNature()



                        SBData.Append(DsUser.Tables(0).Rows(i).Item("fld1").ToString() & "|")
                        SBData.Append(DsUser.Tables(0).Rows(i).Item("fld2").ToString() & "|")
                        SBData.Append(DsUser.Tables(0).Rows(i).Item("fld3").ToString() & "|")
                        SBData.Append(DsUser.Tables(0).Rows(i).Item("fld4").ToString() & "|")
                        SBData.Append(DsUser.Tables(0).Rows(i).Item("fld5").ToString() & "|")
                        SBData.Append(DsUser.Tables(0).Rows(i).Item("fld6").ToString() & "|")
                        SBData.Append(DsUser.Tables(0).Rows(i).Item("fld7").ToString() & "|")
                        SBData.Append(DsUser.Tables(0).Rows(i).Item("fld8").ToString() & "|")
                        SBData.Append(DsUser.Tables(0).Rows(i).Item("fld9").ToString() & "|")
                        SBData.Append(DsUser.Tables(0).Rows(i).Item("fld10").ToString() & "|")
                        SBData.Append(DsUser.Tables(0).Rows(i).Item("fld11").ToString() & "|")
                        SBData.Append(DsUser.Tables(0).Rows(i).Item("fld12").ToString() & "|")
                        SBData.Append(DsUser.Tables(0).Rows(i).Item("fld13").ToString() & "|")
                        SBData.Append(DsUser.Tables(0).Rows(i).Item("fld14").ToString() & "|")
                        SBData.Append(DsUser.Tables(0).Rows(i).Item("fld15").ToString() & "|")
                        SBData.Append(DsUser.Tables(0).Rows(i).Item("fld16").ToString() & "|")
                        SBData.Append(DsUser.Tables(0).Rows(i).Item("fld17").ToString() & "|")
                        SBData.Append(DsUser.Tables(0).Rows(i).Item("fld18").ToString() & "|")
                        SBData.Append(DsUser.Tables(0).Rows(i).Item("fld19").ToString() & "|")
                        SBData.Append(DsUser.Tables(0).Rows(i).Item("fld20").ToString() & "|")
                        SBData.Append(DsUser.Tables(0).Rows(i).Item("fld21").ToString() & "|")
                        SBData.Append(DsUser.Tables(0).Rows(i).Item("fld22").ToString() & "|")
                        SBData.Append(DsUser.Tables(0).Rows(i).Item("fld23").ToString() & "|")
                        SBData.Append(DsUser.Tables(0).Rows(i).Item("fld24").ToString() & "|")
                        SBData.Append(DsUser.Tables(0).Rows(i).Item("fld25").ToString() & "|")
                        SBData.Append(DsUser.Tables(0).Rows(i).Item("fld26").ToString() & "|")
                        SBData.Append(DsUser.Tables(0).Rows(i).Item("fld27").ToString() & "|")
                        SBData.Append(DsUser.Tables(0).Rows(i).Item("fld28").ToString() & "|")
                        SBData.Append(DsUser.Tables(0).Rows(i).Item("fld29").ToString() & "|")
                        SBData.Append(DsUser.Tables(0).Rows(i).Item("fld30").ToString() & "|")
                        SBData.Append(DsUser.Tables(0).Rows(i).Item("fld31").ToString() & "|")
                        SBData.Append(DsUser.Tables(0).Rows(i).Item("fld32").ToString() & "|")
                        SBData.Append(DsUser.Tables(0).Rows(i).Item("fld33").ToString() & "|")
                        SBData.Append(DsUser.Tables(0).Rows(i).Item("fld34").ToString() & "|")
                        SBData.Append(DsUser.Tables(0).Rows(i).Item("fld35").ToString() & "|")
                        SBData.Append(DsUser.Tables(0).Rows(i).Item("fld36").ToString() & "|")
                        SBData.Append(DsUser.Tables(0).Rows(i).Item("fld37").ToString() & "|")
                        SBData.Append(DsUser.Tables(0).Rows(i).Item("fld38").ToString() & "|")
                        SBData.Append(DsUser.Tables(0).Rows(i).Item("fld39").ToString() & "|")
                        SBData.Append(DsUser.Tables(0).Rows(i).Item("fld40").ToString() & "|")
                        SBData.Append(DsUser.Tables(0).Rows(i).Item("fld41").ToString() & "|")
                        SBData.Append(DsUser.Tables(0).Rows(i).Item("fld42").ToString() & "|")
                        SBData.Append(DsUser.Tables(0).Rows(i).Item("fld43").ToString() & "|")
                        SBData.Append(DsUser.Tables(0).Rows(i).Item("fld44").ToString() & "|")
                        SBData.Append(DsUser.Tables(0).Rows(i).Item("fld45").ToString() & "|")
                        SBData.Append(DsUser.Tables(0).Rows(i).Item("fld46").ToString() & "|")
                        SBData.Append(DsUser.Tables(0).Rows(i).Item("fld47").ToString() & "|")
                        SBData.Append(DsUser.Tables(0).Rows(i).Item("fld48").ToString() & "|")
                        SBData.Append(DsUser.Tables(0).Rows(i).Item("fld49").ToString() & "|")
                        SBData.Append(DsUser.Tables(0).Rows(i).Item("fld50").ToString() & "|")
                        SBData.Append(DsUser.Tables(0).Rows(i).Item("fld51").ToString() & "|")
                        SBData.Append(DsUser.Tables(0).Rows(i).Item("fld52").ToString() & "|")
                        SBData.Append(DsUser.Tables(0).Rows(i).Item("fld53").ToString() & "|")
                        SBData.Append(DsUser.Tables(0).Rows(i).Item("fld54").ToString() & "|")
                        SBData.Append(DsUser.Tables(0).Rows(i).Item("fld55").ToString() & "|")
                        SBData.Append(DsUser.Tables(0).Rows(i).Item("fld56").ToString() & "|")
                        SBData.Append(DsUser.Tables(0).Rows(i).Item("fld57").ToString() & "|")
                        SBData.Append(DsUser.Tables(0).Rows(i).Item("fld58").ToString() & "|")
                        SBData.Append(DsUser.Tables(0).Rows(i).Item("fld59").ToString() & "|")
                        SBData.Append(DsUser.Tables(0).Rows(i).Item("fld60").ToString() & "|")
                        SBData.Append(DsUser.Tables(0).Rows(i).Item("fld61").ToString() & "|")
                        SBData.Append(DsUser.Tables(0).Rows(i).Item("fld62").ToString() & "|")
                        SBData.Append(DsUser.Tables(0).Rows(i).Item("fld63").ToString() & "|")
                        SBData.Append(DsUser.Tables(0).Rows(i).Item("fld64").ToString() & "|")
                        SBData.Append(DsUser.Tables(0).Rows(i).Item("fld65").ToString() & "|")
                        SBData.Append(DsUser.Tables(0).Rows(i).Item("fld66").ToString() & "|")
                        SBData.Append(DsUser.Tables(0).Rows(i).Item("fld67").ToString() & "|")
                        SBData.Append(DsUser.Tables(0).Rows(i).Item("fld68").ToString() & "|")
                        SBData.Append(DsUser.Tables(0).Rows(i).Item("fld69").ToString() & "|")
                        SBData.Append(DsUser.Tables(0).Rows(i).Item("fld70").ToString() & "|")
                        SBData.Append(DsUser.Tables(0).Rows(i).Item("fld71").ToString() & "|")
                        SBData.Append(DsUser.Tables(0).Rows(i).Item("fld72").ToString() & "|")
                        SBData.Append(DsUser.Tables(0).Rows(i).Item("fld73").ToString() & "|")
                        SBData.Append(DsUser.Tables(0).Rows(i).Item("fld74").ToString() & "|")
                        SBData.Append(DsUser.Tables(0).Rows(i).Item("fld75").ToString() & "|")
                        SBData.Append(DsUser.Tables(0).Rows(i).Item("fld76").ToString() & "|")
                        SBData.Append(DsUser.Tables(0).Rows(i).Item("fld77").ToString() & "|")
                        SBData.Append(DsUser.Tables(0).Rows(i).Item("fld78").ToString() & "|")
                        SBData.Append(DsUser.Tables(0).Rows(i).Item("fld79").ToString() & "|")
                        SBData.Append(DsUser.Tables(0).Rows(i).Item("fld80").ToString() & "|")
                        SBData.Append(DsUser.Tables(0).Rows(i).Item("fld81").ToString() & "|")
                        SBData.Append(DsUser.Tables(0).Rows(i).Item("fld82").ToString() & "|")
                        SBData.Append(DsUser.Tables(0).Rows(i).Item("fld83").ToString() & "|")
                        SBData.Append(DsUser.Tables(0).Rows(i).Item("fld84").ToString() & "|")
                        SBData.Append(DsUser.Tables(0).Rows(i).Item("fld85").ToString() & "|")
                        SBData.Append(DsUser.Tables(0).Rows(i).Item("fld86").ToString() & "|")
                        SBData.Append(DsUser.Tables(0).Rows(i).Item("fld87").ToString() & "|")
                        SBData.Append(DsUser.Tables(0).Rows(i).Item("fld88").ToString() & "|")
                        SBData.Append(DsUser.Tables(0).Rows(i).Item("fld89").ToString() & "|")
                        SBData.Append(DsUser.Tables(0).Rows(i).Item("fld90").ToString())
                    Next
                    result = SBData.ToString()
                End If
            Else
                result = "Sorry!!! Your Authantication failed."
            End If
        Catch ex As Exception
            Return "RTO"
        End Try
        Return result
    End Function

    Function Syncacknowledgement(Key As String, EID As String, UID As Integer, IMINum As String) As String Implements IBPMMobileSyncAPI.Syncacknowledgement
        Dim ret As String = ""
        Dim objDev As New DeviceInfo()
        Dim result As Integer = 0
        Try
            objDev.IMINumber = IMINum.Substring(0, 15)
            Dim IsValidrequest = False
            Dim dsD As New DataSet()
            dsD = objDev.ValidateDevice(EID, UID)
            objDev.IMINumber = IMINum
            If dsD.Tables(0).Rows.Count > 0 Then
                IsValidrequest = True
            End If
            If IsValidrequest = True Then
                result = objDev.UpdateMobDeviceInfo(EID)
                If result > 0 Then
                    ret = "OK"
                Else
                    ret = "Fail"
                End If
            Else
                ret = "Sorry !!! Authantication failed."
            End If

        Catch ex As Exception
            Return "Fail"
        End Try
        Return ret
    End Function

    Public Function ChangePassword1(key As String, EID As String, UserId As String, Password As String) As String
        Dim ret As String = ""
        Dim ds As New DataSet()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStrLive").ConnectionString
        Try
            If key = BPMKEY Then
                'Code For getting UserDetails

            Else
                ret = "Invalid key supplyed."
            End If
        Catch ex As Exception
            Return "RTO"
        End Try
        Return ret
    End Function

    Public Function ChangePassword(key As String, ByVal EID As String, ByVal UID As Integer, ByVal cpwd As String, ByVal npwd As String) As String Implements IBPMMobileSyncAPI.ChangePassword
        Dim sRes As String = ""
        Dim ret As String = ""
        Dim con As SqlConnection = Nothing
        Dim oda As SqlDataAdapter = Nothing

        Try
            If key = BPMKEY Then
                'Code For getting UserDetails
                con = New SqlConnection(conStr)
                Dim sqlq As String
                sqlq = "Select minchar,maxchar,passtype from MMM_MST_ENTITY where EID=" & EID.ToString()
                oda = New SqlDataAdapter(sqlq, con)
                Dim ds As New DataSet
                oda.Fill(ds, "setting")
                Dim objU As New User()

                If npwd.Length() < Val(ds.Tables("setting").Rows(0).Item("minchar").ToString()) Then
                    ret = "0,Password must be " & ds.Tables("setting").Rows(0).Item("minchar").ToString() & " Character Long"
                    Return ret
                End If

                If npwd.Length() > Val(ds.Tables("setting").Rows(0).Item("maxchar").ToString()) Then
                    ret = "0,Password must not be greater than " & ds.Tables("setting").Rows(0).Item("maxchar").ToString() & " Character Long."
                    Return ret
                End If
                Select Case ds.Tables("setting").Rows(0).Item("passtype").ToString()
                    Case "NORMAL"
                        ' do nothing
                    Case "ALPHA NUMERIC"
                        If Not objU.isAlphaNumeric(npwd) Then
                            ret = "0,Password must be alphanumeric "
                            Return ret
                        End If
                        'must be alphanumeric
                    Case "ALPHA NUMERIC WITH CAPS LETTER"
                        'must be alphanumeric with one Capital letter
                        If Not objU.isAlphaNumericAndCapital(npwd) Then
                            ret = "0,Password must be alphanumeric and contains atleast one capital letter "
                            Return ret
                        End If
                    Case "ALPHA NUMERIC WITH SPECIAL CHARACTER"
                        If Not objU.isAlphaNumericAndSpecial(npwd) Then
                            ret = "0,Password must be alphanumeric and must contain one special character "
                            Return ret
                        End If
                End Select
                sqlq = "SELECT * FROM MMM_MST_USER where uid=" & UID & " AND EID= " & EID
                oda.SelectCommand.CommandText = sqlq
                oda.Fill(ds, "user")
                Dim i As Integer
                i = ds.Tables("user").Rows.Count
                If i = 1 Then
                    Dim sKey As String = ds.Tables("user").Rows(0).Item("sKey").ToString()
                    Dim sPwd As String = objU.DecryptTripleDES(ds.Tables("user").Rows(0).Item("pwd"), sKey)
                    Dim newPwd As String = npwd
                    If sPwd = cpwd Then
                        Dim ssKey As Integer
                        Dim Generator As System.Random = New System.Random()
                        sKey = Generator.Next(10000, 99999)
                        Dim strPwd As String = objU.EncryptTripleDES(npwd, ssKey)
                        'Password matched. now changed the password
                        oda.SelectCommand.CommandText = "UPDATE MMM_MST_USER SET pwd='" & strPwd & "',modifydate=getdate(),sKey='" & ssKey & "' where uid=" & UID
                        oda.SelectCommand.Parameters.Clear()
                        If con.State <> ConnectionState.Open Then
                            con.Open()
                        End If
                        oda.SelectCommand.ExecuteNonQuery()
                        ret = "1,Password changed successfully"
                        Return ret
                    Else
                        'password(doesn) 't matched
                        ret = "0,Password Doesn't matched"
                        Return ret
                    End If
                Else
                    ret = "0,User Not Found"
                    Return ret
                End If
            Else
                ret = "0,Invalid key supplyed."
                Return ret
            End If

        Catch ex As Exception
            Return "0,RTO"
        Finally
            If Not con Is Nothing Then
                con.Close()
                con.Dispose()
            End If
            If Not oda Is Nothing Then
                oda.Dispose()
            End If
        End Try
    End Function

    Public Function ForgotPassword(ByVal UserName As String, ByVal ECODE As String) As String Implements IBPMMobileSyncAPI.ForgotPassword
        Dim ret = ""
        Dim IsValidated = True
        Try
            If String.IsNullOrEmpty(UserName.Trim()) Then
                IsValidated = False
                ret = "User Name required."
            End If
            If String.IsNullOrEmpty(ECODE.Trim()) Then
                IsValidated = False
                ret = "Entity required."
            End If
            If IsValidated = True Then
                Dim obj As New User()
                'Dim ECODE = ""
                'Code For Removing sql Injection 
                'EID = EID.Replace("--", "").Replace("''", "")
                'Dim Query = "SELECT Code FROM MMM_MST_ENTITY WHERE EID=" & EID
                'Dim DS As New DataSet()
                'ECODE = DS.Tables(0).Rows(0).Item("Code")
                ret = obj.MyeDmsPasswordRecover(UserName, ECODE)
            End If
        Catch ex As Exception
            Return "RTO"
        End Try
        'RESULT = obj.MyeDmsPasswordRecover(txtEmailName.Text, txtFEntityName.Text)
        Return ret
    End Function

    Function GetDocCalandar(Key As String, EID As String, UID As Integer, URole As String, IMINum As String, ST As String) As List(Of DocCalandar) Implements IBPMMobileSyncAPI.GetDocCalandar

        Dim lstD As New List(Of DocCalandar)
        Dim obj As New DocCalandar()
        Dim ret = "NEW"
        Try
            If BPMKEY = Key Then
                lstD = obj.GetDocCalandar(EID, URole, IMINum, UID, ST)
            End If
        Catch ex As Exception
            Throw New FaultException("RTO")
        End Try
        Return lstD
    End Function

    Function GetChildCalandar(Key As String, EID As String, UID As Integer, URole As String, IMINum As String, ST As String) As List(Of ChildCalandar) Implements IBPMMobileSyncAPI.GetChildCalandar

        Dim lstD As New List(Of ChildCalandar)
        Dim obj As New ChildCalandar()
        Try
            If BPMKEY = Key Then
                lstD = obj.GetChildCalandar(EID, URole, IMINum, UID, ST)
            End If
        Catch ex As Exception
            Throw New FaultException("RTO")
        End Try
        Return lstD
    End Function

    Function GetCalandar(Key As String, EID As String, UID As Integer, URole As String, IMINum As String, ST As String) As List(Of Calandar) Implements IBPMMobileSyncAPI.GetCalandar

        Dim lstD As New List(Of Calandar)
        Dim obj As New Calandar()
        Try
            If BPMKEY = Key Then
                lstD = obj.GetCalandar(EID, URole, IMINum, UID, ST)
            End If
        Catch ex As Exception
            Throw New FaultException("RTO")
        End Try
        Return lstD
    End Function

    Function GetDashBoard(Key As String, EID As String, UID As Integer, URole As String, IMINum As String, ST As String) As List(Of DashBoard) Implements IBPMMobileSyncAPI.GetDashBoard
        Dim lstD As New List(Of DashBoard)
        Dim obj As New DashBoard()
        Try
            If BPMKEY = Key Then
                lstD = obj.GetDashBoard(EID, URole, IMINum, UID, ST)
            End If
        Catch ex As Exception
            Throw New FaultException("RTO")
        End Try
        Return lstD
    End Function

    Function GetWorkFlowSetting(Key As String, EID As String, UID As Integer, IMINum As String, ST As String) As XElement Implements IBPMMobileSyncAPI.GetWorkFlowSetting
        Dim ds As New DataSet()
        Dim objDev As New DeviceInfo()
        Dim ret = "NEW"
        Dim SB As New StringBuilder("<workflowstatus>")
        Dim doc As New XDocument()
        Try
            Dim IsValidrequest = False
            Dim dsD As New DataSet()
            objDev.IMINumber = IMINum
            dsD = objDev.ValidateDevice(EID, UID)
            If dsD.Tables(0).Rows.Count > 0 Then
                IsValidrequest = True
                If Not ST.ToUpper() = "COMPLETE" Then
                    ret = dsD.Tables(0).Rows(0).Item("LastUpdatedOn").ToString()
                Else
                    ret = "1900-01-01 00:00:00.000"
                End If
            End If
            If BPMKEY = Key And IsValidrequest = True Then
                ds = GetWorkFlowStatus(EID:=EID, LastUpdated:=ret)
                If ds.Tables(0).Rows.Count > 0 Then
                    For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                        SB.Append("<Rows>")
                        SB.Append("<TID>").Append(Convert.ToString(ds.Tables(0).Rows(i).Item("TID"))).Append("</TID>")
                        SB.Append("<EID>").Append(Convert.ToString(ds.Tables(0).Rows(i).Item("EID"))).Append("</EID>")
                        SB.Append("<Dord>").Append(Convert.ToString(ds.Tables(0).Rows(i).Item("Dord"))).Append("</Dord>")
                        SB.Append("<StatusName>").Append(Convert.ToString(ds.Tables(0).Rows(i).Item("StatusName"))).Append("</StatusName>")
                        SB.Append("<Documenttype>").Append(Convert.ToString(ds.Tables(0).Rows(i).Item("Documenttype"))).Append("</Documenttype>")
                        SB.Append("<isauth>").Append(Convert.ToString(ds.Tables(0).Rows(i).Item("isauth"))).Append("</isauth>")
                        SB.Append("<Approve>").Append(Convert.ToString(ds.Tables(0).Rows(i).Item("Approve"))).Append("</Approve>")
                        SB.Append("<Reject>").Append(Convert.ToString(ds.Tables(0).Rows(i).Item("Reject"))).Append("</Reject>")
                        SB.Append("<Reconsider>").Append(Convert.ToString(ds.Tables(0).Rows(i).Item("Reconsider"))).Append("</Reconsider>")
                        SB.Append("<RejectStatus>").Append(Convert.ToString(ds.Tables(0).Rows(i).Item("RejectStatus"))).Append("</RejectStatus>")
                        SB.Append("<Amendment>").Append(Convert.ToString(ds.Tables(0).Rows(i).Item("Amendment"))).Append("</Amendment>")
                        SB.Append("<Recall>").Append(Convert.ToString(ds.Tables(0).Rows(i).Item("Recall"))).Append("</Recall>")
                        SB.Append("<Cancel>").Append(Convert.ToString(ds.Tables(0).Rows(i).Item("Cancel"))).Append("</Cancel>")
                        SB.Append("<ManagebyotherRole>").Append(Convert.ToString(ds.Tables(0).Rows(i).Item("ManagebyotherRole"))).Append("</ManagebyotherRole>")
                        SB.Append("<isallowskip>").Append(Convert.ToString(ds.Tables(0).Rows(i).Item("isallowskip"))).Append("</isallowskip>")
                        SB.Append("<RoleName>").Append(Convert.ToString(ds.Tables(0).Rows(i).Item("RoleName"))).Append("</RoleName>")
                        SB.Append("<Edit>").Append(Convert.ToString(ds.Tables(0).Rows(i).Item("Edit"))).Append("</Edit>")
                        SB.Append("<Split>").Append(Convert.ToString(ds.Tables(0).Rows(i).Item("Split"))).Append("</Split>")
                        SB.Append("<curDocStatus>").Append(Convert.ToString(ds.Tables(0).Rows(i).Item("curDocStatus"))).Append("</curDocStatus>")
                        SB.Append("<NewDocStatus>").Append(Convert.ToString(ds.Tables(0).Rows(i).Item("NewDocStatus"))).Append("</NewDocStatus>")
                        SB.Append("<aprovaltype>").Append(Convert.ToString(ds.Tables(0).Rows(i).Item("aprovaltype"))).Append("</aprovaltype>")
                        SB.Append("<expirydate>").Append(Convert.ToString(ds.Tables(0).Rows(i).Item("expirydate"))).Append("</expirydate>")
                        SB.Append("<afterhours>").Append(Convert.ToString(ds.Tables(0).Rows(i).Item("afterhours"))).Append("</afterhours>")
                        SB.Append("<Copy>").Append(Convert.ToString(ds.Tables(0).Rows(i).Item("Copy"))).Append("</Copy>")
                        SB.Append("<autostatus>").Append(Convert.ToString(ds.Tables(0).Rows(i).Item("autostatus"))).Append("</autostatus>")
                        SB.Append("<autoaction>").Append(Convert.ToString(ds.Tables(0).Rows(i).Item("autoaction"))).Append("</autoaction>")
                        SB.Append("<autonextstatus>").Append(Convert.ToString(ds.Tables(0).Rows(i).Item("autonextstatus"))).Append("</autonextstatus>")
                        SB.Append("<isactive>").Append(Convert.ToString(ds.Tables(0).Rows(i).Item("isactive"))).Append("</isactive>")
                        SB.Append("<remarks>").Append(Convert.ToString(ds.Tables(0).Rows(i).Item("remarks"))).Append("</remarks>")
                        SB.Append("<allowprint>").Append(Convert.ToString(ds.Tables(0).Rows(i).Item("allowprint"))).Append("</allowprint>")
                        SB.Append("</Rows>")
                    Next
                End If
            End If
        Catch ex As Exception
            Throw New FaultException("RTO")
        End Try
        SB.Append("</workflowstatus>")
        Dim StrXml As String = SB.ToString.Replace("&", "&amp;")
        doc = XDocument.Parse(StrXml)
        Return doc.Root
    End Function

    Function GetEntityInfo(Key As String, EID As String, UID As Integer, IMINum As String, ST As String) As XElement Implements IBPMMobileSyncAPI.GetEntityInfo
        Dim ds As New DataSet()
        Dim objDev As New DeviceInfo()
        Dim ret = "NEW"
        Dim SB As New StringBuilder("<Entity>")
        Dim doc As New XDocument()
        Try
            Dim IsValidrequest = False
            Dim dsD As New DataSet()
            objDev.IMINumber = IMINum
            dsD = objDev.ValidateDevice(EID, UID)
            If dsD.Tables(0).Rows.Count > 0 Then
                IsValidrequest = True
                If Not ST.ToUpper() = "COMPLETE" Then
                    ret = dsD.Tables(0).Rows(0).Item("LastUpdatedOn").ToString()
                Else
                    ret = "1900-01-01 00:00:00.000"
                End If
            End If
            If BPMKEY = Key Then
                ds = GetEntityInfo(EID:=EID, LastUpdated:=ret)
                If ds.Tables(0).Rows.Count > 0 Then
                    SB.Append("<EID>").Append(Convert.ToString(ds.Tables(0).Rows(0).Item("EID"))).Append("</EID>")
                    SB.Append("<Code>").Append(Convert.ToString(ds.Tables(0).Rows(0).Item("Code"))).Append("</Code>")
                    SB.Append("<Name>").Append(Convert.ToString(ds.Tables(0).Rows(0).Item("Name"))).Append("</Name>")
                    SB.Append("<isAuth>").Append(Convert.ToString(ds.Tables(0).Rows(0).Item("isAuth"))).Append("</isAuth>")
                    SB.Append("<AccountType>").Append(Convert.ToString(ds.Tables(0).Rows(0).Item("AccountType"))).Append("</AccountType>")
                    SB.Append("<CreatedDate>").Append(Convert.ToString(ds.Tables(0).Rows(0).Item("CreatedDate"))).Append("</CreatedDate>")
                    SB.Append("<addsticker>").Append(Convert.ToString(ds.Tables(0).Rows(0).Item("addsticker"))).Append("</addsticker>")
                    SB.Append("<domainname>").Append(Convert.ToString(ds.Tables(0).Rows(0).Item("domainname"))).Append("</domainname>")
                    SB.Append("<URL>").Append(Convert.ToString(ds.Tables(0).Rows(0).Item("URL"))).Append("</URL>")
                    SB.Append("<theme>").Append(Convert.ToString(ds.Tables(0).Rows(0).Item("theme"))).Append("</theme>")
                    SB.Append("<layout>").Append(Convert.ToString(ds.Tables(0).Rows(0).Item("layout"))).Append("</layout>")
                    SB.Append("<pVisit>").Append(Convert.ToString(ds.Tables(0).Rows(0).Item("pVisit"))).Append("</pVisit>")
                    SB.Append("<webHead>").Append(Convert.ToString(ds.Tables(0).Rows(0).Item("webHead"))).Append("</webHead>")
                    SB.Append("<logo>").Append(Convert.ToString(ds.Tables(0).Rows(0).Item("logo"))).Append("</logo>")
                    SB.Append("<footer>").Append(Convert.ToString(ds.Tables(0).Rows(0).Item("footer"))).Append("</footer>")
                    SB.Append("<UserID>").Append(Convert.ToString(ds.Tables(0).Rows(0).Item("UserID"))).Append("</UserID>")
                    SB.Append("<localfilesystem>").Append(Convert.ToString(ds.Tables(0).Rows(0).Item("localfilesystem"))).Append("</localfilesystem>")
                    SB.Append("<minchar>").Append(Convert.ToString(ds.Tables(0).Rows(0).Item("minchar"))).Append("</minchar>")
                    SB.Append("<maxchar>").Append(Convert.ToString(ds.Tables(0).Rows(0).Item("maxchar"))).Append("</maxchar>")
                    SB.Append("<isWorkFlow>").Append(Convert.ToString(ds.Tables(0).Rows(0).Item("isWorkFlow"))).Append("</isWorkFlow>")
                    SB.Append("<WorkFlowType>").Append(Convert.ToString(ds.Tables(0).Rows(0).Item("WorkFlowType"))).Append("</WorkFlowType>")
                    SB.Append("<headerImage>").Append(Convert.ToString(ds.Tables(0).Rows(0).Item("headerImage"))).Append("</headerImage>")
                    SB.Append("<headerstrip>").Append(Convert.ToString(ds.Tables(0).Rows(0).Item("headerstrip"))).Append("</headerstrip>")
                    SB.Append("<UVDType>").Append(Convert.ToString(ds.Tables(0).Rows(0).Item("UVDType"))).Append("</UVDType>")
                    SB.Append("<UVUserField>").Append(Convert.ToString(ds.Tables(0).Rows(0).Item("UVUserField"))).Append("</UVUserField>")
                    SB.Append("<UVVehicleField>").Append(Convert.ToString(ds.Tables(0).Rows(0).Item("UVVehicleField"))).Append("</UVVehicleField>")
                    SB.Append("<VIDType>").Append(Convert.ToString(ds.Tables(0).Rows(0).Item("VIDType"))).Append("</VIDType>")
                    SB.Append("<VIVehicleField>").Append(Convert.ToString(ds.Tables(0).Rows(0).Item("VIVehicleField"))).Append("</VIVehicleField>")
                    SB.Append("<VIImeiField>").Append(Convert.ToString(ds.Tables(0).Rows(0).Item("VIImeiField"))).Append("</VIImeiField>")
                    SB.Append("<isgpsactivated>").Append(Convert.ToString(ds.Tables(0).Rows(0).Item("isgpsactivated"))).Append("</isgpsactivated>")
                    SB.Append("<mapType>").Append(Convert.ToString(ds.Tables(0).Rows(0).Item("mapType"))).Append("</mapType>")
                    SB.Append("<APIKey>").Append(Convert.ToString(ds.Tables(0).Rows(0).Item("APIKey"))).Append("</APIKey>")
                    SB.Append("<UVStartDateTime>").Append(Convert.ToString(ds.Tables(0).Rows(0).Item("UVStartDateTime"))).Append("</UVStartDateTime>")
                    SB.Append("<UVEndDateTime>").Append(Convert.ToString(ds.Tables(0).Rows(0).Item("UVEndDateTime"))).Append("</UVEndDateTime>")
                    SB.Append("<CurStatus>").Append(Convert.ToString(ds.Tables(0).Rows(0).Item("CurStatus"))).Append("</CurStatus>")
                    SB.Append("<VIDriverNamefield>").Append(Convert.ToString(ds.Tables(0).Rows(0).Item("VIDriverNamefield"))).Append("</VIDriverNamefield>")
                    SB.Append("<VIDrivermnofield>").Append(Convert.ToString(ds.Tables(0).Rows(0).Item("VIDrivermnofield"))).Append("</VIDrivermnofield>")
                    SB.Append("<AppType>").Append(Convert.ToString(ds.Tables(0).Rows(0).Item("AppType"))).Append("</AppType>")
                    SB.Append("<wserrorEmail>").Append(Convert.ToString(ds.Tables(0).Rows(0).Item("wserrorEmail"))).Append("</wserrorEmail>")
                    SB.Append("<SMSAlertMNo>").Append(Convert.ToString(ds.Tables(0).Rows(0).Item("SMSAlertMNo"))).Append("</SMSAlertMNo>")
                    SB.Append("<MailAlertID>").Append(Convert.ToString(ds.Tables(0).Rows(0).Item("MailAlertID"))).Append("</MailAlertID>")
                    SB.Append("<ReloadSeconds>").Append(Convert.ToString(ds.Tables(0).Rows(0).Item("ReloadSeconds"))).Append("</ReloadSeconds>")
                    SB.Append("<defaultpage>").Append(Convert.ToString(ds.Tables(0).Rows(0).Item("defaultpage"))).Append("</defaultpage>")
                    SB.Append("<Startmonth>").Append(Convert.ToString(ds.Tables(0).Rows(0).Item("Startmonth"))).Append("</Startmonth>")
                    SB.Append("<Endmonth>").Append(Convert.ToString(ds.Tables(0).Rows(0).Item("Endmonth"))).Append("</Endmonth>")
                    SB.Append("<FtpFolder>").Append(Convert.ToString(ds.Tables(0).Rows(0).Item("FtpFolder"))).Append("</FtpFolder>")
                    SB.Append("<MapHomePage>").Append(Convert.ToString(ds.Tables(0).Rows(0).Item("MapHomePage"))).Append("</MapHomePage>")
                    SB.Append("<MapReportPage>").Append(Convert.ToString(ds.Tables(0).Rows(0).Item("MapReportPage"))).Append("</MapReportPage>")
                    SB.Append("<CheckDeviceReg>").Append(Convert.ToString(ds.Tables(0).Rows(0).Item("CheckDeviceReg"))).Append("</CheckDeviceReg>")

                    SB.Append("<EnableMap>").Append(Convert.ToString(ds.Tables(0).Rows(0).Item("EnableMap"))).Append("</EnableMap>")
                    SB.Append("<EnableTracking>").Append(Convert.ToString(ds.Tables(0).Rows(0).Item("EnableTracking"))).Append("</EnableTracking>")
                    SB.Append("<angle>").Append(Convert.ToString(ds.Tables(0).Rows(0).Item("angle"))).Append("</angle>")
                    SB.Append("<Time>").Append(Convert.ToString(ds.Tables(0).Rows(0).Item("Time"))).Append("</Time>")
                    SB.Append("<Distance>").Append(Convert.ToString(ds.Tables(0).Rows(0).Item("Distance"))).Append("</Distance>")

                    SB.Append("<mbmMarkin>").Append(Convert.ToString(ds.Tables(0).Rows(0).Item("mbmMarkin"))).Append("</mbmMarkin>")
                    SB.Append("<mbmMarkout>").Append(Convert.ToString(ds.Tables(0).Rows(0).Item("mbmMarkout"))).Append("</mbmMarkout>")
                End If
            End If
        Catch ex As Exception
            Throw New FaultException("RTO")
        End Try
        SB.Append("</Entity>")
        doc = XDocument.Parse(SB.ToString)
        Return doc.Root
    End Function

    Function SyncCRMDATA(Key As String, UID As String, EID As String, URole As String, IMINum As String, ST As String) As String Implements IBPMMobileSyncAPI.SyncCRMDATA

        Try
            Dim result As String = ""
            Dim ret = ""
            Dim con As SqlConnection = Nothing
            Dim da As SqlDataAdapter = Nothing
            Dim DocQuery As New StringBuilder()
            Dim DocH As New StringBuilder()
            Dim MstQuery As New StringBuilder()
            Dim ChildQuery As New StringBuilder()
            Dim lstData As New List(Of DocData)
            Dim objDev As New DeviceInfo()
            Dim SBData As New StringBuilder()
            Dim IsValidrequest = False
            Try
                con = New SqlConnection(conStr)
                objDev.IMINumber = IMINum
                Dim dsD = New DataSet()
                dsD = objDev.ValidateDevice(EID, UID)
                If dsD.Tables(0).Rows.Count > 0 Then
                    IsValidrequest = True
                    If Not ST.ToUpper() = "COMPLETE" Then
                        ret = dsD.Tables(0).Rows(0).Item("LastUpdatedOn").ToString()
                    Else
                        ret = "1900-01-01 12:25:25.010"
                    End If
                End If
                If (Key = BPMKEY) And IsValidrequest = True Then
                    'Dim FinalQuery = MstQuery.ToString() & ";" & DocQuery.ToString() & ";" & ChildQuery.ToString()
                    'Appending Current Bucket Query to main Document Query

                    DocQuery.Append("select * FROM MMM_MST_CRM with (nolock) WHERE EID= " & EID & " and DOCID in(Select M.tid  from MMM_MST_DOC M with (nolock)  LEFT OUTER JOIN MMM_DOC_DTL D with (nolock) on D.tid=M.lasttid where D.userid = " & UID & "  AND EID=" & EID & ")")
                    DocH.Append(" union ALl select * FROM MMM_MST_CRM with (nolock) WHERE EID= " & EID & " and DOCID in(Select  M.tid from MMM_MST_DOC M with (nolock)  LEFT OUTER JOIN MMM_DOC_DTL D with (nolock) on D.docid =M.tid  LEFT OUTER JOIN MMM_MST_USER U with (nolock) ON U.uid=oUID LEFT OUTER JOIN MMM_MST_USER AP with (nolock) ON AP.uid=D.UserID where M.EID=" & EID & " AND  D.UserID = " & UID & " and aprstatus is not null and m.curstatus <> 'ARCHIVE' )")
                    Dim FinalQuery = DocQuery.ToString() & " AND LastUpdate>='" & ret & "'" & " " & DocH.ToString() & " AND LastUpdate>='" & ret & "'"
                    If con.State <> ConnectionState.Open Then
                        con.Open()
                    End If
                    da = New SqlDataAdapter(FinalQuery, con)
                    Dim dsData As New DataSet()
                    da.Fill(dsData)

                    If dsData.Tables.Count > 0 Then
                        For i As Integer = 0 To dsData.Tables.Count - 1
                            If dsData.Tables(i).Rows.Count > 0 Then
                                For j As Integer = 0 To dsData.Tables(i).Rows.Count - 1
                                    SBData.Append("::")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("tid").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("DOCID").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("USERID").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("ADate").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("CurStatus").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("Ordering").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("Remark").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("SMSNo").ToString() & "|")

                                    SBData.Append(dsData.Tables(i).Rows(j).Item("S_MailID").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("T_MailID").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("Communication_MSG").ToString() & "|")

                                    'EID	DOCID	USERID	ADate	CurStatus	Ordering	Remark	SMSNo	
                                    'S_MailID	T_MailID	Communication_MSG
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld1").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld2").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld3").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld4").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld5").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld6").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld7").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld8").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld9").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld10").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld11").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld12").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld13").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld14").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld15").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld16").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld17").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld18").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld19").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld20").ToString() & "|")

                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld21").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld22").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld23").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld24").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld25").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld26").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld27").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld28").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld29").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld30").ToString() & "|")

                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld31").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld32").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld33").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld34").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld35").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld36").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld37").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld38").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld39").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld40").ToString() & "|")

                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld41").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld42").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld43").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld44").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld45").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld46").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld47").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld48").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld49").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld50").ToString() & "|")

                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld51").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld52").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld53").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld54").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld55").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld56").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld57").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld58").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld59").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld60").ToString() & "|")


                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld61").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld62").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld63").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld64").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld65").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld66").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld67").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld68").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld69").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld70").ToString() & "|")

                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld71").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld72").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld73").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld74").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld75").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld76").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld77").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld78").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld79").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld80").ToString() & "|")

                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld81").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld82").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld83").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld84").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld85").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld86").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld87").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld88").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld89").ToString() & "|")
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("fld90").ToString())
                                Next
                            End If
                        Next
                    End If
                    result = SBData.ToString()
                Else
                    result = "Sorry!!! Your Authantication failed."
                End If

            Catch ex As Exception
                Throw New FaultException("RTO")
            Finally
                If Not con Is Nothing Then
                    con.Close()
                    con.Dispose()
                End If
                If Not da Is Nothing Then
                    da.Dispose()
                End If
            End Try
            Try

                Return result
            Catch ex As Exception
                Throw New FaultException(ex.Message)
            End Try
        Catch ex As Exception
            Throw New FaultException(ex.Message)
        End Try

    End Function
    Function GetEntityInfo(EID As Integer, LastUpdated As String) As DataSet
        Dim ds As New DataSet()
        Try
            If (LastUpdated = "NEW") Then
                LastUpdated = "1900-01-01 00:00:00.000"
            End If
            Using con As SqlConnection = New SqlConnection(conStr)
                Using da As New SqlDataAdapter("GetEntityInfo", con)
                    da.SelectCommand.CommandType = CommandType.StoredProcedure
                    da.SelectCommand.Parameters.AddWithValue("@EID", EID)
                    da.SelectCommand.Parameters.AddWithValue("@lsatUpdate", LastUpdated)
                    da.Fill(ds)
                End Using
            End Using
        Catch ex As Exception
            Throw
        End Try
        Return ds
    End Function

    Function GetWorkFlowStatus(EID As Integer, LastUpdated As String) As DataSet
        Dim ds As New DataSet()
        Try
            Using con As SqlConnection = New SqlConnection(conStr)
                Using da As New SqlDataAdapter("getWorkFlowStatus", con)
                    da.SelectCommand.CommandType = CommandType.StoredProcedure
                    da.SelectCommand.Parameters.AddWithValue("@EID", EID)
                    da.SelectCommand.Parameters.AddWithValue("@LastUpdate", LastUpdated)
                    da.Fill(ds)
                End Using
            End Using
        Catch ex As Exception
            Throw
        End Try
        Return ds
    End Function

    Function GetCreatedDocumentByDocumenttype(Eid As Integer, DocType As String, UID As Integer, TimeStamp As String) As arr3 Implements IBPMMobileSyncAPI.GetCreatedDocumentByDocumenttype
        Dim da As SqlDataAdapter = Nothing
        Dim con As SqlConnection = Nothing
        Dim Sb As New StringBuilder()
        Dim Sb1 As New StringBuilder()
        Dim query As String = ""
        Dim StrQuery1 As String
        Dim jsonData As String = ""
        Dim lstarr4 As New arr3()
        Dim QueryFormtype As String = ""
        Try
            'QueryFormtype = "SELECT FormType FROM MMM_MST_FORMS WHERE EID =" & Eid & " AND FormName ='" & DocType & "'"

            query = "SELECT * from MMM_MST_FIELDS where eid= " & Eid & " and DocumentType='" & DocType & "' AND ShowOnDocDetail =1  SELECT FormType,ISNULL(DocMapping,'') DocMapping FROM MMM_MST_FORMS WHERE EID =" & Eid & " AND FormName ='" & DocType & "'"
            con = New SqlConnection(conStr)
            da = New SqlDataAdapter(query, con)
            Dim ds As New DataSet()
            da.Fill(ds)
            Dim StrViewName = "[" & "V" & Eid & DocType.Replace(" ", "_") & "] V"
            If (ds.Tables(0).Rows.Count > 0) Then
                For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                    If ds.Tables(0).Rows(i).Item("FieldType") = "Drop Down" And ds.Tables(0).Rows(i).Item("DropDownType") = "MASTER VALUED" Then
                        Dim arr = ds.Tables(0).Rows(i).Item("DropDown").ToString().Split("-")
                        Dim str = ""
                        Dim str1 = ""
                        If arr(1).ToUpper = "USER" Then
                            str1 = "(SELECT isnull([" & arr(2) & "],'') from MMM_MST_USER s WHERE CAST(s.[UID] AS VARCHAR)=CAST(V.[" & ds.Tables(0).Rows(i).Item("DisplayName") & "] AS VARCHAR))[" & ds.Tables(0).Rows(i).Item("DisplayName") & "]"
                        Else
                            str = "SELECT DisplayName FROM MMM_MST_FIELDS WHERE EID=" & Eid & "AND Documenttype='" & arr(1) & "' AND FieldMapping='" & arr(2) & "'"
                            con = New SqlConnection(conStr)
                            da = New SqlDataAdapter(str, con)
                            Dim ds1 As New DataSet()
                            da.Fill(ds1)
                            If (ds1.Tables(0).Rows.Count > 0) Then
                                str1 = "(SELECT isnull([" & ds1.Tables(0).Rows(0).Item("DisplayName") & "],'')  from [V" & Eid & arr(1).Replace(" ", "_") & "] s WHERE CAST(s.tid AS VARCHAR)=CAST(V.[" & ds.Tables(0).Rows(i).Item("DisplayName") & "] AS VARCHAR))[" & ds.Tables(0).Rows(i).Item("DisplayName") & "]"
                            End If
                        End If
                        If Sb1.ToString().Contains("<" & str1 & ">") Then
                        Else
                            If (str1 <> "") Then
                                Sb.Append(",").Append("<" & str1 & ">")
                            End If
                        End If
                    Else
                        If Sb1.ToString().Contains("<isnull(V.[" & ds.Tables(0).Rows(i).Item("DisplayName") & "],'')'" & ds.Tables(0).Rows(i).Item("DisplayName") & "'>") Then
                        Else
                            Sb.Append(",").Append("<isnull(V.[" & ds.Tables(0).Rows(i).Item("DisplayName") & "],'')'" & ds.Tables(0).Rows(i).Item("DisplayName") & "'>")
                        End If
                    End If
                Next
                StrQuery1 = Sb.ToString.Substring(1, Sb.ToString.Length - 1).Replace("<", "").Replace(">", "")

                StrQuery1 = "SELECT distinct V.tid As 'DOCID', " & StrQuery1 & "FROM " & StrViewName
                If ds.Tables(1).Rows(0)("FormType") = "MASTER" Then
                    StrQuery1 = StrQuery1 & " INNER JOIN MMM_MST_DOC D ON V.TID=D.tid WHERE V.EID=" & Eid & "  and V.DocumentType='" & DocType & "' and Cast(D.LastUpdate AS Date) >= cast( '" & TimeStamp & "' as date)"
                Else
                    StrQuery1 = StrQuery1 & " INNER JOIN MMM_MST_DOC D ON V.TID=D.tid INNER JOIN MMM_DOC_DTL DTL ON DTL.docid =D.tid WHERE V.EID=" & Eid & " AND V.OUId=" & UID & " and V.DocumentType='" & DocType & "' and Cast(D.LastUpdate AS Date) >= cast( '" & TimeStamp & "' as date)"
                End If
                If ds.Tables(1).Rows(0)("DocMapping") <> "" Then
                    StrQuery1 = StrQuery1 & " and V.TID IN (select * from  InputString((select " & ds.Tables(1).Rows(0)("DocMapping") & " from mmm_ref_role_user with(nolock) where eid=" & Eid & " and uid=" & UID & " and rolename ='Auditor')) )"
                End If
                StrQuery1 = StrQuery1 & " order by V.tid desc"
                Dim dsData As New DataSet()
                da = New SqlDataAdapter(StrQuery1, con)
                da.Fill(dsData)
                For j As Integer = 0 To dsData.Tables(0).Rows.Count - 1
                    Dim lstarr2 As New List(Of arr1)
                    Dim lstarr3 As New arr2()
                    For Each dc As DataColumn In dsData.Tables(0).Columns
                        Dim objarr1 As New arr1()
                        objarr1.DisplayName = dc.ColumnName
                        If String.IsNullOrEmpty(Convert.ToString(dsData.Tables(0).Rows(j).Item(dc.ColumnName))) Then
                            objarr1.Value = ""
                        Else
                            objarr1.Value = Convert.ToString(dsData.Tables(0).Rows(j)(dc.ColumnName))
                        End If
                        lstarr2.Add(objarr1)
                    Next
                    lstarr3.items2 = lstarr2
                    lstarr4.items1.Add(lstarr3)
                Next

                'jsonData = JsonConvert.SerializeObject(dsData.Tables(0))
            End If

        Catch ex As Exception
        Finally
            If Not con Is Nothing Then
                con.Close()
                con.Dispose()
            End If
            If Not da Is Nothing Then
                da.Dispose()
            End If
        End Try
        Return lstarr4
    End Function


    'Function GetCreatedDocumentByDocumenttype(Eid As Integer, DocType As String, UID As Integer, TimeStamp As String) As arr3 Implements IBPMMobileSyncAPI.GetCreatedDocumentByDocumenttype
    '    Dim da As SqlDataAdapter = Nothing
    '    Dim con As SqlConnection = Nothing
    '    Dim Sb As New StringBuilder()
    '    Dim Sb1 As New StringBuilder()
    '    Dim query As String = ""
    '    Dim StrQuery1 As String
    '    Dim jsonData As String = ""
    '    Dim lstarr4 As New arr3()
    '    Try
    '        query = "SELECT * from MMM_MST_FIELDS where eid= " & Eid & " and DocumentType='" & DocType & "' AND ShowOnDocDetail =1"
    '        con = New SqlConnection(conStr)
    '        da = New SqlDataAdapter(query, con)
    '        Dim ds As New DataSet()
    '        da.Fill(ds)
    '        Dim StrViewName = "[" & "V" & Eid & DocType.Replace(" ", "_") & "] D"
    '        If (ds.Tables(0).Rows.Count > 0) Then
    '            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
    '                If ds.Tables(0).Rows(i).Item("FieldType") = "Drop Down" And ds.Tables(0).Rows(i).Item("DropDownType") = "MASTER VALUED" Then
    '                    Dim arr = ds.Tables(0).Rows(i).Item("DropDown").ToString().Split("-")
    '                    Dim str = ""
    '                    Dim str1 = ""
    '                    If arr(1).ToUpper = "USER" Then
    '                        str1 = "(SELECT isnull([" & arr(2) & "],'') from MMM_MST_USER s WHERE CAST(s.[UID] AS VARCHAR)=CAST(D.[" & ds.Tables(0).Rows(i).Item("DisplayName") & "] AS VARCHAR))[" & ds.Tables(0).Rows(i).Item("DisplayName") & "]"
    '                    Else
    '                        str = "SELECT DisplayName FROM MMM_MST_FIELDS WHERE EID=" & Eid & "AND Documenttype='" & arr(1) & "' AND FieldMapping='" & arr(2) & "'"
    '                        con = New SqlConnection(conStr)
    '                        da = New SqlDataAdapter(str, con)
    '                        Dim ds1 As New DataSet()
    '                        da.Fill(ds1)
    '                        str1 = "(SELECT isnull([" & ds1.Tables(0).Rows(0).Item("DisplayName") & "],'')  from [V" & Eid & arr(1).Replace(" ", "_") & "] s WHERE CAST(s.tid AS VARCHAR)=CAST(D.[" & ds.Tables(0).Rows(i).Item("DisplayName") & "] AS VARCHAR))[" & ds.Tables(0).Rows(i).Item("DisplayName") & "]"
    '                    End If
    '                    If Sb1.ToString().Contains("<" & str1 & ">") Then
    '                    Else
    '                        Sb.Append(",").Append("<" & str1 & ">")
    '                    End If
    '                Else
    '                    If Sb1.ToString().Contains("<isnull(D.[" & ds.Tables(0).Rows(i).Item("DisplayName") & "],'')'" & ds.Tables(0).Rows(i).Item("DisplayName") & "'>") Then
    '                    Else
    '                        Sb.Append(",").Append("<isnull(D.[" & ds.Tables(0).Rows(i).Item("DisplayName") & "],'')'" & ds.Tables(0).Rows(i).Item("DisplayName") & "'>")
    '                    End If
    '                End If
    '            Next
    '            StrQuery1 = Sb.ToString.Substring(1, Sb.ToString.Length - 1).Replace("<", "").Replace(">", "")

    '            StrQuery1 = "SELECT D.tid As 'DOCID', " & StrQuery1 & "FROM " & StrViewName

    '            StrQuery1 = StrQuery1 & " INNER JOIN MMM_MST_DOC DTL ON D.TID=DTL.tid WHERE D.EID=" & Eid & " AND D.OUId=" & UID & " and D.DocumentType='" & DocType & "' and Cast(DTL.LastUpdate AS Date) >= cast( '" & TimeStamp & "' as date)"
    '            StrQuery1 = StrQuery1 & "order by D.tid desc"
    '            Dim dsData As New DataSet()
    '            da = New SqlDataAdapter(StrQuery1, con)
    '            da.Fill(dsData)
    '            For j As Integer = 0 To dsData.Tables(0).Rows.Count - 1
    '                Dim lstarr2 As New List(Of arr1)
    '                Dim lstarr3 As New arr2()
    '                For Each dc As DataColumn In dsData.Tables(0).Columns
    '                    Dim objarr1 As New arr1()
    '                    objarr1.DisplayName = dc.ColumnName
    '                    If String.IsNullOrEmpty(Convert.ToString(dsData.Tables(0).Rows(0).Item(dc.ColumnName))) Then
    '                        objarr1.Value = ""
    '                    Else
    '                        objarr1.Value = Convert.ToString(dsData.Tables(0).Rows(j)(dc.ColumnName))
    '                    End If
    '                    lstarr2.Add(objarr1)
    '                Next
    '                lstarr3.items2 = lstarr2
    '                lstarr4.items1.Add(lstarr3)
    '            Next

    '            'jsonData = JsonConvert.SerializeObject(dsData.Tables(0))
    '        End If
    '    Catch ex As Exception
    '    Finally
    '        If Not con Is Nothing Then
    '            con.Close()
    '            con.Dispose()
    '        End If
    '        If Not da Is Nothing Then
    '            da.Dispose()
    '        End If
    '    End Try
    '    Return lstarr4
    'End Function

    Function GetCreatedDocumentByDocumenttype1(Eid As Integer, DocType As String, UID As Integer, TimeStamp As String, Filter As String, AuthKey As String) As arr3 Implements IBPMMobileSyncAPI.GetCreatedDocumentByDocumenttype1
        Dim da As SqlDataAdapter = Nothing
        Dim con As SqlConnection = Nothing
        Dim Sb As New StringBuilder()
        Dim Sb1 As New StringBuilder()
        Dim query As String = ""
        Dim StrQuery1 As String
        Dim jsonData As String = ""
        Dim lstarr4 As New arr3()
        Dim QueryFormtype As String = ""
        Try
            If (BPMKEY = AuthKey) Then
                'QueryFormtype = "SELECT FormType FROM MMM_MST_FORMS WHERE EID =" & Eid & " AND FormName ='" & DocType & "'"

                query = "SELECT * from MMM_MST_FIELDS where eid= " & Eid & " and DocumentType='" & DocType & "' AND ShowOnDocDetail =1  SELECT FormType FROM MMM_MST_FORMS WHERE EID =" & Eid & " AND FormName ='" & DocType & "'"
                con = New SqlConnection(conStr)
                da = New SqlDataAdapter(query, con)
                Dim ds As New DataSet()
                da.Fill(ds)
                Dim StrViewName = "[" & "V" & Eid & DocType.Replace(" ", "_") & "] V"
                If (ds.Tables(0).Rows.Count > 0) Then
                    For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                        If ds.Tables(0).Rows(i).Item("FieldType") = "Drop Down" And ds.Tables(0).Rows(i).Item("DropDownType") = "MASTER VALUED" Then
                            Dim arr = ds.Tables(0).Rows(i).Item("DropDown").ToString().Split("-")
                            Dim str = ""
                            Dim str1 = ""
                            If arr(1).ToUpper = "USER" Then
                                str1 = "(SELECT isnull([" & arr(2) & "],'') from MMM_MST_USER s WHERE CAST(s.[UID] AS VARCHAR)=CAST(V.[" & ds.Tables(0).Rows(i).Item("DisplayName") & "] AS VARCHAR))[" & ds.Tables(0).Rows(i).Item("DisplayName") & "]"
                            Else
                                str = "SELECT DisplayName FROM MMM_MST_FIELDS WHERE EID=" & Eid & "AND Documenttype='" & arr(1) & "' AND FieldMapping='" & arr(2) & "'"
                                con = New SqlConnection(conStr)
                                da = New SqlDataAdapter(str, con)
                                Dim ds1 As New DataSet()
                                da.Fill(ds1)
                                str1 = "(SELECT isnull([" & ds1.Tables(0).Rows(0).Item("DisplayName") & "],'')  from [V" & Eid & arr(1).Replace(" ", "_") & "] s WHERE CAST(s.tid AS VARCHAR)=CAST(V.[" & ds.Tables(0).Rows(i).Item("DisplayName") & "] AS VARCHAR))[" & ds.Tables(0).Rows(i).Item("DisplayName") & "]"
                            End If
                            If Sb1.ToString().Contains("<" & str1 & ">") Then
                            Else
                                Sb.Append(",").Append("<" & str1 & ">")
                            End If
                        Else
                            If Sb1.ToString().Contains("<isnull(V.[" & ds.Tables(0).Rows(i).Item("DisplayName") & "],'')'" & ds.Tables(0).Rows(i).Item("DisplayName") & "'>") Then
                            Else
                                Sb.Append(",").Append("<isnull(V.[" & ds.Tables(0).Rows(i).Item("DisplayName") & "],'')'" & ds.Tables(0).Rows(i).Item("DisplayName") & "'>")
                            End If
                        End If
                    Next
                    StrQuery1 = Sb.ToString.Substring(1, Sb.ToString.Length - 1).Replace("<", "").Replace(">", "")

                    StrQuery1 = "SELECT distinct V.tid As 'DOCID', " & StrQuery1 & "FROM " & StrViewName
                    If ds.Tables(1).Rows(0)("FormType") = "MASTER" Then
                        StrQuery1 = StrQuery1 & " INNER JOIN MMM_MST_DOC D ON V.TID=D.tid WHERE V.EID=" & Eid & "  and V.DocumentType='" & DocType & "' and Cast(D.LastUpdate AS Date) >= cast( '" & TimeStamp & "' as date)"
                    Else
                        StrQuery1 = StrQuery1 & " INNER JOIN MMM_MST_DOC D ON V.TID=D.tid INNER JOIN MMM_DOC_DTL DTL ON DTL.docid =D.tid WHERE V.EID=" & Eid & " AND V.OUId=" & UID & " and V.DocumentType='" & DocType & "' and Cast(D.LastUpdate AS Date) >= cast( '" & TimeStamp & "' as date)"
                    End If
                    If Not String.IsNullOrEmpty(Filter) Then
                        Dim flt As String() = Filter.Split("|")
                        If (flt.Length > 0) Then
                            For k As Integer = 0 To flt.Length - 1
                                Dim flt1 As String() = flt(k).Split("::")
                                StrQuery1 = StrQuery1 & "AND " & flt1(0) & "= '" + flt1(2) + "'"
                            Next
                        End If
                    End If
                    StrQuery1 = StrQuery1 & " order by V.tid desc"
                    Dim dsData As New DataSet()
                    da = New SqlDataAdapter(StrQuery1, con)
                    da.Fill(dsData)
                    For j As Integer = 0 To dsData.Tables(0).Rows.Count - 1
                        Dim lstarr2 As New List(Of arr1)
                        Dim lstarr3 As New arr2()
                        For Each dc As DataColumn In dsData.Tables(0).Columns
                            Dim objarr1 As New arr1()
                            objarr1.DisplayName = dc.ColumnName
                            If String.IsNullOrEmpty(Convert.ToString(dsData.Tables(0).Rows(j).Item(dc.ColumnName))) Then
                                objarr1.Value = ""
                            Else
                                objarr1.Value = Convert.ToString(dsData.Tables(0).Rows(j)(dc.ColumnName))
                            End If
                            lstarr2.Add(objarr1)
                        Next
                        lstarr3.items2 = lstarr2
                        lstarr4.items1.Add(lstarr3)
                    Next

                    'jsonData = JsonConvert.SerializeObject(dsData.Tables(0))
                End If
            Else
                Dim objarr1 As New arr1()
                Dim lstarr2 As New List(Of arr1)
                Dim lstarr3 As New arr2()
                objarr1.DisplayName = "Error"
                objarr1.Value = "Invalid AuthKey."
                lstarr2.Add(objarr1)
                lstarr3.items2 = lstarr2
                lstarr4.items1.Add(lstarr3)
            End If

        Catch ex As Exception
        Finally
            If Not con Is Nothing Then
                con.Close()
                con.Dispose()
            End If
            If Not da Is Nothing Then
                da.Dispose()
            End If
        End Try
        Return lstarr4
    End Function


    Function PHRO_EmployeeRegistration(IMINumber As String, Mobile_Number As String, Emp_Code As String, Comp_Code As String, AdminAppr_Status As String, Email_ID As String) As String Implements IBPMMobileSyncAPI.PHRO_EmployeeRegistration
        Dim result As String = ""
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim cmd As SqlCommand = New SqlCommand("AddPHRO_Employee", con)
        cmd.CommandType = CommandType.StoredProcedure
        Try
            Dim OTP As String = RandomString(4, 1)
            cmd.Parameters.AddWithValue("@IMINumber", IMINumber)
            cmd.Parameters.AddWithValue("@Mobile_Number", Mobile_Number)
            cmd.Parameters.AddWithValue("@Emp_Code", Emp_Code)
            cmd.Parameters.AddWithValue("@Comp_Code", Comp_Code)
            cmd.Parameters.AddWithValue("@AdminAppr_Status", AdminAppr_Status)
            cmd.Parameters.AddWithValue("@Email_ID", Email_ID)
            cmd.Parameters.AddWithValue("@OTP", OTP)
            con.Open()
            result = cmd.ExecuteScalar.ToString()
            If result = "notexists" Then
                sendMail(Email_ID, "", "", "Pocket HRO registration", "<p>Pocket HRO registration successfully.<br/>Your OTP: " + OTP + "</p>")
                result = "Pocket HRO registration successfully."
            Else
                result = "Pocket HRO registration alreay exists."
            End If
        Catch ex As Exception
            result = "Sorry. Error occurred in server. Please try again."
        Finally
            con.Close()
            con.Dispose()
            cmd.Dispose()
        End Try
        Return result
    End Function

    Function PHRO_EmployeeVerifiedOTP(IMINumber As String, Emp_Code As String, Comp_Code As String, Email_ID As String, OTP As String) As String Implements IBPMMobileSyncAPI.PHRO_EmployeeVerifiedOTP
        Dim result As String = ""
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim cmd As SqlCommand = New SqlCommand("VerifiedPHRO_EmployeeOTP", con)
        cmd.CommandType = CommandType.StoredProcedure
        Dim objsndMail As New DMSUtil()
        Try
            cmd.CommandType = CommandType.StoredProcedure
            cmd.Parameters.AddWithValue("@IMINumber", IMINumber)
            cmd.Parameters.AddWithValue("@Emp_Code", Emp_Code)
            cmd.Parameters.AddWithValue("@Comp_Code", Comp_Code)
            cmd.Parameters.AddWithValue("@Email_ID", Email_ID)
            cmd.Parameters.AddWithValue("@OTP", OTP)
            con.Open()
            result = cmd.ExecuteScalar.ToString()

            If result = "verifiedOTP" Then
                result = "Pocket HRO registration OTP verified successfully."
            Else
                result = "Rong OTP."
            End If
        Catch ex As Exception
            result = "Sorry. Error occurred in server. Please try again."
        Finally
            con.Close()
            con.Dispose()
            cmd.Dispose()
        End Try
        Return result
    End Function

    Public Sub sendMail(ByVal Mto As String, ByVal cc As String, ByVal bcc As String, ByVal MSubject As String, ByVal MBody As String)
        'Dim Email As New System.Net.Mail.MailMessage("no-reply@myndsol.com", "manish@myndsol.com", MSubject, MBody & Mto)
        Try
            If Left(Mto, 1) = "{" Then
                Exit Sub
            End If
            Dim Email As New System.Net.Mail.MailMessage("no-reply@myndsol.com", Mto, MSubject, MBody)
            Dim mailClient As New System.Net.Mail.SmtpClient()
            Email.IsBodyHtml = True
            If cc <> "" Then
                Email.CC.Add(cc)
            End If

            If bcc <> "" Then
                Email.Bcc.Add(bcc)
            End If
            Dim basicAuthenticationInfo As New System.Net.NetworkCredential("no-reply@myndsol.com", "vc79aK123AJ&$kL0")
            mailClient.Host = "mail.myndsol.com"
            mailClient.UseDefaultCredentials = False
            mailClient.Credentials = basicAuthenticationInfo
            'mailClient.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.PickupDirectoryFromIis
            Try
                mailClient.Send(Email)
            Catch ex As Exception
                Exit Sub
            End Try
        Catch ex As Exception
            Exit Sub
        End Try
    End Sub
    Private Function RandomString(size As Integer, lowerCase As Boolean) As String
        Dim builder As New StringBuilder()
        Dim random As New Random()
        Dim ch As Char
        For i As Integer = 0 To size - 1
            ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)))
            builder.Append(ch)
        Next
        If lowerCase Then
            Return builder.ToString().ToLower()
        End If
        Return builder.ToString()
    End Function


    Function GetAssetss(Eid As Integer, UID As Integer, TimeStamp As String) As arr4 Implements IBPMMobileSyncAPI.GetAssetss
        Dim da As SqlDataAdapter = Nothing
        Dim con As SqlConnection = Nothing
        Dim Sb As New StringBuilder()
        Dim Sb1 As New StringBuilder()
        Dim query As String = ""
        Dim StrVerifiedQuery As String
        Dim StrNotVerifiedQuery As String
        Dim StrTotalQuery As String
        Dim StrNewQuery As String
        Dim jsonData As String = ""
        Dim lstarr4 As New arr3()
        Dim QueryFormtype As String = ""
        Dim TotalCount As Integer = 0
        Dim VerifiedCount As Integer = 0
        Dim NewAssetCount As Integer = 0
        Dim NotVerifiedCount As Integer = 0
        Dim lstarr3 As New arr2()
        Dim objarr1 As New arr4()
        Try
            StrTotalQuery = "SELECT COUNT(*) TotlaCount from V123Physical_verification where eid= " & Eid & " and createdBy = " & UID & " and cast(UpdatedDate as Date) = Cast('" & TimeStamp & "' as Date)"
            StrVerifiedQuery = "SELECT  COUNT(*) VerifiedCunt FROM V123Physical_verification V INNER JOIN V123Asset_Master VM ON Cast(V.[Asset Master ID] as INT) = VM.tid WHERE V.eid= " & Eid & " AND V.createdBy = " & UID & " and CAST(V.UpdatedDate AS Date) = Cast('" & TimeStamp & "' as Date)"
            StrNewQuery = "SELECT COUNT(*) NewCount from V123Physical_verification where eid= " & Eid & " and createdBy = " & UID & " and cast(UpdatedDate as Date) = Cast('" & TimeStamp & "' as Date) and ISNULL([Asset Master ID],'') = ''"

            con = New SqlConnection(conStr)
            da = New SqlDataAdapter(StrTotalQuery + StrVerifiedQuery + StrNewQuery, con)
            Dim ds As New DataSet()
            da.Fill(ds)
            TotalCount = Convert.ToInt32(ds.Tables(0).Rows(0)("TotlaCount"))
            VerifiedCount = Convert.ToInt32(ds.Tables(1).Rows(0)("VerifiedCunt"))
            NewAssetCount = Convert.ToInt32(ds.Tables(2).Rows(0)("NewCount"))
            NotVerifiedCount = (TotalCount - VerifiedCount)

            Dim lstarr2 As New List(Of arr1)

            objarr1.TotalAssetCount = TotalCount
            objarr1.VerifiedAssetCount = VerifiedCount
            objarr1.NewAssetCount = NewAssetCount
            objarr1.NotVerifiedAssetCount = NotVerifiedCount

        Catch ex As Exception

        End Try
        Return objarr1
    End Function
End Class

Public Class arr1
    Public Property DisplayName As String
    Public Property Value As String

End Class
Public Class arr2
    Public items2 As New List(Of arr1)
End Class
Public Class arr3
    Public items1 As New List(Of arr2)
End Class

Public Class arr4
    Public Property TotalAssetCount As String
    Public Property VerifiedAssetCount As String
    Public Property NewAssetCount As String
    Public Property NotVerifiedAssetCount As String
End Class


<DataContractAttribute(Namespace:="", Name:="DashBoard")>
Public Class DashBoard
    Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
    'Tid	Eid	DBName	DBType	WidgetType	DBWidth	Dorder	WidgetNature	RootQuery	FirstLevelQuery	SecondLevelQuery	Status	Roles	CellPosition
    <DataMember(Name:="Tid", Order:=1)> _
    Public Property Tid As String

    <DataMember(Name:="Eid", Order:=2)> _
    Public Property Eid As String

    <DataMember(Name:="DBName", Order:=3)> _
    Public Property DBName As String

    <DataMember(Name:="DBType", Order:=4)> _
    Public Property DBType As String

    <DataMember(Name:="WidgetType", Order:=5)> _
    Public Property WidgetType As String

    <DataMember(Name:="DBWidth", Order:=6)> _
    Public Property DBWidth As String

    <DataMember(Name:="Dorder", Order:=7)> _
    Public Property Dorder As String

    <DataMember(Name:="WidgetNature", Order:=8)> _
    Public Property WidgetNature As String

    <DataMember(Name:="RootQuery", Order:=9)> _
    Public Property RootQuery As String

    <DataMember(Name:="FirstLevelQuery", Order:=10)> _
    Public Property FirstLevelQuery As String

    <DataMember(Name:="SecondLevelQuery", Order:=11)> _
    Public Property SecondLevelQuery As String

    <DataMember(Name:="Status", Order:=12)> _
    Public Property Status As String

    <DataMember(Name:="Roles", Order:=13)> _
    Public Property Roles As String

    <DataMember(Name:="CellPosition", Order:=14)> _
    Public Property CellPosition As String

    Public Function GetDashBoard(EID As Integer, URole As String, IMEINum As String, UID As Integer, ST As String) As List(Of DashBoard)
        Dim objDev As New DeviceInfo()
        Dim Lst As New List(Of DashBoard)
        Dim IsValidrequest As Boolean = False
        Dim dsDash As New DataSet()
        Dim ret = "1900-01-01 00:00:00"
        Try
            objDev.IMINumber = IMEINum
            Dim dsD = New DataSet()
            dsD = objDev.ValidateDevice(EID, UID)
            If dsD.Tables(0).Rows.Count > 0 Then
                IsValidrequest = True
                If Not ST.ToUpper() = "COMPLETE" Then
                    ret = dsD.Tables(0).Rows(0).Item("LastUpdatedOn").ToString()
                Else
                    ret = "1900-01-01 00:00"
                End If
                dsDash = GetDashBoard(EID, URole, ret)
                Dim obj As DashBoard
                For i As Integer = 0 To dsDash.Tables(0).Rows.Count - 1
                    obj = New DashBoard()
                    obj.Tid = dsDash.Tables(0).Rows(i).Item("Tid")
                    obj.Eid = dsDash.Tables(0).Rows(i).Item("EID")
                    obj.DBName = dsDash.Tables(0).Rows(i).Item("DBName")
                    obj.DBType = dsDash.Tables(0).Rows(i).Item("DBType")
                    obj.WidgetType = dsDash.Tables(0).Rows(i).Item("WidgetType")
                    obj.DBWidth = dsDash.Tables(0).Rows(i).Item("DBWidth")
                    obj.WidgetNature = dsDash.Tables(0).Rows(i).Item("WidgetNature")
                    obj.RootQuery = dsDash.Tables(0).Rows(i).Item("RootQuery")
                    obj.FirstLevelQuery = dsDash.Tables(0).Rows(i).Item("FirstLevelQuery")
                    obj.SecondLevelQuery = dsDash.Tables(0).Rows(i).Item("SecondLevelQuery")
                    obj.Status = dsDash.Tables(0).Rows(i).Item("Status")
                    obj.Roles = dsDash.Tables(0).Rows(i).Item("EID")
                    obj.CellPosition = dsDash.Tables(0).Rows(i).Item("CellPosition")
                    Lst.Add(obj)
                Next
            End If
        Catch ex As Exception

        End Try
        Return Lst
    End Function

    Private Function GetDashBoard(EID As Integer, URole As String, LastUpdate As String) As DataSet
        Dim ds As New DataSet()
        Try
            Using con = New SqlConnection(conStr)
                Using da = New SqlDataAdapter("GetDashBoard", con)
                    da.SelectCommand.CommandType = CommandType.StoredProcedure
                    da.SelectCommand.Parameters.AddWithValue("@EID", EID)
                    da.SelectCommand.Parameters.AddWithValue("@Roles", URole)
                    da.SelectCommand.Parameters.AddWithValue("@LastUpdate", LastUpdate)
                    da.Fill(ds)
                End Using
            End Using
        Catch ex As Exception
            Throw
        End Try
        Return ds
    End Function
End Class

<DataContractAttribute(Namespace:="", Name:="Calandar")>
Public Class Calandar
    Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
    'Tid	Eid	DBName	DBType	WidgetType	DBWidth	Dorder	WidgetNature	RootQuery	FirstLevelQuery	SecondLevelQuery	Status	Roles	CellPosition
    <DataMember(Name:="Tid", Order:=1)> _
    Public Property Tid As String

    <DataMember(Name:="Eid", Order:=2)> _
    Public Property Eid As String

    <DataMember(Name:="Name", Order:=3)> _
    Public Property Name As String

    <DataMember(Name:="EnableDashBoard", Order:=4)> _
    Public Property EnableDashBoard As String


    Public Function GetCalandar(EID As Integer, URole As String, IMEINum As String, UID As Integer, ST As String) As List(Of Calandar)

        Dim objDev As New DeviceInfo()
        Dim Lst As New List(Of Calandar)
        Dim IsValidrequest As Boolean = False
        Dim dsCal As New DataSet()
        Dim ret = "1900-01-01 00:00:00"
        Try
            objDev.IMINumber = IMEINum
            Dim dsD = New DataSet()
            dsD = objDev.ValidateDevice(EID, UID)
            If dsD.Tables(0).Rows.Count > 0 Then
                IsValidrequest = True
                If Not ST.ToUpper() = "COMPLETE" Then
                    ret = dsD.Tables(0).Rows(0).Item("LastUpdatedOn").ToString()
                Else
                    ret = "1900-01-01 00:00"
                End If
                dsCal = GetCalandar(EID, URole, ret)
                Dim obj As Calandar
                For i As Integer = 0 To dsCal.Tables(0).Rows.Count - 1
                    obj = New Calandar()
                    obj.Tid = dsCal.Tables(0).Rows(i).Item("Tid")
                    obj.Eid = dsCal.Tables(0).Rows(i).Item("EID")
                    obj.Name = dsCal.Tables(0).Rows(i).Item("Name")
                    obj.EnableDashBoard = dsCal.Tables(0).Rows(i).Item("EnableDashBoard")
                    Lst.Add(obj)
                Next
            End If
        Catch ex As Exception

        End Try
        Return Lst
    End Function

    Private Function GetCalandar(EID As Integer, URole As String, LastUpdate As String) As DataSet
        Dim ds As New DataSet()
        Try
            Using con = New SqlConnection(conStr)
                Using da = New SqlDataAdapter("GetCalandar", con)
                    da.SelectCommand.CommandType = CommandType.StoredProcedure
                    da.SelectCommand.Parameters.AddWithValue("@EID", EID)
                    da.SelectCommand.Parameters.AddWithValue("@Roles", URole)
                    da.SelectCommand.Parameters.AddWithValue("@LastUpdate", LastUpdate)
                    da.Fill(ds)
                End Using
            End Using
        Catch ex As Exception
            Throw
        End Try
        Return ds
    End Function
End Class

<DataContractAttribute(Namespace:="", Name:="DocCalandar")>
Public Class DocCalandar
    Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
    'Tid,Eid,doctype,usrfield,startdate,enddate,Nameusrfield,Namestartdate,Nameenddate,icon,textdatatype,textdata,textdataname,tooltiptype,	tooltip,tooltipname
    <DataMember(Name:="Tid", Order:=1)> _
    Public Property Tid As String

    <DataMember(Name:="Eid", Order:=2)> _
    Public Property Eid As String

    <DataMember(Name:="doctype", Order:=3)> _
    Public Property Doctype As String

    <DataMember(Name:="usrfield", Order:=4)> _
    Public Property UserField As String

    <DataMember(Name:="startdate", Order:=5)> _
    Public Property StartDate As String


    <DataMember(Name:="enddate", Order:=6)> _
    Public Property EndDate As String


    <DataMember(Name:="Nameusrfield", Order:=7)> _
    Public Property Nameusrfield As String

    <DataMember(Name:="Namestartdate", Order:=8)> _
    Public Property Namestartdate As String

    <DataMember(Name:="Nameenddate", Order:=9)> _
    Public Property Nameenddate As String


    <DataMember(Name:="icon", Order:=10)> _
    Public Property icon As String


    <DataMember(Name:="textdatatype", Order:=11)> _
    Public Property textdatatype As String


    <DataMember(Name:="textdataname", Order:=12)> _
    Public Property textdataname As String

    <DataMember(Name:="tooltiptype", Order:=13)> _
    Public Property tooltiptype As String

    <DataMember(Name:="tooltip", Order:=14)> _
    Public Property tooltip As String

    <DataMember(Name:="tooltipname", Order:=15)> _
    Public Property tooltipname As String

    <DataMember(Name:="textdata", Order:=16)> _
    Public Property textdata As String


    Public Function GetDocCalandar(EID As Integer, URole As String, IMEINum As String, UID As Integer, ST As String) As List(Of DocCalandar)

        Dim objDev As New DeviceInfo()
        Dim Lst As New List(Of DocCalandar)
        Dim IsValidrequest As Boolean = False
        Dim dsCal As New DataSet()
        Dim ret = "1900-01-01 00:00:00"
        Try
            objDev.IMINumber = IMEINum
            Dim dsD = New DataSet()
            dsD = objDev.ValidateDevice(EID, UID)
            If dsD.Tables(0).Rows.Count > 0 Then
                IsValidrequest = True
                If Not ST.ToUpper() = "COMPLETE" Then
                    ret = dsD.Tables(0).Rows(0).Item("LastUpdatedOn").ToString()
                Else
                    ret = "1900-01-01 00:00"
                End If
                dsCal = GetDOCCAL(EID, URole, ret)
                Dim obj As DocCalandar
                For i As Integer = 0 To dsCal.Tables(0).Rows.Count - 1
                    obj = New DocCalandar()
                    'Tid,Eid,doctype,usrfield,startdate,enddate,Nameusrfield,Namestartdate,Nameenddate,icon,textdatatype,textdata,textdataname,tooltiptype,	tooltip,tooltipname
                    obj.Tid = dsCal.Tables(0).Rows(i).Item("Tid")
                    obj.Eid = dsCal.Tables(0).Rows(i).Item("EID")
                    obj.Doctype = dsCal.Tables(0).Rows(i).Item("doctype")
                    obj.UserField = dsCal.Tables(0).Rows(i).Item("usrfield")
                    obj.StartDate = dsCal.Tables(0).Rows(i).Item("startdate")
                    obj.EndDate = dsCal.Tables(0).Rows(i).Item("EID")
                    obj.Nameusrfield = dsCal.Tables(0).Rows(i).Item("Nameusrfield")
                    obj.Namestartdate = dsCal.Tables(0).Rows(i).Item("Namestartdate")
                    obj.Nameenddate = dsCal.Tables(0).Rows(i).Item("Nameenddate")
                    obj.textdatatype = dsCal.Tables(0).Rows(i).Item("textdatatype")
                    obj.textdataname = dsCal.Tables(0).Rows(i).Item("textdataname")
                    obj.tooltiptype = dsCal.Tables(0).Rows(i).Item("tooltiptype")
                    obj.tooltip = dsCal.Tables(0).Rows(i).Item("tooltip")
                    obj.tooltipname = dsCal.Tables(0).Rows(i).Item("tooltipname")
                    Lst.Add(obj)
                Next
            End If
        Catch ex As Exception

        End Try
        Return Lst
    End Function

    Private Function GetDOCCAL(EID As Integer, URole As String, LastUpdate As String) As DataSet
        Dim ds As New DataSet()
        Try
            Using con = New SqlConnection(conStr)
                Using da = New SqlDataAdapter("GetDOCCAL", con)
                    da.SelectCommand.CommandType = CommandType.StoredProcedure
                    da.SelectCommand.Parameters.AddWithValue("@EID", EID)
                    da.SelectCommand.Parameters.AddWithValue("@Roles", URole)
                    da.SelectCommand.Parameters.AddWithValue("@LastUpdate", LastUpdate)
                    da.Fill(ds)
                End Using
            End Using
        Catch ex As Exception
            Throw
        End Try
        Return ds
    End Function
End Class

<DataContractAttribute(Namespace:="", Name:="ChildCalandar")>
Public Class ChildCalandar
    Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
    'Tid,ParentID,CalId,Eid,Status,dorder
    <DataMember(Name:="Tid", Order:=1)> _
    Public Property Tid As String

    <DataMember(Name:="Eid", Order:=2)> _
    Public Property Eid As String

    <DataMember(Name:="ParentID", Order:=3)> _
    Public Property ParentID As String

    <DataMember(Name:="CalId", Order:=4)> _
    Public Property CalId As String

    <DataMember(Name:="Status", Order:=5)> _
    Public Property Status As String

    <DataMember(Name:="dorder", Order:=5)> _
    Public Property dorder As String


    Public Function GetChildCalandar(EID As Integer, URole As String, IMEINum As String, UID As Integer, ST As String) As List(Of ChildCalandar)

        Dim objDev As New DeviceInfo()
        Dim Lst As New List(Of ChildCalandar)
        Dim IsValidrequest As Boolean = False
        Dim dsCal As New DataSet()
        Dim ret = "1900-01-01 00:00:00"
        Try
            objDev.IMINumber = IMEINum
            Dim dsD = New DataSet()
            dsD = objDev.ValidateDevice(EID, UID)
            If dsD.Tables(0).Rows.Count > 0 Then
                IsValidrequest = True
                If Not ST.ToUpper() = "COMPLETE" Then
                    ret = dsD.Tables(0).Rows(0).Item("LastUpdatedOn").ToString()
                Else
                    ret = "1900-01-01 00:00"
                End If
                dsCal = GetChildCalandar(EID, URole, ret)
                Dim obj As ChildCalandar
                For i As Integer = 0 To dsCal.Tables(0).Rows.Count - 1
                    obj = New ChildCalandar()
                    'Tid,ParentID,CalId,Eid,Status,dorder
                    obj.Tid = dsCal.Tables(0).Rows(i).Item("Tid")
                    obj.Eid = dsCal.Tables(0).Rows(i).Item("EID")
                    obj.ParentID = dsCal.Tables(0).Rows(i).Item("ParentID")
                    obj.CalId = dsCal.Tables(0).Rows(i).Item("CalId")
                    obj.Status = dsCal.Tables(0).Rows(i).Item("Status")
                    obj.dorder = dsCal.Tables(0).Rows(i).Item("dorder")
                    Lst.Add(obj)
                Next
            End If
        Catch ex As Exception

        End Try
        Return Lst
    End Function

    Private Function GetChildCalandar(EID As Integer, URole As String, LastUpdate As String) As DataSet
        Dim ds As New DataSet()
        Try
            Using con = New SqlConnection(conStr)
                Using da = New SqlDataAdapter("GetCalandar", con)
                    da.SelectCommand.CommandType = CommandType.StoredProcedure
                    da.SelectCommand.Parameters.AddWithValue("@EID", EID)
                    da.SelectCommand.Parameters.AddWithValue("@Roles", URole)
                    da.SelectCommand.Parameters.AddWithValue("@LastUpdate", LastUpdate)
                    da.Fill(ds)
                End Using
            End Using
        Catch ex As Exception
            Throw
        End Try
        Return ds
    End Function



End Class



