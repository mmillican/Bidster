'use strict';

var connection = new signalR.HubConnectionBuilder().withUrl('/bidster').build();

connection.on('SendBidNotification', function (evt, product) {
    console.log('Bid received for ', product.name);
});

connection.on('BidPlaced', function (productName, amount) {
    notificationsApp.addNotification('info', 'A ' + kendo.toString(amount, 'c2') + ' bid was placed on <b>' + productName + '</b>');
});

connection.start().catch(function (err) {
    return console.error(err.toString());
});