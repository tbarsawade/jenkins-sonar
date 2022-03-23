Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Globalization
Imports System.Text
Imports ExpressionEvaluator

Public Class FormulaLib

    Public Property Result() As Integer
        Get
            Return m_Result
        End Get
        Set(value As Integer)
            m_Result = value
        End Set
    End Property

    Private m_Result As Integer

    Public Function ADD(Parameter As String) As Double
        Dim Result As Double = 0
        If Parameter.Contains(",") Then
            Dim arr = Parameter.Split(",")
            For i As Integer = 0 To arr.Length - 1
                Dim var As Double
                Try
                    var = Double.Parse(arr(i))
                Catch ex As Exception
                    var = 0
                End Try
                Result = Result + var
            Next
        Else
            Result = Parameter
        End If
        Return Result
    End Function


    Public Function Max(Parameter As String) As Double
        Dim Result As Double = 0
        Dim arr = Parameter.Split(New String() {","}, StringSplitOptions.RemoveEmptyEntries)
        Dim arrList As New ArrayList
        For i As Integer = 0 To arr.Count - 1
            Try
                arrList.Add(Double.Parse(arr(i)))
            Catch ex As Exception
                arrList.Add(Double.Parse(arr(i)))
            End Try
        Next
        Result = arrList.Cast(Of Double)().Max()
        Return Result
    End Function


    Public Function Min(Parameter As String) As Double
        Dim Result As Double = 0
        Dim arr = Parameter.Split(New String() {","}, StringSplitOptions.RemoveEmptyEntries)
        Dim arrList As New ArrayList
        For i As Integer = 0 To arr.Count - 1
            Try
                arrList.Add(Double.Parse(arr(i)))
            Catch ex As Exception
                arrList.Add(Double.Parse(arr(i)))
            End Try
        Next
        Result = arrList.Cast(Of Double)().Min()
        Return Result
    End Function


    Public Function Ceiling(Parameter As String) As Double
        Dim Result As Double = 0
        Try
            Result = Math.Ceiling(Double.Parse(Parameter))
        Catch ex As Exception
            Result = 0
            Return Result
        End Try
        Return Result
    End Function

    Public Function Floor(Parameter As String) As Double
        Dim Result As Double = 0
        Try
            Result = Math.Floor(Double.Parse(Parameter))
        Catch ex As Exception
            Result = 0
            Return Result
        End Try
        Return Result
    End Function


    Public Function Abs(Parameter As String) As Double
        Dim Result As Double = 0
        Try
            Result = Math.Abs(Double.Parse(Parameter))
        Catch ex As Exception
            Result = 0
            Return Result
        End Try
        Return Result
    End Function

    'List OF datetime function

    Public Function GETDATE() As String
        Dim Result As String = ""
        Dim dt As Date = Date.Today
        Result = dt.ToString("dd/MM/yy", CultureInfo.InvariantCulture)
        Return Result
    End Function


    Public Function GetDays(Parameter As String) As String
        Dim Month As String
        Dim Year As String
        Dim arr = Parameter.Split("/")
        Month = arr(0)
        Year = "20" & arr(1)
        ' daysInJuly gets 31. '
        Dim daysInMonth As Integer = System.DateTime.DaysInMonth(Year, Month)
        Return daysInMonth
    End Function

    Public Function GETDATE(Parameter As String) As String
        Dim Date1 As String
        Dim arr = Parameter.Split("/")
        Date1 = arr(0)
        Return Date1
    End Function

    Public Function GETYEAR(Parameter As String) As String
        Dim Year As String
        Dim arr = Parameter.Split("/")
        Year = arr(2)
        Return Year
    End Function

    Public Function GETMONTH(Parameter As String) As String
        Dim Month As String
        Dim arr = Parameter.Split("/")
        Month = arr(1)
        Return Month
    End Function

    Public Function DATERANGE(Parameter As String) As String
        Dim Month As String

        Dim arr = Parameter.Split("/")
        Month = arr(1)
        Return Month
    End Function

    Public Function GETDAY(Parameter As String) As String
        Dim Day As String = ""
        Dim arr = Parameter.Split("/")
        Dim dt As New Date(arr(2), arr(1), arr(0))
        Dim Day1 = Convert.ToInt32(dt.DayOfWeek.ToString("d"))
        ' ''dt.ToString("dd/MM/yy", CultureInfo.InvariantCulture)
        'If dt.DayOfWeek = DayOfWeek.Sunday Then
        '    Day = "Sunday"
        'End If
        Select Case Day1
            Case 1
                Day = "Monday"
            Case 2
                Day = "Tuesday"
            Case 3
                Day = "Wednesday"
            Case 4
                Day = "Thursday"
            Case 5
                Day = "Friday"
            Case 6
                Day = "Saturday"
            Case 0
                Day = "Sunday"
        End Select
        Return Day
    End Function

    Public Function CONCATE(Parameter As String) As String
        Dim Result = ""
        Result = Parameter.Replace(",", String.Empty)
        Return Result
    End Function

    Public Function SplitFun(Spliter As String, Str As String) As String
        Str = Str.Replace(" ", String.Empty)
        Dim ret As String = ""
        Dim CharAr As Char() = Str
        Dim Start_Pos = 0
        Dim END_Pos = 0
        Dim IsStarterFound = False
        'Checking nested formula 
        If HasNestedFormula(Str) = True Then
            For i As Integer = 0 To CharAr.Count - 1
                'Finding Start position of formula
                If CharAr(i).ToString = "(" Then
                    Start_Pos = i
                    IsStarterFound = True
                End If
                'Finding End position of formula
                If CharAr(i).ToString = ")" Then
                    END_Pos = i
                End If
                'Get innermost Formula!!!! Now take slice of that formula
                If Start_Pos > 0 And END_Pos > 0 Then
                    Dim Formula_StartPOS = 0
                    Dim Own_Bracket_Found = False
                    For k As Integer = Start_Pos - 1 To 0 Step -1
                        'If CharAr(k).ToString = "(" And Own_Bracket_Found = False Then
                        '    Own_Bracket_Found = True
                        '    Continue For
                        'End If
                        'If (CharAr(k).ToString = "(" Or CharAr(k) = ",") And Own_Bracket_Found = True Then
                        If (CharAr(k).ToString = "(" Or CharAr(k) = ",") Then
                            Formula_StartPOS = k + 1
                            Dim StrTest = CharAr(k).ToString
                            Exit For
                        End If
                        'k = k - 1
                    Next
                    'Now getting formula from the string 
                    Dim ForMulaSlice = Str.Substring(Formula_StartPOS, END_Pos - Formula_StartPOS + 1)
                    Dim Result = ExecuteFormula(ForMulaSlice)
                    'Replace the innermostformula with the values
                    Str = Str.Replace(ForMulaSlice, Result)
                    CharAr = Str
                    Start_Pos = 0
                    END_Pos = 0
                    IsStarterFound = False
                    i = 0
                    'checking is there any more nested formula 
                    If (HasNestedFormula(Str) = True) Then
                        Continue For
                    Else
                        Exit For
                    End If
                End If
            Next
        End If
        'Executing Final Formula
        ret = ExecuteFormula(Str)
        Return ret
    End Function


    Public Function HasNestedFormula(Exp As String) As Boolean
        Dim ret = False
        Dim CharAr As Char() = Exp
        Dim ISBracketFound = 0
        For i As Integer = 0 To CharAr.Count - 1
            If CharAr(i).ToString = "(" Then
                ISBracketFound = ISBracketFound + 1
                If ISBracketFound > 1 Then
                    ret = True
                    Exit For
                End If
            End If
        Next
        Return ret
    End Function

    Public Function ExecuteFormula(Exp As String) As String
        Dim Result As String = "10"
        Dim Start_POS As Integer = 0
        Dim END_POS As Integer = 0
        Start_POS = Exp.IndexOf("(")
        END_POS = Exp.IndexOf(")")
        Dim Strparameter = ""
        Strparameter = Exp.Substring(Start_POS + 1, END_POS - Start_POS - 1)
        Result = ADD(Strparameter)
        Return Result
    End Function

    Public Function Evaluate() As Boolean
        Dim ret As Boolean = False
        Dim res As New CompiledExpression("5>3 || 2<1 ")
        Dim result = res.Eval()
        Return ret
    End Function

End Class
