﻿using Draughts.App.Infrastructure.Services;
using Draughts.App.Models;
using Draughts.App.Views;
using Prism.Events;
using Prism.Regions;

namespace Draughts.App.ViewModels
{
    internal class GameViewModel : ViewModelBase
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IAccessService _accessService;
        private readonly IRegionManager _regionManager;

        public GameViewModel(IEventAggregator eventAggregator, IAccessService accessService, IRegionManager regionManager)
        {
            _eventAggregator = eventAggregator;
            _accessService = accessService;
            _regionManager = regionManager;
        }

        private bool _isWhiteTurn;

        public bool IsWhiteTurn
        {
            get => _isWhiteTurn;
            set => SetProperty(ref _isWhiteTurn, value);
        }

        private int _whiteResult;

        public int WhiteResult
        {
            get => _whiteResult;
            set => SetProperty(ref _whiteResult, value);
        }

        private int _blackResult;

        public int BlackResult
        {
            get => _blackResult;
            set => SetProperty(ref _blackResult, value);
        }

        private async void OnTurnEnd(TurnChangeEventData turnChange)
        {
            IsWhiteTurn = turnChange.IsWhiteTurn;
            if (!turnChange.IsGameEnd)
            {
                return;
            }
            if (turnChange.BlackCheckers == 0)
            {
                WhiteResult++;
            }
            else
            {
                BlackResult++;
            }

            await _accessService.AddGameResult(turnChange.Generation, turnChange.BlackCheckers == 0 ? 1 : 0);
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);
            _eventAggregator.GetEvent<PubSubEvent<TurnChangeEventData>>().Subscribe(OnTurnEnd);
            _regionManager.RequestNavigate("BoardRegion", nameof(BoardView));
        }

        public override void OnNavigatedFrom(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);
            _eventAggregator.GetEvent<PubSubEvent<TurnChangeEventData>>().Unsubscribe(OnTurnEnd);
        }
    }
}