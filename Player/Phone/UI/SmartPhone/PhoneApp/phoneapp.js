//PhoneApp Scripts
var dialledNumber = "";
var callTimer = null;
var callLength = 0;
var favShown = false;
function PhoneDialNumber(num) {
    if (dialledNumber.length < 11) {
        if (dialledNumber.length === 3) {
            $('#phone-dial-display-text').append("-");
        }
        $('#phone-dial-display-text').append(num);
    }
    dialledNumber += num;
}

function PhoneDeleteNumber() {
    if (dialledNumber.length > 0) {
        dialledNumber = dialledNumber.slice(0, -1);
        var text = dialledNumber;
        if (text.length > 3) {
            if ($('#phone-dial-display-text').text().includes("-"))
                text = $('#phone-dial-display-text').text().substr(0, $('#phone-dial-display-text').text().length - 1);
            else
                text = $('#phone-dial-display-text').text().substr(0, 3); + "-" + $('#phone-dial-display-text').text().substr(3, $('#phone-dial-display-text').text().length - 1);

        }
        $('#phone-dial-display-text').text(text);
    }
}

function PhoneCallUI() {
    if ($('#phonecall-main').length) {
        $('#phonecall-main').show();
    }
    else {
        $.get("PhoneApp/smartPhoneCall.html", function (data) {
            $("#smartscreen").append(data);
        });
    }

    $('#smart-inset').hide();
    $('#phone-bottombar').hide();
}

function MakePhoneCall() {
    resourceCall('placePhoneCall', dialledNumber);
    PhoneCallUI();
    setTimeout(function () {
        $('#phonecall-number').text($('#phone-dial-display-text').text());
    }, 100);
}

function RedialPhoneCall(number) {
    resourceCall('placePhoneCall', number);
    PhoneCallUI();
    setTimeout(function () {
        $('#phonecall-number').text(number.substr(0, 3) + "-" + number.substr(3, number.length));
    }, 100);
}

function EndCall() {
    resourceCall('endPhoneCall', callLength);

    clearInterval(callTimer);
    callLength = 0;
    $('#phonecall-status').text("call ended");
    $('#phonecall-number').empty();
    setTimeout(function () {
        $('#phonecall-main').hide();
        $('#phonecall-mute').show();
        $('#phonecall-speaker').show();
        $('#smart-inset').show();
        $('#phone-bottombar').show();
        if ($('#phone-recent').hasClass('active')) {
            ShowRecentCalls();
        }
    }, 1000);
}

function AnswerCall() {
    $('#phonecall-mute').fadeIn('slow');
    $('#phonecall-speaker').fadeIn('slow');
    $('#phonecall-answer').fadeOut('slow');
    resourceCall('answerPhoneCall');
    callLength = 0;
}

function phoneCallConnect(returnType) {
    var text = "";
    switch (returnType) {
        case 0:
            text = "invalid number";
            break;
        case 1:
        case 2:
        case 4:
            text = "disconnected";
            break;
        case 3:
            text = "connected";
            break;
        case 5:
            text = "busy";
            break;
    }
    $('#phonecall-status').text(text);

    setTimeout(function () {
        if (returnType !== 3) {
            $('#phonecall-number').empty();
            $('#phonecall-main').hide();
            $('#smart-inset').show();
            $('#phone-bottombar').show();
        }
        else {
            $('#phonecall-status').text("calling...");
        }
    }, 3000);
}

function incomingCall(number) {
    PhoneCallUI();
    setTimeout(function () {
        $('#phonecall-answer').removeClass('hidden');
        $('#phonecall-mute').hide();
        $('#phonecall-speaker').hide();
        $('#phonecall-number').text(number.substr(0, 3) + "-" + number.substr(3, number.length));
        $("#smartphone-main").removeClass("smartphone-collapsed");
    }, 300);
}

function terminateCall() {
    clearInterval(callTimer);
    $('#phonecall-status').text("call ended");
    $('#phonecall-number').empty();

    setTimeout(function () {
        $('#phonecall-main').hide();
        $('#phonecall-mute').show();
        $('#phonecall-speaker').show();
        $('#smart-inset').show();
        $('#phone-bottombar').show();
        if ($('#phone-recent').hasClass('active')) {
            ShowRecentCalls();
        }
    }, 1000);
}

