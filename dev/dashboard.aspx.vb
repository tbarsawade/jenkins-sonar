Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports System.Web.UI.Adapters.ControlAdapter
Imports iTextSharp.text
Imports System.Drawing
Imports System.Web.Services
Imports System.Web.UI.DataVisualization.Charting

Partial Class dashboard
    Inherits System.Web.UI.Page
    Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            BindGrid()
        End If
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
    Protected Sub gv_OnRowCreated(sender As Object, e As GridViewRowEventArgs)

        If e.Row.RowType = DataControlRowType.DataRow Then
            If e.Row.RowState = DataControlRowState.Alternate Then
                e.Row.Attributes.Add("onmouseover", "this.style.backgroundColor='#CAFF70';")
                e.Row.Attributes.Add("onmouseout", "this.style.backgroundColor='#ffffff';")
            Else
                e.Row.Attributes.Add("onmouseover", "this.style.backgroundColor='#CAFF70';")
                e.Row.Attributes.Add("onmouseout", "this.style.backgroundColor='#ffffff';")
            End If
        End If
    End Sub

    Protected Sub BindGrid()
        Try
            Dim Query = "select m.fld1[Branch Code],m.fld10[Branch Name],count(d.fld1)[Total Policies],(select count(*) from mmm_mst_doc where  fld21='1' and fld22=d.fld22)[Inside Catchment],count(d.fld1)-(select count(*) from mmm_mst_doc where  fld21='1' and fld22=d.fld22)[Outside Catchment ] from mmm_mst_master m inner join mmm_mst_doc d on d.fld22=m.tid where m.documenttype='Branch' and m.eid=53 and d.eid=53  group by m.fld1,m.fld10,d.fld22"
            Dim ds As New DataSet()
            Using con = New SqlConnection(conStr)
                Using da = New SqlDataAdapter(Query, con)
                    da.Fill(ds)
                End Using
            End Using
            If ds.Tables(0).Rows.Count > 0 Then
                gvData.DataSource = ds
                gvData.DataBind()
                ' lblDataCount.Text = ds.Tables(0).Rows.Count()
                dlChart.DataSource = ds
                dlChart.DataBind()
                'Dim arrx As String() = {"Inside", "outside"}
                'Dim arry As Integer() = {1, 5}
                'chtBranch.Series("chartbranch").Points.DataBindXY(arrx, arry)
                'chtBranch.Series("chartbranch").Points(0).Color = Color.Green
                'chtBranch.Series("chartbranch").Points(1).Color = Color.Yellow
                'chtBranch.Series("chartbranch").ChartType = SeriesChartType.Pie
                ''chtBranch.Series("chartbranch")("PieLabelStyle") = "Disabled"
                'chtBranch.ChartAreas("ChartArea1").Area3DStyle.Enable3D = True
                'chtBranch.Legends(0).Enabled = True
            Else
                'lblDataCount.Text = "0"
            End If
        Catch ex As Exception

        End Try
    End Sub
    Protected Sub Shalini_ItemCommand(sender As Object, e As DataListItemEventArgs)
        'Dim SurveyID As String = dlChart.DataKeys(e.Item.ItemIndex).ToString()
        Dim a As Integer = gvData.Rows(e.Item.ItemIndex()).Cells(3).Text
        Dim b As Integer = gvData.Rows(e.Item.ItemIndex()).Cells(4).Text

        Dim mychart As Chart = CType(e.Item.FindControl("sChart"), Chart)
        Dim yValues = {a, b}
        Dim xValues As String() = {"OutSide ", "Inside"}

        mychart.Series("Series1").Points.DataBindXY(xValues, yValues)

        mychart.Series("Series1").Points(0).Color = Color.Yellow

        mychart.Series("Series1").Points(1).Color = Color.Green

        mychart.Series("Series1").ChartType = SeriesChartType.Pie

        mychart.Series("Series1")("PieLabelStyle") = "Disabled"

        mychart.ChartAreas("ChartArea1").Area3DStyle.Enable3D = True

        mychart.Legends(0).Enabled = False

    End Sub
End Class
