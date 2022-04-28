using System;
using System.Collections.Generic;
namespace ChessV2
{
    public class Rook : Piece
    {
        private readonly int[] Ranks = { 8, 7, 6, 5, 4, 3, 2, 1 };
        private const string BlackRook = "\u265c";
        private const string WhiteRook = "\u2656";
        public List<(int, int)> BaseMoves = new List<(int, int)>{ (0, 1), (1, 0), (0, -1), (-1, 0) };
        private readonly Dictionary<(int, int), int> Squares = new Dictionary<(int, int), int>()
        {
            [(1, 1)] = 0,
            [(1, 2)] = 0,
            [(1, 3)] = 0,
            [(1, 4)] = 5,
            [(1, 5)] = 5,
            [(1, 6)] = 0,
            [(1, 7)] = 0,
            [(1, 8)] = 0,
            [(2, 1)] = -5,
            [(2, 2)] = 0,
            [(2, 3)] = 0,
            [(2, 4)] = 0,
            [(2, 5)] = 0,
            [(2, 6)] = 0,
            [(2, 7)] = 0,
            [(2, 8)] = -5,
            [(3, 1)] = -5,
            [(3, 2)] = 0,
            [(3, 3)] = 0,
            [(3, 4)] = 0,
            [(3, 5)] = 0,
            [(3, 6)] = 0,
            [(3, 7)] = 0,
            [(3, 8)] = -5,
            [(4, 1)] = -5,
            [(4, 2)] = 0,
            [(4, 3)] = 0,
            [(4, 4)] = 0,
            [(4, 5)] = 0,
            [(4, 6)] = 0,
            [(4, 7)] = 0,
            [(4, 8)] = -5,
            [(5, 1)] = -5,
            [(5, 2)] = 0,
            [(5, 3)] = 0,
            [(5, 4)] = 0,
            [(5, 5)] = 0,
            [(5, 6)] = 0,
            [(5, 7)] = 0,
            [(5, 8)] = -5,
            [(6, 1)] = -5,
            [(6, 2)] = 0,
            [(6, 3)] = 0,
            [(6, 4)] = 0,
            [(6, 5)] = 0,
            [(6, 6)] = 0,
            [(6, 7)] = 0,
            [(6, 8)] = -5,
            [(7, 1)] = 5,
            [(7, 2)] = 10,
            [(7, 3)] = 10,
            [(7, 4)] = 10,
            [(7, 5)] = 10,
            [(7, 6)] = 10,
            [(7, 7)] = 10,
            [(7, 8)] = 5,
            [(8, 1)] = 0,
            [(8, 2)] = 0,
            [(8, 3)] = 0,
            [(8, 4)] = 0,
            [(8, 5)] = 0,
            [(8, 6)] = 0,
            [(8, 7)] = 0,
            [(8, 8)] = 0,
        };

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

        public override double Evaluate()
        {
            if (!Colour)
            {
                return PointsValue + Squares[(AIposition.Item1, Ranks[AIposition.Item2 - 1])];
            }
            return PointsValue + Squares[AIposition];
        }
    }
}
