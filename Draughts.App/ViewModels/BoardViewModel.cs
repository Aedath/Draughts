using Draughts.App.Infrastructure;
using Draughts.App.Models;
using Prism.Commands;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Documents;
using System.Windows.Input;

namespace Draughts.App.ViewModels
{
    internal class BoardViewModel : ViewModelBase
    {
        public BoardViewModel()
        {
            SelectCommand = new DelegateCommand<Peace>(OnSelect);
        }

        private void OnSelect(Peace currentPeace)
        {
            if (currentPeace.HasPeace)
            {
                Board.ForEach(x => x.IsSelected = false);
                currentPeace.IsSelected = !currentPeace.IsSelected;
                if (Board.Any(x => x.IsAvailableMove))
                {
                    Board.ForEach(x => x.IsAvailableMove = false);
                }
                SetAvailableMoves(currentPeace);
            }
            else if (currentPeace.IsAvailableMove || !currentPeace.IsWhite)
            {
                var selectedPeace = Board.First(x => x.HasPeace && x.IsSelected);

                currentPeace.HasPeace = true;
                currentPeace.IsSelected = false;
                currentPeace.IsQueen = selectedPeace.IsQueen;
                currentPeace.IsWhite = selectedPeace.IsWhite;

                selectedPeace.HasPeace = false;
                selectedPeace.IsSelected = false;
                Board.ForEach(x => x.IsAvailableMove = false);
            }
        }

        public ObservableCollection<Peace> Board { get; set; } = new ObservableCollection<Peace>
        {
            new Peace { Position = new Position(7, 7), HasPeace = true, IsWhite = true },
            new Peace { Position = new Position(5, 7), HasPeace = true, IsWhite = true },
            new Peace { Position = new Position(3, 7), HasPeace = true, IsWhite = true },
            new Peace { Position = new Position(1, 7), HasPeace = true, IsWhite = true },
            new Peace { Position = new Position(6, 6), HasPeace = true, IsWhite = true },
            new Peace { Position = new Position(4, 6), HasPeace = true, IsWhite = true },
            new Peace { Position = new Position(2, 6), HasPeace = true, IsWhite = true },
            new Peace { Position = new Position(0, 6), HasPeace = true, IsWhite = true },
            new Peace { Position = new Position(7, 5), HasPeace = true, IsWhite = true },
            new Peace { Position = new Position(5, 5), HasPeace = true, IsWhite = true },
            new Peace { Position = new Position(3, 5), HasPeace = true, IsWhite = true },
            new Peace { Position = new Position(1, 5), HasPeace = true, IsWhite = true },

            new Peace { Position = new Position(0, 4) },
            new Peace { Position = new Position(2, 4) },
            new Peace { Position = new Position(4, 4) },
            new Peace { Position = new Position(6, 4) },
            new Peace { Position = new Position(1, 3) },
            new Peace { Position = new Position(3, 3) },
            new Peace { Position = new Position(5, 3) },
            new Peace { Position = new Position(7, 3) },

            new Peace { Position = new Position(0, 2), HasPeace = true },
            new Peace { Position = new Position(2, 2), HasPeace = true },
            new Peace { Position = new Position(4, 2), HasPeace = true },
            new Peace { Position = new Position(6, 2), HasPeace = true },
            new Peace { Position = new Position(1, 1), HasPeace = true },
            new Peace { Position = new Position(3, 1), HasPeace = true },
            new Peace { Position = new Position(5, 1), HasPeace = true },
            new Peace { Position = new Position(7, 1), HasPeace = true },
            new Peace { Position = new Position(0, 0), HasPeace = true },
            new Peace { Position = new Position(2, 0), HasPeace = true },
            new Peace { Position = new Position(4, 0), HasPeace = true },
            new Peace { Position = new Position(6, 0), HasPeace = true },
        };

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

                    if ((IsDiagonalRight(position, destination, 2) && Board.HasPeace(x + 1, y - 1)) ||
                       ((IsDiagonalLeft(position, destination, 2)) && Board.HasPeace(x - 1, y - 1)) ||
                       (Board.Any(p => p.IsJumpPath && ((IsDiagonal(p.Position, destination, -2) && Board.HasPeace()) || IsDiagonal(p.Position, destination, 2)))))
                    {
                        canJump = true;
                        Board.GetPeace(x, y).IsJumpPath = true;
                    }
                    if (Board.GetPeace(x, y).HasPeace)
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

        private void CanJump(Position originalPosition, Position destination)
        {
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