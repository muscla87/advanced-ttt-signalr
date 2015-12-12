(function () {
    'use strict';


    (function () {
        'use strict';

        angular
          .module('advancedTicTacToe')
          .factory('navigationService', navigationService);

        navigationService.$inject = ['$location', '$window'];

        function navigationService($location, $window) {
            var service = {
                navigateTo: navigateTo,
                getFullUrl: getFullUrl
            };

            return service;
            ////////////////

            function navigateTo(path, refreshPage) {
                if (refreshPage) {
                    $window.location.href = AppName + path;
                }
                else {
                    $location.path(AppName + path);
                }
            }

            function getFullUrl(path) {
                return AppName + path;
            }
        }
    })();

})();
