using System;
using System.Collections.Generic;
namespace ChessV2
{
    public class Knight : Piece
    {
        private const string BlackKnight = "\u265e";
        private const string WhiteKnight = "\u2658";

        public List<(int, int)> BaseMoves = new List<(int, int)> { (2, 1), (-2, 1), (-2, -1), (2, -1), (1, 2), (1, -2), (-1, 2), (-1, -2) };


        public Knight( (int, int) position, (int, int) aiposition, List<Move> moves, bool colour, Char charRep = 'N', int pointsValue=320, bool enPassant=false, bool king = false, bool firstMove = false, bool isPinned = false)
            : base(position, aiposition, moves, colour, charRep, pointsValue, enPassant, king, firstMove, isPinned)
        {
        }

        public override string ToString()
        {
            return Colour ? WhiteKnight : BlackKnight;
        }

        public override List<(int, int)> GetMoves()
        {
            return BaseMoves;
        }

        public override void GenerateMoves(Dictionary<(int, int), Piece> occupiedSquares, List<(int, int)> moves, ref HashSet<(int, int)> protectedSquares, bool turn, ref HashSet<(int, int)> blockCheckMoves, ref int checkCount, ref HashSet<(int, int)> illegalKingMoves, Piece oppositeKing, Move lastMove, ref HashSet<char> checkingPieces)
        {
            (int, int) moveToAdd;
            foreach ((int, int) move in moves)
            {
                moveToAdd = GenNewPosition(move);
                if (CheckMoveOnBoard(moveToAdd))
                {
                    continue;
                } 
                if (CheckSameColour(occupiedSquares, moveToAdd))
                {
                    continue;
                }
                if (turn)
                {
                    Moves.Add(new Move(this, moveToAdd, AIposition.Item1, AIposition.Item2, CheckForCapture(occupiedSquares, moveToAdd) ? true : false));
                }
                else
                {
                    protectedSquares.Add(moveToAdd);
                }
                if (IsCheck(moveToAdd, occupiedSquares))
                {
                    checkCount += 1;
                    blockCheckMoves.Clear();
                    foreach ((int, int) coord in moves)
                    {
                        illegalKingMoves.Add(GenNewPosition(coord));
                    }
                    blockCheckMoves.Add(AIposition);
                    checkingPieces.Add(CharRep);
                }
            }
        }
    }
}

