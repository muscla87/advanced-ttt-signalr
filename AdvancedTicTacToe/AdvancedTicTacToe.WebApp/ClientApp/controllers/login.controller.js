
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

    LoginController.$inject = ['$http', '$templateCache', '$timeout', 'navigationService', 'toaster', 'userIdentity'];

    function LoginController($http, $templateCache, $timeout, navigationService, toaster, userIdentity) {
        /* jshint validthis:true */

        if (userIdentity.isAuthenticated) {
            navigationService.navigateTo("home", { addCacheBust: true });
            return;
        }

        var vm = this;

        vm.isBusy = false;

        vm.user = {
            UserName: '',
            Password: '',
            RememberMe: true
        };

        vm.registerClick = registerClick;
        vm.cancelLogin = cancelLogin;
        vm.sendLoginForm = sendLoginForm;

        function registerClick() {
            navigationService.navigateTo("register");
        };

        function cancelLogin() {
            navigationService.navigateTo("home", { addCacheBust: true });
        };

        function sendLoginForm() {

            var formData = vm.user;
            vm.isBusy = true;
            $timeout(function () {
                $http.post(navigationService.getFullUrl('Template/Security/Login'), formData).then(function (result) {
                    result = result.data;
                    if (result.Result == "Success") {
                        //toaster.pop('success', "", "Logged in successfully");
                        $templateCache.removeAll(); //To be sure that all templates will be reloaded considering the user logged in
                        navigationService.navigateTo("play", { refreshPage: true, addCacheBust: true });
                    }
                    else {
                        vm.isBusy = false;
                        if (result.Result == "ValidationError") {
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
                    }
                });
            }, 1000); //add fixed timeout to avoid glitchy loading animation


        };


    }
})();
