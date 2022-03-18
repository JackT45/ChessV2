using System;
using System.Collections.Generic;
namespace ChessV2
{
    public class MoveNode
    {
        public Move Move
        {
            get; set;
        }

        public List<MoveNode> Children
        {
            get; set;
        }

        public MoveNode Parent
        {
            get; set;
        }

        public int PointsValue
        {
            get; set;
        }

        public bool Terminal
        {
            get; set;
        }

        public MoveNode(Move move, List<MoveNode> children, MoveNode parent=null, int pointsValue = 0, bool terminal = false)
        {
            Move = move;
            Children = children;
            Parent = parent;
            PointsValue = pointsValue;
        }
    }
}
