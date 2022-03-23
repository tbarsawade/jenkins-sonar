Imports System.Data
Imports System.Data.SqlClient
Imports System.Net
Imports System.IO
Imports System.Xml.Serialization
Imports System.Runtime.Serialization
Imports System.ServiceModel
Imports System.ServiceModel.Web
Imports System.Xml

' NOTE: You can use the "Rename" command on the context menu to change the class name "BPMMobileOffline" in code, svc and config file together.
Public Class BPMMobileOffline
    Implements IBPMMobileOffline

    Dim conStr As String = ConfigurationManager.ConnectionStrings("conStrLive").ConnectionString

    Dim BPMKEY As String = ConfigurationManager.AppSettings("BPMKEY").ToString()



    Function SyncData(Key As String, UserID As String, EID As String, URole As String, IMINum As String, ST As String) As String Implements IBPMMobileOffline.SyncData
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
            Try
                con = New SqlConnection(conStr)
                If (Key = BPMKEY) Then
                    'Getting all the form name
                    Dim FormQuery = "select * FROM MMM_MST_FORMS where EID=" & EID & " AND FormType in('DOCUMENT','MASTER')"
                    If con.State <> ConnectionState.Open Then
                        con.Open()
                    End If
                    da = New SqlDataAdapter(FormQuery, con)
                    Dim dsForm As New DataSet()
                    da.Fill(dsForm)
                    'Getting last synk Date

                    objDev.IMINumber = IMINum
                    If Not ST.ToUpper() = "COMPLETE" Then
                        ret = objDev.CheckDevice(EID)
                    Else
                        ret = "NEW"
                    End If

                    'Creating Query
                    If dsForm.Tables(0).Rows.Count > 0 Then
                        For f As Integer = 0 To dsForm.Tables(0).Rows.Count - 1
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
                                Dim Where = UserDataFilter_PreRole(dsForm.Tables(0).Rows(f).Item("FormName"), "MMM_MST_MASTER", EID, URole, UserID)
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

                                Dim Where = UserDataFilter_PreRole(dsForm.Tables(0).Rows(f).Item("FormName"), "MMM_MST_DOC", EID, URole, UserID)

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
                    Dim onlyFiltered As DataView = dsData.Tables(0).DefaultView
                    onlyFiltered.RowFilter = "DocumentType='" & "Enquiry Master" & "' "
                    Dim theFlds As DataTable = onlyFiltered.Table.DefaultView.ToTable()
                    Dim onlyFiltered1 As DataView = dsData.Tables(1).DefaultView
                    onlyFiltered1.RowFilter = "DocumentType='" & "Enquiry Master" & "' "
                    Dim theFlds1 As DataTable = onlyFiltered1.Table.DefaultView.ToTable()

                    If dsData.Tables.Count = 2 Then
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
                                    SBData.Append(dsData.Tables(i).Rows(j).Item("OUID").ToString())
                                Next
                            End If
                        Next
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
            Try
                result = SBData.ToString()
                Return result
            Catch ex As Exception
                Throw New FaultException(ex.Message)
            End Try
        Catch ex As Exception
            Throw New FaultException(ex.Message)
        End Try
    End Function


    Function SyncUser(Key As String, EID As String, IMINum As String, ST As String) As String Implements IBPMMobileOffline.SyncUser
        Dim result = ""
        Dim objDev As New DeviceInfo()
        Dim ret = "NEW"
        Try
            If BPMKEY = Key Then
                objDev.IMINumber = IMINum
                If Not ST.ToUpper() = "COMPLETE" Then
                    ret = objDev.CheckDevice(EID)
                Else
                    ret = "1900-01-01 00:00:00.000"
                End If
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
                result = "Invalid key supplyed."
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

    Function GetDataOFAllForm(Key As String, EID As String, URole As String, IMINum As String, ST As String) As List(Of FormData) Implements IBPMMobileOffline.GetDataOFAllForm
        Dim lstFlds As New List(Of FormData)
        Dim dsfld As New DataSet()
        Dim objFD As FormData
        Dim con As SqlConnection = Nothing
        Dim da As SqlDataAdapter = Nothing
        Dim objDev As New DeviceInfo()
        Dim ret = ""
        Dim Query = ""
        Try
            'Dim BPMKEY As String = ConfigurationManager.AppSettings("BPMKEY").ToString()

            objDev.IMINumber = IMINum
            'ret = objDev.CheckDevice(EID)
            'ret = "NEW" ' objDev.CheckDevice(EID)
            If Not ST.ToUpper() = "COMPLETE" Then
                ret = objDev.CheckDevice(EID)
            Else
                ret = "NEW"
            End If

            con = New SqlConnection(conStr)
            If BPMKEY = Key Then

                Query = "SELECT FF.*,F.*  FROM MMM_MST_FIELDS FF left outer JOIN MMM_MST_FORMS F on F.FormName = FF.DocumentType and F.EID = FF.EID   where (FF.isactive=1 or FieldType='Auto Number') AND FF.DocumentType not in(SELECT FormName FROM MMM_MST_FORMS WHERE EID=" & EID & " AND FormSource='DETAIL FORM' AND FormName Not IN (SELECT dropdown FROM MMM_MST_FIELDS WHERE EID=" & EID & ")) and F.EID=" & EID

                If ret.ToUpper() <> "NEW" Then
                    Query = Query & " AND " & "FF.lastupdate >= " & "'" & ret & "'"
                End If

                da = New SqlDataAdapter(Query, con)
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

    Function GetChildItemDefaultValueALL(Key As String, EID As String, IMINum As String, ST As String) As List(Of ChildItemDefaultValue) Implements IBPMMobileOffline.GetChildItemDefaultValueALL

        Dim lstFlds As New List(Of ChildItemDefaultValue)
        Dim da As SqlDataAdapter = Nothing
        Dim con As SqlConnection = Nothing
        Dim objDev As New DeviceInfo()
        Dim ret = "NEW"
        Try
            objDev.IMINumber = IMINum
            If Not ST.ToUpper() = "COMPLETE" Then
                ret = objDev.CheckDevice(EID)
            Else
                ret = "NEW"
            End If

            Dim ds As New DataSet()
            Dim strQuery = "SELECT Substring(DocumentType,1,len(DocumentType)-7)'DocType', * FROM MMM_MST_MASTER WHERE EID=" & EID & " AND  isauth =1 AND DocumentType in(select FormName+'_Master' FROM MMM_MST_FORMS where EID=" & EID & " and hasdefaultvalue=1 and FormSource='DETAIL FORM') "
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

    Function GetUserMenu(Key As String, EID As String, UserRole As String, IMINum As String, ST As String) As List(Of UserMenu) Implements IBPMMobileOffline.GetUserMenu
        Dim lstMenu As New List(Of UserMenu)
        Dim dsMenu As New DataSet()
        Dim objM As UserMenu
        Dim objDev As New DeviceInfo()
        Dim ret = "NEW"
        Try
            Dim BPMKEY As String = ConfigurationManager.AppSettings("BPMKEY").ToString()
            If BPMKEY = Key Then
                objDev.IMINumber = IMINum
                If Not ST.ToUpper() = "COMPLETE" Then
                    ret = objDev.CheckDevice(EID)
                Else
                    ret = "1900-01-01 12:25:25.010"
                End If

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

    Function GetFormValidaion(Key As String, EID As String, IMINum As String, ST As String) As List(Of FormValidation) Implements IBPMMobileOffline.GetFormValidaion
        Dim lstFLV As New List(Of FormValidation)
        Dim ds As New DataSet()
        Dim objM As New FormValidation()
        Dim objDev As New DeviceInfo()
        Dim ret = "NEW"
        Try
            Dim BPMKEY As String = ConfigurationManager.AppSettings("BPMKEY").ToString()
            If BPMKEY = Key Then
                objDev.IMINumber = IMINum
                If Not ST.ToUpper() = "COMPLETE" Then
                    ret = objDev.CheckDevice(EID)
                Else
                    ret = "1900-01-01 00:00:00.000"
                End If
                'GetFormValidation(EID As String, lastUpdate As String)
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

    Function GetPDFTemplet(Key As String, EID As String, IMINum As String, ST As String) As List(Of PDFTemplet) Implements IBPMMobileOffline.GetPDFTemplet
        Dim lstFLV As New List(Of PDFTemplet)
        Dim ds As New DataSet()
        Dim objM As New PDFTemplet()
        Dim objDev As New DeviceInfo()
        Dim ret = "NEW"
        Try
            Dim BPMKEY As String = ConfigurationManager.AppSettings("BPMKEY").ToString()
            If BPMKEY = Key Then
                objDev.IMINumber = IMINum
                If Not ST.ToUpper() = "COMPLETE" Then
                    ret = objDev.CheckDevice(EID)
                Else
                    ret = "1900-01-01 00:00:00.000"
                End If
                'GetFormValidation(EID As String, lastUpdate As String)
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

    Function GetTriggers(Key As String, EID As String, IMINum As String, ST As String) As List(Of Trigger) Implements IBPMMobileOffline.GetTriggers
        Dim lstTrg As New List(Of Trigger)
        Dim ds As New DataSet()
        Dim objT As New Trigger()
        Dim objDev As New DeviceInfo()
        Dim ret = "NEW"
        Try
            Dim BPMKEY As String = ConfigurationManager.AppSettings("BPMKEY").ToString()
            If BPMKEY = Key Then
                objDev.IMINumber = IMINum
                If Not ST.ToUpper() = "COMPLETE" Then
                    ret = objDev.CheckDevice(EID)
                Else
                    ret = "1900-01-01 00:00:00.000"
                End If
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


    Function SyncDeletedFields(Key As String, EID As String, IMINum As String, ST As String) As String Implements IBPMMobileOffline.SyncDeletedFields
        Dim objDev As New DeviceInfo()
        Dim ret = "NEW"
        Dim Result = ""
        Try
            Dim BPMKEY As String = ConfigurationManager.AppSettings("BPMKEY").ToString()
            If BPMKEY = Key Then
                objDev.IMINumber = IMINum
                If Not ST.ToUpper() = "COMPLETE" Then
                    ret = objDev.CheckDevice(EID)
                Else
                    ret = "1900-01-01 00:00:00.000"
                End If
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

    Function GetFormDetails(Key As String, EID As String, IMINum As String, ST As String) As List(Of FormDetails) Implements IBPMMobileOffline.GetFormDetails

        Dim lstForms As New List(Of FormDetails)
        Dim ds As New DataSet()
        Dim objM As New FormDetails()
        Dim objDev As New DeviceInfo()
        Dim ret = "NEW"
        Try
            Dim BPMKEY As String = ConfigurationManager.AppSettings("BPMKEY").ToString()
            If BPMKEY = Key Then
                objDev.IMINumber = IMINum
                If Not ST.ToUpper() = "COMPLETE" Then
                    ret = objDev.CheckDevice(EID)
                Else
                    ret = "1900-01-01 00:00:00.000"
                End If
                'GetFormValidation(EID As String, lastUpdate As String)
                ds = objM.GetFormDetails(EID:=EID, LastUpdated:=ret)
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
                        lstForms.Add(objM)
                    Next
                End If
            End If
        Catch ex As Exception
            Throw New FaultException("RTO")
        End Try
        Return lstForms
    End Function

    Function Syncacknowledgement(Key As String, EID As String, IMINum As String) As String Implements IBPMMobileOffline.Syncacknowledgement
        Dim ret As String = ""
        Dim objDev As New DeviceInfo()
        Dim result As Integer = 0
        Try
            objDev.IMINumber = IMINum
            result = objDev.UpdateMobDeviceInfo(EID)
            If result > 0 Then
                ret = "OK"
            Else
                ret = "Fail"
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

    Public Function ChangePassword(key As String, ByVal EID As String, ByVal UID As Integer, ByVal cpwd As String, ByVal npwd As String) As String Implements IBPMMobileOffline.ChangePassword
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
    'Function ForgotPassword(Key As String, UserName As String, ECODE As String) As String
    Public Function ForgotPassword(ByVal UserName As String, ByVal ECODE As String) As String Implements IBPMMobileOffline.ForgotPassword
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

