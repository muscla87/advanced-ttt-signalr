
(function () {
    'use strict';

    /**
     * @ngdoc controller
     * @name advancedTicTacToe.controller:HomeController
     * @description Controller that manages the home page of the game
     */
    angular
      .module('advancedTicTacToe')
      .controller('HomeController', HomeController);

    HomeController.$inject = ['$scope', '$http', '$templateCache', 'navigationService', 'userIdentity'];

    function HomeController($scope, $http, $templateCache, navigationService, userIdentity) {
        /* jshint validthis:true */
        var vm = this;

        vm.isBusy = false;

        vm.loginClick = function () {
            navigationService.navigateTo("login");
        };

        vm.logoutClick = function () {
            vm.isBusy = true;
            var logoutUrl = navigationService.getFullUrl("template/security/logoff");

            var postAction = $http.post(logoutUrl, {});
            postAction.then(function () {
                vm.isBusy = false;
                $templateCache.removeAll(); //To be sure that all templates will be reloaded considering the user logged in
                navigationService.navigateTo("home_fake");
            });

        };


    }
})();
