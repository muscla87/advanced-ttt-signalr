(function () {
    'use strict';

    /**
     * @ngdoc overview
     * @name advancedTicTacToe
     * @description Main module of the application
     */
    angular.module('advancedTicTacToe', ['ngRoute', 'SignalR', 'toaster'])
        .config(advancedTicTacToeConfiguration);


    advancedTicTacToeConfiguration.$inject = ['$routeProvider', '$locationProvider'];

    function advancedTicTacToeConfiguration($routeProvider, $locationProvider) {
        $routeProvider.when(AppName + 'login/:bustId?', { templateUrl: '/Template/Security/Login', controller: 'LoginController', controllerAs: 'loginCtrl' });
        $routeProvider.when(AppName + 'register/:bustId?', { templateUrl: '/Template/Security/Register', controller: 'RegistrationController', controllerAs: 'registerCtrl' });
        $routeProvider.when(AppName + 'play/:bustId?', { templateUrl: function (params) { return '/Template/Game/Index?bust=' + params.bustId; }, controller: 'PlayHomeController', controllerAs: 'playHomeCtrl' });
        //$routeProvider.when(AppName + 'play', { templateUrl: '/Template/Game/Play', controller: 'PlayHomeController', controllerAs: 'playHomeCtrl' });
        $routeProvider.when(AppName + 'game/:gameId', { templateUrl: '/Template/Game/Play', controller: 'SingleGameController', controllerAs: 'singleGameCtrl' });
        $routeProvider.otherwise({ redirectTo: AppName + 'play' });
        $locationProvider.html5Mode(true);
    }
})();
