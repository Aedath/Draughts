using Prism.Events;

namespace Draughts.App.Models
{
    internal class TurnChangeEventData : EventBase
    {
        public bool IsWhiteTurn { get; set; }
        public bool IsGameEnd { get; set; }
        public int BlackCheckers { get; set; }
        public int WhiteCheckers { get; set; }
        public int Generation { get; set; }
    }
}