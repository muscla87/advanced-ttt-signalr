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
        $routeProvider.when(AppName + 'login', { templateUrl: '/Template/Security/Login', controller: 'LoginController', controllerAs: 'loginCtrl' });
        $routeProvider.when(AppName + 'register', { templateUrl: '/Template/Security/Register', controller: 'productListViewModel' });
        $routeProvider.when(AppName + 'home', { templateUrl: '/Template/Game/Index', controller: 'HomeController', controllerAs: 'homeCtrl' });
        $routeProvider.when(AppName + 'play/:gameId', { templateUrl: '/Template/Game/Play', controller: 'SingleGameController', controllerAs: 'singleGameCtrl' });
        $routeProvider.otherwise({ redirectTo: AppName + 'home' });
        $locationProvider.html5Mode(true);
    }
})();