function phoneCallAnswered() {
    callTimer = setInterval(function () {
        $('#phonecall-status').text(new Date(1000 * callLength).toISOString().substr(11, 8));
        callLength++;
    }, 1000);
}

function callAction(type) {
    if (type === 1)
        resourceCall('phoneMuteMicrophone');
    else
        resourceCall('phoneSpeakerphone');

    $('#phonecall-action' + type).toggleClass('active');
}

function ShowDialScreen() {
    $('#phone-recent').removeClass('active');
    $('#phone-fav').removeClass('active');
    $('#phone-dial').addClass('active');
    $('#phone-contact').removeClass('active');
    setTimeout(function () {
        $('#smart-inset').load('PhoneApp/smartPhoneDial.html');
    }, 250);
}

function ShowRecentCalls() {
    resourceCall('showRecentCalls');
}

function ShowContactList() {
    resourceCall('showContactList');
    $('#phone-recent').removeClass('active');
    $('#phone-fav').removeClass('active');
    $('#phone-dial').removeClass('active');
    $('#phone-contact').addClass('active');
}

function ShowFavouriteContacts() {
    resourceCall('showFavouriteList');
    $('#phone-recent').removeClass('active');
    $('#phone-fav').addClass('active');
    $('#phone-dial').removeClass('active');
    $('#phone-contact').removeClass('active');
}

function populateRecentCalls(callData, count) {
    var data = callData.split(",");

    $('#smart-inset').empty();
    $('#smart-inset').load('PhoneApp/smartPhoneRecent.html');

    setTimeout(function () {
        var pos = 0;

        if (count === 0) {
            $('#recentcall-list').append('<tr class="recentcall-row"><td>No Recent Calls</td></tr>');
        }
        for (var i = 0; i < count - 1; i += 4) {
            $('#recentcall-list').append('<tr id="recentcall-' + pos + '" class="recentcall-row">' +
                '<td style="padding:0; line-height:1.6vw"><span class="rc-contact" id="recentcall-contact' + pos + '">' + data[i] + '</span ></td > ' +
                '<td class="text-right" style="padding:0; line-height:1.6vw"><span class="rc-date" id="recentcall-date' + pos + '">' + data[i + 2] + '</span></td></tr>');

            $("#recentcall-" + pos).mousedown(function (e) {
                var id = $(this).attr('id').replace("recentcall-", "");
                if (e.which === 1) {
                    resourceCall('recentCallSelect', id);
                }
                else if (e.which === 2) {

                }
            });

            if (data[i + 1] === "inbound") {
                if (data[i + 3] === "no") {
                    $('#recentcall-contact' + pos).css('color', '#f00');
                }
                $('#recentcall-contact' + pos).prepend('<span style="font-size:0.5vw;" class="glyphicon glyphicon-earphone"></span><span style="font-size:0.4vw; margin-left:-0.15vw; margin-top:-0.2vw; padding-right:0.1vw;" class="glyphicon glyphicon-arrow-left" ></span>');
            }
            else {

                $('#recentcall-contact' + pos).prepend('<span style="font-size:0.5vw;" class="glyphicon glyphicon-earphone"></span><span style="font-size:0.4vw; margin-left:-0.15vw; margin-top:-0.25vw; padding-right:0.1vw;" class="glyphicon glyphicon-arrow-right"></span>');
            }

            pos++;
        }
    }, 200);

    $('#phone-recent').addClass('active');
    $('#phone-fav').removeClass('active');
    $('#phone-dial').removeClass('active');
    $('#phone-contact').removeClass('active');
}

function populateContactList(contactNames, isFav) {
    $('#smart-inset').empty();
    $('#smart-inset').load('PhoneApp/smartPhoneContacts.html');
    favShown = isFav;
    setTimeout(function () {
        var names = contactNames.split(",");
        if (names.length === 0) {
            $('#phonecontact-list').append("<tr class='phonecontact-row'><td>No Contacts</td></tr>");
        }
        for (var i = 0; i < names.length; i++) {
            $('#phonecontact-list').append("<tr class='phonecontact-row' onclick='ContactSelect(" + i + ")'>" +
                "<td style='padding-left: 0.2vw; line-height:1.6vw; font-size: 0.8vw'>" + names[i] + "</td></tr>");
        }
    }, 200);
}

