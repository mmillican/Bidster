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
        buyItNowPrice: null,
        purchasedDate: null,
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
        this.buyItNowPrice = window.bids.buyItNowPrice;
        this.purchasedDate = window.bids.purchasedDate;

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
        },
        buyNow: function () {
            if (!confirm('Are you sure you want to purchase this product?')) {
                return;
            }

            var self = this;
            var xhr = new XMLHttpRequest();
            xhr.open('POST', '/bids/buy-now');
            xhr.setRequestHeader('Content-Type', 'application/json');

            xhr.onload = function () {
                if (xhr.readyState === xhr.DONE && xhr.status === 201) {
                    self.refreshBids();
                    self.purchasedDate = new Date();
                }
            };

            var data = {
                productId: this.productId
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