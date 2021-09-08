using System;
using System.Collections.Generic;
using BehaviourTree.Nodes;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using Node = BehaviourTree.Nodes.Node;

namespace BehaviourTree.Editor
{
    public class BehaviourTreeView : GraphView
    {
        public new class UxmlFactory : UxmlFactory<BehaviourTreeView, UxmlTraits>
        {
        }

        public Action<NodeView> NodeSelected = delegate { };
        private BehaviourTree _currentTree;

        public BehaviourTreeView()
        {
            AddElements();
            AddManipulators();
            AddStyleSheet();

            Undo.undoRedoPerformed += OnUndoRedoPerformed;
        }

        private void OnUndoRedoPerformed()
        {
            PopulateView(_currentTree);
            AssetDatabase.SaveAssets();
        }

        private void AddStyleSheet()
        {
            var styleSheet =
                AssetDatabase.LoadAssetAtPath<StyleSheet>(
                    "Assets/Plugins/BehaviourTree/Editor/Styles/BehaviourTreeEditor.uss");
            styleSheets.Add(styleSheet);
        }

        private void AddManipulators()
        {
            this.AddManipulator(new ContentZoomer());
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
        }

        private void AddElements()
        {
            Insert(0, new GridBackground());
        }

        internal void PopulateView(BehaviourTree tree)
        {
            _currentTree = tree;

            graphViewChanged -= OnGraphViewChanged;
            DeleteElements(graphElements);
            graphViewChanged += OnGraphViewChanged;

            if (tree.rootNode == null)
            {
                tree.rootNode = tree.CreateNode<RootNode>();
                EditorUtility.SetDirty(tree);
                AssetDatabase.SaveAssets();
            }

            CreateNodeViews();
            CreateEdges();
        }

        private NodeView GetNodeViewByNode(Node node) => GetNodeByGuid(node.guid) as NodeView;

        private void CreateEdges()
        {
            foreach (var parentNode in _currentTree.nodes)
            {
                foreach (var childNode in _currentTree.GetChildren(parentNode))
                {
                    var parentView = GetNodeViewByNode(parentNode);
                    var childView = GetNodeViewByNode(childNode);
                    var edge = parentView.OutputPort.ConnectTo(childView.InputPort);
                    AddElement(edge);
                }
            }
        }

        private void CreateNodeViews()
        {
            foreach (var treeNode in _currentTree.nodes)
            {
                CreateNodeView(treeNode);
            }
        }

        private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
        {
            DeleteRemovedElements(graphViewChange.elementsToRemove);
            AddCreatedEdges(graphViewChange.edgesToCreate);
            SortChildrenOfSelector(graphViewChange.movedElements);
            return graphViewChange;
        }

        private void SortChildrenOfSelector(List<GraphElement> movedElements)
        {
            if (movedElements == null) return;
            foreach (var graphElement in nodes)
            {
                var element = graphElement as NodeView;
                element?.SortChildren();
            }
        }

        private void AddCreatedEdges(List<Edge> edgesToCreate)
        {
            if (edgesToCreate == null) return;

            foreach (var edge in edgesToCreate)
            {
                var parentView = edge.output.node as NodeView;
                var childView = edge.input.node as NodeView;
                _currentTree.AddChild(parentView?.Node, childView?.Node);
            }
        }

        private void DeleteRemovedElements(List<GraphElement> elementsToRemove)
        {
            if (elementsToRemove == null) return;

            foreach (var element in elementsToRemove)
            {
                switch (element)
                {
                    case NodeView nodeView:
                        _currentTree.DeleteNode(nodeView.Node);
                        break;
                    case Edge edge:
                        var parentView = edge.output.node as NodeView;
                        var childView = edge.input.node as NodeView;
                        _currentTree.RemoveChild(parentView?.Node, childView?.Node);
                        break;
                }
            }
        }

        public override void BuildContextualMenu(ContextualMenuPopulateEvent menuEvent)
        {
            AddMenuActionsForNodeType(menuEvent, typeof(ActionNode));
            AddMenuActionsForNodeType(menuEvent, typeof(GateNode));
            AddMenuActionsForNodeType(menuEvent, typeof(SelectorNode));
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            var result = new List<Port>();
            foreach (var endPort in ports)
            {
                if (endPort.direction != startPort.direction && endPort.node != startPort.node)
                {
                    result.Add(endPort);
                }
            }

            return result;
        }

        private void AddMenuActionsForNodeType(ContextualMenuPopulateEvent menuEvent, Type nodeType)
        {
            var types = TypeCache.GetTypesDerivedFrom(nodeType);
            foreach (var type in types)
            {
                menuEvent.menu.AppendAction($"[{type.BaseType?.Name}] {type.Name}", a => CreateNode(type));
            }
        }

        private void CreateNode(Type type)
        {
            var node = _currentTree.CreateNode(type);
            CreateNodeView(node);
        }

        private void CreateNodeView(Node node)
        {
            var nodeView = new NodeView(node);
            nodeView.Init();
            nodeView.NodeSelected = NodeSelected;
            AddElement(nodeView);
        }

        public void UpdateNodeStates()
        {
            foreach (var node in nodes)
            {
                var nodeView = node as NodeView;
                nodeView?.UpdateState();
            }
        }
    }
}