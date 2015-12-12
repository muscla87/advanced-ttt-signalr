
(function () {
    'use strict';

    /**
     * @ngdoc controller
     * @name advancedTicTacToe.controller:RegistrationController
     * @description Controller that manages the user registration page of the game
     */
    angular
      .module('advancedTicTacToe')
      .controller('RegistrationController', RegistrationController);

    RegistrationController.$inject = ['$http', '$templateCache', '$timeout', 'navigationService', 'toaster', 'userIdentity'];

    function RegistrationController($http, $templateCache, $timeout, navigationService, toaster, userIdentity) {
        /* jshint validthis:true */

        if (userIdentity.isAuthenticated) {
            navigationService.navigateTo("home");
            return;
        }

        var vm = this;

        vm.isBusy = false;

        vm.user = {
            UserName: '',
            Password: '',
            ConfirmPassword: '',
            Email: ''
        };

        vm.cancelRegistration = cancelRegistration;
        vm.sendRegistrationForm = sendRegistrationForm;

        function cancelRegistration() {
            navigationService.navigateTo("home");
        }

        function sendRegistrationForm() {

            var formData = vm.user;
            vm.isBusy = true;
            $timeout(function () {
                $http.post(navigationService.getFullUrl('Template/Security/Register'), formData).then(function (result) {
                    vm.isBusy = false;
                    result = result.data;
                    if (result.Result == "Success") {
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
                });
            }, 1000); //add fixed timeout to avoid glitchy loading animation


        };


    }
})();
