using System;
using System.Collections.Generic;
using System.Linq;
namespace ChessV2
{
    public class Piece
    {
        //Piece may be better as an interface, rook, bishop and queen share move generation methods but none others do
        //Generate moves parameters is messy because of this, with only some of each of pieces requiring all of the parameters
        //If can iterate over in the same way then may be worth it, using a superclass of SlidingPieces for minimum repitition of code
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

        public List<Move> Moves
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

        public Piece((int, int) position, (int, int) aiposition, List<Move> moves, bool colour, char charRep, int pointsValue, bool enPassant, bool king, bool firstMove, bool isPinned)
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
            (int, int) checkMove = (Math.Abs(move.Item1 - position.Item1), Math.Abs(move.Item2 - position.Item2));
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

        public virtual void GenerateMoves(Dictionary<(int, int), Piece> occupiedSquares, List<(int, int)> moves, ref HashSet<(int, int)> protectedSquares, bool turn, ref HashSet<(int, int)> blockCheckMoves, ref int checkCount, ref HashSet<(int, int)> illegalKingMoves, Piece oppositeKing, Move lastMove, ref HashSet<Char> checkingPieces)
        {
            bool pinChecked = false;
            if (IsPinned)
            {
                return;
            }
            foreach ((int, int) move in moves)
            {
                if (!turn && pinChecked == false)
                {
                    pinChecked = true;
                    if (CheckForPinPotential(oppositeKing))
                    {
                        GetPinnedPiece(moves, occupiedSquares, oppositeKing);
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
                    checkingPieces.Add(CharRep);
                    blockCheckMoves.Add(AIposition);
                    break;
                }
                if (CheckForCapture(occupiedSquares, move))
                {
                    if (turn)
                    {
                        Moves.Add(new Move(this, move, this.AIposition.Item1, this.AIposition.Item2, true));
                    }
                    else
                    {
                        protectedSquares.Add(move);
                    }
                    break;
                }
                if (turn)
                {
                    Moves.Add(new Move(this, move, this.AIposition.Item1, this.AIposition.Item2));
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
                piece = QueenCheckType(oppositeKing.AIposition, AIposition);
            }
            return piece == 'B'
                ? DiagonalPin(IsValidMove(oppositeKing.AIposition, AIposition))
                : RookPin(IsValidMove(oppositeKing.AIposition, AIposition));
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

        private void GetPinnedPiece(List<(int, int)> moves, Dictionary<(int, int), Piece> occupiedSquares, Piece oppositeKing)
        {
            int PieceCount = 0;
            List<Piece> pieces = new List<Piece>();
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
                }
                if (PieceCount == 1)
                {
                    if (pieces[0].CharRep == 'P')
                    {

                        if (pieces[0].EnPassant)
                        {
                            if (pieces[0].Colour == oppositeKing.Colour && pieces[0].Moves.Count > 0)
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
                        Piece p = pieces[0];
                        p.Moves.Clear();
                        if (pieces[0].CharRep == checkType || pieces[0].CharRep == 'Q')
                        {
                            for (int i = 0; i < moves.IndexOf(p.AIposition); i++)
                            {
                                p.Moves.Add(new Move(p, moves[i], p.AIposition.Item1, p.AIposition.Item2));
                            }
                            pieces[0].Moves.Add(new Move(p, AIposition, p.AIposition.Item1, p.AIposition.Item2, true));
                            
                        }
                        pieces[0].IsPinned = true;
                        
                    }
                        
                }
                

            }
        }

        public virtual void EnPassantCheck((int, int) position, Dictionary<(int, int), Piece> occupiedSquares)
        {
            (int, int) posToCheck = (position.Item1 + 1, position.Item2);
            if (occupiedSquares.ContainsKey(posToCheck) && occupiedSquares[posToCheck].EnPassant)
            {
                if (occupiedSquares[posToCheck].Moves.Count > 0)
                {
                    occupiedSquares[posToCheck].Moves.Remove(occupiedSquares[posToCheck].Moves.Last());
                }
                
            }
            posToCheck.Item1 -= 2;
            if (occupiedSquares.ContainsKey(posToCheck) && occupiedSquares[posToCheck].EnPassant)
            {
                if (occupiedSquares[posToCheck].Moves.Count > 0)
                {
                    occupiedSquares[posToCheck].Moves.Remove(occupiedSquares[posToCheck].Moves.Last());
                }
            }
        }
    }
}

        