End Class

Public Class DeviceInfo
    Public Property IMINumber As String

    Dim conStr As String = ConfigurationManager.ConnectionStrings("conStrLive").ConnectionString

    Public Function CheckDevice(EID As Integer) As String
        Dim LastUpdatedOn As String = ""
        Dim con As SqlConnection = Nothing
        Dim da As SqlDataAdapter = Nothing
        Try
            con = New SqlConnection(conStr)
            da = New SqlDataAdapter("CheckDevice", con)
            da.SelectCommand.CommandType = CommandType.StoredProcedure
            da.SelectCommand.Parameters.AddWithValue("@IMINum", IMINumber)
            da.SelectCommand.Parameters.AddWithValue("@EID", EID)
            con.Open()
            LastUpdatedOn = da.SelectCommand.ExecuteScalar()
        Catch ex As Exception
            Throw
        Finally
            If Not con Is Nothing Then
                con.Close()
                con.Dispose()
            End If
            If Not da Is Nothing Then
                da.Dispose()
            End If
        End Try
        Return LastUpdatedOn
    End Function

    Public Function ValidateDevice(EID As Integer, UserID As Integer) As DataSet
        Dim ret As Boolean = False
        Dim con As SqlConnection = Nothing
        Dim da As SqlDataAdapter = Nothing
        Dim ds As New DataSet()
        Try
            con = New SqlConnection(conStr)
            da = New SqlDataAdapter("ValidateDevice_0000", con)
            da.SelectCommand.CommandType = CommandType.StoredProcedure
            da.SelectCommand.Parameters.AddWithValue("@IMINUMBER", IMINumber)
            da.SelectCommand.Parameters.AddWithValue("@EID", EID)
            da.SelectCommand.Parameters.AddWithValue("@UserID", UserID)
            da.Fill(ds)
        Catch ex As Exception
            Throw
        Finally
            If Not con Is Nothing Then
                con.Close()
                con.Dispose()
            End If
            If Not da Is Nothing Then
                da.Dispose()
            End If
        End Try
        Return ds
    End Function

    Public Function UpdateMobDeviceInfo(EID As Integer) As String
        Dim con As SqlConnection = Nothing
        Dim da As SqlDataAdapter = Nothing
        Dim ret = 0
        Dim Time As Integer = 0
        Try
            Dim NIMI = IMINumber
            If IMINumber.Length > 15 Then
                NIMI = Left(IMINumber, 15)
                'First Fifteen degint is IMI Number AND Rest IS Time
                Time = IMINumber.Replace(Left(IMINumber, 15), String.Empty)
            End If
            con = New SqlConnection(conStr)
            da = New SqlDataAdapter("UpdateMobDeviceInfo", con)
            da.SelectCommand.CommandType = CommandType.StoredProcedure
            da.SelectCommand.Parameters.AddWithValue("@IMINumber", NIMI)
            da.SelectCommand.Parameters.AddWithValue("@EID", EID)
            da.SelectCommand.Parameters.AddWithValue("@Time", Time)
            con.Open()
            ret = da.SelectCommand.ExecuteNonQuery()
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
        Return ret
    End Function

    
