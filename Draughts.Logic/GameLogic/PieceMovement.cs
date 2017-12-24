using System.Collections.Generic;
using System.Linq;

namespace Draughts.Logic.GameLogic
{
    public class PieceMovement
    {
        public List<List<int>> DetermineValidMoves(List<int> GameBoard, int Player)
        {
            List<List<int>> PossibleMoves = new List<List<int>> { };

            // Proritise Jumping moves to enforce taking of peices
            for (int i = 0; i < GameBoard.Count; i++)
            {
                if (GameBoard[i] == Player || GameBoard[i] == 2 * Player)
                {
                    PossibleMoves.AddRange(DeterminJumps(GameBoard, i / 8, i % 8, Player));
                }
            }

            if (!PossibleMoves.Any())
            {
                for (int i = 0; i < GameBoard.Count; i++)
                {
                    if (GameBoard[i] == Player || GameBoard[i] == 2 * Player)
                    {
                        PossibleMoves.AddRange(MovePeice(GameBoard, i / 8, i % 8));
                    }
                }
            }

            return PossibleMoves;
        }

        // Piece Movement
        private List<List<int>> MovePeice(List<int> Gameboard, int Y, int X)
        {
            List<List<int>> PossibleMoves = new List<List<int>> { };

            if (Gameboard[8 * Y + X] != -1)
            {
                PossibleMoves.AddRange(MoveLeftForawrd(Gameboard, Y, X));
                PossibleMoves.AddRange(MoveRightForward(Gameboard, Y, X));
            }

            if (Gameboard[8 * Y + X] != 1)
            {
                PossibleMoves.AddRange(MoveLeftDown(Gameboard, Y, X));
                PossibleMoves.AddRange(MoveRightDown(Gameboard, Y, X));
            }

            return PossibleMoves;
        }

        private List<List<int>> MoveLeftForawrd(List<int> Gameboard, int Y, int X)
        {
            List<List<int>> PossibleMoves = new List<List<int>> { };

            int LUx = X, LUy = Y;
            int PlayingPiece = Gameboard[8 * Y + X];
            bool Promoted = false;

            while (LUy - 1 >= 0 && LUx - 1 >= 0 && Gameboard[8 * (LUy - 1) + (LUx - 1)] == 0)
            {
                List<int> LeftForwardMove = new List<int> { };
                LeftForwardMove.AddRange(Gameboard);
                LeftForwardMove[8 * Y + X] = 0;
                LUx--; LUy--;

                if (LUy == 0 && PlayingPiece == 1) { PlayingPiece = 2; Promoted = true; } // promote piece if needed.
                LeftForwardMove[8 * LUy + LUx] = PlayingPiece;

                // Save Move
                PossibleMoves.Add(LeftForwardMove);

                if (PlayingPiece == 1 || Promoted) { break; } // Move over as all possioble move accounted for as orignal piece wasnt a queen.
            }

            return PossibleMoves;
        }

        private List<List<int>> MoveRightForward(List<int> Gameboard, int Y, int X)
        {
            List<List<int>> PossibleMoves = new List<List<int>> { };

            int RUx = X, RUy = Y;
            int PlayingPiece = Gameboard[8 * Y + X];
            bool Promoted = false;

            while (RUy - 1 >= 0 && RUx + 1 < 8 && Gameboard[8 * (RUy - 1) + (RUx + 1)] == 0)
            {
                List<int> RightForwardMove = new List<int> { };
                RightForwardMove.AddRange(Gameboard);
                RightForwardMove[8 * Y + X] = 0;
                RUx++; RUy--;

                if (RUy == 0 && PlayingPiece == 1) { PlayingPiece = 2; Promoted = true; } // promote piece if needed.
                RightForwardMove[8 * RUy + RUx] = PlayingPiece;

                PossibleMoves.Add(RightForwardMove);

                if (PlayingPiece == 1 || Promoted) { break; } // Move over as all possioble move accounted for as orignal piece wasnt a queen.
            }

            return PossibleMoves;
        }

        private List<List<int>> MoveLeftDown(List<int> Gameboard, int Y, int X)
        {
            List<List<int>> PossibleMoves = new List<List<int>> { };

            int LDx = X, LDy = Y;
            int PlayingPiece = Gameboard[8 * Y + X];
            bool Promoted = false;

            while (LDy + 1 < 8 && LDx - 1 >= 0 && Gameboard[8 * (LDy + 1) + (LDx - 1)] == 0)
            {
                List<int> LeftDownMove = new List<int> { };
                LeftDownMove.AddRange(Gameboard);
                LeftDownMove[8 * Y + X] = 0;
                LDx--; LDy++;

                if (LDy == 7 && PlayingPiece == -1) { PlayingPiece = -2; Promoted = true; } // promote piece if needed.
                LeftDownMove[8 * LDy + LDx] = PlayingPiece;

                PossibleMoves.Add(LeftDownMove);

                if (PlayingPiece == -1 || Promoted) { break; } // Move over as all possioble move accounted for as orignal piece wasnt a queen.
            }

            return PossibleMoves;
        }

