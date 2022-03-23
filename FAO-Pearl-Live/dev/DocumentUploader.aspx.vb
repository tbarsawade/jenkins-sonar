Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports System.Globalization
Imports System.Drawing
Imports System.Data.OleDb
Imports System.Xml

Partial Class DocumentUploader
    Inherits System.Web.UI.Page
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            If Not Page.IsPostBack Then
                'BindDropDown(ddlSource.SelectedItem.Text.Trim())
            End If
        Catch ex As Exception
        End Try
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
    Public Sub BindDropDown(ByVal source As String)
        Dim Eid As Integer = 0
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim li As New ListItem("--Select Document--", 0)
        Try
            Eid = Convert.ToInt32(Session("eid"))
            ' Dim MQuerry = "SELECT MID,MenuName,PageLink FROM MMM_MST_MENU WHERE MID NOT IN (SELECT DISTINCT PMENU FROM MMM_MST_MENU WHERE PMENU IS NOT NULL and EID=" & Session("EID") & ") and eid=" & Session("EID") & " AND (PageLink like '" & source.ToString() & "s.Aspx?%' or pagelink like 'usermaster.aspx%' ) and roles like '%{" & Session("USERROLE") & "%" & "'"
            Dim MQuerry = "SELECT MID,MenuName,PageLink FROM MMM_MST_MENU WHERE MID NOT IN (SELECT DISTINCT PMENU FROM MMM_MST_MENU WHERE PMENU IS NOT NULL and EID=" & Session("EID") & ") and eid=" & Session("EID") & " AND (PageLink like '" & source.ToString() & "s.Aspx?%' ) and roles like '%{" & Session("USERROLE") & "%" & "'"
            Dim dsMenu As New DataSet()
            Using con1 = New SqlConnection(conStr)
                Using da1 = New SqlDataAdapter(MQuerry, con)
                    da1.Fill(dsMenu)
                End Using
            End Using
            Dim Document As New StringBuilder()
            Dim StrDocument = ""
            If dsMenu.Tables(0).Rows.Count > 0 Then
                For i As Integer = 0 To dsMenu.Tables(0).Rows.Count - 1
                    If (dsMenu.Tables(0).Rows(i).Item("PageLink").ToString.Contains("=")) Then
                        Dim arrPage = dsMenu.Tables(0).Rows(i).Item("PageLink").ToString.Split("=")
                        Document.Append(",").Append("'").Append(arrPage(1)).Append("'")
                    End If
                Next
                '   Document.Append(",").Append("'").Append("User").Append("'")
                StrDocument = Document.ToString.Substring(1, Document.ToString.Length - 1)
            End If

            Dim da As New SqlDataAdapter("select formid,formname from MMM_MST_FORMS where EID=" & Eid & " and FormType='" & source.ToString() & "' and IsActive=1 AND FormSource='MENU DRIVEN' and FormName in (" & StrDocument & ") and isnull(showuploader,0)=1", con)
            Dim ds As New DataSet
            da.Fill(ds, "FormName")
            If ds.Tables("FormName").Rows.Count > 0 Then
                ddlFormtype.DataSource = ds.Tables("FormName")
                ddlFormtype.DataValueField = "FormName"
                ddlFormtype.DataTextField = "FormName"
                ddlFormtype.DataBind()
                ddlFormtype.Items.Insert(0, li)
            Else
                If (ddlFormtype.Items.Count > 0) Then
                    ddlFormtype.Items.Clear()
                    ddlFormtype.Items.Insert(0, li)
                    ddlFormtype.DataBind()
                End If
            End If
            con.Close()
            con.Dispose()
        Catch ex As Exception
            ddlFormtype.Items.Insert(0, li)
            con.Close()
            con.Dispose()
        End Try
    End Sub

    Protected Sub imgCSVSample_Click(sender As Object, e As System.Web.UI.ImageClickEventArgs) Handles imgCSVSample.Click
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Try
            Dim scrname As String = ddlFormtype.SelectedItem.Text
            If ddlSource.SelectedIndex = 2 And ddlOperation.SelectedIndex = 1 Then
                Dim das As New SqlDataAdapter("", con)
                das.SelectCommand.CommandText = "select COUNT(displayName) from MMM_MST_FIELDS where DocumentType ='" & scrname & "' and eid=" & Session("EID").ToString() & " and isunique=1 "
                If con.State = ConnectionState.Closed Then
                    con.Open()
                End If
                If Convert.ToInt32(das.SelectCommand.ExecuteScalar()) = 0 Then
                    lblMessage.Text = "Please configure unique key against " & scrname
                    Exit Sub
                End If
            End If
            Response.Clear()
            Response.Buffer = True


            Response.Charset = ""
            Response.ContentType = "application/vnd.ms-excel"

            'fill Product
            Dim ds As New DataSet

            Dim filename As String = scrname
            filename = filename.Replace(" ", "_")
            Response.AddHeader("content-disposition", "attachment;filename=" & filename & ".xls")
            Dim da As New SqlDataAdapter("", con)
            If ddlSource.SelectedIndex = 2 And ddlOperation.SelectedIndex = 1 Then
                da.SelectCommand.CommandType = CommandType.StoredProcedure
                da.SelectCommand.CommandText = "GetKeys000001"
                da.SelectCommand.Parameters.AddWithValue("@EID", Session("EID"))
                da.SelectCommand.Parameters.AddWithValue("@DocumentType", scrname.ToString())
            Else
                da.SelectCommand.CommandText = "SELECT  displayName[Display Name],case isRequired when 0 then 'No' when 1 then 'Yes' end [Mandatory Fields],case datatype when 'text' then 'Text' when 'numeric' then 'Numeric Digits' when 'Datetime' then 'Date in (DD/MM/YY)' end [Data Type],case MinLen when 0 then '' else MinLen end [Minimum Length], case MaxLen when 0 then '' else maxlen end [Maximum Length] FROM MMM_MST_FIELDS where EID =" & Session("EID").ToString() & " and DocumentType ='" & scrname & "' AND ISACTIVE=1 and  FieldType <>'Formula Field' and FieldType <> 'Calculative Field' AND  IsEditable=1 and invisible=0 order by displayOrder "
            End If
            da.Fill(ds, "data")
            Dim query As String = ""
            If ddlSource.SelectedIndex = 2 And ddlOperation.SelectedIndex = 1 Then
                query = "Create Table #Temp(keys varchar(15))	DECLARE @Keys AS VARCHAR(100)=''	SET @Keys=(select UniqueKeys FROM MMM_MST_FORMS where EID=" & Session("EID") & " AND FormName='" & scrname.ToString() & "')	INSERT INTO #Temp(keys) SELECT  items FROM [dbo].[Split] (@Keys, ',') 	SELECT F.DisplayName as displayName,case F.isRequired when 0 then 'No' when 1 then 'Yes' end [Mandatory Fields],case F.datatype when 'text' then 'Text' when 'numeric' then 'Numeric Digits' when 'Datetime' then 'Date in (DD/MM/YY)' end [Data Type] ,case F.MinLen when 0 then '' else MinLen end [Minimum Length],case F.MaxLen when 0 then '' else maxlen end [Maximum Length]  FROM  MMM_MST_FIELDS F	INNER JOIN MMM_MST_FORMS M ON M.FormName=F.DocumentType 	WHERE F.EID=" & Session("EID") & " AND DocumentType='" & scrname.ToString() & "' AND FieldMapping in(SELECT Keys FROM #Temp) AND M.EID=" & Session("EID") & "                union all	SELECT displayName,case isRequired when 0 then 'No' when 1 then 'Yes' end [Mandatory Fields],case datatype when 'text' then 'Text' when 'numeric' then 'Numeric Digits' when 'Datetime' then 'Date in (DD/MM/YY)' end [Data Type],case MinLen when 0 then '' else MinLen end [Minimum Length], case MaxLen when 0 then '' else maxlen end [Maximum Length] FROM MMM_MST_FIELDS where EID =" & Session("EID") & " 	and DocumentType ='" & scrname.ToString() & "' AND ISACTIVE=1  and  FieldType <>'Formula Field' and FieldType <> 'Calculative Field' AND  IsEditable=1 and invisible=0 and  enableedit=1  	DROP TABLE #Temp  "
            Else
                If scrname.ToUpper = "USER" Then
                    query &= "Select 'USERNAME' [displayname],'Yes' [Mandatory Fields],'Text' [Data Type],'' [Minimum Length], '' [Maximum Length]   union All "
                    query &= "Select 'USERROLE' [displayname],'Yes' [Mandatory Fields],'Text' [Data Type],'' [Minimum Length], '' [Maximum Length]  union All "
                    query &= "Select 'EMAILID' [displayname],'Yes' [Mandatory Fields],'Text' [Data Type],'' [Minimum Length], '' [Maximum Length] union All   "
                End If
                query &= "SELECT displayName,case isRequired when 0 then 'No' when 1 then 'Yes' end [Mandatory Fields],case datatype when 'text' then 'Text' when 'numeric' then 'Numeric Digits' when 'Datetime' then 'Date in (DD/MM/YY)' end [Data Type],case MinLen when 0 then '' else MinLen end [Minimum Length], case MaxLen when 0 then '' else maxlen end [Maximum Length] FROM MMM_MST_FIELDS where EID =" & Session("EID").ToString() & " and DocumentType ='" & scrname & "' AND ISACTIVE=1  and  FieldType <>'Formula Field' and FieldType <> 'Calculative Field' AND  IsEditable=1 and invisible=0  "
            End If
            con.Dispose()
            Dim conStr1 As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con1 As New SqlConnection(conStr1)
            Dim cmd As SqlCommand = New SqlCommand(query, con1)
            con1.Open()
            Dim dr As SqlDataReader = cmd.ExecuteReader(CommandBehavior.CloseConnection)
            Dim dt As DataTable = New DataTable()
            Dim dt2 As DataTable = New DataTable()
            Dim dt3 As DataTable = New DataTable()

            dt.Load(dr)

            dt3 = ds.Tables("data")
            dt2 = GetInversedDataTable(dt, "displayname", "")

            If scrname.ToUpper = "USER" Then
                AddUserStaticField(dt3)
            End If

            Dim gvex As New GridView()
            dt2.Rows.Add()

            gvex.AllowPaging = False
            gvex.DataSource = dt2
            gvex.DataBind()
            Dim gvexx As New GridView()
            gvexx.AllowPaging = False
            gvexx.DataSource = dt3

            gvexx.DataBind()
            Response.Clear()
            Response.Buffer = True
            Dim sw As New StringWriter()
            Dim hw As New HtmlTextWriter(sw)

            Dim tb As New Table()
            Dim tr1 As New TableRow()
            Dim cell1 As New TableCell()
            cell1.Controls.Add(gvex)
            tr1.Cells.Add(cell1)
            Dim cell3 As New TableCell()
            cell3.Controls.Add(gvexx)
            Dim cell2 As New TableCell()
            cell2.Text = "&nbsp;"

            Dim tr2 As New TableRow()
            tr2.Cells.Add(cell2)
            Dim tr3 As New TableRow()
            tr3.Cells.Add(cell3)
            tb.Rows.Add(tr1)
            tb.Rows.Add(tr2)
            tb.Rows.Add(tr3)
            tb.RenderControl(hw)
            'style to format numbers to string 
            Dim style As String = "<style> .textmode { mso-number-format:\@; } </style>"
            Response.Write(style)
            Response.Output.Write(sw.ToString())
            Response.Flush()
            Response.[End]()
            con.Close()
            con.Dispose()
        Catch ex As Exception
            con.Close()
            con.Dispose()
        End Try
    End Sub


    Public Shared Function AddUserStaticField(ByRef dt As DataTable) As DataTable
        dt.Rows.Add()
        dt.Rows(dt.Rows.Count - 1).Item("Display name") = "USERNAME"
        dt.Rows(dt.Rows.Count - 1).Item("Mandatory Fields") = "Yes"
        dt.Rows(dt.Rows.Count - 1).Item("Data Type") = "Text"
        dt.Rows(dt.Rows.Count - 1).Item("Minimum Length") = "0"
        dt.Rows(dt.Rows.Count - 1).Item("Maximum Length") = "0"
        'Text
        dt.Rows.Add()
        dt.Rows(dt.Rows.Count - 1).Item("Display name") = "USERROLE"
        dt.Rows(dt.Rows.Count - 1).Item("Mandatory Fields") = "Yes"
        dt.Rows(dt.Rows.Count - 1).Item("Data Type") = "Text"
        dt.Rows(dt.Rows.Count - 1).Item("Minimum Length") = "0"
        dt.Rows(dt.Rows.Count - 1).Item("Maximum Length") = "0"
        'EnableEdit
        dt.Rows.Add()
        dt.Rows(dt.Rows.Count - 1).Item("Display name") = "EMAILID"
        dt.Rows(dt.Rows.Count - 1).Item("Mandatory Fields") = "Yes"
        dt.Rows(dt.Rows.Count - 1).Item("Data Type") = "Text"
        dt.Rows(dt.Rows.Count - 1).Item("Minimum Length") = "0"
        dt.Rows(dt.Rows.Count - 1).Item("Maximum Length") = "0"
        Return dt
    End Function

    Function GetInversedDataTable(ByVal table As DataTable, ByVal columnX As String, ByVal nullValue As String) As DataTable
        Dim returnTable As New DataTable()
        'returnTable.Columns.Add("DocID")
        If columnX = "" Then
            columnX = table.Columns(0).ColumnName
        End If

        Dim columnXValues As New List(Of String)()

        For Each dr As DataRow In table.Rows
            Dim columnXTemp As String = dr(columnX).ToString()
            If Not columnXValues.Contains(columnXTemp) Then
                columnXValues.Add(columnXTemp)
                returnTable.Columns.Add(columnXTemp)
            End If
        Next
        If nullValue <> "" Then
            For Each dr As DataRow In returnTable.Rows
                For Each dc As DataColumn In returnTable.Columns
                    If dr(dc.ColumnName).ToString() = "" Then
                        dr(dc.ColumnName) = nullValue
                    End If
                Next
            Next
        End If
        Return returnTable

    End Function

    'Protected Sub btnSubmit_Click(sender As Object, e As EventArgs) Handles btnSubmit.Click
    '    Dim SB As StringBuilder = Nothing
    '    Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
    '    Try
    '        Dim EID As Integer
    '        Dim DocType As String = ""
    '        Dim UID As Integer
    '        EID = Convert.ToInt32(Session("eid"))
    '        UID = Convert.ToInt32(Session("UID"))
    '        DocType = ddlFormtype.SelectedItem.Text
    '        If CSVUploader.HasFile Then
    '            If Right(CSVUploader.FileName, 4).ToUpper() = ".CSV" Then
    '                Dim filename As String = "COLL" & Now.Day & Now.Month & Now.Year & Now.Hour & Now.Minute & Now.Minute & Now.Second & Now.Millisecond & Right(CSVUploader.PostedFile.FileName, 4).ToUpper()
    '                CSVUploader.PostedFile.SaveAs(Server.MapPath("Import/") & filename)
    '                'lblMessage.Text = ex.Message & "<br/>" & "File Name IS " & filename
    '                'converting csv file into datatable
    '                Dim dtData As New DataTable
    '                dtData = GetDataFromExcel(filename)
    '                Dim Query = "select DisplayName,fieldType,datatype FROM MMM_MST_Fields where EID=" & EID & " and datatype = 'Datetime' and ISeditable=0 and IsActive=1 and documentType= '" & DocType & "'"
    '                Dim ds As New DataSet()
    '                'Code For populating non editable field with current date
    '                Using con = New SqlConnection(conStr)
    '                    Using da = New SqlDataAdapter(Query, con)
    '                        da.Fill(ds)
    '                    End Using
    '                End Using
    '                If ds.Tables(0).Rows.Count > 0 Then
    '                    Dim Result As String = ""
    '                    Dim dt As Date = Date.Today
    '                    Result = dt.ToString("dd/MM/yy", CultureInfo.InvariantCulture)
    '                    For a As Integer = 0 To ds.Tables(0).Rows.Count - 1
    '                        dtData.Columns.Add(ds.Tables(0).Rows(a).Item("DisplayName"))
    '                        For b As Integer = 0 To dtData.Rows.Count - 1
    '                            dtData.Rows(b).Item(ds.Tables(0).Rows(a).Item("DisplayName")) = Result
    '                        Next
    '                    Next
    '                End If
    '                'Done with non editable field calculation
    '                dtData.Columns.Add("Service_response")
    '                If dtData.Rows.Count > 0 Then
    '                    Dim SuccessCount As Integer = 0
    '                    Dim FailCount As Integer = 0
    '                    For i As Integer = 0 To dtData.Rows.Count - 1
    '                        SB = New StringBuilder()
    '                        For Each column As DataColumn In dtData.Columns
    '                            'Creating String from data table
    '                            SB.Append("|").Append(column.ColumnName.Trim()).Append("::").Append(dtData.Rows(i).Item(column.ColumnName).ToString.Trim)
    '                        Next
    '                        Dim StrData = SB.ToString()
    '                        Dim Result = CommanUtil.ValidateParameterByDocumentType(EID, DocType, UID, StrData)
    '                        Result = Result.Replace("Error(s) in document " & DocType & ".", String.Empty)
    '                        If Result.Contains("Your DocID is") Then
    '                            SuccessCount = SuccessCount + 1
    '                        Else
    '                            FailCount = FailCount + 1
    '                        End If
    '                        dtData.Rows(i).Item("Service_response") = Result
    '                    Next
    '                    lblSuccessDocument.Text = "<b>" & SuccessCount & "</b>"
    '                    lblTotalDocument.Text = "<b>" & dtData.Rows.Count & "</b>"
    '                    lblFailedDocument.Text = "<b>" & FailCount & "</b>"
    '                    gvData.DataSource = dtData
    '                    gvData.DataBind()
    '                    pnlMessage.Visible = True
    '                Else
    '                    pnlMessage.Visible = False
    '                    lblSuccessDocument.Text = "<b>0</b>"
    '                    lblTotalDocument.Text = "<b>0</b>"
    '                    lblFailedDocument.Text = "<b>0</b>"
    '                    gvData.DataSource = Nothing
    '                    gvData.DataBind()
    '                    lblMessage.Text = "No Any records found to upload."
    '                End If
    '            End If
    '        End If
    '    Catch ex As Exception
    '        lblMessage.Text = "Sorry!!! Your request can not completed at the moment. Try again later.<br/>" & ex.Message
    '    End Try
    'End Sub

    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs) Handles btnSubmit.Click
        Dim SB As StringBuilder = Nothing
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Try
            Dim EID As Integer
            Dim DocType As String = ""
            Dim UID As Integer
            Dim dterrorResult As New DataTable
            EID = Convert.ToInt32(Session("eid"))
            UID = Convert.ToInt32(Session("UID"))
            DocType = ddlFormtype.SelectedItem.Text
            If CSVUploader.HasFile Then
                If hdnFileType.Value = "BOTH" Then
                    If Right(CSVUploader.FileName, 4).ToUpper() = ".CSV" Then
                        Dim filename As String = "COLL" & Now.Day & Now.Month & Now.Year & Now.Hour & Now.Minute & Now.Minute & Now.Second & Now.Millisecond & Right(CSVUploader.PostedFile.FileName, 4).ToUpper()
                        CSVUploader.PostedFile.SaveAs(Server.MapPath("Import/") & filename)
                        'lblMessage.Text = ex.Message & "<br/>" & "File Name IS " & filename
                        'converting csv file into datatable
                        Dim dtData As New DataTable
                        dtData = GetDataFromExcel(filename, DocType)
                        Dim Query = "select DisplayName,fieldType,datatype FROM MMM_MST_Fields where EID=" & EID & " and datatype = 'Datetime' and ISeditable=0 and IsActive=1 and documentType= '" & DocType & "'"
                        Dim ds As New DataSet()
                        'Code For populating non editable field with current date
                        Using con = New SqlConnection(conStr)
                            Using da = New SqlDataAdapter(Query, con)
                                da.Fill(ds)
                            End Using
                        End Using
                        If ds.Tables(0).Rows.Count > 0 Then
                            Dim Result As String = ""
                            Dim dt As Date = Date.Today
                            Result = dt.ToString("dd/MM/yy", CultureInfo.InvariantCulture)
                            For a As Integer = 0 To ds.Tables(0).Rows.Count - 1
                                dtData.Columns.Add(ds.Tables(0).Rows(a).Item("DisplayName"))
                                For b As Integer = 0 To dtData.Rows.Count - 1
                                    dtData.Rows(b).Item(ds.Tables(0).Rows(a).Item("DisplayName")) = Result
                                Next
                            Next
                        End If
                        'Done with non editable field calculation
                        dtData.Columns.Add("Service_response")
                        dterrorResult = dtData.Clone()
                        If dtData.Rows.Count > 0 Then
                            Dim SuccessCount As Integer = 0
                            Dim FailCount As Integer = 0
                            For i As Integer = 0 To dtData.Rows.Count - 1
                                SB = New StringBuilder()
                                For Each column As DataColumn In dtData.Columns
                                    'Creating String from data table
                                    SB.Append("|").Append(column.ColumnName.Trim()).Append("::").Append(dtData.Rows(i).Item(column.ColumnName).ToString.Trim)
                                Next
                                Dim StrData = SB.ToString()
                                Dim Result = ""
                                'First condition for Edit operration in Master case only
                                ' prev
                                'If ddlSource.SelectedIndex = 2 And ddlOperation.SelectedIndex = 1 Then
                                '    Dim update As New UpdateData
                                '    Result = update.UpdateData(EID, DocType, UID, StrData, "EnableEdit")
                                'Else
                                '    Result = CommanUtil.ValidateParameterByDocumentType(EID, DocType, UID, StrData)
                                'End If

                                Dim update As New UpdateData
                                If ddlOperation.SelectedIndex = 1 Then
                                    Result = update.UpdateData(EID, DocType, UID, StrData, "EnableEdit")
                                Else
                                    Result = CommanUtil.ValidateParameterByDocumentType(EID, DocType, UID, StrData)
                                End If


                                If Result.Contains("Error(s) in document ") Then
                                    GetRandomRows(dtData, i, dterrorResult, Result)
                                End If

                                Result = Result.Replace("Error(s) in document " & DocType & ".", String.Empty)
                                Dim strPatt As String = "[|\d-]|"
                                Result = Regex.Replace(Result, strPatt, String.Empty)
                                If Result.Contains("Your DocID is") Then
                                    SuccessCount = SuccessCount + 1
                                Else
                                    FailCount = FailCount + 1
                                End If
                                dtData.Rows(i).Item("Service_response") = Result
                            Next
                            lblSuccessDocument.Text = "<b>" & SuccessCount & "</b>"
                            lblTotalDocument.Text = "<b>" & dtData.Rows.Count & "</b>"
                            lblFailedDocument.Text = "<b>" & FailCount & "</b>"
                            gvData.DataSource = dtData
                            gvData.DataBind()
                            pnlMessage.Visible = True
                            If dterrorResult.Rows.Count > 0 Then
                                ViewState("ERRORDATA") = dterrorResult
                            Else
                                ViewState("ERRORDATA") = Nothing
                            End If
                        Else
                            pnlMessage.Visible = False
                            lblSuccessDocument.Text = "<b>0</b>"
                            lblTotalDocument.Text = "<b>0</b>"
                            lblFailedDocument.Text = "<b>0</b>"
                            gvData.DataSource = Nothing
                            gvData.DataBind()
                            lblMessage.Text = "No Any records found to upload."
                        End If
                    ElseIf Right(CSVUploader.FileName, 4).ToUpper() = ".XML" Then
                        Dim filename As String = "COLL" & Now.Day & Now.Month & Now.Year & Now.Hour & Now.Minute & Now.Minute & Now.Second & Now.Millisecond & Right(CSVUploader.PostedFile.FileName, 4).ToUpper()
                        CSVUploader.PostedFile.SaveAs(Server.MapPath("Import/") & filename)
                        Dim dtData As New DataTable
                        GetDataFromXml(filename)
                    End If
                ElseIf hdnFileType.Value = "XML" Then
                    If Right(CSVUploader.FileName, 4).ToUpper() = ".XML" Then
                        Dim filename As String = "COLL" & Now.Day & Now.Month & Now.Year & Now.Hour & Now.Minute & Now.Minute & Now.Second & Now.Millisecond & Right(CSVUploader.PostedFile.FileName, 4).ToUpper()
                        CSVUploader.PostedFile.SaveAs(Server.MapPath("Import/") & filename)
                        Dim dtData As New DataTable
                        GetDataFromXml(filename)
                    Else
                        lblMessage.Text = "You can upload only XML file against this documnet"
                    End If
                ElseIf hdnFileType.Value = "CSV" Then
                    If Right(CSVUploader.FileName, 4).ToUpper() = ".CSV" Then
                        Dim filename As String = "COLL" & Now.Day & Now.Month & Now.Year & Now.Hour & Now.Minute & Now.Minute & Now.Second & Now.Millisecond & Right(CSVUploader.PostedFile.FileName, 4).ToUpper()
                        CSVUploader.PostedFile.SaveAs(Server.MapPath("Import/") & filename)
                        'lblMessage.Text = ex.Message & "<br/>" & "File Name IS " & filename
                        'converting csv file into datatable
                        Dim dtData As New DataTable
                        dtData = GetDataFromExcel(filename, DocType)
                        Dim Query = "select DisplayName,fieldType,datatype FROM MMM_MST_Fields where EID=" & EID & " and datatype = 'Datetime' and ISeditable=0 and IsActive=1 and documentType= '" & DocType & "'"
                        Dim ds As New DataSet()
                        'Code For populating non editable field with current date
                        Using con = New SqlConnection(conStr)
                            Using da = New SqlDataAdapter(Query, con)
                                da.Fill(ds)
                            End Using
                        End Using
                        If ds.Tables(0).Rows.Count > 0 Then
                            Dim Result As String = ""
                            Dim dt As Date = Date.Today
                            Result = dt.ToString("dd/MM/yy", CultureInfo.InvariantCulture)
                            For a As Integer = 0 To ds.Tables(0).Rows.Count - 1
                                dtData.Columns.Add(ds.Tables(0).Rows(a).Item("DisplayName"))
                                For b As Integer = 0 To dtData.Rows.Count - 1
                                    dtData.Rows(b).Item(ds.Tables(0).Rows(a).Item("DisplayName")) = Result
                                Next
                            Next
                        End If
                        'Done with non editable field calculation
                        dtData.Columns.Add("Service_response")
                        dterrorResult = dtData.Clone()
                        If dtData.Rows.Count > 0 Then
                            Dim SuccessCount As Integer = 0
                            Dim FailCount As Integer = 0
                            For i As Integer = 0 To dtData.Rows.Count - 1
                                SB = New StringBuilder()
                                For Each column As DataColumn In dtData.Columns
                                    'Creating String from data table
                                    SB.Append("|").Append(column.ColumnName.Trim()).Append("::").Append(dtData.Rows(i).Item(column.ColumnName).ToString.Trim)
                                Next
                                Dim StrData = SB.ToString()
                                'Dim Result = CommanUtil.ValidateParameterByDocumentType(EID, DocType, UID, StrData)
                                Dim Result = ""
                                If ddlSource.SelectedIndex <> 0 And ddlOperation.SelectedIndex = 1 Then
                                    Dim update As New UpdateData
                                    Result = update.UpdateData(EID, DocType, UID, StrData, "EnableEdit")
                                Else
                                    Result = CommanUtil.ValidateParameterByDocumentType(EID, DocType, UID, StrData)
                                End If
                                If Result.Contains("Error(s) in document ") Then
                                    GetRandomRows(dtData, i, dterrorResult, Result)
                                End If
                                Dim SuccessMsg = Result
                                Result = Result.Replace("Error(s) in document " & DocType & ".", String.Empty)
                                Dim strPatt As String = "[|\d-]|"
                                Result = Regex.Replace(Result, strPatt, String.Empty)
                                If Result.Contains("Your DocID is") Then
                                    SuccessCount = SuccessCount + 1
                                    Result = SuccessMsg
                                ElseIf Result.Contains("Record updated successfully.") Then
                                    SuccessCount = SuccessCount + 1
                                Else
                                    FailCount = FailCount + 1
                                End If
                                dtData.Rows(i).Item("Service_response") = Result
                            Next
                            lblSuccessDocument.Text = "<b>" & SuccessCount & "</b>"
                            lblTotalDocument.Text = "<b>" & dtData.Rows.Count & "</b>"
                            lblFailedDocument.Text = "<b>" & FailCount & "</b>"
                            gvData.DataSource = dtData
                            gvData.DataBind()
                            pnlMessage.Visible = True
                            If dterrorResult.Rows.Count > 0 Then
                                ViewState("ERRORDATA") = dterrorResult
                                'Dim strw = dterrorResult.Rows(2).Item("Service_response").ToString()
                            Else
                                ViewState("ERRORDATA") = Nothing
                            End If
                        Else
                            pnlMessage.Visible = False
                            lblSuccessDocument.Text = "<b>0</b>"
                            lblTotalDocument.Text = "<b>0</b>"
                            lblFailedDocument.Text = "<b>0</b>"
                            gvData.DataSource = Nothing
                            gvData.DataBind()
                            lblMessage.Text = "No Any records found to upload."
                        End If
                    Else
                        lblMessage.Text = "You can upload only CSV file against this document"
                    End If
                End If
            End If
        Catch ex As Exception
            'lblMessage.Text = "Sorry!!! Your request can not completed at the moment. Try again later.<br/>" & ex.Message
            If ex.Message.Contains("Input array is longer than the") Then
                lblMessage.Text = "File Header format are not matched in particular line <br/> " & HttpContext.Current.Session("dataerr")

                'lblMessage.Text = "Issue with File format.<br/>Fields seperator configured is (,) Comma. And one or more of field{s) value contains Comma<br/>So Either remove comma or change the seperator to PIPE (|) "
            ElseIf ex.Message.Contains("does not belong to table") Then
                lblMessage.Text = "Issue with File format.<br/>Fields seperator used in file does not matching with configured in System for This Document type<br/>For Ex. - Seperator configured is PIPE (|) but in File seperator is Comma (,)"
            Else
                lblMessage.Text = "There is some unidentified issue with File Format/Field Seperator.<br/>Please Contact Admin with File to troubleshoot issue.<br/> System Error is - " & ex.Message
            End If
        End Try
    End Sub
    Private Function GetRandomRows(ByVal table As DataTable, ByVal rowCount As Integer, ByVal restable As DataTable, ByVal str As String) As DataTable
        Dim selector As New Random
        'Dim selections As DataTable = table.Clone()
        Dim row As DataRow
        row = table.Rows(rowCount)
        row.Item("Service_response") = str
        restable.ImportRow(row)

        'For count As Integer = 1 To Math.Min(rowCount, table.Rows.Count)
        '    row = table.Rows(selector.Next(0, table.Rows.Count))
        '    row.Item("Service_response") = str
        '    restable.ImportRow(row)
        '    'table.Rows.Remove(row)
        'Next

        Return restable
    End Function
    Protected Sub gv_OnRowCreated(sender As Object, e As GridViewRowEventArgs)

        If e.Row.RowType = DataControlRowType.DataRow Then
            If e.Row.RowState = DataControlRowState.Alternate Then
                e.Row.Attributes.Add("onmouseover", "this.style.backgroundColor='#41b6ee';")
                e.Row.Attributes.Add("onmouseout", "this.style.backgroundColor='#23b4f9';")
            Else
                e.Row.Attributes.Add("onmouseover", "this.style.backgroundColor='#41b6ee';")
                e.Row.Attributes.Add("onmouseout", "this.style.backgroundColor='#23b4f9';")
            End If
        End If
    End Sub
    'This function will obsolete now  
    Public Function GetDataFromExcel(ByVal strDataFilePath As String, Optional ByVal DocumentType As String = Nothing) As DataTable
        ' GetDataFromExcel123(strDataFilePath)
        HttpContext.Current.Session("dataerr") = Nothing
        Dim finalLine As String = ""
        Try
            Dim Seperator As String = ","
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            If Not DocumentType Is Nothing Then
                Using con As New SqlConnection(conStr)
                    Using da As New SqlDataAdapter("select FieldSeperator from mmm_mst_forms where eid=" & Session("EID") & " and formname='" & DocumentType & "'", con)
                        Dim dtForm As New DataSet()
                        da.Fill(dtForm)
                        Select Case dtForm.Tables(0).Rows(0).Item(0).ToString().ToUpper().Trim()
                            Case "COMMA"
                            Case ""
                                Seperator = ","
                            Case "PIPE"
                                Seperator = "|"
                        End Select
                    End Using
                End Using
            End If
            'Dim sr As New StreamReader(Server.MapPath("~/Import/" & strDataFilePath))
            Dim sr As New StreamReader(Server.MapPath("~/Import/" & strDataFilePath), System.Text.Encoding.GetEncoding("iso-8859-1"))
            Dim FileName = Server.MapPath("~/Import/" & strDataFilePath)
            Dim fullFileStr As String = sr.ReadToEnd()
            fullFileStr = fullFileStr.Replace("'=", "=").Replace("'-", "-").Replace("'+", "+").Replace("  ", " - ")
            sr.Close()
            sr.Dispose()
            Dim lines As String() = fullFileStr.Split(ControlChars.Lf)
            Dim recs As New DataTable()
            Dim sArr As String() = lines(0).Split(Seperator)
            For Each s As String In sArr
                recs.Columns.Add(New DataColumn(s.Trim()))
            Next
            Dim row As DataRow
            Dim i As Integer = 0
            'Dim objD As New DynamicForm()
            For Each line As String In lines
                If i > 0 And Not String.IsNullOrEmpty(line.Trim()) Then
                    row = recs.NewRow()
                    finalLine = getSafeString_uploader(line.Replace(Convert.ToString(ControlChars.Cr), ""))
                    ''Conversion due to Pen testing data
                    'finalLine = finalLine.Replace("'=", "=").Replace("'-", "-").Replace("'+", "+")
                    row.ItemArray = finalLine.Split(Seperator)
                    recs.Rows.Add(row)
                End If
                i = i + 1
            Next
            Return recs
        Catch ex As Exception
            HttpContext.Current.Session("dataerr") = finalLine
            Throw ex
        End Try
    End Function



    Public Function GetDataFromExcel12345(ByVal filename As String) As DataTable
        Dim strDataFilePath = Server.MapPath("Import/") & filename
        Dim dt As New DataTable()
        Dim conn As OleDbConnection = New OleDbConnection("Provider=Microsoft.Jet.OleDb.4.0; Data Source = " & Path.GetDirectoryName(strDataFilePath) & "; Extended Properties = ""Text;HDR=YES;FMT=Delimited""")
        conn.Open()
        Dim Adapter As New OleDbDataAdapter("Select * FROM " + Path.GetFileName(strDataFilePath), conn)
        Dim ds As New DataSet("Temp")
        Adapter.Fill(ds)
        conn.Close()
        conn.Dispose()
        Adapter.Dispose()
        dt = ds.Tables(0)
        ds.Dispose()
        Return dt
    End Function

    Protected Sub ddlSource_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlSource.SelectedIndexChanged
        If Not ddlSource.SelectedValue = 0 Then
            BindDropDown(ddlSource.SelectedItem.Text.Trim())
            If ddlSource.SelectedIndex = 2 Or ddlSource.SelectedIndex = 1 Then
                ddlOperation.Visible = True
            Else
                ddlOperation.Visible = False
            End If
            'Else
            '    lblMessage.Text = "* Please Select a Valid Document Source!!!"
            '    Exit Sub
        Else
            If (ddlFormtype.Items.Count > 0) Then
                ddlFormtype.Items.Clear()
                ddlFormtype.Items.Insert(0, New ListItem("--Select Document--"))
                ddlFormtype.DataBind()
            End If
        End If

    End Sub

    Protected Sub ddlFormtype_SelectedIndexChanged(sender As Object, e As EventArgs)
        Dim EID As Integer
        EID = Convert.ToInt32(Session("eid"))
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim Query = "Select iscsv, isxml,isnull(fieldseperator,'') [fieldseperator] from mmm_mst_forms where eid=" & EID & " And FormName='" & ddlFormtype.SelectedItem.Text.Trim() & "'"
        Dim dt As New DataTable()
        'Code For populating non editable field with current date
        Using con = New SqlConnection(conStr)
            Using da = New SqlDataAdapter(Query, con)
                da.Fill(dt)
            End Using
        End Using
        If (dt.Rows.Count > 0) Then
            lblSeperator.Text = "Field Seperator is-" & dt.Rows(0)("fieldseperator").ToString()
            If (dt.Rows(0)("iscsv").ToString() = "1" And dt.Rows(0)("isxml").ToString() = "1") Then
                'FT.Visible = True
                hdnFileType.Value = "BOTH"
            ElseIf (dt.Rows(0)("iscsv").ToString() = "1") Then
                imgCSVSample.Visible = True
                'FT.Visible = False
                hdnFileType.Value = "CSV"
            Else
                imgCSVSample.Visible = False
                'FT.Visible = False
                hdnFileType.Value = "XML"
            End If
        Else
            imgCSVSample.Visible = False
            'FT.Visible = False
        End If
    End Sub

    Protected Sub GetDataFromXml(ByVal strDataFilePath As String)
        Try
            Dim sr As New StreamReader(Server.MapPath("~/Import/" & strDataFilePath))
            Dim FileName = Server.MapPath("~/Import/" & strDataFilePath)
            Dim recs As New DataTable()
            Dim EID As Integer
            Dim DocType As String = ""
            Dim UID As Integer
            EID = Convert.ToInt32(Session("eid"))
            UID = Convert.ToInt32(Session("UID"))
            DocType = ddlFormtype.SelectedItem.Text
            Dim SuccessCount As Integer = 0
            Dim FailCount As Integer = 0
            Dim dtData As New DataTable
            dtData.Columns.Add("Service_response")

            Dim xmldoc As New XmlDocument()
            Dim xmlnode As XmlNodeList
            Dim fs As FileStream = New FileStream(FileName, FileMode.Open, FileAccess.Read)
            xmldoc.Load(fs)
            Dim value As String = Regex.Replace(ddlFormtype.SelectedItem.Text.Trim(), "\s+", "")
            Dim sa As String = xmldoc.DocumentElement.ChildNodes(0).Name

            If (sa.ToUpper() <> value.ToUpper()) Then
                lblMessage.Text = "There were error in XML"
                Exit Sub
            End If
            xmlnode = xmldoc.GetElementsByTagName(sa)
            Dim str As String = ""
            Dim dr As DataRow
            ' Dim SBS As New StringBuilder()
            If (xmlnode.Count > 0) Then
                For j As Integer = 0 To xmlnode.Count - 1
                    dr = dtData.NewRow
                    Dim SBS As New StringBuilder()
                    For i As Integer = 0 To xmlnode(j).ChildNodes.Count - 1
                        SBS.Append("|").Append(xmlnode(j).ChildNodes(i).Name.ToString().Trim().ToUpper()).Append("::").Append(xmlnode(j).ChildNodes(i).InnerText.ToString().Trim())
                    Next
                    SBS.Append("|Service_response::")
                    Dim StrData = SBS.ToString()
                    Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
                    Dim Query = "select inwardxmltagname,displayName from mmm_mst_fields where eid=" & EID & " and documenttype='" & ddlFormtype.SelectedItem.Text.Trim() & "'"
                    Dim dt As New DataTable()
                    'Code For populating non editable field with current date
                    Using con = New SqlConnection(conStr)
                        Using da = New SqlDataAdapter(Query, con)
                            da.Fill(dt)
                        End Using
                    End Using
                    If (dt.Rows.Count > 0) Then
                        For k As Integer = 0 To dt.Rows.Count - 1
                            If (StrData.ToString().Contains("|" & dt.Rows(k)("inwardxmltagname").ToString().ToUpper() & "::")) Then
                                StrData = StrData.Replace("|" & dt.Rows(k)("inwardxmltagname").ToString().ToUpper() & "::", "|" & dt.Rows(k)("displayName").ToString() & "::")
                            End If
                        Next
                    End If
                    Dim Result = ""
                    If ddlSource.SelectedIndex = 2 And ddlOperation.SelectedIndex = 1 Then
                        Dim update As New UpdateData
                        Result = update.UpdateData(EID, DocType, UID, StrData, "EnableEdit")
                    Else
                        Result = CommanUtil.ValidateParameterByDocumentType(EID, DocType, UID, StrData)
                    End If
                    'Dim Result = CommanUtil.ValidateParameterByDocumentType(EID, DocType, UID, StrData)
                    Result = Result.Replace("Error(s) in document " & DocType & ".", String.Empty)
                    Dim strPatt As String = "[|\d-]|"
                    Result = Regex.Replace(Result, strPatt, String.Empty)
                    If Result.Contains("Your DocID is") Then
                        SuccessCount = SuccessCount + 1
                    Else
                        FailCount = FailCount + 1
                    End If
                    dr("Service_response") = Result
                    dtData.Rows.Add(dr)
                Next
                lblSuccessDocument.Text = "<b>" & SuccessCount & "</b>"
                lblTotalDocument.Text = "<b>" & xmlnode.Count & "</b>"
                lblFailedDocument.Text = "<b>" & FailCount & "</b>"
                gvData.DataSource = dtData
                gvData.DataBind()
                pnlMessage.Visible = True
            End If
        Catch ex As Exception
            Throw ex
        End Try

    End Sub

    Protected Sub imgExport_Click(sender As Object, e As ImageClickEventArgs)
        Response.Clear()
        Response.Buffer = True
        Response.Charset = ""
        Response.ContentType = "application/vnd.ms-excel"
        Dim filename As String = ddlFormtype.SelectedItem.Text
        filename = filename.Replace(" ", "_")
        Response.AddHeader("content-disposition", "attachment;filename=" & filename & "_ERROR.xls")

        Dim gvex As New GridView()
        If Not IsNothing(ViewState("ERRORDATA")) Then
            Dim dt2 As DataTable = ViewState("ERRORDATA")
            gvex.AllowPaging = False
            gvex.DataSource = dt2
            gvex.DataBind()
            Response.Clear()
            Response.Buffer = True
            Dim sw As New StringWriter()
            Dim hw As New HtmlTextWriter(sw)

            Dim tb As New Table()
            Dim tr1 As New TableRow()
            Dim cell1 As New TableCell()
            cell1.Controls.Add(gvex)
            tr1.Cells.Add(cell1)
            Dim cell2 As New TableCell()
            cell2.Text = "&nbsp;"
            Dim tr2 As New TableRow()
            tr2.Cells.Add(cell2)
            Dim tr3 As New TableRow()
            tb.Rows.Add(tr1)
            tb.Rows.Add(tr2)
            tb.Rows.Add(tr3)
            tb.RenderControl(hw)
            'style to format numbers to string 
            Dim style As String = "<style> .textmode { mso-number-format:\@; } </style>"
            Response.Write(style)
            Response.Output.Write(sw.ToString())
            Response.Flush()
            Response.[End]()
        End If

    End Sub

    Public Function getSafeString_uploader(ByVal strVar As String) As String
        Trim(strVar)
        strVar = Replace(strVar, "'", "")
        strVar = Replace(strVar, """", "")
        strVar = Replace(strVar, ";", "")
        strVar = Replace(strVar, "--", "")
        'strVar = Replace(strVar, "%", "")
        'strVar = Replace(strVar, "&", "")
        Return strVar
    End Function

    Public Function getSafeString_Names(ByVal strVar As String) As String
        Trim(strVar)
        strVar = Replace(strVar, "'", "")
        strVar = Replace(strVar, ";", "")
        strVar = Replace(strVar, "--", "")
        strVar = Replace(strVar, "%", "Percent")
        strVar = Replace(strVar, "&", "and")
        strVar = Replace(strVar, "!", "")
        strVar = Replace(strVar, """", "")
        strVar = Replace(strVar, "#", "")
        strVar = Replace(strVar, "$", "")
        strVar = Replace(strVar, "*", "")
        strVar = Replace(strVar, ",", "")
        strVar = Replace(strVar, "/", "")
        strVar = Replace(strVar, ":", "")
        strVar = Replace(strVar, "?", "")
        strVar = Replace(strVar, "[", "(")
        strVar = Replace(strVar, "\", "")
        strVar = Replace(strVar, "]", ")")
        strVar = Replace(strVar, "^", "")
        strVar = Replace(strVar, "`", "")
        strVar = Replace(strVar, "{", "(")
        'strVar = Replace(strVar, "|", "")
        strVar = Replace(strVar, "}", ")")
        strVar = Replace(strVar, "~", "")
        strVar = Replace(strVar, "+", "")
        strVar = Replace(strVar, "<", "")
        strVar = Replace(strVar, "=", "")
        strVar = Replace(strVar, ">", "")
        Return strVar
    End Function
End Class