function RecentCallSelect(i) {
    resourceCall('recentCallSelect', i);
}

function AddNewContact() {
    $('#smart-inset').empty();
    $('#smart-inset').load('PhoneApp/smartPhoneNewContact.html');
    resourceCall("addNewContact");
}

function ContactSelect(id) {
    resourceCall("phoneContactSelected", id, favShown);
}

function ShowContact(id, name, number, add1, add2, add3, notes, fav, block, store) {
    $('#smart-inset').empty();
    $('#smart-inset').load('PhoneApp/smartPhoneShowContact.html');

    setTimeout(function () {
        $('#contact-id').text(id);
        $('#showcontact-name').text(name);
        $('#showcontact-number').text(number.substr(0, 3) + "-" + number.substr(3, number.length));
        $('#showcontact-address-1').text(add1);
        $('#showcontact-address-2').text(add2);
        $('#showcontact-address-3').text(add3);
        $('#showcontact-notes').text(notes);

        if (fav) $('#showcontact-favourite').addClass('glyphicon-star');
        else $('#showcontact-favourite').addClass('glyphicon-star-empty');

        if (block) $('#showcontact-block').text("Unblock Contact");

        if (store) $('#showcontact-changestorage').text("Save To Phone");
    }, 200);
}

function SaveNewContact() {
    var name = $('#newcontact-name').val() + " ";
    var number = $('#newcontact-number').val() + " ";
    var address1 = $('#newcontact-address-1').val() + " ";
    var address2 = $('#newcontact-address-2').val() + " ";
    var address3 = $('#newcontact-address-3').val() + " ";
    var notes = $('#newcontact-notes').val() + " ";

    if (name.length < 3) $('#newcontact-name').css('border-color', '#f00'); else $('#newcontact-name').css('border-color', '#fff');
    if (name.length < 3) $('#newcontact-number').css('border-color', '#f00'); else $('#newcontact-number').css('border-color', '#fff');

    if (name.length > 2 && number.length > 2)
        resourceCall("createNewContact", name.replace("|", ""), number, address1, address2, address3, notes);
}

function EditContact() {
    var id = $('#contact-id').text();
    var name = $('#showcontact-name').text();
    var number = $('#showcontact-number').text();
    var address1 = $('#showcontact-address-1').text();
    var address2 = $('#showcontact-address-2').text();
    var address3 = $('#showcontact-address-3').text();
    var notes = $('#showcontact-notes').text();

    $('#smart-inset').empty();
    $('#smart-inset').load('PhoneApp/smartPhoneEditContact.html');

    setTimeout(function () {
        $('#contact-id').text(id);
        $('#newcontact-name').val(name);
        $('#newcontact-number').val(number.replace("-", ""));
        $('#newcontact-address-1').val(address1);
        $('#newcontact-address-2').val(address2);
        $('#newcontact-address-3').val(address3);
        $('#newcontact-notes').val(notes);
        resourceCall("addNewContact");
    }, 200);
}

function UpdateContact() {
    var id = $('#contact-id').text();
    var name = $('#newcontact-name').val() + " ";
    var number = $('#newcontact-number').val() + " ";
    var address1 = $('#newcontact-address-1').val() + " ";
    var address2 = $('#newcontact-address-2').val() + " ";
    var address3 = $('#newcontact-address-3').val() + " ";
    var notes = $('#newcontact-notes').val() + " ";

    if (name.length < 3) $('#newcontact-name').css('border-color', '#f00'); else $('#newcontact-name').css('border-color', '#fff');
    if (name.length < 3) $('#newcontact-number').css('border-color', '#f00'); else $('#newcontact-number').css('border-color', '#fff');

    if (name.length > 2 && number.length > 2)
        resourceCall("updateContact", id, name.replace("|", ""), number, address1, address2, address3, notes);
}

function MakeFavourite() {
    var id = $('#contact-id').text();

    resourceCall('setFavouriteContact', id);
}

function DeleteContact() {
    resourceCall("deletePhoneContact", $('#contact-id').text());
}

function BlockContact() {
    resourceCall("blockPhoneContact", $('#contact-id').text());
}

function ChangeContactStorage() {
    resourceCall("changeContactStorage", $('#contact-id').text());
}

function CallContact() {
    var number = $('#showcontact-number').text().replace("-", "");
    RedialPhoneCall(number);
}