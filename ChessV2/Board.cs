using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
namespace ChessV2
{
    public class Board
    {
        public List<Move> MoveHistory = new List<Move>();
        public int MoveCount = 0;
        public int PieceBalance = 0;
        public HashSet<Char> checkingPieces = new HashSet<char>();
        public HashSet<(int, int)> ProtectedSquares = new HashSet<(int, int)>();
        public HashSet<(int, int)> BlockCheckMoves = new HashSet<(int, int)>();
        public HashSet<(int, int)> IllegalKingMoves = new HashSet<(int, int)>();
        public List<Move> LegalMoves = new List<Move>();
        public int checkCount = 0;
        public HashSet<Piece> Pieces = new HashSet<Piece>();
        public HashSet<Piece> SlidingPieces = new HashSet<Piece>();
        public List<Piece> Kings = new List<Piece>();
        public Dictionary<(int, int), Piece> OccupiedSquares = new Dictionary<(int, int), Piece>();

        public bool turn = true;
        public Board()
        {
            SlidingPieces.Add(new Rook((1, 1), (1, 1), new List<Move>(), true));
            Pieces.Add(new Knight((2, 1), (2, 1), new List<Move>(), true));
            SlidingPieces.Add(new Bishop((3, 1), (3, 1), new List<Move>(), true));
            SlidingPieces.Add(new Queen((4, 1), (4, 1), new List<Move>(), true));
            SlidingPieces.Add(new Bishop((6, 1), (6, 1), new List<Move>(), true));
            Pieces.Add(new Knight((7, 1), (7, 1), new List<Move>(), true));
            SlidingPieces.Add(new Rook((8, 1), (8, 1), new List<Move>(), true));
            for (int i = 1; i < 9; i++)
            {
                Pieces.Add(new Pawn((i, 2), (i, 2), new List<Move>(), true));
            }
            SlidingPieces.Add(new Rook((1, 8), (1, 8), new List<Move>(), false));
            Pieces.Add(new Knight((2, 8), (2, 8), new List<Move>(), false));
            SlidingPieces.Add(new Bishop((3, 8), (3, 8), new List<Move>(), false));
            SlidingPieces.Add(new Queen((4, 8), (4, 8), new List<Move>(), false));
            SlidingPieces.Add(new Bishop((6, 8), (6, 8), new List<Move>(), false));
            Pieces.Add(new Knight((7, 8), (7, 8), new List<Move>(), false));
            SlidingPieces.Add(new Rook((8, 8), (8, 8), new List<Move>(), false));
            for (int i = 1; i < 9; i++)
            {
                Pieces.Add(new Pawn((i, 7), (i, 7), new List<Move>(), false));
            }

            Kings.Add(new King((5, 1), (5, 1), new List<Move>(), true));
            Kings.Add(new King((5, 8), (5, 8), new List<Move>(), false));

            BuildOccupiedSquares();

        }

        public void PrintLegalMoves()
        {
            foreach (Move legalmove in LegalMoves)
            {
                Console.Write($"{legalmove}, ");
            }
            Console.WriteLine();
        }

        public void ResetPieces()
        {
            foreach (Piece P in Pieces)
            {
                P.AIposition = P.Position;
            }
            foreach (Piece P in SlidingPieces)
            {
                P.AIposition = P.Position;
            }
            foreach (Piece P in Kings)
            {
                P.AIposition = P.Position;
            }
        }

        public void BuildOccupiedSquares()
        {
            foreach (Piece p in Pieces)
            {
                OccupiedSquares.Add(p.Position, p);
            }
            foreach (Piece P in SlidingPieces)
            {
                OccupiedSquares.Add(P.Position, P);
            }
            foreach (Piece P in Kings)
            {
                OccupiedSquares.Add(P.Position, P);
            }
        }

        public void OrderMoves()
        {
            List<Move> orderedMoves = new List<Move>();
            foreach (Move move in LegalMoves)
            {
                if (move.Capture)
                {
                    orderedMoves.Add(move);
                }
            }
            if (orderedMoves.Count == 0)
            {
                return;
            }
            foreach (Move move in LegalMoves)
            {
                if (!move.Capture)
                {
                    orderedMoves.Add(move);
                }
            }
            LegalMoves = orderedMoves;
        }

