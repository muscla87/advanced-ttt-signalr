
(function () {
    'use strict';

    /**
     * @ngdoc controller
     * @name advancedTicTacToe.controller:LoadingRouteController
     * @description Controller that manages the loading visibility while loading a new view during routing
     */
    angular
      .module('advancedTicTacToe')
      .controller('LoadingRouteController', LoadingRouteController);

    LoadingRouteController.$inject = ['$rootScope'];

    function LoadingRouteController($rootScope) {
        /* jshint validthis:true */
        var vm = this;

        vm.isRouteLoading = false;

        $rootScope.$on('$routeChangeStart', function () {
            vm.isRouteLoading = true;
        });

        $rootScope.$on('$routeChangeSuccess', function () {
            vm.isRouteLoading = false;
        });

        $rootScope.$on('$routeUpdate', function () {
            vm.isRouteLoading = false;
        });

    }
})();