End Class

<DataContractAttribute(Namespace:="", Name:="DocData")>
Public Class DocData

    <DataMember(Name:="DocumentType", Order:=1)> _
    Public Property DocumentType As String

    <DataMember(Name:="fld1", Order:=2)> _
    Public Property fld1 As String

    <DataMember(Name:="fld2", Order:=3)> _
    Public Property fld2 As String

    <DataMember(Name:="fld3", Order:=4)> _
    Public Property fld3 As String

    <DataMember(Name:="fld4", Order:=5)> _
    Public Property fld4 As String

    <DataMember(Name:="fld5", Order:=6)> _
    Public Property fld5 As String

    <DataMember(Name:="fld6", Order:=7)> _
    Public Property fld6 As String

    <DataMember(Name:="fld7", Order:=8)> _
    Public Property fld7 As String

    <DataMember(Name:="fld8", Order:=10)> _
    Public Property fld8 As String

    <DataMember(Name:="fld9", Order:=11)> _
    Public Property fld9 As String

    <DataMember(Name:="fld10", Order:=12)> _
    Public Property fld10 As String

    <DataMember(Name:="fld11", Order:=13)> _
    Public Property fld11 As String

    <DataMember(Name:="fld12", Order:=14)> _
    Public Property fld12 As String

    <DataMember(Name:="fld13", Order:=15)> _
    Public Property fld13 As String

    <DataMember(Name:="fld14", Order:=16)> _
    Public Property fld14 As String

    <DataMember(Name:="fld15", Order:=17)> _
    Public Property fld15 As String

    <DataMember(Name:="fld16", Order:=18)> _
    Public Property fld16 As String

    <DataMember(Name:="fld17", Order:=19)> _
    Public Property fld17 As String

    <DataMember(Name:="fld18", Order:=20)> _
    Public Property fld18 As String

    <DataMember(Name:="fld19", Order:=21)> _
    Public Property fld19 As String

    <DataMember(Name:="fld20", Order:=22)> _
    Public Property fld20 As String

    <DataMember(Name:="fld21", Order:=22)> _
    Public Property fld21 As String

    <DataMember(Name:="fld22", Order:=23)> _
    Public Property fld22 As String

    <DataMember(Name:="fld23", Order:=24)> _
    Public Property fld23 As String

    <DataMember(Name:="fld24", Order:=25)> _
    Public Property fld24 As String

    <DataMember(Name:="fld25", Order:=26)> _
    Public Property fld25 As String

    <DataMember(Name:="fld26", Order:=27)> _
    Public Property fld26 As String

    <DataMember(Name:="fld27", Order:=28)> _
    Public Property fld27 As String

    <DataMember(Name:="fld28", Order:=29)> _
    Public Property fld28 As String

    <DataMember(Name:="fld29", Order:=30)> _
    Public Property fld29 As String

    <DataMember(Name:="fld30", Order:=31)> _
    Public Property fld30 As String

    <DataMember(Name:="fld31", Order:=32)> _
    Public Property fld31 As String

    <DataMember(Name:="fld32", Order:=33)> _
    Public Property fld32 As String

    <DataMember(Name:="fld33", Order:=34)> _
    Public Property fld33 As String

    <DataMember(Name:="fld34", Order:=35)> _
    Public Property fld34 As String

    <DataMember(Name:="fld35", Order:=36)> _
    Public Property fld35 As String

    <DataMember(Name:="fld36", Order:=37)> _
    Public Property fld36 As String

    <DataMember(Name:="fld37", Order:=38)> _
    Public Property fld37 As String

    <DataMember(Name:="fld38", Order:=39)> _
    Public Property fld38 As String

    <DataMember(Name:="fld39", Order:=40)> _
    Public Property fld39 As String

    <DataMember(Name:="fld40", Order:=41)> _
    Public Property fld40 As String

    <DataMember(Name:="fld41", Order:=42)> _
    Public Property fld41 As String

    <DataMember(Name:="fld42", Order:=43)> _
    Public Property fld42 As String

    <DataMember(Name:="fld43", Order:=44)> _
    Public Property fld43 As String

    <DataMember(Name:="fld44", Order:=45)> _
    Public Property fld44 As String

    <DataMember(Name:="fld45", Order:=46)> _
    Public Property fld45 As String

    <DataMember(Name:="fld46", Order:=47)> _
    Public Property fld46 As String

    <DataMember(Name:="fld47", Order:=48)> _
    Public Property fld47 As String

    <DataMember(Name:="fld48", Order:=49)> _
    Public Property fld48 As String

    <DataMember(Name:="fld49", Order:=50)> _
    Public Property fld49 As String

    <DataMember(Name:="fld50", Order:=51)> _
    Public Property fld50 As String

    <DataMember(Name:="IsAuth", Order:=52)> _
    Public Property IsAuth As String

    <DataMember(Name:="TID", Order:=53)> _
    Public Property TID As String

    <DataMember(Name:="DocID", Order:=54)> _
    Public Property DocID As String

