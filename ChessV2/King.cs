using System;
using System.Collections.Generic;
namespace ChessV2
{
    public class King : Piece
    {

        private const string WhiteKing = "\u2654";
        private const string BlackKing = "\u265A";

        public List<(int, int)> BaseMoves = new List<(int, int)> { (1, 0), (-1, 0), (1, 1), (-1, 1), (1, -1), (-1, -1), (0, 1), (0, -1) };


        public King( (int, int) position, (int, int) aiposition, List<(int, int)> moves, bool colour, Char charRep = 'K', int pointsValue=50, bool enPassant=false, bool king = true, bool firstMove=true, bool isPinned = false)
            : base(position, aiposition, moves, colour, charRep, pointsValue, enPassant, king, firstMove, isPinned)
        {
        }

        public override string ToString()
        {
            if (Colour)
            {
                return WhiteKing;
            }
            else
            {
                return BlackKing;
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
                if (protectedSquares.Contains(moveToAdd))
                {
                    continue;
                }
                if (illegalKingMoves.Contains(moveToAdd))
                {
                    continue;
                }
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
            }
        }
    }
}
