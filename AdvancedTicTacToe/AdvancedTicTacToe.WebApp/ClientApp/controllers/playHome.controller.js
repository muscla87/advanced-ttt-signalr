(function () {
    'use strict';

    /**
     * @ngdoc controller
     * @name advancedTicTacToe.controller:PlayHomeController
     * @description Controller that manages the right panel section of the app which includes Online Users and Live Statistics
     */
    angular
      .module('advancedTicTacToe')
      .controller('PlayHomeController', PlayHomeController);

    PlayHomeController.$inject = ['$scope', '$http', '$templateCache', 'navigationService', 'usersService', 'userIdentity'/*, 'singleMatch'*/];

    function PlayHomeController($scope, $http, $templateCache, navigationService, usersService, userIdentity/*, singleMatch*/) {
        var vm = this;

        vm.onlineUsers = [];
        //methods
        vm.playMatch = playMatch;
        vm.signOff = signOff;
        vm.navigateTo = navigateTo;


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

        function navigateTo(page) {
            navigationService.navigateTo(page);
        };

        function signOff() {
            vm.isBusy = true;
            var logoutUrl = navigationService.getFullUrl("template/security/logoff");

            var postAction = $http.post(logoutUrl, {});
            postAction.then(function () {
                navigationService.navigateTo("play", { refreshPage: true, addCacheBust: true });
            });

        };

        function playMatch(user) {
            debugger;
        }

        function convertToUsers(userNames) {
            var users = [];

            $.each(userNames, function (i, item) {

                users.push({
                    userName: item,
                    isPlaying: false,
                    isMe: item == userIdentity.userName
                });

            });

            return users;
        }
    }
})();
