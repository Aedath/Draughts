using Prism.Mvvm;

namespace Draughts.App.Models
{
    internal class Peace : BindableBase
    {
        private Position _position;
        private bool _isQueen;
        private bool _isWhite;
        private bool _hasPeace;
        private bool _isSelected;
        private bool _isAvailableMove;
        private bool _isJumpPath;

        public Peace(Position position, bool isWhite = false, bool hasPeace = false)
        {
            IsWhite = isWhite;
            Position = position;
            HasPeace = hasPeace;
        }

        public Position Position
        {
            get => _position;
            set => SetProperty(ref _position, value);
        }

        public bool IsSelected
        {
            get =>_isSelected;
            set => SetProperty(ref _isSelected, value);
        }

        public bool IsQueen
        {
            get => _isQueen;
            set => SetProperty(ref _isQueen, value);
        }

        public bool IsWhite
        {
            get =>_isWhite;
            set => SetProperty(ref _isWhite, value);
        }

        public bool HasPeace
        {
            get => _hasPeace;
            set => SetProperty(ref _hasPeace, value);
        }

        public bool IsAvailableMove
        {
            get => _isAvailableMove;
            set => SetProperty(ref _isAvailableMove, value);
        }

        public bool IsJumpPath
        {
            get => _isJumpPath;
            set => SetProperty(ref _isJumpPath, value);
        }
    }
}