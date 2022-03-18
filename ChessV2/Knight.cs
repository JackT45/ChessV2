using System;
using System.Collections.Generic;
namespace ChessV2
{
    public class Knight : Piece
    {
        private const string BlackKnight = "\u265e";
        private const string WhiteKnight = "\u2658";

        public List<(int, int)> BaseMoves = new List<(int, int)> { (2, 1), (-2, 1), (-2, -1), (2, -1), (1, 2), (1, -2), (-1, 2), (-1, -2) };


        public Knight( (int, int) position, (int, int) aiposition, List<(int, int)> moves, bool colour, Char charRep = 'N', int pointsValue=3, bool enPassant=false, bool king = false, bool firstMove = false, bool isPinned = false)
            : base(position, aiposition, moves, colour, charRep, pointsValue, enPassant, king, firstMove, isPinned)
        {
        }

        public override string ToString()
        {
            if (Colour)
            {
                return WhiteKnight;
            }
            else
            {
                return BlackKnight;
            }
        }

        public override List<(int, int)> GetMoves()
        {
            return BaseMoves;
        }

        public override void GenerateMoves(Dictionary<(int, int), Piece> occupiedSquares, List<(int, int)> moves, ref HashSet<(int, int)> protectedSquares, bool turn, ref HashSet<(int, int)> blockCheckMoves, ref int checkCount, ref HashSet<(int, int)> illegalKingMoves, Piece oppositeKing, List<Move> moveHistory)
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
                    Moves.Add(moveToAdd);
                }
                else
                {
                    protectedSquares.Add(moveToAdd);
                }
                if (IsCheck(move, occupiedSquares))
                {
                    checkCount += 1;
                    blockCheckMoves.Clear();
                }
            }
        }
    }
}

