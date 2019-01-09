using Prism.Mvvm;

namespace Draughts.App.Models
{
    internal class Position : BindableBase
    {
        public Position(int x, int y)
        {
            X = x;
            Y = y;
        }

        private int _x, _y;

        public int X
        {
            get => _x;
            set => SetProperty(ref _x, value);
        }

        public int Y
        {
            get => _y;
            set => SetProperty(ref _y, value);
        }
    }
}