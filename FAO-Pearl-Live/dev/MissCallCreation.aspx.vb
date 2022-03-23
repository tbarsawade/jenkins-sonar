Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Data.SqlClient
Imports System.Configuration
Imports System.Diagnostics

Partial Class MissCallCreation
    Inherits System.Web.UI.Page
    Dim Eid As Integer = 66
    Dim DefaultUser As Integer = 7251

    Public Function GetMissedCalls(PhNo As String, Hr As String) As DataTable
        Dim dtMissedCalls As New DataTable()
        Try
            Dim con = New SqlConnection(ConfigurationManager.ConnectionStrings("conStr").ConnectionString)
            Dim qry = "Select Distinct MissCallNo from mmm_mst_missedCall where MissCallNo='" & PhNo & "' and convert(date,MissCallDate) >=convert(date,GetDate()) and datePart(Hour, MissCallDate) =" & Hr
            Dim da As New SqlDataAdapter(qry, con)
            da.Fill(dtMissedCalls)
        Catch ex As Exception

        End Try
        Return dtMissedCalls
    End Function
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
    Public Function GEtRoster() As DataTable
        Dim dt As New DataTable()
        Try
            Dim con = New SqlConnection(ConfigurationManager.ConnectionStrings("conStr").ConnectionString)
            'Dim qry = "select  * from mmm_mst_DOc where Eid=" & Eid & " and documenttype ='Night Guard Roster' and convert(date,fld18,3)=convert(date,getDate(),3)"
            Dim qry = "select d.fld12, d.fld13, d.fld14, d.fld15, d.fld16, d.fld17, d.fld18, x.fld1,x.fld10,x.fld11 "
            qry &= " from mmm_mst_DOc d left join (Select DOcId,fld1, fld10, fld11 from mmm_mst_DOc_Item where documenttype='Guard Roster') x "
            qry &= " on d.tid=x.DocId where Eid=" & Eid & " and documenttype='Night Guard Roster' and convert(date,d.fld18,3)=convert(date,getDate()-1,3)"
            Dim da As New SqlDataAdapter("", con)
            da.SelectCommand.CommandText = qry
            da.Fill(dt)
        Catch ex As Exception
        Finally
        End Try
        Return dt
    End Function
    Public Function GetDefaultCount(dtRoster As DataRow) As Integer
        Dim DefaultCount As Integer = 0
        Dim con = New SqlConnection(ConfigurationManager.ConnectionStrings("conStr").ConnectionString)
        Dim da As New SqlDataAdapter("", con)

        Try
            Dim qry = ""
            qry = "select * from mmm_mst_DOc where Eid=" & Eid & " and documenttype ='Missed Call' and fld13='" & dtRoster.Item("fld1") & "' and convert(date, fld1, 3)=convert(date,getdate())"
            da.SelectCommand.CommandText = qry
            Dim dtDocs As New DataTable()
            da.Fill(dtDocs)
            If dtDocs.Rows.Count > 0 Then
                Dim dr = dtDocs.Compute("MAX(fld16)", "")
                DefaultCount = Convert.ToInt32(dtDocs.Compute("MAX(fld16)", ""))
            End If
        Catch ex As Exception
        Finally
        End Try
        Return DefaultCount
    End Function
    Public Function GetStatus(BranchID As String) As String
        Dim con = New SqlConnection(ConfigurationManager.ConnectionStrings("conStr").ConnectionString)
        Dim da As New SqlDataAdapter("", con)
        Dim ds As New DataSet()
        Try
            Dim qry = "Select top 1 aprStatus, RoleName, ordering from mmm_mst_AuthMetrix where Eid=66 and Doctype='Missed call' order by ordering"
            da.SelectCommand.CommandText = qry
            da.Fill(ds, "AuthMatrix")
            If ds.Tables("AuthMatrix").Rows.Count = 0 Then
                Return ""
            End If
            qry = "Select Uid from mmm_ref_Role_user where Eid=" & Eid & " and RoleName='" & ds.Tables("AuthMatrix").Rows(0).Item("RoleName") & "' and  fld1 like '%" & BranchID & "%' and IsCreate=1"
            da.SelectCommand.CommandText = qry
            da.Fill(ds, "User")
            If ds.Tables("User").Rows.Count = 0 Then
                Return ""
            End If

            Return ds.Tables("User").Rows(0).Item("Uid") & "," & ds.Tables("AuthMatrix").Rows(0).Item("aprStatus") & "," & ds.Tables("AuthMatrix").Rows(0).Item("ordering")

        Catch ex As Exception
        End Try
        Return ""
    End Function

    Public Sub CreateMissedCallDocManually()
        Dim con = New SqlConnection(ConfigurationManager.ConnectionStrings("conStr").ConnectionString)
        Dim da As New SqlDataAdapter("", con)
        Try
            Dim dtRoster = GEtRoster()
            For i As Integer = 0 To dtRoster.Rows.Count - 1
                Try
                    For j As Integer = 0 To 5
                        Dim tran As SqlTransaction = Nothing
                        Try
                            If Not con.State = ConnectionState.Open Then
                                con.Open()
                            End If
                            tran = con.BeginTransaction()

                            da.SelectCommand.Transaction = tran
                            Dim qry = "Select fld10 from mmm_mst_Master where Documenttype='Delivery Branch' and Eid=" & Eid & " and Tid=" & dtRoster.Rows(i).Item("fld12")
                            da.SelectCommand.CommandText = qry

                            Dim BranchCode = Convert.ToString(da.SelectCommand.ExecuteScalar())

                            Dim timeInterval = j & ":" & (j + 1)

                            Dim FineAmount = ""
                            qry = "Select kc_logic from mmm_mst_Fields where Eid=" & Eid & " and documenttype ='Missed Call' and IsActive=1 and FieldMapping='fld2'"
                            da.SelectCommand.CommandText = qry
                            da.SelectCommand.Transaction = tran
                            Dim farmula As String = Convert.ToString(da.SelectCommand.ExecuteScalar())

                            'skip if created a document for this hour and guardID
                            qry = "Select count(*) from mmm_mst_DOc where Documenttype='Missed Call' and Eid=" & Eid
                            qry &= " and convert(date,adate)=convert(date,getdate())"
                            qry &= " and fld15='" & timeInterval & "'"
                            qry &= " and fld13='" & dtRoster.Rows(i).Item("fld1") & "'"
                            da.SelectCommand.CommandText = qry

                            Dim curCount = Convert.ToInt32(da.SelectCommand.ExecuteScalar())

                            If curCount > 0 Then
                                tran.Commit()
                                Continue For
                            End If

                            Dim DefaultCount = Convert.ToString(GetDefaultCount(dtRoster.Rows(i)))
                            Dim dtMissedCall = GetMissedCalls(dtRoster.Rows(i).Item("fld11"), j)

                            If dtMissedCall.Rows.Count > 0 Then
                                tran.Commit()
                                Continue For
                            End If

                            DefaultCount = IIf(dtMissedCall.Rows.Count > 0, DefaultCount, (Convert.ToInt32(DefaultCount) + 1).ToString())

                            Dim DocDate = IIf(System.DateTime.Now.Day < 10, "0" & System.DateTime.Now.Day.ToString(), System.DateTime.Now.Day.ToString()) & "/" & IIf(System.DateTime.Now.Month < 10, "0" & System.DateTime.Now.Month.ToString, System.DateTime.Now.Month.ToString()) & "/" & System.DateTime.Now.Year.ToString.Substring(2, 2)

                            Dim StrStatus = GetStatus(dtRoster.Rows(i).Item("fld12").ToString)
                            Dim Ouid = ""
                            Dim curStatus = ""
                            Dim ordering = 0
                            If Not StrStatus = "" Then
                                Ouid = StrStatus.Split(",")(0)
                                curStatus = StrStatus.Split(",")(1)
                                ordering = Convert.ToInt32(StrStatus.Split(",")(2))
                            End If

                            qry = "insert into mmm_mst_Doc(Eid,Documenttype,fld1,fld10,fld11,fld12,fld13,fld14,fld15,fld16,fld17,fld18,fld19,fld2,adate, IsAuth, Ouid, CurStatus, IsWorkFlow)"
                            qry &= " Values(" & Eid & ", 'Missed Call','" & DocDate & "','" & dtRoster.Rows(i).Item("fld12") & "', '" & BranchCode & "', '" & dtRoster.Rows(i).Item("fld13") & "'"
                            qry &= ", '" & dtRoster.Rows(i).Item("fld1") & "', '" & dtRoster.Rows(i).Item("fld10") & "', '" & timeInterval & "', '" & DefaultCount & "','" & dtRoster.Rows(i).Item("fld14") & "','" & dtRoster.Rows(i).Item("fld16") & "', '" & dtRoster.Rows(i).Item("fld15") & "', '" & FineAmount & "', getdate(),1, '" & DefaultUser & "', '" & curStatus & "', 1) ; Select @@identity"

                            da.SelectCommand.CommandText = qry
                            da.SelectCommand.Transaction = tran
                            Dim DocId As Integer = da.SelectCommand.ExecuteScalar()
                            tran.Commit()
                            Dim val = ExecuteExpression(DocId, farmula, "mmm_mst_Doc")

                            qry = "update mmm_mst_Doc set fld2='" & val & "' where Tid=" & DocId

                            da.SelectCommand.CommandText = qry
                            da.SelectCommand.ExecuteNonQuery()

                            Dim query = "insert into MMM_DOC_DTL(UserID, DocId, fdate, tdate, ptat, atat, aprStatus, ordering, DocNature, Lastupdate)"
                            query &= " Values(" & DefaultUser & ", '" & DocId & "', getdate(), getdate(), 0,0,'UPLOADED',0, 'CREATED', getdate())"
                            da.SelectCommand.CommandText = query
                            da.SelectCommand.ExecuteNonQuery()

                            query = "insert into MMM_DOC_DTL(UserID, DocId, fdate, ptat,  ordering, DocNature, Lastupdate)"
                            query &= " Values(" & Ouid & ", '" & DocId & "', getdate(), 1,1, 'CREATED', getdate()) ; Select @@identity "
                            da.SelectCommand.CommandText = query
                            Dim LastId As Integer = da.SelectCommand.ExecuteScalar()

                            qry = "Update mmm_mst_Doc set LastTid='" & LastId & "' where Tid=" & DocId
                            da.SelectCommand.CommandText = query
                            da.SelectCommand.ExecuteNonQuery()
                            con.Close()
                        Catch ex As Exception
                            tran.Rollback()
                            con.Close()
                            InsertError(ex)
                        End Try
                    Next
                Catch ex As Exception

                End Try
            Next
        Catch ex As Exception

        End Try
        lblMsg.Text = "Document created successfully."
    End Sub
    Public Sub InsertError(Ex As Exception)
        Dim con = New SqlConnection(ConfigurationManager.ConnectionStrings("conStr").ConnectionString)
        Dim da As New SqlDataAdapter("", con)
        Dim sta As New StackTrace(Ex, True)
        Dim fr = sta.GetFrame(sta.FrameCount - 1)
        Dim frline As String = fr.GetFileLineNumber.ToString()
        Dim methodname = fr.GetMethod.ToString()
        da.SelectCommand.CommandText = "INSERT_ERRORLOG"
        da.SelectCommand.CommandType = CommandType.StoredProcedure
        da.SelectCommand.Parameters.Clear()
        da.SelectCommand.Parameters.AddWithValue("@ERRORMSG", Ex.Message & ": LineNo-" & frline & "MethodName" & methodname)
        da.SelectCommand.Parameters.AddWithValue("@ALERTNAME", "MissedDocumentCreation")
        da.SelectCommand.Parameters.AddWithValue("@EID", Eid)
        If con.State <> ConnectionState.Open Then
            con.Open()
        End If
        da.SelectCommand.ExecuteNonQuery()
        con.Close()
    End Sub

    Public Function ExecuteExpression(Tid As Integer, Formula As String, TableName As String) As String
        Dim con = New SqlConnection(ConfigurationManager.ConnectionStrings("conStr").ConnectionString)
        Dim da As New SqlDataAdapter("", con)
        Try
            Dim arr() As String = Formula.Split("*", "/", "-", "(", ")", "+", ",", "=")
            Dim Exp = Formula
            If arr.Length > 0 Then
                Dim fldmapping As String = String.Join("','", arr)
                fldmapping = "'" & fldmapping & "'"
                fldmapping = fldmapping.Replace("}", "")
                fldmapping = fldmapping.Replace("{", "")
                fldmapping = fldmapping.Replace("(", "")
                fldmapping = fldmapping.Replace(")", "")
                fldmapping = fldmapping.Replace("[", "")
                fldmapping = fldmapping.Replace("]", "")

                Dim qry = "Select * from " & TableName & " where Tid=" & Tid
                Dim dtDoc As New DataTable()
                da.SelectCommand.CommandText = qry
                da.Fill(dtDoc)
                qry = "Select FieldMapping,DisplayName from mmm_mst_Fields where Eid=" & dtDoc.Rows(0).Item("Eid") & " and DocumentType='" & dtDoc.Rows(0).Item("DocumentType") & "' and DisplayName in (" & fldmapping & ")"
                da.SelectCommand.CommandText = qry
                Dim dtFld As New DataTable()
                da.Fill(dtFld)

                For i As Integer = 0 To dtFld.Rows.Count - 1
                    Exp = Exp.Replace(dtFld.Rows(i).Item("DisplayName"), dtFld.Rows(i).Item("FieldMapping"))
                Next
                For i As Integer = 0 To dtFld.Rows.Count - 1
                    Exp = Exp.Replace(dtFld.Rows(i).Item("FieldMapping"), dtDoc.Rows(0).Item(dtFld.Rows(i).Item("FieldMapping").ToString))
                Next

                Exp = Exp.Replace("}", "")
                Exp = Exp.Replace("{", "")
                Exp = Exp.Replace("(", "")
                Exp = Exp.Replace(")", "")
                Exp = Exp.Replace("[", "")
                Exp = Exp.Replace("]", "")

                Dim val = Convert.ToString(dtDoc.Compute(Exp, ""))
                Return val
            End If

        Catch ex As Exception

        End Try
        Return ""
    End Function

    Protected Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        CreateMissedCallDocManually()
    End Sub
End Class
