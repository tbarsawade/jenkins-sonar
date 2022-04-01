Imports System.Collections.Generic
Imports System.Linq
Imports System.Web
Imports System.Data
Imports System.Data.SqlClient
Imports System.Configuration
Imports ADODB


'Namespace Models.Utility
Public Class DataLib

    'Public Shared conObj As New SqlConnection(ConfigurationManager.AppSettings("ConnectionString").ToString())
    Public Shared conObj As New SqlConnection(ConfigurationManager.ConnectionStrings("conStr").ConnectionString.ToString())
    Public Enum Connection
        ConnectionString
    End Enum
    Public Shared Function GetConnectionString(connType As Connection) As String
        Dim retValue As String = ""
        Select Case connType
            Case Connection.ConnectionString
                If True Then
                    retValue = ConfigurationManager.ConnectionStrings("conStr").ConnectionString 'ConfigurationManager.AppSettings("ConnectionString").ToString()
                    Exit Select
                End If
        End Select
        Return retValue
    End Function

    Public Shared Function GetStoredProcData(connStr As String, strSP As String, arrSPParams As SqlParameter()) As DataSet
        Dim ds As New DataSet()
        Using con As New SqlConnection(connStr)
            'Prepare the Command
            Using cmd As SqlCommand = con.CreateCommand()
                cmd.Connection = con
                cmd.CommandText = strSP
                cmd.CommandType = System.Data.CommandType.StoredProcedure
                'Add stored procedure parameters to Command
                If arrSPParams IsNot Nothing Then
                    For Each param As SqlParameter In arrSPParams
                        cmd.Parameters.Add(param)
                    Next
                End If
                Using adapter As New SqlDataAdapter(cmd)
                    adapter.SelectCommand = cmd
                    'DO NOT open the connection, allow the fill command to do this
                    adapter.Fill(ds)
                    adapter.SelectCommand = Nothing
                End Using
            End Using
        End Using
        Return ds
    End Function
    Public Shared Function ExecuteStoredProcData(connType As Connection, strSP As String, arrSPParams As SqlParameter()) As Integer
        Dim connStr As String = GetConnectionString(connType)
        Dim RowsAffected As Integer = 0
        Using con As New SqlConnection(connStr)
            'Prepare the Command
            Using cmd As SqlCommand = con.CreateCommand()
                cmd.CommandTimeout = 10000
                cmd.Connection = con
                cmd.CommandText = strSP
                cmd.CommandType = System.Data.CommandType.StoredProcedure

                'Add stored procedure parameters to Command
                If arrSPParams IsNot Nothing Then
                    For Each param As SqlParameter In arrSPParams
                        cmd.Parameters.Add(param)
                    Next
                End If
                Try
                    If con.State = ConnectionState.Open Then
                        con.Close()
                    End If
                    con.Open()
                    RowsAffected = Convert.ToInt32(cmd.ExecuteScalar())
                    con.Close()
                Catch ex As Exception
                    Throw
                Finally
                    If con.State = ConnectionState.Open Then
                        con.Close()
                    End If

                End Try
            End Using
        End Using
        Return RowsAffected
    End Function
    Public Shared Function GetScalarStringStoredProcData(connType As Connection, strSP As String, arrSPParams As SqlParameter()) As String
        Dim connStr As String = String.Empty
        Dim retString As String = String.Empty
        connStr = GetConnectionString(connType)
        Using con As New SqlConnection(connStr)
            'Prepare the Command
            Using cmd As SqlCommand = con.CreateCommand()
                cmd.Connection = con
                cmd.CommandText = strSP
                cmd.CommandType = System.Data.CommandType.StoredProcedure
                'Add stored procedure parameters to Command
                If arrSPParams IsNot Nothing Then
                    For Each param As SqlParameter In arrSPParams
                        cmd.Parameters.Add(param)
                    Next
                End If
                If con.State = ConnectionState.Open Then
                    con.Close()
                End If
                con.Open()
                retString = Convert.ToString(cmd.ExecuteScalar())
                'retString = arrSPParamsOut[0].Value.ToString();
                con.Close()
            End Using
        End Using
        Return retString
    End Function
    'Prashant
    Public Shared Function ExecuteNonQuery(ByVal connString As String, ByVal cmdType As CommandType, ByVal cmdText As String, ByVal cmdParms As SqlParameter()) As Integer
        Dim cmd As SqlCommand = New SqlCommand
        Dim conn As SqlConnection = New SqlConnection(connString)
        'Dim trans As SqlTransaction = conn.BeginTransaction("BuilderTransaction")
        Try
            PrepareCommand(cmd, conn, cmdType, cmdText, cmdParms)
            Dim val As Integer = cmd.ExecuteNonQuery()
            cmd.Parameters.Clear()
            Return val
        Catch ex As SqlException
            Throw New Exception("SQL Exception1 " & ex.Message)
        Catch exx As Exception
            Throw New Exception("ExecuteNonQuery Function", exx)
        Finally                'Add this for finally closing the connection and destroying the command
            conn.Close()
            cmd = Nothing
            cmdParms = Nothing
        End Try
    End Function

    Public Shared Function ExecuteNonQuery(ByRef conn As SqlConnection, ByVal cmdType As CommandType, ByVal cmdText As String, ByVal cmdParms As SqlParameter()) As Integer
        Dim cmd As SqlCommand = New SqlCommand
        Try
            PrepareCommand(cmd, conn, cmdType, cmdText, cmdParms)
            Dim parm As SqlParameter
            'For Each parm In cmdParms
            '    cmd.Parameters.Add(parm)
            'Next
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

    Public Shared Function ExecuteReader(ByRef conn As SqlConnection, ByVal cmdType As CommandType, ByVal cmdText As String, Optional ByVal cmdParms As SqlParameter() = Nothing) As SqlDataReader
        Dim cmd As SqlCommand = New SqlCommand
        'Dim conn As OleDbConnection = New OleDbConnection(connString)
        ' we use a try/catch here because if the method throws an exception we want to 
        ' close the connection throw ex code, because no datareader will exist, hence the 
        ' commandBehaviour.CloseConnection will not work
        Try
            PrepareCommand(cmd, conn, cmdType, cmdText, cmdParms)
            Dim rdr As SqlDataReader = cmd.ExecuteReader()
            'cmd.Parameters.Clear()
            Return rdr
        Catch ex As SqlException

            Throw New Exception("SQL Exception ", ex)
        Catch exx As Exception
            Throw New Exception("ExecuteReader", exx)
        Finally
            cmd = Nothing
        End Try
    End Function

    Public Shared Function ExecuteTable(ByVal connString As String, ByVal cmdType As CommandType, ByVal cmdText As String, Optional ByVal cmdParms As SqlParameter() = Nothing) As DataTable

        Dim cmd As SqlCommand = New SqlCommand
        Dim conn As SqlConnection = New SqlConnection(connString)
        Dim oDataAdapter As New SqlDataAdapter
        Dim oDataTable As New DataTable
        Try
            PrepareCommand(cmd, conn, cmdType, cmdText, cmdParms)
            'cmd.Parameters = cmdParms
            oDataAdapter.SelectCommand = cmd
            oDataAdapter.Fill(oDataTable)
            cmd.Parameters.Clear()
            Return oDataTable
        Catch ex As SqlException

            Throw New Exception("SQL Exception : " & ex.Message, ex)
        Catch exx As Exception
            Throw New Exception("ExecuteTable Exception :", exx)
        Finally
            conn.Close()
            cmd = Nothing
            oDataAdapter = Nothing
        End Try
    End Function

    Public Shared Function ExecuteTable(ByRef oConnection As SqlConnection, ByVal cmdType As CommandType, ByVal cmdText As String, Optional ByVal cmdParms As SqlParameter() = Nothing) As DataTable

        Dim cmd As SqlCommand = New SqlCommand
        Dim oDataAdapter As New SqlDataAdapter
        Dim oDataTable As New DataTable
        Try
            PrepareCommand(cmd, oConnection, cmdType, cmdText, cmdParms)
            oDataAdapter.SelectCommand = cmd
            oDataAdapter.Fill(oDataTable)
            cmd.Parameters.Clear()
            Return oDataTable
        Catch ex As Exception
            Throw New Exception("ExecuteTable", ex)
        Finally
            cmd.Connection.Close()
            cmd.Dispose()
            oDataAdapter.Dispose()
            oDataTable.Dispose()
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


    Public Shared Function ExecuteRow(ByVal connString As String, ByVal cmdType As CommandType, ByVal cmdText As String, Optional ByVal cmdParms As SqlParameter() = Nothing) As DataRow
        Dim cmd As SqlCommand = New SqlCommand
        Dim conn As SqlConnection = New SqlConnection(connString)
        Dim oDataAdapter As New SqlDataAdapter
        Dim oDataRow As DataRow
        Dim oDataTable As New DataTable
        Try
            PrepareCommand(cmd, conn, cmdType, cmdText, cmdParms)
            oDataAdapter.SelectCommand = cmd
            oDataAdapter.Fill(oDataTable)
            cmd.Parameters.Clear()
            If oDataTable.Rows.Count = 0 Then
                Return Nothing
            Else
                Dim oRow As DataRow = oDataTable.Rows(0)
                Return oRow
            End If
        Catch ex As SqlException

            Throw New Exception("SQL Exception ", ex)
        Catch exx As Exception
            Throw New Exception("ExecuteRow", exx)
        Finally
            conn.Close()
            oDataTable = Nothing
            cmd = Nothing
            oDataAdapter = Nothing
        End Try
    End Function

    Public Shared Function ExecuteRow(ByRef oConnection As SqlConnection, ByVal cmdType As CommandType, ByVal cmdText As String, Optional ByVal cmdParms As SqlParameter() = Nothing) As DataRow
        Dim cmd As SqlCommand = New SqlCommand
        Dim conn As SqlConnection = oConnection
        Dim oDataAdapter As New SqlDataAdapter
        Dim oDataRow As DataRow
        Dim oDataTable As New DataTable
        Try
            PrepareCommand(cmd, conn, cmdType, cmdText, cmdParms)
            oDataAdapter.SelectCommand = cmd
            oDataAdapter.Fill(oDataTable)
            cmd.Parameters.Clear()
            If oDataTable.Rows.Count = 0 Then
                Return Nothing
            Else
                Dim oRow As DataRow = oDataTable.Rows(0)
                Return oRow
            End If
        Catch ex As SqlException

            Throw New Exception("SQL Exception ", ex)
        Catch exx As Exception
            Throw New Exception("ExeculateScalar", exx)
        Finally
            oDataTable = Nothing
            cmd.Connection.Close()
            cmd = Nothing
            oDataAdapter = Nothing
        End Try
    End Function

    Public Shared Function ExecuteScalar(ByVal connString As String, ByVal cmdType As CommandType, ByVal cmdText As String, ByVal cmdParms As SqlParameter()) As Object
        Dim cmd As SqlCommand = New SqlCommand
        Dim conn As SqlConnection = New SqlConnection(connString)
        Try
            PrepareCommand(cmd, conn, cmdType, cmdText, cmdParms)
            Dim val As Object = cmd.ExecuteScalar()

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

    Public Shared Function ExecuteScalar(ByRef conn As SqlConnection, ByVal cmdType As CommandType, ByVal cmdText As String, ByVal cmdParms As SqlParameter()) As Object
        Dim cmd As SqlCommand = New SqlCommand
        Try
            PrepareCommand(cmd, conn, cmdType, cmdText, cmdParms)
            Dim val As Object = cmd.ExecuteScalar()
            cmd.Parameters.Clear()
            Return val
        Catch ex As SqlException
            Throw New Exception("SQL Exception ", ex)
        Catch exx As Exception
            Throw New Exception("ExeculateScalar", exx)
        Finally
            cmd.Connection.Close()
            cmd = Nothing
        End Try
    End Function

    Public Shared Function PrepareCommand(ByRef cmd As SqlCommand, ByRef conn As SqlConnection, ByRef cmdType As CommandType, ByRef cmdText As String, ByRef cmdParms As SqlParameter()) As Boolean
        If Not conn.State = ConnectionState.Open Then
            'MsgBox("Connection open")
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
    End Function

    Public Shared Function ExcuteAdapter(ByVal connString As String, ByVal oTable As DataTable, ByVal cmdText As String, Optional ByRef lngMaxID As Long = 0) As Boolean
        Dim conn As SqlConnection
        Dim oDataAdapter As New SqlDataAdapter
        Dim oSqlCmd As New SqlCommand
        Dim oCmdBuilder As SqlCommandBuilder
        Try
            conn = New SqlConnection(connString)
            If Not conn.State = ConnectionState.Open Then
                conn.Open()
            End If
            oSqlCmd.Connection = conn
            oSqlCmd.CommandText = cmdText
            oSqlCmd.CommandType = CommandType.Text
            oDataAdapter.SelectCommand = oSqlCmd
            oCmdBuilder = New SqlCommandBuilder(oDataAdapter)
            oCmdBuilder.GetUpdateCommand()
            oCmdBuilder.GetInsertCommand()
            oCmdBuilder.GetDeleteCommand()
            oDataAdapter.Update(oTable)
            oDataAdapter.SelectCommand = New SqlCommand("SELECT @@IDENTITY", conn)
            lngMaxID = CType(oDataAdapter.SelectCommand.ExecuteScalar(), Long)

        Catch ex As SqlException
            Throw New Exception("SQL Exception ", ex)
        Catch exx As Exception
            Throw New Exception("ExeculateAdapter", exx)
        Finally
            ' cmd.Connection.Close()
            If conn.State = ConnectionState.Open Then conn.Close()
            oSqlCmd = Nothing
            oDataAdapter = Nothing
            oCmdBuilder = Nothing
        End Try

    End Function

    Public Shared Function ExcuteAdapter(ByRef conn As SqlConnection, ByVal oTable As DataTable, ByVal cmdText As String, Optional ByRef lngMaxID As Long = 0) As Boolean

        Dim oDataAdapter As New SqlDataAdapter
        Dim oSqlCmd As New SqlCommand
        Dim oCmdBuilder As SqlCommandBuilder
        Try
            If Not conn.State = ConnectionState.Open Then
                conn.Open()
            End If
            oSqlCmd.Connection = conn
            oSqlCmd.CommandText = cmdText
            oSqlCmd.CommandType = CommandType.Text
            oDataAdapter.SelectCommand = oSqlCmd
            oCmdBuilder = New SqlCommandBuilder(oDataAdapter)
            oCmdBuilder.GetUpdateCommand()
            oCmdBuilder.GetInsertCommand()
            oCmdBuilder.GetDeleteCommand()
            oDataAdapter.Update(oTable)
            oDataAdapter.SelectCommand = New SqlCommand("SELECT @@IDENTITY", conn)
            lngMaxID = CType(oDataAdapter.SelectCommand.ExecuteScalar(), Long)

        Catch ex As SqlException
            Throw New Exception("SQL Exception ", ex)
        Catch exx As Exception
            Throw New Exception("ExeculateAdapter", exx)
        Finally
            ' cmd.Connection.Close()
            If conn.State = ConnectionState.Open Then conn.Close()
            oSqlCmd = Nothing
            oDataAdapter = Nothing
            oCmdBuilder = Nothing
        End Try

    End Function
End Class

'End Namespace
