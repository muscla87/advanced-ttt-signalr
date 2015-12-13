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

            function navigateTo(path, options) {
                var defaultOptions = { refreshPage: false, addCacheBust: false };
                options = angular.extend(defaultOptions, options);

                var url = AppName + path + (options.addCacheBust ? "/" + (new Date().getTime()) : "");

                if (defaultOptions.refreshPage) {
                    $window.location.href = url;
                }
                else {
                    $location.path(url);
                }
            }

            function getFullUrl(path) {
                return AppName + path;
            }
        }
    })();

})();
