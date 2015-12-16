
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

    singleGameController.$inject = ['$scope', '$timeout', '$routeParams', 'gameRoomService', 'userIdentity', 'navigationService'];

    function singleGameController($scope, $timeout, $routeParams, gameRoomService, userIdentity, navigationService) {
        /* jshint validthis:true */
        var vm = this;
        var roomId = $routeParams.gameId;
        vm.isBusy = false;
        vm.toggleOptionsView = toggleOptionsView;
        vm.leaveMatch = leaveMatch;
        vm.currentView = 'game';

        //#region match related things
        vm.player1 = new Player('', 'fa-times', 'p1 hvr-ripple-out');
        vm.player2 = new Player('', 'fa-circle-o', 'p2 hvr-ripple-out');
        vm.players = [vm.player1, vm.player2];
        vm.bigCells = generateEmptyGameData();
        vm.activePlayerIndex = 0;
        vm.activeBigCellIndex = 4;
        vm.cellClicked = onCellClicked;
        //#endregion

        activate();


        function activate() {
            vm.isBusy = true;

            gameRoomService.connect().then(function () {

                gameRoomService.getRoomState(roomId).then(function (roomState) {
                    if (roomState.UniqueId == roomId) {
                        updateModel(roomState);
                    }
                    vm.isBusy = false;
                });

                gameRoomService.roomStateChanged = roomStateChanged;

            });
        }


        function onCellClicked(cell) {

            if (cell.owner == null && cell.bigCellIndex == vm.activeBigCellIndex) {

                var activePlayer = vm.players[vm.activePlayerIndex];
                if (cell.playerIcon) {
                    activePlayer.powerUps.push(cell.playerIcon);
                }

                cell.owner = activePlayer;
                cell.playerIcon = activePlayer.icon + " ";

                for (var i = 0; i < 9; i++) {
                    vm.bigCells[i].isActive = false;
                    vm.activePlayerIndex = (vm.activePlayerIndex + 1) % 2;
                }
                //if (vm.activeBigCellIndex != cell.localIndex) {
                //    vm.activeBigCellIndex = cell.localIndex;
                //    vm.bigCells[vm.activeBigCellIndex].isActive = true;
                //}
                //else {
                vm.activeBigCellIndex = cell.localIndex;
                $timeout(function () { vm.bigCells[vm.activeBigCellIndex].isActive = true; }, 500);
                //}
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


        function toggleOptionsView() {
            if (vm.currentView != 'options') {
                vm.currentView = 'options';
            }
            else {
                vm.currentView = 'game';
            }
        }
        function leaveMatch() {
            navigationService.navigateTo('home');
        }

        function roomStateChanged(newRoomState) {
            if (newRoomState.UniqueId == roomId) {
                $scope.$apply(function () {
                    updateModel(newRoomState);
                });
            }
        }

        function updateModel(newRoomState) {
            vm.player1.name = newRoomState.Player1 ? newRoomState.Player1.UserName : 'Waiting...';
            vm.player2.name = newRoomState.Player2 ? newRoomState.Player2.UserName : 'Waiting...';
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
