using System.Collections.Generic;
using System.Linq;

namespace Draughs.NeuralNetwork
{
    internal class Game
    {
        private readonly Dictionary<int, IPlayer> _players;
        public List<int> Board { get; private set; }
        private int _currentPlayer = 1;
        private int _moves;
        private bool IsGameEnd => !Board.Any(x => x < 0) || !Board.Any(x => x > 0);
        private bool NoValidMoves => !PieceMovement.GetValidMoves(Board, _currentPlayer).Any();

        public bool Draw { get; private set; }
        public IPlayer Winner { get; private set; }
        public IPlayer Loser { get; private set; }

        public int PlayerPeaces(IPlayer player) =>
            Board.Count(x => x == _players.Single(p => p.Value.Equals(player)).Key);

        public int PlayerQueens(IPlayer player) =>
            Board.Count(x => x == (_players.Single(p => p.Value.Equals(player)).Key * 2));

        public Game(IPlayer player1, IPlayer player2)
        {
            _players = new Dictionary<int, IPlayer>
            {
                { 1, player1 },
                { -1, player2 }
            };

            Board = CreateBoard();
            StartGame();
        }

        private void StartGame()
        {
            while (!IsGameEnd && !NoValidMoves && _moves <= 150)
            {
                Board = _players[_currentPlayer].Move(Board, _currentPlayer);
                _currentPlayer *= -1;
                _moves++;
            }

            if (NoValidMoves)
            {
                _currentPlayer *= -1;
            }

            Draw = _moves >= 150;
            Winner = _players[_currentPlayer];
            Loser = _players[-_currentPlayer];
        }

        private static List<int> CreateBoard()
        {
            return new List<int>
            {
                -1, 0,-1, 0,-1, 0,-1, 0,
                 0,-1, 0,-1, 0,-1, 0,-1,
                -1, 0,-1, 0,-1, 0,-1, 0,
                 0, 0, 0, 0, 0, 0, 0, 0,
                 0, 0, 0, 0, 0, 0, 0, 0,
                 0, 1, 0, 1, 0, 1, 0, 1,
                 1, 0, 1, 0, 1, 0, 1, 0,
                 0, 1, 0, 1, 0, 1, 0, 1
            };
        }
    }
}