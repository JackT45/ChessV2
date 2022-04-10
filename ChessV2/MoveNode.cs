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

        public double Value
        {
            get; set;
        }

        public MoveNode(Move move, List<MoveNode> children, double value = 0, MoveNode parent = null)
        {
            Move = move;
            Children = children;
            Parent = parent;
            Value = value;
        }
    }
}
