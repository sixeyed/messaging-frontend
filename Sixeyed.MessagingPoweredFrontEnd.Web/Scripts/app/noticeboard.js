$(function () {

    var noticeboard = $.connection.noticeboard;
    var flipStyle = false;

    function init() {

        var userId = $('#Sender').val()
        noticeboard.server.registerUser(userId);

        noticeboard.server.getMail().done(function (mailBag) {
            var mailBox = $('#mailBox');
            mailBox.empty();
            $.each(mailBag, function () {                
                mailBox.append(format(this));
            });
        });
    }

    function format(mail) {
        var style = flipStyle ? 'warning' : 'success';
        flipStyle = !flipStyle;
        return '<div class="panel panel-' + style + '"><div class="panel-body">' + mail.Content + '</div><div class="panel-footer">From: <span class="text-' + style + '"><b>@' + mail.Sender + '</b></span>, at: ' + mail.SentAt + '</div></div>';
    }

    noticeboard.client.newMail = function (mail) {
        var mailBox = $('#mailBox');
        mailBox.prepend(format(mail));
    }

    noticeboard.client.showResponse = function (response) {
        var type = response.Event == 'broadcast' ? 'success' : 'info';
        toastr[type]('Your mail from ' + response.SentAt + ' was ' + response.Event +
                            ' at ' + response.EventAt);
    }

    $.connection.hub.start().done(init);

});

function send() {
    var noticeboard = $.connection.noticeboard;
    noticeboard.server.send($('#Sender').val(), $('#Content').val());
    $('#Content').val('').focus();
}