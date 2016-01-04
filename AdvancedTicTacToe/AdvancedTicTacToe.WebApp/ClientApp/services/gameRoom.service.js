
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
            setPlayerIcon: setPlayerIcon,
            setPlayerColor: setPlayerColor,
            setGameTurnTimer: setGameTurnTimer,
            setGameUsePowerUps: setGameUsePowerUps,
            setPlayerReady: setPlayerReady,
            applyMove: applyMove,
            leaveRoom: leaveRoom,
            resetRoomForNewGame: resetRoomForNewGame,
            roomStateChanged: null, //callback function
        };

        var connectionDeferred = $q.defer();

        //declaring the hub connection
        var hub = new Hub('gameRoomsHub', {

            useSharedConnection: false,

            //client side methods
            listeners: {
                'roomStateChanged': roomStateChanged
            },

            //server side methods
            methods: ['joinFreeRoomOrCreate',
                      'getRoomState',
                      'setPlayerIcon',
                      'setPlayerColor',
                      'setPlayerReady',
                      'setGameTurnTimer',
                      'setGameUsePowerUps',
                      'applyMove',
                      'leaveRoom',
                      'resetRoomForNewGame',
                      'clearRooms'],


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

        function setPlayerIcon(roomId, icon) {
            var defer = $q.defer();
            hub.setPlayerIcon(roomId, icon).done(function () { defer.resolve(); });
            //TODO: handle errors
            return defer.promise;
        }

        function setPlayerColor(roomId, color) {
            var defer = $q.defer();
            hub.setPlayerColor(roomId, color).done(function () { defer.resolve(); });
            //TODO: handle errors
            return defer.promise;
        }

        function setGameTurnTimer(roomId, timer) {
            var defer = $q.defer();
            hub.setGameTurnTimer(roomId, timer).done(function () { defer.resolve(); });
            //TODO: handle errors
            return defer.promise;
        }

        function setGameUsePowerUps(roomId, usePowerUps) {
            var defer = $q.defer();
            hub.setGameUsePowerUps(roomId, usePowerUps).done(function () { defer.resolve(); });
            //TODO: handle errors
            return defer.promise;
        }

        function setPlayerReady(roomId) {
            var defer = $q.defer();
            hub.setPlayerReady(roomId).done(function () { defer.resolve(); });
            //TODO: handle errors
            return defer.promise;
        }

        function applyMove(roomId, move) {
            var defer = $q.defer();
            hub.applyMove(roomId, move).done(function (arg) {
                defer.resolve();
            });
            //TODO: handle errors
            return defer.promise;
        }

        function leaveRoom(roomId) {
            var defer = $q.defer();
            hub.leaveRoom(roomId).done(function () { defer.resolve(); });
            //TODO: handle errors
            return defer.promise;
        }

        function resetRoomForNewGame(roomId) {
            var defer = $q.defer();
            hub.resetRoomForNewGame(roomId).done(function () { defer.resolve(); });
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