Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Data.SqlClient
Imports System.Xml.Serialization
Imports System.Runtime.Serialization
Imports System.ServiceModel
Imports System.ServiceModel.Web
Imports System.Xml

<DataContractAttribute(Namespace:="", Name:="Trigger")>
Public Class Trigger
    <DataMember(Name:="DocumentType", Order:=1)> _
    Public Property DocumentType As String
    <DataMember(Name:="TriggerText", Order:=2)> _
    Public Property TriggerText As String
    <DataMember(Name:="onCreate", Order:=3)> _
    Public Property onCreate As String
    <DataMember(Name:="onEdit", Order:=4)> _
    Public Property onEdit As String
    <DataMember(Name:="TID", Order:=5)> _
    Public Property TID As String

#Region "Shared Member Function "

    Public Shared Function AddUpdateTrigger(ByVal TId As Integer, ByVal EID As Integer, ByVal FormID As Integer, ByVal TriggerText As String) As Integer
        Dim Result As Integer = -2
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim da As SqlDataAdapter = New SqlDataAdapter("AddUpdateTrigger", con)
        Try
            da.SelectCommand.CommandType = CommandType.StoredProcedure
            da.SelectCommand.Parameters.AddWithValue("@Tid", TId)
            da.SelectCommand.Parameters.AddWithValue("@EID", EID)
            da.SelectCommand.Parameters.AddWithValue("@FormId", FormID)
            da.SelectCommand.Parameters.AddWithValue("@TriggerText", TriggerText)

            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            Result = (Convert.ToInt32(da.SelectCommand.ExecuteScalar()))

            Return Result

        Catch ex As Exception
            Return Result
        Finally
            con.Close()
            con.Dispose()
            da.Dispose()
        End Try

    End Function

    Public Shared Function GetTriggers(ByVal EID As Integer, ByVal fileID As Integer, ByVal FORMName As String, Optional ByVal IsdetailsForm As Integer = 0, Optional ByVal OnEdit As Integer = 0, Optional ByVal OnCreate As Integer = 0, Optional ByVal FormType As String = "Document") As DataSet
        Dim DsTrg As New DataSet
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        ' prev used getTriggrersNEW10
        Dim da As SqlDataAdapter = New SqlDataAdapter("getTriggrersNEW10000", con)
        Try

            'Old One
            ''Dim da As SqlDataAdapter = New SqlDataAdapter("getTriggrersNew1", con)

            da.SelectCommand.CommandType = CommandType.StoredProcedure
            'da.SelectCommand.Parameters.AddWithValue("@TID", TId)
            da.SelectCommand.Parameters.AddWithValue("@EID", EID)
            da.SelectCommand.Parameters.AddWithValue("@FORMID", fileID)
            da.SelectCommand.Parameters.AddWithValue("@DocumentType", FORMName)
            da.SelectCommand.Parameters.AddWithValue("@IsdetailsForm", IsdetailsForm)
            da.SelectCommand.Parameters.AddWithValue("@OnEdit", OnEdit)
            da.SelectCommand.Parameters.AddWithValue("@OnCreate", OnCreate)
            da.SelectCommand.Parameters.AddWithValue("@FormType", FormType)
            '@DocumentType
            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            da.Fill(DsTrg, "tbltrg")

            Return DsTrg
        Catch ex As Exception
            Throw
        Finally
            con.Close()
            da.Dispose()
            da.Dispose()
        End Try
    End Function

    Public Shared Function GetTriggersT(ByVal EID As Integer, ByVal fileID As Integer, ByVal FORMName As String, con As SqlConnection, tran As SqlTransaction, Optional ByVal IsdetailsForm As Integer = 0, Optional ByVal OnEdit As Integer = 0, Optional ByVal OnCreate As Integer = 0, Optional ByVal FormType As String = "Document") As DataSet
        Dim DsTrg As New DataSet
        'Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        'Dim con As SqlConnection = New SqlConnection(conStr)
        Dim da As SqlDataAdapter = New SqlDataAdapter("getTriggrersNEW10000", con)
        da.SelectCommand.Transaction = tran
        Try

            'Old One
            ''Dim da As SqlDataAdapter = New SqlDataAdapter("getTriggrersNew1", con)

            da.SelectCommand.CommandType = CommandType.StoredProcedure
            'da.SelectCommand.Parameters.AddWithValue("@TID", TId)
            da.SelectCommand.Parameters.AddWithValue("@EID", EID)
            da.SelectCommand.Parameters.AddWithValue("@FORMID", fileID)
            da.SelectCommand.Parameters.AddWithValue("@DocumentType", FORMName)
            da.SelectCommand.Parameters.AddWithValue("@IsdetailsForm", IsdetailsForm)
            da.SelectCommand.Parameters.AddWithValue("@OnEdit", OnEdit)
            da.SelectCommand.Parameters.AddWithValue("@OnCreate", OnCreate)
            da.SelectCommand.Parameters.AddWithValue("@FormType", FormType)
            '@DocumentType
            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            da.Fill(DsTrg, "tbltrg")

            Return DsTrg
        Catch ex As Exception
            Throw
        Finally
            'con.Close()
            da.Dispose()
            da.Dispose()
        End Try

    End Function

    Public Shared Sub ExecuteTrigger(ByVal FormName As String, ByVal EID As Integer, ByVal fileID As Integer, Optional ByVal IsdetailsForm As Integer = 0, Optional TriggerNature As String = "Create", Optional FormType As String = "Document")
        Dim Result As Integer = -2
        Dim TriggerText As String = ""
        Dim dsTrg As DataSet
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim da As SqlDataAdapter = New SqlDataAdapter("", con)
        Try
            Try
                Dim OnEdit As Integer = 0, OnCreate As Integer = 0
                If TriggerNature.ToUpper() = "CREATE" Then
                    OnCreate = 1
                Else
                    OnEdit = 1
                End If
                dsTrg = GetTriggers(EID, fileID, FormName, IsdetailsForm, OnEdit, OnCreate)
                Dim str As String = ""
                Dim rStr As String = ""
                If dsTrg.Tables("tbltrg").Rows.Count > 0 Then
                    If FormName.ToUpper.Trim() = dsTrg.Tables("tbltrg").Rows(0).Item("doctype").ToString.ToUpper.Trim() Then
                        da.SelectCommand.CommandType = CommandType.Text
                        da.SelectCommand.Parameters.Clear()
                        If FormType.ToUpper = "MASTER" Then
                            da.SelectCommand.CommandText = "SELECT D.tid AS D_tid,D.EID AS D_EID,D.aDate AS D_aDate,D.fld1 AS D_fld1,D.fld2 AS D_fld2,D.fld3 AS D_fld3,D.fld4 AS D_fld4,D.fld5 AS D_fld5,D.fld6 AS D_fld6,D.fld7 AS D_fld7,D.fld8 AS D_fld8,D.fld9 AS D_fld9,D.fld10 AS D_fld10,D.fld11 AS D_fld11,D.fld12 AS D_fld12,D.fld13 AS D_fld13,D.fld14 AS D_fld14,D.fld15 AS D_fld15,D.fld16 AS D_fld16,D.fld17 AS D_fld17,D.fld18 AS D_fld18,D.fld19 AS D_fld19,D.fld20 AS D_fld20,D.DocumentType AS D_DocumentType,D.fld21 AS D_fld21,D.fld22 AS D_fld22,D.fld23 AS D_fld23,D.fld24 AS D_fld24,D.fld25 AS D_fld25,D.fld26 AS D_fld26,D.fld27 AS D_fld27,D.fld28 AS D_fld28,D.fld29 AS D_fld29,D.fld30 AS D_fld30,D.fld31 AS D_fld31,D.fld32 AS D_fld32,D.fld33 AS D_fld33,D.fld34 AS D_fld34,D.fld35 AS D_fld35,D.fld36 AS D_fld36,D.fld37 AS D_fld37,D.fld38 AS D_fld38,D.fld39 AS D_fld39,D.fld40 AS D_fld40,D.isauth AS D_isauth,D.fld41 AS D_fld41,D.fld42 AS D_fld42,D.fld43 AS D_fld43,D.fld44 AS D_fld44,D.fld45 AS D_fld45,D.fld46 AS D_fld46,D.fld47 AS D_fld47,D.fld48 AS D_fld48,D.fld49 AS D_fld49,D.fld50 AS D_fld50,D.fld51 AS D_fld51,D.fld52 AS D_fld52,D.fld53 AS D_fld53,D.fld54 AS D_fld54,D.fld55 AS D_fld55,D.fld56 AS D_fld56,D.fld57 AS D_fld57,D.fld58 AS D_fld58,D.fld59 AS D_fld59,D.fld60 AS D_fld60,D.fld61 AS D_fld61,D.fld62 AS D_fld62,D.fld63 AS D_fld63,D.fld64 AS D_fld64,D.fld65 AS D_fld65,D.fld66 AS D_fld66,D.fld67 AS D_fld67,D.fld68 AS D_fld68,D.fld69 AS D_fld69,D.fld70 AS D_fld70,D.fld71 AS  D_fld71,D.fld72 AS  D_fld72,D.fld73 AS  D_fld73,D.fld74 AS  D_fld74,D.fld75 AS  D_fld75,D.fld76 AS  D_fld76,D.fld77 AS  D_fld77,D.fld78 AS  D_fld78,D.fld79 AS  D_fld79,D.fld80 AS  D_fld80,D.fld81 AS  D_fld81,D.fld82 AS  D_fld82,D.fld83 AS  D_fld83,D.fld84 AS  D_fld84,D.fld85 AS  D_fld85,D.fld86 AS  D_fld86,D.fld87 AS  D_fld87,D.fld88 AS  D_fld88,D.fld89 AS  D_fld89,D.fld90 AS  D_fld90,D.fld91 AS  D_fld91,D.fld92 AS  D_fld92,D.fld93 AS  D_fld93,D.fld94 AS  D_fld94,D.fld95 AS  D_fld95,D.fld96 AS  D_fld96,D.fld97 AS  D_fld97,D.fld98 AS  D_fld98,D.fld99 AS  D_fld99,D.fld100 AS  D_fld100,D.fld101 AS  D_fld101,D.fld102 AS  D_fld102,D.fld103 AS  D_fld103,D.fld104 AS  D_fld104,D.fld105 AS  D_fld105,D.fld106 AS  D_fld106,D.fld107 AS  D_fld107,D.fld108 AS  D_fld108,D.fld109 AS  D_fld109,D.fld110 AS  D_fld110,D.fld111 AS  D_fld111,D.fld112 AS  D_fld112,D.fld113 AS  D_fld113,D.fld114 AS  D_fld114,D.fld115 AS  D_fld115,D.fld116 AS  D_fld116,D.fld117 AS  D_fld117,D.fld118 AS  D_fld118,D.fld119 AS  D_fld119,D.fld120 AS  D_fld120,D.fld121 AS  D_fld121,D.fld122 AS  D_fld122,D.fld123 AS  D_fld123,D.fld124 AS  D_fld124,D.fld125 AS  D_fld125,D.fld126 AS  D_fld126,D.fld127 AS  D_fld127,D.fld128 AS  D_fld128,D.fld129 AS  D_fld129,D.fld130 AS  D_fld130,D.fld131 AS  D_fld131,D.fld132 AS  D_fld132,D.fld133 AS  D_fld133,D.fld134 AS  D_fld134,D.fld135 AS  D_fld135,D.fld136 AS  D_fld136,D.fld137 AS  D_fld137,D.fld138 AS  D_fld138,D.fld139 AS  D_fld139,D.fld140 AS  D_fld140,D.fld141 AS  D_fld141,D.fld142 AS  D_fld142,D.fld143 AS  D_fld143,D.fld144 AS  D_fld144,D.fld145 AS  D_fld145,D.fld146 AS  D_fld146,D.fld147 AS  D_fld147,D.fld148 AS  D_fld148,D.fld149 AS  D_fld149,D.fld150 AS  D_fld150,D.fld151 AS  D_fld151,D.fld152 AS  D_fld152,D.fld153 AS  D_fld153,D.fld154 AS  D_fld154,D.fld155 AS  D_fld155,D.fld156 AS  D_fld156,D.fld157 AS  D_fld157,D.fld158 AS  D_fld158,D.fld159 AS  D_fld159,D.fld160 AS  D_fld160,D.fld161 AS  D_fld161,D.fld162 AS  D_fld162,D.fld163 AS  D_fld163,D.fld164 AS  D_fld164,D.fld165 AS  D_fld165,D.fld166 AS  D_fld166,D.fld167 AS  D_fld167,D.fld168 AS  D_fld168,D.fld169 AS  D_fld169,D.fld170 AS  D_fld170,D.fld171 AS  D_fld171,D.fld172 AS  D_fld172,D.fld173 AS  D_fld173,D.fld174 AS  D_fld174,D.fld175 AS  D_fld175,D.fld176 AS  D_fld176,D.fld177 AS  D_fld177,D.fld178 AS  D_fld178,D.fld179 AS  D_fld179,D.fld180 AS  D_fld180,D.fld181 AS  D_fld181,D.fld182 AS  D_fld182,D.fld183 AS  D_fld183,D.fld184 AS  D_fld184,D.fld185 AS  D_fld185,D.fld186 AS  D_fld186,D.fld187 AS  D_fld187,D.fld188 AS  D_fld188,D.fld189 AS  D_fld189,D.fld190 AS  D_fld190,D.fld191 AS  D_fld191,D.fld192 AS  D_fld192,D.fld193 AS  D_fld193,D.fld194 AS  D_fld194,D.fld195 AS  D_fld195,D.fld196 AS  D_fld196,D.fld197 AS  D_fld197,D.fld198 AS  D_fld198,D.fld199 AS  D_fld199, D.fld200 AS  D_fld200, D.fld201 AS  D_fld201, D.fld202 AS  D_fld202, D.fld203 AS  D_fld203, D.fld204 AS  D_fld204, D.fld205 AS  D_fld205, D.fld206 AS  D_fld206, D.fld207 AS  D_fld207, D.fld208 AS  D_fld208, D.fld209 AS  D_fld209 , D.fld210 AS  D_fld210, D.fld211 AS  D_fld211, D.fld212 AS D_fld212, D.fld213 AS D_fld213, D.fld214 AS D_fld214, D.fld215 AS D_fld215, D.fld216 AS D_fld216, D.fld217 AS D_fld217, D.fld218 AS D_fld218, D.fld219 AS D_fld219, D.fld220 AS D_fld220, D.fld221 AS D_fld221, D.fld222 AS D_fld222, D.fld223 AS D_fld223, D.fld224 AS D_fld224, D.fld225 AS D_fld225  FROM MMM_MST_MASTER D  where  D.TID=" & fileID & " and D.eid='" & EID & "'"
                        Else
                            da.SelectCommand.CommandText = "SELECT DI.TID AS DI_TID,DI.DOCID AS DI_DOCID,DI.SESSIONID AS DI_SESSIONID,DI.FLD1 AS DI_FLD1,DI.FLD2 AS DI_FLD2,DI.FLD3 AS DI_FLD3,DI.FLD4 AS DI_FLD4,DI.FLD5 AS DI_FLD5,DI.FLD6 AS DI_FLD6,DI.FLD7 AS DI_FLD7,DI.FLD8 AS DI_FLD8,DI.FLD9 AS DI_FLD9,DI.FLD10 AS DI_FLD10,DI.FLD11 AS DI_FLD11,DI.FLD12 AS DI_FLD12,DI.FLD13 AS DI_FLD13,DI.FLD14 AS DI_FLD14,DI.FLD15 AS DI_FLD15,DI.FLD16 AS DI_FLD16,DI.FLD17 AS DI_FLD17,DI.FLD18 AS DI_FLD18,DI.FLD19 AS DI_FLD19,DI.FLD20 AS DI_FLD20,DI.documenttype AS DI_documenttype,DI.fld21 AS DI_fld21,DI.fld22 AS DI_fld22,DI.fld23 AS DI_fld23,DI.fld24 AS DI_fld24,DI.fld25 AS DI_fld25,DI.fld26 AS DI_fld26,DI.fld27 AS DI_fld27,DI.fld28 AS DI_fld28,DI.fld29 AS DI_fld29,DI.fld30 AS DI_fld30,DI.fld31 AS DI_fld31,DI.fld32 AS DI_fld32,DI.fld33 AS DI_fld33,DI.fld34 AS DI_fld34,DI.fld35 AS DI_fld35,DI.fld36 AS DI_fld36,DI.fld37 AS DI_fld37,DI.fld38 AS DI_fld38,DI.fld39 AS DI_fld39,DI.fld40 AS DI_fld40,DI.ISAUTH AS DI_ISAUTH ,D.tid AS D_tid,D.EID AS D_EID,D.gID AS D_gID,D.folderName AS D_folderName,D.dord AS D_dord,D.Stid AS D_Stid,D.FName AS D_FName,D.description AS D_description,D.docUrl AS D_docUrl,D.DocImage AS D_DocImage,D.cnt AS D_cnt,D.aDate AS D_aDate,D.oUID AS D_oUID,D.filesize AS D_filesize,D.fld1 AS D_fld1,D.fld2 AS D_fld2,D.fld3 AS D_fld3,D.fld4 AS D_fld4,D.fld5 AS D_fld5,D.fld6 AS D_fld6,D.fld7 AS D_fld7,D.fld8 AS D_fld8,D.fld9 AS D_fld9,D.fld10 AS D_fld10,D.fld11 AS D_fld11,D.fld12 AS D_fld12,D.fld13 AS D_fld13,D.fld14 AS D_fld14,D.fld15 AS D_fld15,D.fld16 AS D_fld16,D.fld17 AS D_fld17,D.fld18 AS D_fld18,D.fld19 AS D_fld19,D.fld20 AS D_fld20,D.SessionID AS D_SessionID,D.curstatus AS D_curstatus,D.isWorkFlow AS D_isWorkFlow,D.LastTID AS D_LastTID,D.DocumentType AS D_DocumentType,D.fld21 AS D_fld21,D.fld22 AS D_fld22,D.fld23 AS D_fld23,D.fld24 AS D_fld24,D.fld25 AS D_fld25,D.fld26 AS D_fld26,D.fld27 AS D_fld27,D.fld28 AS D_fld28,D.fld29 AS D_fld29,D.fld30 AS D_fld30,D.fld31 AS D_fld31,D.fld32 AS D_fld32,D.fld33 AS D_fld33,D.fld34 AS D_fld34,D.fld35 AS D_fld35,D.fld36 AS D_fld36,D.fld37 AS D_fld37,D.fld38 AS D_fld38,D.fld39 AS D_fld39,D.fld40 AS D_fld40,D.isauth AS D_isauth,D.fld41 AS D_fld41,D.fld42 AS D_fld42,D.fld43 AS D_fld43,D.fld44 AS D_fld44,D.fld45 AS D_fld45,D.fld46 AS D_fld46,D.fld47 AS D_fld47,D.fld48 AS D_fld48,D.fld49 AS D_fld49,D.fld50 AS D_fld50,D.CurDocNature AS D_CurDocNature,D.fld51 AS D_fld51,D.fld52 AS D_fld52,D.fld53 AS D_fld53,D.fld54 AS D_fld54,D.fld55 AS D_fld55,D.fld56 AS D_fld56,D.fld57 AS D_fld57,D.fld58 AS D_fld58,D.fld59 AS D_fld59,D.fld60 AS D_fld60,D.fld61 AS D_fld61,D.fld62 AS D_fld62,D.fld63 AS D_fld63,D.fld64 AS D_fld64,D.fld65 AS D_fld65,D.fld66 AS D_fld66,D.fld67 AS D_fld67,D.fld68 AS D_fld68,D.fld69 AS D_fld69,D.fld70 AS D_fld70,D.fld71 AS  D_fld71,D.fld72 AS  D_fld72,D.fld73 AS  D_fld73,D.fld74 AS  D_fld74,D.fld75 AS  D_fld75,D.fld76 AS  D_fld76,D.fld77 AS  D_fld77,D.fld78 AS  D_fld78,D.fld79 AS  D_fld79,D.fld80 AS  D_fld80,D.fld81 AS  D_fld81,D.fld82 AS  D_fld82,D.fld83 AS  D_fld83,D.fld84 AS  D_fld84,D.fld85 AS  D_fld85,D.fld86 AS  D_fld86,D.fld87 AS  D_fld87,D.fld88 AS  D_fld88,D.fld89 AS  D_fld89,D.fld90 AS  D_fld90,D.fld91 AS  D_fld91,D.fld92 AS  D_fld92,D.fld93 AS  D_fld93,D.fld94 AS  D_fld94,D.fld95 AS  D_fld95,D.fld96 AS  D_fld96,D.fld97 AS  D_fld97,D.fld98 AS  D_fld98,D.fld99 AS  D_fld99,D.fld100 AS  D_fld100,D.fld101 AS  D_fld101,D.fld102 AS  D_fld102,D.fld103 AS  D_fld103,D.fld104 AS  D_fld104,D.fld105 AS  D_fld105,D.fld106 AS  D_fld106,D.fld107 AS  D_fld107,D.fld108 AS  D_fld108,D.fld109 AS  D_fld109,D.fld110 AS  D_fld110,D.fld111 AS  D_fld111,D.fld112 AS  D_fld112,D.fld113 AS  D_fld113,D.fld114 AS  D_fld114,D.fld115 AS  D_fld115,D.fld116 AS  D_fld116,D.fld117 AS  D_fld117,D.fld118 AS  D_fld118,D.fld119 AS  D_fld119,D.fld120 AS  D_fld120,D.fld121 AS  D_fld121,D.fld122 AS  D_fld122,D.fld123 AS  D_fld123,D.fld124 AS  D_fld124,D.fld125 AS  D_fld125,D.fld126 AS  D_fld126,D.fld127 AS  D_fld127,D.fld128 AS  D_fld128,D.fld129 AS  D_fld129,D.fld130 AS  D_fld130,D.fld131 AS  D_fld131,D.fld132 AS  D_fld132,D.fld133 AS  D_fld133,D.fld134 AS  D_fld134,D.fld135 AS  D_fld135,D.fld136 AS  D_fld136,D.fld137 AS  D_fld137,D.fld138 AS  D_fld138,D.fld139 AS  D_fld139,D.fld140 AS  D_fld140,D.fld141 AS  D_fld141,D.fld142 AS  D_fld142,D.fld143 AS  D_fld143,D.fld144 AS  D_fld144,D.fld145 AS  D_fld145,D.fld146 AS  D_fld146,D.fld147 AS  D_fld147,D.fld148 AS  D_fld148,D.fld149 AS  D_fld149,D.fld150 AS  D_fld150,D.fld151 AS  D_fld151,D.fld152 AS  D_fld152,D.fld153 AS  D_fld153,D.fld154 AS  D_fld154,D.fld155 AS  D_fld155,D.fld156 AS  D_fld156,D.fld157 AS  D_fld157,D.fld158 AS  D_fld158,D.fld159 AS  D_fld159,D.fld160 AS  D_fld160,D.fld161 AS  D_fld161,D.fld162 AS  D_fld162,D.fld163 AS  D_fld163,D.fld164 AS  D_fld164,D.fld165 AS  D_fld165,D.fld166 AS  D_fld166,D.fld167 AS  D_fld167,D.fld168 AS  D_fld168,D.fld169 AS  D_fld169,D.fld170 AS  D_fld170,D.fld171 AS  D_fld171,D.fld172 AS  D_fld172,D.fld173 AS  D_fld173,D.fld174 AS  D_fld174,D.fld175 AS  D_fld175,D.fld176 AS  D_fld176,D.fld177 AS  D_fld177,D.fld178 AS  D_fld178,D.fld179 AS  D_fld179,D.fld180 AS  D_fld180,D.fld181 AS  D_fld181,D.fld182 AS  D_fld182,D.fld183 AS  D_fld183,D.fld184 AS  D_fld184,D.fld185 AS  D_fld185,D.fld186 AS  D_fld186,D.fld187 AS  D_fld187,D.fld188 AS  D_fld188,D.fld189 AS  D_fld189,D.fld190 AS  D_fld190,D.fld191 AS  D_fld191,D.fld192 AS  D_fld192,D.fld193 AS  D_fld193,D.fld194 AS  D_fld194,D.fld195 AS  D_fld195,D.fld196 AS  D_fld196,D.fld197 AS  D_fld197,D.fld198 AS  D_fld198,D.fld199 AS  D_fld199,D.fld200 AS  D_fld200, D.fld201 AS  D_fld201, D.fld202 AS  D_fld202, D.fld203 AS  D_fld203, D.fld204 AS  D_fld204, D.fld205 AS  D_fld205, D.fld206 AS  D_fld206, D.fld207 AS  D_fld207, D.fld208 AS  D_fld208, D.fld209 AS  D_fld209 , D.fld210 AS  D_fld210, D.fld211 AS  D_fld211, D.fld212 AS D_fld212, D.fld213 AS D_fld213, D.fld214 AS D_fld214, D.fld215 AS D_fld215, D.fld216 AS D_fld216, D.fld217 AS D_fld217, D.fld218 AS D_fld218, D.fld219 AS D_fld219, D.fld220 AS D_fld220, D.fld221 AS D_fld221, D.fld222 AS D_fld222, D.fld223 AS D_fld223, D.fld224 AS D_fld224, D.fld225 AS D_fld225    FROM MMM_MST_DOC D LEFT OUTER JOIN MMM_MST_DOC_ITEM DI ON D.tid =DI.DOCID    where D.TID=" & fileID & " and D.eid='" & EID & "' ORDER  BY DI.TID DESC"
                        End If
                        '' prev
                        'da.SelectCommand.CommandText = "SELECT DI.TID AS DI_TID,DI.DOCID AS DI_DOCID,DI.SESSIONID AS DI_SESSIONID,DI.FLD1 AS DI_FLD1,DI.FLD2 AS DI_FLD2,DI.FLD3 AS DI_FLD3,DI.FLD4 AS DI_FLD4,DI.FLD5 AS DI_FLD5,DI.FLD6 AS DI_FLD6,DI.FLD7 AS DI_FLD7,DI.FLD8 AS DI_FLD8,DI.FLD9 AS DI_FLD9,DI.FLD10 AS DI_FLD10,DI.FLD11 AS DI_FLD11,DI.FLD12 AS DI_FLD12,DI.FLD13 AS DI_FLD13,DI.FLD14 AS DI_FLD14,DI.FLD15 AS DI_FLD15,DI.FLD16 AS DI_FLD16,DI.FLD17 AS DI_FLD17,DI.FLD18 AS DI_FLD18,DI.FLD19 AS DI_FLD19,DI.FLD20 AS DI_FLD20,DI.documenttype AS DI_documenttype,DI.fld21 AS DI_fld21,DI.fld22 AS DI_fld22,DI.fld23 AS DI_fld23,DI.fld24 AS DI_fld24,DI.fld25 AS DI_fld25,DI.fld26 AS DI_fld26,DI.fld27 AS DI_fld27,DI.fld28 AS DI_fld28,DI.fld29 AS DI_fld29,DI.fld30 AS DI_fld30,DI.fld31 AS DI_fld31,DI.fld32 AS DI_fld32,DI.fld33 AS DI_fld33,DI.fld34 AS DI_fld34,DI.fld35 AS DI_fld35,DI.fld36 AS DI_fld36,DI.fld37 AS DI_fld37,DI.fld38 AS DI_fld38,DI.fld39 AS DI_fld39,DI.fld40 AS DI_fld40,DI.ISAUTH AS DI_ISAUTH ,D.tid AS D_tid,D.EID AS D_EID,D.gID AS D_gID,D.folderName AS D_folderName,D.dord AS D_dord,D.Stid AS D_Stid,D.FName AS D_FName,D.description AS D_description,D.docUrl AS D_docUrl,D.DocImage AS D_DocImage,D.cnt AS D_cnt,D.aDate AS D_aDate,D.oUID AS D_oUID,D.filesize AS D_filesize,D.fld1 AS D_fld1,D.fld2 AS D_fld2,D.fld3 AS D_fld3,D.fld4 AS D_fld4,D.fld5 AS D_fld5,D.fld6 AS D_fld6,D.fld7 AS D_fld7,D.fld8 AS D_fld8,D.fld9 AS D_fld9,D.fld10 AS D_fld10,D.fld11 AS D_fld11,D.fld12 AS D_fld12,D.fld13 AS D_fld13,D.fld14 AS D_fld14,D.fld15 AS D_fld15,D.fld16 AS D_fld16,D.fld17 AS D_fld17,D.fld18 AS D_fld18,D.fld19 AS D_fld19,D.fld20 AS D_fld20,D.SessionID AS D_SessionID,D.curstatus AS D_curstatus,D.isWorkFlow AS D_isWorkFlow,D.LastTID AS D_LastTID,D.DocumentType AS D_DocumentType,D.fld21 AS D_fld21,D.fld22 AS D_fld22,D.fld23 AS D_fld23,D.fld24 AS D_fld24,D.fld25 AS D_fld25,D.fld26 AS D_fld26,D.fld27 AS D_fld27,D.fld28 AS D_fld28,D.fld29 AS D_fld29,D.fld30 AS D_fld30,D.fld31 AS D_fld31,D.fld32 AS D_fld32,D.fld33 AS D_fld33,D.fld34 AS D_fld34,D.fld35 AS D_fld35,D.fld36 AS D_fld36,D.fld37 AS D_fld37,D.fld38 AS D_fld38,D.fld39 AS D_fld39,D.fld40 AS D_fld40,D.isauth AS D_isauth,D.fld41 AS D_fld41,D.fld42 AS D_fld42,D.fld43 AS D_fld43,D.fld44 AS D_fld44,D.fld45 AS D_fld45,D.fld46 AS D_fld46,D.fld47 AS D_fld47,D.fld48 AS D_fld48,D.fld49 AS D_fld49,D.fld50 AS D_fld50,D.CurDocNature AS D_CurDocNature,D.fld51 AS D_fld51,D.fld52 AS D_fld52,D.fld53 AS D_fld53,D.fld54 AS D_fld54,D.fld55 AS D_fld55,D.fld56 AS D_fld56,D.fld57 AS D_fld57,D.fld58 AS D_fld58,D.fld59 AS D_fld59,D.fld60 AS D_fld60,D.fld61 AS D_fld61,D.fld62 AS D_fld62,D.fld63 AS D_fld63,D.fld64 AS D_fld64,D.fld65 AS D_fld65,D.fld66 AS D_fld66,D.fld67 AS D_fld67,D.fld68 AS D_fld68,D.fld69 AS D_fld69,D.fld70 AS D_fld70 FROM MMM_MST_DOC D LEFT OUTER JOIN MMM_MST_DOC_ITEM DI ON D.tid =DI.DOCID    where D.TID=" & fileID & " and D.eid='" & EID & "' ORDER  BY DI.TID DESC"
                        Dim tbl As New DataTable
                        da.Fill(tbl)
                        For i As Integer = 0 To dsTrg.Tables("tbltrg").Rows.Count - 1
                            rStr = dsTrg.Tables("tbltrg").Rows(i).Item("Triggertext").ToString()
                            For Each column As DataColumn In tbl.Columns
                                ' column.ColumnName
                                Dim test As String = "{" & column.ColumnName & "}"
                                If rStr.Contains("{" & column.ColumnName & "}") Then
                                    rStr = rStr.Replace("{" & column.ColumnName & "}", "'" & tbl.Rows(0).Item(column.ColumnName) & "'")
                                End If
                            Next
                            If TriggerText = "" Then
                                TriggerText = rStr
                            Else
                                TriggerText = TriggerText & ";" & rStr
                            End If
                            '' new added
                            Try

                                ' Dim conStr1 As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
                                ' Dim con1 As SqlConnection = New SqlConnection(conStr1)
                                Dim da1 As SqlDataAdapter = New SqlDataAdapter("", con)
                                da1.SelectCommand.CommandType = CommandType.StoredProcedure
                                da1.SelectCommand.CommandText = "insert_mmm_mst_triggers_res"
                                da1.SelectCommand.Parameters.Clear()
                                da1.SelectCommand.Parameters.AddWithValue("@eid", EID)
                                da1.SelectCommand.Parameters.AddWithValue("@docid", fileID)
                                da1.SelectCommand.Parameters.AddWithValue("@Formname", FormName)
                                da1.SelectCommand.Parameters.AddWithValue("@TriggerText", rStr)
                                Dim dt As New DataTable()
                                da1.Fill(dt)
                            Catch ex As Exception

                            End Try
                            '' new added for log
                        Next
                    End If
                End If
            Catch ex As Exception
                Dim str = ex.Message
            End Try
            If TriggerText <> "" Then
                Try
                    con = New SqlConnection(conStr)
                    da = New SqlDataAdapter("ExecuteTrigger", con)
                    da.SelectCommand.CommandType = CommandType.StoredProcedure
                    da.SelectCommand.Parameters.AddWithValue("@TriggerText", TriggerText)
                    If con.State <> ConnectionState.Open Then
                        con.Open()
                    End If
                    Result = (Convert.ToInt32(da.SelectCommand.ExecuteScalar()))
                Catch ex As Exception
                End Try
            End If
        Catch ex As Exception
        Finally
            con.Close()
            con.Dispose()
            da.Dispose()
        End Try
    End Sub

    ', Optional ByVal BeforeWorkflow As Integer = 0
    Public Shared Sub ExecuteTriggerT(ByVal FormName As String, ByVal EID As Integer, ByVal fileID As Integer, con As SqlConnection, tran As SqlTransaction, Optional ByVal IsdetailsForm As Integer = 0, Optional TriggerNature As String = "Create", Optional FormType As String = "Document")
        Dim Result As Integer = -2
        Dim TriggerText As String = ""
        Dim dsTrg As DataSet
        'Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        'Dim con As SqlConnection = New SqlConnection(conStr)
        Dim da As SqlDataAdapter = New SqlDataAdapter("", con)
        Try
            Try
                Dim OnEdit As Integer = 0, OnCreate As Integer = 0
                If TriggerNature.ToUpper() = "CREATE" Then
                    OnCreate = 1
                Else
                    OnEdit = 1
                End If
                dsTrg = GetTriggersT(EID, fileID, FormName, con, tran, IsdetailsForm, OnEdit, OnCreate, FormType)
                Dim str As String = ""
                Dim rStr As String = ""
                If dsTrg.Tables("tbltrg").Rows.Count > 0 Then
                    If FormName.ToUpper.Trim() = dsTrg.Tables("tbltrg").Rows(0).Item("doctype").ToString.ToUpper.Trim() Then
                        da.SelectCommand.CommandType = CommandType.Text
                        da.SelectCommand.Parameters.Clear()
                        If FormType.ToUpper = "MASTER" Then
                            da.SelectCommand.CommandText = "SELECT D.tid AS D_tid,D.EID AS D_EID,D.aDate AS D_aDate,D.fld1 AS D_fld1,D.fld2 AS D_fld2,D.fld3 AS D_fld3,D.fld4 AS D_fld4,D.fld5 AS D_fld5,D.fld6 AS D_fld6,D.fld7 AS D_fld7,D.fld8 AS D_fld8,D.fld9 AS D_fld9,D.fld10 AS D_fld10,D.fld11 AS D_fld11,D.fld12 AS D_fld12,D.fld13 AS D_fld13,D.fld14 AS D_fld14,D.fld15 AS D_fld15,D.fld16 AS D_fld16,D.fld17 AS D_fld17,D.fld18 AS D_fld18,D.fld19 AS D_fld19,D.fld20 AS D_fld20,D.DocumentType AS D_DocumentType,D.fld21 AS D_fld21,D.fld22 AS D_fld22,D.fld23 AS D_fld23,D.fld24 AS D_fld24,D.fld25 AS D_fld25,D.fld26 AS D_fld26,D.fld27 AS D_fld27,D.fld28 AS D_fld28,D.fld29 AS D_fld29,D.fld30 AS D_fld30,D.fld31 AS D_fld31,D.fld32 AS D_fld32,D.fld33 AS D_fld33,D.fld34 AS D_fld34,D.fld35 AS D_fld35,D.fld36 AS D_fld36,D.fld37 AS D_fld37,D.fld38 AS D_fld38,D.fld39 AS D_fld39,D.fld40 AS D_fld40,D.isauth AS D_isauth,D.fld41 AS D_fld41,D.fld42 AS D_fld42,D.fld43 AS D_fld43,D.fld44 AS D_fld44,D.fld45 AS D_fld45,D.fld46 AS D_fld46,D.fld47 AS D_fld47,D.fld48 AS D_fld48,D.fld49 AS D_fld49,D.fld50 AS D_fld50,D.fld51 AS D_fld51,D.fld52 AS D_fld52,D.fld53 AS D_fld53,D.fld54 AS D_fld54,D.fld55 AS D_fld55,D.fld56 AS D_fld56,D.fld57 AS D_fld57,D.fld58 AS D_fld58,D.fld59 AS D_fld59,D.fld60 AS D_fld60,D.fld61 AS D_fld61,D.fld62 AS D_fld62,D.fld63 AS D_fld63,D.fld64 AS D_fld64,D.fld65 AS D_fld65,D.fld66 AS D_fld66,D.fld67 AS D_fld67,D.fld68 AS D_fld68,D.fld69 AS D_fld69,D.fld70 AS D_fld70,D.fld71 AS  D_fld71,D.fld72 AS  D_fld72,D.fld73 AS  D_fld73,D.fld74 AS  D_fld74,D.fld75 AS  D_fld75,D.fld76 AS  D_fld76,D.fld77 AS  D_fld77,D.fld78 AS  D_fld78,D.fld79 AS  D_fld79,D.fld80 AS  D_fld80,D.fld81 AS  D_fld81,D.fld82 AS  D_fld82,D.fld83 AS  D_fld83,D.fld84 AS  D_fld84,D.fld85 AS  D_fld85,D.fld86 AS  D_fld86,D.fld87 AS  D_fld87,D.fld88 AS  D_fld88,D.fld89 AS  D_fld89,D.fld90 AS  D_fld90,D.fld91 AS  D_fld91,D.fld92 AS  D_fld92,D.fld93 AS  D_fld93,D.fld94 AS  D_fld94,D.fld95 AS  D_fld95,D.fld96 AS  D_fld96,D.fld97 AS  D_fld97,D.fld98 AS  D_fld98,D.fld99 AS  D_fld99,D.fld100 AS  D_fld100,D.fld101 AS  D_fld101,D.fld102 AS  D_fld102,D.fld103 AS  D_fld103,D.fld104 AS  D_fld104,D.fld105 AS  D_fld105,D.fld106 AS  D_fld106,D.fld107 AS  D_fld107,D.fld108 AS  D_fld108,D.fld109 AS  D_fld109,D.fld110 AS  D_fld110,D.fld111 AS  D_fld111,D.fld112 AS  D_fld112,D.fld113 AS  D_fld113,D.fld114 AS  D_fld114,D.fld115 AS  D_fld115,D.fld116 AS  D_fld116,D.fld117 AS  D_fld117,D.fld118 AS  D_fld118,D.fld119 AS  D_fld119,D.fld120 AS  D_fld120,D.fld121 AS  D_fld121,D.fld122 AS  D_fld122,D.fld123 AS  D_fld123,D.fld124 AS  D_fld124,D.fld125 AS  D_fld125,D.fld126 AS  D_fld126,D.fld127 AS  D_fld127,D.fld128 AS  D_fld128,D.fld129 AS  D_fld129,D.fld130 AS  D_fld130,D.fld131 AS  D_fld131,D.fld132 AS  D_fld132,D.fld133 AS  D_fld133,D.fld134 AS  D_fld134,D.fld135 AS  D_fld135,D.fld136 AS  D_fld136,D.fld137 AS  D_fld137,D.fld138 AS  D_fld138,D.fld139 AS  D_fld139,D.fld140 AS  D_fld140,D.fld141 AS  D_fld141,D.fld142 AS  D_fld142,D.fld143 AS  D_fld143,D.fld144 AS  D_fld144,D.fld145 AS  D_fld145,D.fld146 AS  D_fld146,D.fld147 AS  D_fld147,D.fld148 AS  D_fld148,D.fld149 AS  D_fld149,D.fld150 AS  D_fld150,D.fld151 AS  D_fld151,D.fld152 AS  D_fld152,D.fld153 AS  D_fld153,D.fld154 AS  D_fld154,D.fld155 AS  D_fld155,D.fld156 AS  D_fld156,D.fld157 AS  D_fld157,D.fld158 AS  D_fld158,D.fld159 AS  D_fld159,D.fld160 AS  D_fld160,D.fld161 AS  D_fld161,D.fld162 AS  D_fld162,D.fld163 AS  D_fld163,D.fld164 AS  D_fld164,D.fld165 AS  D_fld165,D.fld166 AS  D_fld166,D.fld167 AS  D_fld167,D.fld168 AS  D_fld168,D.fld169 AS  D_fld169,D.fld170 AS  D_fld170,D.fld171 AS  D_fld171,D.fld172 AS  D_fld172,D.fld173 AS  D_fld173,D.fld174 AS  D_fld174,D.fld175 AS  D_fld175,D.fld176 AS  D_fld176,D.fld177 AS  D_fld177,D.fld178 AS  D_fld178,D.fld179 AS  D_fld179,D.fld180 AS  D_fld180,D.fld181 AS  D_fld181,D.fld182 AS  D_fld182,D.fld183 AS  D_fld183,D.fld184 AS  D_fld184,D.fld185 AS  D_fld185,D.fld186 AS  D_fld186,D.fld187 AS  D_fld187,D.fld188 AS  D_fld188,D.fld189 AS  D_fld189,D.fld190 AS  D_fld190,D.fld191 AS  D_fld191,D.fld192 AS  D_fld192,D.fld193 AS  D_fld193,D.fld194 AS  D_fld194,D.fld195 AS  D_fld195,D.fld196 AS  D_fld196,D.fld197 AS  D_fld197,D.fld198 AS  D_fld198,D.fld199 AS  D_fld199, D.fld200 AS  D_fld200, D.fld201 AS  D_fld201, D.fld202 AS  D_fld202, D.fld203 AS  D_fld203, D.fld204 AS  D_fld204, D.fld205 AS  D_fld205, D.fld206 AS  D_fld206, D.fld207 AS  D_fld207, D.fld208 AS  D_fld208, D.fld209 AS  D_fld209 , D.fld210 AS  D_fld210, D.fld211 AS  D_fld211, D.fld212 AS D_fld212, D.fld213 AS D_fld213, D.fld214 AS D_fld214, D.fld215 AS D_fld215, D.fld216 AS D_fld216, D.fld217 AS D_fld217, D.fld218 AS D_fld218, D.fld219 AS D_fld219, D.fld220 AS D_fld220, D.fld221 AS D_fld221, D.fld222 AS D_fld222, D.fld223 AS D_fld223, D.fld224 AS D_fld224, D.fld225 AS D_fld225  FROM MMM_MST_MASTER D  where  D.TID=" & fileID & " and D.eid='" & EID & "'"
                        Else
                            da.SelectCommand.CommandText = "SELECT DI.TID AS DI_TID,DI.DOCID AS DI_DOCID,DI.SESSIONID AS DI_SESSIONID,DI.FLD1 AS DI_FLD1,DI.FLD2 AS DI_FLD2,DI.FLD3 AS DI_FLD3,DI.FLD4 AS DI_FLD4,DI.FLD5 AS DI_FLD5,DI.FLD6 AS DI_FLD6,DI.FLD7 AS DI_FLD7,DI.FLD8 AS DI_FLD8,DI.FLD9 AS DI_FLD9,DI.FLD10 AS DI_FLD10,DI.FLD11 AS DI_FLD11,DI.FLD12 AS DI_FLD12,DI.FLD13 AS DI_FLD13,DI.FLD14 AS DI_FLD14,DI.FLD15 AS DI_FLD15,DI.FLD16 AS DI_FLD16,DI.FLD17 AS DI_FLD17,DI.FLD18 AS DI_FLD18,DI.FLD19 AS DI_FLD19,DI.FLD20 AS DI_FLD20,DI.documenttype AS DI_documenttype,DI.fld21 AS DI_fld21,DI.fld22 AS DI_fld22,DI.fld23 AS DI_fld23,DI.fld24 AS DI_fld24,DI.fld25 AS DI_fld25,DI.fld26 AS DI_fld26,DI.fld27 AS DI_fld27,DI.fld28 AS DI_fld28,DI.fld29 AS DI_fld29,DI.fld30 AS DI_fld30,DI.fld31 AS DI_fld31,DI.fld32 AS DI_fld32,DI.fld33 AS DI_fld33,DI.fld34 AS DI_fld34,DI.fld35 AS DI_fld35,DI.fld36 AS DI_fld36,DI.fld37 AS DI_fld37,DI.fld38 AS DI_fld38,DI.fld39 AS DI_fld39,DI.fld40 AS DI_fld40,DI.ISAUTH AS DI_ISAUTH ,D.tid AS D_tid,D.EID AS D_EID,D.gID AS D_gID,D.folderName AS D_folderName,D.dord AS D_dord,D.Stid AS D_Stid,D.FName AS D_FName,D.description AS D_description,D.docUrl AS D_docUrl,D.DocImage AS D_DocImage,D.cnt AS D_cnt,D.aDate AS D_aDate,D.oUID AS D_oUID,D.filesize AS D_filesize,D.fld1 AS D_fld1,D.fld2 AS D_fld2,D.fld3 AS D_fld3,D.fld4 AS D_fld4,D.fld5 AS D_fld5,D.fld6 AS D_fld6,D.fld7 AS D_fld7,D.fld8 AS D_fld8,D.fld9 AS D_fld9,D.fld10 AS D_fld10,D.fld11 AS D_fld11,D.fld12 AS D_fld12,D.fld13 AS D_fld13,D.fld14 AS D_fld14,D.fld15 AS D_fld15,D.fld16 AS D_fld16,D.fld17 AS D_fld17,D.fld18 AS D_fld18,D.fld19 AS D_fld19,D.fld20 AS D_fld20,D.SessionID AS D_SessionID,D.curstatus AS D_curstatus,D.isWorkFlow AS D_isWorkFlow,D.LastTID AS D_LastTID,D.DocumentType AS D_DocumentType,D.fld21 AS D_fld21,D.fld22 AS D_fld22,D.fld23 AS D_fld23,D.fld24 AS D_fld24,D.fld25 AS D_fld25,D.fld26 AS D_fld26,D.fld27 AS D_fld27,D.fld28 AS D_fld28,D.fld29 AS D_fld29,D.fld30 AS D_fld30,D.fld31 AS D_fld31,D.fld32 AS D_fld32,D.fld33 AS D_fld33,D.fld34 AS D_fld34,D.fld35 AS D_fld35,D.fld36 AS D_fld36,D.fld37 AS D_fld37,D.fld38 AS D_fld38,D.fld39 AS D_fld39,D.fld40 AS D_fld40,D.isauth AS D_isauth,D.fld41 AS D_fld41,D.fld42 AS D_fld42,D.fld43 AS D_fld43,D.fld44 AS D_fld44,D.fld45 AS D_fld45,D.fld46 AS D_fld46,D.fld47 AS D_fld47,D.fld48 AS D_fld48,D.fld49 AS D_fld49,D.fld50 AS D_fld50,D.CurDocNature AS D_CurDocNature,D.fld51 AS D_fld51,D.fld52 AS D_fld52,D.fld53 AS D_fld53,D.fld54 AS D_fld54,D.fld55 AS D_fld55,D.fld56 AS D_fld56,D.fld57 AS D_fld57,D.fld58 AS D_fld58,D.fld59 AS D_fld59,D.fld60 AS D_fld60,D.fld61 AS D_fld61,D.fld62 AS D_fld62,D.fld63 AS D_fld63,D.fld64 AS D_fld64,D.fld65 AS D_fld65,D.fld66 AS D_fld66,D.fld67 AS D_fld67,D.fld68 AS D_fld68,D.fld69 AS D_fld69,D.fld70 AS D_fld70,D.fld71 AS  D_fld71,D.fld72 AS  D_fld72,D.fld73 AS  D_fld73,D.fld74 AS  D_fld74,D.fld75 AS  D_fld75,D.fld76 AS  D_fld76,D.fld77 AS  D_fld77,D.fld78 AS  D_fld78,D.fld79 AS  D_fld79,D.fld80 AS  D_fld80,D.fld81 AS  D_fld81,D.fld82 AS  D_fld82,D.fld83 AS  D_fld83,D.fld84 AS  D_fld84,D.fld85 AS  D_fld85,D.fld86 AS  D_fld86,D.fld87 AS  D_fld87,D.fld88 AS  D_fld88,D.fld89 AS  D_fld89,D.fld90 AS  D_fld90,D.fld91 AS  D_fld91,D.fld92 AS  D_fld92,D.fld93 AS  D_fld93,D.fld94 AS  D_fld94,D.fld95 AS  D_fld95,D.fld96 AS  D_fld96,D.fld97 AS  D_fld97,D.fld98 AS  D_fld98,D.fld99 AS  D_fld99,D.fld100 AS  D_fld100,D.fld101 AS  D_fld101,D.fld102 AS  D_fld102,D.fld103 AS  D_fld103,D.fld104 AS  D_fld104,D.fld105 AS  D_fld105,D.fld106 AS  D_fld106,D.fld107 AS  D_fld107,D.fld108 AS  D_fld108,D.fld109 AS  D_fld109,D.fld110 AS  D_fld110,D.fld111 AS  D_fld111,D.fld112 AS  D_fld112,D.fld113 AS  D_fld113,D.fld114 AS  D_fld114,D.fld115 AS  D_fld115,D.fld116 AS  D_fld116,D.fld117 AS  D_fld117,D.fld118 AS  D_fld118,D.fld119 AS  D_fld119,D.fld120 AS  D_fld120,D.fld121 AS  D_fld121,D.fld122 AS  D_fld122,D.fld123 AS  D_fld123,D.fld124 AS  D_fld124,D.fld125 AS  D_fld125,D.fld126 AS  D_fld126,D.fld127 AS  D_fld127,D.fld128 AS  D_fld128,D.fld129 AS  D_fld129,D.fld130 AS  D_fld130,D.fld131 AS  D_fld131,D.fld132 AS  D_fld132,D.fld133 AS  D_fld133,D.fld134 AS  D_fld134,D.fld135 AS  D_fld135,D.fld136 AS  D_fld136,D.fld137 AS  D_fld137,D.fld138 AS  D_fld138,D.fld139 AS  D_fld139,D.fld140 AS  D_fld140,D.fld141 AS  D_fld141,D.fld142 AS  D_fld142,D.fld143 AS  D_fld143,D.fld144 AS  D_fld144,D.fld145 AS  D_fld145,D.fld146 AS  D_fld146,D.fld147 AS  D_fld147,D.fld148 AS  D_fld148,D.fld149 AS  D_fld149,D.fld150 AS  D_fld150,D.fld151 AS  D_fld151,D.fld152 AS  D_fld152,D.fld153 AS  D_fld153,D.fld154 AS  D_fld154,D.fld155 AS  D_fld155,D.fld156 AS  D_fld156,D.fld157 AS  D_fld157,D.fld158 AS  D_fld158,D.fld159 AS  D_fld159,D.fld160 AS  D_fld160,D.fld161 AS  D_fld161,D.fld162 AS  D_fld162,D.fld163 AS  D_fld163,D.fld164 AS  D_fld164,D.fld165 AS  D_fld165,D.fld166 AS  D_fld166,D.fld167 AS  D_fld167,D.fld168 AS  D_fld168,D.fld169 AS  D_fld169,D.fld170 AS  D_fld170,D.fld171 AS  D_fld171,D.fld172 AS  D_fld172,D.fld173 AS  D_fld173,D.fld174 AS  D_fld174,D.fld175 AS  D_fld175,D.fld176 AS  D_fld176,D.fld177 AS  D_fld177,D.fld178 AS  D_fld178,D.fld179 AS  D_fld179,D.fld180 AS  D_fld180,D.fld181 AS  D_fld181,D.fld182 AS  D_fld182,D.fld183 AS  D_fld183,D.fld184 AS  D_fld184,D.fld185 AS  D_fld185,D.fld186 AS  D_fld186,D.fld187 AS  D_fld187,D.fld188 AS  D_fld188,D.fld189 AS  D_fld189,D.fld190 AS  D_fld190,D.fld191 AS  D_fld191,D.fld192 AS  D_fld192,D.fld193 AS  D_fld193,D.fld194 AS  D_fld194,D.fld195 AS  D_fld195,D.fld196 AS  D_fld196,D.fld197 AS  D_fld197,D.fld198 AS  D_fld198,D.fld199 AS  D_fld199,D.fld200 AS  D_fld200, D.fld201 AS  D_fld201, D.fld202 AS  D_fld202, D.fld203 AS  D_fld203, D.fld204 AS  D_fld204, D.fld205 AS  D_fld205, D.fld206 AS  D_fld206, D.fld207 AS  D_fld207, D.fld208 AS  D_fld208, D.fld209 AS  D_fld209 , D.fld210 AS  D_fld210, D.fld211 AS  D_fld211, D.fld212 AS D_fld212, D.fld213 AS D_fld213, D.fld214 AS D_fld214, D.fld215 AS D_fld215, D.fld216 AS D_fld216, D.fld217 AS D_fld217, D.fld218 AS D_fld218, D.fld219 AS D_fld219, D.fld220 AS D_fld220, D.fld221 AS D_fld221, D.fld222 AS D_fld222, D.fld223 AS D_fld223, D.fld224 AS D_fld224, D.fld225 AS D_fld225    FROM MMM_MST_DOC D LEFT OUTER JOIN MMM_MST_DOC_ITEM DI ON D.tid =DI.DOCID    where D.TID=" & fileID & " and D.eid='" & EID & "' ORDER  BY DI.TID DESC"
                        End If
                        'If FormType.ToUpper = "MASTER" Then
                        '    da.SelectCommand.CommandText = "SELECT D.tid AS D_tid,D.EID AS D_EID,D.aDate AS D_aDate,D.fld1 AS D_fld1,D.fld2 AS D_fld2,D.fld3 AS D_fld3,D.fld4 AS D_fld4,D.fld5 AS D_fld5,D.fld6 AS D_fld6,D.fld7 AS D_fld7,D.fld8 AS D_fld8,D.fld9 AS D_fld9,D.fld10 AS D_fld10,D.fld11 AS D_fld11,D.fld12 AS D_fld12,D.fld13 AS D_fld13,D.fld14 AS D_fld14,D.fld15 AS D_fld15,D.fld16 AS D_fld16,D.fld17 AS D_fld17,D.fld18 AS D_fld18,D.fld19 AS D_fld19,D.fld20 AS D_fld20,D.DocumentType AS D_DocumentType,D.fld21 AS D_fld21,D.fld22 AS D_fld22,D.fld23 AS D_fld23,D.fld24 AS D_fld24,D.fld25 AS D_fld25,D.fld26 AS D_fld26,D.fld27 AS D_fld27,D.fld28 AS D_fld28,D.fld29 AS D_fld29,D.fld30 AS D_fld30,D.fld31 AS D_fld31,D.fld32 AS D_fld32,D.fld33 AS D_fld33,D.fld34 AS D_fld34,D.fld35 AS D_fld35,D.fld36 AS D_fld36,D.fld37 AS D_fld37,D.fld38 AS D_fld38,D.fld39 AS D_fld39,D.fld40 AS D_fld40,D.isauth AS D_isauth,D.fld41 AS D_fld41,D.fld42 AS D_fld42,D.fld43 AS D_fld43,D.fld44 AS D_fld44,D.fld45 AS D_fld45,D.fld46 AS D_fld46,D.fld47 AS D_fld47,D.fld48 AS D_fld48,D.fld49 AS D_fld49,D.fld50 AS D_fld50,D.fld51 AS D_fld51,D.fld52 AS D_fld52,D.fld53 AS D_fld53,D.fld54 AS D_fld54,D.fld55 AS D_fld55,D.fld56 AS D_fld56,D.fld57 AS D_fld57,D.fld58 AS D_fld58,D.fld59 AS D_fld59,D.fld60 AS D_fld60,D.fld61 AS D_fld61,D.fld62 AS D_fld62,D.fld63 AS D_fld63,D.fld64 AS D_fld64,D.fld65 AS D_fld65,D.fld66 AS D_fld66,D.fld67 AS D_fld67,D.fld68 AS D_fld68,D.fld69 AS D_fld69,D.fld70 AS D_fld70 FROM MMM_MST_MASTER D  where  D.TID=" & fileID & " and D.eid='" & EID & "'"
                        'Else
                        '    da.SelectCommand.CommandText = "SELECT DI.TID AS DI_TID,DI.DOCID AS DI_DOCID,DI.SESSIONID AS DI_SESSIONID,DI.FLD1 AS DI_FLD1,DI.FLD2 AS DI_FLD2,DI.FLD3 AS DI_FLD3,DI.FLD4 AS DI_FLD4,DI.FLD5 AS DI_FLD5,DI.FLD6 AS DI_FLD6,DI.FLD7 AS DI_FLD7,DI.FLD8 AS DI_FLD8,DI.FLD9 AS DI_FLD9,DI.FLD10 AS DI_FLD10,DI.FLD11 AS DI_FLD11,DI.FLD12 AS DI_FLD12,DI.FLD13 AS DI_FLD13,DI.FLD14 AS DI_FLD14,DI.FLD15 AS DI_FLD15,DI.FLD16 AS DI_FLD16,DI.FLD17 AS DI_FLD17,DI.FLD18 AS DI_FLD18,DI.FLD19 AS DI_FLD19,DI.FLD20 AS DI_FLD20,DI.documenttype AS DI_documenttype,DI.fld21 AS DI_fld21,DI.fld22 AS DI_fld22,DI.fld23 AS DI_fld23,DI.fld24 AS DI_fld24,DI.fld25 AS DI_fld25,DI.fld26 AS DI_fld26,DI.fld27 AS DI_fld27,DI.fld28 AS DI_fld28,DI.fld29 AS DI_fld29,DI.fld30 AS DI_fld30,DI.fld31 AS DI_fld31,DI.fld32 AS DI_fld32,DI.fld33 AS DI_fld33,DI.fld34 AS DI_fld34,DI.fld35 AS DI_fld35,DI.fld36 AS DI_fld36,DI.fld37 AS DI_fld37,DI.fld38 AS DI_fld38,DI.fld39 AS DI_fld39,DI.fld40 AS DI_fld40,DI.ISAUTH AS DI_ISAUTH ,D.tid AS D_tid,D.EID AS D_EID,D.gID AS D_gID,D.folderName AS D_folderName,D.dord AS D_dord,D.Stid AS D_Stid,D.FName AS D_FName,D.description AS D_description,D.docUrl AS D_docUrl,D.DocImage AS D_DocImage,D.cnt AS D_cnt,D.aDate AS D_aDate,D.oUID AS D_oUID,D.filesize AS D_filesize,D.fld1 AS D_fld1,D.fld2 AS D_fld2,D.fld3 AS D_fld3,D.fld4 AS D_fld4,D.fld5 AS D_fld5,D.fld6 AS D_fld6,D.fld7 AS D_fld7,D.fld8 AS D_fld8,D.fld9 AS D_fld9,D.fld10 AS D_fld10,D.fld11 AS D_fld11,D.fld12 AS D_fld12,D.fld13 AS D_fld13,D.fld14 AS D_fld14,D.fld15 AS D_fld15,D.fld16 AS D_fld16,D.fld17 AS D_fld17,D.fld18 AS D_fld18,D.fld19 AS D_fld19,D.fld20 AS D_fld20,D.SessionID AS D_SessionID,D.curstatus AS D_curstatus,D.isWorkFlow AS D_isWorkFlow,D.LastTID AS D_LastTID,D.DocumentType AS D_DocumentType,D.fld21 AS D_fld21,D.fld22 AS D_fld22,D.fld23 AS D_fld23,D.fld24 AS D_fld24,D.fld25 AS D_fld25,D.fld26 AS D_fld26,D.fld27 AS D_fld27,D.fld28 AS D_fld28,D.fld29 AS D_fld29,D.fld30 AS D_fld30,D.fld31 AS D_fld31,D.fld32 AS D_fld32,D.fld33 AS D_fld33,D.fld34 AS D_fld34,D.fld35 AS D_fld35,D.fld36 AS D_fld36,D.fld37 AS D_fld37,D.fld38 AS D_fld38,D.fld39 AS D_fld39,D.fld40 AS D_fld40,D.isauth AS D_isauth,D.fld41 AS D_fld41,D.fld42 AS D_fld42,D.fld43 AS D_fld43,D.fld44 AS D_fld44,D.fld45 AS D_fld45,D.fld46 AS D_fld46,D.fld47 AS D_fld47,D.fld48 AS D_fld48,D.fld49 AS D_fld49,D.fld50 AS D_fld50,D.CurDocNature AS D_CurDocNature,D.fld51 AS D_fld51,D.fld52 AS D_fld52,D.fld53 AS D_fld53,D.fld54 AS D_fld54,D.fld55 AS D_fld55,D.fld56 AS D_fld56,D.fld57 AS D_fld57,D.fld58 AS D_fld58,D.fld59 AS D_fld59,D.fld60 AS D_fld60,D.fld61 AS D_fld61,D.fld62 AS D_fld62,D.fld63 AS D_fld63,D.fld64 AS D_fld64,D.fld65 AS D_fld65,D.fld66 AS D_fld66,D.fld67 AS D_fld67,D.fld68 AS D_fld68,D.fld69 AS D_fld69,D.fld70 AS D_fld70 FROM MMM_MST_DOC D LEFT OUTER JOIN MMM_MST_DOC_ITEM DI ON D.tid =DI.DOCID    where D.TID=" & fileID & " and D.eid='" & EID & "' ORDER  BY DI.TID DESC"
                        'End If
                        da.SelectCommand.Transaction = tran
                        Dim tbl As New DataTable
                        da.Fill(tbl)
                        For i As Integer = 0 To dsTrg.Tables("tbltrg").Rows.Count - 1
                            rStr = dsTrg.Tables("tbltrg").Rows(i).Item("Triggertext").ToString()
                            For Each column As DataColumn In tbl.Columns
                                ' column.ColumnName
                                Dim test As String = "{" & column.ColumnName & "}"
                                If rStr.Contains("{" & column.ColumnName & "}") Then
                                    rStr = rStr.Replace("{" & column.ColumnName & "}", "'" & tbl.Rows(0).Item(column.ColumnName) & "'")
                                End If
                            Next
                            If TriggerText = "" Then
                                TriggerText = rStr
                            Else
                                TriggerText = TriggerText & ";" & rStr
                            End If
                            '' new added
                            Try

                                Dim conStr1 As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
                                Dim con1 As SqlConnection = New SqlConnection(conStr1)
                                Dim da1 As SqlDataAdapter = New SqlDataAdapter("", con1)
                                da1.SelectCommand.CommandType = CommandType.StoredProcedure
                                da1.SelectCommand.CommandText = "insert_mmm_mst_triggers_res"
                                da1.SelectCommand.Parameters.Clear()
                                da1.SelectCommand.Parameters.AddWithValue("@eid", EID)
                                da1.SelectCommand.Parameters.AddWithValue("@docid", fileID)
                                da1.SelectCommand.Parameters.AddWithValue("@Formname", FormName)
                                da1.SelectCommand.Parameters.AddWithValue("@TriggerText", rStr)
                                Dim dt As New DataTable()
                                da1.Fill(dt)
                            Catch ex As Exception

                            End Try
                            '' new added for log
                        Next
                    End If
                End If
            Catch ex As Exception
                Throw
            End Try
            If TriggerText <> "" Then
                Try
                    'con = New SqlConnection(conStr)
                    da = New SqlDataAdapter("ExecuteTrigger", con)
                    da.SelectCommand.CommandType = CommandType.StoredProcedure
                    da.SelectCommand.Parameters.AddWithValue("@TriggerText", TriggerText)
                    'Setting transaction to dataAddapter
                    da.SelectCommand.Transaction = tran
                    'If con.State <> ConnectionState.Open Then
                    '    con.Open()
                    'End If
                    Result = (Convert.ToInt32(da.SelectCommand.ExecuteScalar()))
                    If Result <= 0 Then
                        Throw New Exception("Error in trigger execution.")
                    End If
                Catch ex As Exception
                    Throw
                End Try
            End If
        Catch ex As Exception
            Throw
        Finally
            'Removing this code from here becouse now connection object is passesd in argument
            'con.Close()
            'con.Dispose()

            da.Dispose()
        End Try
    End Sub
    'DocType	TriggerText	onCreate	onEdit
    Function GetTriggers(ByVal TId As Integer, ByVal EID As Integer, ByVal FORMID As Integer, Optional DocumentType As String = "", Optional LastUpdated As String = "1900-01-01 16:09:49.613") As DataSet
        Dim DsTrg As New DataSet
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim da As SqlDataAdapter = New SqlDataAdapter("getTriggrers", con)
        Try
            da.SelectCommand.CommandType = CommandType.StoredProcedure
            da.SelectCommand.Parameters.AddWithValue("@TID", TId)
            da.SelectCommand.Parameters.AddWithValue("@EID", EID)
            da.SelectCommand.Parameters.AddWithValue("@FORMID", FORMID)
            da.SelectCommand.Parameters.AddWithValue("@DocumentType", DocumentType)
            da.SelectCommand.Parameters.AddWithValue("@lastupdate", LastUpdated)
            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            da.Fill(DsTrg, "tbltrg")
            Return DsTrg
        Catch ex As Exception
            Throw
        Finally
            con.Close()
            con.Dispose()
            da.Dispose()
        End Try

    End Function
#End Region
End Class
