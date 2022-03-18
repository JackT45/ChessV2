using System;
using System.Collections.Generic;
namespace ChessV2
{
    public class Queen : Piece
    {

        private const string BlackQueen = "\u265B";
        private const string WhiteQueen = "\u2655";
        public List<(int, int)> BaseMoves = new List<(int, int)> { (0, 1), (0, -1), (1, 0), (-1, 0), (-1, -1), (-1, 1), (1, 1), (1, -1) };


        public Queen((int, int) position, (int, int) aiposition, List<(int, int)> moves, bool colour, Char charRep = 'Q', int pointsValue=9, bool enPassant=false, bool king = false, bool firstMove = false, bool isPinned = false)
            : base(position, aiposition, moves, colour, charRep, pointsValue, enPassant, king, firstMove, isPinned)
        {
        }

        public override string ToString()
        {
            if (Colour)
            {
                return WhiteQueen;
            }
            return BlackQueen;

        }

        public override List<(int, int)> GetMoves()
        {
            return BaseMoves;
        }

        public override List<(int, int)> GetNextMoves((int, int) move)
        {
            return QueenBishopRook(move);
        }
    }
}
