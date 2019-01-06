using Draughs.NeuralNetwork.Evolution;
using System.Collections.Generic;
using System.Linq;

namespace Draughs.NeuralNetwork
{
    public class NeuralNetworkPlayer : IPlayer
    {
        private const int MaxDepth = 4;
        public Gene Gene;

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
            var moves = PieceMovement.GetValidMoves(gameBoard, player);
            var scores = new List<double>();

            if (depth >= MaxDepth || EndGame(gameBoard) || !moves.Any())
            {
                return gameBoard;
            }

            foreach (var move in moves)
            {
                scores.Add(GetScores(Move(move, -player, depth + 1)));
            }

            return depth % 2 != 0 ? moves[scores.IndexOf(scores.Max())] : moves[scores.IndexOf(scores.Min())];
        }

        private double GetScores(List<int> gameBoard)
        {
            return Gene.GetNetworkResult(CompressBoard(gameBoard))[0];
        }

        private static List<double> CompressBoard(List<int> gameBoard)
        {
            return gameBoard
                .Where((t, i) => i / 8 % 2 == 0 && i % 2 == 0 || i / 8 % 2 != 0 && i % 2 != 0)
                .Select(t => (double)t).ToList();
        }

        private static bool EndGame(List<int> gameBoard)
        {
            return gameBoard.Count(x => x < 0) == 0 || gameBoard.Count(x => x > 0) == 0;
        }
    }
}