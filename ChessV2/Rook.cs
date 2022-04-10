using System;
using System.Collections.Generic;
namespace ChessV2
{
    public class Rook : Piece
    {

        private const string BlackRook = "\u265c";
        private const string WhiteRook = "\u2656";
        public List<(int, int)> BaseMoves = new List<(int, int)>{ (0, 1), (1, 0), (0, -1), (-1, 0) };

        public Rook( (int, int) position, (int, int) aiposition, List<Move> moves, bool colour, char charRep = 'R', int pointsValue=500, bool enPassant=false, bool king = false,  bool firstMove=true, bool isPinned = false)
            : base(position, aiposition, moves, colour, charRep, pointsValue, enPassant, king, firstMove, isPinned)
        {
        }

        public override List<(int, int)> GetMoves()
        {
            return BaseMoves;
        }

        public override string ToString()
        {
            return Colour ? WhiteRook : BlackRook;
        }

        public override List<(int, int)> GetNextMoves((int, int) move)
        {
            return QueenBishopRook(move);
        }
    }
}
