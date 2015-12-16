
(function () {
    'use strict';

    /**
     * @ngdoc service
     * @name advancedTicTacToe.service:gameRoomService
     * @description Provides access to the game room functionalities
     */
    angular
      .module('advancedTicTacToe')
      .factory('gameRoomService', gameRoomService);

    gameRoomService.$inject = ['$q', 'Hub'];

    function gameRoomService($q, Hub) {
        var service = {
            connect: connect,
            joinFreeRoomOrCreate: joinFreeRoomOrCreate,
            clearRooms: clearRooms,
            getRoomState: getRoomState,
            roomStateChanged: null, //callback function
        };


        var connectionDeferred = $q.defer();

        //declaring the hub connection
        var hub = new Hub('gameRoomsHub', {

            //client side methods
            listeners: {
                'roomStateChanged': roomStateChanged
            },

            //server side methods
            methods: ['joinFreeRoomOrCreate',
                      'getRoomState',
                      'clearRooms'],


            //handle connection error
            errorHandler: function (error) {
                console.error(error);
            },

            //specify a non default root
            //rootPath: '/api

            stateChanged: function (state) {
                switch (state.newState) {
                    case $.signalR.connectionState.connecting:
                        //your code here
                        console.log('gameRoomsHub connecting');
                        break;
                    case $.signalR.connectionState.connected:
                        //your code here
                        connectionDeferred.resolve();
                        console.log('gameRoomsHub connected');
                        break;
                    case $.signalR.connectionState.reconnecting:
                        //your code here
                        console.log('gameRoomsHub reconnecting');
                        break;
                    case $.signalR.connectionState.disconnected:
                        //your code here
                        console.log('gameRoomsHub disconnected');
                        break;
                }
            }
        });

        function connect() {
            return connectionDeferred.promise;
        }

        function joinFreeRoomOrCreate() {

            var defer = $q.defer();
            hub.joinFreeRoomOrCreate().done(function (gameRoomState) { defer.resolve(gameRoomState); });
            //TODO: handle errors
            return defer.promise;
        }

        function clearRooms() {
            var defer = $q.defer();
            hub.clearRooms().done(function () { defer.resolve(true); });
            //TODO: handle errors
            return defer.promise;
        }

        function getRoomState(roomId) {
            var defer = $q.defer();
            hub.getRoomState(roomId).done(function (roomState) { defer.resolve(roomState); });
            //TODO: handle errors
            return defer.promise;
        }


        function roomStateChanged(res) {
            if (service.roomStateChanged) {
                service.roomStateChanged(res);
            }
        }


        return service;
    }

    ////////////////
})();