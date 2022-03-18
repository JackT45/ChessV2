using System;
using System.Collections.Generic;
using System.Text;

namespace ChessV2
{
    public class Board
    {
        public List<Move> MoveHistory = new List<Move>();
        public int MoveCount = 0;
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
            SlidingPieces.Add(new Rook((1, 1), (1, 1), new List<(int, int)>(), true));
            Pieces.Add(new Knight((2, 1), (2, 1), new List<(int, int)>(), true));
            SlidingPieces.Add(new Bishop((3, 1), (3, 1), new List<(int, int)>(), true));
            SlidingPieces.Add(new Queen((4, 1), (4, 1), new List<(int, int)>(), true));
            SlidingPieces.Add(new Bishop((6, 1), (6, 1), new List<(int, int)>(), true));
            Pieces.Add(new Knight((7, 1), (7, 1), new List<(int, int)>(), true));
            SlidingPieces.Add(new Rook((8, 1), (8, 1), new List<(int, int)>(), true));
            for (int i = 1; i < 9; i++)
            {
                Pieces.Add(new Pawn((i, 2), (i, 2), new List<(int, int)>(), true));
            }
            SlidingPieces.Add(new Rook((1, 8), (1, 8), new List<(int, int)>(), false));
            Pieces.Add(new Knight((2, 8), (2, 8), new List<(int, int)>(), false));
            SlidingPieces.Add(new Bishop((3, 8), (3, 8), new List<(int, int)>(), false));
            SlidingPieces.Add(new Queen((4, 8), (4, 8), new List<(int, int)>(), false));
            SlidingPieces.Add(new Bishop((6, 8), (6, 8), new List<(int, int)>(), false));
            Pieces.Add(new Knight((7, 8), (7, 8), new List<(int, int)>(), false));
            SlidingPieces.Add(new Rook((8, 8), (8, 8), new List<(int, int)>(), false));
            for (int i = 1; i < 9; i++)
            {
                Pieces.Add(new Pawn((i, 7), (i, 7), new List<(int, int)>(), false));
            }

            Kings.Add(new King((5, 1), (5, 1), new List<(int, int)>(), true));
            Kings.Add(new King((5, 8), (5, 8), new List<(int, int)>(), false));

            BuildOccupiedSquares();

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

        public void BuildAIOccupiedSquares()
        {
            foreach (Piece p in Pieces)
            {
                OccupiedSquares.Add(p.AIposition, p);
            }
            foreach (Piece P in SlidingPieces)
            {
                OccupiedSquares.Add(P.AIposition, P);
            }
            foreach (Piece P in Kings)
            {
                OccupiedSquares.Add(P.AIposition, P);
            }
        }



