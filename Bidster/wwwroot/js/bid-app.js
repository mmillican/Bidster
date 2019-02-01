var bidApp = new Vue({
    el: '#bid-app',
    data: {
        event: {
            id: 0,
            hideBidderNames: false,
            isUserAdmin: false,
            isBiddingOpen: false
        },
        productId: 0,
        currentBidAmount: 0,
        nextBidAmount: null,
        bids: [],
        nextBidMinAmount: null
    },
    created: function () {
        this.event.id = window.eventData.id;
        this.event.hideBidderNames = window.eventData.hideBidderNames;
        this.event.isUserAdmin = window.eventData.isUserAdmin;
        this.event.isBiddingOpen = window.eventData.isBiddingOpen;

        this.productId = window.bids.productId;
        this.currentBidAmount = window.bids.currentBidAmount;
        //this.newBidAmount = window.bids.newBidAmount;
        this.nextBidMinAmount = window.bids.nextBidMinAmount;

        this.refreshBids();
    },
    methods: {
        refreshBids: function () {
            var self = this;
            var xhr = new XMLHttpRequest();

            xhr.open('GET', '/bids?productId=' + this.productId);
            xhr.setRequestHeader('Content-Type', 'application/json');

            xhr.onload = function () {
                if (xhr.readyState === xhr.DONE && xhr.status === 200) {
                    self.bids = JSON.parse(xhr.response);
                }
            };
            xhr.send();
        },
        submitBid: function () {
            console.log('submit bid', this.nextBidAmount);

            var self = this;
            var xhr = new XMLHttpRequest();
            xhr.open('POST', '/bids');
            xhr.setRequestHeader('Content-Type', 'application/json');

            xhr.onload = function () {
                if (xhr.readyState === xhr.DONE && xhr.status === 201) {
                    self.refreshBids();
                }
            };

            var data = {
                productId: this.productId,
                amount: this.nextBidAmount
            };
            xhr.send(JSON.stringify(data));
        }
    },
    filters: {
        currency: function (value) {
            return kendo.toString(value, 'c2');
        },
        date: function (value) {
            return kendo.toString(new Date(value), 'g');
        }
    }
})