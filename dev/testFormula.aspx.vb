Imports System.Net
Imports System.Net.Mail
Imports System.Threading
Imports System.IO
Imports System.Data
Imports System.Data.SqlClient
Imports System.Management
Imports System.Xml
Imports System
Imports System.Net.Security
Imports System.Security.Cryptography.X509Certificates
Imports System.Collections.Specialized
Imports ExpressionEvaluator
Imports System.Collections.Generic
Imports System.Linq
Imports System.Web
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports ciloci.FormulaEngine
Imports Microsoft.VisualBasic
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Web.Hosting
Imports System.Diagnostics


Partial Class testFormula
    Inherits System.Web.UI.Page


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


    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load

    End Sub

    Protected Sub btnTestPRAalertmails_Click(sender As Object, e As EventArgs) Handles btnTestPRAalertmails.Click

        Try
            Call Hcl_PRA_NON_EFT_alert_superVisor() ' daily alerts two times 5 pm and 7 pm 
        Catch ex As Exception
            AutoRunLog("Hcl_PRA_NON_EFT_alert_superVisor", "HCL alerts", "TC Exception msg -" & Regex.Replace(ex.Message.ToString, "[""']", String.Empty), 0)
        End Try


        Try
            Call Hcl_PRA_NON_EFT_alert_SalesTeam() ' daily alerts two times 5 pm and 7 pm 
        Catch ex As Exception
            AutoRunLog("Hcl_PRA_NON_EFT_alert_SalesTeam", "HCL alerts", "TC Exception msg -" & Regex.Replace(ex.Message.ToString, "[""']", String.Empty), 0)
        End Try

        'Call Hcl_PRA_EFT_alert_superVisor()

        'Call Hcl_PRA_EFT_alert_SalesTeam()
        'Call Hcl_PRA_NON_EFT_alert_RAO()
    End Sub


    Protected Sub AutoRunLog(ByVal FunctionName As String, ByVal Activity As String, Optional MsgErrror As String = "", Optional eid As Integer = 0)
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Try
            Dim cmd As New SqlCommand("", con)
            cmd.CommandType = CommandType.Text
            cmd.CommandText = "insert into mmm_mst_AutoRunLog(FunctionName,Activity,MsgErrror,LoginTime,eid) values('" & FunctionName & "','" & Activity & "','" & MsgErrror & "',getdate()," & eid & ")"
            If con.State = ConnectionState.Closed Then
                con.Open()
            End If
            cmd.ExecuteNonQuery()
            con.Dispose()
        Catch ex As Exception
            con.Dispose()
        End Try

    End Sub

    Private Sub Hcl_PRA_NON_EFT_alert_SalesTeam()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim da As New SqlDataAdapter("", con)
        If con.State = ConnectionState.Closed Then
            con.Open()
        End If
        Try
            Dim isTime As Boolean = False
            '' for deployment
            Dim cTime As Integer
            cTime = (Now.Hour * 100) + Now.Minute
            If (cTime >= 1910 And cTime <= 1920) Then
                isTime = True
            Else
                isTime = False
            End If
            If isTime Then
                Dim Qrystr As String
                Qrystr = "Select distinct d.OUID [OUID] from mmm_mst_doc d inner join mmm_doc_dtl dt on d.lasttid=dt.tid where d.eid=46 and d.documenttype='PRA' and d.curstatus='Created' and dt.aprstatus is null"

                da.SelectCommand.CommandText = Qrystr
                Dim DtM As New DataTable
                da.Fill(DtM)
                If DtM.Rows.Count <> 0 Then
                    Dim CurrUser As Integer
                    For i As Integer = 0 To DtM.Rows.Count - 1
                        CurrUser = DtM.Rows(i).Item("OUID")
                        Qrystr = "Select  'PRA Non EFT' [PRA Type], u.username,u.emailid, d.fld1 [BPM ID], d.fld100 [Invoice Date] ,d.fld12 [Customer Name], d.fld45 [Payment Type], d.fld57 [Total Payment Amount], d.fld3 [Invoice Total], dt.fdate [Received Date],datediff(hh,dt.fdate,getdate()) [Pending Hours]   from mmm_mst_doc d inner join mmm_doc_dtl dt  on d.lasttid=dt.tid inner join mmm_mst_user U on dt.userid=u.uid  where d.eid=46 and d.documenttype='PRA' and d.curstatus='Created'  and dt.aprstatus is null and d.OUID=" & CurrUser
                        Dim EmailTo As String = ""
                        Dim MailSub As String = "PRA N-EFT Pending for Approval at Sales Manager"

                        da.SelectCommand.CommandText = Qrystr
                        Dim dtR As New DataTable
                        da.Fill(dtR)
                        Dim MailBody As String = ""


                        Qrystr = "Select emailid from mmm_mst_user where eid=46 and uid=" & CurrUser
                        da.SelectCommand.CommandText = Qrystr
                        Dim dtU As New DataTable
                        da.Fill(dtU)
                        EmailTo = dtU.Rows(0).Item("emailid").ToString


                        MailBody &= "Dear Sir/Madam </br></br>"
                        MailBody &= "This is to informed to you that following PRAs are pending for approval of Sales Manager</br>"

                        MailBody &= "<Table borderwidth=""1"" border=""yes""  cellpedding=""12"" cellspacing=""1"" ><Tr bgcolor=""cyan"">"
                        MailBody &= "<TD>PRA Type</TD>"
                        MailBody &= "<TD>BPM ID</TD>"
                        MailBody &= "<TD>Invoice Date</TD>"
                        MailBody &= "<TD>Customer name</TD>"
                        MailBody &= "<TD>Payment type</TD>"
                        MailBody &= "<TD>Total Payment Amount</TD>"
                        MailBody &= "<TD>Invoice Total</TD>"
                        MailBody &= "<TD>Received Date</TD>"
                        MailBody &= "<TD>Pending Hours</TD>"
                        MailBody &= "</Tr>"

                        For k As Integer = 0 To dtR.Rows.Count - 1
                            EmailTo = dtR.Rows(0).Item("emailid").ToString()
                            MailBody &= "<Tr>"
                            MailBody &= "<TD>" & dtR.Rows(k).Item("pra type").ToString() & "</TD>"
                            MailBody &= "<TD>" & dtR.Rows(k).Item("BPM ID").ToString() & "</TD>"
                            MailBody &= "<TD>" & dtR.Rows(k).Item("Invoice Date").ToString() & "</TD>"
                            MailBody &= "<TD>" & dtR.Rows(k).Item("Customer name").ToString() & "</TD>"
                            MailBody &= "<TD>" & dtR.Rows(k).Item("Payment type").ToString() & "</TD>"
                            MailBody &= "<TD>" & dtR.Rows(k).Item("Total Payment Amount").ToString() & "</TD>"
                            MailBody &= "<TD>" & dtR.Rows(k).Item("Invoice Total").ToString() & "</TD>"
                            MailBody &= "<TD>" & dtR.Rows(k).Item("Received Date").ToString() & "</TD>"
                            MailBody &= "<TD>" & dtR.Rows(k).Item("Pending Hours").ToString() & "</TD>"
                            MailBody &= "</Tr>"
                        Next


                        MailBody &= "</Table>"
                        MailBody &= "</BR></BR>"

                        MailBody &= "Regards</BR>"
                        MailBody &= "HCL AR Team"

                        sendMail1(EmailTo, "", "sunil.pareek@myndsol.com", MailSub, MailBody)

                        dtU.Dispose()
                        dtR.Dispose()
                    Next
                End If
                DtM.Dispose()
            End If
            con.Close()
            con.Dispose()
            da.Dispose()
        Catch ex As Exception

        Finally
            con.Close()
            con.Dispose()
            da.Dispose()
        End Try

    End Sub

    Private Sub Hcl_PRA_EFT_alert_SalesTeam()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim da As New SqlDataAdapter("", con)
        If con.State = ConnectionState.Closed Then
            con.Open()
        End If
        Try
            Dim isTime As Boolean = False
            '' for deployment
            Dim cTime As Integer
            cTime = (Now.Hour * 100) + Now.Minute
            If (cTime >= 501 And cTime <= 510) Or (cTime >= 701 And cTime <= 710) Then
                isTime = True
            Else
                isTime = False
            End If
            If isTime Then
                Dim Qrystr As String
                Qrystr = "Select distinct d.OUID [OUID] from mmm_mst_doc d inner join mmm_doc_dtl dt on d.lasttid=dt.tid where d.eid=46 and d.documenttype='PRA EFT' and d.curstatus='Created' and dt.aprstatus is null"

                da.SelectCommand.CommandText = Qrystr
                Dim DtM As New DataTable
                da.Fill(DtM)
                If DtM.Rows.Count <> 0 Then
                    Dim CurrUser As Integer
                    For i As Integer = 0 To DtM.Rows.Count - 1
                        CurrUser = DtM.Rows(i).Item("OUID")
                        Qrystr = "Select  'PRA EFT' [PRA Type], u.username,u.emailid, d.fld45 [BPM ID], d.fld100 [Invoice Date] ,d.fld15 [Customer Name], d.fld21 [Payment Type], d.fld5 [Total Payment Amount], d.fld34 [Invoice Total], dt.fdate [Received Date],datediff(hh,dt.fdate,getdate()) [Pending Hours]   from mmm_mst_doc d inner join mmm_doc_dtl dt  on d.lasttid=dt.tid inner join mmm_mst_user U on dt.userid=u.uid  where d.eid=46 and d.documenttype='PRA Eft' and d.curstatus='Created'  and dt.aprstatus is null and d.OUID=" & CurrUser
                        Dim EmailTo As String = ""
                        Dim MailSub As String = "PRA EFT Pending for Approval at Sales Manager"

                        da.SelectCommand.CommandText = Qrystr
                        Dim dtR As New DataTable
                        da.Fill(dtR)
                        Dim MailBody As String = ""


                        Qrystr = "Select emailid from mmm_mst_user where eid=46 and uid=" & CurrUser
                        da.SelectCommand.CommandText = Qrystr
                        Dim dtU As New DataTable
                        da.Fill(dtU)
                        EmailTo = dtU.Rows(0).Item("emailid").ToString


                        MailBody &= "Dear Sir/Madam </br></br>"
                        MailBody &= "This is to informed to you that following PRAs are pending for approval of Sales Manager</br>"

                        MailBody &= "<Table borderwidth=""1"" border=""yes""  cellpedding=""12"" cellspacing=""1"" ><Tr bgcolor=""cyan"">"
                        MailBody &= "<TD>PRA Type</TD>"
                        MailBody &= "<TD>BPM ID</TD>"
                        MailBody &= "<TD>Invoice Date</TD>"
                        MailBody &= "<TD>Customer name</TD>"
                        MailBody &= "<TD>Payment type</TD>"
                        MailBody &= "<TD>Total Payment Amount</TD>"
                        MailBody &= "<TD>Invoice Total</TD>"
                        MailBody &= "<TD>Received Date</TD>"
                        MailBody &= "<TD>Pending Hours</TD>"
                        MailBody &= "</Tr>"

                        For k As Integer = 0 To dtR.Rows.Count - 1
                            EmailTo = dtR.Rows(0).Item("emailid").ToString()
                            MailBody &= "<Tr>"
                            MailBody &= "<TD>" & dtR.Rows(k).Item("pra type").ToString() & "</TD>"
                            MailBody &= "<TD>" & dtR.Rows(k).Item("BPM ID").ToString() & "</TD>"
                            MailBody &= "<TD>" & dtR.Rows(k).Item("Invoice Date").ToString() & "</TD>"
                            MailBody &= "<TD>" & dtR.Rows(k).Item("Customer name").ToString() & "</TD>"
                            MailBody &= "<TD>" & dtR.Rows(k).Item("Payment type").ToString() & "</TD>"
                            MailBody &= "<TD>" & dtR.Rows(k).Item("Total Payment Amount").ToString() & "</TD>"
                            MailBody &= "<TD>" & dtR.Rows(k).Item("Invoice Total").ToString() & "</TD>"
                            MailBody &= "<TD>" & dtR.Rows(k).Item("Received Date").ToString() & "</TD>"
                            MailBody &= "<TD>" & dtR.Rows(k).Item("Pending Hours").ToString() & "</TD>"
                            MailBody &= "</Tr>"
                        Next


                        MailBody &= "</Table>"
                        MailBody &= "</BR></BR>"

                        MailBody &= "Regards</BR>"
                        MailBody &= "HCL AR Team"

                        sendMail1(EmailTo, "", "sunil.pareek@myndsol.com", MailSub, MailBody)

                        dtU.Dispose()
                        dtR.Dispose()
                    Next
                End If
                DtM.Dispose()
            End If
            con.Close()
            con.Dispose()
            da.Dispose()
        Catch ex As Exception

        Finally
            con.Close()
            con.Dispose()
            da.Dispose()
        End Try

    End Sub

    Private Sub Hcl_PRA_NON_EFT_alert_superVisor()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim da As New SqlDataAdapter("", con)
        If con.State = ConnectionState.Closed Then
            con.Open()
        End If
        Try
            Dim isTime As Boolean = False
            '' for deployment

            Dim cTime As Integer
            cTime = (Now.Hour * 100) + Now.Minute
            If (cTime >= 1940 And cTime <= 1950) Then
                isTime = True
            Else
                isTime = False
            End If
            If isTime Then
                Dim Qrystr As String
                Qrystr = "Select distinct dt.userid from mmm_mst_doc d inner join mmm_doc_dtl dt on d.lasttid=dt.tid where d.eid=46 and d.documenttype='PRA' and d.curstatus='Created' and dt.aprstatus is null"

                da.SelectCommand.CommandText = Qrystr
                Dim DtM As New DataTable
                da.Fill(DtM)
                If DtM.Rows.Count <> 0 Then
                    Dim CurrUser As Integer
                    For i As Integer = 0 To DtM.Rows.Count - 1
                        CurrUser = DtM.Rows(i).Item("userID")
                        Qrystr = "Select  'PRA N-EFT' [PRA Type], u.username,u.emailid, d.fld1 [BPM ID], d.fld100 [Invoice Date] ,d.fld12 [Customer Name], d.fld45 [Payment Type], d.fld57 [Total Payment Amount], d.fld3 [Invoice Total], dt.fdate [Received Date],datediff(hh,dt.fdate,getdate()) [Pending Hours]   from mmm_mst_doc d inner join mmm_doc_dtl dt  on d.lasttid=dt.tid inner join mmm_mst_user U on dt.userid=u.uid  where d.eid=46 and d.documenttype='PRA' and d.curstatus='Created'  and dt.aprstatus is null and dt.userid=" & CurrUser
                        Dim EmailTo As String = ""
                        Dim MailSub As String = "PRA Non Eft Pending for Approval"

                        da.SelectCommand.CommandText = Qrystr
                        Dim dtR As New DataTable
                        da.Fill(dtR)
                        Dim MailBody As String = ""

                        MailBody &= "<p style=""color: Maroon""> Dear Sir/Madam </p>"
                        MailBody &= "<p>Following PRAs are pending for your approval, please take suitable action at your end</p> <br>"

                        MailBody &= "<Table border=1 cellpedding=""12"" cellspacing=""1"" ><Tr bgcolor=""cyan"">"
                        MailBody &= "<TD>PRA Type</TD>"
                        MailBody &= "<TD>BPM ID</TD>"
                        MailBody &= "<TD>Invoice Date</TD>"
                        MailBody &= "<TD>Customer name</TD>"
                        MailBody &= "<TD>Payment type</TD>"
                        MailBody &= "<TD>Total Payment Amount</TD>"
                        MailBody &= "<TD>Invoice Total</TD>"
                        MailBody &= "<TD>Received Date</TD>"
                        MailBody &= "<TD>Pending Hours</TD>"
                        MailBody &= "</Tr>"

                        For k As Integer = 0 To dtR.Rows.Count - 1
                            EmailTo = dtR.Rows(0).Item("emailid").ToString()
                            MailBody &= "<Tr>"
                            MailBody &= "<TD>" & dtR.Rows(k).Item("pra type").ToString() & "</TD>"
                            MailBody &= "<TD>" & dtR.Rows(k).Item("BPM ID").ToString() & "</TD>"
                            MailBody &= "<TD>" & dtR.Rows(k).Item("Invoice Date").ToString() & "</TD>"
                            MailBody &= "<TD>" & dtR.Rows(k).Item("Customer name").ToString() & "</TD>"
                            MailBody &= "<TD>" & dtR.Rows(k).Item("Payment type").ToString() & "</TD>"
                            MailBody &= "<TD>" & dtR.Rows(k).Item("Total Payment Amount").ToString() & "</TD>"
                            MailBody &= "<TD>" & dtR.Rows(k).Item("Invoice Total").ToString() & "</TD>"
                            MailBody &= "<TD>" & dtR.Rows(k).Item("Received Date").ToString() & "</TD>"
                            MailBody &= "<TD>" & dtR.Rows(k).Item("Pending Hours").ToString() & "</TD>"
                            MailBody &= "</Tr>"
                        Next


                        MailBody &= "</Table>"

                        MailBody &= "<p> Click  <a href=""https://hcl.myndsaas.com/""> Here </a>to Login to BPM using your credentials</p>"


                        MailBody &= "<p style=""color: Maroon"">Regards</p>"
                        MailBody &= "<p style=""color: Maroon"">HCL AR Team</p>"

                        sendMail1(EmailTo, "", "sunil.pareek@myndsol.com", MailSub, MailBody)
                    Next
                End If
            End If
            con.Close()
            con.Dispose()
            da.Dispose()
        Catch ex As Exception

        Finally
            con.Close()
            con.Dispose()
            da.Dispose()
        End Try

    End Sub

    Private Sub Hcl_PRA_EFT_alert_superVisor()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim da As New SqlDataAdapter("", con)
        If con.State = ConnectionState.Closed Then
            con.Open()
        End If
        Try
            Dim isTime As Boolean = False
            '' for deployment
            Dim cTime As Integer
            cTime = (Now.Hour * 100) + Now.Minute
            If (cTime >= 501 And cTime <= 510) Or (cTime >= 701 And cTime <= 710) Then
                isTime = True
            Else
                isTime = False
            End If
            If isTime Then
                Dim Qrystr As String
                Qrystr = "Select distinct dt.userid from mmm_mst_doc d inner join mmm_doc_dtl dt on d.lasttid=dt.tid where d.eid=46 and d.documenttype='PRA EFT' and d.curstatus='Created' and dt.aprstatus is null"

                da.SelectCommand.CommandText = Qrystr
                Dim DtM As New DataTable
                da.Fill(DtM)
                If DtM.Rows.Count <> 0 Then
                    Dim CurrUser As Integer
                    For i As Integer = 0 To DtM.Rows.Count - 1
                        CurrUser = DtM.Rows(i).Item("userID")
                        Qrystr = "Select  'PRA EFT' [PRA Type], u.username,u.emailid, d.fld45 [BPM ID], d.fld100 [Invoice Date] ,d.fld15 [Customer Name], d.fld21 [Payment Type], d.fld5 [Total Payment Amount], d.fld34 [Invoice Total], dt.fdate [Received Date],datediff(hh,dt.fdate,getdate()) [Pending Hours]   from mmm_mst_doc d inner join mmm_doc_dtl dt  on d.lasttid=dt.tid inner join mmm_mst_user U on dt.userid=u.uid  where d.eid=46 and d.documenttype='PRA Eft' and d.curstatus='Created'  and dt.aprstatus is null and dt.userid=" & CurrUser
                        Dim EmailTo As String = ""
                        Dim MailSub As String = "PRA EFT Pending for Approval"

                        da.SelectCommand.CommandText = Qrystr
                        Dim dtR As New DataTable
                        da.Fill(dtR)
                        Dim MailBody As String = ""

                        MailBody &= "Dear Sir/Madam </br></br>"
                        MailBody &= "Following PRAs are pending for your approval, please take suitable action at your end </br>"

                        MailBody &= "<Table borderwidth=""1"" border=""yes""  cellpedding=""12"" cellspacing=""1"" ><Tr bgcolor=""cyan"">"
                        MailBody &= "<TD>PRA Type</TD>"
                        MailBody &= "<TD>BPM ID</TD>"
                        MailBody &= "<TD>Invoice Date</TD>"
                        MailBody &= "<TD>Customer name</TD>"
                        MailBody &= "<TD>Payment type</TD>"
                        MailBody &= "<TD>Total Payment Amount</TD>"
                        MailBody &= "<TD>Invoice Total</TD>"
                        MailBody &= "<TD>Received Date</TD>"
                        MailBody &= "<TD>Pending Hours</TD>"
                        MailBody &= "</Tr>"

                        For k As Integer = 0 To dtR.Rows.Count - 1
                            EmailTo = dtR.Rows(0).Item("emailid").ToString()
                            MailBody &= "<Tr>"
                            MailBody &= "<TD>" & dtR.Rows(k).Item("pra type").ToString() & "</TD>"
                            MailBody &= "<TD>" & dtR.Rows(k).Item("BPM ID").ToString() & "</TD>"
                            MailBody &= "<TD>" & dtR.Rows(k).Item("Invoice Date").ToString() & "</TD>"
                            MailBody &= "<TD>" & dtR.Rows(k).Item("Customer name").ToString() & "</TD>"
                            MailBody &= "<TD>" & dtR.Rows(k).Item("Payment type").ToString() & "</TD>"
                            MailBody &= "<TD>" & dtR.Rows(k).Item("Total Payment Amount").ToString() & "</TD>"
                            MailBody &= "<TD>" & dtR.Rows(k).Item("Invoice Total").ToString() & "</TD>"
                            MailBody &= "<TD>" & dtR.Rows(k).Item("Received Date").ToString() & "</TD>"
                            MailBody &= "<TD>" & dtR.Rows(k).Item("Pending Hours").ToString() & "</TD>"
                            MailBody &= "</Tr>"
                        Next


                        MailBody &= "</Table>"
                        MailBody &= "</BR></BR>"

                        MailBody &= "Regards</BR>"
                        MailBody &= "HCL AR Team"

                        sendMail1(EmailTo, "", "sunil.pareek@myndsol.com", MailSub, MailBody)
                    Next
                End If
            End If
            con.Close()
            con.Dispose()
            da.Dispose()
        Catch ex As Exception

        Finally
            con.Close()
            con.Dispose()
            da.Dispose()
        End Try

    End Sub

    Private Sub Hcl_PRA_NON_EFT_alert_RAO()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim da As New SqlDataAdapter("", con)
        If con.State = ConnectionState.Closed Then
            con.Open()
        End If
        Try
            Dim isTime As Boolean = False
            '' for deployment
            Dim cTime As Integer
            cTime = (Now.Hour * 100) + Now.Minute
            If (cTime >= 501 And cTime <= 510) Or (cTime >= 701 And cTime <= 710) Then
                isTime = True
            Else
                isTime = False
            End If
            If isTime Then
                Dim Qrystr As String
                Qrystr = "Select distinct dt.userid from mmm_mst_doc d inner join mmm_doc_dtl dt on d.lasttid=dt.tid where d.eid=46 and d.documenttype='PRA' and d.curstatus='Approved' and dt.aprstatus is null"

                da.SelectCommand.CommandText = Qrystr
                Dim DtM As New DataTable
                da.Fill(DtM)
                If DtM.Rows.Count <> 0 Then
                    Dim CurrUser As Integer
                    For i As Integer = 0 To DtM.Rows.Count - 1
                        CurrUser = DtM.Rows(i).Item("userID")
                        Qrystr = "Select  'PRA N-EFT' [PRA Type], u.username,u.emailid, d.fld1 [BPM ID], d.fld100 [Invoice Date] ,d.fld12 [Customer Name], d.fld45 [Payment Type], d.fld57 [Total Payment Amount], d.fld3 [Invoice Total], dt.fdate [Received Date],datediff(hh,dt.fdate,getdate()) [Pending Hours]   from mmm_mst_doc d inner join mmm_doc_dtl dt  on d.lasttid=dt.tid inner join mmm_mst_user U on dt.userid=u.uid  where d.eid=46 and d.documenttype='PRA' and d.curstatus='Approved'  and dt.aprstatus is null and dt.userid=" & CurrUser
                        Dim EmailTo As String = ""
                        Dim MailSub As String = "PRA Non Eft Pending for Approval"

                        da.SelectCommand.CommandText = Qrystr
                        Dim dtR As New DataTable
                        da.Fill(dtR)
                        Dim MailBody As String = ""

                        MailBody &= "Dear Sir/Madam </br></br>"
                        MailBody &= "Following PRAs are pending for your approval, please take suitable action at your end </br>"

                        MailBody &= "<Table borderwidth=""1"" border=""yes""  cellpedding=""12"" cellspacing=""1"" ><Tr bgcolor=""cyan"">"
                        MailBody &= "<TD>PRA Type</TD>"
                        MailBody &= "<TD>BPM ID</TD>"
                        MailBody &= "<TD>Invoice Date</TD>"
                        MailBody &= "<TD>Customer name</TD>"
                        MailBody &= "<TD>Payment type</TD>"
                        MailBody &= "<TD>Total Payment Amount</TD>"
                        MailBody &= "<TD>Invoice Total</TD>"
                        MailBody &= "<TD>Received Date</TD>"
                        MailBody &= "<TD>Pending Hours</TD>"
                        MailBody &= "</Tr>"

                        For k As Integer = 0 To dtR.Rows.Count - 1
                            EmailTo = dtR.Rows(0).Item("emailid").ToString()
                            MailBody &= "<Tr>"
                            MailBody &= "<TD>" & dtR.Rows(k).Item("pra type").ToString() & "</TD>"
                            MailBody &= "<TD>" & dtR.Rows(k).Item("BPM ID").ToString() & "</TD>"
                            MailBody &= "<TD>" & dtR.Rows(k).Item("Invoice Date").ToString() & "</TD>"
                            MailBody &= "<TD>" & dtR.Rows(k).Item("Customer name").ToString() & "</TD>"
                            MailBody &= "<TD>" & dtR.Rows(k).Item("Payment type").ToString() & "</TD>"
                            MailBody &= "<TD>" & dtR.Rows(k).Item("Total Payment Amount").ToString() & "</TD>"
                            MailBody &= "<TD>" & dtR.Rows(k).Item("Invoice Total").ToString() & "</TD>"
                            MailBody &= "<TD>" & dtR.Rows(k).Item("Received Date").ToString() & "</TD>"
                            MailBody &= "<TD>" & dtR.Rows(k).Item("Pending Hours").ToString() & "</TD>"
                            MailBody &= "</Tr>"
                        Next


                        MailBody &= "</Table>"
                        MailBody &= "</BR></BR>"

                        MailBody &= "Regards</BR>"
                        MailBody &= "HCL AR Team"

                        sendMail1(EmailTo, "", "sunil.pareek@myndsol.com", MailSub, MailBody)
                    Next
                End If
            End If
            con.Close()
            con.Dispose()
            da.Dispose()
        Catch ex As Exception

        Finally
            con.Close()
            con.Dispose()
            da.Dispose()
        End Try

    End Sub


    Private Sub sendMail1(ByVal Mto As String, ByVal cc As String, ByVal bcc As String, ByVal MSubject As String, ByVal MBody As String)
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
            Dim basicAuthenticationInfo As New System.Net.NetworkCredential("no-reply@myndsol.com", "Dn#Ms@538Ti")
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


End Class
