﻿using System;
using System.Collections.Generic;
namespace ChessV2
{
    public class Bishop : Piece
    {
        private readonly int[] Ranks = { 8, 7, 6, 5, 4, 3, 2, 1 };
        private const string BlackBishop = "\u265D";
        private const string WhiteBishop = "\u2657";
        private readonly Dictionary<(int, int), int> Squares = new Dictionary<(int, int), int>()
        {
            [(1, 1)] = -20,
            [(1, 2)] = -10,
            [(1, 3)] = -10,
            [(1, 4)] = -10,
            [(1, 5)] = -10,
            [(1, 6)] = -10,
            [(1, 7)] = -10,
            [(1, 8)] = -20,
            [(2, 1)] = -10,
            [(2, 2)] = 50,
            [(2, 3)] = 0,
            [(2, 4)] = 0,
            [(2, 5)] = 0,
            [(2, 6)] = 0,
            [(2, 7)] = 5,
            [(2, 8)] = -10,
            [(3, 1)] = -10,
            [(3, 2)] = 10,
            [(3, 3)] = 10,
            [(3, 4)] = 10,
            [(3, 5)] = 10,
            [(3, 6)] = 10,
            [(3, 7)] = 10,
            [(3, 8)] = -10,
            [(4, 1)] = -10,
            [(4, 2)] = 0,
            [(4, 3)] = 10,
            [(4, 4)] = 10,
            [(4, 5)] = 10,
            [(4, 6)] = 10,
            [(4, 7)] = 0,
            [(4, 8)] = -10,
            [(5, 1)] = -10,
            [(5, 2)] = 5,
            [(5, 3)] = 5,
            [(5, 4)] = 10,
            [(5, 5)] = 10,
            [(5, 6)] = 5,
            [(5, 7)] = 5,
            [(5, 8)] = -10,
            [(6, 1)] = -10,
            [(6, 2)] = 0,
            [(6, 3)] = 5,
            [(6, 4)] = 10,
            [(6, 5)] = 10,
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
            [(8, 4)] = -10,
            [(8, 5)] = -10,
            [(8, 6)] = -10,
            [(8, 7)] = -10,
            [(8, 8)] = -10,
        };

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