        public override string ToString()
        {
            StringBuilder board = new StringBuilder("A B C D E F G H\n");

            for (int rank = 8; rank > 0; rank--)
            {
                for (int file = 1; file < 9; file++)
                {
                    if (OccupiedSquares.ContainsKey((file, rank)))
                    {
                        board.Append(OccupiedSquares[(file, rank)].ToString());
                        board.Append(' ');
                    }
                    else
                    {
                        board.Append("  ");
                    }
                }
                board.Append($"{rank}\n");
            }
            return board.ToString();
        }

        public void GenerateMoves(Move lastMove)
        {
            foreach (Piece P in Pieces)
            {
                P.GenerateMoves(OccupiedSquares, P.GetMoves(), ref ProtectedSquares, (turn == P.Colour ? true : false), ref BlockCheckMoves, ref checkCount, ref IllegalKingMoves, (turn ? Kings[1] : Kings[0]), lastMove, ref checkingPieces);
            }
            foreach (Piece P in SlidingPieces)
            {
                foreach ((int, int) move in P.GetMoves())
                {
                    P.GenerateMoves(OccupiedSquares, P.GetNextMoves(move), ref ProtectedSquares, (turn == P.Colour ? true : false), ref BlockCheckMoves, ref checkCount, ref IllegalKingMoves, (P.Colour ? Kings[1] : Kings[0]), lastMove, ref checkingPieces);
                }
            }
            if (checkCount > 1)
            {
                BlockCheckMoves.Clear();
            }
            if (turn)
                {
                Kings[0].GenerateMoves(OccupiedSquares, Kings[0].GetMoves(), ref ProtectedSquares, true, ref BlockCheckMoves, ref checkCount, ref IllegalKingMoves, Kings[1], lastMove, ref checkingPieces);
                Kings[1].GenerateMoves(OccupiedSquares, Kings[1].GetMoves(), ref ProtectedSquares, false, ref BlockCheckMoves, ref checkCount, ref IllegalKingMoves, Kings[0], lastMove, ref checkingPieces);
                }
                else
                {
                Kings[1].GenerateMoves(OccupiedSquares, Kings[1].GetMoves(), ref ProtectedSquares, true, ref BlockCheckMoves, ref checkCount, ref IllegalKingMoves, Kings[1], lastMove, ref checkingPieces);
                Kings[0].GenerateMoves(OccupiedSquares, Kings[0].GetMoves(), ref ProtectedSquares, false, ref BlockCheckMoves, ref checkCount, ref IllegalKingMoves, Kings[0], lastMove, ref checkingPieces);
                }
            SelectLegalMoves();
        }

        public void SelectLegalMoves()
        {
            if (checkCount > 0)
            {
                if (checkCount > 1)
                {
                    Piece king = (turn ? Kings[0] : Kings[1]);
                    foreach (Move move in king.Moves)
                    {
                        LegalMoves.Add(move);
                    }
                }
                else
                {
                    foreach (Piece P in SlidingPieces)
                    {
                        if (P.Colour != turn)
                        {
                            continue;
                        }
                        foreach (Move move in P.Moves)
                        {
                            if (BlockCheckMoves.Contains(move.Coords))
                            {
                                LegalMoves.Add(move);
                            }
                        }
                    }
                    foreach (Piece P in Pieces)
                    {
                        if (P.Colour != turn)
                        {
                            continue;
                        }
                        foreach (Move move in P.Moves)
                        {
                            if (BlockCheckMoves.Contains(move.Coords))
                            {
                                LegalMoves.Add(move);
                            }
                        }
                    }
                    foreach (Piece P in Kings)
                    {
                        if (P.Colour != turn)
                        {
                            continue;
                        }
                        foreach (Move move in P.Moves)
                        {
                            if (BlockCheckMoves.Contains(move.Coords))
                            {
                                LegalMoves.Add(move);
                            }
                        }
                    }
                }
            }
            else
            {
                foreach (Piece P in SlidingPieces)
                {
                    if (P.Colour != turn)
                    {
                        continue;
                    }
                    foreach (Move move in P.Moves)
                    {
                        LegalMoves.Add(move);
                    }
                }
                foreach (Piece P in Pieces)
                {
                    if (P.Colour != turn)
                    {
                        continue;
                    }
                    foreach (Move move in P.Moves)
                    {
                        LegalMoves.Add(move);
                    }
                }
                foreach (Piece P in Kings)
                {
                    if (P.Colour != turn)
                    {
                        continue;
                    }
                    foreach (Move move in P.Moves)
                    {
                        LegalMoves.Add(move);
                    }
                }
            }
        }

