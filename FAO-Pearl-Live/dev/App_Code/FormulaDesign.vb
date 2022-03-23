Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Xml
Imports System.Data.SqlClient
Imports System.IO
Imports System.Security
Imports System.Globalization
Imports System.Net
Public Class FormulaDesign
    Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString()
    Dim con As New SqlConnection(conStr)

    Public Function FormulaDesign(ByVal eid As Integer, ByVal FormulaName As String, ByVal isactive As Integer, ByVal Formuladesc As String, ByVal FormulaCategory As String, ByVal formsource As String, ByVal doctype As String, ByVal condition As String, ByVal createdby As Integer, Optional ByVal Tid As String = "0") As String

        Dim Result As String = String.Empty
        Dim da As New SqlDataAdapter("", con)
        Try

            da.SelectCommand.CommandText = "USP_Insert_Formula"
            da.SelectCommand.CommandType = CommandType.StoredProcedure
            da.SelectCommand.Parameters.AddWithValue("@tid", Tid)
            da.SelectCommand.Parameters.AddWithValue("@eid", eid)
            da.SelectCommand.Parameters.AddWithValue("@formulaname", FormulaName.ToString())
            da.SelectCommand.Parameters.AddWithValue("@isactive", isactive)
            da.SelectCommand.Parameters.AddWithValue("@formuladesc", Formuladesc.ToString)
            da.SelectCommand.Parameters.AddWithValue("@FormulaCategory", FormulaCategory.tostring())
            da.SelectCommand.Parameters.AddWithValue("@formsource", formsource.ToString())
            da.SelectCommand.Parameters.AddWithValue("@documenttype", doctype.ToString)
            da.SelectCommand.Parameters.AddWithValue("@condition", condition.ToString())
            da.SelectCommand.Parameters.AddWithValue("@createdby", createdby)
            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            Result = da.SelectCommand.ExecuteScalar()

        Catch ex As Exception
            Result = "Server Error: Operation Time Out"
        Finally
            If Not con Is Nothing Then
                con.Close()
            End If
            If Not da Is Nothing Then
                da.Dispose()
            End If
        End Try


        Return Result
    End Function





End Class
