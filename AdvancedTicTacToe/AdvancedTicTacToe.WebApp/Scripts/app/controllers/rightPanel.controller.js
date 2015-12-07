(function () {
    'use strict';

    /**
     * @ngdoc controller
     * @name advancedTicTacToe.controller:RightPanelController
     * @description Controller that manages the right panel section of the app which includes Online Users and Live Statistics
     */
    angular
      .module('advancedTicTacToe')
      .controller('RightPanelController', RightPanelController);

    RightPanelController.$inject = ['$scope', 'usersService', 'userName'/*, 'singleMatch'*/];

    function RightPanelController($scope, usersService, userName/*, singleMatch*/) {
        var vm = this;

        vm.onlineUsers = [];
        vm.playMatch = playMatch;


        activate();

        function activate() {
            usersService.connect().then(function () {
                usersService.getConnectedUsers().then(function (users) {
                    vm.onlineUsers = convertToUsers(users);
                });

                usersService.userConnected = function (eventData) {
                    $scope.$apply(function () {
                        vm.onlineUsers = convertToUsers(eventData.currentUsersList);
                        //TODO: write notification
                    });
                };

                usersService.userDisconnected = function (eventData) {
                    $scope.$apply(function () {
                        vm.onlineUsers = convertToUsers(eventData.currentUsersList);
                        //TODO: write notification
                    });
                };
            });
        }

        function playMatch(user) {
            debugger;
        }

        function convertToUsers(userNames) {

            var users = [];

            $.each(userNames, function (i, item) {

                users.push({
                    userName: item,
                    isPlaying: false,
                    isMe: item == userName
                });

            });

            return users;
        }
    }
})();
