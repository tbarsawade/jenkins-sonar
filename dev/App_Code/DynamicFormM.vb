Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Data.SqlClient

Public Class DynamicFormM

    Public _CallerPage As Integer

    Public Sub New()

    End Sub

    Public Sub CreateControlsOnPanel(ByVal ds As DataTable, ByRef pnlFields As Panel, ByRef UpdatePanel1 As UpdatePanel, ByRef btnActEdit As Button, ByVal autolayout As Integer)
        Try
            If ds.Rows.Count > 0 Then
                Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
                Dim con As SqlConnection = New SqlConnection(conStr)
                'Adding div for creating tab
                pnlFields.Controls.Add(New LiteralControl("<div id=""tabs"">"))
                'Adding Ul Li for creating header of controls
                pnlFields.Controls.Add(New LiteralControl("<ul>"))
                pnlFields.Controls.Add(New LiteralControl("<li>"))
                pnlFields.Controls.Add(New LiteralControl("<a href=""#" & ds.Rows(0).Item("DocumentType").ToString().Replace(" ", "") & """>"))
                pnlFields.Controls.Add(New LiteralControl(ds.Rows(0).Item("DocumentType").ToString()))
                pnlFields.Controls.Add(New LiteralControl("</a>"))
                pnlFields.Controls.Add(New LiteralControl("</li>"))
                'Adding tab For Child item
                Dim ROWCHILD() As DataRow = ds.Select("FIELDTYPE='CHILD ITEM'")
                If ROWCHILD.Length > 0 Then
                    For j As Integer = 0 To ROWCHILD.Length - 1
                        Dim arr1() As String = ROWCHILD(j).Item("dropdown").ToString().Split("-")
                        pnlFields.Controls.Add(New LiteralControl("<li>"))
                        pnlFields.Controls.Add(New LiteralControl("<a href=""#" & arr1(0).ToString().Replace(" ", "") & """>"))
                        pnlFields.Controls.Add(New LiteralControl(arr1(0).ToString().ToString()))
                        pnlFields.Controls.Add(New LiteralControl("</a>"))
                        pnlFields.Controls.Add(New LiteralControl("</li>"))
                        'pnlFields.Controls.Add(New LiteralControl("<li><a href=""#tabPending" & j.ToString() & """>" & arr1(0).ToString() & "<asp:Label ID=""lblpending" & j & " "" runat=""server""></asp:Label></a></li>"))
                    Next
                End If
                pnlFields.Controls.Add(New LiteralControl("</ul>"))
                'header end here
                ''Adding Div for main form
                pnlFields.Controls.Add(New LiteralControl("<div id=""" & ds.Rows(0).Item("DocumentType").ToString().Replace(" ", "") & """ class=""form"">"))
                CreateControlsOnPanel1(ds, pnlFields, UpdatePanel1, btnActEdit, autolayout)
                pnlFields.Controls.Add(New LiteralControl("</div>"))
                If ROWCHILD.Length > 0 Then
                    For j As Integer = 0 To ROWCHILD.Length - 1
                        Dim arr1() As String = ROWCHILD(j).Item("dropdown").ToString().Split("-")
                        'Adding div for tab
                        pnlFields.Controls.Add(New LiteralControl("<div id=""" & arr1(0).ToString().Replace(" ", "") & """ class=""form"">"))
                        'Creating  Top panel for each child item
                        Dim pnlChld As New Panel()
                        pnlChld.ID = "pnlChld" & arr1(0).ToString().Replace(" ", "")
                        'creating chield field panel
                        Dim pnlField As New Panel()
                        pnlField.ID = "pnlCHField" & arr1(0).ToString().Replace(" ", "")
                        Dim lblMsg As New Label()
                        lblMsg.ID = "lblMSG" & arr1(0).ToString().Replace(" ", "")
                        'Font-Bold="true" ForeColor="Red" Font-Size="Small"
                        lblMsg.Font.Bold = True
                        lblMsg.Font.Size = 10
                        lblMsg.ForeColor = System.Drawing.Color.Red
                        pnlField.Controls.Add(lblMsg)
                        'creating update panel
                        Dim Upanel As New UpdatePanel()
                        Upanel.ID = "UpCF" & arr1(0).ToString().Replace(" ", "")
                        Upanel.UpdateMode = UpdatePanelUpdateMode.Conditional
                        Dim pnlbtn As New Panel()
                        'Creating button panel
                        pnlbtn.ID = "pnlbtn" & arr1(0).ToString().Replace(" ", "")
                        'Creating save botton
                        Dim BTNADD As New Button
                        BTNADD.ID = "BTN" & arr1(0).ToString().Replace(" ", "")
                        BTNADD.UseSubmitBehavior = False
                        BTNADD.Text = " ADD " & arr1(0).ToString() & " "
                        BTNADD.CssClass = "btnDyn"
                        'BTNADD.Width = 100
                        'BTNADD.Height = 20
                        'Wrapping button into panel
                        pnlbtn.Controls.Add(New LiteralControl("<div style=""width: 100%; text-align: right;margin-top:20px;"">"))
                        pnlbtn.Controls.Add(BTNADD)
                        pnlbtn.Controls.Add(New LiteralControl("</div>"))
                        'Wrapping All panel into root Update panel
                        Upanel.ContentTemplateContainer.Controls.Add(pnlField)
                        Upanel.ContentTemplateContainer.Controls.Add(pnlbtn)
                        pnlChld.Controls.Add(Upanel)
                        'Adding controls to child fields
                        pnlFields.Controls.Add(pnlChld)
                        con = New SqlConnection(conStr)
                        Dim oda As SqlDataAdapter = New SqlDataAdapter("SELECT FF.*,F.*  FROM MMM_MST_FIELDS FF left outer JOIN MMM_MST_FORMS F on F.FormName = FF.DocumentType and F.EID = FF.EID   where FF.isactive=1 and F.EID=" & HttpContext.Current.Session("EID").ToString() & " and FormName = '" & arr1(0).ToString() & "' order by displayOrder", con)
                        Dim ds1 As New DataSet()

                        con.Open()
                        oda.Fill(ds1, "data")
                        con.Close()
                        con.Dispose()
                        oda.Dispose()
                        CreateControlsOnPanel1(ds1.Tables("data"), pnlField, Upanel, BTNADD, autolayout)
                        'Upanel.ContentTemplateContainer.Controls.Add(pnl1)
                        Dim arr2() As String = ROWCHILD(j).Item("dropdown").ToString().Split("-")
                        Dim upd As New UpdatePanel
                        upd.ID = "UpChldGrd" & arr1(0).ToString().Replace(" ", "")
                        upd.UpdateMode = UpdatePanelUpdateMode.Conditional
                        'If arr2.Length > 1 Then
                        '    Dim BTNREF As New Button
                        '    BTNREF.ID = "BTN" & arr1(1).ToString() & "-" & ROWCHILD(j).Item("FIELDID").ToString()
                        '    BTNREF.Text = " Retrive " & arr1(0).ToString() & " "
                        '    BTNREF.CssClass = "btnDyn"
                        '    pnl1.Controls.Add(BTNREF)
                        'End If
                        Dim GRDCHLDVIEW As New GridView
                        GRDCHLDVIEW.AutoGenerateColumns = "TRUE"
                        GRDCHLDVIEW.ID = "GRD" & arr1(0).ToString().Replace(" ", "")
                        'GRDCHLDVIEW.Width = "800"
                        GRDCHLDVIEW.CssClass = "mGrid"
                        GRDCHLDVIEW.DataKeyNames = New String() {"tid"}
                        GRDCHLDVIEW.AlternatingRowStyle.CssClass = "alt"
                        upd.ContentTemplateContainer.Controls.Add(GRDCHLDVIEW)
                        pnlChld.Controls.Add(upd)
                        pnlFields.Controls.Add(New LiteralControl("</div>"))
                    Next
                End If
                pnlFields.Controls.Add(New LiteralControl("</div><br/><br/><br/><br/><br/><br/><br/>"))

            End If
        'End If
            ds.Dispose()
        Catch ex As Exception
            Dim exMsg = ex.Message
        End Try
    End Sub
    Public Sub CreateControlsOnPanel1(ByVal ds As DataTable, ByRef pnlFields As Panel, ByRef UpdatePanel1 As UpdatePanel, ByRef btnActEdit As Button, ByVal autolayout As Integer)
        Dim datatype As String = ""
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        pnlFields.Controls.Add(New LiteralControl("<table width=""100%"" cellspacing=""0px"" border=""0px"" cellpadding=""0px"">"))
            For i As Integer = 0 To ds.Rows.Count - 1
                Dim dispName As String = ds.Rows(i).Item("displayname").ToString()
                Dim lbl As New Label
                lbl.ID = "lbl" & ds.Rows(i).Item("FieldID").ToString()
                If ds.Rows(i).Item("isrequired").ToString() = "1" Then
                    dispName &= "*"
                End If
                lbl.Text = dispName
                lbl.Font.Bold = True
                pnlFields.Controls.Add(New LiteralControl("<tr>"))
                pnlFields.Controls.Add(New LiteralControl("<td style=""text-align: left; width: 100%"">"))
                'don't add label for child grid
                If ds.Rows(i).Item("FieldType").ToString().ToUpper() = "CHILD ITEM" Or ds.Rows(i).Item("FieldType").ToString().ToUpper() = "FILE UPLOADER" Then
                Else
                    pnlFields.Controls.Add(lbl)
                End If
                pnlFields.Controls.Add(New LiteralControl("</td>"))
                pnlFields.Controls.Add(New LiteralControl("</tr>"))
                pnlFields.Controls.Add(New LiteralControl("<tr>"))
                pnlFields.Controls.Add(New LiteralControl("<td style=""text-align: left; width: 100%"">"))
                Select Case ds.Rows(i).Item("FieldType").ToString().ToUpper()
                    Case "LOOKUP"
                        datatype = ds.Rows(i).Item("datatype").ToString().ToUpper()
                        Dim txtBox As New TextBox
                    txtBox.ID = "fld" & ds.Rows(i).Item("FieldID").ToString()
                    'txtBox.Width = controlWdth - 10
                        txtBox.CssClass = "txtBoxM"
                        'txtBoxM
                        If ds.Rows(i).Item("isEditable").ToString() = "1" Then
                            txtBox.Enabled = True
                        Else
                            txtBox.Enabled = False
                        End If
                        pnlFields.Controls.Add(txtBox)

                    Case "CHILD ITEM TOTAL"
                        datatype = ds.Rows(i).Item("datatype").ToString().ToUpper()
                        Dim txtBox As New TextBox
                        txtBox.ID = "fld" & ds.Rows(i).Item("FieldID").ToString()
                        'txtBox.Width = controlWdth - 10
                        txtBox.CssClass = "txtBoxM"
                        txtBox.Enabled = False
                        pnlFields.Controls.Add(txtBox)

                    Case "CALCULATIVE FIELD"
                        datatype = ds.Rows(i).Item("datatype").ToString().ToUpper()
                        Dim txtBox As New TextBox
                        txtBox.ID = "fld" & ds.Rows(i).Item("FieldID").ToString()
                        'txtBox.Width = controlWdth - 10
                        txtBox.Enabled = False
                        txtBox.CssClass = "txtBoxM"
                        pnlFields.Controls.Add(txtBox)

                    Case "TEXT BOX"
                        datatype = ds.Rows(i).Item("datatype").ToString().ToUpper()
                        Dim txtBox As New TextBox
                        txtBox.ID = "fld" & ds.Rows(i).Item("FieldID").ToString()
                        'txtBox.Width = controlWdth - 10
                        txtBox.CssClass = "txtBoxM"
                        pnlFields.Controls.Add(txtBox)
                        If datatype = "DATETIME" Then
                            Dim CLNDR As New AjaxControlToolkit.CalendarExtender
                            CLNDR.Controls.Clear()
                            CLNDR.ID = "CLNDR" & ds.Rows(i).Item("FieldID").ToString()
                            CLNDR.Format = "dd/MM/yy"
                            CLNDR.TargetControlID = txtBox.ID
                            txtBox.Enabled = True
                            txtBox.Text = String.Format("{0:dd/MM/yy}", Date.Now())
                            If ds.Rows(i).Item("iseditable") = 1 Then
                                Dim img As New Image
                                img.ID = "img" & ds.Rows(i).Item("FieldID").ToString()
                                img.ImageUrl = "~\images\Cal.png"
                                'txtBox.Width = controlWdth - 30
                                pnlFields.Controls.Add(img)
                                CLNDR.PopupButtonID = "img" & ds.Rows(i).Item("FieldID").ToString()
                            End If
                            pnlFields.Controls.Add(CLNDR)
                        Else
                            Dim KC_Value As String = ds.Rows(i).Item("Cal_Fields").ToString()
                            If ds.Rows(i).Item("Cal_Fields").ToString().Length() > 10 Then
                                If _CallerPage <> 1 Then
                                    txtBox.Attributes.Add("onblur", ds.Rows(i).Item("Cal_Fields").ToString())
                                Else
                                    KC_Value = KC_Value.Replace("ContentPlaceHolder1_", "")
                                    txtBox.Attributes.Add("onblur", KC_Value)
                                End If
                            End If
                        End If


                    Case "AUTO NUMBER"
                        datatype = ds.Rows(i).Item("datatype").ToString().ToUpper()
                        Dim txtBox As New TextBox
                        txtBox.ID = "fld" & ds.Rows(i).Item("FieldID").ToString()
                        'txtBox.Width = controlWdth - 10
                        txtBox.CssClass = "txtBoxM"
                        txtBox.Text = ds.Rows(i).Item("dropdown").ToString() & ds.Rows(i).Item("MaxLen").ToString()
                        txtBox.Enabled = False
                        pnlFields.Controls.Add(txtBox)
                    Case "DROP DOWN"
                        Dim ddl As New DropDownList
                        ddl.ID = "fld" & ds.Rows(i).Item("FieldID").ToString()
                        'ddl.Width = controlWdth - 2
                        ddl.CssClass = "txtBoxM"
                        Dim ddlText As String = ds.Rows(i).Item("dropdown").ToString()
                        Dim dropdowntype As String = ds.Rows(i).Item("dropdowntype").ToString()
                        Dim arr() As String
                        If UCase(dropdowntype) = "FIX VALUED" Then
                            arr = ddlText.Split(",")
                            ddl.Items.Add("SELECT")
                            For ii As Integer = 0 To arr.Count - 1
                                ddl.Items.Add(arr(ii).ToUpper())
                            Next
                        ElseIf UCase(dropdowntype) = "MASTER VALUED" Then
                            'If ds.Rows(i).Item("FORMSOURCE").ToString().ToUpper.Trim() = "DETAIL FORM" And ds.Rows(i).Item("KC_LOGIC").ToString().Length > 1 Then
                            'Else
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
                                str = "select " & arr(2).ToString() & "," & TID & "[tid]  from " & TABLENAME & " M WHERE   DOCUMENTTYPE='" & arr(1).ToString() & "'"
                            ElseIf arr(0).ToUpper() <> "STATIC" Then
                                str = "select " & arr(2).ToString() & "," & TID & "[tid]" & lookUpqry & " from " & TABLENAME & " M WHERE EID=" & HttpContext.Current.Session("EID") & " AND  DOCUMENTTYPE='" & arr(1).ToString() & "'"
                            Else
                                If arr(2).ToString.ToUpper = "LOCATIONSTATE" Then
                                    str = "select DISTINCT " & arr(2).ToString() & ",SID [tid]" & lookUpqry & " from " & TABLENAME & " M "
                                Else
                                    str = "select " & arr(2).ToString() & "," & TID & "[tid]" & lookUpqry & " from " & TABLENAME & " M WHERE EID=" & HttpContext.Current.Session("EID") & " "
                                End If
                            End If

                            Dim xwhr As String = ""
                            Dim tids As String = ""
                            'Dim tidarr() As String

                            ''FILTER THE DATA ACCORDING TO USER 
                            tids = UserDataFilter(ds.Rows(i).Item("documenttype").ToString(), arr(1).ToString())

                            If tids.Length >= 2 Then
                                'tidarr = tids.Split("-")
                                xwhr = " AND CONVERT(NVARCHAR(10),TID) IN (" & tids & ")"
                            ElseIf tids = "0" Then
                                pnlFields.Visible = False
                                btnActEdit.Visible = False
                                UpdatePanel1.Update()
                                xwhr = ""
                            End If
                            str = str & "  AND M.isauth=1 " & xwhr & " order by " & arr(2).ToString()
                            Dim AutoFilter As String = ds.Rows(i).Item("AutoFilter").ToString()
                            If AutoFilter.Length > 0 Then
                                If arr(0).ToUpper() = "CHILD" Then
                                    If AutoFilter.ToUpper = "DOCID" Then
                                        str = GetQuery1(arr(1).ToString, arr(2).ToString())
                                    Else
                                        'str = "select " & arr(2).ToString() & ",convert(nvarchar(10),tid)  [tid] from " & TABLENAME & " M WHERE   DOCUMENTTYPE='" & arr(1).ToString() & "'"
                                        str = GetQuery(arr(1).ToString, arr(2).ToString)
                                    End If
                                    'str = ""
                                    'str = "select " & arr(2).ToString() & ",convert(nvarchar(10),tid)  [tid] from " & TABLENAME & " P WHERE   DOCUMENTTYPE='" & arr(1).ToString() & "'"
                                ElseIf arr(0).ToUpper() <> "STATIC" Then
                                    str = "select " & arr(2).ToString() & ",convert(nvarchar(10),tid)  [tid]" & lookUpqry & " from " & TABLENAME & " M WHERE EID=" & HttpContext.Current.Session("EID") & " AND  DOCUMENTTYPE='" & arr(1).ToString() & "'"
                                    str = str & "  AND M.isauth=1 " & xwhr & " order by " & arr(2).ToString()
                                Else
                                    str = "select " & arr(2).ToString() & ",convert(nvarchar(10)," & TID & ")  [tid]" & lookUpqry & " from " & TABLENAME & " M WHERE EID=" & HttpContext.Current.Session("EID") & " "
                                    str = str & "  AND M.isauth=1 " & xwhr & " order by " & arr(2).ToString()
                                End If
                            End If

                            Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
                            Dim dss As New DataSet
                            If str.Length > 0 Then
                                ''order by " & arr(2).ToString()
                                ' oda.SelectCommand.CommandText = str & "  AND M.isauth=1 " & xwhr & "  "
                                oda.SelectCommand.CommandText = str
                                oda.Fill(dss, "FV")
                                Dim isAddJquery As Integer = 0
                                ddl.Items.Add("Select")
                                ddl.Items(0).Value = "0"
                                For J As Integer = 0 To dss.Tables("FV").Rows.Count - 1
                                    ddl.Items.Add(dss.Tables("FV").Rows(J).Item(0).ToString())
                                    Dim lookddlVal As String = dss.Tables("FV").Rows(J).Item(1).ToString()
                                    ddl.Items(J + 1).Value = lookddlVal
                                Next
                                oda.Dispose()
                                dss.Dispose()
                                If isAddJquery = 1 Then
                                    Dim JQuertStr As String = "var r1 = $('#ContentPlaceHolder1_" & ddl.ClientID & "').val(); var l = 0; var mycars = new Array(); for (var i = 0; i < r1.length; i++) { if (r1[i] == '|') { l++; mycars[l] = i; } } for (var i1 = 1; i1 < l; i1++) { var outpu = r1.substring(mycars[i1] + 1, mycars[i1 + 1]); var outpu1 = outpu.substring(0, outpu.indexOf(':')); var outpu2 = outpu.substring(outpu.indexOf(':') + 1); if (outpu2 == 'S') { var out = r1.substring(0, mycars[1]); var x = $('#ContentPlaceHolder1_tmp' + outpu1 + ' option').length; var options = ''; txt = ''; for (i = 0; i < x; i++) { var strUser = $('#ContentPlaceHolder1_tmp' + outpu1 + ' option:eq(' + i + ')').val(); var sel = strUser.substring(strUser.indexOf('-') + 1);  if (out == sel) { var finalshow = $('#ContentPlaceHolder1_tmp' + outpu1 + ' option:eq(' + i + ')').text();  options = options + '<option value=' + finalshow + '>' + finalshow + '</option>\n'; } } $('#ContentPlaceHolder1_' + outpu1 + '').html(options); } else { $('#ContentPlaceHolder1_' + outpu1 + '').val(outpu2); } $('#ContentPlaceHolder1_' + outpu1 + '').val(outpu2); }"
                                    'ddl.Attributes.Add("onchange", JQuertStr)
                                End If
                            End If

                            'End If
                        ElseIf UCase(dropdowntype) = "SESSION VALUED" Then
                            Dim oda1 As SqlDataAdapter = New SqlDataAdapter("", con)
                            Dim ds1 As New DataSet
                            Dim QRY As String = ""
                            Dim DROPDOWN As String() = ds.Rows(i).Item("DROPDOWN").ToString().Split("-")
                            If DROPDOWN(1).ToString.ToUpper = "USER" Then
                                QRY = "SELECT USERNAME ,UID FROM MMM_MST_USER WHERE EID=" & HttpContext.Current.Session("EID") & " AND " & DROPDOWN(2) & "=" & HttpContext.Current.Session(DROPDOWN(2)) & ""
                            ElseIf DROPDOWN(1).ToString.ToUpper = "LOCATION" Then
                                QRY = "SELECT LOCATIONNAME ,LOCID FROM MMM_MST_LOCATION WHERE EID=" & HttpContext.Current.Session("EID") & " AND " & DROPDOWN(2) & "=" & HttpContext.Current.Session(DROPDOWN(2)) & ""
                            End If
                            oda1.SelectCommand.CommandText = QRY
                            oda1.Fill(ds1, "SESSION")
                            ddl.Items.Clear()
                            For iI As Integer = 0 To ds1.Tables("SESSION").Rows.Count - 1
                                ddl.Items.Add(ds1.Tables("SESSION").Rows(iI).Item(0))
                                ddl.Items(iI).Value = ds1.Tables("SESSION").Rows(iI).Item(1)
                            Next
                            ddl.Items.Insert(0, "SELECT")
                        End If
                        pnlFields.Controls.Add(ddl)

                    Case "CHECKBOX LIST"
                        Dim dynmdiv As System.Web.UI.HtmlControls.HtmlGenericControl = New System.Web.UI.HtmlControls.HtmlGenericControl("DIV")
                        dynmdiv.ID = "div" & ds.Rows(i).Item("FieldID").ToString()
                        Dim chklist As New CheckBoxList
                        chklist.ID = "fld" & ds.Rows(i).Item("FieldID").ToString()
                        chklist.CssClass = "txtbox"
                        Dim ddlText As String = ds.Rows(i).Item("dropdown").ToString()
                        Dim dropdowntype As String = ds.Rows(i).Item("dropdowntype").ToString()
                        Dim arr() As String
                        If UCase(dropdowntype) = "FIX VALUED" Then
                            arr = ddlText.Split(",")
                            For ii As Integer = 0 To arr.Count - 1
                                chklist.Items.Add(arr(ii).ToUpper())
                            Next
                        ElseIf UCase(dropdowntype) = "MASTER VALUED" Then
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
                                str = "select " & arr(2).ToString() & "," & TID & "[tid]  from " & TABLENAME & " M WHERE   DOCUMENTTYPE='" & arr(1).ToString() & "'"
                            ElseIf arr(0).ToUpper() <> "STATIC" Then
                                str = "select " & arr(2).ToString() & "," & TID & "[tid]" & lookUpqry & " from " & TABLENAME & " M WHERE EID=" & HttpContext.Current.Session("EID") & " AND  DOCUMENTTYPE='" & arr(1).ToString() & "'"
                            Else
                                If arr(2).ToString.ToUpper = "LOCATIONSTATE" Then
                                    str = "select DISTINCT " & arr(2).ToString() & ",SID [tid]" & lookUpqry & " from " & TABLENAME & " M "
                                Else
                                    str = "select " & arr(2).ToString() & "," & TID & "[tid]" & lookUpqry & " from " & TABLENAME & " M WHERE EID=" & HttpContext.Current.Session("EID") & " "
                                End If
                            End If
                            'Dim str As String = "select " & arr(2).ToString() & ",tid from " & TABLENAME & " WHERE EID=" & HttpContext.Current.Session("EID") & " AND  DOCUMENTTYPE='" & arr(1).ToString() & "'"
                            Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
                            Dim dss As New DataSet
                            oda.SelectCommand.CommandText = str
                            oda.Fill(dss, "FV")
                            For J As Integer = 0 To dss.Tables("FV").Rows.Count - 1
                                chklist.Items.Add(dss.Tables("FV").Rows(J).Item(0))
                                chklist.Items(J).Value = dss.Tables("FV").Rows(J).Item(1)
                            Next
                            oda.Dispose()
                            dss.Dispose()
                        End If

                        pnlFields.Controls.Add(dynmdiv)

                    Case "LIST BOX"
                        Dim ddl As New ListBox
                        ddl.ID = "fld" & ds.Rows(i).Item("FieldID").ToString()
                        'ddl.Width = controlWdth - 2
                        ddl.CssClass = "txtBox"
                        Dim ddlText As String = ds.Rows(i).Item("dropdown").ToString()
                        Dim dropdowntype As String = ds.Rows(i).Item("dropdowntype").ToString()
                        Dim arr() As String
                        If UCase(dropdowntype) = "FIX VALUED" Then
                            arr = ddlText.Split(",")
                            ddl.Items.Add("")
                            For ii As Integer = 0 To arr.Count - 1
                                ddl.Items.Add(arr(ii).ToUpper())
                            Next
                        ElseIf UCase(dropdowntype) = "MASTER VALUED" Then
                            arr = ddlText.Split("-")
                            Dim TABLENAME As String = ""
                            If UCase(arr(0).ToString()) = "MASTER" Then
                                TABLENAME = "MMM_MST_MASTER"
                            Else
                                TABLENAME = "MMM_MST_DOC"
                            End If
                            Dim str As String = "select " & arr(2).ToString() & ",tid from " & TABLENAME & " WHERE EID=" & HttpContext.Current.Session("EID") & " AND  DOCUMENTTYPE='" & arr(1).ToString() & "'"
                            Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
                            Dim dss As New DataSet
                            oda.SelectCommand.CommandText = str
                            oda.Fill(dss, "FV")
                            For J As Integer = 0 To dss.Tables("FV").Rows.Count - 1
                                ddl.Items.Add(dss.Tables("FV").Rows(J).Item(0))
                                ddl.Items(J).Value = dss.Tables("FV").Rows(J).Item(1)
                            Next
                            oda.Dispose()
                            dss.Dispose()
                        End If
                        ddl.SelectionMode = ListSelectionMode.Multiple
                        pnlFields.Controls.Add(ddl)
                    Case "TEXT AREA"
                    Dim txtBox As New TextBox
                        txtBox.ID = "fld" & ds.Rows(i).Item("FieldID").ToString()
                        'txtBox.Width = controlWdth - 10
                        txtBox.CssClass = "txtBoxM"
                        txtBox.TextMode = TextBoxMode.MultiLine
                        pnlFields.Controls.Add(txtBox)
                    Case "SELF REFERENCE"
                        Dim txtBox As New Menu
                        txtBox.ID = "fld" & ds.Rows(i).Item("fieldid").ToString()
                        'txtBox.Width = controlWdth - 10
                        txtBox.Orientation = Orientation.Vertical
                        txtBox.BackColor = Drawing.Color.Aquamarine
                        txtBox.CssClass = "txtBoxM"
                        LoadWorkGroupMenu(ds.Rows(i).Item("DROPDOWN").ToString(), ds.Rows(i).Item("fieldmapping"), txtBox, ds.Rows(i).Item("FIELDTYPE").ToString())
                        pnlFields.Controls.Add(txtBox)
                    Case "PARENT FIELD"
                        Dim txtBox As New Menu
                        txtBox.ID = "fld" & ds.Rows(i).Item("fieldid").ToString()
                        'txtBox.Width = controlWdth - 10
                        txtBox.Orientation = Orientation.Vertical
                        txtBox.BackColor = Drawing.Color.Aquamarine
                        txtBox.CssClass = "txtBoxM"
                        LoadWorkGroupMenu(ds.Rows(i).Item("DROPDOWN").ToString(), ds.Rows(i).Item("fieldmapping"), txtBox, ds.Rows(i).Item("FIELDTYPE").ToString())
                        pnlFields.Controls.Add(txtBox)
                End Select
                pnlFields.Controls.Add(New LiteralControl("</td>"))
                pnlFields.Controls.Add(New LiteralControl("</tr>"))

            Next
        Dim FLU As DataRow() = ds.Select("Fieldtype='File Uploader'")
            If FLU.Length > 0 Then
                Dim i As Integer = 0
                For Each dr As DataRow In FLU
                    pnlFields.Controls.Add(New LiteralControl("<tr>"))
                    Dim dispName As String = dr.Item("displayname").ToString()
                    Dim lbl As New Label
                    lbl.ID = "lbl" & dr.Item("FieldID").ToString()
                    If dr.Item("isrequired").ToString() = "1" Then
                        dispName &= "*"
                    End If
                    lbl.Text = dispName
                    lbl.Font.Bold = True
                    pnlFields.Controls.Add(New LiteralControl("<td style=""width:100%;"">"))
                    pnlFields.Controls.Add(lbl)
                    pnlFields.Controls.Add(New LiteralControl("</td>"))
                    pnlFields.Controls.Add(New LiteralControl("</tr>"))
                    pnlFields.Controls.Add(New LiteralControl("<tr>"))
                    pnlFields.Controls.Add(New LiteralControl("<td style=""width:100%;"">"))
                    Dim txtBox As New FileUpload
                    txtBox.ID = "fld" & dr.Item("FieldID").ToString()
                    'txtBox.Width = controlWdth - 10
                    txtBox.CssClass = "txtBoxM"
                    Dim pstback As New PostBackTrigger
                    pstback.ControlID = btnActEdit.ID
                    UpdatePanel1.Triggers.Add(pstback)
                    pnlFields.Controls.Add(txtBox)
                    pnlFields.Controls.Add(New LiteralControl("</td>"))
                    pnlFields.Controls.Add(New LiteralControl("</tr>"))
                    i += 1
                Next
        End If
        pnlFields.Controls.Add(New LiteralControl("</table>"))

        'If ds.Rows(0).Item("Iscalendar").ToString() = "1" Then
        '    pnlFields.Controls.Add(New LiteralControl("<tr>"))
        '    pnlFields.Controls.Add(New LiteralControl("<td>"))
        '    Dim btn As New Button
        '    btn.ID = "BTNCLNDR"
        '    btn.CssClass = "btnDyn"
        '    btn.Text = "AddTask"
        '    pnlFields.Controls.Add(btn)
        '    pnlFields.Controls.Add(New LiteralControl("</td>"))
        '    pnlFields.Controls.Add(New LiteralControl("</tr>"))
        '    pnlFields.Controls.Add(New LiteralControl("<tr>"))
        '    pnlFields.Controls.Add(New LiteralControl("<td colspan=""4"">"))
        '    Dim GRD As New GridView
        '    GRD.ID = "GRDCLNDR"
        '    GRD.CssClass = "mGrid"
        '    GRD.AutoGenerateColumns = True
        '    GRD.DataKeyNames = New String() {"UID"}
        '    pnlFields.Controls.Add(GRD)
        '    pnlFields.Controls.Add(New LiteralControl("</td>"))
        '    pnlFields.Controls.Add(New LiteralControl("</tr>"))
        'End If



        'End If
        ds.Dispose()
    End Sub


    Public Sub FillControlsOnPanel(ByVal ds As DataTable, ByRef pnlFields As Panel, ByVal type As String, ByVal pid As Integer, Optional ByVal IsAmend As Integer = 0)
        If ds.Rows.Count > 0 Then
            Dim strcol As String = ""
            Dim strqry As String = ""
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As SqlConnection = New SqlConnection(conStr)
            Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
            Dim dss As New DataSet
            For Each rw As DataRow In ds.Rows
                strcol &= rw.Item("fieldmapping").ToString & ","
            Next
            strcol = strcol.Substring(0, strcol.Length - 1)
            If UCase(type) = "MASTER" Then
                strqry = "Select " & strcol & " from MMM_MST_MASTER WHERE TID=" & pid & ""
            ElseIf UCase(type) = "DOCUMENT" Then
                strqry = "Select " & strcol & " from MMM_MST_DOC WHERE TID=" & pid & ""
            ElseIf UCase(type) = "USER" Then
                strqry = "Select " & strcol & " from MMM_MST_USER WHERE UID=" & pid & ""
            ElseIf UCase(type) = "CHILDITEM" Then
                strqry = "Select " & strcol & " from MMM_MST_DOC_ITEM WHERE TID=" & pid & ""
            End If
            oda.SelectCommand.CommandText = strqry
            oda.Fill(dss, "data")
            For i As Integer = 0 To ds.Rows.Count - 1
                Select Case ds.Rows(i).Item("FieldType").ToString().ToUpper()
                    Case "TEXT BOX"
                        Dim txtBox As New TextBox
                        txtBox = CType(pnlFields.FindControl("fld" & ds.Rows(i).Item("FieldID").ToString()), TextBox)
                        txtBox.Text = dss.Tables("data").Rows(0).Item(i).ToString()
                        If ds.Rows(i).Item("isEditable").ToString() = "0" Then
                            txtBox.Enabled = False
                        End If
                        If IsAmend = 1 Then
                            If ds.Rows(i).Item("isEditonAmend").ToString() = "0" Then
                                txtBox.Enabled = False
                            End If
                        End If
                    Case "DROP DOWN"
                        Dim ddl As New DropDownList
                        ddl = CType(pnlFields.FindControl("fld" & ds.Rows(i).Item("FieldID").ToString()), DropDownList)
                        Dim dropdowntype As String = ds.Rows(i).Item("dropdowntype").ToString()
                        If dropdowntype.ToUpper = "FIX VALUED" Then
                            ddl.SelectedIndex = ddl.Items.IndexOf(ddl.Items.FindByText(dss.Tables("data").Rows(0).Item(i).ToString()))
                        ElseIf dropdowntype.ToUpper = "MASTER VALUED" Then
                            ddl.SelectedIndex = ddl.Items.IndexOf(ddl.Items.FindByValue(dss.Tables("data").Rows(0).Item(i).ToString()))
                            If ddl.SelectedIndex <> 0 Then
                                If ds.Rows(i).Item("lookupvalue").ToString.Length > 2 Then
                                    Dim lookupvalue() As String = ds.Rows(i).Item("lookupvalue").ToString.Split(",")
                                    For ii As Integer = 0 To lookupvalue.Length - 1
                                        If ds.Rows(i).Item("lookupvalue").ToString.Contains("-S") Or ds.Rows(i).Item("lookupvalue").ToString.Contains("-C") Or ds.Rows(i).Item("lookupvalue").ToString.Contains("-fld") Then
                                            bind(ds.Rows(i).Item("FIELDID"), pnlFields, ddl)
                                        End If
                                    Next
                                End If
                            End If
                        ElseIf dropdowntype.ToUpper = "SESSION VALUED" Then
                            ddl.SelectedIndex = ddl.Items.IndexOf(ddl.Items.FindByValue(dss.Tables("data").Rows(0).Item(i).ToString()))
                        End If
                        If ds.Rows(i).Item("iseditable").ToString() = "0" Then
                            ddl.Enabled = False
                        End If
                        If IsAmend = 1 Then
                            If ds.Rows(i).Item("isEditonAmend").ToString() = "0" Then
                                ddl.Enabled = False
                            End If
                        End If
                    Case "AUTO NUMBER"
                        Dim txtBox As New TextBox
                        txtBox = CType(pnlFields.FindControl("fld" & ds.Rows(i).Item("FieldID").ToString()), TextBox)
                        txtBox.Text = dss.Tables("data").Rows(0).Item(i).ToString()
                        txtBox.Enabled = False

                    Case "CHECKBOX LIST"
                        Dim chklist As New CheckBoxList
                        chklist = CType(pnlFields.FindControl("fld" & ds.Rows(i).Item("FieldID").ToString()), CheckBoxList)
                        If dss.Tables("data").Rows(0).Item(i).ToString().Length > 1 Then
                            Dim ARR() As String = dss.Tables("data").Rows(0).Item(i).ToString().Split(",")
                            If ds.Rows(i).Item("DROPDOWNType").ToString().ToUpper() = "FIX VALUED" Then
                                For ii As Integer = 0 To ARR.Length - 1
                                    chklist.Items.FindByText(ARR(ii).ToString()).Selected = True
                                Next
                            Else
                                If ARR.Length > 0 Then
                                    For ii As Integer = 0 To ARR.Length - 1
                                        chklist.Items.FindByValue(ARR(ii).ToString()).Selected = True
                                    Next
                                End If
                            End If
                        End If

                    Case "LIST BOX"
                        Dim ddl As New ListBox
                        ddl = CType(pnlFields.FindControl("fld" & ds.Rows(i).Item("FieldID").ToString()), ListBox)
                        Dim ARR() As String = dss.Tables("data").Rows(0).Item(i).ToString().Split(",")
                        If ds.Rows(i).Item("DROPDOWNType").ToString().ToUpper() = "FIX VALUED" Then
                            For ii As Integer = 0 To ARR.Length - 1
                                ddl.Items.FindByText(ARR(ii).ToString()).Selected = True
                            Next
                        Else
                            For ii As Integer = 0 To ARR.Length - 1
                                ddl.Items.FindByValue(ARR(ii).ToString()).Selected = True
                            Next
                        End If
                    Case "TEXT AREA"
                        Dim txtBox As New TextBox
                        txtBox = CType(pnlFields.FindControl("fld" & ds.Rows(i).Item("FieldID").ToString()), TextBox)
                        txtBox.Text = dss.Tables("data").Rows(0).Item(i).ToString()
                    Case "CALCULATIVE FIELD"
                        Dim txtBox As New TextBox
                        txtBox = CType(pnlFields.FindControl("fld" & ds.Rows(i).Item("FieldID").ToString()), TextBox)
                        txtBox.Text = dss.Tables("data").Rows(0).Item(i).ToString()

                    Case "LOOKUP"
                        Dim txtBox As New TextBox
                        txtBox = CType(pnlFields.FindControl("fld" & ds.Rows(i).Item("FieldID").ToString()), TextBox)
                        txtBox.Text = dss.Tables("data").Rows(0).Item(i).ToString()

                    Case "CHILD ITEM TOTAL"
                        Dim txtBox As New TextBox
                        txtBox = CType(pnlFields.FindControl("fld" & ds.Rows(i).Item("FieldID").ToString()), TextBox)
                        txtBox.Text = dss.Tables("data").Rows(0).Item(i).ToString()
                        txtBox.Enabled = False
                End Select
            Next
            oda.Dispose()
            dss.Dispose()
        End If
        ds.Dispose()
    End Sub

    Public Function ValidateAndGenrateQueryForControls(ByVal qryType As String, ByVal qryField As String, ByVal dataField As String, ByVal ds As DataTable, ByRef pnlFields As Panel, ByVal tid As Integer) As String
        Dim errorMsg As String = "Please Enter "
        Dim autono As String = ""
        Dim updquery As String = ""
        If ds.Rows.Count > 0 Then
            For i = 0 To ds.Rows.Count - 1
                Dim dispName As String = ds.Rows(i).Item("displayname").ToString()
                Select Case ds.Rows(i).Item("FieldType").ToString().ToUpper()

                    Case "TEXT BOX"
                        Dim txtBox As TextBox = CType(pnlFields.FindControl("fld" & ds.Rows(i).Item("FieldID").ToString()), TextBox)
                        'Validation for Mandatory
                        If ds.Rows(i).Item("isrequired").ToString() = 1 And txtBox.Text.Length < 1 Then
                            errorMsg &= dispName & ","
                        End If
                        If ds.Rows(i).Item("datatype") = "Datetime" Then
                            If ds.Rows(i).Item("isrequired") = 1 Then
                                Dim str1 As String() = Split(txtBox.Text, "/")
                                If str1.Length = 3 Then
                                    Dim strDate1 As String = str1(1) & "/" & str1(0) & "/" & str1(2)
                                    txtBox.Text = strDate1
                                Else
                                    errorMsg &= "Date is not in correct format at " & dispName & ","
                                    Exit For
                                End If
                                If Not IsDate(txtBox.Text) Then
                                    errorMsg &= "Date is not in correct format at " & dispName & ","
                                    txtBox.Text = str1(0) & "/" & str1(1) & "/" & str1(2)
                                    Exit For
                                Else
                                    txtBox.Text = Format(Convert.ToDateTime(txtBox.Text.ToString), "dd/MM/yy")
                                    Dim str As String() = Split(txtBox.Text, "/")
                                    Dim strDate As String = str(0).PadLeft(2, "0") & "/" & str(1).PadLeft(2, "0") & "/" & str(2).PadLeft(2, "0")
                                    txtBox.Text = strDate
                                End If
                            Else
                                txtBox.Text = Trim(txtBox.Text)
                                If txtBox.Text = "" Then

                                Else
                                    Dim str1 As String() = Split(txtBox.Text, "/")
                                    If str1.Length = 3 Then
                                        Dim strDate1 As String = str1(1) & "/" & str1(0) & "/" & str1(2)
                                        txtBox.Text = strDate1
                                    Else
                                        errorMsg &= "Date is not in correct format at " & dispName & ","
                                        Exit For
                                    End If
                                    If Not IsDate(txtBox.Text) Then
                                        errorMsg &= "Date is not in correct format at " & dispName & ","
                                        txtBox.Text = str1(0) & "/" & str1(1) & "/" & str1(2)
                                        Exit For
                                    Else
                                        txtBox.Text = Format(Convert.ToDateTime(txtBox.Text.ToString), "dd/MM/yy")
                                        Dim str As String() = Split(txtBox.Text, "/")
                                        Dim strDate As String = str(0).PadLeft(2, "0") & "/" & str(1).PadLeft(2, "0") & "/" & str(2).PadLeft(2, "0")
                                        txtBox.Text = strDate
                                    End If
                                End If
                            End If


                        End If
                        If txtBox.Text.Length < CInt(ds.Rows(i).Item("minlen").ToString) And txtBox.Text.Length > 0 And ds.Rows(i).Item("datatype").ToString.ToUpper <> "DATETIME" Then
                            errorMsg &= "Minimum  " & ds.Rows(i).Item("minlen").ToString() & " character in " & dispName & ","
                            Exit For
                        End If
                        If txtBox.Text.Length > CInt(ds.Rows(i).Item("maxlen").ToString) And txtBox.Text.Length > 0 And ds.Rows(i).Item("datatype").ToString.ToUpper <> "DATETIME" Then
                            errorMsg &= "Maximum  " & ds.Rows(i).Item("maxlen").ToString() & " character in " & dispName & ","
                            Exit For
                        End If
                        If txtBox.Text.Trim() <> "" Then
                            If ds.Rows(i).Item("isunique").ToString() = "1" Then
                                If checkduplicate(qryType, tid, ds.Rows(i).Item("DBTABLENAME").ToString, ds.Rows(i).Item("Fieldmapping").ToString(), txtBox.Text, ds.Rows(i).Item("DOCUMENTTYPE").ToString) Then
                                    errorMsg &= "unique " & dispName & " ,"
                                    Exit For
                                End If
                            End If
                        End If
                        If qryType = "ADD" Then
                            'If ds.Rows(i).Item("isunique").ToString() = "1" Then
                            '    If checkduplicate(qryType, tid, ds.Rows(i).Item("DBTABLENAME").ToString, ds.Rows(i).Item("Fieldmapping").ToString(), txtBox.Text, ds.Rows(i).Item("DOCUMENTTYPE").ToString) Then
                            '        errorMsg &= "unique " & dispName & " ,"
                            '        Exit For
                            '    End If
                            'End If
                            If ds.Rows(i).Item("KC_VALUE").ToString.Length > 5 And ds.Rows(i).Item("KC_STATUS").ToString.Length = 0 Then
                                updquery &= UPDATEKICKING(ds.Rows(i).Item("KC_VALUE").ToString(), txtBox.Text, pnlFields)
                            End If
                            qryField &= ds.Rows(i).Item("Fieldmapping").ToString() & ","
                            dataField &= "'" & getSafeString(txtBox.Text) & "',"
                        Else
                            qryField &= ds.Rows(i).Item("Fieldmapping").ToString() & "=" & "'" & getSafeString(txtBox.Text) & "',"
                        End If


                    Case "DROP DOWN"
                        Dim txtBox As DropDownList = CType(pnlFields.FindControl("fld" & ds.Rows(i).Item("FieldID").ToString()), DropDownList)
                        If ds.Rows(i).Item("isrequired").ToString() = 1 And txtBox.SelectedItem.Text.ToUpper = "SELECT" Then
                            errorMsg &= dispName & ","
                        End If

                        If qryType = "ADD" Then

                            If ds.Rows(i).Item("KC_VALUE").ToString.Length > 5 And ds.Rows(i).Item("KC_STATUS").ToString.Length = 0 Then
                                updquery &= UPDATEKICKING(ds.Rows(i).Item("KC_VALUE").ToString(), txtBox.Text, pnlFields)
                            End If
                            qryField &= ds.Rows(i).Item("Fieldmapping").ToString() & ","
                            If UCase(ds.Rows(i).Item("dropdowntype").ToString()) = "FIX VALUED" Then
                                dataField &= "'" & txtBox.SelectedItem.Text & "',"
                            Else
                                Dim fldpair() As String = txtBox.SelectedValue.ToString().Split("|")
                                If ds.Rows(i).Item("lookupvalue").ToString().Length > 2 Then
                                    'Dim fldpair() As String = txtBox.SelectedValue.ToString().Split("|")
                                    dataField &= "'" & fldpair(0).ToString() & "',"
                                Else
                                    'dataField &= "'" & txtBox.SelectedValue.ToString() & "',"
                                    dataField &= "'" & fldpair(0).ToString() & "',"
                                End If
                            End If
                        Else
                            If UCase(ds.Rows(i).Item("dropdowntype").ToString()) = "FIX VALUED" Then
                                qryField &= ds.Rows(i).Item("Fieldmapping").ToString() & "=" & "'" & txtBox.SelectedItem.Text & "',"
                            Else
                                Dim fldpair() As String = txtBox.SelectedValue.ToString().Split("|")
                                If ds.Rows(i).Item("lookupvalue").ToString().Length > 2 Then
                                    'Dim fldpair() As String = txtBox.SelectedValue.ToString().Split("|")
                                    qryField &= ds.Rows(i).Item("Fieldmapping").ToString() & "=" & "'" & fldpair(0).ToString() & "',"
                                Else
                                    qryField &= ds.Rows(i).Item("Fieldmapping").ToString() & "=" & "'" & fldpair(0).ToString() & "',"
                                    'qryField &= ds.Rows(i).Item("Fieldmapping").ToString() & "=" & "'" & txtBox.SelectedItem.Value & "',"
                                End If
                            End If
                        End If
                        'Case "AUTO NUMBER"

                        '    If qryType = "ADD" Then
                        '        qryField &= ds.Rows(i).Item("Fieldmapping").ToString() & ","
                        '        dataField &= "DMS.GetupdatedAN(" & ds.Rows(i).Item("Fieldid") & ",'" & ds.Rows(i).Item("dropdown").ToString & "'," & ds.Rows(i).Item("maxlen") & "),"

                        '        ''autono &= ";Update MMM_MST_FIELDS Set Maxlen=" & ds.Rows(i).Item("maxlen") + 1 & " where Fieldid=" & ds.Rows(i).Item("Fieldid") & ""

                        '    Else
                        '        'qryField &= ds.Rows(i).Item("Fieldmapping").ToString() & "=" & "'" & txtBox.Text & "',"
                        '    End If

                    Case "CHECKBOX LIST"
                        Dim txtBox As CheckBoxList = CType(pnlFields.FindControl("fld" & ds.Rows(i).Item("FieldID").ToString()), CheckBoxList)
                        If ds.Rows(i).Item("isrequired").ToString() = 1 And txtBox.SelectedItem Is Nothing Then
                            errorMsg &= dispName & ","
                            Continue For
                        End If
                        Dim livalue As String = ""
                        If qryType = "ADD" Then

                            If ds.Rows(i).Item("KC_VALUE").ToString.Length > 5 And ds.Rows(i).Item("KC_STATUS").ToString.Length = 0 Then
                                updquery &= UPDATEKICKING(ds.Rows(i).Item("KC_VALUE").ToString(), txtBox.Text, pnlFields)
                            End If

                            qryField &= ds.Rows(i).Item("Fieldmapping").ToString() & ","
                            If UCase(ds.Rows(i).Item("dropdowntype").ToString()) = "FIX VALUED" Then
                                For Each li As ListItem In txtBox.Items
                                    If li.Selected Then
                                        livalue &= li.Text & ","
                                    End If
                                Next
                                livalue = Left(livalue, livalue.Length - 1)
                                dataField &= "'" & livalue & "',"
                            Else
                                For Each li As ListItem In txtBox.Items
                                    If li.Selected Then
                                        livalue &= li.Value & ","
                                    End If
                                Next
                                livalue = Left(livalue, livalue.Length - 1)
                                dataField &= "'" & livalue & "',"
                            End If
                        Else
                            If UCase(ds.Rows(i).Item("dropdowntype").ToString()) = "FIX VALUED" Then
                                For Each li As ListItem In txtBox.Items
                                    If li.Selected Then
                                        livalue &= li.Text & ","
                                    End If
                                Next
                                livalue = Left(livalue, livalue.Length - 1)
                                qryField &= ds.Rows(i).Item("Fieldmapping").ToString() & "=" & "'" & livalue & "',"
                            Else
                                For Each li As ListItem In txtBox.Items
                                    If li.Selected Then
                                        livalue &= li.Value & ","
                                    End If
                                Next
                                livalue = Left(livalue, livalue.Length - 1)
                                qryField &= ds.Rows(i).Item("Fieldmapping").ToString() & "=" & "'" & livalue & "',"
                            End If
                        End If


                    Case "LIST BOX"
                        Dim txtBox As ListBox = CType(pnlFields.FindControl("fld" & ds.Rows(i).Item("FieldID").ToString()), ListBox)
                        If ds.Rows(i).Item("isrequired").ToString() = 1 And txtBox.SelectedItem.Text.Length < 1 Then
                            errorMsg &= dispName & ","
                        End If


                        Dim livalue As String = ""
                        If qryType = "ADD" Then

                            If ds.Rows(i).Item("KC_VALUE").ToString.Length > 5 And ds.Rows(i).Item("KC_STATUS").ToString.Length = 0 Then
                                updquery &= UPDATEKICKING(ds.Rows(i).Item("KC_VALUE").ToString(), txtBox.Text, pnlFields)
                            End If


                            qryField &= ds.Rows(i).Item("Fieldmapping").ToString() & ","
                            If UCase(ds.Rows(i).Item("dropdowntype").ToString()) = "FIX VALUED" Then
                                For Each li As ListItem In txtBox.Items
                                    If li.Selected Then
                                        livalue &= li.Text & ","
                                    End If
                                Next
                                livalue = Left(livalue, livalue.Length - 1)
                                dataField &= "'" & livalue & "',"
                            Else
                                For Each li As ListItem In txtBox.Items
                                    If li.Selected Then
                                        livalue &= li.Value & ","
                                    End If
                                Next
                                livalue = Left(livalue, livalue.Length - 1)
                                dataField &= "'" & livalue & "',"
                            End If
                        Else
                            If UCase(ds.Rows(i).Item("dropdowntype").ToString()) = "FIX VALUED" Then
                                For Each li As ListItem In txtBox.Items
                                    If li.Selected Then
                                        livalue &= li.Text & ","
                                    End If
                                Next
                                livalue = Left(livalue, livalue.Length - 1)
                                qryField &= ds.Rows(i).Item("Fieldmapping").ToString() & "=" & "'" & livalue & "',"
                            Else
                                For Each li As ListItem In txtBox.Items
                                    If li.Selected Then
                                        livalue &= li.Value & ","
                                    End If
                                Next
                                livalue = Left(livalue, livalue.Length - 1)
                                qryField &= ds.Rows(i).Item("Fieldmapping").ToString() & "=" & "'" & livalue & "',"
                            End If
                        End If

                    Case "TEXT AREA"
                        Dim txtBox As TextBox = CType(pnlFields.FindControl("fld" & ds.Rows(i).Item("FieldID").ToString()), TextBox)
                        If ds.Rows(i).Item("isrequired").ToString() = 1 And txtBox.Text.Length < 1 Then
                            errorMsg &= dispName & ","
                        End If
                        If qryType = "ADD" Then
                            qryField &= ds.Rows(i).Item("Fieldmapping").ToString() & ","
                            dataField &= "'" & getSafeString(txtBox.Text) & "',"
                        Else
                            qryField &= ds.Rows(i).Item("Fieldmapping").ToString() & "=" & "'" & getSafeString(txtBox.Text) & "',"
                        End If

                    Case "FILE UPLOADER"
                        Dim txtBox As FileUpload = CType(pnlFields.FindControl("fld" & ds.Rows(i).Item("FieldID").ToString()), FileUpload)
                        If qryType = "ADD" Then
                            If ds.Rows(i).Item("isrequired").ToString() = 1 Then
                                If txtBox.HasFile Then
                                    Dim FN As String = ""
                                    Dim ext As String = ""
                                    FN = Left(txtBox.FileName, txtBox.FileName.LastIndexOf("."))
                                    ext = txtBox.FileName.Substring(txtBox.FileName.LastIndexOf("."), (txtBox.FileName.Length - txtBox.FileName.LastIndexOf(".")))
                                    qryField &= ds.Rows(i).Item("Fieldmapping").ToString() & ","
                                    dataField &= "'" & HttpContext.Current.Session("EID").ToString() & "/" & getSafeString(FN) & "_" & Now.Day & Now.Month & Now.Year & Now.Minute & Now.Second & Now.Millisecond & ds.Rows(i).Item("FieldID").ToString() & "" & ext & "',"
                                    txtBox.SaveAs(HttpContext.Current.Server.MapPath("DOCS/") & HttpContext.Current.Session("EID").ToString() & "/" & getSafeString(FN) & "_" & Now.Day & Now.Month & Now.Year & Now.Minute & Now.Second & Now.Millisecond & ds.Rows(i).Item("FieldID").ToString() & ext)
                                Else
                                    errorMsg &= dispName & ","
                                End If
                            Else
                                If txtBox.HasFile Then
                                    Dim FN As String = ""
                                    Dim ext As String = ""
                                    FN = Left(txtBox.FileName, txtBox.FileName.LastIndexOf("."))
                                    ext = txtBox.FileName.Substring(txtBox.FileName.LastIndexOf("."), (txtBox.FileName.Length - txtBox.FileName.LastIndexOf(".")))
                                    qryField &= ds.Rows(i).Item("Fieldmapping").ToString() & ","
                                    dataField &= "'" & HttpContext.Current.Session("EID").ToString() & "/" & getSafeString(FN) & "_" & Now.Day & Now.Month & Now.Year & Now.Minute & Now.Second & Now.Millisecond & ds.Rows(i).Item("FieldID").ToString() & "" & ext & "',"
                                    Dim path As String = HttpContext.Current.Server.MapPath("~/DOCS/") & HttpContext.Current.Session("EID").ToString() & "/" & txtBox.FileName
                                    txtBox.SaveAs(HttpContext.Current.Server.MapPath("~/DOCS/") & HttpContext.Current.Session("EID").ToString() & "/" & getSafeString(FN) & "_" & Now.Day & Now.Month & Now.Year & Now.Minute & Now.Second & Now.Millisecond & ds.Rows(i).Item("FieldID").ToString() & ext)
                                Else
                                End If
                            End If
                        Else
                            If txtBox.HasFile Then
                                Dim FN As String = ""
                                Dim ext As String = ""
                                FN = Left(txtBox.FileName, txtBox.FileName.LastIndexOf("."))
                                ext = txtBox.FileName.Substring(txtBox.FileName.LastIndexOf("."), (txtBox.FileName.Length - txtBox.FileName.LastIndexOf(".")))
                                qryField &= ds.Rows(i).Item("Fieldmapping").ToString() & "=" & "'" & HttpContext.Current.Session("EID").ToString() & "/" & getSafeString(FN) & "_" & Now.Day & Now.Month & Now.Year & Now.Minute & Now.Second & Now.Millisecond & ds.Rows(i).Item("FieldID").ToString() & "" & ext & "',"
                                txtBox.SaveAs(HttpContext.Current.Server.MapPath("DOCS/") & HttpContext.Current.Session("EID").ToString() & "/" & getSafeString(FN) & "_" & Now.Day & Now.Month & Now.Year & Now.Minute & Now.Second & Now.Millisecond & ds.Rows(i).Item("FieldID").ToString() & ext)
                            Else
                            End If
                        End If

                    Case "LOOKUP"
                        Dim txtBox As TextBox = CType(pnlFields.FindControl("fld" & ds.Rows(i).Item("FieldID").ToString()), TextBox)
                        'Validation for Mandatory
                        If ds.Rows(i).Item("isrequired").ToString() = 1 And txtBox.Text.Length < 1 Then
                            errorMsg &= dispName & ","
                        End If
                        If txtBox.Text.Trim() <> "" Then
                            If ds.Rows(i).Item("isunique").ToString() = "1" Then
                                If checkduplicate(qryType, tid, ds.Rows(i).Item("DBTABLENAME").ToString, ds.Rows(i).Item("Fieldmapping").ToString(), txtBox.Text, ds.Rows(i).Item("DOCUMENTTYPE").ToString) Then
                                    errorMsg &= "unique " & dispName & " ,"
                                    Exit For
                                End If
                            End If
                        End If
                        If qryType = "ADD" Then
                            'If ds.Rows(i).Item("isunique").ToString() = "1" Then
                            '    If checkduplicate(qryType, tid, ds.Rows(i).Item("DBTABLENAME").ToString, ds.Rows(i).Item("Fieldmapping").ToString(), txtBox.Text, ds.Rows(i).Item("DOCUMENTTYPE").ToString) Then
                            '        errorMsg &= "unique " & dispName & " ,"
                            '        Exit For
                            '    End If
                            'End If
                            If ds.Rows(i).Item("KC_VALUE").ToString.Length > 5 And ds.Rows(i).Item("KC_STATUS").ToString.Length = 0 Then
                                updquery &= UPDATEKICKING(ds.Rows(i).Item("KC_VALUE").ToString(), txtBox.Text, pnlFields)
                            End If
                            qryField &= ds.Rows(i).Item("Fieldmapping").ToString() & ","
                            dataField &= "'" & txtBox.Text & "',"
                        Else
                            qryField &= ds.Rows(i).Item("Fieldmapping").ToString() & "=" & "'" & txtBox.Text & "',"
                        End If

                    Case "CALCULATIVE FIELD"
                        Dim txtBox As TextBox = CType(pnlFields.FindControl("fld" & ds.Rows(i).Item("FieldID").ToString()), TextBox)
                        'Validation for Mandatory
                        If ds.Rows(i).Item("isrequired").ToString() = 1 And txtBox.Text.Length < 1 Then
                            errorMsg &= dispName & ","
                        End If


                        If qryType = "ADD" Then

                            If ds.Rows(i).Item("KC_VALUE").ToString.Length > 5 And ds.Rows(i).Item("KC_STATUS").ToString.Length = 0 Then
                                updquery &= UPDATEKICKING(ds.Rows(i).Item("KC_VALUE").ToString(), txtBox.Text, pnlFields)
                            End If


                            qryField &= ds.Rows(i).Item("Fieldmapping").ToString() & ","
                            dataField &= "'" & txtBox.Text & "',"
                        Else
                            qryField &= ds.Rows(i).Item("Fieldmapping").ToString() & "=" & "'" & txtBox.Text & "',"
                        End If

                    Case "CHILD ITEM TOTAL"
                        Dim txtBox As TextBox = CType(pnlFields.FindControl("fld" & ds.Rows(i).Item("FieldID").ToString()), TextBox)

                        If ds.Rows(i).Item("KC_VALUE").ToString.Length > 5 And ds.Rows(i).Item("KC_STATUS").ToString.Length = 0 Then
                            updquery &= UPDATEKICKING(ds.Rows(i).Item("KC_VALUE").ToString(), txtBox.Text, pnlFields)
                        End If

                        If qryType = "ADD" Then
                            qryField &= ds.Rows(i).Item("Fieldmapping").ToString() & ","
                            dataField &= "'" & txtBox.Text & "',"
                        Else
                            qryField &= ds.Rows(i).Item("Fieldmapping").ToString() & "=" & "'" & txtBox.Text & "',"
                        End If

                    Case "SELF REFERENCE"
                        Dim txtBox As Menu = CType(pnlFields.FindControl("fld" & ds.Rows(i).Item("FieldID").ToString()), Menu)
                        'If ds.Rows(i).Item("isrequired").ToString() = 1 And txtBox.Text.Length < 1 Then
                        '    errorMsg &= dispName & ","
                        'End If

                        If qryType = "ADD" Then
                            If txtBox.SelectedItem.Text Is Nothing Or txtBox.SelectedValue = "0" Then
                            Else
                                qryField &= ds.Rows(i).Item("Fieldmapping").ToString() & ","
                                dataField &= "'" & txtBox.SelectedItem.Value & "',"
                            End If
                        Else
                            'qryField &= ds.Rows(i).Item("Fieldmapping").ToString() & "=" & "'" & txtBox.Text & "',"
                        End If

                    Case "PARENT FIELD"
                        Dim txtBox As Menu = CType(pnlFields.FindControl("fld" & ds.Rows(i).Item("FieldID").ToString()), Menu)
                        'If ds.Rows(i).Item("isrequired").ToString() = 1 And txtBox.Text.Length < 1 Then
                        '    errorMsg &= dispName & ","
                        'End If

                        If qryType = "ADD" Then
                            If txtBox.SelectedItem.Text Is Nothing Or txtBox.SelectedValue = "0" Then
                            Else
                                qryField &= ds.Rows(i).Item("Fieldmapping").ToString() & ","
                                dataField &= "'" & txtBox.SelectedItem.Value & "',"
                            End If
                        Else
                            'qryField &= ds.Rows(i).Item("Fieldmapping").ToString() & "=" & "'" & txtBox.Text & "',"
                        End If
                End Select
            Next
        End If
        '' changed/added below by sunil on 01-11-13
        '' for skipping doc validation on amendment nature/edit
        Dim CurDocNat As String = "CREATE"
        If HttpContext.Current.Session("DOCSTATUS") <> Nothing Then
            If HttpContext.Current.Session("DOCSTATUS") = "MODIFY" Then
                CurDocNat = "MODIFY"
            End If
        End If

        If ds.Rows.Count > 0 Then
            Dim str As String = validateForm(ds.Rows(0).Item("Documenttype").ToString, HttpContext.Current.Session("EID"), pnlFields, ds, qryType, tid, CurDocNat)
            If str.Length > 5 Then
                str = "Please " & str
                Return Left(str, Len(str) - 0)
            End If
        End If

        If errorMsg.Length < 14 Then
            If qryType = "ADD" Then
                If updquery.Length > 1 Then
                    Return Left(qryField, Len(qryField) - 1) & ")" & Left(dataField, Len(dataField) - 1) & ")" & autono & ";" & updquery
                Else
                    Return Left(qryField, Len(qryField) - 1) & ")" & Left(dataField, Len(dataField) - 1) & ")" & autono
                End If
            Else
                Return Left(qryField, Len(qryField) - 1)
            End If
        Else
            Return Left(errorMsg, Len(errorMsg) - 1)
        End If
    End Function

    ''Add Child item to datatable 
    Public Sub ADDITEMTOGRID(ByVal dtFields As DataTable, ByVal FORMNAME As String, ByRef pnlFields As Panel)
        Dim dtFD As New DataTable
        Dim dtField As New DataTable
        Dim DTVALUE As New DataTable
        Dim errormsg As String = ""
        'dtField = ViewState(FORMNAME)
        dtField = dtFields
        If HttpContext.Current.Session(FORMNAME) Is Nothing Then
            For Each dr As DataRow In dtField.Rows
                dtFD.Columns.Add(dr.Item("displayname"), GetType(String))
                DTVALUE.Columns.Add(dr.Item("Displayname"), GetType(String))
            Next
            dtFD.Columns.Add("tid", GetType(String))
        Else
            dtFD = HttpContext.Current.Session(FORMNAME)
            DTVALUE = HttpContext.Current.Session(FORMNAME & "VAL")
            If dtFD.Rows.Count > 1 Then
                dtFD.Rows.RemoveAt(dtFD.Rows.Count - 1)
            End If
        End If
        Dim drnew As DataRow = dtFD.NewRow()
        Dim DRNEWVAL As DataRow = DTVALUE.NewRow()
        For Each dr As DataRow In dtField.Rows
            Dim dispName As String = dr.Item("displayname").ToString()
            Select Case dr.Item("FieldType").ToString().ToUpper()
                Case "TEXT BOX"
                    Dim txtBox As TextBox = CType(pnlFields.FindControl("fld" & dr.Item("FieldID").ToString()), TextBox)
                    drnew.Item(dr.Item("displayname")) = txtBox.Text
                    DRNEWVAL.Item(dr.Item("displayname")) = txtBox.Text
                Case "DROP DOWN"
                    Dim txtBox As DropDownList = CType(pnlFields.FindControl("fld" & dr.Item("FieldID").ToString()), DropDownList)
                    drnew.Item(dr.Item("displayname")) = txtBox.SelectedItem.Text
                    DRNEWVAL.Item(dr.Item("displayname")) = txtBox.SelectedItem.Value
                Case "CALCULATIVE FIELD"
                    Dim txtBox As TextBox = CType(pnlFields.FindControl("fld" & dr.Item("FieldID").ToString()), TextBox)
                    drnew.Item(dr.Item("displayname")) = txtBox.Text
                    DRNEWVAL.Item(dr.Item("displayname")) = txtBox.Text
                Case "LOOKUP"
                    Dim txtBox As TextBox = CType(pnlFields.FindControl("fld" & dr.Item("FieldID").ToString()), TextBox)
                    drnew.Item(dr.Item("displayname")) = txtBox.Text
                    DRNEWVAL.Item(dr.Item("displayname")) = txtBox.Text
            End Select
        Next
        drnew.Item("tid") = FORMNAME & "-" & dtFD.Rows.Count
        dtFD.Rows.Add(drnew)
        DTVALUE.Rows.Add(DRNEWVAL)
        HttpContext.Current.Session(FORMNAME) = dtFD
        HttpContext.Current.Session(FORMNAME & "VAL") = DTVALUE
        'BINDGRID1(dtFD)
        'ModalPopupExtender1.Hide()
    End Sub

    'Public Function validateForm(ByVal doctype As String, ByVal eid As Integer, ByRef pnlFields As Panel) As String
    '    Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
    '    Dim con As SqlConnection = New SqlConnection(conStr)
    '    Dim da As New SqlDataAdapter("", con)
    '    Dim datatype As String
    '    Dim datatype1 As String
    '    Dim errmsg As String = ""
    '    Dim txtobj1 As TextBox
    '    Dim txtobj As TextBox
    '    Dim ddltype As DropDownList
    '    Dim ddltype1 As DropDownList
    '    Dim arr1() As String
    '    Dim rtn As Integer
    '    da.SelectCommand.CommandText = "select * from MMM_MST_FORMVALIDATION where eid=" & eid & " and doctype='" & doctype & "'"
    '    Dim ds As New DataSet
    '    da.Fill(ds, "data")
    '    For Each rw As DataRow In ds.Tables("data").Rows

    '        If rw.Item("Valtype").ToString.ToUpper <> "DYNAMIC" Then
    '            da.SelectCommand.CommandText = "select datatype from mmm_mst_fields where eid=" & eid & " and documenttype='" & doctype & "' and fieldid=" & Right(rw.Item("fldID"), rw.Item("fldID").ToString.Length - 3) & ""
    '            If con.State <> ConnectionState.Open Then
    '                con.Open()
    '            End If
    '            datatype = da.SelectCommand.ExecuteScalar()
    '            If rw.Item("Valtype").ToString.ToUpper = "FIELD" Then
    '                da.SelectCommand.CommandText = "select datatype from mmm_mst_fields where eid=" & eid & " and documenttype='" & doctype & "' and fieldid=" & Right(rw.Item("Value"), rw.Item("Value").ToString.Length - 3) & ""
    '                datatype1 = da.SelectCommand.ExecuteScalar
    '            End If

    '            If rw.Item("Valtype").ToString.ToUpper = "STATIC" Then
    '                txtobj = TryCast(pnlFields.FindControl(rw.Item("fldID")), TextBox)
    '                ddltype1 = TryCast(pnlFields.FindControl(rw.Item("fldID")), DropDownList)
    '            ElseIf rw.Item("Valtype").ToString.ToUpper = "FIELD" Then
    '                txtobj = TryCast(pnlFields.FindControl(rw.Item("fldID")), TextBox)
    '                ddltype1 = TryCast(pnlFields.FindControl(rw.Item("fldID")), DropDownList)
    '                txtobj1 = TryCast(pnlFields.FindControl(rw.Item("Value")), TextBox)
    '                ddltype = TryCast(pnlFields.FindControl(rw.Item("Value")), DropDownList)
    '            ElseIf rw.Item("Valtype").ToString.ToUpper = "MANDATORY" Then
    '                txtobj = TryCast(pnlFields.FindControl(rw.Item("fldID")), TextBox)
    '                ddltype1 = TryCast(pnlFields.FindControl(rw.Item("fldID")), DropDownList)
    '                txtobj1 = TryCast(pnlFields.FindControl(rw.Item("Operator")), TextBox)
    '                ddltype = TryCast(pnlFields.FindControl(rw.Item("Operator")), DropDownList)
    '            End If
    '        End If


    '        Dim tbname As String = ""
    '        Dim sb As String = ""
    '        Dim sts As String = ""

    '        If rw.Item("Valtype").ToString.ToUpper = "DYNAMIC" Then
    '            Dim ddlobj As DropDownList = TryCast(pnlFields.FindControl(rw.Item("fldid").ToString), DropDownList)
    '            If ddlobj.SelectedItem.Text.ToUpper = "SELECT" Then
    '                Dim fid As Integer
    '                Dim dispname As String = ""
    '                fid = rw.Item("fldID").ToString.Length - 3
    '                da.SelectCommand.CommandText = " select displayname from mmm_mst_fields where fieldid=" & fid & ""
    '                If con.State <> ConnectionState.Open Then
    '                    con.Open()
    '                End If
    '                dispname = da.SelectCommand.ExecuteScalar()
    '                errmsg = "Select " & dispname
    '                con.Dispose()
    '                Return errmsg
    '                Exit Function
    '            End If
    '            arr1 = rw.Item("value").ToString.Split("-")
    '            Dim arr2 As String()
    '            Dim CNT As Integer = arr1.Length
    '            For i As Integer = 1 To arr1.Length - 1
    '                arr2 = Split(arr1(i), ":")
    '                If i = 1 Then
    '                    tbname = arr2(0).ToString
    '                End If
    '                If arr2.Length > 1 Then
    '                    sb = sb & arr2(1).ToString & ","
    '                End If
    '                If i > 1 Then
    '                    Dim opr As String = Left(arr2(0), 1)
    '                    Dim opr1 As String = Left(arr2(0), 2)
    '                    opr1 = opr1.Replace(opr, "")

    '                    If opr1 = "=" Then
    '                        opr = opr & opr1
    '                    End If

    '                    Dim txt As String = Right(arr2(0), arr2(0).Length - opr.Length).ToString
    '                    Dim txtbx1 As TextBox = TryCast(pnlFields.FindControl(txt), TextBox)
    '                    Dim ddllst As DropDownList = TryCast(pnlFields.FindControl(txt), DropDownList)
    '                    da.SelectCommand.CommandText = "select datatype from mmm_mst_fields where eid=" & eid & " and fieldid=" & Right(txt, txt.ToString.Length - 3) & ""
    '                    If con.State <> ConnectionState.Open Then
    '                        con.Open()
    '                    End If
    '                    datatype = da.SelectCommand.ExecuteScalar()
    '                    If CNT - 1 <> i Then
    '                        If IsNothing(ddllst) Then
    '                            If datatype.ToUpper = "NUMERIC" Then
    '                                sts = sts & opr & "convert(float," & txtbx1.Text.ToString & ")" & "|"
    '                            ElseIf datatype.ToUpper = "DATETIME" Then
    '                                sts = sts & opr & " convert(datetime," & "'" & getdate(txtbx1.Text.ToString) & "')" & "|"
    '                            Else
    '                                sts = sts & opr & "'" & txtbx1.Text.ToString & "'" & "|"
    '                            End If
    '                        Else
    '                            sts = sts & opr & "'" & ddllst.SelectedValue & "'" & "|"
    '                        End If
    '                    Else
    '                        If IsNothing(ddllst) Then
    '                            If datatype.ToUpper = "NUMERIC" Then
    '                                sts = sts & opr & "convert(float," & txtbx1.Text.ToString & ")" & "|"
    '                            ElseIf datatype.ToUpper = "DATETIME" Then
    '                                sts = sts & opr & " convert(datetime," & "'" & getdate(txtbx1.Text.ToString) & "')" & "|"
    '                            Else
    '                                sts = sts & opr & "'" & txtbx1.Text.ToString & "'" & "|"
    '                            End If
    '                        Else
    '                            sts = sts & opr & "'" & ddllst.SelectedValue & "'" & "|"
    '                        End If
    '                    End If
    '                End If
    '            Next
    '            sb = Left(sb, sb.Length - 1)

    '            da.SelectCommand.CommandText = "select " & sb & " from " & tbname & " where tid=" & ddlobj.SelectedValue & ""
    '            Dim DS1 As New DataSet
    '            da.Fill(DS1, "DATA")
    '            Dim V1 As String = ""
    '            Dim sarr() As String
    '            Dim QR As String = " "
    '            sarr = sts.ToString.Split("|")
    '            If DS1.Tables("DATA").Rows.Count > 0 Then
    '                For X As Integer = 0 To DS1.Tables("DATA").Columns.Count - 1
    '                    V1 = DS1.Tables("DATA").Rows(0).Item(X).ToString
    '                    If V1.ToString.Length > 6 And (V1.ToString.Contains("/") Or V1.ToString.Contains("-")) And Left(V1, 1).Contains("-") = False Then
    '                        QR = QR & " Convert(datetime," & "'" & getdate(V1) & "')" & sarr(X) & " and "
    '                    Else
    '                        QR = QR & "'" & V1 & "'" & sarr(X) & " and "
    '                    End If
    '                Next
    '            End If
    '            QR = Trim(QR)
    '            QR = Left(QR, QR.Length - 3)
    '            da.SelectCommand.CommandText = "select count(tid) from " & tbname & " where eid=" & HttpContext.Current.Session("EID") & " and documenttype='" & arr1(0).ToString() & "' and TID=" & ddlobj.SelectedValue & " and " & QR & " "
    '        End If

    '        If UCase(rw.Item("valtype").ToString) = "STATIC" Then
    '            If IsNothing(txtobj) Then
    '                da.SelectCommand.CommandText = " select case when " & "'" & ddltype1.SelectedItem.Text & "'" & rw.Item("operator") & "'" & rw.Item("value") & "'" & "  then 1 else 0 end"
    '            Else
    '                If datatype.ToString.ToUpper = "TEXT" Then
    '                    da.SelectCommand.CommandText = " select case when " & "'" & txtobj.Text.ToString & "'" & rw.Item("operator") & "'" & rw.Item("value") & "'" & "  then 1 else 0 end"
    '                Else
    '                    da.SelectCommand.CommandText = " select case when " & "'" & txtobj.Text.ToString & "'" & rw.Item("operator") & rw.Item("value") & "  then 1 else 0 end"
    '                End If
    '            End If
    '        ElseIf UCase(rw.Item("valtype").ToString) = "FIELD" Then
    '            If IsNothing(ddltype) Then
    '                If datatype.ToUpper = "DATETIME" And datatype.ToUpper = "DATETIME" Then
    '                    da.SelectCommand.CommandText = " select case when " & " Convert(DateTime," & "'" & getdate(txtobj.Text.ToString) & "'" & ")" & rw.Item("operator") & "Convert(DateTime," & "'" & getdate(txtobj1.Text.ToString) & "'" & ")" & "  then 1 else 0 end"
    '                Else
    '                    da.SelectCommand.CommandText = " select case when " & "'" & txtobj.Text.ToString & "'" & rw.Item("operator") & "'" & txtobj1.Text.ToString & "'" & "  then 1 else 0 end"
    '                End If
    '            Else
    '                da.SelectCommand.CommandText = " select case when " & "'" & txtobj.Text.ToString & "'" & rw.Item("operator") & "'" & ddltype.SelectedItem.Text.ToString & "'" & "  then 1 else 0 end"
    '            End If
    '        ElseIf UCase(rw.Item("valtype").ToString) = "MANDATORY" Then
    '            If IsNothing(ddltype) Then
    '                da.SelectCommand.CommandText = " select case when " & "'" & txtobj1.Text.ToString & "'" & "=" & "'" & rw.Item("value").ToString & "'" & "  then 1 else 0 end"
    '            Else
    '                da.SelectCommand.CommandText = " select case when " & "'" & ddltype.SelectedItem.Text & "'" & "=" & "'" & rw.Item("value").ToString & "'" & "  then 1 else 0 end"
    '            End If
    '        End If

    '        If con.State <> ConnectionState.Open Then
    '            con.Open()
    '        End If
    '        rtn = da.SelectCommand.ExecuteScalar()

    '        If rtn = 0 Then
    '            If UCase(rw.Item("valtype").ToString) <> "MANDATORY" Then
    '                errmsg = Trim(errmsg) & " " & rw.Item("err_msg").ToString()
    '            End If
    '        ElseIf rtn = 1 And rw.Item("Valtype").ToString.ToUpper <> "DYNAMIC" Then
    '            If IsNothing(txtobj) Then
    '                If ddltype1.SelectedItem.Text = "" Then
    '                    errmsg = Trim(errmsg) & " " & rw.Item("err_msg").ToString()
    '                End If
    '            Else
    '                If txtobj.Text.ToString = "" Then
    '                    errmsg = Trim(errmsg) & " " & rw.Item("err_msg").ToString()
    '                End If
    '            End If
    '        End If
    '    Next
    '    con.Dispose()
    '    If errmsg.Length > 5 Then
    '        Return errmsg
    '    Else
    '        Return "True"
    '    End If
    'End Function


    Public Function validateForm(ByVal doctype As String, ByVal eid As Integer, ByRef pnlFields As Panel, ByVal dT As DataTable, ByVal Action As String, ByVal tid As Integer, Optional ByVal DocNat As String = "CREATE") As String
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim da As New SqlDataAdapter("", con)
        Dim datatype As String
        Dim datatype1 As String
        Dim errmsg As String = ""
        Dim txtobj1 As TextBox
        Dim txtobj As TextBox
        Dim ddltype As DropDownList
        Dim ddltype1 As DropDownList
        Dim arr1() As String
        Dim rtn As Integer
        da.SelectCommand.CommandText = "select * from MMM_MST_FORMVALIDATION where eid=" & eid & " and doctype='" & doctype & "' and docNature='" & DocNat & "'"
        Dim ds As New DataSet
        da.Fill(ds, "data")
        For Each rw As DataRow In ds.Tables("data").Rows

            If rw.Item("Valtype").ToString.ToUpper <> "DYNAMIC" Then
                da.SelectCommand.CommandText = "select datatype from mmm_mst_fields where eid=" & eid & " and documenttype='" & doctype & "' and fieldid=" & Right(rw.Item("fldID"), rw.Item("fldID").ToString.Length - 3) & ""
                If con.State <> ConnectionState.Open Then
                    con.Open()
                End If
                datatype = da.SelectCommand.ExecuteScalar()
                If rw.Item("Valtype").ToString.ToUpper = "FIELD" Then
                    da.SelectCommand.CommandText = "select datatype from mmm_mst_fields where eid=" & eid & " and documenttype='" & doctype & "' and fieldid=" & Right(rw.Item("Value"), rw.Item("Value").ToString.Length - 3) & ""
                    datatype1 = da.SelectCommand.ExecuteScalar
                End If

                If rw.Item("Valtype").ToString.ToUpper = "STATIC" Then
                    txtobj = TryCast(pnlFields.FindControl(rw.Item("fldID")), TextBox)
                    ddltype1 = TryCast(pnlFields.FindControl(rw.Item("fldID")), DropDownList)
                ElseIf rw.Item("Valtype").ToString.ToUpper = "FIELD" Then
                    txtobj = TryCast(pnlFields.FindControl(rw.Item("fldID")), TextBox)
                    ddltype1 = TryCast(pnlFields.FindControl(rw.Item("fldID")), DropDownList)
                    txtobj1 = TryCast(pnlFields.FindControl(rw.Item("Value")), TextBox)
                    ddltype = TryCast(pnlFields.FindControl(rw.Item("Value")), DropDownList)
                ElseIf rw.Item("Valtype").ToString.ToUpper = "MANDATORY" Then
                    txtobj = TryCast(pnlFields.FindControl(rw.Item("fldID")), TextBox)
                    ddltype1 = TryCast(pnlFields.FindControl(rw.Item("fldID")), DropDownList)
                    txtobj1 = TryCast(pnlFields.FindControl(rw.Item("Operator")), TextBox)
                    ddltype = TryCast(pnlFields.FindControl(rw.Item("Operator")), DropDownList)
                End If
            End If


            Dim tbname As String = ""
            Dim sb As String = ""
            Dim sts As String = ""

            If rw.Item("Valtype").ToString.ToUpper = "DYNAMIC" Then
                Dim ddlobj As DropDownList = TryCast(pnlFields.FindControl(rw.Item("fldid").ToString), DropDownList)
                If ddlobj.SelectedItem.Text.ToUpper = "SELECT" Then
                    Dim fid As Integer
                    Dim dispname As String = ""
                    fid = rw.Item("fldID").ToString.Length - 3
                    da.SelectCommand.CommandText = " select displayname from mmm_mst_fields where fieldid=" & fid & ""
                    If con.State <> ConnectionState.Open Then
                        con.Open()
                    End If
                    dispname = da.SelectCommand.ExecuteScalar()
                    errmsg = "Select " & dispname
                    con.Dispose()
                    Return errmsg
                    Exit Function
                End If
                arr1 = rw.Item("value").ToString.Split("-")
                Dim arr2 As String()
                Dim CNT As Integer = arr1.Length
                For i As Integer = 1 To arr1.Length - 1
                    arr2 = Split(arr1(i), ":")
                    If i = 1 Then
                        tbname = arr2(0).ToString
                    End If
                    If arr2.Length > 1 Then
                        sb = sb & arr2(1).ToString & ","
                    End If
                    If i > 1 Then
                        Dim opr As String = Left(arr2(0), 1)
                        Dim opr1 As String = Left(arr2(0), 2)
                        opr1 = opr1.Replace(opr, "")

                        If opr1 = "=" Then
                            opr = opr & opr1
                        End If

                        Dim txt As String = Right(arr2(0), arr2(0).Length - opr.Length).ToString
                        Dim txtbx1 As TextBox = TryCast(pnlFields.FindControl(txt), TextBox)
                        Dim ddllst As DropDownList = TryCast(pnlFields.FindControl(txt), DropDownList)
                        da.SelectCommand.CommandText = "select datatype from mmm_mst_fields where eid=" & eid & " and fieldid=" & Right(txt, txt.ToString.Length - 3) & ""
                        If con.State <> ConnectionState.Open Then
                            con.Open()
                        End If
                        datatype = da.SelectCommand.ExecuteScalar()
                        If CNT - 1 <> i Then
                            If IsNothing(ddllst) Then
                                If datatype.ToUpper = "NUMERIC" Then
                                    sts = sts & opr & "convert(float," & txtbx1.Text.ToString & ")" & "|"
                                ElseIf datatype.ToUpper = "DATETIME" Then
                                    sts = sts & opr & " convert(datetime," & "'" & getdate(txtbx1.Text.ToString) & "')" & "|"
                                Else
                                    sts = sts & opr & "'" & txtbx1.Text.ToString & "'" & "|"
                                End If
                            Else
                                sts = sts & opr & "'" & ddllst.SelectedValue & "'" & "|"
                            End If
                        Else
                            If IsNothing(ddllst) Then
                                If datatype.ToUpper = "NUMERIC" Then
                                    sts = sts & opr & "convert(float," & txtbx1.Text.ToString & ")" & "|"
                                ElseIf datatype.ToUpper = "DATETIME" Then
                                    sts = sts & opr & " convert(datetime," & "'" & getdate(txtbx1.Text.ToString) & "')" & "|"
                                Else
                                    sts = sts & opr & "'" & txtbx1.Text.ToString & "'" & "|"
                                End If
                            Else
                                sts = sts & opr & "'" & ddllst.SelectedValue & "'" & "|"
                            End If
                        End If
                    End If
                Next
                sb = Left(sb, sb.Length - 1)

                da.SelectCommand.CommandText = "select " & sb & " from " & tbname & " where tid=" & ddlobj.SelectedValue & ""
                Dim DS1 As New DataSet
                da.Fill(DS1, "DATA")
                Dim V1 As String = ""
                Dim sarr() As String
                Dim QR As String = " "
                sarr = sts.ToString.Split("|")
                If DS1.Tables("DATA").Rows.Count > 0 Then
                    For X As Integer = 0 To DS1.Tables("DATA").Columns.Count - 1
                        V1 = DS1.Tables("DATA").Rows(0).Item(X).ToString
                        If V1.ToString.Length > 6 And (V1.ToString.Contains("/") Or V1.ToString.Contains("-")) And Left(V1, 1).Contains("-") = False Then
                            QR = QR & " Convert(datetime," & "'" & getdate(V1) & "')" & sarr(X) & " and "
                        Else
                            QR = QR & "'" & V1 & "'" & sarr(X) & " and "
                        End If
                    Next
                End If
                QR = Trim(QR)
                QR = Left(QR, QR.Length - 3)

                'If rw.Item("WF_STATUS").ToString.Length > 2 Then
                '    Dim xwhrstatus As String = ""
                '    Dim WFSTATUS() As String = rw.Item("WF_STATUS").ToString.Split(",")
                '    If WFSTATUS.Length > 0 Then
                '        For i As Integer = 0 To WFSTATUS.Length - 1
                '            If i = 0 Then
                '                xwhrstatus = " and (curstatus='" & WFSTATUS(i).ToString & "'"

                '            Else
                '                xwhrstatus &= " or curstatus='" & WFSTATUS(i).ToString & "' "
                '            End If

                '        Next
                '        If xwhrstatus.Length > 5 Then
                '            xwhrstatus &= ")"
                '        End If
                '    End If
                '    QR &= xwhrstatus
                'End If

                da.SelectCommand.CommandText = "select count(tid) from " & tbname & " where eid=" & HttpContext.Current.Session("EID") & " and documenttype='" & arr1(0).ToString() & "' and TID=" & ddlobj.SelectedValue & " and " & QR & " "
            End If

            ''Validation for other Type

            If rw.Item("Valtype").ToString.ToUpper = "OTHER" Then
                Dim ddlobj As DropDownList = TryCast(pnlFields.FindControl(rw.Item("fldid").ToString), DropDownList)
                Dim txtbx1 As TextBox
                If ddlobj.SelectedItem.Text.ToUpper = "SELECT" Then
                    Dim fid As Integer
                    Dim dispname As String = ""
                    fid = rw.Item("fldID").ToString.Length - 3
                    da.SelectCommand.CommandText = " select displayname from mmm_mst_fields where fieldid=" & fid & ""
                    If con.State <> ConnectionState.Open Then
                        con.Open()
                    End If
                    dispname = da.SelectCommand.ExecuteScalar()
                    errmsg = "Select " & dispname
                    con.Dispose()
                    Return errmsg
                    Exit Function
                End If
                arr1 = rw.Item("value").ToString.Split("-")
                Dim arr2 As String()
                Dim CNT As Integer = arr1.Length
                For i As Integer = 1 To arr1.Length - 1
                    arr2 = Split(arr1(i), ":")
                    If i = 1 Then
                        tbname = arr2(0).ToString
                    End If
                    If arr2.Length > 1 Then
                        sb = sb & arr2(1).ToString & ","
                    End If
                    If i > 1 Then
                        Dim opr As String = Left(arr2(0), 1)
                        Dim opr1 As String = Left(arr2(0), 2)
                        opr1 = opr1.Replace(opr, "")

                        If opr1 = "=" Then
                            opr = opr & opr1
                        End If

                        Dim txt As String = Right(arr2(0), arr2(0).Length - opr.Length).ToString
                        txtbx1 = TryCast(pnlFields.FindControl(txt), TextBox)
                        Dim ddllst As DropDownList = TryCast(pnlFields.FindControl(txt), DropDownList)
                        da.SelectCommand.CommandText = "select datatype from mmm_mst_fields where eid=" & eid & " and fieldid=" & Right(txt, txt.ToString.Length - 3) & ""
                        If con.State <> ConnectionState.Open Then
                            con.Open()
                        End If
                        datatype = da.SelectCommand.ExecuteScalar()
                    End If
                Next
                sb = Left(sb, sb.Length - 1)

                da.SelectCommand.CommandText = "select " & sb & " from " & tbname & " where tid=" & ddlobj.SelectedValue & ""
                Dim DS1 As New DataSet
                da.Fill(DS1, "DATA")
                Dim V1 As String = ""
                Dim sarr() As String
                Dim QR As String = " "
                sarr = sts.ToString.Split("|")
                If DS1.Tables("DATA").Rows.Count > 0 Then
                    For X As Integer = 0 To DS1.Tables("DATA").Columns.Count - 1
                        V1 = DS1.Tables("DATA").Rows(0).Item(X).ToString
                    Next
                End If
                da.SelectCommand.CommandText = "select  case when " & txtbx1.Text & "  + " & V1 & ">=0 then 1 else 0 end "
            End If

            ''Validation For Duplicacy Check
            If rw.Item("Valtype").ToString.ToUpper = "DUPLICACYCHECK" Then
                Dim ddlobj As DropDownList = TryCast(pnlFields.FindControl(rw.Item("fldid").ToString), DropDownList)
                Dim txtbx1 As TextBox
                If ddlobj.SelectedItem.Text.ToUpper = "SELECT" Then
                    Dim fid As Integer
                    Dim dispname As String = ""
                    fid = rw.Item("fldID").ToString.Length - 3
                    da.SelectCommand.CommandText = " select displayname from mmm_mst_fields where fieldid=" & fid & ""
                    If con.State <> ConnectionState.Open Then
                        con.Open()
                    End If
                    dispname = da.SelectCommand.ExecuteScalar()
                    errmsg = "Select " & dispname
                    con.Dispose()
                    Return errmsg
                    Exit Function
                End If
                arr1 = rw.Item("value").ToString.Split(":")
                Dim arr2 As String()
                Dim CNT As Integer = arr1.Length
                Dim tbl As String = ""
                Dim documenttype As String = ""
                For ii As Integer = 0 To arr1.Length - 1
                    If ii = 0 Then
                        Dim arrr() As String = arr1(0).ToString.Split("-")
                        tbl = arrr(1).ToString
                        documenttype = arrr(0).ToString()
                    End If
                Next
                Dim xwhr As String = ""
                For i As Integer = 1 To arr1.Length - 1
                    arr2 = Split(arr1(i), "-")
                    If arr2.Length > 1 Then
                        Dim opr As String = Left(arr2(1), 1)
                        Dim opr1 As String = Left(arr2(1), 2)
                        opr1 = opr1.Replace(opr, "")

                        If opr1 = "=" Then
                            opr = opr & opr1
                        End If

                        Dim txt As String = Right(arr2(1), arr2(1).Length - opr.Length).ToString
                        Dim fldid As String = Right(txt, txt.Length - 3)
                        Dim row() As DataRow = dT.Select("fieldid=" & fldid & "")
                        Select Case row(0).Item("FieldType").ToString().ToUpper()
                            Case "DROP DOWN"
                                Dim TXTBOX As DropDownList = TryCast(pnlFields.FindControl(txt), DropDownList)
                                'xwhr &= " AND " & arr3(0).ToString & arr3(1).ToString & TXTBOX.SelectedValue.ToString
                                xwhr &= " AND " & arr2(0).ToString & "" & opr & "'" & TXTBOX.SelectedValue.ToString & "'"
                            Case "TEXT BOX"
                                Dim TXTBOX As TextBox = TryCast(pnlFields.FindControl(txt), TextBox)
                                If row(0).Item("datatype").ToString.ToUpper = "DATETIME" Then
                                    'xwhr &= " AND " & " CONVERT(DATETIME," & arr3(0).ToString & ",3) " & arr3(1).ToString & getdate(TXTBOX.Text.ToString) & ""
                                    xwhr &= " AND Convert(DateTime, " & arr2(0).ToString & ", 3) " & opr & "  CONVERT(DATE,'" & (TXTBOX.Text.ToString) & "',3) AND " & arr2(0).ToString & " <>''"
                                Else
                                    xwhr &= " AND " & arr2(0).ToString & "" & opr & "'" & TXTBOX.Text.ToString & "' AND " & arr2(0).ToString & " <>''"
                                End If
                        End Select
                    End If
                Next
                If rw.Item("WF_STATUS").ToString.Length > 2 Then
                    Dim xwhrstatus As String = ""
                    Dim WFSTATUS() As String = rw.Item("WF_STATUS").ToString.Split(",")
                    If WFSTATUS.Length > 0 Then
                        For i As Integer = 0 To WFSTATUS.Length - 1
                            If i = 0 Then
                                xwhrstatus = " and (curstatus='" & WFSTATUS(i).ToString & "'"

                            Else
                                xwhrstatus &= " or curstatus='" & WFSTATUS(i).ToString & "' "
                            End If

                        Next
                        If xwhrstatus.Length > 5 Then
                            xwhrstatus &= ")"
                        End If
                    End If
                    xwhr &= xwhrstatus
                End If
                If Action = "UPDATE" Then
                    xwhr &= " AND TID<>" & tid & ""
                End If

                da.SelectCommand.CommandText = "SELECT count(*) FROM MMM_MST_DOC WHERE EID=" & HttpContext.Current.Session("EID") & " and documenttype='" & documenttype & "' " & xwhr & ""
            End If

            'If i = 1 Then
            '    tbname = arr2(0).ToString
            'End If

            'If i > 1 Then
            '    Dim opr As String = Left(arr2(0), 1)
            '    Dim opr1 As String = Left(arr2(0), 2)
            '    opr1 = opr1.Replace(opr, "")
            '    If opr1 = "=" Then
            '        opr = opr & opr1
            '    End If

            '    Dim txt As String = Right(arr2(0), arr2(0).Length - opr.Length).ToString
            '    txtbx1 = TryCast(pnlFields.FindControl(txt), TextBox)
            '    Dim ddllst As DropDownList = TryCast(pnlFields.FindControl(txt), DropDownList)
            '    da.SelectCommand.CommandText = "select datatype from mmm_mst_fields where eid=" & eid & " and fieldid=" & Right(txt, txt.ToString.Length - 3) & ""
            '    If con.State <> ConnectionState.Open Then
            '        con.Open()
            '    End If
            '    datatype = da.SelectCommand.ExecuteScalar()
            'End If
            'Next
            ' sb = Left(sb, sb.Length - 1)

            'da.SelectCommand.CommandText = "select " & sb & " from " & tbname & " where tid=" & ddlobj.SelectedValue & ""
            'Dim DS1 As New DataSet
            'da.Fill(DS1, "DATA")
            'Dim V1 As String = ""
            'Dim sarr() As String
            'Dim QR As String = " "
            'sarr = sts.ToString.Split("|")
            'If DS1.Tables("DATA").Rows.Count > 0 Then
            '    For X As Integer = 0 To DS1.Tables("DATA").Columns.Count - 1
            '        V1 = DS1.Tables("DATA").Rows(0).Item(X).ToString
            '    Next
            'End If
            'da.SelectCommand.CommandText = "select  case when " & txtbx1.Text & "  + " & V1 & ">=0 then 1 else 0 end "

            If UCase(rw.Item("valtype").ToString) = "STATIC" Then
                If IsNothing(txtobj) Then
                    da.SelectCommand.CommandText = " select case when " & "'" & ddltype1.SelectedItem.Text & "'" & rw.Item("operator") & "'" & rw.Item("value") & "'" & "  then 1 else 0 end"
                Else
                    If datatype.ToString.ToUpper = "TEXT" Then
                        da.SelectCommand.CommandText = " select case when " & "'" & txtobj.Text.ToString & "'" & rw.Item("operator") & "'" & rw.Item("value") & "'" & "  then 1 else 0 end"
                    Else
                        da.SelectCommand.CommandText = " select case when " & "'" & txtobj.Text.ToString & "'" & rw.Item("operator") & rw.Item("value") & "  then 1 else 0 end"
                    End If
                End If
            ElseIf UCase(rw.Item("valtype").ToString) = "FIELD" Then
                If IsNothing(ddltype) Then
                    If datatype.ToUpper = "DATETIME" And datatype.ToUpper = "DATETIME" Then
                        da.SelectCommand.CommandText = " select case when " & " Convert(DateTime," & "'" & getdate(txtobj.Text.ToString) & "'" & ")" & rw.Item("operator") & "Convert(DateTime," & "'" & getdate(txtobj1.Text.ToString) & "'" & ")" & "  then 1 else 0 end"
                    ElseIf datatype.ToUpper = "NUMERIC" Then
                        da.SelectCommand.CommandText = " select cas5e when " & "" & txtobj.Text.ToString & "" & rw.Item("operator") & "" & txtobj1.Text.ToString & "" & "  then 1 else 0 end"
                    Else
                        da.SelectCommand.CommandText = " select case when " & "'" & txtobj.Text.ToString & "'" & rw.Item("operator") & "'" & txtobj1.Text.ToString & "'" & "  then 1 else 0 end"
                    End If
                Else
                    da.SelectCommand.CommandText = " select case when " & "'" & txtobj.Text.ToString & "'" & rw.Item("operator") & "'" & ddltype.SelectedItem.Text.ToString & "'" & "  then 1 else 0 end"
                End If
            ElseIf UCase(rw.Item("valtype").ToString) = "MANDATORY" Then
                If IsNothing(ddltype) Then
                    da.SelectCommand.CommandText = " select case when " & "'" & txtobj1.Text.ToString & "'" & "=" & "'" & rw.Item("value").ToString & "'" & "  then 1 else 0 end"
                Else
                    da.SelectCommand.CommandText = " select case when " & "'" & ddltype.SelectedItem.Text & "'" & "=" & "'" & rw.Item("value").ToString & "'" & "  then 1 else 0 end"
                End If
            End If

            If con.State <> ConnectionState.Open Then
                con.Open()
            End If

            Try
                rtn = da.SelectCommand.ExecuteScalar()
            Catch ex As Exception
                Return "check the entered data and ensure that all Mandatory fields have been filled with Valid data"
            End Try

            If rtn = 0 And rw.Item("valtype").ToString.ToUpper <> "DUPLICACYCHECK" Then
                If UCase(rw.Item("valtype").ToString) <> "MANDATORY" Then
                    errmsg = Trim(errmsg) & " " & rw.Item("err_msg").ToString()
                ElseIf UCase(rw.Item("valtype").ToString) = "OTHER" Then
                    errmsg = Trim(errmsg) & " " & rw.Item("err_msg").ToString()
                End If
            ElseIf rtn = 1 And (rw.Item("Valtype").ToString.ToUpper = "STATIC" Or rw.Item("Valtype").ToString.ToUpper = "FIELD" Or rw.Item("Valtype").ToString.ToUpper = "MANDATORY") Then
                If IsNothing(txtobj) Then
                    If ddltype1.SelectedItem.Text = "" Then
                        errmsg = Trim(errmsg) & " " & rw.Item("err_msg").ToString()
                    End If
                Else
                    If txtobj.Text.ToString = "" Then
                        errmsg = Trim(errmsg) & " " & rw.Item("err_msg").ToString()
                    End If
                End If
            ElseIf rtn >= 1 And rw.Item("valtype").ToString.ToUpper = "DUPLICACYCHECK" Then
                errmsg = Trim(errmsg) & " " & rw.Item("ERR_MSG").ToString
            End If
        Next
        con.Dispose()
        If errmsg.Length > 5 Then
            Return errmsg
        Else
            Return "True"
        End If
    End Function

    Function getdate(ByVal dbt As String) As DateTime
        Dim dtArr() As String
        dtArr = Split(dbt, "/")
        If dtArr.GetUpperBound(0) = 2 Then
            Dim dd, mm, yy As String
            dd = dtArr(0)
            mm = dtArr(1)
            yy = dtArr(2)
            Dim dt As Date
            Try
                dt = mm & "/" & dd & "/" & yy
                Return dt
            Catch ex As Exception
                Return Now.Date
            End Try
        Else
            Return Now.Date
        End If
    End Function

    Public Function InsertDynamicForm(ByVal FormName As String, ByVal formcaption As String, ByVal formdesc As String, ByVal EID As Integer, ByVal FormType As String, ByVal FormSource As String, ByVal LayoutType As String, ByVal eventname As String, ByVal subevent As String, ByVal curstatus As String, ByVal cal As Integer, ByVal WF As Integer, ByVal His As Integer, ByVal isrole As Integer, ByVal docM As String, ByVal chkPbE As Integer, ByVal chkPbV As Integer, ByVal curDocNature As String) As Integer
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        'new procedure with two more parameter
        Dim oda As SqlDataAdapter = New SqlDataAdapter("uspInsertDynamicFormNewRole", con)
        oda.SelectCommand.CommandType = CommandType.StoredProcedure
        oda.SelectCommand.Parameters.AddWithValue("Formname", FormName)
        oda.SelectCommand.Parameters.AddWithValue("formcaption", formcaption)
        oda.SelectCommand.Parameters.AddWithValue("formdesc", formdesc)
        oda.SelectCommand.Parameters.AddWithValue("EID", EID)
        oda.SelectCommand.Parameters.AddWithValue("formtype", FormType)
        oda.SelectCommand.Parameters.AddWithValue("formSource", FormSource)
        oda.SelectCommand.Parameters.AddWithValue("LayoutType", LayoutType)
        oda.SelectCommand.Parameters.AddWithValue("eventname", eventname)
        oda.SelectCommand.Parameters.AddWithValue("subevent", subevent)
        oda.SelectCommand.Parameters.AddWithValue("curstatus", curstatus)
        oda.SelectCommand.Parameters.AddWithValue("iscalendar", cal)
        oda.SelectCommand.Parameters.AddWithValue("isworkflow", WF)
        oda.SelectCommand.Parameters.AddWithValue("history", His)
        oda.SelectCommand.Parameters.AddWithValue("isrole", isrole)
        oda.SelectCommand.Parameters.AddWithValue("chkPbE", chkPbE)
        oda.SelectCommand.Parameters.AddWithValue("chkPbV", chkPbV)
        oda.SelectCommand.Parameters.AddWithValue("curDocNature", curDocNature)
        If docM <> "" Then
            oda.SelectCommand.Parameters.AddWithValue("docM", docM)
        End If
        If con.State <> ConnectionState.Open Then
            con.Open()
        End If
        Dim iSt As Integer = oda.SelectCommand.ExecuteScalar()
        con.Close()
        oda.Dispose()
        con.Dispose()
        Return iSt
    End Function

    Public Function UpdateDynamicForm(ByVal Formid As Integer, ByVal FormName As String, ByVal formcaption As String, ByVal formdesc As String, ByVal EID As Integer, ByVal FormType As String, ByVal FormSource As String, ByVal LayoutType As String, ByVal eventname As String, ByVal subevent As String, ByVal curstatus As String, ByVal cal As Integer, ByVal WF As Integer, ByVal His As Integer, ByVal isrole As Integer, ByVal docM As String, ByVal chkPbE As Integer, ByVal chkPbV As Integer, ByVal curDocNature As String) As Integer
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("uspUpdateDynamicFormNewRole", con)
        oda.SelectCommand.CommandType = CommandType.StoredProcedure
        oda.SelectCommand.Parameters.Clear()
        oda.SelectCommand.Parameters.AddWithValue("Fid", Formid)
        oda.SelectCommand.Parameters.AddWithValue("Formname", FormName)
        oda.SelectCommand.Parameters.AddWithValue("formcaption", formcaption)
        oda.SelectCommand.Parameters.AddWithValue("formdesc", formdesc)
        oda.SelectCommand.Parameters.AddWithValue("EID", EID)
        oda.SelectCommand.Parameters.AddWithValue("formtype", FormType)
        oda.SelectCommand.Parameters.AddWithValue("formSource", FormSource)
        oda.SelectCommand.Parameters.AddWithValue("LayoutType", LayoutType)
        oda.SelectCommand.Parameters.AddWithValue("eventname", eventname)
        oda.SelectCommand.Parameters.AddWithValue("subevent", subevent)
        oda.SelectCommand.Parameters.AddWithValue("curstatus", curstatus)

        oda.SelectCommand.Parameters.AddWithValue("iscalendar", cal)
        oda.SelectCommand.Parameters.AddWithValue("isworkflow", WF)
        oda.SelectCommand.Parameters.AddWithValue("history", His)
        oda.SelectCommand.Parameters.AddWithValue("isrole", isrole)
        oda.SelectCommand.Parameters.AddWithValue("chkPbE", chkPbE)
        oda.SelectCommand.Parameters.AddWithValue("chkPbV", chkPbV)
        oda.SelectCommand.Parameters.AddWithValue("CurdocNature", curDocNature)

        If docM <> "" Then
            oda.SelectCommand.Parameters.AddWithValue("docM", docM)
        End If

        If con.State <> ConnectionState.Open Then
            con.Open()
        End If
        Dim iSt As Integer = oda.SelectCommand.ExecuteScalar()
        con.Close()
        oda.Dispose()
        con.Dispose()
        Return iSt
    End Function

    Public Sub CLEARDYNAMICFIELDS(ByRef PnlFields As Panel)
        For Each ctl As Control In PnlFields.Controls
            If ctl.GetType().ToString.Equals("System.Web.UI.WebControls.TextBox") Then
                DirectCast(ctl, TextBox).Text = String.Empty
            ElseIf ctl.GetType().ToString.Equals("System.Web.UI.WebControls.ListBox") Then
                DirectCast(ctl, ListBox).ClearSelection()
            ElseIf ctl.GetType().ToString.Equals("System.Web.UI.WebControls.CheckBoxList") Then
                DirectCast(ctl, CheckBoxList).ClearSelection()
            ElseIf ctl.GetType().ToString.Equals("System.Web.UI.WebControls.DropDownList") Then
                DirectCast(ctl, DropDownList).SelectedIndex = 0
            ElseIf ctl.GetType().ToString.Equals("System.Web.UI.WebControls.Gridview") Then
                DirectCast(ctl, GridView).DataSource = ""
                DirectCast(ctl, GridView).DataBind()
            End If
        Next
    End Sub

    Public Sub CreateControlsOnAuthMetrix(ByVal ds As DataTable, ByRef pnlFields As Panel)
        If ds.Rows.Count > 0 Then
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As SqlConnection = New SqlConnection(conStr)
            Dim layout As String = ds.Rows(0).Item("LayoutType").ToString()
            pnlFields.Controls.Add(New LiteralControl("<div class=""form""><table width=""100%"" cellspacing=""5px"" border=""0px"" cellpadding=""0px"">"))
            Dim lblWidth As Integer = 130
            Dim controlWdth As Integer = 240
            Dim datatype As String = ""
            For i As Integer = 0 To ds.Rows.Count - 1
                Dim dispName As String = ds.Rows(i).Item("displayname").ToString()
                Dim lbl As New Label
                lbl.ID = "lbl" & ds.Rows(i).Item("FieldID").ToString()
                lbl.Text = dispName
                lbl.Font.Bold = True

                'If layout = "DOUBLE COLUMN" Then
                If i Mod 2 = 0 Then
                    pnlFields.Controls.Add(New LiteralControl("<tr>"))
                End If
                'Else
                'lblWidth = 210
                'controlWdth = 540
                'pnlFields.Controls.Add(New LiteralControl("<tr>"))
                'End If

                pnlFields.Controls.Add(New LiteralControl("<td style=""width:" & lblWidth & "px;text-align:right"">"))
                pnlFields.Controls.Add(lbl)
                pnlFields.Controls.Add(New LiteralControl("</td><td style=""width:" & controlWdth & "px;text-align:left"">"))
                Select Case ds.Rows(i).Item("FieldType").ToString().ToUpper()
                    Case "TEXT BOX"
                        datatype = ds.Rows(i).Item("datatype").ToString().ToUpper()
                        Dim txtBox As New TextBox
                        txtBox.ID = "fld" & ds.Rows(i).Item("FieldID").ToString()
                        txtBox.Width = controlWdth - 10
                        txtBox.CssClass = "txtBox"
                        pnlFields.Controls.Add(txtBox)

                        If datatype = "DATETIME" Then
                            Dim CLNDR As New AjaxControlToolkit.CalendarExtender
                            CLNDR.ID = "CLNDR" & ds.Rows(i).Item("FieldID").ToString()
                            CLNDR.TargetControlID = txtBox.ID
                            pnlFields.Controls.Add(CLNDR)

                        ElseIf datatype = "NUMERIC" Then
                            Dim WMExtnd As New AjaxControlToolkit.TextBoxWatermarkExtender
                            WMExtnd.ID = "WM" & ds.Rows(i).Item("FieldID").ToString()
                            WMExtnd.TargetControlID = txtBox.ID
                            WMExtnd.WatermarkCssClass = "water"
                            WMExtnd.WatermarkText = "Enter Range '-' separated e.g 100-1000"
                            pnlFields.Controls.Add(WMExtnd)
                        End If

                    Case "DROP DOWN"
                        Dim ddl As New ListBox
                        ddl.ID = "fld" & ds.Rows(i).Item("FieldID").ToString()
                        ddl.Width = controlWdth - 2
                        ddl.CssClass = "txtBox"
                        Dim ddlText As String = ds.Rows(i).Item("dropdown").ToString()
                        Dim dropdowntype As String = ds.Rows(i).Item("dropdowntype").ToString()
                        Dim arr() As String
                        If UCase(dropdowntype) = "FIX VALUED" Then
                            arr = ddlText.Split(",")
                            For ii As Integer = 0 To arr.Count - 1
                                ddl.Items.Add(arr(ii).ToUpper())
                            Next
                        ElseIf UCase(dropdowntype) = "MASTER VALUED" Then
                            arr = ddlText.Split("-")
                            Dim TABLENAME As String = ""
                            If UCase(arr(0).ToString()) = "MASTER" Then
                                TABLENAME = "MMM_MST_MASTER"
                            Else
                                TABLENAME = "MMM_MST_DOC"
                            End If
                            Dim str As String = "select " & arr(2).ToString() & ",tid from " & TABLENAME & " WHERE EID=" & HttpContext.Current.Session("EID") & " AND  DOCUMENTTYPE='" & arr(1).ToString() & "'"
                            Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
                            Dim dss As New DataSet
                            oda.SelectCommand.CommandText = str
                            oda.Fill(dss, "FV")
                            For J As Integer = 0 To dss.Tables("FV").Rows.Count - 1
                                ddl.Items.Add(dss.Tables("FV").Rows(J).Item(0).ToString())
                                ddl.Items(J).Value = dss.Tables("FV").Rows(J).Item(1)
                            Next
                            oda.Dispose()
                            dss.Dispose()
                        ElseIf UCase(dropdowntype) = "SESSION VALUED" Then
                            Dim oda1 As SqlDataAdapter = New SqlDataAdapter("", con)
                            Dim ds1 As New DataSet
                            Dim QRY As String = ""
                            Dim DROPDOWN As String() = ds.Rows(i).Item("DROPDOWN").ToString().Split("-")
                            If DROPDOWN(1).ToString.ToUpper = "USER" Then
                                QRY = "SELECT USERNAME ,UID FROM MMM_MST_USER WHERE EID=" & HttpContext.Current.Session("EID") & " "
                            ElseIf DROPDOWN(1).ToString.ToUpper = "LOCATION" Then
                                QRY = "SELECT LOCATIONNNAME ,LOCID FROM MMM_MST_LOCATION WHERE EID=" & HttpContext.Current.Session("EID") & ""
                            End If
                            oda1.SelectCommand.CommandText = QRY
                            oda1.Fill(ds1, "SESSION")
                            For iI As Integer = 0 To ds1.Tables("SESSION").Rows.Count - 1
                                ddl.Items.Add(ds1.Tables("SESSION").Rows(iI).Item(0))
                                ddl.Items(iI).Value = ds1.Tables("SESSION").Rows(iI).Item(1)
                            Next
                        End If
                        ddl.Items.Insert(0, "ALL")
                        ddl.SelectionMode = ListSelectionMode.Multiple
                        pnlFields.Controls.Add(ddl)

                    Case "CHECKBOX LIST"
                        Dim dynmdiv As System.Web.UI.HtmlControls.HtmlGenericControl = New System.Web.UI.HtmlControls.HtmlGenericControl("DIV")
                        dynmdiv.ID = "div" & ds.Rows(i).Item("FieldID").ToString()
                        Dim chklist As New CheckBoxList
                        chklist.ID = "chklist" & ds.Rows(i).Item("FieldID").ToString()
                        chklist.CssClass = "txtbox"
                        Dim ddlText As String = ds.Rows(i).Item("dropdown").ToString()
                        Dim dropdowntype As String = ds.Rows(i).Item("dropdowntype").ToString()
                        Dim arr() As String
                        If UCase(dropdowntype) = "FIX VALUED" Then
                            arr = ddlText.Split(",")
                            For ii As Integer = 0 To arr.Count - 1
                                chklist.Items.Add(arr(ii).ToUpper())
                            Next
                        ElseIf UCase(dropdowntype) = "MASTER VALUED" Then
                            arr = ddlText.Split("-")
                            Dim TABLENAME As String = ""
                            If UCase(arr(0).ToString()) = "MASTER" Then
                                TABLENAME = "MMM_MST_MASTER"
                            Else
                                TABLENAME = "MMM_MST_DOC"
                            End If
                            Dim str As String = "select " & arr(2).ToString() & ",tid from " & TABLENAME & " WHERE EID=" & HttpContext.Current.Session("EID") & " AND  DOCUMENTTYPE='" & arr(1).ToString() & "'"
                            Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
                            Dim dss As New DataSet
                            oda.SelectCommand.CommandText = str
                            oda.Fill(dss, "FV")
                            For J As Integer = 0 To dss.Tables("FV").Rows.Count - 1
                                chklist.Items.Add(dss.Tables("FV").Rows(J).Item(0))
                                chklist.Items(J).Value = dss.Tables("FV").Rows(J).Item(1)
                            Next
                            oda.Dispose()
                            dss.Dispose()
                        End If
                        dynmdiv.Controls.Add(chklist)
                        pnlFields.Controls.Add(dynmdiv)

                    Case "LIST BOX"
                        Dim ddl As New ListBox
                        ddl.ID = "fld" & ds.Rows(i).Item("FieldID").ToString()
                        ddl.Width = controlWdth - 2
                        ddl.CssClass = "txtBox"
                        Dim ddlText As String = ds.Rows(i).Item("dropdown").ToString()
                        Dim dropdowntype As String = ds.Rows(i).Item("dropdowntype").ToString()
                        Dim arr() As String
                        If UCase(dropdowntype) = "FIX VALUED" Then
                            arr = ddlText.Split(",")
                            ddl.Items.Add("")
                            For ii As Integer = 0 To arr.Count - 1
                                ddl.Items.Add(arr(ii).ToUpper())
                            Next
                        ElseIf UCase(dropdowntype) = "MASTER VALUED" Then
                            arr = ddlText.Split("-")
                            Dim TABLENAME As String = ""
                            If UCase(arr(0).ToString()) = "MASTER" Then
                                TABLENAME = "MMM_MST_MASTER"
                            Else
                                TABLENAME = "MMM_MST_DOC"
                            End If
                            Dim str As String = "select " & arr(2).ToString() & ",tid from " & TABLENAME & " WHERE EID=" & HttpContext.Current.Session("EID") & " AND  DOCUMENTTYPE='" & arr(1).ToString() & "'"
                            Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
                            Dim dss As New DataSet
                            oda.SelectCommand.CommandText = str
                            oda.Fill(dss, "FV")
                            For J As Integer = 0 To dss.Tables("FV").Rows.Count - 1
                                ddl.Items.Add(dss.Tables("FV").Rows(J).Item(0))
                                ddl.Items(J).Value = dss.Tables("FV").Rows(J).Item(1)
                            Next
                            oda.Dispose()
                            dss.Dispose()
                        End If
                        ddl.Items.Insert(0, "ALL")
                        ddl.SelectionMode = ListSelectionMode.Multiple
                        pnlFields.Controls.Add(ddl)
                    Case "TEXT AREA"
                        Dim txtBox As New TextBox
                        txtBox.ID = "fld" & ds.Rows(i).Item("FieldID").ToString()
                        txtBox.Width = controlWdth - 10
                        txtBox.CssClass = "txtBox"
                        txtBox.TextMode = TextBoxMode.MultiLine
                        pnlFields.Controls.Add(txtBox)
                End Select
                pnlFields.Controls.Add(New LiteralControl("</td>"))
                'If layout = "DOUBLE COLUMN" Then
                If i Mod 2 = 1 Then
                    pnlFields.Controls.Add(New LiteralControl("</tr>"))
                End If
                'Else
                'pnlFields.Controls.Add(New LiteralControl("</tr>"))
                'End If
            Next
            pnlFields.Controls.Add(New LiteralControl("</table></div>"))
        End If
        ds.Dispose()
    End Sub

    Public Function ValidateAndGenrateQueryForAUTHMATRIX(ByVal qryType As String, ByVal qryField As String, ByVal dataField As String, ByVal ds As DataTable, ByRef pnlFields As Panel) As String
        Dim errorMsg As String = "Please Enter "
        If ds.Rows.Count > 0 Then
            For i = 0 To ds.Rows.Count - 1
                Dim dispName As String = ds.Rows(i).Item("displayname").ToString()
                Select Case ds.Rows(i).Item("FieldType").ToString().ToUpper()
                    Case "TEXT BOX"
                        Dim txtBox As TextBox = CType(pnlFields.FindControl("fld" & ds.Rows(i).Item("FieldID").ToString()), TextBox)
                        'Validation for Mandatory
                        If ds.Rows(i).Item("isrequired").ToString() = 1 And txtBox.Text.Length < 1 Then
                            errorMsg &= dispName & ","
                        End If

                        If qryType = "ADD" Then
                            qryField &= ds.Rows(i).Item("Fieldmapping").ToString() & ","
                            dataField &= "'" & txtBox.Text & "',"
                        Else
                            qryField &= ds.Rows(i).Item("Fieldmapping").ToString() & "=" & "'" & txtBox.Text & "',"
                        End If
                    Case "DROP DOWN"
                        Dim txtBox As ListBox = CType(pnlFields.FindControl("fld" & ds.Rows(i).Item("FieldID").ToString()), ListBox)
                        If ds.Rows(i).Item("isrequired").ToString() = 1 And txtBox.SelectedItem.Text.Length < 1 Then
                            errorMsg &= dispName & ","
                        End If
                        Dim livalue As String = ""
                        If qryType = "ADD" Then
                            qryField &= ds.Rows(i).Item("Fieldmapping").ToString() & ","
                            If UCase(ds.Rows(i).Item("dropdowntype").ToString()) = "FIX VALUED" Then
                                If txtBox.SelectedItem.Text.ToUpper() = "ALL" Then
                                    livalue = "*"
                                Else
                                    For Each li As ListItem In txtBox.Items
                                        If li.Selected Then
                                            livalue &= li.Text & ","
                                        End If
                                    Next
                                    If livalue.Length > 0 Then
                                        livalue = Left(livalue, livalue.Length - 1)
                                    End If
                                End If
                                dataField &= "'" & livalue & "',"
                            Else
                                If txtBox.SelectedItem.Text.ToUpper() = "ALL" Then
                                    livalue = "*"
                                Else
                                    For Each li As ListItem In txtBox.Items
                                        If li.Selected Then
                                            livalue &= li.Value & ","
                                        End If
                                    Next
                                    If livalue.Length > 0 Then
                                        livalue = Left(livalue, livalue.Length - 1)
                                    End If
                                End If
                                dataField &= "'" & livalue & "',"
                            End If
                        Else
                            If UCase(ds.Rows(i).Item("dropdowntype").ToString()) = "FIX VALUED" Then
                                If txtBox.SelectedItem.Text.ToUpper() = "ALL" Then
                                    livalue = "*"
                                Else
                                    For Each li As ListItem In txtBox.Items
                                        If li.Selected Then
                                            livalue &= li.Text & ","
                                        End If
                                    Next
                                    livalue = Left(livalue, livalue.Length - 1)
                                End If
                                qryField &= ds.Rows(i).Item("Fieldmapping").ToString() & "=" & "'" & livalue & "',"
                            Else
                                If txtBox.SelectedItem.Text.ToUpper() = "ALL" Then
                                    livalue = "*"
                                Else
                                    For Each li As ListItem In txtBox.Items
                                        If li.Selected Then
                                            livalue &= li.Value & ","
                                        End If
                                    Next
                                    livalue = Left(livalue, livalue.Length - 1)
                                End If
                                qryField &= ds.Rows(i).Item("Fieldmapping").ToString() & "=" & "'" & livalue & "',"
                            End If
                        End If
                    Case "LIST BOX"
                        Dim txtBox As ListBox = CType(pnlFields.FindControl("fld" & ds.Rows(i).Item("FieldID").ToString()), ListBox)
                        If ds.Rows(i).Item("isrequired").ToString() = 1 And txtBox.SelectedItem.Text.Length < 1 Then
                            errorMsg &= dispName & ","
                        End If

                        Dim livalue As String = ""
                        If qryType = "ADD" Then
                            qryField &= ds.Rows(i).Item("Fieldmapping").ToString() & ","
                            If UCase(ds.Rows(i).Item("dropdowntype").ToString()) = "FIX VALUED" Then
                                If txtBox.SelectedItem.Text.ToUpper() = "ALL" Then
                                    livalue = "*"
                                Else
                                    For Each li As ListItem In txtBox.Items
                                        If li.Selected Then
                                            livalue &= li.Text & ","
                                        End If
                                    Next
                                    livalue = Left(livalue, livalue.Length - 1)
                                End If
                                dataField &= "'" & livalue & "',"
                            Else
                                If txtBox.SelectedItem.Text.ToUpper() = "ALL" Then
                                    livalue = "*"
                                Else
                                    For Each li As ListItem In txtBox.Items
                                        If li.Selected Then
                                            livalue &= li.Value & ","
                                        End If
                                    Next
                                    livalue = Left(livalue, livalue.Length - 1)
                                End If
                                dataField &= "'" & livalue & "',"
                            End If
                        Else
                            If UCase(ds.Rows(i).Item("dropdowntype").ToString()) = "FIX VALUED" Then
                                If txtBox.SelectedItem.Text.ToUpper() = "ALL" Then
                                    livalue = "*"
                                Else
                                    For Each li As ListItem In txtBox.Items
                                        If li.Selected Then
                                            livalue &= li.Text & ","
                                        End If
                                    Next
                                    livalue = Left(livalue, livalue.Length - 1)
                                End If
                                qryField &= ds.Rows(i).Item("Fieldmapping").ToString() & "=" & "'" & livalue & "',"
                            Else
                                If txtBox.SelectedItem.Text.ToUpper() = "ALL" Then
                                    livalue = "*"
                                Else
                                    For Each li As ListItem In txtBox.Items
                                        If li.Selected Then
                                            livalue &= li.Value & ","
                                        End If
                                    Next
                                    livalue = Left(livalue, livalue.Length - 1)
                                End If
                                qryField &= ds.Rows(i).Item("Fieldmapping").ToString() & "=" & "'" & livalue & "',"
                            End If
                        End If
                    Case "CHECKBOX LIST"
                        Dim txtBox As CheckBoxList = CType(pnlFields.FindControl("fld" & ds.Rows(i).Item("FieldID").ToString()), CheckBoxList)
                        If ds.Rows(i).Item("isrequired").ToString() = 1 And txtBox.SelectedItem.Text.Length < 1 Then
                            errorMsg &= dispName & ","
                        End If
                        Dim livalue As String = ""
                        If qryType = "ADD" Then
                            qryField &= ds.Rows(i).Item("Fieldmapping").ToString() & ","
                            If UCase(ds.Rows(i).Item("dropdowntype").ToString()) = "FIX VALUED" Then

                                For Each li As ListItem In txtBox.Items
                                    If li.Selected Then
                                        livalue &= li.Text & ","
                                    End If

                                Next
                                livalue = Left(livalue, livalue.Length - 1)
                                dataField &= "'" & livalue & "',"
                            Else
                                For Each li As ListItem In txtBox.Items
                                    If li.Selected Then
                                        livalue &= li.Value & ","
                                    End If
                                Next
                                livalue = Left(livalue, livalue.Length - 1)
                                dataField &= "'" & livalue & "',"
                            End If
                        Else
                            If UCase(ds.Rows(i).Item("dropdowntype").ToString()) = "FIX VALUED" Then
                                For Each li As ListItem In txtBox.Items
                                    If li.Selected Then
                                        livalue &= li.Text & ","
                                    End If
                                Next
                                livalue = Left(livalue, livalue.Length - 1)
                                qryField &= ds.Rows(i).Item("Fieldmapping").ToString() & "=" & "'" & livalue & "',"
                            Else
                                For Each li As ListItem In txtBox.Items
                                    If li.Selected Then
                                        livalue &= li.Value & ","
                                    End If
                                Next
                                livalue = Left(livalue, livalue.Length - 1)
                                qryField &= ds.Rows(i).Item("Fieldmapping").ToString() & "=" & "'" & livalue & "',"
                            End If
                        End If

                    Case "TEXT AREA"
                        Dim txtBox As TextBox = CType(pnlFields.FindControl("fld" & ds.Rows(i).Item("FieldID").ToString()), TextBox)
                        If ds.Rows(i).Item("isrequired").ToString() = 1 And txtBox.Text.Length < 1 Then
                            errorMsg &= dispName & ","
                        End If
                        If qryType = "ADD" Then
                            qryField &= ds.Rows(i).Item("Fieldmapping").ToString() & ","
                            dataField &= "'" & txtBox.Text & "',"
                        Else
                            qryField &= ds.Rows(i).Item("Fieldmapping").ToString() & "=" & "'" & txtBox.Text & "',"
                        End If
                End Select
            Next
        End If
        If errorMsg.Length < 14 Then
            If qryType = "ADD" Then
                Return Left(qryField, Len(qryField) - 1) & ")" & Left(dataField, Len(dataField) - 1) & ")"
            Else
                Return Left(qryField, Len(qryField) - 1)
            End If
        Else
            Return Left(errorMsg, Len(errorMsg) - 1)
        End If
    End Function

    'Public Sub FillControlsOnAuthMatrix(ByVal ds As DataTable, ByRef pnlFields As Panel, ByRef pnlUser As Panel, ByVal type As String, ByVal pid As Integer)
    '    If ds.Rows.Count > 0 Then
    '        Dim strcol As String = ""
    '        Dim strqry As String = ""
    '        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
    '        Dim con As SqlConnection = New SqlConnection(conStr)
    '        Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
    '        Dim dss As New DataSet
    '        For Each rw As DataRow In ds.Rows
    '            strcol &= rw.Item("fieldmapping").ToString & ","
    '        Next
    '        strcol = strcol.Substring(0, strcol.Length - 1)
    '        If UCase(type) = "MASTER" Then
    '            strqry = "Select  " & strcol & ",uid,doctype,aprstatus,sla from MMM_MST_AuthMetrix WHERE TID=" & pid & ""
    '        End If

    '        oda.SelectCommand.CommandText = strqry
    '        oda.Fill(dss, "data")
    '        Dim ddluser As New DropDownList
    '        ddluser = CType(pnlUser.FindControl("ddlUser1"), DropDownList)
    '        ddluser.SelectedIndex = ddluser.Items.IndexOf(ddluser.Items.FindByValue(dss.Tables("data").Rows(0).Item("uid").ToString()))
    '        Dim ddlStatus As DropDownList
    '        ddlStatus = CType(pnlUser.FindControl("ddlStatus1"), DropDownList)
    '        ddlStatus.SelectedIndex = ddlStatus.Items.IndexOf(ddlStatus.Items.FindByText(dss.Tables("data").Rows(0).Item("aprstatus").ToString()))
    '        Dim txtsla As New TextBox
    '        txtsla = CType(pnlUser.FindControl("txtsla"), TextBox)
    '        txtsla.Text = dss.Tables("data").Rows(0).Item("sla").ToString()
    '        For i As Integer = 0 To ds.Rows.Count - 1
    '            Select Case ds.Rows(i).Item("FieldType").ToString().ToUpper()
    '                Case "TEXT BOX"
    '                    Dim txtBox As New TextBox
    '                    txtBox = CType(pnlFields.FindControl("fld" & ds.Rows(i).Item("FieldID").ToString()), TextBox)
    '                    txtBox.Text = dss.Tables("data").Rows(0).Item(i).ToString()
    '                    If ds.Rows(i).Item("isEditable").ToString() = "0" Then
    '                        txtBox.Enabled = False
    '                    End If

    '                Case "DROP DOWN"
    '                    Dim ddl As New ListBox
    '                    ddl = CType(pnlFields.FindControl("fld" & ds.Rows(i).Item("FieldID").ToString()), ListBox)
    '                    Dim ARR() As String = dss.Tables("data").Rows(0).Item(i).ToString().Split(",")
    '                    If ds.Rows(i).Item("DROPDOWNType").ToString().ToUpper() = "FIX VALUED" Then
    '                        If ARR(0).ToString() = "*" Then
    '                            ddl.Items.FindByText("ALL").Selected = True
    '                        Else
    '                            For ii As Integer = 0 To ARR.Length - 1
    '                                ddl.Items.FindByText(ARR(ii).ToString()).Selected = True
    '                            Next
    '                        End If
    '                    Else
    '                        If ARR(0).ToString() = "*" Then
    '                            ddl.Items.FindByText("ALL").Selected = True
    '                        Else
    '                            For ii As Integer = 0 To ARR.Length - 1
    '                                ddl.Items.FindByValue(ARR(ii).ToString()).Selected = True
    '                            Next
    '                        End If
    '                    End If
    '                Case "CHECKBOX LIST"

    '                    Dim chklist As New CheckBoxList
    '                    chklist = CType(pnlFields.FindControl("fld" & ds.Rows(i).Item("FieldID").ToString()), CheckBoxList)
    '                    Dim ARR() As String = dss.Tables("data").Rows(0).Item(i).ToString().Split(",")
    '                    If ds.Rows(i).Item("DROPDOWNType").ToString().ToUpper() = "FIX VALUED" Then
    '                        For ii As Integer = 0 To ARR.Length - 1
    '                            'ddl.SelectedIndex = ddl.Items.IndexOf(ddl.Items.FindByText(ARR(ii).ToString()))
    '                            chklist.Items.FindByText(ARR(ii).ToString()).Selected = True
    '                        Next
    '                    Else
    '                        For ii As Integer = 0 To ARR.Length - 1
    '                            chklist.Items.FindByValue(ARR(ii).ToString()).Selected = True
    '                        Next
    '                    End If
    '                Case "LIST BOX"
    '                    Dim ddl As New ListBox
    '                    ddl = CType(pnlFields.FindControl("fld" & ds.Rows(i).Item("FieldID").ToString()), ListBox)
    '                    Dim ARR() As String = dss.Tables("data").Rows(0).Item(i).ToString().Split(",")
    '                    If ds.Rows(i).Item("DROPDOWNType").ToString().ToUpper() = "FIX VALUED" Then
    '                        If ARR(0).ToString() = "*" Then
    '                            ddl.Items.FindByText("ALL").Selected = True
    '                        Else
    '                            For ii As Integer = 0 To ARR.Length - 1
    '                                ddl.Items.FindByText(ARR(ii).ToString()).Selected = True
    '                            Next
    '                        End If
    '                    Else
    '                        If ARR(0).ToString() = "*" Then
    '                            ddl.Items.FindByText("ALL").Selected = True
    '                        Else
    '                            For ii As Integer = 0 To ARR.Length - 1
    '                                ddl.Items.FindByValue(ARR(ii).ToString()).Selected = True
    '                            Next
    '                        End If
    '                    End If
    '                Case "TEXT AREA"
    '                    Dim txtBox As New TextBox
    '                    txtBox = CType(pnlFields.FindControl("fld" & ds.Rows(i).Item("FieldID").ToString()), TextBox)
    '                    txtBox.Text = dss.Tables("data").Rows(0).Item(i).ToString()
    '            End Select
    '        Next
    '        oda.Dispose()
    '        dss.Dispose()
    '    End If
    '    ds.Dispose()
    'End Sub

    Public Sub FillControlsOnAuthMatrix(ByVal ds As DataTable, ByRef pnlFields As Panel, ByRef pnlUser As Panel, ByVal type As String, ByVal pid As Integer)
        If ds.Rows.Count > 0 Then
            Dim strcol As String = ""
            Dim strqry As String = ""
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As SqlConnection = New SqlConnection(conStr)
            Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
            Dim dss As New DataSet
            For Each rw As DataRow In ds.Rows
                strcol &= rw.Item("fieldmapping").ToString & ","
            Next
            strcol = strcol.Substring(0, strcol.Length - 1)
            If UCase(type) = "MASTER" Then
                strqry = "Select  " & strcol & ",uid,doctype,aprstatus,sla,ordering from MMM_MST_AuthMetrix WHERE TID=" & pid & ""
            End If

            oda.SelectCommand.CommandText = strqry
            oda.Fill(dss, "data")
            Dim ddluser As New DropDownList
            ddluser = CType(pnlUser.FindControl("ddlrole"), DropDownList)
            ddluser.SelectedIndex = ddluser.Items.IndexOf(ddluser.Items.FindByValue(dss.Tables("data").Rows(0).Item("uid").ToString()))
            Dim ddlStatus As DropDownList
            ddlStatus = CType(pnlUser.FindControl("ddlStatus1"), DropDownList)
            ddlStatus.SelectedIndex = ddlStatus.Items.IndexOf(ddlStatus.Items.FindByText(dss.Tables("data").Rows(0).Item("aprstatus").ToString()))
            Dim txtsla As New TextBox
            txtsla = CType(pnlUser.FindControl("txtsla"), TextBox)
            txtsla.Text = dss.Tables("data").Rows(0).Item("sla").ToString()
            Dim txtOrdering As New TextBox
            txtOrdering = CType(pnlUser.FindControl("txtOrdering"), TextBox)
            txtOrdering.Text = dss.Tables("data").Rows(0).Item("ordering").ToString()
            For i As Integer = 0 To ds.Rows.Count - 1
                Select Case ds.Rows(i).Item("FieldType").ToString().ToUpper()
                    Case "TEXT BOX"
                        Dim txtBox As New TextBox
                        txtBox = CType(pnlFields.FindControl("fld" & ds.Rows(i).Item("FieldID").ToString()), TextBox)
                        txtBox.Text = dss.Tables("data").Rows(0).Item(i).ToString()
                        If ds.Rows(i).Item("isEditable").ToString() = "0" Then
                            txtBox.Enabled = False
                        End If

                    Case "DROP DOWN"
                        Dim ddl As New ListBox
                        ddl = CType(pnlFields.FindControl("fld" & ds.Rows(i).Item("FieldID").ToString()), ListBox)
                        Dim ARR() As String = dss.Tables("data").Rows(0).Item(i).ToString().Split(",")
                        If ds.Rows(i).Item("DROPDOWNType").ToString().ToUpper() = "FIX VALUED" Then
                            If ARR(0).ToString() = "*" Then
                                ddl.Items.FindByText("ALL").Selected = True
                            Else
                                For ii As Integer = 0 To ARR.Length - 1
                                    ddl.Items.FindByText(ARR(ii).ToString()).Selected = True
                                Next
                            End If
                        Else
                            If ARR(0).ToString() = "*" Then
                                ddl.Items.FindByText("ALL").Selected = True
                            Else
                                For ii As Integer = 0 To ARR.Length - 1
                                    ddl.Items.FindByValue(ARR(ii).ToString()).Selected = True
                                Next
                            End If
                        End If
                    Case "CHECKBOX LIST"

                        Dim chklist As New CheckBoxList
                        chklist = CType(pnlFields.FindControl("fld" & ds.Rows(i).Item("FieldID").ToString()), CheckBoxList)
                        Dim ARR() As String = dss.Tables("data").Rows(0).Item(i).ToString().Split(",")
                        If ds.Rows(i).Item("DROPDOWNType").ToString().ToUpper() = "FIX VALUED" Then
                            For ii As Integer = 0 To ARR.Length - 1
                                'ddl.SelectedIndex = ddl.Items.IndexOf(ddl.Items.FindByText(ARR(ii).ToString()))
                                chklist.Items.FindByText(ARR(ii).ToString()).Selected = True
                            Next
                        Else
                            For ii As Integer = 0 To ARR.Length - 1
                                chklist.Items.FindByValue(ARR(ii).ToString()).Selected = True
                            Next
                        End If
                    Case "LIST BOX"
                        Dim ddl As New ListBox
                        ddl = CType(pnlFields.FindControl("fld" & ds.Rows(i).Item("FieldID").ToString()), ListBox)
                        Dim ARR() As String = dss.Tables("data").Rows(0).Item(i).ToString().Split(",")
                        If ds.Rows(i).Item("DROPDOWNType").ToString().ToUpper() = "FIX VALUED" Then
                            If ARR(0).ToString() = "*" Then
                                ddl.Items.FindByText("ALL").Selected = True
                            Else
                                For ii As Integer = 0 To ARR.Length - 1
                                    ddl.Items.FindByText(ARR(ii).ToString()).Selected = True
                                Next
                            End If
                        Else
                            If ARR(0).ToString() = "*" Then
                                ddl.Items.FindByText("ALL").Selected = True
                            Else
                                For ii As Integer = 0 To ARR.Length - 1
                                    ddl.Items.FindByValue(ARR(ii).ToString()).Selected = True
                                Next
                            End If
                        End If
                    Case "TEXT AREA"
                        Dim txtBox As New TextBox
                        txtBox = CType(pnlFields.FindControl("fld" & ds.Rows(i).Item("FieldID").ToString()), TextBox)
                        txtBox.Text = dss.Tables("data").Rows(0).Item(i).ToString()
                End Select
            Next
            oda.Dispose()
            dss.Dispose()
        End If
        ds.Dispose()
    End Sub

    Public Sub CreateControlOnCustom(ByVal ds As DataTable, ByRef pnlFields As Panel)
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim str As String = ds.Rows(0).Item("layoutdata").ToString()
        Dim str1 As String = ""
        Dim tr() As String = {"<tr"}
        'Dim id As Integer = 0
        Dim arr2() As String = str.Split(tr, StringSplitOptions.None)
        pnlFields.Controls.Add(New LiteralControl("<div class=""form"">"))
        pnlFields.Controls.Add(New LiteralControl(arr2(0).ToString()))
        For i = 1 To arr2.Length - 1
            tr = {"<td"}
            Dim arr1() As String = arr2(i).Split(tr, StringSplitOptions.None)
            pnlFields.Controls.Add(New LiteralControl("<tr " & arr1(0).ToString()))
            For j As Integer = 1 To arr1.Length - 1
                Dim controlWdth As Integer = 240
                If arr1(j).ToString().Contains("{") Then
                    str1 = arr1(j).Substring(0, arr1(j).IndexOf(">") + 1)
                    pnlFields.Controls.Add(New LiteralControl("<td " & str1))
                    str1 = arr1(j).Substring(0, arr1(j).IndexOf("<"))
                    str1 = (str1.Substring(str1.IndexOf("{") + 1, (str1.Length - 1) - (str1.IndexOf("{") + 1))).Trim()
                    If str1.Contains("{") Then
                        str1 = str1.Substring(1, str1.Length - 1)
                    End If
                    If str1.Contains("}") Then
                        str1 = str1.Substring(0, str1.Length - 1)
                    End If
                    Dim row() As DataRow = ds.Select("displayname='" & str1 & "'")
                    If row.Length > 0 Then
                        Dim id As String = row(0).Item("fieldid").ToString()
                        Select Case row(0).Item("FieldType").ToString().ToUpper()
                            Case "TEXT BOX"
                                Dim DataType = row(0).Item("datatype").ToString().ToUpper()
                                Dim txtBox As New TextBox
                                txtBox.ID = "fld" & id.ToString()
                                txtBox.Width = controlWdth - 10
                                txtBox.CssClass = "txtBox"
                                pnlFields.Controls.Add(txtBox)

                                If DataType = "DATETIME" Then
                                    Dim CLNDR As New AjaxControlToolkit.CalendarExtender
                                    CLNDR.ID = "CLNDR" & id.ToString()
                                    CLNDR.TargetControlID = txtBox.ID
                                    pnlFields.Controls.Add(CLNDR)
                                End If

                            Case "DROP DOWN"
                                Dim ddl As New DropDownList
                                ddl.ID = "fld" & id.ToString()
                                ddl.Width = controlWdth - 2
                                ddl.CssClass = "txtBox"
                                Dim ddlText As String = row(0).Item("dropdown").ToString()
                                Dim dropdowntype As String = row(0).Item("dropdowntype").ToString()
                                Dim arr() As String
                                If UCase(dropdowntype) = "FIX VALUED" Then
                                    arr = ddlText.Split(",")
                                    ddl.Items.Add("")
                                    For ii As Integer = 0 To arr.Count - 1
                                        ddl.Items.Add(arr(ii).ToUpper())
                                    Next
                                ElseIf UCase(dropdowntype) = "MASTER VALUED" Then
                                    arr = ddlText.Split("-")
                                    Dim TABLENAME As String = ""
                                    If UCase(arr(0).ToString()) = "MASTER" Then
                                        TABLENAME = "MMM_MST_MASTER"
                                    Else
                                        TABLENAME = "MMM_MST_DOC"
                                    End If
                                    Dim strqry As String = "select " & arr(2).ToString() & ",tid from " & TABLENAME & " WHERE EID=" & HttpContext.Current.Session("EID") & " AND  DOCUMENTTYPE='" & arr(1).ToString() & "'"
                                    Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
                                    Dim dss As New DataSet
                                    oda.SelectCommand.CommandText = strqry
                                    oda.Fill(dss, "FV")
                                    For J1 As Integer = 0 To dss.Tables("FV").Rows.Count - 1
                                        ddl.Items.Add(dss.Tables("FV").Rows(J1).Item(0).ToString())
                                        ddl.Items(J1).Value = dss.Tables("FV").Rows(J1).Item(1)
                                    Next
                                    oda.Dispose()
                                    dss.Dispose()
                                End If
                                pnlFields.Controls.Add(ddl)

                            Case "CHECKBOX LIST"
                                Dim ddl As New ListBox
                                Dim dynmdiv As System.Web.UI.HtmlControls.HtmlGenericControl = New System.Web.UI.HtmlControls.HtmlGenericControl("DIV")
                                dynmdiv.ID = "div" & id.ToString()
                                Dim chklist As New CheckBoxList
                                chklist.ID = "fld" & id.ToString()
                                chklist.CssClass = "txtbox"
                                Dim ddlText As String = row(0).Item("dropdown").ToString()
                                Dim dropdowntype As String = row(0).Item("dropdowntype").ToString()
                                Dim arr() As String
                                If UCase(dropdowntype) = "FIX VALUED" Then
                                    arr = ddlText.Split(",")
                                    For ii As Integer = 0 To arr.Count - 1
                                        chklist.Items.Add(arr(ii).ToUpper())
                                    Next
                                ElseIf UCase(dropdowntype) = "MASTER VALUED" Then
                                    arr = ddlText.Split("-")
                                    Dim TABLENAME As String = ""
                                    If UCase(arr(0).ToString()) = "MASTER" Then
                                        TABLENAME = "MMM_MST_MASTER"
                                    Else
                                        TABLENAME = "MMM_MST_DOC"
                                    End If
                                    Dim strqry As String = "select " & arr(2).ToString() & ",tid from " & TABLENAME & " WHERE EID=" & HttpContext.Current.Session("EID") & " AND  DOCUMENTTYPE='" & arr(1).ToString() & "'"
                                    Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
                                    Dim dss As New DataSet
                                    oda.SelectCommand.CommandText = strqry
                                    oda.Fill(dss, "FV")
                                    For J1 As Integer = 0 To dss.Tables("FV").Rows.Count - 1
                                        chklist.Items.Add(dss.Tables("FV").Rows(J1).Item(0))
                                        chklist.Items(J1).Value = dss.Tables("FV").Rows(J1).Item(1)
                                    Next
                                    oda.Dispose()
                                    dss.Dispose()
                                End If
                                dynmdiv.Controls.Add(chklist)
                                ddl.SelectionMode = ListSelectionMode.Multiple
                                pnlFields.Controls.Add(dynmdiv)

                            Case "LIST BOX"
                                Dim ddl As New ListBox
                                ddl.ID = "fld" & id.ToString()
                                ddl.Width = controlWdth - 2
                                ddl.CssClass = "txtBox"
                                Dim ddlText As String = row(0).Item("dropdown").ToString()
                                Dim dropdowntype As String = row(0).Item("dropdowntype").ToString()
                                Dim arr() As String
                                If UCase(dropdowntype) = "FIX VALUED" Then
                                    arr = ddlText.Split(",")
                                    ddl.Items.Add("")
                                    For ii As Integer = 0 To arr.Count - 1
                                        ddl.Items.Add(arr(ii).ToUpper())
                                    Next
                                ElseIf UCase(dropdowntype) = "MASTER VALUED" Then
                                    arr = ddlText.Split("-")
                                    Dim TABLENAME As String = ""
                                    If UCase(arr(0).ToString()) = "MASTER" Then
                                        TABLENAME = "MMM_MST_MASTER"
                                    Else
                                        TABLENAME = "MMM_MST_DOC"
                                    End If
                                    Dim strqry As String = "select " & arr(2).ToString() & ",tid from " & TABLENAME & " WHERE EID=" & HttpContext.Current.Session("EID") & " AND  DOCUMENTTYPE='" & arr(1).ToString() & "'"
                                    Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
                                    Dim dss As New DataSet
                                    oda.SelectCommand.CommandText = strqry
                                    oda.Fill(dss, "FV")
                                    For J1 As Integer = 0 To dss.Tables("FV").Rows.Count - 1
                                        ddl.Items.Add(dss.Tables("FV").Rows(J1).Item(0))
                                        ddl.Items(J1).Value = dss.Tables("FV").Rows(J1).Item(1)
                                    Next
                                    oda.Dispose()
                                    dss.Dispose()
                                End If
                                ddl.SelectionMode = ListSelectionMode.Multiple
                                pnlFields.Controls.Add(ddl)
                            Case "TEXT AREA"
                                Dim txtBox As New TextBox
                                txtBox.ID = "fld" & id.ToString()
                                txtBox.Width = controlWdth - 10
                                txtBox.CssClass = "txtBox"
                                txtBox.TextMode = TextBoxMode.MultiLine
                                pnlFields.Controls.Add(txtBox)
                        End Select
                        'id = id + 1
                    End If
                    pnlFields.Controls.Add(New LiteralControl("</td>"))
                Else
                    pnlFields.Controls.Add(New LiteralControl("<td " & arr1(j).ToString()))
                End If
            Next
            pnlFields.Controls.Add(New LiteralControl("</tr>"))
        Next
        pnlFields.Controls.Add(New LiteralControl("</table></div>"))
    End Sub

    Public Sub ApplyFieldsOnCustom(ByVal ds As DataTable, ByVal layout As String, ByRef pnlLayout As Panel)
        pnlLayout.Controls.Add(New LiteralControl("<div class=""form"" style=""width:100%"">"))
        Dim str As String = ""
        Dim tr() As String = {"<tr"}
        Dim td() As String = {"<td"}
        Dim arrtr() As String = layout.Split(tr, StringSplitOptions.None)
        pnlLayout.Controls.Add(New LiteralControl(arrtr(0).ToString()))
        For i As Integer = 1 To arrtr.Length - 1
            Dim arrtd() As String = arrtr(i).Split(td, StringSplitOptions.None)
            pnlLayout.Controls.Add(New LiteralControl("<tr " & arrtd(0).ToString()))
            For j As Integer = 1 To arrtd.Length - 1
                str = arrtd(j).Substring(0, arrtd(j).IndexOf(">") + 1)
                pnlLayout.Controls.Add(New LiteralControl("<td " & str))
                Dim ddl As New DropDownList
                ddl.ID = "fld" & i.ToString() & j.ToString()
                ddl.CssClass = "txtbox"
                For ii As Integer = 0 To ds.Rows.Count - 1
                    ddl.Items.Add(ds.Rows(ii).Item(0).ToString())
                    ddl.Items(ii).Value = ds.Rows(ii).Item(1).ToString()
                Next
                pnlLayout.Controls.Add(ddl)
                pnlLayout.Controls.Add(New LiteralControl("</td>"))
            Next
            pnlLayout.Controls.Add(New LiteralControl("</tr>"))
        Next
        pnlLayout.Controls.Add(New LiteralControl("</table></div>"))
        ds.Dispose()
    End Sub

    Public Function SaveFieldsOnCustom(ByVal datafield As String, ByVal ds As DataTable, ByVal layout As String, ByRef pnlLayout As Panel) As String
        Dim str As String = ""
        Dim strlayout As String = ""
        Dim tr() As String = {"<tr"}
        Dim td() As String = {"<td"}
        Dim arrtr() As String = layout.Split(tr, StringSplitOptions.None)
        strlayout &= arrtr(0).ToString()
        For i As Integer = 1 To arrtr.Length - 1
            Dim arrtd() As String = arrtr(i).Split(td, StringSplitOptions.None)
            strlayout &= "<tr " & arrtd(0).ToString()
            For j As Integer = 1 To arrtd.Length - 1
                str = arrtd(j).Substring(0, arrtd(j).IndexOf(">") + 1)
                strlayout &= "<td " & str
                Dim ddl As New DropDownList
                Dim txtBox As DropDownList = CType(pnlLayout.FindControl("fld" & i.ToString() & j.ToString()), DropDownList)
                strlayout &= txtBox.SelectedValue
                strlayout &= "</td>"
            Next
            strlayout &= "</tr>"
        Next
        strlayout &= "</table>"
        Return strlayout
    End Function

    Public Function ValidateAndGenrateQueryForCustom(ByVal qryType As String, ByVal qryField As String, ByVal dataField As String, ByVal ds As DataTable, ByRef pnlFields As Panel) As String
        Dim errorMsg As String = "Please Enter "
        If ds.Rows.Count > 0 Then
            For i = 0 To ds.Rows.Count - 1
                Dim dispName As String = ds.Rows(i).Item("displayname").ToString()
                Dim id As String = ds.Rows(i).Item("fieldid").ToString()
                Select Case ds.Rows(i).Item("FieldType").ToString().ToUpper()
                    Case "TEXT BOX"
                        Dim txtBox As TextBox = CType(pnlFields.FindControl("fld" & id.ToString()), TextBox)
                        'Validation for Mandatory
                        If txtBox Is Nothing Then
                            Continue For
                        End If
                        If ds.Rows(i).Item("isrequired").ToString() = 1 And txtBox.Text.Length < 1 Then
                            errorMsg &= dispName & ","
                        End If

                        If qryType = "ADD" Then
                            qryField &= ds.Rows(i).Item("Fieldmapping").ToString() & ","
                            dataField &= "'" & txtBox.Text & "',"
                        Else
                            qryField &= ds.Rows(i).Item("Fieldmapping").ToString() & "=" & "'" & txtBox.Text & "',"
                        End If
                    Case "DROP DOWN"
                        Dim txtBox As DropDownList = CType(pnlFields.FindControl("fld" & id.ToString()), DropDownList)
                        If txtBox Is Nothing Then
                            Continue For
                        End If
                        If ds.Rows(i).Item("isrequired").ToString() = 1 And txtBox.SelectedItem.Text.Length < 1 Then
                            errorMsg &= dispName & ","
                        End If
                        Dim livalue As String = ""
                        If qryType = "ADD" Then
                            qryField &= ds.Rows(i).Item("Fieldmapping").ToString() & ","
                            If UCase(ds.Rows(i).Item("dropdowntype").ToString()) = "FIX VALUED" Then
                                For Each li As ListItem In txtBox.Items
                                    If li.Selected Then
                                        livalue &= li.Text & ","
                                    End If
                                Next
                                If livalue.Length > 0 Then
                                    livalue = Left(livalue, livalue.Length - 1)
                                End If
                                dataField &= "'" & livalue & "',"
                            Else
                                For Each li As ListItem In txtBox.Items
                                    If li.Selected Then
                                        livalue &= li.Value & ","
                                    End If
                                Next
                                If livalue.Length > 0 Then
                                    livalue = Left(livalue, livalue.Length - 1)
                                End If
                                dataField &= "'" & livalue & "',"
                            End If
                        Else
                            If UCase(ds.Rows(i).Item("dropdowntype").ToString()) = "FIX VALUED" Then
                                For Each li As ListItem In txtBox.Items
                                    If li.Selected Then
                                        livalue &= li.Text & ","
                                    End If
                                Next
                                livalue = Left(livalue, livalue.Length - 1)
                                qryField &= ds.Rows(i).Item("Fieldmapping").ToString() & "=" & "'" & livalue & "',"
                            Else
                                For Each li As ListItem In txtBox.Items
                                    If li.Selected Then
                                        livalue &= li.Value & ","
                                    End If
                                Next
                                livalue = Left(livalue, livalue.Length - 1)
                                qryField &= ds.Rows(i).Item("Fieldmapping").ToString() & "=" & "'" & livalue & "',"
                            End If
                        End If
                    Case "LIST BOX"
                        Dim txtBox As ListBox = CType(pnlFields.FindControl("fld" & id.ToString()), ListBox)
                        If txtBox Is Nothing Then
                            Continue For
                        End If
                        If ds.Rows(i).Item("isrequired").ToString() = 1 And txtBox.SelectedItem.Text.Length < 1 Then
                            errorMsg &= dispName & ","
                        End If

                        Dim livalue As String = ""
                        If qryType = "ADD" Then
                            qryField &= ds.Rows(i).Item("Fieldmapping").ToString() & ","
                            If UCase(ds.Rows(i).Item("dropdowntype").ToString()) = "FIX VALUED" Then
                                For Each li As ListItem In txtBox.Items
                                    If li.Selected Then
                                        livalue &= li.Text & ","
                                    End If
                                Next
                                livalue = Left(livalue, livalue.Length - 1)
                                dataField &= "'" & livalue & "',"
                            Else
                                For Each li As ListItem In txtBox.Items
                                    If li.Selected Then
                                        livalue &= li.Value & ","
                                    End If
                                Next
                                livalue = Left(livalue, livalue.Length - 1)
                                dataField &= "'" & livalue & "',"
                            End If
                        Else
                            If UCase(ds.Rows(i).Item("dropdowntype").ToString()) = "FIX VALUED" Then
                                For Each li As ListItem In txtBox.Items
                                    If li.Selected Then
                                        livalue &= li.Text & ","
                                    End If
                                Next
                                livalue = Left(livalue, livalue.Length - 1)
                                qryField &= ds.Rows(i).Item("Fieldmapping").ToString() & "=" & "'" & livalue & "',"
                            Else
                                For Each li As ListItem In txtBox.Items
                                    If li.Selected Then
                                        livalue &= li.Value & ","
                                    End If
                                Next
                                livalue = Left(livalue, livalue.Length - 1)
                                qryField &= ds.Rows(i).Item("Fieldmapping").ToString() & "=" & "'" & livalue & "',"
                            End If
                        End If
                    Case "CHECKBOX LIST"
                        Dim txtBox As CheckBoxList = CType(pnlFields.FindControl("fld" & id.ToString()), CheckBoxList)
                        If txtBox Is Nothing Then
                            Continue For
                        End If
                        If ds.Rows(i).Item("isrequired").ToString() = 1 And txtBox.SelectedItem.Text.Length < 1 Then
                            errorMsg &= dispName & ","
                        End If
                        Dim livalue As String = ""
                        If qryType = "ADD" Then
                            qryField &= ds.Rows(i).Item("Fieldmapping").ToString() & ","
                            If UCase(ds.Rows(i).Item("dropdowntype").ToString()) = "FIX VALUED" Then

                                For Each li As ListItem In txtBox.Items
                                    If li.Selected Then
                                        livalue &= li.Text & ","
                                    End If

                                Next
                                livalue = Left(livalue, livalue.Length - 1)
                                dataField &= "'" & livalue & "',"
                            Else
                                For Each li As ListItem In txtBox.Items
                                    If li.Selected Then
                                        livalue &= li.Value & ","
                                    End If
                                Next
                                livalue = Left(livalue, livalue.Length - 1)
                                dataField &= "'" & livalue & "',"
                            End If
                        Else
                            If UCase(ds.Rows(i).Item("dropdowntype").ToString()) = "FIX VALUED" Then
                                For Each li As ListItem In txtBox.Items
                                    If li.Selected Then
                                        livalue &= li.Text & ","
                                    End If
                                Next
                                livalue = Left(livalue, livalue.Length - 1)
                                qryField &= ds.Rows(i).Item("Fieldmapping").ToString() & "=" & "'" & livalue & "',"
                            Else
                                For Each li As ListItem In txtBox.Items
                                    If li.Selected Then
                                        livalue &= li.Value & ","
                                    End If
                                Next
                                livalue = Left(livalue, livalue.Length - 1)
                                qryField &= ds.Rows(i).Item("Fieldmapping").ToString() & "=" & "'" & livalue & "',"
                            End If
                        End If

                    Case "TEXT AREA"
                        Dim txtBox As TextBox = CType(pnlFields.FindControl("fld" & id.ToString()), TextBox)
                        If txtBox Is Nothing Then
                            Continue For
                        End If
                        If ds.Rows(i).Item("isrequired").ToString() = 1 And txtBox.Text.Length < 1 Then
                            errorMsg &= dispName & ","
                        End If
                        If qryType = "ADD" Then
                            qryField &= ds.Rows(i).Item("Fieldmapping").ToString() & ","
                            dataField &= "'" & txtBox.Text & "',"
                        Else
                            qryField &= ds.Rows(i).Item("Fieldmapping").ToString() & "=" & "'" & txtBox.Text & "',"
                        End If
                End Select
            Next
        End If
        If errorMsg.Length < 14 Then
            If qryType = "ADD" Then
                Return Left(qryField, Len(qryField) - 1) & ")" & Left(dataField, Len(dataField) - 1) & ")"
            Else
                Return Left(qryField, Len(qryField) - 1)
            End If
        Else
            Return Left(errorMsg, Len(errorMsg) - 1)
        End If
    End Function

    Public Sub BINDITEMGRID(ByVal DS As DataTable, ByRef PNLFIELDS As Panel, ByVal ID As String, ByRef UPD As UpdatePanel, ByVal DS1 As DataTable)
        Dim GID As String = Right(ID, ID.Length - 3)
        Dim GV As GridView = CType(PNLFIELDS.FindControl("GRD" & GID.ToString()), GridView)
        Try
            If DS.Rows.Count > 0 Then
                Dim dr1 As DataRow = DS.NewRow()
                For i As Integer = 1 To DS.Columns.Count - 2
                    Dim total As Decimal = 0
                    For Each dr As DataRow In DS.Rows
                        If IsNumeric(dr.Item(i)) Then
                            total += dr.Item(i)
                            dr1(i) = total
                        Else
                            dr1(i) = ""
                        End If
                    Next
                Next
                dr1(0) = "Total"
                DS.Rows.Add(dr1)
                Dim rowcount As Integer = DS.Rows.Count
                For Each dr As DataRow In DS1.Rows
                    Dim txt As TextBox = CType(PNLFIELDS.FindControl("fld" & dr.Item("Fieldid").ToString()), TextBox)
                    txt.Text = DS.Rows(rowcount - 1).Item(dr.Item("displayname")).ToString()
                Next
            Else
                DS.Rows.Add(DS.NewRow())
                For Each dr As DataRow In DS1.Rows
                    Dim txt As TextBox = CType(PNLFIELDS.FindControl("fld" & dr.Item("Fieldid").ToString()), TextBox)
                    txt.Text = ""
                Next
            End If
            GV.DataSource = DS
            GV.DataBind()
        Catch ex As Exception

        End Try
        UPD.Update()
    End Sub

    ''bind Grid to new Functionalities 
    Public Sub BINDITEMGRID1(ByVal DS As DataTable, ByRef PNLFIELDS As Panel, ByVal ID As String, ByRef UPD As UpdatePanel, ByVal DS1 As DataTable)
        Dim GID As String = ID
        Dim GV As GridView = CType(PNLFIELDS.FindControl("GRD" & GID.ToString()), GridView)
        Try
            Dim dt As DataTable = DS

            If dt.Rows.Count > 1 Then
                dt.Rows.RemoveAt(dt.Rows.Count - 1)
                Dim dr1 As DataRow = dt.NewRow()
                For i As Integer = 1 To dt.Columns.Count - 2
                    Dim total As Decimal = 0
                    For Each dr As DataRow In dt.Rows
                        If IsNumeric(dr.Item(i)) Then
                            total += dr.Item(i)
                            dr1(i) = total
                        Else
                            dr1(i) = ""
                        End If
                    Next
                Next
                dr1(0) = "Total"
                dt.Rows.Add(dr1)
                Dim rowcount As Integer = dt.Rows.Count
                For Each dr As DataRow In DS1.Rows
                    Dim txt As TextBox = CType(PNLFIELDS.FindControl("fld" & dr.Item("Fieldid").ToString()), TextBox)
                    txt.Text = dt.Rows(rowcount - 1).Item(dr.Item("displayname")).ToString()
                Next
            Else
                dt.Rows.Add(dt.NewRow())
                Dim rowcount As Integer = dt.Rows.Count
                For Each dr As DataRow In DS1.Rows
                    Dim txt As TextBox = CType(PNLFIELDS.FindControl("fld" & dr.Item("Fieldid").ToString()), TextBox)
                    txt.Text = ""
                Next
            End If
            GV.DataSource = dt
            GV.DataBind()
        Catch ex As Exception

        End Try
        UPD.Update()
    End Sub

    Public Sub bind(ByVal id1 As Integer, ByRef pnlFields As Panel, ByRef ddl As DropDownList)
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        If con.State <> ConnectionState.Open Then
            con.Open()
        End If
        Dim oda As SqlDataAdapter = New SqlDataAdapter("select LOOKUPVALUE,dropdown,documenttype from MMM_MST_FIELDS WHERE FIELDID=" & id1 & "", con)
        Dim DS As New DataSet
        Dim xwhr As String = ""
        oda.Fill(DS, "data")
        Dim LOOKUPVALUE As String = DS.Tables("data").Rows(0).Item("lookupvalue").ToString()
        Dim documenttype() As String = DS.Tables("data").Rows(0).Item("dropdown").ToString.Split("-")
        If LOOKUPVALUE.Length > 0 Then
            Dim lookfld() As String = LOOKUPVALUE.ToString().Split(",")
            If lookfld.Length > 0 Then
                For iLookFld As Integer = 0 To lookfld.Length - 1
                    Dim fldPair() As String = lookfld(iLookFld).Split("-")
                    If fldPair.Length > 1 Then
                        If GetControl(pnlFields, "fld" & fldPair(0).ToString()) Then
                            oda = New SqlDataAdapter("SELECT * FROM MMM_MST_FIELDS WHERE FIELDID=" & fldPair(0) & "", con)
                            Dim dt As New DataTable
                            oda.Fill(dt)
                            Dim STR As String = ""
                            If fldPair(1).ToString.ToUpper = "C" Then
                                Dim proc As String = dt.Rows(0).Item("CAL_FIELDS").ToString()
                                If proc.Length > 1 Then
                                    Dim DROPDOWN1 As String = dt.Rows(0).Item("AUTOFILTER").ToString()
                                    Dim DDL0 As DropDownList = TryCast(pnlFields.FindControl("fld" & DROPDOWN1), DropDownList)
                                    If DDL0.SelectedItem.Text.ToUpper <> "SELECT" Then
                                        oda.SelectCommand.CommandType = CommandType.StoredProcedure
                                        oda.SelectCommand.Parameters.Clear()
                                        oda.SelectCommand.CommandText = proc
                                        oda.SelectCommand.Parameters.AddWithValue("DOCID", DDL0.SelectedValue)
                                        oda.SelectCommand.Parameters.AddWithValue("FIELDID", CInt(DROPDOWN1))
                                        oda.SelectCommand.Parameters.AddWithValue("VALUE", ddl.SelectedValue)
                                        Dim dss As New DataTable
                                        oda.Fill(dss)
                                        Dim ddl1 As DropDownList = TryCast(pnlFields.FindControl("fld" & fldPair(0).ToString()), DropDownList)
                                        ddl1.Items.Clear()
                                        ddl1.Items.Add("SELECT")
                                        ddl1.Items(0).Value = "0"
                                        For i As Integer = 0 To dss.Rows.Count - 1
                                            ddl1.Items.Add(dss.Rows(i).Item(0).ToString())
                                            ddl1.Items(i + 1).Value = dss.Rows(i).Item("tID")
                                        Next
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
                                Dim ddl1 As DropDownList = TryCast(pnlFields.FindControl("fld" & fldPair(0).ToString()), DropDownList)
                                ddl1.Items.Clear()
                                ddl1.Items.Add("SELECT")
                                ddl1.Items(0).Value = "0"
                                For i As Integer = 0 To dss.Rows.Count - 1
                                    ddl1.Items.Add(dss.Rows(i).Item(0).ToString())
                                    ddl1.Items(i + 1).Value = dss.Rows(i).Item("tID")
                                Next
                            Else
                                Dim DROPDOWN As String() = dt.Rows(0).Item("DROPDOWN").ToString.Split("-")
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
                                If dt.Rows(0).Item("fieldtype").ToString.ToUpper() = "DROP DOWN" Then
                                    Dim AUTOFILTER As String = dt.Rows(0).Item("AUTOFILTER").ToString()
                                    Dim tids As String = ""

                                    ''Filter Data according to Userid
                                    tids = UserDataFilter(DS.Tables("data").Rows(0).Item("documenttype").ToString(), DROPDOWN(1).ToString())
                                    If tids.Length > 2 Then
                                        xwhr = " AND CONVERT(NVARCHAR(10),TID) IN (" & tids & ")"
                                    Else
                                        xwhr = ""
                                    End If

                                    ''Call the Procedure to Bind DropDown @eid int,
                                    '@TableName nvarchar(40),
                                    '@Val int,
                                    '@xwhr nvarchar(max)=null,
                                    '@tid nvarchar(40),
                                    '@documenttype nvarchar(100),
                                    '@fldmapping nvarchar(40),
                                    '@autofilter nvarchar(40)=null

                                    oda.SelectCommand.CommandType = CommandType.StoredProcedure
                                    oda.SelectCommand.CommandText = "USP_BINDDDL"
                                    oda.SelectCommand.Parameters.Clear()
                                    oda.SelectCommand.Parameters.AddWithValue("@EID", HttpContext.Current.Session("EID"))
                                    oda.SelectCommand.Parameters.AddWithValue("@TableName", TABLENAME)
                                    oda.SelectCommand.Parameters.AddWithValue("@Val", SLVALUE(0))
                                    oda.SelectCommand.Parameters.AddWithValue("@xwhr", xwhr)
                                    oda.SelectCommand.Parameters.AddWithValue("@tid", TID)
                                    oda.SelectCommand.Parameters.AddWithValue("@documenttype", DROPDOWN(1))
                                    oda.SelectCommand.Parameters.AddWithValue("@fldmapping", DROPDOWN(2))
                                    oda.SelectCommand.Parameters.AddWithValue("@autofilter", AUTOFILTER)
                                    'oda.SelectCommand.CommandText = STR & " AND isAuth=1 " & xwhr & " Order by " & DROPDOWN(2).ToString()
                                    Dim dtFinal As New DataTable
                                    oda.Fill(dtFinal)

                                    Dim ddlo As DropDownList = TryCast(pnlFields.FindControl("fld" & fldPair(0).ToString()), DropDownList)
                                    ddlo.Items.Clear()
                                    ddlo.Items.Add("SELECT")
                                    ddlo.Items(0).Value = "0"
                                    For i As Integer = 0 To dtFinal.Rows.Count - 1
                                        ddlo.Items.Add(dtFinal.Rows(i).Item(0).ToString())
                                        ddlo.Items(i + 1).Value = dtFinal.Rows(i).Item("tID")
                                    Next
                                Else
                                    Dim TID1 As String() = ddl.SelectedValue.ToString.Split("|")
                                    Dim SELTID As String = ""
                                    If TID1.Length > 1 Then
                                        SELTID = TID1(1).ToString
                                    Else
                                        SELTID = TID1(0).ToString
                                    End If
                                    Dim value As String = ""
                                    If SELTID.ToString <> "0" And SELTID.ToString <> "" Then
                                        oda = New SqlDataAdapter("", con)
                                        oda.SelectCommand.CommandType = CommandType.StoredProcedure
                                        oda.SelectCommand.Parameters.Clear()
                                        oda.SelectCommand.CommandText = "uspGetMasterValue"
                                        oda.SelectCommand.Parameters.AddWithValue("EID", HttpContext.Current.Session("EID"))
                                        oda.SelectCommand.Parameters.AddWithValue("documentType", documenttype(1))
                                        oda.SelectCommand.Parameters.AddWithValue("Type", documenttype(0))
                                        oda.SelectCommand.Parameters.AddWithValue("TID", SELTID)
                                        oda.SelectCommand.Parameters.AddWithValue("FLDMAPPING", fldPair(1))
                                        If con.State <> ConnectionState.Open Then
                                            con.Open()
                                        End If
                                        value = oda.SelectCommand.ExecuteScalar().ToString()
                                    End If
                                    Dim TXTBOX As TextBox = TryCast(pnlFields.FindControl("fld" & fldPair(0).ToString().Trim()), TextBox)
                                    TXTBOX.Text = value
                                    'Dim proc As String = dt.Rows(0).Item("dropdowntype").ToString()
                                    'If proc.Length > 1 Then
                                    '    Dim DROPDOWN1 As String = dt.Rows(0).Item("DROPDOWN").ToString()
                                    '    Dim DDL0 As DropDownList = TryCast(pnlFields.FindControl("fld" & DROPDOWN1), DropDownList)
                                    '    oda.SelectCommand.CommandType = CommandType.StoredProcedure
                                    '    oda.SelectCommand.Parameters.Clear()
                                    '    oda.SelectCommand.CommandText = proc
                                    '    oda.SelectCommand.Parameters.AddWithValue("VcNo", value)
                                    '    oda.SelectCommand.Parameters.AddWithValue("fldmapping", fldPair(1))
                                    '    oda.SelectCommand.Parameters.AddWithValue("FIELDID", CInt(DROPDOWN1))
                                    '    oda.SelectCommand.Parameters.AddWithValue("VALUE", DDL0.SelectedValue)
                                    '    Dim dss As New DataTable
                                    '    oda.Fill(dss)
                                    '    Dim ddl1 As DropDownList = TryCast(pnlFields.FindControl("fld" & dt.Rows(0).Item("autofilter").ToString()), DropDownList)
                                    '    ddl1.Items.Clear()
                                    '    ddl1.Items.Add("SELECT")
                                    '    ddl1.Items(0).Value = "0"
                                    '    For i As Integer = 0 To dss.Rows.Count - 1
                                    '        ddl1.Items.Add(dss.Rows(i).Item(0).ToString())
                                    '        ddl1.Items(i + 1).Value = dss.Rows(i).Item("tID")
                                    '    Next
                                    'End If
                                End If
                            End If
                        End If
                    End If
                Next
            End If
        End If
        con.Dispose()
        oda.Dispose()
    End Sub

    Public Sub totalrow(ByVal sender As Object, ByVal e As GridViewRowEventArgs)
        If e.Row.RowType = DataControlRowType.DataRow Then
            If e.Row.Cells(1).Text.ToUpper() = "TOTAL" Then
                e.Row.Font.Bold = True
                e.Row.ForeColor = Drawing.Color.Black
            Else
                Dim img As ImageButton = New ImageButton()
                img.ID = e.Row.Cells(0).Text
                img.ImageUrl = "~/images/Cancel.gif"
                img.CommandName = "Remove"
                img.CommandArgument = e.Row.Cells(0).Text
                img.Height = Unit.Parse("16")
                img.Width = Unit.Parse("16")
                e.Row.Cells(0).Controls.Add(img)
            End If
        End If
    End Sub

    'Public Sub bind(ByVal id1 As Integer, ByRef pnlFields As Panel, ByRef ddl As DropDownList)
    '    Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
    '    Dim con As SqlConnection = New SqlConnection(conStr)
    '    If con.State <> ConnectionState.Open Then
    '        con.Open()
    '    End If
    '    Dim oda As SqlDataAdapter = New SqlDataAdapter("select LOOKUPVALUE,dropdown,documenttype from MMM_MST_FIELDS WHERE FIELDID=" & id1 & "", con)
    '    Dim DS As New DataSet
    '    Dim xwhr As String = ""
    '    oda.Fill(DS, "data")
    '    Dim LOOKUPVALUE As String = DS.Tables("data").Rows(0).Item("lookupvalue").ToString()
    '    Dim documenttype() As String = DS.Tables("data").Rows(0).Item("dropdown").ToString.Split("-")
    '    If LOOKUPVALUE.Length > 0 Then
    '        Dim lookfld() As String = LOOKUPVALUE.ToString().Split(",")
    '        If lookfld.Length > 0 Then
    '            For iLookFld As Integer = 0 To lookfld.Length - 1
    '                Dim fldPair() As String = lookfld(iLookFld).Split("-")
    '                If fldPair.Length > 1 Then
    '                    oda = New SqlDataAdapter("SELECT * FROM MMM_MST_FIELDS WHERE FIELDID=" & fldPair(0) & "", con)
    '                    Dim dt As New DataTable
    '                    oda.Fill(dt)
    '                    Dim STR As String = ""
    '                    If fldPair(1).ToString.ToUpper = "C" Then
    '                        Dim proc As String = dt.Rows(0).Item("CAL_FIELDS").ToString()
    '                        If proc.Length > 1 Then
    '                            Dim DROPDOWN1 As String = dt.Rows(0).Item("AUTOFILTER").ToString()
    '                            Dim DDL0 As DropDownList = TryCast(pnlFields.FindControl("fld" & DROPDOWN1), DropDownList)
    '                            oda.SelectCommand.CommandType = CommandType.StoredProcedure
    '                            oda.SelectCommand.Parameters.Clear()
    '                            oda.SelectCommand.CommandText = proc
    '                            oda.SelectCommand.Parameters.AddWithValue("DOCID", DDL0.SelectedValue)
    '                            oda.SelectCommand.Parameters.AddWithValue("FIELDID", CInt(DROPDOWN1))
    '                            oda.SelectCommand.Parameters.AddWithValue("VALUE", ddl.SelectedValue)
    '                            Dim dss As New DataTable
    '                            oda.Fill(dss)
    '                            Dim ddl1 As DropDownList = TryCast(pnlFields.FindControl("fld" & fldPair(0).ToString()), DropDownList)
    '                            ddl1.Items.Clear()
    '                            ddl1.Items.Add("SELECT")
    '                            ddl1.Items(0).Value = "0"
    '                            For i As Integer = 0 To dss.Rows.Count - 1
    '                                ddl1.Items.Add(dss.Rows(i).Item(0).ToString())
    '                                ddl1.Items(i + 1).Value = dss.Rows(i).Item("tID")
    '                            Next
    '                        End If
    '                    Else
    '                        Dim DROPDOWN As String() = dt.Rows(0).Item("DROPDOWN").ToString.Split("-")
    '                        Dim TABLENAME As String = ""
    '                        Dim TID As String = "TID"
    '                        If UCase(DROPDOWN(0).ToString()) = "MASTER" Then
    '                            TABLENAME = "MMM_MST_MASTER"
    '                        ElseIf UCase(DROPDOWN(0).ToString()) = "DOCUMENT" Then
    '                            TABLENAME = "MMM_MST_DOC"
    '                        ElseIf UCase(DROPDOWN(0).ToString()) = "CHILD" Then
    '                            TABLENAME = "MMM_MST_DOC_ITEM"
    '                        ElseIf UCase(DROPDOWN(0).ToString()) = "STATIC" Then
    '                            If UCase(DROPDOWN(1).ToString()) = "USER" Then
    '                                TABLENAME = "MMM_MST_USER"
    '                                TID = "UID"
    '                            ElseIf UCase(DROPDOWN(1).ToString()) = "LOCATION" Then
    '                                TABLENAME = "MMM_MST_LOCATION"
    '                                If DROPDOWN(2).ToString.ToUpper = "LOCATIONSTATE" Then
    '                                    TID = "SID"
    '                                Else
    '                                    TID = "LOCID"
    '                                End If
    '                            Else
    '                                TABLENAME = dt.Rows(0).Item("DBTABLENAME").ToString
    '                            End If
    '                        ElseIf UCase(DROPDOWN(0).ToString()) = "SESSION" Then
    '                            TABLENAME = "MMM_MST_USER"
    '                            TID = "UID"
    '                        End If
    '                        Dim SLVALUE As String() = ddl.SelectedValue.Split("|")
    '                        If dt.Rows(0).Item("fieldtype").ToString.ToUpper() = "DROP DOWN" Then
    '                            Dim AUTOFILTER As String = dt.Rows(0).Item("AUTOFILTER").ToString()
    '                            If TABLENAME <> "MMM_MST_USER" And TABLENAME <> "MMM_MST_LOCATION" Then
    '                                STR = "select " & DROPDOWN(2).ToString() & ",convert(nvarchar(10),tid) [tid] from " & TABLENAME & " P WHERE EID=" & HttpContext.Current.Session("EID") & " AND  DOCUMENTTYPE='" & DROPDOWN(1).ToString() & "' AND " & AUTOFILTER & " =" & SLVALUE(0) & "  "
    '                            Else
    '                                If TABLENAME = "MMM_MST_LOCATION" Then
    '                                    If AUTOFILTER.ToUpper = "LOCATIONSTATE" Then
    '                                        STR = "select " & DROPDOWN(2).ToString() & ",convert(nvarchar(10)," & TID & ")  [tid] from " & TABLENAME & " P WHERE  SID=" & SLVALUE(0) & " "
    '                                    Else
    '                                        STR = "select " & DROPDOWN(2).ToString() & ",convert(nvarchar(10)," & TID & ")  [tid] from " & TABLENAME & " P WHERE  " & AUTOFILTER & "=" & SLVALUE(0) & " "
    '                                    End If
    '                                Else
    '                                    STR = "select " & DROPDOWN(2).ToString() & ",convert(nvarchar(10)," & TID & ")  [tid] from " & TABLENAME & " P WHERE EID=" & HttpContext.Current.Session("EID") & " AND " & AUTOFILTER & "=" & SLVALUE(0) & " "
    '                                End If
    '                            End If
    '                            Dim tids As String = ""

    '                            ''Filter Data according to Userid
    '                            tids = UserDataFilter(DS.Tables("data").Rows(0).Item("documenttype").ToString(), DROPDOWN(1).ToString())
    '                            If tids.Length > 2 Then
    '                                xwhr = " AND CONVERT(NVARCHAR(10),TID) IN (" & tids & ")"
    '                            Else
    '                                xwhr = ""
    '                            End If


    '                            oda.SelectCommand.CommandText = STR & " AND isAuth=1 " & xwhr & " Order by " & DROPDOWN(2).ToString()
    '                            Dim dtFinal As New DataTable
    '                            oda.Fill(dtFinal)

    '                            Dim ddlo As DropDownList = TryCast(pnlFields.FindControl("fld" & fldPair(0).ToString()), DropDownList)

    '                            'Dim ddloo As DropDownList = New DropDownList()
    '                            'ddloo.Items.Clear()
    '                            'For Each dr As DataRow In dtFinal.Rows
    '                            '    Dim li As ListItem = New ListItem()
    '                            '    li = ddlo.Items.FindByValue(dr.Item("tid"))
    '                            '    If li Is Nothing Then
    '                            '    Else
    '                            '        ddloo.Items.Add(li)
    '                            '    End If
    '                            'Next
    '                            'ddlo.Items.Clear()
    '                            'For Each li As ListItem In ddloo.Items
    '                            '    ddlo.Items.Add(li)
    '                            'Next
    '                            'ddlo.Items.Insert(0, "SELECT")


    '                            ddlo.Items.Clear()
    '                            ddlo.Items.Add("SELECT")
    '                            ddlo.Items(0).Value = "0"
    '                            For i As Integer = 0 To dtFinal.Rows.Count - 1
    '                                ddlo.Items.Add(dtFinal.Rows(i).Item(0).ToString())
    '                                ddlo.Items(i + 1).Value = dtFinal.Rows(i).Item("tID")
    '                            Next
    '                        Else
    '                            Dim TID1 As String() = ddl.SelectedValue.ToString.Split(":")
    '                            oda = New SqlDataAdapter("", con)
    '                            oda.SelectCommand.CommandType = CommandType.StoredProcedure
    '                            oda.SelectCommand.Parameters.Clear()
    '                            oda.SelectCommand.CommandText = "uspGetMasterValue"
    '                            oda.SelectCommand.Parameters.AddWithValue("EID", HttpContext.Current.Session("EID"))
    '                            oda.SelectCommand.Parameters.AddWithValue("documentType", documenttype(1))
    '                            oda.SelectCommand.Parameters.AddWithValue("Type", documenttype(0))
    '                            oda.SelectCommand.Parameters.AddWithValue("TID", TID1(0).ToString())
    '                            oda.SelectCommand.Parameters.AddWithValue("FLDMAPPING", fldPair(1))
    '                            If con.State <> ConnectionState.Open Then
    '                                con.Open()
    '                            End If
    '                            Dim value As String = oda.SelectCommand.ExecuteScalar().ToString()
    '                            Dim TXTBOX As TextBox = TryCast(pnlFields.FindControl("fld" & fldPair(0).ToString().Trim()), TextBox)
    '                            TXTBOX.Text = value
    '                            Dim proc As String = dt.Rows(0).Item("dropdowntype").ToString()

    '                            If proc.Length > 1 Then
    '                                Dim DROPDOWN1 As String = dt.Rows(0).Item("DROPDOWN").ToString()
    '                                Dim DDL0 As DropDownList = TryCast(pnlFields.FindControl("fld" & DROPDOWN1), DropDownList)
    '                                oda.SelectCommand.CommandType = CommandType.StoredProcedure
    '                                oda.SelectCommand.Parameters.Clear()
    '                                oda.SelectCommand.CommandText = proc
    '                                oda.SelectCommand.Parameters.AddWithValue("VcNo", value)
    '                                oda.SelectCommand.Parameters.AddWithValue("fldmapping", fldPair(1))
    '                                oda.SelectCommand.Parameters.AddWithValue("FIELDID", CInt(DROPDOWN1))
    '                                oda.SelectCommand.Parameters.AddWithValue("VALUE", DDL0.SelectedValue)
    '                                Dim dss As New DataTable
    '                                oda.Fill(dss)
    '                                Dim ddl1 As DropDownList = TryCast(pnlFields.FindControl("fld" & dt.Rows(0).Item("autofilter").ToString()), DropDownList)
    '                                ddl1.Items.Clear()
    '                                ddl1.Items.Add("SELECT")
    '                                ddl1.Items(0).Value = "0"
    '                                For i As Integer = 0 To dss.Rows.Count - 1
    '                                    ddl1.Items.Add(dss.Rows(i).Item(0).ToString())
    '                                    ddl1.Items(i + 1).Value = dss.Rows(i).Item("tID")
    '                                Next
    '                            End If
    '                        End If
    '                    End If
    '                End If
    '            Next
    '        End If
    '    End If
    '    con.Dispose()
    '    oda.Dispose()
    'End Sub



    Private Sub LoadWorkGroupMenu(ByVal FLD As String, ByVal fldmapping As String, ByRef mnu As Menu, ByVal FLDTYPE As String)
        Dim mnuSelf As Menu = TryCast(mnu, Menu)
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        mnuSelf.Items.Clear()
        Dim FLDMAP As String = fldmapping
        Dim arr() As String = FLD.Split("-")
        If UCase(FLDTYPE) = "SELF REFERENCE" Then
            FLDMAP = arr(3).ToString()
        Else
            FLDMAP = fldmapping
        End If

        Dim od As New SqlDataAdapter("select convert(nvarchar(20),tid)[stid] , " & FLDMAP & "," & arr(2).ToString() & " from MMM_MST_MASTER where Eid='" & HttpContext.Current.Session("EID") & "' and documenttype='" & arr(1).ToString() & "' order by fld1", con)
        Dim ds As New DataSet
        od.Fill(ds, "boss")
        Dim dr As New DataRelation("bossrelation", ds.Tables("boss").Columns("stid"), ds.Tables("boss").Columns(FLDMAP), False)
        ds.Relations.Add(dr)
        Dim masterNode As New MenuItem("Parent", 0)
        mnuSelf.Items.Add(masterNode)
        For Each rw As DataRow In ds.Tables("boss").Rows
            If rw.IsNull(FLDMAP) Then
                Dim n As New MenuItem()
                n.Text = rw.Item(arr(2).ToString())
                n.Value = rw.Item("stid")
                masterNode.ChildItems.Add(n)
                LoadRecMenu(rw, n, arr(2).ToString())
            End If
        Next
        ds.Dispose()
        ds = Nothing
    End Sub

    Private Sub LoadRecMenu(ByVal row As DataRow, ByRef node As MenuItem, ByVal fld As String)
        For Each rw As DataRow In row.GetChildRows("bossrelation")
            Dim n As New MenuItem()
            n.Text = rw(fld).ToString()
            n.Value = rw("stid").ToString()
            node.ChildItems.Add(n)
            LoadRecMenu(rw, n, fld)
        Next
    End Sub

    Public Sub Delete(ByVal sender As Object, ByVal e As GridViewCommandEventArgs)
        Dim btnDelete As GridView = TryCast(sender, GridView)

        If e.CommandName = "Remove" Then
            Dim rw As GridViewRow = DirectCast(DirectCast(e.CommandSource, ImageButton).NamingContainer, GridViewRow)
            Dim Pid As String = btnDelete.DataKeys(rw.RowIndex).Value
            btnDelete.DeleteRow(rw.RowIndex)
            btnDelete.DataBind()
        End If


    End Sub

    Public Sub Deleting(ByVal Sender As Object, ByVal e As GridViewDeleteEventArgs)

    End Sub

    Public Function checkduplicate(ByVal qrytype As String, ByVal tid As Integer, ByVal tablename As String, ByVal fldmapping As String, ByVal value As String, ByVal doctype As String) As Boolean
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        If con.State <> ConnectionState.Open Then
            con.Open()
        End If
        Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
        Dim qry As String
        If Trim(tablename) = "MMM_MST_DOC" Then
            qry = "select count(*) from " & tablename & " where eid=" & HttpContext.Current.Session("EID") & " AND DOCUMENTTYPE='" & doctype & "' AND " & fldmapping & "='" & value & "' and curstatus <> 'REJECTED' "
        Else
            qry = "select count(*) from " & tablename & " where eid=" & HttpContext.Current.Session("EID") & " AND DOCUMENTTYPE='" & doctype & "' AND " & fldmapping & "='" & value & "'"
        End If

        Dim XWHR As String = ""

        If qrytype.ToUpper() = "UPDATE" Then
            XWHR = " AND TID<>" & tid & ""
        End If
        qry &= XWHR

        oda.SelectCommand.CommandText = qry
        Dim CNT As Integer = oda.SelectCommand.ExecuteScalar()
        If CNT > 0 Then
            Return 1
        Else
            Return 0
        End If
    End Function

    'Public Function checkduplicate(ByVal tablename As String, ByVal fldmapping As String, ByVal value As String, ByVal doctype As String) As Boolean
    '    Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
    '    Dim con As SqlConnection = New SqlConnection(conStr)
    '    If con.State <> ConnectionState.Open Then
    '        con.Open()
    '    End If
    '    Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
    '    Dim qry As String = "select count(*) from " & tablename & " where eid=" & HttpContext.Current.Session("EID") & " AND DOCUMENTTYPE='" & doctype & "' AND " & fldmapping & "='" & value & "'"
    '    oda.SelectCommand.CommandText = qry
    '    Dim CNT As Integer = oda.SelectCommand.ExecuteScalar()
    '    If CNT > 0 Then
    '        Return 1
    '    Else
    '        Return 0
    '    End If

    'End Function


    '' Hit Kicking Field

    Public Function UPDATEKICKING(ByVal KCFVALUE As String, ByVal UPDVALUE As String, ByRef PNLFIELDS As Panel) As String
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
        Dim UPDQUERY As String = ""
        KCFVALUE = KCFVALUE.Substring(0, KCFVALUE.Length - 1)
        Dim fldarr() As String = KCFVALUE.Split(",")
        For i As Integer = 0 To fldarr.Length - 1
            Dim ARR() As String = fldarr(i).Split(":")
            Dim TABLENAME As String = ""
            Dim tid As Integer = 0
            If ARR(0).ToString().ToUpper() = "MASTER" Then
                TABLENAME = "MMM_MST_MASTER"
            Else
                TABLENAME = "MMM_MST_DOC"
            End If

            Dim DDL As DropDownList = CType(PNLFIELDS.FindControl("fld" & ARR(2).ToString()), DropDownList)
            Dim ddlarr() As String = DDL.SelectedValue.Split("|")
            If ddlarr.Length > 0 Then
                tid = CInt(ddlarr(0))
            ElseIf ddlarr.Length > 1 Then
                tid = CInt(ddlarr(1))
            End If
            If ARR(4).ToString() = "R" Then
                UPDQUERY &= "UPDATE " & TABLENAME & " SET " & ARR(3) & "='" & UPDVALUE & "' WHERE DOCUMENTTYPE='" & ARR(1).ToString() & "' AND TID= " & tid & ";"
            Else
                UPDQUERY &= "UPDATE " & TABLENAME & " SET " & ARR(3) & "=ltrim(Str(convert(float," & ARR(3) & ") " & ARR(4) & " convert(float,'" & UPDVALUE & "'),21,2)) WHERE DOCUMENTTYPE='" & ARR(1).ToString() & "' AND TID= " & tid & ";"
            End If
        Next
        UPDQUERY = UPDQUERY.Substring(0, UPDQUERY.Length - 1)
        Return UPDQUERY

    End Function

    ''Function to Filter the Data according to User
    Public Function UserDataFilter(ByVal cdocumenttype As String, ByVal ddocumenttype As String) As String
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim da As New SqlDataAdapter("", con)
        Dim ds As New DataSet
        Dim fldmapping As String = ""
        Dim fldid As String = ""
        da.SelectCommand.CommandText = "select docmapping,Formname from mmm_mst_forms where eid=" & HttpContext.Current.Session("Eid") & " and Formname='" & ddocumenttype & "'"
        If con.State <> ConnectionState.Open Then
            con.Open()
        End If
        fldmapping = Convert.ToString(da.SelectCommand.ExecuteScalar())
        If fldmapping.Length > 2 Then
            da.SelectCommand.CommandText = "select " & fldmapping & ",documenttype,iscreate,isedit from mmm_ref_role_user where eid=" & HttpContext.Current.Session("eid") & " and Uid=" & HttpContext.Current.Session("uid") & " and roleNAME='" & HttpContext.Current.Session("USERROLE") & "' and '" & cdocumenttype & "' in (select * from InputString1(documenttype))"
            da.Fill(ds, "FILTER")
            If ds.Tables("FILTER").Rows.Count = 0 Then
                fldid = ""
            ElseIf ds.Tables("FILTER").Rows.Count = 1 And ds.Tables("FILTER").Rows(0).Item("iscreate").ToString() <> "0" Then
                fldid = ds.Tables("FILTER").Rows(0).Item(0).ToString()
            Else
                Dim RW() As DataRow = ds.Tables("FILTER").Select("ISCREATE=1")
                If RW.Length > 0 Then
                    fldid = RW(0).Item(0).ToString()
                Else
                    fldid = ""
                End If
                'For Each dr As DataRow In ds.Tables("FILTER").Rows
                '    If dr.Item(0).ToString() <> "*" And dr.Item("iscreate").ToString() <> "0" Then
                '        fldid = dr.Item(0).ToString()
                '    Else
                '        fldid = dr.Item("iscreate").ToString()
                '    End If
                'Next
            End If
        End If
        da.Dispose()
        con.Dispose()
        Return fldid
    End Function

    ''Safe String function to remove special character from string 
    Public Function getSafeString(ByVal strVar As String) As String
        Trim(strVar)
        strVar = Replace(strVar, "'", "")
        strVar = Replace(strVar, ";", "")
        strVar = Replace(strVar, "--", "")
        strVar = Replace(strVar, "%", "")
        strVar = Replace(strVar, "&", "")
        Return strVar
    End Function

    ''MAINTAINING HISTORY OF DOCUMENT OR MASTER
    Public Sub History(ByVal eid As Integer, ByVal docid As Integer, ByVal UID As Integer, ByVal DOCTYPE As String, ByVal Table As String, ByVal Action As String)
        Dim PK_COL As String = ""
        If Table.ToUpper = "MMM_MST_MASTER" Or Table.ToUpper = "MMM_MST_DOC" Then
            PK_COL = "TID"
        ElseIf Table.ToUpper = "MMM_MST_USER" Then
            PK_COL = "UID"
        Else
            PK_COL = "TID"
        End If
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim da As New SqlDataAdapter("", con)

        da.SelectCommand.Parameters.Clear()
        da.SelectCommand.CommandType = CommandType.StoredProcedure
        da.SelectCommand.CommandText = "USP_HISTORY"
        da.SelectCommand.Parameters.AddWithValue("EID", eid)
        da.SelectCommand.Parameters.AddWithValue("TID", docid)
        da.SelectCommand.Parameters.AddWithValue("DOCTYPE", DOCTYPE)
        da.SelectCommand.Parameters.AddWithValue("TABLENAME", Table)
        da.SelectCommand.Parameters.AddWithValue("UID", UID)
        da.SelectCommand.Parameters.AddWithValue("UACTION", Action)
        da.SelectCommand.Parameters.AddWithValue("TIDNAME", PK_COL)

        If con.State <> ConnectionState.Open Then
            con.Open()
        End If
        da.SelectCommand.ExecuteScalar()
        da.Dispose()
        con.Dispose()

    End Sub

    ''Check for Autherity Matrix 
    Public Function CheckAuth(ByVal ds As DataTable, ByVal DOCUMENTTYPE As String, ByVal EID As Integer, ByRef pnlFields As Panel) As Integer
        Dim drColl() As DataRow = ds.Select("isworkflow=1")
        Dim xwhr As String = ""
        Dim STR As String = ""
        Dim RES As Integer = 1
        If drColl.Length > 0 Then
            For i As Integer = 0 To drColl.Length - 1
                Dim FLDMAPPING As String = drColl(i).Item("FieldMapping").ToString()
                Select Case drColl(i).Item("FieldType").ToString().ToUpper()

                    Case "TEXT BOX"
                        Dim txt As TextBox = TryCast(pnlFields.FindControl("fld" & drColl(i).Item("Fieldid").ToString()), TextBox)
                        'xwhr &= FLDMAPPING & "=" & "'" & txt.Text & "'"
                        xwhr &= " AND '" & txt.Text & "' IN (SELECT * FROM DMS.InputString1(isnull(" & FLDMAPPING & ",'')))"
                    Case "DROP DOWN"
                        Dim txt As DropDownList = TryCast(pnlFields.FindControl("fld" & drColl(i).Item("Fieldid").ToString()), DropDownList)
                        xwhr &= " AND '" & txt.SelectedValue & "' IN (SELECT * FROM DMS.InputString1(isnull(" & FLDMAPPING & ",'')))"

                    Case "LOOKUP"
                        Dim txt As TextBox = TryCast(pnlFields.FindControl("fld" & drColl(i).Item("Fieldid").ToString()), TextBox)
                        xwhr &= " AND '" & txt.Text & "' IN (SELECT * FROM DMS.InputString1(isnull(" & FLDMAPPING & ",'')))"
                End Select
            Next
            STR = "SELECT COUNT(*) FROM MMM_MST_AUTHMETRIX WHERE EID=" & EID & " and doctype='" & DOCUMENTTYPE & "' " & xwhr & " "
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As SqlConnection = New SqlConnection(conStr)
            Dim da As New SqlDataAdapter("", con)
            da.SelectCommand.CommandText = STR
            If con.State <> ConnectionState.Open Then
                con.Open()
            End If

            Dim CNT As Integer = da.SelectCommand.ExecuteScalar()
            da.SelectCommand.CommandText = "SELECT COUNT(*) FROM MMM_MST_WORKFLOW_STATUS WHERE EID=" & EID & " AND DOCUMENTTYPE='" & DOCUMENTTYPE & "'"
            Dim CNT1 As Integer = da.SelectCommand.ExecuteScalar()

            If CNT = CNT1 Then
                RES = 1
            Else
                RES = 0
            End If

        End If
        Return RES
    End Function

    ''Create Control for Advance Search by ravi 
    Public Sub CreateControlsOnAdvanceSearch(ByVal ds As DataTable, ByRef pnlFields As Panel)
        Dim strcol As String = ""
        Dim strqry As String = ""
        'If ds.Rows.Count > 0 Then
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        '   Dim layout As String = ds.Rows(0).Item("LayoutType").ToString()
        pnlFields.Controls.Add(New LiteralControl("<div class=""form""><table width=""100%"" cellspacing=""5px"" border=""0px"" cellpadding=""0px"">"))
        Dim lblWidth As Integer = 130
        Dim controlWdth As Integer = 240
        Dim datatype As String = ""
        For i As Integer = 0 To ds.Rows.Count - 1
            Dim dispName As String = ds.Rows(i).Item("displayname").ToString()
            Dim lbl As New Label
            Dim lbl1 As New Label
            datatype = ds.Rows(i).Item("datatype").ToString().ToUpper()
            If datatype.ToUpper = "DATETIME" Then
                lbl.ID = "Frlbl" & ds.Rows(i).Item("FieldID").ToString()
                lbl.Text = dispName
                lbl.Font.Bold = True

                lbl1.ID = "Tolbl" & ds.Rows(i).Item("FieldID").ToString()
                lbl1.Text = dispName
                lbl1.Font.Bold = True
            ElseIf datatype.ToUpper = "NUMERIC" Then
                lbl.ID = "lblR1" & ds.Rows(i).Item("FieldID").ToString()
                lbl.Text = dispName
                lbl.Font.Bold = True

                lbl1.ID = "lblR2" & ds.Rows(i).Item("FieldID").ToString()
                lbl1.Text = dispName
                lbl1.Font.Bold = True
            Else
                lbl.ID = "lbl" & ds.Rows(i).Item("FieldID").ToString()
                lbl.Text = dispName
                lbl.Font.Bold = True
            End If

            'If layout = "DOUBLE COLUMN" Then
            If i Mod 2 = 0 Then
                pnlFields.Controls.Add(New LiteralControl("<tr>"))
            End If
            'Else
            'lblWidth = 210
            'controlWdth = 540
            'pnlFields.Controls.Add(New LiteralControl("<tr>"))
            'End If

            pnlFields.Controls.Add(New LiteralControl("<td style=""width:" & lblWidth & "px;text-align:right"">"))
            pnlFields.Controls.Add(lbl)
            pnlFields.Controls.Add(New LiteralControl("</td><td style=""width:" & controlWdth & "px;text-align:left"">"))
            Select Case ds.Rows(i).Item("FieldType").ToString().ToUpper()
                Case "TEXT BOX"
                    datatype = ds.Rows(i).Item("datatype").ToString().ToUpper()
                    Dim txtBox As New TextBox
                    Dim txtBox1 As New TextBox
                    Dim lblt As New Label
                    Dim lblf As New Label
                    If datatype.ToUpper = "DATETIME" Then
                        lblf.Text = " From "
                        pnlFields.Controls.Add(lblf)
                        txtBox.ID = "Frfld" & ds.Rows(i).Item("FieldID").ToString()
                        txtBox.Width = controlWdth - 10
                        txtBox.CssClass = "txtBox"
                        pnlFields.Controls.Add(txtBox)
                        lblt.Text = " To "
                        pnlFields.Controls.Add(lblt)
                        txtBox1.ID = "Tofld" & ds.Rows(i).Item("FieldID").ToString()
                        txtBox1.Width = controlWdth - 10
                        txtBox1.CssClass = "txtBox"
                        pnlFields.Controls.Add(txtBox1)
                        'datatype = ds.Rows(i).Item("datatype").ToString().ToUpper()
                        'Dim txtBox As New TextBox
                        'Dim txtBox1 As New TextBox
                        'If datatype.ToUpper = "DATETIME" Then
                        '    txtBox.ID = "Frfld" & ds.Rows(i).Item("FieldID").ToString()
                        '    txtBox.Width = controlWdth - 10
                        '    txtBox.CssClass = "txtBox"
                        '    pnlFields.Controls.Add(txtBox)
                        '    txtBox1.ID = "Tofld" & ds.Rows(i).Item("FieldID").ToString()
                        '    txtBox1.Width = controlWdth - 10
                        '    txtBox1.CssClass = "txtBox"
                        '    pnlFields.Controls.Add(txtBox1)
                    ElseIf datatype.ToUpper = "NUMERIC" Then
                        txtBox.ID = "fldR1" & ds.Rows(i).Item("FieldID").ToString()
                        txtBox.Width = controlWdth - 10
                        txtBox.CssClass = "txtBox"
                        pnlFields.Controls.Add(txtBox)
                        txtBox1.ID = "fldR2" & ds.Rows(i).Item("FieldID").ToString()
                        txtBox1.Width = controlWdth - 10
                        txtBox1.CssClass = "txtBox"
                        pnlFields.Controls.Add(txtBox1)
                    Else
                        txtBox.ID = "fld" & ds.Rows(i).Item("FieldID").ToString()
                        txtBox.Width = controlWdth - 10
                        txtBox.CssClass = "txtBox"
                        pnlFields.Controls.Add(txtBox)
                    End If


                    If datatype = "DATETIME" Then
                        Dim CLNDR As New AjaxControlToolkit.CalendarExtender
                        Dim CLNDR1 As New AjaxControlToolkit.CalendarExtender
                        CLNDR.ID = "FrCLNDR" & ds.Rows(i).Item("FieldID").ToString()
                        CLNDR.TargetControlID = txtBox.ID
                        CLNDR.Format = "dd/MM/yy"
                        'txtBox.Text = String.Format("{0:dd/MM/yy}", Date.Now())
                        pnlFields.Controls.Add(CLNDR)

                        CLNDR1.ID = "ToCLNDR" & ds.Rows(i).Item("FieldID").ToString()
                        CLNDR1.TargetControlID = txtBox1.ID
                        CLNDR1.Format = "dd/MM/yy"
                        'txtBox1.Text = String.Format("{0:dd/MM/yy}", Date.Now())
                        pnlFields.Controls.Add(CLNDR1)

                    End If

                Case "DROP DOWN"
                    pnlFields.Controls.Add(New LiteralControl("<div class=""form"" style=""overflow-Y:scroll;width:90%;height:100px""><h4>"))

                    Dim dynmdiv As System.Web.UI.HtmlControls.HtmlGenericControl = New System.Web.UI.HtmlControls.HtmlGenericControl("DIV")
                    dynmdiv.ID = "div" & ds.Rows(i).Item("FieldID").ToString()
                    Dim chklist As New CheckBoxList
                    chklist.ID = "chklist" & ds.Rows(i).Item("FieldID").ToString()
                    chklist.CssClass = "txtbox"
                    Dim ddlText As String = ds.Rows(i).Item("dropdown").ToString()
                    Dim dropdowntype As String = ds.Rows(i).Item("dropdowntype").ToString()
                    Dim arr() As String
                    If UCase(dropdowntype) = "FIX VALUED" Then
                        arr = ddlText.Split(",")
                        For ii As Integer = 0 To arr.Count - 1
                            chklist.Items.Add(arr(ii).ToUpper())
                        Next
                    ElseIf UCase(dropdowntype) = "SESSION VALUED" Then
                        arr = ddlText.Split("-")
                        Dim TABLENAME As String = ""
                        If UCase(arr(0).ToString()) = "MASTER" Then
                            TABLENAME = "MMM_MST_MASTER"
                        Else
                            TABLENAME = "MMM_MST_USER"
                        End If
                        Dim str As String = "select " & arr(2).ToString() & ",username from " & TABLENAME & " WHERE EID=" & HttpContext.Current.Session("EID") & " "
                        Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
                        Dim dss As New DataSet
                        oda.SelectCommand.CommandText = str
                        oda.Fill(dss, "FV")
                        For J As Integer = 0 To dss.Tables("FV").Rows.Count - 1
                            chklist.Items.Add(dss.Tables("FV").Rows(J).Item(1))
                            chklist.Items(J).Value = dss.Tables("FV").Rows(J).Item(0)
                        Next
                        oda.Dispose()
                        dss.Dispose()
                    ElseIf UCase(dropdowntype) = "MASTER VALUED" Then
                        arr = ddlText.Split("-")
                        Dim TABLENAME As String = ""
                        If UCase(arr(0).ToString()) = "MASTER" Then
                            TABLENAME = "MMM_MST_MASTER"
                        Else
                            TABLENAME = "MMM_MST_DOC"
                        End If
                        Dim str As String = "select " & arr(2).ToString() & ",tid from " & TABLENAME & " WHERE EID=" & HttpContext.Current.Session("EID") & " AND  DOCUMENTTYPE='" & arr(1).ToString() & "'"
                        Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
                        Dim dss As New DataSet
                        oda.SelectCommand.CommandText = str
                        oda.Fill(dss, "FV")
                        For J As Integer = 0 To dss.Tables("FV").Rows.Count - 1
                            chklist.Items.Add(dss.Tables("FV").Rows(J).Item(0))
                            chklist.Items(J).Value = dss.Tables("FV").Rows(J).Item(1)
                        Next
                        oda.Dispose()
                        dss.Dispose()
                    End If
                    dynmdiv.Controls.Add(chklist)
                    pnlFields.Controls.Add(dynmdiv)


                Case "CHECKBOX LIST"
                    Dim dynmdiv As System.Web.UI.HtmlControls.HtmlGenericControl = New System.Web.UI.HtmlControls.HtmlGenericControl("DIV")
                    dynmdiv.ID = "div" & ds.Rows(i).Item("FieldID").ToString()
                    Dim chklist As New CheckBoxList
                    chklist.ID = "chklist" & ds.Rows(i).Item("FieldID").ToString()
                    chklist.CssClass = "txtbox"
                    Dim ddlText As String = ds.Rows(i).Item("dropdown").ToString()
                    Dim dropdowntype As String = ds.Rows(i).Item("dropdowntype").ToString()
                    Dim arr() As String
                    If UCase(dropdowntype) = "FIX VALUED" Then
                        arr = ddlText.Split(",")
                        For ii As Integer = 0 To arr.Count - 1
                            chklist.Items.Add(arr(ii).ToUpper())
                        Next
                    ElseIf UCase(dropdowntype) = "MASTER VALUED" Then
                        arr = ddlText.Split("-")
                        Dim TABLENAME As String = ""
                        If UCase(arr(0).ToString()) = "MASTER" Then
                            TABLENAME = "MMM_MST_MASTER"
                        Else
                            TABLENAME = "MMM_MST_DOC"
                        End If
                        Dim str As String = "select " & arr(2).ToString() & ",tid from " & TABLENAME & " WHERE EID=" & HttpContext.Current.Session("EID") & " AND  DOCUMENTTYPE='" & arr(1).ToString() & "'"
                        Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
                        Dim dss As New DataSet
                        oda.SelectCommand.CommandText = str
                        oda.Fill(dss, "FV")
                        For J As Integer = 0 To dss.Tables("FV").Rows.Count - 1
                            chklist.Items.Add(dss.Tables("FV").Rows(J).Item(0))
                            chklist.Items(J).Value = dss.Tables("FV").Rows(J).Item(1)
                        Next
                        oda.Dispose()
                        dss.Dispose()
                    End If
                    dynmdiv.Controls.Add(chklist)
                    pnlFields.Controls.Add(dynmdiv)

                Case "LIST BOX"
                    Dim ddl As New ListBox
                    ddl.ID = "fld" & ds.Rows(i).Item("FieldID").ToString()
                    ddl.Width = controlWdth - 2
                    ddl.CssClass = "txtBox"
                    Dim ddlText As String = ds.Rows(i).Item("dropdown").ToString()
                    Dim dropdowntype As String = ds.Rows(i).Item("dropdowntype").ToString()
                    Dim arr() As String
                    If UCase(dropdowntype) = "FIX VALUED" Then
                        arr = ddlText.Split(",")
                        ddl.Items.Add("")
                        For ii As Integer = 0 To arr.Count - 1
                            ddl.Items.Add(arr(ii).ToUpper())
                        Next
                    ElseIf UCase(dropdowntype) = "MASTER VALUED" Then
                        arr = ddlText.Split("-")
                        Dim TABLENAME As String = ""
                        If UCase(arr(0).ToString()) = "MASTER" Then
                            TABLENAME = "MMM_MST_MASTER"
                        Else
                            TABLENAME = "MMM_MST_DOC"
                        End If
                        Dim str As String = "select " & arr(2).ToString() & ",tid from " & TABLENAME & " WHERE EID=" & HttpContext.Current.Session("EID") & " AND  DOCUMENTTYPE='" & arr(1).ToString() & "'"
                        Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
                        Dim dss As New DataSet
                        oda.SelectCommand.CommandText = str
                        oda.Fill(dss, "FV")
                        For J As Integer = 0 To dss.Tables("FV").Rows.Count - 1
                            ddl.Items.Add(dss.Tables("FV").Rows(J).Item(0))
                            ddl.Items(J).Value = dss.Tables("FV").Rows(J).Item(1)
                        Next
                        oda.Dispose()
                        dss.Dispose()
                    End If
                    ddl.Items.Insert(0, "ALL")
                    ddl.SelectionMode = ListSelectionMode.Multiple
                    pnlFields.Controls.Add(ddl)
                Case "TEXT AREA"
                    Dim txtBox As New TextBox
                    txtBox.ID = "fld" & ds.Rows(i).Item("FieldID").ToString()
                    txtBox.Width = controlWdth - 10
                    txtBox.CssClass = "txtBox"
                    txtBox.TextMode = TextBoxMode.MultiLine
                    pnlFields.Controls.Add(txtBox)
                Case "AUTO NUMBER"
                    datatype = ds.Rows(i).Item("datatype").ToString().ToUpper()
                    Dim txtBox As New TextBox
                    Dim txtBox1 As New TextBox
                    Dim lblf As New Label
                    If datatype.ToUpper = "TEXT" Then

                        pnlFields.Controls.Add(lblf)
                        txtBox.ID = "fld" & ds.Rows(i).Item("FieldID").ToString()
                        txtBox.Width = controlWdth - 10
                        txtBox.CssClass = "txtBox"
                        pnlFields.Controls.Add(txtBox)

                    End If
                Case "LOOKUP"
                    datatype = ds.Rows(i).Item("datatype").ToString().ToUpper()
                    Dim txtBox As New TextBox
                    Dim txtBox1 As New TextBox
                    Dim lblf As New Label
                    If datatype.ToUpper = "TEXT" Then

                        pnlFields.Controls.Add(lblf)
                        txtBox.ID = "fld" & ds.Rows(i).Item("FieldID").ToString()
                        txtBox.Width = controlWdth - 10
                        txtBox.CssClass = "txtBox"
                        pnlFields.Controls.Add(txtBox)

                    End If


            End Select
            pnlFields.Controls.Add(New LiteralControl("</td>"))
            'If layout = "DOUBLE COLUMN" Then
            If i Mod 2 = 1 Then
                pnlFields.Controls.Add(New LiteralControl("</tr>"))
            End If
            'Else
            'pnlFields.Controls.Add(New LiteralControl("</tr>"))
            'End If
        Next
        pnlFields.Controls.Add(New LiteralControl("</table></div>"))
        'End If
        ds.Dispose()

    End Sub

    ''Fetch the value from dynamic Control for Advance Searching by Vishal
    Function getsearchresult(ByVal ds As DataTable, ByRef pnlFields As Panel) As String
        Dim datatype As String = ""
        Dim fieldtype As String = ""
        Dim chklist As String = ""
        Dim dropdowntype As String = ""
        Dim txtobj1 As TextBox
        Dim txtobj As TextBox
        Dim chk As New CheckBoxList
        Dim str As String = ""
        For i As Integer = 0 To ds.Rows.Count - 1
            datatype = ds.Rows(i).Item("DATATYPE").ToString
            fieldtype = ds.Rows(i).Item("fieldtype").ToString
            dropdowntype = ds.Rows(i).Item("dropdowntype").ToString().ToUpper()
            If datatype.ToUpper = "DATETIME" Then
                txtobj = TryCast(pnlFields.FindControl("Frfld" & ds.Rows(i).Item("FieldID").ToString), TextBox)
                If txtobj.Text = "" Then
                Else
                    str = str & "m." & ds.Rows(i).Item("fieldmapping").ToString & ">=" & "'" & txtobj.Text.ToString & "'" & " and "
                End If
                txtobj1 = TryCast(pnlFields.FindControl("Tofld" & ds.Rows(i).Item("FieldID").ToString), TextBox)
                If txtobj1.Text = "" Then
                Else
                    str = str & "m." & ds.Rows(i).Item("fieldmapping").ToString & "<=" & "'" & txtobj1.Text.ToString & "'" & " and "
                End If
            ElseIf datatype.ToUpper = "NUMERIC" And fieldtype.ToUpper = "TEXT BOX" Then
                txtobj = TryCast(pnlFields.FindControl("fldR1" & ds.Rows(i).Item("FieldID").ToString), TextBox)
                If txtobj.Text = "" Then
                Else
                    str = str & "convert(numeric(10,2),m." & ds.Rows(i).Item("fieldmapping").ToString & ") >=" & txtobj.Text.ToString & " and "
                End If
                txtobj1 = TryCast(pnlFields.FindControl("fldR2" & ds.Rows(i).Item("FieldID").ToString), TextBox)
                If txtobj1.Text = "" Then
                Else
                    str = str & "convert(numeric(10,2),m." & ds.Rows(i).Item("fieldmapping").ToString & ") <=" & txtobj1.Text.ToString & " and "
                End If
            ElseIf datatype.ToUpper = "TEXT" And fieldtype.ToUpper = "TEXT BOX" Then
                txtobj = TryCast(pnlFields.FindControl("fld" & ds.Rows(i).Item("FieldID").ToString), TextBox)
                If txtobj.Text = "" Then
                Else
                    str = str & "m." & ds.Rows(i).Item("fieldmapping").ToString & " like " & "'%" & txtobj.Text.ToString & "%'" & " and "
                End If
            ElseIf datatype.ToUpper = "TEXT" And fieldtype.ToUpper = "AUTO NUMBER" Then
                txtobj = TryCast(pnlFields.FindControl("fld" & ds.Rows(i).Item("FieldID").ToString), TextBox)
                If txtobj.Text = "" Then
                Else
                    str = str & "m." & ds.Rows(i).Item("fieldmapping").ToString & " = " & "'" & txtobj.Text.ToString & "'" & " and "
                End If

            ElseIf datatype.ToUpper = "TEXT" And fieldtype.ToUpper = "LOOKUP" Then
                txtobj = TryCast(pnlFields.FindControl("fld" & ds.Rows(i).Item("FieldID").ToString), TextBox)
                If txtobj.Text = "" Then
                Else
                    str = str & "m." & ds.Rows(i).Item("fieldmapping").ToString & " = " & "'" & txtobj.Text.ToString & "'" & " and "
                End If



                'ElseIf datatype.ToUpper = "TEXT" And fieldtype.ToUpper = "DROP DOWN" And dropdowntype.ToUpper = "SESSION VALUED" Then
                '    txtobj = TryCast(pnlFields.FindControl("fld" & ds.Rows(i).Item("FieldID").ToString), TextBox)
                '    If txtobj.Text = "" Then
                '    Else
                '        str = str & "U." & ds.Rows(i).Item("fieldmapping").ToString & " = " & "'" & txtobj.Text.ToString & "'" & " and "
                '    End If




            ElseIf datatype.ToUpper = "TEXT" And fieldtype.ToUpper = "DROP DOWN" Then
                chk = TryCast(pnlFields.FindControl("chklist" & ds.Rows(i).Item("FieldID").ToString), CheckBoxList)
                For j As Integer = 0 To chk.Items.Count - 1
                    If chk.Items(j).Selected = True Then
                        chklist = chklist & chk.Items(j).Value & ","
                    End If
                Next
                If chklist.ToString = "" Then
                Else
                    chklist = Left(chklist, chklist.Length - 1)
                    str = str & "m." & ds.Rows(i).Item("fieldmapping").ToString & " in " & "(" & chklist & ")" & " and "
                End If
            End If

        Next
        Return str
    End Function

    ''Find control exists on the Panel or Not
    Public Shared Function GetControl(ByVal page As Panel, ByVal ctlid As String) As Boolean
        Dim control As Control = Nothing
        control = page.FindControl(ctlid)
        'Dim ctrlname As String = page.Request.Params.[Get]("__EVENTTARGET")
        'If ctrlname IsNot Nothing AndAlso ctrlname <> String.Empty Then
        '    control = page.FindControl(ctrlname)
        'Else
        '    For Each ctl As String In page.Request.Form
        '        Dim c As Control = page.FindControl(ctl)
        '        If TypeOf c Is System.Web.UI.WebControls.Button Then
        '            control = c
        '            Exit For
        '        End If
        '    Next
        'End If

        If control Is Nothing Then
            Return False
        Else
            Return True
        End If
    End Function

    ''Get MasterValued query to bind dropdown
    Public Function GetQuery(ByVal doctype As String, ByVal fld As String) As String
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim da As New SqlDataAdapter("", con)
        Dim ds As New DataSet
        Dim str As String = ""
        da.SelectCommand.CommandText = "usp_GetMasterValued"
        da.SelectCommand.CommandType = CommandType.StoredProcedure
        da.SelectCommand.Parameters.Clear()
        da.SelectCommand.Parameters.AddWithValue("@doctype", doctype)
        da.SelectCommand.Parameters.AddWithValue("@eid", HttpContext.Current.Session("eid"))
        da.SelectCommand.Parameters.AddWithValue("@fldmapping", fld)

        If con.State <> ConnectionState.Open Then
            con.Open()
        End If
        str = da.SelectCommand.ExecuteScalar()
        Return str
    End Function

    Public Function GetQuery1(ByVal doctype As String, ByVal fld As String) As String
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim da As New SqlDataAdapter("", con)
        Dim ds As New DataSet
        Dim str As String = ""
        da.SelectCommand.CommandText = "usp_GetMasterValued1"
        da.SelectCommand.CommandType = CommandType.StoredProcedure
        da.SelectCommand.Parameters.Clear()
        da.SelectCommand.Parameters.AddWithValue("@doctype", doctype)
        da.SelectCommand.Parameters.AddWithValue("@eid", HttpContext.Current.Session("eid"))
        da.SelectCommand.Parameters.AddWithValue("@fldmapping", fld)

        If con.State <> ConnectionState.Open Then
            con.Open()
        End If
        str = da.SelectCommand.ExecuteScalar()
        Return str
    End Function

End Class












