Imports Microsoft.VisualBasic
Imports System.Data.SqlClient
Imports System.Data


Public Class AutoTicketScheduler
    Dim objDC As New DataClass
    Public Sub AutoTicketClosure()
        Try

            Dim dt As New DataTable
            dt = objDC.ExecuteQryDT("select * from mmm_hdmail_schdule with (nolock) where isactive=1 and mdmailid is not null  and mdpwd is not null and mdport is not null and mdisssl is not null and hostname is not null")
            If dt.Rows.Count > 0 Then
                For Each dr As DataRow In dt.Rows
                    Dim dtData As New DataTable
                    dtData = objDC.ExecuteQryDT("select * from mmm_mst_fields where documenttype='" & dr("DocumentType") & "' and eid=" & dr("EID") & " and mdfieldname is not null")
                    Dim readEmail As New ReadEmail()
                    For Each drData As DataRow In dtData.Rows
                        Select Case Convert.ToString(drData("mdfieldname")).ToUpper
                            Case "SUBJECT"
                                readEmail.SubjectFieldMapping = Convert.ToString(drData("fieldmapping"))
                            Case "EMAILID"
                                readEmail.FromFieldMapping = Convert.ToString(drData("fieldmapping"))
                            Case "ASSIGNEE"
                                readEmail.AgentFieldMapping = Convert.ToString(drData("fieldmapping"))
                            Case "STATUS"
                                readEmail.StatusFieldMapping = Convert.ToString(drData("fieldmapping"))
                        End Select
                    Next
                    readEmail.documenttype = dr("DocumentType")
                    readEmail.EID = dr("EID")
                    readEmail.AutoClosureTime = dr("AutoClosureTime")
                    readEmail.StatusData = "SOLVED"
                    GetSavedStatusList(readEmail)
                Next
            End If
        Catch ex As Exception
            Throw
        End Try
    End Sub
    Protected Function GetSavedStatusList(Optional ByVal readmail As ReadEmail = Nothing) As String
        Try
            Dim dtDOC As New DataTable
            dtDOC = objDC.ExecuteQryDT("select * from mmm_mst_doc where documenttype='" & readmail.documenttype & "' and eid=" & readmail.EID & " and " & readmail.StatusFieldMapping & "='" & readmail.StatusData & "' and dateadd(Hour," & readmail.AutoClosureTime & ",lastupdate)<getdate()")
            For Each dr As DataRow In dtDOC.Rows
                Try
                    Dim UID As Integer = 0
                    UID = objDC.ExecuteQryScaller(" select userid from mmm_doc_dtl where docid=" & dr("tid") & " and aprstatus is null")
                    objDC.ExecuteQryScaller("Update mmm_mst_doc set " & readmail.StatusFieldMapping & "='CLOSED', ticketstatus='CLOSED' where eid=" & readmail.EID & " and tid=" & dr("tid"))
                    Dim ht As New Hashtable
                    ht.Add("@tid", dr("tid"))
                    ht.Add("@What", "ARCHIVE")
                    objDC.ExecuteProDT("InsertDefaultMovement", ht)
                    Dim ob As New DynamicForm
                    ob.History(readmail.EID, dr("tid"), UID, readmail.documenttype, "MMM_MST_DOC", "ADD")
                    Trigger.ExecuteTrigger(readmail.documenttype, readmail.EID, dr("tid"))
                    Dim objDMSUtil As New DMSUtil()
                    objDMSUtil.TemplateCalling(dr("tid"), readmail.EID, readmail.documenttype, "APPROVE")
                Catch ex As Exception
                End Try
            Next
        Catch ex As Exception
        End Try
    End Function
    <Serializable()> _
    Public Class ReadEmail
        Public Property From As String
        Public Property FromFieldMapping As String
        Public Property StatusFieldMapping As String
        Public Property StatusData As String
        Public Property Subject As String
        Public Property SubjectFieldMapping As String
        Public Property AgentID As Integer
        Public Property AgentFieldMapping As String
        Public Property documenttype As String
        Public Property EID As Integer
        Public Property AutoClosureTime As Integer
    End Class
End Class
