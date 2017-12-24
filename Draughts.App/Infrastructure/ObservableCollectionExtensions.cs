using Draughts.App.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Draughts.App.Infrastructure
{
    internal static class ObservableCollectionExtensions
    {
        public static void ForEach<T>(this ObservableCollection<T> collection, Action<T> action)
        {
            foreach (var item in collection)
            {
                action.Invoke(item);
            }
        }

        public static List<int> ParseToIntList(this ObservableCollection<Peace> board)
        {
            var result = new List<int>();
            for (int y = 0; y < 8; y++)
            {
                for (int x = 0; x < 8; x++)
                {
                    if (!board.Any(p => (p.Position.X == x && p.Position.Y == y)) || !board.Single(p => p.Position.X == x && p.Position.Y == y).HasPeace)
                    {
                        result.Add(0);
                        continue;
                    }

                    var peace = board.Single(p => p.Position.X == x && p.Position.Y == y);

                    result.Add(peace.IsWhite ? (peace.IsQueen ? 2 : 1) : (peace.IsQueen ? -2 : -1));
                }
            }
            return result;
        }

        public static Peace GetPeace(this ObservableCollection<Peace> board, int x, int y)
        {
            return board.Single(p => p.Position.X == x && p.Position.Y == y);
        }

        public static Peace GetPeace(this ObservableCollection<Peace> board, Position position)
        {
            return GetPeace(board, position.X, position.Y);
        }

        public static bool Exists(this ObservableCollection<Peace> board, int x, int y)
        {
            return board.Any(p => p.Position.X == x && p.Position.Y == y);
        }

        public static bool HasPeace(this ObservableCollection<Peace> board, int x, int y)
        {
            return Exists(board, x, y) && GetPeace(board, x, y).HasPeace;
        }
    }
}