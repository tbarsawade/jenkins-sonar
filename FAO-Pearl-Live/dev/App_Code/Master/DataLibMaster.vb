Imports System.Collections.Generic
Imports System.Linq
Imports System.Web
Imports System.Data
Imports System.Data.SqlClient
Imports System.Configuration
Imports ADODB
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Converters

Public Class DataLibMaster
#Region "variable"
    Public Shared conStr = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
    Public conObj As New SqlConnection(conStr)
#End Region
#Region "DataBaseConnMethods"
    Public Shared Function ExecuteNonQuery(ByRef conn As SqlConnection, ByVal cmdType As CommandType, ByVal cmdText As String, ByVal cmdParms As SqlParameter()) As Integer
        Dim cmd As SqlCommand = New SqlCommand
        Try
            PrepareCommand(cmd, conn, cmdType, cmdText, cmdParms)
            Dim parm As SqlParameter
            Dim val As Integer = cmd.ExecuteNonQuery()
            cmd.Parameters.Clear()
            Return val
        Catch ex As SqlException

            Throw New Exception("SQL Exception ", ex)
        Catch exx As Exception
            Throw New Exception("ExecuteNonQuery", exx)
        Finally
            cmd = Nothing
        End Try
    End Function
    Public Shared Function ExecuteDataSet(ByVal cmdType As CommandType, ByVal cmdText As String, Optional ByVal cmdParms As SqlParameter() = Nothing) As DataSet
        Dim cmd As SqlCommand = New SqlCommand
        Dim oDataAdapter As New SqlDataAdapter
        Dim oDataSet As New DataSet
        Dim conn As SqlConnection = New SqlConnection(conStr)
        Try
            PrepareCommand(cmd, conn, cmdType, cmdText, cmdParms)
            oDataAdapter.SelectCommand = cmd
            'cmd.Connection = conn
            oDataAdapter.Fill(oDataSet)
            cmd.Parameters.Clear()
            Return oDataSet
        Catch ex As SqlException

            Throw New Exception("SQL Exception ", ex)
        Catch exx As Exception
            Throw New Exception("ExecuteDataSet", exx)
        Finally
            conn.Dispose()
            cmd.Dispose()
            oDataAdapter.Dispose()
        End Try
    End Function

    Public Shared Function ExecuteDataSet(ByVal connString As String, ByVal cmdType As CommandType, ByVal cmdText As String, Optional ByVal cmdParms As SqlParameter() = Nothing) As DataSet
        Dim cmd As SqlCommand = New SqlCommand
        Dim conn As SqlConnection = New SqlConnection(connString)

        Dim oDataAdapter As New SqlDataAdapter
        Dim oDataSet As New DataSet
        Try
            PrepareCommand(cmd, conn, cmdType, cmdText, cmdParms)
            oDataAdapter.SelectCommand = cmd
            'cmd.Connection = conn
            oDataAdapter.Fill(oDataSet)
            cmd.Parameters.Clear()
            Return oDataSet
        Catch ex As SqlException

            Throw New Exception("SQL Exception ", ex)
        Catch exx As Exception
            Throw New Exception("ExecuteDataSet", exx)
        Finally
            conn.Close()
            cmd = Nothing
            oDataAdapter = Nothing
        End Try
    End Function
    Public Shared Function ExecuteScalar(ByVal cmdType As CommandType, ByVal cmdText As String, ByVal cmdParms As SqlParameter()) As String
        Dim cmd As SqlCommand = New SqlCommand
        Dim conn As SqlConnection = New SqlConnection(conStr)
        Try
            PrepareCommand(cmd, conn, cmdType, cmdText, cmdParms)
            Dim val As String = cmd.ExecuteScalar()
            cmd.Parameters.Clear()
            Return val
        Catch ex As SqlException
            Throw New Exception("SQL Exception ", ex)
        Catch exx As Exception
            Throw New Exception("ExeculateScalar", exx)
        Finally
            conn.Close()
            conn = Nothing
            cmd = Nothing
        End Try
    End Function
    Public Shared Sub PrepareCommand(ByRef cmd As SqlCommand, ByRef conn As SqlConnection, ByRef cmdType As CommandType, ByRef cmdText As String, ByRef cmdParms As SqlParameter())
        If Not conn.State = ConnectionState.Open Then
            conn.Open()
        End If
        Try
            cmd.Connection = conn
            cmd.CommandText = cmdText
            cmd.Parameters.Clear()
            '  cmd.ParameterCheck = True
            cmd.CommandType = cmdType
            If Not (IsNothing(cmdParms)) Then
                Dim parm As SqlParameter
                For Each parm In cmdParms
                    cmd.Parameters.Add(parm)
                Next
            End If
        Catch ex As SqlException
            Throw New Exception("SQL Exception ", ex)
        Catch exx As Exception
            Throw New Exception("PrepareCommand : ", exx)
        End Try
    End Sub

#End Region
#Region "serializer Methods"
    Public Shared Function JsonTableSerializer(dt As DataTable) As String
        Dim jsonData As String = ""
        Dim serializerSettings As New JsonSerializerSettings()
        Try
            If (dt.Rows.Count > 0) Then
                serializerSettings.Converters.Add(New DataTableConverter())
                jsonData = JsonConvert.SerializeObject(dt, Newtonsoft.Json.Formatting.None, serializerSettings)
            End If
            Return jsonData
        Catch ex As Exception
            Throw New Exception("DataLib.JsonTableSerializer")
        End Try
    End Function

#End Region

End Class