End Class

<DataContractAttribute(Namespace:="", Name:="FormValidation")>
Public Class FormValidation
    <DataMember(Name:="EID", Order:=1)> _
    Public Property EID As String

    <DataMember(Name:="DocType", Order:=2)> _
    Public Property DocType As String

    <DataMember(Name:="ValType", Order:=3)> _
    Public Property ValType As String

    <DataMember(Name:="fldID", Order:=4)> _
    Public Property fldID As String

    <DataMember(Name:="Operator", Order:=5)> _
    Public Property Operator1 As String

    <DataMember(Name:="Value", Order:=6)> _
    Public Property Value As String

    <DataMember(Name:="Err_MSG", Order:=7)> _
    Public Property Err_MSG As String

    <DataMember(Name:="WF_Status", Order:=8)> _
    Public Property WF_Status As String

    <DataMember(Name:="docNature", Order:=9)> _
    Public Property docNature As String

    <DataMember(Name:="TID", Order:=10)> _
    Public Property TID As String

    Public Function GetFormValidation(EID As String, lastUpdate As String) As DataSet
        Dim ds As New DataSet()
        'Dim con As SqlConnection = Nothing
        'Dim da As SqlDataAdapter = Nothing
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStrLive").ConnectionString
        Try
            Using con As New SqlConnection(conStr)
                Using da As New SqlDataAdapter("GetFormValidation", con)
                    'da.SelectCommand.Connection = con
                    'da.SelectCommand.CommandText = "GetFormValidation"
                    da.SelectCommand.CommandType = CommandType.StoredProcedure
                    da.SelectCommand.Parameters.AddWithValue("@EID", EID)
                    da.SelectCommand.Parameters.AddWithValue("@LastUpdate", lastUpdate)
                    If con.State <> ConnectionState.Open Then
                        con.Open()
                    End If
                    da.Fill(ds)
                End Using
            End Using
        Catch ex As Exception
            Throw
        End Try
        Return ds
    End Function