        private List<List<int>> MoveRightDown(List<int> Gameboard, int Y, int X)
        {
            List<List<int>> PossibleMoves = new List<List<int>> { };

            int RDx = X, RDy = Y;
            int PlayingPiece = Gameboard[8 * Y + X];
            bool Promoted = false;

            while (RDy + 1 < 8 && RDx + 1 < 8 && Gameboard[8 * (RDy + 1) + (RDx + 1)] == 0)
            {
                List<int> RightDownMove = new List<int> { };
                RightDownMove.AddRange(Gameboard);

                RightDownMove[8 * Y + X] = 0;
                RDx++; RDy++;

                if (RDy == 7 && PlayingPiece == -1) { PlayingPiece = -2; Promoted = true; } // promote piece if needed.
                RightDownMove[8 * RDy + RDx] = PlayingPiece;

                PossibleMoves.Add(RightDownMove);

                if (PlayingPiece == -1 || Promoted) { break; } // Move over as all possioble move accounted for as orignal piece wasnt a queen.
            }

            return PossibleMoves;
        }

        // Jump Moves

        private List<List<int>> DeterminJumps(List<int> GameBoard, int X, int Y, int Player)
        {
            List<List<int>> Moves = new List<List<int>> { };

            // Determine all aviable Jumping moves by recussion.
            List<List<int>> Jump = _CanJump(GameBoard, X, Y, Player);
            if (Jump.Any())
            {
                for (int i = 0; i < Jump.Count; i++)
                {
                    // convert jump cordinates into new game board
                    List<int> NewGameState = new List<int> { };
                    NewGameState.AddRange(GameBoard);
                    NewGameState[Jump[i][2]] = NewGameState[Jump[i][0]];
                    NewGameState[Jump[i][0]] = 0;
                    NewGameState[Jump[i][1]] = 0;

                    // promote peice if needed
                    if (Jump[i][2] / 8 == 0 && Player == 1) { NewGameState[Jump[i][2]] = 2; }
                    else if (Jump[i][2] / 8 == 7 && Player == -1) { NewGameState[Jump[i][2]] = -2; }

                    List<List<int>> FutureJumps = DeterminJumps(NewGameState, Jump[i][2] / 8, Jump[i][2] % 8, Player);
                    if (FutureJumps.Any()) { Moves.AddRange(FutureJumps); }
                    else { Moves.Add(NewGameState); }
                }
            }

            return Moves;
        }

        private List<List<int>> _CanJump(List<int> GameBoard, int X, int Y, int Player)
        {
            List<List<int>> ValidPositions = new List<List<int>> { }; // store valid board locations at end of the move

            if (GameBoard[8 * X + Y] != -1)
            {
                // Jump forward and left

                // X = row on board Y = column. both have limits 0<X<8 & 0<Y<8
                // valid if opponent piece is placed diagonally next to player piece, and square behind is free.

                if ((X - 2 >= 0 && X - 2 < 8) && (Y - 2 >= 0) && (GameBoard[8 * (X - 1) + (Y - 1)] == -Player || GameBoard[8 * (X - 1) + (Y - 1)] == -2 * Player) && GameBoard[8 * (X - 2) + (Y - 2)] == 0)
                {
                    List<int> JumpFL = new List<int> { 8 * X + Y, 8 * (X - 1) + (Y - 1), 8 * (X - (2)) + (Y - 2) };
                    ValidPositions.Add(JumpFL);
                }

                // Jump forward and right
                if ((X - 2 >= 0 && X - 2 < 8) && (Y + 2 < 8) && (GameBoard[8 * (X - 1) + (Y + 1)] == -Player || GameBoard[8 * (X - 1) + (Y + 1)] == -2 * Player) && GameBoard[8 * (X - 2) + (Y + 2)] == 0)
                {
                    List<int> JumpFR = new List<int> { 8 * X + Y, 8 * (X - 1) + (Y + 1), 8 * (X - (2)) + (Y + 2) };
                    ValidPositions.Add(JumpFR);
                }
            }

            if (GameBoard[8 * X + Y] != 1)
            {
                // Jump back and left
                if ((X + 2 >= 0 && X + 2 < 8) && (Y - 2 >= 0) && (GameBoard[8 * (X + 1) + (Y - 1)] == -Player || GameBoard[8 * (X + 1) + (Y - 1)] == -2 * Player) && GameBoard[8 * (X + 2) + (Y - 2)] == 0)
                {
                    List<int> JumpBL = new List<int> { 8 * X + Y, 8 * (X + 1) + (Y - 1), 8 * (X + 2) + (Y - 2) };
                    ValidPositions.Add(JumpBL);
                }

                // Jump back and right
                if ((X + 2 >= 0 && X + 2 < 8) && (Y + 2 < 8) && (GameBoard[8 * (X + 1) + (Y + 1)] == -Player || GameBoard[8 * (X + 1) + (Y + 1)] == -2 * Player) && GameBoard[8 * (X + 2) + (Y + 2)] == 0)
                {
                    List<int> JumpBR = new List<int> { 8 * X + Y, 8 * (X + 1) + (Y + 1), 8 * (X + 2) + (Y + 2) };
                    ValidPositions.Add(JumpBR);
                }
            }

            return ValidPositions;
        }
    }
}