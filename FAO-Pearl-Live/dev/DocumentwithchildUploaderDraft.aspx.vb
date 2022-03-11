Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports System.Globalization
Imports System.Drawing
Imports System.Data.OleDb
Imports System.Xml
Imports Ionic.Zip

Partial Class DocumentwithchildUploaderDraft
    Inherits System.Web.UI.Page
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            If Not Page.IsPostBack Then
                BindDropDown()
            End If
        Catch ex As Exception
        End Try
    End Sub
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
    Public Sub BindDropDown()
        Dim Eid As Integer = 0
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim li As New ListItem("--Select Document--", 0)
        Try
            Eid = Convert.ToInt32(Session("eid"))
            Dim MQuerry = "SELECT MID,MenuName,PageLink FROM MMM_MST_MENU WHERE MID NOT IN (SELECT DISTINCT PMENU FROM MMM_MST_MENU WHERE PMENU IS NOT NULL and EID=" & Session("EID") & ") and eid=" & Session("EID") & " AND PageLink like 'Documents.Aspx?%'and roles like '%{" & Session("USERROLE") & "%" & "'"
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
                StrDocument = Document.ToString.Substring(1, Document.ToString.Length - 1)
            End If

            Dim da As New SqlDataAdapter("select formid,formname from MMM_MST_FORMS where EID=" & Eid & " and FormType='DOCUMENT' and IsActive=1 AND FormSource='MENU DRIVEN' and FormName in (" & StrDocument & ") and showuploader=1", con)
            Dim ds As New DataSet
            da.Fill(ds, "FormName")
            If ds.Tables("FormName").Rows.Count > 0 Then
                ddlFormtype.DataSource = ds.Tables("FormName")
                ddlFormtype.DataValueField = "FormName"
                ddlFormtype.DataTextField = "FormName"
                ddlFormtype.DataBind()
                ddlFormtype.Items.Insert(0, li)
            End If
            con.Close()
            con.Dispose()
        Catch ex As Exception
            ddlFormtype.Items.Insert(0, li)
            con.Close()
            con.Dispose()
        End Try
    End Sub
    Protected Sub btnChk_Click(sender As Object, e As EventArgs) Handles imgCSVSample.Click
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Try
            Dim scrname As String = ddlFormtype.SelectedItem.Text
            Dim Query = "SELECT 'DocNumber' As 'DisPlayName','Yes'[Mandatory Fields],'Numeric Digits'[Data Type],'1' [Minimum Length] ,''[Maximum Length] UNION ALL SELECT rtrim(ltrim(displayName)) AS [DisplayName],case isRequired when 0 then 'No' when 1 then 'Yes' end [Mandatory Fields],case datatype when 'text' then 'Text' when 'numeric' then 'Numeric Digits' when 'Datetime' then 'Date in (DD/MM/YY)' end [Data Type],case MinLen when 0 then '' else MinLen end [Minimum Length], case MaxLen when 0 then '' else maxlen end [Maximum Length] FROM MMM_MST_FIELDS where EID =" & Session("eid") & " and DocumentType ='" & scrname & "' AND ISACTIVE=1   AND FieldType<>'Child Item' AND  FieldType <>'Formula Field' and FieldType <> 'Calculative Field' AND  IsEditable=1 and invisible=0     UNION ALL SELECT rtrim(ltrim(DocumentType))+ '.'+ rtrim(ltrim(displayName ))AS [Display Name],case isRequired when 0 then 'No' when 1 then 'Yes' end [Mandatory Fields],case datatype when 'text' then 'Text' when 'numeric' then 'Numeric Digits' when 'Datetime' then 'Date in (DD/MM/YY)' end [Data Type],case MinLen when 0 then '' else MinLen end [Minimum Length], case MaxLen when 0 then '' else maxlen end [Maximum Length] FROM MMM_MST_FIELDS where EID =" & Session("eid") & " and DocumentType in(SELECT Dropdown FROM MMM_MST_FIELDS WHERE EID=" & Session("eid") & " AND DocumentType ='" & scrname & "') AND ISACTIVE=1 and  FieldType <>'Formula Field' AND FieldType <>'Child Item' and FieldType <> 'Calculative Field' AND  IsEditable=1 and invisible=0 "
            Dim ds As New DataSet

            Using con = New SqlConnection(conStr)
                Using da = New SqlDataAdapter(Query, con)
                    con.Open()
                    da.Fill(ds, "data")
                End Using
            End Using
            Dim dt As DataTable = New DataTable()
            Dim dt2 As DataTable = New DataTable()
            Dim dt3 As DataTable = New DataTable()
            dt3 = ds.Tables("data")
            dt2 = GetInversedDataTable(ds.Tables("data"), "displayname", "")
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
            Response.Charset = ""
            Response.ContentType = "application/vnd.ms-excel"
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
            Response.AddHeader("content-disposition", "attachment;filename=" & scrname & ".xls")
            tb.Rows.Add(tr1)
            tb.Rows.Add(tr2)
            tb.Rows.Add(tr3)
            tb.RenderControl(hw)
            'style to format numbers to string 
            Dim style As String = "<style> .textmode { mso-number-format:\@; } </style>"
            Response.Write(style)
            Response.Output.Write(sw.ToString())
            If Response.IsClientConnected Then
                Response.Flush()
                Response.[End]()
            End If
        Catch ex As Exception
        End Try
    End Sub

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

    Protected Sub btnUpload_Click(sender As Object, e As EventArgs) Handles btnSubmit.Click
        Dim SB As StringBuilder = Nothing
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim errormsg As String = ""
        Try
            Dim EID As Integer
            Dim DocType As String = ""
            Dim UID As Integer
            Dim dterrorResult As New DataTable
            EID = Convert.ToInt32(Session("eid"))
            UID = Convert.ToInt32(Session("UID"))
            Dim lstData As New List(Of DocumentUp)
            Dim ObjDoc As DocumentUp
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
                        Dim Query = "select DisplayName,fieldType,datatype,DropDown FROM MMM_MST_Fields where EID=" & EID & " AND  IsActive=1 and FieldType='Child Item' and documentType= '" & DocType & "'"
                        Dim ds As New DataSet()
                        'Code For populating non editable field with current date
                        Using con = New SqlConnection(conStr)
                            Using da = New SqlDataAdapter(Query, con)
                                da.Fill(ds)
                            End Using
                        End Using
                        Dim lstChild As New List(Of Child)
                        Dim objC As Child
                        If ds.Tables(0).Rows.Count > 0 Then
                            For j As Integer = 0 To ds.Tables(0).Rows.Count - 1
                                objC = New Child()
                                objC.DocumentType = ds.Tables(0).Rows(j).Item("dropdown")
                                lstChild.Add(objC)
                            Next
                        End If
                        dtData.Columns.Add("Service_response")
                        Dim DocStr As StringBuilder
                        Dim ChildStr As StringBuilder
                        dterrorResult = dtData.Clone()
                        If dtData.Rows.Count > 0 Then
                            Dim SuccessCount As Integer = 0
                            Dim FailCount As Integer = 0
                            Dim RowID As String = "0"
                            For i As Integer = 0 To dtData.Rows.Count - 1
                                'Checking Duplicacy of main Document
                                If RowID <> Convert.ToInt32(dtData.Rows(i).Item("DocNumber")).ToString.Trim Then
                                    ObjDoc = New DocumentUp()
                                    ObjDoc.DocNumber = RowID
                                    DocStr = New StringBuilder()
                                    For Each column As DataColumn In dtData.Columns
                                        'Check for ensuring that this data is of main document
                                        If Not column.ColumnName.Trim.Contains(".") Then
                                            DocStr.Append("|").Append(column.ColumnName.Trim).Append("::").Append(dtData.Rows(i).Item(column.ColumnName).ToString)
                                        End If
                                    Next
                                    ObjDoc.DataString = DocStr.ToString
                                    ObjDoc.lstChild = CreateChildList(ds)
                                    ObjDoc.DocNumber = dtData.Rows(i).Item("DocNumber")
                                    lstData.Add(ObjDoc)
                                    'Traversing column for each child
                                    For Each Child As Child In lstChild
                                        ChildStr = New StringBuilder()
                                        Dim HasData As Boolean = False
                                        For Each column As DataColumn In dtData.Columns
                                            If column.ColumnName.Trim.Contains(Child.DocumentType & ".") Then
                                                If (dtData.Rows(i).Item(column.ColumnName).ToString.Trim <> "") Then
                                                    Dim arr = column.ColumnName.Split(".")
                                                    ChildStr.Append("()").Append(arr(1).Trim).Append("<>").Append(dtData.Rows(i).Item(column.ColumnName).ToString)
                                                    HasData = True
                                                End If
                                            End If
                                        Next
                                        CreateCollecttion(lstData, dtData.Rows(i).Item("DocNumber"), Child.DocumentType, ChildStr.ToString, HasData)
                                    Next
                                Else
                                    For Each Child As Child In lstChild
                                        ChildStr = New StringBuilder()
                                        Dim HasData As Boolean = False
                                        For Each column As DataColumn In dtData.Columns
                                            If column.ColumnName.Trim.Contains(Child.DocumentType & ".") Then
                                                If (dtData.Rows(i).Item(column.ColumnName).ToString.Trim <> "") Then
                                                    Dim arr = column.ColumnName.Split(".")
                                                    ChildStr.Append("()").Append(arr(1).Trim).Append("<>").Append(dtData.Rows(i).Item(column.ColumnName).ToString)
                                                    HasData = True
                                                End If
                                            End If
                                        Next
                                        CreateCollecttion(lstData, dtData.Rows(i).Item("DocNumber"), Child.DocumentType, ChildStr.ToString, HasData)
                                    Next
                                End If
                                RowID = Convert.ToInt32(dtData.Rows(i).Item("DocNumber"))
                            Next
                            'Code of saving going from here 
                            Dim mainStr As StringBuilder

                            For Each Document In lstData
                                mainStr = New StringBuilder(Document.DataString)
                                For Each Child In Document.lstChild
                                    ChildStr = New StringBuilder()
                                    For Each Item In Child.chldData
                                        If Item.HasData = True Then
                                            ChildStr.Append("{}").Append(Item.DataString)
                                        End If
                                    Next
                                    If ChildStr.ToString.Trim <> "" Then
                                        mainStr.Append("|" & Child.DocumentType).Append("::").Append(ChildStr.ToString)
                                    End If
                                Next
                                Dim StrData As String = mainStr.ToString()
                                Dim Result = CommanUtil.ValidateParameterByDocumentType1(EID, DocType, UID, StrData)

                                If Result.Contains("Error(s) in document ") Then
                                    GetRandomRows(dtData, Document.DocNumber, dterrorResult, Result)
                                End If

                                Result = Result.Replace("Error(s) in document " & DocType & ".", String.Empty)
                                Dim strPatt As String = "[|\d-]|]"
                                Result = Regex.Replace(Result, strPatt, String.Empty)
                                Document.Response = Result
                                If Result.Contains("Your DocID is") Then
                                    SuccessCount = SuccessCount + 1
                                Else
                                    FailCount = FailCount + 1
                                End If
                            Next
                            lblSuccessDocument.Text = "<b>" & SuccessCount & "</b>"
                            lblTotalDocument.Text = "<b>" & lstData.Count & "</b>"
                            lblFailedDocument.Text = "<b>" & FailCount & "</b>"
                            gvData.DataSource = lstData
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
                    'commented by manvendra 21-02-2019'

                    'ElseIf hdnFileType.Value = "XML" Then
                    '    If Right(CSVUploader.FileName, 4).ToUpper() = ".XML" Then
                    '        Dim filename As String = "COLL" & Now.Day & Now.Month & Now.Year & Now.Hour & Now.Minute & Now.Minute & Now.Second & Now.Millisecond & Right(CSVUploader.PostedFile.FileName, 4).ToUpper()
                    '        CSVUploader.PostedFile.SaveAs(Server.MapPath("Import/") & filename)
                    '        GetDataFromXml(filename)
                    '    Else

                    '    End If
                ElseIf hdnFileType.Value = "CSV" Then

                    If Right(CSVUploader.FileName, 4).ToUpper() = ".CSV" Then
                        Dim filename As String = "COLL" & Now.Day & Now.Month & Now.Year & Now.Hour & Now.Minute & Now.Minute & Now.Second & Now.Millisecond & Right(CSVUploader.PostedFile.FileName, 4).ToUpper()
                        CSVUploader.PostedFile.SaveAs(Server.MapPath("Import/") & filename)
                        'lblMessage.Text = ex.Message & "<br/>" & "File Name IS " & filename
                        'converting csv file into datatable
                        Dim dtData As New DataTable
                        dtData = GetDataFromExcel(filename, DocType)
                        Dim Query = "select DisplayName,fieldType,datatype,DropDown FROM MMM_MST_Fields where EID=" & EID & " AND  IsActive=1 and FieldType='Child Item' and documentType= '" & DocType & "'"
                        Dim ds As New DataSet()
                        'Code For populating non editable field with current date
                        Using con = New SqlConnection(conStr)
                            Using da = New SqlDataAdapter(Query, con)
                                da.Fill(ds)
                            End Using
                        End Using
                        Dim lstChild As New List(Of Child)
                        Dim objC As Child
                        If ds.Tables(0).Rows.Count > 0 Then
                            For j As Integer = 0 To ds.Tables(0).Rows.Count - 1
                                objC = New Child()
                                objC.DocumentType = ds.Tables(0).Rows(j).Item("dropdown")
                                lstChild.Add(objC)
                            Next
                        End If
                        dtData.Columns.Add("Service_response")
                        Dim DocStr As StringBuilder
                        Dim ChildStr As StringBuilder

                        If dtData.Rows.Count > 0 Then
                            Dim SuccessCount As Integer = 0
                            Dim FailCount As Integer = 0
                            Dim RowID As String = "0"
                            For i As Integer = 0 To dtData.Rows.Count - 1
                                'Checking Duplicacy of main Document
                                'Add condition if unique condition is there 
                                Dim objDC As New DataClass()
                                Dim strUnique As String = objDC.ExecuteQryScaller("select displayname from mmm_mst_fields where eid=" & EID & " and documenttype='" & DocType & "' and isunique=1")
                                If strUnique <> "" Then
                                    If RowID <> Convert.ToString(dtData.Rows(i).Item("" & Convert.ToString(strUnique) & "")).Trim Then
                                        ObjDoc = New DocumentUp()
                                        ObjDoc.DocNumber = RowID
                                        DocStr = New StringBuilder()
                                        For Each column As DataColumn In dtData.Columns
                                            'Check for ensuring that this data is of main document
                                            If Not column.ColumnName.Trim.Contains(".") Then
                                                DocStr.Append("|").Append(column.ColumnName.Trim).Append("::").Append(dtData.Rows(i).Item(column.ColumnName).ToString)
                                            End If
                                        Next
                                        ObjDoc.DataString = DocStr.ToString
                                        ObjDoc.lstChild = CreateChildList(ds)
                                        ObjDoc.DocNumber = dtData.Rows(i).Item("" & Convert.ToString(strUnique) & "")
                                        lstData.Add(ObjDoc)
                                        'Traversing column for each child
                                        'Traversing column for each child
                                        For Each Child As Child In lstChild
                                            ChildStr = New StringBuilder()
                                            Dim HasData As Boolean = False
                                            For Each column As DataColumn In dtData.Columns
                                                If column.ColumnName.Trim.Contains(Child.DocumentType & ".") Then
                                                    If (dtData.Rows(i).Item(column.ColumnName).ToString.Trim <> "") Then
                                                        Dim arr = column.ColumnName.Split(".")
                                                        ChildStr.Append("()").Append(arr(1).Trim).Append("<>").Append(dtData.Rows(i).Item(column.ColumnName).ToString)
                                                        HasData = True
                                                    End If
                                                End If
                                            Next
                                            CreateCollecttion(lstData, dtData.Rows(i).Item("" & Convert.ToString(strUnique) & ""), Child.DocumentType, ChildStr.ToString, HasData)
                                        Next
                                    Else
                                        For Each Child As Child In lstChild
                                            ChildStr = New StringBuilder()
                                            Dim HasData As Boolean = False
                                            For Each column As DataColumn In dtData.Columns
                                                If column.ColumnName.Trim.Contains(Child.DocumentType & ".") Then
                                                    If (dtData.Rows(i).Item(column.ColumnName).ToString.Trim <> "") Then
                                                        Dim arr = column.ColumnName.Split(".")
                                                        ChildStr.Append("()").Append(arr(1).Trim).Append("<>").Append(dtData.Rows(i).Item(column.ColumnName).ToString)
                                                        HasData = True
                                                    End If
                                                End If
                                            Next
                                            CreateCollecttion(lstData, dtData.Rows(i).Item("" & Convert.ToString(strUnique) & ""), Child.DocumentType, ChildStr.ToString, HasData)
                                        Next
                                    End If
                                    RowID = Convert.ToString(dtData.Rows(i).Item("" & Convert.ToString(strUnique) & ""))
                                    'For Existing condition
                                Else
                                    If RowID <> Convert.ToInt32(dtData.Rows(i).Item("DocNumber")).ToString.Trim Then
                                        ObjDoc = New DocumentUp()
                                        ObjDoc.DocNumber = RowID
                                        DocStr = New StringBuilder()
                                        For Each column As DataColumn In dtData.Columns
                                            'Check for ensuring that this data is of main document
                                            If Not column.ColumnName.Trim.Contains(".") Then
                                                DocStr.Append("|").Append(column.ColumnName.Trim).Append("::").Append(dtData.Rows(i).Item(column.ColumnName).ToString)
                                            End If
                                        Next
                                        ObjDoc.DataString = DocStr.ToString
                                        ObjDoc.lstChild = CreateChildList(ds)
                                        ObjDoc.DocNumber = dtData.Rows(i).Item("DocNumber")
                                        lstData.Add(ObjDoc)
                                        'Traversing column for each child
                                        For Each Child As Child In lstChild
                                            ChildStr = New StringBuilder()
                                            Dim HasData As Boolean = False
                                            For Each column As DataColumn In dtData.Columns
                                                If column.ColumnName.Trim.Contains(Child.DocumentType & ".") Then
                                                    If (dtData.Rows(i).Item(column.ColumnName).ToString.Trim <> "") Then
                                                        Dim arr = column.ColumnName.Split(".")
                                                        ChildStr.Append("()").Append(arr(1).Trim).Append("<>").Append(dtData.Rows(i).Item(column.ColumnName).ToString)
                                                        HasData = True
                                                    End If
                                                End If
                                            Next
                                            CreateCollecttion(lstData, dtData.Rows(i).Item("DocNumber"), Child.DocumentType, ChildStr.ToString, HasData)
                                        Next
                                    Else
                                        For Each Child As Child In lstChild
                                            ChildStr = New StringBuilder()
                                            Dim HasData As Boolean = False
                                            For Each column As DataColumn In dtData.Columns
                                                If column.ColumnName.Trim.Contains(Child.DocumentType & ".") Then
                                                    If (dtData.Rows(i).Item(column.ColumnName).ToString.Trim <> "") Then
                                                        Dim arr = column.ColumnName.Split(".")
                                                        ChildStr.Append("()").Append(arr(1).Trim).Append("<>").Append(dtData.Rows(i).Item(column.ColumnName).ToString)
                                                        HasData = True
                                                    End If
                                                End If
                                            Next
                                            CreateCollecttion(lstData, dtData.Rows(i).Item("DocNumber"), Child.DocumentType, ChildStr.ToString, HasData)
                                        Next
                                    End If
                                    RowID = Convert.ToInt32(dtData.Rows(i).Item("DocNumber"))
                                End If
                            Next
                            'Code of saving going from here 
                            Dim mainStr As StringBuilder

                            For Each Document In lstData
                                mainStr = New StringBuilder(Document.DataString)
                                For Each Child In Document.lstChild
                                    ChildStr = New StringBuilder()
                                    For Each Item In Child.chldData
                                        If Item.HasData = True Then
                                            ChildStr.Append("{}").Append(Item.DataString)
                                        End If
                                    Next
                                    If ChildStr.ToString.Trim <> "" Then
                                        mainStr.Append("|" & Child.DocumentType).Append("::").Append(ChildStr.ToString)
                                    End If
                                Next
                                Dim StrData As String = mainStr.ToString()
                                'Functionality for Added Edit 
                                Dim Result = ""
                                Dim update As New UpdateData
                                If ddlOperation.SelectedIndex = 2 Then
                                    Result = update.UpdateData(EID, DocType, UID, StrData, "EnableEdit")
                                Else
                                    'Result = CommanUtil.SaveDraft(EID, DocType, UID, StrData, ObjDoc.DocNumber)

                                    Result = CommanUtil.ValidateParameterByDocumentType1(EID, DocType, UID, StrData)
                                End If

                                Result = Result.Replace("Error(s) In document " & DocType & ".", String.Empty)
                                Dim strPatt As String = "[|\d-]|"
                                Result = Regex.Replace(Result, strPatt, String.Empty)
                                Document.Response = Result
                                If Result.Contains("Your DocID Is") Then
                                    SuccessCount = SuccessCount + 1
                                Else
                                    FailCount = FailCount + 1
                                End If
                            Next
                            Dim newdtData As New DataTable()
                            For Each item In lstData
                                Dim dNo = item.DocNumber
                                Dim Filter = dtData.AsEnumerable().Where(Function(m) m.Field(Of String)("DocNumber") = dNo).[Select](Function(m) m).AsDataView().ToTable()
                                If item.Response.Contains("Error(s) In") Then
                                    Dim test = ""
                                    If Not (item.Response.Contains("|")) Then
                                        test = item.Response
                                        item.Response = item.Response & "|1"
                                    Else
                                        Dim data1 = item.Response.Split("|")
                                        test = data1(0) & data1(2).Substring(0, data1(2).Length - 1)
                                    End If
                                    Dim data = item.Response.Split("|")
                                    Dim lNo = data(1).ToString
                                    item.Response = item.Response.Substring(0, item.Response.Length - 1)

                                    Filter.Rows(lNo - 1).Item("service_response") = test
                                    newdtData.Merge(Filter)
                                    Dim strRplc = "|" + data(1) + "|"
                                    Dim strNew = "Line Number -" + data(1) + "."
                                    item.Response = item.Response.Replace(strRplc, strNew)
                                End If
                            Next
                            gvResult.DataSource = newdtData
                            gvResult.DataBind()
                            lblSuccessDocument.Text = "<b>" & SuccessCount & "</b>"
                            lblTotalDocument.Text = "<b>" & lstData.Count & "</b>"
                            lblFailedDocument.Text = "<b>" & FailCount & "</b>"
                            gvData.DataSource = lstData
                            gvData.DataBind()
                            pnlMessage.Visible = True
                        Else
                            pnlMessage.Visible = False
                            lblSuccessDocument.Text = "<b>0</b>"
                            lblTotalDocument.Text = "<b>0</b>"
                            lblFailedDocument.Text = "<b>0</b>"
                            gvData.DataSource = Nothing
                            gvData.DataBind()
                            lblMessage.Text = "No Any records found To upload."
                        End If
                    End If
                ElseIf hdnFileType.Value = "ZIPWITHCSV" Then
                    If Right(CSVUploader.FileName, 4).ToUpper() = ".ZIP" Then

                        Dim filename As String = "COLL" & Now.Day & Now.Month & Now.Year & Now.Hour & Now.Minute & Now.Minute & Now.Second & Now.Millisecond & Right(CSVUploader.PostedFile.FileName, 4).ToUpper()
                        Dim path As String = Server.MapPath("DOCS/") & filename
                        Try
                            CSVUploader.PostedFile.SaveAs(path)
                        Catch ex As Exception
                            errormsg &= "Error While Saving file In Mail Attach Folder"
                        End Try

                        Dim folderName As String() = filename.ToString().Split(".")
                        Dim prefixFilePath As String = Server.MapPath("~/Import/" & folderName(0))
                        Dim folderExists As Boolean = Directory.Exists(prefixFilePath)
                        If Not folderExists Then
                            Try
                                Directory.CreateDirectory(prefixFilePath)
                            Catch ex As Exception
                                errormsg &= "Directory Creation Error"
                            End Try

                        End If
                        Dim arraylist As New ArrayList()
                        Try
                            Using zip As ZipFile = ZipFile.Read(path)
                                zip.ExtractAll(prefixFilePath, ExtractExistingFileAction.DoNotOverwrite)
                                For Each data In zip
                                    arraylist.Add(data.FileName)
                                Next
                            End Using
                        Catch ex As Exception
                            lblMessage.Text = errormsg & "Error While extracting file from folder" & ex.Message
                        End Try

                        If arraylist.Count > 0 Then
                            'Dim importedfile = From arrlist As ArrayList In arraylist Where arrlist.Contains(".CSV") Select arrlist Order By arrlist
                            For x As Integer = 0 To arraylist.Count - 1
                                If arraylist(x).ToString().ToUpper.Contains(".CSV") Then
                                    Dim dtData As New DataTable
                                    dtData = GetDataFromExcel(folderName(0) & "/" & arraylist(x).ToString(), DocType)
                                    Dim Query = "Select DisplayName, fieldType, datatype, DropDown FROM MMM_MST_Fields where EID=" & EID & " And  IsActive=1 And FieldType in ('Child Item','Child Item Total') and documentType= '" & DocType & "'"
                                    Dim ds As New DataSet()
                                    'Code For populating non editable field with current date
                                    Using con = New SqlConnection(conStr)
                                        Using da = New SqlDataAdapter(Query, con)
                                            da.Fill(ds)
                                        End Using
                                    End Using
                                    Dim lstChild As New List(Of Child)
                                    Dim objC As Child
                                    If ds.Tables(0).Rows.Count > 0 Then
                                        For j As Integer = 0 To ds.Tables(0).Rows.Count - 1
                                            objC = New Child()
                                            objC.DocumentType = ds.Tables(0).Rows(j).Item("dropdown")
                                            lstChild.Add(objC)
                                        Next
                                    End If
                                    dtData.Columns.Add("Service_response")
                                    Dim DocStr As StringBuilder
                                    Dim ChildStr As StringBuilder

                                    If dtData.Rows.Count > 0 Then
                                        Dim SuccessCount As Integer = 0
                                        Dim FailCount As Integer = 0
                                        Dim RowID As String = "0"
                                        For i As Integer = 0 To dtData.Rows.Count - 1
                                            'Checking Duplicacy of main Document
                                            If RowID <> Convert.ToInt32(dtData.Rows(i).Item("DocNumber")).ToString.Trim Then
                                                ObjDoc = New DocumentUp()
                                                ObjDoc.DocNumber = RowID
                                                DocStr = New StringBuilder()
                                                For Each column As DataColumn In dtData.Columns
                                                    'Check for ensuring that this data is of main document
                                                    If Not column.ColumnName.Trim.Contains(".") Then
                                                        DocStr.Append("|").Append(column.ColumnName.Trim).Append("::").Append(dtData.Rows(i).Item(column.ColumnName).ToString)
                                                    End If
                                                Next
                                                ObjDoc.DataString = DocStr.ToString
                                                ObjDoc.lstChild = CreateChildList(ds)
                                                ObjDoc.DocNumber = dtData.Rows(i).Item("DocNumber")
                                                lstData.Add(ObjDoc)
                                                'Traversing column for each child
                                                For Each Child As Child In lstChild
                                                    ChildStr = New StringBuilder()
                                                    Dim HasData As Boolean = False
                                                    For Each column As DataColumn In dtData.Columns
                                                        If column.ColumnName.Trim.Contains(Child.DocumentType & ".") Then
                                                            If (dtData.Rows(i).Item(column.ColumnName).ToString.Trim <> "") Then
                                                                Dim arr = column.ColumnName.Split(".")
                                                                ChildStr.Append("()").Append(arr(1).Trim).Append("<>").Append(dtData.Rows(i).Item(column.ColumnName).ToString)
                                                                HasData = True
                                                            End If
                                                        End If
                                                    Next
                                                    CreateCollecttion(lstData, dtData.Rows(i).Item("DocNumber"), Child.DocumentType, ChildStr.ToString, HasData)
                                                Next
                                            Else
                                                For Each Child As Child In lstChild
                                                    ChildStr = New StringBuilder()
                                                    Dim HasData As Boolean = False
                                                    For Each column As DataColumn In dtData.Columns
                                                        If column.ColumnName.Trim.Contains(Child.DocumentType & ".") Then
                                                            If (dtData.Rows(i).Item(column.ColumnName).ToString.Trim <> "") Then
                                                                Dim arr = column.ColumnName.Split(".")
                                                                ChildStr.Append("()").Append(arr(1).Trim).Append("<>").Append(dtData.Rows(i).Item(column.ColumnName).ToString)
                                                                HasData = True
                                                            End If
                                                        End If
                                                    Next
                                                    CreateCollecttion(lstData, dtData.Rows(i).Item("DocNumber"), Child.DocumentType, ChildStr.ToString, HasData)
                                                Next
                                            End If
                                            RowID = Convert.ToInt32(dtData.Rows(i).Item("DocNumber"))
                                        Next
                                        'Code of saving going from here 
                                        Dim mainStr As StringBuilder

                                        For Each Document In lstData
                                            mainStr = New StringBuilder(Document.DataString)
                                            For Each Child In Document.lstChild
                                                ChildStr = New StringBuilder()
                                                For Each Item In Child.chldData
                                                    If Item.HasData = True Then
                                                        ChildStr.Append("{}").Append(Item.DataString)
                                                    End If
                                                Next
                                                If ChildStr.ToString.Trim <> "" Then
                                                    mainStr.Append("|" & Child.DocumentType).Append("::").Append(ChildStr.ToString)
                                                End If
                                            Next
                                            Dim StrData As String = mainStr.ToString()
                                            Dim Result = CommanUtil.ValidateParameterByDocumentType1(EID, DocType, UID, StrData, Nothing, False, False, False, arraylist, prefixFilePath:=prefixFilePath)
                                            Result = Result.Replace("Error(s) in document " & DocType & ".", String.Empty)
                                            Dim strPatt As String = "[|\d-]|"
                                            Result = Regex.Replace(Result, strPatt, String.Empty)
                                            Document.Response = Result
                                            If Result.Contains("Your DocID is") Then
                                                SuccessCount = SuccessCount + 1
                                            Else
                                                FailCount = FailCount + 1
                                            End If
                                        Next
                                        Dim newdtData As New DataTable()
                                        For Each item In lstData
                                            Dim dNo = item.DocNumber
                                            Dim Filter = dtData.AsEnumerable().Where(Function(m) m.Field(Of String)("DocNumber") = dNo).[Select](Function(m) m).AsDataView().ToTable()
                                            If item.Response.Contains("Error(s) in") Then
                                                Dim test = ""
                                                If Not (item.Response.Contains("|")) Then
                                                    test = item.Response
                                                    item.Response = item.Response & "|1"
                                                Else
                                                    Dim data1 = item.Response.Split("|")
                                                    test = data1(0) & data1(2).Substring(0, data1(2).Length - 1)
                                                End If
                                                Dim data = item.Response.Split("|")
                                                Dim lNo = data(1).ToString
                                                item.Response = item.Response.Substring(0, item.Response.Length - 1)

                                                Filter.Rows(lNo - 1).Item("service_response") = test
                                                newdtData.Merge(Filter)
                                                Dim strRplc = "|" + data(1) + "|"
                                                Dim strNew = "Line Number -" + data(1) + "."
                                                item.Response = item.Response.Replace(strRplc, strNew)
                                            End If
                                        Next
                                        gvResult.DataSource = newdtData
                                        gvResult.DataBind()
                                        lblSuccessDocument.Text = "<b>" & SuccessCount & "</b>"
                                        lblTotalDocument.Text = "<b>" & lstData.Count & "</b>"
                                        lblFailedDocument.Text = "<b>" & FailCount & "</b>"
                                        gvData.DataSource = lstData
                                        gvData.DataBind()
                                        pnlMessage.Visible = True
                                    Else
                                        pnlMessage.Visible = False
                                        lblSuccessDocument.Text = "<b>0</b>"
                                        lblTotalDocument.Text = "<b>0</b>"
                                        lblFailedDocument.Text = "<b>0</b>"
                                        gvData.DataSource = Nothing
                                        gvData.DataBind()
                                        lblMessage.Text = "No Any records found to upload."
                                    End If
                                End If
                            Next
                            'delete file from directory
                            Dim di As DirectoryInfo = New DirectoryInfo(prefixFilePath)
                            If di.Exists Then
                                di.Delete(True)
                            End If
                        End If
                    End If
                End If
            End If
        Catch ex As Exception
            'lblMessage.Text = "Sorry!!! Your request can not completed at the moment. Try again later.<br/>" & ex.Message & " and " & errormsg
            If ex.Message.Contains("Input array is longer than the") Then
                lblMessage.Text = "Issue with File format.<br/>Fields seperator configured is (,) Comma. And one or more of field{s) value contains Comma<br/>So Either remove comma or change the seperator to PIPE (|) "
            ElseIf ex.Message.Contains("does not belong to table") Then
                lblMessage.Text = "Issue with File format.<br/>Fields seperator used in file does not matching with configured in System for This Document type<br/>For Ex. - Seperator configured is PIPE (|) but in File seperator is Comma (,)"
            Else
                lblMessage.Text = "There is some unidentified issue with File Format/Field Seperator.<br/>Please Contact Admin with File to troubleshoot issue.<br/> System Error is - " & ex.Message & " and " & errormsg
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
    Public Function CreateChildList(ds As DataSet) As List(Of Child)
        Dim lstChild As New List(Of Child)
        Dim objC As Child
        If ds.Tables(0).Rows.Count > 0 Then
            For j As Integer = 0 To ds.Tables(0).Rows.Count - 1
                objC = New Child()
                objC.DocumentType = ds.Tables(0).Rows(j).Item("dropdown")
                lstChild.Add(objC)
            Next
        End If
        Return lstChild
    End Function

    Public Function GetDataFromExcel(ByVal strDataFilePath As String, Optional ByVal DocumentType As String = Nothing) As DataTable
        ' GetDataFromExcel123(strDataFilePath)
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
            Dim sr As New StreamReader(Server.MapPath("~/Import/" & strDataFilePath))
            Dim FileName = Server.MapPath("~/Import/" & strDataFilePath)
            Dim fullFileStr As String = sr.ReadToEnd()
            fullFileStr = fullFileStr.Replace("'=", "=").Replace("'-", "-").Replace("'+", "+")
            sr.Close()
            sr.Dispose()
            Dim lines As String() = fullFileStr.Split(ControlChars.Lf)
            Dim recs As New DataTable()
            Dim sArr As String() = lines(0).Split(Seperator)
            For Each s As String In sArr
                recs.Columns.Add(New DataColumn(s.Trim()))
            Next
            Dim row As DataRow
            Dim finalLine As String = ""
            Dim i As Integer = 0
            For Each line As String In lines
                If i > 0 And Not String.IsNullOrEmpty(line.Trim()) Then
                    row = recs.NewRow()
                    finalLine = line.Replace(Convert.ToString(ControlChars.Cr), "")
                    row.ItemArray = finalLine.Split(Seperator)
                    recs.Rows.Add(row)
                End If
                i = i + 1
            Next
            Return recs
        Catch ex As Exception
            Throw ex
        End Try
    End Function

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
            dtData.Columns.Add("DocNumber")
            dtData.Columns.Add("Response")

            Dim xmldoc As New XmlDocument()
            Dim xmlnode As XmlNodeList
            Dim fs As FileStream = New FileStream(FileName, FileMode.Open, FileAccess.Read)
            xmldoc.Load(fs)

            Dim value As String = Regex.Replace(ddlFormtype.SelectedItem.Text.Trim(), "\s+", "")
            'Dim sa As String = xmldoc.DocumentElement.ChildNodes(0).Name
            Dim sa As String = xmldoc.GetElementsByTagName("FORMNAME")(0).InnerText
            sa = Regex.Replace(sa.ToString().Trim(), "\s+", "")
            If (sa.ToUpper() <> value.ToUpper()) Then
                lblMessage.Text = "There were error in XML"
                Exit Sub
            End If
            'xmlnode = xmldoc.GetElementsByTagName(sa)
            xmlnode = xmldoc.GetElementsByTagName("TALLYMESSAGE")
            Dim str As String = ""
            Dim dr As DataRow
            ' Dim SBS As New StringBuilder()
            If (xmlnode.Count > 0) Then
                Dim conStrs As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
                Dim Querys = "select inwardxmltagname,displayName from mmm_mst_fields where eid=" & EID & " and documenttype in(select dropdown from mmm_mst_fields where eid=" & EID & " and documenttype='" & ddlFormtype.SelectedItem.Text.Trim() & "' and fieldType='Child Item') and inwardxmltagname is not null;select inwardxmltagname,displayName from mmm_mst_fields where eid=" & EID & " and documenttype='" & ddlFormtype.SelectedItem.Text.Trim() & "'and fieldType<>'Child Item' and  inwardxmltagname is not null;select inwardxmltagname,dropdown from mmm_mst_fields where eid=" & EID & " and documenttype='" & ddlFormtype.SelectedItem.Text.Trim() & "'and fieldType='Child Item' and inwardxmltagname is not null"
                Dim dts As New DataSet()
                'Code For populating non editable field with current date
                Using con = New SqlConnection(conStrs)
                    Using da = New SqlDataAdapter(Querys, con)
                        da.Fill(dts, "ChildItem")
                    End Using
                End Using
                For j As Integer = 0 To xmlnode.Count - 1
                    dr = dtData.NewRow
                    Dim SBS As New StringBuilder()
                    Dim ChildString As New StringBuilder()
                    For i As Integer = 0 To xmlnode(j).ChildNodes.Count - 1
                        If Not ((xmlnode(j).ChildNodes(i).InnerText) = "") Then
                            If (xmlnode(j).ChildNodes(i).FirstChild.HasChildNodes) Then
                                If (SBS.ToString().Contains("|Service_response::")) Then
                                Else
                                    SBS.Append("|Service_response::")
                                End If
                                If (xmlnode(j).ChildNodes(i).ChildNodes.Count > 0) Then

                                    If (SBS.ToString().Contains("|" & xmlnode(j).ChildNodes(i).Name.ToString().Trim().ToUpper() & "::")) Then
                                        SBS.Append("{}")
                                        For Z As Integer = 0 To xmlnode(j).ChildNodes(i).ChildNodes.Count - 1
                                            Dim sas As String = xmlnode(j).ChildNodes(i).ChildNodes(Z).Name

                                            If (Z = xmlnode(j).ChildNodes(i).ChildNodes.Count - 1) Then
                                                SBS.Append(xmlnode(j).ChildNodes(i).ChildNodes(Z).Name.ToUpper()).Append("<>").Append(xmlnode(j).ChildNodes(i).ChildNodes(Z).InnerText.ToString())
                                            Else
                                                SBS.Append(xmlnode(j).ChildNodes(i).ChildNodes(Z).Name.ToUpper()).Append("<>").Append(xmlnode(j).ChildNodes(i).ChildNodes(Z).InnerText.ToString()).Append("()")
                                            End If
                                        Next
                                    Else
                                        SBS.Append("|").Append(xmlnode(j).ChildNodes(i).Name.ToString().Trim().ToUpper()).Append("::")
                                        For Z As Integer = 0 To xmlnode(j).ChildNodes(i).ChildNodes.Count - 1
                                            Dim sas As String = xmlnode(j).ChildNodes(i).ChildNodes(Z).Name
                                            If (Z = xmlnode(j).ChildNodes(i).ChildNodes.Count - 1) Then
                                                SBS.Append(xmlnode(j).ChildNodes(i).ChildNodes(Z).Name.ToUpper()).Append("<>").Append(xmlnode(j).ChildNodes(i).ChildNodes(Z).InnerText.ToString())
                                            Else
                                                SBS.Append(xmlnode(j).ChildNodes(i).ChildNodes(Z).Name.ToUpper()).Append("<>").Append(xmlnode(j).ChildNodes(i).ChildNodes(Z).InnerText.ToString()).Append("()")
                                            End If
                                        Next
                                    End If


                                End If
                            Else
                                If (SBS.ToString().Contains("<>")) Then
                                Else
                                    SBS.Append("|").Append(xmlnode(j).ChildNodes(i).Name.ToString().Trim().ToUpper()).Append("::").Append(xmlnode(j).ChildNodes(i).InnerText.ToString().Trim())
                                End If

                            End If
                        End If

                    Next
                    Dim StrData = SBS.ToString()

                    If (dts.Tables("ChildItem1").Rows.Count > 0) Then
                        For k As Integer = 0 To dts.Tables("ChildItem1").Rows.Count - 1
                            If (StrData.ToString().Contains("|" & dts.Tables("ChildItem1").Rows(k)("inwardxmltagname").ToString().ToUpper() & "::")) Then
                                StrData = StrData.Replace("|" & dts.Tables("ChildItem1").Rows(k)("inwardxmltagname").ToString().ToUpper() & "::", "|" & dts.Tables("ChildItem1").Rows(k)("displayName").ToString() & "::")
                            End If
                        Next
                    End If
                    If (dts.Tables("ChildItem2").Rows.Count > 0) Then
                        For k As Integer = 0 To dts.Tables("ChildItem2").Rows.Count - 1
                            If (StrData.ToString().Contains("|" & dts.Tables("ChildItem2").Rows(k)("inwardxmltagname").ToString().ToUpper() & "::")) Then
                                StrData = StrData.Replace("|" & dts.Tables("ChildItem2").Rows(k)("inwardxmltagname").ToString().ToUpper() & "::", "|" & dts.Tables("ChildItem2").Rows(k)("dropdown").ToString() & "::")
                            End If
                        Next
                    End If
                    If (dts.Tables("ChildItem").Rows.Count > 0) Then
                        For k As Integer = 0 To dts.Tables("ChildItem").Rows.Count - 1
                            If (StrData.ToString().Contains("::" & dts.Tables("ChildItem").Rows(k)("inwardxmltagname").ToString().ToUpper() & "<>")) Then
                                StrData = StrData.Replace("::" & dts.Tables("ChildItem").Rows(k)("inwardxmltagname").ToString().ToUpper() & "<>", "::" & dts.Tables("ChildItem").Rows(k)("displayName").ToString() & "<>")
                                If (StrData.ToString().Contains("{}" & dts.Tables("ChildItem").Rows(k)("inwardxmltagname").ToString().ToUpper() & "<>")) Then
                                    StrData = StrData.Replace("{}" & dts.Tables("ChildItem").Rows(k)("inwardxmltagname").ToString().ToUpper() & "<>", "{}" & dts.Tables("ChildItem").Rows(k)("displayName").ToString() & "<>")
                                End If
                            ElseIf (StrData.ToString().Contains("()" & dts.Tables("ChildItem").Rows(k)("inwardxmltagname").ToString().ToUpper() & "<>")) Then
                                StrData = StrData.Replace("()" & dts.Tables("ChildItem").Rows(k)("inwardxmltagname").ToString().ToUpper() & "<>", "()" & dts.Tables("ChildItem").Rows(k)("displayName").ToString() & "<>")
                                If (StrData.ToString().Contains("{}" & dts.Tables("ChildItem").Rows(k)("inwardxmltagname").ToString().ToUpper() & "<>")) Then
                                    StrData = StrData.Replace("{}" & dts.Tables("ChildItem").Rows(k)("inwardxmltagname").ToString().ToUpper() & "<>", "{}" & dts.Tables("ChildItem").Rows(k)("displayName").ToString() & "<>")
                                End If
                            End If
                        Next
                    End If

                    'Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
                    'Dim Query = "select inwardxmltagname,displayName from mmm_mst_fields where eid=" & EID & " and documenttype='" & ddlFormtype.SelectedItem.Text.Trim() & "'"
                    'Dim dt As New DataTable()
                    ''Code For populating non editable field with current date
                    'Using con = New SqlConnection(conStr)
                    '    Using da = New SqlDataAdapter(Query, con)
                    '        da.Fill(dt)
                    '    End Using
                    'End Using

                    Dim Result = CommanUtil.ValidateParameterByDocumentType1(EID, DocType, UID, StrData)
                    Result = Result.Replace("Error(s) in document " & DocType & ".", String.Empty)
                    Dim strPatt As String = "[|\d-]|"
                    Result = Regex.Replace(Result, strPatt, String.Empty)
                    If Result.Contains("Your DocID is") Then
                        SuccessCount = SuccessCount + 1
                    Else
                        FailCount = FailCount + 1
                    End If
                    Dim output As String = Regex.Replace(Result, "[^0-9]+", String.Empty)
                    dr("DocNumber") = output
                    dr("Response") = Result
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

    Public Function CreateCollecttion(ByRef LstDoc As List(Of DocumentUp), DocNumber As String, ChildName As String, Datastring As String, HasData As Boolean) As Boolean
        Try
            'Dim ObjDOC = From Obj In LstDoc Where (Obj.DocNumber = DocNumber) Select Obj
            Dim obj As DetailChid
            'loop through all the list
            For Each item In LstDoc
                If item.DocNumber = DocNumber Then
                    For Each line In item.lstChild
                        If line.DocumentType = ChildName Then
                            obj = New DetailChid()
                            obj.HasData = HasData
                            obj.DataString = Datastring
                            line.DocumentType = ChildName
                            line.chldData.Add(obj)
                        End If
                    Next
                End If
            Next
        Catch ex As Exception
            Throw
        End Try
        Return True
    End Function


    Protected Sub ddlFormtype_SelectedIndexChanged(sender As Object, e As EventArgs)
        Dim EID As Integer
        EID = Convert.ToInt32(Session("eid"))
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString

        Dim Query = "select iscsv,isxml,AllowZipFile from mmm_mst_forms where eid=" & EID & " and FormName='" & ddlFormtype.SelectedItem.Text.Trim() & "'"
        Dim dt As New DataTable()
        'Code For populating non editable field with current date
        Using con = New SqlConnection(conStr)
            Using da = New SqlDataAdapter(Query, con)
                da.Fill(dt)
            End Using
        End Using
        If (dt.Rows.Count > 0) Then
            If (dt.Rows(0)("iscsv").ToString() = "1" And dt.Rows(0)("isxml").ToString() = "1") Then
                'FT.Visible = True
                hdnFileType.Value = "BOTH"
            ElseIf (dt.Rows(0)("iscsv").ToString() = "1") Then
                imgCSVSample.Visible = True
                'FT.Visible = False
                If ((dt.Rows(0)("AllowZipFile").ToString() = True) And (dt.Rows(0)("iscsv").ToString() = "1")) Then
                    hdnFileType.Value = "ZIPWITHCSV"
                Else
                    hdnFileType.Value = "CSV"
                End If

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
    Protected Sub imgExport_Click(sender As Object, e As ImageClickEventArgs)
        'Response.Clear()
        'Response.Buffer = True
        'Response.Charset = ""
        'Response.ContentType = "application/vnd.ms-excel"
        'Dim filename As String = ddlFormtype.SelectedItem.Text
        'filename = filename.Replace(" ", "_")
        'Response.AddHeader("content-disposition", "attachment;filename=" & filename & "_ERROR.xls")

        'Dim gvex As New GridView()
        'If Not IsNothing(ViewState("ERRORDATA")) Then
        '    Dim dt2 As DataTable = ViewState("ERRORDATA")
        '    gvex.AllowPaging = False
        '    gvex.DataSource = dt2
        '    gvex.DataBind()
        '    Response.Clear()
        '    Response.Buffer = True
        '    Dim sw As New StringWriter()
        '    Dim hw As New HtmlTextWriter(sw)

        '    Dim tb As New Table()
        '    Dim tr1 As New TableRow()
        '    Dim cell1 As New TableCell()
        '    cell1.Controls.Add(gvex)
        '    tr1.Cells.Add(cell1)
        '    Dim cell2 As New TableCell()
        '    cell2.Text = "&nbsp;"
        '    Dim tr2 As New TableRow()
        '    tr2.Cells.Add(cell2)
        '    Dim tr3 As New TableRow()
        '    tb.Rows.Add(tr1)
        '    tb.Rows.Add(tr2)
        '    tb.Rows.Add(tr3)
        '    tb.RenderControl(hw)
        '    'style to format numbers to string 
        '    Dim style As String = "<style> .textmode { mso-number-format:\@; } </style>"
        '    Response.Write(style)
        '    Response.Output.Write(sw.ToString())
        '    Response.Flush()
        '    Response.[End]()
        'End If
        HttpContext.Current.Response.Clear()
        HttpContext.Current.Response.Buffer = True
        HttpContext.Current.Response.AddHeader("content-disposition", "attachment;filename=" + ddlFormtype.SelectedItem.Text + "-UploaderResponse.xls")
        HttpContext.Current.Response.Charset = ""
        HttpContext.Current.Response.ContentType = "application/vnd.ms-excel"
        Dim sw As New StringWriter()
        Dim hw As New HtmlTextWriter(sw)
        Dim parent As Control = gvResult.Parent
        Dim GridIndex As Integer = 0
        If parent IsNot Nothing Then
            GridIndex = parent.Controls.IndexOf(gvResult)
            parent.Controls.Remove(gvResult)
        End If
        gvResult.RenderControl(hw)
        If parent IsNot Nothing Then
            parent.Controls.AddAt(GridIndex, gvResult)
        End If
        Dim style As String = "<style> .textmode { mso-number-format:\@; } </style>"
        HttpContext.Current.Response.Write(sw.ToString())
        HttpContext.Current.Response.End()
        HttpContext.Current.Response.Flush()
        'HttpContext.Current.ApplicationInstance.CompleteRequest()
        'gvData.Visible = False
    End Sub


End Class
Public Class Child
    Public Property DocumentType As String
    Public Property chldData As New List(Of DetailChid)
End Class
Public Class DocumentUp
    Public Property DocNumber As String
    Public Property DataString As String
    Public Property lstChild As New List(Of Child)
    Public Property Response As String
End Class
Public Class DetailChid
    Public Sub New()   'constructor
    End Sub
    Public Sub New(Data As String, flag As Boolean)   'constructor
        Me.DataString = Data
        Me.HasData = flag
    End Sub
    Public Property DataString As String
    Public Property HasData As Boolean


End Class