End Class

<DataContractAttribute(Namespace:="", Name:="PDFTemplet")>
Public Class PDFTemplet
    <DataMember(Name:="TID", Order:=1)> _
    Public Property TID As String

    <DataMember(Name:="Template_name", Order:=2)> _
    Public Property Template_name As String

    <DataMember(Name:="Body", Order:=3)> _
    Public Property Body As String

    <DataMember(Name:="DocumentType", Order:=4)> _
    Public Property DocumentType As String


    Public Function GetPDFTemplet(EID As String, lastUpdate As String) As DataSet
        Dim ds As New DataSet()
        'Dim con As SqlConnection = Nothing
        'Dim da As SqlDataAdapter = Nothing
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStrLive").ConnectionString
        Try
            Using con As New SqlConnection(conStr)
                Using da As New SqlDataAdapter("GetPDFTemplet", con)
                    'da.SelectCommand.Connection = con
                    'da.SelectCommand.CommandText = "GetFormValidation"
                    da.SelectCommand.CommandType = CommandType.StoredProcedure
                    da.SelectCommand.Parameters.AddWithValue("@EID", EID)
                    da.SelectCommand.Parameters.AddWithValue("@LastUpdate", lastUpdate)
                    If con.State <> ConnectionState.Open Then
                        con.Open()
                    End If
                    da.Fill(ds)
                End Using
            End Using
        Catch ex As Exception
            Throw
        End Try
        Return ds
    End Function

