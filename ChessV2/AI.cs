using System;
using System.Collections.Generic;
namespace ChessV2
{
    public class AI
    {
        
        private readonly int MaxDepth = 5;
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
            changesToRevert.Add(new Move(move.P, (move.P.AIposition), move.P.AIposition.Item1, move.P.AIposition.Item2));
            gameboard.OccupiedSquares.Remove(move.P.AIposition);
            if (gameboard.OccupiedSquares.ContainsKey(move.Coords))
            {
                changesToRevert.Add(new Move(gameboard.OccupiedSquares[move.Coords], move.Coords, move.Coords.Item1, move.Coords.Item2));
                gameboard.OccupiedSquares.Remove(move.Coords);
            }
            gameboard.OccupiedSquares[move.Coords] = move.P;
            gameboard.MoveHistory.Add(move);
            gameboard.ClearAllMoves();
            return changesToRevert;
        }

        private void GenerateNewNode(Board gameboard, MoveNode currentNode, int depth, bool turn)
        {
            Dictionary<(int, int), Piece> CurrentOccupiedSquares = new Dictionary<(int, int), Piece>();
            foreach (KeyValuePair<(int, int), Piece> keyValuePair in gameboard.OccupiedSquares)
            {
                CurrentOccupiedSquares[keyValuePair.Key] = keyValuePair.Value;
            }
            gameboard.turn = turn;
            if (depth == MaxDepth)
            {
                return;
            }
            (int, int) currentPosition = currentNode.Move.P.AIposition;
            gameboard.GenerateMoves();
            gameboard.SelectLegalMoves();
            if (gameboard.LegalMoves.Count == 0 && currentNode != Root)
            {
                currentNode.Terminal = true;
                currentNode.PointsValue = int.MaxValue;
                return;
            }
            List<Move> AIMoves = new List<Move>();
            foreach (Move move in gameboard.LegalMoves)
            {
                AIMoves.Add(move);
            }
            foreach (Move move in AIMoves)
            {
                if (gameboard.OccupiedSquares.ContainsKey(move.Coords))
                {
                    Console.WriteLine(gameboard.OccupiedSquares[move.Coords].PointsValue);
                }
                int valueOfMove = gameboard.OccupiedSquares.ContainsKey(move.Coords) ? gameboard.OccupiedSquares[move.Coords].PointsValue : 0;
                MoveNode nextMove = new MoveNode(move, new List<MoveNode>(), currentNode, currentNode.PointsValue + (turn ?  - valueOfMove : valueOfMove));
                List<Move> changes = MakeMove(gameboard, move);
                GenerateNewNode(gameboard, nextMove, depth + 1, turn ? false : true);
                currentNode.Children.Add(nextMove);
                foreach (Move piece in changes)
                {
                    gameboard.OccupiedSquares[piece.Coords] = piece.P;
                    gameboard.OccupiedSquares[piece.Coords].AIposition = piece.Coords;
                }
                
            }
        }

        private void BuildTree(Board gameboard)
        {
            GenerateNewNode(gameboard, Root, 0, gameboard.turn);
        }

        private int MiniMax(MoveNode origin, int depth, bool maximiser, int alpha, int beta)
        {
            if (depth == MaxDepth || origin.Terminal)
            {
                return origin.PointsValue;
            }
            if (maximiser)
            {
                int value = int.MinValue;
                foreach (MoveNode move in origin.Children)
                {
                    int childValue = MiniMax(move, depth + 1, false, alpha, beta);
                    if (childValue > value)
                    {
                        value = childValue;
                        if (depth == 0)
                        {
                            BestMove = move.Move;
                        }
                        origin.PointsValue = childValue;
                        if (value >= beta)
                        {
                            break;
                        }
                        alpha = Math.Max(alpha, value);
                    }
                }
                origin.PointsValue = value;
                return value;
            }
            else
            {
                int value = int.MaxValue;
                foreach (MoveNode move in origin.Children)
                {
                    int childValue = MiniMax(move, depth + 1, true, alpha, beta);
                    if (childValue < value)
                    {
                        value = childValue;
                        origin.PointsValue = childValue;
                        
                        if (value <= alpha)
                        {
                            break;
                        }
                        beta = Math.Min(beta, value);
                    }
                }
                origin.PointsValue = value;
                return value;
            }
        }

        public void FindBestMove(Board gameboard)
        {
            BuildTree(gameboard);
            MiniMax(Root, 0, true, int.MinValue, int.MaxValue);
            foreach (MoveNode Child in Root.Children)
            {
                Console.WriteLine($"{Child.Move}, {Child.PointsValue}");
            }
            Root.Children.Clear();
            gameboard.ClearAllMoves();
            gameboard.OccupiedSquares.Clear();
            gameboard.BuildOccupiedSquares();
            gameboard.ResetMoveHistory();
            ActualMove(BestMove, gameboard);
            gameboard.ResetPieces();
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
            gameboard.OccupiedSquares[move.Coords] = move.P;
            gameboard.MoveHistory.Add(move);
        }
    }
}
