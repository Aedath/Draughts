using Draughts.App.Infrastructure;
using Draughts.App.Models;
using Prism.Commands;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace Draughts.App.ViewModels
{
    internal class BoardViewModel : ViewModelBase
    {
        private bool _isWhiteTurn;

        public BoardViewModel()
        {
            SelectCommand = new DelegateCommand<Peace>(OnSelect);
        }

        private void OnSelect(Peace currentPeace)
        {
            if (currentPeace.HasPeace )
            {
                Board.ForEach(x => x.IsSelected = false);
                currentPeace.IsSelected = !currentPeace.IsSelected;
                if (Board.Any(x => x.IsAvailableMove || x.IsJumpPath))
                {
                    Board.ForEach(x =>
                    {
                        x.IsJumpPath = false;
                        x.IsAvailableMove = false;
                    });
                }
               SetAvailableMoves(currentPeace);
            }
            else if (currentPeace.IsAvailableMove || Board.Any(x => !x.IsWhite && x.IsSelected))
            {
                var selectedPeace = Board.First(x => x.HasPeace && x.IsSelected);

                currentPeace.HasPeace = true;
                currentPeace.IsSelected = false;
                currentPeace.IsQueen = selectedPeace.IsQueen;
                currentPeace.IsWhite = selectedPeace.IsWhite;

                selectedPeace.HasPeace = false;
                selectedPeace.IsSelected = false;
                Board.ForEach(x =>
                {
                    x.IsJumpPath = false;
                    x.IsAvailableMove = false;
                });
            }
            else if(currentPeace.IsJumpPath)
            {
                var selectedPeace = Board.First(x => x.HasPeace && x.IsSelected);
                int newX = 0;
                int newY = 0;
                if(selectedPeace.Position.X > currentPeace.Position.X)
                {
                    newX = ((selectedPeace.Position.X - currentPeace.Position.X) / 2) + currentPeace.Position.X;
                    newY = ((selectedPeace.Position.Y - currentPeace.Position.Y) / 2) + currentPeace.Position.Y;
                }
                else
                {
                    newX = ((currentPeace.Position.X - selectedPeace.Position.X) / 2) + selectedPeace.Position.X;
                    newY = ((currentPeace.Position.Y - selectedPeace.Position.Y) / 2) + selectedPeace.Position.Y;
                }
                Board.Single(x => x.Position.X == newX && x.Position.Y == newY).HasPeace = false;

                currentPeace.HasPeace = true;
                currentPeace.IsSelected = true;
                currentPeace.IsQueen = selectedPeace.IsQueen;
                currentPeace.IsWhite = selectedPeace.IsWhite;

                selectedPeace.HasPeace = false;
                selectedPeace.IsSelected = false;
                Board.ForEach(x =>
                {
                    x.IsJumpPath = false;
                    x.IsAvailableMove = false;
                });
                GetSubsequentJumps(currentPeace.Position);
            }

            if((currentPeace.IsWhite && currentPeace.Position.Y == 0) || (!currentPeace.IsWhite && currentPeace.Position.Y == 7))
            {
                currentPeace.IsQueen = true;
            }
        }

        public ObservableCollection<Peace> Board { get; set; } = new ObservableCollection<Peace>
        {
            new Peace ( new Position(7, 7), true, true ),
            new Peace ( new Position(5, 7), true, true ),
            new Peace ( new Position(3, 7), true, true ),
            new Peace ( new Position(1, 7), true, true ),
            new Peace ( new Position(6, 6), true, true ),
            new Peace ( new Position(4, 6), true, true ),
            new Peace ( new Position(2, 6), true, true ),
            new Peace ( new Position(0, 6), true, true ),
            new Peace ( new Position(7, 5), true, true ),
            new Peace ( new Position(5, 5), true, true ),
            new Peace ( new Position(3, 5), true, true ),
            new Peace ( new Position(1, 5), true, true ),
                      
            new Peace ( new Position(0, 4) ),
            new Peace ( new Position(2, 4) ),
            new Peace ( new Position(4, 4) ),
            new Peace ( new Position(6, 4) ),
            new Peace ( new Position(1, 3) ),
            new Peace ( new Position(3, 3) ),
            new Peace ( new Position(5, 3) ),
            new Peace ( new Position(7, 3) ),

            new Peace ( new Position(0, 2), false, true ),
            new Peace ( new Position(2, 2), false, true ),
            new Peace ( new Position(4, 2), false, true ),
            new Peace ( new Position(6, 2), false, true ),
            new Peace ( new Position(1, 1), false, true ),
            new Peace ( new Position(3, 1), false, true ),
            new Peace ( new Position(5, 1), false, true ),
            new Peace ( new Position(7, 1), false, true ),
            new Peace ( new Position(0, 0), false, true ),
            new Peace ( new Position(2, 0), false, true ),
            new Peace ( new Position(4, 0), false, true ),
            new Peace ( new Position(6, 0), false, true ),
        };

        public bool IsWhiteTurn
        {
            get => _isWhiteTurn;
            set => SetProperty(ref _isWhiteTurn, value);
        }

        private void SetAvailableMoves(Peace peace)
        {
            var position = peace.Position;

            var canJump = false;

            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    if (!Board.Exists(x, y))
                        continue;

                    var destination = new Position(x, y);

                    if ((IsDiagonalRight(position, destination, 2) && Board.HasOpposingPeace(x - 1, y + 1, false) && !Board.HasPeace(x, y)) ||
                       ((IsDiagonalLeft(position, destination, 2)) && Board.HasOpposingPeace(x + 1, y + 1, false) && !Board.HasPeace(x, y)))
                    {
                        canJump = true;
                        Board.GetPeace(x, y).IsJumpPath = true;
                    }

                    if (Board.HasPeace(x, y))
                        continue;

                    if (!canJump)
                    {
                        if (IsDiagonal(position, destination))
                        {
                            Board.GetPeace(x, y).IsAvailableMove = true;
                            continue;
                        }
                    }
                }
            }
        }

        private void GetSubsequentJumps(Position position)
        {
            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    if (!Board.Exists(x, y))
                        continue;

                    var destination = new Position(x, y);

                    if ((IsDiagonalRight(position, destination, 2) && Board.HasOpposingPeace(x - 1, y + 1, false) && !Board.HasPeace(x, y)) ||
                       ((IsDiagonalLeft(position, destination, 2)) && Board.HasOpposingPeace(x + 1, y + 1, false) && !Board.HasPeace(x, y)) ||
                       ((IsDiagonalRight(position, destination, -2)) && Board.HasOpposingPeace(x + 1, y - 1, false) && !Board.HasPeace(x, y)) ||
                       ((IsDiagonalLeft(position, destination, -2) && Board.HasOpposingPeace(x - 1, y - 1, false) && !Board.HasPeace(x, y))))
                    {
                        Board.GetPeace(x, y).IsJumpPath = true;
                    }
                }
            }
        }

        private bool IsDiagonal(Position originalPosition, Position destination, int distance = 1)
        {
            return IsDiagonalLeft(originalPosition, destination, distance) || IsDiagonalRight(originalPosition, destination, distance);
        }

        private bool IsDiagonalLeft(Position originalPosition, Position destination, int distance = 1)
        {
            return (destination.X == originalPosition.X - distance && destination.Y == originalPosition.Y - distance);
        }

        private bool IsDiagonalRight(Position originalPosition, Position destination, int distance = 1)
        {
            return (destination.X == originalPosition.X + distance && destination.Y == originalPosition.Y - distance);
        }

        public ICommand SelectCommand { get; set; }
    }
}