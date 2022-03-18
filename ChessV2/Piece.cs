﻿using System;
using System.Collections.Generic;
using System.Linq;
namespace ChessV2
{
    public class Piece
    {
        public (int, int) Position
        {
            get; set;
        }

        public (int, int) AIposition
        {
            get; set;
        }

        public bool Colour
        {
            get; set;
        }

        public int PointsValue
        {
            get;
        }

        public bool EnPassant
        {
            get; set;
        }

        public List<(int, int)> Moves
        {
            get; set;
        }

        public bool King
        {
            get;
        }

        public char CharRep
        {
            get;
        }

        public bool FirstMove
        {
            get; set;
        }

        public bool IsPinned
        {
            get; set;
        }

        public Piece((int, int) position, (int, int) aiposition, List<(int, int)> moves, bool colour, char charRep, int pointsValue, bool enPassant, bool king, bool firstMove, bool isPinned)
        {
            CharRep = charRep;
            Position = position;
            AIposition = aiposition;
            Colour = colour;
            PointsValue = pointsValue;
            EnPassant = enPassant;
            King = king;
            Moves = moves;
            FirstMove = firstMove;
            IsPinned = isPinned;

        }

        public virtual (int, int) IsValidMove((int, int) move, (int, int) position)
        {
            (int, int) checkMove = (Math.Abs(move.Item1 - AIposition.Item1), Math.Abs(move.Item2 - AIposition.Item2));
            return checkMove;
        }

        public bool CheckSameColour(Dictionary<(int, int), Piece> occupiedSquares, (int, int) move)
        {
            return occupiedSquares.ContainsKey(move) && occupiedSquares[move].Colour == Colour;
        }

        public bool CheckMoveOnBoard((int, int) move)
        {
            switch (move.Item1)
            {
                case < 1:
                    return true;
                case > 8:
                    return true;
            }
            switch (move.Item2)
            {
                case < 1:
                    return true;
                case > 8:
                    return true;
            }
            return false;
        }

        public virtual bool CheckForCapture(Dictionary<(int, int), Piece> occupiedSquares, (int, int) move)
        {
            return occupiedSquares.ContainsKey(move) && occupiedSquares[move].Colour != Colour;
        }

        public virtual List<(int, int)> GetMoves()
        {
            return new List<(int, int)>();
        }

        public (int, int) GenNewPosition((int, int) move)
        {
            return (AIposition.Item1 + move.Item1, AIposition.Item2 + move.Item2);
        }

        public virtual List<(int, int)> GetNextMoves((int, int) move)
        {
            List<(int, int)> nextMoves = new List<(int, int)>();
            return nextMoves;
        }

        public virtual void GenerateMoves(Dictionary<(int, int), Piece> occupiedSquares, List<(int, int)> moves, ref HashSet<(int, int)> protectedSquares, bool turn, ref HashSet<(int, int)> blockCheckMoves, ref int checkCount, ref HashSet<(int, int)> illegalKingMoves, Piece oppositeKing, List<Move> moveHistory)
        {
            bool pinChecked = false;

            foreach ((int, int) move in moves)
            {
                if (!turn && pinChecked == false)
                {
                    pinChecked = true;
                    if (CheckForPinPotential(oppositeKing))
                    {
                        GetPinnedPiece(moves, occupiedSquares, oppositeKing, moveHistory);
                    }
                }
                if (IsPinned)
                {
                    break;
                }
                if (CheckSameColour(occupiedSquares, move))
                {
                    if (!turn)
                    {
                        protectedSquares.Add(move);
                    }
                    break;
                }
                if (CheckMoveOnBoard(move))
                {
                    continue;
                }
                if (IsCheck(move, occupiedSquares))
                {
                    checkCount += 1;
                    for (int i = 0; i < moves.IndexOf(move); i++)
                    {
                        blockCheckMoves.Add(moves[i]);
                    }
                    foreach ((int, int) coord in moves)
                    {
                        illegalKingMoves.Add(coord);
                    }
                    blockCheckMoves.Add(AIposition);
                    break;
                }
                if (CheckForCapture(occupiedSquares, move))
                {
                    if (turn)
                    {
                        Moves.Add(move);
                    }
                    else
                    {
                        protectedSquares.Add(move);
                    }
                    break;
                }
                if (turn)
                {
                    Moves.Add(move);
                }
                else
                {
                    protectedSquares.Add(move);
                }
            }
        }

