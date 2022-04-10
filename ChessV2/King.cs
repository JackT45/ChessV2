using System;
using System.Collections.Generic;
namespace ChessV2
{
    public class King : Piece
    {

        private const string WhiteKing = "\u2654";
        private const string BlackKing = "\u265A";

        public List<(int, int)> BaseMoves = new List<(int, int)> { (1, 0), (-1, 0), (1, 1), (-1, 1), (1, -1), (-1, -1), (0, 1), (0, -1) };


        public King( (int, int) position, (int, int) aiposition, List<Move> moves, bool colour, Char charRep = 'K', int pointsValue=20000, bool enPassant=false, bool king = true, bool firstMove=true, bool isPinned = false)
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

        public override void GenerateMoves(Dictionary<(int, int), Piece> occupiedSquares, List<(int, int)> moves, ref HashSet<(int, int)> protectedSquares, bool turn, ref HashSet<(int, int)> blockCheckMoves, ref int checkCount, ref HashSet<(int, int)> illegalKingMoves, Piece oppositeKing, Move lastMove, ref HashSet<char> checkingPieces)
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
                    Moves.Add(new Move(this, moveToAdd, AIposition.Item1, AIposition.Item2, CheckForCapture(occupiedSquares, moveToAdd) ? true : false));
                }
                else
                {
                    protectedSquares.Add(moveToAdd);
                }
                if (FirstMove && checkCount == 0)
                {
                    ShortCastle(occupiedSquares, protectedSquares);
                    LongCastle(occupiedSquares, protectedSquares);
                }
            }
        }

        private void ShortCastle(Dictionary<(int, int), Piece> occupiedSquares, HashSet<(int, int)> protectedSquares)
        {
            if (occupiedSquares.ContainsKey((6, AIposition.Item2)) || occupiedSquares.ContainsKey((7, AIposition.Item2)))
            {
                return;
            }
            if (protectedSquares.Contains((6, AIposition.Item2)) || protectedSquares.Contains((7, AIposition.Item2)))
            {
                return;
            }
            if (occupiedSquares.ContainsKey((8, AIposition.Item2)) && occupiedSquares[(8, AIposition.Item2)].FirstMove)
            {
                Moves.Add(new Move(this, (7, AIposition.Item2), AIposition.Item1, AIposition.Item2, false, true));
            } 
        }

        private void LongCastle(Dictionary<(int, int), Piece> occupiedSquares, HashSet<(int, int)> protectedSquares)
        {
            if (occupiedSquares.ContainsKey((4, AIposition.Item2)) || occupiedSquares.ContainsKey((3, AIposition.Item2)) || occupiedSquares.ContainsKey((2, AIposition.Item2)))
            {
                return;
            }
            if (protectedSquares.Contains((4, AIposition.Item2)) || protectedSquares.Contains((3, AIposition.Item2)))
            {
                return;
            }
            if (occupiedSquares.ContainsKey((1, AIposition.Item2)) && occupiedSquares[(1, AIposition.Item2)].FirstMove)
            {
                Moves.Add(new Move(this, (3, AIposition.Item2), AIposition.Item1, AIposition.Item2, false, true));
            }
        }
    }
}
