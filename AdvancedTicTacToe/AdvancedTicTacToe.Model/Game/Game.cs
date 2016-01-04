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

        public Game(string id, string player1Name, string player2Name, bool usePowerUps, int turnTimer)
        {
            UniqueId = id;
            Player1Name = player1Name;
            Player2Name = player2Name;
            ActivePlayer = Player1Name;
            StartPlayer = random.NextDouble() < 0.5 ? player1Name : player2Name;
            StartOuterIndex = random.Next(0, 10);
            Cells = new GameBoardCell[9, 9];
            State = GameState.Running;
            player1PowerUps = new List<PowerUp>();
            player2PowerUps = new List<PowerUp>();
            InitializeRandomCells(usePowerUps);
        }

        private void InitializeRandomCells(bool usePowerUps)
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

            if (usePowerUps)
            {
                IEnumerable<PowerUp> powerUps = new[] { PowerUp.BigBomb, PowerUp.Eraser };
                foreach (var powerUp in powerUps)
                {
                    int randomSmallCellIndex = random.Next(0, 9);
                    int randomBigCellIndex = random.Next(0, 9);
                    //avoid to put a powerup in the starting external square
                    while (randomBigCellIndex == StartOuterIndex)
                        randomBigCellIndex = random.Next(0, 9);

                    this.Cells[randomBigCellIndex, randomSmallCellIndex].PowerUp = powerUp;
                }
            }
        }

        internal GameMoveResult ApplyMove(GameMove move) //newGameState is a clone of the new game state
        {
            if (State == GameState.Running && move.PlayerName == this.ActivePlayer)//The player who is making the move is not in his turn
            {
                int playerIndex = move.PlayerName == this.Player1Name ? 1 : 2;
                var powerUpsCollection = playerIndex == 1 ? this.player1PowerUps : this.player2PowerUps;
                //if it is a regular move without using power ups
                if (move.PowerUp == null)
                {
                    return ApplyRegularMove(move, powerUpsCollection);
                }
                else if (move.PowerUp == PowerUp.BigBomb)
                {
                    return ApplyBigBombPowerUp(move, powerUpsCollection);
                }
                else if (move.PowerUp == PowerUp.Eraser)
                {
                    return ApplyErasetPowerUp(move, powerUpsCollection);
                }
                else
                {
                    return GameMoveResult.Invalid;
                }
            }
            else
            {
                return GameMoveResult.Invalid;
            }
        }

        private GameMoveResult ApplyRegularMove(GameMove move, List<PowerUp> powerUpsCollection)
        {
            if (move.OuterIndex == this.ActiveOuterIndex && //The outer cells group index is the current one
                                    move.OuterIndex < this.Cells.GetLength(0) && //The outer index is within the allowed range
                                    move.InnerIndex < this.Cells.GetLength(1) && //The inner index is within the allowed range
                                    this.Cells[move.OuterIndex, move.InnerIndex].OwnerName == null) //The selected cell is free
            {
                this.Cells[move.OuterIndex, move.InnerIndex].OwnerName = move.PlayerName;
                if (this.Cells[move.OuterIndex, move.InnerIndex].PowerUp.HasValue)
                {
                    powerUpsCollection.Add(this.Cells[move.OuterIndex, move.InnerIndex].PowerUp.Value);
                    this.Cells[move.OuterIndex, move.InnerIndex].PowerUp = null;
                }

                if (CheckForVictory(move.OuterIndex) == false)
                {
                    this.ActiveOuterIndex = move.InnerIndex;
                    this.ActivePlayer = move.PlayerName != this.Player1Name ? this.Player1Name : this.Player2Name;
                }
                else
                {
                    this.State = move.PlayerName == this.Player1Name ? GameState.Player1Win : GameState.Player2Win;
                    this.WinnerName = move.PlayerName;
                }

                return GameMoveResult.Valid;
            }
            else
            {
                return GameMoveResult.Invalid;
            }
        }

        private GameMoveResult ApplyBigBombPowerUp(GameMove move, List<PowerUp> powerUpsCollection)
        {
            if (powerUpsCollection.Contains(move.PowerUp.Value))
            {
                if (move.OuterIndex < this.Cells.GetLength(0))//The outer index is within the allowed range
                {
                    powerUpsCollection.Remove(move.PowerUp.Value);
                    for (int i = 0; i < this.Cells.GetLength(1); i++)
                    {
                        this.Cells[move.OuterIndex, i].OwnerName = null;
                    }
                    this.ActiveOuterIndex = move.OuterIndex;
                    this.ActivePlayer = move.PlayerName != this.Player1Name ? this.Player1Name : this.Player2Name;
                    return GameMoveResult.Valid;
                }
                else
                {
                    return GameMoveResult.Invalid;
                }
            }
            else
            {
                return GameMoveResult.Invalid;
            }
        }

        private GameMoveResult ApplyErasetPowerUp(GameMove move, List<PowerUp> powerUpsCollection)
        {
            if (powerUpsCollection.Contains(move.PowerUp.Value))
            {
                powerUpsCollection.Remove(move.PowerUp.Value);

                if (move.OuterIndex == this.ActiveOuterIndex && //The outer cells group index is the current one
                    move.OuterIndex < this.Cells.GetLength(0) && //The outer index is within the allowed range
                    move.InnerIndex < this.Cells.GetLength(1)) //The inner index is within the allowed range
                {
                    int moveOuterIndexRow, moveOuterIndexCol, moveInnerIndexRow, moveInnerIndexCol;
                    ToRowAndCol(move.OuterIndex, out moveOuterIndexRow, out moveOuterIndexCol);
                    ToRowAndCol(move.InnerIndex, out moveInnerIndexRow, out moveInnerIndexCol);

                    for (int outerIndex = 0; outerIndex < this.Cells.GetLength(1); outerIndex++)
                    {
                        int outerIndexRow, outerIndexCol;
                        ToRowAndCol(outerIndex, out outerIndexRow, out outerIndexCol);
                        if (outerIndexRow == moveOuterIndexRow || outerIndexCol == moveOuterIndexCol)
                        {
                            for (int innerIndex = 0; innerIndex < this.Cells.GetLength(1); innerIndex++)
                            {
                                int innerIndexRow, innerIndexCol;
                                ToRowAndCol(innerIndex, out innerIndexRow, out innerIndexCol);

                                if (outerIndexRow == moveOuterIndexRow && innerIndexRow == moveInnerIndexRow)
                                {
                                    this.Cells[outerIndex, innerIndex].OwnerName = null;
                                }

                                if (outerIndexCol == moveOuterIndexCol && innerIndexCol == moveInnerIndexCol)
                                {
                                    this.Cells[outerIndex, innerIndex].OwnerName = null;
                                }
                            }
                        }
                    }

                    this.ActiveOuterIndex = move.InnerIndex;
                    this.ActivePlayer = move.PlayerName != this.Player1Name ? this.Player1Name : this.Player2Name;

                }
                return GameMoveResult.Valid;
            }
            else
            {
                return GameMoveResult.Invalid;
            }
        }


        internal void PlayerAbandon(string abandoningPlayerName)
        {
            if (string.IsNullOrWhiteSpace(abandoningPlayerName))
                throw new ArgumentException("Cannot provide empty or null string", "abandoningPlayerName");

            if (abandoningPlayerName != Player1Name && abandoningPlayerName != Player2Name)
                throw new InvalidOperationException("The provided player name is not playing the game");

            State = abandoningPlayerName == Player1Name ? GameState.Player1Abandoned : GameState.Player2Abandoned;
        }

        private bool CheckForVictory(int outerIndexToCheck)
        {
            for (int p = 0; p < victoryPatterns.GetLength(0); p++)
            {
                var winningPattern = true;
                var firstCellPlayer = this.Cells[outerIndexToCheck, victoryPatterns[p, 0] - 1].OwnerName;
                for (int i = 1; i < victoryPatterns.GetLength(1); i++)
                {
                    if (firstCellPlayer == null || this.Cells[outerIndexToCheck, victoryPatterns[p, i] - 1].OwnerName != firstCellPlayer)
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

        private void ToRowAndCol(int linearIndex, out int row, out int col)
        {
            row = linearIndex / 3;
            col = linearIndex - row * 3;
        }

        private int FromRowAndCol(int row, int col)
        {
            return row * 3 + col;
        }

        public string UniqueId { get; private set; }

        public string Player1Name { get; private set; }

        public string Player2Name { get; private set; }

        private List<PowerUp> player1PowerUps;
        public IEnumerable<PowerUp> Player1PowerUps { get { return player1PowerUps; } }

        private List<PowerUp> player2PowerUps;
        public IEnumerable<PowerUp> Player2PowerUps { get { return player2PowerUps; } }

        public string WinnerName { get; private set; }

        public string StartPlayer { get; private set; }

        public string ActivePlayer { get; private set; }

        public int StartOuterIndex { get; private set; }

        public int ActiveOuterIndex { get; private set; }

        public GameBoardCell[,] Cells { get; private set; }

        public GameState State { get; internal set; }

    }
}
