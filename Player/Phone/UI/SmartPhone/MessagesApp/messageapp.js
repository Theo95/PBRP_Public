function populateMessages(toFrom, messDate, viewed, convoMess, count) {
    var from = toFrom.split("|");
    var date = messDate.split(",");
    var view = viewed.split(",");
    var message = convoMess.split("|");

    $('#smartscreen').empty();
    $('#smartscreen').load('MessagesApp/smartMessagesHome.html');

    setTimeout(function () {
        
        if (count === 0) {
            $('#messages-list').append('<tr class="recentcall-row"><td>No Messages</td></tr>');
        }
        for (var i = 0; i < count; i ++) {
            $('#messages-list').append('<tr id="message-' + i + '">' +
                '<td style="padding:0; line-height:1vw;"><span class="rc-contact" id="message-contact' + i + '">' + from[i] + '</span ><span class="rc-date" id="message-date' + i + '">' + date[i] + '</span></td></tr>' +
                '<tr id="message2-' + i + '" class="recentcall-row"><td colspan="2" class="messagelist-message" id="message-message' + i + '">' + message[i] + '</td></tr>');

            $("#message-" + i).mousedown(function (e) {
                var id = $(this).attr('id').replace("message-", "");
                if (e.which === 1) {
                    resourceCall('messageSelect', id);
                }
                else if (e.which === 2) {

                }
            });
            $("#message2-" + i).mousedown(function (e) {
                var id = $(this).attr('id').replace("message2-", "");
                if (e.which === 1) {
                    resourceCall('messageSelect', id);
                }
                else if (e.which === 2) {

                }
            });
        }
    }, 300);
}

function populateConversation(name, number, ids, toFrom, messDate, messageData, count)
{
    var date = messDate.split(",");
    var message = messageData.split("|");
    var toFro = toFrom.split(",");
    var id = ids.split(",");

    $('#smartscreen').empty();
    $('#smartscreen').load('MessagesApp/smartMessagesConvo.html');


    setTimeout(function () {
        $('#messageconvo-name').text(name);
        $('#messageconvo-number').text(number);
        for (var i = 0; i < count; i++) {
            if (toFro[i] == "outbound") {
                $('#messageconvo-list').append('<div class="row"><i id="messageconvo-id'+i+'" class="hidden">' + id[i] + '</i><div class="col-md-2 text-center"><span id="messageconvo-delete' + i + '" onclick="resourceCall("phoneMessageDelete",' + number + ',' + i +
                    ')" class="messageconvo-delete glyphicon glyphicon-remove-sign"></span></div><div class="col-md-10"><div data-toggle="tooltip" data-placement="bottom" title="' + date[i] +
                    '" id="messageconvo-mess' + i + '" class="messageconvo-tobubble"><span>' + message[i] +
                    '</span></div></div></div>');
            }
            else {
                $('#messageconvo-list').append('<div class="row"><i id="messageconvo-id' + i +'" class="hidden">' + id[i] + '</i><div class="col-md-10"><div data-toggle="tooltip" data-placement="bottom" title="' + date[i] +
                    '" id="messageconvo-mess' + i + '" class="messageconvo-frombubble"><span>' + message[i] +
                    '</span></div></div><div class="col-md-2 text-center"><span id="messageconvo-delete' + i + '" onclick="resourceCall("phoneMessageDelete",' + number + ',' + i +')" class="messageconvo-delete glyphicon glyphicon-remove-sign"></span></div></div>');
            }
            $('#messageconvo-delete' + i).hide();
            $('#messageconvo-delete' + i).css('line-height', $('#messageconvo-mess' + i).height() + "px");

            $("#messageconvo-mess" + i).mousedown(function (e) {
                var id = $(this).attr('id').replace("messageconvo-mess", "");
                if (e.which === 3) {
                    $("#messageconvo-delete" + id).fadeIn("fast", function () {
                        setTimeout(function () {
                            $("#messageconvo-delete" + id).fadeOut("slow");
                        }, 2000);
                    });
                }
            });
            $('#messageconvo-delete' + i).mousedown(function (e) {
                var id = $(this).attr('id').replace("messageconvo-delete", "");

                if (e.which === 1)
                {
                    resourceCall("phoneMessageDelete", $('#messageconvo-id'+ id).text());
                }
            });
        }

        $('[data-toggle="tooltip"]').tooltip();
        $('#smartscreen').scrollTop(1000000);
    }, 300);
}

