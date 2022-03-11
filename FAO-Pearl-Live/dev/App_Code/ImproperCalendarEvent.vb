Imports System.Collections.Generic
Imports System.Linq
Imports System.Web

'Do not use this object, it is used just as a go between between javascript and asp.net
Public Class ImproperCalendarEvent

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
    Public Property start() As String
        Get
            Return m_start
        End Get
        Set(value As String)
            m_start = Value
        End Set
    End Property
    Private m_start As String
    Public Property [end]() As String
        Get
            Return m_end
        End Get
        Set(value As String)
            m_end = Value
        End Set
    End Property
    Private m_end As String

End Class