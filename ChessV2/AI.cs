using System;
using System.Collections.Generic;
namespace ChessV2
{
    public class AI
    {
        private readonly int MaxDepth = 6;
        private int NodesSearched = 0;
        private Move BestMove;

        private MoveNode Root
        {
            get; set;
        }

        public AI(MoveNode root)
        {
            Root = root;
        }

        private List<Move> MakeMove(Board gameboard, Move move)
        {
            List<Move> changesToRevert = new List<Move>();
            changesToRevert.Add(new Move(move.P, move.P.AIposition, move.P.AIposition.Item1, move.P.AIposition.Item2, false, move.Castle));
            gameboard.OccupiedSquares.Remove(move.P.AIposition);
            if (gameboard.OccupiedSquares.ContainsKey(move.Coords))
            {
                changesToRevert.Add(new Move(gameboard.OccupiedSquares[move.Coords], move.Coords, move.Coords.Item1, move.Coords.Item2));
                gameboard.OccupiedSquares.Remove(move.Coords);
                
            }
            if (move.P.EnPassant && gameboard.OccupiedSquares.ContainsKey((move.Coords.Item1, move.Coords.Item2 + (gameboard.turn ? -1 : 1))) && gameboard.OccupiedSquares[(move.Coords.Item1, move.Coords.Item2 + (gameboard.turn ? -1 : 1))].CharRep == 'P')
            {
                changesToRevert.Add(new Move(gameboard.OccupiedSquares[(move.Coords.Item1, move.Coords.Item2 + (gameboard.turn ? -1 : 1))], (move.Coords.Item1, move.Coords.Item2 + (gameboard.turn ? -1 : 1)), move.Coords.Item1, move.Coords.Item2 + (gameboard.turn ? -1 : 1)));
                gameboard.OccupiedSquares.Remove((move.Coords.Item1, move.Coords.Item2 + (gameboard.turn ? -1 : 1)));
            }
            if (move.Castle)
            {
                if (move.Coords.Item1 == 7)
                {
                    Piece rookToCastle = gameboard.OccupiedSquares[(8, move.P.AIposition.Item2)];
                    changesToRevert.Add(new Move(rookToCastle, rookToCastle.AIposition, rookToCastle.AIposition.Item1, rookToCastle.AIposition.Item2, false, true));
                    rookToCastle.AIposition = (6, move.P.AIposition.Item2);
                    gameboard.OccupiedSquares.Remove((8, move.P.AIposition.Item2));
                    gameboard.OccupiedSquares[(6, move.P.AIposition.Item2)] = rookToCastle;
                }
                else
                {
                    Piece rookToCastle = gameboard.OccupiedSquares[(1, move.P.AIposition.Item2)];
                    changesToRevert.Add(new Move(rookToCastle, rookToCastle.AIposition, rookToCastle.AIposition.Item1, rookToCastle.AIposition.Item2, false, true));
                    rookToCastle.AIposition = (4, move.P.AIposition.Item2);
                    gameboard.OccupiedSquares.Remove((1, move.P.AIposition.Item2));
                    gameboard.OccupiedSquares[(4, move.P.AIposition.Item2)] = rookToCastle;
                }
            }
            gameboard.OccupiedSquares[move.Coords] = move.P;
            move.P.AIposition = move.Coords;
            // This not optimal, probably introduces bugs to move gen with regard to castles
            // Considering implementing event for Rook.Position changing to limit searching for castles replacing rook.FirstMove
            if (move.P.CharRep != 'R')
            {
                move.P.FirstMove = false;
            }
            gameboard.ClearAllMoves();
            return changesToRevert;
        }

        private void RevertChanges(Board gameboard, List<Move> changes)
        {
            foreach (Move piece in changes)
            {
                if (gameboard.OccupiedSquares.ContainsKey(piece.P.AIposition))
                {
                    gameboard.OccupiedSquares.Remove(piece.P.AIposition);
                }
                if (piece.P.CharRep == 'P')
                {
                    if (piece.P.Colour && piece.StartRank == 2)
                    {
                        piece.P.FirstMove = true;
                    }
                    if (!piece.P.Colour && piece.StartRank == 7)
                    {
                        piece.P.FirstMove = true;
                    }
                }
                piece.P.EnPassant = false;
                if (piece.Castle)
                {
                    piece.P.FirstMove = true;
                }
                if (piece.P.King)
                {
                    if (piece.P.Colour && gameboard.WhiteCaste && piece.Coords == (5, 1))
                    {
                        piece.P.FirstMove = true;
                    }
                    if (!piece.P.Colour && gameboard.BlackCastle && piece.Coords == (5, 8))
                    {
                        piece.P.FirstMove = true;
                    }
                }
                gameboard.OccupiedSquares[piece.Coords] = piece.P;
                gameboard.OccupiedSquares[piece.Coords].AIposition = piece.Coords;
            }
        }

