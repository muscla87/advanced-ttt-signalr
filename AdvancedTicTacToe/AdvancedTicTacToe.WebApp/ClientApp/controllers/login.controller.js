
(function () {
    'use strict';

    /**
     * @ngdoc controller
     * @name advancedTicTacToe.controller:LoginController
     * @description Controller that manages the login page of the game
     */
    angular
      .module('advancedTicTacToe')
      .controller('LoginController', LoginController);

    LoginController.$inject = ['$http', '$templateCache', 'navigationService', 'toaster', 'userName'];

    function LoginController($http, $templateCache, navigationService, toaster, userName) {
        /* jshint validthis:true */
        var vm = this;

        vm.user = {
            UserName: '',
            Password: '',
            RememberMe: false
        };

        vm.sendLoginForm = function () {

            var formData = vm.user;

            $http.post(navigationService.getFullUrl('Template/Security/Login'), formData).then(function (result) {
                result = result.data;
                if (result.Result == "Success") {
                    toaster.pop('success', "", "Logged in successfully");
                    $templateCache.removeAll(); //To be sure that all templates will be reloaded considering the user logged in
                    navigationService.navigateTo("home");
                }
                else if (result.Result == "ValidationError") {
                    for (var i = 0; i < result.Errors.length; i++) {
                        toaster.pop('warning', "", result.Errors[i].Error);
                    }
                }
                else if (result.Result == "Failed") {
                    toaster.pop('error', "", result.Error);
                }
                else {
                    toaster.pop('error', "", "Unknown error");
                }
            })


        };


    }
})();
