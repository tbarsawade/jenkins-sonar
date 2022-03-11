Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Data.SqlClient
Imports System.Configuration
Imports System.Globalization

Public Class BpmHelper

    Dim con As SqlConnection
    Dim Eid As Integer

    Public Sub New(Entityid As Integer)
        Eid = Entityid
        con = New SqlConnection(ConfigurationManager.ConnectionStrings("conStr").ConnectionString)
    End Sub

    Sub New()
        con = New SqlConnection(ConfigurationManager.ConnectionStrings("conStr").ConnectionString)
    End Sub

    Public Function EcecDataSet(Query As String) As ReturnObj
        Dim ReturnOb As New ReturnObj
        Try
            Dim ds As New DataSet
            Dim da As New SqlDataAdapter(Query, con)
            da.Fill(ds)
            ReturnOb.Success = True
            ReturnOb.obj = ds
            Return ReturnOb
        Catch ex As Exception
            ReturnOb.Success = False
            ReturnOb.ErrorMessage = ex.Message
        Finally

        End Try
        Return ReturnOb
    End Function

    Public Function ExecDML(Query As String) As Integer
        Try
            Dim cmd As New SqlCommand(Query, con)
            If Not con.State = ConnectionState.Open Then
                con.Open()
            End If
            Return cmd.ExecuteNonQuery()
            con.Close()
        Catch ex As Exception
            If Not con.State = ConnectionState.Closed Then
                con.Close()
            End If
            Return -1
        End Try
    End Function

End Class

Public Class ReturnObj
    Public Property Success As Boolean
    Public Property ErrorMessage As String
    Public Property obj As DataSet
End Class

