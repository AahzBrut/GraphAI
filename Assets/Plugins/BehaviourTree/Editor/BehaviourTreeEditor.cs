using System;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.UIElements;

namespace BehaviourTree.Editor
{
    public class BehaviourTreeEditor : EditorWindow
    {
        private BehaviourTreeView _treeView;
        private InspectorView _inspectorView;
        private IMGUIContainer _treeContextView;

        private SerializedObject _treeObject;
        private SerializedProperty _treeContextProperty;

        [MenuItem("BehaviourTree/Editor...")]
        public static void OpenWindow()
        {
            var treeEditor = GetWindow<BehaviourTreeEditor>();
            treeEditor.titleContent = new GUIContent("Behaviour Tree Editor");
        }

        public void CreateGUI()
        {
            var root = rootVisualElement;

            var visualTree =
                AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                    "Assets/Plugins/BehaviourTree/Editor/UI/BehaviourTreeEditor.uxml");
            visualTree.CloneTree(root);

            var styleSheet =
                AssetDatabase.LoadAssetAtPath<StyleSheet>(
                    "Assets/Plugins/BehaviourTree/Editor/Styles/BehaviourTreeEditor.uss");
            root.styleSheets.Add(styleSheet);

            _treeView = root.Q<BehaviourTreeView>();
            _inspectorView = root.Q<InspectorView>();
            _treeContextView = root.Q<IMGUIContainer>();
            _treeContextView.onGUIHandler = OnContextGUIHandler;
            _treeView.NodeSelected = OnNodeSelected;
            OnSelectionChange();
        }

        private void OnContextGUIHandler()
        {
            if (_treeObject == null) return;
            
            _treeObject.Update();
            EditorGUILayout.PropertyField(_treeContextProperty);
            _treeObject.ApplyModifiedProperties();
        }

        private void OnEnable()
        {
            EditorApplication.playModeStateChanged -= OnPlaymodeStateChanged;
            EditorApplication.playModeStateChanged += OnPlaymodeStateChanged;
        }

        private void OnPlaymodeStateChanged(PlayModeStateChange stateChange)
        {
            switch (stateChange)
            {
                case PlayModeStateChange.EnteredEditMode:
                    OnSelectionChange();
                    break;
                case PlayModeStateChange.ExitingEditMode:
                    break;
                case PlayModeStateChange.EnteredPlayMode:
                    OnSelectionChange();
                    break;
                case PlayModeStateChange.ExitingPlayMode:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(stateChange), stateChange, null);
            }
        }

        private void OnDisable()
        {
            EditorApplication.playModeStateChanged -= OnPlaymodeStateChanged;
        }

        [OnOpenAsset]
        public static bool OnOpenAsset(int instanceId, int lineId)
        {
            if (Selection.activeObject is BehaviourTree)
            {
                OpenWindow();
                return true;
            }

            return false;
        }

        private void OnSelectionChange()
        {
            var tree = Selection.activeObject as BehaviourTree;

            tree = ConnectToRunningTree(tree);
            PopulateTree(tree);

            if (tree != null)
            {
                _treeObject = new SerializedObject(tree);
                _treeContextProperty = _treeObject.FindProperty("treeContext");
            }
        }

        private void PopulateTree(BehaviourTree tree)
        {
            if (Application.isPlaying)
            {
                if (tree != null)
                {
                    _treeView?.PopulateView(tree);
                }
            }
            else
            {
                if (tree != null && AssetDatabase.CanOpenAssetInEditor(tree.GetInstanceID()))
                {
                    _treeView?.PopulateView(tree);
                }
            }
        }

        private static BehaviourTree ConnectToRunningTree(BehaviourTree tree)
        {
            if (tree == null)
            {
                if (Selection.activeGameObject)
                {
                    var runner = Selection.activeGameObject.GetComponent<BehaviourTreeRunner>();
                    if (runner != null)
                    {
                        tree = runner.CurrentTree;
                    }
                }
            }

            return tree;
        }

        private void OnNodeSelected(NodeView nodeView)
        {
            _inspectorView?.UpdateSelection(nodeView);
        }

        private void OnInspectorUpdate()
        {
            _treeView?.UpdateNodeStates();
        }
    }
}