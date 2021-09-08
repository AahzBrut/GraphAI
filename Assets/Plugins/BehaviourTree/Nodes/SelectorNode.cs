using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree.Nodes
{
    public abstract class SelectorNode : Node
    {
        [HideInInspector] public List<Node> childNodes = new List<Node>();

        public override Node Clone()
        {
            var clone = Instantiate(this);
            clone.childNodes = new List<Node>();
            foreach (var childNode in childNodes)
            {
                clone.childNodes.Add(childNode.Clone());
            }
            return clone;
        }

        public override void AddChild(Node child)
        {
            childNodes.Add(child);
        }

        public override void RemoveChild(Node child)
        {
            childNodes.Remove(child);
        }

        public override List<Node> GetChildren()
        {
            return childNodes;
        }
    }
}