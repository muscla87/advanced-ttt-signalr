function InitializeGame()
{
    var SmallCell = function(index, gameViewModel, parentBigCell) {
        this.index = index;
        this.gameViewModel = gameViewModel;
        this.parentBigCell = parentBigCell;
        this.playerSelection = ko.observable();
        this.playerIcon = ko.observable();
        this.computedStyle = ko.computed(function() {
            if(!this.playerSelection())
                return "";
            else if(this.playerSelection() == 1)
                return "p1";
            else 
                return "p2";
        }, this);
        this.clicked = function() {
            console.log(this.i+" "+this.j);
            if(!this.playerSelection() && this.parentBigCell.isActive() && !this.gameViewModel.gameOver())
            {
                this.playerIcon(this.gameViewModel.activePlayer() == 1 ? "glyphicon-remove" : "glyphicon-unchecked");
                this.playerSelection(this.gameViewModel.activePlayer());
                this.gameViewModel.cellSelected(this.parentBigCell.index, this.index);
            }
        };
    };

    var GameViewModel = function() {
        this.bigCells = [];
        this.gameOver = ko.observable(false);
        this.winningBannerVisible = ko.observable(false);
        this.activePlayer = ko.observable(1);
        this.cellSelected = function(bigCellIndex, smallCellIndex) {
            
            if(this.checkVictory(bigCellIndex)) {
                this.gameOver(true);
                this.winningBannerVisible(true)
            }
            else {
                for (var i = 0; i < this.bigCells.length; i++) {
                    this.bigCells[i].isActive(i == smallCellIndex);
                }

                if(this.activePlayer() == 1)
                    this.activePlayer(2);
                else
                    this.activePlayer(1);
            }
        };
        this.dismissWinBanner = function() {
            this.winningBannerVisible(false);
        };
        
        this.checkVictory = function(bigCellIndex) {
            
            var victoryPatterns = [
                [1,2,3], [4,5,6], [7,8,9], //horizontal stripes
                [1,4,7], [2,5,8], [3,6,9], //vertical stripes
                [1,5,9], [3,5,7] 		   //diagonal stripes
            ];
            
            
            for(var p = 0; p < victoryPatterns.length; p++) {
                var winningPattern = true;
                for(var i = 0; i < victoryPatterns[p].length; i++) {
                    if(this.bigCells[bigCellIndex].cells[victoryPatterns[p][i]-1].playerSelection() != this.activePlayer()) {
                        winningPattern = false;
                        break;
                    }
                }
                if(winningPattern)
                    return true;
            }
            
            return false;
        };
    };
    
    var gameViewModel = new GameViewModel();
    var startIndex = parseInt(Math.random()*9);
    for(var i = 0; i < 9; i++) {
        var bigCell = { isActive: ko.observable(i == startIndex), cells: [], index: i };

        for(var j = 0; j < 9; j++) {
            bigCell.cells.push(new SmallCell(j,gameViewModel,bigCell));
        }
        gameViewModel.bigCells.push(bigCell);
    }
    
    
    //ko.applyBindings(gameViewModel);
}