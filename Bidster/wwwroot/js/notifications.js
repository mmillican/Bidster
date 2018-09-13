'use strict';

var connection = new signalR.HubConnectionBuilder().withUrl('/bidNotificationHub').build();

connection.on('SendBidNotification', function (evt, product) {
    console.log('Bid received for ', product.name);
});

connection.start().catch(function (err) {
    return console.error(err.toString());
});