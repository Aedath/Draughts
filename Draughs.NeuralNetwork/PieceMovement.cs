using System;
using System.Collections.Generic;
using System.Linq;

namespace Draughs.NeuralNetwork
{
    internal class PieceMovement
    {
        internal List<List<int>> GetValidMoves(List<int> gameBoard, int player)
        {
            var possibleMoves = new List<List<int>>();
            for (int i = 0; i < gameBoard.Count; i++)
            {
                if (gameBoard[i] == player || gameBoard[i] == 2 * player)
                {
                    possibleMoves.AddRange(GetJumpBoard(gameBoard, i / 8, i % 8, player));
                }
            }

            if (possibleMoves.Any())
            {
                return possibleMoves;
            }

            for (int i = 0; i < gameBoard.Count; i++)
            {
                if (gameBoard[i] == player || gameBoard[i] == 2 * player)
                {
                    possibleMoves.AddRange(MovePiece(gameBoard, i / 8, i % 8));
                }
            }

            return possibleMoves;
        }

        private List<List<int>> MovePiece(List<int> gameBoard, int x, int y)
        {
            var PossibleMoves = new List<List<int>> { };

            if (gameBoard[8 * y + x] != -1)
            {
                PossibleMoves.AddRange(MoveLeftUp(gameBoard, x, y));
                PossibleMoves.AddRange(MoveRightUp(gameBoard, x, y));
            }

            if (gameBoard[8 * y + x] != 1)
            {
                PossibleMoves.AddRange(MoveLeftDown(gameBoard, x, y));
                PossibleMoves.AddRange(MoveRightDown(gameBoard, x, y));
            }

            return PossibleMoves;
        }

        private List<List<int>> MoveRightDown(List<int> gameBoard, int x, int y)
        {
            var possibleMoves = new List<List<int>>();

            var newX = x;
            var newY = y;
            var currentPiece = gameBoard[8 * y + x];
            var isQueen = false;

            while (newY + 1 < 8 && newX + 1 < 8 && gameBoard[8 * (newY + 1) + (newX + 1)] == 0)
            {
                var move = new List<int>(gameBoard)
                {
                    [8 * y + x] = 0
                };
                newX++;
                newY++;

                if (newY == 7 && currentPiece == -1)
                {
                    currentPiece = -2;
                    isQueen = true;
                }
                move[8 * newY + newX] = currentPiece;

                possibleMoves.Add(move);

                if (currentPiece == -1 || isQueen)
                {
                    break;
                }
            }

            return possibleMoves;
        }

        private List<List<int>> MoveLeftDown(List<int> gameBoard, int x, int y)
        {
            var possibleMoves = new List<List<int>>();

            var newX = x;
            var newY = y;
            var currentPiece = gameBoard[8 * y + x];
            var isQueen = false;

            while (newY + 1 < 8 && newX - 1 >= 0 && gameBoard[8 * (newY + 1) + (newX - 1)] == 0)
            {
                var move = new List<int>(gameBoard)
                {
                    [8 * y + x] = 0
                };
                newX--;
                newY++;

                if (newY == 7 && currentPiece == -1)
                {
                    currentPiece = -2;
                    isQueen = true;
                }
                move[8 * newY + newX] = currentPiece;

                possibleMoves.Add(move);

                if (currentPiece == -1 || isQueen)
                {
                    break;
                }
            }

            return possibleMoves;
        }

        private List<List<int>> MoveRightUp(List<int> gameBoard, int x, int y)
        {
            var possibleMoves = new List<List<int>>();

            var newX = x;
            var newY = y;
            var currentPiece = gameBoard[8 * y + x];
            var isQueen = false;

            while (newY - 1 >= 0 && newX + 1 < 8 && gameBoard[8 * (newY - 1) + (newX + 1)] == 0)
            {
                var move = new List<int>(gameBoard)
                {
                    [8 * y + x] = 0
                };
                newX++;
                newY--;

                if (newY == 7 && currentPiece == -1)
                {
                    currentPiece = 2;
                    isQueen = true;
                }
                move[8 * newY + newX] = currentPiece;

                possibleMoves.Add(move);

                if (currentPiece == -1 || isQueen)
                {
                    break;
                }
            }

            return possibleMoves;
        }

