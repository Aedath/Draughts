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

        public static List<int> ParseToIntList(this ObservableCollection<Piece> board)
        {
            var result = new List<int>();
            for (int y = 0; y < 8; y++)
            {
                for (int x = 0; x < 8; x++)
                {
                    if (!board.Any(p => (p.Position.X == x && p.Position.Y == y)) || !board.Single(p => p.Position.X == x && p.Position.Y == y).HasPiece)
                    {
                        result.Add(0);
                        continue;
                    }

                    var piece = board.Single(p => p.Position.X == x && p.Position.Y == y);

                    result.Add(piece.IsWhite ? (piece.IsQueen ? 2 : 1) : (piece.IsQueen ? -2 : -1));
                }
            }
            return result;
        }

        public static List<Piece> ParseToPieceList(this List<int> listBoard)
        {
            var board = new List<Piece>();
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    var index = 8 * i + j;

                    if (((index / 8) % 2 == 0 && index % 2 == 0) || ((index / 8) % 2 != 0 && index % 2 != 0))
                    {
                        var piece = listBoard[index];
                        board.Add(new Piece(new Position(j, i), piece > 0, piece != 0, piece == 2 || piece == -2));
                    }
                }
            }
            return board;
        }

        public static Piece GetPiece(this ObservableCollection<Piece> board, int x, int y)
        {
            return board.Single(p => p.Position.X == x && p.Position.Y == y);
        }

        public static Piece GetPiece(this ObservableCollection<Piece> board, Position position)
        {
            return GetPiece(board, position.X, position.Y);
        }

        public static bool Exists(this ObservableCollection<Piece> board, int x, int y)
        {
            return board.Any(p => p.Position.X == x && p.Position.Y == y);
        }

        public static bool HasPiece(this ObservableCollection<Piece> board, int x, int y)
        {
            return Exists(board, x, y) && GetPiece(board, x, y).HasPiece;
        }

        public static bool HasOpposingPiece(this ObservableCollection<Piece> board, int x, int y, bool white)
        {
            return Exists(board, x, y) && GetPiece(board, x, y).HasPiece && GetPiece(board, x, y).IsWhite == white;
        }
    }
}