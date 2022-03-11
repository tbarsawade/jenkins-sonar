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
Partial Class FileExplorerEcompliance
    Inherits System.Web.UI.Page
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            BindTreeView()
        End If

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



    Sub BindTreeView()
        Try
            ' mask.Visible = True
            Dim firstnode As New TreeNode
            firstnode.Text = "<b > Documents: </b>"
            tvData.Nodes.Add(firstnode)
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim dtComp As New DataTable()
        Dim oda As SqlDataAdapter = New SqlDataAdapter("select tid,fld1 from mmm_mst_master where eid =98 and documenttype ='Company Master'", con)
        Using con
            oda.SelectCommand.CommandType = CommandType.Text
            oda.Fill(dtComp)
        End Using
        For i As Integer = 0 To dtComp.Rows.Count - 1
                Dim node = New TreeNode
                Dim tid = dtComp.Rows(i)("tid").ToString()
                addChildNodesCompany(node, tid)
                node.Text = "<a style =""text-decoration:none;cursor:pointer;"" onclick=""onSelect('" & tid & "','','','','')"">" & dtComp.Rows(i)("fld1").ToString() & "<a>"
                ' node.Text = dtComp.Rows(i)("fld1").ToString()
                node.Value = tid

            tvData.Nodes.Add(node)
            Next

            tvData.CollapseAll()

        Catch ex As Exception
        Finally
            '  mask.Visible = False


        End Try
        'node.Text = "pallavi"


        'tvData.Nodes.Add(node)

        'node.ChildNodes.Add(New TreeNode("Saurabh"))


    End Sub


    Sub addChildNodesCompany(node As TreeNode, compid As String)
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim dtSite As New DataTable()
        Dim oda As SqlDataAdapter = New SqlDataAdapter("Select tid,fld100 from mmm_mst_master where eid =98 and documenttype ='Site Master' and fld1 ='" & compid & "' ", con)
        Using con
            oda.SelectCommand.CommandType = CommandType.Text
            oda.Fill(dtSite)
        End Using
        If (dtSite.Rows.Count > 0) Then


            For i = 0 To dtSite.Rows.Count - 1
                Dim nodesite As New TreeNode()
                Dim tid = dtSite.Rows(i)("tid").ToString()
                addChildNodesSite(nodesite, tid, compid)
                nodesite.Text = "<a style =""text-decoration:none;cursor:pointer;"" onclick=""onSelect('" & compid & "','" & tid & "','','','')"">" & dtSite.Rows(i)("fld100").ToString() & "<a>"
                nodesite.Value = dtSite.Rows(i)("tid")
                node.ChildNodes.Add(nodesite)
            Next
        Else
            addChildNodesAct(node, compid, "", "")

        End If

    End Sub

    Sub addChildNodesSite(node As TreeNode, siteid As String, compid As String)

        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim dtAct As New DataTable()
        Dim oda As SqlDataAdapter = New SqlDataAdapter("Select tid,fld4 from mmm_mst_master where eid =98 and documenttype ='Acts Applicable to site' and fld1 ='" & siteid & "' and fld5 ='" & compid & "'", con)
        Using con
            oda.SelectCommand.CommandType = CommandType.Text
            oda.Fill(dtAct)
        End Using

        For i = 0 To dtAct.Rows.Count - 1
            Dim nodeAct As New TreeNode()
            Dim tid = dtAct.Rows(i)("tid").ToString()
            addChildNodesAct(nodeAct, compid, siteid, tid)
            nodeAct.Text = "<a style =""text-decoration:none;cursor:pointer;"" onclick=""onSelect('" & compid & "','" & siteid & "','" & tid & "','','')"">" & dtAct.Rows(i)("fld4").ToString() & "<a>"
            nodeAct.Value = tid
            node.ChildNodes.Add(nodeAct)
        Next

    End Sub

    Sub addChildNodesAct(node As TreeNode, compid As String, siteid As String, actid As String)

        For i = 2015 To DateTime.Now.Year

            Dim childnode = New TreeNode()
            addChildNodesYear(childnode, compid, siteid, actid, i)
            childnode.Text = "<a style =""text-decoration:none;cursor:pointer;"" onclick=""onSelect('" & compid & "','" & siteid & "','" & actid & "','" & i.ToString() & "','')"">" & i.ToString() & "<a>"
            childnode.Value = i.ToString()

            node.ChildNodes.Add(childnode)
        Next


    End Sub

    Sub addChildNodesYear(node As TreeNode, compid As String, siteid As String, actid As String, year As Integer)
        Dim monthlimit As Integer = 12

        If year = DateTime.Now.Year Then
            monthlimit = DateTime.Now.Month
        End If

        For i = 1 To monthlimit

            Dim childnodemonth = New TreeNode()
            childnodemonth.Text = "<a style =""text-decoration:none;cursor:pointer;"" onclick=""onSelect('" & compid & "','" & siteid & "','" & actid & "','" & year & "','" & i.ToString() & "')"">" & GetMonth(i) & "<a>"
            childnodemonth.Value = i.ToString()
            node.ChildNodes.Add(childnodemonth)
        Next


    End Sub

   


    <System.Web.Services.WebMethod()>
    Public Shared Function GetData() As treeList

        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim sStatement As String = ""
        Dim qrycolumn As String = ""
        Dim jsonString As String = ""
        Dim ds As New DataSet()
        Dim dtComp As New DataTable()

        Dim objret As New treeList()
        Dim objChild As treeClass
        Dim lstData As New List(Of treeClass)
        Dim item As List(Of treeClass)
        'sStatement = "Select   distinct Top 50 M.tid[SYSTEM ID], M.DocumentType[SUBJECT],M.curstatus[STATUS],U.Username[CREATED BY],(replace(convert(char(11),d.fdate,113),' ','-') + ' ' + left(convert(varchar(13),d.fdate,108),8))[RECEIVED ON],datediff(day,fdate,getdate())[PENDING DAYS],M.PRIORITY  ,"
        qrycolumn = "select top 2 tid,fld1 from mmm_mst_master where eid =98 and documenttype ='Company Master'"
        Dim oda As SqlDataAdapter = New SqlDataAdapter(qrycolumn, con)
        Try
            Dim dtCompany As New DataTable()

            Using con
                oda.SelectCommand.CommandType = CommandType.Text
                oda.Fill(dtCompany)
            End Using

            For i As Integer = 0 To dtCompany.Rows.Count - 1
                Dim whereConditionComp As String = ""
                objChild = New treeClass()
                objChild.text = dtCompany.Rows(i)("fld1")
                whereConditionComp &= " and fld5 = '" + Convert.ToString(dtCompany.Rows(i)("tid").ToString()) + "'"
                objChild.WhereCondition = whereConditionComp
                item = New List(Of treeClass)
                Dim con1 As New SqlConnection(conStr)
                Dim oda1 = New SqlDataAdapter("Select tid,fld100 from mmm_mst_master where eid =98 and documenttype ='Site Master' and fld1 ='" & dtCompany.Rows(i)("tid").ToString() & "' ", con1)
                Dim dtSite As New DataTable()
                Using con1
                    oda1.SelectCommand.CommandType = CommandType.Text
                    oda1.Fill(dtSite)
                End Using

                For j As Integer = 0 To dtSite.Rows.Count - 1
                    Dim whereConditionSite As String = ""
                    Dim objchildsite As New treeClass()
                    objchildsite.text = dtSite.Rows(j)("fld100")
                    whereConditionSite &= whereConditionComp & " and fld7 ='" + dtSite.Rows(j)("tid").ToString() + "' "
                    objchildsite.WhereCondition = whereConditionSite
                    Dim itemsite As New List(Of treeClass)

                    Dim con2 As New SqlConnection(conStr)
                    oda1 = New SqlDataAdapter("Select tid,fld4 from mmm_mst_master where eid =98 and documenttype ='Acts Applicable to site' and fld1 ='" & dtSite.Rows(j)("tid").ToString() & "' ", con2)
                    Dim dtAct As New DataTable()
                    Using con2
                        oda1.SelectCommand.CommandType = CommandType.Text
                        oda1.Fill(dtAct)
                    End Using

                    For k As Integer = 0 To dtAct.Rows.Count - 1
                        Dim whereConditionAct As String = ""
                        Dim objchildAct As New treeClass()
                        objchildAct.text = dtAct.Rows(k)("fld4")
                        whereConditionAct &= whereConditionSite & " and fld2='" + dtAct.Rows(k)("tid").ToString() + "'"
                        objchildAct.WhereCondition = whereConditionAct

                        Dim itemAct As New List(Of treeClass)

                        For l As Integer = 2015 To DateTime.Now.Year
                            Dim whereConditionYear As String = ""
                            Dim objchildyear As New treeClass()
                            objchildyear.text = l.ToString()
                            whereConditionYear &= whereConditionAct & " and year(adate)=" + l.ToString()
                            objchildyear.WhereCondition = whereConditionYear
                            Dim itemYear As New List(Of treeClass)
                            Dim monthlimit As Integer = 12

                            If l = DateTime.Now.Year Then
                                monthlimit = DateTime.Now.Month
                            End If
                            For m As Integer = 1 To monthlimit
                                Dim whereConditionMonth As String = ""
                                Dim objchildlastnode As New treeClass()
                                objchildlastnode.text = GetMonth(m)
                                whereConditionMonth &= whereConditionYear & " and Month (adate) = " + m.ToString()
                                objchildlastnode.WhereCondition = whereConditionMonth
                                itemYear.Add(objchildlastnode)
                            Next
                            objchildyear.items = itemYear
                            itemAct.Add(objchildyear)
                        Next

                        objchildAct.items = itemAct
                        itemsite.Add(objchildAct)
                    Next

                    objchildsite.items = itemsite
                    item.Add(objchildsite)
                Next

                objChild.items = item
                lstData.Add(objChild)
            Next
            objret.items = lstData
            'jsonString = JsonConvert.SerializeObject(objChild)

        Catch ex As Exception
        Finally
            con.Close()
            oda.Dispose()
        End Try
        Return objret
    End Function


    Shared Function GetMonth(MonthNm As Integer) As String
        Select Case MonthNm
            Case 1
                Return "JAN"
            Case 2
                Return "FEB"
            Case 3
                Return "MAR"
            Case 4
                Return "APR"
            Case 5
                Return "MAY"
            Case 6
                Return "JUN"
            Case 7
                Return "JUL"
            Case 8
                Return "AUG"
            Case 9
                Return "SEP"
            Case 10
                Return "OCT"
            Case 11
                Return "NOV"
            Case 12
                Return "DEC"
            Case Else
                Return 1
        End Select

    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function GetDocument(CompanyID As String, siteID As String, actid As String, year As String, month As String) As String
        Dim condition As String = ""

        If (CompanyID <> "") Then
            'condition &= " and fld5='" & CompanyID & "'"
            condition &= " and fld2='" & CompanyID & "'"
        End If
        If (siteID <> "") Then
            ' condition &= " and fld7='" & CompanyID & "'"
            condition &= " and fld3='" & siteID & "'"
        End If
        If (actid <> "") Then
            ' condition &= " and fld2 ='" & actid & "'"
            condition &= " and fld4 ='" & actid & "'"
        End If
        If (year <> "") Then
            ' condition &= " and year(adate) ='" & year & "'"
            condition &= " and year(adate) ='" & year & "'"
        End If
        If (month <> "") Then
            'condition &= " and month(adate) ='" & month & "'"
            condition &= " and month(adate) ='" & month & "'"
        End If
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim qrycolumn As String = ""
        Dim jsonString As String = ""
        Dim ds As New DataSet()
        ' qrycolumn = "Select M.Eid, M.TID as [SYSTEM ID],M.TID As [DocID] ,M.fld29 As [COMPANY] ,M.fld30 As [Site] ,M.fld31 As [Act] ,M.fld47 As [PE Activity ] ,M.fld46 As [Contractor Activity ] ,M.fld44 As [Contractor Name] ,M.curstatus As [Status]  ,M.fld43 As [Expiry Date] ,ISNULL(M.fld10,'') AS [FileUpload1],ISNULL(M.fld11,'') AS [FileUpload2],ISNULL(M.fld12,'') AS [FileUpload3],ISNULL(M.fld13,'') AS [FileUpload4],ISNULL(M.fld14,'') AS [FileUpload5],CASE WHEN ISNULL(M.fld25,'') ='CONTRACTOR' THEN M.fld46 ELSE M.fld47 END AS [Activity],CONVERT(VARCHAR,M.aDate,106) AS CreationDate from MMM_MST_DOC M with (nolock)  where m.eid =98  " & condition
        qrycolumn = "select dms.udf_split('MASTER-Company Master-fld1',fld2) [COMPANY],dms.udf_split('MASTER-Site Master-fld100',fld3)[Site],dms.udf_split('MASTER-Acts Applicable To Site-fld4',fld4) [Act],CONVERT(VARCHAR,aDate,106) AS CreationDate ,ISNULL(fld7,'') AS [File1],ISNULL(fld8,'') AS [File1D],ISNULL(fld9,'') AS [File2],ISNULL(fld10,'') AS [File2D],ISNULL(fld11,'') AS [File3],ISNULL(fld12,'') AS [File3D],ISNULL(fld13,'') AS [File4],ISNULL(fld14,'') AS [File4D],ISNULL(fld15,'') AS [File5] ,ISNULL(fld16,'') AS [File5D]  from mmm_mst_doc where eid =98 and documenttype ='file upload document' " & condition
        Dim oda As SqlDataAdapter = New SqlDataAdapter(qrycolumn, con)
        Try
            oda.SelectCommand.CommandType = CommandType.Text
            oda.Fill(ds, "data")
            jsonString = JsonConvert.SerializeObject(ds)
        Catch ex As Exception

        Finally
            con.Close()
            oda.Dispose()
        End Try
        Return jsonString
    End Function

   
End Class

'Create User define class.
Public Class parrentNode
    Public text As String = ""
End Class
Public Class treeClass
    Public text As String = ""
    Public WhereCondition As String = ""
    Public Tid As Integer
    Public items As New List(Of treeClass)
End Class
Public Class treeClasslastnode
    Public text As String = ""
    Public items As New List(Of items)
End Class
Public Class items
    Public Tid As Integer
    Public text As String = ""
End Class
Public Class treeList
    Public items As New List(Of treeClass)

End Class