        private List<List<int>> MoveLeftUp(List<int> gameBoard, int x, int y)
        {
            var possibleMoves = new List<List<int>>();

            var newX = x;
            var newY = y;
            var currentPiece = gameBoard[8 * y + x];
            var isQueen = false;

            while (newY - 1 >= 0 && newX - 1 >= 0 && gameBoard[8 * (newY - 1) + (newX - 1)] == 0)
            {
                var move = new List<int>(gameBoard)
                {
                    [8 * y + x] = 0
                };
                newX--;
                newY--;

                if (newY == 7 && currentPiece == -1)
                {
                    currentPiece = 2;
                    isQueen = true;
                }
                move[8 * newY + newX] = currentPiece;

                possibleMoves.Add(move);

                if (currentPiece == -1 || isQueen)
                {
                    break;
                }
            }

            return possibleMoves;
        }

        private List<List<int>> GetJumpBoard(List<int> gameBoard, int x, int y, int player)
        {
            var moves = new List<List<int>>();
            var jumps = GetJumps(gameBoard, x, y, player);

            if (jumps.Any())
            {
                for (int i = 0; i < jumps.Count; i++)
                {
                    var newGameBoard = new List<int>(gameBoard);
                    newGameBoard[jumps[i][2]] = newGameBoard[jumps[i][0]];
                    newGameBoard[jumps[i][0]] = 0;
                    newGameBoard[jumps[i][1]] = 0;
                    
                    if (jumps[i][2] / 8 == 0 && player == 1)
                    {
                        newGameBoard[jumps[i][2]] = 2;
                    }
                    else if (jumps[i][2] / 8 == 7 && player == -1)
                    {
                        newGameBoard[jumps[i][2]] = -2;
                    }

                    var consequentJumps = GetJumpBoard(newGameBoard, jumps[i][2] / 8, jumps[i][2] % 8, player);
                    if (consequentJumps.Any())
                    {
                        moves.AddRange(consequentJumps);
                    }
                    else
                    {
                        moves.Add(newGameBoard);
                    }
                }
            }

            return moves;
        }

        private List<List<int>> GetJumps(List<int> gameBoard, int x, int y, int player)
        {
            var jumps = new List<List<int>>();

            if (gameBoard[8 * x + y] != -1)
            {
                if ((x - 2 >= 0 && x - 2 < 8) && (y - 2 >= 0) && 
                    (gameBoard[8 * (x - 1) + (y - 1)] == -player || 
                    gameBoard[8 * (x - 1) + (y - 1)] == -2 * player) && 
                    gameBoard[8 * (x - 2) + (y - 2)] == 0)
                {
                    jumps.Add(new List<int> { 8 * x + y, 8 * (x - 1) + (y - 1), 8 * (x - (2)) + (y - 2) });
                }
                
                if ((x - 2 >= 0 && x - 2 < 8) && (y + 2 < 8) && 
                    (gameBoard[8 * (x - 1) + (y + 1)] == -player || 
                    gameBoard[8 * (x - 1) + (y + 1)] == -2 * player) && 
                    gameBoard[8 * (x - 2) + (y + 2)] == 0)
                {
                    jumps.Add(new List<int> { 8 * x + y, 8 * (x - 1) + (y + 1), 8 * (x - (2)) + (y + 2) });
                }
            }

            if (gameBoard[8 * x + y] != 1)
            {
                if ((x + 2 >= 0 && x + 2 < 8) && (y - 2 >= 0) && 
                    (gameBoard[8 * (x + 1) + (y - 1)] == -player || 
                    gameBoard[8 * (x + 1) + (y - 1)] == -2 * player) && 
                    gameBoard[8 * (x + 2) + (y - 2)] == 0)
                {
                    jumps.Add(new List<int> { 8 * x + y, 8 * (x + 1) + (y - 1), 8 * (x + 2) + (y - 2) });
                }
                
                if ((x + 2 >= 0 && x + 2 < 8) && (y + 2 < 8) && 
                    (gameBoard[8 * (x + 1) + (y + 1)] == -player || 
                    gameBoard[8 * (x + 1) + (y + 1)] == -2 * player) && 
                    gameBoard[8 * (x + 2) + (y + 2)] == 0)
                {
                    jumps.Add(new List<int> { 8 * x + y, 8 * (x + 1) + (y + 1), 8 * (x + 2) + (y + 2) });
                }
            }

            return jumps;
        }
    }
}