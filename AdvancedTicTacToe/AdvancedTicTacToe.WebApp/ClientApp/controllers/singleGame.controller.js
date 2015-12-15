
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

        vm.player1 = new Player(userIdentity.userName, 'fa-times', 'p1');
        vm.player2 = new Player('Player 2', 'fa-circle-o', 'p2');

        vm.players = [vm.player1, vm.player2];
        vm.activePlayerIndex = 0;
        vm.activeBigCellIndex = 4;
        vm.cellClicked = onCellClicked;

        function onCellClicked(cell) {

            if (cell.owner == null && cell.bigCellIndex == vm.activeBigCellIndex) {

                var activePlayer = vm.players[vm.activePlayerIndex];
                if (cell.playerIcon) {
                    activePlayer.powerUps.push(cell.playerIcon);
                }

                cell.owner = activePlayer;
                cell.playerIcon = activePlayer.icon;

                for (var i = 0; i < 9; i++) {
                    vm.bigCells[i].isActive = cell.localIndex == i;

                    vm.activePlayerIndex = (vm.activePlayerIndex + 1) % 2;
                }
                vm.activeBigCellIndex = cell.localIndex;
            }
        }


        function generateEmptyGameData() {

            var power_ups = {
                0: {
                    3: 'fa-eraser'
                },
                1: {
                    7: 'fa-history'
                },
                6: {
                    1: 'fa-bomb'
                }
            };

            var bigCells = [];
            for (var i = 0; i < 9; i++) {
                var bigCell = { isActive: i == 4, cells: [], index: i };

                for (var j = 0; j < 9; j++) {



                    bigCell.cells.push({
                        bigCellIndex: i,
                        localIndex: j,
                        owner: null,
                        playerIcon: power_ups[i] ? power_ups[i][j] : ''
                    });//new SmallCell(j, gameViewModel, bigCell));
                }
                bigCells.push(bigCell);
            }

            return bigCells;
        }
    }

    //player class
    function Player(name, icon, colorClass) {

        this.name = name;
        this.colorClass = colorClass;
        this.icon = icon;
        this.powerUps = [];

    }

})();
