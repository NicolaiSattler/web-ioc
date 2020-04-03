var TestHub = $.connection['testHub'];

TestHub.Activated = function () {

    console.log("Activated");
}
// Start the connection.
$.connection.hub.start().done(function () {

    $.connection['testHub'].server.activate();

});