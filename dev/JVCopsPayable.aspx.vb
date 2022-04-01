Imports System.Collections.Generic
Imports System.Linq
Imports System.Web
Imports System.Web.Services
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Data.SqlClient
Imports System.Data
Imports System.Configuration
Imports System.Web.UI.Adapters.ControlAdapter
Imports System.Drawing
Imports System.Threading
Imports System
Imports System.Collections.Specialized
Imports System.Text
Imports System.Net.Security
Imports System.IO
Imports Newtonsoft.Json.Converters
Imports Newtonsoft.Json
Imports System.Web.Script.Serialization

Partial Class JVCopsPayable
    Inherits System.Web.UI.Page
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
        If Not Page.IsPostBack Then
            txtd1.Text = Now.AddDays(-1).ToString("yyyy-MM-dd")
            txtd2.Text = Now.ToString("yyyy-MM-dd")
        End If
    End Sub

    <WebMethod()>
    Public Shared Function GetData(sdate As String, tdate As String) As DGrid
        Dim jsonData As String = ""
        Dim grid As New DGrid()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim da As New SqlDataAdapter("", con)
        Try
            Dim tbl As New DataTable
            tbl.Columns.Add("File Document number", GetType(String))
            tbl.Columns.Add("Company Code", GetType(String))
            tbl.Columns.Add("Posting date", GetType(String))
            tbl.Columns.Add("Document date", GetType(String))
            tbl.Columns.Add("Ledger", GetType(String))
            tbl.Columns.Add("Document type", GetType(String))
            tbl.Columns.Add("Business place", GetType(String))
            tbl.Columns.Add("Section code", GetType(String))
            tbl.Columns.Add("Place of Supply", GetType(String))
            tbl.Columns.Add("Reference", GetType(String))
            tbl.Columns.Add("Header Text", GetType(String))
            tbl.Columns.Add("Currency", GetType(String))
            tbl.Columns.Add("Exchange Rate", GetType(String))
            tbl.Columns.Add("Posting code", GetType(String))
            tbl.Columns.Add("Special G/L Indicator", GetType(String))
            tbl.Columns.Add("Account", GetType(String))
            tbl.Columns.Add("Amount in document currency", GetType(String))
            tbl.Columns.Add("Amount in local currency", GetType(String))
            tbl.Columns.Add("Tax Code", GetType(String))
            tbl.Columns.Add("Withholding Tax Section", GetType(String))
            tbl.Columns.Add("Withholding Tax Percentance", GetType(String))
            tbl.Columns.Add("Calculate tax", GetType(String))
            tbl.Columns.Add("Assignment", GetType(String))
            tbl.Columns.Add("Text", GetType(String))
            tbl.Columns.Add("Profit Center", GetType(String))
            tbl.Columns.Add("Partner Profit center", GetType(String))
            tbl.Columns.Add("Cost center", GetType(String))
            tbl.Columns.Add("Internal order", GetType(String))
            tbl.Columns.Add("Payment term", GetType(String))
            tbl.Columns.Add("Baseline date for payment", GetType(String))
            tbl.Columns.Add("Trading Partner", GetType(String))
            tbl.Columns.Add("HSN / SAC Code", GetType(String))
            tbl.Columns.Add("Reference Key", GetType(String))
            tbl.Columns.Add("WBS ELEMENT", GetType(String))
            tbl.Columns.Add("Special periods (A/B)", GetType(String))
            tbl.Columns.Add("Special Transaction Type", GetType(String))
            tbl.Columns.Add("Payment Block Key", GetType(String))
            tbl.Columns.Add("Name", GetType(String))
            tbl.Columns.Add("City", GetType(String))
            tbl.Columns.Add("Country", GetType(String))
            tbl.Columns.Add("Period", GetType(String))
            tbl.Columns.Add("XREF1_HD", GetType(String))
            tbl.Columns.Add("Negative Posting", GetType(String))


            Dim rwSec As DataRow
            rwSec = tbl.NewRow
            rwSec(0) = ""
            rwSec(1) = "bkpf-bukrs"
            rwSec(2) = "bkpf-budat"
            rwSec(3) = "Bkpf-bldat"
            rwSec(4) = ""
            rwSec(5) = "Bkpf-blart"
            rwSec(6) = "BUPLA"
            rwSec(7) = "SECCO"
            rwSec(8) = "PLC_SUP"
            rwSec(9) = "Bkpf-xblnr"
            rwSec(10) = "Bkpf-bktxt"
            rwSec(11) = "Bkpf-waers"
            rwSec(12) = "Bkpf-kursf"
            rwSec(13) = "Bseg-bschl"
            rwSec(14) = "Bseg-umskz"
            rwSec(15) = ""
            rwSec(16) = "Bseg-wrbtr"
            rwSec(17) = "Bseg-dmbtr"
            rwSec(18) = "Bseg-mwskz"
            rwSec(19) = ""
            rwSec(20) = ""
            rwSec(21) = "Bseg-xmwst"
            rwSec(22) = "Bseg-zuonr"
            rwSec(23) = "Bseg-sgtxt"
            rwSec(24) = "Bseg-prctr"
            rwSec(25) = "bseg-pprct"
            rwSec(26) = "Bseg-kostl"
            rwSec(27) = "bseg-aufnr"
            rwSec(28) = "Bseg-zterm"
            rwSec(29) = "Bseg-zfbdt"
            rwSec(30) = "Bseg-vbund"
            rwSec(31) = "HSN_SAC"
            rwSec(32) = "bseg-xref3"
            rwSec(33) = "PS_POSID"
            rwSec(34) = "BSEG-ZZZ_PERIOD"
            rwSec(35) = "BSEG-ZZZ_SPTT"
            rwSec(36) = "Bseg-ZLSPR"
            rwSec(37) = "BAPIACPA09-NAME"
            rwSec(38) = "BAPIACPA09-CITY"
            rwSec(39) = "BAPIACPA09-COUNTRY"
            rwSec(40) = "BKPF-MONAT"
            rwSec(41) = "XXXXXX"
            rwSec(42) = "BAPIACPA09-NEG_POSTNG"
            tbl.Rows.Add(rwSec)


            Dim rwThr As DataRow
            rwThr = tbl.NewRow
            rwThr(0) = ""
            rwThr(1) = "i.e A059"
            rwThr(2) = "YYYY.MM.DD"
            rwThr(3) = "YYYY.MM.DD"
            rwThr(4) = "If all ledgers - empty field, if specific ledger: 0L, TL, LL, CL"
            rwThr(5) = "ie. PK"
            rwThr(6) = ""
            rwThr(7) = ""
            rwThr(8) = ""
            rwThr(9) = ""
            rwThr(10) = ""
            rwThr(11) = ""
            rwThr(12) = "Exchange Rate only informative; amounts are decisive for postings"
            rwThr(13) = ""
            rwThr(14) = ""
            rwThr(15) = "GL accounts or vendor/customer"
            rwThr(16) = "Entry mandatory"
            rwThr(17) = "Entry mandatory"
            rwThr(18) = ""
            rwThr(19) = ""
            rwThr(20) = ""
            rwThr(21) = ""
            rwThr(22) = ""
            rwThr(23) = ""
            rwThr(24) = ""
            rwThr(25) = ""
            rwThr(26) = ""
            rwThr(27) = "YYYY.MM.DD"
            rwThr(28) = ""
            rwThr(29) = ""
            rwThr(30) = ""
            rwThr(31) = ""
            rwThr(32) = ""
            rwThr(33) = ""
            rwThr(34) = ""
            rwThr(35) = ""
            rwThr(36) = ""
            rwThr(37) = ""
            rwThr(38) = ""
            rwThr(39) = ""
            rwThr(40) = ""
            rwThr(41) = ""
            rwThr(42) = ""
            tbl.Rows.Add(rwThr)

            Dim ds As New DataSet
            da.SelectCommand.CommandType = CommandType.Text
            If HttpContext.Current.Session("USERROLE") = "AP-TEAM" Then
                da.SelectCommand.CommandText = "select distinct d.tid from mmm_mst_doc d inner join mmm_doc_dtl dt on dt.docid=d.tid and dt.aprstatus='Finance Validation' where eid=" & HttpContext.Current.Session("EID") & " and documenttype='Cops Payable' and convert(date,dt.tdate)>='" & sdate & "' and convert(date,dt.tdate)<='" & tdate & "' and curstatus in ('Payment Update')"
            Else
                da.SelectCommand.CommandText = "select distinct d.tid from mmm_mst_doc d inner join mmm_doc_dtl dt on dt.docid=d.tid and dt.aprstatus='Finance Validation' where eid=" & HttpContext.Current.Session("EID") & " and documenttype='Cops Payable' and convert(date,dt.tdate)>='" & sdate & "' and convert(date,dt.tdate)<='" & tdate & "' and curstatus in ('Payment Update')"
                '' earlier code - bkup on 14-may-18 by sp
                ' da.SelectCommand.CommandText = "select distinct tid from mmm_mst_doc where eid=137 and documenttype='Invoice PO' and convert(date,adate)>='" & sdate & "' and convert(date,adate)<='" & tdate & "' and curstatus in ('Update of Payment Details')"
            End If

            da.Fill(ds, "docid")
            da.SelectCommand.CommandText = "select qry from mmm_print_template where template_name='cops payable' and eid=" & HttpContext.Current.Session("EID") & ""
            da.Fill(ds, "data")
            ' FOR CHILD ITEM AND DOC JOIN
            Dim DocCnt As Integer = 0
            If ds.Tables("data").Rows.Count > 0 Then
                For i = 0 To ds.Tables("docid").Rows.Count - 1   '' main doc table loop 
                    DocCnt = i + 1
                    Dim qry As String = ds.Tables("data").Rows(0).Item(0).ToString   '' get qry (child items of selected doc id)
                    da.SelectCommand.CommandType = CommandType.Text
                    qry = Replace(qry, "@tid", ds.Tables("docid").Rows(i).Item("tid"))
                    da.SelectCommand.CommandText = qry
                    ' FOR CHID ITEM AND DOC JOIN REPLACE TID
                    da.Fill(ds, "child")
                    ' FOR VENDOR ROW

                    Dim rw As DataRow
                    rw = tbl.NewRow

                    da.SelectCommand.CommandType = CommandType.Text
                    da.SelectCommand.CommandText = "select sql_child2 from mmm_print_template where template_name='cops payable' and eid=" & HttpContext.Current.Session("EID") & ""  ' qyr for Vendor row (one for each invoice header 
                    da.Fill(ds, "VENDOR")
                    Dim Vqry As String = ds.Tables("VENDOR").Rows(0).Item("sql_child2").ToString
                    Vqry = Replace(Vqry, "@tid", ds.Tables("docid").Rows(i).Item("tid"))
                    da.SelectCommand.CommandType = CommandType.Text
                    da.SelectCommand.CommandText = Vqry
                    ' FILL FIRST VENDOR ROW DATA
                    Dim VENDORDATA As New DataTable
                    da.Fill(VENDORDATA)
                    If VENDORDATA.Rows.Count > 0 Then
                        ' LOOP FOR CHILD ITEM OF INVOICE 
                        rw(0) = DocCnt
                        rw(1) = VENDORDATA.Rows(0).Item(1).ToString
                        rw(2) = VENDORDATA.Rows(0).Item(2).ToString
                        rw(3) = VENDORDATA.Rows(0).Item(3).ToString
                        rw(4) = VENDORDATA.Rows(0).Item(4).ToString
                        rw(5) = VENDORDATA.Rows(0).Item(5).ToString
                        rw(6) = VENDORDATA.Rows(0).Item(6).ToString
                        rw(7) = VENDORDATA.Rows(0).Item(7).ToString
                        rw(8) = VENDORDATA.Rows(0).Item(8).ToString
                        rw(9) = VENDORDATA.Rows(0).Item(9).ToString
                        rw(10) = VENDORDATA.Rows(0).Item(10).ToString
                        rw(11) = VENDORDATA.Rows(0).Item(11).ToString
                        rw(12) = VENDORDATA.Rows(0).Item(12).ToString
                        rw(13) = VENDORDATA.Rows(0).Item(13).ToString
                        rw(14) = VENDORDATA.Rows(0).Item(14).ToString
                        rw(15) = VENDORDATA.Rows(0).Item(15).ToString
                        rw(16) = VENDORDATA.Rows(0).Item(16).ToString
                        rw(17) = VENDORDATA.Rows(0).Item(17).ToString
                        rw(18) = VENDORDATA.Rows(0).Item(18).ToString
                        rw(19) = VENDORDATA.Rows(0).Item(19).ToString
                        rw(20) = VENDORDATA.Rows(0).Item(20).ToString
                        rw(21) = VENDORDATA.Rows(0).Item(21).ToString
                        rw(22) = VENDORDATA.Rows(0).Item(22).ToString
                        rw(23) = VENDORDATA.Rows(0).Item(23).ToString
                        rw(24) = VENDORDATA.Rows(0).Item(24).ToString
                        rw(25) = VENDORDATA.Rows(0).Item(25).ToString
                        rw(26) = VENDORDATA.Rows(0).Item(26).ToString
                        rw(27) = VENDORDATA.Rows(0).Item(27).ToString
                        rw(28) = VENDORDATA.Rows(0).Item(28).ToString
                        rw(29) = VENDORDATA.Rows(0).Item(29).ToString
                        rw(30) = VENDORDATA.Rows(0).Item(30).ToString
                        rw(31) = VENDORDATA.Rows(0).Item(31).ToString
                        rw(32) = VENDORDATA.Rows(0).Item(32).ToString
                        rw(33) = VENDORDATA.Rows(0).Item(33).ToString
                        rw(34) = VENDORDATA.Rows(0).Item(34).ToString
                        rw(35) = VENDORDATA.Rows(0).Item(35).ToString
                        rw(36) = VENDORDATA.Rows(0).Item(36).ToString
                        rw(37) = VENDORDATA.Rows(0).Item(37).ToString
                        rw(38) = VENDORDATA.Rows(0).Item(38).ToString
                        rw(39) = VENDORDATA.Rows(0).Item(39).ToString
                        rw(40) = VENDORDATA.Rows(0).Item(40).ToString
                        rw(41) = VENDORDATA.Rows(0).Item(41).ToString
                        rw(42) = VENDORDATA.Rows(0).Item(42).ToString

                        tbl.Rows.Add(rw)

                    End If
                    If ds.Tables("child").Rows.Count > 0 Then
                        For j = 0 To ds.Tables("child").Rows.Count - 1

                            da.SelectCommand.CommandType = CommandType.Text
                            da.SelectCommand.CommandText = "select sql_childitem from mmm_print_template where template_name='cops payable' and eid=" & HttpContext.Current.Session("EID") & ""
                            ' FOR ALLOCATION
                            da.Fill(ds, "PR")
                            Dim pqry As String = ds.Tables("PR").Rows(0).Item("sql_childitem").ToString
                            pqry = Replace(pqry, "@tid", ds.Tables("docid").Rows(i).Item("tid"))
                            da.SelectCommand.CommandType = CommandType.Text
                            da.SelectCommand.CommandText = pqry
                            ' FILL ALL NON ZERO ALLOCATION FOR CHILD DOCID
                            da.Fill(ds, "PRDATA")
                            If ds.Tables("PRDATA").Rows.Count > 0 Then
                                ' LOOP FOR PR NON ZERO ITEM ON CHILD DOC ID
                                For k = 0 To ds.Tables("PRDATA").Rows.Count - 1
                                    da.SelectCommand.CommandType = CommandType.Text
                                    da.SelectCommand.CommandText = "select sql_child1 from mmm_print_template where template_name='cops payable' and eid=" & HttpContext.Current.Session("EID") & ""
                                    da.Fill(ds, "DOCDATA")
                                    Dim docstr As String = ds.Tables("DOCDATA").Rows(0).Item("sql_child1").ToString
                                    docstr = Replace(docstr, "@tid", ds.Tables("docid").Rows(i).Item("tid"))
                                    da.SelectCommand.CommandText = docstr
                                    Dim allocation As String = ""
                                    allocation = ds.Tables("PRDATA").Rows(k).Item(0)
                                    ' FOR DOC DATA FIXED VALUES
                                    Dim dtfix As New DataTable
                                    da.Fill(dtfix)
                                    Dim rw1 As DataRow
                                    rw1 = tbl.NewRow
                                    Dim Childtid As String = ds.Tables("child").Rows(j).Item("docid").ToString
                                    da.SelectCommand.CommandType = CommandType.Text
                                    ' da.SelectCommand.CommandText = "select convert(numeric(10,2),((convert(numeric(10,2)," & allocation & "))*  (convert(numeric(10,2),di.fld7)))/100)[Amount],(select top 1  fld1 from mmm_mst_master where tid=(select top 1 fld12 from mmm_mst_master where documenttype='Item Master' and tid=di.fld2))[GLCode],fld5[HSN] from mmm_mst_doc_item di where tid=" & Childtid & ""
                                    ' da.SelectCommand.CommandText = "select convert(numeric(10,2),((convert(numeric(10,2)," & allocation & "))*  (convert(numeric(10,2),di.fld7)))/100)[Amount], fld23 [GLCode],fld5 [HSN],dms.udf_split('MASTER-Tax Code Master-fld1',fld22)[TaxCode] from mmm_mst_doc_item di where tid=" & Childtid & ""
                                    'da.Fill(ds, "CHILDDATA")
                                    ' Dim amt As String = ds.Tables("CHILDDATA").Rows(0).Item("Amount").ToString
                                    'Dim glcode As String = ds.Tables("CHILDDATA").Rows(0).Item("GLCode").ToString
                                    'Dim hsncode As String = ds.Tables("CHILDDATA").Rows(0).Item("HSN").ToString
                                    Dim WBSElement As String = ds.Tables("PRDATA").Rows(k).Item("WBSField").ToString
                                    'Dim TaxCode As String = ds.Tables("CHILDDATA").Rows(k).Item("TaxCode").ToString

                                    rw1(0) = DocCnt
                                    rw1(1) = dtfix.Rows(0).Item(1).ToString
                                    rw1(2) = dtfix.Rows(0).Item(2).ToString
                                    rw1(3) = dtfix.Rows(0).Item(3).ToString
                                    rw1(4) = dtfix.Rows(0).Item(4).ToString
                                    rw1(5) = dtfix.Rows(0).Item(5).ToString
                                    rw1(6) = dtfix.Rows(0).Item(6).ToString
                                    rw1(7) = dtfix.Rows(0).Item(7).ToString
                                    rw1(8) = dtfix.Rows(0).Item(8).ToString
                                    rw1(9) = dtfix.Rows(0).Item(9).ToString
                                    rw1(10) = dtfix.Rows(0).Item(10).ToString
                                    rw1(11) = dtfix.Rows(0).Item(11).ToString
                                    rw1(12) = dtfix.Rows(0).Item(12).ToString
                                    rw1(13) = dtfix.Rows(0).Item(13).ToString
                                    rw1(14) = dtfix.Rows(0).Item(14).ToString
                                    rw1(15) = dtfix.Rows(0).Item(15).ToString
                                    rw1(16) = dtfix.Rows(0).Item(16).ToString
                                    rw1(17) = dtfix.Rows(0).Item(17).ToString
                                    rw1(18) = dtfix.Rows(0).Item(18).ToString
                                    'prev from main doc   by sp on 09-Jan-19
                                    'rw1(18) = dtfix.Rows(0).Item(18).ToString
                                    '' now from child item  by sp on 09-Jan-19
                                    rw1(19) = dtfix.Rows(0).Item(19).ToString
                                    rw1(20) = dtfix.Rows(0).Item(20).ToString
                                    rw1(21) = dtfix.Rows(0).Item(21).ToString
                                    rw1(22) = dtfix.Rows(0).Item(22).ToString
                                    rw1(23) = dtfix.Rows(0).Item(23).ToString
                                    rw1(24) = dtfix.Rows(0).Item(24).ToString
                                    rw1(25) = dtfix.Rows(0).Item(25).ToString
                                    rw1(26) = dtfix.Rows(0).Item(26).ToString
                                    rw1(27) = dtfix.Rows(0).Item(27).ToString
                                    rw1(28) = dtfix.Rows(0).Item(28).ToString
                                    rw1(29) = dtfix.Rows(0).Item(29).ToString
                                    rw1(30) = dtfix.Rows(0).Item(30).ToString
                                    rw1(31) = dtfix.Rows(0).Item(31).ToString
                                    rw1(32) = dtfix.Rows(0).Item(32).ToString
                                    rw1(33) = WBSElement
                                    rw1(34) = dtfix.Rows(0).Item(34).ToString
                                    rw1(35) = dtfix.Rows(0).Item(35).ToString
                                    rw1(36) = dtfix.Rows(0).Item(36).ToString
                                    rw1(37) = dtfix.Rows(0).Item(37).ToString
                                    rw1(38) = dtfix.Rows(0).Item(38).ToString
                                    rw1(39) = dtfix.Rows(0).Item(39).ToString
                                    rw1(40) = dtfix.Rows(0).Item(40).ToString
                                    rw1(41) = dtfix.Rows(0).Item(41).ToString
                                    rw1(42) = dtfix.Rows(0).Item(42).ToString

                                    tbl.Rows.Add(rw1)

                                    ds.Tables("DOCDATA").Rows.Clear()
                                    'ds.Tables("CHILDDATA").Rows.Clear()
                                    dtfix.Clear()
                                    '  rw1.Delete()
                                Next
                                ds.Tables("PRDATA").Rows.Clear()
                            End If
                        Next
                        ' ds.Tables("child").Clear()

                    End If
                    VENDORDATA.Clear()
                    ds.Tables("child").Clear()
                Next
            End If

            Dim strError = ""
            grid = DynamicGrid.GridData(tbl, strError)
            If tbl.Rows.Count = 0 Then
                grid.Message = "No data found...!"
                grid.Success = False
            End If

        Catch Ex As Exception
            grid.Success = False
            grid.Message = "No data found...!"
            grid.Count = 0

        Finally
            con.Close()
            da.Dispose()
            con.Dispose()
        End Try
        Return grid
    End Function
End Class
