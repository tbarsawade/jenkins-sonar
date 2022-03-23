Imports System.Net.HttpWebRequest
Imports System.Net
Imports System.IO
Imports System.Data
Imports System.Configuration
Imports System.Data.SqlClient
Imports System.Collections.Generic
Imports System.Xml
Imports System.Web.Services

Partial Class VB
    Inherits System.Web.UI.Page
    Dim DocumentName As String = ""
    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        'If Not Me.IsPostBack Then
        If DropDownList1.SelectedIndex = 0 Then
            Page.ClientScript.RegisterStartupScript(Me.[GetType](), "alert", "Map('NM');", True)
        ElseIf DropDownList1.SelectedIndex = 1 Then
            Page.ClientScript.RegisterStartupScript(Me.[GetType](), "alert", "Map('GM');", True)
        End If
        'End If
        hdnDoc.Value = Request.QueryString("DocName")
    End Sub
    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init

        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("SELECT FF.*,F.*  FROM MMM_MST_FIELDS FF left outer JOIN MMM_MST_FORMS F on F.FormName = FF.DocumentType and F.EID = FF.EID  where FF.isactive=1 and F.EID=" & Session("EID").ToString() & " and FormName = '" & Session("DocumentName").ToString & "' order by displayOrder", con)
        Dim ds As New DataSet()
        oda.Fill(ds, "fields")
        Dim messageStrip As String = Session("HEADERSTRIP").ToString()
        Dim ob As New DynamicForm
        ob.CLEARDYNAMICFIELDS(pnlFields)
        ob.CreateControlsOnPanel(ds.Tables("fields"), pnlFields, updatePanelEdit, btnActEdit, 0)

    End Sub
    Public Function ConvertDataTabletoString() As String
        Dim dt As New DataTable()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        'Dim oda As SqlDataAdapter = New SqlDataAdapter("select top 2 tid,fld1,fld15,fld16,fld11,fld10,fld13,fld14,Case isauth when 1 then 'ACTIVE' when 0 then 'INACTIVE' END  [Active] from mmm_mst_master where EID=32 and documenttype = 'Area Office' and fld15 is not null", con)
        'Dim sqlquery = "select FIELDMAPPING from MMM_MST_Fields WHERE DocumentType="+Session("")""'company master' and  eid=42 AND FIELDTYPE='geo point'"
        'Dim oda As SqlDataAdapter = New SqlDataAdapter("select top 2 tid,fld1,fld15,fld16,fld11,fld10,fld13,fld14,Case isauth when 1 then 'ACTIVE' when 0 then 'INACTIVE' END  [Active] from mmm_mst_master where EID=32 and documenttype = 'Area Office' and fld15 is not null", con)
        ' Dim oda As SqlDataAdapter = New SqlDataAdapter("select tid, [GeoPoint] as GeoPoint from V42enquiry_master where eid=" + Session("EID").ToString() + " and [Geo Point]!=''", con)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
        Dim fld As String = ""
        oda.SelectCommand.CommandText = " select FieldID as Tid, displayname, FieldMapping from mmm_mst_fields where eid=" & Session("EID") & " and documenttype='" & Session("DocumentName").ToString & "' and fieldtype='Geo Point' "

        Dim dtFld As New DataTable
        oda.Fill(dtFld)
        fld = dtFld.Rows(0).Item("displayname")
        hdnFld.Value = dtFld.Rows(0).Item("Tid")
        con.Close()
        oda.SelectCommand.CommandText = "select tid, [" & fld & "] as GeoPoint from V" & Session("EID").ToString + Replace(Session("DocumentName").ToString, " ", "_") & " where eid=" + Session("EID").ToString() + " and [" & fld & "]!='' and [" & fld & "] is not null"
        oda.Fill(dt)
        Session("Datatable") = dt
        Dim serializer As New System.Web.Script.Serialization.JavaScriptSerializer()
        Dim rows As New List(Of Dictionary(Of String, Object))()
        Dim row As Dictionary(Of String, Object)
        For Each dr As DataRow In dt.Rows
            row = New Dictionary(Of String, Object)()
            For Each col As DataColumn In dt.Columns
                row.Add(col.ColumnName, dr(col))
            Next
            rows.Add(row)
        Next
        Return serializer.Serialize(rows)
    End Function
    Protected Sub EditHit(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim tid As String = hdntid.Value
        ViewState("tid") = tid
        Dim btnDetails As ImageButton = TryCast(sender, ImageButton)
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("select FF.* from MMM_MST_FIELDS FF inner join MMM_MST_MASTER M on FF.DocumentType =M.DocumentType   where FF.EID=" & Session("EID") & " AND M.tid =" & tid & " and FF.isactive=1  order by displayOrder", con)
        Dim ds As New DataSet()
        oda.Fill(ds, "fields")
        Dim ob As New DynamicForm
        ob.FillControlsOnPanel(ds.Tables("fields"), pnlFields, "MASTER", tid)
        btnActEdit.Text = "Update"
        lblHeaderPopUp.Text = "Edit " & lblHeaderPopUp.Text.Replace("Add New ", "")
        updPanalHeader.Update()
        updatePanelEdit.Update()
        btnEdit_ModalPopupExtender.Show()
        lblTab.Text = ""

    End Sub
    Protected Sub EditRecord(ByVal sender As Object, ByVal e As System.EventArgs)
        'validation for null entry
        If btnActEdit.Text = "Save" Then
            ValidateData("ADD")
        Else
            ValidateData("EDIT")
        End If
    End Sub
    Protected Sub ValidateData(ByVal actionType As String)
        'Check All Validations
        ' now validation of created controls
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim da As SqlDataAdapter = New SqlDataAdapter("SELECT FF.*,F.*  FROM MMM_MST_FIELDS FF left outer JOIN MMM_MST_FORMS F on F.FormName = FF.DocumentType and F.EID = FF.EID  where FF.isactive=1 and F.EID=" & Session("EID").ToString() & " and FormName = '" & Session("DocumentName").ToString & "' order by displayOrder", con)
        Dim ds As New DataSet
        da.Fill(ds, "fields")
        Dim ob As New DynamicForm
        Dim FinalQry As String
        If actionType = "ADD" Then
            If ds.Tables("fields").Rows(0).Item("layouttype") = "CUSTOM" Then
                FinalQry = ob.ValidateAndGenrateQueryForCustom("ADD", "INSERT INTO MMM_MST_MASTER(EID,Documenttype,createdby,updateddate,", "VALUES (" & Session("EID").ToString() & ",'" & Session("DocumentName").ToString & "'," & Session("UID").ToString() & ",getdate(),", ds.Tables("fields"), pnlFields) & ""
            Else
                FinalQry = ob.ValidateAndGenrateQueryForControls("ADD", "INSERT INTO MMM_MST_MASTER(EID,Documenttype,createdby,updateddate,", "VALUES (" & Session("EID").ToString() & ",'" & Session("DocumentName").ToString & "'," & Session("UID").ToString() & ",getdate(),", ds.Tables("fields"), pnlFields, 0) & ""
            End If

        Else
            'pass query of updation and also type
            FinalQry = ob.ValidateAndGenrateQueryForControls("UPDATE", "UPDATE MMM_MST_MASTER SET updateddate=getdate(),", "", ds.Tables("fields"), pnlFields, ViewState("tid"))
        End If

        If Trim(Left(FinalQry, 6)).ToUpper() = "PLEASE" Then
            lblTab.Text = FinalQry
        Else
            If actionType <> "ADD" Then
                FinalQry = FinalQry & " WHERE tID=" & ViewState("tid")
            Else
                FinalQry = FinalQry & ";select @@identity"
            End If
            'save the data
            lblTab.Text = ""
            da.SelectCommand.CommandText = FinalQry
            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            Dim fileid As Integer = da.SelectCommand.ExecuteScalar()

            If actionType <> "ADD" Then
                fileid = ViewState("tid")
            ElseIf actionType = "ADD" Then
                Dim row As DataRow() = ds.Tables("fields").Select("Fieldtype='Auto Number'")
                If row.Length > 0 Then
                    da.SelectCommand.Parameters.Clear()
                    da.SelectCommand.CommandText = "usp_GetAutoNoNew"
                    da.SelectCommand.CommandType = CommandType.StoredProcedure
                    da.SelectCommand.Parameters.AddWithValue("Fldid", row(0).Item("fieldid"))
                    da.SelectCommand.Parameters.AddWithValue("docid", fileid)
                    da.SelectCommand.Parameters.AddWithValue("fldmapping", row(0).Item("fieldmapping"))
                    da.SelectCommand.Parameters.AddWithValue("FormType", "Master")
                    Dim an As String = da.SelectCommand.ExecuteScalar()
                    'msgAN = "<br/> " & row(0).Item("displayname") & " is " & an & ""
                    da.SelectCommand.Parameters.Clear()
                End If
            End If

            'Added By Komal for Formula

            Dim CalculativeField() As DataRow = ds.Tables("fields").Select("Fieldtype='Formula Field'")
            Dim viewdoc As String = DocumentName
            viewdoc = viewdoc.Replace(" ", "_")
            If CalculativeField.Length > 0 Then
                For Each CField As DataRow In CalculativeField
                    Dim formulaeditorr As New formulaEditor
                    Dim forvalue As String = String.Empty
                    forvalue = formulaeditorr.ExecuteFormula(CField.Item("KC_LOGIC"), fileid, "v" + Session("eid").ToString + viewdoc, Session("eid").ToString, 0)
                    Dim upquery As String = "update " & CField.Item("DBTableName").ToString & "  set  " & CField.Item("FieldMapping").ToString & "='" & forvalue.ToString & "'  where tid =" & fileid & ""
                    da.SelectCommand.CommandText = upquery
                    da.SelectCommand.CommandType = CommandType.Text
                    da.SelectCommand.ExecuteNonQuery()
                Next
            End If
            'End
            'INSERT INTO HISTORY 
            ob.History(Session("EID"), fileid, Session("UID"), Session("DocumentName").ToString, "MMM_MST_MASTER", actionType)
            ob.CLEARDYNAMICFIELDS(pnlFields)
            btnEdit_ModalPopupExtender.Hide()
        End If
        da.Dispose()
        con.Dispose()
    End Sub
    Protected Sub LockHit(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim btnDetails As ImageButton = TryCast(sender, ImageButton)
        Dim tid As String = hdntid.Value
        ViewState("tid") = tid
        Dim ds As New DataSet()
        Dim dt2 As DataTable = DirectCast(Session("Datatable"), DataTable)
        Dim Row1 As DataRow() = dt2.Select("tid='" & tid & "'")
        Dim a = Row1(0).Item(1).ToString()
        If Row1(0).Item(1).ToString().ToUpper().Trim() = "ACTIVE" Then
            lblLock.Text = "<b>Please click the option here under to confirm if you wish to lock the record - """ & Row1(0).Item(1).ToString() & " ""</b>"
            btnLockupdate.Text = "Lock"
        Else
            lblLock.Text = "<b>Please click the option here under to confirm if you wish to Unlock the  record - """ & Row1(0).Item(1).ToString() & " ""</b>"
            btnLockupdate.Text = "Unlock"
        End If
        Me.updLock.Update()
        Me.ModalPopup_Lock.Show()
    End Sub
    Protected Sub LockRecord(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("uspLockMaster", con)
        oda.SelectCommand.CommandType = CommandType.StoredProcedure
        oda.SelectCommand.Parameters.AddWithValue("eid", Session("EID"))
        oda.SelectCommand.Parameters.AddWithValue("tid", ViewState("tid"))
        If con.State <> ConnectionState.Open Then
            con.Open()
        End If
        Dim iSt As Integer = oda.SelectCommand.ExecuteScalar()
        con.Close()
        oda.Dispose()
        con.Dispose()
        If iSt = 0 Or iSt = 1 Then
            lblRecord.Text = "Updated  successfully"
            ModalPopup_Lock.Hide()
        Else
            lblLock.Text = "Not updated"
        End If
        updLock.Update()
    End Sub

    Protected Sub btnsavedata_Click(sender As Object, e As EventArgs)
        Dim UpdateLatLongQuery As String = String.Empty
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim sqlquery As String = "select tid, Address+','+[City Pin Code] as Addressed from V42enquiry_master where DocumentType='Enquiry Master' and eid=" + Session("EID").ToString() + ""
        Dim adap = New SqlDataAdapter(sqlquery, con)
        Dim dt As New DataTable()
        adap.Fill(dt)
        If dt.Rows.Count > 0 Then
            For i As Integer = 2414 To 3467
                UpdateLatLongQuery = "update V42enquiry_master set [Geo Point]=" & "'" & LatLongs(dt.Rows(i)("Addressed").ToString()) & "'" & " where tid = " & dt.Rows(i)("tid").ToString()
                adap = New SqlDataAdapter(UpdateLatLongQuery, con)
                If con.State = ConnectionState.Closed Then
                    con.Open()
                End If
                Dim res As Integer = adap.SelectCommand.ExecuteNonQuery()
            Next
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End If

    End Sub
    Public Function LatLongs(ByVal LocationName As String) As String
        Dim latlng As String = String.Empty
        Dim url As String = "http://geocoder.cit.api.here.com/6.2/geocode.xml?app_id=DemoAppId01082013GAL&app_code=AJKnXv84fjrb0KIHawS0Tg&gen=3&searchtext=" + LocationName + ""
        Dim request As HttpWebRequest = DirectCast(WebRequest.Create(url), HttpWebRequest)
        Dim response As WebResponse = request.GetResponse()
        Dim dataStream As Stream = response.GetResponseStream()
        Dim sreader As New StreamReader(dataStream)
        Dim responsereader As String = sreader.ReadToEnd()
        response.Close()
        Dim xmldoc As New XmlDocument()
        xmldoc.LoadXml(responsereader)
        If xmldoc.ChildNodes.Count > 0 Then
            Dim SelNodesTxt As String = xmldoc.DocumentElement.Name
            Dim Cnt As Integer = 0
            'Dim nodes As XmlNodeList = xmldoc.SelectNodes("//Waypoint/MappedPosition")
            Dim nodes As XmlNodeList = xmldoc.SelectNodes("//Location/DisplayPosition")
            For Each node As XmlNode In nodes
                For c As Integer = 0 To node.ChildNodes.Count - 1
                    If node.ChildNodes.Item(c).Name = "Latitude" Then
                        latlng &= node.ChildNodes.Item(c).InnerText & ","
                    End If
                    If node.ChildNodes.Item(c).Name = "Longitude" Then
                        latlng &= node.ChildNodes.Item(c).InnerText & ","
                    End If
                Next
            Next
        End If
        Return latlng
    End Function

    Protected Sub btnchangeview_Click1(sender As Object, e As EventArgs) Handles btnchangeview.Click
        Response.Redirect("Masters.ASPX?SC=" & Session("DocumentName").ToString & "")
    End Sub

    <WebMethod()> _
 <Script.Services.ScriptMethod()> _
    Public Shared Function GetInfo(Tid As Integer, Doc As String, FldId As Integer) As String
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim InfoStr As New StringBuilder()

        Dim da As New SqlDataAdapter("Select Kc_value from mmm_mst_fields where Fieldid=" & FldId, con)
        Dim dt As New DataTable
        da.Fill(dt)
        If dt.Rows.Count = 0 Then
            Return "No information found."
        ElseIf dt.Rows(0)("Kc_value").ToString.Trim() = "" Then
            Return "No information found."
        End If
        Dim strFields As String() = dt.Rows(0).Item("Kc_value").ToString().Split(",")

        Dim InfoQry = ""
        Dim usr As String = ""
        For i As Integer = 0 To strFields.Length - 1
            Dim qry = "Select * from mmm_mst_Fields where Eid=" & HttpContext.Current.Session("Eid") & " and DocumentType='" & Doc & "' and FieldMapping='" & strFields(i) & "' and isActive=1 "
            Dim DtRow As New DataTable
            da.SelectCommand.CommandText = qry
            da.Fill(DtRow)
            If DtRow.Rows.Count = 0 Then
                Continue For
            End If
            If DtRow.Rows(0).Item("DropDownType").ToString = "MASTER VALUED" Then
                InfoQry &= "dms.udf_split('" & DtRow.Rows(0).Item("DropDown") & "'," & strFields(i) & ")[" & DtRow.Rows(0).Item("displayName") & "],"
            ElseIf DtRow.Rows(0).Item("DropDownType").ToString = "SESSION VALUED" Then
                ' InfoQry &= "dms.udf_split('" & DtRow.Rows(0).Item("DropDown") & "'," & strFields(i) & ")[" & DtRow.Rows(0).Item("displayName") & "],"
            ElseIf DtRow.Rows(0).Item("FieldType").ToString() = "File Uploader" Then
                InfoQry &= strFields(i) & "[" & DtRow.Rows(0).Item("FieldType") & "],"
            Else
                InfoQry &= strFields(i) & "[" & DtRow.Rows(0).Item("displayName") & "],"
            End If
        Next
        InfoQry = InfoQry.Remove(InfoQry.LastIndexOf(","))
        da.SelectCommand.CommandText = "Select " & InfoQry & " from mmm_mst_master where Tid=" & Tid
        Dim dtInfo As New DataTable
        da.Fill(dtInfo)
        Dim strImg As String = ""
        For i As Integer = 0 To dtInfo.Columns.Count - 1
            If dtInfo.Columns(i).ColumnName.ToString() = "File Uploader" Then
                strImg = "<img src='docs/" & dtInfo.Rows(0).Item(dtInfo.Columns(i).ColumnName) & "'  width='100%' height='200px' /><br>"

            Else
                InfoStr.Append("<b>" & dtInfo.Columns(i).ColumnName & "</b> : " & dtInfo.Rows(0).Item(dtInfo.Columns(i).ColumnName) & "<br>")
            End If

        Next
        InfoStr.Append(strImg)
        Return InfoStr.ToString()

    End Function


End Class
