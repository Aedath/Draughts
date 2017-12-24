using Draughts.Logic;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Draughts
{
    class Program
    {
        static void Main(string[] args)
        {
            var board = new Board().GetBoard();
            
            for (int y = 0; y < 8; y++)
            {
                for (int x = 0; x < 8; x++)
                {
                    var peace = "";
                    switch (board[x, y])
                    {
                        case Peace.Empty:
                            peace = " ";
                            break;
                        case Peace.Black:
                            peace = "B";
                            break;
                        case Peace.BlackKing:
                            peace = "K";
                            break;
                        case Peace.White:
                            peace = "W";
                            break;
                        case Peace.WhiteKing:
                            peace = "R";
                            break;
                        default:
                            break;
                    }
                    Console.Write(peace + "|");
                }
                Console.WriteLine();
            }

            Console.Read();
        }
    }
}
