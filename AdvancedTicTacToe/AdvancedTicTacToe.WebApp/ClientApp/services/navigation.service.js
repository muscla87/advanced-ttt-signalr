(function () {
    'use strict';


    (function () {
        'use strict';

        angular
          .module('advancedTicTacToe')
          .factory('navigationService', navigationService);

        navigationService.$inject = ['$location'];

        function navigationService($location) {
            var service = {
                navigateTo: navigateTo,
                getFullUrl: getFullUrl
            };

            return service;
            ////////////////

            function navigateTo(path) {
                $location.path(AppName + path);
            }

            function getFullUrl(path) {
                return AppName + path;
            }
        }
    })();

})();
