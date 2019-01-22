using System.Collections.Generic;
using System.Linq;

namespace Draughs.NeuralNetwork
{
    public static class PieceMovement
    {
        public static List<List<int>> GetValidMoves(List<int> gameBoard, int player)
        {
            var possibleMoves = new List<List<int>>();
            for (var i = 0; i < gameBoard.Count; i++)
            {
                if (gameBoard[i] == player || gameBoard[i] == 2 * player)
                {
                    possibleMoves.AddRange(GetJumpBoard(gameBoard, i % 8, i / 8, player));
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
                    possibleMoves.AddRange(MovePiece(gameBoard, i % 8, i / 8));
                }
            }

            return possibleMoves;
        }

        private static List<List<int>> MovePiece(List<int> gameBoard, int x, int y)
        {
            var possibleMoves = new List<List<int>> { };

            if (gameBoard[8 * y + x] != -1)
            {
                possibleMoves.AddRange(MoveLeftUp(gameBoard, x, y));
                possibleMoves.AddRange(MoveRightUp(gameBoard, x, y));
            }

            if (gameBoard[8 * y + x] == 1)
            {
                return possibleMoves;
            }

            possibleMoves.AddRange(MoveLeftDown(gameBoard, x, y));
            possibleMoves.AddRange(MoveRightDown(gameBoard, x, y));

            return possibleMoves;
        }

        private static List<List<int>> MoveRightDown(List<int> gameBoard, int x, int y)
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

        private static List<List<int>> MoveLeftDown(List<int> gameBoard, int x, int y)
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

        private static List<List<int>> MoveRightUp(List<int> gameBoard, int x, int y)
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

                if (newY == 0 && currentPiece == 1)
                {
                    currentPiece = 2;
                    isQueen = true;
                }
                move[8 * newY + newX] = currentPiece;

                possibleMoves.Add(move);

                if (currentPiece == 1 || isQueen)
                {
                    break;
                }
            }

            return possibleMoves;
        }

        private static List<List<int>> MoveLeftUp(List<int> gameBoard, int x, int y)
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

                if (newY == 0 && currentPiece == 1)
                {
                    currentPiece = 2;
                    isQueen = true;
                }
                move[8 * newY + newX] = currentPiece;

                possibleMoves.Add(move);

                if (currentPiece == 1 || isQueen)
                {
                    break;
                }
            }

            return possibleMoves;
        }

        private static List<List<int>> GetJumpBoard(List<int> gameBoard, int x, int y, int player)
        {
            var moves = new List<List<int>>();
            var jumps = GetJumps(gameBoard, x, y, player);

            if (!jumps.Any())
            {
                return moves;
            }

            foreach (var jump in jumps)
            {
                var newGameBoard = new List<int>(gameBoard);
                newGameBoard[jump[2]] = newGameBoard[jump[0]];
                newGameBoard[jump[0]] = 0;
                newGameBoard[jump[1]] = 0;

                switch (jump[2] / 8)
                {
                    case 0 when player == 1:
                        newGameBoard[jump[2]] = 2;
                        break;

                    case 7 when player == -1:
                        newGameBoard[jump[2]] = -2;
                        break;
                }

                var consequentJumps = GetJumpBoard(newGameBoard, jump[2] % 8, jump[2] / 8, player);
                if (consequentJumps.Any())
                {
                    moves.AddRange(consequentJumps);
                }
                else
                {
                    moves.Add(newGameBoard);
                }
            }

            return moves;
        }

        public static List<List<int>> GetJumps(List<int> gameBoard, int x, int y, int player)
        {
            var jumps = new List<List<int>>();

            if (gameBoard[8 * y + x] != -1)
            {
                if ((y - 2 >= 0 && y - 2 < 8) && (x - 2 >= 0) &&
                    (gameBoard[8 * (y - 1) + (x - 1)] == -player ||
                    gameBoard[8 * (y - 1) + (x - 1)] == -2 * player) &&
                    gameBoard[8 * (y - 2) + (x - 2)] == 0)
                {
                    jumps.Add(new List<int> { 8 * y + x, 8 * (y - 1) + (x - 1), 8 * (y - (2)) + (x - 2) });
                }

                if ((y - 2 >= 0 && y - 2 < 8) && (x + 2 < 8) &&
                    (gameBoard[8 * (y - 1) + (x + 1)] == -player ||
                    gameBoard[8 * (y - 1) + (x + 1)] == -2 * player) &&
                    gameBoard[8 * (y - 2) + (x + 2)] == 0)
                {
                    jumps.Add(new List<int> { 8 * y + x, 8 * (y - 1) + (x + 1), 8 * (y - (2)) + (x + 2) });
                }
            }

            if (gameBoard[8 * y + x] == 1)
            {
                return jumps;
            }

            if ((y + 2 >= 0 && y + 2 < 8) && (x - 2 >= 0) &&
                (gameBoard[8 * (y + 1) + (x - 1)] == -player ||
                 gameBoard[8 * (y + 1) + (x - 1)] == -2 * player) &&
                gameBoard[8 * (y + 2) + (x - 2)] == 0)
            {
                jumps.Add(new List<int> { 8 * y + x, 8 * (y + 1) + (x - 1), 8 * (y + 2) + (x - 2) });
            }

            if ((y + 2 >= 0 && y + 2 < 8) && (x + 2 < 8) &&
                (gameBoard[8 * (y + 1) + (x + 1)] == -player ||
                 gameBoard[8 * (y + 1) + (x + 1)] == -2 * player) &&
                gameBoard[8 * (y + 2) + (x + 2)] == 0)
            {
                jumps.Add(new List<int> { 8 * y + x, 8 * (y + 1) + (x + 1), 8 * (y + 2) + (x + 2) });
            }

            return jumps;
        }
    }
}