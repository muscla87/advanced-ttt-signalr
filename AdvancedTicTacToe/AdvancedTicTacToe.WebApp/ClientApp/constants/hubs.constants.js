
(function () {
    'use strict';

    /**
     * @ngdoc service
     * @name advancedTicTacToe.service:userIdentity
     * @description Constant to expose the current userIdentity
     */
    angular
      .module('advancedTicTacToe')
      .constant("userIdentity", window.userIdentity);

    /**
     * @ngdoc service
     * @name advancedTicTacToe.service:gameHub
     * @description Constant to expose SignalR gameHub
     */
    angular
      .module('advancedTicTacToe')
      .constant("gameHub", $.connection.gameHub);
        
    /**
     * @ngdoc service
     * @name advancedTicTacToe.service:usersHub
     * @description Constant to expose SignalR usersHub
     */
    angular
      .module('advancedTicTacToe')
      .constant("usersHub", $.connection.usersHub);

})();
