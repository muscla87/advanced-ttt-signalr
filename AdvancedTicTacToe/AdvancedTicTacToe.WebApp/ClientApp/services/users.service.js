
(function () {
    'use strict';

    /**
     * @ngdoc service
     * @name advancedTicTacToe.service:usersService
     * @description Provides access to users related information
     */
    angular
      .module('advancedTicTacToe')
      .factory('usersService', usersService);

    usersService.$inject = ['$q', 'Hub'];

    function usersService($q, Hub) {
        var service = {
            connect: connect,
            getConnectedUsers: getConnectedUsers,
            userConnected: null, //callback function
            userDisconnected: null //callback function
        };


        var connectionDeferred = $q.defer();

        //declaring the hub connection
        var hub = new Hub('usersHub', {

            useSharedConnection: false,

            //client side methods
            listeners: {
                'usersListUpdated': onlineUsersListChanged
            },

            //server side methods
            methods: ['getConnectedUsers'],


            //handle connection error
            errorHandler: function (error) {
                console.error(error);
            },

            //specify a non default root
            rootPath: AppName + "signalr",

            stateChanged: function (state) {
                switch (state.newState) {
                    case $.signalR.connectionState.connecting:
                        //your code here
                        console.log('usersHub connecting');
                        break;
                    case $.signalR.connectionState.connected:
                        //your code here
                        connectionDeferred.resolve();
                        console.log('usersHub connected');
                        break;
                    case $.signalR.connectionState.reconnecting:
                        //your code here
                        console.log('usersHub reconnecting');
                        break;
                    case $.signalR.connectionState.disconnected:
                        //your code here
                        console.log('usersHub disconnected');
                        break;
                }
            }
        });

        function connect() {
            return connectionDeferred.promise;
        }

        function getConnectedUsers() {

            var defer = $q.defer();
            hub.getConnectedUsers().done(function (usersList) { defer.resolve(usersList); });
            //TODO: handle errors
            return defer.promise;
        }

        function onlineUsersListChanged(res) {

            if (res.connectedUserName && service.userConnected) {
                service.userConnected(res);
            }

            if (res.disconnectedUserName && service.userDisconnected) {
                service.userDisconnected(res);
            }
        }


        return service;
    }

    ////////////////
})();