        public bool MakeMove((char, (int, int), int, int) userInput)
        {
            foreach (Move move in LegalMoves)
            {

                if (move.EqualsInput(userInput))
                {
                    if (OccupiedSquares.ContainsKey(move.Coords))
                    {
                        Piece pieceToDelete = OccupiedSquares[move.Coords];
                        PieceBalance -= pieceToDelete.PointsValue;
                        if (SlidingPieces.Contains(pieceToDelete))
                        {
                            SlidingPieces.Remove(pieceToDelete);
                        }
                        if (Pieces.Contains(pieceToDelete))
                        {
                            Pieces.Remove(pieceToDelete);
                        }
                    }
                    if (move.P.EnPassant == true && OccupiedSquares.ContainsKey((move.Coords.Item1, move.Coords.Item2 + (turn ? -1 : 1))) && OccupiedSquares[(move.Coords.Item1, move.Coords.Item2 + (turn ? -1 : 1))].CharRep == 'P')
                    {
                        Pieces.Remove(OccupiedSquares[(move.Coords.Item1, move.Coords.Item2 + (turn ? -1 : 1))]);
                        OccupiedSquares.Remove((move.Coords.Item1, move.Coords.Item2 + (turn ? -1 : 1)));
                    }
                    if (move.Castle)
                    {
                        ExecuteCastle(move);
                    }
                    OccupiedSquares.Remove(move.P.Position);
                    move.P.Position = move.Coords;
                    move.P.AIposition = move.Coords;
                    OccupiedSquares[move.Coords] = move.P;
                    move.P.FirstMove = false;
                    MoveHistory.Add(move);
                    MoveCount += 1;
                    ClearAllMoves();
                    return true;
                }
            }
            return false;
        }

        public void ClearAllMoves()
        {
            foreach (Piece P in SlidingPieces)
            {
                P.Moves.Clear();
                P.IsPinned = false;
            }
            foreach (Piece P in Pieces)
            {
                P.Moves.Clear();
                P.EnPassant = false;
                P.IsPinned = false;
            }
            foreach (Piece P in Kings)
            {
                P.Moves.Clear();
                P.IsPinned = false;
            }
            LegalMoves.Clear();
            BlockCheckMoves.Clear();
            ProtectedSquares.Clear();
            IllegalKingMoves.Clear();
            checkingPieces.Clear();
            checkCount = 0;
        }

        private void ExecuteCastle(Move move)
        {
            if (move.Coords.Item1 == 7)
            {
                Piece rookToCastle = OccupiedSquares[(8, move.P.AIposition.Item2)];
                rookToCastle.Position = (6, move.P.AIposition.Item2);
                rookToCastle.AIposition = (6, move.P.AIposition.Item2);
                rookToCastle.FirstMove = false;
                OccupiedSquares.Remove((8, move.P.AIposition.Item2));
                OccupiedSquares[(6, move.P.AIposition.Item2)] = rookToCastle;
            }
            else
            {
                Piece rookToCastle = OccupiedSquares[(1, move.P.AIposition.Item2)];
                rookToCastle.Position = (4, move.P.AIposition.Item2);
                rookToCastle.AIposition = (4, move.P.AIposition.Item2);
                rookToCastle.FirstMove = false;
                OccupiedSquares.Remove((1, move.P.AIposition.Item2));
                OccupiedSquares[(4, move.P.AIposition.Item2)] = rookToCastle;
            }
        }
    }
}

