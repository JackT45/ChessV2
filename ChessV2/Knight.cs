using System;
using System.Collections.Generic;
namespace ChessV2
{
    public class Knight : Piece
    {
        private readonly int[] Ranks = { 8, 7, 6, 5, 4, 3, 2, 1 };
        private const string BlackKnight = "\u265e";
        private const string WhiteKnight = "\u2658";
        private readonly Dictionary<(int, int), int> Squares = new Dictionary<(int, int), int>()
        {
            [(1, 1)] = -50,
            [(1, 2)] = -40,
            [(1, 3)] = -30,
            [(1, 4)] = -30,
            [(1, 5)] = -30,
            [(1, 6)] = -30,
            [(1, 7)] = -40,
            [(1, 8)] = -50,
            [(2, 1)] = -40,
            [(2, 2)] = -20,
            [(2, 3)] = 0,
            [(2, 4)] = 5,
            [(2, 5)] = 5,
            [(2, 6)] = 0,
            [(2, 7)] = -20,
            [(2, 8)] = -40,
            [(3, 1)] = -30,
            [(3, 2)] = 5,
            [(3, 3)] = 10,
            [(3, 4)] = 15,
            [(3, 5)] = 15,
            [(3, 6)] = 10,
            [(3, 7)] = 5,
            [(3, 8)] = -30,
            [(4, 1)] = -30,
            [(4, 2)] = 0,
            [(4, 3)] = 15,
            [(4, 4)] = 20,
            [(4, 5)] = 20,
            [(4, 6)] = 15,
            [(4, 7)] = 0,
            [(4, 8)] = -30,
            [(5, 1)] = -30,
            [(5, 2)] = 5,
            [(5, 3)] = 15,
            [(5, 4)] = 20,
            [(5, 5)] = 20,
            [(5, 6)] = 15,
            [(5, 7)] = 5,
            [(5, 8)] = -30,
            [(6, 1)] = -30,
            [(6, 2)] = 0,
            [(6, 3)] = 10,
            [(6, 4)] = 15,
            [(6, 5)] = 15,
            [(6, 6)] = 10,
            [(6, 7)] = 0,
            [(6, 8)] = -30,
            [(7, 1)] = -40,
            [(7, 2)] = -20,
            [(7, 3)] = 0,
            [(7, 4)] = 0,
            [(7, 5)] = 0,
            [(7, 6)] = 0,
            [(7, 7)] = -20,
            [(7, 8)] = -40,
            [(8, 1)] = -50,
            [(8, 2)] = -40,
            [(8, 3)] = -30,
            [(8, 4)] = -30,
            [(8, 5)] = -30,
            [(8, 6)] = -30,
            [(8, 7)] = -40,
            [(8, 8)] = -50,
        };

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

