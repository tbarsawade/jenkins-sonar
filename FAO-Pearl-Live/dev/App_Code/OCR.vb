Imports System.Data
Imports System.IO
Imports System.Net
Imports Microsoft.VisualBasic
Imports Tamir.SharpSsh
Imports Renci.SshNet
Imports Renci.SshNet.Sftp

Public Class OCR
    Dim objDC As New DataClass()
    Dim _eid As Integer = 0
    Dim _documentType As String = ""
    Dim _fileNameFormat As String = ""
    Dim _OldfileNameFormat As String = ""
    Dim _ftpURL As String = ""
    Dim _ftpPassword As String = ""
    Dim _ftpUserName As String = ""
    Dim _ftpPORT As String = ""
    Dim _docID As Integer = 0
    Dim _targetDocumentType As String = ""
    Dim _targetFieldmapping As String = ""
    Dim _PreftpPath As String = ""
    Dim _TargetDatahistoryFieldMapping As String = ""
    Dim _DisputeInOCRFieldMapping As String = ""
    Public Sub New(EID As Integer, DOCID As Int32)   'constructor
        _eid = EID
        _docID = DOCID
    End Sub
    Public Function SendToFTP() As Boolean
        Dim res As Boolean = False

        If (IsFTPFunctional()) Then
            If (GetFileNameString()) Then
                UploadFileToFTP()
            End If
        End If
        Return res
    End Function



    Private Function UploadFileToFTP() As Boolean
        Dim res As Boolean = False
        Try
            _OldfileNameFormat = objDC.ExecuteQryScaller("declare @qry as nvarchar(max)=''set @qry = 'select '+ (select fieldmapping from mmm_mst_fields where documenttype in(select documenttype from mmm_mst_doc where tid=" & _docID & ") and eid=" & _eid & " and isocrupload=1) +' from mmm_mst_doc with(nolock) where tid=" & _docID & "'exec sp_executesql @qry")
            Dim source As String = System.Web.Hosting.HostingEnvironment.MapPath("~\DOCS\") & _OldfileNameFormat
            Dim filename As String = Path.GetFileName(source)
            _fileNameFormat = _fileNameFormat & Path.GetExtension(source)
            Dim ftpfullpath As String = _PreftpPath & _fileNameFormat
            Dim client As New SftpClient(_ftpURL, _ftpPORT, _ftpUserName, _ftpPassword)
            Dim fs As New FileStream(source, FileMode.Open)
            client.Connect()
            client.UploadFile(fs, ftpfullpath)
            client.Dispose()
            OCRLOG(True)
            res = True
        Catch ex As Exception
            OCRLOG(False)
            Return res
            Throw ex
        End Try
        Return res
    End Function



    '=======================================================
    'Service provided by Telerik (www.telerik.com)
    'Conversion powered by NRefactory.
    'Twitter: @telerik
    'Facebook: facebook.com/telerik
    '=======================================================



    '=======================================================
    'Service provided by Telerik (www.telerik.com)
    'Conversion powered by NRefactory.
    'Twitter: @telerik
    'Facebook: facebook.com/telerik
    '=======================================================

    Private Function GetFileNameString() As Boolean
        Dim ret As String = ""
        Dim fileFormat As String() = _fileNameFormat.ToString().Split("|")
        Dim finalKeyValueList As New List(Of KeyValue)
        For i As Integer = 0 To fileFormat.Length - 1
            Dim _KeyValue As New KeyValue()
            If Not (Convert.ToString(fileFormat(i)).ToString().ToUpper.Contains("SESSION")) Then
                _KeyValue.Key = fileFormat(i)
                _KeyValue.ValueData = IIf(objDC.ExecuteQryScaller("Select " & Convert.ToString(fileFormat(i)).ToString().ToUpper.Replace("{", "").Replace("}", "") & " from mmm_mst_doc where tid=" & _docID & "") = "", Convert.ToString(fileFormat(i)).ToString().ToUpper.Replace("{", "").Replace("}", ""), objDC.ExecuteQryScaller("Select " & Convert.ToString(fileFormat(i)).ToString().ToUpper.Replace("{", "").Replace("}", "") & " from mmm_mst_doc where tid=" & _docID & ""))
                _KeyValue.IsSessionType = False
            Else
                _KeyValue.Key = fileFormat(i)
                Dim str As String = ""
                'Change code for get userid from uid sesssion'
                If fileFormat(i).ToString().ToUpper.Contains("$") Then
                    Dim splitQry As String() = fileFormat(i).ToString().ToUpper.Split("$")
                    Dim dynQuery As New StringBuilder()
                    If splitQry.Length > 1 Then
                        dynQuery.Append(splitQry(0).ToString().ToUpper())
                        str = Convert.ToString(splitQry(1)).ToString().ToUpper.Replace("{", "").Replace("}", "").Replace("[", "").Replace("]", "").Replace("SESSION", "").Replace(")", "").Replace("(", "").Replace("""", "")
                        dynQuery.Append(HttpContext.Current.Session(str.ToString()))
                        dynQuery.Append(splitQry(2).ToString().ToUpper())
                        _KeyValue.ValueData = objDC.ExecuteQryScaller(dynQuery.ToString())
                    End If
                Else
                    str = Convert.ToString(fileFormat(i)).ToString().ToUpper.Replace("{", "").Replace("}", "").Replace("[", "").Replace("]", "").Replace("SESSION", "").Replace(")", "").Replace("(", "").Replace("""", "")
                    _KeyValue.ValueData = HttpContext.Current.Session(str)
                End If
                _KeyValue.IsSessionType = True
            End If
            finalKeyValueList.Add(_KeyValue)
        Next
        Dim finalString As New ArrayList()
        For i As Integer = 0 To finalKeyValueList.Count - 1
            finalString.Add(finalKeyValueList(i).ValueData.ToString())
        Next
        _fileNameFormat = (String.Join("_", finalString.ToArray())).ToString()
        _fileNameFormat = _fileNameFormat.Replace(" ", "_").Replace("|", "_")
        '_fileNameFormat = "HCLPEARL_SUPERUSER_000001"
        ret = True
        Return ret
    End Function
    Private Function IsFTPFunctional() As Boolean
        Dim res As Boolean = False
        Dim objDT As New DataTable
        objDT = objDC.ExecuteQryDT("Select  Case isnull(SendToFTP,0) when 1 then 'YES' else 'NO' end as SendToFTP,isnull(FileFormatName,'') as FileFormatName,formname,ftpurl,FTPUserName,FTPPassword,OCRTargetDocument,TargetFieldMapping,isnull(PORTNO,0) as PORTNO,isnull(PreFTPPath,'') as PreFTPPath, isnull(TargetDatahistoryFieldMapping,'') as TargetDatahistoryFieldMapping,isnull(DisputeInOCRFieldMapping,'') as DisputeInOCRFieldMapping   from mmm_mst_forms where formname in(select documenttype from mmm_mst_doc where tid=" & _docID & ") and eid=" & _eid & "")

        If objDT.Rows.Count > 0 Then
            If Convert.ToString(objDT.Rows(0)("SendToFTP")).ToUpper = "YES" Then
                _fileNameFormat = objDT.Rows(0)("FileFormatName")
                _documentType = objDT.Rows(0)("formName")
                _ftpURL = objDT.Rows(0)("ftpurl")
                _ftpUserName = objDT.Rows(0)("FTPUserName")
                _ftpPassword = objDT.Rows(0)("FTPPassword")
                _targetDocumentType = objDT.Rows(0)("OCRTargetDocument")
                _targetFieldmapping = objDT.Rows(0)("TargetFieldMapping")
                _ftpPORT = objDT.Rows(0)("PORTNO")
                _PreftpPath = objDT.Rows(0)("PreFTPPath")
                _TargetDatahistoryFieldMapping = objDT.Rows(0)("TargetDatahistoryFieldMapping")
                _DisputeInOCRFieldMapping = objDT.Rows(0)("DisputeInOCRFieldMapping")
                Dim _uploadOnFTPBasedOnAuthMetrix As Int32 = objDC.ExecuteQryScaller("select count(*) from MMM_MST_authmetrix where doctype='" & _documentType & "' and eid=" & _eid & " and IsUploadOnFTP=1")
                If _uploadOnFTPBasedOnAuthMetrix > 0 Then
                    If objDC.ExecuteQryScaller("select count(*) from MMM_MST_authmetrix where doctype='" & _documentType & "' and eid=" & _eid & " and IsUploadOnFTP=1 and  aprstatus in(select curstatus from mmm_mst_doc where tid=" & _docID & ")") > 0 Then
                        res = True
                    End If
                Else
                    res = True
                End If
            End If
        End If
        Return res
    End Function



    Public Class KeyValue
        Private KeyData As String
        Public Property Key() As String
            Get
                Return KeyData
            End Get
            Set(ByVal value As String)
                KeyData = value
            End Set
        End Property

        Private KeyValue As String
        Public Property ValueData() As String
            Get
                Return KeyValue
            End Get
            Set(ByVal value As String)
                KeyValue = value
            End Set
        End Property

        Private SessiionType As Boolean
        Public Property IsSessionType() As Boolean
            Get
                Return SessiionType
            End Get
            Set(ByVal value As Boolean)
                SessiionType = value
            End Set
        End Property
    End Class



    Private Function OCRLOG(ByVal IsUploadOnFTP As Boolean) As Object
        objDC.ExecuteQryDT("insert into mmm_mst_ocrlog (eid,documenttype,targetdocumenttype,CreatedOn,IsUploadOnFTP,uid,docid,oldFileName,FTPFileName,TargetFieldMapping,TargetDatahistoryFieldMapping,DisputeInOCRFieldMapping) values (" & _eid & ",'" & _documentType & "','" & _targetDocumentType & "',getdate(),'" & IsUploadOnFTP & "'," & HttpContext.Current.Session("UID") & "," & _docID & ",'" & _OldfileNameFormat & "','" & _fileNameFormat & "','" & _targetFieldmapping & "','" & _TargetDatahistoryFieldMapping & "','" & _DisputeInOCRFieldMapping & "')")
    End Function

End Class
