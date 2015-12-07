
using System;
using System.Collections.Generic;
namespace AdvancedTicTacToe.Model.Game
{
    public class GameManager : IGameManager
    {
        private static readonly Dictionary<string, Game> currentGames = new Dictionary<string, Game>();
        private static readonly Random random = new Random();

        public Game CreateNewGame(string player1, string player2)
        {
            //TODO: make it thread-safe
            //TODO: throw exception if there's already a running game between the two users
            var newGame = new Game()
            {
                Player1 = player1,
                Player2 = player2,
                StartPlayer = random.NextDouble() < 0.5 ? player1 : player2,
                StartOuterIndex = random.Next(0, 10),
                Cells = new int[9, 9],
                State = GameState.Running,
                UniqueId = currentGames.Count.ToString(),
            };

            newGame.ActiveOuterIndex = newGame.StartOuterIndex;
            newGame.ActivePlayer = newGame.StartPlayer;

            currentGames.Add(newGame.UniqueId, newGame);

            return newGame;
        }

        public GameMoveResult ApplyMove(string gameId, GameMove move, out Game newGameState) //newGameState is a clone of the new game state
        {
            Game game = null;
            if (currentGames.TryGetValue(gameId, out game))
            {
                bool isValid = true;

                if (move.PlayerName != game.ActivePlayer || //The player who is making the move is not in his turn
                    move.OuterIndex != game.ActiveOuterIndex || //The outer cells group index is not the current one
                    move.OuterIndex >= game.Cells.GetLength(0) || //The outer index is greater than the allowed value
                    move.InnerIndex >= game.Cells.GetLength(1) || //The inner index is greater than the allowed value
                    game.Cells[move.OuterIndex, move.InnerIndex] != 0) //The selected cell is not free
                {
                    isValid = false;
                }

                if (isValid)
                {
                    int playerIndex = move.PlayerName != game.Player1 ? 1 : 2;
                    game.Cells[move.OuterIndex, move.InnerIndex] = playerIndex;
                    if (CheckForVictory(game, move.OuterIndex))
                    {
                        game.ActiveOuterIndex = move.InnerIndex;
                        game.ActivePlayer = move.PlayerName != game.Player1 ? game.Player1 : game.Player2;
                    }
                    else
                    {
                        game.State = move.PlayerName == game.Player1 ? GameState.Player1Win : GameState.Player2Win;
                        game.WinnerName = move.PlayerName;
                    }

                    newGameState = game.Clone();
                    return GameMoveResult.Valid;
                }
                else
                {
                    newGameState = null;
                    return GameMoveResult.Invalid;
                }

            }
            else
            {
                newGameState = null;
                return GameMoveResult.GameNotFound;
            }
        }


        private static readonly int[,] victoryPatterns = {
                {1,2,3}, {4,5,6}, {7,8,9}, //horizontal stripes
                {1,4,7}, {2,5,8}, {3,6,9}, //vertical stripes
                {1,5,9}, {3,5,7} 		   //diagonal stripes
                };
        private bool CheckForVictory(Game game, int outerIndexToCheck)
        {

            for (int p = 0; p < victoryPatterns.GetLength(0); p++)
            {
                var winningPattern = true;
                var firstCellPlayer = game.Cells[outerIndexToCheck, victoryPatterns[p, 0] - 1];
                for (int i = 1; i < victoryPatterns.GetLength(1); i++)
                {
                    if (game.Cells[outerIndexToCheck, victoryPatterns[p, i] - 1] == firstCellPlayer)
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

        public Game GetExistingRunningGame(string player1, string player2)
        {
            foreach (var game in currentGames.Values)
            {
                if (game.State == GameState.Running &&
                    (game.Player1 == player1 || game.Player2 == player1) &&
                    (game.Player1 == player2 || game.Player2 == player2))
                {
                    return game.Clone();
                }
            }

            return null;

        }

        public Game GetExistingRunningGameById(string gameId)
        {
            Game ret = null;
            if (currentGames.TryGetValue(gameId, out ret))
            {
                return ret.Clone();
            }

            return null;
        }

        public void NotifyUserDisconnection(string userName)
        {
            throw new System.NotImplementedException();
        }

    }
}
