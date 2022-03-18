using System;
using System.Collections.Generic;
using System.Linq;
namespace ChessV2
{
    public class Pawn : Piece
    {

        private const string BlackPawn = "\u265f";
        private const string WhitePawn = "\u2659";
        public List<(int, int)> FirstMovesWhite = new List<(int, int)> { (0, 1), (0, 2) };
        public List<(int, int)> BaseMovesWhite = new List<(int, int)> { (0, 1) };
        public List<(int, int)> FirstMovesBlack = new List<(int, int)> { (0, -1), (0, -2) };
        public List<(int, int)> BaseMovesBlack = new List<(int, int)> { (0, -1) };


        public Pawn((int, int) position, (int, int) aiposition, List<(int, int)> moves, bool colour, Char charRep = 'P', int pointsValue=1, bool enPassant=false, bool king = false, bool firstMove=true, bool isPinned = false)
            :base(position, aiposition, moves, colour, charRep, pointsValue, enPassant, king, firstMove, isPinned)
        {
        }

        public override string ToString()
        {
            if (Colour)
            {
                return WhitePawn;
            }
            else
            {
                return BlackPawn;
            }
        }

        public override List<(int, int)> GetMoves()
        {
            if (FirstMove)
            {
                return (Colour ? FirstMovesWhite : FirstMovesBlack);
            }
            return (Colour ? BaseMovesWhite : BaseMovesBlack);
        }

        public override void GenerateMoves(Dictionary<(int, int), Piece> occupiedSquares, List<(int, int)> moves, ref HashSet<(int, int)> protectedSquares, bool turn, ref HashSet<(int, int)> blockCheckMoves, ref int checkCount, ref HashSet<(int, int)> illegalKingMoves, Piece oppositeKing, List<Move> moveHistory)
        {
            PawnCaptures(occupiedSquares, ref protectedSquares, turn, ref blockCheckMoves);
            if (turn)
            {
                (int, int) moveToAdd;
                foreach ((int, int) move in moves)
                {
                    moveToAdd = GenNewPosition(move);
                    if (CheckSameColour(occupiedSquares, moveToAdd))
                    {
                        continue;
                    }
                    if (CheckMoveOnBoard(moveToAdd))
                    {
                        continue;
                    }
                    if (occupiedSquares.ContainsKey(moveToAdd))
                    {
                        continue;
                    }
                    Moves.Add(moveToAdd);
                }
                if (moveHistory.Count > 0)
                {
                    Move lastMove = moveHistory.Last();
                    if (lastMove.P.CharRep == 'P' && (lastMove.Coords.Item2 == 4 && lastMove.StartRank == 2) || (lastMove.Coords.Item2 == 5 && lastMove.StartRank == 7))
                    {
                        EnPassantCheck((0, 0), occupiedSquares);
                    }
                }
            }

        }

        private void PawnCaptures(Dictionary<(int, int), Piece> occupiedSquares, ref HashSet<(int, int)> protectedSquares, bool turn, ref HashSet<(int, int)> blockCheckMoves)
        {
            (int, int)[] whiteCaptures = new (int, int)[] { (-1, 1), (1, 1) };
            (int, int)[] blackCaptures = new (int, int)[] { (-1, -1), (1, -1) };
            foreach ((int, int) move in (Colour ? whiteCaptures : blackCaptures))
            {
                (int, int) moveToAdd = (AIposition.Item1 + move.Item1, AIposition.Item2 + move.Item2);
                if (occupiedSquares.ContainsKey(moveToAdd))
                {
                    if (CheckSameColour(occupiedSquares, moveToAdd))
                    {
                        continue;
                    }
                    if (IsCheck(moveToAdd, occupiedSquares))
                    {
                        blockCheckMoves.Add(Position);
                    }
                    
                    if (turn)
                    {
                        Moves.Add(moveToAdd);
                    }
                }
                if (!turn && CheckMoveOnBoard(moveToAdd))
                {
                    protectedSquares.Add(moveToAdd);
                }
            }
        }

        public override void EnPassantCheck((int, int) position, Dictionary<(int, int), Piece> occupiedSquares)
        {
            (int, int) posToCheck = (AIposition.Item1 + 1, AIposition.Item2);
            if (occupiedSquares.ContainsKey(posToCheck) && occupiedSquares[posToCheck].CharRep == 'P' && occupiedSquares[posToCheck].Colour != Colour)
            {
                occupiedSquares[posToCheck].EnPassant = true;
                EnPassant = true;
                Moves.Add((posToCheck.Item1 ,posToCheck.Item2 + (Colour ? 1 : -1)));
            }
            posToCheck.Item1 -= 2;
            if (occupiedSquares.ContainsKey(posToCheck) && occupiedSquares[posToCheck].CharRep == 'P' && occupiedSquares[posToCheck].Colour != Colour)
            {
                occupiedSquares[posToCheck].EnPassant = true;
                EnPassant = true;
                Moves.Add((posToCheck.Item1, posToCheck.Item2 + (Colour ? 1 : -1)));
            }
        }
    }   
}
