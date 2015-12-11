
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

    HomeController.$inject = ['$scope', 'navigationService', 'userName'];

    function HomeController($scope, navigationService, userName) {
        /* jshint validthis:true */
        var vm = this;

        vm.loginClick = function () {
            navigationService.navigateTo("login");
        };


    }
})();
