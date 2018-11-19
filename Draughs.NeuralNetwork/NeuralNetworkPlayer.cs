using Draughs.NeuralNetwork.Evolution;
using System.Collections.Generic;
using System.Linq;

namespace Draughs.NeuralNetwork
{
    public class NeuralNetworkPlayer : IPlayer
    {
        private int _maxDepth = 4;
        public Gene Gene;
        private PieceMovement _piece = new PieceMovement();

        public NeuralNetworkPlayer(Gene gene)
        {
            Gene = gene;
        }

        public List<int> Move(List<int> gameBoard, int player)
        {
            return Move(gameBoard, player, 1);
        }

        private List<int> Move(List<int> gameBoard, int player, int depth)
        {
            var moves = _piece.GetValidMoves(gameBoard, player);
            var scores = new List<double>();

            if (depth >= _maxDepth || EndGame(gameBoard) || !moves.Any())
            {
                return gameBoard;
            }
            else
            {
                foreach (var move in moves)
                {
                    scores.Add(GetScores(Move(move, -player, depth + 1)));
                }
            }

            if (depth % 2 != 0)
            {
                return moves[scores.IndexOf(scores.Max())];
            }
            else
            {
                return moves[scores.IndexOf(scores.Min())];
            }
        }

        private double GetScores(List<int> gameBoard)
        {
            return Gene.GetNetworkResult(CompressBoard(gameBoard))[0];
        }

        private List<double> CompressBoard(List<int> gameBoard)
        {
            var compressedBoard = new List<double>();

            for (int i = 0; i < gameBoard.Count; i++)
            {
                if (((i / 8) % 2 == 0 && i % 2 == 0) || ((i / 8) % 2 != 0 && i % 2 != 0))
                {
                    compressedBoard.Add(gameBoard[i]);
                }
            }

            return compressedBoard;
        }

        private bool EndGame(List<int> gameBoard)
        {
            return gameBoard.Count(x => x < 0) == 0 || gameBoard.Count(x => x > 0) == 0;
        }
    }
}