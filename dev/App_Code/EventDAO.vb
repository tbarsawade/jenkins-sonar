Imports System.Collections.Generic
Imports System.Linq
Imports System.Web
Imports System.Data
Imports System.Data.SqlClient


Public Class EventDAO
    'change the connection string as per your database connection.


    Private Shared connectionString As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString

    'this method retrieves all events within range start-end
    Public Shared Function getEvents(DocType As String, Eid As Integer, Uid As Integer, role As String, Flag As Integer) As List(Of CalendarEvent)

        Dim events As New List(Of CalendarEvent)()
        Dim con As New SqlConnection(connectionString)
        Dim cmd As New SqlCommand("exec UspGetCalendardata1 @DocType,@Eid,@Uid,@role,@Flag", con)
        'cmd.Parameters.AddWithValue("@DocType", "VRF FIXED_POOL")
        'cmd.Parameters.AddWithValue("@Eid", 32)
        'cmd.Parameters.AddWithValue("@Uid", 696)
        'cmd.Parameters.AddWithValue("@role", "Admin")
        'cmd.Parameters.AddWithValue("@Flag", 0)
        cmd.Parameters.AddWithValue("@DocType", DocType)
        cmd.Parameters.AddWithValue("@Eid", Eid)
        cmd.Parameters.AddWithValue("@Uid", Uid)
        cmd.Parameters.AddWithValue("@role", role)
        cmd.Parameters.AddWithValue("@Flag", Flag)

        Dim dt As New DataTable()
        Dim da As New SqlDataAdapter(cmd)
        da.Fill(dt)

        For Each dr As DataRow In dt.Rows
            Dim cevent As New CalendarEvent()
            cevent.id = CInt(dr.Item("ID"))
            cevent.title = DirectCast(dr.Item("title"), String)
            cevent.description = DirectCast(dr.Item("description"), String)
            cevent.start = DirectCast(dr.Item("startdate"), DateTime)
            cevent.[end] = DirectCast(dr.Item("enddate"), DateTime)
            events.Add(cevent)
        Next

        'Using con
        '    con.Open()
        '    Dim reader As SqlDataReader = cmd.ExecuteReader()
        '    While reader.Read()
        '        Dim cevent As New CalendarEvent()
        '        cevent.id = CInt(reader("ID"))
        '        cevent.title = DirectCast(reader("title"), String)
        '        cevent.description = DirectCast(reader("description"), String)
        '        cevent.start = DirectCast(reader("start"), DateTime)
        '        cevent.[end] = DirectCast(reader("end"), DateTime)
        '        events.Add(cevent)
        '    End While
        'End Using
        Return events

    End Function

    'this method updates the event title and description
    Public Shared Sub updateEvent(id As Integer, title As [String], description As [String])
        Dim con As New SqlConnection(connectionString)
        Dim cmd As New SqlCommand("UPDATE event SET title=@title, description=@description WHERE event_id=@event_id", con)
        cmd.Parameters.AddWithValue("@title", title)
        cmd.Parameters.AddWithValue("@description", description)
        cmd.Parameters.AddWithValue("@event_id", id)
        Using con
            con.Open()
            cmd.ExecuteNonQuery()
        End Using


    End Sub

    'this method updates the event start and end time
    Public Shared Sub updateEventTime(id As Integer, start As DateTime, [end] As DateTime)
        Dim con As New SqlConnection(connectionString)
        Dim cmd As New SqlCommand("UPDATE event SET event_start=@event_start, event_end=@event_end WHERE event_id=@event_id", con)
        cmd.Parameters.AddWithValue("@event_start", start)
        cmd.Parameters.AddWithValue("@event_end", [end])
        cmd.Parameters.AddWithValue("@event_id", id)
        Using con
            con.Open()
            cmd.ExecuteNonQuery()
        End Using
    End Sub

    'this mehtod deletes event with the id passed in.
    Public Shared Sub deleteEvent(id As Integer)
        Dim con As New SqlConnection(connectionString)
        Dim cmd As New SqlCommand("DELETE FROM event WHERE (event_id = @event_id)", con)
        cmd.Parameters.AddWithValue("@event_id", id)
        Using con
            con.Open()
            cmd.ExecuteNonQuery()
        End Using
    End Sub

    'this method adds events to the database
    Public Shared Function addEvent(cevent As CalendarEvent) As Integer
        'add event to the database and return the primary key of the added event row

        'insert
        Dim con As New SqlConnection(connectionString)
        Dim cmd As New SqlCommand("INSERT INTO event(title, description, event_start, event_end) VALUES(@title, @description, @event_start, @event_end)", con)
        cmd.Parameters.AddWithValue("@title", cevent.title)
        cmd.Parameters.AddWithValue("@description", cevent.description)
        cmd.Parameters.AddWithValue("@event_start", cevent.start)
        cmd.Parameters.AddWithValue("@event_end", cevent.[end])

        Dim key As Integer = 0
        Using con
            con.Open()
            cmd.ExecuteNonQuery()

            'get primary key of inserted row
            cmd = New SqlCommand("SELECT max(event_id) FROM event where title=@title AND description=@description AND event_start=@event_start AND event_end=@event_end", con)
            cmd.Parameters.AddWithValue("@title", cevent.title)
            cmd.Parameters.AddWithValue("@description", cevent.description)
            cmd.Parameters.AddWithValue("@event_start", cevent.start)
            cmd.Parameters.AddWithValue("@event_end", cevent.[end])

            key = CInt(cmd.ExecuteScalar())
        End Using

        Return key

    End Function

    Public Shared Function GetFormNames(EId As Integer) As List(Of DDL)
        Dim forms As New List(Of DDL)()
        Dim con As New SqlConnection(connectionString)
        Dim cmd As New SqlCommand("Select FormName, FormId from MMM_MST_FORMS where EId=" + EId.ToString() + " order by FormName", con)
        Dim dt As New DataTable()
        Dim da As New SqlDataAdapter(cmd)
        da.Fill(dt)

        For Each dr As DataRow In dt.Rows
            Dim form As New DDL()
            form.valueField = CInt(dr.Item("FormId"))
            form.TextField = DirectCast(dr.Item("FormName"), String)
            forms.Add(form)
        Next

        Return forms
    End Function

End Class