End Class

<DataContractAttribute(Namespace:="", Name:="FormDetails")>
Public Class FormDetails

    <DataMember(Name:="FormName", Order:=1)> _
    Public Property FormName As String

    <DataMember(Name:="EID", Order:=2)> _
    Public Property EID As String

    <DataMember(Name:="FormType", Order:=3)> _
    Public Property FormType As String

    <DataMember(Name:="FormSource", Order:=4)> _
    Public Property FormSource As String

    <DataMember(Name:="IsActive", Order:=5)> _
    Public Property IsActive As String

    <DataMember(Name:="EventName", Order:=6)> _
    Public Property EventName As String

    <DataMember(Name:="SubEvent", Order:=7)> _
    Public Property SubEvent As String

    <DataMember(Name:="FormCaption", Order:=8)> _
    Public Property FormCaption As String

    <DataMember(Name:="FormDesc", Order:=9)> _
    Public Property FormDesc As String

    <DataMember(Name:="LayoutType", Order:=10)> _
    Public Property LayoutType As String

    <DataMember(Name:="curstatus", Order:=11)> _
    Public Property curstatus As String

    <DataMember(Name:="layoutdata", Order:=12)> _
    Public Property layoutdata As String

    <DataMember(Name:="Iscalendar", Order:=13)> _
    Public Property Iscalendar As String

    <DataMember(Name:="isWorkFlow", Order:=14)> _
    Public Property isWorkFlow As String

    <DataMember(Name:="History", Order:=15)> _
    Public Property History As String

    <DataMember(Name:="isPush", Order:=16)> _
    Public Property isPush As String

    <DataMember(Name:="isRoleDef", Order:=17)> _
    Public Property isRoleDef As String

    <DataMember(Name:="DocMapping", Order:=18)> _
    Public Property DocMapping As String

    <DataMember(Name:="PUBLICVIEW", Order:=19)> _
    Public Property PUBLICVIEW As String

    <DataMember(Name:="PUBLICENTRY", Order:=20)> _
    Public Property PUBLICENTRY As String

    <DataMember(Name:="DocNature", Order:=21)> _
    Public Property DocNature As String

    <DataMember(Name:="hasdefaultvalue", Order:=22)> _
    Public Property hasdefaultvalue As String

    <DataMember(Name:="AttahcedFileSize", Order:=23)> _
    Public Property AttahcedFileSize As String

    <DataMember(Name:="ShowUploader", Order:=24)> _
    Public Property ShowUploader As String

    <DataMember(Name:="EnableWS", Order:=25)> _
    Public Property EnableWS As String

    <DataMember(Name:="lastupdate", Order:=26)> _
    Public Property lastupdate As String

    <DataMember(Name:="enableDraft", Order:=27)> _
    Public Property enableDraft As String

    <DataMember(Name:="isinlineediting", Order:=28)> _
    Public Property isinlineediting As String

    <DataMember(Name:="inlinetype", Order:=29)> _
    Public Property inlinetype As String

    <DataMember(Name:="inlinesourcedoc", Order:=30)> _
    Public Property inlinesourcedoc As String

    <DataMember(Name:="inlinefilter", Order:=31)> _
    Public Property inlinefilter As String

    <DataMember(Name:="inlinefilterdisplay", Order:=32)> _
    Public Property inlinefilterdisplay As String

    <DataMember(Name:="UniqueKeys", Order:=33)> _
    Public Property UniqueKeys As String

    <DataMember(Name:="UniqueKeys_ExChars", Order:=34)> _
    Public Property UniqueKeys_ExChars As String

    <DataMember(Name:="SortingFields", Order:=35)> _
    Public Property SortingFields As String

    <DataMember(Name:="enableCRM", Order:=36)> _
    Public Property enableCRM As String

    <DataMember(Name:="isUserCreation", Order:=37)> _
    Public Property isUserCreation As String

    <DataMember(Name:="enable_mail_On_CRM", Order:=38)> _
    Public Property enable_mail_On_CRM As String

    <DataMember(Name:="IsDefaultRows", Order:=39)> _
    Public Property IsDefaultRows As String

    <DataMember(Name:="inlinemappingdisplay", Order:=40)> _
    Public Property inlinemappingdisplay As String

    <DataMember(Name:="autosaveinterval", Order:=41)> _
    Public Property autosaveinterval As String

    <DataMember(Name:="Primarykey", Order:=42)> _
    Public Property Primarykey As String

    <DataMember(Name:="JsonQuery", Order:=43)> _
    Public Property JsonQuery As String

    <DataMember(Name:="Balance_Maintenance_Mode", Order:=44)> _
    Public Property Balance_Maintenance_Mode As String

    <DataMember(Name:="Effective_Date_Field", Order:=45)> _
    Public Property Effective_Date_Field As String

    <DataMember(Name:="Balance_Field", Order:=46)> _
    Public Property Balance_Field As String

    <DataMember(Name:="Item_Number", Order:=47)> _
    Public Property Item_Number As String

    <DataMember(Name:="Relation_Doc_Type", Order:=48)> _
    Public Property Relation_Doc_Type As String

    <DataMember(Name:="Balance_Type", Order:=49)> _
    Public Property Balance_Type As String

    <DataMember(Name:="EnableInsertOneditFail", Order:=50)> _
    Public Property EnableInsertOneditFail As String

    <DataMember(Name:="XMLInwardInputDocType", Order:=51)> _
    Public Property XMLInwardInputDocType As String

    <DataMember(Name:="XMLInwardInputEntityCode", Order:=52)> _
    Public Property XMLInwardInputEntityCode As String

    <DataMember(Name:="XMLOutWardDocType", Order:=53)> _
    Public Property XMLOutWardDocType As String

    <DataMember(Name:="XMLOutWardEntityCOde", Order:=54)> _
    Public Property XMLOutWardEntityCOde As String

    <DataMember(Name:="RowFilterXMLTag", Order:=55)> _
    Public Property RowFilterXMLTag As String

    <DataMember(Name:="RowFilterBPMField", Order:=56)> _
    Public Property RowFilterBPMField As String

    <DataMember(Name:="ChildMasterField", Order:=57)> _
    Public Property ChildMasterField As String

    <DataMember(Name:="ChildFilterRule", Order:=58)> _
    Public Property ChildFilterRule As String

    <DataMember(Name:="S_T_DDN", Order:=59)> _
    Public Property S_T_DDN As String

    <DataMember(Name:="Relation_Type", Order:=60)> _
    Public Property Relation_Type As String

    <DataMember(Name:="TallyRegField", Order:=61)> _
    Public Property TallyRegField As String

    <DataMember(Name:="TallyCancelXMlTag", Order:=62)> _
    Public Property TallyCancelXMlTag As String

    <DataMember(Name:="Allowmultiuse", Order:=63)> _
    Public Property Allowmultiuse As String

    <DataMember(Name:="EnableSync_Tally", Order:=64)> _
    Public Property EnableSync_Tally As String

    <DataMember(Name:="Sync_Tally_Ordering", Order:=65)> _
    Public Property Sync_Tally_Ordering As String



    Public Function GetFormDetails(EID As String, LastUpdated As String) As DataSet
        Dim ds As New DataSet()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStrLive").ConnectionString
        Try
            Using con As New SqlConnection(conStr)
                Using da As New SqlDataAdapter("GetFormDetails", con)
                    da.SelectCommand.CommandType = CommandType.StoredProcedure
                    da.SelectCommand.Parameters.AddWithValue("@EID", EID)
                    da.SelectCommand.Parameters.AddWithValue("@LastUpdate", LastUpdated)
                    If con.State <> ConnectionState.Open Then
                        con.Open()
                    End If
                    da.Fill(ds)
                End Using
            End Using
        Catch ex As Exception
            Throw
        End Try
        Return ds
    End Function

End Class


