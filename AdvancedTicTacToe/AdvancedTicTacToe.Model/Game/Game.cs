using System;
using System.Collections.Generic;

namespace AdvancedTicTacToe.Model.Game
{
    public class Game
    {
        private static readonly Random random = new Random();
        private static readonly int[,] victoryPatterns = {
                {1,2,3}, {4,5,6}, {7,8,9}, //horizontal stripes
                {1,4,7}, {2,5,8}, {3,6,9}, //vertical stripes
                {1,5,9}, {3,5,7} 		   //diagonal stripes
        };

        public Game(string id, string player1, string player2)
        {
            UniqueId = id;
            Player1 = player1;
            Player2 = player2;
            StartPlayer = random.NextDouble() < 0.5 ? player1 : player2;
            StartOuterIndex = random.Next(0, 10);
            Cells = new GameBoardCell[9, 9];
            State = GameState.Running;
            InitializeRandomCells();
        }

        private void InitializeRandomCells()
        {

            for (var i = 0; i < this.Cells.GetLength(0); i++)
            {
                for (var j = 0; j < this.Cells.GetLength(1); j++)
                {
                    this.Cells[i, j] = new GameBoardCell()
                    {
                        BigCellIndex = i,
                        LocalIndex = j,
                        OwnerName = null,
                        PowerUp = null
                    };
                }
            }

            IEnumerable<PowerUp> powerUps = new[] { PowerUp.BigBomb, PowerUp.Eraser, PowerUp.UndoTurn };
            foreach (var powerUp in powerUps)
            {
                int randomSmallCellIndex = random.Next(0, 10);
                int randomBigCellIndex = random.Next(0, 10);
                //avoid to put a powerup in the starting external square
                while (randomBigCellIndex == StartOuterIndex)
                    randomBigCellIndex = random.Next(0, 10);

                this.Cells[randomBigCellIndex, randomSmallCellIndex].PowerUp = powerUp;
            }

        }

        internal GameMoveResult ApplyMove(GameMove move) //newGameState is a clone of the new game state
        {
            bool isValid = true;

            if (move.PlayerName != this.ActivePlayer || //The player who is making the move is not in his turn
                move.OuterIndex != this.ActiveOuterIndex || //The outer cells group index is not the current one
                move.OuterIndex >= this.Cells.GetLength(0) || //The outer index is greater than the allowed value
                move.InnerIndex >= this.Cells.GetLength(1) || //The inner index is greater than the allowed value
                this.Cells[move.OuterIndex, move.InnerIndex].OwnerName != null) //The selected cell is not free
            {
                //TODO: add proper trace here
                isValid = false;
            }

            if (isValid)
            {
                //TODO: Assign power up
                //TODO: Consider power up usage
                int playerIndex = move.PlayerName != this.Player1 ? 1 : 2;
                this.Cells[move.OuterIndex, move.InnerIndex].OwnerName = move.PlayerName;
                if (CheckForVictory(move.OuterIndex))
                {
                    this.ActiveOuterIndex = move.InnerIndex;
                    this.ActivePlayer = move.PlayerName != this.Player1 ? this.Player1 : this.Player2;
                }
                else
                {
                    this.State = move.PlayerName == this.Player1 ? GameState.Player1Win : GameState.Player2Win;
                    this.WinnerName = move.PlayerName;
                }

                return GameMoveResult.Valid;
            }
            else
            {
                return GameMoveResult.Invalid;
            }

        }

        private bool CheckForVictory(int outerIndexToCheck)
        {

            for (int p = 0; p < victoryPatterns.GetLength(0); p++)
            {
                var winningPattern = true;
                var firstCellPlayer = this.Cells[outerIndexToCheck, victoryPatterns[p, 0] - 1];
                for (int i = 1; i < victoryPatterns.GetLength(1); i++)
                {
                    if (this.Cells[outerIndexToCheck, victoryPatterns[p, i] - 1] == firstCellPlayer)
                    {
                        winningPattern = false;
                        break;
                    }
                }
                if (winningPattern)
                    return true;
            }

            return false;
        }

        public string UniqueId { get; private set; }

        public string Player1 { get; private set; }

        public string Player2 { get; private set; }

        public string WinnerName { get; private set; }

        public string StartPlayer { get; private set; }

        public string ActivePlayer { get; private set; }

        public int StartOuterIndex { get; private set; }

        public int ActiveOuterIndex { get; private set; }

        public GameBoardCell[,] Cells { get; private set; }

        public GameState State { get; private set; }

        public Game Clone()
        {
            var clonedGame = new Game(UniqueId, Player1, Player2)
            {
                StartPlayer = this.StartPlayer,
                WinnerName = this.WinnerName,
                ActivePlayer = this.ActivePlayer,
                StartOuterIndex = this.StartOuterIndex,
                ActiveOuterIndex = this.ActiveOuterIndex,
                Cells = new GameBoardCell[9, 9],
                State = this.State
            };

            for (var i = 0; i < this.Cells.GetLength(0); i++)
            {
                for (var j = 0; j < this.Cells.GetLength(1); j++)
                {
                    clonedGame.Cells[i, j] = this.Cells[i, j];
                }
            }


            return clonedGame;
        }

    }
}
