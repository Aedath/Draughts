using Draughts.App.Models;
using Prism.Events;

namespace Draughts.App.ViewModels
{
    internal class GameViewModel : ViewModelBase
    {
        private IEventAggregator _eventAggregator;

        public GameViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.GetEvent<PubSubEvent<TurnChangeEvent>>().Subscribe(x => OnTurnEnd(x));
        }

        private bool _isWhiteTurn;
        public bool IsWhiteTurn
        {
            get { return _isWhiteTurn; }
            set { SetProperty(ref _isWhiteTurn, value); }
        }

        private int _whiteResult;
        public int WhiteResult
        {
            get { return _whiteResult; }
            set { SetProperty(ref _whiteResult, value); }
        }

        private int _blackResult;
        public int BlackResult
        {
            get { return _blackResult; }
            set { SetProperty(ref _blackResult, value); }
        }

        private void OnTurnEnd(TurnChangeEvent turnChange)
        {
            IsWhiteTurn = turnChange.IsWhiteTurn;
            if (!turnChange.IsGameEnd)
            {
                return;
            }
            if(turnChange.BlackCheckers == 0)
            {
                WhiteResult++;
            }
            else
            {
                BlackResult++;
            }
        }
    }
}