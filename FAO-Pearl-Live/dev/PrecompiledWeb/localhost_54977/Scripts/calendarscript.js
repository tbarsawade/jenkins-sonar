var currentUpdateEvent;
var addStartDate;
var addEndDate;
var globalAllDay;

function updateEvent(event, element) {

    //PopUp1 = window.open('DocDetail.aspx?DOCID=' + event.id, "popupWindow", "width=700,height=600,location=center,scrollbars=yes");
    //PopUp1.focus();
    //return false;
    
    window.open('DocDetail.aspx?DOCID=' + event.id+'&IsCal=1', "List", "scrollbars=yes,resizable=yes,width=800,height=480");
    return false;

    if ($(this).data("qtip")) $(this).qtip("destroy");

    currentUpdateEvent = event;
    //Prashant
   // $('#updatedialog').dialog('open');

    $("#eventName").val(event.title);
    $("#eventDesc").val(event.description);
    $("#eventId").val(event.id);
    $("#eventStart").text("" + event.start.toLocaleString());

    if (event.end === null) {
        $("#eventEnd").text("");
    }
    else {
        $("#eventEnd").text("" + event.end.toLocaleString());
    }

}

function updateSuccess(updateResult) {
    //alert(updateResult);
}

function deleteSuccess(deleteResult) {
    //alert(deleteResult);
}

function addSuccess(addResult) {
// if addresult is -1, means event was not added
//    alert("added key: " + addResult);

    if (addResult != -1) {
        $('#calendar').fullCalendar('renderEvent',
						{
						    title: $("#addEventName").val(),
						    start: addStartDate,
						    end: addEndDate,
						    id: addResult,
						    description: $("#addEventDesc").val(),
						    allDay: globalAllDay
						},
						true // make the event "stick"
					);


        $('#calendar').fullCalendar('unselect');
    }

}

function UpdateTimeSuccess(updateResult) {
    //alert(updateResult);
}


function selectDate(start, end, allDay) {
    //Prashant
    //$('#addDialog').dialog('open');


    $("#addEventStartDate").text("" + start.toLocaleString());
    $("#addEventEndDate").text("" + end.toLocaleString());


    addStartDate = start;
    addEndDate = end;
    globalAllDay = allDay;
    //alert(allDay);

}

function updateEventOnDropResize(event, allDay) {

    //alert("allday: " + allDay);
    var eventToUpdate = {
        id: event.id,
        start: event.start

    };

    if (allDay) {
        eventToUpdate.start.setHours(0, 0, 0);

    }

    if (event.end === null) {
        eventToUpdate.end = eventToUpdate.start;

    }
    else {
        eventToUpdate.end = event.end;
        if (allDay) {
            eventToUpdate.end.setHours(0, 0, 0);
        }
    }

    eventToUpdate.start = eventToUpdate.start.format("dd-MM-yyyy hh:mm:ss tt");
    eventToUpdate.end = eventToUpdate.end.format("dd-MM-yyyy hh:mm:ss tt");

    PageMethods.UpdateEventTime(eventToUpdate, UpdateTimeSuccess);

}

function eventDropped(event, dayDelta, minuteDelta, allDay, revertFunc) {

    if ($(this).data("qtip")) $(this).qtip("destroy");

    updateEventOnDropResize(event, allDay);



}

function eventResized(event, dayDelta, minuteDelta, revertFunc) {
    //Prashant
   // if ($(this).data("qtip")) $(this).qtip("destroy");
   // updateEventOnDropResize(event);

}

function checkForSpecialChars(stringToCheck) {
    var pattern = /[^A-Za-z0-9 ]/;
    return pattern.test(stringToCheck); 
}

