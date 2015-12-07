
(function () {
    'use strict';

    /**
     * @ngdoc controller
     * @name advancedTicTacToe.controller:SingleGameController
     * @description Controller that manages the flow of a game
     */
    angular
      .module('advancedTicTacToe')
      .controller('SingleGameController', singleGameController);

    singleGameController.$inject = ['$scope', 'userName'];

    function singleGameController($scope, userName) {
        /* jshint validthis:true */
        var vm = this;

        vm.bigCells = generateEmptyGameData();
        vm.player1Name = userName;
        vm.player2Name = "Select opponent";
        vm.activePlayer = 1;
        vm.cellClicked = onCellClicked;

        function onCellClicked(cell) {
            debugger;
        }


        function generateEmptyGameData() {
            var bigCells = [];
            for (var i = 0; i < 9; i++) {
                var bigCell = { isActive: i == 4, cells: [], index: i };

                for (var j = 0; j < 9; j++) {
                    bigCell.cells.push({
                        bigCellIndex: i,
                        localIndex: j,
                        playerSelection: 0,
                        playerIcon: ''
                    });//new SmallCell(j, gameViewModel, bigCell));
                }
                bigCells.push(bigCell);
            }

            return bigCells;
        }


    }
})();
