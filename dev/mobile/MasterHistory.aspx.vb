Imports System.Data
Imports System.Data.SqlClient
Partial Class mobile_masterhistory
    Inherits System.Web.UI.Page
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        If Not Session("EID") Is Nothing Then
            If Not IsPostBack Then
                FillMainPage()
            End If
        Else
            Response.Redirect("~/mobile/login.aspx")
        End If
    End Sub

    Private Sub FillMainPage()

        Dim pid As Integer = Convert.ToString(Request.QueryString("DOCID").ToString())

        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim da As SqlDataAdapter = New SqlDataAdapter("SELECT m.createdby,f.FormDESC,F.FormName from MMM_MST_MASTER M  LEFT OUTER JOIN MMM_MST_FORMS F on F.FormName=M.documenttype  and F.EID=M.EID where M.TID =" & pid, con)
        Dim ds As New DataSet()
        da.Fill(ds, "data")
        da.Dispose()
        con.Dispose()

        If ds.Tables("data").Rows.Count <> 1 Then
            Exit Sub
        End If

        Dim docType As String = ds.Tables("data").Rows(0).Item("formname").ToString()
        Dim formCaption As String = ds.Tables("data").Rows(0).Item("formdesc").ToString()
        ViewState("FORMDESC") = formCaption


        BindDocDetail(pid)
        history(pid, docType)

    End Sub


    Public Sub BindDocDetail(ByVal pid As Integer)
        Dim VAL As Integer = 0
        ' Response.Write(ViewState("FN").ToString()) 'doctype
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
        Dim ds As New DataSet()
        Dim fldQry As String = ""
        oda.SelectCommand.CommandType = CommandType.StoredProcedure
        oda.SelectCommand.CommandText = "uspGetDetailMasterByid"
        oda.SelectCommand.Parameters.Clear()
        oda.SelectCommand.Parameters.AddWithValue("@pid", pid)
        oda.SelectCommand.Parameters.AddWithValue("@uid", Session("UID"))
        oda.SelectCommand.Parameters.AddWithValue("@eid", Session("EID"))
        oda.Fill(ds, "data")
        Dim tblData As New StringBuilder()
        Dim chldata As New StringBuilder()
        Dim chldata1 As New StringBuilder()
        Dim cntchld As Integer = 0






        'tblData.Append("<div class=""form"" style=""background-color:#fff;color:black""><h3> " & ViewState("FORMDESC").ToString() & " - [System Document ID: " & pid & "] </h3><table cellspacing=""0px"" cellpadding=""0px"" width=""100%"" border=""1px green"">")

        'If ds.Tables("data").Rows.Count = 1 Then
        '    Dim cnt As Integer = ds.Tables("data").Columns.Count
        '    For i As Integer = 0 To ds.Tables("data").Columns.Count - 1
        '        If i Mod 2 = 0 Then
        '            tblData.Append("<tr>")
        '        End If




        tblData.Append("<div class=""form"" style=""background-color:#fff;color:black""><h3> " & ViewState("FORMDESC").ToString() & " - [System Document ID: " & pid & "] </h3><table cellspacing=""0px"" cellpadding=""0px"" width=""100%"" border=""1px green"">")

        If ds.Tables("data").Rows.Count = 1 Then
            Dim cnt As Integer = ds.Tables("data").Columns.Count
            ' adding string for  new  child item 
            'Dim tblchildItmString As New StringBuilder()
            'tblchildItmString.Append("<div class=""form"" style=""background-color:#fff;color:black""><h3> " & ViewState("FORMDESC").ToString() & " - [System Document ID: " & pid & "] </h3><table cellspacing=""0px"" cellpadding=""0px"" width=""100%"" border=""1px green"">")
            For i As Integer = 0 To ds.Tables("data").Columns.Count - 1
                If i Mod 2 = 0 Then
                    tblData.Append("<tr>")
                End If
                'If ds.Tables("data").Rows(0).Item(i).ToString().Contains("CHILDITEM") Then
                '    If i Mod 2 = 0 Then
                '        tblData.Append("<td colspan=""4"">")
                '    Else


                '        tblData.Append("<td></td></tr><tr><td colspan=""4"">")
                '    End If

                '    Dim itemName As String() = ds.Tables("data").Rows(0).Item(i).ToString().Split("-")
                If ds.Tables("data").Rows(0).Item(i).ToString().Contains("CHILDITEM") Then
                    Dim arr1 As String() = ds.Tables("data").Rows(0).Item(i).ToString.Split("-")
                    If cntchld = 0 Then
                        chldata.Append("<td colspan=""4""><div id=""tab"" style=""Width:100%;height:100%""><ul>")
                    End If
                    cntchld = cntchld + 1




                    'chldata.Append("<li><a href=""#tabPending" & i.ToString() & """>" & arr1(1).ToString() & "<asp:Label ID=""lblpending" & i & " "" runat=""server""></asp:Label></a></li>")
                    'chldata1.Append("<div id=""tabPending" & i.ToString() & """style=""min-height:300px;"">")


                    '        'now find the child Item and render it 
                    '        oda.SelectCommand.CommandType = CommandType.StoredProcedure
                    '        oda.SelectCommand.Parameters.Clear()
                    '        oda.SelectCommand.CommandText = "uspGetDetailITEMDetail"
                    '        oda.SelectCommand.Parameters.AddWithValue("docid", pid)
                    '        oda.SelectCommand.Parameters.AddWithValue("FN", arr1(1))
                    '        oda.SelectCommand.Parameters.AddWithValue("EID", Session("EID").ToString())

                    '        Dim dtItem As New DataTable()
                    '        oda.Fill(dtItem)

                    '        Dim tblChildItem As New StringBuilder()
                    '        tblChildItem.Append("<table cellspacing=""2px"" cellpadding=""2px"" width=""100%"" border=""1px double"">")

                    '        For iRow As Integer = 0 To dtItem.Rows.Count
                    '            tblChildItem.Append("<tr>")
                    '            For iColumn As Integer = 0 To dtItem.Columns.Count - 1
                    '                If iRow = 0 Then
                    '                    tblChildItem.Append("<td><h3>" & dtItem.Columns(iColumn).ColumnName.ToString() & "</h3></td>")
                    '                Else
                    '                    tblChildItem.Append("<td>" & dtItem.Rows(iRow - 1).Item(iColumn).ToString() & "</td>")
                    '                End If
                    '            Next
                    '            tblChildItem.Append("</tr>")
                    '        Next
                    '        tblChildItem.Append("</table></div>")

                    '        tblData.Append(tblChildItem.ToString())
                    '        tblData.Append("</td>")
                    '        tblData.Append("</tr>")
                    '    Else
                    '        tblData.Append("<td align=""left"">" & ds.Tables("data").Columns(i).ColumnName & "</td>")
                    '        If ds.Tables("data").Rows(0).Item(i).ToString().Contains(Session("EID").ToString() & "/") Then
                    '            tblData.Append("<td align=""left""><input type=""button"" value=""View Attachment"" onclick=""Javascript: return window.open('DOCS/" & ds.Tables("data").Rows(0).Item(i).ToString() & "', 'CustomPopUp', 'width=600, height=600, menubar=no, resizable=yes');"" /></td>")
                    '        Else
                    '            tblData.Append("<td align=""left"">" & ds.Tables("data").Rows(0).Item(i).ToString() & "</td>")
                    '        End If
                    '    End If
                    '    If i Mod 2 = 1 Then
                    '        tblData.Append("</tr>")
                    '    End If
                    'Next
                    'chldata.Append("</ul>")
                    ''chldata1.Append("</div>")
                    'chldata.Append(chldata1)
                    'chldata.Append("</div></td></tr>")
                    'tblData.Append(chldata)

                    'lblDetail.Text = tblData.ToString() & "</table></div>"




                    chldata.Append("<li><a href=""#tabPending" & i.ToString() & """>" & arr1(1).ToString() & "<asp:Label ID=""lblpending" & i & " "" runat=""server""></asp:Label></a></li>")
                    chldata1.Append("<div id=""tabPending" & i.ToString() & """style=""min-height:300px;"">")

                    ''now find the child Item and render it 
                    oda.SelectCommand.CommandType = CommandType.StoredProcedure
                    oda.SelectCommand.Parameters.Clear()
                    oda.SelectCommand.CommandText = "uspGetDetailITEMDetail"
                    oda.SelectCommand.Parameters.AddWithValue("docid", pid)
                    oda.SelectCommand.Parameters.AddWithValue("FN", arr1(1))
                    oda.SelectCommand.Parameters.AddWithValue("EID", Session("EID").ToString())
                    Dim dtItem As New DataTable()
                    oda.Fill(dtItem)
                    Dim tblChildItem As New StringBuilder()
                    tblChildItem.Append("<table cellspacing=""2px"" id=""table"" cellpadding=""2px"" width=""100%"" border=""1px double"">")
                    For iRow As Integer = 0 To dtItem.Rows.Count
                        VAL = 1
                        tblChildItem.Append("<tr>")
                        For iColumn As Integer = 0 To dtItem.Columns.Count - 1
                            If iRow = 0 Then
                                tblChildItem.Append("<td><h3>" & dtItem.Columns(iColumn).ColumnName.ToString() & "</h3></td>")
                            Else
                                tblChildItem.Append("<td>" & dtItem.Rows(iRow - 1).Item(iColumn).ToString() & "</td>")
                            End If
                        Next
                        tblChildItem.Append("</tr>")
                    Next
                    tblChildItem.Append("</table></div>")
                    chldata1.Append(tblChildItem)
                Else
                    tblData.Append("<td align=""left"">" & ds.Tables("data").Columns(i).ColumnName & "</td>")
                    If ds.Tables("data").Rows(0).Item(i).ToString().Contains(Session("EID").ToString() & "/") Then
                        tblData.Append("<td align=""left""><input type=""button"" value=""View Attachment"" onclick=""Javascript: return window.open('DOCS/" & ds.Tables("data").Rows(0).Item(i).ToString() & "', 'CustomPopUp', 'width=600, height=600, menubar=no, resizable=yes');"" /></td>")
                    Else
                        tblData.Append("<td align=""left"">" & ds.Tables("data").Rows(0).Item(i).ToString() & "</td>")
                    End If
                End If
                If i Mod 2 = 1 Then
                    tblData.Append("</tr>")
                End If
            Next
            chldata.Append("</ul>")
            'chldata1.Append("</div>")
            chldata.Append(chldata1)
            chldata.Append("</div></td></tr>")
            tblData.Append(chldata)
            lblDetail.Text = tblData.ToString() & "</table></div>"








            '    lblDetail.Text = "<div class='form' style='background-color:#fff;color:black'><h3> EXPENSE CLAIM TEST - [System Document ID: 64232] </h3><table cellspacing='0px' cellpadding='0px' width='100%' border='1px green'><tr><td align='left'>Created By</td><td align='left'>garima</td><td align='left'>Creation Date</td><td align='left'>13/08/13 – 18:08:41</td></tr><tr><td align='left'>VENDOR</td><td align='left'>AJAY TOUR & TRAVELS</td></tr><tr><td align='left'>Value contract No</td><td align='left'>VC138</td><td align='left'>Total Claim Amount</td><td align='left'></td></tr><tr><td align='left'>Amount Available</td><td align='left'>5000</td><td align='left'>Amount</td><td align='left'></td></tr><tr><td colspan='4'><div id='tab' style='Width:100%;height:100%'><ul><li><a href='#tabPending3'>Claim Details<asp:Label ID='lblpending3 ' runat='server'></asp:Label></a></li><li><a href='#tabPending8'>Claim Details OC<asp:Label ID='lblpending8 ' runat='server'></asp:Label></a></li></ul><div id='tabPending3'style='min-height:300px;'><table cellspacing='2px' id='table' cellpadding='2px' width='100%' border='1px double'><tr><td><h3>Vendor Code</h3></td><td><h3>VC No.</h3></td><td><h3>VRF No.</h3></td><td><h3>Rate Type</h3></td><td><h3>Basic Rate</h3></td><td><h3>UOM</h3></td><td><h3>Vehicle Type</h3></td><td><h3>Vehicle Registration No.</h3></td><td><h3>KM Run as per Invoice</h3></td><td><h3>Billable Unit</h3></td><td><h3>Hire Amount</h3></td><td><h3>Toll Tax</h3></td><td><h3>Parking</h3></td><td><h3>Service Tax</h3></td><td><h3>Other Charges</h3></td><td><h3>Bill Amount</h3></td></tr><tr><td></td><td>VC68</td><td>VR595</td><td>Extra Km Charge.</td><td>8.5</td><td>per Km</td><td>Indica OR Equivalent</td><td>DL 1YC 2146</td><td>2000</td><td>2000</td><td>16000</td><td>10000</td><td>1000</td><td>100</td><td>10</td><td>27110</td></tr></table></div><div id='tabPending8'style='min-height:300px;'><table cellspacing='2px' id='table' cellpadding='2px' width='100%' border='1px double'><tr><td><h3>Vendor Code</h3></td><td><h3>VC No.</h3></td><td><h3>VRF No.</h3></td><td><h3>Vehicle Type</h3></td><td><h3>Rate Type</h3></td><td><h3>Basic Rate</h3></td><td><h3>Billable Unit</h3></td><td><h3>Vehicle Reg.No.</h3></td><td><h3>UOM</h3></td><td><h3>Km Run as per Invoice</h3></td><td><h3>Hire Amount</h3></td><td><h3>Toll Tax</h3></td><td><h3>Parking</h3></td><td><h3>Service Tax</h3></td><td><h3>Other Charges</h3></td><td><h3>Bill Amount</h3></td></tr></table></div></div></td></tr></table></div>"
        End If
        con.Close()
        oda.Dispose()
        con.Dispose()
    End Sub








    Public Sub history(ByVal pid As Integer, ByVal doctype As String)
        '  Response.Write(ViewState("FN").ToString()) 'doctype

        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)

        Dim fldQry As String = ""






        oda.SelectCommand.CommandText = "uspGetHistoryDetail"
        oda.SelectCommand.CommandType = CommandType.StoredProcedure
        oda.SelectCommand.Parameters.AddWithValue("@DOcid", pid)
        oda.SelectCommand.Parameters.AddWithValue("@FN", doctype)
        oda.SelectCommand.Parameters.AddWithValue("@EID", Session("EID"))
        Dim ds As New DataSet()
        oda.Fill(ds, "data")
        gvHistory.DataSource = ds
        gvHistory.DataBind()
        con.Close()
        oda.Dispose()



    End Sub
End Class
