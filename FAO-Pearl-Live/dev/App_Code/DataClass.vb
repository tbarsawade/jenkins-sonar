Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Web
Imports System.Data
Imports System.Data.SqlClient
Imports System.Configuration
Imports System.Collections

Public Class DataClass

    ''' <summary>
    ''' Summary description for DataClass
    ''' </summary>


    Private conn As SqlConnection = New SqlConnection

    Public Sub New()
        MyBase.New()
        conn.ConnectionString = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
    End Sub
    'Public Shared Function Constring() As String
    '   
    '    Return ConnectionString
    'End Function

    Public Function TranExecuteQryScaller(ByVal Qry As String, ByVal con As SqlConnection, ByVal tran As SqlTransaction) As String
        Dim rest As String = ""
        Try
            Dim cmd As SqlCommand = New SqlCommand
            cmd.Connection = con
            cmd.Transaction = tran
            cmd.CommandType = CommandType.Text
            cmd.CommandText = Qry
            If conn.State = ConnectionState.Closed Then
                conn.Open()
            End If
            rest = cmd.ExecuteScalar()
            cmd.Dispose()
        Catch ex As Exception
            Throw ex
        End Try
        Return rest
    End Function
    Public Function ExecuteQryScaller(ByVal Qry As String) As String
        Dim rest As String = ""
        Try
            Dim cmd As SqlCommand = New SqlCommand
            cmd.Connection = conn
            cmd.CommandType = CommandType.Text
            cmd.CommandText = Qry
            If conn.State = ConnectionState.Closed Then
                conn.Open()
            End If
            rest = cmd.ExecuteScalar()
            conn.Close()
            cmd.Dispose()
        Catch ex As Exception
            Return rest
        End Try
        Return rest
    End Function


    Public Function TranExecuteQryDT(ByVal Qry As String, ByVal con As SqlConnection, ByVal tran As SqlTransaction) As DataTable
        Dim dt As DataTable = New DataTable
        Try
            Dim cmd As SqlCommand = New SqlCommand
            cmd.Connection = con
            cmd.Transaction = tran
            cmd.CommandType = CommandType.Text
            cmd.CommandText = Qry
            Dim da As SqlDataAdapter = New SqlDataAdapter(cmd)
            da.Fill(dt)
            da.Dispose()
            cmd.Dispose()
        Catch ex As Exception
            Throw ex
        End Try
        Return dt
    End Function

    Public Function TranExecuteNonQuery(ByRef Qry As String, ByVal con As SqlConnection, ByVal tran As SqlTransaction) As Integer
        Dim result As Integer = 0
        Try
            Dim cmd As SqlCommand = New SqlCommand
            cmd.Connection = conn
            cmd.CommandType = CommandType.Text
            cmd.CommandText = Qry
            If conn.State = ConnectionState.Closed Then
                conn.Open()
            End If
            result = cmd.ExecuteNonQuery()
            con.Close()
            Return result
        Catch ex As Exception
            Throw ex
        End Try
        Return result
    End Function

    Public Function ExecuteNonQuery(ByRef Qry As String) As Integer
        Dim result As Integer = 0
        Try
            Dim cmd As SqlCommand = New SqlCommand
            cmd.Connection = conn
            cmd.CommandType = CommandType.Text
            cmd.CommandText = Qry
            If conn.State = ConnectionState.Closed Then
                conn.Open()
            End If
            result = cmd.ExecuteNonQuery()
            Return result
        Catch ex As Exception
            Throw ex
        End Try
        Return result
    End Function
    Public Function ExecuteQryDT(ByVal Qry As String) As DataTable
        Dim dt As DataTable = New DataTable
        Try
            Dim cmd As SqlCommand = New SqlCommand
            cmd.Connection = conn
            cmd.CommandType = CommandType.Text
            cmd.CommandText = Qry
            cmd.CommandTimeout = 0
            Dim da As SqlDataAdapter = New SqlDataAdapter(cmd)
            da.Fill(dt)
            da.Dispose()
            cmd.Dispose()
        Catch ex As Exception

            Throw ex
        End Try
        Return dt
    End Function

    Public Function TranExecuteProDT(ByVal sp As String, ByVal ht As Hashtable, ByVal con As SqlConnection, ByVal tran As SqlTransaction) As DataTable
        Dim dt As DataTable = New DataTable
        Try
            Dim cmd As SqlCommand = New SqlCommand
            cmd.Connection = con
            cmd.Transaction = tran
            cmd.CommandType = CommandType.StoredProcedure
            cmd.CommandText = sp
            Dim check As Boolean = False
            For Each de As DictionaryEntry In ht
                If (de.Key = "@Message") Then
                    cmd.Parameters.AddWithValue(CType(de.Key, String), de.Value).Direction = ParameterDirection.Output
                    check = True
                Else
                    cmd.Parameters.AddWithValue(CType(de.Key, String), de.Value)
                End If
            Next
            Dim da As SqlDataAdapter = New SqlDataAdapter(cmd)
            da.Fill(dt)
            If check Then
                Dim value As String = cmd.Parameters("@Message").Value.ToString
                If (value.ToString = "0") Then
                    dt.Columns.Add("Message")
                    Dim dr As DataRow = dt.NewRow
                    dr("Message") = value
                    dt.Rows.Add(dr)
                End If
            End If
            da.Dispose()
            cmd.Dispose()
        Catch ex As Exception
            Throw ex
        End Try
        Return dt
    End Function

    Public Function ExecuteProScaller(ByVal sp As String, ByVal ht As Hashtable) As String
        Dim result As String = ""
        Try

            Dim cmd As SqlCommand = New SqlCommand
            cmd.Connection = conn
            cmd.CommandType = CommandType.StoredProcedure
            cmd.CommandText = sp
            Dim check As Boolean = False
            For Each de As DictionaryEntry In ht
                If (de.Key = "@Message") Then
                    cmd.Parameters.AddWithValue(CType(de.Key, String), de.Value).Direction = ParameterDirection.Output
                    check = True
                Else
                    cmd.Parameters.AddWithValue(CType(de.Key, String), de.Value)
                End If
            Next
            Dim da As SqlDataAdapter = New SqlDataAdapter(cmd)
            If conn.State = ConnectionState.Closed Then
                conn.Open()
            End If
            result = cmd.ExecuteScalar()

            da.Dispose()
            cmd.Dispose()
            conn.Close()
        Catch ex As Exception
            Throw ex
        End Try
        Return Result
    End Function
    Public Function ExecuteProDT(ByVal sp As String, ByVal ht As Hashtable) As DataTable
        Dim dt As DataTable = New DataTable
        Try
            Dim cmd As SqlCommand = New SqlCommand
            cmd.Connection = conn
            cmd.CommandType = CommandType.StoredProcedure
            cmd.CommandText = sp
            Dim check As Boolean = False
            For Each de As DictionaryEntry In ht
                If (de.Key = "@Message") Then
                    cmd.Parameters.AddWithValue(CType(de.Key, String), de.Value).Direction = ParameterDirection.Output
                    check = True
                Else
                    cmd.Parameters.AddWithValue(CType(de.Key, String), de.Value)
                End If
            Next
            Dim da As SqlDataAdapter = New SqlDataAdapter(cmd)
            da.Fill(dt)
            If check Then
                Dim value As String = cmd.Parameters("@Message").Value.ToString
                If (value.ToString = "0") Then
                    dt.Columns.Add("Message")
                    Dim dr As DataRow = dt.NewRow
                    dr("Message") = value
                    dt.Rows.Add(dr)
                End If
            End If
            da.Dispose()
            cmd.Dispose()
        Catch ex As Exception
            Throw ex
        End Try
        Return dt
    End Function

    Public Function ExecuteQryDS(ByVal Qry As String, Optional ByVal dsName As String = "Table") As DataSet
        Dim ds As DataSet = New DataSet
        Try
            Dim cmd As SqlCommand = New SqlCommand
            cmd.Connection = conn
            cmd.CommandText = Qry
            cmd.CommandType = CommandType.Text
            Dim da As SqlDataAdapter = New SqlDataAdapter(cmd)
            da.Fill(ds, dsName)
            cmd.Dispose()
            da.Dispose()
        Catch ex As Exception

        End Try
        Return ds
    End Function

    Public Function TranExecuteProDS(ByVal sp As String, ByVal ht As Hashtable, ByVal con As SqlConnection, ByVal tran As SqlTransaction) As DataSet
        Dim ds As DataSet = New DataSet
        Try
            Dim cmd As SqlCommand = New SqlCommand
            cmd.Connection = con
            cmd.Transaction = tran
            cmd.CommandType = CommandType.StoredProcedure
            cmd.CommandText = sp
            Dim check As Boolean = False
            For Each de As DictionaryEntry In ht
                cmd.Parameters.AddWithValue(CType(de.Key, String), de.Value)
            Next
            Dim da As SqlDataAdapter = New SqlDataAdapter(cmd)
            da.Fill(ds)

            da.Dispose()
            cmd.Dispose()
        Catch ex As Exception
            Throw ex
        End Try
        Return ds
    End Function

    Public Function ExecuteProDS(ByVal sp As String, ByVal ht As Hashtable) As DataSet
        Dim ds As DataSet = New DataSet
        Try
            Dim cmd As SqlCommand = New SqlCommand
            cmd.Connection = conn
            cmd.CommandText = sp
            cmd.CommandType = CommandType.StoredProcedure
            For Each de As DictionaryEntry In ht
                cmd.Parameters.AddWithValue(CType(de.Key, String), de.Value)
            Next
            Dim da As SqlDataAdapter = New SqlDataAdapter(cmd)
            da.Fill(ds)
            cmd.Dispose()
            da.Dispose()
        Catch ex As Exception

        End Try
        Return ds
    End Function

    Public Function ExecuteQryDR(ByVal UserEmail As String, ByVal Password As String) As SqlDataReader
        Try
            If (conn.State = ConnectionState.Closed) Then
                conn.Open()
            End If
            Dim cmd As SqlCommand = New SqlCommand
            cmd.Connection = conn
            cmd.CommandType = CommandType.Text
            cmd.CommandText = ("Select Email, Password from Users where Email='" _
                        + (UserEmail + ("' and Password='" _
                        + (Password + "' and UserType='Admin'"))))
            Dim dr As SqlDataReader = cmd.ExecuteReader
            Return dr
            If (conn.State = ConnectionState.Open) Then
                conn.Close()
            End If
            cmd.Dispose()
        Catch ex As Exception
            Throw ex
        End Try
    End Function

    Public Function GetEIDBasedOnAPIKEY(ByVal APIKey As String) As Integer
        Dim Result As Integer = 0
        Try
            Dim cmd As SqlCommand = New SqlCommand
            cmd.Connection = conn
            cmd.CommandType = CommandType.Text
            cmd.CommandText = "select eid from mmm_mst_entity where apikey='" & APIKey & "'"
            Dim da As SqlDataAdapter = New SqlDataAdapter(cmd)
            If conn.State = ConnectionState.Closed Then
                conn.Open()
            End If
            Result = Convert.ToInt32(da.SelectCommand.ExecuteScalar())
        Catch ex As Exception
            Result = 0
        End Try
        Return Result
    End Function

    Public Function GetSSNLoginEntry(Token As String) As DataTable
        Dim dt As DataTable = New DataTable
        Try
            Dim cmd As SqlCommand = New SqlCommand
            cmd.Connection = conn
            'cmd.Transaction = tran
            cmd.CommandType = CommandType.StoredProcedure
            cmd.Parameters.AddWithValue("@Token", Token)
            cmd.CommandText = "uspGetGetSSNLoginEntry"
            Dim da As SqlDataAdapter = New SqlDataAdapter(cmd)
            da.Fill(dt)
            da.Dispose()
            cmd.Dispose()
        Catch ex As Exception
            Throw ex
        End Try
        Return dt
    End Function

    Public Function updateSSNLoginEntry(Token As String)

        Try
            Dim cmd As SqlCommand = New SqlCommand
            cmd.Connection = conn
            'cmd.Transaction = tran
            cmd.CommandType = CommandType.StoredProcedure
            cmd.Parameters.AddWithValue("@Token", Token)
            cmd.CommandText = "usp_updateSSNLoginEntry"
            conn.Open()
            cmd.ExecuteNonQuery()
            conn.Close()
            cmd.Dispose()
        Catch ex As Exception
            Throw ex
        End Try

    End Function


End Class
