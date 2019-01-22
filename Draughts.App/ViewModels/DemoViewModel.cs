using Draughs.NeuralNetwork;
using Draughs.NeuralNetwork.Services;
using Draughts.App.Infrastructure;
using Draughts.App.Infrastructure.Services;
using Draughts.App.Models;
using Prism.Commands;
using Prism.Regions;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Windows.Data;
using Draughts.App.Infrastructure.Notifications;
using Timer = System.Timers.Timer;

namespace Draughts.App.ViewModels
{
    internal class DemoViewModel : ViewModelBase
    {
        private readonly IAccessService _accessService;
        private readonly INotificationService _notificationService;
        private NeuralNetworkPlayer _blackPlayer;
        private NeuralNetworkPlayer _whitePlayer;

        private readonly Timer _timer;

        public DemoViewModel(IAccessService accessService, INotificationService notificationService)
        {
            _accessService = accessService;
            _notificationService = notificationService;
            _timer = new Timer(3000) {AutoReset = true};
            _timer.Elapsed += (sender, args) => ChangeTurn();
            BindingOperations.EnableCollectionSynchronization(Board, new object());
            StartGameCommand = new DelegateCommand(async () =>
            {
                var whiteNetwork = await _accessService.GetByGeneration(WhiteGeneration);
                var blackNetwork = await _accessService.GetByGeneration(BlackGeneration);
                _whitePlayer = EvolutionService.GetPlayer(whiteNetwork.Network);
                _blackPlayer = EvolutionService.GetPlayer(blackNetwork.Network);
                Reset();
                _timer.Start();
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

        public override async void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);
            Generations.Clear();
            var generations = await _accessService.GetGenerations();
            Generations.AddRange(generations);
            WhiteGeneration = Generations[0];
            BlackGeneration = Generations[0];
        }

        public DelegateCommand StartGameCommand { get; }

        public ObservableCollection<int> Generations { get; } = new ObservableCollection<int>();

        private int _whiteGeneration;

        public int WhiteGeneration
        {
            get => _whiteGeneration;
            set => SetProperty(ref _whiteGeneration, value);
        }

        private int _blackGeneration;

        public int BlackGeneration
        {
            get => _blackGeneration;
            set => SetProperty(ref _blackGeneration, value);
        }

        private void ChangeTurn()
        {
            _timer.Stop();
            var moveBoard = !IsWhiteTurn
                ? _blackPlayer.Move(Board.ParseToIntList(), -1)
                : _whitePlayer.Move(Board.ParseToIntList(), 1);
            if (moveBoard.SequenceEqual(Board.ParseToIntList()))
            {
                _timer.Stop();
                _notificationService.NotifySuccess("Game Finished");
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
            IsWhiteTurn = !IsWhiteTurn;
            Thread.Sleep(1);
            _timer.Start();
        }

        public bool IsWhiteTurn { get; set; } = true;


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
    }
}
