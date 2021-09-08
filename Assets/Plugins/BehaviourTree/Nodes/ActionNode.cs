using System.Collections.Generic;

namespace BehaviourTree.Nodes
{
    public abstract class ActionNode : Node
    {
        public override void AddChild(Node child)
        {
            throw new System.InvalidOperationException();
        }

        public override void RemoveChild(Node child)
        {
            throw new System.InvalidOperationException();
        }

        public override List<Node> GetChildren()
        {
            return new List<Node>();
        }
    }
}