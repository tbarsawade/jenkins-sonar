Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports iTextSharp.text.pdf
Imports System.Security.Policy
Imports System.Net.Security
Imports System.Net.HttpWebRequest
Imports System.Net.HttpWebResponse
Imports System.Security.Cryptography.X509Certificates
Imports System.Web.Services
Imports Newtonsoft
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Converters
Imports System.Web.Script.Serialization
Partial Class NewMaster
    Inherits System.Web.UI.Page
    Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString

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
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            If Not (Page.IsPostBack) Then
                'BindFields()

                hdnDocumentType.Value = Request.QueryString("SC")
                'dvControls.InnerHtml = CreatedDynamicFields(Request.QueryString("SC"), Session("EID"), "0")
            End If
        Catch ex As Exception

        End Try
    End Sub
    Protected Sub BindFields()
        Try
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As New SqlConnection(conStr)
            'fill Product  
            Dim scrname As String = Request.QueryString("SC").ToString()
            'lblRecord.Text = ""
            Dim da As New SqlDataAdapter("SELECT displayName,FieldMapping,fieldtype FROM MMM_MST_FIELDS where EID =" & Session("EID").ToString() & " and DocumentType ='" & scrname & "' AND ISACTIVE=1 order by displayOrder", con)
            Dim ds As New DataSet
            'lblCaption.Text = scrname
            da.Fill(ds, "data")
            For i As Integer = 0 To ds.Tables("data").Rows.Count - 1
                'ddlField.Items.Add(ds.Tables("data").Rows(i).Item(0))
                'ddlField.Items(i).Value = ds.Tables("data").Rows(i).Item(1)
            Next
        Catch ex As Exception
        End Try
    End Sub

    <System.Web.Services.WebMethod()>
    Public Shared Function CreatedDynamicFields(DocType As String, EID As String, Tid As String) As String
        Try
            Dim strcol As String = ""
            Dim strqry As String = ""
            Dim con As SqlConnection = Nothing
            Dim oda As SqlDataAdapter = Nothing
            Dim dsData As New DataSet()
            Dim dsFields As New DataSet()
            Dim SB As New StringBuilder()
            If (EID = "") Then
                EID = HttpContext.Current.Session("EID")
            End If
            dsFields = GetFormFields2(EID, DocType)
            Dim onlyFiltered As New DataView()
            onlyFiltered = dsFields.Tables(0).DefaultView()
            onlyFiltered.RowFilter = "Invisible=0"
            Dim ds As DataTable = onlyFiltered.Table.DefaultView.ToTable()

            Dim SBFields As New StringBuilder()
            If Tid > 0 Then
                For Each rw As DataRow In ds.Rows
                    strcol &= rw.Item("fieldmapping").ToString & ","
                Next
                strcol = strcol.Substring(0, strcol.Length - 1)
                strqry = "Select " & strcol & " from MMM_MST_MASTER WHERE TID=" & Tid & ""
                Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
                con = New SqlConnection(conStr)
                oda = New SqlDataAdapter("", con)
                oda.SelectCommand.CommandText = strqry
                oda.Fill(dsData, "data")
            End If

            If ds.Rows.Count > 0 Then
                Dim newScreenWidth As Integer = 0
                Dim newScreenHeight As Integer = 0
                Dim layout As String = ds.Rows(0).Item("LayoutType").ToString()
                Dim clsmain As String = "contact"
                Dim clscredits As String = ""
                If layout = "DOUBLE COLUMN" Then
                    clsmain = "contact"
                    clscredits = "allcredits"
                Else
                    clsmain = "contact main"
                    clscredits = "allcredits cred"
                End If
                SB.Append("<div class=""" & clsmain & """>")
                For i As Integer = 0 To ds.Rows.Count - 1
                    If layout = "DOUBLE COLUMN" Then
                        If i Mod 2 = 0 Then
                            SBFields.Append("<div class=""cf_box"">")
                        End If
                    Else
                        SBFields.Append("<div class=""cf_box"">")
                    End If

                    If ds.Rows(i).Item("FieldType").ToString().ToUpper() = "TEXT AREA" Then
                        SBFields.Append("<div class=""" & clscredits & " full ddd""> <label>")
                    Else
                        SBFields.Append("<div class=""" & clscredits & """> <label>")
                    End If

                    SBFields.Append(ds.Rows(i).Item("DisplayName"))
                    SBFields.Append("<span>")
                    If (ds.Rows(i).Item("isRequired") = "1") Then
                        SBFields.Append("*")
                    End If
                    SBFields.Append("</span>")
                    SBFields.Append(":</label>")
                    Dim val As String = ""
                    If Tid > 0 Then
                        If (dsData.Tables("data").Rows.Count > 0) Then
                            val = dsData.Tables("data").Rows(0).Item(ds.Rows(i).Item("FieldMapping"))
                        End If
                    End If
                    Select Case ds.Rows(i).Item("FieldType").ToString().ToUpper()
                        Case "TEXT BOX"
                            SBFields.Append("<input class=""form-control"" value=""" & val & """ bpminput=true key=""" & ds.Rows(i).Item("DisplayName") & """ id =""fld" & ds.Rows(i).Item("FieldID") & """ type=""text"" ")
                            'If Convert.ToString(ds.Rows(i).Item("cal_fields")) Then
                            '    SBFields.Append(" onblur=""" & ds.Rows(i).Item("cal_fields") & """")
                            'End If
                            SBFields.Append("datatype=""" & ds.Rows(i).Item("datatype") & """ mandatory=""" & ds.Rows(i).Item("isRequired") & """")
                            SBFields.Append("editable=""" & ds.Rows(i).Item("isEditable") & """ Searchable=""" & ds.Rows(i).Item("isSearch") & """ ")
                            SBFields.Append("phoneno=""" & ds.Rows(i).Item("isPhoneNo") & """  description=""" & ds.Rows(i).Item("isDescription") & """ ")
                            SBFields.Append(" total=""" & ds.Rows(i).Item("isTotal") & """ hasRule=""" & ds.Rows(i).Item("HasRule") & """ /> ")

                        Case "LOOKUP"
                            
                            SBFields.Append("<input class=""form-control"" value=""" & val & """ bpminput=true key=""" & ds.Rows(i).Item("DisplayName") & """ id =""fld" & ds.Rows(i).Item("FieldID") & """ type=""text"" ")

                            'SBFields.Append("<input class=""form-control"" bpminput=true key=""" & ds.Rows(i).Item("DisplayName") & """ id =""fld" & ds.Rows(i).Item("FieldID") & """ type=""text"" ")
                            SBFields.Append("mandatory=""" & ds.Rows(i).Item("isRequired") & """ editable=""" & ds.Rows(i).Item("isEditable") & """")
                            SBFields.Append(" Searchable=""" & ds.Rows(i).Item("isSearch") & """ phoneno=""" & ds.Rows(i).Item("isPhoneNo") & """  ")
                            SBFields.Append("description=""" & ds.Rows(i).Item("isDescription") & """  total=""" & ds.Rows(i).Item("isTotal") & """ ")
                            SBFields.Append(" hasRule=""" & ds.Rows(i).Item("HasRule") & """  />")
                        Case "DROP DOWN"
                            ''lookupvalue is not null or ddllookupvalue is not null or multilookUpVal is not null or ddllookupvalueSource is not null or HasRule='1'
                            SBFields.Append("<select class=""form-control"" bpminput=true key=""" & ds.Rows(i).Item("DisplayName") & """ id =""fld" & ds.Rows(i).Item("FieldID") & """")
                            SBFields.Append("mandatory=""" & ds.Rows(i).Item("isRequired") & """ editable=""" & ds.Rows(i).Item("isEditable") & """ ")
                            SBFields.Append("Searchable=""" & ds.Rows(i).Item("isSearch") & """ phoneno=""" & ds.Rows(i).Item("isPhoneNo") & """ ")
                            SBFields.Append(" description=""" & ds.Rows(i).Item("isDescription") & """  total=""" & ds.Rows(i).Item("isTotal") & """ ")
                            SBFields.Append("hasRule=""" & ds.Rows(i).Item("HasRule") & """ ")
                            If Convert.ToString(ds.Rows(i).Item("lookupvalue")) <> "" Then
                                SBFields.Append("allowfilter=""1"" ")
                            End If
                            SBFields.Append(">" & CreateDDLString(ds.Rows(i), ds, val) & "</select>")
                        Case "TEXT AREA"
                            SBFields.Append("<input class=""form-control"" value=""" & val & """ bpminput=true key=""" & ds.Rows(i).Item("DisplayName") & """ id =""fld" & ds.Rows(i).Item("FieldID") & """ type=""text"" ")
                            'SBFields.Append("<input class=""form-control"" bpminput=true key=""" & ds.Rows(i).Item("DisplayName") & """ id =""fld" & ds.Rows(i).Item("FieldID") & """ type=""text"" ")
                            SBFields.Append("mandatory=""" & ds.Rows(i).Item("isRequired") & """ editable=""" & ds.Rows(i).Item("isEditable") & """ ")
                            SBFields.Append("Searchable=""" & ds.Rows(i).Item("isSearch") & """ phoneno=""" & ds.Rows(i).Item("isPhoneNo") & """ ")
                            SBFields.Append(" description=""" & ds.Rows(i).Item("isDescription") & """  total=""" & ds.Rows(i).Item("isTotal") & """ ")
                            SBFields.Append("hasRule=""" & ds.Rows(i).Item("HasRule") & """ /> ")
                        Case "CHECK BOX"
                            If (val <> "") Then
                                SBFields.Append("<input class=""form-control"" style='width: 16px;' checked bpminput=true key=""" & ds.Rows(i).Item("DisplayName") & """ id =""fld" & ds.Rows(i).Item("FieldID") & """ type=""checkbox"" ")
                            Else
                                SBFields.Append("<input class=""form-control"" style='width: 16px;' bpminput=true key=""" & ds.Rows(i).Item("DisplayName") & """ id =""fld" & ds.Rows(i).Item("FieldID") & """ type=""checkbox"" ")
                            End If
                            SBFields.Append(" mandatory=""" & ds.Rows(i).Item("isRequired") & """ editable=""" & ds.Rows(i).Item("isEditable") & """ ")
                            SBFields.Append("Searchable=""" & ds.Rows(i).Item("isSearch") & """ phoneno=""" & ds.Rows(i).Item("isPhoneNo") & """ ")
                            SBFields.Append(" description=""" & ds.Rows(i).Item("isDescription") & """  total=""" & ds.Rows(i).Item("isTotal") & """ ")
                            SBFields.Append("hasRule=""" & ds.Rows(i).Item("HasRule") & """   />")
                        Case "CHECKBOX LIST"
                            'SBFields.Append(CreateCheckBoxListString(ds.Rows(i), ds))
                            Dim dt As New DataTable()
                            dt = ControlUtils.GetCheckBoxListData(ds.Rows(i), 0)
                            SBFields.Append("<div style='overflow:scroll; width: 274px; height:120px;'>")
                            Dim arr As String()
                            If (val <> "") Then
                                arr = val.Split(","c)
                            End If
                            For p As Integer = 0 To dt.Rows.Count - 1
                                If (p = 0) Then
                                    SBFields.Append("<div style='float:left;'><input class=""form-control"" style='width: 16px;' bpminput=true key=""" & ds.Rows(i).Item("DisplayName").ToString() & """ name=""chkCheckBoxList"" value=""" & dt.Rows(p).Item("Text").ToString() & """ id =""fld" & dt.Rows(p).Item("tid").ToString() & """ type=""checkbox""")
                                    SBFields.Append(" mandatory=""" & ds.Rows(i).Item("isRequired") & """ editable=""" & ds.Rows(i).Item("isEditable") & """ ")
                                    SBFields.Append("Searchable=""" & ds.Rows(i).Item("isSearch") & """ phoneno=""" & ds.Rows(i).Item("isPhoneNo") & """ ")
                                    SBFields.Append(" description=""" & ds.Rows(i).Item("isDescription") & """  total=""" & ds.Rows(i).Item("isTotal") & """ ")
                                    SBFields.Append("hasRule=""" & ds.Rows(i).Item("HasRule") & """   ")
                                    If (val <> "") Then
                                        For q As Integer = 0 To arr.Length - 1
                                            If (arr(q) = dt.Rows(p).Item("tid").ToString()) Then
                                                SBFields.Append(" checked ")
                                            End If
                                        Next
                                    End If
                                    SBFields.Append(">")
                                    SBFields.Append("<label style='width: 208px; padding-left: 5px;' for=""fld" & dt.Rows(p).Item("tid").ToString() & """>" & dt.Rows(p).Item("Text").ToString() & "</label> </div>")
                                Else
                                    SBFields.Append("<div style='float:left;'><input class=""form-control"" style='width: 16px;' key=""" & ds.Rows(i).Item("DisplayName").ToString() & """ name=""chkCheckBoxList"" value=""" & dt.Rows(p).Item("Text").ToString() & """ id =""fld" & dt.Rows(p).Item("tid").ToString() & """ type=""checkbox""")
                                    If (val <> "") Then
                                        For q As Integer = 0 To arr.Length - 1
                                            If (arr(q) = dt.Rows(p).Item("tid").ToString()) Then
                                                SBFields.Append(" checked ")
                                            End If
                                        Next
                                    End If
                                    SBFields.Append(">")
                                    SBFields.Append("<label style='width: 208px; padding-left: 5px;' for=""fld" & dt.Rows(p).Item("tid").ToString() & """>" & dt.Rows(p).Item("Text").ToString() & "</label> </div>")
                                End If

                                'Sb.Append("<option value='" & dt.Rows(p).Item("tid").ToString() & "'>" & dt.Rows(p).Item("Text").ToString() & "</option>")
                            Next
                            SBFields.Append("</div>")
                        Case "AUTOCOMPLETE"
                            Dim Mdrop As String = ds.Rows(i).Item("dropdown")
                            Dim arr1 As String()
                            If (val <> "") Then
                                Dim dsAuto As New DataSet()
                                arr1 = Mdrop.Split("-"c)
                                strqry = "Select " & arr1(2) & " from MMM_MST_MASTER WHERE TID=" & val & ""
                                Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
                                con = New SqlConnection(conStr)
                                oda = New SqlDataAdapter("", con)
                                oda.SelectCommand.CommandText = strqry
                                oda.Fill(dsAuto, "data")
                                val = dsAuto.Tables(0).Rows(0).Item(arr1(2)).ToString()
                            End If
                            SBFields.Append("<input class=""k-text"" autocontrol=""1"" value=""" & val & """ bpminput=true key=""" & ds.Rows(i).Item("DisplayName") & """ id =""fld" & ds.Rows(i).Item("FieldID") & """ type=""AUTOCOMPLETE""  ")
                            SBFields.Append("mandatory=""" & ds.Rows(i).Item("isRequired") & """ editable=""" & ds.Rows(i).Item("isEditable") & """ ")
                            SBFields.Append("Searchable=""" & ds.Rows(i).Item("isSearch") & """ phoneno=""" & ds.Rows(i).Item("isPhoneNo") & """ ")
                            SBFields.Append(" description=""" & ds.Rows(i).Item("isDescription") & """  total=""" & ds.Rows(i).Item("isTotal") & """ ")
                            If Convert.ToString(ds.Rows(i).Item("lookupvalue")) <> "" Then
                                SBFields.Append("autofilter=""1"" ")
                            Else
                                SBFields.Append("autofilter=""0"" ")
                            End If
                            SBFields.Append(" hasRule=""" & ds.Rows(i).Item("HasRule") & """ placeholder=""Please type " & ds.Rows(i).Item("DisplayName") & """ filterDocID="""" /> ")
                            SBFields.Append("<input type='hidden' id =""hdn" & ds.Rows(i).Item("FieldID") & """ value=''")
                            'SBFields.Append("autocontrol=""1"" ")
                        Case Else
                            SBFields.Append("<input class=""form-control"" value=""" & val & """ bpminput=true key=""" & ds.Rows(i).Item("DisplayName") & """ id =""fld" & ds.Rows(i).Item("FieldID") & """ type=""text"" ")
                            SBFields.Append("mandatory=""" & ds.Rows(i).Item("isRequired") & """ editable=""" & ds.Rows(i).Item("isEditable") & """ ")
                            SBFields.Append("Searchable=""" & ds.Rows(i).Item("isSearch") & """ phoneno=""" & ds.Rows(i).Item("isPhoneNo") & """ ")
                            SBFields.Append(" description=""" & ds.Rows(i).Item("isDescription") & """  total=""" & ds.Rows(i).Item("isTotal") & """ ")
                            SBFields.Append(" hasRule=""" & ds.Rows(i).Item("HasRule") & """  /> ")
                    End Select
                    SBFields.Append("</div>")
                    If layout = "DOUBLE COLUMN" Then
                        If i Mod 2 = 1 Then
                            SBFields.Append("</div><div class=""clear""></div>")
                        End If
                    Else
                        SBFields.Append("</div><div class=""clear""></div>")
                    End If
                Next
            End If
            SB.Append(SBFields.ToString())
            If Tid > 0 Then
                SB.Append("</div><div class='clear'></div><div  style='text-align:right;'><input type='button' id='btnSave' class='btnsave' value='Update' onclick='javascript:validate();' /></div></div>")
            Else
                SB.Append("</div><div class='clear'></div><div  style='text-align:right;'><input type='button' id='btnSave' class='btnsave' value='Save' onclick='javascript:validate();' /></div></div>")
            End If
            'SB.Append("<div  style='text-align:right;'><input type='button' id='btnSave' class='btnsave' value='Save' onclick='javascript:validate();' /></div></div></div>")

            Return SB.ToString()

        Catch ex As Exception

        End Try
    End Function

    Public Shared Function GetFormFields2(EID As Integer, Optional Doctype As String = "", Optional FieldID As Integer = 0) As DataSet
        Dim dsFields As New DataSet()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim strQuery = ""
        Dim con As SqlConnection = Nothing
        Dim sda As SqlDataAdapter = Nothing
        Try
            strQuery = "SELECT FF.*,F.*  FROM MMM_MST_FIELDS FF left outer JOIN MMM_MST_FORMS F on F.FormName = FF.DocumentType and F.EID = FF.EID   where  F.EID=" & EID & " "
            If FieldID <> 0 Then
                strQuery = strQuery & " AND FF.FieldID= " & FieldID
            Else
                strQuery = strQuery & " AND FF.isactive=1 "
            End If
            If Doctype <> "" Then
                strQuery = strQuery & "and FormName = '" & Doctype & "'"
            End If
            strQuery = strQuery & "  order by displayOrder"
            con = New SqlConnection(conStr)
            con.Open()
            sda = New SqlDataAdapter(strQuery, con)
            sda.Fill(dsFields, "fields")
        Catch ex As Exception
            Throw
        Finally
            If Not con Is Nothing Then
                con.Close()
                con.Dispose()
            End If
            If Not con Is Nothing Then
                sda.Dispose()
            End If
        End Try
        Return dsFields
    End Function

    Protected Shared Function CreateDDLString(DR As DataRow, dtFields As DataTable, val As String) As String
        Dim Str As String = "<option value='0'>--Select--</option>"
        Try
            Dim Sb As New StringBuilder()
            Dim Obj As New ControlUtils()
            Dim dt As New DataTable()
            Dim uBo As New Userdetails()
            dt = ControlUtils.GetDropDownData(DR, dtFields, 0, uBo)
            For p As Integer = 0 To dt.Rows.Count - 1
                If (dt.Rows(p).Item("tid").ToString() = val) Then
                    Sb.Append("<option value='" & dt.Rows(p).Item("tid").ToString() & "' selected='true'>" & dt.Rows(p).Item("Text").ToString() & "</option>")
                Else
                    Sb.Append("<option value='" & dt.Rows(p).Item("tid").ToString() & "'>" & dt.Rows(p).Item("Text").ToString() & "</option>")
                End If
                'Sb.Append("<option value='" & dt.Rows(p).Item("tid").ToString() & "'>" & dt.Rows(p).Item("Text").ToString() & "</option>")
            Next
            Str &= Sb.ToString
            Return Str
        Catch ex As Exception
            Return Str
        End Try
    End Function
    <System.Web.Services.WebMethod()>
    Public Shared Function GetAutoCompleteValue(str As String, tid As String, DocType As String) As String
        Dim dt As New DataTable()
        Dim jsonData As String = ""
        Try
            Dim Obj As New ControlUtils()

            Dim uBo As New Userdetails()
            Dim dsFields As New DataSet()
            Dim Eid As String = HttpContext.Current.Session("EID")
            dsFields = GetFormFields2(Eid, DocType)

            Dim onlyFiltered As New DataView()
            onlyFiltered = dsFields.Tables(0).DefaultView()
            onlyFiltered.RowFilter = "Invisible=0"
            Dim ds As DataTable = onlyFiltered.Table.DefaultView.ToTable()
            onlyFiltered.RowFilter = "FieldID=" & tid & ""
            Dim DR As DataRow = onlyFiltered.Table.DefaultView.ToTable().Rows(0)

            dt = ControlUtils.GetAutoCompleteData(DR, ds, 0, uBo, str)
            jsonData = JsonConvert.SerializeObject(dt, Newtonsoft.Json.Formatting.None)
        Catch ex As Exception
            Throw
        End Try
        Return jsonData
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function FilterDataForAutoComplete(FieldId As String, DOCID As String, str As String, DocType As String) As String
        Dim jsonData As String = ""
        Dim ret As New ClsDropDownFilterdata()
        Dim dsFields As New DataSet()
        Dim Eid As String = HttpContext.Current.Session("EID")
        Dim dsData As New DataSet
        'dsFields = GetFormFields2(Eid, DocType)
        Dim con As SqlConnection = Nothing
        Dim da As SqlDataAdapter = Nothing
        Try
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim StrQuery As String = "Select * from  MMM_MST_FIELDS WHERE EID = " & Eid & " AND DocumentType = '" & DocType & "' and FieldID = " & FieldId & ""
            con = New SqlConnection(conStr)
            da = New SqlDataAdapter(StrQuery, con)
            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            da.Fill(dsFields)
            Dim ddl As String = dsFields.Tables(0).Rows(0).Item("dropdown")
            Dim field As String = dsFields.Tables(0).Rows(0).Item("FieldMapping")
            Dim arr As String()
            arr = ddl.Split("-"c)
            Dim StrDataQuery As String = "Select tid, " & arr(2) & " as Text from  MMM_MST_MASTER WHERE EID = " & Eid & " AND " & arr(2) & " like '%" & str & "%' AND DocumentType ='" & arr(1) & "' AND " & field & " = " & DOCID & ""
            da = New SqlDataAdapter(StrDataQuery, con)
            da.Fill(dsData)
            jsonData = JsonConvert.SerializeObject(dsData.Tables(0), Newtonsoft.Json.Formatting.None)
        Catch ex As Exception
            Throw
        Finally
            con.Close()
        End Try
        Return jsonData
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function FilterDropDown(FieldId As String, DOCID As String) As ClsDropDownFilterdata
        Dim ret As New ClsDropDownFilterdata()
        Dim objC As New ControlUtils()
        FieldId = FieldId.Replace("fld", String.Empty)
        Dim EID = HttpContext.Current.Session("EID")
        ret = objC.DropDownFilter(EID, FieldId, DOCID)
        Return ret
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function Save(strData As String, DocType As String, SMethd As String) As String
        Try
            Dim strResult As String = ""
            Dim obj = New UpdateData()

            If SMethd = "0" Then
                strResult = CommanUtil.ValidateParameterByDocumentType(HttpContext.Current.Session("EID"), DocType, HttpContext.Current.Session("UID"), strData)
            Else
                strResult = obj.UpdateData(HttpContext.Current.Session("EID"), DocType, HttpContext.Current.Session("UID"), strData, "EnableEdit", SMethd)
            End If
            Return strResult
        Catch ex As Exception
            Return "Error occured at server"
        End Try
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function LockRecord(pid As String, doctype As String) As String
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("uspLockMaster", con)
        oda.SelectCommand.CommandType = CommandType.StoredProcedure
        oda.SelectCommand.Parameters.AddWithValue("eid", HttpContext.Current.Session("EID"))
        oda.SelectCommand.Parameters.AddWithValue("tid", pid)
        If con.State <> ConnectionState.Open Then
            con.Open()
        End If
        Dim iSt As Integer = oda.SelectCommand.ExecuteScalar()
        con.Close()
        oda.Dispose()
        con.Dispose()
        If iSt = 0 Or iSt = 1 Then
            'Update CSV
            Dim objcsv As New UpdateSiteCsv()
            objcsv.UpdateCsv(Convert.ToInt32(pid), Convert.ToInt32(HttpContext.Current.Session("EID")), doctype)
            'Update CSV End

            'ModalPopup_Lock.Hide()
        Else
            'lblLock.Text = "Not updated"
        End If
        'updLock.Update()
        'updPnlGrid.Update()
        Return iSt
    End Function

#Region "Grid"
    Private Shared Function Sortgrid(sorting As List(Of SortDescriptionMaster), ds As DataSet) As String
        Dim sortingStr As String = ""
        If sorting IsNot Nothing Then
            If sorting.Count <> 0 Then
                For i As Integer = 0 To sorting.Count - 1

                    If sorting(i).field = "fdate" Or sorting(i).field = "RECEIVEDON" Then
                        sortingStr += ", d." + sorting(i).field + " " + sorting(i).dir
                    ElseIf sorting(i).field = "P_Days" Or sorting(i).field = "PENDINGDAYS" Then
                        sortingStr += ", datediff(day,fdate,getdate()) " + sorting(i).dir
                    Else
                        'sortingStr += ", M." + sorting(i).field + " " + sorting(i).dir

                        If sorting(i).field <> "Status" Then
                            Dim table = ds.Tables(0).AsEnumerable().Where(Function(r) r.Field(Of String)("FieldMapping") = sorting(i).field).AsDataView().ToTable()
                            If table.Rows(0)("dropdowntype") = "MASTER VALUED" Then
                                sortingStr += ", dms.udf_split('" & table.Rows(0)("DROPDOWN") & "', " & sorting(i).field & ")" + " " + sorting(i).dir
                            Else
                                sortingStr += ", M." + sorting(i).field + " " + sorting(i).dir
                            End If
                        Else
                            sortingStr += ", isauth" + " " + sorting(i).dir
                            'filters += filter.filters(i).field + condition
                        End If


                    End If

                Next
            End If
        End If
        Return sortingStr
    End Function

    Private Shared Function filtergrid(filter As FilterContainerMaster, ds As DataSet) As String
        Dim filters As String = ""
        Dim logic As String
        Dim condition As String = ""
        Dim c As Integer = 1
        If filter IsNot Nothing Then
            For i As Integer = 0 To filter.filters.Count - 1
                logic = filter.logic
                If filter.filters(i).[operator] = "eq" Then
                    'If (filter.filters(i).field = "fdate" Or filter.filters(i).field = "fld43" Or filter.filters(i).field = "RECEIVEDON") Then
                    '    Dim [date] As DateTime = DateTime.Parse(filter.filters(i).value)
                    '    Dim dateInString As [String] = [date].ToString("yyyy-MM-dd")
                    '    condition = " = CAST('" + dateInString + "' AS DATE) "
                    If filter.filters(i).type = "Datetime" Then
                        Dim [date] As DateTime = DateTime.Parse(filter.filters(i).value)
                        Dim dateInString As [String] = [date].ToString("yyyy-MM-dd")
                        condition = " = CAST('" + dateInString + "' AS DATE) "
                    Else
                        If filter.filters(i).field = "Status" Then
                            If (filter.filters(i).value = "ACTIVE") Then
                                condition = " = '1' "
                            Else
                                condition = " = '0' "
                            End If
                        Else
                            condition = " = '" + filter.filters(i).value + "' "
                        End If
                    End If

                End If
                If filter.filters(i).[operator] = "neq" Then
                    If filter.filters(i).type = "Datetime" Then
                        Dim [date] As DateTime = DateTime.Parse(filter.filters(i).value)
                        Dim dateInString As [String] = [date].ToString("yyyy-MM-dd")
                        condition = " != CAST('" + dateInString + "' AS DATE) "
                    Else
                        'condition = " != '" + filter.filters(i).value + "' "

                        If filter.filters(i).field = "Status" Then
                            If (filter.filters(i).value = "ACTIVE") Then
                                condition = " != '" + 1 + "' "
                            Else
                                condition = " != '" + 0 + "' "
                            End If
                        Else
                            condition = " != '" + filter.filters(i).value + "' "
                        End If
                    End If
                    'condition = " != '" + filter.filters(i).value + "' "
                End If
                If filter.filters(i).[operator] = "startswith" Then
                    If filter.filters(i).type = "Datetime" Then
                        Dim [date] As DateTime = DateTime.Parse(filter.filters(i).value)
                        Dim dateInString As [String] = [date].ToString("yyyy-MM-dd")
                        condition = " Like CAST('" + dateInString + "' AS DATE) %"
                    Else
                        'condition = " Like '" + filter.filters(i).value + "%' "

                        If filter.filters(i).field = "Status" Then
                            If (filter.filters(i).value = "ACTIVE") Then
                                condition = " Like '" + 1 + "' "
                            Else
                                condition = " Like '" + 0 + "' "
                            End If
                        Else
                            condition = " Like '" + filter.filters(i).value + "%' "
                        End If
                    End If

                    'condition = " Like '" + filter.filters(i).value + "%' "
                End If
                If filter.filters(i).[operator] = "contains" Then
                    'condition = " Like '%" + filter.filters(i).value + "%' "
                    If filter.filters(i).field = "Status" Then
                        If (filter.filters(i).value = "ACTIVE") Then
                            condition = " Like '%" + 1 + "%' "
                        Else
                            condition = " Like '%" + 0 + "%' "
                        End If
                    Else
                        condition = " Like '%" + filter.filters(i).value + "%' "
                    End If
                End If
                'End If
                If filter.filters(i).[operator] = "doesnotcontains" Then
                    condition = " Not Like '%" + filter.filters(i).value + "%' "
                End If
                If filter.filters(i).[operator] = "endswith" Then
                    condition = " Like '%" + filter.filters(i).value + "' "
                End If
                If filter.filters(i).[operator] = "gte" Then
                    condition = " >= '" + filter.filters(i).value + "' "
                End If
                If filter.filters(i).[operator] = "gt" Then
                    condition = " > '" + filter.filters(i).value + "' "
                End If
                If filter.filters(i).[operator] = "lte" Then
                    condition = " <= '" + filter.filters(i).value + "' "
                End If
                If filter.filters(i).[operator] = "lt" Then
                    condition = "< '" + filter.filters(i).value + "' "
                End If

                If filter.filters(i).field = "P_Days" Or filter.filters(i).field = "PENDINGDAYS" Then
                    filters += "datediff(day,fdate,getdate()) " & condition
                ElseIf filter.filters(i).field = "fdate" Or filter.filters(i).field = "RECEIVEDON" Then
                    filters += "CAST(d.fdate AS DATE) " & condition
                ElseIf filter.filters(i).field = "fld43" Then
                    filters += "CONVERT(date,fld43,3) " & condition
                Else
                    If filter.filters(i).field <> "Status" Then
                        Dim table = ds.Tables(0).AsEnumerable().Where(Function(r) r.Field(Of String)("FieldMapping") = filter.filters(i).field).AsDataView().ToTable()
                        If table.Rows(0)("dropdowntype") = "MASTER VALUED" Then
                            filters += "dms.udf_split('" & table.Rows(0)("DROPDOWN") & "', " & filter.filters(i).field & ")" + condition
                        Else
                            filters += filter.filters(i).field + condition
                        End If
                        Else
                            filters += " isauth" + condition
                            'filters += filter.filters(i).field + condition
                    End If
                End If

                    If filter.filters.Count > c Then
                        filters += logic
                        filters += " "
                    End If
                    c += 1
            Next
        End If
        Return filters
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function GetColumn(documentType As String) As kGridMaster
        Dim ret As New kGridMaster()
        Dim dsFields As New DataSet()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim strQuery As String = ""
        Try
            ' strQuery = "SELECT top 6 displayName,FieldMapping,dropdowntype,dropdown,fieldtype,datatype FROM MMM_MST_FIELDS with(nolock) where EID=" & HttpContext.Current.Session("EID") & " and DocumentType='" & documentType & "' and isactive=1 order by displayOrder "

            If HttpContext.Current.Session("EID") = 109 Then
                strQuery = "SELECT displayName,FieldMapping,dropdowntype,dropdown,fieldtype,datatype FROM MMM_MST_FIELDS with(nolock) where EID=" & HttpContext.Current.Session("EID") & " and DocumentType='" & documentType & "' and isactive=1 order by displayOrder "
            Else
                strQuery = "SELECT top 6 displayName,FieldMapping,dropdowntype,dropdown,fieldtype,datatype FROM MMM_MST_FIELDS with(nolock) where EID=" & HttpContext.Current.Session("EID") & " and DocumentType='" & documentType & "' and isactive=1 order by displayOrder "
            End If

            dsFields = DataLibMaster.ExecuteDataSet(conStr, CommandType.Text, strQuery)
            If (dsFields.Tables(0).Rows().Count > 0) Then
                ret.Column = CreateColCollection(dsFields.Tables(0))
            End If
        Catch ex As Exception

        End Try
        Return ret
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function GetMasterData(documentType As String, page As Integer, pageSize As Integer, skip As Integer, take As Integer, sorting As List(Of SortDescriptionMaster), filter As FilterContainerMaster) As kGridMaster
        Dim dataobj As New DataLibMaster()
        Dim ret As New kGridMaster()
        Dim jsonData As String = ""
        Dim dsFields As New DataSet()
        Dim QryFields As String = ""
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim strQuery As String = ""
        Dim strQueryTotalCount As String = ""
        Dim dsData As New DataSet()
        Dim dtCount As New DataTable()
        Try
            strQuery = "SELECT displayName,FieldMapping,dropdowntype,dropdown,fieldtype,datatype FROM MMM_MST_FIELDS with(nolock) where EID=" & HttpContext.Current.Session("EID") & " and DocumentType='" & documentType & "' and isactive=1 order by displayOrder"
            dsFields = DataLibMaster.ExecuteDataSet(conStr, CommandType.Text, strQuery)

            'paging logic==================================
            Dim from1 As Integer = skip + 1 '(page - 1) * pageSize + 1;
            Dim to1 As Integer = take * page 'page * pageSize;
            Dim sortingStr As String = ""

            '===============================================
            'Sorting logic====================================
            If sorting IsNot Nothing Then
                sortingStr = Sortgrid(sorting, dsFields)
                'If sorting.Count <> 0 Then
                '    For i As Integer = 0 To sorting.Count - 1
                '        sortingStr += ", M." + sorting(i).field + " " + sorting(i).dir
                '    Next
                'End If
            End If
            '==================================================
            'filtering logic====================================
            Dim filters As String = ""
            If filter IsNot Nothing Then
                For i As Integer = 0 To filter.filters.Count - 1
                    Dim table = dsFields.Tables(0).AsEnumerable().Where(Function(r) r.Field(Of String)("FieldMapping") = filter.filters(i).field).AsDataView().ToTable()
                    If (filter.filters(i).field <> "Status") Then
                        filter.filters(i).type = table.Rows(0)("datatype")
                    End If
                Next
                filters = filtergrid(filter, dsFields)
            End If
            '==================================================

            sortingStr = sortingStr.TrimStart(","c)

            Dim SBQry As New StringBuilder()
            Dim StaticColumns As String = ""
            If sortingStr <> "" Then
                SBQry.Append("Select ROW_NUMBER() OVER (ORDER BY " & sortingStr & ") AS RowNumber,CAST(M.tid as varchar) as [DocDetID], CAST(M.tid as varchar) as [SYSTEMID], ")
            Else
                
                SBQry.Append("Select ROW_NUMBER() OVER (ORDER BY M.tid desc) AS RowNumber, CAST(M.tid as varchar) as [DocDetID], CAST(M.tid as varchar) as [SYSTEMID], ")
            End If
            'SBQry.Append("Select Top CAST(M.TID as varchar) TID,CAST(M.tid as varchar) as [SYSTEMID], ")
            'query get total count--------------------------
            'strQueryTotalCount = "SELECT COUNT(*) Total from mmm_mst_master M where documenttype='" & documentType & "' and  eid = " & Val(HttpContext.Current.Session("EID").ToString()) & ""
            'If filters <> "" Then
            '    strQueryTotalCount = strQueryTotalCount + " AND " & filters
            'Else
            '    'strQueryTotalCount = strQuery
            'End If
            '---------------------------------

            If (dsFields.Tables(0).Rows().Count > 0) Then
                For i As Integer = 0 To dsFields.Tables(0).Rows.Count - 1
                    If dsFields.Tables(0).Rows(i).Item("dropdowntype") = "MASTER VALUED" Then
                        SBQry.Append("dms.udf_split('" + dsFields.Tables(0).Rows(i).Item("dropdown") + "'," + dsFields.Tables(0).Rows(i).Item("FieldMapping") + ")" + "[" + dsFields.Tables(0).Rows(i).Item("FieldMapping") + "]").Append(",")
                    ElseIf dsFields.Tables(0).Rows(i).Item("fieldtype") = "Parent Field" Or dsFields.Tables(0).Rows(i).Item("fieldtype") = "Self Reference" Then
                        SBQry.Append("dms.udf_split('" + dsFields.Tables(0).Rows(i).Item("dropdown") + "'," + dsFields.Tables(0).Rows(i).Item("FieldMapping") + ")" + "[" + dsFields.Tables(0).Rows(i).Item("FieldMapping") + "]").Append(",")
                    Else
                        If (dsFields.Tables(0).Rows(i).Item("datatype") = "Datetime") Then
                            SBQry.Append("CONVERT(date," & dsFields.Tables(0).Rows(i).Item("FieldMapping") & ",3)" + "[" + dsFields.Tables(0).Rows(i).Item("FieldMapping") + "]").Append(",")
                        Else
                            SBQry.Append(dsFields.Tables(0).Rows(i).Item("FieldMapping")).Append(",")
                        End If
                        'SBQry.Append(dsFields.Tables(0).Rows(i).Item("FieldMapping")).Append(",")
                    End If

                Next
                strQuery = Left(SBQry.ToString(), Len(SBQry.ToString()) - 1) & ", Case isauth when 1 then 'ACTIVE' when 0 then 'INACTIVE' END Status from mmm_mst_master M with (nolock)  where M.documenttype='" & documentType & "' AND M.eid = " & Val(HttpContext.Current.Session("EID").ToString()) & ""
                strQuery = bindgridbyrole(strQuery, documentType)
                strQuery = "WITH Data AS (" & strQuery
                If filters <> "" Then
                    strQuery = strQuery + " AND " & filters
                End If
                strQueryTotalCount = strQuery + ") SELECT Count(*) Total FROM Data"

                Dim eid As String = HttpContext.Current.Session("EID")
                If eid = 98 And documentType = "Activity Master" Then
                    strQuery = strQuery + ") SELECT * FROM Data WHERE RowNumber BETWEEN " & from1 & "AND " & to1 & " order by fld1,fld112,fld108"
                Else
                    strQuery = strQuery + ") SELECT * FROM Data WHERE RowNumber BETWEEN " & from1 & "AND " & to1
                End If
                'If filters <> "" Then
                '    strQueryTotalCount = strQuery + ") SELECT Count(*) Total FROM Data WHERE " & filters & ""
                '    strQuery = strQuery + ") SELECT * FROM Data WHERE " & filters & " AND  RowNumber BETWEEN " & from1 & "AND " & to1

                'Else
                '    strQueryTotalCount = strQuery + ") SELECT Count(*) Total FROM Data"
                '    strQuery = strQuery + ") SELECT * FROM Data WHERE RowNumber BETWEEN " & from1 & "AND " & to1
                'End If
                'IF @SQLSortString IS NOT NULL
                dsData = DataLibMaster.ExecuteDataSet(conStr, CommandType.Text, strQuery)
                'ret.Column = CreateColCollection(dsFields.Tables(0))
            End If
            'dsFields = DataLibMaster.ExecuteDataSet(dataobj.conObj, CommandType.Text, QryFields)
            If dsData.Tables(0).Rows.Count > 0 Then

                'jsonData = JsonConvert.SerializeObject(dsData.Tables(0), Newtonsoft.Json.Formatting.None, serializerSettings)
                jsonData = JsonConvert.SerializeObject(dsData.Tables(0)) 'DataLibMaster.JsonTableSerializer(dsData.Tables(0))
            Else
                jsonData = JsonConvert.SerializeObject(dsData.Tables(0))
            End If
            ret.total = DataLibMaster.ExecuteDataSet(conStr, CommandType.Text, strQueryTotalCount).Tables(0).Rows(0)("Total")
            ret.Data = jsonData
            Return ret
            'Json(ret, JsonRequestBehavior.AllowGet)
            'Return dsMaster
        Catch ex As Exception
            Throw
        Finally
            'oda.Dispose()

        End Try
    End Function
    Public Shared Function bindgridbyrole(query As String, docType As String) As String
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim ds As New DataSet
        Dim da As New SqlDataAdapter("", con)
        Dim queryRole As String = ""
        Try
            Dim scrname As String = docType
            da.SelectCommand.CommandText = "select formname,docmapping from mmm_mst_forms where eid=" & HttpContext.Current.Session("EID") & " and isroledef=1 order by formname "
            da.SelectCommand.CommandType = CommandType.Text
            da.Fill(ds, "docmapping")
            Dim docmappingfld As String = String.Empty
            Dim form As String = String.Empty
            Dim fld As String = String.Empty
            Dim mainqry As String = query ' " from mmm_mst_master where eid=" & HttpContext.Current.Session("EID") & " and documenttype='" & scrname.ToString() & "'  "
            Dim qry As String = String.Empty
            Dim ischeck As String = "select docmapping from mmm_mst_Forms where eid=" & HttpContext.Current.Session("EID") & " and formname='" & scrname.ToString() & "'"
            da.SelectCommand.CommandText = ischeck
            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            Dim issc As String = da.SelectCommand.ExecuteScalar().ToString()
            If ds.Tables("docmapping").Rows.Count > 0 Then
                If Not issc Is Nothing And issc <> "" Then
                    Dim ss As String = "  and tid in (select * from inputstring(coalesce((select top 1  isnull(convert(nvarchar(max)," & issc & "),0) from mmm_ref_role_user where eid=" & HttpContext.Current.Session("EID") & " and uid=" & HttpContext.Current.Session("UID") & " and rolename='" & HttpContext.Current.Session("USERROLE") & "'),'0')))"
                    queryRole = mainqry & ss
                Else
                    For i As Integer = 0 To ds.Tables("docmapping").Rows.Count - 1
                        docmappingfld = ds.Tables("docmapping").Rows(i).Item("docmapping").ToString().Trim()
                        form = ds.Tables("docmapping").Rows(i).Item("formname").ToString().Trim()
                        da.SelectCommand.CommandText = "Select top 1 fieldmapping from mmm_mst_fields where eid=" & HttpContext.Current.Session("EID") & " and documenttype='" & scrname.ToString() & "' and dropdown like '%Master-" & form.ToString() & "-%' order by displayname"
                        If con.State <> ConnectionState.Open Then
                            con.Open()
                        End If
                        fld = da.SelectCommand.ExecuteScalar()
                        If Not fld Is Nothing Then
                            qry = qry & " " & fld & " in (select * from inputstring(coalesce((select top 1  isnull(convert(nvarchar(max)," & docmappingfld & "),0) from mmm_ref_role_user where eid=" & HttpContext.Current.Session("EID") & " and uid=" & HttpContext.Current.Session("UID") & " and rolename='" & HttpContext.Current.Session("USERROLE") & "'),'0'))) and"
                        End If
                    Next
                    If qry.Length > 10 Then
                        qry = qry.Remove(qry.Length - 3)
                    End If

                    If qry.Length > 10 Then
                        mainqry = mainqry & " and   "
                    End If
                    queryRole = mainqry & qry
                End If
            Else
                queryRole = query & " 1=2 "
            End If

            If UCase(HttpContext.Current.Session("USERROLE")) = "SU" Then
                queryRole = query
            End If
        Catch ex As Exception
        Finally
            If Not con Is Nothing Then
                con.Close()
                da.Dispose()
                con.Dispose()
            End If
            If Not da Is Nothing Then
                da.Dispose()
            End If
        End Try
        Return queryRole
    End Function

    Public Shared Function CreateColCollection(dt As DataTable) As List(Of kColumnMaster)
        Dim listcol As New List(Of kColumnMaster)()
        Dim obj As kColumnMaster
        Dim strQuery As String = ""
        Dim Ds As New DataSet()
        For p As Integer = 0 To dt.Rows.Count - 1
            obj = New kColumnMaster()
            If (dt.Rows(p).Item("FieldMapping").ToString = "datediff(day,fdate,getdate())") Then
                obj.field = "P_Days"
                obj.title = dt.Rows(p).Item("FieldName")
                'obj.type = "number"
            ElseIf (dt.Rows(p).Item("FieldMapping").ToString = "CONVERT(VARCHAR(10),M.adate,105)") Then
                obj.field = "Column2"
                obj.title = dt.Rows(p).Item("FieldName")
                'obj.type = "string"
                'obj.format = "{0:dd/MM/yy}"
            Else
                obj.field = dt.Rows(p).Item("FieldMapping").ToString.Replace("M.", "").Replace("d.", "").Replace("U.", "").Replace("m.", "")

                obj.title = dt.Rows(p).Item("displayName")
                'obj.type = "string"
            End If

            If (dt.Rows(p).Item("datatype")) = "Numeric" Then
                obj.type = "string"
                obj.width = 120
                'obj.filterable = "{ui: function (element) {element.kendoNumericTextBox({format: 'n0'});}}"

            ElseIf (dt.Rows(p).Item("datatype")) = "Datetime" Then
                obj.type = "date"
                obj.format = "{0:dd/MM/yyyy}"
            Else
                obj.type = "string"
            End If
            'obj.width = 100

            listcol.Add(obj)
        Next
        obj = New kColumnMaster()
        obj.field = "Status"
        obj.title = "Status"
        listcol.Add(obj)
        Return listcol
    End Function

#End Region
End Class

#Region "External Class"
Public Class SortDescriptionMaster
    Public field As String = ""
    Public dir As String = ""

End Class
Public Class FilterContainerMaster
    Public Property filters() As List(Of FilterDescriptionMaster)
    Public logic As String = ""
End Class

Public Class FilterDescriptionMaster
    Public [operator] As String = ""
    Public field As String = ""
    Public value As String = ""
    Public type As String = ""

End Class
Public Class kGridMaster
    Public Data As String = ""
    Public Count As String = ""
    Public total As Integer = 0
    Public Column As New List(Of kColumnMaster)
End Class
Public Class kColumnMaster
    Public Sub New()

    End Sub
    Public Sub New(staticfield As [String], statictitle As [String], statictype As String, staticFormat As String)
        field = staticfield
        title = statictitle
        type = statictype
        format = staticFormat
        If (statictype = "number") Then
            filterable = ""
        End If
        'width = staticwidth
    End Sub

    Public field As String = ""
    Public title As String = ""
    Public width As Integer = 190
    Public format As String = ""
    Public filterable As String = ""
    'Public locked As Boolean = True
    'Public locked As Boolean = True
    Public type As String = ""
    Public FieldID As String = ""


End Class

#End Region
