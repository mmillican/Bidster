// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

var notificationsApp = new Vue({
    el: '#cms-notices',
    data: {
        notifications: []
    },
    methods: {
        addNotification: function (type, text) {
            var notif = {
                type: 'alert-' + type,
                text: text
            };
            this.notifications.push(notif);
        }
    }
});