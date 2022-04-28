using System;
using System.Collections.Generic;
namespace ChessV2
{
    public class Pawn : Piece
    {
        private readonly int[] Ranks = { 8, 7, 6, 5, 4, 3, 2, 1 };
        private const string BlackPawn = "\u265f";
        private const string WhitePawn = "\u2659";
        public List<(int, int)> FirstMovesWhite = new List<(int, int)> { (0, 1), (0, 2) };
        public List<(int, int)> BaseMovesWhite = new List<(int, int)> { (0, 1) };
        public List<(int, int)> FirstMovesBlack = new List<(int, int)> { (0, -1), (0, -2) };
        public List<(int, int)> BaseMovesBlack = new List<(int, int)> { (0, -1) };
        private readonly Dictionary<(int, int), int> Squares = new Dictionary<(int, int), int>()
        {
            [(1, 1)] = 0,
            [(1, 2)] = 0,
            [(1, 3)] = 0,
            [(1, 4)] = 0,
            [(1, 5)] = 0,
            [(1, 6)] = 0,
            [(1, 7)] = 0,
            [(1, 8)] = 0,
            [(2, 1)] = 5,
            [(2, 2)] = 10,
            [(2, 3)] = 10,
            [(2, 4)] = -20,
            [(2, 5)] = -20,
            [(2, 6)] = 10,
            [(2, 7)] = 10,
            [(2, 8)] = 5,
            [(3, 1)] = 5,
            [(3, 2)] = -5,
            [(3, 3)] = -10,
            [(3, 4)] = 0,
            [(3, 5)] = 0,
            [(3, 6)] = -10,
            [(3, 7)] = -5,
            [(3, 8)] = 5,
            [(4, 1)] = 0,
            [(4, 2)] = 0,
            [(4, 3)] = 0,
            [(4, 4)] = 20,
            [(4, 5)] = 20,
            [(4, 6)] = 0,
            [(4, 7)] = 0,
            [(4, 8)] = 0,
            [(5, 1)] = 5,
            [(5, 2)] = 5,
            [(5, 3)] = 10,
            [(5, 4)] = 25,
            [(5, 5)] = 25,
            [(5, 6)] = 10,
            [(5, 7)] = 5,
            [(5, 8)] = 5,
            [(6, 1)] = 10,
            [(6, 2)] = 10,
            [(6, 3)] = 20,
            [(6, 4)] = 30,
            [(6, 5)] = 30,
            [(6, 6)] = 20,
            [(6, 7)] = 10,
            [(6, 8)] = 10,
            [(7, 1)] = 50,
            [(7, 2)] = 50,
            [(7, 3)] = 50,
            [(7, 4)] = 50,
            [(7, 5)] = 50,
            [(7, 6)] = 50,
            [(7, 7)] = 50,
            [(7, 8)] = 50,
            [(8, 1)] = 0,
            [(8, 2)] = 0,
            [(8, 3)] = 0,
            [(8, 4)] = 0,
            [(8, 5)] = 0,
            [(8, 6)] = 0,
            [(8, 7)] = 0,
            [(8, 8)] = 0,
        };

        public Pawn((int, int) position, (int, int) aiposition, List<Move> moves, bool colour, Char charRep = 'P', int pointsValue=100, bool enPassant=false, bool king = false, bool firstMove=true, bool isPinned = false)
            :base(position, aiposition, moves, colour, charRep, pointsValue, enPassant, king, firstMove, isPinned)
        {
        }

        public override string ToString()
        {
            return Colour ? WhitePawn : BlackPawn;
        }

        public override List<(int, int)> GetMoves()
        {
            if (FirstMove)
            {
                return (Colour ? FirstMovesWhite : FirstMovesBlack);
            }
            return (Colour ? BaseMovesWhite : BaseMovesBlack);
        }

        public override void GenerateMoves(Dictionary<(int, int), Piece> occupiedSquares, List<(int, int)> moves, ref HashSet<(int, int)> protectedSquares, bool turn, ref HashSet<(int, int)> blockCheckMoves, ref int checkCount, ref HashSet<(int, int)> illegalKingMoves, Piece oppositeKing, Move lastMove, ref HashSet<char> checkingPieces)
        {
            PawnCaptures(occupiedSquares, ref protectedSquares, turn, ref blockCheckMoves, ref checkingPieces, ref checkCount);
            if (turn)
            {
                (int, int) moveToAdd;
                foreach ((int, int) move in moves)
                {
                    moveToAdd = GenNewPosition(move);
                    Console.WriteLine(moveToAdd);
                    if (CheckSameColour(occupiedSquares, moveToAdd))
                    {
                        break;
                    }
                    if (CheckMoveOnBoard(moveToAdd))
                    {
                        continue;
                    }
                    if (occupiedSquares.ContainsKey(moveToAdd))
                    {
                        break;
                    }
                    Moves.Add(new Move(this, moveToAdd, AIposition.Item1, AIposition.Item2));
                }
                if (lastMove.P.CharRep == 'P' && (lastMove.Coords.Item2 == 4 && lastMove.StartRank == 2) || (lastMove.Coords.Item2 == 5 && lastMove.StartRank == 7))
                {
                    EnPassantCheck((0, 0), occupiedSquares);
                }
                
            }

        }

        private void PawnCaptures(Dictionary<(int, int), Piece> occupiedSquares, ref HashSet<(int, int)> protectedSquares, bool turn, ref HashSet<(int, int)> blockCheckMoves, ref HashSet<char> checkingPieces, ref int checkCount)
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
                        checkCount += 1;
                        checkingPieces.Add(CharRep);
                    }
                    
                    if (turn)
                    {
                        Moves.Add(new Move(this, moveToAdd, AIposition.Item1, AIposition.Item2, true));
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
                Moves.Add(new Move(this, (posToCheck.Item1 ,posToCheck.Item2 + (Colour ? 1 : -1)), AIposition.Item1, AIposition.Item2, true));
            }
            posToCheck.Item1 -= 2;
            if (occupiedSquares.ContainsKey(posToCheck) && occupiedSquares[posToCheck].CharRep == 'P' && occupiedSquares[posToCheck].Colour != Colour)
            {
                occupiedSquares[posToCheck].EnPassant = true;
                EnPassant = true;
                Moves.Add(new Move(this, (posToCheck.Item1, posToCheck.Item2 + (Colour ? 1 : -1)), AIposition.Item1, AIposition.Item2, true));
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
