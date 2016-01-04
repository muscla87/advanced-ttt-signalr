
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

    singleGameController.$inject = ['$scope', '$timeout', '$routeParams', 'usersService', 'gameRoomService', 'userIdentity', 'navigationService'];

    function singleGameController($scope, $timeout, $routeParams, usersService, gameRoomService, userIdentity, navigationService) {

        var iconList = ['Cross', 'Circle', 'Football', 'Paw', 'Heart'];
        var colorList = ['Red', 'Yellow', 'Green'];
        var timerChoiceList = [0, 15, 30, 45];

        /* jshint validthis:true */
        var vm = this;
        var roomId = parseInt($routeParams.gameId);
        vm.isBusy = false;
        vm.toggleOptionsView = toggleOptionsView;
        vm.prevIcon = prevIcon;
        vm.nextIcon = nextIcon;
        vm.prevColor = prevColor;
        vm.nextColor = nextColor;
        vm.switchTimer = switchTimer;
        vm.leaveRoom = leaveRoom;
        vm.playAgain = playAgain;
        vm.toggleUsePowerUps = toggleUsePowerUps;
        vm.setIsReady = setIsReady;
        vm.powerUpClicked = powerUpClicked;
        vm.ConvertPowerUpToCssIcon = ConvertPowerUpToCssIcon;
        vm.applyBombPowerUp = applyBombPowerUp;
        vm.currentView = 'match_init';
        vm.isGameOwner = false;
        vm.timer = timerChoiceList[1];
        vm.usePowerUps = true;

        //#region match related things
        vm.player1 = new Player('', 'fa-question', 'player_red hvr-ripple-out');
        vm.player2 = new Player('', 'fa-question', 'player_yellow hvr-ripple-out');
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

                    if (roomState && roomState.UniqueId === roomId) {
                        console.log("Received initial room state", roomState);
                        updateModel(roomState);
                    }
                    else {
                        console.warn("Received room state from another room", roomState);
                        navigationService.navigateTo('play');
                    }
                    vm.isBusy = false;
                });

                gameRoomService.roomStateChanged = roomStateChanged;

            });
        }

        function onCellClicked(cell, isHover) {

            var myPlayerIndex = getPlayerIndexByName(userIdentity.userName);
            if (vm.activePlayerIndex == myPlayerIndex) {
                if (vm.bombPowerUpCtrl) {
                    vm.bombPowerUpCtrl.selectedBigCell = vm.bombPowerUpCtrl.bigCells[cell.bigCellIndex];
                }
                else if (vm.eraserPowerUpCtrl) {
                    var moveOuterIndex, moveInnerIndex;
                    moveOuterIndex = ToRowAndCol(cell.bigCellIndex);
                    moveInnerIndex = ToRowAndCol(cell.localIndex);

                    for (var outerIndex = 0; outerIndex < 9; outerIndex++) {
                        var outerIndexObj = ToRowAndCol(outerIndex);
                        for (var innerIndex = 0; innerIndex < 9; innerIndex++) {
                            vm.eraserPowerUpCtrl.bigCells[outerIndex].cells[innerIndex].toBeCleared = false;
                            if (cell.bigCellIndex === vm.activeBigCellIndex && (outerIndexObj.row == moveOuterIndex.row || outerIndexObj.col == moveOuterIndex.col)) {
                                var innerIndexObj = ToRowAndCol(innerIndex);

                                if (outerIndexObj.row == moveOuterIndex.row && innerIndexObj.row == moveInnerIndex.row) {
                                    vm.eraserPowerUpCtrl.bigCells[outerIndex].cells[innerIndex].toBeCleared = true;
                                }

                                if (outerIndexObj.col == moveOuterIndex.col && innerIndexObj.col == moveInnerIndex.col) {
                                    vm.eraserPowerUpCtrl.bigCells[outerIndex].cells[innerIndex].toBeCleared = true;
                                }
                            }
                        }
                    }

                    if (cell.bigCellIndex === vm.activeBigCellIndex && !isHover) {
                        gameRoomService.applyMove(roomId, { OuterIndex: cell.bigCellIndex, InnerIndex: cell.localIndex, PowerUp: 1 });
                        vm.eraserPowerUpCtrl = null;
                    }
                }
                else if (cell.owner === null && cell.bigCellIndex === vm.activeBigCellIndex) {
                    gameRoomService.applyMove(roomId, {
                        OuterIndex: cell.bigCellIndex,
                        InnerIndex: cell.localIndex
                    });
                }
            }
        }

        function generateEmptyGameData() {

            var bigCells = [];
            for (var i = 0; i < 9; i++) {
                var bigCell = {
                    isActive: false,
                    cells: [], index: i
                };

                for (var j = 0; j < 9; j++) {
                    bigCell.cells.push({
                        bigCellIndex: i,
                        localIndex: j,
                        owner: null,
                        playerIcon: '',
                        powerUp: null,
                        getCssIcon: function () {
                            if (this.owner) {
                                return this.owner.iconCss;
                            }
                            else if (this.powerUp) {
                                return ConvertPowerUpToCssIcon(this.powerUp);
                            }
                        },
                    });
                }
                bigCells.push(bigCell);
            }

            return bigCells;
        }

        function toggleOptionsView() {
            if (vm.currentView !== 'options') {
                vm.currentView = 'options';
            }
            else {
                vm.currentView = 'game';
            }
        }

        function leaveRoom() {
            vm.isBusy = true;
            gameRoomService.leaveRoom(roomId).then(function () {
                vm.isBusy = false;
                navigationService.navigateTo('play');
            });
        }

        function roomStateChanged(newRoomState) {
            if (newRoomState.UniqueId === roomId) {
                $scope.$apply(function () {
                    console.log("Received room state update", newRoomState);
                    updateModel(newRoomState);
                });
            }
            else {
                console.warn("Received room state from another room", newRoomState);
            }
        }

        function updateModel(newRoomState) {
            updatePlayerModel(vm.player1, newRoomState.Player1);
            updatePlayerModel(vm.player2, newRoomState.Player2);
            vm.timer = newRoomState.TurnTimer;
            vm.usePowerUps = newRoomState.UsePowerUps;
            vm.isRoomOwner = false;
            if ((vm.player1.allowedToEdit && vm.player1.isRoomOwner) ||
                (vm.player2.allowedToEdit && vm.player2.isRoomOwner)) {
                vm.isRoomOwner = true;
            }

            if (newRoomState.CurrentPlayingGame && vm.currentView == 'match_init') {
                vm.currentView = 'match_running';
                vm.matchEndResult = null;
            }

            if (!newRoomState.CurrentPlayingGame && vm.currentView != 'match_init') {
                vm.currentView = 'match_init';
            }

            if (newRoomState.CurrentPlayingGame)
                updateGameModel(newRoomState.CurrentPlayingGame);


        }

        function updatePlayerModel(playerToUpdate, newModel) {
            playerToUpdate.name = newModel ? newModel.UserName : 'Waiting...';
            playerToUpdate.icon = newModel ? newModel.Icon : '';
            playerToUpdate.iconCss = newModel ? ConvertToCssIcon(newModel.Icon) : ConvertToCssIcon('');
            playerToUpdate.color = newModel ? newModel.Color : '';
            playerToUpdate.colorClass = newModel ? ConvertToCssColorClass(newModel.Color) : ConvertToCssColorClass('');
            playerToUpdate.isRoomOwner = newModel ? newModel.IsRoomOwner : false;
            playerToUpdate.isReady = newModel && newModel.State == 1;
            playerToUpdate.allowedToEdit = newModel ? playerToUpdate.name == userIdentity.userName && !playerToUpdate.isReady : false;
        }

        function updateGameModel(newGameModel) {
            vm.activePlayerIndex = getPlayerIndexByName(newGameModel.ActivePlayer);
            var prevActiveBigCellIndex = vm.activeBigCellIndex;
            vm.activeBigCellIndex = newGameModel.ActiveOuterIndex;

            for (var i = 0; i < 9; i++) {
                var bigCell = vm.bigCells[i];

                vm.bigCells[i].isActive = false;

                for (var j = 0; j < 9; j++) {
                    bigCell.cells[j].owner = getPlayerByName(newGameModel.Cells[i][j].OwnerName);
                    bigCell.cells[j].powerUp = newGameModel.Cells[i][j].PowerUp;
                }
            }

            vm.player1.powerUps = [];
            for (var i = 0; i < newGameModel.Player1PowerUps.length; i++) {
                //vm.player1.powerUps.push({ type: newGameModel.Player1PowerUps[i], iconCss: ConvertPowerUpToCssIcon(newGameModel.Player1PowerUps[i]) });
                vm.player1.powerUps.push((newGameModel.Player1PowerUps[i]));
            }
            vm.player2.powerUps = [];
            for (var i = 0; i < newGameModel.Player2PowerUps.length; i++) {
                //vm.player2.powerUps.push({ type: newGameModel.Player2PowerUps[i], iconCss: ConvertPowerUpToCssIcon(newGameModel.Player2PowerUps[i]) });
                vm.player2.powerUps.push((newGameModel.Player2PowerUps[i]));
            }

            //this hack is here because we want an animation of the outer cell even if the next move is on the same big cell of the previous one
            if (prevActiveBigCellIndex != vm.activeBigCellIndex) {
                vm.bigCells[vm.activeBigCellIndex].isActive = true;
            }
            else {
                $timeout(function () { vm.bigCells[vm.activeBigCellIndex].isActive = true; }, 500);
            }


            if (newGameModel.State != 0) {
                debugger;
                var message = '';
                switch (newGameModel.State) {
                    case 1:
                    case 2:
                        message = 'The other player abandoned the game';
                        break;
                    case 3:
                        message = vm.player1.name == userIdentity.userName ? "You won!" : "You lose :(";
                        break;
                    case 4:
                        message = vm.player2.name == userIdentity.userName ? "You won!" : "You lose :(";
                        break;
                }
                vm.matchEndResult = message;
            }

        }

        function nextIcon(playerIndex) { switchIcon(playerIndex, 1); }

        function prevIcon(playerIndex) { switchIcon(playerIndex, -1); }

        function switchIcon(playerIndex, direction) {
            var player = vm.players[playerIndex];
            if (player.allowedToEdit) {
                var otherPlayer = vm.players[(playerIndex + 1) % 2];

                var playerIconIndex = iconList.indexOf(player.icon);
                if (playerIconIndex != -1) {
                    playerIconIndex = (iconList.length + playerIconIndex + direction) % iconList.length;
                    while (iconList[playerIconIndex] == otherPlayer.icon) {
                        playerIconIndex = (iconList.length + playerIconIndex + direction) % iconList.length;
                    }

                    player.icon = iconList[playerIconIndex];
                    player.iconCss = ConvertToCssIcon(iconList[playerIconIndex]);
                    gameRoomService.setPlayerIcon(roomId, player.icon);
                }
            }
        }

        function nextColor(playerIndex) { switchColor(playerIndex, 1); }

        function prevColor(playerIndex) { switchColor(playerIndex, -1); }

        function switchColor(playerIndex, direction) {
            var player = vm.players[playerIndex];
            if (player.allowedToEdit) {
                var otherPlayer = vm.players[(playerIndex + 1) % 2];

                var playerColorIndex = colorList.indexOf(player.color);
                if (playerColorIndex != -1) {
                    playerColorIndex = (colorList.length + playerColorIndex + direction) % colorList.length;
                    while (colorList[playerColorIndex] == otherPlayer.color) {
                        playerColorIndex = (colorList.length + playerColorIndex + direction) % colorList.length;
                    }

                    player.color = colorList[playerColorIndex];
                    player.colorClass = ConvertToCssColorClass(colorList[playerColorIndex]);

                    gameRoomService.setPlayerColor(roomId, player.color);
                }
            }
        }

        function switchTimer(direction) {
            if (vm.isRoomOwner) {
                var timerIndex = timerChoiceList.indexOf(vm.timer);
                if (timerIndex == -1) {
                    timerIndex = 0;
                }

                timerIndex = (timerChoiceList.length + timerIndex + direction) % timerChoiceList.length;
                vm.timer = timerChoiceList[timerIndex];
                gameRoomService.setGameTurnTimer(roomId, vm.timer);
            }
        }

        function toggleUsePowerUps() {
            if (vm.isRoomOwner) {
                vm.usePowerUps = !vm.usePowerUps;
                gameRoomService.setGameUsePowerUps(roomId, vm.usePowerUps);
            }
        }

        function setIsReady() {
            gameRoomService.setPlayerReady(roomId);
        }

        function getPlayerByName(name) {
            if (vm.player1.name == name) {
                return vm.player1;
            }

            if (vm.player2.name == name) {
                return vm.player2;
            }

            return null;
        }

        function getPlayerIndexByName(name) {
            if (vm.player1.name == name) {
                return 0;
            }

            if (vm.player2.name == name) {
                return 1;
            }

            return -1;
        }

        function playAgain() {
            gameRoomService.resetRoomForNewGame(roomId);
        }

        function powerUpClicked(powerUpIndex, playerIndex) {
            var myPlayerIndex = getPlayerIndexByName(userIdentity.userName);
            if (myPlayerIndex == playerIndex && vm.activePlayerIndex == playerIndex) {
                var powerUp = vm.players[playerIndex].powerUps[powerUpIndex];

                if (powerUp == 1) { //Eraser
                    vm.bombPowerUpCtrl = null;
                    //toggle selected powerUp
                    if (vm.eraserPowerUpCtrl) {
                        vm.eraserPowerUpCtrl = null;
                    }
                    else {
                        vm.eraserPowerUpCtrl = { bigCells: angular.copy(vm.bigCells) };
                    }
                }
                else if (powerUp == 2) { //Bomb
                    vm.eraserPowerUpCtrl = null;
                    //toggle selected powerUp
                    if (vm.bombPowerUpCtrl) {
                        vm.bombPowerUpCtrl = null;
                    }
                    else {
                        vm.bombPowerUpCtrl = { bigCells: angular.copy(vm.bigCells) };
                    }
                }
                else {
                    alert("Unrecognized power up");
                }
            }
        }

        function applyBombPowerUp() {
            var myPlayerIndex = getPlayerIndexByName(userIdentity.userName);
            if (vm.activePlayerIndex == myPlayerIndex) {
                gameRoomService.applyMove(roomId, { OuterIndex: vm.bombPowerUpCtrl.bigCells.indexOf(vm.bombPowerUpCtrl.selectedBigCell), InnerIndex: 0, PowerUp: 2 });
                vm.bombPowerUpCtrl = null;
            }
        }

        function ToRowAndCol(linearIndex) {
            return {
                row: Math.floor(linearIndex / 3),
                col: linearIndex - Math.floor(linearIndex / 3) * 3
            };
        }
    }

    function ConvertToCssIcon(iconName) {
        switch (iconName) {
            case 'Cross': return 'fa-times';
            case 'Circle': return 'fa-circle-o';
            case 'Football': return 'fa-futbol-o';
            case 'Paw': return 'fa-paw';
            case 'Heart': return 'fa-heart';
            default: return 'fa-question';

        }
    }

    function ConvertToCssColorClass(colorName) {
        switch (colorName) {
            case 'Red': return 'player_red hvr-ripple-out';
            case 'Yellow': return 'player_yellow hvr-ripple-out';
            case 'Green': return 'player_green hvr-ripple-out';
            default: return '';
        }
    }

    function ConvertPowerUpToCssIcon(powerUp) {
        switch (powerUp) {
            case 1: return 'fa-eraser';
            case 2: return 'fa-bomb';
            case 3: return 'fa-history';
            default: return 'fa-question';
        }
    }


    //player class
    function Player(name, icon, colorClass) {
        this.name = name;
        this.colorClass = colorClass;
        this.icon = icon;
        this.allowedToEdit = false;
        this.powerUps = [];
    }

})();
