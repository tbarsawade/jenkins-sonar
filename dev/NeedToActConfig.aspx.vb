Imports System.Web.Services
Imports System.Data
Imports System.Data.SqlClient

Partial Class NeedToActConfig
    Inherits System.Web.UI.Page
    Private Shared PageSize As Integer = 10

    Protected Sub Page_Init(sender As Object, e As EventArgs) Handles Me.Init
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
        If Not IsPostBack Then
            BindDummyRow()

        End If
    End Sub

    Private Sub BindDummyRow()
        Dim dummy As New DataTable()
        dummy.Columns.Add("TID")
        dummy.Columns.Add("DocType")
        dummy.Columns.Add("Type")
        dummy.Columns.Add("FieldName")
        dummy.Columns.Add("DisplayOrder")
        dummy.Columns.Add("Action")
        dummy.Rows.Add()
        gvCustomers.DataSource = dummy
        gvCustomers.DataBind()
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
    <WebMethod()> _
    Public Shared Function BindDocumentFieldName(ByVal documentType As String, ByVal OptionType As String) As DocumentFieldsName()
        Dim dt As New DataTable()
        Dim Fielddetails As New List(Of DocumentFieldsName)()
        If OptionType = 2 Then
            Dim constr As String = ConfigurationManager.ConnectionStrings("constr").ConnectionString
            Using con As New SqlConnection(constr)
                Using cmd As New SqlCommand("select displayname,fieldmapping from MMM_MST_FIELDS where documenttype='" & documentType.ToString() & "' and eid=" & HttpContext.Current.Session("EID"), con)
                    con.Open()
                    Dim da As New SqlDataAdapter(cmd)
                    da.Fill(dt)
                    For Each dtrow As DataRow In dt.Rows
                        Dim document As New DocumentFieldsName()
                        document.DocumentFieldsText = dtrow("displayname").ToString()
                        document.DocumentFieldsValue = dtrow("fieldmapping").ToString()
                        Fielddetails.Add(document)
                    Next
                End Using
            End Using
        End If

        Return Fielddetails.ToArray()
    End Function

    <WebMethod()> _
    Public Shared Function BindDocumentDropdown() As DocumentDetails()
        Dim dt As New DataTable()
        Dim details As New List(Of DocumentDetails)()
        Dim constr As String = ConfigurationManager.ConnectionStrings("constr").ConnectionString
        Using con As New SqlConnection(constr)
            Using cmd As New SqlCommand("select FormName from mmm_mst_forms where eid=" & HttpContext.Current.Session("EID") & " and isactive=1 and FormSource='Menu Driven' and FormType='Document' order by FormName", con)
                con.Open()
                Dim da As New SqlDataAdapter(cmd)
                da.Fill(dt)
                For Each dtrow As DataRow In dt.Rows
                    Dim document As New DocumentDetails()
                    document.DocumentName = dtrow("FormName").ToString()
                    details.Add(document)
                Next
            End Using
        End Using
        Return details.ToArray()
    End Function
    <WebMethod()> _
    Public Shared Function SaveData(ByVal DocType As String, ByVal OpType As String, ByVal Fieldval As String, ByVal DispText As String, ByVal DispOrder As Integer) As String
        Dim SNAC As New SaveNeedToActConfig()
        SNAC.DocumentType = DocType
        SNAC.OperationType = OpType
        SNAC.FieldName = DispText
        SNAC.Fieldval = Fieldval
        SNAC.DisplayOrder = DispOrder
        Try
            Dim constr As String = ConfigurationManager.ConnectionStrings("constr").ConnectionString
            Using con As New SqlConnection(constr)
                Using cmd As New SqlCommand("insert into mmm_mst_needtoact_config(EID,DocType,Type,FieldName,DisplayOrder,FieldMapping) values(" & HttpContext.Current.Session("EID") & ",'" & SNAC.DocumentType & "','" & SNAC.OperationType & "','" & SNAC.FieldName & "'," & SNAC.DisplayOrder & ",'" & SNAC.Fieldval & "')", con)
                    con.Open()
                    Dim da As New SqlDataAdapter(cmd)
                    da.SelectCommand.ExecuteNonQuery()
                    Return "Your Configuration Successfully Saved"
                End Using
            End Using
        Catch ex As Exception
            Return "There were some error please try again or contact to admin"
        End Try
    End Function

    <WebMethod()> _
    Public Shared Function GetConfigs(pageIndex As Integer) As String
        Dim query As String = "[GetNeedToActConfig_Pager]"
        Dim cmd As New SqlCommand(query)
        cmd.CommandType = CommandType.StoredProcedure
        cmd.Parameters.AddWithValue("@PageIndex", pageIndex)
        cmd.Parameters.AddWithValue("@PageSize", "200")
        cmd.Parameters.AddWithValue("@EID", HttpContext.Current.Session("EID"))
        cmd.Parameters.Add("@RecordCount", SqlDbType.Int, 4).Direction = ParameterDirection.Output
        Return GetData(cmd, pageIndex).GetXml()
    End Function
    Private Shared Function GetData(cmd As SqlCommand, pageIndex As Integer) As DataSet
        Dim strConnString As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Using con As New SqlConnection(strConnString)
            Using sda As New SqlDataAdapter()
                cmd.Connection = con
                sda.SelectCommand = cmd
                Using ds As New DataSet()
                    sda.Fill(ds, "Customers")
                    Dim dt As New DataTable("Pager")
                    dt.Columns.Add("PageIndex")
                    dt.Columns.Add("PageSize")
                    dt.Columns.Add("RecordCount")
                    dt.Rows.Add()
                    dt.Rows(0)("PageIndex") = pageIndex
                    dt.Rows(0)("PageSize") = PageSize
                    dt.Rows(0)("RecordCount") = cmd.Parameters("@RecordCount").Value
                    ds.Tables.Add(dt)
                    Return ds
                End Using
            End Using
        End Using
    End Function

    <WebMethod()> _
    Public Shared Sub DeleteCustomer(customerId As Integer)
        Dim constr As String = ConfigurationManager.ConnectionStrings("constr").ConnectionString
        Using con As New SqlConnection(constr)
            Using cmd As New SqlCommand("DELETE FROM mmm_mst_needtoact_config WHERE TID = @CustomerId")
                cmd.Parameters.AddWithValue("@CustomerId", customerId)
                cmd.Connection = con
                con.Open()
                cmd.ExecuteNonQuery()
                con.Close()
            End Using
        End Using
    End Sub

    <WebMethod()> _
    Public Shared Sub UPCustomer(ByVal CurrentId As String, ByVal ParrentId As String)

        If Convert.ToString(CurrentId).Contains("UP") Then
            CurrentId = CurrentId.Substring(2)
            ParrentId = ParrentId.Substring(2)
            If ParrentId.Length < 1 Then
                Exit Sub
            End If
        Else
            CurrentId = CurrentId.Substring(4)
            ParrentId = ParrentId.Substring(4)
            If ParrentId.Length < 1 Then
                Exit Sub
            End If
        End If
        Dim constr As String = ConfigurationManager.ConnectionStrings("constr").ConnectionString
        Using con As New SqlConnection(constr)
            Using cmd As New SqlCommand("")
                cmd.Parameters.Clear()
                cmd.CommandType = CommandType.StoredProcedure
                cmd.CommandText = "uspNeedToActPositionUpdate"
                cmd.Parameters.AddWithValue("@tid", CurrentId)
                cmd.Parameters.AddWithValue("@ntid", ParrentId)
                cmd.Connection = con
                con.Open()
                cmd.ExecuteNonQuery()
                con.Close()
            End Using
        End Using
    End Sub

