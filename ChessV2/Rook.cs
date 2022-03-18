using System;
using System.Collections.Generic;
namespace ChessV2
{
    public class Rook : Piece
    {

        private const string BlackRook = "\u265c";
        private const string WhiteRook = "\u2656";
        public List<(int, int)> BaseMoves = new List<(int, int)>{ (0, 1), (1, 0), (0, -1), (-1, 0) };

        public Rook( (int, int) position, (int, int) aiposition, List<(int, int)> moves, bool colour, Char charRep = 'R', int pointsValue=5, bool enPassant=false, bool king = false,  bool firstMove=true, bool isPinned = false)
            : base(position, aiposition, moves, colour, charRep, pointsValue, enPassant, king, firstMove, isPinned)
        {
        }

        public override List<(int, int)> GetMoves()
        {
            return BaseMoves;
        }

        public override string ToString()
        {
            if (Colour)
            {
                return WhiteRook;
            }
            else
            {
                return BlackRook;
            }
        }

        public override List<(int, int)> GetNextMoves((int, int) move)
        {
            return QueenBishopRook(move);
        }
    }
}
