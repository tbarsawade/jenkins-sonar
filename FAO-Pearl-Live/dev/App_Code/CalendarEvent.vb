Imports System.Collections.Generic
Imports System.Linq
Imports System.Web

''' <summary>
''' Summary description for CalendarEvent
''' </summary>
Public Class CalendarEvent
    Public Property id() As Integer
        Get
            Return m_id
        End Get
        Set(value As Integer)
            m_id = Value
        End Set
    End Property
    Private m_id As Integer
    Public Property title() As String
        Get
            Return m_title
        End Get
        Set(value As String)
            m_title = Value
        End Set
    End Property
    Private m_title As String
    Public Property description() As String
        Get
            Return m_description
        End Get
        Set(value As String)
            m_description = Value
        End Set
    End Property
    Private m_description As String
    Public Property start() As DateTime
        Get
            Return m_start
        End Get
        Set(value As DateTime)
            m_start = value
        End Set
    End Property
    Private m_start As DateTime
    Public Property [end]() As DateTime
        Get
            Return m_end
        End Get
        Set(value As DateTime)
            m_end = Value
        End Set
    End Property
    Private m_end As DateTime

    Public Property [Doctype]() As String
        Get
            Return m_Doctype
        End Get
        Set(value As String)
            m_Doctype = value
        End Set
    End Property
    Private m_Doctype As DateTime

End Class

Public Class DDL

    Public Property valueField() As Integer
        Get
            Return m_valueField
        End Get
        Set(value As Integer)
            m_valueField = value
        End Set
    End Property
    Private m_valueField As Integer

    Public Property TextField() As String
        Get
            Return m_TextField
        End Get
        Set(value As String)
            m_TextField = value
        End Set
    End Property
    Private m_TextField As String

End Class