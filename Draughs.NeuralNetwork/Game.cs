using System.Collections.Generic;
using System.Linq;

namespace Draughs.NeuralNetwork
{
    internal class Game
    {
        private readonly Dictionary<int, IPlayer> _players;
        private List<int> _board;
        public List<int> Board => _board;
        private int _curentPlayer = 1;
        private int _moves = 0;
        private bool _isGameEnd => !_board.Any(x => x < 0) || !_board.Any(x => x > 0);
        private bool _noValidMoves => !new PieceMovement().GetValidMoves(_board, _curentPlayer).Any();

        public bool Draw { get; private set; }
        public IPlayer Winner { get; private set; }
        public IPlayer Loser { get; private set; }

        public Game(IPlayer player1, IPlayer player2)
        {
            _players = new Dictionary<int, IPlayer>
            {
                { 1, player1 },
                { -1, player2 }
            };

            _board = CreateBoard();
            StartGame();
        }

        private void StartGame()
        {
            while (!_isGameEnd && !_noValidMoves && _moves <= 100)
            {
                _board = _players[_curentPlayer].Move(_board, _curentPlayer);
                _curentPlayer *= -1;
                _moves++;
            }

            if (_noValidMoves)
            {
                _curentPlayer *= -1;
            }

            Draw = _moves >= 150;
            Winner = _players[_curentPlayer];
            Loser = _players[-_curentPlayer];
        }

        private List<int> CreateBoard()
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