$(document).ready(function() {

    // update Dialog
    $('#updatedialog').dialog({
        autoOpen: false,
        width: 470,
        buttons: {
            "update": function() {
                //alert(currentUpdateEvent.title);
                var eventToUpdate = {
                    id: currentUpdateEvent.id,
                    title: $("#eventName").val(),
                    description: $("#eventDesc").val()
                };

                if (checkForSpecialChars(eventToUpdate.title) || checkForSpecialChars(eventToUpdate.description)) {
                    alert("please enter characters: A to Z, a to z, 0 to 9, spaces");
                }
                else {
                  
                    PageMethods.UpdateEvent(eventToUpdate, updateSuccess);
                    $(this).dialog("close");

                    currentUpdateEvent.title = $("#eventName").val();
                    currentUpdateEvent.description = $("#eventDesc").val();
                    $('#calendar').fullCalendar('updateEvent', currentUpdateEvent);
                }

            },
            "delete": function() {

                if (confirm("do you really want to delete this event?")) {

                    PageMethods.deleteEvent($("#eventId").val(), deleteSuccess);
                    $(this).dialog("close");
                    $('#calendar').fullCalendar('removeEvents', $("#eventId").val());
                }

            }

        }
    });

    //add dialog
    $('#addDialog').dialog({
        autoOpen: false,
        width: 470,
        buttons: {
            "Add": function() {

                //alert("sent:" + addStartDate.format("dd-MM-yyyy hh:mm:ss tt") + "==" + addStartDate.toLocaleString());
                var eventToAdd = {
                    title: $("#addEventName").val(),
                    description: $("#addEventDesc").val(),
                    start: addStartDate.format("dd-MM-yyyy hh:mm:ss tt"),
                    end: addEndDate.format("dd-MM-yyyy hh:mm:ss tt")

                };

                if (checkForSpecialChars(eventToAdd.title) || checkForSpecialChars(eventToAdd.description)) {
                    alert("please enter characters: A to Z, a to z, 0 to 9, spaces");
                }
                else {
                    //alert("sending " + eventToAdd.title);

                    PageMethods.addEvent(eventToAdd, addSuccess);
                    $(this).dialog("close");
                }

            }

        }
    });




    var date = new Date();
    var d = date.getDate();
    var m = date.getMonth();
    var y = date.getFullYear();

    var sc = '';
    sc = GetParameterValues('SC');
    var calendar = $('#calendar').fullCalendar({
        theme: true,
        header: {
            left: 'prev,next today',
            center: 'title',
            right: 'month,agendaWeek,agendaDay'
        },
        eventClick: updateEvent,
        selectable: true,
        selectHelper: true,
        select: selectDate,
        editable: true,
        events: "JsonResponse.ashx?Doc=" + sc +"& Option=0",
        eventDrop: eventDropped,
        eventResize: eventResized,
        eventRender: function(event, element) {
            //alert(event.title);
            element.qtip({
                content: event.description,
                position: { corner: { tooltip: 'bottomLeft', target: 'topRight'} },
                style: {
                    border: {
                        width: 1,
                        radius: 3,
                        color: '#2779AA'
                    },
                    padding: 10,
                    textAlign: 'center',
                    tip: true, // Give it a speech bubble tip with automatic corner detection
                    name: 'cream' // Style it according to the preset 'cream' style
                }

            });
        }

    });


    //BindDdl('#ddlDoctype',true);

});


var Select = false;
var ddl = '';
function BindDdl(ddlId,SelectOption)
{
   
    Select = SelectOption;
    ddl = ddlId;
    $.ajax({
        url: "JsonResponse.ashx?Option=1",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: {},
        responseType: "json",
        success: OnComplete,
        error: function () { alert('Server Error.'); }
    });
    return false;

}

function OnComplete(result) {
    if (Select)
    {
        $(ddl).append($("<option></option>").val('0').html('Select'));
    }
    $.each(result, function () {
        $(ddl).append($("<option></option>").val(this['valueField']).html(this['TextField']));
    });
}


function BinCalendar(sender)
{
    $('#lblcalmsg').html('Loading Calendar...');
    $('#calendar').html('');
    var date = new Date();
    var d = date.getDate();
    var m = date.getMonth();
    var y = date.getFullYear();
    
    var ddlval=sender

    var calendar = $('#calendar').fullCalendar({
        theme: true,
        header: {
            left: 'prev,next today',
            center: 'title',
            right: 'month,agendaWeek,agendaDay'
        },
        eventClick: updateEvent,
        selectable: true,
        selectHelper: true,
        select: selectDate,
        editable: true,
        events: "JsonResponse.ashx?Doc=" + $('#ddlDoctype option:selected').text() + "& Option=0",
        eventDrop: eventDropped,
        eventResize: eventResized,
        eventRender: function (event, element) {
            //alert(event.title);
            element.qtip({
                content: event.description,
                position: { corner: { tooltip: 'bottomLeft', target: 'topRight' } },
                style: {
                    border: {
                        width: 1,
                        radius: 3,
                        color: '#2779AA'
                    },
                    padding: 10,
                    textAlign: 'center',
                    tip: true, // Give it a speech bubble tip with automatic corner detection
                    name: 'cream' // Style it according to the preset 'cream' style
                }
                 
            });
            
        }

    });
    $('#lblcalmsg').html('');
}



function GetParameterValues(param) {
    var url = window.location.href.slice(window.location.href.indexOf('?') + 1).split('&');
    for (var i = 0; i < url.length; i++) {
        var urlparam = url[i].split('=');
        if (urlparam[0] == param) {
            return urlparam[1];
        }
    }
}