//function addNewMessage(toFrom, messDate, messageData)
//{
//    if (toFrom == "outbound") {
//        $('#messageconvo-list').append('<div class="row"><div class="col-md-2"></div><div class="col-md-10"><div data-toggle="tooltip" data-placement="bottom" title="' + date[i] +
//            '" id="messageconvo- mess' + i + '" class="messageconvo-tobubble"><span>' + message[i] +
//            '</span></div></div></div>');
//    }
//    else {
//        $('#messageconvo-list').append('<div class="row"><div class="col-md-10"><div data-toggle="tooltip" data-placement="bottom" title="' + date[i] +
//            '" id= "messageconvo-mess' + i + '" class="messageconvo-frombubble" > <span>' + message[i] +
//            '</div><div class="col-md-2"></div></div></div>');
//    }

//    $("#messageconvo-mess" + i).mousedown(function (e) {
//        var id = $(this).attr('id').replace("message-", "");
//        if (e.which === 2) {
//            alert("delete?");
//        }
//    });
//}

function sendNewMessage() {
    if ($('#messageconvo-messagebox').val().length > 3)
    {
        if ($('#messagecompose-contactnumber').text().length > 0)
            resourceCall('phoneSendTextMessage', $('#messageconvo-messagebox').val().replace("|", ""), $('#messagecompose-contactnumber').text());
        else
            resourceCall('phoneSendTextMessage', $('#messageconvo-messagebox').val().replace("|", ""), $('#messageconvo-number').text());
    }
}

function textMessageResult(res) {
    if (res == 0) {
        $('#messageconvo-send').css('border-color', '#f00');
    }
    else {
        $('#messageconvo-messagebox').val("");
    }
}

function ComposeNewMessage() {

    $('#smartscreen').empty();
    $('#smartscreen').load('MessagesApp/smartMessagesCompose.html');

    resourceCall("phoneMessageComposeContacts");    
}

function populateMessageComposeContact(cNames, cNumbers)
{
    var names = cNames.split("|");
    var numbers = cNumbers.split(",");
    var nameNums = "";

    for (var i = 0; i < names.length; i++)
    {
        nameNums += names[i] + " - " + numbers[i] + "|";
    }

    var cumpile = nameNums.split("|");
    $('#messagecompose-contactheader').hide();
    setTimeout(function () {
        $("#messagecompose-contact").autocomplete({
            source: cumpile,
            select: function (event, ui) {
                onContactSelected(ui.item.value);
            }
        });
    }, 100);
}

function showContactSearch()
{
    $('#messagecompose-contactheader').hide();
    $('#messagecompose-contactname').empty();
    $('#messagecompose-contact').val("");
    $('#messagecompose-contactnumber').empty();
    $('#messagecompose-contactsearch').show();
}

function onContactSelected(selected) {
    if (selected == "")
    {
        if ($.isNumeric($('#messagecompose-contact').val())) {
            selected = $('#messagecompose-contact').val() + " - " + $('#messagecompose-contact').val();
        }
        else
        {
            $('#messageconvo-contact').css('border-color', '#f00');
        }
    }
        
    $('#messagecompose-contactname').text($.trim(selected.substr(0, selected.indexOf('-') - 1)));
    $('#messagecompose-contactnumber').text($.trim(selected.substr(selected.indexOf('-') + 1), selected.length));
    $('#messagecompose-contactsearch').hide();
    $('#messagecompose-contactheader').show();

    resourceCall("phoneMessageActiveConversation", $.trim(selected.substr(selected.indexOf('-') + 1), selected.length));

    $('#messageconvo-messagebox').removeAttr("disabled");
}