        private void AddNodesChildren(Board gameboard, MoveNode currentNode, int depth)
        {
            gameboard.ClearAllMoves();
            gameboard.GenerateMoves(currentNode.Move);
            gameboard.OrderMoves();
            foreach (Move move in gameboard.LegalMoves)
            {
                if (gameboard.MoveCount + depth < 5)
                {
                    if (move.P.CharRep == 'R')
                    {
                        continue;
                    }
                    if (depth != 0 && move.P == currentNode.Parent.Move.P)
                    {
                        continue;
                    }
                    
                }
                MoveNode nextMove = new MoveNode(move, new List<MoveNode>(), currentNode.Move.P.Colour ? int.MinValue : int.MaxValue, currentNode);
                currentNode.Children.Add(nextMove);
            }
        }

        private double GenerateTree(Board gameboard, MoveNode currentNode, int depth, bool maximiser, double alpha=int.MinValue, double beta=int.MaxValue)
        {
            
            if (depth == MaxDepth)
            {
                NodesSearched += 1;
                return Evaluate(gameboard, currentNode);
            }
            gameboard.turn = !currentNode.Move.P.Colour;
            AddNodesChildren(gameboard, currentNode, depth);
            if (maximiser)
            {
                double value = int.MinValue;
                foreach (MoveNode child in currentNode.Children)
                {
                    List<Move> changes = MakeMove(gameboard, child.Move);
                    double ChildValue = GenerateTree(gameboard, child, depth + 1, !maximiser, alpha, beta);
                    value = Math.Max(value, ChildValue);
                    RevertChanges(gameboard, changes);
                    if (value >= beta)
                    {
                        break;
                    }
                    alpha = Math.Max(alpha, value);
                    child.Value = ChildValue;
                }
                return value;
            }
            else
            {
                
                double value = int.MaxValue;
                foreach (MoveNode child in currentNode.Children)
                {
                    List<Move> changes = MakeMove(gameboard, child.Move);
                    double ChildValue = GenerateTree(gameboard, child, depth + 1, !maximiser, alpha, beta);
                    value = Math.Min(value, ChildValue);
                    RevertChanges(gameboard, changes);
                    if (value <= alpha)
                    {
                        break;
                    }
                    beta = Math.Min(beta, value);
                    child.Value = ChildValue;

                }
                return value;
            }
            
        }

        private double Evaluate(Board gameboard, MoveNode currentNode)
        {
            //This needs fleshing out, include transposition tables and space advantage
            //currently only applies to black as the maximising player
            //consider each piece class containing an evaluation function
            double pieceBalance = 0;
            foreach (KeyValuePair<(int, int), Piece> kvp in gameboard.OccupiedSquares)
            {
                if (kvp.Value.Colour)
                {
                    pieceBalance -= kvp.Value.Evaluate();
                }
                else
                {
                    pieceBalance += kvp.Value.Evaluate();
                }
            }
            //if (currentNode.Move.P.CharRep != 'P' && gameboard.ProtectedSquares.Contains(currentNode.Move.Coords))
            //{
            //    if (currentNode.Move.P.Colour)
            //    {
            //        pieceBalance -= currentNode.Move.P.PointsValue;
            //    }
            //    else
            //    {
            //        pieceBalance += currentNode.Move.P.PointsValue;
            //    }
            //}
            return pieceBalance;
        }

        public void FindBestMove(Board gameboard)
        {
            Root.Value = GenerateTree(gameboard, Root, 0, true);
            Console.WriteLine(Root.Value);
            foreach (MoveNode child in Root.Children)
            {
                Console.WriteLine($"{child.Move}, {child.Value}");
                if (child.Value == Root.Value)
                {
                    BestMove = child.Move;
                    break;
                }
            }
            Root.Children.Clear();
            gameboard.ClearAllMoves();
            gameboard.OccupiedSquares.Clear();
            gameboard.BuildOccupiedSquares();
            ActualMove(BestMove, gameboard);
            gameboard.ResetPieces();
            Console.WriteLine($"Nodes Searched : {NodesSearched}");
            NodesSearched = 0;
        }

        private void ActualMove(Move move, Board gameboard)
        {
            gameboard.OccupiedSquares.Remove(move.P.Position);
            move.P.Position = move.Coords;
            move.P.AIposition = move.Coords;
            if (gameboard.OccupiedSquares.ContainsKey(move.Coords))
            {
                if (gameboard.SlidingPieces.Contains(gameboard.OccupiedSquares[move.Coords]))
                {
                    gameboard.SlidingPieces.Remove(gameboard.OccupiedSquares[move.Coords]);
                }
                if (gameboard.Pieces.Contains(gameboard.OccupiedSquares[move.Coords]))
                {
                    gameboard.Pieces.Remove(gameboard.OccupiedSquares[move.Coords]);
                }
            }
            if (move.Castle)
            {
                gameboard.ExecuteCastle(move);
                if (move.P.Colour)
                {
                    gameboard.WhiteCaste = false;
                }
                else
                {
                    gameboard.BlackCastle = false;
                }
            }
            gameboard.OccupiedSquares[move.Coords] = move.P;
            move.P.FirstMove = false;
            gameboard.MoveHistory.Add(move);
            gameboard.MoveCount += 1;
            gameboard.OccupiedSquares.Clear();
            gameboard.BuildOccupiedSquares();
        }

    }
}
