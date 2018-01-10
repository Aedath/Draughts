using Prism.Events;

namespace Draughts.App.Models
{
    internal class TurnChangeEvent : EventBase
    {
        public bool IsWhiteTurn { get; set; }
        public bool IsGameEnd { get; set; }
        public int BlackCheckers { get; set; }
        public int WhiteCheckers { get; set; }
    }
}