using Draughs.NeuralNetwork;
using Draughs.NeuralNetwork.Services;
using Draughts.App.Infrastructure;
using Draughts.App.Models;
using Prism.Commands;
using Prism.Events;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace Draughts.App.ViewModels
{
    internal class BoardViewModel : ViewModelBase
    {
        private bool _isWhiteTurn;
        private readonly NeuralNetworkPlayer _blackPlayer;

        public BoardViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            SelectCommand = new DelegateCommand<Piece>(OnSelect);
            ChangeTurn();
            _blackPlayer = new EvolutionService().GetPlayer();
        }

        private void OnSelect(Piece currentPiece)
        {
            if (currentPiece.CanSelect) //select checker
            {
                Board.ForEach(x => x.IsSelected = false);
                currentPiece.IsSelected = !currentPiece.IsSelected;
                Board.ForEach(x =>
                {
                    x.IsJumpPath = false;
                    x.IsAvailableMove = false;
                });
                SetAvailableMoves(currentPiece);
            }
            else if (currentPiece.IsAvailableMove || Board.Any(x => !x.IsWhite && x.IsSelected)) //regular move
            {
                var selectedPiece = Board.First(x => x.HasPiece && x.IsSelected);

                currentPiece.HasPiece = true;
                currentPiece.IsSelected = false;
                currentPiece.IsQueen = selectedPiece.IsQueen;
                currentPiece.IsWhite = selectedPiece.IsWhite;

                selectedPiece.HasPiece = false;
                selectedPiece.IsSelected = false;
                Board.ForEach(x =>
                {
                    x.IsJumpPath = false;
                    x.IsAvailableMove = false;
                    x.CanSelect = false;
                    x.IsSelected = false;
                });
            }
            else if (currentPiece.IsJumpPath) //jump
            {
                var piece = Board.First(x => x.HasPiece && x.IsSelected);
                int newX;
                int newY;
                if (piece.Position.X > currentPiece.Position.X)
                {
                    newX = ((piece.Position.X - currentPiece.Position.X) / 2) + currentPiece.Position.X;
                    newY = ((piece.Position.Y - currentPiece.Position.Y) / 2) + currentPiece.Position.Y;
                }
                else
                {
                    newX = ((currentPiece.Position.X - piece.Position.X) / 2) + piece.Position.X;
                    newY = ((currentPiece.Position.Y - piece.Position.Y) / 2) + piece.Position.Y;
                }
                Board.Single(x => x.Position.X == newX && x.Position.Y == newY).HasPiece = false;

                currentPiece.HasPiece = true;
                currentPiece.IsSelected = true;
                currentPiece.IsQueen = piece.IsQueen;
                currentPiece.IsWhite = piece.IsWhite;

                piece.HasPiece = false;
                piece.IsSelected = false;
                Board.ForEach(x =>
                {
                    x.IsJumpPath = false;
                    x.IsAvailableMove = false;
                    x.CanSelect = false;
                });
                GetSubsequentJumps(currentPiece.Position, piece.IsQueen);
                if (!Board.Any(x => x.IsJumpPath))
                {
                    Board.ForEach(x => x.IsSelected = false);
                }
            }
            else
            {
                return;
            }

            if ((currentPiece.IsWhite && currentPiece.Position.Y == 0) || (!currentPiece.IsWhite && currentPiece.Position.Y == 7))
            {
                currentPiece.IsQueen = true;
            }

            if (!Board.Any(x => x.IsJumpPath || x.IsSelected))
            {
                ChangeTurn();
            }
        }

        private void ChangeTurn()
        {
            while (true)
            {
                IsWhiteTurn = !IsWhiteTurn;
                var whitePieces = Board.Count(x => x.HasPiece && x.IsWhite);
                var blackPieces = Board.Count(x => x.HasPiece && !x.IsWhite);
                var isGameEnd = whitePieces == 0 || blackPieces == 0;
                if (isGameEnd)
                {
                    Reset();
                }

                PublishTurnEnd(isGameEnd, blackPieces, whitePieces);
                if (!IsWhiteTurn)
                {
                    var moveBoard = _blackPlayer.Move(Board.ParseToIntList(), -1);
                    if (moveBoard.SequenceEqual(Board.ParseToIntList()))
                    {
                        NoValidMoves(whitePieces, blackPieces);
                        return;
                    }

                    var move = moveBoard.ParseToPieceList();
                    foreach (var piece in move)
                    {
                        var originalPiece = Board.GetPiece(piece.Position);
                        originalPiece.HasPiece = piece.HasPiece;
                        originalPiece.IsQueen = piece.IsQueen;
                        originalPiece.IsWhite = piece.IsWhite;
                    }

                    continue;
                }

                foreach (var piece in Board.Where(x => x.IsWhite))
                {
                    SetAvailableMoves(piece);
                    if (Board.Any(x => x.IsJumpPath))
                    {
                        piece.CanSelect = true;
                    }

                    Board.ForEach(x =>
                    {
                        x.IsJumpPath = false;
                        x.IsAvailableMove = false;
                    });
                }

                if (!Board.Any(x => x.CanSelect))
                {
                    foreach (var piece in Board.Where(x => x.IsWhite))
                    {
                        SetAvailableMoves(piece);
                        if (Board.Any(x => x.IsAvailableMove))
                        {
                            piece.CanSelect = true;
                        }

                        Board.ForEach(x =>
                        {
                            x.IsJumpPath = false;
                            x.IsAvailableMove = false;
                        });
                    }
                }

                if (!Board.Any(x => x.CanSelect))
                {
                    NoValidMoves(whitePieces, blackPieces);
                }

                break;
            }
        }

        private void NoValidMoves(int whitePieces, int blackPieces)
        {
            PublishTurnEnd(true, IsWhiteTurn ? blackPieces : 0, IsWhiteTurn ? 0 : whitePieces);
            Reset();
        }

        private void PublishTurnEnd(bool isGameEnd, int blackPieces, int whitePieces)
        {
            _eventAggregator.GetEvent<PubSubEvent<TurnChangeEvent>>().Publish(
                new TurnChangeEvent
                {
                    IsWhiteTurn = IsWhiteTurn,
                    IsGameEnd = isGameEnd,
                    BlackCheckers = blackPieces,
                    WhiteCheckers = whitePieces
                });
        }

        public ObservableCollection<Piece> Board { get; set; } = new ObservableCollection<Piece>
        {
            new Piece ( new Position(7, 7), true, true ),
            new Piece ( new Position(5, 7), true, true ),
            new Piece ( new Position(3, 7), true, true ),
            new Piece ( new Position(1, 7), true, true ),
            new Piece ( new Position(6, 6), true, true ),
            new Piece ( new Position(4, 6), true, true ),
            new Piece ( new Position(2, 6), true, true ),
            new Piece ( new Position(0, 6), true, true ),
            new Piece ( new Position(7, 5), true, true ),
            new Piece ( new Position(5, 5), true, true ),
            new Piece ( new Position(3, 5), true, true ),
            new Piece ( new Position(1, 5), true, true ),

            new Piece ( new Position(6, 4) ),
            new Piece ( new Position(4, 4) ),
            new Piece ( new Position(2, 4) ),
            new Piece ( new Position(0, 4) ),
            new Piece ( new Position(7, 3) ),
            new Piece ( new Position(5, 3) ),
            new Piece ( new Position(3, 3) ),
            new Piece ( new Position(1, 3) ),

            new Piece ( new Position(6, 2), false, true ),
            new Piece ( new Position(4, 2), false, true ),
            new Piece ( new Position(2, 2), false, true ),
            new Piece ( new Position(0, 2), false, true ),
            new Piece ( new Position(7, 1), false, true ),
            new Piece ( new Position(5, 1), false, true ),
            new Piece ( new Position(3, 1), false, true ),
            new Piece ( new Position(1, 1), false, true ),
            new Piece ( new Position(6, 0), false, true ),
            new Piece ( new Position(4, 0), false, true ),
            new Piece ( new Position(2, 0), false, true ),
            new Piece ( new Position(0, 0), false, true ),
        };

        public bool IsWhiteTurn
        {
            get => _isWhiteTurn;
            set => SetProperty(ref _isWhiteTurn, value);
        }

        private void SetAvailableMoves(Piece piece)
        {
            var position = piece.Position;

            for (var x = 0; x < 8; x++)
            {
                for (var y = 0; y < 8; y++)
                {
                    if (!Board.Exists(x, y))
                    {
                        continue;
                    }

                    var destination = new Position(x, y);

                    if (IsDiagonalRight(position, destination, 2) && Board.HasOpposingPiece(x - 1, y + 1, false) && !Board.HasPiece(x, y) ||
                       (IsDiagonalLeft(position, destination, 2) && Board.HasOpposingPiece(x + 1, y + 1, false) && !Board.HasPiece(x, y)))
                    {
                        Board.GetPiece(x, y).IsJumpPath = true;
                    }

                    if (piece.IsQueen &&
                      (IsDiagonalRight(position, destination, -2) && Board.HasOpposingPiece(x + 1, y - 1, false) && !Board.HasPiece(x, y) ||
                      IsDiagonalLeft(position, destination, -2) && Board.HasOpposingPiece(x - 1, y - 1, false) && !Board.HasPiece(x, y)))
                    {
                        Board.GetPiece(x, y).IsJumpPath = true;
                    }

                    if (Board.HasPiece(x, y))
                    {
                        continue;
                    }

                    if (IsDiagonal(position, destination) ||
                       piece.IsQueen && IsDiagonal(position, destination, -1))
                    {
                        Board.GetPiece(x, y).IsAvailableMove = true;
                    }
                }
            }

            if (Board.Any(x => x.IsJumpPath)) // force player to jump if possible
            {
                Board.ForEach(x => x.IsAvailableMove = false);
            }

            if (piece.IsQueen && !Board.Any(x => x.IsJumpPath))
            {
                SetQueenMoves(position);
            }
        }

        private void SetQueenMoves(Position position)
        {
            SetUpperLeftQueenMoves(position);
            SetUpperRightQueenMoves(position);
            SetLowerLeftQueenMoves(position);
            SetLowerRightQueenMoves(position);
        }

        private void SetUpperLeftQueenMoves(Position position)
        {
            var initX = position.X;
            var initY = position.Y;
            if (initX == 0 || initY == 0)
            {
                return;
            }

            initX--;
            initY--;
            for (var x = (initX < initY ? initX : initY); x >= 0; x--)
            {
                if (!Board.GetPiece(initX, initY).HasPiece)
                {
                    Board.GetPiece(initX, initY).IsAvailableMove = true;
                }
                else
                {
                    break;
                }

                initX--;
                initY--;
            }
        }

        private void SetLowerRightQueenMoves(Position position)
        {
            var initX = position.X;
            var initY = position.Y;
            if (initX == 7 || initY == 7)
            {
                return;
            }

            initX++;
            initY++;
            do
            {
                if (!Board.GetPiece(initX, initY).HasPiece)
                {
                    Board.GetPiece(initX, initY).IsAvailableMove = true;
                }
                else
                {
                    break;
                }

                initX++;
                initY++;
            }
            while (initX <= 7 && initY <= 7);
        }

        private void SetLowerLeftQueenMoves(Position position)
        {
            var initX = position.X;
            var initY = position.Y;
            if (initX == 0 || initY == 7)
            {
                return;
            }

            initX--;
            initY++;
            do
            {
                if (!Board.GetPiece(initX, initY).HasPiece)
                {
                    Board.GetPiece(initX, initY).IsAvailableMove = true;
                }
                else
                {
                    break;
                }

                initX--;
                initY++;
            }
            while (initX >= 0 && initY <= 7);
        }

        private void SetUpperRightQueenMoves(Position position)
        {
            var initX = position.X;
            var initY = position.Y;
            if (initX == 7 || initY == 0)
            {
                return;
            }

            initX += 1;
            initY -= 1;
            do
            {
                if (!Board.GetPiece(initX, initY).HasPiece)
                {
                    Board.GetPiece(initX, initY).IsAvailableMove = true;
                }
                else
                {
                    break;
                }

                initX++;
                initY--;
            }
            while (initX <= 7 && initY >= 0);
        }

        private void GetSubsequentJumps(Position position, bool isQueen)
        {
            for (var x = 0; x < 8; x++)
            {
                for (var y = 0; y < 8; y++)
                {
                    if (!Board.Exists(x, y))
                    {
                        continue;
                    }

                    var destination = new Position(x, y);

                    if (IsDiagonalRight(position, destination, 2) && Board.HasOpposingPiece(x - 1, y + 1, false) && !Board.HasPiece(x, y) ||
                       IsDiagonalLeft(position, destination, 2) && Board.HasOpposingPiece(x + 1, y + 1, false) && !Board.HasPiece(x, y) ||
                       isQueen &&
                       (IsDiagonalRight(position, destination, -2) && Board.HasOpposingPiece(x + 1, y - 1, false) && !Board.HasPiece(x, y) ||
                        IsDiagonalLeft(position, destination, -2) && Board.HasOpposingPiece(x - 1, y - 1, false) && !Board.HasPiece(x, y)))
                    {
                        Board.GetPiece(x, y).IsJumpPath = true;
                    }
                }
            }
        }

        private static bool IsDiagonal(Position originalPosition, Position destination, int distance = 1)
        {
            return IsDiagonalLeft(originalPosition, destination, distance) || IsDiagonalRight(originalPosition, destination, distance);
        }

        private static bool IsDiagonalLeft(Position originalPosition, Position destination, int distance = 1)
        {
            return (destination.X == originalPosition.X - distance && destination.Y == originalPosition.Y - distance);
        }

        private static bool IsDiagonalRight(Position originalPosition, Position destination, int distance = 1)
        {
            return (destination.X == originalPosition.X + distance && destination.Y == originalPosition.Y - distance);
        }

        private void Reset()
        {
            for (var x = 0; x < 8; x++)
            {
                for (var y = 0; y < 8; y++)
                {
                    if (!Board.Exists(x, y))
                    {
                        continue;
                    }

                    var piece = Board.GetPiece(x, y);

                    if (y < 3)
                    {
                        piece.HasPiece = true;
                        piece.IsWhite = false;
                    }
                    else if (y > 4)
                    {
                        piece.HasPiece = true;
                        piece.IsWhite = true;
                    }
                    else
                    {
                        piece.HasPiece = false;
                        piece.IsWhite = false;
                    }
                    piece.IsQueen = false;
                    piece.IsAvailableMove = false;
                    piece.IsJumpPath = false;
                    piece.IsSelected = false;
                    piece.CanSelect = false;
                }
            }

            IsWhiteTurn = true;
        }

        private readonly IEventAggregator _eventAggregator;

        public ICommand SelectCommand { get; set; }
    }
}