using Prism.Mvvm;

namespace Draughts.App.Models
{
    internal class Piece : BindableBase
    {
        private Position _position;
        private bool _isQueen;
        private bool _isWhite;
        private bool _hasPiece;
        private bool _isSelected;
        private bool _isAvailableMove;
        private bool _isJumpPath;
        private bool _canSelect;

        public Piece(Position position, bool isWhite = false, bool hasPiece = false, bool isQueen = false)
        {
            IsWhite = isWhite;
            Position = position;
            HasPiece = hasPiece;
            IsQueen = isQueen;
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

        public bool HasPiece
        {
            get => _hasPiece;
            set => SetProperty(ref _hasPiece, value);
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
        public bool CanSelect
        {
            get => _canSelect;
            set => SetProperty(ref _canSelect, value);
        }
    }
}