using System;
using System.Collections.Generic;
using BehaviourTree.Nodes;
using UnityEditor;
using UnityEngine;
using static BehaviourTree.Nodes.Node;

namespace BehaviourTree
{
    [CreateAssetMenu]
    public class BehaviourTree : ScriptableObject
    {
        public Node rootNode;
        public State treeState = State.Running;
        public List<Node> nodes = new List<Node>();
        public TreeContext treeContext = new TreeContext();

        public State Update()
        {
            if (rootNode.state == State.Running)
            {
                treeState = rootNode.Update();
            }

            return treeState;
        }

        public BehaviourTree Clone()
        {
            var clone = Instantiate(this);
            clone.rootNode = rootNode.Clone();
            clone.nodes = new List<Node>();
            Traverse(clone.rootNode, node => clone.nodes.Add(node));
            return clone;
        }

        private void Traverse(Node node, Action<Node> visitor)
        {
            if (node == null) return;
            visitor.Invoke(node);
            foreach (var child in node.GetChildren())
            {
                Traverse(child, visitor);
            }
        }

        public void BindContext()
        {
            Traverse(rootNode, node => node.treeContext = treeContext);
        }

#if UNITY_EDITOR
        public Node CreateNode(Type type)
        {
            var node = CreateInstance(type) as Node;
            if (node)
            {
                node.name = type.Name;
                node.guid = GUID.Generate().ToString();
                Undo.RecordObject(this, "Behaviour Tree (Create Node)");

                nodes.Add(node);
            }
            if (!Application.isPlaying)
            {
                AssetDatabase.AddObjectToAsset(node, this);
            }
            Undo.RegisterCreatedObjectUndo(node, "Behaviour Tree (Create Node)");
            AssetDatabase.SaveAssets();
            return node;
        }
        
        public T CreateNode<T>() where T : Node
        {
            var node = CreateInstance<T>();
            if (node)
            {
                node.name = node.GetType().Name;
                node.guid = GUID.Generate().ToString();
                nodes.Add(node);
            }
            AssetDatabase.AddObjectToAsset(node, this);
            AssetDatabase.SaveAssets();
            return node;
        }

        public void DeleteNode(Node node)
        {
            Undo.RecordObject(this, "Behaviour Tree (Delete Node)");
            nodes.Remove(node);
            Undo.DestroyObjectImmediate(node);
            AssetDatabase.SaveAssets();
        }

        public void AddChild(Node parent, Node child)
        {
            if (!(parent is ActionNode))
            {
                Undo.RecordObject(parent, "Behaviour Tree (Add Child Node)");
                parent.AddChild(child);
                EditorUtility.SetDirty(parent);
            }
        }
        
        public void RemoveChild(Node parent, Node child)
        {
            if (!(parent is ActionNode))
            {
                Undo.RecordObject(parent, "Behaviour Tree (Remove Child Node)");
                parent.RemoveChild(child);
                EditorUtility.SetDirty(parent);
            }
        }
        
        public List<Node> GetChildren(Node node)
        {
            return node.GetChildren();
        }
#endif
    }
}