        public void ResetMoveHistory()
        {
            List<Move> newMoveHistory = new List<Move>();
            for (int i = 0; i < MoveCount; i ++)
            {
                newMoveHistory.Add(MoveHistory[i]);
            }
            MoveHistory = newMoveHistory;
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

        public void GenerateMoves()
        {
            foreach (Piece P in Pieces)
            {
                P.GenerateMoves(OccupiedSquares, P.GetMoves(), ref ProtectedSquares, (turn == P.Colour ? true : false), ref BlockCheckMoves, ref checkCount, ref IllegalKingMoves, (P.Colour ? Kings[1] : Kings[0]), MoveHistory);
            }
            foreach (Piece P in SlidingPieces)
            {
                foreach ((int, int) move in P.GetMoves())
                {
                    P.GenerateMoves(OccupiedSquares, P.GetNextMoves(move), ref ProtectedSquares, (turn == P.Colour ? true : false), ref BlockCheckMoves, ref checkCount, ref IllegalKingMoves, (P.Colour ? Kings[1] : Kings[0]), MoveHistory);
                }
            }
            if (checkCount > 1)
            {
                BlockCheckMoves.Clear();
            }
            if (turn)
                {
                Kings[0].GenerateMoves(OccupiedSquares, Kings[0].GetMoves(), ref ProtectedSquares, true, ref BlockCheckMoves, ref checkCount, ref IllegalKingMoves, Kings[1], MoveHistory);
                Kings[1].GenerateMoves(OccupiedSquares, Kings[1].GetMoves(), ref ProtectedSquares, false, ref BlockCheckMoves, ref checkCount, ref IllegalKingMoves, Kings[0], MoveHistory);
                }
                else
                {
                Kings[1].GenerateMoves(OccupiedSquares, Kings[1].GetMoves(), ref ProtectedSquares, true, ref BlockCheckMoves, ref checkCount, ref IllegalKingMoves, Kings[1], MoveHistory);
                Kings[0].GenerateMoves(OccupiedSquares, Kings[0].GetMoves(), ref ProtectedSquares, false, ref BlockCheckMoves, ref checkCount, ref IllegalKingMoves, Kings[0], MoveHistory);
                }
        }

        public void SelectLegalMoves()
        {
            Move moveToAdd;
            if (checkCount > 0)
            {
                if (checkCount > 1)
                {
                    Piece king = (turn ? Kings[0] : Kings[1]);
                    foreach ((int, int) move in king.Moves)
                    {
                        moveToAdd = new Move(king, move, king.AIposition.Item1, king.AIposition.Item2);
                        LegalMoves.Add(moveToAdd);
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
                        foreach ((int, int) move in P.Moves)
                        {
                            if (BlockCheckMoves.Contains(move))
                            {
                                moveToAdd = new Move(P, move, P.AIposition.Item1, P.AIposition.Item2);
                                LegalMoves.Add(moveToAdd);
                            }
                        }
                    }
                    foreach (Piece P in Pieces)
                    {
                        if (P.Colour != turn)
                        {
                            continue;
                        }
                        foreach ((int, int) move in P.Moves)
                        {
                            if (BlockCheckMoves.Contains(move))
                            {
                                moveToAdd = new Move(P, move, P.AIposition.Item1, P.AIposition.Item2);
                                LegalMoves.Add(moveToAdd);
                            }
                        }
                    }
                    foreach (Piece P in Kings)
                    {
                        if (P.Colour != turn)
                        {
                            continue;
                        }
                        foreach ((int, int) move in P.Moves)
                        {
                            if (BlockCheckMoves.Contains(move))
                            {
                                moveToAdd = new Move(P, move, P.AIposition.Item1, P.AIposition.Item2);
                                LegalMoves.Add(moveToAdd);
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
                    foreach ((int, int) move in P.Moves)
                    {
                        moveToAdd = new Move(P, move, P.AIposition.Item1, P.AIposition.Item2);
                        LegalMoves.Add(moveToAdd);
                    }
                }
                foreach (Piece P in Pieces)
                {
                    if (P.Colour != turn)
                    {
                        continue;
                    }
                    foreach ((int, int) move in P.Moves)
                    {
                        moveToAdd = new Move(P, move, P.AIposition.Item1, P.AIposition.Item2);
                        LegalMoves.Add(moveToAdd);
                    }
                }
                foreach (Piece P in Kings)
                {
                    if (P.Colour != turn)
                    {
                        continue;
                    }
                    foreach ((int, int) move in P.Moves)
                    {
                        moveToAdd = new Move(P, move, P.AIposition.Item1, P.AIposition.Item2);
                        LegalMoves.Add(moveToAdd);
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
                    OccupiedSquares.Remove(move.P.Position);
                    move.P.Position = move.Coords;
                    move.P.AIposition = move.Coords;
                    OccupiedSquares[move.Coords] = move.P;
                    move.P.FirstMove = false;
                    MoveHistory.Add(move);
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
            }
            foreach (Piece P in Pieces)
            {
                P.Moves.Clear();
                P.EnPassant = false;
            }
            foreach (Piece P in Kings)
            {
                P.Moves.Clear();
            }
            LegalMoves.Clear();
            BlockCheckMoves.Clear();
            ProtectedSquares.Clear();
            IllegalKingMoves.Clear();
            checkCount = 0;
        }
    }
}

