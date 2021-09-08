using System;
using BehaviourTree.Nodes;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Node = UnityEditor.Experimental.GraphView.Node;

namespace BehaviourTree.Editor
{
    public class NodeView : Node
    {
        public Action<NodeView> NodeSelected = delegate {  };
        public readonly Nodes.Node Node;
        public Port InputPort;
        public Port OutputPort;

        public NodeView(Nodes.Node node) : base("Assets/Plugins/BehaviourTree/Editor/UI/NodeView.uxml")
        {
            Node = node;
            viewDataKey = node.guid;
            style.left = node.position.x;
            style.top = node.position.y;

            CreateInputPorts();
            CreateOutputPorts();
            SetupClasses();
            SetupDescriptionBinding();
        }

        private void SetupDescriptionBinding()
        {
            var descriptionLabel = this.Q<Label>("description");
            
            if (descriptionLabel != null)
            {
                descriptionLabel.bindingPath = "description";
                descriptionLabel.Bind(new SerializedObject(Node));
            }
        }

        private void SetupClasses()
        {
            switch (Node)
            {
                case ActionNode _:
                    AddToClassList("action-node");
                    break;
                case GateNode _:
                    AddToClassList("gate-node");
                    break;
                case SelectorNode _:
                    AddToClassList("selector-node");
                    break;
                case RootNode _:
                    AddToClassList("root-node");
                    break;
            }
        }

        private void CreateOutputPorts()
        {
            OutputPort = Node switch
            {
                SelectorNode _ => InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Multi,
                    typeof(bool)),
                GateNode _ => InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Single,
                    typeof(bool)),
                RootNode _ => InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Single,
                    typeof(bool)),
                _ => OutputPort
            };

            if (OutputPort != null)
            {
                OutputPort.portName = string.Empty;
                OutputPort.style.flexDirection = FlexDirection.ColumnReverse;
                outputContainer.Add(OutputPort);
            }
        }

        private void CreateInputPorts()
        {
            if (Node is RootNode) return;
            
            InputPort = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(bool));
            InputPort.portName = string.Empty;
            InputPort.style.flexDirection = FlexDirection.Column;
            inputContainer.Add(InputPort);
        }

        public void Init()
        {
            title = Node.name;
        }

        public override void SetPosition(Rect newPos)
        {
            base.SetPosition(newPos);
            Undo.RecordObject(Node, "Behaviour Tree (Set Position)");
            Node.position = newPos.position;
            EditorUtility.SetDirty(Node);
        }

        public override void OnSelected()
        {
            base.OnSelected();
            NodeSelected.Invoke(this);
        }

        public void SortChildren()
        {
            var selector = Node as SelectorNode;
            if (selector != null)
            {
                selector.childNodes.Sort(HorizontalPositionComparator);
            }
        }

        private int HorizontalPositionComparator(Nodes.Node left, Nodes.Node right) => 
            left.position.x < right.position.x ? -1 : 1;

        public void UpdateState()
        {
            RemoveFromClassList("idle-state");
            RemoveFromClassList("running-state");
            RemoveFromClassList("failure-state");
            RemoveFromClassList("success-state");
            
            if (Application.isPlaying)
            {
                switch (Node.state)
                {
                    case Nodes.Node.State.Running when !Node.IsStarted:
                        AddToClassList("idle-state");
                        break;
                    case Nodes.Node.State.Running when Node.IsStarted:
                        AddToClassList("running-state");
                        break;
                    case Nodes.Node.State.Failure:
                        AddToClassList("failure-state");
                        break;
                    case Nodes.Node.State.Success:
                        AddToClassList("success-state");
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
}