End Class

Public Class DocumentFieldsName
    Public DocumentFieldsName()
    Public Property DocumentFieldsText() As String
        Get
            Return m_DocumentFieldName
        End Get
        Set(ByVal value As String)
            m_DocumentFieldName = value
        End Set
    End Property
    Public Property DocumentFieldsValue() As String
        Get
            Return m_DocumentFieldvalue
        End Get
        Set(ByVal value As String)
            m_DocumentFieldvalue = value
        End Set
    End Property
    Private m_DocumentFieldName As String
    Private m_DocumentFieldvalue As String
End Class
Public Class DocumentDetails
    Public DocumentDetails()
    Public Property DocumentName() As String
        Get
            Return m_DocumentName
        End Get
        Set(ByVal value As String)
            m_DocumentName = value
        End Set
    End Property
    Private m_DocumentName As String

End Class

Public Class SaveNeedToActConfig
    Public Property DocumentType() As String
        Get
            Return _Documenttype
        End Get
        Set(value As String)
            _Documenttype = value
        End Set
    End Property
    Private _Documenttype As String
    Public Property OperationType() As String
        Get
            Return _Operationtype
        End Get
        Set(value As String)
            _Operationtype = value
        End Set
    End Property
    Private _Operationtype As String
    Public Property FieldName() As String
        Get
            Return _FieldName
        End Get
        Set(value As String)
            _FieldName = value
        End Set
    End Property
    Private _FieldName As String
    Public Property DisplayOrder() As String
        Get
            Return _DisplayOrder
        End Get
        Set(value As String)
            _DisplayOrder = value
        End Set
    End Property
    Private _DisplayOrder As String
    Public Property Fieldval() As String
        Get
            Return _FieldVal
        End Get
        Set(value As String)
            _FieldVal = value
        End Set
    End Property
    Private _FieldVal As String
End Class




