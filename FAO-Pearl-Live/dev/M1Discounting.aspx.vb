
Imports System.Data

Partial Class M1Discounting
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

    <System.Web.Services.WebMethod()>
    Public Shared Function GetDiscountRateMaster(ByVal docid As Int32) As DiscountingMasterList
        Dim objListOfDiscountingMaster As New DiscountingMasterList()
        Dim listDiscounting As New List(Of DiscountingMaster)
        Dim objDC As New DataClass()
        Dim objDT As New DataTable()
        objDT = objDC.ExecuteQryDT("select tid,fld1,fld2,fld3 from mmm_mst_master where documenttype in (select isnull(DiscountingMaster,'') from mmm_mst_entity where eid in (select eid from mmm_mst_doc where tid= " & docid & ")) and eid in (select eid from mmm_mst_doc where tid= " & docid & ")")
        For Each dr As DataRow In objDT.Rows
            Dim objDiscountingMaster As New DiscountingMaster()
            objDiscountingMaster.MASTERTID = dr("tid")
            objDiscountingMaster.NAME = dr("fld1")
            objDiscountingMaster.DISCOUNTINGPERCENT = dr("fld2")
            objDiscountingMaster.PAYABLEDAYS = dr("fld3")
            listDiscounting.Add(objDiscountingMaster)
        Next
        If listDiscounting.Count > 0 Then
            objListOfDiscountingMaster.ListOfDiscountingMaster = listDiscounting
        End If
        Return objListOfDiscountingMaster
    End Function


    <System.Web.Services.WebMethod()>
    Public Shared Function SaveDiscounting(ByVal type As String, ByVal invoiceamount As String, ByVal docid As String, ByVal disocuntRate As String, ByVal interest As String, ByVal netPaidAmount As String, ByVal discountType As String, ByVal payabledays As String) As DataResponse
        Dim objDC As New DataClass()
        Dim ret As New DataResponse()
        Try
            If type.ToUpper = "SAVE" Then
                objDC.ExecuteNonQuery("insert into mmm_mst_discounting  (docid,invoiceAmount,discountrate,interestrate,netpaidAmount,discountType,Status,PayableDays) values(" & docid & ",'" & invoiceamount & "','" & disocuntRate & "','" & interest & "','" & netPaidAmount & "','" & discountType & "','ACCEPT'," & payabledays & ")")
                ret.RESCODE = 200
                ret.RESMESSAGE = "Your transaction has successfully accepted for discounting"
                ret.RESURL = GetURL(docid:=docid)
                Dim ht As New Hashtable()
                ht.Add("@docid", docid)
                objDC.ExecuteProDT("M1_UpdateDocData", ht)
            Else
                objDC.ExecuteNonQuery("insert into mmm_mst_discounting  (docid,invoiceAmount,discountrate,interestrate,netpaidAmount,discountType,Status,PayableDays) values(" & docid & ",'" & invoiceamount & "','" & disocuntRate & "','" & interest & "','" & netPaidAmount & "','" & discountType & "','REJECT'," & payabledays & ")")
                ret.RESCODE = 201
                ret.RESMESSAGE = "Your transaction has successfully rejected for discounting"
                ret.RESURL = GetURL(docid:=docid)
            End If
        Catch ex As Exception
            ret.RESCODE = 404
            ret.RESMESSAGE = "Something wrong please contact admin!!!"
        End Try

        Return ret
    End Function
    <System.Web.Services.WebMethod()>
    Public Shared Function GetDocumentDetail(ByVal docid As Int32) As Discounting

        Dim objDiscounting As New Discounting()
        objDiscounting.DOCID = docid
        Dim objDC As New DataClass()
        Dim objDT As New DataTable
        Dim ht As New Hashtable
        ht.Add("docid", docid)
        objDT = objDC.ExecuteProDT("M1_GetDiscountingFieldWithData", ht)
        For Each dr As DataRow In objDT.Rows
            For Each dc As DataColumn In objDT.Columns
                Select Case dc.ColumnName.ToUpper
                    Case "TOTALINVOICEAMOUNT"
                        objDiscounting.TOTALINVOICEAMOUNT = dr(dc.ColumnName)
                    Case "VENDORCODE"
                        objDiscounting.VENDORCODE = dr(dc.ColumnName)
                    Case "INVOICEDATE"
                        objDiscounting.INVOICEDATE = dr(dc.ColumnName)
                End Select
            Next
        Next

        objDT = objDC.ExecuteQryDT("select tid,fld1 from mmm_mst_master where documenttype in (select TypeOfDiscounting from mmm_mst_entity where eid in (select eid from mmm_mst_doc where tid= " & docid & ")) and eid in (select eid from mmm_mst_doc where tid= " & docid & ")")
        Dim objlistDisocuntType As New List(Of DiscountType)
        For Each dr As DataRow In objDT.Rows
            Dim objDisocuntType As New DiscountType()
            objDisocuntType.DISCOUNTTYPEID = dr("tid")
            objDisocuntType.DISCOUNTTYPE = dr("fld1")
            objlistDisocuntType.Add(objDisocuntType)
        Next
        If objlistDisocuntType.Count > 0 Then
            objDiscounting.LISTOFDISCOUNTTYPE = objlistDisocuntType
        End If
        Return objDiscounting
    End Function


    Public Shared Function GetURL(ByRef docid As Int32) As String
        Dim res As String = ""
        Dim objDC As New DataClass()
        Dim ht As New Hashtable()
        ht.Add("@docid", docid)
        res = objDC.ExecuteProDT("sp_getURl", ht).Rows(0)(0).ToString()
        Return res
    End Function
    Public Class DataResponse
        Public Property RESCODE As Int32
        Public Property RESMESSAGE As String

        Public Property RESURL As String
    End Class
    Public Class DiscountingMasterList
        Public Property ListOfDiscountingMaster As List(Of DiscountingMaster)
    End Class

    Public Class DiscountingMaster
        Public Property MASTERTID As Int32
        Public Property NAME As String
        Public Property DISCOUNTINGPERCENT As String
        Public Property PAYABLEDAYS As Int32

    End Class

    Public Class DiscountType
        Public Property DISCOUNTTYPEID As Int32
        Public Property DISCOUNTTYPE As String
    End Class
    Public Class Discounting
        Public Property DOCID As Int32
        Public Property TOTALINVOICEAMOUNT As Double
        Public Property INVOICEDATE As String
        Public Property VENDORCODE As String
        Public Property LISTOFDISCOUNTTYPE As List(Of DiscountType)

    End Class

End Class
