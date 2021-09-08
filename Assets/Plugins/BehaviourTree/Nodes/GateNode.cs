using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree.Nodes
{
    public abstract class GateNode : Node
    {
        [HideInInspector] public Node childNode;

        public override Node Clone()
        {
            var clone = Instantiate(this);
            clone.childNode = childNode.Clone();
            return clone;
        }

        public override void AddChild(Node child)
        {
            childNode = child;
        }

        public override void RemoveChild(Node child)
        {
            childNode = null;
        }

        public override List<Node> GetChildren()
        {
            return childNode != null ? new List<Node> { childNode } : new List<Node>();
        }
    }
}