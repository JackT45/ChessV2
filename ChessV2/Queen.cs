using System;
using System.Collections.Generic;
namespace ChessV2
{
    public class Queen : Piece
    {
        private readonly int[] Ranks = { 8, 7, 6, 5, 4, 3, 2, 1 };
        private const string BlackQueen = "\u265B";
        private const string WhiteQueen = "\u2655";
        public List<(int, int)> BaseMoves = new List<(int, int)> { (0, 1), (0, -1), (1, 0), (-1, 0), (-1, -1), (-1, 1), (1, 1), (1, -1) };
        private readonly Dictionary<(int, int), int> Squares = new Dictionary<(int, int), int>()
        {
            [(1, 1)] = -20,
            [(1, 2)] = -10,
            [(1, 3)] = -10,
            [(1, 4)] = -5,
            [(1, 5)] = -5,
            [(1, 6)] = -10,
            [(1, 7)] = -10,
            [(1, 8)] = -20,
            [(2, 1)] = -10,
            [(2, 2)] = 0,
            [(2, 3)] = 5,
            [(2, 4)] = 0,
            [(2, 5)] = 0,
            [(2, 6)] = 0,
            [(2, 7)] = 0,
            [(2, 8)] = -10,
            [(3, 1)] = -10,
            [(3, 2)] = 5,
            [(3, 3)] = 5,
            [(3, 4)] = 5,
            [(3, 5)] = 5,
            [(3, 6)] = 5,
            [(3, 7)] = 0,
            [(3, 8)] = -10,
            [(4, 1)] = 0,
            [(4, 2)] = 0,
            [(4, 3)] = 5,
            [(4, 4)] = 5,
            [(4, 5)] = 5,
            [(4, 6)] = 5,
            [(4, 7)] = 0,
            [(4, 8)] = -5,
            [(5, 1)] = -5,
            [(5, 2)] = 0,
            [(5, 3)] = 5,
            [(5, 4)] = 5,
            [(5, 5)] = 5,
            [(5, 6)] = 5,
            [(5, 7)] = 0,
            [(5, 8)] = -5,
            [(6, 1)] = -10,
            [(6, 2)] = 0,
            [(6, 3)] = 5,
            [(6, 4)] = 5,
            [(6, 5)] = 5,
            [(6, 6)] = 5,
            [(6, 7)] = 0,
            [(6, 8)] = -10,
            [(7, 1)] = -10,
            [(7, 2)] = 0,
            [(7, 3)] = 0,
            [(7, 4)] = 0,
            [(7, 5)] = 0,
            [(7, 6)] = 0,
            [(7, 7)] = 0,
            [(7, 8)] = -10,
            [(8, 1)] = -20,
            [(8, 2)] = -10,
            [(8, 3)] = -10,
            [(8, 4)] = -5,
            [(8, 5)] = -5,
            [(8, 6)] = -10,
            [(8, 7)] = -10,
            [(8, 8)] = -20,
        };

        public Queen((int, int) position, (int, int) aiposition, List<Move> moves, bool colour, Char charRep = 'Q', int pointsValue=900, bool enPassant=false, bool king = false, bool firstMove = false, bool isPinned = false)
            : base(position, aiposition, moves, colour, charRep, pointsValue, enPassant, king, firstMove, isPinned)
        {
        }

        public override string ToString()
        {
            return Colour ? WhiteQueen : BlackQueen;
        }

        public override List<(int, int)> GetMoves()
        {
            return BaseMoves;
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
