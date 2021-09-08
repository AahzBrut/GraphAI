using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree.Nodes
{
    public class RootNode : Node
    {
        [HideInInspector] public Node childNode;
        
        protected override void OnStart()
        {
        }

        protected override void OnStop()
        {
        }

        protected override State OnUpdate()
        {
            return childNode.Update();
        }

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