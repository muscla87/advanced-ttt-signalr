
(function () {
    'use strict';

    /**
     * @ngdoc service
     * @name advancedTicTacToe.service:userName
     * @description Constant to expose the current userName
     */
    angular
      .module('advancedTicTacToe')
      .constant("userName", window.userName);

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
