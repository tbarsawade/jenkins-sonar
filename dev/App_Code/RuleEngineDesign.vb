Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Xml
Imports System.Data.SqlClient
Imports System.IO
Imports System.Security
Imports System.Globalization
Imports System.Net

Public Class RuleEngineDesign

    Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString()
    Dim con As New SqlConnection(conStr)

    Public Function RuleEngine(ByVal eid As Integer, ByVal RuleName As String, ByVal ruledesc As String, ByVal formsource As String, ByVal doctype As String, ByVal docnature As String, ByVal whentorun As String, ByVal condition As String, ByVal sucactiontype As String, ByVal succmsg As String, ByVal failactiontype As String, ByVal failmsg As String, ByVal createdby As Integer, Optional ByVal sucactionfls As String = Nothing, Optional ByVal failactionflds As String = Nothing, Optional ByVal rid As String = "0", Optional ByVal ControlField As String = Nothing, Optional ByVal TargetControlField As String = Nothing) As String


        Dim Result As String = String.Empty

        Dim da As New SqlDataAdapter("", con)
        Try

            da.SelectCommand.CommandText = "USP_Insert_ruleEngine"
            da.SelectCommand.CommandType = CommandType.StoredProcedure
            da.SelectCommand.Parameters.AddWithValue("@rid", rid)
            da.SelectCommand.Parameters.AddWithValue("@eid", eid)
            da.SelectCommand.Parameters.AddWithValue("@rulename", RuleName.ToString())
            'da.SelectCommand.Parameters.AddWithValue("@isactive", isactive)
            da.SelectCommand.Parameters.AddWithValue("@ruledesc", ruledesc.ToString)
            da.SelectCommand.Parameters.AddWithValue("@formsource", formsource.ToString())
            da.SelectCommand.Parameters.AddWithValue("@documenttype", doctype.ToString)
            da.SelectCommand.Parameters.AddWithValue("@docnature", docnature.ToString())
            da.SelectCommand.Parameters.AddWithValue("@whentorun", whentorun.ToString())
            da.SelectCommand.Parameters.AddWithValue("@condition", condition.ToString())
            da.SelectCommand.Parameters.AddWithValue("@SuccActiontype", sucactiontype.ToString())
            da.SelectCommand.Parameters.AddWithValue("@SuccActionField", sucactionfls.ToString())
            da.SelectCommand.Parameters.AddWithValue("@SuccMsg", succmsg.ToString())
            da.SelectCommand.Parameters.AddWithValue("@FailActiontype", failactiontype.ToString())
            da.SelectCommand.Parameters.AddWithValue("@FailActionField", failactionflds.ToString())
            da.SelectCommand.Parameters.AddWithValue("@ErrorMsg", failmsg.ToString())
            da.SelectCommand.Parameters.AddWithValue("@createdby", createdby)
            da.SelectCommand.Parameters.AddWithValue("@controlfield", ControlField)
            da.SelectCommand.Parameters.AddWithValue("@targetcontrolfield", TargetControlField)

            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            Result = da.SelectCommand.ExecuteScalar()

        Catch ex As Exception
            Result = ex.ToString()
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