        public List<(int, int)> QueenBishopRook((int, int) move)
        {
            List<(int, int)> movesToAdd = new List<(int, int)>();
            (int, int) nextPosition = (AIposition.Item1 + move.Item1, AIposition.Item2 + move.Item2);
            while (nextPosition.Item1 < 9 && nextPosition.Item1 > 0 && nextPosition.Item2 < 9 && nextPosition.Item2 > 0)
            {
                movesToAdd.Add(nextPosition);
                nextPosition.Item1 += move.Item1;
                nextPosition.Item2 += move.Item2;
            }
            return movesToAdd;
        }

        public bool IsCheck((int, int) move, Dictionary<(int, int), Piece> occupiedSquares)
        {
            return occupiedSquares.ContainsKey(move) && occupiedSquares[move].King;
        }

        private bool CheckForPinPotential(Piece oppositeKing)
        {
            char piece = CharRep;
            if (piece == 'Q')
            {
                piece = QueenCheckType(AIposition, oppositeKing.AIposition);
            }
            if (piece == 'B')
            {
                return DiagonalPin(IsValidMove(oppositeKing.AIposition, AIposition));
            }
            else
            {
                return RookPin(IsValidMove(oppositeKing.AIposition, AIposition));
            }
        }

        private char QueenCheckType((int, int) checkPosition, (int, int) kingPosition)
        {
            if (checkPosition.Item1 == kingPosition.Item1 || checkPosition.Item2 == kingPosition.Item2)
            {
                return 'R';
            }
            return 'B';
        }

        private bool DiagonalPin((int, int) move)
        {
            return move.Item1 == move.Item2;
        }

        private bool RookPin((int, int) move)
        {
            return move.Item1 == 0 || move.Item2 == 0;
        }

        private void GetPinnedPiece(List<(int, int)> moves, Dictionary<(int, int), Piece> occupiedSquares, Piece oppositeKing, List<Move> moveHistory)
        {
            int PieceCount = 0;
            List<Piece> pieces = new List<Piece>();
            Piece P = oppositeKing;
            char checkType = QueenCheckType(AIposition, oppositeKing.AIposition);
            if (moves.Contains(oppositeKing.AIposition))
            {
                foreach ((int, int) move in moves)
                {
                    if (PieceCount > 1)
                    {
                        break;
                    }
                    if (occupiedSquares.ContainsKey(move))
                    {
                        if (CheckSameColour(occupiedSquares, move))
                        {
                            if (!occupiedSquares[move].EnPassant)
                            {
                                Console.WriteLine("Breaking at !Enpassant");
                                break;
                            }
                            if (PieceCount == 0)
                            {
                                PieceCount += 1;
                                pieces.Add(occupiedSquares[move]);
                            }
                            
                            continue;
                        }
                        if (occupiedSquares[move].King)
                        {
                            break;
                        }
                        if (occupiedSquares[move].CharRep == 'P')
                        {
                            if (PieceCount == 0)
                            {
                                PieceCount += 1;
                                pieces.Add(occupiedSquares[move]);
                            }
                            continue;
                        }
                        pieces.Add(occupiedSquares[move]);
                        PieceCount += 1;
                    }
                    Console.WriteLine(PieceCount);
                    if (PieceCount == 1)
                    {
                        Console.WriteLine(pieces[0]);
                        if (pieces[0].CharRep == 'P')
                        {

                            if (pieces[0].EnPassant)
                            {
                                if (pieces[0].Colour == oppositeKing.Colour)
                                {
                                    pieces[0].Moves.Remove(pieces[0].Moves.Last());
                                }
                                else
                                {
                                    EnPassantCheck(pieces[0].AIposition, occupiedSquares);
                                }
                            }
                        }
                        else
                        {
                            pieces[0].Moves.Clear();
                            if (pieces[0].CharRep == checkType || pieces[0].CharRep == 'Q')
                            {
                                for (int i = 0; i < moves.IndexOf(pieces[0].AIposition); i++)
                                {
                                    occupiedSquares[move].Moves.Add(moves[i]);
                                }
                                occupiedSquares[move].Moves.Add(AIposition);
                                break;
                            }
                        }
                        
                    }
                }

            }
        }

        public virtual void EnPassantCheck((int, int) position, Dictionary<(int, int), Piece> occupiedSquares)
        {
            (int, int) posToCheck = (position.Item1 + 1, position.Item2);
            if (occupiedSquares.ContainsKey(posToCheck) && occupiedSquares[posToCheck].EnPassant)
            {
                occupiedSquares[posToCheck].Moves.Remove(occupiedSquares[posToCheck].Moves.Last());
            }
            posToCheck.Item1 -= 2;
            if (occupiedSquares.ContainsKey(posToCheck) && occupiedSquares[posToCheck].EnPassant)
            {
                occupiedSquares[posToCheck].Moves.Remove(occupiedSquares[posToCheck].Moves.Last());
            }
        }
    }
}

        

