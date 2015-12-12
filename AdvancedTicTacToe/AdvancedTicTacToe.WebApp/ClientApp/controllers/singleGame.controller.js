
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

    singleGameController.$inject = ['$scope', 'userIdentity'];

    function singleGameController($scope, userIdentity) {
        /* jshint validthis:true */
        var vm = this;

        vm.bigCells = generateEmptyGameData();
        vm.player1Name = userIdentity.userName;
        vm.player2Name = "Select opponent";
        vm.activePlayer = 1;
        vm.cellClicked = onCellClicked;

        function onCellClicked(cell) {
            for (var i = 0; i < 9; i++) {
                vm.bigCells[i].isActive = cell.localIndex == i;
                cell.playerSelection = vm.activePlayer;
                cell.playerIcon = cell.playerSelection == 1 ? 'glyphicon-remove' : 'glyphicon-unchecked';
                vm.activePlayer = vm.activePlayer == 1 ? 2 : 1;
            }
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
