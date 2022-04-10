using System;
using System.Collections.Generic;
namespace ChessV2
{
    public class Bishop : Piece
    {
        private const string BlackBishop = "\u265D";
        private const string WhiteBishop = "\u2657";

        public List<(int, int)> BaseMoves = new List<(int, int)> { (1, 1), (1, -1), (-1, -1), (-1, 1), };

        public Bishop((int, int) position, (int, int) aiposition, List<Move> moves, bool colour, char charRep = 'B', int pointsValue=330, bool enPassant=false, bool king = false, bool firstMove = false, bool isPinned = false)
            : base(position, aiposition, moves, colour, charRep, pointsValue, enPassant, king, firstMove, isPinned)
        {
        }

        public override string ToString()
        {
            return Colour ? WhiteBishop : BlackBishop;
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
