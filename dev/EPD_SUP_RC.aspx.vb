
Imports System.Data
Imports System.IO

Partial Class EPD_SUP_RC
    Inherits System.Web.UI.Page



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
    Protected Sub imgCSVSample_Click(sender As Object, e As ImageClickEventArgs)
        Dim objDC As New DataClass()
        Dim objDT As New DataTable()
        objDT = objDC.ExecuteQryDT("declare @Supplier_Formname nvarchar(max),@Supplier_formname_fieldMapping nvarchar(max),@qry nvarchar(max),@VendorLoginID nvarchar(max),@vendorDisplayname nvarchar(max) select @Supplier_Formname=Supplier_formName,@Supplier_formname_fieldMapping=Supplier_formname_fieldMapping from MMM_MST_ENTITY where eid=" & Session("EID") & " select @VendorLoginID=fieldMapping,@vendorDisplayname=displayname from mmm_mst_fields where IsVendorLogin_EPD=1 and eid=" & Session("EID") & " and documenttype= ''+@Supplier_Formname+'' set @qry ='select '+@Supplier_formname_fieldMapping+' as [Supplier Name]  ,tid , '+@VendorLoginID+' as ['+@vendorDisplayname+'],'''' as RateCardDescription,'''' as ROI,'''' as ROIDays from mmm_mst_master where documenttype= '''+@Supplier_Formname+''' and eid=" & Session("EID") & " ' exec sp_executesql  @qry")
        Response.Clear()
        Response.Buffer = True
        Response.Charset = ""
        Response.ContentType = "application/vnd.ms-excel"

        'fill Product


        Dim filename As String = "VENDOR_RATECARD_MASTER"
        filename = filename.Replace(" ", "_")
        Response.AddHeader("content-disposition", "attachment;filename=" & filename & ".xls")
        Dim gvex As New GridView()
        gvex.AllowPaging = False
        gvex.DataSource = objDT
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
        tb.Rows.Add(tr1)
        tb.RenderControl(hw)
        Dim style As String = "<style> .textmode { mso-number-format:\@; } </style>"
        Response.Write(style)
        Response.Output.Write(sw.ToString())
        Response.Flush()
        Response.[End]()
    End Sub
    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs)
        Dim objDC As New DataClass()
        Dim filename As String = "COLL" & Now.Day & Now.Month & Now.Year & Now.Hour & Now.Minute & Now.Minute & Now.Second & Now.Millisecond & Right(fuMaster.PostedFile.FileName, 4).ToUpper()
        fuMaster.PostedFile.SaveAs(Server.MapPath("Import/") & filename)
        Dim dtData As New DataTable
        dtData = GetDataFromExcel(filename)
        Dim colName As String = objDC.ExecuteQryScaller("declare @Supplier_Formname nvarchar(max),@Supplier_formname_fieldMapping nvarchar(max),@qry nvarchar(max),@VendorLoginID nvarchar(max),@vendorDisplayname nvarchar(max) select @Supplier_Formname=Supplier_formName,@Supplier_formname_fieldMapping=Supplier_formname_fieldMapping from MMM_MST_ENTITY where eid=" & Session("EID") & " set @qry ='select displayname from mmm_mst_fields where IsVendorLogin_EPD=1 and eid=" & Session("EID") & " and documenttype= '''+@Supplier_Formname+'''' exec sp_executesql  @qry")


        Dim dynQry As String = "insert into mmm_Ratecard_Vendor ("
        Dim finalString As String = ""
        Dim TotalErrorMsg As New ArrayList()
        TotalErrorMsg.Add("Please Enter \n")
        Dim count As Int32 = 1
        For Each dr As DataRow In dtData.Rows
            Dim ErrorMsg As New ArrayList()
            Dim arCol As New ArrayList
            Dim arColData As New ArrayList
            For Each dc As DataColumn In dtData.Columns
                If dc.ColumnName.ToUpper = "RATECARDDESCRIPTION" Then
                    arCol.Add("RateCardDescription")
                    If dr("RateCardDescription") = "" Then
                        ErrorMsg.Add(" Rate Card Description ")
                    End If
                    arColData.Add("'" & dr("RateCardDescription") & "'")

                End If
                If dc.ColumnName.ToUpper = "ROI" Then
                    arCol.Add("ROI")
                    If dr("ROI") = "" Then
                        ErrorMsg.Add(" ROI ")
                    End If
                    arColData.Add("'" & dr("ROI") & "'")
                End If
                If dc.ColumnName.ToUpper = "ROIDAYS" Then
                    arCol.Add("ROIDays")
                    If dr("ROIDays") = "" Then
                        ErrorMsg.Add(" ROI Days ")
                    End If
                    arColData.Add("'" & dr("ROIDays") & "'")
                End If
                If dc.ColumnName.ToUpper = colName.ToUpper Then
                    arCol.Add("OUID")
                    arColData.Add("'" & dr("" & colName & "") & "'")
                End If
                If dc.ColumnName.ToUpper = "TID" Then
                    arCol.Add("vendortid")
                    If dr("TID") = "" Then
                        ErrorMsg.Add(" Vendor tid ")
                    End If
                    arColData.Add("'" & dr("TID") & "'")
                End If
            Next
            arCol.Add("EID")
            arColData.Add("'" & Session("EID") & "'")
            arCol.Add("CreatedOn")
            arColData.Add("getdate()")
            If ErrorMsg.Count > 0 Then
                TotalErrorMsg.Add("Error found at line no " & count & " following fields are required \n" & String.Join(",", ErrorMsg.ToArray()))
            Else
                finalString = finalString & dynQry & String.Join(",", arCol.ToArray()) & ") values (" & String.Join(",", arColData.ToArray()) & ")"
            End If

            count = count + 1
        Next
        objDC.ExecuteNonQuery(finalString.ToString())
        errorMsg.Text = String.Join(" ", TotalErrorMsg.ToArray())

    End Sub

    Public Function GetDataFromExcel(ByVal strDataFilePath As String, Optional ByVal DocumentType As String = Nothing) As DataTable
        ' GetDataFromExcel123(strDataFilePath)
        Try
            Dim Seperator As String = ","
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
                    ''Conversion due to Pen testing data
                    'finalLine = finalLine.Replace("'=", "=").Replace("'-", "-").Replace("'+", "+")
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
    Protected Sub btnDownload_Click(sender As Object, e As EventArgs)
        Dim objDC As New DataClass()
        Dim objDT As New DataTable()
        objDT = objDC.ExecuteQryDT("declare @Supplier_FormName_FieldMapping nvarchar(200),@Qry nvarchar(max) select  @Supplier_FormName_FieldMapping=Supplier_formname_fieldMapping from mmm_mst_entity where eid=" & Session("EID") & " set @Qry='select  (select '+ @Supplier_FormName_FieldMapping+' from mmm_mst_master where tid in(vendortid)) as SupplierName  ,RatecardDescription,ROI,ROIDays,Ouid,VendorTID from mmm_Ratecard_Vendor where eid=" & Session("EID") & " ' exec sp_executesql @Qry ")
        If objDT.Rows.Count > 0 Then
            Response.Clear()
            Response.Buffer = True
            Response.Charset = ""
            Response.ContentType = "application/vnd.ms-excel"

            'fill Product


            Dim filename As String = "DOWNLOADED_VENDOR_RATECARD_MASTER"
            filename = filename.Replace(" ", "_")
            Response.AddHeader("content-disposition", "attachment;filename=" & filename & ".xls")
            Dim gvex As New GridView()
            gvex.AllowPaging = False
            gvex.DataSource = objDT
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
            tb.Rows.Add(tr1)
            tb.RenderControl(hw)
            Dim style As String = "<style> .textmode { mso-number-format:\@; } </style>"
            Response.Write(style)
            Response.Output.Write(sw.ToString())
            Response.Flush()
            Response.[End]()
        End If
    End Sub
End Class
